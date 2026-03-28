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
using static ApoioContabilidade.ZContabAlterData.Utils.BalanceteClass;

namespace ApoioContabilidade.ZContabAlterData
{
    public partial class FrmRazaoContabil : Form
    {
        private MonteGrid oRazao;

        DataTable dtVauchesRazao;
        DataTable dtRazao;
        public BindingSource bmSource;
        string conta, descricao;
        
        decimal sdoant;
        DateTime inicioPer, fimPer;
        public FrmRazaoContabil(DataTable dtVaucheRazao, string oconta, string odescricao, decimal osdoant, DateTime oinicioPer, DateTime oFimPer)
        {
            dtVauchesRazao = dtVaucheRazao;
            conta = oconta;
            descricao = odescricao;
            sdoant = osdoant;
            inicioPer = oinicioPer;
            fimPer = oFimPer;
            InitializeComponent();
            btnExcel.Enabled = true;
            lbDescricao.Text = odescricao;

            dtRazao = Reload();
            oRazao = new MonteGrid();
            MonteGrids();

            oRazao.oDataGridView = dgvRazao;
            //oBalancete.sbTotal = sbBalancete

            bmSource = new BindingSource();
            bmSource.DataSource = dtRazao;
            ///bmSourceSaida.DataSource = dvSaidas;
            dgvRazao.DataSource = bmSource;

            oRazao.ConfigureDBGridView();


        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            oRazao.tituloExcel = lbDescricao.Text;
            // altera o comportamento em 01 de março de 2023
            oRazao.linhasInicioExcel = 2;
            oRazao.colunaCabecalhoExcel = 1;
            oRazao.ExportaExcel();
        }

        private void MonteGrids()
        {
            oRazao.Clear();

            oRazao.AddValores("DATA", "Data", 10, "", false, 0, "");
            oRazao.AddValores("SDO", "SALDO(R$)", 12, "###,###,##0.00", true, 0, "");
            oRazao.AddValores("VALORDEB", "Debito(R$)", 12, "###,###,##0.00", true, 0, "");
            oRazao.AddValores("VALORCRE", "Credito(R$)", 12, "###,###,##0.00", false, 0, "");
            oRazao.AddValores("DOC_FISC", "Doc.Fiscal", 15, "", false, 0, "");
            oRazao.AddValores("DOC", "Doc.", 15, "", false, 0, "");
            oRazao.AddValores("CONTRAPARTIDA", "ContraPartida", 12, "", false, 0, "");
            oRazao.AddValores("HIST", "Histórico", 45, "", false, 0, "");
            

        }

        private DataTable Reload()
        {
            Cursor.Current = Cursors.WaitCursor;

            DataTable dtRazao = new DataTable();
            try
            {
                
                dtRazao.TableName = "RAZAO";
                // dtRazao.Columns.Add("NUMCONTA", Type.GetType("System.String"));
                // dtRazao.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                dtRazao.Columns.Add("DATA", Type.GetType("System.DateTime"));
                dtRazao.Columns.Add("SDO", Type.GetType("System.Decimal"));
                dtRazao.Columns.Add("VALORDEB", Type.GetType("System.Decimal"));
                dtRazao.Columns.Add("VALORCRE", Type.GetType("System.Decimal"));
                dtRazao.Columns.Add("HIST", Type.GetType("System.String"));
                dtRazao.Columns.Add("DOC_FISC", Type.GetType("System.String"));
                dtRazao.Columns.Add("DOC", Type.GetType("System.String"));
                dtRazao.Columns.Add("CONTRAPARTIDA", Type.GetType("System.String"));
                decimal sdoAtual = sdoant;
                DataRow orowB = dtRazao.NewRow();
                orowB["DATA"] = inicioPer.AddDays(-1);
                orowB["HIST"] = "SALDO ANTERIOR";
                orowB["DOC"] = "";
                orowB["DOC_FISC"] ="";
                sdoAtual = sdoant;
                orowB["VALORCRE"] = 0;
                orowB["VALORDEB"] = 0;
                orowB["SDO"] = sdoAtual;
                orowB["CONTRAPARTIDA"] = "";
                dtRazao.Rows.Add(orowB);
                orowB.AcceptChanges();




                try
                {

                    foreach (var dataRow in dtVauchesRazao.AsEnumerable().Where(a => (a.Field<string>("DEBITO") == conta)
                    || (a.Field<string>("CREDITO") == conta)).OrderBy(a => a.Field<DateTime>("DATA"))
                      .ThenBy(a => (a.Field<string>("DEBITO") == conta ? 0:1))
                      .ThenBy(a => a.Field<string>("DOC_FISC")).ThenBy(a => a.Field<string>("DOC"))
                         )
                    {

                        // if (dataRow["NUMCONTA"].ToString() == "00000000") continue;

                        orowB = dtRazao.NewRow();
                        orowB["DATA"] = dataRow["DATA"];
                        orowB["HIST"] = dataRow["HIST"];
                        orowB["DOC"] = dataRow["DOC"];
                        orowB["DOC_FISC"] = dataRow["DOC_FISC"];
                        if (dataRow["DEBITO"].ToString().Trim() == conta)
                        {
                            sdoAtual = sdoAtual + Convert.ToDecimal(dataRow["VALOR"]);
                            orowB["VALORCRE"] = 0;
                            orowB["VALORDEB"] = dataRow["VALOR"];
                            orowB["SDO"] = sdoAtual;
                            orowB["CONTRAPARTIDA"] = dataRow["CREDITO"];
                        }
                        else
                        {
                            sdoAtual = sdoAtual - Convert.ToDecimal(dataRow["VALOR"]);
                            orowB["VALORCRE"] = dataRow["VALOR"];
                            orowB["VALORDEB"] = 0;
                            orowB["SDO"] = sdoAtual;
                            orowB["CONTRAPARTIDA"] = dataRow["DEBITO"];
                        }

                        dtRazao.Rows.Add(orowB);
                        orowB.AcceptChanges();
                    }

                    dtRazao.AcceptChanges();
                }
                catch (Exception E)
                {

                    MessageBox.Show(E.Message);
                }

            }
            catch (Exception)
            {

                MessageBox.Show("Erro Geração Razao");
            }
            Cursor.Current = Cursors.Default;
            return dtRazao;

        }

    }
}
