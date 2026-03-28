using ApoioContabilidade.PagarReceber;
using ApoioContabilidade.PagarReceber.ServicesLocais;
using ApoioContabilidade.Services;
using ClassConexao;
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

namespace ApoioContabilidade.Financeiro
{
    public partial class FrmFinancConsulta : Form
    {
        public FrmFinancConsulta()
        {
            InitializeComponent();
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
            olinha.oedite[0] =     new TextBox();
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


         * oPesqFin.Linhas.Add('CTAFIN');  // Débito e Crédito da Conta Financ
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Bancos';
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] := maskedit5;

  oPesqFin.Linhas.Add('DEBITO/CREDITO');

  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Contas';
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] := maskedit1;
  //oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,1] := EditNum1;

  oPesqFin.Linhas.Add('HIST');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Historico';
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] := maskedit2;

  oPesqFin.Linhas.Add('FORN');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Titular';
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] := maskedit3;

  oPesqFin.Linhas.Add('DOC_FISC');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Doc.Fiscal';
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] := maskedit4;

  oPesqFin.Linhas.Add('VALOR');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Valor:';
  oPesqFin.Linhas.Funcao[oPesqFin.Linhas.Count-1,0] := @AdoCompareValor;
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] :=  CBValor;
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,1] := EdValor;

  oPesqFin.Linhas.Add('');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Transf.Financ.:';
  oPesqFin.Linhas.Funcao[oPesqFin.Linhas.Count-1,0] := @ExameTransf_Fin;
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] :=  CBTransferencia;

  oPesqFin.Linhas.Add('DEBITO/CREDITO');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Contabilidade:';
  oPesqFin.Linhas.Funcao[oPesqFin.Linhas.Count-1,0] := @FPassa_Contab_Fin;
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] :=  CBClassificados;

  oPesqFin.Linhas.Add('');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Apropriados:';
  oPesqFin.Linhas.Funcao[oPesqFin.Linhas.Count-1,0] := @FPassa_Aprop_fin;
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] :=  CBApropriados;

  oPesqFin.Linhas.Add('DOC');
  oPesqFin.Linhas.Cabecalho[oPesqFin.Linhas.Count-1,0] := 'Documento';
  oPesqFin.Linhas.OEdite[oPesqFin.Linhas.Count-1,0] := Maskedit6;



         * */



        private void btnSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private async void btnConsulta_Click(object sender, EventArgs e)
        {
            //if (retorno)
            // {
            //  PesquisaGenerico oPesquisa = new PesquisaGenerico(DadosComum.dsPesquisa.Copy());
            //  retorno = FiltroFinan.FiltroDadosPagar(oPesqFin, oPesquisa);
            //}
            bool retorno = await ConsultaServidor();

            if (retorno)
            {

                FrmFinanceiro oform = new FrmFinanceiro();
                oform.frmFinancConsulta = this;
                oform.Show();
            }
            else
            {
                MessageBox.Show("Acesso Falhou");
            }
        }
        public async Task<bool> ConsultaServidor()
        {
            bool retorno = false;
            retorno = await FiltroFinan.PesquisaServidor(dtData1.Value, dtData2.Value, oPesqFin);

            return retorno;
        }

        private void btnConsulta_EnabledChanged(object sender, EventArgs e)
        {
            if ((sender as Control).Enabled)
            {
                if (!DadosComum.tabelasJaConfiguradas())
                    DadosComum.TabelasConfigCombos();
            }
        }

       
    }
}
