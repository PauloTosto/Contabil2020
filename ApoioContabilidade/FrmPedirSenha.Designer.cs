namespace ApoioContabilidade
{
    partial class FrmPedirSenha
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblMensagem = new System.Windows.Forms.Label();
            this.lblUsuario = new System.Windows.Forms.Label();
            this.txUsuario = new System.Windows.Forms.TextBox();
            this.lblSenha = new System.Windows.Forms.Label();
            this.txSenha = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnConfigurar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            //
            // lblMensagem
            //
            this.lblMensagem.Location = new System.Drawing.Point(15, 15);
            this.lblMensagem.Name = "lblMensagem";
            this.lblMensagem.Size = new System.Drawing.Size(380, 35);
            this.lblMensagem.TabIndex = 0;
            this.lblMensagem.Text = "Autentique-se com usuário e senha:";
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Location = new System.Drawing.Point(15, 60);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new System.Drawing.Size(60, 17);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuário:";
            //
            // txUsuario
            //
            this.txUsuario.Location = new System.Drawing.Point(110, 57);
            this.txUsuario.Name = "txUsuario";
            this.txUsuario.Size = new System.Drawing.Size(285, 22);
            this.txUsuario.TabIndex = 2;
            //
            // lblSenha
            //
            this.lblSenha.AutoSize = true;
            this.lblSenha.Location = new System.Drawing.Point(15, 95);
            this.lblSenha.Name = "lblSenha";
            this.lblSenha.Size = new System.Drawing.Size(50, 17);
            this.lblSenha.TabIndex = 3;
            this.lblSenha.Text = "Senha:";
            //
            // txSenha
            //
            this.txSenha.Location = new System.Drawing.Point(110, 92);
            this.txSenha.Name = "txSenha";
            this.txSenha.Size = new System.Drawing.Size(285, 22);
            this.txSenha.TabIndex = 3;
            this.txSenha.UseSystemPasswordChar = true;
            //
            // btnOk
            //
            this.btnOk.Location = new System.Drawing.Point(195, 135);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(95, 30);
            this.btnOk.TabIndex = 4;
            this.btnOk.Text = "&OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            //
            // btnCancelar
            //
            this.btnCancelar.Location = new System.Drawing.Point(300, 135);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new System.Drawing.Size(95, 30);
            this.btnCancelar.TabIndex = 5;
            this.btnCancelar.Text = "&Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = true;
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            //
            // btnConfigurar (2026-05-12)
            //
            this.btnConfigurar.Location = new System.Drawing.Point(15, 180);
            this.btnConfigurar.Name = "btnConfigurar";
            this.btnConfigurar.Size = new System.Drawing.Size(180, 28);
            this.btnConfigurar.TabIndex = 6;
            this.btnConfigurar.Text = "Configurar caminhos...";
            this.btnConfigurar.UseVisualStyleBackColor = true;
            this.btnConfigurar.Click += new System.EventHandler(this.btnConfigurar_Click);
            //
            // FrmPedirSenha
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 225);
            this.Controls.Add(this.btnConfigurar);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txSenha);
            this.Controls.Add(this.lblSenha);
            this.Controls.Add(this.txUsuario);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.lblMensagem);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FrmPedirSenha";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Autenticação";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblMensagem;
        private System.Windows.Forms.Label lblUsuario;
        private System.Windows.Forms.TextBox txUsuario;
        private System.Windows.Forms.Label lblSenha;
        private System.Windows.Forms.TextBox txSenha;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnConfigurar;
    }
}
