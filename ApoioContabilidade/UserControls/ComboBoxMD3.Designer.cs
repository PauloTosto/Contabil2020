namespace ApoioContabilidade.UserControls
{
    partial class ComboBoxMD3
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gbFilho = new System.Windows.Forms.GroupBox();
            this.cbFilho = new System.Windows.Forms.ComboBox();
            this.gbPai = new System.Windows.Forms.GroupBox();
            this.cbPai = new System.Windows.Forms.ComboBox();
            this.gbNeto = new System.Windows.Forms.GroupBox();
            this.cbNeto = new System.Windows.Forms.ComboBox();
            this.gbFilho.SuspendLayout();
            this.gbPai.SuspendLayout();
            this.gbNeto.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFilho
            // 
            this.gbFilho.Controls.Add(this.cbFilho);
            this.gbFilho.Location = new System.Drawing.Point(319, 6);
            this.gbFilho.Name = "gbFilho";
            this.gbFilho.Size = new System.Drawing.Size(194, 56);
            this.gbFilho.TabIndex = 3;
            this.gbFilho.TabStop = false;
            this.gbFilho.Text = "Gleba";
            // 
            // cbFilho
            // 
            this.cbFilho.FormattingEnabled = true;
            this.cbFilho.Location = new System.Drawing.Point(17, 18);
            this.cbFilho.Name = "cbFilho";
            this.cbFilho.Size = new System.Drawing.Size(147, 24);
            this.cbFilho.TabIndex = 0;
            // 
            // gbPai
            // 
            this.gbPai.Controls.Add(this.cbPai);
            this.gbPai.Location = new System.Drawing.Point(13, 3);
            this.gbPai.Name = "gbPai";
            this.gbPai.Size = new System.Drawing.Size(291, 56);
            this.gbPai.TabIndex = 2;
            this.gbPai.TabStop = false;
            this.gbPai.Text = "Setor";
            // 
            // cbPai
            // 
            this.cbPai.FormattingEnabled = true;
            this.cbPai.Location = new System.Drawing.Point(15, 18);
            this.cbPai.Name = "cbPai";
            this.cbPai.Size = new System.Drawing.Size(244, 24);
            this.cbPai.TabIndex = 0;
            // 
            // gbNeto
            // 
            this.gbNeto.Controls.Add(this.cbNeto);
            this.gbNeto.Location = new System.Drawing.Point(529, 5);
            this.gbNeto.Name = "gbNeto";
            this.gbNeto.Size = new System.Drawing.Size(173, 56);
            this.gbNeto.TabIndex = 4;
            this.gbNeto.TabStop = false;
            this.gbNeto.Text = "Quadra";
            // 
            // cbNeto
            // 
            this.cbNeto.FormattingEnabled = true;
            this.cbNeto.Location = new System.Drawing.Point(16, 18);
            this.cbNeto.Name = "cbNeto";
            this.cbNeto.Size = new System.Drawing.Size(129, 24);
            this.cbNeto.TabIndex = 0;
            // 
            // ComboBoxMD3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbNeto);
            this.Controls.Add(this.gbFilho);
            this.Controls.Add(this.gbPai);
            this.Name = "ComboBoxMD3";
            this.Size = new System.Drawing.Size(713, 77);
            this.gbFilho.ResumeLayout(false);
            this.gbPai.ResumeLayout(false);
            this.gbNeto.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbFilho;
        private System.Windows.Forms.ComboBox cbFilho;
        private System.Windows.Forms.GroupBox gbPai;
        private System.Windows.Forms.ComboBox cbPai;
        private System.Windows.Forms.GroupBox gbNeto;
        private System.Windows.Forms.ComboBox cbNeto;
    }
}
