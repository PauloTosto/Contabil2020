using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.FormsPesquisa
{
    public partial class FrmPesquisaCkList : Form
    {
        BindingSource bmSource = new BindingSource();
        public DataTable servEscolhidos;
        private MonteGrid oMonte;
        private List<string> lstCodServ;
        public Dictionary<string, int> dictSemana;
        public Dictionary<string, int> dictAnterior;
        public FrmPesquisaCkList(List<string>  olstCodServ, Dictionary<string, int> Semana, Dictionary<string, int> Anterior)
        {
            InitializeComponent();
            dictSemana = Semana;
            dictAnterior = Anterior;
            lstCodServ = olstCodServ;
            MonteGrids();
            dgvEscolha.CellFormatting += DgvEscolha_CellFormatting;
            dgvEscolha.CellContentClick += DgvEscolha_CellContentClick;

            dgvEscolha.Enter += color_Enter;
            dgvEscolha.Leave += color_Leave;
            servEscolhidos = TabelasIniciais.DsTabelasInciais().Tables["SERVICOONLY"].Clone();
             TabelasIniciais.DsTabelasInciais().Tables["SERVICOONLY"].AsEnumerable()
                                     .Where(row => lstCodServ.Contains(row.Field<string>("COD"))).OrderBy(row => row.Field<string>("DESCRI"))
                                     .CopyToDataTable(servEscolhidos,LoadOption.OverwriteChanges);   //odsDados.Tables[0].Copy();
            servEscolhidos.Columns.Add("Escolhido", Type.GetType("System.Boolean"));
            servEscolhidos.Columns.Add("FREQSEMANA", Type.GetType("System.Int32"));
            servEscolhidos.Columns.Add("FREQANTERIOR", Type.GetType("System.Int32"));
            servEscolhidos.TableName = "ESCOLHIDOS";
            foreach (DataRow orow in servEscolhidos.Rows)
            {
                orow.BeginEdit();
                if (dictSemana.ContainsKey(orow["COD"].ToString()))
                {
                    orow["FREQSEMANA"] = dictSemana[orow["COD"].ToString()];
                }
                if (dictAnterior.ContainsKey(orow["COD"].ToString()))
                {
                    orow["FREQANTERIOR"] = dictAnterior[orow["COD"].ToString()];
                }

                orow["ESCOLHIDO"] = false;
                orow.EndEdit();
                orow.AcceptChanges();
            }
            servEscolhidos.AcceptChanges();
            bmSource.DataSource = servEscolhidos.AsDataView();
            oMonte.oDataGridView = dgvEscolha;
            oMonte.oDataGridView.DataSource = bmSource;
            oMonte.ConfigureDBGridView();
            dgvEscolha.Focus();
        }
        private void MonteGrids()
        {

            oMonte = new MonteGrid();
            oMonte.Clear();
            oMonte.AddValores("ESCOLHIDO", "Check", 6, "", false, 0, "");
            oMonte.AddValores("COD", "Código", 6, "", false, 0, "");
            oMonte.AddValores("DESCRI", "Servico", 25, "", false, 0, "");
            oMonte.AddValores("FREQSEMANA", "Semana(n)", 10, "", false, 0, "");
            oMonte.AddValores("FREQANTERIOR", "Anterior(n)", 10, "", false, 0, "");

        }
        private void DgvEscolha_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == (sender as DataGridView).Columns["Escolhido"].Index)
            {
                try
                {
                    // DataRowView orow = (bmSource.Current as DataRowView);
                    DataGridViewRow orow = (sender as DataGridView).Rows[e.RowIndex];
                    if (Convert.ToBoolean(orow.Cells["Escolhido"].Value))
                    {

                        orow.DefaultCellStyle.BackColor = Color.LightGreen;
                        //e.CellStyle.BackColor = Color.LightGreen; 
                    }
                    else
                    {
                        orow.DefaultCellStyle.BackColor = (sender as DataGridView).DefaultCellStyle.BackColor;
                        //e.CellStyle.BackColor = (sender as DataGridView).DefaultCellStyle.BackColor;
                    }

                }
                catch (Exception)
                {
                }
            }

        }

        private void DgvEscolha_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /// Este evento é disparado quando o usuário clica no conteúdo de uma célula
            /// Vamos exibir uma mensagem contendo os valores true ou valse refletindo os
            /// o valores da coluna checkbox
            ///
            //Verificamos se e somente se a celula checkbox (Estado) foi clicada
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == (sender as DataGridView).Columns["Escolhido"].Index)
            {
                try
                {
                    DataRowView orow = (bmSource.Current as DataRowView);
                    orow.Row.BeginEdit();
                    if (Convert.ToBoolean(orow.Row["Escolhido"]))
                    {
                        orow.Row["Escolhido"] = false;
                    }
                    else
                    {
                        orow.Row["Escolhido"] = true;
                    }
                    orow.Row.EndEdit();
                    orow.Row.AcceptChanges();
                }
                catch (Exception)
                {
                }
            }
        }
        private void color_Leave(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.Control;

        }
        private void color_Enter(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.ActiveCaption;
        }

    }
}
