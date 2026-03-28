namespace ApoioContabilidade.Financeiro
{
    partial class FrmCheques
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
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.testeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fontesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.imprimaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tpCheque = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.sbCheque = new System.Windows.Forms.StatusBar();
            this.dgvCheque = new System.Windows.Forms.DataGridView();
            this.tpChequesFilho = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.dgvChequesFilho = new System.Windows.Forms.DataGridView();
            this.printDocument2 = new System.Drawing.Printing.PrintDocument();
            this.printDialog2 = new System.Windows.Forms.PrintDialog();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tpRecibos = new System.Windows.Forms.TabPage();
            this.btnRecibo = new System.Windows.Forms.Button();
            this.gbImprime = new System.Windows.Forms.GroupBox();
            this.rbVauches = new System.Windows.Forms.RadioButton();
            this.rbRecibos = new System.Windows.Forms.RadioButton();
            this.tpCheques = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.btnGrave = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnChequeUmaFolha = new System.Windows.Forms.Button();
            this.btnCheque = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tpCheque.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheque)).BeginInit();
            this.tpChequesFilho.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvChequesFilho)).BeginInit();
            this.tabControl2.SuspendLayout();
            this.tpRecibos.SuspendLayout();
            this.gbImprime.SuspendLayout();
            this.tpCheques.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1361, 28);
            this.menuStrip1.TabIndex = 3;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // testeToolStripMenuItem
            // 
            this.testeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fontesToolStripMenuItem,
            this.imprimaToolStripMenuItem,
            this.previewToolStripMenuItem});
            this.testeToolStripMenuItem.Name = "testeToolStripMenuItem";
            this.testeToolStripMenuItem.Size = new System.Drawing.Size(57, 24);
            this.testeToolStripMenuItem.Text = "Teste";
            // 
            // fontesToolStripMenuItem
            // 
            this.fontesToolStripMenuItem.Name = "fontesToolStripMenuItem";
            this.fontesToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.fontesToolStripMenuItem.Text = "Fontes";
            this.fontesToolStripMenuItem.Click += new System.EventHandler(this.DisplayFonts_Click1);
            // 
            // imprimaToolStripMenuItem
            // 
            this.imprimaToolStripMenuItem.Name = "imprimaToolStripMenuItem";
            this.imprimaToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.imprimaToolStripMenuItem.Text = "Imprima";
            this.imprimaToolStripMenuItem.Click += new System.EventHandler(this.PrintMenuClick);
            // 
            // previewToolStripMenuItem
            // 
            this.previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            this.previewToolStripMenuItem.Size = new System.Drawing.Size(148, 26);
            this.previewToolStripMenuItem.Text = "Preview";
            this.previewToolStripMenuItem.Click += new System.EventHandler(this.PrintMenuClick);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tpCheque);
            this.tabControl1.Controls.Add(this.tpChequesFilho);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1246, 551);
            this.tabControl1.TabIndex = 4;
            // 
            // tpCheque
            // 
            this.tpCheque.Controls.Add(this.panel1);
            this.tpCheque.Location = new System.Drawing.Point(4, 25);
            this.tpCheque.Name = "tpCheque";
            this.tpCheque.Padding = new System.Windows.Forms.Padding(3);
            this.tpCheque.Size = new System.Drawing.Size(1238, 522);
            this.tpCheque.TabIndex = 0;
            this.tpCheque.Text = "Cheques";
            this.tpCheque.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.sbCheque);
            this.panel1.Controls.Add(this.dgvCheque);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1232, 516);
            this.panel1.TabIndex = 1;
            // 
            // sbCheque
            // 
            this.sbCheque.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sbCheque.Dock = System.Windows.Forms.DockStyle.None;
            this.sbCheque.Location = new System.Drawing.Point(15, 486);
            this.sbCheque.Margin = new System.Windows.Forms.Padding(4);
            this.sbCheque.Name = "sbCheque";
            this.sbCheque.Size = new System.Drawing.Size(373, 24);
            this.sbCheque.TabIndex = 11;
            // 
            // dgvCheque
            // 
            this.dgvCheque.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvCheque.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvCheque.Location = new System.Drawing.Point(13, 23);
            this.dgvCheque.Name = "dgvCheque";
            this.dgvCheque.RowHeadersWidth = 51;
            this.dgvCheque.RowTemplate.Height = 24;
            this.dgvCheque.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvCheque.Size = new System.Drawing.Size(1198, 447);
            this.dgvCheque.TabIndex = 0;
            // 
            // tpChequesFilho
            // 
            this.tpChequesFilho.Controls.Add(this.panel2);
            this.tpChequesFilho.Location = new System.Drawing.Point(4, 25);
            this.tpChequesFilho.Name = "tpChequesFilho";
            this.tpChequesFilho.Padding = new System.Windows.Forms.Padding(3);
            this.tpChequesFilho.Size = new System.Drawing.Size(1238, 522);
            this.tpChequesFilho.TabIndex = 1;
            this.tpChequesFilho.Text = "Detalhes";
            this.tpChequesFilho.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.dgvChequesFilho);
            this.panel2.Location = new System.Drawing.Point(45, 29);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1037, 354);
            this.panel2.TabIndex = 3;
            // 
            // dgvChequesFilho
            // 
            this.dgvChequesFilho.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvChequesFilho.Location = new System.Drawing.Point(17, 11);
            this.dgvChequesFilho.Name = "dgvChequesFilho";
            this.dgvChequesFilho.RowHeadersWidth = 51;
            this.dgvChequesFilho.RowTemplate.Height = 24;
            this.dgvChequesFilho.Size = new System.Drawing.Size(995, 324);
            this.dgvChequesFilho.TabIndex = 15;
            // 
            // printDialog2
            // 
            this.printDialog2.UseEXDialog = true;
            // 
            // tabControl2
            // 
            this.tabControl2.Controls.Add(this.tpRecibos);
            this.tabControl2.Controls.Add(this.tpCheques);
            this.tabControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tabControl2.Location = new System.Drawing.Point(0, 557);
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(1246, 96);
            this.tabControl2.TabIndex = 6;
            // 
            // tpRecibos
            // 
            this.tpRecibos.Controls.Add(this.btnRecibo);
            this.tpRecibos.Controls.Add(this.gbImprime);
            this.tpRecibos.Location = new System.Drawing.Point(4, 25);
            this.tpRecibos.Name = "tpRecibos";
            this.tpRecibos.Padding = new System.Windows.Forms.Padding(3);
            this.tpRecibos.Size = new System.Drawing.Size(1238, 67);
            this.tpRecibos.TabIndex = 0;
            this.tpRecibos.Text = "Recibos e Vauches";
            this.tpRecibos.UseVisualStyleBackColor = true;
            // 
            // btnRecibo
            // 
            this.btnRecibo.Location = new System.Drawing.Point(557, 18);
            this.btnRecibo.Name = "btnRecibo";
            this.btnRecibo.Size = new System.Drawing.Size(118, 38);
            this.btnRecibo.TabIndex = 8;
            this.btnRecibo.Text = "&Imprime";
            this.btnRecibo.UseVisualStyleBackColor = true;
            this.btnRecibo.Click += new System.EventHandler(this.btnRecibo_Click);
            // 
            // gbImprime
            // 
            this.gbImprime.Controls.Add(this.rbVauches);
            this.gbImprime.Controls.Add(this.rbRecibos);
            this.gbImprime.Location = new System.Drawing.Point(24, 13);
            this.gbImprime.Name = "gbImprime";
            this.gbImprime.Size = new System.Drawing.Size(461, 46);
            this.gbImprime.TabIndex = 7;
            this.gbImprime.TabStop = false;
            this.gbImprime.Text = "Tipos";
            // 
            // rbVauches
            // 
            this.rbVauches.AutoSize = true;
            this.rbVauches.Location = new System.Drawing.Point(235, 17);
            this.rbVauches.Name = "rbVauches";
            this.rbVauches.Size = new System.Drawing.Size(84, 21);
            this.rbVauches.TabIndex = 1;
            this.rbVauches.Text = "Vauches";
            this.rbVauches.UseVisualStyleBackColor = true;
            // 
            // rbRecibos
            // 
            this.rbRecibos.AutoSize = true;
            this.rbRecibos.Checked = true;
            this.rbRecibos.Location = new System.Drawing.Point(69, 15);
            this.rbRecibos.Name = "rbRecibos";
            this.rbRecibos.Size = new System.Drawing.Size(80, 21);
            this.rbRecibos.TabIndex = 0;
            this.rbRecibos.TabStop = true;
            this.rbRecibos.Text = "Recibos";
            this.rbRecibos.UseVisualStyleBackColor = true;
            // 
            // tpCheques
            // 
            this.tpCheques.Controls.Add(this.groupBox1);
            this.tpCheques.Controls.Add(this.btnChequeUmaFolha);
            this.tpCheques.Controls.Add(this.btnCheque);
            this.tpCheques.Location = new System.Drawing.Point(4, 25);
            this.tpCheques.Name = "tpCheques";
            this.tpCheques.Padding = new System.Windows.Forms.Padding(3);
            this.tpCheques.Size = new System.Drawing.Size(1238, 67);
            this.tpCheques.TabIndex = 1;
            this.tpCheques.Text = "Cheques";
            this.tpCheques.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.btnGrave);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(403, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(585, 56);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Enumeração Cheques";
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(242, 25);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(132, 22);
            this.textBox2.TabIndex = 13;
            this.textBox2.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBox2.Validating += new System.ComponentModel.CancelEventHandler(this.textBox2_Validating);
            // 
            // textBox1
            // 
            this.textBox1.AcceptsTab = true;
            this.textBox1.Location = new System.Drawing.Point(60, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(117, 22);
            this.textBox1.TabIndex = 11;
            this.textBox1.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.textBox_KeyPress);
            this.textBox1.Validating += new System.ComponentModel.CancelEventHandler(this.textBox1_Validating);
            // 
            // btnGrave
            // 
            this.btnGrave.Location = new System.Drawing.Point(395, 18);
            this.btnGrave.Name = "btnGrave";
            this.btnGrave.Size = new System.Drawing.Size(184, 32);
            this.btnGrave.TabIndex = 10;
            this.btnGrave.Text = "Grave números cheques";
            this.btnGrave.UseVisualStyleBackColor = true;
            this.btnGrave.Click += new System.EventHandler(this.btnGrave_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 9;
            this.label2.Text = "Série:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(192, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 17);
            this.label1.TabIndex = 8;
            this.label1.Text = "limite:";
            // 
            // btnChequeUmaFolha
            // 
            this.btnChequeUmaFolha.Location = new System.Drawing.Point(207, 11);
            this.btnChequeUmaFolha.Name = "btnChequeUmaFolha";
            this.btnChequeUmaFolha.Size = new System.Drawing.Size(153, 45);
            this.btnChequeUmaFolha.TabIndex = 3;
            this.btnChequeUmaFolha.Text = "Imprime Cheques_&Folha";
            this.btnChequeUmaFolha.UseVisualStyleBackColor = true;
            this.btnChequeUmaFolha.Click += new System.EventHandler(this.btnChequeUmaFolha_Click);
            // 
            // btnCheque
            // 
            this.btnCheque.Location = new System.Drawing.Point(25, 11);
            this.btnCheque.Name = "btnCheque";
            this.btnCheque.Size = new System.Drawing.Size(153, 46);
            this.btnCheque.TabIndex = 2;
            this.btnCheque.Text = "Imprime &Cheques";
            this.btnCheque.UseVisualStyleBackColor = true;
            this.btnCheque.Click += new System.EventHandler(this.btnimprime_Click);
            // 
            // FrmCheques
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1246, 653);
            this.Controls.Add(this.tabControl2);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.Name = "FrmCheques";
            this.Text = "FrmCheques";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tpCheque.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvCheque)).EndInit();
            this.tpChequesFilho.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvChequesFilho)).EndInit();
            this.tabControl2.ResumeLayout(false);
            this.tpRecibos.ResumeLayout(false);
            this.gbImprime.ResumeLayout(false);
            this.gbImprime.PerformLayout();
            this.tpCheques.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem testeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fontesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem imprimaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previewToolStripMenuItem;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tpCheque;
        private System.Windows.Forms.TabPage tpChequesFilho;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView dgvChequesFilho;
        private System.Drawing.Printing.PrintDocument printDocument2;
        private System.Windows.Forms.PrintDialog printDialog2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.StatusBar sbCheque;
        private System.Windows.Forms.DataGridView dgvCheque;
        private System.Windows.Forms.TabControl tabControl2;
        private System.Windows.Forms.TabPage tpRecibos;
        private System.Windows.Forms.Button btnRecibo;
        private System.Windows.Forms.GroupBox gbImprime;
        private System.Windows.Forms.RadioButton rbVauches;
        private System.Windows.Forms.RadioButton rbRecibos;
        private System.Windows.Forms.TabPage tpCheques;
        private System.Windows.Forms.Button btnChequeUmaFolha;
        private System.Windows.Forms.Button btnCheque;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnGrave;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
    }
}