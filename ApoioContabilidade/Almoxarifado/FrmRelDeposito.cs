using ApoioContabilidade.Almoxarifado.Model;
using ApoioContabilidade.Almoxarifado.ServicesAlmox;
using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Almoxarifado
{
    public partial class FrmRelDeposito : Form
    {
        string deposito = "";
        string item = "";
        DataTable tabMestre;
        DataTable tabDetalhe; // movest

        DataTable qrMovest; // resultado da ida ao servidor

        private BindingSource bmMestre = new BindingSource();
        private BindingSource bmDetalhe = new BindingSource();
     //   private BindingSource bmDepositos = new BindingSource();




      //  private ServAlmox servAlmox;
        private BindingSource bmArmazem;
        private BindingSource bmItens;

        private MonteGrid oMestre;
        private MonteGrid oDetalhe;
        private MonteGrid oDeposito;

        bool necessita_atualizar = false;
        bool novalinhaMestre = false;

        private ServAlmox servAlmox;
        public FrmRelDeposito(ServAlmox oservAlmox,  DateTime data1, DateTime data2)
        {
            servAlmox = oservAlmox;
            InitializeComponent();
            dtData1.Value = data1;
            dtData2.Value = data2;

            dgvDeposito.CellFormatting += PadraoDetalhes_CellFormatting;
            dgvDetalhe.CellFormatting += PadraoDetalhes_CellFormatting;


            bmArmazem = new BindingSource();
            bmArmazem.DataSource = DadosComum.ArmazemCombo.AsEnumerable().Where(row=> 
                 row.Field<string>("DEPOSITO").Trim().CompareTo("20") <= 0 ).AsDataView();

            cbArmazem.DataSource = bmArmazem;  // 
            bmArmazem.Sort = "NOME_DEP ASC";
            cbArmazem.DisplayMember = "NOME_DEP";
            cbArmazem.ValueMember = "DEPOSITO";
            cbArmazem.MaxDropDownItems = 7;
            cbArmazem.DropDownStyle = ComboBoxStyle.DropDown;
            cbArmazem.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbArmazem.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbArmazem.SelectedIndex = -1;
            bmItens = new BindingSource();
            bmItens.DataSource = DadosComum.CadestCombo.Copy().AsDataView();
            cbItens.DataSource = bmItens;
            cbItens.DisplayMember = "DESCRI";
            cbItens.ValueMember = "COD";
            bmItens.Sort = "DESCRI ASC";
            cbItens.MaxDropDownItems = 7;
            cbItens.DropDownStyle = ComboBoxStyle.DropDown;
            cbItens.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbItens.AutoCompleteSource = AutoCompleteSource.ListItems;
            cbItens.SelectedIndex = -1;
            bmMestre.PositionChanged += BmMestre_PositionChanged;
        
        }

        private void BmMestre_PositionChanged(object sender, EventArgs e)
        {
            novalinhaMestre = true;
        }
        private void PadraoDetalhes_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            DataGridView ogrid = ((DataGridView)sender);
            string campo = ogrid.Columns[e.ColumnIndex].Name;
            try
            {
                DataView view = ((ogrid.DataSource as BindingSource).DataSource as DataView);
                if (view == null) return;
                if (!view.Table.Columns.Contains(campo)) return;
                if (view.Table.Columns[campo].DataType == Type.GetType("System.Double"))
                    if (Convert.ToDouble(e.Value) == 0)
                    {
                        e.Value = "";
                    }
            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message);
            }
        }
        private async void tcDados_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (((sender as TabControl).SelectedIndex != 0) && (novalinhaMestre))
            {
                bool espere = await ApronteDetalhe();
                if (!espere)
                    MessageBox.Show("Erro Mesquisa Detalhe");
                else novalinhaMestre = false;
            }
            else
                necessita_atualizar = false;

        }
        private void FrmRelDeposito_Load(object sender, EventArgs e)
        {
           // dtData1.Value = servAlmox.dtData;

           // dtData2.Value = servAlmox.UltimoPonto;
            btnConsulta.PerformClick();
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {

            tcDados.SelectedIndex = 0;
            novalinhaMestre = true;

            if (dtData1.Value > dtData2.Value)
            {
                MessageBox.Show("Inicio maior que Fim ???");
                return;
            }


            if (SqlServer_VFP_Config.tipoAcesso == 1)
            {
                DataSet dsResult = null;
                string str = "";
                List<string> lstquery = new List<string>();
                lstquery.Add("inicio=" + dtData1.Value.ToString("yyyy-MM-dd"));
                lstquery.Add("fim=" + dtData2.Value.ToString("yyyy-MM-dd"));
                lstquery.Add("deposito=" + deposito);
                lstquery.Add("sp_numero=" + "215");

                lstquery.Add(str);
                try
                {
                    dsResult = await ApiServices.Api_QuerySP(lstquery);
                }
                catch (Exception)
                {
                }
                if (dsResult != null)
                {

                    tabMestre = dsResult.Tables[0].Copy();
                    tabMestre.TableName = "MESTRE";
                    qrMovest = dsResult.Tables[1].Copy();
                    qrMovest.TableName = "qrMOVEST";

                }
                else
                {
                    MessageBox.Show("Sem Resultados");
                }
            }
            else
            {
                CrieMestre();
                bool pesq = await RelEstoque(deposito, dtData1.Value, dtData2.Value);
                if (!pesq) MessageBox.Show("Sem Resultados");
            }

            MonteGridMestre();
            
            bmMestre.DataSource = tabMestre.AsEnumerable().Where(row=>(row.Field<double>("SDOANT") != 0) || (row.Field<double>("SDOATUAL") != 0) 
                                                                    ||  (row.Field<double>("TRANSF") != 0)      ).AsDataView();
            oMestre.oDataGridView = dgvMestre;
            oMestre.oDataGridView.DataSource = bmMestre;
      
            oMestre.ConfigureDBGridView();
            BmMestre_PositionChanged(bmMestre, null);
        }

        private void cbItens_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Combo_KeyPress(sender, e);
        }

        private void cbArmazem_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Combo_KeyPress(sender, e);
        }

        private void cbItens_SelectedIndexChanged(object sender, EventArgs e)
        {
            item = "";
            if ((sender as ComboBox).SelectedValue != null)
                item = (sender as ComboBox).SelectedValue.ToString();
        }
        private void cbArmazem_SelectedIndexChanged(object sender, EventArgs e)
        {
            deposito = "";
            if ((sender as ComboBox).SelectedValue != null)
                deposito = (sender as ComboBox).SelectedValue.ToString();

        }


        private async Task<bool> ApronteDetalhe()
        {
            if (bmMestre.Count == 0) return false;
            // item
            dgvDetalhe.Visible = false;
            dgvDeposito.Visible = false;
            DataRowView rowMestre = (bmMestre.Current as DataRowView);
            string cod = rowMestre["COD"].ToString();
            DateTime dtaPrimeiraCompra;
            //DataTable Movest_COD = null;
            DataSet dsDados = null;
            List<string> lst = new List<string>();
            Int64 Id = 0;
           
            // estoques dos depósitos -- anterior ao periodo deste relatório
                string str = "SELECT deposito, SUM(iif(tipo='E',QUANT,QUANT*-1)) as totquant  "
                          + " FROM MOVEST WHERE COD = '" + cod.PadLeft(4) + "'"
              + " AND (DATA >= CTOD('" + Utils.dataRef.ToString("MM/dd/yyyy")
               + "'))  AND (DATA < CTOD('" + dtData1.Value.ToString("MM/dd/yyyy") + "') ) "
               + (deposito.Trim() == "" ? "" : " AND (DEPOSITO = '" + deposito + "') ")
               + "GROUP BY DEPOSITO ORDER BY DEPOSITO";
                lst.Add(str); // 

            // estoques dos depósitos -- no periodo deste relatório

               str = "SELECT DEPOSITO, SUM(QUANT) as totquant FROM MOVEST WHERE COD = '" + cod.PadLeft(4) 
                 +"' AND (DATA >= CTOD('" +
                       dtData1.Value.ToString("MM/dd/yyyy") + "'))  AND "
                       + "(DATA <= CTOD('" +
                       dtData2.Value.ToString("MM/dd/yyyy") + "') ) " +
                " and (DEPOSITO <> '') and (QUANT <> 0) GROUP BY DEPOSITO;"; 

                lst.Add(str); // depositos

            str = "SELECT * FROM ALMOXCUSTOMEDIO  WHERE COD = '" + cod.PadLeft(4) +"' "
              /*+ "' AND (FIM >= CTOD('" +
                    dtData1.Value.ToString("MM/dd/yyyy") + "'))  " +
           */
                + " ORDER BY INICIO";

            lst.Add(str); // depositos



            try
            {
                dsDados = await ApiServices.Api_QueryMulti(lst);
            }
            catch (Exception)
            {
                return false;
            }

            if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
            DataTable somaDepositos = dsDados.Tables[0].Copy();
            DataTable tabDepositos = dsDados.Tables[1].Copy();
            DataTable tabHistCustoMedio_Cod = dsDados.Tables[2].Copy();
            // Todos os depósitos Que tiveram algum movimento e que tenham ainda algum saldo em estoque
            List<string> lstDeposito = new List<string>();
            if (deposito == "")
            {
                foreach (DataRow orowDep in somaDepositos.AsEnumerable().Where(row=>row.Field<double>("TOTQUANT") != 0 ))
                {
                    lstDeposito.Add(orowDep["DEPOSITO"].ToString());
                }
                foreach (DataRow orowDep in tabDepositos.AsEnumerable().Where(row=> !lstDeposito.Contains(row["DEPOSITO"].ToString())) )
                {
                    lstDeposito.Add(orowDep["DEPOSITO"].ToString());
                }
            }
            else
            {

                lstDeposito.Add(deposito.Trim());
            }
            lstDeposito.Sort();
            CrieDetalhe(lstDeposito);
            MonteGridDetalhe(lstDeposito);
            MonteGridDeposito(lstDeposito);
            DataTable adodetalhetot = tabDetalhe.Clone(); ;
            DataRow rowTot = adodetalhetot.NewRow();
            rowTot["DATA"] = DateTime.Now;
            rowTot["TIPO"] = "Totais =>";
            adodetalhetot.Rows.Add(rowTot);

            DataRow rowDet = tabDetalhe.NewRow();
            rowDet["TIPO"] = "Sdo Ant";
            rowDet["ID"] = (Id++);
            tabDetalhe.Rows.Add(rowDet);

            double tquant = Convert.ToDouble(rowMestre["ANT_QUANTENT"]) - Convert.ToDouble(rowMestre["ANT_QUANTSAI"]);
            double tvalor = Convert.ToDouble(rowMestre["ANT_VALORENT"]) - Convert.ToDouble(rowMestre["ANT_VALORSAI"]); ;
            // custoMEDIO deve ser 
            /*DataView  = tabHistCustoMedio_Cod.AsEnumerable().Where(row =>
                      row.Field<string>("COD").Trim() == cod.Trim()
                      && (row.Field<DateTime>("INICIO").CompareTo(dtData1) >= 0)
                      && (row.Field<DateTime>("FIM").CompareTo(dtData2) <= 0)
                      ).OrderBy(row => row.Field<DateTime>("INICIO")).AsDataView();*/
            // DateTime primeiraEntrada = Convert.ToDateTime(tabHistCustoMedio_Cod[0]["INICIO"]);
            double cust_me_almox = 0;
            
            DateTime maxDataFim = tabHistCustoMedio_Cod.AsEnumerable().Max(row => row.Field<DateTime>("FIM"));
            DataTable tabCMedioPeriodo = null;
            if (maxDataFim.CompareTo(dtData1.Value) < 0)
            {
                tabCMedioPeriodo = tabHistCustoMedio_Cod.AsEnumerable().Where(row => row.Field<DateTime>("FIM").CompareTo(maxDataFim) == 0).CopyToDataTable();
            }
            else
                tabCMedioPeriodo = tabHistCustoMedio_Cod.AsEnumerable().Where(row => row.Field<DateTime>("FIM").CompareTo(dtData1.Value) >= 0).CopyToDataTable();

            if ((tabCMedioPeriodo != null) && (tabCMedioPeriodo.Rows.Count > 0))
            {

                cust_me_almox = Convert.ToDouble(tabCMedioPeriodo.AsDataView()[0]["CUSTOMEDIO"]);
                //DataRowView orowSaldoAnterior = tabHistCustoMedio_Cod.Table.AsEnumerable()
            }

            double tcust_me = 0;
            if ((tquant != 0) && (tvalor != 0))
                tcust_me = Math.Round(tvalor / tquant, 4);

            double sdo = 0;
            double sdovalor = 0;

            rowDet.BeginEdit();
            double totTransferidos = 0;
            foreach (DataRow dado in somaDepositos.AsEnumerable().Where(row => row.Field<double>("TOTQUANT") != 0))
            {
                rowDet["SDO" + dado["DEPOSITO"].ToString()] = Convert.ToDouble(dado["TOTQUANT"]);
                if (deposito.Trim() == "")
                {
                    totTransferidos = totTransferidos + Convert.ToDouble(dado["TOTQUANT"]);
                }
            }

            if (deposito.Trim() != "")
            {
                sdo = Convert.ToDouble(rowDet["QUANT" + deposito]);
                sdovalor = Math.Round(sdo * tcust_me, 2);
                rowDet["SDO"] = sdo;
                rowDet["SDOVALOR"] = sdovalor;
                rowDet["PUNIT_CALC"] = tcust_me;
                rowDet["PUNIT"] = cust_me_almox;
            }
            else
            {
                rowDet["SDO"] = tquant;
                rowDet["SDOVALOR"] = tvalor;
                rowDet["PUNIT_CALC"] = tcust_me;
                rowDet["QUANTTRANSF"] = totTransferidos;
                rowDet["PUNIT"] = cust_me_almox;

            }
            rowDet.EndEdit();
            rowDet.AcceptChanges();

            DataRow rowDetInicial = adodetalhetot.NewRow();
            rowDetInicial.BeginEdit();
            foreach (DataColumn ocol in rowDet.Table.Columns)
            {
                if (ocol.DataType == Type.GetType("System.Double"))
                {
                    if ((ocol.ColumnName.ToUpper() == "QUANT") ||
                        (ocol.ColumnName.ToUpper() == "VALOR") ||
                        (ocol.ColumnName.ToUpper() == "PUNIT")) continue;
                    rowDetInicial[ocol.ColumnName] = rowDet[ocol.ColumnName];
                }
            }
            



            rowTot.BeginEdit();
            foreach (DataColumn ocol in rowDet.Table.Columns)
            {
                if (ocol.DataType == Type.GetType("System.Double"))
                {
                    if ((ocol.ColumnName.ToUpper() == "QUANT") ||
                        (ocol.ColumnName.ToUpper() == "VALOR") ||
                        (ocol.ColumnName.ToUpper() == "PUNIT")) continue;
                    rowTot[ocol.ColumnName] = rowDet[ocol.ColumnName];
                }
            }
            rowTot.EndEdit();
            rowTot.AcceptChanges();
            tabDetalhe.AcceptChanges();

            DataTable QrMovEst = qrMovest.Clone();
            if (qrMovest.Rows.Count > 0)
                qrMovest.AsEnumerable().Where(row =>
                             (deposito.Trim() == "" ? true :
                                 (row.Field<string>("DEPOSITO").Trim() == deposito.Trim()))
                             && row.Field<string>("COD").Trim() == cod.Trim()
                             && row.Field<double>("QUANT") >= 0
                             && row.Field<double>("VALOR") >= 0

                             && (row.Field<DateTime>("DATA").CompareTo(dtData1.Value) >= 0)
                             && (row.Field<DateTime>("DATA").CompareTo(dtData2.Value) <= 0)

                             ).OrderBy(row => row.Field<DateTime>("DATA")).ThenBy(row => row.Field<string>("TIPO")).CopyToDataTable(QrMovEst, LoadOption.OverwriteChanges);

            DateTime tdata_cus;
            necessita_atualizar = false;
            rowTot.BeginEdit();
            DataRow orow = null;
            try
            {


                foreach (DataRow rowquery in QrMovEst.Rows)
                {
                    orow = tabDetalhe.NewRow();
                    orow["DATA"] = Convert.ToDateTime(rowquery["DATA"]);
                    orow["ID"] = (Id++);

                    if (rowquery["TIPO2"].ToString().Trim() != "T")
                    {
                        orow["TIPO"] = rowquery["TIPO"];
                        if (rowquery["TIPO"].ToString().Trim() == "E")
                        {
                            if (Convert.ToDouble(rowquery["VALOR"]) != 0)
                                tvalor = tvalor + Convert.ToDouble(rowquery["VALOR"]);
                            else
                                tvalor = tvalor + Math.Round(Convert.ToDouble(rowquery["QUANT"]) * tcust_me, 2);
                            tquant = tquant + Convert.ToDouble(rowquery["QUANT"]);
                            if (deposito.Trim() != "")
                                sdo = sdo + Convert.ToDouble(rowquery["QUANT"]);
                            if ((tquant != 0) && (tvalor != 0))
                            {
                                tcust_me = Math.Round(tvalor / tquant, 4);
                            }
                            tdata_cus = Convert.ToDateTime(rowquery["DATA"]);
                            orow["QUANTENT"] = rowquery["QUANT"];
                            orow["VALORENT"] = rowquery["VALOR"];
                            orow["PUNIT"] = rowquery["PUNIT"];  //Math.Round(Convert.ToDouble(rowquery["VALOR"]) / Convert.ToDouble(rowquery["QUANT"]), 2);
                            orow["PUNIT_CALC"] = tcust_me;
                            rowTot["QUANTENT"] = Convert.ToDouble(rowTot["QUANTENT"])
                                              + Convert.ToDouble(orow["QUANTENT"]);
                            rowTot["VALORENT"] = Convert.ToDouble(rowTot["VALORENT"])
                                              + Convert.ToDouble(orow["VALORENT"]);
                        }
                        else if (rowquery["TIPO"].ToString().Trim() == "S")
                        {
                            tvalor = tvalor - Math.Round(Convert.ToDouble(rowquery["QUANT"]) * tcust_me, 2);

                            tquant = tquant - Convert.ToDouble(rowquery["QUANT"]);
                            if (deposito != "")
                                sdo = sdo - Convert.ToDouble(rowquery["QUANT"]);


                            orow["QUANTSAI"] = rowquery["QUANT"];
                            orow["VALORSAI"] = rowquery["VALOR"];
                            orow["PUNIT"] = rowquery["PUNIT"];
                            orow["PUNIT_CALC"] = tcust_me;

                            if (Convert.ToDouble(orow["PUNIT_CALC"]) != Convert.ToDouble(orow["PUNIT"]))
                            { necessita_atualizar = true; }

                            rowTot["QUANTSAI"] = Convert.ToDouble(rowTot["QUANTSAI"])
                                                        + Convert.ToDouble(orow["QUANTSAI"]);
                            rowTot["VALORSAI"] = Convert.ToDouble(rowTot["VALORSAI"])
                                              + Convert.ToDouble(orow["VALORSAI"]);
                        }

                        if (deposito == "")
                        {
                            orow["SDO"] = tquant;
                            orow["SDOVALOR"] = tvalor;
                        }


                    }
                    else // tipo2 == T
                    {
                        orow["TIPO"] = rowquery["TIPO2"];
                        if (rowquery["TIPO"].ToString().Trim() == "E")
                        {
                            orow["QUANTTRANSF"] = rowquery["QUANT"];
                            if (deposito != "")
                                sdo = sdo + Convert.ToDouble(rowquery["QUANT"]);
                           
                        }
                        else
                        {
                            orow["QUANTTRANSF"] = Convert.ToDouble(rowquery["QUANT"]) * -1;
                            if (deposito != "")
                                sdo = sdo - Convert.ToDouble(rowquery["QUANT"]);
                        }

                    }
                    if (deposito != "")
                    {
                        orow["SDO"] = sdo;
                        sdovalor = Math.Round(sdo * tcust_me, 2);
                        orow["SDOVALOR"] = sdovalor;
                    }

                    if (rowquery["DEPOSITO"].ToString() != "")
                    {
                        if (rowquery["TIPO"].ToString() == "E")
                        {
                            orow["QUANT" + rowquery["DEPOSITO"].ToString()] =
                              Convert.ToDouble(orow["QUANT" + rowquery["DEPOSITO"].ToString()]) + Convert.ToDouble(rowquery["QUANT"]);
                            rowDetInicial["SDO" + rowquery["DEPOSITO"].ToString()] =
                              Convert.ToDouble(rowDetInicial["SDO" + rowquery["DEPOSITO"].ToString()]) + Convert.ToDouble(rowquery["QUANT"]);


                        }
                        else
                        { orow["QUANT" + rowquery["DEPOSITO"].ToString()] =
                              Convert.ToDouble(orow["QUANT" + rowquery["DEPOSITO"].ToString()]) - Convert.ToDouble(rowquery["QUANT"]);
                            rowDetInicial["SDO" + rowquery["DEPOSITO"].ToString()] =
                                 Convert.ToDouble(rowDetInicial["SDO" + rowquery["DEPOSITO"].ToString()]) - Convert.ToDouble(rowquery["QUANT"]);

                        }
                        orow["SDO" + rowquery["DEPOSITO"].ToString()] = rowDetInicial["SDO" + rowquery["DEPOSITO"].ToString()];

                    }


                    rowTot["QUANTTRANSF"] = Convert.ToDouble(rowTot["QUANTTRANSF"])
                                             + Convert.ToDouble(orow["QUANTTRANSF"]);

                    tabDetalhe.Rows.Add(orow);
                    orow.AcceptChanges();
                }
                tabDetalhe.AcceptChanges();

                if (deposito != "")
                {
                    rowTot["SDO"] = sdo;
                    rowTot["SDOVALOR"] = sdovalor;
                    rowTot["PUNIT_CALC"] = tcust_me;
                    if (Convert.ToDouble(rowTot["QUANTENT"]) != 0)
                        rowTot["PUNIT"] = Math.Round(Convert.ToDouble(rowTot["VALORENT"]) / Convert.ToDouble(rowTot["QUANTENT"]), 2);
                   



                }
                else
                {
                    rowTot["SDO"] = tquant;
                    rowTot["SDOVALOR"] = tvalor;
                    rowTot["PUNIT_CALC"] = tcust_me;
                    if (Convert.ToDouble(rowTot["QUANTENT"]) != 0)
                        rowTot["PUNIT"] = Math.Round(Convert.ToDouble(rowTot["VALORENT"]) / Convert.ToDouble(rowTot["QUANTENT"]), 2);
                    foreach (string numero in lstDeposito)
                    {
                        rowTot["SDO" + numero] = rowDetInicial["SDO" + numero];
                    }

                }
                rowTot.EndEdit();

                orow = tabDetalhe.NewRow();
                rowTot["ID"] = (Id++);

                foreach (DataColumn ocol in rowTot.Table.Columns)
                {
                    orow[ocol.ColumnName] = rowTot[ocol.ColumnName];
                }
                
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
            tabDetalhe.Rows.Add(orow);
            orow.AcceptChanges();
        
            tabDetalhe.AcceptChanges();
            dgvDetalhe.Visible = true;
            dgvDeposito.Visible = true;
            
            bmDetalhe.DataSource = tabDetalhe.AsDataView();
            bmDetalhe.Position = 0;
            oDetalhe.oDataGridView = dgvDetalhe;
            oDetalhe.oDataGridView.DataSource = bmDetalhe;
            oDetalhe.ConfigureDBGridView();

            //    bmDepositos.DataSource = tabDetalhe.AsDataView();
           
            oDeposito.oDataGridView = dgvDeposito;
            oDeposito.oDataGridView.DataSource = bmDetalhe;
            oDeposito.ConfigureDBGridView();
            return true;
        }


        private void MonteGridMestre()
        {
            oMestre = new MonteGrid();
            oMestre.Clear();
            oMestre.AddValores("COD", "Código", 8, "", false, 0, "");
            oMestre.AddValores("DESCR", "Descrição", 40, "", false, 0, "");
            oMestre.AddValores("UNID", "Unid", 6, "", false, 0, "");

            oMestre.AddValores("SDOANT", "Sdo Ant", 12, "###,##0.000", true, 0, "");
            oMestre.AddValores("ENTRADA", "Entradas", 12, "###,##0.000", true, 0, "");
            oMestre.AddValores("SAIDA", "Saidas", 12, "###,##0.000", true, 0, "");
            oMestre.AddValores("SDOATUAL", "Saldo", 12, "###,##0.000", true, 0, "");
            oMestre.AddValores("TRANSF", "Transf", 12, "###,##0.000", true, 0, "");
        }

        private void MonteGridDetalhe(List<string> lstDep)
        {
            oDetalhe = new MonteGrid();
            oDetalhe.Clear();
            oDetalhe.AddValores("ID", "Ordem", 6, "##,##0", false, 0, "");
            oDetalhe.AddValores("DATA", "Data", 9, "", false, 0, "");
            
            oDetalhe.AddValores("TIPO", "Tipo", 5, "", false, 0, "");
          
            oDetalhe.AddValores("QUANTENT", "Ent.(Un)", 9, "##,##0.00", true, 0, "");
            oDetalhe.AddValores("VALORENT", "Ent.(R$)", 11, "####,##0.00", true, 0, "");
            oDetalhe.AddValores("QUANTSAI", "Saida(Un)", 9, "##,##0.00", true, 0, "");
            oDetalhe.AddValores("VALORSAI", "Saida(R$)", 11, "####,###.#", true, 0, "");
            oDetalhe.AddValores("PUNIT", "Vlr Unid(R$)", 9, "##,##0.00", true, 0, "");
            oDetalhe.AddValores("PUNIT_CALC", "C.Médio(R$)", 9, "##,##0.00", true, 0, "");
            oDetalhe.AddValores("SDO", "Saldo", 11, "####,##0.00", true, 0, "");
            oDetalhe.AddValores("SDOVALOR", "Saldo(R$)", 12, "#,###,##0.00", true, 0, "");
            oDetalhe.AddValores("QUANTTRANSF", "Transf(Un)", 9, "##,##0.00", true, 0, "");
        }

        private void MonteGridDeposito(List<string> lstDep)
        {
            oDeposito = new MonteGrid();
            oDeposito.Clear();
            oDeposito.AddValores("ID", "Ordem", 6, "####,##0", false, 0, "");
            oDeposito.AddValores("DATA", "Data", 9, "", false, 0, "");
            oDeposito.AddValores("TIPO", "Tipo", 5, "", false, 0, "");
            oDeposito.AddValores("SDO", "Saldo(Un)", 11, "####,##0.00", true, 0, "");
            oDeposito.AddValores("QUANTTRANSF", "Transf(Un)", 9, "##,##0.00", true, 0, "");
            foreach (string numero in lstDep)
            {
                oDeposito.AddValores("QUANT" + numero, "Dep." + numero, 9, "##,##0.00", true, 0, "");
                oDeposito.AddValores("SDO" + numero, "Sdo " + numero, 9, "##,##0.00", true, 0, "");
            }
        }



        private void CrieMestre()
        {
            tabMestre = new DataTable();
            tabMestre.TableName = "MESTRE";
            tabMestre.Columns.Add("COD", Type.GetType("System.String"));
            tabMestre.Columns["COD"].MaxLength = 4;
            tabMestre.Columns.Add("DATA_CAD", Type.GetType("System.DateTime"));
            tabMestre.Columns.Add("DESCR", Type.GetType("System.String"));
            tabMestre.Columns["DESCR"].MaxLength = 40;
            tabMestre.Columns.Add("UNID", Type.GetType("System.String"));
            tabMestre.Columns["UNID"].MaxLength = 3;
          
            tabMestre.Columns.Add("ANT_QUANTENT", Type.GetType("System.Double"));
            tabMestre.Columns.Add("ANT_VALORENT", Type.GetType("System.Double"));
            tabMestre.Columns.Add("ANT_QUANTSAI", Type.GetType("System.Double"));
            tabMestre.Columns.Add("ANT_VALORSAI", Type.GetType("System.Double"));

            tabMestre.Columns.Add("SDOANT", Type.GetType("System.Double"));
            tabMestre.Columns.Add("ENTRADA", Type.GetType("System.Double"));
            tabMestre.Columns.Add("SAIDA", Type.GetType("System.Double"));
            tabMestre.Columns.Add("SDOATUAL", Type.GetType("System.Double"));

            tabMestre.Columns.Add("VLRSDOANT", Type.GetType("System.Double"));
            tabMestre.Columns.Add("VLRENTRADA", Type.GetType("System.Double"));
            tabMestre.Columns.Add("VLRSAIDA", Type.GetType("System.Double"));
            tabMestre.Columns.Add("VLRSDOATUAL", Type.GetType("System.Double"));

            tabMestre.Columns.Add("TRANSF", Type.GetType("System.Double"));

            foreach (DataColumn ocol in tabMestre.Columns)
            {
                if ((ocol.DataType == Type.GetType("System.Double")) || (ocol.DataType == Type.GetType("System.Int64")) )
                    ocol.DefaultValue = 0;
            }
        }
        private void CrieDetalhe(List<string> lstDep)
        {

            tabDetalhe = new DataTable();
            tabDetalhe.TableName = "DETALHE";
            tabDetalhe.Columns.Add("ID", Type.GetType("System.Int64"));

            tabDetalhe.Columns.Add("DATA", Type.GetType("System.DateTime"));
            tabDetalhe.Columns.Add("TIPO", Type.GetType("System.String"));
            tabDetalhe.Columns["TIPO"].MaxLength = 10;

            tabDetalhe.Columns.Add("QUANTENT", Type.GetType("System.Double"));
            tabDetalhe.Columns.Add("VALORENT", Type.GetType("System.Double"));

            tabDetalhe.Columns.Add("QUANTSAI", Type.GetType("System.Double"));
            tabDetalhe.Columns.Add("QUANTTRANSF", Type.GetType("System.Double"));
            tabDetalhe.Columns.Add("VALORSAI", Type.GetType("System.Double"));

            tabDetalhe.Columns.Add("SDO", Type.GetType("System.Double"));
            tabDetalhe.Columns.Add("SDOVALOR", Type.GetType("System.Double"));

            tabDetalhe.Columns.Add("PUNIT", Type.GetType("System.Double"));
            tabDetalhe.Columns.Add("PUNIT_CALC", Type.GetType("System.Double"));

            foreach (string numero in lstDep)
            {
                tabDetalhe.Columns.Add("QUANT" + numero, Type.GetType("System.Double"));
                tabDetalhe.Columns.Add("SDO" + numero, Type.GetType("System.Double"));
            }
            foreach (DataColumn ocol in tabDetalhe.Columns)
            {
                if ((ocol.DataType == Type.GetType("System.Double")) || (ocol.DataType == Type.GetType("System.Int64")))
                    ocol.DefaultValue = 0;
            }
        }


        // Versão DBF
        // Equivalente a store procedure [dbo].[sp_RelDeposito]        

        private async Task<bool> RelEstoque(string tdeposito, DateTime dtData1, DateTime dtData2)
        {
            try
            {

                // if (!MovEstStatic.MovEstOk()) return;
                List<string> lst = new List<string>();
                string str = 
                "Select cadest.Cod,cadest.DESCRI as Descr ,Cadest.UNID, sum(movest.Quant) as tot,sum(movest.VALOR) as totValor "
                 + " FROM Cadest, Movest WHERE (cadest.cod = movest.cod) AND "
                + "MOVEST.DATA BETWEEN CTOD('"+dtData1.ToString("MM/dd/yyyy") 
                +"')  and CTOD('"+dtData2.ToString("MM/dd/yyyy") + "') and (TIPO = 'E') "
                    + (tdeposito.Trim() == "" ? " and(TIPO2 <> 'T') AND " : " DEPOSITO = '" + tdeposito + "' AND ")  
                    + " (movest.QUANT >= 0) AND  (movest.VALOR >= 0) "
                    + " group by cadest.cod,cadest.DESCRI,Cadest.UNID order by cadest.cod; ";
                lst.Add(str);

                str =
                "Select cadest.Cod,cadest.DESCRI as Descr ,Cadest.UNID, sum(movest.Quant) as tot,sum(movest.VALOR) as totValor "
                 + " FROM Cadest, Movest WHERE (cadest.cod = movest.cod) AND "
                + "MOVEST.DATA BETWEEN CTOD('" + dtData1.ToString("MM/dd/yyyy")
                + "')  and CTOD('" + dtData2.ToString("MM/dd/yyyy") + "') and (TIPO = 'S') "
                    + (tdeposito.Trim() == "" ? " and(TIPO2 <> 'T') AND " : " DEPOSITO = '" + tdeposito + "' AND ")
                    + " (movest.QUANT >= 0) AND  (movest.VALOR >= 0) "
                    + " group by cadest.cod,cadest.DESCRI,Cadest.UNID order by cadest.cod; ";
                lst.Add(str);


               str =
              "Select cadest.Cod,cadest.DESCRI as Descr ,Cadest.UNID, sum(movest.Quant) as tot,sum(movest.VALOR) as totValor, "
              + " SUM(iif((tipo2<>'T'),movest.QUANT,0.0000)) as ant_QUANTENT, " 
              + " SUM(iif((tipo2<>'T') ,movest.VALOR,0.00)) as ant_VALORENT " 
              + " FROM Cadest, Movest WHERE (cadest.cod = movest.cod) AND "
              + "MOVEST.DATA < CTOD('" + dtData1.ToString("MM/dd/yyyy")
              + "')  and MOVEST.DATA > CTOD('" + Utils.dataRef.ToString("MM/dd/yyyy") + "') and (TIPO = 'E') "
                + (tdeposito.Trim() == "" ? " and(TIPO2 <> 'T') AND " : " DEPOSITO = '" + tdeposito + "' AND ")
                + " (movest.QUANT >= 0) AND  (movest.VALOR >= 0) "
                + " group by cadest.cod,cadest.DESCRI,Cadest.UNID order by cadest.cod; ";
                lst.Add(str);

                str =
             "Select cadest.Cod,cadest.DESCRI as Descr ,Cadest.UNID, sum(movest.Quant) as tot,sum(movest.VALOR) as totValor, "
             + " SUM(iif((tipo2<>'T'),movest.QUANT,0.0000)) as ant_QUANTSAI, "
             + " SUM(iif((tipo2<>'T') ,movest.VALOR,0.00)) as ant_VALORSAI "
             + " FROM Cadest, Movest WHERE (cadest.cod = movest.cod) AND "
             + "MOVEST.DATA < CTOD('" + dtData1.Date.ToString("MM/dd/yyyy")
             + "')  and MOVEST.DATA > CTOD('" + Utils.dataRef.ToString("MM/dd/yyyy") + "') and (TIPO = 'S') "
               + (tdeposito.Trim() == "" ? " and(TIPO2 <> 'T') AND " : " DEPOSITO = '" + tdeposito + "' AND ")
               + " (movest.QUANT >= 0) AND  (movest.VALOR >= 0) "
               + " group by cadest.cod,cadest.DESCRI,Cadest.UNID order by cadest.cod; ";
                lst.Add(str);


                str =
            "Select Cod ,  SUM(IIF((TIPO='E'),QUANT,QUANT*-1) )   as transf  "
            + " FROM MOVEST WHERE  "
            + "MOVEST.DATA < CTOD('" + dtData2.Date.ToString("MM/dd/yyyy")
            + "')  and MOVEST.DATA > CTOD('" + Utils.dataRef.ToString("MM/dd/yyyy") + "')  "
             + " and(TIPO2 = 'T') "
             + " group by cod order by cod; ";
                lst.Add(str);

                str = "SELECT * FROM MOVEST WHERE " + "(DATA >= CTOD('" +
                       dtData1.Date.ToString("MM/dd/yyyy") + "'))  AND " 
                       + "(DATA <= CTOD('" +
                       dtData2.Date.ToString("MM/dd/yyyy") + "') ) " +
                       " ORDER BY DATA,TIPO";

                lst.Add(str);




                DataSet dsDados = null;
                try
                {
                    dsDados = await ApiServices.Api_QueryMulti(lst);
                }
                catch (Exception)
                {
                    return false;
                }


                if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;

                DataTable qrItensEnt = dsDados.Tables[0].Copy();
                DataTable qrItensSai = dsDados.Tables[1].Copy();
                DataTable qrSdoEnt = dsDados.Tables[2].Copy();
                DataTable qrSdoSai = dsDados.Tables[3].Copy();
                DataTable qrTransf = dsDados.Tables[4].Copy();
                qrMovest = dsDados.Tables[5].Copy();
                qrMovest.TableName = "qrMOVEST";


                tabMestre.Rows.Clear();
                
                foreach(DataRow row in qrItensEnt.Rows)
                {
                    DataRow orow = tabMestre.NewRow();
                    orow["COD"] = row["COD"];
                    orow["DESCR"] = row["DESCR"];
                    orow["UNID"] = row["UNID"];
                    orow["ENTRADA"] = row["TOT"];
                    orow["VLRENTRADA"] = row["TOTVALOR"];
                    orow["SAIDA"] = 0;
                    orow["VLRSAIDA"] = 0;
                    tabMestre.Rows.Add(orow);
                }
                tabMestre.AcceptChanges();

                foreach (DataRow row in qrItensSai.Rows)
                {
                    DataRow orow = null;
                    try
                    {
                        orow = tabMestre.AsEnumerable().Where(rou => rou.Field<string>("COD").Trim() == row.Field<string>("COD").Trim()).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                    }
                    if (orow == null)
                    {
                        orow = tabMestre.NewRow();
                        orow["COD"] = row["COD"];
                        orow["DESCR"] = row["DESCR"];
                        orow["UNID"] = row["UNID"];
                        orow["ENTRADA"] = 0;
                        orow["VLRENTRADA"] = 0;
                        


                        tabMestre.Rows.Add(orow);
                        orow.AcceptChanges();
                    }
                    orow.BeginEdit();
                    orow["SAIDA"] = row["TOT"];
                    orow["VLRSAIDA"] = row["TOTVALOR"];
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                tabMestre.AcceptChanges();

                foreach (DataRow row in qrSdoEnt.Rows)
                {
                    DataRow orow = null;
                    try
                    {
                        orow = tabMestre.AsEnumerable().Where(rou => rou.Field<string>("COD").Trim() == row.Field<string>("COD").Trim()).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                    }
                    if (orow == null)
                    {
                        orow = tabMestre.NewRow();
                        orow["COD"] = row["COD"];
                        orow["DESCR"] = row["DESCR"];
                        orow["UNID"] = row["UNID"];
                        orow["ENTRADA"] = 0;
                        orow["VLRENTRADA"] = 0;
                        orow["SAIDA"] = 0;
                        orow["VLRSAIDA"] = 0;
                        tabMestre.Rows.Add(orow);
                        orow.AcceptChanges();
                    }
                    orow.BeginEdit();
                    orow["ANT_QUANTENT"] = row["ANT_QUANTENT"];
                    orow["ANT_VALORENT"] = row["ANT_VALORENT"];
                    orow["SDOANT"] = row["TOT"];
                    orow["VLRSDOANT"] = row["TOTVALOR"];
                    // orow["SDOANT"] = Math.Round(dado.SdoEnt_TOT - dado.SdoSai_TOT, 2);
                    // orow["VLRSDOANT"] = Math.Round(dado.SdoEnt_TOTVALOR - dado.SdoSai_TOTVALOR, 2);

                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                tabMestre.AcceptChanges();

                foreach (DataRow row in qrSdoSai.Rows)
                {
                    DataRow orow = null;
                    try
                    {
                        orow = tabMestre.AsEnumerable().Where(rou => rou.Field<string>("COD").Trim() == row.Field<string>("COD").Trim()).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                    }
                    if (orow == null)
                    {
                        orow = tabMestre.NewRow();
                        orow["COD"] = row["COD"];
                        orow["DESCR"] = row["DESCR"];
                        orow["UNID"] = row["UNID"];
                        orow["ENTRADA"] = 0;
                        orow["VLRENTRADA"] = 0;
                        orow["SAIDA"] = 0;
                        orow["VLRSAIDA"] = 0;
                        orow["ANT_QUANTENT"] = 0;
                        orow["ANT_VALORENT"] = 0;
                        orow["SDOANT"] = 0;
                        orow["VLRSDOANT"] = 0;

                        tabMestre.Rows.Add(orow);
                        orow.AcceptChanges();
                    }
                    orow.BeginEdit();
                    orow["ANT_QUANTSAI"] = row["ANT_QUANTSAI"];
                    orow["ANT_VALORSAI"] = row["ANT_VALORSAI"];
                    orow["SDOANT"] = Math.Round(Convert.ToDouble(orow["SDOANT"]) - Convert.ToDouble(row["TOT"]), 2); 
                    orow["VLRSDOANT"] = Math.Round(Convert.ToDouble(orow["VLRSDOANT"]) - Convert.ToDouble(row["TOTVALOR"]), 2); 
                    orow.EndEdit();
                    orow.AcceptChanges();
                }
                tabMestre.AcceptChanges();

                foreach(DataRow orow in tabMestre.Rows)
                {
                    orow.BeginEdit();
                    orow["SDOATUAL"] = Math.Round(Convert.ToDouble(orow["SDOANT"]) +
                        Convert.ToDouble(orow["ENTRADA"]) - Convert.ToDouble(orow["SAIDA"]), 2);
                    orow["VLRSDOATUAL"] = Math.Round(Convert.ToDouble(orow["VLRSDOANT"]) +
                        Convert.ToDouble(orow["VLRENTRADA"]) - Convert.ToDouble(orow["VLRSAIDA"]), 2);
                    DataRow rowTransf = null;
                    try
                    {
                        rowTransf = tabMestre.AsEnumerable().Where(rou => rou.Field<string>("COD").Trim() == orow.Field<string>("COD").Trim()).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                    }

                    orow["TRANSF"] = 0;
                    if (rowTransf != null)
                        orow["TRANSF"] = rowTransf["TRANSF"];

                    orow.EndEdit();
                    orow.AcceptChanges();

                }
                tabMestre.AcceptChanges();
                return true;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
        }
    }
}
/*   ANTIGO APRONTEDETALHE ANTES DO CUSTOALMOX
 *  private async Task<bool> ApronteDetalhe()
        {
            if (bmMestre.Count == 0) return false;
            // item
            dgvDetalhe.Visible = false;
            dgvDeposito.Visible = false;
            DataRowView rowMestre = (bmMestre.Current as DataRowView);
            string cod = rowMestre["COD"].ToString();
            DateTime dtaPrimeiraCompra;
            //DataTable Movest_COD = null;
            DataSet dsDados = null;
            List<string> lst = new List<string>();
Int64 Id = 0;

string str = "SELECT deposito, SUM(iif(tipo='E',QUANT,QUANT*-1)) as totquant  "
          + " FROM MOVEST WHERE COD = '" + cod.PadLeft(4) + "'"
+ " AND (DATA >= CTOD('" + Utils.dataRef.ToString("MM/dd/yyyy")
+ "'))  AND (DATA < CTOD('" + dtData1.Value.ToString("MM/dd/yyyy") + "') ) "
//  + " AND (QUANT > 0) AND (VALOR > 0 ) "
+ (deposito.Trim() == "" ? "" : " AND (DEPOSITO = '" + deposito + "') ")
+ "GROUP BY DEPOSITO ORDER BY DEPOSITO";
lst.Add(str); // somaDepositos

str = "SELECT MIN(DATA) dtaPrimeiraCompra FROM MOVEST WHERE COD = '" + cod.PadLeft(4) + "' " +
       " AND TIPO = 'E' AND TIPO2 != '';";
lst.Add(str); // dataPrimeiraCompra

str = "SELECT DEPOSITO, SUM(QUANT) as totquant FROM MOVEST WHERE COD = '" + cod.PadLeft(4)
 + "' AND (DATA >= CTOD('" +
       dtData1.Value.ToString("MM/dd/yyyy") + "'))  AND "
       + "(DATA <= CTOD('" +
       dtData2.Value.ToString("MM/dd/yyyy") + "') ) " +
" and (DEPOSITO <> '') and (QUANT <> 0) GROUP BY DEPOSITO;";

lst.Add(str); // depositos
try
{
    dsDados = await ApiServices.Api_QueryMulti(lst);
}
catch (Exception)
{
    return false;
}

if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
DataTable somaDepositos = dsDados.Tables[0].Copy();
DataTable tabPrimeiraCompra = dsDados.Tables[1].Copy();
DataTable tabDepositos = dsDados.Tables[2].Copy();

if (tabPrimeiraCompra.Rows.Count == 0) return false;
dtaPrimeiraCompra = Convert.ToDateTime(tabPrimeiraCompra.Rows[0]["dtaPrimeiraCompra"]);


List<string> lstDeposito = new List<string>();
if (deposito == "")
{
    foreach (DataRow orowDep in somaDepositos.AsEnumerable().Where(row => row.Field<double>("TOTQUANT") != 0))
    {
        lstDeposito.Add(orowDep["DEPOSITO"].ToString());
    }
    foreach (DataRow orowDep in tabDepositos.AsEnumerable().Where(row => !lstDeposito.Contains(row["DEPOSITO"].ToString())))
    {
        lstDeposito.Add(orowDep["DEPOSITO"].ToString());
    }


}
else
{

    lstDeposito.Add(deposito.Trim());
}
lstDeposito.Sort();
CrieDetalhe(lstDeposito);
MonteGridDetalhe(lstDeposito);
MonteGridDeposito(lstDeposito);
DataTable adodetalhetot = tabDetalhe.Clone(); ;
DataRow rowTot = adodetalhetot.NewRow();
rowTot["DATA"] = DateTime.Now;
rowTot["TIPO"] = "Totais =>";
adodetalhetot.Rows.Add(rowTot);

DataRow rowDet = tabDetalhe.NewRow();
// rowDet["DATA"] = DateTime.Now;
rowDet["TIPO"] = "Sdo Ant";
rowDet["ID"] = (Id++);
tabDetalhe.Rows.Add(rowDet);

double tquant = Convert.ToDouble(rowMestre["ANT_QUANTENT"]) - Convert.ToDouble(rowMestre["ANT_QUANTSAI"]);
double tvalor = Convert.ToDouble(rowMestre["ANT_VALORENT"]) - Convert.ToDouble(rowMestre["ANT_VALORSAI"]); ;

double tcust_me = 0; // ptROUND(tVALOR/tQUANT,4);
if ((tquant != 0) && (tvalor != 0))
    tcust_me = Math.Round(tvalor / tquant, 4);

double sdo = 0;
double sdovalor = 0;

rowDet.BeginEdit();
double totTransferidos = 0;
foreach (DataRow dado in somaDepositos.AsEnumerable().Where(row => row.Field<double>("TOTQUANT") != 0))
{
    rowDet["QUANT" + dado["DEPOSITO"].ToString()] = Convert.ToDouble(dado["TOTQUANT"]);
    if (deposito.Trim() == "")
    {
        totTransferidos = totTransferidos + Convert.ToDouble(dado["TOTQUANT"]);
    }
}

if (deposito.Trim() != "")
{
    sdo = Convert.ToDouble(rowDet["QUANT" + deposito]);
    sdovalor = Math.Round(sdo * tcust_me, 2);
    rowDet["SDO"] = sdo;
    rowDet["SDOVALOR"] = sdovalor;
    rowDet["PUNIT_CALC"] = tcust_me;
}
else
{
    rowDet["SDO"] = tquant;
    rowDet["SDOVALOR"] = tvalor;
    rowDet["PUNIT_CALC"] = tcust_me;
    rowDet["QUANTTRANSF"] = totTransferidos;
}
rowDet.EndEdit();
rowDet.AcceptChanges();

rowTot.BeginEdit();
foreach (DataColumn ocol in rowDet.Table.Columns)
{
    if (ocol.DataType == Type.GetType("System.Double"))
    {
        if ((ocol.ColumnName.ToUpper() == "QUANT") ||
            (ocol.ColumnName.ToUpper() == "VALOR") ||
            (ocol.ColumnName.ToUpper() == "PUNIT")) continue;
        rowTot[ocol.ColumnName] = rowDet[ocol.ColumnName];
    }
}
rowTot.EndEdit();
rowTot.AcceptChanges();
tabDetalhe.AcceptChanges();

DataTable QrMovEst = qrMovest.Clone();
if (qrMovest.Rows.Count > 0)
    qrMovest.AsEnumerable().Where(row =>
                 (deposito.Trim() == "" ? true :
                     (row.Field<string>("DEPOSITO").Trim() == deposito.Trim()))
                 && row.Field<string>("COD").Trim() == cod.Trim()
                 && row.Field<double>("QUANT") >= 0
                 && row.Field<double>("VALOR") >= 0

                 && (row.Field<DateTime>("DATA").CompareTo(dtData1.Value) >= 0)
                 && (row.Field<DateTime>("DATA").CompareTo(dtData2.Value) <= 0)

                 ).OrderBy(row => row.Field<DateTime>("DATA")).ThenBy(row => row.Field<string>("TIPO")).CopyToDataTable(QrMovEst, LoadOption.OverwriteChanges);

DateTime tdata_cus;
necessita_atualizar = false;
rowTot.BeginEdit();
DataRow orow = null;
try
{


    foreach (DataRow rowquery in QrMovEst.Rows)
    {
        orow = tabDetalhe.NewRow();
        orow["DATA"] = Convert.ToDateTime(rowquery["DATA"]);
        orow["ID"] = (Id++);

        if (rowquery["TIPO2"].ToString().Trim() != "T")
        {
            orow["TIPO"] = rowquery["TIPO"];
            if (rowquery["TIPO"].ToString().Trim() == "E")
            {
                if (Convert.ToDouble(rowquery["VALOR"]) != 0)
                    tvalor = tvalor + Convert.ToDouble(rowquery["VALOR"]);
                else
                    tvalor = tvalor + Math.Round(Convert.ToDouble(rowquery["QUANT"]) * tcust_me, 2);
                tquant = tquant + Convert.ToDouble(rowquery["QUANT"]);
                if (deposito.Trim() != "")
                    sdo = sdo + Convert.ToDouble(rowquery["QUANT"]);
                if ((tquant != 0) && (tvalor != 0))
                {
                    tcust_me = Math.Round(tvalor / tquant, 4);
                }
                tdata_cus = Convert.ToDateTime(rowquery["DATA"]);
                orow["QUANTENT"] = rowquery["QUANT"];
                orow["VALORENT"] = rowquery["VALOR"];
                orow["PUNIT"] = Math.Round(Convert.ToDouble(rowquery["VALOR"]) / Convert.ToDouble(rowquery["QUANT"]), 2);
                orow["PUNIT_CALC"] = tcust_me;
                rowTot["QUANTENT"] = Convert.ToDouble(rowTot["QUANTENT"])
                                  + Convert.ToDouble(orow["QUANTENT"]);
                rowTot["VALORENT"] = Convert.ToDouble(rowTot["VALORENT"])
                                  + Convert.ToDouble(orow["VALORENT"]);
            }
            else if (rowquery["TIPO"].ToString().Trim() == "S")
            {
                tvalor = tvalor - Math.Round(Convert.ToDouble(rowquery["QUANT"]) * tcust_me, 2);

                tquant = tquant - Convert.ToDouble(rowquery["QUANT"]);
                if (deposito != "")
                    sdo = sdo - Convert.ToDouble(rowquery["QUANT"]);


                orow["QUANTSAI"] = rowquery["QUANT"];
                orow["VALORSAI"] = rowquery["VALOR"];
                orow["PUNIT"] = rowquery["PUNIT"];
                orow["PUNIT_CALC"] = tcust_me;

                if (Convert.ToDouble(orow["PUNIT_CALC"]) != Convert.ToDouble(orow["PUNIT"]))
                { necessita_atualizar = true; }

                rowTot["QUANTSAI"] = Convert.ToDouble(rowTot["QUANTSAI"])
                                            + Convert.ToDouble(orow["QUANTSAI"]);
                rowTot["VALORSAI"] = Convert.ToDouble(rowTot["VALORSAI"])
                                  + Convert.ToDouble(orow["VALORSAI"]);
            }

            if (deposito == "")
            {
                orow["SDO"] = tquant;
                orow["SDOVALOR"] = tvalor;
            }


        }
        else // tipo2 == T
        {
            orow["TIPO"] = rowquery["TIPO2"];
            if (rowquery["TIPO"].ToString().Trim() == "E")
            {
                orow["QUANTTRANSF"] = rowquery["QUANT"];
                if (deposito != "")
                    sdo = sdo + Convert.ToDouble(rowquery["QUANT"]);

            }
            else
            {
                orow["QUANTTRANSF"] = Convert.ToDouble(rowquery["QUANT"]) * -1;
                if (deposito != "")
                    sdo = sdo - Convert.ToDouble(rowquery["QUANT"]);
            }

        }
        if (deposito != "")
        {
            orow["SDO"] = sdo;
            sdovalor = Math.Round(sdo * tcust_me, 2);
            orow["SDOVALOR"] = sdovalor;
        }

        if (rowquery["DEPOSITO"].ToString() != "")
        {
            if (rowquery["TIPO"].ToString() == "E")
                orow["QUANT" + rowquery["DEPOSITO"].ToString()] =
                  Convert.ToDouble(orow["QUANT" + rowquery["DEPOSITO"].ToString()]) + Convert.ToDouble(rowquery["QUANT"]);
            else
                orow["QUANT" + rowquery["DEPOSITO"].ToString()] =
                   Convert.ToDouble(orow["QUANT" + rowquery["DEPOSITO"].ToString()]) - Convert.ToDouble(rowquery["QUANT"]);
        }


        rowTot["QUANTTRANSF"] = Convert.ToDouble(rowTot["QUANTTRANSF"])
                                 + Convert.ToDouble(orow["QUANTTRANSF"]);

        tabDetalhe.Rows.Add(orow);
        orow.AcceptChanges();
    }
    tabDetalhe.AcceptChanges();
    if (deposito != "")
    {
        rowTot["SDO"] = sdo;
        rowTot["SDOVALOR"] = sdovalor;
        rowTot["PUNIT_CALC"] = tcust_me;
        if (Convert.ToDouble(rowTot["QUANTENT"]) != 0)
            rowTot["PUNIT"] = Math.Round(Convert.ToDouble(rowTot["VALORENT"]) / Convert.ToDouble(rowTot["QUANTENT"]), 2);

    }
    else
    {
        rowTot["SDO"] = tquant;
        rowTot["SDOVALOR"] = tvalor;
        rowTot["PUNIT_CALC"] = tcust_me;
        if (Convert.ToDouble(rowTot["QUANTENT"]) != 0)
            rowTot["PUNIT"] = Math.Round(Convert.ToDouble(rowTot["VALORENT"]) / Convert.ToDouble(rowTot["QUANTENT"]), 2);
    }
    rowTot.EndEdit();

    orow = tabDetalhe.NewRow();
    rowTot["ID"] = (Id++);

    foreach (DataColumn ocol in rowTot.Table.Columns)
    {
        orow[ocol.ColumnName] = rowTot[ocol.ColumnName];
    }
}
catch (Exception E)
{
    MessageBox.Show(E.Message);
    return false;
}
tabDetalhe.Rows.Add(orow);
orow.AcceptChanges();

tabDetalhe.AcceptChanges();
dgvDetalhe.Visible = true;
dgvDeposito.Visible = true;

bmDetalhe.DataSource = tabDetalhe.AsDataView();
oDetalhe.oDataGridView = dgvDetalhe;
oDetalhe.oDataGridView.DataSource = bmDetalhe;
oDetalhe.ConfigureDBGridView();

bmDepositos.DataSource = tabDetalhe.AsDataView();
oDeposito.oDataGridView = dgvDeposito;
oDeposito.oDataGridView.DataSource = bmDepositos;
oDeposito.ConfigureDBGridView();
return true;
        }


 */
