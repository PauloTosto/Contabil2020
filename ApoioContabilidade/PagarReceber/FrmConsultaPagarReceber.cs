using ApoioContabilidade.PagarReceber.ServicesLocais;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.PagarReceber
{
    public partial class FrmConsultaPagarReceber : Form
    {
        // private Comum comum;
        int tipoConta;
        DateTime inicio_ult;
        DateTime fim_ult;
        int tipoConta_ult = -1;
        public FrmConsultaPagarReceber()
        {
            InitializeComponent();
            this.KeyPreview = true; // para permitir que o form controle o teclado
           /// this.KeyDown += FrmConsultaPagarReceber_KeyDown;

            tipoConta = 1; // apagar
            btnFiltro.Enabled = false;
            btnConsulta.Enabled = false;
            oPesqFin = new Pesquise();//TPesquisa.Create(self);
            oPesqFin.Parent = this;
            oPesqFin.TabIndex = 0;
            oPesqFin.Left = 0;
            oPesqFin.Top = this.Top + pnTop.Top + pnTop.Height + 2;
            oPesqFin.Width = pnTop.Width;
            oPesqFin.Height = (this.ClientSize.Height - oPesqFin.Top);
          //  oPesqFin.KeyDown += OPesqFin_KeyDown;
           // oPesqFin.KeyPress += OPesqFin_KeyPress;
            TabelasIniciaisConfigura();
            // Comum.MonteGrids();
        }


        private async void TabelasIniciaisConfigura()
        {
            if (!TabelasIniciais.TabelasIniciaisOk())
            {
                try
                {
                    btnFiltro.Enabled = await TabelasIniciais.Execute();
                    // while (!TabelasIniciais.TabelasIniciaisOk())
                    // { }
                }
                catch (Exception)
                {

                    throw;
                }

            }
            else
            {
                btnFiltro.Enabled = true;

            }
            btnConsulta.Enabled = btnFiltro.Enabled;
        }


        private ArmeEdicao ArmeLinhasMestre()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;
            
            if (tipoConta == 1)
            {
                olinha = new Linha("CREDITO");
                olinha.cabecalho[0] = "Fornecedores";
            }
            else
            {
                olinha = new Linha("DEBITO");
                olinha.cabecalho[0] = "Clientes";
            }
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoAprox;
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 250;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

           
            olinha = new Linha("DOC_FISC");
            olinha.cabecalho[0] = "Doc.Fiscal";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("VALOR");
            olinha.cabecalho[0] = "Valor";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("=");
            ((ComboBox)olinha.oedite[0]).Items.Add(">");
            ((ComboBox)olinha.oedite[0]).Items.Add("<");
            ((ComboBox)olinha.oedite[0]).Width = 30;
            olinha.oedite[1] = new NumericTextBox();
            ((NumericTextBox)olinha.oedite[1]).TextAlign = HorizontalAlignment.Right;
            olinha.ofuncao = PesquisaFuncoes.CompareValor;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contabilidade";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("S Classificados");
            ((ComboBox)olinha.oedite[0]).Items.Add("N Não Classificados");
            olinha.ofuncao = PesquisaFuncoes.fPassa_Contab2;
            oArme.Linhas.Add(olinha);

            /* olinha = new Linha("DOC");
             olinha.cabecalho[0] = "Documento";
             olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
             olinha.oedite[0] = new TextBox();
             ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
             oArme.Linhas.Add(olinha);*/
            return oArme;
        }

        private ArmeEdicao ArmeLinhasDetalhe1()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;

            if (tipoConta == 1)
                olinha = new Linha("CREDITO");
            else
                olinha = new Linha("DEBITO");
            olinha.cabecalho[0] = "Bancos";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 150;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("FORN");
            olinha.cabecalho[0] = "Titular";
            olinha.oedite[0] = new TextBox();
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoAprox;
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 250;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("HIST");
            olinha.cabecalho[0] = "Historico";
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 350;
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoAprox;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);
            return oArme;
        }

        private ArmeEdicao ArmeLinhasDetalhe2()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;

            if (tipoConta == 1)
                olinha = new Linha("DEBITO");
            else
                olinha = new Linha("CREDITO");
            olinha.cabecalho[0] = "Contas de Resultado";
            olinha.oedite[0] = new TextBox();
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoAprox;
            olinha.oedite[0].Width = 250;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("HIST");
            olinha.cabecalho[0] = "Historico";
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 350;
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoAprox;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            /* olinha = new Linha("FORN");
             olinha.cabecalho[0] = "Titular";
             olinha.oedite[0] = new TextBox();
             ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
             oArme.Linhas.Add(olinha);*/
            return oArme;
        }
        private ArmeEdicao ArmeLinhasGerencial()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;

            olinha = new Linha("SETOR");

            olinha.cabecalho[0] = "Peq.Empresa";
            olinha.oedite[0] = new TextBox();
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("GLEBA");

            olinha.cabecalho[0] = "Centro Resultado";
            olinha.oedite[0] = new TextBox();
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);


            BindingSource bmtipoMov= new BindingSource();
            bmtipoMov.DataSource = DadosComum.TipoMovCombo.Copy().AsDataView();


            olinha = new Linha("ICODSER");
            olinha.cabecalho[0] = "Tipo do Movimento";
            olinha.oedite[0] = new ComboBox();
            olinha.TextoPesquisaConfigure(olinha.oedite[0]);
            ((ComboBox)olinha.oedite[0]).DataSource = bmtipoMov;//DadosComum.TipoMovCombo.Copy().AsDataView();
            bmtipoMov.Sort = "DESCRICAO";
            ((ComboBox)olinha.oedite[0]).DisplayMember = "DESCRICAO";
            ((ComboBox)olinha.oedite[0]).ValueMember = "TPMOV";
            ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
            ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
            ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

            
            olinha.oedite[0].Width = 250;
            olinha.ofuncao = PesquisaFuncoes.CompareModelo;
            oArme.Linhas.Add(olinha);
            /**/
            //olinha.ofuncao =  @CompareTipoMov
            //cbTipoMov
            //oArme.Linhas.Add(olinha);

            BindingSource bmModelo = new BindingSource();
            bmModelo.DataSource = DadosComum.ModeloCombo.Copy().AsDataView();

            olinha = new Linha("NUM_MOD");
            olinha.cabecalho[0] = "Modelo";
            olinha.oedite[0] = new ComboBox();
            olinha.TextoPesquisaConfigure(olinha.oedite[0]);
            ((ComboBox)olinha.oedite[0]).DataSource =bmModelo;
            bmModelo.Sort = "DESCRICAO";

            ((ComboBox)olinha.oedite[0]).DisplayMember = "DESCRICAO";
            ((ComboBox)olinha.oedite[0]).ValueMember = "NUM_MOD";
            ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
            ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
            ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;


            olinha.oedite[0].Width = 200;
            /*
            ;*/
            olinha.ofuncao = PesquisaFuncoes.CompareModelo;
            //CbModelo;
            oArme.Linhas.Add(olinha);


            return oArme;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            if (oPesqFin.Pagina("Mestre") != null) return;
            ArmeEdicao oArme = ArmeLinhasMestre();

            if (oArme == null) return;
            oPesqFin.Linhas.Clear();
            foreach (Linha olinha in oArme.Linhas)
            {
                oPesqFin.Linhas.Add(olinha);
            }
            // oPesqFin.Linhas = oArme.Linhas;
            oPesqFin.NovaPagina("Mestre");
            oPesqFin.Pagina("Mestre").Text = "1) Lançamentos";
            

            oPesqFin.Linhas.Clear();
            oArme = ArmeLinhasDetalhe1();
            foreach (Linha olinha in oArme.Linhas)
            {
                oPesqFin.Linhas.Add(olinha);
            }
            oPesqFin.NovaPagina("Detalhe1");
            oPesqFin.Pagina("Detalhe1").Text = "2) Prazos";

            
            oPesqFin.Linhas.Clear();
            oArme = ArmeLinhasGerencial();
            foreach (Linha olinha in oArme.Linhas)
            {
                oPesqFin.Linhas.Add(olinha);
            }
            oPesqFin.NovaPagina("DetalheCentro");
            oPesqFin.Pagina("DetalheCentro").Text = "3) Gerencial";


            oPesqFin.Linhas.Clear();
            oArme = ArmeLinhasDetalhe2();
            foreach (Linha olinha in oArme.Linhas)
            {
                oPesqFin.Linhas.Add(olinha);
            }
            oPesqFin.NovaPagina("Detalhe2");

            oPesqFin.Pagina("Detalhe2").Text = "4) Aprop.Contábeis";

            oPesqFin.SelectedIndex = 0;

        }

        private void rbPagar_Click(object sender, EventArgs e)
        {
            tipoConta = 1;
        }

        private void rbReceber_CheckedChanged(object sender, EventArgs e)
        {
            tipoConta = 2;
        }

        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            bool retorno = false;
            /*bool naoPesquiseServidor = false;
            if ((inicio_ult != null) && (fim_ult != null) && (tipoConta_ult != -1))
            {
                if ((inicio_ult <= dtData1.Value) && (fim_ult >= dtData2.Value) && (tipoConta == tipoConta_ult))
                {
                    retorno = true;
                    naoPesquiseServidor = true;
                }
            }*/
            if (!retorno)
            {
                retorno = await FiltroPgRc.PesquisaServidor(dtData1.Value, dtData2.Value, (tipoConta == 1 ? "P" : "R"), oPesqFin);
                inicio_ult = dtData1.Value;
                fim_ult = dtData2.Value;
                tipoConta_ult = tipoConta;
            }
            if (retorno)
            {
                bool naoPesquiseServidor = false; // recurso que preciso estudar como acessar menos vezes o servidor
                PesquisaGenerico oPesquisa = new PesquisaGenerico(DadosComum.dsPesquisa.Copy());
                retorno = FiltroPgRc.FiltroDadosPagar(oPesqFin, oPesquisa, naoPesquiseServidor, dtData1.Value, dtData2.Value);
                if (retorno)
                {
                    FrmPagarReceber oform = new FrmPagarReceber(tipoConta, FiltroPgRc.dsFiltrado.Copy());
                    oform.Show();
                }
            }
            if (!retorno)
            {
                MessageBox.Show("Acesso Falhou");
            }
        }


        private void btnConsulta_EnabledChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Enabled)
            {
                if (!DadosComum.tabelasJaConfiguradas())
                    DadosComum.TabelasConfigCombos();

            }
        }
        private void OPesqFin_KeyDown(object sender, KeyEventArgs e)
        {
            // 
        }
        private void OPesqFin_KeyPress(object sender, KeyPressEventArgs e)
        {
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if (keyData == (Keys.Alt | Keys.D1))
            {
                oPesqFin.SelectTab(0);
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D2))
            {
                oPesqFin.SelectTab(1); 
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D3))
            {
                oPesqFin.SelectTab(2);
                //oPesqFin.TabIndex = 2;
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D4))
            {
                oPesqFin.SelectTab(3);
                //oPesqFin.TabIndex = 3;
                return true;
            }
          

            return base.ProcessDialogKey(keyData);
        }

       
    }
    
}
