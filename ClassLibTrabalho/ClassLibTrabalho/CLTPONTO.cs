using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ClassConexao;
using ClassFiltroEdite;

namespace ClassLibTrabalho
{
    public class ClassCLTPONTO
    {
        static public DataTable dtCLTPONTO()
        {
            DataTable otable = new DataTable("CLTPONTO");
            otable.Columns.Add("DATA", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("SETOR", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("FAZMOV", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TRAB", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 4;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("TIPOMOV", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NUM_MOD", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("CODSER", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("QUANT", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("SEMANA", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 30;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("VALOR", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("BL", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 3;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("OK", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIARIA", System.Type.GetType("System.Decimal"));
            otable.Columns.Add("DIA1", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIA2", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIA3", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIA4", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIA5", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIA6", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DIA7", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 2;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("DTA_INI", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("DTA_FIM", System.Type.GetType("System.DateTime"));
            otable.Columns.Add("OBS", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 25;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NOTURNO", System.Type.GetType("System.String"));
            otable.Columns[otable.Columns.Count - 1].MaxLength = 1;
            otable.Columns[otable.Columns.Count - 1].DefaultValue = "";
            otable.Columns.Add("NREG", System.Type.GetType("System.Int32"));
            return otable;
        }
        static public OleDbDataAdapter CLTPONTOConstruaAdaptador(string path)
        {
            OleDbDataAdapter OleDbda = new OleDbDataAdapter();
            path = TDataControlReduzido.Get_Path("TRABALHO");
            // Construção do insert padrao 

         /*   OleDbCommand cmd = new OleDbCommand(
            "INSERT INTO " + path + " CLTPONTO ( " +
            " DATA,  SETOR,  FAZMOV,  TRAB,  TIPOMOV,  NUM_MOD, " +
            " CODSER,  QUANT,  SEMANA,  VALOR,  BL,  OK,  DIARIA, " +
            " DIA1,  DIA2,  DIA3,  DIA4,  DIA5,  DIA6,  DIA7, " +
            " DTA_INI,  DTA_FIM,  OBS,  NOTURNO ) VALUES (  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@FAZMOV", OleDbType.Char, 4, "FAZMOV");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPOMOV", OleDbType.Char, 1, "TIPOMOV");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@SEMANA", OleDbType.Char, 30, "SEMANA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@BL", OleDbType.Char, 3, "BL");
            cmd.Parameters.Add("@OK", OleDbType.Char, 1, "OK");
            cmd.Parameters.Add("@DIARIA", OleDbType.Numeric, 0, "DIARIA");
            cmd.Parameters.Add("@DIA1", OleDbType.Char, 2, "DIA1");
            cmd.Parameters.Add("@DIA2", OleDbType.Char, 2, "DIA2");
            cmd.Parameters.Add("@DIA3", OleDbType.Char, 2, "DIA3");
            cmd.Parameters.Add("@DIA4", OleDbType.Char, 2, "DIA4");
            cmd.Parameters.Add("@DIA5", OleDbType.Char, 2, "DIA5");
            cmd.Parameters.Add("@DIA6", OleDbType.Char, 2, "DIA6");
            cmd.Parameters.Add("@DIA7", OleDbType.Char, 2, "DIA7");
            cmd.Parameters.Add("@DTA_INI", OleDbType.DBDate, 0, "DTA_INI");
            cmd.Parameters.Add("@DTA_FIM", OleDbType.DBDate, 0, "DTA_FIM");
            cmd.Parameters.Add("@OBS", OleDbType.Char, 25, "OBS");
            cmd.Parameters.Add("@NOTURNO", OleDbType.Char, 1, "NOTURNO");
            OleDbda.InsertCommand = cmd;
            */
            
            OleDbCommand cmd = new OleDbCommand(
            "INSERT INTO " + path + "CLTPONTO ( " +
            " DATA,  SETOR,  FAZMOV,  TRAB,  TIPOMOV,  NUM_MOD, " +
            " CODSER,  QUANT,  SEMANA,  VALOR,  BL,  OK,  DIARIA, " +
            " DIA1,  DIA2,  DIA3,  DIA4,  DIA5,  DIA6,  DIA7, " +
            " DTA_INI,  DTA_FIM,  OBS,  NOTURNO,NREG ) VALUES (  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?, " +
            " ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,  ?,? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());


            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@FAZMOV", OleDbType.Char, 4, "FAZMOV");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPOMOV", OleDbType.Char, 1, "TIPOMOV");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@SEMANA", OleDbType.Char, 30, "SEMANA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@BL", OleDbType.Char, 3, "BL");
            cmd.Parameters.Add("@OK", OleDbType.Char, 1, "OK");
            cmd.Parameters.Add("@DIARIA", OleDbType.Numeric, 0, "DIARIA");
            cmd.Parameters.Add("@DIA1", OleDbType.Char, 2, "DIA1");
            cmd.Parameters.Add("@DIA2", OleDbType.Char, 2, "DIA2");
            cmd.Parameters.Add("@DIA3", OleDbType.Char, 2, "DIA3");
            cmd.Parameters.Add("@DIA4", OleDbType.Char, 2, "DIA4");
            cmd.Parameters.Add("@DIA5", OleDbType.Char, 2, "DIA5");
            cmd.Parameters.Add("@DIA6", OleDbType.Char, 2, "DIA6");
            cmd.Parameters.Add("@DIA7", OleDbType.Char, 2, "DIA7");
             cmd.Parameters.Add("@DTA_INI", OleDbType.DBDate, 0, "DTA_INI");
            cmd.Parameters.Add("@DTA_FIM", OleDbType.DBDate, 0, "DTA_FIM");
            cmd.Parameters.Add("@OBS", OleDbType.Char, 25, "OBS");
            cmd.Parameters.Add("@NOTURNO", OleDbType.Char, 1, "NOTURNO");
            cmd.Parameters.Add("@NREG", OleDbType.Numeric, 0, "NREG");
            OleDbda.InsertCommand = cmd;

            

            // Construção do altera padrao 

            cmd = new OleDbCommand(
           "UPDATE " + path + "CLTPONTO SET " +
           " DATA = ? ,  SETOR = ? ,  FAZMOV = ? ,  TRAB = ? , " +
           " TIPOMOV = ? ,  NUM_MOD = ? ,  CODSER = ? ,  QUANT = ? , " +
           " SEMANA = ? ,  VALOR = ? ,  BL = ? ,  OK = ? , " +
           " DIARIA = ? ,  DIA1 = ? ,  DIA2 = ? ,  DIA3 = ? , " +
           " DIA4 = ? ,  DIA5 = ? ,  DIA6 = ? ,  DIA7 = ? , " +
           " DTA_INI = ? ,  DTA_FIM = ? ,  OBS = ? ,  NOTURNO = ? " +
           " WHERE ( NREG = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.DBDate, 0, "DATA");
            cmd.Parameters.Add("@SETOR", OleDbType.Char, 2, "SETOR");
            cmd.Parameters.Add("@FAZMOV", OleDbType.Char, 4, "FAZMOV");
            cmd.Parameters.Add("@TRAB", OleDbType.Char, 4, "TRAB");
            cmd.Parameters.Add("@TIPOMOV", OleDbType.Char, 1, "TIPOMOV");
            cmd.Parameters.Add("@NUM_MOD", OleDbType.Char, 2, "NUM_MOD");
            cmd.Parameters.Add("@CODSER", OleDbType.Char, 3, "CODSER");
            cmd.Parameters.Add("@QUANT", OleDbType.Numeric, 0, "QUANT");
            cmd.Parameters.Add("@SEMANA", OleDbType.Char, 30, "SEMANA");
            cmd.Parameters.Add("@VALOR", OleDbType.Numeric, 0, "VALOR");
            cmd.Parameters.Add("@BL", OleDbType.Char, 3, "BL");
            cmd.Parameters.Add("@OK", OleDbType.Char, 1, "OK");
            cmd.Parameters.Add("@DIARIA", OleDbType.Numeric, 0, "DIARIA");
            cmd.Parameters.Add("@DIA1", OleDbType.Char, 2, "DIA1");
            cmd.Parameters.Add("@DIA2", OleDbType.Char, 2, "DIA2");
            cmd.Parameters.Add("@DIA3", OleDbType.Char, 2, "DIA3");
            cmd.Parameters.Add("@DIA4", OleDbType.Char, 2, "DIA4");
            cmd.Parameters.Add("@DIA5", OleDbType.Char, 2, "DIA5");
            cmd.Parameters.Add("@DIA6", OleDbType.Char, 2, "DIA6");
            cmd.Parameters.Add("@DIA7", OleDbType.Char, 2, "DIA7");
            cmd.Parameters.Add("@DTA_INI", OleDbType.DBDate, 0, "DTA_INI");
            cmd.Parameters.Add("@DTA_FIM", OleDbType.DBDate, 0, "DTA_FIM");
            cmd.Parameters.Add("@OBS", OleDbType.Char, 25, "OBS");
            cmd.Parameters.Add("@NOTURNO", OleDbType.Char, 1, "NOTURNO");
            cmd.Parameters.Add("@NREG", OleDbType.Numeric,0 , "NREG").SourceVersion = DataRowVersion.Original;
            OleDbda.UpdateCommand = cmd;

            // Construção do deleta padrao 

            cmd = new OleDbCommand(
           " DELETE FROM " + path + "CLTPONTO" +
           " WHERE ( NREG = ? )", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            cmd.Parameters.Add("@NREG", OleDbType.Numeric, 0, "NREG").SourceVersion = DataRowVersion.Original;
            OleDbda.DeleteCommand = cmd;
            
            
            return OleDbda;
        }
        static public int MaiorNReg()
        {
            string path = TDataControlReduzido.Get_Path("TRABALHO");
            OleDbCommand command = new OleDbCommand(
                    "SELEC MAX(NREG) AS numero FROM " + path + "CLTPONTO",
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            //int numero = 0;
            try
            {
                if (TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().State == ConnectionState.Closed)
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception)
            {
                return -1;
            }
        }


        static public DataSet Get_CltCad_GuiaCltPonto(DateTime data1, DateTime data2, List<LinhaSolucao> oLista,DataSet dstrabalhista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result = null;
            string criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                        " ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                         " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) ";

            stroledb = " SELECT CODCAD, NOMECAD, GLECAD, SETOR,  CARTIDENT, ADMI, DEMI, PRAZO, SALBASE,"+ //"OPCAO, AVULSO, DEPEND, IRDEPEND, INSCPIS, CTA_FGTS," +
                         "  MENSALISTA "+//" BANCO1, CONTA1, AGENCIA1, BCOAGCC, BANCO_OK, CPF, NUMERO, " +
                        // " SEXO, NASC, COD_ADMI, CARTTRAB, SERIE,ULT_ATUAL, CBO, CATEGORIA, VLRFGTS, DTAFGTS, TIPODEMI, AVISO, SALRESC,MAE, PAI, CONJUGE, END_RUA, END_CID, END_UF, END_CEP, NCIDADE, NUF, " + 
                         // " EMICID, EMICTRAB, EMIPIS, ESTCIVIL, COR, TITELEITOR, RESERV, RESERV_CAT, TPSANGUE, DEFIC, TPDEFIC, APRENDIZ, " +
                       //  " TPMOV, DTAAVISO " +
                       "FROM " + path + "CLTCAD  WHERE " + criterio_admi_demi;
            //campos fora:
            
            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            stroledb += " ORDER by CLTCAD.SETOR,CLTCAD.codcad";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTCAD");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
              
               // DataSet dsReajustes = TDataControlTrabalho.Get_CltReajuste(data1, data2, null, criterio_admi_demi);
                odataset.Tables[0].Columns.Add("SALARIOREAL", Type.GetType("System.Decimal"));
              
                //DataSet dsFerias = TDataControlTrabalho.Get_CltFerias(data1, data2, null, criterio_admi_demi);
                odataset.Tables[0].Columns.Add("GOZO_FIM", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("GOZO_INI", Type.GetType("System.DateTime"));

              
                odataset.Tables[0].Columns.Add("INCLUIDO", Type.GetType("System.Boolean"));
                odataset.Tables[0].Columns.Add("DIARIAS", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("HORAS50", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("HORASFE", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("HORAS100", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("VALOREMP", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("Critica", Type.GetType("System.String"));
                

                DataTable tabDeb = odataset.Tables[0].Clone();

                Decimal salmin = TDataControlTrabalho.CalcSalarioMinimo(data2, dstrabalhista.Tables["TABSAL"]);


                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;
                   
                    odatarow.BeginEdit();

                    odatarow["SALARIOREAL"] = odatarow["SALBASE"];
                   // odatarow["SALARIOREAL"] = TDataControlTrabalho.CalcSalBase(Convert.ToString(odatarow["CODCAD"]), Convert.ToDateTime(odatarow["ADMI"]),
                   //     Convert.ToDecimal(odatarow["SALBASE"]),salmin, data2,   dsReajustes.Tables[0]);
                    odatarow["INCLUIDO"] = false; 

                   /* TDataControlTrabalho.informeferias oferias = TDataControlTrabalho.UltimaFeriasGozadas(Convert.ToString(odatarow["CODCAD"]), data2, dsFerias.Tables[0]);
                    if (oferias != null)
                    {
                        odatarow["GOZO_INI"] = Convert.ToDateTime(oferias.Gozo_ini);
                        odatarow["GOZO_FIM"] = Convert.ToDateTime(oferias.Gozo_fim);
                    }*/
                    odatarow.EndEdit();
                    odatarow.AcceptChanges();
                    tabDeb.Rows.Add(odatarow.ItemArray);
                }
                tabDeb.PrimaryKey = new DataColumn[1] { tabDeb.Columns["CODCAD"] };
                result = new DataSet();
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            { throw; }
        }

      /*  static public DataTable Get_Cltponto_reader(DateTime inicio, DateTime fim, string tipo)
        { 
             OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            string lit_tipo;
            if (tipo == "")
                lit_tipo = "";
            else
                lit_tipo = " (TIPOMOV = '" + tipo + "')  AND ";
            if (inicio.Date == fim.Date)
            {
                inicio = inicio.AddDays(-1);
                fim = fim.AddDays(1);
            }
            
            strOleDb = "Select SETOR,FAZMOV,BL,NUM_MOD,CODSER,DATA,TRAB ,TIPOMOV, OK,DIA1,DIA2,DIA3,DIA4,DIA5,DIA6,DIA7," +
                "QUANT,semana, valor,DIARIA, dta_ini, dta_fim, obs,NOTURNO, RECNO() AS nreg   FROM " +
                    path + "CLTPONTO " +
                    "WHERE " + lit_tipo + " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") ) " 
                 + "AND  ( (FAZMOV <> '') OR (NUM_MOD <> '')  OR (BL <> '') OR (CODSER <> '') " +
                        " OR (DIA1 <> '') OR (DIA2 <> '') OR (DIA3 <> '') OR (DIA4 <> '') " +
                       " OR (DIA5 <> '') OR (DIA6 <> '') OR (DIA7 <> '')) " 

                    + " ORDER BY TRAB ";
            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leitor = oledbcomm.ExecuteReader();
                DataTable dtcltponto = new DataTable();
                DataTable esquema = leitor.GetSchemaTable();
                for (int i = 0; i < esquema.Rows.Count; i++)
                {
                    DataRow row = esquema.Rows[i];
                    int tam = 0;
                    if (System.Type.GetType(row["DataType"].ToString()) == typeof(string))
                        tam = Convert.ToInt16(row["ColumnSize"]);
                    dtcltponto.Columns.Add(TDataControlReduzido.Coluna(System.Type.GetType(row["DataType"].ToString()), row["ColumnName"].ToString(), tam, false));
                    DataColumn ocol = dtcltponto.Columns[dtcltponto.Columns.Count - 1];
                    if (ocol.DataType == Type.GetType("System.String"))
                        ocol.DefaultValue = "";
                    else
                        if (ocol.DataType == Type.GetType("System.Decimal"))
                            ocol.DefaultValue = 0.00;

                }
                dtcltponto.TableName = "CLTPONTO";
                if (leitor.HasRows)
                {
                    while (leitor.Read())
                    {
                        DataRow orow = dtcltponto.Rows.Add();
                        for (int i = 0; i < leitor.FieldCount; i++)
                        {
                            orow[i] = leitor[i];
                        }
                    }
                }
                leitor.Close();
                return dtcltponto;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        */
        static public DataSet Get_Cltponto(DateTime inicio, DateTime fim, string tipo)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            string lit_tipo;
            if (tipo == "")
                lit_tipo = "";
            else
                lit_tipo = " (TIPOMOV = '" + tipo + "')  AND ";
            if (inicio.Date == fim.Date)
            {
                inicio = inicio.AddDays(-1);
                fim = fim.AddDays(1);
            }

            strOleDb = "Select SETOR,FAZMOV,BL,NUM_MOD,CODSER,DATA,TRAB ,TIPOMOV, OK,DIA1,DIA2,DIA3,DIA4,DIA5,DIA6,DIA7," +
                "QUANT,semana, valor,DIARIA, dta_ini, dta_fim, obs,NOTURNO, RECNO() AS nreg   FROM " +
                    path + "CLTPONTO " +
                    "WHERE " + lit_tipo + " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") ) "
                 + "AND  ( (FAZMOV <> '') OR (NUM_MOD <> '')  OR (BL <> '') OR (CODSER <> '') " +
                        " OR (DIA1 <> '') OR (DIA2 <> '') OR (DIA3 <> '') OR (DIA4 <> '') " +
                      " OR (DIA5 <> '') OR (DIA6 <> '') OR (DIA7 <> '')) " 

                    + " ORDER BY TRAB";
            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

              
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTPONTO");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
              
                
                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }

        }





    }
    //
    

}

