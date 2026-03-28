using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using System.IO;
using ClassConexao;
using ClassFiltroEdite;



namespace ClassLibTrabalho
{
    public enum CltCadastroChamadas
    {
        Padrao,
        Admitidos,
        Demitidos,
        Ambos
    }
    public enum CltRelReajuste
    {
        MensalistasNaDataBase,
        DiversosAnos,
        ComparaMassaSalario
    }


    public class TDataControlTrabalho
    {
        
        static public DataSet Get_CltAdiant(DateTime data1, List<LinhaSolucao> oLista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result = null;
            stroledb = "SELECT * FROM " + path + "CLTADIAN"
            + " WHERE (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(TDataControlReduzido.PrimeiroDiaMes(data1)) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(TDataControlReduzido.UltimoDiaMes(data1)) + ") ) ";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }

            stroledb += " ORDER by trab";

            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTADIAN");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                //   TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Close();

                DataTable tabDeb = odataset.Tables[0].Clone();

                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;
                    tabDeb.Rows.Add(odatarow.ItemArray);

                }
                result = new DataSet();
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            {
                throw;
            }

        }

        


        static public DataSet Get_CltCadastroGeral(DateTime data1, DateTime data2, List<LinhaSolucao> oLista,CltCadastroChamadas ochamadas)
        {
            string criterio_admi_demi = "";
            if (ochamadas == CltCadastroChamadas.Padrao)
            {
                criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ))  AND " +
                           " ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI > CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                            " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() +
                            ")) OR  (CLTCAD.PRAZO > CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) ) ";
            }
            else if (ochamadas == CltCadastroChamadas.Admitidos)
               // mesmo que já tenha sido DEMItido
            {
                criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + "))" +
                   " AND (CLTCAD.ADMI  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) +
                                   ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) )";
            
            }
            else if (ochamadas == CltCadastroChamadas.Demitidos)
            {
                criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() +
                                           ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ))  AND " +
                                           "( (CLTCAD.DEMI <> CTOD(" + TDataControlReduzido.DataVazia() + "))" +
                                " AND  (CLTCAD.DEMI  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) +
                                   ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )  ) " +
                          " OR ( (CLTCAD.PRAZO <> CTOD(" + TDataControlReduzido.DataVazia() + "))" +
                                " AND  (CLTCAD.PRAZO  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) +
                                   ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )  ) ";
            }
            else if (ochamadas == CltCadastroChamadas.Ambos)
            {

                criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + "))" +
                                  " AND (CLTCAD.ADMI  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) +
                                                  ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) )"+
                                                   " OR   ( ( (CLTCAD.DEMI <> CTOD(" + TDataControlReduzido.DataVazia() + "))" +
                                " AND  (CLTCAD.DEMI  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) +
                                   ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )  ) " +
                          " OR ( (CLTCAD.PRAZO <> CTOD(" + TDataControlReduzido.DataVazia() + "))" +
                                " AND  (CLTCAD.PRAZO  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) +
                                   ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )  ) ) ";
                           
            }



            return Get_CltCadastroBruto(data1, data1, oLista, criterio_admi_demi);

        }
         //criterio folha_trabalho
        static public DataSet Get_CltCadastroFolha(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {
            string criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" + TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                        " ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                         " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) ";

            return Get_CltCadastroBruto(data1, data2, oLista, criterio_admi_demi);

        }

        static public DataSet Get_CltCadastroBruto(DateTime data1, DateTime data2, List<LinhaSolucao> oLista,string criterio_admi_demi)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result = null;
            stroledb = " SELECT CODCAD, NOMECAD, GLECAD, SETOR, SEXO, NASC, COD_ADMI, CARTTRAB, SERIE, CARTIDENT, ADMI, DEMI, PRAZO, SALBASE, OPCAO, AVULSO, DEPEND, INSCPIS, CTA_FGTS," +
                         " ULT_ATUAL, CBO, CATEGORIA, VLRFGTS, DTAFGTS, MENSALISTA, TIPODEMI, AVISO, SALRESC, BANCO1, CONTA1, AGENCIA1, BCOAGCC, BANCO_OK, CPF, NUMERO, MAE, PAI, CONJUGE," +
                         " EMICID, EMICTRAB, EMIPIS, ESTCIVIL, COR, NCIDADE, NUF, TITELEITOR, RESERV, RESERV_CAT, TPSANGUE, END_RUA, END_CID, END_UF, END_CEP, DEFIC, TPDEFIC, APRENDIZ, " +
                         " TPMOV, IRDEPEND, DTAAVISO " +
                       "FROM " + path + "CLTCAD  WHERE " + criterio_admi_demi;
                     
                  if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
                  stroledb += " ORDER by CLTCAD.codcad";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTCAD");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                DataSet dsDependentes = TDataControlTrabalho.Get_CltDependentes(data1,data2, null);
                //dsDependentes.Tables["CLTDEPENDENTES"].PrimaryKey = new DataColumn[1] {dsDependentes.Tables["CLTDEPENDENTES"].Columns["CODCAD"]};
                DataSet dsTabelasTrabalhistas = TabelasTrabalhistas();
                DataSet dsReajustes = Get_CltReajuste(data1,data2,null,criterio_admi_demi);
                odataset.Tables[0].Columns.Add("SALARIOREAL", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("SALLIQUIDO", Type.GetType("System.Decimal"));
                odataset.Tables[0].Columns.Add("INSS", Type.GetType("System.Decimal"));

                DataSet dsFerias = Get_CltFerias(data1, data2, null, criterio_admi_demi);
                odataset.Tables[0].Columns.Add("FERIASDATA_PG", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("AQUIS_FIM", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("AQUIS_INI", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("GOZO_FIM", Type.GetType("System.DateTime"));
                odataset.Tables[0].Columns.Add("GOZO_INI", Type.GetType("System.DateTime"));
                
                odataset.Tables[0].Columns.Add("FERIASVENCIDAS", Type.GetType("System.Int16"));
                odataset.Tables[0].Columns.Add("FERIASPROP", Type.GetType("System.Int16"));

                Decimal salmin = TDataControlTrabalho.CalcSalarioMinimo(data2, dsTabelasTrabalhistas.Tables["TABSAL"]);
                
                DataTable tabDeb = odataset.Tables[0].Clone();
                
                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow,null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                   // DataRow odatarow2 =  dsDependentes.Tables["CLTDEPENDENTES"].Rows.Find(odatarow["CODCAD"]);
                   // if (odatarow2 != null)
                   // {
                        odatarow.BeginEdit();
                        odatarow["DEPEND"] = DependentesClt(Convert.ToString(odatarow["CODCAD"]),data2,dsDependentes.Tables["CLTDEPENDENTES"]);
                        odatarow.EndEdit();
                        odatarow.AcceptChanges();
                   // }
                    DataRow [] odatarows = dsDependentes.Tables["IRDEPENDENTES"].Select("CODCAD = '"+Convert.ToString(odatarow["CODCAD"])+"'","DATA DESC" );
                    if (odatarows.Length >0)
                    {
                        odatarow.BeginEdit();
                        odatarow["IRDEPEND"] = Convert.ToInt16(odatarows[0]["NDEPEND"]);
                        odatarow.EndEdit();
                        odatarow.AcceptChanges();
                    }
                    odatarow.BeginEdit();
                    odatarow["SALARIOREAL"] = TDataControlTrabalho.CalcSalBase(Convert.ToString(odatarow["CODCAD"]), Convert.ToDateTime(odatarow["ADMI"]),
                        Convert.ToDecimal(odatarow["SALBASE"]), salmin, data2, dsReajustes.Tables[0]);
                    odatarow["SALLIQUIDO"] =  CalcSalLiq(data2, Convert.ToDecimal(odatarow["SALARIOREAL"]),
                        Convert.ToInt16(odatarow["DEPEND"]), Convert.ToInt16(odatarow["IRDEPEND"]), dsTabelasTrabalhistas);
                  
                    informeferias oferias = UltimaFeriasAdquiridas(Convert.ToString(odatarow["CODCAD"]), Convert.ToDateTime(odatarow["ADMI"]), dsFerias.Tables[0]);
                    if (oferias != null)
                    {
                        oferias.CalculeFeriasVencidas(data2);
                        odatarow["FERIASDATA_PG"] = Convert.ToDateTime(oferias.Data_pg);
                        odatarow["AQUIS_FIM"] = Convert.ToDateTime(oferias.Aquis_fim);
                        odatarow["AQUIS_INI"] = Convert.ToDateTime(oferias.Aquis_ini);
                        odatarow["GOZO_INI"] = Convert.ToDateTime(oferias.Gozo_ini);
                        odatarow["GOZO_FIM"] = Convert.ToDateTime(oferias.Gozo_fim);
                        odatarow["FERIASVENCIDAS"] = Convert.ToInt16(oferias.Vencidas);
                        odatarow["FERIASPROP"] = Convert.ToInt16(oferias.Proporcionais);
                       /* informeferias oferias2 = UltimaFeriasGozadas(Convert.ToString(odatarow["CODCAD"]),data2, dsFerias.Tables[0]);
                        if (oferias2 == null)
                            odatarow["GOZO_INI"] = Convert.ToDateTime(oferias2.Gozo_ini);
                            odatarow["GOZO_FIM"] = Convert.ToDateTime(oferias2.Gozo_fim);
                         */   

                    }
                   


                    odatarow.EndEdit();
                    odatarow.AcceptChanges();
                    tabDeb.Rows.Add(odatarow.ItemArray);
                }
                result = new DataSet();
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            { throw;}
        }


    
        static public DataSet Get_CltDependentes(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            //DateTime nascidosdepoisde = data1.AddYears(-15);
            DateTime nascidosdepoisde = Convert.ToDateTime("01/"+data1.Month.ToString()+"/"+(data1.Year-15).ToString()); 
            DataSet result = null;
            stroledb =
                " SELECT CLTFILHO.codcad, CLTFILHO.NASC,CLTFILHO.DATA_CAD FROM " + path + "CLTFILHO," +
                " " + path + "CLTCAD " +
                " WHERE (CLTCAD.CODCAD = CLTFILHO.CODCAD) AND ( (CLTCAD.ADMI <> CTOD(" +
                TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                        "  ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" +
                        TDataControlReduzido.FormatDataGravar(data2) + ") ) )" +
                         " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() +
                         ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) " +
                      "  AND ( CLTFILHO.NASC IS NOT NULL) " + " AND (CLTFILHO.NASC >= CTOD(" + TDataControlReduzido.FormatDataGravar(nascidosdepoisde) +
                    ")) " +
                    " AND ( CLTFILHO.DATA_CAD IS NOT NULL ) ";

                     // " AND (CLTFILHO.DATA_CAD < CTOD("+ TDataControlReduzido.FormatDataGravar(data1) + ") OR CLTFILHO.DATA_CAD IS NULL )) " +
                   // " AND (CLTFILHO.NASC >= CTOD(" + TDataControlReduzido.FormatDataGravar(nascidosdepoisde) + 
                  //  " GROUP BY CLTFILHO.CODCAD ";


            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }

            stroledb += " ORDER by cltfilho.CODCAD";
            
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTDEPENDENTES");
           
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);

                stroledb =
               " SELECT IRDEPEND.codcad, IRDEPEND.NDEPEND,IRDEPEND.DATA FROM " + path + "IRDEPEND," +
               " " + path + "CLTCAD " +
               " WHERE (CLTCAD.CODCAD = IRDEPEND.CODCAD) AND ( (CLTCAD.ADMI <> CTOD(" +
               TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                       "  ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" +
                       TDataControlReduzido.FormatDataGravar(data2) + ") ) )" +
                        " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() +
                        ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ) " +
                        " AND (IRDEPEND.DATA <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + "))";

                stroledb += " ORDER by IRDEPEND.CODCAD,IRDEPEND.DATA";
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "IRDEPENDENTES");
                oledbda.Fill(odataset);

                oledbda.Dispose();

                return odataset;
            }
            catch (Exception)
            {
                throw;
            }

        }


        static public Int16 DependentesClt(string codcad, DateTime data1, DataTable dtcltfilhos)
        {
            // data1 == data do pagamento do salario familia
           // DateTime nascidosdepoisde = data1.AddYears(-14).AddDays(-1);//um dia antes de completar 14 anos...
            // regra antiga = no mes do aniv de 14 _> recebe...ainda
           // nascidosdepoisde = TDataControlReduzido.UltimoDiaMes(data1).AddYears(-14);
              // DateTime nascidosdepoisde = Convert.ToDateTime("01/" + data1.Month.ToString() + "/" + (data1.Year - 14).ToString()); 
            
            try
            {
                System.Nullable<Int32> result =
                    (from linha in dtcltfilhos.AsEnumerable()
                     where (linha.Field<string>("CODCAD") == codcad) &&
                           ( ( (linha.Field<DateTime>("NASC").Year + 14) > data1.Year)
                            || ( ((linha.Field<DateTime>("NASC").Year + 14) == data1.Year)
                                && (linha.Field<DateTime>("NASC").Month >= data1.Month))
                            )
                            && (linha.Field<DateTime>("DATA_CAD").CompareTo(data1) <= 0)
                     select linha.Field<string>("CODCAD")).Count();

                return Convert.ToInt16(result);
            }
            catch(Exception)
            { throw; }
        }





        //FOLHA E ADIANTAMENTO
        #region 
        static public DataSet Get_CltFolha(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {


            OleDbCommand oledbcomm;
            string stroledb, path ;//, strclttrab;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("TRABALHO");


            stroledb = "SELECT * FROM " + path + "CLTFOLHA"
              + " WHERE (DATA  BETWEEN  CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") AND CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") ) ";
             // 
           
       
            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }

            stroledb += " ORDER by trab";


            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTFOLHA");
               
                DataSet odataset = Get_CltCadastroFolha(data1, data2, oLista);       //new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                // TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Close();

                DataSet dsAdiant = Get_CltAdiant(data1, oLista);

                DataTable cltAdiant = dsAdiant.Tables[0];
                cltAdiant.TableName = "CLTADIAN";
                /*  DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                  PrimaryKeyColumns[0] = cltAdiant.Columns["TRAB"];
                  cltAdiant.PrimaryKey = PrimaryKeyColumns;
                  */



                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = odataset.Tables["CLTCAD"].Columns["CODCAD"];
                odataset.Tables["CLTCAD"].PrimaryKey = PrimaryKeyColumns;


                DataTable cltFolhaTot = odataset.Tables["CLTFOLHA"].Clone();
                cltFolhaTot.TableName = "CLTFOLHATOT";


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "NOME", 45, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ADIANT", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALLIQ", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SBRUTO", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HE50", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ISIND", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CONTABCO", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "AGENCIA", 8, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "BANCO", 3, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "OBSBANCO", 21, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CPF", 14, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "END", 50, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CIDADE", 25, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CEP", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "UF", 2, false));

               
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CBO", 20, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CATEGORIA", 30, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "ADMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DEMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "MENSALISTA", 1, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETORCAD", 2, false));

                result = new DataSet();
                result.Tables.Add(cltFolhaTot);
               

                PrimaryKeyColumns[0] = result.Tables["CLTFOLHATOT"].Columns["TRAB"];
                result.Tables["CLTFOLHATOT"].PrimaryKey = PrimaryKeyColumns;




                for (int i = 0; i < odataset.Tables["CLTFOLHA"].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables["CLTFOLHA"].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;
                    if (odatarow["TRAB"].ToString() == "8474")
                    {
                        passa = true;
                    }

                    DataRow totrow = cltFolhaTot.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        totrow = cltFolhaTot.NewRow();
                        totrow["TRAB"] = odatarow["TRAB"];
                       
                        DataRow ocadrow = odataset.Tables["CLTCAD"].Rows.Find(odatarow["TRAB"]);
                        if (ocadrow == null)
                            continue;
                        if (((Convert.ToDateTime(ocadrow["ADMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["ADMI"]) >= data2))
                            continue;

                        if (((Convert.ToDateTime(ocadrow["DEMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["DEMI"]) <= data2))
                            if (Convert.ToDateTime(ocadrow["DEMI"]).ToString("yyyyMM") == data2.ToString("yyyyMM"))
                                continue;

                        cltFolhaTot.Rows.Add(totrow);
                        totrow["Mensalista"] = ocadrow["Mensalista"];
                        totrow["Nome"] = ocadrow["Nomecad"];
                        totrow["SetorCad"] = ocadrow["Setor"];
                     
                        System.Nullable<decimal> totadiant =
                         (from linha in cltAdiant.AsEnumerable()
                          where (linha.Field<string>("TRAB") == Convert.ToString(totrow["TRAB"]))
                          select linha.Field<decimal>("ADIANT")).Sum();

                        totrow["ADIANT"] = Convert.ToSingle(totadiant);
                       
                        
                        totrow["CBO"] = Convert.ToString(ocadrow["CBO"]);
                        totrow["Categoria"] = Convert.ToString(ocadrow["Categoria"]);
                        totrow["ADMI"] = ocadrow["ADMI"];
                        totrow["DEMI"] = ocadrow["DEMI"];
                        totrow["CONTABCO"] = ocadrow["CONTA1"];
                        totrow["AGENCIA"] = ocadrow["AGENCIA1"];
                        totrow["BANCO"] = ocadrow["BANCO1"];
                        if (Convert.IsDBNull(ocadrow["BCOAGCC"]))
                            totrow["OBSBANCO"] = "";
                        else
                            totrow["OBSBANCO"] = ocadrow["BCOAGCC"];


                        totrow["CPF"] = ocadrow["CPF"];
                        totrow["END"] = ocadrow["END_RUA"];
                        totrow["CIDADE"] = ocadrow["END_CID"];
                        totrow["CEP"] = ocadrow["END_CEP"];
                        totrow["UF"] = ocadrow["END_UF"];
                        totrow["ISIND"] = Convert.ToSingle(0);

                       totrow["SALARIO"] = Convert.ToSingle(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToSingle(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToSingle(odatarow["VLR_HXS"]);
                        totrow["GRATIF"] = Convert.ToSingle(odatarow["GRATIF"]);
                        
                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["GRATIF"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["GRATIF"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);
                        if (odatarow["PONTO"].ToString().Trim() != "")
                            totrow["PONTO"] = odatarow["PONTO"];

                    }
                    else
                    {
                        totrow["SALARIO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(odatarow["SALARIO"]);
                        totrow["INSS"] = Convert.ToSingle(totrow["INSS"]) + Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = Convert.ToSingle(totrow["IRFONTE"]) + Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = Convert.ToSingle(totrow["SALFAM"]) + Convert.ToSingle(odatarow["SALFAM"]);
                        totrow["EDUC"] = Convert.ToSingle(totrow["EDUC"]) + Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = Convert.ToSingle(totrow["TERC"]) + Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = Convert.ToSingle(totrow["FGTS"]) + Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = Convert.ToSingle(totrow["DECIMO"]) + Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = Convert.ToSingle(totrow["FERIAS"]) + Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = Convert.ToSingle(totrow["VLR_HXA"]) + Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = Convert.ToSingle(totrow["HX"]) + Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = Convert.ToSingle(totrow["HXA"]) + Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = Convert.ToSingle(totrow["HXN"]) + Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(odatarow["VLR_HXS"]);
                        totrow["GRATIF"] = Convert.ToSingle(totrow["GRATIF"]) + +Convert.ToSingle(odatarow["GRATIF"]);

                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["GRATIF"])+Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["GRATIF"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);
                        if (odatarow["PONTO"].ToString().Trim() != "")
                            totrow["PONTO"] = odatarow["PONTO"];

                    }

                }


                /*  
                 * // funcoes agregadas
                 * CompleteString(Canvas,floattostrf(ptRound((HX/8),2),ffnumber,8,2),ColWidths[DIAS_A],false);

          Cells[SALARIO_A,linhas] :=
             CompleteString(Canvas,floattostrf(SALARIO,ffnumber,10,2),ColWidths[SALARIO_A],false);

          Cells[HE50_A,linhas] :=
          CompleteString(Canvas,floattostrf(VLR_HXA+VLR_HXS,ffnumber,9,2),ColWidths[HE50_A],false);
          Cells[HE100_A,linhas] :=
          CompleteString(Canvas,floattostrf(VLR_HXN,ffnumber,9,2),ColWidths[HE100_A],false);


          Cells[SBRUTO_A,linhas] :=
          CompleteString(Canvas,floattostrf((SALARIO + VLR_HXA + VLR_HXN+ VLR_HXS),ffnumber,10,2),ColWidths[SBRUTO_A],false);


          Cells[INSS_A,linhas] :=
          CompleteString(Canvas,floattostrf(INSS,ffnumber,9,2),ColWidths[INSS_A],false);
          Cells[SALFAM_A,linhas] :=
          CompleteString(Canvas,floattostrf(SALFAM,ffnumber,9,2),ColWidths[SALFAM_A],false);
          Cells[ADIANT_A,linhas] :=
          CompleteString(Canvas,floattostrf(ADIANT,ffnumber,9,2),ColWidths[ADIANT_A],false);

          Cells[SALLIQ_A,linhas] :=
          CompleteString(Canvas,floattostrf((SALARIO + VLR_HXA + VLR_HXN +  VLR_HXS + SALFAM - INSS - ADIANT),
                           ffnumber,10,2),ColWidths[SALLIQ_A],false);

          Cells[FGTS_A,linhas] :=
            CompleteString(Canvas,floattostrf(FGTS,ffnumber,9,2),ColWidths[FGTS_A],false);

          Cells[EDUC_A,linhas] :=
            CompleteString(Canvas,floattostrf(EDUC,ffnumber,9,2),ColWidths[EDUC_A],false);

          Cells[TERC_A,linhas] :=
            CompleteString(Canvas,floattostrf(TERC,ffnumber,9,2),ColWidths[TERC_A],false);

          Cells[PROV_A,linhas] :=
            CompleteString(Canvas,floattostrf((DECIMO + FERIAS),ffnumber,9,2),ColWidths[PROV_A],false);

          Cells[CUSTOTOT_A,linhas] :=
            CompleteString(Canvas,floattostrf((SALARIO + VLR_HXA + VLR_HXN + VLR_HXS+ FGTS + DECIMO + FERIAS + EDUC + TERC)
               ,ffnumber,10,2),ColWidths[CUSTOTOT_A],false);
          oCheque := TRegistroFin.Create;
          ocheque.data := datetostr(dtCheque.Date);
          oCheque.forn := NOME;
          oCheque.valor := (SALARIO + VLR_HXA + VLR_HXN + VLR_HXS+ SALFAM - INSS - ADIANT);
          oCheque.ctafinan := CONTABANCO;
          oCheque.ListBMark := TList.Create;
          oCheque.ListBMark.Add(oReg);

          oBjects[1,linhas] := oCheque;
        end;*/

                // DataTable cltFolhaTrab = odataset.Tables["CLTFOLHATOT"].Clone();
                // cltFolhaTrab.TableName = "CLTFOLHATRAB";



                result.Tables["CLTFOLHATOT"].AcceptChanges();
                for (int i = 0; i < result.Tables["CLTFOLHATOT"].Rows.Count; i++)
                {
                    if (Convert.ToSingle(result.Tables["CLTFOLHATOT"].Rows[i]["SALLIQ"]) < 0.00)
                    {
                        result.Tables["CLTFOLHATOT"].Rows[i].Delete();
                    }

                }
                result.Tables["CLTFOLHATOT"].AcceptChanges();
                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }

        
      ////

        static public DataSet Adiantamento(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {


            string  path;//, strclttrab;
            DataSet result = null;
            //path = TDataControlReduzido.Get_Path("TRABALHO");

            


            try
            {

                DataSet dsAdiant = Get_CltAdiant(data1, oLista);
                DataSet odataset = Get_CltCadastroFolha(data1, data2, oLista);       //new DataSet();
                
              
                // é a tabela de adiantamento..
                DataTable cltFolhaTot = dsAdiant.Tables[0].Clone();
                cltFolhaTot.TableName = "CLTFOLHATOT";
               


                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = odataset.Tables["CLTCAD"].Columns["CODCAD"];
                odataset.Tables["CLTCAD"].PrimaryKey = PrimaryKeyColumns;

                //diferenças (folha e Adiant)
                //cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "ADIANT", 0, false));//tirei
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALARIO", 0, false));//acrescentei
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "IRFONTE", 0, false));//acrescentei
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "EDUC", 0, false));//acrescentei
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "TERC", 0, false));//acrescentei
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "GRATIF", 0, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "DECIMO", 0, false));//acrescentei
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "FERIAS", 0, false));//acrescentei


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "NOME", 45, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SALLIQ", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "SBRUTO", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.Decimal"), "HE50", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CPF", 14, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "END", 50, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CIDADE", 25, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CEP", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "UF", 2, false));

                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CONTABCO", 10, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "AGENCIA", 8, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "BANCO", 3, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "OBSBANCO", 21, false));


                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CBO", 20, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "CATEGORIA", 30, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "ADMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.DateTime"), "DEMI", 0, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "MENSALISTA", 1, false));
                cltFolhaTot.Columns.Add(TDataControlReduzido.Coluna(Type.GetType("System.String"), "SETORCAD", 2, false));

                result = new DataSet();
                result.Tables.Add(cltFolhaTot);


                PrimaryKeyColumns[0] = result.Tables["CLTFOLHATOT"].Columns["TRAB"];
                result.Tables["CLTFOLHATOT"].PrimaryKey = PrimaryKeyColumns;




                for (int i = 0; i < dsAdiant.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = dsAdiant.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    DataRow totrow = cltFolhaTot.Rows.Find(odatarow["TRAB"]);
                    if (totrow == null)
                    {
                        totrow = cltFolhaTot.NewRow();
                        totrow["TRAB"] = odatarow["TRAB"];

                        DataRow ocadrow = odataset.Tables["CLTCAD"].Rows.Find(odatarow["TRAB"]);
                        if (ocadrow == null)
                            continue;
                        if (((Convert.ToDateTime(ocadrow["ADMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["ADMI"]) >= data2))
                            continue;

                        if (((Convert.ToDateTime(ocadrow["DEMI"]) > Convert.ToDateTime("01/01/1900"))) && (Convert.ToDateTime(ocadrow["DEMI"]) <= data2))
                            if (Convert.ToDateTime(ocadrow["DEMI"]).ToString("yyyyMM") == data2.ToString("yyyyMM"))
                                continue;

                        cltFolhaTot.Rows.Add(totrow);
                        totrow["Mensalista"] = ocadrow["Mensalista"];
                        totrow["Nome"] = ocadrow["Nomecad"];
                        totrow["SetorCad"] = ocadrow["Setor"];
                       
                   
                        totrow["CBO"] = Convert.ToString(ocadrow["CBO"]);
                        totrow["Categoria"] = Convert.ToString(ocadrow["Categoria"]);
                        totrow["ADMI"] = ocadrow["ADMI"];
                        totrow["DEMI"] = ocadrow["DEMI"];
                        totrow["CONTABCO"] = ocadrow["CONTA1"];
                        totrow["AGENCIA"] = ocadrow["AGENCIA1"];
                        totrow["BANCO"] = ocadrow["BANCO1"];
                        if (Convert.IsDBNull(ocadrow["BCOAGCC"]))
                            totrow["OBSBANCO"] = "";
                        else
                        totrow["OBSBANCO"] = ocadrow["BCOAGCC"];

                        
                        totrow["CPF"] = ocadrow["CPF"];
                        totrow["END"] = ocadrow["END_RUA"];
                        totrow["CIDADE"] = ocadrow["END_CID"];
                        totrow["CEP"] = ocadrow["END_CEP"];
                        totrow["UF"] = ocadrow["END_UF"];

                        /*Data := oAtualFOlha.fim;
                        if dmTrab.tblCLTCAD.FieldbyName('ULT_ATUAL').ISNULL then
                          tultAtual :=  dmTrab.tblCLTCAD.FieldbyName('ADMI').asDateTime
                        else
                          tultAtual := dmTrab.tblCLTCAD.FieldbyName('ULT_ATUAL').asDateTime;

                          Salbase :=    ptRound((Descubra_Salbase(dmTrab.tblCLTCAD.FieldbyName('CODCAD').asString,
                                        dmTrab.tblCLTCAD.fieldbyName('SALBASE').asFloat,tultatual
                                ,oAtualFOlha.fim)  * oAtualFOlha.salmin),2);
                                   if cbImposto.Checked then
                           iSind := ptRound(Salbase/30,2);
                        for i := 1 to 16 do
                        begin
                           ponto1[i] := '  ';
                           ponto2[i] := '  ';
                        end;
                        mensalista := chekMensalista.Checked;
                                   */



                        totrow["SALARIO"] = Convert.ToSingle(odatarow["ADIANT"]);
                        totrow["ADIANT"] = 0.00;
                        totrow["GRATIF"] = 0.00;
                         totrow["INSS"] = 0.00;//Convert.ToSingle(odatarow["INSS"]);
                        totrow["IRFONTE"] = 0.00; //Convert.ToSingle(odatarow["IRFONTE"]);
                        totrow["SALFAM"] = 0.00;//Convert.ToSingle(odatarow["SALFAM"]);
                        totrow["EDUC"] = 0.00;//Convert.ToSingle(odatarow["EDUC"]);
                        totrow["TERC"] = 0.00;//Convert.ToSingle(odatarow["TERC"]);
                        totrow["FGTS"] = 0.00;//Convert.ToSingle(odatarow["FGTS"]);

                        totrow["DECIMO"] = 0.00;//Convert.ToSingle(odatarow["DECIMO"]) + Convert.ToSingle(odatarow["FGTS_DEC"]);
                        totrow["FERIAS"] = 0.00;//Convert.ToSingle(odatarow["FERIAS"]) + Convert.ToSingle(odatarow["FGTS_FE"]);
                        totrow["VLR_HXA"] = 0.00;//Convert.ToSingle(odatarow["VLR_HXA"]);
                        totrow["HX"] = 0.00;//Convert.ToSingle(odatarow["HX"]);
                        totrow["VLR_HXN"] = 0.00;//Convert.ToSingle(odatarow["VLR_HXN"]);
                        totrow["HXA"] = 0.00;//Convert.ToSingle(odatarow["HXA"]);
                        totrow["HXN"] = 0.00;//Convert.ToSingle(odatarow["HXN"]);
                        totrow["VLR_HXS"] = 0.00;//Convert.ToSingle(odatarow["VLR_HXS"]);*/
                        totrow["SALLIQ"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["GRATIF"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["SALFAM"]) - Convert.ToSingle(totrow["INSS"]) - Convert.ToSingle(totrow["ADIANT"]) - Convert.ToSingle(totrow["IRFONTE"]);
                        totrow["SBRUTO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(totrow["GRATIF"]) + Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXN"]) + Convert.ToSingle(totrow["VLR_HXS"]);
                        totrow["HE50"] = Convert.ToSingle(totrow["VLR_HXA"])
                            + Convert.ToSingle(totrow["VLR_HXS"]) + Convert.ToSingle(totrow["VLR_HXN"]);
                        
                    }
                    else
                    {
                        totrow["SALARIO"] = Convert.ToSingle(totrow["SALARIO"]) + Convert.ToSingle(odatarow["ADIANT"]);
                      
                    }

                    //cltFolhaTot.Rows.Add(odatarow.ItemArray);

                }


                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion


        

          
     public class informeferias
    {
        private DateTime gozo_ini,gozo_fim;
        private DateTime aquis_ini, aquis_fim;
        private DateTime data_pg;
        private DateTime admi,demi; 
        Int16 vencidas;
        Int16 proporcionais;
        public Decimal salcontrib;
        public Decimal inss;
        public Decimal salfam;

        public informeferias(DateTime oadmi)
        {
            admi = oadmi;
            vencidas = 0;
            proporcionais = 0;
            aquis_fim = new DateTime();
            salcontrib = 0;
            inss = 0;
            salfam = 0;
        }
        public DateTime Gozo_ini
        {
            get { return gozo_ini; }
            set { gozo_ini = value; }
        }
        public DateTime Gozo_fim
        {
            get { return gozo_fim; }
            set { gozo_fim = value; }
        }
        public DateTime Aquis_ini
        {
            get { return aquis_ini; }
            set { aquis_ini = value; }
        }
        public DateTime Aquis_fim
        {
            get { return aquis_fim; }
            set { aquis_fim = value; }
        }
        public DateTime Data_pg
        {
            get { return data_pg; }
            set { data_pg = value; }
        }
        public DateTime Admi
        {
            get { return admi; }
            set { admi = value; }
        }
        public DateTime Demi
        {
            get { return demi; }
            set { demi = value; }
        }
        public Int16 Vencidas
        {
            get { return vencidas; }
            set { vencidas = value; }
        }
        public Int16 Proporcionais
        {
            get { return proporcionais; }
            set { proporcionais = value; }
        }
        public void CalculeFeriasVencidas(DateTime odata)
        {
            DateTime novadata, novofimaquis;
            if (admi == DateTime.MinValue) 
                return;

            DateTime inicioaquisicao = admi;

            if (aquis_fim == DateTime.MinValue)
                novadata = admi.AddDays(-1);
            else
                novadata = aquis_fim;
            Int32 contferias = 0;
            // defina a data de vencimento da férias adquirida (mais antiga) e não paga

           /* novofimaquis = novadata; 
            novadata = novadata.AddDays(1).AddYears(1).AddDays(-1);    //:= UmAnoaMais(novadata+1)-1;
            
           // para estabelecer o direito de ferias de recem admitidos preciso saber se o admitido tem + de 1 ano de admissão
              DateTime inicioano = odata.AddYears(-1);
           
              if ((novadata.CompareTo(odata) > 0) && (admi.CompareTo(inicioano)<0 ))
              {
                 novadata = novofimaquis;
              }
     

            Int16 contferias = 0;
            
            while (novadata.CompareTo(odata) < 0)
            {
                if (novadata.AddYears(1).CompareTo(odata) <= 0)
                     contferias += 1;
                inicioaquisicao = novadata.AddDays(1);
                novadata = inicioaquisicao.AddYears(1).AddDays(-1);

            }
            */
            inicioaquisicao = novadata.AddDays(1);
            Int32 nper = 0;
            //if ((novadata.CompareTo(odata) > 0))
            {
                Int16 meiomes = 15;
                if (odata.Month == 2) meiomes = 14;
                //deverá contar numero de dias trabalhos no aviso
                nper = (odata.Year - inicioaquisicao.Year) * 12;
                nper = nper + (odata.Month - inicioaquisicao.Month);
                if (odata.Day < inicioaquisicao.Day)
                {
                    if ((odata.Day - inicioaquisicao.Day) - 1 < (meiomes * -1))
                        nper = nper - 1;
                }
                else
                {
                    if ((odata.Day - inicioaquisicao.Day + 1) >= meiomes)
                    { 
                        //no rescisao.. 
                        /*if trunc(objresc.dias/8) >= MeioMes then
                               nper := nper + 1;
                         */
                        nper += 1;
                    }
                  }
            }
            // = Convert.ToInt32(Math.Abs((nper / 12)));
            contferias = Math.DivRem(nper, 12, out nper);
            Vencidas = Convert.ToInt16(contferias);
            Proporcionais = Convert.ToInt16(nper);


        }

    }

     static public informeferias UltimaFeriasAdquiridas(string tcod,DateTime admi, DataTable tabferias)
     {
         informeferias oferias = new informeferias(admi); 
         DataRow[] orows = tabferias.Select("COD = '" + tcod + "'", "AQUIS_FIM DESC");
         if (orows.Length == 0) 
             return oferias;
         oferias.Aquis_fim = Convert.ToDateTime(orows[0]["AQUIS_FIM"]);
         oferias.Aquis_ini = Convert.ToDateTime(orows[0]["AQUIS_INI"]);
         oferias.Gozo_fim = Convert.ToDateTime(orows[0]["GOZO_FIM"]);
         oferias.Gozo_ini = Convert.ToDateTime(orows[0]["GOZO_INI"]);
         oferias.Data_pg = Convert.ToDateTime(orows[0]["DATA"]);
         oferias.salcontrib = Convert.ToDecimal(orows[0]["VLR"]) + Convert.ToDecimal(orows[0]["TERCVLR"]);
         oferias.inss = Convert.ToDecimal(orows[0]["INSS"]);
         oferias.salfam = Convert.ToDecimal(orows[0]["SALFAM"]);
         return oferias;
     }
     static public informeferias UltimaFeriasGozadas(string tcod, DateTime data, DataTable tabferias)
     {
         CultureInfo ci;
         ci = new CultureInfo("en-US");
         
         informeferias oferias = new informeferias(data);
         DataRow[] orows = tabferias.Select("(COD = '" + tcod + "')" + " AND " + " (GOZO_FIM <= #" + data.Date.ToString("d",ci)+"#)", "GOZO_INI DESC");
         if (orows.Length == 0)
             return oferias;
         oferias.Admi = Convert.ToDateTime(orows[0]["ADMI"]);
         oferias.Aquis_fim = Convert.ToDateTime(orows[0]["AQUIS_FIM"]);
         oferias.Aquis_ini = Convert.ToDateTime(orows[0]["AQUIS_INI"]);
         oferias.Gozo_fim = Convert.ToDateTime(orows[0]["GOZO_FIM"]);
         oferias.Gozo_ini = Convert.ToDateTime(orows[0]["GOZO_INI"]);
         oferias.Data_pg = Convert.ToDateTime(orows[0]["DATA"]);
         oferias.salcontrib = Convert.ToDecimal(orows[0]["VLR"]) + Convert.ToDecimal(orows[0]["TERCVLR"]);
         oferias.inss = Convert.ToDecimal(orows[0]["INSS"]);
         oferias.salfam = Convert.ToDecimal(orows[0]["SALFAM"]);
         return oferias;
     }


     static public DataSet Get_CltFerias(DateTime data1, DateTime data2, List<LinhaSolucao> oLista, string criterio_admi_demi)
     {
         OleDbCommand oledbcomm;
         string stroledb, path;
         path = TDataControlReduzido.Get_Path("TRABALHO");
         if (criterio_admi_demi == "")
         {
             criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" +
                 TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                    "  ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" +
                    TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                     " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() +
                     ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) ) ";
         }
         stroledb =
             //  tipo,
    " SELECT  ferias.data,  ferias.cod, ferias.aquis_ini, ferias.aquis_fim, ferias.gozo_ini, ferias.gozo_fim, " +
      "ferias.salbase, ferias.media, ferias.diasabono, ferias.vlr, ferias.tercvlr, ferias.vlrabono, ferias.inss," +
      " ferias.salfam, ferias.educ, ferias.terc, ferias.fgts, ferias.irf, ferias.faltas, ferias.diasfe," +
               "ferias.salcont, ferias.vlrliq ,cltcad.admi, cltcad.demi" +
         " FROM " + path + "FERIAS, " + path + "CLTCAD " +
         "  WHERE  (FERIAS.COD = cltcad.codcad ) AND " +
          criterio_admi_demi;
         if (oLista != null)
         {
             for (int i = 0; i < oLista.Count; i++)
             {
                 if (oLista[i].ofuncao == null)
                 {
                     if (oLista[i].ofuncaoSql != null)
                     {
                         stroledb += oLista[i].ofuncaoSql(oLista[i]);
                     }
                     else
                         stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                 }
             }
         }
         stroledb += " ORDER by FERIAS.COD,FERIAS.DATA";
         try
         {
             oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
             OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
             oledbda.TableMappings.Add("Table", "FERIAS");
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



        static public DataSet TabelasTrabalhistas()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            //, descricao, sdo, ant_sdo, data, data_ant, novocod, novadesc 
            setoroledb = "SELECT  data, valor, fgts FROM  "
                + path + "TABSAL WHERE        (valor <> 0) " +
                       " ORDER BY DATA DESC";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "TABSAL");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);

         //   DataTable result = odataset.Tables[0];
            
           /// DataColumn[] PrimaryKeyColumns = new DataColumn[1];
           // PrimaryKeyColumns[0] = result.Columns["DATA"];
           // result.PrimaryKey = PrimaryKeyColumns;

            setoroledb = "SELECT        data, faixa, valor1, valor2, taxa FROM "+      
                path + "TABINSS  " +
                       " ORDER BY DATA DESC,faixa";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "TABINSS");
          
            oledbda.Fill(odataset);

            setoroledb = "SELECT        data, faixa, base1, base2, aliq, deduzir FROM " +
                path + "TABIR  " +
                       " ORDER BY DATA DESC,faixa";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "TABIR");

            oledbda.Fill(odataset);


            setoroledb = "SELECT data, deducao FROM " +
                path + "IRDEDU  " +
                       " ORDER BY DATA DESC";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "IRDEDU");

            oledbda.Fill(odataset);


            setoroledb = "SELECT        data, faixa, valor1, valor2, salfam FROM " +
                  path + "SALFAM  " +
                         " ORDER BY DATA DESC,faixa";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "SALFAM");

            oledbda.Fill(odataset);

            // primary
            return odataset;
        }

        static public DataSet Get_CltReajuste(DateTime data1, DateTime data2, List<LinhaSolucao> oLista,string criterio_admi_demi)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            if (criterio_admi_demi == "")
            {
                criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" +
                    TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                       "  ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" +
                       TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                        " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() +
                        ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) ) ";
            }
            stroledb = " SELECT cltreaj.data, cltreaj.trab, cltreaj.reaj,  cltcad.glecad, cltcad.setor, cltcad.admi, cltcad.demi, cltcad.prazo " +
            "FROM " + path + "CLTREAJ, " + path + "CLTCAD " +
            "  WHERE  (cltreaj.trab = cltcad.codcad ) AND " +
             criterio_admi_demi;   
            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                   {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            stroledb += " ORDER by CLTREAJ.TRAB,CLTREAJ.DATA";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTREAJUSTE");
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


       


       
        static public decimal CalcSalarioMinimo(DateTime data, DataTable SalarioMinimo)
        {
            Decimal result = 0.0M;
            
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow [] orow = SalarioMinimo.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if ((orow.Length > 0) && (Convert.ToDecimal(orow[0]["VALOR"]) != 0.0M))
            {
                result = Convert.ToDecimal(orow[0]["VALOR"]);
                return result;
            }
            else
            {
                MessageBox.Show("Falta Salario Minimo Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }

        }
        static public Decimal CalcFgts(DateTime data, DataTable SalarioMinimo)
        {
            Decimal result = 0.0M;
            
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow [] orow = SalarioMinimo.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if ((orow.Length > 0) && (Convert.ToDecimal(orow[0]["VALOR"]) != 0.0M))
            {
                result = Convert.ToDecimal(orow[0]["FGTS"]);
                return result;
            }
            else
            {
                MessageBox.Show("Falta FGTS Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }

        }


    



        static public Decimal CalcSalLiq(DateTime data, Decimal salbasebruto, int ndep, int irdep, DataSet dsTrabalhista)
        {
           
           Decimal inss = CalcInss(data,salbasebruto, dsTrabalhista.Tables["TABINSS"]);
            Decimal salfam = CalcSalFam(data, salbasebruto, ndep, dsTrabalhista.Tables["SALFAM"]);
            Decimal irfonte = CalcIRF(data, salbasebruto,inss, irdep, dsTrabalhista.Tables["TABIR"], dsTrabalhista.Tables["IRDEDU"]);
            Decimal result = salbasebruto - inss + salfam - irfonte;
            return result;
        }
        static public Decimal CalcInss(DateTime data, Decimal salbase, DataTable TabelaInss)
        {
            Decimal result = 0;
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow[] orow = TabelaInss.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if (orow.Length > 0)
            {
                Decimal valormax = 0;
                Decimal taxamax = 0;
                orow = TabelaInss.Select("DATA = '" + Convert.ToDateTime(orow[0]["DATA"]).ToString("d") + "'", "FAIXA");
                foreach (DataRow olinha in orow)
                {
                    if ((Convert.ToDecimal(olinha["VALOR1"]) <= salbase) && (Convert.ToDecimal(olinha["VALOR2"]) >= salbase))
                        result = ((Convert.ToDecimal(olinha["TAXA"]) / 100) * salbase);
                    else
                    {
                        valormax = Convert.ToDecimal(olinha["VALOR2"]);
                        taxamax = Convert.ToDecimal(olinha["TAXA"]);
                    }
                }
                if ((result == 0.0M) && (salbase > valormax))
                {
                    result = ((taxamax / 100) * valormax);
                }
                  if ((result != 0.0M) && ( data.ToString("yyyyMM").CompareTo("200203") >= 0)) 
                      result = ptTruncRound(result,2);
  
                return result;
            }
            else
            {
                MessageBox.Show("Falta INSS Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }
          
        
        }
        static public Decimal TaxaInss(DateTime data, Decimal salbase, DataTable TabelaInss)
        {
            Decimal result = 0;
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow[] orow = TabelaInss.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if (orow.Length > 0)
            {
                Decimal valormax = 0;
                Decimal taxamax = 0;
                orow = TabelaInss.Select("DATA = '" + Convert.ToDateTime(orow[0]["DATA"]).ToString("d") + "'", "FAIXA");
                foreach (DataRow olinha in orow)
                {
                    if ((Convert.ToDecimal(olinha["VALOR1"]) <= salbase) && (Convert.ToDecimal(olinha["VALOR2"]) >= salbase))
                        result = Convert.ToDecimal(olinha["TAXA"]);
                    else
                    {
                        valormax = Convert.ToDecimal(olinha["VALOR2"]);
                        taxamax = Convert.ToDecimal(olinha["TAXA"]);
                    }
                }
                if ((result == 0.0M) && (salbase > valormax))
                {
                    result = taxamax;
                }
                if ((result != 0.0M) && (data.ToString("yyyyMM").CompareTo("200203") >= 0))
                    result = ptTruncRound(result, 2);

                return result;
            }
            else
            {
                MessageBox.Show("Falta INSS Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }
        }



        static public Decimal CalcSalFam(DateTime data, Decimal salbase,Int32 ndep, DataTable TabelaSalFam)
        {
            Decimal result = 0.0M;
            if (ndep == 0)      return result;
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow[] orow = TabelaSalFam.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if (orow.Length > 0)
            {
                orow = TabelaSalFam.Select("DATA = '" + Convert.ToDateTime(orow[0]["DATA"]).ToString("d") + "'", "FAIXA");
                foreach (DataRow olinha in orow)
                {
                    if ((Convert.ToDecimal(olinha["VALOR1"]) <= salbase) && (Convert.ToDecimal(olinha["VALOR2"]) >= salbase))
                    {  
                        result = (Convert.ToDecimal(olinha["SALFAM"]) * ndep);
                        break;
                    }
                }
              
                if ((result != 0.0M) && (data.ToString("yyyyMM").CompareTo("200203") >= 0))
                    result = ptTruncRound(result, 2);

                return result;
            }
            else
            {
                MessageBox.Show("Falta SalFam Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }

        }

/*        function CalcIRF(tcod:string;tdata:TDateTime;tvalor,tinss:Decimal):Decimal;
var
 tdataatual : TDateTime;
 tactive,pesquise_depend : boolean;
 irded : Decimal;
begin
 result := 0;
 tvalor := tvalor - tinss;
 pesquise_depend := false;
 IRded := 0;
 with dmTRab.tblTabIr do
 begin
  tactive := active;
  if active = false then active :=true;
  IndexDefs.Update;
  IndexName := 'ITABIR1';
  SetKey;
  FieldbyName('DATA').asDateTime := tdata;
  GotoNearest;
  if (bof) and (formatDateTime('yyyymm',tdata) < formatDateTime('yyyymm',FieldbyName('DATA').asDateTime)) then
  begin
     exit;
  end;

  while (not bof) and (formatDateTime('yyyymm',tdata) < formatDateTime('yyyymm',FieldbyName('DATA').asDateTime) ) do
    Prior;
  // encontrada a data de menor nivel possiciona no registro 1. dessa data
  tdataatual := FieldbyName('DATA').asDateTime;
  while ((not bof) and (formatDateTime('yyyymm',tdataatual) = formatDateTime('yyyymm',FieldbyName('DATA').asDateTime) )) do
    Prior;
  if (not bof) then
    Next;
  while ((not eof) and (result = 0) and (formatDateTime('yyyymm',tdata) >= formatDateTime('yyyymm',FieldbyName('DATA').asDateTime) ))
  do begin
     if (fieldbyName('BASE1').asFLoat <= tvalor) and (fieldbyName('BASE2').asFLoat >= tvalor) and
         (fieldbyName('ALIQ').asFLoat = 0) then
     begin
         result := -1;
         continue;
     end;
     if not pesquise_depend then
     begin
        irded := PesqDeduzIR(tcod,tdata);
        tvalor := tvalor - irded;
        pesquise_depend := true;
     end;
     if (fieldbyName('BASE1').asFLoat <= tvalor) and (fieldbyName('BASE2').asFLoat >= tvalor) then
     begin
         result :=  ((fieldbyName('ALIQ').asFLoat/100) *tvalor) - fieldbyName('DEDUZIR').asFLoat;
     end;

     next;
  end;
  if result =  -1 then result := 0;
  active := tactive;
  if (result <> 0) and (formatDateTime('yyyymm',tdata) > '200203') then
  begin
    result := ptTruncRound(result,2);
  end;
 end;

end;
        */
        static public Decimal CalcIRF(DateTime data,Decimal salbase, Decimal inss, int ndepir,DataTable TabelaIrf, DataTable TabelaIrDedu)
        {
            Decimal result = 0.0M;
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow[] orow = TabelaIrDedu.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            Decimal valordedu_depend = 0.0M;
            if (orow.Length > 0)
            {
                valordedu_depend = Convert.ToDecimal(orow[0]["Deducao"]);
            }

             orow = TabelaIrf.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if (orow.Length > 0)
            {
                orow = TabelaIrf.Select("DATA = '" + Convert.ToDateTime(orow[0]["DATA"]).ToString("d") + "'", "FAIXA");
                salbase = ((salbase -inss) - (ndepir * valordedu_depend));
                foreach (DataRow olinha in orow)
                {
                    if ((Convert.ToDecimal(olinha["BASE1"]) <= salbase) && (Convert.ToDecimal(olinha["BASE2"]) >= salbase)
                         && (Convert.ToDecimal(olinha["ALIQ"]) != 0.0M) )
                    {
                        
                        result = ((Convert.ToDecimal(olinha["ALIQ"]) / 100.0M) * salbase) - Convert.ToDecimal(olinha["DEDUZIR"]);
                        break;
                    }
                }
            
                if ((result != 0.0M) && (data.ToString("yyyyMM").CompareTo("200203") >= 0))
                    result = ptTruncRound(result, 2);

                return result;
            }
            else
            {
                MessageBox.Show("Falta IR Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }
        }
        static public Decimal DeducaoIRF(DateTime data, int ndepir, DataTable TabelaIrf, DataTable TabelaIrDedu)
        {
            Decimal result = 0.0M;
            DateTime ofimdomes = TDataControlReduzido.UltimoDiaMes(data);
            DataRow[] orow = TabelaIrDedu.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            Decimal valordedu_depend = 0.0M;
            if (orow.Length > 0)
            {
                valordedu_depend = Convert.ToDecimal(orow[0]["Deducao"]);
            }

            orow = TabelaIrf.Select("DATA <= '" + ofimdomes.ToString("d") + "'", "DATA DESC");
            if (orow.Length > 0)
            {
                orow = TabelaIrf.Select("DATA = '" + Convert.ToDateTime(orow[0]["DATA"]).ToString("d") + "'", "FAIXA");
                result = (ndepir * valordedu_depend);
                
                return result;
            }
            else
            {
                MessageBox.Show("Falta IR Data:" + ofimdomes.ToString("d"));
                return 0.00M;
            }
        }

       
        static public DataSet Get_CltCadastroReajustes(DateTime data1, DateTime data2, List<LinhaSolucao> oLista, int anos,CltRelReajuste ocltreaj,Decimal taxa)
        {
            OleDbCommand oledbcomm;
            string stroledb, path;
            path = TDataControlReduzido.Get_Path("TRABALHO");
            DataSet result = null;
            if (ocltreaj == CltRelReajuste.MensalistasNaDataBase)
                anos = 2;
            string criterio_admi_demi = "( (CLTCAD.ADMI <> CTOD(" +
                   TDataControlReduzido.DataVazia() + ")) AND (CLTCAD.ADMI <= CTOD(" + TDataControlReduzido.FormatDataGravar(data2) + ") )) AND " +
                      "  ((CLTCAD.DEMI = CTOD(" + TDataControlReduzido.DataVazia() + ")) OR  (CLTCAD.DEMI >= CTOD(" +
                      TDataControlReduzido.FormatDataGravar(data1) + ") ) )" +
                       " AND ( (CLTCAD.PRAZO = CTOD(" + TDataControlReduzido.DataVazia() +
                       ")) OR  (CLTCAD.PRAZO >= CTOD(" + TDataControlReduzido.FormatDataGravar(data1) + ") ) ) ";

           
            // SEXO, NASC, COD_ADMI, CARTTRAB, SERIE, CARTIDENT,
            //ULT_ATUAL, CBO, CATEGORIA, VLRFGTS, DTAFGTS, MAE, PAI, CONJUGE,
            stroledb = " SELECT CODCAD, NOMECAD, GLECAD, SETOR, ADMI, DEMI, PRAZO, SALBASE, OPCAO, AVULSO, DEPEND, IRDEPEND, INSCPIS, CTA_FGTS," +
                         "  MENSALISTA, TIPODEMI, AVISO, SALRESC, BANCO1, CONTA1, AGENCIA1, BCOAGCC, BANCO_OK, CPF, NUMERO " +
                        // " EMICID, EMICTRAB, EMIPIS, ESTCIVIL, COR, NCIDADE, NUF, TITELEITOR, RESERV, RESERV_CAT, TPSANGUE, END_RUA, END_CID, END_UF, END_CEP, DEFIC, TPDEFIC, APRENDIZ, " +
                        // " TPMOV, DTAAVISO " +
                       "FROM " + path + "CLTCAD  WHERE " + criterio_admi_demi;

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            stroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            stroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            stroledb += " ORDER by CLTCAD.codcad";
            try
            {
                oledbcomm = new OleDbCommand(stroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "CLTCAD");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                DataSet dsTabelasTrabalhistas = TabelasTrabalhistas();
                DataSet dsReajustes = Get_CltReajuste(data1, data2, null, criterio_admi_demi);
                
                DataSet[] dsDependentes = new DataSet[anos];
                DateTime[] Datas1 = new DateTime[anos];
                DateTime[] Datas2 = new DateTime[anos];
                Int16[] Deps = new Int16[anos];
                Int16[] IrDeps = new Int16[anos];
                Decimal[] SalarioMinimos = new Decimal[anos];
                //Decimal[]  = new Decimal[anos];

                if (ocltreaj == CltRelReajuste.MensalistasNaDataBase)
                {
                    for (int i = 0; i < anos; i++)
                    {
                        Datas1[i] = data1.AddMonths((-1 * i));
                        Datas2[i] = data2.AddMonths((-1 * i));
                        SalarioMinimos[i] = CalcSalarioMinimo(data2.AddYears((-1 * i)), dsTabelasTrabalhistas.Tables["TABSAL"]);
                    }

                    for (int i = 0; i < anos; i++)
                    {
                        dsDependentes[i] = TDataControlTrabalho.Get_CltDependentes(Datas1[i], Datas2[i], null);
                        //dsDependentes[i].Tables["CLTDEPENDENTES"].PrimaryKey = new DataColumn[1] { dsDependentes[i].Tables["CLTDEPENDENTES"].Columns["CODCAD"] };
                    }

                    for (int i = 0; i < anos; i++)
                    {
                        odataset.Tables[0].Columns.Add("SALBRUTO" + Datas2[i].Month.ToString(), Type.GetType("System.Decimal"));
                        odataset.Tables[0].Columns.Add("SALLIQ" + Datas2[i].Month.ToString(), Type.GetType("System.Decimal"));
                    }
                    odataset.Tables[0].Columns.Add("SALBRUTOTAXA", Type.GetType("System.Decimal"));
                    odataset.Tables[0].Columns.Add("SALLIQTAXA" , Type.GetType("System.Decimal"));
                    odataset.Tables[0].Columns.Add("DIFSALARIOTAXA", Type.GetType("System.Decimal"));
                }
                else
                {
                    for (int i = 0; i < anos; i++)
                    {
                        Datas1[i] = data1.AddYears((-1 * i));
                        Datas2[i] = data2.AddYears((-1 * i));
                        SalarioMinimos[i] = CalcSalarioMinimo(Datas2[i], dsTabelasTrabalhistas.Tables["TABSAL"]);
                    }

                    for (int i = 0; i < anos; i++)
                    {
                        dsDependentes[i] = TDataControlTrabalho.Get_CltDependentes(Datas1[i], Datas2[i], null);
                        //dsDependentes[i].Tables["CLTDEPENDENTES"].PrimaryKey = new DataColumn[1] { dsDependentes[i].Tables["CLTDEPENDENTES"].Columns["CODCAD"] };
                    }

                    for (int i = 0; i < anos; i++)
                    {
                        odataset.Tables[0].Columns.Add("SALBRUTO" + Datas2[i].Year.ToString(), Type.GetType("System.Decimal"));
                        odataset.Tables[0].Columns.Add("SALLIQ" + Datas2[i].Year.ToString(), Type.GetType("System.Decimal"));
                    }
                }
                if (ocltreaj == CltRelReajuste.MensalistasNaDataBase)
                {
                    odataset.Tables[0].Columns.Add("REAJUSTE" + Datas2[0].Month.ToString(), Type.GetType("System.Decimal"));
                    odataset.Tables[0].Columns.Add("DIFSALARIO", Type.GetType("System.Decimal"));
                    odataset.Tables[0].Columns.Add("REAJUSTETAXA", Type.GetType("System.Decimal"));
                    odataset.Tables[0].Columns.Add("DIFSALARIORAXA", Type.GetType("System.Decimal"));
             
                }
                DataTable tabDeb = odataset.Tables[0].Clone();

                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                                if (!passa) break;
                            }
                        }
                        catch
                        {
                            passa = false;
                        }
                    }
                    if (!passa) continue;

                    for (int j = 0; j < anos; j++)
                    {
                        //DataRow odatarow2 = dsDependentes[j].Tables["CLTDEPENDENTES"].Rows.Find(odatarow["CODCAD"]);
                        Deps[j] = 0;
                        //if (odatarow2 != null)
                        Deps[j] = DependentesClt(Convert.ToString(odatarow["CODCAD"]),
                            Datas2[j], dsDependentes[j].Tables["CLTDEPENDENTES"]); 

                        DataRow[] odatarows = dsDependentes[j].Tables["IRDEPENDENTES"].Select("CODCAD = '" + Convert.ToString(odatarow["CODCAD"]) + "'", "DATA DESC");
                        IrDeps[j] = 0;
                        if (odatarows.Length > 0)
                            IrDeps[j] = Convert.ToInt16(odatarows[0]["NDEPEND"]);
                    }

                    odatarow.BeginEdit();
                    odatarow["DEPEND"] = Deps[0];
                    odatarow["IRDEPEND"] = IrDeps[0];

                    for (int j = 0; j < anos; j++)
                    {
                        if (ocltreaj == CltRelReajuste.MensalistasNaDataBase)
                        {
                            if (j == 0)
                            {
                                Decimal reajustesalario = 0;

                                Decimal ocalculado = CalcSalBase(Convert.ToString(odatarow["CODCAD"]),
                                    Convert.ToDateTime(odatarow["ADMI"]), Convert.ToDecimal(odatarow["SALBASE"]),SalarioMinimos[j], Datas2[j],
                                    dsReajustes.Tables[0]);
                                Decimal ocalculado2 = ocalculado;
                                // verifica se já foi dado o reajuste nesta database
                                DataRow[] orow = dsReajustes.Tables[0].Select("TRAB = '" + Convert.ToString(odatarow["CODCAD"]) + "'", "DATA DESC");
                                if ((orow.Length == 0) || (Convert.ToDateTime(orow[0]["DATA"]).CompareTo(data1) < 0))
                                {
                                    reajustesalario = Math.Round(SalarioMinimos[0] / SalarioMinimos[1], 6);

                                    {
                                        ocalculado = ptRound(ocalculado * reajustesalario, 4);
                                    }
                                }
                                odatarow["REAJUSTE" + Datas2[j].Month.ToString()] = Convert.ToDecimal((reajustesalario - 1) * 100);
                                odatarow["SALBRUTO" + Datas2[j].Month.ToString()] = ocalculado;
                                ocalculado2 = ptRound(ocalculado2 * (1+(taxa/100)), 4);
                                odatarow["REAJUSTETAXA"] = Convert.ToDecimal(taxa);
                                odatarow["SALBRUTOTAXA"] = ocalculado2;
                                odatarow["SALLIQTAXA"] = CalcSalLiq(Datas2[j],
                                ocalculado2,
                                Deps[j], IrDeps[j], dsTabelasTrabalhistas);    
                   
                            }
                            else
                                odatarow["SALBRUTO" + Datas2[j].Month.ToString()] =
                                    CalcSalBase(Convert.ToString(odatarow["CODCAD"]),
                                    Convert.ToDateTime(odatarow["ADMI"]), Convert.ToDecimal(odatarow["SALBASE"]), SalarioMinimos[j], Datas2[j],
                                    dsReajustes.Tables[0]);
                            odatarow["SALLIQ" + Datas2[j].Month.ToString()] = CalcSalLiq(Datas2[j],
                             Convert.ToDecimal(odatarow["SALBRUTO" + Datas2[j].Month.ToString()]),
                             Deps[j], IrDeps[j], dsTabelasTrabalhistas);
                           
                        }
                        else
                        {
                            odatarow["SALBRUTO" + Datas2[j].Year.ToString()] = CalcSalBase(Convert.ToString(odatarow["CODCAD"]),
                                    Convert.ToDateTime(odatarow["ADMI"]), Convert.ToDecimal(odatarow["SALBASE"]), SalarioMinimos[j], Datas2[j],
                                    dsReajustes.Tables[0]);
                              
                            odatarow["SALLIQ" + Datas2[j].Year.ToString()] = CalcSalLiq(Datas2[j],
                                Convert.ToDecimal(odatarow["SALBRUTO" + Datas2[j].Year.ToString()]),
                                Deps[j], IrDeps[j], dsTabelasTrabalhistas);
                        }
                    }
                    if (ocltreaj == CltRelReajuste.MensalistasNaDataBase) 
                    {
                        odatarow["DIFSALARIO"] = Convert.ToDecimal(odatarow["SALLIQ" + Datas2[0].Month.ToString()])
                                                          - Convert.ToDecimal(odatarow["SALLIQ" + Datas2[1].Month.ToString()]);
                        odatarow["DIFSALARIOTAXA"] = Convert.ToDecimal(odatarow["SALLIQTAXA"])
                                                          - Convert.ToDecimal(odatarow["SALLIQ" + Datas2[1].Month.ToString()]);
                            
                    }
                    odatarow.EndEdit();
                    odatarow.AcceptChanges();
                    tabDeb.Rows.Add(odatarow.ItemArray);
                }
                result = new DataSet();
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            { throw; }
        }





        // rotina padrao para arredondar calculos de fgts e outros (busquei da rotina do delphi
        static public Decimal ptTruncRound(Decimal valor, Int16 numero)
        {
            Decimal[] divisor = new Decimal[5] { 10.0M, 100.0M, 1000.0M, 10000.0M, 100000.0M };
            if (numero > 4)
            {
                MessageBox.Show("Limite de PtTruncRound = 4");
                return valor;
            }
            if (numero < 2)
            {
                MessageBox.Show("Limite de PtTruncRound > 1");
                return valor;
            }
            try
            {
                CultureInfo ci = new CultureInfo("en-us");
                Decimal tint;
                Decimal tfrace;
                valor = ptRoundparabaixo(valor, Convert.ToInt16(numero+1));
                tint = Decimal.Truncate(valor);

                tfrace = Decimal.Remainder(valor, 1.0M);
                if (tfrace > 0.0M)
                {
                    tfrace = Decimal.Round(Decimal.Multiply(tfrace, divisor[numero - 1]),2);
                    string tlit = tfrace.ToString("F02",ci);//("D02", ci);///////format('%2.1f',[tfrace]);
                    string result = "";
                    for (int i = 0; i < tlit.Length; i++)
                    {
                        if (tlit.Substring(i, 1) == ".") break;
                        result += tlit.Substring(i, 1);
                    }

                    Int32 tfraceint = Convert.ToInt32(result);
                    tfrace = tfraceint / divisor[(numero - 1)];
                }

               return (tint + tfrace);
            }
            catch
            {
                return valor;
            }
        }
        
        // rotina padrao para arredondar calculos de fgts e outros (busquei da rotina do delphi
        static public Decimal Roundparabaixo(Decimal valor, Int16 numero)
        {
            Decimal tint;
            Decimal tfrace;
            tint = Decimal.Truncate(valor);
            tfrace = Decimal.Remainder(valor, 1.0M);
          //  tfrace = (valor - tint);
            if (tfrace == 0.5M)
            {
               if (numero > 2)
                    tint += 1;
                else
                {
                   
                   string strint = Convert.ToString(tint).Trim();
                    if (strint.Length > 1)
                    {
                        int ult = Convert.ToInt32(strint.Substring(strint.Length - 1, 1));
                        if (ult >= 5)
                            tint += 1;
                    }
                }
                return tint;
            }
            if (tfrace > 0.5M)
            {
                tint += 1;
            }
            return tint;

        }
        static public Decimal Roundparabaixoradical(Decimal valor, Int16 numero)
        {
            Decimal tint;
            Decimal tfrace;
            tint = Decimal.Truncate(valor);
            tfrace = Decimal.Remainder(valor, 1.0M);
             if (tfrace > 0.5M)
            {
                tint += 1;
            }
            return tint;

        }

        
        static public decimal ptRound2(Decimal valor, Int16 numero)
        {
            Decimal[] divisor = new Decimal[6] { 10M, 100M, 1000M, 10000M, 100000M, 1000000M };
            Int32 numero2 = numero - 1;
            if (numero2 > 5)
            {
                MessageBox.Show("Limite de PtTruncRound = 5");
                return valor;
            }
            if (numero2 < 0)
            {
                MessageBox.Show("Limite de PtTruncRound >= 1");
                return valor;
            }
            try
            {
                Decimal vlrint = Decimal.Multiply(valor, divisor[numero2]);
                vlrint = Decimal.Round(vlrint);//,MidpointRounding.AwayFromZero);
                decimal result = Decimal.Divide(vlrint, divisor[numero2]);
                return (result);
                
            }
            catch
            {
                return valor;
            }
        }

        static public decimal ptRound(Decimal valor, Int16 numero)
        {
            Decimal[] divisor = new Decimal[6] { 10.0M, 100.0M, 1000.0M, 10000.0M, 100000.0M, 1000000.0M };
            Int32 numero2 = numero - 1;
            if (numero2 > 5)
            {
                MessageBox.Show("Limite de PtTruncRound = 5");
                return valor;
            }
            if (numero2 < 0)
            {
                MessageBox.Show("Limite de PtTruncRound >= 1");
                return valor;
            }
            try
            {
                Decimal vlrint = Math.Round(Decimal.Multiply(valor, divisor[numero2]),5);
                vlrint =   Roundparabaixo(vlrint, numero);
                decimal result = Math.Round(Decimal.Divide(vlrint, divisor[numero2]),numero);
                return (result);
             }
            catch
            {
                return valor;
            }
        }
        static public decimal ptRoundparabaixo(Decimal valor, Int16 numero)
        {
            Decimal[] divisor = new Decimal[6] { 10.0M, 100.0M, 1000.0M, 10000.0M, 100000.0M, 1000000.0M };
            Int32 numero2 = numero - 1;
            if (numero2 > 5)
            {
                MessageBox.Show("Limite de PtTruncRound = 5");
                return valor;
            }
            if (numero2 < 0)
            {
                MessageBox.Show("Limite de PtTruncRound >= 1");
                return valor;
            }
            try
            {
                Decimal vlrint = Math.Round(Decimal.Multiply(valor, divisor[numero2]), 5);
                vlrint = Roundparabaixoradical(vlrint, numero);
                decimal result = Decimal.Divide(vlrint, divisor[numero2]);
                return (result);
            }
            catch
            {
                return valor;
            }
        }

       
 
        static public decimal CalcSalBase(string codcad, DateTime admi, decimal salbase, decimal salmin, DateTime data, DataTable CltReaj)
        {
            if (data < admi)
                return 0.0M;

            decimal result = salbase;
            decimal fatordesempate=result;
            DataRow[] orow = CltReaj.Select("TRAB = '" + codcad + "'", "DATA");
         
            foreach (DataRow olinha in orow)
            {
                if ((Convert.ToDateTime(olinha["DATA"]) <= data) && (Convert.ToDecimal(olinha["REAJ"]) != 0.0M))
                {
                    
                    decimal fator = (Convert.ToDecimal(olinha["REAJ"]) / 100.0M) + 1.0M;
                   fator =  Decimal.Round(fator, 8);
                    result = Decimal.Multiply(fator,result);
                    fatordesempate = result;
                    result = Decimal.Round(result, 4, MidpointRounding.AwayFromZero);
                }
            }
            Decimal result3 = Decimal.Multiply(result, salmin);
            Decimal resto = Decimal.Remainder((result3*100), 1M);
            Decimal salario1 = ptRound2(result3, 2);
            decimal escolha = salario1;
            if (resto == 0.5M)
            {
                result = Math.Round(Decimal.Multiply(fatordesempate, salmin),2) ;
                Decimal sobra = Math.Round(result3, 2,MidpointRounding.AwayFromZero);
                if (sobra.CompareTo(result) == 0)
                    escolha = sobra;
            }
            return escolha;
        }

        static public Decimal CalcFatorSalBase(string codcad, DateTime admi, Decimal salbase, DateTime data, DataTable CltReaj)
        {
            if (data < admi)
                return 0.0M;

            Decimal result = salbase;
            DateTime datacompara = data;
            DataRow[] orow = CltReaj.Select("TRAB = '" + codcad + "'", "DATA");
            foreach (DataRow olinha in orow)
            {
                if ((Convert.ToDateTime(olinha["DATA"]) <= data) && (Convert.ToDecimal(olinha["REAJ"]) != 0.0M))
                {
                    result = ptRound(Math.Round(result * (1 + (Convert.ToDecimal(olinha["REAJ"]) / 100)), 8), 4);
                }
            }
            return result;
        }




      
    }
}








