using ApoioContabilidade.FerramentasEditar;
using ApoioContabilidade.Models;
using ApoioContabilidade.ZContabAlterData.Utils;
using ClassConexao;
using ClassFiltroEdite;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ApoioContabilidade
{
    public partial class FrmEdicaoPTMovFin : Form
    {
        public DataTable otable;
        private MonteGrid oMovFin;
        
        List<int> Anos; 
        public BindingSource bmSourceMovFin;
        private FormFiltro oForm;
        List<string> tabelasDBF;

        private List<LinhaSolucao> oList;
        private DateTime inicio, fim;
        DataTable oPlacon;
       
        DataSet dsPesquisa;
        DataSet dsMovFin;
        DataTable movFinView;
        public PesquisaGenerico oPesquisa;

        private string tabelaAtual;
        private ArmeEdicao EditeMovFin;

        // rotinas de inclusao, exclusao e alteraçao
        OleDbDataAdapter adapterPtMovFin;

        Dictionary<string, string> dictNumConta_Desc2;
        Dictionary<string, string> dictDesc2_NumConta;
        int AnoGeral = 0;
        // o USUÁRIO PODERÁ ESCOLHER SE TRABALHA COM O MNMONICO(DESC2) OU COM NUMCONTA 
        string campo_pesq = "DESC2";  /// default 

        List<string> camposValidos = new List<string>();
       // List<string> camposValidosDesc2 = new List<string>();

        public FrmEdicaoPTMovFin()
        {
            InitializeComponent();
            Anos = new List<int>();
            oMovFin = new MonteGrid();
            MonteGrids();
            dgvMovFin.ReadOnly = true;
            oMovFin.oDataGridView = dgvMovFin;
            // encontrar Tabelas com o PADRAO PTMOVFIN<ano>.dbf no diretorio padrao(CONTAB)

            // para a pesquisa entre NUMCONTA e DESC2(mnemonico) ficar mais rápida foi criado um dictionary
            // preenchido aqui

            dictNumConta_Desc2 = new Dictionary<string, string>();
            dictDesc2_NumConta = new Dictionary<string, string>();
            oPlacon = TDataControlContab.PlaconAlterado();
            foreach(DataRow placon in oPlacon.Rows)
            {
                dictNumConta_Desc2.Add(placon["NUMCONTA"].ToString().Trim(), placon["DESC2"].ToString().Trim());
                dictDesc2_NumConta.Add(placon["DESC2"].ToString().Trim(), placon["NUMCONTA"].ToString().Trim());
            }
            // necessário para pesquisa generica -> reconstruir (melhorar)
            dsPesquisa = new DataSet();
            dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
            dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());
            oPesquisa = new PesquisaGenerico(dsPesquisa);
            InicializaAno();
        }
        private void InicializaAno()
        {
            tabelasDBF = TDataControlContab.EncontreTabela(false);
           
            foreach (string tabela in tabelasDBF)
            {
                int pos = tabela.IndexOf("PTMOVFIN");
                if (tabela.Contains("PTMOVFIN") && (pos == 0))
                {
                    try
                    {
                        string anostr = tabela.Substring(8, 4);
                        int ano = Convert.ToInt32(anostr);
                        if (tabela.Length != 16) continue; // padrao 
                        Anos.Add(ano);
                    }
                    catch (Exception) { }

                }
            }
            if (Anos.Count > 0)
            {
                foreach (int ano in Anos)
                {
                    comboBox1.Items.Add(ano.ToString());
                }
                comboBox1.SelectedIndex = -1;
            }
            else
            {
                MessageBox.Show("É necessario PTMOVFIN<ano>.DBF");
                return;
            }
            oForm = new FormFiltro();
            oForm.dtData1.Value = new DateTime(DateTime.Now.Year, 1, 1);
            oForm.dtData2.Value = new DateTime(DateTime.Now.Year, 1, 31);
            
            bmSourceMovFin = new BindingSource();
        }
        private void Reload()
        {
            int ano = 0; 
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Selecione tabela");
                return;
            }
               ano = Convert.ToInt32(comboBox1.SelectedItem);
                
               
            //if ((ano != 0) && (ano != AnoGeral))
            if (ano != AnoGeral)
            {
                AnoGeral = ano;

                if (comboBox1.Enabled)
                {
                    oForm.dtData1.Value = new DateTime(ano, 1, 1);
                    oForm.dtData2.Value = new DateTime(ano, 1, 31);
                    oForm.dtData1.MinDate = new DateTime(ano, 1, 1);
                    oForm.dtData1.MaxDate = new DateTime(ano, 12, 31);
                    oForm.dtData2.MinDate = new DateTime(ano, 1, 1);
                    oForm.dtData2.MaxDate = new DateTime(ano, 12, 31);
                }
            }
            
            oForm.oArme = ProcComuns.ArmeEdicaoFiltroEdita(); 
            oForm.ShowDialog();
            if (oForm.oPesqFin.TabCount > 0)
                oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
            else
                oList = null;
            inicio = oForm.dtData1.Value;
            fim = oForm.dtData2.Value;
            comboBox1.Enabled = false;

            tabelaAtual = "PTMOVFIN" + ano.ToString(); 
            try
            {
                dsMovFin = TDataControlContab.MovFinanVauches_Relaciona(tabelaAtual, inicio, fim, oList, oPesquisa);
            }
            catch (Exception E)
            {
                MessageBox.Show("Não Foi Encontrado o " + tabelaAtual + " " + E.Message);
                return;
            }
            try
            {
                adapterPtMovFin = TDataControlContab.ConstruaAdaptador_PTMovFin(tabelaAtual);
            }
            catch (Exception E)
            {

                MessageBox.Show("Falha no Acesso Edição  " + tabelaAtual + " " + E.Message);
                return;
            }
          

           // MODIFICA TABELA PARA EDIçAO (acrescenta campos para intercambiar entre NUMCONTA e MNEMONICO 
            DataTableCollection oTables = dsMovFin.Tables;
            DataView dvEntradas = oTables[0].AsDataView();
            movFinView = oTables[0].Clone();
            movFinView.Columns.Add("DEBITOVIEW", Type.GetType("System.String"));
            movFinView.Columns.Add("CREDITOVIEW", Type.GetType("System.String"));
            movFinView.Columns.Add("TIPOEDICAO", Type.GetType("System.Int32"));

            foreach (DataColumn col in movFinView.Columns)
            {
                if (col.DataType == Type.GetType("System.String"))
                    col.DefaultValue = "";
                else if (col.DataType == Type.GetType("System.DateTime"))
                    col.DefaultValue = DateTime.MinValue;
                else if (col.DataType == Type.GetType("System.Bolean"))
                    col.DefaultValue = 0;
                else col.DefaultValue = 0;
            }
            // É Possivel colocar em cada COLumn um valor default específico para quando for criado uma nova linha
            movFinView.TableNewRow += new DataTableNewRowEventHandler(movFinView_TableNewRow);
            
            bmSourceMovFin.DataSource = movFinView;//   dvEntradas;
            
            dgvMovFin.DataSource = bmSourceMovFin;

            oMovFin.sbTotal = sbMovFin;
            oMovFin.ConfigureDBGridView();
            PesquiseMovFin();
            Totaliza();

            comboBox1.Enabled = true;
        }

        private void Totaliza()
        {
            System.Nullable<decimal> total1 =
              (from valor1 in movFinView.AsEnumerable()
               where (valor1.Field<string>("DEBITOVIEW").Trim() != "")
               // && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("DEBITO")))
               select valor1.Field<decimal>("VALOR")).Sum();
            oMovFin.LinhasCampo[1].total = Convert.ToDouble(total1);

            System.Nullable<decimal> total2 =
      (from valor1 in movFinView.AsEnumerable()
       where (valor1.Field<string>("CREDITOVIEW").Trim() != "")
       // && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("CREDITO")))
       select valor1.Field<decimal>("VALOR")).Sum();
            oMovFin.LinhasCampo[2].total = Convert.ToDouble(total2);

            oMovFin.ColocaTotais();
        }

        private void MonteGrids()
        {
            oMovFin.Clear();
            oMovFin.AddValores("DATA", "DATA", 0, "", false, 0, "");
            oMovFin.AddValores("VALOR", "VALOR", 12, "#,###,##0.00", true, 0, "");
            oMovFin.AddValores("DEBITOVIEW", "DEBITO", 25,"", true, 0, "");
            oMovFin.AddValores("CREDITOVIEW", "CREDITO", 25, "", true, 0, "");
            
            oMovFin.AddValores("HIST", "HISTORICO", 40, "", false, 0, "");
            oMovFin.AddValores("DOC", "Doc.Finan.", 10, "", false, 0, "");
            oMovFin.AddValores("DOC_FISC", "N.Fiscal", 10, "", false, 0, "");
            oMovFin.AddValores("FORN", "Fornecedor", 30, "", false, 0, "");
          
        }

        private void btnPesquisa_Click(object sender, EventArgs e)
        {
            Reload();
        }


        private void PesquiseMovFin()
        {

            dgvMovFin.Enabled = false;
            DataTable tabordenada = dsMovFin.Tables[0];
            groupBox1.Enabled = false;
            camposValidos.Clear();
            campo_pesq = "NUMCONTA";
            if (rbDesc2.Checked)
                campo_pesq = "DESC2";
            try
            {
                movFinView.Rows.Clear();
                //string codant = "";
                for (int i = 0; i < tabordenada.Rows.Count; i++)//dsrelaciona.Tables["RELACIONA"].Rows.Count; i++)
                {

                    DataRow linexc = tabordenada.Rows[i];// dsrelaciona.Tables["RELACIONA"].Rows[i];


                    string data = TDataControlReduzido.FormatDataGravarExtenso(Convert.ToDateTime(linexc["DATA"]));

                    string debito = Convert.ToString(linexc["DEBITO"]).Trim();
                    string credito = Convert.ToString(linexc["CREDITO"]).Trim();
                    string debitoview = Convert.ToString(linexc["DEBITO"]).Trim();
                    string creditoview = Convert.ToString(linexc["CREDITO"]).Trim();

                    if (rbDesc2.Checked)
                    {

                        debitoview = NumConta_Desc2(debito);
                        creditoview = NumConta_Desc2(credito);

                    }
                    DataRow razaoRow = movFinView.NewRow();
                    razaoRow.ItemArray = linexc.ItemArray;
                    razaoRow["DEBITO"] = debito;
                    razaoRow["CREDITO"] = credito;
                    razaoRow["DEBITOVIEW"] = debitoview;
                    razaoRow["CREDITOVIEW"] = creditoview;
                    razaoRow["TIPOEDICAO"] = 2;
                    if (rbDesc2.Checked)
                    {
                        razaoRow["TIPOEDICAO"] = 1;
                    }

                    movFinView.Rows.Add(razaoRow);
                }
                movFinView.AcceptChanges();

                EditeMovFin = new ArmeEdicao(bmSourceMovFin);
                MonteEdits();
                EditeMovFin.MonteEdicao();
                EditeMovFin.AlteraRegistrosOk -= new EventHandler<AlteraRegistroEventArgs>(EditeMovFin_AlteraRegistrosOk);
                EditeMovFin.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(EditeMovFin_AlteraRegistrosOk);
                EditeMovFin.BeforeAlteraRegistros -= new EventHandler<AlteraRegistroEventArgs>(EditeMovFin_BeforeAlteraRegistros);
                EditeMovFin.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(EditeMovFin_BeforeAlteraRegistros);
                //EditeMovFin.DeletaRegistrosOk += EditeMovFin_DeletaRegistrosOk;
                EditeMovFin.DeletaRegistrosAdapter -= EditeMovFin_DeletaRegistrosAdapter;
                EditeMovFin.DeletaRegistrosAdapter += EditeMovFin_DeletaRegistrosAdapter;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                dgvMovFin.Enabled = true;
                groupBox1.Enabled = true;
                camposValidos = PreenchaCamposValidos();
            }
        }

        

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            btnPesquisa.Enabled = true;
        }

       
        private void rbNumconta_CheckedChanged(object sender, EventArgs e)
        {
            PesquiseMovFin();
        }

        private void rbDesc2_CheckedChanged(object sender, EventArgs e)
        {
            PesquiseMovFin();
        }
        const int DATA_edite = 0;
        const int VALOR_edite = 1;
        const int DEBITOVIEW_edite = 2;
        const int CREDITOVIEW_edite = 3;
        
        const int HIST_edite = 4;
        const int DOC_edite = 5;
        const int DOCFISCAL_edite = 6;
        const int FORN_edite = 7;

        private void MonteEdits()
        {
            try
            {
                string displayMember = campo_pesq.Trim() + "_DESCRICAO";
                if (campo_pesq.Trim() != "NUMCONTA")
                    displayMember = campo_pesq.Trim();

                EditeMovFin.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[DATA_edite], new DateTimePicker());
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[VALOR_edite], new NumericTextBox());
                olinha.oedite[1].Validating += new CancelEventHandler(FrmFinan_Valor);
                olinha.oedite[1].Validated += new EventHandler(FrmFinan_CampoValidado);
                EditeMovFin.Add(olinha);

                olinha = new Linha("Linha 2");
                ComboBox ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas
                

                ocombox.DataSource = oPlacon.AsEnumerable().CopyToDataTable().DefaultView;  //oPlacon.AsEnumerable().OrderBy(row=> row.Field<string>(pesquisa_conta)).CopyToDataTable();
                ocombox.DisplayMember = displayMember;
                ocombox.ValueMember = campo_pesq;
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[DEBITOVIEW_edite], ocombox);
                olinha.oedite[0].Width = 300;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                if (campo_pesq.Trim() != "NUMCONTA")
                    ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingDesc2);
                else
                    ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNumconta);
                ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidado);
                

                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";  //só inteiros(I)
                

                ocombox.DataSource = oPlacon.AsEnumerable().CopyToDataTable();  //oPlacon.AsEnumerable().OrderBy(row=> row.Field<string>(pesquisa_conta)).CopyToDataTable();
                ocombox.DisplayMember = displayMember;
                ocombox.ValueMember = campo_pesq;
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[CREDITOVIEW_edite], ocombox);

                olinha.oedite[1].Width = 300;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                if (campo_pesq.Trim() != "NUMCONTA")
                    ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingDesc2);
                else
                    ((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNumconta);
                ((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidado);
                EditeMovFin.Add(olinha);

                olinha = new Linha("Linha 3");

                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[HIST_edite], new TextBox());
                EditeMovFin.Add(olinha);

                olinha = new Linha("Linha 4");
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[DOC_edite], new TextBox());
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[DOCFISCAL_edite], new TextBox());
                olinha.TextoConfigure(bmSourceMovFin, oMovFin.oDataGridView.Columns[FORN_edite], new TextBox());
                EditeMovFin.Add(olinha);
            }
            catch (Exception)
            {

                throw;
            }
        }

        void FrmFinan_ComboValidatingNumconta(object sender, CancelEventArgs e)
        {
            string texto = ((ComboBox)sender).Text.Trim();
            if (texto == "") return;

                // estou aceitando campo em branco
                /*if (texto == "")
                {
                    e.Cancel = true; // NÃO ACEITA CAMPO ZERADO
                                     // if (ultimoSelectEnter != -1)
                                     // {
                    try
                    {
                        ((ComboBox)sender).DataBindings[0].ReadValue();
                    }
                    catch (Exception)
                    {
                    }
                    this.errorProvider1.SetError(((ComboBox)sender), "Campo tem que ser preenchido");
                    return;
                }*/
                if (texto.Length > 8)
            { texto = texto.Substring(0, 8);}
            string selvalue = "";
            try  { selvalue = (string)((ComboBox)sender).SelectedValue; }catch (Exception)  {   }
            if (selvalue != null) 
            {   if  (selvalue.Trim() == texto)
                    {
                    ((ComboBox)sender).Text = texto;
                    return; }
            }
            e.Cancel = true;
            DataTable oquery;
            DataRow retornado = null;
            try
            {
                oquery = ((ComboBox)sender).DataSource as DataTable;
                retornado = oquery.AsEnumerable().Where(row => row.Field<string>("NUMCONTA").Trim() == texto.Trim()).FirstOrDefault();
            }
            catch (Exception)
            {
            }

            if (retornado != null) { e.Cancel = false; }
           // MessageBox.Show("Campo Invalido");
           
            this.errorProvider1.SetError(((ComboBox)sender), "Campo Invalido! Redigite");
            try
            {
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }
            catch (Exception)
            {
            }
        }

        void FrmFinan_ComboValidatingDesc2(object sender, CancelEventArgs e)
        {

            string texto = ((ComboBox)sender).Text.Trim();
            if (texto == "") return;
            // estou aceitando campo em branco
            /*if (texto == "")
            {
                e.Cancel = true; // NÃO ACEITA CAMPO ZERADO
                try
                {
                    ((ComboBox)sender).DataBindings[0].ReadValue();
                }
                catch (Exception) { }
                return;
            }*/
            string selvalue = "";
            try { selvalue = (string)((ComboBox)sender).SelectedValue; } catch (Exception) { }
            // valor foi selecionado no Combobox
            if (selvalue != null)
            {
                if (selvalue.Trim() == texto.Trim())
                {
                    ((ComboBox)sender).Text = texto.Trim();
                    return;
                }
            }
            e.Cancel = true;
             
            DataRow retornado = null;
            try
            {
                //string textocompara = texto.Trim().PadRight(25,' '); 
                DataView oquery = (((ComboBox)sender).DataSource as System.Data.DataView); 
                if (oquery != null)
                   retornado = oquery.Table.AsEnumerable().Where(row => row.Field<string>("DESC2").Trim() == texto.Trim()).FirstOrDefault();
            }
            catch (Exception)
            {
            }
            if (retornado != null) { e.Cancel = false; return; }
            try
            {
                ((ComboBox)sender).DataBindings[0].ReadValue();
            }
            catch (Exception)
            {
            }
            this.errorProvider1.SetError(((ComboBox)sender), "Campo Invalido! Redigite");
        }

        void FrmFinan_CampoValidado(object sender, EventArgs e)
        {
            // string textoOrigem = ((ComboBox)sender).
            this.errorProvider1.SetError((Control)sender, "");
        }

        void FrmFinan_Valor(object sender, CancelEventArgs e)
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

                this.errorProvider1.SetError(((Control)sender), "Valor Não pode ser <= 0");
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


        /* private void dgvEntradas_Click_1(object sender, EventArgs e)
         {
             //    EditeReceber.Edite(this);

         }*/

        private void dgvMovFin_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if ((((DataGridView)sender).CurrentCell != null) && (((DataGridView)sender).CurrentCell.Selected))
                EditeMovFin.Edite(this, ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
            else
                EditeMovFin.Edite(this);
        }


        private void dgvMovFin_KeyDown(object sender, KeyEventArgs e)
        {
            if ( ((int)e.KeyCode == 70)) // F9
            {
                if ((((DataGridView)sender).CurrentCell != null) && (((DataGridView)sender).CurrentCell.Selected))
                    EditeMovFin.Edite(this, ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
                else
                    EditeMovFin.Edite(this);
                return;
            }
            if (((int)e.KeyCode == 113)) // F2
            {
                
                int position = bmSourceMovFin.Position;
                int colIndex = ((DataGridView)sender).CurrentCell.ColumnIndex;
                string valorOriginal = ((DataGridView)sender).CurrentCell.Value.ToString();
                string NomeDoCampo = ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName;

                EditarColunaMulti editaMulti = new EditarColunaMulti(valorOriginal,NomeDoCampo);
                if ((NomeDoCampo == "DEBITOVIEW")
                    || (NomeDoCampo == "CREDITOVIEW"))
                {
                    editaMulti.lstCamposValidos = camposValidos;
                }
                DataTable origem = bmSourceMovFin.DataSource as DataTable;

                DataView filtrada = (bmSourceMovFin.DataSource as DataTable).AsEnumerable().Where(row => row.Field<string>(NomeDoCampo) == valorOriginal).AsDataView();
                
                BindingSource bmFiltro = new BindingSource();
                bmFiltro.DataSource = filtrada;
                ((DataGridView)sender).DataSource = bmFiltro;

                string novovalor = "";
                if (editaMulti.Execute())
                {
                    novovalor = editaMulti.SelItem();
                }


                if (novovalor.Trim() == "")
                {
                    ((DataGridView)sender).DataSource = bmSourceMovFin;
                    bmSourceMovFin.Position = position;
                    return;
                }
                foreach(DataRowView orow in filtrada)
                {
                    orow.Row.BeginEdit();
                    orow.Row[NomeDoCampo] = novovalor;
                    int inicioview = NomeDoCampo.IndexOf("VIEW");
                    if ((Convert.ToInt32(orow["TIPOEDICAO"]) == 1) && (inicioview >-1)) 
                    {
                        
                        string debcre = NomeDoCampo.Substring(0, inicioview);
                        orow[debcre] = Desc2_Numconta(orow[NomeDoCampo].ToString().Trim()); ;
                    }                    

                    orow.Row.EndEdit();
                }
                adapterPtMovFin.Update(filtrada.Table);
                movFinView.AcceptChanges();
                ((DataGridView)sender).DataSource = bmSourceMovFin;
                bmSourceMovFin.Position = position;
                ((DataGridView)sender).CurrentCell = ((DataGridView)sender).CurrentRow.Cells[colIndex];


                return;
                    
               
            }

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
                if ((((DataGridView)sender).CurrentCell != null) && (((DataGridView)sender).CurrentCell.Selected))
                    EditeMovFin.Edite(this, ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
                else
                    EditeMovFin.Edite(this);
            }
        }
        // Rotina de alterar/Incluir os registros 
        void EditeMovFin_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((orow["DEBITOVIEW"].ToString().Trim() == "") || (orow["CREDITOVIEW"].ToString().Trim() == "")
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITOVIEW"].ToString().Trim() == orow["CREDITOVIEW"].ToString().Trim())
                    || (Convert.ToDateTime(orow["DATA"]).Year != AnoGeral))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return; 
            }

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {
                if ((orow["DEBITOVIEW"].ToString().Trim() == "") || (orow["CREDITOVIEW"].ToString().Trim() == "")
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITOVIEW"].ToString().Trim() == orow["CREDITOVIEW"].ToString().Trim())
                    || (Convert.ToDateTime(orow["DATA"]).Year != AnoGeral))
                {
                    MessageBox.Show("Dados Inconsistentes com padrão");
                }
                int mov_id = -1;
                if (e.TipoMuda == DataRowState.Added)
                {
                    mov_id = TDataControlContab.MaiorMov_ID_Movim(tabelaAtual);
                    if (mov_id == -1)
                    {
                        MessageBox.Show("Erro ao buscar numero exclusivo");
                        return;
                    }
                }

                orow.BeginEdit();
                if (Convert.ToInt32(orow["TIPOEDICAO"]) == 1)
                {
                    orow["DEBITO"] = Desc2_Numconta(orow["DEBITOVIEW"].ToString().Trim()); ;
                    orow["CREDITO"] = Desc2_Numconta(orow["CREDITOVIEW"].ToString().Trim());
                }
                if (e.TipoMuda == DataRowState.Added)
                    orow["MOV_ID"] = mov_id+1;
                orow.EndEdit();
            }
            try
            {
                // para evitar valores nullos use defaults 
                foreach(DataColumn col in orow.Table.Columns)
                {
                   if (orow[col.ColumnName] is System.DBNull)
                    {
                        orow[col.ColumnName] = col.DefaultValue;
                    }

                }
                adapterPtMovFin.Update(new DataRow[] { orow });
                movFinView.AcceptChanges();
                Totaliza();

            }
            catch (Exception E)
            {
                
                MessageBox.Show("Operação Falhou: "+ E.ToString());
                throw;

            }
            
        }

        void EditeMovFin_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((orow["DEBITOVIEW"].ToString().Trim() == "") || (orow["CREDITOVIEW"].ToString().Trim() == "")
                    || (Convert.ToDecimal(orow["VALOR"]) == 0)
                    || (orow["DEBITOVIEW"].ToString().Trim() == orow["CREDITOVIEW"].ToString().Trim())
                    || (Convert.ToDateTime(orow["DATA"]).Year != AnoGeral))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
        }



        //dvEntradas.TableNewRow += new DataTableNewRowEventHandler(dvEntradas_TableNewRow);
        void movFinView_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DateTime datainicial = new DateTime(AnoGeral != 0 ? AnoGeral : DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day);
            DataRow orow = null;
            try
            {
                if (bmSourceMovFin.Count > 0)
                {
                    orow = ((DataRowView)bmSourceMovFin.Current).Row;
                }
            }
            catch (Exception)
            {
            }
            if (orow != null) { datainicial = Convert.ToDateTime(orow["data"]); }

            e.Row["DATA"] = datainicial;
            e.Row["VENC"] = datainicial;
            e.Row["DATA_EMI"] = datainicial;
            e.Row["TIPO"] = "";
            e.Row["DOC"] = "";
            e.Row["TP_FIN"] = 0;
            e.Row["DEBITO"] = "";
            e.Row["CREDITO"] = "";
            e.Row["DOC_FISC"] = "";
            e.Row["DEBITOVIEW"] = "";
            e.Row["CREDITOVIEW"] = "";
            if (campo_pesq == "DESC2") {
                e.Row["TIPOEDICAO"] = 1; // mnemonico
            }
            else {
                e.Row["TIPOEDICAO"] = 2; // numconta
            }

        }

        private void EditeMovFin_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            try
            {
                //adapterPtMovFin.Update(new DataRow[] { orow }); ;
                //movFinView.AcceptChanges();
            }
            catch (Exception E)
            {
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;
            }
            // throw new NotImplementedException();
        }
        private void EditeMovFin_DeletaRegistrosAdapter(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            try
            {
                adapterPtMovFin.Update(new DataRow[] { orow }); ;
                movFinView.AcceptChanges();
                Totaliza();
            }
            catch (Exception E)
            {
                MessageBox.Show("Operação Falhou: " + E.ToString());
                throw;
            }

        }




        public List<string> PreenchaCamposValidos()
        {
            List<string> result;
            if (campo_pesq.Trim() == "DESC2")
            {
                result = dictDesc2_NumConta.AsEnumerable().Select(dic => dic.Key).ToList();
            }
            else
            {
                result = dictNumConta_Desc2.AsEnumerable().Select(dic => dic.Key).ToList();
            }
            return result;
        }

        public string NumConta_Desc2(string conta)
        {
            string result = "";
            if (conta.Trim() == "") return result;
            try
            {
                if (dictNumConta_Desc2.ContainsKey(conta.Trim()))
                {
                    result = dictNumConta_Desc2[conta.Trim()].Trim();
                }
            }
            catch (Exception)
            {
            }
            return result;
        }

       
        
        public string Desc2_Numconta(string conta)
        {
            string result = "";
            if (conta.Trim() == "") return result;
            try
            {
                if (dictDesc2_NumConta.ContainsKey(conta.Trim()))
                {
                    result = dictDesc2_NumConta[conta.Trim()].Trim();
                }
            }
            catch (Exception)
            {
            }
            return result;

        }
    }
}
