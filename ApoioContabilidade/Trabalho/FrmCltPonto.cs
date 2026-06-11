using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.ConfigComponentes;
using ApoioContabilidade.Trabalho.FormsAuxiliar;
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

        // Estado de "desfazer último colar" (Ctrl+Z). Só último nível de undo.
        // Expira automaticamente quando muda a aba ou o trabalhador (contexto).
        // Adicionado 2026-05-08.
        private List<DataRow> _undoColarRows;
        private DataTable _undoColarTable;
        private string _undoColarTab;     // nome da TabPage onde foi colado
        private string _undoColarTrab;    // CODCAD do trabalhador atual no momento
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

            dgvTrabalhadores.CellFormatting += DgvTrabalhadores_CellFormatting;


            dgvTrabMov.Enter += color_Enter;
            dgvTrabMov.Leave += color_Leave;

            dgvTrabMovAnt.Enter += color_Enter;
            dgvTrabMovAnt.Leave += color_Leave;

            dgvNoturno.Enter += color_Enter;
            dgvNoturno.Leave += color_Leave;


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

            comum.oTrabMovAnterior.oDataGridView = dgvTrabMovAnt;
            comum.oTrabMovAnterior.oDataGridView.DataSource = evt_Ponto.bmTrabMovAnt;

            comum.oTrabCad.ConfigureDBGridView();
            comum.oTrabMov.ConfigureDBGridView();
            comum.oTrabNoturno.ConfigureDBGridView();
            comum.oTrabMovAnterior.ConfigureDBGridView();

            evt_Ponto.comum = comum;

            evt_Ponto.dtTrabMov.TableNewRow += evt_Ponto.DtTrabMov_TableNewRow;
            evt_Ponto.dtTrabNoturno.TableNewRow += evt_Ponto.DtTrabMovNoturno_TableNewRow;


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
                    case Keys.Z:
                        // Só intercepta se há undo de colar disponível;
                        // senão deixa passar pra undo nativo do controle (TextBox etc).
                        if (_undoColarRows != null && _undoColarRows.Count > 0)
                        {
                            e.Handled = true;
                            // fire-and-forget: KeyDown não pode ser async direto
                            _ = DesfazerUltimoColar();
                        }
                        break;
                }
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // Bug fix 2026-05-07: o original tinha "if" aninhado duplicado em Alt+D1,
            // o que fazia Alt+2/3/T nunca funcionarem (eram dead code dentro de outro if).
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

            // 2026-05-12: visibilidade reativa do botão "Retificar provisório".
            // PositionChanged nas 3 BindingSources cobre navegação por linha; o
            // SelectedIndexChanged do tab control cobre troca de aba.
            EventHandler atualizaRet = (s, ev) => AtualizaVisibilidadeRetificar();
            if (evt_Ponto.bmTrabMov != null)     evt_Ponto.bmTrabMov.PositionChanged     += atualizaRet;
            if (evt_Ponto.bmTrabNoturno != null) evt_Ponto.bmTrabNoturno.PositionChanged += atualizaRet;
            if (evt_Ponto.bmTrabMovAnt != null)  evt_Ponto.bmTrabMovAnt.PositionChanged  += atualizaRet;
            // estado inicial
            AtualizaVisibilidadeRetificar();

            // 2026-05-12: sinalização visual + bloqueio de edição em mês fechado.
            // Aplica nos 3 grids (Diaristas, Noturno, Semana Anterior).
            ConfiguraSinalizacaoMesFechado(dgvTrabMov);
            ConfiguraSinalizacaoMesFechado(dgvNoturno);
            ConfiguraSinalizacaoMesFechado(dgvTrabMovAnt);
        }

        /// <summary>
        /// Pluga handlers de CellFormatting (cores) e CellBeginEdit (bloqueio) num grid:
        ///  - Linha com CODSER='943' fica amarela (alerta visual de provisório)
        ///  - Célula DIA_n cujo dia real cai em mês fechado fica cinza + ReadOnly
        ///  - Tentativa de editar célula congelada é silenciosamente cancelada
        ///    (validator no save é o cinto-suspensório, mas UI não deve nem deixar tentar)
        ///
        /// Adicionado 2026-05-12.
        /// </summary>
        private void ConfiguraSinalizacaoMesFechado(System.Windows.Forms.DataGridView dgv)
        {
            if (dgv == null) return;
            dgv.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                try
                {
                    var row = dgv.Rows[e.RowIndex];
                    var col = dgv.Columns[e.ColumnIndex];

                    // Linha amarela se CODSER='943'
                    if (row.Cells["CODSER"] != null)
                    {
                        string codser = (row.Cells["CODSER"].Value ?? "").ToString().Trim();
                        if (codser == ServicoFiltroTrab.CODSERV_PONTOPROVISORIO_PUBLIC)
                        {
                            e.CellStyle.BackColor = System.Drawing.Color.LightGoldenrodYellow;
                        }
                    }

                    // DIA_n cinza se está em mês fechado
                    string colName = col.DataPropertyName ?? col.Name ?? "";
                    if (colName.StartsWith("DIA") && colName.Length == 4)
                    {
                        int n;
                        if (!int.TryParse(colName.Substring(3, 1), out n)) return;
                        if (n < 1 || n > 7) return;
                        if (row.Cells["DATA"] != null && row.Cells["DATA"].Value != null
                            && row.Cells["DATA"].Value != DBNull.Value)
                        {
                            DateTime data = Convert.ToDateTime(row.Cells["DATA"].Value);
                            if (RegrasMesFechado.DiaEstaFechado(data, n))
                            {
                                e.CellStyle.BackColor = System.Drawing.Color.LightGray;
                                e.CellStyle.ForeColor = System.Drawing.Color.DimGray;
                            }
                        }
                    }
                }
                catch (Exception) { /* defensivo: cor é cosmético, nunca explode */ }
            };

            dgv.CellBeginEdit += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;
                try
                {
                    var row = dgv.Rows[e.RowIndex];
                    var col = dgv.Columns[e.ColumnIndex];
                    string colName = col.DataPropertyName ?? col.Name ?? "";

                    if (row.Cells["DATA"] == null || row.Cells["DATA"].Value == null
                        || row.Cells["DATA"].Value == DBNull.Value) return;
                    DateTime data = Convert.ToDateTime(row.Cells["DATA"].Value);

                    // DIA_n: bloqueia se dia individual em mês fechado
                    if (colName.StartsWith("DIA") && colName.Length == 4)
                    {
                        int n;
                        if (!int.TryParse(colName.Substring(3, 1), out n)) return;
                        if (n >= 1 && n <= 7 && RegrasMesFechado.DiaEstaFechado(data, n))
                        {
                            e.Cancel = true;
                        }
                        return;
                    }

                    // Outros campos congelados: bloqueia se mês inteiro fechado
                    if (RegrasMesFechado.MesFechado(data))
                    {
                        // Campos que congelam (exceto CODSER, que tem fluxo dedicado de retificação)
                        string up = colName.ToUpper();
                        if (up == "DATA" || up == "NOTURNO" || up == "BL" || up == "NUM_MOD" || up == "FAZMOV")
                        {
                            e.Cancel = true;
                        }
                    }
                }
                catch (Exception) { /* defensivo */ }
            };
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

        // B2 fix 2026-05-08: removida PodeAlterarMovAtual — função órfã (não era chamada
        // de lugar algum) com 2 bugs visíveis: literal "TRAB" em vez da variável `trab`
        // no ContainsKey, e MessageBox.Show("A") debug não removido. Inseguro deixar
        // pendurado pra alguém um dia chamar e quebrar fluxo.

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

            if (!retrievedData.GetDataPresent("CustomFormat")) return;

            List<IDictionary<string, object>> item =
                retrievedData.GetData("CustomFormat") as List<IDictionary<string, object>>;
            if ((item == null) || (item.Count == 0)) return;
            if (ArmePadrao.Navegador.BindingSource == null)
                ArmePadrao.Navegador.BindingSource = BmSource;

            DataTable dataTable = (BmSource.DataSource as DataView).Table;

            // B3 fix 2026-05-08: refatorado para "salvar primeiro, popular UI só após sucesso".
            // Antes:
            //   - rows eram criadas via BmSource.AddNew() ANTES de mandar pro servidor
            //   - se servidor falhasse no meio do batch, fazia break sem reverter UI
            //     nem alertar que parte das linhas já tinha sido salva no banco
            //   - estado ficava inconsistente: UI mostrando linhas que estão (algumas)
            //     salvas no servidor e (outras) só locais
            // Agora:
            //   1) Cria rows como Detached (DataTable.NewRow), sem tocar BindingSource
            //   2) Envia ao servidor em batches de 5
            //   3) Falha → mensagem clara informando quantas foram salvas e como
            //      sincronizar (fechar/reabrir). UI fica intocada.
            //   4) Sucesso total → adiciona rows na DataTable (UI reflete o estado real)

            // 1) cria todas as rows como Detached
            List<DataRow> dataRows = new List<DataRow>();
            foreach (Dictionary<string, object> odict in item)
            {
                DataRow orowNovo = dataTable.NewRow();
                foreach (KeyValuePair<string, object> par in odict)
                {
                    orowNovo[par.Key] = par.Value;
                }
                dataRows.Add(orowNovo);
            }

            // 2) envia ao servidor em batches de 5
            const int BATCH_SIZE = 5;
            int salvas = 0;
            bool result = true;
            for (int i = 0; i < dataRows.Count; i += BATCH_SIZE)
            {
                DataRow[] batch = dataRows.Skip(i).Take(BATCH_SIZE).ToArray();
                result = await Prepara_Sql.OpereIncluaRegistrosServidorAsync(batch, "CLTPONTO");
                if (!result) break;
                salvas += batch.Length;
            }

            if (!result)
            {
                // 3) falha: rows ainda Detached → UI não foi alterada.
                //    Avisa o operador sobre o estado real do servidor.
                string msg;
                if (salvas == 0)
                {
                    msg = "Falha ao colar — nenhuma linha foi salva no servidor.";
                }
                else
                {
                    msg = "Falha ao colar.\n\n" +
                          salvas + " de " + dataRows.Count + " linhas foram salvas no servidor antes da falha.\n\n" +
                          "Recomendado: feche e reabra este apontamento para sincronizar a tela com o que está no banco.";
                }
                MessageBox.Show(msg, "ERRO AO COLAR", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 4) sucesso total: popular UI
            foreach (DataRow orow in dataRows)
            {
                dataTable.Rows.Add(orow);
                orow.AcceptChanges();
                try
                {
                    if (orow["NOTURNO"].ToString().Trim() == "")
                        evt_Ponto.AtualizaDados(evt_Ponto.bmTrabMov, evt_Ponto.bmTrabNoturno);
                    else
                        evt_Ponto.AtualizaDados(evt_Ponto.bmTrabNoturno, evt_Ponto.bmTrabMov);
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao atualizar Mestre");
                }
            }
            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();

            // 5) Guarda contexto pra suportar Ctrl+Z (desfazer último colar).
            //    Se mudar aba ou trabalhador antes do undo, será recusado.
            _undoColarRows = new List<DataRow>(dataRows);
            _undoColarTable = dataTable;
            _undoColarTab = tcCltPonto.SelectedTab != null ? tcCltPonto.SelectedTab.Name : "";
            DataRowView rowMestreUndo = evt_Ponto.bmTrabCad.Current as DataRowView;
            _undoColarTrab = rowMestreUndo != null ? rowMestreUndo["CODCAD"].ToString() : "";
        }

        /// <summary>
        /// Desfaz o último colar bem-sucedido — DELETE das linhas no servidor + remoção da UI.
        /// Disparado por Ctrl+Z. Só age se o contexto (aba + trabalhador) ainda for o mesmo
        /// de quando o colar aconteceu; senão, undo expira silenciosamente.
        /// Adicionado 2026-05-08.
        /// </summary>
        private async Task DesfazerUltimoColar()
        {
            if (_undoColarRows == null || _undoColarRows.Count == 0) return;

            // (4b) valida contexto — undo expira ao mudar aba ou trabalhador
            string tabAtual = tcCltPonto.SelectedTab != null ? tcCltPonto.SelectedTab.Name : "";
            DataRowView rowAtual = evt_Ponto.bmTrabCad.Current as DataRowView;
            string trabAtual = rowAtual != null ? rowAtual["CODCAD"].ToString() : "";
            if (tabAtual != _undoColarTab || trabAtual != _undoColarTrab)
            {
                _undoColarRows = null;
                return;
            }

            int total = _undoColarRows.Count;
            int modificadas = _undoColarRows.Count(r => r.RowState != DataRowState.Unchanged
                                                     && r.RowState != DataRowState.Detached);

            // (2a) confirmar com MessageBox; (3b) avisar se houver linhas editadas
            string pergunta;
            if (modificadas > 0)
            {
                pergunta = "Desfazer última colagem?\n\n" +
                           total + " linhas serão removidas.\n\n" +
                           "ATENÇÃO: " + modificadas + " dessas linhas foi editada após o colar — será removida igualmente.";
            }
            else
            {
                pergunta = "Desfazer última colagem?\n\n" +
                           total + " linhas serão removidas do servidor.";
            }
            DialogResult resp = MessageBox.Show(pergunta, "Desfazer Colagem",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;

            // DELETE no servidor uma a uma (API não tem batch delete)
            int deletadas = 0;
            bool sucesso = true;
            foreach (DataRow orow in _undoColarRows)
            {
                if (orow.RowState == DataRowState.Detached) { deletadas++; continue; }
                bool ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, "CLTPONTO");
                if (!ok) { sucesso = false; break; }
                deletadas++;
            }

            // Remove localmente as que já foram deletadas no servidor
            // (mesmo em caso de falha parcial, pra UI ficar consistente com servidor)
            for (int i = 0; i < deletadas && i < _undoColarRows.Count; i++)
            {
                DataRow r = _undoColarRows[i];
                if (r.RowState != DataRowState.Detached)
                    _undoColarTable.Rows.Remove(r);
            }
            _undoColarTable.AcceptChanges();

            if (!sucesso)
            {
                string msg = (deletadas == 0)
                    ? "Falha ao desfazer — nenhuma linha foi removida do servidor."
                    : "Falha ao desfazer.\n\n" +
                      deletadas + " de " + total + " linhas foram removidas no servidor antes da falha.\n\n" +
                      "Recomendado: feche e reabra este apontamento para sincronizar.";
                MessageBox.Show(msg, "ERRO AO DESFAZER", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                _undoColarRows = null; // estado bagunçado, não dá pra retentar
                return;
            }

            // sucesso: sincroniza mestre
            try { evt_Ponto.AtualizaDados(evt_Ponto.bmTrabMov, evt_Ponto.bmTrabNoturno); } catch (Exception) { }
            try { evt_Ponto.AtualizaDados(evt_Ponto.bmTrabNoturno, evt_Ponto.bmTrabMov); } catch (Exception) { }
            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();

            _undoColarRows = null; // consumido — só 1 nível de undo (1a)
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
            else if (otab.SelectedTab.Name.ToUpper() == "TPSEMANAANT")
            {
                dgvTrabMovAnt.Focus();
            }
        }

        private void tcCltPonto_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 2026-05-12: re-checa se botão "Retificar provisório" deve aparecer
            // (a aba ativa muda qual BindingSource serve a row corrente).
            AtualizaVisibilidadeRetificar();

            TabControl otab = tcCltPonto;
            if (otab.SelectedTab.Name.ToUpper() == "TPDIARISTAS")
            {
                dgvTrabMov.Focus();

            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPNOTURNO")
            {
                dgvNoturno.Focus();
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
        // 2026-05-08: tsServico (botão "Serviços") — abre FrmServ filtrado pelo NUM_MOD
        // da row corrente (aba Diaristas/Noturno/Semana Anterior). Equivalente C# do
        // ConsultaServExecute Delphi (fNovotrab_Ponto.pas:2716).
        private void tsServico_Click(object sender, EventArgs e)
        {
            string numMod = "";
            try
            {
                // pega a row corrente da aba ativa
                BindingSource bm = null;
                TabControl otab = tcCltPonto;
                if (otab.SelectedTab != null)
                {
                    string nome = otab.SelectedTab.Name.ToUpper();
                    if (nome == "TPDIARISTAS") bm = evt_Ponto.bmTrabMov;
                    else if (nome == "TPNOTURNO") bm = evt_Ponto.bmTrabNoturno;
                    else if (nome == "TPSEMANAANT") bm = evt_Ponto.bmTrabMovAnt;
                }
                DataRowView rv = bm != null ? bm.Current as DataRowView : null;
                if (rv != null && rv.Row.Table.Columns.Contains("NUM_MOD"))
                    numMod = (rv["NUM_MOD"] ?? "").ToString().Trim();
            }
            catch (Exception) { }

            FrmServ form = new FrmServ();
            form.NumModeloFiltro = numMod;
            form.ShowDialog(this);
        }

        /// <summary>
        /// Abre FrmRetificarProvisorio pra trocar CODSER=943 da linha corrente
        /// pelo serviço real. Após retornar OK, atualiza CODSER/NUM_MOD/QUANT/OBS
        /// da DataRow e dispara save normal (que vai pelo validator de mês fechado
        /// + audit log).
        ///
        /// Adicionado 2026-05-12.
        /// </summary>
        private void tsRetificar_Click(object sender, EventArgs e)
        {
            DataRowView rv = RowCorrenteCltPonto();
            if (rv == null)
            {
                MessageBox.Show("Selecione a linha provisória que deseja retificar.",
                    "Retificar provisório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string codserAtu = (rv["CODSER"] ?? "").ToString().Trim();
            if (codserAtu != ServicoFiltroTrab.CODSERV_PONTOPROVISORIO_PUBLIC)
            {
                MessageBox.Show("Só lançamentos com CODSER='" +
                    ServicoFiltroTrab.CODSERV_PONTOPROVISORIO_PUBLIC + "' (provisório) podem ser retificados.",
                    "Retificar provisório", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (FrmRetificarProvisorio dlg = new FrmRetificarProvisorio(rv.Row))
            {
                if (dlg.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    rv.BeginEdit();
                    rv["CODSER"] = dlg.NovoCodser;
                    if (rv.Row.Table.Columns.Contains("NUM_MOD") && !string.IsNullOrEmpty(dlg.NovoNumMod))
                        rv["NUM_MOD"] = dlg.NovoNumMod;
                    rv["QUANT"] = dlg.NovaQuant;
                    if (rv.Row.Table.Columns.Contains("OBS"))
                        rv["OBS"] = dlg.NovaObs;
                    rv.EndEdit();
                    // O save real acontece pelo fluxo normal (Edita_Cad / ArmeEdicao):
                    // o usuário move a linha ou clica salvar e o validator de mês fechado
                    // + audit do CLTPONTO disparam normalmente.
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Falha ao aplicar retificação: " + ex.Message,
                        "Retificar provisório", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Devolve a DataRowView corrente da aba ativa (Diaristas/Noturno/SemanaAnt).
        /// </summary>
        private DataRowView RowCorrenteCltPonto()
        {
            BindingSource bm = null;
            TabControl otab = tcCltPonto;
            if (otab.SelectedTab != null)
            {
                string nome = otab.SelectedTab.Name.ToUpper();
                if (nome == "TPDIARISTAS") bm = evt_Ponto.bmTrabMov;
                else if (nome == "TPNOTURNO") bm = evt_Ponto.bmTrabNoturno;
                else if (nome == "TPSEMANAANT") bm = evt_Ponto.bmTrabMovAnt;
            }
            return bm != null ? bm.Current as DataRowView : null;
        }

        /// <summary>
        /// Atualiza visibilidade do botão tsRetificar baseado na linha corrente:
        /// só aparece quando linha tem CODSER=943.
        /// Chamado pelos PositionChanged das 3 BindingSources e por SelectedIndexChanged
        /// do tab control.
        /// </summary>
        private void AtualizaVisibilidadeRetificar()
        {
            try
            {
                DataRowView rv = RowCorrenteCltPonto();
                bool eh943 = rv != null
                    && rv.Row.Table.Columns.Contains("CODSER")
                    && (rv["CODSER"] ?? "").ToString().Trim() == ServicoFiltroTrab.CODSERV_PONTOPROVISORIO_PUBLIC;
                tsRetificar.Visible = eh943;
            }
            catch (Exception)
            {
                tsRetificar.Visible = false;
            }
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
