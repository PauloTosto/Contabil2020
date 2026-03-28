using ApoioContabilidade.Models;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Produção.Servicos
{
    // CAMADA DOS DATASETS
    public class Eventos_Produto
    {
        public BindingSource bmProdLote;
        public BindingSource bmProdMov;
        public BindingSource bmProdRec;
        public BindingSource bmFruta;
        public BindingSource bmDestino;
        public BindingSource bmSobra;
        public BindingSource bmTriagMov;

        public DataTable tabProdLote;
        public DataTable tabProdMov;
        public DataTable tabRec;
        public DataTable tabSobra;
        public DataTable tabFruta;
        public DataTable tabDestino;
        public DataTable tabTriagMov;



        public bool recomece = true;
        public bool recalcula;
        public EdtProduto edtProduto;
        public FrmProd ofrmProd;


        private ObjProduto objProd; // produto atual (CATPROD)


        public Eventos_Produto()
        {
            // edtProduto = new EdtProduto(this);

            bmProdLote = new BindingSource();
            bmProdMov = new BindingSource();
            bmProdRec = new BindingSource();
            bmFruta = new BindingSource();
            bmDestino = new BindingSource();
            bmSobra = new BindingSource();
            bmTriagMov = new BindingSource();
        }

        public async Task<bool> Consulte()
        {
            bool result = true;

            try
            {
                if (edtProduto.frmFiltro.cbProdutos.SelectedIndex == -1)
                {
                    MessageBox.Show("Defina Produto");
                    return result;
                }



                /*oProdLote.oTable.DisableControls;
                oProdMov.oTable.DisableControls;
                oProdRec.oTable.DisableControls;

                oProdLote.oTable.AfterScroll := nil;
                */

                result = await filtroMestre();


                /* oProdLote.oTable.AfterScroll := FiltroProdMov;
                 oProdLote.oTable.first;

                 oProdLote.oTable.EnableControls;
                 oProdMov.oTable.EnableControls;
                 oProdRec.oTable.EnableControls;
                 dsProdMov.DataSet := oProdMov.oTable;*/



            }
            finally
            {


            }

            return result;
        }


        private async Task<bool> filtroMestre()
        {

            // Pegue os Dados Do Filtro e Classifique para a PEsquisa
            // CamposFiltro.Add(OrdemCampos.Strings[ind2]+'='+Cells[1,(ind2+1)]);
            // with frmFiltroNovoProd do
            // begin

            // if FTipoRelatorio then
            // begin
            // oloteconf := Tconfigura_adoTable.Create;
            // oloteconf.PegTable(oProdLote.oTable, 'LOTE');
            // end;

            Dictionary<string, string> valoresFiltro_Antes = new Dictionary<string, string>();
            foreach (var filtro in edtProduto.valoresFiltro)
            {
                valoresFiltro_Antes.Add(filtro.Key, filtro.Value);
            }
            // guarda os valores velhos e pega os novos
            edtProduto.ValoresFiltro();


            objProd = edtProduto.objProd;
            //ListaLote.Clear;
            string strprodsafra = " (PROD = '" + objProd.cod + "') AND "
                   + " (SAFRA BETWEEN '" + edtProduto.valoresFiltro["SAFRAINICIO"] + "' AND '" +

                   edtProduto.valoresFiltro["SAFRAFIM"] + "') ";


            bool pesquisedenovo = false;
            string strVendas = "";
            if ((valoresFiltro_Antes["VENDAS"] != edtProduto.valoresFiltro["VENDAS"])
                 || (valoresFiltro_Antes["VENDAS_CONTRATO"] != edtProduto.valoresFiltro["VENDAS_CONTRATO"]))
                pesquisedenovo = true;
            if (edtProduto.valoresFiltro["VENDAS"] == "VENDIDOS")
            {
                if (edtProduto.valoresFiltro["VENDAS_CONTRATO"] != "")
                {
                    strVendas = " AND ( EXISTS (SELECT Lot_venda.id_lote " +
                    " FROM lot_venda " +
                    " WHERE  (lot_venda.id_lote = LOTE.id_lote)" + ") ) ";
                }
                else
                {
                    strVendas = " AND (EXISTS (SELECT Lot_venda.id_lote " +
                       " FROM lot_venda " + " WHERE (lot_venda.id_lote = LOTE.id_lote)" +
                    " AND (alltrim(lot_venda.cont) = '"
                    + edtProduto.valoresFiltro["VENDAS_CONTRATO"].Substring(0, 6) +
                         "') AND " + " (lot_VENDA.PROD = '" +
                        objProd.cod + "')"
                        + " AND (lot_VENDA.SAFRA = '" + edtProduto.valoresFiltro["VENDAS_CONTRATO"].Substring(0, 4) + "') ) ) ";
                }

            }
            else if (edtProduto.valoresFiltro["VENDAS"] == "NÃO VENDIDOS")
            {
                strVendas = " AND ( NOOT EXISTS (SELECT Lot_venda.id_lote " +
                   " FROM lot_venda " +
                   " WHERE  (lot_venda.id_lote = LOTE.id_lote)" + ") ) ";
            }
            try
            {


                if ((valoresFiltro_Antes["SAFRAINICIO"] != edtProduto.valoresFiltro["SAFRAINICIO"])
                    || (valoresFiltro_Antes["SAFRAFIM"] != edtProduto.valoresFiltro["SAFRAFIM"])
                    || (valoresFiltro_Antes["PRODUTO"] != edtProduto.valoresFiltro["PRODUTO"])
                    || pesquisedenovo
                    || recomece
                    )
                {
                    List<string> lststr = new List<string>();
                    lststr.Add("SELECT * FROM FRUTADLT");
                    // frutas
                    string str = "SELECT  LOTE.id_lote, lot_venda.dta_venda, lot_venda.bulk_KG as BULK_KG, lot_venda.fino as FINO, "
                        + "lot_venda.dalton as DALTON, lot_venda.vassoura, lot_venda.po AS PO, lot_venda.certif,"
                        + " lot_venda.Cont as contrato, vendas.firma, vendas.complem " +
                        " FROM  LOTE, lot_venda, vendas " +
                        " WHERE LOTE.id_lote = lot_venda.id_lote AND lot_venda.id_venda = vendas.vendas_id AND "
                        + " (LOTE.SAFRA BETWEEN '" + edtProduto.valoresFiltro["SAFRAINICIO"]
                        + "' AND '" + edtProduto.valoresFiltro["SAFRAFIM"] + "')  ";
                    lststr.Add(str);
                    // destino
                    str = "SELECT LOTE.id_lote, SOBRA_LINK.DATA_ORG as DTA_VENDA,SOBRAS.firma,SOBRA_LINK.CONT_ORG as CONTRATO, SOBRAS.complem, "
                            + " iif(sobracalc.prod_tp = 0,KG_RAT,000.00) as BULK_KG, " +
                            " iif(sobracalc.prod_tp = 1,KG_RAT,000.00) as FINO, " +
                            " iif(sobracalc.prod_tp = 11,KG_RAT,000.00) as DALTON " +
                            " FROM SOBRA_LINK, SOBRACALC, LOTE, SOBRAS_NOVA  SOBRAS " +
                            " WHERE  SOBRA_LINK.id_org = SOBRACALC.id_org AND SOBRA_LINK.id_cmp = SOBRACALC.id_cmp "
                            + " AND SOBRACALC.id_lote = LOTE.id_lote AND SOBRA_LINK.id_cmp = SOBRAS.id_CMP "
                            + " AND (LOTE.SAFRA BETWEEN '" +
                            edtProduto.valoresFiltro["SAFRAINICIO"] + "' AND '" +
                            edtProduto.valoresFiltro["SAFRAFIM"] + "')  AND (SOBRACALC.kg_rat <> 0) ";
                    lststr.Add(str);
                    // sobra

                    str = "SELECT Produto.*,0.0000 as QUANT_PA_RI FROM PRODUTO WHERE "
                       + strprodsafra + " ORDER BY PROD,SAFRA,LOTE,SETOR";
                    lststr.Add(str);
                    // PRODMOV

                    str = "SELECT * FROM LOTE WHERE " + // datastring +
                       " (ALLTRIM(LOTE) <> '') AND (ALLTRIM(PROD) <> '') AND " +
                       "(ALLTRIM(SAFRA) <> '') AND " + strprodsafra + strVendas +
                       " ORDER BY  SETOR,PROD,SAFRA,LOTE";
                    lststr.Add(str);
                    //prodLote

                    str = "SELECT * FROM TRIAG_MOV WHERE " +
                     "(ALLTRIM(LOTE) <> '') AND (ALLTRIM(PROD) <> '') AND " +
                     "(ALLTRIM(SAFRA) <> '') AND " + strprodsafra +
                     " ORDER BY  SETOR,PROD,SAFRA,LOTE";
                    // triagmov


                    lststr.Add(str);

                    str = "SELECT * FROM SEGUE WHERE " +
                       "(ALLTRIM(LOTE) <> '') AND (ALLTRIM(PROD) <> '') AND " +
                        "(ALLTRIM(SAFRA) <> '') AND " + strprodsafra +
                        " ORDER BY  SETOR,PROD,SAFRA,LOTE";
                    lststr.Add(str);


                    DataSet dsDados = await ApiServices.Api_QueryMulti(lststr);
                    if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;

                    tabFruta = dsDados.Tables[0].Copy();
                    tabFruta.TableName = "FRUTA";
                    tabDestino = dsDados.Tables[1].Copy();
                    tabDestino.TableName = "DESTINO";
                    tabSobra = dsDados.Tables[2].Copy();
                    tabSobra.TableName = "SOBRA";
                    tabProdMov = dsDados.Tables[3].Copy();
                    tabProdMov.TableName = "PRODMOV";
                    tabProdLote = dsDados.Tables[4].Copy();
                    tabProdLote.TableName = "PRODLOTE";
                    tabProdLote.AcceptChanges();
                    tabTriagMov = dsDados.Tables[5].Copy();
                    tabTriagMov.TableName = "TRIAGMOV";
                    tabRec = dsDados.Tables[6].Copy();
                    tabRec.TableName = "SEGUE";
                }
            }
            catch (Exception)
            {

                MessageBox.Show("Error acesso Dados");
                return false;
            }
            recomece = false;
            // primeira situação Filtro de Tipo de Produto ... Só relavante para CACAU 
            string strtipoprod = "";
            int tipoProduto = -1;
            if (edtProduto.objProd.cod == "  1") // CACAU
            {
                tipoProduto = Convert.ToInt32(edtProduto.frmFiltro.cbTipoProd.SelectedValue);

                if (tipoProduto != -1)
                {
                    strtipoprod = " (TIPOPROD = " + tipoProduto.ToString() + ") ";
                }

                /*switch (edtProduto.valoresFiltro["ESTAGIOS"].ToUpper().Trim())
                {
                    case "SEM FILTRO":
                    case "COCHOFERMENTAÇÃO":
                    case "FAZENDA":
                    case "DEPÓSITO":
                    case "APROVE":
                    case "CATAGEM":
                        break;
                    default:
                        break;
                }
                switch (edtProduto.valoresFiltro["ARMAZENAMENTO"].ToUpper().Trim())
                {
                    case "SEM FILTRO":
                    case "FAZENDA":
                    case "TRIAGEM":
                    case "DEPÓSITO CENTRAL":
                        break;
                    default:
                        break;
                }*/

            }
            // verificar os outros dados do filtro da pesquisa
            //  datastring := datastring + ' AND ' + strtipoprod

            TabPage_Apoio pagina = null;
            Pesquise oPesquisa = edtProduto.frmFiltro.oPesqProd;

            List<string> lstSetor = new List<string>();
            List<string> lstLote = new List<string>();
            DateTime inicioApronte = DateTime.MinValue;
            DateTime fimApronte = DateTime.MinValue;
            if (oPesquisa != null)
            {
                if (oPesquisa.TabPages.ContainsKey("Lotes"))
                {
                    pagina = (oPesquisa.TabPages["Lotes"] as TabPage_Apoio);
                }
                List<LinhaSolucao> oLista = null;
                if (pagina != null)
                    oLista = pagina.Get_LinhaSolucao();

                if ((oLista != null) && (oLista.Count > 0))
                {
                    foreach (LinhaSolucao linha in oLista)
                    {
                        if (linha.campo.ToUpper() == "LOTE")
                        {
                            lstLote = linha.dado[0].Trim().Split(Convert.ToChar("/")).ToList();
                            for (int i = 0; i < lstLote.Count; i++)
                            {
                                lstLote[i] = lstLote[i].Trim();
                            }

                        }
                        else if (linha.campo.ToUpper() == "SETOR")
                        {
                            lstSetor = linha.dado[0].Trim().Split(Convert.ToChar("/")).ToList();
                            for (int i = 0; i < lstSetor.Count; i++)
                            {
                                lstSetor[i] = lstSetor[i].Trim();
                            }

                        }
                        else if (linha.campo.ToUpper() == "APRONTE")
                        {
                            inicioApronte = Convert.ToDateTime(linha.dado[0]);
                            fimApronte = Convert.ToDateTime(linha.dado[1]);

                        }
                    }
                }
            }
            bmProdLote.DataSource = tabProdLote.AsEnumerable().Where(row =>
                                        (tipoProduto == -1 ? true : row.Field<Int32>("TIPOPROD") == tipoProduto)
                                    && (lstSetor.Count == 0 ? true : lstSetor.Contains(row.Field<string>("SETOR").Trim()))
                                    && (lstLote.Count == 0 ? true : lstLote.Contains(row.Field<string>("LOTE").Trim()))
                                    && (inicioApronte == DateTime.MinValue ? true :
                                         (!row.IsNull("APRONTE"))
                                        && (row.Field<DateTime>("APRONTE").CompareTo(inicioApronte) >= 0)
                                         && (row.Field<DateTime>("APRONTE").CompareTo(fimApronte) <= 0)
                                         )

                    ).OrderBy(row => row.Field<string>("SETOR")).ThenBy(row => row.Field<string>("SAFRA")).
                     ThenBy(row => row.Field<string>("LOTE")).AsDataView();
            bmProdMov.DataSource = tabProdMov.AsDataView();
            bmProdRec.DataSource = tabRec.AsDataView();
            bmSobra.DataSource = tabSobra.AsDataView();
            bmDestino.DataSource = tabDestino.AsDataView();
            bmFruta.DataSource = tabFruta.AsDataView();
            return true;
        }
        public async void BmProdLote_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);
            if (registro == null) return;
            if ((registro.Row.RowState == DataRowState.Detached)
                  || (registro.Row.RowState == DataRowState.Added))
                return;
            // botão do recalcula
            recalcula = false;
            ofrmProd.btnRecalcula.Enabled = false;



            TabPage_Apoio pagina = null;
            Pesquise oPesquisa = edtProduto.frmFiltro.oPesqProd;

            List<string> lstSetor = new List<string>();
            List<string> lstLote = new List<string>();
            DateTime inicioApronte = DateTime.MinValue;
            DateTime fimApronte = DateTime.MinValue;
            if (oPesquisa != null)
            {
                if (oPesquisa.TabPages.ContainsKey("Lotes"))
                {
                    pagina = (oPesquisa.TabPages["Lotes"] as TabPage_Apoio);
                }
                List<LinhaSolucao> oLista = null;
                if (pagina != null)
                    oLista = pagina.Get_LinhaSolucao();

                if ((oLista != null) && (oLista.Count > 0))
                {
                    foreach (LinhaSolucao linha in oLista)
                    {
                        if (linha.campo.ToUpper() == "LOTE")
                        {
                            lstLote = linha.dado[0].Trim().Split(Convert.ToChar("/")).ToList();
                            for (int i = 0; i < lstLote.Count; i++)
                            {
                                lstLote[i] = lstLote[i].Trim();
                            }

                        }
                        else if (linha.campo.ToUpper() == "SETOR")
                        {
                            lstSetor = linha.dado[0].Trim().Split(Convert.ToChar("/")).ToList();
                            for (int i = 0; i < lstSetor.Count; i++)
                            {
                                lstSetor[i] = lstSetor[i].Trim();
                            }

                        }
                        else if (linha.campo.ToUpper() == "APRONTE")
                        {
                            inicioApronte = Convert.ToDateTime(linha.dado[0]);
                            fimApronte = Convert.ToDateTime(linha.dado[1]);

                        }
                    }
                }
            }
            bmProdMov.DataSource = tabProdMov.AsEnumerable().Where(row =>
                                 (row.Field<string>("SETOR").Trim() == registro["SETOR"].ToString().Trim())
                                 && (row.Field<string>("LOTE").Trim() == registro["LOTE"].ToString().Trim())
                                && (lstSetor.Count == 0 ? true : lstSetor.Contains(row.Field<string>("SETOR").Trim()))
                                    && (lstLote.Count == 0 ? true : lstLote.Contains(row.Field<string>("LOTE").Trim()))
                                    && (inicioApronte == DateTime.MinValue ? true :
                                         (!row.IsNull("APRONTE"))
                                        && (row.Field<DateTime>("APRONTE").CompareTo(inicioApronte) >= 0)
                                         && (row.Field<DateTime>("APRONTE").CompareTo(fimApronte) <= 0)
                                         )

                    ).OrderBy(row => row.Field<string>("SETOR")).ThenBy(row => row.Field<string>("SAFRA")).
                     ThenBy(row => row.Field<string>("LOTE")).AsDataView();

            if (ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpFruta))
                ofrmProd.tcDetalhes.TabPages.Remove(ofrmProd.tpFruta);
            if (ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpVendas))
                ofrmProd.tcDetalhes.TabPages.Remove(ofrmProd.tpVendas);
            if (ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpAjustes))
                ofrmProd.tcDetalhes.TabPages.Remove(ofrmProd.tpAjustes);

            if (objProd.conversao != 0)
            {

                bool retorno = await Grave_TriagMov(registro);
                bmProdRec.DataSource = tabRec.AsEnumerable().Where(row =>
                                          (row.Field<string>("SETOR").Trim() == registro["SETOR"].ToString().Trim())
                                 && (row.Field<string>("SAFRA").Trim() == registro["SAFRA"].ToString().Trim())
                                 && (row.Field<string>("LOTE").Trim() == registro["LOTE"].ToString().Trim())
                                 && (row.Field<string>("PROD").Trim() == registro["PROD"].ToString().Trim())

                                 ).OrderBy(row => row.Field<string>("PROD"))
                                 .ThenBy(row => row.Field<string>("SAFRA"))
                                 .ThenBy(row => row.Field<string>("LOTE"))
                                 .ThenBy(row => row.Field<string>("SETOR"))
                                 .AsDataView();

                if (Convert.ToDouble(registro["TIPOPROD"]) == 11)
                {
                    if (!ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpFruta))
                        ofrmProd.tcDetalhes.TabPages.Insert(ofrmProd.tcDetalhes.TabPages.Count,
                            ofrmProd.tpFruta);
                    bmFruta.DataSource = tabFruta.AsEnumerable().Where(row =>
                             row.Field<double>("ID_LOTE") == Convert.ToDouble(registro["ID_LOTE"])).AsDataView();
                }
                bmDestino.DataSource = tabDestino.AsEnumerable().Where(row =>
                             row.Field<double>("ID_LOTE") == Convert.ToDouble(registro["ID_LOTE"])).AsDataView();
                if (bmDestino.Count > 0)
                {
                    if (!ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpVendas))
                        ofrmProd.tcDetalhes.TabPages.Insert(ofrmProd.tcDetalhes.TabPages.Count,
                            ofrmProd.tpVendas);
                }
                bmSobra.DataSource = tabSobra.AsEnumerable().Where(row =>
                            row.Field<double>("ID_LOTE") == Convert.ToDouble(registro["ID_LOTE"])).AsDataView();
                if (bmSobra.Count > 0)
                {
                    if (!ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpAjustes))
                        ofrmProd.tcDetalhes.TabPages.Insert(ofrmProd.tcDetalhes.TabPages.Count,
                            ofrmProd.tpAjustes);
                }

                ofrmProd.btnRecalcula.Enabled = VerifiqueRecalcula(registro);
            }
            else
            {
                bmSobra.DataSource = tabSobra.AsEnumerable().Where(row =>
                            row.IsNull("ID")).AsDataView();

                bmProdRec.DataSource = tabRec.AsEnumerable().Where(row =>
                               row.IsNull("ID")).AsDataView();
                bmFruta.DataSource = tabFruta.AsEnumerable().Where(row =>
                               row.IsNull("ID")).AsDataView();
                if (ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpVendas))
                {
                    ofrmProd.tcDetalhes.TabPages.Remove(ofrmProd.tpVendas);
                }
                if (ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpAjustes))
                {
                    ofrmProd.tcDetalhes.TabPages.Remove(ofrmProd.tpAjustes);
                }
                if (ofrmProd.tcDetalhes.TabPages.Contains(ofrmProd.tpFruta))
                    ofrmProd.tcDetalhes.TabPages.Remove(ofrmProd.tpFruta);

            }
            edtProduto.oProdMov.FuncaoSoma();
            ofrmProd.colocaSaldoDetalhes(edtProduto.oProdMov);
            edtProduto.oProdRec.FuncaoSoma();
            ofrmProd.colocaSaldoDetalhes(edtProduto.oProdRec);
            edtProduto.oProdDestino.FuncaoSoma();
            ofrmProd.colocaSaldoDetalhes(edtProduto.oProdDestino);


            /* 
            
            */






        }




        private async Task<bool> Grave_TriagMov(DataRowView orowProdLote)
        {
            bool result = true;
            var rows = tabTriagMov.AsEnumerable().Where(row => row.Field<double>("ID_LOTE") ==
               Convert.ToDouble(orowProdLote["ID_LOTE"]));
            if (rows != null) return result;
            DateTime transp = Convert.ToDateTime(orowProdLote["TRANSP"]); // data campo
            DateTime apronte = Convert.ToDateTime(orowProdLote["APRONTE"]);
            double kg_faz = Convert.ToDouble(orowProdLote["BENEF"]);
            double benef = kg_faz;
            double natura = Convert.ToDouble(orowProdLote["NATURA"]);
            double kg_ent = kg_faz;
            DateTime dta_ent = apronte;

            DateTime dta_bulk = new DateTime(0, 0, 0);
            DateTime dta_res = new DateTime(0, 0, 0);

            DateTime dta_aprove = new DateTime(0, 0, 0);
            DateTime dta_catag = dta_aprove;
            DateTime dta_mofo = dta_aprove;

            double aprove = 0; // 20 // 10
            double desaprove = 0; // codigo 2 / 22
            double bom = 0; // 1 / 11
            double peq = 0; // 3  / 23
            double po = 0; // 4 // 24
            double mofo = 0; // 4 // 24

            DataRow orow = tabTriagMov.NewRow();
            orow["ID_LOTE"] = orowProdLote["ID_LOTE"];
            orow["SAFRA"] = orowProdLote["SAFRA"];
            orow["LOTE"] = orowProdLote["LOTE"];
            orow["PROD"] = orowProdLote["PROD"];
            orow["SETOR"] = orowProdLote["SETOR"];
            orow["TIPOPROD"] = orowProdLote["TIPOPROD"];
            orow["APRONTE"] = apronte;
            orow["TRANSP"] = transp;
            orow["DTA_ENT"] = dta_ent;
            orow["DTA_APROVE"] = dta_aprove;
            orow["DTA_CATAG"] = dta_catag;
            orow["DTA_MOFO"] = dta_mofo;

            orow["DTA_BULK"] = dta_bulk;
            orow["DTA_RES"] = dta_res;
            orow["KG_FAZ"] = kg_faz;
            orow["KG_ENT"] = kg_ent;

            orow["APROVE"] = aprove;
            orow["DESAPROVE"] = desaprove;
            orow["BOM"] = bom;
            orow["PEQ"] = peq;

            orow["PO"] = po;
            orow["MOFO"] = mofo;
            orow["NATURA"] = natura;
            orow["BENEF"] = benef;
            orow["EDITADO"] = -1;
            tabTriagMov.Rows.Add(orow);
            bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, orow.RowState, "TRIAG_MOV"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                MessageBox.Show("Erro ao tentar Incluir TRIAG_MOV!! ID_LOTE" + orowProdLote["ID_LOTE"].ToString());
            }

            return ok;
        }



        private bool VerifiqueRecalcula(DataRowView orowProdLote)
        {
            bool result = false;

            if (orowProdLote == null) return result;
            if (Convert.ToInt64(orowProdLote["ID_LOTE"]) == 0) return result;
            if ((Convert.ToInt32(orowProdLote["TIPOPROD"]) != 0)
                && (Convert.ToInt32(orowProdLote["TIPOPROD"]) != 1)
                && (Convert.ToInt32(orowProdLote["TIPOPROD"]) != 11)
                ) return result;

            double quant_benef = 0;
            double bulk_triag = 0;
            double dalton_triag = 0;
            double fino_triag = 0;
            double bulk_dest = 0;
            double dalton_dest = 0;
            double fino_dest = 0;
            double bulk_lote = 0;
            double dalton_lote = 0;
            double fino_lote = 0;
            foreach (DataRowView orowv in (bmDestino.DataSource as DataView))
            {
                if (Convert.ToInt32(orowv["COMPLEM"]) == 0) continue;
                bulk_dest = bulk_dest + Convert.ToDouble(orowv["BULK_KG"]);
                fino_dest = fino_dest + Convert.ToDouble(orowv["FINO"]);
                dalton_dest = dalton_dest + Convert.ToDouble(orowv["DALTON"]);
            }
            foreach (DataRowView orowv in (bmSobra.DataSource as DataView))
            {
                if (Convert.ToInt32(orowv["COMPLEM"]) == 0) continue;
                bulk_dest = bulk_dest + Convert.ToDouble(orowv["BULK_KG"]);
                fino_dest = fino_dest + Convert.ToDouble(orowv["FINO"]);
                dalton_dest = dalton_dest + Convert.ToDouble(orowv["DALTON"]);
            }


            DataRow orow = tabTriagMov.AsEnumerable().Where(row => (!row.IsNull("ID_LOTE"))
                       && row.Field<double>("ID_LOTE") == Convert.ToDouble(orowProdLote["ID_LOTE"])).FirstOrDefault();
            if (orow != null)
            {
                quant_benef = Convert.ToDouble(orow["KG_ENT"]);
                if (Convert.ToInt32(orow["TIPOPROD"]) != 0)
                {
                    bulk_triag = Convert.ToDouble(orow["DESAPROVE"]) +
                                      Convert.ToDouble(orow["PEQ"]) + Convert.ToDouble(orow["MOFO"]);
                }
                else
                    bulk_triag = quant_benef;
                if (Convert.ToInt32(orow["TIPOPROD"]) == 1)
                {
                    fino_triag = Convert.ToDouble(orow["BOM"]) -
                                      Convert.ToDouble(orow["MOFO"]);
                }
                else if (Convert.ToInt32(orow["TIPOPROD"]) == 11)
                {
                    dalton_triag = Convert.ToDouble(orow["BOM"]) -
                                        Convert.ToDouble(orow["MOFO"]);
                }
            }
            if ((bulk_dest != 0) || (fino_dest != 0) || (dalton_dest != 0))
            {
                if (Convert.ToInt32(orowProdLote["TIPOPROD"]) != 0)
                {
                    bulk_lote = Convert.ToDouble(orowProdLote["DESAPROVE"]) +
                                    Convert.ToDouble(orowProdLote["PEQ"]) + Convert.ToDouble(orowProdLote["MOFO"]);
                }
                else
                    bulk_lote = Convert.ToDouble(orowProdLote["KG_ENT"]);


                if (Convert.ToInt32(orowProdLote["TIPOPROD"]) == 1)
                {
                    fino_lote = Convert.ToDouble(orowProdLote["BOM"]) -
                                    Convert.ToDouble(orowProdLote["MOFO"]);
                }
                if (Convert.ToInt32(orowProdLote["TIPOPROD"]) == 11)
                {
                    dalton_lote = Convert.ToDouble(orowProdLote["BOM"]) -
                                    Convert.ToDouble(orowProdLote["MOFO"]);
                }

                if (((fino_triag + fino_dest) != fino_lote) ||
                         ((dalton_triag + dalton_dest) != dalton_lote) ||
                        ((bulk_triag + bulk_dest) != bulk_lote))
                {
                    result = true;
                }
            }
            return result;
        }

        private void RateieBulkFino(DataRow rowClone, DataRow rowvenda)
        {
            double desaprove, peq, mofo, total, resto;
            rowClone.BeginEdit();
            rowClone["BENEF"] =
              Convert.ToDouble(rowClone["BENEF"]) +
              Convert.ToDouble(rowvenda["KG_RAT"]);
            rowClone["KG_ENT"] =
              Convert.ToDouble(rowClone["KG_ENT"]) +
              Convert.ToDouble(rowvenda["KG_RAT"]);
            if ((Convert.ToDouble(rowClone["DESAPROVE"]) == 0) &&
                    (Convert.ToDouble(rowClone["MOFO"]) == 0))
            {
                rowClone["PEQ"] =
              Convert.ToDouble(rowClone["PEQ"]) +
              Convert.ToDouble(rowvenda["KG_RAT"]);

            }
            else
            {
                desaprove = Convert.ToDouble(rowClone["DESAPROVE"]);
                peq = Convert.ToDouble(rowClone["PEQ"]);
                mofo = Convert.ToDouble(rowClone["MOFO"]);

                total = desaprove + peq + mofo;
                if (total != 0)
                {
                    desaprove = Math.Truncate((desaprove / total) * Convert.ToDouble(rowvenda["KG_RAT"]));
                    peq = Math.Truncate((peq / total) * Convert.ToDouble(rowvenda["KG_RAT"]));
                    mofo = Math.Truncate((mofo / total) * Convert.ToDouble(rowvenda["KG_RAT"]));
                    resto = Convert.ToDouble(rowvenda["KG_RAT"]) - (peq + mofo + desaprove);
                    if ((Convert.ToDouble(rowClone["DESAPROVE"]) > Convert.ToDouble(rowClone["MOFO"]))
                        && (Convert.ToDouble(rowClone["DESAPROVE"]) > Convert.ToDouble(rowClone["PEQ"])))
                    {
                        desaprove = desaprove + resto;
                    }
                    else if ((Convert.ToDouble(rowClone["PEQ"]) > Convert.ToDouble(rowClone["MOFO"]))
                        && (Convert.ToDouble(rowClone["PEQ"]) > Convert.ToDouble(rowClone["DESAPROVE"])))
                    {
                        peq = peq + resto;
                    }
                    else if ((Convert.ToDouble(rowClone["MOFO"]) > Convert.ToDouble(rowClone["PEQ"]))
                     && (Convert.ToDouble(rowClone["MOFO"]) > Convert.ToDouble(rowClone["DESAPROVE"])))
                    {
                        mofo = mofo + resto;
                    }
                }
                rowClone["DESAPROVE"] =
                      Convert.ToDouble(rowClone["DESAPROVE"]) + desaprove;
                rowClone["PEQ"] =
                      Convert.ToDouble(rowClone["PEQ"]) + peq;
                rowClone["MOFO"] =
                  Convert.ToDouble(rowClone["MOFO"]) + mofo;

                if ((peq + mofo) != 0)
                {
                     rowClone["APROVE"] =
                        Convert.ToDouble(rowClone["APROVE"]) + peq+ mofo;
                }
                if (mofo != 0)
                {
                    rowClone["BOM"] =
                       Convert.ToDouble(rowClone["BOM"]) +  mofo;
                }
            }
        }
        private async Task<bool> PegVendasSobras(DataTable adoClone, List<string> lststr)
        {
            DataRow rowClone = adoClone.Rows[0];
            DataSet dsDados = await ApiServices.Api_QueryMulti(lststr);
            if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
            foreach (DataTable vendasobras in dsDados.Tables)
            {
                if (vendasobras.Rows.Count == 0) continue;
                foreach (DataRow rowvenda in vendasobras.Rows)
                {
                    if (Convert.ToInt32(rowvenda["PROD_TP"]) == 0) //contrato de venda_bulk
                    {
                        if (Convert.ToInt32(rowvenda["PROD_TPLOT"]) == 0) // se o lote original é bulk
                        {
                            rowClone.BeginEdit();
                            rowClone["BENEF"] =
                              Convert.ToDouble(rowClone["BENEF"]) +
                              Convert.ToDouble(rowvenda["KG_RAT"]);
                            rowClone["KG_ENT"] =
                              Convert.ToDouble(rowClone["KG_ENT"]) +
                              Convert.ToDouble(rowvenda["KG_RAT"]);
                            rowClone.EndEdit();
                        }
                        else  // para contrato do tipo bulk porém com lote do tipo fino ou dalton (nao bulk)
                        {
                            RateieBulkFino(rowClone, rowvenda);
                        }
                    }
                    else // nao sendo bulk
                    {
                        rowClone.BeginEdit();
                        rowClone["BENEF"] =
                          Convert.ToDouble(rowClone["BENEF"]) +
                          Convert.ToDouble(rowvenda["KG_RAT"]);
                        rowClone["KG_ENT"] =
                          Convert.ToDouble(rowClone["KG_ENT"]) +
                          Convert.ToDouble(rowvenda["KG_RAT"]);
                        rowClone["APROVE"] =
                          Convert.ToDouble(rowClone["APROVE"]) +
                         Convert.ToDouble(rowvenda["KG_RAT"]);
                        rowClone["BOM"] =
                         Convert.ToDouble(rowClone["BOM"]) +
                        Convert.ToDouble(rowvenda["KG_RAT"]);
                        rowClone.EndEdit();
                    }
                }
            }
            return true;
        }

        public async Task<bool> Reconstrua_Editados()
        {
            bool resul = false;
            DataRowView orowProdLote = (bmProdLote.Current as DataRowView);
            if (orowProdLote == null) return resul;
            DataRow orowTrig = tabTriagMov.AsEnumerable().Where(row => (!row.IsNull("ID_LOTE"))
                      && row.Field<double>("ID_LOTE") == Convert.ToDouble(orowProdLote["ID_LOTE"])).FirstOrDefault();
            if (orowTrig != null)
            {
                MessageBox.Show("TRIAG_MOV não compativel com LOTE");
                return resul; 
            }
            DataTable adoClone = tabTriagMov.Clone();
            DataRow rowClone = adoClone.NewRow();
            foreach(DataColumn ocol in orowTrig.Table.Columns)
            {
                if ((ocol.ColumnName.ToUpper() == "ID") || (ocol.ColumnName.ToUpper() == "ID_LOTE")) continue;
                rowClone[ocol.ColumnName] = orowTrig[ocol.ColumnName];
            }
            rowClone["EDITADO"] = 0;
            adoClone.Rows.Add(rowClone);
            adoClone.AcceptChanges();
            List<string> lst = new List<string>();
            string  str = "SELECT * FROM VENDACALC WHERE ID_LOTE = " +
                 orowProdLote["ID_LOTE"].ToString() + " AND (kg_rat <> 0) ";
            lst.Add(str);
            str = "SELECT * FROM SOBRACALC WHERE ID_LOTE = " +
                 orowProdLote["ID_LOTE"].ToString() + " AND (kg_rat <> 0) ";
            lst.Add(str);
            bool resultado = await PegVendasSobras(adoClone, lst);

       

            /*
             * if (oProdLote.oTable.FieldByName('ID_LOTE').asInteger <>
    qrTRIAG_MOV.FieldByName('ID_LOTE').asInteger) then
  begin
    ShowMessage('TRIAG_MOV não compativel com LOTE');
    exit;
  end;

  adoClone := TADODataSet.Create(nil);
  adoClone.Clone(qrTRIAG_MOV);
  adoClone.recordSet := nil;
  adoClone.CreateDataset;

  oProdconf := Tconfigura_adoTable.Create;
  oProdconf.PegTable(oProdMov.oTable, 'PRODUTO');

  oloteconf := Tconfigura_adoTable.Create;
  oloteconf.PegTable(oProdLote.oTable, 'LOTE');

  osegueconf := Tconfigura_adoTable.Create;
  osegueconf.PegTable(oProdRec.oTable, 'SEGUE');

  oTrig_conf_tabela := Tconfigura_adoTable.Create();
  oTrig_conf_tabela.PegTable(qrTRIAG_MOV, 'TRIAG_MOV');

  oTrig_Vendas := Tconfigura_adoTable.Create();
  oTrig_Vendas.PegTable(oProdDestino.oTable, 'VENDACALC');
  oTrig_Sobra := Tconfigura_adoTable.Create();
  oTrig_Sobra.PegTable(oProdSobra.oTable, 'SOBRACALC');
  // altera o lote com o valor do triag_mov (que é o espelho do lote sem os ajustes)

  adoClone.Edit;
  for i := 0 to adoClone.Fields.Count - 1 do
  begin
    if (adoClone.Fields[i].DisplayName = 'ID') or
      (adoClone.Fields[i].DisplayName = 'ID_LOTE') then
      continue;
    if (qrTRIAG_MOV.FieldDefs.IndexOf(adoClone.Fields[i].DisplayName) < 0) then
      continue;
    adoClone.FieldByName(adoClone.Fields[i].DisplayName).Value :=
      qrTRIAG_MOV.FieldByName(adoClone.Fields[i].DisplayName).Value;
  end;
  adoClone.FieldByName('EDITADO').asInteger := 0;
  adoClone.Post;

  str := 'SELECT * FROM VENDACALC WHERE ID_LOTE = ' +
    oProdLote.oTable.FieldByName('ID_LOTE').asString + ' AND (kg_rat <> 0) ';
  Peg_Vendas_sobras(adoClone, str);

  str := 'SELECT * FROM SOBRACALC WHERE ID_LOTE = ' +
    oProdLote.oTable.FieldByName('ID_LOTE').asString + ' AND (kg_rat <> 0) ';
  Peg_Vendas_sobras(adoClone, str);
  //
  //
  try
    oAltere := TAltereLote.Create;
    oAltere.AltereLote_Um(adoClone, oProdLote.oTable, oProdMov.oTable,
      oProdRec.oTable, true);
    oAltere.Destroy;
  except
    ShowMessage('Erro ao Gravar');
  end;

  oTrig_conf_tabela.DevolveTable(qrTRIAG_MOV, 'TRIAG_MOV');

  oProdconf.DevolveTable(oProdMov.oTable, 'PRODUTO');
  oloteconf.DevolveTable(oProdLote.oTable, 'LOTE');
  osegueconf.DevolveTable(oProdRec.oTable, 'SEGUE');

  oTrig_Vendas.DevolveTable(oProdDestino.oTable, 'VENDACALC');
  oTrig_Sobra.DevolveTable(oProdSobra.oTable, 'SOBRACALC');

  with oProdLote do
    for i := 0 to Campos.Count - 1 do
      if Campos.Soma[i] then
        Campos.Total[i] := 0;

  with oProdLote do
  begin
    bMark := oTable.GetBookMark;
    oTable.DisableControls;
    tafterscroll := oTable.AfterScroll;
    oTable.AfterScroll := nil;
    oTable.first;
    while not oTable.eof do
    begin
      for i := 0 to Campos.Count - 1 do
      begin
        if Campos.Soma[i] then
          Campos.Total[i] := Campos.Total[i] + oTable.FieldByName
            (Campos.Strings[i]).asFloat;
      end;
      oTable.next;
    end;

    oTable.AfterScroll := tafterscroll;
    oTable.GotoBookMark(bMark);
    oTable.EnableControls;

    // frmFiltroNovoProd.tblAssocCalcFields(frmFiltroNovoProd.oProdLote.oTable);
    ColocaTotal;
  end;

  with oProdMov do
    for i := 0 to Campos.Count - 1 do
      if Campos.Soma[i] then
        Campos.Total[i] := 0;

  with oProdMov do
  begin
    oTable.DisableControls;
    bMark := oTable.GetBookMark;
    oTable.first;
    while not oTable.eof do
    begin
      for i := 0 to Campos.Count - 1 do
      begin
        if Campos.Soma[i] then
          Campos.Total[i] := Campos.Total[i] + oTable.FieldByName
            (Campos.Strings[i]).asFloat;
      end;
      oTable.next;
    end;
    oTable.EnableControls;
    oTable.first;
    oTable.GotoBookMark(bMark);
    ColocaTotal;
  end;

             */
            return resul;
        }






        /*
         * function TProdutoMethodo.Reconstrua_Editados: Boolean;
  function Rateie_Bulk_fino(adoClone: TADODataSet; odataset: TDataSet): Boolean;
  var
    desaprove, peq, mofo, Total, resto: double;
  begin
    adoClone.Edit;

    adoClone.FieldByName('BENEF').asFloat := adoClone.FieldByName('BENEF')
      .asFloat + odataset.FieldByName('KG_RAT').asFloat;
    adoClone.FieldByName('KG_ENT').asFloat := adoClone.FieldByName('KG_ENT')
      .asFloat + odataset.FieldByName('KG_RAT').asFloat;

    if (adoClone.FieldByName('DESAPROVE').asFloat = 0.0) and
      (adoClone.FieldByName('MOFO').asFloat = 0.0) then

      adoClone.FieldByName('PEQ').asFloat := adoClone.FieldByName('PEQ').asFloat
        + odataset.FieldByName('KG_RAT').asFloat
    else
    begin
      desaprove := adoClone.FieldByName('DESAPROVE').asFloat;
      peq := adoClone.FieldByName('peq').asFloat;
      mofo := adoClone.FieldByName('mofo').asFloat;

      Total := desaprove + peq + mofo;
      if Total <> 0 then
      begin
        desaprove := trunc((desaprove / Total) * odataset.FieldByName
          ('KG_RAT').asFloat);
        peq := trunc((peq / Total) * odataset.FieldByName('KG_RAT').asFloat);
        mofo := trunc((mofo / Total) * odataset.FieldByName('KG_RAT').asFloat);

        resto := odataset.FieldByName('KG_RAT').asFloat -
          (peq + mofo + desaprove);
        if (adoClone.FieldByName('DESAPROVE').asFloat >
          adoClone.FieldByName('MOFO').asFloat) and
          (adoClone.FieldByName('DESAPROVE').asFloat >
          adoClone.FieldByName('peq').asFloat) then
          desaprove := desaprove + resto
        else if (adoClone.FieldByName('peq').asFloat >
          adoClone.FieldByName('MOFO').asFloat) and
          (adoClone.FieldByName('peq').asFloat > adoClone.FieldByName
          ('DESAPROVE').asFloat) then
          peq := peq + resto
        else if (adoClone.FieldByName('MOFO').asFloat >
          adoClone.FieldByName('peq').asFloat) and
          (adoClone.FieldByName('MOFO').asFloat > adoClone.FieldByName
          ('DESAPROVE').asFloat) then
          mofo := mofo + resto;
      end;
      adoClone.FieldByName('DESAPROVE').asFloat :=
        adoClone.FieldByName('DESAPROVE').asFloat + desaprove;
      adoClone.FieldByName('PEQ').asFloat := adoClone.FieldByName('PEQ')
        .asFloat + peq;

      adoClone.FieldByName('MOFO').asFloat := adoClone.FieldByName('MOFO')
        .asFloat + mofo;
      if (peq + mofo) <> 0 then
      begin
        adoClone.FieldByName('APROVE').asFloat := adoClone.FieldByName('APROVE')
          .asFloat + peq + mofo;

      end;

      if (mofo) <> 0 then
      begin
        adoClone.FieldByName('BOM').asFloat := adoClone.FieldByName('BOM')
          .asFloat + mofo;

      end;

    end;
    adoClone.Post;

  end;

  function Peg_Vendas_sobras(adoClone: TADODataSet; str: string): Boolean;
  var
    adoCalc: TADODataSet;
  begin
    adoCalc := TADODataSet.Create(nil);
    adoCalc := DevolveQuery(str, dmadofinan.PATH_TRABALHO, '', adoCalc);
    if (adoCalc.RecordCount > 0) then
    begin
      while not adoCalc.eof do
      begin
        if (adoCalc.FieldByName('prod_tp').asInteger = 0) then
        // contrato de venda bulk
        begin
          if (adoCalc.FieldByName('PROD_TPLOT').asInteger = 0) then
          // Lote tipo bulk
          begin
            adoClone.Edit;

            adoClone.FieldByName('BENEF').asFloat :=
              adoClone.FieldByName('BENEF').asFloat +
              adoCalc.FieldByName('KG_RAT').asFloat;
            adoClone.FieldByName('KG_ENT').asFloat :=
              adoClone.FieldByName('KG_ENT').asFloat +
              adoCalc.FieldByName('KG_RAT').asFloat;
            adoClone.Post;
          end
          else
          // para contrato do tipo bulk porém com lote do tipo fino ou dalton (nao bulk)
          begin
            Rateie_Bulk_fino(adoClone, adoCalc);
          end;

        end
        else
        begin // não sendo bulk (fino ou dalton)
          adoClone.Edit;
          adoClone.FieldByName('BENEF').asFloat := adoClone.FieldByName('BENEF')
            .asFloat + adoCalc.FieldByName('KG_RAT').asFloat;
          adoClone.FieldByName('KG_ENT').asFloat :=
            adoClone.FieldByName('KG_ENT').asFloat +
            adoCalc.FieldByName('KG_RAT').asFloat;
          adoClone.FieldByName('APROVE').asFloat :=
            adoClone.FieldByName('APROVE').asFloat +
            adoCalc.FieldByName('KG_RAT').asFloat;
          adoClone.FieldByName('BOM').asFloat := adoClone.FieldByName('BOM')
            .asFloat + adoCalc.FieldByName('KG_RAT').asFloat;

          adoClone.Post;

        end;
        adoCalc.next;
      end;
      adoCalc.Destroy;

    end;

  end;

var
  i: integer;
  oProdconf, oloteconf, osegueconf: Tconfigura_adoTable;
  bMark: TBytes;
  tafterscroll: TDataSetNotifyEvent;
  oTrig_conf_tabela: Tconfigura_adoTable;
  oTrig_Sobra, oTrig_Vendas: Tconfigura_adoTable;
  oAltere: TAltereLote;
  str: string;
  // adoCalc: TDataSet;
  adoClone: TADODataSet;
begin

  if (oProdLote.oTable.FieldByName('ID_LOTE').asInteger <>
    qrTRIAG_MOV.FieldByName('ID_LOTE').asInteger) then
  begin
    ShowMessage('TRIAG_MOV não compativel com LOTE');
    exit;
  end;

  adoClone := TADODataSet.Create(nil);
  adoClone.Clone(qrTRIAG_MOV);
  adoClone.recordSet := nil;
  adoClone.CreateDataset;

  oProdconf := Tconfigura_adoTable.Create;
  oProdconf.PegTable(oProdMov.oTable, 'PRODUTO');

  oloteconf := Tconfigura_adoTable.Create;
  oloteconf.PegTable(oProdLote.oTable, 'LOTE');

  osegueconf := Tconfigura_adoTable.Create;
  osegueconf.PegTable(oProdRec.oTable, 'SEGUE');

  oTrig_conf_tabela := Tconfigura_adoTable.Create();
  oTrig_conf_tabela.PegTable(qrTRIAG_MOV, 'TRIAG_MOV');

  oTrig_Vendas := Tconfigura_adoTable.Create();
  oTrig_Vendas.PegTable(oProdDestino.oTable, 'VENDACALC');
  oTrig_Sobra := Tconfigura_adoTable.Create();
  oTrig_Sobra.PegTable(oProdSobra.oTable, 'SOBRACALC');
  // altera o lote com o valor do triag_mov (que é o espelho do lote sem os ajustes)

  adoClone.Edit;
  for i := 0 to adoClone.Fields.Count - 1 do
  begin
    if (adoClone.Fields[i].DisplayName = 'ID') or
      (adoClone.Fields[i].DisplayName = 'ID_LOTE') then
      continue;
    if (qrTRIAG_MOV.FieldDefs.IndexOf(adoClone.Fields[i].DisplayName) < 0) then
      continue;
    adoClone.FieldByName(adoClone.Fields[i].DisplayName).Value :=
      qrTRIAG_MOV.FieldByName(adoClone.Fields[i].DisplayName).Value;
  end;
  adoClone.FieldByName('EDITADO').asInteger := 0;
  adoClone.Post;

  str := 'SELECT * FROM VENDACALC WHERE ID_LOTE = ' +
    oProdLote.oTable.FieldByName('ID_LOTE').asString + ' AND (kg_rat <> 0) ';
  Peg_Vendas_sobras(adoClone, str);

  str := 'SELECT * FROM SOBRACALC WHERE ID_LOTE = ' +
    oProdLote.oTable.FieldByName('ID_LOTE').asString + ' AND (kg_rat <> 0) ';
  Peg_Vendas_sobras(adoClone, str);
  //
  //
  try
    oAltere := TAltereLote.Create;
    oAltere.AltereLote_Um(adoClone, oProdLote.oTable, oProdMov.oTable,
      oProdRec.oTable, true);
    oAltere.Destroy;
  except
    ShowMessage('Erro ao Gravar');
  end;

  oTrig_conf_tabela.DevolveTable(qrTRIAG_MOV, 'TRIAG_MOV');

  oProdconf.DevolveTable(oProdMov.oTable, 'PRODUTO');
  oloteconf.DevolveTable(oProdLote.oTable, 'LOTE');
  osegueconf.DevolveTable(oProdRec.oTable, 'SEGUE');

  oTrig_Vendas.DevolveTable(oProdDestino.oTable, 'VENDACALC');
  oTrig_Sobra.DevolveTable(oProdSobra.oTable, 'SOBRACALC');

  with oProdLote do
    for i := 0 to Campos.Count - 1 do
      if Campos.Soma[i] then
        Campos.Total[i] := 0;

  with oProdLote do
  begin
    bMark := oTable.GetBookMark;
    oTable.DisableControls;
    tafterscroll := oTable.AfterScroll;
    oTable.AfterScroll := nil;
    oTable.first;
    while not oTable.eof do
    begin
      for i := 0 to Campos.Count - 1 do
      begin
        if Campos.Soma[i] then
          Campos.Total[i] := Campos.Total[i] + oTable.FieldByName
            (Campos.Strings[i]).asFloat;
      end;
      oTable.next;
    end;

    oTable.AfterScroll := tafterscroll;
    oTable.GotoBookMark(bMark);
    oTable.EnableControls;

    // frmFiltroNovoProd.tblAssocCalcFields(frmFiltroNovoProd.oProdLote.oTable);
    ColocaTotal;
  end;

  with oProdMov do
    for i := 0 to Campos.Count - 1 do
      if Campos.Soma[i] then
        Campos.Total[i] := 0;

  with oProdMov do
  begin
    oTable.DisableControls;
    bMark := oTable.GetBookMark;
    oTable.first;
    while not oTable.eof do
    begin
      for i := 0 to Campos.Count - 1 do
      begin
        if Campos.Soma[i] then
          Campos.Total[i] := Campos.Total[i] + oTable.FieldByName
            (Campos.Strings[i]).asFloat;
      end;
      oTable.next;
    end;
    oTable.EnableControls;
    oTable.first;
    oTable.GotoBookMark(bMark);
    ColocaTotal;
  end;

end;

         */

    }
}
