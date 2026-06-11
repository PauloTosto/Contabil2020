using System;
using System.Windows.Forms;
using ClassConexao;
using System.Collections.Generic;
using System.IO;
using ApoioContabilidade.Core;
using ApoioContabilidade.PagarReceber;
using ApoioContabilidade.Financeiro;
using ApoioContabilidade.Fiscal.Comum;
using ApoioContabilidade.Fiscal;
using ApoioContabilidade.Contabilidade;
using ApoioContabilidade.Trabalho;
using ApoioContabilidade.Almoxarifado;
using ApoioContabilidade.Services;
using ApoioContabilidade.Produção;
using ApoioContabilidade.Financeiro_MOVFIN;
using ApoioContabilidade.FinVauche_MOVFIN;

namespace ApoioContabilidade
{
    public partial class FrmMenu : Form
    {
        Boolean sucessoServidor;
        Boolean sucesso;
        string path;
        string servidor;
        bool atualizouTabelas = false;
        public FrmMenu()
        {
            sucesso = false;
            sucessoServidor = false;
            path = "";
            servidor = "";
            InitializeComponent();
            //Process.Start(Properties.Resources.bauhaus_medium_bt);
            transferenciasContabilidadeToolStripMenuItem.Visible = false;
            trabalhoToolStripMenuItem.Visible = false;
            toolStripFiscais.Visible = false;
            apiFinanceiroMenuItem.Visible = false;
            produçãoToolStripMenuItem.Visible = false;
            contabilidadeToolStripMenuItem.Visible = false;
        }

        private void VerificaServidores()
        {
            // B8 fix 2026-05-08: usar AppPaths.GetConfigPath em vez de concatenar enum (resolvia caminho relativo ao CWD).
            if (File.Exists(AppPaths.GetConfigPath("ConfigPath.txt")))
            {
                ListaCaminhos.Paths = AcessosStream.LeiaLista(AppPaths.GetConfigPath("ConfigPath.txt"));
            }


            path = ListaCaminhos.GetPath("CONTAB");
            if (path != "")
            {
                sucesso = Directory.Exists(path);
            }
            else { sucesso = false; }


            servidor = ListaCaminhos.GetPath("SERVIDOR_IP");
            if (servidor != "")
            {
                sucessoServidor = TestaConexao.CheckForInternetConnection(servidor);
                ConexaoAtual.SERVIDOR_IP = servidor;
            }
            else { sucessoServidor = false; }


            // carrega os valores para as notas fiscais ( se definido)
            if (File.Exists(AppPaths.GetConfigPath(Configura_XML_NF.fileConfigXML)))
            {
                Dictionary<string, string> ListaCaminhosXML = AcessosStream.LeiaLista(AppPaths.GetConfigPath(Configura_XML_NF.fileConfigXML));
                Configura_XML_NF.PATHNFE = ListaCaminhosXML["CAMINHOXML"];
                Configura_XML_NF.cUF_pad = ListaCaminhosXML["CUF"];
                Configura_XML_NF.CNPJ_pad = ListaCaminhosXML["CNPJ"];
            }

        }

        private void diretorioToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmWinConfigura owinform;
            System.Windows.Forms.DialogResult result;

            owinform = new FrmWinConfigura();
            result = System.Windows.Forms.DialogResult.OK;
            if (owinform.ShowDialog() == result)
            {
            }
            owinform.Dispose();
            VerificaServidores();
            if (((!sucesso) && (!sucessoServidor)))
            { 
                
               if (MessageBox.Show("Configuração Incorreta!! Aplicação será Encerrada. Reconfigura? ", "Mensagem",
                    MessageBoxButtons.YesNo)
                    == DialogResult.No) { this.Close(); }
               else
                {
                    diretorioToolStripMenuItem1.PerformClick();
                    return;
                }
            
            
            }
            contabilidadeToolStripMenuItem.Visible = sucesso;
            apiFinanceiroMenuItem.Visible = sucessoServidor;
            transferenciasContabilidadeToolStripMenuItem.Visible  = sucessoServidor && sucesso;
            trabalhoToolStripMenuItem.Visible = sucessoServidor;
            toolStripFiscais.Visible = sucessoServidor;
            apiFinanceiroMenuItem.Visible = sucessoServidor;
            produçãoToolStripMenuItem.Visible = sucessoServidor;
            if ((sucesso) && (!sucessoServidor)) { MessageBox.Show("Só Modo Contab"); }
            if ((!sucesso) && (sucessoServidor)) { MessageBox.Show("Só Modo Servidor SQL"); }
        }

        private async void FrmMenu_Load(object sender, EventArgs e)
        {
            VerificaServidores();
            if ((!sucesso) && (!sucessoServidor)) // chame o configura se servidor e/ou caminho forem invalidos
            {
                FrmWinConfigura oform = new FrmWinConfigura();
                bool resposta = (oform.ShowDialog() == DialogResult.OK);
                if (resposta) VerificaServidores();
            }
            if (((!sucesso) && (!sucessoServidor)))
            {
                if (MessageBox.Show("Configuração Incorreta!! Aplicação será Encerrada. Reconfigura? ", "Mensagem",
                    MessageBoxButtons.YesNo)
                    == DialogResult.No)
                {
                    this.Close();
                }
                else
                {
                    diretorioToolStripMenuItem1.PerformClick();

                }

            }
            else
            {
                // 2026-05-12: se há config de SERVIDOR_IP mas servidor está inacessível,
                // não adianta nem mostrar login (USUARIOSMLSA é remoto). Força reconfig
                // antes do login. Loop: enquanto inacessível, oferece FrmWinConfigura.
                // Se usuário cancelar, fecha o app. Modo Contab puro (sem SERVIDOR_IP)
                // segue sem login.
                string servidorConfig = ListaCaminhos.GetPath("SERVIDOR_IP");
                while (!string.IsNullOrEmpty(servidorConfig) && !sucessoServidor)
                {
                    var resp = MessageBox.Show(
                        "Servidor configurado (" + servidorConfig + ") não está acessível.\n\n" +
                        "Deseja reconfigurar agora?",
                        "Servidor inacessível", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (resp != DialogResult.Yes)
                    {
                        this.Close();
                        return;
                    }
                    // Modo "só servidor" — CONTAB é local e não impede login.
                    FrmWinConfigura cfg = new FrmWinConfigura(soServidor: true);
                    cfg.ShowDialog();
                    cfg.Dispose();
                    VerificaServidores();
                    servidorConfig = ListaCaminhos.GetPath("SERVIDOR_IP");
                }

                // 2026-05-11: login obrigatório quando há servidor (gerentes acessam direto).
                // Popula SessaoAtual com escopo de setores ANTES de TabelasIniciais.Execute(),
                // pra que GLEBAS já venha filtrada do servidor. Modo Contab puro (sem servidor)
                // não pede login — preserva comportamento de fluxos DBF-only.
                if (sucessoServidor)
                {
                    // Callback pro botão "Configurar caminhos..." dentro do login.
                    // Re-roda VerificaServidores e devolve se servidor agora está OK.
                    Func<bool> reverifica = () =>
                    {
                        VerificaServidores();
                        return sucessoServidor;
                    };
                    bool autenticou = await LoginInicial.ExecutarAsync(this, reverifica);
                    if (!autenticou)
                    {
                        MessageBox.Show("Autenticação cancelada ou falhou. Aplicação será encerrada.",
                            "Login", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Close();
                        return;
                    }
                }

                contabilidadeToolStripMenuItem.Visible = sucesso;
                apiFinanceiroMenuItem.Visible = sucessoServidor;
                transferenciasContabilidadeToolStripMenuItem.Visible = sucessoServidor && sucesso;
                trabalhoToolStripMenuItem.Visible = sucessoServidor;
                toolStripFiscais.Visible = sucessoServidor;
                produçãoToolStripMenuItem.Visible = sucessoServidor;

                atualizouTabelas = await TabelasIniciais.Execute();

                // 2026-05-11: perfil gerente. Quando o usuário tem escopo restrito
                // (>=1 setor em USUARIO_SETORES), esconde menus administrativos.
                // Convenção: presença = gerente, ausência = escritório (vê tudo).
                if (SessaoAtual.TemEscopoRestrito)
                {
                    AplicaPerfilGerente();
                }

                // Mostra na barra do título quem está logado e o escopo (ajuda a depurar
                // "estou vendo dado de qual fazenda?").
                if (!string.IsNullOrEmpty(SessaoAtual.Usuario))
                {
                    string escopo = SessaoAtual.TemEscopoRestrito
                        ? " — Setores: " + string.Join(",", SessaoAtual.Setores)
                        : " — (sem restrição)";
                    this.Text = this.Text + "  [" + SessaoAtual.Usuario + escopo + "]";
                }
            }
        }

        /// <summary>
        /// Esconde menus que não fazem sentido pro perfil gerente. Chamado por
        /// FrmMenu_Load quando SessaoAtual.TemEscopoRestrito = true.
        ///
        /// Defesa em camadas: além de esconder na UI, qualquer endpoint sensível
        /// no servidor deve revalidar (ainda dependente da migração pra ApiMLSAv2).
        /// Por hora, esconder reduz a superfície e evita confusão do operador.
        ///
        /// Adicionado 2026-05-11.
        /// </summary>
        private void AplicaPerfilGerente()
        {
            // Menus de raiz invisíveis pra gerente.
            pagarEReceberToolStripMenuItem.Visible = false;
            contabilidadeToolStripMenuItem.Visible = false;
            apiFinanceiroMenuItem.Visible = false;
            transferenciasContabilidadeToolStripMenuItem.Visible = false;
            toolStripFiscais.Visible = false;
            produçãoToolStripMenuItem.Visible = false;

            // Dentro de Trabalho, mantém só o que faz sentido pro dia-a-dia do gerente.
            // Visíveis: Cadastro (ver dados dos seus trabalhadores), Lançamentos Ponto.
            cltCodigoToolStripMenuItem.Visible = false;        // edição de códigos = admin
            testeToolStripMenuItem.Visible = false;            // debug interno
            relatórioToolStripMenuItem.Visible = false;        // a revisar caso a caso
            pontoMensalRelatórioToolStripMenuItem.Visible = false;
        }

        private void preparaPlaconToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPreparaPlacon1 owinform = new FrmPreparaPlacon1();
            owinform.Show();
        }

       
        private void finanVauchesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmFinVauche ofrmfinvauches = new FrmFinVauche();
            ofrmfinvauches.Show();
        }

       
        

        private void financeiroToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            FrmFinan2 ofrmfinan = new FrmFinan2();
            ofrmfinan.Show();
        }

       
        private void loteSaldoFimDeAnoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmSdoPlano oform = new FrmSdoPlano();
            oform.Show();
        }

        private void razaoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRazao oform = new FrmRazao();
     
            oform.Show();
        }

        private void exportaLoteAlterDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRelaciona oform = new FrmRelaciona();
            oform.Show();
        }

        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void editaPTMOVFINanoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmEdicaoPTMovFin oform = new FrmEdicaoPTMovFin();
     
            oform.Show();
        }

        private void pagarEReceberToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmConsultaPagarReceber oform = new FrmConsultaPagarReceber();
            oform.Show();
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmFinancConsulta oform = new FrmFinancConsulta();
            oform.Show();
        }

        private void configuraXMLToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            FrmCaminhoXML oform = new FrmCaminhoXML();
            oform.Show();
        }

        private void notasFiscaisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmFiscal_Novo oform = new FrmFiscal_Novo();
            oform.Show();
        }

       

        private void cLTPONTOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmConsultaTrab ocltPonto = new FrmConsultaTrab();
            ocltPonto.Show();
        }

        // Adicionado 2026-05-08 (F7): cadastro de códigos de apontamento (CLTCODIG).
        private void cltCodigoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmCltCodig form = new FrmCltCodig();
            form.Show();
        }

        private void testeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Form1 form1 = new Form1();
            // form1.Show();
            FrmSaidaInsumosPonto form1 = new FrmSaidaInsumosPonto();
            form1.Show();
        }

        private void lançamentosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmProd form1 = new FrmProd();
            form1.Show();

        }

       

        private void movimentoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmDadosContabilidade oform = new FrmDadosContabilidade();
            oform.Show();
        }

        private void relatórioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmRelTrabalhista oform = new FrmRelTrabalhista();
            
            oform.Show();
        }

       

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            FrmCadastro oform = new FrmCadastro();
            oform.Show();
        }

        private void editaMOVFINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmFinVauche_MOVFIN oform = new FrmFinVauche_MOVFIN();

            oform.Show();
        }

        private void financeiroMOVFINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmFinan_MOVFIN oform = new FrmFinan_MOVFIN();
            oform.Show();
        }

       

        private void pontoMensalRelatórioToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPontoMes oform = new FrmPontoMes();
            oform.Show();
        }

        private void folhaEInsumosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmTransfereContabilidade oform = new FrmTransfereContabilidade();
            oform.Show();
        }

        private void dbfsServidorMovfinImobilToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmGravaMovFinNoServidor oform = new FrmGravaMovFinNoServidor();
            oform.Show();
        }
    }
}
