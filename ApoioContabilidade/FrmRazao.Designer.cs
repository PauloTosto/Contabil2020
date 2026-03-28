namespace ApoioContabilidade
{
    partial class FrmRazao
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
            this.pnGeral = new System.Windows.Forms.Panel();
            this.button2 = new System.Windows.Forms.Button();
            this.btnGravePtMovFin = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbReduzido = new System.Windows.Forms.RadioButton();
            this.rbContabilidade = new System.Windows.Forms.RadioButton();
            this.rbDesc2 = new System.Windows.Forms.RadioButton();
            this.rbNumconta = new System.Windows.Forms.RadioButton();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.sbRazao = new System.Windows.Forms.StatusBar();
            this.dgEntradas = new System.Windows.Forms.DataGridView();
            this.pnGeral.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).BeginInit();
            this.SuspendLayout();
            // 
            // pnGeral
            // 
            this.pnGeral.AutoScroll = true;
            this.pnGeral.Controls.Add(this.button2);
            this.pnGeral.Controls.Add(this.btnGravePtMovFin);
            this.pnGeral.Controls.Add(this.groupBox1);
            this.pnGeral.Controls.Add(this.btnConsulta);
            this.pnGeral.Controls.Add(this.sbRazao);
            this.pnGeral.Controls.Add(this.dgEntradas);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGeral.Location = new System.Drawing.Point(0, 0);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1542, 702);
            this.pnGeral.TabIndex = 0;
            // 
            // button2
            // 
            this.button2.AutoSize = true;
            this.button2.Location = new System.Drawing.Point(1214, 16);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(137, 27);
            this.button2.TabIndex = 11;
            this.button2.Text = "Mostre Erros";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // btnGravePtMovFin
            // 
            this.btnGravePtMovFin.AutoSize = true;
            this.btnGravePtMovFin.Enabled = false;
            this.btnGravePtMovFin.Location = new System.Drawing.Point(922, 17);
            this.btnGravePtMovFin.Name = "btnGravePtMovFin";
            this.btnGravePtMovFin.Size = new System.Drawing.Size(232, 27);
            this.btnGravePtMovFin.TabIndex = 10;
            this.btnGravePtMovFin.Text = "Grave no PTMOVFIN(ano)";
            this.btnGravePtMovFin.UseCompatibleTextRendering = true;
            this.btnGravePtMovFin.UseVisualStyleBackColor = true;
            this.btnGravePtMovFin.Click += new System.EventHandler(this.btnGravePtMovFin_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbReduzido);
            this.groupBox1.Controls.Add(this.rbContabilidade);
            this.groupBox1.Controls.Add(this.rbDesc2);
            this.groupBox1.Controls.Add(this.rbNumconta);
            this.groupBox1.Location = new System.Drawing.Point(214, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(620, 44);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exibição";
            // 
            // rbReduzido
            // 
            this.rbReduzido.AutoSize = true;
            this.rbReduzido.Location = new System.Drawing.Point(438, 14);
            this.rbReduzido.Name = "rbReduzido";
            this.rbReduzido.Size = new System.Drawing.Size(158, 21);
            this.rbReduzido.TabIndex = 3;
            this.rbReduzido.Text = "AlterData(Reduzido)";
            this.rbReduzido.UseVisualStyleBackColor = true;
            this.rbReduzido.Click += new System.EventHandler(this.rbReduzido_Click);
            // 
            // rbContabilidade
            // 
            this.rbContabilidade.AutoSize = true;
            this.rbContabilidade.Location = new System.Drawing.Point(258, 15);
            this.rbContabilidade.Name = "rbContabilidade";
            this.rbContabilidade.Size = new System.Drawing.Size(135, 21);
            this.rbContabilidade.TabIndex = 2;
            this.rbContabilidade.Text = "AlterData(Conta)";
            this.rbContabilidade.UseVisualStyleBackColor = true;
            this.rbContabilidade.CheckedChanged += new System.EventHandler(this.rbContabilidade_CheckedChanged);
            // 
            // rbDesc2
            // 
            this.rbDesc2.AutoSize = true;
            this.rbDesc2.Location = new System.Drawing.Point(146, 16);
            this.rbDesc2.Name = "rbDesc2";
            this.rbDesc2.Size = new System.Drawing.Size(74, 21);
            this.rbDesc2.TabIndex = 1;
            this.rbDesc2.Text = "DESC2";
            this.rbDesc2.UseVisualStyleBackColor = true;
            this.rbDesc2.CheckedChanged += new System.EventHandler(this.rbDesc2_CheckedChanged);
            // 
            // rbNumconta
            // 
            this.rbNumconta.AutoSize = true;
            this.rbNumconta.Checked = true;
            this.rbNumconta.Location = new System.Drawing.Point(25, 18);
            this.rbNumconta.Name = "rbNumconta";
            this.rbNumconta.Size = new System.Drawing.Size(93, 21);
            this.rbNumconta.TabIndex = 0;
            this.rbNumconta.TabStop = true;
            this.rbNumconta.Text = "Numconta";
            this.rbNumconta.UseVisualStyleBackColor = true;
            this.rbNumconta.CheckedChanged += new System.EventHandler(this.rbNumconta_CheckedChanged);
            // 
            // btnConsulta
            // 
            this.btnConsulta.CausesValidation = false;
            this.btnConsulta.Location = new System.Drawing.Point(26, 10);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(75, 23);
            this.btnConsulta.TabIndex = 8;
            this.btnConsulta.Text = "Consulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // sbRazao
            // 
            this.sbRazao.Dock = System.Windows.Forms.DockStyle.None;
            this.sbRazao.Location = new System.Drawing.Point(26, 662);
            this.sbRazao.Margin = new System.Windows.Forms.Padding(4);
            this.sbRazao.Name = "sbRazao";
            this.sbRazao.Size = new System.Drawing.Size(133, 27);
            this.sbRazao.TabIndex = 7;
            // 
            // dgEntradas
            // 
            this.dgEntradas.AllowUserToAddRows = false;
            this.dgEntradas.AllowUserToDeleteRows = false;
            this.dgEntradas.AllowUserToOrderColumns = true;
            this.dgEntradas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgEntradas.Location = new System.Drawing.Point(26, 60);
            this.dgEntradas.Name = "dgEntradas";
            this.dgEntradas.ReadOnly = true;
            this.dgEntradas.RowHeadersWidth = 51;
            this.dgEntradas.RowTemplate.Height = 24;
            this.dgEntradas.Size = new System.Drawing.Size(1503, 583);
            this.dgEntradas.TabIndex = 0;
            // 
            // FrmRazao
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1542, 702);
            this.Controls.Add(this.pnGeral);
            this.Name = "FrmRazao";
            this.Text = "FrmRazao";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnGeral.ResumeLayout(false);
            this.pnGeral.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.DataGridView dgEntradas;
        private System.Windows.Forms.StatusBar sbRazao;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbContabilidade;
        private System.Windows.Forms.RadioButton rbDesc2;
        private System.Windows.Forms.RadioButton rbNumconta;
        private System.Windows.Forms.Button btnGravePtMovFin;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.RadioButton rbReduzido;
    }
}