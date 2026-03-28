using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    public partial class FrmErros : Form
    {
        public DataTable otable;
        private MonteGrid oEntradas;
       // private bool recomece;
       // private FormFiltro oForm;

        public BindingSource bmSourceEntrada;
        // public BindingSource bmSourceSaida;
        //DataView oPlacon;

        public FrmErros(DataTable odatatable)
        {
            InitializeComponent();
            otable = odatatable;
            oEntradas = new MonteGrid();
            MonteGrids();
           // dataGridView1.CaptionText = "ERROS ";
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            //dataGridView1.AllowSorting = false;
            oEntradas.oDataGridView = dataGridView1;
           // oEntradas.sbTotal = sbEntradas;
           // oPlacon = TDataControlReduzido.Placon();


            //oForm = new FormFiltro();
            bmSourceEntrada = new BindingSource();
            bmSourceEntrada.DataSource = otable.AsDataView();
            ///bmSourceSaida.DataSource = dvSaidas;
            dataGridView1.DataSource = bmSourceEntrada;

            oEntradas.ConfigureDBGridView();

  
        }
        private void MonteGrids()
        {
            oEntradas.Clear();
            oEntradas.AddValores("DATA", "DATA", 0, "", false, 0, "");
            oEntradas.AddValores("DEBITO", "DEBITO", 40, "", true, 0, "");
            oEntradas.AddValores("CREDITO", "CREDITO", 40, "", true, 0, "");
            oEntradas.AddValores("VALOR", "VALOR", 12, "#,###,##0.00", true, 0, "");
            oEntradas.AddValores("HIST", "HISTORICO", 40, "", false, 0, "");
            oEntradas.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oEntradas.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");


        }
       
    }
}
