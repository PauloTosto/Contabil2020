using ApoioContabilidade.Models;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ApoioContabilidade.FinVauche_MOVFIN.Servicos.FiltroVauchesDBF;
using ApoioContabilidade.FinVauche_MOVFIN.Servicos;

namespace ApoioContabilidade.FinVauche_MOVFIN
{
    public class EventosDBF_Vauches
    {

       // public BindingSource bmSource;
        public BindingSource bmVauches;
        public DataSet dsFiltrado;
        public EventosDBF_Vauches(DataSet odsFiltrado)
        {
            dsFiltrado = odsFiltrado;
         //   bmSource = new BindingSource();
            bmVauches = new BindingSource();

        }

        public void Vauches_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (
                  (orow["CREDITO"].ToString().Trim() == "") && (orow["DEBITO"].ToString().Trim() == "")
                  || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
           
            bool tipofinDeb = false;
            bool tipofinCre = false;
            string debFin = "";
            string creFin = "";
            if (orow["DEBITO"].ToString().Trim().Length > 0)
            {
                debFin = orow["DEBITO"].ToString().Trim().Substring(0, 2);
            }
            if (orow["CREDITO"].ToString().Trim().Length > 0)
            {
                creFin = orow["CREDITO"].ToString().Trim().Substring(0, 2);

            }
            if (debFin != "")
            {
                try
                {
                    int i = Convert.ToInt32(debFin);
                    tipofinDeb = true;
                    if (orow["DEBITO"].ToString().Trim().Length > 2)
                    {
                        tipofinCre = false;
                    }
                }
                catch { }
            }
            if (creFin != "")
            {

                try
                {
                    int i = Convert.ToInt32(creFin);
                    tipofinCre = true;
                    if (orow["CREDITO"].ToString().Trim().Length > 2)
                    {
                        tipofinCre = false;
                    }
                }
                catch { }
            }
            if ((Convert.ToBoolean(orow["TP_FIN"]) == false) && (tipofinCre || tipofinDeb))
            {
                MessageBox.Show("Lancamento é  VAUCHE! Não permite conta numero Banco ");
                e.Cancela = true;
                return;
            }
            if ((Convert.ToBoolean(orow["TP_FIN"]) == true) && (!tipofinCre) && !(tipofinDeb))
            {
                MessageBox.Show("Lancamento é FINANCEIRO! Pelo menos uma conta Banco!!  ");
                e.Cancela = true;
                return;
            }

            if (tipofinCre & tipofinDeb)
            {
                MessageBox.Show("Duas Contas Bancos????");
                e.Cancela = true;
                return;
            }


            try
            {
                // if (Convert.ToBoolean(orow["TP_FIN"]) )
                //{
                //      orow["TP_FIN"] = true;
                //}


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

                        cmd = new OleDbCommand("INSERT INTO " + FiltroVauchesDBF.path + "MOVFIN" + " ( DATA,VALOR,DEBITO,CREDITO, " +
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
                              FiltroVauchesDBF.path + "MOVFIN", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
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

                        } else
                        {
                            e.Cancela = true;
                            MessageBox.Show("Operação Falhou");
                        }


                    }
                    else
                    {
                        cmd = new OleDbCommand("UPDATE " + FiltroVauchesDBF.path + "MOVFIN" + " SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                       " FORN = ?,HIST = ?,DOC = ?,  DOC_FISC = ?,VENC=? ,TIPO = ?" +
                          "WHERE  (RECNO() = "+ orow["NREG"].ToString() + ")", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                        //OleDbParameter parm;
                        //parm = cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");

                        cmd.Parameters.Add("@DATA", OleDbType.Date, 8).Value = orow["DATA"];
                        cmd.Parameters.Add("@VALOR", OleDbType.Double, 0).Value = orow["VALOR"];
                        cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25).Value = orow["DEBITO"];
                        cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25).Value = orow["CREDITO"];
                        cmd.Parameters.Add("@FORN", OleDbType.Char, 35).Value = orow["FORN"];
                        cmd.Parameters.Add("@HIST", OleDbType.Char, 40).Value = orow["HIST"];
                        cmd.Parameters.Add("@DOC", OleDbType.Char, 13).Value = orow["DOC"];
                        cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13).Value = orow["DOC_FISC"];
                        cmd.Parameters.Add("@VENC", OleDbType.Date, 8).Value = orow["VENC"];
                        cmd.Parameters.Add("@TIPO", OleDbType.Char, 1).Value = orow["TIPO"];
                        cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 10).Value = orow["MOV_ID"];
                        //cmd.Parameters.Add("@NREG", OleDbType.Numeric, 10).Value = orow["NREG"];
                        int nreg = cmd.ExecuteNonQuery();

                        // FiltroFinanDBF.adapterMovFin.Update(new DataRow[] { orow });
                    }
                }
                catch (Exception E)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro " + E.Message); return;

                }

                //movFinView.AcceptChanges();
                //Totaliza();


         
                // dsFiltrado.Tables["RMOV_FIN"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public void Vauches_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            TDataControlContab.PonhaValoresDefault(e.Row);
            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "";
            e.Row["DOC"] = "";
            e.Row["TP_FIN"] = 0;
            e.Row["DEBITO"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            e.Row["OBS"] = "";
            e.Row["FORN"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DEBITO"] = "00";
        }



        public void Vauches_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            try
            {
                if (
                      (orow["CREDITO"].ToString().Trim() == "") && (orow["DEBITO"].ToString().Trim() == "")
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

           
        }
        public void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            bool ok = true;
            string tabela = orow.Table.TableName.ToUpper().Trim().Substring(1);
            try
            {
                OleDbCommand command = new OleDbCommand(
                          "DELETE FROM " + FiltroVauchesDBF.path + "MOVFIN" + " WHERE (RECNO() = " + orow["NREG"].ToString() + ")",
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
            /*if (!orow.IsNull("OUTRO_ID"))
            {
                if (Convert.ToDouble(orow["OUTRO_ID"]) != 0)
                {
                    e.Cancela = true;
                    MessageBox.Show("Para Deletar Use o Pagar&Receber"); return;
                }
            }*/

        }
    }

}

