namespace ApoioContabilidade.Fiscal
{
    partial class FrmRecarregueDisponivel
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
            this.pnMestre = new System.Windows.Forms.Panel();
            this.pnCabecalho = new System.Windows.Forms.Panel();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.pnMestreTop = new System.Windows.Forms.Panel();
            this.dgvMestre = new System.Windows.Forms.DataGridView();
            this.pnMestreTotal = new System.Windows.Forms.Panel();
            this.statusMestre = new System.Windows.Forms.StatusStrip();
            this.pnDetalhe = new System.Windows.Forms.Panel();
            this.pnDetalheTotal = new System.Windows.Forms.Panel();
            this.statusDetalhe = new System.Windows.Forms.StatusStrip();
            this.pnDetalheTop = new System.Windows.Forms.Panel();
            this.dgvDetalhe = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.pnMestre.SuspendLayout();
            this.pnCabecalho.SuspendLayout();
            this.pnMestreTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMestre)).BeginInit();
            this.pnMestreTotal.SuspendLayout();
            this.pnDetalhe.SuspendLayout();
            this.pnDetalheTotal.SuspendLayout();
            this.pnDetalheTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalhe)).BeginInit();
            this.SuspendLayout();
            // 
            // pnMestre
            // 
            this.pnMestre.Controls.Add(this.pnCabecalho);
            this.pnMestre.Controls.Add(this.pnMestreTop);
            this.pnMestre.Controls.Add(this.pnMestreTotal);
            this.pnMestre.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMestre.Location = new System.Drawing.Point(0, 0);
            this.pnMestre.Name = "pnMestre";
            this.pnMestre.Size = new System.Drawing.Size(800, 214);
            this.pnMestre.TabIndex = 0;
            // 
            // pnCabecalho
            // 
            this.pnCabecalho.Controls.Add(this.button1);
            this.pnCabecalho.Controls.Add(this.checkBox1);
            this.pnCabecalho.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnCabecalho.Location = new System.Drawing.Point(0, 0);
            this.pnCabecalho.Name = "pnCabecalho";
            this.pnCabecalho.Size = new System.Drawing.Size(800, 39);
            this.pnCabecalho.TabIndex = 0;
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(470, 7);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(130, 21);
            this.checkBox1.TabIndex = 0;
            this.checkBox1.Text = "Mostra Zerados";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckStateChanged += new System.EventHandler(this.checkBox1_CheckStateChanged);
            // 
            // pnMestreTop
            // 
            this.pnMestreTop.Controls.Add(this.dgvMestre);
            this.pnMestreTop.Location = new System.Drawing.Point(0, 53);
            this.pnMestreTop.Name = "pnMestreTop";
            this.pnMestreTop.Size = new System.Drawing.Size(800, 114);
            this.pnMestreTop.TabIndex = 2;
            // 
            // dgvMestre
            // 
            this.dgvMestre.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMestre.Location = new System.Drawing.Point(0, 3);
            this.dgvMestre.Name = "dgvMestre";
            this.dgvMestre.RowHeadersWidth = 51;
            this.dgvMestre.RowTemplate.Height = 24;
            this.dgvMestre.Size = new System.Drawing.Size(800, 121);
            this.dgvMestre.TabIndex = 0;
            // 
            // pnMestreTotal
            // 
            this.pnMestreTotal.Controls.Add(this.statusMestre);
            this.pnMestreTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnMestreTotal.Location = new System.Drawing.Point(0, 176);
            this.pnMestreTotal.Name = "pnMestreTotal";
            this.pnMestreTotal.Size = new System.Drawing.Size(800, 38);
            this.pnMestreTotal.TabIndex = 1;
            // 
            // statusMestre
            // 
            this.statusMestre.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusMestre.Location = new System.Drawing.Point(0, 16);
            this.statusMestre.Name = "statusMestre";
            this.statusMestre.Size = new System.Drawing.Size(800, 22);
            this.statusMestre.TabIndex = 0;
            this.statusMestre.Text = "statusStrip1";
            // 
            // pnDetalhe
            // 
            this.pnDetalhe.Controls.Add(this.pnDetalheTotal);
            this.pnDetalhe.Controls.Add(this.pnDetalheTop);
            this.pnDetalhe.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnDetalhe.Location = new System.Drawing.Point(0, 231);
            this.pnDetalhe.Name = "pnDetalhe";
            this.pnDetalhe.Size = new System.Drawing.Size(800, 267);
            this.pnDetalhe.TabIndex = 1;
            // 
            // pnDetalheTotal
            // 
            this.pnDetalheTotal.Controls.Add(this.statusDetalhe);
            this.pnDetalheTotal.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnDetalheTotal.Location = new System.Drawing.Point(0, 210);
            this.pnDetalheTotal.Name = "pnDetalheTotal";
            this.pnDetalheTotal.Size = new System.Drawing.Size(800, 57);
            this.pnDetalheTotal.TabIndex = 1;
            // 
            // statusDetalhe
            // 
            this.statusDetalhe.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusDetalhe.Location = new System.Drawing.Point(0, 35);
            this.statusDetalhe.Name = "statusDetalhe";
            this.statusDetalhe.Size = new System.Drawing.Size(800, 22);
            this.statusDetalhe.TabIndex = 0;
            this.statusDetalhe.Text = "statusStrip1";
            // 
            // pnDetalheTop
            // 
            this.pnDetalheTop.Controls.Add(this.dgvDetalhe);
            this.pnDetalheTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnDetalheTop.Location = new System.Drawing.Point(0, 0);
            this.pnDetalheTop.Name = "pnDetalheTop";
            this.pnDetalheTop.Size = new System.Drawing.Size(800, 204);
            this.pnDetalheTop.TabIndex = 0;
            // 
            // dgvDetalhe
            // 
            this.dgvDetalhe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalhe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDetalhe.Location = new System.Drawing.Point(0, 0);
            this.dgvDetalhe.Name = "dgvDetalhe";
            this.dgvDetalhe.RowHeadersWidth = 51;
            this.dgvDetalhe.RowTemplate.Height = 24;
            this.dgvDetalhe.Size = new System.Drawing.Size(800, 204);
            this.dgvDetalhe.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(647, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "E&xcel";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmRecarregueDisponivel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 498);
            this.Controls.Add(this.pnDetalhe);
            this.Controls.Add(this.pnMestre);
            this.Name = "FrmRecarregueDisponivel";
            this.Text = "Disponivel";
            this.pnMestre.ResumeLayout(false);
            this.pnCabecalho.ResumeLayout(false);
            this.pnCabecalho.PerformLayout();
            this.pnMestreTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMestre)).EndInit();
            this.pnMestreTotal.ResumeLayout(false);
            this.pnMestreTotal.PerformLayout();
            this.pnDetalhe.ResumeLayout(false);
            this.pnDetalheTotal.ResumeLayout(false);
            this.pnDetalheTotal.PerformLayout();
            this.pnDetalheTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalhe)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnMestre;
        private System.Windows.Forms.Panel pnMestreTotal;
        private System.Windows.Forms.StatusStrip statusMestre;
        private System.Windows.Forms.Panel pnMestreTop;
        private System.Windows.Forms.DataGridView dgvMestre;
        private System.Windows.Forms.Panel pnDetalhe;
        private System.Windows.Forms.Panel pnDetalheTotal;
        private System.Windows.Forms.Panel pnDetalheTop;
        private System.Windows.Forms.DataGridView dgvDetalhe;
        private System.Windows.Forms.StatusStrip statusDetalhe;
        private System.Windows.Forms.Panel pnCabecalho;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Button button1;
    }
}