using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Trabalho.ServicesTrab
{



    public class AtualizaFolha_Cli
    {
        string Tipo = "";
        List<string> tabCodigo;
        List<string> tabGratif, listaTrab, listaTarefa;
        string diretoriobase;
        string dircampotrabalho, direscritorestoque;
        DateTime inicio, fim;
        double Perc_Adiant = 40, SalMin = 0, FGTS = 0;
        List<int> campo_Mov;
        DataTable adodataset, adoTabSal, adoTabInss, adoTabSalFam, adoTabIRDedu, adoTabIR, adoCLTReaj, adocltponto, adoSEFIP;
        ModeloClass oModelo;
        ServicoClass oServico;
        public AtualizaFolha_Cli()
        {

            oModelo = new ModeloClass(TabelasIniciais_Trab.dsTabelasTrab.Tables["MODELO"].Copy());
            oServico = new ServicoClass(TabelasIniciais_Trab.dsTabelasTrab.Tables["SERVICO"].Copy());
            oModelo.cliente = "C";
            oServico.cliente = "C";
        }
      
        public bool Gera_Arquivo()
        {
            bool result = true;
            adoTabSal = TabelasIniciais_Trab.dsTabelasTrab.Tables["TABSAL"].Copy();
            adoTabInss = TabelasIniciais_Trab.dsTabelasTrab.Tables["TABINSS"].Copy();
            adoTabIR = TabelasIniciais_Trab.dsTabelasTrab.Tables["TABIR"].Copy();
            adoTabIRDedu = TabelasIniciais_Trab.dsTabelasTrab.Tables["IRDEDU"].Copy();
            adoTabSalFam = TabelasIniciais_Trab.dsTabelasTrab.Tables["SALFAM"].Copy();
            adoCLTReaj = TabelasIniciais_Trab.dsTabelasTrab.Tables["CLTREAJ"].Copy();
            return result;
        }
        public Dictionary<string,CltCodigo> Enche_TabCodigo()
        {
            Dictionary<string, CltCodigo> lst = new Dictionary<string, CltCodigo>();
            foreach(DataRow orow in TabelasIniciais_Trab.dsTabelasTrab.Tables["CLTCODIG"].Rows)
            {
                CltCodigo ocod = new CltCodigo();
                ocod.IndCod = orow["INDCOD"].ToString();
                ocod.DescCod = orow["DESCCOD"].ToString();
                
                ocod.Diarias = Convert.ToDouble(orow["FATOR"]);
               // if (ocod.IndCod.Substring(0, 1) != "Y")
                    ocod.Horas_Efetivas = Convert.ToDouble(orow["HORAS"]);
                //else
                  //  ocod.Horas_Efetivas = Convert.ToDouble(orow["FATOR"]);
                ocod.Horas50 = Convert.ToDouble(orow["X_HORAS"]);
                ocod.Horas100 = Convert.ToDouble(orow["NOTURNO"]);
                ocod.HorasFe = Convert.ToDouble(orow["H_NORMAL"]);
                ocod.Compati = orow["COMPATI"].ToString();
                lst.Add(ocod.IndCod, ocod);
            }
            return lst;
        }





    }

    public class ModeloClass
    {
        /*string str = "SELECT  num, prod, tipo, cod, seq, modelo.DESCRI as descricao, cult " +
           " FROM  modelo " +
           " where (alltrim(modelo.DESCRI) <> '') AND ( ALLTRIM(NUM) <> '') ";
        */
        private DataTable tabmodelo;
        public string cliente = "C";
        public ModeloClass(DataTable modelo)
        {
            tabmodelo = modelo.AsEnumerable().CopyToDataTable();
        }

        string ModeloNatureza(string tcod)
        {
            tabmodelo.AsDataView().RowFilter = " (NUM = '" + tcod + "') " +
                                         " AND (COD ='') ";

            string result = "Sem Definição";
            if (tabmodelo.AsDataView().Count > 0)
            {
                DataRowView orow = tabmodelo.AsDataView()[0];
                if (orow["TIPO"].ToString() == "I")
                    result = "INVESTIMENTO ";
                else if (orow["TIPO"].ToString() == "M")
                    result = "MANUT.INVEST.";
                else if (orow["TIPO"].ToString() == "C")
                    result = "CUSTEIO      ";
                else if (orow["TIPO"].ToString() == "R")
                    result = "RECEITA      ";
                else if (orow["TIPO"].ToString() == "T")
                    result = "TRANSFERENCIA";
                else if (orow["TIPO"].ToString() == "P")
                    result = "CUST.PARCEIRO";
                else if (orow["TIPO"].ToString() == "O")
                    result = "CUST.OUTORGAN";
                else
                    result = "Sem Definição";
            }
            tabmodelo.AsDataView().RowFilter = "";
            return result;
        }
        string ModeloProduto(string tcod)
        {
            tabmodelo.AsDataView().RowFilter = " (NUM = '" + tcod + "') " +
                                         " AND (COD ='') ";
            string result = "";
            if (tabmodelo.AsDataView().Count > 0)
            {
                DataRowView orow = tabmodelo.AsDataView()[0];
                result = orow["CULT"].ToString();
            }
            tabmodelo.AsDataView().RowFilter = "";
            return result;
        }
        string ModeloCultura(string tcod)
        {
            tabmodelo.AsDataView().RowFilter = " (NUM = '" + tcod + "') " +
                                         " AND (COD ='') ";

            string result = "";
            if (tabmodelo.AsDataView().Count > 0)
            {
                DataRowView orow = tabmodelo.AsDataView()[0];
                result = orow["PROD"].ToString();
            }
            tabmodelo.AsDataView().RowFilter = "";
            return result;
        }
    }

    public class ServicoClass
    {
        /*string str = "SELECT * " +
           " FROM  Servic "; */
        private DataTable tabservico;
        public string cliente = "C";
        public ServicoClass(DataTable servico)
        {
            tabservico = servico.AsEnumerable().CopyToDataTable();
        }

        string servicoUnid(string tcod)
        {
            tabservico.AsDataView().RowFilter = " (COD = '" + tcod + "') ";

            string result = "";
            if (tabservico.AsDataView().Count > 0)
            {
                DataRowView orow = tabservico.AsDataView()[0];
                result = orow["UNID"].ToString();
            }
            tabservico.AsDataView().RowFilter = "";
            return result;
        }
        string servicoDescri(string tcod)
        {
            tabservico.AsDataView().RowFilter = " (COD = '" + tcod + "') ";
            string result = "";
            if (tabservico.AsDataView().Count > 0)
            {
                DataRowView orow = tabservico.AsDataView()[0];
                result = orow["DESCRI"].ToString();
            }
            tabservico.AsDataView().RowFilter = "";
            return result;
        }

    }
    static public class TabelasIniciais_Trab
    {

        static public DataSet dsTabelasTrab;
        static public bool inprocess = false;
        static public Dictionary<string, bool> atualizado = new Dictionary<string, bool>();
        static private List<string> sql;
        static private List<string> nometabela;
        static public async Task<bool> Execute()
        {
            inprocess = true;
            sql = new List<string>();
            nometabela = new List<string>();
            string str = "SELECT * FROM SERVIC";
            sql.Add(str);
            nometabela.Add("SERVICO");
            str = "SELECT NUM,SEQ,COD, Modelo.DESCRI as Descricao, TIPO,PROD, CULT FROM MODELO " +
                 "WHERE MODELO.DESCRI <> '' AND ( ALLTRIM(NUM) <> '') ORDER BY DESCRICAO";
            sql.Add(str);
            nometabela.Add("MODELO");

            str = "SELECT  data, valor, fgts FROM TABSAL WHERE (valor <> 0) ORDER BY DATA DESC";
            sql.Add(str);
            nometabela.Add("TABSAL");

            str = "SELECT data, faixa, valor1, valor2, taxa FROM TABINSS  ORDER BY DATA DESC,faixa";
            sql.Add(str);
            nometabela.Add("TABINSS");

            str = "SELECT  data, faixa, base1, base2, aliq, deduzir FROM TABIR  ORDER BY DATA DESC,faixa";
            sql.Add(str);
            nometabela.Add("TABIR");

            str = "SELECT * FROM IRDEDU ORDER BY DATA DESC";
            sql.Add(str);
            nometabela.Add("IRDEDU");

            str = "SELECT data, faixa, valor1, valor2, salfam FROM SALFAM ORDER BY DATA DESC,faixa";
            sql.Add(str);
            nometabela.Add("SALFAM");


            str = "SELECT * FROM REAJUSTE AS CLTREAJ order by data";
            sql.Add(str);
            nometabela.Add("CLTREAJ");

            str = "SELECT * FROM CLTCODIG WHERE INDCOD IS NOT NULL";
            sql.Add(str);
            nometabela.Add("CLTCODIG");



            str = "SELECT * FROM GRATIF WHERE (CODSER IS NOT NULL) AND (VLRDIARIA IS NOT NULL) AND (DTA_INI IS NOT NULL)";
            sql.Add(str);
            nometabela.Add("GRATIF");


            str = "SELECT ID,CODCAD,NOMECAD,SETOR,ADMI,DEMI,PRAZO,SALBASE,OPCAO,AVULSO,MENSALISTA FROM CLTCAD WHERE id IS null";
            sql.Add(str);
            nometabela.Add("CLTCAD_STRUCT");

            str = "SELECT  obssefip.id, obssefip.cod, obssefip.data, obssefip.tpsefip, obssefip.entsaisefi, obssefip.obs, tabsefip.tipo, "
           + " obssefip.data_sai,obssefip.num_prev  FROM            obssefip, tabsefip "
           + " WHERE        obssefip.tpsefip = tabsefip.cod " +
           " ORDER BY obssefip.cod, obssefip.data DESC,TIPO";
            sql.Add(str);
            nometabela.Add("OBSSEFIP");



            bool result = true;
            try
            {
                atualizado.Clear();
                dsTabelasTrab = await ApiServices.Api_QueryMulti(sql);
                if (dsTabelasTrab.Tables.Count == nometabela.Count)
                {
                    for (int i = 0; i < dsTabelasTrab.Tables.Count; i++)
                    {
                        dsTabelasTrab.Tables[i].TableName = nometabela[i].ToUpper().Trim();
                        atualizado.Add(nometabela[i].ToUpper().Trim(), true);
                    }
                }
                inprocess = false;
            }
            catch (Exception E)
            {

                result = false;
            }

            return result;
        }
        static public async Task<bool> AtualizeTabela(string tabelaNome)
        {
            if (!nometabela.Contains(tabelaNome.ToUpper().Trim())) return false;
            int index = nometabela.IndexOf(tabelaNome.ToUpper().Trim());
            string str = sql[index];
            DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[] { str }));
            if ((odataset != null) && (odataset.Tables != null) && (odataset.Tables.Count > 0))
            {
                dsTabelasTrab.Tables[index].Rows.Clear();
                foreach (DataRow orow in odataset.Tables[0].Rows)
                {
                    DataRow orowNew = dsTabelasTrab.Tables[index].NewRow();
                    orow.ItemArray.CopyTo(orowNew.ItemArray, 0);
                    dsTabelasTrab.Tables[index].Rows.Add(orowNew);
                }
                return true;
            }
            else return false;

        }
        static public bool TabelasIniciaisOk()
        {
            bool retorno = false;
            if ((dsTabelasTrab == null) || (dsTabelasTrab.Tables.Count < 0) || inprocess)
            {
                return retorno;
            }
            retorno = true;

            return retorno;
        }
        static public DateTime UltimoDiaMes(DateTime data)
        {
            DateTime result =
                new DateTime(data.Year, data.Month, 01).AddMonths(1).AddDays(-1);

            return result;
        }
        static public DateTime PrimeiroDiaMes(DateTime data)
        {
            DateTime result =
                new DateTime(data.Year, data.Month, 01);

            return result;
        }

    }
}
