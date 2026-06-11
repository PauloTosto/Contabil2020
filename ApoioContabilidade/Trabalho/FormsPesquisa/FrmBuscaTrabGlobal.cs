using ApoioContabilidade.Trabalho.ServicesTrab;
using System;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.FormsPesquisa
{
    /// <summary>
    /// Popup modal para busca GLOBAL de trabalhador (qualquer fazenda).
    /// Usado pelo gerente quando precisa lançar ponto de trabalhador eventual
    /// e ainda não sabe o código.
    ///
    /// Operador digita parte do nome OU código, clica Buscar (ou Enter), seleciona
    /// uma linha e clica Selecionar (ou duplo-clique). DialogResult=OK e a
    /// propriedade <see cref="CodigoSelecionado"/> retorna o CODCAD escolhido.
    ///
    /// Adicionado 2026-05-11.
    /// </summary>
    public class FrmBuscaTrabGlobal : Form
    {
        private TextBox txBusca;
        private Button btnBuscar;
        private Button btnSelecionar;
        private Button btnCancelar;
        private DataGridView dgvResultados;
        private Label lblInstrucao;
        private Label lblStatus;

        /// <summary>CODCAD do trabalhador escolhido. Vazio se cancelado.</summary>
        public string CodigoSelecionado { get; private set; } = "";

        /// <summary>NOMECAD do trabalhador escolhido (para feedback visual no caller).</summary>
        public string NomeSelecionado { get; private set; } = "";

        /// <summary>Semana de referência (quinta-feira) — define a janela de aptidão.</summary>
        private readonly DateTime semanaRef;

        public FrmBuscaTrabGlobal(DateTime semana)
        {
            this.semanaRef = semana;
            InicializaUi();
        }

        private void InicializaUi()
        {
            this.Text = "Buscar trabalhador (qualquer fazenda) — semana " + semanaRef.ToString("dd/MM/yyyy");
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ClientSize = new Size(720, 420);

            lblInstrucao = new Label
            {
                Text = "Parte do nome ou código:",
                Location = new Point(12, 15),
                AutoSize = true
            };
            this.Controls.Add(lblInstrucao);

            txBusca = new TextBox
            {
                Location = new Point(170, 12),
                Size = new Size(330, 22),
                CharacterCasing = CharacterCasing.Upper
            };
            txBusca.KeyDown += TxBusca_KeyDown;
            this.Controls.Add(txBusca);

            btnBuscar = new Button
            {
                Text = "&Buscar",
                Location = new Point(510, 10),
                Size = new Size(90, 28)
            };
            btnBuscar.Click += BtnBuscar_Click;
            this.Controls.Add(btnBuscar);

            lblStatus = new Label
            {
                Location = new Point(12, 45),
                Size = new Size(688, 18),
                ForeColor = Color.DimGray,
                Text = "Digite pelo menos 3 caracteres. Mostra até 100 trabalhadores aptos na semana selecionada."
            };
            this.Controls.Add(lblStatus);

            dgvResultados = new DataGridView
            {
                Location = new Point(12, 70),
                Size = new Size(696, 280),
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                MultiSelect = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoGenerateColumns = false,
                RowHeadersVisible = false,
                BackgroundColor = SystemColors.Window
            };
            dgvResultados.Columns.Add(new DataGridViewTextBoxColumn { Name = "CODCAD",  HeaderText = "Código",   DataPropertyName = "CODCAD",  Width = 80 });
            dgvResultados.Columns.Add(new DataGridViewTextBoxColumn { Name = "NOMECAD", HeaderText = "Nome",     DataPropertyName = "NOMECAD", Width = 320 });
            dgvResultados.Columns.Add(new DataGridViewTextBoxColumn { Name = "SETOR",   HeaderText = "Setor",    DataPropertyName = "SETOR",   Width = 60 });
            dgvResultados.Columns.Add(new DataGridViewTextBoxColumn { Name = "GLECAD",  HeaderText = "Gleba",    DataPropertyName = "GLECAD",  Width = 80 });
            dgvResultados.Columns.Add(new DataGridViewTextBoxColumn { Name = "ADMI",    HeaderText = "Admissão", DataPropertyName = "ADMI",    Width = 90 });
            dgvResultados.Columns.Add(new DataGridViewTextBoxColumn { Name = "DEMI",    HeaderText = "Demissão", DataPropertyName = "DEMI",    Width = 90 });
            dgvResultados.CellDoubleClick += DgvResultados_CellDoubleClick;
            this.Controls.Add(dgvResultados);

            btnSelecionar = new Button
            {
                Text = "&Selecionar",
                Location = new Point(498, 365),
                Size = new Size(100, 32),
                Enabled = false
            };
            btnSelecionar.Click += BtnSelecionar_Click;
            this.Controls.Add(btnSelecionar);

            btnCancelar = new Button
            {
                Text = "&Cancelar",
                Location = new Point(608, 365),
                Size = new Size(100, 32),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnBuscar;
            this.CancelButton = btnCancelar;

            dgvResultados.SelectionChanged += (s, e) =>
                btnSelecionar.Enabled = dgvResultados.SelectedRows.Count > 0;
        }

        private void TxBusca_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                BtnBuscar_Click(sender, e);
            }
        }

        private async void BtnBuscar_Click(object sender, EventArgs e)
        {
            string termo = txBusca.Text.Trim();
            if (termo.Length < 3)
            {
                lblStatus.Text = "Digite pelo menos 3 caracteres.";
                lblStatus.ForeColor = Color.DarkRed;
                return;
            }

            btnBuscar.Enabled = false;
            UseWaitCursor = true;
            lblStatus.ForeColor = Color.DimGray;
            lblStatus.Text = "Buscando...";
            try
            {
                DataTable resultado = await ServicoFiltroTrab.BuscaTrabalhadorGlobal(termo, semanaRef);
                if (resultado == null)
                {
                    lblStatus.Text = "Falha ao consultar servidor.";
                    lblStatus.ForeColor = Color.DarkRed;
                    dgvResultados.DataSource = null;
                    return;
                }
                dgvResultados.DataSource = resultado;
                int n = resultado.Rows.Count;
                lblStatus.Text = n == 0
                    ? "Nenhum trabalhador ativo encontrado para esse termo."
                    : (n == 100
                        ? "100 resultados (limite). Refine o termo se não encontrou."
                        : n + " resultado(s).");
                lblStatus.ForeColor = n == 0 ? Color.DarkRed : Color.DimGray;
            }
            finally
            {
                btnBuscar.Enabled = true;
                UseWaitCursor = false;
            }
        }

        private void DgvResultados_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            BtnSelecionar_Click(sender, e);
        }

        private void BtnSelecionar_Click(object sender, EventArgs e)
        {
            if (dgvResultados.SelectedRows.Count == 0) return;
            DataGridViewRow row = dgvResultados.SelectedRows[0];
            CodigoSelecionado = (row.Cells["CODCAD"].Value ?? "").ToString().Trim();
            NomeSelecionado = (row.Cells["NOMECAD"].Value ?? "").ToString().Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
