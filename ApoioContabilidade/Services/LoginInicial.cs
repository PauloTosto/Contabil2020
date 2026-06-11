using ApoioContabilidade.Core;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Services
{
    /// <summary>
    /// Tela de login obrigatório no startup. Captura usuário + senha via
    /// FrmPedirSenha, valida via AutenticacaoService.AutenticarAsync e popula
    /// SessaoAtual com o escopo (lista de setores) do usuário.
    ///
    /// Adicionado 2026-05-11 (passo 2 da migração "gerentes acessam direto"):
    /// sem isso, SessaoAtual fica vazia e todo usuário vê tudo — o filtro de
    /// USUARIO_SETORES nunca dispara.
    /// </summary>
    /// <remarks>
    /// Política:
    ///  - 3 tentativas; depois disso o caller deve encerrar o app.
    ///  - Cancelar (X ou botão Cancelar) = retorna false; caller deve encerrar.
    ///  - Sucesso = popula SessaoAtual e retorna true.
    ///  - "Configurar caminhos..." (2026-05-12): fecha o diálogo com Retry, e o
    ///    loop reentra do começo — chama <c>reverificarServidores</c> callback
    ///    (se fornecida) pra repopular sucesso/sucessoServidor do caller, e
    ///    reseta a contagem de tentativas (operador acabou de mudar config, é
    ///    um novo começo).
    ///
    /// Nada de "lembrar usuário" por enquanto. Se virar fricção pro escritório,
    /// adicionar persistência criptografada (DPAPI) depois.
    /// </remarks>
    public static class LoginInicial
    {
        private const int MAX_TENTATIVAS = 3;

        /// <summary>
        /// Pede credencial e popula SessaoAtual em caso de sucesso.
        /// Retorna true se autenticou, false se o usuário cancelou ou estourou tentativas.
        /// </summary>
        /// <param name="owner">Owner do diálogo modal (geralmente o FrmMenu).</param>
        /// <param name="reverificarServidores">Callback opcional chamada após o
        /// usuário clicar em "Configurar caminhos..." e salvar nova config.
        /// Deve retestar CONTAB/SERVIDOR_IP e retornar true se servidor agora
        /// está acessível. Se retornar false, oferece reconfig de novo (loop).
        /// </param>
        public static async Task<bool> ExecutarAsync(
            IWin32Window owner = null,
            Func<bool> reverificarServidores = null)
        {
            int tentativa = 1;
            while (tentativa <= MAX_TENTATIVAS)
            {
                string mensagem = tentativa == 1
                    ? "Login no sistema:"
                    : $"Falha na autenticação. Tentativa {tentativa} de {MAX_TENTATIVAS}:";

                using (FrmPedirSenha dlg = new FrmPedirSenha(mensagem))
                {
                    DialogResult dr = owner == null ? dlg.ShowDialog() : dlg.ShowDialog(owner);

                    if (dr == DialogResult.Retry)
                    {
                        // Usuário reconfigurou via "Configurar caminhos..." — reentra do começo.
                        // Reseta tentativas (é um novo começo, com servidor potencialmente diferente).
                        if (reverificarServidores != null)
                        {
                            bool ok = false;
                            try { ok = reverificarServidores(); } catch (Exception) { ok = false; }
                            if (!ok)
                            {
                                MessageBox.Show(
                                    "Servidor ainda não está acessível após reconfigurar. " +
                                    "Verifique o endereço ou a conexão de rede e tente de novo.",
                                    "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                // Permite novo Retry: não incrementa tentativa, não falha definitivo.
                                continue;
                            }
                        }
                        tentativa = 1;
                        continue;
                    }

                    if (dr != DialogResult.OK)
                    {
                        // Cancelar / X → não tenta de novo.
                        return false;
                    }

                    AuthResult auth = await AutenticacaoService.AutenticarAsync(dlg.Usuario, dlg.Senha);
                    if (auth.Sucesso)
                    {
                        SessaoAtual.Usuario = auth.Usuario;
                        SessaoAtual.Privilegios = auth.Privilegios;
                        SessaoAtual.Setores = auth.Setores ?? new System.Collections.Generic.List<string>();
                        return true;
                    }

                    MessageBox.Show(auth.Mensagem ?? "Falha ao autenticar.",
                        "Login", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tentativa++;
                }
            }
            // Estourou tentativas.
            return false;
        }
    }
}
