using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using System.IO;


using ApoioContabilidade.Core;

namespace ApoioContabilidade
{
    public partial class WinFrmConfigura : Form
    {
        public string servidor = "";
        public Dictionary<string, string> ListaCaminhos; 
        public WinFrmConfigura()
        {
            // B8 fix 2026-05-08: AppPaths.GetConfigPath em vez de concat de enum.
            if (File.Exists(AppPaths.GetConfigPath("ConfigServidor.txt")))
            {
                ListaCaminhos = AcessosStream.LeiaLista(AppPaths.GetConfigPath("ConfigServidor.txt"));
                servidor = ListaCaminhos["SERVIDOR_IP"];

            }


            
             
            InitializeComponent();
            textBoxServidor.Text = servidor; 


        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            
            ListaCaminhos["SERVIDOR_IP"] = servidor;
            
            AcessosStream.WriteLista(AppPaths.GetConfigPath("ConfigServidor.txt"), ListaCaminhos);
                
        }




        
        private void TextBoxServidor_Validating(object sender, CancelEventArgs e)
        {
            servidor = (sender as TextBox).Text;
            if (servidor == "") { return; }
             bool Sucesso = TestaConexao.CheckForInternetConnection(servidor);
            if (!Sucesso)
            {
                servidor = "";
                e.Cancel = true;
                MessageBox.Show("Servidor Invalido");
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // WinFrmConfigura
            // 
            this.ClientSize = new System.Drawing.Size(489, 253);
            this.Name = "WinFrmConfigura";
            this.ResumeLayout(false);

        }
    }


}
