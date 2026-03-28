using ApoioContabilidade.Models;
using ApoioContabilidade.Services;
using ApoioContabilidade.Trabalho.ConfigComponentes;
using ClassFiltroEdite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ApoioContabilidade.Trabalho.ServicesTrab
{
    public class ServicoFiltroTrab
    {
        public DateTime semana;
        const int SEMANAS_ANT = 28;
        const int SEMANAS_ANT_2 = 56;
        const string CODSERV_PONTOPROVISORIO = "943";
        public ServCltPonto servPonto;
        public DataTable dtTrabMov = null;
        public DataTable dtTrabMovAnt = null;
        public DataTable dtTrabNoturno = null;
        public DataTable dtTrabMovNotAnt = null;
        public DataTable dtTrabPremio = null;

        public DataTable dtTrabCad = null;

        public BindingSource bmTrabCad = new BindingSource();
        public BindingSource bmTrabMov = new BindingSource();
        public BindingSource bmTrabNoturno = new BindingSource();
        public BindingSource bmTrabMovAnt = new BindingSource();
        public BindingSource bmTrabPremio = new BindingSource();
        //public BindingSource bmTrabMovNotAnt = new BindingSource();

        DataTable CltCadastroCompleto;

        public List<Int64> lstFiltroMovID;
        public List<Int64> lstFiltroMovAnterior;
        public List<string> lstFiltroMestreTrab;
        public List<string> lstFiltroMestreTrabAnterior;
        //public List<double> lstFiltroMovNreg;
        public bool ckTodos;
        public bool ckSemana;
        public bool ckAnterior;
        // campos Dos Filtros
        string txSetor = "";
        string txNome = "";
        string txTrab = "";
        string txServico = "";
        public EditaCltPonto comum;

        // PontoProvisorio AVISO
        public Dictionary<string, List<DateTime>> dictPontoProvisorioAviso;

        public ServicoFiltroTrab(DateTime osemana, ServCltPonto oservPonto)
        {
            ckTodos = true;
            semana = osemana.Date;
            servPonto = oservPonto;
            lstFiltroMovID = new List<Int64>();
            lstFiltroMovAnterior = new List<Int64>();
            lstFiltroMestreTrab = new List<string>();
            lstFiltroMestreTrabAnterior = new List<string>();
        }

        public void BmTrabCad_PositionChanged(object sender, EventArgs e)
        {
            DataRowView registro = ((sender as BindingSource).Current as DataRowView);

            bool result = (registro != null);
            if (result)
            {
                if (registro.Row.RowState == DataRowState.Detached) return;
                if (registro.Row.RowState == DataRowState.Added)
                {
                    if (!registro.Row.IsNull("CODCAD"))
                    {
                        string trab = registro.Row["CODCAD"].ToString().Trim();
                        if (trab == "") return;
                    }
                    else return;
                }
            }
            if (result) { result = !registro.Row.IsNull("CODCAD"); }
            if (result)
            {
                string trab = registro.Row["CODCAD"].ToString().Trim();
                try
                {   // MovTrab

                    var dado = dtTrabMov.AsEnumerable().Where(row =>
                        (!row.IsNull("TRAB")) &&
                        (row.Field<string>("TRAB").Trim() == trab)
                        && (row.Field<DateTime>("DATA").CompareTo(semana) == 0)
                        // se for checktodos
                        && (!ckTodos ? true : ((lstFiltroMovID.Count == 0) && (lstFiltroMovAnterior.Count == 0) ? true : 
                                             lstFiltroMovID.Contains(row.Field<Int64>("ID")) ))
                        // se for Semana
                        && (!ckSemana ? true : ((lstFiltroMovID.Count == 0)  ? false :
                                             lstFiltroMovID.Contains(row.Field<Int64>("ID"))))

                        );
                    if ((dado != null))
                    { bmTrabMov.DataSource = dado.AsDataView(); }
                    else { bmTrabMov.DataSource = dtTrabMov.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                }
                catch (Exception)
                {
                    bmTrabMov.DataSource = dtTrabMov.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                }
                try
                {   // MovTrabNoturno

                    var dado = dtTrabNoturno.AsEnumerable().Where(row =>
                       (!row.IsNull("TRAB")) &&
                       (row.Field<string>("TRAB").Trim() == trab)
                        && (row.Field<DateTime>("DATA").CompareTo(semana) == 0)
                        
                        );
                    if ((dado != null))
                    { bmTrabNoturno.DataSource = dado.AsDataView(); }
                    else { bmTrabNoturno.DataSource = dtTrabNoturno.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                }
                catch (Exception)
                {
                    bmTrabNoturno.DataSource = dtTrabNoturno.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                }

                try
                {   // MovTrabPremio em abril 2021

                    var dado = dtTrabPremio.AsEnumerable().Where(row =>
                       (!row.IsNull("TRAB")) &&
                       (row.Field<string>("TRAB").Trim() == trab)
                        && (row.Field<DateTime>("DATA").CompareTo(semana) == 0)

                        );
                    if ((dado != null))
                    { bmTrabPremio.DataSource = dado.AsDataView(); }
                    else { bmTrabPremio.DataSource = dtTrabPremio.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                }
                catch (Exception)
                {
                    bmTrabPremio.DataSource = dtTrabPremio.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                }



                try
                {   // MovTrabAnterior

                    var dado = dtTrabMovAnt.AsEnumerable().Where(row =>
                     (!row.IsNull("TRAB")) &&
                       (row.Field<string>("TRAB").Trim() == trab)
                       // se for check Todos
                       && (!ckTodos ? true : ((lstFiltroMovID.Count == 0) && (lstFiltroMovAnterior.Count == 0) ? true :
                            lstFiltroMovAnterior.Contains(row.Field<Int64>("ID"))))

                       // se for check Anterior
                       && (!ckAnterior ? true : ( (lstFiltroMovAnterior.Count == 0) ? false :
                            lstFiltroMovAnterior.Contains(row.Field<Int64>("ID"))))




                       ).OrderByDescending(row => row.Field<DateTime>("DATA")).ThenBy(row => row.Field<Int64>("ID"));
                    if ((dado != null))
                    { bmTrabMovAnt.DataSource = dado.AsDataView(); }
                    else { bmTrabMovAnt.DataSource = dtTrabMovAnt.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView(); }

                }
                catch (Exception)
                {
                    bmTrabMovAnt.DataSource = dtTrabMovAnt.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                }
            }
            else
            {
                bmTrabMov.DataSource = dtTrabMov.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
                bmTrabNoturno.DataSource = dtTrabNoturno.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();
               // abril 2021
                bmTrabPremio.DataSource = dtTrabPremio.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();

                bmTrabMovAnt.DataSource = dtTrabMovAnt.AsEnumerable().Where(row => row.IsNull("ID")).AsDataView();

            }
        }

        public void FiltreSetaCampos(string otxTrab, string otxSetor, string otxNome, string otxServico)
        {
            txNome = otxNome;
            txSetor = otxSetor;
            txTrab = otxTrab;
            txServico = otxServico;
        }
        public void FiltreSetaChecks(bool todos, bool semana, bool anterior)
        {
            ckTodos = todos;
            ckSemana = semana;
            ckAnterior = anterior; 
        }

        public void FiltreMestre()
        {
            txNome = txNome.Trim();
            if (txTrab.Trim() == "")
                txTrab = "";
            if (txSetor.Trim() == "")
                txSetor = "";
            if (txServico.Trim() == "")
                txServico = "";

            lstFiltroMestreTrab.Clear();
            lstFiltroMestreTrabAnterior.Clear();
            lstFiltroMovID.Clear();
            lstFiltroMovAnterior.Clear();

            if ((txTrab.Trim() == "") && (txSetor.Trim() == "") && (txNome.Trim() == "") && (txServico.Trim() == ""))
            {
                ckTodos = true;
                ckSemana = false;
                ckAnterior = false;
                bmTrabCad.DataSource = dtTrabCad.AsEnumerable().OrderBy(
                            row => row.Field<string>("NOMECAD")).AsDataView();
                return;
            }

            List<string> dadosTrab = new List<string>();
            if (txTrab != "")
                dadosTrab = txTrab.Split(Convert.ToChar("/")).ToList();

            List<string> dadosSetor = new List<string>();
            if (txSetor != "")
                dadosSetor = txSetor.Split(Convert.ToChar("/")).ToList();

            List<string> dadosServico = new List<string>();
            if (txServico != "")
                dadosServico = txServico.Split(Convert.ToChar("/")).ToList();
            if (dadosServico.Count > 0)
            {
                try
                {
                    lstFiltroMestreTrab =
                    (from gr in dtTrabMov.AsEnumerable().Where(orow =>

                        (dadosServico.Count == 0 ? true : dadosServico.Contains(orow.Field<string>("CODSER")))
                    )
                     group gr by new
                     {
                         trab = gr.Field<string>("TRAB")
                     } into g
                     select g.Key.trab
                     ).ToList();

                }
                catch (Exception)
                {
                    lstFiltroMestreTrab = new List<string>();
                }
                // Pega os Trabalhadores
                try
                {
                    
                    lstFiltroMestreTrabAnterior =
                    (from gr in dtTrabMovAnt.AsEnumerable().Where(orow =>

                        (dadosServico.Count == 0 ? true : dadosServico.Contains(orow.Field<string>("CODSER")))
                    )
                     group gr by new
                     {
                         trab = gr.Field<string>("TRAB")
                     } into g
                     select g.Key.trab
                     ).ToList();

                    

                }
                catch (Exception)
                {
                    
                }

                try
                {
                    lstFiltroMovID = (from gr in dtTrabMov.AsEnumerable().Where(orow =>

                        (dadosServico.Count == 0 ? true : dadosServico.Contains(orow.Field<string>("CODSER")))
                                )
                                        group gr by new
                                        {
                                            nreg = gr.Field<Int64>("ID")
                                        } into g
                                        select g.Key.nreg
                     ).ToList();

                }
                catch (Exception E)
                {
                    lstFiltroMovID = new List<Int64>();
                }

                try
                {
                    lstFiltroMovAnterior = (from gr in dtTrabMovAnt.AsEnumerable().Where(orow =>

                        (dadosServico.Count == 0 ? true : dadosServico.Contains(orow.Field<string>("CODSER")))
                                )
                                      group gr by new
                                      {
                                          nreg = gr.Field<Int64>("ID")
                                      } into g
                                      select g.Key.nreg
                     ).ToList();

                }
                catch (Exception)
                {
                    lstFiltroMovAnterior = new List<Int64>();
                }
            }


            try
            {
                if (ckTodos)
                {
                    lstFiltroMestreTrab = lstFiltroMestreTrab.Union(lstFiltroMestreTrabAnterior).ToList();
                    bmTrabCad.DataSource = dtTrabCad.AsEnumerable().Where(row =>
                    (dadosTrab.Count == 0 ? true : dadosTrab.Contains(row.Field<string>("CODCAD")))
                    &&
                    (dadosSetor.Count == 0 ? true : dadosSetor.Contains(row.Field<string>("SETOR")))
                    &&
                    (txNome == "" ? true : txNome.Substring(0, txNome.Length)
                                            == row.Field<string>("NOMECAD").Substring(0, txNome.Length))
                    &&
                    (lstFiltroMestreTrab.Count == 0 ? true : lstFiltroMestreTrab.Contains(row.Field<string>("CODCAD")))

                    ).OrderBy(row => row.Field<string>("NOMECAD")).AsDataView();
                }
                else if (ckSemana)
                {
                    bmTrabCad.DataSource = dtTrabCad.AsEnumerable().Where(row =>
                    (dadosTrab.Count == 0 ? true : dadosTrab.Contains(row.Field<string>("CODCAD")))
                    &&
                    (dadosSetor.Count == 0 ? true : dadosSetor.Contains(row.Field<string>("SETOR")))
                    &&
                    (txNome == "" ? true : txNome.Substring(0, txNome.Length)
                                            == row.Field<string>("NOMECAD").Substring(0, txNome.Length))
                    &&
                    (lstFiltroMestreTrab.Count == 0 ? false : lstFiltroMestreTrab.Contains(row.Field<string>("CODCAD")))

                    ).OrderBy(row => row.Field<string>("NOMECAD")).AsDataView();

                }
                else if (ckAnterior)
                {
                    bmTrabCad.DataSource = dtTrabCad.AsEnumerable().Where(row =>
                    (dadosTrab.Count == 0 ? true : dadosTrab.Contains(row.Field<string>("CODCAD")))
                    &&
                    (dadosSetor.Count == 0 ? true : dadosSetor.Contains(row.Field<string>("SETOR")))
                    &&
                    (txNome == "" ? true : txNome.Substring(0, txNome.Length)
                                            == row.Field<string>("NOMECAD").Substring(0, txNome.Length))
                    &&
                    (lstFiltroMestreTrabAnterior.Count == 0 ? false : lstFiltroMestreTrabAnterior.Contains(row.Field<string>("CODCAD")))

                    ).OrderBy(row => row.Field<string>("NOMECAD")).AsDataView();

                }

                // Reposicione
                BmTrabCad_PositionChanged(bmTrabCad, null);
            }
            catch (Exception)
            {
               MessageBox.Show("Pesquisa Não Resultou em registros válidos");
                ckTodos = true;
                ckSemana = false;
                ckAnterior = false;

                bmTrabCad.DataSource = dtTrabCad.AsEnumerable().OrderBy(
                            row => row.Field<string>("NOMECAD")).AsDataView();
            }
        }

        public void DtTrabMov_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DataRow orow = null;
            try
            {
                if (bmTrabCad.Count > 0)
                {
                    orow = ((DataRowView)bmTrabCad.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Trabalhador??");
                throw;
            }
            if (orow == null) return;
            e.Row["TRAB"] = orow["CODCAD"].ToString();
            e.Row["DATA"] = semana.Date;
            //e.Row["SEMANA"] = semana.Date;
            e.Row["DIA1"] = "";
            e.Row["DIA2"] = "";
            e.Row["DIA3"] = "";
            e.Row["DIA4"] = "";
            e.Row["DIA5"] = "";
            e.Row["DIA6"] = "";
            e.Row["DIA7"] = "";
            e.Row["FAZMOV"] = ""; // centro
            e.Row["BL"] = ""; // centro
            e.Row["CODSER"] = "";
            e.Row["NUM_MOD"] = "";
            e.Row["QUANT"] = 0;
            e.Row["OK"] = "";
            e.Row["DIARIA"] = 0;
            e.Row["NOTURNO"] = "";
            e.Row["OBS"] = "";
            e.Row["TIPOMOV"] = "D";
            e.Row["NREG"] = 0;
        }
        public void DtTrabMovNoturno_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DataRow orow = null;
            try
            {
                if (bmTrabCad.Count > 0)
                {
                    orow = ((DataRowView)bmTrabCad.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Trabalhador??");
                throw;
            }
            if (orow == null) return;
            e.Row["TRAB"] = orow["CODCAD"].ToString();
            e.Row["DATA"] = semana.Date;
            //e.Row["SEMANA"] = semana.Date;
            e.Row["DIA1"] = "";
            e.Row["DIA2"] = "";
            e.Row["DIA3"] = "";
            e.Row["DIA4"] = "";
            e.Row["DIA5"] = "";
            e.Row["DIA6"] = "";
            e.Row["DIA7"] = "";
            e.Row["FAZMOV"] = ""; // centro
            e.Row["BL"] = ""; // centro
            e.Row["CODSER"] = "";
            e.Row["NUM_MOD"] = "";
            e.Row["QUANT"] = 0;
            e.Row["OK"] = "";
            e.Row["DIARIA"] = 0;
            e.Row["NOTURNO"] = "X";
            e.Row["OBS"] = "";
            e.Row["TIPOMOV"] = "D";
            e.Row["NREG"] = 0;
        }
        public void DtTrabMovPremio_TableNewRow(object sender, DataTableNewRowEventArgs e)
        {
            DataRow orow = null;
            try
            {
                if (bmTrabCad.Count > 0)
                {
                    orow = ((DataRowView)bmTrabCad.Current).Row;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Trabalhador??");
                throw;
            }
            if (orow == null) return;
            e.Row["TRAB"] = orow["CODCAD"].ToString();
            e.Row["DATA"] = semana.Date;
            //e.Row["SEMANA"] = semana.Date;
            e.Row["DIA1"] = "";
            e.Row["DIA2"] = "";
            e.Row["DIA3"] = "";
            e.Row["DIA4"] = "";
            e.Row["DIA5"] = "";
            e.Row["DIA6"] = "";
            e.Row["DIA7"] = "";
            e.Row["FAZMOV"] = ""; // centro
            e.Row["BL"] = ""; // centro
            e.Row["CODSER"] = "";
            e.Row["NUM_MOD"] = "";
            e.Row["QUANT"] = 0;
            e.Row["OK"] = "";
            e.Row["DIARIA"] = 0;
            e.Row["VALOR"] = 0;
            e.Row["NOTURNO"] = "P";
            e.Row["OBS"] = "";
            e.Row["TIPOMOV"] = "D";
            e.Row["DTA_INI"] = semana.Date;
            e.Row["DTA_FIM"] = semana.Date;
            e.Row["NREG"] = 0;
        }
        public void DtTrabMov_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {

            DataRow orow = e.Rows[0];
            if (orow["NOTURNO"].ToString().Trim() == "P")
            {
                // abril 2021
                if ((Convert.ToDouble(orow["VALOR"]) <= 0)
                   || (orow["FAZMOV"].ToString().Trim() == "")
                   || (orow["NUM_MOD"].ToString().Trim() == "")
                   || (orow["CODSER"].ToString().Trim() == "")
                   || (orow.IsNull("DATA"))
                    || (orow.IsNull("DATA"))
                   || (orow.IsNull("DTA_INI"))
                   || (orow.IsNull("DTA_FIM"))
                   || (Convert.ToDateTime(orow["DTA_FIM"]).CompareTo(Convert.ToDateTime(orow["DTA_INI"])) < 0)
                   )
                {
                    MessageBox.Show("Dados Inconsistentes com padrão");
                    e.Cancela = true;
                    return;
                }
            }
            else  if ((Convert.ToDouble(orow["QUANT"]) < 0)
                    || (orow["FAZMOV"].ToString().Trim() == "")
                    || (orow["NUM_MOD"].ToString().Trim() == "")
                    || (orow["CODSER"].ToString().Trim() == "")
                    || ((orow["DIA1"].ToString().Trim() == "") && (orow["DIA2"].ToString().Trim() == "") && (orow["DIA3"].ToString().Trim() == "")
                       && (orow["DIA4"].ToString().Trim() == "") && (orow["DIA5"].ToString().Trim() == "") && (orow["DIA6"].ToString().Trim() == "")
                       && (orow["DIA7"].ToString().Trim() == "")
                        )
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
        }

        public async void DtTrabMov__AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if (orow["NOTURNO"].ToString().Trim() == "P")
            {
                // abril 2021
                if ((Convert.ToDouble(orow["VALOR"]) <= 0)
                   || (orow["FAZMOV"].ToString().Trim() == "")
                   || (orow["NUM_MOD"].ToString().Trim() == "")
                   || (orow["CODSER"].ToString().Trim() == "")
                   || (orow.IsNull("DATA"))
                   || (orow.IsNull("DTA_INI"))
                   || (orow.IsNull("DTA_FIM"))
                   || (Convert.ToDateTime(orow["DTA_FIM"]).CompareTo(Convert.ToDateTime(orow["DTA_INI"])) < 0)  
                   )
                {
                    MessageBox.Show("Dados Inconsistentes com padrão");
                    e.Cancela = true;
                    return;
                }
            }
            else if ((Convert.ToDouble(orow["QUANT"]) < 0)
                 || (orow["FAZMOV"].ToString().Trim() == "")
                 || (orow["NUM_MOD"].ToString().Trim() == "")
                 || (orow["CODSER"].ToString().Trim() == "")
                 || ((orow["DIA1"].ToString().Trim() == "") && (orow["DIA2"].ToString().Trim() == "") && (orow["DIA3"].ToString().Trim() == "")
                    && (orow["DIA4"].ToString().Trim() == "") && (orow["DIA5"].ToString().Trim() == "") && (orow["DIA6"].ToString().Trim() == "")
                    && (orow["DIA7"].ToString().Trim() == "")
                     )
                 || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = true;
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmTrabCad.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao Trabalhador");
                e.Cancela = true;
                return;
            }
            if (rowmestre["CODCAD"].ToString().Trim() != orow["TRAB"].ToString().Trim())
            {
                MessageBox.Show("Codcad # TRAB");
                e.Cancela = true;
                return;
            }
            string tsetor = "";
            try
            {
                tsetor = TabelasIniciais.SetorAtual_PelaData(orow["FAZMOV"].ToString(), semana.Date);

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
            orow["SETOR"] = tsetor;
           /*   alterei em 25 de abril de 2021 para a forma sistetica de uma só ida ao servidor
            * if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Modified))
             {
                 double nreg = -1;
                 if (e.TipoMuda == DataRowState.Added)
                 {
                     try
                     {
                         DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                         { "Select MAX(NREG) as nreg from CLTPONTO" }));
                         nreg = Convert.ToDouble(odataset.Tables[0].Rows[0][0]);
                     }
                     catch (Exception) { }
                     if (nreg == -1)
                     {
                         MessageBox.Show("Erro ao buscar numero exclusivo");
                         e.Cancela = true;
                         return;
                     }
                    orow["NREG"] = nreg + 1;
                }
                 else nreg = -2;

                // if (e.TipoMuda == DataRowState.Added)

            }*/
            foreach (DataColumn col in orow.Table.Columns)
            {
                if (orow[col.ColumnName] is System.DBNull)
                {
                    orow[col.ColumnName] = col.DefaultValue;
                }
            }


            // 

            bool ok = false;
            if ((e.TipoMuda == DataRowState.Added) || (e.TipoMuda == DataRowState.Detached))
            {

                ok = await Prepara_Sql.OpereIncluaRegistroServidorAsync_diversosMAX(orow, "CLTPONTO", new List<string>() { "NREG" });
            }
            else 
                ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, e.TipoMuda, "CLTPONTO"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            
            if (!ok)
            {
                e.Cancela = true;
                MessageBox.Show("Erro ao Inserir Registro CLTPONTO"); return;

            }


            try
            {
                // oexclua.Add("ADMI");
                orow.AcceptChanges();
                try
                {
                    if (orow["NOTURNO"].ToString().Trim() == "")
                        AtualizaDados(bmTrabMov, bmTrabNoturno);
                    else if (orow["NOTURNO"].ToString().Trim() == "X")
                        AtualizaDados(bmTrabNoturno, bmTrabMov);
                    /*else if (orow["NOTURNO"].ToString().Trim() == "P")
                        AtualizaDados(bmTrabPremio, bmTrabMov);*/
                }
                catch (Exception) { }
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



        /*
        public async void DtTrabMov__AlteraRegistrosOk(object sender, AsyncAlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            if ((Convert.ToDouble(orow["QUANT"]) < 0)
                 || (orow["FAZMOV"].ToString().Trim() == "")
                 || (orow["NUM_MOD"].ToString().Trim() == "")
                 || (orow["CODSER"].ToString().Trim() == "")
                 || ((orow["DIA1"].ToString().Trim() == "") && (orow["DIA2"].ToString().Trim() == "") && (orow["DIA3"].ToString().Trim() == "")
                    && (orow["DIA4"].ToString().Trim() == "") && (orow["DIA5"].ToString().Trim() == "") && (orow["DIA6"].ToString().Trim() == "")
                    && (orow["DIA7"].ToString().Trim() == "")
                     )
                 || (orow.IsNull("DATA")))
            {
                MessageBox.Show("Dados Inconsistentes com padrão");
                e.Cancela = Task.FromResult(true);
                return;
            }
            DataRowView rowmestre = null;
            try
            {
                rowmestre = bmTrabCad.Current as DataRowView;
            }
            catch (Exception)
            {
                MessageBox.Show("Falta Referencia ao Trabalhador");
                e.Cancela = Task.FromResult(true); 
                return;
            }
            if (rowmestre["CODCAD"].ToString().Trim() != orow["TRAB"].ToString().Trim())
            {
                MessageBox.Show("Codcad # TRAB");
                e.Cancela = Task.FromResult(true); 
                return;
            }
            string tsetor = "";
            try
            {
                tsetor = TabelasIniciais.SetorAtual_PelaData(orow["FAZMOV"].ToString(), semana.Date);

            }
            catch (Exception)
            {
                tsetor = "";
            }
            if (tsetor == "")
            {
                MessageBox.Show("Falhou em acessar SETOR a partir da GLEBA");
                e.Cancela = Task.FromResult(true); 
                return;
            }

            foreach (DataColumn col in orow.Table.Columns)
            {
                if (orow[col.ColumnName] is System.DBNull)
                {
                    orow[col.ColumnName] = col.DefaultValue;
                }
            }
            Int16 espera = -1;
            espera = await PesquiseNREG_Grave(orow);
            while (espera == -1) {
                Thread.Sleep(20);
            };
            bool retorna = true; ;
            if (espera == 0)
                retorna = false;
            if (!retorna)
            {
                e.Cancela = Task.FromResult(true); 
                MessageBox.Show("Erro ao Inserir Registro CLTPONTO"); return;

            }
            try
            {
                // oexclua.Add("ADMI");
                orow.AcceptChanges();
                try
                {
                    if (orow["NOTURNO"].ToString().Trim() == "")
                        AtualizaDados(bmTrabMov, bmTrabNoturno);
                    else
                        AtualizaDados(bmTrabNoturno, bmTrabMov);
                }
                catch (Exception) { }
            }
            catch (Exception E)
            {
                e.Cancela = Task.FromResult(true); 
                MessageBox.Show("Operação Falhou: " + E.ToString());
                // throw;
                return;
            }
            return;
        }
        
        private async Task<Int16> PesquiseNREG_Grave(DataRow orow)
        {
            Int16 retorne = 0;
            if ((orow.RowState == DataRowState.Added) || (orow.RowState == DataRowState.Modified))
            {
                double nreg = -1;
                if (orow.RowState == DataRowState.Added)
                {
                    try
                    {
                        DataSet odataset = await ApiServices.Api_QueryMulti(new List<string>(new string[]
                        { "Select MAX(NREG) as nreg from CLTPONTO" }));
                        nreg = Convert.ToDouble(odataset.Tables[0].Rows[0][0]);
                    }
                    catch (Exception) { }
                    if (nreg == -1)
                    {
                        //MessageBox.Show("Erro ao buscar numero exclusivo");
                        return retorne;
                    }
                    orow["NREG"] = nreg + 1;
                }
            }
            bool ok = await Prepara_Sql.OpereRegistroServidorAsync(orow, orow.RowState, "CLTPONTO"); // campos que serão excluidos//, new List<string>(new string[] { "QUANTV_ANT" }));
            if (ok) return 1;
            else return 0;
        }
        */

        public async void Padrao_DeletaRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            bool ok = false;
        //    bool noturno = orow["NOTURNO"].ToString().Trim() == "X"  ? true : false;
            string tabela = "CLTPONTO"; // orow.Table.TableName.ToUpper().Trim().Substring(1);
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
       

        public void Padrao_AfterDeletaRegistros(object sender, AlteraRegistroEventArgs e)
        {
            e.Cancela = true;
            if (bmTrabMov.Count > 0)
                AtualizaDados(bmTrabMov, bmTrabNoturno);
            else if (bmTrabNoturno.Count > 0)
                AtualizaDados(bmTrabNoturno, bmTrabMov);
            else
            {
                DataRow rowmestre = null;
                try
                {
                    rowmestre = (bmTrabCad.Current as DataRowView).Row;
                }
                catch (Exception)
                {
                    e.Cancela = false;
                    return;
                }
                if (rowmestre == null)
                {
                    return;
                }
                servPonto.Reinicialize_RegistroListaTrab(rowmestre);
                servPonto.AdoZere_Horas_ExcetoSemanaAnterior_casoExcluidosTodos(rowmestre["CODCAD"].ToString());
                servPonto.AdotblAssocCalcFields(rowmestre);
            }
        }


        // Preciso REVER para verificar Como Atuar Quando Tiver PREMIO
        // abril 2021
        public void AtualizaDados(BindingSource bmSource, BindingSource bmSourceOutro)
        {
            DataRow rowmestre = null;
            DataRow rowcorrente = null;
            try
            {
                rowmestre = (bmTrabCad.Current as DataRowView).Row;
            }
            catch (Exception)
            {
                return;
            }
            try
            {
                rowcorrente = (bmSource.Current as DataRowView).Row;
            }
            catch (Exception)
            {
                return;
            }

            servPonto.Reinicialize_RegistroListaTrab(rowmestre);
            servPonto.AdoZere_Horas_ExcetoSemanaAnterior(rowcorrente);
            DataTable otable = (bmSource.DataSource as DataView).ToTable();
            foreach (DataRow orow in otable.Rows)
            {
                servPonto.AdoHoras_Valores(orow);
            }

            
            // adciona valores diurno ou noturno 
            otable = (bmSourceOutro.DataSource as DataView).ToTable();
            foreach (DataRow orow in otable.Rows)
            {
                servPonto.AdoHoras_Valores(orow);
            }

            servPonto.AdotblAssocCalcFields(rowmestre);

            comum.oTrabCad.FuncaoSoma();
            comum.oTrabCad.ssColocaTotais();
        }








        #region Chamadas nas Consultas (direto noServidor)

        public async Task<Boolean> FiltroTrabMovRapidoNovo(string trab = "")
        {
            bool result = true;
            try
            {
                string datastring = "";
                if (trab != "") 
                  datastring = " (TRAB = '"+trab+"') AND ";
                
                // (DATA BETWEEN '" + inicio.ToString("yyyy-MM-dd") + "' AND '" + fim.ToString("yyyy-MM-dd") + "') " +
                string str = " (DATA >= CTOD('" + semana.AddDays(-SEMANAS_ANT).ToString("MM/dd/yyyy") + "') ) AND " +
                        " (DATA <= CTOD('" + semana.AddDays(+6).ToString("MM/dd/yyyy") + "')) ORDER BY TRAB,DATA";
                datastring = datastring + str;
                

                str = "SElect cltponto.id, data, cltponto.setor, fazmov, trab, tipomov, num_mod, codser,"
                    + " quant, semana, valor, bl, ok, diaria, dia1, dia2, dia3, dia4, dia5, dia6, dia7, dta_ini,"
                   + " dta_fim, obs, noturno,cltponto.nreg as nreg " +
                   "     from cltponto where  TIPOMOV = 'D' AND "
                  + datastring;


                List<string> lst = new List<string>();
                lst.Add(str);

               // string datastringdetalhe = "";
                
             datastring = "";
             if (trab != "")
                 datastring = " (CLTCAD.CODCAD = '" + trab + "') AND ";

                str = "  (alltrim(dtos(admi)) <> '') AND  " +
             " ( ADMI <= CTOD('" + semana.AddDays(+6).ToString("MM/dd/yyyy") +
             " ') ) " +

             " AND (((alltrim(dtos(cltcad.demi)) = '') AND (alltrim(dtos(cltcad.prazo)) = '')) OR "
             + "  (substr(dtos(cltcad.demi), 1, 8) >= '" + semana.ToString("yyyyMMdd") + "')) ";


                datastring = datastring + str; 
               

                str =
                    "SElect ID,CODCAD,NOMECAD,GLECAD,SETOR,ADMI,DEMI,PRAZO,SALBASE,OPCAO,AVULSO,MENSALISTA   from cltcad where "
                    + datastring + " order by NOMECAD ";


                lst.Add(str);


                DataSet dsDados = await ApiServices.Api_QueryMulti(lst);
                


                if ((dsDados == null) || (dsDados.Tables.Count == 0)) return false;
                // Tabela virtual

                CltCadastroCompleto = dsDados.Tables[1].Copy();
                // o Moovimento Anterior tem que SerSalvo antes da Alteração do procedimento 
                // que altera os dsDados para só reflitir os que estão na semna em curso: dsDados = AlteredsDados(dsDados);

                dtTrabMovAnt = dsDados.Tables[0].Clone();
                try
                {
                    dsDados.Tables[0].AsEnumerable().Where(orow => // orow.Field<String>("TIPOMOV").Trim() == "D" && 
                                                                   // && (!orow.IsNull("NOTURNO") && orow.Field<string>("NOTURNO").Trim() == "")
                       (orow.Field<DateTime>("DATA").CompareTo(semana) < 0)).
                       OrderBy(row => row.Field<DateTime>("DATA")).CopyToDataTable(dtTrabMovAnt, LoadOption.OverwriteChanges);
                   
                }
                catch (Exception)
                {
                }




                dsDados = AlteredsDados(dsDados);



                servPonto.tabListaTrab = servPonto.AdoLista();

                dtTrabMov = dsDados.Tables[0].Clone();
               // dtTrabMovAnt = dsDados.Tables[0].Clone();
                dtTrabNoturno = dsDados.Tables[0].Clone();
                // Acrescentado em abril 2021
                dtTrabPremio = dsDados.Tables[0].Clone();


                var rows = dsDados.Tables[0].AsEnumerable().Where(row =>
                    (row.Field<DateTime>("DATA").CompareTo(semana.AddDays(-7)) >= 0)).
                       OrderBy(orow => orow.Field<string>("TRAB")).ThenByDescending(orow => orow.Field<DateTime>("DATA"));
                if (rows != null)
                {
                    foreach (DataRow orow in rows)
                    {
                        if (Convert.ToDateTime(orow["DATA"]).CompareTo(semana) >= 0)
                            servPonto.AdoHoras_Valores(orow);
                        else
                            servPonto.AdoHoras_SemanaAnteriorNovo(orow);
                    }


                    try
                    {
                        dtTrabMov.Rows.Clear();
                        dsDados.Tables[0].AsEnumerable().Where(orow => 
                            (!orow.IsNull("NOTURNO") && orow.Field<string>("NOTURNO").Trim() == "")
                            && (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)

                            ).CopyToDataTable(dtTrabMov,LoadOption.OverwriteChanges);
                    }
                    catch (Exception)
                    {

                    }

                    try
                    {
                        dtTrabNoturno.Rows.Clear();
                        dsDados.Tables[0].AsEnumerable().Where(orow => 
                            (orow.Field<string>("NOTURNO").Trim() == "X")
                            && (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)
                            ).CopyToDataTable(dtTrabNoturno, LoadOption.OverwriteChanges);

                    }
                    catch (Exception)
                    {
                    }

                    try
                    {
                        dtTrabPremio.Rows.Clear(); 
                        dsDados.Tables[0].AsEnumerable().Where(orow =>
                            (orow.Field<string>("NOTURNO").Trim().ToUpper() == "P")
                            && (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)
                            ).CopyToDataTable(dtTrabPremio, LoadOption.OverwriteChanges);

                    }
                    catch (Exception)
                    {
                    }




                }



                servPonto.DataSemana = semana;

                dtTrabCad = servPonto.ConstruaCltCad(dsDados.Tables[1]);
                foreach (DataRow orow in dtTrabCad.Rows)
                {
                    servPonto.AdotblAssocCalcFields(orow);
                }
            }
            catch (Exception E)
            {
                result = false;
                return result;
            }
            dtTrabCad.TableName = "TABTRABCAD";
            dtTrabMov.TableName = "TABTRABMOV";
            dtTrabNoturno.TableName = "TABTRABNOTURNO";
            dtTrabPremio.TableName = "TABTRABPREMIO";

            dtTrabMovAnt.TableName = "TABTRABMOVANTERIOR";
            //dtTrabMovNotAnt.TableName = "TABTRABNOTURNOANTERIOR";
            PesquisaPontoProvisorio();
            return result;
        }

        public void PesquisaPontoProvisorio()
        {
            try
            {
                dictPontoProvisorioAviso = (from gr in dtTrabMovAnt.AsEnumerable().Where(row => 
                                                          (row.Field<string>("CODSER").Trim() == CODSERV_PONTOPROVISORIO)
                                                             )
                                                           group gr by new
                                                           {
                                                               trab = gr.Field<string>("TRAB"),

                                                           } into g
                                                           select new
                                                           {
                                                               trab = g.Key.trab,
                                                               lstData = g.Select(orow => orow.Field<DateTime>("DATA")).Distinct().ToList(),
                                                           }).ToDictionary(g => g.trab, g => g.lstData);

               foreach(KeyValuePair<string,List<DateTime>> keyValue in dictPontoProvisorioAviso)
               {
                    DataRow orow = null;
                    try
                    {
                        orow = dtTrabCad.AsEnumerable().Where(row => row.Field<string>("CODCAD").Trim() == keyValue.Key.Trim()).FirstOrDefault();
                    }
                    catch (Exception)
                    {
                    }
                    if (orow != null)
                    {
                        string datas = "";
                        string separador = "";
                        foreach(DateTime semana in keyValue.Value)
                        {
                            datas = datas + separador + semana.ToString("dd/MM");
                            separador = "; ";
                        }
                        orow.BeginEdit();
                        orow["APONTAMENTOPROV"] =  datas;
                        orow.EndEdit();
                        orow.AcceptChanges();
                    }
                 
               }
            
            }
            catch (Exception)
            {

                dictPontoProvisorioAviso = new Dictionary<string, List<DateTime>>();
            }
            
        }



        private DataSet AlteredsDados(DataSet dsDados)
        {
            // Deletar no Cadastro os que não estão incluido no pponto da semana em curso
            servPonto.CadAdmi = new Dictionary<string, DateTime>();
            servPonto.CadAdmi = dsDados.Tables[1].AsEnumerable()
                                 .ToDictionary(gr => gr.Field<string>("CODCAD"), gr => gr.Field<DateTime>("ADMI"));

            List<string> DistinctTrabSemana =

                (from gr in dsDados.Tables[0].AsEnumerable().Where( orow =>
                  
                            (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)
                )
                group gr by new
                {
                    trab = gr.Field<string>("TRAB")
                } into g
                select  g.Key.trab
                 ).ToList();

            // Necessário para conservar as caracteristicas
            DataTable cad_SoExistente = cad_SoExistente = dsDados.Tables[1].Clone();
            try
            {
                
                dsDados.Tables[1].AsEnumerable().Where(row => DistinctTrabSemana.Contains(row.Field<string>("CODCAD"))).CopyToDataTable(
                    cad_SoExistente,LoadOption.OverwriteChanges);

            }
            catch (Exception)
            {
                cad_SoExistente = cad_SoExistente = dsDados.Tables[1].Clone();
            }
            cad_SoExistente.TableName = dsDados.Tables[1].TableName;

            
            // deletar da tabela CltPONTO  Todos os registros 
            // QUE O 
            List<string> lstTrab_valido = cad_SoExistente.AsEnumerable().Select(row => row.Field<string>("CODCAD")).ToList();

            DataTable soCltPOnto = dsDados.Tables[0].Clone();
            try
            {
               dsDados.Tables[0].AsEnumerable().Where(row => lstTrab_valido.Contains(row.Field<string>("TRAB"))).CopyToDataTable(soCltPOnto,LoadOption.OverwriteChanges);
            }
            catch (Exception)
            {
                soCltPOnto = dsDados.Tables[0].Clone();
            }
         
            soCltPOnto.TableName = dsDados.Tables[0].TableName;
            dsDados.Tables.Clear();
            dsDados.Tables.Add(soCltPOnto);
            dsDados.Tables.Add(cad_SoExistente);
            return dsDados;
        }

        public DataSet PegDadosCltCadParaIncluir()
        {
            // dados que não estao no browse
            DataSet result = null;
            try
            {
                List<string> DistinctTrabSemanaDiurno =

              (from gr in dtTrabMov.AsEnumerable().Where(orow =>

                         (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)
              )
               group gr by new
               {
                   trab = gr.Field<string>("TRAB")
               } into g
               select g.Key.trab
               ).ToList();
                List<string> DistinctTrabSemanaNoturno =

                    (from gr in dtTrabNoturno.AsEnumerable().Where(orow =>

                               (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)
                    )
                     group gr by new
                     {
                         trab = gr.Field<string>("TRAB")
                     } into g
                     select g.Key.trab
                     ).ToList();

                List<string> DistinctTrabSemanaPremio =

                    (from gr in dtTrabPremio.AsEnumerable().Where(orow =>

                               (orow.Field<DateTime>("DATA").CompareTo(semana) == 0)
                    )
                     group gr by new
                     {
                         trab = gr.Field<string>("TRAB")
                     } into g
                     select g.Key.trab
                     ).ToList();


                List<string> DistinctTrabCadastro =

                    (from gr in dtTrabCad.AsEnumerable()
                     group gr by new
                     {
                         trab = gr.Field<string>("CODCAD")
                     } into g
                     select g.Key.trab
                     ).ToList();

                List<string> todosPresentes = DistinctTrabSemanaDiurno.Union(DistinctTrabSemanaNoturno).Union(DistinctTrabSemanaPremio).ToList();
                
                DataTable cad_Faltando = 
                    CltCadastroCompleto.AsEnumerable().Where(row => 
                    !todosPresentes.Contains(row.Field<string>("CODCAD"))
                    && !DistinctTrabCadastro.Contains(row.Field<string>("CODCAD"))
                    ).CopyToDataTable();
                cad_Faltando.TableName = "Cadastro";
                result = new DataSet();
                result.Tables.Add(cad_Faltando);
            }
            catch (Exception)
            {

            }
            // Trabalhadores Nesta Sema
          
            return result;

        }



        public async Task<DataSet> InclusaoNoBrowse()
        {
            DataSet result = null;
            string datastring = "";

            string str = " (alltrim(dtos(admi)) <> '') AND  (ADMI <= CTOD('" + semana.AddDays(6).ToString("MM/dd/yyyy") + "') ) AND " +
                    " ( ((alltrim(dtos(cltcad.demi)) = '') AND (alltrim(dtos(cltcad.prazo)) = '')) OR " +
                     "  (substr(dtos(cltcad.demi), 1, 8)  >= '" + semana.ToString("yyyyMMdd") + "' )) ";


            datastring = datastring + str +
            " AND Not EXISTS " +
            " (SELECT data, trab FROM cltponto " +
               " WHERE        (trab = cltcad.codcad) " + "AND (data = ctod('" +
                  semana.ToString("MM/dd/yyyy") + "'))) ";

            str = "SELECT ID,CODCAD,NOMECAD,GLECAD,SETOR,ADMI,DEMI,PRAZO,SALBASE,OPCAO,AVULSO,MENSALISTA from cltcad where " + datastring +
                  " order by NOMECAD ";


            List<string> lst = new List<string>();
            lst.Add(str);
            try
            {
                result = await ApiServices.Api_QueryMulti(lst);
            }
            catch (Exception E)
            {
                MessageBox.Show("Inclusao Cadastro:" + E.Message);
            }
            return result;
        }
        #endregion
    }
}