using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace ApoioContabilidade
{
    public partial class WinFrmConfigura : Form
    {
        public WinFrmConfigura()
        {
            InitializeComponent();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {

        }

        private void TextBoxCampo_Validating(object sender, CancelEventArgs e)
        {
     
             string diretorio = (sender as TextBox).Text;
            if (diretorio =="") {return;}
            if (diretorio[diretorio.Length-1] != Path.DirectorySeparatorChar) 
            {diretorio = diretorio + Path.DirectorySeparatorChar;}

            if ( Directory.Exists(diretorio) == false)
            { 
               e.Cancel = true;
               MessageBox.Show("Diretorio Invalido", "",
               MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk); 
            }


            else
             if (diretorio != (sender as TextBox).Text) 
             { (sender as TextBox).Text = diretorio;}

        }

        private void ButtonCampo_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog1;
            System.Windows.Forms.DialogResult result;

            folderBrowserDialog1 = new FolderBrowserDialog();

            result = System.Windows.Forms.DialogResult.OK;
            
            if (folderBrowserDialog1.ShowDialog() == result)
            {

                try
                {
                    if ((sender as Button).Name == "ButtonCampo")
                    {
                        TextBoxCampo.Text = folderBrowserDialog1.SelectedPath;
                        TextBoxCampo.Select();
                    }
                    if ((sender as Button).Name == "ButtonTrabalho")
                    {
                        TextBoxTrabalho.Text = folderBrowserDialog1.SelectedPath;
                        TextBoxTrabalho.Select();
                    }
                    if ((sender as Button).Name == "ButtonEscritor")
                    {
                        TextBoxEscritor.Text = folderBrowserDialog1.SelectedPath;
                        TextBoxEscritor.Select();
                    }

                    if ((sender as Button).Name == "ButtonContab")
                    {
                        TextBoxContab.Text = folderBrowserDialog1.SelectedPath;
                        TextBoxContab.Select();
                    }
                   (sender as Button).Select();
                }
                finally { }

            }
            folderBrowserDialog1.Dispose();

        }

        private void BtnOK_Click_1(object sender, EventArgs e)
        {

        }

        private void btnCancela_Click(object sender, EventArgs e)
        {

        }

         }
}
