using ApoioContabilidade.Financeiro.Services;
using ApoioContabilidade.Services;
using ClassConexao;
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

namespace ApoioContabilidade.Financeiro
{
    public partial class FrmTarifasLancAutoBB : Form
    {
       
        BindingSource bmPagar;
        BindingSource bmReceber;
        BindingSource bmExtrato;
        BindingSource bmExtratoBB;
        DataTable Extrato;
        DataTable extratoBB;
        public MonteGrid oExtrato;
        public MonteGrid oExtratoBB;
        private Dictionary<string, string> BBHistorico;


        //  Evt_Fin evt_Fin_Extrato;
        public FrmTarifasLancAutoBB(BindingSource obmPagar,
        BindingSource obmReceber)
        {

            InitializeComponent();
            BBHistorico = new Dictionary<string, string>();
            BBHistorico.Add("438", "TED");
            BBHistorico.Add("978", "TED - Crédito");
            BBHistorico.Add("855", "BB RF CP Aut Empresa"); // CREDITO
            BBHistorico.Add("345", "BB RF CP Aut Empresa"); // DEBITO
            // 	Tarifa Pacote de Serviços
            BBHistorico.Add("257", "Tarifa Unica Exp Internet");
            BBHistorico.Add("170", "Tar Pag Salár Créd Conta");
            BBHistorico.Add("435", "Tarifa Pacote de Serviços");
            BBHistorico.Add("500", "Tarifa Envio OPE");
             







            CriaTabelaVirtualExtratoBB();
            MonteGrids();

            comboBancos.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBancos.KeyPress += ComboBancos_KeyPress;
            bmExtrato = new BindingSource();
            bmExtratoBB = new BindingSource();
            bmPagar = obmPagar;
            bmReceber = obmReceber;
            List<string> bancosNumeros = new List<string>();
            bancosNumeros.Add("04");
            bancosNumeros.Add("14");
            DataTable mov_fin = null;
            try
            {
                mov_fin = (bmPagar.DataSource as DataTable).AsEnumerable().
                       Where(row => bancosNumeros.Contains(row.Field<string>("DEBITO").Trim()) ||
                                    bancosNumeros.Contains(row.Field<string>("CREDITO").Trim())).CopyToDataTable();
                mov_fin.TableName = "MOV_FIN";
            }
            catch (Exception)
            {

            }
            DataTable mov_fin_Receber = null;
            try
            {
                mov_fin_Receber = (bmReceber.DataSource as DataTable).AsEnumerable().
                       Where(row => bancosNumeros.Contains(row.Field<string>("DEBITO").Trim()) ||
                                    bancosNumeros.Contains(row.Field<string>("CREDITO").Trim())).CopyToDataTable();
                mov_fin_Receber.TableName = "MOV_FIN_REC";
            }
            catch (Exception)
            {

            }
            if ((mov_fin != null) && (mov_fin_Receber != null))
            {

                foreach (DataRow orow in mov_fin_Receber.Rows)
                {
                    DataRow orowmovFin = mov_fin.NewRow();
                    foreach (DataColumn ocol in (bmReceber.DataSource as DataTable).Columns)
                    {
                        try
                        {
                            orowmovFin[ocol.ColumnName] = orow[ocol.ColumnName];
                        }
                        catch (Exception)
                        {
                        }
                    }

                    mov_fin.Rows.Add(orowmovFin);
                }
                mov_fin.AcceptChanges();
            }
            else
            {
                if (mov_fin == null)
                {
                    mov_fin = mov_fin_Receber.Copy();
                    mov_fin.TableName = "MOV_FIN";
                }

            }



            try
            {
                dtData2.Value = mov_fin.AsEnumerable().Max(orow => orow.Field<DateTime>("DATA"));
                dtData1.Value = mov_fin.AsEnumerable().Min(orow => orow.Field<DateTime>("DATA"));


                List<string> bcoRec = (from gr in mov_fin.AsEnumerable().Where(row => row.Field<string>("TIPO") == "R")
                                       group gr by new { banco = gr.Field<string>("DEBITO").Trim() } into g
                                       select new
                                       {
                                           g.Key.banco

                                       }).Select(a => a.banco).ToList();
                List<string> bcos = (from gr in mov_fin.AsEnumerable().Where(row => (row.Field<string>("TIPO") == "P") && !bcoRec.Contains(row.Field<string>("CREDITO").Trim()))
                                     group gr by new { banco = gr.Field<string>("CREDITO").Trim() } into g
                                     select new
                                     {
                                         g.Key.banco

                                     }).Select(a => a.banco).ToList();
                bcos.AddRange(bcoRec);


                Extrato = new DataTable("EXTRATO");
                Extrato.Columns.Add("Concilia", Type.GetType("System.Int32"));
                Extrato.Columns.Add("DATA", Type.GetType("System.DateTime"));
                Extrato.Columns.Add("DOC", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 13 + 5;
                Extrato.Columns.Add("FORN", Type.GetType("System.String")); // titular
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 40 + 5;
                Extrato.Columns.Add("VALOR", Type.GetType("System.Decimal"));
                Extrato.Columns.Add("DBCR", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 1;
                Extrato.Columns.Add("SALDO", Type.GetType("System.Decimal"));
                Extrato.Columns.Add("HIST", Type.GetType("System.String")); // historico
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 40 + 5;
                Extrato.Columns.Add("LANC", Type.GetType("System.String")); // lanc.Contabl 7
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 25 + 5;
                Extrato.Columns.Add("CTA", Type.GetType("System.String")); // banco 8
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 2;
                Extrato.Columns.Add("DOC_FISC", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 20;
                Extrato.Columns.Add("OUTRO_ID", Type.GetType("System.Double"));
                Extrato.Columns.Add("MOV_ID", Type.GetType("System.Double"));
                // IRÁ GUARDAR O HISTORICO DO BANCO CONCILIADO
                Extrato.Columns.Add("NHIST", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 3;
                // IRÁ SINALIZAR SE FOI LANCAMENTO GERADO POR EXTRATO BANCÁRIO (TARIFAS E CREDITO AUTOMÁTICO)
                Extrato.Columns.Add("LIVRO", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 2;
                // RESERVADO
                Extrato.Columns.Add("CODIGOF", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 3;


                Extrato.Columns.Add("HISTBB", Type.GetType("System.String"));
                Extrato.Columns[Extrato.Columns.Count - 1].MaxLength = 30;
                Extrato.Columns.Add("DATABB", Type.GetType("System.DateTime"));




                foreach (DataRow orow in mov_fin.Rows)
                {
                    DataRow oext = Extrato.NewRow();
                    oext["DATA"] = orow["DATA"];
                    oext["DOC"] = orow["DOC"];
                    oext["FORN"] = orow["FORN"];
                    oext["HIST"] = orow["HIST"];
                    // lançamentos sinalizadores EXTRATO
                    oext["NHIST"] = orow["NHIST"];
                    oext["LIVRO"] = orow["LIVRO"];

                    oext["OUTRO_ID"] = !orow.IsNull("OUTRO_ID") ? Convert.ToDouble(orow["OUTRO_ID"]) : 0;
                    oext["MOV_ID"] = Convert.ToDouble(orow["MOV_ID"]);
                    oext["DOC_FISC"] = orow["DOC_FISC"];
                    if (bcos.Contains(orow["CREDITO"].ToString().Trim()))
                    {
                        string tdeb = TabelasIniciais.NBancoDesc2(orow["DEBITO"].ToString()).Trim();
                        oext["DBCR"] = "D";
                        oext["VALOR"] = Convert.ToDecimal(orow["VALOR"]);
                        oext["LANC"] = orow["DEBITO"];

                        oext["CTA"] = orow["CREDITO"].ToString().Trim();
                        if (tdeb != "")
                        {
                            oext["FORN"] = tdeb;
                            oext["LANC"] = TabelasIniciais.Desc2Banco(orow["CREDITO"].ToString().Trim()).Trim();
                        }
                        else continue;
                    }
                    else
                    {
                        string tcre = TabelasIniciais.NBancoDesc2(orow["CREDITO"].ToString()).Trim();

                        oext["DBCR"] = "C";
                        oext["VALOR"] = Convert.ToDecimal(orow["VALOR"]);
                        oext["LANC"] = orow["CREDITO"];
                        oext["CTA"] = orow["DEBITO"].ToString().Trim();
                        if (tcre != "")
                        {
                            oext["FORN"] =tcre;
                            oext["LANC"] = TabelasIniciais.Desc2Banco(orow["DEBITO"].ToString().Trim()).Trim();
                        }
                        else continue;

                    }
                    Extrato.Rows.Add(oext);
                }
                Extrato.AcceptChanges();
                BindingSource bmBancos = new BindingSource();
                DataTable bancosAlt;
                try
                {
                    bancosAlt = DadosComum.BancosCombo.AsEnumerable().Where(row => (bcos.Contains(row.Field<string>("NBANCO")))).CopyToDataTable();
                }
                catch (Exception)
                {

                    bancosAlt = DadosComum.BancosCombo.Clone();
                }

                bmBancos.DataSource = bancosAlt.AsDataView();
                comboBancos.DataSource = bmBancos;
                comboBancos.DisplayMember = "DESCRI";
                comboBancos.ValueMember = "NBANCO";
            }
            catch (Exception)
            {
                MessageBox.Show("Dados Vazios");

            }

        }

        private void ComboBancos_KeyPress(object sender, KeyPressEventArgs e)
        {
            //  e.Handled = true;
        }


        #region MonteGrid

        public void MonteGrids()
        {
            oExtrato = new MonteGrid();
            oExtrato.Clear();
            oExtrato.AddValores("DATA", "Data", 11, "", false, 0, "");
            oExtrato.AddValores("VALOR", "Valor(R$)", 13, "##,###,##0.00", true, 0, "");
            oExtrato.AddValores("DBCR", "DC", 3, "", false, 0, "");
            oExtrato.AddValores("FORN", "Titular", 30, "", false, 0, "");
          //  oExtrato.AddValores("SALDO", "Saldo Dia(R$)", 13, "##,###,##0.00", true, 0, "");
            oExtrato.AddValores("LANC", "Lançamento", 30, "", false, 0, "");
            oExtrato.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oExtrato.AddValores("DOC_FISC", "DOC", 13, "", false, 0, "");
            oExtrato.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");

            oExtratoBB = new MonteGrid();
            oExtratoBB.Clear();
            oExtratoBB.AddValores("CONCILIA", "Concilia", 8, "##,###,###", false, 0, "");
            oExtratoBB.AddValores("DATA", "Data", 11, "", false, 0, "");
            oExtratoBB.AddValores("VALOR", "Valor(R$)", 13, "##,###,##0.00", true, 0, "");
            oExtratoBB.AddValores("DBCR", "DC", 3, "", false, 0, "");

            oExtratoBB.AddValores("AGENCIA_ORIG", "Ag.Orig.", 12, "", false, 0, "");
            oExtratoBB.AddValores("LOTE", "Lote", 10, "", false, 0, "");
            oExtratoBB.AddValores("NUMERODOCUMENTO", "N.Documento", 17, "", false, 0, "");

            oExtratoBB.AddValores("CODHISTORICO", "C.Hist", 6, "", false, 0, "");
            oExtratoBB.AddValores("HISTORICO", "Histórico", 25, "", false, 0, "");
            oExtratoBB.AddValores("DETALHEHISTORICO", "Detalhe Histórico", 70, "", false, 0, "");
            oExtratoBB.AddValores("OBSERVACAO", "OBSERVACAO", 100, "", false, 0, "");
            oExtratoBB.AddValores("DATA_BALANCETE", "Data", 11, "", false, 0, "");


        }
        public void MonteGridsBB()
        {
            oExtrato = new MonteGrid();
            oExtrato.Clear();
            oExtrato.AddValores("CONCILIA", "Concilia", 8, "##,###,###", false, 0, "");
            oExtrato.AddValores("HISTBB", "HIST BB", 20, "", false, 0, "");
            oExtrato.AddValores("DATABB", "DATA BB", 20, "", false, 0, "");
            oExtrato.AddValores("DATA", "Data", 11, "", false, 0, "");
            oExtrato.AddValores("VALOR", "Valor(R$)", 13, "##,###,##0.00", true, 0, "");
            oExtrato.AddValores("DBCR", "DC", 3, "", false, 0, "");

            oExtrato.AddValores("FORN", "Titular", 30, "", false, 0, "");
            oExtrato.AddValores("LANC", "Lançamento", 30, "", false, 0, "");
            oExtrato.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oExtrato.AddValores("DOC_FISC", "DOC", 13, "", false, 0, "");
            oExtrato.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
        }

        #endregion

        private void btnConsulta_Click(object sender, EventArgs e)
        {
            //if (comboBancos.SelectedValue == null) return;
           // string banco = comboBancos.SelectedValue.ToString();
            if (Extrato.Rows.Count == 0)
            {
                MessageBox.Show("Dados Vazio!  ...");
                return;
            }
            // bmExtrato.DataSource = null;
            DataTable novoExtrato;
            try
            {
                novoExtrato = Extrato.AsEnumerable().Where(row =>
             //(row.Field<string>("CTA") == banco)
              //&& 
            
            (row.Field<DateTime>("DATA").CompareTo(dtData1.Value) >= 0) &&
                       (row.Field<DateTime>("DATA").CompareTo(dtData2.Value) <= 0)
            ).OrderBy(row =>
                row.Field<DateTime>("DATA")).CopyToDataTable();
            }
            catch (Exception)
            {
                MessageBox.Show("Dados Vazio!  ...");
                return;

            }

            Decimal saldo = 0;
            foreach (DataRow orow in novoExtrato.Rows)
            {
                orow.BeginEdit();
                if (orow["DBCR"].ToString().Trim() == "D")
                {
                    saldo = saldo - Convert.ToDecimal(orow["VALOR"]);
                    orow["SALDO"] = saldo;
                }
                else
                {
                    saldo = saldo + Convert.ToDecimal(orow["VALOR"]);
                    orow["SALDO"] = saldo;
                }
                orow.EndEdit();
            }

            novoExtrato.TableName = "ExtratoMLSA";
            bmExtrato.DataSource = novoExtrato.AsDataView();
            bmExtratoBB.DataSource = null;
            MonteGrids();
            oExtrato.oDataGridView = dgvExtrato;
            oExtrato.oDataGridView.DataSource = bmExtrato;
            oExtrato.ConfigureDBGridView();

        }
        #region CONCILIAÇÃO BB


        private void ChameNovoExtrato_Concilia()
        {
            //string banco = comboBancos.SelectedValue.ToString();

            if (bmExtrato.DataSource == null)
            {
                // bmExtrato.DataSource = null;
                DataTable novoExtrato = Extrato.AsEnumerable().Where(row =>
                // (row.Field<string>("CTA") == banco) &&
                 (row.Field<DateTime>("DATA").CompareTo(dtData1.Value) >= 0) &&
                           (row.Field<DateTime>("DATA").CompareTo(dtData2.Value) <= 0)
                ).OrderBy(row =>
                    row.Field<DateTime>("DATA")).CopyToDataTable();
                novoExtrato.TableName = "ExtratoMLSA";
                bmExtrato.DataSource = novoExtrato.AsDataView();
            }
            MonteGridsBB();
            oExtrato.oDataGridView = dgvExtrato;
            oExtrato.oDataGridView.DataSource = bmExtrato;
            oExtrato.ConfigureDBGridView();

        }


        private void btnBBrasil_Click(object sender, EventArgs e)
        {
            /*string banco = (comboBancos.SelectedValue == null) ? "" : comboBancos.SelectedValue.ToString();
            if (banco.Trim() != "04")
            {
                MessageBox.Show("Selecione BB");
                return;
            }
            */

            if (!pegExtratoVirtualBB()) return;

            bmExtratoBB.DataSource = null;
            DataTable novoExtrato = null;
            try
            {
                novoExtrato = extratoBB.AsEnumerable().Where(row =>
                            //(row.Field<DateTime>("DATA").CompareTo(dtData1.Value) >= 0) &&
                            //(row.Field<DateTime>("DATA").CompareTo(dtData2.Value) <= 0)
                            (row.Field<String>("CODHISTORICO").Trim() != "000")
                           && (row.Field<String>("CODHISTORICO").Trim() != "999")
                           ).OrderBy(row =>
              row.Field<DateTime>("DATA")).CopyToDataTable();

            }
            catch (Exception E)
            {

            }

            if (novoExtrato == null)
            {
                MessageBox.Show("Erro ao carregar planilha Excel");
                return;
            }

            // max e min data sem os saldos
            DateTime data2 = extratoBB.AsEnumerable().Where(orow => (orow.Field<String>("CODHISTORICO").Trim() != "000")
                                                                       && (orow.Field<String>("CODHISTORICO").Trim() != "999")
                                                                    ).Max(orow => orow.Field<DateTime>("DATA"));
            DateTime data1 = extratoBB.AsEnumerable().Where(orow => (orow.Field<String>("CODHISTORICO").Trim() != "000")
                                                                       && (orow.Field<String>("CODHISTORICO").Trim() != "999")
                                                                    ).Min(orow => orow.Field<DateTime>("DATA"));
            // começar sempre no primeiro dia do mes
            data1 = new DateTime(data1.Year, data1.Month, 1);
            if (dtData1.Value.CompareTo(data1) != 0)
                dtData1.Value = data1;
            if (dtData2.Value.CompareTo(data2) != 0)
                dtData2.Value = data2;
            bmExtrato.DataSource = null;
            ChameNovoExtrato_Concilia();

            novoExtrato.TableName = "ExtratoBBFiltro";


            ServiceConcilia.criaAsRelaçoesdeConciliacao(novoExtrato, (bmExtrato.DataSource as DataView).Table);


            oExtrato.oDataGridView = dgvExtrato;
            oExtrato.oDataGridView.DataSource = bmExtrato;
            oExtrato.ConfigureDBGridView();


            bmExtratoBB.DataSource = novoExtrato.AsDataView();
            oExtratoBB.oDataGridView = dgvExtratoBB;
            oExtratoBB.oDataGridView.DataSource = bmExtratoBB;
            oExtratoBB.ConfigureDBGridView();
          

        }





        private void CriaTabelaVirtualExtratoBB()
        {
            extratoBB = new DataTable("extratoBB");

            extratoBB.Columns.Add("Concilia", Type.GetType("System.Int32"));

            extratoBB.Columns.Add("ID", Type.GetType("System.Int32"));

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

        private bool pegExtratoVirtualBB()
        {
            bool result = false;
            if (openFileDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    string nomeplanilha = TDataControlReduzido.ExcelNomePlanilha(openFileDialog1.FileName);
                    DataTable tabexcel = TDataControlReduzido.LeiaExcel(openFileDialog1.FileName, nomeplanilha);
                    if (nomeplanilha.Trim().ToUpper() != "EXTRATO")
                    {
                        MessageBox.Show("Não é extrato!");
                        return result;
                    }
                    Int32 contId = 0;
                    extratoBB.Rows.Clear();
                    extratoBB.AcceptChanges();
                    foreach (DataRow orow in tabexcel.Rows)
                    {
                        bool nulo = false;
                        foreach (DataColumn ocol in orow.Table.Columns)
                        {
                            if (orow.IsNull(ocol.ColumnName))
                            {
                                nulo = true;
                                break;
                            }
                        }
                        if (nulo) continue;
                        DataRow novo = extratoBB.NewRow();
                        novo[0] = 0; // field CONCILIA (FORA DA COPIA DA TAB EXCEL ORIGINAL DO BANCO)
                        // COMEÇA DA COLUNA 2 , porque a 0 é conciliação
                        contId++;
                        novo[1] = contId;


                        int i = 2;
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
                                else if (novo.Table.Columns[i].DataType.FullName == "System.Int32")
                                {
                                    novo[i] = Convert.ToInt32(orow[ocol.ColumnName].ToString().Trim());
                                }
                                else
                                {
                                    MessageBox.Show("Coluna de tipo Não Previsto");
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

                    result = true;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return result;
        }




        #endregion

        private void splitter1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            var spit = e;
        }

    



    }
}
