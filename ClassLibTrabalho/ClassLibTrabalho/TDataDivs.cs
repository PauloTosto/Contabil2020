using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;
using ClassConexao;

namespace ClassLibTrabalho
{
    public class TDataDivs
    {
        static public OleDbDataReader Get_ParceirosPeriodo(DateTime data)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            string oprod = "  1";
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            strOleDb = "SELECT distinct parceiro.cod as CODIGO, prgarea.ncontra, prgarea.cod, prgarea.gleba , parceiro.nome, prgarea.inicio, prgarea.fim" +
              " FROM " + path + "PARCEIRO, " + path + "PRGAREA " +
              "WHERE   (PROD = '" + oprod + "') AND( prgarea.cod = parceiro.cod) AND (CTOD(" + TDataControlReduzido.FormatDataGravar(data) + ") BETWEEN prgarea.inicio AND prgarea.fim)";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                return OleDbcomm.ExecuteReader();
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }

        }
        static public OleDbDataReader Get_ParceirosProducao(DateTime inicio, DateTime fim)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            string oprod = "  1";
            strOleDb = "SELECT codparc, SUM(quant_pa) AS TOTAL, SUM(quant_fr) AS TOTALEMP, SUM(quant_ri) AS TOTALRI, ncontra " +
              " FROM " + path + "PRODUTO " +
              "WHERE (PROD = '" + oprod + "')  AND (DATAP  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              "GROUP BY codparc, ncontra " +
              " ORDER BY codparc, ncontra ";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                return OleDbcomm.ExecuteReader();
                
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }



        static public DataSet Get_CLTPontosPeriodo(DateTime inicio, DateTime fim, string tipo)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            strOleDb = "Select SETOR,FAZMOV,BL,NUM_MOD,CODSER,DATA,TRAB ,OK,DIA1,DIA2,DIA3,DIA4,DIA5,DIA6,DIA7,QUANT,NOTURNO  FROM " +
                    path + "CLTPONTO " +
                    "WHERE (TIPOMOV = '" + tipo + "')  AND (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
                     " AND ( BL <> '' ) AND ( CODSER <> '' )  " +
                         "AND  ( (FAZMOV <> '') OR (NUM_MOD <> '')  " +
                        " OR (DIA1 <> '') OR (DIA2 <> '') OR (DIA3 <> '') OR (DIA4 <> '') " +
                       " OR (DIA5 <> '') OR (DIA6 <> '') OR (DIA7 <> '')) " +

                     " ORDER BY BL,CODSER,DATA DESC,OK DESC ";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbDataReader leitor = OleDbcomm.ExecuteReader();

                DataTable dtcltponto = new DataTable("CLTPONTO");

                if (leitor.HasRows)
                {
                    DataTable esquema = leitor.GetSchemaTable();
                    for (int i = 0; i < esquema.Rows.Count; i++)
                    {
                        DataRow row = esquema.Rows[i];
                        int tam = 0;
                        if (System.Type.GetType(row["DataType"].ToString()) == typeof(string))
                            tam = Convert.ToInt16(row["ColumnSize"]);
                        dtcltponto.Columns.Add(TDataControlReduzido.Coluna(System.Type.GetType(row["DataType"].ToString()), row["ColumnName"].ToString(), tam, false));
                    }
                    dtcltponto.Columns.Add(TDataControlReduzido.Coluna(typeof(Double), "HEfetivas", 0, false));

                    //DateTime linicio = TAtualizaFolha.DiaDoPonto(inicio, TAtualizaFolha._DIADASEMANA);//ENCodeDate(ano,mes,1);
                    //DateTime lfim = TAtualizaFolha.DiaDoPonto(fim, TAtualizaFolha._DIAFIMDASEMANA);  //UltimoDiaMes(tdata);

                    Dictionary<DateTime, TObjPonto> ListPonto = TAtualizaFolha.Peg_Ponto_LongoPeriodo(inicio, tipo);
                    Dictionary<string, TCLTCodigo> ListCodigo = TAtualizaFolha.GetDict_CLTCodigo();
                    TObjPonto oponto = null;
                    DateTime comparadta = DateTime.MinValue;
                    DateTime ultimopontolido = DateTime.MinValue;
                    DateTime pontofim = DateTime.MinValue;
                    DateTime chavefim = DateTime.MinValue;
                    DateTime chaveinicio = DateTime.MinValue;


                    DataRow orow = null;
                    string bl_codser = "";
                    while (leitor.Read())
                    {
                        if (bl_codser != (leitor["BL"].ToString() + leitor["CODSER"].ToString()))
                        {
                            if (leitor["OK"].ToString().Trim() != "X")
                                continue;
                            bl_codser = (leitor["BL"].ToString() + leitor["CODSER"].ToString());
                        }
                        // calculo de horas efetivas
                        if (comparadta != Convert.ToDateTime(leitor["DATA"]))
                        {
                            comparadta = Convert.ToDateTime(leitor["DATA"]);
                            oponto = ListPonto[comparadta.Date];
                            if (oponto == null)
                            {
                                MessageBox.Show("Erro pegada dias ponto");
                                continue;
                            }

                        }
                        decimal horas_efetivas = 0;
                        int ultimodia_efetivo = 0;
                        int primeirodia_efetivo = 0;
                        int j;
                        for (j = oponto.diainicial; j <= oponto.diafinal; j++)
                        {
                            string campo = "DIA" + j.ToString().Trim();
                            if ((leitor[campo] == null) || (leitor[campo].ToString().Trim() == "")) continue;
                            TCLTCodigo ocodigo = ListCodigo[leitor[campo].ToString()];
                            // DataRow orow = dtCLTCodig.Rows.Find(ponto[campo]);
                            if (ocodigo == null)
                            {
                                MessageBox.Show("Tabela de Codigo Incoerente :" + Convert.ToString(leitor[campo]));
                                continue;
                            }

                            if (ocodigo.horas_efetivas > 0)
                            {
                                if (primeirodia_efetivo == 0)
                                    primeirodia_efetivo = j;
                                if (((oponto.inddomingo - 1) == j) && (ocodigo.indcod == "X "))

                                    horas_efetivas = horas_efetivas + (ocodigo.horas_efetivas / 2M);
                                else
                                    horas_efetivas = horas_efetivas + ocodigo.horas_efetivas;
                                ultimodia_efetivo = j;
                            }
                        }
                        if (horas_efetivas == 0)
                        {
                            continue;
                        }


                        orow = dtcltponto.Rows.Add();
                        for (int i = 0; i < leitor.FieldCount; i++)
                        {
                            orow[i] = leitor[i];
                        }
                        orow["Hefetivas"] = (object)horas_efetivas;



                    }
                }
                leitor.Close();
                DataSet result = new DataSet();
                result.Tables.Add(dtcltponto);
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }

        }

        static public DataSet GeraValor_CLTPontosPeriodo(DateTime inicio, DateTime fim, string tipo)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            string lit_tipo;
            if (tipo == "")
                lit_tipo = "";
            else
                lit_tipo = " (TIPOMOV = '" + tipo + "')  AND ";

            strOleDb = "Select SETOR,FAZMOV,BL,NUM_MOD,CODSER,DATA,TRAB ,OK,DIA1,DIA2,DIA3,DIA4,DIA5,DIA6,DIA7," +
                "QUANT,semana, valor,DIARIA, dta_ini, dta_fim, obs,NOTURNO   FROM " +
                    path + "CLTPONTO " +
                    "WHERE " + lit_tipo + " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") ) "
                 + "AND  ( (FAZMOV <> '') OR (NUM_MOD <> '')  OR (BL <> '') OR (CODSER <> '') " +
                        " OR (DIA1 <> '') OR (DIA2 <> '') OR (DIA3 <> '') OR (DIA4 <> '') " +
                       " OR (DIA5 <> '') OR (DIA6 <> '') OR (DIA7 <> '')) " +

                     " ORDER BY DATA ,TRAB ";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbDataReader leitor = OleDbcomm.ExecuteReader();

                DataTable dtcltponto = new DataTable("CLTPONTO");

                if (leitor.HasRows)
                {
                    DataTable esquema = leitor.GetSchemaTable();
                    for (int i = 0; i < esquema.Rows.Count; i++)
                    {
                        DataRow row = esquema.Rows[i];
                        int tam = 0;
                        if (System.Type.GetType(row["DataType"].ToString()) == typeof(string))
                            tam = Convert.ToInt16(row["ColumnSize"]);
                        dtcltponto.Columns.Add(TDataControlReduzido.Coluna(System.Type.GetType(row["DataType"].ToString()), row["ColumnName"].ToString(), tam, false));
                    }
                    // dtcltponto.Columns.Add(TDataControlReduzido.Coluna(typeof(Double), "HEfetivas", 0, false));


                    // TObjPonto oponto = null;
                    DateTime comparadta = DateTime.MinValue;
                    DateTime ultimopontolido = DateTime.MinValue;
                    DateTime pontofim = DateTime.MinValue;
                    DateTime chavefim = DateTime.MinValue;
                    DateTime chaveinicio = DateTime.MinValue;


                    DataRow orow = null;
                    // string bl_codser = "";
                    while (leitor.Read())
                    {
                        orow = dtcltponto.Rows.Add();
                        for (int i = 0; i < leitor.FieldCount; i++)
                        {
                            orow[i] = leitor[i];
                        }
                    }
                }
                leitor.Close();
                DataSet result = new DataSet();
                result.Tables.Add(dtcltponto);
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }

        }



        static public DataSet TabelasCLT()
        {

            OleDbCommand OleDbcomm;
            OleDbDataReader leitor;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result;
            try
            {
                result = new DataSet();

                strOleDb = " SELECT  modelo.* FROM " + path + "MODELO";
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                leitor = OleDbcomm.ExecuteReader();

                DataTable dtmodelo = new DataTable("MODELO");

                if (leitor.HasRows)
                {
                    DataTable esquema = leitor.GetSchemaTable();
                    
                    for (int i = 0; i < esquema.Rows.Count; i++)
                    {
                        DataRow row = esquema.Rows[i];
                        int tam = 0;
                        if (System.Type.GetType(row["DataType"].ToString()) == typeof(string))
                            tam = Convert.ToInt16(row["ColumnSize"]);
                        dtmodelo.Columns.Add(TDataControlReduzido.Coluna(System.Type.GetType(row["DataType"].ToString()), row["ColumnName"].ToString(), tam, false));
                    }
                    DataRow orow = null;
                    while (leitor.Read())
                    {
                        orow = dtmodelo.Rows.Add();
                        for (int i = 0; i < leitor.FieldCount; i++)
                        {
                            orow[i] = leitor[i];
                        }
                    }
                }
                leitor.Close();
                result.Tables.Add(dtmodelo);


                strOleDb = " SELECT  servic.* FROM " + path + "SERVIC";

                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                leitor = OleDbcomm.ExecuteReader();

                DataTable dtservico = new DataTable("SERVIC");

                if (leitor.HasRows)
                {
                    DataTable esquema = leitor.GetSchemaTable();
                    for (int i = 0; i < esquema.Rows.Count; i++)
                    {
                        DataRow row = esquema.Rows[i];
                        int tam = 0;
                        if (System.Type.GetType(row["DataType"].ToString()) == typeof(string))
                            tam = Convert.ToInt16(row["ColumnSize"]);
                        dtservico.Columns.Add(TDataControlReduzido.Coluna(System.Type.GetType(row["DataType"].ToString()), row["ColumnName"].ToString(), tam, false));
                    }
                    DataRow orow = null;
                    while (leitor.Read())
                    {
                        orow = dtservico.Rows.Add();
                        for (int i = 0; i < leitor.FieldCount; i++)
                        {
                            orow[i] = leitor[i];
                        }


                    }
                }
                leitor.Close();
                result.Tables.Add(dtservico);



                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["SERVIC"].Columns["COD"];
                result.Tables["SERVIC"].PrimaryKey = PrimaryKeyColumns;

                return result;

            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }

        static public DataTable Get_Setores()
        {
            OleDbCommand odbccomm;
            string strodbc, path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            strodbc = "SELECT * FROM " + path + "SETORES where fim = ctod('  /  /  ')";
            try
            {
                odbccomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                DataTable dtsetores = new DataTable("SETORES");
                DataTable esquema = new DataTable("SETORES");
                //if (leitor.HasRows)
                //{
                OleDbDataReader leitor = odbccomm.ExecuteReader();
                esquema = leitor.GetSchemaTable();
                for (int i = 0; i < esquema.Rows.Count; i++)
                {
                    DataRow row = esquema.Rows[i];
                    int tam = 0;
                    if (System.Type.GetType(row["DataType"].ToString()) == typeof(string))
                        tam = Convert.ToInt16(row["ColumnSize"]);
                    dtsetores.Columns.Add(TDataControlReduzido.Coluna(System.Type.GetType(row["DataType"].ToString()), row["ColumnName"].ToString(), tam, false));
                }
                if (leitor.HasRows)
                {
                    while (leitor.Read())
                    {
                        DataRow orow = dtsetores.NewRow();
                        for (int z = 0; z < leitor.FieldCount; z++)
                        {
                            orow[z] = leitor[z];
                        }
                        dtsetores.Rows.Add(orow);
                    }
                }

                leitor.Close();

                return dtsetores;



                return dtsetores;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }

        }






        static public double peghectares(string quadra)
        {
            double result = 0;
            OleDbCommand oledbcomm;
            string strodbc, path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");

            strodbc = "SELECT qua, ha, air, cent, aceiro, estrada, real FROM " + path + "AREAS where " +
             " (QUA = '" + quadra + "')";
            try
            {
                oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leitor = oledbcomm.ExecuteReader();
                if (leitor.HasRows)
                {
                    leitor.Read();
                    result = ((Convert.ToSingle(leitor["HA"]) * 10000) + (Convert.ToSingle(leitor["AIR"]) * 100)
                        + Convert.ToSingle(leitor["CENT"])) / 10000;
                }
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "AREAS"));
            }

        }

        static public double pegcaixas(string quadra, DateTime dtaponto, DateTime dtafim)
        {
            double result = 0;
            OleDbCommand oledbcomm;
            string strodbc, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            strodbc = "Select QUA,DATAE,NQUANT FROM " + path + "PRODUTO " +
                  " where " +
                  " (DATAP  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(dtaponto.AddDays(-2)) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(dtafim.AddDays(7)) + ") )" +
                      " (QUA = '" + quadra + "')";
            try
            {
                oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leitor = oledbcomm.ExecuteReader();
                if (leitor.HasRows)
                {
                    while (leitor.Read())
                        result += Convert.ToDouble(leitor["NQUANT"]);
                }

                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "QUADRAS"));
            }

        }

        static public double pegplantas(string quadra, string cult)
        {
            double result = 0;
            OleDbCommand oledbcomm;
            string strodbc, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");

            strodbc = "Select    qua, cultura, variedade, cavalo, data_plant, dcavalo, quant, data, tipomov, hist, setor " + path + "PLANTIO " +
                  " where " +
                      " (QUA = '" + quadra + "') AND " +
                      " (CULTURA = '" + cult + "')";

            try
            {
                oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leitor = oledbcomm.ExecuteReader();
                if (leitor.HasRows)
                {
                    while (leitor.Read())
                    {
                        if (leitor["TIPOMOV"].ToString() == "S")
                            result -= Convert.ToDouble(leitor["QUANT"]);
                        else
                            result += Convert.ToDouble(leitor["QUANT"]);
                    }
                }

                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "QUADRAS"));
            }
        }
        static public OleDbDataReader Get_Quadras(string CodSetor)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            strOleDb = "SELECT * FROM " + path + "QUADRAS where fim = ctod('  /  /  ')  and " +
             " SETOR = '" + CodSetor + "' order by qua";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                return OleDbcomm.ExecuteReader();
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "QUADRAS"));
            }

        }

        static public DataSet GlebasAtuais()
        {
            OleDbCommand OleDbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            stroledb = "SELECT cod, `desc` as Descricao, inicio, fim, setor, setorant, setorpos FROM  " + path + "GLEBAS ";
            stroledb += " ORDER BY COD";
            //where fim = ctod('  /  /  ')
            try
            {
                OleDbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "GLEBAS");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Não Foi Possivel acessar a tabela Setores/Quadras no caminho:", path));
            }
        }


        static public DataSet SetoresQuadras()
        {
            OleDbCommand OleDbcomm;
            string setorOleDb, path;
            DataSet result;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            setorOleDb = "SELECT * FROM " + path + "SETORES where fim = ctod('  /  /  ');" +
              "SELECT * FROM " + path + "QUADRAS where fim = ctod('  /  /  ') and qua <> ''" +
                 " and setor <> ''";
            try
            {
                OleDbcomm = new OleDbCommand(setorOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "SETORES");
                OleDbda.TableMappings.Add("Table1", "QUADRAS");
                result = new DataSet();
                OleDbda.Fill(result);
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["SETORES"].Columns["COD"];
                result.Tables["SETORES"].PrimaryKey = PrimaryKeyColumns;

                //DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["QUADRAS"].Columns["QUA"];
                result.Tables["QUADRAS"].PrimaryKey = PrimaryKeyColumns;
                DataColumn[] parentcolumn = new DataColumn[1];
                DataColumn[] childcolumn = new DataColumn[1];
                DataRelation[] mydatarelation = new DataRelation[1];

                parentcolumn[0] = result.Tables["SETORES"].Columns["Cod"];
                childcolumn[0] = result.Tables["QUADRAS"].Columns["Setor"];
                mydatarelation[0] = new DataRelation("Setor_QUADRAS", parentcolumn, childcolumn, true);

                result.Relations.AddRange(mydatarelation);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Não Foi Possivel acessar a tabela Setores/Quadras no caminho:", path));
            }
        }
        static public DataSet GlebasQuadras()
        {
            OleDbCommand OleDbcomm;
            string setorOleDb, path;
            DataSet result;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            setorOleDb = "SELECT * FROM " + path + "GLEBAS where fim = ctod('  /  /  ') ";
            setorOleDb += " ORDER BY COD";
           
            try
            {
                OleDbcomm = new OleDbCommand(setorOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "GLEBAS");
                result = new DataSet();
                OleDbda.Fill(result);
                
                result.Tables["GLEBAS"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                result.Tables["GLEBAS"].Columns["DisplayCodNome"].MaxLength = result.Tables["GLEBAS"].Columns["COD"].MaxLength + 1 + result.Tables["GLEBAS"].Columns["DESC"].MaxLength;

                foreach (DataRow orow in result.Tables["GLEBAS"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["COD"].ToString() + " " + orow["DESC"].ToString();
                }


                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["GLEBAS"].Columns["COD"];
                result.Tables["GLEBAS"].PrimaryKey = PrimaryKeyColumns;

                setorOleDb = "SELECT * FROM " + path + "QUADRAS where fim = ctod('  /  /  ') and qua <> ''" +
                     " and setor <> ''";
                setorOleDb += " ORDER BY QUA";
                OleDbcomm = new OleDbCommand(setorOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "QUADRAS");
                OleDbda.Fill(result);
           
                result.Tables["QUADRAS"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                //result.Tables["QUADRAS"].Columns["DisplayCodNome"].MaxLength = result.Tables["QUADRAS"].Columns["QUA"].MaxLength + 1 + result.Tables["GLEBAS"].Columns["DESC"].MaxLength;

                foreach (DataRow orow in result.Tables["QUADRAS"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["QUA"].ToString();// +" " + orow["DESC"].ToString();
                }
                PrimaryKeyColumns[0] = result.Tables["QUADRAS"].Columns["QUA"];
                result.Tables["QUADRAS"].PrimaryKey = PrimaryKeyColumns;
                DataColumn[] parentcolumn = new DataColumn[1];
                DataColumn[] childcolumn = new DataColumn[1];
                DataRelation[] mydatarelation = new DataRelation[1];

                parentcolumn[0] = result.Tables["GLEBAS"].Columns["Cod"];
                childcolumn[0] = result.Tables["QUADRAS"].Columns["GLEBA"];
                mydatarelation[0] = new DataRelation("Gleba_QUADRAS", parentcolumn, childcolumn, true);

                result.Relations.AddRange(mydatarelation);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Não Foi Possivel acessar a tabela GLebas/Quadras no caminho:", path));
            }
        }
        static public DataSet ModeloServicos()
        {
            OleDbCommand OleDbcomm;
            string setorOleDb, path;
            DataSet result;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            //Add('SELECT NUM,SEQ,COD,Modelo."DESC" as Descricao, TIPO,PROD FROM MODELO ');
            // Add(' where ');
            // Add(' ( MODELO."Desc" <> "" )' );
            // Add(' order by Descricao');
            // e

            setorOleDb = "SELECT NUM,SEQ,COD,DESC as Descricao, TIPO,PROD FROM " + path + "MODELO where DESC <> '' ";
            setorOleDb += " ORDER BY NUM";

            try
            {
                OleDbcomm = new OleDbCommand(setorOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "MODELO");
                result = new DataSet();
                OleDbda.Fill(result);

                result.Tables["MODELO"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                result.Tables["MODELO"].Columns["DisplayCodNome"].MaxLength = result.Tables["MODELO"].Columns["NUM"].MaxLength + 1 + result.Tables["MODELO"].Columns["DESCRICAO"].MaxLength;

                foreach (DataRow orow in result.Tables["MODELO"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["NUM"].ToString() + " " + orow["DESCRICAO"].ToString();
                }


                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["MODELO"].Columns["NUM"];
                result.Tables["MODELO"].PrimaryKey = PrimaryKeyColumns;
            
                setorOleDb = "SELECT NUM,SEQ,MODELO.COD,MODELO.DESC as Descricao,SERVIC.COD as CODSERV,SERVIC.DESC as DESCSERV,MODELO.TIPO,PROD FROM " + path + "MODELO " +
                    "INNER JOIN " + path + "SERVIC ON (MODELO.COD = SERVIC.COD) where  (MODELO.COD <> '') "; 
                     
                setorOleDb += " ORDER BY DESCSERV";
                OleDbcomm = new OleDbCommand(setorOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "SERVICO");
                OleDbda.Fill(result);

                result.Tables["SERVICO"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                result.Tables["SERVICO"].Columns["DisplayCodNome"].MaxLength = result.Tables["SERVICO"].Columns["CODSERV"].MaxLength + 1 + result.Tables["SERVICO"].Columns["DESCSERV"].MaxLength;

                foreach (DataRow orow in result.Tables["SERVICO"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["CODSERV"].ToString() + " " + orow["DESCSERV"].ToString();
                }
                /*PrimaryKeyColumns = new DataColumn[2];
                PrimaryKeyColumns[0] = result.Tables["SERVICO"].Columns["NUM"];
                PrimaryKeyColumns[1] = result.Tables["SERVICO"].Columns["CODSER"];
                result.Tables["SERVICO"].PrimaryKey = PrimaryKeyColumns;*/
                DataColumn[] parentcolumn = new DataColumn[1];
                DataColumn[] childcolumn = new DataColumn[1];
                DataRelation[] mydatarelation = new DataRelation[1];

                parentcolumn[0] = result.Tables["MODELO"].Columns["NUM"];
                childcolumn[0] = result.Tables["SERVICO"].Columns["NUM"];
                mydatarelation[0] = new DataRelation("MODELO_SERVICOS", parentcolumn, childcolumn, true);

                result.Relations.AddRange(mydatarelation);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Não Foi Possivel acessar a tabela modelo/servico no caminho:", path));
            }
        }

       


    } 
}
