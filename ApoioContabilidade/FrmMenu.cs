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
            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + "ConfigPath.txt"))
            {
                ListaCaminhos.Paths = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + "ConfigPath.txt");
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
            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML))
            {
                Dictionary<string, string> ListaCaminhosXML = AcessosStream.LeiaLista(Environment.SpecialFolder.LocalApplicationData + Configura_XML_NF.fileConfigXML);
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

                contabilidadeToolStripMenuItem.Visible = sucesso;
                apiFinanceiroMenuItem.Visible = sucessoServidor;
                transferenciasContabilidadeToolStripMenuItem.Visible = sucessoServidor && sucesso;
                trabalhoToolStripMenuItem.Visible = sucessoServidor;
                toolStripFiscais.Visible = sucessoServidor;
                produçãoToolStripMenuItem.Visible = sucessoServidor;

                atualizouTabelas = await TabelasIniciais.Execute();
            }
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

        private void relacionaPlaconXAlterDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FrmPlaconRelaciona oform = new FrmPlaconRelaciona();
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
