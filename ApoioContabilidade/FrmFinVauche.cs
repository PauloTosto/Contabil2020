using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using ClassConexao;
using ClassFiltroEdite;
namespace ApoioContabilidade
{
    public partial class FrmFinVauche : Form
    {

        private DateTime inicio, fim;
        private List<LinhaSolucao> oList;// Lista solucao para pesquisa induzida

        private MonteGrid oEntradas;
        private bool recomece;
        private FormFiltro oForm;

        public BindingSource bmSourceEntrada;
        // public BindingSource bmSourceSaida;
        DataView oPlacon;

        public PesquisaGenerico oPesquisa;


        const int DATA_ger = 0;
        const int DEBITO_ger = 1;
        const int CREDITO_ger = 2;
        const int VALOR_ger = 3;
        const int HIST_ger = 4;
        const int DOC_ger = 5;

        DataTable ErrosNovasContas;
        DataTable VauchesRazao;
        public FrmFinVauche()
        {
            InitializeComponent();



            recomece = true;
            oEntradas = new MonteGrid();
            MonteGrids();
            // dgEntradas.CaptionText = "FINAN E VAUCHES";
            dgEntradas.ReadOnly = true;
            // dgEntradas.AllowSorting = false;
            oEntradas.oDataGridView = dgEntradas;
            oEntradas.sbTotal = sbEntradas;
            oPlacon = TDataControlContab.Placon();


            oForm = new FormFiltro();
            bmSourceEntrada = new BindingSource();
            // bmSourceEntrada.BindingComplete += new BindingCompleteEventHandler(bmSource_BindingComplete);
            //  bmSourceEntrada.AddingNew += new AddingNewEventHandler(bmSource_AddingNew);

        }

        private void FrmFinVauche_Load(object sender, EventArgs e)
        {
            if (recomece)
            {
                oForm.oArme = ArmeEdicaoFiltroGenerico();
                oForm.ShowDialog();
                if (oForm.oPesqFin.TabCount > 0)
                    oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
                else
                    oList = null;
                inicio = oForm.dtData1.Value;
                fim = oForm.dtData2.Value;

                DataSet dsPesquisa = new DataSet();
                dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
                dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());
                oPesquisa = new PesquisaGenerico(dsPesquisa);


                dataSet1 = TDataControlContab.MovFinanVauches(inicio, fim, oList, oPesquisa);

                DataTableCollection oTables = dataSet1.Tables;
                DataView dvEntradas = oTables[0].AsDataView();
                ErrosNovasContas = oTables[0].Clone();
                VauchesRazao = oTables[0].Clone();


                bmSourceEntrada.DataSource = dvEntradas;
                ///bmSourceSaida.DataSource = dvSaidas;
                dgEntradas.DataSource = bmSourceEntrada;

                //oEntradas.ConfigureDBGrid();
                oEntradas.ConfigureDBGridView();
                System.Nullable<decimal> total1 =
                  (from valor1 in dataSet1.Tables[0].AsEnumerable() where (valor1.Field<string>("DEBITO").Trim() != "")
                   && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("DEBITO")))
                   select valor1.Field<decimal>("VALOR")).Sum();
                oEntradas.LinhasCampo[1].total = Convert.ToDouble(total1);

                System.Nullable<decimal> total2 =
          (from valor1 in dataSet1.Tables[0].AsEnumerable() where (valor1.Field<string>("CREDITO").Trim() != "")
            && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("CREDITO")))
           select valor1.Field<decimal>("VALOR")).Sum();
                oEntradas.LinhasCampo[2].total = Convert.ToDouble(total2);

                oEntradas.ColocaTotais();

                recomece = false;
            }
            this.Refresh();

        }
        private void MonteGrids()
        {
            oEntradas.Clear();
            oEntradas.AddValores("DATA", "DATA", 0, "", false, 0, "");
            oEntradas.AddValores("DEBITO", "DEBITO", 40, "", true, 0, "");
            oEntradas.AddValores("CREDITO", "CREDITO", 40, "", true, 0, "");
            oEntradas.AddValores("VALOR", "VALOR", 12, "#,###,##0.00", true, 0, "");
            oEntradas.AddValores("HIST", "HISTORICO", 40, "", false, 0, "");
            oEntradas.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oEntradas.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");


        }
        private ArmeEdicao ArmeEdicaoFiltroGenerico()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha = new Linha("SUBS(DEBITO,1,2) / SUBS(CREDITO,1,2)");//("CTAFIN");
            olinha.cabecalho[0] = "Bancos";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contas";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("HIST");
            olinha.cabecalho[0] = "Historico";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("FORN");
            olinha.cabecalho[0] = "Titular";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DOC_FISC");
            olinha.cabecalho[0] = "Doc.Fiscal";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("VALOR");
            olinha.cabecalho[0] = "Valor";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("=");

            ((ComboBox)olinha.oedite[0]).Items.Add(">");
            ((ComboBox)olinha.oedite[0]).Items.Add("<");
            ((ComboBox)olinha.oedite[0]).Width = 30;
            olinha.oedite[1] = new NumericTextBox();
            ((NumericTextBox)olinha.oedite[1]).TextAlign = HorizontalAlignment.Right;
            olinha.ofuncaoSql = Miscelania.CompareValor;
            oArme.Linhas.Add(olinha);


            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contabilidade";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("S Classificados");
            ((ComboBox)olinha.oedite[0]).Items.Add("N Não Classificados");
            olinha.ofuncao = TDataControlContab.fPassa_Contab2;
            oArme.Linhas.Add(olinha);




            olinha = new Linha("DOC");
            olinha.cabecalho[0] = "Documento";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);


            return oArme;

        }

        // Rotinas para Exportar dados

        private string FormateParaNumConta(string conta, DataTable tabrelaciona)
        {
            try
            {
                string contanum = "";
                string result = "";
                if (conta.Trim() == "") return "";
                if (conta.Trim().Length < 3)
                    if (TDataControlContab.EBanco(conta, true))
                    {
                        contanum = TDataControlContab.EBancoContab(conta);
                        /*var rowsPesq = tabrelaciona.AsEnumerable().Where(row => row.Field<string>("NUMCONTA").Trim() == contanum);
                        if ((rowsPesq != null) && (rowsPesq.Count() > 0))
                        {
                            DataRow retRow = rowsPesq.FirstOrDefault();
                            if (retRow != null)
                            {
                                result = retRow["NOVOCOD"].ToString().Trim();// .Substring(0, 9);
                                return result;
                            }
                        }
                        */


                        for (int i = 0; i < tabrelaciona.Rows.Count; i++)
                        {
                            if (Convert.ToString(tabrelaciona.Rows[i]["NUMCONTA"]).Trim() == contanum)
                            {
                                result = Convert.ToString(tabrelaciona.Rows[i]["NOVOCOD"]);
                                break;
                            }
                        }
                        if (result == "")
                        {
                            result = "-1";

                        }
                        return result;
                    }
                if (oPesquisa.ExisteValor("BANCOS", "DESC2", conta)) return "";

                contanum = TDataControlContab.EPlanoRetornaNumConta(conta, oPesquisa);
                /* var rows = tabrelaciona.Where(row => (row.Field<string>("NUMCONTA").Trim() == contanum)).Select(row=> row);
                 if (rows != null)
                 {
                     int cont = rows.Count();
                     DataRow retRow = rows.FirstOrDefault();
                     if (retRow != null)
                         result = retRow["NOVOCOD"].ToString().Trim();// .Substring(0, 9);
                 }*/

                for (int i = 0; i < tabrelaciona.Rows.Count; i++)
                {
                    if (Convert.ToString(tabrelaciona.Rows[i]["NUMCONTA"]).Trim() == contanum)
                    {
                        result = Convert.ToString(tabrelaciona.Rows[i]["NOVOCOD"]);// .Substring(0, 9);
                        break;
                    }
                }
                if (result == "")
                {
                    result = "-1";

                }
                return result;


            }
            catch (Exception E)
            {
                MessageBox.Show("Erro 3: " + E.Message);
                throw;
            }

        }

        private string FormateNumeroCodigo(string conta)
        {
            try
            {
                if ((conta.Length == 0) || (conta.Length > 9) || (conta.Length == 7) || (conta.Length == 8) || (conta.Length == 5)) return "";
                if ((conta.Length == 9) && (conta.Substring(6, 3) == "000"))
                    conta = conta.Substring(0, 6);
                if ((conta.Length == 6) && (conta.Substring(4, 2) == "00"))
                    conta = conta.Substring(0, 4);
                if ((conta.Length == 4) && (conta.Substring(3, 1) == "0"))
                    conta = conta.Substring(0, 3);
                if ((conta.Length == 3) && (conta.Substring(2, 1) == "0"))
                    conta = conta.Substring(0, 2);
                if ((conta.Length == 2) && (conta.Substring(1, 1) == "0"))
                    conta = conta.Substring(0, 1);
                if ((conta.Length == 1) && (conta.Substring(0, 1) == "0"))
                    return conta = "";
                string resultado = conta.Substring(0, 1);
                int passo = 1;
                for (int i = 1; i < conta.Length; i += passo)
                {
                    if (i > 6) break;
                    if (i == 4) passo = 2;
                    if (i == 6) passo = 3;
                    resultado = resultado + "." + conta.Substring(i, passo);
                }
                return resultado;
            }
            catch (Exception)
            {
                MessageBox.Show("Erro 2");
                throw;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {


            saveFileDialog1.FileName = "MovimImportado.txt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tarquivo = saveFileDialog1.FileName;

                DataSet dsrelaciona = TDataControlContab.TabRelaciona();
                DataTable tabrelaciona = dsrelaciona.Tables[0];


                DataTable tabordenada = dataSet1.Tables[0];

                try
                {
                    StreamWriter sw = File.CreateText(tarquivo);
                    //string codant = "";
                    for (int i = 0; i < tabordenada.Rows.Count; i++)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                    {

                        DataRow linexc = tabordenada.Rows[i];// dsrelaciona.Tables["RELACIONA"].Rows[i];


                        if (((Convert.IsDBNull(linexc["DEBITO"])) || (Convert.ToString(linexc["DEBITO"]).Trim() == ""))
                            && ((Convert.IsDBNull(linexc["CREDITO"])) || (Convert.ToString(linexc["CREDITO"]).Trim() == "")))
                            continue;

                        string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));
                        string debito = FormateParaNumConta(Convert.ToString(linexc["DEBITO"]), tabrelaciona);
                        string credito = FormateParaNumConta(Convert.ToString(linexc["CREDITO"]), tabrelaciona);
                        if ((debito == "-1") || (credito == "-1"))
                        {
                            DataRow orow = ErrosNovasContas.NewRow();
                            orow.ItemArray = linexc.ItemArray;
                            ErrosNovasContas.Rows.Add(orow);
                            continue;
                        }
                        if (!Convert.ToBoolean(linexc["TP_FIN"]))
                        {
                            DataRow razaoRow = VauchesRazao.NewRow();
                            razaoRow.ItemArray = linexc.ItemArray;
                            VauchesRazao.Rows.Add(razaoRow);
                        }
                        string linha = "";
                        // debito = FormateNumeroCodigo(debito);
                        // credito = FormateNumeroCodigo(credito);
                        if ((credito.Trim() == "") && (debito.Trim() == "")) continue;
                        linha = TDataControlReduzido.preenchaespaco(data, 8);
                        linha += TDataControlReduzido.preenchaespaco("", 5);
                        linha += TDataControlReduzido.preenchaespaco(debito, 14);
                        linha += TDataControlReduzido.preenchaespaco("", 3);
                        linha += TDataControlReduzido.preenchaespaco(credito, 14);
                        linha += TDataControlReduzido.preenchaespaco("", 3);
                        linha += TDataControlReduzido.FormatNumero(Convert.ToSingle(linexc["VALOR"]), 14);
                        linha += TDataControlReduzido.preenchaespaco("", 3);
                        string tdoc = "";
                        if (Convert.ToString(linexc["DOC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC"]).Trim();
                        else
                            if (Convert.ToString(linexc["DOC_FISC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC_FISC"]).Trim();
                        if (tdoc != "")
                            tdoc = "DOC.n.:" + tdoc;

                        if (Convert.ToString(linexc["HIST"]).Trim() != "")
                            linha += TDataControlReduzido.preenchaespaco(Convert.ToString(linexc["HIST"]).Trim() + " " + tdoc, 50);
                        else
                            linha += TDataControlReduzido.preenchaespaco(tdoc, 50);

                        sw.WriteLine(linha);

                    }
                    sw.Close();
                }
                catch (Exception)
                {
                    throw;
                }



            }
        }

        private void btnExportaModExcel_Click(object sender, EventArgs e)
        {
            ExportaExcel();
            /*  saveFileDialog1.FileName = "MovimExporta.xlsx";
              if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
              {
                  string tarquivo = saveFileDialog1.FileName;

                  DataSet dsrelaciona = TDataControlContab.TabRelaciona();
                  DataTable tabrelaciona = dsrelaciona.Tables[0];


                  DataTable tabordenada = dataSet1.Tables[0];

                  try
                  {
                      for (int i = 0; i < tabordenada.Rows.Count; i++)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                      {

                          DataRow linexc = tabordenada.Rows[i];// dsrelaciona.Tables["RELACIONA"].Rows[i];


                          if (((Convert.IsDBNull(linexc["DEBITO"])) || (Convert.ToString(linexc["DEBITO"]).Trim() == ""))
                              && ((Convert.IsDBNull(linexc["CREDITO"])) || (Convert.ToString(linexc["CREDITO"]).Trim() == "")))
                              continue;

                          string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));
                          string debito = FormateParaNumConta(Convert.ToString(linexc["DEBITO"]), tabrelaciona);
                          string credito = FormateParaNumConta(Convert.ToString(linexc["CREDITO"]), tabrelaciona);
                          if ((debito == "-1") || (credito == "-1"))
                          {
                              DataRow orow = ErrosNovasContas.NewRow();
                              orow.ItemArray = linexc.ItemArray;
                              ErrosNovasContas.Rows.Add(orow);
                              continue;
                          }
                          string linha = "";
                          // debito = FormateNumeroCodigo(debito);
                          // credito = FormateNumeroCodigo(credito);
                          if ((credito.Trim() == "") && (debito.Trim() == "")) continue;
                          linha = TDataControlReduzido.preenchaespaco(data, 8);
                          linha += TDataControlReduzido.preenchaespaco("", 5);
                          linha += TDataControlReduzido.preenchaespaco(debito, 14);
                          linha += TDataControlReduzido.preenchaespaco("", 3);
                          linha += TDataControlReduzido.preenchaespaco(credito, 14);
                          linha += TDataControlReduzido.preenchaespaco("", 3);
                          linha += TDataControlReduzido.FormatNumero(Convert.ToSingle(linexc["VALOR"]), 14);
                          linha += TDataControlReduzido.preenchaespaco("", 3);
                          string tdoc = "";
                          if (Convert.ToString(linexc["DOC"]).Trim() != "")
                              tdoc = Convert.ToString(linexc["DOC"]).Trim();
                          else
                              if (Convert.ToString(linexc["DOC_FISC"]).Trim() != "")
                              tdoc = Convert.ToString(linexc["DOC_FISC"]).Trim();
                          if (tdoc != "")
                              tdoc = "DOC.n.:" + tdoc;

                          if (Convert.ToString(linexc["HIST"]).Trim() != "")
                              linha += TDataControlReduzido.preenchaespaco(Convert.ToString(linexc["HIST"]).Trim() + " " + tdoc, 50);
                          else
                              linha += TDataControlReduzido.preenchaespaco(tdoc, 50);

                      }

                  }
                  catch (Exception)
                  {
                      throw;
                  }
              }*/
        }

        private void button2_Click(object sender, EventArgs e)
        {
            FrmErros oForm = new FrmErros(ErrosNovasContas);
            //oForm.otable = ErrosNovasContas.Copy();
            oForm.Show();
        }
        private void btnRazao_Click(object sender, EventArgs e)
        {
           // PesquiseRazao();
           // FrmRazao oForm = new FrmRazao(VauchesRazao, oPesquisa);
            //oForm.otable = ErrosNovasContas.Copy();
           // oForm.Show();
        }

        

        private Microsoft.Office.Interop.Excel.Range Colunas_exc(string colunas, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            coluna1 = colunas.Trim();
            return oworksheet.get_Range(coluna1 + "1", coluna1 + "65536");
        }

        private Microsoft.Office.Interop.Excel.Range Colunas_exc(string Coluna1, string Coluna2, int Linha1, int Linha2, Microsoft.Office.Interop.Excel._Worksheet oworksheet)
        {
            string coluna1 = "";
            string coluna2 = "";
            coluna1 = Coluna1.Trim();
            coluna2 = Coluna2.Trim();
            return oworksheet.get_Range(coluna1 + Linha1.ToString().Trim(), coluna2 + Linha2.ToString().Trim());
        }

        private Char[] Letras = new Char[21] {'A','B','C','D','E','F','G','H','I','J','K',
                                  'L','M','N','O','P','Q','R','S','T','U'};

        

        public void ExportaExcel()
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
                        DataTable tabrelaciona = dsrelaciona.Tables[0];


                        DataTable tabordenada = dataSet1.Tables[0];
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

                        /*numcol = 1;
                        linhas = linhasinicio;
                        linhastr = linhas.ToString();

                        // cabeçalho
                        Type tipo = null;
                        foreach (Campo campo in LinhasCampo)
                        {
                            Oget_Range = Colunas_exc(Letras[numcol].ToString(), Letras[numcol].ToString(),
                                                      (linhas + 1), linhasmax, oworksheet);
                            oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = campo.cabecalho;
                            Oget_Range.ColumnWidth = campo.tamanho;
                            if (odataview.Table.Columns.Contains(campo.titulo)) { tipo = odataview.Table.Columns[campo.titulo].DataType; }
                            if (tipo != null)
                            {
                                if ((tipo != Type.GetType("System.String")) && (tipo != Type.GetType("System.Boolean")) && (tipo != Type.GetType("System.DateTime")))
                                {
                                    int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                                    if (tam > Oget_Range.ColumnWidth)
                                        Oget_Range.ColumnWidth = tam;
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignRight;
                                }
                                if ((tipo == Type.GetType("System.DateTime")) && (campo.format != ""))
                                {
                                    int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                                    if (tam > Oget_Range.ColumnWidth)
                                        Oget_Range.ColumnWidth = tam;
                                    oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
                                }
                            }
                            else
                            {
                                if (campo.format != "")
                                {
                                    int tam = campo.format.Length > 0 ? campo.format.Length : 5;
                                    if (tam > Oget_Range.ColumnWidth)
                                        Oget_Range.ColumnWidth = tam;
                                }
                            }
                            numcol += 1;
                        }
                        */

                        linhas += 1;

                        
                        try
                        {
                            foreach (DataRow linexc in tabordenada.Rows)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                            {



                                if (((Convert.IsDBNull(linexc["DEBITO"])) || (Convert.ToString(linexc["DEBITO"]).Trim() == ""))
                                    && ((Convert.IsDBNull(linexc["CREDITO"])) || (Convert.ToString(linexc["CREDITO"]).Trim() == "")))
                                    continue;

                                string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));
                                string debito = FormateParaNumConta(Convert.ToString(linexc["DEBITO"]), tabrelaciona);
                                string credito = FormateParaNumConta(Convert.ToString(linexc["CREDITO"]), tabrelaciona);
                                if ((debito == "-1") || (credito == "-1"))
                                {
                                    DataRow orow = ErrosNovasContas.NewRow();
                                    orow.ItemArray = linexc.ItemArray;
                                    ErrosNovasContas.Rows.Add(orow);
                                    continue;
                                }
                               // string linha = "";
                                // debito = FormateNumeroCodigo(debito);
                                // credito = FormateNumeroCodigo(credito);
                                if ((credito.Trim() == "") && (debito.Trim() == "")) continue;
                                /*linha = TDataControlReduzido.preenchaespaco(data, 8);
                                linha += TDataControlReduzido.preenchaespaco("", 5);
                                linha += TDataControlReduzido.preenchaespaco(debito, 14);
                                linha += TDataControlReduzido.preenchaespaco("", 3);
                                linha += TDataControlReduzido.preenchaespaco(credito, 14);
                                linha += TDataControlReduzido.preenchaespaco("", 3);
                                linha += TDataControlReduzido.FormatNumero(Convert.ToSingle(linexc["VALOR"]), 14);
                                linha += TDataControlReduzido.preenchaespaco("", 3);*/
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
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "dd/mm/aaaa";
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = Convert.ToDateTime(linexc["DATA"]);
                                
                                numcol = 4; // valor
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).NumberFormat = "##.###.##0,00";
                                oworksheet.get_Range(Letras[numcol] + linhastr, Letras[numcol] + linhastr).Value2 = Convert.ToDecimal(linexc["VALOR"]);
                                
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
                        MessageBox.Show("Erro ao tentar abrir tabela Excel "+E.Message, "",
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
