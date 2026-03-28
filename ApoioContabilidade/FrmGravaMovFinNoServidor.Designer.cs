namespace ApoioContabilidade
{
    partial class FrmGravaMovFinNoServidor
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
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dtData2 = new System.Windows.Forms.DateTimePicker();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.btnImobil = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(205, 116);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(289, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Grave no MOVFIN do Servidor";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.dtData2);
            this.groupBox1.Controls.Add(this.dtData1);
            this.groupBox1.Location = new System.Drawing.Point(201, 19);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 66);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Perio&do";
            // 
            // dtData2
            // 
            this.dtData2.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData2.Location = new System.Drawing.Point(162, 26);
            this.dtData2.Margin = new System.Windows.Forms.Padding(4);
            this.dtData2.Name = "dtData2";
            this.dtData2.Size = new System.Drawing.Size(123, 22);
            this.dtData2.TabIndex = 1;
            // 
            // dtData1
            // 
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(30, 26);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.Size = new System.Drawing.Size(123, 22);
            this.dtData1.TabIndex = 0;
            // 
            // btnImobil
            // 
            this.btnImobil.Location = new System.Drawing.Point(204, 214);
            this.btnImobil.Name = "btnImobil";
            this.btnImobil.Size = new System.Drawing.Size(289, 23);
            this.btnImobil.TabIndex = 2;
            this.btnImobil.Text = "Grave no IMOBIL do Servidor";
            this.btnImobil.UseVisualStyleBackColor = true;
            this.btnImobil.Click += new System.EventHandler(this.btnImobil_Click);
            // 
            // FrmGravaMovFinNoServidor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnImobil);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button1);
            this.Name = "FrmGravaMovFinNoServidor";
            this.Text = "FrmGravaMovFinNoServidor";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        public System.Windows.Forms.DateTimePicker dtData2;
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.Button btnImobil;
    }
}