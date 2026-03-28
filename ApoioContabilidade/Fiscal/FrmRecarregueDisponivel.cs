using ApoioContabilidade.Fiscal.Servicos;
using ClassFiltroEdite;
using System;
using System.Windows.Forms;

namespace ApoioContabilidade.Fiscal
{
    public partial class FrmRecarregueDisponivel : Form
    {
        MonteGrid oMestre;
        MonteGrid oDetalhe;
        public RecarregueManejoDados manejoDados;
        bool inclueZerados = false;
        int index = 0;
        public FrmRecarregueDisponivel()
        {
            InitializeComponent();
            manejoDados = new RecarregueManejoDados();
            MonteGrids();
            oMestre.ConfigureDBGridView();
            oDetalhe.ConfigureDBGridView();
            Inicie();

        }
        private async void Inicie()
        {
            bool ok = await manejoDados.inicieRecarregue(oMestre, oDetalhe);
            if (ok)
              { manejoDados.SelecaoIndex(index, inclueZerados); }
            else { MessageBox.Show("Não encontrado Lotes Disponíveis");  }

        }
        private void MonteGrids()
        {
            oMestre = new MonteGrid();
            oMestre.modeloAlternativo = true;
            oMestre.LinhasMinimas = 4; // default é 3
            dgvMestre.DataSource = manejoDados.fazendasBSource;
            oMestre.oDataGridView = dgvMestre;
            oMestre.ssTotal = statusMestre;
            oMestre.Clear();
            oMestre.AddValores("SETOR", "Setor", 5, "", false, 0, "");
            oMestre.AddValores("INICIO", "Inicio", 8, "dd/MM/yyyy", false, 0, "");
            oMestre.AddValores("FIM", "Fim", 8, "dd/MM/yyyy", false, 0, "");
            oMestre.AddValores("Quant", "Disp.(kg)", 12, "###,##0.00", true, 0, "");
            dgvMestre.AllowUserToAddRows = false;
            dgvMestre.AllowUserToDeleteRows = false;
            dgvMestre.AllowUserToOrderColumns = true;
            dgvMestre.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvMestre.StandardTab = true;
            dgvMestre.ReadOnly = true;
            dgvMestre.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

            oDetalhe = new MonteGrid();
            oDetalhe.modeloAlternativo = true;
            oDetalhe.LinhasMinimas = 4; // default é 3
            dgvDetalhe.DataSource = manejoDados.dispLoteBSource;
            oDetalhe.oDataGridView = dgvDetalhe;
            oDetalhe.ssTotal = statusDetalhe;
            oDetalhe.Clear();
            oDetalhe.AddValores("SAFRA", "Safra", 5, "", false, 0, "");
            oDetalhe.AddValores("SETOR", "Setor", 5, "", false, 0, "");
            oDetalhe.AddValores("LOTE", "Lote", 5, "", false, 0, "");
            oDetalhe.AddValores("APRONTE", "Apronte", 8, "dd/MM/yyyy", false, 0, "");
            oDetalhe.AddValores("Benef", "Faz(kg)", 10, "###,##0.00", true, 0, "");
            oDetalhe.AddValores("DTA_ENT", "Dta Dep", 8, "dd/MM/yyyy", false, 0, "");
            oDetalhe.AddValores("KG_ENT", "Dep(kg)", 10, "###,##0.00", true, 0, "");
            oDetalhe.AddValores("SDOKG_ENT", "Disp.(kg)", 10, "###,##0.00", true, 0, "");
            dgvDetalhe.AllowUserToAddRows = false;
            dgvDetalhe.AllowUserToDeleteRows = false;
            dgvDetalhe.AllowUserToOrderColumns = true;
            dgvDetalhe.SelectionMode = DataGridViewSelectionMode.CellSelect;
            dgvDetalhe.StandardTab = true;
            dgvDetalhe.ReadOnly = true;
            dgvDetalhe.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;

        }

        private void checkBox1_CheckStateChanged(object sender, EventArgs e)
        {

            if ((sender as CheckBox).Checked) { manejoDados.InclueZerados = true; }
            else { manejoDados.InclueZerados = false; }
            inclueZerados = manejoDados.InclueZerados;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            oMestre.tituloExcel = "Exporta Excel";
            //oMestre.ExportaExcel();
            // oMestre.oDataGridView.SuspendLayout();
            int position = manejoDados.fazendasBSource.Position;
            // manejoDados.fazendasBSource.SuspendBinding();
            oMestre.MestreDetalhe(oDetalhe);
            manejoDados.fazendasBSource.Position = position;
           // manejoDados.fazendasBSource.ResetBindings(true);
            // oMestre.oDataGridView.ResumeLayout(true);
        }
    }
    }
