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
using OfficeOpenXml;
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

        

        public void ExportaExcel()
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

                DataSet dsrelaciona = TDataControlContab.TabRelaciona();
                DataTable tabrelaciona = dsrelaciona.Tables[0];
                DataTable tabordenada = dataSet1.Tables[0];

                using (var pkg = new ExcelPackage(fi))
                {
                    var ws = pkg.Workbook.Worksheets.Add("Planilha1");

                    // Cabecalho linha 1 (mesmo layout AlterData de ExportaLoteExcel)
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
                    foreach (DataRow linexc in tabordenada.Rows)
                    {
                        bool debitoVazio = Convert.IsDBNull(linexc["DEBITO"]) || Convert.ToString(linexc["DEBITO"]).Trim() == "";
                        bool creditoVazio = Convert.IsDBNull(linexc["CREDITO"]) || Convert.ToString(linexc["CREDITO"]).Trim() == "";
                        if (debitoVazio && creditoVazio) continue;

                        string debito = FormateParaNumConta(Convert.ToString(linexc["DEBITO"]), tabrelaciona);
                        string credito = FormateParaNumConta(Convert.ToString(linexc["CREDITO"]), tabrelaciona);
                        if (debito == "-1" || credito == "-1")
                        {
                            DataRow orow = ErrosNovasContas.NewRow();
                            orow.ItemArray = linexc.ItemArray;
                            ErrosNovasContas.Rows.Add(orow);
                            continue;
                        }
                        if (credito.Trim() == "" && debito.Trim() == "") continue;

                        string tdoc = "";
                        if (Convert.ToString(linexc["DOC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC"]).Trim();
                        else if (Convert.ToString(linexc["DOC_FISC"]).Trim() != "")
                            tdoc = Convert.ToString(linexc["DOC_FISC"]).Trim();
                        if (tdoc != "") tdoc = "DOC.n.:" + tdoc;

                        string thist = "";
                        if (Convert.ToString(linexc["HIST"]).Trim() != "")
                            thist = Convert.ToString(linexc["HIST"]).Trim();

                        // Mesmo auto-convert numerico de ExportaLoteExcel para paridade visual
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

                        ws.Cells[row, 5].Value = Convert.ToDecimal(linexc["VALOR"]);
                        ws.Cells[row, 5].Style.Numberformat.Format = "###,###,##0.00";

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
                    "Erro export FinVauche", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor.Current = Cursors.Default;
            }
        }












    }
}
