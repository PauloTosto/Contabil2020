using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ApoioContabilidade.PagarReceber.ServicesLocais;
using ApoioContabilidade.Produção.Servicos;
using ApoioContabilidade.Services;
using ClassFiltroEdite;
//using ClassFiltroEdite.UserControls;


namespace ApoioContabilidade.Produção
{
   
    public class ObjProduto
    {
        public string cod { get; set; }
        public string natura { get; set; }
        public string unid_bene { get; set; }
        public Single conversao { get; set; }
        public Single parceria { get; set; }
        public Single reinvest { get; set; }
    }
    public class EdtProduto
    {

        public MonteGrid oProdLote;
        public MonteGrid oProdMov;
        public MonteGrid oProdRec;
        public MonteGrid oProdSobra;
        public MonteGrid oProdFruta;
        public MonteGrid oProdDestino;
        public FrmFiltroProd frmFiltro;

        Eventos_Produto evtProduto;
        
        public ObjProduto objProd;

        public Dictionary<string, string> valoresFiltro = new Dictionary<string, string>();

        public EdtProduto(Eventos_Produto oevtProduto)
        {
            // Na criação 

            frmFiltro = new FrmFiltroProd();
            ConfigureFiltro();


            evtProduto = oevtProduto;
        }

        public void ConfigureFiltro()
        {
          
         //   frmFiltro = new FrmFiltroProd(); //      ofrmFiltro;
            ConfigureCombox();
            ValoresFiltro(); // carrega valores iniciais do form filtro
            frmFiltro.btnFiltro.Click += BtnFiltro_Click;
         
        }

        private void BtnFiltro_Click(object sender, EventArgs e)
        {
            ArmePequisaFiltro();
        }

        private void ConfigureCombox()
        {
            //CATALOGO PRODUTO
            
            BindingSource BmCatProduto = new BindingSource();
            BmCatProduto.DataSource = TabelasIniciais.DsTabelasInciais().Tables["CATPROD"].Copy().AsDataView();
            frmFiltro.cbProdutos.DataSource = BmCatProduto;
            BmCatProduto.Sort = "COD";
            frmFiltro.cbProdutos.DisplayMember = "DESCRI";
            //  ocombox.ValueMember = "";
           // frmFiltro.cbProdutos.Width = 200;
            frmFiltro.cbProdutos.MaxDropDownItems = 7;
            frmFiltro.cbProdutos.DropDownStyle = ComboBoxStyle.DropDown;
            frmFiltro.cbProdutos.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            frmFiltro.cbProdutos.AutoCompleteSource = AutoCompleteSource.ListItems;
            // TIPO PRODUTO
            BindingSource BmTipoProd = new BindingSource();
            DataTable tipoProd = TabelasIniciais.DsTabelasInciais().Tables["TIPOPROD"].Copy();
            DataRow orow = tipoProd.NewRow();
            orow["COD"] = -1;
            orow["DESCRICAO"] = "TODOS";
            tipoProd.Rows.Add(orow);
            BmTipoProd.DataSource = tipoProd.AsEnumerable().Where(row=> 
               (row.Field<double>("COD") == -1)
              || (row.Field<double>("COD") == 0)
              || (row.Field<double>("COD") == 1)
              || (row.Field<double>("COD") == 11)
              || (row.Field<double>("COD") == 21)
            ).AsDataView();
            frmFiltro.cbTipoProd.DataSource = BmTipoProd;
            BmTipoProd.Sort = "COD";
            frmFiltro.cbTipoProd.DisplayMember = "DESCRICAO";
            frmFiltro.cbTipoProd.ValueMember = "COD";
           // frmFiltro.cbTipoProd.Width = 200;
            frmFiltro.cbTipoProd.MaxDropDownItems = 3;
            frmFiltro.cbTipoProd.DropDownStyle = ComboBoxStyle.DropDown;
            frmFiltro.cbTipoProd.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            frmFiltro.cbTipoProd.AutoCompleteSource = AutoCompleteSource.ListItems;
            // SAFRA
            frmFiltro.cbSafra.Items.Clear();
            int ano = DateTime.Now.Year;
            int mes = DateTime.Now.Month;
            ano = ano - 15;
            for (int i = 0; i <= 23; i++)
            {
                frmFiltro.cbSafra.Items.Add(ano + i);
            }
            if (mes < 4)
                ano = ano - 1;
            frmFiltro.cbSafra.SelectedIndex =  frmFiltro.cbSafra.Items.IndexOf(ano + 15);

        }
        public void ValoresFiltro()
        {
            valoresFiltro.Clear();
            Control checado = frmFiltro.gbVendas.Controls.OfType<RadioButton>()
                        .FirstOrDefault(n => n.Checked);
            valoresFiltro.Add("VENDAS", checado.Text.ToUpper().Trim());
            valoresFiltro.Add("VENDAS_CONTRATO", frmFiltro.txContratos.Text.Trim());
            checado = frmFiltro.gbArmazenamento.Controls.OfType<RadioButton>()
                        .FirstOrDefault(n => n.Checked);
            valoresFiltro.Add("ARMAZENAMENTO", checado.Text.ToUpper().Trim());
            checado = frmFiltro.gbEstagios.Controls.OfType<RadioButton>()
                        .FirstOrDefault(n => n.Checked);
            valoresFiltro.Add("ESTAGIOS", checado.Text.ToUpper().Trim());
            string safrainicio = frmFiltro.cbSafra.Items[frmFiltro.cbSafra.SelectedIndex].ToString();
            
            valoresFiltro.Add("SAFRAINICIO", safrainicio);
            string safrafim = safrainicio;
            if (frmFiltro.upAnos.Value > 1)
            {
                try
                {
                    safrafim = (Convert.ToInt32(safrafim) + frmFiltro.upAnos.Value).ToString();
                }
                catch (Exception)
                {
                    safrafim = safrainicio;
                }
            }
            valoresFiltro.Add("SAFRAFIM", safrafim);
            DataRowView rowprod = (frmFiltro.cbProdutos.Items[frmFiltro.cbProdutos.SelectedIndex] as DataRowView);
            valoresFiltro.Add("PRODUTO", rowprod["COD"].ToString());
            objProd = new ObjProduto();
            objProd.cod = rowprod["COD"].ToString();
            objProd.unid_bene = rowprod["UNID_BENE"].ToString();
            objProd.natura = rowprod["NATURA"].ToString();
            objProd.reinvest = Convert.ToSingle(rowprod["REINVEST"]);
            objProd.conversao = Convert.ToSingle(rowprod["CONVERSAO"]);



        }
        private void ArmePequisaFiltro()
        {

            frmFiltro.oPesqProd = new Pesquise();//TPesquisa.Create(self);
            frmFiltro.oPesqProd.Parent = frmFiltro;
            frmFiltro.oPesqProd.TabIndex = 0;
            frmFiltro.oPesqProd.Left = 0;
            frmFiltro.oPesqProd.Top = frmFiltro.pnTop.Top + frmFiltro.pnTop.Height + 2;
            frmFiltro.oPesqProd.Width = frmFiltro.pnTop.Width;
            frmFiltro.oPesqProd.Height = (frmFiltro.ClientSize.Height - frmFiltro.oPesqProd.Top);
            //

            if (frmFiltro.oPesqProd.Pagina("Lotes") != null) return;
            ArmeEdicao oArme = ArmeLinhasLote();

            if (oArme == null) return;
            frmFiltro.oPesqProd.Linhas.Clear();
            foreach (Linha olinha in oArme.Linhas)
            {
                frmFiltro.oPesqProd.Linhas.Add(olinha);
            }
            // oPesqFin.Linhas = oArme.Linhas;
            frmFiltro.oPesqProd.NovaPagina("Lotes");
            frmFiltro.oPesqProd.Pagina("Lotes").Text = "1) Lotes";


            frmFiltro.oPesqProd.Linhas.Clear();
            oArme = ArmeLinhasLanc();
            foreach (Linha olinha in oArme.Linhas)
            {
                frmFiltro.oPesqProd.Linhas.Add(olinha);
            }
            frmFiltro.oPesqProd.NovaPagina("Geral");
            frmFiltro.oPesqProd.Pagina("Geral").Text = "2) Lançamentos";

            frmFiltro.oPesqProd.SelectedIndex = 0;
        }
        private  ArmeEdicao ArmeLinhasLote()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;

             olinha = new Linha("LOTE");
             olinha.cabecalho[0] = "Lote:";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 250;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);


            olinha = new Linha("SETOR");
            olinha.cabecalho[0] = "Setor:";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("APRONTE");
            olinha.cabecalho[0] = "Periodo:";
            //olinha.ofuncaoSql = PesquisaFuncoes.CompareGenericoExato;
            DateTime ini = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime fim = DateTime.Now;

            olinha.oedite[0] = new MaskedTextBox();
            (olinha.oedite[0] as MaskedTextBox).Text = ini.ToString("d");
            olinha.oedite[1] = new MaskedTextBox();
            (olinha.oedite[1] as MaskedTextBox).Text = ini.ToString("d");
            ((MaskedTextBox)olinha.oedite[0]).Mask = "00/00/0000";
            ((MaskedTextBox)olinha.oedite[0]).PromptChar = (Char)32; // space no lugar do prompt padrao que é _
            ((MaskedTextBox)olinha.oedite[0]).InsertKeyMode = InsertKeyMode.Default;
            ((MaskedTextBox)olinha.oedite[1]).Mask = "00/00/0000";
            ((MaskedTextBox)olinha.oedite[1]).PromptChar = (Char)32; // space no lugar do prompt padrao que é _
            ((MaskedTextBox)olinha.oedite[1]).InsertKeyMode = InsertKeyMode.Default;
            ((MaskedTextBox)olinha.oedite[0]).Enter += Linha_Enter;
            ((MaskedTextBox)olinha.oedite[1]).Enter += Linha_Enter;
            ((MaskedTextBox)olinha.oedite[0]).Width = 66;
            ((MaskedTextBox)olinha.oedite[1]).Width = 66;

            oArme.Linhas.Add(olinha);

            /*

         
             
             */



            /*
             olinha = new Linha("DEBITO/CREDITO");
             olinha.cabecalho[0] = "Contabilidade";
             olinha.oedite[0] = new ComboBox();
             ((ComboBox)olinha.oedite[0]).Items.Add("* Normal");
             ((ComboBox)olinha.oedite[0]).Items.Add("S Classificados");
             ((ComboBox)olinha.oedite[0]).Items.Add("N Não Classificados");
             olinha.ofuncao = PesquisaFuncoes.fPassa_Contab2;
             oArme.Linhas.Add(olinha);
            */

            /* olinha = new Linha("DOC");
             olinha.cabecalho[0] = "Documento";
             olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
             olinha.oedite[0] = new TextBox();
             ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
             oArme.Linhas.Add(olinha);*/
            return oArme;

/*          // DELPHI
 *          oPesquisa.linhas.Add('LOTE');
            oPesquisa.linhas.Cabecalho[oPesquisa.linhas.Count - 1, 0] := 'Lote:';
            oPesquisa.linhas.oEdite[oPesquisa.linhas.Count - 1, 0] := MaskEdit5;
            oPesquisa.linhas.Funcao[oPesquisa.linhas.Count - 1, 0] := @CompareIgualdade;

            {$IFDEF MLSA}
            oPesquisa.linhas.Add('SETOR');
            oPesquisa.linhas.Cabecalho[oPesquisa.linhas.Count - 1, 0] :=
              'Peq.Empresa(Setor):';
            oPesquisa.linhas.oEdite[oPesquisa.linhas.Count - 1, 0] := MaskEdit4;
            {$ENDIF}
            oPesquisa.linhas.Add('APRONTE/DATAINI/DATAFIN');
            decodeDAte(Date, ano, mes, dia);
            periodo1.DateTime := EncodeDate(ano, mes, 01);
            periodo2.DateTime := UltimodiaMes(Date);
            oPesquisa.linhas.Cabecalho[oPesquisa.linhas.Count - 1, 0] := 'Periodo:';
            oPesquisa.linhas.oEdite[oPesquisa.linhas.Count - 1, 0] := cbIndica;
            oPesquisa.linhas.Funcao[oPesquisa.linhas.Count - 1, 0] := @ComparePeriodoLote;
            // funcoes_check
            oPesquisa.linhas.oEdite[oPesquisa.linhas.Count - 1, 1] := periodo1;
            oPesquisa.linhas.oEdite[oPesquisa.linhas.Count - 1, 2] := periodo2;

           
            oPesquisa.NovaPagina('Geral');
            oPesquisa.Pagina['Geral'].Caption := '&2) Lançamentos';

            oPesquisa.ActivePageIndex := 0;
*/

        }
        private void Linha_Enter(object sender, EventArgs e)
        {

            (sender as MaskedTextBox).SelectionStart = 0;
            (sender as MaskedTextBox).SelectionLength = 10;
        }

        private ArmeEdicao ArmeLinhasLanc()
        {
            ArmeEdicao oArme = new ArmeEdicao();
            Linha olinha;
            
            olinha = new Linha("VALOR");
            olinha.cabecalho[0] = "Valor";
            olinha.oedite[0] = new ComboBox();
            ((ComboBox)olinha.oedite[0]).Items.Add("=");
            ((ComboBox)olinha.oedite[0]).Items.Add(">");
            ((ComboBox)olinha.oedite[0]).Items.Add("<");
            ((ComboBox)olinha.oedite[0]).Width = 30;
            olinha.oedite[1] = new NumericTextBox();
            ((NumericTextBox)olinha.oedite[1]).TextAlign = HorizontalAlignment.Right;
            olinha.ofuncao = PesquisaFuncoes.CompareValor;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("LOTE");
            olinha.cabecalho[0] = "Lote:";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            olinha.oedite[0].Width = 250;
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("SETOR");
            olinha.cabecalho[0] = "Setor:";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("GLEBA");
            olinha.cabecalho[0] = "Centro Resultado:";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);

            olinha = new Linha("QUA");
            olinha.cabecalho[0] = "Quadra:";
            olinha.ofuncao = PesquisaFuncoes.CompareGenericoExato;
            olinha.oedite[0] = new TextBox();
            ((TextBox)olinha.oedite[0]).CharacterCasing = CharacterCasing.Upper;
            oArme.Linhas.Add(olinha);
            return oArme;
         
        }

        public void MonteGrids()
        {
            oProdLote = new MonteGrid();
            oProdLote.Clear();
            oProdLote.AddValores("SETOR", "Setor", 4, "", false, 0, ""); // Aqui tem um ifdef indicando MLSA , se não não entra campo setor???
            oProdLote.AddValores("SAFRA", "Safra", 5, "", false, 0, "");
            oProdLote.AddValores("LOTE", "Lote", 5, "", false, 0, "");
            if (objProd.conversao != 0)
            {
                oProdLote.AddValores("TIPOPROD", "Tp Prod.", 10, "", false, 0, "");
                oProdLote.AddValores("TRANSP", "Dta Campo", 10, "", false, 0, "");
                oProdLote.AddValores("NATURA", objProd.natura, 8, "###,##0.00", true, 0, "");
                oProdLote.AddValores("APRONTE", "Dta Apronte", 10, "", false, 0, "");
                oProdLote.AddValores("BENEF", objProd.unid_bene, 9, "###,##0.00", true, 0, "");
                oProdLote.AddValores("CONVERSA", objProd.unid_bene.Trim() + "/" + objProd.natura.Trim(), 9, "###0.00", true, 0, "");
                oProdLote.AddValores("DTA_ENT", "Deposito", 10, "", false, 0, "");
                oProdLote.AddValores("KG_ENT", "Deposito", 10, "###,##0.00", true, 0, "");
                oProdLote.AddValores("DTA_APROVE", "Aprove", 10, "", false, 0, "");
                oProdLote.AddValores("APROVE", "Aprove", 9, "###,##0.00", true, 0, "");
                oProdLote.AddValores("DESAPROVE", "DesAprove", 9, "###,##0.00", true, 0, "");
                oProdLote.AddValores("DTA_BULK", "Dta Bulk", 10, "", false, 0, "");
                oProdLote.AddValores("DTA_CATAG", "Catag", 10, "", false, 0, "");
                oProdLote.AddValores("BOM", "Padrão", 9, "###,##0.00", true, 0, "");
                oProdLote.AddValores("PEQ", "A.Peq", 9, "###,##0.00", true, 0, "");
                oProdLote.AddValores("PO", "Pó", 9, "###,##0.00", true, 0, "");
                oProdLote.AddValores("DTA_RES", "Dta_Res", 10, "", false, 0, "");
                oProdLote.AddValores("MOFO", "Mofo/Inset", 7, "###,##0.00", true, 0, "");
                oProdLote.AddValores("DTA_MOFO", "Dta Mofo", 10, "", false, 0, "");
            }
            else
            {
                oProdLote.AddValores("BENEF", objProd.unid_bene, 9, "", true, 0, "");
                oProdLote.AddValores("TRANSP", "Dta Campo", 10, "", false, 0, "");
            }

            // oTable.FindField('TIPOPROD').onSetText := TGetSet.TpProdSetText;
            //oTable.FindField('TIPOPROD').onGetText := TGetSet.TpProdGetText;

            oProdMov = new MonteGrid();
            oProdMov.Clear();
            oProdMov.AddValores("DATAE", "Data", 12, "", false, 0, "");

            oProdMov.AddValores("QUA", "Quadra", 7, "", false, 0, ""); // Aqui tem um ifdef indicando MLSA , se não não entra campo setor???
            if (objProd.conversao != 0)
            {
                oProdMov.AddValores("NQUANT", objProd.natura.Trim(), 9, "###,##0.00", true, 0, "");
                oProdMov.AddValores("QUANT", "Tot.Benef.", 10, "###,##0.000", true, 0, "");
                oProdMov.AddValores("QUANT_FR", "Empresa(" + objProd.unid_bene.Trim() + ")", 10, "###,##0.000", false, 0, "");
                oProdMov.AddValores("QUANT_PA_RI", "Tot.Parc.(" + objProd.unid_bene.Trim() + ")", 10, "###,##0.000", false, 0, "");
                oProdMov.AddValores("QUANT_PA", "Parc.(" + objProd.unid_bene.Trim() + ")", 10, "###,##0.000", false, 0, "");
                oProdMov.AddValores("QUANT_RI", "R.I.(" + objProd.unid_bene.Trim() + ")", 10, "###,##0.000", false, 0, "");

            }
            else
            {
                oProdMov.AddValores("QUANT", "Tot.Benef.", 10, "###,##0.000", true, 0, "");
                oProdMov.AddValores("QUANT_FR", "Empresa(" + objProd.natura.Trim() + ")", 10, "###,##0.000", false, 0, "");
                oProdMov.AddValores("QUANT_PA_RI", "Tot.Parc.(" + objProd.natura.Trim() + ")", 10, "###,##0.000", false, 0, "");
                oProdMov.AddValores("QUANT_PA", "Parc.(" + objProd.natura.Trim() + ")", 10, "###,##0.000", false, 0, "");
                oProdMov.AddValores("QUANT_RI", "R.I.(" + objProd.natura.Trim() + ")", 10, "###,##0.000", false, 0, "");
            }


            oProdRec = new MonteGrid();
            //           oTable.FindField('TIPOPROD').onSetText := TGetSet.TpProdSetText;
            //         oTable.FindField('TIPOPROD').onGetText := TGetSet.TpProdGetText;
            oProdRec.Clear();
            oProdRec.AddValores("DATA", "Data", 12, "", false, 0, "");
            oProdRec.AddValores("QUANT", objProd.unid_bene.Trim(), 10, "###,##0.00", true, 0, "");
            oProdRec.AddValores("TIPOPROD", "Tp Prod.", 20, "", false, 0, "");


            oProdFruta = new MonteGrid();
            oProdFruta.Clear();
            oProdFruta.AddValores("DTACOMPPRA", "Data", 12, "", false, 0, "");
            oProdFruta.AddValores("CODPROD", "Fruta", 25, "", false, 0, "");
            oProdFruta.AddValores("QUANT", "Quant(kg)", 12, "#####,##0.0", true, 0, "");
            oProdFruta.AddValores("POLPA", "Polpa(kg)", 12, "#####,##0.0", true, 0, "");
            oProdFruta.AddValores("VALOR", "Valor R$", 12, "##,###,##0.00", true, 0, "");



            oProdDestino = new MonteGrid();
            oProdDestino.Clear();
            oProdDestino.AddValores("DTA_VENDA", "Data", 10, "", false, 0, "");
            oProdDestino.AddValores("FIRMA", "Firma", 40, "", false, 0, "");
            oProdDestino.AddValores("CONTRATO", "Contrato", 8, "", false, 0, "");
            oProdDestino.AddValores("COMPLEM", "Tipo", 15, "", false, 0, "");
            oProdDestino.AddValores("BULK_KG", "Bulk(kg)", 10, "##,###,##0.00", true, 0, "");
            oProdDestino.AddValores("FINO", "Fino(kg)", 10, "##,###,##0.00", true, 0, "");
            oProdDestino.AddValores("DALTON", "Dalton(kg)", 10, "##,###,##0.00", true, 0, "");


            oProdSobra = new MonteGrid();
            oProdSobra.Clear();
            oProdSobra.AddValores("DTA_VENDA", "Data", 10, "", false, 0, "");
            oProdSobra.AddValores("FIRMA", "Firma", 40, "", false, 0, "");
            oProdSobra.AddValores("CONTRATO", "Contrato", 8, "", false, 0, "");
            oProdSobra.AddValores("COMPLEM", "Tipo", 15, "", false, 0, "");
            oProdSobra.AddValores("BULK_KG", "Bulk(kg)", 10, "##,###,##0.00", true, 0, "");
            oProdSobra.AddValores("FINO", "Fino(kg)", 10, "##,###,##0.00", true, 0, "");
            oProdSobra.AddValores("DALTON", "Dalton(kg)", 10, "##,###,##0.00", true, 0, "");
        }

        


    }
}
