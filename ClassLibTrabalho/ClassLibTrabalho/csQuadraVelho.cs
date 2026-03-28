using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.OleDb;
using ClassConexao;


namespace ClassLibTrabalho
{
   public class csQuadraVelho
    {
        public class csQuadras
        {

            string fquadra, fcodser, fnum, funid, fcult;
            DateTime finicio, ffim;
            Decimal farea, fprod, fprod_area;
            private DataSet dsmodeloservico;
            // private DataSet dsareacaixaplantas;

            //private DataRow[] linhasclt;
            public string unidade { get { return funid; } }
            public string cult { get { return fcult; } }
            public DateTime inicio { get { return finicio; } }
            public DateTime fim { get { return ffim; } }

            private Dictionary<string, decimal> DictAreas = new Dictionary<string, decimal>();
            private Dictionary<string, Dictionary<string, TProdutoData>> DictProduto = new Dictionary<string, Dictionary<string, TProdutoData>>();  // 
            private Dictionary<string, TCulturaquadra> DictPlantas = new Dictionary<string, TCulturaquadra>();



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
                        fprod = 0;
                    else
                        if (fprod == 0)
                            fprod = Convert.ToDecimal(TDataDivs.pegcaixas(fquadra, finicio, ffim));
                    return fprod;
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
            public Decimal quant
            {
                get
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
            }
            public csQuadras(string tquadra, string tcodser, string tnum, string tunid, string tcult, DateTime dtaponto, DateTime dtafim)
            {
                fquadra = tquadra;
                fcodser = tcodser;
                fnum = tnum;
                finicio = dtaponto;
                ffim = dtafim;
                funid = tunid;
                farea = 0;
                fcult = "";
                fprod_area = 0;
                fprod = 0;
                if (fcodser.Trim() == "")
                    fcult = "";
                fcult = tcult;
            }

            public csQuadras(System.Data.EnumerableRowCollection<DataRow> linhasok, DateTime oinicio, DateTime ofim)
            {
                //acesso os dados do servidor a partir das demandas futuras previstas do cltponto     
                //quadras a serem pesquisadas
                TProdutoData oprod;
                ffim = ofim;
                finicio = oinicio;
                if (dsmodeloservico == null)
                    dsmodeloservico = TDataDivs.TabelasCLT();
                DataTable dtservico = dsmodeloservico.Tables["SERVIC"];
                DataTable dtmodelo = dsmodeloservico.Tables["MODELO"];

                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = dtservico.Columns["COD"];
                dtservico.PrimaryKey = PrimaryKeyColumns;


                foreach (DataRow linha in linhasok)
                {
                    DataRow rowserv = dtservico.Rows.Find(linha["CODSER"].ToString());
                    string tunid = rowserv["UNID"].ToString().ToUpper().Trim();
                    DataRow[] rowmodelos;
                    string tprod = "";
                    if (tunid != "")
                    {
                        rowmodelos = dtmodelo.Select("NUM = '" + linha["NUM_MOD"].ToString() + "' AND COD = ''");
                        if (rowmodelos.Length > 0)
                            tprod = rowmodelos[0]["PROD"].ToString();
                    }
                    //DataRow rowserv = dtservico.Rows.Find(rowmodelo["COD"].ToString() );            
                    if ((tunid == "TAR") || (tunid == "TAREFA") || (tunid == "TAREFAS"))
                    {
                        if (!(DictAreas.ContainsKey(linha["BL"].ToString())))
                            DictAreas.Add(linha["BL"].ToString(), 0M);
                        continue;
                    }
                    if (((tunid == "CX") || (tunid == "CXS") || (tunid == "CAIXA") || (tunid == "CAIXAS")) && (tprod.Trim() != "") &&
                         (linha["BL"].ToString().Trim() != ""))
                    {
                        if (!(DictProduto.ContainsKey(tprod)))
                        {
                            Dictionary<string, TProdutoData> oproddata = new Dictionary<string, TProdutoData>();
                            oprod = new TProdutoData();
                            oprod.chave = linha["BL"].ToString();// tprod;
                            oprod.inicio.Add(Convert.ToDateTime(linha["DATA"]).Date);
                            oprod.quant.Add(0M);
                            oproddata.Add(oprod.chave, oprod);
                            DictProduto.Add(tprod, oproddata);
                        }
                        else
                        {
                            Dictionary<string, TProdutoData> oproddata = DictProduto[tprod];
                            if (!(oproddata.ContainsKey(linha["BL"].ToString())))
                            {
                                oprod = new TProdutoData();
                                oprod.chave = linha["BL"].ToString(); //tprod;
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
                    // DictProduto.Add(linha["BL"].ToString(), 0M);
                    if ((tprod.Trim() != "") && ((tunid == "PL") || (tunid == "PLS") || (tunid == "PLANTA") || (tunid == "PLANTAS")))
                        if (!(DictPlantas.ContainsKey(tprod)))
                        {
                            TCulturaquadra ocultura = new TCulturaquadra();
                            ocultura.quadra.Add(Convert.ToString(linha["BL"]));
                            ocultura.quant.Add(0M);
                            DictPlantas.Add(tprod, ocultura);
                        }
                        else
                        {
                            TCulturaquadra ocultura = DictPlantas[tprod];
                            if (!(ocultura.quadra.Contains(Convert.ToString(linha["BL"]))))
                            {
                                ocultura.quadra.Add(Convert.ToString(linha["BL"]));
                                ocultura.quant.Add(0M);
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

                    // oledbcomm;
                    // strodbc, path;
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
   
    }
}
