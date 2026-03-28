using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Data.Odbc;
using System.Data.OleDb;
using ClassConexao;
using ClassFiltroEdite;

namespace ApoioContabilidade
{
    public partial class FrmFinan2 : Form
    {
           private DateTime inicio, fim;
        private List<LinhaSolucao> oList;// Lista solucao para pesquisa induzida
        private OleDbDataAdapter odbcentrada,odbcsaida;
        DataSet dsentrada, dssaida;

        public PesquisaGenerico oPesquisa;

        private MonteGrid oEntradas, oSaidas;
        private bool recomece;
        private FormFiltro oForm;
        private ArmeEdicao EditeReceber;
        public  BindingSource bmSourceEntrada;
        public BindingSource bmSourceSaida;
        DataView oPlacon,oBancos;
        DataTable NovoBanco;
        
        const int DATA_ger = 0;
        const int VALOR_ger = 1;
      const int CONTA_ger = 2;
      const int CONTAFIN_ger = 3;
      const int TITULAR_ger  = 4;
      const int HIST_ger = 5;
      const int DOC_ger = 6;
      const int VENC_ger = 8;
      const int DOC_FISC_ger = 7;
      const int TAM_BANCOS = 2;
        

        public FrmFinan2()
        {
            InitializeComponent();
            TDataControlContab.AltereTabelaMov_Fin();
            recomece = true;
            oEntradas = new MonteGrid();
            oSaidas = new MonteGrid();
            MonteGrids();
            dgvEntradas.AllowUserToAddRows = false;
            dgvEntradas.AllowUserToDeleteRows = false;
            oEntradas.oDataGridView = dgvEntradas;
            oEntradas.sbTotal = sbEntradas;
            oSaidas.oDataGridView = dgvSaidas;
            oSaidas.sbTotal = sbSaidas;
            oSaidas.sbTotalGeral = sbSaldoGeral;
            //util para edição 
            oPlacon = TDataControlContab.Placon();
            oBancos = TDataControlContab.Bancos();
            NovoBanco = oBancos.ToTable("Bancos");
            NovoBanco.Columns.Add("CODBANCO",typeof(string));
            NovoBanco.Columns.Add("DESCBANCO", typeof(string));
            for (int i = 0; i < NovoBanco.Rows.Count; i++)
            {
                if (Convert.ToInt16(NovoBanco.Rows[i]["NBANCO"]) > 9)
                {
                    NovoBanco.Rows[i]["CODBANCO"] = NovoBanco.Rows[i]["NBANCO"].ToString();
                    NovoBanco.Rows[i]["DESCBANCO"] = NovoBanco.Rows[i]["CODBANCO"].ToString().Trim() + " " + NovoBanco.Rows[i]["NOME_BANCO"].ToString().Trim();
                }
                else
                {
                    if (Convert.ToInt16(NovoBanco.Rows[i]["NBANCO"]) == 0)
                        NovoBanco.Rows[i]["CODBANCO"] = "00";
                    else
                        NovoBanco.Rows[i]["CODBANCO"] = "0" + NovoBanco.Rows[i]["NBANCO"].ToString().Trim();
                    NovoBanco.Rows[i]["DESCBANCO"] = NovoBanco.Rows[i]["CODBANCO"].ToString().Trim() + " " + NovoBanco.Rows[i]["NOME_BANCO"].ToString().Trim();
                }
 
            }
            DataRow orow = NovoBanco.NewRow();
            orow["CODBANCO"] = "00";
            orow["NBANCO"] = 0;
            orow["DESCBANCO"] = "";
            NovoBanco.Rows.InsertAt(orow, 0);

            oForm = new FormFiltro();
            bmSourceEntrada = new BindingSource();
            bmSourceSaida = new BindingSource();
            DataSet dsPesquisa = new DataSet();
            dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
            dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());

            oPesquisa = new PesquisaGenerico(dsPesquisa);

        }

        private void FrmFinan2_Load(object sender, EventArgs e)
        {
            if (recomece)
            {
                ConfiguraInicio();
                recomece = false;
            }
            this.Refresh();

        }

           
   
        private void ConfiguraInicio()
        {
            oForm.oArme = ArmeEdicaoFiltroGenerico();
            oForm.ShowDialog();
            if (oForm.oPesqFin.TabCount > 0)
                oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
            else
                oList = null;
            
            inicio = oForm.dtData1.Value;
            fim = oForm.dtData2.Value;

            //pega os dados do filtro 
            dsentrada = new DataSet();
            dssaida = new DataSet();
            odbcentrada = TDataControlContab.NovoMovFinanceiro(ref dsentrada, inicio, fim, oList, "R", oPesquisa);
            odbcsaida = TDataControlContab.NovoMovFinanceiro(ref dssaida, inicio, fim, oList, "P", oPesquisa);

           // odbcentrada.RowUpdating += new OleDbRowUpdatingEventHandler(odbcentrada_RowUpdating);
           

            DataTable dvEntradas = dsentrada.Tables[0];
            DataTable dvSaidas = dssaida.Tables[0];

            dvEntradas.TableNewRow += new DataTableNewRowEventHandler(dvEntradas_TableNewRow);
            bmSourceEntrada.DataSource = dvEntradas.AsDataView();

            //dvEntradas.AsDataView().AllowNew = true;
            
            bmSourceSaida.DataSource = dvSaidas.AsDataView();
            dgvEntradas.DataSource = bmSourceEntrada;
            dgvSaidas.DataSource = bmSourceSaida;

            oEntradas.ConfigureDBGridView();
            oSaidas.ConfigureDBGridView();

            oEntradas.EncontraTotaisView();
            oEntradas.ColocaTotais();
            oSaidas.EncontraTotaisView();
            oSaidas.ColocaTotais();
            if (EditeReceber == null)
            {
                EditeReceber = new ArmeEdicao(bmSourceEntrada);
                MonteEdits();
                EditeReceber.MonteEdicao();
                EditeReceber.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(EditeReceber_AlteraRegistrosOk);
            }
        
        
        }

    /*    void odbcentrada_RowUpdating(object sender, OleDbRowUpdatingEventArgs e)
        {
            OleDbDataAdapter odbcentrada = (OleDbDataAdapter)sender;
            IDataParameter[] oparams = odbcentrada.GetFillParameters();
            
        }
        */





        void EditeReceber_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                int numreg = -1;
                if (e.TipoMuda == DataRowState.Added)
                {
                    numreg = TDataControlContab.MaiorNRegMovim();
                    if (numreg == -1)
                    {
                        MessageBox.Show("Erro ao buscar numero exclusivo");
                        return;
                    }
                }
                orow.BeginEdit();
                orow["DEBITO"] = orow["CONTAFIN"];
                if (e.TipoMuda == DataRowState.Added)
                    orow["NUMREG"] = numreg + 1;
                orow.EndEdit();
            }
            odbcentrada.Update(new DataRow[] { orow });
            dsentrada.Tables[0].AcceptChanges();
        }

       
       void dvEntradas_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
           e.Row["DATA"] = DateTime.Now.Date;
           e.Row["VENC"] = DateTime.Now.Date;
           e.Row["DATA_EMI"] = DateTime.Now.Date;
           e.Row["TIPO"] = "R";
           e.Row["DOC"] = "";
           e.Row["TP_FIN"] = true;
           e.Row["DEBITO"] = "";
           e.Row["CREDITO"] = "";
           e.Row["DOC_FISC"] = "";
        }
       
        private void MonteGrids()
        {
            oEntradas.Clear();
            oEntradas.AddValores("DATA", "Data", 0, "", false, 0,"");
            oEntradas.AddValores("VALOR", "Valor", 12, "#,###,##0.00", true, 0,"");
            oEntradas.AddValores("CREDITO", "Cliente", 30, "", false, 0,"");
            oEntradas.AddValores("CONTAFIN", "Cta.Fin", 5, "", false, 0, "");
            oEntradas.AddValores("FORN", "Titular", 45, "", false, 0, "");
            oEntradas.AddValores("HIST", "Historico", 40, "", false, 0, "");
            oEntradas.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oEntradas.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oEntradas.AddValores("VENC", "Dta Emissão", 0, "", false, 0, "");

            oSaidas.AddValores("DATA", "Data", 0, "", false, 0, "");
            oSaidas.AddValores("VALOR", "Valor", 12, "#,###,##0.00", true, 0, "");
            oSaidas.AddValores("DEBITO", "Cliente", 30, "", false, 0, "");
            oSaidas.AddValores("CONTAFIN", "Cta.Fin", 5, "", false, 0, "");
            oSaidas.AddValores("FORN", "Titular", 45, "", false, 0, "");
            oSaidas.AddValores("HIST", "Historico", 40, "", false, 0, "");
            oSaidas.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oSaidas.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oSaidas.AddValores("VENC", "Dta Emissão", 0, "", false, 0, "");
        }




        private ArmeEdicao ArmeEdicaoFiltroGenerico()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha = new Linha("SUBS(DEBITO,1,2) / SUBS(CREDITO,1,2)");//("CTAFIN");
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
            olinha.ofuncaoSql = Miscelania.CompareValor;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Transf.Financ.:";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("- Sem Transferencias");
            ((ComboBox)olinha.oedite[0]).Items.Add("= Só as Transferencias");
         //   olinha.ofuncao = TDataControlContab.ExameTransf;
            oArme.Linhas.Add(olinha);
            

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contabilidade";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("S Classificados");
            ((ComboBox)olinha.oedite[0]).Items.Add("N Não Classificados");
            olinha.ofuncao = TDataControlContab.fPassa_Contab2;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Apropriados";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("Só os Apropriados(Gerencial ou Almoxarifado");
            ((ComboBox)olinha.oedite[0]).Items.Add("Não Apropriados");
          //  olinha.ofuncao = TDataControlContab.fPassa_Aprop;
            oArme.Linhas.Add(olinha);

            
          

            olinha = new Linha("DOC");
            olinha.cabecalho[0] = "Documento";
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);


            return oArme;
 
        }

        private void MonteEdits()
        {
            //bmSourceEntrada.AddingNew

            EditeReceber.Clear();
            
            Linha olinha = new Linha("Linha 1");
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[DATA_ger], new DateTimePicker());
            //olinha.oedite[0].Leave += new EventHandler(Data_Leave);
            
            
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[VALOR_ger], new NumericTextBox());
            EditeReceber.Add(olinha);

            olinha = new Linha("Linha 2");
            ComboBox ocombox = new ComboBox();
            ocombox.Tag = (object)"M";// maiusculas
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[CONTA_ger], ocombox);
            
            ocombox = new ComboBox();
            ocombox.Tag = (object)"I";  //só inteiros
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[CONTAFIN_ger], ocombox);
            //placon
            ((ComboBox)olinha.oedite[0]).Items.Clear();
            ((ComboBox)olinha.oedite[0]).Text = "";
            ((ComboBox)olinha.oedite[0]).DataSource = oPlacon;
            ((ComboBox)olinha.oedite[0]).DisplayMember = "DESC2";
            ((ComboBox)olinha.oedite[0]).ValueMember = "DESC2";
            ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
            ((ComboBox)olinha.oedite[1]).Sorted = true;
            ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidating);

            ((ComboBox)olinha.oedite[1]).Items.Clear();
            ((ComboBox)olinha.oedite[1]).DataSource = NovoBanco.AsDataView();
            ((ComboBox)olinha.oedite[1]).DisplayMember = "CODBANCO";
            ((ComboBox)olinha.oedite[1]).ValueMember = "CODBANCO";
            ((ComboBox)olinha.oedite[1]).MaxLength = 2;
            ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
            ((ComboBox)olinha.oedite[1]).Width = 178;
            ((ComboBox)olinha.oedite[1]).Sorted = true;
             ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidating);

            EditeReceber.Add(olinha);

            olinha = new Linha("Linha 3");

            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[TITULAR_ger], new TextBox());
          
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[HIST_ger], new TextBox());
            EditeReceber.Add(olinha);

            olinha = new Linha("Linha 4");

            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[DOC_ger], new TextBox());
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[VENC_ger], new DateTimePicker());
            olinha.TextoConfigure(bmSourceEntrada, dgvEntradas.Columns[DOC_FISC_ger], new TextBox());
            EditeReceber.Add(olinha);
            
        }

        void FrmFinan_ComboValidating(object sender, CancelEventArgs e)
        {
            string texto = ((ComboBox)sender).Text.Trim();
            if (texto == "") return;
            if ((texto.Substring(0,1) == "*")) return;
            string seltexto = "";
            try
            {
                seltexto = (string)((ComboBox)sender).SelectedText;
            }
            catch (Exception)
            {
            }
            if (seltexto.Trim() == texto)     return;
            //e.Cancel = true;
            DataTable oquery;
            try
            {
                oquery = (DataTable)((ComboBox)sender).DataSource;
            }
            catch (Exception)
            {
 
                try 
              	{
                    DataView oquery2 = (DataView)((ComboBox)sender).DataSource;
                    oquery = oquery2.Table;
		
	            }
	            catch (Exception)
	            {
		
	         	    throw;
	            }
            }
            DataRow []orow = oquery.Select(((ComboBox)sender).ValueMember + "='" + texto + "'");
            if (orow.Length == 0) e.Cancel = true;           
        }
       


       
        private void dgvEntradas_Click_1(object sender, EventArgs e)
        {
        //    EditeReceber.Edite(this);

        }

        private void dgvEntradas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Alt) return;
            bool alfanum = false;
            if ((e.KeyCode >= Keys.D0) && (e.KeyCode <= Keys.D9))
                alfanum = true;
            else     // Determine whether the keystroke is a number from the keypad.
                if (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9)
                    alfanum = true;
                else
                    if (((int)e.KeyCode >= 65) && ((int)e.KeyCode <= 90))
                        alfanum = true;
            if (alfanum)
            {
                if ( (((DataGridView)sender).CurrentCell != null) && (((DataGridView)sender).CurrentCell.Selected))
                    EditeReceber.Edite(this,((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
                else

                    EditeReceber.Edite(this);

            }
        }

    }
}
