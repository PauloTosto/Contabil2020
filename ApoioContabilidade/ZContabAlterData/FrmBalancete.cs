using ApoioContabilidade.Excel;
using ApoioContabilidade.Models;
using ClassConexao;
using ClassFiltroEdite;
using Microsoft.Office.Interop.Excel;
using Microsoft.Reporting.Map.WebForms.BingMaps;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using static ApoioContabilidade.ZContabAlterData.Utils.BalanceteClass;

namespace ApoioContabilidade.ZContabAlterData
{
    public partial class FrmBalancete : Form
    {

        public System.Data.DataTable dtBalancete;
        //public DataTable dtBalancinho;
        //public DataTable dtBalanceteOriginal;
        public System.Data.DataTable dtPlaconAno;
        public System.Data.DataTable dtVauchesRazao;

        private MonteGrid oBalancete;
        // private bool recomece;
        // private FormFiltro oForm;
        List<string> MesesAno = new List<String>();

        public BindingSource bmSource;
        DateTime inicio;
        DateTime fim;
        bool exclui_fechamento;
        bool sintetico;
        public Dictionary<string, Relaciona> dictrelaciona;


        //, DataTable odtBalancete, DataTable odtBalancinho
        public FrmBalancete(System.Data.DataTable odtPlaconAno, System.Data.DataTable ovauchesRazao
            , DateTime oinicio, DateTime ofim)
        {
            
            InitializeComponent();

            exclui_fechamento = false;
            sintetico = false;
            
            ckSintetico.Checked = false;
            cbExcluiFechamento.Checked = false;
            dtVauchesRazao = ovauchesRazao;
            dtPlaconAno = odtPlaconAno;
            // necessario para o DBCONFIGURA Funcioine 
            pnGeral.AutoScroll = true;
            // o tabela tem que ter um nome
            inicio = oinicio; fim = ofim;
            DateTime serie = oinicio;
            MesesAno = new List<string>();
            MesesAno.Add("TODO ANO");
            btnExcel.Enabled= false;

            btnSaldo.Enabled = false;

            

            MesesAno.Add(serie.Month.ToString().PadLeft(2, '0') + "/" + serie.Year.ToString());
            while (
                    (serie.Year.ToString() + "/" + serie.Month.ToString().PadLeft(2, '0')).CompareTo(
                    fim.Year.ToString() + "/" + fim.Month.ToString().PadLeft(2, '0'))
                         < 0
                    )
            {
                serie = serie.AddMonths(1);
                MesesAno.Add(serie.Month.ToString().PadLeft(2, '0') + "/" + serie.Year.ToString());
            }

            cbMeses.DataSource = MesesAno;
            cbMeses.SelectedIndex = 0;
           /* dtBalanceteOriginal = odtBalancete.Copy();

            dtBalancete = dtBalanceteOriginal.AsEnumerable().Where(a => (a.Field<decimal>("SDOANT") != 0)
            ||
            (a.Field<decimal>("SDO") != 0)
             ||
            (a.Field<decimal>("DEBITO") != 0)
             ||
            (a.Field<decimal>("CREDITO") != 0)

            ).CopyToDataTable();
            dtBalancete.TableName = "BalanceteGeral";

            dtBalancinho = odtBalancinho;


            oBalancete = new MonteGrid();
            MonteGrids();
           */
            dgvBalancete.ReadOnly = true;
            dgvBalancete.AllowUserToAddRows = false;
            //dgvBalancete.AllowSorting = false;
           /* oBalancete.oDataGridView = dgvBalancete;
            oBalancete.sbTotal = sbBalancete;
            // oPlacon = TDataControlReduzido.Placon();


            //oForm = new FormFiltro();
            bmSource = new BindingSource();
            bmSource.DataSource = dtBalancete;
            ///bmSourceSaida.DataSource = dvSaidas;
            dgvBalancete.DataSource = bmSource;

            oBalancete.ConfigureDBGridView();
            // oBalancete.FuncaoSoma();
            // oBalancete.ColocaTotais();*/
        }
        private void MonteGrids()
        {
            oBalancete.Clear();

            // oBalancete.AddValores("NUMCONTA", "CONTA", 12, "", false, 0, "");
            //  oBalancete.AddValores("DESCRICAO", "DESCRIÇÃO", 50, "", false, 0, "");
            oBalancete.AddValores("DESCBAL", "Descrição", 55, "", false, 0, "");
            oBalancete.AddValores("SDOANT", "Saldo Anterior", 12, "###,###,##0.00", true, 0, "");
            oBalancete.AddValores("DEBITO", "Debito", 12, "###,###,##0.00", true, 0, "");
            oBalancete.AddValores("CREDITO", "Credito", 12, "###,###,##0.00", true, 0, "");
            oBalancete.AddValores("SDO", "Sdo Atual", 12, "###,###,##0.00", true, 0, "");

            /* 
             * 
             * //orowB["NUMCONTA"] = dataRow["NUMCONTA"];
             * orowB["DESCRICAO"] = dataRow["DESCRICAO"];
             orowB["DESC2"] = dataRow["DESC2"];
             orowB["SDOANT"] = dataRow["SDO"];
             orowB["SDOANTCALC"] = 0;
             orowB["SDO"] = 0;
             orowB["DEBITO"] = 0;
             orowB["CREDITO"] = 0;
             orowB["Inicio"] = inicio;
             orowB["Fim"] = fim;
             ;*/



        }

        private void dgvBalancete_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;
            if (dgv.Columns[e.ColumnIndex].Name == "SDOVLR" &&
                e.RowIndex >= 0 &&
                dgv["SDOVLR", e.RowIndex].Value is Decimal)
            {


                if (e.Value != null)
                {
                    try
                    {
                        System.Text.StringBuilder numberString = new System.Text.StringBuilder();

                        /* string numero  = number
                         dateString.Append(theDate.Month);
                         dateString.Append("/");
                         dateString.Append(theDate.Day);
                         dateString.Append("/");
                         dateString.Append(theDate.Year.ToString().Substring(2));
                         formatting.Value = dateString.ToString();
                         formatting.FormattingApplied = true;*/
                    }
                    catch (FormatException)
                    {
                        // Set to false in case there are other handlers interested trying to
                        // format this DataGridViewCellFormattingEventArgs instance.
                        e.FormattingApplied = false;
                    }
                }



                /*switch ((int)dgv["TargetColumnName", e.RowIndex].Value)
                {

                //Create custom display text/value here and assign to e.Value
                string dataformatValue = //Create from database value;
                e.Value = dataformatValue;
                e.FormattingApplied = true; 
               */
            }
        }
        // REFERENCIAS PARA ROWFILTER => https://www.csharp-examples.net/dataview-rowfilter/
        private void txNumconta_TextChanged(object sender, EventArgs e)
        {
            try
            {

            string numconta = (sender as System.Windows.Forms.TextBox).Text.Trim();
            //  if ((numconta.Length == 4) || (numconta.Length == 6) || (numconta.Length == 7))  return;
            // string filtro = "SUBSTRING(NUMCONTA,1," + numconta.Length.ToString() + ") = '" + numconta + "'";
            if (numconta.Length == 0) { bmSource.DataSource = dtBalancete; }
            else
            {
                try
                {
                    var dado = dtBalancete.AsEnumerable().Where(row =>
                        (row.Field<string>("NUMCONTA").Substring(0, numconta.Length) == numconta));
                    if ((dado != null) && (dado.Count() > 0))
                    { bmSource.DataSource = dado.CopyToDataTable(); }
                    else { return; }
                }
                catch (Exception)
                {
                    return;
                }
            }
            }
            catch (Exception)
            {

                MessageBox.Show("Não teve efeito. A consulta não deve ser realizada com o a pesquisa de uma conta em aberto");
            }

            //oBalancete.FuncaoSoma();
            oBalancete.oDataGridView.Refresh();
            //oBalancete.ColocaTotais();
        }

        private void btnExcel_Click(object sender, EventArgs e)
        {
            string periodo = cbMeses.Text;
            DateTime inicioPer = inicio;
            DateTime  fimPer = fim;
            if (periodo != "TODO ANO")
            {
                int mes = Convert.ToInt16(periodo.Substring(0, 2));
                int ano = Convert.ToInt16(periodo.Substring(3, 4));
                inicioPer = new DateTime(ano, mes, 1);
                fimPer = inicioPer.AddMonths(1).AddDays(-1);
            }



            oBalancete.tituloExcel = "Balancete Periodo de " + inicioPer.ToString("d") + " a " + fimPer.ToString("d");
            // oBalancete.ExportaExcelBalancete();
            oBalancete.ExportaExcel();
        }

        private void btnConsulte_Click(object sender, EventArgs e)
        {
            if (txNumconta.Text != "")
                txNumconta.Text = "";

            System.Data.DataTable dataTable = Reload();


            if (!sintetico)
            {
                dtBalancete = dataTable.AsEnumerable().Where(a => (a.Field<decimal>("SDOANT") != 0)
                     ||
                     (a.Field<decimal>("SDO") != 0)
                      ||
                     (a.Field<decimal>("DEBITO") != 0)
                      ||
                     (a.Field<decimal>("CREDITO") != 0)

                 ).CopyToDataTable();
            }
            else
            {
                dtBalancete = dataTable.AsEnumerable().Where(a => 
                    ((a.Field<decimal>("SDOANT") != 0)
                     ||
                     (a.Field<decimal>("SDO") != 0)
                      ||
                     (a.Field<decimal>("DEBITO") != 0)
                      ||
                     (a.Field<decimal>("CREDITO") != 0))
                     && (a.Field<string>("NUMCONTA").Substring(5,3) == "000")

                 ).CopyToDataTable();

            }

            dtBalancete.TableName = "BalanceteGeral";
            oBalancete = new MonteGrid();
            MonteGrids();

            dgvBalancete.ReadOnly = true;
            dgvBalancete.AllowUserToAddRows = false;
            //dgvBalancete.AllowSorting = false;
            oBalancete.oDataGridView = dgvBalancete;
            oBalancete.sbTotal = sbBalancete;
            // oPlacon = TDataControlReduzido.Placon();


            //oForm = new FormFiltro();
            bmSource = new BindingSource();
            bmSource.DataSource = dtBalancete;
            ///bmSourceSaida.DataSource = dvSaidas;
            dgvBalancete.DataSource = bmSource;

            oBalancete.ConfigureDBGridView();
            
            
            btnExcel.Enabled = true;

            string periodo = cbMeses.Text;
            if (periodo == "TODO ANO")
            {
                btnSaldo.Enabled = true;
            }
            else { btnSaldo.Enabled = false;  }


        }


        private System.Data.DataTable Reload()
        {
            btnExcel.Enabled = false;
            btnSaldo.Enabled = false;
            Cursor.Current = Cursors.WaitCursor;

            System.Data.DataTable Balancete = new System.Data.DataTable();
            try
            {


                string periodo = cbMeses.Text;
                DateTime inicioPer = inicio;
                DateTime fimPer = fim;
                if (periodo != "TODO ANO")
                {
                    int mes = Convert.ToInt16(periodo.Substring(0, 2));
                    int ano = Convert.ToInt16(periodo.Substring(3, 4));
                    inicioPer = new DateTime(ano, mes, 1);
                    fimPer = inicioPer.AddMonths(1).AddDays(-1);
                }

                var lstsdoAntDebito =

                (from soma in dtVauchesRazao.AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(inicioPer) < 0)

                                )
                 group soma by new
                 {
                     numConta = soma.Field<string>("DEBITO"),

                     // icodser = soma.ICodSer
                 } into g
                 select new BalancoCampos
                 {
                     numconta = g.Key.numConta,
                     sdoAntCalc = g.Sum(p => p.Field<decimal>("VALOR")),

                 }).ToList();

                var lstsdoAntCredito =

                (from soma in dtVauchesRazao.AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(inicioPer) < 0)

                                )
                 group soma by new
                 {
                     numConta = soma.Field<string>("CREDITO"),

                     // icodser = soma.ICodSer
                 } into g
                 select new BalancoCampos
                 {
                     numconta = g.Key.numConta,
                     sdoAntCalc = g.Sum(p => p.Field<decimal>("VALOR")),

                 }).ToList();


                var lstDebito =

                       (from soma in dtVauchesRazao.AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(inicioPer) >= 0)
                           && (a.Field<DateTime>("DATA").CompareTo(fimPer) <= 0)
                          && (exclui_fechamento ? (  //(a.Field<string>("DOC").Trim()!="SIST_BAL") && 
                          !( (a.Field<string>("DOC").Trim() == "SIST_BAL") 
                              || (a.Field<string>("DEBITO").Trim() == "23398001") 
                              || (a.Field<string>("CREDITO").Trim() == "23398001")) ) : true)
                             )                      
                        group soma by new
                        {
                            numConta = soma.Field<string>("DEBITO"),

                            // icodser = soma.ICodSer
                        } into g
                        select new BalancoCampos
                        {
                            numconta = g.Key.numConta,
                            debito = g.Sum(p => p.Field<decimal>("VALOR")),

                        }).ToList();

                var lstCredito =

                      (from soma in dtVauchesRazao.AsEnumerable().Where(a => (a.Field<DateTime>("DATA").CompareTo(inicioPer) >= 0)
                          && (a.Field<DateTime>("DATA").CompareTo(fimPer) <= 0)
                       //&& (exclui_fechamento ? (a.Field<string>("DOC").Trim() != "SIST_BAL") : true))
                       && (exclui_fechamento ? (  //(a.Field<string>("DOC").Trim()!="SIST_BAL") && 
                                                  //! ((a.Field<string>("DEBITO").Trim() == "23398001") || (a.Field<string>("CREDITO").Trim() == "23398001"))) : true)
                          !((a.Field<string>("DOC").Trim() == "SIST_BAL")
                              || (a.Field<string>("DEBITO").Trim() == "23398001")
                              || (a.Field<string>("CREDITO").Trim() == "23398001"))) : true)


                         )

                       group soma by new
                       {
                           numConta = soma.Field<string>("CREDITO"),

                           // icodser = soma.ICodSer
                       } into g
                       select new BalancoCampos
                       {
                           numconta = g.Key.numConta,
                           credito = g.Sum(p => p.Field<decimal>("VALOR")),

                       }).ToList();


                Balancete.TableName = "BALANCETE";
                Balancete.Columns.Add("NUMCONTA", Type.GetType("System.String"));
                Balancete.Columns.Add("GRAU", Type.GetType("System.String"));
                Balancete.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                Balancete.Columns.Add("DESCBAL", Type.GetType("System.String"));
                Balancete.Columns.Add("DESC2", Type.GetType("System.String"));
                Balancete.Columns.Add("SDOANT", Type.GetType("System.Decimal"));
                Balancete.Columns.Add("SDOANTCALC", Type.GetType("System.Decimal"));
                Balancete.Columns.Add("DEBITO", Type.GetType("System.Decimal"));
                Balancete.Columns.Add("CREDITO", Type.GetType("System.Decimal"));
                Balancete.Columns.Add("SDO", Type.GetType("System.Decimal"));
                Balancete.Columns.Add("Inicio", Type.GetType("System.DateTime"));
                Balancete.Columns.Add("Fim", Type.GetType("System.DateTime"));
                try
                {

                    foreach (DataRowView dataRow in dtPlaconAno.AsDataView())
                    {
//                        var existe = dtBalancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
  //                      if (existe != null) continue;

                        if (dataRow["NUMCONTA"].ToString() == "00000000") continue;
                        // if (dataRow["NUMCONTA"].ToString() == "20000000") break;

                        DataRow orowB = Balancete.NewRow();
                        orowB["NUMCONTA"] = dataRow["NUMCONTA"];
                        orowB["DESCRICAO"] = dataRow["DESCRICAO"];
                        if (dataRow["NUMCONTA"].ToString().Substring(1, 7) == "0000000")
                        {
                            orowB["DESCBAL"] =
                                dataRow["NUMCONTA"].ToString().Substring(0, 1) + "  " + dataRow["DESCRICAO"].ToString();
                        }
                        else if (dataRow["NUMCONTA"].ToString().Substring(2, 6) == "000000")
                        {
                            orowB["DESCBAL"] = dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                             dataRow["NUMCONTA"].ToString().Substring(1, 1)
                             + "  " + dataRow["DESCRICAO"].ToString();
                        }
                        else if (dataRow["NUMCONTA"].ToString().Substring(3, 5) == "00000")
                        {
                            orowB["DESCBAL"] = dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                             dataRow["NUMCONTA"].ToString().Substring(1, 1)
                             + "." +
                            dataRow["NUMCONTA"].ToString().Substring(2, 1)
                             + "  " + dataRow["DESCRICAO"].ToString();
                        }
                        else if (dataRow["NUMCONTA"].ToString().Substring(5, 3) == "000")
                        {
                            orowB["DESCBAL"] = dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                             dataRow["NUMCONTA"].ToString().Substring(1, 1)
                             + "." +
                             dataRow["NUMCONTA"].ToString().Substring(2, 1) + "." +
                             dataRow["NUMCONTA"].ToString().Substring(3, 2)
                             + "  " + dataRow["DESCRICAO"].ToString();
                        }

                        else
                        {
                            orowB["DESCBAL"] =
                                dataRow["NUMCONTA"].ToString().Substring(0, 1) + "." +
                                dataRow["NUMCONTA"].ToString().Substring(1, 1) + "." +
                                dataRow["NUMCONTA"].ToString().Substring(2, 1) + "." +
                                 dataRow["NUMCONTA"].ToString().Substring(3, 2) + "." +
                                 dataRow["NUMCONTA"].ToString().Substring(5, 3)
                                + "  " + dataRow["DESCRICAO"].ToString();
                        }
                        orowB["DESC2"] = dataRow["DESC2"];
                        orowB["SDOANT"] = dataRow["SDO"];
                        orowB["SDOANTCALC"] = 0;
                        orowB["SDO"] = 0;
                        orowB["DEBITO"] = 0;
                        orowB["CREDITO"] = 0;
                        orowB["Inicio"] = inicioPer;
                        orowB["Fim"] = fimPer;

                        if (dataRow["NUMCONTA"].ToString().Substring(5, 3) != "000")
                        {

                            decimal debitoant = 0;
                            BalancoCampos sdoAntDeb = lstsdoAntDebito.Where(a => a.numconta == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
                            if (sdoAntDeb != null)
                            {
                                debitoant = sdoAntDeb.sdoAntCalc;
                            }


                            decimal creditoant = 0;
                            BalancoCampos sdoAntCre = lstsdoAntCredito.Where(a => a.numconta == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
                            if (sdoAntCre != null)
                            {
                                creditoant = sdoAntCre.sdoAntCalc;
                            }


                            orowB["SDOANTCALC"] = debitoant - creditoant;

                            /*


                    decimal debito = (from linha in dtVauchesRazao.AsEnumerable()
                                      where ((linha.Field<string>("DEBITO").Trim() == dataRow["NUMCONTA"].ToString())
                                      // && ((linha.Field<DateTime>("DATA").CompareTo(dateTime) >= 0))
                                      && (linha.Field<DateTime>("DATA").CompareTo(inicioPer) >= 0)
                                      && (linha.Field<DateTime>("DATA").CompareTo(fimPer) <= 0))
                                      select linha.Field<decimal>("VALOR")).Sum();

                    decimal credito =

                            (from linha in dtVauchesRazao.AsEnumerable()
                             where ((linha.Field<string>("CREDITO").Trim() == dataRow["NUMCONTA"].ToString())
                             // && ((linha.Field<DateTime>("DATA").CompareTo(dateTime) >= 0))
                             && (linha.Field<DateTime>("DATA").CompareTo(inicioPer) >= 0)
                             && (linha.Field<DateTime>("DATA").CompareTo(fimPer) <= 0))
                             select linha.Field<decimal>("VALOR")).Sum();

                        */

                            decimal debito = 0;
                            BalancoCampos Debito = lstDebito.Where(a => a.numconta == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
                            if (Debito != null)
                            {
                                debito = Debito.debito;
                            }

                            decimal credito = 0;
                            BalancoCampos Credito = lstCredito.Where(a => a.numconta == dataRow["NUMCONTA"].ToString()).FirstOrDefault();
                            if (Credito != null)
                            {
                                credito = Credito.credito;
                            }

                            orowB["SDOANT"] = Convert.ToDecimal(orowB["SDOANT"]) + Convert.ToDecimal(orowB["SDOANTCALC"]);
                            orowB["DEBITO"] = debito;
                            orowB["CREDITO"] = credito;
                            orowB["SDO"] = Convert.ToDecimal(orowB["SDOANT"])  + debito - credito;
                        }
                        Balancete.Rows.Add(orowB);
                        orowB.AcceptChanges();
                    }

                    Balancete.AcceptChanges();
                }
                catch (Exception E)
                {

                    MessageBox.Show(E.Message);
                }


                List<BalancoCampos> quartoGrau = 
                    (from soma in Balancete.AsEnumerable().Where(a => (a.Field<string>("NUMCONTA").Substring(5, 3) != "000")

                                )
                                                  group soma by new
                                                  {
                                                      numConta = soma.Field<string>("NUMCONTA").Substring(0, 5),

                                                      // icodser = soma.ICodSer
                                                  } into g
                                                  select new BalancoCampos
                                                  {
                                                      numconta = g.Key.numConta,
                                                      sdoAntCalc = Math.Round(g.Sum(p => p.Field<decimal>("SDOANTCalc")),2),
                                                      sdoAnt = Math.Round(g.Sum(p => p.Field<decimal>("SDOANT")),2),
                                                      debito = Math.Round(g.Sum(p => p.Field<decimal>("DEBITO")),2),
                                                      credito = Math.Round(g.Sum(p => p.Field<decimal>("CREDITO")),2),

                                                  }).ToList();



                List<BalancoCampos> terceiroGrau = (from soma in quartoGrau.AsEnumerable()


                                                    group soma by new
                                                    {
                                                        numConta = soma.numconta.Substring(0, 3),

                                                        // icodser = soma.ICodSer
                                                    } into g
                                                    select new BalancoCampos
                                                    {
                                                        numconta = g.Key.numConta,
                                                        sdoAntCalc = g.Sum(p => p.sdoAntCalc),
                                                        sdoAnt = g.Sum(p => p.sdoAnt),
                                                        debito = g.Sum(p => p.debito),
                                                        credito = g.Sum(p => p.credito),

                                                    }).ToList();


                List<BalancoCampos> segundoGrau = (from soma in terceiroGrau.AsEnumerable()


                                                   group soma by new
                                                   {
                                                       numConta = soma.numconta.Substring(0, 2),

                                                       // icodser = soma.ICodSer
                                                   } into g
                                                   select new BalancoCampos
                                                   {
                                                       numconta = g.Key.numConta,
                                                       sdoAntCalc = g.Sum(p => p.sdoAntCalc),
                                                       sdoAnt = g.Sum(p => p.sdoAnt),
                                                       debito = g.Sum(p => p.debito),
                                                       credito = g.Sum(p => p.credito),

                                                   }).ToList();

                List<BalancoCampos> primeiroGrau = (from soma in segundoGrau.AsEnumerable()


                                                    group soma by new
                                                    {
                                                        numConta = soma.numconta.Substring(0, 1),

                                                        // icodser = soma.ICodSer
                                                    } into g
                                                    select new BalancoCampos
                                                    {
                                                        numconta = g.Key.numConta,
                                                        sdoAntCalc = g.Sum(p => p.sdoAntCalc),
                                                        sdoAnt = g.Sum(p => p.sdoAnt),
                                                        debito = g.Sum(p => p.debito),
                                                        credito = g.Sum(p => p.credito),

                                                    }).ToList();

                foreach (var g in quartoGrau)
                {
                    DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "000").FirstOrDefault();
                    if (orow == null)
                    { continue; }
                    orow.BeginEdit();
                    orow["SDOANT"] = Convert.ToDecimal(orow["SDOANT"]) + g.sdoAntCalc;
                    orow["DEBITO"] = g.debito;
                    orow["CREDITO"] = g.credito;
                    orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                foreach (var g in terceiroGrau)
                {
                    DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "00000").FirstOrDefault();
                    if (orow == null)
                    { continue; }
                    orow.BeginEdit();
                    orow["SDOANT"] = Convert.ToDecimal(orow["SDOANT"]) + g.sdoAntCalc;
                    orow["DEBITO"] = g.debito;
                    orow["CREDITO"] = g.credito;
                    orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                foreach (var g in segundoGrau)
                {
                    DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "000000").FirstOrDefault();
                    if (orow == null)
                    { continue; }
                    orow.BeginEdit();
                    orow["SDOANT"] = Convert.ToDecimal(orow["SDOANT"]) + g.sdoAntCalc;
                    orow["DEBITO"] = g.debito;
                    orow["CREDITO"] = g.credito;
                    orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                foreach (var g in primeiroGrau)
                {
                    DataRow orow = Balancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == g.numconta + "0000000").FirstOrDefault();
                    if (orow == null)
                    { continue; }
                    orow.BeginEdit();
                    orow["SDOANT"] = Convert.ToDecimal(orow["SDOANT"]) + g.sdoAntCalc;
                    orow["DEBITO"] = g.debito;
                    orow["CREDITO"] = g.credito;
                    orow["SDO"] = Convert.ToDecimal(orow["SDOANT"]) + g.debito - g.credito;
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                Balancete.AcceptChanges();
            }
            catch (Exception)
            {

                MessageBox.Show("Erro Geração Balancete");
            }
            Cursor.Current = Cursors.Default;
            return Balancete;



        }

        private void btnSaldo_Click(object sender, EventArgs e)
        {
            if (fim.Month != 12) {

                MessageBox.Show("Deve ser fim de ano!!!");
                return; }
            int anoGeral = fim.Year;

            List<string> tabelas = TDataControlContab.EncontreTabelaPlacon();
            if (tabelas.IndexOf("PTPLA" + anoGeral.ToString() + ".DBF") < 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + anoGeral.ToString());
                return;
            }

            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string pla = "PTPLA" + anoGeral.ToString() + ".DBF";
            setoroledb = "SELECT numconta, descricao, sdo, desc2, data FROM " + path + pla +
                       " ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
           
            OleDbCommand cmd = new OleDbCommand("UPDATE " + path + pla + " SET DATA = ?,SDO = ? " +
                          "WHERE  (NUMCONTA = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
            cmd.Parameters.Add("@SDO", OleDbType.Double, 0, "SDO");
           
            OleDbParameter parm;
            parm = cmd.Parameters.Add("@NUMCONTA", OleDbType.Char, 8, "NUMCONTA");
            //parm.SourceVersion = DataRowVersion.Original;

            // da.UpdateCommand = cmd;
            oledbda.UpdateCommand = cmd;

            oledbda.TableMappings.Add("Table", "PTPLA" + anoGeral.ToString());
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            if (odataset.Tables.Count == 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + anoGeral.ToString());
                return;
            }

            System.Data.DataTable result = odataset.Tables[0];


            
            foreach (DataRow orow in result.Rows)
            {
                if (orow["NUMCONTA"].ToString() == "00000000")
                {
                    orow.BeginEdit();
                    orow["DATA"] = fim;
                    orow.EndEdit();
                    continue;
                }
                var row = dtBalancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == orow["NUMCONTA"].ToString()).FirstOrDefault();
                if (row == null)
                {
                    orow.BeginEdit();
                    orow["DATA"] = fim;
                    orow["SDO"] = 0;
                    orow.EndEdit();
                    continue;
                }
                orow.BeginEdit();
                orow["DATA"] = fim;
                orow["SDO"] = Math.Round(Convert.ToDecimal(row["SDO"]),2);
                orow.EndEdit();
                

            }

            int reg = oledbda.Update(odataset);


            // PROCEDIMENTO DE UPDATE

        }

       
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as System.Windows.Forms.CheckBox).Checked)
            {
                exclui_fechamento = true;
            }
            else
            {
                exclui_fechamento = false;
            }
        }

        private void ckSintetico_CheckedChanged(object sender, EventArgs e)
        {
            if ((sender as System.Windows.Forms.CheckBox).Checked)
            {
                sintetico = true;
            }
            else
            {
                sintetico = false;
            }
        }

        private void btnRazao_Click(object sender, EventArgs e)
        {
            var orowdgv = dgvBalancete.CurrentRow;
            DataRowView orow = (orowdgv.DataBoundItem as DataRowView );
            if (orow != null)
            {
                string conta = orow["NUMCONTA"].ToString();
                if (conta.Substring(5, 3) == "000")
                    return;
                string descri = orow["DESCBAL"].ToString();
                decimal sdoant = Convert.ToDecimal(orow["SDOANT"]);
                DateTime inicio = Convert.ToDateTime(orow["INICIO"]);
                DateTime fim = Convert.ToDateTime(orow["FIM"]);
                FrmRazaoContabil oform = new FrmRazaoContabil(dtVauchesRazao, conta, descri, sdoant, inicio, fim);
                oform.Show();
            }
            //oform.Show();
        }

        private void btnRazaoGeral_Click(object sender, EventArgs e)
        {
            FrmRazaoContabilGeral oform = new FrmRazaoContabilGeral(dgvBalancete,dtVauchesRazao);
            oform.Show();
        }

        private void btnSaldoAlter_Click(object sender, EventArgs e)
        {
            string debito = "-1";
            string credito = "-1";

            Relaciona reldebito = null;
            Relaciona relcredito = null;
          /*  if (dictrelaciona.ContainsKey(linexc["DEBITO"].ToString().Trim()))
            {
                reldebito = dictrelaciona[linexc["DEBITO"].ToString().Trim()];
            }
            if (dictrelaciona.ContainsKey(linexc["CREDITO"].ToString().Trim()))
            {
                relcredito = dictrelaciona[linexc["CREDITO"].ToString().Trim()];
            }

            if (rbDesc2.Checked)
            {
                debito = reldebito != null ? reldebito.reduzido.ToString() : debito;
                credito = relcredito != null ? relcredito.reduzido.ToString() : credito;
            }
            else
            {
                debito = reldebito != null ? reldebito.novocod.ToString() : debito;
                credito = relcredito != null ? relcredito.novocod.ToString() : credito;
            }
            Cursor.Current = Cursors.WaitCursor;
            ExportaLoteExcel.ExportaExcelLote(dtVauchesRazao);*/
            Cursor.Current = Cursors.Default;
        }

        private void btnNovasContasPlacon_Click(object sender, EventArgs e)
        {
            int anoGeral = fim.Year;

            List<string> tabelas = TDataControlContab.EncontreTabelaPlacon();
            if (tabelas.IndexOf("PTPLA" + (anoGeral - 1).ToString() + ".DBF") < 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + (anoGeral - 1).ToString());
                return;
            }

            OleDbCommand oledbcomm;
            string setoroledb, path;
            path = TDataControlReduzido.Get_Path("CONTAB");
            string pla = "PTPLA" + (anoGeral - 1).ToString() + "  ano ";
            setoroledb = "SELECT numconta, descricao, sdo, desc2, data FROM " + path 
                 + "PLACON WHERE NOT EXISTS (SELECT ano.NUMCONTA FROM " + path + pla + " WHERE PLACON.NUMCONTA = ano.NUMCONTA) " +
                       " ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            OleDbDataAdapter oledbda = new OleDbDataAdapter(oledbcomm);
            oledbda.TableMappings.Add("Table", "PLACON" );
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);

            if (odataset.Tables.Count == 0)
            {
                MessageBox.Show("Não Encontrado Novos Registros NOVOSPLA1");
                return;
            }

            System.Data.DataTable dtnovos = odataset.Tables[0].Copy();
            if (dtnovos.Rows.Count == 0)
            {
                MessageBox.Show("Não Encontrado Novos Registros NOVOSPLA2");
                return;
            }
            odataset.Dispose();
            oledbda.Dispose();
            string plaInsert = "PTPLA" + (anoGeral - 1).ToString();
            setoroledb = "INSERT INTO " +path+plaInsert + "  SELECT * FROM " + path
                + "PLACON WHERE NOT EXISTS (SELECT ano.NUMCONTA FROM " + path + pla + " WHERE PLACON.NUMCONTA = ano.NUMCONTA) " +
                      " ORDER BY NUMCONTA";
            oledbcomm = new OleDbCommand(setoroledb, TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            int ret =  oledbcomm.ExecuteNonQuery();
            /*

            OleDbCommand cmd = new OleDbCommand("UPDATE " + path + pla + " SET DATA = ?,SDO = ? " +
                          "WHERE  (NUMCONTA = ?)", TDataControlReduzido.ConnectionPooling.GetConnectionOleDb());
            cmd.Parameters.Add("@DATA", OleDbType.Date, 8, "DATA");
            cmd.Parameters.Add("@SDO", OleDbType.Double, 0, "SDO");

            OleDbParameter parm;
            parm = cmd.Parameters.Add("@NUMCONTA", OleDbType.Char, 8, "NUMCONTA");
            //parm.SourceVersion = DataRowVersion.Original;

            // da.UpdateCommand = cmd;
            oledbda.UpdateCommand = cmd;

            oledbda.TableMappings.Add("Table", "PTPLA" + anoGeral.ToString());
            DataSet odataset = new DataSet();
            oledbda.Fill(odataset);
            if (odataset.Tables.Count == 0)
            {
                MessageBox.Show("Não Encontrado Placon com Saldos de " + anoGeral.ToString());
                return;
            }

            System.Data.DataTable result = odataset.Tables[0];



            foreach (DataRow orow in result.Rows)
            {
                if (orow["NUMCONTA"].ToString() == "00000000")
                {
                    orow.BeginEdit();
                    orow["DATA"] = fim;
                    orow.EndEdit();
                    continue;
                }
                var row = dtBalancete.AsEnumerable().Where(a => a.Field<string>("NUMCONTA") == orow["NUMCONTA"].ToString()).FirstOrDefault();
                if (row == null)
                {
                    orow.BeginEdit();
                    orow["DATA"] = fim;
                    orow["SDO"] = 0;
                    orow.EndEdit();
                    continue;
                }
                orow.BeginEdit();
                orow["DATA"] = fim;
                orow["SDO"] = Math.Round(Convert.ToDecimal(row["SDO"]), 2);
                orow.EndEdit();


            }

            int reg = oledbda.Update(odataset);
            */


        }
    }



}
