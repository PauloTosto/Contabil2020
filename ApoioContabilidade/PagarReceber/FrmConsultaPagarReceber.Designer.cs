using ClassFiltroEdite;

namespace ApoioContabilidade.PagarReceber
{
    partial class FrmConsultaPagarReceber
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtData2 = new System.Windows.Forms.DateTimePicker();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.btnSair = new System.Windows.Forms.Button();
            this.gbPagarReceber = new System.Windows.Forms.GroupBox();
            this.rbReceber = new System.Windows.Forms.RadioButton();
            this.rbPagar = new System.Windows.Forms.RadioButton();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.btnFiltro = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbPagarReceber.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.groupBox1);
            this.pnTop.Controls.Add(this.btnSair);
            this.pnTop.Controls.Add(this.gbPagarReceber);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.btnFiltro);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(627, 191);
            this.pnTop.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtData2);
            this.groupBox1.Controls.Add(this.dtData1);
            this.groupBox1.Location = new System.Drawing.Point(31, 23);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 66);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Perio&do";
            // 
            // dtData2
            // 
            this.dtData2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData2.Location = new System.Drawing.Point(162, 26);
            this.dtData2.Margin = new System.Windows.Forms.Padding(4);
            this.dtData2.Name = "dtData2";
            this.dtData2.Size = new System.Drawing.Size(123, 22);
            this.dtData2.TabIndex = 1;
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(30, 26);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(123, 22);
            this.dtData1.TabIndex = 0;
            // 
            // btnSair
            // 
            this.btnSair.Location = new System.Drawing.Point(416, 12);
            this.btnSair.Margin = new System.Windows.Forms.Padding(4);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(127, 28);
            this.btnSair.TabIndex = 3;
            this.btnSair.Text = "&Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // gbPagarReceber
            // 
            this.gbPagarReceber.Controls.Add(this.rbReceber);
            this.gbPagarReceber.Controls.Add(this.rbPagar);
            this.gbPagarReceber.Location = new System.Drawing.Point(29, 99);
            this.gbPagarReceber.Name = "gbPagarReceber";
            this.gbPagarReceber.Size = new System.Drawing.Size(272, 71);
            this.gbPagarReceber.TabIndex = 4;
            this.gbPagarReceber.TabStop = false;
            this.gbPagarReceber.Text = "&Contas";
            // 
            // rbReceber
            // 
            this.rbReceber.AutoSize = true;
            this.rbReceber.Location = new System.Drawing.Point(147, 30);
            this.rbReceber.Name = "rbReceber";
            this.rbReceber.Size = new System.Drawing.Size(83, 21);
            this.rbReceber.TabIndex = 1;
            this.rbReceber.Text = "&Receber";
            this.rbReceber.UseVisualStyleBackColor = true;
            this.rbReceber.CheckedChanged += new System.EventHandler(this.rbReceber_CheckedChanged);
            // 
            // rbPagar
            // 
            this.rbPagar.AutoSize = true;
            this.rbPagar.Checked = true;
            this.rbPagar.Location = new System.Drawing.Point(17, 29);
            this.rbPagar.Name = "rbPagar";
            this.rbPagar.Size = new System.Drawing.Size(67, 21);
            this.rbPagar.TabIndex = 0;
            this.rbPagar.TabStop = true;
            this.rbPagar.Text = "&Pagar";
            this.rbPagar.UseVisualStyleBackColor = true;
            this.rbPagar.Click += new System.EventHandler(this.rbPagar_Click);
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(416, 78);
            this.btnConsulta.Margin = new System.Windows.Forms.Padding(4);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(127, 28);
            this.btnConsulta.TabIndex = 1;
            this.btnConsulta.Text = "C&onsulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.EnabledChanged += new System.EventHandler(this.btnConsulta_EnabledChanged);
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // btnFiltro
            // 
            this.btnFiltro.Location = new System.Drawing.Point(416, 43);
            this.btnFiltro.Margin = new System.Windows.Forms.Padding(4);
            this.btnFiltro.Name = "btnFiltro";
            this.btnFiltro.Size = new System.Drawing.Size(127, 28);
            this.btnFiltro.TabIndex = 2;
            this.btnFiltro.Text = "&Filtro";
            this.btnFiltro.UseVisualStyleBackColor = true;
            this.btnFiltro.Click += new System.EventHandler(this.btnFiltro_Click);
            // 
            // FrmConsultaPagarReceber
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 502);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmConsultaPagarReceber";
            this.Text = "FrmConsultaPagarReceber";
            this.pnTop.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.gbPagarReceber.ResumeLayout(false);
            this.gbPagarReceber.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.GroupBox gbPagarReceber;
        private System.Windows.Forms.RadioButton rbReceber;
        private System.Windows.Forms.RadioButton rbPagar;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.Button btnFiltro;
        public Pesquise oPesqFin;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.DateTimePicker dtData2;
        public System.Windows.Forms.DateTimePicker dtData1;
    }
}