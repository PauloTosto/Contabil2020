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

namespace ApoioContabilidade.Trabalho.PegDadosCltCad
{
    public partial class FrmEscolhaTrab : Form
    {
        public DataSet dsDados;
        BindingSource bmSource = new BindingSource();
        public DataTable trabsEscolhidos;
        private MonteGrid oMonte;
        

        public FrmEscolhaTrab(DataSet odsDados)
        {
            dsDados = odsDados;
            InitializeComponent();
            MonteGrids();
            dgvEscolha.CellFormatting += DgvEscolha_CellFormatting;
            dgvEscolha.CellContentClick += DgvEscolha_CellContentClick;

            dgvEscolha.Enter += color_Enter;
            dgvEscolha.Leave += color_Leave;

            trabsEscolhidos = odsDados.Tables[0].Copy();
            trabsEscolhidos.Columns.Add("Escolhido", Type.GetType("System.Boolean"));
            trabsEscolhidos.TableName = "ESCOLHIDOS";
            foreach (DataRow orow in trabsEscolhidos.Rows)
            {
                orow.BeginEdit();
                orow["ESCOLHIDO"] = false;
                orow.EndEdit();
                orow.AcceptChanges();
            }
            trabsEscolhidos.AcceptChanges();
            List<string> setores = (from gr in trabsEscolhidos.AsEnumerable()
                                    group gr by new
                                    {
                                        setor = gr.Field<string>("SETOR")
                                    } into g
                                    select g.Key.setor
                                ).ToList();
            ckListSetores.Items.Clear();
            ckListSetores.ColumnWidth = 35;
            ckListSetores.Height = 70;
            ckListSetores.Width = 120;
            foreach (string setor in setores)
            {
                ckListSetores.Items.Add(setor);
            }
            
            bmSource.DataSource = trabsEscolhidos.AsDataView();
            oMonte.oDataGridView = dgvEscolha;
            oMonte.oDataGridView.DataSource = bmSource;
            oMonte.ConfigureDBGridView();
            pnGrid.Focus();
            dgvEscolha.Focus();
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

        private void MonteGrids()
        {

            oMonte = new MonteGrid();
            oMonte.Clear();
            oMonte.AddValores("ESCOLHIDO", "Check", 6, "", false, 0, "");
            oMonte.AddValores("CODCAD", "Código", 8, "", false, 0, "");
            oMonte.AddValores("NOMECAD", "Nome", 45, "", false, 0, "");
            oMonte.AddValores("Setor", "Setor", 6, "", false, 0, "");
        }

        private void FrmEscolhaTrab_Load(object sender, EventArgs e)
        {
            dgvEscolha.Focus();
        }

        private void cbMarcarTodos_CheckedChanged(object sender, EventArgs e)
        {
            bool marque = (sender as CheckBox).Checked;
            try
            {
                foreach (DataRowView orow in (bmSource.DataSource as DataView))
                {
                    orow.BeginEdit();
                    orow["ESCOLHIDO"] = marque;
                    orow.EndEdit();
                    orow.Row.AcceptChanges();
                }
            }
            catch (Exception)
            {


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

        private void rbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                if (ckListSetores.CheckedItems.Count == 0)
                    bmSource.DataSource = trabsEscolhidos.AsDataView();
                else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => ckListSetores.CheckedItems.Contains(row.Field<string>("SETOR"))).AsDataView();
                }
            }
        }

        private void rbMensalistas_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                if (ckListSetores.CheckedItems.Count == 0)
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => (row.Field<string>("MENSALISTA").Trim() == "X")).AsDataView();

                else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row =>
                        (row.Field<string>("MENSALISTA").Trim() == "X") &&
                        ckListSetores.CheckedItems.Contains(row.Field<string>("SETOR"))).AsDataView();
                }
            }
        }

        private void rbDiaristas_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                if (ckListSetores.CheckedItems.Count == 0)
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => (row.Field<string>("MENSALISTA").Trim() == "")).AsDataView();

                else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row =>
                        (row.Field<string>("MENSALISTA").Trim() == "") &&
                        ckListSetores.CheckedItems.Contains(row.Field<string>("SETOR"))).AsDataView();
                }
            }
        }

        private void ckListSetores_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // se o estado for unchecked está prestes a mudar para check
            CheckState estado = e.CurrentValue;
            List<string> setoresCheck = new List<string>();
            string esteSetor = (sender as CheckedListBox).Items[e.Index].ToString();
            foreach (string oj in ckListSetores.CheckedItems)
            {
                if ((estado == CheckState.Checked) && (oj == esteSetor)) continue; // tire da lista  
                setoresCheck.Add(oj);
            }
            // será setado
            if (estado == CheckState.Unchecked)
            {
                setoresCheck.Add(esteSetor);
            }

            if (rbTodos.Checked)
            {
                if (setoresCheck.Count == 0)
                    bmSource.DataSource = trabsEscolhidos.AsDataView();
                else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => setoresCheck.Contains(row.Field<string>("SETOR"))).AsDataView();
                }
             
            }
            if (rbMensalistas.Checked)
            {
                if (setoresCheck.Count == 0)
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => (row.Field<string>("MENSALISTA").Trim() == "X")).AsDataView();

                else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row =>
                        (row.Field<string>("MENSALISTA").Trim() == "X") &&
                        setoresCheck.Contains(row.Field<string>("SETOR"))).AsDataView();
                }

            }
            if (rbDiaristas.Checked)
            {
                if (setoresCheck.Count == 0)
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => (row.Field<string>("MENSALISTA").Trim() == "")).AsDataView();

                else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row =>
                        (row.Field<string>("MENSALISTA").Trim() == "") &&
                        setoresCheck.Contains(row.Field<string>("SETOR"))).AsDataView();
                }
            }

            
        }
    }
}
