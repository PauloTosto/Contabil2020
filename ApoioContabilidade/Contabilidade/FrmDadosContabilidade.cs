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
using System.Windows.Forms;

namespace ApoioContabilidade.Contabilidade
{
    public partial class FrmDadosContabilidade : Form
    {
        DataTable movInsere;
        OleDbCommand oledbcomm;
        OleDbDataAdapter oledbda;
        DataSet ptdataset;
        public FrmDadosContabilidade()
        {
            InitializeComponent();
            dtData1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            dtData2.Value = dtData1.Value.AddMonths(1).AddDays(-1);
            btnTransfere.Enabled = false;
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            tcMeses.TabPages.Clear();
            if (dtData1.Value.CompareTo(dtData2.Value) > 0)
            {
                return;
            }
            btnTransfere.Enabled = false;

            movInsere = new DataTable();

            if (rbInserirRegistros.Checked)
            {
                DataSet dsResult = null;
                List<string> lstquery = new List<string>();
                lstquery.Add("dataDe=" + dtData1.Value.ToString("yyyy-MM-dd"));
                lstquery.Add("dataAte=" + dtData2.Value.ToString("yyyy-MM-dd"));
                lstquery.Add("sp_numero=" + "601"); //  [dbo].[Finan_Movs

               
                try
                {
                    dsResult = await ApiServices.Api_QuerySP(lstquery);
                }
                catch (Exception)
                {
                }
                if (dsResult != null)
                {
                    if (dsResult.Tables.Count > 0)
                    {
                        movInsere = dsResult.Tables[0];
                        dataGridView1.DataSource = movInsere;
                        dataGridView1.DefaultCellStyle.ForeColor = Color.Blue;
                    }
                }
            }
            else if (rbDeletarRegistros.Checked)
            {
                string path = TDataControlReduzido.Get_Path("CONTAB");
                string tabela = "MOVFIN.DBF";
                string setoroledb = "SELECT Data, valor, debito, credito, tipo, tp_fin, doc, hist, tipo_doc, forn, venc, doc_dupl,"
                     + "nhist, doc_fisc, emissor, data_emi, obs, mov_id, outro_id " +
                      "from movfin where DATA BETWEEN CTOD('"+ dtData1.Value.ToString("MM/dd/yyyy")+
                      "') AND " + " CTOD('"+ dtData2.Value.ToString("MM/dd/yyyy") + "')";
                // delete 
                string strdelete = " DELETE FROM movfin " +
                    "WHERE(mov_id IN " +
                    " (SELECT  mov_id " +
                   "from movfin where DATA BETWEEN CTOD('" + dtData1.Value.ToString("MM/dd/yyyy") +
                   "') AND " + " CTOD('" + dtData2.Value.ToString("MM/dd/yyyy") + "')  ) )";

                ptdataset = new DataSet();
                try
                {
                    oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    oledbda = TDataControlContab.GravePtMovFin(tabela);
                    oledbda.SelectCommand = oledbcomm;
                    oledbda.TableMappings.Add("Table", tabela);
                    oledbda.Fill(ptdataset);

                    // Comando de Deletar
                 
                    OleDbCommand command = new OleDbCommand(
                          strdelete,
                          TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    oledbda.DeleteCommand = command;
                }
                catch (Exception E)
                {
                }
                if (ptdataset.Tables.Count > 0)
                {
                    movInsere = ptdataset.Tables[0];
                    dataGridView1.DataSource = movInsere;
                    dataGridView1.DefaultCellStyle.ForeColor = Color.Blue;

                }
            }
            MonteGrids();
          //  tcMeses.Visible = true; 
        }

        private void MonteGrids()
        {
            tcMeses.TabPages.Clear();
            MonteGrid oEntradas = new MonteGrid();
            oEntradas.Clear();
            oEntradas.AddValores("DATA", "DATA", 0, "", false, 0, "");
            oEntradas.AddValores("DEBITO", "DEBITO", 40, "", true, 0, "");
            oEntradas.AddValores("CREDITO", "CREDITO", 40, "", true, 0, "");
            oEntradas.AddValores("VALOR", "VALOR", 12, "#,###,##0.00", true, 0, "");
            oEntradas.AddValores("TIPO", "Tp", 3, "", false, 0, "");
            oEntradas.AddValores("TP_FIN", "Fin", 4, "", false, 0, "");
            oEntradas.AddValores("HIST", "HISTORICO", 40, "", false, 0, "");
            oEntradas.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oEntradas.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
           // oEntradas.AddValores("MOV_ID", "Mov_ID", 10, "", false, 0, "");
            oEntradas.AddValores("FORN", "FORN(HIST)", 40, "", false, 0, "");
            oEntradas.AddValores("EMISSOR", "Emissor", 40, "", false, 0, "");
            oEntradas.AddValores("DATA_EMI", "Emissão", 0, "", false, 0, "");
            /*  venc, doc_dupl,"
                     + " obs, mov_id, outro_id
             */
            tcMeses.TabPages.Add(new TabPage());
            tcMeses.TabPages[0].Text = rbInserirRegistros.Checked ? "Inserir" : "Deletar";
            Panel opanel = new Panel();
            opanel.Parent = tcMeses.TabPages[0];
           
            opanel.Dock = DockStyle.Fill;
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
            tcMeses.TabPages[0].Padding = new Padding(10);
            BindingSource binding = new BindingSource();
            binding.DataSource = movInsere.AsDataView();
            oEntradas.oDataGridView = dataGrid;
            oEntradas.oDataGridView.DataSource = binding;
            oEntradas.ConfigureDBGridView();
            dataGrid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            // dataGrid.Dock = DockStyle.Fill;
            //dataGrid.Location = new Point(10, 10);
            //DateTime fim_mes = fim.AddMonths(i);
            decimal debito = 0;
            decimal credito = 0;
            if (rbInserirRegistros.Checked)
            {
                try
                {
                    debito = movInsere.AsEnumerable().Where(a =>
                     (a.Field<string>("DEBITO").Trim() != "")
                     ).Sum(a => (decimal) a.Field<double>("VALOR"));
                    credito = movInsere.AsEnumerable().Where(a =>
                    /*(a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                    && (a.Field<Int64>("FORMATO") == 2)
                    &&*/
                    (a.Field<string>("CREDITO").Trim() != "")
                    ).Sum(a => (decimal) a.Field<double>("VALOR"));
                }
                catch (Exception E)
                {
                }
            }
            else
            {
                try
                {
                    debito = movInsere.AsEnumerable().Where(a =>
                     (a.Field<string>("DEBITO").Trim() != "")
                     ).Sum(a => a.Field<decimal>("VALOR"));
                    credito = movInsere.AsEnumerable().Where(a =>
                    /*(a.Field<DateTime>("DATA").CompareTo(fim_mes) == 0)
                    && (a.Field<Int64>("FORMATO") == 2)
                    &&*/
                    (a.Field<string>("CREDITO").Trim() != "")
                    ).Sum(a => a.Field<decimal>("VALOR"));
                }
                catch (Exception E)
                {
                }
            }
            oTotal.Text = "TOTAIS => " + String.Format(" Debito: {0,12:###,###,##0.00}", Convert.ToDecimal(debito));
            oTotal.Text = oTotal.Text + String.Format("    Credito: {0,12:###,###,##0.00}", Convert.ToDecimal(credito));

            dataGrid.DefaultCellStyle.ForeColor = rbInserirRegistros.Checked ? Color.Blue : Color.Red;
            btnTransfere.Text = rbInserirRegistros.Checked ? "Incluir Registros" : "Deletar Registros";
            btnTransfere.Enabled = true;
        }

        private void btnTransfere_Click(object sender, EventArgs e)
        {
            Cursor.Current = Cursors.WaitCursor;

            if (rbDeletarRegistros.Checked)
            {
                try
                {
                    // deleta no servidor
                    oledbda.DeleteCommand.ExecuteScalar();

                    // deleta o virtual
                    foreach (DataRow orow in ptdataset.Tables[0].Rows)
                    {
                        orow.Delete();
                    }
                    //oledbda.Update(ptdataset);
                    ptdataset.Tables[0].AcceptChanges();

                }
                catch (Exception E)
                {
                }
            }
            else
            {
                try
                {
                    string path = TDataControlReduzido.Get_Path("CONTAB");
                    string tabela = "MOVFIN.DBF";
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
                        return;

                    }

                    DataSet ptdataset = new DataSet();
                    string setoroledb = "SELECT *  FROM " + path + tabela + " WHERE MOV_ID = NULL";
                    oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    OleDbDataAdapter oledbda = TDataControlContab.GravePtMovFin(tabela);
                    oledbda.SelectCommand = oledbcomm;
                    oledbda.TableMappings.Add("Table", tabela);
                    oledbda.Fill(ptdataset);

                    DataTable tabordenada = movInsere;
                    foreach (DataRow row in tabordenada.Rows)
                    {
                        DataRow rowinc = ptdataset.Tables[0].NewRow();
                        TDataControlContab.PonhaValoresDefault(rowinc);
                        foreach (DataColumn origem in tabordenada.Columns)
                        {
                            if (row.IsNull(origem.ColumnName)) continue; // não aceita valores null
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
            Cursor.Current = Cursors.Default;
            btnTransfere.Enabled = false;
        }
    }
}
