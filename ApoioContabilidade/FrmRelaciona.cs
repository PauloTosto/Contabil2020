using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using ClassFiltroEdite;
using ClassConexao;
using System.Data.OleDb;
//using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;

using ApoioContabilidade.Models;
using ApoioContabilidade.Excel;
using ApoioContabilidade.ZContabAlterData;
using static ApoioContabilidade.ZContabAlterData.Utils.BalanceteClass;
using Microsoft.Office.Interop.Excel;

namespace ApoioContabilidade
{

    public partial class FrmRelaciona : Form
    {

        public DataTable VauchesRazao;
        private MonteGrid oEntradas;

        public BindingSource bmSourceEntrada;
        public PesquisaGenerico oPesquisa;

        private DateTime inicio, fim;
        private List<LinhaSolucao> oList;// Lista solucao para pesquisa induzida
        DataSet dsPesquisa;
       
        private FormFiltro oForm;

        //private DataSet dsBancos; 
        // public BindingSource bmSourceSaida;
        DataView oPlacon;

        DataSet dataSet1;
        DataTable errosPesquisa;
        List<string> tabelasDBF;
        List<int> Anos;

        // VAriaveis da importação do RELACIONA.DBF
        DataTable tabexcel;
        OleDbDataAdapter adrelaciona;
        DataSet dsrelaciona;

        DataTable ContasUnicas;
        DataTable Balancinho;

        DataTable Balancete;


        //  ///////////
        Dictionary<string, Relaciona> dictrelaciona;


        int AnoGeral = 0;

        public FrmRelaciona()
        {
            InitializeComponent();
            btnUnicas.Enabled = false;
            btnBalancinho.Enabled = false;
            btBalancete.Enabled = false;
            groupBox1.Enabled = false;
            chkExcluiDebCre.Enabled = false;
            Anos = new List<int>();
            oEntradas = new MonteGrid();
            MonteGrids();
            dgEntradas.ReadOnly = true;
            oEntradas.oDataGridView = dgEntradas;
            // encontrar Tabelas com o PADRAO PTMOVFIN<ano>.dbf no diretorio padrao(CONTAB)
            oPlacon = TDataControlContab.Placon();
            InicializaAno();
            /*tabelasDBF = TDataControlContab.EncontreTabela();
            foreach( string tabela in tabelasDBF)
            {
                int pos = tabela.IndexOf("PTMOVFIN");
                if (tabela.Contains("PTMOVFIN") && (pos == 0))
                {
                    
                    try {
                        string anostr = tabela.Substring(8, 4);
                        int ano = Convert.ToInt32(anostr);
                        if (tabela.Length != 16) continue; // padrao 
                        Anos.Add(ano);
                    } catch (Exception) { }
                    
                }
            }
            if (Anos.Count > 0)
            {
                foreach(int ano in Anos)
                {
                    comboBox1.Items.Add(ano.ToString());
                }
                comboBox1.SelectedIndex = -1;


            }
            else
            {
                MessageBox.Show("É necessario PTMOVFIN<ano>.DBF");
                return;
            }

            ContasUnicas = new DataTable();
            ContasUnicas.Columns.Add("NUMCONTA", Type.GetType("System.String"));

            oPlacon = TDataControlContab.Placon();

            oForm = new FormFiltro();
            oForm.dtData1.Value = new DateTime(2019, 1, 1);
            oForm.dtData2.Value = new DateTime(2019, 1, 31);
            bmSourceEntrada = new BindingSource();
            */
        }
        private void InicializaAno()
        {
            tabelasDBF = TDataControlContab.EncontreTabela();
            foreach (string tabela in tabelasDBF)
            {
                int pos = tabela.IndexOf("PTMOVFIN");
                if (tabela.Contains("PTMOVFIN") && (pos == 0))
                {

                    try
                    {
                        string anostr = tabela.Substring(8, 4);
                        int ano = Convert.ToInt32(anostr);
                        if (tabela.Length != 16) continue; // padrao 
                        Anos.Add(ano);
                    }
                    catch (Exception) { }

                }
            }
            if (Anos.Count > 0)
            {
                foreach (int ano in Anos)
                {
                    comboBox1.Items.Add(ano.ToString());
                }
                comboBox1.SelectedIndex = -1;


            }
            else
            {
                MessageBox.Show("É necessario PTMOVFIN<ano>.DBF");
                return;
            }

            ContasUnicas = new DataTable();
            ContasUnicas.Columns.Add("NUMCONTA", Type.GetType("System.String"));

            Balancinho = new DataTable();
            Balancinho.TableName = "BALANCINHO";
            Balancinho.Columns.Add("NUMCONTA", Type.GetType("System.String"));
            Balancinho.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            Balancinho.Columns.Add("DEBITOVLR", Type.GetType("System.Decimal"));
            Balancinho.Columns.Add("CREDITOVLR", Type.GetType("System.Decimal"));
            Balancinho.Columns.Add("SDOVLR", Type.GetType("System.Decimal"));
            Balancinho.Columns.Add("SDOANTVLR", Type.GetType("System.Decimal"));


            oForm = new FormFiltro();
            oForm.dtData1.Value = new DateTime(2019, 1, 1);
            oForm.dtData2.Value = new DateTime(2019, 12, 31);
            bmSourceEntrada = new BindingSource();
        }
        
        
        
        
        
        
        private void Reload()
        {
            //   if (recomece)
            // {

            

            btnImporta.Enabled = false;
            btnUnicas.Enabled = false;
            btnBalancinho.Enabled = false;
            groupBox1.Enabled = false;
            chkExcluiDebCre.Enabled = false;
            btBalancete.Enabled = false;


            int ano = Convert.ToInt32(comboBox1.SelectedItem);
            try
            {
                if ((ano != 0) && (ano != AnoGeral))
                {
                    AnoGeral = ano;
                    if (comboBox1.Enabled)
                    {
                        oForm.dtData1.Value = new DateTime(ano, 1, 1);
                        oForm.dtData2.Value = new DateTime(ano, 12, 31);
                        oForm.dtData1.MinDate = new DateTime(ano, 1, 1);
                        oForm.dtData1.MaxDate = new DateTime(ano, 12, 31);
                        oForm.dtData2.MinDate = new DateTime(ano, 1, 1);
                        oForm.dtData2.MaxDate = new DateTime(ano, 12, 31);
                    }
                }
            }
            catch (Exception)
            {
                
                this.Close();
                return;
                
            }
            
            oForm.oArme = ArmeEdicaoFiltroGenerico();
            btnExportaLote.Enabled = false;
            btnUnicas.Enabled = false;
            
            oForm.ShowDialog();
            if (oForm.oPesqFin.TabCount > 0)
                oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
            else
                oList = null;
            inicio = oForm.dtData1.Value;
            fim = oForm.dtData2.Value;
            comboBox1.Enabled = false;

            dsPesquisa = new DataSet();
            dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
            dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());
            oPesquisa = new PesquisaGenerico(dsPesquisa);
            string tabela = "PTMOVFIN" + ano.ToString(); ;
            try
            {
                dataSet1 = TDataControlContab.MovFinanVauches_Relaciona(tabela, inicio, fim, oList, oPesquisa);
            }
            catch (Exception E)
            {
                btnImporta.Enabled = true;
                MessageBox.Show("Não Foi Encontrado o " + tabela+ " "+E.Message);
                return;
            }
           
            // procedimeno para aglutinar registro
            errosPesquisa = dataSet1.Tables[0].Clone();
            errosPesquisa.Columns.Add("ObsErro", Type.GetType("System.String"));
            //var altereTabela =  dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<int>("MOV_ID") != 0) && row.Field<int>("OUTRO_ID") == 0)  )
            DataTableCollection oTables = dataSet1.Tables;
            DataView dvEntradas = oTables[0].AsDataView();
            VauchesRazao = oTables[0].Clone();
            PesquiseRazao();
            // VauchesRazao

            bmSourceEntrada.DataSource = VauchesRazao.AsDataView();//   dvEntradas;
            ///bmSourceSaida.DataSource = dvSaidas;
            dgEntradas.DataSource = bmSourceEntrada;

            //oEntradas.ConfigureDBGrid();
            oEntradas.sbTotal = sbRazao;
            oEntradas.ConfigureDBGridView();
            System.Nullable<decimal> total1 =
              (from valor1 in VauchesRazao.AsEnumerable()
               where (valor1.Field<string>("DEBITO").Trim() != "")
// && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("DEBITO")))
               select valor1.Field<decimal>("VALOR")).Sum();
            oEntradas.LinhasCampo[1].total = Convert.ToDouble(total1);

            System.Nullable<decimal> total2 =
      (from valor1 in VauchesRazao.AsEnumerable()
       where (valor1.Field<string>("CREDITO").Trim() != "")
// && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("CREDITO")))
       select valor1.Field<decimal>("VALOR")).Sum();
            oEntradas.LinhasCampo[2].total = Convert.ToDouble(total2);

            oEntradas.ColocaTotais();

            
            if (VauchesRazao.Rows.Count > 0)
                btnExportaLote.Enabled = true;
            //    }
            //  this.Refresh();

            btnImporta.Enabled = true;
            comboBox1.Enabled = true;
            groupBox1.Enabled = true;
            chkExcluiDebCre.Enabled = true;
        }

        private void PesquiseRazao()
        {
            DataSet dsrelaciona = TDataControlContab.TabRelaciona();
            DataTable tabrelaciona = dsrelaciona.Tables[0];

            if ((dataSet1 == null) || (dataSet1.Tables.Count == 0))
            {
               
                return;
            }

            Cursor.Current = Cursors.WaitCursor;

            DataTable tabordenada = dataSet1.Tables[0];

            List<string> movimCredito =
              (from mov in tabordenada.AsEnumerable()
               group mov by new { cred = mov.Field<String>("CREDITO").Trim() } into g
               select  //LinhaGuiaQuadra
                  g.Key.cred.Trim()
               ).ToList();

            List<string> movimDebito =
              (from mov in tabordenada.AsEnumerable().Where(row => !movimCredito.Contains(row.Field<string>("DEBITO").Trim()))
               group mov by new { deb = mov.Field<String>("DEBITO") } into g
               select
                   g.Key.deb.Trim()).ToList();
            foreach(string conta in movimDebito)
            {
                movimCredito.Add(conta);
            }

            List<string> movGeralUnico = movimCredito.ToArray().Distinct().ToList();
           
            /*  List<Relaciona> relacionamento = (
                        tabrelaciona.AsEnumerable().Where(row => !row.IsNull("NUMCONTA") && movimCredito.Contains(row.Field<string>("NUMCONTA").Trim())).Select(s =>
                       new Relaciona {
                            codnovo = s.Field<String>("CODNOVO").Trim(),
                            numconta = s.Field<String>("NUMCONTA").Trim(),
                            descricao = s.Field<String>("DESCRICAO").Trim(),
                            reduzido = s.Field<int>("REDUZIDO")

                       })).ToList();*/
           dictrelaciona = null;
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

            ContasUnicas.Rows.Clear();
            Balancinho.Rows.Clear();
            foreach (string numconta in movGeralUnico)
            {
                DataRow balancinho = Balancinho.NewRow();
                balancinho["NUMCONTA"] = numconta;
                balancinho["DEBITOVLR"] = 0;
                balancinho["CREDITOVLR"] = 0;
                balancinho["SDOVLR"] = 0;
                balancinho["SDOANTVLR"] = 0;
                Balancinho.Rows.Add(balancinho);

                DataRow tab = null;
                try
                {
                    tab = tabrelaciona.AsEnumerable().Where(row => row.Field<string>("NUMCONTA").Trim() == numconta.Trim()).FirstOrDefault();
                }
                catch (Exception)
                {

                  
                }
                if (tab != null) continue; 
               // if (dictrelaciona.ContainsKey(numconta.Trim())) continue;

                DataRow unica = ContasUnicas.NewRow();
                unica["NUMCONTA"] = numconta;
                ContasUnicas.Rows.Add(unica);
               
            }

            btnUnicas.Enabled = true;

            try
            {
                VauchesRazao.Rows.Clear();
                //string codant = "";
                for (int i = 0; i < tabordenada.Rows.Count; i++)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                {

                    DataRow linexc = tabordenada.Rows[i];// dsrelaciona.Tables["RELACIONA"].Rows[i];


                    if (((Convert.IsDBNull(linexc["DEBITO"])) || (Convert.ToString(linexc["DEBITO"]).Trim() == ""))
                        && ((Convert.IsDBNull(linexc["CREDITO"])) || (Convert.ToString(linexc["CREDITO"]).Trim() == "")))
                        continue;

                    string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));
                    //string debito = FormateParaNumConta(Convert.ToString(linexc["DEBITO"]), tabrelaciona);
                    //string credito = FormateParaNumConta(Convert.ToString(linexc["CREDITO"]), tabrelaciona);
                    string debito = "";
                    string credito = "";
                    debito = Convert.ToString(linexc["DEBITO"]).Trim();
                    credito = Convert.ToString(linexc["CREDITO"]).Trim();
                    
                    // QUANDO O DEBITO == CREDITO RETIREI EM DEZEMBRO 2022
                    //if (debito == credito) continue;
                    
                    if (chkExcluiDebCre.Checked)
                    {
                        if (debito == credito) continue;
                    }


                    if (Convert.ToDecimal(linexc["VALOR"]) == 0) continue;
                    DataRow rowDebito = Balancinho.AsEnumerable().Where(row => row.Field<string>("NUMCONTA").Trim() == debito).FirstOrDefault();
                    rowDebito.BeginEdit();
                    rowDebito["DEBITOVLR"] = Convert.ToDecimal(rowDebito["DEBITOVLR"]) + Convert.ToDecimal(linexc["VALOR"]);
                    rowDebito.EndEdit();
                    rowDebito.AcceptChanges();
                    DataRow rowCredito = Balancinho.AsEnumerable().Where(row => row.Field<string>("NUMCONTA").Trim() == credito).FirstOrDefault();
                    rowCredito.BeginEdit();
                    rowCredito["CREDITOVLR"] = Convert.ToDecimal(rowCredito["CREDITOVLR"]) + Convert.ToDecimal(linexc["VALOR"]);
                    rowCredito.EndEdit();
                    rowCredito.AcceptChanges();

                    if (rbNumconta.Checked)
                    {
                    }
                    else if ((rbDesc2.Checked) || (rbContabilidade.Checked))
                    {
                        debito = "-1";
                        credito = "-1";

                        Relaciona reldebito = null;
                        Relaciona relcredito = null;
                        if (dictrelaciona.ContainsKey(linexc["DEBITO"].ToString().Trim())) {
                            reldebito = dictrelaciona[linexc["DEBITO"].ToString().Trim()];             
                        }
                        if (dictrelaciona.ContainsKey(linexc["CREDITO"].ToString().Trim()))
                        {
                            relcredito = dictrelaciona[linexc["CREDITO"].ToString().Trim()];
                        }
                        
                        if (rbDesc2.Checked) {
                            debito = reldebito != null ? reldebito.reduzido.ToString() : debito;
                            credito = relcredito != null ? relcredito.reduzido.ToString() : credito;
                        } else
                        {
                            debito = reldebito != null ? reldebito.novocod.ToString() : debito;
                            credito = relcredito != null ? relcredito.novocod.ToString() : credito;
                        }
                       
                    }
                       

                    //if ((debito == "-1") || (credito == "-1"))
                    //{
                    //  DataRow orow = ErrosNovasContas.NewRow();
                    // orow.ItemArray = linexc.ItemArray;
                    // ErrosNovasContas.Rows.Add(orow);
                    //   continue;
                    // }
                    DataRow razaoRow = VauchesRazao.NewRow();
                    razaoRow.ItemArray = linexc.ItemArray;
                    razaoRow["DEBITO"] = debito;
                    razaoRow["CREDITO"] = credito;
                    if ((razaoRow["HIST"].ToString().Trim() == "") && (razaoRow["FORN"].ToString().Trim() != "")) {
                        razaoRow["HIST"] = razaoRow["FORN"].ToString();
                    }
                    VauchesRazao.Rows.Add(razaoRow);
                    

                }
                btnBalancinho.Enabled = true;
                if (inicio.Month == 1)
                {
                    btBalancete.Enabled = true;
                }
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                throw;
            }
            Cursor.Current = Cursors.Default;
        }
      /*  private string FormateParaAlterData(string conta, string campo, DataTable tabrelaciona)
        {
            try
            {
                string result = "";
                DataRow retorno = null;
                try
                {
                    retorno = tabrelaciona.AsEnumerable().Where(row => row.Field<string>("NUMCONTA").Trim() == conta.Trim()).FirstOrDefault();
                }
                catch (Exception)
                {
                }
             
                if (retorno != null)
                {
                    result = retorno[campo].ToString();
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

        */


        /* private string FormateParaNumContaNormal(string conta)
         {
             try
             {
                 string contanum = "";
                 string result = "";
                 if (conta.Trim() == "") return "";
                 if (conta.Trim().Length < 3)
                     if (EBancoContabil(conta))
                     {
                         contanum = NBanco_Contab(conta);

                         result = contanum;
                         return result;
                     }
                 // Tirei porque os bancos serão descrito pelo DESC2 (incluidos após transformação sofridas pelas TRANSFERÊNCIAS
                 // if (oPesquisa.ExisteValor("BANCOS", "DESC2", conta)) return "";

                 contanum = TDataControlContab.EPlanoRetornaNumConta(conta, oPesquisa);

                 result = contanum;
                 return result;
             }
             catch (Exception E)
             {
                 MessageBox.Show("Erro 3: " + E.Message);
                 throw;
             }
         }*/

        private void MonteGrids()
        {
            oEntradas.Clear();
            oEntradas.AddValores("DATA", "DATA", 0, "", false, 0, "");
            oEntradas.AddValores("DEBITO", "DEBITO", 25, "", true, 0, "");
            oEntradas.AddValores("CREDITO", "CREDITO", 25, "", true, 0, "");
            oEntradas.AddValores("VALOR", "VALOR", 12, "#,###,##0.00", true, 0, "");
            oEntradas.AddValores("HIST", "HISTORICO", 40, "", false, 0, "");
            oEntradas.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oEntradas.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oEntradas.AddValores("MOV_ID", "Mov_ID", 10, "", false, 0, "");
            oEntradas.AddValores("OUTRO_ID", "OUTRO_ID", 10, "", false, 0, "");
            oEntradas.AddValores("NOVO_ID", "OUTRO_ID", 10, "", false, 0, "");

        }

        private void rbNumconta_Click(object sender, EventArgs e)
        {
            // normal
            PesquiseRazao();
        }

        private void rbDesc2_Click(object sender, EventArgs e)
        {
            // reduzido
            PesquiseRazao();
        }

        private void rbContabilidade_Click(object sender, EventArgs e)
        {
            // numero plano alter data
            PesquiseRazao();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Reload();
            
        }

        private void btnExporta_Click(object sender, EventArgs e)
        {
            if (!rbDesc2.Checked)
            {
                bool naoePadrao = (MessageBox.Show("O Padrão é exportar com Codigo REDUZIDO.Exporta Assim Mesmo?", "Responda", MessageBoxButtons.OKCancel) == DialogResult.OK);
                if (!naoePadrao) return;
            }
            Cursor.Current = Cursors.WaitCursor;
            ExportaLoteExcel.ExportaExcelLote(VauchesRazao);
            Cursor.Current = Cursors.Default;
        }

        
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnPesquisa.Enabled = true;
            groupBox1.Enabled = true;
            chkExcluiDebCre.Enabled = true;
        }

     
        private ArmeEdicao ArmeEdicaoFiltroGenerico()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha = new Linha("DEBITO/CREDITO");//("CTAFIN");
            olinha.cabecalho[0] = "DEBITO/CREDITO";
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO");
            olinha.cabecalho[0] = "DEBITO";
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("CREDITO");
            olinha.cabecalho[0] = "CREDITO";
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);



            olinha = new Linha("HIST");
            olinha.cabecalho[0] = "Historico";
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("FORN");
            olinha.cabecalho[0] = "Titular";
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DOC_FISC");
            olinha.cabecalho[0] = "Doc.Fiscal";
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
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
            olinha.oedite[0] = new System.Windows.Forms.TextBox();
            ((System.Windows.Forms.TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);


            return oArme;

        }
// IMPORTA ATRAVÉS DO EXCEL OS DADOS DE RALIONAMENTO ENTRE NUMCONTA E ALTERDATE E RECRIA O RELACIONA.DBF
        private void btnImporta_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    Cursor.Current = Cursors.WaitCursor;
                    string nomeplanilha = TDataControlReduzido.ExcelNomePlanilha(openFileDialog1.FileName);
                    tabexcel = TDataControlReduzido.LeiaExcel(openFileDialog1.FileName, nomeplanilha);
                    Form oform = new Form();
                    DataGrid oview = new DataGrid();
                    if (nomeplanilha.Trim().ToUpper() != "PESQUISA")
                    {
                        tabexcel.AsEnumerable().ToList().ForEach(
                                            row =>
                                            {
                                                row.BeginEdit();
                                                row["NOVADESC"] = row["NOVADESC"].ToString().Substring(1);
                                                row.EndEdit();
                                            });
                        tabexcel.AcceptChanges();
                    }
                    // oview
                    bool mostraRelaciona = false;
                    if (nomeplanilha.Trim().ToUpper() == "PESQUISA") // Dentro do arquivo excel tem que ter a planilha com o nome PESQUISA para gravar o relaciona definitivo 
                    {

                        mostraRelaciona = (MessageBox.Show("Cria novo RELACIONA.DBF sobreescrevendo atual.Importa os dados com base na planilha EXCEL <PESQUISA>?", "Responda", MessageBoxButtons.OKCancel) == DialogResult.OK);
                        bool retorno = ImportaRelaciona_Excel();
                        if (!retorno)
                        {
                            MessageBox.Show("Problemas com a importação!Verifique campos necessários no Excel!.");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("O nome da Pasta pode ser qualquer um. O Nome da PLANILHA tem que ser PESQUISA");
                        return;
                    }
                    if (mostraRelaciona)
                    {
                        oview.DataSource = dsrelaciona.Tables["RELACIONA"];
                        oform.Text = "RELACIONA => Criado pelo EXCEL";

                    }
                    else
                    {
                        oview.DataSource = tabexcel;
                        oform.Text = "EXCEL DIRETO";
                    }
                    // for (int i = 0;i<oview.TableStyles[0].GridColumnStyles.Count;i++)
                    //    oview.TableStyles[0].GridColumnStyles[i].NullText = ""; 
                    oview.Parent = oform;
                    oview.Dock = DockStyle.Fill;
                    oform.Show();

                }
                catch (Exception)
                {
                    Cursor.Current = Cursors.Default;
                    throw;
                }
                finally
                {
                    Cursor.Current = Cursors.Default;
                }
            }
        }

        private void btnContasUnicas_Click_1(object sender, EventArgs e)
        {
            FrmErros frmErros = new FrmErros(ContasUnicas);
            frmErros.Show();
        }

        private void btnBalancinho_Click(object sender, EventArgs e)
        {
            if (Balancinho.Rows.Count == 0) return;



            foreach (DataRow orow in Balancinho.Rows)
            {
                string numconta = orow["NUMCONTA"].ToString();
                Relaciona dict = null; ;
                if (dictrelaciona.ContainsKey(numconta))
                {
                    dict = dictrelaciona[numconta];
                }
                orow.BeginEdit();
                orow["SDOVLR"] = Convert.ToDecimal(orow["DEBITOVLR"]) - Convert.ToDecimal(orow["CREDITOVLR"]);
                if (dict != null)
                {
                    orow["DESCRICAO"] = dict.descricao;
                }
                orow.EndEdit();
                orow.AcceptChanges();
            }
            FrmBalancinho balancinho = new FrmBalancinho(Balancinho);
            balancinho.Show();
        }

        private void btBalancete_Click(object sender, EventArgs e)
        {

            if (Balancinho.Rows.Count == 0) return;

            List<string> tabelas = TDataControlContab.EncontreTabelaPlacon();
            if (tabelas.IndexOf("PTPLA" + (AnoGeral - 1).ToString()+".DBF") < 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + (AnoGeral - 1).ToString());
                return;
            }

            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string pla = "PTPLA" + (AnoGeral - 1).ToString() + ".DBF";
            setoroledb = "SELECT numconta, descricao, sdo, desc2, data FROM " + path + pla +  
                       " ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PTPLA"+ (AnoGeral - 1).ToString());
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            
            if (odataset.Tables.Count == 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + (AnoGeral - 1).ToString());
                return;
            }
           
            DataTable result = odataset.Tables[0].Copy();
            odataset.Dispose();
            oledbda.Dispose();
            DateTime dateTime = DateTime.Now;
            try
            {
                dateTime = (from valor1 in result.AsEnumerable()
                                     where (valor1.Field<string>("NUMCONTA").Trim() == "10000000")
                                     select valor1.Field<DateTime>("Data")).Max();

            }
            catch (Exception)
            {

                
            }
            if (dateTime.AddDays(1) != inicio)
            {
                MessageBox.Show("ERRO: Saldos PLACON DEVEM SER ATUALIZADOS PARA O ANO FISCAL ANTERIOR A ESTA PESQUISA  " );
                return;
            }


            /* Balancete = new DataTable();
             Balancete.TableName = "BALANCETE";
             Balancete.Columns.Add("NUMCONTA", Type.GetType("System.String"));
             Balancete.Columns.Add("GRAU", Type.GetType("System.String"));
             Balancete.Columns.Add("DESCRICAO", Type.GetType("System.String"));
             Balancete.Columns.Add("DESCBAL", Type.GetType("System.String"));
             Balancete.Columns.Add("DESC2", Type.GetType("System.String"));
             Balancete.Columns.Add("SDOANT", Type.GetType("System.Decimal"));
             Balancete.Columns.Add("SDOANTCALC", Type.GetType("System.Decimal"));
             Balancete.Columns.Add("DEBITO", Type.GetType("System.Decimal"));
             Balancete.Columns.Add("CREDITO", Type.GetType("System.Decimal"));
             Balancete.Columns.Add("SDO", Type.GetType("System.Decimal"));
             Balancete.Columns.Add("Inicio", Type.GetType("System.DateTime"));
             Balancete.Columns.Add("Fim", Type.GetType("System.DateTime"));
             try
             {

                 foreach (DataRowView dataRow in result.AsDataView())
                 {

                     if (dataRow["NUMCONTA"].ToString() == "00000000") continue;

                     DataRow orowB = Balancete.NewRow();
                     orowB["NUMCONTA"] = dataRow["NUMCONTA"];
                     orowB["DESCRICAO"] = dataRow["DESCRICAO"];
                     if (dataRow["NUMCONTA"].ToString().Substring(1, 7) == "0000000") {
                         orowB["DESCBAL"] =
                             dataRow["NUMCONTA"].ToString().Substring(0, 1) + "  " + dataRow["DESCRICAO"].ToString();
                     }
                     else if (dataRow["NUMCONTA"].ToString().Substring(2, 6) == "000000")
                     {
                         orowB["DESCBAL"] = dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                          dataRow["NUMCONTA"].ToString().Substring(1, 1) 
                          + "  " + dataRow["DESCRICAO"].ToString();
                     }
                     else if (dataRow["NUMCONTA"].ToString().Substring(3, 5) == "00000")
                     {
                         orowB["DESCBAL"] = dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                          dataRow["NUMCONTA"].ToString().Substring(1, 1)
                          + "." +
                         dataRow["NUMCONTA"].ToString().Substring(2, 1)
                          + "  " + dataRow["DESCRICAO"].ToString();
                     }
                     else if (dataRow["NUMCONTA"].ToString().Substring(5, 3) == "000")
                     {
                         orowB["DESCBAL"] = dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                          dataRow["NUMCONTA"].ToString().Substring(1, 1)
                          + "." +
                          dataRow["NUMCONTA"].ToString().Substring(2, 1) + "." +
                          dataRow["NUMCONTA"].ToString().Substring(3, 2) 
                          + "  " + dataRow["DESCRICAO"].ToString();
                     }

                     else
                     { 
                     orowB["DESCBAL"] =
                         dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                         dataRow["NUMCONTA"].ToString().Substring(1, 1) + "." +
                         dataRow["NUMCONTA"].ToString().Substring(2, 1) + "." +
                          dataRow["NUMCONTA"].ToString().Substring(3, 2) + "." +
                          dataRow["NUMCONTA"].ToString().Substring(5, 3)
                         + "  " + dataRow["DESCRICAO"].ToString(); }
                     orowB["DESC2"] = dataRow["DESC2"];
                     orowB["SDOANT"] = dataRow["SDO"];
                     orowB["SDOANTCALC"] = 0;
                     orowB["SDO"] = 0;
                     orowB["DEBITO"] = 0;
                     orowB["CREDITO"] = 0;
                     orowB["Inicio"] = inicio;
                     orowB["Fim"] = fim;

                     if (dataRow["NUMCONTA"].ToString().Substring(5, 3) != "000")
                     {
                         decimal debito = (from linha in Balancinho.AsEnumerable()
                                          where (linha.Field<string>("NUMCONTA").Trim() == dataRow["NUMCONTA"].ToString())
                                         // && ((linha.Field<DateTime>("DATA").CompareTo(dateTime) >= 0))
                                          select linha.Field<decimal>("DEBITOVLR")).Sum();

                         decimal credito = (from linha in Balancinho.AsEnumerable()
                                           where (linha.Field<string>("NUMCONTA").Trim() == dataRow["NUMCONTA"].ToString())
                                          // && ((linha.Field<DateTime>("DATA").CompareTo(dateTime) >= 0))
                                           select linha.Field<decimal>("CREDITOVLR")).Sum();
                         orowB["DEBITO"] = debito;
                         orowB["CREDITO"] = credito;
                         orowB["SDO"] = Convert.ToDecimal(orowB["SDOANT"]) + debito - credito;       }
                     Balancete.Rows.Add(orowB);
                     orowB.AcceptChanges();
                 }

                 Balancete.AcceptChanges();
             }
             catch (Exception E)
             {

                 MessageBox.Show(E.Message);
             }


             List<BalancoCampos> quartoGrau = (from soma in Balancete.AsEnumerable().Where(a => (a.Field<string>("NUMCONTA").Substring(5,3) != "000")

                             )
                                          group soma by new
                                          {
                                              numConta = soma.Field<string>("NUMCONTA").Substring(0,5),

                                              // icodser = soma.ICodSer
                                          } into g
                                          select new BalancoCampos
                                          {
                                              numconta = g.Key.numConta, 

                                              sdoAnt = g.Sum(p => p.Field<decimal>("SDOANT")),
                                              debito = g.Sum(p => p.Field<decimal>("DEBITO")),
                                              credito = g.Sum(p => p.Field<decimal>("CREDITO")),

                                          }).ToList();



               List<BalancoCampos> terceiroGrau = (from soma in quartoGrau.AsEnumerable()


                                               group soma by new
                                               {
                                                   numConta = soma.numconta.Substring(0, 3),

                                                   // icodser = soma.ICodSer
                                               } into g
                                               select new BalancoCampos
                                               {
                                                   numconta = g.Key.numConta,

                                                   sdoAnt = g.Sum(p => p.sdoAnt),
                                                   debito = g.Sum(p => p.debito),
                                                   credito = g.Sum(p => p.credito),

                                               }).ToList();


             List<BalancoCampos> segundoGrau = (from soma in terceiroGrau.AsEnumerable()


                                                 group soma by new
                                                 {
                                                     numConta = soma.numconta.Substring(0, 2),

                                                     // icodser = soma.ICodSer
                                                 } into g
                                                 select new BalancoCampos
                                                 {
                                                     numconta = g.Key.numConta,

                                                     sdoAnt = g.Sum(p => p.sdoAnt),
                                                     debito = g.Sum(p => p.debito),
                                                     credito = g.Sum(p => p.credito),

                                                 }).ToList();

               List<BalancoCampos> primeiroGrau = (from soma in segundoGrau.AsEnumerable()


                                                group soma by new
                                                {
                                                    numConta = soma.numconta.Substring(0, 1),

                                                    // icodser = soma.ICodSer
                                                } into g
                                                select new BalancoCampos
                                                {
                                                    numconta = g.Key.numConta,

                                                    sdoAnt = g.Sum(p => p.sdoAnt),
                                                    debito = g.Sum(p => p.debito),
                                                    credito = g.Sum(p => p.credito),

                                                }).ToList();

             foreach(var g in quartoGrau)
             {
                 DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "000").FirstOrDefault(); 
                 if (orow == null)
                 { continue; }
                 orow.BeginEdit();
                 orow["DEBITO"] = g.debito;
                 orow["CREDITO"] = g.credito;
                 orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                 orow.EndEdit();
                 orow.AcceptChanges();
             }
             foreach (var g in terceiroGrau)
             {
                 DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "00000").FirstOrDefault();
                 if (orow == null)
                 { continue; }
                 orow.BeginEdit();
                 orow["DEBITO"] = g.debito;
                 orow["CREDITO"] = g.credito;
                 orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                 orow.EndEdit();
                 orow.AcceptChanges();
             }
             foreach (var g in segundoGrau)
             {
                 DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "000000").FirstOrDefault();
                 if (orow == null)
                 { continue; }
                 orow.BeginEdit();
                 orow["DEBITO"] = g.debito;
                 orow["CREDITO"] = g.credito;
                 orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                 orow.EndEdit();
                 orow.AcceptChanges();
             }
             foreach (var g in primeiroGrau)
             {
                 DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "0000000").FirstOrDefault();
                 if (orow == null)
                 { continue; }
                 orow.BeginEdit();
                 orow["DEBITO"] = g.debito;
                 orow["CREDITO"] = g.credito;
                 orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                 orow.EndEdit();
                 orow.AcceptChanges();
             }


             // Atualizar as contas sinteticas



             ,Balancete, Balancinho
             */
            FrmBalancete balancete = new FrmBalancete(result,VauchesRazao ,inicio,fim);
            balancete.dictrelaciona = dictrelaciona;
            balancete.Show();


        }

        private void btnFechamento_Click(object sender, EventArgs e)
        {
       /*    var existe = VauchesRazao.AsEnumerable().Where(a => (a.Field<string>("DOC").Trim() == "SIST_BAL") 
           && (  (a.Field<string>("DEBITO").Trim() == "21110090")
             || (a.Field<string>("CREDITO").Trim() == "21110090") )
           ).FirstOrDefault(); 
           if (existe != null)
            {
                MessageBox.Show("Já Existe Lote de Fechamento de Balanço");
                
            }*/        
            if ((inicio.Month != 1) || (inicio.Day != 1) 
                || (fim.Month != 12) || (fim.Day != 31)
                ) {
                MessageBox.Show("Periodo tem que ser o ano todo!");
                return;
            }
            var lstDebito =

                       (from soma in VauchesRazao.AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(inicio) >= 0)
                           && (a.Field<DateTime>("DATA").CompareTo(fim) <= 0)
                          && (a.Field<string>("DEBITO").Substring(0,1) == "3")
                          &&                         
                         ( (a.Field<string>("DEBITO").Trim() != "23398001") 
                          && (a.Field<string>("CREDITO").Trim() != "23398001")
                          ) )  
                        group soma by new
                        {
                            numConta = soma.Field<string>("DEBITO"),

                            // icodser = soma.ICodSer
                        } into g
                        select new BalancoCampos
                        {
                            numconta = g.Key.numConta,
                            debito = g.Sum(p => p.Field<decimal>("VALOR")),

                        }).ToList();

            var lstCredito =

                  (from soma in VauchesRazao.AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(inicio) >= 0)
                      && (a.Field<DateTime>("DATA").CompareTo(fim) <= 0)
                      && (a.Field<string>("CREDITO").Substring(0, 1) == "3")
                       &&
                         ((a.Field<string>("DEBITO").Trim() != "23398001")
                          && (a.Field<string>("CREDITO").Trim() != "23398001")
                          )
                      /*&&  (a.Field<string>("DOC").Trim() != "SIST_BAL") &&
                         ((a.Field<string>("DEBITO").Trim() != "23398001")
                          || (a.Field<string>("CREDITO").Trim() != "23398001"))
                        */
                      )
                       //&& (exclui_fechamento ? (a.Field<string>("DOC").Trim() != "SIST_BAL") : true))
                   group soma by new
                   {
                       numConta = soma.Field<string>("CREDITO"),

                       // icodser = soma.ICodSer
                   } into g
                   select new BalancoCampos
                   {
                       numconta = g.Key.numConta,
                       credito = g.Sum(p => p.Field<decimal>("VALOR")),

                   }).ToList();


            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string pla = "PTPLA" + (AnoGeral - 1).ToString() + ".DBF";
            setoroledb = "SELECT numconta, descricao, sdo, desc2, data FROM " + path + pla +
                       " ORDER BY NUMCONTA";
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

            DataTable result = odataset.Tables[0].Copy();



            

            //OleDbCommand oledbcomm;
            //string setoroledb, path;
            //DataTable AnoTable = dataSet1.Tables[0];
            //if (AnoTable.Rows.Count == 0 ) { MessageBox.Show("Sem Lançamentos"); return; }
            //DataRow orow = AnoTable.Rows[0];
            //string ano = Convert.ToDateTime(orow["DATA"]).Year.ToString(); 
           
            path = TDataControlReduzido.Get_Path("CONTAB");

            string tabela = "PTMOVFIN" + AnoGeral.ToString();                ;
           // bool naoePadrao = 
             //   (MessageBox.Show("Sobreescreverá tabela "+tabela+" se existir. Confirma?", "Responda", MessageBoxButtons.OKCancel) == DialogResult.OK);

            //if (naoePadrao == false) { return; }
            OleDbCommand command = new OleDbCommand(
                      "DELETE FROM " + path + tabela + " WHERE  ((DEBITO = '23398001') OR (CREDITO = '23398001')) ",
                      TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            try
            {
               
                command.ExecuteScalar();
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                MessageBox.Show("Erro Deletar FECHAMENTO ANTERIOR");
                return;
                
            }

            Decimal mov_id = 0; 
            // setoroledb += " ORDER BY DATA,NUMREG";
            try
            {
                DataSet ptdataset = new DataSet();
                setoroledb = "SELECT *  FROM " + path + tabela;
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                oledbda = TDataControlContab.GravePtMovFin(tabela);
                oledbda.SelectCommand = oledbcomm;
                oledbda.TableMappings.Add("Table", tabela);
                oledbda.Fill(ptdataset);
                int numero_linhasGerais = dataSet1.Tables[0].Rows.Count;

                DataTable tabordenada = dataSet1.Tables[0];

                /*foreach(DataRow row in tabordenada.Rows)
                {
                    string debito = FormateParaNumContaNormal(Convert.ToString(row["DEBITO"]));
                    string credito = FormateParaNumContaNormal(Convert.ToString(row["CREDITO"]));
                    if (debito.Trim() == "")
                        MessageBox.Show("Erro debito");
                    if (credito.Trim() == "")
                        MessageBox.Show("Erro credito");


                    DataRow rowinc = ptdataset.Tables[0].NewRow();
                    foreach (DataColumn origem in tabordenada.Columns)
                    {
                        
                        if (ptdataset.Tables[0].Columns.Contains(origem.ColumnName))
                        {
                            
                            if (origem.ColumnName.ToUpper().Trim() == "DEBITO")
                            {
                                rowinc[origem.ColumnName] = debito;
                            }
                            else if (origem.ColumnName.ToUpper().Trim() == "CREDITO")
                            {
                                rowinc[origem.ColumnName] = credito;
                            }
                            else
                            {
                                if (origem.ColumnName.ToUpper().Trim() == "TP_FIN")
                                {
                                    rowinc[origem.ColumnName] = 0;
                                }
                                else if(origem.ColumnName.ToUpper().Trim() == "TIPO")
                                {
                                    if (row[origem.ColumnName].ToString().Trim() == "")
                                         { rowinc[origem.ColumnName] = "P"; }
                                    else rowinc[origem.ColumnName] = row[origem.ColumnName]; 

                                }
                                else
                                    rowinc[origem.ColumnName] = row[origem.ColumnName];
                            }
                        }
                        else
                        {

                        }

                    }
                    mov_id += 1;
                    rowinc["MOV_ID"] = mov_id;
                    ptdataset.Tables[0].Rows.Add(rowinc);

                }
                int numero_linhas = ptdataset.Tables[0].Rows.Count;
                */

                setoroledb = "SELECT MAX(MOV_ID) movidMAx  FROM " + path + tabela;
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                var numid = oledbcomm.ExecuteScalar();
                try
                {
                    mov_id = Convert.ToDecimal(numid);
                }
                catch (Exception)
                {

                    mov_id= 15000;
                }
                


                foreach (DataRow dataRow in result.AsEnumerable().Where(a => (a.Field<string>("NUMCONTA").Substring(0, 1) == "3")
                      && (a.Field<string>("NUMCONTA").Substring(5, 3) != "000")
               ))
                {
                    decimal debito = 0;
                    BalancoCampos Debito = lstDebito.Where(a => a.numconta == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
                    if (Debito != null)
                    {
                        debito = Debito.debito;
                    }

                    decimal credito = 0;
                    BalancoCampos Credito = lstCredito.Where(a => a.numconta == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
                    if (Credito != null)
                    {
                        credito = Credito.credito;
                    }
                    if ((debito == 0.00M) && (credito == 0.00M)) continue;

                    DataRow rowinc = ptdataset.Tables[0].NewRow();
                    // data, valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, venc, doc_dupl, nhist, doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof
                    decimal sdo = credito - debito;
                    string conta = dataRow["NUMCONTA"].ToString();

                    if (sdo == 0.00M) continue;
                    
                    rowinc["DATA"] = fim;
                    if (sdo > 0)
                      {
                        rowinc["DEBITO"] = dataRow["NUMCONTA"].ToString();
                        rowinc["CREDITO"] = "23398001";
                    }
                    else
                    {
                        rowinc["CREDITO"] = dataRow["NUMCONTA"].ToString();
                        rowinc["DEBITO"] = "23398001";
                        sdo = sdo * -1;
                    }
                    rowinc["VALOR"] = sdo;
                    rowinc["DOC"] = "SIST_BAL";
                    rowinc["HIST"] = "TRANSF ENCERRAMENTO BALANCO             ";
                    rowinc["TIPO"] = "P";
                    rowinc["TP_FIN"] = 0;
                    rowinc["TP_OK"] = 0;
                    rowinc["FORN"] = "";
                    rowinc["TIPO_DOC"] = "";
                    rowinc["VENC"] = DateTime.MinValue;
                    rowinc["DOC_DUPL"] = "";
                    rowinc["DOC_FISC"] = "";
                    rowinc["EMISSOR"] = "";
                    rowinc["DATA_EMI"] = DateTime.MinValue;
                    rowinc["NHIST"] = "";
                    rowinc["OBS"] = "";
                    rowinc["LIVRO"] = "";
                    rowinc["CODIGOF"] = "";
                    //obs, mov_id, outro_id, serienf, basecalc, isento, outros




                                       mov_id += 1;
                    rowinc["MOV_ID"] = mov_id;
                    ptdataset.Tables[0].Rows.Add(rowinc);
                }





                    int numero_gravados =  oledbda.Update(ptdataset.Tables[tabela]);
                ptdataset.Tables[tabela].AcceptChanges();

                
                // return oledbda;
            }
            catch (Exception E)
            {
                //throw;
                Cursor.Current = Cursors.Default;
                throw new Exception(E.Message);
            }
            Cursor.Current = Cursors.Default;
            Reload();

        }

        private Boolean ImportaRelaciona_Excel()
        {
            bool result = false;
            if (!((tabexcel != null) && (tabexcel.Columns.Contains("NUMCONTA"))
                && (tabexcel.Columns.Contains("NOVOCOD")) && (tabexcel.Columns.Contains("NOVADESC"))
                && (tabexcel.Columns.Contains("REDUZIDO"))))
            {
                MessageBox.Show("Abra Excel com dados e nome_de_campos corretos");
                return result;
            }
            /*if (!odataset.Tables.Contains("PLACON"))
            {
                MessageBox.Show("Abra Placon");
                return false;
            }*/

            result = true;
            try
            {
                bool deletou = TDataControlContab.DeleteTabRelaciona();
                adrelaciona = TDataControlContab.CreateTabRelaciona();

                adrelaciona.TableMappings.Add("Table", "RELACIONA");
                dsrelaciona = new DataSet();
                adrelaciona.Fill(dsrelaciona);
                int nreg = 0;

                // int order = 1;
                foreach (DataRow linexc in tabexcel.AsEnumerable().OrderBy(row => row.Field<string>("novocod")))
                {
                    //DataRow linexc = tabexcel.Rows[i];
                    if ((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == ""))
                        continue;
                    DataRow linharelaciona = dsrelaciona.Tables["RELACIONA"].NewRow();

                    if (!((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == "")))
                        linharelaciona["NOVOCOD"] = linexc["NOVOCOD"];
                    else
                        linharelaciona["NOVOCOD"] = "";

                    if (!((Convert.IsDBNull(linexc["NOVADESC"])) || (Convert.ToString(linexc["NOVADESC"]).Trim() == "")))
                        linharelaciona["NOVADESC"] = linexc["NOVADESC"];
                    else
                        linharelaciona["NOVADESC"] = "";

                    if (!((Convert.IsDBNull(linexc["NUMCONTA"])) || (Convert.ToString(linexc["NUMCONTA"]).Trim() == "")))
                        linharelaciona["NUMCONTA"] = linexc["NUMCONTA"];
                    else
                        linharelaciona["NUMCONTA"] = "";

                    if (!((Convert.IsDBNull(linexc["DESCRICAO"])) || (Convert.ToString(linexc["DESCRICAO"]).Trim() == "")))
                        linharelaciona["DESCRICAO"] = linexc["DESCRICAO"];
                    else
                        linharelaciona["DESCRICAO"] = "";

                    if (!(Convert.IsDBNull(linexc["REDUZIDO"])))
                        linharelaciona["REDUZIDO"] = Convert.ToInt32(linexc["REDUZIDO"]);
                    else
                        linharelaciona["REDUZIDO"] = 0;

                    nreg++;
                    linharelaciona["NREG"] = nreg;
                    dsrelaciona.Tables["RELACIONA"].Rows.Add(linharelaciona);
                }
                adrelaciona.Update(dsrelaciona.Tables["RELACIONA"]);
                dsrelaciona.Tables["RELACIONA"].AcceptChanges();



            }
            catch (Exception E)
            {
                result = false;


            }

            return true;
        }


    }

}
