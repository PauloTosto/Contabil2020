using System;
using System.Windows.Forms;
using ApoioContabilidade.Fiscal.Filtros;
using ApoioContabilidade.Fiscal.Servicos;
using ClassFiltroEdite;
//using PrjApiParceiro_C.Config_Componentes;
//using PrjApiParceiro_C.Filtros;
//using PrjApiParceiro_C.Fiscais.Servicos;

namespace ApoioContabilidade.Fiscal
{
    public partial class FrmFiscal_Novo : Form
    {
        MonteGrid oMestre;
        MonteGrid oItensFis;

        MonteGrid oLoteDisp;
        MonteGrid oNfeLote;

        MonteGrid oVendaDisp;
        MonteGrid oNfeVenda;

        Int16 selecaoIndex = -1;
        Int16 indexCompleto = -1;

        // controles de navegação
        bool deleteNfLote = false;
        bool deleteVenda = false;
       // principal Classe que maneja od dados
        ManejoDados manejoDados;

        DateTime data1, data2;
        public FrmFiscal_Novo()
        {
            InitializeComponent();
            // ao criar cria os bindings correspondente
            manejoDados = new ManejoDados();
            this.KeyPreview = true;  // para que o formulario receba antes dos controles os eventos de teclado
            // os tabControls.tabstop ficarão falsos, ajudar navegação
            tpMestre.TabStop = false;
            tcMestre.TabStop = false;
            tpItens.TabStop = false;
        
            tpDisponivel.TabStop = false;
            tpLigados.TabStop = false;
            tpDisponivelVenda.TabStop = false;
            tpLigadosVenda.TabStop = false;

            txCFOP.TabStop = false;
            txFazenda.TabStop = false;
            gbCritica.TabStop = false;
            gbFiscais.TabStop = false;
            rbCompleto.TabStop = false;
            rbEntradas.TabStop = false;
            rbGeral.TabStop = false;
            rbVendas.TabStop = false;
            rbIncompleto.TabStop = false;
            rbTodas.TabStop = false;
            rbNenhuma.TabStop = false;

            tcLink.Visible = false;
            tcVenda.Visible = false;
            gbCritica.Visible = false;
            gbFiltro.Visible = false;
            gbFiscais.Enabled = false;

            toolStripNfe.Enabled = false;
            toolStripVenda.Enabled = false;
            toolStripDelete.Enabled = false;
            MonteGrids();
            selecaoIndex = 0;
            indexCompleto = 0;
            inicializeDados();
        }
        private async void inicializeDados()
        {
            data1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            data2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            var oformConsulta = new FrmFiltroBasico();
            oformConsulta.data1 = data1;
            oformConsulta.data2 = data2;
            var retorno = oformConsulta.ShowDialog();
            if (retorno != DialogResult.OK) { return; }
            toolStripConsulta.Enabled = false;
            try
            {


                rbGeral.Checked = true;
                data1 = oformConsulta.data1;
                data2 = oformConsulta.data2;
                lbPeriodo.Text = "Periodo de " + data1.ToString("dd/MM/yyyy") + " até " + data2.ToString("dd/MM/yyyy");
                if (!lbPeriodo.Visible) lbPeriodo.Visible = true;
                // Inicialização das rotinas de dados
                // Rotina Necessária para LINKAR OS DADOS AOS COMPONENTES ATRAVES DO BINDINGSOURCE
                bool resultado = await manejoDados.Inicialize_ManejoDados(data1, data2, oMestre,
                      oItensFis, oVendaDisp, oNfeVenda, oLoteDisp, oNfeLote);

                if (!resultado)
                {
                    MessageBox.Show("Problemas na Inicialização dos Dados");
                    return;
                }
                oMestre.ConfigureDBGridView();
                oMestre.FuncaoSoma();
                oMestre.ssColocaTotais();

                oItensFis.ConfigureDBGridView();

                oLoteDisp.ConfigureDBGridView();
                oNfeLote.ConfigureDBGridView();

                oVendaDisp.ConfigureDBGridView();
                oNfeVenda.ConfigureDBGridView();

                manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
            }
            finally
            {
                toolStripConsulta.Enabled = true;
            }
            gbFiscais.Enabled = true;
        }

        private async void toolStripConsulta_Click(object sender, EventArgs e)
        {
            var oformConsulta = new FrmFiltroBasico();
            oformConsulta.data1 = data1;
            oformConsulta.data2 = data2;
            var retorno = oformConsulta.ShowDialog();
            if (retorno != DialogResult.OK) { return; }
            rbGeral.Checked = true;
            data1 = oformConsulta.data1;
            data2 = oformConsulta.data2;
            lbPeriodo.Text = "Periodo de " + data1.ToString("dd/MM/yyyy") + " até " + data2.ToString("dd/MM/yyyy");
            if (!lbPeriodo.Visible) lbPeriodo.Visible = true;
            // Consulta dados
            gbFiscais.Enabled = false;
            (sender as ToolStripButton).Enabled = false;
            Boolean ok = false;
            try
            {
                ok = await manejoDados.ConsulteServidor(data1, data2);
                if (ok) 
                    manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
            }
            catch (Exception) {
                MessageBox.Show("Problema na Carga Fiscal ");
            }
            finally
            {
                (sender as ToolStripButton).Enabled = true;
                gbFiscais.Enabled = true;
            }
        }

        private void MonteGrids()
        {
            oMestre = new MonteGrid();
            oMestre.modeloAlternativo = true;
            oMestre.LinhasMinimas = 4; // default é 3
            dgvMestre.DataSource = manejoDados.mestreBSource;
            oMestre.oDataGridView = dgvMestre;
            oMestre.ssTotal = statusMestre;
            oMestre.Clear();
            oMestre.AddValores("DTAEMI", "Emissão", 8, "dd/MM/yyyy", false, 0, "");
            oMestre.AddValores("EVENTO", "Canc", 6, "", false, 0, "");
            oMestre.AddValores("TPNF", "E/S", 3, "", false, 0, "");
            oMestre.AddValores("CFOP", "cfpo", 5, "", false, 0, "");
            oMestre.AddValores("NATOP", "NatOp", 15, "", false, 0, "");
            oMestre.AddValores("CRITICA", "Critica", 12, "", false, 0, "");
            oMestre.AddValores("NFISCAL", "NFiscal", 8, "", false, 0, "");
            oMestre.AddValores("DEST_NOME", "Destinatario", 30, "", false, 0, "");
            oMestre.AddValores("FIRMA", "Contabil", 10, "", false, 0, "");
            oMestre.AddValores("QUANT_NF", "Quant.Fiscal", 0, "##,##0.00", true, 0, "");
            oMestre.AddValores("VLRNF", "Vlr Fiscal", 0, "###,##0.00", true, 0, "");
            oMestre.AddValores("CODFAZ", "Faz.", 5, "", false, 0, "");
            oMestre.AddValores("DEST_CNPJ", "CNPJ/CPF", 14, "", false, 0, "");
            oMestre.AddValores("XLGR", "Nome Fazenda", 12, "", false, 0, "");
            oMestre.AddValores("FINNF", "Finalidade", 12, "", false, 0, "");
            oMestre.AddValores("IDDEST", "Destino Operação", 12, "", false, 0, "");
            dgvMestre.AllowUserToAddRows = false;
            dgvMestre.AllowUserToDeleteRows = false;
            dgvMestre.AllowUserToOrderColumns = true;
            dgvMestre.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvMestre.StandardTab = true;
            dgvMestre.ReadOnly = true;
            dgvMestre.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;


            oItensFis = new MonteGrid();
            oItensFis.modeloAlternativo = true;
            oItensFis.LinhasMinimas = 4; // default é 3
            dgvItens.DataSource = manejoDados.itensFiscalBSource;
            oItensFis.oDataGridView = dgvItens;
            oItensFis.Clear();
            oItensFis.AddValores("CFOP", "CFOP", 5, "", false, 0, "");
            oItensFis.AddValores("cProd", "cProd", 4, "", false, 0, "");
            oItensFis.AddValores("xProd", "Descricao Produto", 15, "", false, 0, "");
            oItensFis.AddValores("cUnid", "Unid Com", 4, "", false, 0, "");
            oItensFis.AddValores("cQuant", "Quant.Com", 0, "###,##0.00", true, 0, "");
            oItensFis.AddValores("cPUnit", "Preço Unit", 0, "###,##0.00", true, 0, "");
            oItensFis.AddValores("cVlr", "Valor", 0, "###,##0.00", true, 0, "");
            oItensFis.AddValores("InfAdProd", "Informe Adcionais", 100, "", false, 0, "");
            dgvItens.AllowUserToAddRows = false;
            dgvItens.AllowUserToDeleteRows = false;
            dgvItens.AllowUserToOrderColumns = true;
            dgvItens.ReadOnly = true;
            dgvItens.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvItens.StandardTab = true;
            dgvItens.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            // NFE_DISPONIVEIS
            oLoteDisp = new MonteGrid();
            oLoteDisp.modeloAlternativo = true;
            oLoteDisp.LinhasMinimas = 3; // default é 3
            dgvDisponivel.DataSource = manejoDados.loteDispBSource;
            oLoteDisp.oDataGridView = dgvDisponivel;
            oLoteDisp.ssTotal = statusDisponivel;
            oLoteDisp.Clear();
            oLoteDisp.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            oLoteDisp.AddValores("LOTE", "Lote", 5, "", false, 0, "");
            oLoteDisp.AddValores("DTA_ENT", "Dta Depos", 8, "dd/MM/yyyy", false, 0, "");
            oLoteDisp.AddValores("SDOKG_ENT", "Quant Disponivel", 0, "###,##0.00", true, 0, "");
            dgvDisponivel.AllowUserToAddRows = false;
            dgvDisponivel.AllowUserToDeleteRows = false;
            dgvDisponivel.AllowUserToOrderColumns = true;
            dgvDisponivel.ReadOnly = true;
            dgvDisponivel.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvDisponivel.StandardTab = true;

            oNfeLote = new MonteGrid();
            oNfeLote.modeloAlternativo = true;
            oNfeLote.LinhasMinimas = 3; // default é 3
            dgvLink.DataSource = manejoDados.nfeLoteBSource;
            oNfeLote.oDataGridView = dgvLink;
            oNfeLote.ssTotal = statusLink;
            oNfeLote.Clear();
            oNfeLote.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            oNfeLote.AddValores("LOTE", "Lote", 5, "", false, 0, "");
            oNfeLote.AddValores("DTA_ENT", "Dta Depos", 8, "dd/MM/yyyy", false, 0, "");
            oNfeLote.AddValores("DTA_EMI", "Emissão", 8, "dd/MM/yyyy", false, 0, "");
            oNfeLote.AddValores("KG", "Quant Link", 0, "###,##0.00", true, 0, "");
            dgvLink.AllowUserToAddRows = false;
            dgvLink.AllowUserToDeleteRows = false;
            dgvLink.AllowUserToOrderColumns = true;
            dgvLink.ReadOnly = true;
            dgvLink.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvLink.StandardTab = true;

            oVendaDisp = new MonteGrid();
            oVendaDisp.modeloAlternativo = true;
            oVendaDisp.LinhasMinimas = 3; // default é 3
            dgvDispVenda.DataSource = manejoDados.vendaDispBSource;
            oVendaDisp.oDataGridView = dgvDispVenda;
            oVendaDisp.ssTotal = statusDispVenda;
            oVendaDisp.Clear();
            oVendaDisp.AddValores("FIRMA", "Firma", 8, "", false, 0, "");
            oVendaDisp.AddValores("DESCFIRMA", "Nome", 20, "", false, 0, "");
            oVendaDisp.AddValores("PROD_TP", "Tipo Prod", 12, "", false, 0, "");  // GetSet.TpProdGetText;
            oVendaDisp.AddValores("CERTIF", "Certif.", 6, "", false, 0, "");
            oVendaDisp.AddValores("SAFRA", "Safra", 4, "", false, 0, "");
            oVendaDisp.AddValores("CONT", "Contrato", 4, "", false, 0, "");
            oVendaDisp.AddValores("DATA", "Dta", 8, "dd/MM/yyyy", false, 0, "");
            oVendaDisp.AddValores("SDOQUANT", "Quant Disponivel", 0, "###,##0.00", true, 0, "");
            dgvDispVenda.AllowUserToAddRows = false;
            dgvDispVenda.AllowUserToDeleteRows = false;
            dgvDispVenda.AllowUserToOrderColumns = true;
            dgvDispVenda.ReadOnly = true;
            dgvDispVenda.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvDispVenda.StandardTab = true;

            oNfeVenda = new MonteGrid();
            oNfeVenda.modeloAlternativo = true;
            oNfeVenda.LinhasMinimas = 3; // default é 3
            dgvLinkVenda.DataSource = manejoDados.nfeVendaBSource;
            oNfeVenda.oDataGridView = dgvLinkVenda;
            oNfeVenda.ssTotal = statusLinkVenda;
            oNfeVenda.Clear();
            oNfeVenda.AddValores("FIRMA", "Firma", 8, "", false, 0, "");
            oNfeVenda.AddValores("SAFRA", "Safra", 4, "", false, 0, "");
            oNfeVenda.AddValores("CONT", "Contrato", 4, "", false, 0, "");
            oNfeVenda.AddValores("DTA_VENDA", "Dta Venda", 8, "dd/MM/yyyy", false, 0, "");
            oNfeVenda.AddValores("QUANT_V", "Quant.", 0, "###,##0.00", true, 0, "");
            dgvLinkVenda.AllowUserToAddRows = false;
            dgvLinkVenda.AllowUserToDeleteRows = false;
            dgvLinkVenda.AllowUserToOrderColumns = true;
            dgvLinkVenda.ReadOnly = true;
            dgvLinkVenda.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvLinkVenda.StandardTab = true;

        }
        // Opçoes entre ver o geral das notas fiscais (entrada e saida) ou as entradas ou as saidas
        private void rbEntradas_Click(object sender, EventArgs e)
        {
            selecaoIndex = 1;
            tcVenda.Visible = false;
            tcLink.Visible = true;
            gbCritica.Visible = true;
            gbFiltro.Visible = true;
            lbFazenda_Contabil.Text = "FAZENDA:";
            manejoDados.SelecaoIndex(selecaoIndex,txFazenda.Text,txCFOP.Text,indexCompleto);
        }

        private void rbVendas_Click(object sender, EventArgs e)
        {
            selecaoIndex = 2;
            gbCritica.Visible = false;
            gbFiltro.Visible = true;
            lbFazenda_Contabil.Text = "CONTABIL:";
            tcLink.Visible = false;
            tcVenda.Visible = true;
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }

        private void rbGeral_Click(object sender, EventArgs e)
        {
            selecaoIndex = 0;
            gbCritica.Visible = false;
            gbFiltro.Visible = false;
            tcVenda.Visible = false;
            tcLink.Visible = false;
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }
        private void rbNenhuma_Click(object sender, EventArgs e)
        {
            indexCompleto = 3;
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }

        private void rbIncompleto_Click(object sender, EventArgs e)
        {
            indexCompleto = 2;
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }

        private void rbCompleto_Click(object sender, EventArgs e)
        {
            indexCompleto = 1;
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }

        private void rbTodas_Click(object sender, EventArgs e)
        {
            indexCompleto = 0;
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }

        private void txCFOP_Leave(object sender, EventArgs e)
        {
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
        }

        private void txFazenda_Leave(object sender, EventArgs e)
        {
            manejoDados.SelecaoIndex(selecaoIndex, txFazenda.Text, txCFOP.Text, indexCompleto);
            
        }

        private bool SelecioneGrupoRadio(GroupBox grupo)
        {
            bool processou = false;
            if (grupo.Visible && grupo.Enabled)
                foreach (Control radio in grupo.Controls)
                {
                    if ((radio is RadioButton) &&
                       ((radio as RadioButton).Checked))
                    {
                        radio.Select();
                        processou = true;
                        break;
                    }
                }
            return processou;
        }

        private void FrmFiscal_Novo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt)
            {
                switch (e.KeyCode)
                {
                    case Keys.F:
                        SelecioneGrupoRadio(gbFiscais);
                        break;
                    case Keys.T:
                        SelecioneGrupoRadio(gbCritica);
                        break;
                    default:
                        break;
                }
            }

        }

        private void toolStripNfe_Click(object sender, EventArgs e)
        {
            manejoDados.LoteDisp_to_NFeLote();
            oNfeLote.FuncaoSoma();
            oNfeLote.ssColocaTotais();
            oLoteDisp.FuncaoSoma();
            oLoteDisp.ssColocaTotais();
        }

        private void toolStripVenda_Click(object sender, EventArgs e)
        {
            manejoDados.VendaDisp_to_NFeVenda();
            oNfeVenda.FuncaoSoma();
            oNfeVenda.ssColocaTotais();
            oVendaDisp.FuncaoSoma();
            oVendaDisp.ssColocaTotais();
        }

        private void toolStripDelete_Click(object sender, EventArgs e)
        {
            if (deleteNfLote)
            {
                manejoDados.NfeLote_Delete();
            }
            else if (deleteVenda)
            {
                manejoDados.NfeVenda_Delete();
            }
        }

        private void dgvLink_Enter(object sender, EventArgs e)
        {
            deleteNfLote = true;
            toolStripDelete.Enabled = true;
        }

        private void dgvLink_Leave(object sender, EventArgs e)
        {
            deleteNfLote = false;
            toolStripDelete.Enabled = false;
        }


        private void dgvLinkVenda_Enter(object sender, EventArgs e)
        {
            deleteVenda = true;
            toolStripDelete.Enabled = true;
        }

        private void dgvLinkVenda_Leave(object sender, EventArgs e)
        {
            deleteVenda = false;
            toolStripDelete.Enabled = deleteVenda;
        }
        private void dgvDisponivel_Enter(object sender, EventArgs e)
        {
            toolStripNfe.Enabled = true;
        }

        private void dgvDisponivel_Leave(object sender, EventArgs e)
        {
            toolStripNfe.Enabled = false;
        }

        private void dgvDispVenda_Enter(object sender, EventArgs e)
        {
            toolStripVenda.Enabled = true;
        }

        private void  toolStripRecarregueDisponivel_Click(object sender, EventArgs e)
        {
            // bool ok = await manejoDados.Recarregue_Disponivel();
            (sender as ToolStripButton).Enabled = false;
            var oform = new FrmRecarregueDisponivel();
            oform.Show();
            (sender as ToolStripButton).Enabled = true;
        }

        private void dgvDispVenda_Leave(object sender, EventArgs e)
        {
            toolStripVenda.Enabled = false;
        }
    }
}
