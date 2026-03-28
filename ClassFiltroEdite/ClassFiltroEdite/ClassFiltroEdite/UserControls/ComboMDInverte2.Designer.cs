namespace ClassFiltroEdite.UserControls
{
    partial class ComboMDInverte2
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
            this.gbNeto = new System.Windows.Forms.GroupBox();
            this.cbNeto = new System.Windows.Forms.ComboBox();
            this.gbFilho.SuspendLayout();
            this.gbNeto.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbFilho
            // 
            this.gbFilho.Controls.Add(this.cbFilho);
            this.gbFilho.Location = new System.Drawing.Point(233, 6);
            this.gbFilho.Name = "gbFilho";
            this.gbFilho.Size = new System.Drawing.Size(214, 56);
            this.gbFilho.TabIndex = 1;
            this.gbFilho.TabStop = false;
            this.gbFilho.Text = "Gleba";
            // 
            // cbFilho
            // 
            this.cbFilho.Location = new System.Drawing.Point(17, 18);
            this.cbFilho.Name = "cbFilho";
            this.cbFilho.Size = new System.Drawing.Size(175, 24);
            this.cbFilho.TabIndex = 1;
            // 
            // gbNeto
            // 
            this.gbNeto.Controls.Add(this.cbNeto);
            this.gbNeto.Location = new System.Drawing.Point(18, 5);
            this.gbNeto.Name = "gbNeto";
            this.gbNeto.Size = new System.Drawing.Size(173, 56);
            this.gbNeto.TabIndex = 0;
            this.gbNeto.TabStop = false;
            this.gbNeto.Text = "Quadra";
            // 
            // cbNeto
            // 
            this.cbNeto.Location = new System.Drawing.Point(16, 18);
            this.cbNeto.Name = "cbNeto";
            this.cbNeto.Size = new System.Drawing.Size(129, 24);
            this.cbNeto.TabIndex = 0;
            // 
            // ComboMDInverte2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbNeto);
            this.Controls.Add(this.gbFilho);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Name = "ComboMDInverte2";
            this.Size = new System.Drawing.Size(474, 77);
            this.gbFilho.ResumeLayout(false);
            this.gbNeto.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckBox cbResposta;
        private System.Windows.Forms.Label lbTitulo;
        private System.Windows.Forms.TextBox txBoxDado;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox gbFilho;
        private System.Windows.Forms.ComboBox cbFilho;
        private System.Windows.Forms.GroupBox gbNeto;
        private System.Windows.Forms.ComboBox cbNeto;
    }
}
