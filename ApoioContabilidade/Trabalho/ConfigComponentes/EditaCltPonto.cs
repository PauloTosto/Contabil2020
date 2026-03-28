using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.ServicesTrab;
using ApoioContabilidade.UserControls;
using ClassFiltroEdite;
using ClassFiltroEdite.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.ConfigComponentes
{
    public class EditaCltPonto
    {
        public MonteGrid oTrabCad;
        public MonteGrid oTrabMov;
        public MonteGrid oTrabNoturno;
        public MonteGrid oTrabMovAnterior;
        public MonteGrid oTrabNoturnoAnterior;
        public MonteGrid oTrabPremio;


        public MonteGrid oAtividade;
        public MonteGrid oProdutividade;
        public MonteGrid oResumo;


       // public ArmeEdicao EdtTrabcad;
        public ArmeEdicao EdtTrabMov;
        public ArmeEdicao EdtTrabNoturno;
        public ArmeEdicao EdtTrabPremio;
        // public ArmeEdicao EdtTrabMovAnterior;
        // public ArmeEdicao EdtTrabNoturnoAnterior;

        ServicoFiltroTrab evt_Ponto;
        // permite o acesso a todos os bmSources deste grupo
        //private Evt_PgRc evt_Fin_Aux;


        //  private System.Windows.Forms.ErrorProvider errorProvider1;
        public EditaCltPonto(ServicoFiltroTrab oEvt_Ponto)
        {
            evt_Ponto = oEvt_Ponto;
        }
        public void MonteGrids()
        {
            oTrabCad = new MonteGrid();
            oTrabCad.Clear();
            oTrabCad.AddValores("CODCAD", "Codigo", 6,"" ,false, 0,"");
            oTrabCad.AddValores("NOMECAD", "Nome Trabalhador", 45,"", false, 0, "");
            oTrabCad.AddValores("GLECAD", "C.R.", 6, "",false, 0,"");
            oTrabCad.AddValores("DIARIAS", "Diarias", 8, "###0.00", true, 0,"");
            oTrabCad.AddValores("HORAS50", "H 50", 6, "##0.00", true, 0, "");
            oTrabCad.AddValores("HORAS100", "H Dom", 6, "##0.00", true, 0, "");
            oTrabCad.AddValores("HORASFE", "H Fer.", 6, "##0.00", true, 0,"");
            oTrabCad.AddValores("DIARIASN", "Noturno", 7, "##0.00", true, 0, "");
           // oTrabCad.AddValores("VALOREMP", "Tarefas", 7, "###0.00", true, 0, "");

            oTrabCad.AddValores("CRITICA", "Critica", 21, "", false, 0, "");
            oTrabCad.AddValores("APONTAMENTOPROV", "Apontamento Prov.", 18, "", false, 0, "");

            oTrabMov = new MonteGrid();
            oTrabMov.Clear();
            oTrabMov.AddValores("FAZMOV", "Centro", 6, "", false, 0, "");
            oTrabMov.AddValores("BL", "Quadra", 6, "", false, 0, "");
            oTrabMov.AddValores("NUM_MOD", "Modelo", 25, "", false, 0, "");
            oTrabMov.AddValores("CODSER", "Serviço", 25, "", false, 0, "");
            oTrabMov.AddValores("DIA1", "Qui", 5, "", false, 0, "");
            oTrabMov.AddValores("DIA2", "Sex", 5, "", false, 0, "");
            oTrabMov.AddValores("DIA3", "Sab", 5, "", false, 0, "");
            oTrabMov.AddValores("DIA4", "Dom", 5, "", false, 0, "");
            oTrabMov.AddValores("DIA5", "Seg", 5, "", false, 0, "");
            oTrabMov.AddValores("DIA6", "Ter", 5, "", false, 0, "");
            oTrabMov.AddValores("DIA7", "Qua", 5, "", false, 0, "");
            oTrabMov.AddValores("OK", "Ok", 4, "", false, 0, "");
            oTrabMov.AddValores("QUANT", "Quant.", 10, "####0.00", false, 0, "");
            oTrabMov.AddValores("DIARIA", "Diaria", 10, "####0.00", false, 0, "");


            oTrabNoturno = new MonteGrid();
            oTrabNoturno.Clear();
            oTrabNoturno.AddValores("FAZMOV", "Centro", 6, "", false, 0, "");
            oTrabNoturno.AddValores("BL", "Quadra", 6, "", false, 0, "");
            oTrabNoturno.AddValores("NUM_MOD", "Modelo", 25, "", false, 0, "");
            oTrabNoturno.AddValores("CODSER", "Serviço", 25, "", false, 0, "");
            oTrabNoturno.AddValores("DIA1", "Qui", 5, "", false, 0, "");
            oTrabNoturno.AddValores("DIA2", "Sex", 5, "", false, 0, "");
            oTrabNoturno.AddValores("DIA3", "Sab", 5, "", false, 0, "");
            oTrabNoturno.AddValores("DIA4", "Dom", 5, "", false, 0, "");
            oTrabNoturno.AddValores("DIA5", "Seg", 5, "", false, 0, "");
            oTrabNoturno.AddValores("DIA6", "Ter", 5, "", false, 0, "");
            oTrabNoturno.AddValores("DIA7", "Qua", 5, "", false, 0, "");
            oTrabNoturno.AddValores("OK", "Ok", 4, "", false, 0, "");
            oTrabNoturno.AddValores("QUANT", "Quant.", 10, "####0.00", false, 0, "");
            oTrabNoturno.AddValores("DIARIA", "Diaria", 10, "####0.00", false, 0, "");







            oTrabMovAnterior = new MonteGrid();
            oTrabMovAnterior.Clear();
            oTrabMovAnterior.AddValores("DATA", "Semana", 10, "", false, 0, "");
            oTrabMovAnterior.AddValores("FAZMOV", "Centro", 6, "", false, 0, "");
            oTrabMovAnterior.AddValores("BL", "Quadra", 6, "", false, 0, "");
            oTrabMovAnterior.AddValores("NUM_MOD", "Modelo", 25, "", false, 0, "");
            oTrabMovAnterior.AddValores("CODSER", "Serviço", 25, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA1", "Qui", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA2", "Sex", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA3", "Sab", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA4", "Dom", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA5", "Seg", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA6", "Ter", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("DIA7", "Qua", 5, "", false, 0, "");
            oTrabMovAnterior.AddValores("OK", "Ok", 4, "", false, 0, "");
            oTrabMovAnterior.AddValores("QUANT", "Quant.", 10, "####0.00", false, 0, "");
            oTrabMovAnterior.AddValores("DIARIA", "Diaria", 10, "####0.00", false, 0, "");
            oTrabMovAnterior.AddValores("NOTURNO", "Not", 3, "", false, 0, "");


            oTrabNoturnoAnterior = new MonteGrid();
            oTrabNoturnoAnterior.Clear();
            oTrabNoturnoAnterior.AddValores("FAZMOV", "Centro", 6, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("BL", "Quadra", 6, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("NUM_MOD", "Modelo", 25, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("CODSER", "Serviço", 25, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA1", "Qui", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA2", "Sex", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA3", "Sab", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA4", "Dom", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA5", "Seg", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA6", "Ter", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIA7", "Qua", 5, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("OK", "Ok", 3, "", false, 0, "");
            oTrabNoturnoAnterior.AddValores("QUANT", "Quant.", 10, "###,##0.00", false, 0, "");
            oTrabNoturnoAnterior.AddValores("DIARIA", "Diaria", 10, "###,##0.00", false, 0, "");


            oTrabPremio = new MonteGrid();
            oTrabPremio.Clear();
            oTrabPremio.AddValores("FAZMOV", "Centro", 6, "", false, 0, "");
            oTrabPremio.AddValores("BL", "Quadra", 6, "", false, 0, "");
            oTrabPremio.AddValores("NUM_MOD", "Modelo", 25, "", false, 0, "");
            oTrabPremio.AddValores("CODSER", "Serviço", 25, "", false, 0, "");
            oTrabPremio.AddValores("DTA_INI", "Inicio", 10, "", false, 0, "");
            oTrabPremio.AddValores("DTA_FIM", "Fim", 10, "", false, 0, "");
            oTrabPremio.AddValores("VALOR", "Valor Premio", 12, "####0.00", false, 0, "");
        }

        //// EDIÇÃO
        public bool MonteEditCltPonto(BindingSource bindSoure)
        {
            bool result = false;
            if ((oTrabMov == null) || (oTrabMov.oDataGridView == null) || (oTrabMov.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtTrabMov= new ArmeEdicao(bindSoure);
                MonteEditeTrabMov(bindSoure);
                EdtTrabMov.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool MonteEditCltPontoNoturno(BindingSource bindSoure)
        {
            bool result = false;
            if ((oTrabNoturno == null) || (oTrabNoturno.oDataGridView == null) || (oTrabNoturno.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtTrabNoturno = new ArmeEdicao(bindSoure);
                MonteEditeTrabNoturno(bindSoure);
                EdtTrabNoturno.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool MonteEditCltPontoPremio(BindingSource bindSoure)
        {
            bool result = false;
            if ((oTrabPremio == null) || (oTrabPremio.oDataGridView == null) || (oTrabPremio.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtTrabPremio = new ArmeEdicao(bindSoure);
                MonteEditeTrabPremio(bindSoure);
                EdtTrabPremio.MonteEdicao();
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }



        // Centros Resultado
        const int GLEBA_trabMov = 0;
        const int QUADRA_trabMov = 1;
        const int MODELO_trabMov = 2;
        const int SERVICO_trabMov = 3;
        const int Dia1_trabMov = 4;
        const int Dia2_trabMov = 5;
        const int Dia3_trabMov = 6;
        const int Dia4_trabMov = 7;
        const int Dia5_trabMov = 8;
        const int Dia6_trabMov = 9;
        const int Dia7_trabMov = 10;
        const int OK_trabMov = 11;
        const int Quant_trabMov = 12;
        const int Diaria_trabMov = 13;

        private BindingSource glebaSource;
        private BindingSource quadraSource;
        private BindingSource servicoSource;

        private BindingSource SourceTrabMov;

        private void MonteEditeTrabMov(BindingSource bmTrabMov)
        {
            try
            {
                SourceTrabMov = bmTrabMov;

                // modelos-Serviço
                BindingSource modeloSource = new BindingSource();
                modeloSource.DataSource = DadosComum.ModeloCombo.Copy().AsDataView();

                servicoSource = new BindingSource();
                servicoSource.DataSource = DadosComum.ServicoModeloCombo.Copy().AsDataView();


                // setor-glebas-quadras
                BindingSource setorSource = new BindingSource();
                setorSource.DataSource = DadosComum.SetoresCombo.Copy().AsDataView();


                glebaSource = new BindingSource();

                glebaSource.DataSource = DadosComum.GlebasCombo.Copy().AsDataView();



                quadraSource = new BindingSource();
                quadraSource.DataSource = DadosComum.QuadrasCombo.Copy().AsDataView();


                EdtTrabMov.Clear();

                Linha olinha = new Linha("Linha 1");
                ComboMDInverte2 comboset = new ComboMDInverte2();
                comboset.ConfigureComboBoxFilhoNeto(olinha,
                       bmTrabMov, 
                       oTrabMov.oDataGridView.Columns[GLEBA_trabMov].DataPropertyName,
                       oTrabMov.oDataGridView.Columns[QUADRA_trabMov].DataPropertyName,
                       DadosComum.GlebasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                              && row.Field<string>("GLEBA").Trim() != ""
                          ).CopyToDataTable().AsDataView(),
                       DadosComum.QuadrasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                       && row.Field<string>("GLEBA").Trim() != ""
                        && row.Field<string>("QUADRA").Trim() != ""
                    ).CopyToDataTable().AsDataView()

                       );

                EdtTrabMov.Add(olinha);

                olinha = new Linha("Linha 2");
                // TIPO DO COMBOBOX MODELO E SERVICO -> AGREGA NAS DESCRIÇÔES OS CÓDIGOS
                ComboBoxMD2 combomodserv = new ComboBoxMD2(1);
                combomodserv.ConfigureComboBoxPaiFilho(olinha,
                       bmTrabMov,
                       oTrabMov.oDataGridView.Columns[MODELO_trabMov].DataPropertyName,
                       oTrabMov.oDataGridView.Columns[SERVICO_trabMov].DataPropertyName,
                       DadosComum.ModeloCombo_Cod_Descr.Copy().AsDataView(),
                       DadosComum.ServicoModeloCombo_Cod_Descr.Copy().AsDataView());

                EdtTrabMov.Add(olinha);



                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia1_trabMov], new TextBox());
                //olinha.oedite[0].Width = olinha.oedite[0].Width + (2 * 6;
                olinha.oedite[0].Enter += EditaCltPonto_Enter;
                olinha.oedite[0].Validating += EditaCltPonto_Validating;
                olinha.oedite[0].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia2_trabMov], new TextBox());
               // olinha.oedite[1].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[1].Enter += EditaCltPonto_Enter;
                olinha.oedite[1].Validating += EditaCltPonto_Validating;
                olinha.oedite[1].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia3_trabMov], new TextBox());
               // olinha.oedite[2].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[2].Enter += EditaCltPonto_Enter;
                olinha.oedite[2].Validating += EditaCltPonto_Validating;
                olinha.oedite[2].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia4_trabMov], new TextBox());
               // olinha.oedite[3].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[3].Enter += EditaCltPonto_Enter;
                olinha.oedite[3].Validating += EditaCltPonto_Validating;
                olinha.oedite[3].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia5_trabMov], new TextBox());
               // olinha.oedite[4].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[4].Enter += EditaCltPonto_Enter;
                olinha.oedite[4].Validating += EditaCltPonto_Validating;
                olinha.oedite[4].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia6_trabMov], new TextBox());
               // olinha.oedite[5].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[5].Enter += EditaCltPonto_Enter;
                olinha.oedite[5].Validating += EditaCltPonto_Validating;
                olinha.oedite[5].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Dia7_trabMov], new TextBox());
               // olinha.oedite[6].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[6].Enter += EditaCltPonto_Enter;
                olinha.oedite[6].Validating += EditaCltPonto_Validating;
                olinha.oedite[6].Validated += EditaCltPonto_Validated;

                EdtTrabMov.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[OK_trabMov], new TextBox());

                olinha.TextoConfigure(bmTrabMov, oTrabMov.oDataGridView.Columns[Quant_trabMov], new NumericTextBox());
                olinha.oedite[1].Enter += EditaCltPonto_Enter1;

                EdtTrabMov.Add(olinha);

                // teste do control composto       

            }
            catch (Exception E)
            { throw; }
        }


        private void MonteEditeTrabNoturno(BindingSource bmTrabMovNoturno)
        {
            try
            {
                SourceTrabMov = bmTrabMovNoturno;

                // modelos-Serviço
                BindingSource modeloSource = new BindingSource();
                modeloSource.DataSource = DadosComum.ModeloCombo.Copy().AsDataView();

                servicoSource = new BindingSource();
                servicoSource.DataSource = DadosComum.ServicoModeloCombo.Copy().AsDataView();


                // setor-glebas-quadras
                BindingSource setorSource = new BindingSource();
                setorSource.DataSource = DadosComum.SetoresCombo.Copy().AsDataView();


                glebaSource = new BindingSource();

                glebaSource.DataSource = DadosComum.GlebasCombo.Copy().AsDataView();



                quadraSource = new BindingSource();
                quadraSource.DataSource = DadosComum.QuadrasCombo.Copy().AsDataView();


                EdtTrabNoturno.Clear();

                Linha olinha = new Linha("Linha 1");
                ComboMDInverte2 comboset = new ComboMDInverte2();
                comboset.ConfigureComboBoxFilhoNeto(olinha,
                       bmTrabMovNoturno,
                       oTrabNoturno.oDataGridView.Columns[GLEBA_trabMov].DataPropertyName,
                       oTrabNoturno.oDataGridView.Columns[QUADRA_trabMov].DataPropertyName,
                         DadosComum.GlebasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                              && row.Field<string>("GLEBA").Trim() != ""
                          ).CopyToDataTable().AsDataView(),
                       DadosComum.QuadrasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                       && row.Field<string>("GLEBA").Trim() != ""
                        && row.Field<string>("QUADRA").Trim() != ""
                    ).CopyToDataTable().AsDataView()
                     );

                EdtTrabNoturno.Add(olinha);

                olinha = new Linha("Linha 2");
                ComboBoxMD2 combomodserv = new ComboBoxMD2();
                combomodserv.ConfigureComboBoxPaiFilho(olinha,
                       bmTrabMovNoturno,
                       oTrabNoturno.oDataGridView.Columns[MODELO_trabMov].DataPropertyName,
                       oTrabNoturno.oDataGridView.Columns[SERVICO_trabMov].DataPropertyName,
                          DadosComum.ModeloCombo.Copy().AsDataView(),
                       DadosComum.ServicoModeloCombo.Copy().AsDataView());


                EdtTrabNoturno.Add(olinha);



                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia1_trabMov], new TextBox());
                //olinha.oedite[0].Width = olinha.oedite[0].Width + (2 * 6;
                olinha.oedite[0].Enter += EditaCltPonto_Enter;
                olinha.oedite[0].Validating += EditaCltPonto_Validating;
                olinha.oedite[0].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia2_trabMov], new TextBox());
                // olinha.oedite[1].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[1].Enter += EditaCltPonto_Enter;
                olinha.oedite[1].Validating += EditaCltPonto_Validating;
                olinha.oedite[1].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia3_trabMov], new TextBox());
                // olinha.oedite[2].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[2].Enter += EditaCltPonto_Enter;
                olinha.oedite[2].Validating += EditaCltPonto_Validating;
                olinha.oedite[2].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia4_trabMov], new TextBox());
                // olinha.oedite[3].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[3].Enter += EditaCltPonto_Enter;
                olinha.oedite[3].Validating += EditaCltPonto_Validating;
                olinha.oedite[3].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia5_trabMov], new TextBox());
                // olinha.oedite[4].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[4].Enter += EditaCltPonto_Enter;
                olinha.oedite[4].Validating += EditaCltPonto_Validating;
                olinha.oedite[4].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia6_trabMov], new TextBox());
                // olinha.oedite[5].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[5].Enter += EditaCltPonto_Enter;
                olinha.oedite[5].Validating += EditaCltPonto_Validating;
                olinha.oedite[5].Validated += EditaCltPonto_Validated;

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Dia7_trabMov], new TextBox());
                // olinha.oedite[6].Width = olinha.oedite[0].Width + (2 * 6);
                olinha.oedite[6].Enter += EditaCltPonto_Enter;
                olinha.oedite[6].Validating += EditaCltPonto_Validating;
                olinha.oedite[6].Validated += EditaCltPonto_Validated;

                EdtTrabNoturno.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[OK_trabMov], new TextBox());

                olinha.TextoConfigure(bmTrabMovNoturno, oTrabNoturno.oDataGridView.Columns[Quant_trabMov], new NumericTextBox());
                olinha.oedite[1].Enter += EditaCltPonto_Enter1;

                EdtTrabNoturno.Add(olinha);

                // teste do control composto       

            }
            catch (Exception E)
            { throw; }
        }

        const int Inicio_Premio = 4;
        const int Fim_Premio = 5;
        const int Valor_Premio = 6;

        private void MonteEditeTrabPremio(BindingSource bmTrabMovPremio)
        {
            try
            {
                SourceTrabMov = bmTrabMovPremio;

                // modelos-Serviço
                BindingSource modeloSource = new BindingSource();
                modeloSource.DataSource = DadosComum.ModeloCombo.Copy().AsDataView();

                servicoSource = new BindingSource();
                servicoSource.DataSource = DadosComum.ServicoModeloCombo.Copy().AsDataView();


                // setor-glebas-quadras
                BindingSource setorSource = new BindingSource();
                setorSource.DataSource = DadosComum.SetoresCombo.Copy().AsDataView();


                glebaSource = new BindingSource();

                glebaSource.DataSource = DadosComum.GlebasCombo.Copy().AsDataView();



                quadraSource = new BindingSource();
                quadraSource.DataSource = DadosComum.QuadrasCombo.Copy().AsDataView();


                EdtTrabPremio.Clear();

                Linha olinha = new Linha("Linha 1");
                ComboMDInverte2 comboset = new ComboMDInverte2();
                comboset.ConfigureComboBoxFilhoNeto(olinha,
                       bmTrabMovPremio,
                       oTrabPremio.oDataGridView.Columns[GLEBA_trabMov].DataPropertyName,
                       oTrabPremio.oDataGridView.Columns[QUADRA_trabMov].DataPropertyName,
                         DadosComum.GlebasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                              && row.Field<string>("GLEBA").Trim() != ""
                          ).CopyToDataTable().AsDataView(),
                       DadosComum.QuadrasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                       && row.Field<string>("GLEBA").Trim() != ""
                        && row.Field<string>("QUADRA").Trim() != ""
                    ).CopyToDataTable().AsDataView()
                     );

                EdtTrabPremio.Add(olinha);

                olinha = new Linha("Linha 2");
                ComboBoxMD2 combomodserv = new ComboBoxMD2();
                combomodserv.ConfigureComboBoxPaiFilho(olinha,
                       bmTrabMovPremio,
                       oTrabPremio.oDataGridView.Columns[MODELO_trabMov].DataPropertyName,
                       oTrabPremio.oDataGridView.Columns[SERVICO_trabMov].DataPropertyName,
                          DadosComum.ModeloCombo.Copy().AsDataView(),
                       DadosComum.ServicoModeloCombo.Copy().AsDataView());


                EdtTrabPremio.Add(olinha);

               

                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmTrabMovPremio, oTrabPremio.oDataGridView.Columns[Inicio_Premio], new MaskedTextBox());
                ((MaskedTextBox)olinha.oedite[0]).Validated += Inicio_Fim_Validated;

                olinha.TextoConfigure(bmTrabMovPremio, oTrabPremio.oDataGridView.Columns[Fim_Premio], new MaskedTextBox());
                ((MaskedTextBox)olinha.oedite[1]).Validated += Inicio_Fim_Validated;
                EdtTrabPremio.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmTrabMovPremio, oTrabPremio.oDataGridView.Columns[Valor_Premio], new NumericTextBox());
                olinha.oedite[0].Validated += Frm_CampoValidado;
                olinha.oedite[0].Validating += Premio_Valor;
                EdtTrabPremio.Add(olinha);

                // teste do control composto       

            }
            catch (Exception E)
            { throw; }
        }

        private void Inicio_Fim_Validated(object sender, EventArgs e)
        {
            // COMPARAR DATAS DE EMISSAO
            try
            {
                
              // ((MaskedTextBox) EdtMestre.Linhas[4 - 1].oedite[0]).Text = (sender as MaskedTextBox).Text;

            }
            catch (Exception)
            {

                throw;
            }
        }
        void Premio_Valor(object sender, CancelEventArgs e)
        {
            string texto = ((NumericTextBox)sender).Text;
            Decimal valor = 0;
            try
            {
                valor = Convert.ToDecimal(texto);
            }
            catch (Exception)
            {
            }
            if (valor <= Convert.ToDecimal(0.009))
            {
                e.Cancel = true; // NÃO ACEITA CAMPO ZERADO
                                 // { texto = texto.Substring(0, 8); }
                Error_Set(sender as Control, "Valor Não pode ser <= 0");

                try
                {
                    ((NumericTextBox)sender).DataBindings[0].ReadValue();
                }
                catch (Exception)
                {
                }
                return;
            }
        }
        void Frm_CampoValidado(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            Error_Set(sender as Control, "");

        }
        private void EditaCltPonto_Enter1(object sender, EventArgs e)
        {
            // implementar
            /*with Sender as TDBEdit do
      begin

        if (Field.asFloat = 0) and
          (not(trim(Field.DataSet.fieldbyName('ok').asString) = '')) and
          (not(trim(Field.DataSet.fieldbyName('ok').asString) = '')) then
        begin
          try
            Field.asFloat := PegQuantidade(Field.DataSet.fieldbyName('BL').asString,
              Field.DataSet.fieldbyName('CODSER').asString,
              Field.DataSet.fieldbyName('NUM_MOD').asString,
              Field.DataSet.fieldbyName('DATA').asdatetime,
              Field.DataSet.fieldbyName('DATA').asdatetime);
          except
            exit;
          end;
        end
        else
          exit;

             */
        }
        private void EditaCltPonto_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((sender as TextBox).Text.Trim() == "") return;
            string ttext = (sender as TextBox).Text.PadRight(2);
            if (!evt_Ponto.servPonto.ListaCodigo.ContainsKey(ttext))
            {
                e.Cancel = true; 
                Error_Set(sender as Control, "??");

                try
                {
                    ((TextBox)sender).DataBindings[0].ReadValue();
                }
                catch (Exception)
                {
                }
                return;
            }
        }

        private void EditaCltPonto_Validated(object sender, EventArgs e)
        {
            Error_Set(sender as Control, "");
        }
        private void EditaCltPonto_Enter(object sender, EventArgs e)
        {
            string campo = (sender as TextBox).DataBindings[0].BindingMemberInfo.BindingField;
            int ind = -1;
            try
            {
                if (campo.Substring(0, 3) == "DIA")
                    ind = Convert.ToInt32(campo.Substring(3, 1));
            }
            catch (Exception)
            {
                return;
            }
            if (ind == -1) return;
            DataRowView registro = (evt_Ponto.bmTrabCad.Current as DataRowView);
            if (registro == null) return;
            string trab = registro["CODCAD"].ToString();
            DataRow dataRowlst = null;
            dataRowlst = evt_Ponto.servPonto.tabListaTrab.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() ==
                   trab.Trim()).FirstOrDefault();
            if (dataRowlst == null) return;
            if (dataRowlst["COMPATIVEL"+ind.ToString().Trim()].ToString().ToUpper() == "N" )
            {
                (sender as Control).SelectNextControl((sender as Control),true, true,false,false);
            }
           
        }
        private void Error_Set(Control ocontrol, string msg = "Campo Invalido! Redigite")
        {
            try
            {
                System.Windows.Forms.ErrorProvider errorProvider1 = (ocontrol.FindForm() as FormDlgForm).errorProvider1;
                if (errorProvider1 != null)
                    errorProvider1.SetError(ocontrol, msg);
            }
            catch (Exception)
            {

            }
        }
      
        /*        Gleba_TrabMov = 0;
          quadra_TrabMov = 1;
          Modelo_TrabMov = 2;
          Servico_TRabMov = 3;
          Dia1_TRabMov = 4;
          Dia2_TRabMov = 5;
          Dia3_TRabMov = 6;
          Dia4_TRabMov = 7;
          Dia5_TRabMov = 8;
          Dia6_TRabMov = 9;
          Dia7_TRabMov = 10;
          ok_trabMov = 11;
          Quant_trabMov = 12;
          Diaria_trabMov = 13;
                    procedure TFrmNovoTrab_ponto.MonteEdits;
                begin

                  DBCBModelo.Field.onValidate := Valide_Modelo;
                DBCbServicos.Field.onValidate := Valide_Servico;
          DBCBGlebas.Field.onValidate := Valide_Gleba;
          DBCBQuadras.Field.onValidate := Valide_Quadra;

          DBCBModeloN.Field.onValidate := Valide_ModeloN;
          DBCBServicosN.Field.onValidate := Valide_ServicoN;
          DBCBGlebasN.Field.onValidate := Valide_GlebaN;
          DBCBQuadrasN.Field.onValidate := Valide_QuadraN;

          with EditeTrabMov do
          begin
            Clear;
                Add('');

                Cabecalho[Count - 1, 0] := DBGTrabMov.Columns[Gleba_TrabMov].Title.Caption;
            OEdite[Count - 1, 0] := DBCBGlebas;

            Cabecalho[Count - 1, 1] := DBGTrabMov.Columns[quadra_TrabMov].Title.Caption;
            OEdite[Count - 1, 1] := DBCBQuadras;

            Add('');
                Cabecalho[Count - 1, 0] := DBGTrabMov.Columns[Modelo_TrabMov].Title.Caption;
            OEdite[Count - 1, 0] := DBCBModelo;

            Cabecalho[Count - 1, 1] := DBGTrabMov.Columns[Servico_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 1] := DBCbServicos;

            Add('');

                Cabecalho[Count - 1, 0] := DBGTrabMov.Columns[Dia1_TRabMov].Title.Caption;
            OEdite[Count - 1, 0] := CrieDBEDit(DBGTrabMov.Columns[Dia1_TRabMov]);
                with OEdite[Count - 1, 0] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;
            Cabecalho[Count - 1, 1] := DBGTrabMov.Columns[Dia2_TRabMov].Title.Caption;
            OEdite[Count - 1, 1] := CrieDBEDit(DBGTrabMov.Columns[Dia2_TRabMov]);
                with OEdite[Count - 1, 1] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 2] := DBGTrabMov.Columns[Dia3_TRabMov].Title.Caption;
            OEdite[Count - 1, 2] := CrieDBEDit(DBGTrabMov.Columns[Dia3_TRabMov]);
                with OEdite[Count - 1, 2] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 3] := DBGTrabMov.Columns[Dia4_TRabMov].Title.Caption;
            OEdite[Count - 1, 3] := CrieDBEDit(DBGTrabMov.Columns[Dia4_TRabMov]);
                with OEdite[Count - 1, 3] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 4] := DBGTrabMov.Columns[Dia5_TRabMov].Title.Caption;
            OEdite[Count - 1, 4] := CrieDBEDit(DBGTrabMov.Columns[Dia5_TRabMov]);
                with OEdite[Count - 1, 4] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 5] := DBGTrabMov.Columns[Dia6_TRabMov].Title.Caption;
            OEdite[Count - 1, 5] := CrieDBEDit(DBGTrabMov.Columns[Dia6_TRabMov]);
                with OEdite[Count - 1, 5] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 6] := DBGTrabMov.Columns[Dia7_TRabMov].Title.Caption;
            OEdite[Count - 1, 6] := CrieDBEDit(DBGTrabMov.Columns[Dia7_TRabMov]);
                with OEdite[Count - 1, 6] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

       
            Add('');
            Cabecalho[Count - 1, 0] := DBGTrabMov.Columns[ok_trabMov].Title.Caption;
            OEdite[Count - 1, 0] := CrieDBEDit(DBGTrabMov.Columns[ok_trabMov]);

            Cabecalho[Count - 1, 1] := DBGTrabMov.Columns[Quant_trabMov].Title.Caption;
            OEdite[Count - 1, 1] := CrieDBEDit(DBGTrabMov.Columns[Quant_trabMov]);

            with OEdite[Count - 1, 1] as TDBEdit do
            begin
              onEnter := EntraQuant;
              width := width + (2 * 6);
            end;

          end;

          with EditeTrabNoturno do
          begin
            Clear;
            Add('');

            Cabecalho[Count - 1, 0] := DBGTrabNoturno.Columns[Gleba_TrabMov]
              .Title.Caption;
            OEdite[Count - 1, 0] := DBCBGlebasN;

            Cabecalho[Count - 1, 1] := DBGTrabNoturno.Columns[quadra_TrabMov]
              .Title.Caption;
            OEdite[Count - 1, 1] := DBCBQuadrasN;

            Add('');
            Cabecalho[Count - 1, 0] := DBGTrabNoturno.Columns[Modelo_TrabMov]
              .Title.Caption;
            OEdite[Count - 1, 0] := DBCBModeloN;

            Cabecalho[Count - 1, 1] := DBGTrabNoturno.Columns[Servico_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 1] := DBCBServicosN;

            Add('');

            Cabecalho[Count - 1, 0] := DBGTrabNoturno.Columns[Dia1_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 0] := CrieDBEDit(DBGTrabNoturno.Columns[Dia1_TRabMov]);
            with OEdite[Count - 1, 0] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;
            Cabecalho[Count - 1, 1] := DBGTrabNoturno.Columns[Dia2_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 1] := CrieDBEDit(DBGTrabNoturno.Columns[Dia2_TRabMov]);
            with OEdite[Count - 1, 1] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 2] := DBGTrabNoturno.Columns[Dia3_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 2] := CrieDBEDit(DBGTrabNoturno.Columns[Dia3_TRabMov]);
            with OEdite[Count - 1, 2] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 3] := DBGTrabNoturno.Columns[Dia4_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 3] := CrieDBEDit(DBGTrabNoturno.Columns[Dia4_TRabMov]);
            with OEdite[Count - 1, 3] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 4] := DBGTrabNoturno.Columns[Dia5_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 4] := CrieDBEDit(DBGTrabNoturno.Columns[Dia5_TRabMov]);
            with OEdite[Count - 1, 4] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 5] := DBGTrabNoturno.Columns[Dia6_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 5] := CrieDBEDit(DBGTrabNoturno.Columns[Dia6_TRabMov]);
            with OEdite[Count - 1, 5] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            Cabecalho[Count - 1, 6] := DBGTrabNoturno.Columns[Dia7_TRabMov]
              .Title.Caption;
            OEdite[Count - 1, 6] := CrieDBEDit(DBGTrabNoturno.Columns[Dia7_TRabMov]);
            with OEdite[Count - 1, 6] as TDBEdit do
            begin
              onEnter := Passa;
              Field.onValidate := Valide_Codigo;
              width := width + (2 * 6);
            end;

            { Cabecalho[Count - 1, 2] := DBGTrabNoturno.Columns[DIA2_TrabMov].Title.caption;
              OEdite[Count - 1, 2] := CrieDBEDit(DBGTrabNoturno.Columns[DIA2_TrabMov]);
        }

        Add('');
        Cabecalho[Count - 1, 0] := DBGTrabNoturno.Columns[ok_trabMov].Title.Caption;
        OEdite[Count - 1, 0] := CrieDBEDit(DBGTrabNoturno.Columns[ok_trabMov]);

        Cabecalho[Count - 1, 1] := DBGTrabNoturno.Columns[Quant_trabMov]
          .Title.Caption;
        OEdite[Count - 1, 1] := CrieDBEDit(DBGTrabNoturno.Columns[Quant_trabMov]);

        with OEdite[Count - 1, 1] as TDBEdit do
            begin
              onEnter := EntraQuant;
        width:= width + (2 * 6);
        end;

        end;

        // EditeMestre.ArmeEdicao;
        EditeTrabMov.ArmeEdicao;
        EditeTrabNoturno.ArmeEdicao;
        // EditeTrabTarefa.ArmeEdicao;

        end;

        procedure TFrmNovoTrab_ponto.Passa(Sender: TObject);
        var
          ind: Integer;
        begin
          ind := -1;
        with Sender as TDBEdit do
          begin
            if (copy(Field.FieldName, 1, 3) = 'DIA') and(length(Field.FieldName) > 3)
            then
            begin
              try
                ind := strtoint(copy(Field.FieldName, 4, 1));
        except
          exit;
        end;
        end
            else
            exit;
        if not(trim(Field.asString) = '') then
          exit;
        end;
        if ind <> -1 then
        begin
            frmNovoFiltroTrab.AdoLista.Filter := 'CODCAD = ''' +
              frmNovoFiltroTrab.oTRabCad.Otable.fieldbyName('CODCAD').asString + '''';
        frmNovoFiltroTrab.AdoLista.filtered := true;
        // ind_cad := FrmNovoFiltroTrab.ListaAssociado.IndexOf(FrmNovoFiltroTrab.oTrabCad.oTable.FieldByName('CODCAD').asString);
        if frmNovoFiltroTrab.AdoLista.RecordCount > 0 then
        begin
              if frmNovoFiltroTrab.AdoLista.fieldbyName('COMPATIVEL' + inttostr(ind))
                .asString = 'N' then
                SelectNext(Sender as TwinCOntrol, true, true);
        end;
        end;

        end;
        */



    }
}
