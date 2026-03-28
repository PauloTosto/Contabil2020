namespace ApoioContabilidade
{
    partial class FrmImprimeFolha
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
            this.label2 = new System.Windows.Forms.Label();
            this.cbSetores = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.datapg = new System.Windows.Forms.DateTimePicker();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnimprime = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnExportaFinan = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbcat_diarista = new System.Windows.Forms.RadioButton();
            this.rbcat_mensal = new System.Windows.Forms.RadioButton();
            this.rbcat_todos = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbbanco_sim = new System.Windows.Forms.RadioButton();
            this.rbbanco_todos = new System.Windows.Forms.RadioButton();
            this.rbbanco_nao = new System.Windows.Forms.RadioButton();
            this.button1 = new System.Windows.Forms.Button();
            this.sbTotalExtra = new System.Windows.Forms.StatusBar();
            this.dgEntradas = new System.Windows.Forms.DataGridView();
            this.sbEntradas = new System.Windows.Forms.StatusBar();
            this.panel1 = new System.Windows.Forms.Panel();
            this.printDialog1 = new System.Windows.Forms.PrintDialog();
            this.printDocument1 = new System.Drawing.Printing.PrintDocument();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(146, 12);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 13);
            this.label2.TabIndex = 12;
            this.label2.Text = "Setores";
            // 
            // cbSetores
            // 
            this.cbSetores.FormattingEnabled = true;
            this.cbSetores.Location = new System.Drawing.Point(149, 27);
            this.cbSetores.Name = "cbSetores";
            this.cbSetores.Size = new System.Drawing.Size(170, 21);
            this.cbSetores.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Data Pagamento";
            // 
            // datapg
            // 
            this.datapg.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datapg.Location = new System.Drawing.Point(21, 28);
            this.datapg.Name = "datapg";
            this.datapg.Size = new System.Drawing.Size(81, 20);
            this.datapg.TabIndex = 9;
            this.datapg.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.btnimprime);
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.btnExportaFinan);
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.cbSetores);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.datapg);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1020, 73);
            this.panel2.TabIndex = 4;
            // 
            // btnimprime
            // 
            this.btnimprime.Location = new System.Drawing.Point(467, 25);
            this.btnimprime.Name = "btnimprime";
            this.btnimprime.Size = new System.Drawing.Size(75, 23);
            this.btnimprime.TabIndex = 21;
            this.btnimprime.Text = "Imprime";
            this.btnimprime.UseVisualStyleBackColor = true;
            this.btnimprime.Click += new System.EventHandler(this.btnimprime_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(921, 22);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 20;
            this.button2.Text = "Excel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnExportaFinan
            // 
            this.btnExportaFinan.Location = new System.Drawing.Point(766, 35);
            this.btnExportaFinan.Name = "btnExportaFinan";
            this.btnExportaFinan.Size = new System.Drawing.Size(109, 21);
            this.btnExportaFinan.TabIndex = 19;
            this.btnExportaFinan.Text = "Exporta Financeiro";
            this.btnExportaFinan.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbcat_diarista);
            this.groupBox2.Controls.Add(this.rbcat_mensal);
            this.groupBox2.Controls.Add(this.rbcat_todos);
            this.groupBox2.Location = new System.Drawing.Point(609, 11);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(266, 45);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Categoria";
            // 
            // rbcat_diarista
            // 
            this.rbcat_diarista.AutoSize = true;
            this.rbcat_diarista.Location = new System.Drawing.Point(191, 17);
            this.rbcat_diarista.Name = "rbcat_diarista";
            this.rbcat_diarista.Size = new System.Drawing.Size(65, 17);
            this.rbcat_diarista.TabIndex = 2;
            this.rbcat_diarista.Text = "Diaristas";
            this.rbcat_diarista.UseVisualStyleBackColor = true;
            // 
            // rbcat_mensal
            // 
            this.rbcat_mensal.AutoSize = true;
            this.rbcat_mensal.Location = new System.Drawing.Point(86, 17);
            this.rbcat_mensal.Name = "rbcat_mensal";
            this.rbcat_mensal.Size = new System.Drawing.Size(80, 17);
            this.rbcat_mensal.TabIndex = 1;
            this.rbcat_mensal.Text = "Mensalistas";
            this.rbcat_mensal.UseVisualStyleBackColor = true;
            // 
            // rbcat_todos
            // 
            this.rbcat_todos.AutoSize = true;
            this.rbcat_todos.Checked = true;
            this.rbcat_todos.Location = new System.Drawing.Point(25, 17);
            this.rbcat_todos.Name = "rbcat_todos";
            this.rbcat_todos.Size = new System.Drawing.Size(55, 17);
            this.rbcat_todos.TabIndex = 0;
            this.rbcat_todos.TabStop = true;
            this.rbcat_todos.Text = "Todos";
            this.rbcat_todos.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbbanco_sim);
            this.groupBox1.Controls.Add(this.rbbanco_todos);
            this.groupBox1.Controls.Add(this.rbbanco_nao);
            this.groupBox1.Location = new System.Drawing.Point(548, 11);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 45);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Conta Bancaria";
            // 
            // rbbanco_sim
            // 
            this.rbbanco_sim.AutoSize = true;
            this.rbbanco_sim.Location = new System.Drawing.Point(90, 17);
            this.rbbanco_sim.Name = "rbbanco_sim";
            this.rbbanco_sim.Size = new System.Drawing.Size(42, 17);
            this.rbbanco_sim.TabIndex = 18;
            this.rbbanco_sim.Text = "Sim";
            this.rbbanco_sim.UseVisualStyleBackColor = true;
            // 
            // rbbanco_todos
            // 
            this.rbbanco_todos.AutoSize = true;
            this.rbbanco_todos.Checked = true;
            this.rbbanco_todos.Location = new System.Drawing.Point(6, 17);
            this.rbbanco_todos.Name = "rbbanco_todos";
            this.rbbanco_todos.Size = new System.Drawing.Size(55, 17);
            this.rbbanco_todos.TabIndex = 17;
            this.rbbanco_todos.TabStop = true;
            this.rbbanco_todos.Text = "Todos";
            this.rbbanco_todos.UseVisualStyleBackColor = true;
            // 
            // rbbanco_nao
            // 
            this.rbbanco_nao.AutoSize = true;
            this.rbbanco_nao.Location = new System.Drawing.Point(161, 17);
            this.rbbanco_nao.Name = "rbbanco_nao";
            this.rbbanco_nao.Size = new System.Drawing.Size(45, 17);
            this.rbbanco_nao.TabIndex = 15;
            this.rbbanco_nao.Text = "Não";
            this.rbbanco_nao.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(766, 8);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(109, 21);
            this.button1.TabIndex = 7;
            this.button1.Text = "Exporta BB";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // sbTotalExtra
            // 
            this.sbTotalExtra.CausesValidation = false;
            this.sbTotalExtra.Dock = System.Windows.Forms.DockStyle.None;
            this.sbTotalExtra.Location = new System.Drawing.Point(11, 338);
            this.sbTotalExtra.Name = "sbTotalExtra";
            this.sbTotalExtra.Size = new System.Drawing.Size(232, 22);
            this.sbTotalExtra.SizingGrip = false;
            this.sbTotalExtra.TabIndex = 6;
            // 
            // dgEntradas
            // 
            this.dgEntradas.Location = new System.Drawing.Point(0, 3);
            this.dgEntradas.Name = "dgEntradas";
            this.dgEntradas.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgEntradas.Size = new System.Drawing.Size(1018, 278);
            this.dgEntradas.TabIndex = 0;
            // 
            // sbEntradas
            // 
            this.sbEntradas.CausesValidation = false;
            this.sbEntradas.Dock = System.Windows.Forms.DockStyle.None;
            this.sbEntradas.Location = new System.Drawing.Point(11, 287);
            this.sbEntradas.Name = "sbEntradas";
            this.sbEntradas.Size = new System.Drawing.Size(232, 22);
            this.sbEntradas.SizingGrip = false;
            this.sbEntradas.TabIndex = 5;
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.AutoScrollMargin = new System.Drawing.Size(20, 20);
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.sbTotalExtra);
            this.panel1.Controls.Add(this.sbEntradas);
            this.panel1.Controls.Add(this.dgEntradas);
            this.panel1.Location = new System.Drawing.Point(0, 74);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1020, 473);
            this.panel1.TabIndex = 3;
            // 
            // printDialog1
            // 
            this.printDialog1.UseEXDialog = true;
            // 
            // printDocument1
            // 
            this.printDocument1.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(this.printDocument1_PrintPage_1);
            // 
            // FrmImprimeFolha
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 447);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "FrmImprimeFolha";
            this.Text = "FrmImprimeFolha";
            this.Load += new System.EventHandler(this.FrmImprimeFolha_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgEntradas)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cbSetores;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker datapg;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnExportaFinan;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton rbcat_diarista;
        private System.Windows.Forms.RadioButton rbcat_mensal;
        private System.Windows.Forms.RadioButton rbcat_todos;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbbanco_sim;
        private System.Windows.Forms.RadioButton rbbanco_todos;
        private System.Windows.Forms.RadioButton rbbanco_nao;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.StatusBar sbTotalExtra;
        private System.Windows.Forms.DataGridView dgEntradas;
        private System.Windows.Forms.StatusBar sbEntradas;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PrintDialog printDialog1;
        private System.Drawing.Printing.PrintDocument printDocument1;
        private System.Windows.Forms.Button btnimprime;
    }
}