
namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    partial class FrmPesqFolha
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPesqFolha));
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbFaltas = new System.Windows.Forms.Label();
            this.dgvFaltas = new System.Windows.Forms.DataGridView();
            this.lbFolhas = new System.Windows.Forms.Label();
            this.dgvFolhas = new System.Windows.Forms.DataGridView();
            this.pnTop = new System.Windows.Forms.Panel();
            this.inicio = new System.Windows.Forms.DateTimePicker();
            this.fim = new System.Windows.Forms.DateTimePicker();
            this.tsTop = new System.Windows.Forms.ToolStrip();
            this.tsLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsConsulta = new System.Windows.Forms.ToolStripButton();
            this.panel1.SuspendLayout();
            this.panel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFaltas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolhas)).BeginInit();
            this.pnTop.SuspendLayout();
            this.tsTop.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel3);
            this.panel1.Controls.Add(this.pnTop);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1287, 544);
            this.panel1.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbFaltas);
            this.panel3.Controls.Add(this.dgvFaltas);
            this.panel3.Controls.Add(this.lbFolhas);
            this.panel3.Controls.Add(this.dgvFolhas);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 74);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1287, 470);
            this.panel3.TabIndex = 1;
            // 
            // lbFaltas
            // 
            this.lbFaltas.AutoSize = true;
            this.lbFaltas.Location = new System.Drawing.Point(18, 231);
            this.lbFaltas.Name = "lbFaltas";
            this.lbFaltas.Size = new System.Drawing.Size(341, 17);
            this.lbFaltas.TabIndex = 3;
            this.lbFaltas.Text = "Faltas Injustificadas e Afastamentos INSS no Periodo";
            // 
            // dgvFaltas
            // 
            this.dgvFaltas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFaltas.Location = new System.Drawing.Point(16, 258);
            this.dgvFaltas.Name = "dgvFaltas";
            this.dgvFaltas.RowHeadersWidth = 51;
            this.dgvFaltas.RowTemplate.Height = 24;
            this.dgvFaltas.Size = new System.Drawing.Size(1255, 180);
            this.dgvFaltas.TabIndex = 2;
            // 
            // lbFolhas
            // 
            this.lbFolhas.AutoSize = true;
            this.lbFolhas.Location = new System.Drawing.Point(14, 11);
            this.lbFolhas.Name = "lbFolhas";
            this.lbFolhas.Size = new System.Drawing.Size(123, 17);
            this.lbFolhas.TabIndex = 1;
            this.lbFolhas.Text = "Folhas no Período";
            // 
            // dgvFolhas
            // 
            this.dgvFolhas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFolhas.Location = new System.Drawing.Point(12, 38);
            this.dgvFolhas.Name = "dgvFolhas";
            this.dgvFolhas.RowHeadersWidth = 51;
            this.dgvFolhas.RowTemplate.Height = 24;
            this.dgvFolhas.Size = new System.Drawing.Size(1255, 180);
            this.dgvFolhas.TabIndex = 0;
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.tsTop);
            this.pnTop.Controls.Add(this.inicio);
            this.pnTop.Controls.Add(this.fim);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1287, 74);
            this.pnTop.TabIndex = 0;
            // 
            // inicio
            // 
            this.inicio.CustomFormat = "";
            this.inicio.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.inicio.Location = new System.Drawing.Point(12, 33);
            this.inicio.Name = "inicio";
            this.inicio.Size = new System.Drawing.Size(104, 22);
            this.inicio.TabIndex = 2;
            // 
            // fim
            // 
            this.fim.CustomFormat = "";
            this.fim.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fim.Location = new System.Drawing.Point(163, 34);
            this.fim.Name = "fim";
            this.fim.Size = new System.Drawing.Size(104, 22);
            this.fim.TabIndex = 1;
            // 
            // tsTop
            // 
            this.tsTop.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.tsTop.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsLabel,
            this.toolStripSeparator1,
            this.tsConsulta});
            this.tsTop.Location = new System.Drawing.Point(0, 0);
            this.tsTop.Name = "tsTop";
            this.tsTop.Size = new System.Drawing.Size(1287, 27);
            this.tsTop.TabIndex = 4;
            // 
            // tsLabel
            // 
            this.tsLabel.AutoSize = false;
            this.tsLabel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsLabel.Font = new System.Drawing.Font("Segoe UI", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsLabel.Name = "tsLabel";
            this.tsLabel.Size = new System.Drawing.Size(300, 24);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 27);
            // 
            // tsConsulta
            // 
            this.tsConsulta.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsConsulta.Image = ((System.Drawing.Image)(resources.GetObject("tsConsulta.Image")));
            this.tsConsulta.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsConsulta.Name = "tsConsulta";
            this.tsConsulta.Size = new System.Drawing.Size(70, 24);
            this.tsConsulta.Text = "&Consulta";
            this.tsConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // FrmPesqFolha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1287, 544);
            this.Controls.Add(this.panel1);
            this.Name = "FrmPesqFolha";
            this.Text = "FrmPesqFolha";
            this.panel1.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFaltas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFolhas)).EndInit();
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.tsTop.ResumeLayout(false);
            this.tsTop.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.DataGridView dgvFolhas;
        private System.Windows.Forms.DateTimePicker inicio;
        private System.Windows.Forms.DateTimePicker fim;
        private System.Windows.Forms.Label lbFolhas;
        private System.Windows.Forms.Label lbFaltas;
        private System.Windows.Forms.DataGridView dgvFaltas;
        private System.Windows.Forms.ToolStrip tsTop;
        private System.Windows.Forms.ToolStripLabel tsLabel;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsConsulta;
    }
}