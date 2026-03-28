using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Produção
{
    public partial class FrmFiltroProd : Form
    {
        public FrmFiltroProd()
        {
            InitializeComponent();
            lbContrato.Visible = false;
            txContratos.Visible = false;

        }

        private void rbVendidos_Click(object sender, EventArgs e)
        {
            if (rbVendidos.Checked)
            {
                lbContrato.Visible = true;
                txContratos.Visible = true;
            } 
            else
            {
                lbContrato.Visible = false;
                txContratos.Visible = false;
                //txContratos.Text = "";
            }



        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!rbVendidos.Checked)
                txContratos.Text = "";
        }

        private void btnCancele_Click(object sender, EventArgs e)
        {
            if (!rbVendidos.Checked)
                txContratos.Text = "";
        }
    }
}
