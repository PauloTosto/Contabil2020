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

namespace ApoioContabilidade.Financeiro
{
    public partial class FrmConciliaBB : Form
    {
        DataTable extratoBB;
        public FrmConciliaBB()
        {
            InitializeComponent();
            CriaTabelaVirtualExtrato();
        }

        private void CriaTabelaVirtualExtrato()
        {
            extratoBB = new DataTable("extratoBB");

            extratoBB.Columns.Add("DATA", Type.GetType("System.DateTime"));

            extratoBB.Columns.Add("OBSERVACAO", Type.GetType("System.String")); 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 100;

            extratoBB.Columns.Add("DATA_BALANCETE", Type.GetType("System.DateTime"));


            // numero
            extratoBB.Columns.Add("AGENCIA_ORIG", Type.GetType("System.String")); // 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 4;

            // numero
            extratoBB.Columns.Add("LOTE", Type.GetType("System.String")); // 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 5;


            extratoBB.Columns.Add("NUMERODOCUMENTO", Type.GetType("System.String")); // 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 17;

            extratoBB.Columns.Add("CODHISTORICO", Type.GetType("System.String")); // 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 3;


            extratoBB.Columns.Add("HISTORICO", Type.GetType("System.String")); // 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 25;


            extratoBB.Columns.Add("VALOR", Type.GetType("System.Decimal"));

            extratoBB.Columns.Add("DBCR", Type.GetType("System.String")); // 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 1;



            extratoBB.Columns.Add("DETALHAHISTORICO", Type.GetType("System.String")); // Descritivo 
            extratoBB.Columns[extratoBB.Columns.Count - 1].MaxLength = 70;

        }

        private void btnLeiaExcel_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {

                    string nomeplanilha = TDataControlReduzido.ExcelNomePlanilha(openFileDialog1.FileName);
                    DataTable tabexcel = TDataControlReduzido.LeiaExcel(openFileDialog1.FileName, nomeplanilha);
                    Form oform = new Form();
                    DataGrid oview = new DataGrid();
                    if (nomeplanilha.Trim().ToUpper() != "EXTRATO")
                    {
                        MessageBox.Show("Não é extrato!");
                        return;
                    }
                    foreach(DataRow orow in tabexcel.Rows)
                    {
                        bool nulo = false;
                        foreach (DataColumn ocol in orow.Table.Columns)
                        {
                            if (orow.IsNull(ocol.ColumnName))
                            { nulo = true;
                                break;
                            }
                        }
                        if (nulo) continue;
                        DataRow novo = extratoBB.NewRow();
                        int i = 0;
                        try
                        {
                            foreach (DataColumn ocol in orow.Table.Columns)
                            {
                                if (novo.Table.Columns[i].DataType.FullName == "System.String")
                                {
                                    novo[i] = orow[ocol.ColumnName].ToString();
                                }
                                else if (novo.Table.Columns[i].DataType.FullName == "System.Decimal")
                                {
                                    novo[i] = Convert.ToDecimal(orow[ocol.ColumnName].ToString().Trim());
                                }
                                else if (novo.Table.Columns[i].DataType.FullName == "System.DateTime")
                                {
                                    novo[i] = Convert.ToDateTime(orow[ocol.ColumnName].ToString().Trim());
                                }
                                else
                                {
                                    MessageBox.Show("Coluna tipo Não Previsto");
                                }

                                i++;
                            }

                        }
                        catch (Exception)
                        {

                            continue;
                        }
                        extratoBB.Rows.Add(novo);

                    }
                    extratoBB.AcceptChanges();


                        /*  if (nomeplanilha.Trim().ToUpper() == "PESQUISA") // Dentro do arquivo excel tem que ter a planilha com o nome PESQUISA para gravar o relaciona definitivo 
                          {

                             /* mostraRelaciona = (MessageBox.Show("Cria novo RELACIONA.DBF sobreescrevendo atual.Importa os dados com base na planilha EXCEL <PESQUISA>?", "Responda", MessageBoxButtons.OKCancel) == DialogResult.OK);
                              bool retorno = ImportaRelaciona_Excel();
                              if (!retorno)
                              {
                                  MessageBox.Show("Problemas com a importação!Verifique campos necessários no Excel!.");
                                  return;
                              }*/
                        /* }
                         else
                         {
                             MessageBox.Show("O nome da Pasta pode ser qualquer um. O Nome da PLANILHA tem que ser PESQUISA");
                             return;
                         }*/
                    oview.DataSource = extratoBB;
                    oform.Text = "EXCEL DIRETO";
                    
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
    }
}
