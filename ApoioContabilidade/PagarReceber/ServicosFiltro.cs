using ApoioContabilidade.Models;
using ApoioContabilidade.PagarReceber.ServicesLocais;
using ApoioContabilidade.Services;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.PagarReceber
{
    static public class FiltroPgRc
    {
        static private DataSet dsPagarReceber;
        static public DataSet dsFiltrado;

        static public async Task<bool> PesquisaServidor(DateTime inicio, DateTime fim, string tipoConta, Pesquise oPesquisa)
        {
            bool result = true;
            List<string> lstquery = new List<string>();
            lstquery.Add("Inicio=" + inicio.ToString("yyyy-MM-dd"));
            lstquery.Add("Fim=" + fim.ToString("yyyy-MM-dd"));
            lstquery.Add("TIPO=" + tipoConta);
            lstquery.Add("sp_numero=" + "218");
            try
            {
                dsPagarReceber = await ApiServices.Api_QuerySP(lstquery);
                if ((dsPagarReceber == null) || (dsPagarReceber.Tables.Count == 0))
                {
                    return false;
                }
                dsPagarReceber.Tables[0].TableName = "MOV_PGRC";
                dsPagarReceber.Tables[1].TableName = "MOV_FIN";
                dsPagarReceber.Tables[2].TableName = "MOV_APRO";
                dsPagarReceber.Tables[3].TableName = "CTACENTR";
                dsPagarReceber.Tables[4].TableName = "MOVEST";
                dsPagarReceber.Tables[5].TableName = "VENDAS";
                foreach (DataTable table in dsPagarReceber.Tables)
                {
                    table.AcceptChanges();
                }
            }
            catch (Exception)
            {

                dsPagarReceber = null;
                return false;
            }
            return result;
        }

        static public bool FiltroDadosPagar(Pesquise oPesquisa, PesquisaGenerico pesquisaGenerico, bool filtreDataMestre,
            DateTime inicio, DateTime fim)
        {
            dsFiltrado = new DataSet();
            bool result = true;
            // verifica os filtros detalhes iniciais
            Dictionary<string, string> relacaoFiltroTabela = new Dictionary<string, string>();

            relacaoFiltroTabela.Add("MOV_PGRC", "MESTRE");
            relacaoFiltroTabela.Add("MOV_FIN", "DETALHE1"); // pagamentos // recebimentos
            relacaoFiltroTabela.Add("MOV_APRO", "DETALHE2"); // apropriações contabeis
            relacaoFiltroTabela.Add("CTACENTR", "DETALHECENTRO"); // gerencial
            relacaoFiltroTabela.Add("MOVEST", "");
            relacaoFiltroTabela.Add("VENDAS", "");

            Dictionary<string, string> detalheFiltrado = new Dictionary<string, string>();
            detalheFiltrado.Add("MOV_PGRC", ""); // MOV_ID
            detalheFiltrado.Add("MOV_FIN", "OUTRO_ID"); // pagamentos // recebimentos
            detalheFiltrado.Add("MOV_APRO", "OUTRO_ID"); // apropriações contabeis
            detalheFiltrado.Add("CTACENTR", "MOV_ID"); // gerencial
            detalheFiltrado.Add("MOVEST", "MOV_ID");
            detalheFiltrado.Add("VENDAS", "MOV_ID");
            List<double> lstMovIdMestre = new List<double>();
            Dictionary<string, List<double>> diversosID = new Dictionary<string, List<double>>();
            diversosID.Add("MOV_PGRC", null); // MOV_ID
            diversosID.Add("MOV_FIN", new List<double>()); // pagamentos // recebimentos
            diversosID.Add("MOV_APRO", new List<double>()); // apropriações contabeis
            diversosID.Add("CTACENTR", new List<double>()); // gerencial
            diversosID.Add("MOVEST", null);
            diversosID.Add("VENDAS", null);

            Dictionary<string, bool> temFiltroAtivo = new Dictionary<string, bool>();
            temFiltroAtivo.Add("MOV_PGRC", false);
            temFiltroAtivo.Add("MOV_FIN", false); // pagamentos // recebimentos
            temFiltroAtivo.Add("MOV_APRO", false); // apropriações contabeis
            temFiltroAtivo.Add("CTACENTR", false); // gerencial
            temFiltroAtivo.Add("MOVEST", false);
            temFiltroAtivo.Add("VENDAS", false);



            foreach (KeyValuePair<string, string> tabelafiltro in relacaoFiltroTabela)
            {
                string pageName = tabelafiltro.Value;
                string tabelaNome = tabelafiltro.Key;
                TabPage_Apoio pagina = null;
                if (oPesquisa.TabPages.ContainsKey(pageName))
                {
                    pagina = (oPesquisa.TabPages[pageName] as TabPage_Apoio);
                }
                List<LinhaSolucao> oLista = null;
                if (pagina != null)
                    oLista = pagina.Get_LinhaSolucao();

                // DataSet dataSet = new DataSet();

                if ((oLista != null) && (oLista.Count > 0))
                {

                    Boolean temFiltro = false;

                    foreach (LinhaSolucao linha in oLista)
                    {
                        if (linha.ofuncao != null)
                        {
                            temFiltro = true;

                        }
                        if (temFiltro) break;
                    }
                    if (temFiltro & (temFiltroAtivo.ContainsKey(tabelaNome)))
                    {
                        temFiltroAtivo[tabelaNome] = true;
                    }

                    string campoid = "";
                    if (detalheFiltrado.ContainsKey(tabelaNome))
                    {
                        campoid = detalheFiltrado[tabelaNome];
                    }
                    List<double> estesIds = null;
                    if (diversosID.ContainsKey(tabelaNome))
                    { estesIds = diversosID[tabelaNome]; }
                    DataTable tab = dsPagarReceber.Tables[tabelaNome].Clone();
                    foreach (DataRow orow in dsPagarReceber.Tables[tabelaNome].Rows)
                    {
                        /*  // mecanismo para filtrar perido menor dentro de periodo maior, para não precisar acessar 
                         // 
                         if ((pageName == "MESTRE") && filtreDataMestre)
                         {
                             if ((Convert.ToDateTime(orow["DATA"]).CompareTo(inicio) == -1) ||
                                (Convert.ToDateTime(orow["DATA"]).CompareTo(fim) > 0)) continue;
                         }
                        */

                        Boolean passa = true;
                        try
                        {
                            foreach (LinhaSolucao linha in oLista)
                            {
                                if (linha.ofuncao != null)
                                {
                                    passa = linha.ofuncao(linha, orow, pesquisaGenerico);
                                }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }

                        if (!passa) continue;
                        // para que só aparecam os ids no mestre do que foi filtrado pelo detalhe...
                        if (campoid != "")
                        {
                            double mov_id = Convert.ToDouble(orow[campoid]);
                            if (!estesIds.Contains(mov_id))
                            {
                                estesIds.Add(mov_id);
                            }
                            if (!lstMovIdMestre.Contains(mov_id))
                            {
                                lstMovIdMestre.Add(mov_id);
                            }

                        }

                        tab.Rows.Add(orow.ItemArray);
                        tab.Rows[tab.Rows.Count - 1].AcceptChanges();
                    }
                    tab.AcceptChanges();
                    dsFiltrado.Tables.Add(tab);
                }
                else
                {
                    if (temFiltroAtivo.ContainsKey(tabelaNome))
                    {
                        temFiltroAtivo[tabelaNome] = false;
                    }

                    dsFiltrado.Tables.Add(dsPagarReceber.Tables[tabelaNome].Copy());
                }
            }
            // verifica se houver ids selecionados por detalhe que não estejam nos demais detalhes,
            // será retirado da lista geral do mestre, desde que a condição que estamos testanto é && ( AND logico)
            bool temAlgumFiltrado = false;
            foreach (var filtroAtivo in temFiltroAtivo.AsEnumerable().Where(dic => (dic.Value == true)))
            {
                if (filtroAtivo.Key == "MOV_PGRC") continue;
                temAlgumFiltrado = true;
                break;
            }

            foreach (var lstId in diversosID.AsEnumerable().Where(dic => (dic.Value != null) && (dic.Value.Count > 0)))
            {
                for (int i = 0; i < lstMovIdMestre.Count; i++)
                {
                    if (!lstId.Value.Contains(lstMovIdMestre[i]))
                    {
                        lstMovIdMestre[i] = -1;
                    }
                }
                lstMovIdMestre = lstMovIdMestre.Where(val => val > 0).ToList();
            }
            // A Segunda Verificaçao é se um Detalhe que necessariamente tem FILTRO, e O MESTRE TEM IDS QUE NÃO ESTÁ CONTIDO NELE,
            // é NECESSÁRIO TIRAR DO ID MESTRE
            bool mestreTemFiltro = temFiltroAtivo["MOV_PGRC"];
            foreach (var filtroAtivo in temFiltroAtivo.AsEnumerable().Where(dic => (dic.Value == true)))
            {
                List<double> lstId = null;
                if (!diversosID.ContainsKey(filtroAtivo.Key)) continue;
                lstId = diversosID[filtroAtivo.Key];
                // Se o Mestre tiver filtro e for vazio => nada feito para ninguém               
                if ((filtroAtivo.Key == "MOV_PGRC") && (lstId == null))
                {
                    //lstMovIdMestre.Clear();
                    continue;
                }
                if (lstId == null) continue;
                for (int i = 0; i < lstMovIdMestre.Count; i++)
                {
                    if (!lstId.Contains(lstMovIdMestre[i]))
                    {
                        lstMovIdMestre[i] = -1;
                    }
                }
                lstMovIdMestre = lstMovIdMestre.Where(val => val > 0).ToList();
            }
            if ((temAlgumFiltrado) && (dsFiltrado.Tables["MOV_PGRC"].Rows.Count > 0)) // se tem algum filtrado
            {

                if (lstMovIdMestre.Count > 0)
                {
                    DataTable novoMestre = dsFiltrado.Tables["MOV_PGRC"].AsEnumerable()
                        .Where(row => lstMovIdMestre.Contains(row.Field<double>("MOV_ID"))).CopyToDataTable();
                    novoMestre.TableName = "MOV_PGRC";
                    dsFiltrado.Tables.Remove(dsFiltrado.Tables["MOV_PGRC"]);
                    dsFiltrado.Tables.Add(novoMestre);
                    // = novoMestre.Copy(); 
                }
                else // nenhum filtro carregou algum registro (zerado)
                {
                    // se o mestre tem registros 
                    //   if ((dsFiltrado.Tables["MOV_PGRC"].Rows.Count > 0) && (mestreTemFiltro)) return result;

                    DataTable novoMestre = dsFiltrado.Tables["MOV_PGRC"].Clone();
                    novoMestre.TableName = "MOV_PGRC";
                    dsFiltrado.Tables.Remove(dsFiltrado.Tables["MOV_PGRC"]);
                    dsFiltrado.Tables.Add(novoMestre);
                }
            }
            return result;
        }
    }
    ////////////////////////////////
    ///
    static public class FiltroFinan
    {
        // 
        static public DataSet dsFinanceiro;
        // static public DataSet dsFiltrado;

        static public async Task<bool> PesquisaServidor(DateTime inicio, DateTime fim, Pesquise oPesquisa)
        {
            if ((fim == null) || (fim.CompareTo(inicio) < 0)) fim = inicio;
            string sqlString = "( DATA BETWEEN '" + inicio.ToString("yyyy-MM-dd") + "' AND '" + fim.ToString("yyyy-MM-dd") + "') " +
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

            string receber = "SElect * from mov_fin where " + sqlString +
              " order by data";

            sqlString = "( DATA BETWEEN '" + inicio.ToString("yyyy-MM-dd") + "' AND '" + fim.ToString("yyyy-MM-dd") + "') " +
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

            string pagar = "Select * from mov_fin where " + sqlString +
              " order by data";
            List<string> lst = new List<string>();
            lst.Add(receber);
            lst.Add(pagar);

            bool result = true;
            try
            {
                dsFinanceiro = await ApiServices.Api_QueryMulti(lst);

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
