
namespace ApoioContabilidade.Almoxarifado
{
    partial class FrmRelDeposito
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
            this.btnAtualiza = new System.Windows.Forms.Button();
            this.btnImprime = new System.Windows.Forms.Button();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.cbArmazem = new System.Windows.Forms.ComboBox();
            this.cbItens = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.dtData2 = new System.Windows.Forms.DateTimePicker();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.tcDados = new System.Windows.Forms.TabControl();
            this.tpMestre = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dgvMestre = new System.Windows.Forms.DataGridView();
            this.tpDEtalhe = new System.Windows.Forms.TabPage();
            this.pnDetalhe = new System.Windows.Forms.Panel();
            this.dgvDetalhe = new System.Windows.Forms.DataGridView();
            this.tpDeposito = new System.Windows.Forms.TabPage();
            this.dgvDeposito = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.pnTop.SuspendLayout();
            this.tcDados.SuspendLayout();
            this.tpMestre.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMestre)).BeginInit();
            this.tpDEtalhe.SuspendLayout();
            this.pnDetalhe.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalhe)).BeginInit();
            this.tpDeposito.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeposito)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.btnAtualiza);
            this.pnTop.Controls.Add(this.btnImprime);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.cbArmazem);
            this.pnTop.Controls.Add(this.cbItens);
            this.pnTop.Controls.Add(this.label3);
            this.pnTop.Controls.Add(this.label2);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.dtData2);
            this.pnTop.Controls.Add(this.dtData1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1488, 119);
            this.pnTop.TabIndex = 0;
            // 
            // btnAtualiza
            // 
            this.btnAtualiza.Location = new System.Drawing.Point(829, 14);
            this.btnAtualiza.Name = "btnAtualiza";
            this.btnAtualiza.Size = new System.Drawing.Size(187, 23);
            this.btnAtualiza.TabIndex = 19;
            this.btnAtualiza.Text = "&Atualiza Item Material";
            this.btnAtualiza.UseVisualStyleBackColor = true;
            this.btnAtualiza.Visible = false;
            // 
            // btnImprime
            // 
            this.btnImprime.Location = new System.Drawing.Point(618, 68);
            this.btnImprime.Name = "btnImprime";
            this.btnImprime.Size = new System.Drawing.Size(179, 23);
            this.btnImprime.TabIndex = 18;
            this.btnImprime.Text = "&Imprime";
            this.btnImprime.UseVisualStyleBackColor = true;
            this.btnImprime.Visible = false;
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(618, 12);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(179, 23);
            this.btnConsulta.TabIndex = 16;
            this.btnConsulta.Text = "&Consulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // cbArmazem
            // 
            this.cbArmazem.FormattingEnabled = true;
            this.cbArmazem.Location = new System.Drawing.Point(31, 83);
            this.cbArmazem.Name = "cbArmazem";
            this.cbArmazem.Size = new System.Drawing.Size(250, 24);
            this.cbArmazem.TabIndex = 15;
            this.cbArmazem.SelectedIndexChanged += new System.EventHandler(this.cbArmazem_SelectedIndexChanged);
            this.cbArmazem.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbArmazem_KeyPress);
            // 
            // cbItens
            // 
            this.cbItens.FormattingEnabled = true;
            this.cbItens.Location = new System.Drawing.Point(320, 32);
            this.cbItens.Name = "cbItens";
            this.cbItens.Size = new System.Drawing.Size(216, 24);
            this.cbItens.TabIndex = 13;
            this.cbItens.Visible = false;
            this.cbItens.SelectedIndexChanged += new System.EventHandler(this.cbItens_SelectedIndexChanged);
            this.cbItens.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbItens_KeyPress);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(321, 12);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 17);
            this.label3.TabIndex = 12;
            this.label3.Text = "Itens Material";
            this.label3.Visible = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(30, 62);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 17);
            this.label2.TabIndex = 11;
            this.label2.Text = "Armazem";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(71, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 17);
            this.label1.TabIndex = 10;
            this.label1.Text = "Periodo";
            // 
            // dtData2
            // 
            this.dtData2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData2.Location = new System.Drawing.Point(158, 32);
            this.dtData2.Margin = new System.Windows.Forms.Padding(4);
            this.dtData2.Name = "dtData2";
            this.dtData2.Size = new System.Drawing.Size(123, 22);
            this.dtData2.TabIndex = 9;
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(26, 32);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(123, 22);
            this.dtData1.TabIndex = 8;
            // 
            // tcDados
            // 
            this.tcDados.Controls.Add(this.tpMestre);
            this.tcDados.Controls.Add(this.tpDEtalhe);
            this.tcDados.Controls.Add(this.tpDeposito);
            this.tcDados.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcDados.Location = new System.Drawing.Point(0, 119);
            this.tcDados.Name = "tcDados";
            this.tcDados.SelectedIndex = 0;
            this.tcDados.Size = new System.Drawing.Size(1488, 404);
            this.tcDados.TabIndex = 1;
            this.tcDados.SelectedIndexChanged += new System.EventHandler(this.tcDados_SelectedIndexChanged);
            // 
            // tpMestre
            // 
            this.tpMestre.Controls.Add(this.panel1);
            this.tpMestre.Location = new System.Drawing.Point(4, 25);
            this.tpMestre.Name = "tpMestre";
            this.tpMestre.Padding = new System.Windows.Forms.Padding(3);
            this.tpMestre.Size = new System.Drawing.Size(1480, 375);
            this.tpMestre.TabIndex = 0;
            this.tpMestre.Text = "Mestre";
            this.tpMestre.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.panel4);
            this.panel1.Controls.Add(this.dgvMestre);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1474, 369);
            this.panel1.TabIndex = 0;
            // 
            // dgvMestre
            // 
            this.dgvMestre.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvMestre.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMestre.Location = new System.Drawing.Point(3, 0);
            this.dgvMestre.Name = "dgvMestre";
            this.dgvMestre.RowHeadersWidth = 51;
            this.dgvMestre.RowTemplate.Height = 24;
            this.dgvMestre.Size = new System.Drawing.Size(1357, 350);
            this.dgvMestre.TabIndex = 0;
            // 
            // tpDEtalhe
            // 
            this.tpDEtalhe.Controls.Add(this.pnDetalhe);
            this.tpDEtalhe.Location = new System.Drawing.Point(4, 25);
            this.tpDEtalhe.Name = "tpDEtalhe";
            this.tpDEtalhe.Padding = new System.Windows.Forms.Padding(3);
            this.tpDEtalhe.Size = new System.Drawing.Size(1480, 375);
            this.tpDEtalhe.TabIndex = 1;
            this.tpDEtalhe.Text = "Detalhe";
            this.tpDEtalhe.UseVisualStyleBackColor = true;
            // 
            // pnDetalhe
            // 
            this.pnDetalhe.Controls.Add(this.panel3);
            this.pnDetalhe.Controls.Add(this.dgvDetalhe);
            this.pnDetalhe.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnDetalhe.Location = new System.Drawing.Point(3, 3);
            this.pnDetalhe.Name = "pnDetalhe";
            this.pnDetalhe.Size = new System.Drawing.Size(1474, 369);
            this.pnDetalhe.TabIndex = 0;
            // 
            // dgvDetalhe
            // 
            this.dgvDetalhe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dgvDetalhe.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDetalhe.Location = new System.Drawing.Point(3, 0);
            this.dgvDetalhe.Name = "dgvDetalhe";
            this.dgvDetalhe.RowHeadersWidth = 51;
            this.dgvDetalhe.RowTemplate.Height = 24;
            this.dgvDetalhe.Size = new System.Drawing.Size(1397, 349);
            this.dgvDetalhe.TabIndex = 1;
            // 
            // tpDeposito
            // 
            this.tpDeposito.Controls.Add(this.panel2);
            this.tpDeposito.Controls.Add(this.dgvDeposito);
            this.tpDeposito.Location = new System.Drawing.Point(4, 25);
            this.tpDeposito.Name = "tpDeposito";
            this.tpDeposito.Padding = new System.Windows.Forms.Padding(3);
            this.tpDeposito.Size = new System.Drawing.Size(1480, 375);
            this.tpDeposito.TabIndex = 2;
            this.tpDeposito.Text = "Depositos";
            this.tpDeposito.UseVisualStyleBackColor = true;
            // 
            // dgvDeposito
            // 
            this.dgvDeposito.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvDeposito.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvDeposito.Location = new System.Drawing.Point(3, 3);
            this.dgvDeposito.Name = "dgvDeposito";
            this.dgvDeposito.RowHeadersWidth = 51;
            this.dgvDeposito.RowTemplate.Height = 24;
            this.dgvDeposito.Size = new System.Drawing.Size(1474, 369);
            this.dgvDeposito.TabIndex = 2;
            // 
            // panel2
            // 
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 355);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1474, 17);
            this.panel2.TabIndex = 3;
            // 
            // panel3
            // 
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(0, 352);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(1474, 17);
            this.panel3.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel4.Location = new System.Drawing.Point(0, 352);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1474, 17);
            this.panel4.TabIndex = 5;
            // 
            // FrmRelDeposito
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1488, 523);
            this.Controls.Add(this.tcDados);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmRelDeposito";
            this.Text = "FrmRelDeposito";
            this.Load += new System.EventHandler(this.FrmRelDeposito_Load);
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.tcDados.ResumeLayout(false);
            this.tpMestre.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMestre)).EndInit();
            this.tpDEtalhe.ResumeLayout(false);
            this.pnDetalhe.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDetalhe)).EndInit();
            this.tpDeposito.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvDeposito)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.TabControl tcDados;
        private System.Windows.Forms.TabPage tpMestre;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage tpDEtalhe;
        private System.Windows.Forms.Panel pnDetalhe;
        private System.Windows.Forms.Button btnAtualiza;
        private System.Windows.Forms.Button btnImprime;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.ComboBox cbArmazem;
        private System.Windows.Forms.ComboBox cbItens;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.DateTimePicker dtData2;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.DataGridView dgvMestre;
        private System.Windows.Forms.DataGridView dgvDetalhe;
        private System.Windows.Forms.TabPage tpDeposito;
        private System.Windows.Forms.DataGridView dgvDeposito;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
    }
}