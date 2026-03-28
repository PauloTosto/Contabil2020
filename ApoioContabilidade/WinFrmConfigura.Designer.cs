namespace PrjApiParceiro_C
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
            this.textBoxServidor = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.TabControl1.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TabControl1
            // 
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TabControl1.Location = new System.Drawing.Point(0, 0);
            this.TabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(512, 224);
            this.TabControl1.TabIndex = 0;
            // 
            // TabPage1
            // 
            this.TabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.TabPage1.Controls.Add(this.btnCancela);
            this.TabPage1.Controls.Add(this.BtnOK);
            this.TabPage1.Controls.Add(this.Panel1);
            this.TabPage1.Location = new System.Drawing.Point(4, 25);
            this.TabPage1.Margin = new System.Windows.Forms.Padding(4);
            this.TabPage1.Name = "TabPage1";
            this.TabPage1.Size = new System.Drawing.Size(504, 195);
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "Informe";
            // 
            // btnCancela
            // 
            this.btnCancela.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancela.Location = new System.Drawing.Point(312, 134);
            this.btnCancela.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancela.Name = "btnCancela";
            this.btnCancela.Size = new System.Drawing.Size(100, 28);
            this.btnCancela.TabIndex = 0;
            this.btnCancela.TabStop = false;
            this.btnCancela.Text = "&Cancele";
            // 
            // BtnOK
            // 
            this.BtnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.BtnOK.Location = new System.Drawing.Point(104, 134);
            this.BtnOK.Margin = new System.Windows.Forms.Padding(4);
            this.BtnOK.Name = "BtnOK";
            this.BtnOK.Size = new System.Drawing.Size(100, 28);
            this.BtnOK.TabIndex = 0;
            this.BtnOK.TabStop = false;
            this.BtnOK.Text = "&OK";
            this.BtnOK.Click += new System.EventHandler(this.BtnOK_Click);
            // 
            // Panel1
            // 
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.textBoxServidor);
            this.Panel1.Controls.Add(this.Label1);
            this.Panel1.Location = new System.Drawing.Point(25, 30);
            this.Panel1.Margin = new System.Windows.Forms.Padding(4);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(459, 59);
            this.Panel1.TabIndex = 0;
            // 
            // textBoxServidor
            // 
            this.textBoxServidor.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.textBoxServidor.Location = new System.Drawing.Point(108, 15);
            this.textBoxServidor.Margin = new System.Windows.Forms.Padding(4);
            this.textBoxServidor.Name = "textBoxServidor";
            this.textBoxServidor.Size = new System.Drawing.Size(300, 22);
            this.textBoxServidor.TabIndex = 1;
            this.textBoxServidor.Validating += new System.ComponentModel.CancelEventHandler(this.TextBoxServidor_Validating);
            // 
            // Label1
            // 
            this.Label1.Location = new System.Drawing.Point(4, 25);
            this.Label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(96, 15);
            this.Label1.TabIndex = 0;
            this.Label1.Text = "SERVIDOR";
            // 
            // WinFrmConfigura
            // 
            this.AcceptButton = this.BtnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancela;
            this.ClientSize = new System.Drawing.Size(512, 224);
            this.ControlBox = false;
            this.Controls.Add(this.TabControl1);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "WinFrmConfigura";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "LOCALIZAÇÃO DADOS";
            this.TabControl1.ResumeLayout(false);
            this.TabPage1.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
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

        System.Windows.Forms.Panel Panel1;
        System.Windows.Forms.Label Label1;
        System.Windows.Forms.Button BtnOK;
        System.Windows.Forms.Button btnCancela;
        public System.Windows.Forms.TextBox textBoxServidor;

        /*   procedure InitializeComponent;
           procedure TabPage1_Click(sender: System.Object; e: System.EventArgs);
           procedure Button1_Click(sender: System.Object; e: System.EventArgs);
           procedure TextBox1_Validating(sender: System.Object; e: System.ComponentModel.CancelEventArgs);*/
    }
}