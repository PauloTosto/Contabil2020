
namespace ApoioContabilidade.Almoxarifado.FormsConsulta
{
    partial class FrmConsulta
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
            this.cbMarcarTodos = new System.Windows.Forms.CheckBox();
            this.lbQuadra = new System.Windows.Forms.Label();
            this.lbGleba = new System.Windows.Forms.Label();
            this.lbServico = new System.Windows.Forms.Label();
            this.txGleba = new System.Windows.Forms.TextBox();
            this.txQuadra = new System.Windows.Forms.TextBox();
            this.txServico = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pnGrid = new System.Windows.Forms.Panel();
            this.dgvEscolha = new System.Windows.Forms.DataGridView();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            this.panel2.SuspendLayout();
            this.pnGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEscolha)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.cbMarcarTodos);
            this.pnTop.Controls.Add(this.lbQuadra);
            this.pnTop.Controls.Add(this.lbGleba);
            this.pnTop.Controls.Add(this.lbServico);
            this.pnTop.Controls.Add(this.txGleba);
            this.pnTop.Controls.Add(this.txQuadra);
            this.pnTop.Controls.Add(this.txServico);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1352, 100);
            this.pnTop.TabIndex = 1;
            // 
            // cbMarcarTodos
            // 
            this.cbMarcarTodos.AutoSize = true;
            this.cbMarcarTodos.Location = new System.Drawing.Point(11, 34);
            this.cbMarcarTodos.Name = "cbMarcarTodos";
            this.cbMarcarTodos.Size = new System.Drawing.Size(118, 21);
            this.cbMarcarTodos.TabIndex = 6;
            this.cbMarcarTodos.Text = "&Marcar Todos";
            this.cbMarcarTodos.UseVisualStyleBackColor = true;
            this.cbMarcarTodos.CheckedChanged += new System.EventHandler(this.cbMarcarTodos_CheckedChanged);
            // 
            // lbQuadra
            // 
            this.lbQuadra.AutoSize = true;
            this.lbQuadra.Location = new System.Drawing.Point(874, 15);
            this.lbQuadra.Name = "lbQuadra";
            this.lbQuadra.Size = new System.Drawing.Size(60, 17);
            this.lbQuadra.TabIndex = 5;
            this.lbQuadra.Text = "Quadra:";
            // 
            // lbGleba
            // 
            this.lbGleba.AutoSize = true;
            this.lbGleba.Location = new System.Drawing.Point(648, 14);
            this.lbGleba.Name = "lbGleba";
            this.lbGleba.Size = new System.Drawing.Size(50, 17);
            this.lbGleba.TabIndex = 4;
            this.lbGleba.Text = "Gleba:";
            // 
            // lbServico
            // 
            this.lbServico.AutoSize = true;
            this.lbServico.Location = new System.Drawing.Point(178, 13);
            this.lbServico.Name = "lbServico";
            this.lbServico.Size = new System.Drawing.Size(66, 17);
            this.lbServico.TabIndex = 3;
            this.lbServico.Text = "Serviços:";
            // 
            // txGleba
            // 
            this.txGleba.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txGleba.Location = new System.Drawing.Point(647, 35);
            this.txGleba.Name = "txGleba";
            this.txGleba.Size = new System.Drawing.Size(185, 22);
            this.txGleba.TabIndex = 2;
            this.txGleba.TextChanged += new System.EventHandler(this.txGleba_TextChanged);
            // 
            // txQuadra
            // 
            this.txQuadra.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txQuadra.Location = new System.Drawing.Point(870, 35);
            this.txQuadra.Name = "txQuadra";
            this.txQuadra.Size = new System.Drawing.Size(165, 22);
            this.txQuadra.TabIndex = 1;
            this.txQuadra.TextChanged += new System.EventHandler(this.txGleba_TextChanged);
            // 
            // txServico
            // 
            this.txServico.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txServico.Location = new System.Drawing.Point(175, 34);
            this.txServico.Name = "txServico";
            this.txServico.Size = new System.Drawing.Size(447, 22);
            this.txServico.TabIndex = 0;
            this.txServico.TextChanged += new System.EventHandler(this.txServico_TextChanged);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.pnGrid);
            this.panel2.Controls.Add(this.btnCancel);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 100);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1352, 455);
            this.panel2.TabIndex = 2;
            // 
            // pnGrid
            // 
            this.pnGrid.Controls.Add(this.dgvEscolha);
            this.pnGrid.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnGrid.Location = new System.Drawing.Point(0, 0);
            this.pnGrid.Name = "pnGrid";
            this.pnGrid.Size = new System.Drawing.Size(1352, 398);
            this.pnGrid.TabIndex = 0;
            // 
            // dgvEscolha
            // 
            this.dgvEscolha.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEscolha.Location = new System.Drawing.Point(14, 11);
            this.dgvEscolha.Name = "dgvEscolha";
            this.dgvEscolha.RowHeadersWidth = 51;
            this.dgvEscolha.RowTemplate.Height = 24;
            this.dgvEscolha.Size = new System.Drawing.Size(1321, 376);
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
            // FrmConsulta
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1352, 555);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmConsulta";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FrmEscolha";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.pnGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEscolha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel pnGrid;
        private System.Windows.Forms.DataGridView dgvEscolha;
        private System.Windows.Forms.TextBox txServico;
        private System.Windows.Forms.TextBox txGleba;
        private System.Windows.Forms.TextBox txQuadra;
        private System.Windows.Forms.Label lbQuadra;
        private System.Windows.Forms.Label lbGleba;
        private System.Windows.Forms.Label lbServico;
        private System.Windows.Forms.CheckBox cbMarcarTodos;
    }
}