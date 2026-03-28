using ApoioContabilidade.Almoxarifado.ServicesAlmox;
using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
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

namespace ApoioContabilidade.Almoxarifado.ConfigComponentes
{
    public class EditaAlmox
    {
        public MonteGrid oMestre;
        public MonteGrid oDetalhe;

        public MonteGrid oSugere;


        public ArmeEdicao EdtMestre;
        public ArmeEdicao EdtDetalhe;


        public BindingSource bmArmazem = new BindingSource();
        //public ArmeEdicao EdtTrabMov;
        //public ArmeEdicao EdtTrabNoturno;

        ServAlmox servAlmox;

        public BindingSource bmItensMateriais = new BindingSource();


        public NumericTextBox txtSaldoMaterial;

        public EditaAlmox(ServAlmox oservAlmox)
        {
            servAlmox = oservAlmox;
        }

        public void MonteGrids()
        {
            oMestre = new MonteGrid();
            oMestre.Clear();
            oMestre.AddValores("SETOR", "Setor", 6, "", false, 0, "");
            //oMestre.AddValores("MODEL", "Modelo", 17, "", false, 0, "");
            oMestre.AddValores("SERV", "Serviço", 30, "", false, 0, "");

            oDetalhe = new MonteGrid();
            oDetalhe.Clear();
            oDetalhe.AddValores("FALSO_INC", " * ", 3, "", false, 0, "");
            oDetalhe.AddValores("DATA", "Data", 10, "", false, 0, "");
            oDetalhe.AddValores("NUM_MOD", "Modelo", 25, "", false, 0, "");
            oDetalhe.AddValores("GLEBA", "Gleba", 8, "", false, 0, "");
            oDetalhe.AddValores("QUADRA", "Quadra", 8, "", false, 0, "");
            oDetalhe.AddValores("DEPOSITO", "Deposito", 8, "", false, 0, "");
            oDetalhe.AddValores("COD", "Insumos", 30, "", false, 0, "");
            oDetalhe.AddValores("UNID_COD", "Unidade", 8, "", false, 0, "");
            oDetalhe.AddValores("QUANT", "Quant.", 12, "###,##0.0000", false, 0, "");


            oSugere = new MonteGrid();
            oSugere.Clear();
            oSugere.AddValores("CHECKED", "Check", 6, "", false, 0, "");
            oSugere.AddValores("DESCRICAO", "Insumos", 35, "", false, 0, "");
            oSugere.AddValores("DEPOSITO", "Dep", 5, "", false, 0, "");
            oSugere.AddValores("SDO", "Quant.", 12, "###,##0.0000", false, 0, "");
            oSugere.AddValores("UNID", "Unidade", 8, "", false, 0, "");

           // tabSugere.Columns.Add("OCORRENCIAS", Type.GetType("System.Int32"));


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
                // Configura o BOTÃO INCLUIR NOVO PARA NÃO FUNCIONAR
                try
                {
                    EdtDetalhe.IncluiAutomatico = false;

                    ToolStripItem[] novo = EdtDetalhe.Navegador.Items.Find("Novo", false);
                    if (novo.Length == 1)
                    {
                        bool enable = novo[0].Enabled;
                        novo[0].Enabled = false;
                        novo[0].Visible = false;
                    }
                }
                catch (Exception)
                {
                }


                result = true;
            }
            catch (Exception)
            {
            }
            return result;
        }


         const int _DATA = 1;
         const int _NUM_MOD = 2;
         const int _GLEBA = 3;
         const int _QUADRA = 4;

         const int _DEPOSITO = 5;
        const int _COD = 6; // insumo
        const int _UNID_COD = 7; //
        const int _QUANT = 8;





        private void MonteEditeDetalhe(BindingSource bmSource)
        {
            try
            {
                // BindingSource bmBancos = new BindingSource();
                // bmBancos.DataSource = DadosComum.BancosCombo.Copy().AsDataView();

              
                EdtDetalhe.Clear();

                Linha olinha = new Linha("Linha 1");
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[_NUM_MOD], new TextBox());

                ComboMDInverte2 comboset = new ComboMDInverte2();
                comboset.ConfigureComboBoxFilhoNeto(olinha,
                       bmSource,
                       oDetalhe.oDataGridView.Columns[_GLEBA].DataPropertyName,
                       oDetalhe.oDataGridView.Columns[_QUADRA].DataPropertyName,
                       DadosComum.GlebasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                              && row.Field<string>("GLEBA").Trim() != ""
                          ).CopyToDataTable().AsDataView(),
                       DadosComum.QuadrasCombo.AsEnumerable().Where(row => row.Field<string>("SETOR").Trim() != ""
                       && row.Field<string>("GLEBA").Trim() != ""
                        && row.Field<string>("QUADRA").Trim() != ""
                    ).CopyToDataTable().AsDataView());
                EdtDetalhe.Add(olinha);

                olinha = new Linha("Linha 2");
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[_DATA], new MaskedTextBox());
                EdtDetalhe.Add(olinha);

                olinha = new Linha("Linha 3");

                // DEPOSITO
                BindingSource bmCadest = new BindingSource();
                bmCadest.DataSource = DadosComum.CadestCombo.Copy().AsDataView();

                
                bmArmazem.DataSource = DadosComum.ArmazemCombo.Copy().AsDataView();

                ComboBox ocombox = new ComboBox();

                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT
                ocombox.DataSource = bmArmazem;  // 
                bmArmazem.Sort = "NOME_DEP ASC";
                ocombox.DisplayMember = "NOME_DEP";
                ocombox.ValueMember = "DEPOSITO";
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[_DEPOSITO], ocombox);
                olinha.oedite[0].Width = 200;
                ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;


                //((ComboBox)olinha.oedite[1]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                //((ComboBox)olinha.oedite[1]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);




                ocombox = new ComboBox();
                ocombox.Tag = (object)"M";// maiusculas e comportamento co combo com bind is TEXT

                

                bmItensMateriais.DataSource = servAlmox.tabSaldoDeposito.AsDataView();
                ocombox.DataSource = bmItensMateriais; // Cadest


                ocombox.DisplayMember = "DESCRICAO";
                ocombox.ValueMember = "COD";
                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[_COD], ocombox);
                olinha.oedite[1].Width = 250;
                ((ComboBox)olinha.oedite[1]).MaxDropDownItems = 7;
                ((ComboBox)olinha.oedite[1]).DropDownStyle = ComboBoxStyle.DropDown;
                ((ComboBox)olinha.oedite[1]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                ((ComboBox)olinha.oedite[1]).AutoCompleteSource = AutoCompleteSource.ListItems;
                ((ComboBox)olinha.oedite[1]).Enter += EditaAlmox_Enter;
                ((ComboBox)olinha.oedite[1]).SelectedIndexChanged += EditaAlmox_SelectedIndexChanged;

                // ((ComboBox)olinha.oedite[0]).Validating += new CancelEventHandler(FrmFinan_ComboValidatingNaoPermiteVazio);
                // ((ComboBox)olinha.oedite[0]).Validated += new EventHandler(FrmFinan_CampoValidadoNaoPermiteVazio);

                EdtDetalhe.Add(olinha);

            
                txtSaldoMaterial = new NumericTextBox();
                txtSaldoMaterial.Name = "SaldoMaterial";
                olinha = new Linha("Linha 4");
                olinha.oedite[0] = txtSaldoMaterial;
                EdtDetalhe.Add(olinha);

                olinha = new Linha("Linha 5");

                olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[_QUANT], new NumericTextBox());
                olinha.oedite[0].Validating += EditaAlmox_Validating;

                //olinha.TextoConfigure(bmSource, oDetalhe.oDataGridView.Columns[_COD], new TextBox());



                EdtDetalhe.Add(olinha);

              
            }
            catch (Exception E)
            {

                throw;
            }
        }

        private void EditaAlmox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string tcod = "";
            ComboBox combo = (sender as ComboBox);
            if (combo.SelectedIndex != -1)
            {
                tcod = combo.SelectedValue.ToString();
            }
            servAlmox.DisplaySaldoItens(servAlmox.bmDetalhe, tcod);
        }

        private void EditaAlmox_SelectedValueChanged(object sender, EventArgs e)
        {
            
            
        }

        
        public void EditaAlmox_Validating(object sender, CancelEventArgs e)
        {
            servAlmox.Valide_Valor(sender, e);
          /*  if (servAlmox.frmSaida.ckSdoQuant.Checked) return;
            DataRowView orow = (servAlmox.bmDetalhe.Current as DataRowView);
            string tdeposito = orow["DEPOSITO"].ToString();
            string tcod = orow["COD"].ToString();
            double quant_ant = 0;
            if (!((orow.Row.RowState == DataRowState.Detached) || (orow.Row.RowState == DataRowState.Added)))
            {
                // se for uma edição (não é um novo)
                string depOrigem = orow.Row["DEPOSITO", DataRowVersion.Original].ToString();
                string codOrigem = orow.Row["COD", DataRowVersion.Original].ToString();
                if ((depOrigem == tdeposito) && (codOrigem == tcod))
                    quant_ant = Convert.ToDouble(orow.Row["QUANT", DataRowVersion.Original]);
           }
            string str = "SELECT movest.deposito, movest.cod ," +
         " SUM(iif((tipo='E'),movest.quant,movest.quant*-1)) AS totquant "
         + " FROM movest  where  " + " (deposito = '" + tdeposito + "') and "
         + " (Cod = '" + tcod + "') and "
           + " movest.DATA >= CTOD('" + servAlmox.dataRef.ToString("MM/dd/yyyy") + "') "
        + " AND movest.DATA <= CTOD('" + servAlmox.UltimoPonto.ToString("MM/dd/yyyy") + "') " +
         "GROUP BY deposito, movest.cod";
            List<string> lstStr = new List<string>();
            lstStr.Add(str);
           DataSet dsDados = await ApiServices.Api_QueryMulti(lstStr);

         
            if ((dsDados == null) || (dsDados.Tables.Count == 0)) return;
            double valor = Convert.ToDouble((sender as NumericTextBox).DecimalValue);
            if (valor == 0) return;


           if (dsDados.Tables[0].Rows.Count > 0)
            {
                DataRow rowResult = dsDados.Tables[0].Rows[0];
                if (valor > (Convert.ToDouble(rowResult["TOTQUANT"]) + quant_ant))
                {
                    if (MessageBox.Show("Saldo Insuficiente:" + String.Format("{0:####,##0.0000}", Convert.ToDecimal(rowResult["TOTQUANT"])),
                             " Confima mesmo assim?", MessageBoxButtons.YesNo) == DialogResult.No)
                    {
                        e.Cancel = true;
                    }
                }
            }*/
        }

        private void EditaAlmox_Enter(object sender, EventArgs e)
        {
            string deposito = "";
            try
            {
                Binding bind = ((ComboBox)sender).DataBindings[0];
                deposito = ((DataRowView)bind.BindingManagerBase.Current)["DEPOSITO"].ToString();

            }
            catch (Exception) {           
            }
            if (deposito == "") return;
            bmItensMateriais.DataSource = servAlmox.tabSaldoDeposito.AsEnumerable().
                           Where(row => row.Field<string>("DEPOSITO").Trim() == deposito.Trim()).AsDataView();
            ((ComboBox)sender).DataBindings[0].ReadValue();
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

    }
}
