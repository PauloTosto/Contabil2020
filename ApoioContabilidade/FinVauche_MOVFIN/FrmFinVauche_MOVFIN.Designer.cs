using ApoioContabilidade.Financeiro_MOVFIN;

namespace ApoioContabilidade.FinVauche_MOVFIN
{
    partial class FrmFinVauche_MOVFIN
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
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
            this.toolMenu = new System.Windows.Forms.ToolStrip();
            this.toolSair = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
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
            this.ssVauches = new System.Windows.Forms.StatusStrip();
            this.pnEntradas = new System.Windows.Forms.Panel();
            this.dgvVauches = new System.Windows.Forms.DataGridView();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.bindingNavFin = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem1 = new System.Windows.Forms.ToolStripLabel();
            this.toolDebito = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.toolCredito = new System.Windows.Forms.ToolStripLabel();
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
            this.toolSaldo = new System.Windows.Forms.ToolStripLabel();
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
            ((System.ComponentModel.ISupportInitialize)(this.dgvVauches)).BeginInit();
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
            this.toolMenu.Size = new System.Drawing.Size(1485, 31);
            this.toolMenu.TabIndex = 0;
            // 
            // toolSair
            // 
            this.toolSair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolSair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolSair.Name = "toolSair";
            this.toolSair.Size = new System.Drawing.Size(38, 28);
            this.toolSair.Text = "&Sair";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolImprime
            // 
            this.toolImprime.AutoSize = false;
            this.toolImprime.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolImprime.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolImprime.Name = "toolImprime";
            this.toolImprime.Size = new System.Drawing.Size(65, 24);
            this.toolImprime.Text = "&Excel";
            this.toolImprime.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolImprime.Click += new System.EventHandler(this.toolImprime_Click_1);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 31);
            // 
            // toolNovo
            // 
            this.toolNovo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolNovo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolNovo.Name = "toolNovo";
            this.toolNovo.Size = new System.Drawing.Size(49, 28);
            this.toolNovo.Text = "&Novo";
            // 
            // toolStripSeparator6
            // 
            this.toolStripSeparator6.Name = "toolStripSeparator6";
            this.toolStripSeparator6.Size = new System.Drawing.Size(6, 31);
            // 
            // toolDeleta
            // 
            this.toolDeleta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolDeleta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolDeleta.Name = "toolDeleta";
            this.toolDeleta.Size = new System.Drawing.Size(57, 28);
            this.toolDeleta.Text = "&Deleta";
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(6, 31);
            // 
            // toolEdita
            // 
            this.toolEdita.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolEdita.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolEdita.Name = "toolEdita";
            this.toolEdita.Size = new System.Drawing.Size(47, 28);
            this.toolEdita.Text = "&Edita";
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(6, 31);
            // 
            // tsConsulta
            // 
            this.tsConsulta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsConsulta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsConsulta.Name = "tsConsulta";
            this.tsConsulta.Size = new System.Drawing.Size(70, 28);
            this.tsConsulta.Text = "&Consulta";
            this.tsConsulta.Click += new System.EventHandler(this.tsConsulta_Click_1);
            // 
            // gbEntradas
            // 
            this.gbEntradas.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gbEntradas.Controls.Add(this.ssVauches);
            this.gbEntradas.Controls.Add(this.pnEntradas);
            this.gbEntradas.Location = new System.Drawing.Point(6, 42);
            this.gbEntradas.Name = "gbEntradas";
            this.gbEntradas.Size = new System.Drawing.Size(1450, 730);
            this.gbEntradas.TabIndex = 1;
            this.gbEntradas.TabStop = false;
            // 
            // ssVauches
            // 
            this.ssVauches.Dock = System.Windows.Forms.DockStyle.None;
            this.ssVauches.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.ssVauches.Location = new System.Drawing.Point(26, 703);
            this.ssVauches.Name = "ssVauches";
            this.ssVauches.Size = new System.Drawing.Size(202, 22);
            this.ssVauches.TabIndex = 12;
            // 
            // pnEntradas
            // 
            this.pnEntradas.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnEntradas.Controls.Add(this.dgvVauches);
            this.pnEntradas.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnEntradas.Location = new System.Drawing.Point(3, 18);
            this.pnEntradas.Name = "pnEntradas";
            this.pnEntradas.Size = new System.Drawing.Size(1444, 671);
            this.pnEntradas.TabIndex = 11;
            // 
            // dgvVauches
            // 
            this.dgvVauches.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvVauches.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvVauches.Location = new System.Drawing.Point(11, 11);
            this.dgvVauches.Name = "dgvVauches";
            this.dgvVauches.RowHeadersWidth = 51;
            this.dgvVauches.RowTemplate.Height = 24;
            this.dgvVauches.Size = new System.Drawing.Size(1420, 641);
            this.dgvVauches.TabIndex = 11;
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
            this.toolDebito,
            this.toolStripSeparator3,
            this.toolCredito,
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
            this.toolSaldo});
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
            // toolDebito
            // 
            this.toolDebito.AutoSize = false;
            this.toolDebito.Name = "toolDebito";
            this.toolDebito.Size = new System.Drawing.Size(250, 39);
            this.toolDebito.Text = "Débito:0";
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 39);
            // 
            // toolCredito
            // 
            this.toolCredito.AutoSize = false;
            this.toolCredito.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolCredito.Name = "toolCredito";
            this.toolCredito.Size = new System.Drawing.Size(250, 39);
            this.toolCredito.Text = "Crédito:0";
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
            // toolSaldo
            // 
            this.toolSaldo.AutoSize = false;
            this.toolSaldo.Name = "toolSaldo";
            this.toolSaldo.Size = new System.Drawing.Size(250, 39);
            this.toolSaldo.Text = "Saldo:0";
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
            // FrmFinVauche_MOVFIN
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1485, 846);
            this.Controls.Add(this.bindingNavFin);
            this.Controls.Add(this.gbEntradas);
            this.Controls.Add(this.toolMenu);
            this.Name = "FrmFinVauche_MOVFIN";
            this.Text = "FrmFinanceiroMOVFIN";
            this.toolMenu.ResumeLayout(false);
            this.toolMenu.PerformLayout();
            this.gbEntradas.ResumeLayout(false);
            this.gbEntradas.PerformLayout();
            this.pnEntradas.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvVauches)).EndInit();
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
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private System.Windows.Forms.BindingNavigator bindingNavFin;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem1;
        private System.Windows.Forms.ToolStripLabel toolCredito;
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
        private System.Windows.Forms.Panel pnEntradas;
        private System.Windows.Forms.DataGridView dgvVauches;
        private System.Windows.Forms.ToolStripLabel toolDebito;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.StatusStrip ssVauches;
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
        private System.Windows.Forms.ToolStripLabel toolSaldo;

        /*private System.ComponentModel.IContainer components = null;

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
            this.Text = "FrmFinVauche_MOVFIN";
        }

        #endregion*/
    }
}