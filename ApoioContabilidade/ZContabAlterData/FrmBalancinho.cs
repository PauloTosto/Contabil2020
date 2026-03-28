using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApoioContabilidade
{
    public partial class FrmBalancinho : Form
    {
        public DataTable otable;
        private MonteGrid oBalancinho;
        // private bool recomece;
        // private FormFiltro oForm;

        public BindingSource bmSource;

        public FrmBalancinho(DataTable odatatable)
        {
            InitializeComponent();
            // necessario para o DBCONFIGURA Funcioine 
            pnGeral.AutoScroll = true;
            // o tabela tem que ter um nome


            otable = odatatable;
            oBalancinho = new MonteGrid();
            MonteGrids();

            dgvBalancinho.ReadOnly = true;
            dgvBalancinho.AllowUserToAddRows = false;
            //dgvBalancinho.AllowSorting = false;
            oBalancinho.oDataGridView = dgvBalancinho;
            oBalancinho.sbTotal = sbBalancinho;
            // oPlacon = TDataControlReduzido.Placon();


            //oForm = new FormFiltro();
            bmSource = new BindingSource();
            bmSource.DataSource = otable;
            ///bmSourceSaida.DataSource = dvSaidas;
            dgvBalancinho.DataSource = bmSource;

            oBalancinho.ConfigureDBGridView();
            oBalancinho.FuncaoSoma();
            oBalancinho.ColocaTotais();
        }
        private void MonteGrids()
        {
            oBalancinho.Clear();

            oBalancinho.AddValores("NUMCONTA", "CONTA", 12, "", false, 0, "");
            oBalancinho.AddValores("DESCRICAO", "DESCRIÇÃO", 50, "", false, 0, "");
            oBalancinho.AddValores("DEBITOVLR", "DEBITO", 12, "#,###,##0.00", true, 0, "");
            oBalancinho.AddValores("CREDITOVLR", "CREDITO", 12, "#,###,##0.00", true, 0, "");
            oBalancinho.AddValores("SDOVLR", "SALDO", 12, "#,###,##0.00", true, 0, "");

        }

        private void dgvBalancinho_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Columns[e.ColumnIndex].Name == "SDOVLR" &&
                e.RowIndex >= 0 &&
                dgv["SDOVLR", e.RowIndex].Value is Decimal)
            {


                if (e.Value != null)
                {
                    try
                    {
                        System.Text.StringBuilder numberString = new System.Text.StringBuilder();

                        /* string numero  = number
                         dateString.Append(theDate.Month);
                         dateString.Append("/");
                         dateString.Append(theDate.Day);
                         dateString.Append("/");
                         dateString.Append(theDate.Year.ToString().Substring(2));
                         formatting.Value = dateString.ToString();
                         formatting.FormattingApplied = true;*/
                    }
                    catch (FormatException)
                    {
                        // Set to false in case there are other handlers interested trying to
                        // format this DataGridViewCellFormattingEventArgs instance.
                        e.FormattingApplied = false;
                    }
                }



                /*switch ((int)dgv["TargetColumnName", e.RowIndex].Value)
                {

                //Create custom display text/value here and assign to e.Value
                string dataformatValue = //Create from database value;
                e.Value = dataformatValue;
                e.FormattingApplied = true; 
               */
            }
        }
        // REFERENCIAS PARA ROWFILTER => https://www.csharp-examples.net/dataview-rowfilter/
        private void txNumconta_TextChanged(object sender, EventArgs e)
        {
           string numconta =  (sender as TextBox).Text.Trim();
         //  if ((numconta.Length == 4) || (numconta.Length == 6) || (numconta.Length == 7))  return;
            // string filtro = "SUBSTRING(NUMCONTA,1," + numconta.Length.ToString() + ") = '" + numconta + "'";
            if (numconta.Length == 0) { bmSource.DataSource = otable; }
            else
            {
                try
                {
                    var dado = otable.AsEnumerable().Where(row =>
                        (row.Field<string>("NUMCONTA").Substring(0, numconta.Length) == numconta));
                    if ((dado != null) && (dado.Count() > 0))
                    { bmSource.DataSource = dado.CopyToDataTable(); }
                    else { return; }
                }
                catch (Exception)
                {
                    return;
                }
            }
            oBalancinho.FuncaoSoma();
            oBalancinho.oDataGridView.Refresh();
            oBalancinho.ColocaTotais(); 
        }
    }

}
