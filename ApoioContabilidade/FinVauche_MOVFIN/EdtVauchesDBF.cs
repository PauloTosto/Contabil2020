using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ApoioContabilidade.FinVauche_MOVFIN
{
    public class EdtVauchesDBF
    {
        public MonteGrid oVauches;
        public ArmeEdicao EdtVauches;
        // public System.Windows.Forms.ErrorProvider errorProvider1;
        private EventosDBF_Vauches evt_Fin_Aux;
        private BindingSource bmDesc2DEBITO;
        private BindingSource bmDesc2CREDITO;
        public bool UseObsGeral;
        int col_extra = 1;
        int DATA_ger = 0;
        int VALOR_ger = 1;
        int DEBITO_ger = 2;
        int CREDITO_ger = 3;
        int TITULAR_ger = 4;
        int HIST_ger = 5;
        int DOC_ger = 6;
        int VENC_ger = 8;
        int DOC_FISC_ger = 7;
        int OBS_ger = 9;

        const int TAM_BANCOS = 2;

        public EdtVauchesDBF(EventosDBF_Vauches oevt_Fin_Aux)
        {
            evt_Fin_Aux = oevt_Fin_Aux;
            UseObsGeral = true;
            col_extra = 1;

        }
        public void MonteGrids()
        {
            oVauches = new MonteGrid();
            oVauches.Clear();
            oVauches.AddValores("DATA", "Data", 11, "", false, 0, "");
            oVauches.AddValores("VALOR", "Valor(R$)", 13, "##,###,##0.00", true, 0, "");
            oVauches.AddValores("DEBITO", "DEBITO", 30, "Debito", false, 0, "");
            oVauches.AddValores("CREDITO", "Credito", 30, "", false, 0, "");
           
            oVauches.AddValores("FORN", "Titular", 50, "", false, 0, "");
            oVauches.AddValores("HIST", "Histórico", 40, "", false, 0, "");
            oVauches.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");

            oVauches.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oVauches.AddValores("VENC", "Emissão", 10, "", false, 0, "");
            oVauches.AddValores("OBS", "Forma Pg", 20, "", false, 0, "");

            
        }
        public bool MonteEdtReceber(BindingSource bindSoure)
        {
            bool result = false;
            if ((oVauches == null) || (oVauches.oDataGridView == null) || (oVauches.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtVauches = new ArmeEdicao(bindSoure);
                MonteEditeVauches(bindSoure);
                EdtVauches.MonteEdicao();
                EdtVauches.oFormEdite.FormClosed += OFormEdite_FormClosed_Receber;
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }
      
        private void OFormEdite_FormClosed_Pagar(object sender, FormClosedEventArgs e)
        {
            Linha olinha = EdtVauches.Linhas[1];
            var controle = olinha.ControlByFieldName("DEBITO");
            if (controle != null)
                ((ComboBox)controle).DataSource = null;
        }

        private void OFormEdite_FormClosed_Receber(object sender, FormClosedEventArgs e)
        {
            Linha olinha = EdtVauches.Linhas[1];
            var controle = olinha.ControlByFieldName("CREDITO");
            if (controle != null)
                ((ComboBox)controle).DataSource = null;
        }





       

        private void MonteEditeVauches(BindingSource bmSource)
        {
            try
            {

                    col_extra = 0;
                    DATA_ger = 0 + col_extra;
                    VALOR_ger = 1 + col_extra;
                    DEBITO_ger = 2 + col_extra;
                    CREDITO_ger = 3 + col_extra;
                    TITULAR_ger = 4 + col_extra;
                    HIST_ger = 5 + col_extra;
                    DOC_ger = 6 + col_extra;
                    VENC_ger = 8 + col_extra;
                    DOC_FISC_ger = 7 + col_extra;
                    OBS_ger = 9 + col_extra;

               


                //    BindingSource bmBancos = new BindingSource();
                //  bmBancos.DataSource = DadosComum.BancosCombo.Copy().AsDataView();
                bmDesc2DEBITO = new BindingSource();
                DataTable Desc2bancosAltDeb = DadosComumDBF.Desc2BancosCombo.Copy();
                DataRow nova = Desc2bancosAltDeb.NewRow();
                nova.BeginEdit();
                nova["DESC2"] = "00";
                
                // nova["DESCRI_N"] = "00";
                //nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                nova.EndEdit();
                Desc2bancosAltDeb.Rows.Add(nova);
               /* nova = Desc2bancosAltDeb.NewRow();
                nova.BeginEdit();
                nova["DESC2"] = "                         ";

                // nova["DESCRI_N"] = "00";
                //nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                nova.EndEdit();
                Desc2bancosAltDeb.Rows.Add(nova);
               */
                Desc2bancosAltDeb.AcceptChanges();
                bmDesc2DEBITO.DataSource = Desc2bancosAltDeb.AsDataView();

                bmDesc2CREDITO = new BindingSource();
                DataTable Desc2bancosAltCre = DadosComumDBF.Desc2BancosCombo.Copy();
                DataRow novacre = Desc2bancosAltCre.NewRow();
                novacre.BeginEdit();
                novacre["DESC2"] = "00";

                // nova["DESCRI_N"] = "00";
                //nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                novacre.EndEdit();
                Desc2bancosAltCre.Rows.Add(novacre);
               /* novacre = Desc2bancosAltCre.NewRow();
                novacre.BeginEdit();
                novacre["DESC2"] = "                         ";

                // nova["DESCRI_N"] = "00";
                //nova["DESCRI"] = "00 - FALTA LANÇAMENTO";
                novacre.EndEdit();
                Desc2bancosAltCre.Rows.Add(novacre);*/
                Desc2bancosAltCre.AcceptChanges();
                
                bmDesc2CREDITO.DataSource = Desc2bancosAltCre.AsDataView();



                /*bmDesc2Pagar = new BindingSource();
                bmDesc2Pagar.DataSource = DadosComumDBF.Desc2Combo.Copy().AsDataView();
                */

                bmDesc2DEBITO.DataSource = Desc2bancosAltDeb.AsDataView();
                bmDesc2CREDITO.DataSource = Desc2bancosAltCre.AsDataView();



                EdtVauches.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[DATA_ger], new MaskedTextBox());

                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[VALOR_ger], new NumericTextBox());

                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);

                EdtVauches.Add(olinha);

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

                ocombox.DataSource= bmDesc2DEBITO;
                 ocombox.DisplayMember = "DESC2";
                ocombox.ValueMember = "DESC2";
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[DEBITO_ger], ocombox);
                olinha.oedite[0].Width = 250;

                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)olinha.oedite[0]).Enter += EdtFin_EnterDEB;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
               // ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingPermiteVazio);
               // ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoPermiteVazio);

                

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmDesc2CREDITO;
                ocombox.DisplayMember = "DESC2";
                ocombox.ValueMember = "DESC2";
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[CREDITO_ger], ocombox);
                olinha.oedite[1].Width = 250;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)olinha.oedite[1]).Enter += EdtFin_EnterCRE;
              //  ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingPermiteVazio);  //FrmFinan_ComboValidatingBcos
               // ((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidadoPermiteVazio);


                EdtVauches.Add(olinha);

                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[TITULAR_ger], new TextBox());
                EdtVauches.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[HIST_ger], new TextBox());

                EdtVauches.Add(olinha);
                olinha = new Linha("Linha 5");
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[DOC_ger], new TextBox());
                olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[DOC_FISC_ger], new TextBox());
                EdtVauches.Add(olinha);
                if (UseObsGeral)
                {
                    olinha = new Linha("Linha 6");
                    olinha.TextoConfigure(bmSource, oVauches.oDataGridView.Columns[OBS_ger], new TextBox());
                    EdtVauches.Add(olinha);
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void EdtFin_EnterCRE(object sender, EventArgs e)
        {
            if ((sender as ComboBox).DataSource == null)
            {
                (sender as ComboBox).DisplayMember = "DESC2";
                (sender as ComboBox).ValueMember = "DESC2";

                (sender as ComboBox).DataSource = bmDesc2CREDITO;
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }

        }


        private void EdtFin_EnterDEB(object sender, EventArgs e)
        {
            if ((sender as ComboBox).DataSource == null)
            {
                (sender as ComboBox).DisplayMember = "DESC2";
                (sender as ComboBox).ValueMember = "DESC2";
                (sender as ComboBox).DataSource = bmDesc2DEBITO;
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }

        }



 

        void FrmFinan_ComboValidatingBcos(object sender, CancelEventArgs e)
        {

            string texto = ((ComboBox)sender).Text.Trim();
            string selectedtexto = ((ComboBox)sender).SelectedText.Trim();
            // string valuemember = ((ComboBox)sender).ValueMember;
            if ( (texto == "") || (selectedtexto == ""))
            {

                ((ComboBox)sender).SelectedIndex = -1;
                return;
            }

            Boolean ebanco = false;
            try
            {
                Convert.ToInt16((texto.Substring(0, 2)));
                ebanco = true;
            }
            catch (Exception)
            {
            }

            
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
                    retornado = oquery.Table.AsEnumerable().Where(row => row.Field<string>("DESC2").Trim().Substring(0, 2) == texto.Trim().Substring(0, 2)).FirstOrDefault();
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

        void FrmFinan_ComboValidatingPermiteVazio(object sender, CancelEventArgs e)
        {
            string texto = ((ComboBox)sender).Text;

            string selectedtexto = ((ComboBox)sender).SelectedText.Trim();

            if ((texto.Length == 26) && (selectedtexto == "") && (texto.Substring(0, 1) == "*"))
            {
                ((ComboBox)sender).SelectedValue = texto.Substring(0, 25);
            }


          /*  string text = ((ComboBox)sender).SelectedText.Trim();
            if (text.Trim() == "")
            {
                int index = ((ComboBox)sender).SelectedIndex;
                ((ComboBox)sender).SelectedIndex = -1;
                ((ComboBox)sender).SelectedIndex = index;

            }*/
        
        }

        void FrmFinan_CampoValidadoPermiteVazio(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            Error_Set(sender as Control, "");
            if ((sender as Control) is ComboBox)
            {
                string text = ((ComboBox)sender).SelectedText.Trim();
                if (text == "")
                {
                    ((ComboBox)sender).SelectedItem = -1;
                   /* DataRowView orow = (DataRowView)((ComboBox)sender).SelectedItem;
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
                    }*/
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
    }
}