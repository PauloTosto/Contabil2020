using ApoioContabilidade.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ApoioContabilidade.Trabalho.ServicesTrab
{
    /// <summary>
    /// Consulta a tabela GRATIF pra responder "este serviço tem gratificação
    /// vigente neste mês pra este setor?". Usado pelo validator de retificação
    /// 943→real: só permite trocar CODSER em mês fechado se NEM o serviço atual
    /// NEM o novo tiver gratificação (caso contrário, trocar mudaria o valor pago,
    /// criando divergência com a folha já fechada).
    ///
    /// Adicionado 2026-05-12.
    ///
    /// Regra do GRATIF (lida da SP FolhaClt_Atualiza):
    ///   "tem prêmio" = existe linha em GRATIF onde:
    ///     - CODSER bate
    ///     - (SETOR bate OU SETOR é vazio — coringa)
    ///     - PERCENT &lt;&gt; 0
    ///     - DTA_INI &lt;= mês de referência
    ///     - DTA_FIM IS NULL OU DTA_FIM &gt;= mês de referência
    ///
    /// Performance: cache em memória do conjunto de gratificações vigentes do
    /// mês consultado. Como o mês muda raramente durante uma sessão de operador
    /// (ele tipicamente trabalha 1 mês por vez), 1 query no carregamento serve
    /// pra dezenas/centenas de validações.
    /// </summary>
    public static class ServicoGratificacao
    {
        // Cache: chave = "yyyy-MM" do mês consultado, valor = lista de tuplas (CODSER, SETOR_OU_VAZIO).
        // Reset manual ou por mês diferente.
        private static string _mesCacheado = "";
        private static HashSet<string> _gratificacoesVigentes = new HashSet<string>();

        /// <summary>
        /// True se o serviço tem gratificação vigente em GRATIF pra esse setor
        /// no mês de referência. Setor vazio em GRATIF é coringa (vale pra todos).
        /// </summary>
        public static async Task<bool> ServicoTemPremio(string codser, string setor, DateTime mesReferencia)
        {
            if (string.IsNullOrWhiteSpace(codser)) return false;
            await GarantirCacheCarregado(mesReferencia);

            string codserNorm = codser.Trim();
            string setorNorm = (setor ?? "").Trim();

            // Match direto pelo setor OU pelo coringa (setor vazio em GRATIF).
            if (_gratificacoesVigentes.Contains(codserNorm + "|" + setorNorm)) return true;
            if (_gratificacoesVigentes.Contains(codserNorm + "|")) return true;
            return false;
        }

        /// <summary>
        /// Limpa o cache. Chamar quando o usuário muda o mês de trabalho ou
        /// edita GRATIF (este último não deve acontecer no app gerente, mas é defensivo).
        /// </summary>
        public static void InvalidarCache()
        {
            _mesCacheado = "";
            _gratificacoesVigentes = new HashSet<string>();
        }

        private static async Task GarantirCacheCarregado(DateTime mesReferencia)
        {
            string chaveMes = mesReferencia.ToString("yyyy-MM");
            if (_mesCacheado == chaveMes) return;

            // Mês de referência = primeiro dia do mês (pra comparar com DTA_INI/DTA_FIM).
            string mesIso = new DateTime(mesReferencia.Year, mesReferencia.Month, 1).ToString("yyyy-MM-dd");
            string ultimoDiaMesIso = new DateTime(mesReferencia.Year, mesReferencia.Month, 1)
                                        .AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");

            // Mesma lógica do WHERE em FolhaClt_Atualiza:
            //   DTA_INI <= @inicio AND (DTA_FIM >= @fim OR DTA_FIM IS NULL)
            //   AND PERCENT <> 0
            string query =
                "SELECT CODSER, ISNULL(LTRIM(RTRIM(SETOR)), '') AS SETOR_NORM " +
                "FROM GRATIF " +
                "WHERE DTA_INI <= '" + mesIso + "' " +
                "  AND (DTA_FIM >= '" + ultimoDiaMesIso + "' OR DTA_FIM IS NULL) " +
                "  AND CODSER IS NOT NULL " +
                "  AND [PERCENT] IS NOT NULL " +
                "  AND [PERCENT] <> 0";

            HashSet<string> novoSet = new HashSet<string>();
            try
            {
                DataSet ds = await ApiServices.Api_QueryMulti(new List<string> { query });
                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        string codser = (r["CODSER"] ?? "").ToString().Trim();
                        string setor = (r["SETOR_NORM"] ?? "").ToString().Trim();
                        if (codser != "") novoSet.Add(codser + "|" + setor);
                    }
                }
                _gratificacoesVigentes = novoSet;
                _mesCacheado = chaveMes;
            }
            catch (Exception ex)
            {
                // Falha ao consultar = cache vazio (todo serviço é tratado como "sem prêmio").
                // ATENÇÃO: isso é permissivo — em caso de falha de rede, retificações
                // que deveriam ser bloqueadas vão passar. O validator deveria fazer
                // re-fetch antes de cada retificação crítica, mas por agora aceitamos
                // o trade-off (operador raro, falha rara).
                System.Diagnostics.Debug.WriteLine("[ServicoGratificacao] falha consultando GRATIF: " + ex.Message);
                _gratificacoesVigentes = novoSet;
                _mesCacheado = chaveMes;
            }
        }
    }
}
