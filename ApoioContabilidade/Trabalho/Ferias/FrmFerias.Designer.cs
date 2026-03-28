
namespace ApoioContabilidade.Trabalho.Ferias
{
    partial class FrmFerias
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
            this.dgvFerias = new System.Windows.Forms.DataGridView();
            this.btnAdiciona = new System.Windows.Forms.Button();
            this.btnLancaGerencial = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFerias)).BeginInit();
            this.SuspendLayout();
            // 
            // dgvFerias
            // 
            this.dgvFerias.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvFerias.Location = new System.Drawing.Point(10, 32);
            this.dgvFerias.Name = "dgvFerias";
            this.dgvFerias.RowHeadersWidth = 51;
            this.dgvFerias.RowTemplate.Height = 24;
            this.dgvFerias.Size = new System.Drawing.Size(1122, 533);
            this.dgvFerias.TabIndex = 1;
            // 
            // btnAdiciona
            // 
            this.btnAdiciona.Location = new System.Drawing.Point(78, 590);
            this.btnAdiciona.Name = "btnAdiciona";
            this.btnAdiciona.Size = new System.Drawing.Size(75, 23);
            this.btnAdiciona.TabIndex = 2;
            this.btnAdiciona.Text = "&Adiciona";
            this.btnAdiciona.UseVisualStyleBackColor = true;
            // 
            // btnLancaGerencial
            // 
            this.btnLancaGerencial.Location = new System.Drawing.Point(227, 590);
            this.btnLancaGerencial.Name = "btnLancaGerencial";
            this.btnLancaGerencial.Size = new System.Drawing.Size(157, 23);
            this.btnLancaGerencial.TabIndex = 3;
            this.btnLancaGerencial.Text = "Lança no Gerencial";
            this.btnLancaGerencial.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(474, 590);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(188, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "&Imprime Aviso e Recibo";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // FrmFerias
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1141, 642);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnLancaGerencial);
            this.Controls.Add(this.btnAdiciona);
            this.Controls.Add(this.dgvFerias);
            this.Name = "FrmFerias";
            this.Text = "FrmFerias";
            this.Load += new System.EventHandler(this.FrmFerias_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFerias)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvFerias;
        private System.Windows.Forms.Button btnAdiciona;
        private System.Windows.Forms.Button btnLancaGerencial;
        private System.Windows.Forms.Button button3;
    }
}