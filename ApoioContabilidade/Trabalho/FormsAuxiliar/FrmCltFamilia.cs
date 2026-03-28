using ApoioContabilidade.Models;
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

namespace ApoioContabilidade.Trabalho.FormsAuxiliar
{
    public partial class FrmCltFamilia : Form
    {
        public DateTime dataReferencia;
        private MonteGrid oMonte;
        private ArmeEdicao oEdite;
        BindingSource bmSource;
        private Eventos_Cad eventos_Cad;
        DataRowView rowMestre = null;
        DateTime limiteData;
        public FrmCltFamilia(Eventos_Cad oeventos_Cad)
        {
            eventos_Cad = oeventos_Cad;
            InitializeComponent();
            bmSource = new BindingSource();
            limiteData = oeventos_Cad.dataRelatorio.AddYears(-14);
            //eventos_Cad.dataRelatorio;
        }
        private void FrmCltFamilia_Load(object sender, EventArgs e)
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
                    bmSource.DataSource = eventos_Cad.tabCltFilhos.AsEnumerable().
                        Where(row => row.Field<string>("CODCAD").Trim() ==
                       rowMestre["CODCAD"].ToString().Trim()).AsDataView();
                }
                catch (Exception)
                {
                    bmSource.DataSource = eventos_Cad.tabCltFilhos.AsEnumerable().
                       Where(row => row.IsNull("ID")).AsDataView();
                }
                oMonte.oDataGridView = dgvFamilia;
                oMonte.oDataGridView.DataSource = bmSource;
                oMonte.ConfigureDBGridView();
                MonteEdit();
                eventos_Cad.tabCltFilhos.TableNewRow += tabCltFilhos_TableNewRow;

                oMonte.oDataGridView.CellFormatting += ODataGridView_CellFormatting;
            }
        }



        private void FrmCltFamilia_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (rowMestre == null) return;
            if (bmSource == null) return;
            int ndeps = Convert.ToInt32(rowMestre.Row.IsNull("DEPEND") ? 0 : rowMestre["DEPEND"]);
            int ndepsAtual = 0;
            foreach (DataRowView orow in (bmSource.DataSource as DataView))
            {
                if (orow.Row.IsNull("NASC")) continue;
                if (Convert.ToDateTime(orow["NASC"]).ToString("yyyyMM").CompareTo(limiteData.ToString("yyyMM")) >= 0)
                {
                    ndepsAtual++;
                }
            }
            if (ndeps != ndepsAtual)
            {
                try
                {
                    atualize_Servidor(ndepsAtual);
                    rowMestre.BeginEdit();
                    rowMestre["DEPEND"] = ndepsAtual;
                    rowMestre.EndEdit();
                    rowMestre.Row.AcceptChanges();
                }
                catch (Exception)
                {
                    MessageBox.Show("Erro ao Atualizar Servidor");


                }
            }
        }

        private async void atualize_Servidor(int ndeps)
        {
            // tipo 0 == alteracao
            // tipo 1 == exclusao
            // tipo 9 == inclusao
            string str = "UPDATE CLTCAD SET DEPEND = "+ndeps.ToString() +" WHERE ID = " + rowMestre.Row["ID"].ToString();
            int ok = await ApiServices.PostApi(str, 0);
        }

        private void ODataGridView_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (((DataGridView)sender).Columns[e.ColumnIndex].Name.Equals("NASC"))
            {
                try
                {
                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
                    DateTime Value = Convert.ToDateTime(e.Value);
                   // if (orow.Row.IsNull("NASC")) continue;
                    if ((Value != null) && (Value.ToString("yyyyMM").CompareTo(limiteData.ToString("yyyMM")) >= 0))
                    {
                        row.DefaultCellStyle.BackColor = Color.LightSeaGreen;
                    }
                    else
                    {
                        row.DefaultCellStyle.BackColor = Color.White;

                    }
                }
                catch (Exception)
                {
                    DataGridViewRow row = ((DataGridView)sender).Rows[e.RowIndex];
                    row.DefaultCellStyle.BackColor = Color.White;

                    return;
                }

                //  row.DefaultCellStyle.BackColor = Color.LightYellow;



            }
        }

        private void MonteGrid()
        {
            oMonte = new MonteGrid();
            oMonte.AddValores("NOME", "Nome Filho(a)", 35, "", false, 0, "");
            oMonte.AddValores("NASC", "Dta Nasc", 10, "", false, 0, "");
            oMonte.AddValores("DATA_CAD", "Dta Cadastro", 10, "", false, 0, "");
        }
        private void MonteEdit()
        {
            oEdite = new ArmeEdicao(bmSource);
            oEdite.Clear();
            Linha olinha = new Linha("Linha 1");
            
            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[0], new TextBox());
            olinha.oedite[0].Validating += NaoPodeSerNULL_Validating;
            olinha.oedite[0].Validated += Edita_Cad_Validated;

            oEdite.Add(olinha);
            olinha = new Linha("Linha 2");
            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[1], new MaskedTextBox());
            olinha.oedite[0].Validating += Nascimento_Validating;
            olinha.oedite[0].Validated += Edita_Cad_Validated;

            olinha.TextoConfigure(bmSource, oMonte.oDataGridView.Columns[2], new MaskedTextBox());
            olinha.oedite[1].Validating += Cadastro_Validating;
            olinha.oedite[1].Validated += Edita_Cad_Validated;
            oEdite.Add(olinha);



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
        }

        private void tsEdita_Click(object sender, EventArgs e)
        {
            Editar_Novo_Mestre(false);
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

        }

        private void tsSair_Click(object sender, EventArgs e)
        {
            this.Close();
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
                if (data.CompareTo(DateTime.Now.AddYears(-30)) < 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Mais de 30 anos?");
                    return;
                }


            }
            catch (Exception)
            {
                e.Cancel = true;
                Error_Set(sender as Control);

            }
        }
        private void Cadastro_Validating(object sender, System.ComponentModel.CancelEventArgs e)
        {
            string texto = (sender as MaskedTextBox).Text;
            //  if (texto == "  /  /") return;
            DateTime data = new DateTime();
            try
            {
                data = Convert.ToDateTime(texto);

                if (data.CompareTo(DateTime.Now.AddYears(-50)) < 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Mais de 50 anos?");
                    return;
                }
                BindingSource dado = ((sender as MaskedTextBox).DataBindings[0].DataSource as BindingSource);
                DataRow orow = (dado.Current as DataRowView).Row;
                DateTime nasc = Convert.ToDateTime(orow["NASC"]);
                if (nasc.CompareTo(data) > 0)
                {
                    e.Cancel = true;
                    Error_Set(sender as Control, "Nasc > Cadastro?");
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


        public void tabCltFilhos_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            e.Row["CODCAD"] = rowMestre["CODCAD"];
            e.Row["NOME"] = "";
           
        }

        public async void Cadastro__AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            // abril 2021
            if (
                (orow["NOME"].ToString().Trim() == "")
               || (orow["CODCAD"].ToString().Trim() == "")
               || (orow.IsNull("DATA_CAD"))
               || (orow.IsNull("NASC"))
               || (Convert.ToDateTime(orow["DATA_CAD"]).CompareTo(Convert.ToDateTime(orow["NASC"])) < 0)
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
          
            ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "CLTFILHO"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Inserir Registro CLTFILHO"); return;
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
                (orow["NOME"].ToString().Trim() == "")
               || (orow["CODCAD"].ToString().Trim() == "")
               || (orow.IsNull("DATA_CAD"))
               || (orow.IsNull("NASC"))
               || (Convert.ToDateTime(orow["DATA_CAD"]).CompareTo(Convert.ToDateTime(orow["NASC"])) < 0)
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
            string tabela = "CLTFILHO"; // orow.Table.TableName.ToUpper().Trim().Substring(1);
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

          }
}
