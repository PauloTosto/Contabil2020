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
using ApoioContabilidade.Services;

namespace ApoioContabilidade.UserControls
{
    public partial class ComboBoxMD3 : UserControl
    {
        public ComboBoxMD3()
        {
            InitializeComponent();
            this.Enter += ComboBoxMD_Enter;
            this.Leave += ComboBoxMD_Leave;
            this.TabStop = true;

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
                        if (cbFilho.Focused)
                        {
                            cbNeto.Focus();
                            return true;
                        }
                        break;
                    }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private BindingSource bmPai;
        private BindingSource bmFilho;
        private BindingSource bmNeto;

      
        private BindingSource bmDados;


        private Linha lnPai;

        private Linha lnFilho;



        private string fieldNamePai;
        private string fieldNameFilho;
        private string fieldNameNeto;

        private object origvaluePai;
        private object origvalueFilho;
        private object origvalueNeto;

        private object modifiedvaluePai;
        private object modifiedvalueFilho;
        private object modifiedvalueNeto;


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
        public BindingSource BmNeto
        {
            get { return bmNeto; }
            set { bmNeto = value; }
        }

        public BindingSource BmDados
        {
            get { return bmDados; }
            set { bmDados = value; }
        }



        public ComboBox ComboPai
        {
            get { return this.cbPai; }
            set
            {


                this.cbPai = value;
            }
        }
        public ComboBox ComboFilho
        {
            get { return this.cbFilho; }
            set { this.cbFilho = value; }
        }
        public ComboBox ComboNeto
        {
            get { return this.cbNeto; }
            set { this.cbNeto = value; }
        }
        public void ConfigureComboBoxPaiFilhoNeto(Linha olinha,
           BindingSource obmDados, string ofieldNamePai, string ofieldNameFilho, string ofieldNameNeto, bool quadraValidaVazio = true)
        {
            fieldNameFilho = ofieldNameFilho;
            fieldNamePai = ofieldNamePai;
            fieldNameNeto = ofieldNameNeto;

            BmPai = new BindingSource();
            BmPai.DataSource = DadosComum.SetoresCombo.Copy().AsDataView(); 

            BmFilho = new BindingSource();
            BmFilho.DataSource = DadosComum.GlebasCombo.Copy().AsDataView();

            BmNeto = new BindingSource();
            BmNeto.DataSource = DadosComum.QuadrasCombo.Copy().AsDataView();


            olinha.oedite[0] = this;
            olinha.oedite[0].Width = this.Width;

            BmDados = obmDados;
            //  ComboBox ocombox = new ComboBox();
            ComboPai.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
            ComboPai.DataSource = BmPai; // modelo

            ComboPai.DisplayMember = "CODDESCRI";
            ComboPai.ValueMember = "SETOR";
            
            // olinha.TextoConfigure(bmDetalheCentro, oColunaPai, ocombox);

            ConfigureComboLinha(ComboPai, BmDados, fieldNamePai);
            ComboPai.MaxDropDownItems = 7;
            ComboPai.DropDownStyle = ComboBoxStyle.DropDown;
            ComboPai.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboPai.AutoCompleteSource = AutoCompleteSource.ListItems;
            ComboPai.Validating += ComboValidatingNaoPermiteVazio;
            ComboPai.Validated += CampoValidadoNaoPermiteVazio;
            ComboPai.Leave += Setor_Leave;
            // ComboPai.KeyPress += EdtPgRc_KeyPress;
            ComboPai.KeyDown += EdtPgRc_KeyDown;

            ComboFilho.Tag = "M";// maiusculas e comportamento co combo com bind is TEXT
            ComboFilho.DataSource = BmFilho;  // servico

            ComboFilho.DisplayMember = "GLEBA";
            ComboFilho.ValueMember = "GLEBA";
            ConfigureComboLinhaFilho(ComboFilho, BmDados, fieldNameFilho);
            // olinha.TextoConfigure(bmDetalheCentro, oDetalheCentro.oDataGridView.Columns[SERVICO_centro], ocombox);
            // ComboFilho.Width = 200;
            ComboFilho.MaxDropDownItems = 7;
            ComboFilho.DropDownStyle = ComboBoxStyle.DropDown;
            ComboFilho.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ComboFilho.AutoCompleteSource = AutoCompleteSource.ListItems;
            ComboFilho.Validating += ComboValidatingNaoPermiteVazio;
            ComboFilho.Validated += CampoValidadoNaoPermiteVazio;
            ComboFilho.Enter += GlebaEnter;

            //  Binding b = new Binding("Valor", bmDados, "", false); //, DataSourceUpdateMode.OnValidation);
            // this.DataBindings.Add(b);

            ComboNeto.Tag = "M";// maiusculas e comportamento co combo com bind is TEXT
            ComboNeto.DataSource = BmNeto;  // servico

            ComboNeto.DisplayMember = "DESCRICAO";
            ComboNeto.ValueMember = "QUADRA";
            ConfigureComboLinhaNeto(ComboNeto, BmDados, fieldNameNeto);
            ComboNeto.MaxDropDownItems = 7;
            ComboNeto.DropDownStyle = ComboBoxStyle.DropDown;
            ComboNeto.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            // ((ComboBox)oedite[oindexativo]).AutoCompleteMode = AutoCompleteMode.Append;
            ComboNeto.AutoCompleteSource = AutoCompleteSource.ListItems;

            if (!quadraValidaVazio)
            {
                ComboNeto.Validating += ComboValidatingNaoPermiteVazio;
                ComboNeto.Validated += CampoValidadoNaoPermiteVazio;
            }
            else
            {
                ComboNeto.Validating += ComboValidatingQuadra;
                ComboNeto.Validated += CampoValidado;
            }

            ComboNeto.Enter += QuadraEnter;


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

            

            b.BindingComplete += B_BindingComplete;
            b.NullValue = "";
         
            // b.Format += new ConvertEventHandler(Transform.DbNulltoString);
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
            //if (Convert.ToString(ocombo.Tag) == "I") // inteiro
            //  ocombo.KeyPress += new KeyPressEventHandler(Inteiro_KeyPress);
        }

        private void ConfigureComboLinhaFilho(ComboBox ocombo, BindingSource oBinding, string fieldNameCampo)
        {
            //  cabecalho[oindexativo] = oColuna.HeaderText;
            //oedite[oindexativo].Width = oColuna.Width;
            Boolean formating = true;
            Binding b;
            ocombo.Tag = "M";
           
             b = new Binding
              ("SelectedValue", oBinding, fieldNameCampo, formating, DataSourceUpdateMode.OnValidation);

            

            b.BindingComplete += B_BindingCompleteFilho;
            b.NullValue = "";
            // b.Format += new ConvertEventHandler(Transform.DbNulltoString);
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
            //if (Convert.ToString(ocombo.Tag) == "I") // inteiro
            //  ocombo.KeyPress += new KeyPressEventHandler(Inteiro_KeyPress);
        }

        private void ConfigureComboLinhaNeto(ComboBox ocombo, BindingSource oBinding, string fieldNameCampo)
        {
            //  cabecalho[oindexativo] = oColuna.HeaderText;
            //oedite[oindexativo].Width = oColuna.Width;
            Boolean formating = true;
            Binding b;
            ocombo.Tag = "M";
                b = new Binding
              ("SelectedValue", oBinding, fieldNameCampo, formating, DataSourceUpdateMode.OnValidation);

            

            b.BindingComplete += B_BindingCompleteNeto;
            b.NullValue = "";
            // b.Format += new ConvertEventHandler(Transform.DbNulltoString);
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
            //if (Convert.ToString(ocombo.Tag) == "I") // inteiro
            //  ocombo.KeyPress += new KeyPressEventHandler(Inteiro_KeyPress);
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
        void ComboValidatingQuadra(object sender, CancelEventArgs e)
        {
            string texto = ((ComboBox)sender).Text;
            string valuemember = ((ComboBox)sender).ValueMember;

            if (texto.Trim() == "")
            {
                ((ComboBox)sender).SelectedIndex = -1;
                ((ComboBox)sender).Text = "";
                return;
            }

            if (((ComboBox)sender).SelectedValue == null)
            {
                e.Cancel = true;

                Error_Set(sender as Control);


            }
            else
            {
                if (((ComboBox)sender).SelectedValue.ToString().Trim() == texto.Trim()) { return; }

                DataRow retornado = null;
                try
                {
                    //string textocompara = texto.Trim().PadRight(25,' '); 
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
                Error_Set(sender as Control);
                return;
            }
        }
        void CampoValidado(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            Error_Set(sender as Control, "");

        }


        private void Setor_Leave(object sender, EventArgs e)
        {

            mudancaFiltroPai();
         
        }
        private void Gleba_Leave(object sender, EventArgs e)
        {
            MudancaFiltroFilho();
        }

        


        // na mudança da gleba
        private void GlebaEnter(object sender, EventArgs e)
        {
            mudancaFiltroPai();
        }

        private void QuadraEnter(object sender, EventArgs e)
        {
            MudancaFiltroFilho();
        }


        private void mudancaFiltroPai()
        {
            DataRowView orow = null;
            try
            {
                orow = (bmDados.Current as DataRowView);
            }
            catch (Exception)
            {
            }
            if (orow == null) return;
            string setor = orow[fieldNamePai].ToString();
            string gleba = orow[fieldNameFilho].ToString();
            string filtroantes = BmFilho.Filter;
            string filtro = "SETOR = '" + setor.ToString().Trim() + "'";
            if (filtro != filtroantes)
            {
                string sort = BmFilho.Sort;
                BmFilho.Filter = filtro;
                BmFilho.Sort = "GLEBA";
                bool result = ((BmFilho.DataSource as DataView).Find(gleba.Trim()) != -1);
                BmFilho.Sort = sort;
                if (!result)
                {
                    cbFilho.SelectedIndex = -1;
                    cbNeto.SelectedIndex = -1;
                    cbFilho.Text = "";
                    cbNeto.Text = "";
                }
            }

        }

        private void MudancaFiltroFilho()
        {

            DataRowView orow = null;
            try
            {
                orow = (bmDados.Current as DataRowView);
            }
            catch (Exception)
            {
            }
            if (orow == null) return;
            string gleba = orow[fieldNameFilho].ToString();
            string quadra = orow[fieldNameNeto].ToString();
            string filtroantes = BmNeto.Filter;
            string filtro = "GLEBA = '" + gleba.ToString().Trim() + "'";
            if (filtro != filtroantes)
            {
                string sort = BmNeto.Sort;
                BmNeto.Filter = filtro;
                BmNeto.Sort = "QUADRA";
                bool result = ((BmNeto.DataSource as DataView).Find(quadra.Trim()) != -1);
                BmNeto.Sort = sort;
                if (!result)
                {

                    cbNeto.SelectedIndex = -1;
                    cbNeto.Text = "";
                }

            }
        }




        private void ComboBoxMD_Enter(object sender, EventArgs e)
        {
            this.Tag = 0;
            if (bmDados.Current == null)
            {
                origvaluePai = null;
                origvalueFilho = null;
                origvalueNeto = null;
                modifiedvaluePai = null;
                modifiedvalueFilho = null;
                modifiedvalueNeto = null;

                ComboPai.SelectedIndex = -1;
                ComboFilho.SelectedIndex = -1;
                ComboNeto.SelectedIndex = -1;
            }
            else
            {
                DataRow orow = ((DataRowView)bmDados.Current).Row;
                origvaluePai = orow[fieldNamePai].ToString();
                origvalueFilho = orow[fieldNameFilho].ToString();
                origvalueNeto = orow[fieldNameNeto].ToString();

                modifiedvaluePai = origvaluePai;
                modifiedvalueFilho = origvalueFilho;
                modifiedvalueNeto = origvalueNeto;

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
                string filtro = "SETOR = '" + origvaluePai.ToString().Trim() + "'";
                if (filtro != filtroantes)
                {
                    string sort = BmFilho.Sort;
                    BmFilho.Filter = filtro;
                    BmFilho.Sort = "GLEBA";
                    bool result = ((BmFilho.DataSource as DataView).Find(origvalueFilho.ToString().Trim()) != -1);
                    BmFilho.Sort = sort;
                    if (!result)
                    {
                        origvalueFilho = "";
                    }
                }
                
                if (ComboFilho.SelectedValue == null)
                {
                    if (origvalueFilho.ToString().Trim() != "")
                        ComboFilho.SelectedValue = origvalueFilho.ToString().Trim();
                }
                else
                   if (ComboFilho.SelectedValue.ToString().Trim() != origvalueFilho.ToString().Trim())
                    ComboFilho.SelectedValue = origvalueFilho.ToString().Trim();

                filtroantes = BmNeto.Filter;
                filtro = "GLEBA = '" + origvalueFilho.ToString().Trim() + "'";
                if (filtro != filtroantes)
                {
                    string sort = BmNeto.Sort;
                    BmNeto.Filter = filtro;
                    BmNeto.Sort = "QUADRA";
                    bool result = ((BmNeto.DataSource as DataView).Find(origvalueNeto.ToString().Trim()) != -1);
                    BmNeto.Sort = sort;
                    if (!result)
                    {
                        origvalueNeto = "";
                    }
                }
                if (ComboNeto.SelectedValue == null)
                {
                    if (origvalueNeto.ToString().Trim() != "")
                        ComboNeto.SelectedValue = origvalueNeto.ToString().Trim();
                }
                else
                  if (ComboNeto.SelectedValue.ToString().Trim() != origvalueNeto.ToString().Trim())
                    ComboNeto.SelectedValue = origvalueNeto.ToString().Trim();

            }
          
            cbPai.Focus();
        }
        private void ComboBoxMD_Leave(object sender, EventArgs e)
        {

            modifiedvaluePai = ComboPai.SelectedValue;
            modifiedvalueFilho = ComboFilho.SelectedValue;
            modifiedvalueNeto = ComboNeto.SelectedValue;
            modifiedvaluePai = modifiedvaluePai == null ? "" : modifiedvaluePai;
            modifiedvalueFilho = modifiedvalueFilho == null ? "" : modifiedvalueFilho;
            modifiedvalueNeto = modifiedvalueNeto == null ? "" : modifiedvalueNeto;
            origvaluePai = origvaluePai == null ? "" : origvaluePai;
            origvalueFilho = origvalueFilho == null ? "" : origvalueFilho;
            origvalueNeto = origvalueNeto == null ? "" : origvalueNeto;
            if ((modifiedvalueFilho.ToString().Trim() != origvalueFilho.ToString().Trim()) || 
                (modifiedvaluePai.ToString().Trim() != origvaluePai.ToString().Trim()
                || (modifiedvalueNeto.ToString().Trim() != origvalueNeto.ToString().Trim() )
                ))
            {
                this.Tag = 1;
            }
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
        private void B_BindingComplete(object sender, BindingCompleteEventArgs e)
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
                    // tirei esta linha em 5.09.2020
                    //if ((((ComboBox)bind.Control).SelectedValue != null) && (((ComboBox)bind.Control).SelectedValue.ToString().Trim() != texto.Trim())) 
                    //     ((ComboBox)bind.Control).SelectedValue = texto; 
                    if (((ComboBox)bind.Control).SelectedValue == null) //&& (((ComboBox)bind.Control).DisplayMember == ((ComboBox)bind.Control).ValueMember))
                    {
                        ((ComboBox)bind.Control).SelectedValue = texto.Trim();
                    }
                }
                //  bind.BindingManagerBase.EndCurrentEdit();
            }
            // dados do controle sendo atualizados no datasource
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    if (bind.PropertyName == "SelectedValue")
                    {
                        ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField]
                     = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString().PadRight(2);
                      
                    }
                }
        }

        private void B_BindingCompleteFilho(object sender, BindingCompleteEventArgs e)
        {

            Binding bind = (sender as Binding);
            object texto;
          //  object valuePai;
            try
            {
               // valuePai = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];

                texto = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];
            }
            catch (Exception E)
            {
                return;
            }


            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                if (((ComboBox)bind.Control).SelectedValue == null) // && (valuePai != null))
                {

                    // if (valuePai.ToString().Trim() != "")
                    //{
                    // string filtro = "SETOR = '" + valuePai.ToString().Trim() + "'";
                    // BmFilho.Filter = filtro;
                    ((ComboBox)bind.Control).SelectedValue = texto.ToString().Trim();
                    // }
                }

            }

            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField]
                      = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString().PadRight(4);
                }
        }


        private void B_BindingCompleteNeto(object sender, BindingCompleteEventArgs e)
        {

            Binding bind = (sender as Binding);
            object texto;
          //  object valueFilho;
            try
            {
               // valueFilho = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];

                texto = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];
            }
            catch (Exception E)
            {
                return;
            }


            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                if (((ComboBox)bind.Control).SelectedValue == null)
                     ((ComboBox)bind.Control).SelectedValue = texto.ToString().Trim();
                /*if ((((ComboBox)bind.Control).SelectedValue == null)  //&& (valueFilho != null))
                {

                    if (valueFilho.ToString().Trim() != "")
                    {
                        string filtro = "GLEBA = '" + valueFilho.ToString().Trim() + "'";
                        BmFilho.Filter = filtro;
                        ((ComboBox)bind.Control).SelectedValue = texto.ToString().Trim();
                    }
                }*/

            }

            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                if (e.BindingCompleteState == BindingCompleteState.DataError)
                    e.Binding.BindingManagerBase.CancelCurrentEdit();
                else
                if (e.BindingCompleteState == BindingCompleteState.Success)
                {
                    // quadra
                    ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField]
                        = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString().PadRight(3);
    //                if (((ComboBox)bind.Control).SelectedValue == null)
      //              {
                        //bind.WriteValue
        //            }

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

    }
}
