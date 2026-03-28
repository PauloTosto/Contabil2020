namespace ApoioContabilidade.Contabilidade
{
    partial class FrmTransfereContabilidade
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
            this.lbPeriodo = new System.Windows.Forms.Label();
            this.lbMeses = new System.Windows.Forms.Label();
            this.nMeses = new System.Windows.Forms.NumericUpDown();
            this.btnImprime = new System.Windows.Forms.Button();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.btnTransfere = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSaidasAlmox = new System.Windows.Forms.RadioButton();
            this.rbProvFolha = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tcMeses = new System.Windows.Forms.TabControl();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nMeses)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.lbPeriodo);
            this.pnTop.Controls.Add(this.lbMeses);
            this.pnTop.Controls.Add(this.nMeses);
            this.pnTop.Controls.Add(this.btnImprime);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.btnTransfere);
            this.pnTop.Controls.Add(this.groupBox1);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.dtData1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1480, 100);
            this.pnTop.TabIndex = 0;
            // 
            // lbPeriodo
            // 
            this.lbPeriodo.AutoSize = true;
            this.lbPeriodo.Location = new System.Drawing.Point(21, 66);
            this.lbPeriodo.Name = "lbPeriodo";
            this.lbPeriodo.Size = new System.Drawing.Size(0, 16);
            this.lbPeriodo.TabIndex = 15;
            // 
            // lbMeses
            // 
            this.lbMeses.AutoSize = true;
            this.lbMeses.Location = new System.Drawing.Point(121, 11);
            this.lbMeses.Name = "lbMeses";
            this.lbMeses.Size = new System.Drawing.Size(55, 16);
            this.lbMeses.TabIndex = 14;
            this.lbMeses.Text = "Meses+";
            this.lbMeses.Visible = false;
            // 
            // nMeses
            // 
            this.nMeses.Location = new System.Drawing.Point(132, 33);
            this.nMeses.Maximum = new decimal(new int[] {
            11,
            0,
            0,
            0});
            this.nMeses.Name = "nMeses";
            this.nMeses.Size = new System.Drawing.Size(42, 22);
            this.nMeses.TabIndex = 13;
            this.nMeses.Visible = false;
            this.nMeses.ValueChanged += new System.EventHandler(this.nMeses_ValueChanged);
            // 
            // btnImprime
            // 
            this.btnImprime.Location = new System.Drawing.Point(521, 53);
            this.btnImprime.Name = "btnImprime";
            this.btnImprime.Size = new System.Drawing.Size(137, 25);
            this.btnImprime.TabIndex = 12;
            this.btnImprime.Text = "&Imprime";
            this.btnImprime.UseVisualStyleBackColor = true;
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(520, 29);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(137, 23);
            this.btnConsulta.TabIndex = 11;
            this.btnConsulta.Text = "&Consulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // btnTransfere
            // 
            this.btnTransfere.Location = new System.Drawing.Point(521, 5);
            this.btnTransfere.Name = "btnTransfere";
            this.btnTransfere.Size = new System.Drawing.Size(137, 23);
            this.btnTransfere.TabIndex = 10;
            this.btnTransfere.Text = "Transfere Dados";
            this.btnTransfere.UseVisualStyleBackColor = true;
            this.btnTransfere.Click += new System.EventHandler(this.btnTransfere_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSaidasAlmox);
            this.groupBox1.Controls.Add(this.rbProvFolha);
            this.groupBox1.Location = new System.Drawing.Point(213, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 77);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transferencias p/Contabilidade";
            // 
            // rbSaidasAlmox
            // 
            this.rbSaidasAlmox.AutoSize = true;
            this.rbSaidasAlmox.Location = new System.Drawing.Point(12, 49);
            this.rbSaidasAlmox.Name = "rbSaidasAlmox";
            this.rbSaidasAlmox.Size = new System.Drawing.Size(249, 20);
            this.rbSaidasAlmox.TabIndex = 1;
            this.rbSaidasAlmox.Text = "Saidas de Materiais do Almoxarifado";
            this.rbSaidasAlmox.UseVisualStyleBackColor = true;
            this.rbSaidasAlmox.CheckedChanged += new System.EventHandler(this.rbSaidasAlmox_CheckedChanged);
            // 
            // rbProvFolha
            // 
            this.rbProvFolha.AutoSize = true;
            this.rbProvFolha.Checked = true;
            this.rbProvFolha.Location = new System.Drawing.Point(13, 21);
            this.rbProvFolha.Name = "rbProvFolha";
            this.rbProvFolha.Size = new System.Drawing.Size(145, 20);
            this.rbProvFolha.TabIndex = 0;
            this.rbProvFolha.TabStop = true;
            this.rbProvFolha.Text = "Provisões de Folha";
            this.rbProvFolha.UseVisualStyleBackColor = true;
            this.rbProvFolha.CheckedChanged += new System.EventHandler(this.rbProvFolha_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 16);
            this.label1.TabIndex = 8;
            this.label1.Text = "Mes/Ano:";
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtData1.Location = new System.Drawing.Point(13, 33);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(88, 22);
            this.dtData1.TabIndex = 7;
            this.dtData1.ValueChanged += new System.EventHandler(this.dtData1_ValueChanged);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tcMeses);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1480, 497);
            this.panel1.TabIndex = 2;
            // 
            // tcMeses
            // 
            this.tcMeses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMeses.Location = new System.Drawing.Point(0, 0);
            this.tcMeses.Multiline = true;
            this.tcMeses.Name = "tcMeses";
            this.tcMeses.SelectedIndex = 0;
            this.tcMeses.Size = new System.Drawing.Size(1478, 495);
            this.tcMeses.TabIndex = 2;
            // 
            // FrmTransfereContabilidade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1480, 597);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmTransfereContabilidade";
            this.Text = "FrmTransfereContabilidade";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.nMeses)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.Button btnImprime;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.Button btnTransfere;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSaidasAlmox;
        private System.Windows.Forms.RadioButton rbProvFolha;
        private System.Windows.Forms.NumericUpDown nMeses;
        private System.Windows.Forms.Label lbMeses;
        private System.Windows.Forms.Label lbPeriodo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tcMeses;
    }
}