
namespace ApoioContabilidade.Trabalho.FormsPesquisa
{
    partial class FrmPesquisaCkList
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
            this.pnDecida = new System.Windows.Forms.Panel();
            this.btnCancel = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.pnGeral = new System.Windows.Forms.Panel();
            this.dgvEscolha = new System.Windows.Forms.DataGridView();
            this.pnDecida.SuspendLayout();
            this.pnGeral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvEscolha)).BeginInit();
            this.SuspendLayout();
            // 
            // pnDecida
            // 
            this.pnDecida.Controls.Add(this.btnCancel);
            this.pnDecida.Controls.Add(this.button1);
            this.pnDecida.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnDecida.Location = new System.Drawing.Point(0, 412);
            this.pnDecida.Name = "pnDecida";
            this.pnDecida.Size = new System.Drawing.Size(648, 67);
            this.pnDecida.TabIndex = 0;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(186, 20);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 37);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "&Cancele";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button1.Location = new System.Drawing.Point(98, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 37);
            this.button1.TabIndex = 6;
            this.button1.Text = "&Ok";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // pnGeral
            // 
            this.pnGeral.Controls.Add(this.dgvEscolha);
            this.pnGeral.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnGeral.Location = new System.Drawing.Point(0, 0);
            this.pnGeral.Name = "pnGeral";
            this.pnGeral.Size = new System.Drawing.Size(648, 406);
            this.pnGeral.TabIndex = 1;
            // 
            // dgvEscolha
            // 
            this.dgvEscolha.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvEscolha.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvEscolha.Location = new System.Drawing.Point(0, 0);
            this.dgvEscolha.Name = "dgvEscolha";
            this.dgvEscolha.RowHeadersWidth = 51;
            this.dgvEscolha.RowTemplate.Height = 24;
            this.dgvEscolha.Size = new System.Drawing.Size(648, 406);
            this.dgvEscolha.TabIndex = 0;
            // 
            // FrmPesquisaCkList
            // 
            this.AcceptButton = this.button1;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(648, 479);
            this.Controls.Add(this.pnGeral);
            this.Controls.Add(this.pnDecida);
            this.Name = "FrmPesquisaCkList";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Escolha Serviço";
            this.pnDecida.ResumeLayout(false);
            this.pnGeral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvEscolha)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnDecida;
        private System.Windows.Forms.Panel pnGeral;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.DataGridView dgvEscolha;
    }
}