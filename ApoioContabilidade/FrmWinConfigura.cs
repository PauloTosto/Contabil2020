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

        /// <summary>
        /// Quando true, esconde os controles relacionados a CONTAB (caminho DBF
        /// local) e edita SÓ o SERVIDOR_IP. Usado pelo fluxo pré-login do FrmMenu
        /// e pelo botão "Configurar caminhos..." do FrmPedirSenha: no contexto de
        /// autenticação, o CONTAB é irrelevante (USUARIOSMLSA é remoto). Evita
        /// que operador mexa em config local por engano.
        /// Adicionado 2026-05-12.
        /// </summary>
        private readonly bool soServidor;

        public FrmWinConfigura() : this(false) { }

        public FrmWinConfigura(bool soServidor)
        {
            this.soServidor = soServidor;

            // B8 fix 2026-05-08: AppPaths.GetConfigPath em vez de concat de enum.
            if (File.Exists(AppPaths.GetConfigPath("ConfigPath.txt")))
            {
                ListaCaminhos.Paths = AcessosStream.LeiaLista(AppPaths.GetConfigPath("ConfigPath.txt"));
                path = ListaCaminhos.GetPath("CONTAB");
                servidor = ListaCaminhos.GetPath("SERVIDOR_IP");
            }

            InitializeComponent();
            txPath.Text = path;
            txServidor.Text = servidor;

            if (soServidor)
            {
                // Esconde a parte de CONTAB. Move o painel do servidor pro topo
                // pra não ficar espaço em branco. Ajusta altura do form.
                Panel1.Visible = false;
                int yOrigPanel2 = panel2.Top;
                int deslocamento = yOrigPanel2 - Panel1.Top;
                panel2.Top = Panel1.Top;
                BtnOK.Top -= deslocamento;
                btnCancela.Top -= deslocamento;
                this.Height -= deslocamento;
                this.Text = "Configurar servidor";
            }
        }
        private void BtnOK_Click(object sender, EventArgs e)
        {
            // No modo "só servidor", preserva o CONTAB existente (não toca).
            // No modo completo, lê do TextBox.
            if (!soServidor)
            {
                path = txPath.Text.Trim();
                if (ListaCaminhos.Paths.ContainsKey("CONTAB"))
                { ListaCaminhos.Paths["CONTAB"] = path; }
                else
                {
                    ListaCaminhos.Paths.Add("CONTAB", path);
                }
            }

            servidor = txServidor.Text.Trim();
            if (ListaCaminhos.Paths.ContainsKey("SERVIDOR_IP"))
            { ListaCaminhos.Paths["SERVIDOR_IP"] = servidor; }
            else
            {
                ListaCaminhos.Paths.Add("SERVIDOR_IP", servidor);
            }

            // Aviso "vazio" só faz sentido no modo completo — no modo só-servidor,
            // o CONTAB pode estar vazio legitimamente e não é responsabilidade desse fluxo.
            if (!soServidor && (path == "") && (servidor == ""))
            {
                MessageBox.Show("Sem Informação na Configuração Aplicativo não Irá funcionar");
            }

            AcessosStream.WriteLista(AppPaths.GetConfigPath("ConfigPath.txt"), ListaCaminhos.Paths);
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
