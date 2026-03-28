using ApoioContabilidade.Models;
using ClassConexao;
using ClassFiltroEdite;
using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Windows.Forms;

namespace ApoioContabilidade
{
    public partial class FrmGravaMovFinNoServidor : Form
    {
        public FrmGravaMovFinNoServidor()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            DateTime inicio, fim;
            inicio = dtData1.Value;
            fim = dtData2.Value;


            string condicao = " DATA BETWEEN '" + inicio.ToString("yyyy-MM-dd") + "' AND '" + fim.ToString("yyyy-MM-dd") + "'";

            Cursor.Current = Cursors.WaitCursor;

            int retorno = await Prepara_Sql.APIDeleteTodosRegistrosTabela("MOVFIN", condicao);
            int result = await Grave(inicio, fim);
            Cursor.Current = Cursors.Default;


        }

        private async Task<int> Grave(DateTime inicio, DateTime fim)
        {
            int totRegistrosIncluidos = 0;

            try
            {
                Cursor.Current = Cursors.WaitCursor;

                DataSet dataSet1 = MovFinanVauches(inicio, fim);
                List<DataRow> dataRowsServidor = new List<DataRow>();
                int result = -1;
                int numeroRegistros = dataSet1.Tables[0].Rows.Count;
                foreach (DataRow orow in dataSet1.Tables[0].Rows)
                {
                    dataRowsServidor.Add(orow);

                    //19 / 08 / 2015 00:00:00    72,6300 PAIN # MAN_INST          
                    if (dataRowsServidor.Count > 10)
                    {
                        result = await Prepara_Sql.OpereIncluaRegistrosMultiplosServidorAsync(dataRowsServidor.ToArray(), "MOVFIN");
                        if (result == -1)
                        {
                            /*    if (!(numeroRegistros == totRegistrosIncluidos))
                                {
                                    MessageBox.Show("Não foram gravados todos reg. Registros Gravados:" + totRegistrosIncluidos.ToString()
                                        +"Total Registros: " + numeroRegistros.ToString()

                                        );


                                }*/
                            break;
                        }
                        else
                            totRegistrosIncluidos = totRegistrosIncluidos + dataRowsServidor.Count;
                        dataRowsServidor.Clear();

                    }
                }
                if (dataRowsServidor.Count > 0)
                {
                    result = await Prepara_Sql.OpereIncluaRegistrosMultiplosServidorAsync(dataRowsServidor.ToArray(), "MOVFIN");
                    totRegistrosIncluidos = totRegistrosIncluidos + dataRowsServidor.Count();
                    dataRowsServidor.Clear();
                }

                if (!(numeroRegistros == totRegistrosIncluidos))
                {
                    MessageBox.Show("Não foram gravados todos reg. Registros Gravados:" + totRegistrosIncluidos.ToString()
                        + "Total Registros: " + numeroRegistros.ToString()

                        );


                } else
                {
                    MessageBox.Show("Sucesso " + "Total Registros: " + numeroRegistros.ToString());
                }


            }
            finally
            {
                Cursor.Current = Cursors.Default;

            }
            return totRegistrosIncluidos;


        }








        private DataSet MovFinanVauches(DateTime data1, DateTime data2) //, List<LinhaSolucao> oLista, PesquisaGenerico oPesquisa)
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
               " doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof ";

            }
            else
            {

                campos = " data, valor valor, debito, credito, tipo, tp_fin, tp_ok, doc, hist, tipo_doc, forn, venc, doc_dupl, nhist," +
                    " doc_fisc, emissor, data_emi, obs, mov_id, outro_id, serienf, basecalc, isento, outros, icms, livro, codigof ";
            }
            setoroledb = "SELECT " + campos + " FROM " + path + "MOVFIN  where (data between " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data1.Date) + ") AND " +
               "CTOD(" + TDataControlReduzido.FormatDataGravar(data2.Date) + ")) AND (VALOR > 0)";

            /*if (oLista != null)
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
            }*/
            setoroledb += " ORDER BY DATA,DOC";
            try
            {
                oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                oledbda.TableMappings.Add("Table", "MOVFIN");
                DataSet odataset = new DataSet();
                oledbda.Fill(odataset);
                oledbda.Dispose();
                DataTable tabDeb = odataset.Tables[0].Copy();
                //DataTable tabDeb = odataset.Tables[0].Clone();

                /*for (int i = 0; i < odataset.Tables[0].Rows.Count; i++)
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

                }*/
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

        private async void btnImobil_Click(object sender, EventArgs e)
        {
            //SELECT cod, `desc`, contab, dep_acum, resultado, val_aquis, data_aquis, data_corr, val_ufir, dep_ufir, data_baixa, val_baixa
            //   FROM imobil
            int retorno = await Prepara_Sql.APIDeleteTodosRegistrosTabela("IMOBIL", "");
            int res = await GraveImobil();
        }

        private async Task<int> GraveImobil()
        {
            int totRegistrosIncluidos = 0;
            try
            {
                Cursor.Current = Cursors.WaitCursor;
                DataTable tabDeb = new DataTable();
                OleDbCommand oledbcomm;
                string setoroledb, path;

                path = TDataControlReduzido.Get_Path("CONTAB");
                string campos = "";
                campos = "cod, `desc` DESCRI, contab, dep_acum, resultado, val_aquis, data_aquis, data_corr, val_ufir, dep_ufir, " +
                    "data_baixa, val_baixa";
                setoroledb = "SELECT " + campos + " FROM " + path + "IMOBIL ";
                try
                {
                    oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
                    OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
                    oledbda.TableMappings.Add("Table", "IMOBIL");
                    DataSet odataset = new DataSet();
                    oledbda.Fill(odataset);
                    oledbda.Dispose();
                    tabDeb = odataset.Tables[0].Copy();


                }
                catch { }
                int result = -1;
                List<DataRow> dataRowsServidor = new List<DataRow>();
                int numeroRegistros = tabDeb.Rows.Count;

                foreach (DataRow orow in tabDeb.Rows)
                {

                    if (Convert.ToDateTime(orow["DATA_BAIXA"]) ==  new DateTime(1899,12,30))
                    {
                        orow["DATA_BAIXA"] = DBNull.Value;
                    }
                    if (Convert.ToDateTime(orow["DATA_CORR"]) == new DateTime(1899, 12, 30))
                    {
                        orow["DATA_CORR"] = DBNull.Value;
                    }
                    if (Convert.ToDateTime(orow["DATA_AQUIS"]) == new DateTime(1899, 12, 30))
                    {
                        orow["DATA_AQUIS"] = DBNull.Value;
                    }



                    dataRowsServidor.Add(orow);

                    //19 / 08 / 2015 00:00:00    72,6300 PAIN # MAN_INST          
                    if (dataRowsServidor.Count > 10)
                    {
                        result = await Prepara_Sql.OpereIncluaRegistrosMultiplosServidorAsync(dataRowsServidor.ToArray(), "IMOBIL");
                        if (result == -1)
                        {

                            break;
                        }
                        else
                            totRegistrosIncluidos = totRegistrosIncluidos + dataRowsServidor.Count;
                        dataRowsServidor.Clear();

                    }
                }
                if (dataRowsServidor.Count > 0)
                {
                    result = await Prepara_Sql.OpereIncluaRegistrosMultiplosServidorAsync(dataRowsServidor.ToArray(), "IMOBIL");
                    totRegistrosIncluidos = totRegistrosIncluidos + dataRowsServidor.Count();
                    dataRowsServidor.Clear();
                }

                if (!(numeroRegistros == totRegistrosIncluidos))
                {
                    MessageBox.Show("Não foram gravados todos reg. Registros Gravados:" + totRegistrosIncluidos.ToString()
                        + "Total Registros: " + numeroRegistros.ToString()

                        );


                }
                else
                {
                    MessageBox.Show("Sucesso " + "Total Registros: " + numeroRegistros.ToString());
                }


            }
            finally
            {
                Cursor.Current = Cursors.Default;

            }
            return totRegistrosIncluidos;


        }
    }
}
