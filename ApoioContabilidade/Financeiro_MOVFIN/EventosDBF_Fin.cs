using ApoioContabilidade.Financeiro_MOVFIN.Servicos;
using ApoioContabilidade.Models;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Services.Description;
using System.Windows.Forms;
using static ApoioContabilidade.Financeiro_MOVFIN.Servicos.ServicosFiltroDBF;

namespace ApoioContabilidade.Financeiro_MOVFIN
{
    public class EventosDBF_Fin
    {
        public BindingSource bmPagar;
        public BindingSource bmReceber;
        public DataSet dsFiltrado;
        public EventosDBF_Fin(DataSet odsFiltrado)
        {
            dsFiltrado = odsFiltrado;
            bmPagar = new BindingSource();
            bmReceber = new BindingSource();
        }

        public  void Rec_Fin_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (
                  (orow["CREDITO"].ToString().Trim() == "") && (orow["DEBITO"].ToString().Trim() == "")
                  || (orow["DEBITO"].ToString().Trim().Length != 2)
                  || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            
            try
            {
                // if (Convert.ToBoolean(orow["TP_FIN"]) )
                //{
          //      orow["TP_FIN"] = true;
                //}

                orow["TIPO"] = "R";
                orow["VENC"] = orow["DATA"];


                // para evitar valores nullos use defaults 
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {

                        //if (col.ColumnName.ToUpper() == "ID") continue;
                        Type tipo = col.DataType;

                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            try
                            {
                                orow[col.ColumnName] = DateTime.MinValue;
                            }
                            catch (Exception) { }
                        }
                        else if (tipo == Type.GetType("System.String"))
                        {
                            orow[col.ColumnName] = "";
                        }
                        else if (tipo == Type.GetType("System.Boolean"))
                        {
                            orow[col.ColumnName] = 0;
                        }
                        else
                        {
                            orow[col.ColumnName] = 0;
                        }

                    }

                }
            
                //TDataControlContab.PonhaValoresDefault(orow);

                
                try
                {
                    OleDbCommand cmd;
                    if (e.TipoMuda == DataRowState.Added)
                    {

                        cmd = new OleDbCommand("INSERT INTO " + FiltroFinanDBF.path + "MOVFIN" + " ( DATA,VALOR,DEBITO,CREDITO, " +
                           " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,DATA_EMI,TP_OK,TIPO_DOC, " +
                           " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                             "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF)" +

                            " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?)"
                             , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        cmd.Parameters.Add("@DATA", OleDbType.Date).Value = orow["DATA"];
                        cmd.Parameters.Add("@VALOR", OleDbType.Double).Value = orow["VALOR"];
                        cmd.Parameters.Add("@DEBITO", OleDbType.Char).Value = orow["DEBITO"];
                        cmd.Parameters.Add("@CREDITO", OleDbType.Char).Value = orow["CREDITO"];
                        cmd.Parameters.Add("@FORN", OleDbType.Char).Value = orow["FORN"]    ;
                        cmd.Parameters.Add("@HIST", OleDbType.Char).Value = orow["HIST"];
                        cmd.Parameters.Add("@DOC", OleDbType.Char).Value = orow["DOC"];
                        cmd.Parameters.Add("@DOC_FISC", OleDbType.Char).Value = orow["DOC_FISC"];
                        cmd.Parameters.Add("@VENC", OleDbType.Date).Value = orow["VENC"];
                        cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean).Value = orow["TP_FIN"];
                        cmd.Parameters.Add("@TIPO", OleDbType.Char).Value = orow["TIPO"];
                        // cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 0, "NUMREG");//RECNO()
                        cmd.Parameters.Add("@DATA_EMI", OleDbType.Date).Value = orow["DATA_EMI"];
                        cmd.Parameters.Add("@TP_OK", OleDbType.Boolean).Value = true;
                        cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char).Value = " ";// Char.Parse("");

                        cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                        cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                        cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                        cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                        cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric).Value = orow["MOV_ID"];
                        cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                        cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                        cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");

                        int nreg = cmd.ExecuteNonQuery();
                        if (nreg > 0)
                        {

                            OleDbCommand oledbcomm = new OleDbCommand("SELECT MAX(RECNO()) AS MAX_NREG FROM " +
                              FiltroFinanDBF.path + "MOVFIN", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                            OleDbDataAdapter dataAdpter = new OleDbDataAdapter();
                            dataAdpter.SelectCommand = oledbcomm;
                            dataAdpter.TableMappings.Clear();
                            dataAdpter.TableMappings.Add("Table", "MAXMOVFIN");
                            DataSet odataset = new DataSet();
                            dataAdpter.Fill(odataset);
                            try
                            {
                                nreg = Convert.ToInt32(odataset.Tables[0].Rows[0][0]);
                                orow["NREG"] = nreg;
                            }
                            catch (Exception E)
                            {
                                e.Cancela = true;
                                MessageBox.Show("Operação Falhou: " + E.ToString());
                            }

                        }
                        else
                        {
                            e.Cancela = true;
                            MessageBox.Show("Operação Falhou");
                        }




                    }
                    else
                    {
                        cmd = new OleDbCommand("UPDATE " + FiltroFinanDBF.path + "MOVFIN" + " SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                       " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TIPO = ?" +
                          "WHERE  (RECNO() = "+orow["NREG"].ToString()+")", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        
                        //OleDbParameter parm;
                        //parm = cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");

                        cmd.Parameters.Add("@DATA", OleDbType.Date,8).Value = orow["DATA"];
                        cmd.Parameters.Add("@VALOR", OleDbType.Double,0).Value = orow["VALOR"];
                        cmd.Parameters.Add("@DEBITO", OleDbType.Char,25).Value = orow["DEBITO"];
                        cmd.Parameters.Add("@CREDITO", OleDbType.Char,25).Value = orow["CREDITO"];
                        cmd.Parameters.Add("@FORN", OleDbType.Char, 35).Value = orow["FORN"];
                        cmd.Parameters.Add("@HIST", OleDbType.Char,40).Value = orow["HIST"];
                        cmd.Parameters.Add("@DOC", OleDbType.Char,13).Value = orow["DOC"];
                        cmd.Parameters.Add("@DOC_FISC", OleDbType.Char,13).Value = orow["DOC_FISC"];
                        cmd.Parameters.Add("@VENC", OleDbType.Date,8).Value = orow["VENC"];
                        cmd.Parameters.Add("@TIPO", OleDbType.Char,1).Value = orow["TIPO"];
                        cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric,10 ).Value = orow["MOV_ID"];

                        int nreg = cmd.ExecuteNonQuery();

                       // FiltroFinanDBF.adapterMovFin.Update(new DataRow[] { orow });
                    }
                }
                catch (Exception E)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro SAIDAS "+E.Message); return;

                }
                
                //movFinView.AcceptChanges();
                //Totaliza();


                /*bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_FIN"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                   
                }*/

                // dsFiltrado.Tables["RMOV_FIN"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public void Rec_movFin_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            TDataControlContab.PonhaValoresDefault(e.Row);
            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "R";
            e.Row["DOC"] = "";
            e.Row["TP_FIN"] = 1;
            e.Row["DEBITO"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            e.Row["OBS"] = "";
            e.Row["FORN"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DEBITO"] = "00";
        }

        public void Pag_movFin_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            TDataControlContab.PonhaValoresDefault(e.Row);
            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "P";
            e.Row["DOC"] = "";
            e.Row["TP_FIN"] = 1;
            e.Row["DEBITO"] = "";   
            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            e.Row["OBS"] = "";
            e.Row["FORN"] = "";
            e.Row["CREDITO"] = "00";
            e.Row["DEBITO"] = "";
        }

        public void Rec_Fin_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            try
            {
                if (
                      (orow["CREDITO"].ToString().Trim() == "") && (orow["DEBITO"].ToString().Trim() == "")
                       || (orow["DEBITO"].ToString().Trim().Length != 2)
                        || (Convert.ToDecimal(orow["VALOR"]) == 0)
                        || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                        || (orow.IsNull("DATA")))
                {
                    MessageBox.Show("Dados Inconsistentes com padrão");
                    e.Cancela = true;
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Dados Inconsistente! Existem campos nulos");
                e.Cancela = true;
                return;
            }

            bool result = true;

            if ((orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Detached) || (orow.RowState == DataRowState.Unchanged))
            { return; }

            if ((!orow.IsNull("outro_id")) && (Convert.ToDouble(orow["outro_id"]) != 0)) // em tese oriundo do pgrc
            {
                if ((orow.RowState == DataRowState.Modified))
                {
                    List<string> camposNaoAltera = new List<string>();
                    camposNaoAltera.Add("CREDITO");
                    camposNaoAltera.Add("VALOR");
                    camposNaoAltera.Add("DOC_FISC");
                    foreach (string campo in camposNaoAltera)
                    {
                        if (!orow.Table.Columns.Contains(campo)) continue;
                        DataColumn ocol = orow.Table.Columns[campo];
                        if ((orow[ocol, DataRowVersion.Current].ToString().Trim() == "") && (orow[ocol, DataRowVersion.Original].ToString().Trim() == ""))
                            continue;
                        if (orow[ocol, DataRowVersion.Current].ToString() != orow[ocol, DataRowVersion.Original].ToString())
                        {
                            result = false;
                            orow[ocol] = orow[ocol, DataRowVersion.Original];
                            //break;
                        }
                    }
                }
            }
            if (!result)
            {
                MessageBox.Show("Não pode Alterar Campos Valor e/ou Cliente ou Doc FIscal");
                e.Cancela = true;
            }

        }
        public void Pag_Fin_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            try
            {
                if (
                 (orow["CREDITO"].ToString().Trim() == "") && (orow["DEBITO"].ToString().Trim() == "")
                  || (orow["CREDITO"].ToString().Trim().Length != 2)
                   || (Convert.ToDecimal(orow["VALOR"]) == 0)
                   || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                   || (orow.IsNull("DATA")))
                {
                    MessageBox.Show("Dados Inconsistentes com padrão");
                    e.Cancela = true;
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Dados Inconsistente! Existem campos nulos");
                e.Cancela = true;
                return;
            }

            bool result = true;
            if ((!orow.IsNull("OUTRO_ID")) && (Convert.ToDouble(orow["OUTRO_ID"]) != 0))
            {
                if ((orow.RowState == DataRowState.Modified) || (orow.RowState == DataRowState.Unchanged))
                {
                    List<string> camposNaoAltera = new List<string>();
                    camposNaoAltera.Add("DEBITO");
                    camposNaoAltera.Add("VALOR");
                    camposNaoAltera.Add("DOC_FISC");
                    foreach (string campo in camposNaoAltera)
                    {
                        if (!orow.Table.Columns.Contains(campo)) continue;
                        DataColumn ocol = orow.Table.Columns[campo];
                        if ((orow[ocol, DataRowVersion.Current].ToString().Trim() == "") && (orow[ocol, DataRowVersion.Original].ToString().Trim() == ""))
                            continue;
                        if (orow[ocol, DataRowVersion.Current].ToString() != orow[ocol, DataRowVersion.Original].ToString())
                        {
                            result = false;

                            orow[ocol] = orow[ocol, DataRowVersion.Original];
                            // break;
                        }
                    }
                }
            }
            if (!result)
            {
                MessageBox.Show("Não pode Alterar Campos Valor e/ou Fornecedor ou Doc FIscal");
                e.Cancela = true;
            }

        }

        public void Pag_Fin_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if (
                  (orow["CREDITO"].ToString().Trim() == "") && (orow["DEBITO"].ToString().Trim() == "")
                    || (orow["CREDITO"].ToString().Trim().Length != 2)
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            try
            {
                //orow["TP_FIN"] = 1;


                orow["TIPO"] = "P";
                orow["VENC"] = orow["DATA"];


                // para evitar valores nullos use defaults 
                

                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        Type tipo = col.DataType;

                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            try
                            {
                                orow[col.ColumnName] = DateTime.MinValue;
                            }
                            catch (Exception) { }
                        }
                        else if (tipo == Type.GetType("System.String"))
                        {
                            orow[col.ColumnName] = "";
                        }
                        else if (tipo == Type.GetType("System.Boolean"))
                        {
                            orow[col.ColumnName] = 0;
                        }
                        else
                        {
                            orow[col.ColumnName] = 0;
                        }

                    }
                }
                try
                {
                    OleDbCommand cmd;
                    if (e.TipoMuda == DataRowState.Added)
                    {

                        cmd = new OleDbCommand("INSERT INTO " + FiltroFinanDBF.path + "MOVFIN" + " ( DATA,VALOR,DEBITO,CREDITO, " +
                           " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,DATA_EMI,TP_OK,TIPO_DOC, " +
                           " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                             "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF)" +

                            " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?)"
                             , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        cmd.Parameters.Add("@DATA", OleDbType.Date).Value = orow["DATA"];
                        cmd.Parameters.Add("@VALOR", OleDbType.Double).Value = orow["VALOR"];
                        cmd.Parameters.Add("@DEBITO", OleDbType.Char).Value = orow["DEBITO"];
                        cmd.Parameters.Add("@CREDITO", OleDbType.Char).Value = orow["CREDITO"];
                        cmd.Parameters.Add("@FORN", OleDbType.Char).Value = orow["FORN"];
                        cmd.Parameters.Add("@HIST", OleDbType.Char).Value = orow["HIST"];
                        cmd.Parameters.Add("@DOC", OleDbType.Char).Value = orow["DOC"];
                        cmd.Parameters.Add("@DOC_FISC", OleDbType.Char).Value = orow["DOC_FISC"];
                        cmd.Parameters.Add("@VENC", OleDbType.Date).Value = orow["VENC"];
                        cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean).Value = orow["TP_FIN"];
                        cmd.Parameters.Add("@TIPO", OleDbType.Char).Value = orow["TIPO"];
                        // cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 0, "NUMREG");//RECNO()
                        cmd.Parameters.Add("@DATA_EMI", OleDbType.Date).Value = orow["DATA_EMI"];
                        cmd.Parameters.Add("@TP_OK", OleDbType.Boolean).Value = true;
                        cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char).Value = " ";// Char.Parse("");

                        cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                        cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                        cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                        cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                        cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric).Value = orow["MOV_ID"];
                        cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                        cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                        cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                        cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");

                        int nreg = cmd.ExecuteNonQuery();
                        if (nreg > 0)
                        {

                            OleDbCommand oledbcomm = new OleDbCommand("SELECT MAX(RECNO()) AS MAX_NREG FROM " +
                              FiltroFinanDBF.path + "MOVFIN", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                            OleDbDataAdapter dataAdpter = new OleDbDataAdapter();
                            dataAdpter.SelectCommand = oledbcomm;
                            dataAdpter.TableMappings.Clear();
                            dataAdpter.TableMappings.Add("Table", "MAXMOVFIN");
                            DataSet odataset = new DataSet();
                            dataAdpter.Fill(odataset);
                            try
                            {
                                nreg = Convert.ToInt32(odataset.Tables[0].Rows[0][0]);
                                orow["NREG"] = nreg;
                            }
                            catch (Exception E)
                            {
                                e.Cancela = true;
                                MessageBox.Show("Operação Falhou: " + E.ToString());
                            }

                        }
                        else
                        {
                            e.Cancela = true;
                            MessageBox.Show("Operação Falhou");
                        }

                    }
                    else
                    {
                        cmd = new OleDbCommand("UPDATE " + FiltroFinanDBF.path + "MOVFIN" + " SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                      " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TIPO = ?" +
                         "WHERE  (RECNO() = "+ orow["NREG"].ToString()+ ")", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        cmd.Parameters.Add("@DATA", OleDbType.Date).Value = orow["DATA"];
                        cmd.Parameters.Add("@VALOR", OleDbType.Double).Value = orow["VALOR"];
                        cmd.Parameters.Add("@DEBITO", OleDbType.Char).Value = orow["DEBITO"];
                        cmd.Parameters.Add("@CREDITO", OleDbType.Char).Value = orow["CREDITO"];
                        cmd.Parameters.Add("@FORN", OleDbType.Char).Value = orow["FORN"];
                        cmd.Parameters.Add("@HIST", OleDbType.Char).Value = orow["HIST"];
                        cmd.Parameters.Add("@DOC", OleDbType.Char).Value = orow["DOC"];
                        cmd.Parameters.Add("@DOC_FISC", OleDbType.Char).Value = orow["DOC_FISC"];
                        cmd.Parameters.Add("@VENC", OleDbType.Date).Value = orow["VENC"];
                        cmd.Parameters.Add("@TIPO", OleDbType.Char).Value = orow["TIPO"];
                        cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric).Value = orow["MOV_ID"];

                        int nreg = cmd.ExecuteNonQuery();


                    }
                 //   FiltroFinanDBF.adapterMovFin.Update(new DataRow[] { orow });

                }
                catch (Exception)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro  "); return;

                }
                //movFinView.AcceptChanges();
                //Totaliza();
                /*   bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_FIN"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                   if (!ok)
                   {

                   } */
            
            

                // dsFiltrado.Tables["PMOV_FIN"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public  void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            bool ok = true;
            string tabela = orow.Table.TableName.ToUpper().Trim().Substring(1);
            try
            {
                OleDbCommand command = new OleDbCommand(
                          "DELETE FROM " + FiltroFinanDBF.path + "MOVFIN" + " WHERE (RECNO() = "+ orow["NREG"].ToString() + ")",
                          TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
             
                // parameter2.SourceVersion = DataRowVersion.Original;
                int nreg = command.ExecuteNonQuery();
                if (nreg > 0)
                    ok = true;

                //ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, tabela);
            }
            catch (Exception)
            {
            }
            // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Deletar Registro " + tabela); return;
            }
        }
        public void Padrao_BeforeDeletaRegistros(object sender, AlteraRegistroEventArgs e)
        {
            // ATENçÂO => ANTES DE DELETAR O MESTRE É NECESSÁRIO VERIFICAR SE EXISTEM REGISTROS LIGADOS A ESTE REGISTRO
            DataRow orow = e.Rows[0];
            if (!orow.IsNull("OUTRO_ID"))
            {
                if (Convert.ToDouble(orow["OUTRO_ID"]) != 0)
                {
                    e.Cancela = true;
                    MessageBox.Show("Para Deletar Use o Pagar&Receber"); return;
                }
            }

        }
        public void EditeMovFin_DeletaRegistrosAdapter(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            try
            {
                OleDbCommand command = new OleDbCommand(
                           "DELETE FROM " + FiltroFinanDBF.path + "MOVFIN" + " WHERE (RECNO() = "+orow["NREG"].ToString()+")",
                           TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                 command.ExecuteNonQuery();

//                FiltroFinanDBF.adapterMovFin.Update(new DataRow[] { orow }); ;
//movFinView.AcceptChanges();
//Totaliza();
            }
            catch (Exception E)
            {
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;
            }

        }






        public  bool InsereTransferencia(string tdeb, string tcred, BindingSource bmSourceDe, BindingSource bmSourcePara)
        {
      /*      DataRow orowDe = (bmSourceDe.Current as DataRowView).Row;

            DataTable tableDestino = bmSourcePara.DataSource as DataTable;
            DataRow orowPara = tableDestino.NewRow();
            foreach (DataColumn ocol in orowDe.Table.Columns)
            {
                orowPara[ocol.ColumnName] = orowDe[ocol.ColumnName];
            }
            if (orowDe["TIPO"].ToString() == "P")
            { orowPara["TIPO"] = "R"; }
            else
                orowPara["TIPO"] = "P";

            orowPara["DEBITO"] = tdeb;
            orowPara["CREDITO"] = tcred;

            double mov_id = -1;
            try
            {
                DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                { "SELECT MAX(MOV_ID) AS MAX_MOV_ID FROM MOV_FIN" }));
                mov_id = Convert.ToDouble(odataset.Tables[0].Rows[0][0]);
            }
            catch (Exception) { }
            if (mov_id == -1)
            {
                MessageBox.Show("Erro ao buscar numero exclusivo");

                return false;
            }

            orowPara["MOV_ID"] = mov_id + 1;

            try
            {
                tableDestino.Rows.Add(orowPara);
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orowPara, DataRowState.Added, "MOV_FIN"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    tableDestino.RejectChanges();
                    MessageBox.Show("Erro ao Inserir Registro  "); return false;
                }
                // tableDestino.Rows.Add(orowPara);
                tableDestino.AcceptChanges();

            }
            catch (Exception E)
            {

                MessageBox.Show("Operação Falhou: " + E.ToString());
                return false;

            } */
            return true;
        }
    }
}
