using OfficeOpenXml;
using System;
using System.Data;
using System.IO;
using System.Windows.Forms;

namespace ApoioContabilidade.Excel
{
    /// <summary>
    /// Gera o arquivo XLSX no layout de import do AlterData a partir do
    /// VauchesRazao do FrmRelaciona.
    ///
    /// Layout AlterData (10 colunas A-J):
    ///   A: lancto auto                (vazio)
    ///   B: debito                     (string - codigo da conta)
    ///   C: credito                    (string - codigo da conta)
    ///   D: data                       (DateTime, formato dd/MM/yyyy)
    ///   E: valor                      (decimal, formato ###,###,##0.00)
    ///   F: cod historico              (sempre 98 - ver CLAUDE.md)
    ///   G: complemento historico      (HIST trimmed)
    ///   H: Ccusto debito              (vazio)
    ///   I: Ccusto credito             (vazio)
    ///   J: NrDocumento                (DOC ou DOC_FISC, prefixado "DOC.n.:")
    /// </summary>
    public static class ExportaLoteExcel
    {
        public static void ExportaExcelLote(System.Data.DataTable VauchesRazao)
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
                // EPPlus joga ao tentar abrir um arquivo ja existente quando se quer recriar.
                if (fi.Exists) fi.Delete();

                using (var pkg = new ExcelPackage(fi))
                {
                    var ws = pkg.Workbook.Worksheets.Add("Planilha1");

                    // Cabeçalho (linha 1)
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

                    // Larguras (preservadas do layout original)
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

                        string tdoc = "";
                        if (Convert.ToString(linexc["DOC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC"]).Trim();
                        else if (Convert.ToString(linexc["DOC_FISC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC_FISC"]).Trim();
                        if (tdoc != "") tdoc = "DOC.n.:" + tdoc;

                        string thist = "";
                        if (Convert.ToString(linexc["HIST"]).Trim() != "")
                            thist = Convert.ToString(linexc["HIST"]).Trim();

                        // Mimetiza o auto-convert do Excel Interop Value2: codigo numerico vira numero
                        // (alinhado a direita), nao-numerico fica como string (alinhado a esquerda).
                        // AlterData historicamente recebia esses campos como numero -- preserva paridade.
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
                        ws.Cells[row, 10].Value = tdoc;
                        row++;
                    }

                    pkg.Save();
                }

                MessageBox.Show("Excel " + filename + " gravado OK.");
            }
            catch (Exception E)
            {
                MessageBox.Show("Erro ao gerar planilha Excel: " + E.Message,
                    "Erro export AlterData", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }
    }
}
