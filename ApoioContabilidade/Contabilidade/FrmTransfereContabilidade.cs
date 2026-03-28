using ApoioContabilidade.Models;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace ApoioContabilidade.Contabilidade
{
    public partial class FrmTransfereContabilidade : Form
    {
        bool movFinExiste = false;
        DataSet dsInsumos;
        DataSet dsFolhas;
        List<MonteGrid> lstMonteGrid;
        public FrmTransfereContabilidade()
        {
            InitializeComponent();
            dtData1.Format = DateTimePickerFormat.Custom;
            dtData1.CustomFormat = "MM/yyyy";
            checkDiretorioContabil();
            dsInsumos = new DataSet();
            dsFolhas = new DataSet();
            lstMonteGrid = new List<MonteGrid>();
            tcMeses.TabPages.Clear();
            btnTransfere.Enabled = false;
        }

        private void checkDiretorioContabil()
        {
            movFinExiste = TDataControlContab.TabelaExiste("MOVFIN.DBF");
            if (!movFinExiste)
            {
                MessageBox.Show("Tabela MOVFIN.DBF Não Existe em:" + TDataControlReduzido.Get_Path("CONTAB"));

            }
            btnTransfere.Enabled = movFinExiste;
        }

        
        private void dtData1_ValueChanged(object sender, EventArgs e)
        {
            if (nMeses.Value == 0)
            {
                lbPeriodo.Text = "";
            }
            else
            {
                lbPeriodo.Text = dtData1.Value.ToString("MM/yyyy") + " a " + dtData1.Value.AddMonths(Convert.ToInt32(nMeses.Value)).ToString("MM/yyyy");
            }
            btnConsulta.Enabled = true;
        }

        private void nMeses_ValueChanged(object sender, EventArgs e)
        {
            if ((sender as NumericUpDown).Value == 0)
            {
                lbPeriodo.Text = "";
            }
            else
            {
                lbPeriodo.Text = dtData1.Value.ToString("MM/yyyy") + " a " + dtData1.Value.AddMonths(Convert.ToInt32(nMeses.Value)).ToString("MM/yyyy");
            }
            btnConsulta.Enabled = true;
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            btnTransfere.Enabled = false;
            dsFolhas.Tables.Clear();
            dsInsumos.Tables.Clear();
            tcMeses.TabPages.Clear();
            rbProvFolha.Checked = true;
            /*
             * [dbo].[COMPATIBILIDADE_CUSTOS_CONTABIL]
	            @inicio_orig date,
	            @fim_orig date,
	            @meses int = 0
            */
            DateTime inicio, fim;
            inicio = new DateTime(dtData1.Value.Year, dtData1.Value.Month, 1);
            fim = inicio.AddMonths(1).AddDays(-1);
            DataSet dsResult = null;
            string str = "";
            nMeses.Value = 0;

            List<string> lstquery = new List<string>();
            lstquery.Add("inicio_orig=" + inicio.ToString("yyyy-MM-dd"));
            lstquery.Add("fim_orig=" + fim.ToString("yyyy-MM-dd"));
            lstquery.Add("meses=" + Convert.ToInt32(nMeses.Value).ToString());
            lstquery.Add("sp_numero=" + "602");


            dsInsumos.Tables.Clear();
            dsFolhas.Tables.Clear();
            try
            {
                /* no resultado cada select tem as datas e os formatos
                  FORMATO 0 = > INSUMOS
                  FORMATO 1 = > CUSTOFOLHA
                  FORMATO 2 = > NOVACLTFOLHA(Forma até 2020)*/

                dsResult = await ApiServices.Api_QuerySP(lstquery);
            }
            catch (Exception)
            {
            }
            if (dsResult != null)
            {
                int ordem = 0;
                foreach (DataTable table in dsResult.Tables)
                {
                    if (ordem == 1)
                    {
                        dsInsumos.Tables.Add(table.Copy());
                        dsInsumos.Tables[dsInsumos.Tables.Count - 1].TableName = "Insumos" + dsInsumos.Tables.Count.ToString();
                    }
                    else if (ordem == 0) // modelo antigo 
                    {
                        dsFolhas.Tables.Add(table.Copy());
                        dsFolhas.Tables[dsFolhas.Tables.Count - 1].TableName = "Folhas" + dsFolhas.Tables.Count.ToString();
                    }
                    ordem++;
                    if (ordem == 2)
                        ordem = 0;
                }
                MonteGrids();

            }
            else
            {
                MessageBox.Show("Sem Resultados");
            }

            btnConsulta.Enabled = false;
            btnTransfere.Enabled = true;
        }
        private void MonteGrids()
        {
            lstMonteGrid.Clear();
            for (int i = 0; i < dsFolhas.Tables.Count; i++)
            {
                MonteGrid oMonte = new MonteGrid();
                oMonte.Clear();
                oMonte.AddValores("DATA", "Data", 0, "", false, 0, "");
                oMonte.AddValores("DEBITO", "Debito", 25, "", false, 0, "");
                oMonte.AddValores("CREDITO", "Credito", 25, "", false, 0, "");
                oMonte.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
                oMonte.AddValores("HIST", "Historico", 40, "", false, 0, "");
                oMonte.AddValores("DOC", "Doc.", 15, "", false, 0, "");
                lstMonteGrid.Add(oMonte);
            }
            tcMeses.TabPages.Clear();
            DateTime inicio = new DateTime(dtData1.Value.Year, dtData1.Value.Month, 1);
            DateTime fim = inicio.AddMonths(1).AddDays(-1);
            for (int i = 0; i < dsFolhas.Tables.Count; i++)
            {
                tcMeses.TabPages.Add(new TabPage());
                tcMeses.TabPages[i].Text = "Mes_" + dtData1.Value.AddMonths(i).ToString("MM_yyyyy");
                Panel opanel = new Panel();
                opanel.Parent = tcMeses.TabPages[i];
                opanel.BackColor = Color.AliceBlue;
                opanel.Dock = DockStyle.Fill;
                tcMeses.TabPages[i].Enter += color_Enter;
                tcMeses.TabPages[i].Leave += color_Leave;
                DataGridView dataGrid = new DataGridView();
                dataGrid.Parent = opanel;
                dataGrid.Width = opanel.Width;
                dataGrid.Height = opanel.Height - 40;
                Label oTotal = new Label();
                oTotal.Parent = opanel;
                oTotal.Left = 10;
                oTotal.Top = dataGrid.Height + 5;
                oTotal.Height = oTotal.Height + 10;
                oTotal.Text = "TOTAL";
                oTotal.AutoSize = true;
                //dataGrid.Dock = DockStyle.Fill;
                /* dataGrid.Width = dataGrid.Parent.Size.Width - 40;
                 dataGrid.Height = dataGrid.Parent.Size.Height - 40;
                 dataGrid.Left = 20;
                 dataGrid.Top = 20;*/
                tcMeses.TabPages[i].Padding = new Padding(10);
                BindingSource binding = new BindingSource();
                binding.DataSource = dsFolhas.Tables[i].AsDataView();
                lstMonteGrid[i].oDataGridView = dataGrid;
                lstMonteGrid[i].oDataGridView.DataSource = binding;
                lstMonteGrid[i].ConfigureDBGridView();
                dataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                // dataGrid.Dock = DockStyle.Fill;
                //dataGrid.Location = new Point(10, 10);

                DateTime fim_mes = fim.AddMonths(i);
                double? debito = 0;
                double? credito = 0;
                try
                {
                    debito = dsFolhas.Tables[i].AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                 // && (a.Field<Int64>("FORMATO") == 1)
                  && (a.Field<string>("DEBITO").Trim() != "")
                     ).Sum(a => a.Field<double?>("VALOR")) ?? 0;

                }
                catch (Exception)
                {
                }
                try
                {
                    credito = dsFolhas.Tables[i].AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                   //     && (a.Field<Int64>("FORMATO") == 1)
                        && (a.Field<string>("CREDITO").Trim() != "")
                        ).Sum(a => (double?)a.Field<double?>("VALOR")) ?? 0;
                }
                catch (Exception)
                {
                }
                oTotal.Text = "TOTAIS => " + String.Format(" Debito: {0,12:###,###,##0.00}", Convert.ToDecimal(debito));
                oTotal.Text = oTotal.Text + String.Format("    Credito: {0,12:###,###,##0.00}", Convert.ToDecimal(credito));



             /*   DateTime fim_mes = fim.AddMonths(i);
                double debito = 0;
                double credito = 0;
                try
                {
                    debito = dsFolhas.Tables[i].AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                  && (a.Field<Int64>("FORMATO") == 2)
                  && (a.Field<string>("DEBITO").Trim() != "")
                     ).Sum(a => a.Field<double>("VALOR"));
                    credito = dsFolhas.Tables[i].AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                    && (a.Field<Int64>("FORMATO") == 2)
                    && (a.Field<string>("CREDITO").Trim() != "")
                    ).Sum(a => a.Field<double>("VALOR"));
                }
                catch (Exception)
                {
                }
               oTotal.Text = "TOTAIS => "+ String.Format(" Debito: {0,12:###,###,##0.00}", Convert.ToDecimal(debito));
               oTotal.Text = oTotal.Text + String.Format("    Credito: {0,12:###,###,##0.00}", Convert.ToDecimal(credito));*/
            }
            tcMeses.SelectedIndex = 0;
        }

        private void MonteGrids_Insumos()
        {
            lstMonteGrid.Clear();
            for (int i = 0; i < dsFolhas.Tables.Count; i++)
            {
                MonteGrid oMonte = new MonteGrid();
                oMonte.Clear();
                oMonte.AddValores("DATA", "Data", 0, "", false, 0, "");
                oMonte.AddValores("DEBITO", "Debito", 25, "", false, 0, "");
                oMonte.AddValores("CREDITO", "Credito", 25, "", false, 0, "");
                oMonte.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
                oMonte.AddValores("HIST", "Historico", 40, "", false, 0, "");
                oMonte.AddValores("DOC", "Doc.", 15, "", false, 0, "");
                lstMonteGrid.Add(oMonte);
            }
            tcMeses.TabPages.Clear();
            DateTime inicio = new DateTime(dtData1.Value.Year, dtData1.Value.Month, 1);
            DateTime fim = inicio.AddMonths(1).AddDays(-1);
            for (int i = 0; i < dsInsumos.Tables.Count; i++)
            {
                tcMeses.TabPages.Add(new TabPage());
                tcMeses.TabPages[i].Text = "Mes_" + dtData1.Value.AddMonths(i).ToString("MM_yyyyy");
                Panel opanel = new Panel();
                opanel.Parent = tcMeses.TabPages[i];
                opanel.BackColor = Color.AliceBlue;
                opanel.Dock = DockStyle.Fill;
                tcMeses.TabPages[i].Enter += color_Enter;
                tcMeses.TabPages[i].Leave += color_Leave;
                DataGridView dataGrid = new DataGridView();
                dataGrid.Parent = opanel;
                dataGrid.Width = opanel.Width;
                dataGrid.Height = opanel.Height - 40;
                Label oTotal = new Label();
                oTotal.Parent = opanel;
                oTotal.Left = 10;
                oTotal.Top = dataGrid.Height + 5;
                oTotal.Height = oTotal.Height + 10;
                oTotal.Text = "TOTAL";
                oTotal.AutoSize = true;
                //dataGrid.Dock = DockStyle.Fill;
                /* dataGrid.Width = dataGrid.Parent.Size.Width - 40;
                 dataGrid.Height = dataGrid.Parent.Size.Height - 40;
                 dataGrid.Left = 20;
                 dataGrid.Top = 20;*/
                tcMeses.TabPages[i].Padding = new Padding(10);
                BindingSource binding = new BindingSource();
                binding.DataSource = dsInsumos.Tables[i].AsDataView();
                lstMonteGrid[i].oDataGridView = dataGrid;
                lstMonteGrid[i].oDataGridView.DataSource = binding;
                lstMonteGrid[i].ConfigureDBGridView();
                dataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
                // dataGrid.Dock = DockStyle.Fill;
                //dataGrid.Location = new Point(10, 10);
                DateTime fim_mes = fim.AddMonths(i);
                double? debito = 0;
                double? credito = 0;
                try
                {
                    debito = dsInsumos.Tables[i].AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                  //&& (a.Field<Int64>("FORMATO") == 0)
                  && (a.Field<string>("DEBITO").Trim() != "")
                     ).Sum(a => a.Field<double?>("VALOR")) ?? 0;

                }
                catch (Exception)
                {
                }
                try { 
                credito = dsInsumos.Tables[i].AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                   // && (a.Field<Int64>("FORMATO") == 0)
                    && (a.Field<string>("CREDITO").Trim() != "")
                    ).Sum(a => (double?)a.Field<double?>("VALOR")) ?? 0;
                }
                catch (Exception)
                {
                }
                oTotal.Text = "TOTAIS => " + String.Format(" Debito: {0,12:###,###,##0.00}", Convert.ToDecimal(debito));
                oTotal.Text = oTotal.Text + String.Format("    Credito: {0,12:###,###,##0.00}", Convert.ToDecimal(credito));
            }
            tcMeses.SelectedIndex = 0;
        }




        private void color_Enter(object sender, EventArgs e)
        {
            (sender as Control).BackColor = SystemColors.ActiveCaption;

        }

        private void color_Leave(object sender, EventArgs e)
        {
            (sender as Control).BackColor = SystemColors.Control;


        }

        private void rbProvFolha_CheckedChanged(object sender, EventArgs e)
        {
            if (((sender as RadioButton).Checked) && (dsFolhas.Tables.Count > 0))
            {
                MonteGrids();
            }
                

        }

        private void rbSaidasAlmox_CheckedChanged(object sender, EventArgs e)
        {
            if (((sender as RadioButton).Checked) && (dsInsumos.Tables.Count > 0))
            {
                MonteGrids_Insumos();
            }

        }
        private void btnTransfere_Click(object sender, EventArgs e)
        {
            bool erro = false;
            if (rbProvFolha.Checked)
            {
                if (dsFolhas.Tables.Count == 0) { MessageBox.Show("Sem Lançamentos"); return; }
                DataTable AnoMesTable = dsFolhas.Tables[0];
                if (AnoMesTable.Rows.Count == 0) { MessageBox.Show("Sem Lançamentos"); return; }
                DataRow orow = AnoMesTable.Rows[0];
                string ano = Convert.ToDateTime(orow["DATA"]).Year.ToString();
                string mes = Convert.ToDateTime(orow["DATA"]).Month.ToString().PadLeft(2, Convert.ToChar("0"));

                string codicaodelete = " WHERE(doc = 'SIST_RURAL NW ') AND(SUBSTR(DTOS(data), 1, 6) = '" + ano + mes + "') ";
                if (dsFolhas.Tables.Count == 0) erro = false;
                else erro = TranfereDataSet(dsFolhas, codicaodelete);
            }
            else if (rbSaidasAlmox.Checked)
            {
                if (dsInsumos.Tables.Count == 0) { MessageBox.Show("Sem Lançamentos"); return; }
                DataTable AnoMesTable = dsInsumos.Tables[0];
                if (AnoMesTable.Rows.Count == 0) { MessageBox.Show("Sem Lançamentos"); return; }
                DataRow orow = AnoMesTable.Rows[0];
                string ano = Convert.ToDateTime(orow["DATA"]).Year.ToString();
                string mes = Convert.ToDateTime(orow["DATA"]).Month.ToString().PadLeft(2, Convert.ToChar("0"));


                string codicaodelete = " WHERE(doc = 'SIST_ALMOX NW ') AND(SUBSTR(DTOS(data), 1, 6) = '" + ano + mes + "') ";
                if (dsInsumos.Tables.Count == 0) erro = false;
                else   erro = TranfereDataSet(dsInsumos, codicaodelete);
            }
            if (!erro) MessageBox.Show("Sem Registros para Transferir");
            else MessageBox.Show("Transferencia Ok!");
        }

        private bool TranfereDataSet(DataSet dsTransfere, string condicao_delete )
        {
            bool retorno = false;
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");

            string tabela = "MOVFIN.DBF";
            
            // pega maior id
            Decimal mov_id = 0;
            OleDbCommand command_mov_id = new OleDbCommand(
                      "SELECT MAX(MOV_ID) max_movId FROM " + path + tabela +
                      "",
                      TDataControlReduzido.ConnectionPooling.GetConnectionOleDb()); ;
            try
            {
                mov_id = Convert.ToDecimal(command_mov_id.ExecuteScalar());
            }
            catch (Exception)
            {
                MessageBox.Show("Erro Pesquisar MAX(MOV_ID) " + tabela);
                return retorno;

            }

            foreach (DataTable otable in dsTransfere.Tables)
            {
                DataTable AnoMesTable = otable;
                if (AnoMesTable.Rows.Count == 0) { MessageBox.Show("Sem Lançamentos"); return retorno; }
                DataRow orow = AnoMesTable.Rows[0];
                string ano = Convert.ToDateTime(orow["DATA"]).Year.ToString();
                string mes = Convert.ToDateTime(orow["DATA"]).Month.ToString().PadLeft(2, Convert.ToChar("0"));


                // if (naoePadrao == false) { return; }
                OleDbCommand command = new OleDbCommand(
                          "DELETE FROM " + path + tabela +
                          condicao_delete,
                          //" WHERE(doc = 'SIST_RURAL NW ') AND(SUBSTR(DTOS(data), 1, 6) = '" + ano + mes + "') ",
                          TDataControlReduzido.ConnectionPooling.GetConnectionOleDb()); ;
                try
                {
                    command.ExecuteScalar();
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao esvaziar tabela " + tabela + "AnoMes" + ano + mes);
                    return retorno;
                }

                try
                {
                    DataSet ptdataset = new DataSet();
                    setoroledb = "SELECT *  FROM " + path + tabela + " WHERE MOV_ID = NULL";
                    oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    OleDbDataAdapter oledbda = TDataControlContab.GravePtMovFin(tabela);
                    oledbda.SelectCommand = oledbcomm;
                    oledbda.TableMappings.Add("Table", tabela);
                    oledbda.Fill(ptdataset);

                    DataTable tabordenada = otable;
                    foreach (DataRow row in tabordenada.Rows)
                    {
                        DataRow rowinc = ptdataset.Tables[0].NewRow();
                        TDataControlContab.PonhaValoresDefault(rowinc);
                        foreach (DataColumn origem in tabordenada.Columns)
                        {
                            if (origem.ColumnName.ToUpper().Trim() == "FORMATO") continue;

                            rowinc[origem.ColumnName] = row[origem.ColumnName];
                           
                        }
                        mov_id += 1;
                        rowinc["MOV_ID"] = mov_id;
                        ptdataset.Tables[0].Rows.Add(rowinc);
                    }
                    oledbda.Update(ptdataset.Tables[tabela]);
                    ptdataset.Tables[tabela].AcceptChanges();
                }
                catch (Exception E)
                {
                    throw new Exception(E.Message);
                }
            }
            retorno = true;
            return retorno;
        }
        

        
    }
}
