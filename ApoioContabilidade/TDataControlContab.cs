using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Windows.Forms;
using System.Globalization;
using ClassConexao;
using System.IO;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    class TDataControlContab
    {
        static public OleDbDataAdapter AdapterGet_Placon()
        {
            OleDbCommand odbccomm;
            string strodbc, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            strodbc = "SELECT * ,recno() as Numero FROM " + path + "PLACON";
            try
            {
                odbccomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leia = odbccomm.ExecuteReader(CommandBehavior.SchemaOnly);
                DataTable schema = leia.GetSchemaTable();
                leia.Close();
                Boolean ok = false;
                foreach (DataRow row in schema.Rows)
                    if (row[0].ToString().ToUpper() == "NOVOCOD")
                    {
                        ok = true;
                        break;
                    }
                if (!ok)
                {
                    TDataControlReduzido.AlteraTabela("CONTAB", "PLACON", "NOVOCOD", "CHAR(30)");
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();
                }
                ok = false;
                foreach (DataRow row in schema.Rows)
                    if (row[0].ToString().ToUpper() == "NOVADESC")
                    {
                        ok = true;
                        break;
                    }
                if (!ok)
                {
                    TDataControlReduzido.AlteraTabela("CONTAB", "PLACON", "NOVADESC", "CHAR(50)");
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();
                }

                odbccomm = new OleDbCommand(strodbc, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter odbcda = new OleDbDataAdapter(odbccomm);

                // comando de alteraçao
                // ,recno() as Numero 
                OleDbCommand cmd = new OleDbCommand("UPDATE PLACON SET NOVOCOD = ?,NUMCONTA = ?,NOVADESC = ? " +
                       "WHERE (NUMCONTA = ?) AND (RECNO() = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@NOVOCOD", OleDbType.Char, 30, "NOVOCOD");
                cmd.Parameters.Add("@NUMCONTA", OleDbType.Char, 8, "NUMCONTA");
                cmd.Parameters.Add("@NOVADESC", OleDbType.Char, 50, "NOVADESC");
                OleDbParameter parm;
                parm = cmd.Parameters.Add("@OLDNUMCONTA", OleDbType.Char, 8, "NUMCONTA");
                parm.SourceVersion = DataRowVersion.Original;
                parm = cmd.Parameters.Add("@NREG", OleDbType.Integer, 10, "NUMERO");
                parm.SourceVersion = DataRowVersion.Original;

                // da.UpdateCommand = cmd;
                odbcda.UpdateCommand = cmd;

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM PLACON WHERE (NUMCONTA = ?) AND (RECNO() = ?)",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbParameter parameter = command.Parameters.Add(
                    "@NUMCONTA", OleDbType.Char, 8, "NUMCONTA");
                parameter.SourceVersion = DataRowVersion.Original;
                OleDbParameter parameter2 = command.Parameters.Add(
                   "@NREG", OleDbType.Integer, 10, "NUMERO");
                parameter2.SourceVersion = DataRowVersion.Original;


                odbcda.DeleteCommand = command;
                return odbcda;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "PLACON"));
            }

        }

        // Encontre Tabelas(PTMOVFIN--ano)  Para o FrmRelaciona
        static public List<string> EncontreTabela(bool incluaMovFin = false)
        {
            List<string> tabelas = new List<string>();
            string path = TDataControlReduzido.Get_Path("CONTAB");
            
            var dir = new DirectoryInfo(path);

            
            // foreach (var file in FindDiretorio.TraverseDirectory(path, f => f.Extension == ".DBF"))
            foreach (var file in dir.GetFiles().Where( f => f.Extension == ".DBF"))
            {
                
                if (file.Name.Contains("PTMOVFIN") || file.Name.Contains("PTPLACON") )
                {
                    tabelas.Add(file.Name);
                }
                if (incluaMovFin  && (file.Name.ToUpper() == "MOVFIN.DBF" ) && (!tabelas.Contains("MOVFIN.DBF")))
                {
                    tabelas.Add(file.Name);
                }

            }
            return tabelas;
        }
        static public List<string> EncontreTabelaPlacon()
        {
            List<string> tabelas = new List<string>();
            string path = TDataControlReduzido.Get_Path("CONTAB");

            var dir = new DirectoryInfo(path);


            // foreach (var file in FindDiretorio.TraverseDirectory(path, f => f.Extension == ".DBF"))
            foreach (var file in dir.GetFiles().Where(f => f.Extension == ".DBF"))
            {

                if (file.Name.Contains("PTPLA"))
                {
                    tabelas.Add(file.Name);
                }
            }
            return tabelas;
        }


        static public bool EncontreCopiePTMovFin(string ano)
        {
            bool result = true;
            List<string> tabelas = new List<string>();
            string path = TDataControlReduzido.Get_Path("CONTAB");

            var dir = new DirectoryInfo(path);
            
            foreach (var file in dir.GetFiles().Where(f => f.Extension == ".DBF"))
            {

                if ((file.Name.Contains("PTMOVFIN")  || (file.Name.ToUpper() == "MOVFIN.DBF")))
                {
                    tabelas.Add(file.FullName);
                }
            }
            if (tabelas.Count == 0) {
                result = false;
                MessageBox.Show("Diretorio " + path + " Não contém tabela MOVFIN.DBF ou PTMOVFIN.DBF para modelo da nova tabela ");
                return result;
            }
            if (!(tabelas.Contains(Path.Combine(path, "PTMOVFIN" + ano + ".DBF"))))
            {
                string tabelaOrig = "";
                tabelaOrig = tabelas[0];
                File.Copy(tabelaOrig, Path.Combine(path, "PTMOVFIN" + ano + ".DBF"));
            }
            return result;
        }
        static public bool TabelaExiste(string tabela)
        {
            bool result = false;
            string path = TDataControlReduzido.Get_Path("CONTAB");
            result = File.Exists(Path.Combine(path, tabela));
            return result;
        }

        static public void DropTabela()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            strOleDb = "DROP TABLE " + path + "RELACIONA";
            OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

            try
            {
                OleDbcomm.ExecuteNonQuery();
            }
            catch (Exception)
            { }
        }

        static public OleDbDataAdapter CreateTabRelaciona()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            //if (File.Exists(path + "RELACIONA.DBF"))
            //  File.Delete(path + "RELACIONA.DBF");



            try
            {

                if (!(File.Exists(path + "RELACIONA.DBF")))
                {
                    strOleDb = "CREATE TABLE " + path + "RELACIONA (NREG INTEGER, NUMCONTA CHAR(8),NOVOCOD CHAR(30),DESCRICAO CHAR(40),NOVADESC CHAR(50),REDUZIDO INTEGER )";
                    OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                    try
                    {
                        OleDbcomm.ExecuteNonQuery();
                    }
                    catch (Exception)
                    { throw; }
                }
                strOleDb = "SELECT * FROM " + path + "RELACIONA";
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);

                // comando de alteraçao
                OleDbCommand cmd = new OleDbCommand("INSERT INTO " + path + "RELACIONA (NREG, NUMCONTA,NOVOCOD,DESCRICAO,NOVADESC, REDUZIDO) VALUES(?, ?,?,?,?,?)",
                         TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@NREG", OleDbType.Integer, 10, "NREG");
                cmd.Parameters.Add("@NUMCONTA", OleDbType.Char, 8, "NUMCONTA");
                cmd.Parameters.Add("@NOVOCOD", OleDbType.Char, 30, "NOVOCOD");
                cmd.Parameters.Add("@DESCRICAO", OleDbType.Char, 40, "DESCRICAO");
                cmd.Parameters.Add("@NOVADESC", OleDbType.Char, 50, "NOVADESC");
                cmd.Parameters.Add("@REDUZIDO", OleDbType.Integer, 10, "REDUZIDO");

                OleDbda.InsertCommand = cmd;



                cmd = new OleDbCommand("UPDATE RELACIONA SET NOVOCOD = ?,NUMCONTA = ?,DESCRICAO = ?,NOVADESC = ? " +
                       "WHERE (NREG = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@NOVOCOD", OleDbType.Char, 30, "NOVOCOD");
                cmd.Parameters.Add("@NUMCONTA", OleDbType.Char, 8, "NUMCONTA");
                cmd.Parameters.Add("@DESCRICAO", OleDbType.Char, 40, "DESCRICAO");
                cmd.Parameters.Add("@NOVADESC", OleDbType.Char, 50, "NOVADESC");
                cmd.Parameters.Add("@NREG", OleDbType.Integer, 10, "NREG");
                cmd.Parameters.Add("@REDUZIDO", OleDbType.Integer, 10, "REDUZIDO");

                OleDbParameter parm;
                parm = cmd.Parameters.Add("@NREG", OleDbType.Integer, 10, "NREG");
                parm.SourceVersion = DataRowVersion.Original;

                // da.UpdateCommand = cmd;
                OleDbda.UpdateCommand = cmd;

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM RELACIONA WHERE (NREG = ?)",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbParameter parameter2 = command.Parameters.Add(
                   "@NREG", OleDbType.Integer, 10, "NREG");
                parameter2.SourceVersion = DataRowVersion.Original;


                OleDbda.DeleteCommand = command;
                return OleDbda;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "PLACON"));
            }

        }

        static public bool DeleteTabRelaciona()
        {
            // OleDbCommand OleDbcomm;
            string path;
            path = TDataControlReduzido.Get_Path("CONTAB");


            try
            {

                if (!(File.Exists(path + "RELACIONA.DBF")))
                {
                    return true;
                }
                //strOleDb = "DELETE FROM " + path + "RELACIONA";
                //OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                //OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM " + path + "RELACIONA ",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());



                //OleDbda.DeleteCommand = command;

                bool result = false;
                try
                {
                    command.ExecuteScalar();
                    result = true;
                }
                catch (Exception)
                {

                    throw;
                }

                return result;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não Deletou", path + "RELACIONA"));
            }

        }
        static public void PonhaValoresDefault(DataRow orow)
        {
            foreach (DataColumn col in orow.Table.Columns)
            {
                //if (col.ColumnName.ToUpper() == "ID") continue;
                Type tipo = col.DataType;

                if (tipo == Type.GetType("System.DateTime"))
                {
                    try
                    {
                        orow[col.ColumnName] = DateTime.MinValue;
                    }
                    catch (Exception) { }
                }
                else if (tipo == Type.GetType("System.String"))
                {
                    orow[col.ColumnName] = "";
                }
                else if (tipo == Type.GetType("System.Boolean"))
                {
                    orow[col.ColumnName] = 0;
                }
                else
                {
                    orow[col.ColumnName] = 0;
                }
            }
        }


        static public OleDbDataAdapter GravePtMovFin(string tabela)
        {
            {

                string path = TDataControlReduzido.Get_Path("CONTAB");


                // string strOleDb = "SELECT *  FROM " + path + "MOVFIN";
                try
                {

                    OleDbDataAdapter OleDbda = new OleDbDataAdapter();
                    //data, valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, venc, doc_dupl, nhist, doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof

                    OleDbCommand cmd = new OleDbCommand("INSERT INTO " + path + tabela+" ( DATA,VALOR,DEBITO,CREDITO, " +
                          " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,DATA_EMI,TP_OK,TIPO_DOC, " +
                          " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                            "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF)" +

                           " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?)"
                            , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                    cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                    cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                    cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                    cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                    cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                    cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                    cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                    cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                    cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean,1,"TP_FIN" );
                    cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
                    // cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 0, "NUMREG");//RECNO()
                    cmd.Parameters.Add("@DATA_EMI", OleDbType.Date, 8, "DATA_EMI");
                    cmd.Parameters.Add("@TP_OK", OleDbType.Boolean, 1).Value = false;
                    cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char, 1).Value = " ";// Char.Parse("");

                    cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                    cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                    cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                    cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                    cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric,10,"MOV_ID");
                    cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric).Value = 0;
                    cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                    cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                    cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                    cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                    cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                    cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                    cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");

                    OleDbda.InsertCommand = cmd;

                    cmd = new OleDbCommand("UPDATE " + path + tabela + " SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                        " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TIPO = ?" +
                           "WHERE  (RECNO() = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                    cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                    cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                    cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                    cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                    cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                    cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                    cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                    cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                   // cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean).Value = true;
                    cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");

                    OleDbParameter parm;
                    parm = cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");
                    //parm.SourceVersion = DataRowVersion.Original;

                    // da.UpdateCommand = cmd;
                    OleDbda.UpdateCommand = cmd;

                    // commnado de exclusao
                    OleDbCommand command = new OleDbCommand(
                           "DELETE FROM " + path + tabela + " WHERE (MOV_ID = ?)",
                           TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                    OleDbParameter parameter2 = command.Parameters.Add(
                       "@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");
                    parameter2.SourceVersion = DataRowVersion.Original;


                    OleDbda.DeleteCommand = command;

                    // commnado Retorna maior numero
                    command = new OleDbCommand(
                           "SELEC MAX(recno()) AS numero FROM " + path + tabela,
                           TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                    OleDbda.SelectCommand = command;
                    return OleDbda;
                }
                catch (Exception)
                {
                    throw new Exception(string.Format("{0}Não foi possivel acessar", path + tabela));
                }

            }


        }



        static public DataSet TabRelaciona()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");

            try
            {

                strOleDb = "SELECT * FROM " + path + "RELACIONA";
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);

                OleDbda.TableMappings.Add("Table", "RELACIONA");
                DataSet odataset = new DataSet();
                OleDbda.Fill(odataset);
                return odataset;

            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "RELACIONA"));
            }

        }

        static public DataSet TabRelacionaVelho()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");

            try
            {

                strOleDb = "SELECT * FROM " + path + "VELHORELACIONA";
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);

                OleDbda.TableMappings.Add("Table", "VELHORELACIONA");
                DataSet odataset = new DataSet();
                OleDbda.Fill(odataset);
                return odataset;

            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "VELHORELACIONA"));
            }

        }




        static public DataSet TabBancos()
        {
            OleDbCommand OleDbcomm;
            string strOleDb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");

            try
            {

                strOleDb = "SELECT nbanco,contab,desc2, nome_banco FROM " + path + "BANCOS";
                OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter OleDbda = new OleDbDataAdapter(OleDbcomm);

                OleDbda.TableMappings.Add("Table", "BANCOS");
                DataSet odataset = new DataSet();
                OleDbda.Fill(odataset);
                return odataset;

            }
            catch (Exception E)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "BANCOS"));
            }

        }




        //.. tem que acessar servidor a cada pesquisa...(isso é pouco eficiente) 
        static public bool EBanco(string conta, Boolean contabil)
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
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT * FROM " + path + "BANCOS where (nBANCO = " + conta + ")";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataReader leitor = oledbcomm.ExecuteReader();
            if (leitor.HasRows)
            {
                if (leitor.Read())
                {
                    if (contabil)
                    {
                        if (Convert.ToString(leitor["CONTAB"]).Trim() == "")
                            result = false;
                        else
                            result = true;

                    }
                    else
                        result = true;
                }
            }
            leitor.Close();
            return result;
        }

        static public string EBancoContab(string conta)
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
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT * FROM " + path + "BANCOS where (nBANCO = " + conta + ")";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataReader leitor = oledbcomm.ExecuteReader();
            if (leitor.HasRows)
            {
                if (leitor.Read())
                {
                    if (Convert.IsDBNull(leitor["CONTAB"]))
                        result = "";
                    else
                        result = Convert.ToString(leitor["CONTAB"]);


                }
            }
            leitor.Close();
            return result;
        }

        static public bool EBancoNormalizado(string conta, PesquisaGenerico opesquise, Boolean contabil)
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
            if (intconta == 0) return result;
            conta = intconta.ToString("D2");

            DataRow[] linhas = opesquise.PesqLinhas("BANCOS", new string[1] { "NBANCO" }, new string[1] { conta });
            if (linhas.Length == 0) return result;
            if (contabil && (Convert.ToString(linhas[0]["CONTAB"]).Trim() == ""))
                return result;
            return true;
        }


        static private string Ache_Emissor(string conta)
        {
            string result = "";
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT numconta,DESC2 FROM " + path + "PLACON  where (DESC2 = '" + conta.Replace("'", "''") + "')";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataReader leitor = oledbcomm.ExecuteReader();
            if (leitor.HasRows)
            {
                if (leitor.Read())
                    result = (string)leitor["NUMCONTA"];

            }
            leitor.Close();
            return result;
        }

        static private bool EPlano(string conta)
        {
            conta = Ache_Emissor(conta);
            if ((conta != "") && (conta.Substring(5, 3) != "000"))
                return true;
            return false;
        }

        static public string EPlanoRetornaNumConta(string conta, PesquisaGenerico oPesquisa)
        {
            DataRow[] linha = oPesquisa.PesqLinhas("PLACON", new string[1] { "DESC2" }, new string[1] { conta });
            if ((linha == null) || (linha.Length == 0)) return "";
            string numconta = Convert.ToString(linha[0]["NUMCONTA"]);
            return numconta;
        }
        static public Boolean fPassa_Contab2(LinhaSolucao oLinha, DataRow odatarow, PesquisaGenerico opesquisa)
        {
            bool result = false;
            if ((oLinha.dado[0] == "") || (oLinha.dado[0] == "*")) return result;
            bool classificado = false;
            if (opesquisa == null)
            {
                if ((Boolean)odatarow["TP_FIN"] == true)
                {
                    if ((string)odatarow["TIPO"] == "P")
                    {
                        if (((EBancoNormalizado((string)odatarow["CREDITO"], opesquisa, true)))//|| (((string)odatarow["CREDITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                            classificado = true;
                    }
                    else
                        if (((EBancoNormalizado((string)odatarow["DEBITO"], opesquisa, true)))// || (((string)odatarow["DEBITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
                else
                {
                    if (((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])) || (((string)odatarow["CREDITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                        classificado = true;
                    if ((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"]) || (((string)odatarow["DEBITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
            }
            else
            {
                if ((Boolean)odatarow["TP_FIN"] == true)
                {
                    if ((string)odatarow["TIPO"] == "P")
                    {                                          //  || (((string)odatarow["CREDITO"]).Trim() == "00"))
                        if (((EBancoNormalizado((string)odatarow["CREDITO"], opesquisa, true)))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                            classificado = true;
                    }
                    else
                        if (((EBancoNormalizado((string)odatarow["DEBITO"], opesquisa, true)))//|| (((string)odatarow["DEBITO"]).Trim() == "00")))
                            && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
                else
                {
                    if (((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])) || (((string)odatarow["CREDITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"])))
                        classificado = true;
                    if ((opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["DEBITO"]) || (((string)odatarow["DEBITO"]).Trim() == ""))
                     && (opesquisa.ExisteValor("PLACON", "DESC2", (string)odatarow["CREDITO"])))
                        classificado = true;
                }
            }



            if ((classificado) && (oLinha.dado[0].Substring(0, 1) == "S")) result = true;
            if ((!classificado) && (oLinha.dado[0].Substring(0, 1) == "N")) result = true;

            return result;
        }


        // construindo delegates....
        // Delegate que construi como pedaço de código SQL
        /*  static public string CompareValor(LinhaSolucao oLinha)
          {
              string result = "";
              if ((oLinha.dado[0] == "") || (oLinha.dado[1] == "") || (oLinha.campo.Trim() == ""))
                  return result;
              string valor = "";
              try
              {
                  double ovalor = Convert.ToDouble(oLinha.dado[1].Trim());
                  valor = TDataControlReduzido.FormatDoubleGravar(ovalor); //2DECIMAIS
              }
              catch
              {
                  throw;
              }

              string comparador = oLinha.dado[0].Trim();
              result = oLinha.campo.Trim() + " " + comparador + " " + valor;
              if (result.Length > 0)
              { result = "  AND (" + result + ")"; }
              return result;
          }




          static public List<string> StringparaLista(string campo)
          {
              List<string> ostring = new List<string>();

              campo.Trim();
              int i = campo.IndexOf("/");
              while (i > 0)
              {
                  ostring.Add(campo.Substring(0, i));
                  campo = campo.Remove(0, i + 1);
                  i = campo.IndexOf("/");
              }
              if (campo.Length > 0)
                  ostring.Add(campo);

              return ostring;
          }

          static public string TDataControlReduzido.ConstruaSql(string campo, List<string> dados)
          {
              string result = "";
              List<string> vetdados = new List<string>();
              List<string> vetcampos = StringparaLista(campo);
              for (int i = 0; i < dados.Count; i++)
              {
                  List<string> vetprov = StringparaLista(dados[i]);
                  for (int j = 0; j < vetprov.Count; j++)
                  { vetdados.Add(vetprov[j]); }
              }

              for (int i = 0; i < vetcampos.Count; i++)
              {
                  for (int j = 0; j < vetdados.Count; j++)
                  {
                      if (result != "") result += " OR ";
                      result += "(" + vetcampos[i] + " LIKE " + "'%" + vetdados[j] + "%')";
                  }
              }
              if (result.Length > 0) result = "  AND (" + result + ")";
              return result;
          }
          */
        static public void AltereTabelaMovim()
        {

            string path = TDataControlReduzido.Get_Path("CONTAB");


            string strOleDb = "SELECT *  FROM " + path + "MOVFIN";
            try
            {
                OleDbCommand OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leia = OleDbcomm.ExecuteReader(CommandBehavior.SchemaOnly);
                DataTable schema = leia.GetSchemaTable();
                leia.Close();
                Boolean ok = false;
                foreach (DataRow row in schema.Rows)
                    if (row[0].ToString().ToUpper() == "NUMREG")
                    {
                        ok = true;
                        break;
                    }


                if (!ok)
                {
                    try
                    {
                        TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Close();
                        TDataControlReduzido.AlteraTabela("CONTAB", "MOVFIN", "NUMREG", "NUMERIC");
                        TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                try
                {
                    OleDbCommand cmd1 = new OleDbCommand("UPDATE  " + path + "MOVFIN SET NUMREG = RECNO() WHERE (NUMREG = 0)"
                       , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                    cmd1.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    throw;
                }


            }
            catch (Exception)
            {
                throw;
            }

        }

        static public void AltereTabelaMov_Fin()
        {

            string path = TDataControlReduzido.Get_Path("ESCRITOR");


            string strOleDb = "SELECT *  FROM " + path + "MOV_FIN";
            try
            {
                OleDbCommand OleDbcomm = new OleDbCommand(strOleDb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataReader leia = OleDbcomm.ExecuteReader(CommandBehavior.SchemaOnly);
                DataTable schema = leia.GetSchemaTable();
                leia.Close();
                Boolean ok = false;
                foreach (DataRow row in schema.Rows)
                    if (row[0].ToString().ToUpper() == "NUMREG")
                    {
                        ok = true;
                        break;
                    }


                if (!ok)
                {
                    try
                    {
                        TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Close();
                        TDataControlReduzido.AlteraTabela("ESCRITOR", "MOV_FIN", "NUMREG", "NUMERIC");
                        TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }

                try
                {
                    OleDbCommand cmd1 = new OleDbCommand("UPDATE  " + path + "MOV_FIN SET NUMREG = RECNO() WHERE (NUMREG = 0)"
                       , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                    cmd1.ExecuteNonQuery();

                }
                catch (Exception)
                {
                    throw;
                }


            }
            catch (Exception)
            {
                throw;
            }

        }


        static public OleDbDataAdapter ConstruaAdaptador()
        {

            string path = TDataControlReduzido.Get_Path("CONTAB");


            // string strOleDb = "SELECT *  FROM " + path + "MOVFIN";
            try
            {

                OleDbDataAdapter OleDbda = new OleDbDataAdapter();

                OleDbCommand cmd = new OleDbCommand("INSERT INTO " + path + "MOVFIN ( DATA,VALOR,DEBITO,CREDITO, " +
                      " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,NUMREG,DATA_EMI,TP_OK,TIPO_DOC, " +
                      " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                        "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF)" +

                       " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?,?)"
                        , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = true;
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
                cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 0, "NUMREG");//RECNO()
                cmd.Parameters.Add("@DATA_EMI", OleDbType.Date, 8, "DATA_EMI");
                cmd.Parameters.Add("@TP_OK", OleDbType.Boolean, 1).Value = false;
                cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char, 1).Value = " ";// Char.Parse("");

                cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");

                OleDbda.InsertCommand = cmd;

                cmd = new OleDbCommand("UPDATE " + path + "MOVFIN SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                    " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TP_FIN = ?,TIPO = ?" +
                       "WHERE  (RECNO() = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = true;
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");

                OleDbParameter parm;
                parm = cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 10, "NUMREG");
                parm.SourceVersion = DataRowVersion.Original;

                // da.UpdateCommand = cmd;
                OleDbda.UpdateCommand = cmd;

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM " + path + "MOVFIN WHERE (NUMREG = ?)",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbParameter parameter2 = command.Parameters.Add(
                   "@NUMREG", OleDbType.Numeric, 10, "NUMREG");
                parameter2.SourceVersion = DataRowVersion.Original;


                OleDbda.DeleteCommand = command;

                // commnado Retorna maior numero
                command = new OleDbCommand(
                       "SELEC MAX(recno()) AS numero FROM " + path + "MOVFIN",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbda.SelectCommand = command;
                return OleDbda;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "MOVFIN"));
            }
        }
        static public int MaiorNRegMovim()
        {
            string path = TDataControlReduzido.Get_Path("CONTAB");
            OleDbCommand command = new OleDbCommand(
                    "SELEC MAX(recno()) AS numero FROM " + path + "MOVFIN",
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            //int numero = 0;
            try
            {
                if (TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().State == ConnectionState.Closed)
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();
                return Convert.ToInt32(command.ExecuteScalar());
            }
            catch (Exception)
            {
                return -1;
            }
        }

        static public int MaiorMov_ID_Movim(string tabela)
        {
            string path = TDataControlReduzido.Get_Path("CONTAB");
            OleDbCommand command = new OleDbCommand(
                    "SELEC MAX(MOV_ID) AS MAX_MOV_ID FROM " + path + tabela,
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            //int numero = 0;
            try
            {
                if (TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().State == ConnectionState.Closed)
                    TDataControlReduzido.ConnectionPooling.GetConnectionOleDb().Open();
                var result = command.ExecuteScalar();
                return Convert.ToInt32(result);
            }
            catch (Exception)
            {
                return -1;
            }
        }
        // para inserir, alterar e deletar o PTMOVFIN<ano>
        static public OleDbDataAdapter ConstruaAdaptador_PTMovFin(string tabela)
        {

            string path = TDataControlReduzido.Get_Path("CONTAB");


            try
            {

                OleDbDataAdapter OleDbda = new OleDbDataAdapter();

                OleDbCommand cmd = new OleDbCommand("INSERT INTO " + path + tabela+" ( DATA,VALOR,DEBITO,CREDITO, " +
                      " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,DATA_EMI,TP_OK,TIPO_DOC, " +
                      " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                        "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF)" +
                       " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?)"
                        , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = 0;
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1).Value = " ";
                cmd.Parameters.Add("@DATA_EMI", OleDbType.Date, 8, "DATA_EMI");
               cmd.Parameters.Add("@TP_OK", OleDbType.Boolean, 1).Value = false;
                cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char, 1).Value = " ";// Char.Parse("");
                cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                 cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric,10, "MOV_ID");
                cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric,10, "OUTRO_ID");
                cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");

                OleDbda.InsertCommand = cmd;

                cmd = new OleDbCommand("UPDATE " + path + tabela + " SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                    " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TP_FIN = ?,TIPO = ?" +
                       "WHERE  (MOV_ID = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC").Value = "";
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1).Value = 0;
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");

                OleDbParameter parm;
                parm = cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");
                parm.SourceVersion = DataRowVersion.Original;

                // da.UpdateCommand = cmd;
                OleDbda.UpdateCommand = cmd;

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM " + path + tabela + " WHERE (MOV_ID = ?)",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbParameter parameter2 = command.Parameters.Add(
                   "@MOV_ID", OleDbType.Numeric, 10, "MOV_ID");
                parameter2.SourceVersion = DataRowVersion.Original;


                OleDbda.DeleteCommand = command;

                // commnado Retorna maior numero
                command = new OleDbCommand(
                       "SELEC MAX(recno()) AS numero FROM " + path + tabela ,
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbda.SelectCommand = command;
                return OleDbda;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + tabela));
            }
        }



        static public OleDbDataAdapter ConstruaAdaptador_MovFin()
        {

            string path = TDataControlReduzido.Get_Path("ESCRITOR");


            // string strOleDb = "SELECT *  FROM " + path + "MOV_FIN";
            try
            {

                OleDbDataAdapter OleDbda = new OleDbDataAdapter();

                OleDbCommand cmd = new OleDbCommand("INSERT INTO " + path + "MOV_FIN ( DATA,VALOR,DEBITO,CREDITO, " +
                      " FORN,HIST,DOC,DOC_FISC,VENC ,TP_FIN,TIPO,NUMREG,DATA_EMI,TP_OK,TIPO_DOC, " +
                      " DOC_DUPL, NHIST, EMISSOR,  OBS, MOV_ID, OUTRO_ID, SERIENF," +
                        "BASECALC, ISENTO, OUTROS, ICMS, LIVRO, CODIGOF)" +

                       " VALUES (?,?,?,?,?,?,?,?,?,?,?,?,?,?,? " + ",?,?,?,?,?,?,? " + ",?,?,?,?,?,?)"
                        , TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
                cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean, 1,"TP_FIN");
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");
                cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 0, "NUMREG");//RECNO()
                cmd.Parameters.Add("@DATA_EMI", OleDbType.Date, 8, "DATA_EMI");
                cmd.Parameters.Add("@TP_OK", OleDbType.Boolean, 1,"TP_OK");
                cmd.Parameters.Add("@TIPO_DOC", OleDbType.Char, 1).Value = " ";// Char.Parse("");

                cmd.Parameters.Add("@DOC_DUPL", OleDbType.Char, 8).Value = "        ";
                cmd.Parameters.Add("@NHIST", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@EMISSOR", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@OBS", OleDbType.Char, 3).Value = "   ";
                cmd.Parameters.Add("@MOV_ID", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@OUTRO_ID", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@SERIENF", OleDbType.Char, 4).Value = "    ";
                cmd.Parameters.Add("@BASECALC", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ISENTO", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@OUTROS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@ICMS", OleDbType.Numeric).Value = 0;
                cmd.Parameters.Add("@LIVRO", OleDbType.Char, 2).Value = "  ";// Char.Parse("");
                cmd.Parameters.Add("@CODIGOF", OleDbType.Char, 3).Value = "";// Char.Parse("");

                OleDbda.InsertCommand = cmd;

                cmd = new OleDbCommand("UPDATE " + path + "MOV_FIN SET DATA = ?,VALOR = ?,DEBITO = ?,CREDITO = ?, " +
                    " FORN = ?,HIST = ?,DOC = ?,DOC_FISC = ?,VENC=? ,TIPO = ?" +
                       "WHERE  (RECNO() = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
                cmd.Parameters.Add("@VALOR", OleDbType.Double, 0, "VALOR");
                cmd.Parameters.Add("@DEBITO", OleDbType.Char, 25, "DEBITO");
                cmd.Parameters.Add("@CREDITO", OleDbType.Char, 25, "CREDITO");
                cmd.Parameters.Add("@FORN", OleDbType.Char, 35, "FORN");
                cmd.Parameters.Add("@HIST", OleDbType.Char, 40, "HIST");
                cmd.Parameters.Add("@DOC", OleDbType.Char, 13, "DOC");
                cmd.Parameters.Add("@DOC_FISC", OleDbType.Char, 13, "DOC_FISC");
                cmd.Parameters.Add("@VENC", OleDbType.Date, 8, "VENC");
               // cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean,0, "TP_FIN");
                //cmd.Parameters.Add("@TP_FIN", OleDbType.Boolean,1).
                cmd.Parameters.Add("@TIPO", OleDbType.Char, 1, "TIPO");

                OleDbParameter parm;
                parm = cmd.Parameters.Add("@NUMREG", OleDbType.Numeric, 10, "NUMREG");
                parm.SourceVersion = DataRowVersion.Original;

                // da.UpdateCommand = cmd;
                OleDbda.UpdateCommand = cmd;

                // commnado de exclusao
                OleDbCommand command = new OleDbCommand(
                       "DELETE FROM " + path + "MOV_FIN WHERE (NUMREG = ?)",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbParameter parameter2 = command.Parameters.Add(
                   "@NUMREG", OleDbType.Numeric, 10, "NUMREG");
                parameter2.SourceVersion = DataRowVersion.Original;


                OleDbda.DeleteCommand = command;

                // commnado Retorna maior numero
                command = new OleDbCommand(
                       "SELEC MAX(recno()) AS numero FROM " + path + "MOV_FIN",
                       TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());

                OleDbda.SelectCommand = command;
                return OleDbda;
            }
            catch (Exception)
            {
                throw new Exception(string.Format("{0}Não foi possivel acessar", path + "MOV_FIN"));
            }
        }


        static public OleDbDataAdapter NovoMov_Fin(ref DataSet odataset, DateTime data1, DateTime data2, List<LinhaSolucao> oLista, string tipo, PesquisaGenerico oPesquisa)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            //  DataSet result = null;
            path = TDataControlReduzido.Get_Path("ESCRITOR");
            setoroledb = "SELECT *  FROM " + path + "MOV_FIN  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (TP_FIN = .T.) AND TIPO = '" + tipo + "'";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            setoroledb += " ORDER BY DATA,NUMREG";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = ConstruaAdaptador_MovFin();
                oledbda.SelectCommand = oledbcomm;
                oledbda.TableMappings.Add("Table", "MOV_FIN");
                oledbda.Fill(odataset);
                //     odataset.Tables[0].Columns.Add("CONTA", typeof(string));
                odataset.Tables[0].Columns.Add("CONTAFIN", typeof(string)).MaxLength = 2;
                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    if (tipo == "R")
                        odatarow["CONTAFIN"] = ((string)odatarow["DEBITO"]).Substring(0, 2);
                    else
                        odatarow["CONTAFIN"] = ((string)odatarow["CREDITO"]).Substring(0, 2);
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, oPesquisa); }
                                if (!passa)
                                {
                                    odatarow.Delete();
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            odatarow.Delete();
                        }
                    }


                }
                odataset.Tables[0].AcceptChanges();
                return oledbda;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }


        static public OleDbDataAdapter NovoMovFinanceiro(ref DataSet odataset, DateTime data1, DateTime data2, List<LinhaSolucao> oLista, string tipo, PesquisaGenerico oPesquisa)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            //  DataSet result = null;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT *  FROM " + path + "MOVFIN  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (TP_FIN = .T.) AND TIPO = '" + tipo + "'";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            setoroledb += " ORDER BY DATA,NUMREG";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = ConstruaAdaptador();
                oledbda.SelectCommand = oledbcomm;
                oledbda.TableMappings.Add("Table", "MOVIM");
                oledbda.Fill(odataset);
                //     odataset.Tables[0].Columns.Add("CONTA", typeof(string));
                odataset.Tables[0].Columns.Add("CONTAFIN", typeof(string)).MaxLength = 2;
                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    if (tipo == "R")
                        odatarow["CONTAFIN"] = ((string)odatarow["DEBITO"]).Substring(0, 2);
                    else
                        odatarow["CONTAFIN"] = ((string)odatarow["CREDITO"]).Substring(0, 2);
                    Boolean passa = true;
                    if (oLista != null)
                    {
                        try
                        {
                            for (int i2 = 0; i2 < oLista.Count; i2++)
                            {
                                if (oLista[i2].ofuncao != null)
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, oPesquisa); }
                                if (!passa)
                                {
                                    odatarow.Delete();
                                    break;
                                }
                            }
                        }
                        catch
                        {
                            odatarow.Delete();
                        }
                    }


                }
                odataset.Tables[0].AcceptChanges();
                return oledbda;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }

        // Reserva versão antiga
        /* static public DataSet MovFinanceiro(DateTime data1, DateTime data2, List<LinhaSolucao> oLista)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT * ,recno() as Numero FROM " + path + "MOVFIN  where (data between " +
               "CTOD("+ FormatDataGravar (data1.Date)+ ") AND " +
               "CTOD(" + FormatDataGravar(data2.Date)+")) AND (TP_FIN = .T.) ";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "MOVFIN");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                odataset.Tables[0].Columns.Add("CONTA", typeof(string));
                odataset.Tables[0].Columns.Add("CONTAFIN", typeof(string)).MaxLength = 2;
                
                DataTable tabDeb = odataset.Tables[0].Clone();
                DataTable tabCre = odataset.Tables[0].Clone();
                tabCre.TableName = "CREDITO";
                tabDeb.TableName = "DEBITO";
                //acrescenta dois campo para normalizar DEBITO E CREDITO = CONTA e CONTAFIN
                
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
                    //aqui pode entrar o delegate 
                    if (odatarow["TIPO"].ToString() == "R")
                    {
                        odatarow["CONTA"] = odatarow["CREDITO"];
                        odatarow["CONTAFIN"] = ((string)odatarow["DEBITO"]).Substring(0, 2);
                    
                        tabCre.Rows.Add(odatarow.ItemArray);
                    }
                    else
                    {
                        odatarow["CONTA"] = odatarow["DEBITO"];
                        odatarow["CONTAFIN"] = ((string)odatarow["CREDITO"]).Substring(0, 2);
                    
                        tabDeb.Rows.Add(odatarow.ItemArray);
                    }
                }
                result = new DataSet();
                result.Tables.Add(tabCre);
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            {
                throw;
             //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }
        */

        static public DataSet MovFinanVauches(DateTime data1, DateTime data2, List<LinhaSolucao> oLista, PesquisaGenerico oPesquisa)
        {

            // DataSet dsPesquisa = TDataControlReduzido.TabelaPlacon();
            // oBancos = TDataControlReduzido.Bancos();

            OleDbCommand oledbcomm;
            string setoroledb, path;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string campos = "";
            if (data1.Date.Year >= 2017)
            {
                campos = " data, valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, venc, doc_dupl, nhist," +
               " doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof, ";

            }
            else
            {

                campos = " data, ROUND(valor,2) valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, venc, doc_dupl, nhist," +
                    " doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof, ";
            }
            setoroledb = "SELECT "+campos+" RECNO() as nreg, 0 as novo_id FROM " + path + "MOVFIN  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (VALOR > 0)";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            setoroledb += " ORDER BY DATA,DOC";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "MOVFIN");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

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
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, oPesquisa); }
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
                //result.Tables.Add(tabCre);
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }


        static public DataSet MovFinanceiroFiltro(DateTime data1, DateTime data2, List<LinhaSolucao> oLista, PesquisaGenerico oPesquisa
            , int tipofinan, string PgRc)
        {

            // DataSet dsPesquisa = TDataControlReduzido.TabelaPlacon();
            // oBancos = TDataControlReduzido.Bancos();

            OleDbCommand oledbcomm;
            string setoroledb, path;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string campos = "";
                campos = " data, valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, venc, doc_dupl, nhist," +
               " doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof ";

            setoroledb = "SELECT " + campos + ",recno() AS NREG FROM " + path + "MOVFIN  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (VALOR > 0)" +
               " AND (TP_FIN ="+tipofinan.ToString()+" )" + " AND (TIPO = '"+PgRc+"') ";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            setoroledb += " ORDER BY DATA,DOC";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "MOVFIN");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

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
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, oPesquisa); }
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
                //result.Tables.Add(tabCre);
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }




        static public DataSet MovFinanVauches_Relaciona(string tabela,
            DateTime data1, DateTime data2,
            List<LinhaSolucao> oLista, 
            PesquisaGenerico oPesquisa)
        {

            // DataSet dsPesquisa = TDataControlReduzido.TabelaPlacon();
            // oBancos = TDataControlReduzido.Bancos();

            OleDbCommand oledbcomm;
            string setoroledb, path;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("CONTAB");
            
           /* setoroledb = "SELECT *,RECNO() as numero FROM " + path + tabela + "  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) ";
           */

            setoroledb = "SELECT data," +
             // " ROUND(valor - ((valor - `INT`(valor)) * 100 - `INT`((valor - `INT`(valor)) * 100)) / 100, 2) AS valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, " +
             " ROUND(valor,2) AS valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, " +
                " venc, doc_dupl, nhist, " +
                "doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof, RECNO() as numero "+
               " FROM " + path + tabela + "  where(data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) ";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            setoroledb += " ORDER BY DATA,DOC";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", tabela);
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();

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
                                { passa = oLista[i2].ofuncao(oLista[i2], odatarow, oPesquisa); }
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
                //result.Tables.Add(tabCre);
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }


        static public DataSet MovFinanceiro(DateTime data1, DateTime data2, List<LinhaSolucao> oLista, bool verde)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            DataSet result = null;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string entradas = "SELECT entradas.*,placon.numconta,placon.desc2 FROM " + path + "MOVFIN AS ENTRADAS " +
                "LEFT OUTER JOIN " + path + "placon on (entradas.credito = placon.desc2)"
                + " where (entradas.data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (TP_FIN = .T.) " +
               " AND (TIPO = 'R')";
            string saidas = "SELECT saidas.*,placon.numconta,placon.desc2 FROM " + path + "MOVFIN AS SAIDAS " +
                "LEFT OUTER JOIN " + path + "placon on (saidas.debito = placon.desc2)"
                + " where (saidas.data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (TP_FIN = .T.) " +
               " AND (TIPO = 'P')";

            setoroledb = "";

            if (oLista != null)
            {
                for (int i = 0; i < oLista.Count; i++)
                {
                    if (oLista[i].ofuncao == null)
                    {
                        if (oLista[i].ofuncaoSql != null)
                        {
                            setoroledb += oLista[i].ofuncaoSql(oLista[i]);
                        }
                        else
                            setoroledb += TDataControlReduzido.ConstruaSql(oLista[i].campo, oLista[i].dado);
                    }
                }
            }
            try
            {
                entradas += setoroledb + ";";
                saidas += setoroledb + ";";
                setoroledb = entradas;
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "ENTRADAS");
                //oledbda.TableMappings.Add("Table1", "SAIDAS");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                setoroledb = saidas;
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "SAIDAS");
                oledbda.Fill(odataset);
                oledbda.Dispose();
                odataset.Tables[0].Columns.Add("CONTA", typeof(string));
                odataset.Tables[0].Columns.Add("CONTAFIN", typeof(string)).MaxLength = 2;
                odataset.Tables[1].Columns.Add("CONTA", typeof(string));
                odataset.Tables[1].Columns.Add("CONTAFIN", typeof(string)).MaxLength = 2;

                DataTable tabDeb = odataset.Tables[0].Clone();
                DataTable tabCre = odataset.Tables[1].Clone();
                tabCre.TableName = "CREDITO";
                tabDeb.TableName = "DEBITO";
                //acrescenta dois campo para normalizar DEBITO E CREDITO = CONTA e CONTAFIN


                for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[0].Rows[i];
                    Boolean passa = true;

                    if (oLista != null)
                    {
                        for (int i2 = 0; i2 < oLista.Count; i2++)
                        {
                            if (oLista[i2].ofuncao != null)
                            { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                            if (!passa) break;
                        }
                    }

                    if (!passa) continue;
                    odatarow["CONTA"] = odatarow["CREDITO"];
                    odatarow["CONTAFIN"] = ((string)odatarow["DEBITO"]).Substring(0, 2);
                    tabCre.Rows.Add(odatarow.ItemArray);

                }
                for (int i = 0; i < odataset.Tables[1].Rows.Count; i++)
                {
                    DataRow odatarow = odataset.Tables[1].Rows[i];
                    Boolean passa = true;
                    if (oLista != null)
                    {

                        for (int i2 = 0; i2 < oLista.Count; i2++)
                        {
                            if (oLista[i2].ofuncao != null)
                            { passa = oLista[i2].ofuncao(oLista[i2], odatarow, null); }
                            if (!passa) break;
                        }
                    }
                    if (!passa) continue;
                    odatarow["CONTA"] = odatarow["DEBITO"];
                    odatarow["CONTAFIN"] = ((string)odatarow["CREDITO"]).Substring(0, 2);
                    tabDeb.Rows.Add(odatarow.ItemArray);
                }

                result = new DataSet();
                result.Tables.Add(tabCre);
                result.Tables.Add(tabDeb);
                return result;
            }
            catch (Exception)
            {
                throw;
                //   throw new Exception(string.Format("Não Foi Possivel acessar a tabela MoVFIN no caminho:", path));
            }
        }
        static public DataSet TabelaPlacon()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            //, descricao, sdo, ant_sdo, data, data_ant, novocod, novadesc 
            setoroledb = "SELECT distinct desc2,numconta,descricao FROM "
                + path + "placon.DBF WHERE        (SUBS(numconta, 6, 3) > '000') AND (DESC2 <> '') " +
                       " ORDER BY DESC2";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PLACON");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataTable result = odataset.Tables[0];
            string anterior = "";
            for (int i = 0; i < result.Rows.Count; i++)
            {
                if (anterior == Convert.ToString(result.Rows[i]["DESC2"]).Trim())
                    result.Rows[i].Delete();
                else
                    anterior = Convert.ToString(result.Rows[i]["DESC2"]).Trim();
            }
            result.AcceptChanges();

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = result.Columns["DESC2"];
            result.PrimaryKey = PrimaryKeyColumns;

            // primary
            return odataset;
        }
        static public DataSet TabelaPlaconAno(string ano)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            //, descricao, sdo, ant_sdo, data, data_ant, novocod, novadesc 
            setoroledb = "SELECT distinct desc2,numconta,descricao FROM "
                + path + "PTPLA"+ano+".DBF WHERE        (SUBS(numconta, 6, 3) > '000') AND (DESC2 <> '') " +
                       " ORDER BY DESC2";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PTPLA"+ano);
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataTable result = odataset.Tables[0];
            string anterior = "";
            for (int i = 0; i < result.Rows.Count; i++)
            {
                if (anterior == Convert.ToString(result.Rows[i]["DESC2"]).Trim())
                    result.Rows[i].Delete();
                else
                    anterior = Convert.ToString(result.Rows[i]["DESC2"]).Trim();
            }
            result.AcceptChanges();

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = result.Columns["DESC2"];
            result.PrimaryKey = PrimaryKeyColumns;

            // primary
            return odataset;
        }

        static public DataView Placon()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT * FROM " + path + "placon.DBF WHERE        (SUBS(numconta, 6, 3) > '000')" +
                       " ORDER BY DESC2";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PLACON");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataView result = odataset.Tables[0].AsDataView();
            odataset.Dispose();
            return result;
        }
        static public DataTable PlaconAlterado()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT ALLTRIM(NUMCONTA) as NUMCONTA, DESC2, DESCRICAO, ALLTRIM(NUMCONTA+ '    ' + DESCRICAO) as NUMCONTA_DESCRICAO, ALLTRIM(ALLTRIM(DESC2)+ '    ' + DESCRICAO) " +
                "as DESC2_DESCRICAO  FROM " + path + "placon.DBF WHERE        (SUBS(numconta, 6, 3) > '000')" +
                       " ORDER BY DESC2";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PLACON");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataTable result = odataset.Tables[0];
            odataset.Dispose();
            return result;
        }

        static public DataTable dtPlacon()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT * FROM " + path + "placon.DBF WHERE        (SUBS(numconta, 6, 3) > '000')" +
                       " ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PLACON");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataTable result = odataset.Tables[0];
            odataset.Dispose();
            return result;
        }

        static public DataTable dtPlaconAno(string ano)
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT * FROM " + path + "ptpla"+ano+".DBF WHERE        (SUBS(numconta, 6, 3) > '000')" +
                       " ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PTPLA"+ano);
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataTable result = odataset.Tables[0];
            odataset.Dispose();
            return result;
        }


        static public DataView Bancos()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT nbanco, nome_banco, saldo, desc2, contab, data_ult FROM " + path + "Bancos ORDER BY NBANCO";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "BANCOS");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataView result = odataset.Tables[0].AsDataView();
            odataset.Dispose();
            return result;
        }

        static public DataTable BancosContab()
        {
            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            setoroledb = "SELECT nbanco, nome_banco, saldo, desc2, contab, data_ult FROM " + path + "Bancos ORDER BY DESC2";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "BANCOS");
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            DataTable result = odataset.Tables[0];
            odataset.Dispose();
            return result;
        }
        
        public static class FindDiretorio
        {
            public static IEnumerable<FileInfo> TraverseDirectory(string rootPath, Func<FileInfo, bool> Pattern)
            {
                var directoryStack = new Stack<DirectoryInfo>();
                directoryStack.Push(new DirectoryInfo(rootPath));
                while (directoryStack.Count > 0)
                {
                    var dir = directoryStack.Pop();
                    try
                    {
                        foreach (var i in dir.GetDirectories())
                            directoryStack.Push(i);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue; // We don't have access to this directory, so skip it
                    }
                    foreach (var f in dir.GetFiles().Where(Pattern)) // "Pattern" is a function
                        yield return f;
                }
            }

        }
    }
}
