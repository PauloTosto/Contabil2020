using ApoioContabilidade.Models;
using ApoioContabilidade.PagarReceber.ServicesLocais;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro_MOVFIN.Servicos
{
    public static class ServicosFiltroDBF
    {
        static public class FiltroFinanDBF
        {
            // 
            static public DataSet dsFinanceiro;
            // static public DataSet dsFiltrado;

            static public OleDbDataAdapter adapterMovFin;
           

            static public string path;

            static public bool PesquisaServidor(DateTime inicio, DateTime fim, PesquisaGenerico oPesquisa, List<LinhaSolucao> oList)
            {
                bool ret = true;
                path = TDataControlReduzido.Get_Path("CONTAB");
                if ((fim == null) || (fim.CompareTo(inicio) < 0)) fim = inicio;

                dsFinanceiro = new DataSet();
                DataSet dataSetRec = TDataControlContab.MovFinanceiroFiltro(inicio, fim, oList, oPesquisa, 1, "R");
                DataSet dataSetPag = TDataControlContab.MovFinanceiroFiltro(inicio, fim, oList, oPesquisa, 1, "P");

                if (dataSetRec.Tables.Count > 0)
                {
                    dsFinanceiro.Tables.Add(dataSetRec.Tables[0].Copy());
                    dsFinanceiro.Tables[0].TableName = "RMOV_FIN";
                }

                //dsFinanceiro = await ApiServices.Api_QueryMulti(lst);
                if (dataSetPag.Tables.Count > 0)
                {
                    dsFinanceiro.Tables.Add(dataSetPag.Tables[0].Copy());
                    dsFinanceiro.Tables[1].TableName = "PMOV_FIN";
                }

                return ret;
            }
                static public bool PesquisaServidorVelho(DateTime inicio, DateTime fim, Pesquise oPesquisa)
                {


                    path  = TDataControlReduzido.Get_Path("CONTAB");

                    if ((fim == null) || (fim.CompareTo(inicio) < 0)) fim = inicio;
                    string sqlString = "( DATA BETWEEN CTOD('" + inicio.ToString("MM-dd-yyyy") + "') AND CTOD('" + fim.ToString("MM-dd-yyyy") + "')) " +
                                       "AND (TP_FIN = 1) AND (TIPO = 'R') ";
                    //    List<LinhaSolucao> oLista = oPesquisa.Pagina("Geral").Get_LinhaSolucao();

                    TabPage_Apoio pagina = null;
                    if (oPesquisa.TabPages.ContainsKey("GERAL"))
                    {
                        pagina = (oPesquisa.TabPages["GERAL"] as TabPage_Apoio);
                    }
                    List<LinhaSolucao> oLista = null;
                    if (pagina != null)
                        oLista = pagina.Get_LinhaSolucao();

                   // TDataControlContab.MovFinanceiroFiltro(inicio, fim, oLista, oPesquisa,1,"R");



                    string datastring = "";

                    if (oLista != null)
                    {
                        for (int i = 0; i < oLista.Count; i++)
                        {
                            if (oLista[i].ofuncao == null)
                            {
                                if (oLista[i].ofuncaoSql != null)
                                {
                                    datastring += oLista[i].ofuncaoSql(oLista[i]);
                                }
                                else if (oLista[i].ofuncaoSqlDictionary != null)
                                {
                                    Dictionary<string, string> keyValuePairs = oLista[i].ofuncaoSqlDictionary(oLista[i]);
                                    if (keyValuePairs.ContainsKey("DEBITO"))
                                    {
                                        //if (datastring != "")
                                        datastring += " AND ";
                                        datastring += keyValuePairs["DEBITO"];
                                    }
                                }
                                else
                                    datastring += RetornaCodigoSql.ConstruaSql(oLista[i].campo, oLista[i].dado);
                            }
                        }
                    }
                    if (datastring != "")
                    {
                        sqlString = sqlString + datastring;
                        datastring = "";
                    }

                    string receber = "SElect recno() AS nreg,* from " + path+"MOVFIN where " + sqlString +
                      " order by data";

                    sqlString = "( DATA BETWEEN CTOD('" + inicio.ToString("MM-dd-yyyy") + "') AND CTOD('" + fim.ToString("MM-dd-yyyy") + "')) " +//"( DATA BETWEEN '" + inicio.ToString("yyyy-MM-dd") + "' AND '" + fim.ToString("yyyy-MM-dd") + "') " +
                                       "AND (TP_FIN = 1) AND (TIPO = 'P') ";
                    datastring = "";

                    if (oLista != null)
                    {
                        for (int i = 0; i < oLista.Count; i++)
                        {
                            if (oLista[i].ofuncao == null)
                            {
                                if (oLista[i].ofuncaoSql != null)
                                {
                                    datastring += oLista[i].ofuncaoSql(oLista[i]);
                                }
                                else if (oLista[i].ofuncaoSqlDictionary != null)
                                {
                                    Dictionary<string, string> keyValuePairs = oLista[i].ofuncaoSqlDictionary(oLista[i]);
                                    if (keyValuePairs.ContainsKey("CREDITO"))
                                    {
                                        datastring += " AND ";
                                        datastring += keyValuePairs["CREDITO"];
                                    }
                                }
                                else
                                    datastring += RetornaCodigoSql.ConstruaSql(oLista[i].campo, oLista[i].dado);
                            }
                        }
                    }
                    if (datastring != "")
                    {
                        sqlString = sqlString + datastring;
                        datastring = "";
                    }

                    string pagar = "Select recno() AS nreg,* from " + path+"MOVFIN where " + sqlString +
                      " order by data";
                    List<string> lst = new List<string>();
                    lst.Add(receber);
                    lst.Add(pagar);

                    bool result = true;
                    try
                    {

                        try
                        {
                            adapterMovFin = TDataControlContab.ConstruaAdaptador_PTMovFin("MOVFIN");
                        }
                        catch (Exception E)
                        {
                            MessageBox.Show("Falha no Acesso MOVFIN  " + E.Message);
                            return false;
                        }


                         dsFinanceiro = new DataSet();
                        //DataSet ptdataset = new DataSet();
                        //     setoroledb = "SELECT *  FROM " + path + tabela;
                        OleDbCommand oledbcomm = new OleDbCommand(receber, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        //OleDbDataAdapter oledbda = TDataControlContab.GravePtMovFin(tabela);
                        adapterMovFin.SelectCommand = oledbcomm;
                        adapterMovFin.TableMappings.Add("Table", "RMOV_FIN");
                        adapterMovFin.Fill(dsFinanceiro);

                        oledbcomm = new OleDbCommand(pagar, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        //OleDbDataAdapter oledbda = TDataControlContab.GravePtMovFin(tabela);
                        adapterMovFin.SelectCommand = oledbcomm;
                        adapterMovFin.TableMappings.Clear();
                        adapterMovFin.TableMappings.Add("Table1", "PMOV_FIN");
                        DataSet dataSet = new DataSet();
                        adapterMovFin.Fill(dataSet);

                        if (dataSet.Tables.Count > 0)
                        {
                            dsFinanceiro.Tables.Add(dataSet.Tables[0].Copy());
                        }

                        //dsFinanceiro = await ApiServices.Api_QueryMulti(lst);

                        dsFinanceiro.Tables[0].TableName = "RMOV_FIN";
                        dsFinanceiro.Tables[1].TableName = "PMOV_FIN";
                        foreach (DataTable table in dsFinanceiro.Tables)
                        {
                            table.AcceptChanges();
                        }
                    }
                    catch (Exception)
                    {

                        dsFinanceiro = null;
                        return false;
                    }
                    return result;
                }
            }

        }
}
