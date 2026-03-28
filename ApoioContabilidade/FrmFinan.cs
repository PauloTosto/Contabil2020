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
namespace ApoioContabilidade
{
    public partial class FrmFinan : Form
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
        

        public FrmFinan()
        {
            InitializeComponent();
            TDataControlReduzido.AltereTabelaMovim();
            recomece = true;
            this.dgEntradas.Height = ((3+5) * this.dgEntradas.PreferredRowHeight);
            this.dgEntradas.RowHeaderWidth = 15;
            this.dgSaidas.Height = ((8+4) * this.dgSaidas.PreferredRowHeight);
            this.dgSaidas.RowHeaderWidth = 15;
            
            oEntradas = new MonteGrid();
            oSaidas = new MonteGrid();
            MonteGrids();
            dgEntradas.CaptionText = "ENTRADAS";
            dgEntradas.ReadOnly = false;
            dgEntradas.AllowSorting = false;
            dgSaidas.CaptionText = "SAIDAS";
            dgSaidas.ReadOnly = true;
            dgSaidas.AllowSorting = false;
            
            oEntradas.oDataGrid = dgEntradas;
            oEntradas.sbTotal = sbEntradas;
            oSaidas.oDataGrid = dgSaidas;
            oSaidas.sbTotal = sbSaidas;
            oSaidas.sbTotalGeral = sbSaldoGeral;
            //util para edição 
            oPlacon = TDataControlReduzido.Placon();
            oBancos = TDataControlReduzido.Bancos();
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
            dsPesquisa.Tables.Add(TDataControlReduzido.TabelaPlacon().Tables["PLACON"].Copy());
            dsPesquisa.Tables.Add(TDataControlReduzido.BancosContab().Copy());

            oPesquisa = new PesquisaGenerico(dsPesquisa);

        }

       
           
        private void Form3_Load(object sender, EventArgs e)
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
            odbcentrada = TDataControlReduzido.NovoMovFinanceiro(ref dsentrada, inicio, fim, oList, "R", oPesquisa);
            odbcsaida = TDataControlReduzido.NovoMovFinanceiro(ref dssaida, inicio, fim, oList, "P", oPesquisa);

            DataTable dvEntradas = dsentrada.Tables[0];
            /*dvEntradas.Columns["DATA"].DefaultValue = DateTime.Now.Date;
            dvEntradas.Columns["VENC"].DefaultValue = DateTime.Now.Date;
            dvEntradas.Columns["DATA_EMI"].DefaultValue = DateTime.Now.Date;
            dvEntradas.Columns["TIPO"].DefaultValue = "R";
            dvEntradas.Columns["DOC"].DefaultValue = "";
            dvEntradas.Columns["TP_FIN"].DefaultValue = true;*/
            DataTable dvSaidas = dssaida.Tables[0];

            dvEntradas.TableNewRow += new DataTableNewRowEventHandler(dvEntradas_TableNewRow);
            bmSourceEntrada.DataSource = dvEntradas.AsDataView();

            //dvEntradas.AsDataView().AllowNew = true;
            
            bmSourceSaida.DataSource = dvSaidas.AsDataView();
            dgEntradas.DataSource = bmSourceEntrada;
            dgSaidas.DataSource = bmSourceSaida;

            oEntradas.ConfigureDBGrid();
            oSaidas.ConfigureDBGrid();

            oEntradas.EncontraTotais();
            oEntradas.ColocaTotais();
            oSaidas.EncontraTotais();
            oSaidas.ColocaTotais();
            if (EditeReceber == null)
            {
                EditeReceber = new ArmeEdicao(bmSourceEntrada);
                MonteEdits();
                EditeReceber.MonteEdicao();
                EditeReceber.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(EditeReceber_AlteraRegistrosOk);
            }
        
        
        }






        void EditeReceber_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (e.TipoMuda == DataRowState.Added)
            {
                orow.BeginEdit();
                orow["NUMREG"] = TDataControlReduzido.MaiorNRegMovim() + 1;
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
            olinha.ofuncaoSql = TDataControlReduzido.CompareValor;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Transf.Financ.:";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("- Sem Transferencias");
            ((ComboBox)olinha.oedite[0]).Items.Add("= Só as Transferencias");
         //   olinha.ofuncao = TDataControlReduzido.ExameTransf;
            oArme.Linhas.Add(olinha);
            

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Contabilidade";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("S Classificados");
            ((ComboBox)olinha.oedite[0]).Items.Add("N Não Classificados");
            olinha.ofuncao = TDataControlReduzido.fPassa_Contab2;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("DEBITO/CREDITO");
            olinha.cabecalho[0] = "Apropriados";
            olinha.oedite[0] = new ComboBox();

            ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
            ((ComboBox)olinha.oedite[0]).Items.Add("Só os Apropriados(Gerencial ou Almoxarifado");
            ((ComboBox)olinha.oedite[0]).Items.Add("Não Apropriados");
          //  olinha.ofuncao = TDataControlReduzido.fPassa_Aprop;
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

            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[DATA_ger], new DateTimePicker());
            //olinha.oedite[0].Leave += new EventHandler(Data_Leave);
            
            
            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[VALOR_ger], new NumericTextBox());
            EditeReceber.Add(olinha);

            olinha = new Linha("Linha 2");
            ComboBox ocombox = new ComboBox();
            ocombox.Tag = (object)"M";// maiusculas
            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[CONTA_ger], ocombox);
            
            ocombox = new ComboBox();
            ocombox.Tag = (object)"I";  //só inteiros
            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[CONTAFIN_ger], ocombox);
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

            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[TITULAR_ger], new TextBox());
          
            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[HIST_ger], new TextBox());
            EditeReceber.Add(olinha);

            olinha = new Linha("Linha 4");

            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[DOC_ger], new TextBox());
            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[VENC_ger], new DateTimePicker());
            olinha.TextoConfigure(bmSourceEntrada, dgEntradas.TableStyles[0].GridColumnStyles[DOC_FISC_ger], new TextBox());
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
       


        private void dgEntradas_Click(object sender, EventArgs e)
        {
            EditeReceber.Edite(this);
        }

        private void dgEntradas_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "Alt", e.Alt);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Control", e.Control);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "KeyCode", e.KeyCode);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "KeyValue", e.KeyValue);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "KeyData", e.KeyData);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Modifiers", e.Modifiers);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Shift", e.Shift);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "IsInputKey", e.IsInputKey);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "PreviewKeyDown Event");

            
        //    if (e.KeyData == Keys.Tab)
          //      SelectNextControl((Control) sender,true,true,false,true);
        }

        private void dgEntradas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                SelectNextControl((Control)sender, true, true, false, true);
        }

        private void FrmFinan_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Tab)
                SelectNextControl(ActiveControl, true, true, false, true);
        }

        private void FrmFinan_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            System.Text.StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "Alt", e.Alt);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Control", e.Control);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "KeyCode", e.KeyCode);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "KeyValue", e.KeyValue);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "KeyData", e.KeyData);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Modifiers", e.Modifiers);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "Shift", e.Shift);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "IsInputKey", e.IsInputKey);
            messageBoxCS.AppendLine();
            MessageBox.Show(messageBoxCS.ToString(), "PreviewKeyDown Event");

        }

    


    }
}

/*
 * if (Key in [#32..#255]) then
   begin
      TBAltera.Click;
   end;

 */

/*
 * procedure TFrmFinan.DBGReceberEnter(Sender: TObject);
begin

  EditeDados := EditeReceber;
  dsDados := dmFinan.DsFinRc;

 (Sender as TDBGrid).color := clwhite;
 // ColocaSaldo;
  dmFinan.oReceber.ColocaTotal;
  ColocaSaldo;

  inherited;



end;

 */


/*
 * procedure TFrmFinan.MonteEdits;
begin

dbCBCtaPagar.Field.onValidate := ComboBoxValideConta;
dbCBCtaReceber.Field.onValidate  := ComboBoxValideConta;

dbCBBcoPagar.Field.onValidate := ComboBoxValideBco;
dbCBBcoReceber.Field.onValidate  := ComboBoxValideBco;



dbCBBcoReceber.Field.onSetText := tblBancosSetText;
dbCBBcoPagar.Field.onSetText := tblBancosSetText;


with EditePagar do
begin
Clear;
Add('');
Cabecalho[Count-1,0] := DBGPagar.Columns[DATA_ger].Title.caption ;
OEdite[Count-1,0] := CrieDBEDit(DBGPagar.Columns[DATA_ger]);
with OEdite[Count-1,0] as TDBEdit do
 onExit := Mst_DataExit;

Cabecalho[Count-1,1] := DBGPagar.Columns[VALOR_ger].Title.caption;
OEdite[Count-1,1] := CrieDBEDit(DBGPagar.Columns[VALOR_ger]);
(OEdite[Count-1,1] as TDBEdit).onEnter := Mst_ValorEnter;
(OEdite[Count-1,1] as TDBEdit).Field.onSetText := SetEdit_AtualTotal;
(OEdite[Count-1,1] as TDBEdit).Field.onChange := Change_AtualTotal;

Add('');
Cabecalho[Count-1,0] := DBGPagar.Columns[CONTA_ger].Title.caption;
OEdite[Count-1,0]    := DBCBCtaPAgar;

Cabecalho[Count-1,1] := DBGPagar.Columns[CONTAFIN_ger].Title.caption;
OEdite[Count-1,1]    := DBCBBcoPAgar;



Add('');
Cabecalho[Count-1,0] := DBGPagar.Columns[TITULAR_ger].Title.caption;
OEdite[Count-1,0]    := CrieDBEDit(DBGPagar.Columns[Titular_ger]);

Add('');
Cabecalho[Count-1,0] := DbgPagar.Columns[HIST_ger].Title.caption;
OEdite[Count-1,0] := TDBEDitCheckList.Create(self);
with OEdite[Count-1,0] as  TDBEDitCheckList do
begin
visible    := false;
parent := self;
DataSource := DbgPagar.Columns[HIST_ger].Grid.DataSource;
DataField  := DbgPagar.Columns[HIST_ger].Field.FieldName;
width      := DbgPagar.Columns[HIST_ger].Width;
if Field.DataType = ftstring then
  charcase   := ecUpperCase;
CrieCheckList;
end;

Add('');
Cabecalho[Count-1,0] := DBGPagar.Columns[DOC_ger].Title.caption;
OEdite[Count-1,0]    := CrieDBEDit(DBGPagar.Columns[DOC_ger]);

Cabecalho[Count-1,1] := DBGPagar.Columns[VENC_ger].Title.caption;
OEdite[Count-1,1]    := CrieDBEDit(DBGPagar.Columns[VENC_ger]);

Cabecalho[Count-1,1] := DBGPagar.Columns[DOC_FISC_ger].Title.caption;
OEdite[Count-1,1]    := CrieDBEDit(DBGPagar.Columns[DOC_FISC_ger]);
end;

with Editereceber do
begin
Clear;
Add('');
Cabecalho[Count-1,0] := DBGReceber.Columns[DATA_ger].Title.caption ;
OEdite[Count-1,0] := CrieDBEDit(DBGReceber.Columns[DATA_ger]);
with OEdite[Count-1,0] as TDBEdit do
 onExit := Mst_DataExit;

Cabecalho[Count-1,1] := DBGReceber.Columns[VALOR_ger].Title.caption;
OEdite[Count-1,1] := CrieDBEDit(DBGReceber.Columns[VALOR_ger]);
(OEdite[Count-1,1] as TDBEdit).onEnter := Mst_ValorEnter;
(OEdite[Count-1,1] as TDBEdit).Field.onSetText := SetEdit_AtualTotal;
(OEdite[Count-1,1] as TDBEdit).Field.onChange := Change_AtualTotal;

Add('');
Cabecalho[Count-1,0] := DBGReceber.Columns[CONTA_ger].Title.caption;
OEdite[Count-1,0]    := DBCBCtaReceber;

Cabecalho[Count-1,1] := DBGReceber.Columns[CONTAFIN_ger].Title.caption;
OEdite[Count-1,1]    := DBCBBcoReceber;



Add('');
Cabecalho[Count-1,0] := DBGReceber.Columns[TITULAR_ger].Title.caption;
OEdite[Count-1,0]    := CrieDBEDit(DBGReceber.Columns[Titular_ger]);

Add('');
Cabecalho[Count-1,0] := DBGReceber.Columns[HIST_ger].Title.caption;
OEdite[Count-1,0] := TDBEDitCheckList.Create(self);
with OEdite[Count-1,0] as  TDBEDitCheckList do
begin
visible    := false;
parent := self;
DataSource := DBGReceber.Columns[HIST_ger].Grid.DataSource;
DataField  := DBGReceber.Columns[HIST_ger].Field.FieldName;
width      := DBGReceber.Columns[HIST_ger].Width;
if Field.DataType = ftstring then
  charcase   := ecUpperCase;
CrieCheckList;
end;

Add('');
Cabecalho[Count-1,0] := DBGReceber.Columns[DOC_ger].Title.caption;
OEdite[Count-1,0]    := CrieDBEDit(DBGReceber.Columns[DOC_ger]);

Cabecalho[Count-1,1] := DBGReceber.Columns[VENC_ger].Title.caption;
OEdite[Count-1,1]    := CrieDBEDit(DBGReceber.Columns[VENC_ger]);

Cabecalho[Count-1,1] := DBGReceber.Columns[DOC_FISC_ger].Title.caption;
OEdite[Count-1,1]    := CrieDBEDit(DBGReceber.Columns[DOC_FISC_ger]);
end;





EditeReceber.ArmeEdicao;

EditePagar.ArmeEdicao;


end;

 */

