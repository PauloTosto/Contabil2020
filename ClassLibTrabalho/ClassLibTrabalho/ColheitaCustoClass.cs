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
    public class ColheitaCustoClass
    {
        public static DateTime DiaDoPonto(DateTime tdata, DayOfWeek tconst)
        {
            DateTime result = tdata;
            while (result.DayOfWeek != tconst)   //Wednesday"
                result = result.AddDays(-1);
            return result;
        }

        static public DataSet Get_ParceirosCacauPeriodo(DateTime inicio, DateTime fim)
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            string oprod = "  1";
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            strOleDb = "SELECT parceiro.cod as CODIGO, prgarea.ncontra, prgarea.cod, prgarea.gleba , QUADRAS.qua, parceiro.nome, prgarea.inicio, prgarea.fim, prgarea.ncontra " +
              " FROM " + path + "PARCEIRO, " + path + "PRGAREA, " + path + "QUADRAS " +
              "WHERE   (PROD = '" + oprod + "') AND( prgarea.cod = parceiro.cod) AND ( prgarea.gleba = QUADRAS.gleba) AND" +
              " (QUADRAS.fim = CTOD('  /  /  ')) AND " +
              "( (CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") BETWEEN prgarea.inicio AND prgarea.fim) " +
              " OR (CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") BETWEEN prgarea.inicio AND prgarea.fim) )";
            try
            {
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);
                OleDbda.TableMappings.Add("Table", "PARCEIRO");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }

        static public DataSet GetDadosPontoColheita(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            //,cltponto.ok, cltponto.num_mod, cltponto.codser,cltponto.setor, cltponto.fazmov
            stroledb = "SELECT        cltponto.data,  cltponto.bl, servic.nindice ,cltponto.DIA1,cltponto.DIA2,cltponto.DIA3,cltponto.DIA4," +
                 "cltponto.DIA5,cltponto.DIA6,cltponto.DIA7,cltponto.OK ,cltponto.num_mod,cltponto.codser " +
           "  FROM         " + path + "CLTponto," + path + "SERVIC " +
           " WHERE         cltponto.codser = servic.cod " +
                //"AND (cltponto.data > ctod('04/01/2011')) "+
              " AND (cltponto.data BETWEEN CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              " AND " +
                 "            (cltponto.num_mod = ' 1' OR " +
                  "           cltponto.num_mod = ' 2' OR " +
                   "          cltponto.num_mod = '12' OR " +
                    "         cltponto.num_mod = '13') AND (servic.nindice = 10)" +
                //   "  AND (ok = 'X' ) "+
                //" ORDER BY cltponto.setor, cltponto.fazmov, cltponto.bl, cltponto.data,servic.nindice, cltponto.num_mod, cltponto.codser ";
                //cltponto.setor, cltponto.fazmov, , cltponto.num_mod, cltponto.codser, modelo.`desc`, servic.`desc`
                // " GROUP BY cltponto.bl, cltponto.data,servic.nindice ";
          " ORDER BY cltponto.bl, cltponto.data,servic.nindice ";

            /*SELECT        cltponto.data, cltponto.bl, SERVIC.nindice, cltponto.dia1, cltponto.dia2, cltponto.dia3, cltponto.dia4, cltponto.dia5, cltponto.dia6, cltponto.dia7, cltponto.ok
FROM            cltponto, SERVIC
WHERE        cltponto.codser = SERVIC.cod AND (cltponto.bl = '405') AND (cltponto.data BETWEEN CTOD('09/29/2011') AND CTOD('11/03/2011')) AND 
                         (cltponto.num_mod = ' 1' OR
                         cltponto.num_mod = ' 2' OR
                         cltponto.num_mod = '12' OR
                         cltponto.num_mod = '13') AND (SERVIC.nindice = 10)
ORDER BY cltponto.ok DESC, cltponto.data, SERVIC.nindice
             */ 
            
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "CLTPONTO");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }

        }

        /*
         *  SELECT        cltponto.data,  cltponto.bl, servic.nindice 
             FROM         CLTponto, SERVIC 
            WHERE         cltponto.codser = servic.cod 
               AND (cltponto.data > ctod('04/01/2011'))AND (cltponto.data < ctod('04/01/2011')) 
               AND 
                             (cltponto.num_mod = ' 1' OR 
                             cltponto.num_mod = ' 2' OR 
                             cltponto.num_mod = '12' OR 
                             cltponto.num_mod = '13') AND (servic.nindice = 10)
            GROUP BY cltponto.bl, cltponto.data,servic.nindice ;
         */

        static public DataSet GetProdutoPeriodo(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            stroledb = "SELECT  setor, gleba, qua, safra, lote, datae, nquant AS caixas,quant AS kgtotal, quant_fr AS kgempresa, quant_pa AS kgparceiro, quant_ri AS kgri ,codparc, ncontra, datap " +
           "  FROM         " + path + "PRODUTO" +
           " WHERE        (prod = '  1') " +
              " AND (produto.datae BETWEEN CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              " ORDER BY setor, qua, datae DESC";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "PRODUTO");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        static public DataSet GetProdutoFinoPeriodo(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = "SELECT  setor, gleba, qua, safra, lote, datae, nquant AS caixas,quant AS kgtotal, quant_fr AS kgempresa, quant_pa AS kgparceiro, quant_ri AS kgri, codparc, ncontra, datap " +
           "  FROM         " + path + "PRODUTO_PT" +
           " WHERE        (prod = '  1') " +
              " AND (produto.datae BETWEEN CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              " ORDER BY setor, qua, datae DESC";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "PRODUTO_PT");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }


        static public DataSet GetProdutoSegue(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = "SELECT        safra, lote, prod, setor, data, quant, scs " +
             "  FROM         " + path + "SEGUE" +
             " WHERE        (prod = '  1') " +
              " AND (data BETWEEN CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              " ORDER BY safra,lote,setor,data";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "SEGUE");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }

        static public DataSet GetProdutoSegue_Paineiras(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = "SELECT        safra, lote, prod, setor, data, quant, scs " +
             "  FROM         " + path + "SEGUE_PT" +
             " WHERE        (prod = '  1') " +
              " AND (data BETWEEN CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              " ORDER BY safra,lote,setor,data";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "SEGUE_PT");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        /*
         * SELECT        setor, prod, safra, lote, u_benef, dataini, datafim, natura, benef, transp, apronte, fecha, quant_fr, quant_pa, quant_ri, vquant_fr, vquant_pa, vlr_fr, vlr_pa, 
                         vlr_ri, b_rateio, b_venda
FROM            lote
         */

        static public DataSet peghectares()
        {
            OleDbCommand oledbcomm;
            string path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");

            string stroledb = "SELECT qua, ha, air, cent, aceiro, estrada, real FROM " + path + "AREAS";//where " +
            // " (QUA = '" + quadra + "')";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "PRODUTO");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                result.Tables[0].Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "AREA", 0, false));
                foreach (DataRow orow in result.Tables[0].Rows)
                {
                    orow.BeginEdit();
                    orow["area"] = ((Convert.ToDecimal(orow["HA"]) * 10000) + (Convert.ToDecimal(orow["AIR"]) * 100)
                        + Convert.ToDecimal(orow["CENT"])) / 10000;
                    orow.EndEdit();
                }
                result.Tables[0].AcceptChanges();
                return result;
                /* oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                 OleDbDataReader leitor = oledbcomm.ExecuteReader();
                 if (leitor.HasRows)
                 {
                     leitor.Read();
                     result = ((Convert.ToSingle(leitor["HA"]) * 10000) + (Convert.ToSingle(leitor["AIR"]) * 100)
                         + Convert.ToSingle(leitor["CENT"])) / 10000;
                 }
                 return result;*/
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "AREAS"));
            }

        }

        static public DataSet GetDadosFolhaColheita(DateTime inicio, DateTime fim)//, List<LinhaSolucao> oLista
        {

            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            stroledb = "SELECT        cltfolha.data, cltfolha.setor, cltfolha.centro, cltfolha.quadra, cltfolha.num_mod, cltfolha.codser, servic.nindice, modelo.`desc` as ModDesc, servic.`desc` AS ServDescricao, " +
           "   SUM(cltfolha.salario + cltfolha.vlr_hxs + cltfolha.vlr_hxn + cltfolha.vlr_hxa) AS salario, " +
           "  SUM(cltfolha.fgts + cltfolha.ferias + cltfolha.decimo + cltfolha.educ + cltfolha.fgts_fe + cltfolha.fgts_dec + cltfolha.terc + cltfolha.educ_dec + cltfolha.educ_fe) " +
           " AS encargos, SUM(cltfolha.quant) AS quantidade , SUM(cltfolha.hefetiva) as hefetiva " +
           "  FROM         " + path + "CLTFOLHA," + path + "MODELO," + path + "SERVIC " +
           " WHERE        cltfolha.num_mod = modelo.num AND cltfolha.codser = servic.cod " +
                //"AND (cltfolha.data > ctod('04/01/2011')) "+
              " AND (cltfolha.data BETWEEN CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
              " AND (modelo.`desc` <> '') AND " +
                 "            (cltfolha.num_mod = ' 1' OR " +
                  "           cltfolha.num_mod = ' 2' OR " +
                   "          cltfolha.num_mod = '12' OR " +
                    "         cltfolha.num_mod = '13') AND (servic.nindice = 10 OR " +
                     "        servic.nindice = 20 OR " +
                      "        servic.nindice = 25 OR " +
                       "      servic.nindice = 30) " +
           " GROUP BY cltfolha.setor, cltfolha.centro, cltfolha.quadra, cltfolha.data,servic.nindice, cltfolha.num_mod, cltfolha.codser, modelo.`desc`, servic.`desc` ";


            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                DataSet oparceiros = Get_ParceirosCacauPeriodo(inicio, fim);
                DataTable parceiros = oparceiros.Tables[0];

                DataSet ocltponto = GetDadosPontoColheita(inicio, fim);
                DataTable cltponto = ocltponto.Tables[0];
                cltponto.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LANCADO", 1, false));
                cltponto.Columns["LANCADO"].DefaultValue = "";

                DataSet oproduto = GetProdutoPeriodo(inicio, fim.AddDays(10));
                DataTable produto = oproduto.Tables[0];
                
                produto.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LANCADO", 1, false));
                produto.Columns["LANCADO"].DefaultValue = "";
                produto.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINO", 0, false));
                produto.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINOREJEITO", 0, false));
                produto.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FAPESBFINO", 0, false));
                produto.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FAPESBREJEITO", 0, false));


                DataSet osegue = GetProdutoSegue(inicio, fim.AddDays(30));
                DataTable segue = osegue.Tables[0];


                DataSet dsareas = peghectares();
                DataTable dtareas = dsareas.Tables[0];
                // dtareas
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = dtareas.Columns["QUA"];
                dtareas.PrimaryKey = PrimaryKeyColumns;

                DataTable ErrosProduto = produto.Clone();
                DataTable FapesbProduto = produto.Clone();
                FapesbProduto.TableName = "FAPESBPROD";
                DataTable Dados = odataset.Tables["CLTFOLHA"];
                DataTable DadosFolha = odataset.Tables["CLTFOLHA"].Clone();

                DadosFolha.TableName = "CLTQUADRAS";

                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LANCADO", 1, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "PARCEIRO", 1, false));
                DadosFolha.Columns["PARCEIRO"].DefaultValue = "";
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "CAIXAS", 0, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SECO", 0, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINO", 0, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINOREJEITO", 0, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINO_FAPESB", 0, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "AREA", 0, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "FAPESB", 1, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "ACUMULALOTE", 1, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LOTE1", 15, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LOTE2", 15, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LOTE3", 15, false));
                DadosFolha.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "LOTE4", 15, false));

                DataTable DadosSecagem = odataset.Tables["CLTFOLHA"].Clone();
                DadosSecagem.TableName = "CLTSECAGEM";
                DadosSecagem.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "CAIXAS", 0, false));
                DadosSecagem.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SECO", 0, false));
                DadosSecagem.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINO", 0, false));

                DataTable DadosTransporte = odataset.Tables["CLTFOLHA"].Clone();
                DadosTransporte.TableName = "CLTTransporte";
                DadosTransporte.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "CAIXAS", 0, false));
                DadosTransporte.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SECO", 0, false));
                DadosTransporte.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINO", 0, false));


                DataTable DadosGerais = odataset.Tables["CLTFOLHA"].Clone();
                DadosGerais.TableName = "CLTTransporte";
                DadosGerais.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "CAIXAS", 0, false));
                DadosGerais.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SECO", 0, false));
                DadosGerais.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FINO", 0, false));
                DataRow odatarow;
                DataRow orow;
                string tquadra = "";
                string tparceiro = "";
                for (int i = 0; i < Dados.Rows.Count; i++)
                {
                    odatarow = Dados.Rows[i];
                    //
                    if ((odatarow["CENTRO"].ToString().Substring(2, 2) == "61") || (Convert.ToDecimal(odatarow["NINDICE"]) == 30.0M))
                    {
                        orow = DadosSecagem.NewRow();
                        foreach (DataColumn ocol in odatarow.Table.Columns)
                            orow[ocol.ColumnName] = odatarow[ocol.ColumnName];
                        DadosSecagem.Rows.Add(orow);
                        continue;
                    }
                    if ((odatarow["CENTRO"].ToString().Substring(2, 2) == "62") || (odatarow["CENTRO"].ToString().Substring(2, 2) == "63") || (Convert.ToDecimal(odatarow["NINDICE"]) == 20.0M) || (Convert.ToDecimal(odatarow["NINDICE"]) == 25.0M))
                    {
                        orow = DadosTransporte.NewRow();
                        foreach (DataColumn ocol in odatarow.Table.Columns)
                            orow[ocol.ColumnName] = odatarow[ocol.ColumnName];
                        DadosTransporte.Rows.Add(orow);
                        continue;
                    }
                    if ((odatarow["QUADRA"].ToString().Trim() == "") && (Convert.ToDecimal(odatarow["NINDICE"]) == 10.0M))
                    {
                        orow = DadosGerais.NewRow();
                        foreach (DataColumn ocol in odatarow.Table.Columns)
                            orow[ocol.ColumnName] = odatarow[ocol.ColumnName];
                        DadosGerais.Rows.Add(orow);
                        continue;
                    }

                    orow = DadosFolha.NewRow();
                    foreach (DataColumn ocol in odatarow.Table.Columns)
                        orow[ocol.ColumnName] = odatarow[ocol.ColumnName];
                    //DadosFolha.Rows.Add(orow);
                    if (orow["QUADRA"].ToString() != tquadra)
                    {
                        tquadra = orow["QUADRA"].ToString();
                        DataRow[] dataparc = parceiros.Select("QUA = " + "'" + orow["QUADRA"] + "'");
                        if (dataparc.Length > 0)
                        {
                            tparceiro = "X";
                        }
                        else
                        {
                            tparceiro = "";
                        }
                    }
                    orow["PARCEIRO"] = tparceiro;
                    orow["LANCADO"] = "";
                    DadosFolha.Rows.Add(orow);
                }

               // inovação
                //cltfolha.num_mod, cltfolha.codser
              /*  var dados_cltParceiros =
                    from linha in DadosFolha.AsEnumerable()
                    where (linha.Field<string>("Parceiro") == "X")
                    group linha by new
                    {
                        quadra = linha.Field<string>("QUADRA"),
                        data = linha.Field<DateTime>("Data"),
                        num_mod = linha.Field<string>("NUM_MOD"),
                        codser = linha.Field<string>("CODSER")
                    } into g
                    orderby g.Key.data descending,g.Key.quadra
                    select new
                    {
                        quadra = g.Key.quadra,
                        data = g.Key.data,
                        num_mod = g.Key.num_mod,
                        codser = g.Key.codser,
                        hefetivas = g.Sum((linha => linha.Field<Decimal?>("Hefetiva"))),
                        salarios = g.Sum((linha => linha.Field<Decimal?>("Salario"))),
                        encargos = g.Sum((linha => linha.Field<Decimal?>("encargos")))
                    }
                    
                    ;

                // faz um link entre a folha e o cltponto
                DateTime dtacompara = DateTime.MinValue;
                int diainicio;
                DateTime fimponto = DateTime.MinValue;
                DateTime inicioponto = DateTime.MinValue; 
                int diafim;
                foreach (var linha in dados_cltParceiros)
                {
                    if (!dtacompara.Equals(linha.data))
                    {
                        inicioponto = DiaDoPonto(TDataControlReduzido.PrimeiroDiaMes(linha.data), DayOfWeek.Thursday);
                        diainicio = (TDataControlReduzido.PrimeiroDiaMes(linha.data) - inicioponto).Days;
                        fimponto = DiaDoPonto(linha.data, DayOfWeek.Thursday);
                        diafim = (linha.data - fimponto).Days;
                        dtacompara = linha.data;
                    }
                    DataRow[] rowscltponto = cltponto.Select("(BL = " + "'" + linha.quadra + "') " +
                               " AND (DATA <= '" + fimponto.ToString("d") + "') " +
                                    " AND (DATA >= '" + inicioponto.ToString("d") + "') ", "OK DESC,DATA DESC");
                    if (rowscltponto.Length == 0)
                    {
                        continue;
                    }
                    int ind = 0;
                    while (ind < rowscltponto.Length) 
                    {
                        DataRow orowclt = rowscltponto[ind];
                        if (orowclt["OK"].ToString() == "X")
                        {
                            if ((ind + 1) < rowscltponto.Length)
                            { 
                                int ind2 = ind + 1;
                                DataRow orowclt2 = rowscltponto[ind2];
                                while ((ind2 < rowscltponto.Length) && (orowclt2["NUM_MOD"].ToString() == orowclt["NUM_MOD"].ToString())
                                      && (orowclt2["CODSER"].ToString() == orowclt["CODSER"].ToString()))
                                {
                                    orowclt2 = rowscltponto[ind2];
                                    ind2 = ind2 + 1;
                                }
                                ind = ind2;
                            }
                            else
                                ind = ind +1;

                        }
                        else
                            ind = ind + 1;

                    }
                }
   
                
                */


                // preparação das tabelas produto e segue
                // as fazendas Paineiras/LG e PIM nao separam os lotes de cacau fino
                // a fazenda paineiras usa codigo "934" 
              DataTable Produto_PT = GetProdutoFinoPeriodo(inicio,fim).Tables[0];
              DataSet resultado = Class_Produto_Rateio.TabelaProdutoAjustada(produto, segue, Produto_PT, parceiros);

              produto = resultado.Tables[0];
              produto.TableName = "Produto";
         //     segue = resultado.Tables[1];
               var dados_segue =
               from linha in segue.AsEnumerable()
               where (linha.Field<decimal>("scs") > 0M)
               group linha by linha.Field<string>("SAFRA") +
                     linha.Field<string>("LOTE") +
               linha.Field<string>("SETOR") into grupo
        
               select new
               {
                   safra = grupo.Key.ToString().Substring(0, 4),
                   lote = grupo.Key.ToString().Substring(4, 3),
                   setor = grupo.Key.ToString().Substring(7, 2)
               };

                    // para cada safra/setor/lote
               foreach (var campos in dados_segue)
               {
                   DataRow[] orows = produto.Select("(SAFRA = '" + campos.safra + "') AND " +
                                                      "(LOTE = '" + campos.lote + "') AND " +
                                                      "(SETOR = '" + campos.setor + "')");
                   //total kilossecos produto
                   System.Nullable<decimal> totallote =
                     (from valor1 in orows.AsEnumerable()
                      select valor1.Field<decimal>("kgtotal")).Sum();


                   DataRow[] orowssegue = segue.Select("(SAFRA = '" + campos.safra + "') AND " +
                                                        "(LOTE = '" + campos.lote + "') AND " +
                                                        "(SETOR = '" + campos.setor + "')");
                   // total kilos seco beneficiamento
                   System.Nullable<decimal> campos_total =
                       (from linha in orowssegue.AsEnumerable()
                        select linha.Field<decimal?>("QUANT")).Sum();
                   if (campos_total == 0)
                       continue;

                   if (totallote == 0)
                   {
                       continue;
                   }
                   if (totallote != campos_total)
                   {
                       MessageBox.Show("erro dif segue e lote");
                       continue;
                   }
                   System.Nullable<decimal> segue_0 =
                           (from linha in orowssegue.AsEnumerable()
                            where (linha.Field<decimal>("SCS") == 0)
                            select linha.Field<decimal?>("QUANT")).Sum();
                   if (segue_0 == campos_total)
                   {
                       continue;
                   }

                   System.Nullable<decimal> CacauFino_1 =
                       (from linha in orowssegue.AsEnumerable()
                        where (linha.Field<decimal>("SCS") == 1)
                        select linha.Field<decimal?>("QUANT")).Sum();

                   if (CacauFino_1 != 0)
                   {
                       if (CacauFino_1 == campos_total)
                       {
                           foreach (DataRow rowedite in orows)
                           {
                               rowedite.BeginEdit();
                               rowedite["FINO"] = Convert.ToDecimal(rowedite["KGTOTAL"]);
                               rowedite.EndEdit();
                               rowedite.AcceptChanges();
                           }
                       }
                       else
                       {
                           decimal sobrafino = (decimal)CacauFino_1;
                           foreach (DataRow rowedite in orows)
                           {
                               rowedite.BeginEdit();
                               decimal rateiofino = Math.Round(((Convert.ToDecimal(rowedite["KGTOTAL"]) / (decimal)totallote) * (decimal)CacauFino_1), 2);
                               if (sobrafino > 0)
                                   sobrafino = sobrafino - rateiofino;
                               rowedite["FINO"] = rateiofino;
                               rowedite.EndEdit();
                               rowedite.AcceptChanges();
                           }
                           if (sobrafino < 0.5M)
                           {
                               DataRow rowedite = orows.Last();
                               rowedite.BeginEdit();
                               rowedite["FINO"] = Convert.ToDecimal(rowedite["FINO"]) + sobrafino;
                               rowedite.EndEdit();
                               rowedite.AcceptChanges();
                           }
                       }
                       System.Nullable<decimal> OutroCacauFino_1 =
                              (from linha in orows.AsEnumerable()
                               select linha.Field<decimal?>("FINO")).Sum();
                       if (OutroCacauFino_1 != CacauFino_1)
                       {
                           MessageBox.Show("erro no rateio de cacaufino");
                       }

                   }
                   // verifica se houve parte do fino que foi rejeitado como fino
                   System.Nullable<decimal> FinoRejeitado_2 =
                         (from linha in orowssegue.AsEnumerable()
                          where (linha.Field<decimal>("SCS") == 2)
                          select linha.Field<decimal?>("QUANT")).Sum();

                   if (FinoRejeitado_2 != 0)
                   {
                       System.Nullable<decimal> OutroCacauFino_1 =
                          (from linha in orows.AsEnumerable()
                           select linha.Field<decimal?>("FINO")).Sum();
                       string campo_rateio = "FINO";
                   
                       if (OutroCacauFino_1 == 0)
                       {
                           OutroCacauFino_1 = totallote;
                           campo_rateio = "KGTOTAL";
                       }


                       decimal sobrarejeito = (decimal)FinoRejeitado_2;
                       foreach (DataRow rowedite in orows)
                       {
                           rowedite.BeginEdit();
                           decimal rateiorejeito = Math.Round(((Convert.ToDecimal(rowedite[campo_rateio]) / (decimal)OutroCacauFino_1) * (decimal)FinoRejeitado_2), 4);
                           if (sobrarejeito > 0)
                               sobrarejeito = sobrarejeito - rateiorejeito;
                           rowedite["FINOREJEITO"] = rateiorejeito;
                           rowedite.EndEdit();
                           rowedite.AcceptChanges();
                       }
                       if (sobrarejeito < 0.5M)
                       {
                           DataRow rowedite = orows.Last();
                           rowedite.BeginEdit();
                           rowedite["FINOREJEITO"] = Convert.ToDecimal(rowedite["FINOREJEITO"]) + sobrarejeito;
                           rowedite.EndEdit();
                           rowedite.AcceptChanges();
                       }
                   }

                   /*      System.Nullable<decimal> segue_3 =
                        (from linha in orowssegue.AsEnumerable()
                         where (linha.Field<decimal>("SCS") == 3)
                         select linha.Field<decimal?>("QUANT")).Sum();

                         System.Nullable<decimal> segue_4 =
                            (from linha in orowssegue.AsEnumerable()
                             where (linha.Field<decimal>("SCS") == 4)
                             select linha.Field<decimal?>("QUANT")).Sum();

                         continue;
                     }
                         */

                   System.Nullable<decimal> FinoFapesb_61 =
                       (from linha in orowssegue.AsEnumerable()
                        where (linha.Field<decimal>("SCS") == 61)
                        select linha.Field<decimal?>("QUANT")).Sum();



                   if (FinoFapesb_61 == campos_total)
                   {
                       foreach (DataRow rowedite in orows)
                       {
                           rowedite.BeginEdit();
                           rowedite["FAPESBFINO"] = Convert.ToDecimal(rowedite["KGTOTAL"]);
                           rowedite.EndEdit();
                           rowedite.AcceptChanges();
                       }
                   }
               }

                    //sao varios os tipos de cacau seco informado no segue (no campo "SCS":
                    //  0 normal
                    // 1 fino
                    // 2 fino desclassificado
                    // 61 fino fapesp 
                    // 3 residuo da catagem do fino
                    // 4 perda de peso da catagem

                
                var indice_seco_cx =
                from linha in produto.AsEnumerable()
                where (linha.Field<decimal>("kgtotal") > 0M)
               // && (linha.Field<decimal>("kgparceiro") = 0M)
                && (linha.Field<DateTime>("datae") > inicio)
                && (linha.Field<DateTime>("datae") < fim)

                group linha by linha.Field<string>("Setor") into g
                select new
                {
                    setor = g.Key,
                    totalcaixas = g.Sum(linha => linha.Field<Decimal?>("Caixas")),
                    totalseco = g.Sum(linha => linha.Field<Decimal?>("kgtotal")),
                    totalfino = g.Sum(linha => linha.Field<Decimal?>("fino")),
                    totalfinorejeitado = g.Sum(linha => linha.Field<Decimal?>("finorejeito")),
                    totalfinofapesb = g.Sum(linha => linha.Field<Decimal?>("fapesbfino"))
                };
               
                // 
                Dictionary<string, decimal> ListIndice = new Dictionary<string, decimal>();
                //   decimal indice_ = 0M;
                foreach (var linha in indice_seco_cx)
                {
                    if (linha.totalcaixas == 0) continue;
                    if (linha.totalseco == 0) continue;
                    ListIndice.Add(linha.setor, (decimal)Math.Round(((double)linha.totalseco / (double)linha.totalcaixas), 4));
                }

                // composicao dos lotes em função do cacau fino (complementa informaçoes na produto)
                //
                // for (int ind1 = 0; ind1 < segue.Rows.Count;ind1++)
                // {
                //   DataRow rowsegue =


                Dados.Rows.Clear();
                Dados = null;
                Dictionary<string, TCLTCodigo> ListCodigo = TAtualizaFolha.GetDict_CLTCodigo();
                //string quadra = "";
                DataRow[] rowcltponto = null;
                // string lancado = "X";
                Dictionary<DateTime, decimal> ListMes = new Dictionary<DateTime, decimal>();
                List<string> lotes = new List<string>();
                Dictionary<DateTime, DataRow[]> DictRowsFolha = new Dictionary<DateTime, DataRow[]>(); 

                var dados_data =
                from linha in DadosFolha.AsEnumerable()
                group linha by linha.Field<DateTime>("DATA") into grupo
                orderby grupo.Key
                select grupo;
                DateTime MaxDataFolha = dados_data.Last().Key;
                DateTime MinDataFolha = dados_data.First().Key;


                var Cada_Quadra =
                    from linha in produto.AsEnumerable()
                    //where (linha.Field<string>("QUA").Trim() != "")
                    group linha by linha.Field<string>("QUA") into g
                    orderby g.Key
                    select g;

                DateTime LimiteSuperiorPesquisaCltPonto = DiaDoPonto(MaxDataFolha, DayOfWeek.Thursday);

                foreach (var quadra in Cada_Quadra)
                {
                    DataRow[] produtos = produto.Select("(QUA = '" + quadra.Key + "')", "DATAE DESC");
                    decimal caixas = 0;
                    decimal kgseco = 0;
                    decimal fino = 0;
                    decimal finorejeito = 0;
                    decimal fino_fapesb = 0;
                    string fapesb = "";
                    decimal areaha = 0;
                    //Boolean acumula_lotes = false;
                    int ultimo_ind = -1;
                    //    DateTime limiteinferior = inicio;
                    List<DataRow> rowslinha = new List<DataRow>();
                    for (int ind = 0; ind < produtos.Length; ind++)
                    {
                        DateTime limiteinferior = inicio;
                        DataRow orowprod = produtos[ind];
                        orowprod.BeginEdit();
                        orowprod["LANCADO"] = "";
                        orowprod.EndEdit();
                        orowprod.AcceptChanges();
                        if (orowprod["QUA"].ToString() == "") continue;

                        if (!Convert.IsDBNull(orowprod["FAPESBFINO"]))
                            if (Convert.ToDecimal(orowprod["FAPESBFINO"]) > 0)
                            {
                                fapesb = "X";
                            }
                            else
                                fapesb = "";
                        
                        rowslinha.Add(orowprod);                       
                        if ((ind + 1) < produtos.Length) // se não for o ultimo registro do produto...
                        {   // veja o proximo
                            DataRow orowprodnext = produtos[ind + 1];
                            ultimo_ind = (ind + 1);
                            limiteinferior = Convert.ToDateTime(orowprodnext["DATAE"]);
                            if ((Convert.ToDateTime(orowprod["DATAE"]) - limiteinferior).Days < 5)
                            {   if (!Convert.IsDBNull(orowprodnext["FAPESBFINO"]))
                                {
                                    if (Convert.ToDecimal(orowprodnext["FAPESBFINO"]) > 0)
                                    {
                                        fapesb = "X";
                                    }
                                    else
                                        fapesb = "";
                                }
                                rowslinha.Add(orowprodnext);
                                ind = ind + 1;
                                limiteinferior = inicio;
                                orowprod = orowprodnext;
                                if ((ind + 1) < produtos.Length)
                                {
                                    orowprodnext = produtos[ind + 1];
                                    if (orowprod["QUA"].ToString() == orowprodnext["QUA"].ToString())
                                    {
                                        ultimo_ind = (ind + 1);
                                        limiteinferior = Convert.ToDateTime(orowprodnext["DATAE"]);
                                    }
                                }
                            }
                            limiteinferior = limiteinferior.AddDays(1);
                        }

                        DateTime datapesquisa = Convert.ToDateTime(orowprod["DATAE"]);
                        if (datapesquisa.CompareTo(LimiteSuperiorPesquisaCltPonto) > 0)
                            datapesquisa = LimiteSuperiorPesquisaCltPonto;
                        rowcltponto = cltponto.Select("(BL = " + "'" + orowprod["QUA"] + "') " +
                               " AND (DATA <= '" + datapesquisa.ToString("d") + "') " +
                                    " AND (DATA >= '" + limiteinferior.ToString("d") + "') "
                                                         , "DATA DESC");
                        
                        // acumula_datas = false;
                        if ((rowcltponto.Length == 0) || ((fapesb == "X") && (orowprod["SETOR"].ToString() != " 9") && (orowprod["SETOR"].ToString() != " 4")))
                        {

                            if ((Convert.ToDecimal(orowprod["KGPARCEIRO"]) > 0)
                                || (Convert.ToDecimal(orowprod["KGTOTAL"]) == 0)
                            || (Convert.ToDateTime(orowprod["DATAE"]).AddDays(-20).CompareTo(inicio) < 0)
                                )
                            {
                               // acumula_lotes = false;
                                ultimo_ind = ind;
                                rowslinha.Clear();
                                continue;
                            }
                           
                            if (fapesb == "X")
                            {
                                fapesb = "";

                                DataRow oerro = FapesbProduto.NewRow();
                                oerro.ItemArray = orowprod.ItemArray;
                                orowprod.ItemArray.CopyTo(oerro.ItemArray, 0);
                                FapesbProduto.Rows.Add(oerro);
                                oerro.AcceptChanges();
                            }
                           else
                            {
                               DataRow oerro = ErrosProduto.NewRow();
                                oerro.ItemArray = orowprod.ItemArray;
                                orowprod.ItemArray.CopyTo(oerro.ItemArray, 0);
                                ErrosProduto.Rows.Add(oerro);
                                oerro.AcceptChanges();
                            }
                            continue;
                        }
                        ultimo_ind = ind;

                        DataRow rowarea = dtareas.Rows.Find(orowprod["QUA"]);
                        if (rowarea != null)
                            areaha = Convert.ToDecimal(rowarea["AREA"]);
                        else
                            areaha = 0;

                        decimal totaldiarias = 0.0M;

                        ListMes.Clear();
                        DictRowsFolha.Clear();
                        DateTime datapontocompara = DateTime.MinValue;
                        int dia_mudames = 7;
                        DateTime datapg = DateTime.MinValue;

                        foreach (DataRow orowcltponto in rowcltponto)
                        {
                            DateTime diadoponto = Convert.ToDateTime(orowcltponto["DATA"]);
                            if (!(datapontocompara.Equals(diadoponto)))
                            {
                                dia_mudames = 7;
                                datapontocompara = diadoponto;
                                datapg = TDataControlReduzido.UltimoDiaMes(diadoponto);

                                for (int j = 1; j < 7; j++)
                                {
                                    if (!(TDataControlReduzido.UltimoDiaMes(diadoponto.AddDays(j)).Equals(datapg)))
                                    {
                                        dia_mudames = j;
                                        break;
                                    }
                                }
                            }
                            datapg = TDataControlReduzido.UltimoDiaMes(diadoponto);
                            decimal diarias = 0;
                            for (int j = 1; j < (dia_mudames + 1); j++)
                            {
                                string campo = "DIA" + j.ToString().Trim();
                                if ((orowcltponto[campo] == null) || (orowcltponto[campo].ToString().Trim() == "")) continue;
                                TCLTCodigo ocodigo = ListCodigo[orowcltponto[campo].ToString()];
                                if (ocodigo == null) continue;
                                diarias += ocodigo.horas_efetivas;
                            }
                            if (diarias > 0)
                            {
                                if ((datapg >= MinDataFolha) && (datapg <= MaxDataFolha))
                                {
                                    if (!(ListMes.ContainsKey(datapg)))
                                    {
                                        DataRow[] orowfolha = DadosFolha.Select("(QUADRA = " + "'" + orowprod["QUA"] + "')" +
                                                                    " AND (DATA = " + "'" + datapg.ToString("d") + "' ) ", "SALARIO DESC");
                                        if (orowfolha.Length > 0)
                                        {
                                            DictRowsFolha.Add(datapg, orowfolha);
                                            ListMes.Add(datapg, diarias);
                                            totaldiarias = totaldiarias + diarias;
                                        }
                                    }
                                    else
                                    {
                                        ListMes[datapg] = ListMes[datapg] + diarias;
                                        totaldiarias = totaldiarias + diarias;
                                    }
                                }
                            }
                            if (dia_mudames != 7)
                            {
                                datapg = TDataControlReduzido.UltimoDiaMes(diadoponto.AddDays(dia_mudames));
                                diarias = 0;
                                for (int j = (dia_mudames + 1); j < 8; j++)
                                {
                                    string campo = "DIA" + j.ToString().Trim();
                                    if ((orowcltponto[campo] == null) || (orowcltponto[campo].ToString().Trim() == "")) continue;
                                    TCLTCodigo ocodigo = ListCodigo[orowcltponto[campo].ToString()];
                                    if (ocodigo == null) continue;
                                    diarias += ocodigo.horas_efetivas;
                                }
                                if (diarias > 0)
                                {

                                    if ((datapg >= MinDataFolha) && (datapg <= MaxDataFolha))
                                    {
                                        if (!(ListMes.ContainsKey(datapg)))
                                        {
                                            DataRow[] orowfolha = DadosFolha.Select("(QUADRA = " + "'" + orowprod["QUA"] + "')" +
                                                                   " AND (DATA = " + "'" + datapg.ToString("d") + "' ) ", "SALARIO DESC");
                                            if (orowfolha.Length > 0)
                                            {
                                                DictRowsFolha.Add(datapg, orowfolha);
                                                ListMes.Add(datapg, diarias);
                                                totaldiarias = totaldiarias + diarias;
                                            }
    
                                        }
                                        else
                                        {
                                            ListMes[datapg] = ListMes[datapg] + diarias;
                                            totaldiarias = totaldiarias + diarias;
                                        }
                                     
                                    }
                                    
                                }
                            }
                        }
                        if (ListMes.Count == 0)
                        {
                            rowslinha.Clear();
                            continue;
                        }

                        
                        if (totaldiarias == 0)
                        {
                            rowslinha.Clear();
                            continue;
                        }
                        
                        //fazer rotina para tirar daqui os dados que correspondam a datamax e minima
                        caixas = 0;
                        kgseco = 0;
                        fino = 0;
                        fino_fapesb = 0;

                        foreach (DataRow rowlin in rowslinha.AsEnumerable())
                        {
                            decimal lote_kgseco = Convert.ToDecimal(rowlin["KGTOTAL"]);
                            if (lote_kgseco == 0)
                            {
                                lote_kgseco = Convert.ToDecimal(rowlin["CAIXAS"]) * 30M;
                                if (ListIndice.ContainsKey(rowlin["SETOR"].ToString()))
                                    lote_kgseco = Convert.ToDecimal(rowlin["CAIXAS"]) * ListIndice[rowlin["SETOR"].ToString()];
                            }
                            //lote_kgseco
                            caixas = caixas + Convert.ToDecimal(rowlin["CAIXAS"]);
                            kgseco = kgseco + (decimal)lote_kgseco;
                            if (!Convert.IsDBNull(rowlin["FINO"]))
                                fino = fino + Convert.ToDecimal(rowlin["FINO"]);
                            if (!Convert.IsDBNull(rowlin["FAPESBFINO"]))
                                if (Convert.ToDecimal(rowlin["FAPESBFINO"]) > 0)
                                {
                                    fino_fapesb = fino_fapesb + Convert.ToDecimal(rowlin["FAPESBFINO"]);
                                }
                        }

                        decimal sobra_caixas = caixas;
                        decimal sobra_kgseco = kgseco;
                        decimal sobra_fino = fino;
                        decimal sobra_area = areaha;
                        decimal sobra_fino_fapesb = fino_fapesb;
                        
                        
                        foreach (KeyValuePair<DateTime, decimal> k_otrab in ListMes)
                        {
                            DateTime datafolha = k_otrab.Key;
                            if (k_otrab.Value == 0)
                                continue;
                           // DataRow[] orowfolha = DadosFolha.Select("(QUADRA = " + "'" + orowprod["QUA"].ToString() + "')" +
                             //                                       " AND (DATA = " + "'" + datafolha.ToString("d") + "' ) ", "SALARIO DESC");
                            DataRow[] orowfolha = DictRowsFolha[datafolha];

                            if (orowfolha.Length > 0)
                            {
                                DataRow rowfolha = orowfolha[0];
                                rowfolha.BeginEdit();
                                decimal rateiocaixas = Math.Round(k_otrab.Value / totaldiarias, 4) * caixas;

                                sobra_caixas = sobra_caixas - rateiocaixas;
                                if (sobra_caixas < 0.1M)
                                {
                                    rateiocaixas += sobra_caixas;
                                    sobra_caixas = 0;
                                }

                                decimal rateiokgseco = Math.Round(k_otrab.Value / totaldiarias, 4) * kgseco;
                                sobra_kgseco = sobra_kgseco - rateiokgseco;
                                if (sobra_kgseco < 0.5M)
                                {
                                    rateiokgseco += sobra_kgseco;
                                    sobra_kgseco = 0;
                                }

                                decimal rateiofino = Math.Round((k_otrab.Value / totaldiarias) * fino, 4);
                                if (sobra_fino < 0.5M)
                                {
                                    rateiofino += sobra_fino;
                                    sobra_fino = 0;
                                }
                                decimal rateiofino_fapesb = Math.Round((k_otrab.Value / totaldiarias) * fino_fapesb, 4);

                                if (sobra_fino_fapesb < 0.5M)
                                {
                                    rateiofino_fapesb += sobra_fino_fapesb;
                                    sobra_fino_fapesb = 0;
                                }


                                decimal rateioarea = Math.Round(k_otrab.Value / totaldiarias, 4) * areaha;
                                sobra_area = sobra_area - rateioarea;
                                if (sobra_area < 0.1M)
                                {
                                    rateioarea += sobra_area;
                                    sobra_area = 0;
                                }

                                if (rowfolha["CAIXAS"] == Convert.DBNull)
                                    rowfolha["CAIXAS"] = rateiocaixas;//Convert.ToDecimal(orowprod["CAIXAS"]);
                                else
                                    rowfolha["CAIXAS"] = Convert.ToDecimal(rowfolha["CAIXAS"]) + rateiocaixas;// Convert.ToDecimal(orowprod["CAIXAS"]);
                                if (rowfolha["SECO"] == Convert.DBNull)
                                    rowfolha["SECO"] = rateiokgseco;//Convert.ToDecimal(orowprod["SECO"]);
                                else
                                    rowfolha["SECO"] = Convert.ToDecimal(rowfolha["SECO"]) + rateiokgseco;// Convert.ToDecimal(orowprod["SECO"]);

                                if (rowfolha["FINO"] == Convert.DBNull)
                                    rowfolha["FINO"] = rateiofino;//Convert.ToDecimal(orowprod["FINO"]);
                                else
                                    rowfolha["FINO"] = Convert.ToDecimal(rowfolha["FINO"]) + rateiofino;// Convert.ToDecimal(orowprod["FINO"]);

                                if (rowfolha["FINO_FAPESB"] == Convert.DBNull)
                                    rowfolha["FINO_FAPESB"] = rateiofino_fapesb;//Convert.ToDecimal(orowprod["fino_fapesb"]);
                                else
                                    rowfolha["FINO_FAPESB"] = Convert.ToDecimal(rowfolha["FINO_FAPESB"]) + rateiofino_fapesb;// Convert.ToDecimal(orowprod["fino_fapesb"]);
                               
                                if (rowfolha["AREA"] == Convert.DBNull)
                                    rowfolha["AREA"] = rateioarea;//Convert.ToDecimal(orowprod["AREA"]);
                                else
                                    rowfolha["AREA"] = Convert.ToDecimal(rowfolha["AREA"]) + rateioarea;// 

                                rowfolha.EndEdit();

                                rowfolha.AcceptChanges();

                                foreach (DataRow rowfolha2 in orowfolha)
                                {
                                    rowfolha2.BeginEdit();
                                    int x = 1;
                                    for (int z = rowslinha.Count - 1; z > -1; z--)
                                    {
                                        if (x < 5)
                                            rowfolha2["LOTE" + x.ToString().Trim()] = rowslinha[z]["LOTE"].ToString() + "_" + Convert.ToDateTime(rowslinha[z]["DATAE"]).ToString("d");
                                        x += 1;
                                    }
                                    rowfolha2["LANCADO"] = "X";
                                    rowfolha2.EndEdit();
                                    rowfolha2.AcceptChanges();
                                }
                            }
                            else
                                continue;//  MessageBox.Show( "    Quadra:" + orowprod["QUA"].ToString() + "  Mes:" + Convert.ToDateTime(orowprod["DATAE"]).ToString("d"));

                        }
                        if ((sobra_caixas > 0) || (sobra_caixas < 0))
                        {
                            rowslinha.Clear();
                            continue; //MessageBox.Show("SobraCaixa: " + sobra_caixas.ToString() + "    Quadra:" + orowprod["QUA"].ToString() + "  Mes:" + Convert.ToDateTime(orowprod["DATAE"]).ToString("d"));

                        }
                        foreach (DataRow rowlin in rowslinha.AsEnumerable())
                        {
                            rowlin.BeginEdit();
                            rowlin["LANCADO"] = "X";
                            rowlin.EndEdit();
                            rowlin.AcceptChanges();
                        }
                        rowslinha.Clear();
                    }
                }
                produto.AcceptChanges();
                DataSet result = new DataSet();
                result.Tables.Add(DadosFolha);
                result.Tables.Add(DadosSecagem);
                result.Tables.Add(DadosGerais);
                result.Tables.Add(ErrosProduto);
                result.Tables.Add(FapesbProduto);
                result.Tables.Add(produto.Copy());
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}

/*
 * procedure TFrmFiltroProd.RateieNatura(DataSet: TDataSet);
var
i : integer;
bMark : TBookMark;
oParceria : TdadosParceiro;
begin
with DataSet as TTable do
begin
  beforePost = nil;
  AfterPost = nil;
  edit;
  FieldbyName('DATAP').asDAteTime = FieldbyName('DATAE').asDAteTime;
  FieldByName('QUANT_PA').asFloat = 0;
  FieldByName('QUANT_RI').asFloat = 0;
 // FieldbyName('CODPARC').AsString = '';
  //FieldbyName('NCONTRA').AsString = '';

  oParceria = TdadosParceiro.create(FieldbyName('QUA').asString,FieldbyName('GLEBA').asString,
       FieldbyName('PROD').asString,FIELDBYNAME('DATAE').asDateTime); // alterado em out/2007
//  if Ver_Parceria(FieldbyName('QUA').asString,FieldbyName('GLEBA').asString,
 //      FieldbyName('PROD').asString,FIELDBYNAME('DATAE').asDateTime) = true then
  if oParceria.EParceiro then
  begin
     if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Parceria <> 0 then
     begin
        FieldByName('QUANT_PA').asFloat = ptRound(FieldByName('QUANT').asFloat *
               (TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Parceria/100),3);
        FieldbyName('CODPARC').AsString = oparceria.codParc;
        FieldbyName('NCONTRA').AsString = oparceria.nContra;

     end;
     FieldByName('QUANT_FR').asFloat = FieldByName('QUANT').asFloat - FieldByName('QUANT_PA').asFloat;

     if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Reinvest <> 0 then
     begin
        FieldByName('QUANT_RI').asFloat = ptRound(FieldByName('QUANT_PA').asFloat *
               (TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Reinvest/100),3);
        FieldByName('QUANT_PA').asFloat = FieldByName('QUANT_PA').asFloat - FieldByName('QUANT_RI').asFloat;
     end;
  end
  else
     FieldByName('QUANT_FR').asFloat = FieldByName('QUANT').asFloat;
  post;
  BeforePost = tblProdMovBeforePost;
  AfterPost = tblProdMovAfterPost;
  oparceria.Free;

end;

end;

 */
/*
TobjProd = Class(TObject)
  cod : string;
  Natura : string;
  Unid_bene : string;
  Conversao : double;
  Parceria : double;
  Reinvest : double;
end;

TRegProd = Class(TObject)
  transp : TDateTime; // maior data campo do lote
  apronte : TDateTime; // data da chegada do lote cacau seco
  Natura : double;
  Benef : double;
  mudou : boolean;
end;
//como pega as informações do produto
CbProdutos.Clear;
  with dmtrab.tblCatProd do
  begin
    active = true;
    first;
    while not eof do
    begin
     if FieldByName('COD').isnull then
     begin
        next;
        continue;
     end;
     oProd = TObjProd.Create;
     oProd.cod     = FieldByName('COD').asString;
     oProd.Natura  = FieldByName('NATURA').asString;
     oProd.Unid_BENE  = FieldByName('Unid_Bene').asString;
     oProd.Conversao  = FieldByName('Conversao').asFloat;
     oProd.Parceria  = FieldByName('Parceria').asFloat;
     oProd.Reinvest  = FieldByName('Reinvest').asFloat;
     CbProdutos.Items.AddObject(FieldByName('DESC').asString,oProd);
     next;
    end;
    active = false;
  end;
  if cbProdutos.Items.Count > 0 then  cbProdutos.ItemIndex = 0;


 * 
 * 
 * * procedure TFrmFiltroProd.RateieLote(DataSet: TDataSet);
var
 tdif, tvalor,  maiornquant, tot1 : double;
 tquant_FR ,tquant_PA , tquant_RI : double;
 bMark , bMarkMaior : TBookMark;
 tregMaior : LongInt;
 totQuant, totquant_FR, totquant_PA, totquant_RI : double;
 nreg, i : integer;
 tdatap : TDateTime;
 tcodparc,tcontraparc : string;
 oParceria : TdadosParceiro;
begin

if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Conversao <> 0 then
begin
   //verifica se a taxa de conversao 'e coerente
   with DataSet as TTable do
   begin
     tdatap = FieldbyName('APRONTE').asDateTime;
     if (FieldbyName('BENEF').isNull) or (FieldbyName('NATURA').isNull) or
        (FieldbyName('BENEF').asFloat = 0) or (FieldbyName('NATURA').asFloat = 0)
     then
        tdif = PAD_DIF + 33
     else
     begin
        tdif   = ptRound((FieldbyName('BENEF').asFloat/FieldbyName('NATURA').asFloat),3)
              -    TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Conversao;
        tdif  = ptRound( ((tdif / TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Conversao) * 100),2);
     end;
     if (tdif > PAD_DIF) or (tdif < (-1 * PAD_DIF )) then
     begin
        if not(FieldbyName('BENEF').isNull) and not(FieldbyName('NATURA').isNull) then
        if lig_mensagem then   ShowMessage(' (Padrao <> Conversao) Fora do Padrao. Lote N.:'+ FieldbyName('LOTE').asString);
        with oProdMov.oTable do
        begin
          with oProdMov do
          for i = 0 to Campos.Count - 1 do
               if Campos.Soma[i] then
                   Campos.Total[i] = 0;
          if oProdMov.ListRecord.count > 0 then
          begin
         // nreg = recno;
            bMark = GetBookMark;
            disablecontrols;
            beforePost = nil;
            AfterPost = nil;
            first;
            while not eof do
            begin
              Edit;
              FieldbyName('QUANT').asFloat = 0;
              FieldbyName('QUANT_FR').asFloat = 0;
              FieldbyName('QUANT_PA').asFloat = 0;
              FieldbyName('QUANT_RI').asFloat = 0;
              FieldbyName('DATAP').asDAteTime = tdatap;
              FieldbyName('CODPARC').asString = '';
              FieldbyName('NCONTRA').asString = '';
              post;
              for i = 0 to oProdMov.Campos.Count - 1 do
              begin
                 if oProdMov.Campos.Soma[i]  then
                    oProdMov.Campos.Total[i]  = oProdMov.Campos.Total[i] + fieldbyName(oProdMov.Campos.Strings[i]).asFloat;
              end;
              next;
            end;
            GotoBookMark(bMark);
            FreeBookMark(bMark);
            enableControls;
            BeforePost = tblProdMovBeforePost;
            AfterPost = tblProdMovAfterPost;
          end;
        end;
        edit;
        FieldbyName('QUANT_FR').asFloat = 0;
        FieldbyName('QUANT_PA').asFloat = 0;
        FieldbyName('QUANT_RI').asFloat = 0;
        post;
        exit;
     end;
   end;
end;

with oProdMov.oTable do
begin
  if oProdMOv.ListRecord.Count < 1 then
  begin
    with DataSet as TTable do
    begin
      totquant_FR = 0;
      totquant_PA = 0;
      totquant_RI = 0;
      edit;
      FieldbyName('QUANT_FR').asFloat = TotQuant_FR;
      FieldbyName('QUANT_PA').asFloat = TotQuant_PA;
      FieldbyName('QUANT_RI').asFloat = TotQuant_RI;
      post;
    end;
    exit;
  end;

  if eof or bof then First;
  bMark = GetBookMark;
//  nreg = recno;
   disablecontrols;
   beforePost = nil;
   AfterPost = nil;
   with oProdMov do
   for i = 0 to Campos.Count - 1 do
     if Campos.Soma[i] then  Campos.Total[i] = 0;
   first;
   totQuant = 0;
   totquant_FR = 0;
   totquant_PA = 0;
   totquant_RI = 0;
   bMarkMaior = nil;
//  tregmaior = 0;
   maiornquant = 0;
   with DataSet as TTable do
   begin
    tot1 = FieldbyName('BENEF').asFloat;
    tdif = (FieldbyName('BENEF').asFloat/FieldbyName('NATURA').asFloat);
   end;
   while not eof do
   begin
     tvalor = ptROUND(FieldbyName('NQUANT').asFloat * tdif,2);
     tot1   = tot1 - tvalor;
     if FieldbyName('NQUANT').asFloat > maiornquant then // o destino da sobra do rateio vai p/ o maior valor
     begin
        maiornquant = FieldbyName('NQUANT').asFloat;
        bMarkMaior = GetBookMark;
       // tregmaior = recno;
     end;
     tquant_FR = 0;
     tquant_PA = 0;
     tquant_RI = 0;
     tcodparc= '';
     tcontraparc = '';
     oParceria = TdadosParceiro.create(FieldbyName('QUA').asString,FieldbyName('GLEBA').asString,
        FieldbyName('PROD').asString,FIELDBYNAME('DATAE').asDateTime);
     if OParceria.EParceiro then
  //   if Ver_Parceria(FieldbyName('QUA').asString,FieldbyName('GLEBA').asString,
   //     FieldbyName('PROD').asString,FIELDBYNAME('DATAE').asDateTime) = true then
     begin

        if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Parceria <> 0 then
        begin
            tquant_PA = ptRound(tvalor *
               (TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Parceria/100),3);
          tcodparc = oParceria.codParc;
          tcontraparc =  oParceria.nContra;
        end;
        tQuant_FR = (tvalor - tquant_PA);
        if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Reinvest <> 0 then
        begin
            tquant_RI = ptRound(tquant_PA *
               (TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Reinvest/100),3);
             tQUANT_PA = tQUANT_PA - tQuant_RI;
        end;
     end
     else
        tquant_FR = tvalor;

     Edit;
     FieldbyName('QUANT').asFloat = tValor;
     FieldbyName('QUANT_FR').asFloat = tQUANT_FR;
     FieldbyName('QUANT_PA').asFloat = tQUANT_PA;
     FieldbyName('QUANT_RI').asFloat = tQUANT_RI;
     FieldbyName('CODPARC').asstring = tcodparc;
     FieldbyName('NCONTRA').asstring = tcontraparc;
     FieldbyName('DATAP').asDAteTime = tdatap;

     Post;
     with oProdMov do
     for i = 0 to Campos.Count - 1 do
     begin
        if Campos.Soma[i]  then
           Campos.Total[i]  = Campos.Total[i] + fieldbyName(Campos.Strings[i]).asFloat;
     end;
     totQuant = totQuant + FieldbyName('QUANT').asFloat;
     totquant_FR = totquant_FR + FieldbyName('QUANT_FR').asFloat;
     totquant_PA = totquant_PA + FieldbyName('QUANT_PA').asFloat;
     totquant_RI = totquant_RI + FieldbyName('QUANT_RI').asFloat;
     next;
  end;
  if tot1 <> 0 then  // Coloca no Maior Valor a Diferenca que Houver do Rateio
  begin
     //GotoNReg(tregMaior);
     GotoBookMark(bMarkMaior);
     tvalor = FieldbyName('QUANT').asFloat  + tot1;
     tquant_FR = 0;
     tquant_PA = 0;
     tquant_RI = 0;
     totquant_FR = totquant_FR - FieldByName('QUANT_FR').asFloat;
     totquant_PA = totquant_PA - FieldByName('QUANT_PA').asFloat;
     totquant_RI = totquant_RI - FieldByName('QUANT_RI').asFloat;
     totquant = totquant - FieldByName('QUANT').asFloat;
     oParceria = TdadosParceiro.create(FieldbyName('QUA').asString,FieldbyName('GLEBA').asString,
        FieldbyName('PROD').asString,FIELDBYNAME('DATAE').asDateTime);
     if OParceria.EParceiro then
  //      if Ver_Parceria(FieldbyName('QUA').asString,FieldbyName('GLEBA').asString,
     //    FieldbyName('PROD').asString,FIELDBYNAME('DATAE').asDateTime) = true then
     begin
        if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Parceria <> 0 then
             tquant_PA = ptRound(tvalor *
               (TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Parceria/100),3);

        tQuant_FR = (tvalor - tquant_PA);
        if TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Reinvest <> 0 then
        begin
            tquant_RI = ptRound(tquant_PA *
               (TobjProd(CBProdutos.Items.Objects[CBProdutos.ItemIndex]).Reinvest/100),3);
             tQUANT_PA = tQUANT_PA - tQuant_RI;
        end;
     end
     else
        tquant_FR = tvalor;
     with oProdMov do
     for i = 0 to Campos.Count - 1 do
     if Campos.Soma[i]  then
         Campos.Total[i]  = Campos.Total[i] - fieldbyName(Campos.Strings[i]).asFloat;

     Edit;
     FieldbyName('QUANT').asFloat = tValor;
     FieldbyName('QUANT_FR').asFloat = tQUANT_FR;
     FieldbyName('QUANT_PA').asFloat = tQUANT_PA;
     FieldbyName('QUANT_RI').asFloat = tQUANT_RI;
     FieldbyName('DATAP').asDAteTime = tdatap;
     Post;
     with oProdMov do
     for i = 0 to Campos.Count - 1 do
        if Campos.Soma[i]  then
         Campos.Total[i]  = Campos.Total[i] + fieldbyName(Campos.Strings[i]).asFloat;

     totQuant = totQuant + FieldbyName('QUANT').asFloat;
     totquant_FR = totquant_FR + FieldbyName('QUANT_FR').asFloat;
     totquant_PA = totquant_PA + FieldbyName('QUANT_PA').asFloat;
     totquant_RI = totquant_RI + FieldbyName('QUANT_RI').asFloat;
  end;
  GotoBookMark(bMark);
  FreeBookMark(bMark);
  if bMarkMaior <> nil then
       FreeBookMark(bMarkMaior);
 // gotonreg(nreg);
  enableControls;
  BeforePost = tblProdMovBeforePost;
  AfterPost = tblProdMovAfterPost;
end;
with DataSet as TTable do
begin
   edit;
   FieldbyName('QUANT_FR').asFloat = TotQuant_FR;
   FieldbyName('QUANT_PA').asFloat = TotQuant_PA;
   FieldbyName('QUANT_RI').asFloat = TotQuant_RI;
   post;
end;



end;


 */



/*
 * 
 */ 

/*if (orowprod["QUA"].ToString().Trim() == "")
                 {
                     DataRow oerro = ErrosProduto.NewRow();
                     oerro.ItemArray = orowprod.ItemArray;
                     orowprod.ItemArray.CopyTo(oerro.ItemArray, 0);
                     ErrosProduto.Rows.Add(oerro);
                     oerro.AcceptChanges();
                     lotes.Clear();
                     caixas = 0;
                     kgseco = 0;
                     fino = 0;
                     continue;
                 }*/
