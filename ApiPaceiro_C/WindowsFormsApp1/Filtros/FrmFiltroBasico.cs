using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PrjApiParceiro_C.Filtros
{
    public partial class FrmFiltroBasico : Form
    {
        public DateTime data1 = new DateTime();
        public DateTime data2 = new DateTime();
        public FrmFiltroBasico()
        {
            InitializeComponent();
        }

        private void FrmFiltroBasico_Load(object sender, EventArgs e)
        {
            inicio.Value = data1;
            fim.Value = data2;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            data1 = inicio.Value;
            data2 = fim.Value;
        }
    }
}
