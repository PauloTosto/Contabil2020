using ClassFiltroEdite;

namespace ApoioContabilidade.Financeiro_MOVFIN
{
    partial class FrmFinanc_MOVFINConsulta
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
        private void InitializeComponent()
        {
            this.pnTop = new System.Windows.Forms.Panel();
            this.btnSair = new System.Windows.Forms.Button();
            this.dtData2 = new System.Windows.Forms.DateTimePicker();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.btnFiltro = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.btnSair);
            this.pnTop.Controls.Add(this.dtData2);
            this.pnTop.Controls.Add(this.dtData1);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.btnFiltro);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(627, 160);
            this.pnTop.TabIndex = 0;
            // 
            // btnSair
            // 
            this.btnSair.Location = new System.Drawing.Point(416, 12);
            this.btnSair.Margin = new System.Windows.Forms.Padding(4);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(127, 28);
            this.btnSair.TabIndex = 9;
            this.btnSair.Text = "&Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            // 
            // dtData2
            // 
            this.dtData2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData2.Location = new System.Drawing.Point(162, 35);
            this.dtData2.Margin = new System.Windows.Forms.Padding(4);
            this.dtData2.Name = "dtData2";
            this.dtData2.Size = new System.Drawing.Size(123, 22);
            this.dtData2.TabIndex = 7;
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(30, 35);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(123, 22);
            this.dtData1.TabIndex = 6;
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(416, 78);
            this.btnConsulta.Margin = new System.Windows.Forms.Padding(4);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(127, 28);
            this.btnConsulta.TabIndex = 5;
            this.btnConsulta.Text = "C&onsulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            // 
            // btnFiltro
            // 
            this.btnFiltro.Location = new System.Drawing.Point(416, 43);
            this.btnFiltro.Margin = new System.Windows.Forms.Padding(4);
            this.btnFiltro.Name = "btnFiltro";
            this.btnFiltro.Size = new System.Drawing.Size(127, 28);
            this.btnFiltro.TabIndex = 4;
            this.btnFiltro.Text = "&Filtro";
            this.btnFiltro.UseVisualStyleBackColor = true;
            // 
            // FrmFinanc_MOVFINConsulta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 502);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmFinanc_MOVFINConsulta";
            this.Text = "FrmFinancConsulta";
            this.pnTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Button btnSair;
        public System.Windows.Forms.DateTimePicker dtData2;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.Button btnFiltro;
       // public Pesquise oPesqFin;
        public Pesquise oPesqFin;



    }
}