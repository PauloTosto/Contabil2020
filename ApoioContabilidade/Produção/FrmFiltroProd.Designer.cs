
using ClassFiltroEdite;

namespace ApoioContabilidade.Produção
{
    partial class FrmFiltroProd
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
            this.lbColheita = new System.Windows.Forms.Label();
            this.upAnos = new System.Windows.Forms.NumericUpDown();
            this.lbSafra = new System.Windows.Forms.Label();
            this.lbProduto = new System.Windows.Forms.Label();
            this.lbContrato = new System.Windows.Forms.Label();
            this.txContratos = new System.Windows.Forms.TextBox();
            this.gbVendas = new System.Windows.Forms.GroupBox();
            this.rbNaoVendidos = new System.Windows.Forms.RadioButton();
            this.rbVendidos = new System.Windows.Forms.RadioButton();
            this.rbLotesSemFiltro = new System.Windows.Forms.RadioButton();
            this.gbArmazenamento = new System.Windows.Forms.GroupBox();
            this.rbDepositoCentral = new System.Windows.Forms.RadioButton();
            this.rbTriagem = new System.Windows.Forms.RadioButton();
            this.rbArmazFazenda = new System.Windows.Forms.RadioButton();
            this.rbArmazSemFiltro = new System.Windows.Forms.RadioButton();
            this.cbTipoProd = new System.Windows.Forms.ComboBox();
            this.cbSafra = new System.Windows.Forms.ComboBox();
            this.cbProdutos = new System.Windows.Forms.ComboBox();
            this.btnCancele = new System.Windows.Forms.Button();
            this.gbEstagios = new System.Windows.Forms.GroupBox();
            this.rbCatagem = new System.Windows.Forms.RadioButton();
            this.rbAprove = new System.Windows.Forms.RadioButton();
            this.rbDeposito = new System.Windows.Forms.RadioButton();
            this.rbEstagFazenda = new System.Windows.Forms.RadioButton();
            this.rbCocho = new System.Windows.Forms.RadioButton();
            this.rbEstagSemFiltro = new System.Windows.Forms.RadioButton();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnFiltro = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upAnos)).BeginInit();
            this.gbVendas.SuspendLayout();
            this.gbArmazenamento.SuspendLayout();
            this.gbEstagios.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.lbColheita);
            this.pnTop.Controls.Add(this.upAnos);
            this.pnTop.Controls.Add(this.lbSafra);
            this.pnTop.Controls.Add(this.lbProduto);
            this.pnTop.Controls.Add(this.lbContrato);
            this.pnTop.Controls.Add(this.txContratos);
            this.pnTop.Controls.Add(this.gbVendas);
            this.pnTop.Controls.Add(this.gbArmazenamento);
            this.pnTop.Controls.Add(this.cbTipoProd);
            this.pnTop.Controls.Add(this.cbSafra);
            this.pnTop.Controls.Add(this.cbProdutos);
            this.pnTop.Controls.Add(this.btnCancele);
            this.pnTop.Controls.Add(this.gbEstagios);
            this.pnTop.Controls.Add(this.btnOk);
            this.pnTop.Controls.Add(this.btnFiltro);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(674, 390);
            this.pnTop.TabIndex = 1;
            // 
            // lbColheita
            // 
            this.lbColheita.AutoSize = true;
            this.lbColheita.Location = new System.Drawing.Point(31, 139);
            this.lbColheita.Name = "lbColheita";
            this.lbColheita.Size = new System.Drawing.Size(139, 17);
            this.lbColheita.TabIndex = 15;
            this.lbColheita.Text = "Colheita Cacau Tipo:";
            // 
            // upAnos
            // 
            this.upAnos.Location = new System.Drawing.Point(158, 101);
            this.upAnos.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.upAnos.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.upAnos.Name = "upAnos";
            this.upAnos.ReadOnly = true;
            this.upAnos.Size = new System.Drawing.Size(38, 22);
            this.upAnos.TabIndex = 14;
            this.upAnos.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // lbSafra
            // 
            this.lbSafra.AutoSize = true;
            this.lbSafra.Location = new System.Drawing.Point(29, 78);
            this.lbSafra.Name = "lbSafra";
            this.lbSafra.Size = new System.Drawing.Size(46, 17);
            this.lbSafra.TabIndex = 13;
            this.lbSafra.Text = "Safra:";
            // 
            // lbProduto
            // 
            this.lbProduto.AutoSize = true;
            this.lbProduto.Location = new System.Drawing.Point(25, 7);
            this.lbProduto.Name = "lbProduto";
            this.lbProduto.Size = new System.Drawing.Size(62, 17);
            this.lbProduto.TabIndex = 12;
            this.lbProduto.Text = "Produto:";
            // 
            // lbContrato
            // 
            this.lbContrato.AutoSize = true;
            this.lbContrato.Location = new System.Drawing.Point(276, 343);
            this.lbContrato.Name = "lbContrato";
            this.lbContrato.Size = new System.Drawing.Size(161, 17);
            this.lbContrato.TabIndex = 11;
            this.lbContrato.Text = "Contrato(safra-número):";
            // 
            // txContratos
            // 
            this.txContratos.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txContratos.Location = new System.Drawing.Point(440, 342);
            this.txContratos.Name = "txContratos";
            this.txContratos.Size = new System.Drawing.Size(188, 22);
            this.txContratos.TabIndex = 10;
            // 
            // gbVendas
            // 
            this.gbVendas.Controls.Add(this.rbNaoVendidos);
            this.gbVendas.Controls.Add(this.rbVendidos);
            this.gbVendas.Controls.Add(this.rbLotesSemFiltro);
            this.gbVendas.Location = new System.Drawing.Point(277, 255);
            this.gbVendas.Name = "gbVendas";
            this.gbVendas.Size = new System.Drawing.Size(384, 63);
            this.gbVendas.TabIndex = 9;
            this.gbVendas.TabStop = false;
            this.gbVendas.Text = "&Lotes Vendas";
            // 
            // rbNaoVendidos
            // 
            this.rbNaoVendidos.AutoSize = true;
            this.rbNaoVendidos.Location = new System.Drawing.Point(237, 22);
            this.rbNaoVendidos.Name = "rbNaoVendidos";
            this.rbNaoVendidos.Size = new System.Drawing.Size(118, 21);
            this.rbNaoVendidos.TabIndex = 3;
            this.rbNaoVendidos.Text = "Não Vendidos";
            this.rbNaoVendidos.UseVisualStyleBackColor = true;
            this.rbNaoVendidos.Click += new System.EventHandler(this.rbVendidos_Click);
            // 
            // rbVendidos
            // 
            this.rbVendidos.AutoSize = true;
            this.rbVendidos.Location = new System.Drawing.Point(120, 21);
            this.rbVendidos.Name = "rbVendidos";
            this.rbVendidos.Size = new System.Drawing.Size(88, 21);
            this.rbVendidos.TabIndex = 2;
            this.rbVendidos.Text = "Vendidos";
            this.rbVendidos.UseVisualStyleBackColor = true;
            this.rbVendidos.Click += new System.EventHandler(this.rbVendidos_Click);
            // 
            // rbLotesSemFiltro
            // 
            this.rbLotesSemFiltro.AutoSize = true;
            this.rbLotesSemFiltro.Checked = true;
            this.rbLotesSemFiltro.Location = new System.Drawing.Point(11, 23);
            this.rbLotesSemFiltro.Name = "rbLotesSemFiltro";
            this.rbLotesSemFiltro.Size = new System.Drawing.Size(92, 21);
            this.rbLotesSemFiltro.TabIndex = 1;
            this.rbLotesSemFiltro.TabStop = true;
            this.rbLotesSemFiltro.Text = "Sem Filtro";
            this.rbLotesSemFiltro.UseVisualStyleBackColor = true;
            this.rbLotesSemFiltro.Click += new System.EventHandler(this.rbVendidos_Click);
            // 
            // gbArmazenamento
            // 
            this.gbArmazenamento.Controls.Add(this.rbDepositoCentral);
            this.gbArmazenamento.Controls.Add(this.rbTriagem);
            this.gbArmazenamento.Controls.Add(this.rbArmazFazenda);
            this.gbArmazenamento.Controls.Add(this.rbArmazSemFiltro);
            this.gbArmazenamento.Location = new System.Drawing.Point(473, 27);
            this.gbArmazenamento.Name = "gbArmazenamento";
            this.gbArmazenamento.Size = new System.Drawing.Size(184, 210);
            this.gbArmazenamento.TabIndex = 8;
            this.gbArmazenamento.TabStop = false;
            this.gbArmazenamento.Text = "&Armazenamento Bulk";
            // 
            // rbDepositoCentral
            // 
            this.rbDepositoCentral.AutoSize = true;
            this.rbDepositoCentral.Location = new System.Drawing.Point(20, 116);
            this.rbDepositoCentral.Name = "rbDepositoCentral";
            this.rbDepositoCentral.Size = new System.Drawing.Size(134, 21);
            this.rbDepositoCentral.TabIndex = 3;
            this.rbDepositoCentral.Text = "Depósito Central";
            this.rbDepositoCentral.UseVisualStyleBackColor = true;
            // 
            // rbTriagem
            // 
            this.rbTriagem.AutoSize = true;
            this.rbTriagem.Location = new System.Drawing.Point(18, 88);
            this.rbTriagem.Name = "rbTriagem";
            this.rbTriagem.Size = new System.Drawing.Size(81, 21);
            this.rbTriagem.TabIndex = 2;
            this.rbTriagem.Text = "Triagem";
            this.rbTriagem.UseVisualStyleBackColor = true;
            // 
            // rbArmazFazenda
            // 
            this.rbArmazFazenda.AutoSize = true;
            this.rbArmazFazenda.Location = new System.Drawing.Point(18, 59);
            this.rbArmazFazenda.Name = "rbArmazFazenda";
            this.rbArmazFazenda.Size = new System.Drawing.Size(84, 21);
            this.rbArmazFazenda.TabIndex = 1;
            this.rbArmazFazenda.Text = "Fazenda";
            this.rbArmazFazenda.UseVisualStyleBackColor = true;
            // 
            // rbArmazSemFiltro
            // 
            this.rbArmazSemFiltro.AutoSize = true;
            this.rbArmazSemFiltro.Checked = true;
            this.rbArmazSemFiltro.Location = new System.Drawing.Point(17, 29);
            this.rbArmazSemFiltro.Name = "rbArmazSemFiltro";
            this.rbArmazSemFiltro.Size = new System.Drawing.Size(92, 21);
            this.rbArmazSemFiltro.TabIndex = 0;
            this.rbArmazSemFiltro.TabStop = true;
            this.rbArmazSemFiltro.Text = "Sem Filtro";
            this.rbArmazSemFiltro.UseVisualStyleBackColor = true;
            // 
            // cbTipoProd
            // 
            this.cbTipoProd.FormattingEnabled = true;
            this.cbTipoProd.Location = new System.Drawing.Point(29, 161);
            this.cbTipoProd.Name = "cbTipoProd";
            this.cbTipoProd.Size = new System.Drawing.Size(230, 24);
            this.cbTipoProd.TabIndex = 7;
            // 
            // cbSafra
            // 
            this.cbSafra.FormattingEnabled = true;
            this.cbSafra.Location = new System.Drawing.Point(27, 101);
            this.cbSafra.Name = "cbSafra";
            this.cbSafra.Size = new System.Drawing.Size(113, 24);
            this.cbSafra.TabIndex = 6;
            // 
            // cbProdutos
            // 
            this.cbProdutos.FormattingEnabled = true;
            this.cbProdutos.Location = new System.Drawing.Point(25, 30);
            this.cbProdutos.Name = "cbProdutos";
            this.cbProdutos.Size = new System.Drawing.Size(231, 24);
            this.cbProdutos.TabIndex = 5;
            // 
            // btnCancele
            // 
            this.btnCancele.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancele.Location = new System.Drawing.Point(139, 254);
            this.btnCancele.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancele.Name = "btnCancele";
            this.btnCancele.Size = new System.Drawing.Size(93, 28);
            this.btnCancele.TabIndex = 3;
            this.btnCancele.Text = "&Cancele";
            this.btnCancele.UseVisualStyleBackColor = true;
            this.btnCancele.Click += new System.EventHandler(this.btnCancele_Click);
            // 
            // gbEstagios
            // 
            this.gbEstagios.Controls.Add(this.rbCatagem);
            this.gbEstagios.Controls.Add(this.rbAprove);
            this.gbEstagios.Controls.Add(this.rbDeposito);
            this.gbEstagios.Controls.Add(this.rbEstagFazenda);
            this.gbEstagios.Controls.Add(this.rbCocho);
            this.gbEstagios.Controls.Add(this.rbEstagSemFiltro);
            this.gbEstagios.Location = new System.Drawing.Point(275, 17);
            this.gbEstagios.Name = "gbEstagios";
            this.gbEstagios.Size = new System.Drawing.Size(184, 225);
            this.gbEstagios.TabIndex = 4;
            this.gbEstagios.TabStop = false;
            this.gbEstagios.Text = "&Estágios";
            // 
            // rbCatagem
            // 
            this.rbCatagem.AutoSize = true;
            this.rbCatagem.Location = new System.Drawing.Point(21, 176);
            this.rbCatagem.Name = "rbCatagem";
            this.rbCatagem.Size = new System.Drawing.Size(85, 21);
            this.rbCatagem.TabIndex = 5;
            this.rbCatagem.Text = "Catagem";
            this.rbCatagem.UseVisualStyleBackColor = true;
            // 
            // rbAprove
            // 
            this.rbAprove.AutoSize = true;
            this.rbAprove.Location = new System.Drawing.Point(20, 145);
            this.rbAprove.Name = "rbAprove";
            this.rbAprove.Size = new System.Drawing.Size(74, 21);
            this.rbAprove.TabIndex = 4;
            this.rbAprove.Text = "Aprove";
            this.rbAprove.UseVisualStyleBackColor = true;
            // 
            // rbDeposito
            // 
            this.rbDeposito.AutoSize = true;
            this.rbDeposito.Location = new System.Drawing.Point(20, 116);
            this.rbDeposito.Name = "rbDeposito";
            this.rbDeposito.Size = new System.Drawing.Size(85, 21);
            this.rbDeposito.TabIndex = 3;
            this.rbDeposito.Text = "Depósito";
            this.rbDeposito.UseVisualStyleBackColor = true;
            // 
            // rbEstagFazenda
            // 
            this.rbEstagFazenda.AutoSize = true;
            this.rbEstagFazenda.Location = new System.Drawing.Point(18, 88);
            this.rbEstagFazenda.Name = "rbEstagFazenda";
            this.rbEstagFazenda.Size = new System.Drawing.Size(84, 21);
            this.rbEstagFazenda.TabIndex = 2;
            this.rbEstagFazenda.Text = "Fazenda";
            this.rbEstagFazenda.UseVisualStyleBackColor = true;
            // 
            // rbCocho
            // 
            this.rbCocho.AutoSize = true;
            this.rbCocho.Location = new System.Drawing.Point(18, 59);
            this.rbCocho.Name = "rbCocho";
            this.rbCocho.Size = new System.Drawing.Size(152, 21);
            this.rbCocho.TabIndex = 1;
            this.rbCocho.Text = "&CochoFermentação";
            this.rbCocho.UseVisualStyleBackColor = true;
            // 
            // rbEstagSemFiltro
            // 
            this.rbEstagSemFiltro.AutoSize = true;
            this.rbEstagSemFiltro.Checked = true;
            this.rbEstagSemFiltro.Location = new System.Drawing.Point(17, 29);
            this.rbEstagSemFiltro.Name = "rbEstagSemFiltro";
            this.rbEstagSemFiltro.Size = new System.Drawing.Size(92, 21);
            this.rbEstagSemFiltro.TabIndex = 0;
            this.rbEstagSemFiltro.TabStop = true;
            this.rbEstagSemFiltro.Text = "Sem Filtro";
            this.rbEstagSemFiltro.UseVisualStyleBackColor = true;
            // 
            // btnOk
            // 
            this.btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOk.Location = new System.Drawing.Point(135, 214);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(96, 28);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "&Ok";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnFiltro
            // 
            this.btnFiltro.Location = new System.Drawing.Point(18, 214);
            this.btnFiltro.Margin = new System.Windows.Forms.Padding(4);
            this.btnFiltro.Name = "btnFiltro";
            this.btnFiltro.Size = new System.Drawing.Size(103, 28);
            this.btnFiltro.TabIndex = 2;
            this.btnFiltro.Text = "&Filtro";
            this.btnFiltro.UseVisualStyleBackColor = true;
            // 
            // FrmFiltroProd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(674, 697);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmFiltroProd";
            this.Text = "FrmFiltroProd";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upAnos)).EndInit();
            this.gbVendas.ResumeLayout(false);
            this.gbVendas.PerformLayout();
            this.gbArmazenamento.ResumeLayout(false);
            this.gbArmazenamento.PerformLayout();
            this.gbEstagios.ResumeLayout(false);
            this.gbEstagios.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion


        // Como vou chamar os componentes referenciando em outra instancia, tornei todos os componentes públicos 
        public System.Windows.Forms.Panel pnTop;
        public System.Windows.Forms.Label lbColheita;
        public System.Windows.Forms.NumericUpDown upAnos;
        public System.Windows.Forms.Label lbSafra;
        public System.Windows.Forms.Label lbProduto;
        public System.Windows.Forms.Label lbContrato;
        public System.Windows.Forms.TextBox txContratos;
        public System.Windows.Forms.GroupBox gbVendas;
        public System.Windows.Forms.RadioButton rbNaoVendidos;
        public System.Windows.Forms.RadioButton rbVendidos;
        public System.Windows.Forms.RadioButton rbLotesSemFiltro;
        public System.Windows.Forms.GroupBox gbArmazenamento;
        public System.Windows.Forms.RadioButton rbDepositoCentral;
        public System.Windows.Forms.RadioButton rbTriagem;
        public System.Windows.Forms.RadioButton rbArmazFazenda;
        public System.Windows.Forms.RadioButton rbArmazSemFiltro;
        public System.Windows.Forms.ComboBox cbTipoProd;
        public System.Windows.Forms.ComboBox cbSafra;
        public System.Windows.Forms.ComboBox cbProdutos;
        public System.Windows.Forms.Button btnCancele;
        public System.Windows.Forms.GroupBox gbEstagios;
        public System.Windows.Forms.RadioButton rbCatagem;
        public System.Windows.Forms.RadioButton rbAprove;
        public System.Windows.Forms.RadioButton rbDeposito;
        public System.Windows.Forms.RadioButton rbEstagFazenda;
        public System.Windows.Forms.RadioButton rbCocho;
        public System.Windows.Forms.RadioButton rbEstagSemFiltro;
        public System.Windows.Forms.Button btnOk;
        public System.Windows.Forms.Button btnFiltro;
        public Pesquise oPesqProd;
    }
}