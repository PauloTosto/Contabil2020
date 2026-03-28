
namespace ApoioContabilidade.Trabalho.Ferias
{
    partial class FrmFeriasConsulta
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
            this.gbConsulte = new System.Windows.Forms.GroupBox();
            this.rbGozo = new System.Windows.Forms.RadioButton();
            this.rbPagamento = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dtPagamento = new System.Windows.Forms.DateTimePicker();
            this.dtGozo = new System.Windows.Forms.DateTimePicker();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancele = new System.Windows.Forms.Button();
            this.gbConsulte.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbConsulte
            // 
            this.gbConsulte.Controls.Add(this.rbPagamento);
            this.gbConsulte.Controls.Add(this.rbGozo);
            this.gbConsulte.Location = new System.Drawing.Point(55, 21);
            this.gbConsulte.Name = "gbConsulte";
            this.gbConsulte.Size = new System.Drawing.Size(376, 63);
            this.gbConsulte.TabIndex = 0;
            this.gbConsulte.TabStop = false;
            this.gbConsulte.Text = "Consulte";
            // 
            // rbGozo
            // 
            this.rbGozo.AutoSize = true;
            this.rbGozo.Checked = true;
            this.rbGozo.Location = new System.Drawing.Point(27, 21);
            this.rbGozo.Name = "rbGozo";
            this.rbGozo.Size = new System.Drawing.Size(63, 21);
            this.rbGozo.TabIndex = 0;
            this.rbGozo.TabStop = true;
            this.rbGozo.Text = "Gozo";
            this.rbGozo.UseVisualStyleBackColor = true;
            // 
            // rbPagamento
            // 
            this.rbPagamento.AutoSize = true;
            this.rbPagamento.Location = new System.Drawing.Point(204, 22);
            this.rbPagamento.Name = "rbPagamento";
            this.rbPagamento.Size = new System.Drawing.Size(101, 21);
            this.rbPagamento.TabIndex = 1;
            this.rbPagamento.Text = "Pagamento\r\n";
            this.rbPagamento.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(61, 117);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(171, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Mes Inicio Gozo de Férias";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 161);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(177, 17);
            this.label2.TabIndex = 3;
            this.label2.Text = "Mes Pagamento  de Férias";
            // 
            // dtPagamento
            // 
            this.dtPagamento.CustomFormat = "MM/yyyy";
            this.dtPagamento.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtPagamento.Location = new System.Drawing.Point(300, 162);
            this.dtPagamento.Name = "dtPagamento";
            this.dtPagamento.Size = new System.Drawing.Size(82, 22);
            this.dtPagamento.TabIndex = 4;
            // 
            // dtGozo
            // 
            this.dtGozo.CustomFormat = "MM/yyyy";
            this.dtGozo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtGozo.Location = new System.Drawing.Point(299, 118);
            this.dtGozo.Name = "dtGozo";
            this.dtGozo.Size = new System.Drawing.Size(82, 22);
            this.dtGozo.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnCancele);
            this.panel1.Controls.Add(this.btnOK);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 247);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(470, 100);
            this.panel1.TabIndex = 6;
            // 
            // btnOK
            // 
            this.btnOK.Location = new System.Drawing.Point(47, 36);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(124, 23);
            this.btnOK.TabIndex = 0;
            this.btnOK.Text = "&Ok";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancele
            // 
            this.btnCancele.Location = new System.Drawing.Point(255, 37);
            this.btnCancele.Name = "btnCancele";
            this.btnCancele.Size = new System.Drawing.Size(124, 23);
            this.btnCancele.TabIndex = 1;
            this.btnCancele.Text = "&Cancele";
            this.btnCancele.UseVisualStyleBackColor = true;
            // 
            // FrmFeriasConsulta
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 347);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.dtGozo);
            this.Controls.Add(this.dtPagamento);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.gbConsulte);
            this.Name = "FrmFeriasConsulta";
            this.Text = "Consulta Férias";
            this.gbConsulte.ResumeLayout(false);
            this.gbConsulte.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gbConsulte;
        private System.Windows.Forms.RadioButton rbPagamento;
        private System.Windows.Forms.RadioButton rbGozo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtPagamento;
        private System.Windows.Forms.DateTimePicker dtGozo;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnCancele;
        private System.Windows.Forms.Button btnOK;
    }
}