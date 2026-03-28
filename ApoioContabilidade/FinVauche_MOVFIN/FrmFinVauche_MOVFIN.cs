using ApoioContabilidade.Financeiro.Imprimir;
using ApoioContabilidade.FinVauche_MOVFIN.Servicos;
using ApoioContabilidade.Services;
using ApoioContabilidade.ZContabAlterData.Utils;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApoioContabilidade.FinVauche_MOVFIN
{
    public partial class FrmFinVauche_MOVFIN : Form
    {
        EventosDBF_Vauches evt_Fin;
        EdtVauchesDBF comum;
        DataSet dsFiltrado;
        PlanilhaFinanceiraImprime oDataGridViewPrinter;
        //public FrmFinanc_MOVFINConsulta frmFinanc_MOVFINConsulta;

        private FormFiltro oForm;
        public PesquisaGenerico oPesquisa;

        private DateTime inicio, fim;
        private List<LinhaSolucao> oList;// Lista solucao para pesquisa induzida
        DataSet dsPesquisa;

        Boolean primeiraEntrada;


        public FrmFinVauche_MOVFIN()
        {
            InitializeComponent();
            // this.tsConsulta.Click += new System.EventHandler(this.tsConsulta_Click);

            this.Load += new System.EventHandler(this.FrmFinanceiro_Load);
          //  this.toolComboPaginas.SelectedIndexChanged += new System.EventHandler(this.toolComboPaginas_SelectedIndexChanged);
            this.toolBtnEdite.Click += new System.EventHandler(this.toolBtnEditeDetalhe_Click);
            this.gbEntradas.Enter += new System.EventHandler(this.gbEntradas_Enter);
            this.gbEntradas.Leave += new System.EventHandler(this.gbEntradas_Leave);
            this.toolEdita.Click += new System.EventHandler(this.toolEdita_Click);
            this.toolDeleta.Click += new System.EventHandler(this.toolDeleta_Click);
            this.toolImprime.Click += new System.EventHandler(this.toolImprime_Click);
            this.toolSair.Click += new System.EventHandler(this.toolSair_Click);
            this.toolNovo.Click += new System.EventHandler(this.toolNovo_Click);

            primeiraEntrada = true;


            TabelasIniciaisDBF.Execute();
            if (!DadosComumDBF.tabelasJaConfiguradas())
                DadosComumDBF.TabelasConfigCombos();

            oForm = new FormFiltro();
            oForm.dtData1.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            oForm.dtData2.Value = DateTime.Now;


            //  comum.errorProvider1 = this.errorProvider1;
            dgvVauches.KeyDown += dgvPadrao_KeyDown;

            ssVauches.Items.Clear();
            ssVauches.Dock = DockStyle.None;
            ssVauches.SizingGrip = false;
            ssVauches.Width = 300;    //0; //14;
            ssVauches.AutoSize = false;
            ssVauches.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
            ssVauches.Items[ssVauches.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
            ssVauches.Items[ssVauches.Items.Count - 1].Text = "";
            ssVauches.Items[ssVauches.Items.Count - 1].AutoSize = true;
        }

        private void Reload()
        {
            oForm.oArme = ProcComuns.ArmeEdicaoFiltroGenerico();

            oForm.ShowDialog();
            if (oForm.oPesqFin.TabCount > 0)
                oList = oForm.oPesqFin.Pagina("Geral").Get_LinhaSolucao();
            else
                oList = null;
            inicio = oForm.dtData1.Value;
            fim = oForm.dtData2.Value;

            Cursor.Current = Cursors.WaitCursor;
            dsPesquisa = new DataSet();
            dsPesquisa.Tables.Add(TDataControlContab.TabelaPlacon().Tables["PLACON"].Copy());
            dsPesquisa.Tables.Add(TDataControlContab.BancosContab().Copy());
            oPesquisa = new PesquisaGenerico(dsPesquisa);


            FiltroVauchesDBF.PesquisaServidor(inicio, fim, oPesquisa, oList);

            // procedimeno para aglutinar registro


            dsFiltrado = FiltroVauchesDBF.dsFinanceiro.Copy();

            evt_Fin = new EventosDBF_Vauches(dsFiltrado);

            comum = new EdtVauchesDBF(evt_Fin);

            comum.MonteGrids();
            
            dsFiltrado.Tables["MOVFIN"].TableNewRow -= new DataTableNewRowEventHandler(evt_Fin.Vauches_TableNewRow);
            dsFiltrado.Tables["MOVFIN"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.Vauches_TableNewRow);

            evt_Fin.bmVauches.DataSource = dsFiltrado.Tables["MOVFIN"];

            dgvVauches.Columns.Clear();
            
            comum.oVauches.oDataGridView = dgvVauches;
            comum.oVauches.oDataGridView.DataSource = evt_Fin.bmVauches;
            //comum.oReceber.ssTotal = ssReceber;

            evt_Fin.bmVauches.Sort = "DATA";

           
            comum.oVauches.ConfigureDBGridView();

            // FOrçar acrescenta coluna
            /*
            */

            /*    */
            // 
            // Monte as Edições
            MonteEdits();
            //dsFiltrado.Tables["MOVFIN"].TableNewRow -= new DataTableNewRowEventHandler(evt_Fin.Vauches_TableNewRow);
            //dsFiltrado.Tables["MOVFIN"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.Vauches_TableNewRow);



            comum.oVauches.FuncaoSoma();
            // comum.oReceber.ColocaTotais();
            // comum.oPagar.ColocaTotais();
            colocaSaldo();
            Cursor.Current = Cursors.Default;

        }




        private void FrmFinanceiro_Load(object sender, EventArgs e)
        {
            Reload();

        }
        private void MonteEdits()
        {
            comum.UseObsGeral = false;
            bool ok = comum.MonteEdtReceber(evt_Fin.bmVauches);
            if (!ok) { MessageBox.Show("Erro ao montar Edição MOVFIN..Possivelemente Acesso a dados falhou"); return; }
            comum.EdtVauches.AlteraRegistrosOk -= new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Vauches_AlteraRegistrosOk);
            comum.EdtVauches.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Vauches_AlteraRegistrosOk);
            comum.EdtVauches.BeforeAlteraRegistros -= new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Vauches_BeforeAlteraRegistros);
            comum.EdtVauches.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Vauches_BeforeAlteraRegistros);
            comum.EdtVauches.DeletaRegistrosOk -= evt_Fin.Padrao_DeletaRegistrosOk;
            comum.EdtVauches.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;
            comum.EdtVauches.BeforeDeletaRegistros -= evt_Fin.Padrao_BeforeDeletaRegistros;
            comum.EdtVauches.BeforeDeletaRegistros += evt_Fin.Padrao_BeforeDeletaRegistros;
           
           
            //comum.EdtReceber.DeletaRegistrosAdapter += evt_Fin.EditeMovFin_DeletaRegistrosAdapter;



        }

        private void gbEntradas_Enter(object sender, EventArgs e)
        {
            
            pnEntradas.BackColor = SystemColors.ActiveCaption;
            
            bindingNavFin.BindingSource = evt_Fin.bmVauches;

        }
        private void gbEntradas_Leave(object sender, EventArgs e)
        {
            pnEntradas.BackColor = SystemColors.Control;
        }
       
        private void toolComboPaginas_SelectedIndexChanged(object sender, EventArgs e)
        {

           /* var texto = (sender as ToolStripComboBox).Text;
            if (texto.ToUpper() == "ENTRADAS")
            {
                if (toolCredito.Text.ToUpper() != texto.ToUpper())
                {
                    dgvVauches.Focus();
                    toolCredito.Text = "ENTRADAS";
                }
            }*/
            
        }

        private void dgvPadrao_KeyDown(object sender, KeyEventArgs e)
        {

            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;
            string nomeDataGrid = (sender as DataGridView).Name;
            if (nomeDataGrid.ToUpper() == "DGVVAUCHES")
            {
                ArmePadrao = comum.EdtVauches;
                ogrid = comum.oVauches;
            }
            else return;
            if (ArmePadrao == null) return;

            if (((int)e.KeyCode == 113)) // F2
            {
                if ((((DataGridView)sender).CurrentCell != null) && (((DataGridView)sender).CurrentCell.Selected))
                    ArmePadrao.Edite(this, ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
                else
                    ArmePadrao.Edite(this);
                ogrid.FuncaoSoma();
                colocaSaldo();
                dgvVauches.Refresh();
            
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
                    ArmePadrao.Edite(this, ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
                else
                    ArmePadrao.Edite(this);
                ogrid.FuncaoSoma();
                //ogrid.ColocaTotais();
                colocaSaldo();
                dgvVauches.Refresh();

            }
        }

        // Edição do Mestre POR DENTRO DO BINDINGNAVIGATOR MESTRE


        private void toolBtnEditeDetalhe_Click(object sender, EventArgs e)
        {
            /*ArmeEdicao ArmePadrao = null;
            DataGridView dataGridView = null;
            MonteGrid ogrid = null;
            
                ogrid = comum.oVauches;
                ArmePadrao = comum.EdtVauches;
            */
            if (comum.oVauches == null) return;
            if (comum.oVauches.oDataGridView == null) return;
          // dataGridView = ogrid.oDataGridView;

            comum.oVauches.oDataGridView.Focus();
            if ((comum.oVauches.oDataGridView.CurrentCell != null) && (comum.oVauches.oDataGridView.CurrentCell.Selected))
                comum.EdtVauches.Edite(this, comum.oVauches.oDataGridView.CurrentCell.OwningColumn.DataPropertyName);
            else
                comum.EdtVauches.Edite(this);
            comum.oVauches.FuncaoSoma();
            // ogrid.ColocaTotais();
            colocaSaldo();
            dgvVauches.Refresh();
            return;
        }

        private void colocaSaldo()
        {
            System.Nullable<decimal> total1 =
                  (from valor1 in (evt_Fin.bmVauches.DataSource as DataTable).AsEnumerable()
                   where (valor1.Field<string>("DEBITO").Trim() != "")
                  && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("DEBITO")))
                   select valor1.Field<decimal>("VALOR")).Sum();
            // oEntradas.LinhasCampo[1].total = Convert.ToDouble(total1);
            toolDebito.Text = String.Format("Débito: {0:###,###,##0.00}", total1);


            System.Nullable<decimal> total2 =
            (from valor1 in (evt_Fin.bmVauches.DataSource as DataTable).AsEnumerable()
            where (valor1.Field<string>("CREDITO").Trim() != "")
        && (!oPesquisa.ExisteValor("BANCOS", "DESC2", valor1.Field<string>("CREDITO")))
            select valor1.Field<decimal>("VALOR")).Sum();
            toolCredito.Text = String.Format("Crédito: {0:###,###,##0.00}", total2);
            //   oEntradas.LinhasCampo[2].total = Convert.ToDouble(total2);

            //evt_Fin.bmVauches.DataSource



            // Original
            System.Nullable<decimal> sdo = total1 - total2;
            toolSaldo.Text = String.Format("Saldo: {0:###,###,##0.00}", sdo);

        }

        private void toolSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

       
        private void toolNovo_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            DataGridView dataGridView = null;
            MonteGrid ogrid = null;
                ogrid = comum.oVauches;
                ArmePadrao = comum.EdtVauches;
            
            if (ogrid == null) return;
            if (ogrid.oDataGridView == null) return;
            dataGridView = ogrid.oDataGridView;

            dataGridView.Focus();
            //  if ((dataGridView.CurrentCell != null) && (dataGridView.CurrentCell.Selected))
            ArmePadrao.Edite(this, "", true);
            ogrid.FuncaoSoma();
            // ogrid.ColocaTotais();
            colocaSaldo();
            dgvVauches.Refresh();


        }

        private void toolImprime_Click(object sender, EventArgs e)
        {


   /*         bool primeiraimpressao = false;
            if (!primeiraimpressao)
                primeiraimpressao = SetupThePrinting();
            ConfigureRelatorio();
            if (primeiraimpressao)
            {
                //ConfigureRelatorio();
                PrintPreviewDialog MyPrintPreviewDialog = new PrintPreviewDialog();
                MyPrintPreviewDialog.Document = printDocument1;
                MyPrintPreviewDialog.ShowDialog();
                // printDialog1.Document = printDocument1;
                // printDialog1.ShowDialog();
            }
*/        }

        private bool SetupThePrinting()
        {
            printDialog1.AllowCurrentPage = false;
            printDialog1.AllowPrintToFile = false;
            printDialog1.AllowSelection = false;
            printDialog1.AllowSomePages = false;
            printDialog1.PrintToFile = false;
            printDialog1.ShowHelp = false;
            printDialog1.ShowNetwork = false;
            printDialog1.PrinterSettings.DefaultPageSettings.Landscape = true;
            printDialog1.Document = null;
            // if (printDialog1.ShowDialog() != DialogResult.OK)
            //   return false; //21cm * 0.3937008 
            string tituloDoc = "";
            //  if (comum.oPagar.oDataGridView.Focused)
            //    tituloDoc = "SAIDAS";
            // else tituloDoc = "ENTRADAS";

            printDocument1.DocumentName = "Planilha Financeira - " + tituloDoc;

            printDocument1.PrinterSettings =
                        printDialog1.PrinterSettings;
            printDocument1.DefaultPageSettings =
           printDialog1.PrinterSettings.DefaultPageSettings;
            printDocument1.DefaultPageSettings.Margins =
                             new System.Drawing.Printing.Margins(5, 5, 5, 5);
            printDocument1.DefaultPageSettings.Landscape = true;
            return true;


        }
        private bool ConfigureRelatorio()
        {
           /* string tituloDoc = "";
            DataGridView dataGridView = null;
            if (comum.oPagar.oDataGridView.Focused)
            {
                dataGridView = comum.oPagar.oDataGridView;
                tituloDoc = "SAIDAS";
            }
            else
            {
                tituloDoc = "ENTRADAS";
                dataGridView = comum.oReceber.oDataGridView;

            }
            if (dataGridView == null)
            {
                MessageBox.Show("Focalize um dos Grids");
                return false;
            }

            oDataGridViewPrinter = new PlanilhaFinanceiraImprime(dataGridView, printDocument1, false, true,
                "Planilha Financeira - " + tituloDoc, new Font("Microsoft Sans Serif", 9, FontStyle.Regular), Color.Black, true, "M.Libanio Agricola S.A.",
                "Emitido em:" + String.Format("{0: dd/MM/yyyy    hh:mm:ss}", DateTime.Now));
            //  calculo tamanho =  (x cm * 0.3937008)*100 {0:dd/MM/yyyy hh:mm tt}
            oDataGridViewPrinter.RowsHeight.Clear();

            oDataGridViewPrinter.RowsHeight.Add(19.685F);

            oDataGridViewPrinter.LinhaCab1.Add("Data");
            oDataGridViewPrinter.LinhaCab1.Add("Valor");
            oDataGridViewPrinter.LinhaCab1.Add("Fornecedor");
            oDataGridViewPrinter.LinhaCab1.Add("Bco");
            oDataGridViewPrinter.LinhaCab1.Add("Titular");
            oDataGridViewPrinter.LinhaCab1.Add("Historico");
            oDataGridViewPrinter.LinhaCab1.Add("Doc.Financ");
            oDataGridViewPrinter.LinhaCab1.Add("N.Fiscal");
            oDataGridViewPrinter.LinhaCab1.Add("Emissao");
            oDataGridViewPrinter.LinhaCab1.Add("Forma Pg");//
            for (int z = 0; z < comum.oPagar.LinhasCampo.Count - 1; z++)
                if (comum.oPagar.LinhasCampo[z].total != 0)
                    oDataGridViewPrinter.LinhaTotal.Add(z, Convert.ToSingle(comum.oPagar.LinhasCampo[z].total));

            printDocument1.PrintPage -= printDocument1_PrintPage_1;
            printDocument1.PrintPage += printDocument1_PrintPage_1;

            */
            return true;

        }

        private void printDocument1_PrintPage_1(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            // The PrintPage action for the PrintDocument control

            bool more = oDataGridViewPrinter.DrawDataGridView(e.Graphics);
            if (more == true)
                e.HasMorePages = true;

        }

        private void toolDeleta_Click(object sender, EventArgs e)
        {

            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


                ArmePadrao = comum.EdtVauches;
                ogrid = comum.oVauches;

            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                return;
            }
            //
            // Verifica se a deleatar um mestre, se existem registros ligados..
            BindingSource bmSource = (ogrid.oDataGridView.DataSource as BindingSource);
            DataRow orow = (bmSource.Current as DataRowView).Row;
            if (ArmePadrao.OnBeforeDeleta(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
            {

                if (MessageBox.Show("Confirma Exclusao?", "Confirma", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    try
                    {
                        if (ArmePadrao.OnDeletaLinhas(new AlteraRegistroEventArgs(new DataRow[] { orow }, orow.RowState)))
                        {
                            orow.Delete();
                            orow.Table.AcceptChanges();
                            ArmePadrao.OnAfterDeleta(new AlteraRegistroEventArgs(new DataRow[] { null }, DataRowState.Deleted));

                            ogrid.FuncaoSoma();
                            colocaSaldo();


                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            dgvVauches.Refresh();

        }

        private void toolEdita_Click(object sender, EventArgs e)
        {


            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


                ArmePadrao = comum.EdtVauches;
                ogrid = comum.oVauches;
            
            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                return;
            }
            DataGridView dataGridView = ogrid.oDataGridView;
            if ((dataGridView.CurrentCell != null) && (dataGridView.CurrentCell.Selected))
                ArmePadrao.Edite(this, dataGridView.CurrentCell.OwningColumn.DataPropertyName);
            else
                ArmePadrao.Edite(this);

            ogrid.FuncaoSoma();
            // ogrid.ColocaTotais();
            colocaSaldo();
            dgvVauches.Refresh();

        }

        #region COMPORTAMENTO BOTÂO Categoria (transf e Apro)


        #endregion



        #region EXTRATO


        #endregion

        private void toolImprime_Click_1(object sender, EventArgs e)
        {
            comum.oVauches.tituloExcel = "Financeiro & Vauches de " + inicio.ToString("d") + " a " + fim.ToString("d");
            comum.oVauches.ExportaExcel();
        }

        private void tsConsulta_Click_1(object sender, EventArgs e)
        {
            Reload();
        }

        

    }






}

