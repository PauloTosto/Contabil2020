using System;

namespace ApoioContabilidade.Services
{
    /// <summary>
    /// Indirecao para reportar erros da camada API sem que ServicosApi importe System.Windows.Forms.
    ///
    /// Em runtime de produção (WinForms): Program.Main registra um handler que chama MessageBox.Show.
    /// Em testes/headless: registrar outro handler (log, swallow, throw, etc) ou deixar null
    /// (qualquer chamada vira no-op via ?.Invoke).
    ///
    /// Trade-off conhecido: usa estado global static. Para um app WinForms single-instance isso
    /// é suficiente. Se o projeto migrar para DI, substituir por interface injetada.
    /// </summary>
    public static class ApiErrorReporter
    {
        /// <summary>
        /// Acao a ser invocada quando ServicosApi encontra um erro.
        /// Parametros: (mensagem amigavel, excecao opcional ou null).
        /// </summary>
        public static Action<string, Exception> OnError;

        /// <summary>
        /// Atalho para chamadas sem excecao.
        /// </summary>
        public static void Report(string mensagem)
        {
            OnError?.Invoke(mensagem, null);
        }

        /// <summary>
        /// Atalho para chamadas com excecao.
        /// </summary>
        public static void Report(string mensagem, Exception ex)
        {
            OnError?.Invoke(mensagem, ex);
        }
    }
}
