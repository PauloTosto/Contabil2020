using System.Windows.Forms;
namespace ApoioContabilidade
{
    partial class FrmFinan
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
            this.dgSaidas = new DataGrid();
            this.dgEntradas = new DataGrid();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgSaidas)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.sbSaldoGeral);
            this.panel1.Controls.Add(this.sbEntradas);
            this.panel1.Controls.Add(this.sbSaidas);
            this.panel1.Controls.Add(this.dgSaidas);
            this.panel1.Controls.Add(this.dgEntradas);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1083, 507);
            this.panel1.TabIndex = 0;
            // 
            // sbSaldoGeral
            // 
            this.sbSaldoGeral.Dock = System.Windows.Forms.DockStyle.None;
            this.sbSaldoGeral.Location = new System.Drawing.Point(34, 414);
            this.sbSaldoGeral.Name = "sbSaldoGeral";
            this.sbSaldoGeral.Size = new System.Drawing.Size(230, 22);
            this.sbSaldoGeral.TabIndex = 6;
            // 
            // sbEntradas
            // 
            this.sbEntradas.Dock = System.Windows.Forms.DockStyle.None;
            this.sbEntradas.Location = new System.Drawing.Point(34, 124);
            this.sbEntradas.Name = "sbEntradas";
            this.sbEntradas.Size = new System.Drawing.Size(100, 22);
            this.sbEntradas.TabIndex = 5;
            // 
            // sbSaidas
            // 
            this.sbSaidas.Dock = System.Windows.Forms.DockStyle.None;
            this.sbSaidas.Location = new System.Drawing.Point(34, 460);
            this.sbSaidas.Name = "sbSaidas";
            this.sbSaidas.Size = new System.Drawing.Size(230, 22);
            this.sbSaidas.TabIndex = 4;
            // 
            // dgSaidas
            // 
            this.dgSaidas.DataMember = "";
            this.dgSaidas.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgSaidas.Location = new System.Drawing.Point(11, 152);
            this.dgSaidas.Name = "dgSaidas";
            this.dgSaidas.Size = new System.Drawing.Size(996, 256);
            this.dgSaidas.TabIndex = 1;
            // 
            // dgEntradas
            // 
            this.dgEntradas.AllowNavigation = false;
            this.dgEntradas.CaptionText = "ENTRADAS";
            this.dgEntradas.DataMember = "";
            this.dgEntradas.HeaderForeColor = System.Drawing.SystemColors.ControlText;
            this.dgEntradas.Location = new System.Drawing.Point(11, 3);
            this.dgEntradas.Name = "dgEntradas";
            this.dgEntradas.Size = new System.Drawing.Size(996, 113);
            this.dgEntradas.TabIndex = 0;
            this.dgEntradas.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.dgEntradas_PreviewKeyDown);
            this.dgEntradas.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dgEntradas_KeyDown);
            this.dgEntradas.Click += new System.EventHandler(this.dgEntradas_Click);
            // 
            // FrmFinan
            // 
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1083, 507);
            this.Controls.Add(this.panel1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "FrmFinan";
            this.Text = "Form3";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.FrmFinan_PreviewKeyDown);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmFinan_KeyDown);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgSaidas)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private DataGrid dgSaidas;
        private DataGrid dgEntradas;
        private System.Windows.Forms.StatusBar sbEntradas;
        private System.Windows.Forms.StatusBar sbSaidas;

        private System.Windows.Forms.StatusBar sbSaldoGeral;
    }
}