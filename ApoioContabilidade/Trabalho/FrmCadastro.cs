using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.FormsAuxiliar;
using ApoioContabilidade.Trabalho.ServicesTrab;
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

namespace ApoioContabilidade.Trabalho
{
    public partial class FrmCadastro : Form
    {
        Edita_Cad edtCad;
        Eventos_Cad evtCad;
        string txNomeTrab = "";
        string txSetor = "";
        string txTrab = "";
        string txGleba = "";
        public FrmCadastro()
        {
            InitializeComponent();

            evtCad = new Eventos_Cad();
            evtCad.dataRelatorio = TabelasIniciais_Trab.UltimoDiaMes(dtData1.Value);
            edtCad = new Edita_Cad(evtCad);
            edtCad.MonteGrids();
        }
        private async void FrmCadastro_Load(object sender, EventArgs e)
        {
            bool carrega = false;
            if (!TabelasIniciais_Trab.TabelasIniciaisOk())
            {
                try
                {
                    carrega = await TabelasIniciais_Trab.Execute();
                }
                catch (Exception E)
                {
                    MessageBox.Show(E.Message);
                    throw;
                }
            }
            else
            {
                carrega = true;
            }

            if (carrega) { CarregaDados(); }

        }
        private void btnConsulta_Click(object sender, EventArgs e)
        {
            CarregaDados();
        }
        private async void CarregaDados()
        {
            bool retorno = await evtCad.Salarios_Acesso(dtData1.Value);
            if (!retorno) { return; }
            edtCad.MonteGrids();
            edtCad.oMestre.oDataGridView = dgvMestre;
            edtCad.oDetalhe.oDataGridView = dgvDetalhe;
            edtCad.oCorrente.oDataGridView = dgvInfoBanco;
            FiltreDados();
            edtCad.oMestre.ConfigureDBGridView();
            edtCad.oDetalhe.ConfigureDBGridView();
            edtCad.oCorrente.ConfigureDBGridView();
            
            edtCad.oMestre.oDataGridView.CellFormatting += ODataGridView_CellFormatting;
            edtCad.oDetalhe.oDataGridView.CellFormatting += ODataGridView_CellFormattingDetalhe;


            if (evtCad.bmMestre.Count == 0)
            {
                // evtCad.bmMestre.PositionChanged -= evtCad.BmMestre_PositionChanged;
                // evtCad.bmMestre.PositionChanged += evtCad.BmMestre_PositionChanged; ;

                evtCad.BmMestre_PositionChanged(evtCad.bmMestre, null);
            }
            else
            {
                //evtCad.bmMestre.PositionChanged -= evtCad.BmMestre_PositionChanged;
                // evtCad.bmMestre.PositionChanged += evtCad.BmMestre_PositionChanged; ;
                // evtCad.bmMestre.MoveFirst();
                evtCad.BmMestre_PositionChanged(evtCad.bmMestre, null);
            }

            Posicione_HistoricoSalarios(); // para configurar na primeira entrada o datagrid do historico de salários
            edtCad.oSalarios.oDataGridView = dgvSalarios;
            edtCad.oSalarios.oDataGridView.DataSource = evtCad.bmSalarios;
            edtCad.oSalarios.ConfigureDBGridView();



            MonteEdits();

            evtCad.tabCltCad.TableNewRow += evtCad.DtCadastro_TableNewRow;


            //  ColocaTotais();
        }
        private void MonteEdits()
        {
            if (!DadosComum.tabelasJaConfiguradas())
                DadosComum.TabelasConfigCombos();
            bool ok = edtCad.MonteEdtMestre(evtCad.bmMestre);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Mestre"); return; }
            edtCad.EdtMestre.AlteraRegistrosOk += evtCad.Cadastro__AlteraRegistrosOk;
            edtCad.EdtMestre.BeforeAlteraRegistros += evtCad.Cadastro_BeforeAlteraRegistros;
            edtCad.EdtMestre.BeforeDeletaRegistros += evtCad.Padrao_BeforeDeletaRegistros;
            edtCad.EdtMestre.DeletaRegistrosOk += evtCad.Padrao_DeletaRegistrosOk;


            ok = edtCad.MonteEdtDetalhe(evtCad.bmMestre);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Detalhe"); return; }
            edtCad.EdtDetalhe.AlteraRegistrosOk += evtCad.Cadastro__AlteraRegistrosOk;
            edtCad.EdtDetalhe.BeforeAlteraRegistros += evtCad.Cadastro_BeforeAlteraRegistros;
            edtCad.EdtDetalhe.BeforeDeletaRegistros += evtCad.Padrao_BeforeDeletaRegistros;
            edtCad.EdtDetalhe.DeletaRegistrosOk += evtCad.Padrao_DeletaRegistrosOk;


            ok = edtCad.MonteEdtCorrente(evtCad.bmMestre);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Dados Bancários"); return; }
            edtCad.EdtCorrente.AlteraRegistrosOk += evtCad.Cadastro__AlteraRegistrosOk;
            edtCad.EdtCorrente.BeforeAlteraRegistros += evtCad.Cadastro_BeforeAlteraRegistros;
            edtCad.EdtCorrente.BeforeDeletaRegistros += evtCad.Padrao_BeforeDeletaRegistros;
            edtCad.EdtCorrente.DeletaRegistrosOk += evtCad.Padrao_DeletaRegistrosOk;


            /* comum.EdtTrabPremio.BeforeAlteraRegistros += evt_Ponto.DtTrabMov_BeforeAlteraRegistros;
             comum.EdtTrabPremio.AlteraRegistrosOk += evt_Ponto.DtTrabMov__AlteraRegistrosOk;
             comum.EdtTrabPremio.DeletaRegistrosOk += evt_Ponto.Padrao_DeletaRegistrosOk;
            */

        }
        private void Editar_Novo_Mestre(bool novo)
        {
            ArmeEdicao ArmePadrao = null;
            // MonteGrid ogrid = .oMestre;
            dgvMestre.Focus();
            ArmePadrao = edtCad.EdtMestre;

            ArmePadrao.Edite(this, "", novo);
        }
        private void Editar_Novo_Detalhe(bool novo)
        {
            ArmeEdicao ArmePadrao = null;
            dgvDetalhe.Focus();
            ArmePadrao = edtCad.EdtDetalhe;

            ArmePadrao.Edite(this, "", novo);
        }

        private void Editar_Novo_Corrente(bool novo)
        {
            ArmeEdicao ArmePadrao = null;
            dgvInfoBanco.Focus();
            ArmePadrao = edtCad.EdtCorrente;

            ArmePadrao.Edite(this, "", novo);
        }

        private void ODataGridView_CellFormattingDetalhe(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("COR"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                DataRow orow = edtCad.dtCor.AsEnumerable().Where(r => r.Field<string>("COD").Trim() == stringValue.Trim()).FirstOrDefault();
                if (orow != null)
                {
                    e.Value = orow["DESCRICAO"].ToString();
                }
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("ESTCIVIL"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                DataRow orow = edtCad.dtEstadoCivil.AsEnumerable().Where(r => r.Field<string>("COD").Trim() == stringValue.Trim()).FirstOrDefault();
                if (orow != null)
                {
                    e.Value = orow["DESCRICAO"].ToString();
                }
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("SEXO"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                DataRow orow = edtCad.dtGenero.AsEnumerable().Where(r => r.Field<string>("COD").Trim() == stringValue.Trim()).FirstOrDefault();
                if (orow != null)
                {
                    e.Value = orow["DESCRICAO"].ToString();
                }
            }

            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("GRAUINST"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                DataRow orow = edtCad.dtGrauInst.AsEnumerable().Where(r => r.Field<string>("COD").Trim() == stringValue.Trim()).FirstOrDefault();
                if (orow != null)
                {
                    e.Value = orow["DESCRICAO"].ToString();
                }
            }



        }

        private void ODataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("TIPODEMI"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                DataRow orow = edtCad.dtTipoDemi.AsEnumerable().Where(r => r.Field<string>("COD").Trim() == stringValue.Trim()).FirstOrDefault();
                if (orow != null)
                {
                    e.Value = orow["DESCRICAO"].ToString();
                }
            }

        }

        private void DgvMestre_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private bool FiltreDados()
        {
            if (evtCad.tabCltCad == null) return false;


            txNomeTrab = txNomeTrab.Trim();
            if (txTrab.Trim() == "")
                txTrab = "";
            if (txSetor.Trim() == "")
                txSetor = "";
            if (txGleba.Trim() == "")
                txGleba = "";


            List<string> dadosNomeTrab = new List<string>();
            if (txNomeTrab != "")
                dadosNomeTrab = txNomeTrab.Split(Convert.ToChar("/")).ToList();
            dadosNomeTrab.RemoveAll(a => a.Trim() == "");

            if (txSetor.Trim() == "")
                txSetor = "";
            List<string> dadosTrab = new List<string>();
            if (txTrab != "")
                dadosTrab = txTrab.Split(Convert.ToChar("/")).ToList();
            dadosTrab.RemoveAll(a => a.Trim() == "");

            List<string> dadosSetor = new List<string>();
            if (txSetor != "")
                dadosSetor = txSetor.Split(Convert.ToChar("/")).ToList();
            dadosSetor.RemoveAll(a => a.Trim() == "");
            for (int i = 0; i < dadosSetor.Count; i++)
            {
                dadosSetor[i] = dadosSetor[i].Trim();
            }

            List<string> dadosGleba = new List<string>();
            if (txGleba != "")
                dadosGleba = txSetor.Split(Convert.ToChar("/")).ToList();
            dadosGleba.RemoveAll(a => a.Trim() == "");
            for (int i = 0; i < dadosGleba.Count; i++)
            {
                dadosGleba[i] = dadosGleba[i].Trim();
            }



            DateTime data = dtData1.Value;
            //evtCad.bmMestre = new BindingSource();

            evtCad.bmMestre.DataSource = evtCad.tabCltCad.AsEnumerable().
               Where(a =>
                     (cbAtive.Checked == false ? true :
                         rbAdmi.Checked ? (!a.IsNull("ADMI")
                                && a.Field<DateTime>("ADMI").CompareTo(dtInicio.Value) >= 0 &&
                                    a.Field<DateTime>("ADMI").CompareTo(dtFim.Value) <= 0) :
                                    (!a.IsNull("DEMI")
                                && a.Field<DateTime>("DEMI").CompareTo(dtInicio.Value) >= 0 &&
                                   a.Field<DateTime>("DEMI").CompareTo(dtFim.Value) <= 0))

                     && (rbAtivos.Checked ? (a.IsNull("DEMI") || a.Field<DateTime>("DEMI").CompareTo(data) > 0)
                                    && (!a.IsNull("ADMI") && a.Field<DateTime>("ADMI").CompareTo(data) <= 0)
                                    && (a.IsNull("PRAZO") || a.Field<DateTime>("PRAZO").CompareTo(data) > 0)
                                 : true)
                    && (rbDemi.Checked ? (!a.IsNull("DEMI") && a.Field<DateTime>("DEMI").CompareTo(data) < 0)
                                 : true)

                    && (rbMensalista.Checked ? (!a.IsNull("MENSALISTA") && a.Field<string>("MENSALISTA").ToUpper().Trim() == "X")
                                 : true)
                    && (rbDiarista.Checked ? (!a.IsNull("MENSALISTA") && a.Field<string>("MENSALISTA").Trim() == "")
                                 : true)
                    && (dadosSetor.Count == 0 ? true : dadosSetor.Contains(a.Field<string>("SETOR").Trim()))
                     && (dadosGleba.Count == 0 ? true : dadosGleba.Contains(a.Field<string>("GLECAD").Trim()))
                    && (dadosNomeTrab.Count == 0 ? true : dadosNomeTrab.AsEnumerable()
                        .Where(b => a.Field<string>("NOMECAD").StartsWith(b)).FirstOrDefault() != null)
                    && (dadosTrab.Count == 0 ? true : dadosTrab.AsEnumerable()
                        .Where(b => a.Field<string>("CODCAD").StartsWith(b)).FirstOrDefault() != null)
               ).AsDataView();


            int mensalista = evtCad.tabCltCad.AsEnumerable().
               Where(a =>
                     (cbAtive.Checked == false ? true :
                         rbAdmi.Checked ? (!a.IsNull("ADMI")
                                && a.Field<DateTime>("ADMI").CompareTo(dtInicio.Value) >= 0 &&
                                    a.Field<DateTime>("ADMI").CompareTo(dtFim.Value) <= 0) :
                                    (!a.IsNull("DEMI")
                                && a.Field<DateTime>("DEMI").CompareTo(dtInicio.Value) >= 0 &&
                                   a.Field<DateTime>("DEMI").CompareTo(dtFim.Value) <= 0))

                     && (rbAtivos.Checked ? (a.IsNull("DEMI") || a.Field<DateTime>("DEMI").CompareTo(data) > 0)
                                      && (!a.IsNull("ADMI") && a.Field<DateTime>("ADMI").CompareTo(data) <= 0)
                                    && (a.IsNull("PRAZO") || a.Field<DateTime>("PRAZO").CompareTo(data) > 0)
                                 : true)
                    && (rbDemi.Checked ? (!a.IsNull("DEMI") && a.Field<DateTime>("DEMI").CompareTo(data) < 0)
                                 : true)
                     && (rbMensalista.Checked ? (!a.IsNull("MENSALISTA") && a.Field<string>("MENSALISTA").ToUpper().Trim() == "X")
                                 : true)
                    && (rbDiarista.Checked ? (!a.IsNull("MENSALISTA") && a.Field<string>("MENSALISTA").Trim() == "")
                                 : true)

                    /* && (rbContaCom.Checked ? (!a.IsNull("CONTA1")) && a.Field<string>("CONTA1").Trim() != "" : true)
                     && (rbContaSem.Checked ? (a.IsNull("CONTA1") || a.Field<string>("CONTA1").Trim() == "") : true) */
                    && (dadosSetor.Count == 0 ? true : dadosSetor.Contains(a.Field<string>("SETOR").Trim()))
                     && (dadosGleba.Count == 0 ? true : dadosGleba.Contains(a.Field<string>("GLECAD").Trim()))
                    && (dadosNomeTrab.Count == 0 ? true : dadosNomeTrab.AsEnumerable()
                        .Where(b => a.Field<string>("NOMECAD").StartsWith(b)).FirstOrDefault() != null)
                    && (dadosTrab.Count == 0 ? true : dadosTrab.AsEnumerable()
                        .Where(b => a.Field<string>("CODCAD").StartsWith(b)).FirstOrDefault() != null)
                    && (a.Field<string>("MENSALISTA").ToString().ToUpper().Trim() == "X")
               ).Count();
            ColocaTotais(mensalista);
            SortDados();

            edtCad.oMestre.oDataGridView.DataSource = evtCad.bmMestre;
            edtCad.oDetalhe.oDataGridView.DataSource = evtCad.bmMestre;
            edtCad.oCorrente.oDataGridView.DataSource = evtCad.bmMestre;



            return true;
        }
        private void SortDados()
        {
            if ((evtCad.bmMestre == null) || (evtCad.bmMestre.Count == 0)) return;
            if (rbSortCodigo.Checked)
                evtCad.bmMestre.Sort = "CODCAD";
            else if (rbSortAlfa.Checked)
            {
                evtCad.bmMestre.Sort = "NOMECAD";
            }
            else if (rbSortNumerico.Checked)
            {
                evtCad.bmMestre.Sort = "NUMERO";
            }

        }


        private void txNome_TextChanged(object sender, EventArgs e)
        {
            FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, (sender as TextBox).Text, txGlebas.Text);
            if (FiltreDados())
            {
                //     oFolha.FuncaoSoma();
                //ColocaTotais();
            }

        }
        private void txTrabalhador_TextChanged(object sender, EventArgs e)
        {
            FiltreSetaCampos((sender as TextBox).Text, txSetores.Text, txNome.Text, txGlebas.Text);
            if (FiltreDados())
            {
                // oFolha.FuncaoSoma();
                //ColocaTotais();
            }

        }

        private void txSetores_TextChanged(object sender, EventArgs e)
        {
            FiltreSetaCampos(txTrabalhador.Text, (sender as TextBox).Text, txNome.Text, txGlebas.Text);
            if (FiltreDados())
            {
                //oFolha.FuncaoSoma();
                //ColocaTotais();
            }

        }
        private void txGlebas_TextChanged(object sender, EventArgs e)
        {
            FiltreSetaCampos(txTrabalhador.Text, txSetores.Text, txNome.Text, (sender as TextBox).Text);
            if (FiltreDados())
            {
                //oFolha.FuncaoSoma();
                //ColocaTotais();
            }

        }
        public void FiltreSetaCampos(string otxTrab, string otxSetor, string otxNome, string otxGleba)
        {
            txNomeTrab = otxNome;
            txSetor = otxSetor;
            txTrab = otxTrab;
            txGleba = otxGleba;

        }


        private void ColocaTotais(int numMensalista)
        {
            // coloca totais dinamico no FrmRelTrabalhista 

            tsCabecalho.Items.Clear();
            tsCabecalho.Left = tsTotais.Left;
            if ((evtCad.bmMestre == null) || (evtCad.bmMestre.Count == 0)) return;

            int totalTrab = evtCad.bmMestre.Count;
            tsCabecalho.Items.Add(new ToolStripLabel(""));
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].AutoSize = false;
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].Width = 200;
            tsCabecalho.Items[0].Text = "N.Trabalhadores :" +
                   totalTrab.ToString();

            tsCabecalho.Items.Add(new ToolStripSeparator());

            tsCabecalho.Items.Add(new ToolStripLabel(""));
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].AutoSize = false;
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].Width = 200;
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].Text = "Diaristas:" +
                   (totalTrab - numMensalista).ToString();


            tsCabecalho.Items.Add(new ToolStripSeparator());

            tsCabecalho.Items.Add(new ToolStripLabel(""));
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].AutoSize = false;
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].Width = 200;
            tsCabecalho.Items[tsCabecalho.Items.Count - 1].Text = "Mensalistas:" +
                   numMensalista.ToString();

        }

        private void rbAtivos_CheckedChanged(object sender, EventArgs e)
        {
            if (FiltreDados())
            {
                // oFolha.FuncaoSoma();
                //  ColocaTotais();
            }
        }

        private void rbDemitidos_CheckedChanged(object sender, EventArgs e)
        {
            if (FiltreDados())
            {
                // oFolha.FuncaoSoma();
                // ColocaTotais();
            }
        }

        private void cbAtive_CheckedChanged(object sender, EventArgs e)
        {
            if (FiltreDados())
            {
                // oFolha.FuncaoSoma();
                // ColocaTotais();
            }
        }

        private void rbDiarista_CheckedChanged(object sender, EventArgs e)
        {
            FiltreDados();
        }



        private void dtData1_ValueChanged(object sender, EventArgs e)
        {
            (sender as DateTimePicker).Value = TabelasIniciais_Trab.UltimoDiaMes((sender as DateTimePicker).Value);
            evtCad.dataRelatorio = (sender as DateTimePicker).Value;
            
        }

        private void tcCadastro_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((sender as TabControl).SelectedIndex == 3)
            {
                Posicione_HistoricoSalarios();
            }
        }

        private void Posicione_HistoricoSalarios()
        {
            if (evtCad.bmMestre.Count > 0)
            {
                DataRowView registro = (evtCad.bmMestre.Current as DataRowView);
                bool result = (registro != null);

                if (result) { result = !registro.Row.IsNull("CODCAD"); }
                if (result)
                {
                    evtCad.ConstrucaoTabelaReajustesSalariosUnico(dtData1.Value);

                    string trab = registro.Row["CODCAD"].ToString().Trim();
                    try
                    {   // MovTrab

                        var dado = evtCad.ReajusteVirtual.AsEnumerable().Where(row =>
                            (!row.IsNull("TRAB")) &&
                            (row.Field<string>("TRAB").Trim() == trab)

                            );
                        if ((dado != null))
                        { evtCad.bmSalarios.DataSource = dado.AsDataView(); }
                        else { evtCad.bmSalarios.DataSource = evtCad.ReajusteVirtual.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                    }
                    catch (Exception E)
                    {
                        evtCad.bmSalarios.DataSource = evtCad.ReajusteVirtual.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                    }

                }
                else
                {
                    evtCad.bmSalarios.DataSource = evtCad.ReajusteVirtual.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                }
            }
        }
        private void btnEdita_Click(object sender, EventArgs e)
        {
            if (tcCadastro.SelectedIndex == 0)
                Editar_Novo_Mestre(false);
            else if (tcCadastro.SelectedIndex == 1)
                Editar_Novo_Detalhe(false);
            else if (tcCadastro.SelectedIndex == 2)
                Editar_Novo_Corrente(false);
            TotalizeFiltro();
        }

        private void btnNovo_Click(object sender, EventArgs e)
        {
            if (tcCadastro.SelectedIndex == 0)
                Editar_Novo_Mestre(true);
            else if (tcCadastro.SelectedIndex == 1)
                Editar_Novo_Detalhe(true);
            else if (tcCadastro.SelectedIndex == 2)
                Editar_Novo_Corrente(true);
            TotalizeFiltro();
        }

        private void btnDeleta_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


            if (dgvMestre.Focused)
            {
                ArmePadrao = edtCad.EdtMestre;
                ogrid = edtCad.oMestre;
            }
            else if (dgvDetalhe.Focused)
            {
                ArmePadrao = edtCad.EdtDetalhe;
                ogrid = edtCad.oDetalhe;

            }
            else if (dgvInfoBanco.Focused)
            {
                ArmePadrao = edtCad.EdtCorrente;
                ogrid = edtCad.oCorrente;
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
                TotalizeFiltro();
            }
            catch (Exception)
            {
            }
        }
        private bool TotalizeFiltro()
        {
            if (evtCad.tabCltCad == null) return false;

            txNomeTrab = txNomeTrab.Trim();
            if (txTrab.Trim() == "")
                txTrab = "";
            if (txSetor.Trim() == "")
                txSetor = "";
            if (txGleba.Trim() == "")
                txGleba = "";


            List<string> dadosNomeTrab = new List<string>();
            if (txNomeTrab != "")
                dadosNomeTrab = txNomeTrab.Split(Convert.ToChar("/")).ToList();
            dadosNomeTrab.RemoveAll(a => a.Trim() == "");

            if (txSetor.Trim() == "")
                txSetor = "";
            List<string> dadosTrab = new List<string>();
            if (txTrab != "")
                dadosTrab = txTrab.Split(Convert.ToChar("/")).ToList();
            dadosTrab.RemoveAll(a => a.Trim() == "");

            List<string> dadosSetor = new List<string>();
            if (txSetor != "")
                dadosSetor = txSetor.Split(Convert.ToChar("/")).ToList();
            dadosSetor.RemoveAll(a => a.Trim() == "");
            for (int i = 0; i < dadosSetor.Count; i++)
            {
                dadosSetor[i] = dadosSetor[i].Trim();
            }

            List<string> dadosGleba = new List<string>();
            if (txGleba != "")
                dadosGleba = txSetor.Split(Convert.ToChar("/")).ToList();
            dadosGleba.RemoveAll(a => a.Trim() == "");
            for (int i = 0; i < dadosGleba.Count; i++)
            {
                dadosGleba[i] = dadosGleba[i].Trim();
            }

            DateTime data = dtData1.Value;
            int mensalista = 0;
            try
            {
                mensalista= evtCad.tabCltCad.AsEnumerable().
               Where(a =>
                     (cbAtive.Checked == false ? true :
                         rbAdmi.Checked ? (!a.IsNull("ADMI")
                                && a.Field<DateTime>("ADMI").CompareTo(dtInicio.Value) >= 0 &&
                                    a.Field<DateTime>("ADMI").CompareTo(dtFim.Value) <= 0) :
                                    (!a.IsNull("DEMI")
                                && a.Field<DateTime>("DEMI").CompareTo(dtInicio.Value) >= 0 &&
                                   a.Field<DateTime>("DEMI").CompareTo(dtFim.Value) <= 0))

                     && (rbAtivos.Checked ? (a.IsNull("DEMI") || a.Field<DateTime>("DEMI").CompareTo(data) > 0)
                                      && (!a.IsNull("ADMI") && a.Field<DateTime>("ADMI").CompareTo(data) <= 0)
                                    && (a.IsNull("PRAZO") || a.Field<DateTime>("PRAZO").CompareTo(data) > 0)
                                 : true)
                    && (rbDemi.Checked ? (!a.IsNull("DEMI") && a.Field<DateTime>("DEMI").CompareTo(data) < 0)
                                 : true)
                     && (rbMensalista.Checked ? (!a.IsNull("MENSALISTA") && a.Field<string>("MENSALISTA").ToUpper().Trim() == "X")
                                 : true)
                    && (rbDiarista.Checked ? (!a.IsNull("MENSALISTA") && a.Field<string>("MENSALISTA").Trim() == "")
                                 : true)

                    /* && (rbContaCom.Checked ? (!a.IsNull("CONTA1")) && a.Field<string>("CONTA1").Trim() != "" : true)
                     && (rbContaSem.Checked ? (a.IsNull("CONTA1") || a.Field<string>("CONTA1").Trim() == "") : true) */
                    && (dadosSetor.Count == 0 ? true : dadosSetor.Contains(a.Field<string>("SETOR").Trim()))
                     && (dadosGleba.Count == 0 ? true : dadosGleba.Contains(a.Field<string>("GLECAD").Trim()))
                    && (dadosNomeTrab.Count == 0 ? true : dadosNomeTrab.AsEnumerable()
                        .Where(b => a.Field<string>("NOMECAD").StartsWith(b)).FirstOrDefault() != null)
                    && (dadosTrab.Count == 0 ? true : dadosTrab.AsEnumerable()
                        .Where(b => a.Field<string>("CODCAD").StartsWith(b)).FirstOrDefault() != null)
                    && (a.Field<string>("MENSALISTA").ToString().ToUpper().Trim() == "X")
               ).Count();

             
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                
            }
            ColocaTotais(mensalista);
            return true;
        }

        private void tsFamilia_Click(object sender, EventArgs e)
        {
            FrmCltFamilia frmCltFamilia = new FrmCltFamilia(evtCad);
            DialogResult result = frmCltFamilia.ShowDialog();
        }

        private void tsReajuste_Click(object sender, EventArgs e)
        {
            FrmCltReajuste frmCltReajuste = new FrmCltReajuste(evtCad);
            DialogResult result = frmCltReajuste.ShowDialog();
        }

        private void tsObsSefip_Click(object sender, EventArgs e)
        {
            FrmCltSefip frmCltSefip = new FrmCltSefip(evtCad);
            DialogResult result = frmCltSefip.ShowDialog();

        }

        private void tsSalarios_Click(object sender, EventArgs e)
        {
            try
            {
                FrmPesqFolha frmPesqFolha = new FrmPesqFolha(evtCad);
                DialogResult result = frmPesqFolha.ShowDialog();


            }
            catch (Exception)
            {

                MessageBox.Show("Erro Salarios");
            }
           
        }

       
    }
}

