using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.PagarReceber
{
        
    public  class Evt_PgRc
    {
        public  BindingSource bmMestre;
        public  BindingSource bmDetalhe1;
        public  BindingSource bmDetalhe2;
        public  BindingSource bmDetalheCentro;
        public  BindingSource bmEstoque;
        public  BindingSource bmVendas;
        private int tipoConta;
        public FrmPagarReceber form;
        private DataSet dsFiltrado;
        public EdtPgRc comum;
        public Evt_PgRc(int otipoConta, DataSet odsFiltrado)
        {
            dsFiltrado = odsFiltrado;
            tipoConta = otipoConta;
            bmMestre = new BindingSource();
            bmDetalhe1 = new BindingSource();
            bmDetalhe2 = new BindingSource();
            bmDetalheCentro = new BindingSource();
            bmEstoque = new BindingSource();
            bmVendas = new BindingSource();
        }
        public  async void EdtMestre_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (
                 (tipoConta == 1 ? (orow["CREDITO"].ToString().Trim() == "") : (orow["DEBITO"].ToString().Trim() == ""))
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified) || (e.TipoMuda == DataRowState.Detached))
            {
                Mestre_PositionChanged(bmMestre, null);
                double mov_id = Convert.ToDouble(orow["MOV_ID"]);
                if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Detached))
                {
                    double mov_id2 = -1;
                    try
                     {
                        
                         DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                         { "SELECT MAX(MOV_ID) AS MAX_MOV_ID FROM MOV_PGRC" }));
                        mov_id2 = Convert.ToDouble(odataset.Tables[0].Rows[0][0]);
                     }
                     catch (Exception) { }
                     if (mov_id2 != -1) 
                     {
                        mov_id2++;
                        //if (mov_id2 != mov_id)
                        //{
                        orow["MOV_ID"] = mov_id2;
                        Mestre_PositionChanged(bmMestre, null);

                        //}
                    }

                    
                }
                string conta; 
                if (tipoConta == 1) // pagar
                    conta = orow["CREDITO"].ToString();
                else
                    conta = orow["DEBITO"].ToString();

                if (conta.Substring(0,1) == "*")
                {
                    conta = conta.Substring(1);
                }
                orow["EMISSOR"] = TabelasIniciais.AcheEmissor(conta);

                //orow.BeginEdit();
                //if (e.TipoMuda == DataRowState.Added)
                  //  orow["MOV_ID"] = mov_id + 1;
                //orow.EndEdit();
              
            }
            try
            {
                // para evitar valores nullos use defaults 
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_PGRC"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro MOV_PGRC "); return;
                }
                orow.AcceptChanges();
                // dsFiltrado.Tables["MOV_PGRC"].AcceptChanges();
                // int position = (bmDetalhe1.DataSource as DataView).

                string fornecedor = TabelasIniciais.Ache_Descricao(orow["EMISSOR"].ToString());
                foreach (DataRowView orowdet in (bmDetalhe1.DataSource as DataView))
                {
                    orowdet.Row.BeginEdit();
                    if (tipoConta == 1)
                        orowdet.Row["DEBITO"] = orow["CREDITO"];
                    else orowdet.Row["CREDITO"] = orow["DEBITO"];
                    orowdet.Row["DOC_FISC"] = orow["DOC_FISC"];
                    orowdet.Row["DATA_EMI"] = orow["DATA_EMI"];
                    if (orowdet.Row["EMISSOR"].ToString() != orow["EMISSOR"].ToString())
                    {
                        if (fornecedor.Length != 0)
                        {
                            if (fornecedor.Length > 35)
                                orowdet.Row["FORN"] = fornecedor.Substring(0, 35);
                            else orowdet.Row["FORN"] = fornecedor;
                        }
                        else
                            orowdet.Row["FORN"] = "";
                    }
                    orowdet.Row["EMISSOR"] = orow["EMISSOR"];
                    ok = await Prepara_Sql.OpereRegistroServidorAsync(orowdet.Row, DataRowState.Modified, "MOV_FIN");
                    orowdet.Row.EndEdit();
                }
                dsFiltrado.Tables["MOV_FIN"].AcceptChanges();
                if (bmDetalhe1.Count > 0)
                {
                    //bmDetalhe1.SuspendBinding();
                    int position = bmDetalhe1.Position;
                    foreach (DataRowView orowdet in (bmDetalhe1.DataSource as DataView))
                    {
                        orowdet.Row.BeginEdit();
                        if (tipoConta == 1)
                            orowdet.Row["DEBITO"] = orow["CREDITO"];
                        else orowdet.Row["CREDITO"] = orow["DEBITO"];
                        orowdet.Row["DOC_FISC"] = orow["DOC_FISC"];
                        orowdet.Row["DATA_EMI"] = orow["DATA_EMI"];
                        if (orowdet.Row["EMISSOR"].ToString() != orow["EMISSOR"].ToString())
                        {
                            if (fornecedor.Length != 0)
                            {
                                if (fornecedor.Length > 35)
                                    orowdet.Row["FORN"] = fornecedor.Substring(0, 35);
                                else orowdet.Row["FORN"] = fornecedor;
                            }
                            else
                                orowdet.Row["FORN"] = "";
                            
                        }
                        orowdet.Row["EMISSOR"] = orow["EMISSOR"];
                        ok = await Prepara_Sql.OpereRegistroServidorAsync(orowdet.Row, DataRowState.Modified, "MOV_FIN");
                        orowdet.Row.EndEdit();
                    }
                    dsFiltrado.Tables["MOV_FIN"].AcceptChanges();
                    bmDetalhe1.Position = position;
                }
                if (bmDetalhe2.Count > 0)
                {
                   // bmDetalhe2.SuspendBinding();
                    int position = bmDetalhe2.Position;
                    foreach (DataRowView orowdet in (bmDetalhe2.DataSource as DataView))
                    {
                        orowdet.Row.BeginEdit();
                        orowdet.Row["DOC_FISC"] = orow["DOC_FISC"];
                        orowdet.Row["DATA_EMI"] = orow["DATA_EMI"];
                        orowdet.Row["EMISSOR"] = orow["EMISSOR"];
                        ok = await Prepara_Sql.OpereRegistroServidorAsync(orowdet.Row, DataRowState.Modified, "MOV_APRO");
                        orowdet.Row.EndEdit();
                    }
                    dsFiltrado.Tables["MOV_APRO"].AcceptChanges();
                    bmDetalhe2.Position = position;
                }
                if (tipoConta == 2)
                {
                    if (bmVendas.Count > 0)
                    {
                       // bmVendas.SuspendBinding();
                        int position = bmVendas.Position;
                        foreach (DataRowView orowdet in (bmVendas.DataSource as DataView))
                        {
                            orowdet.Row.BeginEdit();
                            orowdet.Row["DOC"] = orow["DOC_FISC"];
                            orowdet.Row["DATA"] = orow["DATA_EMI"];
                            orowdet.Row["FIRMA"] = orow["EMISSOR"];
                            ok = await Prepara_Sql.OpereRegistroServidorAsync(orowdet.Row, DataRowState.Modified, "VENDAS");
                            orowdet.Row.EndEdit();
                        }
                        dsFiltrado.Tables["VENDAS"].AcceptChanges();
                        bmVendas.Position = position;
                    }
                }
            }
            catch (Exception E)
            {
                MessageBox.Show("Operação Falhou: " + E.ToString());
                e.Cancela = true;
                throw;

            }
        }







        public void Mestre_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);
            double? mov_id = 0;
            bool result = (registro != null);
            if (result)
            {
                if (registro.Row.RowState == DataRowState.Detached) return;
                if (registro.Row.RowState == DataRowState.Added)
                {
                    if (!registro.Row.IsNull("MOV_ID"))
                    {
                        mov_id = Convert.ToDouble(registro["MOV_ID"]);
                        // if (mov_id == -1) return;
                    }
                    else return;
                }
            }
            if (result) { result = !registro.Row.IsNull("MOV_ID"); }
            if ((result) && (mov_id != -1))
            {
                mov_id = Convert.ToDouble(registro["MOV_ID"]);
                try
                {   // financeiro

                    var dado = dsFiltrado.Tables["MOV_FIN"].AsEnumerable().Where(row => row.Field<double?>("OUTRO_ID") == mov_id);
                    if ((dado != null))
                    { bmDetalhe1.DataSource = dado.AsDataView(); }
                    else { bmDetalhe1.DataSource = dsFiltrado.Tables["MOV_FIN"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                }
                catch (Exception) { bmDetalhe1.DataSource = dsFiltrado.Tables["MOV_FIN"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                try
                {   // apropriacao
                    var dado = dsFiltrado.Tables["MOV_APRO"].AsEnumerable().Where(row => row.Field<double?>("OUTRO_ID") == mov_id);
                    if ((dado != null))
                    { bmDetalhe2.DataSource = dado.AsDataView(); }
                    else { bmDetalhe2.DataSource = dsFiltrado.Tables["MOV_APRO"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }
                }
                catch (Exception) { bmDetalhe2.DataSource = dsFiltrado.Tables["MOV_APRO"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                try
                {   // Centro de resultado
                    var dado = dsFiltrado.Tables["CTACENTR"].AsEnumerable().Where(row => row.Field<double?>("MOV_ID") == mov_id);
                    if ((dado != null))
                    { bmDetalheCentro.DataSource = dado.AsDataView(); }
                    else { bmDetalheCentro.DataSource = dsFiltrado.Tables["CTACENTR"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }
                }
                catch (Exception) { bmDetalheCentro.DataSource = dsFiltrado.Tables["CTACENTR"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                try
                {   // almoxarifado Estoque
                    var dado = dsFiltrado.Tables["MOVEST"].AsEnumerable().Where(row => row.Field<double?>("MOV_ID") == mov_id);
                    if ((dado != null))
                    { bmEstoque.DataSource = dado.AsDataView(); }
                    else { bmEstoque.DataSource = dsFiltrado.Tables["MOVEST"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }
                }
                catch (Exception) { bmEstoque.DataSource = dsFiltrado.Tables["MOVEST"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }
                try
                {   // Contrato de Vendas de Produtos
                    var dado = dsFiltrado.Tables["VENDAS"].AsEnumerable().Where(row => row.Field<double?>("MOV_ID") == mov_id);
                    if ((dado != null))
                    { bmVendas.DataSource = dado.AsDataView(); }
                    else { bmVendas.DataSource = dsFiltrado.Tables["VENDAS"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }
                }
                catch (Exception) { bmVendas.DataSource = dsFiltrado.Tables["VENDAS"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

            }
            else
            {
                
                bmDetalhe1.DataSource = dsFiltrado.Tables["MOV_FIN"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                bmDetalhe2.DataSource = dsFiltrado.Tables["MOV_APRO"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                bmDetalheCentro.DataSource = dsFiltrado.Tables["CTACENTR"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                bmEstoque.DataSource = dsFiltrado.Tables["MOVEST"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                bmVendas.DataSource = dsFiltrado.Tables["VENDAS"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            }

            comum.oDetalhe1.FuncaoSoma();
            comum.oDetalhe1.ColocaTotais();
            comum.oDetalhe2.FuncaoSoma();
            comum.oDetalhe2.ColocaTotais();
            comum.oDetalheCentro.FuncaoSoma();
            comum.oDetalheCentro.ColocaTotais();
            if (comum.TipoConta == 1)
            {
                comum.oDetalheEst.FuncaoSoma();
                comum.oDetalheEst.ColocaTotais();
            }
            else
            {
                comum.oVenda.FuncaoSoma();
                comum.oVenda.ColocaTotais();
            }
            form.colocaSaldoDetalhe();
            //oItensFis.FuncaoSoma();
            //oItensFis.ColocaTotais();
            //FUNCOES SOMA E COLOCA TOTAIS DE TODOS OS DETALHES
            // CRIAR UMA CLASSE ESTATITICA COM TODOS DO DELPHI EVENTOS_FINAN (DO FINANCEIRO)
        }

        public void EdtMestre_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((tipoConta == 1 ? (orow["CREDITO"].ToString().Trim() == "") : (orow["DEBITO"].ToString().Trim() == ""))
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
                string desc2 = tipoConta == 1 ? orow["CREDITO"].ToString() : orow["DEBITO"].ToString();
                if (desc2.Substring(0, 1) == "*")
                    desc2 = desc2.Substring(1);
                bool retorno = TabelasIniciais.Existe_Desc2(desc2);
                if (!retorno)
                {
                    MessageBox.Show("CONTA " + (tipoConta == 1 ? orow["CREDITO"].ToString() : orow["DEBITO"].ToString()) + " INVALIDO");
                    e.Cancela = true;
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("ERRO AO TENTAR ACESSAR TABELA PLACON PARA CHECAR CONTA");
                e.Cancela = true;
                return;

            }


            try
            {

            // verifica se alteração do valor é menor dos browses filho
            decimal valor = Convert.ToDecimal(orow["VALOR"]);
            if ((orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Detached)) return;
            decimal finan = 0;
            decimal centros = 0;
            decimal estoque = 0;
            decimal aprop = 0;
            decimal vendas = 0;
            if ((comum.oDetalhe1.dictCampoTotal.Count > 0) && comum.oDetalhe1.dictCampoTotal.ContainsKey("VALOR"))
                finan = Convert.ToDecimal(comum.oDetalhe1.dictCampoTotal["VALOR"]);
            if ((comum.oDetalhe2.dictCampoTotal.Count > 0) && comum.oDetalhe2.dictCampoTotal.ContainsKey("VALOR"))
                aprop = Convert.ToDecimal(comum.oDetalhe2.dictCampoTotal["VALOR"]);
            if ((comum.oDetalheCentro.dictCampoTotal.Count > 0) && comum.oDetalheCentro.dictCampoTotal.ContainsKey("VALOR"))
                centros = Convert.ToDecimal(comum.oDetalheCentro.dictCampoTotal["VALOR"]);
            if ((comum.oDetalheEst.dictCampoTotal.Count > 0) && comum.oDetalheEst.dictCampoTotal.ContainsKey("VALOR"))
                estoque = Convert.ToDecimal(comum.oDetalheEst.dictCampoTotal["VALOR"]);
            if ((comum.oVenda.dictCampoTotal.Count > 0) && comum.oVenda.dictCampoTotal.ContainsKey("VALOR"))
                vendas = Convert.ToDecimal(comum.oVenda.dictCampoTotal["VALOR"]);


            if ((valor < vendas) || (valor < (estoque + aprop))
                || (valor < finan) ||  (valor <centros)        )
            {
                MessageBox.Show("Valor Menor que Somatório de Detalhe");
                e.Cancela = true;
                return;
                
            }
            }
            catch (Exception)
            {

                MessageBox.Show("ERRO AO APURAR VALORES DOS DETALHES");
                e.Cancela = true;
                return;
            }

        }

        public void EdtMestre_BeforeDeletaRegistros(object sender, AlteraRegistroEventArgs e)
        {
            // ATENçÂO => ANTES DE DELETAR O MESTRE É NECESSÁRIO VERIFICAR SE EXISTEM REGISTROS LIGADOS A ESTE REGISTRO
            bool ligados = tipoConta == 1 ? (bmEstoque.Count > 0) : (bmVendas.Count > 0);
            if (ligados || (bmDetalhe1.Count > 0) || (bmDetalhe2.Count > 0) || (bmDetalheCentro.Count > 0))
            {
                e.Cancela = true;
                MessageBox.Show("Delete Antes os registros ligados  "); return;
            }

        }
        public  async void EdtMestre_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            // ATENçÂO => ANTES DE DELETAR O MESTRE É NECESSÁRIO VERIFICAR SE EXISTEM REGISTROS LIGADOS A ESTE REGISTRO
          /*  bool ligados = tipoConta == 1 ? (bmEstoque.Count > 0) : (bmVendas.Count > 0);
            if (ligados || (bmDetalhe1.Count > 0) || (bmDetalhe2.Count > 0) || (bmDetalheCentro.Count > 0) ) {
                e.Cancela = true;
                MessageBox.Show("Delete Antes os registros ligados  "); return;
            }
          */
            DataRow orow = e.Rows[0];
            bool ok = false;
            try
            {
                ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, "MOV_PGRC");
            }
            catch (Exception)
            {
            }
            // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Deletar Registro MOV_PGRC "); return;
            }
        }

        public  async void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            bool ok = false;
            try
            {
                ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, orow.Table.TableName.ToUpper().Trim());
            }
            catch (Exception)
            {
            }
            // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Deletar Registro " + orow.Table.TableName.ToUpper().Trim()); return;
            }
        }


        public async void movPgRc_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            
            double mov_id = -1;
            
            
            /* em 23.12.  isolei este passo
             * try
            {
                DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                { "SELECT MAX(MOV_ID) AS MAX_MOV_ID FROM MOV_PGRC" }));
                mov_id = Convert.ToDouble(odataset.Tables[0].Rows[0][0]);
            }
            catch (Exception) { }
            if (mov_id == -1)
            {
                MessageBox.Show("Erro ao buscar numero exclusivo");
                return;
            }
            else { mov_id++; }
            */
            

            e.Row["MOV_ID"] = mov_id;
            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "";
            e.Row["DOC"] = "";
            e.Row["TP_FIN"] = 0;
            e.Row["DEBITO"] = "";
            e.Row["FORN"] = "";

            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            if (tipoConta == 1)
                e.Row["TIPO"] = "P";
            else e.Row["TIPO"] = "R";
           // Mestre_PositionChanged(bmMestre, null);
        }
        public  void movApro_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            DataRow orow = null;
            try
            {
                if (bmMestre.Count > 0)
                {
                    orow = ((DataRowView)bmMestre.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Tem que ter Mestre");
                throw;
            }
            //  if (orow == null) { }
            datainicial = Convert.ToDateTime(orow["data"]);
            // Condição do filtro
            e.Row["OUTRO_ID"] = orow["MOV_ID"];
            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "";
            e.Row["DOC"] = "";
            e.Row["FORN"] = "";
            e.Row["OBS"] = "";
            e.Row["TP_FIN"] = 0;
            e.Row["DEBITO"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            e.Row["VALOR"] = 0;
        }
        public void movFin_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            DataRow orow = null;
            try
            {
                if (bmMestre.Count > 0)
                {
                    orow = ((DataRowView)bmMestre.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Tem que ter Mestre");
                throw;
            }
            if (orow != null)
            {
                Convert.ToDateTime(orow["data"]);
            }
            // Condição do filtro
            e.Row["OUTRO_ID"] = orow["MOV_ID"];
            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "";
            e.Row["DOC"] = "";
            e.Row["TP_FIN"] = 1;
            e.Row["DEBITO"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            e.Row["VALOR"] = 0;
            if (tipoConta == 1)
            {
                string forn = TabelasIniciais.Ache_Descricao(orow["EMISSOR"].ToString()).Trim();
                if (forn.Length > e.Row.Table.Columns["FORN"].MaxLength)
                    forn = forn.Substring(0, e.Row.Table.Columns["FORN"].MaxLength);
                e.Row["FORN"] = forn;
                e.Row["CREDITO"] = "00";
            }
            else
            { e.Row["DEBITO"] = "00"; }
        }



        /// <summary>
        /// / MOV_FIN 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// 



        public async void EdtDetalhe1_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (
                 (tipoConta == 1 ? (orow["CREDITO"].ToString().Trim() == "") : (orow["DEBITO"].ToString().Trim() == ""))
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
                return;
            }
            double outroId = Convert.ToDouble(rowmestre["MOV_ID"]);

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
                orow["OUTRO_ID"] = outroId;
                if (e.TipoMuda == DataRowState.Added)
                    orow["MOV_ID"] = mov_id + 1;
                // orow.EndEdit();
            }
            try
            {
                orow["TP_FIN"] = 1;
                if (tipoConta == 1)
                {
                    orow["TIPO"] = "P";
                    if (orow["HIST"].ToString() == "")
                        orow["HIST"] = "PAGAMENTO";
                    orow["DEBITO"] = rowmestre["CREDITO"];
                }
                else
                {
                    orow["TIPO"] = "R";
                    if (orow["HIST"].ToString() == "")
                        orow["HIST"] = "RECEBIMENTO";
                    orow["CREDITO"] = rowmestre["DEBITO"];
                }
                orow["DOC_FISC"] = rowmestre["DOC_FISC"];
                orow["DATA_EMI"] = rowmestre["DATA_EMI"];
                orow["VENC"] = orow["DATA"];


                // para evitar valores nullos use defaults 
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                
                bool ok                 
                    = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_FIN"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro MOV_FIN "); return;
                }
              //  orow.AcceptChanges();
               // bmDetalhe1.AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public  void EdtDetalhe1_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((tipoConta == 1 ? (orow["CREDITO"].ToString().Trim() == "") : (orow["DEBITO"].ToString().Trim() == ""))
                    || (Convert.ToDouble(orow["VALOR"]) <= 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            try
            {
                string desc2 = tipoConta == 1 ? orow["CREDITO"].ToString() : orow["DEBITO"].ToString();
                if (desc2.Substring(0, 1) == "*")
                    desc2 = desc2.Substring(1);
                bool retorno = TabelasIniciais.Existe_Banco(desc2);
                if (!retorno)
                {
                    MessageBox.Show("CAMPO " + (tipoConta == 1 ? orow["CREDITO"].ToString() : orow["DEBITO"].ToString()) + " INVALIDO");
                    e.Cancela = true;
                    return;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("ERRO AO TENTAR ACESSAR TABELA PLACON PARA CHECAR CONTA");
                e.Cancela = true;
                return;

            }






            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
            }

            double valorMestre = Convert.ToDouble(rowmestre["VALOR"]);
            double soma =
              (bmDetalhe1.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR"));
            
            double valor = 0;
            //if (Convert.ToDouble(orow["VALOR"]);
           
            if ((valor + soma) > valorMestre)
            {
                MessageBox.Show("Soma dos Valores Maior que do Mestre");
                e.Cancela = true;
                return;
            }

        }
        public  async void EdtDetalhe2_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (
                 (tipoConta == 1 ? (orow["DEBITO"].ToString().Trim() == "") : (orow["CREDITO"].ToString().Trim() == ""))
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
                return;
            }
            double outroId = Convert.ToDouble(rowmestre["MOV_ID"]);

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                double mov_id = -1;
                if (e.TipoMuda == DataRowState.Added)
                {
                    try
                    {
                        DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                        { "SELECT MAX(MOV_ID) AS MAX_MOV_ID FROM MOV_APRO" }));
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
                orow["OUTRO_ID"] = outroId;
                if (e.TipoMuda == DataRowState.Added)
                    orow["MOV_ID"] = mov_id + 1;
                // orow.EndEdit();
            }
            try
            {
                orow["TP_FIN"] = 0;
                orow["DOC_FISC"] = rowmestre["DOC_FISC"];
                orow["DATA_EMI"] = rowmestre["DATA_EMI"];
                orow["EMISSOR"] = rowmestre["EMISSOR"];
                orow["VENC"] = orow["DATA"];


                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOV_APRO"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro MOV_APRO "); return;
                }
                orow.AcceptChanges();
                // dsFiltrado.Tables["MOV_APRO"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public  void EdtDetalhe2_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((tipoConta == 1 ? (orow["DEBITO"].ToString().Trim() == "") : (orow["CREDITO"].ToString().Trim() == ""))
                    || (Convert.ToDouble(orow["VALOR"]) <= 0)
                    || (orow["DEBITO"].ToString().Trim() == orow["CREDITO"].ToString().Trim())
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
            }

            double valorMestre = Convert.ToDouble(rowmestre["VALOR"]);
            double soma =
              (bmDetalhe2.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR"));
            double valor = 0;
            //if (Convert.ToDouble(orow["VALOR"]);

            if ((valor + soma) > valorMestre)
            {
                MessageBox.Show("Soma dos Valores Maior que do Mestre");
                e.Cancela = true;
                return;
            }

        }

        public  async void ctaCentro_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            
            DataRow orow = e.Rows[0];
            if ((orow["NUM_MOD"].ToString() == "")
                   || (orow["CODSER"].ToString() == "")
                     || (orow["SETOR"].ToString() == "")
                      || (orow["GLEBA"].ToString() == "")
                    || (Convert.ToDouble(orow["VALOR"]) <= 0)
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
                return;
            }
            double outroId = Convert.ToDouble(rowmestre["MOV_ID"]);

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                orow["MOV_ID"] = outroId;
            }
            try
            {
                // CONSISTENCIA COM OS FORMATOS DOS DADOS NO SERVIDOR
                orow["NUM_MOD"] = orow["NUM_MOD"].ToString().Trim().PadRight(2);
                orow["CODSER"] = orow["CODSER"].ToString().Trim().PadRight(3);
                orow["SETOR"] = orow["SETOR"].ToString().Trim().PadRight(2);
                orow["GLEBA"] = orow["GLEBA"].ToString().Trim().PadRight(4);
                orow["QUADRA"] = orow["QUADRA"].ToString().Trim().PadRight(3);
            



                if (orow["DOC"].ToString().Trim() == "")
                {
                    string doc_fisc = rowmestre["DOC_FISC"].ToString().Trim();
                    if (doc_fisc.Length > orow.Table.Columns["DOC"].MaxLength)
                        doc_fisc = doc_fisc.Substring(0, orow.Table.Columns["DOC"].MaxLength);
                    orow["DOC"] = doc_fisc;
                }
                if (orow.IsNull("DATA")) { orow["DATA"] = rowmestre["DATA"];  }
                if (tipoConta == 2) { orow["DBCR"] = "C"; }
                else { orow["DBCR"] = "D"; }

                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "CTACENTR");
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro CTACENTR "); return;
                }
                orow.AcceptChanges();
                //   dsFiltrado.Tables["CTACENTR"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
            return;
        }
        public  void ctaCentro_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((orow["NUM_MOD"].ToString() == "")
                   || (orow["CODSER"].ToString() == "")
                     || (orow["SETOR"].ToString() == "")
                      || (orow["GLEBA"].ToString() == "")
                    || (Convert.ToDouble(orow["VALOR"]) <= 0)
                    || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Campo(s) Obrigatorio(s) Vazio(s) ");
                e.Cancela = true;
                return;
            }
            if (!DadosComum.campoGlebaSource(orow["SETOR"].ToString(), orow["GLEBA"].ToString()))
            {
                MessageBox.Show("GLEBA NÃO PERTENCE AO SETOR");
                e.Cancela = true;
                return;
            }

            if ((orow["QUADRA"].ToString().Trim() != "") && (!DadosComum.campoQuadraSource(orow["GLEBA"].ToString(), orow["QUADRA"].ToString())))
            {
                MessageBox.Show("QUADRA NÃO PERTENCE A GLEBA");
                e.Cancela = true;
                return;
            }
            if (!DadosComum.campoServicoSource(orow["NUM_MOD"].ToString(), orow["CODSER"].ToString()))
            {
                MessageBox.Show("SERVIÇO NÃO INCLUSO NESTE MODELO");
                e.Cancela = true;
                return;
            }

            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
            }

            double valorMestre = Convert.ToDouble(rowmestre["VALOR"]);
            double soma =
              Math.Round((bmDetalheCentro.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR")), 2);
            double soma2 = 0;
            if (bmEstoque.Count > 0)
            {
               soma2 = Math.Round((bmEstoque.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR")), 2);

            }


            if ((soma + soma2) > valorMestre)
            {
                MessageBox.Show("Soma dos Centro e Estoque Maior que do Mestre");
                e.Cancela = true;
                return;
            }

        }

        public  void ctaCentro_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            DataRow orow = null;
            try
            {
                if (bmMestre.Count > 0)
                {
                    orow = ((DataRowView)bmMestre.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Tem que ter Mestre");
                throw;
            }
            //  if (orow == null) { }
            datainicial = Convert.ToDateTime(orow["data"]);
            // Condição do filtro
            e.Row["MOV_ID"] = orow["MOV_ID"];
            e.Row["DATA"] = datainicial;
            e.Row["VALOR"] = 0;
            e.Row["NUM_MOD"] = "";
            e.Row["CODSER"] = "";
            e.Row["SETOR"] = "";
            e.Row["GLEBA"] = "";
            e.Row["QUADRA"] = "";
            e.Row["ICODSER"] = 0;
            e.Row["HISTORICO"] = "";
            e.Row["CODMAT"] = "";
        }


        public  async void deposito_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((orow["SETOR"].ToString() == "")
                 || (orow["COD"].ToString() == "")
                  || (orow["DEPOSITO"].ToString() == "")
                  || (Convert.ToDouble(orow["QUANT"]) <= 0)
                || (Convert.ToDouble(orow["VALOR"]) <= 0)
                || (orow.IsNull("DATAC")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
                return;
            }
            double outroId = Convert.ToDouble(rowmestre["MOV_ID"]);

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                orow["MOV_ID"] = outroId;
            }
            try
            {
                if (orow["DOC"].ToString().Trim() == "")
                    orow["DOC"] = rowmestre["DOC_FISC"];
                orow["FORN"] = rowmestre["EMISSOR"];
                orow["SETOR"] = orow["SETOR"].ToString().Trim().PadRight(2);

                if (!orow.IsNull("DATAC")) { orow["DATA"] = orow["DATAC"]; }
                else { orow["DATA"] = rowmestre["DATA"]; }
                orow["TIPO"] = "E";
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }
                }
                try
                {
                    double punit = Math.Round(Convert.ToDouble(orow["VALOR"]) / Convert.ToDouble(orow["QUANT"]), 2);
                    orow["PUNIT"] = punit;
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao Calcula Preço Unitário");
                    e.Cancela = true;
                    return;
                }

                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOVEST");
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir Registro MOVEST"); return;
                }
                orow.AcceptChanges();
                // dsFiltrado.Tables["MOVEST"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;

            }
        }
        public  void deposito_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((orow["SETOR"].ToString() == "")
                 || (orow["COD"].ToString() == "")
                  || (orow["DEPOSITO"].ToString() == "")
                  || (Convert.ToDouble(orow["QUANT"]) <= 0)
                || (Convert.ToDouble(orow["VALOR"]) <= 0)
                || (orow.IsNull("DATAC")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
            }

            double valorMestre = Convert.ToDouble(rowmestre["VALOR"]);
            double soma =
              Math.Round((bmEstoque.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR")), 2);
            double soma2 = 0;
            if (bmDetalheCentro.Count > 0)
            {
                soma2 = Math.Round((bmDetalheCentro.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR")), 2);

            }

            if ((soma + soma2 ) > valorMestre)
            {
                MessageBox.Show("Soma dos Centro e Estoque Maior que do Mestre");
                e.Cancela = true;
                return;
            }

        }
        public  void deposito_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            DataRow orow = null;
            try
            {
                if (bmMestre.Count > 0)
                {
                    orow = ((DataRowView)bmMestre.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Tem que ter Mestre");
                throw;
            }
            //  if (orow == null) { }
            datainicial = Convert.ToDateTime(orow["data"]);
            // Condição do filtro
            e.Row["MOV_ID"] = orow["MOV_ID"];
            e.Row["DATAC"] = datainicial;
            e.Row["VALOR"] = 0;
            e.Row["QUANT"] = 0;
            e.Row["PUNIT"] = 0;
            e.Row["NUM_MOD"] = "";
            e.Row["CODSER"] = "";
            e.Row["SETOR"] = "";
            e.Row["COD"] = "";
            e.Row["DEPOSITO"] = "";
            e.Row["DOC"] = "";
        }

        /// <summary>
        /// // ORDEM DE VENDA
        /// </summary>
        public  async void venda_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((orow["SAFRA"].ToString() == "")
                 || (orow["FIRMA"].ToString() == "")
                  || (orow["PROD"].ToString() == "")
                  || (Convert.ToDouble(orow["QUANT"]) <= 0)
                || (Convert.ToDouble(orow["VALOR"]) <= 0)
                || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
                return;
            }
            double outroId = Convert.ToDouble(rowmestre["MOV_ID"]);

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                orow["MOV_ID"] = outroId;
            }
            try
            {
                foreach (DataColumn col in orow.Table.Columns)
                {
                    if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                try
                {

                    double punit = Math.Round(Convert.ToDouble(orow["VALOR"]) / Convert.ToDouble(orow["QUANT"]), 4);
                    orow["PRECO"] = punit;
                    string ultimocontrato = "SELECT MAX(CONT) as max_cont FROM VENDAS WHERE (SAFRA = '" + orow["SAFRA"].ToString() +
                           "') AND (PROD = '" + orow["PROD"].ToString() + "');";

                    DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                     { "SELECT MAX(OUTRO_ID) AS MAXOUTRO,MAX(VENDAS_ID) AS MAXVENDAS  FROM VENDAS;",
                          ultimocontrato }));
                    double outro_id = Convert.ToDouble(odataset.Tables[0].Rows[0][0]);
                    double vendas_id = Convert.ToDouble(odataset.Tables[0].Rows[0][1]);
                    orow["OUTRO_ID"] = outro_id;
                    orow["VENDAS_ID"] = vendas_id;
                    if (orow["CONT"].ToString().Trim() == "")
                    {
                        string cont = "  1";
                        if (odataset.Tables.Count > 1)
                        {
                            cont = odataset.Tables[1].Rows[0][0].ToString();
                            if (!string.IsNullOrEmpty(cont))
                            {
                                cont = (Convert.ToInt32(cont)+1).ToString().PadLeft(3);
                            }
                            else
                                cont = "  1";
                        }
                        orow["CONT"] = cont;
                    }
                    orow["DOC"] = rowmestre["DOC_FISC"];
                    orow["DATA"] = rowmestre["DATA"];

                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao Preparar Edição/Inserção Registro Vendas");
                    e.Cancela = true;
                    return;
                }

                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "VENDAS");
                if (!ok)
                {
                    e.Cancela = true;
                    MessageBox.Show("Erro ao Inserir/Editar Registro VENDAS"); return;
                }
                else 
                   orow.AcceptChanges();
                // dsFiltrado.Tables["VENDAS"].AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;
            }
        }
        public  void venda_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((orow["SAFRA"].ToString() == "")
                 || (orow["PROD"].ToString() == "")
                  || (orow["FIRMA"].ToString() == "")
                  || (Convert.ToDouble(orow["QUANT"]) <= 0)
                || (Convert.ToDouble(orow["VALOR"]) <= 0)
                || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                e.Cancela = true;
            }

            double valorMestre = Convert.ToDouble(rowmestre["VALOR"]);
            double soma =
              Math.Round((bmVendas.DataSource as DataView).ToTable().AsEnumerable().Sum(row => row.Field<double>("VALOR")), 2);

            if (soma > valorMestre)
            {
                MessageBox.Show("Soma dos Valores Maior que do Mestre");
                e.Cancela = true;
                return;
            }

        }
        public  void venda_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            DataRow orow = null;
            try
            {
                if (bmMestre.Count > 0)
                {
                    orow = ((DataRowView)bmMestre.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Tem que ter Mestre");
                throw;
            }
            //  if (orow == null) { }
            datainicial = Convert.ToDateTime(orow["data"]);
            // Condição do filtro
            e.Row["MOV_ID"] = orow["MOV_ID"];
            e.Row["DATA"] = datainicial;
            e.Row["VALOR"] = 0;
            e.Row["QUANT"] = 0;
           // e.Row["PUNIT"] = 0;
            e.Row["SAFRA"] = datainicial.Year.ToString(); 
            e.Row["PROD"] = "";
            e.Row["FIRMA"] = orow["EMISSOR"];
            e.Row["CONT"] = "";
            e.Row["PROD_TP"] = 0;
            e.Row["CERTIF"] = 0;
            e.Row["COMPLEM"] = 0;
            e.Row["DOC"] = orow["DOC_FISC"];
        }

        public bool EstaVinculado(DataRow orow)
        {
            bool result = true;
            if (Convert.ToDouble(orow["OUTRO_ID"]) == 0) result = false; 
            return result;
        }
        async public Task<bool> DesfazVinculoPGRC(DataRow orow)
        {
            bool result = true;
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                result = false;
                return result;
            }
            if ( Convert.ToDouble(orow["OUTRO_ID"]) == Convert.ToDouble(orow["MOV_ID"]))
            {
                orow.BeginEdit();
                orow["OUTRO_ID"] = 0;
                orow.EndEdit();
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, DataRowState.Modified, orow.Table.TableName);
                if (!ok)
                {
                    result = false;
                    MessageBox.Show("Erro ao Inserir/Editar Registro VENDAS"); 
                }
            }
            return result;
        }
        async public Task<bool> DesfazVinculoCentros(DataRow orow)
        {
            bool result = true;
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao MESTRE");
                result = false;
                return result;
            }
            if (Convert.ToDouble(orow["MOV_ID"]) == Convert.ToDouble(orow["MOV_ID"]))
            {
                orow.BeginEdit();
                orow["MOV_ID"] = 0;
                orow.EndEdit();
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, DataRowState.Modified, orow.Table.TableName);
                if (!ok)
                {
                    result = false;
                    MessageBox.Show("Erro ao Inserir/Editar Registro VENDAS");
                }
            }
            return result;
        }
        public bool PodeApropriar()
        {
            bool result = false;
            if ((bmMestre.Current == null) || (bmDetalheCentro.Current == null)) return false;
            if (bmDetalheCentro.Count == 0) return result;
            DataRowView rowcentro = (bmDetalheCentro.Current as DataRowView);
            if (rowcentro.Row.IsNull("SETOR") || rowcentro.Row.IsNull("NUM_MOD") || rowcentro.Row.IsNull("ICODSER")) return result;
            double valor = Convert.ToDouble(rowcentro["VALOR"]);
            if (valor == 0) return result;
            DataRowView rowmst = (bmMestre.Current as DataRowView);
            if ((rowmst["DOC_FISC"].ToString() == "SIST_RURAL") || (rowmst["DOC_FISC"].ToString() == "SIST_RESC")) return result;
            
            double total = Convert.ToDouble(rowmst["VALOR"]);
            double totalCentro = (bmDetalheCentro.DataSource as DataView).Table.AsEnumerable().Sum(row => Math.Round(row.Field<double>("VALOR"),2));
            if (totalCentro == 0) return result;
            double totalEstoque = 0;
            if ((tipoConta == 1) && (bmEstoque.Count > 0))
                totalEstoque = (bmEstoque.DataSource as DataView).Table.AsEnumerable().Sum(row => Math.Round(row.Field<double>("VALOR"),2));
            double totalApropria = 0;
            if (bmDetalhe2.Count > 0)
                totalApropria = (bmDetalhe2.DataSource as DataView).Table.AsEnumerable().Sum(row => Math.Round(row.Field<double>("VALOR"),2));
            double totcompare = totalCentro + totalEstoque;
            if (total < totcompare) return result;
            if ((total <= totcompare) && (totcompare < (totalApropria + valor))) return result;
            result = true;
            return result;
        }

    }
}
