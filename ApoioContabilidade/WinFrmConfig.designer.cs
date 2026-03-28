namespace ApoioContabilidade
{
    partial class WinFrmConfigura
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
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.TabPage1 = new System.Windows.Forms.TabPage();
            this.btnCancela = new System.Windows.Forms.Button();
            this.BtnOK = new System.Windows.Forms.Button();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.ButtonCampo = new System.Windows.Forms.Button();
            this.TextBoxCampo = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.ButtonTrabalho = new System.Windows.Forms.Button();
            this.TextBoxTrabalho = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.ButtonContab = new System.Windows.Forms.Button();
            this.TextBoxContab = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.ButtonEscritor = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.TextBoxEscritor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(480, 378);
            this.TabControl1.TabIndex = 0;
            // 
            // TabPage1
            // 
            this.TabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TabPage1.Controls.Add(this.panel4);
            this.TabPage1.Controls.Add(this.btnCancela);
            this.TabPage1.Controls.Add(this.BtnOK);
            this.TabPage1.Controls.Add(this.Panel1);
            this.TabPage1.Controls.Add(this.Panel2);
            this.TabPage1.Controls.Add(this.Panel3);
            this.TabPage1.Location = new System.Drawing.Point(4, 22);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Size = new System.Drawing.Size(472, 352);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Diretorios";
            // 
            // btnCancela
            // 
            this.btnCancela.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancela.Location = new System.Drawing.Point(265, 318);
            this.btnCancela.Name = "btnCancela";
            this.btnCancela.Size = new System.Drawing.Size(75, 23);
            this.btnCancela.TabIndex = 6;
            this.btnCancela.Text = "&Cancele";
            this.btnCancela.Click += new System.EventHandler(this.btnCancela_Click);
            // 
            // BtnOK
            // 
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOK.Location = new System.Drawing.Point(120, 320);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(75, 23);
            this.BtnOK.TabIndex = 5;
            this.BtnOK.Text = "&OK";
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click_1);
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.ButtonCampo);
            this.Panel1.Controls.Add(this.TextBoxCampo);
            this.Panel1.Controls.Add(this.Label1);
            this.Panel1.Location = new System.Drawing.Point(19, 24);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(432, 48);
            this.Panel1.TabIndex = 0;
            // 
            // ButtonCampo
            // 
            this.ButtonCampo.Location = new System.Drawing.Point(344, 12);
            this.ButtonCampo.Name = "ButtonCampo";
            this.ButtonCampo.Size = new System.Drawing.Size(75, 23);
            this.ButtonCampo.TabIndex = 2;
            this.ButtonCampo.Tag = "Campo";
            this.ButtonCampo.Text = "Pesquisa";
            this.ButtonCampo.Click += new System.EventHandler(this.ButtonCampo_Click);
            // 
            // TextBoxCampo
            // 
            this.TextBoxCampo.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBoxCampo.Location = new System.Drawing.Point(94, 12);
            this.TextBoxCampo.Name = "TextBoxCampo";
            this.TextBoxCampo.Size = new System.Drawing.Size(226, 20);
            this.TextBoxCampo.TabIndex = 0;
            this.TextBoxCampo.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxCampo_Validating);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(16, 12);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(72, 12);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "CAMPO";
            // 
            // Panel2
            // 
            this.Panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel2.Controls.Add(this.ButtonTrabalho);
            this.Panel2.Controls.Add(this.TextBoxTrabalho);
            this.Panel2.Controls.Add(this.Label2);
            this.Panel2.Location = new System.Drawing.Point(19, 82);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(432, 48);
            this.Panel2.TabIndex = 2;
            // 
            // ButtonTrabalho
            // 
            this.ButtonTrabalho.Location = new System.Drawing.Point(344, 12);
            this.ButtonTrabalho.Name = "ButtonTrabalho";
            this.ButtonTrabalho.Size = new System.Drawing.Size(75, 23);
            this.ButtonTrabalho.TabIndex = 2;
            this.ButtonTrabalho.Tag = "Trabalho";
            this.ButtonTrabalho.Text = "Pesquisa";
            this.ButtonTrabalho.Click += new System.EventHandler(this.ButtonCampo_Click);
            // 
            // TextBoxTrabalho
            // 
            this.TextBoxTrabalho.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBoxTrabalho.Location = new System.Drawing.Point(94, 12);
            this.TextBoxTrabalho.Name = "TextBoxTrabalho";
            this.TextBoxTrabalho.Size = new System.Drawing.Size(226, 20);
            this.TextBoxTrabalho.TabIndex = 1;
            this.TextBoxTrabalho.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxCampo_Validating);
            // 
            // Label2
            // 
            this.Label2.Location = new System.Drawing.Point(16, 12);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 12);
            this.Label2.TabIndex = 1;
            this.Label2.Text = "TRABALHO";
            // 
            // Panel3
            // 
            this.Panel3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel3.Controls.Add(this.ButtonContab);
            this.Panel3.Controls.Add(this.TextBoxContab);
            this.Panel3.Controls.Add(this.Label3);
            this.Panel3.Location = new System.Drawing.Point(19, 209);
            this.Panel3.Name = "Panel3";
            this.Panel3.Size = new System.Drawing.Size(432, 56);
            this.Panel3.TabIndex = 2;
            // 
            // ButtonContab
            // 
            this.ButtonContab.Location = new System.Drawing.Point(344, 12);
            this.ButtonContab.Name = "ButtonContab";
            this.ButtonContab.Size = new System.Drawing.Size(75, 23);
            this.ButtonContab.TabIndex = 2;
            this.ButtonContab.Tag = "Contab";
            this.ButtonContab.Text = "Pesquisa";
            this.ButtonContab.Click += new System.EventHandler(this.ButtonCampo_Click);
            // 
            // TextBoxContab
            // 
            this.TextBoxContab.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBoxContab.Location = new System.Drawing.Point(94, 12);
            this.TextBoxContab.Name = "TextBoxContab";
            this.TextBoxContab.Size = new System.Drawing.Size(226, 20);
            this.TextBoxContab.TabIndex = 1;
            this.TextBoxContab.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxCampo_Validating);
            // 
            // Label3
            // 
            this.Label3.Location = new System.Drawing.Point(16, 12);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(72, 12);
            this.Label3.TabIndex = 0;
            this.Label3.Text = "CONTAB";
            // 
            // ButtonEscritor
            // 
            this.ButtonEscritor.Location = new System.Drawing.Point(344, 12);
            this.ButtonEscritor.Name = "ButtonEscritor";
            this.ButtonEscritor.Size = new System.Drawing.Size(75, 23);
            this.ButtonEscritor.TabIndex = 2;
            this.ButtonEscritor.Tag = "Escritor";
            this.ButtonEscritor.Text = "Pesquisa";
            this.ButtonEscritor.Click += new System.EventHandler(this.ButtonCampo_Click);
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel4.Controls.Add(this.ButtonEscritor);
            this.panel4.Controls.Add(this.TextBoxEscritor);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Location = new System.Drawing.Point(19, 148);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(432, 55);
            this.panel4.TabIndex = 7;
            // 
            // TextBoxEscritor
            // 
            this.TextBoxEscritor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.TextBoxEscritor.Location = new System.Drawing.Point(94, 12);
            this.TextBoxEscritor.Name = "TextBoxEscritor";
            this.TextBoxEscritor.Size = new System.Drawing.Size(226, 20);
            this.TextBoxEscritor.TabIndex = 1;
            this.TextBoxEscritor.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxCampo_Validating);
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(16, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 12);
            this.label4.TabIndex = 0;
            this.label4.Text = "FINANCEIRO";
            // 
            // WinFrmConfigura
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancela;
            this.ClientSize = new System.Drawing.Size(480, 378);
            this.Controls.Add(this.TabControl1);
            this.Name = "WinFrmConfigura";
            this.Text = "DIRETORIOS";
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.Panel2.ResumeLayout(false);
            this.Panel2.PerformLayout();
            this.Panel3.ResumeLayout(false);
            this.Panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion


    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    /// 

        System.Windows.Forms.TabControl TabControl1;
        System.Windows.Forms.TabPage TabPage1;

        System.Windows.Forms.Panel Panel1,Panel2,Panel3;
        System.Windows.Forms.Label Label1, Label2,Label3;
        System.Windows.Forms.Button ButtonCampo;
        System.Windows.Forms.Button ButtonTrabalho;
        System.Windows.Forms.Button ButtonContab;
        System.Windows.Forms.Button BtnOK;
        System.Windows.Forms.Button btnCancela;
        public System.Windows.Forms.TextBox TextBoxCampo;
        public System.Windows.Forms.TextBox TextBoxContab;
        public System.Windows.Forms.TextBox TextBoxTrabalho;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button ButtonEscritor;
        public System.Windows.Forms.TextBox TextBoxEscritor;
        private System.Windows.Forms.Label label4;
        //private FirebirdSql.Data.FirebirdClient.FbConnection fbConnection1;
 
 /*   procedure InitializeComponent;
    procedure TabPage1_Click(sender: System.Object; e: System.EventArgs);
    procedure Button1_Click(sender: System.Object; e: System.EventArgs);
    procedure TextBox1_Validating(sender: System.Object; e: System.ComponentModel.CancelEventArgs);*/
    }
}