using ApoioContabilidade.Produção.Servicos;
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

namespace ApoioContabilidade.Produção
{
    public partial class FrmProd : Form
    {
        Eventos_Produto evtProduto;
        EdtProduto edtProduto;
        bool recomece = true;

        Dictionary<Int32, string> dictTiposComplementos;

        //  FrmFiltroProd frmFiltro = null;
        public FrmProd()
        {
            InitializeComponent();

            this.KeyPreview = true;
            this.KeyDown += FrmProd_KeyDown;


            dgvLote.CellFormatting += DgvLote_CellFormatting;
            dgvProdRec.CellFormatting += DgvProdRec_CellFormatting;

            dgvDestino.CellFormatting += DgvDestino_CellFormatting;
            dgvSobra.CellFormatting += DgvDestino_CellFormatting;
            //   frmFiltro = new FrmFiltroProd();
            evtProduto = new Eventos_Produto();
            edtProduto = new EdtProduto(evtProduto);
            evtProduto.edtProduto = edtProduto;
            
            evtProduto.ofrmProd = this;

            dgvLote.Enter += color_Enter;
            dgvLote.Leave += color_Leave;

            dgvProdMov.Enter += color_Enter;
            dgvProdMov.Leave += color_Leave;
            dgvProdRec.Enter += color_Enter;
            dgvProdRec.Leave += color_Leave;
            dgvDestino.Enter += color_Enter;
            dgvDestino.Leave += color_Leave;
            dgvSobra.Enter += color_Enter;
            dgvSobra.Leave += color_Leave;
            dgvDestino.Enter += color_Enter;
            dgvDestino.Leave += color_Leave;
            dgvFruta.Enter += color_Enter;
            dgvFruta.Leave += color_Leave;



            dictTiposComplementos = new Dictionary<int, string>();
            dictTiposComplementos.Add(0, "VENDA");
            dictTiposComplementos.Add(1, "COMPLEM.");
            dictTiposComplementos.Add(99, "AJUSTE CARGA");
            dictTiposComplementos.Add(98, "AJUSTE ESTOQUE");
        }

        private void DgvDestino_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("FIRMA"))
            {
                string stringValue = e.Value.ToString();
                try
                {
                    e.Value = TabelasIniciais.Ache_Descricao(stringValue).Trim();
                }
                catch (Exception)
                {
                    e.Value = stringValue;
                }
            }
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("COMPLEM"))
            {
                Int32 stringValue = Convert.ToInt32(e.Value);
                try
                {
                    e.Value = "";
                    if (dictTiposComplementos.ContainsKey(stringValue))
                        e.Value = dictTiposComplementos[stringValue];

                }
                catch (Exception)
                {
                    e.Value = stringValue;
                }
            }

        }

        private void DgvProdRec_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("TIPOPROD"))
            {
                Int32 stringValue = Convert.ToInt32(e.Value);
                try
                {
                    e.Value = TabelasIniciais.TipoProdDescricao(stringValue).Trim();
                }
                catch (Exception)
                {
                    e.Value = stringValue;
                }
            }
        }

        private void DgvLote_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("TIPOPROD"))
            {
                Int32 stringValue = Convert.ToInt32(e.Value);
                try
                {
                    e.Value = TabelasIniciais.TipoProdDescricao(stringValue).Trim();
                }
                catch (Exception)
                {
                    e.Value = stringValue;
                }
            }

        }

        private async void  btnConsulta_Click(object sender, EventArgs e)
        {

            // edtProduto.ConfigureFiltro();
            if (edtProduto.frmFiltro.ShowDialog() == DialogResult.OK)
            {
                bool espera = await evtProduto.Consulte();
                ConfigureInicial();
            }

        }

        private async void FrmProd_Load(object sender, EventArgs e)
        {
           if (recomece) // força
            {

                /*ultimosetor:= '';
                     // LockWindowUpdate(Handle);
                     self.pgGeral.Visible := false;
                     self.GbMestre.Visible := false;
                     self.PnEditar.Visible := false;
                     oEncapsulaProduto.Consulte;
                     Configure_Inicial;
                     self.pgGeral.Visible := true;
                     self.GbMestre.Visible := true;
                     self.PnEditar.Visible := true;
                */
                bool espera = await evtProduto.Consulte();
                ConfigureInicial();
                recomece = false;
            }
   
        // LockWindowUpdate(0);
    
        }

        private void ConfigureInicial()
        {

            edtProduto.MonteGrids();

            edtProduto.oProdLote.oDataGridView = dgvLote;
            edtProduto.oProdLote.oDataGridView.DataSource = evtProduto.bmProdLote;
            edtProduto.oProdLote.ssTotal = ssProdLote;


            edtProduto.oProdMov.oDataGridView = dgvProdMov;
            edtProduto.oProdMov.oDataGridView.DataSource = evtProduto.bmProdMov;
            edtProduto.oProdMov.ssTotal = ssProdMov;


            edtProduto.oProdRec.oDataGridView = dgvProdRec;
            edtProduto.oProdRec.oDataGridView.DataSource = evtProduto.bmProdRec;
            edtProduto.oProdRec.ssTotal = ssProdRec;


           edtProduto.oProdFruta.oDataGridView = dgvFruta;
            edtProduto.oProdFruta.oDataGridView.DataSource = evtProduto.bmFruta;

            edtProduto.oProdDestino.oDataGridView = dgvDestino;
            edtProduto.oProdDestino.oDataGridView.DataSource = evtProduto.bmDestino;
            edtProduto.oProdDestino.ssTotal = ssProdDestino;


            edtProduto.oProdSobra.oDataGridView = dgvSobra;
            edtProduto.oProdSobra.oDataGridView.DataSource = evtProduto.bmSobra;


            edtProduto.oProdLote.ConfigureDBGridView();
            edtProduto.oProdMov.ConfigureDBGridView();
            edtProduto.oProdRec.ConfigureDBGridView();
            edtProduto.oProdFruta.ConfigureDBGridView();
            edtProduto.oProdDestino.ConfigureDBGridView();
            edtProduto.oProdSobra.ConfigureDBGridView();




            /* edtProduto.oSugere.oDataGridView.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
             edtProduto.oSugere.oDataGridView.RowTemplate.Height = 10;
             // edtProduto.oSugere.oDataGridView.


             bool ok = edtProduto.MonteEdtDetalhe(evtProduto.bmDetalhe);



             // para tornar os campo enable é preciso definir o tabstop como false
             edtProduto.EdtDetalhe.Linhas[3].oedite[0].Enabled = false; // sdo estoque
             edtProduto.EdtDetalhe.Linhas[3].oedite[0].TabStop = false;


             edtProduto.EdtDetalhe.Linhas[0].oedite[0].Enabled = false;
             // edtProduto.EdtDetalhe.Linhas[1].oedite[0].Enabled = false;
             edtProduto.EdtDetalhe.Linhas[0].oedite[0].TabStop = false;
             // edtProduto.EdtDetalhe.Linhas[1].oedite[0].TabStop = false;
             // edtProduto.EdtDetalhe.IncluiAutomatico = false;

             */




            /*  evtProduto.tabDetalheVirtual.TableNewRow += evtProduto.TabDetalhe_TableNewRow;
              edtProduto.EdtDetalhe.AlteraRegistrosOk += new EventHandler<AlteraRegistroEventArgs>(evtProduto.EdtDetalhe_AlteraRegistrosOk);
              edtProduto.EdtDetalhe.BeforeAlteraRegistros += new EventHandler<AlteraRegistroEventArgs>(evtProduto.EdtDetalhe_BeforeAlteraRegistros);
              edtProduto.EdtDetalhe.DeletaRegistrosOk += evtProduto.Padrao_DeletaRegistrosOk;





              evtProduto.bmDetalhe.PositionChanged -= evtProduto.BmDetalhe_PositionChanged;
              evtProduto.bmDetalhe.PositionChanged += evtProduto.BmDetalhe_PositionChanged;
            */

            if (evtProduto.bmProdLote.Count == 0)
            {
                evtProduto.bmProdLote.PositionChanged -= evtProduto.BmProdLote_PositionChanged;
                evtProduto.bmProdLote.PositionChanged += evtProduto.BmProdLote_PositionChanged;

                evtProduto.BmProdLote_PositionChanged(evtProduto.bmProdLote, null);
            }
            else
            {
                evtProduto.bmProdLote.PositionChanged -= evtProduto.BmProdLote_PositionChanged;
                evtProduto.bmProdLote.PositionChanged += evtProduto.BmProdLote_PositionChanged;
                evtProduto.bmProdLote.MoveFirst();
                evtProduto.BmProdLote_PositionChanged(evtProduto.bmProdLote, null);
                //bmMestre.ResetCurrentItem();
            }

            edtProduto.oProdLote.FuncaoSoma();

            colocaSaldo();

        }

        private void colocaSaldo()
        {
            StatusStrip ssTotal = ssProdLote;
            DataGridView oDataGrid = edtProduto.oProdLote.oDataGridView; 
            ssTotal.Items.Clear();
            ssTotal.Dock = DockStyle.None;
            ssTotal.SizingGrip = false;
            ssTotal.Width = 0;    //0; //14;
            ssTotal.AutoSize = true;
            ssTotal.Top = pnProdLote.Height + pnProdLote.Top+ 1;
            ssTotal.Left = oDataGrid.Left + oDataGrid.RowHeadersWidth;
            
            foreach (var campos in edtProduto.oProdLote.dictCampoTotal)
            {
                // titulo
                DataGridViewColumn coluna = null;
                if (oDataGrid.Columns.Contains(campos.Key))
                {
                    coluna = oDataGrid.Columns[campos.Key];
                }
                
                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                ssTotal.Items[ssTotal.Items.Count - 1].Text = coluna.HeaderText.Trim() + ":";
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = true;
               // ssTotal.Items[ssTotal.Items.Count - 1].Width = coluna.Width;
                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Left;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleRight;
                //ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;
                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                if (campos.Key == "CONVERSA")
                {
                    decimal natura = 0;
                    if (edtProduto.oProdLote.dictCampoTotal.ContainsKey("NATURA"))
                        natura = Convert.ToDecimal(edtProduto.oProdLote.dictCampoTotal["NATURA"]);
                    decimal benef = 0;
                    if (edtProduto.oProdLote.dictCampoTotal.ContainsKey("BENEF"))
                        benef = Convert.ToDecimal(edtProduto.oProdLote.dictCampoTotal["BENEF"]);
                    if ((benef > 0) && (natura > 0))
                    {
                        ssTotal.Items[ssTotal.Items.Count - 1].Text = String.Format("{0:" + coluna.DefaultCellStyle.Format + "}", Math.Round(benef / natura, 2));
                    }
                    else
                        ssTotal.Items[ssTotal.Items.Count - 1].Text = "";

                }
                else 
                   ssTotal.Items[ssTotal.Items.Count - 1].Text = Convert.ToDecimal(campos.Value).ToString(coluna.DefaultCellStyle.Format).Trim();
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = true;
                  
                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Left;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleLeft;
               // ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;

                ssTotal.Items.Add(new System.Windows.Forms.ToolStripSeparator());
               
                //ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;
            }
            ssTotal.Refresh();
          //  ColocaDiversosTotais();
        }
        
        private void ColocaDiversosTotais()
        {
            colocaSaldoDetalhes(edtProduto.oProdMov);
        }
        public void colocaSaldoDetalhes(MonteGrid oMonte)
        {
            
            StatusStrip ssTotal = oMonte.ssTotal;
            if (ssTotal == null) return;
            DataGridView oDataGrid = oMonte.oDataGridView;
            ssTotal.Items.Clear();
            ssTotal.Dock = DockStyle.None;
         //   ssTotal.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top |
           //     System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Bottom))));


            ssTotal.SizingGrip = false;
            ssTotal.Width = 0;    //0; //14;
            ssTotal.AutoSize = true;
            Control checado = ssTotal.Parent.Controls.OfType<Panel>().
                    FirstOrDefault();
            if (checado != null)
                ssTotal.Top = checado.Height + checado.Top;
            else
                ssTotal.Top = ssTotal.Parent.Height - ssTotal.Height;

            ssTotal.Left = oDataGrid.Left + oDataGrid.RowHeadersWidth;

            foreach (var campos in oMonte.dictCampoTotal)
            {
                // titulo
                DataGridViewColumn coluna = null;
                if (oDataGrid.Columns.Contains(campos.Key))
                {
                    coluna = oDataGrid.Columns[campos.Key];
                }

                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                ssTotal.Items[ssTotal.Items.Count - 1].Text = coluna.HeaderText.Trim() + ":";
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = true;
                // ssTotal.Items[ssTotal.Items.Count - 1].Width = coluna.Width;
                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Left;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleRight;
                //ssTotal.Width = ssTotal.Width + ssTotal.Items[ssTotal.Items.Count - 1].Width;
                ssTotal.Items.Add(new System.Windows.Forms.ToolStripStatusLabel());
                ssTotal.Items[ssTotal.Items.Count - 1].DisplayStyle = ToolStripItemDisplayStyle.Text;
                ssTotal.Items[ssTotal.Items.Count - 1].Text = Convert.ToDecimal(campos.Value).ToString(coluna.DefaultCellStyle.Format).Trim();
                ssTotal.Items[ssTotal.Items.Count - 1].AutoSize = true;

                ssTotal.Items[ssTotal.Items.Count - 1].Alignment = ToolStripItemAlignment.Left;
                ssTotal.Items[ssTotal.Items.Count - 1].TextAlign = ContentAlignment.MiddleLeft;
                ssTotal.Items.Add(new System.Windows.Forms.ToolStripSeparator());
            }
            ssTotal.Refresh();
        }


        private void color_Enter(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.ActiveCaption;
            if (((sender as DataGridView).Name.ToUpper() == "DGVLOTE") 
                || ((sender as DataGridView).Name.ToUpper() == "DGVPRODMOV")
                    || ((sender as DataGridView).Name.ToUpper() == "DGVPRODREC")
                )
            {
                btnNovo.Enabled = true;
               // toolNovo.Enabled = false;
                //toolDelete.Enabled = false;
            }
            else
            {
                btnNovo.Enabled = false;

                //toolEdite.Enabled = true;
                // toolNovo.Enabled = true;
                //  toolDelete.Enabled = true;
            }
        }

        private void color_Leave(object sender, EventArgs e)
        {
            (sender as Control).Parent.BackColor = SystemColors.Control;
            btnNovo.Enabled = false;
           // toolNovo.Enabled = false;
           // toolDelete.Enabled = true;
        }

        private void FrmProd_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control)
            {
                switch (e.KeyCode)
                {
                    case Keys.N:
                        btnNovo.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.C:
                        btnConsulta.PerformClick();
                        e.Handled = true;
                        break;
                    case Keys.R:
                        btnConsulta.PerformClick();
                        e.Handled = true;
                        break;
                }
            }
        }
        protected override bool ProcessDialogKey(Keys keyData)
        {

            if (keyData == (Keys.Alt | Keys.L))
            {
                dgvLote.Focus();
                return true;
            }
            else
            if (keyData == (Keys.Alt | Keys.D1))
            {
                if (tcDetalhes.SelectedIndex == 0)
                    dgvProdMov.Focus();
                else
                    tcDetalhes.SelectTab(0);
                return true;
            }
            else if (keyData == (Keys.Alt | Keys.D2))
            {
                tcDetalhes.SelectTab(1);
                return true;
            }
           /* else if (keyData == (Keys.Alt | Keys.D3))
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
           */
            return base.ProcessDialogKey(keyData);
        }
      
        private void tpProdMov_SizeChanged(object sender, EventArgs e)
        {
            Control ckPn = (sender as Control).Controls.OfType<Panel>().
                  FirstOrDefault();
            Control ckssTrip = (sender as Control).Controls.OfType<StatusStrip>().
                  FirstOrDefault();

            if ((ckssTrip == null) || (ckPn == null)) return;
            

            ckssTrip.Top = ckPn.Height + ckPn.Top;
        }

        private void btnRecalcula_Click(object sender, EventArgs e)
        {
          
        }
    }
}
