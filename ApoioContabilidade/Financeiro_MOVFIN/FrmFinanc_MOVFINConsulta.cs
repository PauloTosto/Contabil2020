using ApoioContabilidade.Financeiro;
using ApoioContabilidade.PagarReceber.ServicesLocais;
using ApoioContabilidade.PagarReceber;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using static ApoioContabilidade.Financeiro_MOVFIN.Servicos.ServicosFiltroDBF;

namespace ApoioContabilidade.Financeiro_MOVFIN
{
    public partial class FrmFinanc_MOVFINConsulta : Form
    {
        public FrmFinanc_MOVFINConsulta()
        {
            InitializeComponent();
            
            this.btnFiltro.Click += new System.EventHandler(this.btnFiltro_Click);
            this.btnSair.Click += new System.EventHandler(this.btnSair_Click);
            this.btnConsulta.EnabledChanged += new System.EventHandler(this.btnConsulta_EnabledChanged);
            this.btnConsulta.Click += new System.EventHandler(this.btnConsulta_Click);

            btnFiltro.Enabled = false;
            btnConsulta.Enabled = false;
            oPesqFin = new Pesquise();//TPesquisa.Create(self);
            oPesqFin.Parent = this;
            oPesqFin.TabIndex = 0;
            oPesqFin.Left = 0;
            oPesqFin.Top = this.Top + pnTop.Top + pnTop.Height + 2;
            oPesqFin.Width = pnTop.Width;
            oPesqFin.Height = (this.ClientSize.Height - oPesqFin.Top);
            TabelasIniciaisConfigura();
        }

        private void TabelasIniciaisConfigura()
        {
            try
            {
                btnFiltro.Enabled = TabelasIniciaisDBF.Execute();
                // while (!TabelasIniciais.TabelasIniciaisOk())
                // { }
            }
            catch (Exception)
            {
                MessageBox.Show("Tabelas Iniciais não encontradas");
                throw;
            }


            btnFiltro.Enabled = true;


            btnConsulta.Enabled = btnFiltro.Enabled;
        }

        private void btnFiltro_Click(object sender, EventArgs e)
        {
            ArmeEdicao oArme = ArmeLinhasMestre();
            if (oPesqFin.Pagina("GERAL") != null) return;
            if (oArme == null) return;
            oPesqFin.Linhas = oArme.Linhas;
            oPesqFin.NovaPagina("GERAL");
            oPesqFin.Pagina("GERAL").Text = "&1) Lançamentos";

            oPesqFin.SelectedIndex = 0;


        }
        private ArmeEdicao ArmeLinhasMestre()
        {

            // usarei neste filtro as funcoes que retornam pedaços de código sql
            // assim a filtragem praticamente acontece no servidor
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;
            olinha = new Linha("substring(DEBITO,1,2)/substring(CREDITO,1,2)");
            olinha.cabecalho[0] = "Bancos";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);


            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contas";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;

            oArme.Linhas.Add(olinha);



            olinha = new Linha("HIST");
            olinha.cabecalho[0] = "Historico";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("FORN");
            olinha.cabecalho[0] = "Titular";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DOC_FISC");
            olinha.cabecalho[0] = "Doc.Fiscal";
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
            olinha.ofuncaoSql = RetornaCodigoSql.CompareValor;
            oArme.Linhas.Add(olinha);


            olinha = new Linha("");
            olinha.cabecalho[0] = "Transf.Financ.:";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");

            ((ComboBox)olinha.oedite[0]).Items.Add("- Sem Transferencias");
            ((ComboBox)olinha.oedite[0]).Items.Add("= Só as Transferencias");
            ((ComboBox)olinha.oedite[0]).Width = 150;
            olinha.ofuncaoSql = RetornaCodigoSql.exameTransf_Fin;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contabilidade";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("S Classificados");
            ((ComboBox)olinha.oedite[0]).Items.Add("N Não Classificados");
            olinha.ofuncaoSqlDictionary = RetornaCodigoSql.passa_Contab_Fin;
            oArme.Linhas.Add(olinha);


            olinha = new Linha("");
            olinha.cabecalho[0] = "Apropriados:";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");

            ((ComboBox)olinha.oedite[0]).Items.Add("Só os Apropriados(Gerencial ou Almoxarifado)");
            ((ComboBox)olinha.oedite[0]).Items.Add("Não Apropriados");
            ((ComboBox)olinha.oedite[0]).Width = 250;
            olinha.ofuncaoSql = RetornaCodigoSql.passaAprop_Fin;
            oArme.Linhas.Add(olinha);




            olinha = new Linha("DOC");
            olinha.cabecalho[0] = "Documento";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);
            return oArme;
        }

        /*
         *   '* Normal'
      '- Sem Transferencias'
      '= S'#243' as Transferencias')

        '* Normal'
      'S'#243' os Apropriados(Gerencial ou Almoxarifado)'
      'N'#227'o Apropriados')



         * */



        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnConsulta_Click(object sender, EventArgs e)
        {
            //if (retorno)
            // {
            //  PesquisaGenerico oPesquisa = new PesquisaGenerico(DadosComum.dsPesquisa.Copy());
            //  retorno = FiltroFinan.FiltroDadosPagar(oPesqFin, oPesquisa);
            //}
            bool retorno =  ConsultaServidor();

            if (retorno)
            {

                FrmFinan_MOVFIN oform = new FrmFinan_MOVFIN();
                oform.frmFinanc_MOVFINConsulta = this;
                oform.Show();
            }
            else
            {
                MessageBox.Show("Acesso Falhou");
            }
        }
        public bool ConsultaServidor()
        {
            bool retorno = false;
            retorno =  FiltroFinanDBF.PesquisaServidorVelho(dtData1.Value, dtData2.Value, oPesqFin);

            return retorno;
        }

        private void btnConsulta_EnabledChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Enabled)
            {
                if (!DadosComumDBF.tabelasJaConfiguradas())
                    DadosComumDBF.TabelasConfigCombos();
            }
        }

       
    }
}
