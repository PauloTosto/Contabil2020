using ApoioContabilidade.Almoxarifado.ConfigComponentes;
using ApoioContabilidade.Almoxarifado.Model;
using ApoioContabilidade.Core;
using ApoioContabilidade.Fiscal.Comum;
using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ClassConexao;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Almoxarifado.ServicesAlmox
{
    public class ServAlmox
    {
        public Int32 RG_SOITENS;
        public Boolean CK_SETOREXCLUSIVE;
        public Boolean CK_SDONEGATIVO;
        public string STR_DEPOSITO;
        public Boolean CK_SUGERE;

        public DateTime INI_DTAPESQUISA;

        public DateTime UltimoPonto;
        public DateTime dataRef = Utils.dataRef; 
        public Int32 ANOS;

        public DateTime dtData;
       



        public string setor;
        public string deposito;


        
        
        public DataTable tabGeralCodNumMovest;
        public DataTable tabSaldoDeposito;
        public DataTable tabMestreVirtual;
        public DataTable tabDetalheVirtual;
        //public DataTable tabDetalheAnterior; // clone da dtDetalheVirtual
        public DataTable tabMaterialAI;
        //public DataTable tabCustoMedia;
        public DataTable tabMatGeral; //

        public DataTable tabSugere;

        // só irá servir quando o acesso for à SQL SERVER
        public DataTable tabAlmoxCustoMedio = new DataTable();

        public BindingSource bmCustoMedio = new BindingSource();
        public BindingSource bmGeralCodNumMovest = new BindingSource();
        public BindingSource bmSaldoDeposito = new BindingSource();
        public BindingSource bmMestre = new BindingSource();
        public BindingSource bmDetalhe = new BindingSource();

        public BindingSource bmSugere = new BindingSource();

        // OPÇÔES DO CLIENTE
        public bool checkAI;

        
        public FrmSaidaInsumosPonto frmSaida; 
        public ServAlmox(FrmSaidaInsumosPonto ofrmSaida)
        {
            frmSaida = ofrmSaida; 
            ANOS = 4;
            CK_SUGERE = true;
            setor = "";

            INI_DTAPESQUISA = DateTime.Now.AddYears(-ANOS);
                //;AnoaMenos(date, ANOS);

           CK_SDONEGATIVO = false;
           CK_SETOREXCLUSIVE = false;
           STR_DEPOSITO = "";
           RG_SOITENS = 0;
            PegDadosConfigurações();
        

        }
        public void PegDadosConfigurações()
        {
            Dictionary<string, object> ListaCaminhosXML;
            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + Configura_Almox_Saidas.fileConfigInsumosSaidas.ToString()))
            {
                ListaCaminhosXML = AcessosStream.LeiaListaInsumos(Environment.SpecialFolder.LocalApplicationData +
                    Configura_Almox_Saidas.fileConfigInsumosSaidas.ToString());
                Configura_Almox_Saidas.INS_CK_SDONEGATIVO = ListaCaminhosXML["INS_CK_SDONEGATIVO"];
                Configura_Almox_Saidas.INS_CK_SETOREXCLUSIVE = ListaCaminhosXML["INS_CK_SETOREXCLUSIVE"];
                Configura_Almox_Saidas.INS_CK_SUGERE = ListaCaminhosXML["INS_CK_SUGERE"];
                Configura_Almox_Saidas.INS_STR_DEPOSITO = ListaCaminhosXML["INS_STR_DEPOSITO"];
                Configura_Almox_Saidas.INS_INI_DTAPESQUISA = ListaCaminhosXML["INS_INI_DTAPESQUISA"];
            }
            else
            {
                ListaCaminhosXML = new Dictionary<string, object>();
                ListaCaminhosXML.Add("INS_CK_SDONEGATIVO", Configura_Almox_Saidas.INS_CK_SDONEGATIVO);
                ListaCaminhosXML.Add("INS_CK_SETOREXCLUSIVE", Configura_Almox_Saidas.INS_CK_SETOREXCLUSIVE);
                ListaCaminhosXML.Add("INS_CK_SUGERE", Configura_Almox_Saidas.INS_CK_SUGERE);
                ListaCaminhosXML.Add("INS_STR_DEPOSITO", Configura_Almox_Saidas.INS_STR_DEPOSITO);
                ListaCaminhosXML.Add("INS_INI_DTAPESQUISA", Configura_Almox_Saidas.INS_INI_DTAPESQUISA);

                try
                {
                    AcessosStream.WriteListaInsumos(Environment.SpecialFolder.LocalApplicationData + Configura_Almox_Saidas.fileConfigInsumosSaidas.ToString(),
                            ListaCaminhosXML);
                }
                catch (Exception)
                {

                    throw;
                }
            }

            CK_SUGERE = Convert.ToBoolean(Configura_Almox_Saidas.INS_CK_SUGERE);
            INI_DTAPESQUISA = Convert.ToDateTime(Configura_Almox_Saidas.INS_INI_DTAPESQUISA);
            CK_SDONEGATIVO = Convert.ToBoolean(Configura_Almox_Saidas.INS_CK_SDONEGATIVO);
            CK_SETOREXCLUSIVE = Convert.ToBoolean(Configura_Almox_Saidas.INS_CK_SETOREXCLUSIVE);
            STR_DEPOSITO = Configura_Almox_Saidas.INS_STR_DEPOSITO.ToString();
            RG_SOITENS = Convert.ToInt32(Configura_Almox_Saidas.RG_SOITENS);
        }
        public void GraveConfigurações()
        {

            Configura_Almox_Saidas.INS_CK_SUGERE = CK_SUGERE;
            Configura_Almox_Saidas.INS_INI_DTAPESQUISA = INI_DTAPESQUISA;
            Configura_Almox_Saidas.INS_CK_SDONEGATIVO = CK_SDONEGATIVO;
            Configura_Almox_Saidas.INS_CK_SETOREXCLUSIVE = CK_SETOREXCLUSIVE;
            Configura_Almox_Saidas.INS_STR_DEPOSITO = STR_DEPOSITO;
            Configura_Almox_Saidas.RG_SOITENS = RG_SOITENS;


            Dictionary<string, object> ListaCaminhosXML;
            ListaCaminhosXML = new Dictionary<string, object>();
            ListaCaminhosXML.Add("INS_CK_SDONEGATIVO", Configura_Almox_Saidas.INS_CK_SDONEGATIVO);
            ListaCaminhosXML.Add("INS_CK_SETOREXCLUSIVE", Configura_Almox_Saidas.INS_CK_SETOREXCLUSIVE);
            ListaCaminhosXML.Add("INS_CK_SUGERE", Configura_Almox_Saidas.INS_CK_SUGERE);
            ListaCaminhosXML.Add("INS_STR_DEPOSITO", Configura_Almox_Saidas.INS_STR_DEPOSITO);
            ListaCaminhosXML.Add("INS_INI_DTAPESQUISA", Configura_Almox_Saidas.INS_INI_DTAPESQUISA);

            if (File.Exists(Environment.SpecialFolder.LocalApplicationData + Configura_Almox_Saidas.fileConfigInsumosSaidas.ToString()))
            {
                try
                {
                    AcessosStream.WriteListaInsumos(Environment.SpecialFolder.LocalApplicationData + Configura_Almox_Saidas.fileConfigInsumosSaidas.ToString(),
                            ListaCaminhosXML);
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public async void BmMestre_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);
            string codSer = "";
            string setor = "";

            bool result = (registro != null);
            if (result)
            {
                if (registro.Row.RowState == DataRowState.Detached) return;
                if (registro.Row.RowState == DataRowState.Added)
                {
                    if ((!registro.Row.IsNull("CODSER")) || (!registro.Row.IsNull("SETOR")))
                    {
                        codSer = registro.Row["CODSER"].ToString().Trim();
                        setor = registro.Row["SETOR"].ToString().Trim();
                        if ((codSer == "") || (setor == "")) return;
                    }
                    else return;
                }
            }
            if (result) { result = (!registro.Row.IsNull("CODSER") && !registro.Row.IsNull("SETOR")); }
            if (result)
            {
                codSer = registro.Row["CODSER"].ToString().Trim();
                setor = registro.Row["SETOR"].ToString().Trim();

                try
                {   // MovTrab

                    var dado = tabDetalheVirtual.AsEnumerable().Where(row =>
                        (!row.IsNull("CODSER")) &&
                        (row.Field<string>("CODSER").Trim() == codSer) &&
                        (!row.IsNull("SETOR")) &&
                        (row.Field<string>("SETOR").Trim() == setor)
                        );
                    if ((dado != null))
                    { bmDetalhe.DataSource = dado.AsDataView(); }
                    else { bmDetalhe.DataSource = tabDetalheVirtual.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                }
                catch (Exception)
                {
                    bmDetalhe.DataSource = tabDetalheVirtual.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                }
            }
            else
            {
                bmDetalhe.DataSource = tabDetalheVirtual.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
            }
            BmDetalhe_PositionChanged(bmDetalhe, null);
            // SÓ ENTRA SE checkAI = true
            //sbGraveSugesta.Enabled := false;
            frmSaida.btnGraveSugesta.Enabled = false;
            //bmSugere.DataSource = tabSugere.AsDataView();
            tabSugere.Rows.Clear();
            if (frmSaida.ckAll.Checked)
            {
               

                if ((sender as BindingSource).Count > 0)
                {
                    //  Desmarque todos os CheckListSugere.CheckAll(cbUnchecked, true, false);

                    //if (tabMaterialAI.Rows.Count == 0)
                    bool pesquisaOk = await AIInsumos(codSer.PadLeft(3), setor.PadLeft(2)); //Pesquise
                    if (!pesquisaOk) tabMaterialAI.Rows.Clear();
                    if (tabMaterialAI.Rows.Count > 0)
                    {
                        foreach (DataRow orow in tabMaterialAI.AsEnumerable().Where(row => row.Field<string>("CODSER").Trim() == codSer.Trim()
                             && row.Field<string>("SETOR").Trim() == setor.Trim()))
                        {
                            DataRow rowNovo = tabSugere.NewRow();
                            rowNovo["COD"] = orow["COD"];
                            rowNovo["DEPOSITO"] = orow["DEPOSITO"];
                            rowNovo["UNID"] = orow["UNID"];
                            rowNovo["DESCRICAO"] = orow["DESCRICAO"];
                            rowNovo["SDO"] = Convert.ToDouble(orow["TOTQUANT"]);
                            rowNovo["CHECKED"] = false;
                            tabSugere.Rows.Add(rowNovo);
                            rowNovo.AcceptChanges();
                        }
                    }
                }
                if (tabSugere.Rows.Count > 0)
                    frmSaida.btnGraveSugesta.Enabled = true;


            }
            bmSugere.DataSource = tabSugere.AsDataView();
        }

        public void BmDetalhe_PositionChanged(object sender, EventArgs e)
        {
            if (frmSaida.comum.EdtDetalhe == null) return;
            DataRowView rowDetalhe = null;
            try
            {
                rowDetalhe = bmDetalhe.Current as DataRowView;
            }
            catch (Exception)
            {
            }
            string tcod = "";

            if (rowDetalhe != null)
                tcod = rowDetalhe["COD"].ToString();            
            DisplaySaldoItens((sender as BindingSource), tcod);

            if (frmSaida.comum.EdtDetalhe.oFormEdite.ContainsFocus)
            {
                /*ComboBox ocombo = (frmSaida.comum.EdtDetalhe.Linhas[2].oedite[1] as ComboBox);
                if ((sender as BindingSource).Count > 0)
                {
                    DataRowView orow = ((sender as BindingSource).Current as DataRowView);
                    Preencha_DBCBMAteriais(orow["DEPOSITO"].ToString());
                    string cod = orow["COD"].ToString();

                    int index = -1;
                    for (int i = 0; i < ocombo.Items.Count; i++)
                    {
                        if ((ocombo.Items[i] as DataRowView)["COD"].ToString() == cod.Trim())
                        {
                            index = i;
                            break;
                        }
                    }
                    ocombo.SelectedIndex = index;
                }
                else ocombo.Items.Clear();*/
            }
        }
        /*
         * if DataSet.RecordCount > 0 then
    begin

      Preencha_DBCBMAteriais(DataSet.FieldByName('DEPOSITO').AsString);

      index := IndexMateriais(DBCBItemsMat, DataSet.FieldByName('COD')
        .AsString);
      if index <> -1 then
      begin
        DBCBItemsMat.Text := DBCBItemsMat.Items[index];
        DBCBItemsMat.ItemIndex := index;
      end
      else
        DBCBItemsMat.ItemIndex := index;
    end
    else
    begin
      DBCBItemsMat.Clear;
    end;

        */
        public void DisplaySaldoItens(BindingSource bmDetalhe, string tcod)
        {
            if ((frmSaida.comum.EdtDetalhe == null) || (frmSaida.comum.EdtDetalhe.Linhas.Count < 3) ||
                  (frmSaida.comum.EdtDetalhe.Linhas[3].oedite[0] == null) ) return;
            DataRowView rowDetalhe = null;
            try
            {
                rowDetalhe = bmDetalhe.Current as DataRowView;
            }
            catch (Exception)
            {
            }
            if ((rowDetalhe == null) || (tcod == "") ) 
            {
                frmSaida.comum.EdtDetalhe.Linhas[3].oedite[0].Text = "vazio";
                return;
            }
            if ((bmDetalhe.Count > 0) && (tabSaldoDeposito != null))
            {
                DataRow rowpesquisa = tabSaldoDeposito.AsEnumerable().Where(row =>
                      (row.Field<string>("DEPOSITO").Trim() == rowDetalhe["DEPOSITO"].ToString().Trim())
                      && (row.Field<string>("COD").Trim() == tcod.Trim())).FirstOrDefault();
                if (rowpesquisa != null)
                {
                    frmSaida.comum.EdtDetalhe.Linhas[3].oedite[0].Text =
                        String.Format("{0:####,##0.0000}", Convert.ToDecimal(rowpesquisa["TOTQUANT"])) +
                          rowpesquisa["UNID"].ToString().ToLower();
                }
                else frmSaida.comum.EdtDetalhe.Linhas[3].oedite[0].Text = "0.0000";
            }
            else frmSaida.comum.EdtDetalhe.Linhas[3].oedite[0].Text = "0.0000";

        }
        private void  Preencha_DBCBMAteriais(string deposito)
        {
            /*frmSaida.comum.bmItensMateriais.DataSource = tabSaldoDeposito.AsEnumerable().
                           Where(row=> row.Field<string>("DEPOSITO").Trim() == deposito.Trim()).AsDataView();*/
        }
                
               



        private async Task<bool> AIInsumos(string codser, string setor)
        {
            // if (!MovEstStatic.MovEstOk()) return;
            DateTime dtLimite = frmSaida.dtLimite.Value;
            string str = "SELECT movest.deposito,movest.cod,movest.Setor," +
            " cadest.DESCRI as Descricao, MAX(movest.data) AS UltData, " +
             " COUNT(movest.cod) AS Ocorrencias " + " FROM  MOVEST, CADEST " +
            " WHERE        movest.cod = cadest.cod " + " AND (movest.codser = '" +
                codser + "') " +
                " AND (movest.setor = '" + setor + "')  AND " +
             " (movest.data > ctod('" + dtLimite.ToString("MM/dd/yyyy") + "'))" +
              " GROUP BY movest.deposito,movest.cod,  cadest.DESCRI ,movest.Setor " +
              " ORDER BY ultdata DESC, ocorrencias DESC";
            List<string> lst = new List<string>();
            lst.Add(str);
            DataTable dados = null;
            try
            {
                DataSet dsDados = await ApiServices.Api_QueryMulti(lst);

                if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
                // Tabela virtual

                dados = dsDados.Tables[0].Copy();

            }
            catch(Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
                // tabMatGeral

                /*  var dados = (from gr in MovEstStatic.MovEst().AsEnumerable().Where(row =>
                                     row.Field<DateTime>("DATA").CompareTo(dtLimite) > 0
                                  && row.Field<string>("CODSER").Trim() == codser.Trim()
                                   && row.Field<string>("SETOR").Trim() == setor.Trim())
                               group gr by new
                               {
                                   deposito = gr.Field<string>("DEPOSITO"),
                                   cod = gr.Field<string>("COD"),
                                   descri = gr.Field<string>("DESCRICAO"),
                                   setor = gr.Field<string>("SETOR"),
                                   codser = gr.Field<string>("CODSER"),
                               } into g
                               select new
                               {
                                   deposito = g.Key.deposito,
                                   cod = g.Key.cod,
                                   descricao = g.Key.descri,
                                   setor = g.Key.setor,
                                   codser = g.Key.codser,
                                   ultData = g.Max(gr => gr.Field<DateTime>("DATA")),
                                   ocorrencias = g.Count(),
                               });

                  */
            tabMaterialAI.Rows.Clear();
            int id = 0;
            foreach (var dado in dados.AsEnumerable().OrderByDescending(s => s.Field<DateTime>("ultData")).OrderByDescending(s => s.Field<Int64>("ocorrencias")))
            {
                DataRow orow = tabMaterialAI.NewRow();
                id++;
                orow["ID"] = id;
                orow["COD"] = dado.Field<string>("COD");
                orow["DESCRICAO"] = dado.Field<string>("DESCRICAO");
                orow["SETOR"] = dado.Field<string>("SETOR");
                orow["DEPOSITO"] = dado.Field<string>("DEPOSITO");
                orow["CODSER"] = codser;// dado.Field<string>("CODSER"); 
                orow["OCORRENCIAS"] = Convert.ToInt32(dado.Field<Int64>("OCORRENCIAS")); 
                orow["ULTDATA"] = dado.Field<DateTime>("ultData");
                orow["TOTQUANT"] = 0;
                orow["UNID"] = "";

                try
                {
                    DataRow rowpesquisa = tabSaldoDeposito.AsEnumerable().Where(row =>
                     (row.Field<string>("DEPOSITO").Trim() == dado.Field<string>("DEPOSITO").Trim())
                     && (row.Field<string>("COD").Trim() == dado.Field<string>("COD").Trim())).FirstOrDefault();
                    if (rowpesquisa != null)
                    {
                        orow["TOTQUANT"] = rowpesquisa["TOTQUANT"];
                        orow["UNID"] = rowpesquisa["UNID"];
                    }
                }
                catch (Exception)  {
                    return false;                
                }
                tabMaterialAI.Rows.Add(orow);
            }
            tabMaterialAI.AcceptChanges();

            return true;
            /*
            */


        }

        public void TabDetalhe_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DataRow orow = null;
            try
            {
                if (bmMestre.Count > 0)
                {
                    orow = ((DataRowView)bmMestre.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Trabalhador??");
                throw;
            }
            if (orow == null) return;
            if (!orow.IsNull("DATAOK"))
                e.Row["DATA"] = orow["DATAOK"];
            else if (!orow.IsNull("FIM"))
                e.Row["DATA"] = orow["FIM"];
            else
                e.Row["DATA"] = dtData;
            e.Row["SETOR"] = orow["SETOR"];
            e.Row["CODSER"] = orow["CODSER"];
            e.Row["TIPO"] = "S";
            e.Row["QUANT"] = 0;
            if ((deposito != null) && (deposito.Trim() != ""))
            {
                string[] depos = deposito.Split(Convert.ToChar("/")).ToArray();
                if (depos.Length > 0)
                {
                    e.Row["DEPOSITO"] = depos[0];
                }
            }
            else
                e.Row["DEPOSITO"] = "";
        }
        public async void EdtDetalhe_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if ((Convert.ToDouble(orow["QUANT"]) <= 0)
                    || (orow["GLEBA"].ToString().Trim() == "")
                    || (orow["NUM_MOD"].ToString().Trim() == "")
                    || (orow["CODSER"].ToString().Trim() == "")
                    || (orow["COD"].ToString().Trim() == "")
                     || (orow.IsNull("DATA")))
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
            string message_data = "";
            if ((Convert.ToDateTime(orow["DATA"]).CompareTo(new DateTime(2000, 01, 01)) <= 0) ||
                 (Convert.ToDateTime(orow["DATA"]).CompareTo(DateTime.Now.AddDays(100)) > 0) )
            {
                MessageBox.Show("Data ???");
                e.Cancela = true;
                return;
            }
            string confirmeString = "";
            try
            {

                bool validequant = await ValideQuant(orow);
                if (validequant == false)
                {
                    confirmeString = "Sdo Insumo Negativo!!!";
                    MessageBox.Show(confirmeString);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Erro ao testar Quant");
                e.Cancela = true;
                return;
            }

            string tsetor = "";
            try
            {
                tsetor = TabelasIniciais.SetorAtual_PelaData(orow["GLEBA"].ToString(), dtData);

            }
            catch (Exception)
            {
                tsetor = "";
            }
            if (tsetor == "")
            {
                MessageBox.Show("Falhou em acessar SETOR a partir da GLEBA");
                e.Cancela = true;
                return;
            }

            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
            {


            }
            orow["TIPO"] = "S";
            orow["SETOR"] = tsetor;
            DataRow rowCME = null;
            if (tabAlmoxCustoMedio.Rows.Count > 0)
            {
                rowCME = tabAlmoxCustoMedio.AsEnumerable().Where(row =>
                     (row.Field<DateTime>("INICIO").CompareTo(Convert.ToDateTime(orow["DATA"])) >= 0)
                    && (row.Field<DateTime>("FIM").CompareTo(Convert.ToDateTime(orow["DATA"])) <= 0)
                ).FirstOrDefault();
            }
            if (rowCME == null) MessageBox.Show("Não foi possivel Calcular Custo medio deste item");
            orow["PUNIT"] = rowCME["CUSTOMEDIO"];
            orow["VALOR"] = Math.Round(Convert.ToDouble(rowCME["CUSTOMEDIO"]) * Convert.ToDouble(orow["QUANT"]), 2);
        }

        








        public async void EdtDetalhe_AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((Convert.ToDouble(orow["QUANT"]) <= 0)
                    || (orow["GLEBA"].ToString().Trim() == "")
                    || (orow["NUM_MOD"].ToString().Trim() == "")
                    || (orow["CODSER"].ToString().Trim() == "")
                    || (orow["COD"].ToString().Trim() == "")
                     || (orow.IsNull("DATA")))

            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmMestre.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao Mestre");
                e.Cancela = true;
                return;
            }

            PonhaDefaultValores(orow);
           /* foreach (DataColumn col in orow.Table.Columns)
            {
                if (orow[col.ColumnName] is System.DBNull)
                {
                    orow[col.ColumnName] = col.DefaultValue;
                }
            }*/
        

            List<string> oExclua = new List<string>();
            oExclua.Add("UNID_COD");
            oExclua.Add("FALSO_INC");
            oExclua.Add("DESCRICAO");
            oExclua.Add("UNID");
            DataRowState estado = e.TipoMuda;
            bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "MOVEST",oExclua); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));

            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Inserir Registro MOVEST"); return;

            }


            try
            {
                // oexclua.Add("ADMI");

                // pARA MANTER a compatibilidade ENTRE O TABMOVEST carregado EM CACHE E O DO servidor NO CASO ATUAL DE INSTANCIAS UNICAS,
                // ATUALIZE TAMBÉM O VIRTUAL
                
                //bool result = MovEstStatic.AltereMovest(orow, estado);
                //if (!result) MessageBox.Show("Ataulização de Movest Static falhou");

                orow["FALSO_INC"] = 1;
                orow.AcceptChanges();
            }
            catch (Exception E)
            {
                e.Cancela = true;
                MessageBox.Show("Operação Falhou: " + E.ToString());
                
                return;
            }
            return;
        }

        public async void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            bool ok = false;
            try
            {
                Int64 id = Convert.ToInt64(orow["ID"]);
                ok = await Prepara_Sql.OpereDeleteRegistroServidorAsync(orow, "MOVEST");
                if (ok)
                {
                   // bool result = MovEstStatic.AltereMovest(orow, DataRowState.Deleted, id);
                   // if (!result) MessageBox.Show("Exclusão de Movest Static falhou");

                }
            }
            catch (Exception)
            {
            }
            // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Deletar Registro " + "MOVEST"); return;
            }
        }



        // EditaAlmox_Validating



        private void PonhaDefaultValores(DataRow orow)
        {

            foreach (DataColumn col in orow.Table.Columns)
            {
                if (orow[col.ColumnName] is System.DBNull)
                {
                    if (col.DataType == Type.GetType("System.DateTime"))
                        orow[col.ColumnName] = DateTime.MinValue;
                    else if (col.DataType == Type.GetType("System.String"))
                        orow[col.ColumnName] = "";
                    else if (col.DataType == Type.GetType("System.Boolean"))
                        orow[col.ColumnName] = 0;
                    else orow[col.ColumnName] = 0;
                }
            }

        }

        private async Task<bool> ValideQuant(DataRow orow)
        {
            bool result = false;
            string deposito = orow["DEPOSITO"].ToString();
            string cod = orow["COD"].ToString();
            double quant_ant = 0;
            if (!((orow.RowState == DataRowState.Detached) || (orow.RowState == DataRowState.Added)))
            {
                // se for uma edição (não é um novo)
                string depOrigem = orow["DEPOSITO", DataRowVersion.Original].ToString();
                string codOrigem = orow["COD", DataRowVersion.Original].ToString();
                if ((depOrigem == deposito) && (codOrigem == cod))
                    quant_ant = Convert.ToDouble(orow["QUANT", DataRowVersion.Original]);
            }
            string str = "SELECT movest.deposito, movest.cod ," +
         " SUM(iif((tipo='E'),movest.quant,movest.quant*-1)) AS totquant "
         + " FROM movest  where  " + " (deposito = '" + deposito + "') and "
         + " (Cod = '" + cod + "') and "
           + " movest.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') "
        + " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
         "GROUP BY deposito, movest.cod";
            List<string> lstStr = new List<string>();
            lstStr.Add(str);
            DataSet dsDados = await ApiServices.Api_QueryMulti(lstStr);


            if ((dsDados == null) || (dsDados.Tables.Count == 0)) return result;
            double valor = Convert.ToDouble(orow["QUANT"]);
            if (valor == 0) return false;



            if (dsDados.Tables[0].Rows.Count > 0)
            {
                DataRow rowResult = dsDados.Tables[0].Rows[0];
                if (valor > (Convert.ToDouble(rowResult["TOTQUANT"]) + quant_ant))
                {
                    result = false;
                }
                else result = true;
            }
            else
            {
                MessageBox.Show("Não Existe Saldo neste Deposito");
                result = false;
            }

            return result;
        }


        public async void Valide_Valor(object sender, CancelEventArgs e)
        {

            if (frmSaida.ckSdoQuant.Checked) return;
            DataRowView orow = (bmDetalhe.Current as DataRowView);
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
           + " movest.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') "
        + " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
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
            }
        } 

        // 
        public void GraveDetalheSugere()
        {
            // pega da tabela sugere o deposito e o cod

            List<DataRow> lstSugere = null;
            try
            {
                lstSugere = tabSugere.AsEnumerable().Where(row => row.Field<bool>("CHECKED") == true).ToList<DataRow>();
            }
            catch (Exception)
            {
            }
            if (lstSugere == null) return;



            if (lstSugere.Count < 2)
            {
                string tcod = lstSugere[0]["COD"].ToString();
                string tdeposito = lstSugere[0]["DEPOSITO"].ToString();

                foreach (DataRow orow in tabDetalheVirtual.AsEnumerable().Where(row => (row.Field<Int32>("FALSO_INC") != 1)
                                      && (row.Field<double>("QUANT") == 0)
                                                     ).OrderBy(row => row.Field<string>("GLEBA")
                                                     ).ThenBy(row => row.Field<string>("QUADRA")))
                {
                    
                    orow.BeginEdit();
                    orow["DEPOSITO"] = tdeposito; // 
                    orow["COD"] = tcod;
                    orow.EndEdit();
                    
                }
            }
            else
            {
                var gle_qua_num = (from gr in tabDetalheVirtual.AsEnumerable().Where(row => (row.Field<Int32>("FALSO_INC") != 1)
                                       && (row.Field<double>("QUANT") == 0) )
                                        group gr by new
                                        {
                                            gleba = gr.Field<string>("GLEBA"),
                                            quadra = gr.Field<string>("QUADRA"),
                                            num_mod = gr.Field<string>("NUM_MOD")
                                        } into g
                                              select new
                                              {
                                                  gleba = g.Key.gleba,
                                                  quadra = g.Key.quadra,
                                                  num_mod = g.Key.num_mod,
                                                  data = g.Max(row=>row.Field<DateTime>("DATA"))
                                              });

                foreach (var obj in gle_qua_num)
                {
                    List<DataRow> 
                    lstRows = tabDetalheVirtual.AsEnumerable().Where(row =>
                                     (row.Field<Int32>("FALSO_INC") != 1)
                                    && (row.Field<double>("QUANT") == 0)
                                   && (row.Field<string>("GLEBA").Trim() == obj.gleba.Trim())
                                    && (row.Field<string>("QUADRA").Trim() == obj.quadra.Trim())
                                     && (row.Field<string>("NUM_MOD").Trim() == obj.num_mod.Trim())).ToList<DataRow>();
                    for (int i = 0; i < lstSugere.Count; i++)
                    {
                        DataRow orowSugere = lstSugere[i];
                        if (i < lstRows.Count)
                        {
                            DataRow orowDetalhe = lstRows[i];
                            orowDetalhe.BeginEdit();
                            orowDetalhe["DEPOSITO"] = orowSugere["DEPOSITO"];
                            orowDetalhe["COD"] = orowSugere["COD"];
                            orowDetalhe.EndEdit();
                            orowDetalhe.AcceptChanges();
                        }
                        else
                        {
                            DataRow orowDetalhe = tabDetalheVirtual.NewRow();
                            orowDetalhe["DEPOSITO"] = orowSugere["DEPOSITO"];
                            orowDetalhe["COD"] = orowSugere["COD"];
                            orowDetalhe["GLEBA"] = obj.gleba;
                            orowDetalhe["QUADRA"] = obj.quadra;
                            orowDetalhe["NUM_MOD"] = obj.num_mod;
                            orowDetalhe["DATA"] = obj.data;
                            tabDetalheVirtual.Rows.Add(orowDetalhe);
                            orowDetalhe.AcceptChanges();
                        }
                    }
                }
            }
            // Lê da tabela sugere todos os selecionados 
        }
        public void ConstruaMaterialAI()
        {
            
            tabMaterialAI = new DataTable();
            tabMaterialAI.TableName = "MaterialAI";
            tabMaterialAI.Columns.Add("ID", Type.GetType("System.Int32"));

            tabMaterialAI.Columns.Add("NUM_MOD", Type.GetType("System.String"));
            tabMaterialAI.Columns["NUM_MOD"].MaxLength = 2;
            tabMaterialAI.Columns.Add("CODSER", Type.GetType("System.String"));
            tabMaterialAI.Columns["CODSER"].MaxLength = 3;
            tabMaterialAI.Columns.Add("COD", Type.GetType("System.String"));
            tabMaterialAI.Columns["COD"].MaxLength = 4;

            tabMaterialAI.Columns.Add("SETOR", Type.GetType("System.String"));
            tabMaterialAI.Columns["SETOR"].MaxLength = 2;
            tabMaterialAI.Columns.Add("DEPOSITO", Type.GetType("System.String"));
            tabMaterialAI.Columns["DEPOSITO"].MaxLength = 2;

            tabMaterialAI.Columns.Add("UNID", Type.GetType("System.String"));
            tabMaterialAI.Columns["UNID"].MaxLength = 7;

            tabMaterialAI.Columns.Add("TOTQUANT", Type.GetType("System.Double"));
            tabMaterialAI.Columns.Add("ULTDATA", Type.GetType("System.DateTime"));

            tabMaterialAI.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            tabMaterialAI.Columns["DESCRICAO"].MaxLength = 40;

            tabMaterialAI.Columns.Add("OCORRENCIAS", Type.GetType("System.Int32"));

        }
        public void ConstruaSugere()
        {

            tabSugere = new DataTable();
            tabSugere.TableName = "SUGERE";
            tabSugere.Columns.Add("ID", Type.GetType("System.Int32"));
            tabSugere.Columns.Add("CHECKED", Type.GetType("System.Boolean"));
            tabSugere.Columns.Add("COD", Type.GetType("System.String"));
            tabSugere.Columns["COD"].MaxLength = 4;
            tabSugere.Columns.Add("DEPOSITO", Type.GetType("System.String"));
            tabSugere.Columns["DEPOSITO"].MaxLength = 3;
            tabSugere.Columns.Add("UNID", Type.GetType("System.String"));
            tabSugere.Columns["UNID"].MaxLength = 7;
         
            tabSugere.Columns.Add("ULTDATA", Type.GetType("System.DateTime"));

            tabSugere.Columns.Add("DESCRICAO", Type.GetType("System.String"));
            tabSugere.Columns["DESCRICAO"].MaxLength = 40;

            tabSugere.Columns.Add("OCORRENCIAS", Type.GetType("System.Int32"));
            tabSugere.Columns.Add("SDO", Type.GetType("System.Double"));
        }

        public async Task<DataTable> IncluaNovos()
        {

            DataTable adoConsulta = new DataTable();

            string minData = "MIN(iif(dia1 <> '',data,iif(dia2 <> '',data+1,iif(dia3 <> '',data+2,"
                            + " iif((dia4 <> '' and dia4 <> 'R') ,data+3, " +
                            " iif(dia5 <> '',data+4,iif(dia6 <> '',data+5,iif(dia7 <> '',data+6,DATA)))))))) as INICIO ,";

            string maxData =
              "MAX(iif(dia7 <> '',data+6,iif(dia6 <> '',data+5,iif(dia5 <> '',data+4,"
              + "iif((dia4 <> '' and dia4 <> 'R'),data+3,"
              + "iif(dia3 <> '',data+2,iif(dia2 <> '',data+1,data))))))) as FIM , ";

            string okData = "MAX(iif(ok='X',iif(dia7 <> '',data+6,iif(dia6 <> '',data+5,"
                  + "iif(dia5 <> '',data+4,iif((dia4 <> '' and dia4 <> 'R'),"
              + "data+3,iif(dia3 <> '',data+2,iif(dia2 <> '',data+1,data))))) ),CTOD('')  )) " +
             " as DATAOK,";

            string horasefetivas =
              " SUM (iif(dia1<>'', (Select HORAS from cltcodig where (dia1 = indcod)),0.0 ) + "
              + " iif(dia2<>'', (Select HORAS from cltcodig where (dia2 = indcod)),0.0 ) + "
              + " iif(dia3<>'',(Select HORAS from cltcodig where (dia3 = indcod)),0.0    ) + "
              + " iif(dia4<>'',(Select HORAS from cltcodig where (dia4 = indcod)),0.0    ) + "
              + " iif(dia5<>'',(Select HORAS from cltcodig where (dia5 = indcod)),0.0    ) +  "
              + " iif(dia6<>'',(Select HORAS from cltcodig where (dia6 = indcod)),0.0    ) + "
              + " iif(dia7<>'',(Select HORAS from cltcodig where (dia7 = indcod)),0.0    )) as horas_efetivas ";
            try
            {
                DataSet dsResult = null;
                if (SqlServer_VFP_Config.tipoAcesso != 1)
                {
                    string str;
                    if (frmSaida.numericAnos.Value == 1)
                    {
                        str = "SELECT distinct num_mod, modelo.DESCRI as model , codser ,servic.DESCRI as serv, "
                        + " SETOR, FAZMOV as GLEBA, BL as QUADRA , 0 as FICA, ' ' as MUDOU," +
                         minData + maxData + okData + " MAX(OK) as OK, " + horasefetivas +
                        " FROM cltponto, modelo, servic " +
                           " WHERE    (modelo.num = num_mod) and (modelo.DESCRI <> '') and " +
                         " (servic.Cod = codser) " +
                         " and movest.DATA >= CTOD('" + dtData.ToString("MM/dd/yyyy") + "') " +
                        " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
                         " and setor = '" + setor + "' " +
                         " GROUP BY NUM_MOD,model,codser ,serv,SETOR, gleba,quadra,FICA,mudou  "
                         + " order by num_mod,codser";
                    }
                    else
                    {
                        str = "SELECT distinct DATA, num_mod, modelo.DESCRI as model , codser ,servic.DESCRI as serv, "
                        + " SETOR, FAZMOV as GLEBA, BL as QUADRA , 0 as FICA, ' ' as MUDOU," +
                           minData + maxData + okData + " MAX(OK) as OK , " + horasefetivas +
                         " FROM cltponto, modelo, servic " +
                         " WHERE    (modelo.num = num_mod) and (modelo.DESCRI <> '') and " +
                         " (servic.Cod = codser) " +
                             " and movest.DATA >= CTOD('" + dtData.ToString("MM/dd/yyyy") + "') " +
                            " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
                             " and setor = '" + setor + "' " +
                           " GROUP BY DATA,NUM_MOD,model,codser ,serv,SETOR, gleba,quadra,FICA,mudou  "
                            + " order by num_mod,codser";

                    }
                    List<string> lstquery = new List<string>();
                    lstquery.Add(str);
                    try
                    {
                        dsResult = await ApiServices.Api_QueryMulti(lstquery);
                    }
                    catch (Exception)
                    {
                    }
                }
                else
                {
                    List<string> lstquery = new List<string>();
                    lstquery.Add("inicio=" + dtData.ToString("yyyy-MM-dd"));
                    lstquery.Add("fim=" + UltimoPonto.ToString("yyyy-MM-dd"));
                    lstquery.Add("setor=" + setor);
                    lstquery.Add("sp_numero=" + "214");
                    try
                    {
                        dsResult = await ApiServices.Api_QuerySP(lstquery);
                    }
                    catch (Exception) { }
                }
                if ((dsResult == null) || (dsResult.Tables.Count == 0)) return adoConsulta;

                adoConsulta = dsResult.Tables[0].Copy();
                adoConsulta.TableName = "Consulta";
                // Delete todos cujas Horas Efetivas == 0
                List<DataRow> lstConsultaDel = adoConsulta.AsEnumerable().Where(row => row.Field<double>("HORAS_EFETIVAS") == 0).ToList<DataRow>();
                adoConsulta.AcceptChanges();
                // Delete todos cujas gleba, quadra ou num_mod coincidem
                foreach (DataRow rowDetalhe in tabDetalheVirtual.Rows )
                {
                    lstConsultaDel.Clear();
                    string quadra = rowDetalhe["QUADRA"].ToString().Trim();
                    string gleba = rowDetalhe["GLEBA"].ToString().Trim();
                    string num_mod = rowDetalhe["NUM_MOD"].ToString().Trim();
                    string codser = rowDetalhe["CODSER"].ToString().Trim();
                    lstConsultaDel = adoConsulta.AsEnumerable().Where(row => 
                                                  ( quadra== "" ? (row.IsNull("QUADRA") || (row.Field<string>("QUADRA").Trim() == quadra) )
                                                  : (row.Field<string>("QUADRA").Trim() == quadra) )
                                                   && (row.Field<string>("GLEBA").Trim() == gleba)
                                                   && (row.Field<string>("NUM_MOD").Trim() == num_mod)
                                                    && (row.Field<string>("CODSER").Trim() == codser)
                                                    ).ToList<DataRow>();
                    foreach (DataRow orow in lstConsultaDel)
                    {
                        orow.Delete();
                    }
                    adoConsulta.AcceptChanges();
                }
                if ((frmSaida.rbItensNoUsuais.Checked) || (frmSaida.rbItensUsuais.Checked))
                {
                    foreach (DataRow orow in tabGeralCodNumMovest.Rows)
                    {
                        string codser = orow["CODSER"].ToString().Trim();
                        foreach (DataRow rowCons in adoConsulta.AsEnumerable().Where(
                                       row => (row.Field<string>("CODSER").Trim() == codser)))
                        {
                            rowCons.BeginEdit();
                            rowCons["FICA"] = 1;
                            rowCons.EndEdit();
                            rowCons.AcceptChanges();
                        }
                    }
                    lstConsultaDel.Clear();
                    if (frmSaida.rbItensUsuais.Checked)
                    {

                        lstConsultaDel = adoConsulta.AsEnumerable().Where(row => 
                                                  (row.Field<Int64>("FICA") == 0)
                                                  ).ToList<DataRow>();
                    }
                    else
                    {
                        lstConsultaDel = adoConsulta.AsEnumerable().Where(row =>
                                                (row.Field<Int64>("FICA") == 0)
                                                ).ToList<DataRow>();
                    }
                    foreach (DataRow orow in lstConsultaDel)
                    {
                        orow.Delete();
                    }
                    adoConsulta.AcceptChanges();
                }
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
            }
            return adoConsulta;
        }

       




       // public bool Carregue_Deposito(string deposito)
       // {
            /*bool result = false;
            if (deposito == "")
            {
                frmSaida.comum.bmArmazem.DataSource =  DadosComum.ArmazemCombo.Copy().AsDataView();
                return true;
            }
            else
            {
                string[] depos = deposito.Split(Convert.ToChar("/")).ToArray();
                for (int i = 0; i < depos.Length; i++)
                {
                    depos[i] = depos[i].Trim();
                }

                frmSaida.comum.bmArmazem.DataSource = DadosComum.ArmazemCombo.AsEnumerable().Where(row =>
                                (row.Field<string>("DEPOSITO").Trim() != "")
                          && depos.Contains(row.Field<string>("DEPOSITO").Trim())).AsDataView();
            }
            if (frmSaida.comum.bmArmazem.Count == 0)
                return result;
            if ((setor.Trim() != "") && (tabDetalheVirtual != null) && (tabDetalheVirtual.Rows.Count > 0 ))
            {
                string[] depos = (from gr in tabDetalheVirtual.AsEnumerable().Where(row=>
                                  row.Field<string>("DEPOSITO").Trim() != ""
                                 && row.Field<string>("SETOR").Trim() == setor.Trim())
                                group gr by new
                                {
                                    deposito = gr.Field<string>("DEPOSITO").Trim(),
                               
                                } into g
                                select g.Key.deposito
                                ).ToArray();
                frmSaida.comum.bmArmazem.DataSource = DadosComum.ArmazemCombo.AsEnumerable().Where(row =>
                                (row.Field<string>("DEPOSITO").Trim() != "")
                          && depos.Contains(row.Field<string>("DEPOSITO").Trim())).AsDataView();
            }
            if (frmSaida.comum.bmArmazem.Count > 0)
                result = true;
            return result;*/
        //}


        public async void Recomponha_tabGeralCodNumMovest(string setorescolha)
        {
           // if (!MovEstStatic.MovEstOk()) return;
            DateTime dataInicio = frmSaida.dtLimite.Value;
          
            string str = "SELECT CODSER , SERVIC.DESCRI as SERV FROM MOVEST, SERVIC "
                    + "WHERE "
                    + (setor.Trim() == "" ? "" : " (SETOR = '" + setor.PadLeft(2) + "') AND ") 
                    +" (CODSER = SERVIC.COD) AND(MOVEST.TIPO = 'S') "
                    + " AND(MOVEST.TIPO2 <> 'T') "
                    + "AND (MOVEST.DATA > CTOD('"+ dataInicio.ToString("MM/dd/yyyy") + "')) "
                    + "GROUP BY CODSER , SERVIC.DESCRI   ORDER BY CODSER";
            List<string> lst = new List<string>(); 
            lst.Add(str);
            DataSet dsDados = null;
            try
            {
                dsDados = await ApiServices.Api_QueryMulti(lst);
            }
            catch (Exception)
            {
                return ;
            }


            if ((dsDados == null) || (dsDados.Tables.Count == 0)) return;
            tabGeralCodNumMovest = dsDados.Tables[0].Copy();
            tabGeralCodNumMovest.TableName = "GERALCODNUMMOVEST";

           // tabGeralCodNumMovest.Columns.Add("CODSER", Type.GetType("System.String"));
            tabGeralCodNumMovest.Columns["CODSER"].MaxLength = 4;
           // tabGeralCodNumMovest.Columns.Add("SERV", Type.GetType("System.String"));
            tabGeralCodNumMovest.Columns["SERV"].MaxLength = 25;



           
        }



        


        #region Chamadas nas Consultas (direto noServidor)

      
        // OUTRO FILTRO BASEADO NA DISPONIBILIDADE DO MOVESTSTATIC

        public async Task<bool> FiltroAlmoxarifado_BaseMovestStatic()
        {
          //  if (!MovEstStatic.MovEstOk()) return false;
            
            bool result = true;
            ConstruaSugere();
            ConstruaMaterialAI();

            setor = setor.Trim().PadLeft(2);
            try
            {
                try
                {
                    List<string> lstString = new List<string>(); 
                    string str = "SELECT Movest.*, ALLTRIM(CADEST.DESCRI) as DESCRICAO, CADEST.UNID  " +
                       " FROM movest, CADEST WHERE " +
                       " movest.cod = cadest.cod AND " +
                   " MOVEST.DATA >= CTOD('" + dtData.ToString("MM/dd/yyyy") + "') " +
                       " AND MOVEST.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
                   "  ORDER BY movest.cod ";
                    lstString.Add(str);

                    
                    str = "SELECT movest.deposito, movest.cod ,alltrim(cadest.DESCRI) as descricao, cadest.unid,"
                      + " SUM(iif((tipo='E'),movest.quant,movest.quant*-1)) AS totquant " +
                     " FROM movest,cadest  where (movest.cod = cadest.cod) and " +
                    " movest.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') " +
                    " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
                   " GROUP BY deposito, movest.cod,cadest.DESCRI,cadest.unid  ORDER BY deposito, movest.cod ";
                    lstString.Add(str);

                    str = "SELECT * FROM ALMOXCUSTOMEDIO ORDER BY COD, INICIO";
                    lstString.Add(str);


                    try
                    {
                        DataSet dsDados = await ApiServices.Api_QueryMulti(lstString);

                        if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
                        // Tabela virtual

                        tabMatGeral = dsDados.Tables[0].Copy();
                        tabMatGeral.TableName = "MOVESTGERAL";
                        tabMatGeral.Columns["DESCRICAO"].MaxLength = 40;
                        tabSaldoDeposito = dsDados.Tables[1].Copy();
                        tabSaldoDeposito.TableName = "SALDODEPOSITO";

                        tabAlmoxCustoMedio = dsDados.Tables[2].Copy();
                        tabAlmoxCustoMedio.TableName = "ALMOXCUSTOMEDIO";
                    }
                    catch (Exception E)
                    { }

                }
                catch (Exception E)
                {
                    return false;
                }

                var servicos = (from gr in DadosComum.ServicoModeloCombo.Copy().AsEnumerable()
                                group gr by new
                                {
                                    codserv = gr.Field<string>("CODSER"),
                                    servic = gr.Field<string>("DESCSERV")
                                } into g
                                select new
                                {
                                    cod = g.Key.codserv,
                                    servic = g.Key.servic,
                                });


                Recomponha_tabGeralCodNumMovest("");


               
                var mestreVirtual =
                    (from gr in tabMatGeral.AsEnumerable()
                                         .Where(row =>
                        (row.Field<string>("TIPO2").Trim() != "T")
                        && row.Field<string>("TIPO").Trim() == "S"
                        && row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                        && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                         && row.Field<string>("SETOR").Trim() == setor.Trim()
                        )
                     join serv in servicos on gr.Field<string>("CODSER").Trim() equals serv.cod
                       into ps
                     from serv in ps.DefaultIfEmpty()


                     group gr by new
                     {
                         setor = gr.Field<string>("SETOR"),
                         codser = gr.Field<string>("CODSER"),
                         serv = serv.servic,
                     } into g
                     select new
                     {
                         SETOR = g.Key.setor,
                         CODSER = g.Key.codser,
                         SERV = g.Key.serv,

                     }).OrderBy(a => a.CODSER);


                tabMestreVirtual = new DataTable();
                tabMestreVirtual.TableName = "MESTREVIRTUAL";

                tabMestreVirtual.Columns.Add("SETOR", Type.GetType("System.String"));
                tabMestreVirtual.Columns["SETOR"].MaxLength = 2;
                tabMestreVirtual.Columns.Add("CODSER", Type.GetType("System.String"));
                tabMestreVirtual.Columns["CODSER"].MaxLength = 4;
                tabMestreVirtual.Columns.Add("SERV", Type.GetType("System.String"));
                tabMestreVirtual.Columns["SERV"].MaxLength = 25;
                tabMestreVirtual.Columns.Add("NUM_MOD", Type.GetType("System.String"));
                tabMestreVirtual.Columns["NUM_MOD"].MaxLength = 2;
                tabMestreVirtual.Columns.Add("MODEL", Type.GetType("System.String"));
                tabMestreVirtual.Columns["MODEL"].MaxLength = 25;
                tabMestreVirtual.Columns.Add("INICIO", Type.GetType("System.DateTime"));
                tabMestreVirtual.Columns.Add("FIM", Type.GetType("System.DateTime"));
                tabMestreVirtual.Columns.Add("DATAOK", Type.GetType("System.DateTime"));


                foreach (var dado in mestreVirtual)
                {
                    DataRow orow = tabMestreVirtual.NewRow();
                    orow["SETOR"] = dado.SETOR;
                    orow["CODSER"] = dado.CODSER;
                    orow["SERV"] = dado.SERV;
                    tabMestreVirtual.Rows.Add(orow);
                    orow.AcceptChanges();
                }



                tabDetalheVirtual = tabMatGeral.Clone();
                tabMatGeral.AsEnumerable().Where(row =>
                            (row.Field<string>("TIPO2").Trim() != "T")
                        && row.Field<string>("TIPO").Trim() == "S"
                        && row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                        && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                         && row.Field<string>("SETOR").Trim() == setor.Trim()
                       ).CopyToDataTable(tabDetalheVirtual, LoadOption.OverwriteChanges);
                tabDetalheVirtual.Columns.Add("UNID_COD", Type.GetType("System.String"));
                tabDetalheVirtual.Columns["UNID_COD"].MaxLength = 12;
                tabDetalheVirtual.Columns["UNID_COD"].DefaultValue = "            ";
                tabDetalheVirtual.Columns.Add("FALSO_INC", Type.GetType("System.Int32"));
                tabDetalheVirtual.Columns["FALSO_INC"].DefaultValue = 0;

                tabDetalheVirtual.TableName = "DETALHEVIRTUAL";
                tabDetalheVirtual.AcceptChanges();
                foreach (DataRow orow in tabDetalheVirtual.Rows)
                {
                    orow.BeginEdit();
                    orow["FALSO_INC"] = 1;
                    orow["UNID_COD"] = "            ";
                    orow.EndEdit();
                    orow.AcceptChanges();
                }




                // PEGAR DE INICIO OS DADOS DO MOVEST p/TRABALHAR COM MAIS FOLGA(RAPIDEZ) NO POSISTIONCHANGE

            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
            return result;
        }








        #endregion

        #region SUPORTE PARA CUSTOMEDIO PRODUTOS ALMOXARIFADO
       /* public async Task<Dictionary<string, List<InicioFim>>> NovoAlgoritmoParaPMedio(string tcod,  DateTime dtData, DateTime UltimoPonto)
        {
            Dictionary<string, List<InicioFim>> dict = new Dictionary<string, List<InicioFim>>();

            try
            {

             
                InicioFim result = null;

                List<string> lst = new List<string>();
                // as entradas dia a dia sem as transferencias
                string str = "Select DATA, COD, SUM(QUANT) QUANT, SUM(VALOR) VALOR FROM MOVEST " +
                      " WHERE " + (tcod.Trim() == "" ? "" : " COD = '" + tcod.PadLeft(4) + "' AND ") +
                      " (TIPO = 'E') AND " +
                       " (TIPO2 <> 'T') AND " +
                       " (QUANT > 0)  AND " +
                       " (VALOR > 0)  AND " +
                       " movest.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') " +
                       " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
                       " GROUP BY COD, DATA ";
                lst.Add(str);
                // as saidas dia a dia sem as transferencias
                    str = "Select DATA, COD, SUM(QUANT) QUANT FROM MOVEST " +
                      " WHERE " + (tcod.Trim() == "" ? "" : " COD = '" + tcod.PadLeft(4) + "' AND ") +
                      " (TIPO = 'S') AND " +
                       " (TIPO2 <> 'T') AND " +
                       " (QUANT > 0)  AND " +
                       " (VALOR > 0)  AND " +
                       " movest.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') " +
                       " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
                       " GROUP BY COD, DATA ";
                    lst.Add(str);

              

                DataSet dsDados = null;
                try
                {
                    dsDados = await ApiServices.Api_QueryMulti(lst);
                }
                catch (Exception)
                {
                    return dict;
                }

                if ((dsDados == null) || (dsDados.Tables.Count == 0)) return dict;

                DataTable dadoCompras = dsDados.Tables[0].Copy();
                DataTable tabSaidas = dsDados.Tables[1].Copy();
                
                    var codigos = (from gr in dadoCompras.AsEnumerable()
                               group gr by new
                               {
                                   cod = gr.Field<string>("COD"),
                               } into g
                               select new
                               {
                                   cod = g.Key.cod,
                                   lstData = g.Select(obj =>
                                   new {
                                       data = obj.Field<DateTime>("DATA"),
                                       quant = obj.Field<double>("QUANT"),
                                       valor = obj.Field<double>("VALOR"),
                                   }).OrderBy(obj => obj.data).ToList()
                               });
               // Dictionary<string, List<InicioFim>> dict = new Dictionary<string, List<InicioFim>>();
                // COMO O CUSTO MÉDIO É ALTERADO SOMENTE QUENDO EXISTE UMA NOVA ENTRADA (COMPRAS) 
                // O PERIODO ENTRE UMA COMPRA E OUTRA É CRUCIAL PARA O CÁLCULO
                // coloque inicio da compra e dia anterior a uma nova compra
                // o CUSTO MEDIO É ALTERADO A CADA NOVA COMPRA, Daí a necessidade de ajustar este periodo
                // saber a quantidade saida dentro deste periodo
                foreach (var dado in codigos)
                {
                    List<InicioFim> lstInicioFims = new List<InicioFim>();
                    for (int i = 0; i < dado.lstData.Count; i++)
                    {
                        InicioFim inicioFim = new InicioFim();
                        inicioFim.inicio = dado.lstData[i].data;
                        if (i == (dado.lstData.Count - 1)) // ultima data de compra
                        {
                            inicioFim.fim = dtData;
                        }
                        else
                            inicioFim.fim = dado.lstData[i + 1].data.AddDays(-1);
                        inicioFim.quant = dado.lstData[i].quant;
                        inicioFim.valor = dado.lstData[i].valor;
                        // saidas do almox no periodo
                        double quantSaidaPeriodo = tabSaidas.AsEnumerable().Where(obj =>
                          (obj.Field<DateTime>("DATA").CompareTo(inicioFim.inicio) >= 0)
                          && (obj.Field<DateTime>("DATA").CompareTo(inicioFim.fim) <= 0)
                          ).Sum(obj => obj.Field<Double>("QUANT"));
                        inicioFim.quantSaida = quantSaidaPeriodo;
                        lstInicioFims.Add(inicioFim);
                    }
                    // quantidade saida no periodo
                    dict.Add(dado.cod, lstInicioFims);
                }
                // calculo do custo medio
                // a classe auxiliar InicioFim é descrita no 
                foreach (KeyValuePair<string, List<InicioFim>> dado in dict)
                {
                    double custoMedio = 0;
                    double quantAcum = 0;
                    double valAcum = 0;
                    InicioFim obj = null;
                    for (int i = 0; i < dado.Value.Count; i++)
                    {
                        obj = dado.Value[i];
                        quantAcum = quantAcum + obj.quant;
                        valAcum = valAcum + Math.Round(obj.valor, 2);
                        // novo Custo Medio
                        custoMedio = Math.Round(valAcum / quantAcum, 5);
                        obj.custoMedio = custoMedio;
                        if (obj.quantSaida < 0)
                        {
                            obj.tipoErro = "Erro Saida < 0 ";
                            obj.errosInicioFim.Add(obj);
                            
                            // MessageBox.Show("Erro Saida < 0 " + obj.inicio.ToString("d"));
                        }
                        double valorSaidasPeriodo = Math.Round(obj.quantSaida * custoMedio, 4);
                        // abatendo a saida
                        quantAcum = Math.Round(quantAcum - obj.quantSaida, 5);
                        valAcum = Math.Round(valAcum - valorSaidasPeriodo, 2);
                        obj.quantAcum = quantAcum;
                        obj.valAcum = valAcum;
                        obj.valorSaida = valorSaidasPeriodo;
                        if ((valAcum < 0) || (quantAcum < 0))
                        {
                            obj.tipoErro = "ValorAcumulado ou QuantidadeAcumulada < 0 ";
                            obj.errosInicioFim.Add(obj);
                        }
                           // MessageBox.Show("Erro < 0 " + obj.inicio.ToString("d"));

                    }
                   // result = obj;
                }
                return dict;
            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return dict;
            }
        }*/

        #endregion


    }
}
#region RASCUNHOS

/*  
 *   /*var GeralCodNumMovest = (from gr in MovEstStatic.MovEst().AsEnumerable()
                                     .Where(row =>
                    (row.Field<string>("TIPO2").Trim() != "T")
                    && (row.Field<string>("TIPO").Trim() == "S")
                    && (setorescolha.Trim() == "" ? true : row.Field<string>("SETOR").Trim() == setorescolha.Trim())
                    && (row.Field<DateTime>("DATA").CompareTo(dataInicio) >= 0))
                                     join serv in servicos on gr.Field<string>("CODSER").Trim() equals serv.cod.Trim()
                                      into ps
                                     from serv in ps.DefaultIfEmpty()

                                     group gr by new
                                     {
                                         codser = gr.Field<string>("CODSER"),
                                         servic = serv.servic,

                                     } into g
                                     select new
                                     {

                                        CODSER = g.Key.codser,
                                         SERV = g.Key.servic
                                     }).OrderBy(a => a.CODSER).ToList();
            */
// poderá filtrar pelo setor
/*foreach (var dado in GeralCodNumMovest)
{
    DataRow orow = tabGeralCodNumMovest.NewRow();
    orow["CODSER"] = dado.CODSER;
    orow["SERV"] = dado.SERV;
    tabGeralCodNumMovest.Rows.Add(orow);
    orow.AcceptChanges();
}*/


  /*  public bool FiltroAlmoxarifado_BaseMovestStatic()
        {
            if (!MovEstStatic.MovEstOk()) return false;

            bool result = true;
            ConstruaSugere();
            ConstruaMaterialAI();

            setor = setor.Trim().PadLeft(2);
            try
            {
                try
                {


                    tabMatGeral = MovEstStatic.MovEst().Clone();
                    MovEstStatic.MovEst().AsEnumerable().Where(row =>
                          row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                         && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                          ).CopyToDataTable(tabMatGeral, LoadOption.OverwriteChanges);
                    tabMatGeral.TableName = "MOVESTGERAL";
                    tabMatGeral.Columns["DESCRICAO"].MaxLength = 40;



                    /// FIM TABMATGERAL

                    var saldoDeposito = from gr in MovEstStatic.MovEst().AsEnumerable().Where(row =>
                                                  !row.IsNull("DATA")
                                                  && !row.IsNull("QUANT")
                                                  && !row.IsNull("TIPO")
                                                  && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0)
                                        group gr by new
                                        {
                                            cod = gr.Field<string>("COD"),
                                            deposito = gr.Field<string>("DEPOSITO"),
                                            descricao = gr.Field<string>("DESCRICAO").Trim(),
                                            unid = gr.Field<string>("UNID")
                                        } into g
                                        select new
                                        {
                                            COD = g.Key.cod,
                                            DEPOSITO = g.Key.deposito,
                                            DESCRICAO = g.Key.descricao,
                                            UNID = g.Key.unid,
                                            TOTQUANT = g.Sum(row => row.Field<string>("TIPO").Trim() == "E" ? row.Field<double>("QUANT") :
                                                                (row.Field<double>("QUANT") * -1))
                                        };


                    tabSaldoDeposito = new DataTable();
                    tabSaldoDeposito.TableName = "SALDODEPOSITO";
                    tabSaldoDeposito.Columns.Add("DESCRICAO", Type.GetType("System.String"));
                    tabSaldoDeposito.Columns["DESCRICAO"].MaxLength = 40;
                    tabSaldoDeposito.Columns.Add("UNID", Type.GetType("System.String"));
                    tabSaldoDeposito.Columns["UNID"].MaxLength = 3;
                    tabSaldoDeposito.Columns.Add("COD", Type.GetType("System.String"));
                    tabSaldoDeposito.Columns["COD"].MaxLength = 4;
                    tabSaldoDeposito.Columns.Add("DEPOSITO", Type.GetType("System.String"));
                    tabSaldoDeposito.Columns["DEPOSITO"].MaxLength = 2;
                    tabSaldoDeposito.Columns.Add("TOTQUANT", Type.GetType("System.Double"));
                    foreach (var dado in saldoDeposito)
                    {
                        DataRow orow = tabSaldoDeposito.NewRow();
                        orow["COD"] = dado.COD;
                        orow["DEPOSITO"] = dado.DEPOSITO;
                        orow["DESCRICAO"] = dado.DESCRICAO;
                        orow["UNID"] = dado.UNID;
                        orow["TOTQUANT"] = dado.TOTQUANT;
                        tabSaldoDeposito.Rows.Add(orow);
                    }


                }
                catch (Exception E)
                {
                    return false;
                }

                var servicos = (from gr in DadosComum.ServicoModeloCombo.Copy().AsEnumerable()
                                group gr by new
                                {
                                    codserv = gr.Field<string>("CODSER"),
                                    servic = gr.Field<string>("DESCSERV")
                                } into g
                                select new
                                {
                                    cod = g.Key.codserv,
                                    servic = g.Key.servic,
                                });


                Recomponha_tabGeralCodNumMovest("");



                var mestreVirtual =
                    (from gr in tabMatGeral.AsEnumerable()
                                         .Where(row =>
                        (row.Field<string>("TIPO2").Trim() != "T")
                        && row.Field<string>("TIPO").Trim() == "S"
                        && row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                        && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                         && row.Field<string>("SETOR").Trim() == setor.Trim()
                        )
                     join serv in servicos on gr.Field<string>("CODSER").Trim() equals serv.cod
                       into ps
                     from serv in ps.DefaultIfEmpty()


                     group gr by new
                     {
                         setor = gr.Field<string>("SETOR"),
                         codser = gr.Field<string>("CODSER"),
                         serv = serv.servic,
                     } into g
                     select new
                     {
                         SETOR = g.Key.setor,
                         CODSER = g.Key.codser,
                         SERV = g.Key.serv,

                     }).OrderBy(a => a.CODSER);


                tabMestreVirtual = new DataTable();
                tabMestreVirtual.TableName = "MESTREVIRTUAL";

                tabMestreVirtual.Columns.Add("SETOR", Type.GetType("System.String"));
                tabMestreVirtual.Columns["SETOR"].MaxLength = 2;
                tabMestreVirtual.Columns.Add("CODSER", Type.GetType("System.String"));
                tabMestreVirtual.Columns["CODSER"].MaxLength = 4;
                tabMestreVirtual.Columns.Add("SERV", Type.GetType("System.String"));
                tabMestreVirtual.Columns["SERV"].MaxLength = 25;
                tabMestreVirtual.Columns.Add("NUM_MOD", Type.GetType("System.String"));
                tabMestreVirtual.Columns["NUM_MOD"].MaxLength = 2;
                tabMestreVirtual.Columns.Add("MODEL", Type.GetType("System.String"));
                tabMestreVirtual.Columns["MODEL"].MaxLength = 25;
                tabMestreVirtual.Columns.Add("INICIO", Type.GetType("System.DateTime"));
                tabMestreVirtual.Columns.Add("FIM", Type.GetType("System.DateTime"));
                tabMestreVirtual.Columns.Add("DATAOK", Type.GetType("System.DateTime"));


                foreach (var dado in mestreVirtual)
                {
                    DataRow orow = tabMestreVirtual.NewRow();
                    orow["SETOR"] = dado.SETOR;
                    orow["CODSER"] = dado.CODSER;
                    orow["SERV"] = dado.SERV;
                    tabMestreVirtual.Rows.Add(orow);
                    orow.AcceptChanges();
                }



                tabDetalheVirtual = tabMatGeral.Clone();
                tabMatGeral.AsEnumerable().Where(row =>
                            (row.Field<string>("TIPO2").Trim() != "T")
                        && row.Field<string>("TIPO").Trim() == "S"
                        && row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                        && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                         && row.Field<string>("SETOR").Trim() == setor.Trim()
                       ).CopyToDataTable(tabDetalheVirtual, LoadOption.OverwriteChanges);
                tabDetalheVirtual.Columns.Add("UNID_COD", Type.GetType("System.String"));
                tabDetalheVirtual.Columns["UNID_COD"].MaxLength = 12;
                tabDetalheVirtual.Columns["UNID_COD"].DefaultValue = "            ";
                tabDetalheVirtual.Columns.Add("FALSO_INC", Type.GetType("System.Int32"));
                tabDetalheVirtual.Columns["FALSO_INC"].DefaultValue = 0;

                tabDetalheVirtual.TableName = "DETALHEVIRTUAL";
                tabDetalheVirtual.AcceptChanges();
                foreach (DataRow orow in tabDetalheVirtual.Rows)
                {
                    orow.BeginEdit();
                    orow["FALSO_INC"] = 1;
                    orow["UNID_COD"] = "            ";
                    orow.EndEdit();
                    orow.AcceptChanges();
                }


            }
            catch (Exception E)
            {
                MessageBox.Show(E.Message);
                return false;
            }
            return result;
        }
      */

/*
*  
 *  
 *  public async Task<Boolean> OutroFiltroAlmoxarifado()
  {
      bool result = true;
      ConstruaSugere();
      ConstruaMaterialAI();

      setor = setor.Trim().PadLeft(2);
      try
      {
          List<string> lstStr = new List<string>();
          string str = "SELECT movest.*," +
          " cadest.DESCRI as Descricao, cadest.UNID as UNID " +
          "  FROM  MOVEST, CADEST " +
          " WHERE   movest.cod = cadest.cod " +
            "  AND " +
          // " (movest.data > ctod('" + dataRef.ToString("MM/dd/yyyy") + "')) ";
           " (movest.data > ctod('" + INI_DTAPESQUISA.ToString("MM/dd/yyyy") + "')) ";

          lstStr.Add(str);
          str = "SELECT movest.deposito, movest.cod ,alltrim(cadest.DESCRI) as descricao, cadest.unid,"
            + " SUM(iif((tipo='E'),movest.quant,movest.quant*-1)) AS totquant " +
           " FROM movest,cadest  where (movest.cod = cadest.cod) and " +
          " movest.DATA >= CTOD('" + dataRef.ToString("MM/dd/yyyy") + "') " +
          " AND movest.DATA <= CTOD('" + UltimoPonto.ToString("MM/dd/yyyy") + "') " +
         " GROUP BY deposito, movest.cod,cadest.DESCRI,cadest.unid  ORDER BY deposito, movest.cod ";
          lstStr.Add(str);

          try
          {
              DataSet dsDados = await ApiServices.Api_QueryMulti(lstStr);

              if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
              // Tabela virtual

              tabMatGeral = dsDados.Tables[0].Copy();
              tabMatGeral.TableName = "MOVESTGERAL";
              tabSaldoDeposito = dsDados.Tables[1].Copy();
              tabSaldoDeposito.TableName = "SALDODEPOSITO";
          }
          catch (Exception E)
          { }

          var servicos = (from gr in DadosComum.ServicoModeloCombo.Copy().AsEnumerable()
                          group gr by new
                          {
                              codserv = gr.Field<string>("CODSER"),
                              servic = gr.Field<string>("DESCSERV")
                          } into g
                          select new
                          {
                              cod = g.Key.codserv,
                              servic = g.Key.servic,
                          });


          Recomponha_tabGeralCodNumMovest("", INI_DTAPESQUISA);


          str = "SELECT     distinct SETOR,  codser ,servic.DESCRI as serv , " +
           " CTOD('') as INICIO, CTOD('') as FIM, CTOD('') as DATAOK      " +
           " FROM  movest, modelo, servic  WHERE " +
           " (servic.Cod = codser)  and (movest.tipo = 'S') " +
           " and (movest.tipo2 <> 'T') and  (data BETWEEN ctod('" +
           dtData.ToString("MM/dd/yyyy") + "') and ctod('" +
            UltimoPonto.ToString("MM/dd/yyyy") + "') )  and setor = '" + setor + "' order by codser";
          lstStr.Add(str);


          var mestreVirtual =
              (from gr in tabMatGeral.AsEnumerable()
                                   .Where(row =>
                  (row.Field<string>("TIPO2").Trim() != "T")
                  && row.Field<string>("TIPO").Trim() == "S"
                  && row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                  && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                   && row.Field<string>("SETOR").Trim() == setor.Trim()
                  )
               join serv in servicos on gr.Field<string>("CODSER").Trim() equals serv.cod
                 into ps
               from serv in ps.DefaultIfEmpty()


               group gr by new
               {
                   setor = gr.Field<string>("SETOR"),
                   codser = gr.Field<string>("CODSER"),
                   serv = serv.servic,
               } into g
               select new
               {
                   SETOR = g.Key.setor,
                   CODSER = g.Key.codser,
                   SERV = g.Key.serv,

               }).OrderBy(a => a.CODSER);


          tabMestreVirtual = new DataTable();
          tabMestreVirtual.TableName = "MESTREVIRTUAL";

          tabMestreVirtual.Columns.Add("SETOR", Type.GetType("System.String"));
          tabMestreVirtual.Columns["SETOR"].MaxLength = 2;
          tabMestreVirtual.Columns.Add("CODSER", Type.GetType("System.String"));
          tabMestreVirtual.Columns["CODSER"].MaxLength = 4;
          tabMestreVirtual.Columns.Add("SERV", Type.GetType("System.String"));
          tabMestreVirtual.Columns["SERV"].MaxLength = 25;
          tabMestreVirtual.Columns.Add("NUM_MOD", Type.GetType("System.String"));
          tabMestreVirtual.Columns["NUM_MOD"].MaxLength = 2;
          tabMestreVirtual.Columns.Add("MODEL", Type.GetType("System.String"));
          tabMestreVirtual.Columns["MODEL"].MaxLength = 25;
          tabMestreVirtual.Columns.Add("INICIO", Type.GetType("System.DateTime"));
          tabMestreVirtual.Columns.Add("FIM", Type.GetType("System.DateTime"));
          tabMestreVirtual.Columns.Add("DATAOK", Type.GetType("System.DateTime"));


          foreach (var dado in mestreVirtual)
          {
              DataRow orow = tabMestreVirtual.NewRow();
              orow["SETOR"] = dado.SETOR;
              orow["CODSER"] = dado.CODSER;
              orow["SERV"] = dado.SERV;
              tabMestreVirtual.Rows.Add(orow);
              orow.AcceptChanges();
          }



          tabDetalheVirtual = tabMatGeral.Clone();
          tabMatGeral.AsEnumerable().Where(row =>
                      (row.Field<string>("TIPO2").Trim() != "T")
                  && row.Field<string>("TIPO").Trim() == "S"
                  && row.Field<DateTime>("DATA").CompareTo(dtData) >= 0
                  && row.Field<DateTime>("DATA").CompareTo(UltimoPonto) <= 0
                   && row.Field<string>("SETOR").Trim() == setor.Trim()
                 ).CopyToDataTable(tabDetalheVirtual, LoadOption.OverwriteChanges);
          tabDetalheVirtual.Columns.Add("UNID_COD", Type.GetType("System.String"));
          tabDetalheVirtual.Columns["UNID_COD"].MaxLength = 12;
          tabDetalheVirtual.Columns["UNID_COD"].DefaultValue = "            ";
          tabDetalheVirtual.Columns.Add("FALSO_INC", Type.GetType("System.Int32"));
          tabDetalheVirtual.Columns["FALSO_INC"].DefaultValue = 0;

          tabDetalheVirtual.TableName = "DETALHEVIRTUAL";
          tabDetalheVirtual.AcceptChanges();
          foreach (DataRow orow in tabDetalheVirtual.Rows)
          {
              orow.BeginEdit();
              orow["FALSO_INC"] = 1;
              orow["UNID_COD"] = "            ";
              orow.EndEdit();
              orow.AcceptChanges();
          }




          // PEGAR DE INICIO OS DADOS DO MOVEST p/TRABALHAR COM MAIS FOLGA(RAPIDEZ) NO POSISTIONCHANGE

      }
      catch (Exception E)
      {
          MessageBox.Show(E.Message);

      }
      return result;
  }
*/
#endregion