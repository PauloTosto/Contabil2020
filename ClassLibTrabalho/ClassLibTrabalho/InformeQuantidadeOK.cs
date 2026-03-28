using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using System.Data.OleDb;
using ClassConexao;

namespace ClassLibTrabalho
{
    
    //CLASS QUE AUXILIA A DESCOBERTA DAS QUANTIDADES DE TAREFAS QUANDO DO OK DO PONTO

    public class QuantitativosLinhaOk
    {
        string fquadra, fcodser, fnum, funid, fcult, fprod;
        DateTime finicio, ffim;
        Decimal farea, fproducao, fprod_area;
        private DataSet dsmodeloservico;
        // private DataSet dsareacaixaplantas;

        //private DataRow[] linhasclt;
        public string unidade { get { return funid; } }
        public string cult { get { return fcult; } }
        public string prod { get { return fprod; } }



        public DateTime inicio { get { return finicio; } }
        public DateTime fim { get { return ffim; } }

        private Dictionary<string, decimal> DictAreas = new Dictionary<string, decimal>();
        private Dictionary<string, Dictionary<string, TProdutoData>> DictProduto = new Dictionary<string, Dictionary<string, TProdutoData>>();  // 
        private Dictionary<string, TCulturaquadra> DictPlantas = new Dictionary<string, TCulturaquadra>();
        private DataTable dtservico;
        private DataTable dtmodelo;

        public QuantitativosLinhaOk()
        {
            if (dsmodeloservico == null)
                dsmodeloservico = TDataDivs.TabelasCLT();
            dtservico = dsmodeloservico.Tables["SERVIC"];
            dtmodelo = dsmodeloservico.Tables["MODELO"];

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = dtservico.Columns["COD"];
            dtservico.PrimaryKey = PrimaryKeyColumns;

        }
        // a partir da linha do ponto
        public TabelasOkQuant determinetabelaok(DataRow linha)
        {
            TabelasOkQuant ok = TabelasOkQuant.Nenhum;
            fprod = "";
            DataRow rowserv = dtservico.Rows.Find(linha["CODSER"].ToString());
            funid = rowserv["UNID"].ToString().ToUpper().Trim();
            fquadra = linha["BL"].ToString();
            if (funid.Trim() == "") return ok;
            if (fquadra.Trim() == "") return ok;

            DataRow[] rowmodelos;

            rowmodelos = dtmodelo.Select("NUM = '" + linha["NUM_MOD"].ToString() + "' AND COD = ''");
            if (rowmodelos.Length > 0)
                fprod = rowmodelos[0]["PROD"].ToString();

            fcult = fprod;

            if ((funid == "TAR") || (funid == "TAREFA") || (funid == "TAREFAS"))
                return TabelasOkQuant.Areas;
            if (((funid == "CX") || (funid == "CXS") || (funid == "CAIXA") || (funid == "CAIXAS")) && (fprod.Trim() != ""))
                return TabelasOkQuant.Produtos;
            if ((fprod.Trim() != "") && ((funid == "PL") || (funid == "PLS") || (funid == "PLANTA") || (funid == "PLANTAS")))
                return TabelasOkQuant.Plantas;
            return ok;
        }


        public Decimal area
        {
            get
            {
                if (fquadra.Trim() == "") return 0;
                if (farea == 0)
                    farea = Math.Round((2.333M * Convert.ToDecimal(TDataDivs.peghectares(fquadra))), 4);
                return farea;
            }
        }
        public Decimal producao
        {
            get
            {
                if (funid.Trim() != "CX")
                    fproducao = 0;
                else
                    if (fproducao == 0)
                        fproducao = Convert.ToDecimal(TDataDivs.pegcaixas(fquadra, finicio, ffim));
                return fproducao;
            }
        }
        public Decimal prod_area
        {
            get
            {
                if ((producao != 0) && (area != 0))
                    fprod_area = (producao / area);  // caixas/tarefas
                else fprod_area = 0;
                return fprod_area;
            }
        }
        public Decimal quant()
        {
            Decimal result = 0;
            if ((funid.Trim() == "PL") && (fcult.Trim() != ""))
                result = Convert.ToDecimal(TDataDivs.pegplantas(fquadra, fcult));
            else
                if (funid.Trim() == "TAR")
                    result = area;
                else
                    if (funid.Trim() == "CX")
                        result = producao;
            return result;
        }

        private Decimal plantasquant()
        {
            if (DictPlantas.ContainsKey(fcult))
                if (DictPlantas[fcult].quadra.Contains(fquadra))
                {
                    int index = DictPlantas[fcult].quadra.IndexOf(fquadra);
                    if (index > -1)
                        return DictPlantas[fcult].quant[index];
                }
            return 0M;
        }
        private Decimal produtoquant(DateTime osemana)
        {
            if (DictProduto.ContainsKey(fprod))
            {
                Dictionary<string, TProdutoData> prodquadras = DictProduto[fprod];
                if (prodquadras.ContainsKey(fquadra))
                {
                    int index = prodquadras[fquadra].inicio.IndexOf(osemana.Date);
                    if (index > -1)
                        return prodquadras[fquadra].quant[index];
                }
            }
            return 0M;
        }
        private Decimal areaquant()
        {
            if (DictAreas.ContainsKey(fquadra))
            {
                return DictAreas[fquadra];
            }
            return 0M;
        }

        public Decimal quant(DataRow ponto, DateTime semana)
        {
            Decimal result = 0;
            TabelasOkQuant ok = determinetabelaok(ponto);
            if (ok == TabelasOkQuant.Plantas)
                return plantasquant();
            if (ok == TabelasOkQuant.Produtos)
                return produtoquant(semana);
            if (ok == TabelasOkQuant.Areas)
                return Math.Round((2.333M * areaquant()), 4);
            return result;
        }



        public void InformeLinhaok(string tquadra, string tcodser, string tnum, string tunid, string tcult, DateTime dtaponto, DateTime dtafim)
        {
            fquadra = tquadra;
            fcodser = tcodser;
            fnum = tnum;
            finicio = dtaponto;
            ffim = dtafim;
            funid = tunid;
            farea = 0;
            fcult = "";
            fprod = "";
            fprod_area = 0;
            fproducao = 0;
            if (fcodser.Trim() == "")
                fcult = "";
            fcult = tcult;
        }

        public void InformeTodasLinhasOk(System.Data.EnumerableRowCollection<DataRow> linhasok, DateTime oinicio, DateTime ofim)
        {
            //acesso os dados do servidor a partir das demandas futuras previstas do cltponto     
            //quadras a serem pesquisadas
            TProdutoData oprod;
            ffim = ofim;
            finicio = oinicio;
            foreach (DataRow linha in linhasok)
            {
                TabelasOkQuant ok = determinetabelaok(linha);
                if (ok == TabelasOkQuant.Nenhum) continue;
                if (ok == TabelasOkQuant.Areas)
                {
                    if (!(DictAreas.ContainsKey(linha["BL"].ToString())))
                        DictAreas.Add(linha["BL"].ToString(), 0M);
                    continue;
                }
                if (ok == TabelasOkQuant.Produtos)
                {
                    if (!(DictProduto.ContainsKey(prod)))
                    {
                        Dictionary<string, TProdutoData> oproddata = new Dictionary<string, TProdutoData>();
                        oprod = new TProdutoData();
                        oprod.chave = linha["BL"].ToString();// prod;
                        oprod.inicio.Add(Convert.ToDateTime(linha["DATA"]).Date);
                        oprod.quant.Add(0M);
                        oproddata.Add(oprod.chave, oprod);
                        DictProduto.Add(prod, oproddata);
                    }
                    else
                    {
                        Dictionary<string, TProdutoData> oproddata = DictProduto[prod];
                        if (!(oproddata.ContainsKey(linha["BL"].ToString())))
                        {
                            oprod = new TProdutoData();
                            oprod.chave = linha["BL"].ToString(); //prod;
                            oprod.inicio.Add(Convert.ToDateTime(linha["DATA"]).Date);
                            oprod.quant.Add(0M);
                            oproddata.Add(oprod.chave, oprod);
                        }
                        else
                        {
                            oprod = oproddata[linha["BL"].ToString()];
                            if (!(oprod.inicio.Contains(Convert.ToDateTime(linha["DATA"]).Date)))
                            {
                                oprod.inicio.Add(Convert.ToDateTime(linha["DATA"]).Date);
                                oprod.quant.Add(0M);
                            }
                        }
                    }
                    continue;
                }
                if (ok == TabelasOkQuant.Plantas)
                {
                    if (!(DictPlantas.ContainsKey(prod)))
                    {
                        TCulturaquadra ocultura = new TCulturaquadra();
                        ocultura.quadra.Add(Convert.ToString(linha["BL"]));
                        ocultura.quant.Add(0M);
                        DictPlantas.Add(prod, ocultura);
                    }
                    else
                    {
                        TCulturaquadra ocultura = DictPlantas[prod];
                        if (!(ocultura.quadra.Contains(Convert.ToString(linha["BL"]))))
                        {
                            ocultura.quadra.Add(Convert.ToString(linha["BL"]));
                            ocultura.quant.Add(0M);
                        }
                    }
                }
            }
            pegdsareacaixaplantas(inicio, fim);
        }
        private void pegdsareacaixaplantas(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strodbc, path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");
            if (DictAreas.Count > 0)
            {
                strodbc = "SELECT qua, ha, air, cent, aceiro, estrada, real FROM " + path + "AREAS WHERE ";
                string tand = "";
                foreach (KeyValuePair<string, decimal> oKey in DictAreas)
                {
                    strodbc += tand + " (QUA = '" + oKey.Key + "') ";
                    tand = " OR ";
                }
                try
                {
                    oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    OleDbDataReader leitor = oledbcomm.ExecuteReader();
                    if (leitor.HasRows)
                    {
                        while (leitor.Read())
                        {

                            Single result =
                           ((Convert.ToSingle(leitor["HA"]) * 10000) + (Convert.ToSingle(leitor["AIR"]) * 100)
                              + Convert.ToSingle(leitor["CENT"])) / 10000;
                            DictAreas[Convert.ToString(leitor["QUA"])] = Convert.ToDecimal(result);
                        }
                    }
                    leitor.Close();
                }

                catch (Exception)
                {
                    throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "AREAS"));
                }

            }
            if (DictPlantas.Count > 0)
            {

                path = TDataControlReduzido.Get_Path("PATRIMONIO");

                strodbc = "Select    qua, cultura, variedade, cavalo, data_plant, dcavalo, quant, data, tipomov, hist, setor FROM " + path + "PLANTIO " +
                      " where ";
                string tand = "";

                foreach (KeyValuePair<string, TCulturaquadra> oKey in DictPlantas)
                {
                    if (oKey.Value.quadra.Count < 15)
                    {
                        strodbc += tand + " ( (CULTURA = '" + oKey.Key + "')  AND (";
                        string tor = "";
                        for (int i = 0; i < oKey.Value.quadra.Count; i++)
                        {
                            strodbc += tor + "  (QUA = '" + Convert.ToString(oKey.Value.quadra[i]) + "')  ";
                            tor = "  OR ";
                        }
                        strodbc += " ) ) ";
                    }
                    else
                    {
                        strodbc += tand + " (CULTURA = '" + oKey.Key + "') ";
                    }
                    tand = " OR ";
                }
                strodbc += " ORDER BY QUA,CULTURA ";
                try
                {
                    oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    OleDbDataReader leitor = oledbcomm.ExecuteReader();
                    if (leitor.HasRows)
                    {
                        while (leitor.Read())
                        {
                            if (DictPlantas.ContainsKey(leitor["CULTURA"].ToString()))
                            {
                                TCulturaquadra ocult = DictPlantas[leitor["CULTURA"].ToString()];
                                int index = ocult.quadra.IndexOf(leitor["QUA"].ToString());
                                if (index > -1)
                                {
                                    if (leitor["TIPOMOV"].ToString() == "S")
                                        ocult.quant[index] = Convert.ToDecimal(ocult.quant[index]) - Convert.ToDecimal(leitor["QUANT"]);
                                    else
                                        ocult.quant[index] += Convert.ToDecimal(leitor["QUANT"]);
                                }
                            }
                        }
                    }
                    leitor.Close();
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("{0}Não foi possivel acessar", path + "QUADRAS"));
                }
            }
            if (DictProduto.Count > 0)
            {
                path = TDataControlReduzido.Get_Path("TRABALHO");
                // para cada tipo de produto uma ida ao servidor...
                foreach (KeyValuePair<string, Dictionary<string, TProdutoData>> oKey in DictProduto)
                {
                    if (oKey.Key.Trim() == "")
                        continue;
                    strodbc = "Select QUA,DATAE,NQUANT,PROD,DATAP FROM " + path + "PRODUTO " +
                          " where ";
                    string tand = " AND ( ";
                    strodbc += " (PROD = '" + oKey.Key + " ' ) ";
                    strodbc += " AND   (DATAP  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio.AddDays(-2)) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(fim.AddDays(15)) + ") ) ";
                    /*Dictionary<string, TProdutoData> oprodutodata = oKey.Value;
                    foreach (KeyValuePair<string, TProdutoData> oKeyProd in oprodutodata)
                    {
                       
                            strodbc += tand+ " (QUA = '" + oKeyProd.Value.chave + "')  ";
                            tand = " OR ";
                        
                    }
                    if (oprodutodata.Count > 0)
                        strodbc += " ) ";*/
                    try
                    {
                        oledbcomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                        OleDbDataReader leitor = oledbcomm.ExecuteReader();
                        if (leitor.HasRows)
                        {
                            while (leitor.Read())
                            {
                                if (!(DictProduto.ContainsKey(leitor["PROD"].ToString())))
                                    continue;
                                Dictionary<string, TProdutoData> oprodutodta = DictProduto[leitor["PROD"].ToString()];
                                if (!(oprodutodta.ContainsKey(leitor["QUA"].ToString())))
                                    continue;

                                TProdutoData oprod = oprodutodta[leitor["QUA"].ToString()];

                                for (int i = 0; i < oprod.inicio.Count - 1; i++)
                                {
                                    if ((oprod.inicio[i].AddDays(-2) <= Convert.ToDateTime(leitor["DATAP"]).Date) &&
                                        (oprod.inicio[i].AddDays(15) >= Convert.ToDateTime(leitor["DATAP"]).Date))

                                        oprod.quant[i] += Convert.ToDecimal(leitor["NQUANT"]);
                                }
                            }
                        }

                        leitor.Close();
                    }
                    catch (Exception)
                    {
                        throw new Exception(string.Format("{0}Não foi possivel acessar", path + "QUADRAS"));
                    }

                }
            }
        }
    }
    public class TProdutoData : Object
    {
        // public string quadra;
        public string chave;
        public List<DateTime> inicio;
        public DateTime fim;
        public List<Decimal> quant;
        public TProdutoData()
        {
            chave = "";
            inicio = new List<DateTime>();
            fim = DateTime.MaxValue;
            quant = new List<Decimal>();
        }
    }

    public class TCulturaquadra : Object
    {
        public ArrayList quadra;
        public List<Decimal> quant;
        public TCulturaquadra()
        {
            quant = new List<Decimal>();
            quadra = new ArrayList();
        }
    }

    public enum TabelasOkQuant
    {
        Plantas,
        Produtos,
        Areas,
        Nenhum
    }  

}
