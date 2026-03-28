namespace ApoioContabilidade.Financeiro
{
    partial class FrmConciliaBB
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
            this.pnGeral = new System.Windows.Forms.Panel();
            this.btnLeiaExcel = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pnTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.btnLeiaExcel);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1097, 68);
            this.pnTop.TabIndex = 0;
            // 
            // pnGeral
            // 
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGeral.Location = new System.Drawing.Point(0, 68);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1097, 382);
            this.pnGeral.TabIndex = 1;
            // 
            // btnLeiaExcel
            // 
            this.btnLeiaExcel.Location = new System.Drawing.Point(53, 14);
            this.btnLeiaExcel.Name = "btnLeiaExcel";
            this.btnLeiaExcel.Size = new System.Drawing.Size(95, 23);
            this.btnLeiaExcel.TabIndex = 0;
            this.btnLeiaExcel.Text = "Leia E&xcel";
            this.btnLeiaExcel.UseVisualStyleBackColor = true;
            this.btnLeiaExcel.Click += new System.EventHandler(this.btnLeiaExcel_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FrmConciliaBB
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1097, 450);
            this.Controls.Add(this.pnGeral);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmConciliaBB";
            this.Text = "FrmConciliaBB";
            this.pnTop.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.Button btnLeiaExcel;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}