using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ClassConexao;
using System.Runtime.Remoting.Messaging;
using ApoioContabilidade.Core;
using System.Data.OleDb;
//using Microsoft.Office.Interop.Excel;
using DataTable = System.Data.DataTable;
using ApoioContabilidade.ZContabAlterData.Utils;

namespace ApoioContabilidade
{
    
    public partial class FrmRazao : Form
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

        public FrmRazao()
        {
            InitializeComponent();
            btnGravePtMovFin.Enabled = true;
            oEntradas = new MonteGrid();
            MonteGrids();
            // dgEntradas.CaptionText = "FINAN E VAUCHES";
            dgEntradas.ReadOnly = true;
            // dgEntradas.AllowSorting = false;
            oEntradas.oDataGridView = dgEntradas;
            //oEntradas.sbTotal = sbRazao;
            oPlacon = TDataControlContab.Placon();
          //  dsBancos = TDataControlContab.TabBancos();

            oForm = new FormFiltro();
            oForm.dtData1.Value = new DateTime(DateTime.Now.Year, 1, 1);
            oForm.dtData2.Value = new DateTime(DateTime.Now.Year, 12, 31);
            bmSourceEntrada = new BindingSource();



           // Reload();
           
        }

        private void Reload()
        {
            //   if (recomece)
            // {
                oForm.oArme = ProcComuns.ArmeEdicaoFiltroGenerico();
                
                oForm.ShowDialog();
                if (oForm.oPesqFin.TabCount > 0)
                    oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
                else
                    oList = null;
                inicio = oForm.dtData1.Value;
                fim = oForm.dtData2.Value;

               
                dsPesquisa = new DataSet();
                dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
                dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());
                oPesquisa = new PesquisaGenerico(dsPesquisa);

            Cursor.Current = Cursors.WaitCursor;

            dataSet1 = TDataControlContab.MovFinanVauches(inicio, fim, oList, oPesquisa);
            // procedimeno para aglutinar registro
                errosPesquisa = dataSet1.Tables[0].Clone();
                errosPesquisa.Columns.Add("ObsErro", Type.GetType("System.String"));

                //var altereTabela =  dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<int>("MOV_ID") != 0) && row.Field<int>("OUTRO_ID") == 0)  )
                AltereDataTableParaRazao();
             JuntaRegistrosTransfBancos();
                 FOLHA_TentaAdvinhar("DOC", "SIST_RURAL NW");
            SistemaAutomaticos_NotasFiscaisTentaAdvinhar();
               SistemaAutomaticos_TentaAdvinhar("","");
            CasosEspeciais_Onde_a_Regra_Foi_Quebrada(); //2017
            
            SistemaAutomaticos_NotasFiscaisTentaAdvinhar("FORN");// 2016 pesquisar por forne
            
            // Caso

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
                  && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("DEBITO")))
                   select valor1.Field<decimal>("VALOR")).Sum();
                oEntradas.LinhasCampo[1].total = Convert.ToDouble(total1);

                System.Nullable<decimal> total2 =
          (from valor1 in VauchesRazao.AsEnumerable()
           where (valor1.Field<string>("CREDITO").Trim() != "")
&& (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("CREDITO")))
           select valor1.Field<decimal>("VALOR")).Sum();
                oEntradas.LinhasCampo[2].total = Convert.ToDouble(total2);

                oEntradas.ColocaTotais();

            //    }
            //  this.Refresh();
            Cursor.Current = Cursors.Default;

        }

        private void PesquiseRazao()
        {

            Cursor.Current = Cursors.WaitCursor;

            DataSet dsrelaciona = TDataControlContab.TabRelaciona();
            DataTable tabrelaciona = dsrelaciona.Tables[0];

            btnGravePtMovFin.Enabled = false;
            DataTable tabordenada = dataSet1.Tables[0];
            bool podeGravarMovFin = true;
            if (tabordenada.Rows.Count == 0) podeGravarMovFin = false;
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
                    string campoAlterdata = "";
                    if (rbNumconta.Checked)
                    {
                        debito = FormateParaNumContaNormal(Convert.ToString(linexc["DEBITO"]));
                        credito = FormateParaNumContaNormal(Convert.ToString(linexc["CREDITO"]));
                        if ((debito == "-1") || (credito == "-1"))
                        {
                            podeGravarMovFin = false;
                        }
                        if ((debito == "") || (credito == ""))
                        {
                            podeGravarMovFin = false;
                        }
                    }
                    else if (rbDesc2.Checked)
                    {
                        debito = Convert.ToString(linexc["DEBITO"]);
                        credito = Convert.ToString(linexc["CREDITO"]);

                    }
                    else if (rbContabilidade.Checked)
                    {
                        campoAlterdata = "NOVOCOD";
                        debito = FormateParaAlterData(Convert.ToString(linexc["DEBITO"]), tabrelaciona, campoAlterdata);
                        credito = FormateParaAlterData(Convert.ToString(linexc["CREDITO"]), tabrelaciona, campoAlterdata);
                    }
                    else if (rbReduzido.Checked)
                    {
                        campoAlterdata = "REDUZIDO";
                        debito = FormateParaAlterData(Convert.ToString(linexc["DEBITO"]), tabrelaciona, campoAlterdata);
                        credito = FormateParaAlterData(Convert.ToString(linexc["CREDITO"]), tabrelaciona, campoAlterdata);
                    }

                    //string debito = Convert.ToString(linexc["DEBITO"]);
                    //string credito = Convert.ToString(linexc["CREDITO"]);


                    //if ((debito == "-1") || (credito == "-1"))
                    //{
                    //  DataRow orow = ErrosNovasContas.NewRow();
                    // orow.ItemArray = linexc.ItemArray;
                    // ErrosNovasContas.Rows.Add(orow);
                    //   continue;
                    // }
                    //if ((debito == "") || (credito == ""))
                    // {
                    DataRow razaoRow = VauchesRazao.NewRow();
                        razaoRow.ItemArray = linexc.ItemArray;
                        razaoRow["DEBITO"] = debito;
                        razaoRow["CREDITO"] = credito;
                        VauchesRazao.Rows.Add(razaoRow);
                    //}

                }
                btnGravePtMovFin.Enabled = true;
                if ((rbNumconta.Checked) && (podeGravarMovFin))
                {
                    btnGravePtMovFin.Enabled = true;
                }
            }
            catch (Exception)
            {
                Cursor.Current = Cursors.Default;
                throw;
            }
            Cursor.Current = Cursors.Default;
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
            oEntradas.AddValores("MOV_ID", "Mov_ID", 10, "", false, 0, "");
            oEntradas.AddValores("OUTRO_ID", "OUTRO_ID", 10, "", false, 0, "");
            oEntradas.AddValores("NOVO_ID", "OUTRO_ID", 10, "", false, 0, "");
            oEntradas.AddValores("FORN", "FORN(HIST)", 40, "", false, 0, "");

        }

        private void btnConsulta_Click(object sender, EventArgs e)
        {
            Reload();
        }

        
        private string FormateParaAlterData(string conta, DataTable tabrelaciona, string camporetornado)
        {
            try
            {
                string contanum = "";
                string result = "";
                if (conta.Trim() == "") return "";
                if (conta.Trim().Length < 3)
                    // if (TDataControlContab.EBanco(conta, true))
                    if (EBancoContabil(conta))
                    {
                        contanum = NBanco_Contab(conta);
                       

                        for (int i = 0; i < tabrelaciona.Rows.Count; i++)
                        {
                            if (Convert.ToString(tabrelaciona.Rows[i]["NUMCONTA"]).Trim() == contanum)
                            {
                                result = Convert.ToString(tabrelaciona.Rows[i][camporetornado]);
                                break;
                            }
                        }
                        if (result == "")
                        {
                            result = "-1";

                        }
                        return result;
                    }
                // Tirei porque os bancos serão descrito pelo DESC2 (incluidos após transformação sofridas pelas TRANSFERÊNCIAS
                // if (oPesquisa.ExisteValor("BANCOS", "DESC2", conta)) return "";

                contanum = TDataControlContab.EPlanoRetornaNumConta(conta, oPesquisa);
               
                for (int i = 0; i < tabrelaciona.Rows.Count; i++)
                {
                    if (Convert.ToString(tabrelaciona.Rows[i]["NUMCONTA"]).Trim() == contanum)
                    {
                        result = Convert.ToString(tabrelaciona.Rows[i][camporetornado]);// .Substring(0, 9);
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
        private string FormateParaNumContaNormal(string conta)
        {
            try
            {
                string contanum = "";
                string result = "";
                if (conta.Trim() == "")
                {
                    return "";
                }    
                    
                    
                  
                if (conta.Trim().Length < 3)
                    if (EBancoContabil(conta))
                    {
                        contanum = NBanco_Contab(conta);
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

                        result = contanum;
                        return result;
                    }
                // Tirei porque os bancos serão descrito pelo DESC2 (incluidos após transformação sofridas pelas TRANSFERÊNCIAS
                // if (oPesquisa.ExisteValor("BANCOS", "DESC2", conta)) return "";

                contanum = TDataControlContab.EPlanoRetornaNumConta(conta, oPesquisa);
                /* var rows = tabrelaciona.Where(row => (row.Field<string>("NUMCONTA").Trim() == contanum)).Select(row=> row);
                 if (rows != null)
                 {
                     int cont = rows.Count();
                     DataRow retRow = rows.FirstOrDefault();
                     if (retRow != null)
                         result = retRow["NOVOCOD"].ToString().Trim();// .Substring(0, 9);
                 }*/

                result = contanum;
                return result;


            }
            catch (Exception E)
            {
                MessageBox.Show("Erro 3: " + E.Message);
                throw;
            }

        }

        private void AltereDataTableParaRazao()
        {
            List<DataRow> altereTabela = new List<DataRow>();
            try
            {
                 altereTabela = dataSet1.Tables[0].AsEnumerable().Where(row => ((row.Field<Decimal>("MOV_ID") != 0) && (row.Field<Decimal>("OUTRO_ID") == 0)
                                                                   && ((row.Field<string>("DEBITO").Trim() == "") || (row.Field<string>("CREDITO").Trim() == "")))).ToList();

            }
            catch (Exception E)
            {

                
            }
            // 
            foreach(DataRow rowpai in altereTabela.AsEnumerable().Where(row => (row.Field<string>("DEBITO").Trim() == "") && 
                     (row.Field<string>("CREDITO").Substring(0,1) != "*") )  )
            {
                List<DataRow> regfilhos = dataSet1.Tables[0].AsEnumerable().Where(row => (rowpai.Field<Decimal>("MOV_ID") == row.Field<Decimal>("OUTRO_ID"))
                 && (row.Field<string>("CREDITO").Trim() == "")
                ).ToList();
                decimal somafilhos = 0;
                if ((regfilhos != null) && (regfilhos.Count > 0))
                {
                    somafilhos = regfilhos.Sum(row => row.Field<decimal>("valor"));
                }
                if ((somafilhos != 0) && (somafilhos == rowpai.Field<decimal>("valor")))
                {
                    foreach(DataRow regfilho in regfilhos)
                    {
                        regfilho.BeginEdit();
                        regfilho["CREDITO"] = rowpai["CREDITO"].ToString();
                        regfilho["NOVO_ID"] = rowpai["MOV_ID"];
                        regfilho.EndEdit();
                        regfilho.AcceptChanges();
                    }
                    rowpai.BeginEdit();
                    rowpai["NOVO_ID"] = -1;
                    rowpai.EndEdit();
                    rowpai.AcceptChanges();
                }
            }
            foreach (DataRow rowpai in altereTabela.Where(row => (row.Field<string>("CREDITO").Trim() == "") && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                              ))
            {
                List<DataRow> regfilhos = dataSet1.Tables[0].AsEnumerable().Where(row => (rowpai.Field<Decimal>("MOV_ID") == row.Field<Decimal>("OUTRO_ID")) && 
                               (row.Field<string>("DEBITO").Trim() == "")).ToList();
                decimal somafilhos = 0;
                if ((regfilhos != null) && (regfilhos.Count > 0))
                {
                    somafilhos = regfilhos.Sum(row => row.Field<decimal>("valor"));
                }
                if ((somafilhos != 0) && (somafilhos == rowpai.Field<decimal>("valor")))
                {
                    foreach (DataRow regfilho in regfilhos)
                    {
                        regfilho.BeginEdit();
                        regfilho["DEBITO"] = rowpai["DEBITO"].ToString();
                        regfilho["NOVO_ID"] = rowpai["MOV_ID"];
                        regfilho.EndEdit();
                        regfilho.AcceptChanges();
                    }
                    rowpai.BeginEdit();
                    rowpai["NOVO_ID"] = -1;
                    rowpai.EndEdit();
                    rowpai.AcceptChanges();
                }
            }
            var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
            foreach(DataRow orow in  rowsdeletar)
            {
                orow.Delete();
                orow.AcceptChanges();
            }
        }
        
        private void JuntaRegistrosTransfBancos()
        {
            List<DataRow> paisPerdidos = new List<DataRow>();

            List<DataRow> altereTabela = new List<DataRow>();
            try
            {
                altereTabela = dataSet1.Tables[0].AsEnumerable().Where(row => (
                (    (row.Field<string>("DEBITO").Trim().Length == 2) && (row.Field<string>("CREDITO").Substring(0, 1) != "*")
                      && (row.Field<string>("CREDITO").Trim() != "")
                     && EBancoContabil_Desc2(row.Field<string>("CREDITO").Trim())
                     )
                  || ((row.Field<string>("CREDITO").Trim().Length == 2)
                         && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                          && (EBancoContabil_Desc2(row.Field<string>("DEBITO").Trim()))
                          )
                          )
                  ).ToList();

            }
            catch (Exception E)
            {
                throw;

            }
            // Procura o par para cada transferencia NO DEBITO O (NUMERO) DO BANCO, no CREDITO DESC2 do banco tranferido
            foreach (DataRow rowpai in altereTabela.AsEnumerable().Where(row => (row.Field<string>("CREDITO").Trim().Length == 2) &&
                                    EBancoContabil(row.Field<string>("CREDITO").Trim())))
            {
                // pega o lanc CUJO CREDITO (NUMERO) DE BANCO COINCIDA COM O 
                List<DataRow> regfilhos = null;
                try
                {
                    regfilhos = altereTabela.AsEnumerable().Where(row =>
                   (rowpai.Field<DateTime>("DATA").CompareTo(row.Field<DateTime>("DATA")) == 0)
                     // && EBancoContabil_Desc2(row.Field<string>("CREDITO").Trim()) 
                     // && (row.Field<Decimal>("VALOR") == rowpai.Field<Decimal>("VALOR"))
                     && (row.Field<string>("DEBITO").Trim().Length == 2)
                     && (row.Field<Decimal>("NOVO_ID") == 0)
                     && (NBanco_DESC2(row.Field<string>("DEBITO")).Trim() == rowpai.Field<string>("DEBITO").Trim())
                     && (row.Field<string>("CREDITO").Trim() == NBanco_DESC2(rowpai.Field<string>("CREDITO").Trim()).Trim())
                ).ToList();
                }
                catch (Exception)
                {

                    throw;
                }

                decimal somafilhos = 0;
                if ((regfilhos != null) && (regfilhos.Count > 0))
                {
                    somafilhos = regfilhos.Sum(row => row.Field<decimal>("valor"));
                    if ((regfilhos.Count > 1) && (somafilhos != rowpai.Field<decimal>("valor")) )
                    {
                        DataRow filhoSalvo = regfilhos.Where(row => row.Field<Decimal>("VALOR") == rowpai.Field<Decimal>("VALOR")).FirstOrDefault();
                        if (filhoSalvo != null)
                        {
                            regfilhos.Clear();
                            regfilhos.Add(filhoSalvo);
                            somafilhos = regfilhos.Sum(row => row.Field<decimal>("valor"));
                        }
                    }
                    

                    if ((somafilhos != 0) && (somafilhos == rowpai.Field<decimal>("valor")))
                    {
                        foreach (DataRow regfilho in regfilhos)
                        {
                            regfilho.BeginEdit();
                            regfilho["DEBITO"] = NBanco_DESC2(regfilho["DEBITO"].ToString());
                            regfilho["NOVO_ID"] = rowpai["MOV_ID"];
                            regfilho.EndEdit();
                            regfilho.AcceptChanges();
                        }
                        rowpai.BeginEdit();
                        rowpai["NOVO_ID"] = -1;
                        rowpai.EndEdit();
                        rowpai.AcceptChanges();
                    }
                    else
                    {
                        string ver = rowpai["CREDITO"].ToString();
                    }
                }
                else
                {
                    paisPerdidos.Add(rowpai);
                   // string ver = rowpai["CREDITO"].ToString();
                }
            }

            foreach (DataRow rowpai in paisPerdidos)
            {
                // pega o lanc CUJO CREDITO (NUMERO) DE BANCO COINCIDA COM O 
                List<DataRow> regfilhos = null;
                try
                {
                    regfilhos = altereTabela.AsEnumerable().Where(row =>
                   ( (  (rowpai.Field<DateTime>("DATA") - row.Field<DateTime>("DATA")).TotalDays < 3)
                  || ((rowpai.Field<DateTime>("DATA") - row.Field<DateTime>("DATA")).TotalDays > 3) )
                     // && EBancoContabil_Desc2(row.Field<string>("CREDITO").Trim()) 
                     // && (row.Field<Decimal>("VALOR") == rowpai.Field<Decimal>("VALOR"))
                     && (row.Field<string>("DEBITO").Trim().Length == 2)
                     && (row.Field<Decimal>("NOVO_ID") == 0)
                     && (NBanco_DESC2(row.Field<string>("DEBITO")).Trim() == rowpai.Field<string>("DEBITO").Trim())
                     && (row.Field<string>("CREDITO").Trim() == NBanco_DESC2(rowpai.Field<string>("CREDITO").Trim()).Trim())
                ).ToList();
                }
                catch (Exception)
                {

                    throw;
                }

                decimal somafilhos = 0;
                DataRow regErro = null;
                if ((regfilhos != null) && (regfilhos.Count > 0))
                {
                    somafilhos = regfilhos.Sum(row => row.Field<decimal>("valor"));
                    if ((regfilhos.Count > 1) && (somafilhos != rowpai.Field<decimal>("valor")))
                    {
                        DataRow filhoSalvo = regfilhos.Where(row => row.Field<Decimal>("VALOR") == rowpai.Field<Decimal>("VALOR")).FirstOrDefault();
                        if (filhoSalvo != null)
                        {
                            regfilhos.Clear();
                            regfilhos.Add(filhoSalvo);
                            somafilhos = regfilhos.Sum(row => row.Field<decimal>("valor"));
                        }
                    }


                    if ((somafilhos != 0) && (somafilhos == rowpai.Field<decimal>("valor")))
                    {


                        
                        foreach (DataRow regfilho in regfilhos)
                        {
                            regErro = errosPesquisa.NewRow();
                            foreach (DataColumn ocol in regfilho.Table.Columns)
                            {
                                regErro[ocol.ColumnName] = regfilho[ocol.ColumnName]; 
                            }
                            regErro["obsErro"] = "Lanc Financ TRansf com datas diferentes";
                            errosPesquisa.Rows.Add(regErro);
                            regfilho.BeginEdit();
                            regfilho["DEBITO"] = NBanco_DESC2(regfilho["DEBITO"].ToString());
                            regfilho["NOVO_ID"] = rowpai["MOV_ID"];
                            regfilho.EndEdit();
                            regfilho.AcceptChanges();
                        }
                        regErro = errosPesquisa.NewRow();
                        foreach (DataColumn ocol in rowpai.Table.Columns)
                        {
                            regErro[ocol.ColumnName] = rowpai[ocol.ColumnName];
                        }
                        regErro["obsErro"] = "Lanc Financ TRansf com datas diferentes";
                        errosPesquisa.Rows.Add(regErro);

                        rowpai.BeginEdit();
                        rowpai["NOVO_ID"] = -1;
                        rowpai.EndEdit();
                        rowpai.AcceptChanges();
                    }
                    else
                    {
                        string ver = rowpai["CREDITO"].ToString();
                        regErro = errosPesquisa.NewRow();
                        foreach (DataColumn ocol in rowpai.Table.Columns)
                        {
                            regErro[ocol.ColumnName] = rowpai[ocol.ColumnName];
                        }
                        regErro["obsErro"] = "Soma Difere";
                        errosPesquisa.Rows.Add(regErro);

                    }
                }
                else
                {
                    regErro = errosPesquisa.NewRow();
                    foreach (DataColumn ocol in rowpai.Table.Columns)
                    {
                        regErro[ocol.ColumnName] = rowpai[ocol.ColumnName];
                    }
                    regErro["obsErro"] = "Não Encontrado PAR";
                    errosPesquisa.Rows.Add(regErro);

                    string ver = rowpai["CREDITO"].ToString();
                }
            }
            ///
            var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
            foreach (DataRow orow in rowsdeletar)
            {
                orow.Delete();
                orow.AcceptChanges();
            }
        }
        
        // numero é Banco?
        public bool EBancoContabil(string conta)
        {
            Boolean result = false;
            if (conta.Trim().Length != 2) return result;
            int intconta;
            try
            { intconta = Convert.ToInt16(conta.Trim()); }
            catch
            {
                return result;
            }
            conta = intconta.ToString("D2");
            DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
            bancos.RowFilter = "nBANCO = " + conta;  
            if (bancos.Count > 0)
            {
                if (Convert.ToString(bancos[0]["CONTAB"]).Trim() == "")
                    result = false;
                else
                    result = true;
            }
            return result;
        }
        public bool EBanco(string conta)
        {
            Boolean result = false;
            if (conta.Trim().Length != 2) return result;
            result = true;
            int intconta;
            try
            { intconta = Convert.ToInt16(conta.Trim());
                conta = intconta.ToString("D2");
                DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
                bancos.RowFilter = "nBANCO = " + conta;
                if (bancos.Count == 0) result = false;
            }
            catch
            {
                result = false;
                
            }
            return result;
        }
        public bool EBancoContabil_Desc2(string conta)
        {
            Boolean result = false;
            if (conta.Trim().Length == 2) return result;
           
            DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
            bancos.RowFilter = "DESC2 = '" + conta.Trim()+"'";
            if (bancos.Count > 0)
            {
                if (Convert.ToString(bancos[0]["CONTAB"]).Trim() == "")
                    result = false;
                else
                    result = true;
            }
            return result;
        }
        // numero é O CANTAB DESTE NUMERO DE BANCO
        private string NBanco_Contab(string conta)
        {
            string result = "";
            if (conta.Trim().Length != 2) return result;
            int intconta;
            try
            { intconta = Convert.ToInt16(conta.Trim()); }
            catch
            {
                return result;
            }
            conta = intconta.ToString("D2");
            DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
            bancos.RowFilter = "nBANCO = " + conta;
            if (bancos.Count > 0)
            {
                if (Convert.ToString(bancos[0]["CONTAB"]).Trim() == "")
                    result = "";
                else
                    result = bancos[0]["CONTAB"].ToString();
            }
            return result;

        }
        // numero é O DESC2 DESTE NUMERO DE BANCO
        private string NBanco_DESC2(string conta)
        {
            string result = "";
            if (conta.Trim().Length != 2) return result;
            int intconta;
            try
            { intconta = Convert.ToInt16(conta.Trim()); }
            catch
            {
                return result;
            }
            conta = intconta.ToString("D2");
            DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
            bancos.RowFilter = "nBANCO = " + conta;
            if (bancos.Count > 0)
            {
                if (Convert.ToString(bancos[0]["DESC2"]).Trim() == "")
                    result = "";
                else
                    result = bancos[0]["DESC2"].ToString();
            }
            return result;

        }
        // o NUMERO DE BANCO DESTE CONTAB
        private string Contab_NBanco(string conta)
        {
            string result = "";
            
            DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
            bancos.RowFilter = "CONTAB = '" + conta.Trim()+"'";
            if (bancos.Count > 0)
            {
                if (Convert.ToString(bancos[0]["NBANCO"]).Trim() == "")
                    result = "";
                else
                {
                    result = bancos[0]["NBANCO"].ToString().Trim();
                    if (result.Length == 1) result = "0" + result;
                }
            }
            return result.Trim();

        }
        // o NUMERO DE BANCO DESTE DESC2
        private string DESC2_NBANCO(string conta)
        {
            string result = "";

            DataView bancos = dsPesquisa.Tables["BANCOS"].AsDataView();
            bancos.RowFilter = "DESC2 = '" + conta.Trim() + "'";
            if (bancos.Count > 0)
            {
                if (Convert.ToString(bancos[0]["NBANCO"]).Trim() == "")
                    result = "";
                else
                {
                    result = bancos[0]["NBANCO"].ToString().Trim();
                    if (result.Length == 1) result = "0" + result;
                }
                
            }
            return result.Trim();

        }


        private void SistemaAutomaticos_TentaAdvinhar(string tdoc, string tvalor)
        {
            // os lançamentos Solteiros (Tentar liga-los) 
            // 'SIST_RURAL NW'
            List<DataRow> creditoVazio = new List<DataRow>();
            List<DataRow> debitoVazio = new List<DataRow>();
            try
            {
                creditoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("DEBITO").Trim())) && (row.Field<string>("CREDITO").Trim() == "")
                     && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                      && (row.Field<string>("DEBITO").Trim() != "")
                      && (tdoc.Trim() == "" ? true : (row.Field<string>(tdoc).Trim() == tvalor.Trim()))
                     )
                  ).ToList();
                debitoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("CREDITO").Trim())) && (row.Field<string>("DEBITO").Trim() == "")
                     && (row.Field<string>("CREDITO").Substring(0, 1) != "*")
                     && (row.Field<string>("CREDITO").Trim() != "")
                      && (tdoc.Trim() == "" ? true : (row.Field<string>(tdoc).Trim() == tvalor.Trim()))
                     )
                  ).ToList();

            }
            catch (Exception E)
            {
            }
            List<DateTime> datasUnicas = debitoVazio.GroupBy(row => row.Field<DateTime>("DATA")).Select(rowl => rowl.Key).ToList();

            foreach (DateTime dia in datasUnicas)
            {
                // começa trabalhando os maiores valores 
               // var maxCredito = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                //var maxDebito = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                Decimal maxCredito = 0;
                try
                {
                    maxCredito = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                }
                catch (Exception)
                {
                    maxCredito = 0;
                }
                Decimal maxDebito = 0;
                try
                {
                    maxDebito = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                }
                catch (Exception)
                {
                    maxDebito = 0;
                }
                maxCredito = Math.Round(maxCredito, 2);
                maxDebito = Math.Round(maxDebito, 2);


                while ((maxCredito != 0) && (maxDebito != 0))
                {
                    if (maxCredito > maxDebito)
                    {
                        decimal valorCompara = maxCredito;
                        DataRow rowpai = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (row.Field<decimal>("VALOR") == valorCompara)).FirstOrDefault();
                        if (rowpai == null) break; // força a saida porque não deve ser

                        // 
                        List<DataRow> lstValoresSoma =
                        creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).OrderBy(row => row.Field<decimal>("VALOR")).ToList();
                        if (lstValoresSoma == null) break;

                        List<Decimal> mov_ids = listaSomadosNivelUm(lstValoresSoma, valorCompara);
                        if (mov_ids.Count == 0)
                        {
                            //if (lstValoresSoma.Count > 10) { MessageBox.Show("Limite Alcançado"); }
                            if ((lstValoresSoma.Count > 10) && (dia.Year != 2018)) { MessageBox.Show("Limite Alcançado"); }
                            mov_ids = listaSomadosOK(lstValoresSoma, valorCompara);
                        }
                        if (mov_ids.Count > 0)
                        {
                            foreach (Decimal mov_id in mov_ids)
                            {
                                DataRow regfilho = creditoVazio.Where(row => (row.Field<Decimal>("MOV_ID") == mov_id)).FirstOrDefault();
                                regfilho.BeginEdit();
                                regfilho["CREDITO"] = rowpai["CREDITO"].ToString();
                                regfilho["NOVO_ID"] = Convert.ToDecimal(rowpai["MOV_ID"]);
                                regfilho.EndEdit();
                                regfilho.AcceptChanges();
                            }
                            rowpai.BeginEdit();
                            rowpai["NOVO_ID"] = -1;
                            rowpai.EndEdit();
                            rowpai.AcceptChanges();
                        }
                        else { break; }
                    }
                    else if (maxCredito < maxDebito)

                    {
                        decimal valorCompara = maxDebito;
                        DataRow rowpai = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (row.Field<decimal>("VALOR") == valorCompara)).FirstOrDefault();
                        if (rowpai == null) break; // força a saida porque não deve ser

                        // 
                        List<DataRow> lstValoresSoma = null;
                        lstValoresSoma = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).OrderBy(row => row.Field<decimal>("VALOR")).ToList();
                        if (lstValoresSoma == null) break;
                        List<Decimal> mov_ids = listaSomadosNivelUm(lstValoresSoma, valorCompara);
                        if (mov_ids.Count == 0)
                        {

                            if ((lstValoresSoma.Count > 10) && (dia.Year !=2018)) { MessageBox.Show("Limite Alcançado"); }
                            mov_ids = listaSomadosOK(lstValoresSoma, valorCompara);
                        }
                        //List<Decimal> mov_ids = listaSomadosOK(lstValoresSoma, valorCompara);
                        if (mov_ids.Count > 0)
                        {
                            foreach (Decimal mov_id in mov_ids)
                            {
                                DataRow regfilho = debitoVazio.Where(row => (row.Field<Decimal>("MOV_ID") == mov_id)).FirstOrDefault();
                                regfilho.BeginEdit();
                                regfilho["DEBITO"] = rowpai["DEBITO"].ToString();
                                regfilho["NOVO_ID"] = Convert.ToDecimal(rowpai["MOV_ID"]);
                                regfilho.EndEdit();
                                regfilho.AcceptChanges();
                            }
                            rowpai.BeginEdit();
                            rowpai["NOVO_ID"] = -1;
                            rowpai.EndEdit();
                            rowpai.AcceptChanges();
                        }
                        else { break; }
                    }
                    else if (maxCredito == maxDebito)
                    {
                        decimal valorCompara = maxDebito;
                        DataRow rowpai = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (Math.Round(row.Field<decimal>("VALOR"),2) == valorCompara)).FirstOrDefault();
                        DataRow regfilho = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (Math.Round(row.Field<decimal>("VALOR"),2) == valorCompara)).FirstOrDefault();
                        regfilho.BeginEdit();
                        regfilho["DEBITO"] = rowpai["DEBITO"].ToString();
                        regfilho["NOVO_ID"] = Convert.ToDecimal(rowpai["MOV_ID"]);
                        regfilho.EndEdit();
                        regfilho.AcceptChanges();
                        rowpai.BeginEdit();
                        rowpai["NOVO_ID"] = -1;
                        rowpai.EndEdit();
                        rowpai.AcceptChanges();

                    }
                    try
                    {
                        maxCredito = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                    }
                    catch { maxCredito = 0; }
                    try
                    {
                        maxDebito = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                    }
                    catch (Exception)
                    {
                        maxDebito = 0;
                    }
                }
            }
            var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
            foreach (DataRow orow in rowsdeletar)
            {
                orow.Delete();
                orow.AcceptChanges();
            }
        }

        // Pesquisar Solteiros pelas notas fiscais
        private void SistemaAutomaticos_NotasFiscaisTentaAdvinhar(string padrao_pesquisa = "DOC_FISC")
        {
            List<DataRow> creditoVazio = new List<DataRow>();
            List<DataRow> debitoVazio = new List<DataRow>();
            try
            {
                

                // DOC_FISC e FORN
                creditoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (row.Field<Decimal>("NOVO_ID") == 0) &&
                 (!EBanco(row.Field<string>("DEBITO").Trim())) && (row.Field<string>("CREDITO").Trim() == "")
                     && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                      && (row.Field<string>("DEBITO").Trim() != "")
                      && (row.Field<string>(padrao_pesquisa).Trim() != "")
                     )
                  ).ToList();
                debitoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                (row.Field<Decimal>("NOVO_ID") == 0) &&
                 (!EBanco(row.Field<string>("CREDITO").Trim())) && (row.Field<string>("DEBITO").Trim() == "")
                     && (row.Field<string>("CREDITO").Substring(0, 1) != "*")
                     && (row.Field<string>("CREDITO").Trim() != "")
                       && (row.Field<string>(padrao_pesquisa).Trim() != "")
                       

                     )
                  ).ToList();

            }
            catch (Exception E)
            {
            }
            
            List<DateTime> datasUnicas = debitoVazio.GroupBy(row => row.Field<DateTime>("DATA")).Select(rowl => rowl.Key).ToList();

            foreach (DateTime dia in datasUnicas)
            {
                // começa trabalhando os maiores valores 
                Decimal maxCredito = 0;
                try
                {
                    maxCredito = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                }
                catch (Exception)    {
                    maxCredito = 0;
                }
                Decimal maxDebito = 0;
                try
                {
                    maxDebito = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                }
                catch (Exception)
                {
                    maxDebito = 0;
                }

                maxCredito = Math.Round(maxCredito, 2);
                maxDebito = Math.Round(maxDebito, 2);
             
             /*  if (maxCredito == Convert.ToDecimal(942.79))
                {
                    int foi = 0;
                }*/
            
                while ((maxCredito != 0) && (maxDebito != 0))
                {
                    if (maxCredito > maxDebito)
                    {
                        decimal valorCompara = maxCredito;
                        DataRow rowpai = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (row.Field<decimal>("VALOR") == valorCompara)).FirstOrDefault();
                        if (rowpai == null) break; // força a saida porque não deve ser

                        string DOC_FISC = rowpai[padrao_pesquisa].ToString().Trim();
                        List<DataRow> lstValoresSoma =
                        creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0)
                        && (row.Field<string>(padrao_pesquisa).Trim() == DOC_FISC)
                        && (row.Field<Decimal>("NOVO_ID") == 0)).OrderBy(row => row.Field<decimal>("VALOR")).ToList();
                        if (lstValoresSoma == null) break;

                        List<Decimal> mov_ids = listaSomadosNivelUm(lstValoresSoma, valorCompara);

                        if (mov_ids.Count > 0)
                        {
                            foreach (Decimal mov_id in mov_ids)
                            {
                                DataRow regfilho = creditoVazio.Where(row => (row.Field<Decimal>("MOV_ID") == mov_id)).FirstOrDefault();
                                regfilho.BeginEdit();
                                regfilho["CREDITO"] = rowpai["CREDITO"].ToString();
                                regfilho["NOVO_ID"] = Convert.ToDecimal(rowpai["MOV_ID"]);
                                regfilho.EndEdit();
                                regfilho.AcceptChanges();
                            }
                            rowpai.BeginEdit();
                            rowpai["NOVO_ID"] = -1;
                            rowpai.EndEdit();
                            rowpai.AcceptChanges();
                        }
                        else
                        {
                            rowpai["NOVO_ID"] = -2;
                        }
                           // break; }
                    }
                    else if (maxCredito < maxDebito)

                    {
                        decimal valorCompara = maxDebito;
                        DataRow rowpai = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (row.Field<decimal>("VALOR") == valorCompara)).FirstOrDefault();
                        if (rowpai == null) break; // força a saida porque não deve ser

                        // 
                        List<DataRow> lstValoresSoma = null;
                        string DOC_FISC = rowpai[padrao_pesquisa].ToString().Trim();
                        lstValoresSoma = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0)
                           && (row.Field<string>(padrao_pesquisa).Trim() == DOC_FISC)
                           && (row.Field<Decimal>("NOVO_ID") == 0)).OrderBy(row => row.Field<decimal>("VALOR")).ToList();
                        if (lstValoresSoma == null) break;
                        List<Decimal> mov_ids = listaSomadosNivelUm(lstValoresSoma, valorCompara);

                        //List<Decimal> mov_ids = listaSomadosOK(lstValoresSoma, valorCompara);
                        if (mov_ids.Count > 0)
                        {
                            foreach (Decimal mov_id in mov_ids)
                            {
                                DataRow regfilho = debitoVazio.Where(row => (row.Field<Decimal>("MOV_ID") == mov_id)).FirstOrDefault();
                                regfilho.BeginEdit();
                                regfilho["DEBITO"] = rowpai["DEBITO"].ToString();
                                regfilho["NOVO_ID"] = Convert.ToDecimal(rowpai["MOV_ID"]);
                                regfilho.EndEdit();
                                regfilho.AcceptChanges();
                            }
                            rowpai.BeginEdit();
                            rowpai["NOVO_ID"] = -1;
                            rowpai.EndEdit();
                            rowpai.AcceptChanges();
                        }
                        else
                        {
                            rowpai["NOVO_ID"] = -2;
                        }
                         //   break; }
                    }
                    else if (maxCredito == maxDebito)
                    {
                        decimal valorCompara = maxDebito;
                        DataRow rowpai = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) 
                            && (Math.Round(row.Field<decimal>("VALOR"),2) == valorCompara)).FirstOrDefault();
                        DataRow regfilho = 
                            debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) 
                            && (Math.Round(row.Field<decimal>("VALOR"),2) == valorCompara)).FirstOrDefault();
                        if (regfilho != null)
                        {
                            regfilho.BeginEdit();
                            regfilho["DEBITO"] = rowpai["DEBITO"].ToString();
                            regfilho["NOVO_ID"] = Convert.ToDecimal(rowpai["MOV_ID"]);
                            regfilho.EndEdit();
                            regfilho.AcceptChanges();
                        }
                        else
                        {
                    

                        }
                        rowpai.BeginEdit();
                        rowpai["NOVO_ID"] = -1;
                        rowpai.EndEdit();
                        rowpai.AcceptChanges();

                    }
                    try
                    {
                        maxCredito = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                    }
                    catch { maxCredito = 0; }
                    try
                    {
                        maxDebito = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)).Max(row => row.Field<decimal>("VALOR"));
                    }
                    catch (Exception)
                    {
                        maxDebito = 0;
                    }
                }
                
            }
            foreach (DataRow rowpai in debitoVazio.Where(row =>  (row.Field<Decimal>("NOVO_ID") == -2)).ToList())
            {
                rowpai.BeginEdit();
                rowpai["NOVO_ID"] = 0;
                rowpai.EndEdit();
                rowpai.AcceptChanges();

            }
            foreach (DataRow rowpai in creditoVazio.Where(row => (row.Field<Decimal>("NOVO_ID") == -2)).ToList())
            {
                rowpai.BeginEdit();
                rowpai["NOVO_ID"] = 0;
                rowpai.EndEdit();
                rowpai.AcceptChanges();

            }
            var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
            foreach (DataRow orow in rowsdeletar)
            {
                orow.Delete();
                orow.AcceptChanges();
            }
        }

        // Ultimo Nivel de Tentativas, Especifico de 31/12/2017, quando Houve alguma alteração 
        // Posterior nos valores do IRPF

        private void CasosEspeciais_Onde_a_Regra_Foi_Quebrada()
        {
            List<DataRow> creditoVazio = new List<DataRow>();
            List<DataRow> debitoVazio = new List<DataRow>();
            List<DataRow> creditoVazioAgrupadoDia = new List<DataRow>();
            List<DataRow> debitoVazioAgrupadoDia = new List<DataRow>();
            try
            {
                creditoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("DEBITO").Trim())) && (row.Field<string>("CREDITO").Trim() == "")
                     && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                      && (row.Field<string>("DEBITO").Trim() != "")

                     )
                  ).ToList();
                debitoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("CREDITO").Trim())) && (row.Field<string>("DEBITO").Trim() == "")
                     && (row.Field<string>("CREDITO").Substring(0, 1) != "*")
                     && (row.Field<string>("CREDITO").Trim() != "")


                     )
                  ).ToList();

            }
            catch (Exception E)
            {
                return;
            }
            if ((debitoVazio.Count == 0) || (creditoVazio.Count == 0)) return;
            List<DateTime> datasUnicas = debitoVazio.GroupBy(row => row.Field<DateTime>("DATA")).Select(rowl => rowl.Key).ToList();

            foreach (DateTime dia in datasUnicas)
            {
                // começa trabalhando os maiores valores 
                // Verifica primeiro os creditos que possam ser guias e

                bool pixotagem = true;
                while (pixotagem)
                {
                    try
                    {
                        debitoVazioAgrupadoDia = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0) && (row.Field<Decimal>("MOV_ID") != 0) &&
                        (row.Field<Decimal>("OUTRO_ID") == 0)).ToList();
                        creditoVazioAgrupadoDia = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                        && (row.Field<Decimal>("MOV_ID") != 0) && (row.Field<Decimal>("OUTRO_ID") == 0)).ToList();
                    }
                    catch (Exception)
                    {
                    }
                    if ((debitoVazioAgrupadoDia.Count == 0) && (creditoVazioAgrupadoDia.Count == 0)) { pixotagem = false; continue; }

                    string creditoescolhido = "";
                    string debitoescolhido = "";
                    Decimal maxCredito = 0;
                    Decimal maxDebito = 0;
                    try
                    {
                        if (debitoVazioAgrupadoDia.Count != 0)
                        {
                            List<string> CreditosTestar = debitoVazioAgrupadoDia.GroupBy(row => row.Field<string>("CREDITO").Trim()).Select(s => s.Key).ToList();
                            foreach (var conta in CreditosTestar)
                            {
                                var valor = debitoVazioAgrupadoDia.Where(row => row.Field<string>("CREDITO").Trim() == conta).Sum(row => row.Field<Decimal>("VALOR"));
                                if (valor > maxCredito)
                                {
                                    maxCredito = valor;
                                    creditoescolhido = conta;
                                }
                            }
                        }

                    }
                    catch (Exception)
                    {
                        maxCredito = 0;
                    }


                    try
                    {
                        if (creditoVazioAgrupadoDia.Count != 0)
                        {
                            List<string> DebitosTestar = creditoVazioAgrupadoDia.GroupBy(row => row.Field<string>("DEBITO").Trim()).Select(s => s.Key).ToList();
                            foreach (var conta in DebitosTestar)
                            {
                                var valor = creditoVazioAgrupadoDia.Where(row => row.Field<string>("DEBITO").Trim() == conta).Sum(row => row.Field<Decimal>("VALOR"));
                                if (valor > maxDebito)
                                {
                                    maxDebito = valor;
                                    debitoescolhido = conta;
                                }
                            }

                        }

                    }
                    catch (Exception)
                    {
                        maxDebito = 0;
                    }
                    maxCredito = Math.Round(maxCredito, 2);
                    maxDebito = Math.Round(maxDebito, 2);
                    if ((maxCredito == 0) && (maxDebito == 0)) { pixotagem = false; continue; }
                    {
                        if (maxCredito > maxDebito)
                        {
                            decimal valorCompara = maxCredito;
                            List<DataRow> rowpais = null;
                            try
                            {
                                rowpais = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)

                                 && (row.Field<string>("CREDITO").Trim() == creditoescolhido)).ToList();
                                if (rowpais.Count() == 0) break; // força a saida porque não deve ser
                            }
                            catch (Exception)
                            {
                            }
                            if ((rowpais == null) || (rowpais.Count == 0)) break;
                            List<Decimal> pesqmovs_ids = new List<Decimal>();
                            foreach (DataRow pai in rowpais)
                            {
                                pesqmovs_ids.Add(Convert.ToDecimal(pai["MOV_ID"]));
                            }

                            List<DataRow> lstValoresSoma =
                            creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0)
                            && pesqmovs_ids.Contains(row.Field<Decimal>("OUTRO_ID"))
                            && (row.Field<Decimal>("NOVO_ID") == 0)).OrderBy(row => row.Field<decimal>("VALOR")).ToList();
                            if (lstValoresSoma == null) break;

                            List<Decimal> mov_ids = listaSomadosNivelUm(lstValoresSoma, valorCompara);

                            if (mov_ids.Count > 0)
                            {
                                foreach (Decimal mov_id in mov_ids)
                                {
                                    DataRow regfilho = creditoVazio.Where(row => (row.Field<Decimal>("MOV_ID") == mov_id)).FirstOrDefault();
                                    regfilho.BeginEdit();
                                    regfilho["CREDITO"] = creditoescolhido;
                                    regfilho["NOVO_ID"] = pesqmovs_ids[0];
                                    regfilho.EndEdit();
                                    regfilho.AcceptChanges();
                                }
                                foreach (DataRow rowpai in rowpais)
                                {

                                    rowpai.BeginEdit();
                                    rowpai["NOVO_ID"] = -1;
                                    rowpai.EndEdit();
                                    rowpai.AcceptChanges();
                                }
                            }
                            else
                            {
                                foreach (DataRow rowpai in rowpais)
                                {
                                    rowpai["NOVO_ID"] = -2;
                                }

                            }
                            // break; }
                        }
                        else if (maxCredito < maxDebito)

                        {
                            decimal valorCompara = maxDebito;
                            List<DataRow> rowpais = null;
                            try
                            {
                                rowpais = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)

                                 && (row.Field<string>("DEBITO").Trim() == debitoescolhido)).ToList();
                                if (rowpais.Count() == 0) break; // força a saida porque não deve ser
                            }
                            catch (Exception)
                            {
                            }
                            if ((rowpais == null) || (rowpais.Count == 0)) break;
                            List<Decimal> pesqmovs_ids = new List<Decimal>();
                            foreach (DataRow pai in rowpais)
                            {
                                pesqmovs_ids.Add(Convert.ToDecimal(pai["MOV_ID"]));
                            }

                            List<DataRow> lstValoresSoma =
                            debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0)
                            && pesqmovs_ids.Contains(row.Field<Decimal>("OUTRO_ID"))
                            && (row.Field<Decimal>("NOVO_ID") == 0)).OrderBy(row => row.Field<decimal>("VALOR")).ToList();
                            if (lstValoresSoma == null) break;

                            List<Decimal> mov_ids = listaSomadosNivelUm(lstValoresSoma, valorCompara);

                            if (mov_ids.Count > 0)
                            {
                                foreach (Decimal mov_id in mov_ids)
                                {
                                    DataRow regfilho = creditoVazio.Where(row => (row.Field<Decimal>("MOV_ID") == mov_id)).FirstOrDefault();
                                    regfilho.BeginEdit();
                                    regfilho["DEBITO"] = debitoescolhido;
                                    regfilho["NOVO_ID"] = pesqmovs_ids[0];
                                    regfilho.EndEdit();
                                    regfilho.AcceptChanges();
                                }
                                foreach (DataRow rowpai in rowpais)
                                {

                                    rowpai.BeginEdit();
                                    rowpai["NOVO_ID"] = -1;
                                    rowpai.EndEdit();
                                    rowpai.AcceptChanges();
                                }
                            }
                            else
                            {
                                foreach (DataRow rowpai in rowpais)
                                {
                                    rowpai["NOVO_ID"] = -2;
                                }

                            }
                            //   break; }
                        }
                        // verifica se zerou no dia
                    }
                }
            }
            foreach (DataRow rowpai in debitoVazio.Where(row => (row.Field<Decimal>("NOVO_ID") == -2)).ToList())
            {
                rowpai.BeginEdit();
                rowpai["NOVO_ID"] = 0;
                rowpai.EndEdit();
                rowpai.AcceptChanges();

            }
            foreach (DataRow rowpai in creditoVazio.Where(row => (row.Field<Decimal>("NOVO_ID") == -2)).ToList())
            {
                rowpai.BeginEdit();
                rowpai["NOVO_ID"] = 0;
                rowpai.EndEdit();
                rowpai.AcceptChanges();

            }
            var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
            foreach (DataRow orow in rowsdeletar)
            {
                orow.Delete();
                orow.AcceptChanges();
            }
        }






        private void FOLHA_TentaAdvinhar(string tdoc, string tvalor)
        {
            // os lançamentos Solteiros (Tentar liga-los) 
            // 'SIST_RURAL NW'

            List<DataRow> creditoVazio = new List<DataRow>();
            List<DataRow> debitoVazio = new List<DataRow>();
            try
            {
                creditoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("DEBITO").Trim())) && (row.Field<string>("CREDITO").Trim() == "")
                     && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                      && (row.Field<string>("DEBITO").Trim() != "")
                      && (tdoc.Trim() == "" ? true : (row.Field<string>(tdoc).Trim() == tvalor.Trim()))
                     )
                  ).ToList();
                debitoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("CREDITO").Trim())) && (row.Field<string>("DEBITO").Trim() == "")
                     && (row.Field<string>("CREDITO").Substring(0, 1) != "*")
                     && (row.Field<string>("CREDITO").Trim() != "")
                      && (tdoc.Trim() == "" ? true : (row.Field<string>(tdoc).Trim() == tvalor.Trim()))
                     )
                  ).ToList();

            }
            catch (Exception E)
            {
            }
            List<DateTime> datasUnicas = debitoVazio.GroupBy(row => row.Field<DateTime>("DATA")).Select(rowl => rowl.Key).ToList();

            foreach (DateTime dia in datasUnicas)
            {
                string tipoLanc = "FOLHA SAL. FAMILIA";  // A Palavra INSS LANCADA NO HIST
                                                         // "FOLHA SAL.FAMILIA"
                                                         // "FOLHA SAL.FAMILIA"
                List<DataRow> salarioFamilia = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                     && (row.Field<string>("HIST").Contains(tipoLanc))).ToList();
                DataRow regSalFam = salarioFamilia.FirstOrDefault();
                tipoLanc = "DESC. INSS TRAB.";
                List<DataRow> impINNS = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                     && (row.Field<string>("HIST").Contains(tipoLanc))).ToList();
                
                DataRow regProvINSS = impINNS.FirstOrDefault();

                List<DataRow> irFonte = null;
                DataRow regIrFonte = null;
                Decimal ValoraRatearIR = 0;
                DataRow dataRowMaiorValor = null;
                if (dia.Year >= 2023)
                {
                    tipoLanc = "DESC. IRF TRAB.";  // A Palavra IRF LANCADA NO HIST
                                                   // "DESC. IRF TRAB."
                    irFonte = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                            && (row.Field<string>("HIST").Contains(tipoLanc))).ToList();
                    regIrFonte = irFonte.FirstOrDefault();
                    ////////////

                    ValoraRatearIR = Convert.ToDecimal(regIrFonte["VALOR"]);
                }

                tipoLanc = "FOLHA TRAB RURAL"; // FOLHA TRAB RURAL

                DataRow rowProvFolha = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                               && (row.Field<string>("HIST").Contains("FOLHA"))).FirstOrDefault();
                
                List<DataRow> lancFazendas = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                              && (row.Field<string>("HIST").Contains(tipoLanc))).ToList();

                Decimal somalanc = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                               && (row.Field<string>("HIST").Contains(tipoLanc))).Sum(row => row.Field<Decimal>("VALOR"));

                if (ValoraRatearIR > 0)
                {
                  // var tipo =   lancFazendas.OrderByDescending(row => row.Field<double>("VALOR")).FirstOrDefault();
                    dataRowMaiorValor = lancFazendas.OrderByDescending(row => row.Field<Decimal>("VALOR")).FirstOrDefault();
                    somalanc = somalanc - ValoraRatearIR;


                }

                // A DIFERENCA ENTRE A PROVISAO DO INSS E O VALOR DO SALARIO FAMILIA DEVERÁ SER RATEADO ENTRE OS LANC DAS FAZENDAS 
                Decimal ValoraRatear = Convert.ToDecimal(regProvINSS["VALOR"]) - Convert.ToDecimal(regSalFam["VALOR"]);

                DataTable fazendasInssRateado = dataSet1.Tables[0].Clone();
                Decimal vlrIRF = 0;
                foreach (DataRow rowfazenda in lancFazendas)
                {
                    vlrIRF = 0;
                    if (Convert.ToDecimal(rowfazenda["VALOR"]) == 0)
                    {
                        rowfazenda.BeginEdit();
                        rowfazenda["NOVO_ID"] = -1;
                        rowfazenda.EndEdit();
                        rowfazenda.AcceptChanges();
                        // MessageBox.Show("Lancamento folha zerado?? Excluir");
                        continue;

                    }
                    if ((regIrFonte != null) &&  (dataRowMaiorValor != null) && dataRowMaiorValor["MOV_ID"] == rowfazenda["MOV_ID"]) 
                    {
                        vlrIRF = ValoraRatearIR;
                    }

                    Decimal valLancado = Math.Round(((Convert.ToDecimal(rowfazenda["VALOR"]) - vlrIRF) / somalanc) * ValoraRatear, 2);
                    if (valLancado <= Convert.ToDecimal(0.00)) continue;

                    DataRow rownovo = fazendasInssRateado.NewRow();
                    rownovo.ItemArray = rowfazenda.ItemArray;
                    rownovo["CREDITO"] = regProvINSS["CREDITO"].ToString();
                    rownovo["VALOR"] = valLancado;
                    rownovo["HIST"] = regSalFam["HIST"];
                    rownovo["MOV_ID"] = Convert.ToDecimal(rowfazenda["MOV_ID"]);
                    rownovo["NOVO_ID"] = regProvINSS["MOV_ID"];
                    fazendasInssRateado.Rows.Add(rownovo);
                }
                fazendasInssRateado.AcceptChanges();

                Decimal somaRateado = fazendasInssRateado.AsEnumerable().Sum(row => row.Field<Decimal>("VALOR"));
                if (somaRateado != ValoraRatear)
                {
                    // coloca a diferença no maior valor da série
                    Decimal valorMax = fazendasInssRateado.AsEnumerable().Max(row => row.Field<Decimal>("VALOR"));
                    DataRow rowMax = fazendasInssRateado.AsEnumerable().Where(row => row.Field<Decimal>("VALOR") == valorMax).FirstOrDefault();
                    rowMax.BeginEdit();
                    rowMax["VALOR"] = Convert.ToDecimal(rowMax["VALOR"]) - (ValoraRatear - somaRateado);
                    rowMax.EndEdit();
                    rowMax.AcceptChanges();
                }
                fazendasInssRateado.AcceptChanges();
                // retificar valores  Originais 
                foreach (DataRow rowfazenda in lancFazendas)
                {
                    DataRow rowRateadosInss = null;
                    try { rowRateadosInss = fazendasInssRateado.AsEnumerable().Where(row => row.Field<Decimal>("MOV_ID") == Convert.ToDecimal(rowfazenda["MOV_ID"])).FirstOrDefault(); }
                    catch { rowRateadosInss = null; }
                    if (rowRateadosInss == null) continue;
                    vlrIRF = 0;
                    if ((regIrFonte != null) && (dataRowMaiorValor != null) && dataRowMaiorValor["MOV_ID"] == rowfazenda["MOV_ID"])
                    {
                        vlrIRF = ValoraRatearIR;
                    }
                    rowfazenda.BeginEdit();
                    rowfazenda["VALOR"] = (Convert.ToDecimal(rowfazenda["VALOR"]) - vlrIRF) - Convert.ToDecimal(rowRateadosInss["VALOR"]);
                    rowfazenda["CREDITO"] = rowProvFolha["CREDITO"];
                    rowfazenda["NOVO_ID"] = rowfazenda["MOV_ID"];
                    rowfazenda.EndEdit();
                    rowfazenda.AcceptChanges();
                }
                // Colocar os valores do INSS das fazendas na TabelaPrincipal;
                foreach (DataRow fazinss in fazendasInssRateado.Rows)
                {
                    DataRow dataRow = dataSet1.Tables[0].NewRow();
                    dataRow.ItemArray = fazinss.ItemArray;
                    dataSet1.Tables[0].Rows.Add(dataRow);
                }


                if ((regIrFonte != null) && (dataRowMaiorValor != null) )
                {
                    dataRowMaiorValor.BeginEdit();
                    dataRowMaiorValor["VALOR"] = (Convert.ToDecimal(dataRowMaiorValor["VALOR"]) - ValoraRatearIR);
                    dataRowMaiorValor.EndEdit();

                    regIrFonte.BeginEdit();
                    regIrFonte["DEBITO"] = dataRowMaiorValor["DEBITO"];
                    regIrFonte["VALOR"] = ValoraRatearIR;
                    regIrFonte["NOVO_ID"] = dataRowMaiorValor["MOV_ID"];
                    regIrFonte.EndEdit();
                 }
                    // COLOCA A CONTRA PARTIDA NO REGSALFAM
                regSalFam.BeginEdit();
                regSalFam["CREDITO"] = regProvINSS["CREDITO"];
                regSalFam["NOVO_ID"] = regSalFam["MOV_ID"];
                regSalFam.EndEdit();
                regSalFam.AcceptChanges();
                // Marca LANÇAMENTOS BASE DOS RATEIOS PARA DELETAR 
                // marca lançamento PROVINNS para DELETAR
                regProvINSS.BeginEdit();
                regProvINSS["NOVO_ID"] = -1;
                regProvINSS.EndEdit();
                regProvINSS.AcceptChanges();
                // marca lançamento PROVINNS para DELETAR
                rowProvFolha.BeginEdit();
                rowProvFolha["NOVO_ID"] = -1;
                rowProvFolha.EndEdit();
                rowProvFolha.AcceptChanges();
            }
            try
            {
                dataSet1.Tables[0].AcceptChanges(); // aceitar as inserções
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ALteração FOLHAS TRAB");
                throw;
            }

            var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
            foreach (DataRow orow in rowsdeletar)
            {
                orow.Delete();
                orow.AcceptChanges();
            }
            
           // FOLHA_Ajuste_IRFONTE("DOC", "SIST_RURAL NW");
        }



        
        private void FOLHA_Ajuste_IRFONTE(string tdoc, string tvalor)
        {
            // ACreSCENTEI O LANÇAMENTO SOLTEIRO DO IRF NO CRÉDITO, 
            // EM JANEIRO DE 2024 
            List<DataRow> creditoVazio = new List<DataRow>();
            List<DataRow> debitoVazio = new List<DataRow>();
            try
            {
                creditoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("DEBITO").Trim())) && (row.Field<string>("CREDITO").Trim() == "")
                     && (row.Field<string>("DEBITO").Substring(0, 1) != "*")
                      && (row.Field<string>("DEBITO").Trim() != "")
                      && (tdoc.Trim() == "" ? true : (row.Field<string>(tdoc).Trim() == tvalor.Trim()))
                     )
                  ).ToList();
                debitoVazio = dataSet1.Tables[0].AsEnumerable().Where(row => (
                 (!EBanco(row.Field<string>("CREDITO").Trim())) && (row.Field<string>("DEBITO").Trim() == "")
                     && (row.Field<string>("CREDITO").Substring(0, 1) != "*")
                     && (row.Field<string>("CREDITO").Trim() != "")
                      && (tdoc.Trim() == "" ? true : (row.Field<string>(tdoc).Trim() == tvalor.Trim()))
                     )
                  ).ToList();

            }
            catch (Exception E)
            {
            }
            List<DateTime> datasUnicas = debitoVazio.GroupBy(row => row.Field<DateTime>("DATA")).Select(rowl => rowl.Key).ToList();

            foreach (DateTime dia in datasUnicas)
            {
                if (dia.Year < 2023) continue;
                
                    ////// CASO DO IRF
                string tipoLanc = "DESC. IRF TRAB.";  // A Palavra IRF LANCADA NO HIST
                                                // "DESC. IRF TRAB."
                List<DataRow> irFonte = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                        && (row.Field<string>("HIST").Contains(tipoLanc))).ToList();
                DataRow regIrFonte = irFonte.FirstOrDefault();
                ////////////

                Decimal ValoraRatear = Convert.ToDecimal(regIrFonte["VALOR"]);
               



                tipoLanc = "FOLHA TRAB RURAL"; // FOLHA TRAB RURAL
                DataRow rowProvFolha = debitoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                               && (row.Field<string>("HIST").Contains("FOLHA"))).FirstOrDefault();
                List<DataRow> lancFazendas = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                              && (row.Field<string>("HIST").Contains(tipoLanc))).ToList();

                Decimal somalanc = creditoVazio.Where(row => (row.Field<DateTime>("DATA").CompareTo(dia) == 0) && (row.Field<Decimal>("NOVO_ID") == 0)
                               && (row.Field<string>("HIST").Contains(tipoLanc))).Sum(row => row.Field<Decimal>("VALOR"));

                if (ValoraRatear == 0) continue;

                DataTable fazendasIRFRateado = dataSet1.Tables[0].Clone();
                foreach(DataRow rowfazenda in lancFazendas)
                {
                    if (Convert.ToDecimal(rowfazenda["VALOR"]) == 0)
                    {
                        rowfazenda.BeginEdit();
                        rowfazenda["NOVO_ID"] = -1;
                        rowfazenda.EndEdit();
                        rowfazenda.AcceptChanges();
                        // MessageBox.Show("Lancamento folha zerado?? Excluir");
                        continue;
                        
                    }
                    Decimal valLancado = Math.Round((Convert.ToDecimal(rowfazenda["VALOR"]) / somalanc) * ValoraRatear, 2);
                    if (valLancado <= Convert.ToDecimal(0.00)) continue;  
                    DataRow rownovo = fazendasIRFRateado.NewRow();
                    rownovo.ItemArray = rowfazenda.ItemArray;
                    rownovo["CREDITO"] = regIrFonte["CREDITO"].ToString();
                    rownovo["VALOR"] = valLancado;
                    rownovo["HIST"] = regIrFonte["HIST"];
                    rownovo["MOV_ID"] = Convert.ToDecimal(rowfazenda["MOV_ID"]);
                    rownovo["NOVO_ID"] = regIrFonte["MOV_ID"];
                    fazendasIRFRateado.Rows.Add(rownovo);
                }
                fazendasIRFRateado.AcceptChanges();
                Decimal somaRateado = fazendasIRFRateado.AsEnumerable().Sum(row => row.Field<Decimal>("VALOR"));
                if (somaRateado != ValoraRatear)
                {
                    // coloca a diferença no maior valor da série
                    Decimal valorMax = fazendasIRFRateado.AsEnumerable().Max(row => row.Field<Decimal>("VALOR"));
                    DataRow rowMax = fazendasIRFRateado.AsEnumerable().Where(row => row.Field<Decimal>("VALOR") == valorMax).FirstOrDefault();
                    rowMax.BeginEdit();
                    rowMax["VALOR"] = Convert.ToDecimal(rowMax["VALOR"]) - (ValoraRatear - somaRateado);
                    rowMax.EndEdit();
                    rowMax.AcceptChanges();
                }
                fazendasIRFRateado.AcceptChanges();
                // retificar valores  Originais 
                foreach (DataRow rowfazenda in lancFazendas)
                {
                    DataRow rowRateadosInss = null;
                    try { rowRateadosInss = fazendasIRFRateado.AsEnumerable().Where(row => row.Field<Decimal>("MOV_ID") == Convert.ToDecimal(rowfazenda["MOV_ID"])).FirstOrDefault(); }
                     catch { rowRateadosInss = null; }
                    if (rowRateadosInss == null) continue;
                    rowfazenda.BeginEdit();
                    rowfazenda["VALOR"] = Convert.ToDecimal(rowfazenda["VALOR"]) - Convert.ToDecimal(rowRateadosInss["VALOR"]);
                    rowfazenda["CREDITO"] = rowProvFolha["CREDITO"];
                    rowfazenda["NOVO_ID"] = rowfazenda["MOV_ID"];
                    rowfazenda.EndEdit();
                    rowfazenda.AcceptChanges();
                }
                // Colocar os valores do IRF das fazendas na TabelaPrincipal;
                foreach(DataRow fazinss in fazendasIRFRateado.Rows)
                {
                    DataRow dataRow = dataSet1.Tables[0].NewRow();
                    dataRow.ItemArray = fazinss.ItemArray;
                    dataSet1.Tables[0].Rows.Add(dataRow);
                }
                regIrFonte.BeginEdit();
                regIrFonte["NOVO_ID"] = -1;
                regIrFonte.EndEdit();
                regIrFonte.AcceptChanges();

                // COLOCA A CONTRA PARTIDA NO REGIRFONTE
                /* regSalFam.BeginEdit();
                 regSalFam["CREDITO"] = regProvINSS["CREDITO"];
                 regSalFam["NOVO_ID"] = regSalFam["MOV_ID"];
                 regSalFam.EndEdit();
                 regSalFam.AcceptChanges();
                 // Marca LANÇAMENTOS BASE DOS RATEIOS PARA DELETAR 
                 // marca lançamento PROVINNS para DELETAR
                 regProvINSS.BeginEdit();
                 regProvINSS["NOVO_ID"] = -1;
                 regProvINSS.EndEdit();
                 regProvINSS.AcceptChanges();*/
                 // marca lançamento PROVINNS para DELETAR
                 rowProvFolha.BeginEdit();
                 rowProvFolha["NOVO_ID"] = -1;
                 rowProvFolha.EndEdit();
                 rowProvFolha.AcceptChanges();
             }
             try
             {
                 dataSet1.Tables[0].AcceptChanges(); // aceitar as inserções
             }
             catch (Exception)
             {
                 MessageBox.Show("Erro ALteração FOLHAS TRAB");
                 throw;
             }

             var rowsdeletar = dataSet1.Tables[0].AsEnumerable().Where(row => (row.Field<Decimal>("NOVO_ID") == -1)).ToList();
             foreach (DataRow orow in rowsdeletar)
             {
                 orow.Delete();
                 orow.AcceptChanges();
             }

         }



         private void rbDesc2_CheckedChanged(object sender, EventArgs e)
         {
             PesquiseRazao();
         }

         private void rbContabilidade_CheckedChanged(object sender, EventArgs e)
         {
             PesquiseRazao();
         }

         private void rbNumconta_CheckedChanged(object sender, EventArgs e)
         {
             PesquiseRazao();
         }
         private void rbReduzido_Click(object sender, EventArgs e)
         {
             PesquiseRazao();
         }


         private List<Decimal> listaSomadosOK(List<DataRow> registros, decimal valormeta)
         {
             List<decimal> result = new List<decimal>(); // lista dos mov_id (identificados

             //List<List<List<int>>> posibilidades = new List<List<List<int>>>();
             // int menor = 0; int maior = registros.Length - 1;
             int[] tipo = new int[registros.Count];
             for (int i = 0; i < registros.Count; i++)
             {
                 tipo[i] = i;
             }
             List<List<List<int>>> posibilidades = PossibidadesLimite(tipo, registros.Count);

             bool match = false; 
             foreach (var primeiro in posibilidades)
             {
                 foreach (var segundo in primeiro)
                 {
                     decimal soma = 0;
                     result.Clear();
                     foreach (int terceiro in segundo)
                     {
                         soma += Convert.ToDecimal(registros[terceiro]["VALOR"]);
                         result.Add(Convert.ToDecimal(registros[terceiro]["MOV_ID"]));
                     }
                     if (soma == valormeta) {
                         match = true; 
                         break; }
                     else { result.Clear(); }
                 }
                 if (match) { break; }
             }

             return result;
         }

         private List<Decimal> listaSomadosNivelUm(List<DataRow> registros, decimal valormeta)
         {
             List<decimal> result = new List<decimal>(); // lista dos mov_id (identificados

             //List<List<List<int>>> posibilidades = new List<List<List<int>>>();
             // int menor = 0; int maior = registros.Length - 1;
             int[] tipo = new int[registros.Count];
             for (int i = 0; i < registros.Count; i++)
             {
                 tipo[i] = i;
             }
             // TENTE somente um nivel

             //bool match = false;
             decimal soma = 0;
             result.Clear();
             foreach (int terceiro in tipo)
             {
                 soma += Math.Round(Convert.ToDecimal(registros[terceiro]["VALOR"]),4);
                 result.Add(Convert.ToDecimal(registros[terceiro]["MOV_ID"]));
                 // alterei em janeiro 2025
                 if (Math.Round(soma,2) == valormeta)
                 {
                     // match = true;
                     break;
                 }
             }
            // alterei em janeiro 2025
            if (Math.Round(soma, 2) != valormeta)
             {
                 result.Clear();
             }
             return result;
         }

         //
         //
         private void btnGravePtMovFin_Click(object sender, EventArgs e)
         {
             Cursor.Current = Cursors.WaitCursor;

             OleDbCommand oledbcomm;
             string setoroledb, path;
             //  DataSet result = null;
             DataTable AnoTable = dataSet1.Tables[0];
             if (AnoTable.Rows.Count == 0 ) { MessageBox.Show("Sem Lançamentos"); return; }
             DataRow orow = AnoTable.Rows[0];
             string ano = Convert.ToDateTime(orow["DATA"]).Year.ToString(); 
             if (!TDataControlContab.EncontreCopiePTMovFin(ano))
             {
                 Cursor.Current = Cursors.Default;
                 MessageBox.Show("Não Gravado");

                 return;
             }

             path = TDataControlReduzido.Get_Path("CONTAB");

             string tabela = "PTMOVFIN" + ano;
             bool naoePadrao = (MessageBox.Show("Sobreescreverá tabela "+tabela+" se existir. Confirma?", "Responda", MessageBoxButtons.OKCancel) == DialogResult.OK);

             if (naoePadrao == false) { return; }
             OleDbCommand command = new OleDbCommand(
                       "DELETE FROM " + path + tabela,
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
             try
             {
                 command.ExecuteScalar();
             }
             catch (Exception)
             {
                 Cursor.Current = Cursors.Default;
                 MessageBox.Show("Erro ao esvaziar tabela "+tabela);
                 return;

             }

             Decimal mov_id = 0; 
             // setoroledb += " ORDER BY DATA,NUMREG";
             try
             {
                 DataSet ptdataset = new DataSet();
                 setoroledb = "SELECT *  FROM " + path + tabela;
                 oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                 OleDbDataAdapter oledbda = TDataControlContab.GravePtMovFin(tabela);
                 oledbda.SelectCommand = oledbcomm;
                 oledbda.TableMappings.Add("Table", tabela);
                 oledbda.Fill(ptdataset);
                 int numero_linhasGerais = dataSet1.Tables[0].Rows.Count;

                 DataTable tabordenada = dataSet1.Tables[0];
                 foreach(DataRow row in tabordenada.Rows)
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
         }

         private void button2_Click(object sender, EventArgs e)
         {
             // mostra erros
             FrmErros oForm = new FrmErros(errosPesquisa);
             //oForm.otable = ErrosNovasContas.Copy();
             oForm.Show();
         }


         // trabalha as possibilidade 
         // Se o numero de em brancos for maior que 6, 

         private List<List<List<int>>> PossibidadesLimite(int[] tipo, int registrosCount)
         {

             List<List<List<int>>> posibilidades = new List<List<List<int>>>();
             if (registrosCount < 16)
             {
                 for (int i = 0; i < registrosCount; i++)
                 {
                     List<List<int>> combina2 = new List<List<int>>();
                     var resu = Gerais.Combinations(tipo, i + 1);
                     foreach (var num in resu)
                     {
                         List<int> combina = new List<int>();
                         foreach (int num2 in num)
                         {
                             combina.Add(num2);
                         }
                         combina2.Add(combina);
                     }
                     posibilidades.Add(combina2);
                 }
             }
             else
             {
                 for (int i = 0; i < 3 ; i++)
                 {
                     List<List<int>> combina2 = new List<List<int>>();
                     var resu = Gerais.Combinations(tipo, i + 1);
                     foreach (var num in resu)
                     {
                         List<int> combina = new List<int>();
                         foreach (int num2 in num)
                         {
                             combina.Add(num2);
                         }
                         combina2.Add(combina);
                     }
                     posibilidades.Add(combina2);
                 }
                /* for (int i = registrosCount - 1; i > registrosCount - 3; i--)
                 {
                     List<List<int>> combina2 = new List<List<int>>();
                     var resu = Gerais.Combinations(tipo, i + 1);
                     foreach (var num in resu)
                     {
                         List<int> combina = new List<int>();
                         foreach (int num2 in num)
                         {
                             combina.Add(num2);
                         }
                         combina2.Add(combina);
                     }
                     posibilidades.Add(combina2);
                 }
                */
            }

            return posibilidades;
        }


        


    }


}
