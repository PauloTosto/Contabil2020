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

namespace ApoioContabilidade.Almoxarifado.FormsConsulta
{
    public partial class FrmConsulta : Form
    {
        public DataSet dsDados;
        BindingSource bmSource = new BindingSource();
        public DataTable servEscolhidos;
        private MonteGrid oMonte;
        private int anos;
         
        public FrmConsulta(DataSet odataSet, int oanos)
        {
            dsDados = odataSet;
            anos = oanos;
            InitializeComponent();
            if (anos == 1)
                cbMarcarTodos.Visible = true;
            else
                cbMarcarTodos.Visible = false;

            servEscolhidos = dsDados.Tables[0].Copy();
            servEscolhidos.Columns.Add("ESCOLHIDO", Type.GetType("System.Boolean"));
            servEscolhidos.TableName = "ESCOLHIDOS";
            foreach (DataRow orow in servEscolhidos.Rows)
            {
                orow.BeginEdit();
                orow["ESCOLHIDO"] = false;
                orow.EndEdit();
                orow.AcceptChanges();
            }
            servEscolhidos.AcceptChanges();

                     MonteGrids();
            dgvEscolha.CellFormatting += DgvEscolha_CellFormatting;
            dgvEscolha.CellContentClick += DgvEscolha_CellContentClick;

            dgvEscolha.Enter += color_Enter;
            dgvEscolha.Leave += color_Leave;

            bmSource.DataSource = servEscolhidos.AsDataView();
            oMonte.oDataGridView = dgvEscolha;
            oMonte.oDataGridView.DataSource = bmSource;
            oMonte.ConfigureDBGridView();
            pnGrid.Focus();
            dgvEscolha.Focus();



        }
        private void DgvEscolha_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex == -1) return;
            if ((sender as DataGridView).Columns.Contains("ESCOLHIDO"))
            {
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

        private void MonteGrids()
        {

            oMonte = new MonteGrid();
            oMonte.Clear();
            if (anos == 1)
            {
                oMonte.AddValores("ESCOLHIDO", "Check", 6, "", false, 0, "");
            }
            else
            {
                oMonte.AddValores("DATA", "Semana", 10, "", false, 0, "");
            }

            
            oMonte.AddValores("NUM_MOD", "Num.", 4, "", false, 0, "");
            oMonte.AddValores("MODEL", "Modelo", 20, "", false, 0, "");
            oMonte.AddValores("CODSER", "Código", 6, "", false, 0, "");
            oMonte.AddValores("SERV", "Serviço", 35, "", false, 0, "");
            oMonte.AddValores("GLEBA", "Gleba", 6, "", false, 0, "");
            oMonte.AddValores("QUADRA", "Quadra", 6, "", false, 0, "");
            oMonte.AddValores("OK", "OK", 4, "", false, 0, "");
            oMonte.AddValores("DATAOK", "Data OK", 10, "", false, 0, "");
            oMonte.AddValores("INICIO", "Inicio", 10, "", false, 0, "");
            oMonte.AddValores("FIM", "Fim", 10, "", false, 0, "");
            oMonte.AddValores("HORAS_EFETIVAS", "Horas", 10, "###,##0.00", false, 0, "");
        }

        private void cbMarcarTodos_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as RadioButton).Checked)
            {
                filtro();
               //  bmSource.DataSource = servEscolhidos.AsDataView();
                /*else
                {
                    bmSource.DataSource = trabsEscolhidos.AsEnumerable().Where(
                        row => ckListSetores.CheckedItems.Contains(row.Field<string>("SETOR"))).AsDataView();
                }*/
            }
        }

        private void filtro()
        {
            string servico = txServico.Text.Trim();
           List<string> dadosServico = new List<string>();
            List<string> dadosGleba = new List<string>();
            List<string> dadosQuadra = new List<string>();

            if (txServico.Text.Trim() != "")
            {
                dadosServico = txServico.Text.Split(Convert.ToChar("/")).ToList();
                for (int i = 0; i < dadosServico.Count; i++)
                {
                    if (dadosServico[i].Trim() == "") dadosServico.RemoveAt(i);
                }
            }
            
            if (txGleba.Text.Trim() != "")
                dadosGleba = txGleba.Text.Split(Convert.ToChar("/")).ToList();

            if (txQuadra.Text.Trim() != "")
                dadosQuadra = txQuadra.Text.Split(Convert.ToChar("/")).ToList();
            bmSource.DataSource = servEscolhidos.AsEnumerable()
            .Where(row =>
            //  (servico == "" ? true :  row.Field<string>("SERV").Trim().Contains(servico) )
            (dadosServico.Count == 0 ? true : PesquiseLikeServico(row.Field<string>("SERV"),dadosServico))
            &&
            (dadosGleba.Count == 0 ? true : dadosGleba.Contains(row.Field<string>("GLEBA")))
            &&
            (dadosQuadra.Count == 0 ? true : dadosQuadra.Contains(row.Field<string>("QUADRA")))
           
            ).AsDataView();
        }

        private bool PesquiseLikeServico(string servField, List<string> lstServico)
        {
            bool result = false;
            foreach(string servico in lstServico)
            {
                result = (servField.Contains(servico) ) || result;
            }
            return result;
        }





        private void txServico_TextChanged(object sender, EventArgs e)
        {
            filtro();
        }

     
        private void txGleba_TextChanged(object sender, EventArgs e)
        {
            filtro();
        }
    }
}
