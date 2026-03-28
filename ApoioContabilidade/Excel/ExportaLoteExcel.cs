using ClassConexao;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Office.Interop.Excel;
namespace ApoioContabilidade.Excel
{
    public static class ExportaLoteExcel
    {

        static private Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};

        static private Microsoft.Office.Interop.Excel.Range Colunas_exc(string colunas, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            coluna1 = colunas.Trim();
            return oworksheet.get_Range(coluna1 + "1", coluna1 + "65536");
        }

        static private Microsoft.Office.Interop.Excel.Range Colunas_exc(string Coluna1, string Coluna2, int Linha1, int Linha2, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            string coluna2 = "";
            coluna1 = Coluna1.Trim();
            coluna2 = Coluna2.Trim();
            return oworksheet.get_Range(coluna1 + Linha1.ToString().Trim(), coluna2 + Linha2.ToString().Trim());
        }


        static public void ExportaExcelLote(System.Data.DataTable VauchesRazao)
        {


            /*Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};*/
            //  Int16[] Colunas = new Int16[21] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 ,21};
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

                    Microsoft.Office.Interop.Excel.Range Oget_Range;
                    try
                    {
                        int linhasinicio = 1;
                        Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Worksheets;

                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (Microsoft.Office.Interop.Excel.Worksheet)sheets.get_Item(1);

                        oworksheet.Activate();
                        oworksheet.Name = "Planilha1";

                        // define linha max
                        // int linhasmax = linhasinicio + odataview.Count;

                        //titulo
                        int linhas = 1;
                        int numcol = 0;
                        string linhastr = linhas.ToString();

                        DataSet dsrelaciona = TDataControlContab.TabRelaciona();
                        System.Data.DataTable tabrelaciona = dsrelaciona.Tables[0];


                        System.Data.DataTable tabordenada = VauchesRazao;
                        int linhasmax = linhasinicio + tabordenada.Rows.Count;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                      (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "lancto auto";
                        Oget_Range.ColumnWidth = 11;

                        numcol = 1;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "debito";
                        Oget_Range.ColumnWidth = 14;

                        numcol = 2;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "credito";
                        Oget_Range.ColumnWidth = 14;

                        numcol = 3;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "data";
                        // oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "dd/MM/yyyy";
                        Oget_Range.ColumnWidth = 12;


                        numcol = 4;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "valor";
                        //oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "###,##0.00";
                        Oget_Range.ColumnWidth = 14;

                        numcol = 5;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "cód histórico";
                        Oget_Range.ColumnWidth = 8;

                        numcol = 6;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "complemento historico";
                        Oget_Range.ColumnWidth = 45;

                        numcol = 7;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "Ccusto debito";
                        Oget_Range.ColumnWidth = 8;

                        numcol = 8;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "Ccusto credito";
                        Oget_Range.ColumnWidth = 8;

                        numcol = 9;
                        Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                     (linhas + 1), linhasmax, oworksheet);
                        oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = "NrDocumento";
                        Oget_Range.ColumnWidth = 20;

                      

                        linhas += 1;


                        try
                        {
                            foreach (DataRow linexc in tabordenada.Rows)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                            {



                                if (((Convert.IsDBNull(linexc["DEBITO"])) || (Convert.ToString(linexc["DEBITO"]).Trim() == ""))
                                    && ((Convert.IsDBNull(linexc["CREDITO"])) || (Convert.ToString(linexc["CREDITO"]).Trim() == "")))
                                    continue;

                                string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));
                                string debito = linexc["DEBITO"].ToString();
                                string credito = linexc["CREDITO"].ToString();
                                if ((credito.Trim() == "") && (debito.Trim() == "")) continue;
                                
                                string tdoc = "";
                                if (Convert.ToString(linexc["DOC"]).Trim() != "")
                                    tdoc = Convert.ToString(linexc["DOC"]).Trim();
                                else
                                    if (Convert.ToString(linexc["DOC_FISC"]).Trim() != "")
                                    tdoc = Convert.ToString(linexc["DOC_FISC"]).Trim();
                                if (tdoc != "")
                                    tdoc = "DOC.n.:" + tdoc;
                                string thist = "";
                                if (Convert.ToString(linexc["HIST"]).Trim() != "")
                                    thist = Convert.ToString(linexc["HIST"]).Trim();
                                linhastr = linhas.ToString();
                                numcol = 1; // debito
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = debito;

                                numcol = 2; // credito
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = credito;

                                numcol = 3; // data
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "dd/MM/yyyy";
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = Convert.ToDateTime(linexc["DATA"]);

                                numcol = 4; // valor
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "###,###,##0.00";
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = 
                                    Math.Round(Convert.ToDecimal(linexc["VALOR"]),2);

                                numcol = 5; // cod hist
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "##0";
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = 98;


                                numcol = 6; // historico
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = thist;
                                numcol = 9; // Documento
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = tdoc;
                                linhas += 1;
                            }

                        }
                        catch (Exception)
                        {
                            throw;
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
        }
    }
}
