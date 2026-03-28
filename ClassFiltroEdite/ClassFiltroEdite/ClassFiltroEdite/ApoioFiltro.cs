using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Globalization;
using System.Data;
using System.Linq;
using System.Data.OleDb;

namespace ClassFiltroEdite
{
    public class FormDlgForm : Form
    {
        public int tamanho = 0, altura = 0;
        public int left, top;
        public bool modeloFiltro = false;
        //private BindingManagerBase oBinding;
        public BindingNavigator oNavegador;
        public BindingSource aFonte;
        private Boolean force=false;
        public ErrorProvider errorProvider1;
        private System.ComponentModel.IContainer components;
        public ArmeEdicao oArmeEdicao;

        public FormDlgForm()
        {
            oArmeEdicao = null;
            InitializeComponent();
        }

        public Boolean Force
        {
            
            get { return force; }
            set { force = value; }
        }

        protected override void OnLoad( EventArgs e)
        {
            if (!modeloFiltro)
                this.StartPosition = FormStartPosition.CenterParent;

            if ((tamanho != 0) || (altura != 0))
            {
                if (this.ClientSize.Width != tamanho)
                    this.ClientSize = new Size(tamanho, ClientSize.Height);
                if (this.ClientSize.Height != altura)
                    this.ClientSize = new Size(ClientSize.Width, altura);
            }
            base.OnLoad(e);
        }

       [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
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
                        if (oArmeEdicao != null)
                        {
                            oArmeEdicao.AtiveString(this.ActiveControl);
                        }

                        return true;
                    }

                case Keys.Down:
                    {
                        if (force)
                        {
                            if (this.ActiveControl != null)
                            {
                                if (!(this.ActiveControl is ComboBox))
                                {
                                    this.SelectNextControl(this.ActiveControl, ((keyData == Keys.Right) || (keyData == Keys.Down)), true, true, false);
                                    return true;
                                }
                            }
                        }
                        break;

                    }
                case Keys.Escape:
                    {
                        /*if (!(this.ActiveControl.Focused)) 
                        {
                            this.DialogResult = DialogResult.Cancel;
                            return true;
                        }
                        */

                        try
                        {
                            this.ActiveControl.DataBindings[0].ReadValue();

                        }
                        catch (Exception)
                        {

                        }
                        if (oArmeEdicao != null)
                        {
                            this.DialogResult = DialogResult.Cancel;
                            if (!oArmeEdicao.cancele.Enabled)
                            {
                                oArmeEdicao.cancele.Enabled = true;
                            }
                            oArmeEdicao.cancele.PerformClick();
                        }

                        this.DialogResult = DialogResult.Cancel;
                       // return base.ProcessCmdKey(ref msg, keyData);
                        return true;
                    }
                case Keys.Return:
             //   case Keys.Tab:
                    {

                        if (force)
                        {
                            if (UltimoAtivo(this.ActiveControl))
                            {
                                //SendKeys.SendWait("{ENTER}");
                                this.DialogResult = DialogResult.OK;
                                return true;
                            }
                            else
                            {
                                if ((oNavegador != null) && (oNavegador.BindingSource != null))
                                {
                                    if ((this.ActiveControl is ComboBox) && (((ComboBox)this.ActiveControl).DroppedDown == true))
                                    {
                                        // return base.ProcessCmdKey(ref msg, keyData);
                                        break;
                                    }
                                    else
                                    {
                                        return base.ProcessCmdKey(ref msg, keyData);
                                        // break;
                                    }
  
                                }
                                else if ((this.ActiveControl is UserControl) )
                                {
                                    // return base.ProcessCmdKey(ref msg, keyData);
                                    break;
                                }
                           
                                else
                                {
                                    this.SelectNextControl(this.ActiveControl, (Control.ModifierKeys != Keys.Shift), true, false, false);
                                    return true;
                                }
                            }
                        }
                        else
                        {///serve para a edição padrao
                            if (UltimoAtivo(this.ActiveControl))
                            {
                                //oArmeEdicao.PegAnteriores();
                                /*  if (oNavegador.Items.ContainsKey("Proximo"))
                                      oNavegador.Items["Proximo"].PerformClick();
                                         */
                                return base.ProcessCmdKey(ref msg, keyData);

                                
                            }
                            else
                            {
                                if ((oNavegador != null) && (oNavegador.BindingSource != null))
                                {
                                    if ((this.ActiveControl is ComboBox) && (((ComboBox)this.ActiveControl).DroppedDown == true))
                                    {
                                         return base.ProcessCmdKey(ref msg, keyData);
                                        // break;
                                        // return base.ProcessCmdKey(ref msg, Keys.Tab);
                                    //    SendKeys.Send("{TAB}");
                                        // SendKeys.Send(Keys.Tab.ToString());
                                        //return false;
                                    }
                                    else
                                    {
                                        this.SelectNextControl(this.ActiveControl, true, true, true, false);
                                        return true;
                                    }
                                }
                                else if ((this.ActiveControl is UserControl))
                                {
                                    // return base.ProcessCmdKey(ref msg, keyData);
                                    break;
                                }

                                else
                                {
                                    this.SelectNextControl(this.ActiveControl, (Control.ModifierKeys != Keys.Shift), true, false, false);
                                    return true;
                                }
                            }
                        }
                    }
                case Keys.PageDown:
                    {
                        if ((oNavegador == null) || (oNavegador.BindingSource == null)) break;
                       
                       // oArmeEdicao.PegAnteriores();
                        
                        if (oNavegador.Items.ContainsKey("Proximo"))
                            oNavegador.Items["Proximo"].PerformClick();
                        return true;
                    }
                case Keys.PageUp:
                    {
                        if ((oNavegador == null) || (oNavegador.BindingSource == null)) break;

                    //    oArmeEdicao.PegAnteriores();

                        if (oNavegador.Items.ContainsKey("Anterior"))
                            oNavegador.Items["Anterior"].PerformClick();
                        return true;
                    }
                default:
                    break;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        
        
        //funcão recorrente que percorre os constrols do form
        int PesqControl(Control oControlPai, int index)
        {
            int result = index;
            foreach (Control oControl in oControlPai.Controls)
            {
                if (oControl.Controls.Count > 0)
                {
                    result = PesqControl(oControl, result);
                    continue;
                }
                if (oControl.TabStop)
                    if (oControl.TabIndex > result)
                        result = oControl.TabIndex;
            }
            return result;
        }

        private void FormDlgForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.ActiveControl == null) return;
            if (this.ActiveControl.Focused)
            {

                /*if (this.ActiveControl is ComboBox)
                {

                    if (((ComboBox)this.ActiveControl).DroppedDown == true)
                     {
                       ((ComboBox)this.ActiveControl).DataBindings[0].ReadValue();
                    // base.ProcessCmdKey(ref msg, keyData);

                }*/
                try
                {
                    if ((this.ActiveControl.DataBindings != null) && (this.ActiveControl.DataBindings.Count > 0))
                       this.ActiveControl.DataBindings[0].ReadValue();
                }
                catch (Exception)
                {
                }
                
                if (oArmeEdicao != null)
                {
                  //  if (oArmeEdicao.cancele.Enabled)
                       oArmeEdicao.cancele.PerformClick();
                }

            }
            return;
        }

        Boolean UltimoAtivo(Control curControl)
        {

            int oindex = -1;
            oindex = PesqControl(this, oindex);

            if (oindex == curControl.TabIndex)
                return true;
            else
                return false;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FormDlgForm
            // 
            this.ClientSize = new System.Drawing.Size(611, 264);
            this.Name = "FormDlgForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormDlgForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }
       
        
      
    }

    public class NumericTextBox : TextBox
    {
        bool allowSpace = false;
   //     int numberDecimalDigito = 2;

        // Restricts the entry of characters to digits (including hex), the negative sign,
        // the decimal point, and editing keystrokes (backspace).
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            base.OnKeyPress(e);

            NumberFormatInfo numberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat;
     //       numberFormatInfo.NumberDecimalDigits = numberDecimalDigito;
            string decimalSeparator = numberFormatInfo.NumberDecimalSeparator;
            string groupSeparator = numberFormatInfo.NumberGroupSeparator;
            string negativeSign = numberFormatInfo.NegativeSign;

            string keyInput = e.KeyChar.ToString();

            if (Char.IsDigit(e.KeyChar))
            {
                // Digits are OK
            }
            else if (keyInput.Equals(decimalSeparator) ||   //|| keyInput.Equals(groupSeparator)
             keyInput.Equals(negativeSign))
            {
                // Decimal separator is OK
            }
            else if (e.KeyChar == '\b')
            {
                // Backspace key is OK
            }
            //    else if ((ModifierKeys & (Keys.Control | Keys.Alt)) != 0)
            //    {
            //     // Let the edit control handle control and alt key combinations
            //    }
            else if (this.allowSpace && e.KeyChar == ' ')
            {

            }
            else
            {
                // Consume this invalid key and beep
                // MessageBeep();
                e.Handled = true;

            }
        }

        public int IntValue
        {
            get
            {
                if (this.Text.Trim() == "")
                    return 0;
                else
                   return Int32.Parse(this.Text);
            }
        }

        public decimal DecimalValue
        {
            get
            {
                return Decimal.Parse(this.Text);
            }
        }

        public bool AllowSpace
        {
            set
            {
                this.allowSpace = value;
            }

            get
            {
                return this.allowSpace;
            }
        }
    }


    public class CustomDateTimePicker : DateTimePicker
    {
        int count = 1;
        public CustomDateTimePicker()
        {
            base.Format = DateTimePickerFormat.Short;
          //  base.CustomFormat = "dd/MM/y";
        }
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            string text = this.Text;
           
          /*  if ((e.KeyCode == Keys.Left) || (e.KeyCode == Keys.Back))
            {
                if (count == 2)
                {
                    count = 1;
                    return;
                }

            }
            if (count == 2)
            {
                SendKeys.Send("{RIGHT}");
                count = 0;
            }
            else
            {
                count++;
            }*/
        }
        protected override void OnEnter(EventArgs e)
        {
            base.OnEnter(e);
            count = 1;
        }
    }


        // public delegate Boolean ponteiroPesquisa(LinhaSolucao oLinha, DataRow orow);
        public delegate Boolean ponteiroPesquisa(LinhaSolucao oLinha, DataRow orow,PesquisaGenerico oPesquisa);
    public delegate string ponteiroLinhaSql(LinhaSolucao oLinha);
    public delegate Dictionary<string,string> ponteiroLinhaSqlComplexo(LinhaSolucao oLinha);

    public class ApoioBancos
    {
        private DataTable Banco;
        public ApoioBancos(DataTable oBancos)
        {
            Banco = oBancos;
            string anterior = "";
            for (int i = 0; i < Banco.Rows.Count; i++)
            {
                if (anterior == Convert.ToString(Banco.Rows[i]["DESC2"]).Trim())
                    Banco.Rows[i].Delete();
                else
                    anterior = Convert.ToString(Banco.Rows[i]["DESC2"]).Trim();
            }
            Banco.AcceptChanges();

            DataColumn[] PrimaryKeyColumns = new DataColumn[1];
            PrimaryKeyColumns[0] = Banco.Columns["DESC2"];
            Banco.PrimaryKey = PrimaryKeyColumns;
        }
        public bool ExisteConta(string campo)
        {
            if (campo.Trim() == "") return false;
            DataRow linha = Banco.Rows.Find(campo);
            if (linha == null) return false;
            return true;
        }

    }

    public class PesquisaGenerico
    {
        private DataSet datasetGeral;
        public PesquisaGenerico(DataSet odataset)
        {
            datasetGeral = odataset;
           
        }
        public DataTable PegTabela(string tabela)
        {
            if (ExisteTabela(tabela))
                return datasetGeral.Tables[tabela];
            else
                return null;
        }
        public bool ExisteTabela(string tabela)
        {
            return datasetGeral.Tables.Contains(tabela); 
        }
        public bool ExisteColuna(string tabela, string campo)
        {
            if (tabela.Trim() == "") return false;
            if (campo.Trim() == "") return false;
            if (!ExisteTabela(tabela)) return false;
            return datasetGeral.Tables[tabela].Columns.Contains(campo);
         }
        public bool ExisteValor(string tabela, string campo,string valor)
        {
            if (tabela.Trim() == "") return false;
            if (campo.Trim() == "") return false;
           // if (!ExisteTabela(tabela)) return false;
            string literal = campo + " = '" + valor + "'" ;
            DataRow[] linha = datasetGeral.Tables[tabela].Select(literal);// Banco.Rows.Find(campo);
            if (linha.Length == 0) return false;
            return true;
        }
      
        public DataRow[] PesqLinhas(string tabela, string[] campos,string [] valores)
        {
            //if (!ExisteCampo(tabela, campo)) return null;
            try
            {
                string strlinha = "";
                for (int i = 0;i < campos.GetLength(0);i++)
                {
                    if (i < valores.GetLength(0))
                    {
                        if (strlinha.Length > 0) 
                           strlinha += " AND ";
                      strlinha += strlinha + " "+campos[i] + " = "+"'"+valores[i]+"' ";
                    }
                }
               DataRow[] linhas = datasetGeral.Tables[tabela].Select(strlinha);
               return linhas;
            }
            catch(Exception)
            {return null;}
        }
      
    }


    class Apoio
    {
        public static int ind = 8;
        static public SizeF TamanhoElementoF(Control oControl)
        {
            return oControl.CreateGraphics().MeasureString(oControl.Text.ToString(),oControl.Font); 
        }
        static public Size TamanhoElemento(Control oControl)
        {
            Size oproposto = new Size(oControl.Width, oControl.Width);
            return TextRenderer.MeasureText(oControl.CreateGraphics(), oControl.Text.ToString(), oControl.Font, oproposto, TextFormatFlags.TextBoxControl);
        }

        static public int TamanhoString(string ocampo,Font oFont)
        {
            Size osize = TextRenderer.MeasureText(ocampo,oFont);
            return osize.Width;
        }
    }
    
   
    public class LinhaSolucao  //sintese da informação pegada no TabPage_apoio
    {
        public ponteiroPesquisa ofuncao;
        public ponteiroLinhaSql ofuncaoSql;
        public ponteiroLinhaSqlComplexo ofuncaoSqlDictionary;
        public List<string> dado;
        public string campo;
        public LinhaSolucao(string value)
        {
            campo = value;
            ofuncao = null;
            ofuncaoSql = null;
            ofuncaoSqlDictionary = null;
            dado = new List<string>(Apoio.ind);
            for (int j = 0; j < Apoio.ind; j++)
                dado.Add("");
        }
    }


   
    /// <summary>
    /// Form auxiliar do Pesquise
 

    // cada pagina(TabPage_apoio) do PageControl tem  seu \checkList e os dados (olinha) para cada linha do checklist que fica no List<Linha> Teste 

    public class CheckListPt : CheckedListBox
    {
        //private Pesquise oPesquise;
        public CheckListPt()
        {

            //  oPesquise = (Pesquise)this.Parent.Parent;
            TabIndex = 0;
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkedListBox_MouseDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.checkedListBox_KeyDown);
         //   this.Font = new System.Drawing.Font(this.Parent.Font.FontFamily, this.Parent.Font.Size);
            // oFont.FontFamily. = 10;
        }
        private void checkedListBox_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (((CheckedListBox)sender).SelectedIndex != -1)

                if ((CheckState)(((CheckedListBox)sender).GetItemCheckState(((CheckedListBox)sender).SelectedIndex))!=CheckState.Checked)
                    PegDado();
                else
                    PonhaDefault();
        }
        private void checkedListBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if ((e.KeyValue == 13) || (e.KeyData == Keys.Space))
                if (((CheckedListBox)sender).SelectedIndex != -1)
                {
                    PegDado();
                    e.SuppressKeyPress = true;
                }
        }

        private void PonhaDefault()
        {
            Boolean selecao = false;
            Pesquise oPesquise = (Pesquise)this.Parent.Parent; // descendente de TabControl
            TabPage_Apoio oApoio = (TabPage_Apoio)oPesquise.SelectedTab;//descendente de TabPage_
            Linha oLinha = (Linha)oApoio.Teste[this.SelectedIndex]; // Linha 
            for (int j = 0; j < Apoio.ind; j++)
            {
                if (oLinha.oedite[j] != null)
                {
                    oPesquise.PosicioneControle(oLinha.oedite[j], oLinha.vdado[j]);
                    if (oLinha.vdado[j] != "")
                        selecao = true;
                    else
                        oLinha.oedite[j].Visible = false;
                }
            }
            this.SetItemChecked(this.SelectedIndex, selecao);

        }

        // o pegDado funciona para pegar cada linha por vez.
        public void PegDado()
        {
            if (!(this.Parent.Parent is Pesquise)) return;// colocar o tabpage na criação
            Boolean selecao = false;
            Pesquise oPesquise = (Pesquise)this.Parent.Parent; // descendente de TabControl
            TabPage_Apoio oApoio = (TabPage_Apoio)oPesquise.SelectedTab;//descendente de TabPage_
            Linha oLinha = (Linha)oApoio.Teste[this.SelectedIndex]; // Linha 


            // Coordenadas relativas ao item(linha) no CheckedList
            System.Drawing.Rectangle inicio_linha = this.GetItemRectangle(this.SelectedIndex);
           System.Drawing.Rectangle ApoioRect = oApoio.ClientRectangle;


            int top_check = inicio_linha.Top;
            int left_check = inicio_linha.Left + (inicio_linha.Width) + 8;//==Right?
            // a altura (height) é a mesma
            int altura = inicio_linha.Height;

            // relativo ao form
            System.Drawing.Point pontoini = new System.Drawing.Point();
            pontoini.X = left_check + ApoioRect.Left;
            pontoini.Y = top_check + ApoioRect.Top;
            pontoini = this.PointToScreen(pontoini);
             
            //tamanho maximo do form
            int tamanhomax = (ApoioRect.Width - (this.Right + 3) - oApoio.Margin.Left - oApoio.Margin.Right);

            // relativo aos controles (texto, etc..)
            // acresta o top e left do chelist     
            left_check = left_check+this.Left +3;
            top_check = top_check + this.Top + 3;


            int tamanho = 0;
            

            bool result = false;
            // 
            for (int j = 0; j < Apoio.ind; j++)
            {
                if (oLinha.oedite[j] != null)
                {
                    result = true;
                    if (this.SelectedIndex == oApoio.Teste.Count - 1)//o último reseta o combode funcoes do filtro
                        ((ComboBox)oLinha.oedite[j]).SelectedIndex = 0;
            
                }
            }
           
            if (result == false) return;
            try
            {
                FormDlgForm oform = new FormDlgForm();
                oform.modeloFiltro = true;
                oform.Force = true;
                oform.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
                oform.AutoScroll = false;
                oform.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                oform.MaximizeBox = false;
                oform.Name = "ptForm3";
                oform.StartPosition = FormStartPosition.Manual;
                 oform.SetDesktopLocation(pontoini.X, pontoini.Y );
            

                int col = 0;//MulDiv(0, DialogUnits.X, 4); // posicao esquer da inicial
                int intervalo_col = 0;
                int ttaborder = 0;
                
                foreach (Control ocontrol in oLinha.oedite)
                {
                    if (ocontrol == null) break;
                    ocontrol.Enabled = true;
                    ocontrol.Visible = true;

                    ocontrol.TabIndex = ttaborder;
                 
                    ocontrol.Left = col + intervalo_col;
                    if ((altura) < ocontrol.Height)
                        ocontrol.Height = altura;
                    ocontrol.Top = 0;//MulDiv(0, DialogUnits.Y, 8);
                    ocontrol.Parent = oform;
                    col = col + intervalo_col + ocontrol.Width;
                    intervalo_col = ((int)ocontrol.Font.Size * 2);       // MulDiv(6, DialogUnits.X, 4);
                    ttaborder += 1;
                }

                  
                tamanho = col;
                
                oform.ClientSize = new System.Drawing.Size(tamanho, altura);
                // tive que alterar o form porque o comportamento padrao modifica a altura e tamanho
                oform.altura = altura + 3;
                oform.tamanho = tamanho;
                oform.BackColor = Color.AliceBlue;
                
                if (oform.ShowDialog() == DialogResult.OK)
                    result = true;
                else
                    result = false;
                

            }
            finally
            {
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oLinha.oedite[j] != null)
                    {
                        oLinha.oedite[j].Parent = oApoio;//this.Parent;
                        oLinha.oedite[j].Visible = false;
                    }
                }
            }
            if (result == true)
            {
                // Quando acionada a Funcao de Limpeza dos Filtros
                if (this.SelectedIndex == oApoio.Teste.Count - 1)//o último
                {
                    if (((ComboBox)oLinha.oedite[0]).SelectedIndex == 0)
                    {
                        for (int i = 0; i < oApoio.Teste.Count - 1; i++)
                        {
                            this.SelectedIndex = i;
                            PonhaDefault();//coloca valores originais
                            for (int j = 0; j < Apoio.ind; j++)
                            {
                                if (oLinha.oedite[j] != null)
                                {
                                    oLinha.oedite[j].Enabled = false;
                                    if (this.GetItemChecked(this.SelectedIndex))
                                        oLinha.oedite[j].Visible = true;
                                    else
                                        oLinha.oedite[j].Visible = false;
                                    this.Refresh();
                                }
                            }
                        }
                    }
                }
                else
                {
                    for (int j = 0; j < Apoio.ind; j++)
                    {
                        if (oLinha.oedite[j] != null)
                        {
                            if (oLinha.dado[j] != oPesquise.TextoResposta(oLinha.oedite[j]))
                            {
                                oApoio.modificado = true;
                                oLinha.dado[j] = oPesquise.TextoResposta(oLinha.oedite[j]);
                            }
                            oPesquise.PosicioneControle(oLinha.oedite[j], oLinha.dado[j]);
                            if (oLinha.dado[j] != "") selecao = true;
                        }
                    }
                }
            }
            else
            {
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oLinha.oedite[j] != null)
                    {
                        oApoio.modificado = false;
                        oPesquise.PosicioneControle(oLinha.oedite[j], oLinha.dado[j]);
                        if (oLinha.dado[j] != "") selecao = true;
                    }
                }
            }
            this.SetItemChecked(this.SelectedIndex, selecao);
            if (selecao)
            {
                int col = left_check; // MulDiv(0, DialogUnits.X, 4);
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oLinha.oedite[j] != null)
                    {
                        oLinha.oedite[j].Enabled = false;
                        oLinha.oedite[j].Visible = true;
                        oLinha.oedite[j].Left = col;
                        oLinha.oedite[j].Top = top_check;  //  
                        col = col + oLinha.oedite[j].Width + ((int)oLinha.oedite[j].Font.Size * 2); //MulDiv(2, DialogUnits.X, 4);
                        this.Show();
                    }
                }
            }
        }

    }


    public class TabPage_Apoio : TabPage
    {
        public CheckListPt oList;
        public List<Linha> Teste;
        public bool modificado;
        //ArmeEdicao oArme;
        public TabPage_Apoio()
        {
            Teste = new List<Linha>();
            oList = new CheckListPt();
            oList.Parent = this;
            // Font font = new Font(FontFamily.GenericSansSerif, 12);
            
           // oList.Font = font;
            oList.Font = new System.Drawing.Font(this.Font.FontFamily,11);
            modificado = false;
            this.UseVisualStyleBackColor = false;
            this.HScroll = true;
        }
        public string PageName()
        {
            string result = this.Text;
            return result;

        }
        public List<LinhaSolucao> Get_LinhaSolucao()
        {
            List<LinhaSolucao> result = new List<LinhaSolucao>();
            for (int i = 0; i < oList.CheckedIndices.Count; i++)
            {
                result.Add(new LinhaSolucao(Teste[oList.CheckedIndices[i]].titulo));
                result[result.Count - 1].ofuncao = Teste[oList.CheckedIndices[i]].ofuncao;
                result[result.Count - 1].ofuncaoSql = Teste[oList.CheckedIndices[i]].ofuncaoSql;
                result[result.Count - 1].ofuncaoSqlDictionary = Teste[oList.CheckedIndices[i]].ofuncaoSqlDictionary;
                for (int j = 0; j < Teste[oList.CheckedIndices[i]].dado.Count; j++)
                  {
                      result[result.Count - 1].dado[j] = Teste[oList.CheckedIndices[i]].dado[j];
                      
                  }
               
            }
            return result;
        }
    }

    public class Pesquise : TabControl
    {
        public List<Linha> Linhas;

        public Pesquise()//construtor
        {
            Linhas = new List<Linha>();
            this.TabIndexChanged += new System.EventHandler(this.tabControl_TabIndexChanged);
            this.TabPages.Clear();
            this.Dock = DockStyle.None;
            this.DrawMode = TabDrawMode.Normal;
            this.Appearance = TabAppearance.Normal;
            this.Visible = false;
        }

        public void NovaPagina()
        {
            
            TabPage_Apoio oTab = new TabPage_Apoio();
            TabPages.Add(oTab);
            this.SelectedTab = TabPages[TabPages.Count - 1];
            CrieCheckList();
            this.Visible = true;
        }
        public void NovaPagina(string nome)
        {
            NovaPagina();
            this.SelectedTab.Name = nome;
        }
        public TabPage_Apoio Pagina(string nome)
        {
            if (this.TabPages.ContainsKey(nome))
                return (TabPage_Apoio)this.TabPages[nome];
            else
                return null;
        }
        public TabPage_Apoio Pagina(int numero)
        {
            if (this.TabPages.Count > numero)
                return (TabPage_Apoio)this.TabPages[numero];
            else
                return null;
        }
        public void LimpaPaginas()
        {
            this.TabPages.Clear();
            Linhas.Clear();
            this.Visible = false;
        }
      //  public 



        private void tabControl_TabIndexChanged(object sender, EventArgs e)
        {
            TabPage_Apoio oPage = ((TabPage_Apoio)((TabControl)(sender)).SelectedTab);
            if (oPage == null) return;

            for (int i = 0; i < oPage.Teste.Count; i++)
            {
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oPage.Teste[i].oedite[j] != null)
                    {
                        (oPage.Teste[i].oedite[j]).Parent = oPage;
                        PosicioneControle(oPage.Teste[i].oedite[j], oPage.Teste[i].vdado[j]);
                    }
                }
            }
        }

        public void PosicioneControle(Control obj, string campo)
        {
            if (obj is System.Windows.Forms.TextBoxBase)
            {
                ((System.Windows.Forms.TextBoxBase)obj).Text = campo;
                return;
            }
            if (obj is System.Windows.Forms.ComboBox)
            {
                if (((System.Windows.Forms.ComboBox)obj).ValueMember == "") // QUANDO O VALUEMEMBER É IGUAL ao DISPLAYMEMBER
                    ((System.Windows.Forms.ComboBox)obj).SelectedIndex = ((System.Windows.Forms.ComboBox)obj).Items.IndexOf(campo);
                return;
            }
            if (obj is System.Windows.Forms.ListBox)
            {
                ((System.Windows.Forms.ListBox)obj).SelectedIndex = ((System.Windows.Forms.ListBox)obj).Items.IndexOf(campo);
                return;
            }
        }
        public string TextoResposta(Control obj)
        {
            if (obj is System.Windows.Forms.TextBoxBase)
            {
                return ((System.Windows.Forms.TextBoxBase)obj).Text;
            }
            else
                if (obj is System.Windows.Forms.ComboBox)
                {
                if (((System.Windows.Forms.ComboBox)obj).SelectedIndex > -1)
                {
                    string resposta = "";
                    try
                    {
                        if (((System.Windows.Forms.ComboBox)obj).ValueMember != "") // QUANDO O VALUEMEMBER É DIFERENTE DO DISPLAYMEMBER
                        {
                           
                            resposta = Convert.ToString(((System.Windows.Forms.ComboBox)obj).SelectedValue); 
                        }
                        else
                        {
                            resposta = (string)((System.Windows.Forms.ComboBox)obj).Items[((System.Windows.Forms.ComboBox)obj).SelectedIndex];
                        }
                    }
                    catch (Exception)
                    {
                    }
                    return resposta;
                }
                else
                    return "";
                }
                else
                    if (obj is System.Windows.Forms.ListBox)
                    {
                        if (((System.Windows.Forms.ListBox)obj).SelectedIndex > -1)
                            return (string)((System.Windows.Forms.ListBox)obj).Items[((System.Windows.Forms.ListBox)obj).SelectedIndex];
                        else
                            return "";
                    }
                    else
                        return "";
        }

        void CrieCheckList()
        {
            TabPage_Apoio oPage = (TabPage_Apoio)this.SelectedTab;
            oPage.modificado = false;
            oPage.Teste.Clear();
            oPage.oList.Items.Clear();
            oPage.oList.Parent = oPage;
            oPage.oList.Left = 8;
            oPage.oList.Top = 12;
            oPage.oList.TabIndex = 0;
          
            // Passa Todas as Informações da Linha para o Teste
            for (int i = 0; i < Linhas.Count; i++)
            {
                oPage.Teste.Add(Linhas[i]);
                for (int j = 0; j < Apoio.ind; j++)
                {
                    oPage.Teste[i].cabecalho[0] = Linhas[i].cabecalho[0];
                    oPage.Teste[i].oedite[j] = Linhas[i].oedite[j];
                    oPage.Teste[i].dado[j] = Linhas[i].dado[j];
                    oPage.Teste[i].vdado[j] = Linhas[i].vdado[j];
                    oPage.Teste[i].ofuncao = Linhas[i].ofuncao;
                }
            }
            Linha olinha = new Linha("");
            olinha.oedite[0] = new ComboBox();
            olinha.oedite[0].Parent = oPage;
            ((ComboBox)olinha.oedite[0]).Items.Add("Limpa Filtro");
           // ((ComboBox)olinha.oedite[0]).Items.Add("Retorne     ");
            ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDownList;
            ((ComboBox)olinha.oedite[0]).SelectionStart = 0;
            ((ComboBox)olinha.oedite[0]).Hide();
            olinha.dado[0] = "";
            olinha.vdado[0] = "";
            olinha.cabecalho[0] = "Funcoes Filtro:";
            oPage.Teste.Add(olinha);
            int tamlinha = 0;
            int Altura = 0;
            foreach (Linha olin in oPage.Teste)
            {
                if (Apoio.TamanhoString(olin.cabecalho[0],oPage.oList.Font) > tamlinha)
                {
                    tamlinha = Apoio.TamanhoString(olin.cabecalho[0], oPage.oList.Font);
                }
                oPage.oList.Items.Add(olin.cabecalho[0]);
                if (olin.oedite[0] != null)
                    Altura += olin.oedite[0].Height;
                foreach(Control ocontrol in olin.oedite)
                {
                    if (ocontrol == null) break;
                    ocontrol.Visible = false;
                }
                /*for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oPage.Teste[i].oedite[j] != null)
                    {
                        oPage.Teste[i].oedite[j].Visible = false;
                       // if (j == 0)
                           
                    }
                }*/
            }
           oPage.oList.Width = (tamlinha + 8) +  25; // verificar se está correto...
            if (oPage.oList.Items.Count > 0)
            {
                oPage.oList.Height = Altura + 5; //Altura;// (oPage.oList.Items.Count * oPage.oList.PreferredHeight);
                for (int i = 0; i < oPage.Teste.Count; i++)
                {
                    for (int j = 0; j < Apoio.ind; j++)
                    {
                        if (oPage.Teste[i].dado[j].Trim() != "")
                            oPage.oList.CheckedItems[j] = true;
                    }
                }
            }
            
            // dimensao das áreas editávies (necessário para ajuste)
            int tamanhomaior = 0;
            for (int i = 0; i < oPage.Teste.Count; i++)
            {
           
                Linha oLinha = oPage.Teste[i];
                int col = 0;
                int intervalo_col = 0;
                for (int j = 0; j < Apoio.ind; j++)
                {
                    if (oLinha.oedite[j] != null)
                    {
                        col = col + intervalo_col + oLinha.oedite[j].Width;
                        intervalo_col = ((int)oLinha.oedite[j].Font.Size * 2);       
                    }
                }
                if (tamanhomaior < col) tamanhomaior = col;
            }
            // Ajuste Altura
            // Size previo da area do TabPage necessária a edição
            Size oSize = new Size(0, 0);
            oSize.Width =  oPage.oList.Left + oPage.oList.Width + tamanhomaior + 8 +8;
         
            Form oForm = this.FindForm();

            if (oSize.Width > ClientSize.Width)
            {
                oForm.ClientSize = new Size(oSize.Width,oForm.ClientSize.Height) ;
                oPage.Parent.ClientSize = new Size(oForm.ClientSize.Width, oPage.Parent.Height);
                oPage.ClientSize = new Size(oPage.Parent.ClientSize.Width,oPage.Height);
            }
                  





            TransfValores(0); //1- pega do geral para  o tabsheet,
        }

        void TransfValores(int direcao)
        {
            TabPage_Apoio oPage = (TabPage_Apoio)this.SelectedTab;
            if (oPage.oList.Items.Count > 0)
            {
                for (int i = 0; i < oPage.Teste.Count; i++)
                {
                    for (int j = 0; j < Apoio.ind; j++)
                    {
                        if (oPage.Teste[i].oedite[j] != null)
                        {
                            if (direcao > 0)
                                oPage.Teste[i].dado[j] = TextoResposta(oPage.Teste[i].oedite[j]);
                            else
                                PosicioneControle(oPage.Teste[i].oedite[j], oPage.Teste[i].dado[j]);
                        }
                    }
                }
            }
        }
    }
  
    /// Form auxiliar do Pesquise
    /// </summary>

    /// </summary>

    
}

    