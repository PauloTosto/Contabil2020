
namespace ApoioContabilidade.Trabalho.PegDadosCltCad
{
    partial class FrmEscolhaTrab
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnTop = new System.Windows.Forms.Panel();
            this.ckListSetores = new System.Windows.Forms.CheckedListBox();
            this.gbCategoria = new System.Windows.Forms.GroupBox();
            this.rbDiaristas = new System.Windows.Forms.RadioButton();
            this.rbMensalistas = new System.Windows.Forms.RadioButton();
            this.rbTodos = new System.Windows.Forms.RadioButton();
            this.cbMarcarTodos = new System.Windows.Forms.CheckBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnGrid = new System.Windows.Forms.Panel();
            this.dgvEscolha = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            this.gbCategoria.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEscolha)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.ckListSetores);
            this.pnTop.Controls.Add(this.gbCategoria);
            this.pnTop.Controls.Add(this.cbMarcarTodos);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(939, 100);
            this.pnTop.TabIndex = 1;
            // 
            // ckListSetores
            // 
            this.ckListSetores.CheckOnClick = true;
            this.ckListSetores.FormattingEnabled = true;
            this.ckListSetores.Location = new System.Drawing.Point(576, 15);
            this.ckListSetores.MultiColumn = true;
            this.ckListSetores.Name = "ckListSetores";
            this.ckListSetores.Size = new System.Drawing.Size(293, 55);
            this.ckListSetores.Sorted = true;
            this.ckListSetores.TabIndex = 3;
            this.ckListSetores.ThreeDCheckBoxes = true;
            this.ckListSetores.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.ckListSetores_ItemCheck);
            // 
            // gbCategoria
            // 
            this.gbCategoria.Controls.Add(this.rbDiaristas);
            this.gbCategoria.Controls.Add(this.rbMensalistas);
            this.gbCategoria.Controls.Add(this.rbTodos);
            this.gbCategoria.Location = new System.Drawing.Point(181, 12);
            this.gbCategoria.Name = "gbCategoria";
            this.gbCategoria.Size = new System.Drawing.Size(358, 48);
            this.gbCategoria.TabIndex = 2;
            this.gbCategoria.TabStop = false;
            this.gbCategoria.Text = "Cate&goria";
            // 
            // rbDiaristas
            // 
            this.rbDiaristas.AutoSize = true;
            this.rbDiaristas.Location = new System.Drawing.Point(233, 18);
            this.rbDiaristas.Name = "rbDiaristas";
            this.rbDiaristas.Size = new System.Drawing.Size(84, 21);
            this.rbDiaristas.TabIndex = 2;
            this.rbDiaristas.Text = "Diaristas";
            this.rbDiaristas.UseVisualStyleBackColor = true;
            this.rbDiaristas.CheckedChanged += new System.EventHandler(this.rbDiaristas_CheckedChanged);
            // 
            // rbMensalistas
            // 
            this.rbMensalistas.AutoSize = true;
            this.rbMensalistas.Location = new System.Drawing.Point(105, 18);
            this.rbMensalistas.Name = "rbMensalistas";
            this.rbMensalistas.Size = new System.Drawing.Size(103, 21);
            this.rbMensalistas.TabIndex = 1;
            this.rbMensalistas.Text = "Mensalistas";
            this.rbMensalistas.UseVisualStyleBackColor = true;
            this.rbMensalistas.CheckedChanged += new System.EventHandler(this.rbMensalistas_CheckedChanged);
            // 
            // rbTodos
            // 
            this.rbTodos.AutoSize = true;
            this.rbTodos.Checked = true;
            this.rbTodos.Location = new System.Drawing.Point(6, 21);
            this.rbTodos.Name = "rbTodos";
            this.rbTodos.Size = new System.Drawing.Size(69, 21);
            this.rbTodos.TabIndex = 0;
            this.rbTodos.TabStop = true;
            this.rbTodos.Text = "Todos";
            this.rbTodos.UseVisualStyleBackColor = true;
            this.rbTodos.CheckedChanged += new System.EventHandler(this.rbTodos_CheckedChanged);
            // 
            // cbMarcarTodos
            // 
            this.cbMarcarTodos.AutoSize = true;
            this.cbMarcarTodos.Location = new System.Drawing.Point(18, 26);
            this.cbMarcarTodos.Name = "cbMarcarTodos";
            this.cbMarcarTodos.Size = new System.Drawing.Size(118, 21);
            this.cbMarcarTodos.TabIndex = 1;
            this.cbMarcarTodos.Text = "&Marcar Todos";
            this.cbMarcarTodos.UseVisualStyleBackColor = true;
            this.cbMarcarTodos.CheckedChanged += new System.EventHandler(this.cbMarcarTodos_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnGrid);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(939, 455);
            this.panel2.TabIndex = 2;
            // 
            // pnGrid
            // 
            this.pnGrid.Controls.Add(this.dgvEscolha);
            this.pnGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnGrid.Location = new System.Drawing.Point(0, 0);
            this.pnGrid.Name = "pnGrid";
            this.pnGrid.Size = new System.Drawing.Size(939, 398);
            this.pnGrid.TabIndex = 0;
            // 
            // dgvEscolha
            // 
            this.dgvEscolha.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEscolha.Location = new System.Drawing.Point(14, 11);
            this.dgvEscolha.Name = "dgvEscolha";
            this.dgvEscolha.RowHeadersWidth = 51;
            this.dgvEscolha.RowTemplate.Height = 24;
            this.dgvEscolha.Size = new System.Drawing.Size(912, 376);
            this.dgvEscolha.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(542, 409);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 37);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "&Cancele";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(454, 409);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 37);
            this.button1.TabIndex = 4;
            this.button1.Text = "&Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // FrmEscolhaTrab
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(939, 555);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmEscolhaTrab";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmEscolhaTrab";
            this.Load += new System.EventHandler(this.FrmEscolhaTrab_Load);
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.gbCategoria.ResumeLayout(false);
            this.gbCategoria.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.pnGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEscolha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.GroupBox gbCategoria;
        private System.Windows.Forms.RadioButton rbDiaristas;
        private System.Windows.Forms.RadioButton rbMensalistas;
        private System.Windows.Forms.RadioButton rbTodos;
        private System.Windows.Forms.CheckBox cbMarcarTodos;
        private System.Windows.Forms.CheckedListBox ckListSetores;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel pnGrid;
        private System.Windows.Forms.DataGridView dgvEscolha;
    }
}