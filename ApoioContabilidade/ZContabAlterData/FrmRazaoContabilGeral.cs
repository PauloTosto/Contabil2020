using ApoioContabilidade.Fiscal.ACBR.Classes;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
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
        private Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};



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
            System.Windows.Forms.DialogResult result;// = System.Windows.Forms.DialogResult.OK;
            SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
            SaveFileDialog1.DefaultExt = "xls";
            SaveFileDialog1.AddExtension = true;

            SaveFileDialog1.CheckFileExists = false;
            result = SaveFileDialog1.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Cursor.Current = Cursors.WaitCursor;
                Microsoft.Office.Interop.Excel.Application ExcelObj = new Microsoft.Office.Interop.Excel.Application();

                try
                {
                    if (ExcelObj == null)
                    {
                        MessageBox.Show("ERROR: EXCEL não pôde ser iniciado!");
                        return;
                    }
                    // ExcelObj.Visible = true;

                    Microsoft.Office.Interop.Excel.Workbook oWorkbook =
                        ExcelObj.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);


                   // Microsoft.Office.Interop.Excel.Range Oget_Range;


                    try
                    {

                        int linhasinicio = 1;
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                        oworksheet.Activate();
                        oworksheet.Name = "Planilha1";
                        

                        foreach (DataGridViewRow orowdgv in dgvBalancete.Rows)
                        {

                            //var orowdgv = dgvBalancete.CurrentRow;
                            DataRowView orow = (orowdgv.DataBoundItem as DataRowView);
                            if (orow != null)
                            {
                                string conta = orow["NUMCONTA"].ToString();
                                

                                if (conta.Substring(5, 3) == "000")
                                    continue;
                                string descri = orow["DESCBAL"].ToString();
                                decimal sdoant = Convert.ToDecimal(orow["SDOANT"]);
                                DateTime inicio = Convert.ToDateTime(orow["INICIO"]);
                                DateTime fim = Convert.ToDateTime(orow["FIM"]);


                                dtRazao = Reload(conta,inicio,sdoant);
                                if (dtRazao.Rows.Count < 2)
                                { continue; }
                                oRazao = new MonteGrid();
                                MonteGrids();

                                oRazao.oDataGridView = dgvRazao;
                                //oBalancete.sbTotal = sbBalancete

                                bmSource = new BindingSource();
                                bmSource.DataSource = dtRazao;
                                ///bmSourceSaida.DataSource = dvSaidas;
                                dgvRazao.DataSource = bmSource;
                                oRazao.ConfigureDBGridView();
                                string odatamember = "";
                                DataSet odataset = null;
                                DataView odataview = null;
                                if (oRazao.oDataGridView == null) continue;
                                if (oRazao.oDataGridView.DataSource is BindingSource)
                                {
                                    if (((BindingSource)oRazao.oDataGridView.DataSource).DataSource is DataSet)
                                    {
                                        odataset = (DataSet)((BindingSource)oRazao.oDataGridView.DataSource).DataSource;
                                        odatamember = ((BindingSource)oRazao.oDataGridView.DataSource).DataMember;
                                    }
                                    else
                                    {
                                        if (((BindingSource)oRazao.oDataGridView.DataSource).DataSource is DataView)
                                        {
                                            odataview = (DataView)((BindingSource)oRazao.oDataGridView.DataSource).DataSource;
                                            odatamember = odataview.Table.TableName;
                                        }
                                        else if (((BindingSource)oRazao.oDataGridView.DataSource).DataSource is System.Data.DataTable)
                                        {
                                            odataview = (((BindingSource)oRazao.oDataGridView.DataSource).DataSource as System.Data.DataTable).DefaultView;
                                            odatamember = odataview.Table.TableName;
                                        }
                                    }
                                }
                                else
                                {
                                    odataset = (DataSet)oRazao.oDataGridView.DataSource;
                                    odatamember = oRazao.oDataGridView.DataMember;
                                }
                                try
                                {
                                    lbDescricao.Text = descri;
                                    Cursor.Current = Cursors.WaitCursor;
                                    linhasinicio = CorpoExcel(oWorkbook, oworksheet, odataview, descri, linhasinicio);
                                    linhasinicio = linhasinicio + 3;
                                }
                                catch (Exception E)
                                {
                                    MessageBox.Show(E.Message);
                                    
                                }
                                
                            }


                            
                        }
                       oWorkbook.SaveAs(SaveFileDialog1.FileName, Microsoft.Office.Interop.Excel.XlFileFormat.xlWorkbookNormal,
                      "", "", false, false, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlShared,
                       Microsoft.Office.Interop.Excel.XlSaveConflictResolution.xlUserResolution, true, "", "", false);

                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Erro ao tentar abrir tabela Excel ", "",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk);
                        return;
                    }
                }
                finally
                {
                    //oWor;
                    if (ExcelObj != null)
                    {
                        ExcelObj.Quit();
                        MessageBox.Show("Excel " + SaveFileDialog1.FileName + " Gravado Ok");
                    }

                    Cursor.Current = Cursors.Default;
                }
            }



            // oRazao.ExportaExcel();
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

        


        private Microsoft.Office.Interop.Excel.Range Colunas_exc(string colunas, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            coluna1 = colunas.Trim();
            return oworksheet.get_Range(coluna1 + "1", coluna1 + "65536");
        }

        private Microsoft.Office.Interop.Excel.Range Colunas_exc(string Coluna1, string Coluna2, int Linha1, int Linha2, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            string coluna2 = "";
            coluna1 = Coluna1.Trim();
            coluna2 = Coluna2.Trim();
            return oworksheet.get_Range(coluna1 + Linha1.ToString().Trim(), coluna2 + Linha2.ToString().Trim());
        }

        
        private int CorpoExcel(Microsoft.Office.Interop.Excel.Workbook oWorkbook ,
            Microsoft.Office.Interop.Excel._Worksheet oworksheet,
            DataView odataview, string tituloExcel, int linhasInicioExcel)
        {

           Microsoft.Office.Interop.Excel.Range Oget_Range;


            int linhasinicio = linhasInicioExcel;
           
            // define linha max
            int linhasmax = linhasinicio + odataview.Count;

            //titulo
            int linhas = linhasinicio;
            int numcol = 0;
            string linhastr = linhas.ToString();

            if (tituloExcel != "")
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = tituloExcel;


            numcol = 0;
            linhas = linhas + 1;
            linhastr = linhas.ToString();

            // cabeçalho
            Type tipo = null;
            foreach (Campo campo in oRazao.LinhasCampo)
            {
                Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                          (linhas + 1), linhasmax, oworksheet);
                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
                Oget_Range.ColumnWidth = campo.tamanho;
                if (odataview.Table.Columns.Contains(campo.titulo)) { tipo = odataview.Table.Columns[campo.titulo].DataType; }
                if (tipo != null)
                {
                    if ((tipo != Type.GetType("System.String")) && (tipo != Type.GetType("System.Boolean")) && (tipo != Type.GetType("System.DateTime")))
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > Convert.ToInt32(Oget_Range.ColumnWidth))
                            Oget_Range.ColumnWidth = tam;
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                    }
                    if ((tipo == Type.GetType("System.DateTime")) && (campo.format != ""))
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > Convert.ToInt32(Oget_Range.ColumnWidth))
                            Oget_Range.ColumnWidth = tam;
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                    }
                }
                else
                {
                    if (campo.format != "")
                    {
                        int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                        if (tam > Oget_Range.ColumnWidth)
                            Oget_Range.ColumnWidth = tam;
                    }
                }
                numcol += 1;
            }


            linhas += 1;
            foreach (DataRowView orow in odataview)
            {
                numcol = 0;
                linhastr = linhas.ToString();
                foreach (Campo campo in oRazao.LinhasCampo)
                {
                    object dado = "";
                    try
                    {
                        tipo = orow[campo.titulo].GetType();
                        if (tipo == Type.GetType("System.DateTime"))
                        {
                            if (campo.format != "")
                            {
                                dado = orow[campo.titulo];
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                            }
                            else { 
                                dado = Convert.ToDateTime(orow[campo.titulo]);
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "DD/MM/YYY";
                            }

                        }
                        else if ((tipo != Type.GetType("System.String")) && (tipo != Type.GetType("System.Boolean"))) // numerico
                        {
                            if (campo.format != "")
                            {
                                if (tipo == Type.GetType("System.Decimal") || tipo == Type.GetType("System.Double") || tipo == Type.GetType("System.Single"))
                                { dado = orow[campo.titulo]; }
                                else
                                { dado = orow[campo.titulo]; }
                            }
                            else { dado = orow[campo.titulo]; }
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = campo.format;
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                        }
                        else
                            dado = orow[campo.titulo].ToString();
                    }
                    catch
                    { }

                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = dado;
                    numcol += 1;
                }
                linhas += 1;
            }

            int linhasretorno = linhas;

            linhas = linhasinicio;
            numcol = 0;
            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol + (oRazao.LinhasCampo.Count - 1)].ToString(),
                 linhas, (linhasmax+1), oworksheet);
            Oget_Range.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
            return linhasretorno;

        }

       
    }
}
