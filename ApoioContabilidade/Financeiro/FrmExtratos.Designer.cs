namespace ApoioContabilidade.Financeiro
{
    partial class FrmExtratos
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
            this.button1 = new System.Windows.Forms.Button();
            this.btnBBrasil = new System.Windows.Forms.Button();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.dtData2 = new System.Windows.Forms.DateTimePicker();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
           // this.edtNumSaldo = new ClassFiltroEdite.NumericTextBox();
            this.comboBancos = new System.Windows.Forms.ComboBox();
            this.pnGeral = new System.Windows.Forms.Panel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.dgvExtrato = new System.Windows.Forms.DataGridView();
            this.dgvExtratoBB = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.pnTop.SuspendLayout();
            this.pnGeral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExtrato)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExtratoBB)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.button1);
            this.pnTop.Controls.Add(this.btnBBrasil);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Controls.Add(this.dtData2);
            this.pnTop.Controls.Add(this.dtData1);
           // this.pnTop.Controls.Add(this.edtNumSaldo);
            this.pnTop.Controls.Add(this.comboBancos);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1461, 119);
            this.pnTop.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1055, 74);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 12;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnBBrasil
            // 
            this.btnBBrasil.Location = new System.Drawing.Point(736, 73);
            this.btnBBrasil.Name = "btnBBrasil";
            this.btnBBrasil.Size = new System.Drawing.Size(223, 31);
            this.btnBBrasil.TabIndex = 11;
            this.btnBBrasil.Text = "&Importe BB Extrato Excel";
            this.btnBBrasil.UseVisualStyleBackColor = true;
            this.btnBBrasil.Click += new System.EventHandler(this.btnBBrasil_Click);
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(36, 73);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(117, 23);
            this.btnConsulta.TabIndex = 10;
            this.btnConsulta.Text = "&Extrato";
            this.btnConsulta.TextImageRelation = System.Windows.Forms.TextImageRelation.TextAboveImage;
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // dtData2
            // 
            this.dtData2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData2.Location = new System.Drawing.Point(164, 32);
            this.dtData2.Margin = new System.Windows.Forms.Padding(4);
            this.dtData2.Name = "dtData2";
            this.dtData2.Size = new System.Drawing.Size(123, 22);
            this.dtData2.TabIndex = 9;
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(32, 32);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(123, 22);
            this.dtData1.TabIndex = 8;
            // 
            // edtNumSaldo
            // 
        /*    this.edtNumSaldo.AllowSpace = false;
            this.edtNumSaldo.Location = new System.Drawing.Point(672, 31);
            this.edtNumSaldo.Name = "edtNumSaldo";
            this.edtNumSaldo.Size = new System.Drawing.Size(130, 22);
            this.edtNumSaldo.TabIndex = 1;
            this.edtNumSaldo.Text = "0";
            this.edtNumSaldo.Visible = false;
          */  // 
            // comboBancos
            // 
            this.comboBancos.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.comboBancos.DropDownStyle = System.Windows.Forms.ComboBoxStyle.Simple;
            this.comboBancos.FormattingEnabled = true;
            this.comboBancos.Location = new System.Drawing.Point(328, 30);
            this.comboBancos.Name = "comboBancos";
            this.comboBancos.Size = new System.Drawing.Size(284, 24);
            this.comboBancos.Sorted = true;
            this.comboBancos.TabIndex = 0;
            // 
            // pnGeral
            // 
            this.pnGeral.Controls.Add(this.splitContainer1);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnGeral.Location = new System.Drawing.Point(0, 119);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1461, 429);
            this.pnGeral.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AutoScroll = true;
            this.splitContainer1.Panel1.Controls.Add(this.dgvExtrato);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AutoScroll = true;
            this.splitContainer1.Panel2.Controls.Add(this.dgvExtratoBB);
            this.splitContainer1.Size = new System.Drawing.Size(1461, 429);
            this.splitContainer1.SplitterDistance = 725;
            this.splitContainer1.SplitterIncrement = 6;
            this.splitContainer1.SplitterWidth = 8;
            this.splitContainer1.TabIndex = 2;
            // 
            // dgvExtrato
            // 
            this.dgvExtrato.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExtrato.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExtrato.Location = new System.Drawing.Point(0, 0);
            this.dgvExtrato.Name = "dgvExtrato";
            this.dgvExtrato.RowHeadersWidth = 51;
            this.dgvExtrato.RowTemplate.Height = 24;
            this.dgvExtrato.Size = new System.Drawing.Size(725, 429);
            this.dgvExtrato.TabIndex = 0;
            // 
            // dgvExtratoBB
            // 
            this.dgvExtratoBB.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvExtratoBB.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvExtratoBB.Location = new System.Drawing.Point(0, 0);
            this.dgvExtratoBB.Name = "dgvExtratoBB";
            this.dgvExtratoBB.RowHeadersWidth = 51;
            this.dgvExtratoBB.RowTemplate.Height = 24;
            this.dgvExtratoBB.Size = new System.Drawing.Size(728, 429);
            this.dgvExtratoBB.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // FrmExtratos
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1461, 548);
            this.Controls.Add(this.pnGeral);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmExtratos";
            this.Text = "FrmExtratos";
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.pnGeral.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvExtrato)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvExtratoBB)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.ComboBox comboBancos;
      //  private ClassFiltroEdite.NumericTextBox edtNumSaldo;
        public System.Windows.Forms.DateTimePicker dtData2;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.Button btnConsulta;
        private System.Windows.Forms.Button btnBBrasil;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dgvExtrato;
        private System.Windows.Forms.DataGridView dgvExtratoBB;
        private System.Windows.Forms.Button button1;
    }
}