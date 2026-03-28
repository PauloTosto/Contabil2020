using ApoioContabilidade.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.ServiceModel.Configuration;
using System.Threading.Tasks;

namespace ApoioContabilidade.Services
{
    public static class TabelasIniciais
    {
        static private DataSet dsTabelasIniciais;
        static private bool inprocess = false;
        static public Dictionary<int, string> certif = new Dictionary<int, string>();
        static public Dictionary<int, string> complemento = new Dictionary<int, string>();

        static public async Task<bool> Execute()
        {
            certif = new Dictionary<int, string>();

            certif.Add(0, "NÃO CERTIFICADO");
            certif.Add(1, "RAIN FOREST    ");
            certif.Add(2, "UTZ            ");
            certif.Add(3, "RAIN FOREST/UTZ");
            certif.Add(99, "INDIFERENTE    ");
            complemento = new Dictionary<int, string>();

            complemento.Add(0, "VENDA      ");
            complemento.Add(1, "COMPLEMENTO");

            inprocess = true;
            List<string> sql = new List<string>();
            List<string> nometabela = new List<string>();
            string str = "SELECT NUM,SEQ,MODELO.COD,MODELO.DESCRI as Descricao,substring(Serv.Cod,1,3) as Codserv,"
            + " SERV.DESCRI as DescServ,MODELO.TIPO,modelo.PROD FROM MODELO, " +
            " SERVIC SERV WHERE  (MODELO.COD = substring(SERV.COD,1,3) ) " +
            " AND  ( MODELO.COD <> '' ) order by SERV.DESCRI";
            sql.Add(str);
            nometabela.Add("SERVICOMODELO");

            str = "SELECT * FROM CATPROD"; // CATPROD
            sql.Add(str);// 
            nometabela.Add("CATPROD");
            str = "SELECT NUM,SEQ,COD, Modelo.DESCRI as Descricao, TIPO,PROD, CULT FROM MODELO " +
                   "WHERE MODELO.DESCRI <> '' AND ( ALLTRIM(NUM) <> '') ORDER BY DESCRICAO;";

            sql.Add(str);//
            nometabela.Add("MODELO");
            str = "SELECT COD, NOME_DEP FROM ARMAZEM.DBF " +
                " WHERE  (COD IS NOT NULL) and (NOME_DEP is not NULL) ORDER BY COD";
            sql.Add(str); //
            nometabela.Add("ARMAZEM");
            str = "Select NBANCO, NOME_BANCO, DESC2,CONTAB FROM Bancos";
            sql.Add(str); // 
            nometabela.Add("BANCOS");
            str = "SELECT DESC2, NUMCONTA,DESCRICAO FROM  Placon " +
                    "WHERE  (alltrim(DESC2) <> '') and " +
                " (alltrim(DESCRICAO) <> '') and " + " (Numconta not LIKE '%000')" +
                  " ORDER BY DESC2";
            sql.Add(str); // 
            nometabela.Add("PLACON");
            str = "SELECT DESC2, NUMCONTA,DESCRICAO FROM  Placon " +
                    "WHERE  (alltrim(DESC2) <> '') and " +
                " (alltrim(DESCRICAO) <> '') and " + " (Numconta LIKE '%000')" +
                  " ORDER BY DESC2";
            sql.Add(str); // 
            nometabela.Add("PLACON_SINTETICO");

            str = "SELECT * FROM FORN_CLI " +
              "WHERE  SUBS(NUMCONTA,1,3) = '211' and SUBS(NUMCONTA,6,3) <> '000' ";
            sql.Add(str); // 
            nometabela.Add("FornCli");
            str = "SELECT * FROM TIPOMOV WHERE ALLTRIM(DESCRICAO) <> '' ";
            sql.Add(str); // 
            nometabela.Add("TipoMov");

            str = "SELECT * FROM TIPOPROD WHERE ALLTRIM(DESCRICAO) <>  '' ";
            sql.Add(str); // 
            nometabela.Add("TIPOPROD");



            str = "SELECT id, cod, DESCRI as descr, forn, unid, quant, pmedio, punit, data_cad, data, valor, tp_mat FROM CADEST";
            sql.Add(str);//
            nometabela.Add("CADEST");
            
            str = "SELECT COD,contabil,DESCRI,CNPJ FROM FIRMA";
            sql.Add(str); //Firmas
            nometabela.Add("FIRMA");

            str = "SELECT * FROM CAVALO";
            sql.Add(str);
            nometabela.Add("CAVALO");

            str = "Select id, substring(cod,1,3) COD, descri, unid, tipo, indice, apagado, nindice from Servic where (alltrim(DESCRI) <> '') ";
            sql.Add(str);
            nometabela.Add("SERVICOONLY");

            str = "SELECT * FROM SETORES where (alltrim(DESCRI) <> '') and (ALLTRIM(DTOS(FIM)) = '') order by cod";
            sql.Add(str);
            nometabela.Add("setores");

            str = "SELECT * FROM NEGOCIO";
            sql.Add(str);
            nometabela.Add("Negocio");


            str = "SELECT * FROM GLEBAS WHERE (ALLTRIM(DTOS(FIM)) = '') AND (ALLTRIM(COD) <> '' ) ORDER BY SETOR,COD";
            sql.Add(str);
            nometabela.Add("GLebas");

            str = "SELECT * FROM Quadras where  (ALLTRIM(DTOS(FIM)) = '') AND (ALLTRIM(QUA) <> '' ) ORDER BY GLEBA,QUA";
            sql.Add(str);
            nometabela.Add("QUADRAS");

            str = "SELECT * FROM ClassMat order by nome_mat";
            sql.Add(str);
            nometabela.Add("ClassMat");

            str = "SELECT * FROM CULTURA";
            sql.Add(str);
            nometabela.Add("CULTURA");

            str = "SELECT * FROM VARIED";
            sql.Add(str);
            nometabela.Add("Varied");

            str = "SELECT* FROM CATEGORIA ORDER BY DESCRICAO"; // categoria trabalho
            sql.Add(str);
            nometabela.Add("CATEGORIA");

            str = "SELECT * FROM TABSefip Where(descricao<> '') and cod<> '' ORDER BY COD ";
            sql.Add(str);
            nometabela.Add("TABSEFIP");


           






            bool result = true;
            try
            {
                dsTabelasIniciais = await ApiServices.Api_QueryMulti(sql);
                if (dsTabelasIniciais.Tables.Count == nometabela.Count)
                {
                    for (int i = 0; i < dsTabelasIniciais.Tables.Count; i++)
                    {
                        dsTabelasIniciais.Tables[i].TableName = nometabela[i].ToUpper().Trim();
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
        static public DataSet DsTabelasInciais()
        {
            return dsTabelasIniciais;
        }
        static public bool TabelasIniciaisOk()
        {
            bool retorno = false;
            if ((dsTabelasIniciais == null) || (dsTabelasIniciais.Tables.Count < 0) || inprocess)
            {
                return retorno;
            }
            retorno = true;

            return retorno;
        }




        

         
        public static string AcheEmissor(string desc2)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("PLACON")) || (desc2 == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("DESC2").Trim() == desc2.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["NUMCONTA"].ToString(); }
            return result;
        }
        public static string Ache_Desc2(string numconta)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("PLACON")) || (numconta == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("numconta").Trim() == numconta.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESC2"].ToString(); }
            return result;
        }
       static public bool Existe_Desc2(string desc2)
        {
            bool result = false; ;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("PLACON")) || (desc2 == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("DESC2").Trim() == desc2.Trim()).FirstOrDefault();
            if (orow != null) { result = true; }
            return result;
        }
        static public bool Existe_Banco(string nbanco)
        {
            bool result = false;
            int numbanco = 0;
            try
            {
                numbanco = Convert.ToInt32(nbanco.Trim());
                if (numbanco == 0) return true;
            }
            catch { return false; }

            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("BANCOS")) || (numbanco < 0)) return result;
            try
            {
                DataRow orow = dsTabelasIniciais.Tables["BANCOS"].AsEnumerable().Where(row => row.Field<double>("NBANCO") == numbanco).FirstOrDefault();
                if (orow != null) { result = true; }

            }
            catch (Exception )  {              
               
            }
            return result;
        }




        public static string Ache_Descricao(string numconta)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("PLACON")) || (numconta == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("numconta").Trim() == numconta.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRICAO"].ToString(); }
            return result;
        }

        public static string NomeDaFirma()
        {
            string result = "";
            string numconta = "00000000";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("PLACON")) || (numconta == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["PLACON_SINTETICO"].AsEnumerable().Where(row => row.Field<string>("numconta").Trim() == numconta.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRICAO"].ToString(); }
            return result;
        }

        public static string ModeloDesc(string num)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("MODELO")) || (num == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["MODELO"].AsEnumerable().Where(row => row.Field<string>("NUM").Trim() == num.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRICAO"].ToString(); }
            return result;
        }
        public static string ModeloCod(string pesquise)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("MODELO")) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["MODELO"].AsEnumerable().
                Where(row => row.Field<string>("DESCRICAO").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["NUM"].ToString(); }
            return result;
        }
        public static string ModeloProduto(string pesquise)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("MODELO")) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["MODELO"].AsEnumerable().
                Where(row => row.Field<string>("NUM").Trim() == pesquise.Trim() && row.Field<string>("COD").Trim() == "").FirstOrDefault();
            if (orow != null) { result = orow["CULT"].ToString(); }
            return result;
        }
        public static string ModeloCultura(string pesquise)
        {
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains("MODELO")) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables["MODELO"].AsEnumerable().
                Where(row => row.Field<string>("NUM").Trim() == pesquise.Trim() && row.Field<string>("COD").Trim() == "").FirstOrDefault();
            if (orow != null) { result = orow["PROD"].ToString(); }
            return result;
        }
        public static string TipoMovDesc(double pesquise)
        {
            string tabela = "TIPOMOV";
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == 0)) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<double>("TPMOV") == pesquise).FirstOrDefault();
            if (orow != null) { result = orow["DESCRICAO"].ToString(); }
            return result;
        }
        public static int TipoMovTPMOV(string pesquise)
        {
            string tabela = "TIPOMOV";
            int result = 0;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise.Trim() == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("DESCRICAO").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = Convert.ToInt32(orow["TPMOV"]); }
            return result;
        }
        public static int TipoProdTipo(string pesquise)
        {
            string tabela = "TIPOPROD";
            int result = 0;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise.Trim() == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("DESCRICAO").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = Convert.ToInt32(orow["COD"]); }
            return result;
        }
        public static string TipoProdDescricao(int pesquise)
        {
            string tabela = "TIPOPROD";
            string result = "";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela))) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<double>("COD") == pesquise).FirstOrDefault();
            if (orow != null) { result = orow["DESCRICAO"].ToString(); }
            return result;
        }
        public static string ServicoDesc(string pesquise)
        {
            string result = "";
            string tabela = "SERVICOONLY";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString(); }
            return result;
        }
        public static string ServicoCod(string pesquise)
        {
            string result = "";
            string tabela = "SERVICOONLY";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("DESCRI").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["COD"].ToString(); }
            return result;
        }
        public static string ServicoUnid(string pesquise)
        {
            string result = "";
            string tabela = "SERVICOONLY";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["UNID"].ToString(); }
            return result;
        }
        // CATPROD  catalogo de produtos
        public static string ProdutoDesc(string pesquise)
        {
            string result = "";
            string tabela = "CATPROD";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString(); }
            return result;
        }

        public static string ProdutoUnid(string pesquise)
        {
            string result = "";
            string tabela = "CATPROD";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["UNID_BENE"].ToString(); }
            return result;
        }

        public static string ProdutoDesc_Cod(string pesquise)
        {
            string result = "";
            string tabela = "CATPROD";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("DESCRI").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["COD"].ToString(); }
            return result;
        }


        // CADEST    (CADASTRO MATERIAIS EM ESTOQUE)
        public static string EstoqueDesc(string pesquise)
        {
            string result = "";
            string tabela = "CADEST";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCR"].ToString(); }
            return result;
        }
        public static string EstoqueUnid(string pesquise)
        {
            string result = "";
            string tabela = "CADEST";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["UNID"].ToString(); }
            return result;
        }

        public static string Estoque_TpMat(string pesquise)
        {
            string result = "";
            string tabela = "CADEST";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["TP_MAT"].ToString(); }
            return result;
        }

        public static string EstoqueDesc_Unid(string pesquise)
        {
            string result = "";
            string tabela = "CADEST";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCR"].ToString().Trim() + "(" + orow["UNID"].ToString().Trim() + ")"; }
            return result;
        }
        // SETORES
        public static string SetorDesc(string pesquise)
        {
            string result = "";
            string tabela = "SETORES";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString().Trim(); }
            return result;
        }

        public static int NegocioAtual(string pesquise)
        {
            string tabela = "SETORES";
            int result = 0;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise.Trim() == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = Convert.ToInt32(orow["NEGOCIO"]); }
            return result;
        }

        // GLEBAS
        public static string GlebasDesc(string pesquise)
        {
            string result = "";
            string tabela = "GLEBAS";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString().Trim(); }
            return result;
        }
        public static string SetorAtual(string pesquise)
        {
            string result = "";
            string tabela = "GLEBAS";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("COD").Trim() == pesquise.Trim()) && row.IsNull("FIM")).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString().Trim(); }
            return result;
        }
        public static string SetorAtual_PelaData(string pesquise, DateTime tdata)
        {
            string result = "";
            string tabela = "GLEBAS";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            List<DataRow> orows = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("COD").Trim() == pesquise.Trim() && row.IsNull("FIM")) ||
                ((row.Field<string>("COD").Trim() == pesquise.Trim()) &&
                 (row.Field<DateTime>("FIM").CompareTo(tdata) >= 0) &&
                 (row.Field<DateTime>("INICIO").CompareTo(tdata) <= 0))).OrderBy(orow => orow.Field<DateTime>("INICIO")).ToList();
            foreach (DataRow orow in orows)
            {
                if (orow.IsNull("FIM") || (Convert.ToDateTime(orow["FIM"]).CompareTo(tdata) >= 0))
                {
                    result = orow["SETOR"].ToString();
                    break;
                }
            }

            return result;
        }

        public static int ClassMat(string pesquise)
        {
            string tabela = "CLASSMAT";
            int result = 0;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise.Trim() == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => row.Field<string>("COD").Trim() == pesquise.Trim()).FirstOrDefault();
            if (orow != null) { result = Convert.ToInt32(orow["ICODSER"]); }
            return result;
        }
        public static string DescClassMat(string pesquise)
        {
            string result = "";
            string tabela = "CLASSMAT";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("COD").Trim() == pesquise.Trim())).FirstOrDefault();
            if (orow != null) { result = orow["NOME_MAT"].ToString().Trim(); }
            return result;
        }
        public static string CodClassMat(string pesquise)
        {
            string result = "";
            string tabela = "CLASSMAT";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("NOME_MAT").Trim() == pesquise.Trim())).FirstOrDefault();
            if (orow != null) { result = orow["COD"].ToString().Trim(); }
            return result;
        }
        // FIRMA
        public static string NomeFirma(string pesquise)
        {
            string result = "";
            string tabela = "FIRMA";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("CONTABIL").Trim() == pesquise.Trim())).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString().Trim(); }
            return result;
        }

        public static string NomeBanco(string pesquise)
        {
            string result = "";
            string tabela = "BANCOS";
            Int32 numbanco = -1;
            try
            {
                numbanco = Convert.ToInt32(pesquise.Trim());
            }
            catch (Exception)
            {
                return result;
            }
            if (numbanco == 0) return result;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela))) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<double>("NBANCO") == Convert.ToDouble(numbanco))).FirstOrDefault();
            if (orow != null) { result = orow["NOME_BANCO"].ToString().Trim(); }
            return result;
        }
        public static string Desc2Banco(string pesquise)
        {
            string result = "";
            string tabela = "BANCOS";
            Int32 numbanco = -1;
            try
            {
                numbanco = Convert.ToInt32(pesquise.Trim());
            }
            catch (Exception)
            {
                return result;
            }
            if (numbanco == 0) return result;
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela))) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<double>("NBANCO") == Convert.ToDouble(numbanco))).FirstOrDefault();
            if (orow != null) { result = orow["DESC2"].ToString().Trim(); }
            return result;
        }
        public static string NBancoDesc2(string pesquise)
        {
            string result = "";
            if (pesquise.Length < 2) return result;
            string tabela = "BANCOS";
            string desc2;
            try
            {
                desc2 = pesquise.Substring(0,1) == "*" ? pesquise.Substring(1) : pesquise ;
            }
            catch (Exception)
            {
                return result;
            }
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela))) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("DESC2").Trim() == desc2.Trim())).FirstOrDefault();
            if ((orow != null) && !orow.IsNull("NBANCO"))  { result = Convert.ToString(orow["NBANCO"]).Trim().PadLeft(2,Convert.ToChar("0")); }
            return result;
        }

        public static string CertificadoDesc(int tcod)
        {
            string result = tcod.ToString();
            if (certif.Keys.Contains(tcod)) result = certif[tcod];
            return result;
        }
        public static int CertificadoDesc_N(string tcod)
        {
            int result = -1;
            if (certif.Values.Contains(tcod))
            {
                KeyValuePair<int,string> key = certif.Where(valor => valor.Value.Trim() == tcod.Trim()).FirstOrDefault();
                result = key.Key;
            }
            return result;
        }

        public static string ComplementoDesc(int tcod)
        {
            string result = tcod.ToString();
            if (complemento.Keys.Contains(tcod)) result = complemento[tcod];
            return result;
        }

        public static int Comlemento_N(string tcod)
        {
            int result = -1;
            if (complemento.Values.Contains(tcod))
            {
                KeyValuePair<int, string> key = complemento.Where(valor => valor.Value.Trim() == tcod.Trim()).FirstOrDefault();
                result = key.Key;
            }
            return result;
        }

        /*
         * /// / CERTIFICAÇÂO

function CertificadoDesc(tcod: integer): string;
var
  Lista: TStrings;
begin
  result := inttostr(tcod);
  Lista := TStringList.Create;
  Lista.Add('NAO CERTIFICADO');
  Lista.Add('RAIN FOREST    ');
  Lista.Add('UTZ            ');
  Lista.Add('RAINFOREST/UTZ ');
  Lista.Add('INDIFERENTE    ');
  if tcod < 0 then
    exit;
  if tcod = 99 then
    tcod := (Lista.Count - 1);
  result := Lista[tcod];
end;

function DESC_NCERTIF(tcod: String): integer;
var
  Lista: TStrings;
begin

  Lista := TStringList.Create;
  Lista.Add('NAO CERTIFICADO');
  Lista.Add('RAIN FOREST');
  Lista.Add('UTZ');
  Lista.Add('RAINFOREST/UTZ');
  Lista.Add('INDIFERENTE');
  result := Lista.IndexOf(trim(uppercase(tcod)));
  if result = (Lista.Count - 1) then
    result := 99;

end;

function ComplementoDesc(tcod: integer): string;
var
  Lista: TStrings;
begin
  result := inttostr(tcod);
  Lista := TStringList.Create;
  Lista.Add('VENDA');
  Lista.Add('COMPLEMENTO');
  if tcod < 0 then
    exit;
  result := Lista[tcod];
end;

function ComplementoDESC_N(tcod: String): integer;
var
  Lista: TStrings;
begin

  Lista := TStringList.Create;
  Lista.Add('VENDA');
  Lista.Add('COMPLEMENTO');
  result := Lista.IndexOf(trim(uppercase(tcod)));

end;

function CavaloCulturaDesc(tcod, tcult: string): string;
var
  i: integer;
begin
  result := '';
  if trim(tcod) = '' then
    exit;

  i := DmAdoFinan.AtualizaQueries.IndexOf('CAVALO');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := '(COD = ''' + tcod + ''') AND (CULTURA = ''' + tcult + ''' )';
    Filtered := true;
    if RecordCount > 0 then
      result := FieldByName('DESCRI').AsString;
    Filtered := false;
  end;

end;

function ModeloNatureza(tcod: string): string;
var
  str, ttipo: string;
  i: integer;
begin
  result := '';
  if trim(tcod) = '' then
    exit;
  ttipo := '';
  i := DmAdoFinan.AtualizaQueries.IndexOf('MODELO');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := '(NUM = ''' + tcod + ''') AND (COD = '''' )';
    Filtered := true;
    if RecordCount > 0 then
      ttipo := FieldByName('TIPO').AsString;
    Filtered := false;
  end;
  if trim(ttipo) = 'I' then
    result := 'INVESTIMENTO '
  else if trim(ttipo) = 'M' then
    result := 'MANUT.INVEST.'
  else if trim(ttipo) = 'C' then
    result := 'CUSTEIO      '
  else if trim(ttipo) = 'R' then
    result := 'RECEITA      '
  else if trim(ttipo) = 'T' then
    result := 'TRANSFERENCIA'
  else if trim(ttipo) = 'P' then
    result := 'CUST.PARCEIRO'
  else if trim(ttipo) = 'O' then
    result := 'CUST.OUTORGAN'
  else if trim(ttipo) = '' then
    result := 'Sem Definição';
end;

// VARIED
function VariedadeCulturaDesc(tcod, tcult: string): string;
var
  i: integer;
begin
  result := '';
  if trim(tcod) = '' then
    exit;

  i := DmAdoFinan.AtualizaQueries.IndexOf('VARIED');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := '(COD = ''' + tcod + ''') AND ' + ' (CULTURA = ''' +
      tcult + ''')';
    Filtered := true;
    if RecordCount > 0 then
      result := FieldByName('DESCRI').AsString;
    Filtered := false;
  end;

end;

// VARIED
function VariedadeCulturaDescNovo(tcod, tcult: string): string;
var
  i: integer;
begin
  result := '';
  if trim(tcod) = '' then
    exit;

  i := DmAdoFinan.AtualizaQueries.IndexOf('VARIED');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := '(CODNOVO = ' + trim(tcod) + ') AND ' + ' (CULTURA = ''' +
      tcult + ''')';
    Filtered := true;
    if RecordCount > 0 then
      result := FieldByName('DESCR').AsString;
    Filtered := false;
  end;
end;

// CULTURA
function CulturaDesc(tcod: string): string;
var
  i: integer;
begin
  result := '';
  if trim(tcod) = '' then
    exit;
  i := DmAdoFinan.AtualizaQueries.IndexOf('CULTURA');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := ' (COD = ''' + tcod + ''' )';;
    Filtered := true;
    if RecordCount > 0 then
      result := FieldByName('DESCRI').AsString;
    Filtered := false;
  end;
end;


// Armqzem
function DepositoDesc(tcod: string): string;
var
  i: integer;
begin
  result := '';
  if trim(tcod) = '' then
    exit;

  i := DmAdoFinan.AtualizaQueries.IndexOf('ARMAZEM');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := ' (COD = ''' + tcod + ''' )';;
    Filtered := true;
    if RecordCount > 0 then
      result := FieldByName('NOME_DEP').AsString;
    Filtered := false;
  end;

end;



// CLASSMAT

function NomedaEmpresa(tcod: string): string;
var
  i: integer;
  PesqDataSet: TAdoDataSet;
begin
  result := '';
  PesqDataSet := TAdoDataSet.Create(nil);
  PesqDataSet := DevolveQuery
    ('SELECT DESCRICAO FROM PLACON WHERE NUMCONTA = ''00000000''',
    DmAdoFinan.PATH_ESCRITOR, '', PesqDataSet);
  if PesqDataSet.RecordCount > 0 then
    result := PesqDataSet.FieldByName('DESCRICAO').AsString;
  PesqDataSet.Destroy;
end;

procedure PreenchaServer_CbQuadra(var CbQuadra: TCustomComboBox;
  tgleba: String);
var
  guarde: TObjDoc;
  i: integer;
begin
  i := DmAdoFinan.AtualizaQueries.IndexOf('QUADRAS');
  if TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualizado = false
  then
    TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).atualize();
  with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
  begin
    Filter := '(FIM = #30/12/1899#) AND (GLEBA = ''' + tgleba + ''')';
    Filtered := true;

  end;
  with CbQuadra do
  begin
    Items.Clear;
    with TAtualizaQueries(DmAdoFinan.AtualizaQueries.Objects[i]).adodataset do
    begin
      while not EOF do
      begin
        guarde := TObjDoc.Create;
        guarde.tDoc := FieldByName('QUA').AsString;
        Items.AddObject(FieldByName('QUA').AsString, guarde);
        next;
      end;
      Filtered := false;
    end;

  end;
  // qrQuadras.Destroy;
end;

         */


        /// <summary>
        ///  Tabelas RELACIONADAS
        /// </summary>
        static public DataSet SetoresGlebasQuadras()
        {
            DataSet result = null;
            if ((dsTabelasIniciais == null) || (dsTabelasIniciais.Tables.Count == 0)) return result;
            result = new DataSet();

            DataTable Setores = dsTabelasIniciais.Tables["SETORES"].AsEnumerable().Where(row => (row.IsNull("FIM"))).OrderBy(row=>row.Field<string>("COD")).CopyToDataTable();
            Setores.TableName = "SETORES";
            DataTable Glebas = dsTabelasIniciais.Tables["GLEBAS"].AsEnumerable().Where(row => (row.IsNull("FIM"))).OrderBy(row => row.Field<string>("COD")).CopyToDataTable();
            Glebas.TableName = "GLEBAS";
            DataTable Quadras = dsTabelasIniciais.Tables["QUADRAS"].AsEnumerable().Where(row => (row.IsNull("FIM"))).OrderBy(row => row.Field<string>("COD")).CopyToDataTable();
            Quadras.TableName = "QUADRAS";
            Glebas.Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
            Glebas.Columns["DisplayCodNome"].MaxLength = Glebas.Columns["COD"].MaxLength + 1 + Glebas.Columns["DESCRI"].MaxLength;

            foreach (DataRow orow in Glebas.AsEnumerable())
            {
                orow.BeginEdit();
                orow["DisplayCodNome"] = orow["COD"].ToString() + " " + orow["DESCRI"].ToString();
                orow.EndEdit();
            }
            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = Glebas.Columns["COD"];
            Glebas.PrimaryKey = PrimaryKeyColumns;

            PrimaryKeyColumns[0] = Quadras.Columns["QUA"];
            Quadras.PrimaryKey = PrimaryKeyColumns;
            DataColumn[] parentcolumn = new DataColumn[1];
            DataColumn[] childcolumn = new DataColumn[1];
            DataRelation[] mydatarelation = new DataRelation[1];

            parentcolumn[0] = Glebas.Columns["Cod"];
            childcolumn[0] = Quadras.Columns["GLEBA"];
            mydatarelation[0] = new DataRelation("Gleba_QUADRAS", parentcolumn, childcolumn, true);

            result.Tables.Add(Setores);
            result.Tables.Add(Glebas);
            result.Tables.Add(Quadras);
            result.Relations.AddRange(mydatarelation);

            return result;
        }








        /*
         * 
         *   static public DataSet GlebasQuadras()
        {
            OdbcCommand odbccomm;
            string setorodbc, path;
            DataSet result;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            setorodbc = "SELECT * FROM " + path + "GLEBAS where fim = ctod('  /  /  ') ";
            setorodbc += " ORDER BY COD";
           
            try
            {
                odbccomm = new OdbcCommand(setorodbc, TDataControlReduzido.ConnectionPooling.GetConnection());
                OdbcDataAdapter odbcda = new OdbcDataAdapter(odbccomm);
                odbcda.TableMappings.Add("Table", "GLEBAS");
                result = new DataSet();
                odbcda.Fill(result);
                
                result.Tables["GLEBAS"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                result.Tables["GLEBAS"].Columns["DisplayCodNome"].MaxLength = result.Tables["GLEBAS"].Columns["COD"].MaxLength + 1 + result.Tables["GLEBAS"].Columns["DESC"].MaxLength;

                foreach (DataRow orow in result.Tables["GLEBAS"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["COD"].ToString() + " " + orow["DESC"].ToString();
                }


                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["GLEBAS"].Columns["COD"];
                result.Tables["GLEBAS"].PrimaryKey = PrimaryKeyColumns;

                setorodbc = "SELECT * FROM " + path + "QUADRAS where fim = ctod('  /  /  ') and qua <> ''" +
                     " and setor <> ''";
                setorodbc += " ORDER BY QUA";
                odbccomm = new OdbcCommand(setorodbc, TDataControlReduzido.ConnectionPooling.GetConnection());
                odbcda = new OdbcDataAdapter(odbccomm);
                odbcda.TableMappings.Add("Table", "QUADRAS");
                odbcda.Fill(result);
           
                result.Tables["QUADRAS"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                //result.Tables["QUADRAS"].Columns["DisplayCodNome"].MaxLength = result.Tables["QUADRAS"].Columns["QUA"].MaxLength + 1 + result.Tables["GLEBAS"].Columns["DESC"].MaxLength;

                foreach (DataRow orow in result.Tables["QUADRAS"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["QUA"].ToString();// +" " + orow["DESC"].ToString();
                }
                PrimaryKeyColumns[0] = result.Tables["QUADRAS"].Columns["QUA"];
                result.Tables["QUADRAS"].PrimaryKey = PrimaryKeyColumns;
                DataColumn[] parentcolumn = new DataColumn[1];
                DataColumn[] childcolumn = new DataColumn[1];
                DataRelation[] mydatarelation = new DataRelation[1];

                parentcolumn[0] = result.Tables["GLEBAS"].Columns["Cod"];
                childcolumn[0] = result.Tables["QUADRAS"].Columns["GLEBA"];
                mydatarelation[0] = new DataRelation("Gleba_QUADRAS", parentcolumn, childcolumn, true);

                result.Relations.AddRange(mydatarelation);
                odbcda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Não Foi Possivel acessar a tabela GLebas/Quadras no caminho:", path));
            }
        }
        static public DataSet ModeloServicos()
        {
            OdbcCommand odbccomm;
            string setorodbc, path;
            DataSet result;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            //Add('SELECT NUM,SEQ,COD,Modelo."DESC" as Descricao, TIPO,PROD FROM MODELO ');
            // Add(' where ');
            // Add(' ( MODELO."Desc" <> "" )' );
            // Add(' order by Descricao');
            // e

            setorodbc = "SELECT NUM,SEQ,COD,DESC as Descricao, TIPO,PROD FROM " + path + "MODELO where DESC <> '' ";
            setorodbc += " ORDER BY NUM";

            try
            {
                odbccomm = new OdbcCommand(setorodbc, TDataControlReduzido.ConnectionPooling.GetConnection());
                OdbcDataAdapter odbcda = new OdbcDataAdapter(odbccomm);
                odbcda.TableMappings.Add("Table", "MODELO");
                result = new DataSet();
                odbcda.Fill(result);

                result.Tables["MODELO"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                result.Tables["MODELO"].Columns["DisplayCodNome"].MaxLength = result.Tables["MODELO"].Columns["NUM"].MaxLength + 1 + result.Tables["MODELO"].Columns["DESCRICAO"].MaxLength;

                foreach (DataRow orow in result.Tables["MODELO"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["NUM"].ToString() + " " + orow["DESCRICAO"].ToString();
                }


                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables["MODELO"].Columns["NUM"];
                result.Tables["MODELO"].PrimaryKey = PrimaryKeyColumns;
            
                setorodbc = "SELECT NUM,SEQ,MODELO.COD,MODELO.DESC as Descricao,SERVIC.COD as CODSERV,SERVIC.DESC as DESCSERV,MODELO.TIPO,PROD FROM " + path + "MODELO " +
                    "INNER JOIN " + path + "SERVIC ON (MODELO.COD = SERVIC.COD) where  (MODELO.COD <> '') "; 
                     
                setorodbc += " ORDER BY DESCSERV";
                odbccomm = new OdbcCommand(setorodbc, TDataControlReduzido.ConnectionPooling.GetConnection());
                odbcda = new OdbcDataAdapter(odbccomm);
                odbcda.TableMappings.Add("Table", "SERVICO");
                odbcda.Fill(result);

                result.Tables["SERVICO"].Columns.Add("DisplayCodNome", System.Type.GetType("System.String"));
                result.Tables["SERVICO"].Columns["DisplayCodNome"].MaxLength = result.Tables["SERVICO"].Columns["CODSERV"].MaxLength + 1 + result.Tables["SERVICO"].Columns["DESCSERV"].MaxLength;

                foreach (DataRow orow in result.Tables["SERVICO"].AsEnumerable())
                {
                    orow["DisplayCodNome"] = orow["CODSERV"].ToString() + " " + orow["DESCSERV"].ToString();
                }
        DataColumn[] parentcolumn = new DataColumn[1];
        DataColumn[] childcolumn = new DataColumn[1];
        DataRelation[] mydatarelation = new DataRelation[1];

        parentcolumn[0] = result.Tables["MODELO"].Columns["NUM"];
                childcolumn[0] = result.Tables["SERVICO"].Columns["NUM"];
                mydatarelation[0] = new DataRelation("MODELO_SERVICOS", parentcolumn, childcolumn, true);

        result.Relations.AddRange(mydatarelation);
                odbcda.Dispose();
                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("Não Foi Possivel acessar a tabela modelo/servico no caminho:", path));
            }
        }

         */ 

    }


    static public class DadosComum
    {

        static public DataTable Desc2Combo;
        static public DataTable ModeloCombo;
        static public DataTable ServicoModeloCombo;
        static public DataTable SetoresCombo;
        static public DataTable GlebasCombo;
        static public DataTable GlebasCADCombo;
        static public DataTable QuadrasCombo;
        static public DataTable TipoMovCombo;
        static public DataTable CadestCombo;
        static public DataTable CatProdCombo;
        static public DataTable FirmaCombo;
        static public DataTable BancosCombo;
        static public DataTable ArmazemCombo;
        static public DataTable TipoProdCombo;
        static public DataTable TabSefipCombo;

        static public DataTable ModeloCombo_Cod_Descr;
        static public DataTable ServicoModeloCombo_Cod_Descr;

        static public DataTable Categoria;

        static public DataSet dsPesquisa;

        static public bool tabelasJaConfiguradas()
        {
            bool result = false;
            if ((dsPesquisa != null) && (dsPesquisa.Tables.Count > 0))
                result = true;
            return result;
        }
        static public void TabelasConfigCombos()
        {
            //PESQDESC2; PLACON
            /// ATENÇÂO PARA A REGRA DAS TABELAS QUE IRÃO ALIMENTAR OS COMBOS

            dsPesquisa = new DataSet();
            DataSet odataset = TabelasIniciais.DsTabelasInciais();

            if (odataset.Tables.Contains("PLACON"))
            {
                dsPesquisa.Tables.Add(odataset.Tables["PLACON"].Copy());
            }
            if (odataset.Tables.Contains("BANCOS"))
            {
                dsPesquisa.Tables.Add(odataset.Tables["BANCOS"].Copy());
            }


            if (odataset.Tables.Contains("PLACON"))
            {
                int maxLengh =  odataset.Tables["PLACON"].Columns["DESC2"].MaxLength;
                Desc2Combo = new DataTable();
                Desc2Combo.Columns.Add("DESC2", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["PLACON"].Rows)
                {
                    DataRow combo = Desc2Combo.NewRow();

                    combo["DESC2"] = SubstituaSlash(row["DESC2"].ToString());
                    Desc2Combo.Rows.Add(combo);
                }
                foreach (DataRow row in odataset.Tables["PLACON"].Rows)
                {
                    DataRow combo = Desc2Combo.NewRow();
                    string descAsterisco = "*" + SubstituaSlash(row["DESC2"].ToString().Trim());
                    if (descAsterisco.Length > maxLengh)
                        descAsterisco = descAsterisco.Substring(0, maxLengh);
                    else
                        descAsterisco = descAsterisco.PadRight(maxLengh);
                    combo["DESC2"] = descAsterisco;
                    Desc2Combo.Rows.Add(combo);
                }

                Desc2Combo.AcceptChanges();

            }
            if (odataset.Tables.Contains("MODELO"))
            {
                ModeloCombo = new DataTable();
                ModeloCombo.Columns.Add("NUM_MOD", Type.GetType("System.String"));
                ModeloCombo.Columns["NUM_MOD"].MaxLength = 2;
                ModeloCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                ModeloCombo.Columns["DESCRICAO"].MaxLength = 15;
                foreach (DataRow row in odataset.Tables["MODELO"].Rows)
                {
                    DataRow combo = ModeloCombo.NewRow();
                    combo["NUM_MOD"] = row["NUM"].ToString().Trim();
                    combo["DESCRICAO"] = SubstituaSlash(row["DESCRICAO"].ToString());
                    ModeloCombo.Rows.Add(combo);
                }
                ModeloCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("MODELO"))
            {
                ModeloCombo_Cod_Descr = new DataTable();
                ModeloCombo_Cod_Descr.Columns.Add("NUM_MOD", Type.GetType("System.String"));
                ModeloCombo_Cod_Descr.Columns["NUM_MOD"].MaxLength = 2;
                ModeloCombo_Cod_Descr.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                ModeloCombo_Cod_Descr.Columns["NUM_MOD"].MaxLength = 18;
                foreach (DataRow row in odataset.Tables["MODELO"].Rows)
                {
                    DataRow combo = ModeloCombo_Cod_Descr.NewRow();
                    combo["NUM_MOD"] = row["NUM"].ToString().Trim();
                    combo["DESCRICAO"] = row["NUM"].ToString()+ " "+ SubstituaSlash(row["DESCRICAO"].ToString());
                    ModeloCombo_Cod_Descr.Rows.Add(combo);
                }
                ModeloCombo_Cod_Descr.AcceptChanges();
            }


            if (odataset.Tables.Contains("SERVICOMODELO"))
            {
                ServicoModeloCombo = new DataTable();
                ServicoModeloCombo.Columns.Add("NUM_MOD", Type.GetType("System.String"));
                ServicoModeloCombo.Columns.Add("CODSER", Type.GetType("System.String"));
                ServicoModeloCombo.Columns["NUM_MOD"].MaxLength = 2;
                ServicoModeloCombo.Columns["CODSER"].MaxLength = 3;
                ServicoModeloCombo.Columns.Add("DESCSERV", Type.GetType("System.String"));
                ServicoModeloCombo.Columns["DESCSERV"].MaxLength = 25;
                foreach (DataRow row in odataset.Tables["SERVICOMODELO"].Rows)
                {
                    DataRow combo = ServicoModeloCombo.NewRow();
                    combo["NUM_MOD"] = row["NUM"].ToString().Trim();
                    combo["CODSER"] = row["CODSERV"].ToString().Trim();
                    combo["DESCSERV"] = SubstituaSlash(row["DESCSERV"].ToString());
                    ServicoModeloCombo.Rows.Add(combo);
                }
                ServicoModeloCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("SERVICOMODELO"))
            {
                ServicoModeloCombo_Cod_Descr = new DataTable();
                ServicoModeloCombo_Cod_Descr.Columns.Add("NUM_MOD", Type.GetType("System.String"));
                ServicoModeloCombo_Cod_Descr.Columns.Add("CODSER", Type.GetType("System.String"));

                ServicoModeloCombo_Cod_Descr.Columns["NUM_MOD"].MaxLength = 2;
                ServicoModeloCombo_Cod_Descr.Columns["CODSER"].MaxLength = 3;
                ServicoModeloCombo_Cod_Descr.Columns.Add("DESCSERV", Type.GetType("System.String"));
                ServicoModeloCombo_Cod_Descr.Columns["DESCSERV"].MaxLength = 29;
                foreach (DataRow row in odataset.Tables["SERVICOMODELO"].Rows)
                {
                    DataRow combo = ServicoModeloCombo_Cod_Descr.NewRow();
                    combo["NUM_MOD"] = row["NUM"].ToString().Trim();
                    combo["CODSER"] = row["CODSERV"].ToString().Trim();
                    combo["DESCSERV"] = row["CODSERV"].ToString()+" "+ SubstituaSlash(row["DESCSERV"].ToString());
                    ServicoModeloCombo_Cod_Descr.Rows.Add(combo);
                }
                ServicoModeloCombo_Cod_Descr.AcceptChanges();
            }



            if (odataset.Tables.Contains("SETORES"))
            {
                SetoresCombo = new DataTable();
                SetoresCombo.Columns.Add("SETOR", Type.GetType("System.String"));
                SetoresCombo.Columns.Add("DESCRI", Type.GetType("System.String"));
                SetoresCombo.Columns.Add("CODDESCRI", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["SETORES"].Rows)
                {
                    DataRow combo = SetoresCombo.NewRow();
                    combo["SETOR"] = row["COD"].ToString().Trim();
                    combo["DESCRI"] = row["DESCRI"];
                    combo["CODDESCRI"] = row["COD"].ToString() + " " + row["DESCRI"].ToString();
                    SetoresCombo.Rows.Add(combo);
                }
                SetoresCombo.AcceptChanges();

            }
            if (odataset.Tables.Contains("GLEBAS"))
            {
                GlebasCombo = new DataTable();
                GlebasCombo.Columns.Add("GLEBA", Type.GetType("System.String"));
                GlebasCombo.Columns.Add("GLECAD", Type.GetType("System.String"));
                GlebasCombo.Columns.Add("DESCRI", Type.GetType("System.String"));
                GlebasCombo.Columns.Add("SETOR", Type.GetType("System.String"));
                GlebasCombo.Columns.Add("CODDESCRI", Type.GetType("System.String"));
                GlebasCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["GLEBAS"].Rows)
                {
                    DataRow combo = GlebasCombo.NewRow();
                    combo["GLEBA"] = row["COD"].ToString().Trim() ;
                    combo["GLECAD"] = row["COD"].ToString().Trim();
                    combo["SETOR"] = row["SETOR"].ToString().Trim();
                    combo["DESCRI"] = row["DESCRI"];
                    combo["DESCRICAO"] = row["COD"];
                    combo["CODDESCRI"] = row["COD"].ToString() + " " + row["DESCRI"].ToString();
                    GlebasCombo.Rows.Add(combo);
                }
                GlebasCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("QUADRAS"))
            {
                QuadrasCombo = new DataTable();
                QuadrasCombo.Columns.Add("QUADRA", Type.GetType("System.String"));
                QuadrasCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                QuadrasCombo.Columns.Add("SETOR", Type.GetType("System.String"));
                QuadrasCombo.Columns.Add("GLEBA", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["QUADRAS"].Rows)
                {
                    DataRow combo = QuadrasCombo.NewRow();
                    combo["QUADRA"] = row["QUA"].ToString().Trim();
                    combo["SETOR"] = row["SETOR"].ToString().Trim();
                    combo["GLEBA"] = row["GLEBA"].ToString().Trim(); 
                    combo["DESCRICAO"] = row["QUA"];
                    QuadrasCombo.Rows.Add(combo);
                }
                QuadrasCombo.AcceptChanges();
            }



            if (odataset.Tables.Contains("TIPOMOV"))
            {
                TipoMovCombo = new DataTable();
                TipoMovCombo.Columns.Add("TPMOV", Type.GetType("System.Double"));
                TipoMovCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["TIPOMOV"].Rows)
                {
                    DataRow combo = TipoMovCombo.NewRow();
                    //  string cod = Convert.ToInt32(row["TPMOV"]).ToString().PadLeft(3);
                    combo["TPMOV"] = row["TPMOV"];
                    combo["DESCRICAO"] = SubstituaSlash(row["DESCRICAO"].ToString());
                    TipoMovCombo.Rows.Add(combo);
                }
                TipoMovCombo.AcceptChanges();

            }

            if (odataset.Tables.Contains("CADEST"))
            {
                CadestCombo = new DataTable();
                CadestCombo.Columns.Add("COD", Type.GetType("System.String"));
                CadestCombo.Columns["COD"].MaxLength = 4;
                CadestCombo.Columns.Add("DESCRI", Type.GetType("System.String"));
                CadestCombo.Columns["DESCRI"].MaxLength = 40;
                CadestCombo.Columns.Add("UNID", Type.GetType("System.String"));
                CadestCombo.Columns["UNID"].MaxLength = 3;
                foreach (DataRow row in odataset.Tables["CADEST"].Rows)
                {
                    DataRow combo = CadestCombo.NewRow();

                    combo["COD"] = row["COD"];
                    combo["UNID"] = row["UNID"];
                    combo["DESCRI"] = SubstituaSlash(row["DESCR"].ToString());
                    CadestCombo.Rows.Add(combo);
                }
                CadestCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("ARMAZEM"))
            {
                ArmazemCombo = new DataTable();
                ArmazemCombo.Columns.Add("DEPOSITO", Type.GetType("System.String"));
                ArmazemCombo.Columns.Add("NOME_DEP", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["ARMAZEM"].Rows)
                {
                    DataRow combo = ArmazemCombo.NewRow();

                    combo["DEPOSITO"] = row["COD"];
                    combo["NOME_DEP"] = row["COD"].ToString() + " " + row["NOME_DEP"].ToString();
                    ArmazemCombo.Rows.Add(combo);
                }
                ArmazemCombo.AcceptChanges();
            }


            if (odataset.Tables.Contains("CATPROD"))
            {
                CatProdCombo = new DataTable();
                CatProdCombo.Columns.Add("PROD", Type.GetType("System.String"));
                CatProdCombo.Columns[0].MaxLength = 3;
                CatProdCombo.Columns.Add("DESCRI", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["CATPROD"].Rows)
                {
                    DataRow combo = CatProdCombo.NewRow();

                    combo["PROD"] = row["COD"];
                    combo["DESCRI"] = SubstituaSlash(row["DESCRI"].ToString());
                    CatProdCombo.Rows.Add(combo);
                }
                CatProdCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("FIRMA"))
            {
                FirmaCombo = new DataTable();
                FirmaCombo.Columns.Add("FIRMA", Type.GetType("System.String"));
                FirmaCombo.Columns.Add("DESCRI", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["FIRMA"].Rows)
                {
                    DataRow combo = FirmaCombo.NewRow();

                    combo["FIRMA"] = row["CONTABIL"];
                    combo["DESCRI"] = SubstituaSlash(row["DESCRI"].ToString());
                    FirmaCombo.Rows.Add(combo);
                }
                FirmaCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("BANCOS"))
            {
                BancosCombo = new DataTable();
                BancosCombo.Columns.Add("NBANCO", Type.GetType("System.String"));
                BancosCombo.Columns.Add("DESCRI", Type.GetType("System.String"));
                BancosCombo.Columns.Add("DESCRI_N", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["BANCOS"].Rows)
                {
                    DataRow combo = BancosCombo.NewRow();
                    combo["NBANCO"] = row["NBANCO"].ToString().PadLeft(2, '0');
                    combo["DESCRI_N"] = row["NBANCO"].ToString().PadLeft(2, '0');
                    combo["DESCRI"] = row["NBANCO"].ToString().PadLeft(2, '0') + " " + row["NOME_BANCO"].ToString();
                    BancosCombo.Rows.Add(combo);
                }
                BancosCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("TIPOPROD"))
            {
                TipoProdCombo = new DataTable();
                TipoProdCombo.Columns.Add("PROD_TP", Type.GetType("System.Int32"));
                TipoProdCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                List<Int32> incluido = new List<int>();
                incluido.Add(0);
                incluido.Add(11);
                incluido.Add(1);
                incluido.Add(21);
                foreach (DataRow row in odataset.Tables["TIPOPROD"].Rows)
                {
                    if (!incluido.Contains(Convert.ToInt32(row["COD"]))) continue;
                    DataRow combo = TipoProdCombo.NewRow();
                    combo["PROD_TP"] = row["COD"];
                    combo["DESCRICAO"] = SubstituaSlash(row["DESCRICAO"].ToString());
                    TipoProdCombo.Rows.Add(combo);
                }
                TipoProdCombo.AcceptChanges();
            }
            if (odataset.Tables.Contains("CATEGORIA"))
            {
                Categoria = new DataTable();
                Categoria.Columns.Add("NUMCAT", Type.GetType("System.Int32"));
                //Categoria.Columns[0].MaxLength = 2;
                Categoria.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                Categoria.Columns[1].MaxLength = 30;
                foreach (DataRow row in odataset.Tables["CATEGORIA"].Rows)
                {
                    DataRow combo = Categoria.NewRow();
                   
                    combo["NUMCAT"] = row["NUMCAT"];
                    combo["DESCRICAO"] = SubstituaSlash(row["DESCRICAO"].ToString()).Substring(0,30);
                    Categoria.Rows.Add(combo);
                }
                Categoria.AcceptChanges();
            }
            if (odataset.Tables.Contains("TABSEFIP"))
            {
                TabSefipCombo = new DataTable();
                TabSefipCombo.Columns.Add("COD", Type.GetType("System.String"));
                TabSefipCombo.Columns[0].MaxLength = 2;
                TabSefipCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                TabSefipCombo.Columns[1].MaxLength = 110;
                TabSefipCombo.Columns.Add("TIPO", Type.GetType("System.String"));
                TabSefipCombo.Columns[2].MaxLength = 2;

                foreach (DataRow row in odataset.Tables["TabSefip"].Rows)
                {
                    DataRow combo = TabSefipCombo.NewRow();

                    combo["COD"] = row["COD"];
                    combo["TIPO"] = row["TIPO"];
                    combo["DESCRICAO"] = row["COD"] + " " +SubstituaSlash(row["DESCRICAO"].ToString()).Trim();
                    TabSefipCombo.Rows.Add(combo);
                }
                TabSefipCombo.AcceptChanges();
            }
        }
        // Testar se o filho pertence ao Parent
        // Serviço => pai Modelo
        // probleMA slash 
        static private string SubstituaSlash(string texto)
        {
            string result = texto.Contains(Convert.ToChar("/")) ? texto.Replace(Convert.ToChar("/"), Convert.ToChar("|")) : texto;
            return result;
        }

        static public bool campoServicoSource(string pai_valor, string filho_valor)
        {
            bool result = false;
            DataView serv = ServicoModeloCombo.Copy().AsDataView();
            serv.RowFilter = "NUM_MOD = '" + pai_valor.Trim() + "'";
            serv.Sort = "CODSER";
            int i = serv.Find(filho_valor.Trim());
            result = (i != -1);
            return result;
        }

        // Glebas => pai Setor
        static public bool campoGlebaSource(string pai_valor, string filho_valor)
        {
            bool result = false;
            DataView serv = GlebasCombo.Copy().AsDataView();
            serv.RowFilter = "SETOR = '" + pai_valor.Trim() + "'";
            serv.Sort = "GLEBA";
            int i = serv.Find(filho_valor.Trim());
            result = (i != -1);
            return result;
        }
        // Quadras => pai Gleba
        static public bool campoQuadraSource(string pai_valor, string filho_valor)
        {
            bool result = false;
            DataView serv = QuadrasCombo.Copy().AsDataView();
            serv.RowFilter = "GLEBA = '" + pai_valor.Trim() + "'";
            serv.Sort = "QUADRA";
            int i = serv.Find(filho_valor.Trim());
            result = (i != -1);
            return result;
        }
    }
}
