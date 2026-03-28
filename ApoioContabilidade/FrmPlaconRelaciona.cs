using ClassConexao;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApoioContabilidade.Excel;
using System.Data.OleDb;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    public partial class FrmPlaconRelaciona : Form
    {
        DataTable tabexcel;
        OleDbDataAdapter adrelaciona;
        DataSet dsrelaciona;
        private MonteGrid oRelaciona;
        DataTable tabrelaciona;
        public FrmPlaconRelaciona()
        {

            InitializeComponent();
            oRelaciona = new MonteGrid();
            MonteGrids();
            dgvRelaciona.ReadOnly = true;
            dgvRelaciona.AllowUserToAddRows = false;
            dgvRelaciona.AllowUserToDeleteRows = false;
            oRelaciona.oDataGridView = dgvRelaciona;
            bool relacionaExiste = TDataControlContab.TabelaExiste("RELACIONA.DBF");
            if (!relacionaExiste)
            {
                adrelaciona = TDataControlContab.CreateTabRelaciona();

                adrelaciona.TableMappings.Add("Table", "RELACIONA");
                dsrelaciona = new DataSet();
                adrelaciona.Fill(dsrelaciona);
            }
            else
            {
               dsrelaciona = TDataControlContab.TabRelaciona();
            }
            tabrelaciona = dsrelaciona.Tables[0];
        }


        // IMPORTA ATRAVÉS DO EXCEL OS DADOS DE RALIONAMENTO ENTRE NUMCONTA E ALTERDATE E RECRIA O RELACIONA.DBF
        private void btnImporta_Click(object sender, EventArgs e)
        {
            /*if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {

                    string nomeplanilha = TDataControlReduzido.ExcelNomePlanilha(openFileDialog1.FileName);
                    tabexcel = TDataControlReduzido.LeiaExcel(openFileDialog1.FileName, nomeplanilha);
                  
                   // DataGrid oview = new DataGrid();
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
                        if (!retorno)
                        {
                            MessageBox.Show("Problemas com a importação!Verifique campos necessários no Excel!.");
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("O nome da Pasta pode ser qualquer um. O Nome da PLANILHA tem que ser PESQUISA");
                        return;
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
            }*/
        }
        private void MonteGrids()
        {
            oRelaciona.Clear();
            oRelaciona.AddValores("NUMCONTA", "NUMCONTA", 15, "", false, 0, "");
            oRelaciona.AddValores("DESCRICAO", "DESCRICAO", 45, "", false, 0, "");
            oRelaciona.AddValores("NOVOCOD", "NOVOCOD", 15, "", false, 0, "");
            oRelaciona.AddValores("REDUZIDO", "REDUZIDO", 15, "", false, 0, "");
            oRelaciona.AddValores("NOVADESC", "NOVADESC", 45, "", false, 0, "");

        }
        private Boolean ImportaRelaciona_Excel()
        {
            bool result = false;
            if (!((tabexcel != null) && (tabexcel.Columns.Contains("NUMCONTA")) && (tabexcel.Columns.Contains("NOVOCOD")) && (tabexcel.Columns.Contains("NOVADESC"))
                && (tabexcel.Columns.Contains("REDUZIDO"))))
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




    }
}
