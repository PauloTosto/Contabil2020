namespace PrjApiParceiro_C.Fiscais
{
    partial class FrmFiscal_Novo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFiscal_Novo));
            this.pnCabecalho = new System.Windows.Forms.Panel();
            this.lbPeriodo = new System.Windows.Forms.Label();
            this.gbFiltro = new System.Windows.Forms.GroupBox();
            this.lbFazenda_Contabil = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txFazenda = new System.Windows.Forms.TextBox();
            this.txCFOP = new System.Windows.Forms.TextBox();
            this.gbCritica = new System.Windows.Forms.GroupBox();
            this.rbNenhuma = new System.Windows.Forms.RadioButton();
            this.rbIncompleto = new System.Windows.Forms.RadioButton();
            this.rbCompleto = new System.Windows.Forms.RadioButton();
            this.rbTodas = new System.Windows.Forms.RadioButton();
            this.gbFiscais = new System.Windows.Forms.GroupBox();
            this.rbVendas = new System.Windows.Forms.RadioButton();
            this.rbEntradas = new System.Windows.Forms.RadioButton();
            this.rbGeral = new System.Windows.Forms.RadioButton();
            this.tcLink = new System.Windows.Forms.TabControl();
            this.tpDisponivel = new System.Windows.Forms.TabPage();
            this.pnDisponivel = new System.Windows.Forms.Panel();
            this.dgvDisponivel = new System.Windows.Forms.DataGridView();
            this.pn_Disp_Strip = new System.Windows.Forms.Panel();
            this.statusDisponivel = new System.Windows.Forms.StatusStrip();
            this.tpLigados = new System.Windows.Forms.TabPage();
            this.pnSubLink = new System.Windows.Forms.Panel();
            this.statusLink = new System.Windows.Forms.StatusStrip();
            this.pnLink = new System.Windows.Forms.Panel();
            this.dgvLink = new System.Windows.Forms.DataGridView();
            this.tcVenda = new System.Windows.Forms.TabControl();
            this.tpDisponivelVenda = new System.Windows.Forms.TabPage();
            this.pnDispVenda_sub = new System.Windows.Forms.Panel();
            this.statusDispVenda = new System.Windows.Forms.StatusStrip();
            this.pnDispVenda_Top = new System.Windows.Forms.Panel();
            this.dgvDispVenda = new System.Windows.Forms.DataGridView();
            this.tpLigadosVenda = new System.Windows.Forms.TabPage();
            this.pnVendaLink_sub = new System.Windows.Forms.Panel();
            this.statusLinkVenda = new System.Windows.Forms.StatusStrip();
            this.pnVendaLink_Top = new System.Windows.Forms.Panel();
            this.dgvLinkVenda = new System.Windows.Forms.DataGridView();
            this.pnToolStrip = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripDelete = new System.Windows.Forms.ToolStripButton();
            this.toolStripConsulta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSair = new System.Windows.Forms.ToolStripButton();
            this.toolStripNfe = new System.Windows.Forms.ToolStripButton();
            this.toolStripVenda = new System.Windows.Forms.ToolStripButton();
            this.toolStripRecarregueDisponivel = new System.Windows.Forms.ToolStripButton();
            this.pnMestre = new System.Windows.Forms.Panel();
            this.pnMestreSub = new System.Windows.Forms.Panel();
            this.statusMestre = new System.Windows.Forms.StatusStrip();
            this.tcMestre = new System.Windows.Forms.TabControl();
            this.tpMestre = new System.Windows.Forms.TabPage();
            this.dgvMestre = new System.Windows.Forms.DataGridView();
            this.pnNFs = new System.Windows.Forms.Panel();
            this.tcItens = new System.Windows.Forms.TabControl();
            this.tpItens = new System.Windows.Forms.TabPage();
            this.dgvItens = new System.Windows.Forms.DataGridView();
            this.pnCabecalho.SuspendLayout();
            this.gbFiltro.SuspendLayout();
            this.gbCritica.SuspendLayout();
            this.gbFiscais.SuspendLayout();
            this.tcLink.SuspendLayout();
            this.tpDisponivel.SuspendLayout();
            this.pnDisponivel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponivel)).BeginInit();
            this.pn_Disp_Strip.SuspendLayout();
            this.tpLigados.SuspendLayout();
            this.pnSubLink.SuspendLayout();
            this.pnLink.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLink)).BeginInit();
            this.tcVenda.SuspendLayout();
            this.tpDisponivelVenda.SuspendLayout();
            this.pnDispVenda_sub.SuspendLayout();
            this.pnDispVenda_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDispVenda)).BeginInit();
            this.tpLigadosVenda.SuspendLayout();
            this.pnVendaLink_sub.SuspendLayout();
            this.pnVendaLink_Top.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinkVenda)).BeginInit();
            this.pnToolStrip.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.pnMestre.SuspendLayout();
            this.pnMestreSub.SuspendLayout();
            this.tcMestre.SuspendLayout();
            this.tpMestre.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMestre)).BeginInit();
            this.pnNFs.SuspendLayout();
            this.tcItens.SuspendLayout();
            this.tpItens.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).BeginInit();
            this.SuspendLayout();
            // 
            // pnCabecalho
            // 
            this.pnCabecalho.Controls.Add(this.lbPeriodo);
            this.pnCabecalho.Controls.Add(this.gbFiltro);
            this.pnCabecalho.Controls.Add(this.gbCritica);
            this.pnCabecalho.Controls.Add(this.gbFiscais);
            this.pnCabecalho.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnCabecalho.Location = new System.Drawing.Point(0, 0);
            this.pnCabecalho.Name = "pnCabecalho";
            this.pnCabecalho.Size = new System.Drawing.Size(1494, 80);
            this.pnCabecalho.TabIndex = 0;
            // 
            // lbPeriodo
            // 
            this.lbPeriodo.AutoSize = true;
            this.lbPeriodo.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPeriodo.Location = new System.Drawing.Point(15, 4);
            this.lbPeriodo.Name = "lbPeriodo";
            this.lbPeriodo.Size = new System.Drawing.Size(0, 17);
            this.lbPeriodo.TabIndex = 3;
            this.lbPeriodo.Visible = false;
            // 
            // gbFiltro
            // 
            this.gbFiltro.Controls.Add(this.lbFazenda_Contabil);
            this.gbFiltro.Controls.Add(this.label1);
            this.gbFiltro.Controls.Add(this.txFazenda);
            this.gbFiltro.Controls.Add(this.txCFOP);
            this.gbFiltro.Location = new System.Drawing.Point(12, 26);
            this.gbFiltro.Name = "gbFiltro";
            this.gbFiltro.Size = new System.Drawing.Size(413, 43);
            this.gbFiltro.TabIndex = 2;
            this.gbFiltro.TabStop = false;
            this.gbFiltro.Text = "Filtros";
            // 
            // lbFazenda_Contabil
            // 
            this.lbFazenda_Contabil.AutoSize = true;
            this.lbFazenda_Contabil.Location = new System.Drawing.Point(199, 17);
            this.lbFazenda_Contabil.Name = "lbFazenda_Contabil";
            this.lbFazenda_Contabil.Size = new System.Drawing.Size(76, 17);
            this.lbFazenda_Contabil.TabIndex = 11;
            this.lbFazenda_Contabil.Text = "FAZENDA:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "CFOP:";
            // 
            // txFazenda
            // 
            this.txFazenda.Location = new System.Drawing.Point(283, 17);
            this.txFazenda.Name = "txFazenda";
            this.txFazenda.Size = new System.Drawing.Size(121, 22);
            this.txFazenda.TabIndex = 9;
            this.txFazenda.Leave += new System.EventHandler(this.txFazenda_Leave);
            // 
            // txCFOP
            // 
            this.txCFOP.Location = new System.Drawing.Point(97, 17);
            this.txCFOP.Name = "txCFOP";
            this.txCFOP.Size = new System.Drawing.Size(75, 22);
            this.txCFOP.TabIndex = 8;
            this.txCFOP.Leave += new System.EventHandler(this.txCFOP_Leave);
            // 
            // gbCritica
            // 
            this.gbCritica.Controls.Add(this.rbNenhuma);
            this.gbCritica.Controls.Add(this.rbIncompleto);
            this.gbCritica.Controls.Add(this.rbCompleto);
            this.gbCritica.Controls.Add(this.rbTodas);
            this.gbCritica.Location = new System.Drawing.Point(923, 10);
            this.gbCritica.Name = "gbCritica";
            this.gbCritica.Size = new System.Drawing.Size(518, 46);
            this.gbCritica.TabIndex = 1;
            this.gbCritica.TabStop = false;
            this.gbCritica.Text = "Crí&tica";
            // 
            // rbNenhuma
            // 
            this.rbNenhuma.AutoSize = true;
            this.rbNenhuma.Location = new System.Drawing.Point(388, 16);
            this.rbNenhuma.Name = "rbNenhuma";
            this.rbNenhuma.Size = new System.Drawing.Size(90, 21);
            this.rbNenhuma.TabIndex = 3;
            this.rbNenhuma.Text = "Nenhuma";
            this.rbNenhuma.UseVisualStyleBackColor = true;
            this.rbNenhuma.Click += new System.EventHandler(this.rbNenhuma_Click);
            // 
            // rbIncompleto
            // 
            this.rbIncompleto.AutoSize = true;
            this.rbIncompleto.Location = new System.Drawing.Point(272, 16);
            this.rbIncompleto.Name = "rbIncompleto";
            this.rbIncompleto.Size = new System.Drawing.Size(97, 21);
            this.rbIncompleto.TabIndex = 2;
            this.rbIncompleto.Text = "Incompleto";
            this.rbIncompleto.UseVisualStyleBackColor = true;
            this.rbIncompleto.Click += new System.EventHandler(this.rbIncompleto_Click);
            // 
            // rbCompleto
            // 
            this.rbCompleto.AutoSize = true;
            this.rbCompleto.Location = new System.Drawing.Point(148, 17);
            this.rbCompleto.Name = "rbCompleto";
            this.rbCompleto.Size = new System.Drawing.Size(88, 21);
            this.rbCompleto.TabIndex = 1;
            this.rbCompleto.Text = "Completo";
            this.rbCompleto.UseVisualStyleBackColor = true;
            this.rbCompleto.Click += new System.EventHandler(this.rbCompleto_Click);
            // 
            // rbTodas
            // 
            this.rbTodas.AutoSize = true;
            this.rbTodas.Checked = true;
            this.rbTodas.Location = new System.Drawing.Point(35, 18);
            this.rbTodas.Name = "rbTodas";
            this.rbTodas.Size = new System.Drawing.Size(69, 21);
            this.rbTodas.TabIndex = 0;
            this.rbTodas.TabStop = true;
            this.rbTodas.Text = "Todas";
            this.rbTodas.UseVisualStyleBackColor = true;
            this.rbTodas.Click += new System.EventHandler(this.rbTodas_Click);
            // 
            // gbFiscais
            // 
            this.gbFiscais.Controls.Add(this.rbVendas);
            this.gbFiscais.Controls.Add(this.rbEntradas);
            this.gbFiscais.Controls.Add(this.rbGeral);
            this.gbFiscais.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.gbFiscais.Location = new System.Drawing.Point(483, 11);
            this.gbFiscais.Name = "gbFiscais";
            this.gbFiscais.Size = new System.Drawing.Size(368, 49);
            this.gbFiscais.TabIndex = 0;
            this.gbFiscais.TabStop = false;
            this.gbFiscais.Text = "Notas &Fiscais";
            // 
            // rbVendas
            // 
            this.rbVendas.AutoSize = true;
            this.rbVendas.Location = new System.Drawing.Point(257, 18);
            this.rbVendas.Name = "rbVendas";
            this.rbVendas.Size = new System.Drawing.Size(77, 21);
            this.rbVendas.TabIndex = 2;
            this.rbVendas.Text = "Vendas";
            this.rbVendas.UseVisualStyleBackColor = true;
            this.rbVendas.Click += new System.EventHandler(this.rbVendas_Click);
            // 
            // rbEntradas
            // 
            this.rbEntradas.AutoSize = true;
            this.rbEntradas.Location = new System.Drawing.Point(141, 20);
            this.rbEntradas.Name = "rbEntradas";
            this.rbEntradas.Size = new System.Drawing.Size(86, 21);
            this.rbEntradas.TabIndex = 1;
            this.rbEntradas.Text = "Entradas";
            this.rbEntradas.UseVisualStyleBackColor = true;
            this.rbEntradas.Click += new System.EventHandler(this.rbEntradas_Click);
            // 
            // rbGeral
            // 
            this.rbGeral.AutoSize = true;
            this.rbGeral.Checked = true;
            this.rbGeral.Location = new System.Drawing.Point(31, 19);
            this.rbGeral.Name = "rbGeral";
            this.rbGeral.Size = new System.Drawing.Size(64, 21);
            this.rbGeral.TabIndex = 0;
            this.rbGeral.TabStop = true;
            this.rbGeral.Text = "Geral";
            this.rbGeral.UseVisualStyleBackColor = true;
            this.rbGeral.Click += new System.EventHandler(this.rbGeral_Click);
            // 
            // tcLink
            // 
            this.tcLink.Controls.Add(this.tpDisponivel);
            this.tcLink.Controls.Add(this.tpLigados);
            this.tcLink.Location = new System.Drawing.Point(221, 498);
            this.tcLink.Name = "tcLink";
            this.tcLink.SelectedIndex = 0;
            this.tcLink.Size = new System.Drawing.Size(596, 236);
            this.tcLink.TabIndex = 3;
            // 
            // tpDisponivel
            // 
            this.tpDisponivel.Controls.Add(this.pnDisponivel);
            this.tpDisponivel.Controls.Add(this.pn_Disp_Strip);
            this.tpDisponivel.Location = new System.Drawing.Point(4, 25);
            this.tpDisponivel.Name = "tpDisponivel";
            this.tpDisponivel.Padding = new System.Windows.Forms.Padding(3);
            this.tpDisponivel.Size = new System.Drawing.Size(588, 207);
            this.tpDisponivel.TabIndex = 0;
            this.tpDisponivel.Text = "Disponíveis";
            this.tpDisponivel.UseVisualStyleBackColor = true;
            // 
            // pnDisponivel
            // 
            this.pnDisponivel.Controls.Add(this.dgvDisponivel);
            this.pnDisponivel.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnDisponivel.Location = new System.Drawing.Point(3, 3);
            this.pnDisponivel.Name = "pnDisponivel";
            this.pnDisponivel.Size = new System.Drawing.Size(582, 170);
            this.pnDisponivel.TabIndex = 3;
            // 
            // dgvDisponivel
            // 
            this.dgvDisponivel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDisponivel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDisponivel.Location = new System.Drawing.Point(0, 0);
            this.dgvDisponivel.Name = "dgvDisponivel";
            this.dgvDisponivel.RowHeadersWidth = 51;
            this.dgvDisponivel.RowTemplate.Height = 24;
            this.dgvDisponivel.Size = new System.Drawing.Size(582, 170);
            this.dgvDisponivel.TabIndex = 0;
            this.dgvDisponivel.Enter += new System.EventHandler(this.dgvDisponivel_Enter);
            this.dgvDisponivel.Leave += new System.EventHandler(this.dgvDisponivel_Leave);
            // 
            // pn_Disp_Strip
            // 
            this.pn_Disp_Strip.Controls.Add(this.statusDisponivel);
            this.pn_Disp_Strip.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pn_Disp_Strip.Location = new System.Drawing.Point(3, 174);
            this.pn_Disp_Strip.Name = "pn_Disp_Strip";
            this.pn_Disp_Strip.Size = new System.Drawing.Size(582, 30);
            this.pn_Disp_Strip.TabIndex = 2;
            // 
            // statusDisponivel
            // 
            this.statusDisponivel.Dock = System.Windows.Forms.DockStyle.None;
            this.statusDisponivel.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusDisponivel.Location = new System.Drawing.Point(11, 3);
            this.statusDisponivel.Name = "statusDisponivel";
            this.statusDisponivel.Size = new System.Drawing.Size(202, 22);
            this.statusDisponivel.TabIndex = 0;
            this.statusDisponivel.Text = "statusStrip1";
            // 
            // tpLigados
            // 
            this.tpLigados.Controls.Add(this.pnSubLink);
            this.tpLigados.Controls.Add(this.pnLink);
            this.tpLigados.Location = new System.Drawing.Point(4, 25);
            this.tpLigados.Name = "tpLigados";
            this.tpLigados.Padding = new System.Windows.Forms.Padding(3);
            this.tpLigados.Size = new System.Drawing.Size(588, 212);
            this.tpLigados.TabIndex = 1;
            this.tpLigados.Text = "Ligados";
            this.tpLigados.UseVisualStyleBackColor = true;
            // 
            // pnSubLink
            // 
            this.pnSubLink.Controls.Add(this.statusLink);
            this.pnSubLink.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnSubLink.Location = new System.Drawing.Point(3, 180);
            this.pnSubLink.Name = "pnSubLink";
            this.pnSubLink.Size = new System.Drawing.Size(582, 29);
            this.pnSubLink.TabIndex = 1;
            // 
            // statusLink
            // 
            this.statusLink.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusLink.Location = new System.Drawing.Point(0, 7);
            this.statusLink.Name = "statusLink";
            this.statusLink.Size = new System.Drawing.Size(582, 22);
            this.statusLink.TabIndex = 0;
            this.statusLink.Text = "statusStrip1";
            // 
            // pnLink
            // 
            this.pnLink.Controls.Add(this.dgvLink);
            this.pnLink.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnLink.Location = new System.Drawing.Point(3, 3);
            this.pnLink.Name = "pnLink";
            this.pnLink.Size = new System.Drawing.Size(582, 145);
            this.pnLink.TabIndex = 0;
            // 
            // dgvLink
            // 
            this.dgvLink.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLink.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLink.Location = new System.Drawing.Point(0, 0);
            this.dgvLink.Name = "dgvLink";
            this.dgvLink.RowHeadersWidth = 51;
            this.dgvLink.RowTemplate.Height = 24;
            this.dgvLink.Size = new System.Drawing.Size(582, 145);
            this.dgvLink.TabIndex = 1;
            this.dgvLink.Enter += new System.EventHandler(this.dgvLink_Enter);
            this.dgvLink.Leave += new System.EventHandler(this.dgvLink_Leave);
            // 
            // tcVenda
            // 
            this.tcVenda.Controls.Add(this.tpDisponivelVenda);
            this.tcVenda.Controls.Add(this.tpLigadosVenda);
            this.tcVenda.Location = new System.Drawing.Point(455, 498);
            this.tcVenda.Name = "tcVenda";
            this.tcVenda.SelectedIndex = 0;
            this.tcVenda.Size = new System.Drawing.Size(837, 222);
            this.tcVenda.TabIndex = 4;
            // 
            // tpDisponivelVenda
            // 
            this.tpDisponivelVenda.Controls.Add(this.pnDispVenda_sub);
            this.tpDisponivelVenda.Controls.Add(this.pnDispVenda_Top);
            this.tpDisponivelVenda.Location = new System.Drawing.Point(4, 25);
            this.tpDisponivelVenda.Name = "tpDisponivelVenda";
            this.tpDisponivelVenda.Padding = new System.Windows.Forms.Padding(3);
            this.tpDisponivelVenda.Size = new System.Drawing.Size(829, 193);
            this.tpDisponivelVenda.TabIndex = 0;
            this.tpDisponivelVenda.Text = "Disponiveis Venda";
            this.tpDisponivelVenda.UseVisualStyleBackColor = true;
            // 
            // pnDispVenda_sub
            // 
            this.pnDispVenda_sub.Controls.Add(this.statusDispVenda);
            this.pnDispVenda_sub.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnDispVenda_sub.Location = new System.Drawing.Point(3, 158);
            this.pnDispVenda_sub.Name = "pnDispVenda_sub";
            this.pnDispVenda_sub.Size = new System.Drawing.Size(823, 32);
            this.pnDispVenda_sub.TabIndex = 1;
            // 
            // statusDispVenda
            // 
            this.statusDispVenda.Dock = System.Windows.Forms.DockStyle.None;
            this.statusDispVenda.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusDispVenda.Location = new System.Drawing.Point(10, 3);
            this.statusDispVenda.Name = "statusDispVenda";
            this.statusDispVenda.Size = new System.Drawing.Size(202, 22);
            this.statusDispVenda.TabIndex = 0;
            this.statusDispVenda.Text = "statusStrip2";
            // 
            // pnDispVenda_Top
            // 
            this.pnDispVenda_Top.Controls.Add(this.dgvDispVenda);
            this.pnDispVenda_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnDispVenda_Top.Location = new System.Drawing.Point(3, 3);
            this.pnDispVenda_Top.Name = "pnDispVenda_Top";
            this.pnDispVenda_Top.Size = new System.Drawing.Size(823, 149);
            this.pnDispVenda_Top.TabIndex = 0;
            // 
            // dgvDispVenda
            // 
            this.dgvDispVenda.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDispVenda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDispVenda.Location = new System.Drawing.Point(0, 0);
            this.dgvDispVenda.Name = "dgvDispVenda";
            this.dgvDispVenda.RowHeadersWidth = 51;
            this.dgvDispVenda.RowTemplate.Height = 24;
            this.dgvDispVenda.Size = new System.Drawing.Size(823, 149);
            this.dgvDispVenda.TabIndex = 1;
            this.dgvDispVenda.Enter += new System.EventHandler(this.dgvDispVenda_Enter);
            this.dgvDispVenda.Leave += new System.EventHandler(this.dgvDispVenda_Leave);
            // 
            // tpLigadosVenda
            // 
            this.tpLigadosVenda.Controls.Add(this.pnVendaLink_sub);
            this.tpLigadosVenda.Controls.Add(this.pnVendaLink_Top);
            this.tpLigadosVenda.Location = new System.Drawing.Point(4, 25);
            this.tpLigadosVenda.Name = "tpLigadosVenda";
            this.tpLigadosVenda.Padding = new System.Windows.Forms.Padding(3);
            this.tpLigadosVenda.Size = new System.Drawing.Size(723, 193);
            this.tpLigadosVenda.TabIndex = 1;
            this.tpLigadosVenda.Text = "Ligados Venda";
            this.tpLigadosVenda.UseVisualStyleBackColor = true;
            // 
            // pnVendaLink_sub
            // 
            this.pnVendaLink_sub.Controls.Add(this.statusLinkVenda);
            this.pnVendaLink_sub.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnVendaLink_sub.Location = new System.Drawing.Point(3, 155);
            this.pnVendaLink_sub.Name = "pnVendaLink_sub";
            this.pnVendaLink_sub.Size = new System.Drawing.Size(717, 35);
            this.pnVendaLink_sub.TabIndex = 1;
            // 
            // statusLinkVenda
            // 
            this.statusLinkVenda.Dock = System.Windows.Forms.DockStyle.None;
            this.statusLinkVenda.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusLinkVenda.Location = new System.Drawing.Point(9, 5);
            this.statusLinkVenda.Name = "statusLinkVenda";
            this.statusLinkVenda.Size = new System.Drawing.Size(202, 22);
            this.statusLinkVenda.TabIndex = 0;
            this.statusLinkVenda.Text = "statusStrip3";
            // 
            // pnVendaLink_Top
            // 
            this.pnVendaLink_Top.Controls.Add(this.dgvLinkVenda);
            this.pnVendaLink_Top.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnVendaLink_Top.Location = new System.Drawing.Point(3, 3);
            this.pnVendaLink_Top.Name = "pnVendaLink_Top";
            this.pnVendaLink_Top.Size = new System.Drawing.Size(717, 143);
            this.pnVendaLink_Top.TabIndex = 0;
            // 
            // dgvLinkVenda
            // 
            this.dgvLinkVenda.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLinkVenda.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLinkVenda.Location = new System.Drawing.Point(0, 0);
            this.dgvLinkVenda.Name = "dgvLinkVenda";
            this.dgvLinkVenda.RowHeadersWidth = 51;
            this.dgvLinkVenda.RowTemplate.Height = 24;
            this.dgvLinkVenda.Size = new System.Drawing.Size(717, 143);
            this.dgvLinkVenda.TabIndex = 1;
            this.dgvLinkVenda.Enter += new System.EventHandler(this.dgvLinkVenda_Enter);
            this.dgvLinkVenda.Leave += new System.EventHandler(this.dgvLinkVenda_Leave);
            // 
            // pnToolStrip
            // 
            this.pnToolStrip.Controls.Add(this.toolStrip1);
            this.pnToolStrip.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnToolStrip.Location = new System.Drawing.Point(1394, 80);
            this.pnToolStrip.Name = "pnToolStrip";
            this.pnToolStrip.Size = new System.Drawing.Size(100, 645);
            this.pnToolStrip.TabIndex = 10;
            // 
            // toolStrip1
            // 
            this.toolStrip1.AllowItemReorder = true;
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip1.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripDelete,
            this.toolStripConsulta,
            this.toolStripSair,
            this.toolStripNfe,
            this.toolStripVenda,
            this.toolStripRecarregueDisponivel});
            this.toolStrip1.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Table;
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(100, 645);
            this.toolStrip1.TabIndex = 10;
            this.toolStrip1.Text = "Tool";
            // 
            // toolStripDelete
            // 
            this.toolStripDelete.Image = ((System.Drawing.Image)(resources.GetObject("toolStripDelete.Image")));
            this.toolStripDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripDelete.Name = "toolStripDelete";
            this.toolStripDelete.Size = new System.Drawing.Size(73, 24);
            this.toolStripDelete.Text = "&Delete";
            this.toolStripDelete.Click += new System.EventHandler(this.toolStripDelete_Click);
            // 
            // toolStripConsulta
            // 
            this.toolStripConsulta.Image = ((System.Drawing.Image)(resources.GetObject("toolStripConsulta.Image")));
            this.toolStripConsulta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripConsulta.Name = "toolStripConsulta";
            this.toolStripConsulta.Size = new System.Drawing.Size(87, 24);
            this.toolStripConsulta.Text = "C&onsulta";
            this.toolStripConsulta.Click += new System.EventHandler(this.toolStripConsulta_Click);
            // 
            // toolStripSair
            // 
            this.toolStripSair.Image = ((System.Drawing.Image)(resources.GetObject("toolStripSair.Image")));
            this.toolStripSair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSair.Name = "toolStripSair";
            this.toolStripSair.Size = new System.Drawing.Size(57, 24);
            this.toolStripSair.Text = "&Sair";
            // 
            // toolStripNfe
            // 
            this.toolStripNfe.Image = ((System.Drawing.Image)(resources.GetObject("toolStripNfe.Image")));
            this.toolStripNfe.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripNfe.Name = "toolStripNfe";
            this.toolStripNfe.Size = new System.Drawing.Size(74, 24);
            this.toolStripNfe.Text = "Nfe =>";
            this.toolStripNfe.Click += new System.EventHandler(this.toolStripNfe_Click);
            // 
            // toolStripVenda
            // 
            this.toolStripVenda.Image = ((System.Drawing.Image)(resources.GetObject("toolStripVenda.Image")));
            this.toolStripVenda.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripVenda.Name = "toolStripVenda";
            this.toolStripVenda.Size = new System.Drawing.Size(93, 24);
            this.toolStripVenda.Text = "Venda =>";
            this.toolStripVenda.Click += new System.EventHandler(this.toolStripVenda_Click);
            // 
            // toolStripRecarregueDisponivel
            // 
            this.toolStripRecarregueDisponivel.Image = ((System.Drawing.Image)(resources.GetObject("toolStripRecarregueDisponivel.Image")));
            this.toolStripRecarregueDisponivel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRecarregueDisponivel.Name = "toolStripRecarregueDisponivel";
            this.toolStripRecarregueDisponivel.Size = new System.Drawing.Size(97, 24);
            this.toolStripRecarregueDisponivel.Text = "Disponivel";
            this.toolStripRecarregueDisponivel.Click += new System.EventHandler(this.toolStripRecarregueDisponivel_Click);
            // 
            // pnMestre
            // 
            this.pnMestre.AutoScroll = true;
            this.pnMestre.Controls.Add(this.pnMestreSub);
            this.pnMestre.Controls.Add(this.tcMestre);
            this.pnMestre.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMestre.Location = new System.Drawing.Point(0, 80);
            this.pnMestre.Name = "pnMestre";
            this.pnMestre.Size = new System.Drawing.Size(1394, 230);
            this.pnMestre.TabIndex = 11;
            // 
            // pnMestreSub
            // 
            this.pnMestreSub.Controls.Add(this.statusMestre);
            this.pnMestreSub.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMestreSub.Location = new System.Drawing.Point(0, 200);
            this.pnMestreSub.Name = "pnMestreSub";
            this.pnMestreSub.Size = new System.Drawing.Size(1373, 46);
            this.pnMestreSub.TabIndex = 3;
            // 
            // statusMestre
            // 
            this.statusMestre.Dock = System.Windows.Forms.DockStyle.None;
            this.statusMestre.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusMestre.Location = new System.Drawing.Point(424, 9);
            this.statusMestre.Name = "statusMestre";
            this.statusMestre.Size = new System.Drawing.Size(202, 22);
            this.statusMestre.TabIndex = 0;
            this.statusMestre.Text = "statusStrip1";
            // 
            // tcMestre
            // 
            this.tcMestre.Controls.Add(this.tpMestre);
            this.tcMestre.Dock = System.Windows.Forms.DockStyle.Top;
            this.tcMestre.Location = new System.Drawing.Point(0, 0);
            this.tcMestre.Name = "tcMestre";
            this.tcMestre.SelectedIndex = 0;
            this.tcMestre.Size = new System.Drawing.Size(1373, 200);
            this.tcMestre.TabIndex = 2;
            this.tcMestre.TabStop = false;
            // 
            // tpMestre
            // 
            this.tpMestre.AutoScroll = true;
            this.tpMestre.Controls.Add(this.dgvMestre);
            this.tpMestre.Location = new System.Drawing.Point(4, 25);
            this.tpMestre.Name = "tpMestre";
            this.tpMestre.Padding = new System.Windows.Forms.Padding(3);
            this.tpMestre.Size = new System.Drawing.Size(1365, 171);
            this.tpMestre.TabIndex = 0;
            this.tpMestre.Text = "Notas Fiscais";
            this.tpMestre.UseVisualStyleBackColor = true;
            // 
            // dgvMestre
            // 
            this.dgvMestre.AllowUserToAddRows = false;
            this.dgvMestre.AllowUserToDeleteRows = false;
            this.dgvMestre.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMestre.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvMestre.Location = new System.Drawing.Point(3, 3);
            this.dgvMestre.Margin = new System.Windows.Forms.Padding(10);
            this.dgvMestre.Name = "dgvMestre";
            this.dgvMestre.RowHeadersWidth = 51;
            this.dgvMestre.RowTemplate.Height = 18;
            this.dgvMestre.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvMestre.Size = new System.Drawing.Size(1359, 165);
            this.dgvMestre.StandardTab = true;
            this.dgvMestre.TabIndex = 0;
            // 
            // pnNFs
            // 
            this.pnNFs.Controls.Add(this.tcItens);
            this.pnNFs.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnNFs.Location = new System.Drawing.Point(0, 310);
            this.pnNFs.Name = "pnNFs";
            this.pnNFs.Size = new System.Drawing.Size(1394, 160);
            this.pnNFs.TabIndex = 12;
            // 
            // tcItens
            // 
            this.tcItens.Controls.Add(this.tpItens);
            this.tcItens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcItens.Location = new System.Drawing.Point(0, 0);
            this.tcItens.Name = "tcItens";
            this.tcItens.SelectedIndex = 0;
            this.tcItens.Size = new System.Drawing.Size(1394, 160);
            this.tcItens.TabIndex = 3;
            this.tcItens.TabStop = false;
            // 
            // tpItens
            // 
            this.tpItens.Controls.Add(this.dgvItens);
            this.tpItens.Location = new System.Drawing.Point(4, 25);
            this.tpItens.Name = "tpItens";
            this.tpItens.Padding = new System.Windows.Forms.Padding(3);
            this.tpItens.Size = new System.Drawing.Size(1386, 131);
            this.tpItens.TabIndex = 0;
            this.tpItens.Text = "Itens da NF";
            this.tpItens.UseVisualStyleBackColor = true;
            // 
            // dgvItens
            // 
            this.dgvItens.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItens.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItens.Location = new System.Drawing.Point(3, 3);
            this.dgvItens.Name = "dgvItens";
            this.dgvItens.RowHeadersWidth = 51;
            this.dgvItens.RowTemplate.Height = 24;
            this.dgvItens.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dgvItens.Size = new System.Drawing.Size(1380, 125);
            this.dgvItens.StandardTab = true;
            this.dgvItens.TabIndex = 0;
            // 
            // FrmFiscal_Novo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1494, 725);
            this.Controls.Add(this.pnNFs);
            this.Controls.Add(this.pnMestre);
            this.Controls.Add(this.pnToolStrip);
            this.Controls.Add(this.tcVenda);
            this.Controls.Add(this.tcLink);
            this.Controls.Add(this.pnCabecalho);
            this.Name = "FrmFiscal_Novo";
            this.Text = "FrmFiscal_Novo";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmFiscal_Novo_KeyDown);
            this.pnCabecalho.ResumeLayout(false);
            this.pnCabecalho.PerformLayout();
            this.gbFiltro.ResumeLayout(false);
            this.gbFiltro.PerformLayout();
            this.gbCritica.ResumeLayout(false);
            this.gbCritica.PerformLayout();
            this.gbFiscais.ResumeLayout(false);
            this.gbFiscais.PerformLayout();
            this.tcLink.ResumeLayout(false);
            this.tpDisponivel.ResumeLayout(false);
            this.pnDisponivel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDisponivel)).EndInit();
            this.pn_Disp_Strip.ResumeLayout(false);
            this.pn_Disp_Strip.PerformLayout();
            this.tpLigados.ResumeLayout(false);
            this.pnSubLink.ResumeLayout(false);
            this.pnSubLink.PerformLayout();
            this.pnLink.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLink)).EndInit();
            this.tcVenda.ResumeLayout(false);
            this.tpDisponivelVenda.ResumeLayout(false);
            this.pnDispVenda_sub.ResumeLayout(false);
            this.pnDispVenda_sub.PerformLayout();
            this.pnDispVenda_Top.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDispVenda)).EndInit();
            this.tpLigadosVenda.ResumeLayout(false);
            this.pnVendaLink_sub.ResumeLayout(false);
            this.pnVendaLink_sub.PerformLayout();
            this.pnVendaLink_Top.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLinkVenda)).EndInit();
            this.pnToolStrip.ResumeLayout(false);
            this.pnToolStrip.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.pnMestre.ResumeLayout(false);
            this.pnMestreSub.ResumeLayout(false);
            this.pnMestreSub.PerformLayout();
            this.tcMestre.ResumeLayout(false);
            this.tpMestre.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMestre)).EndInit();
            this.pnNFs.ResumeLayout(false);
            this.tcItens.ResumeLayout(false);
            this.tpItens.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItens)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnCabecalho;
        private System.Windows.Forms.TabControl tcLink;
        private System.Windows.Forms.TabPage tpDisponivel;
        private System.Windows.Forms.TabPage tpLigados;
        private System.Windows.Forms.TabControl tcVenda;
        private System.Windows.Forms.TabPage tpDisponivelVenda;
        private System.Windows.Forms.TabPage tpLigadosVenda;
        private System.Windows.Forms.GroupBox gbFiscais;
        private System.Windows.Forms.GroupBox gbCritica;
        private System.Windows.Forms.RadioButton rbNenhuma;
        private System.Windows.Forms.RadioButton rbIncompleto;
        private System.Windows.Forms.RadioButton rbCompleto;
        private System.Windows.Forms.RadioButton rbTodas;
        private System.Windows.Forms.RadioButton rbVendas;
        private System.Windows.Forms.RadioButton rbEntradas;
        private System.Windows.Forms.RadioButton rbGeral;
        private System.Windows.Forms.Panel pnToolStrip;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolStripDelete;
        private System.Windows.Forms.ToolStripButton toolStripConsulta;
        private System.Windows.Forms.Panel pnMestre;
        private System.Windows.Forms.TabControl tcMestre;
        private System.Windows.Forms.TabPage tpMestre;
        private System.Windows.Forms.Panel pnNFs;
        private System.Windows.Forms.TabControl tcItens;
        private System.Windows.Forms.TabPage tpItens;
        private System.Windows.Forms.DataGridView dgvItens;
        private System.Windows.Forms.DataGridView dgvMestre;
        private System.Windows.Forms.Panel pnMestreSub;
        private System.Windows.Forms.StatusStrip statusMestre;
        private System.Windows.Forms.Panel pn_Disp_Strip;
        private System.Windows.Forms.StatusStrip statusDisponivel;
        private System.Windows.Forms.Panel pnDisponivel;
        private System.Windows.Forms.DataGridView dgvDisponivel;
        private System.Windows.Forms.Panel pnSubLink;
        private System.Windows.Forms.StatusStrip statusLink;
        private System.Windows.Forms.Panel pnLink;
        private System.Windows.Forms.DataGridView dgvLink;
        private System.Windows.Forms.Panel pnDispVenda_sub;
        private System.Windows.Forms.StatusStrip statusDispVenda;
        private System.Windows.Forms.Panel pnDispVenda_Top;
        private System.Windows.Forms.DataGridView dgvDispVenda;
        private System.Windows.Forms.Panel pnVendaLink_sub;
        private System.Windows.Forms.StatusStrip statusLinkVenda;
        private System.Windows.Forms.Panel pnVendaLink_Top;
        private System.Windows.Forms.DataGridView dgvLinkVenda;
        private System.Windows.Forms.GroupBox gbFiltro;
        private System.Windows.Forms.Label lbFazenda_Contabil;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txFazenda;
        private System.Windows.Forms.TextBox txCFOP;
        private System.Windows.Forms.Label lbPeriodo;
        private System.Windows.Forms.ToolStripButton toolStripSair;
        private System.Windows.Forms.ToolStripButton toolStripNfe;
        private System.Windows.Forms.ToolStripButton toolStripVenda;
        private System.Windows.Forms.ToolStripButton toolStripRecarregueDisponivel;
    }
}