namespace ClassFiltroEdite.UserControls
{
    partial class ComboBoxMD2
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
            this.gbPai = new System.Windows.Forms.GroupBox();
            this.cbPai = new System.Windows.Forms.ComboBox();
            this.gbFilho = new System.Windows.Forms.GroupBox();
            this.cbFilho = new System.Windows.Forms.ComboBox();
            this.gbPai.SuspendLayout();
            this.gbFilho.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbPai
            // 
            this.gbPai.Controls.Add(this.cbPai);
            this.gbPai.Location = new System.Drawing.Point(3, 5);
            this.gbPai.Name = "gbPai";
            this.gbPai.Size = new System.Drawing.Size(343, 60);
            this.gbPai.TabIndex = 0;
            this.gbPai.TabStop = false;
            this.gbPai.Text = "Modelo";
            // 
            // cbPai
            // 
            this.cbPai.FormattingEnabled = true;
            this.cbPai.Location = new System.Drawing.Point(11, 19);
            this.cbPai.Name = "cbPai";
            this.cbPai.Size = new System.Drawing.Size(300, 25);
            this.cbPai.TabIndex = 0;
            // 
            // gbFilho
            // 
            this.gbFilho.Controls.Add(this.cbFilho);
            this.gbFilho.Location = new System.Drawing.Point(369, 8);
            this.gbFilho.Name = "gbFilho";
            this.gbFilho.Size = new System.Drawing.Size(341, 60);
            this.gbFilho.TabIndex = 1;
            this.gbFilho.TabStop = false;
            this.gbFilho.Text = "Serviço";
            // 
            // cbFilho
            // 
            this.cbFilho.FormattingEnabled = true;
            this.cbFilho.Location = new System.Drawing.Point(11, 19);
            this.cbFilho.Name = "cbFilho";
            this.cbFilho.Size = new System.Drawing.Size(300, 25);
            this.cbFilho.TabIndex = 0;
            // 
            // ComboBoxMD2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Window;
            this.Controls.Add(this.gbFilho);
            this.Controls.Add(this.gbPai);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Name = "ComboBoxMD2";
            this.Size = new System.Drawing.Size(719, 81);
            this.gbPai.ResumeLayout(false);
            this.gbFilho.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbPai;
        private System.Windows.Forms.GroupBox gbFilho;
        private System.Windows.Forms.ComboBox cbPai;
        private System.Windows.Forms.ComboBox cbFilho;
    }
}
