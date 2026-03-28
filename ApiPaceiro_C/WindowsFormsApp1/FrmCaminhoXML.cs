using PrjApiParceiro_C.Comum;
using PrjApiParceiro_C.Core;
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
    public partial class FrmCaminhoXML : Form
    {
        List<String> lista = new List<string>();
        public Dictionary<string, string> ListaCaminhos;
        string CNPJ;
        string PATHNFE;
        string CUF;

        public FrmCaminhoXML()
        {
            InitializeComponent();
            ListaCaminhos = new Dictionary<string, string>();

            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML))
            {
                ListaCaminhos = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML);
                CNPJ = ListaCaminhos["CNPJ"];
                PATHNFE = ListaCaminhos["CAMINHOXML"];
                CUF = ListaCaminhos["CUF"];
                Configura_XML_NF.PATHNFE = PATHNFE;
                Configura_XML_NF.cUF_pad = CUF;
                Configura_XML_NF.CNPJ_pad = CNPJ;
            }
            else
            {
                ListaCaminhos.Add("CAMINHOXML", Configura_XML_NF.PATHNFE);
                ListaCaminhos.Add("CNPJ", Configura_XML_NF.CNPJ_pad);
                ListaCaminhos.Add("CUF", Configura_XML_NF.cUF_pad);
                CNPJ = Configura_XML_NF.CNPJ_pad;
                PATHNFE = Configura_XML_NF.PATHNFE;
                CUF = Configura_XML_NF.cUF_pad;
            }

            txtCUF.Text = CUF;
            txtPATHNFE.Text = PATHNFE;
            txtCNPJ.Text = CNPJ;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (!(Directory.Exists(txtPATHNFE.Text)))
            {
                MessageBox.Show("Diretorio Não Existe");
                return; 
            }

           
            ListaCaminhos["CNPJ"] = txtCNPJ.Text;
            ListaCaminhos["CAMINHOXML"] = txtPATHNFE.Text;
            ListaCaminhos["CUF"] = txtCUF.Text;

            try
            {
                AcessosStream.WriteLista(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML, ListaCaminhos);
            }
            catch (Exception)
            {

                throw;
            }

            Configura_XML_NF.PATHNFE = ListaCaminhos["CAMINHOXML"];
            Configura_XML_NF.cUF_pad = ListaCaminhos["CUF"];
            Configura_XML_NF.CNPJ_pad = ListaCaminhos["CNPJ"];
           Close();
        }

        private void BtnCancele_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}

