using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    /// <summary>
    /// Cadastro de códigos de apontamento (tabela CLTCODIG).
    /// Equivalente C# do <c>fcltCodig.pas</c> Delphi original.
    /// Adicionado 2026-05-08 (F7).
    /// </summary>
    /// <remarks>
    /// Schema CLTCODIG: Id, INDCOD nchar(2), DESCCOD nchar(21),
    /// FATOR, HORAS, X_HORAS, NOTURNO, H_NORMAL (numéricos), COMPATI nchar(1).
    /// Edição inline no DataGridView (toggle ReadOnly).
    /// </remarks>
    public partial class FrmCltCodig : Form
    {
        private DataTable dtCodigos;
        private bool emEdicao = false;

        // Estado de autenticação de supervisor (sticky por sessão da janela).
        // Pedido na primeira ação de edição; mantido até fechar o form.
        // Adicionado 2026-05-08.
        private bool _supervisorAutenticado = false;
        private string _supervisorUsuario;

        public FrmCltCodig()
        {
            InitializeComponent();
            this.Load += FrmCltCodig_Load;
            dgvCodigos.SelectionChanged += (s, e) => AtualizarBotoes();

            // 2026-05-08: força UPPERCASE em INDCOD ao digitar.
            // CharacterCasing.Upper só afeta letras (a-z → A-Z); dígitos e símbolos
            // passam como digitados. Reset pra Normal nas outras colunas pra não
            // ficar "preso" em Upper depois de editar INDCOD.
            dgvCodigos.EditingControlShowing += (s, e) =>
            {
                TextBox tb = e.Control as TextBox;
                if (tb == null) return;
                bool ehIndcod = dgvCodigos.CurrentCell != null
                    && dgvCodigos.CurrentCell.OwningColumn != null
                    && dgvCodigos.CurrentCell.OwningColumn.Name == "INDCOD";
                tb.CharacterCasing = ehIndcod
                    ? CharacterCasing.Upper
                    : CharacterCasing.Normal;
            };
        }

        private async void FrmCltCodig_Load(object sender, EventArgs e)
        {
            await CarregarCodigos();
        }

        private async Task CarregarCodigos()
        {
            UseWaitCursor = true;
            try
            {
                string query =
                    "SELECT Id, INDCOD, DESCCOD, FATOR, HORAS, X_HORAS, NOTURNO, H_NORMAL, COMPATI " +
                    "FROM CLTCODIG ORDER BY INDCOD";
                DataSet ds = await ApiServices.Api_QueryMulti(new List<string> { query });
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0] == null)
                {
                    MessageBox.Show("Falha ao carregar CLTCODIG do servidor.",
                        "FrmCltCodig", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                dtCodigos = ds.Tables[0];
                // BUG FIX 2026-05-08: rows vêm da API em estado Added (não Unchanged).
                // Sem AcceptChanges, o método Salvar coletaria TODAS as rows como
                // "novas" e mandaria INSERT pra cada uma — duplicaria toda a tabela.
                dtCodigos.AcceptChanges();
                dgvCodigos.DataSource = dtCodigos;
                ConfigurarColunas();
                AtualizarBotoes();
                lblStatus.Text = dtCodigos.Rows.Count + " código(s) carregado(s).";
            }
            finally
            {
                UseWaitCursor = false;
            }
        }

        private void ConfigurarColunas()
        {
            // Id escondido (auto-increment do banco)
            if (dgvCodigos.Columns["Id"] != null)
            {
                dgvCodigos.Columns["Id"].Visible = false;
                dgvCodigos.Columns["Id"].ReadOnly = true;
            }
            ConfigCol("INDCOD", "Código", 70);
            ConfigCol("DESCCOD", "Descrição", 230);
            // Tamanhos máximos no input pra bater com o schema (INDCOD nchar(2), DESCCOD nchar(21))
            // — impede o operador digitar além do que o banco aceita.
            if (dgvCodigos.Columns["INDCOD"] is DataGridViewTextBoxColumn colInd)
                colInd.MaxInputLength = 2;
            if (dgvCodigos.Columns["DESCCOD"] is DataGridViewTextBoxColumn colDesc)
                colDesc.MaxInputLength = 21;
            ConfigCol("FATOR", "Fator", 90, "0.0000");
            ConfigCol("HORAS", "H. Efetivas", 90, "0.0000");
            ConfigCol("X_HORAS", "H. Extras", 80, "0.##");
            ConfigCol("NOTURNO", "H. Noturno", 80, "0.##");
            ConfigCol("H_NORMAL", "H. Feriado", 80, "0.##");
            ConfigCol("COMPATI", "Comp.", 60);
            dgvCodigos.ReadOnly = !emEdicao;
        }

        private void ConfigCol(string nome, string header, int width, string format = null)
        {
            if (dgvCodigos.Columns[nome] == null) return;
            dgvCodigos.Columns[nome].HeaderText = header;
            dgvCodigos.Columns[nome].Width = width;
            if (!string.IsNullOrEmpty(format))
                dgvCodigos.Columns[nome].DefaultCellStyle.Format = format;
        }

        private void AtualizarBotoes()
        {
            tsSalvar.Enabled = emEdicao;
            tsCancelar.Enabled = emEdicao;
            tsNovo.Enabled = !emEdicao;
            tsEdit.Enabled = !emEdicao && dgvCodigos.CurrentRow != null;
            tsDelete.Enabled = !emEdicao && dgvCodigos.CurrentRow != null;
            tsRefresh.Enabled = !emEdicao;
        }

        private void tsRefresh_Click(object sender, EventArgs e)
        {
            _ = CarregarCodigos();
        }

        /// <summary>
        /// Garante que há supervisor autenticado nesta sessão antes de operação de edição.
        /// Se ainda não há, abre FrmPedirSenha (até 3 tentativas).
        /// Retorna true se autenticado, false se cancelou ou esgotou tentativas.
        /// Adicionado 2026-05-08.
        /// </summary>
        private async Task<bool> GarantirSupervisorAsync()
        {
            if (_supervisorAutenticado) return true;

            const int MAX_TENTATIVAS = 3;
            for (int tentativa = 1; tentativa <= MAX_TENTATIVAS; tentativa++)
            {
                string msg = "Para alterar Códigos de Apontamento, autentique-se com usuário e senha de supervisor.";
                if (tentativa > 1)
                    msg = "Tentativa " + tentativa + " de " + MAX_TENTATIVAS + ".\n\n" + msg;

                FrmPedirSenha dlg = new FrmPedirSenha(msg);
                if (dlg.ShowDialog(this) != DialogResult.OK)
                    return false; // cancelou

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

        private async void tsNovo_Click(object sender, EventArgs e)
        {
            if (dtCodigos == null) return;
            if (!await GarantirSupervisorAsync()) return;
            DataRow nova = dtCodigos.NewRow();
            nova["INDCOD"] = "";
            nova["DESCCOD"] = "";
            nova["FATOR"] = 0m;
            nova["HORAS"] = 0m;
            nova["X_HORAS"] = 0m;
            nova["NOTURNO"] = 0m;
            nova["H_NORMAL"] = 0m;
            nova["COMPATI"] = " ";
            dtCodigos.Rows.Add(nova);
            EntrarEmEdicao();
            int idx = dtCodigos.DefaultView.Count - 1;
            if (idx >= 0)
            {
                dgvCodigos.CurrentCell = dgvCodigos.Rows[idx].Cells["INDCOD"];
                dgvCodigos.BeginEdit(true);
            }
        }

        private async void tsEdit_Click(object sender, EventArgs e)
        {
            if (dgvCodigos.CurrentRow == null) return;
            if (!await GarantirSupervisorAsync()) return;
            EntrarEmEdicao();
            dgvCodigos.BeginEdit(true);
        }

        private void EntrarEmEdicao()
        {
            emEdicao = true;
            dgvCodigos.ReadOnly = false;
            // Id permanece readonly (não é editável manualmente)
            if (dgvCodigos.Columns["Id"] != null)
                dgvCodigos.Columns["Id"].ReadOnly = true;
            AtualizarBotoes();
            lblStatus.Text = "Em edição — Salvar ou Cancelar para continuar.";
        }

        private async void tsSalvar_Click(object sender, EventArgs e)
        {
            // Salvar exige supervisor (mesmo se editou via Novo/Editar que já autenticaram —
            // protege caso _supervisorAutenticado seja resetado por algum motivo futuro).
            if (!await GarantirSupervisorAsync()) return;

            // BUG FIX 2026-05-08: EndEdit() sozinho não basta — em DataGridView ligado
            // a DataTable, o valor digitado na célula corrente só é persistido na DataRow
            // depois de CommitEdit + EndCurrentEdit do CurrencyManager. Sem isso,
            // a validação que vem a seguir lia INDCOD/DESCCOD vazios mesmo o operador
            // tendo preenchido (e dava "Código e Descrição são obrigatórios").
            try
            {
                dgvCodigos.CommitEdit(DataGridViewDataErrorContexts.Commit);
                if (BindingContext != null && BindingContext[dtCodigos] != null)
                    ((CurrencyManager)BindingContext[dtCodigos]).EndCurrentEdit();
                dgvCodigos.EndEdit();
            }
            catch (Exception) { /* não bloqueia o save por falha de commit */ }

            DataTable modificadas = dtCodigos.GetChanges(DataRowState.Added | DataRowState.Modified);
            if (modificadas == null || modificadas.Rows.Count == 0)
            {
                SairEdicao("Nada a salvar.");
                return;
            }

            // Validação 1: INDCOD + DESCCOD obrigatórios + tamanhos máximos
            // (igual Delphi BeforePost, mais restrição de tamanho do schema).
            // Schema: INDCOD nchar(2), DESCCOD nchar(21).
            foreach (DataRow r in modificadas.Rows)
            {
                string ind = (r["INDCOD"] ?? "").ToString().Trim();
                string desc = (r["DESCCOD"] ?? "").ToString().Trim();
                if (ind == "" || desc == "")
                {
                    MessageBox.Show("Código e Descrição são obrigatórios.",
                        "FrmCltCodig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (ind.Length > 2)
                {
                    MessageBox.Show("Código '" + ind + "' tem mais de 2 caracteres (máximo permitido).",
                        "FrmCltCodig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                if (desc.Length > 21)
                {
                    MessageBox.Show("Descrição '" + desc + "' tem mais de 21 caracteres (máximo permitido).",
                        "FrmCltCodig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            // Validação 2: duplicidade de INDCOD (entre rows da própria DataTable).
            // Evita que servidor rejeite por unique index e pra dar mensagem amigável.
            // Comparar com TRIM em ambos os lados (banco armazena padded com espaços).
            foreach (DataRow r in modificadas.Rows)
            {
                string ind = r["INDCOD"].ToString().Trim();
                int duplicatasNaTabela = 0;
                foreach (DataRow outra in dtCodigos.Rows)
                {
                    if (outra.RowState == DataRowState.Detached || outra.RowState == DataRowState.Deleted) continue;
                    if (outra["INDCOD"].ToString().Trim() == ind) duplicatasNaTabela++;
                }
                if (duplicatasNaTabela > 1)
                {
                    MessageBox.Show("Já existe um código '" + ind + "'. Códigos não podem se repetir.",
                        "FrmCltCodig", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            UseWaitCursor = true;
            int salvas = 0;
            int total = 0;
            try
            {
                // itera sobre as rows REAIS no dtCodigos (mantendo RowState pra OpereRegistroServidorAsync saber se INSERT ou UPDATE)
                List<DataRow> aSalvar = new List<DataRow>();
                foreach (DataRow r in dtCodigos.Rows)
                {
                    if (r.RowState == DataRowState.Added || r.RowState == DataRowState.Modified)
                        aSalvar.Add(r);
                }
                total = aSalvar.Count;

                bool ok = true;
                foreach (DataRow r in aSalvar)
                {
                    DataRowState estado = r.RowState;

                    // F9 audit: capturar snapshot ANTES (só faz sentido para UPDATE)
                    CltCodigSnapshot antes = null;
                    if (estado == DataRowState.Modified)
                        antes = CltCodigSnapshot.FromRow(r, DataRowVersion.Original);

                    bool resp = await Prepara_Sql.OpereRegistroServidorAsync(r, estado, "CLTCODIG");
                    if (!resp) { ok = false; break; }
                    r.AcceptChanges();
                    salvas++;

                    // F9 audit: gravar entrada (Insert: só Depois; Update: Antes+Depois).
                    // Falha de audit é silenciosa — não bloqueia operação.
                    CltCodigSnapshot depois = CltCodigSnapshot.FromRow(r, DataRowVersion.Current);
                    string indcod = (r["INDCOD"] ?? "").ToString();
                    int idLinha = 0;
                    try { idLinha = Convert.ToInt32(r["Id"]); } catch (Exception) { }
                    string op = (estado == DataRowState.Added) ? "I" : "U";
                    _ = AuditService.RegistrarCltCodigAuditAsync(
                        op, _supervisorUsuario ?? "?", idLinha, indcod, antes, depois);
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
            dgvCodigos.ReadOnly = true;
            AtualizarBotoes();
            lblStatus.Text = statusMsg;
        }

        private void tsCancelar_Click(object sender, EventArgs e)
        {
            try { dgvCodigos.CancelEdit(); } catch (Exception) { }
            if (dtCodigos != null)
                dtCodigos.RejectChanges();
            SairEdicao("Edição cancelada.");
        }

        private async void tsDelete_Click(object sender, EventArgs e)
        {
            if (dgvCodigos.CurrentRow == null) return;
            if (!await GarantirSupervisorAsync()) return;
            DataRowView rView = dgvCodigos.CurrentRow.DataBoundItem as DataRowView;
            if (rView == null) return;
            DataRow row = rView.Row;
            string indcod = (row["INDCOD"] ?? "").ToString().Trim();
            string desccod = (row["DESCCOD"] ?? "").ToString().Trim();

            DialogResult resp = MessageBox.Show(
                "Deletar o código '" + indcod + "' (" + desccod + ")?",
                "Deletar Código",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            UseWaitCursor = true;
            try
            {
                // F9 audit: snapshot ANTES (Current ainda válido — row não foi removida)
                CltCodigSnapshot antes = CltCodigSnapshot.FromRow(row, DataRowVersion.Current);
                int idLinha = 0;
                try { idLinha = Convert.ToInt32(row["Id"]); } catch (Exception) { }

                bool ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(row, "CLTCODIG");
                if (ok)
                {
                    dtCodigos.Rows.Remove(row);
                    dtCodigos.AcceptChanges();
                    AtualizarBotoes();
                    lblStatus.Text = "Código '" + indcod + "' deletado.";

                    // F9 audit: registra delete (só Antes preenchido)
                    _ = AuditService.RegistrarCltCodigAuditAsync(
                        "D", _supervisorUsuario ?? "?", idLinha, indcod, antes, null);
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
                    "FrmCltCodig", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (r != DialogResult.Yes) return;
            }
            this.Close();
        }
    }
}
