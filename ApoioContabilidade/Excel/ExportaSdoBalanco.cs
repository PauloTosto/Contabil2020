using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace ApoioContabilidade.Excel
{
    /// <summary>
    /// Export de saldo de balanco (chamado por FrmSdoPlano).
    /// Mesmo layout 10 colunas A-J que ExportaLoteExcel, exceto que tdoc
    /// vai na coluna H (Ccusto debito) em vez de J (NrDocumento) -- isso eh
    /// um comportamento pre-existente preservado, possivelmente bug intencional
    /// ou nao, mas a migracao nao muda semantica.
    /// </summary>
    public static class ExportaSdoBalanco
    {
        public static void ExportaExcelSdoBalanco(System.Data.DataTable VauchesRazao)
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

                    // Cabecalho linha 1 (mesmo layout do ExportaLoteExcel)
                    ws.Cells[1, 1].Value = "lancto auto";
                    ws.Cells[1, 2].Value = "debito";
                    ws.Cells[1, 3].Value = "credito";
                    ws.Cells[1, 4].Value = "data";
                    ws.Cells[1, 5].Value = "valor";
                    ws.Cells[1, 6].Value = "cód histórico";
                    ws.Cells[1, 7].Value = "complemento historico";
                    ws.Cells[1, 8].Value = "Ccusto debito";
                    ws.Cells[1, 9].Value = "Ccusto credito";
                    ws.Cells[1, 10].Value = "NrDocumento";

                    ws.Column(1).Width = 11;
                    ws.Column(2).Width = 14;
                    ws.Column(3).Width = 14;
                    ws.Column(4).Width = 12;
                    ws.Column(5).Width = 14;
                    ws.Column(6).Width = 8;
                    ws.Column(7).Width = 45;
                    ws.Column(8).Width = 8;
                    ws.Column(9).Width = 8;
                    ws.Column(10).Width = 20;

                    int row = 2;
                    foreach (DataRow linexc in VauchesRazao.Rows)
                    {
                        bool debitoVazio = Convert.IsDBNull(linexc["DEBITO"]) || Convert.ToString(linexc["DEBITO"]).Trim() == "";
                        bool creditoVazio = Convert.IsDBNull(linexc["CREDITO"]) || Convert.ToString(linexc["CREDITO"]).Trim() == "";
                        if (debitoVazio && creditoVazio) continue;

                        string debito = Convert.ToString(linexc["DEBITO"]);
                        string credito = Convert.ToString(linexc["CREDITO"]);

                        // Diferenca vs ExportaLoteExcel: aqui so olha DOC, nao DOC_FISC
                        string tdoc = "";
                        if (Convert.ToString(linexc["DOC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC"]).Trim();
                        if (tdoc != "") tdoc = "DOC.n.:" + tdoc;

                        string thist = "";
                        if (Convert.ToString(linexc["HIST"]).Trim() != "")
                            thist = Convert.ToString(linexc["HIST"]).Trim();

                        // Mesmo auto-convert numerico que ExportaLoteExcel para paridade visual
                        long debitoNum;
                        if (!string.IsNullOrEmpty(debito) && long.TryParse(debito, out debitoNum))
                            ws.Cells[row, 2].Value = debitoNum;
                        else
                            ws.Cells[row, 2].Value = debito;

                        long creditoNum;
                        if (!string.IsNullOrEmpty(credito) && long.TryParse(credito, out creditoNum))
                            ws.Cells[row, 3].Value = creditoNum;
                        else
                            ws.Cells[row, 3].Value = credito;

                        ws.Cells[row, 4].Value = Convert.ToDateTime(linexc["DATA"]);
                        ws.Cells[row, 4].Style.Numberformat.Format = "dd/MM/yyyy";

                        ws.Cells[row, 5].Value = Math.Round(Convert.ToDecimal(linexc["VALOR"]), 2);
                        ws.Cells[row, 5].Style.Numberformat.Format = "###,###,##0.00";

                        ws.Cells[row, 6].Value = 98;
                        ws.Cells[row, 6].Style.Numberformat.Format = "##0";

                        ws.Cells[row, 7].Value = thist;
                        // Coluna H (8) -- preservacao do comportamento original que punha tdoc aqui
                        // em vez da coluna J (NrDocumento). Pode ser bug pre-existente.
                        ws.Cells[row, 8].Value = tdoc;
                        row++;
                    }

                    pkg.Save();
                }

                MessageBox.Show("Excel " + filename + " gravado OK.");
            }
            catch (Exception E)
            {
                MessageBox.Show("Erro ao gerar planilha Excel: " + E.Message,
                    "Erro export saldo balanco", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
