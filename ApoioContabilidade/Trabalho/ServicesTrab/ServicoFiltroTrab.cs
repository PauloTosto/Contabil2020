using ApoioContabilidade.Core;
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
        // 2026-05-12: alias público pra callers externos (FrmCltPonto, FrmRetificarProvisorio)
        // sem precisar duplicar a constante. Mantém a const interna pra uso local.
        public const string CODSERV_PONTOPROVISORIO_PUBLIC = "943";
        public ServCltPonto servPonto;
        public DataTable dtTrabMov = null;
        public DataTable dtTrabMovAnt = null;
        public DataTable dtTrabNoturno = null;
        public DataTable dtTrabMovNotAnt = null;

        public DataTable dtTrabCad = null;

        public BindingSource bmTrabCad = new BindingSource();
        public BindingSource bmTrabMov = new BindingSource();
        public BindingSource bmTrabNoturno = new BindingSource();
        public BindingSource bmTrabMovAnt = new BindingSource();
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
                    // Bug fix 2026-05-08: filtro de nome usava Substring que crashava se
                    // txNome.Length > NOMECAD.Length. StartsWith é equivalente e seguro.
                    (txNome == "" ? true : row.Field<string>("NOMECAD").StartsWith(txNome))
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
                    // Bug fix 2026-05-08: filtro de nome usava Substring que crashava se
                    // txNome.Length > NOMECAD.Length. StartsWith é equivalente e seguro.
                    (txNome == "" ? true : row.Field<string>("NOMECAD").StartsWith(txNome))
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
                    // Bug fix 2026-05-08: filtro de nome usava Substring que crashava se
                    // txNome.Length > NOMECAD.Length. StartsWith é equivalente e seguro.
                    (txNome == "" ? true : row.Field<string>("NOMECAD").StartsWith(txNome))
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
            e.Row["BL"] = ""; // BL (Quadra) opcional no noturno: vigia cobre gleba inteira
            // 2026-05-07: pré-selecionar MODELO=16 (ADMINISTRACAO) + CODSER=3 (VIGILANCIA),
            // que é o padrão do vigia/plantão noturno. Operador pode trocar se precisar.
            // Formato: NUM_MOD nchar(2) sem padding, CODSER nchar(3) right-aligned.
            e.Row["NUM_MOD"] = "16";
            e.Row["CODSER"] = "3".PadLeft(3); // "  3"
            e.Row["QUANT"] = 0;
            e.Row["OK"] = "";
            e.Row["DIARIA"] = 0;
            e.Row["NOTURNO"] = "X";
            e.Row["OBS"] = "";
            e.Row["TIPOMOV"] = "D";
            e.Row["NREG"] = 0;
        }
        // 2026-05-08: DtTrabMovPremio_TableNewRow removido (aba Premio descontinuada).

        /// <summary>
        /// Converte o valor de uma célula para double com defesa contra DBNull/null.
        /// Sem isso, validators explodiam com InvalidCastException ao encontrar
        /// QUANT/VALOR null em registros recém-criados ou vindos do banco como NULL.
        /// Adicionado 2026-05-08 (bug fix).
        /// </summary>
        private static double ToDoubleSafe(object value, double valorPadrao = 0)
        {
            if (value == null || value == DBNull.Value) return valorPadrao;
            try { return Convert.ToDouble(value); }
            catch (Exception) { return valorPadrao; }
        }

        /// <summary>
        /// Valida campos obrigatórios de um lançamento CLTPONTO antes de gravar.
        /// Retorna null se OK, ou mensagem específica listando o que faltou/está inválido.
        ///
        /// 2026-05-08 (B11): substitui o "Dados Inconsistentes com padrão" genérico,
        /// que era a última linha de defesa após R5 ter removido o e.Cancel dos
        /// validators dos combos. Agora o usuário sabe exatamente qual campo corrigir.
        /// </summary>
        private static string ValidaCamposObrigatorios(DataRow orow)
        {
            var faltas = new System.Collections.Generic.List<string>();

            // Comuns a todos os tipos de lançamento
            if (orow["FAZMOV"].ToString().Trim() == "") faltas.Add("Gleba (FAZMOV)");
            if (orow["NUM_MOD"].ToString().Trim() == "") faltas.Add("Modelo");
            if (orow["CODSER"].ToString().Trim() == "") faltas.Add("Serviço");
            if (orow.IsNull("DATA")) faltas.Add("Data");

            bool isPremio = orow["NOTURNO"].ToString().Trim() == "P";
            if (isPremio)
            {
                // Premio descontinuado em abril/2021, mas mantido por defesa em
                // profundidade caso row antiga venha do banco com NOTURNO='P'.
                if (ToDoubleSafe(orow["VALOR"]) <= 0) faltas.Add("Valor (deve ser > 0)");
                if (orow.IsNull("DTA_INI")) faltas.Add("Data Início");
                if (orow.IsNull("DTA_FIM")) faltas.Add("Data Fim");
                if (!orow.IsNull("DTA_INI") && !orow.IsNull("DTA_FIM")
                    && Convert.ToDateTime(orow["DTA_FIM"]).CompareTo(Convert.ToDateTime(orow["DTA_INI"])) < 0)
                {
                    faltas.Add("Data Fim anterior à Data Início");
                }
            }
            else
            {
                if (ToDoubleSafe(orow["QUANT"]) < 0) faltas.Add("Quantidade (deve ser >= 0)");
                bool nenhumDia = orow["DIA1"].ToString().Trim() == ""
                                 && orow["DIA2"].ToString().Trim() == ""
                                 && orow["DIA3"].ToString().Trim() == ""
                                 && orow["DIA4"].ToString().Trim() == ""
                                 && orow["DIA5"].ToString().Trim() == ""
                                 && orow["DIA6"].ToString().Trim() == ""
                                 && orow["DIA7"].ToString().Trim() == "";
                if (nenhumDia) faltas.Add("Pelo menos 1 dia da semana (DIA1..DIA7)");
            }

            if (faltas.Count == 0) return null;
            return "Não foi possível salvar. Corrija:\n  • " + string.Join("\n  • ", faltas);
        }

        /// <summary>
        /// Validação síncrona de "mês fechado" — bloqueia mudança em campos
        /// congelados (DIA_n por dia individual, DATA, NOTURNO, BL, NUM_MOD, FAZMOV).
        /// Retorna null se OK, string de erro caso contrário.
        ///
        /// NÃO valida CODSER aqui — isso depende de consulta async ao GRATIF.
        /// CODSER é validado em ValidaMesFechadoAsync, chamado de AlteraRegistrosOk.
        ///
        /// Adicionado 2026-05-12.
        /// </summary>
        private static string ValidaMesFechadoSync(DataRow orow, DataRowState tipoMuda)
        {
            // Inserção: bloqueia se a SEMANA INTEIRA está em mês fechado.
            // Semana cruzando virada (ex: 28/mai a 03/jun) é permitida pra preencher
            // os dias do mês aberto.
            if (tipoMuda == DataRowState.Added || tipoMuda == DataRowState.Detached)
            {
                if (orow.IsNull("DATA")) return null; // será pego pela validação de obrigatórios
                DateTime dataNova = Convert.ToDateTime(orow["DATA"]);
                if (RegrasMesFechado.SemanaInteiraEmMesFechado(dataNova))
                {
                    return "Não é possível inserir lançamento com data " +
                           dataNova.ToString("dd/MM/yyyy") +
                           " — toda a semana caiu em mês fechado. Solicite ao escritório.";
                }
                return null; // Insert em semana válida não tem campos congelados pra checar.
            }

            if (tipoMuda != DataRowState.Modified && tipoMuda != DataRowState.Deleted) return null;

            // Original disponível só pra Modified e Deleted.
            DateTime dataOrig = Convert.ToDateTime(orow["DATA", DataRowVersion.Original]);
            var faltas = new System.Collections.Generic.List<string>();

            // DATA: se mês fechado, não pode mudar (move semana inteira).
            if (RegrasMesFechado.MesFechado(dataOrig))
            {
                if (tipoMuda == DataRowState.Modified)
                {
                    DateTime dataAtual = Convert.ToDateTime(orow["DATA"]);
                    if (dataOrig != dataAtual)
                        faltas.Add("DATA do lançamento (mês fechado)");
                }
                if (tipoMuda == DataRowState.Deleted)
                {
                    return "Não é possível remover lançamento de mês fechado (" +
                           dataOrig.ToString("dd/MM/yyyy") + "). Solicite ao escritório.";
                }
            }

            if (tipoMuda == DataRowState.Modified)
            {
                // DIA_n: cada um independentemente, baseado no dia REAL daquela célula.
                for (int n = 1; n <= 7; n++)
                {
                    if (!RegrasMesFechado.DiaEstaFechado(dataOrig, n)) continue;
                    string col = "DIA" + n;
                    string ant = SafeStr(orow, col, DataRowVersion.Original);
                    string atu = SafeStr(orow, col, DataRowVersion.Current);
                    if (ant != atu)
                        faltas.Add(col + " (dia " + dataOrig.AddDays(n - 1).ToString("dd/MM") + " em mês fechado)");
                }

                // Outros campos congelados em mês fechado (qualquer dia da semana fechado serve
                // como gatilho — eles são por registro, não por dia).
                if (RegrasMesFechado.MesFechado(dataOrig))
                {
                    if (Mudou(orow, "NOTURNO")) faltas.Add("NOTURNO (mês fechado)");
                    if (Mudou(orow, "BL"))      faltas.Add("BL/Quadra (mês fechado)");
                    if (Mudou(orow, "NUM_MOD")) faltas.Add("Modelo (mês fechado)");
                    if (Mudou(orow, "FAZMOV"))  faltas.Add("Gleba/FAZMOV (mês fechado — afeta setor e rateio de custo)");
                }
            }

            if (faltas.Count == 0) return null;
            return "Não é possível alterar campos de lançamento em mês fechado:\n  • " +
                   string.Join("\n  • ", faltas) +
                   "\n\nPara retificar serviço (CODSER), use o botão 'Retificar provisório' " +
                   "(disponível para lançamentos com CODSER=" + CODSERV_PONTOPROVISORIO + ").";
        }

        /// <summary>
        /// Validação async — só checa CODSER vs GRATIF. Chamada de AlteraRegistrosOk
        /// antes do save servidor. Retorna null se OK, string de erro caso contrário.
        /// </summary>
        private static async Task<string> ValidaMesFechadoCodserAsync(DataRow orow, DataRowState tipoMuda)
        {
            if (tipoMuda != DataRowState.Modified) return null;
            DateTime dataOrig = Convert.ToDateTime(orow["DATA", DataRowVersion.Original]);
            if (!RegrasMesFechado.MesFechado(dataOrig)) return null;

            string codserAnt = SafeStr(orow, "CODSER", DataRowVersion.Original);
            string codserAtu = SafeStr(orow, "CODSER", DataRowVersion.Current);
            if (codserAnt == codserAtu) return null; // não mudou — nada a validar

            string setorAtu = SafeStr(orow, "SETOR", DataRowVersion.Current);
            DateTime mesRef = new DateTime(dataOrig.Year, dataOrig.Month, 1);

            bool antTemPremio = await ServicoGratificacao.ServicoTemPremio(codserAnt, setorAtu, mesRef);
            bool atuTemPremio = await ServicoGratificacao.ServicoTemPremio(codserAtu, setorAtu, mesRef);

            if (antTemPremio || atuTemPremio)
            {
                return "Não é possível trocar CODSER de '" + codserAnt + "' para '" + codserAtu + "' " +
                       "em mês fechado: pelo menos um dos serviços tem gratificação vigente em GRATIF " +
                       "(setor " + setorAtu.Trim() + ", mês " + mesRef.ToString("MM/yyyy") + "). " +
                       "Trocar criaria divergência com a folha já paga. Solicite ao escritório.";
            }
            return null;
        }

        private static string SafeStr(DataRow r, string col, DataRowVersion v)
        {
            try
            {
                if (!r.Table.Columns.Contains(col)) return "";
                object o = r[col, v];
                if (o == null || o == DBNull.Value) return "";
                return o.ToString().Trim();
            }
            catch (Exception) { return ""; }
        }

        private static bool Mudou(DataRow r, string col)
        {
            return SafeStr(r, col, DataRowVersion.Original) != SafeStr(r, col, DataRowVersion.Current);
        }

        public void DtTrabMov_BeforeAlteraRegistros(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            string erro = ValidaCamposObrigatorios(orow);
            if (erro != null)
            {
                MessageBox.Show(erro, "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancela = true;
                return;
            }

            // 2026-05-12: validação de mês fechado (parte síncrona — campos congelados puros).
            // CODSER vs GRATIF é checado em AlteraRegistrosOk (precisa async).
            string erroMes = ValidaMesFechadoSync(orow, e.TipoMuda);
            if (erroMes != null)
            {
                MessageBox.Show(erroMes, "Mês fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancela = true;
                return;
            }
        }

        public async void DtTrabMov__AlteraRegistrosOk(object sender, AlteraRegistroEventArgs e)
        {
            DataRow orow = e.Rows[0];
            // 2026-05-08 (B11): mesma validação que BeforeAlteraRegistros, agora via
            // helper único com mensagem específica por campo. Mantida aqui também
            // como cinto-suspensório: o pipeline pode chamar AlteraRegistrosOk sem
            // ter passado por BeforeAlteraRegistros em alguns caminhos legados.
            string erro = ValidaCamposObrigatorios(orow);
            if (erro != null)
            {
                MessageBox.Show(erro, "Campos obrigatórios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancela = true;
                return;
            }

            // 2026-05-12: validação de mês fechado — parte síncrona (cinto-suspensório
            // do BeforeAlteraRegistros) + parte async (CODSER vs GRATIF).
            string erroMesSync = ValidaMesFechadoSync(orow, e.TipoMuda);
            if (erroMesSync != null)
            {
                MessageBox.Show(erroMesSync, "Mês fechado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                e.Cancela = true;
                return;
            }
            string erroMesCodser = await ValidaMesFechadoCodserAsync(orow, e.TipoMuda);
            if (erroMesCodser != null)
            {
                MessageBox.Show(erroMesCodser, "Mês fechado — gratificação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
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

            // 2026-05-11: audit log de CLTPONTO. Snapshot Antes/Depois capturado
            // ANTES do AcceptChanges (que joga fora a versão Original). Falha no
            // audit é logada via Debug.WriteLine mas NÃO bloqueia (não pode virar SPOF).
            try
            {
                CltPontoSnapshot snapAntes = null;
                CltPontoSnapshot snapDepois = null;
                string opAudit = "?";
                if (e.TipoMuda == DataRowState.Modified)
                {
                    opAudit = "U";
                    snapAntes = CltPontoSnapshot.FromRow(orow, DataRowVersion.Original);
                    snapDepois = CltPontoSnapshot.FromRow(orow, DataRowVersion.Current);
                }
                else if (e.TipoMuda == DataRowState.Added || e.TipoMuda == DataRowState.Detached)
                {
                    opAudit = "I";
                    snapDepois = CltPontoSnapshot.FromRow(orow, DataRowVersion.Current);
                }
                else if (e.TipoMuda == DataRowState.Deleted)
                {
                    opAudit = "D";
                    snapAntes = CltPontoSnapshot.FromRow(orow, DataRowVersion.Original);
                }

                int idAudit = 0;
                try { if (!orow.IsNull("ID")) idAudit = Convert.ToInt32(orow["ID"]); } catch (Exception) { }
                string trabAudit = orow["TRAB"]?.ToString().Trim() ?? "";
                DateTime? dataAudit = null;
                try { if (!orow.IsNull("DATA")) dataAudit = Convert.ToDateTime(orow["DATA"]); } catch (Exception) { }
                string setorAudit = orow["SETOR"]?.ToString() ?? "";

                // Fire-and-forget intencional: não esperar o audit pra continuar o save.
                // Se falhar, Debug.WriteLine — não estoura UI nem bloqueia gravação.
                _ = AuditService.RegistrarCltPontoAuditAsync(
                    opAudit, SessaoAtual.Usuario, idAudit,
                    trabAudit, dataAudit, setorAudit,
                    snapAntes, snapDepois);
            }
            catch (Exception exAudit)
            {
                System.Diagnostics.Debug.WriteLine("[Audit CLTPONTO] falhou silenciosamente: " + exAudit);
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
            // F8 instrumentação 2026-05-08: medir tempo total + breakdown SQL vs processing.
            var swTotal = System.Diagnostics.Stopwatch.StartNew();
            long tApi = 0, tProcessing = 0;
            bool result = true;
            try
            {
                // F8 fix 2026-05-08: queries reescritas em T-SQL puro (sem dbo.CTOD, dbo.ALLTRIM,
                // dbo.DTOS) pra permitir uso de índices. Combinado com índices em CLTPONTO(DATA,TIPOMOV)
                // medido localmente: query CLTPONTO 22s → 0.1s; CLTCAD <0.05s.
                // Datas vão direto em formato ISO 'yyyy-MM-dd' (SQL Server entende sem função).
                // ADMI/DEMI/PRAZO são DATE (não string) — checar NULL em vez de alltrim(dtos)=''.
                string dataInicio = semana.AddDays(-SEMANAS_ANT).ToString("yyyy-MM-dd");
                string dataFim = semana.AddDays(+6).ToString("yyyy-MM-dd");
                string semanaIso = semana.ToString("yyyy-MM-dd");

                string filtroTrabPonto = (trab != "") ? " (TRAB = '" + trab + "') AND " : "";

                // 2026-05-11: filtro de escopo por SETOR. Só aplica em CARGA GERAL (trab=="");
                // consulta nominal (trab="X") sempre passa, porque é intent explícito do usuário
                // (ex: clicou em "anterior" pra um trabalhador específico).
                //
                // Para CLTPONTO: filtra por cltponto.setor (setor do LANÇAMENTO, não do cadastro).
                // Isso pega corretamente o caso "trabalhador eventual" — gerente B vê os pontos
                // que o trabalhador X (cadastrado em A) lançou nas glebas de B, mas NÃO vê os
                // pontos de X em A.
                string filtroSetorPonto = "";
                if (trab == "" && SessaoAtual.TemEscopoRestrito)
                {
                    // LTRIM+RTRIM: cltponto.setor é nchar(2) com padding inconsistente
                    // (leading em dados legados Delphi/VFP, trailing em dados novos).
                    filtroSetorPonto = " LTRIM(RTRIM(cltponto.setor)) IN (" +
                        string.Join(",", SessaoAtual.Setores.Select(s => "'" + s.Replace("'", "''").Trim() + "'")) +
                        ") AND ";
                }

                string str =
                    "SELECT cltponto.id, data, cltponto.setor, fazmov, trab, tipomov, num_mod, codser, " +
                    "       quant, semana, valor, bl, ok, diaria, dia1, dia2, dia3, dia4, dia5, dia6, dia7, dta_ini, " +
                    "       dta_fim, obs, noturno, cltponto.nreg as nreg " +
                    "FROM cltponto " +
                    "WHERE " + filtroTrabPonto + filtroSetorPonto +
                    "  TIPOMOV = 'D' " +
                    "  AND DATA >= '" + dataInicio + "' " +
                    "  AND DATA <= '" + dataFim + "' " +
                    "ORDER BY TRAB, DATA";

                List<string> lst = new List<string>();
                lst.Add(str);

                string filtroTrabCad = (trab != "") ? " (CLTCAD.CODCAD = '" + trab + "') AND " : "";

                // Para CLTCAD: trabalhador entra na lista se
                //   (a) é cadastrado num setor autorizado (caso comum), OU
                //   (b) lançou ponto em algum setor autorizado nesta janela (trabalhador eventual).
                // Sem (b), gerente B nunca veria o trabalhador eventual e a lista de pontos
                // ficaria órfã (CLTPONTO mostra ponto, CLTCAD não traz o cadastro pra cruzar).
                string filtroSetorCad = "";
                if (trab == "" && SessaoAtual.TemEscopoRestrito)
                {
                    string lista = string.Join(",", SessaoAtual.Setores.Select(s => "'" + s.Replace("'", "''").Trim() + "'"));
                    // LTRIM+RTRIM: cltcad.setor e cltponto.setor são nchar(2) com padding
                    // inconsistente (leading em dados legados Delphi/VFP, trailing em novos).
                    filtroSetorCad =
                        " ( LTRIM(RTRIM(CLTCAD.SETOR)) IN (" + lista + ") " +
                        "   OR CLTCAD.CODCAD IN (SELECT trab FROM cltponto " +
                        "                        WHERE LTRIM(RTRIM(cltponto.setor)) IN (" + lista + ") " +
                        "                          AND data >= '" + dataInicio + "' " +
                        "                          AND data <= '" + dataFim + "') " +
                        " ) AND ";
                }

                str =
                    "SELECT ID, CODCAD, NOMECAD, GLECAD, SETOR, ADMI, DEMI, PRAZO, SALBASE, OPCAO, AVULSO, MENSALISTA " +
                    "FROM cltcad " +
                    "WHERE " + filtroTrabCad + filtroSetorCad +
                    "  admi IS NOT NULL " +
                    "  AND admi <= '" + dataFim + "' " +
                    "  AND ( (demi IS NULL AND prazo IS NULL) OR demi >= '" + semanaIso + "' ) " +
                    "ORDER BY NOMECAD";

                lst.Add(str);

                var swApi = System.Diagnostics.Stopwatch.StartNew();
                DataSet dsDados = await ApiServices.Api_QueryMulti(lst);
                swApi.Stop();
                tApi = swApi.ElapsedMilliseconds;
                var swProc = System.Diagnostics.Stopwatch.StartNew();
                


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
                dtTrabNoturno = dsDados.Tables[0].Clone();


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

                    // 2026-05-08: filtro de NOTURNO='P' (aba Premio) removido — descontinuado.





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

            dtTrabMovAnt.TableName = "TABTRABMOVANTERIOR";
            //dtTrabMovNotAnt.TableName = "TABTRABNOTURNOANTERIOR";
            PesquisaPontoProvisorio();
            tProcessing = swTotal.ElapsedMilliseconds - tApi;
            swTotal.Stop();
            System.Diagnostics.Debug.WriteLine(
                "[FiltroTrabMovRapidoNovo] total=" + swTotal.ElapsedMilliseconds +
                "ms (API=" + tApi + "ms, processing=" + tProcessing + "ms)");
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

                // 2026-05-08: DistinctTrabSemanaPremio removido (aba Premio descontinuada).

                List<string> DistinctTrabCadastro =

                    (from gr in dtTrabCad.AsEnumerable()
                     group gr by new
                     {
                         trab = gr.Field<string>("CODCAD")
                     } into g
                     select g.Key.trab
                     ).ToList();

                List<string> todosPresentes = DistinctTrabSemanaDiurno.Union(DistinctTrabSemanaNoturno).ToList();
                
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

        /// <summary>
        /// Busca trabalhador GLOBAL — sem aplicar filtro de SETOR/escopo.
        /// Usado pelo gerente quando precisa lançar ponto pra trabalhador eventual
        /// de outra fazenda e ainda não sabe o código (caso "primeiro lançamento").
        ///
        /// Retorna SÓ trabalhadores APTOS na semana de referência (mesmo critério
        /// do <see cref="FiltroTrabMovRapidoNovo"/>): admitido até o fim da semana
        /// e (sem demissão E sem prazo) OU demitido a partir do início da semana.
        ///
        /// O termo é aplicado como LIKE em NOMECAD (case-insensitive) E como
        /// startsWith em CODCAD. Limita a 100 linhas pra evitar payload absurdo
        /// se o operador digitar termo curto demais.
        ///
        /// Adicionado 2026-05-11.
        /// </summary>
        /// <param name="termo">Parte do nome ou código (mínimo 3 caracteres, validado no caller).</param>
        /// <param name="semana">Quinta-feira de referência da semana de ponto. Define a janela
        /// de aptidão (admi &lt;= semana+6 e demi &gt;= semana).</param>
        public static async Task<DataTable> BuscaTrabalhadorGlobal(string termo, DateTime semana)
        {
            if (string.IsNullOrWhiteSpace(termo)) return null;
            string termoSql = termo.Trim().Replace("'", "''");
            string termoUpper = termoSql.ToUpper();

            // Mesmo critério de aptidão do FiltroTrabMovRapidoNovo:
            //   admi IS NOT NULL AND admi <= fim_da_semana
            //   AND ((demi IS NULL AND prazo IS NULL) OR demi >= inicio_da_semana)
            // Sem filtro de SETOR de propósito — é o ponto inteiro do método.
            string dataFim = semana.AddDays(6).ToString("yyyy-MM-dd");
            string semanaIso = semana.ToString("yyyy-MM-dd");

            string query =
                "SELECT TOP 100 CODCAD, NOMECAD, GLECAD, SETOR, ADMI, DEMI, PRAZO " +
                "FROM CLTCAD " +
                "WHERE ( UPPER(NOMECAD) LIKE '%" + termoUpper + "%' " +
                "        OR CODCAD LIKE '" + termoSql + "%' ) " +
                "  AND admi IS NOT NULL " +
                "  AND admi <= '" + dataFim + "' " +
                "  AND ( (demi IS NULL AND prazo IS NULL) OR demi >= '" + semanaIso + "' ) " +
                "ORDER BY NOMECAD";

            try
            {
                DataSet ds = await ApiServices.Api_QueryMulti(new List<string> { query });
                if (ds == null || ds.Tables.Count == 0) return null;
                return ds.Tables[0];
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}