using PrjApiParceiro_C.Comum;
using PrjApiParceiro_C.Core;
using PrjApiParceiro_C.Fiscais;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrjApiParceiro_C
{
    public partial class FrmPrincipal : Form
    {

        
        public Dictionary<string, string> ListaCaminhos = new Dictionary<string, string>();
       // string SERVIDOR_IP = "Servidor";
        public FrmPrincipal()
        {
            InitializeComponent();
            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + "ConfigServidor.txt"))
            {
                ListaCaminhos = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + "ConfigServidor.txt");
                ConexaoAtual.SERVIDOR_IP = ListaCaminhos["SERVIDOR_IP"];

            }
            else
            {
                ListaCaminhos = new Dictionary<string, string>();
                ListaCaminhos.Add("SERVIDOR_IP", ConexaoAtual.SERVIDOR_IP);//13 10

                AcessosStream.WriteLista(Environment.SpecialFolder.LocalApplicationData + "ConfigServidor.txt", ListaCaminhos);
            }

            // carrega os valores para as notas fiscais ( se definido)
            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML))
            {
                Dictionary<string, string> ListaCaminhosXML = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML);
                Configura_XML_NF.PATHNFE = ListaCaminhosXML["CAMINHOXML"];
                Configura_XML_NF.cUF_pad = ListaCaminhosXML["CUF"];
                Configura_XML_NF.CNPJ_pad = ListaCaminhosXML["CNPJ"]; 
            }


            Boolean sucesso = false;
            try
            {

                sucesso = TestaConexao.CheckForInternetConnection(ConexaoAtual.SERVIDOR_IP);


            }
            catch
            {

            }


            if (!sucesso) // chame o configura se servidor e/ou caminho forem invalidos
                configuraçãoToolStripMenuItem.PerformClick();


        }

        private void configuraçãoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            WinFrmConfigura owinform;
            System.Windows.Forms.DialogResult result;
            owinform = new WinFrmConfigura();

            result = System.Windows.Forms.DialogResult.OK;
            if (owinform.ShowDialog() == result)
            {
                ListaCaminhos = owinform.ListaCaminhos;

            }
            owinform.Dispose();

            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + "ConfigServidor.txt"))
            {
                ListaCaminhos = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + "ConfigServidor.txt");
                ConexaoAtual.SERVIDOR_IP = ListaCaminhos["SERVIDOR_IP"];


            }
            else
            {
                ListaCaminhos = new Dictionary<string, string>();
                ListaCaminhos.Add("SERVIDOR_IP", ConexaoAtual.SERVIDOR_IP);//13 10

                AcessosStream.WriteLista(Environment.SpecialFolder.LocalApplicationData + "ConfigServidor.txt", ListaCaminhos);
            }

            Boolean sucesso = false;
            try
            {

                sucesso = TestaConexao.CheckForInternetConnection(ConexaoAtual.SERVIDOR_IP);
                //System.Net.IPHostEntry entry = System.Net.Dns.GetHostEntry(SERVIDOR_IP);



            }
            catch 
            {

            }
            if (!sucesso)
            {
                MessageBox.Show("Servidor ou Diretorio Invalido!! Encerrando Aplicativo");
                sairToolStripMenuItem.PerformClick();
            }

        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void configuraXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCaminhoXML form = new FrmCaminhoXML();
            form.ShowDialog();
        }

        private void nFiscaisProduçãoNewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmFiscal_Novo form = new FrmFiscal_Novo();
            form.Show();
        }

        

        
    }
}
