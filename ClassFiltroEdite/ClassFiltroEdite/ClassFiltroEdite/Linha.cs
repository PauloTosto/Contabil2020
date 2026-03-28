using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Data;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ClassFiltroEdite
{
   //classes Linha e TRansform
    public class Linha
    {
        public List<string> cabecalho;
        public ponteiroPesquisa ofuncao;
        public ponteiroLinhaSql ofuncaoSql;
        public ponteiroLinhaSqlComplexo ofuncaoSqlDictionary;
        public List<Control> oedite;
        public List<string> dado;
        public List<string> vdado; // valores default
        public List<System.ComponentModel.Component> oparent;
        public string titulo;
        private int index;
        public int indexativo
        {
            get { return index; }
            set
            {
                if ((value > Apoio.ind) || (value < 0))
                {
                    index = Apoio.ind;
                    throw new Exception("Indice fora dos limites");
                }
                index = value;
            }
        }
        public Linha(string value)
        {
            index = -1;
            titulo = value;
            oedite = new List<Control>(Apoio.ind);
            for (int j = 0; j < Apoio.ind; j++)
                oedite.Add(null);
            cabecalho = new List<string>(Apoio.ind);
            for (int j = 0; j < Apoio.ind; j++)
                cabecalho.Add("");
            ofuncao = null;
            ofuncaoSql = null;
            dado = new List<string>(Apoio.ind);
            for (int j = 0; j < Apoio.ind; j++)
                dado.Add("");
            vdado = new List<string>(Apoio.ind);
            for (int j = 0; j < Apoio.ind; j++)
                vdado.Add("");
            oparent = new List<System.ComponentModel.Component>(Apoio.ind);
            for (int j = 0; j < Apoio.ind; j++)
                oparent.Add(null);
        }
        public Control ControlByFieldName(string fieldName)
        {
            Control ocontrol = null;
            for (int j = 0; j < oedite.Count; j++)
            {
                if (oedite[j] == null) break;

                if (((oedite[j] as Control).DataBindings != null) && ((oedite[j] as Control).DataBindings.Count > 0))
                {
                    bool existe = false;
                    try
                    {
                        existe = (oedite[j] as Control).DataBindings[0].BindingMemberInfo.BindingField.Trim().ToUpper() == fieldName.ToUpper();
                    }
                    catch (Exception) {}
                    if (existe)
                    {
                        ocontrol = oedite[j];
                        break;
                    }
                }
            }
            return ocontrol;
        }
        public Control ControlByIndex(int index)
        {
            Control ocontrol = null;
            if (index >= oedite.Count) return ocontrol;
            if (oedite[index] == null) return ocontrol;
            ocontrol = oedite[index];
            return ocontrol;
        }
        private int IndexAtivoSeguinte()
        {
            int j;
            for (j = 0; j < Apoio.ind; j++)
            { if (oedite[j] == null) break; }
            if (j == Apoio.ind)
            {
                oedite.Add(null);
                cabecalho.Add("");
                dado.Add("");
                vdado.Add("");
                oparent.Add(null);
                j = oedite.Count - 1;
            }
            return j;
        }
        private void Linha_KeyPress(object sender, KeyPressEventArgs e)
        {
           
            //throw new NotImplementedException();
        }

        private void Linha_ValueChanged(object sender, EventArgs e)
        {
            //SendKeys.Send("{RIGHT}");
        }

        
        public void TextoConfigure(BindingSource oBinding, DataGridViewColumn oColuna, Control novocontrol)
        {
            TextoConfigure(oBinding, oColuna, novocontrol, true, null);
        }
        public void TextoConfigure(BindingSource oBinding, DataGridViewColumn oColuna, Control novocontrol, Boolean autocomplete, AutoCompleteStringCollection Colecao)
        {
            // cria para o indice ativo um controle edite tipo Texto

            Boolean formating = true;   //é o que permite o bmsource manejar as edicoes..
            int oindexativo = -1;
            if ((indexativo == -1) || (indexativo > Apoio.ind))
                oindexativo = IndexAtivoSeguinte();
            else
                oindexativo = indexativo;
            this.oedite[oindexativo] = novocontrol;
            DataTable otable = null;
            if ((oBinding.DataSource != null) && ((oBinding.DataSource is DataTable) || (oBinding.DataSource is DataView)))
            {
                if (oBinding.DataSource is DataTable) { otable = oBinding.DataSource as DataTable; }
                else { { otable = (oBinding.DataSource as DataView).Table; } }
                
            }




            if (novocontrol is NumericTextBox)
            {
                cabecalho[oindexativo] = oColuna.HeaderText;
                ((NumericTextBox)oedite[oindexativo]).TextAlign = HorizontalAlignment.Right;
                ((NumericTextBox)oedite[oindexativo]).Width = oColuna.Width;
                // verifica se o valornumerico é decimal ou double 
                Binding b = new Binding
                ("Text", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation, 0);
                Type tiponumerico = null;
                if (otable != null)
                   tiponumerico = otable.Columns[oColuna.DataPropertyName].DataType;
                
                if (tiponumerico == Type.GetType("System.Decimal"))
                {
                    b.Parse += new ConvertEventHandler(Transform.CurrencyStringToDecimal);
                    b.Format += new ConvertEventHandler(Transform.DecimalToCurrencyString);
                }
                else if (tiponumerico == Type.GetType("System.Double"))
                {
                    b.Parse += new ConvertEventHandler(Transform.CurrencyStringToDouble);
                    b.Format += new ConvertEventHandler(Transform.DoubleToCurrencyString);
                }
                else
                {
                    b.Parse += new ConvertEventHandler(Transform.CurrencyStringToDecimal);
                    b.Format += new ConvertEventHandler(Transform.DecimalToCurrencyString);
                }
                ((NumericTextBox)oedite[oindexativo]).DataBindings.Add(b);
                return;
            }

            if (novocontrol is TextBox)
            {
                cabecalho[oindexativo] = oColuna.HeaderText;
                //((TextBox)oedite[oindexativo]).Validated += new EventHandler(TextBox_Validated);
                ((TextBox)oedite[oindexativo]).CharacterCasing = CharacterCasing.Upper;
                // ((TextBox)oedite[oindexativo]).AutoCompleteMode = 
                ((TextBox)oedite[oindexativo]).Multiline = false;
                if (autocomplete)
                {
                    ((TextBox)oedite[oindexativo]).AutoCompleteMode = AutoCompleteMode.Append;
                    if (Colecao == null)
                        ((TextBox)oedite[oindexativo]).AutoCompleteCustomSource = new AutoCompleteStringCollection();
                    else
                        ((TextBox)oedite[oindexativo]).AutoCompleteCustomSource = Colecao;
                    ((TextBox)oedite[oindexativo]).AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                if (otable != null)
                {
                    if (otable.Columns.Contains(oColuna.DataPropertyName))
                    {
                        if (otable.Columns[oColuna.DataPropertyName].MaxLength > 0)
                           ((TextBox)oedite[oindexativo]).MaxLength = otable.Columns[oColuna.DataPropertyName].MaxLength;
                    }
                }
                Binding b = new Binding("Text", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation, "");
                // b.Format += new ConvertEventHandler(Transform.DbNulltoString);
                ((TextBox)oedite[oindexativo]).DataBindings.Add(b);
                oedite[oindexativo].Width = oColuna.Width;
                return;
            }
            if (novocontrol is MaskedTextBox)
            {
                cabecalho[oindexativo] = oColuna.HeaderText;
                //((TextBox)oedite[oindexativo]).Validated += new EventHandler(TextBox_Validated);
                ((MaskedTextBox)oedite[oindexativo]).Mask = "00/00/0000";
                // ((TextBox)oedite[oindexativo]).AutoCompleteMode = 
                ((MaskedTextBox)oedite[oindexativo]).PromptChar = (Char)32; // space no lugar do prompt padrao que é _
                ((MaskedTextBox)oedite[oindexativo]).InsertKeyMode = InsertKeyMode.Default;
                //((MaskedTextBox)oedite[oindexativo]).KeyDown += Linha_KeyDown;
                ((MaskedTextBox)oedite[oindexativo]).Enter += Linha_Enter;
                // ((MaskedTextBox)oedite[oindexativo]).GotFocus += Linha_GotFocus;
                
                if (otable != null)
                {
                    if (otable.Columns.Contains(oColuna.DataPropertyName))
                    {
                        if (otable.Columns[oColuna.DataPropertyName].MaxLength > 0)
                            ((MaskedTextBox)oedite[oindexativo]).MaxLength = otable.Columns[oColuna.DataPropertyName].MaxLength;
                    }
                }
                Binding b = new Binding("Text", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation, "");
                // b.Format += new ConvertEventHandler(Transform.DbNulltoString);
                ((MaskedTextBox)oedite[oindexativo]).DataBindings.Add(b);
                oedite[oindexativo].Width = oColuna.Width;
                return;
            }
            
            if ((novocontrol is CustomDateTimePicker) || (novocontrol is DateTimePicker))
            {
                if (novocontrol is CustomDateTimePicker)
                {
                    oedite[oindexativo].Width = oColuna.Width + 8;
                    cabecalho[oindexativo] = oColuna.HeaderText;
                    Binding b = new Binding("Value", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation, DateTime.Now.Date);
                    b.NullValue = DateTime.Now.Date;

                   // ((CustomDateTimePicker)oedite[oindexativo]).Format = DateTimePickerFormat.Short;

                    ((CustomDateTimePicker)oedite[oindexativo]).DataBindings.Add(b);

                }
                else
                {
                    oedite[oindexativo].Width = oColuna.Width + 8;
                    cabecalho[oindexativo] = oColuna.HeaderText;
                    Binding b = new Binding("Value", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation, DateTime.Now.Date);
                    b.NullValue = DateTime.Now.Date;

                    // b.Format += new ConvertEventHandler(Transform.DbNulltoDate);
                    ((DateTimePicker)oedite[oindexativo]).Format = DateTimePickerFormat.Short;

                    ((DateTimePicker)oedite[oindexativo]).DataBindings.Add(b);
                }
            }
            if (novocontrol is ComboBox)
            {
                cabecalho[oindexativo] = oColuna.HeaderText;
                oedite[oindexativo].Width = oColuna.Width;
                Binding b;
                if (Convert.ToString((object)((ComboBox)oedite[oindexativo]).Tag) == "R")
                {
                    ((ComboBox)oedite[oindexativo]).Tag = "M";
                    b = new Binding
                         ("Text", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation);
                }
                else
                {
                    if ((((ComboBox)oedite[oindexativo]).ValueMember.Trim() != "")
                    && (((ComboBox)oedite[oindexativo]).DisplayMember == ((ComboBox)oedite[oindexativo]).ValueMember))
                    {
                        b = new Binding
                       ("Text", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation);
                    }
                    else
                    {
                        b = new Binding
                      ("SelectedValue", oBinding, oColuna.DataPropertyName, formating, DataSourceUpdateMode.OnValidation);

                    }
                }

              //  b.BindingComplete += B_BindingComplete;
                b.NullValue = "";
                // b.Format += new ConvertEventHandler(Transform.DbNulltoString);
                ((ComboBox)oedite[oindexativo]).AutoCompleteMode = AutoCompleteMode.Append;
                ((ComboBox)oedite[oindexativo]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)oedite[oindexativo]).DataBindings.Add(b);
                if (Convert.ToString((object)((ComboBox)oedite[oindexativo]).Tag) == "M") // maiuscula
                    ((ComboBox)oedite[oindexativo]).KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
                if (Convert.ToString((object)((ComboBox)oedite[oindexativo]).Tag) == "I") // inteiro
                    ((ComboBox)oedite[oindexativo]).KeyPress += new KeyPressEventHandler(Inteiro_KeyPress);

            }
        }
        private void Linha_Enter(object sender, EventArgs e)
        {
           // SendKeys.Send("{HOME}");
            //SendKeys.Send("{LEFT}}");
            
            (sender as MaskedTextBox).SelectionStart = 0;
            (sender as MaskedTextBox).SelectionLength = 10;
        }
        /*
        private void Linha_GotFocus(object sender, EventArgs e)
        {
            // SendKeys.Send("{HOME}");
           // (sender as MaskedTextBox).SelectionStart = 0;
           // (sender as MaskedTextBox).SelectionLength = 0;
        }

       

        private void Linha_KeyDown(object sender, KeyEventArgs e)
        {
            // fonte : https://social.msdn.microsoft.com/Forums/pt-BR/6185eb08-2c4e-4c8e-ab71-8318b87fa7eb/mascara-data-no-textbox?forum=vscsharppt
            if (e.KeyCode == Keys.Enter)
            {
                SendKeys.Send((e.Shift ? "+" : "") + "{TAB}");
            }
        }
        */
        // CUSTOMIZEI O COMBOBOX PARA ACEITAR QUALQUER VALOR AGOSTO/2020
        // TIREI ESTE EM 8/11/2020
        private void B_BindingComplete(object sender, BindingCompleteEventArgs e)
        {
            /*string texto;
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
                    if ((((ComboBox)bind.Control).SelectedValue == null) && (((ComboBox)bind.Control).DisplayMember == ((ComboBox)bind.Control).ValueMember))
                    {
                        ((ComboBox)bind.Control).Text = texto;
                    }
                }
            }
            if (e.BindingCompleteContext == BindingCompleteContext.DataSourceUpdate
                 && e.Exception == null)
                 if (e.BindingCompleteState == BindingCompleteState.DataError)
                     e.Binding.BindingManagerBase.CancelCurrentEdit();
                 else
                 if (e.BindingCompleteState == BindingCompleteState.Success)
                 {
                     if (bind.PropertyName == "SelectedValue")
                     {
                         if (((ComboBox)bind.Control).SelectedValue == null)
                         {
                         }
                     }
                 } */
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

        //// PARA SERVIR AO PESQUISA (17/10/2020)
        ///BindingSource oBinding, DataGridViewColumn oColuna,
        public void TextoPesquisaConfigure( Control novocontrol,
            
            Boolean autocomplete = true)
        {
            
            if (novocontrol is TextBox)
            {
                (novocontrol as TextBox).CharacterCasing = CharacterCasing.Upper;
                // ((TextBox)oedite[oindexativo]).AutoCompleteMode = 
                (novocontrol as TextBox).Multiline = false;
                if (autocomplete)
                {
                    (novocontrol as TextBox).AutoCompleteMode = AutoCompleteMode.Append;
                    (novocontrol as TextBox).AutoCompleteCustomSource = new AutoCompleteStringCollection();
                    (novocontrol as TextBox).AutoCompleteSource = AutoCompleteSource.CustomSource;
                }
                return;
            }

            if (novocontrol is DateTimePicker)
            {
                (novocontrol as DateTimePicker).Format = DateTimePickerFormat.Short;
            }
            if (novocontrol is ComboBox)
            {
                (novocontrol as ComboBox).KeyPress += new KeyPressEventHandler(Maiuscula_KeyPress);
            
            }
        }

    }
    static public class Transform
    {
        static public void DecimalToCurrencyString(object sender, ConvertEventArgs cevent)
        {
            
            // The application can only convert to string type. 
            if (Convert.IsDBNull(cevent.Value))
            {
                cevent.Value = ((decimal)0).ToString("F");
                return;
            }
            if (cevent.DesiredType != typeof(string)) return;

            cevent.Value = ((decimal)cevent.Value).ToString("F");//("c");
        }

        static public void CurrencyStringToDecimal(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(decimal)) return;

            cevent.Value = Decimal.Parse(cevent.Value.ToString(),
              NumberStyles.Float, null);
        }
        static public void DoubleToCurrencyString(object sender, ConvertEventArgs cevent)
        {
     
            // The application can only convert to string type. 
            if (Convert.IsDBNull(cevent.Value))
            {
                cevent.Value = ((double)0).ToString("F");
                return;
            }
            if (cevent.DesiredType != typeof(string)) return;

            cevent.Value = ((double)cevent.Value).ToString("F");//("c");
        }

        static public void CurrencyStringToDouble(object sender, ConvertEventArgs cevent)
        {
        
            // Can only convert to decimal type.
            if (cevent.DesiredType != typeof(double)) return;

            cevent.Value = Double.Parse(cevent.Value.ToString(),
              NumberStyles.Float, null);
        }

        static public void InteiroparaString(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(string)) return;
            cevent.Value = cevent.Value.ToString();
        }
        static public void StringparaInteiro(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(Int16)) return;
            cevent.Value = Convert.ToInt16(cevent.Value.ToString());
        }

        static public void DbNulltoDate(object sender, ConvertEventArgs cevent)
        {
            if (Convert.IsDBNull(cevent.Value))
                cevent.Value = DateTime.Today;//((DateTimePicker)((Binding)sender).Control).MinDate.Date;
        }

        static public void DbNulltoString(object sender, ConvertEventArgs cevent)
        {
            if (Convert.IsDBNull(cevent.Value))
                cevent.Value = "";
        }

    }
   

}
