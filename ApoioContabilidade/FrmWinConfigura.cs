using ApoioContabilidade.Core;
using ClassConexao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApoioContabilidade
{
    public partial class FrmWinConfigura : Form
    {
        public string path = "";
        public string servidor = "";
        public FrmWinConfigura()
        {
            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + "ConfigPath.txt"))
            {
                ListaCaminhos.Paths = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + "ConfigPath.txt");
                path = ListaCaminhos.GetPath("CONTAB");
                servidor = ListaCaminhos.GetPath("SERVIDOR_IP");
            }




            InitializeComponent();
            txPath.Text = path;
            txServidor.Text = servidor;
            

        }
        private void BtnOK_Click(object sender, EventArgs e)
        {
            path = txPath.Text.Trim();
            if (ListaCaminhos.Paths.ContainsKey("CONTAB"))
            { ListaCaminhos.Paths["CONTAB"] = path; }
            else
            {
                ListaCaminhos.Paths.Add("CONTAB", path);
            }
            servidor = txServidor.Text.Trim();
            if (ListaCaminhos.Paths.ContainsKey("SERVIDOR_IP"))
            { ListaCaminhos.Paths["SERVIDOR_IP"] = servidor; }
            else
            {
                ListaCaminhos.Paths.Add("SERVIDOR_IP", servidor);
            }
            if ((path == "") && (servidor == "")) {
                MessageBox.Show("Sem Informação na Configuração Aplicativo não Irá funcionar");
            }


            AcessosStream.WriteLista(Environment.SpecialFolder.LocalApplicationData + "ConfigPath.txt", ListaCaminhos.Paths);

        }

        private void TextBoxPath_Validating(object sender, CancelEventArgs e)
        {
            string path = (sender as TextBox).Text;
            if (path.Trim() == "") return;
            bool Sucesso = Directory.Exists(path); 
            if (!Sucesso)
            {
                path = "";
                e.Cancel = true;
                MessageBox.Show("Caminho Não Existes");
                
            }
            
        }
        private void TextBoxServidor_Validating(object sender, CancelEventArgs e)
        {
            servidor = (sender as TextBox).Text;
            if (servidor.Trim() == "") { return; }
            bool Sucesso = TestaConexao.CheckForInternetConnection(servidor);
            if (!Sucesso)
            {
                servidor = "";
                e.Cancel = true;
                MessageBox.Show("Servidor Invalido");
            }
        }



    }
}
