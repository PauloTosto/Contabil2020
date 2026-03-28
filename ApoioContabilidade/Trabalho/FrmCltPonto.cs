using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.ConfigComponentes;
using ApoioContabilidade.Trabalho.FormsPesquisa;
using ApoioContabilidade.Trabalho.PegDadosCltCad;
using ApoioContabilidade.Trabalho.ServicesTrab;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    public partial class FrmCltPonto : Form
    {
        ServicoFiltroTrab evt_Ponto;

        EditaCltPonto comum;
        public FrmConsultaTrab frmConsultaTrab;
        public bool modelTrabAnterior = false;
        public bool administrador = false;
        public FrmCltPonto(ServicoFiltroTrab oevt_Ponto)
        {
            evt_Ponto = oevt_Ponto;


            InitializeComponent();

            

            // maior numero de caracteres no nome do trabalhador
            txNome.MaxLength = evt_Ponto.dtTrabCad.Columns["NOMECAD"].MaxLength;

            this.KeyPreview = true;
            this.KeyDown += FrmCltPonto_KeyDown;
            comum = new EditaCltPonto(evt_Ponto);
            comum.MonteGrids();

            dgvTrabMovAnt.MultiSelect = true;
            dgvTrabMovAnt.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTrabMov.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTrabMov.MultiSelect = true;
            dgvNoturno.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvNoturno.MultiSelect = true;


            dgvTrabMov.CellFormatting += DgvTrabMov_CellFormatting;
            dgvTrabMovAnt.CellFormatting += DgvTrabMov_CellFormatting;
            dgvNoturno.CellFormatting += DgvTrabMov_CellFormatting;
            dgvPremio.CellFormatting += DgvTrabMov_CellFormatting;

            dgvTrabalhadores.CellFormatting += DgvTrabalhadores_CellFormatting;


            dgvTrabMov.Enter += color_Enter;
            dgvTrabMov.Leave += color_Leave;

            dgvTrabMovAnt.Enter += color_Enter;
            dgvTrabMovAnt.Leave += color_Leave;

            dgvNoturno.Enter += color_Enter;
            dgvNoturno.Leave += color_Leave;

            dgvPremio.Enter += color_Enter;
            dgvPremio.Leave += color_Leave;


            dgvTrabalhadores.Enter += color_Enter;
            dgvTrabalhadores.Leave += color_Leave;

            tsEdite.Enabled = false;
            tsNovo.Enabled = true;
            tsDelete.Enabled = false;

            if (!DadosComum.tabelasJaConfiguradas())
                DadosComum.TabelasConfigCombos();
        }

       
        private void FrmCltPonto_Load(object sender, EventArgs e)
        {
            // se o modelo fôr TrabalhoAnterior 


            evt_Ponto.bmTrabCad.DataSource =
                evt_Ponto.dtTrabCad.AsEnumerable().OrderBy(
                            row => row.Field<string>("NOMECAD")).AsDataView();
               
            evt_Ponto.bmTrabMov.DataSource = evt_Ponto.dtTrabMov.AsEnumerable().Where(row =>
                     row.IsNull("ID")).AsDataView();
            
            evt_Ponto.bmTrabNoturno.DataSource = evt_Ponto.dtTrabNoturno.AsEnumerable().Where(row =>
                     row.IsNull("ID")).AsDataView();
            // abril 2021
            evt_Ponto.bmTrabPremio.DataSource = evt_Ponto.dtTrabPremio.AsEnumerable().Where(row =>
                     row.IsNull("ID")).AsDataView();

            evt_Ponto.bmTrabMovAnt.DataSource = evt_Ponto.dtTrabMovAnt.AsEnumerable().Where(row =>
                     row.IsNull("ID")).AsDataView();
            // evt_Ponto.bmTrabMovNotAnt.DataSource = evt_Ponto.dtTrabMovNotAnt.AsEnumerable().Where(row =>
            //      row.IsNull("ID")).AsDataView();




            comum.oTrabCad.oDataGridView = dgvTrabalhadores;
            comum.oTrabCad.oDataGridView.DataSource = evt_Ponto.bmTrabCad;

            comum.oTrabMov.oDataGridView = dgvTrabMov;
            comum.oTrabMov.oDataGridView.DataSource = evt_Ponto.bmTrabMov;

            comum.oTrabNoturno.oDataGridView = dgvNoturno;
            comum.oTrabNoturno.oDataGridView.DataSource = evt_Ponto.bmTrabNoturno;

            comum.oTrabPremio.oDataGridView = dgvPremio;
            comum.oTrabPremio.oDataGridView.DataSource = evt_Ponto.bmTrabPremio;


            comum.oTrabMovAnterior.oDataGridView = dgvTrabMovAnt;
            comum.oTrabMovAnterior.oDataGridView.DataSource = evt_Ponto.bmTrabMovAnt;

            comum.oTrabCad.ConfigureDBGridView();
            comum.oTrabMov.ConfigureDBGridView();
            comum.oTrabNoturno.ConfigureDBGridView();
            comum.oTrabMovAnterior.ConfigureDBGridView();
            comum.oTrabPremio.ConfigureDBGridView();

            evt_Ponto.comum = comum;

            evt_Ponto.dtTrabMov.TableNewRow += evt_Ponto.DtTrabMov_TableNewRow;
            evt_Ponto.dtTrabNoturno.TableNewRow += evt_Ponto.DtTrabMovNoturno_TableNewRow;
            evt_Ponto.dtTrabPremio.TableNewRow += evt_Ponto.DtTrabMovPremio_TableNewRow;


            MonteEdits();

            if (evt_Ponto.bmTrabCad.Count == 0)
            {
                evt_Ponto.bmTrabCad.PositionChanged -= evt_Ponto.BmTrabCad_PositionChanged;
                evt_Ponto.bmTrabCad.PositionChanged += evt_Ponto.BmTrabCad_PositionChanged;

                evt_Ponto.BmTrabCad_PositionChanged(evt_Ponto.bmTrabCad, null);
            }
            else
            {
                evt_Ponto.bmTrabCad.PositionChanged -= evt_Ponto.BmTrabCad_PositionChanged;
                evt_Ponto.bmTrabCad.PositionChanged += evt_Ponto.BmTrabCad_PositionChanged;
                evt_Ponto.bmTrabCad.MoveFirst();
                evt_Ponto.BmTrabCad_PositionChanged(evt_Ponto.bmTrabCad, null);
                //bmMestre.ResetCurrentItem();
            }
            comum.oTrabCad.ssTotal = ssTrabalhadores;
            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();

        }

       
        private void FrmCltPonto_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            //{

            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.C:
                        tsCopiar.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.V:
                        tsColar.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.N:
                        tsNovo.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.E:
                        tsEdite.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.D:
                        tsDelete.PerformClick();
                        e.Handled = true;
                        break;
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {

            if (keyData == (Keys.Alt | Keys.D1))
                if (keyData == (Keys.Alt | Keys.D1))
                {
                    if (tcCltPonto.SelectedIndex == 0)
                        dgvTrabMov.Focus();
                    else
                        tcCltPonto.SelectTab(0);
                    return true;
                }
                else if (keyData == (Keys.Alt | Keys.D2))
                {
                    tcCltPonto.SelectTab(1);
                    return true;
                }
                else if (keyData == (Keys.Alt | Keys.D3))
                {
                    tcCltPonto.SelectTab(2);
                    return true;
                }
                else if (keyData == (Keys.Alt | Keys.T))
                {
                    dgvTrabalhadores.Focus();
                    return true;
                }
            /*else if (keyData == (Keys.ControlKey | Keys.C))
            {
                tsCopiar.PerformClick();
                return true;
            }
            else if (keyData == (Keys.ControlKey | Keys.V))
            {
                tsColar.PerformClick();
                return true;
            }*/


            return base.ProcessDialogKey(keyData);
        }


        private void DgvTrabMov_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("NUM_MOD"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.ModeloDesc(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("CODSER"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.ServicoDesc(stringValue);
            }
        }
        private void DgvTrabalhadores_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("APONTAMENTOPROV"))
            {
                if (e.Value.ToString().Trim() != "")
                {
                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
                    row.DefaultCellStyle.BackColor = Color.LightYellow; 
                
                }
                else
                {
                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
                    row.DefaultCellStyle.BackColor = Color.White;
                }

            }

        }

        private void MonteEdits()
        {
            bool ok = comum.MonteEditCltPonto(evt_Ponto.bmTrabMov);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Trab Diurno"); return; }
            comum.EdtTrabMov.BeforeAlteraRegistros += evt_Ponto.DtTrabMov_BeforeAlteraRegistros;
            
            // comum.EdtTrabMov.AsyncAlteraRegistrosOk += evt_Ponto.DtTrabMov__AlteraRegistrosOk;
            comum.EdtTrabMov.AlteraRegistrosOk += evt_Ponto.DtTrabMov__AlteraRegistrosOk;
            comum.EdtTrabMov.DeletaRegistrosOk += evt_Ponto.Padrao_DeletaRegistrosOk;
            comum.EdtTrabMov.AfterDeletaRegistros += evt_Ponto.Padrao_AfterDeletaRegistros;

            ok = comum.MonteEditCltPontoNoturno(evt_Ponto.bmTrabNoturno);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Trab Noturno"); return; }
            comum.EdtTrabNoturno.BeforeAlteraRegistros += evt_Ponto.DtTrabMov_BeforeAlteraRegistros;
            comum.EdtTrabNoturno.AlteraRegistrosOk += evt_Ponto.DtTrabMov__AlteraRegistrosOk;
            comum.EdtTrabNoturno.DeletaRegistrosOk += evt_Ponto.Padrao_DeletaRegistrosOk;
            comum.EdtTrabNoturno.AfterDeletaRegistros += evt_Ponto.Padrao_AfterDeletaRegistros;

            // abril 2021
            ok = comum.MonteEditCltPontoPremio(evt_Ponto.bmTrabPremio);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Trab Premio"); return; }
            comum.EdtTrabPremio.BeforeAlteraRegistros += evt_Ponto.DtTrabMov_BeforeAlteraRegistros;
            comum.EdtTrabPremio.AlteraRegistrosOk += evt_Ponto.DtTrabMov__AlteraRegistrosOk;
            comum.EdtTrabPremio.DeletaRegistrosOk += evt_Ponto.Padrao_DeletaRegistrosOk;
           // Não inclui esta rotina para os premios
            // comum.EdtTrabPremio.AfterDeletaRegistros += evt_Ponto.Padrao_AfterDeletaRegistros;



        }
        private void color_Enter(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.ActiveCaption;
            if ((evt_Ponto.bmTrabCad != null) && (evt_Ponto.bmTrabCad.Current != null) && ((sender as DataGridView).Name.ToUpper() == "DGVTRABMOV"))
            {
                DataRowView orowCad = (evt_Ponto.bmTrabCad.Current as DataRowView);
                if ((orowCad["APONTAMENTOPROV"].ToString().Trim() != "") && (modelTrabAnterior == false) && (administrador == false)) 
                {
                    tsColar.Enabled = false;
                    tsNovo.Enabled = false;
                    tsEdite.Enabled = false;
                }
            }
            else if ((evt_Ponto.bmTrabCad != null) && (evt_Ponto.bmTrabCad.Current != null) 
                && ((sender as DataGridView).Name.ToUpper() == "DGVTRABMOVANT"))
            {
                if (modelTrabAnterior == false)
                {
                    tsTrabAnterior.Enabled = true;
                }
                else tsTrabAnterior.Enabled = false;

            }
        }

        private void color_Leave(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.Control;
            tsColar.Enabled = true;
            tsNovo.Enabled = true;
            tsEdite.Enabled = true;
            tsTrabAnterior.Enabled = false;
        }

        private void tsNovo_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;
            if (tcCltPonto.ContainsFocus)
            {
                TabControl otab = tcCltPonto;
                if (otab.SelectedTab.Name.ToUpper() == "TPDIARISTAS")
                {
                    ogrid = comum.oTrabMov;
                    ArmePadrao = comum.EdtTrabMov;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPNOTURNO")
                {
                    ogrid = comum.oTrabNoturno;
                    ArmePadrao = comum.EdtTrabNoturno;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPPREMIO")
                {
                    ogrid = comum.oTrabPremio;
                    ArmePadrao = comum.EdtTrabPremio;
                }
            }
            else if (pnCadastro.ContainsFocus)
            {
                DataSet dsCadNovos = evt_Ponto.PegDadosCltCadParaIncluir();
                if ((dsCadNovos == null) || (dsCadNovos.Tables[0].Rows.Count == 0))
                {
                    MessageBox.Show("Não Existem novos trabalhadores a Incluir");
                    return;
                }
                FrmEscolhaTrab frmEscolha = new FrmEscolhaTrab(dsCadNovos);
                if (frmEscolha.ShowDialog() == DialogResult.OK)
                {
                    if ((frmEscolha.trabsEscolhidos != null) && (frmEscolha.trabsEscolhidos.Rows.Count > 0))
                    {
                        // fique só com os marcadas
                        DataTable escolhidos = null;
                        try
                        {
                            escolhidos = frmEscolha.trabsEscolhidos.AsEnumerable().Where(row => row.Field<bool>("Escolhido")).CopyToDataTable();
                        }
                        catch (Exception) { }
                        if (escolhidos == null) return;
                        foreach (DataRow orow in escolhidos.Rows)
                        {
                            DataRow existeCodCad = null;

                            existeCodCad = evt_Ponto.dtTrabCad.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() == orow["CODCAD"].ToString().Trim()).FirstOrDefault();
                            if (existeCodCad != null) continue;

                            DataRow rowTrab = evt_Ponto.dtTrabCad.NewRow();
                            foreach (DataColumn ocol in evt_Ponto.dtTrabCad.Columns)
                            {
                                if (escolhidos.Columns.Contains(ocol.ColumnName))
                                {
                                    rowTrab[ocol.ColumnName] = orow[ocol.ColumnName];
                                }
                            }
                            // inclua na tablista
                            string codcad = rowTrab["CODCAD"].ToString();
                            DataRow orowtabLista = evt_Ponto.servPonto.tabListaTrab.NewRow();
                            orowtabLista["CODCAD"] = codcad;
                            orowtabLista["ADMI"] = Convert.ToDateTime(rowTrab["ADMI"]);
                            evt_Ponto.servPonto.tabListaTrab.Rows.Add(orowtabLista);
                            evt_Ponto.servPonto.tabListaTrab.AcceptChanges();
                            // inclua no trabcad
                            if (evt_Ponto.dictPontoProvisorioAviso.ContainsKey(codcad))
                            {
                                string datas = "";
                                string separador = "";
                                foreach (DateTime semana in evt_Ponto.dictPontoProvisorioAviso[codcad])
                                {
                                    datas = datas + separador + semana.ToString("dd/MM");
                                    separador = "; ";
                                }
                                rowTrab["APONTAMENTOPROV"] = datas;
                            }

                            evt_Ponto.dtTrabCad.Rows.Add(rowTrab);
                        }
                        evt_Ponto.dtTrabCad.AcceptChanges();
                        evt_Ponto.FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, txNome.Text, txServico.Text);
                        evt_Ponto.FiltreMestre();
                        evt_Ponto.BmTrabCad_PositionChanged(evt_Ponto.bmTrabCad, null);
                    }
                }
                return;
            }

            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                return;
            }
            ArmePadrao.Edite(this, "", true);
            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();
            return;
        }


        private void tsEdite_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;

            if (tcCltPonto.ContainsFocus)
            {
                TabControl otab = tcCltPonto;
                if (otab.SelectedTab.Name.ToUpper() == "TPDIARISTAS")
                {
                    ogrid = comum.oTrabMov;
                    ArmePadrao = comum.EdtTrabMov;
                    
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPNOTURNO")
                {
                    ogrid = comum.oTrabNoturno;
                    ArmePadrao = comum.EdtTrabNoturno;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPPREMIO")
                {
                    ogrid = comum.oTrabPremio;
                    ArmePadrao = comum.EdtTrabPremio;
                }
            }
            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                return;
            }
            ArmePadrao.Edite(this, "");

            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();
            return;
        }

        private bool PodeAlterarMovAtual()
        {
            bool result = false;
            if (evt_Ponto.bmTrabCad == null) return result;
            DataRowView registro = (evt_Ponto.bmTrabCad.Current as DataRowView);

            if (registro == null) return result;
            if (evt_Ponto.dictPontoProvisorioAviso == null) return true;
            string trab = registro["CODCAD"].ToString();
            if (!evt_Ponto.dictPontoProvisorioAviso.ContainsKey("TRAB")) return true;

            MessageBox.Show("A");
            return false;
        }
        private void tsDelete_Click(object sender, EventArgs e)
        {

            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


            if (dgvTrabMov.Focused)
            {
                ArmePadrao = comum.EdtTrabMov;
                ogrid = comum.oTrabMov;
            }
            else if (dgvNoturno.Focused)
            {
                ArmePadrao = comum.EdtTrabNoturno;
                ogrid = comum.oTrabNoturno;
            }
            else if (dgvPremio.Focused)
            {
                ArmePadrao = comum.EdtTrabPremio;
                ogrid = comum.oTrabPremio;
            }

            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                return;
            }
            try
            {
                ToolStripItem[] delete = ArmePadrao.Navegador.Items.Find("NovoDelete", false);
                if (delete.Length == 1)
                {
                    bool enable = delete[0].Enabled;
                    delete[0].Enabled = true;
                    delete[0].PerformClick();
                    delete[0].Enabled = enable;
                }
            }
            catch (Exception)
            {
            }

            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();
        }

        private void tsCopiar_Click(object sender, EventArgs e)
        {
            DataGridView odataGrid = null;
            bool movant = false;
            if (dgvPremio.Focused) return;
            if (dgvTrabMov.Focused)
            {
                odataGrid = dgvTrabMov;
            }
            else if (dgvNoturno.Focused)
            {
                odataGrid = dgvNoturno;

            }
            else if (dgvTrabMovAnt.Focused)
            {
                odataGrid = dgvTrabMovAnt;
                movant = true;
            }
            if (odataGrid == null) return;
            Clipboard.Clear();
            DataObject dadosClip = new DataObject();

            List<IDictionary<string, object>> lstDict = new List<IDictionary<string, object>>();
            // ORGANIZAR A PEGADA PELA ORDEM QUE ESTÁ NO GRID
            List<int> lstOrdem = new List<int>();
            foreach (DataGridViewRow ogrid in odataGrid.SelectedRows)
            {
                lstOrdem.Add(ogrid.Index);
            }
            foreach (int i in lstOrdem.OrderBy(ele => ele))
            {
                foreach (DataGridViewRow ogrid in odataGrid.SelectedRows)
                {
                    if (i != ogrid.Index) continue;
                    Dictionary<string, object> keyValuePairs = new Dictionary<string, object>();
                    foreach (DataGridViewCell cell in ogrid.Cells)
                    {
                        if (movant)
                        {
                            if ((odataGrid.Columns[cell.ColumnIndex].DataPropertyName.ToUpper() == "DATA")
                                || (odataGrid.Columns[cell.ColumnIndex].DataPropertyName.ToUpper() == "NOTURNO"))
                                continue;
                        }
                        keyValuePairs.Add(odataGrid.Columns[cell.ColumnIndex].DataPropertyName, cell.Value);
                        //textData = (textData + cell.Value.ToString()
                    }
                    lstDict.Add(keyValuePairs);
                }
            }
            dadosClip.SetData("CustomFormat", lstDict);
            Clipboard.SetDataObject(dadosClip);


            // After this call, the data (string) is placed on the clipboard and tagged
        }    // with a data format of "Text".

        private async void tsColar_Click(object sender, EventArgs e)
        {
            DataGridView odataGrid = null;
            BindingSource BmSource = null;
            ArmeEdicao ArmePadrao = null;
            if (tcCltPonto.ContainsFocus)
            {
                TabControl otab = tcCltPonto;
                if (otab.SelectedTab.Name.ToUpper() == "TPDIARISTAS")
                {
                    if (!dgvTrabMov.Focused) dgvTrabMov.Focus();
                    BmSource = evt_Ponto.bmTrabMov;
                    odataGrid = dgvTrabMov;
                    ArmePadrao = comum.EdtTrabMov;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPNOTURNO")
                {
                    if (!dgvNoturno.Focused) dgvNoturno.Focus();
                    BmSource = evt_Ponto.bmTrabNoturno;
                    odataGrid = dgvNoturno;
                    ArmePadrao = comum.EdtTrabNoturno;
                }
            }
            else return;

            if ((odataGrid == null) || (ArmePadrao == null)) return;

            DataObject retrievedData = (DataObject)Clipboard.GetDataObject();

            if (retrievedData.GetDataPresent("CustomFormat"))
            {
                List<IDictionary<string, object>> item =
                    retrievedData.GetData("CustomFormat") as List<IDictionary<string, object>>;
                if ((item == null) || (item.Count == 0)) return;
                if (ArmePadrao.Navegador.BindingSource == null)
                    ArmePadrao.Navegador.BindingSource = BmSource;
               /* ToolStripItem[] tsSalve = ArmePadrao.Navegador.Items.Find("salve", false);
                ToolStripItem salve = null;
                bool Enable;
                if (tsSalve.Length == 1)
                {
                    salve = tsSalve[0];
                    Enable = salve.Enabled;
                }
                else
                {
                    MessageBox.Show("Desconfigurou Colagem");
                }
                tsSalve = ArmePadrao.Navegador.Items.Find("novo", false);*/
                /* ToolStripItem novo = null;
                 if (tsSalve.Length == 1)
                 {
                     novo = tsSalve[0];
                 }
                */


                DataRowView mestre = (evt_Ponto.bmTrabCad.Current as DataRowView);
                DataTable dataTable = (BmSource.DataSource as DataView).Table;

                // Parcelar PARA QUE GRAVE DE 8 em 8


                List<DataRow> dataRows = new List<DataRow>();

                foreach (Dictionary<string, Object> odict in item)
                {
                    DataRowView orowNovo = (DataRowView)BmSource.AddNew();
                    foreach (KeyValuePair<string, object> par in odict)
                    {
                        orowNovo.Row[par.Key] = par.Value;
                    }
                    dataRows.Add(orowNovo.Row);

                    // dataTable.Rows.Add(orowNovo);
                }
                List<DataRow> dataRowsServidor = new List<DataRow>();
                bool result = true;
                foreach (DataRow orow in dataRows)
                {
                    dataRowsServidor.Add(orow);
                    if (dataRowsServidor.Count > 5)
                    {
                        result = await Prepara_Sql.OpereIncluaRegistrosServidorAsync(dataRowsServidor.ToArray(), "CLTPONTO");
                        dataRowsServidor.Clear();
                        if (!result)
                        {
                            break;
                        }
                    }
                }
                if (dataRowsServidor.Count > 0)
                {
                    result = await Prepara_Sql.OpereIncluaRegistrosServidorAsync(dataRowsServidor.ToArray(), "CLTPONTO");
                    dataRowsServidor.Clear();
                }
                if (!result) { MessageBox.Show("ERRO AO COLAR"); return; }



                foreach (DataRow orow in dataRows)
                {
                    if (orow.RowState == DataRowState.Detached)
                        dataTable.Rows.Add(orow);
                    orow.AcceptChanges();
                    try
                    {
                        if (orow["NOTURNO"].ToString().Trim() == "")
                            evt_Ponto.AtualizaDados(evt_Ponto.bmTrabMov, evt_Ponto.bmTrabNoturno);
                        else
                            evt_Ponto.AtualizaDados(evt_Ponto.bmTrabNoturno, evt_Ponto.bmTrabMov);
                    }
                    catch (Exception) { MessageBox.Show("Erro ao atualizar Mestre"); }
                }
                comum.oTrabCad.FuncaoSoma();
                comum.oTrabCad.ssColocaTotais();
                /*

                foreach (Dictionary<string, Object> odict in item)
                {
                    DataRowView orowNovo = (DataRowView)BmSource.AddNew();
                    foreach (KeyValuePair<string, object> par in odict)
                    {
                        orowNovo.Row[par.Key] = par.Value;
                    }
                    // dataTable.Rows.Add(orowNovo);
                    salve.Enabled = true;
                    try
                    {
                        salve.PerformClick();
                        Thread.Sleep(10);
                        if (salve.Enabled)
                        {
                            BmSource.CancelEdit();
                            orowNovo.Row.Delete();
                            int pos = BmSource.CurrencyManager.Position;
                            if (pos >= 0)
                                BmSource.CurrencyManager.Refresh();
                        }
                    }
                    catch (Exception E)
                    {
                        MessageBox.Show("Erro na Colagem:" + E.Message);
                        salve.Enabled = false;
                        return;
                    }

                }
                salve.Enabled = false;
                */
            }

        }

        private void tcCltPonto_Enter(object sender, EventArgs e)
        {
            tsEdite.Enabled = true;
            tsDelete.Enabled = true;
            tsColar.Enabled = true;
            tsCopiar.Enabled = true;
            TabControl otab = tcCltPonto;
            if (otab.SelectedTab.Name.ToUpper() == "TPDIARISTAS")
            {
                dgvTrabMov.Focus();

            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPNOTURNO")
            {
                dgvNoturno.Focus();
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPPREMIO")
            {
                dgvPremio.Focus();
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPSEMANAANT")
            {
                dgvTrabMovAnt.Focus();
            }
        }

        private void tcCltPonto_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl otab = tcCltPonto;
            if (otab.SelectedTab.Name.ToUpper() == "TPDIARISTAS")
            {
                dgvTrabMov.Focus();

            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPNOTURNO")
            {
                dgvNoturno.Focus();
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPPREMIO")
            {
                dgvPremio.Focus();
            }

            else if (otab.SelectedTab.Name.ToUpper() == "TPSEMANAANT")
            {
                dgvTrabMovAnt.Focus();
            }

        }

      
        private void pnCadastro_Enter(object sender, EventArgs e)
        {
            tsEdite.Enabled = false;
            tsDelete.Enabled = false;
            tsColar.Enabled = false;
            tsCopiar.Enabled = false;
        }
        private void tsConsulta_Click(object sender, EventArgs e)
        {
            try
            {
                frmConsultaTrab.Activate();
                this.Close();
            }
            catch (Exception)
            {

                MessageBox.Show("Consulta foi Descartado");
            }

        }

        private void txTrabalhador_TextChanged(object sender, EventArgs e)
        {
            evt_Ponto.FiltreSetaCampos((sender as TextBox).Text, txSetores.Text, txNome.Text, txServico.Text);
            evt_Ponto.FiltreMestre();
        }

        private void txSetores_TextChanged(object sender, EventArgs e)
        {
            evt_Ponto.FiltreSetaCampos(txTrabalhador.Text, (sender as TextBox).Text, txNome.Text, txServico.Text);
            evt_Ponto.FiltreMestre();
        }

      

        private void txNome_TextChanged(object sender, EventArgs e)
        {
            evt_Ponto.FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, (sender as TextBox).Text, txServico.Text);
            evt_Ponto.FiltreMestre();
        }

        private void txServico_TextChanged(object sender, EventArgs e)
        {
           evt_Ponto.FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, txNome.Text, (sender as TextBox).Text);
            evt_Ponto.FiltreMestre();
        }
        private void tsFiltro_Click(object sender, EventArgs e)
        {
            if (pnFiltro.Enabled)
                pnFiltro.Enabled = false;
            else
                pnFiltro.Enabled = true;
        }

        private void btnSearchServ_Click(object sender, EventArgs e)
        {
           
            Dictionary<string,int> dictCodServ =
            (from gr in evt_Ponto.dtTrabMov.AsEnumerable()

             group gr by new
             {
                 codser = gr.Field<string>("CODSER")
             } into g
             select new
             {
                 cod = g.Key.codser,
                 frequencia = g.Count()
             }  ).ToDictionary( g=> g.cod, g=>g.frequencia);

            Dictionary<string, int> dictCodServAnterior =
            (from gr in evt_Ponto.dtTrabMovAnt.AsEnumerable()

             group gr by new
             {
                 codser = gr.Field<string>("CODSER")
             } into g
             select new
             {
                 cod = g.Key.codser,
                 frequencia = g.Count()
             }).ToDictionary(g => g.cod, g => g.frequencia);

            // Dicionati
           // var lsstCodServ = lstCodServ.Union(lstCodServAnt).ToList();




            List<string> lstCodServ = (from gr in evt_Ponto.dtTrabMov.AsEnumerable()

                                           group gr by new
                                           {
                                               codser = gr.Field<string>("CODSER")
                                           } into g
                                           select g.Key.codser
                         ).ToList();
            List<string> lstCodServAnt = (from gr in evt_Ponto.dtTrabMovAnt.AsEnumerable()

                                       group gr by new
                                       {
                                           codser = gr.Field<string>("CODSER")
                                       } into g
                                       select g.Key.codser
                         ).ToList();

            lstCodServ = lstCodServ.Union(lstCodServAnt).ToList();
            /* string[] dados = (from gr in TabelasIniciais.DsTabelasInciais().Tables["SERVICOONLY"].AsEnumerable()
                                   .Where(row => lstCodServ.Contains(row.Field<string>("COD"))).OrderBy(row=>row.Field<string>("DESCRI"))
                               select gr.Field<string>("COD") + " - " + gr.Field<string>("DESCRI")).ToArray();*/

            if (lstCodServ.Count == 0) return;
            FrmPesquisaCkList oform = new FrmPesquisaCkList(lstCodServ,dictCodServ,dictCodServAnterior);
            /* Point oponto = new Point();
             oponto.Y = groupBox1.Top;
             oponto.X = groupBox1.Left;
             Point topleft = groupBox1.PointToClient(oponto);
             oform.Location = topleft;*/
            //oform.Left = topleft.X;
            //oform.pnCkList.Visible = true;
            if (oform.ShowDialog() == DialogResult.OK)
            {
                string dads = "";
                string traco = "";
                foreach (DataRow orow in oform.servEscolhidos.AsEnumerable().Where(row => row.Field<bool>("ESCOLHIDO") == true))
                {
                    string dado = orow["COD"].ToString();
                    dads = dads + traco + dado;
                    traco = "/";
                }
                txServico.Text = dads;

            }
            else
            {
                rbTodos.PerformClick();
                txServico.Text = "";
                
            }
                //evt_Ponto.FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, txNome.Text, (sender as TextBox).Text);
                // evt_Ponto.FiltreMestre();
            }

        private void button1_Click(object sender, EventArgs e)
        {
            txServico.Text = "";
            rbTodos.PerformClick();
        }

        
        private void rbTodos_Click(object sender, EventArgs e)
        {
            evt_Ponto.FiltreSetaChecks((sender as RadioButton).Checked, rbSemana.Checked, rbAnterior.Checked);
            evt_Ponto.FiltreMestre();
        }

        private void rbSemana_Click(object sender, EventArgs e)
        {
            evt_Ponto.FiltreSetaChecks(rbTodos.Checked, (sender as RadioButton).Checked,  rbAnterior.Checked);
            evt_Ponto.FiltreMestre();
        }

        private void rbAnterior_Click(object sender, EventArgs e)
        {
            evt_Ponto.FiltreSetaChecks(rbTodos.Checked, rbSemana.Checked, (sender as RadioButton).Checked);
            evt_Ponto.FiltreMestre();
        }

        private async void tsTrabAnterior_Click(object sender, EventArgs e)
        {
         

            DataRowView rowMestre = (evt_Ponto.bmTrabCad.Current as DataRowView);
            if (rowMestre == null) return;
            string trab = rowMestre["CODCAD"].ToString();

            DataRowView registro = (evt_Ponto.bmTrabMovAnt.Current as DataRowView);
            if (registro == null) return;
            DateTime semanaAnt = Convert.ToDateTime(registro["DATA"]);



            AtualizaFolha_Cli ofolhaAtual = new AtualizaFolha_Cli();
            ofolhaAtual.Gera_Arquivo();
            
            ServCltPonto servPonto = new ServCltPonto();
            servPonto.ListaCodigo = ofolhaAtual.Enche_TabCodigo();
             
            ServicoFiltroTrab servicoFiltroTrab = new ServicoFiltroTrab(semanaAnt, servPonto);
            bool result = await servicoFiltroTrab.FiltroTrabMovRapidoNovo(trab);
            if (result == false)
            {
                return;
            }
            FrmCltPonto frmCltPonto = new FrmCltPonto(servicoFiltroTrab);
            frmCltPonto.tsLabelSemana.Text = "SEMANA:" + semanaAnt.ToString("dd/MM/yyyy");
            //frmCltPonto.frmConsultaTrab = this; // ref consulta
            frmCltPonto.tsConsulta.Enabled = false;
            frmCltPonto.tsTrabAnterior.Enabled = false;
            frmCltPonto.modelTrabAnterior = true;
            frmCltPonto.administrador = true;
            frmCltPonto.StartPosition = FormStartPosition.CenterParent;
            frmCltPonto.ShowDialog();
            // refletir as alter~ções que houveram na semana anterior
            List<DataRow> dataRows = new List<DataRow>(); 
            foreach (DataRow orow in evt_Ponto.dtTrabMovAnt.AsEnumerable().Where(row =>
                  (row.Field<DateTime>("DATA").CompareTo(semanaAnt) == 0) && (row.Field<string>("TRAB") == trab)))
            {
                dataRows.Add(orow); 
            }
            foreach( DataRow orow in dataRows)
            {  
                orow.Delete();
                orow.AcceptChanges();
            }
            foreach (DataRow orow in frmCltPonto.evt_Ponto.dtTrabMov.AsEnumerable().Where(row =>
                  (row.Field<DateTime>("DATA").CompareTo(semanaAnt) == 0) && (row.Field<string>("TRAB") == trab)))
            {
                DataRow rowNovo = evt_Ponto.dtTrabMovAnt.NewRow();
                foreach(DataColumn col in orow.Table.Columns)
                {
                    rowNovo[col.ColumnName] = orow[col.ColumnName];
                }
                evt_Ponto.dtTrabMovAnt.Rows.Add(rowNovo);
                rowNovo.AcceptChanges();
            }
            if (evt_Ponto.dictPontoProvisorioAviso.ContainsKey(trab))
            {
                evt_Ponto.dictPontoProvisorioAviso.Remove(trab);
                evt_Ponto.PesquisaPontoProvisorio();
                if (evt_Ponto.dictPontoProvisorioAviso.ContainsKey(trab))
                {
                    string datas = "";
                    string separador = "";
                    foreach (DateTime semana in evt_Ponto.dictPontoProvisorioAviso[trab])
                    {
                        datas = datas + separador + semana.ToString("dd/MM");
                        separador = "; ";
                    }
                    rowMestre["APONTAMENTOPROV"] = datas;
                }
                else rowMestre["APONTAMENTOPROV"] = "";
            }
            
            evt_Ponto.BmTrabCad_PositionChanged(evt_Ponto.bmTrabCad, null);
            
            registro = (evt_Ponto.bmTrabMov.Current as DataRowView);
            if (registro != null)
            {
                evt_Ponto.AtualizaDados(evt_Ponto.bmTrabMov, evt_Ponto.bmTrabNoturno);
            }
            registro = (evt_Ponto.bmTrabNoturno.Current as DataRowView);
            if (registro != null)
            {
                evt_Ponto.AtualizaDados( evt_Ponto.bmTrabNoturno, evt_Ponto.bmTrabMov);
            }

            // Atualiza 
            //if (orow["NOTURNO"].ToString().Trim() == "")
            // evt_Ponto.AtualizaDados(evt_Ponto.bmTrabMov, evt_Ponto.bmTrabNoturno);
            // else
            // evt_Ponto.AtualizaDados(evt_Ponto.bmTrabNoturno, evt_Ponto.bmTrabMov);

        }
    }
}
