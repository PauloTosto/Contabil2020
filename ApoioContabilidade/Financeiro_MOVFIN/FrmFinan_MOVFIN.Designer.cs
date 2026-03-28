using ApoioContabilidade.Financeiro;

namespace ApoioContabilidade.Financeiro_MOVFIN
{
    partial class FrmFinan_MOVFIN
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmFinan_MOVFIN));
            this.toolMenu = new System.Windows.Forms.ToolStrip();
            this.toolSair = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCheques = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.toolImprime = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolNovo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
            this.toolDeleta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.toolEdita = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.tsConsulta = new System.Windows.Forms.ToolStripButton();
            this.gbEntradas = new System.Windows.Forms.GroupBox();
            this.ssReceber = new System.Windows.Forms.StatusStrip();
            this.pnEntradas = new System.Windows.Forms.Panel();
            this.dgvReceber = new System.Windows.Forms.DataGridView();
            this.gbSaidas = new System.Windows.Forms.GroupBox();
            this.ssPagar = new System.Windows.Forms.StatusStrip();
            this.pnSaidas = new System.Windows.Forms.Panel();
            this.dgvPagar = new System.Windows.Forms.DataGridView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.bindingNavFin = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem1 = new System.Windows.Forms.ToolStripLabel();
            this.toolSaldo = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolTitulo = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorMoveFirstItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem1 = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem1 = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.toolBtnEdite = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolComboPaginas = new System.Windows.Forms.ToolStripComboBox();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.ContextMenuPop = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItemAprop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator8 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItemTransfere = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.toolMenu.SuspendLayout();
            this.gbEntradas.SuspendLayout();
            this.pnEntradas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceber)).BeginInit();
            this.gbSaidas.SuspendLayout();
            this.pnSaidas.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavFin)).BeginInit();
            this.bindingNavFin.SuspendLayout();
            this.ContextMenuPop.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolMenu
            // 
            this.toolMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSair,
            this.toolStripSeparator1,
            this.toolCheques,
            this.toolStripSeparator4,
            this.toolImprime,
            this.toolStripSeparator5,
            this.toolNovo,
            this.toolStripSeparator6,
            this.toolDeleta,
            this.toolStripSeparator7,
            this.toolEdita,
            this.toolStripSeparator9,
            this.tsConsulta});
            this.toolMenu.Location = new System.Drawing.Point(0, 0);
            this.toolMenu.Name = "toolMenu";
            this.toolMenu.Size = new System.Drawing.Size(1485, 27);
            this.toolMenu.TabIndex = 0;
            // 
            // toolSair
            // 
            this.toolSair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolSair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSair.Name = "toolSair";
            this.toolSair.Size = new System.Drawing.Size(38, 24);
            this.toolSair.Text = "&Sair";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // toolCheques
            // 
            this.toolCheques.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolCheques.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolCheques.Name = "toolCheques";
            this.toolCheques.Size = new System.Drawing.Size(137, 24);
            this.toolCheques.Text = "Che&ques e Recibos";
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // toolImprime
            // 
            this.toolImprime.AutoSize = false;
            this.toolImprime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolImprime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolImprime.Name = "toolImprime";
            this.toolImprime.Size = new System.Drawing.Size(65, 24);
            this.toolImprime.Text = "&Imprime";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // toolNovo
            // 
            this.toolNovo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolNovo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolNovo.Name = "toolNovo";
            this.toolNovo.Size = new System.Drawing.Size(49, 24);
            this.toolNovo.Text = "&Novo";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 27);
            // 
            // toolDeleta
            // 
            this.toolDeleta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolDeleta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDeleta.Name = "toolDeleta";
            this.toolDeleta.Size = new System.Drawing.Size(57, 24);
            this.toolDeleta.Text = "&Deleta";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 27);
            // 
            // toolEdita
            // 
            this.toolEdita.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolEdita.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolEdita.Name = "toolEdita";
            this.toolEdita.Size = new System.Drawing.Size(47, 24);
            this.toolEdita.Text = "&Edita";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 27);
            // 
            // tsConsulta
            // 
            this.tsConsulta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsConsulta.Image = ((System.Drawing.Image)(resources.GetObject("tsConsulta.Image")));
            this.tsConsulta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsConsulta.Name = "tsConsulta";
            this.tsConsulta.Size = new System.Drawing.Size(70, 24);
            this.tsConsulta.Text = "&Consulta";
            this.tsConsulta.Click += new System.EventHandler(this.tsConsulta_Click);
            // 
            // gbEntradas
            // 
            this.gbEntradas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEntradas.Controls.Add(this.ssReceber);
            this.gbEntradas.Controls.Add(this.pnEntradas);
            this.gbEntradas.Location = new System.Drawing.Point(6, 42);
            this.gbEntradas.Name = "gbEntradas";
            this.gbEntradas.Size = new System.Drawing.Size(1450, 367);
            this.gbEntradas.TabIndex = 1;
            this.gbEntradas.TabStop = false;
            this.gbEntradas.Text = "ENTRADAS";
            // 
            // ssReceber
            // 
            this.ssReceber.Dock = System.Windows.Forms.DockStyle.None;
            this.ssReceber.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssReceber.Location = new System.Drawing.Point(26, 337);
            this.ssReceber.Name = "ssReceber";
            this.ssReceber.Size = new System.Drawing.Size(202, 22);
            this.ssReceber.TabIndex = 12;
            // 
            // pnEntradas
            // 
            this.pnEntradas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnEntradas.Controls.Add(this.dgvReceber);
            this.pnEntradas.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnEntradas.Location = new System.Drawing.Point(3, 18);
            this.pnEntradas.Name = "pnEntradas";
            this.pnEntradas.Size = new System.Drawing.Size(1444, 313);
            this.pnEntradas.TabIndex = 11;
            // 
            // dgvReceber
            // 
            this.dgvReceber.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvReceber.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvReceber.Location = new System.Drawing.Point(11, 11);
            this.dgvReceber.Name = "dgvReceber";
            this.dgvReceber.RowHeadersWidth = 51;
            this.dgvReceber.RowTemplate.Height = 24;
            this.dgvReceber.Size = new System.Drawing.Size(1420, 286);
            this.dgvReceber.TabIndex = 11;
            // 
            // gbSaidas
            // 
            this.gbSaidas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbSaidas.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.gbSaidas.Controls.Add(this.ssPagar);
            this.gbSaidas.Controls.Add(this.pnSaidas);
            this.gbSaidas.Location = new System.Drawing.Point(9, 417);
            this.gbSaidas.Name = "gbSaidas";
            this.gbSaidas.Size = new System.Drawing.Size(1450, 367);
            this.gbSaidas.TabIndex = 4;
            this.gbSaidas.TabStop = false;
            this.gbSaidas.Text = "SAÍDAS";
            // 
            // ssPagar
            // 
            this.ssPagar.Dock = System.Windows.Forms.DockStyle.None;
            this.ssPagar.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssPagar.Location = new System.Drawing.Point(26, 337);
            this.ssPagar.Name = "ssPagar";
            this.ssPagar.Size = new System.Drawing.Size(202, 22);
            this.ssPagar.TabIndex = 12;
            this.ssPagar.Text = "statusStrip1";
            // 
            // pnSaidas
            // 
            this.pnSaidas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnSaidas.Controls.Add(this.dgvPagar);
            this.pnSaidas.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnSaidas.Location = new System.Drawing.Point(3, 18);
            this.pnSaidas.Name = "pnSaidas";
            this.pnSaidas.Size = new System.Drawing.Size(1444, 313);
            this.pnSaidas.TabIndex = 11;
            // 
            // dgvPagar
            // 
            this.dgvPagar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvPagar.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvPagar.Location = new System.Drawing.Point(11, 11);
            this.dgvPagar.Name = "dgvPagar";
            this.dgvPagar.RowHeadersWidth = 51;
            this.dgvPagar.Size = new System.Drawing.Size(1420, 286);
            this.dgvPagar.TabIndex = 11;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // bindingNavFin
            // 
            this.bindingNavFin.AddNewItem = null;
            this.bindingNavFin.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.bindingNavFin.AutoSize = false;
            this.bindingNavFin.CountItem = this.bindingNavigatorCountItem1;
            this.bindingNavFin.DeleteItem = null;
            this.bindingNavFin.Dock = System.Windows.Forms.DockStyle.None;
            this.bindingNavFin.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.bindingNavFin.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolSaldo,
            this.toolStripSeparator3,
            this.toolTitulo,
            this.bindingNavigatorMoveFirstItem1,
            this.bindingNavigatorMovePreviousItem1,
            this.bindingNavigatorSeparator3,
            this.bindingNavigatorPositionItem1,
            this.bindingNavigatorCountItem1,
            this.bindingNavigatorSeparator4,
            this.bindingNavigatorMoveNextItem1,
            this.bindingNavigatorMoveLastItem1,
            this.bindingNavigatorSeparator5,
            this.toolBtnEdite,
            this.toolStripSeparator2,
            this.toolComboPaginas});
            this.bindingNavFin.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.bindingNavFin.Location = new System.Drawing.Point(14, 792);
            this.bindingNavFin.MoveFirstItem = this.bindingNavigatorMoveFirstItem1;
            this.bindingNavFin.MoveLastItem = this.bindingNavigatorMoveLastItem1;
            this.bindingNavFin.MoveNextItem = this.bindingNavigatorMoveNextItem1;
            this.bindingNavFin.MovePreviousItem = this.bindingNavigatorMovePreviousItem1;
            this.bindingNavFin.Name = "bindingNavFin";
            this.bindingNavFin.PositionItem = this.bindingNavigatorPositionItem1;
            this.bindingNavFin.Size = new System.Drawing.Size(1448, 39);
            this.bindingNavFin.TabIndex = 20;
            this.bindingNavFin.Text = "Financeiro";
            // 
            // bindingNavigatorCountItem1
            // 
            this.bindingNavigatorCountItem1.Name = "bindingNavigatorCountItem1";
            this.bindingNavigatorCountItem1.Size = new System.Drawing.Size(45, 36);
            this.bindingNavigatorCountItem1.Text = "of {0}";
            this.bindingNavigatorCountItem1.ToolTipText = "Total number of items";
            // 
            // toolSaldo
            // 
            this.toolSaldo.AutoSize = false;
            this.toolSaldo.Name = "toolSaldo";
            this.toolSaldo.Size = new System.Drawing.Size(150, 39);
            this.toolSaldo.Text = "Saldo:0";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // toolTitulo
            // 
            this.toolTitulo.AutoSize = false;
            this.toolTitulo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolTitulo.Name = "toolTitulo";
            this.toolTitulo.Size = new System.Drawing.Size(350, 24);
            this.toolTitulo.Text = "ENTRADAS";
            // 
            // bindingNavigatorMoveFirstItem1
            // 
            this.bindingNavigatorMoveFirstItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem1.Name = "bindingNavigatorMoveFirstItem1";
            this.bindingNavigatorMoveFirstItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem1.Size = new System.Drawing.Size(29, 36);
            this.bindingNavigatorMoveFirstItem1.Text = "Move first";
            // 
            // bindingNavigatorMovePreviousItem1
            // 
            this.bindingNavigatorMovePreviousItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem1.Name = "bindingNavigatorMovePreviousItem1";
            this.bindingNavigatorMovePreviousItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem1.Size = new System.Drawing.Size(29, 36);
            this.bindingNavigatorMovePreviousItem1.Text = "Move previous";
            // 
            // bindingNavigatorSeparator3
            // 
            this.bindingNavigatorSeparator3.Name = "bindingNavigatorSeparator3";
            this.bindingNavigatorSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // bindingNavigatorPositionItem1
            // 
            this.bindingNavigatorPositionItem1.AccessibleName = "Position";
            this.bindingNavigatorPositionItem1.AutoSize = false;
            this.bindingNavigatorPositionItem1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.bindingNavigatorPositionItem1.Name = "bindingNavigatorPositionItem1";
            this.bindingNavigatorPositionItem1.Size = new System.Drawing.Size(50, 27);
            this.bindingNavigatorPositionItem1.Text = "0";
            this.bindingNavigatorPositionItem1.ToolTipText = "Current position";
            // 
            // bindingNavigatorSeparator4
            // 
            this.bindingNavigatorSeparator4.Name = "bindingNavigatorSeparator4";
            this.bindingNavigatorSeparator4.Size = new System.Drawing.Size(6, 39);
            // 
            // bindingNavigatorMoveNextItem1
            // 
            this.bindingNavigatorMoveNextItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem1.Name = "bindingNavigatorMoveNextItem1";
            this.bindingNavigatorMoveNextItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem1.Size = new System.Drawing.Size(29, 36);
            this.bindingNavigatorMoveNextItem1.Text = "Move next";
            // 
            // bindingNavigatorMoveLastItem1
            // 
            this.bindingNavigatorMoveLastItem1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem1.Name = "bindingNavigatorMoveLastItem1";
            this.bindingNavigatorMoveLastItem1.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem1.Size = new System.Drawing.Size(29, 36);
            this.bindingNavigatorMoveLastItem1.Text = "Move last";
            // 
            // bindingNavigatorSeparator5
            // 
            this.bindingNavigatorSeparator5.Name = "bindingNavigatorSeparator5";
            this.bindingNavigatorSeparator5.Size = new System.Drawing.Size(6, 39);
            // 
            // toolBtnEdite
            // 
            this.toolBtnEdite.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolBtnEdite.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolBtnEdite.Name = "toolBtnEdite";
            this.toolBtnEdite.Size = new System.Drawing.Size(47, 36);
            this.toolBtnEdite.Text = "&Edite";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // toolComboPaginas
            // 
            this.toolComboPaginas.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.toolComboPaginas.Items.AddRange(new object[] {
            "ENTRADAS",
            "SAIDAS"});
            this.toolComboPaginas.Name = "toolComboPaginas";
            this.toolComboPaginas.Size = new System.Drawing.Size(121, 39);
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // ContextMenuPop
            // 
            this.ContextMenuPop.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ContextMenuPop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemAprop,
            this.toolStripSeparator8,
            this.toolStripMenuItemTransfere});
            this.ContextMenuPop.Name = "contextMenuStrip1";
            this.ContextMenuPop.Size = new System.Drawing.Size(172, 58);
            // 
            // toolStripMenuItemAprop
            // 
            this.toolStripMenuItemAprop.Name = "toolStripMenuItemAprop";
            this.toolStripMenuItemAprop.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.toolStripMenuItemAprop.ShowShortcutKeys = false;
            this.toolStripMenuItemAprop.Size = new System.Drawing.Size(171, 24);
            this.toolStripMenuItemAprop.Text = "&Apropriações";
            // 
            // toolStripSeparator8
            // 
            this.toolStripSeparator8.Name = "toolStripSeparator8";
            this.toolStripSeparator8.Size = new System.Drawing.Size(168, 6);
            // 
            // toolStripMenuItemTransfere
            // 
            this.toolStripMenuItemTransfere.Name = "toolStripMenuItemTransfere";
            this.toolStripMenuItemTransfere.Size = new System.Drawing.Size(171, 24);
            this.toolStripMenuItemTransfere.Text = "Transferencias";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FrmFinan_MOVFIN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1485, 846);
            this.Controls.Add(this.bindingNavFin);
            this.Controls.Add(this.gbSaidas);
            this.Controls.Add(this.gbEntradas);
            this.Controls.Add(this.toolMenu);
            this.Name = "FrmFinan_MOVFIN";
            this.Text = "FrmFinanceiroMOVFIN";
            this.toolMenu.ResumeLayout(false);
            this.toolMenu.PerformLayout();
            this.gbEntradas.ResumeLayout(false);
            this.gbEntradas.PerformLayout();
            this.pnEntradas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvReceber)).EndInit();
            this.gbSaidas.ResumeLayout(false);
            this.gbSaidas.PerformLayout();
            this.pnSaidas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvPagar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingNavFin)).EndInit();
            this.bindingNavFin.ResumeLayout(false);
            this.bindingNavFin.PerformLayout();
            this.ContextMenuPop.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolMenu;
        private System.Windows.Forms.ToolStripButton toolSair;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.GroupBox gbEntradas;
        private System.Windows.Forms.GroupBox gbSaidas;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.BindingNavigator bindingNavFin;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem1;
        private System.Windows.Forms.ToolStripLabel toolTitulo;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator3;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator4;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem1;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator5;
        private System.Windows.Forms.ToolStripButton toolBtnEdite;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripComboBox toolComboPaginas;
        private System.Windows.Forms.Panel pnEntradas;
        private System.Windows.Forms.DataGridView dgvReceber;
        private System.Windows.Forms.Panel pnSaidas;
        private System.Windows.Forms.DataGridView dgvPagar;
        private System.Windows.Forms.ToolStripLabel toolSaldo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.StatusStrip ssReceber;
        private System.Windows.Forms.StatusStrip ssPagar;
        private System.Windows.Forms.ToolStripButton toolCheques;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton toolNovo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton toolImprime;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
        private System.Windows.Forms.ToolStripButton toolDeleta;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripButton toolEdita;
        private System.Windows.Forms.ContextMenuStrip ContextMenuPop;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemAprop;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator8;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemTransfere;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator9;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.ToolStripButton tsConsulta;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        /* private System.ComponentModel.IContainer components = null;

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
             this.components = new System.ComponentModel.Container();
             this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
             this.ClientSize = new System.Drawing.Size(800, 450);
             this.Text = "FrmFinan_MOVIFN";
         }

         #endregion*/
    }
}