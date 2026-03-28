namespace ApoioContabilidade
{
    partial class FrmBalancinho
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
            this.txNumconta = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnGeral = new System.Windows.Forms.Panel();
            this.sbBalancinho = new System.Windows.Forms.StatusBar();
            this.dgvBalancinho = new System.Windows.Forms.DataGridView();
            this.pnTop.SuspendLayout();
            this.pnGeral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalancinho)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.txNumconta);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1245, 48);
            this.pnTop.TabIndex = 0;
            // 
            // txNumconta
            // 
            this.txNumconta.Location = new System.Drawing.Point(139, 8);
            this.txNumconta.Name = "txNumconta";
            this.txNumconta.Size = new System.Drawing.Size(195, 22);
            this.txNumconta.TabIndex = 1;
            this.txNumconta.TextChanged += new System.EventHandler(this.txNumconta_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filtre NumConta:";
            // 
            // pnGeral
            // 
            this.pnGeral.Controls.Add(this.sbBalancinho);
            this.pnGeral.Controls.Add(this.dgvBalancinho);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGeral.Location = new System.Drawing.Point(0, 48);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1245, 659);
            this.pnGeral.TabIndex = 1;
            // 
            // sbBalancinho
            // 
            this.sbBalancinho.Dock = System.Windows.Forms.DockStyle.None;
            this.sbBalancinho.Location = new System.Drawing.Point(22, 619);
            this.sbBalancinho.Margin = new System.Windows.Forms.Padding(4);
            this.sbBalancinho.Name = "sbBalancinho";
            this.sbBalancinho.Size = new System.Drawing.Size(133, 27);
            this.sbBalancinho.SizingGrip = false;
            this.sbBalancinho.TabIndex = 8;
            // 
            // dgvBalancinho
            // 
            this.dgvBalancinho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBalancinho.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvBalancinho.Location = new System.Drawing.Point(0, 0);
            this.dgvBalancinho.Name = "dgvBalancinho";
            this.dgvBalancinho.RowHeadersWidth = 51;
            this.dgvBalancinho.RowTemplate.Height = 24;
            this.dgvBalancinho.Size = new System.Drawing.Size(1245, 612);
            this.dgvBalancinho.TabIndex = 0;
            // 
            // FrmBalancinho
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1245, 707);
            this.Controls.Add(this.pnGeral);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmBalancinho";
            this.Text = "FrmBalancinho";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.pnGeral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalancinho)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.DataGridView dgvBalancinho;
        private System.Windows.Forms.StatusBar sbBalancinho;
        private System.Windows.Forms.TextBox txNumconta;
        private System.Windows.Forms.Label label1;
    }
}