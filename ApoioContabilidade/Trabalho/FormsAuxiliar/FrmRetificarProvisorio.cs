using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.ServicesTrab;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    /// <summary>
    /// Diálogo modal de RETIFICAÇÃO de lançamento provisório (CODSER='943').
    /// Aberto pelo botão "Retificar provisório" no FrmCltPonto, recebendo a
    /// DataRow CLTPONTO do registro selecionado.
    ///
    /// Fluxo:
    ///  1. Mostra info imutável: TRAB, DATA, FAZMOV, DIA1..DIA7 (só leitura)
    ///  2. Combo "Novo CODSER" populado com SERVIC vigente
    ///  3. QUANT e OBS editáveis (sempre livres)
    ///  4. OK valida via ServicoGratificacao — bloqueia se o novo serviço tem
    ///     gratificação vigente em GRATIF pra este setor (criaria divergência)
    ///  5. Retorna DialogResult.OK com NovoCodser/NovoNumMod/NovaQuant/NovaObs
    ///     preenchidos. Caller atualiza a DataRow e dispara save normal.
    ///
    /// Adicionado 2026-05-12.
    /// </summary>
    public class FrmRetificarProvisorio : Form
    {
        private readonly DataRow rowOrigem;

        // Resultado (válido só se DialogResult == OK)
        public string NovoCodser  { get; private set; } = "";
        public string NovoNumMod  { get; private set; } = "";
        public decimal NovaQuant  { get; private set; }
        public string NovaObs     { get; private set; } = "";

        // Controles
        private Label lblInfoFixo;
        private ComboBox cbServico;
        private NumericUpDown nudQuant;
        private TextBox txtObs;
        private Button btnOk;
        private Button btnCancelar;
        private Label lblStatus;

        public FrmRetificarProvisorio(DataRow rowCltPonto)
        {
            this.rowOrigem = rowCltPonto ?? throw new ArgumentNullException(nameof(rowCltPonto));
            InicializaUi();
            PreencheValoresIniciais();
            CarregaServicos();
        }

        private void InicializaUi()
        {
            this.Text = "Retificar lançamento provisório (943)";
            this.StartPosition = FormStartPosition.CenterParent;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.ClientSize = new Size(560, 400);

            // Painel topo: info imutável (só leitura)
            lblInfoFixo = new Label
            {
                Location = new Point(12, 12),
                Size = new Size(536, 110),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(8),
                Font = new Font(FontFamily.GenericMonospace, 9f),
                BackColor = SystemColors.Info
            };
            this.Controls.Add(lblInfoFixo);

            // Label novo CODSER
            var lblServ = new Label { Text = "Novo serviço:", Location = new Point(12, 145), AutoSize = true };
            this.Controls.Add(lblServ);

            cbServico = new ComboBox
            {
                Location = new Point(140, 142),
                Size = new Size(408, 24),
                DropDownStyle = ComboBoxStyle.DropDownList,
                DisplayMember = "Display",
                ValueMember = "Codser"
            };
            this.Controls.Add(cbServico);

            // QUANT
            var lblQuant = new Label { Text = "Quantidade:", Location = new Point(12, 185), AutoSize = true };
            this.Controls.Add(lblQuant);

            nudQuant = new NumericUpDown
            {
                Location = new Point(140, 182),
                Size = new Size(120, 24),
                DecimalPlaces = 4,
                Minimum = 0,
                Maximum = 999999
            };
            this.Controls.Add(nudQuant);

            // OBS
            var lblObs = new Label { Text = "Observação:", Location = new Point(12, 220), AutoSize = true };
            this.Controls.Add(lblObs);

            txtObs = new TextBox
            {
                Location = new Point(140, 217),
                Size = new Size(408, 60),
                Multiline = true,
                MaxLength = 200
            };
            this.Controls.Add(txtObs);

            // Status
            lblStatus = new Label
            {
                Location = new Point(12, 290),
                Size = new Size(536, 40),
                ForeColor = Color.DimGray,
                Text = "Ao confirmar, o sistema valida que o novo serviço NÃO tem gratificação " +
                       "vigente — caso contrário, a retificação criaria divergência com a folha já paga."
            };
            this.Controls.Add(lblStatus);

            // Botões
            btnOk = new Button
            {
                Text = "&Confirmar retificação",
                Location = new Point(308, 345),
                Size = new Size(180, 32)
            };
            btnOk.Click += BtnOk_Click;
            this.Controls.Add(btnOk);

            btnCancelar = new Button
            {
                Text = "&Cancelar",
                Location = new Point(498, 345),
                Size = new Size(80, 32),
                DialogResult = DialogResult.Cancel
            };
            this.Controls.Add(btnCancelar);

            this.AcceptButton = btnOk;
            this.CancelButton = btnCancelar;
        }

        private void PreencheValoresIniciais()
        {
            string trab    = (rowOrigem["TRAB"]    ?? "").ToString().Trim();
            string fazmov  = (rowOrigem["FAZMOV"]  ?? "").ToString().Trim();
            string setor   = (rowOrigem["SETOR"]   ?? "").ToString().Trim();
            string data    = "";
            try { if (!rowOrigem.IsNull("DATA")) data = Convert.ToDateTime(rowOrigem["DATA"]).ToString("dd/MM/yyyy"); }
            catch (Exception) { }

            string dias = "";
            for (int n = 1; n <= 7; n++)
            {
                string col = "DIA" + n;
                string v = (rowOrigem[col] ?? "").ToString().Trim();
                dias += string.Format("D{0}=[{1,2}] ", n, v);
            }

            lblInfoFixo.Text =
                $"TRAB:   {trab}\r\n" +
                $"DATA:   {data}    SETOR: {setor}    GLEBA: {fazmov}\r\n" +
                $"DIAS:   {dias}\r\n" +
                $"CODSER atual: 943 (provisório)";

            // QUANT/OBS começam com valores atuais
            try
            {
                if (!rowOrigem.IsNull("QUANT")) nudQuant.Value = Convert.ToDecimal(rowOrigem["QUANT"]);
            }
            catch (Exception) { nudQuant.Value = 0; }
            txtObs.Text = (rowOrigem["OBS"] ?? "").ToString();
        }

        private void CarregaServicos()
        {
            // Lista SERVIC da TabelasIniciais — já carregada no startup do app.
            DataSet ds = TabelasIniciais.DsTabelasInciais();
            if (ds == null || !ds.Tables.Contains("SERVICOMODELO"))
            {
                cbServico.Enabled = false;
                lblStatus.Text = "Erro: tabela SERVICOMODELO não disponível. Reabra o app.";
                lblStatus.ForeColor = Color.DarkRed;
                btnOk.Enabled = false;
                return;
            }

            // SERVICOMODELO traz NUM, SEQ, COD (modelo), DESCRICAO, CODSERV, DESCSERV, TIPO, PROD
            // Ordena por DESCSERV pra UX. Exclui CODSERV='943' (já é o atual).
            var rows = ds.Tables["SERVICOMODELO"].AsEnumerable()
                .Where(r => (r["CODSERV"] ?? "").ToString().Trim() != "943"
                         && (r["DESCSERV"] ?? "").ToString().Trim() != "")
                .Select(r => new ServicoItem
                {
                    Codser = (r["CODSERV"] ?? "").ToString().Trim(),
                    NumMod = (r["COD"] ?? "").ToString().Trim(),  // modelo
                    Display = string.Format("[{0,-3}] {1} (mod {2})",
                                            (r["CODSERV"] ?? "").ToString().Trim(),
                                            (r["DESCSERV"] ?? "").ToString().Trim(),
                                            (r["COD"] ?? "").ToString().Trim())
                })
                .OrderBy(s => s.Display)
                .ToList();

            cbServico.DataSource = rows;
            cbServico.SelectedIndex = -1;
        }

        private async void BtnOk_Click(object sender, EventArgs e)
        {
            if (cbServico.SelectedItem == null)
            {
                MessageBox.Show("Selecione o novo serviço.", "Retificar",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ServicoItem item = (ServicoItem)cbServico.SelectedItem;
            string novoCodser = item.Codser;
            string novoNumMod = item.NumMod;
            string setor = (rowOrigem["SETOR"] ?? "").ToString().Trim();
            DateTime data = Convert.ToDateTime(rowOrigem["DATA"]);
            DateTime mesRef = new DateTime(data.Year, data.Month, 1);

            // Pré-validação ANTES de fechar diálogo: se mês fechado, checar gratificação.
            // Se mês aberto, pode trocar livremente (sem prêmio importa só pós-pagamento).
            btnOk.Enabled = false;
            UseWaitCursor = true;
            try
            {
                if (RegrasMesFechado.MesFechado(data))
                {
                    bool atualTemPremio = await ServicoGratificacao.ServicoTemPremio("943", setor, mesRef);
                    bool novoTemPremio = await ServicoGratificacao.ServicoTemPremio(novoCodser, setor, mesRef);
                    if (atualTemPremio || novoTemPremio)
                    {
                        MessageBox.Show(
                            "Não é possível retificar para o serviço '" + novoCodser + "' " +
                            "em mês fechado: tem gratificação vigente em GRATIF " +
                            "(setor " + setor + ", mês " + mesRef.ToString("MM/yyyy") + ").\n\n" +
                            "Trocar criaria divergência com a folha já paga. " +
                            "Solicite ao escritório.",
                            "Retificar — bloqueado",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }
            }
            finally
            {
                btnOk.Enabled = true;
                UseWaitCursor = false;
            }

            // Confirma e fecha — caller aplica os valores na DataRow e dispara save.
            NovoCodser = novoCodser;
            NovoNumMod = novoNumMod;
            NovaQuant  = nudQuant.Value;
            NovaObs    = (txtObs.Text ?? "").Trim();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // DTO interno do combo
        private class ServicoItem
        {
            public string Codser { get; set; }
            public string NumMod { get; set; }
            public string Display { get; set; }
        }
    }
}
