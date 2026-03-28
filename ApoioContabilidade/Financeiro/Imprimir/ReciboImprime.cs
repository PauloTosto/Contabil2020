using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Windows.Forms;
using ApoioContabilidade.Financeiro.Models;


namespace ApoioContabilidade.Financeiro.Imprimir
{
    class ReciboImprime
    {

        private Recibos recibo;
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

        private int condicao_par_impar = 1;

        private float CurrentY; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate



        // The class constructor
        public ReciboImprime(Recibos orecibo, PrintDocument aPrintDocument,
               Font aTitleFont)
        {
            recibo = orecibo;
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
            Pen TheLinePen = new Pen(Color.Black, 3);

            // The style paramters that will be used to print each cell
            Font RowFont = TheTitleFont;
            SolidBrush RowForeBrush;

            // Setting the format that will be used to print each cell
            StringFormat CellFormat = new StringFormat();
            CellFormat.Trimming = StringTrimming.Word;
            CellFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit;


            SizeF padrao_linha_col = e.Graphics.MeasureString("Anything", RowFont);

            int largura = e.MarginBounds.Width;
            int altura = e.MarginBounds.Height;

            float linhasPorPagina = e.MarginBounds.Height / RowFont.GetHeight(e.Graphics);
            float margemEsquerda = e.MarginBounds.Left;
            float margemSuperior = e.MarginBounds.Top;
            //string tcampo = "";
            RowForeBrush = new SolidBrush(Color.Black);

            
            List<float> colunas_left = new List<float>();
            List<float> tamanhos = new List<float>();
            List<string> titulos = new List<string>();
            float totcol = 40 + 25 + 12 + 12 + Convert.ToSingle(3);
            float espaco = (Convert.ToSingle(1) / totcol) * largura;
           
            Font fonte = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            tamanhos.Add(e.Graphics.MeasureString("ABCDETGHIJLMNORSTUVXZCCEFGHIJLMNO", fonte).Width);
            tamanhos.Add(e.Graphics.MeasureString("ABCDE_GHIJLMNO#XXRSTUVXZA", fonte).Width);
            tamanhos.Add(e.Graphics.MeasureString("9.999.999,99", fonte).Width);
            tamanhos.Add(e.Graphics.MeasureString("123456789 ", fonte).Width);
            titulos.Add("Historico");
            colunas_left.Add(0);
            titulos.Add("Lanc.Contábil");
            colunas_left.Add(colunas_left[0] + tamanhos[0] + espaco);
            titulos.Add("Vlr.Lanc.");
            colunas_left.Add(colunas_left[1] + tamanhos[1] + espaco);
            titulos.Add("Doc.Fisc.");
            colunas_left.Add(colunas_left[2] + tamanhos[2] + 3*espaco);
            


            /* tcampo = "Histórico".PadRight(45) + "Lanc.Contábil".PadRight(25) + "Vlr.Lanc.".PadLeft(12) + "Doc.Fiscal".PadRight(12);
             rectangleF = new RectangleF((float)LeftMargin, CurrentY, 
                   (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                     e.Graphics.MeasureString(tcampo, fonte).Height);
             e.Graphics.DrawString(tcampo, fonte, new SolidBrush(Color.Black), rectangleF,stringFormat);
            */

            //      Font fonte = new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            float alturaLinha = e.Graphics.MeasureString("Qualquer String", fonte).Height;
            CurrentY = CurrentY + margemSuperior+alturaLinha;
            int max_num_linha = Convert.ToInt32((e.MarginBounds.Height - margemSuperior) / CurrentY);
            //CurrentY = margemSuperior + (alturaPadrao *3);
            
            try
            {
                int contlinha = 0;
             
                RectangleF rectangleF;
                StringFormat stringFormat;
                while (CurrentRow < recibo.recibos.Count)
                {
                    RecibosDet orecibo = recibo.recibos[CurrentRow];

                    foreach (string doc in orecibo.Linhas)
                    {
                        stringFormat = new StringFormat();
                        stringFormat.Trimming = StringTrimming.Word;
                        stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                        stringFormat.Alignment = StringAlignment.Near;
                    
                        rectangleF = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                             e.Graphics.MeasureString(doc, fonte).Height);

                        e.Graphics.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                        CurrentY = CurrentY + e.Graphics.MeasureString(doc, fonte).Height + (alturaLinha / 2);
                    }
                    // titulo historico
                    stringFormat = new StringFormat();
                    stringFormat.Trimming = StringTrimming.Word;
                    stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    stringFormat.Alignment = StringAlignment.Near;
                    float cur = 0;
                    for(int i = 0; i < titulos.Count; i++ )
                    {
                        rectangleF = new RectangleF(((float)LeftMargin + colunas_left[i]), CurrentY,
                           tamanhos[i] ,
                             e.Graphics.MeasureString(titulos[i], fonte).Height);
                        stringFormat.Alignment = StringAlignment.Near;
                        if (i == 2) stringFormat.Alignment = StringAlignment.Far;
                       e.Graphics.DrawString(titulos[i], fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                       if (i == 0)
                            cur = e.Graphics.MeasureString(titulos[i], fonte).Height + (alturaLinha / 2);
                    }
                  
                    CurrentY = CurrentY  + cur;
                    contlinha = 0;
                    foreach (ItemsRecibo oItems in orecibo.Items)
                    {
                        stringFormat = new StringFormat();
                        stringFormat.Trimming = StringTrimming.Word;
                        stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                        stringFormat.Alignment = StringAlignment.Near;
                        cur = 0;
                        for (int i = 0; i < titulos.Count; i++)
                        {
                            string value = "";
                            if (i == 0)
                            {
                                if (oItems.Historico.Length > 33)
                                    value = oItems.Historico.Substring(0, 33);
                                value = oItems.Historico;
                            }
                            else if (i == 1) value = oItems.LancContab;
                            else if (i == 2) value = String.Format("{0:#,###,##0.00}", oItems.Valor);
                            else if (i == 3) value = oItems.DocFisc;
                            rectangleF = new RectangleF(((float)LeftMargin + colunas_left[i]), CurrentY,
                               tamanhos[i],
                                 e.Graphics.MeasureString(value, fonte).Height);
                            stringFormat.Alignment = StringAlignment.Near;
                            if (i == 2) stringFormat.Alignment = StringAlignment.Far;
                            e.Graphics.DrawString(value, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                            if (i == 0)
                                cur = e.Graphics.MeasureString(value, fonte).Height  +(alturaLinha / 2);
                        }
                        CurrentY = CurrentY + cur;
                        if (contlinha > 30)
                        {
                            contlinha = 0;
                            return true;
                        }
                        contlinha++;
                    }
                    // Local,DAta e Assinatura
                    if (orecibo.Items.Count < 11)
                    {  for (int i = orecibo.Items.Count; i < 11; i++)
                        {
                            CurrentY = CurrentY + alturaLinha + (alturaLinha / 2);
                        }
                    }

                    stringFormat.Alignment = StringAlignment.Center;
                    float tamLocalDta = e.Graphics.MeasureString(orecibo.local + "," + orecibo.dataExtendida, fonte).Width;
                    string linhaAssinatura = "_______________________________________________________".PadRight(50);
                    rectangleF = new RectangleF((float)LeftMargin + tamLocalDta, CurrentY,
                        (float)PageWidth - (float)RightMargin - (float)LeftMargin - tamLocalDta,
                          e.Graphics.MeasureString(linhaAssinatura, fonte).Height);
                    e.Graphics.DrawString(linhaAssinatura, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                    CurrentY = CurrentY + e.Graphics.MeasureString(linhaAssinatura, fonte).Height + (alturaLinha / 2);

                    stringFormat = new StringFormat();
                    stringFormat.Trimming = StringTrimming.Word;
                    stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                    stringFormat.Alignment = StringAlignment.Near;
                    rectangleF = new RectangleF((float)LeftMargin, CurrentY,
                          (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                            e.Graphics.MeasureString(orecibo.local + "," + orecibo.dataExtendida, fonte).Height);
                    e.Graphics.DrawString(orecibo.local+","+orecibo.dataExtendida, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);

                    
                    stringFormat.Alignment = StringAlignment.Center;
                    rectangleF = new RectangleF((float)LeftMargin + tamLocalDta , CurrentY,
                          (float)PageWidth - (float)RightMargin - (float)LeftMargin - tamLocalDta,
                            e.Graphics.MeasureString(orecibo.titular.ToUpper(), fonte).Height);
                    e.Graphics.DrawString(orecibo.titular.ToUpper(), fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                    CurrentY = CurrentY + e.Graphics.MeasureString(orecibo.titular.ToUpper().PadLeft(50), fonte).Height + (alturaLinha / 2);

                   


                    CurrentRow++;

                    if (recibo.recibos.Count == CurrentRow)
                    {
                        CurrentRow = 0;
                        return false;

                    }
                    if ((CurrentRow % 2) == condicao_par_impar) // condição do segundo item dentro a folha
                    {
                        // recibo.recibos[CurrentRow].Items < 11

                        // imprime na mesma pagina (2 recibo) SE o recibo anterior foi de tamanho normal (até a metade da pagina,
                        // e SE este proximo couber na metade desta pagina que sobro
                        if ( (orecibo.Items.Count < 11) && (recibo.recibos[CurrentRow].Items.Count < 11) )
                        {
                            CurrentY = (altura / 2);
                            DrawHeader(e.Graphics, true);
                           
                            continue;
                        }
                        else
                        {
                           if (condicao_par_impar == 1) condicao_par_impar = 0;
                              else condicao_par_impar = 1;
                            
                        }
                    }

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
        private void DrawHeader(Graphics g, bool mesmalinha = false)
        {
            
            float maiorCurrentY = 0;
            if (!mesmalinha)
                CurrentY = (float)TopMargin + 10;
           

            Font fonte = new Font("Arial", 10, FontStyle.Bold, GraphicsUnit.Point);
            string doc = "Doc N.:__________"; // 

            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Far;
         

             RectangleF rectangleF = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                  g.MeasureString(doc, fonte).Height);

            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);

            

             if (g.MeasureString(doc, fonte).Height > maiorCurrentY)
                 maiorCurrentY = g.MeasureString(doc, fonte).Height;
            

            fonte = new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);
          //  location = new Point(350, 35);
           // tam = new SizeF(100, 24);
          //  rectangleF = new RectangleF(location, tam);
            doc = "R E C I B O"; // 

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Center;

            rectangleF = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                g.MeasureString(doc, fonte).Height);

            
            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
            if (g.MeasureString(doc, fonte).Height > maiorCurrentY)
                maiorCurrentY = g.MeasureString(doc, fonte).Height;



            
            
            fonte = new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            doc = recibo.empresa; // 

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Near;
            rectangleF = new RectangleF((float)LeftMargin, CurrentY,
                   (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                    g.MeasureString(doc, fonte).Height);


            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
            if (g.MeasureString(doc, fonte).Height > maiorCurrentY)
                maiorCurrentY = g.MeasureString(doc, fonte).Height;

            if (!mesmalinha)
            {
                CurrentY = maiorCurrentY;
                CurrentY = CurrentY + 20;
            }
            else
            {
                CurrentY = CurrentY + 20 + maiorCurrentY;
            }
        }


        public bool DrawDataGridView(System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                DrawHeader(e.Graphics);
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
/*
 * object FrmQr2: TFrmQr2
  Left = 213
  Top = 105
  Caption = 'FrmQr2'
  ClientHeight = 337
  ClientWidth = 528
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Scaled = False
  PixelsPerInch = 96
  TextHeight = 13
  object QuickRep1: TQuickRep
    Left = 0
    Top = 844
    Width = 816
    Height = 1056
    Font.Charset = DEFAULT_CHARSET
    Font.Color = clWindowText
    Font.Height = -13
    Font.Name = 'Arial'
    Font.Style = []
    Functions.Strings = (
      'PAGENUMBER'
      'COLUMNNUMBER'
      'REPORTTITLE'
      'QRSTRINGSDETALHE'
      'QRSTRINGSHIST'
      'QRSTRINGSREC'
      'QRSTRINGSBAND1')
    Functions.DATA = (
      '0'
      '0'
      #39#39
      #39#39
      #39#39
      #39#39
      #39#39)
    Options = [FirstPageHeader, LastPageFooter]
    Page.Columns = 1
    Page.Orientation = poPortrait
    Page.PaperSize = Letter
    Page.Continuous = False
    Page.Values = (
      0.000000000000000000
      2794.000000000000000000
      0.000000000000000000
      2159.000000000000000000
      10.000000000000000000
      10.000000000000000000
      0.000000000000000000)
    PrinterSettings.Copies = 1
    PrinterSettings.OutputBin = Auto
    PrinterSettings.Duplex = False
    PrinterSettings.FirstPage = 0
    PrinterSettings.LastPage = 0
    PrinterSettings.UseStandardprinter = False
    PrinterSettings.UseCustomBinCode = False
    PrinterSettings.CustomBinCode = 0
    PrinterSettings.ExtendedDuplex = 0
    PrinterSettings.UseCustomPaperCode = False
    PrinterSettings.CustomPaperCode = 0
    PrinterSettings.PrintMetaFile = False
    PrinterSettings.PrintQuality = 0
    PrinterSettings.Collate = 0
    PrinterSettings.ColorOption = 0
    PrintIfEmpty = True
    SnapToGrid = True
    Units = MM
    Zoom = 100
    PrevFormStyle = fsNormal
    PreviewInitialState = wsNormal
    PrevInitialZoom = qrZoomToFit
    PreviewDefaultSaveType = stQRP
    PreviewLeft = 0
    PreviewTop = 0
    object QRSubDetalhe: TQRSubDetail
      Left = 4
      Top = 136
      Width = 808
      Height = 3
      AlignToBottom = False
      Color = clWhite
      TransparentBand = False
      ForceNewColumn = False
      ForceNewPage = False
      Size.Values = (
        7.937500000000000000
        2137.833333333333000000)
      PreCaluculateBandHeight = False
      KeepOnOnePage = False
      Master = QRStringsREC
      FooterBand = QRBand1
      PrintBefore = False
      PrintIfEmpty = True
    end
    object QRStringsREC: TQRStringsBand
      Left = 4
      Top = 0
      Width = 808
      Height = 136
      AlignToBottom = False
      Color = clWhite
      TransparentBand = False
      ForceNewColumn = False
      ForceNewPage = False
      LinkBand = QRBand1
      Size.Values = (
        359.833333333333300000
        2137.833333333333000000)
      PreCaluculateBandHeight = False
      KeepOnOnePage = False
      Master = QuickRep1
      PrintBefore = False
      object QRLabel1: TQRLabel
        Left = 350
        Top = 35
        Width = 100
        Height = 24
        Size.Values = (
          63.500000000000000000
          926.041666666666700000
          92.604166666666670000
          264.583333333333400000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Caption = 'R E C I B O'
        Color = clWhite
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -19
        Font.Name = 'Arial'
        Font.Style = [fsBold]
        ParentFont = False
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 14
      end
      object QRExprLin1: TQRExpr
        Left = 40
        Top = 82
        Width = 71
        Height = 19
        Size.Values = (
          50.270833333333330000
          105.833333333333300000
          216.958333333333400000
          187.854166666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Arial'
        Font.Style = []
        Color = clWhite
        ParentFont = False
        ResetAfterPrint = False
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRExprLin2: TQRExpr
        Left = 40
        Top = 107
        Width = 71
        Height = 19
        Size.Values = (
          50.270833333333330000
          105.833333333333300000
          283.104166666666700000
          187.854166666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Arial'
        Font.Style = []
        Color = clWhite
        ParentFont = False
        ResetAfterPrint = False
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRLbEmpresa: TQRLabel
        Left = 40
        Top = 35
        Width = 113
        Height = 20
        Size.Values = (
          52.916666666666660000
          105.833333333333300000
          92.604166666666670000
          298.979166666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Caption = 'QRLbEmpresa'
        Color = clWhite
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -16
        Font.Name = 'Arial'
        Font.Style = [fsBold]
        ParentFont = False
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 12
      end
      object QRDoc: TQRLabel
        Left = 650
        Top = 35
        Width = 116
        Height = 17
        Size.Values = (
          44.979166666666670000
          1719.791666666667000000
          92.604166666666670000
          306.916666666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Caption = 'Doc N.:__________'
        Color = clWhite
        Font.Charset = DEFAULT_CHARSET
        Font.Color = clWindowText
        Font.Height = -13
        Font.Name = 'Arial'
        Font.Style = [fsBold]
        ParentFont = False
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
    end
    object QRStringsBand1: TQRStringsBand
      Left = 4
      Top = 139
      Width = 808
      Height = 21
      AlignToBottom = False
      Color = clWhite
      TransparentBand = False
      ForceNewColumn = False
      ForceNewPage = False
      Size.Values = (
        55.562500000000000000
        2137.833333333333000000)
      PreCaluculateBandHeight = False
      KeepOnOnePage = False
      Master = QRSubDetalhe
      PrintBefore = False
      object QRExpr1: TQRExpr
        Left = 26
        Top = 2
        Width = 193
        Height = 17
        Size.Values = (
          44.979166666666670000
          68.791666666666670000
          5.291666666666667000
          510.645833333333300000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Color = clWhite
        ResetAfterPrint = False
        Transparent = False
        Expression = 'COPY(QRSTRINGSBAND1,1,45)'
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRExpr2: TQRExpr
        Left = 215
        Top = 2
        Width = 200
        Height = 17
        Size.Values = (
          44.979166666666670000
          568.854166666666700000
          5.291666666666667000
          529.166666666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Color = clWhite
        ResetAfterPrint = False
        Transparent = False
        Expression = 'COPY(QRSTRINGSBAND1,46,25)'
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRExpr3: TQRExpr
        Left = 306
        Top = 2
        Width = 200
        Height = 17
        Size.Values = (
          44.979166666666670000
          809.625000000000000000
          5.291666666666667000
          529.166666666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Color = clWhite
        ResetAfterPrint = False
        Transparent = False
        Expression = 'COPY(QRSTRINGSBAND1,71,12)'
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRExpr4: TQRExpr
        Left = 633
        Top = 2
        Width = 200
        Height = 17
        Size.Values = (
          44.979166666666670000
          1674.812500000000000000
          5.291666666666667000
          529.166666666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Color = clWhite
        ResetAfterPrint = False
        Transparent = False
        Expression = 'COPY(QRSTRINGSBAND1,83,12)'
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
    end
    object QRBand1: TQRBand
      Left = 4
      Top = 160
      Width = 808
      Height = 200
      AlignToBottom = False
      Color = clWhite
      TransparentBand = False
      ForceNewColumn = False
      ForceNewPage = False
      LinkBand = QRStringsBand1
      Size.Values = (
        529.166666666666700000
        2137.833333333333000000)
      PreCaluculateBandHeight = False
      KeepOnOnePage = False
      BandType = rbGroupFooter
      object QRLbLocalData: TQRLabel
        Left = 40
        Top = 165
        Width = 92
        Height = 17
        Size.Values = (
          44.979166666666670000
          105.833333333333300000
          436.562500000000000000
          243.416666666666700000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Caption = 'QRLbLocalData'
        Color = clWhite
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRLBnOME: TQRLabel
        Left = 406
        Top = 165
        Width = 73
        Height = 17
        Size.Values = (
          44.979166666666670000
          1074.208333333333000000
          436.562500000000000000
          193.145833333333300000)
        XLColumn = 0
        Alignment = taLeftJustify
        AlignToBand = False
        Caption = 'QRLBnOME'
        Color = clWhite
        Transparent = False
        ExportAs = exptText
        WrapStyle = BreakOnSpaces
        FontSize = 10
      end
      object QRShape1: TQRShape
        Left = 325
        Top = 161
        Width = 324
        Height = 1
        Size.Values = (
          2.645833333333330000
          859.895833333333000000
          425.979166666667000000
          857.250000000000000000)
        XLColumn = 0
        Shape = qrsRectangle
        VertAdjust = 0
      end
    end
  end
end
 */ 