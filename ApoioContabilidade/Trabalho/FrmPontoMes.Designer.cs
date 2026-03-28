namespace ApoioContabilidade.Trabalho
{
    partial class FrmPontoMes
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
            this.pnCab = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txSetores = new System.Windows.Forms.TextBox();
            this.lbTexto = new System.Windows.Forms.Label();
            this.btnExcel = new System.Windows.Forms.Button();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.pnCorpo = new System.Windows.Forms.Panel();
            this.dgvPonto = new System.Windows.Forms.DataGridView();
            this.pnCab.SuspendLayout();
            this.pnCorpo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPonto)).BeginInit();
            this.SuspendLayout();
            // 
            // pnCab
            // 
            this.pnCab.Controls.Add(this.label1);
            this.pnCab.Controls.Add(this.txSetores);
            this.pnCab.Controls.Add(this.lbTexto);
            this.pnCab.Controls.Add(this.btnExcel);
            this.pnCab.Controls.Add(this.btnConsulta);
            this.pnCab.Controls.Add(this.dtData1);
            this.pnCab.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnCab.Location = new System.Drawing.Point(0, 0);
            this.pnCab.Name = "pnCab";
            this.pnCab.Size = new System.Drawing.Size(1472, 70);
            this.pnCab.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(782, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(42, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "Setor:";
            // 
            // txSetores
            // 
            this.txSetores.Location = new System.Drawing.Point(834, 19);
            this.txSetores.Name = "txSetores";
            this.txSetores.Size = new System.Drawing.Size(109, 22);
            this.txSetores.TabIndex = 5;
            // 
            // lbTexto
            // 
            this.lbTexto.AutoSize = true;
            this.lbTexto.Location = new System.Drawing.Point(192, 30);
            this.lbTexto.Name = "lbTexto";
            this.lbTexto.Size = new System.Drawing.Size(0, 16);
            this.lbTexto.TabIndex = 4;
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(1253, 17);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(136, 33);
            this.btnExcel.TabIndex = 3;
            this.btnExcel.Text = "&Excel";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(1129, 14);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(88, 39);
            this.btnConsulta.TabIndex = 2;
            this.btnConsulta.Text = "&Consulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // dtData1
            // 
            this.dtData1.Checked = false;
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(30, 25);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(110, 22);
            this.dtData1.TabIndex = 1;
            // 
            // pnCorpo
            // 
            this.pnCorpo.Controls.Add(this.dgvPonto);
            this.pnCorpo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnCorpo.Location = new System.Drawing.Point(0, 70);
            this.pnCorpo.Name = "pnCorpo";
            this.pnCorpo.Size = new System.Drawing.Size(1472, 464);
            this.pnCorpo.TabIndex = 2;
            // 
            // dgvPonto
            // 
            this.dgvPonto.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPonto.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPonto.Location = new System.Drawing.Point(0, 0);
            this.dgvPonto.Name = "dgvPonto";
            this.dgvPonto.RowHeadersWidth = 51;
            this.dgvPonto.RowTemplate.Height = 24;
            this.dgvPonto.Size = new System.Drawing.Size(1472, 464);
            this.dgvPonto.TabIndex = 0;
            // 
            // FrmPontoMes
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1472, 534);
            this.Controls.Add(this.pnCorpo);
            this.Controls.Add(this.pnCab);
            this.Name = "FrmPontoMes";
            this.Text = "FrmPontoMes";
            this.pnCab.ResumeLayout(false);
            this.pnCab.PerformLayout();
            this.pnCorpo.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPonto)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnCab;
        private System.Windows.Forms.Panel pnCorpo;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnConsulta;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.DataGridView dgvPonto;
        private System.Windows.Forms.Label lbTexto;
        private System.Windows.Forms.TextBox txSetores;
        private System.Windows.Forms.Label label1;
    }
}