namespace PrjApiParceiro_C
{
    partial class FrmCaminhoXML
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtCUF = new System.Windows.Forms.TextBox();
            this.txtPATHNFE = new System.Windows.Forms.TextBox();
            this.txtCNPJ = new System.Windows.Forms.TextBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.BtnCancele = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.txtCUF);
            this.panel1.Controls.Add(this.txtPATHNFE);
            this.panel1.Controls.Add(this.txtCNPJ);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(663, 240);
            this.panel1.TabIndex = 0;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(41, 175);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(78, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Código UF:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(41, 109);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 17);
            this.label2.TabIndex = 5;
            this.label2.Text = "Caminho NF XML:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(41, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 17);
            this.label1.TabIndex = 3;
            this.label1.Text = "CNPJ Empresa:";
            // 
            // txtCUF
            // 
            this.txtCUF.Location = new System.Drawing.Point(176, 172);
            this.txtCUF.Name = "txtCUF";
            this.txtCUF.Size = new System.Drawing.Size(66, 22);
            this.txtCUF.TabIndex = 2;
            // 
            // txtPATHNFE
            // 
            this.txtPATHNFE.Location = new System.Drawing.Point(176, 104);
            this.txtPATHNFE.Name = "txtPATHNFE";
            this.txtPATHNFE.Size = new System.Drawing.Size(445, 22);
            this.txtPATHNFE.TabIndex = 1;
            // 
            // txtCNPJ
            // 
            this.txtCNPJ.Location = new System.Drawing.Point(176, 36);
            this.txtCNPJ.Name = "txtCNPJ";
            this.txtCNPJ.Size = new System.Drawing.Size(224, 22);
            this.txtCNPJ.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.BtnCancele);
            this.panel2.Controls.Add(this.btnOK);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 236);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(663, 91);
            this.panel2.TabIndex = 1;
            // 
            // BtnCancele
            // 
            this.BtnCancele.Location = new System.Drawing.Point(274, 33);
            this.BtnCancele.Name = "BtnCancele";
            this.BtnCancele.Size = new System.Drawing.Size(75, 26);
            this.BtnCancele.TabIndex = 2;
            this.BtnCancele.Text = "&Cancele";
            this.BtnCancele.UseVisualStyleBackColor = true;
            this.BtnCancele.Click += new System.EventHandler(this.BtnCancele_Click);
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(167, 33);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 26);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // FrmCaminhoXML
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(663, 327);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FrmCaminhoXML";
            this.Text = "Configure Caminho XML";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtCUF;
        private System.Windows.Forms.TextBox txtPATHNFE;
        private System.Windows.Forms.TextBox txtCNPJ;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button BtnCancele;
        private System.Windows.Forms.Button btnOK;
    }
}