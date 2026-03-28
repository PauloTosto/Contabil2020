
namespace ApoioContabilidade.Contabilidade
{
    partial class FrmDadosContabilidade
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
            this.dtData2 = new System.Windows.Forms.DateTimePicker();
            this.lbPeriodo = new System.Windows.Forms.Label();
            this.lbMeses = new System.Windows.Forms.Label();
            this.btnImprime = new System.Windows.Forms.Button();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.btnTransfere = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbDeletarRegistros = new System.Windows.Forms.RadioButton();
            this.rbInserirRegistros = new System.Windows.Forms.RadioButton();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tcMeses = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.pnTop.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tcMeses.SuspendLayout();
            this.tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.dtData2);
            this.pnTop.Controls.Add(this.lbPeriodo);
            this.pnTop.Controls.Add(this.lbMeses);
            this.pnTop.Controls.Add(this.btnImprime);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.btnTransfere);
            this.pnTop.Controls.Add(this.groupBox1);
            this.pnTop.Controls.Add(this.dtData1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1713, 100);
            this.pnTop.TabIndex = 0;
            // 
            // dtData2
            // 
            this.dtData2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData2.Location = new System.Drawing.Point(137, 33);
            this.dtData2.Margin = new System.Windows.Forms.Padding(4);
            this.dtData2.Name = "dtData2";
            this.dtData2.Size = new System.Drawing.Size(107, 22);
            this.dtData2.TabIndex = 16;
            // 
            // lbPeriodo
            // 
            this.lbPeriodo.AutoSize = true;
            this.lbPeriodo.Location = new System.Drawing.Point(21, 66);
            this.lbPeriodo.Name = "lbPeriodo";
            this.lbPeriodo.Size = new System.Drawing.Size(0, 17);
            this.lbPeriodo.TabIndex = 15;
            // 
            // lbMeses
            // 
            this.lbMeses.AutoSize = true;
            this.lbMeses.Location = new System.Drawing.Point(47, 11);
            this.lbMeses.Name = "lbMeses";
            this.lbMeses.Size = new System.Drawing.Size(61, 17);
            this.lbMeses.TabIndex = 14;
            this.lbMeses.Text = "Periodo:";
            // 
            // btnImprime
            // 
            this.btnImprime.Location = new System.Drawing.Point(626, 53);
            this.btnImprime.Name = "btnImprime";
            this.btnImprime.Size = new System.Drawing.Size(137, 25);
            this.btnImprime.TabIndex = 12;
            this.btnImprime.Text = "&Imprime";
            this.btnImprime.UseVisualStyleBackColor = true;
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(625, 29);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(137, 23);
            this.btnConsulta.TabIndex = 11;
            this.btnConsulta.Text = "&Consulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // btnTransfere
            // 
            this.btnTransfere.Location = new System.Drawing.Point(626, 5);
            this.btnTransfere.Name = "btnTransfere";
            this.btnTransfere.Size = new System.Drawing.Size(137, 23);
            this.btnTransfere.TabIndex = 10;
            this.btnTransfere.Text = "Transfere Dados";
            this.btnTransfere.UseVisualStyleBackColor = true;
            this.btnTransfere.Click += new System.EventHandler(this.btnTransfere_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbDeletarRegistros);
            this.groupBox1.Controls.Add(this.rbInserirRegistros);
            this.groupBox1.Location = new System.Drawing.Point(335, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(271, 77);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Transferencias no Periodo MovFin";
            // 
            // rbDeletarRegistros
            // 
            this.rbDeletarRegistros.AutoSize = true;
            this.rbDeletarRegistros.Location = new System.Drawing.Point(12, 49);
            this.rbDeletarRegistros.Name = "rbDeletarRegistros";
            this.rbDeletarRegistros.Size = new System.Drawing.Size(139, 21);
            this.rbDeletarRegistros.TabIndex = 1;
            this.rbDeletarRegistros.Text = "Deletar Registros";
            this.rbDeletarRegistros.UseVisualStyleBackColor = true;
            // 
            // rbInserirRegistros
            // 
            this.rbInserirRegistros.AutoSize = true;
            this.rbInserirRegistros.Checked = true;
            this.rbInserirRegistros.Location = new System.Drawing.Point(13, 21);
            this.rbInserirRegistros.Name = "rbInserirRegistros";
            this.rbInserirRegistros.Size = new System.Drawing.Size(132, 21);
            this.rbInserirRegistros.TabIndex = 0;
            this.rbInserirRegistros.TabStop = true;
            this.rbInserirRegistros.Text = "Inserir Registros";
            this.rbInserirRegistros.UseVisualStyleBackColor = true;
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtData1.Location = new System.Drawing.Point(13, 33);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(108, 22);
            this.dtData1.TabIndex = 7;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.tcMeses);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 100);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1713, 497);
            this.panel1.TabIndex = 2;
            // 
            // tcMeses
            // 
            this.tcMeses.Controls.Add(this.tabPage1);
            this.tcMeses.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMeses.Location = new System.Drawing.Point(0, 0);
            this.tcMeses.Multiline = true;
            this.tcMeses.Name = "tcMeses";
            this.tcMeses.SelectedIndex = 0;
            this.tcMeses.Size = new System.Drawing.Size(1711, 495);
            this.tcMeses.TabIndex = 2;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.dataGridView1);
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(1703, 466);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "tabPage1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(3, 3);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 51;
            this.dataGridView1.RowTemplate.Height = 24;
            this.dataGridView1.Size = new System.Drawing.Size(1697, 460);
            this.dataGridView1.TabIndex = 0;
            // 
            // FrmDadosContabilidade
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1713, 597);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmDadosContabilidade";
            this.Text = "FrmDadosContabilidade";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.tcMeses.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.Button btnImprime;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.Button btnTransfere;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbDeletarRegistros;
        private System.Windows.Forms.RadioButton rbInserirRegistros;
        private System.Windows.Forms.Label lbMeses;
        private System.Windows.Forms.Label lbPeriodo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tcMeses;
        public System.Windows.Forms.DateTimePicker dtData2;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.DataGridView dataGridView1;
    }
}