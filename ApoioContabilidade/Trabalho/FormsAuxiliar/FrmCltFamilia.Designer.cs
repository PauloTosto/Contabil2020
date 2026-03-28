
namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    partial class FrmCltFamilia
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCltFamilia));
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvFamilia = new System.Windows.Forms.DataGridView();
            this.tsMenu = new System.Windows.Forms.ToolStrip();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsNovo = new System.Windows.Forms.ToolStripButton();
            this.tsEdita = new System.Windows.Forms.ToolStripButton();
            this.tsDeleta = new System.Windows.Forms.ToolStripButton();
            this.tsSair = new System.Windows.Forms.ToolStripButton();
            this.tsNome = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilia)).BeginInit();
            this.tsMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tsMenu);
            this.panel1.Controls.Add(this.dgvFamilia);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(800, 450);
            this.panel1.TabIndex = 0;
            // 
            // dgvFamilia
            // 
            this.dgvFamilia.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFamilia.Location = new System.Drawing.Point(36, 47);
            this.dgvFamilia.Name = "dgvFamilia";
            this.dgvFamilia.RowHeadersWidth = 51;
            this.dgvFamilia.RowTemplate.Height = 24;
            this.dgvFamilia.Size = new System.Drawing.Size(727, 357);
            this.dgvFamilia.TabIndex = 0;
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
            this.tsSair});
            this.tsMenu.Location = new System.Drawing.Point(0, 0);
            this.tsMenu.Name = "tsMenu";
            this.tsMenu.Size = new System.Drawing.Size(800, 31);
            this.tsMenu.TabIndex = 1;
            this.tsMenu.Text = "toolStrip1";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 31);
            // 
            // tsNovo
            // 
            this.tsNovo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsNovo.Image = ((System.Drawing.Image)(resources.GetObject("tsNovo.Image")));
            this.tsNovo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsNovo.Name = "tsNovo";
            this.tsNovo.Size = new System.Drawing.Size(49, 28);
            this.tsNovo.Text = "&Novo";
            this.tsNovo.Click += new System.EventHandler(this.tsNovo_Click);
            // 
            // tsEdita
            // 
            this.tsEdita.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsEdita.Image = ((System.Drawing.Image)(resources.GetObject("tsEdita.Image")));
            this.tsEdita.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsEdita.Name = "tsEdita";
            this.tsEdita.Size = new System.Drawing.Size(47, 28);
            this.tsEdita.Text = "&Edita";
            this.tsEdita.Click += new System.EventHandler(this.tsEdita_Click);
            // 
            // tsDeleta
            // 
            this.tsDeleta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDeleta.Image = ((System.Drawing.Image)(resources.GetObject("tsDeleta.Image")));
            this.tsDeleta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsDeleta.Name = "tsDeleta";
            this.tsDeleta.Size = new System.Drawing.Size(57, 28);
            this.tsDeleta.Text = "&Deleta";
            this.tsDeleta.Click += new System.EventHandler(this.tsDeleta_Click);
            // 
            // tsSair
            // 
            this.tsSair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSair.Image = ((System.Drawing.Image)(resources.GetObject("tsSair.Image")));
            this.tsSair.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsSair.Name = "tsSair";
            this.tsSair.Size = new System.Drawing.Size(38, 28);
            this.tsSair.Text = "&Sair";
            this.tsSair.Click += new System.EventHandler(this.tsSair_Click);
            // 
            // tsNome
            // 
            this.tsNome.AutoSize = false;
            this.tsNome.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsNome.Name = "tsNome";
            this.tsNome.Size = new System.Drawing.Size(300, 28);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 31);
            // 
            // FrmCltFamilia
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.panel1);
            this.Name = "FrmCltFamilia";
            this.Text = "FrmCltFamilia";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmCltFamilia_FormClosing);
            this.Load += new System.EventHandler(this.FrmCltFamilia_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFamilia)).EndInit();
            this.tsMenu.ResumeLayout(false);
            this.tsMenu.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvFamilia;
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
    }
}