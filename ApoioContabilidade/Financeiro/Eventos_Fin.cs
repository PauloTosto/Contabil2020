using ApoioContabilidade.Models;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro
{
    public class Evt_Fin
    {
        public BindingSource bmPagar;
        public BindingSource bmReceber;
        public DataSet dsFiltrado;
        public Evt_Fin( DataSet odsFiltrado)
        {
            dsFiltrado = odsFiltrado;
            bmPagar = new BindingSource();
            bmReceber = new BindingSource();
        }

        public async void Rec_Fin_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
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
           
            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                double mov_id = -1;
                if (e.TipoMuda == DataRowState.Added)
                {
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
                        e.Cancela = true;
                        return;
                    }
                }

                // orow.BeginEdit();
                if (e.TipoMuda == DataRowState.Added)
                    orow["MOV_ID"] = mov_id + 1;
                // orow.EndEdit();
            }
            try
            {
                orow["TP_FIN"] = 1;
                orow["TIPO"] = "R";
                orow["VENC"] = orow["DATA"];


                // para evitar valores nullos use defaults 
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_FIN"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro SAIDAS "); return;
                }

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
               
            if  ((!orow.IsNull("outro_id"))   &&  (Convert.ToDouble(orow["outro_id"]) != 0)) // em tese oriundo do pgrc
            {
                if ((orow.RowState == DataRowState.Modified) )
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
                //if (Convert.ToDouble(orow["outro_id"]) != 0) // em tese oriundo do pgrc
                
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

        public async void Pag_Fin_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
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

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                double mov_id = -1;
                if (e.TipoMuda == DataRowState.Added)
                {
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
                        e.Cancela = true;
                        return;
                    }
                }
                if (e.TipoMuda == DataRowState.Added)
                    orow["MOV_ID"] = mov_id + 1;
            }
            try
            {
                orow["TP_FIN"] = 1;
                orow["TIPO"] = "P";
                orow["VENC"] = orow["DATA"];


                // para evitar valores nullos use defaults 
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_FIN"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro  "); return;
                }

               // dsFiltrado.Tables["PMOV_FIN"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public async void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            bool ok = false;
            string tabela = orow.Table.TableName.ToUpper().Trim().Substring(1);
            try
            {
                
                ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, tabela);
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



        public async Task<bool> InsereTransferencia(string tdeb, string tcred, BindingSource bmSourceDe, BindingSource bmSourcePara)
        {
            DataRow orowDe = (bmSourceDe.Current as DataRowView).Row;

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

            }
            return true;
        }
    }
}
