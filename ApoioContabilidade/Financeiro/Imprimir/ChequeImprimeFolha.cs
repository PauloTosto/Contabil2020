using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using ApoioContabilidade.Financeiro.Models;

namespace ApoioContabilidade.Financeiro.Imprimir
{
    class ChequeImprimeFolha
    {
        private ChequeBradesco chequeBradesco;
        //  private DataGridView TheDataGridView; // The DataGridView Control which will be printed
        private PrintDocument ThePrintDocument; // The PrintDocument to be used for printing
        public Font TheTitleFont; // The font to be used with the title text (if IsWithTitle is set to true)

        int CurrentRow; // A static parameter that keep track on which Row (in the DataGridView control) that should be printed


        private int PageWidth;
        private int PageHeight;
        private int LeftMargin;
        private int TopMargin;
        private int RightMargin;
        private int BottomMargin;

        private float CurrentY; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate



        // The class constructor
        public ChequeImprimeFolha(ChequeBradesco ochequeBradesco, PrintDocument aPrintDocument,
               Font aTitleFont)
        {
            chequeBradesco = ochequeBradesco;
            ThePrintDocument = aPrintDocument;
            TheTitleFont = aTitleFont;



            // Claculating the PageWidth and the PageHeight
            if (!ThePrintDocument.DefaultPageSettings.Landscape)
            {
                PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
                PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
            }
            else
            {
                PageHeight = ThePrintDocument.DefaultPageSettings.PaperSize.Width;
                PageWidth = ThePrintDocument.DefaultPageSettings.PaperSize.Height;
            }

            // Claculating the page margins
            LeftMargin = ThePrintDocument.DefaultPageSettings.Margins.Left;
            TopMargin = ThePrintDocument.DefaultPageSettings.Margins.Top;
            RightMargin = ThePrintDocument.DefaultPageSettings.Margins.Right;
            BottomMargin = ThePrintDocument.DefaultPageSettings.Margins.Bottom;

            // First, the current row to be printed is the first row in the DataGridView control
            CurrentRow = 0;
        }

        // 

        // The function that print a bunch of rows that fit in one page
        // When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
        // When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
        private bool DrawRows(System.Drawing.Printing.PrintPageEventArgs e)
        {

            // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Pen TheLinePen = new Pen(Color.Black, 2);

            // The style paramters that will be used to print each cell
            Font RowFont = TheTitleFont;
            SolidBrush RowForeBrush;

            // Setting the format that will be used to print each cell
            StringFormat CellFormat = new StringFormat();
            CellFormat.Trimming = StringTrimming.Word;
            CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

            //  int incremente_ = Math.Round(OffSetUL.Y / 2);
            // espaco:= MMtoPixelY(76.5) + incremente;


           // int incremente = 0; //  := RouND(OffSetUL.Y / 2);
           // float espaco = 1807;       //MMtoPixelY(76.5) + incremente;
            // espaco = MMtoPixelY(e.Graphics, 76.5);
           // espaco = MMtoY(e, 76.5);
            SizeF padrao_linha_col = e.Graphics.MeasureString("Anything", RowFont);
            int largura = e.MarginBounds.Width;
            int altura = e.MarginBounds.Height;


            // mmPointY = Printer.PageHeight / GetDeviceCaps(Printer.Handle, VERTSIZE);

            float linhasPorPagina = e.MarginBounds.Height / RowFont.GetHeight(e.Graphics);
            float margemEsquerda = e.MarginBounds.Left;
            float margemSuperior = e.MarginBounds.Top;
            float CurrentX;
            float ColumnWidth;
            float margemNegativo = -7;// maximo negativo
            RowForeBrush = new SolidBrush(Color.Black);
            try
            {
                while (CurrentRow < chequeBradesco.cheques.Count)
                {
                    Extendidos ocheque = chequeBradesco.cheques[CurrentRow];
                    string tcampo = String.Format("R$ {0:#,###,##0.00}", ocheque.valor);

                    SizeF tmpSize = e.Graphics.MeasureString(tcampo, RowFont);

                    ColumnWidth = tmpSize.Width;
                    float linhaAltura = tmpSize.Height;

                    CurrentY = MMtoY(e, (7 + margemNegativo - 0.5 )) ;
                    //
                    // CurrentX = MMtoPixelX(e.Graphics,  130 + 2);
                    CurrentX = MMtoX(e, 130 + 2 + margemNegativo - 1);
                    RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);

                    e.Graphics.DrawString(tcampo, RowFont, RowForeBrush, CellBounds, CellFormat);

                    // primeira linha de valores
                    tcampo = ocheque.valores[0];
                    tmpSize = e.Graphics.MeasureString(tcampo, RowFont);
                    CurrentY = MMtoY(e, (13 + margemNegativo));   // 16
                    CurrentX = MMtoX(e, 25 + margemNegativo - 2); //(-5)
                    ColumnWidth = tmpSize.Width;
                    linhaAltura = tmpSize.Height;
                    CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                    e.Graphics.DrawString(tcampo, RowFont, RowForeBrush, CellBounds, CellFormat);

                    if (ocheque.valores.Count > 1)
                    {
                        tcampo = ocheque.valores[1];
                        tmpSize = e.Graphics.MeasureString(tcampo, RowFont);
                        CurrentY = MMtoY(e, (19 + margemNegativo));   // 16
                        CurrentX = MMtoX(e, 5 - 3.5); //-5

                        ColumnWidth = tmpSize.Width;
                        linhaAltura = tmpSize.Height;
                        CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                        e.Graphics.DrawString(tcampo, RowFont, RowForeBrush, CellBounds, CellFormat);
                    }

                    //// TITULAR DO CHEQUE
                    tcampo = ocheque.nomes[0];
                    tmpSize = e.Graphics.MeasureString(tcampo, RowFont);

                    //  CurrentY = MMtoPixelY(e.Graphics, 25) + (espaco * contcheques) + incremente;   // 16
                    //  CurrentX = MMtoPixelX(e.Graphics, 7 - 4);
                    CurrentY = MMtoY(e, (25 + margemNegativo));   // 16
                    CurrentX = MMtoX(e, 7 + margemNegativo - 1); //- 4

                    ColumnWidth = tmpSize.Width;
                    linhaAltura = tmpSize.Height;
                    CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                    e.Graphics.DrawString(tcampo, RowFont, RowForeBrush, CellBounds, CellFormat);

                    //LOCAL
                    tcampo = ocheque.local;
                    tmpSize = e.Graphics.MeasureString(tcampo, RowFont);

                    CurrentY = MMtoY(e, (32 + margemNegativo));   // 16
                    CurrentX = MMtoX(e, 75 + margemNegativo); // - 5

                    ColumnWidth = tmpSize.Width;
                    linhaAltura = tmpSize.Height;
                    CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                    e.Graphics.DrawString(tcampo, RowFont, RowForeBrush, CellBounds, CellFormat);
                    /// DATA
                    /// 
                    tcampo = ocheque.dataExtendida.Trim();
                    // DIA
                    tmpSize = e.Graphics.MeasureString(tcampo.Substring(0, 2), RowFont);
                    CurrentX = MMtoX(e, 105 + margemNegativo); //- 7

                    ColumnWidth = tmpSize.Width;
                    linhaAltura = tmpSize.Height;
                    CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                    e.Graphics.DrawString(tcampo.Substring(0, 2), RowFont, RowForeBrush, CellBounds, CellFormat);
                    // MES
                    string mes = tcampo.Substring(3, (tcampo.Length - 8));
                    tmpSize = e.Graphics.MeasureString(mes, RowFont);

                    CurrentX = MMtoX(e, 125 + margemNegativo); // - 7

                    ColumnWidth = tmpSize.Width;
                    linhaAltura = tmpSize.Height;
                    CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                    e.Graphics.DrawString(mes, RowFont, RowForeBrush, CellBounds, CellFormat);
                    // ANO
                    string ano = tcampo.Substring(tcampo.Length - 4);
                    tmpSize = e.Graphics.MeasureString(ano, RowFont);

                    CurrentX = MMtoX(e, (162 + margemNegativo) - 1 - 2.2);

                    ColumnWidth = tmpSize.Width;
                    linhaAltura = tmpSize.Height;
                    CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, linhaAltura);
                    e.Graphics.DrawString(ano, RowFont, RowForeBrush, CellBounds, CellFormat);
                    
                    CurrentRow++;
                    if (CurrentRow < chequeBradesco.cheques.Count)
                        return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
            CurrentRow = 0;
            return false;
        }

        Single MMtoX(System.Drawing.Printing.PrintPageEventArgs e, double mmX)
        {
            Single result = 0;
            double mmLargura = 180;
            //  1cm => 0.3937008
            double largura = Convert.ToDouble(e.MarginBounds.Width);
            //int altura = e.MarginBounds.Height;
            result = Convert.ToSingle((mmX / mmLargura) * largura) + e.MarginBounds.Left;
            return result;
        }
        Single MMtoY(System.Drawing.Printing.PrintPageEventArgs e, double mmX)
        {
            Single result = 0;
            double mmaltura = 76.5;
            //  1cm => 0.3937008
            double altura = Convert.ToDouble(e.MarginBounds.Height);
            //int altura = e.MarginBounds.Height;
            result = Convert.ToSingle((mmX / mmaltura) * altura) + e.MarginBounds.Top;
            return result;
        }




        public bool DrawDataGridView(System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                bool bContinue = DrawRows(e);
                return bContinue;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Operação Falhou: " + ex.Message.ToString(), Application.ProductName + " - Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }


    }
}
