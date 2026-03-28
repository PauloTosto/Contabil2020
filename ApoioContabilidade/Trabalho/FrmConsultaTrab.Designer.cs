
using ClassFiltroEdite;

namespace ApoioContabilidade.Trabalho
{
    partial class FrmConsultaTrab
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmConsultaTrab));
            this.pnTop = new System.Windows.Forms.Panel();
            this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
            this.toolStripDatas = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.label1 = new System.Windows.Forms.Label();
            this.dtData1 = new System.Windows.Forms.DateTimePicker();
            this.btnSair = new System.Windows.Forms.Button();
            this.btnConsulta = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txAdministrador = new System.Windows.Forms.TextBox();
            this.pnTop.SuspendLayout();
            this.toolStripDatas.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnTop
            // 
            this.pnTop.Controls.Add(this.txAdministrador);
            this.pnTop.Controls.Add(this.label2);
            this.pnTop.Controls.Add(this.monthCalendar1);
            this.pnTop.Controls.Add(this.toolStripDatas);
            this.pnTop.Controls.Add(this.label1);
            this.pnTop.Controls.Add(this.dtData1);
            this.pnTop.Controls.Add(this.btnSair);
            this.pnTop.Controls.Add(this.btnConsulta);
            this.pnTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnTop.Location = new System.Drawing.Point(0, 0);
            this.pnTop.Name = "pnTop";
            this.pnTop.Size = new System.Drawing.Size(598, 230);
            this.pnTop.TabIndex = 0;
            // 
            // monthCalendar1
            // 
            this.monthCalendar1.FirstDayOfWeek = System.Windows.Forms.Day.Thursday;
            this.monthCalendar1.Location = new System.Drawing.Point(142, 9);
            this.monthCalendar1.Name = "monthCalendar1";
            this.monthCalendar1.ShowToday = false;
            this.monthCalendar1.ShowWeekNumbers = true;
            this.monthCalendar1.TabIndex = 0;
            this.monthCalendar1.Visible = false;
            this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
            // 
            // toolStripDatas
            // 
            this.toolStripDatas.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStripDatas.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.toolStripDatas.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1});
            this.toolStripDatas.Location = new System.Drawing.Point(9, 70);
            this.toolStripDatas.Name = "toolStripDatas";
            this.toolStripDatas.Size = new System.Drawing.Size(98, 27);
            this.toolStripDatas.TabIndex = 5;
            this.toolStripDatas.Text = "toolStrip1";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(85, 24);
            this.toolStripButton1.Text = "Calendário";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(35, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 17);
            this.label1.TabIndex = 4;
            this.label1.Text = "Semana";
            // 
            // dtData1
            // 
            this.dtData1.Checked = false;
            this.dtData1.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtData1.Location = new System.Drawing.Point(14, 44);
            this.dtData1.Margin = new System.Windows.Forms.Padding(4);
            this.dtData1.Name = "dtData1";
            this.dtData1.ShowUpDown = true;
            this.dtData1.Size = new System.Drawing.Size(110, 22);
            this.dtData1.TabIndex = 0;
            this.dtData1.ValueChanged += new System.EventHandler(this.dtData1_ValueChanged);
            // 
            // btnSair
            // 
            this.btnSair.Location = new System.Drawing.Point(487, 34);
            this.btnSair.Margin = new System.Windows.Forms.Padding(4);
            this.btnSair.Name = "btnSair";
            this.btnSair.Size = new System.Drawing.Size(83, 32);
            this.btnSair.TabIndex = 3;
            this.btnSair.Text = "&Sair";
            this.btnSair.UseVisualStyleBackColor = true;
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            // 
            // btnConsulta
            // 
            this.btnConsulta.Location = new System.Drawing.Point(486, 77);
            this.btnConsulta.Margin = new System.Windows.Forms.Padding(4);
            this.btnConsulta.Name = "btnConsulta";
            this.btnConsulta.Size = new System.Drawing.Size(83, 33);
            this.btnConsulta.TabIndex = 1;
            this.btnConsulta.Text = "C&onsulta";
            this.btnConsulta.UseVisualStyleBackColor = true;
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label2.Location = new System.Drawing.Point(537, 212);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "MLSA";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // txAdministrador
            // 
            this.txAdministrador.Location = new System.Drawing.Point(481, 186);
            this.txAdministrador.Name = "txAdministrador";
            this.txAdministrador.PasswordChar = '*';
            this.txAdministrador.Size = new System.Drawing.Size(100, 22);
            this.txAdministrador.TabIndex = 7;
            // 
            // FrmConsultaTrab
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(598, 237);
            this.Controls.Add(this.pnTop);
            this.Name = "FrmConsultaTrab";
            this.Text = "FrmConsultaTrab";
            this.Load += new System.EventHandler(this.FrmConsultaTrab_Load);
            this.pnTop.ResumeLayout(false);
            this.pnTop.PerformLayout();
            this.toolStripDatas.ResumeLayout(false);
            this.toolStripDatas.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Panel pnTop;
        private System.Windows.Forms.Button btnSair;
        private System.Windows.Forms.Button btnConsulta;
        public Pesquise oPesquisa;
        
        public System.Windows.Forms.DateTimePicker dtData1;
        private System.Windows.Forms.ToolStrip toolStripDatas;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MonthCalendar monthCalendar1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private System.Windows.Forms.TextBox txAdministrador;
        private System.Windows.Forms.Label label2;
    }
}