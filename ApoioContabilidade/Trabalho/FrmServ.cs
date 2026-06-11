using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    /// <summary>
    /// Cadastro de serviços (tabela SERVIC).
    /// Equivalente C# do <c>FrmServ</c> Delphi (Gerencial/FServ.pas), versão simplificada.
    /// Pode ser aberto standalone ou a partir do FrmCltPonto via tsServico
    /// (passando o NUM_MOD da row corrente — destaca os serviços do modelo).
    /// Adicionado 2026-05-08.
    /// </summary>
    /// <remarks>
    /// Schema SERVIC: Id, COD nchar(4), DESCRI nvarchar(25), UNID nchar(15),
    /// TIPO nchar(1), INDICE numeric, APAGADO nchar(1), NINDICE numeric.
    ///
    /// Edição inline no DataGridView (toggle ReadOnly), padrão idêntico ao FrmCltCodig.
    /// Auth de supervisor (mesma regra: AcessoLivre/priv=1 não pode editar).
    /// Sem audit log nesta primeira versão.
    /// </remarks>
    public partial class FrmServ : Form
    {
        private DataTable dtServ;
        private bool emEdicao = false;

        private bool _supervisorAutenticado = false;
        private string _supervisorUsuario;

        /// <summary>
        /// Se preenchido (NUM do modelo), abre o form com filtro destacando
        /// os serviços que pertencem àquele modelo. Caller (FrmCltPonto) passa
        /// o NUM_MOD da row corrente.
        /// </summary>
        public string NumModeloFiltro { get; set; }

        public FrmServ()
        {
            InitializeComponent();
            this.Load += FrmServ_Load;
            dgvServ.SelectionChanged += (s, e) => AtualizarBotoes();

            // INDCOD-style: força UPPERCASE no COD ao digitar (igual FrmCltCodig).
            dgvServ.EditingControlShowing += (s, e) =>
            {
                TextBox tb = e.Control as TextBox;
                if (tb == null) return;
                bool ehCod = dgvServ.CurrentCell != null
                    && dgvServ.CurrentCell.OwningColumn != null
                    && dgvServ.CurrentCell.OwningColumn.Name == "COD";
                tb.CharacterCasing = ehCod
                    ? CharacterCasing.Upper
                    : CharacterCasing.Normal;
            };
        }

        private async void FrmServ_Load(object sender, EventArgs e)
        {
            await CarregarServicos();
        }

        private async Task CarregarServicos()
        {
            UseWaitCursor = true;
            try
            {
                string query =
                    "SELECT Id, COD, DESCRI, UNID, TIPO, INDICE, APAGADO, NINDICE " +
                    "FROM SERVIC ORDER BY COD";
                DataSet ds = await ApiServices.Api_QueryMulti(new List<string> { query });
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null)
                {
                    MessageBox.Show("Falha ao carregar SERVIC do servidor.",
                        "FrmServ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                dtServ = ds.Tables[0];
                // BUG fix idêntico ao do FrmCltCodig: rows vêm da API em estado Added;
                // sem AcceptChanges, Salvar coletaria todas como "novas" e duplicaria a tabela.
                dtServ.AcceptChanges();
                dgvServ.DataSource = dtServ;
                ConfigurarColunas();
                AtualizarBotoes();
                AtualizarStatus();
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ConfigurarColunas()
        {
            if (dgvServ.Columns["Id"] != null)
            {
                dgvServ.Columns["Id"].Visible = false;
                dgvServ.Columns["Id"].ReadOnly = true;
            }
            ConfigCol("COD", "Código", 80);
            ConfigCol("DESCRI", "Descrição", 280);
            ConfigCol("UNID", "Unidade", 100);
            ConfigCol("TIPO", "Tipo", 60);
            ConfigCol("INDICE", "Índice", 100, "0.0000");
            ConfigCol("NINDICE", "N.Índice", 100, "0.0000");
            ConfigCol("APAGADO", "Apag.", 60);

            if (dgvServ.Columns["COD"] is DataGridViewTextBoxColumn colCod)
                colCod.MaxInputLength = 4;
            if (dgvServ.Columns["DESCRI"] is DataGridViewTextBoxColumn colDesc)
                colDesc.MaxInputLength = 25;
            if (dgvServ.Columns["UNID"] is DataGridViewTextBoxColumn colUnid)
                colUnid.MaxInputLength = 15;
            if (dgvServ.Columns["TIPO"] is DataGridViewTextBoxColumn colTipo)
                colTipo.MaxInputLength = 1;
            if (dgvServ.Columns["APAGADO"] is DataGridViewTextBoxColumn colApag)
                colApag.MaxInputLength = 1;

            dgvServ.ReadOnly = !emEdicao;
        }

        private void ConfigCol(string nome, string header, int width, string format = null)
        {
            if (dgvServ.Columns[nome] == null) return;
            dgvServ.Columns[nome].HeaderText = header;
            dgvServ.Columns[nome].Width = width;
            if (!string.IsNullOrEmpty(format))
                dgvServ.Columns[nome].DefaultCellStyle.Format = format;
        }

        private void AtualizarBotoes()
        {
            tsSalvar.Enabled = emEdicao;
            tsCancelar.Enabled = emEdicao;
            tsNovo.Enabled = !emEdicao;
            tsEdit.Enabled = !emEdicao && dgvServ.CurrentRow != null;
            tsDelete.Enabled = !emEdicao && dgvServ.CurrentRow != null;
            tsRefresh.Enabled = !emEdicao;
        }

        private void AtualizarStatus()
        {
            string filtro = !string.IsNullOrEmpty(NumModeloFiltro)
                ? " (filtro: modelo=" + NumModeloFiltro.Trim() + ")"
                : "";
            lblStatus.Text = (dtServ != null ? dtServ.Rows.Count.ToString() : "0") +
                             " serviço(s) carregado(s)." + filtro;
        }

        private async Task<bool> GarantirSupervisorAsync()
        {
            if (_supervisorAutenticado) return true;

            const int MAX_TENTATIVAS = 3;
            for (int tentativa = 1; tentativa <= MAX_TENTATIVAS; tentativa++)
            {
                string msg = "Para alterar Serviços, autentique-se com usuário e senha de supervisor.";
                if (tentativa > 1)
                    msg = "Tentativa " + tentativa + " de " + MAX_TENTATIVAS + ".\n\n" + msg;

                FrmPedirSenha dlg = new FrmPedirSenha(msg);
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return false;

                AuthResult r;
                UseWaitCursor = true;
                try
                {
                    r = await AutenticacaoService.AutenticarSupervisorAsync(dlg.Usuario, dlg.Senha);
                }
                finally
                {
                    UseWaitCursor = false;
                }

                if (r.Sucesso)
                {
                    _supervisorAutenticado = true;
                    _supervisorUsuario = r.Usuario;
                    lblStatus.Text = "Autenticado como '" + _supervisorUsuario + "' (priv " + r.Privilegios + ").";
                    return true;
                }

                if (tentativa == MAX_TENTATIVAS)
                {
                    MessageBox.Show(r.Mensagem + "\n\nNúmero máximo de tentativas atingido.",
                        "Acesso Negado", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
                MessageBox.Show(r.Mensagem, "Acesso Negado",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            return false;
        }

        private void tsRefresh_Click(object sender, EventArgs e)
        {
            _ = CarregarServicos();
        }

        private async void tsNovo_Click(object sender, EventArgs e)
        {
            if (dtServ == null) return;
            if (!await GarantirSupervisorAsync()) return;

            DataRow nova = dtServ.NewRow();
            nova["COD"] = "";
            nova["DESCRI"] = "";
            nova["UNID"] = "";
            nova["TIPO"] = " ";
            nova["INDICE"] = 0m;
            nova["NINDICE"] = 0m;
            nova["APAGADO"] = " ";
            dtServ.Rows.Add(nova);
            EntrarEmEdicao();
            int idx = dtServ.DefaultView.Count - 1;
            if (idx >= 0)
            {
                dgvServ.CurrentCell = dgvServ.Rows[idx].Cells["COD"];
                dgvServ.BeginEdit(true);
            }
        }

        private async void tsEdit_Click(object sender, EventArgs e)
        {
            if (dgvServ.CurrentRow == null) return;
            if (!await GarantirSupervisorAsync()) return;
            EntrarEmEdicao();
            dgvServ.BeginEdit(true);
        }

        private void EntrarEmEdicao()
        {
            emEdicao = true;
            dgvServ.ReadOnly = false;
            if (dgvServ.Columns["Id"] != null)
                dgvServ.Columns["Id"].ReadOnly = true;
            AtualizarBotoes();
            lblStatus.Text = "Em edição — Salvar ou Cancelar para continuar.";
        }

        private async void tsSalvar_Click(object sender, EventArgs e)
        {
            if (!await GarantirSupervisorAsync()) return;

            // EndEdit + CommitEdit antes de validar (mesmo bug fix do FrmCltCodig).
            try
            {
                dgvServ.CommitEdit(DataGridViewDataErrorContexts.Commit);
                if (BindingContext != null && BindingContext[dtServ] != null)
                    ((CurrencyManager)BindingContext[dtServ]).EndCurrentEdit();
                dgvServ.EndEdit();
            }
            catch (Exception) { }

            DataTable modificadas = dtServ.GetChanges(DataRowState.Added | DataRowState.Modified);
            if (modificadas == null || modificadas.Rows.Count == 0)
            {
                SairEdicao("Nada a salvar.");
                return;
            }

            // Validação 1: COD + DESCRI obrigatórios + tamanhos máximos
            foreach (DataRow r in modificadas.Rows)
            {
                string cod = (r["COD"] ?? "").ToString().Trim();
                string desc = (r["DESCRI"] ?? "").ToString().Trim();
                if (cod == "" || desc == "")
                {
                    MessageBox.Show("Código e Descrição são obrigatórios.",
                        "FrmServ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (cod.Length > 4)
                {
                    MessageBox.Show("Código '" + cod + "' tem mais de 4 caracteres.",
                        "FrmServ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (desc.Length > 25)
                {
                    MessageBox.Show("Descrição '" + desc + "' tem mais de 25 caracteres.",
                        "FrmServ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Validação 2: duplicidade de COD entre rows.
            foreach (DataRow r in modificadas.Rows)
            {
                string cod = r["COD"].ToString().Trim();
                int duplicatas = 0;
                foreach (DataRow outra in dtServ.Rows)
                {
                    if (outra.RowState == DataRowState.Detached || outra.RowState == DataRowState.Deleted) continue;
                    if (outra["COD"].ToString().Trim() == cod) duplicatas++;
                }
                if (duplicatas > 1)
                {
                    MessageBox.Show("Já existe um serviço com código '" + cod + "'. Códigos não podem se repetir.",
                        "FrmServ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            UseWaitCursor = true;
            int salvas = 0;
            int total = 0;
            try
            {
                List<DataRow> aSalvar = new List<DataRow>();
                foreach (DataRow r in dtServ.Rows)
                {
                    if (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified)
                        aSalvar.Add(r);
                }
                total = aSalvar.Count;

                bool ok = true;
                foreach (DataRow r in aSalvar)
                {
                    DataRowState estado = r.RowState;
                    bool resp = await Prepara_Sql.OpereRegistroServidorAsync(r, estado, "SERVIC");
                    if (!resp) { ok = false; break; }
                    r.AcceptChanges();
                    salvas++;
                }

                if (ok)
                {
                    SairEdicao("Salvo com sucesso (" + salvas + " linha(s)).");
                }
                else
                {
                    string msg = (salvas == 0)
                        ? "Falha ao salvar — nenhuma linha foi gravada."
                        : "Falha ao salvar.\n\n" + salvas + " de " + total +
                          " linhas foram gravadas antes da falha.\n\n" +
                          "Recomendado: Cancelar e Atualizar para sincronizar.";
                    MessageBox.Show(msg, "ERRO AO SALVAR",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    lblStatus.Text = "Erro ao salvar — " + salvas + "/" + total + " gravadas.";
                }
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void SairEdicao(string statusMsg)
        {
            emEdicao = false;
            dgvServ.ReadOnly = true;
            AtualizarBotoes();
            lblStatus.Text = statusMsg;
        }

        private void tsCancelar_Click(object sender, EventArgs e)
        {
            try { dgvServ.CancelEdit(); } catch (Exception) { }
            if (dtServ != null) dtServ.RejectChanges();
            SairEdicao("Edição cancelada.");
        }

        private async void tsDelete_Click(object sender, EventArgs e)
        {
            if (dgvServ.CurrentRow == null) return;
            if (!await GarantirSupervisorAsync()) return;
            DataRowView rView = dgvServ.CurrentRow.DataBoundItem as DataRowView;
            if (rView == null) return;
            DataRow row = rView.Row;
            string cod = (row["COD"] ?? "").ToString().Trim();
            string desc = (row["DESCRI"] ?? "").ToString().Trim();

            DialogResult resp = MessageBox.Show(
                "Deletar o serviço '" + cod + "' (" + desc + ")?",
                "Deletar Serviço",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            UseWaitCursor = true;
            try
            {
                bool ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(row, "SERVIC");
                if (ok)
                {
                    dtServ.Rows.Remove(row);
                    dtServ.AcceptChanges();
                    AtualizarBotoes();
                    AtualizarStatus();
                }
                else
                {
                    MessageBox.Show("Falha ao deletar no servidor.", "ERRO",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void tsSair_Click(object sender, EventArgs e)
        {
            if (emEdicao)
            {
                DialogResult r = MessageBox.Show(
                    "Há edições não salvas. Sair mesmo assim?",
                    "FrmServ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r != DialogResult.Yes) return;
            }
            this.Close();
        }
    }
}
