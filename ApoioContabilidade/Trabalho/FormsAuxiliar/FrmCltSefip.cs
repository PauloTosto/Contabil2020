using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ClassFiltroEdite;


namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    public partial class FrmCltSefip : Form
    {
        private MonteGrid oMonte;
        private ArmeEdicao oEdite;
        BindingSource bmSource;
        private Eventos_Cad eventos_Cad;
        DataRowView rowMestre = null;
        //   DateTime limiteData;

        public FrmCltSefip(Eventos_Cad oeventos_Cad)
        {
            eventos_Cad = oeventos_Cad;
            InitializeComponent();
            bmSource = new BindingSource();
            //  limiteData = oeventos_Cad.dataRelatorio.AddYears(-14);
            //eventos_Cad.dataRelatorio;
        }
        private void FrmCltReajuste_Load(object sender, EventArgs e)
        {
            rowMestre = (eventos_Cad.bmMestre.Current as DataRowView);
            if (rowMestre == null)
            {
                MessageBox.Show("Trabalhador não Existe. Aponte para um!!");
                this.Close();
            }
            else
            {
                tsNome.Text = rowMestre["NOMECAD"].ToString();

                MonteGrid();
                try
                {
                    bmSource.DataSource = eventos_Cad.tabObsSefip.AsEnumerable().
                        Where(row => row.Field<string>("COD").Trim() ==
                       rowMestre["CODCAD"].ToString().Trim()).AsDataView();
                }
                catch (Exception)
                {
                    bmSource.DataSource = eventos_Cad.tabObsSefip.AsEnumerable().
                       Where(row => row.IsNull("ID")).AsDataView();
                }
                oMonte.oDataGridView = dgvSefip;
                oMonte.oDataGridView.DataSource = bmSource;
                oMonte.ConfigureDBGridView();


                MonteEdit();
                eventos_Cad.tabObsSefip.TableNewRow += tabObsSefip_TableNewRow;

                oMonte.oDataGridView.CellFormatting += ODataGridView_CellFormatting;
               // PonhaSalarioAtual(eventos_Cad.dataRelatorio);
            }
        }



        private void FrmCltFamilia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rowMestre == null) return;
            if (bmSource == null) return;

        }



        private void ODataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("TPSEFIP"))
            {
                String stringValue = e.Value as string;
                if ((stringValue == null) || (stringValue.Trim() == "")) return;
                DataGridViewCell cell = ((DataGridView)sender)[e.ColumnIndex, e.RowIndex];
                cell.ToolTipText = stringValue;
                DataRow orow = DadosComum.TabSefipCombo.AsEnumerable().Where(r => r.Field<string>("COD").Trim() == stringValue.Trim()).FirstOrDefault();
                if (orow != null)
                {
                    e.Value = orow["DESCRICAO"].ToString();
                }
            }
            /*if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("DATA_SAI"))
            {
                string data= "";
                try
                {
                    data = Convert.ToDateTime(e.Value).ToString("d");
                }
                catch (Exception)
                {

                    
                }
                e.Value = data;
             
            }*/

        }

        private void MonteGrid()
        {
            oMonte = new MonteGrid();
            oMonte.AddValores("DATA", "Data", 10, "", false, 0, "");
            oMonte.AddValores("TPSEFIP", "Tipo Sefip/Obs", 70, "", false, 0, "");
            oMonte.AddValores("ENTSAISEFIP", "E/S", 5, "", false, 0, "");

            oMonte.AddValores("DATA_SAI", "Inicio Benefic.", 10, "", false, 0, "");
            oMonte.AddValores("NUM_PREV", "Num.Beneficio", 10, "", false, 0, "");
            oMonte.AddValores("OBS", "Comentários", 60, "", false, 0, "");
          
            /*
    
             */
        }
        private void MonteEdit()
        {
            BindingSource bmTabSefip = new BindingSource();
            bmTabSefip.DataSource = DadosComum.TabSefipCombo.Copy().AsDataView();

            oEdite = new ArmeEdicao(bmSource);
            oEdite.Clear();
            Linha olinha = new Linha("Linha 1");

            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[0], new MaskedTextBox());
            olinha.oedite[0].Validating += Data_Validating;
            olinha.oedite[0].Validated += Edita_Cad_Validated;

            oEdite.Add(olinha);

            olinha = new Linha("Linha 2");
            ComboBox ocombox = new ComboBox();
            ocombox.Tag = (object)"M";//
            ocombox.DataSource = bmTabSefip;
            ocombox.DisplayMember = "DESCRICAO";
            ocombox.ValueMember = "COD";
            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[1], ocombox);
            olinha.oedite[0].Width = 400;
            ((ComboBox)olinha.oedite[0]).MaxDropDownItems = 7;
            ((ComboBox)olinha.oedite[0]).DropDownStyle = ComboBoxStyle.DropDown;
            ((ComboBox)olinha.oedite[0]).AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            ((ComboBox)olinha.oedite[0]).AutoCompleteSource = AutoCompleteSource.ListItems;

            

            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[3], new MaskedTextBox());
            //olinha.oedite[1].Validating += Data_Validating;
            //olinha.oedite[1].Validated += Edita_Cad_Validated;


            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[4], new TextBox());
            oEdite.Add(olinha);

            olinha = new Linha("Linha 3");
            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[5], new TextBox());
            oEdite.Add(olinha);


            /*            with EditeDetalhe do
                            begin
                                clear;
                 Add('');
                        Cabecalho[Count - 1, 0] := DBGrid1.Columns[0].Title.caption;
                        OEdite[Count - 1, 0] := CrieDBEDit(DBGrid1.Columns[0]);



                        Add('');

                        Cabecalho[Count - 1, 0] := DBGrid1.Columns[1].Title.caption;
                        OEdite[Count - 1, 0] := DBCBSEFIP;
                        Add('');

                        Cabecalho[Count - 1, 0] := DBGrid1.Columns[3].Title.caption;
                        OEdite[Count - 1, 0] := CrieDBEDit(DBGrid1.Columns[3]);


                        Cabecalho[Count - 1, 1] := DBGrid1.Columns[4].Title.caption;
                        OEdite[Count - 1, 1] := CrieDBEDit(DBGrid1.Columns[4]);

                        Add('');
                        Cabecalho[Count - 1, 0] := DBGrid1.Columns[5].Title.caption;
                        OEdite[Count - 1, 0] := CrieDBEDit(DBGrid1.Columns[5]);

                        */




            // Configurações para o Servidor
            oEdite.AlteraRegistrosOk += Cadastro__AlteraRegistrosOk;
            oEdite.BeforeAlteraRegistros += Cadastro_BeforeAlteraRegistros;
            oEdite.DeletaRegistrosOk += Padrao_DeletaRegistrosOk;

            oEdite.MonteEdicao();

        }
        private void Editar_Novo_Mestre(bool novo)
        {
            ArmeEdicao ArmePadrao = null;
            // MonteGrid ogrid = .oMestre;

            ArmePadrao = oEdite;

            ArmePadrao.Edite(this, "", novo);
        }

        private void tsNovo_Click(object sender, EventArgs e)
        {
            Editar_Novo_Mestre(true);
            //PonhaSalarioAtual(eventos_Cad.dataRelatorio);
        }

        private void tsEdita_Click(object sender, EventArgs e)
        {
            Editar_Novo_Mestre(false);
           // PonhaSalarioAtual(eventos_Cad.dataRelatorio);
        }

        private void tsDeleta_Click(object sender, EventArgs e)
        {
            try
            {
                ToolStripItem[] delete = oEdite.Navegador.Items.Find("NovoDelete", false);
                if (delete.Length == 1)
                {
                    bool enable = delete[0].Enabled;
                    delete[0].Enabled = true;
                    delete[0].PerformClick();
                    delete[0].Enabled = enable;
                }

            }
            catch (Exception)
            {
            }
            //PonhaSalarioAtual(eventos_Cad.dataRelatorio);
        }

        private void tsSair_Click(object sender, EventArgs e)
        {
            this.Close();
        }





        /*   private void NaoPodeSerNULL_Validating(object sender, System.ComponentModel.CancelEventArgs e)
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
        */
        private void Data_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
            DateTime data = new DateTime();
            try
            {
                data = Convert.ToDateTime(texto);
                if (data.CompareTo(DateTime.Now.AddYears(-30)) < 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Mais de 30 anos Atrás?");
                    return;
                }


            }
            catch (Exception)
            {
                e.Cancel = true;
                Error_Set(sender as Control);

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


        public void tabObsSefip_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["COD"] = rowMestre["CODCAD"];
            e.Row["DATA"] = eventos_Cad.dataRelatorio;

            foreach (DataColumn ocol in e.Row.Table.Columns)
            {
                if ((ocol.DataType == Type.GetType("System.Double"))
                    || (ocol.DataType == Type.GetType("System.Decimal"))
                    || (ocol.DataType == Type.GetType("System.Int32"))
                    || (ocol.DataType == Type.GetType("System.Int64"))
                    || (ocol.DataType == Type.GetType("System.Int16")))
                {
                    e.Row[ocol.ColumnName] = 0;
                }
            }

        }

        public async void Cadastro__AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            // abril 2021
            if (
                 (orow["COD"].ToString().Trim() == "")
               || (orow.IsNull("DATA"))

               )
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            foreach (DataColumn col in orow.Table.Columns)
            {
                if (orow[col.ColumnName] is System.DBNull)
                {
                    orow[col.ColumnName] = col.DefaultValue;
                }
            }
            bool ok = false;
            //cltcad.ult_atual, cltcad.salbase
           /* List<string> ExcluaCampos = new List<string>();
            ExcluaCampos.Add("ULT_ATUAL");
            ExcluaCampos.Add("SALBASE");*/
            ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "OBSSEFIP"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Inserir Registro OBSSEFIP"); return;
            }
            try
            {

                orow.AcceptChanges();

            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                // throw;
                return;
            }
            return;
        }

        public void Cadastro_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if (
                (orow["COD"].ToString().Trim() == "")
              || (orow.IsNull("DATA"))

              )

            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }

            // Outras Inconsistencias que se Fizerem necessárias testar
            try { }
            catch (Exception)
            {
                MessageBox.Show("ERRO");
                e.Cancela = true;
                return;
            }
        }


        public async void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];

            bool ok = false;
            string tabela = "OBSSEFIP"; // orow.Table.TableName.ToUpper().Trim().Substring(1);
            try
            {
                ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, tabela);

            }
            catch (Exception)
            {
            }
            // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Deletar Registro " + tabela); return;
            }
        }

       /* private void PonhaSalarioAtual(DateTime data)
        {
            eventos_Cad.ConstrucaoTabelaReajustesSalariosUnico(data);
            DataRow orow = eventos_Cad.ReajusteVirtual.AsEnumerable().LastOrDefault();
            string valorSalBase = String.Format("{0:#,###,##0.00}", Convert.ToDecimal(orow["VLRSALBASE"]));
            tsValorSalarioBase.Text = valorSalBase;
        }*/

    }
}
