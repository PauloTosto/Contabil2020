using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Windows.Forms;



namespace ClassLibTrabalho
{
    public class DataGridViewPrinter_Folha
    {
        private DataGridView TheDataGridView; // The DataGridView Control which will be printed
        private PrintDocument ThePrintDocument; // The PrintDocument to be used for printing
        private bool IsCenterOnPage; // Determine if the report will be printed in the Top-Center of the page
        private bool IsWithTitle; // Determine if the page contain title text
        private string TheTitleText; // The title text to be printed in each page (if IsWithTitle is set to true)
        private Font TheTitleFont; // The font to be used with the title text (if IsWithTitle is set to true)
        private Color TheTitleColor; // The color to be used with the title text (if IsWithTitle is set to true)
        private bool IsWithPaging; // Determine if paging is used

        private string EmpresaText; // The title text to be printed in each page (if IsWithTitle is set to true)
   
        private string FazendaText;

        static int CurrentRow; // A static parameter that keep track on which Row (in the DataGridView control) that should be printed

        static int PageNumber;

        private int PageWidth;
        private int PageHeight;
        private int LeftMargin;
        private int TopMargin;
        private int RightMargin;
        private int BottomMargin;

        private float CurrentY; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate

        private float RowHeaderHeight;
        
        public List<float> RowsHeight;
        public List<float> ColumnsWidth;

        public List<string> LinhaCab1;
        public List<string> LinhaCab2;

        public Dictionary<int,float> LinhaTotal;
        
        private float TheDataGridViewWidth;

        // Maintain a generic list to hold start/stop points for the column printing
        // This will be used for wrapping in situations where the DataGridView will not fit on a single page
        private List<int[]> mColumnPoints;
        private List<float> mColumnPointsWidth;
        private int mColumnPoint;

        // The class constructor
        public DataGridViewPrinter_Folha(DataGridView aDataGridView, PrintDocument aPrintDocument, bool CenterOnPage, bool WithTitle, string aTitleText, Font aTitleFont, Color aTitleColor, bool WithPaging, string aempresatext, string afazendatext)
        {
            TheDataGridView = aDataGridView;
            ThePrintDocument = aPrintDocument;
            IsCenterOnPage = CenterOnPage;
            IsWithTitle = WithTitle;
            TheTitleText = aTitleText;
            TheTitleFont = aTitleFont;
            TheTitleColor = aTitleColor;
            IsWithPaging = WithPaging;
            EmpresaText = aempresatext;
            FazendaText = afazendatext;

            PageNumber = 0;

            RowsHeight = new List<float>();
            ColumnsWidth = new List<float>();

            LinhaCab1 = new List<string>();
            LinhaCab2 = new List<string>();

            LinhaTotal = new Dictionary<int, float>();

            mColumnPoints = new List<int[]>();
            mColumnPointsWidth = new List<float>();

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

        // The funtion that print the title, page number, and the header row
        private void DrawHeader(Graphics g)
        {

            float MargemDireitaAdicional = 60.0F;
            CurrentY = (float)TopMargin;
            CurrentY = 43.30F;
            // titulo centralizado
            StringFormat TitleFormat = new StringFormat();
            TitleFormat.Trimming = StringTrimming.Word;
            TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            TitleFormat.Alignment = StringAlignment.Center;
            RectangleF TitleRectangle = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString(TheTitleText, TheTitleFont).Height);
            g.DrawString(TheTitleText, TheTitleFont, new SolidBrush(TheTitleColor), TitleRectangle, TitleFormat);

            SizeF tmpSize = g.MeasureString(TheTitleText, TheTitleFont);

            PageNumber++;
            string PageString = "PAG. " + PageNumber.ToString();

            StringFormat PageStringFormat = new StringFormat();
            PageStringFormat.Trimming = StringTrimming.Word;
            PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            PageStringFormat.Alignment = StringAlignment.Far;

            Font PageStringFont = new Font("Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point);

            RectangleF PageStringRectangle = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin - MargemDireitaAdicional, g.MeasureString(PageString, PageStringFont).Height);

            // RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight[0]);



            g.DrawString(PageString, PageStringFont, new SolidBrush(Color.Black), PageStringRectangle, PageStringFormat);


            //Empresa
            StringFormat EmpresaFormat = new StringFormat();
            EmpresaFormat.Trimming = StringTrimming.Word;
            EmpresaFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            //if (IsCenterOnPage)
            //EmpresaFormat.Alignment = StringAlignment.Center;
            //else
            EmpresaFormat.Alignment = StringAlignment.Near;

            RectangleF EmpresaRectangle = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString(EmpresaText, TheTitleFont).Height);

            g.DrawString(EmpresaText, TheTitleFont, new SolidBrush(TheTitleColor), EmpresaRectangle, EmpresaFormat);


            CurrentY = 70.866F;//g.MeasureString(TheTitleText, TheTitleFont).Height;

            EmpresaFormat = new StringFormat();
            EmpresaFormat.Trimming = StringTrimming.Word;
            EmpresaFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            //if (IsCenterOnPage)
            //EmpresaFormat.Alignment = StringAlignment.Center;
            //else
            EmpresaFormat.Alignment = StringAlignment.Near;

            RectangleF fazendaRectangle = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString(EmpresaText, TheTitleFont).Height);

            g.DrawString(FazendaText, TheTitleFont, new SolidBrush(TheTitleColor), fazendaRectangle, EmpresaFormat);

            EmpresaFormat = new StringFormat();
            EmpresaFormat.Trimming = StringTrimming.Word;
            EmpresaFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            //if (IsCenterOnPage)
            //EmpresaFormat.Alignment = StringAlignment.Center;
            //else
            EmpresaFormat.Alignment = StringAlignment.Far;
            string assinaturatext = "Assinatura do Responsável:............................................................";
            Font AssinaturaFont = new Font(TheTitleFont.FontFamily, TheTitleFont.Size, FontStyle.Italic);
            RectangleF assinaturaRectangle = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin - MargemDireitaAdicional, g.MeasureString(assinaturatext, AssinaturaFont).Height);

            g.DrawString(assinaturatext, AssinaturaFont, new SolidBrush(TheTitleColor), assinaturaRectangle, EmpresaFormat);


            CurrentY = 98.4252F;     //g.MeasureString(FazendaText, TheTitleFont).Height;

            // Calculating the starting x coordinate that the printing process will start from
            float CurrentX = (float)LeftMargin;
            // if (IsCenterOnPage)
            //   CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;

            // Setting the HeaderFore style
            Color HeaderForeColor = TheDataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
            if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
                HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
            SolidBrush HeaderForeBrush = new SolidBrush(HeaderForeColor);

            // Setting the HeaderBack style
            Color HeaderBackColor = TheDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
            if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
                HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor;
            SolidBrush HeaderBackBrush = new SolidBrush(HeaderBackColor);

            // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Pen TheLinePen = new Pen(TheDataGridView.GridColor, 1);

            // Setting the HeaderFont style
            Font HeaderFont = TheDataGridView.ColumnHeadersDefaultCellStyle.Font;
            if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
                HeaderFont = TheDataGridView.DefaultCellStyle.Font;

            // Calculating and drawing the HeaderBounds        
            // RectangleF HeaderBounds = new RectangleF(CurrentX, CurrentY, mColumnPointsWidth[mColumnPoint], RowHeaderHeight);
            // g.FillRectangle(HeaderBackBrush, HeaderBounds);

            // Setting the format that will be used to print each cell of the header row
            StringFormat CellFormat = new StringFormat();
            CellFormat.Trimming = StringTrimming.Word;
            CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;

            // Printing each visible cell of the header row
            RowHeaderHeight = 17.716536F;
            RectangleF CellBounds;
            float ColumnWidth;

            for (int i = (int)mColumnPoints[mColumnPoint].GetValue(0); i < (int)mColumnPoints[mColumnPoint].GetValue(1); i++)
            {
                // Setting the HeaderFore style
                HeaderForeColor = TheDataGridView.Columns[i].DefaultCellStyle.ForeColor;
                if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
                    HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
                HeaderForeBrush = new SolidBrush(HeaderForeColor);

                // Setting the HeaderBack style
                HeaderBackColor = TheDataGridView.Columns[i].DefaultCellStyle.BackColor;
                if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
                    HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor;
                HeaderBackBrush = new SolidBrush(HeaderBackColor);

                // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
                TheLinePen = new Pen(TheDataGridView.GridColor, 1);

                // Setting the HeaderFont style
                HeaderFont = TheDataGridView.Columns[i].DefaultCellStyle.Font;
                if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
                    HeaderFont = TheDataGridView.DefaultCellStyle.Font;

                ColumnWidth = ColumnsWidth[i];

                // Check the CurrentCell alignment and apply it to the CellFormat
                if (TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
                    CellFormat.Alignment = StringAlignment.Far;
                else if (TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
                    CellFormat.Alignment = StringAlignment.Center;
                else
                    CellFormat.Alignment = StringAlignment.Near;
                // Check the CurrentCell alignment and apply it to the CellFormat
                if (TheDataGridView.Columns[i].DefaultCellStyle.Alignment.ToString().Contains("Right"))
                    CellFormat.Alignment = StringAlignment.Far;
                else if (TheDataGridView.Columns[i].DefaultCellStyle.Alignment.ToString().Contains("Center"))
                    CellFormat.Alignment = StringAlignment.Center;
                else
                    CellFormat.Alignment = StringAlignment.Near;

                CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);
                PointF inicio = new PointF(CurrentX, CurrentY - 1);
                PointF fim = new PointF(CurrentX + ColumnWidth, CurrentY - 1);
                g.DrawLine(TheLinePen, inicio, fim);

                if (i != 0)
                {
                    inicio = new PointF(CurrentX, CurrentY);
                    fim = new PointF(CurrentX, CurrentY + RowHeaderHeight);
                    g.DrawLine(TheLinePen, inicio, fim);
                }

                // Printing the cell text
                Font CellFont = TheDataGridView.Columns[i].DefaultCellStyle.Font;
                if (CellFont != null)
                    g.DrawString(LinhaCab1[i], HeaderFont, HeaderForeBrush, CellBounds, CellFormat);
                else
                    g.DrawString(LinhaCab1[i], HeaderFont, HeaderForeBrush, CellBounds, CellFormat);
                //LINHA ACIMA

                // Drawing the cell bounds
                //    if (TheDataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None) // Draw the cell border only if the HeaderBorderStyle is not None
                //      g.DrawRectangle(TheLinePen, CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);

                CurrentX += ColumnWidth;
            }

            CurrentY += RowHeaderHeight;

            CurrentX = (float)LeftMargin;
            for (int i = (int)mColumnPoints[mColumnPoint].GetValue(0); i < (int)mColumnPoints[mColumnPoint].GetValue(1); i++)
            {
                // Setting the HeaderFore style
                HeaderForeColor = TheDataGridView.Columns[i].DefaultCellStyle.ForeColor;
                if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
                    HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
                HeaderForeBrush = new SolidBrush(HeaderForeColor);

                // Setting the HeaderBack style
                HeaderBackColor = TheDataGridView.Columns[i].DefaultCellStyle.BackColor;
                if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
                    HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor;
                HeaderBackBrush = new SolidBrush(HeaderBackColor);

                // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
                TheLinePen = new Pen(TheDataGridView.GridColor, 1);

                // Setting the HeaderFont style
                HeaderFont = TheDataGridView.Columns[i].DefaultCellStyle.Font;
                if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
                    HeaderFont = TheDataGridView.DefaultCellStyle.Font;

                ColumnWidth = ColumnsWidth[i];

                // Check the CurrentCell alignment and apply it to the CellFormat
                if (TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
                    CellFormat.Alignment = StringAlignment.Far;
                else if (TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
                    CellFormat.Alignment = StringAlignment.Center;
                else
                    CellFormat.Alignment = StringAlignment.Near;
                // Check the CurrentCell alignment and apply it to the CellFormat
                if (TheDataGridView.Columns[i].DefaultCellStyle.Alignment.ToString().Contains("Right"))
                    CellFormat.Alignment = StringAlignment.Far;
                else if (TheDataGridView.Columns[i].DefaultCellStyle.Alignment.ToString().Contains("Center"))
                    CellFormat.Alignment = StringAlignment.Center;
                else
                    CellFormat.Alignment = StringAlignment.Near;

                CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);

                PointF inicio = new PointF(CurrentX, CurrentY + RowHeaderHeight - 1);
                PointF fim = new PointF(CurrentX + ColumnWidth, CurrentY + RowHeaderHeight - 1);
                g.DrawLine(TheLinePen, inicio, fim);

                if (i != 0)
                {
                    inicio = new PointF(CurrentX, CurrentY);
                    fim = new PointF(CurrentX, CurrentY + RowHeaderHeight);
                    g.DrawLine(TheLinePen, inicio, fim);
                }

                // Printing the cell text
                Font CellFont = TheDataGridView.Columns[i].DefaultCellStyle.Font;
                if (CellFont != null)
                    g.DrawString(LinhaCab2[i], HeaderFont, HeaderForeBrush, CellBounds, CellFormat);
                else
                    g.DrawString(LinhaCab2[i], HeaderFont, HeaderForeBrush, CellBounds, CellFormat);
                //if ((CurrentRow % 2) != 0) // quando for impar par
                // {

                //}
                // Drawing the cell bounds
                // if (TheDataGridView.RowHeadersBorderStyle != DataGridViewHeaderBorderStyle.None) // Draw the cell border only if the HeaderBorderStyle is not None
                //   g.DrawRectangle(TheLinePen, CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);

                CurrentX += ColumnWidth;
            }


            CurrentY += RowHeaderHeight;

        }

        // The function that print a bunch of rows that fit in one page
        // When it returns true, meaning that there are more rows still not printed, so another PagePrint action is required
        // When it returns false, meaning that all rows are printed (the CureentRow parameter reaches the last row of the DataGridView control) and no further PagePrint action is required
        private bool DrawRows(Graphics g)
        {
            // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Pen TheLinePen = new Pen(TheDataGridView.GridColor, 1);

            // The style paramters that will be used to print each cell
            Font RowFont;
            Color RowForeColor;
            Color RowBackColor;
            SolidBrush RowForeBrush;
            SolidBrush RowBackBrush;
            SolidBrush RowAlternatingBackBrush;

            // Setting the format that will be used to print each cell
            StringFormat CellFormat = new StringFormat();
            CellFormat.Trimming = StringTrimming.Word;
            CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;

            // Printing each visible cell
            RectangleF RowBounds;
            float CurrentX;
            float ColumnWidth;
            Font CellFont;
            
            
            try
            {

                while (CurrentRow < TheDataGridView.Rows.Count)
                {
                    //while (CurrentRow < TheDataGridView.Rows.Count)
                    // {
                    if (TheDataGridView.Rows[CurrentRow].Visible) // Print the cells of the CurrentRow only if that row is visible
                    {
                        // Setting the row font style
                        RowFont = TheDataGridView.Rows[CurrentRow].DefaultCellStyle.Font;
                        if (RowFont == null) // If the there is no special font style of the CurrentRow, then use the default one associated with the DataGridView control
                            RowFont = TheDataGridView.DefaultCellStyle.Font;

                        // Setting the RowFore style
                        RowForeColor = TheDataGridView.Rows[CurrentRow].DefaultCellStyle.ForeColor;
                        if (RowForeColor.IsEmpty) // If the there is no special RowFore style of the CurrentRow, then use the default one associated with the DataGridView control
                            RowForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
                        RowForeBrush = new SolidBrush(RowForeColor);

                        // Setting the RowBack (for even rows) and the RowAlternatingBack (for odd rows) styles
                        RowBackColor = TheDataGridView.Rows[CurrentRow].DefaultCellStyle.BackColor;
                        if (RowBackColor.IsEmpty) // If the there is no special RowBack style of the CurrentRow, then use the default one associated with the DataGridView control
                        {
                            RowBackBrush = new SolidBrush(TheDataGridView.DefaultCellStyle.BackColor);
                            RowAlternatingBackBrush = new SolidBrush(TheDataGridView.AlternatingRowsDefaultCellStyle.BackColor);
                        }
                        else // If the there is a special RowBack style of the CurrentRow, then use it for both the RowBack and the RowAlternatingBack styles
                        {
                            RowBackBrush = new SolidBrush(RowBackColor);
                            RowAlternatingBackBrush = new SolidBrush(RowBackColor);
                        }

                        // Calculating the starting x coordinate that the printing process will start from
                        CurrentX = (float)LeftMargin;
                        if (IsCenterOnPage)
                            CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;

                        // Calculating the entire CurrentRow bounds                

                        //RowBounds = new RectangleF(CurrentX, CurrentY, mColumnPointsWidth[mColumnPoint], RowsHeight[CurrentRow]); //unica dimensao de height de linhas
                        RowBounds = new RectangleF(CurrentX, CurrentY, mColumnPointsWidth[mColumnPoint], RowsHeight[0]);
                        // Filling the back of the CurrentRow
                        if (CurrentRow % 2 == 0)
                            g.FillRectangle(RowBackBrush, RowBounds);
                        else
                            g.FillRectangle(RowAlternatingBackBrush, RowBounds);
                        // Printing each visible cell of the CurrentRow                
                        for (int CurrentCell = (int)mColumnPoints[mColumnPoint].GetValue(0); CurrentCell < (int)mColumnPoints[mColumnPoint].GetValue(1); CurrentCell++)
                        {
                            if (!TheDataGridView.Columns[CurrentCell].Visible) continue; // If the cell is belong to invisible column, then ignore this iteration

                            // Check the CurrentCell alignment and apply it to the CellFormat
                            if (TheDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Right"))
                                CellFormat.Alignment = StringAlignment.Far;
                            else if (TheDataGridView.Columns[CurrentCell].DefaultCellStyle.Alignment.ToString().Contains("Center"))
                                CellFormat.Alignment = StringAlignment.Center;
                            else
                                CellFormat.Alignment = StringAlignment.Near;

                            ColumnWidth = ColumnsWidth[CurrentCell];
                            //RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight[CurrentRow]);
                            RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight[0]);
                            // Printing the cell text
                            CellFont = TheDataGridView.Columns[TheDataGridView.Rows[CurrentRow].Cells[CurrentCell].ColumnIndex].DefaultCellStyle.Font;
                            if (CellFont != null)
                            {
                                g.DrawString(TheDataGridView.Rows[CurrentRow].Cells[CurrentCell].EditedFormattedValue.ToString(), CellFont, RowForeBrush, CellBounds, CellFormat);
                            }
                            else
                               g.DrawString(TheDataGridView.Rows[CurrentRow].Cells[CurrentCell].EditedFormattedValue.ToString(), RowFont, RowForeBrush, CellBounds, CellFormat);


                            if (CurrentCell != 0)
                            {
                                PointF inicio = new PointF(CurrentX, CurrentY);
                                PointF fim = new PointF(CurrentX, CurrentY + RowsHeight[0]);
                                g.DrawLine(TheLinePen, inicio, fim);
                            }
                            if ((CurrentRow % 2) != 0) // quando for impar par
                            {
                                PointF inicio = new PointF(CurrentX, CurrentY + RowsHeight[0] - 1);
                                PointF fim = new PointF(CurrentX + ColumnWidth, CurrentY + RowsHeight[0] - 1);
                                g.DrawLine(TheLinePen, inicio, fim);
                            }
                            // Drawing the cell bounds
                            //  if (TheDataGridView.CellBorderStyle != DataGridViewCellBorderStyle.None) // Draw the cell border only if the CellBorderStyle is not None


                            CurrentX += ColumnWidth;
                        }
                        CurrentY += RowsHeight[0];

                        // Checking if the CurrentY is exceeds the page boundries
                        // If so then exit the function and returning true meaning another PagePrint action is required
                        if ((int)CurrentY > (PageHeight - TopMargin - BottomMargin - (RowsHeight[0]*3.2F)))
                        {
                            CurrentRow++;
                            return true;
                        }

                    }
                    CurrentRow++;
                }
            }
            catch (Exception)
            {
                throw;
            }
            CurrentRow = 0;
            mColumnPoint++; // Continue to print the next group of columns
            if (mColumnPoint == mColumnPoints.Count) // Which means all columns are printed
            {
                mColumnPoint = 0;
                return false;
            }
            else
                return true;
        }

        // The method that calls all other functions
        public bool DrawDataGridView(Graphics g)
        {
            try
            {
                CalculateFolha(g);
                DrawHeader(g);
                bool bContinue = DrawRows(g);
                if (bContinue == false)
                    DrawTotal(g);
                DrawRodape(g);
                return bContinue;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Operação Falhou: " + ex.Message.ToString(), Application.ProductName + " - Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        // customização para a folha

        private void CalculateFolha(Graphics g)
        {
            if (PageNumber == 0) // Just calculate once
            {
                float tmpWidth;

                TheDataGridViewWidth = 0;
                for (int i = 0; i < ColumnsWidth.Count; i++)
                {

                    tmpWidth = ColumnsWidth[i];
                    TheDataGridViewWidth += tmpWidth;
                }
                // Define the start/stop column points based on the page width and the DataGridView Width
                // We will use this to determine the columns which are drawn on each page and how wrapping will be handled
                // By default, the wrapping will occurr such that the maximum number of columns for a page will be determine
              //  int k;

                int mStartPoint = 0;
                int mEndPoint = TheDataGridView.Columns.Count;
                float mTempWidth = TheDataGridViewWidth;
                float mTempPrintArea = (float)PageWidth - (float)LeftMargin - (float)RightMargin;

                // We only care about handling where the total datagridview width is bigger then the print area

                if (TheDataGridViewWidth > mTempPrintArea)
                {
                    MessageBox.Show("Só Gerar em A4 e LandScape");
                   
                }
                // Add the last set of columns
                mColumnPoints.Add(new int[] { mStartPoint, mEndPoint });
                mColumnPointsWidth.Add(mTempWidth);
                mColumnPoint = 0;
            }
        }
        private void DrawRodape(Graphics g)
        {
            //748,031
            //float MargemDireitaAdicional = 60.0F;
            CurrentY = (float)TopMargin;
            CurrentY = 787.04F;
            string rodapeText = "IMPRESSÃO DIGITAL VIDE VERSO";
            // titulo centralizado
            StringFormat TitleFormat = new StringFormat();
            TitleFormat.Trimming = StringTrimming.Word;
            TitleFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            TitleFormat.Alignment = StringAlignment.Center;
            RectangleF TitleRectangle = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin, g.MeasureString(rodapeText, TheTitleFont).Height);
            g.DrawString(rodapeText, TheTitleFont, new SolidBrush(TheTitleColor), TitleRectangle, TitleFormat);

            //SizeF tmpSize = g.MeasureString(rodapeText, TheTitleFont);
            
        }
        private void DrawTotal(Graphics g)
        {

            // Calculating the starting x coordinate that the printing process will start from
            float CurrentX = (float)LeftMargin;
            // if (IsCenterOnPage)
            //   CurrentX += (((float)PageWidth - (float)RightMargin - (float)LeftMargin) - mColumnPointsWidth[mColumnPoint]) / 2.0F;

            // Setting the HeaderFore style
            Color HeaderForeColor = TheDataGridView.ColumnHeadersDefaultCellStyle.ForeColor;
            if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
                HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
            SolidBrush HeaderForeBrush = new SolidBrush(HeaderForeColor);

            // Setting the HeaderBack style
            Color HeaderBackColor = TheDataGridView.ColumnHeadersDefaultCellStyle.BackColor;
            if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
                HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor;
            SolidBrush HeaderBackBrush = new SolidBrush(HeaderBackColor);

            // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Pen TheLinePen = new Pen(TheDataGridView.GridColor, 1);

            // Setting the HeaderFont style
            Font HeaderFont = TheDataGridView.ColumnHeadersDefaultCellStyle.Font;
            if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
                HeaderFont = TheDataGridView.DefaultCellStyle.Font;

        
            // Setting the format that will be used to print each cell of the header row
            StringFormat CellFormat = new StringFormat();
            CellFormat.Trimming = StringTrimming.Word;
            CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;

            // Printing each visible cell of the header row
            RowHeaderHeight = 17.716536F;
            RectangleF CellBounds;
            float ColumnWidth;

            for (int i = (int)mColumnPoints[mColumnPoint].GetValue(0); i < (int)mColumnPoints[mColumnPoint].GetValue(1); i++)
            {
                // Setting the HeaderFore style
                HeaderForeColor = TheDataGridView.Columns[i].DefaultCellStyle.ForeColor;
                if (HeaderForeColor.IsEmpty) // If there is no special HeaderFore style, then use the default DataGridView style
                    HeaderForeColor = TheDataGridView.DefaultCellStyle.ForeColor;
                HeaderForeBrush = new SolidBrush(HeaderForeColor);

                // Setting the HeaderBack style
                HeaderBackColor = TheDataGridView.Columns[i].DefaultCellStyle.BackColor;
                if (HeaderBackColor.IsEmpty) // If there is no special HeaderBack style, then use the default DataGridView style
                    HeaderBackColor = TheDataGridView.DefaultCellStyle.BackColor;
                HeaderBackBrush = new SolidBrush(HeaderBackColor);

                // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
                TheLinePen = new Pen(TheDataGridView.GridColor, 1);

                // Setting the HeaderFont style
                HeaderFont = TheDataGridView.Columns[i].DefaultCellStyle.Font;
                if (HeaderFont == null) // If there is no special HeaderFont style, then use the default DataGridView font style
                    HeaderFont = TheDataGridView.DefaultCellStyle.Font;

                ColumnWidth = ColumnsWidth[i];

                // Check the CurrentCell alignment and apply it to the CellFormat
                if (TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Right"))
                    CellFormat.Alignment = StringAlignment.Far;
                else if (TheDataGridView.ColumnHeadersDefaultCellStyle.Alignment.ToString().Contains("Center"))
                    CellFormat.Alignment = StringAlignment.Center;
                else
                    CellFormat.Alignment = StringAlignment.Near;
                // Check the CurrentCell alignment and apply it to the CellFormat
                if (TheDataGridView.Columns[i].DefaultCellStyle.Alignment.ToString().Contains("Right"))
                    CellFormat.Alignment = StringAlignment.Far;
                else if (TheDataGridView.Columns[i].DefaultCellStyle.Alignment.ToString().Contains("Center"))
                    CellFormat.Alignment = StringAlignment.Center;
                else
                    CellFormat.Alignment = StringAlignment.Near;

                CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowHeaderHeight);
           //     PointF inicio = new PointF(CurrentX, CurrentY - 1);
             //   PointF fim = new PointF(CurrentX + ColumnWidth, CurrentY - 1);
               // g.DrawLine(TheLinePen, inicio, fim);
                PointF inicio = new PointF(CurrentX, CurrentY + RowHeaderHeight - 1);
                PointF fim = new PointF(CurrentX + ColumnWidth, CurrentY + RowHeaderHeight - 1);
                g.DrawLine(TheLinePen, inicio, fim);

                string valortotal = "";
                if (LinhaTotal.ContainsKey(i))
                {
                    string formato = TheDataGridView.Columns[i].DefaultCellStyle.Format;
                    valortotal = LinhaTotal[i].ToString(formato);
                }

                if (i == 0)
                    valortotal = "T O T A I S";
                if (valortotal == "")
                {
                    CurrentX += ColumnWidth;
                    continue;
                }
                // Printing the cell text
                Font CellFont = TheDataGridView.Columns[i].DefaultCellStyle.Font;
                if (i == 0)
                {
                    if (CellFont != null)
                        g.DrawString(valortotal, CellFont, HeaderForeBrush, CellBounds, CellFormat);
                    else
                        g.DrawString(valortotal, HeaderFont, HeaderForeBrush, CellBounds, CellFormat);
                }
                else
                {
                    if (CellFont == null)
                        CellFont = HeaderFont;
                    Font FontReduzida = new Font(CellFont.OriginalFontName, CellFont.Size - 1, CellFont.Style);
                    g.DrawString(valortotal, FontReduzida, HeaderForeBrush, CellBounds, CellFormat);

                    inicio = new PointF(CurrentX, CurrentY);
                    fim = new PointF(CurrentX, CurrentY + RowHeaderHeight);
                    g.DrawLine(TheLinePen, inicio, fim);
                    inicio = new PointF(CurrentX + ColumnWidth, CurrentY);
                    fim = new PointF(CurrentX + ColumnWidth, CurrentY + RowHeaderHeight);
                    g.DrawLine(TheLinePen, inicio, fim);
                }


                CurrentX += ColumnWidth;
            }

            CurrentY += RowHeaderHeight;


        }


    }
}
