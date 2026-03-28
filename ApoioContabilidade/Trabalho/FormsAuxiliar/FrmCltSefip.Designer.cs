
namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    partial class FrmCltSefip
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.tsNome = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsNovo = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsEdita = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsDeleta = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsSair = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.dgvSefip = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            this.tsMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSefip)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tsMenu);
            this.panel1.Controls.Add(this.dgvSefip);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1207, 521);
            this.panel1.TabIndex = 0;
            // 
            // tsMenu
            // 
            this.tsMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsNome,
            this.toolStripSeparator4,
            this.tsNovo,
            this.toolStripSeparator1,
            this.tsEdita,
            this.toolStripSeparator2,
            this.tsDeleta,
            this.toolStripSeparator3,
            this.tsSair,
            this.toolStripSeparator5});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(1207, 31);
            this.tsMenu.TabIndex = 1;
            this.tsMenu.Text = "toolStrip1";
            // 
            // tsNome
            // 
            this.tsNome.AutoSize = false;
            this.tsNome.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsNome.Name = "tsNome";
            this.tsNome.Size = new System.Drawing.Size(300, 24);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 27);
            // 
            // tsNovo
            // 
            this.tsNovo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsNovo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNovo.Name = "tsNovo";
            this.tsNovo.Size = new System.Drawing.Size(49, 24);
            this.tsNovo.Text = "&Novo";
            this.tsNovo.Click += new System.EventHandler(this.tsNovo_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // tsEdita
            // 
            this.tsEdita.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsEdita.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsEdita.Name = "tsEdita";
            this.tsEdita.Size = new System.Drawing.Size(47, 24);
            this.tsEdita.Text = "&Edita";
            this.tsEdita.Click += new System.EventHandler(this.tsEdita_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 27);
            // 
            // tsDeleta
            // 
            this.tsDeleta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDeleta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDeleta.Name = "tsDeleta";
            this.tsDeleta.Size = new System.Drawing.Size(57, 24);
            this.tsDeleta.Text = "&Deleta";
            this.tsDeleta.Click += new System.EventHandler(this.tsDeleta_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 27);
            // 
            // tsSair
            // 
            this.tsSair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSair.Name = "tsSair";
            this.tsSair.Size = new System.Drawing.Size(38, 24);
            this.tsSair.Text = "&Sair";
            this.tsSair.Click += new System.EventHandler(this.tsSair_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 27);
            // 
            // dgvSefip
            // 
            this.dgvSefip.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvSefip.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSefip.Location = new System.Drawing.Point(36, 47);
            this.dgvSefip.Name = "dgvSefip";
            this.dgvSefip.RowHeadersWidth = 51;
            this.dgvSefip.RowTemplate.Height = 24;
            this.dgvSefip.Size = new System.Drawing.Size(1152, 438);
            this.dgvSefip.TabIndex = 0;
            // 
            // FrmCltSefip
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1207, 521);
            this.Controls.Add(this.panel1);
            this.Name = "FrmCltSefip";
            this.Text = "CltSefip";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCltFamilia_FormClosing);
            this.Load += new System.EventHandler(this.FrmCltReajuste_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSefip)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvSefip;
        private System.Windows.Forms.ToolStrip tsMenu;
        private System.Windows.Forms.ToolStripButton tsNovo;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsEdita;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsDeleta;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsSair;
        private System.Windows.Forms.ToolStripLabel tsNome;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;

        /// <summary>
        /// Required designer variable.
        /// </summary>

    }
}