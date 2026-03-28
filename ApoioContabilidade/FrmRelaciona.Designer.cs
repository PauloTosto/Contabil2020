namespace ApoioContabilidade
{
    partial class FrmRelaciona
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
            this.sbRazao = new System.Windows.Forms.StatusBar();
            this.pnGeral = new System.Windows.Forms.Panel();
            this.dgEntradas = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pnTop = new System.Windows.Forms.Panel();
            this.btBalancete = new System.Windows.Forms.Button();
            this.chkExcluiDebCre = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbContabilidade = new System.Windows.Forms.RadioButton();
            this.rbDesc2 = new System.Windows.Forms.RadioButton();
            this.rbNumconta = new System.Windows.Forms.RadioButton();
            this.btnPesquisa = new System.Windows.Forms.Button();
            this.btnBalancinho = new System.Windows.Forms.Button();
            this.btnUnicas = new System.Windows.Forms.Button();
            this.btnImporta = new System.Windows.Forms.Button();
            this.btnExportaLote = new System.Windows.Forms.Button();
            this.btnFechamento = new System.Windows.Forms.Button();
            this.pnGeral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).BeginInit();
            this.pnTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // sbRazao
            // 
            this.sbRazao.Dock = System.Windows.Forms.DockStyle.None;
            this.sbRazao.Location = new System.Drawing.Point(19, 567);
            this.sbRazao.Margin = new System.Windows.Forms.Padding(4);
            this.sbRazao.Name = "sbRazao";
            this.sbRazao.Size = new System.Drawing.Size(152, 28);
            this.sbRazao.TabIndex = 9;
            // 
            // pnGeral
            // 
            this.pnGeral.AutoScroll = true;
            this.pnGeral.Controls.Add(this.dgEntradas);
            this.pnGeral.Controls.Add(this.sbRazao);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGeral.Location = new System.Drawing.Point(0, 0);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1568, 617);
            this.pnGeral.TabIndex = 12;
            // 
            // dgEntradas
            // 
            this.dgEntradas.AllowUserToAddRows = false;
            this.dgEntradas.AllowUserToDeleteRows = false;
            this.dgEntradas.AllowUserToOrderColumns = true;
            this.dgEntradas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgEntradas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgEntradas.Location = new System.Drawing.Point(12, 87);
            this.dgEntradas.Name = "dgEntradas";
            this.dgEntradas.ReadOnly = true;
            this.dgEntradas.RowHeadersWidth = 51;
            this.dgEntradas.RowTemplate.Height = 24;
            this.dgEntradas.Size = new System.Drawing.Size(1519, 467);
            this.dgEntradas.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.btnFechamento);
            this.pnTop.Controls.Add(this.btBalancete);
            this.pnTop.Controls.Add(this.chkExcluiDebCre);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.comboBox1);
            this.pnTop.Controls.Add(this.groupBox1);
            this.pnTop.Controls.Add(this.btnPesquisa);
            this.pnTop.Controls.Add(this.btnBalancinho);
            this.pnTop.Controls.Add(this.btnUnicas);
            this.pnTop.Controls.Add(this.btnImporta);
            this.pnTop.Controls.Add(this.btnExportaLote);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1568, 79);
            this.pnTop.TabIndex = 13;
            // 
            // btBalancete
            // 
            this.btBalancete.Location = new System.Drawing.Point(1069, 50);
            this.btBalancete.Name = "btBalancete";
            this.btBalancete.Size = new System.Drawing.Size(134, 24);
            this.btBalancete.TabIndex = 25;
            this.btBalancete.Text = "Balancete";
            this.btBalancete.UseVisualStyleBackColor = true;
            this.btBalancete.Click += new System.EventHandler(this.btBalancete_Click);
            // 
            // chkExcluiDebCre
            // 
            this.chkExcluiDebCre.AutoSize = true;
            this.chkExcluiDebCre.Checked = true;
            this.chkExcluiDebCre.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkExcluiDebCre.Location = new System.Drawing.Point(160, 47);
            this.chkExcluiDebCre.Name = "chkExcluiDebCre";
            this.chkExcluiDebCre.Size = new System.Drawing.Size(326, 20);
            this.chkExcluiDebCre.TabIndex = 24;
            this.chkExcluiDebCre.Text = "(Debito = Credito no mesmo lançamento) Excluido";
            this.chkExcluiDebCre.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 16);
            this.label1.TabIndex = 23;
            this.label1.Text = "Escolha Ano";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(20, 31);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 24);
            this.comboBox1.TabIndex = 19;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbContabilidade);
            this.groupBox1.Controls.Add(this.rbDesc2);
            this.groupBox1.Controls.Add(this.rbNumconta);
            this.groupBox1.Location = new System.Drawing.Point(569, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(408, 44);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Exibição";
            // 
            // rbContabilidade
            // 
            this.rbContabilidade.AutoSize = true;
            this.rbContabilidade.Location = new System.Drawing.Point(248, 17);
            this.rbContabilidade.Name = "rbContabilidade";
            this.rbContabilidade.Size = new System.Drawing.Size(130, 20);
            this.rbContabilidade.TabIndex = 2;
            this.rbContabilidade.Text = "Contab AlterData";
            this.rbContabilidade.UseVisualStyleBackColor = true;
            this.rbContabilidade.Click += new System.EventHandler(this.rbContabilidade_Click);
            // 
            // rbDesc2
            // 
            this.rbDesc2.AutoSize = true;
            this.rbDesc2.Location = new System.Drawing.Point(134, 17);
            this.rbDesc2.Name = "rbDesc2";
            this.rbDesc2.Size = new System.Drawing.Size(86, 20);
            this.rbDesc2.TabIndex = 1;
            this.rbDesc2.Text = "Reduzido";
            this.rbDesc2.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            this.rbDesc2.UseVisualStyleBackColor = true;
            this.rbDesc2.Click += new System.EventHandler(this.rbDesc2_Click);
            // 
            // rbNumconta
            // 
            this.rbNumconta.AutoSize = true;
            this.rbNumconta.Checked = true;
            this.rbNumconta.Location = new System.Drawing.Point(25, 18);
            this.rbNumconta.Name = "rbNumconta";
            this.rbNumconta.Size = new System.Drawing.Size(89, 20);
            this.rbNumconta.TabIndex = 0;
            this.rbNumconta.TabStop = true;
            this.rbNumconta.Text = "Numconta";
            this.rbNumconta.UseVisualStyleBackColor = true;
            this.rbNumconta.Click += new System.EventHandler(this.rbNumconta_Click);
            // 
            // btnPesquisa
            // 
            this.btnPesquisa.CausesValidation = false;
            this.btnPesquisa.Enabled = false;
            this.btnPesquisa.Location = new System.Drawing.Point(156, 9);
            this.btnPesquisa.Name = "btnPesquisa";
            this.btnPesquisa.Size = new System.Drawing.Size(121, 31);
            this.btnPesquisa.TabIndex = 16;
            this.btnPesquisa.Text = "Consulta";
            this.btnPesquisa.UseVisualStyleBackColor = true;
            this.btnPesquisa.Click += new System.EventHandler(this.button3_Click);
            // 
            // btnBalancinho
            // 
            this.btnBalancinho.Location = new System.Drawing.Point(1066, 5);
            this.btnBalancinho.Name = "btnBalancinho";
            this.btnBalancinho.Size = new System.Drawing.Size(134, 24);
            this.btnBalancinho.TabIndex = 22;
            this.btnBalancinho.Text = "Balanço Contas";
            this.btnBalancinho.UseVisualStyleBackColor = true;
            this.btnBalancinho.Click += new System.EventHandler(this.btnBalancinho_Click);
            // 
            // btnUnicas
            // 
            this.btnUnicas.Location = new System.Drawing.Point(1065, 28);
            this.btnUnicas.Name = "btnUnicas";
            this.btnUnicas.Size = new System.Drawing.Size(140, 23);
            this.btnUnicas.TabIndex = 21;
            this.btnUnicas.Text = "Contas \"Solteiras\"";
            this.btnUnicas.UseVisualStyleBackColor = true;
            this.btnUnicas.Click += new System.EventHandler(this.btnContasUnicas_Click_1);
            // 
            // btnImporta
            // 
            this.btnImporta.Location = new System.Drawing.Point(1249, 3);
            this.btnImporta.Name = "btnImporta";
            this.btnImporta.Size = new System.Drawing.Size(216, 28);
            this.btnImporta.TabIndex = 20;
            this.btnImporta.Text = "Importa Excel (RELACIONA)";
            this.btnImporta.UseVisualStyleBackColor = true;
            this.btnImporta.Click += new System.EventHandler(this.btnImporta_Click);
            // 
            // btnExportaLote
            // 
            this.btnExportaLote.Enabled = false;
            this.btnExportaLote.Location = new System.Drawing.Point(1252, 27);
            this.btnExportaLote.Name = "btnExportaLote";
            this.btnExportaLote.Size = new System.Drawing.Size(215, 27);
            this.btnExportaLote.TabIndex = 18;
            this.btnExportaLote.Text = "Exporta Lote AlterData";
            this.btnExportaLote.UseVisualStyleBackColor = true;
            this.btnExportaLote.Click += new System.EventHandler(this.btnExporta_Click);
            // 
            // btnFechamento
            // 
            this.btnFechamento.Location = new System.Drawing.Point(1254, 51);
            this.btnFechamento.Name = "btnFechamento";
            this.btnFechamento.Size = new System.Drawing.Size(210, 24);
            this.btnFechamento.TabIndex = 26;
            this.btnFechamento.Text = "Fechamento Balanço";
            this.btnFechamento.UseVisualStyleBackColor = true;
            this.btnFechamento.Click += new System.EventHandler(this.btnFechamento_Click);
            // 
            // FrmRelaciona
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1568, 617);
            this.Controls.Add(this.pnTop);
            this.Controls.Add(this.pnGeral);
            this.Name = "FrmRelaciona";
            this.Text = "FrmRelaciona";
            this.pnGeral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).EndInit();
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        
        private System.Windows.Forms.StatusBar sbRazao;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.DataGridView dgEntradas;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbContabilidade;
        private System.Windows.Forms.RadioButton rbDesc2;
        private System.Windows.Forms.RadioButton rbNumconta;
        private System.Windows.Forms.Button btnPesquisa;
        private System.Windows.Forms.Button btnBalancinho;
        private System.Windows.Forms.Button btnUnicas;
        private System.Windows.Forms.Button btnImporta;
        private System.Windows.Forms.Button btnExportaLote;
        private System.Windows.Forms.CheckBox chkExcluiDebCre;
        private System.Windows.Forms.Button btBalancete;
        private System.Windows.Forms.Button btnFechamento;
    }
}