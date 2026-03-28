
namespace ApoioContabilidade.Trabalho
{
    partial class FrmRelTrabalhista
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
            this.pnTOPTOP = new System.Windows.Forms.Panel();
            this.btnRecibos = new System.Windows.Forms.Button();
            this.btnExpFinan = new System.Windows.Forms.Button();
            this.btnExpBB = new System.Windows.Forms.Button();
            this.pnFiltro = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.txNome = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txSetores = new System.Windows.Forms.TextBox();
            this.txTrabalhador = new System.Windows.Forms.TextBox();
            this.rbImpostoSind = new System.Windows.Forms.RadioButton();
            this.pnTop = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbContaTodos = new System.Windows.Forms.RadioButton();
            this.rbContaSem = new System.Windows.Forms.RadioButton();
            this.rbContaCom = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbSortAlfaGeral = new System.Windows.Forms.RadioButton();
            this.rbSortAlfaSetor = new System.Windows.Forms.RadioButton();
            this.rbSortCodigo = new System.Windows.Forms.RadioButton();
            this.gbDiaristas = new System.Windows.Forms.GroupBox();
            this.rbTodos = new System.Windows.Forms.RadioButton();
            this.rbDiaristas = new System.Windows.Forms.RadioButton();
            this.rbMensalistas = new System.Windows.Forms.RadioButton();
            this.btnFolha = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.dtCheque = new System.Windows.Forms.DateTimePicker();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.printDocument2 = new System.Drawing.Printing.PrintDocument();
            this.printDialog2 = new System.Windows.Forms.PrintDialog();
            this.pnRelatorio = new System.Windows.Forms.Panel();
            this.pnRodape = new System.Windows.Forms.Panel();
            this.tsCabecalho = new System.Windows.Forms.ToolStrip();
            this.tsTotais = new System.Windows.Forms.ToolStrip();
            this.tcFolha = new System.Windows.Forms.TabControl();
            this.tpFolha = new System.Windows.Forms.TabPage();
            this.dgvFolha = new System.Windows.Forms.DataGridView();
            this.tpBanco = new System.Windows.Forms.TabPage();
            this.dgvBanco = new System.Windows.Forms.DataGridView();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.pnTOPTOP.SuspendLayout();
            this.pnFiltro.SuspendLayout();
            this.pnTop.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.gbDiaristas.SuspendLayout();
            this.pnRelatorio.SuspendLayout();
            this.pnRodape.SuspendLayout();
            this.tcFolha.SuspendLayout();
            this.tpFolha.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolha)).BeginInit();
            this.tpBanco.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvBanco)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTOPTOP
            // 
            this.pnTOPTOP.Controls.Add(this.btnRecibos);
            this.pnTOPTOP.Controls.Add(this.btnExpFinan);
            this.pnTOPTOP.Controls.Add(this.btnExpBB);
            this.pnTOPTOP.Controls.Add(this.pnFiltro);
            this.pnTOPTOP.Controls.Add(this.rbImpostoSind);
            this.pnTOPTOP.Controls.Add(this.pnTop);
            this.pnTOPTOP.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTOPTOP.Location = new System.Drawing.Point(0, 0);
            this.pnTOPTOP.Name = "pnTOPTOP";
            this.pnTOPTOP.Size = new System.Drawing.Size(1570, 139);
            this.pnTOPTOP.TabIndex = 2;
            // 
            // btnRecibos
            // 
            this.btnRecibos.Location = new System.Drawing.Point(895, 98);
            this.btnRecibos.Name = "btnRecibos";
            this.btnRecibos.Size = new System.Drawing.Size(131, 23);
            this.btnRecibos.TabIndex = 12;
            this.btnRecibos.Text = "Contra-Cheque";
            this.btnRecibos.UseVisualStyleBackColor = true;
            this.btnRecibos.Click += new System.EventHandler(this.btnRecibos_Click);
            // 
            // btnExpFinan
            // 
            this.btnExpFinan.Location = new System.Drawing.Point(743, 98);
            this.btnExpFinan.Name = "btnExpFinan";
            this.btnExpFinan.Size = new System.Drawing.Size(145, 24);
            this.btnExpFinan.TabIndex = 11;
            this.btnExpFinan.TabStop = false;
            this.btnExpFinan.Text = "Exporta Financeiro";
            this.btnExpFinan.UseVisualStyleBackColor = true;
            this.btnExpFinan.Click += new System.EventHandler(this.btnExpFinan_Click);
            // 
            // btnExpBB
            // 
            this.btnExpBB.Location = new System.Drawing.Point(635, 98);
            this.btnExpBB.Name = "btnExpBB";
            this.btnExpBB.Size = new System.Drawing.Size(97, 24);
            this.btnExpBB.TabIndex = 10;
            this.btnExpBB.Text = "Exporta BB";
            this.btnExpBB.UseVisualStyleBackColor = true;
            this.btnExpBB.Click += new System.EventHandler(this.btnExpBB_Click);
            // 
            // pnFiltro
            // 
            this.pnFiltro.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pnFiltro.Controls.Add(this.label3);
            this.pnFiltro.Controls.Add(this.txNome);
            this.pnFiltro.Controls.Add(this.label2);
            this.pnFiltro.Controls.Add(this.label4);
            this.pnFiltro.Controls.Add(this.txSetores);
            this.pnFiltro.Controls.Add(this.txTrabalhador);
            this.pnFiltro.Enabled = false;
            this.pnFiltro.Location = new System.Drawing.Point(622, 6);
            this.pnFiltro.Name = "pnFiltro";
            this.pnFiltro.Size = new System.Drawing.Size(901, 79);
            this.pnFiltro.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(36, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 17);
            this.label3.TabIndex = 9;
            this.label3.Text = "Nome Trabalhador";
            // 
            // txNome
            // 
            this.txNome.CharacterCasing = System.Windows.Forms.CharacterCasing.Upper;
            this.txNome.Location = new System.Drawing.Point(34, 30);
            this.txNome.MaxLength = 40;
            this.txNome.Name = "txNome";
            this.txNome.Size = new System.Drawing.Size(379, 22);
            this.txNome.TabIndex = 8;
            this.txNome.TextChanged += new System.EventHandler(this.txNome_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(448, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 17);
            this.label2.TabIndex = 7;
            this.label2.Text = "Setores";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(641, 8);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Código(s) Trab.";
            // 
            // txSetores
            // 
            this.txSetores.Location = new System.Drawing.Point(445, 30);
            this.txSetores.Name = "txSetores";
            this.txSetores.Size = new System.Drawing.Size(181, 22);
            this.txSetores.TabIndex = 5;
            this.txSetores.TextChanged += new System.EventHandler(this.txSetores_TextChanged);
            // 
            // txTrabalhador
            // 
            this.txTrabalhador.Location = new System.Drawing.Point(639, 29);
            this.txTrabalhador.Name = "txTrabalhador";
            this.txTrabalhador.Size = new System.Drawing.Size(242, 22);
            this.txTrabalhador.TabIndex = 4;
            this.txTrabalhador.TextChanged += new System.EventHandler(this.txTrabalhador_TextChanged);
            // 
            // rbImpostoSind
            // 
            this.rbImpostoSind.AutoSize = true;
            this.rbImpostoSind.Enabled = false;
            this.rbImpostoSind.Location = new System.Drawing.Point(1037, 100);
            this.rbImpostoSind.Name = "rbImpostoSind";
            this.rbImpostoSind.Size = new System.Drawing.Size(131, 21);
            this.rbImpostoSind.TabIndex = 4;
            this.rbImpostoSind.TabStop = true;
            this.rbImpostoSind.Text = "Imposto Sindical";
            this.rbImpostoSind.UseVisualStyleBackColor = true;
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.groupBox2);
            this.pnTop.Controls.Add(this.groupBox1);
            this.pnTop.Controls.Add(this.gbDiaristas);
            this.pnTop.Controls.Add(this.btnFolha);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.dtCheque);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.dtData1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(619, 139);
            this.pnTop.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbContaTodos);
            this.groupBox2.Controls.Add(this.rbContaSem);
            this.groupBox2.Controls.Add(this.rbContaCom);
            this.groupBox2.Location = new System.Drawing.Point(272, 70);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox2.Size = new System.Drawing.Size(335, 36);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            // 
            // rbContaTodos
            // 
            this.rbContaTodos.AutoSize = true;
            this.rbContaTodos.Checked = true;
            this.rbContaTodos.Location = new System.Drawing.Point(230, 11);
            this.rbContaTodos.Name = "rbContaTodos";
            this.rbContaTodos.Size = new System.Drawing.Size(69, 21);
            this.rbContaTodos.TabIndex = 2;
            this.rbContaTodos.TabStop = true;
            this.rbContaTodos.Text = "Todos";
            this.rbContaTodos.UseVisualStyleBackColor = true;
            this.rbContaTodos.CheckedChanged += new System.EventHandler(this.rbContaTodos_CheckedChanged);
            // 
            // rbContaSem
            // 
            this.rbContaSem.AutoSize = true;
            this.rbContaSem.Location = new System.Drawing.Point(118, 11);
            this.rbContaSem.Name = "rbContaSem";
            this.rbContaSem.Size = new System.Drawing.Size(79, 21);
            this.rbContaSem.TabIndex = 1;
            this.rbContaSem.Text = "S/Conta";
            this.rbContaSem.UseVisualStyleBackColor = true;
            this.rbContaSem.CheckedChanged += new System.EventHandler(this.rbContaTodos_CheckedChanged);
            // 
            // rbContaCom
            // 
            this.rbContaCom.AutoSize = true;
            this.rbContaCom.Location = new System.Drawing.Point(22, 11);
            this.rbContaCom.Name = "rbContaCom";
            this.rbContaCom.Size = new System.Drawing.Size(79, 21);
            this.rbContaCom.TabIndex = 0;
            this.rbContaCom.Text = "C/Conta";
            this.rbContaCom.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbSortAlfaGeral);
            this.groupBox1.Controls.Add(this.rbSortAlfaSetor);
            this.groupBox1.Controls.Add(this.rbSortCodigo);
            this.groupBox1.Location = new System.Drawing.Point(267, 35);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(0);
            this.groupBox1.Size = new System.Drawing.Size(338, 37);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            // 
            // rbSortAlfaGeral
            // 
            this.rbSortAlfaGeral.AutoSize = true;
            this.rbSortAlfaGeral.Checked = true;
            this.rbSortAlfaGeral.Location = new System.Drawing.Point(230, 11);
            this.rbSortAlfaGeral.Name = "rbSortAlfaGeral";
            this.rbSortAlfaGeral.Size = new System.Drawing.Size(98, 21);
            this.rbSortAlfaGeral.TabIndex = 2;
            this.rbSortAlfaGeral.TabStop = true;
            this.rbSortAlfaGeral.Text = "Alfa(Geral)";
            this.rbSortAlfaGeral.UseVisualStyleBackColor = true;
            this.rbSortAlfaGeral.CheckedChanged += new System.EventHandler(this.rbSortAlfaGeral_CheckedChanged);
            // 
            // rbSortAlfaSetor
            // 
            this.rbSortAlfaSetor.AutoSize = true;
            this.rbSortAlfaSetor.Location = new System.Drawing.Point(118, 11);
            this.rbSortAlfaSetor.Name = "rbSortAlfaSetor";
            this.rbSortAlfaSetor.Size = new System.Drawing.Size(95, 21);
            this.rbSortAlfaSetor.TabIndex = 1;
            this.rbSortAlfaSetor.Text = "Alfa(setor)";
            this.rbSortAlfaSetor.UseVisualStyleBackColor = true;
            this.rbSortAlfaSetor.CheckedChanged += new System.EventHandler(this.rbSortAlfaGeral_CheckedChanged);
            // 
            // rbSortCodigo
            // 
            this.rbSortCodigo.AutoSize = true;
            this.rbSortCodigo.Location = new System.Drawing.Point(28, 11);
            this.rbSortCodigo.Name = "rbSortCodigo";
            this.rbSortCodigo.Size = new System.Drawing.Size(73, 21);
            this.rbSortCodigo.TabIndex = 0;
            this.rbSortCodigo.Text = "Código";
            this.rbSortCodigo.UseVisualStyleBackColor = true;
            this.rbSortCodigo.CheckedChanged += new System.EventHandler(this.rbSortAlfaGeral_CheckedChanged);
            // 
            // gbDiaristas
            // 
            this.gbDiaristas.Controls.Add(this.rbTodos);
            this.gbDiaristas.Controls.Add(this.rbDiaristas);
            this.gbDiaristas.Controls.Add(this.rbMensalistas);
            this.gbDiaristas.Location = new System.Drawing.Point(266, -2);
            this.gbDiaristas.Margin = new System.Windows.Forms.Padding(0);
            this.gbDiaristas.Name = "gbDiaristas";
            this.gbDiaristas.Padding = new System.Windows.Forms.Padding(0);
            this.gbDiaristas.Size = new System.Drawing.Size(338, 37);
            this.gbDiaristas.TabIndex = 6;
            this.gbDiaristas.TabStop = false;
            // 
            // rbTodos
            // 
            this.rbTodos.AutoSize = true;
            this.rbTodos.Checked = true;
            this.rbTodos.Location = new System.Drawing.Point(260, 11);
            this.rbTodos.Name = "rbTodos";
            this.rbTodos.Size = new System.Drawing.Size(69, 21);
            this.rbTodos.TabIndex = 2;
            this.rbTodos.TabStop = true;
            this.rbTodos.Text = "Todos";
            this.rbTodos.UseVisualStyleBackColor = true;
            this.rbTodos.CheckedChanged += new System.EventHandler(this.rbTodos_CheckedChanged);
            // 
            // rbDiaristas
            // 
            this.rbDiaristas.AutoSize = true;
            this.rbDiaristas.Location = new System.Drawing.Point(144, 13);
            this.rbDiaristas.Name = "rbDiaristas";
            this.rbDiaristas.Size = new System.Drawing.Size(84, 21);
            this.rbDiaristas.TabIndex = 1;
            this.rbDiaristas.Text = "Diaristas";
            this.rbDiaristas.UseVisualStyleBackColor = true;
            this.rbDiaristas.CheckedChanged += new System.EventHandler(this.rbTodos_CheckedChanged);
            // 
            // rbMensalistas
            // 
            this.rbMensalistas.AutoSize = true;
            this.rbMensalistas.Location = new System.Drawing.Point(12, 12);
            this.rbMensalistas.Name = "rbMensalistas";
            this.rbMensalistas.Size = new System.Drawing.Size(103, 21);
            this.rbMensalistas.TabIndex = 0;
            this.rbMensalistas.Text = "Mensalistas";
            this.rbMensalistas.UseVisualStyleBackColor = true;
            this.rbMensalistas.CheckedChanged += new System.EventHandler(this.rbTodos_CheckedChanged);
            // 
            // btnFolha
            // 
            this.btnFolha.Location = new System.Drawing.Point(129, 62);
            this.btnFolha.Name = "btnFolha";
            this.btnFolha.Size = new System.Drawing.Size(75, 29);
            this.btnFolha.TabIndex = 5;
            this.btnFolha.Text = "&Folha";
            this.btnFolha.UseVisualStyleBackColor = true;
            this.btnFolha.Click += new System.EventHandler(this.btnFolha_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(129, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(118, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Data Pagamento:";
            // 
            // dtCheque
            // 
            this.dtCheque.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtCheque.Location = new System.Drawing.Point(129, 26);
            this.dtCheque.Name = "dtCheque";
            this.dtCheque.Size = new System.Drawing.Size(102, 22);
            this.dtCheque.TabIndex = 2;
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(28, 62);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(75, 30);
            this.btnConsulta.TabIndex = 1;
            this.btnConsulta.Text = "&Consulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // dtData1
            // 
            this.dtData1.CustomFormat = "MM/yyyy";
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtData1.Location = new System.Drawing.Point(28, 24);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(82, 22);
            this.dtData1.TabIndex = 0;
            // 
            // printDialog2
            // 
            this.printDialog2.UseEXDialog = true;
            // 
            // pnRelatorio
            // 
            this.pnRelatorio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnRelatorio.Controls.Add(this.pnRodape);
            this.pnRelatorio.Controls.Add(this.tcFolha);
            this.pnRelatorio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnRelatorio.Location = new System.Drawing.Point(0, 139);
            this.pnRelatorio.Name = "pnRelatorio";
            this.pnRelatorio.Size = new System.Drawing.Size(1570, 662);
            this.pnRelatorio.TabIndex = 3;
            // 
            // pnRodape
            // 
            this.pnRodape.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnRodape.Controls.Add(this.tsCabecalho);
            this.pnRodape.Controls.Add(this.tsTotais);
            this.pnRodape.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnRodape.Location = new System.Drawing.Point(0, 596);
            this.pnRodape.Name = "pnRodape";
            this.pnRodape.Size = new System.Drawing.Size(1568, 64);
            this.pnRodape.TabIndex = 6;
            // 
            // tsCabecalho
            // 
            this.tsCabecalho.Dock = System.Windows.Forms.DockStyle.None;
            this.tsCabecalho.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsCabecalho.Location = new System.Drawing.Point(8, 8);
            this.tsCabecalho.Name = "tsCabecalho";
            this.tsCabecalho.Size = new System.Drawing.Size(112, 25);
            this.tsCabecalho.TabIndex = 8;
            this.tsCabecalho.Text = "toolStrip1";
            // 
            // tsTotais
            // 
            this.tsTotais.Dock = System.Windows.Forms.DockStyle.None;
            this.tsTotais.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTotais.Location = new System.Drawing.Point(7, 33);
            this.tsTotais.Name = "tsTotais";
            this.tsTotais.Size = new System.Drawing.Size(112, 25);
            this.tsTotais.TabIndex = 7;
            this.tsTotais.Text = "toolStrip1";
            // 
            // tcFolha
            // 
            this.tcFolha.Controls.Add(this.tpFolha);
            this.tcFolha.Controls.Add(this.tpBanco);
            this.tcFolha.Dock = System.Windows.Forms.DockStyle.Top;
            this.tcFolha.Location = new System.Drawing.Point(0, 0);
            this.tcFolha.Name = "tcFolha";
            this.tcFolha.SelectedIndex = 0;
            this.tcFolha.Size = new System.Drawing.Size(1568, 646);
            this.tcFolha.TabIndex = 0;
            // 
            // tpFolha
            // 
            this.tpFolha.Controls.Add(this.dgvFolha);
            this.tpFolha.Location = new System.Drawing.Point(4, 25);
            this.tpFolha.Name = "tpFolha";
            this.tpFolha.Padding = new System.Windows.Forms.Padding(3);
            this.tpFolha.Size = new System.Drawing.Size(1560, 617);
            this.tpFolha.TabIndex = 0;
            this.tpFolha.Text = "Folha";
            this.tpFolha.UseVisualStyleBackColor = true;
            // 
            // dgvFolha
            // 
            this.dgvFolha.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFolha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFolha.Location = new System.Drawing.Point(3, 3);
            this.dgvFolha.Name = "dgvFolha";
            this.dgvFolha.RowHeadersWidth = 51;
            this.dgvFolha.RowTemplate.Height = 24;
            this.dgvFolha.Size = new System.Drawing.Size(1554, 611);
            this.dgvFolha.TabIndex = 0;
            // 
            // tpBanco
            // 
            this.tpBanco.Controls.Add(this.dgvBanco);
            this.tpBanco.Location = new System.Drawing.Point(4, 25);
            this.tpBanco.Name = "tpBanco";
            this.tpBanco.Padding = new System.Windows.Forms.Padding(3);
            this.tpBanco.Size = new System.Drawing.Size(1560, 617);
            this.tpBanco.TabIndex = 1;
            this.tpBanco.Text = "Banco";
            this.tpBanco.UseVisualStyleBackColor = true;
            // 
            // dgvBanco
            // 
            this.dgvBanco.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvBanco.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvBanco.Location = new System.Drawing.Point(3, 3);
            this.dgvBanco.Name = "dgvBanco";
            this.dgvBanco.RowHeadersWidth = 51;
            this.dgvBanco.RowTemplate.Height = 24;
            this.dgvBanco.Size = new System.Drawing.Size(1554, 611);
            this.dgvBanco.TabIndex = 1;
            // 
            // FrmRelTrabalhista
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1570, 801);
            this.Controls.Add(this.pnRelatorio);
            this.Controls.Add(this.pnTOPTOP);
            this.Name = "FrmRelTrabalhista";
            this.Text = "FrmRelTrabalhista";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.pnTOPTOP.ResumeLayout(false);
            this.pnTOPTOP.PerformLayout();
            this.pnFiltro.ResumeLayout(false);
            this.pnFiltro.PerformLayout();
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbDiaristas.ResumeLayout(false);
            this.gbDiaristas.PerformLayout();
            this.pnRelatorio.ResumeLayout(false);
            this.pnRodape.ResumeLayout(false);
            this.pnRodape.PerformLayout();
            this.tcFolha.ResumeLayout(false);
            this.tpFolha.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolha)).EndInit();
            this.tpBanco.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvBanco)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnTOPTOP;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.DateTimePicker dtCheque;
        private System.Windows.Forms.Label label1;
        private System.Drawing.Printing.PrintDocument printDocument2;
        private System.Windows.Forms.PrintDialog printDialog2;
        private System.Windows.Forms.Button btnFolha;
        private System.Windows.Forms.Panel pnRelatorio;
        private System.Windows.Forms.Panel pnRodape;
        private System.Windows.Forms.TabControl tcFolha;
        private System.Windows.Forms.TabPage tpFolha;
        private System.Windows.Forms.DataGridView dgvFolha;
        private System.Windows.Forms.TabPage tpBanco;
        private System.Windows.Forms.ToolStrip tsTotais;
        private System.Windows.Forms.ToolStrip tsCabecalho;
        private System.Windows.Forms.GroupBox gbDiaristas;
        private System.Windows.Forms.RadioButton rbTodos;
        private System.Windows.Forms.RadioButton rbDiaristas;
        private System.Windows.Forms.RadioButton rbMensalistas;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbSortAlfaGeral;
        private System.Windows.Forms.RadioButton rbSortAlfaSetor;
        private System.Windows.Forms.RadioButton rbSortCodigo;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbContaTodos;
        private System.Windows.Forms.RadioButton rbContaSem;
        private System.Windows.Forms.RadioButton rbContaCom;
        private System.Windows.Forms.Panel pnFiltro;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txNome;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txSetores;
        private System.Windows.Forms.TextBox txTrabalhador;
        private System.Windows.Forms.DataGridView dgvBanco;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Button btnExpBB;
        private System.Windows.Forms.Button btnExpFinan;
        private System.Windows.Forms.Button btnRecibos;
        private System.Windows.Forms.RadioButton rbImpostoSind;
    }
}