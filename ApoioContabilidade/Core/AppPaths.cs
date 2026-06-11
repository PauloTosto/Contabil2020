using System;
using System.IO;

namespace ApoioContabilidade.Core
{
    /// <summary>
    /// Centraliza o caminho dos arquivos de config do app dentro do LocalApplicationData
    /// do usuário (ex: C:\Users\&lt;usuario&gt;\AppData\Local\).
    /// </summary>
    /// <remarks>
    /// Bug fix B8 (2026-05-08): o código original concatenava
    /// <c>Environment.SpecialFolder.LocalApplicationData + "Arquivo.txt"</c>
    /// — o operador <c>+</c> chama <c>ToString()</c> do enum e devolve a string
    /// <c>"LocalApplicationData"</c>. O resultado era um caminho relativo
    /// (<c>"LocalApplicationDataArquivo.txt"</c>) resolvido contra o CWD do processo.
    /// Quando o app rodava com CWD = <c>C:\WINDOWS\System32</c> (ex: "Run as
    /// administrator" pelo menu), gravava lá e dava <c>UnauthorizedAccessException</c>.
    /// Agora resolve para o caminho real do AppData do usuário, independente de CWD.
    /// </remarks>
    public static class AppPaths
    {
        private static string LocalAppDataDir =>
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        /// <summary>
        /// Retorna o caminho absoluto para um arquivo de config dentro do
        /// LocalApplicationData do usuário (sem subpasta — preserva o
        /// comportamento histórico do app, que gravava direto na raiz).
        /// </summary>
        public static string GetConfigPath(string fileName)
        {
            return Path.Combine(LocalAppDataDir, fileName);
        }
    }
}
