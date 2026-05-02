using ApoioContabilidade.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace ApoioContabilidade
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Liga o reporter de erros do ServicosApi à UI WinForms.
            // ServicosApi nao depende mais diretamente de System.Windows.Forms.
            ApiErrorReporter.OnError = (mensagem, ex) => MessageBox.Show(mensagem);

            Application.Run(new FrmMenu());
        }
    }
}
