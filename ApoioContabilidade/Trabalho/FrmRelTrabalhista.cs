using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.Imprimir;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    public partial class FrmRelTrabalhista : Form
    {
        DataTable folhaServidor;
        public DataTable folhaVirtual;
        DateTime inicio;
        DateTime fim;
        MonteGrid oFolha;
        MonteGrid oBanco;
        bool primeiraimpressao;
        BindingSource bmFolha;
        FolhaImp oFolhaImprime;
        string txNomeTrab = "";
        string txSetor = "";
        string txTrab = "";
        List<string> dadosSetor = new List<string>();
        public FrmRelTrabalhista()
        {
            InitializeComponent();
            btnConsulta.Enabled = false;
            TabelasIniciaisConfigura();
           // string[] recursos = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();
          
        }
        private async void TabelasIniciaisConfigura()
        {
            if (!TabelasIniciais.TabelasIniciaisOk())
            {
                try
                {
                    btnConsulta.Enabled = await TabelasIniciais.Execute();
                    // while (!TabelasIniciais.TabelasIniciaisOk())
                    // { }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            else
            {
                btnConsulta.Enabled = true;

            }
          
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            pnFiltro.Enabled = false;
            DataSet dsResult = null;
            string str = "";
            Int32 naoGrave = 1;
            inicio = new DateTime(dtData1.Value.Year, dtData1.Value.Month, 1);
            fim = inicio.AddMonths(1).AddDays(-1);

            List<string> lstquery = new List<string>();
            lstquery.Add("inicio=" + inicio.ToString("yyyy-MM-dd"));
            lstquery.Add("fim=" + fim.ToString("yyyy-MM-dd"));
            lstquery.Add("naograve=" + naoGrave.ToString());
            lstquery.Add("sp_numero=" + "222");

            lstquery.Add(str);
            try
            {
                dsResult = await ApiServices.Api_QuerySP(lstquery);
            }
            catch (Exception)
            {
            }
            if (dsResult != null)
            {

                folhaServidor = dsResult.Tables[0].Copy();
                folhaServidor.TableName = "folhaServidor";

            }
            else
            {
                MessageBox.Show("Sem Resultados");
                return;
            }

            folhaVirtual = folhaServidor.Clone();
            if (!folhaVirtual.Columns.Contains("SALARIOATUAL"))
                folhaVirtual.Columns.Add("SALARIOATUAL", Type.GetType("System.Double"));
            if (!folhaVirtual.Columns.Contains("DIAS"))
                folhaVirtual.Columns.Add("DIAS", Type.GetType("System.Double"));
            if (!folhaVirtual.Columns.Contains("HEXT"))
                folhaVirtual.Columns.Add("HEXT", Type.GetType("System.Double"));
            if (!folhaVirtual.Columns.Contains("SALBRUTO"))
                folhaVirtual.Columns.Add("SALBRUTO", Type.GetType("System.Double"));


            foreach (DataRow rowServ in folhaServidor.Rows)
            {

                DataRow rowInc = folhaVirtual.NewRow();
                foreach (DataColumn ocol in folhaServidor.Columns)
                {
                    if (rowInc.Table.Columns.Contains(ocol.ColumnName))
                        rowInc[ocol.ColumnName] = rowServ[ocol.ColumnName];
                }
                rowInc["BASEIRF"] = Convert.ToDouble(rowServ["SALARIO"])
                    + Convert.ToDouble(rowServ["VLR_HXA"])
                    + Convert.ToDouble(rowServ["GRATIF"])
                    + Convert.ToDouble(rowServ["VLR_HXN"])
                    + Convert.ToDouble(rowServ["VLR_HXS"])
                    - Convert.ToDouble(rowServ["INSS"]);
                rowInc["BASEIRF"] = Convert.ToDouble(rowServ["BASEIRF"]) -
                     Convert.ToDouble(rowServ["IRFONTE"]);
                if (Convert.ToDouble(rowInc["BASEIRF"]) < 0)
                {
                    rowInc["BASEIRF"] = 0;
                }
                rowInc["DATA"] = dtCheque.Value;
                rowInc["SALLIQ"] = Convert.ToDouble(rowServ["SALARIO"])
                   + Convert.ToDouble(rowServ["GRATIF"])
                   + Convert.ToDouble(rowServ["VLR_HXA"])
                   + Convert.ToDouble(rowServ["VLR_HXN"])
                   + Convert.ToDouble(rowServ["VLR_HXS"])
                   + Convert.ToDouble(rowServ["SALFAM"])
                    - Convert.ToDouble(rowServ["INSS"])
                      - Convert.ToDouble(rowServ["IRFONTE"]);
                if (rbImpostoSind.Checked)
                {
                    double salbase = Convert.ToDouble(rowServ["SALARIOATUAL"]);
                    rowInc["ISIND"] = Math.Round(salbase / 30, 2);
                    if (Convert.ToDouble(rowServ["SALLIQ"]) < Convert.ToDouble(rowServ["ISIND"]))
                    {
                        rowInc["ISIND"] = 0;
                    }
                }

                rowInc["DIAS"] = Math.Round(Convert.ToDouble(rowServ["HX"]) / 8, 2);
                rowInc["HEXT"] = Convert.ToDouble(rowServ["VLR_HXA"])
                   + Convert.ToDouble(rowServ["VLR_HXN"])
                   + Convert.ToDouble(rowServ["VLR_HXS"]);
                rowInc["GRATIF"] = Convert.ToDouble(rowServ["GRATIF"]);
                rowInc["SALBRUTO"] = Convert.ToDouble(rowInc["SALARIO"]) + Convert.ToDouble(rowInc["HEXT"]) + Convert.ToDouble(rowInc["GRATIF"]);
                folhaVirtual.Rows.Add(rowInc);
                rowInc.AcceptChanges();
            }
            folhaVirtual.AcceptChanges();

            MonteGrids();
            oFolha.oDataGridView = dgvFolha;
            oBanco.oDataGridView = dgvBanco;
            FiltreDados();
            oFolha.ConfigureDBGridView();
            oBanco.ConfigureDBGridView();
            Font oFonte = (Font)oFolha.oDataGridView.DefaultCellStyle.Font.Clone();
            oFolha.oDataGridView.Columns["NOME"].DefaultCellStyle.Font = new Font(oFonte.FontFamily, (float)6.2);
            oFolha.oDataGridView.Columns["PONTO"].DefaultCellStyle.Font = new Font(oFonte.FontFamily, (float)6.0);
            oFolha.FuncaoSoma();
  
            ColocaTotais();
            pnFiltro.Enabled = true;
            //  lbTotais.Top = tcFolha.Top + tcFolha.Height + 10;
        }
        private void MonteGrids()
        {

            oFolha = new MonteGrid();
            oFolha.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            oFolha.AddValores("COD", "Código", 6, "", false, 0, "");
            oFolha.AddValores("NOME", "Nome Trabalhador", 25, "", false, 0, "");
            oFolha.AddValores("DIAS", "Dias", 5, "", false, 0, "");
            oFolha.AddValores("SALARIO", "Total Salario", 10, "#,###,##0.00", true, 0, "");
            oFolha.AddValores("HEXT", "H.Ext.", 9, "#,###,##0.00", true, 0, "");
            oFolha.AddValores("GRATIF", "Gratf", 9, "#,###,##0.00", true, 0, "");
            oFolha.AddValores("SALBRUTO", "Sal.Bruto", 9, "#,###,##0.00", true, 0, "");
            oFolha.AddValores("SALFAM", "Sal.Fam.", 8, "##,##0.00", true, 0, "");
            oFolha.AddValores("INSS", "INSS", 8, "##,##0.00", true, 0, "");
            oFolha.AddValores("IRFONTE", "IRFONTE", 8, "###,##0.00", true, 0, "");
            if (rbImpostoSind.Checked)
                oFolha.AddValores("ISIND", "I.Sind.", 8, "##,##0.00", true, 0, "");
            oFolha.AddValores("SALLIQ", "Sal.Liquido", 10, "#,###,##0.00", true, 0, "");
            oFolha.AddValores("PONTO", "Ponto", 45, "", false, 0, "");

            oBanco = new MonteGrid(); 
           
            oBanco.AddValores("COD", "Código", 6, "", false, 0, "");
            oBanco.AddValores("NOME", "Nome Trabalhador", 25, "", false, 0, "");
            oBanco.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            oBanco.AddValores("CENTRO", "C.R.", 6, "", false, 0, "");
            oBanco.AddValores("BANCO1", "Banco", 6, "", false, 0, "");
            oBanco.AddValores("AGENCIA1", "Agencia", 8, "", false, 0, "");
            oBanco.AddValores("CONTA1", "C/Corrente", 12, "", false, 0, "");
            oBanco.AddValores("SALLIQ", "Sal.Liquido", 10, "#,###,##0.00", true, 0, "");
        }

        
        private void ColocaTotais()
        {
            tsTotais.Items.Clear();
            tsCabecalho.Items.Clear();
            tsTotais.Left = tcFolha.Left + oFolha.oDataGridView.Left + oFolha.oDataGridView.RowHeadersWidth;
            tsCabecalho.Left = tsTotais.Left;
            int tam_Ini = 0;
            string totais = "";

            foreach (DataGridViewColumn coluna in   oFolha.oDataGridView.Columns)
            {
                if (oFolha.dictCampoTotal.ContainsKey(coluna.Name))
                {
                    if (tam_Ini > 0)
                    {
                        tsTotais.Items.Add(new ToolStripLabel(""));
                        tsTotais.Items[tsTotais.Items.Count - 1].AutoSize = false;
                        tsTotais.Items[tsTotais.Items.Count - 1].Width = tam_Ini;
                        tsCabecalho.Items.Add(new ToolStripLabel(""));
                        tsCabecalho.Items[tsTotais.Items.Count - 1].AutoSize = false;
                        tsCabecalho.Items[tsTotais.Items.Count - 1].Width = tam_Ini;

                        tam_Ini = 0;
                        tsTotais.Items.Add(new ToolStripSeparator());
                        tsTotais.Items[tsTotais.Items.Count - 1].AutoSize = false;
                        tsTotais.Items[tsTotais.Items.Count - 1].Width = 1;
                        tsCabecalho.Items.Add(new ToolStripSeparator());
                        tsCabecalho.Items[tsTotais.Items.Count - 1].AutoSize = false;
                        tsCabecalho.Items[tsTotais.Items.Count - 1].Width = 1;
                    }
                    var tot = oFolha.dictCampoTotal[coluna.Name];
                    totais = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(tot));
                    tsTotais.Items.Add(new ToolStripLabel(totais));
                    tsTotais.Items[tsTotais.Items.Count - 1].AutoSize = false;
                    tsTotais.Items[tsTotais.Items.Count - 1].Width = coluna.Width-1;
                    
                    tsCabecalho.Items.Add(new ToolStripLabel(coluna.HeaderText));
                    tsCabecalho.Items[tsTotais.Items.Count - 1].AutoSize = false;
                    tsCabecalho.Items[tsTotais.Items.Count - 1].Width = coluna.Width - 1;




                    tsTotais.Items.Add(new ToolStripSeparator());
                    tsTotais.Items[tsTotais.Items.Count - 1].AutoSize = false;
                    tsTotais.Items[tsTotais.Items.Count - 1].Width = 1;
                    tsCabecalho.Items.Add(new ToolStripSeparator());
                    tsCabecalho.Items[tsTotais.Items.Count - 1].AutoSize = false;
                    tsCabecalho.Items[tsTotais.Items.Count - 1].Width = 1;
                }
                else
                {
                    tam_Ini = tam_Ini + coluna.Width - 1;
                }
            }
            if (tsCabecalho.Items.Count > 0)
                tsCabecalho.Items[0].Text = "N.Trabalhadores :" + (oFolha.oDataGridView.DataSource as BindingSource).Count.ToString();

        
        }



        private bool ConfigureRelatorioFolha()
        {
           // int tipo = rbRecibos.Checked ? 1 : rbVauches.Checked ? 2 : 0;
            //if (tipo == 0) { MessageBox.Show("Selecione Tipo"); return false; };
            //Recibos orecibos = oFolhaImprimeImp.MonteRecibos(dgvCheque, bmCheques, bmChequesFilhos, "Gandu(Ba)", tipo);
            //orecibos.empresa = "M.Libanio Agricola S.A.";

            printDocument2.PrintPage -= PrintDocument2_PrintPage;
            printDocument2.PrintPage += PrintDocument2_PrintPage;

            

            //BindingSource oBind = new BindingSource();
            //oBind.DataSource = bmFolha;
            oFolhaImprime = new FolhaImp(bmFolha, printDocument2,
                      new Font("MS Sans Serif", 8, FontStyle.Regular, GraphicsUnit.Point), // new Font("Arial", 12, FontStyle.Regular),
                "M.Libanio Agrícola S.A.", "FOLHA DE PAGAMENTO de "
                       +inicio.ToString("d") + " a "+fim.ToString("d"), TabelasIniciais.SetorDesc(dadosSetor[0]), dadosSetor[0]);
           
            return true;

        }
        private void PrintDocument2_PrintPage(object sender, PrintPageEventArgs e)
        {
            bool more = oFolhaImprime.DrawDataGridView(e);
            if (more == true)
                e.HasMorePages = true;
        }
        private bool SetupThePrintingFolha()
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

            printDialog2.PrinterSettings.DefaultPageSettings.Landscape = true;
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
            printDocument2.DefaultPageSettings.Landscape = true;

            return true;

        }

        private  void btnFolha_Click(object sender, EventArgs e)
        {
            if ((bmFolha == null) || (bmFolha.Count == 0)) return;
            if (dadosSetor.Count != 1)
            {
                MessageBox.Show((dadosSetor.Count == 0) ? "Selecione um Setor" : "Selecione Sómente 1 Setor");
                return;
            }
           
            int posicao = bmFolha.Position;
            primeiraimpressao = false;
            if (!primeiraimpressao)
                primeiraimpressao =
                    SetupThePrintingFolha();
            ConfigureRelatorioFolha();
            if (primeiraimpressao)
            {

                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument2;
                MyPrintPreviewDialog.ShowDialog();
            }
            bmFolha.Position = posicao;
        }

        private bool FiltreDados()
        {
            if (folhaVirtual == null) return false;

            txNomeTrab = txNomeTrab.Trim();
            if (txTrab.Trim() == "")
                txTrab = "";
            if (txSetor.Trim() == "")
                txSetor = "";

            List<string> dadosNomeTrab = new List<string>();
            if (txNomeTrab != "")
                dadosNomeTrab = txNomeTrab.Split(Convert.ToChar("/")).ToList();
            dadosNomeTrab.RemoveAll(a => a.Trim() == "");

            List<string> dadosTrab = new List<string>();
            if (txTrab != "")
                dadosTrab = txTrab.Split(Convert.ToChar("/")).ToList();
            dadosTrab.RemoveAll(a => a.Trim() == "");

            dadosSetor = new List<string>();
            if (txSetor != "")
                dadosSetor = txSetor.Split(Convert.ToChar("/")).ToList();
             dadosSetor.RemoveAll(a => a.Trim() == "");
            for (int i = 0; i < dadosSetor.Count; i++)
            {
                dadosSetor[i] = dadosSetor[i].Trim();
            }
            

            


            bmFolha = new BindingSource();

            bmFolha.DataSource = folhaVirtual.AsEnumerable().
               Where(a => (rbDiaristas.Checked ? a.Field<string>("MENSALISTA").Trim() == "" : true)
                    && (rbMensalistas.Checked ? a.Field<string>("MENSALISTA").Trim() == "X" : true)
                    && (rbContaCom.Checked ? (!a.IsNull("CONTA1")) &&  a.Field<string>("CONTA1").Trim() != "" : true)
                    && (rbContaSem.Checked ? (a.IsNull("CONTA1") || a.Field<string>("CONTA1").Trim() == "") : true)
                   // && (dadosTrab.Count == 0 ? true : dadosTrab.Contains(a.Field<string>("COD")))
                    &&
                       (dadosSetor.Count == 0 ? true : dadosSetor.Contains(a.Field<string>("SETOR").Trim()))
                    && (dadosNomeTrab.Count == 0 ? true : dadosNomeTrab.AsEnumerable()
                        .Where(b=> a.Field<string>("NOME").StartsWith(b)).FirstOrDefault() != null)
                    && (dadosTrab.Count == 0 ? true : dadosTrab.AsEnumerable()
                        .Where(b => a.Field<string>("COD").StartsWith(b) ).FirstOrDefault() != null)


               ).AsDataView();


            // .OrderBy(a => a.Field<string>("COD")).AsDataView();

            SortDados();

            oFolha.oDataGridView.DataSource = bmFolha;
            oBanco.oDataGridView.DataSource = bmFolha;
            return true;
        }
        private void SortDados()
        {
            if ((bmFolha == null) || (bmFolha.Count == 0)) return;
            if (rbSortCodigo.Checked)
                bmFolha.Sort = "COD";
            else if (rbSortAlfaSetor.Checked)
            {
                bmFolha.Sort = "SETOR, NOME";
            }
            else if (rbSortAlfaGeral.Checked)
            {
                bmFolha.Sort = "NOME";
            }

        }


        private void rbTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (FiltreDados())
            {
                oFolha.FuncaoSoma();
                ColocaTotais();
            }
        }

        private void rbSortAlfaGeral_CheckedChanged(object sender, EventArgs e)
        {
            SortDados();
        }

        private void rbContaTodos_CheckedChanged(object sender, EventArgs e)
        {
            if (FiltreDados())
            {
                oFolha.FuncaoSoma();
                ColocaTotais();
            }
            
        }
        public void FiltreSetaCampos(string otxTrab, string otxSetor, string otxNome)
        {
            txNomeTrab = otxNome;
            txSetor = otxSetor;
            txTrab = otxTrab;
            
        }
        private void txNome_TextChanged(object sender, EventArgs e)
        {
                   FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, (sender as TextBox).Text);
                   if (FiltreDados())
                   {
                       oFolha.FuncaoSoma();
                       ColocaTotais();
                   }
            
        }
        private void txTrabalhador_TextChanged(object sender, EventArgs e)
        {
                 FiltreSetaCampos((sender as TextBox).Text, txSetores.Text, txNome.Text);
                 if (FiltreDados())
                 {
                     oFolha.FuncaoSoma();
                     ColocaTotais();
                 }
            
        }

        private void txSetores_TextChanged(object sender, EventArgs e)
        {
               FiltreSetaCampos(txTrabalhador.Text, (sender as TextBox).Text, txNome.Text);
               if (FiltreDados())
               {
                   oFolha.FuncaoSoma();
                   ColocaTotais();
               }
            
        }

        static public void WriteListaInsumos(string filename, Dictionary<string, object> oLista)
        {

            FileMode oenum = FileMode.Truncate;
            if (File.Exists(filename) == false)
                oenum = FileMode.CreateNew;

            using (FileStream fs = new FileStream(filename, oenum))
            {
                using (StreamWriter writer = new StreamWriter(fs))
                {
                    foreach (var olinha in oLista)
                        writer.WriteLine(olinha.Key + "=" + olinha.Value.ToString());
                }
                fs.Close();
            }
        }
        private void btnExpBB_Click(object sender, EventArgs e)
        {
            if ((bmFolha == null) || (bmFolha.Count == 0)) return;

            string filename = "FolhaBB_" + dtData1.Value.ToString("yyyy/MM");
            filename = filename.Replace("/", "_");

            saveFileDialog1.FileName = filename;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Documentos do Texto (.txt)|*.txt|Todos Arquivos|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tarquivo = saveFileDialog1.FileName;


                int pos = bmFolha.Position;
                bmFolha.MoveFirst();

                try
                {
                    StreamWriter sw = new StreamWriter(tarquivo, false, Encoding.Default); 
                        //File.CreateText(tarquivo);

                    sw.NewLine = Convert.ToString((char)0xD + (char)0xA);
                    sw.AutoFlush = false;
                     
                    //string codant = "";
                    int CurrentRow = 0;
                    while (CurrentRow < bmFolha.Count)
                    {
                        DataRowView olinha = (bmFolha.Current as DataRowView);

                        if ((Convert.ToDecimal(olinha["SALLIQ"]) == 0) || (olinha["CONTA1"].ToString().Trim() == ""))
                        {
                            CurrentRow++;
                            bmFolha.MoveNext();
                            continue;
                        }

                        string salliq = String.Format("{0:000000000000000}", Convert.ToDecimal(olinha["SALLIQ"]) * 100);

                        string linha = "";
                        // Nome do favorecido
                        linha = olinha["NOME"].ToString().Trim().PadRight(30, Convert.ToChar(" ")).Substring(0,30);          //TDataControlReduzido.preenchaespaco(olinha["NOME"].ToString(), 30);
                        // tipo inscricao
                        linha = linha + "1";
                        // inscriçao cpf
                        string cpf = olinha["CPF"].ToString().Trim();
                        while (cpf.IndexOf(Convert.ToChar("-")) > -1 )
                        {
                            cpf = cpf.Remove(cpf.IndexOf(Convert.ToChar("-"), 1),1);
                        }
                        linha = linha + cpf.PadRight(14, Convert.ToChar(" ")).Substring(0,14);

                        // dia pagamento

                        linha = linha + dtCheque.Value.Day.ToString().PadLeft(2, Convert.ToChar("0"));
                        // mes pagamento
                        linha = linha + dtCheque.Value.Month.ToString().PadLeft(2, Convert.ToChar("0"));
                        // ano pagamento
                        linha = linha + dtCheque.Value.Year.ToString().PadLeft(4, Convert.ToChar("0"));


                        // Int32 vlr = 0;

                        linha = linha + salliq;

                        // vinte espaços
                        linha = linha + " ".PadLeft(20, Convert.ToChar(" "));

                        // forma de pagamento
                        linha = linha + "01";
                        // finalidade do pagamento
                        linha = linha + "  ";
                        // linha += "  ";
                        linha = linha + "30";
                        // banco favoreciddo
                        linha = linha + olinha["BANCO1"].ToString();

                        // agencia favoreciddo
                        linha = linha + olinha["AGENCIA1"].ToString().Trim();

                        // conta favorecida
                        string conta1 = olinha["CONTA1"].ToString().Trim();
                        while (conta1.IndexOf(Convert.ToChar("-")) > -1)
                        {
                            conta1 = conta1.Remove(conta1.IndexOf(Convert.ToChar("-"), 1),1);
                        }
                        
                        linha = linha + conta1.Trim().PadRight(12, Convert.ToChar(" ")).Substring(0,12);
                        // PreencheEspaco
                        //(tireHifen(FieldbyName("CONTA1").asString), 12);

                        // indicador de emissão de aviso de cred
                        linha = linha + "1";
                        linha = linha + olinha["END_RUA"].ToString().Trim().PadRight(30, Convert.ToChar(" ")).Substring(0,30);
                        linha = linha + olinha["END_CID"].ToString().Trim().PadRight(20, Convert.ToChar(" ")).Substring(0,20);
                        string cep = "";
                        foreach (char letra in olinha["END_CEP"].ToString())
                        {
                            if (Char.IsNumber(letra))
                            {
                                cep = cep + letra;
                            }
                        }
                        linha = linha + cep.Trim().PadRight(8, Convert.ToChar(" ")).Substring(0,8);
                        linha = linha + olinha["END_UF"].ToString().Trim().PadRight(2, Convert.ToChar(" "));
                        linha = linha + "1";
                        linha = linha + (char)0xD + (char)0xA;// +  CHR($D) + CHR($A);


                        // DESTA MANEIRA ESCREVE EM ASCII
                        sw.Write(linha, Encoding.Default);

                        // DESTA NÃO ESCREVE os sinais da Lingua portuguesa n
                        //sw.WriteLine(linha);
                        // ou
                        /* foreach (char let in linha)
                        {
                            sw.Write(let);
                        }*/

                        CurrentRow++;
                        bmFolha.MoveNext();
                    }
                    bmFolha.Position = pos;
                  
                    sw.Close();
                }
                catch (Exception E)
                {
                    MessageBox.Show("Erro:" + E.Message);
                }



            }
        }

        private async void btnExpFinan_Click(object sender, EventArgs e)
        {
            if ((bmFolha == null) || (bmFolha.Count == 0)) return;
            DataSet dataSet = null;

            try
            {
                dataSet = await ApiServices.Api_QueryMulti(new List<string>() { "SELECT * FROM MOV_FIN WHERE ID = null", 
                      "SELECT MAX(MOV_ID) max_movID FROM MOV_FIN" });

            }
            catch (Exception)
            {
                MessageBox.Show("Erro Acesso Banco de Dados ou Api");
                return;
            }
            if ((dataSet == null) || (dataSet.Tables.Count == 0))
            {
                MessageBox.Show("Não Encontrado MOV_FIN no Banco de Dados");
                return;
            }
            DataTable movfin = dataSet.Tables[0].Clone();
            double maxMov_ID = 0;
            try
            {
                maxMov_ID = Convert.ToDouble(dataSet.Tables[1].Rows[0]["max_movID"]);
            }
            catch (Exception)
            {
                MessageBox.Show("Erro Max Mov_ID");
                return;

            }
           

            int pos = bmFolha.Position;
            bmFolha.MoveFirst();
            int CurrentRow = 0;
            while (CurrentRow < bmFolha.Count)
            {
                DataRowView orowOrig = (bmFolha.Current as DataRowView);

               if ((Convert.ToDecimal(orowOrig["SALLIQ"]) == 0) || (orowOrig["CONTA1"].ToString().Trim() != ""))
                {
                    CurrentRow++;
                    bmFolha.MoveNext();
                    continue;
                }
                DataRow orow = movfin.NewRow();
                orow["DATA"] = dtCheque.Value;
                orow["FORN"] = orowOrig["Nome"].ToString().Trim();
                orow["HIST"] = "SALDO SALARIO " + dtCheque.Value.ToString("MM/yyyy") + " [" +
                                 orowOrig["SETOR"].ToString().Trim().PadLeft(2, Convert.ToChar("0")) + "]";
                orow["VALOR"] = Convert.ToDouble(orowOrig["SALLIQ"]);
                orow["DEBITO"] = "PROV # FOLHA";
                orow["CREDITO"] = "00";
                orow["TP_FIN"] = true;
                orow["TIPO"] = "P";
                orow["DATA_EMI"] = dtCheque.Value;
                orow["VENC"] = dtCheque.Value;
                maxMov_ID++;
                orow["MOV_ID"] = maxMov_ID;
                movfin.Rows.Add(orow);
                CurrentRow++;
                bmFolha.MoveNext();

            }
            string strquery = "";
            foreach (DataRow orow in movfin.Rows)
            {
                string campo = Prepara_Sql.APIConstruaInclusao(orow, "MOV_FIN", new List<string>())+";";
                strquery = strquery + campo;
            }
            int result = await ApiServices.PostApi(strquery, 9); // inclusao

        }

    /*    static private Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};

        */
        private void PonhaNoReciboExcel(Microsoft.Office.Interop.Excel._Worksheet oworksheet,
                  int tiponum,string letra, object valor )
        {
            int linha = 0;
           // int tiponum = 6; // 
            int via2 = 39;
            int via1 = linha + tiponum;
            string linhaStr = via1.ToString();
            var range = oworksheet.get_Range(letra + linhaStr, letra + linhaStr);
            range.FormulaR1C1 = valor;
            via1 = linha + tiponum + via2;
            linhaStr = via1.ToString();

            range = oworksheet.get_Range(letra + linhaStr, letra + linhaStr);
            range.FormulaR1C1 =valor;

        }

        private void btnRecibos_Click(object sender, EventArgs e)
        {
            string filename = "Recibo de PAgamento de Salário " + dtData1.Value.ToString("MM/yyyy");
            filename = filename.Replace("/", "_");

            saveFileDialog1.FileName = filename;
            saveFileDialog1.DefaultExt = "xls";
            saveFileDialog1.Filter = "Planilha Excel (.xls)|*.xls|Todos Arquivos|*.*";

            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tarquivo = saveFileDialog1.FileName;
                byte[] copiaBytes = Properties.Resources.Modelo_recibo_de_pagamento_de_salario;
                File.WriteAllBytes(tarquivo, copiaBytes);
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

                    /*Microsoft.Office.Interop.Excel.Workbook oWorkbook =
                        ExcelObj.Workbooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);

                    Microsoft.Office.Interop.Excel.Range Oget_Range;
                    try
                    */
                    Microsoft.Office.Interop.Excel.Workbook oWorkbook = ExcelObj.Workbooks.Open(tarquivo, Type.Missing, false, Type.Missing, "", "",
                        Type.Missing, Type.Missing
                        , Type.Missing, Type.Missing
                        , Type.Missing, Type.Missing, true, Type.Missing, Type.Missing);
                    Microsoft.Office.Interop.Excel.Sheets sheets = oWorkbook.Sheets;
                    int pos = bmFolha.Position;
                    bmFolha.MoveFirst();
                    int CurrentRow = 1;
                    int ind1 = 1;
                    while (CurrentRow < bmFolha.Count)
                    {
                        var before = sheets.get_Item(ind1);
                        (sheets.get_Item(1) as Microsoft.Office.Interop.Excel.Worksheet).Copy(before, Type.Missing);
                        ind1++;
                        CurrentRow++;
                       // bmFolha.MoveNext();
                    }
                   // bmFolha.MoveFirst();
                    CurrentRow = 0;

                    ind1 = 0;
                    while (CurrentRow < bmFolha.Count)
                    {
                        ind1++;
                        DataRowView orow = (bmFolha.Current as DataRowView);
                        Microsoft.Office.Interop.Excel._Worksheet oworksheet = (sheets.get_Item(ind1) as Microsoft.Office.Interop.Excel.Worksheet);
                        oworksheet.Name = orow["NOME"].ToString().Substring(0, 15);
                        PonhaNoReciboExcel(oworksheet, 11, "C", orow["NOME"].ToString().Trim());

                        /*int linha = 0;
                        int via1 = 0;
                        int via2 = 39;
                        int tiponum = 11; // nome
                        string letra = "C";
                        via1 = linha + tiponum;
                        string linhaStr = via1.ToString();
                        var range = oworksheet.get_Range(letra + linhaStr, letra + linhaStr);
                        range.FormulaR1C1 = orow["NOME"].ToString();
                        via1 = linha + tiponum + via2;
                        linhaStr = via1.ToString();
                        range = oworksheet.get_Range(letra + linhaStr, letra + linhaStr);
                        range.FormulaR1C1 = orow["NOME"].ToString();*/
                        // Data
                        PonhaNoReciboExcel(oworksheet, 6, "J", dtData1.Value.ToString("MMMM/yyyy"));
                        // CBO  
                        PonhaNoReciboExcel(oworksheet, 11, "H", Math.Truncate(Convert.ToDouble(orow["CBO"])));
                        // Categoria
                        PonhaNoReciboExcel(oworksheet, 11, "I", orow["CATEGORIA"].ToString());
                        // salario
                        PonhaNoReciboExcel(oworksheet, 16, "H", Math.Truncate(Convert.ToDouble(orow["HX"]) / 8));

                        double salconf = 0;
                        double CalcSalbase = 0;
                        double SALARIO = Convert.ToDouble(orow["SALARIO"]);
                        double tpercconf = Convert.ToDouble(orow["PERCCONF"]);
                        if (tpercconf != 0)
                        {
                            CalcSalbase = Math.Round(SALARIO / (1 + (tpercconf / 100)), 2);
                            salconf = SALARIO - CalcSalbase;
                            SALARIO = CalcSalbase;

                        }

                        PonhaNoReciboExcel(oworksheet, 16, "I", SALARIO);

                        PonhaNoReciboExcel(oworksheet, 17, "H", Convert.ToDouble(orow["HXA"]));

                        PonhaNoReciboExcel(oworksheet, 17, "I", Convert.ToDouble(orow["VLR_HXA"]));

                        double vlr_dom = 0;
                        double vlr_fer = 0;
                        double horas_dom = 0;
                        double horas_fer = 0;
                        if (Convert.ToDouble(orow["VLR_HXS"]) > 0)
                        {
                            vlr_dom = Convert.ToDouble(orow["VLR_DOM"]);
                            vlr_fer = Convert.ToDouble(orow["VLR_FER"]);
                            horas_dom = Convert.ToDouble(orow["HORAS_DOM"]);
                            horas_fer = Convert.ToDouble(orow["HORAS_FER"]);

                        }
                        PonhaNoReciboExcel(oworksheet, 18, "H", horas_dom);
                        PonhaNoReciboExcel(oworksheet, 19, "H", horas_fer);
                        PonhaNoReciboExcel(oworksheet, 18, "I", vlr_dom);
                        PonhaNoReciboExcel(oworksheet, 19, "I", vlr_fer);
                        // ADICIONAL NOTURNO
                        PonhaNoReciboExcel(oworksheet, 20, "I", Convert.ToDouble(orow["VLR_HXN"]));
                        // SALARIO FAMILIA
                        PonhaNoReciboExcel(oworksheet, 21, "I", Convert.ToDouble(orow["SALFAM"]));
                        // Desconto taxa INSS Literal
                        string descontotaxa = "Desconto INSS(" + orow["TXINSS"].ToString().Trim() + ")";
                        PonhaNoReciboExcel(oworksheet, 22, "C", descontotaxa);
                        // Desconto INSS
                        PonhaNoReciboExcel(oworksheet, 22, "J", Convert.ToDouble(orow["INSS"]));
                        // IRRF
                        PonhaNoReciboExcel(oworksheet, 23, "J", Convert.ToDouble(orow["IRFONTE"]));

                        // ADIANT
                        PonhaNoReciboExcel(oworksheet, 24, "J", Convert.ToDouble(orow["ADIANT"]));
                        // IMPOSTO SINDICAL
                        PonhaNoReciboExcel(oworksheet, 25, "J", Convert.ToDouble(orow["ISIND"]));
                        // Adicional cargo de confiança
                        PonhaNoReciboExcel(oworksheet, 26, "I", salconf);

                        PonhaNoReciboExcel(oworksheet, 37, "B", Convert.ToDouble(orow["SALARIOATUAL"]));
                        // SALTETO salcontribuicao inss
                        PonhaNoReciboExcel(oworksheet, 37, "D",
                            Convert.ToDouble(orow["SALARIO"])
                            + Convert.ToDouble(orow["VLR_HXA"])
                            + Convert.ToDouble(orow["VLR_HXN"])
                              + Convert.ToDouble(orow["VLR_HXS"])
                            );
                        // SALBRUTO
                        PonhaNoReciboExcel(oworksheet, 37, "E",
                            Convert.ToDouble(orow["SALARIO"])
                            + Convert.ToDouble(orow["VLR_HXA"])
                            + Convert.ToDouble(orow["VLR_HXN"])
                              + Convert.ToDouble(orow["VLR_HXS"])
                            );

                        // FGTS
                        PonhaNoReciboExcel(oworksheet, 37, "G",
                            Convert.ToDouble(orow["FGTS"]));

                        // BASE_IRRF
                        PonhaNoReciboExcel(oworksheet, 37, "I",
                            Convert.ToDouble(orow["BASEIRF"]));

                        CurrentRow++;
                        bmFolha.MoveNext();
                    }
                    bmFolha.Position = pos;

                        (sheets.get_Item(1) as Microsoft.Office.Interop.Excel.Worksheet).Activate();
                    oWorkbook.Save();
                    oWorkbook.Close();
                    ExcelObj.Quit();
                    MessageBox.Show("Excel Salvo");
                }
                catch (Exception E)
                {
                    MessageBox.Show("Erro " + E.Message);
                    return;

                }
            }
        }
    }
}
/*
 * with ExcelApplication1 do
    begin
      connect;
      try

        // visible := false;
        FileName := tdoc;

        try

          WorkBooks.Open(FileName, emptyParam, false, emptyParam, '', '',
            emptyParam, emptyParam, emptyParam, emptyParam, emptyParam,
            emptyParam, true, emptyParam, emptyParam, LCID);
       
          // Sheets.Add(emptyparam,before,ListaServico.Count - 2,emptyparam,lcid);

          with folhavirtual do
          begin
            first;
            ind := 1;
            while not Eof do

           
            begin
              Before := Sheets[ind];
              with Sheets[1] as _worksheet do
                copy(emptyParam, Before, LCID);
              INC(ind);
              next;
            end;
            first;
            ind := 0;
            While not Eof do
            // ListaServico.Count - 1 do
            begin
              with Sheets[ind + 1] as _worksheet do
              begin
                linha := 0;
                via1 := 0;
                via2 := 39;
                name := system.copy(FieldbyName('NOME').asString, 1, 15);

                tiponum := 11; // nome
                letra := 'C';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('NOME').asString;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('NOME').asString;

                tiponum := 6; // Data
                letra := 'J';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := formatdatetime('mmmm/yyyy', dtData1.Date);
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := formatdatetime('mmmm/yyyy', dtData1.Date);

                tiponum := 11; // CBO
                letra := 'H';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := trunc(FieldbyName('CBO').asFloat / 1);
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := trunc(FieldbyName('CBO').asFloat / 1);

                tiponum := 11; // FUNCAO
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('CATEGORIA').asString;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('CATEGORIA').asString;

                tiponum := 16; // salario
                letra := 'H';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := trunc(FieldbyName('HX').asFloat / 8);
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := trunc(FieldbyName('HX').asFloat / 8);

                salconf := 0;
                CalcSalbase := 0;
                SALARIO := FieldbyName('SALARIO').asFloat;

                if (not dmadofinan.UD_APIMODO) then
                begin
                  tpercconf := oAtualFolha.PercConfSalbase
                    (FieldbyName('COD').asString, FieldbyName('SALBASE')
                    .asFloat, FieldbyName('ADMI').asDateTime, dtData1.Date);
                end
                else
                  tpercconf := FieldbyName('PERCCONF').asFloat;

                if (tpercconf <> 0) then
                begin
                  CalcSalbase := ptround(SALARIO / (1 + (tpercconf / 100)), 2);
                  // salario base
                  // salconf := ptRound((tpercConf/100)*salario,2);
                  salconf := SALARIO - CalcSalbase;
                  SALARIO := CalcSalbase;
                end;

                tiponum := 16; // salario
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := SALARIO;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := SALARIO;

                tiponum := 17; // HORA_EXTRA
                letra := 'H';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('HXA').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('HXA').asFloat;

                tiponum := 17; // HORA_EXTRA
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('VLR_HXA').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('VLR_HXA').asFloat;
                ovlrdom_fer := TObjSepareDom_fer.Create;
                if (FieldbyName('VLR_HXS').asFloat > 0) then
                begin
                  if (dmadofinan.UD_APIMODO) then
                  begin
                    ovlrdom_fer.vlr_dom := FieldbyName('VLR_DOM').asFloat;
                    ovlrdom_fer.vlr_fer := FieldbyName('VLR_FER').asFloat;
                    ovlrdom_fer.horas_dom := FieldbyName('HORAS_DOM').asFloat;
                    ovlrdom_fer.horas_fer := FieldbyName('HORAS_FER').asFloat;
                  end
                  else
                  begin
                    tinicio := PrimeirodiaMes(dtData1.Date);
                    if FieldbyName('ADMI').asDateTime > tinicio then
                      tinicio := FieldbyName('ADMI').asDateTime;

                    ovlrdom_fer := oAtualFolha.SepareDomFer
                      (FieldbyName('VLR_HXS').asFloat,
                      FieldbyName('COD').asString, tinicio,
                      UltimodiaMes(dtData1.Date));
                  end;
                end;

                tiponum := 18; // DOMINGOS horas
                letra := 'H';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.horas_dom;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.horas_dom;

                tiponum := 19; // FERIADOS  horas
                letra := 'H';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.horas_fer;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.horas_fer;

                tiponum := 18; // DOMINGOS
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.vlr_dom;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.vlr_dom;

                tiponum := 19; // FERIADOS
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.vlr_fer;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := ovlrdom_fer.vlr_fer;

                tiponum := 20; // ADICIONAL NOTURNO
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('VLR_HXN').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('VLR_HXN').asFloat;

                tiponum := 21; // SALARIO FAMILIA
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('SALFAM').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('SALFAM').asFloat;

                tiponum := 22; // Desconto taxa INSS Literal
                letra := 'C';
                via1 := linha + tiponum;
                if (not dmadofinan.UD_APIMODO) then
                begin
                  edit;
                  FieldbyName('TXINSS').asFloat :=
                    oAtualFolha.taxainss(FieldbyName('data').asDateTime,
                    FieldbyName('SALARIO').asFloat + +FieldbyName('VLR_HXA')
                    .asFloat + FieldbyName('VLR_HXN').asFloat +
                    FieldbyName('VLR_HXS').asFloat);
                  Post;
                end;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := 'Desconto INSS(' +
                  trim(checkfloattostr(FieldbyName('TXINSS').asFloat)) + ')';
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := 'Desconto INSS(' +
                  trim(checkfloattostr(FieldbyName('TXINSS').asFloat)) + ')';

                tiponum := 22; // Desconto INSS
                letra := 'J';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('INSS').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('INSS').asFloat;

                tiponum := 23; // IRRF
                letra := 'J';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('IRFONTE').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('IRFONTE').asFloat;

                tiponum := 24; // ADIANT
                letra := 'J';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('ADIANT').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('ADIANT').asFloat;

                tiponum := 25; // IMPOSTO SINDICAL
                letra := 'J';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('ISIND').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('ISIND').asFloat;

                tiponum := 26; // Adicional cargo de confiança
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := salconf;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := salconf;
                if (not dmadofinan.UD_APIMODO) then
                begin
                  tsalbase := oAtualFolha.CalcSalbase
                    (FieldbyName('COD').asString, FieldbyName('SALBASE')
                    .asFloat, FieldbyName('ADMI').asDateTime, dtData1.Date);
                end
                else
                begin
                  tsalbase := FieldbyName('SALARIOATUAL').asFloat;
                end;
                tiponum := 37; // SALBASE
                letra := 'B';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := tsalbase;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := tsalbase;

                tiponum := 37; // SALTETO salcontribuicao inss
                letra := 'D';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('SALARIO').asFloat +
                  FieldbyName('VLR_HXA').asFloat + FieldbyName('VLR_HXN')
                  .asFloat + FieldbyName('VLR_HXS').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('SALARIO').asFloat +
                  FieldbyName('VLR_HXA').asFloat + FieldbyName('VLR_HXN')
                  .asFloat + FieldbyName('VLR_HXS').asFloat;

                tiponum := 37; // SALBRUTO
                letra := 'E';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('SALARIO').asFloat +
                  FieldbyName('VLR_HXA').asFloat + FieldbyName('VLR_HXN')
                  .asFloat + FieldbyName('VLR_HXS').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('SALARIO').asFloat +
                  FieldbyName('VLR_HXA').asFloat + FieldbyName('VLR_HXN')
                  .asFloat + FieldbyName('VLR_HXS').asFloat;

                tiponum := 37; // FGTS
                letra := 'G';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('FGTS').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('FGTS').asFloat;

                if (not dmadofinan.UD_APIMODO) then
                begin
                  edit;
                  FieldbyName('BASEIRF').asFloat :=
                    (FieldbyName('SALARIO').asFloat + FieldbyName('VLR_HXA')
                    .asFloat + FieldbyName('VLR_HXN').asFloat +
                    FieldbyName('VLR_HXS').asFloat) -
                    FieldbyName('INSS').asFloat;

                  FieldbyName('BASEIRF').asFloat := FieldbyName('BASEIRF')
                    .asFloat - oAtualFolha.PesqDeduzIR
                    (FieldbyName('COD').asString,
                    FieldbyName('data').asDateTime);
                  if FieldbyName('BASEIRF').asFloat < 0 then
                    FieldbyName('BASEIRF').asFloat := 0;

                  Post;
                end;
                tiponum := 37; // BASE_IRRF
                letra := 'I';
                via1 := linha + tiponum;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('BASEIRF').asFloat;
                via1 := linha + tiponum + via2;
                Range[letra + trim(inttostr(via1)), letra + trim(inttostr(via1))
                  ].FormulaR1C1 := FieldbyName('BASEIRF').asFloat;

                // SALARIO + VLR_HXA + VLR_HXN+ VLR_HXS
                // linha := linha + 78;
              end;
              next;
              INC(ind);
            end;
          end;
        except
          ShowMessage('Não Consegui Abrir ' + string(FileName));

          WorkBooks.Close(LCID);
          disconnect;

          raise;
        end;

    
 */ 