using ApoioContabilidade.Services;
using ApoioContabilidade.UserControls;
using ClassFiltroEdite;
using ClassFiltroEdite.UserControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho
{
    public class Edita_Cad
    {
        public MonteGrid oMestre;
        public MonteGrid oDetalhe;
        public MonteGrid oCorrente;

        public MonteGrid oSalarios;

        public ArmeEdicao EdtMestre;
        public ArmeEdicao EdtDetalhe;
        public ArmeEdicao EdtCorrente;

        public DataTable dtTipoDemi = new DataTable();
        public DataTable dtCor = new DataTable();
        public DataTable dtGrauInst = new DataTable();
        public DataTable dtEstadoCivil = new DataTable();
        public DataTable dtGenero = new DataTable();
       
        Eventos_Cad evt_Cad; 
        public Edita_Cad(Eventos_Cad oevt_Cad)
        {
            evt_Cad = oevt_Cad;
            TransformeDataTable();




        }

        private void TransformeDataTable()
        {
            Dictionary<string, string> dictGenerico;
            dictGenerico = new Dictionary<string, string>();
            dictGenerico.Add("H ", "RESCISÃO C/JUSTA CAUSA,INICIATIVA EMPREGADOR");
            dictGenerico.Add("I1", "RESCISÃO S/JUSTA CAUSA,INICIATIVA EMPREGADOR");
            dictGenerico.Add("B ", "RESCISÃO POR CULPA RECÍPROCA OU FORÇA MAIOR ");
            dictGenerico.Add("I3", "RESCISÃO POR TERMINO DE CONTRATO A TERMO    ");
            dictGenerico.Add("J ", "RESCISÃO POR INICIATIVA DO EMPREGADO        ");
            dictGenerico.Add("L ", "OUTROS MOTIVOS                              ");
            dictGenerico.Add("S2", "FALECIMENTO                                 ");
            dictGenerico.Add("S3", "FALECIMENTO POR ACIDENTE DE TRABALHO        ");
            dictGenerico.Add("", "                                            ");

            dtTipoDemi.Columns.Add("COD", Type.GetType("System.String"));
            dtTipoDemi.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            dtTipoDemi.Columns[dtTipoDemi.Columns.Count - 1].MaxLength = 44;

            foreach (var valor in dictGenerico)
            {
                DataRow orow = dtTipoDemi.NewRow();
                orow["COD"] = valor.Key;
                orow["DESCRICAO"] = valor.Value;
                dtTipoDemi.Rows.Add(orow);
            }
            dtTipoDemi.AcceptChanges();
            /// ETNIA (COR DA PELE)
            dictGenerico = new Dictionary<string, string>();
            dictGenerico.Add("1", "INDIGENA     ");
            dictGenerico.Add("2", "BRANCA       ");
            dictGenerico.Add("4", "PRETA        ");
            dictGenerico.Add("6", "AMARELA      ");
            dictGenerico.Add("8", "PARDA        ");
            dictGenerico.Add("9", "NÃO INFORMADO");

            dtCor.Columns.Add("COD", Type.GetType("System.String"));
            dtCor.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            dtCor.Columns[dtCor.Columns.Count - 1].MaxLength = 13;

            foreach (var valor in dictGenerico)
            {
                DataRow orow = dtCor.NewRow();
                orow["COD"] = valor.Key;
                orow["DESCRICAO"] = valor.Value;
                dtCor.Rows.Add(orow);
            }
            dtCor.AcceptChanges();
            // GRAU DE INSTRUÇ]AO
            dictGenerico = new Dictionary<string, string>();
            dictGenerico.Add("01", "GRAU 01      ");
            dictGenerico.Add("02", "GRAU 02      ");
            dictGenerico.Add("03", "GRAU 03      ");
            dictGenerico.Add("04", "GRAU 04      ");
            dictGenerico.Add("05", "GRAU 05      ");
            dictGenerico.Add("06", "GRAU 06      ");
            dictGenerico.Add("07", "GRAU 07      ");
            dictGenerico.Add("08", "GRAU 08      ");
            dictGenerico.Add("09", "GRAU 09      ");
            dictGenerico.Add("10", "GRAU 10      ");
            dictGenerico.Add("11", "GRAU 11      ");
            dictGenerico.Add("  ", "NÃO INFORMADO");
            dtGrauInst.Columns.Add("COD", Type.GetType("System.String"));
            dtGrauInst.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            dtGrauInst.Columns[dtGrauInst.Columns.Count - 1].MaxLength = 13;

            foreach (var valor in dictGenerico)
            {
                DataRow orow = dtGrauInst.NewRow();
                orow["COD"] = valor.Key;
                orow["DESCRICAO"] = valor.Value;
                dtGrauInst.Rows.Add(orow);
            }
            dtGrauInst.AcceptChanges();

            //estado civil
            dictGenerico = new Dictionary<string, string>();

            dictGenerico.Add("1", "CASADO       ");
            dictGenerico.Add("2", "DIVORCIADO   ");
            dictGenerico.Add("3", "SOLTEIRO     ");
            dictGenerico.Add("4", "UNIAO ESTAVEL");
            dictGenerico.Add("5", "VIUVO        ");
            dtEstadoCivil.Columns.Add("COD", Type.GetType("System.String"));
            dtEstadoCivil.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            dtEstadoCivil.Columns[dtEstadoCivil.Columns.Count - 1].MaxLength = 13;

            foreach (var valor in dictGenerico)
            {
                DataRow orow = dtEstadoCivil.NewRow();
                orow["COD"] = valor.Key;
                orow["DESCRICAO"] = valor.Value;
                dtEstadoCivil.Rows.Add(orow);
            }
            dtEstadoCivil.AcceptChanges();

            // genero
            dictGenerico = new Dictionary<string, string>();

            dictGenerico.Add("M", "MASCULINO");
            dictGenerico.Add("F", "FEMININO ");
            dtGenero.Columns.Add("COD", Type.GetType("System.String"));
            dtGenero.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            dtGenero.Columns[dtGenero.Columns.Count - 1].MaxLength = 9;

            foreach (var valor in dictGenerico)
            {
                DataRow orow = dtGenero.NewRow();
                orow["COD"] = valor.Key;
                orow["DESCRICAO"] = valor.Value;
                dtGenero.Rows.Add(orow);
            }
            dtGenero.AcceptChanges();
        }

        public void MonteGrids()
        {
            oMestre = new MonteGrid();
            oMestre.Clear();
            oMestre.AddValores("NUMERO", "Número", 8, "", false, 0, "");
            oMestre.AddValores("CODCAD", "Codigo", 6, "", false, 0, "");
            oMestre.AddValores("NOMECAD", "Nome Trabalhador", 35, "", false, 0, "");

            oMestre.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            oMestre.AddValores("GLECAD", "Gleba", 6, "", false, 0, "");
            oMestre.AddValores("NASC", "Nasc.", 10, "", false, 0, "");
            oMestre.AddValores("ADMI", "Admissao", 10, "", false, 0, "");

            oMestre.AddValores("COD_ADMI", "Cod.Adm.", 8, "###", false, 0, ""); // verificar formato
            oMestre.AddValores("DEPEND", "Deps.", 6, "", false, 0, "");
            oMestre.AddValores("SALBASE", "Sal.Base", 8, "###0.00000", false, 0, "");
            oMestre.AddValores("CARTTRAB", "Cart.Trab.", 10, "", false, 0, "");

            oMestre.AddValores("Serie", "Serie", 11, "", false, 0, "");
            oMestre.AddValores("EMICTRAB", "Emissao CT", 10, "", false, 0, "");
            oMestre.AddValores("CARTIDENT", "Cart.Ident.", 18, "", false, 0, "");
            oMestre.AddValores("EMICID", "Emissao RG", 10, "", false, 0, "");

            oMestre.AddValores("CTA_FGTS", "Cta.FGTS", 10, "", false, 0, "");
            oMestre.AddValores("OPCAO", "Opcao FGTS", 10, "", false, 0, "");
            oMestre.AddValores("INSCPIS", "Insc.PIS", 14, "", false, 0, "");
            oMestre.AddValores("EMIPIS", "Emissao PIS", 10, "", false, 0, "");


            oMestre.AddValores("CBO", "CBO", 8, "", false, 0, "");
            oMestre.AddValores("CATEGORIA", "Categoria Funcional", 32, "", false, 0, "");
            oMestre.AddValores("CPF", "CPF", 11, "", false, 0, "");
            oMestre.AddValores("MENSALISTA", "Mensalista", 8, "", false, 0, "");

            oMestre.AddValores("DTAFGTS", "Dta Caixa", 10, "", false, 0, "");
            oMestre.AddValores("VLRFGTS", "Sdo FGTS", 12, "##,###,##0.00", false, 0, "");
            oMestre.AddValores("PRAZO", "Prazo Contrato", 10, "", false, 0, "");
            oMestre.AddValores("DEMI", "Demissao", 10, "", false, 0, "");

            oMestre.AddValores("TIPODEMI", "Tp Demissão", 20, "", false, 0, "");
            oMestre.AddValores("AVISO", "Aviso", 10, "", false, 0, "");

            oMestre.AddValores("SALRESC", "Sal.Rescisão", 12, "##,###,##0.00", false, 0, "");
            oMestre.AddValores("IRDEPEND", "I.R.Deps.", 8, "#0.0", false, 0, "");

            oDetalhe = new MonteGrid();
            oDetalhe.Clear();
            oDetalhe.AddValores("NOMECAD", "Nome Trabalhador", 35, "", false, 0, "");
            oDetalhe.AddValores("SEXO", "Genero", 10, "", false, 0, "");
            oDetalhe.AddValores("ESTCIVIL", "Est.Civil", 13, "", false, 0, "");
            oDetalhe.AddValores("COR", "Cor", 10, "", false, 0, "");

            oDetalhe.AddValores("GRAUINST", "Grau Inst.", 12, "", false, 0, "");
            oDetalhe.AddValores("MAE", "Nome da Mãe", 30, "", false, 0, "");
            oDetalhe.AddValores("PAI", "Nome do Pai", 30, "", false, 0, "");
            oDetalhe.AddValores("CONJUGE", "Nome do Conjuge", 30, "", false, 0, "");

            oDetalhe.AddValores("NCIDADE", "LOCAL de NASCIMENTO", 20, "", false, 0, "");
            oDetalhe.AddValores("NUF", "UF", 3, "", false, 0, "");
            oDetalhe.AddValores("TITELEITOR", "Tit.Eleitor", 12, "", false, 0, "");

            oDetalhe.AddValores("RESERV", "Reservista", 8, "", false, 0, "");
            oDetalhe.AddValores("RESERV_CAT", "Catg", 5, "", false, 0, "");
            oDetalhe.AddValores("TPSANGUE", "Tp Sangue", 10, "", false, 0, "");
            oDetalhe.AddValores("END_RUA", "Rua/N./Bairro", 30, "", false, 0, "");

            oDetalhe.AddValores("END_CID", "Cidade", 12, "", false, 0, "");
            oDetalhe.AddValores("END_UF", "UF", 5, "", false, 0, "");
            oDetalhe.AddValores("END_CEP", "CEP", 10, "", false, 0, "");
            oDetalhe.AddValores("DEFIC", "Deficiente?", 10, "", false, 0, "");
            oDetalhe.AddValores("tPDEFIC", "Tp Deficiencia", 10, "", false, 0, "");
            oDetalhe.AddValores("APRENDIZ", "Aprendiz", 10, "", false, 0, "");


            oCorrente = new MonteGrid();
            oCorrente.Clear();
            oCorrente.AddValores("NOMECAD", "Nome Trabalhador", 35, "", false, 0, "");
            oCorrente.AddValores("BANCO1", "BCO", 5, "", false, 0, "");
            oCorrente.AddValores("AGENCIA1", "AGENCIA", 8, "", false, 0, "");
            oCorrente.AddValores("CONTA1", "C/Corrente", 12, "", false, 0, "");
            oCorrente.AddValores("BCOAGCC", "OBS.", 16, "", false, 0, "");

            oSalarios = new MonteGrid();
            oSalarios.AddValores("DATA", "Data", 10, "", false, 0, "");
            oSalarios.AddValores("VLRSALBASE", "Salario Base", 12, "###,##0.00", false, 0, "");
            oSalarios.AddValores("REAJ", "Indice Reaj(%)", 12, "##,##0.00000", false, 0, "");
            oSalarios.AddValores("VLRSALMIN", "Salario Minimo", 12, "###,##0.00", false, 0, "");

        }
        const int NUMERO_A = 0;
        const int CODCAD_A = 1;
        const int NOMECAD_A = 2;

        const int SETOR_A = 3;
        const int GLECAD_A = 4;
        const int NASC_A = 5;
        const int ADMI_A = 6;
        const int COD_ADMI = 7;
        const int DEPEND_A = 8;
        const int SALBASE_A = 9;

        const int CARTTRAB_A = 10;
        const int Serie_A = 11;
        const int EMICTRAB_A = 12;

        const int CARTIDENT_A = 13;
        const int EMIID_A = 14;

        const int CTA_FGTS_A = 15;
        const int OPCAO_A = 16;

        const int INSCPIS_A = 17;
        const int EMIPIS_A = 18;

        const int CBO_A = 19;
        const int CATEGORIA_A = 20;
        const int CPF_A = 21;

        const int MENSALISTA_A = 25 - 3;

        const int DTAFGTS_A = 26 - 3;
        const int VLRFGTS_A = 27 - 3;

        const int PRAZO_A = 28 - 3;
        const int DEMI_A = 29 - 3;
        const int TIPODEMI_A = 30 - 3;
        const int AVISO_A = 31 - 3;
        const int SALRESC_A = 32 - 3;

        public bool MonteEdtMestre(BindingSource bindSoure)
        {
            bool result = false;
            if ((oMestre == null) || (oMestre.oDataGridView == null) || (oMestre.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtMestre = new ArmeEdicao(bindSoure);
                MonteEditeMestre(bindSoure);
                EdtMestre.MonteEdicao();
               // EdtReceber.oFormEdite.FormClosed += OFormEdite_FormClosed_Receber;
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool MonteEdtDetalhe(BindingSource bindSoure)
        {
            bool result = false;
            if ((oDetalhe == null) || (oDetalhe.oDataGridView == null) || (oDetalhe.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtDetalhe = new ArmeEdicao(bindSoure);
                MonteEditeDetalhe(bindSoure);
                EdtDetalhe.MonteEdicao();
                // EdtReceber.oFormEdite.FormClosed += OFormEdite_FormClosed_Receber;
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        public bool MonteEdtCorrente(BindingSource bindSoure)
        {
            bool result = false;
            if ((oCorrente == null) || (oCorrente.oDataGridView == null) || (oCorrente.oDataGridView.DataSource == null))
                return result;
            try
            {
                EdtCorrente = new ArmeEdicao(bindSoure);
                MonteEditeCorrente(bindSoure);
                EdtCorrente.MonteEdicao();
                // EdtReceber.oFormEdite.FormClosed += OFormEdite_FormClosed_Receber;
                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }

        private void MonteEditeMestre(BindingSource bmSource)
        {
            try
            {

                EdtMestre.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CODCAD_A], new MaskedTextBox());
                (olinha.oedite[0] as MaskedTextBox).Mask = "0000";
                olinha.oedite[0].Validating += Edita_Cad_Validating;
                olinha.oedite[0].Validated += Edita_Cad_Validated;

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[NOMECAD_A], new TextBox());
                olinha.oedite[1].Validating += NaoPodeSerNULL_Validating;
                olinha.oedite[1].Validated += Edita_Cad_Validated;

                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 2");
                //ComboMDGlebaSetor comboset = new ComboMDGlebaSetor();
               ComboMDInverte3 comboset = new ComboMDInverte3();
                comboset.ConfigureComboBoxFilhoNeto(olinha,
                       bmSource,
                       oMestre.oDataGridView.Columns[SETOR_A].DataPropertyName,
                      oMestre.oDataGridView.Columns[GLECAD_A].DataPropertyName,
                       DadosComum.SetoresCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                                      ).CopyToDataTable().AsDataView(),
                       DadosComum.GlebasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                       && row.Field<string>("GLEBA").Trim() != ""
                                           ).CopyToDataTable().AsDataView());
                
                EdtMestre.Add(olinha);
                

                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[NASC_A], new MaskedTextBox());
                olinha.oedite[0].Validating += Nascimento_Validating;
                olinha.oedite[0].Validated += Edita_Cad_Validated;

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[ADMI_A], new MaskedTextBox());
                olinha.oedite[1].Validating += Admissao_Validating;
                olinha.oedite[1].Validated += Edita_Cad_Validated;

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[COD_ADMI], new MaskedTextBox());
                (olinha.oedite[2] as MaskedTextBox).Mask = "00";

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[SALBASE_A], new NumericTextBox());
                

                EdtMestre.Add(olinha);


                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CARTTRAB_A], new TextBox());
               
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[Serie_A], new TextBox());

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[EMICTRAB_A], new MaskedTextBox());
                olinha.oedite[2].Validating += AceitaDataVazia_Validating;
                olinha.oedite[2].Validated += Edita_Cad_Validated;


                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CARTIDENT_A], new TextBox());
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[EMIID_A], new MaskedTextBox());
                olinha.oedite[4].Validating += AceitaDataVazia_Validating;
                olinha.oedite[4].Validated += Edita_Cad_Validated;


                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 5");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CTA_FGTS_A], new TextBox());

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[OPCAO_A], new TextBox());

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[INSCPIS_A], new TextBox());


                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[EMIPIS_A], new MaskedTextBox());
                olinha.oedite[3].Validating += AceitaDataVazia_Validating;
                olinha.oedite[3].Validated += Edita_Cad_Validated;

                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 6");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CBO_A], new MaskedTextBox());
                (olinha.oedite[0] as MaskedTextBox).Mask = "99999";

                ComboBox ocombox;
                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = DadosComum.Categoria; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "DESCRICAO";
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CATEGORIA_A], ocombox);
                olinha.oedite[1].Width = 200;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[CPF_A], new TextBox());

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[MENSALISTA_A], new MaskedTextBox());
                (olinha.oedite[3] as MaskedTextBox).Mask = ">L"; //maiuscula
                //(olinha.oedite[3] as TextBox).CharacterCasing = CharacterCasing.Upper;
                olinha.oedite[3].Validating += Mensalista_Validating;
                olinha.oedite[3].Validated += Edita_Cad_Validated;

                EdtMestre.Add(olinha);


                olinha = new Linha("Linha 7");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[DTAFGTS_A], new MaskedTextBox());
                olinha.oedite[0].Validating += AceitaDataVazia_Validating;
                olinha.oedite[0].Validated += Edita_Cad_Validated;
               
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[VLRFGTS_A], new NumericTextBox());
            
                EdtMestre.Add(olinha);

                olinha = new Linha("Linha 8");
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[DEMI_A], new MaskedTextBox());
                olinha.oedite[0].Validating += AceitaDataVazia_Validating;
                olinha.oedite[0].Validated += Edita_Cad_Validated;

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = dtTipoDemi; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[TIPODEMI_A], ocombox);
                olinha.oedite[1].Width = 250;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)olinha.oedite[1]).Enter += Edita_Cad_Enter;
                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[AVISO_A], new MaskedTextBox());
                olinha.oedite[2].Enter += Edita_Cad_Enter;
                //olinha.oedite[2].Width = olinha.oedite[2].Width * 2;

                olinha.TextoConfigure(bmSource, oMestre.oDataGridView.Columns[SALRESC_A], new NumericTextBox());
                olinha.oedite[3].Enter += Edita_Cad_Enter;


                EdtMestre.Add(olinha);

                /*

                */
                // teste do control composto       

            }
            catch (Exception E)
            { throw; }

        }

        const int SEXO = 1;
        const int ESTCIVIL = 2;
        const int COR = 3;
        const int GRAUINST = 4;

        const int MAE = 5;
        const int PAI = 6;
        const int CONJUGE = 7;
        const int NCIDADE = 8;

        const int NUF = 9;
        const int TITELEITOR = 10;
        const int RESERV = 11;
        const int RESERV_CAT = 12;

        const int TPSANGUE = 13;
        const int END_RUA = 14;
        const int END_CID = 15;
        const int END_UF = 16;

        const int END_CEP = 17;
        const int DEFIC = 18;
        const int TPDEFIC = 19;
        const int APRENDIZ = 20;

   
        private void MonteEditeDetalhe(BindingSource bmSource)
        {
            try
            {

                EdtDetalhe.Clear();

                Linha olinha = new Linha("Linha 1");
              
                ComboBox ocombox;
                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = dtGenero; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[SEXO], ocombox);
                olinha.oedite[0].Width = 100;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 3;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = dtEstadoCivil; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[ESTCIVIL], ocombox);
                olinha.oedite[1].Width = 150;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = dtCor; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[COR], ocombox);
                olinha.oedite[2].Width = 150;
                ((ComboBox)olinha.oedite[2]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[2]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[2]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[2]).AutoCompleteSource = AutoCompleteSource.ListItems;

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";//
                ocombox.DataSource = dtGrauInst; // 

                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[GRAUINST], ocombox);
                olinha.oedite[3].Width = 150;
                ((ComboBox)olinha.oedite[3]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[3]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[3]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[3]).AutoCompleteSource = AutoCompleteSource.ListItems;


                EdtDetalhe.Add(olinha);

                
                olinha = new Linha("Linha 2");
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[MAE], new TextBox());
                //olinha.oedite[0].Validating += AceitaDataVazia_Validating;
                //olinha.oedite[0].Validated += Edita_Cad_Validated;

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[PAI], new TextBox());
                EdtDetalhe.Add(olinha);
                
                olinha = new Linha("Linha 3");
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[CONJUGE], new TextBox());

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[NCIDADE], new TextBox());

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[NUF], new TextBox());
                EdtDetalhe.Add(olinha);


                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[TITELEITOR], new TextBox());

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[RESERV], new TextBox());
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[RESERV_CAT], new TextBox());
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[TPSANGUE], new TextBox());

                EdtDetalhe.Add(olinha);

                
                olinha = new Linha("Linha 5");

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[END_RUA], new TextBox());

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[END_CID], new TextBox());

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[END_UF], new TextBox());
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[END_CEP], new TextBox());
                EdtDetalhe.Add(olinha);
                
            }
            catch (Exception E)
            { throw; }

        }
        const int BANCO1 = 1;
        const int AGENCIA1 = 2;
        const int CONTA1 = 3;
        const int BCOAGCC = 4;
     
        private void MonteEditeCorrente(BindingSource bmSource)
        {
            try
            {

                EdtCorrente.Clear();

             
                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSource, oCorrente.oDataGridView.Columns[BANCO1], new TextBox());

                olinha.TextoConfigure(bmSource, oCorrente.oDataGridView.Columns[AGENCIA1], new TextBox());

                olinha.TextoConfigure(bmSource, oCorrente.oDataGridView.Columns[CONTA1], new TextBox());
                EdtCorrente.Add(olinha);

                olinha = new Linha("Linha 2");
                olinha.TextoConfigure(bmSource, oCorrente.oDataGridView.Columns[BCOAGCC], new TextBox());

          
                EdtCorrente.Add(olinha);


            }
            catch (Exception E)
            { throw; }

        }

        private void Edita_Cad_Enter(object sender, EventArgs e)
        {

            // quando entra numa rescisão
            //throw new NotImplementedException();
            BindingSource dado = ((sender as Control).DataBindings[0].DataSource as BindingSource);
            DataRow orow = (dado.Current as DataRowView).Row;
            if (orow.IsNull("DEMI"))
            {
                (sender as Control).FindForm().SelectNextControl((sender as Control), true, true, true, false);
                
            }
        }

        private void Edita_Cad_Validated(object sender, EventArgs e)
        {
            Error_Set(sender as Control, "");
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

        private void Edita_Cad_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
            //string selectedtexto = (sender as TextBox).SelectedText.Trim();

            if ((texto.Length != 4) || (texto == ""))
            {
                e.Cancel = true;
                Error_Set(sender as Control);
                return;
            }
            string campo = (sender as MaskedTextBox).DataBindings[0].BindingMemberInfo.BindingField;
            BindingSource dado = ((sender as MaskedTextBox).DataBindings[0].DataSource as BindingSource);
            DataRow orow = (dado.Current as DataRowView).Row;
        
            if (!( (orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Detached) ))
            {
                if (orow[campo, DataRowVersion.Original].ToString() == texto) return;
                (sender as MaskedTextBox).DataBindings[0].ReadValue();
                MessageBox.Show("Não é permitido Alterar Codigo Existente");
                Error_Set(sender as Control, "Codigo já Existe");
                return;
            }

            orow = null;
            try
            {
                orow = evt_Cad.tabCltCad.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() == texto.Trim()).FirstOrDefault();
            }
            catch (Exception)
            {
                orow = null;
            }
            if (orow != null)
            {
                // JÁ ESTÁ MAS AINDA NÃO foi salvo
                if (((orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Detached))) return;

                e.Cancel = true;
                Error_Set(sender as Control, "Codigo já Existe");
            }


        }
        private void NaoPodeSerNULL_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as TextBox).Text;
                     //string selectedtexto = (sender as TextBox).SelectedText.Trim();

            if ((texto.Length == 0) || (texto == ""))
            {
                e.Cancel = true;
                Error_Set(sender as Control, "Não pode Ficar em Branco");
                return;
            }
         

        }

        private void Nascimento_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
            DateTime data = new DateTime();
            try
            {
                data = Convert.ToDateTime(texto);
                if (data.CompareTo(DateTime.Now.AddYears(-100)) < 0 )
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Mais de 100 anos?");
                    return;
                }
                

            }
            catch (Exception)
            {
                e.Cancel = true;
                Error_Set(sender as Control);

            }
        }
        private void Admissao_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
          //  if (texto == "  /  /") return;
            DateTime data = new DateTime();
            try
            {
                data = Convert.ToDateTime(texto);

                if (data.CompareTo(DateTime.Now.AddYears(-100)) < 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Mais de 100 anos?");
                    return;
                }
                    BindingSource dado = ((sender as MaskedTextBox).DataBindings[0].DataSource as BindingSource);
                DataRow orow = (dado.Current as DataRowView).Row;
                DateTime nasc = Convert.ToDateTime(orow["NASC"]);
                if (nasc.CompareTo(data) >= 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Nasc > Admi?");
                    return;
                }
            }
            catch (Exception)
            {
                e.Cancel = true;
                Error_Set(sender as Control);
                return;
            }
        }

        private void AceitaDataVazia_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
            if (texto == "  /  /") return;
            DateTime data = new DateTime();
            try
            {
                data = Convert.ToDateTime(texto);

                if (data.CompareTo(DateTime.Now.AddYears(-100)) < 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Mais de 100 anos?");
                    return;
                }
            }
            catch (Exception)
            {

                e.Cancel = true;
                Error_Set(sender as Control);
                return;
            }
        }

        private void Mensalista_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
            if ((texto.Trim() == "") || texto.Trim() == "X") return;
            e.Cancel = true;
            Error_Set(sender as Control);
            return;
           
        }





    }



}


