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


namespace ApoioContabilidade.Trabalho.Imprimir
{
    class FolhaImp
    {

        private BindingSource folha;
        //  private DataGridView TheDataGridView; // The DataGridView Control which will be printed
        private PrintDocument ThePrintDocument; // The PrintDocument to be used for printing
        public Font TheTitleFont; // The font to be used with the title text (if IsWithTitle is set to true)

        private string nomeEmpresa;
        private string titulo;
        private string fazenda; // descricao do Setor
        private string setor;
        int CurrentRow; // A static parameter that keep track on which Row (in the DataGridView control) that should be printed


        private int PageWidth;
        private int PageHeight;
        private int LeftMargin;
        private int TopMargin;
        private int RightMargin;
        private int BottomMargin;
        private int PageNumber;

        private float CurrentY; // A parameter that keep track on the y coordinate of the page, so the next object to be printed will start from this y coordinate


        List<float> colunas_left;
        List<float> tamanhos;
        List<string> titulos;

        // The class constructor
        public FolhaImp(BindingSource ofolha, PrintDocument aPrintDocument,
               Font aTitleFont, string onomeEmpresa, string otitulo, string ofazenda, string osetor)
        {
            folha = ofolha;
            ThePrintDocument = aPrintDocument;
            TheTitleFont = aTitleFont;
            nomeEmpresa = onomeEmpresa;
            fazenda = ofazenda;
            setor = osetor;
            titulo = otitulo;
            PageNumber = 0;
            if (folha.Count > 0)
                folha.MoveFirst();

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

        private bool DrawRows(System.Drawing.Printing.PrintPageEventArgs e)
        {

            // Setting the LinePen that will be used to draw lines and rectangles (derived from the GridColor property of the DataGridView control)
            Pen TheLinePen = new Pen(Color.LightGray, 1);

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


            colunas_left = new List<float>();
            tamanhos = new List<float>();
            titulos = new List<string>();
           // float totcol = 40 + 25 + 12 + 12 + Convert.ToSingle(3);
            float espaco = 0;// (Convert.ToSingle(1) / totcol) * largura;

            Font fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point); //new Font("Arial", 10, FontStyle.Regular, GraphicsUnit.Point);
            tamanhos.Add(e.Graphics.MeasureString("ABCDETGHMNORZCJLNOPQRSTXZ", fonte).Width);
            titulos.Add("N. Nome Trabalhador"); ;
            colunas_left.Add(0);
            // dias do ponto
            titulos.Add("PONTO");
            fonte = new Font("MS Sans Serif", 5, FontStyle.Regular, GraphicsUnit.Point);
            for (int i = 0; i < 16; i++)
            {
                colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
                tamanhos.Add(e.Graphics.MeasureString("FE", fonte).Width);

            }
            fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);

            titulos.Add("Dias");
            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("999,9", fonte).Width);

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("9.999.999,99", fonte).Width);
            titulos.Add("Salario");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("99.999,99", fonte).Width);
            titulos.Add("HEXT");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("99.999,99", fonte).Width);
            titulos.Add("GRATIF");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("9.999.999,99", fonte).Width);
            titulos.Add("SALBRUTO");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("99.999,99", fonte).Width);
            titulos.Add("SALFAM");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("99.999,99", fonte).Width);
            titulos.Add("INSS");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("99.999,99", fonte).Width);
            titulos.Add("IRFONTE");

            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            tamanhos.Add(e.Graphics.MeasureString("9.999.999,99", fonte).Width);
            titulos.Add("SALLIQ");



            colunas_left.Add(colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1] + espaco);
            float tamanho_assinaturas = (PageWidth - LeftMargin - RightMargin) - colunas_left[colunas_left.Count - 1];
            tamanhos.Add(tamanho_assinaturas);
            titulos.Add("ASSINATURAS");


            /*  Colunas.Add("ISIND"); -- Depreciado
                              
            */





            float alturaLinha = e.Graphics.MeasureString("Qualquer String", fonte).Height + 4+ 5;
            CurrentY = CurrentY + margemSuperior + alturaLinha;
            int max_num_linha = Convert.ToInt32((e.MarginBounds.Height - margemSuperior) / CurrentY);




            //PointF inicio;
            // PointF fim;
            try
            {
                int contlinha = 0;

                RectangleF rectangleF;
                StringFormat stringFormat;
                List<string> Colunas = new List<string>();
                //  Colunas.Add("COD");
                Colunas.Add("NOME");
                Colunas.Add("PONTO");
                Colunas.Add("DIAS");
                Colunas.Add("SALARIO");
                Colunas.Add("HEXT");
                Colunas.Add("GRATIF");
                Colunas.Add("SALBRUTO");
                Colunas.Add("SALFAM");
                Colunas.Add("INSS");
                Colunas.Add("IRFONTE");
                // Colunas.Add("ISIND"); depreciado
                Colunas.Add("SALLIQ");
                // folha.MoveFirst();
                contlinha = 0;

                Cabecalho(e.Graphics, alturaLinha);
                /*    CurrentY = CurrentY + (alturaLinha * 2);

                    // linha horizontal
                    inicio = new PointF((float)LeftMargin + colunas_left[0], CurrentY - 2);
                    fim = new PointF((float)LeftMargin + colunas_left[colunas_left.Count - 1], CurrentY - 2);
                    e.Graphics.DrawLine(TheLinePen, inicio, fim);

                    */

                while (CurrentRow < folha.Count)
                {
                    DataRowView olinha = (folha.Current as DataRowView);

                    float cur = 0;
                    // CurrentY = 0;
                    float linhaY = CurrentY;
                    foreach (string coluna in Colunas)
                    {

                        stringFormat = new StringFormat();
                        stringFormat.Trimming = StringTrimming.Word;
                        stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
                        stringFormat.Alignment = StringAlignment.Near;
                        cur = 0;
                        linhaY = CurrentY;
                        int iTam = 0;
                        if (coluna == "PONTO")
                        {
                            fonte = new Font("MS Sans Serif", 5, FontStyle.Regular, GraphicsUnit.Point);

                            string ponto = olinha["PONTO"].ToString();
                            /*if (olinha["COD"].ToString() == "9917")
                            {
                                iTam = 0;
                            }*/
                            int tamPonto = ponto.Length;

                            int i_subs = 0;
                            for (int linhaPonto = 0; linhaPonto < 2; linhaPonto++)
                            {
                                iTam = 1;

                                for (int i = 0; i < 16; i++)
                                {
                                    string value = "";
                                    if ((linhaPonto == 0) && (i == 15))
                                    {
                                        value = " ";
                                    }
                                    else
                                    {
                                        if ((i_subs + 2) > tamPonto)
                                        {
                                            value = " ";
                                        }
                                        else value = ponto.Substring(i_subs, 2);
                                    }
                                    rectangleF = new RectangleF(((float)LeftMargin + colunas_left[iTam]), linhaY,
                                   tamanhos[iTam], alturaLinha);
                                    // e.Graphics.MeasureString(value, fonte).Height);
                                    stringFormat.Alignment = StringAlignment.Near;
                                    stringFormat.LineAlignment = StringAlignment.Center;
                                    //if (i == 2) stringFormat.Alignment = StringAlignment.Far;
                                    e.Graphics.DrawString(value, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                                    cur = e.Graphics.MeasureString(value, fonte).Height;// + (alturaLinha / 2);
                                                                                        // SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha);
                                    if (!((linhaPonto == 0) && (i == 15)))
                                        i_subs = i_subs + 2;
                                    iTam++;
                                }
                                linhaY = linhaY + cur;
                            }
                            iTam = 1;
                            for (int i = 0; i < 16; i++)
                            {
                                SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                                iTam++;
                            }
                        }
                        else if (coluna == "NOME")
                        {
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);

                            iTam = 0;
                            string nome = olinha["NOME"].ToString().Trim();
                            string cod = olinha["COD"].ToString();
                            

                           
                            List<string> lstNomes = NomePartido(nome);
                            float cur_nome = ((alturaLinha * 2) / 3) * 2;
                            float cur_primeiro = e.Graphics.MeasureString(lstNomes[0], fonte).Height;
                            if (lstNomes[1].Trim().Length == 0)
                                cur_nome = (cur_nome - cur_primeiro) / 2;
                            else
                                cur_nome = (cur_nome - (cur_primeiro * 2) ) / 2;
                            if (cur_nome < 0)
                            {
                                cur_nome = 0;
                            }
                            linhaY = CurrentY + cur_nome;
                            for (int i = 0; i < lstNomes.Count; i++)
                            {
                                rectangleF = new RectangleF(((float)LeftMargin + colunas_left[iTam]), linhaY,
                                 tamanhos[iTam],
                                 e.Graphics.MeasureString(lstNomes[i], fonte).Height);
                                stringFormat.Alignment = StringAlignment.Near;
                                //if (i == 2) stringFormat.Alignment = StringAlignment.Far;
                                e.Graphics.DrawString(lstNomes[i], fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                                cur = e.Graphics.MeasureString(lstNomes[i], fonte).Height; // + (alturaLinha / 2);
                                linhaY = linhaY + cur;
                            }
                            //linhaY = linhaY + cur;
                            // ultima linha 
                            fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);

                            float cur_ult = (((alturaLinha * 2) / 3) * 2) ;
                            linhaY = CurrentY + cur_ult;
                            rectangleF = new RectangleF(((float)LeftMargin + colunas_left[iTam]), linhaY,
                                tamanhos[iTam],
                             e.Graphics.MeasureString("Código:" + cod, fonte).Height);
                            stringFormat.Alignment = StringAlignment.Near;
                            //if (i == 2) stringFormat.Alignment = StringAlignment.Far;
                            e.Graphics.DrawString("Cod:" + cod, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
                            cur = e.Graphics.MeasureString("Cod:" + cod, fonte).Height;// + (alturaLinha / 2);
                                                                                          // SeparadorColunas(e.Graphics, linhaY, iTam, cur);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                            linhaY = linhaY + cur;
                        }

                        else if (coluna == "DIAS")
                        {
                            iTam = 17;
                            string valor = String.Format("{0:#,###,##0.0}", Convert.ToDecimal(olinha["DIAS"])).Trim();

                            fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "SALARIO")
                        {
                            iTam = 18;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha["SALARIO"])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "HEXT")
                        {
                            iTam = 19;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "GRATIF")
                        {
                            iTam = 20;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "SALBRUTO")
                        {
                            iTam = 21;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "SALFAM")
                        {
                            iTam = 22;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "INSS")
                        {
                            iTam = 23;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "IRFONTE")
                        {
                            iTam = 24;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }
                        else if (coluna == "SALLIQ")
                        {
                            iTam = 25;
                            string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(olinha[coluna])).Trim();
                            fonte = new Font("MS Sans Serif", 7, FontStyle.Regular, GraphicsUnit.Point);
                            StringFormat ostringFormat = new StringFormat();
                            ostringFormat.Alignment = StringAlignment.Far;

                            LinhaDupla(e.Graphics, CurrentY, iTam, alturaLinha, "", valor, fonte, ostringFormat);
                            SeparadorColunas(e.Graphics, CurrentY, iTam, alturaLinha * 2);
                        }




                    }
                    CurrentY = CurrentY + (alturaLinha * 2);

                    // linha horizontal
                    LinhaHorizontal(e.Graphics);




                    if (contlinha > 14)
                    {
                        contlinha = 0;
                        folha.MoveNext();
                        CurrentRow++;

                        return true;
                    }
                    contlinha++;



                    folha.MoveNext();
                    CurrentRow++;



                    // return false;
                }
            }
            catch (Exception E)
            {
                if (folha.Count > 0)
                {
                    folha.MoveFirst();
                }
                PageNumber = 0;
                MessageBox.Show(E.Message);

                throw;
            }
            CurrentRow = 0;
            if (folha.Count > 0)
            {
                folha.MoveFirst();
            }
            PageNumber = 0;
            return false;
        }
        private void Cabecalho(Graphics g, float altura)
        {
            float currentY = CurrentY;

            LinhaHorizontal(g);

            // Nome Traballhhador
            int iTam = 0;
            Font fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "", "Nome Trabalhador", fonte);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            // Nome Ponto
            iTam = 1;
            for (int i = 0; i < 16; i++)
            {
                string linha1 = i < 15 ? (i + 1).ToString().PadLeft(2, Convert.ToChar(" ")) : "  ";
                string linha2 = (i + 16).ToString();
                fonte = new Font("MS Sans Serif", 5, FontStyle.Bold, GraphicsUnit.Point);

                LinhaDupla(g, currentY, iTam, altura, linha1, linha2, fonte);
                SeparadorColunas(g, currentY, iTam, altura * 2);
                iTam++;
            }
            StringFormat stringFormat = new StringFormat();
            stringFormat.Alignment = StringAlignment.Far;
            // Dias
            iTam = 17;
            fonte = new Font("MS Sans Serif", 6, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "", "Dias", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            // salario
            iTam = 18;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "Total", "Salário", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            // Valor H.Extras
            iTam = 19;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "Vlr H.", "Extras", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);
            // Gratif
            iTam = 20;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "", "Gratif", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);
            // salario Bruto
            iTam = 21;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "Salário", "Bruto", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            // salario Família
            iTam = 22;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "Salário", "Família", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            // INSS
            iTam = 23;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "", "INSS", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            // IRFONTE
            iTam = 24;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "I.Renda", "Fonte", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);
            // Sal Liq
            iTam = 25;
            fonte = new Font("MS Sans Serif", 7, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "Salário", "Liquido", fonte, stringFormat);
            SeparadorColunas(g, currentY, iTam, altura * 2);

            stringFormat.Alignment = StringAlignment.Center;
            iTam = 26;
            fonte = new Font("MS Sans Serif", 8, FontStyle.Bold, GraphicsUnit.Point);
            LinhaDupla(g, currentY, iTam, altura, "", "ASSINATURAS", fonte, stringFormat);
            //SeparadorColunas(g, currentY, iTam, altura * 2);



            CurrentY = CurrentY + (altura * 2);

            // linha horizontal
            LinhaHorizontal(g);
        }


        private List<string> NomePartido(string nome)
        {
            List<string> nomes_result = new List<string>();
            List<string> nomes = nome.Split(Convert.ToChar(" ")).ToList();
            int acum = 0;
            string acumula = "";
            foreach (string parte in nomes)
            {
                if ((parte.Length + acum + 1) <= 25)
                {
                    acumula = acumula + parte + " ";
                    acum = acumula.Length;
                }
                else
                {
                    nomes_result.Add(acumula.Trim());
                    acumula = parte + " ";
                    acum = acumula.Length; 
                }
            }
            nomes_result.Add(acumula.Trim());
            if (nomes_result.Count == 1)
            {
                nomes_result.Add("");
            }
            return nomes_result;
        }


        private void LinhaHorizontal(Graphics g)
        {
            Pen TheLinePen = new Pen(Color.Gray, 1);
            PointF inicio = new PointF((float)LeftMargin + colunas_left[0], CurrentY - 2);
            PointF fim = new PointF((float)LeftMargin + colunas_left[colunas_left.Count - 1] + tamanhos[tamanhos.Count - 1], CurrentY - 2);
            g.DrawLine(TheLinePen, inicio, fim);

        }
        private void LinhaDupla(Graphics g, float currentY, int iTam, float altura, string linha1, string linha2,
            Font fonte, StringFormat ostringFormat = null)
        {

            RectangleF rectangleF;
            StringFormat stringFormat;


            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Near;

            if (ostringFormat != null)
            {
                stringFormat.Alignment = ostringFormat.Alignment;
            }


            //  float cur;
            float linhaY;

            stringFormat.LineAlignment = StringAlignment.Center;

            linhaY = currentY;
            // altura = alturaLinha; //      g.MeasureString(linha1, fonte).Height;

            rectangleF = new RectangleF(((float)LeftMargin + colunas_left[iTam]), linhaY,
                    tamanhos[iTam],
               altura);
            // stringFormat.Alignment = StringAlignment.Near;
            //if (i == 2) stringFormat.Alignment = StringAlignment.Far;
            g.DrawString(linha1, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
            /*cur = g.MeasureString(linha1, fonte).Height;
            if (cur == 0)
            {
                cur = g.MeasureString("Ab", fonte).Height;
            }
            */
            linhaY = linhaY + (altura - 1);
            rectangleF = new RectangleF(((float)LeftMargin + colunas_left[iTam]), linhaY,
             tamanhos[iTam],
             altura);
            // stringFormat.Alignment = StringAlignment.Near;
            //if (i == 2) stringFormat.Alignment = StringAlignment.Far;
            g.DrawString(linha2, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);

        }

        private void LinhaSimples(Graphics g, float currentY, int iTam, float altura, string linha1,
            Font fonte, StringFormat ostringFormat = null)
        {

            RectangleF rectangleF;
            StringFormat stringFormat;


            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Near;

            if (ostringFormat != null)
            {
                stringFormat.Alignment = ostringFormat.Alignment;
            }


            //  float cur;
            float linhaY;

            stringFormat.LineAlignment = StringAlignment.Center;

            linhaY = currentY;
            // altura = alturaLinha; //      g.MeasureString(linha1, fonte).Height;

            rectangleF = new RectangleF(((float)LeftMargin + colunas_left[iTam]), linhaY,
                    tamanhos[iTam],
               altura);
            // stringFormat.Alignment = StringAlignment.Near;
            //if (i == 2) stringFormat.Alignment = StringAlignment.Far;
            g.DrawString(linha1, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);

        }




        private void SeparadorColunas(Graphics g, float currentY, int iTam, float altura)
        {
            Pen TheLinePen = new Pen(Color.Gray, 1);
            PointF inicio = new PointF((float)LeftMargin + colunas_left[iTam] + tamanhos[iTam], currentY);
            PointF fim = new PointF((float)LeftMargin + colunas_left[iTam] + tamanhos[iTam], currentY + altura);
            g.DrawLine(TheLinePen, inicio, fim);

        }
        /*private void ImprimeLogo(System.Drawing.Printing.PrintPageEventArgs e, bool mesmalinha = false)
        {
            Bitmap imageToPr = Properties.Resources.ImagemMLSA_Reduzida;
            float x = e.MarginBounds.Left;
            float y = e.MarginBounds.Top;

            double factor = 2.54;
            int wPixel = Convert.ToInt32((27.0 / factor) * (double)100);
            int hPixel = Convert.ToInt32((18.0 / factor) * (double)100);

            Bitmap imageToPr = new Bitmap(wPixel, hPixel);
            Graphics toDr = Graphics.FromImage(imageToPr);
            toDr.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            toDr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            toDr.DrawImage(bmIm, new Rectangle(0, 0, wPixel, hPixel));
           
            e.Graphics.DrawImage(imageToPr, new Rectangle((int)x, (int)y, imageToPr.Width, imageToPr.Height), (float)0, 
                (float)0, (float)wPixel, (float)hPixel, GraphicsUnit.Pixel);

        }
        */
        private void ImprimeLogo(Graphics g)
        {


            Bitmap imageToPr = Properties.Resources.ImagemMLSA_LOGOTIPO2_AJADYR;                 //Properties.Resources.ImagemMLSA_Reduzida;

            float left = (float)LeftMargin + (float)20;
            float top = CurrentY - (float)10;
            RectangleF rectangleF = new RectangleF(left, top, imageToPr.Width,     // (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                 imageToPr.Width);

            g.DrawImage(imageToPr, rectangleF);


            // Mlibanio
            float left_mlsa = left - 20;
            float top_mlsa  = top + imageToPr.Width;

            Font fonte =  
                new Font("Bauhaus 93", 10, FontStyle.Regular, GraphicsUnit.Point);
            string doc = "M.LIBANIO"; // 

            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Near;

            rectangleF = new RectangleF(left_mlsa , top_mlsa , 
                    (float)PageWidth - left_mlsa - (float)RightMargin,
                      g.MeasureString(doc, fonte).Height
                    );

            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
            float largura_mlsa = g.MeasureString(doc, fonte).Height;
            float tam_mlsa = g.MeasureString("M.LIBANIO", fonte).Width;
            fonte =
              new Font("Bauhaus 93", 6, FontStyle.Regular, GraphicsUnit.Point);
            doc = "Desde 1921"; // 

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;


            rectangleF = new RectangleF(left_mlsa, top_mlsa + (largura_mlsa/2) + 4,
                    tam_mlsa,
                      g.MeasureString(doc, fonte).Height
                    );
            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);


        }

        /*private void ImprimeLogo_Outro(Graphics g)
        {

            Bitmap imageToPr = Properties.Resources.M_Libanio___logotipo;                 //Properties.Resources.ImagemMLSA_Reduzida;

            float left = (float)LeftMargin + (float)20;
            float top = CurrentY - 10; // 15
            RectangleF rectangleF = new RectangleF(left, top, imageToPr.Width,     // (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                 imageToPr.Width);

            g.DrawImage(imageToPr, rectangleF);


            // Mlibanio
            float left_mlsa = left - 20;
            float top_mlsa = top + imageToPr.Width;

            Font fonte =
                new Font("Bauhaus 93", 10, FontStyle.Regular, GraphicsUnit.Point);
            string doc = "M.LIBANIO"; // 

            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Near;

            rectangleF = new RectangleF(left_mlsa, top_mlsa,
                    (float)PageWidth - left_mlsa - (float)RightMargin,
                      g.MeasureString(doc, fonte).Height
                    );

            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
            float largura_mlsa = g.MeasureString(doc, fonte).Height;
            float tam_mlsa = g.MeasureString("M. LIBÂNIO", fonte).Width;
            float top_Agricola = top_mlsa + (largura_mlsa / 3) + 3; 
            // Agricola

            fonte =
             new Font("Bauhaus 93", 6, FontStyle.Regular, GraphicsUnit.Point);
            doc = "agrícola s.a."; // 

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Far;
            stringFormat.LineAlignment = StringAlignment.Center;


            rectangleF = new RectangleF(left_mlsa, top_Agricola,
                    tam_mlsa,
                      largura_mlsa
                    );
            // Color customColor = Color.FromArgb(50, Color.Gray);
            // SolidBrush shadowBrush = new SolidBrush(customColor);

            g.DrawString(doc, fonte, new SolidBrush(Color.Gray), rectangleF, stringFormat);

            // float largura_agricola = g.MeasureString(doc, fonte).Height;
            float top_Desde = top_Agricola  + g.MeasureString(doc, fonte).Height;


            fonte =
              new Font("Bauhaus 93", 6, FontStyle.Regular, GraphicsUnit.Point);
            
            
            doc = "Desde 1921"; // 

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Center;
            stringFormat.LineAlignment = StringAlignment.Center;


            rectangleF = new RectangleF(left_mlsa, top_Desde,
                    tam_mlsa,
                      g.MeasureString(doc, fonte).Height
                    );
           // Color customColor = Color.FromArgb(50, Color.Gray);
           // SolidBrush shadowBrush = new SolidBrush(customColor);
            
            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);



        }
        */
        private void DrawHeader(Graphics g, bool mesmalinha = false)
        {

            float maiorCurrentY = 0;
            if (!mesmalinha)
                CurrentY = (float)TopMargin + 10;

            ImprimeLogo(g);


            float MargemEsquerdaAdicional = 75.0F + 10;
            Font fonte = new Font("MS Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);
            string doc = fazenda.ToUpper(); // nomeEmpresa.ToUpper(); // 

            StringFormat stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Center;


           

            RectangleF rectangleF = new RectangleF((float)LeftMargin + MargemEsquerdaAdicional, 
                  CurrentY + 10, (float)PageWidth - (float)RightMargin - (float)LeftMargin - MargemEsquerdaAdicional,
               g.MeasureString(doc, fonte).Height);


           // g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);

            



            if (g.MeasureString(doc, fonte).Height > maiorCurrentY)
                maiorCurrentY = g.MeasureString(doc, fonte).Height;


            fonte = new Font("MS Sans Serif", 8, FontStyle.Bold, GraphicsUnit.Point); //new Font("Arial", 14, FontStyle.Bold, GraphicsUnit.Point);
            //  location = new Point(350, 35);
            // tam = new SizeF(100, 24);
            //  rectangleF = new RectangleF(location, tam);
            doc = titulo; // 

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Center;

            rectangleF = new RectangleF((float)LeftMargin, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin,
                g.MeasureString(doc, fonte).Height);


            g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);
            if (g.MeasureString(doc, fonte).Height > maiorCurrentY)
                maiorCurrentY = g.MeasureString(doc, fonte).Height;





            //  fonte = new Font("MS Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);//new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            PageNumber++;
            string PageString = "PAG. " + PageNumber.ToString();
            float MargemDireitaAdicional = 60.0F;


            StringFormat PageStringFormat = new StringFormat();
            PageStringFormat.Trimming = StringTrimming.Word;
            PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            PageStringFormat.Alignment = StringAlignment.Far;

            Font PageStringFont = new Font("Tahoma", 8, FontStyle.Regular, GraphicsUnit.Point);

            RectangleF PageStringRectangle = new RectangleF((float)LeftMargin, CurrentY, 
                (float)PageWidth - (float)RightMargin - (float)LeftMargin - MargemDireitaAdicional, g.MeasureString(PageString, PageStringFont).Height);

            // RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight[0]);



            g.DrawString(PageString, PageStringFont, new SolidBrush(Color.Black), PageStringRectangle, PageStringFormat);

            // Segunda Linha
            CurrentY = CurrentY + 25;// - 3;
            fonte = new Font("MS Sans Serif", 8, FontStyle.Bold, GraphicsUnit.Point);
            doc =   nomeEmpresa.Trim().ToUpper() + " - "+ fazenda.ToUpper(); // 
            

            stringFormat = new StringFormat();
            stringFormat.Trimming = StringTrimming.Word;
            stringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            stringFormat.Alignment = StringAlignment.Center;

            MargemEsquerdaAdicional = 0;
            rectangleF = new RectangleF((float)LeftMargin + MargemEsquerdaAdicional, CurrentY, (float)PageWidth - (float)RightMargin - (float)LeftMargin - MargemEsquerdaAdicional,
                 g.MeasureString(doc, fonte).Height);

             g.DrawString(doc, fonte, new SolidBrush(Color.Black), rectangleF, stringFormat);

           // CurrentY = CurrentY +  3;

            fonte = new Font("MS Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);
            // fonte = new Font("MS Sans Serif", 8, FontStyle.Italic, GraphicsUnit.Point);//new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point);
            MargemDireitaAdicional = 20.0F;
            PageString = "Responsável:.......................................................................................";
            PageStringFormat.Trimming = StringTrimming.Word;
            PageStringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            PageStringFormat.Alignment = StringAlignment.Far;

            PageStringFont = new Font("Tahoma", 8, FontStyle.Italic, GraphicsUnit.Point);

            PageStringRectangle = new RectangleF((float)LeftMargin, CurrentY + 10,
                (float)PageWidth - (float)RightMargin - (float)LeftMargin - MargemDireitaAdicional, g.MeasureString(PageString, PageStringFont).Height);

            // RectangleF CellBounds = new RectangleF(CurrentX, CurrentY, ColumnWidth, RowsHeight[0]);

            g.DrawString(PageString, PageStringFont, new SolidBrush(Color.Black), PageStringRectangle, PageStringFormat);
            if (g.MeasureString(PageString, fonte).Height > maiorCurrentY)
                maiorCurrentY = g.MeasureString(PageString, fonte).Height;
           
            if (!mesmalinha)
            {
                CurrentY = maiorCurrentY;
                CurrentY = CurrentY + 29;
            }
            else
            {
                CurrentY = CurrentY + 29 + maiorCurrentY;
            }
        }


        public bool DrawDataGridView(System.Drawing.Printing.PrintPageEventArgs e)
        {
            try
            {
                DrawHeader(e.Graphics);
                try
                {
                    bool bContinue = DrawRows(e);
                    if (bContinue == false)
                        DrawTotal(e.Graphics);
                    //DrawRodape(g);
                    return bContinue;
                }
                catch (Exception)
                {
                    return false;

                }


            }
            catch (Exception ex)
            {
                MessageBox.Show("Operação Falhou: " + ex.Message.ToString(), Application.ProductName + " - Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

        }
        private void DrawTotal(Graphics g)
        {

            List<string> Colunas = new List<string>();
            Colunas.Add("SALARIO");
            Colunas.Add("HEXT");
            Colunas.Add("GRATIF");
            Colunas.Add("SALBRUTO");
            Colunas.Add("SALFAM");
            Colunas.Add("INSS");
            Colunas.Add("IRFONTE");
            // Colunas.Add("ISIND"); depreciado
            Colunas.Add("SALLIQ");


            Dictionary<string, Decimal> dictColunas = new Dictionary<string, decimal>();

            /*foreach (string tot in Colunas)
            {
                Decimal valor = Convert.ToDecimal(
                    (folha.DataSource as DataView).
                    Where(a => a.Field<string>("SETOR").Trim() == setor.Trim()).Sum(a => a.Field<double>(tot)));
                dictColunas.Add(tot, valor);
            }*/
            
            foreach(DataRowView orow in (folha.DataSource as DataView))
            {
                foreach (string col in Colunas)
                {
                    Decimal valor = Convert.ToDecimal(
                       orow[col]);
                    if (!dictColunas.ContainsKey(col))
                    {
                        dictColunas.Add(col, valor);
                    }
                    else 
                    {
                        dictColunas[col] = dictColunas[col]  + valor;

                    }   
                }

            }


            Font fonte;
            float cur = 0;
            // CurrentY = 0;
            float linhaY = CurrentY;
            fonte = new Font("MS Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);
            float alturaLinha = g.MeasureString("Qualquer String", fonte).Height + 4;

            //

            fonte = new Font("MS Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point);
            StringFormat ostringFormat = new StringFormat();
            ostringFormat.Trimming = StringTrimming.Word;
            ostringFormat.FormatFlags = StringFormatFlags.NoWrap | StringFormatFlags.LineLimit | StringFormatFlags.NoClip;
            ostringFormat.Alignment = StringAlignment.Near;
            LinhaSimples(g, CurrentY, 0, alturaLinha, "TOTAIS", fonte, ostringFormat);
            SeparadorColunas(g, CurrentY, 0, alturaLinha);
            for (int i = 1; i < 18; i++)
            {
                LinhaSimples(g, CurrentY, 0, alturaLinha, "", fonte, ostringFormat);
            }
            SeparadorColunas(g, CurrentY, 17, alturaLinha);



            foreach (KeyValuePair<string, Decimal> coluna in dictColunas)
            {


                cur = 0;
                linhaY = CurrentY;
                int iTam = 0;
                
                if (coluna.Key == "SALARIO")
                {
                    iTam = 18;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "HEXT")
                {
                    iTam = 19;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "GRATIF")
                {
                    iTam = 20;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "SALBRUTO")
                {
                    iTam = 21;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "SALFAM")
                {
                    iTam = 22;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "INSS")
                {
                    iTam = 23;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "IRFONTE")
                {
                    iTam = 24;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
                else if (coluna.Key == "SALLIQ")
                {
                    iTam = 25;
                    string valor = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(coluna.Value)).Trim();
                    fonte = new Font("MS Sans Serif", 6, FontStyle.Regular, GraphicsUnit.Point);
                    ostringFormat.Alignment = StringAlignment.Far;

                    LinhaSimples(g, CurrentY, iTam, alturaLinha, valor, fonte, ostringFormat);
                    SeparadorColunas(g, CurrentY, iTam, alturaLinha);
                }
            }

            CurrentY = CurrentY + alturaLinha;

            // linha horizontal
            LinhaHorizontal(g);




        }

       
    }

}