using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
//using PrjApiParceiro_C.AcessosServidor;
//using System.Collections.Generic;
//using PrjApiParceiro_C.Config_Componentes;
using System.Reflection;
using System.ComponentModel;
//using System.Linq;
//using PrjApiParceiro_C.Filtros;
using ClassFiltroEdite;
using ApoioContabilidade.Models;

namespace ApoioContabilidade.Fiscal.Servicos
{
    public  class ManejoDados
    {
        public DataSet odataset;
        public BindingSource mestreBSource;
        public BindingSource itensFiscalBSource;

        // NFE_DISPONIVEIS
        public BindingSource loteDispBSource;
        public BindingSource nfeLoteBSource;

        public BindingSource vendaDispBSource;
        public BindingSource nfeVendaBSource;

        private MonteGrid oMestre;
        private MonteGrid oItensFis;

        private MonteGrid oVendaDisp;
        private MonteGrid oNfeVenda;

        private MonteGrid oLoteDisp;
        private MonteGrid oNfeLote;

       

        public ManejoDados()
        {
            mestreBSource = new BindingSource();
            itensFiscalBSource = new BindingSource();

            // NFE_DISPONIVEIS
            loteDispBSource = new BindingSource();
            nfeLoteBSource = new BindingSource();
            vendaDispBSource = new BindingSource();
            nfeVendaBSource = new BindingSource();

            
        }

        public async Task<bool> Inicialize_ManejoDados(DateTime data1, DateTime data2, MonteGrid Mestre,
            MonteGrid ItensFis,  MonteGrid VendaDisp,   MonteGrid NfeVenda,   MonteGrid LoteDisp,    
            MonteGrid NfeLote)
        {
            oMestre = Mestre;
            oItensFis = ItensFis;
            oVendaDisp = VendaDisp;
            oNfeVenda = NfeVenda;
            oLoteDisp = LoteDisp;
            oNfeLote = NfeLote;

            Boolean ok = await ConsulteServidor(data1, data2);
        
            if (ok == false) { MessageBox.Show("Problema na Carga Fiscal "); return ok; }
           // ok = await ClassEventosNF.RecarregueDisponivel();
           
            /*odataset = ClassEventosNF.odataset;
            DataSet odata_disp = ClassEventosNF.odata_disp;
            odataset.Tables.Add(odata_disp.Tables["Lote_disp"].Copy());
            odataset.Tables.Add(odata_disp.Tables["Venda_disp"].Copy());
            odata_disp.Tables.Clear();
            odata_disp = null;*/

            mestreBSource.DataSource = odataset.Tables["Mestre"];
            itensFiscalBSource.DataSource = odataset.Tables["ItensFis"].Clone();
            // lote Disponiveis
            loteDispBSource.DataSource = odataset.Tables["Lote_Disp"].Clone();
            nfeLoteBSource.DataSource = odataset.Tables["NfeLote"].Clone();
            // vendas disponivel
            vendaDispBSource.DataSource = odataset.Tables["Venda_Disp"].Clone(); ;
            nfeVendaBSource.DataSource = odataset.Tables["NfeVenda"].Clone();
           

            return ok;
        }
        public async Task<bool> ConsulteServidor(DateTime data1, DateTime data2)
        {
            Boolean ok = await ClassEventosNF.CargaNFeXML(data1, data2);
            if (ok == false) { MessageBox.Show("Problema na Carga Fiscal "); return ok; }
            ok = await ClassEventosNF.RecarregueDisponivel();
           // ok = await ClassEventosNF.Recarregue_FazendaDisponivel();

            odataset = ClassEventosNF.odataset.Copy();
            
            DataSet odata_disp = ClassEventosNF.odata_disp;
            odataset.Tables.Add(odata_disp.Tables["Lote_disp"].Copy());
            odataset.Tables.Add(odata_disp.Tables["Venda_disp"].Copy());
            odata_disp.Tables.Clear();
            //odata_disp = null;
           // SelecaoIndex(selecaoIndex, fazenda_contabil, Cfop, indexCompleto);
            return ok;
        }
        
        public void Mestre_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);

            if (registro != null)
            {
                int nfiscal = Convert.ToInt32(registro["NFISCAL"]);
                try
                {
                    var dado = odataset.Tables["ItensFis"].AsEnumerable().Where(row => row.Field<int>("ID_NF") == nfiscal);
                    if ((dado != null) && (dado.Count() > 0))
                    { itensFiscalBSource.DataSource = dado.CopyToDataTable(); }
                    else { itensFiscalBSource.DataSource = odataset.Tables["ItensFis"].Clone(); }
                }
                catch (Exception) { itensFiscalBSource.DataSource = odataset.Tables["ItensFis"].Clone(); }
            }
            else { itensFiscalBSource.DataSource = odataset.Tables["ItensFis"].Clone(); }
            //oItensFis.FuncaoSoma();
            //oItensFis.ColocaTotais();
        }
        // PARA AS NOTAS FISCAIS DE ENTRADA(apanha de cacau nas fazendas) == sub-detalhe ItensFiscais e Lotes Disponiveis para ligação entre eles 
        public void Itens_PositionChanged_NfeDisp(object sender, EventArgs e)
        {
            DataRowView itensFis = ((sender as BindingSource).Current as DataRowView);
            DataRowView mestre = mestreBSource.Current as DataRowView;
            double quant = 0;
            string tsetor = "";
            DateTime tdata;
            // informações do mestre
            if ((mestre == null) || (itensFis == null))
            {
                loteDispBSource.DataSource = odataset.Tables["Lote_Disp"].Clone();
                nfeLoteBSource.DataSource = odataset.Tables["NfeLote"].Clone();
                oNfeLote.FuncaoSoma();
                oLoteDisp.FuncaoSoma();
                oLoteDisp.ssColocaTotais();
                oNfeLote.ssColocaTotais();
                return;
            }

            tdata = Convert.ToDateTime(mestre["dtaEMi"]);
            tsetor = mestre["CODFAZ"].ToString();

            // informações dos lançamentos dos itens fircais
            if (itensFis != null) { quant = Convert.ToDouble(itensFis["cQuant"]); }
            // o setor 1 será apresentado dentro do setor 2  
            if (tsetor == " 1") { tsetor = " 2"; }
            // Verifica lotes disponíveis para este setor
            try
            {
                var dado = odataset.Tables["Lote_Disp"].AsEnumerable().Where(row =>
                 ((row.Field<double>("SDOKG_ENT") != 0) && (row.Field<string>("SETOR") == tsetor))).OrderBy(row => row.Field<DateTime>("DTA_ENT"));
                // se a pesquisa não apresentar resultado a imagem da tabela (Clone()) representará os dados de lotes disponíveis deste setor
                if ((dado != null) && (dado.Count() > 0)) { loteDispBSource.DataSource = dado.CopyToDataTable(); }
                else { loteDispBSource.DataSource = odataset.Tables["Lote_Disp"].Clone(); }
            }
            catch (Exception)
            {
                loteDispBSource.DataSource = odataset.Tables["Lote_Disp"].Clone();
            }
            // notas fiscais de entradas já ligadas a algum lote de cacau produzido
            try
            {
                double nfiscal = Convert.ToDouble(mestre["NFISCAL"]);
                var dado = odataset.Tables["NfeLote"].AsEnumerable()
                  .Where(row => row.Field<Double>("ITENSNF_ID") == nfiscal);
                // oNfeLote.FuncaoSoma();
                // oNfeLote.ColocaTotais();
                if ((dado != null) && (dado.Count() > 0)) { nfeLoteBSource.DataSource = dado.CopyToDataTable(); } // representação em tabela das linhas selecionadas
                else { nfeLoteBSource.DataSource = odataset.Tables["NfeLote"].Clone(); }

                oNfeLote.FuncaoSoma();
                // pega o total das QUANT_NF desta seleção para comparar e informa necessidade de ação do usuário 
                double totkg = oNfeLote.LinhasCampo[oNfeLote.LinhasCampo.Count - 1].total;
                quant = Convert.ToDouble(mestre["QUANT_NF"]);
                string result = "";
                if (totkg == 0)
                {   // nenhum lote ligado a este iten da nota fiscas
                    result = "Nenhuma";
                }
                else if (totkg != quant) { result = "Incompleto"; }
                else if (totkg == quant) { result = "Completo"; }
                // altera o Mestre
                if (mestre["Critica"].ToString() != result)
                {
                    mestre.BeginEdit();
                    mestre["Critica"] = result;
                    mestre.EndEdit();
                    mestre.DataView.Table.AcceptChanges();
                    // altera também no odataSet (fonte original dos dados de dados
                    DataRow mestreorg = odataset.Tables["Mestre"].AsEnumerable().Where(row => row.Field<int>("ID") == Convert.ToInt32(mestre["ID"])).FirstOrDefault();
                    if (mestreorg != null) { mestreorg["CRITICA"] = mestre["CRITICA"]; odataset.Tables["Mestre"].AcceptChanges(); }
                }

            }
            catch (Exception E)
            {
                nfeLoteBSource.DataSource = odataset.Tables["NfeLote"].Clone();
                oNfeLote.FuncaoSoma();
            }
            oLoteDisp.FuncaoSoma();
            oLoteDisp.ssColocaTotais();
            oNfeLote.ssColocaTotais();
        }
        // PARA AS NOTAS FISCAIS DE SAIDA(Vendas de Cacau) == sub-detalhe Liga os contratos de vendas com as notas fiscais de saida
        // Os contratos de vendas de Cacau devem estar ligados a notas fiscais de saidas correspondente! aqui é o lugar desta relação 
        public void Itens_PositionChanged_VendaDisp(object sender, EventArgs e)
        {
            DataRowView mestre = mestreBSource.Current as DataRowView;
            string tfirma = "";
            // double quant = 0;
            DateTime tdata;
            if (mestre == null)
            {
                vendaDispBSource.DataSource = odataset.Tables["Venda_Disp"].Clone();
                nfeVendaBSource.DataSource = odataset.Tables["NfeVenda"].Clone();
                oVendaDisp.FuncaoSoma();
                oVendaDisp.ssColocaTotais();
                oNfeVenda.FuncaoSoma();
                oNfeVenda.ssColocaTotais();
                return;
            }
            // pesquisa pela data de emissão da nota e pela firma as vendas que ainda não foram ligadas a uma nota fiscal de saida
            tdata = Convert.ToDateTime(mestre["DTAEMi"]);
            tfirma = mestre["FIRMA"].ToString();
            try
            {
                var dado = odataset.Tables["Venda_Disp"].AsEnumerable().Where(row =>
                ((row.Field<double>("SDOQUANT") != 0) && ((tfirma == "") ? true : (row.Field<string>("FIRMA") == tfirma)))).OrderBy(row => row.Field<DateTime>("DATA"));
                if ((dado != null) && (dado.Count() > 0)) { vendaDispBSource.DataSource = dado.CopyToDataTable(); }
                else { vendaDispBSource.DataSource = odataset.Tables["Venda_Disp"].Clone(); }
            }
            catch (Exception E)
            {
                vendaDispBSource.DataSource = odataset.Tables["Venda_Disp"].Clone();
            }

            try
            {
                double nfiscal = Convert.ToDouble(mestre["NFISCAL"]);
                // pesquisa para uma determinada nota fiscal de saida Os itens que já estão ligados 
                var dado = odataset.Tables["NfeVenda"].AsEnumerable()
                  .Where(row => row.Field<Double>("ID_ITENS") == nfiscal);
                if ((dado != null) && (dado.Count() > 0)) { nfeVendaBSource.DataSource = dado.CopyToDataTable(); }
                else { nfeVendaBSource.DataSource = odataset.Tables["NfeVenda"].Clone(); }
            }
            catch (Exception E)
            {
                nfeVendaBSource.DataSource = odataset.Tables["NfeVenda"].Clone();
            }

            oVendaDisp.FuncaoSoma();
            oVendaDisp.ssColocaTotais();
            oNfeVenda.FuncaoSoma();
            oNfeVenda.ssColocaTotais();
        }

        /// <summary>
        ///  Alternancias entre Geral / entradas /saidas
        /// </summary>
        public void SelecaoIndex(int index, string fazenda_contabil, string Cfop, int indexCompleto)
        {
            try
            {
                switch (index)
                {
                    case 0: // GERAL
                        itensFiscalBSource.CurrentItemChanged -= Itens_PositionChanged_VendaDisp;
                        itensFiscalBSource.CurrentItemChanged -= Itens_PositionChanged_NfeDisp;
                        mestreBSource.CurrentItemChanged -= Mestre_PositionChanged;
                        mestreBSource.DataSource = odataset.Tables["Mestre"];
                        mestreBSource.CurrentItemChanged += Mestre_PositionChanged;
                        if ((mestreBSource.DataSource as DataTable).Rows.Count == 0)
                        {
                            Mestre_PositionChanged(mestreBSource, null);
                        }
                        mestreBSource.ResetCurrentItem();
                        oMestre.FuncaoSoma();
                        oMestre.ssColocaTotais();
                        break;
                    case 1:
                        // o mestre estará filtrado
                        mestreBSource.CurrentItemChanged -= Mestre_PositionChanged;
                        itensFiscalBSource.CurrentItemChanged -= Itens_PositionChanged_VendaDisp;
                        itensFiscalBSource.CurrentItemChanged -= Itens_PositionChanged_NfeDisp;
                        try
                        {
                            var dado = odataset.Tables["Mestre"].AsEnumerable().Where(row =>
                                       (((fazenda_contabil.Trim() == "") ? (row.Field<string>("CODFAZ") != "") : (row.Field<string>("CODFAZ").Trim() == fazenda_contabil.Trim())) &&
                                       ((Cfop.Trim() == "") ? (row.Field<string>("CFOP") != "") : (row.Field<string>("CFOP").Trim() == Cfop.Trim())) &&
                                                  (row.Field<string>("TPNF") == "E") &&
                                            (row.Field<string>("PRODFIS") == ClassEventosNF.CACAU_)));
                            /* filtro da critica */
                            if ((indexCompleto == 1) && (dado != null) && (dado.Count() > 0))
                            {
                                dado = dado.Where(row => row.Field<string>("CRITICA").Trim().ToUpper() == "COMPLETO");
                            }
                            else if ((indexCompleto == 2) && (dado != null) && (dado.Count() > 0))
                            {
                                dado = dado.Where(row => row.Field<string>("CRITICA").Trim().ToUpper() == "INCOMPLETO");
                            }
                            else if ((indexCompleto == 3) && (dado != null) && (dado.Count() > 0))
                            {
                                dado = dado.Where(row => row.Field<string>("CRITICA").Trim().ToUpper() == "NENHUMA");
                            }

                            if ((dado != null) && (dado.Count() > 0))
                            {
                                mestreBSource.DataSource = dado.CopyToDataTable();
                            }
                            else { mestreBSource.DataSource = odataset.Tables["Mestre"].Clone(); }
                        }
                        catch (Exception E)
                        {
                            mestreBSource.DataSource = mestreBSource.DataSource = odataset.Tables["Mestre"].Clone();
                        }
                        // mestreBSource.ResetBindings(true);
                        itensFiscalBSource.CurrentItemChanged += Itens_PositionChanged_NfeDisp;
                        mestreBSource.CurrentItemChanged += Mestre_PositionChanged;
                        if ((mestreBSource.DataSource as DataTable).Rows.Count == 0)
                        {
                            // EventArgs arg = new EventArgs();
                            Mestre_PositionChanged(mestreBSource, null);
                        }
                        mestreBSource.ResetCurrentItem();

                        oMestre.FuncaoSoma();
                        oMestre.ssColocaTotais();
                        break;
                    case 2:


                        // mestreBSource.SuspendBinding();
                        itensFiscalBSource.CurrentItemChanged -= Itens_PositionChanged_VendaDisp;
                        itensFiscalBSource.CurrentItemChanged -= Itens_PositionChanged_NfeDisp;
                        mestreBSource.CurrentItemChanged -= Mestre_PositionChanged;
                        try
                        {
                            var dado = odataset.Tables["Mestre"].AsEnumerable().Where(row =>
                           (((fazenda_contabil.Trim() == "") ? (row.Field<string>("FIRMA") != "") : (row.Field<string>("FIRMA").Trim() == fazenda_contabil.Trim())) &&
                            ((Cfop.Trim() == "") ? (row.Field<string>("CFOP") != "") : (row.Field<string>("CFOP").Trim() == Cfop.Trim())) &&
                                   (row.Field<string>("TPNF") == "S") &&
                                  (row.Field<string>("PRODFIS") == ClassEventosNF.CACAU_)));
                            if ((dado != null) && (dado.Count() > 0)) { mestreBSource.DataSource = dado.CopyToDataTable(); }
                            else { mestreBSource.DataSource = odataset.Tables["Mestre"].Clone(); }
                        }
                        catch (Exception)
                        {
                            mestreBSource.DataSource = odataset.Tables["Mestre"].Clone();

                        }
                        // mestreBSource.ResetBindings(true);
                        itensFiscalBSource.CurrentItemChanged += Itens_PositionChanged_VendaDisp;
                        mestreBSource.CurrentItemChanged += Mestre_PositionChanged;
                        if ((mestreBSource.DataSource as DataTable).Rows.Count == 0)
                        {
                            // EventArgs arg = new EventArgs();
                            Mestre_PositionChanged(mestreBSource, null);
                        }
                        mestreBSource.ResetCurrentItem();
                        oMestre.FuncaoSoma();
                        oMestre.ssColocaTotais();
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async void NfeLote_Delete()
        {
            DataRowView rownfe = (nfeLoteBSource.Current as DataRowView);
            double totquant = Convert.ToDouble(rownfe["KG"]);
            int id_item_nfe = Convert.ToInt32(rownfe["LOTE_ID"]);
            DataRow drLote = (loteDispBSource.DataSource as DataTable).Select("(ID = " + id_item_nfe.ToString() + ")").FirstOrDefault();
            if (drLote != null)
            {
                drLote["SDOKG_ENT"] = Convert.ToDouble(drLote["SDOKG_ENT"]) + totquant;
            }
            else if (totquant > 0)
            {
                drLote = (loteDispBSource.DataSource as DataTable).NewRow();
                drLote["SETOR"] = rownfe["SETOR"];
                drLote["LOTE"] = rownfe["LOTE"];
                drLote["PROD"] = rownfe["PROD"];
                drLote["SAFRA"] = rownfe["SAFRA"];
                drLote["SDOKG_ENT"] = totquant;
                drLote["ID"] = id_item_nfe;
                drLote["DTA_ENT"] = rownfe["DTA_ENT"];
                // drLote["ID_LOTE"] = rownfe["LOTE_ID"];

                (loteDispBSource.DataSource as DataTable).Rows.Add(drLote);
                (loteDispBSource.DataSource as DataTable).AcceptChanges();
                loteDispBSource.ResetBindings(true);
            }

            bool resposta = await Prepara_Sql.OpereDeleteRegistroServidorAsync(rownfe.Row, "NFE_LOTE");
            if (resposta)
            {
                rownfe.Delete();
                (nfeLoteBSource.DataSource as DataTable).AcceptChanges();
                oLoteDisp.FuncaoSoma();
                oLoteDisp.ssColocaTotais();
                oNfeLote.FuncaoSoma();
                oNfeLote.ssColocaTotais();
            }

        }

        public async void LoteDisp_to_NFeLote()
        {

            if (loteDispBSource.Count == 0) return;
            DataRowView mestre = mestreBSource.Current as DataRowView;
            if ((mestre == null) || ((mestre["Evento"].ToString().Length > 0) && (mestre["Evento"].ToString().ToUpper().Substring(0, 4) == "CANC")))
            {
                MessageBox.Show("Operação Ilegal!! Nota Cancelada!!");
                return;
            }
            DataRowView itensFis = (itensFiscalBSource.Current as DataRowView);
            double totalancar = Convert.ToDouble(itensFis["cquant"]);
            double totlancado = 0;
            DataTable nfeLote = null;


            if (nfeLoteBSource.Count > 0)
            {
                nfeLote = (nfeLoteBSource.DataSource as DataTable);
                nfeLoteBSource.SuspendBinding();
                foreach (DataRow orow in nfeLote.Rows)
                {
                    totlancado = totlancado + Convert.ToDouble(orow["KG"]);
                }
                nfeLoteBSource.ResetBindings(false);
            }
            if (totlancado == totalancar)
            {
                MessageBox.Show("Não Há Sdo Quant a Ligar");
                // return;
            }
            else if (totlancado > totalancar)
            {
                MessageBox.Show("Erro Total Lançado Maior que do Item");
                return;
            }
            // reserve os registros selecionados
            List<DataRow> lst = new List<DataRow>();
            foreach (DataGridViewRow row in oLoteDisp.oDataGridView.SelectedRows)
            {

                DataRowView oData = (row.DataBoundItem as DataRowView);

                lst.Add(oData.Row);
            }
            if (lst.Count == 0)
            {
                foreach (DataGridViewRow row in oLoteDisp.oDataGridView.Rows)
                {
                    DataRowView oData = (row.DataBoundItem as DataRowView);
                    lst.Add(oData.Row);
                }
            }
            int max_id = 0;
            try
            {
                odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[] { "Select MAX(ID) as max_id from NFE_LOTE" }));
                max_id = Convert.ToInt32(odataset.Tables[0].Rows[0][0]);
            }
            catch (Exception)
            {
            }
            totalancar = totalancar - totlancado;
            double quant = 0;
            // exemplo 
            foreach (DataRow orow in lst)
            {
                if (totalancar == 0) break;
                if (Convert.ToDouble(orow["SDOKG_ENT"]) > 0)
                {
                    quant = Convert.ToDouble(orow["SDOKG_ENT"]);
                    if (quant > totalancar) { quant = totalancar; }
                }
                DataRow newRow = nfeLote.NewRow();


                newRow["ID"] = max_id;
                max_id += 1;
                newRow["ITENSNF_ID"] = mestre["NFISCAL"];
                newRow["LOTE_ID"] = orow["ID_LOTE"];
                newRow["DTA_ENT"] = orow["DTA_ENT"];
                newRow["DTA_EMI"] = mestre["DTAEMI"];
                newRow["PROD"] = orow["PROD"];
                newRow["LOTE"] = orow["LOTE"];
                newRow["SETOR"] = orow["SETOR"];
                newRow["SAFRA"] = orow["SAFRA"];
                newRow["NFISCAL"] = mestre["NFISCAL"];
                newRow["SERIE"] = mestre["sERIE"];
                newRow["KG"] = quant;
                newRow["KG_ANT"] = quant;

                totalancar = totalancar - quant;

                // para combatibilizar com o itensfis..
                newRow["FORN_CLI"] = "    20  ";
                newRow["E_ORIG"] = "    " + orow["SETOR"].ToString();
                if (orow["SETOR"].ToString() == "9")
                {
                    newRow["E_ORIG"] = newRow["E_ORIG"].ToString() + " 9";
                }
                else
                {
                    newRow["E_ORIG"] = newRow["E_ORIG"].ToString() + "  ";
                }

                newRow["CODIGOF"] = "122";
                nfeLote.Rows.Add(newRow);
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(newRow, DataRowState.Added, "NFE_LOTE", new List<string>(new string[] { "KG_ANT" }));
                if (!ok)
                {
                    MessageBox.Show("Erro ao Inserir Registro NFE_LOTE "); return;
                }
                nfeLote.AcceptChanges();
                if ((Convert.ToDouble(orow["SDOKG_ENT"]) - quant) == 0) { orow.Delete(); }
                else
                {
                    orow.BeginEdit();
                    orow["SDOKG_ENT"] = Convert.ToDouble(orow["SDOKG_ENT"]) - quant;
                    orow.EndEdit();
                }
            }
            (loteDispBSource.DataSource as DataTable).AcceptChanges();
            oLoteDisp.FuncaoSoma();
            oLoteDisp.ssColocaTotais();
            oNfeLote.FuncaoSoma();
            oNfeLote.ssColocaTotais();

        }

        // VENDAS
        public async void NfeVenda_Delete()
        {
            DataRowView rownfe = (nfeVendaBSource.Current as DataRowView);
            double totquant = Convert.ToDouble(rownfe["QUANT_V"]);
            int id_item_venda = Convert.ToInt32(rownfe["ID_VENDA"]);
            DataTable venda = (vendaDispBSource.DataSource as DataTable);
            DataRow drVenda = null;
            if (venda.Rows.Count > 0)
                drVenda = venda.Select("(VENDAS_ID = " + id_item_venda.ToString() + ")").FirstOrDefault();
            DataRow drOrigem = null;
            try
            {
                drOrigem = odataset.Tables["Venda_Disp"].Select("(VENDAS_ID = " + id_item_venda.ToString() + ")").FirstOrDefault();
            }
            catch (Exception E) { }
            if (drOrigem == null) { return; }

            if (drVenda != null)
            {
                drVenda["SDOQUANT"] = Convert.ToDouble(drVenda["SDOQUANT"]) + totquant;
                drOrigem["SDOQUANT"] = drVenda["SDOQUANT"];

            }
            else if (totquant > 0)
            {
                drVenda = (vendaDispBSource.DataSource as DataTable).NewRow();
                drOrigem["SDOQUANT"] = totquant;
                foreach (DataColumn col in drOrigem.Table.Columns)
                {
                    drVenda[col.ColumnName] = drOrigem[col.ColumnName];
                }
                  (vendaDispBSource.DataSource as DataTable).Rows.Add(drVenda);
            }

            (vendaDispBSource.DataSource as DataTable).AcceptChanges();
            vendaDispBSource.ResetBindings(true);
            odataset.Tables["Venda_Disp"].AcceptChanges();

            bool resposta = await Prepara_Sql.OpereDeleteRegistroServidorAsync(rownfe.Row, "NFE_VENDA");
            if (resposta)
            {
                rownfe.Delete();
                (nfeVendaBSource.DataSource as DataTable).AcceptChanges();
                oVendaDisp.FuncaoSoma();
                oVendaDisp.ssColocaTotais();
                oNfeVenda.FuncaoSoma();
                oNfeVenda.ssColocaTotais();
            }

        }

        public async void VendaDisp_to_NFeVenda()
        {
            if (vendaDispBSource.Count == 0) return;
            DataRowView mestre = mestreBSource.Current as DataRowView;
            // itens da nota fiscais a lançar no contrato de venda
            DataRowView itensNF = (itensFiscalBSource.Current as DataRowView);
            double totalancar = 0;
            totalancar = +Convert.ToDouble(itensNF["cquant"]);

            double totlancado = 0;
            DataTable nfeVenda = null;

            nfeVenda = (nfeVendaBSource.DataSource as DataTable);
            if (nfeVendaBSource.Count > 0)
            {
                nfeVendaBSource.SuspendBinding();
                foreach (DataRow orow in nfeVenda.Rows)
                {
                    totlancado = totlancado + Convert.ToDouble(orow["QUANT_V"]);
                }
                nfeVendaBSource.ResetBindings(false);
            }
            if (totlancado == totalancar)
            {
                MessageBox.Show("Não Há Sdo Quant a Ligar");
                // return;
            }
            else if (totlancado > totalancar)
            {
                MessageBox.Show("Erro Total Lançado Maior que do Item");
                return;
            }

            /*
             */
            string firma_unica = mestre["FIRMA"].ToString();
            // reserve os registros selecionados
            List<DataRow> lst = new List<DataRow>();
            foreach (DataGridViewRow row in oVendaDisp.oDataGridView.SelectedRows)
            {
                DataRowView oData = (row.DataBoundItem as DataRowView);
                if (firma_unica == "")
                { firma_unica = oData["FIRMA"].ToString(); }
                else if (firma_unica != oData["FIRMA"].ToString()) continue;
                lst.Add(oData.Row);
            }
            if (lst.Count == 0)
            {
                foreach (DataGridViewRow row in oVendaDisp.oDataGridView.Rows)
                {
                    DataRowView oData = (row.DataBoundItem as DataRowView);
                    if (firma_unica == "")
                    { firma_unica = oData["FIRMA"].ToString(); }
                    else if (firma_unica != oData["FIRMA"].ToString()) continue;
                    lst.Add(oData.Row);
                }

            }
            if ((mestre["FIRMA"].ToString() == "") && (firma_unica != ""))
            {
                mestre.BeginEdit();
                mestre["FIRMA"] = firma_unica;
                mestre.EndEdit(); // verificar se é necessário alterr no servidor
            }


            int max_id = 0;
            try
            {
                odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[] { "Select MAX(ID) as max_id from NFE_VENDA" }));
                max_id = Convert.ToInt32(odataset.Tables[0].Rows[0][0]);
            }
            catch (Exception)
            {
            }

            // Verificar validate no oNfeVenda.oTable.FindField('QUANT_V').OnValidate

            totalancar = totalancar - totlancado;
            double quant = 0;
            // exemplo 
            foreach (DataRow orow in lst)
            {
                if (totalancar == 0) break;
                if (Convert.ToDouble(orow["SDOQUANT"]) > 0)
                {
                    quant = Convert.ToDouble(orow["SDOQUANT"]);
                    if (quant > totalancar) { quant = totalancar; }
                }
                DataRow newRow = nfeVenda.NewRow();


                newRow["ID"] = max_id;
                max_id += 1;
                newRow["ID_ITENS"] = mestre["NFISCAL"];

                newRow["ID_VENDA"] = orow["VENDAS_ID"];

                newRow["DTA_VENDA"] = orow["DATA"];
                newRow["DTA_EMI"] = mestre["DTAEMI"];
                newRow["PROD"] = orow["PROD"];
                newRow["CONT"] = orow["CONT"];
                newRow["FIRMA"] = orow["FIRMA"];
                newRow["SAFRA"] = orow["SAFRA"];
                newRow["PROD_TP"] = orow["PROD_TP"];
                newRow["CERTIF"] = orow["CERTIF"];
                newRow["NFISCAL"] = mestre["NFISCAL"];
                newRow["SERIE"] = mestre["SERIE"];
                newRow["QUANT_V"] = quant;
                newRow["QUANTV_ANT"] = quant;
                newRow["E_ORIG"] = "    20  ";
                newRow["CODIGOF"] = "511";
                totalancar = totalancar - quant;
                nfeVenda.Rows.Add(newRow);
                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(newRow, DataRowState.Added,"NFE_VENDA", new List<string>(new string[] { "QUANTV_ANT" }));
                if (!ok)
                {
                    MessageBox.Show("Erro ao Inserir Registro NFE_VENDA "); return;
                }
                nfeVenda.AcceptChanges();
                if ((Convert.ToDouble(orow["SDOQUANT"]) - quant) == 0) { orow.Delete(); }
                else
                {
                    orow.BeginEdit();
                    orow["SDOQUANT"] = Convert.ToDouble(orow["SDOQUANT"]) - quant;
                    orow.EndEdit();
                }
            }
         (vendaDispBSource.DataSource as DataTable).AcceptChanges();
            oVendaDisp.FuncaoSoma();
            oVendaDisp.ssColocaTotais();
            oNfeVenda.FuncaoSoma();
            oNfeVenda.ssColocaTotais();
        }
    }
}
