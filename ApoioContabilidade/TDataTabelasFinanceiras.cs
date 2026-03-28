using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using ClassConexao;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    class TDataTabelasFinanceiras
    {



        static public OleDbDataAdapter ConstruaAdaptador()
        {

            string path = TDataControlReduzido.Get_Path("FINAN");

            try
            {

                OleDbDataAdapter OleDbda = new OleDbDataAdapter();

                OleDbCommand cmd = new OleDbCommand("INSERT INTO " + path + "MOV_FIN ( ID,DATA,VALOR,DEBITO,CREDITO, " +
                      " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,DATA_EMI,TP_OK,TIPO_DOC, " +
                      " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                        "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF , MUDA_INT )" +

                       " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?,?,?,?)"
                        , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@ID", OleDbType.Numeric, 0, "ID");
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = true;
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
                //cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 0, "NUMREG");//RECNO()
                cmd.Parameters.Add("@DATA_EMI", OleDbType.Date, 8, "DATA_EMI");
                cmd.Parameters.Add("@TP_OK", OleDbType.Boolean, 1).Value = false;
                cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char, 1).Value = " ";// Char.Parse("");

                cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric,0,"MOV_ID");
                cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");
                cmd.Parameters.Add("@MUDA_INT", OleDbType.Boolean).Value = false;
               
                OleDbda.InsertCommand = cmd;

                cmd = new OleDbCommand("UPDATE " + path + "MOV_FIN SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                    " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TP_FIN = ?,TIPO = ?" +
                       "WHERE  (MOV_ID () = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = true;
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");

                OleDbParameter parm;
                parm = cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");
                parm.SourceVersion = DataRowVersion.Original;

                // da.UpdateCommand = cmd;
                OleDbda.UpdateCommand = cmd;

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM " + path + "MOV_FIN WHERE (MOV_ID = ?)",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbParameter parameter2 = command.Parameters.Add(
                   "@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");
                parameter2.SourceVersion = DataRowVersion.Original;


                OleDbda.DeleteCommand = command;

                // commnado Retorna maior numero
                command = new OleDbCommand(
                       "SELEC MAX(MOV_ID) AS numero FROM " + path + "MOV_FIN",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbda.SelectCommand = command;
                return OleDbda;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "MOV_FIN"));
            }
        }
         

        static public List<int> MaiorMov_id_ID()
        {
            List<int> oListNum = new List<int>(); 
            string path = TDataControlReduzido.Get_Path("FINAN");
            OleDbCommand command = new OleDbCommand(
                    "SELEC MAX(MOV_ID) AS numero, MAX(ID) AS nID FROM " + path + "MOV_FIN",
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            //int numero = 0;
            try
            {
                if (TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().State == ConnectionState.Closed)
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();
                //ExecuteReader();
                OleDbDataReader leitor = command.ExecuteReader();
                if (leitor.HasRows)
                {
                    if (leitor.Read())
                    {
                        oListNum.Add(Convert.ToInt32(leitor["Numero"]));
                        oListNum.Add(Convert.ToInt32(leitor["nid"]));
                    }
                }

 
                return oListNum;
            }
            catch (Exception)
            {
                return oListNum;
            }
        }

        static public OleDbDataAdapter Peg_PagamentosFolha(ref DataSet odataset, DateTime data1, DateTime data2, List<LinhaSolucao> oLista, string tipo)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            //  DataSet result = null;
            path = TDataControlReduzido.Get_Path("FINAN");
            setoroledb = "SELECT *  FROM " + path + "MOV_FIN  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (TP_FIN = .T.) AND TIPO = '" + tipo + "'";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            setoroledb += " ORDER BY DATA,MOV_ID";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = ConstruaAdaptador();
                oledbda.SelectCommand = oledbcomm;
                oledbda.TableMappings.Add("Table", "MOV_FIN");
                oledbda.Fill(odataset);
                //     odataset.Tables[0].Columns.Add("CONTA", typeof(string));
               // odataset.Tables[0].Columns.Add("CONTAFIN", typeof(string)).MaxLength = 2;
                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                  //  if (tipo == "P")
                  //      odatarow["CONTAFIN"] = ((string)odatarow["DEBITO"]).Substring(0, 2);
                   // else
                   //     odatarow["CONTAFIN"] = ((string)odatarow["CREDITO"]).Substring(0, 2);

                    if (Convert.ToString(odatarow["DEBITO"]).Trim() != "PROV # FOLHA")
                        odatarow.Delete();
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa)
                                {
                                    odatarow.Delete();
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            odatarow.Delete();
                        }
                    }


                }
                odataset.Tables[0].AcceptChanges();
                return oledbda;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }


        static public void AcertaMov_Id( )
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            //  DataSet result = null;
            OleDbDataAdapter oledbda = new OleDbDataAdapter();

            
              


            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT RECNO() as NREG, *   FROM " + path + "MOVFIN  where (MOV_ID = 0) ";

            setoroledb += " ORDER BY DATA,NREG";

            OleDbCommand cmd = new OleDbCommand("UPDATE " + path + "MOVFIN SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                  " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TP_FIN = ?,TIPO = ?,MOV_ID = ?" +
                     "WHERE  (RECNO() = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
            cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
            cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
            cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
            cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
            cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
            cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
            cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
            cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
            cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = true;
            cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
            cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 0, "MOV_ID");

            OleDbParameter parm;
            parm = cmd.Parameters.Add("@NREG", OleDbType.Numeric, 10, "NREG");
            parm.SourceVersion = DataRowVersion.Original;



            try
            {
                DataSet odataset = new DataSet();
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                //OleDbDataAdapter oledbda = ConstruaAdaptador();
              
                oledbda.SelectCommand = oledbcomm;
                oledbda.UpdateCommand = cmd;



                oledbda.TableMappings.Add("Table", "MOVFIN");
                oledbda.Fill(odataset);
                List<int> maiormov_id_ID = MaiorMov_id_ID();
                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    odatarow.BeginEdit();
                  //  maiormov_id += 1;
                    maiormov_id_ID[0] += 1;
                    maiormov_id_ID[1] += 1;
                    odatarow["MOV_ID"] = (object)maiormov_id_ID[0];
                    odatarow["ID"] = (object)maiormov_id_ID[1];
                    odatarow.EndEdit();
                }
                oledbda.Update(odataset.Tables[0]);
                    
           
                odataset.Tables[0].AcceptChanges();
                return ;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }
        


    }
}
