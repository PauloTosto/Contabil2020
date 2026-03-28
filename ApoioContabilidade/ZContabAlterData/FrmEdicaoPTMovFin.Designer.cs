namespace ApoioContabilidade
{
    partial class FrmEdicaoPTMovFin
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
            this.components = new System.ComponentModel.Container();
            this.pnTop = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDesc2 = new System.Windows.Forms.RadioButton();
            this.rbReduzido = new System.Windows.Forms.RadioButton();
            this.rbNumconta = new System.Windows.Forms.RadioButton();
            this.btnPesquisa = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.pnBottom = new System.Windows.Forms.Panel();
            this.sbMovFin = new System.Windows.Forms.StatusBar();
            this.dgvMovFin = new System.Windows.Forms.DataGridView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.pnTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovFin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.groupBox1);
            this.pnTop.Controls.Add(this.btnPesquisa);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.comboBox1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1586, 76);
            this.pnTop.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDesc2);
            this.groupBox1.Controls.Add(this.rbReduzido);
            this.groupBox1.Controls.Add(this.rbNumconta);
            this.groupBox1.Location = new System.Drawing.Point(536, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(337, 44);
            this.groupBox1.TabIndex = 27;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exibição";
            // 
            // rbDesc2
            // 
            this.rbDesc2.AutoSize = true;
            this.rbDesc2.Checked = true;
            this.rbDesc2.Location = new System.Drawing.Point(26, 17);
            this.rbDesc2.Name = "rbDesc2";
            this.rbDesc2.Size = new System.Drawing.Size(101, 21);
            this.rbDesc2.TabIndex = 2;
            this.rbDesc2.TabStop = true;
            this.rbDesc2.Text = "Mnemonico";
            this.rbDesc2.UseVisualStyleBackColor = true;
            this.rbDesc2.CheckedChanged += new System.EventHandler(this.rbDesc2_CheckedChanged);
            // 
            // rbReduzido
            // 
            this.rbReduzido.AutoSize = true;
            this.rbReduzido.Location = new System.Drawing.Point(246, 17);
            this.rbReduzido.Name = "rbReduzido";
            this.rbReduzido.Size = new System.Drawing.Size(89, 21);
            this.rbReduzido.TabIndex = 1;
            this.rbReduzido.Text = "Reduzido";
            this.rbReduzido.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbReduzido.UseVisualStyleBackColor = true;
            this.rbReduzido.Visible = false;
            // 
            // rbNumconta
            // 
            this.rbNumconta.AutoSize = true;
            this.rbNumconta.Location = new System.Drawing.Point(148, 18);
            this.rbNumconta.Name = "rbNumconta";
            this.rbNumconta.Size = new System.Drawing.Size(93, 21);
            this.rbNumconta.TabIndex = 0;
            this.rbNumconta.Text = "Numconta";
            this.rbNumconta.UseVisualStyleBackColor = true;
            this.rbNumconta.CheckedChanged += new System.EventHandler(this.rbNumconta_CheckedChanged);
            // 
            // btnPesquisa
            // 
            this.btnPesquisa.CausesValidation = false;
            this.btnPesquisa.Enabled = false;
            this.btnPesquisa.Location = new System.Drawing.Point(169, 28);
            this.btnPesquisa.Name = "btnPesquisa";
            this.btnPesquisa.Size = new System.Drawing.Size(121, 27);
            this.btnPesquisa.TabIndex = 26;
            this.btnPesquisa.Text = "Consulta";
            this.btnPesquisa.UseVisualStyleBackColor = true;
            this.btnPesquisa.Click += new System.EventHandler(this.btnPesquisa_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 25;
            this.label1.Text = "Escolha Ano";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(15, 31);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 24;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // pnBottom
            // 
            this.pnBottom.Controls.Add(this.sbMovFin);
            this.pnBottom.Controls.Add(this.dgvMovFin);
            this.pnBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnBottom.Location = new System.Drawing.Point(0, 76);
            this.pnBottom.Name = "pnBottom";
            this.pnBottom.Size = new System.Drawing.Size(1586, 436);
            this.pnBottom.TabIndex = 1;
            // 
            // sbMovFin
            // 
            this.sbMovFin.Dock = System.Windows.Forms.DockStyle.None;
            this.sbMovFin.Location = new System.Drawing.Point(11, 397);
            this.sbMovFin.Margin = new System.Windows.Forms.Padding(4);
            this.sbMovFin.Name = "sbMovFin";
            this.sbMovFin.Size = new System.Drawing.Size(152, 28);
            this.sbMovFin.TabIndex = 10;
            // 
            // dgvMovFin
            // 
            this.dgvMovFin.AllowUserToAddRows = false;
            this.dgvMovFin.AllowUserToDeleteRows = false;
            this.dgvMovFin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMovFin.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvMovFin.Location = new System.Drawing.Point(0, 0);
            this.dgvMovFin.Name = "dgvMovFin";
            this.dgvMovFin.RowHeadersWidth = 51;
            this.dgvMovFin.RowTemplate.Height = 24;
            this.dgvMovFin.Size = new System.Drawing.Size(1586, 390);
            this.dgvMovFin.TabIndex = 0;
            this.dgvMovFin.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvMovFin_CellDoubleClick);
            this.dgvMovFin.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvMovFin_KeyDown);
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // FrmEdicaoMovFin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1586, 512);
            this.Controls.Add(this.pnBottom);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmEdicaoPTMovFin";
            this.Text = "FrmEdicaoPTMovFin";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMovFin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnBottom;
        private System.Windows.Forms.DataGridView dgvMovFin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.StatusBar sbMovFin;
        private System.Windows.Forms.Button btnPesquisa;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDesc2;
        private System.Windows.Forms.RadioButton rbReduzido;
        private System.Windows.Forms.RadioButton rbNumconta;
        private System.Windows.Forms.ErrorProvider errorProvider1;
    }
}