
namespace ApoioContabilidade.Produção
{
    partial class FrmProd
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProd));
            this.pnTop = new System.Windows.Forms.Panel();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.btnNovo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnConsulta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRecalcula = new System.Windows.Forms.ToolStripButton();
            this.pnMeio = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.ssProdLote = new System.Windows.Forms.StatusStrip();
            this.pnProdLote = new System.Windows.Forms.Panel();
            this.dgvLote = new System.Windows.Forms.DataGridView();
            this.tcDetalhes = new System.Windows.Forms.TabControl();
            this.tpProdMov = new System.Windows.Forms.TabPage();
            this.ssProdMov = new System.Windows.Forms.StatusStrip();
            this.pnProdMov = new System.Windows.Forms.Panel();
            this.dgvProdMov = new System.Windows.Forms.DataGridView();
            this.tpProdRec = new System.Windows.Forms.TabPage();
            this.ssProdRec = new System.Windows.Forms.StatusStrip();
            this.pnProdRec = new System.Windows.Forms.Panel();
            this.dgvProdRec = new System.Windows.Forms.DataGridView();
            this.tpFruta = new System.Windows.Forms.TabPage();
            this.dgvFruta = new System.Windows.Forms.DataGridView();
            this.tpVendas = new System.Windows.Forms.TabPage();
            this.ssProdDestino = new System.Windows.Forms.StatusStrip();
            this.pnDestino = new System.Windows.Forms.Panel();
            this.dgvDestino = new System.Windows.Forms.DataGridView();
            this.tpAjustes = new System.Windows.Forms.TabPage();
            this.dgvSobra = new System.Windows.Forms.DataGridView();
            this.tsMenu.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.pnProdLote.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLote)).BeginInit();
            this.tcDetalhes.SuspendLayout();
            this.tpProdMov.SuspendLayout();
            this.pnProdMov.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdMov)).BeginInit();
            this.tpProdRec.SuspendLayout();
            this.pnProdRec.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdRec)).BeginInit();
            this.tpFruta.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFruta)).BeginInit();
            this.tpVendas.SuspendLayout();
            this.pnDestino.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDestino)).BeginInit();
            this.tpAjustes.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSobra)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1450, 21);
            this.pnTop.TabIndex = 0;
            // 
            // tsMenu
            // 
            this.tsMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNovo,
            this.toolStripSeparator1,
            this.btnConsulta,
            this.toolStripSeparator2,
            this.btnRecalcula});
            this.tsMenu.Location = new System.Drawing.Point(0, 21);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(1450, 27);
            this.tsMenu.TabIndex = 1;
            this.tsMenu.Text = "menu";
            // 
            // btnNovo
            // 
            this.btnNovo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNovo.Image = ((System.Drawing.Image)(resources.GetObject("btnNovo.Image")));
            this.btnNovo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNovo.Name = "btnNovo";
            this.btnNovo.Size = new System.Drawing.Size(49, 24);
            this.btnNovo.Text = "&Novo";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // btnConsulta
            // 
            this.btnConsulta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnConsulta.Image = ((System.Drawing.Image)(resources.GetObject("btnConsulta.Image")));
            this.btnConsulta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(70, 24);
            this.btnConsulta.Text = "&Consulta";
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // btnRecalcula
            // 
            this.btnRecalcula.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRecalcula.Image = ((System.Drawing.Image)(resources.GetObject("btnRecalcula.Image")));
            this.btnRecalcula.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRecalcula.Name = "btnRecalcula";
            this.btnRecalcula.Size = new System.Drawing.Size(76, 24);
            this.btnRecalcula.Text = "&Recalcula";
            this.btnRecalcula.Click += new System.EventHandler(this.btnRecalcula_Click);
            // 
            // pnMeio
            // 
            this.pnMeio.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnMeio.Location = new System.Drawing.Point(0, 48);
            this.pnMeio.Name = "pnMeio";
            this.pnMeio.Size = new System.Drawing.Size(1450, 16);
            this.pnMeio.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.ssProdLote);
            this.groupBox1.Controls.Add(this.pnProdLote);
            this.groupBox1.Location = new System.Drawing.Point(8, 70);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1425, 309);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Lotes";
            // 
            // ssProdLote
            // 
            this.ssProdLote.Dock = System.Windows.Forms.DockStyle.None;
            this.ssProdLote.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssProdLote.Location = new System.Drawing.Point(19, 273);
            this.ssProdLote.Name = "ssProdLote";
            this.ssProdLote.Size = new System.Drawing.Size(202, 22);
            this.ssProdLote.TabIndex = 1;
            this.ssProdLote.Text = "statusStrip1";
            // 
            // pnProdLote
            // 
            this.pnProdLote.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnProdLote.Controls.Add(this.dgvLote);
            this.pnProdLote.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnProdLote.Location = new System.Drawing.Point(3, 18);
            this.pnProdLote.Name = "pnProdLote";
            this.pnProdLote.Size = new System.Drawing.Size(1419, 257);
            this.pnProdLote.TabIndex = 0;
            // 
            // dgvLote
            // 
            this.dgvLote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLote.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLote.Location = new System.Drawing.Point(11, 8);
            this.dgvLote.Name = "dgvLote";
            this.dgvLote.RowHeadersWidth = 51;
            this.dgvLote.RowTemplate.Height = 24;
            this.dgvLote.Size = new System.Drawing.Size(1397, 229);
            this.dgvLote.TabIndex = 0;
            // 
            // tcDetalhes
            // 
            this.tcDetalhes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tcDetalhes.Controls.Add(this.tpProdMov);
            this.tcDetalhes.Controls.Add(this.tpProdRec);
            this.tcDetalhes.Controls.Add(this.tpFruta);
            this.tcDetalhes.Controls.Add(this.tpVendas);
            this.tcDetalhes.Controls.Add(this.tpAjustes);
            this.tcDetalhes.Location = new System.Drawing.Point(188, 393);
            this.tcDetalhes.Name = "tcDetalhes";
            this.tcDetalhes.SelectedIndex = 0;
            this.tcDetalhes.Size = new System.Drawing.Size(1233, 368);
            this.tcDetalhes.TabIndex = 4;
            // 
            // tpProdMov
            // 
            this.tpProdMov.Controls.Add(this.ssProdMov);
            this.tpProdMov.Controls.Add(this.pnProdMov);
            this.tpProdMov.Location = new System.Drawing.Point(4, 25);
            this.tpProdMov.Name = "tpProdMov";
            this.tpProdMov.Padding = new System.Windows.Forms.Padding(10);
            this.tpProdMov.Size = new System.Drawing.Size(1225, 339);
            this.tpProdMov.TabIndex = 0;
            this.tpProdMov.Text = "Produção em Natura(1)";
            this.tpProdMov.UseVisualStyleBackColor = true;
            this.tpProdMov.SizeChanged += new System.EventHandler(this.tpProdMov_SizeChanged);
            // 
            // ssProdMov
            // 
            this.ssProdMov.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ssProdMov.Dock = System.Windows.Forms.DockStyle.None;
            this.ssProdMov.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssProdMov.Location = new System.Drawing.Point(22, 310);
            this.ssProdMov.Name = "ssProdMov";
            this.ssProdMov.Size = new System.Drawing.Size(202, 22);
            this.ssProdMov.TabIndex = 1;
            this.ssProdMov.Text = "statusStrip1";
            // 
            // pnProdMov
            // 
            this.pnProdMov.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.pnProdMov.Controls.Add(this.dgvProdMov);
            this.pnProdMov.Location = new System.Drawing.Point(50, 2);
            this.pnProdMov.Name = "pnProdMov";
            this.pnProdMov.Size = new System.Drawing.Size(1164, 300);
            this.pnProdMov.TabIndex = 0;
            // 
            // dgvProdMov
            // 
            this.dgvProdMov.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvProdMov.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdMov.Location = new System.Drawing.Point(12, 11);
            this.dgvProdMov.Name = "dgvProdMov";
            this.dgvProdMov.RowHeadersWidth = 51;
            this.dgvProdMov.RowTemplate.Height = 24;
            this.dgvProdMov.Size = new System.Drawing.Size(1138, 273);
            this.dgvProdMov.TabIndex = 2;
            // 
            // tpProdRec
            // 
            this.tpProdRec.Controls.Add(this.ssProdRec);
            this.tpProdRec.Controls.Add(this.pnProdRec);
            this.tpProdRec.Location = new System.Drawing.Point(4, 25);
            this.tpProdRec.Name = "tpProdRec";
            this.tpProdRec.Padding = new System.Windows.Forms.Padding(3);
            this.tpProdRec.Size = new System.Drawing.Size(1225, 339);
            this.tpProdRec.TabIndex = 1;
            this.tpProdRec.Text = "Segue(2)";
            this.tpProdRec.UseVisualStyleBackColor = true;
            // 
            // ssProdRec
            // 
            this.ssProdRec.Dock = System.Windows.Forms.DockStyle.None;
            this.ssProdRec.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssProdRec.Location = new System.Drawing.Point(2, 313);
            this.ssProdRec.Name = "ssProdRec";
            this.ssProdRec.Size = new System.Drawing.Size(202, 22);
            this.ssProdRec.TabIndex = 2;
            this.ssProdRec.Text = "statusStrip1";
            // 
            // pnProdRec
            // 
            this.pnProdRec.Controls.Add(this.dgvProdRec);
            this.pnProdRec.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnProdRec.Location = new System.Drawing.Point(3, 3);
            this.pnProdRec.Name = "pnProdRec";
            this.pnProdRec.Size = new System.Drawing.Size(1219, 291);
            this.pnProdRec.TabIndex = 0;
            // 
            // dgvProdRec
            // 
            this.dgvProdRec.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProdRec.Location = new System.Drawing.Point(13, 8);
            this.dgvProdRec.Name = "dgvProdRec";
            this.dgvProdRec.RowHeadersWidth = 51;
            this.dgvProdRec.RowTemplate.Height = 24;
            this.dgvProdRec.Size = new System.Drawing.Size(1188, 270);
            this.dgvProdRec.TabIndex = 2;
            // 
            // tpFruta
            // 
            this.tpFruta.Controls.Add(this.dgvFruta);
            this.tpFruta.Location = new System.Drawing.Point(4, 25);
            this.tpFruta.Name = "tpFruta";
            this.tpFruta.Padding = new System.Windows.Forms.Padding(3);
            this.tpFruta.Size = new System.Drawing.Size(1225, 339);
            this.tpFruta.TabIndex = 2;
            this.tpFruta.Text = "Fruta Dalton";
            this.tpFruta.UseVisualStyleBackColor = true;
            // 
            // dgvFruta
            // 
            this.dgvFruta.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvFruta.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFruta.Location = new System.Drawing.Point(26, 13);
            this.dgvFruta.Name = "dgvFruta";
            this.dgvFruta.RowHeadersWidth = 51;
            this.dgvFruta.RowTemplate.Height = 24;
            this.dgvFruta.Size = new System.Drawing.Size(1152, 294);
            this.dgvFruta.TabIndex = 1;
            // 
            // tpVendas
            // 
            this.tpVendas.Controls.Add(this.ssProdDestino);
            this.tpVendas.Controls.Add(this.pnDestino);
            this.tpVendas.Location = new System.Drawing.Point(4, 25);
            this.tpVendas.Name = "tpVendas";
            this.tpVendas.Padding = new System.Windows.Forms.Padding(3);
            this.tpVendas.Size = new System.Drawing.Size(1225, 339);
            this.tpVendas.TabIndex = 3;
            this.tpVendas.Text = "Vendas";
            this.tpVendas.UseVisualStyleBackColor = true;
            // 
            // ssProdDestino
            // 
            this.ssProdDestino.Dock = System.Windows.Forms.DockStyle.None;
            this.ssProdDestino.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssProdDestino.Location = new System.Drawing.Point(2, 309);
            this.ssProdDestino.Name = "ssProdDestino";
            this.ssProdDestino.Size = new System.Drawing.Size(202, 22);
            this.ssProdDestino.TabIndex = 3;
            this.ssProdDestino.Text = "statusStrip1";
            // 
            // pnDestino
            // 
            this.pnDestino.Controls.Add(this.dgvDestino);
            this.pnDestino.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnDestino.Location = new System.Drawing.Point(3, 3);
            this.pnDestino.Name = "pnDestino";
            this.pnDestino.Size = new System.Drawing.Size(1219, 290);
            this.pnDestino.TabIndex = 0;
            // 
            // dgvDestino
            // 
            this.dgvDestino.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDestino.Location = new System.Drawing.Point(10, 12);
            this.dgvDestino.Name = "dgvDestino";
            this.dgvDestino.RowHeadersWidth = 51;
            this.dgvDestino.RowTemplate.Height = 24;
            this.dgvDestino.Size = new System.Drawing.Size(1190, 266);
            this.dgvDestino.TabIndex = 2;
            // 
            // tpAjustes
            // 
            this.tpAjustes.Controls.Add(this.dgvSobra);
            this.tpAjustes.Location = new System.Drawing.Point(4, 25);
            this.tpAjustes.Name = "tpAjustes";
            this.tpAjustes.Padding = new System.Windows.Forms.Padding(3);
            this.tpAjustes.Size = new System.Drawing.Size(1225, 339);
            this.tpAjustes.TabIndex = 4;
            this.tpAjustes.Text = "Ajustes Internos";
            this.tpAjustes.UseVisualStyleBackColor = true;
            // 
            // dgvSobra
            // 
            this.dgvSobra.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSobra.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSobra.Location = new System.Drawing.Point(10, 12);
            this.dgvSobra.Name = "dgvSobra";
            this.dgvSobra.RowHeadersWidth = 51;
            this.dgvSobra.RowTemplate.Height = 24;
            this.dgvSobra.Size = new System.Drawing.Size(1203, 307);
            this.dgvSobra.TabIndex = 12;
            // 
            // FrmProd
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1450, 784);
            this.Controls.Add(this.tcDetalhes);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pnMeio);
            this.Controls.Add(this.tsMenu);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmProd";
            this.Text = "FrmProd";
            this.Load += new System.EventHandler(this.FrmProd_Load);
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.pnProdLote.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLote)).EndInit();
            this.tcDetalhes.ResumeLayout(false);
            this.tpProdMov.ResumeLayout(false);
            this.tpProdMov.PerformLayout();
            this.pnProdMov.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdMov)).EndInit();
            this.tpProdRec.ResumeLayout(false);
            this.tpProdRec.PerformLayout();
            this.pnProdRec.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvProdRec)).EndInit();
            this.tpFruta.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFruta)).EndInit();
            this.tpVendas.ResumeLayout(false);
            this.tpVendas.PerformLayout();
            this.pnDestino.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDestino)).EndInit();
            this.tpAjustes.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSobra)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Panel pnTop;
        public System.Windows.Forms.ToolStrip tsMenu;
        public System.Windows.Forms.ToolStripButton btnNovo;
        public System.Windows.Forms.Panel pnMeio;
        public System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.Panel pnProdLote;
        public System.Windows.Forms.DataGridView dgvLote;
        public System.Windows.Forms.TabControl tcDetalhes;
        public System.Windows.Forms.TabPage tpProdMov;
        public System.Windows.Forms.TabPage tpProdRec;
        public System.Windows.Forms.TabPage tpFruta;
        public System.Windows.Forms.DataGridView dgvFruta;
        public System.Windows.Forms.TabPage tpVendas;
        public System.Windows.Forms.TabPage tpAjustes;
        public System.Windows.Forms.DataGridView dgvSobra;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        public System.Windows.Forms.ToolStripButton btnConsulta;
        public System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        public System.Windows.Forms.ToolStripButton btnRecalcula;
        private System.Windows.Forms.StatusStrip ssProdLote;
        private System.Windows.Forms.StatusStrip ssProdMov;
        private System.Windows.Forms.Panel pnProdMov;
        public System.Windows.Forms.DataGridView dgvProdMov;
        private System.Windows.Forms.StatusStrip ssProdRec;
        private System.Windows.Forms.Panel pnProdRec;
        public System.Windows.Forms.DataGridView dgvProdRec;
        private System.Windows.Forms.StatusStrip ssProdDestino;
        private System.Windows.Forms.Panel pnDestino;
        public System.Windows.Forms.DataGridView dgvDestino;
    }
}