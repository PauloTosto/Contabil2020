namespace ApoioContabilidade
{
    partial class FrmFinan2
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
            this.sbSaldoGeral = new System.Windows.Forms.StatusBar();
            this.sbEntradas = new System.Windows.Forms.StatusBar();
            this.sbSaidas = new System.Windows.Forms.StatusBar();
            this.dgvSaidas = new System.Windows.Forms.DataGridView();
            this.dgvEntradas = new System.Windows.Forms.DataGridView();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaidas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntradas)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.sbSaldoGeral);
            this.panel1.Controls.Add(this.sbEntradas);
            this.panel1.Controls.Add(this.sbSaidas);
            this.panel1.Controls.Add(this.dgvSaidas);
            this.panel1.Controls.Add(this.dgvEntradas);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1173, 549);
            this.panel1.TabIndex = 0;
            // 
            // sbSaldoGeral
            // 
            this.sbSaldoGeral.Dock = System.Windows.Forms.DockStyle.None;
            this.sbSaldoGeral.Location = new System.Drawing.Point(33, 439);
            this.sbSaldoGeral.Name = "sbSaldoGeral";
            this.sbSaldoGeral.Size = new System.Drawing.Size(230, 22);
            this.sbSaldoGeral.TabIndex = 9;
            // 
            // sbEntradas
            // 
            this.sbEntradas.Dock = System.Windows.Forms.DockStyle.None;
            this.sbEntradas.Location = new System.Drawing.Point(33, 179);
            this.sbEntradas.Name = "sbEntradas";
            this.sbEntradas.Size = new System.Drawing.Size(100, 22);
            this.sbEntradas.TabIndex = 8;
            // 
            // sbSaidas
            // 
            this.sbSaidas.Dock = System.Windows.Forms.DockStyle.None;
            this.sbSaidas.Location = new System.Drawing.Point(33, 485);
            this.sbSaidas.Name = "sbSaidas";
            this.sbSaidas.Size = new System.Drawing.Size(230, 22);
            this.sbSaidas.TabIndex = 7;
            // 
            // dgvSaidas
            // 
            this.dgvSaidas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvSaidas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvSaidas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvSaidas.Location = new System.Drawing.Point(33, 218);
            this.dgvSaidas.Name = "dgvSaidas";
            this.dgvSaidas.Size = new System.Drawing.Size(1042, 215);
            this.dgvSaidas.TabIndex = 1;
            // 
            // dgvEntradas
            // 
            this.dgvEntradas.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dgvEntradas.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEntradas.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dgvEntradas.Location = new System.Drawing.Point(33, 23);
            this.dgvEntradas.Name = "dgvEntradas";
            this.dgvEntradas.Size = new System.Drawing.Size(1042, 150);
            this.dgvEntradas.StandardTab = true;
            this.dgvEntradas.TabIndex = 0;
            this.dgvEntradas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgvEntradas_KeyDown);
            this.dgvEntradas.Click += new System.EventHandler(this.dgvEntradas_Click_1);
            // 
            // FrmFinan2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1173, 549);
            this.Controls.Add(this.panel1);
            this.Name = "FrmFinan2";
            this.Text = "FrmFinan2";
            this.Load += new System.EventHandler(this.FrmFinan2_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvSaidas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEntradas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.DataGridView dgvEntradas;
        private System.Windows.Forms.DataGridView dgvSaidas;
        private System.Windows.Forms.StatusBar sbSaldoGeral;
        private System.Windows.Forms.StatusBar sbEntradas;
        private System.Windows.Forms.StatusBar sbSaidas;
    }
}