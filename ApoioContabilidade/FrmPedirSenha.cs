using System;
using System.Windows.Forms;

namespace ApoioContabilidade
{
    /// <summary>
    /// Popup modal genérico que pede usuário + senha de supervisor.
    /// NÃO valida — só captura. Quem chamar deve validar via AutenticacaoService.
    /// Equivalente C# do <c>fSenha.pas</c> Delphi original.
    /// Adicionado 2026-05-08.
    /// </summary>
    public partial class FrmPedirSenha : Form
    {
        public string Usuario { get { return txUsuario.Text.Trim(); } }
        public string Senha { get { return txSenha.Text; } }

        public FrmPedirSenha(string mensagem = "Autentique-se com usuário e senha de supervisor:")
        {
            InitializeComponent();
            lblMensagem.Text = mensagem;
            this.AcceptButton = btnOk;
            this.CancelButton = btnCancelar;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txUsuario.Text) || string.IsNullOrEmpty(txSenha.Text))
            {
                MessageBox.Show("Informe usuário e senha.", "FrmPedirSenha",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                this.DialogResult = DialogResult.None;
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        /// <summary>
        /// Abre FrmWinConfigura inline durante o login pra permitir reconfigurar
        /// CONTAB/SERVIDOR_IP quando o servidor configurado é o errado ou está
        /// inacessível. Após salvar, fecha o login com DialogResult.Retry — o
        /// LoginInicial reentra do começo (retesta servidores, reseta tentativas).
        ///
        /// Convenção 2026-05-12: aviso de admin (sem bloqueio por senha — qualquer
        /// um pode reconfigurar a máquina local). Risco aceito: gerente em fazenda
        /// remota pode precisar sair de enrascada sem o admin disponível.
        /// </summary>
        private void btnConfigurar_Click(object sender, EventArgs e)
        {
            var resp = MessageBox.Show(
                "Esta é uma operação de administrador. Reconfigurar o caminho do " +
                "CONTAB ou o IP do servidor pode quebrar o acesso aos dados.\n\n" +
                "Tem certeza de que deseja continuar?",
                "Configurar caminhos",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                MessageBoxDefaultButton.Button2);
            if (resp != DialogResult.Yes) return;

            // Modo "só servidor": CONTAB é local e irrelevante pra autenticação.
            using (FrmWinConfigura cfg = new FrmWinConfigura(soServidor: true))
            {
                if (cfg.ShowDialog(this) == DialogResult.OK)
                {
                    // Sinaliza ao LoginInicial pra retestar servidores e reentrar.
                    this.DialogResult = DialogResult.Retry;
                    this.Close();
                }
            }
        }
    }
}
