using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.FerramentasEditar
{

    public class BoxEditarForm : Form
    {

        private System.ComponentModel.IContainer components = null;

        public TextBox txOrigem, txDestino;
        public ComboBox Destino;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;

        //public CancelEventHandler cancelEventHandler;

        private bool combobox = true; 
        private System.Windows.Forms.ErrorProvider errorProvider1;

        private List<string> camposvalidar = new List<string>();

        public BoxEditarForm(List<string> campos)
        {
            camposvalidar = campos;
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            AutoScroll = false;
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;  // Nones
            MinimizeBox = false;
            MaximizeBox = false;
            combobox = (campos.Count > 0);

         
            InitializeComponent();
            this.tableLayoutPanel1.Controls.Add(this.txDestino, 1, 0);
            this.txDestino.Visible = !combobox;
            this.Destino.Visible = combobox;
            txOrigem.TabStop = true;
            txDestino.TabStop = true;
            txDestino.CharacterCasing = CharacterCasing.Upper;
            if (combobox)
            {
                Destino.Items.Clear();
                Destino.DataSource = campos;
                Destino.SelectedIndex = -1;
            }
           
        }


        private void BoxEditar_KeyDown(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                this.DialogResult = DialogResult.OK;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }

        private void SoEsc_KeyDown(object sender, KeyEventArgs e)
        {
          if (e.KeyCode == Keys.Escape)
            {
                this.DialogResult = DialogResult.Cancel;
            }
        }


        public string ValorRetornado()
        {
            string result = "";
            if (combobox) result = Destino.Text;
            else result = txDestino.Text;
            return result;
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txOrigem = new System.Windows.Forms.TextBox();
            this.Destino = new System.Windows.Forms.ComboBox();
            this.txDestino = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txOrigem
            // 
            this.txOrigem.AcceptsTab = true;
            this.txOrigem.Location = new System.Drawing.Point(3, 3);
            this.txOrigem.Name = "txOrigem";
            this.txOrigem.ReadOnly = true;
            this.txOrigem.Size = new System.Drawing.Size(380, 22);
            this.txOrigem.TabIndex = 0;
            // 
            // Destino
            // 
            this.Destino.DropDownWidth = 150;
            this.Destino.Location = new System.Drawing.Point(403, 3);
            this.Destino.MaxDropDownItems = 4;
            this.Destino.Name = "Destino";
            this.Destino.Size = new System.Drawing.Size(380, 24);
            this.Destino.Sorted = true;
            this.Destino.TabIndex = 1;
            this.Destino.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cmb_KeyPress);
            // 
            // txDestino
            // 
            this.txDestino.AcceptsTab = true;
            this.txDestino.Location = new System.Drawing.Point(403, 3);
            this.txDestino.Name = "txDestino";
            this.txDestino.Size = new System.Drawing.Size(380, 22);
            this.txDestino.TabIndex = 1;
            this.txDestino.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.txOrigem, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Destino, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 26);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // BoxEditarForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 26);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BoxEditarForm";
            this.Load += new System.EventHandler(this.BoxEditarForm_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }


        private void BoxEditarForm_Load(object sender, EventArgs e)
        {
            this.txOrigem.KeyDown += SoEsc_KeyDown;
            if (!combobox)
            {
                this.txDestino.KeyDown += BoxEditar_KeyDown;
                this.txDestino.Validating += CampoValidando;
                this.txDestino.Validated += CampoValidado;
            }
            else
            {
                this.Destino.KeyDown += BoxEditar_KeyDown;
                this.Destino.Validating += CampoValidando;
                this.Destino.Validated += CampoValidado;
            }
            this.Destino.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
        }
        void CampoValidado(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            this.errorProvider1.SetError((Control)sender, "");
        }
        void CampoValidando(object sender, CancelEventArgs e)
        {
            string texto = ((Control)sender).Text.Trim();
            if (texto == "") return;
            // estou aceitando campo em branco
            if (camposvalidar.Count == 0) return;
            if (camposvalidar.Contains(texto.Trim())) return;
            e.Cancel = true;
            this.errorProvider1.SetError(((Control)sender), "Campo Invalido! Redigite");
        }

        private void cmb_KeyPress(object sender, KeyPressEventArgs e)
        {
            //habilitar a abertura automatica da combo.
            (sender as ComboBox).DroppedDown = true;
            
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

    }

    public class EditarColunaMulti
    {
        // public List<string> Lines { get; set; }
        //   public int Select { get; set; }
        //    public string Title { get; set; }
       // private DataGridViewCell ocell;
        private string valorOriginal = "";
        private string nomedoCampo = "";
        private string novoValor = "";
        public List<string> lstCamposValidos = new List<string>();
        public EditarColunaMulti(string _valor,string _nome)
       {
            valorOriginal = _valor;
            nomedoCampo = _nome;
       }
        public string SelItem()
        {
           // string result = "";
         //   if ((Select > -1) && (Select < Lines.Count))
              //  result = Lines[Select];
            return novoValor;
        }


        public bool Execute()
        {
            bool result = false;
            // if (Lines.Count == 0)
            // MessageBox.Show("Nenhum Item na Lista");
            BoxEditarForm boxEditarForm = new BoxEditarForm(lstCamposValidos);
            try
            {
             
                boxEditarForm.Text = "Origem:"+nomedoCampo;
                boxEditarForm.txOrigem.Text = valorOriginal;
                boxEditarForm.txDestino.Text = "";
                boxEditarForm.Destino.Text = "";
                
                if (boxEditarForm.ShowDialog() == DialogResult.OK)
                {
                    novoValor = boxEditarForm.ValorRetornado();
                    if (novoValor.Trim() != "")
                        result = true;
                   // Select = listform.listBox.SelectedIndex;
                }
            }
            catch { }
            return result;
        }
    }
}
