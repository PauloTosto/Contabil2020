using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.Collections;

using ClassConexao;


namespace ClassApropriaçoes
{
    public class CalculaTabelasProvisorias
    {
        public DataTable tableprov;
        public DataTable tableprovMensal;
        public DataTable resumoquadras;
        public DataTable resumocentros;
   
        public Decimal precomediocacau, precomediocacaufino, precomedioseringa;
        public Boolean criereceitacacau, ifinanceiro, iprevisto, itrabclt, isotrabclt, iforanolanc, igerencial, iencargos;
        public Boolean iencargosseparados, naopegprev;
       // Dictionary<string, tobjfin_aprop> listaprop;
        public DateTime inicio, fim;
        public ClassModelo oclassmodelo;
        public ClassServico oclassservico;
        public ClassCentro oclasscentro;
        // as classes que correspondem a registros

      /*  class tregcentro
        {
            public DateTime data;
            public string num, codser, setor, centro, quadra, historico;
            public Int16 tipo;
            public Single valor, quant, valorcalc;
            public Int64 nreg;
        }
        class tregestoque
        {
            public DateTime data;
            public string num, codser, setor, centro, quadra;
            public string item, deposito;
            public Int16 tipo;
            public Single valor, quant, valorcalc;
            public Int64 nreg;
        }
        public class tregmovfin
        {
            public DateTime data;
            public string conta, hist, tipo; // D ou C
            public string docfisc, obs;
            public Single valor, valorcalc;
            public Int64 nreg;
        }
        public class objreg
        {
            public List<object> listreg;
            public Int64 regaprop;
            public int aprop_est;
            public Single vlr_ap, vlrfiltro_ap;
        }

        public class tobjfin_aprop
        {
            public List<objreg> listobjreg;
            public Single tvalormestre, tvalorfin;// : Currency;
            public Int16 ent_sai;
            public DateTime data;
            public tregmovfin regmovfin;
        }
        */


        public CalculaTabelasProvisorias()
        {
            precomediocacau = 0;
            precomediocacaufino = 0;
            criereceitacacau = false;
            precomedioseringa = 0;
            ifinanceiro = false;
            iprevisto = false;
            itrabclt = false;
            isotrabclt = false;
            iforanolanc = false;
            igerencial = false;
            iencargos = false;
            iencargosseparados = false;
            naopegprev = false;
            tableprov = new DataTable();
            tableprovMensal = new DataTable(); 
            
            try
            {
                oclassmodelo = new ClassModelo();
            }
            catch
            {
                oclassmodelo = null;
            }
            try
            {
                oclassservico = new ClassServico();
            }
            catch
            {
                oclassservico = null;
            }
            try
            {
                oclasscentro = new ClassCentro();
            }
            catch
            {
                oclasscentro = null;
            }
            resumoquadras = new DataTable("ResumoQudras");
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "QUADRA", 3, false));
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETOR", 2, false));
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CENTRO", 4, false));
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "AREA", 0, false));
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "RECEITA_SERINGA", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "RECEITA_CACAU", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "RECEITA_CACAUFINO", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PROD_CACAU", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PARC_CACAU", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PROD_SERINGA", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PARC_SERINGA", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;

            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PROD_CACAUFINO", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PARC_CACAUFINO", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "MAO_OBRA_CUST", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "MAO_OBRA_INVEST", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "MAO_OBRA", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "INSUMOS", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "TERCEIROS", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "EQUIPAMENTOS", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumoquadras.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "RATEIOS", 0, false));
            resumoquadras.Columns[resumoquadras.Columns.Count - 1].DefaultValue = 0;
            resumocentros = resumoquadras.Clone();

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = resumoquadras.Columns["QUADRA"];
            resumoquadras.PrimaryKey = PrimaryKeyColumns;

            PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = resumocentros.Columns["Centro"];
            resumocentros.PrimaryKey = PrimaryKeyColumns;
 


          //  listaprop = new Dictionary<string, tobjfin_aprop>();
        }


     
        Boolean pesqegrave(string tsetor, string tcentro, string tquadra, string tnum, string tcodser,
                         Int16 ttipomov, Int16 tipoarq, Int16 ent_sai, Single valor, Single tquant, string tproduto,
                              Single totarea,Int16 tfino)
        {

            if (valor == 0) return true;
            DataRow orow = tableprov.NewRow();


            orow["SAIDA"] = 0;
            orow["ENTRADA"] = 0;
            orow["SETOR"] = tsetor;
            orow["CENTRO"] = tcentro;
            orow["QUADRA"] = tquadra;
            orow["NUM_MOD"] = tnum;
            orow["CODSER"] = tcodser;
            orow["ICODSER"] = ttipomov;
            orow["ORIGEM"] = tipoarq;
            orow["PROD"] = tproduto;
            orow["FINO"] = tfino;
            orow["quant"] = tquant;
            orow["area"] = totarea;
            if (ent_sai == 1)
                orow["SAIDA"] = (valor * -1);
            else
                orow["SAIDA"] = valor;


            tableprov.Rows.Add(orow);
            return true;

        }

        Boolean pesqegraveMensal(DateTime mensal, string tsetor, string tcentro, string tquadra, string tnum, string tcodser,
                        Int16 ttipomov, Int16 tipoarq, Int16 ent_sai, Single valor, Single tquant, string tproduto,
                             Single totarea, Single hefetivas)
        {

            if (valor == 0) return true;
            DataRow orow = tableprovMensal.NewRow();
            //em fino vou colocar as horas efetivas trabalhadas

            orow["SAIDA"] = 0;
            orow["ENTRADA"] = 0;
            orow["SETOR"] = tsetor;
            orow["DATA"] = mensal;
            orow["CENTRO"] = tcentro;
            orow["QUADRA"] = tquadra;
            orow["NUM_MOD"] = tnum;
            orow["CODSER"] = tcodser;
            orow["ICODSER"] = ttipomov;
            orow["ORIGEM"] = tipoarq;
            orow["PROD"] = tproduto;
            orow["HEFETIVA"] = hefetivas;
            orow["quant"] = tquant;
            orow["area"] = totarea;
            if (ent_sai == 1)
                orow["SAIDA"] = (valor * -1);
            else
                orow["SAIDA"] = valor;


            tableprovMensal.Rows.Add(orow);
            return true;

        }





        Boolean altereegrave(DataTable otable, string tsetor, string tcentro, string tquadra, string tproduto,
                         Int16 ttipomov, double valor, double tquant,
                              double totarea)
        {
            return true;
        }



        public Boolean PrepareCompleto()
        {

           
        //    if (!(Classprovrel2.SQLCreateTableprovrel2("FINAN"))) return false;


            DataSet dsArea = peghectares();


           // OleDbDataAdapter oadapter = Classprovrel3.provrel3ConstruaAdaptador("FINAN");
            DataSet dstableprov = new DataSet();
            tableprov = Classprovrel3.dtprovrel3();
           // dstableprov.Tables.Add(tableprov);
           // Classprovrel3.SQLCreateTableprovrel3("FINAN");
            // oadapter.Fill(tableprov);
            

            tableprovMensal = Classprovrel3.dtprovrel3();
            tableprovMensal.TableName = "PREVRELMENSAL";
            tableprovMensal.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DATA", 0, false));
            tableprovMensal.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HEFETIVA", 0, false));

            DataSet odataset = Get_Cltfolha_Quadra(inicio, fim);
            // Folha Trabalhadores (tipo = 1)
            Decimal totfolha = 0M;
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                pesqegrave(orow["SETOR"].ToString(), orow["CENTRO"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 1, 4, 2,( Convert.ToSingle(orow["tSAL"]) +
                   Convert.ToSingle(orow["tGRATIF"]) +
                Convert.ToSingle(orow["tHXA"]) + Convert.ToSingle(orow["tHXN"]) +
                Convert.ToSingle(orow["tHXS"]) + Convert.ToSingle(orow["tfgts"]) +
                Convert.ToSingle(orow["tferias"]) + Convert.ToSingle(orow["tfgts_fe"]) +
                 Convert.ToSingle(orow["tdecimo"]) + Convert.ToSingle(orow["tfgts_dec"]) +
                 Convert.ToSingle(orow["teduc"]) + Convert.ToSingle(orow["tterc"])),
                Convert.ToSingle(orow["qrquant"]), "", 0,0);
                if (orow["QUADRA"].ToString().Trim() != "")
                    totfolha = totfolha + (Convert.ToDecimal(orow["tSAL"]) +
                   Convert.ToDecimal(orow["tGRATIF"]) +
                Convert.ToDecimal(orow["tHXA"]) + Convert.ToDecimal(orow["tHXN"]) +
                Convert.ToDecimal(orow["tHXS"]) + Convert.ToDecimal(orow["tfgts"]) +
                Convert.ToDecimal(orow["tferias"]) + Convert.ToDecimal(orow["tfgts_fe"]) +
                 Convert.ToDecimal(orow["tdecimo"]) + Convert.ToDecimal(orow["tfgts_dec"]) +
                 Convert.ToDecimal(orow["teduc"]) + Convert.ToDecimal(orow["tterc"]));

            }


            //MAO DE OBRA MENSALMENTE

            odataset = Get_CltfolhaMensal_Quadra(inicio, fim);
            // Folha Trabalhadores (tipo = 1)
            totfolha = 0M;
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                pesqegraveMensal(Convert.ToDateTime(orow["DATA"]),orow["SETOR"].ToString(), orow["CENTRO"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 1, 4, 2, (Convert.ToSingle(orow["tSAL"]) +
                   Convert.ToSingle(orow["tGRATIF"]) +
                Convert.ToSingle(orow["tHXA"]) + Convert.ToSingle(orow["tHXN"]) +
                Convert.ToSingle(orow["tHXS"]) + Convert.ToSingle(orow["tfgts"]) +
                Convert.ToSingle(orow["tferias"]) + Convert.ToSingle(orow["tfgts_fe"]) +
                 Convert.ToSingle(orow["tdecimo"]) + Convert.ToSingle(orow["tfgts_dec"]) +
                 Convert.ToSingle(orow["teduc"]) + Convert.ToSingle(orow["tterc"])),
                Convert.ToSingle(orow["qrquant"]), "", 0, Convert.ToSingle(orow["thefetiva"]));
                if (orow["QUADRA"].ToString().Trim() != "")
                    totfolha = totfolha + (Convert.ToDecimal(orow["tSAL"]) +
                   Convert.ToDecimal(orow["tGRATIF"]) +
                Convert.ToDecimal(orow["tHXA"]) + Convert.ToDecimal(orow["tHXN"]) +
                Convert.ToDecimal(orow["tHXS"]) + Convert.ToDecimal(orow["tfgts"]) +
                Convert.ToDecimal(orow["tferias"]) + Convert.ToDecimal(orow["tfgts_fe"]) +
                 Convert.ToDecimal(orow["tdecimo"]) + Convert.ToDecimal(orow["tfgts_dec"]) +
                 Convert.ToDecimal(orow["teduc"]) + Convert.ToDecimal(orow["tterc"]));

            }


           /// PRODUÇÂO 
           /// 
            ///// Pesquisas Para a Producao e Produtividade das Áreas
            // Só cacau para que possam ser inseridos a partir da tabela segue o tipo do cacau (1-fino, etc..
            odataset = Get_Producao_Fino_Quadra(inicio, fim);
            Single area = 0.00F;//so cacau
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];
                DataRow rowarea = dsArea.Tables[0].Rows.Find(orow["QUA"].ToString());
                if (rowarea != null)
                    area = Convert.ToSingle(rowarea["AREA"]);
                else
                    area = 0.00F;
                string cacaufino = "  1";// orow["PROD"].ToString();

                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUA"].ToString(), "",
                "", 8, 2, 2, Convert.ToSingle(orow["tprod"]) + Convert.ToSingle(orow["tremun"]),
                  Convert.ToSingle(orow["tparceria"]), cacaufino, area, Convert.ToInt16(orow["tipocacau"]));

            }
            // todos os outros produtos menos cacau..
            odataset = Get_Producao_Quadra(inicio, fim);
            area = 0.00F;
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {

                DataRow orow = odataset.Tables[0].Rows[i];
                if (orow["PROD"].ToString() == "  1") continue;
                DataRow rowarea = dsArea.Tables[0].Rows.Find(orow["QUA"].ToString());
                if (rowarea != null)
                    area = Convert.ToSingle(rowarea["AREA"]);
                else
                    area = 0.00F;

                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUA"].ToString(), "",
                "", 8, 2, 2, Convert.ToSingle(orow["tprod"]) + Convert.ToSingle(orow["tremun"]),
                  Convert.ToSingle(orow["tparceria"]), orow["PROD"].ToString(), area, 0);

            }
         
            

            
            
            
            
            
            
            
            odataset = Get_Insumo_Quadra(inicio, fim);

            // Insumos do estoque (tipo = 2)
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 2, 2, 2, Convert.ToSingle(orow["tval"]),
                0, "", 0,0);
            }

            /*
             * // Pegar os grupos a partir do CTACENTR (lançamento gerenciais)


            // Grupo INSUMOS
            {
             Fertilizantes 11
             Inseticidas 12
             Insumos/Mat  2
             Ração    13
             Combustível 15
             Mat.Divs  10

              // Insumos Lancados Diretos (tipo = 3)
            }
  
             */
            // alterei esses lançamentos para permitir que os lançamentos que não houvessem a informação sobre
            // a quadra pudesse ser 
            odataset = Get_CtaCentro3_Quadra(inicio, fim,tableprov,oclassmodelo,dsArea.Tables[0]);
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 3, 2, 2, Convert.ToSingle(orow["tval"]),
                0, "", 0,0);
            }

            /*  // Grupo SERVICOS DE TERCEIROS
              {
               Servicos 1
               Viagem   8
               Frete    9

                // (tipo = 4)
              }

             */

            odataset = Get_CtaCentro4_Quadra(inicio, fim);
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 4, 2, 2, Convert.ToSingle(orow["tval"]),
                0, "", 0,0);
            }


            // Grupo MAQ e EQUIPAMENTOS
            /*{
             Maq e eQuipamento 14
             Peças de Reposição

              // (tipo = 5)
            }*/
            odataset = Get_CtaCentro5_Quadra(inicio, fim);
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 5, 2, 2, Convert.ToSingle(orow["tval"]),
                0, "", 0,0);
            }


            //   Todo Tipo de Credito

            // (tipo = 21)

            odataset = Get_ReceitaCtaCentro5_Quadra(inicio, fim);
            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];
                pesqegrave(orow["SETOR"].ToString(), orow["GLEBA"].ToString(), orow["QUADRA"].ToString(), orow["NUM_MOD"].ToString(),
                  orow["CODSER"].ToString(), 21, 2, 1, Convert.ToSingle(orow["tval"]),
                0, "", 0,0);
            }

            // Grupo RATEIO DB E CR

            // todos os tipo de rateio

            // (tipo = 11) e 21


            odataset = Get_RateioCtaCentro_Quadra(inicio, fim);

            for (int i = 0; i < odataset.Tables[0].Rows.Count - 1; i++)
            {
                DataRow orow = odataset.Tables[0].Rows[i];

                if (Convert.ToSingle(orow["tval_fr"]) != 0)
                {
                    pesqegrave(orow["SETOR"].ToString(), orow["CENTRO"].ToString(), "", orow["NUM_MOD"].ToString(),
                      orow["CODSER"].ToString(), 11, 2, 2, Convert.ToSingle(orow["tval_fr"]),
                    0, "", 0,0);
                    pesqegrave(orow["SETOR_CRE"].ToString(), orow["CENTRO_CRE"].ToString(), "", orow["NUM_MOD_CR"].ToString(),
                  orow["CODSER_CRE"].ToString(), 11, 2, 1, Convert.ToSingle(orow["tval_fr"]),
                0, "", 0,0);
                }
                if (Convert.ToSingle(orow["tval_pa"]) != 0)
                {
                    pesqegrave(orow["SETOR_CRE"].ToString(), orow["CENTRO_CRE"].ToString(), "", orow["NUM_MOD_CR"].ToString(),
               orow["CODSER_CRE"].ToString(), 21, 2, 1, Convert.ToSingle(orow["tval_fr"]),
             0, "", 0,0);
                }
            }

            
            

            try
            {

                //oadapter.Update(tableprov);
                //TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Close();
               
            }
            catch (OleDbException e)
            {
                string errorMessages = "";

                for (int i = 0; i < e.Errors.Count; i++)
                {
                    errorMessages += "Index #" + i + "\n" +
                                     "Message: " + e.Errors[i].Message + "\n" +
                                     "NativeError: " + e.Errors[i].NativeError + "\n" +
                                     "Source: " + e.Errors[i].Source + "\n" +
                                     "SQLState: " + e.Errors[i].SQLState + "\n";
                }
                MessageBox.Show(errorMessages);
                throw;
            }



            tableprov.AcceptChanges();
            //}

            /*
             *  // Grupo INSUMOS
          {
           Fertilizantes 11
           Inseticidas 12
           Insumos/Mat  2
           Ração    13
           Combustível 15
           Mat.Divs  10

            // Insumos Lancados Diretos (tipo = 3)
          }

             */





            
           
            return true;
        }












        //ESTATISTICA CUSTO QUADRA MENSAL
        #region

        static public DataSet Get_CltfolhaMensal_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            // string oprod = "  1";
            strOleDb = "SELECT DATA,SETOR,CENTRO,NUM_MOD,CODSER,QUADRA, SUM(SALARIO) as tsaL, SUM(GRATIF) as tgratif ,SUM(VLR_HXA) as thxa, SUM(VLR_HXN) as tHXN,  SUM(VLR_HXS) as thxs , SUM(SALFAM) tsalfam, SUM(INSS) as tinss," +
            " SUM(QUANT) as QrQuant, SUM(EDUC) as teduc, SUM(TERC) as tTerc, SUM(FGTS) as tfgts, " +
            " SUM(FGTS_FE) as tfgts_fe, SUM(FERIAS) as tferias, SUM(DECIMO) as tdecimo, SUM(FGTS_DEC) as tfgts_dec , SUM(HEFETIVA) as tHEfetiva " +
            "  FROM " + path + "CLTFOLHA" +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
            " group by Setor,QUADRA,DATA,Num_mod,CODSER,Centro" +
            " order by Setor,QUADRA,DATA,Num_mod,CODSER,Centro";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;

                //return OleDbcomm.ExecuteReader();

            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }



        #endregion



        //ESTATISTICA CUSTO QUADRA
        #region

        static public DataSet Get_Cltfolha_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            // string oprod = "  1";
            strOleDb = "SELECT SETOR,CENTRO,NUM_MOD,CODSER,QUADRA, SUM(SALARIO) as tsaL, SUM(GRATIF) as tgratif ,SUM(VLR_HXA) as thxa, SUM(VLR_HXN) as tHXN,  SUM(VLR_HXS) as thxs , SUM(SALFAM) tsalfam, SUM(INSS) as tinss," +
            " SUM(QUANT) as QrQuant, SUM(EDUC) as teduc, SUM(TERC) as tTerc, SUM(FGTS) as tfgts, " +
            " SUM(FGTS_FE) as tfgts_fe, SUM(FERIAS) as tferias, SUM(DECIMO) as tdecimo, SUM(FGTS_DEC) as tfgts_dec " +
            "  FROM " + path + "CLTFOLHA" +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") )" +
            " group by Setor,QUADRA,Num_mod,CODSER,Centro" +
            " order by Setor,QUADRA,Num_mod,CODSER,Centro";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;

                //return OleDbcomm.ExecuteReader();

            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        static public DataSet Get_Insumo_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("ESTOQUE");
            // string oprod = "  1";

            strOleDb = "SELECT SETOR,QUADRA, GLEBA,NUM_MOD,CODSER, SUM(VLR_FR) as tval, SUM(QUANT_FR) as tquant  " +
            "  FROM " + path + "MOVEST " +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
            " AND (TIPO = 'S')  AND (TIPO2 = '' ) " +
            " group by Setor,QUADRA,Num_mod,CODSER,GLEBA" +
            " order by Setor,QUADRA,Num_mod,CODSER,GLEBA";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "MOVEST");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

             







                return odataset;

                //return OleDbcomm.ExecuteReader();

            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }

        static public DataSet Get_CtaCentro3_Quadra(DateTime inicio, DateTime fim, DataTable tableprov,ClassModelo omodelo,DataTable areas)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("FINAN");
            string pathtrab = TDataControlReduzido.Get_Path("TRABALHO");

            strOleDb = "SELECT SETOR,GLEBA,QUADRA,NUM_MOD,CODSER, SUM(VALOR) as tval " +
            "  FROM " + path + "CTACENTR " +
            " inner join " + pathtrab + "TIPOMOV on (ctacentr.icodser = tipomov.tpmov) " +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
            " AND (DBCR = 'D')  AND  " +
            " (( GRUPO = 3) or (ICODSER = 0 ) ) " +
            " group by Setor,GLEBA,QUADRA,Num_mod,CODSER" +
            " order by Setor,GLEBA,QUADRA,Num_mod,CODSER";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CTACENTR");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                if (omodelo == null) return odataset; // nao é possivel trabalhar o rateio
                Decimal totalteste = 0M;
                foreach (DataRow orow in odataset.Tables[0].AsEnumerable())
                {
                    totalteste = totalteste + Convert.ToDecimal(orow["tval"]);
                }



                DataTable insumos = odataset.Tables[0].Clone();
                DataRow rowinsumosex;
                foreach (DataRow orow in odataset.Tables[0].AsEnumerable())
                {

                    if (orow["QUADRA"].ToString().Trim() != "")
                    {
                        rowinsumosex = insumos.NewRow();
                        rowinsumosex.ItemArray = orow.ItemArray;
                        insumos.Rows.Add(rowinsumosex);
                        continue;
                    }
                    string produto = omodelo.ModeloProduto(orow["NUM_MOD"].ToString());

                    DataRow[] rowsproduto = tableprov.Select("(SETOR = '" + orow["SETOR"].ToString()
                                                         + "') AND (CENTRO = '" + orow["GLEBA"].ToString() + "')" +
                                                         " AND (PROD = '" + produto + "') " +
                                                         " AND (QUADRA <> '')" +
                                                         " AND (SAIDA > 0 )" + " AND (ICODSER =  8) ", "QUADRA");
                    if (rowsproduto.Length == 0)
                    {
                        rowinsumosex = insumos.NewRow();
                        rowinsumosex.ItemArray = orow.ItemArray;
                        insumos.Rows.Add(rowinsumosex);
                        continue;
                    }
                    var quadras =
                                   from linha in rowsproduto.AsEnumerable()
                                   where (linha.Field<string>("QUADRA").Trim() != "")
                                   group linha by new
                                   {
                                       quadra = linha.Field<string>("QUADRA")

                                   } into g
                                   orderby g.Key.quadra
                                   select new
                                   {
                                       quadra = g.Key.quadra,
                                       totprod = g.Sum((linha => linha.Field<Decimal?>("SAIDA")))
                                   };

                    Decimal totalarea = 0M;
                    Decimal totalproducao = 0M;
                    Decimal totalvalor = Convert.ToDecimal(orow["tval"]);
                    Boolean areavazia = false;
                    foreach (var campo in quadras)
                    {

                        DataRow rowarea = areas.Rows.Find(campo.quadra);
                        if (rowarea != null)
                            totalarea = totalarea + Convert.ToDecimal(rowarea["AREA"]);
                        else
                            areavazia = true;
                        totalproducao = totalproducao + Convert.ToDecimal(campo.totprod);
                    }
                    if (areavazia == false)
                    {

                        // criterio area
                        Decimal area = 0M;
                        DataRow maiorrow = null;
                        Decimal maiorvalor = 0M;
                        Decimal difvalor = Convert.ToDecimal(orow["tval"]);
                        foreach (var campo in quadras)
                        {
                            DataRow rowarea = areas.Rows.Find(campo.quadra);
                            area = 0;
                            if (rowarea != null)
                                area = Convert.ToDecimal(rowarea["AREA"]);

                            rowinsumosex = insumos.NewRow();
                            rowinsumosex.ItemArray = orow.ItemArray;
                            rowinsumosex["QUADRA"] = campo.quadra;
                            rowinsumosex["tval"] = Decimal.Round(Convert.ToDecimal(orow["tval"]) * (area / totalarea),2);
                            if (Convert.ToDecimal(rowinsumosex["tval"]) > maiorvalor)
                            {
                                maiorvalor = Convert.ToDecimal(rowinsumosex["tval"]);
                                maiorrow = rowinsumosex;
                            }
                            insumos.Rows.Add(rowinsumosex);
                            difvalor = difvalor - Convert.ToDecimal(rowinsumosex["tval"]);
                        }
                        if (difvalor != 0)
                        {
                            if (maiorrow != null)
                            {
                                maiorrow.BeginEdit();
                                maiorrow["tval"] = Convert.ToDecimal(maiorrow["tval"]) + difvalor;
                                maiorrow.EndEdit();
                            }

                        }
                    }
                    else
                    {
                        rowinsumosex = insumos.NewRow();
                        rowinsumosex.ItemArray = orow.ItemArray;
                        insumos.Rows.Add(rowinsumosex);
                    }

                }

                DataSet novodataset = new DataSet();
                novodataset.Tables.Add(insumos);
                return novodataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        static public DataSet Get_CtaCentro4_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("FINAN");
            string pathtrab = TDataControlReduzido.Get_Path("TRABALHO");

            strOleDb = "SELECT SETOR,GLEBA,QUADRA,NUM_MOD,CODSER, SUM(VALOR) as tval " +
            "  FROM " + path + "CTACENTR " +
            " inner join " + pathtrab + "TIPOMOV on (ctacentr.icodser = tipomov.tpmov) " +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
            " AND (DBCR = 'D')  AND  " +
            "  ( GRUPO = 4)   " +
            " group by Setor,GLEBA,QUADRA,Num_mod,CODSER" +
            " order by Setor,GLEBA,QUADRA,Num_mod,CODSER";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CTACENTR");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }

        static public DataSet Get_CtaCentro5_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("FINAN");
            string pathtrab = TDataControlReduzido.Get_Path("TRABALHO");

            strOleDb = "SELECT SETOR,GLEBA,QUADRA,NUM_MOD,CODSER, SUM(VALOR) as tval " +
            "  FROM " + path + "CTACENTR " +
            " inner join " + pathtrab + "TIPOMOV on (ctacentr.icodser = tipomov.tpmov) " +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
            " AND (DBCR = 'D')  AND  " +
            " ( GRUPO = 5)   " +
            " group by Setor,GLEBA,QUADRA,Num_mod,CODSER" +
            " order by Setor,GLEBA,QUADRA,Num_mod,CODSER";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CTACENTR");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }


        static public DataSet Get_ReceitaCtaCentro5_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("FINAN");
            string pathtrab = TDataControlReduzido.Get_Path("TRABALHO");

            strOleDb = "SELECT SETOR,GLEBA,QUADRA,NUM_MOD,CODSER, SUM(VALOR) as tval " +
            "  FROM " + path + "CTACENTR " +
            " inner join " + pathtrab + "TIPOMOV on (ctacentr.icodser = tipomov.tpmov) " +
            " where " +
            " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
            " AND (DBCR = 'C')  AND  " +
            " ( GRUPO = 5)   " +
            " group by Setor,GLEBA,QUADRA,Num_mod,CODSER" +
            " order by Setor,GLEBA,QUADRA,Num_mod,CODSER";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CTACENTR");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }

        static public DataSet Get_RateioCtaCentro_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");

            strOleDb = "SELECT SETOR,CENTRO,NUM_MOD,CODSER,SETOR_CRE,CENTRO_CRE,NUM_MOD_CR,CODSER_CRE,SUM(VLR_FR) as tval_Fr ,SUM(VLR_PA) as tval_Pa" +
               "  FROM " + path + "RATEIODB" +
               " where " +
               " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
            " group by Setor,Centro,Num_mod,CodSer,SETOR_CRE,CENTRO_CRE,NUM_MOD_CR,CODSER_CRE " +
            " order by Setor,Centro,Num_mod,Codser,SETOR_CRE,CENTRO_CRE,NUM_MOD_CR,CODSER_CRE ";


            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "RATEIODB");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        // todos produtos menos cacau
        static public DataSet Get_Producao_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            string pathcampo = TDataControlReduzido.Get_Path("PATRIMONIO");

            strOleDb = "Select Setor, Gleba, Qua,PROD  , SUM(QUANT_FR) as tprod, SUM(QUANT_PA) as tparceria, SUM(QUANT_RI) as tRemun  FROM " + path + "Produto" +
           " where " +
               " (DATAP  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
                      " AND (PROD <> '  1') "+
        " group by setor,Gleba,QUA,Prod " +//Glebas."Desc"
        " order by setor,Gleba,QUA,Prod ";

            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "PRODUTO");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        
        // só cacau (porcausa do fino vir descriminado...)
        static public DataSet Get_Producao_Fino_Quadra(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            strOleDb = "Select produto.Setor, produto.datap,produto.Gleba, produto.Qua,produto.safra,produto.lote, SUM(QUANT_FR) as tprod, SUM(QUANT_PA) as tparceria," +
                "SUM(QUANT_RI) as tRemun  FROM " + path + "Produto " +
           " where " +
               " (DATAP  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
                       " AND (Produto.PROD = '  1') " +
            " group by produto.setor,produto.safra,produto.lote,produto.QUA, produto.datap ,produto.gleba" +//Glebas."Desc"
            " order by produto.setor,produto.safra,produto.lote,produto.QUA, produto.datap ,produto.gleba ";

            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "PRODUTO");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                DataSet dssegue = Get_Segue(inicio, fim);

                DataTable produtoex = odataset.Tables[0].Clone();
                produtoex.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Int16"), "TipoCacau", 0, false));


                DataTable copiatabela = dssegue.Tables[0].Copy();

                var grupo =
                         from linha in copiatabela.AsEnumerable()
                         group linha by new
                         {
                             lote = linha.Field<string>("LOTE"),
                             setor = linha.Field<string>("SETOR"),
                             safra = linha.Field<string>("Safra")

                         } into g
                         orderby g.Key.setor, g.Key.safra, g.Key.lote
                         select new
                         {
                             lote = g.Key.lote,
                             setor = g.Key.setor,
                             safra = g.Key.safra,
                             total = g.Sum((linha => linha.Field<Decimal?>("QUANT"))),
                             totalperc = g.Sum((linha => linha.Field<Decimal?>("PERC")))

                         };

                /* para cada setor/safra/lote redistribua os valores de cada quadra desde lote na proporção dos tipos existentes no segue */
                foreach (var camposlote in grupo)  
                {
                    if ((decimal)camposlote.totalperc != 1) 
                        continue;
                    DataRow[] rowssegue = dssegue.Tables[0].Select("(SETOR = '" + camposlote.setor + "') AND (LOTE = '" + camposlote.lote + "')" +
                                                              " AND (SAFRA = '" + camposlote.safra + "')", "SCS");
                                      
                    
                    DataRow[] rowsproduto = odataset.Tables[0].Select("(SETOR = '" + camposlote.setor + "') AND (LOTE = '" + camposlote.lote + "')" +
                                                              " AND (SAFRA = '" + camposlote.safra + "')", "QUA");
                    
                    // para cada linha(quadra) de produto deste lote irei subdividir os valores em função da proporção de cada tipo do segue
                    foreach (DataRow rowproduto in rowsproduto)
                    {
                        Decimal tprod = Convert.ToDecimal(rowproduto["TPROD"]);
                        Decimal tparceria = Convert.ToDecimal(rowproduto["TPARCERIA"]);
                        Decimal tremun = Convert.ToDecimal(rowproduto["TREMUN"]);
                        Decimal maiorprod = 0M;
                        DataRow MaiorRow = null;

                        foreach (DataRow rowsegue in rowssegue)
                        {
                            if (Convert.ToDecimal(rowsegue["PERC"]) == 0)
                                continue;

                            DataRow rowextend = produtoex.NewRow();
                            rowextend["SETOR"] = rowproduto["SETOR"];
                            rowextend["DATAP"] = rowproduto["DATAP"];
                            rowextend["QUA"] = rowproduto["QUA"];
                            rowextend["GLEBA"] = rowproduto["GLEBA"];
                            rowextend["SAFRA"] = rowproduto["SAFRA"];
                            rowextend["LOTE"] = rowproduto["LOTE"];
                            rowextend["tipocacau"] = rowsegue["SCS"];
                            rowextend["TPROD"] = Decimal.Round(Convert.ToDecimal(rowproduto["TPROD"]) * Convert.ToDecimal(rowsegue["PERC"]), 3);
                            rowextend["TPARCERIA"] = Decimal.Round(Convert.ToDecimal(rowproduto["TPARCERIA"]) * Convert.ToDecimal(rowsegue["PERC"]), 3);
                            rowextend["TREMUN"] = Decimal.Round(Convert.ToDecimal(rowproduto["TREMUN"]) * Convert.ToDecimal(rowsegue["PERC"]), 3);
                            produtoex.Rows.Add(rowextend);
                            if (maiorprod < Convert.ToDecimal(rowextend["tprod"]))
                            {
                                maiorprod = Convert.ToDecimal(rowextend["tprod"]);
                                MaiorRow = rowextend;
                            }
                            tprod = tprod - Convert.ToDecimal(rowextend["tprod"]);
                            tparceria = tparceria - Convert.ToDecimal(rowextend["tparceria"]);
                            tremun = tremun - Convert.ToDecimal(rowextend["tremun"]);
                        }
                        if ((tprod != 0) || (tparceria != 0) || (tremun != 0))
                        {
                            MaiorRow.BeginEdit();
                            MaiorRow["tprod"] = Convert.ToDecimal(MaiorRow["tprod"]) + tprod;
                            MaiorRow["tparceria"] = Convert.ToDecimal(MaiorRow["tparceria"]) + tparceria;
                            MaiorRow["tremun"] = Convert.ToDecimal(MaiorRow["tremun"]) + tremun;
                            MaiorRow.EndEdit();
                        }

                    }




                    


                }

                DataTable produtoagrupado = produtoex.Clone();
                var agrupado =
                         from linha in produtoex.AsEnumerable()
                         group linha by new
                         {
                             quadra = linha.Field<string>("QUA"),
                             setor = linha.Field<string>("SETOR"),
                             gleba = linha.Field<string>("gleba"),
                             tipocacau = linha.Field<Int16>("tipocacau"),

                         } into g
                         orderby g.Key.setor, g.Key.quadra, g.Key.gleba,g.Key.tipocacau
                         select new
                         {
                             quadra = g.Key.quadra,
                             setor = g.Key.setor,
                             gleba = g.Key.gleba,
                             tipocacau = g.Key.tipocacau,
                             tprod = g.Sum((linha => linha.Field<Decimal?>("tprod"))),
                             tparceria = g.Sum((linha => linha.Field<Decimal?>("tparceria"))),
                             tremun = g.Sum((linha => linha.Field<Decimal?>("tremun")))

                         };
                foreach(var campo in agrupado)
                {
                    DataRow rowextend = produtoagrupado.NewRow();
                    rowextend["SETOR"] = campo.setor;
                    rowextend["QUA"] = campo.quadra;
                    rowextend["GLEBA"] = campo.gleba;
                    rowextend["tipocacau"] = campo.tipocacau;
                    rowextend["TPROD"] = campo.tprod;
                    rowextend["TPARCERIA"] = campo.tparceria;
                    rowextend["TREMUN"] = campo.tremun;
                    produtoagrupado.Rows.Add(rowextend);
                
                }
                
                DataSet novadataset = new DataSet();
                novadataset.Tables.Add(produtoagrupado);
                return novadataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
        static public DataSet Get_Segue(DateTime inicio, DateTime fim)
        {
            OleDbCommand oledbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            strOleDb = "Select Setor, safra,lote, data, QUANT,scs FROM " +
                  path + "SEGUE " +
           " where " +
               " (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(inicio) + ") AND CTOD("
                      + TDataControlReduzido.FormatDataGravar(fim) + ") ) " +
                         " AND (PROD = '  1') " +
            " order by setor,safra,lote,scs ";

            try
            {
                oledbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "SEGUE");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                DataTable copiatabela = odataset.Tables[0].Copy();

                var grupo =
                         from linha in copiatabela.AsEnumerable()
                         // where (linha.Field<string>("SETOR") == " 9")
                         group linha by new
                         {
                             lote = linha.Field<string>("LOTE"),
                             setor = linha.Field<string>("SETOR"),
                             safra = linha.Field<string>("Safra")

                         } into g
                         orderby g.Key.setor, g.Key.safra, g.Key.lote
                         select new
                         {
                             lote = g.Key.lote,
                             setor = g.Key.setor,
                             safra = g.Key.safra,
                             total = g.Sum((linha => linha.Field<Decimal?>("QUANT")))

                         };
                odataset.Tables[0].Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "PERC", 0, false));
                foreach (var camposlote in grupo)
                {
                    if ((decimal)camposlote.total == 0) continue;
                    DataRow[] rowsproduto = odataset.Tables[0].Select("(SETOR = '" + camposlote.setor + "') AND (LOTE = '" + camposlote.lote + "')" +
                                                              " AND (SAFRA = '" + camposlote.safra + "')", "SCS");
                    decimal maiorquant = 0;
                    decimal tot1 = (decimal)camposlote.total;
                    //decimal tdif = tot1 / (decimal)camposlote.caixas;
                    DataRow maiorrow = null;
                    decimal perc = 1M;
                    foreach (DataRow rowprod in rowsproduto)
                    {

                        if (Convert.ToDecimal(rowprod["QUANT"]) > maiorquant)
                        {
                            maiorquant = Convert.ToDecimal(rowprod["QUANT"]);
                            maiorrow = rowprod;
                        }
                        rowprod.BeginEdit();
                        rowprod["PERC"] = Convert.ToDecimal(rowprod["QUANT"]) / (decimal)camposlote.total;
                        rowprod.EndEdit();
                        rowprod.AcceptChanges();
                        tot1 = tot1 - Convert.ToDecimal(rowprod["QUANT"]);
                        perc = perc - Convert.ToDecimal(rowprod["PERC"]);

                    }
                    if (perc != 0)
                    {
                        if (maiorrow != null)
                        {
                            maiorrow.BeginEdit();
                            maiorrow["PERC"] = Convert.ToDecimal(maiorrow["PERC"]) + perc;
                            maiorrow.EndEdit();
                            maiorrow.AcceptChanges();
                        }
                    }
                }









                return odataset;
            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "SETORES"));
            }
        }
     

        #endregion

        static public DataSet Get_CltFolha(DateTime data1, DateTime data2)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            // DataSet result = null;
            stroledb = "SELECT SETOR,QUADRA, CENTRO,NUM_MOD,CODSER, SUM(SALARIO) as tsaL ,SUM(GRATIF) as tgratif ,SUM(VLR_HXA) as thxa, SUM(VLR_HXN) as tHXN,  SUM(VLR_HXS) as thxs , SUM(SALFAM) tsalfam, SUM(INSS) as tinss," +
                    " SUM(QUANT) as QrQuant, SUM(EDUC) as teduc, SUM(TERC) as tTerc, SUM(FGTS) as tfgts , " +
          " SUM(FGTS_FE) as tfgts_fe, SUM(FERIAS) as tferias, SUM(DECIMO) as tdecimo, SUM(FGTS_DEC) as tfgts_dec " +
           " FROM " + path + "CLTFOLHA" +  //SUM(FERIAS+DECIMO+FGTS_FE+FGTS_DEC) as QrProvGerais
          " WHERE (CLTFOLHA.DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") AND CTOD(" +
                    TDataControlReduzido.FormatDataGravar(data2) + ") ) ";
            stroledb += " GROUP by Setor,QUADRA,Num_mod,CodSer,Centro";

            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;
            }

        }

        static public DataSet peghectares()
        {
            OleDbCommand oledbcomm;
            string path;
            path = TDataControlReduzido.Get_Path("PATRIMONIO");

            string stroledb = "SELECT qua, ha, air, cent, aceiro, estrada, real FROM " + path + "AREAS";//where " +
            // " (QUA = '" + quadra + "')";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(oledbcomm);
                OleDbda.TableMappings.Add("Table", "PRODUTO");
                DataSet result = new DataSet();
                OleDbda.Fill(result);
                OleDbda.Dispose();
                result.Tables[0].Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "AREA", 0, false));
                foreach (DataRow orow in result.Tables[0].Rows)
                {
                    orow.BeginEdit();
                    orow["area"] = ((Convert.ToDecimal(orow["HA"]) * 10000) + (Convert.ToDecimal(orow["AIR"]) * 100)
                        + Convert.ToDecimal(orow["CENT"])) / 10000;
                    orow.EndEdit();
                }
                result.Tables[0].AcceptChanges();
                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = result.Tables[0].Columns["QUA"];
                result.Tables[0].PrimaryKey = PrimaryKeyColumns;

                //DataColumn okey = new DataColumn(
                return result;

            }
            catch (Exception)
            {
                throw;// new Exception(string.Format("{0}Não foi possivel acessar", path + "AREAS"));
            }

        }



    }

}
/*
 *   SELECT SETOR,GLEBA,QUADRA,NUM_MOD,CODSER, SUM(VALOR) as tval  
              FROM   CTACENTR  
             inner join    TIPOMOV on (ctacentr.icodser = tipomov.tpmov)  
             where  
             (DATA  BETWEEN  CTOD(  '04/01/2011'  ) AND CTOD(
                       '03/31/2012'  ) )  
             AND (DBCR = 'D')  AND   
             (( GRUPO = 3) or (ICODSER = 0 ) )  
             group by Setor,GLEBA,QUADRA,Num_mod,CODSER 
             order by Setor,GLEBA,QUADRA,Num_mod,CODSER;


*/