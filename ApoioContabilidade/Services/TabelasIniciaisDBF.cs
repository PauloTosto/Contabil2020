using ApoioContabilidade.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApoioContabilidade.Services
{
    public static class TabelasIniciaisDBF
    {
        static private DataSet dsTabelasIniciais;
        static private bool inprocess = false;

        static public  bool Execute()
        {

            inprocess = true;
            List<string> sql = new List<string>();
            List<string> nometabela = new List<string>();
             // string str = "Select NBANCO, NOME_BANCO, DESC2,CONTAB FROM Bancos";
             /* sql.Add(str); // 
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
            */

            bool result = true;
            try
            {
                dsTabelasIniciais = new DataSet(); //      await ApiServices.Api_QueryMulti(sql);
                // DdsPesquisa = new DataSet();
                dsTabelasIniciais.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
                dsTabelasIniciais.Tables.Add(TDataControlContab.BancosContab().Copy());



                /*if (dsTabelasIniciais.Tables.Count == nometabela.Count)
                {
                    for (int i = 0; i < dsTabelasIniciais.Tables.Count; i++)
                    {
                        dsTabelasIniciais.Tables[i].TableName = nometabela[i].ToUpper().Trim();
                    }
                }*/
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
            catch (Exception)
            {

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
    
        
        // FIRMA
        /*public static string NomeFirma(string pesquise)
        {
            string result = "";
            string tabela = "FIRMA";
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela)) || (pesquise == "")) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("CONTABIL").Trim() == pesquise.Trim())).FirstOrDefault();
            if (orow != null) { result = orow["DESCRI"].ToString().Trim(); }
            return result;
        }
        */
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
                desc2 = pesquise.Substring(0, 1) == "*" ? pesquise.Substring(1) : pesquise;
            }
            catch (Exception)
            {
                return result;
            }
            if ((dsTabelasIniciais == null) || (!dsTabelasIniciais.Tables.Contains(tabela))) return result;
            DataRow orow = dsTabelasIniciais.Tables[tabela].AsEnumerable().
                Where(row => (row.Field<string>("DESC2").Trim() == desc2.Trim())).FirstOrDefault();
            if ((orow != null) && !orow.IsNull("NBANCO")) { result = Convert.ToString(orow["NBANCO"]).Trim().PadLeft(2, Convert.ToChar("0")); }
            return result;
        }

        
       
       
    }


    static public class DadosComumDBF
    {

       /* 
        static public DataTable ModeloCombo;
        static public DataTable ServicoModeloCombo;
        static public DataTable SetoresCombo;
        static public DataTable GlebasCombo;
        static public DataTable GlebasCADCombo;
        static public DataTable QuadrasCombo;
        static public DataTable TipoMovCombo;
        static public DataTable CadestCombo;
        static public DataTable CatProdCombo;
        static public DataTable FirmaCombo;*/
        static public DataTable BancosCombo;
        static public DataTable Desc2Combo;
        static public DataTable Desc2BancosCombo;
        /*static public DataTable ArmazemCombo;
        static public DataTable TipoProdCombo;
        static public DataTable TabSefipCombo;

        static public DataTable ModeloCombo_Cod_Descr;
        static public DataTable ServicoModeloCombo_Cod_Descr;

        static public DataTable Categoria;
        */
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
            DataSet odataset = TabelasIniciaisDBF.DsTabelasInciais();

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
                int maxLengh = odataset.Tables["PLACON"].Columns["DESC2"].MaxLength;
                if (maxLengh < 0)
                {
                    maxLengh = 45;
                }
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

            if ((odataset.Tables.Contains("PLACON")) && (odataset.Tables.Contains("BANCOS")))
            {

                int maxLengh = odataset.Tables["PLACON"].Columns["DESC2"].MaxLength;
                if (maxLengh < 0)
                {
                    maxLengh = 45;
                }
                Desc2BancosCombo = new DataTable();
                Desc2BancosCombo.Columns.Add("DESC2", Type.GetType("System.String"));
                foreach (DataRow row in odataset.Tables["PLACON"].Rows)
                {
                    DataRow combo = Desc2BancosCombo.NewRow();

                    combo["DESC2"] = SubstituaSlash(row["DESC2"].ToString());
                    Desc2BancosCombo.Rows.Add(combo);
                }
                foreach (DataRow row in odataset.Tables["PLACON"].Rows)
                {
                    DataRow combo = Desc2BancosCombo.NewRow();
                    string descAsterisco = "*" + SubstituaSlash(row["DESC2"].ToString().Trim());
                    if (descAsterisco.Length > maxLengh)
                        descAsterisco = descAsterisco.Substring(0, maxLengh);
                    else
                        descAsterisco = descAsterisco.PadRight(maxLengh);
                    combo["DESC2"] = descAsterisco;
                    Desc2BancosCombo.Rows.Add(combo);
                }
                foreach (DataRow row in odataset.Tables["BANCOS"].Rows)
                {
                    DataRow combo = Desc2BancosCombo.NewRow();
                    combo["DESC2"] = row["NBANCO"].ToString().PadLeft(2, '0');// + row["NOME_BANCO"].ToString();
                    Desc2BancosCombo.Rows.Add(combo);
                    
                }




                Desc2Combo.AcceptChanges();

            }





                /*if (odataset.Tables.Contains("MODELO"))
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
                        combo["DESCRICAO"] = row["NUM"].ToString() + " " + SubstituaSlash(row["DESCRICAO"].ToString());
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
                        combo["DESCSERV"] = row["CODSERV"].ToString() + " " + SubstituaSlash(row["DESCSERV"].ToString());
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
                        combo["GLEBA"] = row["COD"].ToString().Trim();
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
                }*/
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
            /*if (odataset.Tables.Contains("TIPOPROD"))
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
                    combo["DESCRICAO"] = SubstituaSlash(row["DESCRICAO"].ToString()).Substring(0, 30);
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
                    combo["DESCRICAO"] = row["COD"] + " " + SubstituaSlash(row["DESCRICAO"].ToString()).Trim();
                    TabSefipCombo.Rows.Add(combo);
                }
                TabSefipCombo.AcceptChanges();
            }*/
        }
        // Testar se o filho pertence ao Parent
        // Serviço => pai Modelo
        // probleMA slash 
        static private string SubstituaSlash(string texto)
        {
            string result = texto.Contains(Convert.ToChar("/")) ? texto.Replace(Convert.ToChar("/"), Convert.ToChar("|")) : texto;
            return result;
        }

        /*static public bool campoServicoSource(string pai_valor, string filho_valor)
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
        }*/

    }
}
