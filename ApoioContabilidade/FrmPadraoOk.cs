using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
//using System.IO;
//using System.Data.OleDb;
//using ClassConexao;
using ClassFiltroEdite;
//using ClassLibTrabalho;


namespace ApoioContabilidade
{
    public partial class FrmPadraoOk : Form
    {
        public DataTable odatatable;
        public DataSet odataset;
        public MonteGrid oCadastro;
        BindingSource bmResto;
        
        public FrmPadraoOk()
        {
            InitializeComponent();
         
        }

        private void FrmPadraoOk_Load(object sender, EventArgs e)
        {
            if (odataset == null) return;
            oCadastro = new MonteGrid();
            MonteGrids();
            DataTable cltresto = odataset.Tables[0].Clone();
            cltresto.TableName = "CLTRESTO";
            string lit = "INCLUIDO = false";
            odataset.Tables[0].Select(lit).CopyToDataTable(cltresto, LoadOption.PreserveChanges);
            bmResto = new BindingSource();
            bmResto.DataSource = cltresto.AsDataView();
            oCadastro.oDataGridView = dgvCadastro;
            oCadastro.oDataGridView.DataSource = bmResto;
           // oCadastro.oDataGridView.DataMember = odataset.Tables[0].TableName;
            oCadastro.ConfigureDBGridView();
            Refresh();
        }

        private void MonteGrids()
        {
            oCadastro.Clear();
            oCadastro.AddValores("CODCAD", "CODIGO", 8, "", false, 0, "");
            oCadastro.AddValores("NOMECAD", "NOME", 35, "", false, 0, "");
            oCadastro.AddValores("SETOR", "SETOR", 6, "", false, 0, "");
            
        }
    }
}
