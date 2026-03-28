namespace ApoioContabilidade.ZContabAlterData
{
    partial class FrmBalancete
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
            this.btnSaldoAlter = new System.Windows.Forms.Button();
            this.btnRazaoGeral = new System.Windows.Forms.Button();
            this.btnRazao = new System.Windows.Forms.Button();
            this.ckSintetico = new System.Windows.Forms.CheckBox();
            this.cbExcluiFechamento = new System.Windows.Forms.CheckBox();
            this.btnSaldo = new System.Windows.Forms.Button();
            this.btnConsulte = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.cbMeses = new System.Windows.Forms.ComboBox();
            this.btnExcel = new System.Windows.Forms.Button();
            this.txNumconta = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.pnGeral = new System.Windows.Forms.Panel();
            this.sbBalancete = new System.Windows.Forms.StatusBar();
            this.dgvBalancete = new System.Windows.Forms.DataGridView();
            this.btnNovasContasPlacon = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            this.pnGeral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalancete)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.btnNovasContasPlacon);
            this.pnTop.Controls.Add(this.btnSaldoAlter);
            this.pnTop.Controls.Add(this.btnRazaoGeral);
            this.pnTop.Controls.Add(this.btnRazao);
            this.pnTop.Controls.Add(this.ckSintetico);
            this.pnTop.Controls.Add(this.cbExcluiFechamento);
            this.pnTop.Controls.Add(this.btnSaldo);
            this.pnTop.Controls.Add(this.btnConsulte);
            this.pnTop.Controls.Add(this.label2);
            this.pnTop.Controls.Add(this.cbMeses);
            this.pnTop.Controls.Add(this.btnExcel);
            this.pnTop.Controls.Add(this.txNumconta);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1589, 48);
            this.pnTop.TabIndex = 0;
            // 
            // btnSaldoAlter
            // 
            this.btnSaldoAlter.Location = new System.Drawing.Point(1337, 13);
            this.btnSaldoAlter.Name = "btnSaldoAlter";
            this.btnSaldoAlter.Size = new System.Drawing.Size(62, 23);
            this.btnSaldoAlter.TabIndex = 13;
            this.btnSaldoAlter.Text = "Sdo AlterData";
            this.btnSaldoAlter.UseVisualStyleBackColor = true;
            this.btnSaldoAlter.Click += new System.EventHandler(this.btnSaldoAlter_Click);
            // 
            // btnRazaoGeral
            // 
            this.btnRazaoGeral.Location = new System.Drawing.Point(1245, 14);
            this.btnRazaoGeral.Name = "btnRazaoGeral";
            this.btnRazaoGeral.Size = new System.Drawing.Size(75, 23);
            this.btnRazaoGeral.TabIndex = 12;
            this.btnRazaoGeral.Text = "&RazãoGeral";
            this.btnRazaoGeral.UseVisualStyleBackColor = true;
            this.btnRazaoGeral.Click += new System.EventHandler(this.btnRazaoGeral_Click);
            // 
            // btnRazao
            // 
            this.btnRazao.Location = new System.Drawing.Point(1153, 13);
            this.btnRazao.Name = "btnRazao";
            this.btnRazao.Size = new System.Drawing.Size(75, 23);
            this.btnRazao.TabIndex = 11;
            this.btnRazao.Text = "&Razão";
            this.btnRazao.UseVisualStyleBackColor = true;
            this.btnRazao.Click += new System.EventHandler(this.btnRazao_Click);
            // 
            // ckSintetico
            // 
            this.ckSintetico.AutoSize = true;
            this.ckSintetico.Location = new System.Drawing.Point(672, 14);
            this.ckSintetico.Name = "ckSintetico";
            this.ckSintetico.Size = new System.Drawing.Size(80, 20);
            this.ckSintetico.TabIndex = 10;
            this.ckSintetico.Text = "Sintetico";
            this.ckSintetico.UseVisualStyleBackColor = true;
            this.ckSintetico.CheckedChanged += new System.EventHandler(this.ckSintetico_CheckedChanged);
            // 
            // cbExcluiFechamento
            // 
            this.cbExcluiFechamento.AutoSize = true;
            this.cbExcluiFechamento.Location = new System.Drawing.Point(513, 13);
            this.cbExcluiFechamento.Name = "cbExcluiFechamento";
            this.cbExcluiFechamento.Size = new System.Drawing.Size(142, 20);
            this.cbExcluiFechamento.TabIndex = 9;
            this.cbExcluiFechamento.Text = "Exclui Fechamento";
            this.cbExcluiFechamento.UseVisualStyleBackColor = true;
            this.cbExcluiFechamento.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // btnSaldo
            // 
            this.btnSaldo.Location = new System.Drawing.Point(943, 13);
            this.btnSaldo.Name = "btnSaldo";
            this.btnSaldo.Size = new System.Drawing.Size(196, 23);
            this.btnSaldo.TabIndex = 6;
            this.btnSaldo.Text = "&Atualize Sdo PTPLA<ano>";
            this.btnSaldo.UseVisualStyleBackColor = true;
            this.btnSaldo.Click += new System.EventHandler(this.btnSaldo_Click);
            // 
            // btnConsulte
            // 
            this.btnConsulte.Location = new System.Drawing.Point(768, 13);
            this.btnConsulte.Name = "btnConsulte";
            this.btnConsulte.Size = new System.Drawing.Size(75, 23);
            this.btnConsulte.TabIndex = 5;
            this.btnConsulte.Text = "&Consulte";
            this.btnConsulte.UseVisualStyleBackColor = true;
            this.btnConsulte.Click += new System.EventHandler(this.btnConsulte_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(293, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 16);
            this.label2.TabIndex = 4;
            this.label2.Text = "Periodo";
            // 
            // cbMeses
            // 
            this.cbMeses.FormattingEnabled = true;
            this.cbMeses.Location = new System.Drawing.Point(353, 9);
            this.cbMeses.Name = "cbMeses";
            this.cbMeses.Size = new System.Drawing.Size(121, 24);
            this.cbMeses.TabIndex = 3;
            // 
            // btnExcel
            // 
            this.btnExcel.Location = new System.Drawing.Point(852, 14);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(75, 23);
            this.btnExcel.TabIndex = 2;
            this.btnExcel.Text = "&Excel";
            this.btnExcel.UseVisualStyleBackColor = true;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // txNumconta
            // 
            this.txNumconta.Location = new System.Drawing.Point(121, 8);
            this.txNumconta.Name = "txNumconta";
            this.txNumconta.Size = new System.Drawing.Size(150, 22);
            this.txNumconta.TabIndex = 1;
            this.txNumconta.TextChanged += new System.EventHandler(this.txNumconta_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Filtre NumConta:";
            // 
            // pnGeral
            // 
            this.pnGeral.Controls.Add(this.sbBalancete);
            this.pnGeral.Controls.Add(this.dgvBalancete);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGeral.Location = new System.Drawing.Point(0, 48);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1589, 659);
            this.pnGeral.TabIndex = 1;
            // 
            // sbBalancete
            // 
            this.sbBalancete.Dock = System.Windows.Forms.DockStyle.None;
            this.sbBalancete.Location = new System.Drawing.Point(22, 619);
            this.sbBalancete.Margin = new System.Windows.Forms.Padding(4);
            this.sbBalancete.Name = "sbBalancete";
            this.sbBalancete.Size = new System.Drawing.Size(133, 27);
            this.sbBalancete.SizingGrip = false;
            this.sbBalancete.TabIndex = 8;
            // 
            // dgvBalancete
            // 
            this.dgvBalancete.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBalancete.Dock = System.Windows.Forms.DockStyle.Top;
            this.dgvBalancete.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvBalancete.Location = new System.Drawing.Point(0, 0);
            this.dgvBalancete.Name = "dgvBalancete";
            this.dgvBalancete.RowHeadersWidth = 51;
            this.dgvBalancete.RowTemplate.Height = 24;
            this.dgvBalancete.Size = new System.Drawing.Size(1589, 612);
            this.dgvBalancete.TabIndex = 0;
            // 
            // btnNovasContasPlacon
            // 
            this.btnNovasContasPlacon.Location = new System.Drawing.Point(1413, 11);
            this.btnNovasContasPlacon.Name = "btnNovasContasPlacon";
            this.btnNovasContasPlacon.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.btnNovasContasPlacon.Size = new System.Drawing.Size(148, 23);
            this.btnNovasContasPlacon.TabIndex = 14;
            this.btnNovasContasPlacon.Text = "Novas Contas Placon";
            this.btnNovasContasPlacon.UseVisualStyleBackColor = true;
            this.btnNovasContasPlacon.Click += new System.EventHandler(this.btnNovasContasPlacon_Click);
            // 
            // FrmBalancete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1589, 707);
            this.Controls.Add(this.pnGeral);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmBalancete";
            this.Text = "FrmBalancete";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.pnGeral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBalancete)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.DataGridView dgvBalancete;
        private System.Windows.Forms.StatusBar sbBalancete;
        private System.Windows.Forms.TextBox txNumconta;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.ComboBox cbMeses;
        private System.Windows.Forms.Button btnConsulte;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnSaldo;
        private System.Windows.Forms.CheckBox ckSintetico;
        private System.Windows.Forms.CheckBox cbExcluiFechamento;
        private System.Windows.Forms.Button btnRazao;
        private System.Windows.Forms.Button btnRazaoGeral;
        private System.Windows.Forms.Button btnSaldoAlter;
        private System.Windows.Forms.Button btnNovasContasPlacon;
    }
}