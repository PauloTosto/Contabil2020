using ApoioContabilidade.Almoxarifado.ConfigComponentes;
using ApoioContabilidade.Almoxarifado.FormsConsulta;
using ApoioContabilidade.Almoxarifado.Model;
using ApoioContabilidade.Almoxarifado.ServicesAlmox;
using ApoioContabilidade.Services;
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

namespace ApoioContabilidade.Almoxarifado
{
    public partial class FrmSaidaInsumosPonto : Form
    {
        const DayOfWeek _DIADASEMANA = DayOfWeek.Thursday;
        //const DayOfWeek _DIAFIMDASEMANA = DayOfWeek.Wednesday;
        private DateTime dataAnterior;

        ServAlmox servAlmox;
        public EditaAlmox comum;
        // DataTable matAI;
        BindingSource bmSetores;
        public FrmSaidaInsumosPonto()
        {
            /*if (!MovEstStatic.MovEstOk())
            {
                MovEstStatic.Execute(DateTime.Now);
            }*/
            this.KeyPreview = true;
            this.KeyDown += PadraoForm_KeyDown;

            InitializeComponent();
            servAlmox = new ServAlmox(this);
            // matAI = servAlmox.ConstruaMaterialAI();
            SetValoresConfig();

            dtData1.Enabled = true;
            btnConsulta.Enabled = false;
            cbSetores.Enabled = false;
            dataAnterior = Util.DiadoPonto(dtData1.Value, _DIADASEMANA);
            numericAnos.Value = 1;

            comum = new EditaAlmox(servAlmox);
            comum.MonteGrids();
            dgvMestre.CellFormatting += DgvMestre_CellFormatting;
            dgvDetalhe.CellFormatting += DgvDetalhe_CellFormatting;
            dgvSugere.CellFormatting += DgvSugere_CellFormatting;
            dgvSugere.CellContentClick += DgvSugere_CellContentClick;

            dgvMestre.Enter += color_Enter;
            dgvMestre.Leave += color_Leave;

            dgvDetalhe.Enter += color_Enter;
            dgvDetalhe.Leave += color_Leave;

            dgvSugere.Enter += color_Enter;
            dgvSugere.Leave += color_Leave;

            tsEdite.Enabled = false;
            tsDelete.Enabled = false;
            tsNovo.Enabled = false;
            tsCopiaReg.Enabled = false;
            tsNovo.Text = "Ite&ns=>Incluir";
            TabelasIniciaisConfigura();
        }


        
        private void SetValoresConfig()
        {
            cbSetorExclusivo.Enabled = false;
            if (servAlmox.RG_SOITENS == 0)
                rbItensUsuais.Checked = true;
            else if (servAlmox.RG_SOITENS == 1)
                rbItensNoUsuais.Checked = true;
            else
                rbGeral.Checked = true;
            cbSetorExclusivo.Checked = servAlmox.CK_SETOREXCLUSIVE;
            ckSdoQuant.Checked = servAlmox.CK_SDONEGATIVO;
            txtDeposito.Text = servAlmox.STR_DEPOSITO;
            ckAll.Checked = servAlmox.CK_SUGERE;
            
            cbSetorExclusivo.Enabled = true;
           
        }

        private void AtualizeValoresConfig()
        {
            if (rbItensUsuais.Checked)
                servAlmox.RG_SOITENS = 0;
            else if (rbItensNoUsuais.Checked)
                servAlmox.RG_SOITENS = 1;
            else // geral     
                servAlmox.RG_SOITENS = 2;
            servAlmox.CK_SETOREXCLUSIVE = cbSetorExclusivo.Checked;
            servAlmox.CK_SDONEGATIVO = ckSdoQuant.Checked;
            servAlmox.STR_DEPOSITO = txtDeposito.Text;
            servAlmox.CK_SUGERE = ckAll.Checked;
            servAlmox.INI_DTAPESQUISA = dtLimite.Value;
        }


        private void dtData1_ValueChanged(object sender, EventArgs e)
        {
            if (dataAnterior.CompareTo(dtData1.Value) == 0) return;
            if (dataAnterior.CompareTo(dtData1.Value.AddDays(-1)) == 0)
            {
                dtData1.Value = dataAnterior.AddDays(7);
            }
            else if (dataAnterior.CompareTo(dtData1.Value.AddDays(1)) == 0)
            {
                dtData1.Value = dataAnterior.AddDays(-7);
            }
            else
            {
                dtData1.Value = Util.DiadoPonto(dtData1.Value, _DIADASEMANA);
            }
            dataAnterior = dtData1.Value;
            // monthCalendar1.SetDate(dtData1.Value);
            servAlmox.UltimoPonto = dtData1.Value.AddDays(Convert.ToDouble(numericAnos.Value) * 7).AddDays(-1);
            lbPeriodo.Text = dtData1.Value.ToString("d") + " a " + servAlmox.UltimoPonto.ToString("d");
            btnConsulta.Enabled = true;
        }
        private void NumericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            servAlmox.UltimoPonto = dtData1.Value.AddDays(Convert.ToDouble(numericAnos.Value) * 7).AddDays(-1);
            lbPeriodo.Text = dtData1.Value.ToString("d") + " a " + servAlmox.UltimoPonto.ToString("d");
            if (numericAnos.Value == 1)
                tsNovo.Text = "Ite&ns=>Incluir";
            else
                tsNovo.Text = "Ite&ns=>Pesquisa";
            btnConsulta.Enabled = true;


        }
        private void cbSetores_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnConsulta.Enabled = true;
        }
        private void FrmSaidaInsumosPonto_Load(object sender, EventArgs e)
        {
            dtData1.Value = dataAnterior;
            servAlmox.UltimoPonto = dtData1.Value.AddDays(Convert.ToDouble(numericAnos.Value) * 7).AddDays(-1);
            servAlmox.dtData = dtData1.Value;
            dtData1.Refresh();
            dtLimite.Value = servAlmox.INI_DTAPESQUISA;
            dtLimite.Refresh();

            //
            /*  servAlmox.bmMestre.DataSource =
                servAlmox.tabMestreVirtual.AsDataView();

              servAlmox.bmDetalhe.DataSource = servAlmox.tabDetalheAnterior.AsEnumerable().Where(row =>
                       row.IsNull("ID")).AsDataView();

              comum.oMestre.oDataGridView = dgvMestre;
              comum.oMestre.oDataGridView.DataSource = servAlmox.bmMestre;

              comum.oDetalhe.oDataGridView = dgvDetalhe;
              comum.oDetalhe.oDataGridView.DataSource = servAlmox.bmDetalhe;



              comum.oMestre.ConfigureDBGridView();
              comum.oDetalhe.ConfigureDBGridView();

            //  servAlmox.dtTrabMov.TableNewRow += servAlmox.DtTrabMov_TableNewRow;
             // servAlmox.dtTrabNoturno.TableNewRow += servAlmox.DtTrabMovNoturno_TableNewRow;



              if (servAlmox.bmMestre.Count == 0)
              {
                  servAlmox.bmTrabCad.PositionChanged -= servAlmox.BmTrabCad_PositionChanged;
                  servAlmox.bmTrabCad.PositionChanged += servAlmox.BmTrabCad_PositionChanged;

                  servAlmox.BmTrabCad_PositionChanged(servAlmox.bmTrabCad, null);
              }
              else
              {
                  servAlmox.bmTrabCad.PositionChanged -= servAlmox.BmTrabCad_PositionChanged;
                  servAlmox.bmTrabCad.PositionChanged += servAlmox.BmTrabCad_PositionChanged;
                  servAlmox.bmTrabCad.MoveFirst();
                  servAlmox.BmTrabCad_PositionChanged(servAlmox.bmTrabCad, null);
                  //bmMestre.ResetCurrentItem();
              }
             // comum.oTrabCad.ssTotal = ssTrabalhadores;
           //   comum.oTrabCad.FuncaoSoma();
             / comum.oTrabCad.ssColocaTotais();
           */


        }
        private void color_Enter(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.ActiveCaption;
            if ((sender as DataGridView).Name.ToUpper() == "DGVDETALHE")
            {
                tsCopiaReg.Enabled = true;
                tsEdite.Enabled = true;
                tsDelete.Enabled = true;
            }
            else if ((sender as DataGridView).Name.ToUpper() == "DGVMESTRE")
            {
                tsNovo.Enabled = true;
                if (numericAnos.Value == 1)
                    tsNovo.Text = "Ite&ns=>Incluir";
                else
                    tsNovo.Text = "Ite&ns=>Pesquisa";
            }
        }
        private void color_Leave(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.Control;
            tsEdite.Enabled = false;
            tsNovo.Enabled = false;
            tsDelete.Enabled = false;
            tsCopiaReg.Enabled = false;
        }

        private void DgvMestre_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            // throw new NotImplementedException();
        }

        private void DgvDetalhe_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
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
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("FALSO_INC"))
            {
                int intValue = Convert.ToInt32(e.Value);
                string valor = (intValue == 0) ? " " : "G";
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = intValue.ToString();
                e.Value = valor;
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("UNID_COD"))
            {
                /*String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue; */
                string cod = ((DataGridView)sender).Rows[e.RowIndex].Cells["COD"].Value.ToString();
                e.Value = TabelasIniciais.EstoqueUnid(cod);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("COD"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.EstoqueDesc(stringValue);
            }



        }
        private void DgvSugere_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (e.ColumnIndex == (sender as DataGridView).Columns["CHECKED"].Index)
            {
                /*try
                {
                    DataRowView orow = (servAlmox.bmSugere.Current as DataRowView);
                    orow.Row.BeginEdit();
                    if (Convert.ToBoolean(orow.Row["CHECKED"]))
                    {
                        orow.Row["CHECKED"] = false;
                    }
                    else
                    {
                        orow.Row["CHECKED"] = true;
                    }
                    orow.Row.EndEdit();
                    orow.Row.AcceptChanges();
                }
                catch (Exception)
                {
                }*/
            }
        }
        private void DgvSugere_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            /// Este evento é disparado quando o usuário clica no conteúdo de uma célula
            /// Vamos exibir uma mensagem contendo os valores true ou valse refletindo os
            /// o valores da coluna checkbox
            ///
            //Verificamos se e somente se a celula checkbox (Estado) foi clicada
            if (e.RowIndex == -1) return;
            if (e.ColumnIndex == (sender as DataGridView).Columns["CHECKED"].Index)
            {
                try
                {
                    DataRowView orow = (servAlmox.bmSugere.Current as DataRowView);
                    orow.Row.BeginEdit();
                    if (Convert.ToBoolean(orow.Row["CHECKED"]))
                    {
                        orow.Row["CHECKED"] = false;
                    }
                    else
                    {
                        orow.Row["CHECKED"] = true;
                    }
                    orow.Row.EndEdit();
                    orow.Row.AcceptChanges();
                }
                catch (Exception)
                {
                }
            }
        }

        private async void TabelasIniciaisConfigura()
        {
            if (!TabelasIniciais.TabelasIniciaisOk())
            {
                try
                {
                    btnConsulta.Enabled = await TabelasIniciais.Execute();
                    // while (!TabelasIniciais.TabelasIniciaisOk())
                    // { }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            else
            {
                btnConsulta.Enabled = true;

            }
            if (btnConsulta.Enabled)
            {
                cbSetores.Enabled = true;
                if (!DadosComum.tabelasJaConfiguradas())
                    DadosComum.TabelasConfigCombos();
                // 

                BindingSource setorSource = new BindingSource();
                setorSource.DataSource = DadosComum.SetoresCombo.AsEnumerable().
                    Where(row => String.Compare(row.Field<string>("SETOR").Trim(), 1, "9", 1, 2, StringComparison.Ordinal) <= 0).CopyToDataTable().AsDataView();
                cbSetores.Tag = "M";// maiusculas e comportamento co combo com bind is TEXT
                cbSetores.DataSource = setorSource;

                cbSetores.DisplayMember = "CODDESCRI";
                cbSetores.ValueMember = "SETOR";
                cbSetores.MaxDropDownItems = 7;
                cbSetores.DropDownStyle = ComboBoxStyle.DropDown;
                cbSetores.AutoCompleteMode = AutoCompleteMode.Suggest;
                cbSetores.AutoCompleteSource = AutoCompleteSource.ListItems;
            }
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            if (this.cbSetores.SelectedValue == null)
            {
                MessageBox.Show("Selecione um Setor");
                return;
            }
            btnConsulta.Enabled = false;
            tsEdite.Enabled = false;
            tsDelete.Enabled = false;
            string setor = this.cbSetores.SelectedValue.ToString();
            servAlmox.setor = setor;
            servAlmox.dtData = dtData1.Value;
            servAlmox.UltimoPonto = dtData1.Value.AddDays(Convert.ToDouble(numericAnos.Value) * 7).AddDays(-1);
            lbPeriodo.Text = dtData1.Value.ToString("d") + " a " + servAlmox.UltimoPonto.ToString("d");
            bool espera = await servAlmox.FiltroAlmoxarifado_BaseMovestStatic();
            if (!espera)
            {
                MessageBox.Show("Sistema Trabalhando..Tente Outra Vez");
                btnConsulta.Enabled = true;
                return;
            }
           // { espera = await servAlmox.OutroFiltroAlmoxarifado(); }
            if (espera)
            {
                ConfigureInicial();
            }
            tsEdite.Enabled = true;
            tsDelete.Enabled = true;
            dgvMestre.Focus();
            // teste


        }

        private void ConfigureInicial()
        {
            servAlmox.bmDetalhe.DataSource =
               servAlmox.tabDetalheVirtual.AsEnumerable().OrderBy(
                           row => row.Field<string>("COD")).AsDataView();

            servAlmox.bmMestre.DataSource = servAlmox.tabMestreVirtual.AsDataView();

            servAlmox.bmSugere.DataSource = servAlmox.tabSugere.AsDataView();


            comum.oMestre.oDataGridView = dgvMestre;
            comum.oMestre.oDataGridView.DataSource = servAlmox.bmMestre;

            comum.oDetalhe.oDataGridView = dgvDetalhe;
            comum.oDetalhe.oDataGridView.DataSource = servAlmox.bmDetalhe;

            comum.oSugere.oDataGridView = dgvSugere;
            comum.oSugere.oDataGridView.DataSource = servAlmox.bmSugere;


            comum.oSugere.ConfigureDBGridView();
            comum.oDetalhe.ConfigureDBGridView();
            comum.oMestre.ConfigureDBGridView();

            comum.oSugere.oDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            comum.oSugere.oDataGridView.RowTemplate.Height = 10;
           // comum.oSugere.oDataGridView.


            bool ok = comum.MonteEdtDetalhe(servAlmox.bmDetalhe);
            
         

            // para tornar os campo enable é preciso definir o tabstop como false
            comum.EdtDetalhe.Linhas[3].oedite[0].Enabled = false; // sdo estoque
            comum.EdtDetalhe.Linhas[3].oedite[0].TabStop = false;


            comum.EdtDetalhe.Linhas[0].oedite[0].Enabled = false;
            // comum.EdtDetalhe.Linhas[1].oedite[0].Enabled = false;
            comum.EdtDetalhe.Linhas[0].oedite[0].TabStop = false;
            // comum.EdtDetalhe.Linhas[1].oedite[0].TabStop = false;
            // comum.EdtDetalhe.IncluiAutomatico = false;






            servAlmox.tabDetalheVirtual.TableNewRow += servAlmox.TabDetalhe_TableNewRow;
            comum.EdtDetalhe.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(servAlmox.EdtDetalhe_AlteraRegistrosOk);
            comum.EdtDetalhe.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(servAlmox.EdtDetalhe_BeforeAlteraRegistros);
            //comum.EdtDetalhe.BeforeDeletaRegistros += servAlmox.EdtDetalhe_BeforeDeletaRegistros;
            comum.EdtDetalhe.DeletaRegistrosOk += servAlmox.Padrao_DeletaRegistrosOk;



            // servAlmox.tabMestreVirtual.TableNewRow += servAlmox.;


            servAlmox.bmDetalhe.PositionChanged -= servAlmox.BmDetalhe_PositionChanged;
            servAlmox.bmDetalhe.PositionChanged += servAlmox.BmDetalhe_PositionChanged;


            if (servAlmox.bmMestre.Count == 0)
            {
                servAlmox.bmMestre.PositionChanged -= servAlmox.BmMestre_PositionChanged;
                servAlmox.bmMestre.PositionChanged += servAlmox.BmMestre_PositionChanged;

                servAlmox.BmMestre_PositionChanged(servAlmox.bmMestre, null);
            }
            else
            {
                servAlmox.bmMestre.PositionChanged -= servAlmox.BmMestre_PositionChanged;
                servAlmox.bmMestre.PositionChanged += servAlmox.BmMestre_PositionChanged;
                servAlmox.bmMestre.MoveFirst();
                servAlmox.BmMestre_PositionChanged(servAlmox.bmMestre, null);
                //bmMestre.ResetCurrentItem();
            }



        }


        private void btnGraveSugesta_Click(object sender, EventArgs e)
        {
            if (servAlmox.bmDetalhe.Count == 0) return;
            // Guarda as configurações atuais do tabDetalhe
            // PegOS SELECIONADOS CKLISTSUGERE
            // 
            servAlmox.GraveDetalheSugere();


        }

        private void tsEdite_Click(object sender, EventArgs e)
        {
            
            if (!dgvDetalhe.Focused) return;
            if (servAlmox.bmDetalhe.Count == 0) return;
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = comum.oDetalhe;
            dgvDetalhe.Focus();
            ArmePadrao = comum.EdtDetalhe;
            ArmePadrao.Edite(this, "");


        }

        private void tsCopiaReg_Click(object sender, EventArgs e)
        {
            if (servAlmox.bmDetalhe.Count > 0)
            {
                DataRowView rowDetalhe = null;
                try
                {
                    rowDetalhe = servAlmox.bmDetalhe.Current as DataRowView;
                }
                catch (Exception)
                {
                }
                if ((rowDetalhe == null))
                {
                    return;
                }
                string gleba = rowDetalhe["GLEBA"].ToString();
                string quadra = rowDetalhe["QUADRA"].ToString();
                string num = rowDetalhe["NUM_MOD"].ToString();
                DateTime data = Convert.ToDateTime(rowDetalhe["DATA"]);
                DataRow rowNovo = servAlmox.tabDetalheVirtual.NewRow();
                rowNovo["GLEBA"] = gleba;
                rowNovo["QUADRA"] = quadra;
                rowNovo["NUM_MOD"] = num;
                rowNovo["DATA"] = data;
                servAlmox.tabDetalheVirtual.Rows.Add(rowNovo);
                servAlmox.BmMestre_PositionChanged(servAlmox.bmMestre, null);

            }

        }

        private async void tsNovo_Click(object sender, EventArgs e)
        {
            DataTable adoConsulta = await servAlmox.IncluaNovos();
            if ((adoConsulta == null) || (adoConsulta.Rows.Count == 0))
            {
                MessageBox.Show("Nenhum Item a Incluir nespa Pesquisa");
                return;
            }
            DataSet dataset = new DataSet();
            dataset.Tables.Add(adoConsulta);
            FrmConsulta oform = new FrmConsulta(dataset, Decimal.ToInt32(numericAnos.Value));
            if (oform.ShowDialog() == DialogResult.OK)
            {

                DataTable adoResultado = oform.servEscolhidos.Clone();
                oform.servEscolhidos.AsEnumerable().Where(row => row.Field<bool>("ESCOLHIDO") == true).CopyToDataTable(adoResultado, LoadOption.OverwriteChanges);

                var setorservicos = (from gr in adoResultado.AsEnumerable()
                                     group gr by new
                                     {
                                         setor = gr.Field<string>("SETOR"),
                                         codser = gr.Field<string>("CODSER")
                                     } into g
                                     select new
                                     {
                                         setor = g.Key.setor,
                                         codser = g.Key.codser,
                                     });

                bool alterouMestre = false;
                int pos = servAlmox.bmMestre.Position;

                foreach (var dado in setorservicos)
                {
                    List<DataRow> lst = servAlmox.tabMestreVirtual.AsEnumerable().Where(row =>
                   row.Field<string>("SETOR").Trim() == dado.setor.Trim()
                   && row.Field<string>("CODSER").Trim() == dado.codser.ToString().Trim()
                   ).ToList<DataRow>();
                    if (lst.Count == 0)
                    {
                        alterouMestre = true;
                        DataRow oescol = adoResultado.AsEnumerable().Where(row =>
                              row.Field<string>("SETOR").Trim() == dado.setor.Trim()
                                    && row.Field<string>("CODSER").Trim() == dado.codser.ToString().Trim()).FirstOrDefault();
                        DataRowView orowView = (DataRowView)servAlmox.bmMestre.AddNew();
                        
                        DataRow orowmst = orowView.Row;
                        orowmst["SETOR"] = oescol["SETOR"];
                        orowmst["CODSER"] = oescol["CODSER"];
                        orowmst["MODEL"] = oescol["MODEL"];
                        orowmst["SERV"] = oescol["SERV"];
                        orowmst["INICIO"] = oescol["INICIO"];
                        orowmst["FIM"] = oescol["FIM"];
                        orowmst["DATAOK"] = oescol["DATAOK"];
                        orowmst.Table.Rows.Add(orowmst);
                        orowmst.Table.AcceptChanges();
                        //servAlmox.tabMestreVirtual.AcceptChanges();
                        foreach (DataRow oesdet in adoResultado.AsEnumerable().Where(row =>
                                     row.Field<string>("CODSER").Trim() == dado.codser.ToString().Trim()))
                        {
                            DataRowView orowViewDet = (DataRowView)servAlmox.bmDetalhe.AddNew();
                            DataRow orowdtl = orowViewDet.Row;
                            orowdtl["SETOR"] = oesdet["SETOR"];
                            orowdtl["NUM_MOD"] = oesdet["NUM_MOD"];
                            orowdtl["GLEBA"] = oesdet["GLEBA"];
                            orowdtl["QUADRA"] = oesdet["QUADRA"];
                            orowdtl["FALSO_INC"] = 0;
                            orowViewDet.Row.Table.Rows.Add(orowdtl);
                            orowViewDet.Row.Table.AcceptChanges();
                        }
                    }
                    if (alterouMestre)
                    {
                        servAlmox.BmMestre_PositionChanged(servAlmox.bmMestre, null);
                    }
                }

            }

        }
        private void tsDelete_Click(object sender, EventArgs e)
        {
            if (!dgvDetalhe.Focused) return;
            if (servAlmox.bmDetalhe.Count == 0) return;
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = comum.oDetalhe;
            
            ArmePadrao = comum.EdtDetalhe;
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
        }
        private void cbSetorExclusivo_CheckedChanged(object sender, EventArgs e)
        {
           // if (!MovEstStatic.MovEstOk()) return;

            string setorcolhido = cbSetorExclusivo.Checked ? servAlmox.setor : "";
            servAlmox.Recomponha_tabGeralCodNumMovest(setorcolhido);
        }

        private void dtLimite_Validated(object sender, EventArgs e)
        {
           // if (!MovEstStatic.MovEstOk()) return;
            servAlmox.ConstruaMaterialAI();
            string setorcolhido = cbSetorExclusivo.Checked ? servAlmox.setor : "";
            servAlmox.Recomponha_tabGeralCodNumMovest(setorcolhido);

            if (servAlmox.bmMestre.Count > 0)
                servAlmox.BmMestre_PositionChanged(servAlmox.bmMestre, null);


        }

        private void txtDeposito_Leave(object sender, EventArgs e)
        {
        
          /*  if (servAlmox.Carregue_Deposito((sender as TextBox).Text) == false)
            {
                MessageBox.Show("DEPOSITO INVALIDO");
                (sender as TextBox).Focus();

            }*/
        }

        private void FrmSaidaInsumosPonto_FormClosed(object sender, FormClosedEventArgs e)
        {
            // quando entra aqui o form já nao existe (se houver algum é um outro)
            int instancias = 0;
            foreach (Form form in Application.OpenForms)
            {

                if (form.Name.ToUpper() == "FRMSAIDAINSUMOSPONTO")
                    instancias++;
            }
            if (instancias == 0)
            {
                //libere a área em memoria onde está o moveste do movest
              //  MovEstStatic.LibereMovest();

            }
        }

        private void PadraoForm_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.C && e.Modifiers == Keys.Control)
            //{

            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    /*case Keys.C:
                        tsCopiar.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.V:
                        tsColar.PerformClick();
                        e.Handled = true;
                        break;*/
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

        private void tsRelatorioDep_Click(object sender, EventArgs e)
        {
            // MessageBox.Show("Ainda TRabalhando...");
            /*if (!MovEstStatic.MovEstOk())
            {
                MessageBox.Show("Sistema Carregando..Aguarde um pouco");
                return;
            }*/
            DateTime ultimo = dtData1.Value.AddDays(Convert.ToDouble(numericAnos.Value) * 7).AddDays(-1);
            FrmRelDeposito oform = new FrmRelDeposito(servAlmox, dtData1.Value, ultimo);
            oform.Show();
        }

        private void cbSetores_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utils.Combo_KeyPress(sender, e);
        }

        private void FrmSaidaInsumosPonto_FormClosing(object sender, FormClosingEventArgs e)
        {
            // grava as configurações
            AtualizeValoresConfig();
            servAlmox.GraveConfigurações();

        }
    }
}
