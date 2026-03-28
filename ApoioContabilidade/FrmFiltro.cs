using System;
using System.Windows.Forms;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    public partial class FormFiltro : Form
    {
        // Construção do Filtro Generico
        public ArmeEdicao oArme;
        
        public FormFiltro()
        {
            InitializeComponent();
            oPesqFin = new Pesquise();//TPesquisa.Create(self);
            oPesqFin.Parent = this;
            oPesqFin.TabIndex = 0;
            oPesqFin.Left = 0;
            oPesqFin.Top = this.Top + panel1.Top + panel1.Height + 2;
            oPesqFin.Width = panel1.Width;
            oPesqFin.Height = (this.ClientSize.Height - oPesqFin.Top);
            

        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            if (oPesqFin.Pagina("Geral") != null) return;
            if (oArme == null) return;
            oPesqFin.Linhas = oArme.Linhas;
            oPesqFin.NovaPagina("Geral");
            oPesqFin.Pagina("Geral").Text = "&1) Lançamentos";
            oPesqFin.SelectedIndex = 0;
        }


       

        private void btnConsulta_Click(object sender, EventArgs e)
        {
            Close();
       
        }

        
        
        
       
       
     
     
    }
}
