namespace ApoioContabilidade
{
    partial class FrmPlaconRelaciona
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
            this.pnGeral = new System.Windows.Forms.Panel();
            this.dgvRelaciona = new System.Windows.Forms.DataGridView();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.button1 = new System.Windows.Forms.Button();
            this.pnTop.SuspendLayout();
            this.pnGeral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRelaciona)).BeginInit();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.button1);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(1349, 70);
            this.pnTop.TabIndex = 0;
            // 
            // pnGeral
            // 
            this.pnGeral.Controls.Add(this.dgvRelaciona);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnGeral.Location = new System.Drawing.Point(0, 73);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(1349, 374);
            this.pnGeral.TabIndex = 1;
            // 
            // dgvRelaciona
            // 
            this.dgvRelaciona.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvRelaciona.Location = new System.Drawing.Point(84, 19);
            this.dgvRelaciona.Name = "dgvRelaciona";
            this.dgvRelaciona.RowHeadersWidth = 51;
            this.dgvRelaciona.RowTemplate.Height = 24;
            this.dgvRelaciona.Size = new System.Drawing.Size(1156, 328);
            this.dgvRelaciona.TabIndex = 0;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(456, 12);
            this.button1.Name = "btnImporta";
            this.button1.Size = new System.Drawing.Size(151, 27);
            this.button1.TabIndex = 0;
            this.button1.Text = "Importa Excel Relaciona";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btnImporta_Click);
            // 
            // FrmPlaconRelaciona
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1349, 447);
            this.Controls.Add(this.pnGeral);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmPlaconRelaciona";
            this.Text = "FrmPlaconRelaciona";
            this.pnTop.ResumeLayout(false);
            this.pnGeral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRelaciona)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.DataGridView dgvRelaciona;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}