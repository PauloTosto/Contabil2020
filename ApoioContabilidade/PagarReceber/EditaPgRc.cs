using ApoioContabilidade.Services;
using ApoioContabilidade.UserControls;
using ClassFiltroEdite;
using ClassFiltroEdite.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ApoioContabilidade.PagarReceber
{
    public class EdtPgRc
    {
        public int TipoConta;
        public MonteGrid oMestre;
        public MonteGrid oDetalhe1;
        public MonteGrid oDetalhe2;
        public MonteGrid oDetalheCentro;
        public MonteGrid oDetalheEst;
        public MonteGrid oVenda;

        public ArmeEdicao EdtMestre;
        public ArmeEdicao EdtDetalhe1;
        public ArmeEdicao EdtDetalhe2;
        public ArmeEdicao EdtDetalheCentro;
        public ArmeEdicao EdtDetalheEst;
        public ArmeEdicao EdtVenda;

        // permite o acesso a todos os bmSources deste grupo
        private Evt_PgRc evt_Fin_Aux;


        BindingSource bmDesc2Mestre;
        BindingSource bmDesc2Aprop;
     //  private System.Windows.Forms.ErrorProvider errorProvider1;
        public EdtPgRc(int otipoConta, Evt_PgRc evt_Fin)
        {
            TipoConta = otipoConta;
            evt_Fin_Aux = evt_Fin;
        }
        public void MonteGrids()
        {
            oMestre = new MonteGrid();
            oMestre.Clear();
            oMestre.AddValores("DATA", "Data", 0, "", false, 0, "");
            oMestre.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
            if (TipoConta == 1)
                oMestre.AddValores("CREDITO", "Fornecedor", 30, "", false, 0, "");
            else
                oMestre.AddValores("DEBITO", "Cliente", 30, "", false, 0, "");
            oMestre.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oMestre.AddValores("DATA_EMI", "Dta Emissão", 0, "", false, 0, "");
            ///////////////////////////////////////////////////
            oDetalhe1 = new MonteGrid();
            oDetalhe1.AddValores("DATA", "Data", 0, "", false, 0, "");
            oDetalhe1.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
            if (TipoConta == 1)
                oDetalhe1.AddValores("CREDITO", "Bco", 5, "", false, 0, "");
            else
                oDetalhe1.AddValores("DEBITO", "Bco", 5, "", false, 0, "");
            oDetalhe1.AddValores("FORN", "Titular", 50, "", false, 0, "");
            oDetalhe1.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oDetalhe1.AddValores("OBS", "Forma Pagamento/Recebimento", 30, "", false, 0, "");
            //////////////////////////
            oDetalhe2 = new MonteGrid();
            oDetalhe2.AddValores("DATA", "Data", 0, "", false, 0, "");
            oDetalhe2.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
            if (TipoConta == 1)
                oDetalhe2.AddValores("DEBITO", "Centro de Resultado", 30, "", false, 0, "");
            else
                oDetalhe2.AddValores("CREDITO", "Centro de Resultado", 30, "", false, 0, "");
            oDetalhe2.AddValores("HIST", "Histórico", 60, "", false, 0, "");
            ////////////////
            oDetalheCentro = new MonteGrid();
            oDetalheCentro.AddValores("DATA", "Data", 0, "", false, 0, "");
            oDetalheCentro.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
            oDetalheCentro.AddValores("NUM_MOD", "Modelo", 15, "", false, 0, "");
            oDetalheCentro.AddValores("CODSER", "Serviço", 25, "", false, 0, "");
            oDetalheCentro.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            oDetalheCentro.AddValores("GLEBA", "Gleba", 6, "", false, 0, "");
            oDetalheCentro.AddValores("QUADRA", "Quadra", 6, "", false, 0, "");
            oDetalheCentro.AddValores("ICODSER", "Tp.Mov.", 20, "", false, 0, "");
            oDetalheCentro.AddValores("HISTORICO", "Histórico", 40, "", false, 0, "");
            oDetalheCentro.AddValores("CODMAT", "Material", 10, "", false, 0, "");

            ////////////////////////////////
            oDetalheEst = new MonteGrid();
            oDetalheEst.AddValores("DATAC", "Data", 0, "", false, 0, "");
            oDetalheEst.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");

            oDetalheEst.AddValores("QUANT", "Quantidade", 12, "##,##0.00", false, 0, "");
            oDetalheEst.AddValores("COD", "Item de Estoque", 25, "", false, 0, "");
            oDetalheEst.AddValores("DEPOSITO", "Dep.", 5, "", false, 0, "");
            oDetalheEst.AddValores("SETOR", "Setor Destinado", 15, "", false, 0, "");
            oDetalheEst.AddValores("DOC", "Documento", 15, "", false, 0, "");
            oDetalheEst.AddValores("PUNIT", "Preço Medio", 10, "##,##0.00", true, 0, "");
            ////////////////////////
            oVenda = new MonteGrid();
            oVenda.AddValores("SAFRA", "Safra", 6, "", false, 0, "");
            oVenda.AddValores("PROD", "Produto", 20, "", false, 0, "");
            oVenda.AddValores("FIRMA", "Firma", 20, "", false, 0, "");
            oVenda.AddValores("CONT", "N.O.", 8, "", false, 0, "");
            oVenda.AddValores("UnidVenda", "Unid", 6, "", false, 0, "");
            oVenda.AddValores("QUANT", "Quantidade", 10, "##,##0.00", false, 0, "");
            oVenda.AddValores("VALOR", "Valor(R$)", 12, "#,###,##0.00", true, 0, "");
            oVenda.AddValores("PRECO", "Preço", 10, "##,##0.00", false, 0, "");
            oVenda.AddValores("PROD_TP", "Tipo", 12, "", false, 0, "");
            oVenda.AddValores("CERTIF", "Certif.", 12, "", false, 0, "");
            oVenda.AddValores("COMPLEM", "Complem.", 10, "", false, 0, "");

        }

        /// <summary>
        ///  As Tabelas que irão ser a fonte de dados para os combos dos diversos browses
        ///  estas tabelas(cópias virtuais) deverão ter ses campos nomeados (os que servirem como valueMember)
        /// O NOME DOS CAMPOS INDEXADOS TEM QUE SER O MESMO DAS TABELAS QUE UTILLIZARÃO O COMOBOX
        /// EXEMPLO A TABELA CTACENTR UTILIZARÁ O MODELO E O SERVIC ( O CAMPO CHAVE NUM DEVERÁ CHAMAR-SE NUM_MOD COMO NA TABELA CTACENtR)
        // esta regra permitirá o EditeForm na função BindingComplete, recuperar uma relação (pai-filho) no combo-box quando houver um cancelamento
        ///  
        /// </summary>

        public bool MonteEditMestre(BindingSource bindSoure)
        {
            bool result = false;
            if ((oMestre == null) || (oMestre.oDataGridView == null) || (oMestre.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtMestre = new ArmeEdicao(bindSoure);
                MonteEditePgRc(bindSoure);
                EdtMestre.MonteEdicao();
                EdtMestre.oFormEdite.FormClosed += OFormEdite_FormClosed_Mestre; 
                result = true;
            }
            catch (Exception)
            {
            }

            return result;
        }

        private void OFormEdite_FormClosed_Mestre(object sender, FormClosedEventArgs e)
        {
            Linha olinha = EdtMestre.Linhas[1]; // Fornecedores está na linha 1 da Edição
            string campo = "";
            if (TipoConta == 1)
                campo = "CREDITO";
            else
                campo = "DEBITO";
            var controle = olinha.ControlByFieldName(campo);
            if (controle != null)
                ((ComboBox)controle).DataSource = null;
        }

        public bool MonteEditDetalhe1(BindingSource bindSoure)
        {
            bool result = false;
            if ((oDetalhe1 == null) || (oDetalhe1.oDataGridView == null) || (oDetalhe1.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtDetalhe1 = new ArmeEdicao(bindSoure);
                MonteEditeFinanc(bindSoure);
                EdtDetalhe1.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }

            return result;
        }

        public bool MonteEditDetalhe2(BindingSource bindSoure)
        {
            bool result = false;
            if ((oDetalhe2 == null) || (oDetalhe2.oDataGridView == null) || (oDetalhe2.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtDetalhe2 = new ArmeEdicao(bindSoure);
                MonteEditeAprop(bindSoure);
                EdtDetalhe2.MonteEdicao();
                EdtDetalhe2.oFormEdite.FormClosed += OFormEdite_FormClosed_Detalhe2;
                result = true;
            }
            catch (Exception)
            {
            }

            return result;
        }

        private void OFormEdite_FormClosed_Detalhe2(object sender, FormClosedEventArgs e)
        {
            Linha olinha = EdtDetalhe2.Linhas[0];
            string campo = "";
            if (TipoConta == 1)
                campo = "DEBITO";
            else
                campo = "CREDITO";
            var controle = olinha.ControlByFieldName(campo);
            if (controle != null)
                ((ComboBox)controle).DataSource = null;

        }

        public bool MonteEditCentro(BindingSource bindSoure)
        {
            bool result = false;
            if ((oDetalheCentro == null) || (oDetalheCentro.oDataGridView == null) || (oDetalheCentro.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtDetalheCentro = new ArmeEdicao(bindSoure);
                MonteEditeCentro(bindSoure);
                EdtDetalheCentro.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public bool MonteEditEstoque(BindingSource bindSoure)
        {
            bool result = false;
            if ((oDetalheEst == null) || (oDetalheEst.oDataGridView == null) || (oDetalheEst.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtDetalheEst = new ArmeEdicao(bindSoure);
                MonteEditeEstoque(bindSoure);
                EdtDetalheEst.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public bool MonteEditVenda(BindingSource bindSoure)
        {
            bool result = false;
            if ((oVenda == null) || (oVenda.oDataGridView == null) || (oVenda.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtVenda = new ArmeEdicao(bindSoure);
                MonteEditeVenda(bindSoure);
                EdtVenda.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }



        // mestre mov_pgrc
        const int DATA_mst = 0;
        const int VALOR_mst = 1;
        const int CONTA_mst = 2;
        const int DOC_FISC_mst = 3;
        const int DATAEMI_mst = 4;
        private void MonteEditePgRc(BindingSource bmMestre)
        {
            try
            {
                EdtMestre.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmMestre, oMestre.oDataGridView.Columns[DATA_mst], new MaskedTextBox());
                ((MaskedTextBox)olinha.oedite[0]).Validated += EdtPgRc_Validated;
               // ((MaskedTextBox)olinha.oedite[0]).GotFocus += EdtPgRc_GotFocus;

                olinha.TextoConfigure(bmMestre, oMestre.oDataGridView.Columns[VALOR_mst], new NumericTextBox());
                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 2");
                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas
                bmDesc2Mestre = new BindingSource();
                bmDesc2Mestre.DataSource = DadosComum.Desc2Combo.Copy().AsDataView();

                ocombox.DisplayMember = "DESC2";
                ocombox.ValueMember = "DESC2";
                olinha.TextoConfigure(bmMestre, oMestre.oDataGridView.Columns[CONTA_mst], ocombox);
                olinha.oedite[0].Width = 300;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).Enter += EdtPgRc_Enter_Mestre;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);


                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 3");

                olinha.TextoConfigure(bmMestre, oMestre.oDataGridView.Columns[DOC_FISC_mst], new TextBox());
                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmMestre, oMestre.oDataGridView.Columns[DATAEMI_mst], new MaskedTextBox());

                EdtMestre.Add(olinha);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EdtPgRc_GotFocus(object sender, EventArgs e)
        {
            //(sender as MaskedTextBox).SelectionStart = 0;
            //(sender as MaskedTextBox).SelectionLength = 10;
           
        }

      

        private void EdtPgRc_Validated(object sender, EventArgs e)
        {
            // COLOCAR O VALOR NA DATA DE EMISSAO
            try
            {
                
                ((MaskedTextBox)EdtMestre.Linhas[4-1].oedite[0]).Text = (sender as MaskedTextBox).Text;

            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EdtPgRc_Enter_Mestre(object sender, EventArgs e)
        {
            if ((sender as ComboBox).DataSource == null)
            {
                (sender as ComboBox).DisplayMember = "DESC2";
                (sender as ComboBox).ValueMember = "DESC2";
                (sender as ComboBox).DataSource = bmDesc2Mestre;
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }
        }

        // det1 Financ
        const int DATA_det1 = 0;
        const int VALOR_det1 = 1;
        const int CONTA_det1 = 2;
        const int FORN_det1 = 3;
        const int HIST_det1 = 4;
        const int OBS_det1 = 5;

        private void MonteEditeFinanc(BindingSource bmDetalhe1)
        {
            try
            {
                BindingSource bmBancos = new BindingSource();
                DataTable bancosAlt = DadosComum.BancosCombo.Copy();
                DataRow nova = bancosAlt.NewRow();
                nova.BeginEdit();
                nova["NBANCO"] = "00";
               // nova["DESCRI_N"] = "00";
                nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                nova.EndEdit();
                bancosAlt.AcceptChanges();
                bancosAlt.Rows.Add(nova);
                bmBancos.DataSource = bancosAlt.AsDataView();

                EdtDetalhe1.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmDetalhe1, oDetalhe1.oDataGridView.Columns[DATA_det1], new MaskedTextBox());

                olinha.TextoConfigure(bmDetalhe1, oDetalhe1.oDataGridView.Columns[VALOR_det1], new NumericTextBox());


                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmBancos;
                bmBancos.Sort = "DESCRI";
                ocombox.DisplayMember = "DESCRI";
                ocombox.ValueMember = "NBANCO";
                olinha.TextoConfigure(bmDetalhe1, oDetalhe1.oDataGridView.Columns[CONTA_det1], ocombox);
                olinha.oedite[2].Width = 200;
                ((ComboBox)olinha.oedite[2]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[2]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[2]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[2]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)olinha.oedite[2]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingBcos);
                ((ComboBox)olinha.oedite[2]).Validated += new EventHandler(FrmFinan_CampoValidado);


                EdtDetalhe1.Add(olinha);

                olinha = new Linha("Linha 2");

                olinha.TextoConfigure(bmDetalhe1, oDetalhe1.oDataGridView.Columns[FORN_det1], new TextBox());
                EdtDetalhe1.Add(olinha);

                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmDetalhe1, oDetalhe1.oDataGridView.Columns[HIST_det1], new TextBox());

                EdtDetalhe1.Add(olinha);
                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmDetalhe1, oDetalhe1.oDataGridView.Columns[OBS_det1], new TextBox());

                EdtDetalhe1.Add(olinha);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EdtPgRc_Format(object sender, ListControlConvertEventArgs e)
        {
            string value = (e.ListItem as DataRowView).Row["DESCRI"].ToString();
            e.Value = value;
        }


        // det2 Aprop
        const int DATA_det2 = 0;
        const int VALOR_det2 = 1;
        const int CONTA_det2 = 2;
        const int HIST_det2 = 3;

        private void MonteEditeAprop(BindingSource bmDetalhe2)
        {
            try
            {
                // BindingSource bmDesc2 = new BindingSource();
                //bmDesc2.DataSource = DadosComum.Desc2Combo.Copy().AsDataView();

                bmDesc2Aprop = new BindingSource();
                bmDesc2Aprop.DataSource = DadosComum.Desc2Combo.Copy().AsDataView();


                EdtDetalhe2.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmDetalhe2, oDetalhe2.oDataGridView.Columns[DATA_det2], new MaskedTextBox());

                olinha.TextoConfigure(bmDetalhe2, oDetalhe2.oDataGridView.Columns[VALOR_det2], new NumericTextBox());


                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                //ocombox.DataSource = bmDesc2; 
                ocombox.DisplayMember = "DESC2";
                ocombox.ValueMember = "DESC2";
                olinha.TextoConfigure(bmDetalhe2, oDetalhe2.oDataGridView.Columns[CONTA_det2], ocombox);
                olinha.oedite[2].Width = 200;
                ((ComboBox)olinha.oedite[2]).Enter += EdtPgRc_Enter_Apropriacao;
                ((ComboBox)olinha.oedite[2]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[2]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[2]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[2]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[2]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[2]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);
                EdtDetalhe2.Add(olinha);

                olinha = new Linha("Linha 2");
                olinha.TextoConfigure(bmDetalhe2, oDetalhe2.oDataGridView.Columns[HIST_det2], new TextBox());
                EdtDetalhe2.Add(olinha);
            }
            catch (Exception)
            { throw; }
        }

        private void EdtPgRc_Enter_Apropriacao(object sender, EventArgs e)
        {
            if ((sender as ComboBox).DataSource == null)
            {
                (sender as ComboBox).DisplayMember = "DESC2";
                (sender as ComboBox).ValueMember = "DESC2";
                (sender as ComboBox).DataSource = bmDesc2Aprop;
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }

        }

        // Centros Resultado
        const int DATA_centro = 0;
        const int VALOR_centro = 1;
        const int MODELO_centro = 2;
        const int SERVICO_centro = 3;
        const int SETOR_centro = 4;
        const int GLEBA_centro = 5;
        const int QUADRA_centro = 6;
        const int TIPO_centro = 7;
        const int HIST_centro = 8;

        private BindingSource glebaSource;
        private BindingSource quadraSource;
        private BindingSource servicoSource;

       /* private ComboBox BoxServicoCentro;
        private ComboBox BoxGlebaCentro;
        private ComboBox BoxQuadraCentro;
       */
        private BindingSource SourceDetalheCentro;


        private void MonteEditeCentro(BindingSource bmDetalheCentro)
        {
            try
            {
                SourceDetalheCentro = bmDetalheCentro;

                // modelos-Serviço
                BindingSource modeloSource = new BindingSource();
                modeloSource.DataSource = DadosComum.ModeloCombo.Copy().AsDataView();

                servicoSource = new BindingSource();
                servicoSource.DataSource = DadosComum.ServicoModeloCombo.Copy().AsDataView();


                // setor-glebas-quadras
                BindingSource setorSource = new BindingSource();
                setorSource.DataSource = DadosComum.SetoresCombo.Copy().AsDataView();


                glebaSource = new BindingSource();

                glebaSource.DataSource = DadosComum.GlebasCombo.Copy().AsDataView();



                quadraSource = new BindingSource();
                quadraSource.DataSource = DadosComum.QuadrasCombo.Copy().AsDataView();


                EdtDetalheCentro.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[DATA_centro], new MaskedTextBox());

                olinha.TextoConfigure(bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[VALOR_centro], new NumericTextBox());


                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                EdtDetalheCentro.Add(olinha);


                olinha = new Linha("Linha 2");
                ComboBoxMD2 combomd = new ComboBoxMD2();
                combomd.ConfigureComboBoxPaiFilho(olinha,
                      bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[MODELO_centro].DataPropertyName,
                      oDetalheCentro.oDataGridView.Columns[SERVICO_centro].DataPropertyName,
                         DadosComum.ModeloCombo.Copy().AsDataView(),
                       DadosComum.ServicoModeloCombo.Copy().AsDataView());

                EdtDetalheCentro.Add(olinha);


               
                olinha = new Linha("Linha 3");
                ComboMDInverte comboset = new ComboMDInverte();
                comboset.ConfigureComboBoxPaiFilhoNeto(olinha,
                       bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[SETOR_centro].DataPropertyName,
                       oDetalheCentro.oDataGridView.Columns[GLEBA_centro].DataPropertyName,
                       oDetalheCentro.oDataGridView.Columns[QUADRA_centro].DataPropertyName);

                EdtDetalheCentro.Add(olinha);

                
                olinha = new Linha("Linha 4");

                ComboBox ocombox;
                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = DadosComum.TipoMovCombo; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "TPMOV";
                olinha.TextoConfigure(bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[TIPO_centro], ocombox);
                olinha.oedite[0].Width = 200;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                EdtDetalheCentro.Add(olinha);

                olinha = new Linha("Linha 5");
                olinha.TextoConfigure(bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[HIST_centro], new TextBox());
                EdtDetalheCentro.Add(olinha);

                // teste do control composto       

            }
            catch (Exception E)
            { throw; }
        }

        
        public void EdtCentro_ConfigureInicial(object sender, InicioEdicaoEventArgs e)
        {
            // Configura Inicialmente  na entrada da ediçãos
            DataRowView orow = e.current;
            if (orow == null) return;
            string num = orow["NUM_MOD"].ToString();
            string filtro = "";
            if (!string.IsNullOrEmpty(num))
                filtro = "NUM_MOD = '" + num + "'";
            servicoSource.Filter = filtro;
            servicoSource.Sort = "DESCSERV ASC";
            string setor = orow["SETOR"].ToString();
            filtro = "";
            if (!string.IsNullOrEmpty(setor))
                filtro = "SETOR = '" + setor + "'";
            glebaSource.Filter = filtro;
            glebaSource.Sort = "GLEBA ASC";
            string gleba = orow["GLEBA"].ToString();
            filtro = "";
            if (!string.IsNullOrEmpty(gleba))
                filtro = "GLEBA = '" + gleba + "'";
            quadraSource.Filter = filtro;
            quadraSource.Sort = "QUADRA ASC";
        }
        
   
        private void EdtPgRc_KeyDown(object sender, KeyEventArgs e)
        {
            
            
                if (((ComboBox)sender).DroppedDown == true)
            if (e.KeyCode == Keys.Enter) // return
                {
                    e.Handled = true;
                   // SendKeys.Send("{TAB}");
                   // SendKeys.Send(Keys.Tab.ToString());
                }

            return;
        }


        private void EdtPgRc_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == Convert.ToChar(27) ) { // Esc

                if (((ComboBox)sender).DroppedDown == true) { 
                    ((ComboBox)sender).DroppedDown = false;
                    e.Handled = true;
                    return;
                }

            }
            

            /*if (((ComboBox)sender).DroppedDown == true)
            {
                if (e.KeyChar == Convert.ToChar(32)) // space
                    e.Handled = true;
            }*/

            // e.Handled = true; 
        }


       


        // det1 Financ


        //Estoque
        const int DATA_est = 0;
        const int VALOR_est = 1;
        const int QUANT_est = 2;
        const int ITEMS_est = 3;
        const int DEPOSITO_est = 4;
        const int SETOR_est = 5;
        const int DOC_est = 6;

        private void MonteEditeEstoque(BindingSource bmDetalheEst)
        {
            try
            {
                BindingSource bmCadest = new BindingSource();
                bmCadest.DataSource = DadosComum.CadestCombo.Copy().AsDataView();

                BindingSource bmArmazem = new BindingSource();
                bmArmazem.DataSource = DadosComum.ArmazemCombo.Copy().AsDataView();

                BindingSource bmSetor = new BindingSource();
                bmSetor.DataSource = DadosComum.SetoresCombo.Copy().AsDataView();


                EdtDetalheEst.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[DATA_est], new MaskedTextBox());

                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[VALOR_centro], new NumericTextBox());
                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[QUANT_est], new NumericTextBox());
                olinha.oedite[2].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[2].Validated += new EventHandler(FrmFinan_CampoValidado);


                EdtDetalheEst.Add(olinha);


                olinha = new Linha("Linha 2");

                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmCadest; // Cadest

                ocombox.DisplayMember = "DESCRI";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[ITEMS_est], ocombox);
                olinha.oedite[0].Width = 250;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;


                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmArmazem;  // 
                bmArmazem.Sort = "NOME_DEP ASC";
                ocombox.DisplayMember = "NOME_DEP";
                ocombox.ValueMember = "DEPOSITO";
                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[DEPOSITO_est], ocombox);
                olinha.oedite[1].Width = 200;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;


                ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);
                EdtDetalheEst.Add(olinha);
                olinha = new Linha("Linha 3");
                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = bmSetor; // setor
                bmSetor.Sort = "SETOR ASC";
                ocombox.DisplayMember = "CODDESCRI";
                ocombox.ValueMember = "SETOR";
                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[SETOR_est], ocombox);
                olinha.oedite[0].Width = 200;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;


                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);
                EdtDetalheEst.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmDetalheEst, oDetalheEst.oDataGridView.Columns[DOC_est], new TextBox());
                EdtDetalheEst.Add(olinha);
            }
            catch (Exception E)
            { throw; }
        }



        // vendas
        const int SAFRA_ven = 0;
        const int PROD_ven = 1;
        const int FIRMA_ven = 2;
        const int CONT_ven = 3;
        const int UNID_ven = 4;
        const int QUANT_ven = 5;
        const int VALOR_ven = 6;
        const int PRECO_ven = 7;
        const int PROD_TP_ven = 8;
        const int CERTIF_ven = 9;
        const int COMPLEM_ven = 10;

        private void MonteEditeVenda(BindingSource bmVendas)
        {
            try
            {
                BindingSource bmProduto = new BindingSource();
                bmProduto.DataSource = DadosComum.CatProdCombo.Copy().AsDataView();

                BindingSource bmFirma = new BindingSource();
                bmFirma.DataSource = DadosComum.FirmaCombo.Copy().AsDataView();

                BindingSource bmTpProd = new BindingSource();
                bmTpProd.DataSource = DadosComum.TipoProdCombo.Copy().AsDataView();
                DataTable CertificadoCombo = new DataTable();
                CertificadoCombo.Columns.Add("CERTIF", Type.GetType("System.Int32"));
                CertificadoCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                foreach (var keypair in TabelasIniciais.certif)
                {
                    DataRow orow = CertificadoCombo.NewRow();
                    orow["CERTIF"] = keypair.Key;
                    orow["DESCRICAO"] = keypair.Value;
                    CertificadoCombo.Rows.Add(orow);
                }
                CertificadoCombo.AcceptChanges();

                DataTable ComplementoCombo = new DataTable();
                ComplementoCombo.Columns.Add("COMPLEM", Type.GetType("System.Int32"));
                ComplementoCombo.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                foreach (var keypair in TabelasIniciais.complemento)
                {
                    DataRow orow = ComplementoCombo.NewRow();
                    orow["COMPLEM"] = keypair.Key;
                    orow["DESCRICAO"] = keypair.Value;
                    ComplementoCombo.Rows.Add(orow);
                }
                ComplementoCombo.AcceptChanges();

                BindingSource bmCertif = new BindingSource();
                bmCertif.DataSource = CertificadoCombo.AsDataView();
                BindingSource bmComplemento = new BindingSource();
                bmComplemento.DataSource = ComplementoCombo.AsDataView();




                // COMPLEM
                EdtVenda.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[SAFRA_ven], new TextBox());
                olinha.oedite[0].Width = 50;

                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";
                ocombox.DataSource = bmProduto;

                ocombox.DisplayMember = "DESCRI";
                ocombox.ValueMember = "PROD";
                // bmProduto.Sort = "DESCRI ASC";
                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[PROD_ven], ocombox);
                olinha.oedite[1].Width = 250;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;


                ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);
                EdtVenda.Add(olinha);

                olinha = new Linha("Linha 2");
                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmFirma;  // 
                bmFirma.Sort = "DESCRI ASC";
                ocombox.DisplayMember = "DESCRI";
                ocombox.ValueMember = "FIRMA";
                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[FIRMA_ven], ocombox);
                olinha.oedite[0].Width = 250;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);
                EdtVenda.Add(olinha);

                olinha = new Linha("Linha 3");

                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[QUANT_ven], new NumericTextBox());
                olinha.oedite[0].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[0].Validated += new EventHandler(FrmFinan_CampoValidado);

                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[VALOR_ven], new NumericTextBox());
                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);
                EdtVenda.Add(olinha);

                olinha = new Linha("Linha 4");

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = bmTpProd;
                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "PROD_TP";
                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[PROD_TP_ven], ocombox);
                olinha.oedite[0].Width = 200;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);
                EdtVenda.Add(olinha);

                olinha = new Linha("Linha 5");

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = bmCertif;
                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "CERTIF";
                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[CERTIF_ven], ocombox);
                olinha.oedite[0].Width = 150;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = bmComplemento;
                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COMPLEM";
                olinha.TextoConfigure(bmVendas, oVenda.oDataGridView.Columns[COMPLEM_ven], ocombox);
                olinha.oedite[0].Width = 150;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                EdtVenda.Add(olinha);
            }
            catch (Exception E)
            { throw; }
        }



        #region Validações
        void FrmFinan_ComboValidatingNaoPermiteVazio(object sender, CancelEventArgs e)
        {
            string texto = ((ComboBox)sender).Text;

            string selectedtexto = ((ComboBox)sender).SelectedText.Trim();
            
            if ((texto.Length == 26) && (selectedtexto == "") && (texto.Substring(0, 1) == "*"))
            {
                ((ComboBox)sender).SelectedValue = texto.Substring(0, 25);
            }

            if ((((ComboBox)sender).SelectedValue == null) || (texto.Trim() == "")) // || (selectedtexto.Trim() == ""))
            {
                e.Cancel = true;
                Error_Set(sender as Control);

                ((ComboBox)sender).DataBindings[0].ReadValue();
            }
            else
            {

                string text = ((ComboBox)sender).SelectedText.Trim();
                if (text.Trim() == "")
                {
                    int index = ((ComboBox)sender).SelectedIndex;
                    ((ComboBox)sender).SelectedIndex = -1;
                    ((ComboBox)sender).SelectedIndex = index;

                }
            }
        }

        void FrmFinan_CampoValidadoNaoPermiteVazio(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            Error_Set(sender as Control, "");
            if ((sender as Control) is ComboBox)
            {
                string text = ((ComboBox)sender).SelectedText.Trim();
                if (text == "")
                {
                    DataRowView orow = (DataRowView)((ComboBox)sender).SelectedItem;
                    string displayMember = ((ComboBox)sender).DisplayMember;
                    string valor = "";
                    if ((displayMember != "") && (orow != null))
                    {
                        if (orow.Row.Table.Columns.Contains(displayMember))
                        {
                            valor = orow[displayMember].ToString();
                        }
                        if (valor.Trim() != text)
                        {
                            ((ComboBox)sender).DataBindings[0].ReadValue();
                        }
                    }
                }
            }



        }

        void FrmFinan_ComboValidatingBcos(object sender, CancelEventArgs e)
        {

            string texto = ((ComboBox)sender).Text.Trim();
            // string valuemember = ((ComboBox)sender).ValueMember;

            if ((texto == "99") || (texto == ""))
            {

                ((ComboBox)sender).SelectedIndex = -1;
                ((ComboBox)sender).Text = texto;
                return;
            }

            // estou aceitando campo em branco
            string selvalue = "";
            try { selvalue = (string)((ComboBox)sender).SelectedValue; } catch (Exception) { }
            // valor foi selecionado no Combobox
            if (selvalue != null)
            {
                if (selvalue == "0")
                {
                    ((ComboBox)sender).Text = "00";

                }
                else if (selvalue.Trim().Substring(0, 2) == texto.Trim().Substring(0, 2))
                {
                    ((ComboBox)sender).Text = texto.Trim().Substring(0, 2);

                }
                return;
            }
            e.Cancel = true;

            DataRow retornado = null;
            try
            {
                //string textocompara = texto.Trim().PadRight(25,' '); 
                DataView oquery = (((ComboBox)sender).DataSource as System.Data.DataView);
                if (oquery != null)
                    retornado = oquery.Table.AsEnumerable().Where(row => row.Field<string>("NBANCO").Trim() == texto.Trim().Substring(0, 2)).FirstOrDefault();
            }
            catch (Exception)
            {
            }
            if (retornado != null) { e.Cancel = false; return; }
            try
            {
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }
            catch (Exception)
            {
            }
            Error_Set(sender as Control);
        }


        void FrmFinan_CampoValidado(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            Error_Set(sender as Control, "");

        }

        void FrmFinan_ComboValidatingPadrao(object sender, CancelEventArgs e)
        {
            // Em geral DESC2 aceitando branco e não preparado para balanço
            string texto = ((ComboBox)sender).Text.Trim();
            string valuemember = ((ComboBox)sender).ValueMember;
            /*if (((texto.Length > 0) && (texto.Substring(0, 1) == "*")) || (texto == ""))
            {
                if ((texto.Length > 0)  && (texto.Substring(0, 1) == "*"))
                {
                    ((ComboBox)sender).Text = ((ComboBox)sender).Text.Substring(0, ((ComboBox)sender).Text.Length - 2);
                }

                ((ComboBox)sender).SelectedIndex = -1;
                return;
            }
            */
            // estou aceitando campo em branco


            string selectedtexto = ((ComboBox)sender).SelectedText.Trim();

            if ((((ComboBox)sender).SelectedValue == null) || (texto.Trim() == "")) // || (selectedtexto.Trim() == ""))
            {
                e.Cancel = true;
                Error_Set(sender as Control);

                ((ComboBox)sender).DataBindings[0].ReadValue();
                return;
            }
            else
            {

                string text = ((ComboBox)sender).SelectedText.Trim();
                if (text.Trim() == "")
                {
                    int index = ((ComboBox)sender).SelectedIndex;
                    ((ComboBox)sender).SelectedIndex = -1;
                    ((ComboBox)sender).SelectedIndex = index;
                    ((ComboBox)sender).DataBindings[0].ReadValue();
                }
            }


            /* DataRow retornado = null;
             try
             {
                 DataView oquery = (((ComboBox)sender).DataSource as System.Data.DataView);
                 if (oquery != null)
                     retornado = oquery.Table.AsEnumerable().Where(row => row.Field<string>(valuemember).Trim() == texto.Trim()).FirstOrDefault();
             }
             catch (Exception)
             {
             }
             if (retornado != null) { e.Cancel = false; return; }
             try
             {
                 ((ComboBox)sender).DataBindings[0].ReadValue();
             }
             catch (Exception)
             {
             }

             Error_Set(sender as Control);*/
        }

        private void Error_Set(Control ocontrol, string msg = "Campo Invalido! Redigite")
        {
            try
            {
                System.Windows.Forms.ErrorProvider errorProvider1 = (ocontrol.FindForm() as FormDlgForm).errorProvider1;
                if (errorProvider1 != null)
                    errorProvider1.SetError(ocontrol, msg);
            }
            catch (Exception)
            {

            }

        }

        void FrmFinan_Valor(object sender, CancelEventArgs e)
        {
            string texto = ((NumericTextBox)sender).Text;
            Decimal valor = 0;
            try
            {
                valor = Convert.ToDecimal(texto);
            }
            catch (Exception)
            {
            }
            if (valor <= Convert.ToDecimal(0.009))
            {
                e.Cancel = true; // NÃO ACEITA CAMPO ZERADO
                                 // { texto = texto.Substring(0, 8); }
                Error_Set(sender as Control, "Valor Não pode ser <= 0");

                try
                {
                    ((NumericTextBox)sender).DataBindings[0].ReadValue();
                }
                catch (Exception)
                {
                }
                return;
            }
        }

        void FrmFinan_ValorMst(object sender, CancelEventArgs e)
        {
            string texto = ((NumericTextBox)sender).Text;
            Decimal valor = 0;
            try
            {
                valor = Convert.ToDecimal(texto);
            }
            catch (Exception)
            {
                e.Cancel = true;
                return;

            }
            if (valor <= Convert.ToDecimal(0.009))
            {
                e.Cancel = true; // NÃO ACEITA CAMPO ZERADO
                                 // { texto = texto.Substring(0, 8); }

                Error_Set(sender as Control, "Valor Não pode ser <= 0");

                try
                {
                    ((NumericTextBox)sender).DataBindings[0].ReadValue();
                }
                catch (Exception)
                {
                }
                return;
            }
        }


        #endregion
        

    }

}
