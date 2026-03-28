using System;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace ClassFiltroEdite.UserControls
{
    public partial class ComboMDInverte3 : UserControl
    {
        // os nomes dos campos que as tabelas "clientes" deste controle utilizam 
        string Neto_NOME = "";
        string Filho_NOME = "";
        private DataTable tabAnterior;


        string Neto_Literal = "GLEBA";
        string Filho_Literal = "SETOR";

        int Neto_Tam = 4;
        int Filho_Tam = 2;


        public ComboMDInverte3()
        {
            InitializeComponent();
            this.Enter += ComboMDInverte_Enter;
            this.Leave += ComboMDInverte_Leave; ;
            this.TabStop = true;


            tabAnterior = new DataTable();
            tabAnterior.Columns.Add("ID", Type.GetType("System.Int32"));
            tabAnterior.Columns.Add(Filho_Literal, Type.GetType("System.String"));
            tabAnterior.Columns[Filho_Literal].MaxLength = Filho_Tam;
            tabAnterior.Columns.Add(Neto_Literal, Type.GetType("System.String"));
            tabAnterior.Columns[Neto_Literal].MaxLength = Neto_Tam;


        }
        public void PegAnterior()
        {
            string textoNeto = cbNeto.Text;
            //if (textoNeto.Trim() == "") return;
            string textoFilho = cbFilho.Text;
            if ((textoNeto.Trim() == "") && (textoFilho.Trim() == "")) return;
            int idMax = 0;
            try
            {
                idMax = tabAnterior.AsEnumerable().Max(row => row.Field<Int32>("ID"));
            }
            catch (Exception)
            {
            }
            // se já existir Altera só a numeração que pertence

            DataRow dataRow = null;
            try
            {
                dataRow = tabAnterior.AsEnumerable().Where(row =>
                  (row.Field<string>(Neto_Literal).Trim() == textoNeto.Trim()) &&
                  (row.Field<string>(Filho_Literal).Trim() == textoFilho.Trim())

                  ).FirstOrDefault(); ;
            }
            catch (Exception)
            {
            }
            idMax++;
            if (dataRow == null)
            {
                dataRow = tabAnterior.NewRow();
                dataRow["ID"] = idMax;
                dataRow[Neto_Literal] = textoNeto;
                dataRow[Filho_Literal] = textoFilho;
                tabAnterior.Rows.Add(dataRow);
                tabAnterior.AcceptChanges();
            }
            else
            {
                dataRow.BeginEdit();
                dataRow["ID"] = idMax;
                dataRow.EndEdit();
                dataRow.AcceptChanges();
            }


        }
        public bool AtiveString()
        {
            bool result = false;
            /*if (Anterior.Count == 0)
            {
                return result;
            }*/
            if (tabAnterior.Rows.Count == 0)
            {
                return result;
            }

            ListBoxDialog listBoxDialog = new ListBoxDialog();
            string texsel = "";
            suspendeValidate = true;
            if (cbNeto.Focused)
            {

                foreach (DataRow orow in tabAnterior.AsEnumerable().OrderByDescending(row => row.Field<Int32>("ID")))
                {
                    string quadra = orow[Neto_Literal].ToString();

                    listBoxDialog.Lines.Add(quadra);
                }
                listBoxDialog.Title = Neto_Literal;
                if (listBoxDialog.Execute())
                {
                    //suspendeValidate = true;
                    texsel = listBoxDialog.SelItem();

                    if (texsel != "")
                    {
                        (cbNeto as ComboBox).SelectedText = texsel;
                    }
                }
                //cbP.Focus();
                suspendeValidate = false;

            }
            else if (cbFilho.Focused)
            {

                foreach (DataRow orow in tabAnterior.AsEnumerable().
                    OrderByDescending(row => row.Field<Int32>("ID")))
                {

                    string gleba = orow[Filho_Literal].ToString();
                    listBoxDialog.Lines.Add(gleba);
                }
                listBoxDialog.Title = Filho_Literal;
                if (listBoxDialog.Execute())
                {
                    // suspendeValidate = true;

                    texsel = listBoxDialog.SelItem();

                    if (texsel.Trim() != "")
                    {
                        // (cbFilho as ComboBox).Text = codserv[1];
                        (cbFilho as ComboBox).SelectedText = texsel;

                    }
                    //cbFilho.Focus();
                    // suspendeValidate = false;
                }
            }
            suspendeValidate = false;
            return result = true;
        }



        #region Propriedades
        // private BindingSource bmPai;
        private BindingSource bmFilho;
        private BindingSource bmNeto;


        private BindingSource bmDados;


        // private Linha lnPai;

        private Linha lnFilho;



        // private string fieldNamePai;
        private string fieldNameFilho;
        private string fieldNameNeto;

        //  private object origvaluePai;
        private object origvalueFilho;
        private object origvalueNeto;

        // private object modifiedvaluePai;
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

        /*public Linha LnPai
        {
            get { return lnPai; }
            set { lnPai = value; }
        }


        public BindingSource BmPai
        {
            get { return bmPai; }
            set { bmPai = value; }
        }
        */
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



        /* public ComboBox ComboPai
         {
             get { return this.cbPai; }
             set
             {


                 this.cbPai = value;
             }
         }*/
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
        #endregion


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
                        AtiveString();
                        return true;
                    }
                case Keys.Enter:
                    {
                        if (cbNeto.Focused)
                        {
                            if (cbNeto.Text.Trim() == "")
                            {
                                cbFilho.TabStop = true;
                                if (cbFilho.TabStop)
                                    cbFilho.Focus();
                                return true;
                            }
                        }
                        if (cbFilho.Focused)
                        {
                        }
                        break;
                    }
                case Keys.Tab:
                    {
                        if (cbNeto.Focused)
                        {
                            if (cbNeto.Text.Trim() == "")
                            {
                                cbFilho.TabStop = true;
                                if (cbFilho.TabStop)
                                    cbFilho.Focus();
                                return true;
                            }
                        }
                        break;
                    }
                case Keys.Escape:
                    {
                        if ((cbNeto.Focused) || (cbFilho.Focused))
                        {

                            if (RetorneVAloresOriginais())
                                return true;
                        }
                        break;
                    }

            }
            return base.ProcessCmdKey(ref msg, keyData);
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
                if (cbNeto.Focused)
                {
                    cbNeto.CausesValidation = false;
                    cbNeto.Text = "";
                    cbNeto.CausesValidation = true;

                    Error_Set(cbNeto, "");
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

            string neto_cur = orow[fieldNameNeto].ToString();
            string filho_cur = orow[fieldNameFilho].ToString();
            //          string paicampo = cbPai.DataBindings[0].BindingMemberInfo.BindingField;
            string neto = "";
            string filho = "";
            foreach (DataColumn ocol in orow.Row.Table.Columns)
            {
                if (ocol.ColumnName.ToUpper().Trim() == Neto_NOME)
                    neto = orow.Row[ocol, DataRowVersion.Original].ToString().Trim();          // == "") && (orow[ocol, DataRowVersion.Original].ToString().Trim() == ""))
                if (ocol.ColumnName.ToUpper().Trim() == Filho_NOME)
                    filho = orow.Row[ocol, DataRowVersion.Original].ToString().Trim();
            }

            if ((neto.Trim() == neto_cur.Trim()) && (filho.Trim() == filho_cur.Trim()))
            {

                if (cbNeto.Focused)
                    Error_Set(cbNeto, "");
                if (cbFilho.Focused)
                    Error_Set(cbFilho, "");
                //his.Focus();
                return retorno;
            }

            return retorno;
        }

        public void ConfigureComboBoxFilhoNeto(Linha olinha,
           BindingSource obmDados, string ofieldNameFilho, string ofieldNameNeto,
           DataView Glebas, DataView Quadras,
           bool quadraValidaVazio = true)
        {
            
            fieldNameFilho = ofieldNameFilho;
            fieldNameNeto = ofieldNameNeto;
            Neto_NOME = fieldNameNeto;
            Filho_NOME = fieldNameFilho;

            BmFilho = new BindingSource();
            BmFilho.DataSource = Glebas;
           
            BmNeto = new BindingSource();
            BmNeto.DataSource = Quadras;


            olinha.oedite[0] = this;
            olinha.oedite[0].Width = this.Width;

            BmDados = obmDados;
            ComboFilho.Tag = "M";// maiusculas e comportamento co combo com bind is TEXT
            ComboFilho.DataSource = BmFilho;  // servico

            ComboFilho.DisplayMember = "CODDESCRI";
            ComboFilho.ValueMember = Filho_Literal;

            ConfigureComboLinhaFilho(ComboFilho, BmDados, fieldNameFilho);
            ComboFilho.MaxDropDownItems = 7;
            ComboFilho.DropDownStyle = ComboBoxStyle.DropDown;
            ComboFilho.AutoCompleteMode = AutoCompleteMode.Suggest;
            ComboFilho.AutoCompleteSource = AutoCompleteSource.ListItems;
            ComboFilho.Validating += ComboValidatingNaoPermiteVazio;
            ComboFilho.Validated += ComboFilho_Validated; ;
            ComboFilho.Enter += GlebaEnter;


            ComboNeto.Tag = "M";// maiusculas e comportamento co combo com bind is TEXT
            ComboNeto.DataSource = BmNeto;  // servico

            ComboNeto.DisplayMember = "DESCRICAO";
            ComboNeto.ValueMember = Neto_Literal;
            ConfigureComboLinhaNeto(ComboNeto, BmDados, fieldNameNeto);
            ComboNeto.MaxDropDownItems = 7;
            ComboNeto.DropDownStyle = ComboBoxStyle.DropDown;
            ComboNeto.AutoCompleteMode = AutoCompleteMode.Suggest;
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
                ComboNeto.Validated += ComboNeto_Validated;       // CampoValidado;
            }

            ComboNeto.Enter += QuadraEnter;
            ComboNeto.Leave += ComboNeto_Leave;

        }




        private void ConfigureComboLinhaFilho(ComboBox ocombo, BindingSource oBinding, string fieldNameCampo)
        {
            Boolean formating = true;
            Binding b;
            ocombo.Tag = "M";

            b = new Binding
             ("SelectedValue", oBinding, fieldNameCampo, formating, DataSourceUpdateMode.OnValidation);



            b.BindingComplete += B_BindingCompleteFilho;
            b.NullValue = "";
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
        }

        private void ConfigureComboLinhaNeto(ComboBox ocombo, BindingSource oBinding, string fieldNameCampo)
        {
            Boolean formating = true;
            Binding b;
            ocombo.Tag = "M";
            b = new Binding
          ("SelectedValue", oBinding, fieldNameCampo, formating, DataSourceUpdateMode.OnValidation);
            b.BindingComplete += B_BindingCompleteNeto;
            b.NullValue = "";
            ocombo.DataBindings.Add(b);
            if (Convert.ToString(ocombo.Tag) == "M") // maiuscula
                ocombo.KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
        }

        #region Binding
        /*private void B_BindingComplete(object sender, BindingCompleteEventArgs e)
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
                    if (((ComboBox)bind.Control).SelectedValue == null) //&& (((ComboBox)bind.Control).DisplayMember == ((ComboBox)bind.Control).ValueMember))
                    {
                        // ((ComboBox)bind.Control).SelectedValue = texto.Trim();
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
        */
        private void B_BindingCompleteFilho(object sender, BindingCompleteEventArgs e)
        {

            Binding bind = (sender as Binding);
            object texto;
            //  object valuePai;
            try
            {
                texto = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField];
            }
            catch (Exception E)
            {
                return;
            }
           /* DataRowView orow = ((DataRowView)bind.BindingManagerBase.Current);
            if ((orow.Row.RowState == DataRowState.Detached) || (orow.Row.RowState == DataRowState.Added))
            {
                texto = ((DataRowView)bind.BindingManagerBase.Current)[bind.BindingMemberInfo.BindingField].ToString();
            }
           */

            if (e.BindingCompleteContext == BindingCompleteContext.ControlUpdate
                && e.Exception == null)
            {
                
                if (((ComboBox)bind.Control).SelectedValue == null) // && (valuePai != null))
                {
                    ((ComboBox)bind.Control).SelectedValue = texto.ToString().Trim();
                }
                else if (texto.ToString().Trim() == "")
                {
                    ((ComboBox)bind.Control).SelectedIndex = -1;
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
                else if (texto.ToString().Trim() == "")
                {
                    ((ComboBox)bind.Control).SelectedIndex = -1;
                };
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

                }
        }
        #endregion

        #region Validações
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
            if (suspendeValidate) return;
            string texto = ((ComboBox)sender).Text;
            string valuemember = ((ComboBox)sender).ValueMember;

            if (texto.Trim() == "")
            {
                ((ComboBox)sender).SelectedIndex = -1;
                ((ComboBox)sender).Text = "";
                ((ComboBox)sender).DataBindings[0].WriteValue();
                //((ComboBox)sender).SelectedValue = "";
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
        private void ComboNeto_Validated(object sender, EventArgs e)
        {

            if (suspendeValidate) return;
            // string textoOrigem = ((ComboBox)sender).
            string quadra = ((ComboBox)sender).Text;
            Error_Set(sender as Control, "");

            if (quadra.Trim() == "")
            {

                cbFilho.TabStop = true;
                return;
            }
            DataRow orowQuadras = null;
            suspendeValidate = true;
            DataRowView orowQuadrasTeste = (BmNeto.Current as DataRowView);
            try
            {
                orowQuadras = (BmNeto.DataSource as DataView).Table.AsEnumerable().Where(row => row.Field<string>(Neto_Literal).Trim() == quadra.Trim()).FirstOrDefault();
            }
            catch (Exception)
            {
            }
            if (orowQuadras != null)
            {
                string gleba = orowQuadras[Filho_Literal].ToString();
                //string setor = orowQuadras[Filho_Literal].ToString();
                cbFilho.SelectedValue = gleba;
                //  cbPai.SelectedValue = setor;
            }
            suspendeValidate = false;
            cbFilho.TabStop = false;

        }
        private void ComboFilho_Validated(object sender, EventArgs e)
        {
            if (suspendeValidate) return;
            string gleba = ((ComboBox)sender).Text;
            Error_Set(sender as Control, "");

            DataRow orow = null;
            suspendeValidate = true;
            try
            {
                orow = (BmFilho.DataSource as DataView).Table.AsEnumerable().Where(row => row.Field<string>(Filho_Literal).Trim() == gleba.Trim()).FirstOrDefault();
            }
            catch (Exception)
            {
            }
            if (orow != null)
            {

                // string setor = orow[Filho_Literal].ToString();
                //cbPai.SelectedValue = setor;
            }
            suspendeValidate = false;
        }

        #endregion





        // na mudança da gleba
        private void GlebaEnter(object sender, EventArgs e)
        {
            // mudancaFiltroPai();
            // cbNeto.TabStop = true;
        }

        private void QuadraEnter(object sender, EventArgs e)
        {
            // MudancaFiltroFilho();
            // cbFilho.TabStop = false;
            // cbPai.TabStop = false;
            cbFilho.SelectionLength = 0;
        }
        private void ComboNeto_Leave(object sender, EventArgs e)
        {

            /*  DataRowView orow = null;
              try
              {
                  orow = (bmDados.Current as DataRowView);
              }
              catch (Exception)
              {
              }
              if (orow == null) return;
              string quadra = (sender as ComboBox).Text.ToString();
              if (quadra.Trim() == "")
              {
                  (sender as ComboBox).SelectedIndex = -1;
                  cbFilho.TabStop = true;
              }
            */
        }




        private void ComboMDInverte_Leave(object sender, EventArgs e)
        {
            // modifiedvaluePai = ComboPai.SelectedValue;
            modifiedvalueFilho = ComboFilho.SelectedValue;
            modifiedvalueNeto = ComboNeto.SelectedValue;
            // modifiedvaluePai = modifiedvaluePai == null ? "" : modifiedvaluePai;
            modifiedvalueFilho = modifiedvalueFilho == null ? "" : modifiedvalueFilho;
            modifiedvalueNeto = modifiedvalueNeto == null ? "" : modifiedvalueNeto;
            // origvaluePai = origvaluePai == null ? "" : origvaluePai;
            origvalueFilho = origvalueFilho == null ? "" : origvalueFilho;
            origvalueNeto = origvalueNeto == null ? "" : origvalueNeto;
            if ((modifiedvalueFilho.ToString().Trim() != origvalueFilho.ToString().Trim())
                //|| (modifiedvaluePai.ToString().Trim() != origvaluePai.ToString().Trim()
                || (modifiedvalueNeto.ToString().Trim() != origvalueNeto.ToString().Trim())
                )
            {
                this.Tag = 1;
            }

        }

        private void ComboMDInverte_Enter(object sender, EventArgs e)
        {
            this.Tag = 0;
            if (bmDados.Current == null)
            {
                // origvaluePai = null;
                origvalueFilho = null;
                origvalueNeto = null;
                // modifiedvaluePai = null;
                modifiedvalueFilho = null;
                modifiedvalueNeto = null;

                // ComboPai.SelectedIndex = -1;
                ComboFilho.SelectedIndex = -1;
                ComboNeto.SelectedIndex = -1;
            }
            else
            {
                DataRow orow = ((DataRowView)bmDados.Current).Row;
                // origvaluePai = orow[fieldNamePai].ToString();
                origvalueFilho = orow[fieldNameFilho].ToString();
                origvalueNeto = orow[fieldNameNeto].ToString();

                //  modifiedvaluePai = origvaluePai;
                modifiedvalueFilho = origvalueFilho;
                modifiedvalueNeto = origvalueNeto;
            }
            

            

            cbNeto.TabStop = true;
            cbNeto.Focus();
            cbFilho.TabStop = false;

            cbFilho.SelectionLength = 0;

            
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
