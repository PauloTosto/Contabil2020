using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassLibTrabalho;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    public partial class FrmImprimeFolha : Form
    {
        public TAtualizaFolha oatualfolha;// recebe 
        private MonteGrid oFolha;
        public BindingSource bmSourceEntrada;
        public DataView dvEntradas;
        DataGridViewPrinter_Folha oDataGridViewPrinter;
        DataSet dsResult;
        string tabelaatual = "CLTFOLHATOT";

        private bool primeiraimpressao;

        public FrmImprimeFolha()
        {
            InitializeComponent();
            //recomece = false;
            primeiraimpressao = false;
            oFolha = new MonteGrid();
            oFolha.tituloExcel = "Relatorio Trabalhadores Pagamentos Realizados Em " + datapg.Value.ToString("d");
            dgEntradas.ReadOnly = true;

            oFolha.oDataGridView = dgEntradas;
            oFolha.oDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True; // o text não será manipulado no cabeçalho

            foreach (DataGridViewColumn ocol in oFolha.oDataGridView.Columns)
            {
                ocol.SortMode = DataGridViewColumnSortMode.NotSortable;

            }
            oFolha.oDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True; // o text não será manipulado no cabeçalho
            oFolha.oDataGridView.AllowUserToAddRows = false;



            oFolha.sbTotal = sbEntradas;
            oFolha.sbTotalExtra = sbTotalExtra;
            oFolha.Totalizando += new EventHandler<TotalizaEventArgs>(oFolha_Totalizando);

           // oForm = new FormFiltro();
           // oForm.dtData2.Visible = false;
           // oForm.dtData2.Enabled = false;
            bmSourceEntrada = new BindingSource();
            if (oatualfolha == null)
               oatualfolha = new TAtualizaFolha();
        }

        void oFolha_Totalizando(object sender, TotalizaEventArgs e)
        {
            MonteGrid ofolha = (MonteGrid)sender;
            ofolha.sbTotalExtra.Width = 0;
            ofolha.sbTotalExtra.Panels.Clear();
            ofolha.sbTotalExtra.ShowPanels = true;
            ofolha.sbTotalExtra.Panels.Add("Numero Trabalhadores:");
            ofolha.sbTotalExtra.Panels[0].AutoSize = StatusBarPanelAutoSize.Contents;

            ofolha.sbTotalExtra.Panels.Add("");
            //ofolha.sbTotalExtra.Panels[1].BorderStyle = StatusBarPanelBorderStyle.Raised;
            ofolha.sbTotalExtra.Panels[1].Style = StatusBarPanelStyle.Text;
            ofolha.sbTotalExtra.Panels[1].Alignment = HorizontalAlignment.Center;
            System.Nullable<int> total1 =
          (from valor1 in e.Tabpesq.AsEnumerable()
           select valor1).Count();
            total1 = total1 / 2;
            ofolha.sbTotalExtra.Panels[1].Text = total1.ToString();
            //  LinhasCampo[i].total = Convert.ToDouble(total1);

            ofolha.sbTotalExtra.Panels[1].AutoSize = StatusBarPanelAutoSize.Contents;
            ofolha.sbTotalExtra.Width = ofolha.sbTotalExtra.Panels[0].Width + ofolha.sbTotalExtra.Panels[1].Width;
            ofolha.sbTotalExtra.SizingGrip = false;
        }
        private void FrmImprimeFolha_Load(object sender, EventArgs e)
        {
            // if (recomece)
            // {
            /*    oForm.oArme = ArmeEdicaoFiltroGenerico();
                oForm.ShowDialog();
                if (oForm.oPesqFin.TabCount > 0)
                    oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
                else
                    oList = null;
                datapg.Value = oForm.dtData1.Value;
                inicio = TDataControlReduzido.PrimeiroDiaMes(oForm.dtData1.Value);
                fim = TDataControlReduzido.UltimoDiaMes(oForm.dtData1.Value);*/
            if (oatualfolha == null)
            {
                try
                {
                    oatualfolha = new TAtualizaFolha();
                    /*if (rbfolhamensal.Checked)
                        oatualfolha.tipo = "";
                    else
                        oatualfolha.tipo = "A";*/
                    oatualfolha.datadigitada = datapg.Value.Date;
                    oatualfolha.Gera_Arquivo(datapg.Value.Date);
                    dsResult = new DataSet();
                    if (oatualfolha.tipo == "A")
                        dsResult.Tables.Add(oatualfolha.CltAdiant);
                    else
                        dsResult.Tables.Add(oatualfolha.CltFolhaTotal);
                }
                catch (Exception)
                {
                    throw;
                }
            }
            else
            {
                datapg.Value = oatualfolha.fim;
                dsResult = new DataSet();
                if (oatualfolha.tipo == "A")
                    //oatualfolha.GraveCLTADIANT();
                    dsResult.Tables.Add(oatualfolha.CltAdiant);
                else
                    dsResult.Tables.Add(oatualfolha.Clt_RelFolhaTotal());
            
            }

            DataTable dtsetores = TDataDivs.Get_Setores();
            
            var setores =
                  from linha in dsResult.Tables[tabelaatual].AsEnumerable()
                  group linha by linha.Field<string>("SetorCad") into g
                  select new
                  {
                      codsetor = g.Key
                  };
            //ache os descritivos dos setores
            var setores_desc =
                from setor in setores
                 join linha in dtsetores.AsEnumerable() on setor.codsetor equals linha.Field<string>("cod")
                select new { codsetor = setor.codsetor, descricao = linha.Field<string>("DESC") }; //produces flat sequence

            cbSetores.Items.Clear();
            //cbSetores.Items.Add("  ");
            foreach (var linha in setores_desc)
            {
                cbSetores.Items.Add(linha.codsetor + " "+linha.descricao);
            }
            cbSetores.Sorted = true;
            this.cbSetores.SelectedIndexChanged += new System.EventHandler(this.cbSetores_SelectedIndexChanged);

            cbSetores.SelectedIndex = 0;   
            this.Refresh();
        }
   
        private void MonteGrids()
        {
            oFolha.Clear();
            //ofolha.tipo = "A"

            oFolha.AddValores("NOME", "NOME", 40, "", false, 0, "");
            oFolha.AddValores("DIA1", " 1", 2, "", false, 0, "");
            oFolha.AddValores("DIA2", " 2", 2, "", false, 0, "");
            oFolha.AddValores("DIA3", " 3", 2, "", false, 0, "");
            oFolha.AddValores("DIA4", " 4", 2, "", false, 0, "");
            oFolha.AddValores("DIA5", " 5", 2, "", false, 0, "");
            oFolha.AddValores("DIA6", " 6", 2, "", false, 0, "");
            oFolha.AddValores("DIA7", " 7", 2, "", false, 0, "");
            oFolha.AddValores("DIA8", " 8", 2, "", false, 0, "");
            oFolha.AddValores("DIA9", " 9", 2, "", false, 0, "");
            oFolha.AddValores("DIA10", "10", 2, "", false, 0, "");
            oFolha.AddValores("DIA11", "11", 2, "", false, 0, "");
            oFolha.AddValores("DIA12", "12", 2, "", false, 0, "");
            oFolha.AddValores("DIA13", "13", 2, "", false, 0, "");
            oFolha.AddValores("DIA14", "14", 2, "", false, 0, "");
            oFolha.AddValores("DIA15", "15", 2, "", false, 0, "");
            oFolha.AddValores("DIA16", "16", 2, "", false, 0, "");


            if (oatualfolha.tipo == "A")
                oFolha.AddValores("ADIANT", "ADIANT", 12, "#,###,##0.00", true, 0, "");
            else
            {
                oFolha.AddValores("NDIAS", "Dias", 5, "#0.0", false, 0, "");
                oFolha.AddValores("SALARIO", "Salario", 10, "###,##0.00", true, 0, "");
                oFolha.AddValores("HE50", "H.Ext.", 8, "##,##0.00", true, 0, "");
                oFolha.AddValores("SBRUTO", "Sal.Bruto", 10, "###,##0.00", true, 0, "");
                oFolha.AddValores("SALFAM", "Sal.Fam.", 8, "##,##0.00", true, 0, "");
                oFolha.AddValores("INSS", "INSS", 8, "##,##0.00", true, 0, "");
                //oFolha.AddValores("IRFONTE", "Ir.Fonte", 8, "#,###,##0.00", true, 0, "");
                oFolha.AddValores("ADIANT", "Adiantamento", 10, "###,##0.00", true, 0, "");

                oFolha.AddValores("SALLIQ", "Sal.Liquido", 10, "###,##0.00", true, 0, "");
                oFolha.AddValores("ASSINATURA", "ASSINATURAS", 62, "", false, 0, "");
            }
        }

        private void Reconstrua()
        {
            //PercorraCondicoes();
            // string lit = pegfiltro();
            //  if (lit == "")
            // {
            dvEntradas = dsResult.Tables[0].AsDataView();
            //}
            /* else
             {
                 DataTable tabgeral = dsResult.Tables[tabelaatual].Clone();
                 tabgeral.TableName = "tabgeral";

                 dsResult.Tables[tabelaatual].Select(lit).CopyToDataTable(tabgeral, LoadOption.Upsert);
                 dvEntradas = tabgeral.AsDataView();
             }*/
            string tsetor = "";
            if (cbSetores.SelectedIndex > -1)
                tsetor = " AND  (SETORCAD = '" + cbSetores.Text.Substring(0,2) + "') ";
            string lit = "(MENSALISTA = '')" + tsetor;
            string tsort = "ORDEM";  // setor//nome
            DataTable tabgeral = dsResult.Tables[tabelaatual].Clone();
            tabgeral.TableName = "tabgeral";

            dsResult.Tables[tabelaatual].Select(lit,tsort).CopyToDataTable(tabgeral, LoadOption.Upsert);
            dvEntradas = tabgeral.AsDataView();

            MonteGrids();
            bmSourceEntrada.DataSource = dvEntradas;
            dgEntradas.DataSource = bmSourceEntrada;

            oFolha.ConfigureDBGridView();
            foreach (DataGridViewColumn ocol in oFolha.oDataGridView.Columns)
            {
                ocol.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (ocol.DataPropertyName.Substring(0, 3) == "DIA")
                {
                    ocol.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 5, FontStyle.Regular);
                    //ocol.Width = (2 * Convert.ToInt16(ocol.DefaultCellStyle.Font.Size)); 
                }
                else
                    ocol.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 6, FontStyle.Regular);
               
            }
            oFolha.oDataGridView.ColumnHeadersDefaultCellStyle.Font =
                  new Font("Microsoft Sans Serif", 5, FontStyle.Regular);
            oFolha.oDataGridView.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.True; // o text não será manipulado no cabeçalho
            // oFolha.oDataGridView.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // oFolha.oDataGridView.DefaultCellStyle.Font = new Font("Microsoft Sans Serif", 7, FontStyle.Regular);
            oFolha.oDataGridView.AllowUserToAddRows = false;
            oFolha.EncontraTotaisView();
            oFolha.ColocaTotais();
            this.Refresh();

        }
       /* private void button3_Click(object sender, EventArgs e)
        {
            if (oatualfolha != null)
            {
                if (oatualfolha.tipo == "A")
                    oatualfolha.GraveCLTADIANT();
                else
                    oatualfolha.GraveCLTFOLHA();
            }
        }*/

        private void btnimprime_Click(object sender, EventArgs e)
        {
            if (!primeiraimpressao)
                primeiraimpressao = SetupThePrinting();
            if (primeiraimpressao)
            {
                ConfigureRelatorio();
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument1;
                MyPrintPreviewDialog.ShowDialog();
            }
        }

        private bool SetupThePrinting()
        {
            printDialog1.AllowCurrentPage = false;
            printDialog1.AllowPrintToFile = false;
            printDialog1.AllowSelection = false;
            printDialog1.AllowSomePages = false;
            printDialog1.PrintToFile = false;
            printDialog1.ShowHelp = false;
            printDialog1.ShowNetwork = false;
            printDialog1.PrinterSettings.DefaultPageSettings.Landscape = true;

            if (printDialog1.ShowDialog() != DialogResult.OK)
                return false; //21cm * 0.3937008 

            printDocument1.DocumentName = "Relatorio Folha";
            //  printDocument1.DefaultPageSettings.PaperSize = new System.Drawing.Printing.PaperSize();
            // printDocument1.DefaultPageSettings.PaperSize.PaperName = "A4";
            // printDocument1.DefaultPageSettings.PaperSize.Width = 830;
            // printDocument1.DefaultPageSettings.PaperSize.Height = 1169;

            printDocument1.PrinterSettings =
                        printDialog1.PrinterSettings;
            printDocument1.DefaultPageSettings =
           printDialog1.PrinterSettings.DefaultPageSettings;
            printDocument1.DefaultPageSettings.Margins =
                             new System.Drawing.Printing.Margins(2, 2, 2, 2);
            printDocument1.DefaultPageSettings.Landscape = true;
            return true;
          
        }
        private bool ConfigureRelatorio()
        {
            oDataGridViewPrinter = new DataGridViewPrinter_Folha(oFolha.oDataGridView, printDocument1, false, true,
                "FOLHA DE PAGAMENTO de " + oatualfolha.inicio.ToString("d") + " a " + oatualfolha.fim.ToString("d"), new Font("Microsoft Sans Serif", 9, FontStyle.Regular), Color.Black, true, "M.Libanio Agricola S.A.", "Fazenda");
            //  calculo tamanho =  (x cm * 0.3937008)*100 
            oDataGridViewPrinter.RowsHeight.Clear();

            oDataGridViewPrinter.RowsHeight.Add(19.685F);
            oDataGridViewPrinter.ColumnsWidth.Clear();
            oDataGridViewPrinter.ColumnsWidth.Add(188.976F); // nome
            for (int i = 1; i < 17; i++)
            {
                oDataGridViewPrinter.ColumnsWidth.Add(11.811F); // pontos
            }
            oDataGridViewPrinter.ColumnsWidth.Add(22.559F);// dias
            oDataGridViewPrinter.ColumnsWidth.Add(55.118F);  // salario
            oDataGridViewPrinter.ColumnsWidth.Add(44.488F); // hextra
            oDataGridViewPrinter.ColumnsWidth.Add(55.118F);  // salbruto
            oDataGridViewPrinter.ColumnsWidth.Add(44.488F); // salfam
            oDataGridViewPrinter.ColumnsWidth.Add(44.488F); // inss
            oDataGridViewPrinter.ColumnsWidth.Add(55.118F);  // adiant
            oDataGridViewPrinter.ColumnsWidth.Add(55.118F);  // salliquido
            oDataGridViewPrinter.ColumnsWidth.Add(350.39F);  // assinaturas

            oDataGridViewPrinter.LinhaCab1.Add("");
            int j;
            for (j = 1; j < 16; j++)
            {
                oDataGridViewPrinter.LinhaCab1.Add(j.ToString());
                // oDataGridViewPrinter.ColumnsWidth.Add(11.811F); // pontos
            }
            oDataGridViewPrinter.LinhaCab1.Add("");
            oDataGridViewPrinter.LinhaCab1.Add("");//dias
            oDataGridViewPrinter.LinhaCab1.Add("Total");//salario
            oDataGridViewPrinter.LinhaCab1.Add("Vlr.H.");
            oDataGridViewPrinter.LinhaCab1.Add("Salario");//bruto
            oDataGridViewPrinter.LinhaCab1.Add("Salario");//familia
            oDataGridViewPrinter.LinhaCab1.Add("");//inss
            oDataGridViewPrinter.LinhaCab1.Add("Adianta");//mento
            oDataGridViewPrinter.LinhaCab1.Add("Salario");//liquido
            oDataGridViewPrinter.LinhaCab1.Add(""); // assinaturas

            oDataGridViewPrinter.LinhaCab2.Add("N. Nome Trabalhador");

            for (j = 16; j < (oatualfolha.fim.Day + 1); j++)
            {
                oDataGridViewPrinter.LinhaCab2.Add(j.ToString());
                // oDataGridViewPrinter.ColumnsWidth.Add(11.811F); // pontos
            }
            while (j < 31)
            {
                oDataGridViewPrinter.LinhaCab2.Add("");
                j += 1;
            }
            oDataGridViewPrinter.LinhaCab2.Add("Dias");//dias
            oDataGridViewPrinter.LinhaCab2.Add("Salario");//salario
            oDataGridViewPrinter.LinhaCab2.Add("Extras");
            oDataGridViewPrinter.LinhaCab2.Add("Bruto");//bruto
            oDataGridViewPrinter.LinhaCab2.Add("Familia");//familia
            oDataGridViewPrinter.LinhaCab2.Add("INSS");//inss
            oDataGridViewPrinter.LinhaCab2.Add("mento");//mento
            oDataGridViewPrinter.LinhaCab2.Add("Liquido");//liquido
            oDataGridViewPrinter.LinhaCab2.Add("ASSINATURAS"); // assinaturas
            for (int z = 0;z < oFolha.LinhasCampo.Count - 1;z++)
                if (oFolha.LinhasCampo[z].total != 0)
                    oDataGridViewPrinter.LinhaTotal.Add(z, Convert.ToSingle(oFolha.LinhasCampo[z].total));
        
            return true;

        }

        private void printDocument1_PrintPage_1(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // The PrintPage action for the PrintDocument control

            bool more = oDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;

        }

        private void printDocument1_BeginPrint(object sender, System.Drawing.Printing.PrintEventArgs e)
        {

        }
        private void PercorraCondicoes()
        {
            /* setor = "";
             if (cbSetores.SelectedIndex > 0)
                 setor = " (SETORCAD = '" + cbSetores.Text + "') ";
             banco = "";
             if (rbbanco_sim.Checked)
                 banco = " (CONTABCO <> '') ";
             else
                 if (rbbanco_nao.Checked)
                     banco = " (CONTABCO = '') ";
             mensal = "";
             if (rbcat_mensal.Checked)
                 mensal = " (MENSALISTA = 'X') ";
             else
                 if (rbcat_diarista.Checked)
                     mensal = " ((MENSALISTA <> 'X')) ";
             */
        }

        private void cbSetores_SelectedIndexChanged(object sender, EventArgs e)
        {
            Reconstrua();
        }

       
       
       /* private string pegfiltro()
        {
            string literal = "";
            if (mensal != "")
                literal = mensal;
            if (banco != "")
            {
                if (literal != "")
                    literal += " AND ";
                literal += banco;
            }
            if (setor != "")
            {
                if (literal != "")
                    literal += " AND ";
                literal += setor;
            }
            return literal;
        }*/

    }
}
