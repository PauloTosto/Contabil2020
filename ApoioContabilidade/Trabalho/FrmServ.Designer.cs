namespace ApoioContabilidade.Trabalho
{
    partial class FrmServ
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.toolStripPrincipal = new System.Windows.Forms.ToolStrip();
            this.tsSair = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsNovo = new System.Windows.Forms.ToolStripButton();
            this.tsEdit = new System.Windows.Forms.ToolStripButton();
            this.tsSalvar = new System.Windows.Forms.ToolStripButton();
            this.tsCancelar = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsDelete = new System.Windows.Forms.ToolStripButton();
            this.dgvServ = new System.Windows.Forms.DataGridView();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripPrincipal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServ)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            //
            // toolStripPrincipal
            //
            this.toolStripPrincipal.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripPrincipal.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
                this.tsSair,
                this.toolStripSeparator1,
                this.tsRefresh,
                this.toolStripSeparator2,
                this.tsNovo,
                this.tsEdit,
                this.tsSalvar,
                this.tsCancelar,
                this.toolStripSeparator3,
                this.tsDelete});
            this.toolStripPrincipal.Location = new System.Drawing.Point(0, 0);
            this.toolStripPrincipal.Name = "toolStripPrincipal";
            this.toolStripPrincipal.Size = new System.Drawing.Size(950, 27);
            this.toolStripPrincipal.TabIndex = 0;
            //
            // tsSair
            //
            this.tsSair.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSair.Name = "tsSair";
            this.tsSair.Text = "&Sair";
            this.tsSair.Click += new System.EventHandler(this.tsSair_Click);
            //
            // toolStripSeparator1
            //
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            //
            // tsRefresh
            //
            this.tsRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsRefresh.Name = "tsRefresh";
            this.tsRefresh.Text = "&Atualizar";
            this.tsRefresh.Click += new System.EventHandler(this.tsRefresh_Click);
            //
            // toolStripSeparator2
            //
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            //
            // tsNovo
            //
            this.tsNovo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsNovo.Name = "tsNovo";
            this.tsNovo.Text = "&Novo";
            this.tsNovo.Click += new System.EventHandler(this.tsNovo_Click);
            //
            // tsEdit
            //
            this.tsEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsEdit.Name = "tsEdit";
            this.tsEdit.Text = "&Editar";
            this.tsEdit.Click += new System.EventHandler(this.tsEdit_Click);
            //
            // tsSalvar
            //
            this.tsSalvar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsSalvar.Enabled = false;
            this.tsSalvar.Name = "tsSalvar";
            this.tsSalvar.Text = "Sa&lvar";
            this.tsSalvar.Click += new System.EventHandler(this.tsSalvar_Click);
            //
            // tsCancelar
            //
            this.tsCancelar.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsCancelar.Enabled = false;
            this.tsCancelar.Name = "tsCancelar";
            this.tsCancelar.Text = "&Cancelar";
            this.tsCancelar.Click += new System.EventHandler(this.tsCancelar_Click);
            //
            // toolStripSeparator3
            //
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            //
            // tsDelete
            //
            this.tsDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsDelete.Name = "tsDelete";
            this.tsDelete.Text = "&Deletar";
            this.tsDelete.Click += new System.EventHandler(this.tsDelete_Click);
            //
            // dgvServ
            //
            this.dgvServ.AllowUserToAddRows = false;
            this.dgvServ.AllowUserToDeleteRows = false;
            this.dgvServ.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvServ.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvServ.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.dgvServ.Location = new System.Drawing.Point(0, 27);
            this.dgvServ.Name = "dgvServ";
            this.dgvServ.ReadOnly = true;
            this.dgvServ.RowHeadersWidth = 30;
            this.dgvServ.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvServ.Size = new System.Drawing.Size(950, 473);
            this.dgvServ.TabIndex = 1;
            //
            // statusStrip1
            //
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { this.lblStatus });
            this.statusStrip1.Location = new System.Drawing.Point(0, 478);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(950, 22);
            this.statusStrip1.TabIndex = 2;
            //
            // lblStatus
            //
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Text = "Carregando...";
            //
            // FrmServ
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(950, 500);
            this.Controls.Add(this.dgvServ);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStripPrincipal);
            this.Name = "FrmServ";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Cadastro de Serviços (SERVIC)";
            this.toolStripPrincipal.ResumeLayout(false);
            this.toolStripPrincipal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvServ)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStripPrincipal;
        private System.Windows.Forms.ToolStripButton tsSair;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsRefresh;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton tsNovo;
        private System.Windows.Forms.ToolStripButton tsEdit;
        private System.Windows.Forms.ToolStripButton tsSalvar;
        private System.Windows.Forms.ToolStripButton tsCancelar;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripButton tsDelete;
        private System.Windows.Forms.DataGridView dgvServ;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
    }
}
