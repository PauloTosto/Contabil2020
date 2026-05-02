using ApoioContabilidade.Fiscal.ACBR.Classes;
using ClassFiltroEdite;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.ZContabAlterData
{
    public partial class FrmRazaoContabilGeral : Form
    {

        private MonteGrid oRazao;

        public int LinhasMinimas = 3;



        //public string tituloExcel = "";
        public int linhasInicioExcel = 4;
        public int colunaCabecalhoExcel = 0;
        

        DataTable dtVauchesRazao;
        DataGridView dgvBalancete;
        DataTable dtRazao;
        public BindingSource bmSource;
       
        /*
            var orowdgv = dgvBalancete.CurrentRow;
            DataRowView orow = (orowdgv.DataBoundItem as DataRowView );
            if (orow != null)
            {
                string conta = orow["NUMCONTA"].ToString();
                if (conta.Substring(5, 3) == "000")
                    return;
                string descri = orow["DESCBAL"].ToString();
                decimal sdoant = Convert.ToDecimal(orow["SDOANT"]);
                DateTime inicio = Convert.ToDateTime(orow["INICIO"]);
                DateTime fim = Convert.ToDateTime(orow["FIM"]);
            
         */

        public FrmRazaoContabilGeral(DataGridView odgvBalancete,
            DataTable dtVaucheRazao)
        {
           // oRazao.Da
           dgvBalancete= odgvBalancete;
            dtVauchesRazao = dtVaucheRazao;
           /* conta = oconta;
            descricao = odescricao;
            sdoant = osdoant;
            inicioPer = oinicioPer;
            fimPer = oFimPer;*/
            InitializeComponent();
            btnExcel.Enabled = true;
            //lbDescricao.Text = odescricao;

            

        }
        private DataTable Reload(string conta,DateTime inicioPer, decimal sdoant)
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
                orowB["DOC_FISC"] = "";
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
                      .ThenBy(a => (a.Field<string>("DEBITO") == conta ? 0 : 1))
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

      

        private void btnExcel_Click(object sender, EventArgs e)
        {
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.DefaultExt = "xlsx";
            SaveFileDialog1.Filter = "Excel Workbook (*.xlsx)|*.xlsx";
            SaveFileDialog1.AddExtension = true;
            SaveFileDialog1.CheckFileExists = false;

            if (SaveFileDialog1.ShowDialog() != DialogResult.OK) return;

            Cursor.Current = Cursors.WaitCursor;
            string filename = SaveFileDialog1.FileName;
            if (!filename.ToLower().EndsWith(".xlsx")) filename += ".xlsx";

            try
            {
                FileInfo fi = new FileInfo(filename);
                if (fi.Exists) fi.Delete();

                using (var pkg = new ExcelPackage(fi))
                {
                    var ws = pkg.Workbook.Worksheets.Add("Planilha1");

                    int linhasinicio = 1;
                    foreach (DataGridViewRow orowdgv in dgvBalancete.Rows)
                    {
                        DataRowView orow = (orowdgv.DataBoundItem as DataRowView);
                        if (orow == null) continue;

                        string conta = orow["NUMCONTA"].ToString();
                        if (conta.Substring(5, 3) == "000") continue;

                        string descri = orow["DESCBAL"].ToString();
                        decimal sdoant = Convert.ToDecimal(orow["SDOANT"]);
                        DateTime inicio = Convert.ToDateTime(orow["INICIO"]);
                        DateTime fim = Convert.ToDateTime(orow["FIM"]);

                        dtRazao = Reload(conta, inicio, sdoant);
                        if (dtRazao.Rows.Count < 2) continue;

                        oRazao = new MonteGrid();
                        MonteGrids();
                        oRazao.oDataGridView = dgvRazao;

                        bmSource = new BindingSource();
                        bmSource.DataSource = dtRazao;
                        dgvRazao.DataSource = bmSource;
                        oRazao.ConfigureDBGridView();

                        // Extrai DataView do binding (preserva logica original)
                        DataView odataview = null;
                        if (oRazao.oDataGridView == null) continue;
                        if (oRazao.oDataGridView.DataSource is BindingSource)
                        {
                            var bs = (BindingSource)oRazao.oDataGridView.DataSource;
                            if (bs.DataSource is DataView)
                            {
                                odataview = (DataView)bs.DataSource;
                            }
                            else if (bs.DataSource is System.Data.DataTable)
                            {
                                odataview = ((System.Data.DataTable)bs.DataSource).DefaultView;
                            }
                        }
                        if (odataview == null) continue;

                        try
                        {
                            lbDescricao.Text = descri;
                            Cursor.Current = Cursors.WaitCursor;
                            linhasinicio = CorpoExcel(ws, odataview, descri, linhasinicio);
                            linhasinicio += 3;
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show(E.Message);
                        }
                    }

                    pkg.Save();
                }

                MessageBox.Show("Excel " + filename + " gravado OK.");
            }
            catch (Exception E)
            {
                MessageBox.Show("Erro ao gerar planilha Excel: " + E.Message, "",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }

        private void MonteGrids()
        {
            oRazao.Clear();

            oRazao.AddValores("DATA", "Data", 10, "", false, 0, "");
            oRazao.AddValores("VALORDEB", "Debito(R$)", 12, "###,###,##0.00", true, 0, "");
            oRazao.AddValores("VALORCRE", "Credito(R$)", 12, "###,###,##0.00", false, 0, "");
            oRazao.AddValores("SDO", "SALDO(R$)", 12, "###,###,##0.00", true, 0, "");
            oRazao.AddValores("DOC_FISC", "Doc.Fiscal", 15, "", false, 0, "");
            oRazao.AddValores("DOC", "Doc.", 15, "", false, 0, "");
            oRazao.AddValores("CONTRAPARTIDA", "ContraPartida", 12, "", false, 0, "");
            oRazao.AddValores("HIST", "Histórico", 45, "", false, 0, "");


        }

        


        private int CorpoExcel(ExcelWorksheet ws, DataView odataview, string tituloExcel, int linhasInicioExcel)
        {
            int linhasinicio = linhasInicioExcel;
            int linhasmax = linhasinicio + odataview.Count;

            // Linha de titulo
            int linha = linhasinicio;
            int col = 1;
            if (tituloExcel != "")
                ws.Cells[linha, col].Value = tituloExcel;

            // Linha de cabecalho
            linha += 1;
            col = 1;
            Type tipo = null;
            foreach (Campo campo in oRazao.LinhasCampo)
            {
                ws.Cells[linha, col].Value = campo.cabecalho;
                ws.Column(col).Width = campo.tamanho;
                if (odataview.Table.Columns.Contains(campo.titulo))
                    tipo = odataview.Table.Columns[campo.titulo].DataType;
                else
                    tipo = null;

                if (tipo != null)
                {
                    if (tipo != typeof(string) && tipo != typeof(bool) && tipo != typeof(DateTime))
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > ws.Column(col).Width) ws.Column(col).Width = tam;
                        ws.Cells[linha, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    }
                    if (tipo == typeof(DateTime) && campo.format != "")
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > ws.Column(col).Width) ws.Column(col).Width = tam;
                        ws.Cells[linha, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }
                else if (campo.format != "")
                {
                    int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                    if (tam > ws.Column(col).Width) ws.Column(col).Width = tam;
                }
                col += 1;
            }

            // Linhas de dados
            linha += 1;
            foreach (DataRowView orow in odataview)
            {
                col = 1;
                foreach (Campo campo in oRazao.LinhasCampo)
                {
                    object dado = "";
                    try
                    {
                        tipo = orow[campo.titulo].GetType();
                        if (tipo == typeof(DateTime))
                        {
                            dado = orow[campo.titulo];
                            ws.Cells[linha, col].Style.Numberformat.Format =
                                campo.format != "" ? campo.format : "DD/MM/YYY";
                        }
                        else if (tipo != typeof(string) && tipo != typeof(bool)) // numerico
                        {
                            dado = orow[campo.titulo];
                            ws.Cells[linha, col].Style.Numberformat.Format = campo.format;
                            ws.Cells[linha, col].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                        }
                        else
                        {
                            dado = orow[campo.titulo].ToString();
                        }
                    }
                    catch { }

                    ws.Cells[linha, col].Value = dado;
                    col += 1;
                }
                linha += 1;
            }

            int linhasretorno = linha;

            // Grade completa (replica Range.Borders.LineStyle = xlContinuous do Excel Interop,
            // que aplicava bordas em todas as celulas, nao apenas as externas).
            int colFim = oRazao.LinhasCampo.Count;
            var rangeGrade = ws.Cells[linhasinicio, 1, linhasmax + 1, colFim];
            rangeGrade.Style.Border.Top.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Left.Style = ExcelBorderStyle.Thin;
            rangeGrade.Style.Border.Right.Style = ExcelBorderStyle.Thin;
            return linhasretorno;
        }

       
    }
}
