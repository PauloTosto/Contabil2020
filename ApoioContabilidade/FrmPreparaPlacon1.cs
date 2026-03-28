using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.Odbc;
using System.IO;
using ClassConexao;
using System.Data.OleDb;
using System.CodeDom;

namespace ApoioContabilidade
{
    public partial class FrmPreparaPlacon1 : Form
    {
        DataTable tabexcel;
        DataSet odataset;
        OleDbDataAdapter odbcda;
        OleDbDataAdapter adrelaciona;
        DataSet dsrelaciona;
        public FrmPreparaPlacon1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (CheckPlacon())
            {
                if (!((tabexcel != null) && (tabexcel.Columns.Contains("NUMCONTA"))))
                    ImporteExcel();




            }
            //}
            //  if (checkedListBox1.CheckedIndices.Contains(1) )
            //    checkedListBox1.SetItemChecked(1, false);
        }
        private Boolean CheckPlacon()
        {
            try
            {
                odbcda = TDataControlContab.AdapterGet_Placon();
                odbcda.RowUpdating += new OleDbRowUpdatingEventHandler(odbcda_RowUpdating);
                odbcda.TableMappings.Add("Table", "PLACON");
                odataset = new DataSet();
                odbcda.Fill(odataset);

              
                for (int i = 0; i < odataset.Tables["PLACON"].Rows.Count; i++)
                {
                    if ((Convert.IsDBNull(odataset.Tables["PLACON"].Rows[i]["NUMCONTA"])) || (Convert.ToString(odataset.Tables["PLACON"].Rows[i]["NUMCONTA"]).Trim() == ""))
                        odataset.Tables["PLACON"].Rows[i].Delete();

                }
                odbcda.Update(odataset.Tables["PLACON"]);
                odataset.Tables["PLACON"].AcceptChanges();

                Form oform = new Form();
                DataGrid oview = new DataGrid();
                oview.DataSource = odataset.Tables["PLACON"];
                oview.Parent = oform;

                // for (int i = 0;i<oview.TableStyles[0].GridColumnStyles.Count;i++)
                //    oview.TableStyles[0].GridColumnStyles[i].NullText = ""; 
                oview.Dock = DockStyle.Fill;
                oform.Show();


                /* int i = 0;
                 while (i < odataset.Tables["PLACON"].Rows.Count)
                 {
                     if (odataset.Tables["PLACON"].Rows[i].RowState == DataRowState.Deleted)
                         odataset.Tables["PLACON"].Rows[i].Re
                 }*/

                DataRow[] lista = odataset.Tables["PLACON"].Select("", "NUMCONTA");
                int tam = lista.GetLength(0);
                string anterior = "";
                for (int i = 0; i < tam; i++)
                {
                    if (Convert.ToString(lista[i]["NUMCONTA"]).Trim() == anterior)
                    {
                        MessageBox.Show(anterior);
                        lista[i].BeginEdit();
                        lista[i]["NOVOCOD"] = lista[i]["NUMCONTA"];
                        lista[i]["NUMCONTA"] = "9" + anterior.Substring(1, 7);
                        lista[i].EndEdit();
                    }
                    anterior = Convert.ToString(lista[i]["NUMCONTA"]).Trim();
                }

                odbcda.Update(lista);
                odataset.Tables["PLACON"].AcceptChanges();
                //select valor1.Field<decimal>(LinhasCampo[i].titulo)).Sum();
                //LinhasCampo[i].total = Convert.ToDouble(total1);




                DataColumn[] PrimaryKeyColumns = new DataColumn[1];
                PrimaryKeyColumns[0] = odataset.Tables["PLACON"].Columns["NUMCONTA"];
                odataset.Tables["PLACON"].PrimaryKey = PrimaryKeyColumns;

            }
            catch (Exception)
            {
                throw;
            }

            return true;

        }

        void odbcda_RowUpdating(object sender, OleDbRowUpdatingEventArgs e)
        {

            //  throw new NotImplementedException();
        }
        private void ImporteExcel()
        {

            button2.PerformClick();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {

                    string nomeplanilha = TDataControlReduzido.ExcelNomePlanilha(openFileDialog1.FileName);
                    tabexcel = TDataControlReduzido.LeiaExcel(openFileDialog1.FileName, nomeplanilha);
                    Form oform = new Form();
                    DataGrid oview = new DataGrid();
                    if (nomeplanilha.Trim().ToUpper() != "PESQUISA")
                    {
                        tabexcel.AsEnumerable().ToList().ForEach(
                                            row =>
                                            {
                                                row.BeginEdit();
                                                row["NOVADESC"] = row["NOVADESC"].ToString().Substring(1);
                                                row.EndEdit();
                                            });
                        tabexcel.AcceptChanges();
                    }
                    // oview
                    bool mostraRelaciona = false;
                    if (nomeplanilha.Trim().ToUpper() == "PESQUISA") // Dentro do arquivo excel tem que ter a planilha com o nome PESQUISA para gravar o relaciona definitivo 
                    {

                        mostraRelaciona = (MessageBox.Show("Cria novo RELACIONA.DBF sobreescrevendo atual.Importa os dados com base na planilha EXCEL <PESQUISA>?", "Responda", MessageBoxButtons.OKCancel) == DialogResult.OK);
                        bool retorno = ImportaRelaciona_Excel();
                        if (!retorno) { MessageBox.Show("Problemas com a importação!Verifique campos necessários no Excel!.");
                            return;
                        }
                    }
                    if (mostraRelaciona)
                    {
                        oview.DataSource = dsrelaciona.Tables["RELACIONA"];
                        oform.Text = "RELACIONA => Criado pelo EXCEL";

                    }
                    else
                    {
                        oview.DataSource = tabexcel;
                        oform.Text = "EXCEL DIRETO";
                    }
                    // for (int i = 0;i<oview.TableStyles[0].GridColumnStyles.Count;i++)
                    //    oview.TableStyles[0].GridColumnStyles[i].NullText = ""; 
                    oview.Parent = oform;
                    oview.Dock = DockStyle.Fill;
                    oform.Show();

                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private bool Compara(string placon, string nova)
        {
            bool retorno = false;
            retorno = ((placon.ToUpper().Trim().CompareTo(nova.ToUpper().Trim())) == 0);
            /*    int vezes = 3;
                int inicio = 0;
                int tam = nova.ToUpper().Trim().Length;
                int cadeia = 10;

                while ((!retorno) && (vezes >0))
                {
                    if ((inicio + cadeia) > tam) { vezes = 0; continue; } 
                    retorno = placon.ToUpper().Trim().Contains(nova.ToUpper().Trim().Substring(inicio,cadeia ));
                    inicio += 10;
                    vezes -= 1;
                }*/
            return retorno;
        }
        private Boolean ConciliePlacon_Excel()
        {
            if (!((tabexcel != null) && (tabexcel.Columns.Contains("NUMCONTA")) && (tabexcel.Columns.Contains("NOVOCOD")) && (tabexcel.Columns.Contains("NOVADESC"))))
            {
                MessageBox.Show("Abra Excel com dados e nome_de_campos corretos");
                return false;
            }
            if (!odataset.Tables.Contains("PLACON"))
            {
                MessageBox.Show("Abra Placon");
                return false;
            }

            adrelaciona = TDataControlContab.CreateTabRelaciona();
            //odbcda.RowUpdating += new OdbcRowUpdatingEventHandler(odbcda_RowUpdating); 
            adrelaciona.TableMappings.Add("Table", "RELACIONA");
            dsrelaciona = new DataSet();
            adrelaciona.Fill(dsrelaciona);
            //DataTable planorder = odataset.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("GRAU") == "5" ? row.Field<decimal>("SDO") != 0 : true).OrderBy(row => row.Field<string>("numconta")).CopyToDataTable();
            //DataTable planorder = odataset.Tables["PLACON"].AsEnumerable().OrderBy(row => row.Field<string>("numconta")).CopyToDataTable();
            try
            {

                int nreg = 0;
                try
                {
                    nreg = dsrelaciona.Tables["RELACIONA"].AsEnumerable().Max(row => row.Field<int>("NREG"));
                }
                catch (Exception)
                {
                }
                List<string> listaAchados = new List<string>();
                // int order = 1;
                foreach (DataRow linexc in tabexcel.AsEnumerable().OrderBy(row => row.Field<string>("novocod")))
                {
                    //DataRow linexc = tabexcel.Rows[i];
                    if ((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == ""))
                        continue;
                    DataRow linhaplacon = null;
                    if (linexc["NOVOCOD"].ToString().Length == 14) // desprezando nivel 2 do novo
                    {
                        var rows = odataset.Tables["PLACON"].AsEnumerable().Where(row => (

                            row.Field<string>("NUMCONTA").Substring(5, 3) != "000") &&
                            (!listaAchados.Contains(row.Field<string>("NUMCONTA").ToString())) &&
                            (linexc["NOVOCOD"].ToString().Substring(0, 1) == row.Field<string>("NUMCONTA").Substring(0, 1)) &&
                           Compara(row.Field<string>("DESCRICAO"), linexc["NOVADESC"].ToString()));
                        if (rows.Count() > 0)
                        {
                            linhaplacon = rows.FirstOrDefault();
                            listaAchados.Add(linhaplacon["NUMCONTA"].ToString());
                        }
                        /*if (order < odataset.Tables["PLACON"].Rows.Count)
                        {

                            linhaplacon = planorder.Rows[order];
                            order += 1;
                        }*/
                    }
                    DataRow linharelaciona = dsrelaciona.Tables["RELACIONA"].NewRow();

                    if (!((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == "")))
                        linharelaciona["NOVOCOD"] = linexc["NOVOCOD"];
                    else
                        linharelaciona["NOVOCOD"] = "";

                    if (!((Convert.IsDBNull(linexc["NOVADESC"])) || (Convert.ToString(linexc["NOVADESC"]).Trim() == "")))
                        linharelaciona["NOVADESC"] = linexc["NOVADESC"];
                    else
                        linharelaciona["NOVADESC"] = "";
                    if (linhaplacon != null)
                    {
                        if (!((Convert.IsDBNull(linhaplacon["NUMCONTA"])) || (Convert.ToString(linhaplacon["NUMCONTA"]).Trim() == "")))
                            linharelaciona["NUMCONTA"] = linhaplacon["NUMCONTA"];
                        else
                            linharelaciona["NUMCONTA"] = "";

                        if (!((Convert.IsDBNull(linhaplacon["DESCRICAO"])) || (Convert.ToString(linhaplacon["DESCRICAO"]).Trim() == "")))
                            linharelaciona["DESCRICAO"] = linhaplacon["DESCRICAO"];
                        else
                            linharelaciona["DESCRICAO"] = "";
                    }
                    else
                    {
                        linharelaciona["DESCRICAO"] = "";
                        linharelaciona["NUMCONTA"] = "";

                    }
                    nreg++;
                    linharelaciona["NREG"] = nreg;
                    dsrelaciona.Tables["RELACIONA"].Rows.Add(linharelaciona);
                }
                adrelaciona.Update(dsrelaciona.Tables["RELACIONA"]);
                dsrelaciona.Tables["RELACIONA"].AcceptChanges();

                Form oform = new Form();
                DataGrid oview = new DataGrid();
                oview.DataSource = dsrelaciona.Tables["RELACIONA"];
                oview.Parent = oform;
                oform.Text = "RELACIONA COM PLACON e EXCEL";
                oview.Dock = DockStyle.Fill;
                oform.Show();


            }
            catch (Exception E)
            {
                throw;
            }

            return true;
        }


        //

        private Boolean ImportaRelaciona_Excel()
        {
            bool result = false;
            if (!((tabexcel != null) && (tabexcel.Columns.Contains("NUMCONTA")) && (tabexcel.Columns.Contains("NOVOCOD")) && (tabexcel.Columns.Contains("NOVADESC")) 
                &&  (tabexcel.Columns.Contains("REDUZIDO"))))
            {
                MessageBox.Show("Abra Excel com dados e nome_de_campos corretos");
                return result;
            }
            /*if (!odataset.Tables.Contains("PLACON"))
            {
                MessageBox.Show("Abra Placon");
                return false;
            }*/

            result = true;
            try
            {
                bool deletou = TDataControlContab.DeleteTabRelaciona();
                adrelaciona = TDataControlContab.CreateTabRelaciona();

                adrelaciona.TableMappings.Add("Table", "RELACIONA");
                dsrelaciona = new DataSet();
                adrelaciona.Fill(dsrelaciona);
                int nreg = 0;

                // int order = 1;
                foreach (DataRow linexc in tabexcel.AsEnumerable().OrderBy(row => row.Field<string>("novocod")))
                {
                    //DataRow linexc = tabexcel.Rows[i];
                    if ((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == ""))
                        continue;
                    DataRow linharelaciona = dsrelaciona.Tables["RELACIONA"].NewRow();

                    if (!((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == "")))
                        linharelaciona["NOVOCOD"] = linexc["NOVOCOD"];
                    else
                        linharelaciona["NOVOCOD"] = "";

                    if (!((Convert.IsDBNull(linexc["NOVADESC"])) || (Convert.ToString(linexc["NOVADESC"]).Trim() == "")))
                        linharelaciona["NOVADESC"] = linexc["NOVADESC"];
                    else
                        linharelaciona["NOVADESC"] = "";

                    if (!((Convert.IsDBNull(linexc["NUMCONTA"])) || (Convert.ToString(linexc["NUMCONTA"]).Trim() == "")))
                        linharelaciona["NUMCONTA"] = linexc["NUMCONTA"];
                    else
                        linharelaciona["NUMCONTA"] = "";
                   
                    if (!((Convert.IsDBNull(linexc["DESCRICAO"])) || (Convert.ToString(linexc["DESCRICAO"]).Trim() == "")))
                        linharelaciona["DESCRICAO"] = linexc["DESCRICAO"];
                    else
                        linharelaciona["DESCRICAO"] = "";

                    if (!(Convert.IsDBNull(linexc["REDUZIDO"]))) 
                        linharelaciona["REDUZIDO"] = Convert.ToInt32(linexc["REDUZIDO"]);
                    else
                        linharelaciona["REDUZIDO"] = 0;

                    nreg++;
                    linharelaciona["NREG"] = nreg;
                    dsrelaciona.Tables["RELACIONA"].Rows.Add(linharelaciona);
                }
                adrelaciona.Update(dsrelaciona.Tables["RELACIONA"]);
                dsrelaciona.Tables["RELACIONA"].AcceptChanges();

               

            }
            catch (Exception E)
            {
                result = false;
                
                
            }

            return true;
        }



        private Boolean GraveRelaciona_Excel()
        {
            if (!((tabexcel != null) && (tabexcel.Columns.Contains("NUMCONTA")) && (tabexcel.Columns.Contains("NOVOCOD")) && (tabexcel.Columns.Contains("NOVADESC"))))
            {
                MessageBox.Show("Abra Excel com dados e nome_de_campos corretos");
                return false;
            }
            if (!odataset.Tables.Contains("PLACON"))
            {
                MessageBox.Show("Abra Placon");
                return false;
            }
            
            adrelaciona = TDataControlContab.CreateTabRelaciona();
            //odbcda.RowUpdating += new OdbcRowUpdatingEventHandler(odbcda_RowUpdating); 
            adrelaciona.TableMappings.Add("Table", "RELACIONA");
            dsrelaciona = new DataSet();
            adrelaciona.Fill(dsrelaciona);
            //DataTable planorder = odataset.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("GRAU") == "5" ? row.Field<decimal>("SDO") != 0 : true).OrderBy(row => row.Field<string>("numconta")).CopyToDataTable();
            //DataTable planorder = odataset.Tables["PLACON"].AsEnumerable().OrderBy(row => row.Field<string>("numconta")).CopyToDataTable();
            int nreg = 0;
            try
            {

                // int order = 1;
                foreach (DataRow linexc in tabexcel.AsEnumerable().OrderBy(row => row.Field<string>("novocod")))
                {
                    //DataRow linexc = tabexcel.Rows[i];
                    if ((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == ""))
                        continue;
                    DataRow linharelaciona = dsrelaciona.Tables["RELACIONA"].NewRow();

                    if (!((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == "")))
                        linharelaciona["NOVOCOD"] = linexc["NOVOCOD"];
                    else
                        linharelaciona["NOVOCOD"] = "";

                    if (!((Convert.IsDBNull(linexc["NOVADESC"])) || (Convert.ToString(linexc["NOVADESC"]).Trim() == "")))
                        linharelaciona["NOVADESC"] = linexc["NOVADESC"];
                    else
                        linharelaciona["NOVADESC"] = "";

                    if (!((Convert.IsDBNull(linexc["NUMCONTA"])) || (Convert.ToString(linexc["NUMCONTA"]).Trim() == "")))
                        linharelaciona["NUMCONTA"] = linexc["NUMCONTA"];
                    else
                        linharelaciona["NUMCONTA"] = "";
                    if (!((Convert.IsDBNull(linexc["DESCRICAO"])) || (Convert.ToString(linexc["DESCRICAO"]).Trim() == "")))
                        linharelaciona["DESCRICAO"] = linexc["DESCRICAO"];
                    else
                        linharelaciona["NUMCONTA"] = "";

                    nreg++;
                    linharelaciona["NREG"] = nreg;
                    dsrelaciona.Tables["RELACIONA"].Rows.Add(linharelaciona);
                }
                adrelaciona.Update(dsrelaciona.Tables["RELACIONA"]);
                dsrelaciona.Tables["RELACIONA"].AcceptChanges();

                /* Form oform = new Form();
                 DataGrid oview = new DataGrid();
                 oview.DataSource = dsrelaciona.Tables["RELACIONA"];
                 oview.Parent = oform;
                 oform.Text = "RELACIONA COM PLACON e EXCEL";
                 oview.Dock = DockStyle.Fill;
                 oform.Show();
                */

            }
            catch (Exception E)
            {
                throw;
            }

            return true;
        }








        private void button3_Click(object sender, EventArgs e)
        {
            TDataControlContab.DropTabela();
            ConciliePlacon_Excel();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ExportePlaconFormatoText();
        }


        private string preenchaespaco(string campo, int total)
        {
            string resultado = "";
            resultado = campo;
            while (resultado.Length <= total)
                resultado += " ";
            return resultado;

        }

        private string FormateNumeroCodigo(string conta)
        {
            try
            {
                if ((conta.Length == 0) || (conta.Length > 9) || (conta.Length == 7) || (conta.Length == 8) || (conta.Length == 5)) return "";
                if ((conta.Length == 9) && (conta.Substring(6, 3) == "000"))
                    conta = conta.Substring(0, 6);
                if ((conta.Length == 6) && (conta.Substring(4, 2) == "00"))
                    conta = conta.Substring(0, 4);
                if ((conta.Length == 4) && (conta.Substring(3, 1) == "0"))
                    conta = conta.Substring(0, 3);
                if ((conta.Length == 3) && (conta.Substring(2, 1) == "0"))
                    conta = conta.Substring(0, 2);
                if ((conta.Length == 2) && (conta.Substring(1, 1) == "0"))
                    conta = conta.Substring(0, 1);
                if ((conta.Length == 1) && (conta.Substring(0, 1) == "0"))
                    return conta = "";
                string resultado = conta.Substring(0, 1);
                int passo = 1;
                for (int i = 1; i < conta.Length; i += passo)
                {
                    if (i > 6) break;
                    if (i == 4) passo = 2;
                    if (i == 6) passo = 3;
                    resultado = resultado + "." + conta.Substring(i, passo);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private string NovoFormateNumeroCodigo(string conta)
        {
            try
            {
                //if ((conta.Length == 0) || (conta.Length > 9) || (conta.Length == 7) || (conta.Length == 8) || (conta.Length == 5)) return "";
                if ((conta.Length == 0) || (conta.Length > 8)) return "";
                if ((conta.Length == 8) && (conta.Substring(5, 3) == "000"))
                    conta = conta.Substring(0, 5);
                if ((conta.Length == 5) && (conta.Substring(3, 2) == "00"))
                    conta = conta.Substring(0, 3);
                if ((conta.Length == 3) && (conta.Substring(2, 1) == "0"))
                    conta = conta.Substring(0, 2);
                if ((conta.Length == 2) && (conta.Substring(1, 1) == "0"))
                    conta = conta.Substring(0, 1);
                if ((conta.Length == 1) && (conta.Substring(0, 1) == "0"))
                    return conta = "";
                string resultado = conta.Substring(0, 1);
                int passo = 1;
                for (int i = 1; i < conta.Length; i += passo)
                {
                    if (i > 5) break;
                    if (i == 2) passo = 1;
                    if (i == 5) passo = 2;
                    resultado = resultado + "." + conta.Substring(i, passo);
                }
                return resultado;
            }
            catch (Exception)
            {
                throw;
            }
        }




        private void ExportePlaconFormatoText()
        {
            if ((dsrelaciona == null) ||
                (!dsrelaciona.Tables.Contains("RELACIONA")))
            {
                adrelaciona = TDataControlContab.CreateTabRelaciona();
                //odbcda.RowUpdating += new OdbcRowUpdatingEventHandler(odbcda_RowUpdating); 
                adrelaciona.TableMappings.Add("Table", "RELACIONA");
                dsrelaciona = new DataSet();
                adrelaciona.Fill(dsrelaciona);


            }


            saveFileDialog1.FileName = "PlanoImportado.txt";
            if (saveFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string tarquivo = saveFileDialog1.FileName;
                //if (File.Exists(tarquivo))
                //{
                //   Console.WriteLine(" Já EXiste");
                //  return;
                // }

                //  using (StreamWriter sw = File.CreateText(tarquivo))
                // {
                //    sw.WriteLine("This is my file.");
                //   sw.WriteLine("I can write ints {0} or floats {1}, and so on.",
                //      1, 4.2);
                // sw.Close();
                // }


                DataRow[] tabordenada = dsrelaciona.Tables["RELACIONA"].Select("NOVOCOD <> ''", "NOVOCOD");

                try
                {
                    StreamWriter sw = File.CreateText(tarquivo);
                    string codant = "";
                    for (int i = 0; i < tabordenada.Length; i++)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                    {

                        DataRow linexc = tabordenada[i];// dsrelaciona.Tables["RELACIONA"].Rows[i];
                        if (Convert.ToString(linexc["NOVOCOD"]).Trim() == codant) continue;
                        codant = Convert.ToString(linexc["NOVOCOD"]).Trim();
                        if (((Convert.IsDBNull(linexc["NUMCONTA"])) || (Convert.ToString(linexc["NUMCONTA"]).Trim() == ""))
                            && ((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == "")))
                            continue;
                        string linha = "";
                        string conta = FormateNumeroCodigo(Convert.ToString(linexc["NOVOCOD"]).Trim());
                        if (conta.Trim() == "") continue;
                        linha += preenchaespaco(conta, 69);
                        if ((Convert.IsDBNull(linexc["NUMCONTA"])) || (Convert.ToString(linexc["NUMCONTA"]).Trim() == ""))
                            linha += "S";
                        else
                        {
                            if (Convert.ToString(linexc["NUMCONTA"]).Substring(5, 3) == "000")
                                linha += "S";
                            else
                                linha += "A";
                        }
                        if (Convert.ToString(linexc["NOVADESC"]).Trim() != "")
                            linha += preenchaespaco(Convert.ToString(linexc["NOVADESC"]), 50);
                        else
                            linha += preenchaespaco(Convert.ToString(linexc["DESCRICAO"]), 50);
                        linha += preenchaespaco(" MNN", 32);
                        linha += "0000000000N          0000000,00" + "                                                                    NN0000000000                              00000000000           0         ";
                        sw.WriteLine(linha);

                    }
                    sw.Close();
                }




                /*
                 * private const string FILE_NAME = "MyFile.txt";
   public static void Main(String[] args) 
   {
       if (File.Exists(FILE_NAME)) 
       {
           Console.WriteLine("{0} already exists.", FILE_NAME);
           return;
       }
       using (StreamWriter sw = File.CreateText(FILE_NAME))
       {
           sw.WriteLine ("This is my file.");
           sw.WriteLine ("I can write ints {0} or floats {1}, and so on.", 
               1, 4.2);
           sw.Close();
       }
   }

                 */
                catch (Exception)
                {
                    throw;
                }
            }
            /*if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string nomeplanilha = openFileDialog1.SafeFileName;
                    nomeplanilha = nomeplanilha.Remove(nomeplanilha.IndexOf("."));
                    DataTable tabtexto = TDataControlReduzido.LeiaTexto(openFileDialog1.FileName, nomeplanilha);
                    Form oform = new Form();
                    DataGrid oview = new DataGrid();
                    oview.DataSource = tabtexto;
                    oview.Parent = oform;

                    // for (int i = 0;i<oview.TableStyles[0].GridColumnStyles.Count;i++)
                    //    oview.TableStyles[0].GridColumnStyles[i].NullText = ""; 
                    oview.Dock = DockStyle.Fill;
                    oform.Show();


                }
                catch (Exception)
                {
                    throw;
                }
            }*/

        }

        private void button5_Click(object sender, EventArgs e)
        {
            adrelaciona = TDataControlContab.CreateTabRelaciona();
            //odbcda.RowUpdating += new OdbcRowUpdatingEventHandler(odbcda_RowUpdating); 
            adrelaciona.TableMappings.Add("Table", "RELACIONA");
            dsrelaciona = new DataSet();
            adrelaciona.Fill(dsrelaciona);
            try
            {

                foreach (DataRow linhaplacon in dsrelaciona.Tables["RELACIONA"].Rows)
                // foreach (DataRow linhaplacon in odataset.Tables["PLACON"].Rows)
                //foreach (DataRow linhaplacon in tabexcel.Rows)
                {

                    //  DataRow linexc = tabexcel.Rows[i];
                    //if ((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == ""))
                    //  continue;
                    // DataRow linhaplacon = odataset.Tables["PLACON"].Rows.Find(Convert.ToString(linexc["NUMCONTA"]));
                    // DataRow linharelaciona = dsrelaciona.Tables["RELACIONA"].NewRow();

                    //if (!((Convert.IsDBNull(linexc["NOVOCOD"])) || (Convert.ToString(linexc["NOVOCOD"]).Trim() == "")))
                    //  linharelaciona["NOVOCOD"] = linexc["NOVOCOD"];
                    //else
                    //  linharelaciona["NOVOCOD"] = "";

                    // if (!((Convert.IsDBNull(linexc["NOVADESC"])) || (Convert.ToString(linexc["NOVADESC"]).Trim() == "")))
                    //   linharelaciona["NOVADESC"] = linexc["NOVADESC"];
                    // else
                    //   linharelaciona["NOVADESC"] = "";
                    string Descricao = "NOVADESC";
                    string contaMLSA = "NUMCONTA";
                    string contaOrg = "NOVOCOD";
                    // linhaplacon.BeginEdit();
                    if (!((Convert.IsDBNull(linhaplacon[contaMLSA])) || (Convert.ToString(linhaplacon[contaMLSA]).Trim() == "")))
                    {
                        //linhaplacon[contaMLSA] = linhaplacon[contaMLSA].ToString();
                        linhaplacon[contaOrg] = NovoFormateNumeroCodigo(linhaplacon[contaMLSA].ToString());
                    }


                    if (!((Convert.IsDBNull(linhaplacon["DESCRICAO"])) || (Convert.ToString(linhaplacon["DESCRICAO"]).Trim() == "")))
                    {
                        linhaplacon[Descricao] = linhaplacon["DESCRICAO"];
                    }
                    else
                        linhaplacon[Descricao] = "";
                    // linhaplacon.EndEdit();
                    // dsrelaciona.Tables["RELACIONA"].Rows.Add(linharelaciona);
                }
                adrelaciona.Update(dsrelaciona.Tables["RELACIONA"]);
                dsrelaciona.Tables["RELACIONA"].AcceptChanges();

                Form oform = new Form();
                DataGrid oview = new DataGrid();
                oview.DataSource = dsrelaciona.Tables["RELACIONA"];
                oview.Parent = oform;
                oform.Text = "RELACIONA COM PLACON";
                oview.Dock = DockStyle.Fill;
                oform.Show();


            }
            catch (Exception)
            {
                throw;
            }

            return;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            AltereRelaciona_Excel();
        }

        private void AltereRelaciona_Excel()
        {
            DataTable tabexcelAltera;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {

                    string nomeplanilha = TDataControlReduzido.ExcelNomePlanilha(openFileDialog1.FileName);
                    // string query = " select NUMCONTA, SUBSTRING(DESCRICAO,2,45) as DESCRICAO " +
                    //" from " + "`" + nomeplanilha + "$`";
                    tabexcelAltera = TDataControlReduzido.LeiaExcel(openFileDialog1.FileName, nomeplanilha);
                    Form oform = new Form();
                    DataGrid oview = new DataGrid();
                    if (nomeplanilha.Trim().ToUpper() == "PLANO ALTERDATA")
                    {
                        tabexcelAltera.AsEnumerable().ToList().ForEach(
                                            row =>
                                            {
                                                row.BeginEdit();
                                                row["NOVADESC"] = row["NOVADESC"].ToString().Substring(1);
                                                row.EndEdit();
                                            });
                        tabexcelAltera.AcceptChanges();
                    }
                    else
                    {
                        MessageBox.Show("ESPECIFICO PARA RECRIAR O RELACIONA ACRESCENTANDO AO RELACIONA O CAMPO 'REDUZIDO' ");
                        return;
                    }
                    DataSet dsRelacionaVelho = TDataControlContab.TabRelacionaVelho();
                    DataTable oVelho = dsRelacionaVelho.Tables[0];
                    // oview

                    adrelaciona = TDataControlContab.CreateTabRelaciona();
                    //odbcda.RowUpdating += new OdbcRowUpdatingEventHandler(odbcda_RowUpdating); 
                    adrelaciona.TableMappings.Add("Table", "RELACIONA");
                    dsrelaciona = new DataSet();
                    adrelaciona.Fill(dsrelaciona);
                    //DataTable planorder = odataset.Tables["PLACON"].AsEnumerable().Where(row => row.Field<string>("GRAU") == "5" ? row.Field<decimal>("SDO") != 0 : true).OrderBy(row => row.Field<string>("numconta")).CopyToDataTable();
                    //DataTable planorder = odataset.Tables["PLACON"].AsEnumerable().OrderBy(row => row.Field<string>("numconta")).CopyToDataTable();
                    int nreg = 0;
                    try
                    {
                        foreach (DataRow velho in oVelho.Rows)
                        {
                            DataRow origem = dsrelaciona.Tables["RELACIONA"].NewRow();
                            foreach (DataColumn col in oVelho.Columns)
                            {
                                if (dsrelaciona.Tables["RELACIONA"].Columns.Contains(col.ColumnName))
                                {
                                    origem[col.ColumnName] = velho[col.ColumnName];

                                }
                            }

                            DataRow dadoExcel = tabexcelAltera.AsEnumerable().Where(row => row.Field<string>("NOVOCOD") == velho["NOVOCOD"].ToString().Trim()).FirstOrDefault();
                            if (dadoExcel == null)
                            {
                                MessageBox.Show("COdigo REDUZIDO NÃO ENCONTRADO PARA NOVOCOD" + velho["NOVOCOD"].ToString());
                                return;
                            }
                            origem["REDUZIDO"] = Convert.ToInt32(dadoExcel["REDUZIDO"]);

                            nreg++;
                            origem["NREG"] = nreg;
                            dsrelaciona.Tables["RELACIONA"].Rows.Add(origem);
                            // acrescenta o codigo reduzido QUE ESTÀ NA PLANILHAEXCEL
                        }
                        adrelaciona.Update(dsrelaciona.Tables["RELACIONA"]);
                        dsrelaciona.Tables["RELACIONA"].AcceptChanges();
                    }
                    catch (Exception)
                    {
                        throw;
                    }


                    oview.DataSource = dsrelaciona.Tables["RELACIONA"];
                    oform.Text = "RELACIONA => ALTERADO EXCEL";

                    // for (int i = 0;i<oview.TableStyles[0].GridColumnStyles.Count;i++)
                    //    oview.TableStyles[0].GridColumnStyles[i].NullText = ""; 
                    oview.Parent = oform;
                    oview.Dock = DockStyle.Fill;
                    oform.Show();



                    return;
                }
                catch (Exception)
                {
                    throw;
                }

            }
        }
    }
}
