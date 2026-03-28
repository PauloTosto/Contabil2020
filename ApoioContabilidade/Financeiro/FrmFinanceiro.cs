using ApoioContabilidade.Financeiro.Imprimir;
using ApoioContabilidade.PagarReceber;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
using System;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ApoioContabilidade.Financeiro
{
    public partial class FrmFinanceiro : Form
    {
        Evt_Fin evt_Fin;
        EdtFin comum;
        DataSet dsFiltrado;
        PlanilhaFinanceiraImprime oDataGridViewPrinter;
        public FrmFinancConsulta frmFinancConsulta;
        public FrmFinanceiro()
        {
            InitializeComponent();

            toolStripExtrato.Visible = true;

            dsFiltrado = FiltroFinan.dsFinanceiro.Copy();

            evt_Fin = new Evt_Fin(dsFiltrado);

            comum = new EdtFin(evt_Fin);
            comum.MonteGrids();


            //dgvReceber.CellMouseDown += dgvPadrao_CellMouseDown;

            //  comum.errorProvider1 = this.errorProvider1;
            dgvReceber.KeyDown += dgvPadrao_KeyDown;
            dgvPagar.KeyDown += dgvPadrao_KeyDown;

            ssReceber.Items.Clear();
            ssReceber.Dock = DockStyle.None;
            ssReceber.SizingGrip = false;
            ssReceber.Width = 300;    //0; //14;
            ssReceber.AutoSize = false;
            ssReceber.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
            ssReceber.Items[ssReceber.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
            ssReceber.Items[ssReceber.Items.Count - 1].Text = "";
            ssReceber.Items[ssReceber.Items.Count - 1].AutoSize = true;
            ssPagar.Items.Clear();
            ssPagar.Dock = DockStyle.None;
            ssPagar.SizingGrip = false;
            ssPagar.Width = 300; //0; //14;
            ssPagar.AutoSize = false;
            ssPagar.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
            ssPagar.Items[ssPagar.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
            ssPagar.Items[ssPagar.Items.Count - 1].Text = "";
            ssPagar.Items[ssPagar.Items.Count - 1].AutoSize = true;
        }

        private void FrmFinanceiro_Load(object sender, EventArgs e)
        {

            evt_Fin.bmReceber.DataSource = dsFiltrado.Tables["RMOV_FIN"];
            evt_Fin.bmPagar.DataSource = dsFiltrado.Tables["PMOV_FIN"];


            comum.oReceber.oDataGridView = dgvReceber;
            comum.oReceber.oDataGridView.DataSource = evt_Fin.bmReceber;
            //comum.oReceber.ssTotal = ssReceber;

            evt_Fin.bmReceber.Sort = "DATA";
            evt_Fin.bmPagar.Sort = "DATA";

            comum.oPagar.oDataGridView = dgvPagar;
            comum.oPagar.oDataGridView.DataSource = evt_Fin.bmPagar;
            //   comum.oPagar.ssTotal = ssPagar;


            comum.oReceber.ConfigureDBGridView();
            comum.oPagar.ConfigureDBGridView();

            // FOrçar acrescenta coluna
            DataGridViewButtonColumn btnCol = new DataGridViewButtonColumn();
            btnCol.HeaderText = "Categ.";
            btnCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            btnCol.Width = 45;
            
            DataGridViewCellStyle cellStyle = new DataGridViewCellStyle();

            cellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            cellStyle.NullValue = "";

            btnCol.DefaultCellStyle = cellStyle;
            btnCol.SortMode = DataGridViewColumnSortMode.Automatic;
            comum.oReceber.oDataGridView.Columns.Insert(0, btnCol);

            btnCol = new DataGridViewButtonColumn();
            btnCol.HeaderText = "Categ.";
            btnCol.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            btnCol.Width = 45;
            btnCol.DefaultCellStyle = cellStyle;
            btnCol.SortMode = DataGridViewColumnSortMode.Automatic;
            
            comum.oPagar.oDataGridView.Columns.Insert(0, btnCol);









           // dgvReceber.RowHeadersWidth = 54;
           // dgvPagar.RowHeadersWidth = 54;
            dgvReceber.CellFormatting += dgvLancadosReceber_CellFormatting;
            dgvPagar.CellFormatting += dgvLancadosPagar_CellFormatting;


            dsFiltrado.Tables["PMOV_FIN"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.Pag_movFin_TableNewRow);
            dsFiltrado.Tables["RMOV_FIN"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.Rec_movFin_TableNewRow);
            // 
            // Monte as Edições
            MonteEdits();

            comum.oReceber.FuncaoSoma();
            // comum.oReceber.ColocaTotais();
            comum.oPagar.FuncaoSoma();
            // comum.oPagar.ColocaTotais();
            colocaSaldo();
        }
        private void MonteEdits()
        {
            bool ok = comum.MonteEdtReceber(evt_Fin.bmReceber);
            if (!ok) { MessageBox.Show("Erro ao montar Edição ENTRADAS..Possivelemente Acesso a dados falhou"); return; }
            comum.EdtReceber.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Rec_Fin_AlteraRegistrosOk);
            comum.EdtReceber.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Rec_Fin_BeforeAlteraRegistros);
            comum.EdtReceber.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;
            comum.EdtReceber.BeforeDeletaRegistros += evt_Fin.Padrao_BeforeDeletaRegistros;


            ok = comum.MonteEdtPagar(evt_Fin.bmPagar);
            if (!ok) { MessageBox.Show("Erro ao montar Edição SAIDAS..Possivelemente Acesso a dados falhou"); return; }
            comum.EdtPagar.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Pag_Fin_AlteraRegistrosOk);
            comum.EdtPagar.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.Pag_Fin_BeforeAlteraRegistros);
            comum.EdtPagar.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;
            comum.EdtPagar.BeforeDeletaRegistros += evt_Fin.Padrao_BeforeDeletaRegistros;

        }
        private void gbSaidas_Enter(object sender, EventArgs e)
        {
            pnSaidas.BackColor = SystemColors.ActiveCaption;
            if (toolTitulo.Text.ToUpper() == "ENTRADAS")
                toolTitulo.Text = "SAIDAS";
            if ((toolComboPaginas.Text.ToUpper() == "ENTRADAS") || (toolComboPaginas.Text.ToUpper().Trim() == ""))
            { toolComboPaginas.Text = "SAIDAS"; }
            bindingNavFin.BindingSource = evt_Fin.bmPagar;

        }
        private void gbEntradas_Enter(object sender, EventArgs e)
        {
            pnEntradas.BackColor = SystemColors.ActiveCaption;
            if (toolTitulo.Text.ToUpper() == "SAIDAS")
                toolTitulo.Text = "ENTRADAS";
            if ((toolComboPaginas.Text.ToUpper() == "SAIDAS") || (toolComboPaginas.Text.ToUpper().Trim() == ""))
            { toolComboPaginas.Text = "ENTRADAS"; }
            bindingNavFin.BindingSource = evt_Fin.bmReceber;

        }
        private void gbEntradas_Leave(object sender, EventArgs e)
        {
            pnEntradas.BackColor = SystemColors.Control;
        }
        private void gbSaidas_Leave(object sender, EventArgs e)
        {
            pnSaidas.BackColor = SystemColors.Control;
        }

        private void toolComboPaginas_SelectedIndexChanged(object sender, EventArgs e)
        {

            var texto = (sender as ToolStripComboBox).Text;
            if (texto.ToUpper() == "ENTRADAS")
            {
                if (toolTitulo.Text.ToUpper() != texto.ToUpper())
                {
                    dgvReceber.Focus();
                    toolTitulo.Text = "ENTRADAS";
                }
            }
            else if (texto.ToUpper() == "SAIDAS")
            {
                if (toolTitulo.Text.ToUpper() != texto.ToUpper())
                {

                    dgvPagar.Focus();
                    toolTitulo.Text = "SAIDAS";
                }
            }
        }

        private void dgvPadrao_KeyDown(object sender, KeyEventArgs e)
        {

            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;
            string nomeDataGrid = (sender as DataGridView).Name;
            if (nomeDataGrid.ToUpper() == "DGVRECEBER")
            {
                ArmePadrao = comum.EdtReceber;
                ogrid = comum.oReceber;
            }
            else if (nomeDataGrid.ToUpper() == "DGVPAGAR")
            {
                ArmePadrao = comum.EdtPagar;
                ogrid = comum.oPagar;
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
                dgvReceber.Refresh();
                dgvPagar.Refresh();

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
                dgvReceber.Refresh();
                dgvPagar.Refresh();

            }
        }

        // Edição do Mestre POR DENTRO DO BINDINGNAVIGATOR MESTRE


        private void toolBtnEditeDetalhe_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            DataGridView dataGridView = null;
            MonteGrid ogrid = null;
            if (gbEntradas.ContainsFocus)
            {
                ogrid = comum.oReceber;
                ArmePadrao = comum.EdtReceber;
            }
            else if (gbSaidas.ContainsFocus)
            {
                ogrid = comum.oPagar;
                ArmePadrao = comum.EdtPagar;
            }
            if (ogrid == null) return;
            if (ogrid.oDataGridView == null) return;
            dataGridView = ogrid.oDataGridView;

            dataGridView.Focus();
            if ((dataGridView.CurrentCell != null) && (dataGridView.CurrentCell.Selected))
                ArmePadrao.Edite(this, dataGridView.CurrentCell.OwningColumn.DataPropertyName);
            else
                ArmePadrao.Edite(this);
            ogrid.FuncaoSoma();
            // ogrid.ColocaTotais();
            colocaSaldo();
            dgvReceber.Refresh();
            dgvPagar.Refresh();
            return;
        }

        private void colocaSaldo()
        {

            decimal receber = 0;
            if (comum.oReceber.dictCampoTotal.ContainsKey("VALOR"))
                receber = Convert.ToDecimal(comum.oReceber.dictCampoTotal["VALOR"]);
            ssReceber.Items[0].Text = String.Format("Entradas: {0:#,###,##0.00}", receber);

            //toolEntradas.Visible = true;
            decimal pagar = 0;
            if (comum.oPagar.dictCampoTotal.ContainsKey("VALOR"))
                pagar = Convert.ToDecimal(comum.oPagar.dictCampoTotal["VALOR"]);
            ssPagar.Items[0].Text = String.Format("Saidas: {0:#,###,##0.00}", pagar);
            Decimal sdo = receber - pagar;
            toolSaldo.Text = String.Format("Saldo: {0:#,###,##0.00}", sdo);

        }

        private void toolSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolCheques_Click(object sender, EventArgs e)
        {
            FrmCheques frmCheques = new FrmCheques(evt_Fin.bmPagar);
            frmCheques.Show();

        }

        private void toolNovo_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            DataGridView dataGridView = null;
            MonteGrid ogrid = null;
            if (gbEntradas.ContainsFocus)
            {
                ogrid = comum.oReceber;
                ArmePadrao = comum.EdtReceber;
            }
            else if (gbSaidas.ContainsFocus)
            {
                ogrid = comum.oPagar;
                ArmePadrao = comum.EdtPagar;
            }
            if (ogrid == null) return;
            if (ogrid.oDataGridView == null) return;
            dataGridView = ogrid.oDataGridView;

            dataGridView.Focus();
            //  if ((dataGridView.CurrentCell != null) && (dataGridView.CurrentCell.Selected))
            ArmePadrao.Edite(this, "", true);
            ogrid.FuncaoSoma();
            // ogrid.ColocaTotais();
            colocaSaldo();
            dgvReceber.Refresh();
            dgvPagar.Refresh();


        }

        private void toolImprime_Click(object sender, EventArgs e)
        {
           

            bool primeiraimpressao = false;
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
        }

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
            string tituloDoc = "";
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


            if (dgvPagar.Focused)
            {
                ArmePadrao = comum.EdtPagar;
                ogrid = comum.oPagar;
            }
            else if (dgvReceber.Focused)
            {
                ArmePadrao = comum.EdtReceber;
                ogrid = comum.oReceber;
            }

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
                            ArmePadrao.OnAfterDeleta(new AlteraRegistroEventArgs(new DataRow[] { null } , DataRowState.Deleted ));
                            
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
            dgvReceber.Refresh();
            dgvPagar.Refresh();

        }

        private void toolEdita_Click(object sender, EventArgs e)
        {


            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


            if (dgvPagar.Focused)
            {
                ArmePadrao = comum.EdtPagar;
                ogrid = comum.oPagar;
            }
            else if (dgvReceber.Focused)
            {
                ArmePadrao = comum.EdtReceber;
                ogrid = comum.oReceber;
            }

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
            dgvReceber.Refresh();
            dgvPagar.Refresh();

        }

        #region COMPORTAMENTO BOTÂO Categoria (transf e Apro)
        private void geral_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // if ((e.Button == MouseButtons.Right) && (e.ColumnIndex == 0))

            if (e.RowIndex == -1) return;
            if  (e.ColumnIndex == 0)
            {
                if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value == null) return;
                if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "") return;

                if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "Pg")
                    toolStripMenuItemAprop.PerformClick();
                else if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "Rc")
                    toolStripMenuItemAprop.PerformClick();
                else if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "T->")
                    toolStripMenuItemTransfere.PerformClick();
                else if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "Tr")
                    VaParaOutroTransferido();
                
                // através do menu contex
                // var relativeMousePosition = (sender as DataGridView).PointToClient(Cursor.Position);
                //ContextMenuPop.Show((sender as DataGridView), relativeMousePosition);
            }
            else
           if (e.Button == MouseButtons.Right) {
                if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value == null) return;
                if (((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value.ToString().Trim() == "") return;
                var relativeMousePosition = (sender as DataGridView).PointToClient(Cursor.Position);
                ContextMenuPop.Show((sender as DataGridView), relativeMousePosition);

            }

        }

        private void dgvLancadosPagar_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("DATA"))
            {
                DataRowView registro = (((DataGridView)sender).Rows[e.RowIndex].DataBoundItem as DataRowView);
                if ((!registro.Row.IsNull("OUTRO_ID")) && (Convert.ToDouble(registro["OUTRO_ID"]) != 0))
                {
                    // ((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value = "Pg";
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "Pg";

                }
                else if (camposTransf(registro, 2))
                {
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "T->";
                    //((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value = "T->";
                    if (camposTransfConfirmado(registro, 2))
                        ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "Tr";
                    //    ((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value = "Tr";

                }
                else ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "";

            }
        }
        private void dgvLancadosReceber_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("DATA"))
            {
                DataRowView registro = (((DataGridView)sender).Rows[e.RowIndex].DataBoundItem as DataRowView);
                if ((!registro.Row.IsNull("OUTRO_ID")) && (Convert.ToDouble(registro["OUTRO_ID"]) != 0))
                {
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "Rc";
                    ///((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value = "Rc";
                   // toolStripMenuItemAprop.Enabled = true;
                }
                else if (camposTransf(registro, 1))
                {
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "T->";
                    //((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value = "T->";
                    if (camposTransfConfirmado(registro, 1))
                        ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "Tr";
                    // ((DataGridView)sender).Rows[e.RowIndex].HeaderCell.Value = "Tr";
                }
                else ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = "";
            }
        }



        private async void toolStripMenuItemAprop_Click(object sender, EventArgs e)
        {
            int ent_sai = 0;
            BindingSource bmDados = null;
            if (dgvReceber.Focused)
            {
                ent_sai = 1;
                bmDados = evt_Fin.bmReceber;
            }
            else if (dgvPagar.Focused)
            {
                ent_sai = 2;
                bmDados = evt_Fin.bmPagar;
            }
            else return;
            if (bmDados.Current == null) return;
            DataRowView orowView = (bmDados.Current as DataRowView);
                if (Convert.ToDouble(orowView["OUTRO_ID"]) == 0)

                {
                    MessageBox.Show("Este Lançamento Não tem Correspondente no PgRc");
                return;
            }

           // DataSet dados = await FinApropExtendida.FinAprop(Convert.ToDouble(orowView["OUTRO_ID"]));
            FrmFinAprop oform = new FrmFinAprop(ent_sai, bmDados);
            oform.ShowDialog();
            if (oform.mudou)
            {
                bool result = await frmFinancConsulta.ConsultaServidor();
                if (!result) return;
                dsFiltrado = FiltroFinan.dsFinanceiro.Copy();
                evt_Fin.dsFiltrado = dsFiltrado;

                evt_Fin.bmReceber.DataSource = dsFiltrado.Tables["RMOV_FIN"];
                evt_Fin.bmPagar.DataSource = dsFiltrado.Tables["PMOV_FIN"];
                dgvReceber.Refresh();
                dgvPagar.Refresh();

                comum.oPagar.FuncaoSoma();
                comum.oReceber.FuncaoSoma();
                // ogrid.ColocaTotais();
                colocaSaldo();
                

            }

        }

        private void VaParaOutroTransferido()
        {
            int ent_sai = 0;
            BindingSource bmDados = null;
            MonteGrid ogrid = null;   // datagridview do DESTINO (que irá receber a transferencia)
            if (dgvReceber.Focused)
            {
                ent_sai = 1;
                bmDados = evt_Fin.bmReceber;
                ogrid = comum.oPagar;

            }
            else if (dgvPagar.Focused)
            {
                ent_sai = 2;
                bmDados = evt_Fin.bmPagar;
                ogrid = comum.oReceber;

            }
            else return;
            if (bmDados.Current == null) return;
            DataRowView orowView = (bmDados.Current as DataRowView);
            string tdeb = "";
            string tcre = "";
            if (ent_sai == 2)
            {

                tcre = TabelasIniciais.Desc2Banco(orowView["CREDITO"].ToString()).Trim();
                tdeb = orowView.Row.IsNull("DEBITO") ? "" : orowView["DEBITO"].ToString();
                if ((tcre != "") && (tdeb.Length > 0) && (tdeb.Substring(0, 1) == "*"))
                {
                    tcre = "*" + ((tcre.Length > 24) ? tcre.Substring(0, 24) : tcre);
                }
                tdeb = TabelasIniciais.NBancoDesc2(orowView["DEBITO"].ToString()).Trim();
            }
            else
            {
                tdeb = TabelasIniciais.Desc2Banco(orowView["DEBITO"].ToString()).Trim();
                tcre = orowView.Row.IsNull("CREDITO") ? "" : orowView["CREDITO"].ToString();
                if ((tdeb != "") && (tcre.Length > 0) && (tcre.Substring(0, 1) == "*"))
                {
                    tdeb = "*" + ((tdeb.Length > 24) ? tdeb.Substring(0, 24) : tcre);
                }
                tcre = TabelasIniciais.NBancoDesc2(orowView["CREDITO"].ToString()).Trim();

            }
            if ((tcre == "") || (tdeb == ""))
            {
                MessageBox.Show("Não Foi Possivel ir");
                return;
            }
            // verifica se já foi lancado
            Double valor = Convert.ToDouble(orowView["VALOR"]);
            DateTime data = Convert.ToDateTime(orowView["DATA"]);
            DataRowView dados = null;

            if (ent_sai == 2)
            {

                try
                {

                    int posicaoAnt = evt_Fin.bmReceber.Position;
                    int posicaoDesejada = -1;
                    evt_Fin.bmReceber.MoveFirst();
                    while (evt_Fin.bmReceber.Position < evt_Fin.bmReceber.Count)
                    {
                        dados = (evt_Fin.bmReceber.Current as DataRowView);
                        if ((dados["DEBITO"].ToString().Trim() == tdeb.Trim()) && (dados["CREDITO"].ToString().Trim() == tcre.Trim())
                              && (Convert.ToDateTime(dados["DATA"]) == data)
                              && (Convert.ToDouble(dados["VALOR"]) == valor)
                            )
                        {
                            posicaoDesejada = evt_Fin.bmReceber.Position;
                            break;
                        }
                        evt_Fin.bmReceber.MoveNext();
                    }
                    if (posicaoDesejada != -1)
                    {
                        ogrid.oDataGridView.Focus();
                        evt_Fin.bmReceber.Position = posicaoDesejada;
                    }
                    else
                        evt_Fin.bmReceber.Position = posicaoAnt;

                }
                catch { }


            }
            else
            {
                try
                {

                    int posicaoAnt = evt_Fin.bmPagar.Position;
                    int posicaoDesejada = -1;
                    evt_Fin.bmPagar.MoveFirst();
                    while (evt_Fin.bmPagar.Position < evt_Fin.bmPagar.Count)
                    {
                        dados = (evt_Fin.bmPagar.Current as DataRowView);

                        if ((dados["DEBITO"].ToString().Trim() == tdeb.Trim()) && (dados["CREDITO"].ToString().Trim() == tcre.Trim())
                              && (Convert.ToDateTime(dados["DATA"]) == data)
                              && (Convert.ToDouble(dados["VALOR"]) == valor)
                            )
                        {
                            posicaoDesejada = evt_Fin.bmPagar.Position;
                            break;
                        }
                        evt_Fin.bmPagar.MoveNext();
                    }
                    if (posicaoDesejada != -1)
                    {
                        ogrid.oDataGridView.Focus();
                        evt_Fin.bmPagar.Position = posicaoDesejada;
                    }
                    else
                        evt_Fin.bmPagar.Position = posicaoAnt;

                }
                catch { }
            }

            ogrid.oDataGridView.Refresh();

        }
        private async void toolStripMenuItemTransfere_Click(object sender, EventArgs e)
        {
            int ent_sai = 0;
            BindingSource bmDados = null;
            MonteGrid ogrid = null;   // datagridview do DESTINO (que irá receber a transferencia)
            if (dgvReceber.Focused)
            {
                ent_sai = 1;
                bmDados = evt_Fin.bmReceber;
                ogrid = comum.oPagar;

            }
            else if (dgvPagar.Focused)
            {
                ent_sai = 2;
                bmDados = evt_Fin.bmPagar;
                ogrid = comum.oReceber;
            
            }
            else return;
            if (bmDados.Current == null) return;
            DataRowView orowView = (bmDados.Current as DataRowView);
            string tdeb = "";
            string tcre = "";
            if (ent_sai == 2)
            {

                tcre = TabelasIniciais.Desc2Banco(orowView["CREDITO"].ToString()).Trim();
                tdeb = orowView.Row.IsNull("DEBITO") ? "" : orowView["DEBITO"].ToString();
                if ((tcre != "") && (tdeb.Length > 0) && (tdeb.Substring(0, 1) == "*"))
                {
                    tcre = "*" + ((tcre.Length > 24) ? tcre.Substring(0, 24) : tcre);
                }
                tdeb = TabelasIniciais.NBancoDesc2(orowView["DEBITO"].ToString()).Trim();
            }
            else
            {
                tdeb = TabelasIniciais.Desc2Banco(orowView["DEBITO"].ToString()).Trim();
                tcre = orowView.Row.IsNull("CREDITO") ? "" : orowView["CREDITO"].ToString();
                if ((tdeb != "") && (tcre.Length > 0) && (tcre.Substring(0, 1) == "*"))
                {
                    tdeb = "*" + ((tdeb.Length > 24) ? tdeb.Substring(0, 24) : tcre);
                }
                tcre = TabelasIniciais.NBancoDesc2(orowView["CREDITO"].ToString()).Trim();

            }
            if ((tcre == "") || (tdeb == ""))
            {
                MessageBox.Show("Não é possivel Esta Transferencia");
                return;
            }
            // verifica se já foi lancado
            Double valor = Convert.ToDouble(orowView["VALOR"]);
            DateTime data = Convert.ToDateTime(orowView["DATA"]);
            DataRow dados = null;

            if (ent_sai == 2)
            {
                try
                {
                    dados = (evt_Fin.bmReceber.DataSource as DataTable).AsEnumerable().Where(row =>
                ((row.Field<string>("DEBITO").Trim() == tdeb.Trim())
                 && (row.Field<string>("CREDITO").Trim() == tcre.Trim())
                   && (row.Field<Double>("VALOR") == valor)
                   && (row.Field<DateTime>("DATA") == data)
                 )
                 ).FirstOrDefault();

                }
                catch (Exception E)
                {

                }

                if (dados != null)
                {
                    MessageBox.Show("Já lançada Transferencia");
                    return;
                }
                bool retorno = await evt_Fin.InsereTransferencia(tdeb, tcre, evt_Fin.bmPagar, evt_Fin.bmReceber);
                if (!retorno)
                {
                    MessageBox.Show("Erro Transferencia");
                    return;
                }
                evt_Fin.bmReceber.Sort = "DATA";

            }
            else
            {
                try
                {
                    dados = (evt_Fin.bmPagar.DataSource as DataTable).AsEnumerable().Where(row =>
                ((row.Field<string>("DEBITO").Trim() == tdeb.Trim())
                 && (row.Field<string>("CREDITO").Trim() == tcre.Trim())
                   && (row.Field<Double>("VALOR") == valor)
                   && (row.Field<DateTime>("DATA") == data)
                 )
                 ).FirstOrDefault();

                }
                catch (Exception E)
                {
                }

                if (dados != null)
                {
                    MessageBox.Show("Já lançada Transferencia");
                    return;
                }

                bool retorno = await evt_Fin.InsereTransferencia(tdeb, tcre, evt_Fin.bmReceber, evt_Fin.bmPagar);

                
                if (!retorno)
                {
                    MessageBox.Show("Erro Transferencia");
                    return;
                }
               
                evt_Fin.bmPagar.Sort = "DATA";

            }

            if (dgvReceber.Focused)
            {

                dgvReceber.Refresh();

            }
            else if (dgvPagar.Focused)
            {
                dgvPagar.Refresh();
              

            }




            ogrid.oDataGridView.Refresh();
            
            ogrid.FuncaoSoma();
            colocaSaldo();
        }


        private bool camposTransf(DataRowView orowView, int ent_sai)
        {
            string tdeb = "";
            string tcre = "";
            if (ent_sai == 2)
            {
                tcre = TabelasIniciais.Desc2Banco(orowView["CREDITO"].ToString()).Trim();
                tdeb = orowView.Row.IsNull("DEBITO") ? "" : orowView["DEBITO"].ToString();
                if ((tcre != "") && (tdeb.Length > 0) && (tdeb.Substring(0, 1) == "*"))
                {
                    tcre = "*" + ((tcre.Length > 24) ? tcre.Substring(0, 24) : tcre);
                }
                tdeb = TabelasIniciais.NBancoDesc2(orowView["DEBITO"].ToString()).Trim();
            }
            else
            {
                tdeb = TabelasIniciais.Desc2Banco(orowView["DEBITO"].ToString()).Trim();
                tcre = orowView.Row.IsNull("CREDITO") ? "" : orowView["CREDITO"].ToString();
                if ((tdeb != "") && (tcre.Length > 0) && (tcre.Substring(0, 1) == "*"))
                {
                    tdeb = "*" + ((tdeb.Length > 24) ? tdeb.Substring(0, 24) : tcre);
                }
                tcre = TabelasIniciais.NBancoDesc2(orowView["CREDITO"].ToString()).Trim();

            }
            bool result = true;
            if ((tdeb == "") || (tcre == ""))
                result = false;
            return result;
        }

        private bool camposTransfConfirmado(DataRowView orowView, int ent_sai)
        {
            string tdeb = "";
            string tcre = "";
            if (ent_sai == 2)
            {
                tcre = TabelasIniciais.Desc2Banco(orowView["CREDITO"].ToString()).Trim();
                tdeb = orowView.Row.IsNull("DEBITO") ? "" : orowView["DEBITO"].ToString();
                if ((tcre != "") && (tdeb.Length > 0) && (tdeb.Substring(0, 1) == "*"))
                {
                    tcre = "*" + ((tcre.Length > 24) ? tcre.Substring(0, 24) : tcre);
                }
                tdeb = TabelasIniciais.NBancoDesc2(orowView["DEBITO"].ToString()).Trim();
            }
            else
            {
                tdeb = TabelasIniciais.Desc2Banco(orowView["DEBITO"].ToString()).Trim();
                tcre = orowView.Row.IsNull("CREDITO") ? "" : orowView["CREDITO"].ToString();
                if ((tdeb != "") && (tcre.Length > 0) && (tcre.Substring(0, 1) == "*"))
                {
                    tdeb = "*" + ((tdeb.Length > 24) ? tdeb.Substring(0, 24) : tcre);
                }
                tcre = TabelasIniciais.NBancoDesc2(orowView["CREDITO"].ToString()).Trim();

            }
            
            if ((tdeb == "") || (tcre == "")) return false;
            Double valor = Convert.ToDouble(orowView["VALOR"]);
            DateTime data = Convert.ToDateTime(orowView["DATA"]);
            DataRow dados = null;
            bool result = false;
            if (ent_sai == 2)
            {
                try
                {
                    dados = (evt_Fin.bmReceber.DataSource as DataTable).AsEnumerable().Where(row =>
                ((row.Field<string>("DEBITO").Trim() == tdeb.Trim())
                 && (row.Field<string>("CREDITO").Trim() == tcre.Trim())
                   && (row.Field<Double>("VALOR") == valor)
                   && (row.Field<DateTime>("DATA") == data)
                 )
                 ).FirstOrDefault();

                }
                catch (Exception E)
                {

                }

                if (dados != null)
                {
                    result = true;
                }
            }
            else
            {
                try
                {
                    dados = (evt_Fin.bmPagar.DataSource as DataTable).AsEnumerable().Where(row =>
                ((row.Field<string>("DEBITO").Trim() == tdeb.Trim())
                 && (row.Field<string>("CREDITO").Trim() == tcre.Trim())
                   && (row.Field<Double>("VALOR") == valor)
                   && (row.Field<DateTime>("DATA") == data)
                 )
                 ).FirstOrDefault();

                }
                catch (Exception E)
                {
                }

                if (dados != null)
                {
                    result = true;
                }
            }
            return result;
        }


        private void toolStripBtnCategoria_Click(object sender, EventArgs e)
        {
            // MonteGrid ogrid = null;   // datagridview do DESTINO (que irá receber a transferencia)
            DataGridView odatagrid = null;
            if (dgvReceber.Focused)
            {
                odatagrid = dgvReceber;
               // ogrid = comum.oPagar;

            }
            else if (dgvPagar.Focused)
            {
                odatagrid = dgvPagar;
                //ogrid = comum.oReceber;

            }
            else return;
            if (odatagrid.CurrentRow == null) return;
            
            if (odatagrid.Rows[odatagrid.CurrentRow.Index].Cells[0].Value == null) return;
            if (odatagrid.Rows[odatagrid.CurrentRow.Index].Cells[0].Value.ToString().Trim() == "") return;

            if (odatagrid.Rows[odatagrid.CurrentRow.Index].Cells[0].Value.ToString().Trim() == "Pg")
                toolStripMenuItemAprop.PerformClick();
            else if (odatagrid.Rows[odatagrid.CurrentRow.Index].Cells[0].Value.ToString().Trim() == "Rc")
                toolStripMenuItemAprop.PerformClick();
            else if (odatagrid.Rows[odatagrid.CurrentRow.Index].Cells[0].Value.ToString().Trim() == "T->")
                toolStripMenuItemTransfere.PerformClick();
            else if (odatagrid.Rows[odatagrid.CurrentRow.Index].Cells[0].Value.ToString().Trim() == "Tr")
                VaParaOutroTransferido();
        }


        #endregion

      

        #region EXTRATO
        private void toolStripExtrato_Click(object sender, EventArgs e)
        {
            if ( (evt_Fin.bmPagar.Count == 0) && (evt_Fin.bmReceber.Count == 0))
            {
                MessageBox.Show("Dados Vazios");
                return;
            }

            FrmExtratos frmExtratos = new FrmExtratos(evt_Fin.bmPagar, evt_Fin.bmReceber);
            if (frmExtratos != null)
                frmExtratos.Show();
        }






        #endregion

        private void toolStripAutoLancBB_Click(object sender, EventArgs e)
        {
            FrmTarifasLancAutoBB frmAutoLancBB = new FrmTarifasLancAutoBB(evt_Fin.bmPagar, evt_Fin.bmReceber);
            if (frmAutoLancBB != null)
                frmAutoLancBB.Show();
        }

        private void toolMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
    
}

 