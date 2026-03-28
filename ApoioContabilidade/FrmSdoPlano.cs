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
using System.Data.OleDb;
using ApoioContabilidade.Models;
using ApoioContabilidade.Excel;
namespace ApoioContabilidade
{
    public partial class FrmSdoPlano : Form
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
        DataTable dtEntradas;

        DateTime dataconst;

        DataTable tabexcel;
        OleDbDataAdapter adrelaciona;
        DataSet dsrelaciona;
        DataTable dtMovSaldos;

        /*DataTable ContasUnicas;
        DataTable Balancinho;

        DataTable Balancete;
        */
        DataTable VauchesRazao;
        //  ///////////
        Dictionary<string, Relaciona> dictrelaciona;


        const int DATA_ger = 0;
        const int DEBITO_ger = 1;
        const int CREDITO_ger = 2;
        const int VALOR_ger = 3;
        const int HIST_ger = 4;
        const int DOC_ger = 5;

        DataTable ErrosNovasContas;

        public FrmSdoPlano()
        {
            InitializeComponent();


            dataconst = Convert.ToDateTime("31/12/2009").Date;
            recomece = true;
            oEntradas = new MonteGrid();
            MonteGrids();
           // dgEntradas.CaptionText = "Saldo Plano";
            dgEntradas.ReadOnly = true;
            //dgEntradas.AllowSorting = false;
            oEntradas.oDataGridView = dgEntradas;
            oEntradas.sbTotal = sbEntradas;
            oPlacon = TDataControlContab.Placon();

         
            oForm = new FormFiltro();
            bmSourceEntrada = new BindingSource();
            // bmSourceEntrada.BindingComplete += new BindingCompleteEventHandler(bmSource_BindingComplete);
          //  bmSourceEntrada.AddingNew += new AddingNewEventHandler(bmSource_AddingNew);
         
        }

        private void FrmSdoPlano_Load(object sender, EventArgs e)
        {
            int AnoGeral = 2023;
            inicio = Convert.ToDateTime("01/01/2023").Date;
            List<string> tabelas = TDataControlContab.EncontreTabelaPlacon();
            if (tabelas.IndexOf("PTPLA" + (AnoGeral - 1).ToString() + ".DBF") < 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + (AnoGeral - 1).ToString());
                return;
            }

            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string pla = "PTPLA" + (AnoGeral - 1).ToString() + ".DBF";
            setoroledb = "SELECT numconta, descricao, sdo, desc2, data FROM " + path + pla +
                       " WHERE (SUBS(NUMCONTA,6,3) <> '000')  and (sdo <> 0)  ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PTPLA" + (AnoGeral - 1).ToString());
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);

            if (odataset.Tables.Count == 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + (AnoGeral - 1).ToString());
                return;
            }

            DataTable dtEntradas = odataset.Tables[0].Copy();
            dtMovSaldos = new DataTable();
            dtMovSaldos.Columns.Add("DATA", typeof(DateTime));
            dtMovSaldos.Columns.Add("DEBITO", typeof(string));
            dtMovSaldos.Columns.Add("CREDITO", typeof(string));
            dtMovSaldos.Columns.Add("VALOR", typeof(decimal));
            dtMovSaldos.Columns.Add("HIST", typeof(string));
            dtMovSaldos.Columns.Add("NHIST", typeof(string));
            dtMovSaldos.Columns.Add("DOC", typeof(string));

            odataset.Dispose();
            oledbda.Dispose();
            DateTime dateTime = DateTime.Now;
            try
            {
                dateTime = (from valor1 in dtEntradas.AsEnumerable()
                            where (valor1.Field<string>("NUMCONTA").Trim().Substring(0,1) == "1")
                            select valor1.Field<DateTime>("Data")).Max();

            }
            catch (Exception)
            {


            }
            if (dateTime.AddDays(1) != inicio)
            {
                MessageBox.Show("ERRO: Saldos PLACON DEVEM SER ATUALIZADOS PARA O ANO FISCAL ANTERIOR A ESTA PESQUISA  ");
                return;
            }
            DataSet dsrelaciona = TDataControlContab.TabRelaciona();
            DataTable tabrelaciona = dsrelaciona.Tables[0];
            dictrelaciona = null;
            List<string> movGeralUnico =
              (from mov in dtEntradas.AsEnumerable()
               group mov by new { cred = mov.Field<String>("NUMCONTA").Trim() } into g
               select  
                  g.Key.cred.Trim()
               ).ToList();


            try
            {
                dictrelaciona = tabrelaciona.AsEnumerable().Where(row => !row.IsNull("NUMCONTA") &&
                (row.Field<string>("NUMCONTA").Trim() != "") &&
                movGeralUnico.Contains(row.Field<string>("NUMCONTA").Trim())).

                ToDictionary(
                 s => s.Field<string>("NUMCONTA").Trim(),
                  s => new Relaciona
                  {
                      novocod = s.Field<string>("NOVOCOD"),
                      numconta = s.Field<string>("NUMCONTA"),
                      descricao = s.Field<string>("DESCRICAO"),
                      reduzido = s.Field<int>("REDUZIDO")
                  }
                 );


            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;

                MessageBox.Show("RELACIONA.DBF tem NUMCONTA DUPLICADO, verifique!!!!!!!");
                return;

            }




            /*    DataSet dsPesquisa = new DataSet();
                   // dsPesquisa.Tables.Add(TDataControlContab.TabelaPlaconAno("2021").Tables["PTPLA2021"].Copy());
                    dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
                    dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());
                    oPesquisa = new PesquisaGenerico(dsPesquisa);


                //dtEntradas = TDataControlContab.dtPlaconAno("2021");
                dtEntradas = TDataControlContab.dtPlacon();
                DataView dvEntradas = dtEntradas.AsDataView();//oTables[0].AsDataView();
                ErrosNovasContas = dvEntradas.Table.Clone();
            */


            System.Nullable<decimal> total1 =
           (from valor1 in dtEntradas.AsEnumerable()
            where (valor1.Field<string>("NUMCONTA").Substring(0, 1) == "1")
            select valor1.Field<decimal>("SDO")).Sum();
         
            System.Nullable<decimal> total2 =
            (from valor1 in dtEntradas.AsEnumerable()
            where (valor1.Field<string>("NUMCONTA").Substring(0, 1) == "2")
            select valor1.Field<decimal>("SDO")).Sum();


            foreach(DataRow row in dtEntradas.Rows)
            {
                DataRow movRow = dtMovSaldos.NewRow();
                string numconta = row["NUMCONTA"].ToString();
                int alterdataNumConta = -1;
                Relaciona relnumconta = null;
                if (dictrelaciona.ContainsKey(numconta.Trim()))
                {
                    relnumconta = dictrelaciona[numconta.Trim()];
                    alterdataNumConta = relnumconta.reduzido;
                }
                
                if (row["NUMCONTA"].ToString().Substring(0, 1) == "1")
                {
                    if (Convert.ToDouble(row["SDO"]) > 0)
                    {
                        movRow["DEBITO"] = alterdataNumConta.ToString();
                        movRow["CREDITO"] = "20363";
                        movRow["VALOR"] = row["SDO"];
                    }
                    else
                    {
                        movRow["CREDITO"] = alterdataNumConta.ToString();
                        movRow["DEBITO"] = "20363";
                        movRow["VALOR"] = Convert.ToDouble(row["SDO"]) * -1;
                    }
                }
                else  // passiv
                {
                    if (Convert.ToDouble(row["SDO"]) > 0)  // inverte
                    {
                        movRow["DEBITO"] = alterdataNumConta.ToString();
                        movRow["CREDITO"] = "20363";
                        movRow["VALOR"] = row["SDO"];
                    }
                    else
                    {
                        movRow["CREDITO"] = alterdataNumConta.ToString();
                        movRow["DEBITO"] = "20363";
                        movRow["VALOR"] = Convert.ToDouble(row["SDO"]) * -1;
                    }
                   
                }
                
                movRow["HIST"] = "SALDO BALANCO N/DATA";
                movRow["DATA"] = inicio.AddDays(-1);
                dtMovSaldos.Rows.Add(movRow);
            }
            dtMovSaldos.AcceptChanges();

            bmSourceEntrada.DataSource = dtMovSaldos;
                dgEntradas.DataSource = bmSourceEntrada;

                oEntradas.ConfigureDBGridView();
            oEntradas.LinhasCampo[1].total = Convert.ToDouble(total1);
            oEntradas.LinhasCampo[2].total = Convert.ToDouble(total2);


            oEntradas.ColocaTotais();
              
                recomece = false;

            this.Refresh();
      
        }
        private void MonteGrids()
        {
            oEntradas.Clear();
           
            oEntradas.AddValores("DEBITO", "DEBITO", 8, "", false, 0, "");
           // oEntradas.AddValores("GRAU", "GRAU", 14, "", true, 0, "");
            oEntradas.AddValores("CREDITO", "CREDITO", 40, "", true   , 0, "");
            oEntradas.AddValores("DATA", "DATA", 0, "", false, 0, "");
            oEntradas.AddValores("VALOR", "VALOR", 12, "#,###,##0.00", true, 0, "");
            // oEntradas.AddValores("VAL1", "SALDO", 12, "#,###,##0.00", true, 0, "");
            // oEntradas.AddValores("DATA", "DATA", 0, "", false, 0, ""); 
            oEntradas.AddValores("HIST", "HIST", 45, "", false, 0, "");
            oEntradas.AddValores("CODHIST", "COD HIST", 5, "", false, 0, "");

        }
//        numconta, grau, descricao, sdo, ant_sdo, desc2, data, data_ant, val1, val2, val3, val4, indice, taxa, novocod, novadesc



       /* private ArmeEdicao ArmeEdicaoFiltroGenerico()
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
            olinha.ofuncaoSql = TDataControlReduzido.CompareValor;
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
        */
        // Rotinas para Exportar dados

        private string FormateParaNumConta(string conta,DataTable tabrelaciona)
        {
            try
            {
                string contanum = conta;
                string result = "";
                /*if (conta.Trim() == "") return "";
                if (conta.Trim().Length < 3)
                    if (TDataControlContab.EBanco(conta, true))
                    {
                        contanum = TDataControlContab.EBancoContab(conta);
                        for (int i = 0; i < tabrelaciona.Rows.Count; i++)
                        {
                            if (Convert.ToString(tabrelaciona.Rows[i]["NUMCONTA"]).Trim() == contanum)
                            {
                                result = Convert.ToString(tabrelaciona.Rows[i]["NOVOCOD"]).Substring(0, 9);
                                break;
                            }
                        }
                        return result;
                    }
                if (oPesquisa.ExisteValor("BANCOS","DESC2",conta)) return "";

                contanum = TDataControlContab.EPlanoRetornaNumConta(conta, oPesquisa);*/
                for (int i = 0; i < tabrelaciona.Rows.Count; i++)
                {
                    if (Convert.ToString(tabrelaciona.Rows[i]["NUMCONTA"]).Trim() == contanum)
                    {
                        result = Convert.ToString(tabrelaciona.Rows[i]["NOVOCOD"]).Substring(0, 9);
                        break;
                    }
                }
                if (result == "")
                {
                    result = "-1";
                    //MessageBox.Show("Conta Inexistente no NOVOCODIGO:" + conta);
                }
                return result;


            }
            catch (Exception)
            {
                MessageBox.Show("Erro 3");
                throw;
            }

        }

           private string FormateNumeroCodigo(string conta)
        {
            try
            {
                if ((conta.Length ==0) ||(conta.Length > 9) || (conta.Length == 7) || (conta.Length == 8) || (conta.Length == 5)) return "";
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
            if (dtMovSaldos == null) return;
            if (dtMovSaldos.Rows.Count == 0) return;
            Cursor.Current = Cursors.WaitCursor;
            ExportaSdoBalanco.ExportaExcelSdoBalanco(dtMovSaldos);
            Cursor.Current = Cursors.Default;
            /*saveFileDialog1.FileName = "SdoPlaconImportado.txt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tarquivo = saveFileDialog1.FileName;

                DataSet dsrelaciona = TDataControlContab.TabRelaciona();
                DataTable tabrelaciona = dsrelaciona.Tables[0];


                DataTable tabordenada = dtEntradas;//dataSet1.Tables[0];

                try
                {
                    StreamWriter sw = File.CreateText(tarquivo);
                    //string codant = "";
                    string debito;
                    string credito;
                    for (int i = 0; i < tabordenada.Rows.Count; i++)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                    {

                        DataRow linexc = tabordenada.Rows[i];// dsrelaciona.Tables["RELACIONA"].Rows[i];


                        if ((Convert.IsDBNull(linexc["NUMCONTA"])) || (Convert.ToDecimal(linexc["VAL1"]) == 0))
                            //&& ((Convert.IsDBNull(linexc["CREDITO"])) || (Convert.ToString(linexc["CREDITO"]).Trim() == "")))
                            continue;

                        string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(dataconst));// Date.ToString("d");//TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));
                        if (linexc["NUMCONTA"].ToString().Substring(0, 1) == "1") //
                        {
                            if (Convert.ToDecimal(linexc["VAL1"]) >= 0)
                            {
                                debito = FormateParaNumConta(Convert.ToString(linexc["NUMCONTA"]), tabrelaciona);
                                credito = "";
                            }
                            else
                            {
                                credito = FormateParaNumConta(Convert.ToString(linexc["NUMCONTA"]), tabrelaciona);
                                debito = "";
                            }
                        }
                        else if (linexc["NUMCONTA"].ToString().Substring(0, 1) == "2") //
                        {
                            if (Convert.ToDecimal(linexc["VAL1"]) >= 0)
                            {
                                credito = FormateParaNumConta(Convert.ToString(linexc["NUMCONTA"]), tabrelaciona);
                                debito = "";
                            }
                            else
                            {
                                debito = FormateParaNumConta(Convert.ToString(linexc["NUMCONTA"]), tabrelaciona);
                                credito = "";
                            }
                        }
                        else
                            continue;

                        if ((debito == "-1") || (credito == "-1"))
                        {
                            DataRow orow = ErrosNovasContas.NewRow();
                            orow.ItemArray = linexc.ItemArray;
                            ErrosNovasContas.Rows.Add(orow);
                            continue;
                        }
                        string linha = "";
                         debito = FormateNumeroCodigo(debito);
                         credito = FormateNumeroCodigo(credito);
                        if ((credito.Trim() == "") && (debito.Trim()=="")) continue;
                        linha = TDataControlReduzido.preenchaespaco(data, 8);
                        linha += TDataControlReduzido.preenchaespaco("", 5);
                        linha += TDataControlReduzido.preenchaespaco(debito, 14);
                        linha += TDataControlReduzido.preenchaespaco("", 3);
                        linha += TDataControlReduzido.preenchaespaco(credito,14);
                        linha += TDataControlReduzido.preenchaespaco("", 3);
                        decimal tsdo = Convert.ToDecimal(linexc["VAL1"]);
                        if (tsdo < 0)
                            tsdo = tsdo * -1;
                        linha += TDataControlReduzido.FormatNumero(Convert.ToSingle(tsdo), 14);
                        linha += TDataControlReduzido.preenchaespaco("", 3);
                       // string tdoc = "";
                        string thist = "TRANSF.SDO BALANCO N/DATA";
                        //if (Convert.ToString(linexc["HIST"]).Trim() != "")
                          //  linha += TDataControlReduzido.preenchaespaco(Convert.ToString(linexc["HIST"]).Trim() + " " + tdoc, 50);
                       // else
                            linha += TDataControlReduzido.preenchaespaco(thist, 50);

                        sw.WriteLine(linha);

                    }
                    sw.Close();
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
       
    }
}
