using ApoioContabilidade.Financeiro.Imprimir;
using ApoioContabilidade.Financeiro.Models;
using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using System.Linq;
using System.Web.Services.Description;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro
{
    public partial class FrmCheques : Form
    {
        BindingSource bmCheques;
        DataTable cheques;
        MonteGrid oCheque;

        BindingSource bmChequesFilhos;
        DataTable chequesFilhos;
        MonteGrid oChequeFilhos;
        //  private int fontcount;
        private int fontposition = 1;
        private float ypos = 1;
        private PrintPreviewDialog previewDlg = null;
        private bool primeiraimpressao = false;
        ChequeImprime oChequeImprime;
        ReciboImprime oReciboImprime;
        ChequeImprimeFolha oChequeImprimeFolha;
        string tipoPapelCheque = "";
        BindingSource bmPagar;
        public FrmCheques(BindingSource obmPagar)
        {
            InitializeComponent();
            //  panel2.Visible = false;
            //  dgvChequesFilho.Visible = false;
            this.Text = "Cheques e Recibos";
            bmPagar = obmPagar;
            bmCheques = new BindingSource();
            bmChequesFilhos = new BindingSource();
            DataColumn[] colunasOrig = new DataColumn[(bmPagar.DataSource as DataTable).Columns.Count];
            (bmPagar.DataSource as DataTable).Columns.CopyTo(colunasOrig, 0);
            cheques = new DataTable();
            cheques.TableName = "CHEQUESPAGAR";
            List<string> colunasDesejadas = new List<string>(new string[]
               {"MOV_ID", "DATA","FORN","VALOR","DOC","DOC_FISC","HIST","CREDITO", "DEBITO" });
            foreach (DataColumn col in colunasOrig)
            {
                if (!colunasDesejadas.Contains(col.ColumnName.ToUpper())) continue;
                cheques.Columns.Add(col.ColumnName, col.DataType);
            }
            cheques.Columns.Add("CPF");
            cheques.Columns.Add("RG");
            cheques.Columns.Add("IDLOCAL", Type.GetType("System.Int32"));
            cheques.Columns.Add("NUMEROCHEQUE", Type.GetType("System.Int32"));


            chequesFilhos = cheques.Clone();
            int id = 0;

            foreach (DataRow orow in (bmPagar.DataSource as DataTable).AsEnumerable().
                Where(row => row.Field<string>("FORN").Trim() != ""))
            {
                DataRow rowcheque = null;
                try
                {
                    rowcheque = cheques.AsEnumerable().Where(row =>
                       (row.Field<string>("FORN").Trim() == orow.Field<string>("FORN").Trim()) &&
                       (row.Field<string>("CREDITO").Trim() == orow.Field<string>("CREDITO").Trim())
                       && (!row.IsNull("DATA") && (row.Field<DateTime>("DATA").CompareTo(orow.Field<DateTime>("DATA")) == 0))).FirstOrDefault();
                }
                catch (Exception)
                {
                }
                if (rowcheque == null)
                {
                    rowcheque = cheques.NewRow();
                    foreach (string campo in colunasDesejadas)
                    { rowcheque[campo] = orow[campo]; }
                    id++;
                    rowcheque["IDLOCAL"] = id;
                    rowcheque["NUMEROCHEQUE"] = 0;
                    if (orow["DOC"].ToString().Trim() != "")
                    {
                        rowcheque["NUMEROCHEQUE"] = RetorneNumCheque(orow["DOC"].ToString().Trim());
                    }
                    cheques.Rows.Add(rowcheque);
                    cheques.AcceptChanges();
                    DataRow rowchequefil = chequesFilhos.NewRow();
                    foreach (string campo in colunasDesejadas)
                    { rowchequefil[campo] = orow[campo]; }
                    rowchequefil["IDLOCAL"] = rowcheque["IDLOCAL"];

                    chequesFilhos.Rows.Add(rowchequefil);
                    chequesFilhos.AcceptChanges();

                }
                else
                {
                    rowcheque.BeginEdit();
                    rowcheque["VALOR"] = Convert.ToDouble(rowcheque["VALOR"]) + Convert.ToDouble(orow["VALOR"]);

                    if ((orow["DOC"].ToString().Trim() != "") && (Convert.ToInt32(rowcheque["NUMEROCHEQUE"]) == 0))
                    {
                        rowcheque["NUMEROCHEQUE"] = RetorneNumCheque(orow["DOC"].ToString().Trim());
                        rowcheque["DOC"] = orow["DOC"];
                    }




                    if (!rowcheque.IsNull("DOC_FISC") && (rowcheque["DOC_FISC"].ToString().Trim()
                             != orow["DOC_FISC"].ToString().Trim()))
                        rowcheque["DOC_FISC"] = rowcheque["DOC_FISC"].ToString().Trim() + "/" + orow["DOC_FISC"].ToString().Trim();
                    else rowcheque["DOC_FISC"] = orow["DOC_FISC"];
                    if (!rowcheque.IsNull("HIST") && (rowcheque["HIST"].ToString().Trim()
                             != orow["HIST"].ToString().Trim()))
                        rowcheque["HIST"] = rowcheque["HIST"].ToString().Trim() + "/" + orow["HIST"].ToString().Trim();
                    else rowcheque["HIST"] = orow["HIST"].ToString().Trim();
                    rowcheque.EndEdit();
                    rowcheque.AcceptChanges();
                    DataRow rowchequefil = chequesFilhos.NewRow();
                    foreach (string campo in colunasDesejadas)
                    { rowchequefil[campo] = orow[campo]; }
                    rowchequefil["IDLOCAL"] = rowcheque["IDLOCAL"];
                    chequesFilhos.Rows.Add(rowchequefil);
                    chequesFilhos.AcceptChanges();
                }
            }
            bmCheques.DataSource = cheques.AsDataView();
            bmChequesFilhos.DataSource = chequesFilhos.AsDataView();
            MonteGrid();
            dgvCheque.CellFormatting += DgvCheques_CellFormatting;

            oCheque.oDataGridView = dgvCheque;
            oCheque.oDataGridView.DataSource = bmCheques;
            oCheque.sbTotal = sbCheque;
            oCheque.ConfigureDBGridView();
            oCheque.FuncaoSoma();
            oCheque.ColocaTotais();
            oChequeFilhos.oDataGridView = dgvChequesFilho;
            oChequeFilhos.oDataGridView.DataSource = bmChequesFilhos;
            oChequeFilhos.ConfigureDBGridView();


            bmCheques.PositionChanged -= BmCheques_PositionChanged;
            bmCheques.PositionChanged += BmCheques_PositionChanged;

            if (bmCheques.Count != 0)
            {
                bmCheques.MoveFirst();
            }
            BmCheques_PositionChanged(bmCheques, null);


        }


        private int RetorneNumCheque(string tcampo)
        {
            int result = 0;
            string justNumbers = new String(tcampo.Where(Char.IsDigit).ToArray());
            if (justNumbers.Trim() == "") return result;
            try
            {
                result = Convert.ToInt32(justNumbers);
            }
            catch (Exception)
            {
            }
            return result;
        }
        private void DgvCheques_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("DOC"))
            {
                DataRowView registro = (((DataGridView)sender).Rows[e.RowIndex].DataBoundItem as DataRowView);
                if (Convert.ToInt32(registro["NUMEROCHEQUE"]) != 0)
                {
                    // ((DataGridView)sender).Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.Red;
                    ((DataGridView)sender).Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.LightGreen;
                    // if (((DataGridView)sender).Rows[e.RowIndex].Selected)
                    //   ((DataGridView)sender).Rows[e.RowIndex].Selected = false;
                }

            }

        }
        private void BmCheques_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);

            bool result = (registro != null);
            // dgvChequesFilho.Visible = false;
            if (result)
            {
                int? localid = Convert.ToInt32(registro["IDLOCAL"]);
                double? id = Convert.ToDouble(registro["MOV_ID"]);
                try
                {   // filhos

                    var dado = chequesFilhos.AsEnumerable().Where(row => (row.Field<int?>("IDLOCAL") == localid));

                    // && (row.Field<double?>("MOV_ID") != id));
                    // );  
                    //(row.Field<double?>("ID") != Convert.ToDouble(registro["ID"])));
                    if ((dado != null))
                    {
                        bmChequesFilhos.DataSource = dado.AsDataView();
                        //     dgvChequesFilho.Visible = true;


                    }
                    else { bmChequesFilhos.DataSource = chequesFilhos.AsEnumerable().Where(row => row.IsNull("IDLOCAL")).AsDataView(); }

                }
                catch (Exception)
                {
                    bmChequesFilhos.DataSource = chequesFilhos.AsEnumerable().Where(row => row.IsNull("IDLOCAL")).AsDataView();
                }

            }
            else
            {
                bmChequesFilhos.DataSource = chequesFilhos.AsEnumerable().Where(row => row.IsNull("IDLOCAL")).AsDataView();
            }
        }

        private void MonteGrid()
        {
            oCheque = new MonteGrid();
            oCheque.Clear();
            oCheque.AddValores("DATA", "Data", 9, "", false, 0, "");
            oCheque.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
            oCheque.AddValores("CREDITO", "Bco", 6, "", false, 0, "");
            oCheque.AddValores("FORN", "Titular", 30, "", false, 0, "");
            oCheque.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oCheque.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oCheque.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            // oCheque.AddValores("CPF", "CPF", 14, "", false, 0, "");
            // oCheque.AddValores("RG", "RG", 10, "", false, 0, "");


            oChequeFilhos = new MonteGrid();
            oChequeFilhos.Clear();

            oChequeFilhos.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oChequeFilhos.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oChequeFilhos.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oChequeFilhos.AddValores("DEBITO", "Lanc.Contabil", 25, "", false, 0, "");


        }

        /*private void btnCheque_Click(object sender, EventArgs e)
        {
            oprinter = new DataGridViewPrinter(oCheque.oDataGridView, printDocument1, true, true, "teste"
                , this.Font, this.BackColor, true);

            printDocument1.PrintPage += PrintDocument1_PrintPage;

            printDocument1.Print();

            // oprinter.DrawDataGridView();
        }

        private void PrintDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool result = oprinter.DrawDataGridView(e);
            e.HasMorePages = result;
           
        }
    */
        /// <summary>
        /// /////////////CHEQUES
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnimprime_Click(object sender, EventArgs e)
        {
            tipoPapelCheque = "cheque";
            primeiraimpressao = false;
            if (!primeiraimpressao)
                primeiraimpressao = SetupThePrinting();
            ConfigureRelatorio();

            if (primeiraimpressao)
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }



        // RELAçÃO CM PARA POLEGADA  1cm => 0.3937008 pol
        private bool SetupThePrinting()
        {
            printDialog1.AllowCurrentPage = false;
            printDialog1.AllowPrintToFile = false;
            printDialog1.AllowSelection = false;
            printDialog1.AllowSomePages = false;
            printDialog1.PrintToFile = false;
            printDialog1.ShowHelp = false;
            printDialog1.ShowNetwork = false;
            // if (printDialog1.PrinterSettings.PrinterName != "Microsoft Print to PDF")
            //    printDialog1.PrinterSettings.PrinterName = "Microsoft Print to PDF";

            printDialog1.PrinterSettings.DefaultPageSettings.Landscape = false;
            printDialog1.Document = null;
            if (printDialog1.ShowDialog() != DialogResult.OK)
                return false; //21cm * 0.3937008 

            printDocument1.DocumentName = "";
            PaperSize papEscolha = null;
            foreach (PaperSize pap in printDialog1.PrinterSettings.PaperSizes)
            {
                if (pap.PaperName.ToLower() == tipoPapelCheque.ToLower())
                {
                    papEscolha = pap;

                }
            }

            if (papEscolha != null)
                printDialog1.PrinterSettings.DefaultPageSettings.PaperSize = papEscolha;
            else
            {
                MessageBox.Show("É preciso Configurar a Impressora com tipo <" + tipoPapelCheque + ">");
                return false;
            }



            printDocument1.PrinterSettings =
                       printDialog1.PrinterSettings;
            printDocument1.DefaultPageSettings =
           printDialog1.PrinterSettings.DefaultPageSettings;
            printDocument1.DefaultPageSettings.Margins =
                             new System.Drawing.Printing.Margins(0, 0, 0, 0);

            printDocument1.DefaultPageSettings.Landscape = false;
            return true;
        }
        private bool ConfigureRelatorio()
        {
            ChequeBradesco cheques = ChequesImp.PreencheListaCheques(dgvCheque, bmCheques);

            printDocument1.PrintPage -= printDocument1_PrintPage_Folha;
            printDocument1.PrintPage -= printDocument1_PrintPage_1;
            printDocument1.PrintPage += printDocument1_PrintPage_1;

            oChequeImprime = new ChequeImprime(cheques, printDocument1,
                new Font("Arial", 12, FontStyle.Regular));

            return true;

        }
        // const int GETPRINTINGOFFSET = 13;
        private void printDocument1_PrintPage_1(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = oChequeImprime.DrawDataGridView(e);
            if (more == true)
                e.HasMorePages = true;
        }

        private void btnChequeUmaFolha_Click(object sender, EventArgs e)
        {
            tipoPapelCheque = "chequefolha";

            primeiraimpressao = false;
            if (!primeiraimpressao)
                primeiraimpressao = SetupThePrinting();
            ConfigureRelatorioChequeFolha();

            if (primeiraimpressao)
            {
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }
        private bool ConfigureRelatorioChequeFolha()
        {
            ChequeBradesco cheques = ChequesImp.PreencheListaCheques(dgvCheque, bmCheques);

            printDocument1.PrintPage -= printDocument1_PrintPage_1;
            printDocument1.PrintPage -= printDocument1_PrintPage_Folha;
            printDocument1.PrintPage += printDocument1_PrintPage_Folha;

            oChequeImprimeFolha = new ChequeImprimeFolha(cheques, printDocument1,
                new Font("Arial", 12, FontStyle.Regular));

            return true;

        }
        // const int GETPRINTINGOFFSET = 13;
        private void printDocument1_PrintPage_Folha(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            bool more = oChequeImprimeFolha.DrawDataGridView(e);
            if (more == true)
                e.HasMorePages = true;
        }




        /// <summary>
        /// //////////////////////RECIBO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btnRecibo_Click(object sender, EventArgs e)
        {
            primeiraimpressao = false;
            if (!primeiraimpressao)
                primeiraimpressao =
                    SetupThePrintingRecibos();
            ConfigureRelatorioRecibo();
            if (primeiraimpressao)
            {

                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument2;
                MyPrintPreviewDialog.ShowDialog();
            }


        }
        private bool ConfigureRelatorioRecibo()
        {
            int tipo = rbRecibos.Checked ? 1 : rbVauches.Checked ? 2 : 0;
            if (tipo == 0) { MessageBox.Show("Selecione Tipo"); return false; };
            Recibos orecibos = RecibosImp.MonteRecibos(dgvCheque, bmCheques, bmChequesFilhos, "Gandu(Ba)", tipo);
            orecibos.empresa = "M.Libanio Agricola S.A.";
            printDocument2.PrintPage -= PrintDocument2_PrintPage;
            printDocument2.PrintPage += PrintDocument2_PrintPage;

            oReciboImprime = new ReciboImprime(orecibos, printDocument2,
                new Font("Arial", 12, FontStyle.Regular));

            return true;

        }

        private void PrintDocument2_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool more = oReciboImprime.DrawDataGridView(e);
            if (more == true)
                e.HasMorePages = true;
        }

        private bool SetupThePrintingRecibos()
        {
            printDialog2.AllowCurrentPage = false;
            printDialog2.AllowPrintToFile = false;
            printDialog2.AllowSelection = false;
            printDialog2.AllowSomePages = false;
            printDialog2.PrintToFile = false;
            printDialog2.ShowHelp = false;
            printDialog2.ShowNetwork = false;
            // if (printDialog2.PrinterSettings.PrinterName != "Microsoft Print to PDF")
            //    printDialog2.PrinterSettings.PrinterName = "Microsoft Print to PDF";

            printDialog2.PrinterSettings.DefaultPageSettings.Landscape = false;
            printDialog2.Document = null;
            if (printDialog2.ShowDialog() != DialogResult.OK)
                return false; //21cm * 0.3937008 

            printDocument2.DocumentName = "";
            printDocument2.PrinterSettings =
                       printDialog2.PrinterSettings;
            printDocument2.DefaultPageSettings =
           printDialog2.PrinterSettings.DefaultPageSettings;
            printDocument2.DefaultPageSettings.Margins =
                             new System.Drawing.Printing.Margins(20, 20, 20, 20);
            printDocument2.DefaultPageSettings.Landscape = false;

            return true;

        }

        /// <summary>
        /// //////////  EXPERIMENTAÇÃO
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>


        private void DisplayFonts_Click1(object sender, System.EventArgs e)
        {
            /* //Create InstalledFontCollection objects  
             InstalledFontCollection ifc =
              new InstalledFontCollection();
             //Get font families  
             FontFamily[] ffs = ifc.Families;
             Font f;
             //Make sure rich text box is empty  
             richTextBox1.Clear();
             //Read font families one by one,  
             //set font to some text,  
             //and add text to the text box  
             foreach (FontFamily ff in ffs)
             {
                 if (ff.IsStyleAvailable(FontStyle.Regular))
                     f = new Font(ff.GetName(1), 12, FontStyle.Regular);
                 else if (ff.IsStyleAvailable(FontStyle.Bold))
                     f = new Font(ff.GetName(1), 12, FontStyle.Bold);
                 else if (ff.IsStyleAvailable(FontStyle.Italic))
                     f = new Font(ff.GetName(1), 12, FontStyle.Italic);
                 else
                     f = new Font(ff.GetName(1), 12, FontStyle.Underline);
                 richTextBox1.SelectionFont = f;
                 richTextBox1.AppendText(ff.GetName(1) + "\r\n");
                 richTextBox1.SelectionFont = f;
                 richTextBox1.AppendText("abcdefghijklmnopqrstuvwxyz\r\n");
                 richTextBox1.SelectionFont = f;
                 richTextBox1.AppendText("ABCDEFGHIJKLMNOPQRSTUVWXYZ\r\n");
                 richTextBox1.AppendText("==========================\r\n");
             }*/
        }
        private void PrintPreviewMenuClick(object sender, System.EventArgs e)
        {
            //Create a PrintPreviewDialog object  
            previewDlg = new PrintPreviewDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();
            //Add print-page event handler  
            pd.PrintPage +=
            new PrintPageEventHandler(pd_PrintPage);
            //Set Document property of PrintPreviewDialog  
            previewDlg.Document = pd;
            //Display dialog  
            previewDlg.Show();
        }

        private void PrintMenuClick(object sender, System.EventArgs e)
        {
            //Create a PrintPreviewDialog object  
            previewDlg = new PrintPreviewDialog();
            //Create a PrintDocument object  
            PrintDocument pd = new PrintDocument();
            //Add print-page event handler  
            pd.PrintPage +=
            new PrintPageEventHandler(pd_PrintPage);
            //Print  
            pd.Print();
        }
        public void pd_PrintPage(object sender, PrintPageEventArgs ev)
        {
            ypos = 1;
            float pageheight = ev.MarginBounds.Height;
            //Create a Graphics object  
            Graphics g = ev.Graphics;
            //Get installed fonts  
            InstalledFontCollection ifc =
            new InstalledFontCollection();
            //Get font families  
            FontFamily[] ffs = ifc.Families;
            //Draw string on the paper  
            while (ypos + 60 < pageheight &&
            fontposition < ffs.GetLength(0))
            {
                //Get the font name   
                Font f =
                new Font(ffs[fontposition].GetName(0), 25);
                //Draw string  
                g.DrawString(ffs[fontposition].GetName(0), f,
                new SolidBrush(Color.Black), 1, ypos);
                fontposition = fontposition + 1;
                ypos = ypos + 60;
            }
            if (fontposition < ffs.GetLength(0))
            {
                //Has more pages??  
                ev.HasMorePages = true;
            }
        }

        private void textBox1_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (textBox1.Text == "") return;
            Int64 tx2 = 0;
            try
            {
                tx2 = Convert.ToInt64(textBox2.Text);
            }
            catch (Exception)
            {
            }
            Int64 tx1 = 0;
            try
            {
                tx1 = Convert.ToInt64((sender as System.Windows.Forms.TextBox).Text);
            }
            catch (Exception)
            {
            }

            if ((tx2 != 0) && (tx2 < tx1))
            {
                if (MessageBox.Show("Inicio da Serie Maior que Limite!! Altera Limite?", "Alerta",
                    MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    textBox2.Text = (sender as System.Windows.Forms.TextBox).Text;
                }
                e.Cancel = true;
            }
        }

        private void textBox2_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {

            
            if (textBox2.Text == "") return;
            Int64 tx1 = 0;
            try
            {
                tx1 = Convert.ToInt64(textBox1.Text);
            }
            catch (Exception)
            {
            }
            Int64 tx2 = 0;
            try
            {
                tx2 = Convert.ToInt64((sender as System.Windows.Forms.TextBox).Text);
            }
            catch (Exception)
            {
            }



            if (tx1 == 0)
            {
                MessageBox.Show("Falta Inicio da Serie !!");
                e.Cancel = true;
                return;
            }
            if ((tx1 != 0) && (tx1 > tx2))
            {
                MessageBox.Show("Limite menor que Inicio da Serie !!");
                e.Cancel = true;
            }
         
        }
        private async void btnGrave_Click(object sender, EventArgs e)
        {
            //   if (textBox2.Text == "") return;
            Int64 tx1 = 0;
            try
            {
                tx1 = Convert.ToInt64(textBox1.Text);
            }
            catch (Exception)
            {
            }

            Int64 tx2 = 0;
            if (textBox2.Text != "")
            {
                try
                {
                    tx2 = Convert.ToInt64(textBox2.Text);
                }
                catch (Exception)
                {
                }
            }
            int tamnumeroserie = textBox1.Text.Trim().Length;
          

            if (tx1 == 0)
            {
                MessageBox.Show("Verifique numeros Inicio dos Cheques");
                return;
            }
            if ((tx2 != 0) && (tx1 > tx2))
            {
                MessageBox.Show("Verifique numeros Inicio e Limite dos Cheques");
                return;
            }
            if (dgvCheque.SelectedRows.Count == 0)
            {
                MessageBox.Show("Necessário Selecionar Linhas");
                return;
            }

            bool necessita_subscreve = false;
            bool banco = true;
            List<double> mov_ids = new List<double>();
            foreach (DataGridViewRow dgvrow in dgvCheque.Rows)
            {

                if (!(dgvrow.Selected)) continue; 
                DataRowView orow = (dgvrow.DataBoundItem as DataRowView);
                if (Convert.ToInt32(orow["NUMEROCHEQUE"]) != 0)
                {
                    necessita_subscreve = true;

                }
                if (TabelasIniciais.NomeBanco(orow["CREDITO"].ToString()) != "BRADESCO")
                {
               
                        banco = false;

                }
                mov_ids.Add(Convert.ToDouble(orow["MOV_ID"]));
            }
            if (!banco)
            {
                if (MessageBox.Show("Existem Lançamentos de Banco diferente de <02>!! Continua assim mesmo?", "Alerta",
                      MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            if (necessita_subscreve)
            {
                if (MessageBox.Show("Entre as linhas selecionadas existem cheques já numerados!! Subscreve?", "Alerta",
                    MessageBoxButtons.YesNo) == DialogResult.No)
                {
                    return;
                }
            }
            Int64 inicioSerie = tx1;
            List<string> updates = new List<string>();
           // bmPagar.DataSource as DataTable
            foreach (DataGridViewRow dgvrow in dgvCheque.Rows)
            {
                if (!(dgvrow.Selected)) continue;
                DataRowView orow = (dgvrow.DataBoundItem as DataRowView);

                int tamnumero = inicioSerie.ToString().Trim().Length;

                string doc = "";
                if (tamnumero < tamnumeroserie)
                    doc = "CH" + inicioSerie.ToString().Trim().PadLeft(tamnumeroserie, Convert.ToChar("0"));
                else
                   doc = "CH" + inicioSerie.ToString().Trim();
                
                
                orow.Row.BeginEdit();
                orow.Row["DOC"] = doc;
                orow.Row.EndEdit();
                orow.Row.AcceptChanges();
                string condicao = "";
                mov_ids.Clear();
               // List<string> registroUpdate = new List<string>();
                foreach (DataRowView orowdet in (bmChequesFilhos.DataSource as DataView).Table.AsEnumerable().
                   Where(row => row.Field<int>("IDLOCAL") == Convert.ToInt32(orow["IDLOCAL"])).AsDataView())
                {
                    orowdet.Row.BeginEdit();
                    orowdet.Row["DOC"] = doc;
                    orowdet.Row.EndEdit();
                    orowdet.Row.AcceptChanges();
                    
                    DataRow orowPagar =
                        (bmPagar.DataSource as DataTable).AsEnumerable().Where(row => row.Field<double>("MOV_ID") ==
                        Convert.ToDouble(orowdet["MOV_ID"])).FirstOrDefault();

                    if (orowPagar != null)
                    {
                        orowPagar.BeginEdit();
                        orowPagar["DOC"] = doc;
                        orowPagar.EndEdit();
                        orowPagar.AcceptChanges();
                    }
                    mov_ids.Add(Convert.ToDouble(orow["MOV_ID"]));
                }
                if (mov_ids.Count > 0)
                {
                    condicao = " WHERE MOV_ID IN ( ";
                    string virgula = "";
                    string incluir = "";
                    foreach (float mov_id in mov_ids)
                    {
                        incluir = incluir + virgula + Convert.ToInt64(mov_id).ToString();
                        virgula = ", ";
                    }
                    condicao = condicao + incluir + " ); ";
                    string command = "update MOV_FIN SET DOC = '" +doc.Trim() + "'" + condicao;
                    updates.Add(command);
                }
                inicioSerie++;
                if (tx2 != 0)
                {
                    if (tx2 == inicioSerie)
                        break;
                }
            }
            string strquery = "";
            foreach(string campo in updates)
            {
                strquery = strquery + campo;
            }
            int result =    await ApiServices.PostApi(strquery, 0);

            //  '' WHERE MOV_ID  IN (123, 1234, 12233, 12212)

        }
            private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(Char.IsDigit(e.KeyChar)))
            {
                if (!(e.KeyChar == '\b'))
                    e.Handled = true; 
            }
        }
    }
}
