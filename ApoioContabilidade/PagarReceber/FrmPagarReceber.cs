using ApoioContabilidade.Almoxarifado.ServicesAlmox;
using ApoioContabilidade.Models;
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
    public partial class FrmPagarReceber : Form
    {
       
        Evt_PgRc evt_Fin;
        EdtPgRc comum;
        DataSet dsFiltrado;
        public FrmPagarReceber(int tipoConta, DataSet odsFiltrado)
        {
            
            InitializeComponent();
            tcDetalhes.SelectedIndex = -1;
         
            this.KeyPreview = true;
            this.KeyDown += FrmPagarReceber_KeyDown;

            dsFiltrado = odsFiltrado;
            tcDetalhes.TabIndex = 1;
            evt_Fin = new Evt_PgRc(tipoConta, dsFiltrado);
            evt_Fin.form = this;
            comum = new EdtPgRc(tipoConta, evt_Fin);

            comum.MonteGrids();

            tcDetalhes.SuspendLayout();
            tcDetalhes.TabPages.Clear();
            if (comum.TipoConta == 1)
            {
                // tpEstoque.Show();
                // 
                tcDetalhes.TabPages.Add(tpPagar);
                tcDetalhes.TabPages.Add(tpCentros);
                tcDetalhes.TabPages.Add(tpEstoque);
                tcDetalhes.TabPages.Add(tpContab);

                // tcDetalhes.ResumeLayout();

            }
            else
            {
                // tpVendas.Show();
                tcDetalhes.TabPages.Add(tpPagar);
                tcDetalhes.TabPages.Add(tpCentros);
                tcDetalhes.TabPages.Add(tpVendas);
                tcDetalhes.TabPages.Add(tpContab);
            }

            tcDetalhes.SelectedTab = tpPagar;
            tcDetalhes.ResumeLayout();


            bindingNavDetalhe.BindingSource = evt_Fin.bmDetalhe1;
            toolDetalheTitulo.Text = tpPagar.Text;
            toolComboPaginas.Items.Clear();
            foreach (TabPage pag in tcDetalhes.TabPages)
            {
                toolComboPaginas.Items.Add(pag.Text);
            }
            
            toolComboPaginas.SelectedIndex = 0;
            toolComboPaginas.Text = tpPagar.Text;

            
            /*  
              
            */
            // para retornar no browse um valor diferente do campo original 
            dgvCentros.CellFormatting += DgvCentros_CellFormatting;
            dgvEstoque.CellFormatting += DgvCentros_CellFormatting;
            dgvVendas.CellFormatting += DgvVendas_CellFormatting;

            dgvMestre.KeyDown += dgvPadrao_KeyDown;
            dgvPagar.KeyDown += dgvPadrao_KeyDown;
            dgvAprop.KeyDown += dgvPadrao_KeyDown;
            dgvCentros.KeyDown += dgvPadrao_KeyDown;
            dgvEstoque.KeyDown += dgvPadrao_KeyDown;

           // comum.errorProvider1 = this.errorProvider1;


            dgvAprop.Enter += color_Enter;
            dgvAprop.Leave += color_Leave;

            dgvMestre.Enter += color_Enter;
            dgvMestre.Leave += color_Leave;

            dgvPagar.Enter += color_Enter;
            dgvPagar.Leave += color_Leave;

            dgvVendas.Enter += color_Enter;
            dgvVendas.Leave += color_Leave;

            dgvEstoque.Enter += color_Enter;
            dgvEstoque.Leave += color_Leave;

            dgvCentros.Enter += color_Enter;
            dgvCentros.Leave += color_Leave;

            toolEdite.Enabled = false;
            toolNovo.Enabled = false;
            toolDelete.Enabled = false;

        }
        private void FrmPagarReceber_Load(object sender, EventArgs e)
        {

            if (comum.TipoConta == 1)
            {
                gbMestre.Text = "Pagamentos        &Mestre";

            }
            else
            {
                gbMestre.Text = "Recebimentos       &Mestre";

            }
            toolLabelMestre.Text = gbMestre.Text + " {Mestre}";

            evt_Fin.bmMestre.DataSource = dsFiltrado.Tables["MOV_PGRC"].AsDataView();
            evt_Fin.bmDetalhe1.DataSource = dsFiltrado.Tables["MOV_FIN"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            evt_Fin.bmDetalhe2.DataSource = dsFiltrado.Tables["MOV_APRO"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            evt_Fin.bmDetalheCentro.DataSource = dsFiltrado.Tables["CTACENTR"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            evt_Fin.bmEstoque.DataSource = dsFiltrado.Tables["MOVEST"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            evt_Fin.bmVendas.DataSource = dsFiltrado.Tables["VENDAS"].AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            comum.oMestre.oDataGridView = dgvMestre;
            comum.oMestre.oDataGridView.DataSource = evt_Fin.bmMestre;
            // comum.oMestre.sbTotal = sbMestre;

            comum.oDetalhe1.oDataGridView = dgvPagar;
            comum.oDetalhe1.oDataGridView.DataSource = evt_Fin.bmDetalhe1;
            // comum.oDetalhe1.sbTotal = sbPagar;

            comum.oDetalhe2.oDataGridView = dgvAprop;
            comum.oDetalhe2.oDataGridView.DataSource = evt_Fin.bmDetalhe2;
            // comum.oDetalhe2.sbTotal = sbAprop;

            comum.oDetalheCentro.oDataGridView = dgvCentros;
            comum.oDetalheCentro.oDataGridView.DataSource = evt_Fin.bmDetalheCentro;
            // comum.oDetalheCentro.sbTotal = sbCentros;

            comum.oDetalheEst.oDataGridView = dgvEstoque;
            comum.oDetalheEst.oDataGridView.DataSource = evt_Fin.bmEstoque;
            // comum.oDetalheEst.sbTotal = sbEstoque;

            comum.oVenda.oDataGridView = dgvVendas;
            comum.oVenda.oDataGridView.DataSource = evt_Fin.bmVendas;
            // comum.oVenda.sbTotal = sbVendas;

            comum.oMestre.ConfigureDBGridView();
            comum.oDetalhe1.ConfigureDBGridView();
            comum.oDetalhe2.ConfigureDBGridView();
            comum.oDetalheCentro.ConfigureDBGridView();
            comum.oDetalheEst.ConfigureDBGridView();
            comum.oVenda.ConfigureDBGridView();
            comum.oDetalheEst.ConfigureDBGridView();
            evt_Fin.comum = comum;



            dsFiltrado.Tables["MOV_PGRC"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.movPgRc_TableNewRow);
            dsFiltrado.Tables["MOV_FIN"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.movFin_TableNewRow);
            dsFiltrado.Tables["MOV_APRO"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.movApro_TableNewRow);
            dsFiltrado.Tables["CTACENTR"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.ctaCentro_TableNewRow);
            if (comum.TipoConta == 1) dsFiltrado.Tables["MOVEST"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.deposito_TableNewRow);
            else dsFiltrado.Tables["VENDAS"].TableNewRow += new DataTableNewRowEventHandler(evt_Fin.venda_TableNewRow);
            // 
            // Monte as Edições
            MonteEdits();
            //colocaSaldoDetalhe()
            if (evt_Fin.bmMestre.Count == 0)
            {
                evt_Fin.bmMestre.PositionChanged -= evt_Fin.Mestre_PositionChanged;
                evt_Fin.bmMestre.PositionChanged += evt_Fin.Mestre_PositionChanged;

                evt_Fin.Mestre_PositionChanged(evt_Fin.bmMestre, null);
            }
            else
            {
                evt_Fin.bmMestre.PositionChanged -= evt_Fin.Mestre_PositionChanged;
                evt_Fin.bmMestre.PositionChanged += evt_Fin.Mestre_PositionChanged;
                evt_Fin.bmMestre.MoveFirst();
                evt_Fin.Mestre_PositionChanged(evt_Fin.bmMestre, null);
                //bmMestre.ResetCurrentItem();
            }

            


            comum.oMestre.FuncaoSoma();
            colocaSaldo();
            bindingNavMestre.BindingSource = evt_Fin.bmMestre;

            dgvMestre.Focus();
        }



        private void FrmPagarReceber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        toolNovo.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.E:
                        toolEdite.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.D:
                        toolDelete.PerformClick();
                        e.Handled = true;
                        break;
                }
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {

            if (keyData == (Keys.Alt | Keys.D1))
            {
                if (tcDetalhes.SelectedIndex == 0)
                    dgvPagar.Focus();
                else
                    tcDetalhes.SelectTab(0);
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D2))
            {
                tcDetalhes.SelectTab(1);
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D3))
            {
                tcDetalhes.SelectTab(2);
                //oPesqFin.TabIndex = 2;
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D4))
            {
                tcDetalhes.SelectTab(3);
                //oPesqFin.TabIndex = 3;
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.M))
            {
                dgvMestre.Focus();
                //oPesqFin.TabIndex = 3;
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }



       
        private void DgvCentros_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("NUM_MOD"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.ModeloDesc(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("CODSER"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.ServicoDesc(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("ICODSER"))
            {
                double stringValue;
                try
                {
                    stringValue = Convert.ToDouble(e.Value);
                }
                catch (Exception)
                {
                    return;
                }
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue.ToString();
                e.Value = TabelasIniciais.TipoMovDesc(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("COD")) // codigo de materiais do esstoque
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.EstoqueDesc(stringValue);
            }
        }
        private void DgvVendas_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("PROD"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.ProdutoDesc(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("FIRMA"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                e.Value = TabelasIniciais.NomeFirma(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("PROD_TP"))
            {
                Int32 stringValue = Convert.ToInt32(e.Value);
                // if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue.ToString();
                e.Value = TabelasIniciais.TipoProdDescricao(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("CERTIF"))
            {
                int stringValue;
                try
                {
                    stringValue = Convert.ToInt32(e.Value);
                }
                catch (Exception)
                {
                    return;
                }
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue.ToString();
                e.Value = TabelasIniciais.CertificadoDesc(stringValue);
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("COMPLEM"))
            {
                int stringValue;
                try
                {
                    stringValue = Convert.ToInt32(e.Value);
                }
                catch (Exception)
                {
                    return;
                }
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue.ToString();
                e.Value = TabelasIniciais.ComplementoDesc(stringValue);
            }

        }
        private void MonteEdits()
        {
            bool ok = comum.MonteEditMestre(evt_Fin.bmMestre);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Mestre"); return; }
            comum.EdtMestre.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.EdtMestre_AlteraRegistrosOk);
               // async (s, e) => await evt_Fin.EdtMestre_NewAlteraRegistrosOk;


               comum.EdtMestre.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.EdtMestre_BeforeAlteraRegistros);
            comum.EdtMestre.BeforeDeletaRegistros += evt_Fin.EdtMestre_BeforeDeletaRegistros;
            comum.EdtMestre.DeletaRegistrosOk += evt_Fin.EdtMestre_DeletaRegistrosOk;
            

            ok = comum.MonteEditDetalhe1(evt_Fin.bmDetalhe1);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Financeiro"); return; }
            comum.EdtDetalhe1.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.EdtDetalhe1_AlteraRegistrosOk);
            comum.EdtDetalhe1.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.EdtDetalhe1_BeforeAlteraRegistros);
            comum.EdtDetalhe1.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;

            ok = comum.MonteEditDetalhe2(evt_Fin.bmDetalhe2);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Apropriação"); return; }
            comum.EdtDetalhe2.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.EdtDetalhe2_AlteraRegistrosOk);
            comum.EdtDetalhe2.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.EdtDetalhe2_BeforeAlteraRegistros);
            comum.EdtDetalhe2.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;

            ok = comum.MonteEditCentro(evt_Fin.bmDetalheCentro);
            if (!ok) { MessageBox.Show("Erro ao montar Edição Centros"); return; }
            comum.EdtDetalheCentro.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.ctaCentro_AlteraRegistrosOk);
            comum.EdtDetalheCentro.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.ctaCentro_BeforeAlteraRegistros);
            comum.EdtDetalheCentro.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;
            comum.EdtDetalheCentro.OnPrimeiroRegistro += comum.EdtCentro_ConfigureInicial;
            if (comum.TipoConta == 1)
            {
                ok = comum.MonteEditEstoque(evt_Fin.bmEstoque);
                if (!ok) { MessageBox.Show("Erro ao montar Edição Compras Estoque"); return; }
                comum.EdtDetalheEst.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.deposito_AlteraRegistrosOk);
                comum.EdtDetalheEst.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.deposito_BeforeAlteraRegistros);
                comum.EdtDetalheEst.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;
            }
            else
            {
                ok = comum.MonteEditVenda(evt_Fin.bmVendas);
                if (!ok) { MessageBox.Show("Erro ao montar Edição Contratos de Venda"); return; }
                comum.EdtVenda.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.venda_AlteraRegistrosOk);
                comum.EdtVenda.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evt_Fin.venda_BeforeAlteraRegistros);
                comum.EdtVenda.DeletaRegistrosOk += evt_Fin.Padrao_DeletaRegistrosOk;
            }
        }




        ////////////////////
        /// Area de Edição
        /// 
        private void dgvPadrao_KeyDown(object sender, KeyEventArgs e)
        {
          /*  ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;
            string nomeDataGrid = (sender as DataGridView).Name;
            if (nomeDataGrid.ToUpper() == "DGVMESTRE")
            { ArmePadrao = comum.EdtMestre;
                ogrid = comum.oMestre;
            }
            else if (nomeDataGrid.ToUpper() == "DGVPAGAR")
            {
                ArmePadrao = comum.EdtDetalhe1;
                ogrid = comum.oDetalhe1;
            }
            else if (nomeDataGrid.ToUpper() == "DGVAPROP")
            {
                ArmePadrao = comum.EdtDetalhe2;
                ogrid = comum.oDetalhe2;
            }
            else if (nomeDataGrid.ToUpper() == "DGVCENTROS")
            {
                ArmePadrao = comum.EdtDetalheCentro;
                ogrid = comum.oDetalheCentro;
            }
            else if (nomeDataGrid.ToUpper() == "DGVESTOQUE")
            {
                ArmePadrao = comum.EdtDetalheEst;
                ogrid = comum.oDetalheEst;
            }
            else if (nomeDataGrid.ToUpper() == "DGVVENDAS")
            {
                ArmePadrao = comum.EdtVenda;
                ogrid = comum.oVenda;
            }


            if (ArmePadrao == null) return;

            if (((int)e.KeyCode == 113)) // F2
            {
               // if ((((DataGridView)sender).CurrentCell != null) && (((DataGridView)sender).CurrentCell.Selected))
                 //   ArmePadrao.Edite(this, ((DataGridView)sender).CurrentCell.OwningColumn.DataPropertyName);
                //else
                    ArmePadrao.Edite(this);
                ogrid.FuncaoSoma();
                colocaSaldo();
                
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
                colocaSaldo();
                
            }*/
        }

        private void tcDetalhes_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            if (evt_Fin == null) return;
            TabControl otab = (sender as TabControl);
            if (otab.SelectedIndex == -1) return;
            if (otab.SelectedTab.Name.ToUpper() == "TPPAGAR")
            {
               // if (otab.ContainsFocus) dgvPagar.Focus();
                bindingNavDetalhe.BindingSource = evt_Fin.bmDetalhe1;
                toolDetalheTitulo.Text = tpPagar.Text;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCENTROS")
            {
                //if (otab.ContainsFocus) dgvCentros.Focus();
                bindingNavDetalhe.BindingSource = evt_Fin.bmDetalheCentro;
                toolDetalheTitulo.Text = tpCentros.Text;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPESTOQUE")
            {
                //if (otab.ContainsFocus) dgvEstoque.Focus();
                bindingNavDetalhe.BindingSource = evt_Fin.bmEstoque;
                toolDetalheTitulo.Text = tpEstoque.Text;
            }

            else if (otab.SelectedTab.Name.ToUpper() == "TPVENDAS")
            {
                //if (otab.ContainsFocus) dgvVendas.Focus();  
                 bindingNavDetalhe.BindingSource = evt_Fin.bmVendas;
                toolDetalheTitulo.Text = tpVendas.Text;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCONTAB")
            {
                //if (otab.ContainsFocus) dgvAprop.Focus();
                bindingNavDetalhe.BindingSource = evt_Fin.bmDetalhe2;
                toolDetalheTitulo.Text = tpContab.Text;
            }
            toolComboPaginas.SelectedItem = toolDetalheTitulo.Text;
        }

        // Edição do Mestre POR DENTRO DO BINDINGNAVIGATOR MESTRE

        private void Editar_Novo_Detalhe(bool novo)
        {
            ArmeEdicao ArmePadrao = null;
            DataGridView dataGridView = null;
            MonteGrid ogrid = null;
            TabControl otab = tcDetalhes;
            if (evt_Fin.bmMestre.Count == 0)
            {
                MessageBox.Show("Inclua Antes registro Mestre");
                return;
            }
            if (otab.SelectedTab.Name.ToUpper() == "TPPAGAR")
            {

                ogrid = comum.oDetalhe1;
                ArmePadrao = comum.EdtDetalhe1;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCENTROS")
            {
                ogrid = comum.oDetalheCentro;
                ArmePadrao = comum.EdtDetalheCentro;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPESTOQUE")
            {
                ogrid = comum.oDetalheEst;
                ArmePadrao = comum.EdtDetalheEst;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPVENDAS")
            {
                ogrid = comum.oVenda;
                ArmePadrao = comum.EdtVenda;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCONTAB")
            {
                ogrid = comum.oDetalhe2;
                ArmePadrao = comum.EdtDetalhe2;
            }
            if (ogrid == null) return;
            if (ogrid.oDataGridView == null) return;
            dataGridView = ogrid.oDataGridView;

            dataGridView.Focus();
            ArmePadrao.Edite(this,"",novo);
            ogrid.FuncaoSoma();
            colocaSaldo();
            return;

        }

        private void Editar_Novo_Mestre(bool novo)
        {
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = comum.oMestre;
            dgvMestre.Focus();
            ArmePadrao = comum.EdtMestre;
          
           ArmePadrao.Edite(this,"",novo);

            ogrid.FuncaoSoma();
            colocaSaldo();

        }

        private void toolBtnEditeDetalhe_Click(object sender, EventArgs e)
        {
            Editar_Novo_Detalhe(false);
            return;

        }

        private void toolBtnEditeMestre_Click(object sender, EventArgs e)
        {
            Editar_Novo_Mestre(false);
            return;

        }

      
        private void toolComboPaginas_SelectedIndexChanged(object sender, EventArgs e)
        {
            var texto = (sender as ToolStripComboBox).Text;
            foreach (TabPage pag in tcDetalhes.TabPages)
            {
                if (pag.Text == texto)
                {
                    tcDetalhes.SelectedTab = pag;
                    break;
                }
            }
            colocaSaldoDetalhe();

        }

        private void toolStripSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolNovo_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


            if (gbMestre.ContainsFocus)
            {
                if (!dgvMestre.Focused)
                    dgvMestre.Focus();
                ArmePadrao = comum.EdtMestre;
                ogrid = comum.oMestre;
            }
            else
            if (tcDetalhes.ContainsFocus) 
            {
                TabControl otab = tcDetalhes;
                if (otab.SelectedTab.Name.ToUpper() == "TPPAGAR")
                {

                    ogrid = comum.oDetalhe1;
                    ArmePadrao = comum.EdtDetalhe1;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPCENTROS")
                {
                    ogrid = comum.oDetalheCentro;
                    ArmePadrao = comum.EdtDetalheCentro;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPESTOQUE")
                {
                    ogrid = comum.oDetalheEst;
                    ArmePadrao = comum.EdtDetalheEst;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPVENDAS")
                {
                    ogrid = comum.oVenda;
                    ArmePadrao = comum.EdtVenda;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPCONTAB")
                {
                    ogrid = comum.oDetalhe2;
                    ArmePadrao = comum.EdtDetalhe2;
                }

            }

            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                ArmePadrao = comum.EdtMestre;
                ogrid = comum.oMestre;
                dgvMestre.Focus();
            }    
                

            ArmePadrao.Edite(this,"",true);
            ogrid.FuncaoSoma();
            colocaSaldo();
            return;
        }

    
        private void toolEdite_Click(object sender, EventArgs e)
        {
            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


            if (gbMestre.ContainsFocus)
            {
                if (!dgvMestre.Focused)
                    dgvMestre.Focus();
                ArmePadrao = comum.EdtMestre;
                ogrid = comum.oMestre;
            }
            else
            if (tcDetalhes.ContainsFocus)
            {
                TabControl otab = tcDetalhes;
                if (otab.SelectedTab.Name.ToUpper() == "TPPAGAR")
                {

                    ogrid = comum.oDetalhe1;
                    ArmePadrao = comum.EdtDetalhe1;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPCENTROS")
                {
                    ogrid = comum.oDetalheCentro;
                    ArmePadrao = comum.EdtDetalheCentro;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPESTOQUE")
                {
                    ogrid = comum.oDetalheEst;
                    ArmePadrao = comum.EdtDetalheEst;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPVENDAS")
                {
                    ogrid = comum.oVenda;
                    ArmePadrao = comum.EdtVenda;
                }
                else if (otab.SelectedTab.Name.ToUpper() == "TPCONTAB")
                {
                    ogrid = comum.oDetalhe2;
                    ArmePadrao = comum.EdtDetalhe2;
                }

            }

            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                ArmePadrao = comum.EdtMestre;
                ogrid = comum.oMestre;
                dgvMestre.Focus();
            }
            ArmePadrao.Edite(this, "");
            ogrid.FuncaoSoma();
            colocaSaldo();
            return;
        }

        private void tcDetalhes_Enter(object sender, EventArgs e)
        {
           TabControl otab = (sender as TabControl);
            if (otab.SelectedTab.Name.ToUpper() == "TPPAGAR")
            {
                if (otab.ContainsFocus) dgvPagar.Focus();
                
              
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCENTROS")
            {
                if (otab.ContainsFocus)  dgvCentros.Focus();
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPESTOQUE")
            {
                if (otab.ContainsFocus) dgvEstoque.Focus();
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPVENDAS")
            {
                if (otab.ContainsFocus) dgvVendas.Focus();
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCONTAB")
            {
                if (otab.ContainsFocus) dgvAprop.Focus();
            }
        }

        private void colocaSaldo()
        {
            decimal mestre = 0;
            if (comum.oMestre.dictCampoTotal.ContainsKey("VALOR"))
                mestre = Convert.ToDecimal(comum.oMestre.dictCampoTotal["VALOR"]);
            toolTotalMestre.Text = String.Format("Total: {0:#,###,##0.00}", mestre);
            colocaSaldoDetalhe();
           
        }

        public void colocaSaldoDetalhe()
        {
            MonteGrid MonteDetalhe = null;
            if (tcDetalhes.SelectedTab.Name.ToUpper() == "TPPAGAR")
            {
                MonteDetalhe = comum.oDetalhe1;
            }
            else if (tcDetalhes.SelectedTab.Name.ToUpper() == "TPCENTROS")
            {
                MonteDetalhe = comum.oDetalheCentro;
            }
            else if (tcDetalhes.SelectedTab.Name.ToUpper() == "TPESTOQUE")
            {
                MonteDetalhe = comum.oDetalheEst;
            }
            else if (tcDetalhes.SelectedTab.Name.ToUpper() == "TPVENDAS")
            {
                MonteDetalhe = comum.oVenda;
            }
            else if (tcDetalhes.SelectedTab.Name.ToUpper() == "TPCONTAB")
            {
                MonteDetalhe = comum.oDetalhe2;
            }
            //toolEntradas.Visible = true;
            decimal detalhe = 0;
            if ((MonteDetalhe == null) || (MonteDetalhe.dictCampoTotal == null))
                detalhe = 0;
            else
               if (MonteDetalhe.dictCampoTotal.ContainsKey("VALOR"))
                detalhe = Convert.ToDecimal(MonteDetalhe.dictCampoTotal["VALOR"]);
            
            toolTotalDetalhe.Text = String.Format("Total: {0:#,###,##0.00}", detalhe);
            decimal vlrMestre = 0;
            try
            {
                DataRowView registro = (evt_Fin.bmMestre.Current as DataRowView);
                if (registro != null)
                {
                    vlrMestre = Convert.ToDecimal(registro["VALOR"]);
                }
            
            } catch (Exception){}
            toolTotalDetalhe.ForeColor = (detalhe != vlrMestre)?Color.Red:Color.Black ;
        }

        private void color_Enter(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.ActiveCaption;
            if (((sender as DataGridView).Name.ToUpper() != "DGVMESTRE") && (evt_Fin.bmMestre.Count == 0))
            {
                toolEdite.Enabled = false;
                toolNovo.Enabled = false;
                toolDelete.Enabled = false;
            }
            else
            {
                toolEdite.Enabled = true;
                toolNovo.Enabled = true;
                toolDelete.Enabled = true;
            }
        }

        private void color_Leave(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.Control;
            toolEdite.Enabled = false;
            toolNovo.Enabled = false;
            toolDelete.Enabled = true;
        }

        private void toolDelete_Click(object sender, EventArgs e)
        {
            if (evt_Fin.bmMestre.Current == null) return;

            ArmeEdicao ArmePadrao = null;
            MonteGrid ogrid = null;


            if (dgvMestre.Focused)
            {
                ArmePadrao = comum.EdtMestre;
                ogrid = comum.oMestre;
            }
            else if (dgvPagar.Focused)
            {
                ArmePadrao = comum.EdtDetalhe1;
                ogrid = comum.oDetalhe1;
            }
            else if (dgvCentros.Focused)
            {
                ArmePadrao = comum.EdtDetalheCentro;
                ogrid = comum.oDetalheCentro;
            }
            else if (dgvEstoque.Focused)
            {
                ArmePadrao = comum.EdtDetalheEst;
                ogrid = comum.oDetalheEst;
            }
            else if (dgvVendas.Focused)
            {
                ArmePadrao = comum.EdtVenda;
                ogrid = comum.oVenda;
            }
            else if (dgvAprop.Focused)
            {
                ArmePadrao = comum.EdtDetalhe2;
                ogrid = comum.oDetalhe2;
            }

            // se não houver nenhum datagrid em focus, coloca o Mestre em Foco
            if (ArmePadrao == null)
            {
                return;
            }
            // ArmePadrao.Edite(this, "");
            // ogrid.FuncaoSoma();
            //colocaSaldo();

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
                           // ArmePadrao.OnAfterDeleta(new AlteraRegistroEventArgs(new DataRow[] { null }, DataRowState.Deleted));
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
        }



        /*private void tcDetalhes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (evt_Fin == null) return;
            TabControl otab = (sender as TabControl);
            if (otab.SelectedTab.Name.ToUpper() == "TPPAGAR")
            {
                bindingNavDetalhe.BindingSource = evt_Fin.bmDetalhe1;
                toolDetalheTitulo.Text = tpPagar.Text;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCENTROS")
            {
                bindingNavDetalhe.BindingSource = evt_Fin.bmDetalheCentro;
                toolDetalheTitulo.Text = tpCentros.Text;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPESTOQUE")
            {
                bindingNavDetalhe.BindingSource = evt_Fin.bmEstoque;
                toolDetalheTitulo.Text = tpEstoque.Text;
            }

            else if (otab.SelectedTab.Name.ToUpper() == "TPVENDAS")
            {
                bindingNavDetalhe.BindingSource = evt_Fin.bmVendas;
                toolDetalheTitulo.Text = tpVendas.Text;
            }
            else if (otab.SelectedTab.Name.ToUpper() == "TPCONTAB")
            {
                bindingNavDetalhe.BindingSource = evt_Fin.bmDetalhe2;
                toolDetalheTitulo.Text = tpContab.Text;
            }
            toolComboPaginas.SelectedItem = toolDetalheTitulo.Text;
        }*/
    }
}



