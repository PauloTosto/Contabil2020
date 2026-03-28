using ApoioContabilidade.Financeiro;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro_MOVFIN
{
    public class EdtFinDBF
    {
        public MonteGrid oReceber;
        public MonteGrid oPagar;
        public ArmeEdicao EdtReceber;
        public ArmeEdicao EdtPagar;
        // public System.Windows.Forms.ErrorProvider errorProvider1;
        private EventosDBF_Fin evt_Fin_Aux;
        private BindingSource bmDesc2Pagar;
        private BindingSource bmDesc2Receber;
        public bool UseObsGeral;
        int col_extra = 1;
        int DATA_ger = 0;
        int VALOR_ger = 1;
        int CONTA_ger = 2;
        int CONTAFIN_ger = 3;
        int TITULAR_ger = 4;
        int HIST_ger = 5;
        int DOC_ger = 6;
        int VENC_ger = 8;
        int DOC_FISC_ger = 7;
        int OBS_ger = 9;

            const int TAM_BANCOS = 2;

        public EdtFinDBF(EventosDBF_Fin oevt_Fin_Aux)
        {
            evt_Fin_Aux = oevt_Fin_Aux;
            UseObsGeral = true;
            col_extra = 1;
            
        }
        public void MonteGrids()
        {
            oReceber = new MonteGrid();
            oReceber.Clear();
            oReceber.AddValores("DATA", "Data", 11, "", false, 0, "");
            oReceber.AddValores("VALOR", "Valor(R$)", 13, "##,###,##0.00", true, 0, "");
            oReceber.AddValores("CREDITO", "Fornecedor", 30, "", false, 0, "");
            oReceber.AddValores("DEBITO", "Bco", 4, "", false, 0, "");
            oReceber.AddValores("FORN", "Titular", 50, "", false, 0, "");
            oReceber.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oReceber.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");

            oReceber.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oReceber.AddValores("VENC", "Emissão", 10, "", false, 0, "");
            oReceber.AddValores("OBS", "Forma Pg", 20, "", false, 0, "");

            oPagar = new MonteGrid();
            oPagar.Clear();
            oPagar.AddValores("DATA", "Data", 11, "", false, 0, "");
            oPagar.AddValores("VALOR", "Valor(R$)", 13, "##,###,##0.00", true, 0, "");
            oPagar.AddValores("DEBITO", "Fornecedor", 30, "", false, 0, "");
            oPagar.AddValores("CREDITO", "Bco", 4, "", false, 0, "");
            oPagar.AddValores("FORN", "Titular", 50, "", false, 0, "");
            oPagar.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oPagar.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");

            oPagar.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oPagar.AddValores("VENC", "Emissão", 10, "", false, 0, "");
            oPagar.AddValores("OBS", "Forma Pg", 20, "", false, 0, "");

        }
        public bool MonteEdtReceber(BindingSource bindSoure)
        {
            bool result = false;
            if ((oReceber == null) || (oReceber.oDataGridView == null) || (oReceber.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtReceber = new ArmeEdicao(bindSoure);
                MonteEditeReceber(bindSoure);
                EdtReceber.MonteEdicao();
                EdtReceber.oFormEdite.FormClosed += OFormEdite_FormClosed_Receber;
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }
        public bool MonteEdtPagar(BindingSource bindSoure)
        {
            bool result = false;
            if ((oPagar == null) || (oPagar.oDataGridView == null) || (oPagar.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtPagar = new ArmeEdicao(bindSoure);
                MonteEditePagar(bindSoure);
                EdtPagar.MonteEdicao();
                EdtPagar.oFormEdite.FormClosed += OFormEdite_FormClosed_Pagar;
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        private void OFormEdite_FormClosed_Pagar(object sender, FormClosedEventArgs e)
        {
            Linha olinha = EdtPagar.Linhas[1];
            var controle = olinha.ControlByFieldName("DEBITO");
            if (controle != null)
                ((ComboBox)controle).DataSource = null;
        }

        private void OFormEdite_FormClosed_Receber(object sender, FormClosedEventArgs e)
        {
            Linha olinha = EdtReceber.Linhas[1];
            var controle = olinha.ControlByFieldName("CREDITO");
            if (controle != null)
                ((ComboBox)controle).DataSource = null;
        }




       
        private void MonteEditeReceber(BindingSource bmSource)
        {
            try
            {
                // BindingSource bmBancos = new BindingSource();
                // bmBancos.DataSource = DadosComum.BancosCombo.Copy().AsDataView();

                if (UseObsGeral)
                {
                    col_extra = 1;
                    DATA_ger = 0 + col_extra;
                    VALOR_ger = 1 + col_extra;
                    CONTA_ger = 2 + col_extra;
                    CONTAFIN_ger = 3 + col_extra;
                    TITULAR_ger = 4 + col_extra;
                    HIST_ger = 5 + col_extra;
                    DOC_ger = 6 + col_extra;
                    VENC_ger = 8 + col_extra;
                    DOC_FISC_ger = 7 + col_extra;
                    OBS_ger = 9 + col_extra;

                }
                else
                {
                    DATA_ger = 0 ;
                    VALOR_ger = 1 ;
                    CONTA_ger = 2;
                    CONTAFIN_ger = 3;
                    TITULAR_ger = 4;
                    HIST_ger = 5;
                    DOC_ger = 6;
                    VENC_ger = 8;
                    DOC_FISC_ger = 7;
                    OBS_ger = 9;

                }

                BindingSource bmBancos = new BindingSource();
                DataTable bancosAlt = DadosComumDBF.BancosCombo.Copy();
                DataRow nova = bancosAlt.NewRow();
                nova.BeginEdit();
                nova["NBANCO"] = "00";
                // nova["DESCRI_N"] = "00";
                nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                nova.EndEdit();
                bancosAlt.AcceptChanges();
                bancosAlt.Rows.Add(nova);
                bmBancos.DataSource = bancosAlt.AsDataView();



                bmDesc2Receber = new BindingSource();
                bmDesc2Receber.DataSource = DadosComumDBF.Desc2Combo.Copy().AsDataView();


                EdtReceber.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[DATA_ger], new MaskedTextBox());

                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[VALOR_ger], new NumericTextBox());

                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                EdtReceber.Add(olinha);

                olinha = new Linha("Linha 2");
                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                                          //ocombox.DataSource = bmDesc2;
                ocombox.DisplayMember = "DESC2";
                ocombox.ValueMember = "DESC2";
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[CONTA_ger], ocombox);
                olinha.oedite[0].Width = 250;
                ((ComboBox)olinha.oedite[0]).Enter += EdtFin_Enter1;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmBancos;
                ocombox.DisplayMember = "DESCRI";
                ocombox.ValueMember = "NBANCO";
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[CONTAFIN_ger], ocombox);
                olinha.oedite[1].Width = 250;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;


                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);


                EdtReceber.Add(olinha);

                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[TITULAR_ger], new TextBox());
                EdtReceber.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[HIST_ger], new TextBox());

                EdtReceber.Add(olinha);
                olinha = new Linha("Linha 5");
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[DOC_ger], new TextBox());
                olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[DOC_FISC_ger], new TextBox());
                EdtReceber.Add(olinha);
                if (UseObsGeral)
                {
                    olinha = new Linha("Linha 6");
                    olinha.TextoConfigure(bmSource, oReceber.oDataGridView.Columns[OBS_ger], new TextBox());
                    EdtReceber.Add(olinha);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        private void MonteEditePagar(BindingSource bmSource)
        {
            try
            {

                if (UseObsGeral)
                {
                    col_extra = 1;
                    DATA_ger = 0 + col_extra;
                    VALOR_ger = 1 + col_extra;
                    CONTA_ger = 2 + col_extra;
                    CONTAFIN_ger = 3 + col_extra;
                    TITULAR_ger = 4 + col_extra;
                    HIST_ger = 5 + col_extra;
                    DOC_ger = 6 + col_extra;
                    VENC_ger = 8 + col_extra;
                    DOC_FISC_ger = 7 + col_extra;
                    OBS_ger = 9 + col_extra;

                }
                else
                {
                    DATA_ger = 0;
                    VALOR_ger = 1;
                    CONTA_ger = 2;
                    CONTAFIN_ger = 3;
                    TITULAR_ger = 4;
                    HIST_ger = 5;
                    DOC_ger = 6;
                    VENC_ger = 8;
                    DOC_FISC_ger = 7;
                    OBS_ger = 9;

                }



                //    BindingSource bmBancos = new BindingSource();
                //  bmBancos.DataSource = DadosComum.BancosCombo.Copy().AsDataView();
                BindingSource bmBancos = new BindingSource();
                DataTable bancosAlt = DadosComumDBF.BancosCombo.Copy();
                DataRow nova = bancosAlt.NewRow();
                nova.BeginEdit();
                nova["NBANCO"] = "00";
                // nova["DESCRI_N"] = "00";
                nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                nova.EndEdit();
                bancosAlt.AcceptChanges();
                bancosAlt.Rows.Add(nova);
                bmBancos.DataSource = bancosAlt.AsDataView();

                /*List<Bancos> lstBancos = new List<Bancos>();
                Bancos obanco = new Bancos();
                obanco.DESCRI = "00 - FALTA LANÇAMENTO";
                obanco.NBANCO = "00";
                foreach (DataRow orow in DadosComum.BancosCombo.Rows)
                {
                    obanco = new Bancos();
                    obanco.DESCRI = orow["DESCRI"].ToString();
                    obanco.NBANCO = orow["NBANCO"].ToString();
                    lstBancos.Add(obanco);
                }*/
                //bmBancos.DataSource = lstBancos;

                bmDesc2Pagar = new BindingSource();
                /*List<PlaconDesc2> lstDesc2 = new List<PlaconDesc2>();
                foreach (DataRow orow in DadosComum.Desc2Combo.Rows)
                {
                    PlaconDesc2 odesc2 = new PlaconDesc2();
                    odesc2.DESC2 = orow["DESC2"].ToString();
                    lstDesc2.Add(odesc2);
                }*/
                //  bmDesc2.DataSource = lstDesc2;
                // bmDesc2.DataSource = DadosComum.Desc2Combo.AsEnumerable().Where(row=>row.Field<String>("DESC2").Substring(0,1) != "*").CopyToDataTable().AsDataView();
                bmDesc2Pagar.DataSource = DadosComumDBF.Desc2Combo.Copy().AsDataView();

                EdtPagar.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[DATA_ger], new MaskedTextBox());

                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[VALOR_ger], new NumericTextBox());

                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                EdtPagar.Add(olinha);

                olinha = new Linha("Linha 2");
                ComboBox ocombox = new ComboBox();
                /*foreach (DataRow orow in DadosComum.Desc2Combo.Rows)
                {
                    PlaconDesc2 odesc2 = new PlaconDesc2();
                    odesc2.DESC2 = orow["DESC2"].ToString();
                    ocombox.Items.Add(odesc2);
                }
                */
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                                          //ocombox.DataSource = bmDesc2;

                ocombox.DisplayMember = "DESC2";
                ocombox.ValueMember = "DESC2";
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[CONTA_ger], ocombox);
                olinha.oedite[0].Width = 250;

                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)olinha.oedite[0]).Enter += EdtFin_Enter;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                /*Bancos obanco = new Bancos();
               obanco.DESCRI = "00 - FALTA LANÇAMENTO";
               obanco.NBANCO = "00";
               foreach (DataRow orow in DadosComum.BancosCombo.Rows)
               {
                   obanco = new Bancos();
                   obanco.DESCRI = orow["DESCRI"].ToString();
                   obanco.NBANCO = orow["NBANCO"].ToString();
                   ocombox.Items.Add(obanco);
               }
                ((ComboBox)olinha.oedite[0]).DataSource = bmDesc2Pagar;
               */

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmBancos;
                ocombox.DisplayMember = "DESCRI";
                ocombox.ValueMember = "NBANCO";
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[CONTAFIN_ger], ocombox);
                olinha.oedite[1].Width = 250;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);  //FrmFinan_ComboValidatingBcos
                ((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);


                EdtPagar.Add(olinha);

                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[TITULAR_ger], new TextBox());
                EdtPagar.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[HIST_ger], new TextBox());

                EdtPagar.Add(olinha);
                olinha = new Linha("Linha 5");
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[DOC_ger], new TextBox());
                olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[DOC_FISC_ger], new TextBox());
                EdtPagar.Add(olinha);
                if (UseObsGeral)
                {
                    olinha = new Linha("Linha 6");
                    olinha.TextoConfigure(bmSource, oPagar.oDataGridView.Columns[OBS_ger], new TextBox());
                    EdtPagar.Add(olinha);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EdtFin_Enter1(object sender, EventArgs e)
        {
            if ((sender as ComboBox).DataSource == null)
            {
                (sender as ComboBox).DisplayMember = "DESC2";
                (sender as ComboBox).ValueMember = "DESC2";

                (sender as ComboBox).DataSource = bmDesc2Receber;
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }

        }


        private void EdtFin_Enter(object sender, EventArgs e)
        {
            if ((sender as ComboBox).DataSource == null)
            {
                (sender as ComboBox).DisplayMember = "DESC2";
                (sender as ComboBox).ValueMember = "DESC2";
                (sender as ComboBox).DataSource = bmDesc2Pagar;
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }

        }



        #region Validações


        void FrmFinan_ComboValidatingBcos(object sender, CancelEventArgs e)
        {

            string texto = ((ComboBox)sender).Text.Trim();
            string selectedtexto = ((ComboBox)sender).SelectedText.Trim();
            // string valuemember = ((ComboBox)sender).ValueMember;

            if ((texto == "00") || (texto == "99") || (texto == "") || (selectedtexto == ""))
            {

                ((ComboBox)sender).SelectedIndex = -1;
                return;
            }

            // estou aceitando campo em branco
            string selvalue = "";
            try { selvalue = (string)((ComboBox)sender).SelectedValue; } catch (Exception) { }
            // valor foi selecionado no Combobox
            if (selvalue != null)
            {
                if (selvalue.Trim().Substring(0, 2) == texto.Trim().Substring(0, 2))
                {
                    ((ComboBox)sender).Text = texto.Trim().Substring(0, 2);
                    return;
                }
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
            Error_Set(sender as Control, "");
        }


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
            if (valor <= Convert.ToDecimal(0.0009))
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


        #endregion




    }



}

