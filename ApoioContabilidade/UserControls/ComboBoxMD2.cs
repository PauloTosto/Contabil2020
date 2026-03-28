using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ClassFiltroEdite;
//using ApoioContabilidade.Services;

namespace ApoioContabilidade.UserControls
{
    public partial class ComboBoxMD2 : UserControl
    {

        
        string paiBindField = "";
        string filhoBindField = "";
        List<TGuardaUserAnt> lstAnterior;
        class TGuardaUserAnt
        {
            // guarda a relacao pai/filho
            private Dictionary<string,List<string>> Anterior;
        }

        public ComboBoxMD2()
        {
            InitializeComponent();
            this.Enter += ComboBoxMD2_Enter;
            this.Leave += ComboBoxMD2_Leave;
            this.TabStop = true;
            lstAnterior = new List<TGuardaUserAnt>();
        }

        public void PegAnterior()
        {

        }

        protected override bool ProcessCmdKey(
           ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                //case Keys.Left:
                //case Keys.Right:
                // case Keys.Up:
                case Keys.F2:
                    {
                        return true;
                    }
                case Keys.Enter:
                    {
                        if (cbPai.Focused)
                        {
                            cbFilho.Focus();
                            return true;
                        }
                        break;
                    }
                case Keys.Escape:
                    {
                        if ((cbPai.Focused) || (cbFilho.Focused))
                        {

                            if (RetorneVAloresOriginais())
                                return true;
                        }
                        break;
                    }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private BindingSource bmPai;
        private BindingSource bmFilho;

        private BindingSource bmDados;


        private Linha lnPai;

        private Linha lnFilho;



        private string fieldNamePai;
        private string fieldNameFilho;

        private object origvaluePai;
        private object origvalueFilho;

        private object modifiedvaluePai;
        private object modifiedvalueFilho;


        private bool suspendeValidate = false;

        private object valor;
        

        public object Valor
        {
            get { return valor; }
            set { valor = value; }
        }

        public Linha LnFilho
        {
            get { return lnFilho; }
            set { lnFilho = value; }
        }

        public Linha LnPai
        {
            get { return lnPai; }
            set { lnPai = value; }
        }


        public BindingSource BmPai
        {
            get { return bmPai; }
            set { bmPai = value; }
        }

        public BindingSource BmFilho
        {
            get { return bmFilho; }
            set { bmFilho = value; }
        }

        public BindingSource BmDados
        {
            get { return bmDados; }
            set { bmDados = value; }
        }



        public ComboBox ComboPai
        {
            get { return this.cbPai; }
            set {


                this.cbPai = value;
            }
        }
        public ComboBox ComboFilho
        {
            get { return this.cbFilho; }
            set { this.cbFilho = value; }
        }

        public void ConfigureComboBoxPaiFilho(Linha olinha,
            BindingSource bmDetalheCentro, string ofieldNamePai, string ofieldNameFilho,
            DataView Modelo, DataView ServicoModelo
            )
        {
            fieldNameFilho = ofieldNameFilho;
            fieldNamePai = ofieldNamePai;

            BmPai = new BindingSource();
            BmPai.DataSource = Modelo;   //DadosComum.ModeloCombo.Copy().AsDataView();
            BmPai.Sort = "DESCRICAO";

            BmFilho = new BindingSource();
            BmFilho.DataSource = ServicoModelo;  //DadosComum.ServicoModeloCombo.Copy().AsDataView();
            BmFilho.Sort = "DESCSERV";


            olinha.oedite[0] = this;
            olinha.oedite[0].Width = this.Width;

            BmDados = bmDetalheCentro;
            //  ComboBox ocombox = new ComboBox();
            ComboPai.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
            ComboPai.DataSource = BmPai; // modelo

            ComboPai.DisplayMember = "DESCRICAO";
            ComboPai.ValueMember = "NUM_MOD";
            // olinha.TextoConfigure(bmDetalheCentro, oColunaPai, ocombox);

            ConfigureComboLinha(ComboPai, BmDados, fieldNamePai);
            ComboPai.MaxDropDownItems = 7;
            ComboPai.DropDownStyle = ComboBoxStyle.DropDown;
            ComboPai.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboPai.AutoCompleteSource = AutoCompleteSource.ListItems;

            ComboPai.Validating += ComboValidatingNaoPermiteVazio;
            ComboPai.Validated += CampoValidadoNaoPermiteVazio;
            ComboPai.Leave += Modelo_Leave;
            // ComboPai.KeyPress += EdtPgRc_KeyPress;
            ComboPai.KeyDown += EdtPgRc_KeyDown;






            ComboFilho.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
            ComboFilho.DataSource = BmFilho;  // servico

            ComboFilho.DisplayMember = "DESCSERV";
            ComboFilho.ValueMember = "CODSER";
            ConfigureComboLinhaFilho(ComboFilho, BmDados, fieldNameFilho);
            // olinha.TextoConfigure(bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[SERVICO_centro], ocombox);
          // ComboFilho.Width = 200;
            ComboFilho.MaxDropDownItems = 7;
            ComboFilho.DropDownStyle = ComboBoxStyle.DropDown;
            ComboFilho.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboFilho.AutoCompleteSource = AutoCompleteSource.ListItems;

            ComboFilho.Validating += ComboValidatingNaoPermiteVazio;
            ComboFilho.Validated += CampoValidadoNaoPermiteVazio;
            ComboFilho.Enter += BoxServicoEnter;

     
            

        }
        private void ConfigureComboLinha(ComboBox ocombo, BindingSource oBinding, string fieldNameCampo)
        {
            //  cabecalho[oindexativo] = oColuna.HeaderText;
            //oedite[oindexativo].Width = oColuna.Width;
            Boolean formating = true;
            Binding b;
            ocombo.Tag = "M";
                b = new Binding
              ("SelectedValue", oBinding, fieldNameCampo, formating, DataSourceUpdateMode.OnValidation);

            b.BindingComplete += B_BindingCompletePaieFilho;
            b.NullValue = "";
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
        }

        private void ConfigureComboLinhaFilho(ComboBox ocombo, BindingSource oBinding, string fieldNameCampo)
        {
            Boolean formating = true;
            Binding b;
            ocombo.Tag = "M";
            
                b = new Binding
              ("SelectedValue", oBinding, fieldNameCampo, formating, DataSourceUpdateMode.OnValidation);

            b.BindingComplete += B_BindingCompletePaieFilho;
            b.NullValue = "";
           
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
           
        }

        void ComboValidatingNaoPermiteVazio(object sender, CancelEventArgs e)
        {
            if (suspendeValidate) return;
            string texto = ((ComboBox)sender).Text;

            string selectedtexto = ((ComboBox)sender).SelectedText.Trim();

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

        void CampoValidadoNaoPermiteVazio(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            if (suspendeValidate) return;
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
        private void Modelo_Leave(object sender, EventArgs e)
        {
            mudancaFiltroPai();
        

        }
        private void BoxServicoEnter(object sender, EventArgs e)
        {
            mudancaFiltroPai();
         
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


        private bool RetorneVAloresOriginais()
        {
            bool retorno = false;
            DataRowView orow = null;
            try
            {
                orow = (BmDados.Current as DataRowView);
            }
            catch (Exception)
            {
            }
            if (orow == null) return retorno;

            if ((orow.Row.RowState == DataRowState.Added) || (orow.Row.RowState == DataRowState.Detached))
            {
                if (cbPai.Focused)
                {
                    cbPai.CausesValidation = false;
                    cbPai.Text = "";
                    cbPai.CausesValidation = true;
                    Error_Set(cbPai, "");
                }
                if (cbFilho.Focused)
                {
                    cbFilho.CausesValidation = false;
                    cbFilho.Text = "";
                    cbFilho.CausesValidation = true;

                    Error_Set(cbFilho, "");
                }
               //his.Focus();
                return retorno;
            }
            
            string pai_cur = orow[fieldNamePai].ToString();
            string filho_cur = orow[fieldNameFilho].ToString();
            //          string paicampo = cbPai.DataBindings[0].BindingMemberInfo.BindingField;
            string pai = "";
            string filho = "";
            foreach (DataColumn ocol in orow.Row.Table.Columns)
            {
                if (ocol.ColumnName.ToUpper().Trim() == "NUM_MOD")
                    pai = orow.Row[ocol, DataRowVersion.Original].ToString().Trim();          // == "") && (orow[ocol, DataRowVersion.Original].ToString().Trim() == ""))
                if (ocol.ColumnName.ToUpper().Trim() == "CODSER")
                    filho = orow.Row[ocol, DataRowVersion.Original].ToString().Trim();
            }

            if ( (pai.Trim() == pai_cur.Trim()) && (filho.Trim() == filho_cur.Trim()))
            {

                if (cbPai.Focused)
                    Error_Set(cbPai, "");
                if (cbFilho.Focused)
                    Error_Set(cbFilho, "");
               //his.Focus();
                return retorno;
            }
            retorno = true;


            string filtroantes = BmFilho.Filter;
            string filtro = "NUM_MOD = '" + pai.ToString().Trim() + "'";
            if (filtro != filtroantes)
            {
                BmFilho.Filter = "";
                BmFilho.Filter = filtro;
                BmFilho.Sort = "CODSER";
                 
            }
            //else BmFilho.Filter = filtro;
            cbPai.CausesValidation = false;
            cbPai.SelectedValue = pai;
            cbPai.CausesValidation = true;
            cbFilho.CausesValidation = false;
            cbFilho.SelectedValue = filho;
            cbFilho.CausesValidation = true;
            Error_Set(cbPai, "");
            Error_Set(cbFilho, "");
            return retorno;
        }






        private void mudancaFiltroPai()
        {
            DataRowView orow = null;
            try
            {
                orow = (BmDados.Current as DataRowView);
            }
            catch (Exception)
            {
            }
            if (orow == null) return;

            string pai = orow[fieldNamePai].ToString();
            string filho = orow[fieldNameFilho].ToString();
            string filtroantes = BmFilho.Filter;
            string filtro = "NUM_MOD = '" + pai.ToString().Trim() + "'";
            if (filtro != filtroantes)
            {
                string sort = BmFilho.Sort;
                BmFilho.Filter = filtro;
                BmFilho.Sort = "CODSER";
                bool result = ((BmFilho.DataSource as DataView).Find(filho.Trim()) != -1);
                BmFilho.Sort = sort;
                if (!result)
                {
                    cbFilho.SelectedIndex = -1;
                    cbFilho.Text = "";
                }
            }
        }


        private void B_BindingCompletePaieFilho(object sender, BindingCompleteEventArgs e)
        {
            string textopai;
            string textofilho;
            Binding bind = (sender as Binding);
            string campo = bind.BindingMemberInfo.BindingField;
            try
            {

                textopai = ((DataRowView)bind.BindingManagerBase.Current)["NUM_MOD"].ToString();
                textofilho = ((DataRowView)bind.BindingManagerBase.Current)["CODSER"].ToString();
            }
            catch (Exception E)
            {
                return;
            }

            // dados do datasource sendo atualizados nos controles
            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                if (bind.PropertyName == "SelectedValue")
                {
                    if (campo == "CODSER")
                    {
                        try
                        {
                            ((ComboBox)bind.Control).SelectedValue = textofilho.Trim();
                        }
                        catch (Exception)
                        {
                        }
                      
                        if ((((ComboBox)bind.Control).SelectedValue == null) && (textofilho.Trim() != "") && (textopai.Trim() != ""))
                        {
                            string filtroantes = BmFilho.Filter;
                            string filtro = "NUM_MOD = '" + textopai.Trim() + "'";
                            if (filtro != filtroantes)
                            {
                                string sort = BmFilho.Sort;
                                BmFilho.Filter = filtro;
                                BmFilho.Sort = "CODSER";
                                ((ComboBox)bind.Control).SelectedValue = textofilho.Trim();
                            }
                        }
                    }
                    else if (campo == "NUM_MOD")
                    {

                        if ((((ComboBox)bind.Control).SelectedValue == null) || (((ComboBox)bind.Control).SelectedValue.ToString().Trim() != textopai.Trim()))
                        {
                            try
                            {
                                ((ComboBox)bind.Control).SelectedValue = textopai.Trim();
                            }
                            catch (Exception)
                            {
                            }
                           
                            if (((ComboBox)bind.Control).SelectedValue != null)
                            {
                                string filtroantes = BmFilho.Filter;
                                string filtro = "NUM_MOD = '" + textopai.Trim() + "'";
                                if (filtro != filtroantes)
                                {
                                    string sort = BmFilho.Sort;
                                    BmFilho.Filter = filtro;
                                    BmFilho.Sort = "CODSER";
                                    ((ComboBox)bind.Control).SelectedValue = textofilho.Trim();
                                }
                            }

                        }
                    }
                    
                }

            }
            // dados do controle sendo atualizados no datasource
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField]
                     = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString().PadRight(2);

                }
        }

        private void B_BindingCompletePai(object sender, BindingCompleteEventArgs e)
        {
            string texto;
            Binding bind = (sender as Binding);
            try
            {
                texto = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString();
            }
            catch (Exception E)
            {
                return;
            }

            // dados do datasource sendo atualizados nos controles
            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                if (bind.PropertyName == "SelectedValue")
                {
                   // if (((ComboBox)bind.Control).SelectedValue == null) 
                    //{
                        ((ComboBox)bind.Control).SelectedValue = texto.Trim();
                   // }
                }
                
            }
            // dados do controle sendo atualizados no datasource
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField]
                     = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString().PadRight(2);
                  
                }
        }

        private void B_BindingCompleteFilho(object sender, BindingCompleteEventArgs e)
        {

            Binding bind = (sender as Binding);
            object texto;
           // object valuePai;
            try
            {
                //valuePai = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];

                texto = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];
            }
            catch (Exception E)
            {
                return;
            }


            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                ((ComboBox)bind.Control).SelectedValue = texto.ToString().Trim();
          
            }

            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField]
               = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString().PadRight(3);


                }
        }

        void Maiuscula_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((e.KeyChar >= 'a') && (e.KeyChar <= 'z'))
            {
                e.Handled = true;
                SendKeys.Send(e.KeyChar.ToString().ToUpper());
            }

        }
        private void Inteiro_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!(((e.KeyChar <= '9') && (e.KeyChar >= '0')) ||
               (e.KeyChar == (Char)8)))
            {
                e.Handled = true;
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

        private void ComboBoxMD2_Enter(object sender, EventArgs e)
        {
           
            this.Tag = 0;
            if (bmDados.Current == null)
            {
                origvaluePai = null;
                origvalueFilho = null;
                modifiedvaluePai = null;
                modifiedvalueFilho = null;
                ComboPai.SelectedIndex = -1;
                ComboFilho.SelectedIndex = -1;
            }
            else
            {
                DataRow  orow = ((DataRowView)bmDados.Current).Row;
                origvaluePai = orow[fieldNamePai].ToString();
                origvalueFilho = orow[fieldNameFilho].ToString();
                modifiedvaluePai = origvaluePai;
                modifiedvalueFilho = origvalueFilho;
                if (ComboPai.SelectedValue == null)
                {
                    if (origvaluePai.ToString().Trim() != "")
                        ComboPai.SelectedValue = origvaluePai.ToString().Trim();
                }
                else
                if (ComboPai.SelectedValue.ToString().Trim() != origvaluePai.ToString().Trim())
                    ComboPai.SelectedValue = origvaluePai.ToString().Trim();
                // testa o filtro do filho(gleba)
                string filtroantes = BmFilho.Filter;
                string filtro = "NUM_MOD = '" + origvaluePai.ToString().Trim() + "'";
                if (filtro != filtroantes)
                {
                    string sort = BmFilho.Sort;
                    BmFilho.Filter = filtro;
                    BmFilho.Sort = "CODSER";
                    try
                    {
                        bool result = ((BmFilho.DataSource as DataView).Find(origvalueFilho.ToString().Trim()) != -1);
                        BmFilho.Sort = sort;
                        if (!result)
                        {
                            origvalueFilho = "";
                        }
                    }
                    catch (Exception)
                    {

                    }
                   
                }
                try
                {
                    if (ComboFilho.SelectedValue == null)
                    {
                        if (origvalueFilho.ToString().Trim() != "")
                            ComboFilho.SelectedValue = origvalueFilho.ToString().Trim();
                    }
                    else
                   if (ComboFilho.SelectedValue.ToString().Trim() != origvalueFilho.ToString().Trim())
                        ComboFilho.SelectedValue = origvalueFilho.ToString().Trim();

                }
                catch (Exception)
                {

                }
                
                /* string filtroantes = BmFilho.Filter;
                 string filtro = "NUM_MOD = '" + origvaluePai.ToString().Trim()+ "'";
                 if (filtro == filtroantes) return;
                 BmFilho.Filter = filtro;
                 BmFilho.Sort = "DESCSERV ASC";
                 bool dentro = DadosComum.campoServicoSource(origvaluePai.ToString().Trim(), origvalueFilho.ToString().Trim());
                 if (dentro)
                 {
                     ComboFilho.SelectedValue = origvalueFilho.ToString().Trim();
                 }
                 ComboPai.SelectedValue = origvaluePai;*/
            }
            if (!this.cbFilho.Focused)
            {
                suspendeValidate = true;
                cbPai.Focus();
                suspendeValidate = false;
            }
           
        }
        private void ComboBoxMD2_Leave(object sender, EventArgs e)
        {

            modifiedvaluePai = ComboPai.SelectedValue;
            modifiedvalueFilho = ComboFilho.SelectedValue;
            
            modifiedvaluePai = modifiedvaluePai == null ? "" : modifiedvaluePai;
            modifiedvalueFilho = modifiedvalueFilho == null ? "" : modifiedvalueFilho;
            origvaluePai = origvaluePai == null ? "" : origvaluePai;
            origvalueFilho = origvalueFilho == null ? "" : origvalueFilho;
            if ((modifiedvalueFilho.ToString().Trim() != origvalueFilho.ToString().Trim()) 
                || (modifiedvaluePai.ToString().Trim() != origvaluePai.ToString().Trim()))
            {
                this.Tag = 1;
            }
        }
    }
}
