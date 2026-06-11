using ApoioContabilidade.Models;
using System;
using System.Data;
using System.Threading.Tasks;

namespace ApoioContabilidade.Services
{
    /// <summary>
    /// Snapshot de uma linha de CLTCODIG (usado para audit Antes/Depois).
    /// </summary>
    public class CltCodigSnapshot
    {
        public string DESCCOD;
        public decimal? FATOR;
        public decimal? HORAS;
        public decimal? X_HORAS;
        public decimal? NOTURNO;
        public decimal? H_NORMAL;
        public string COMPATI;

        /// <summary>
        /// Constrói snapshot lendo de uma DataRow (na versão Original ou Current).
        /// Se a row foi deletada, use DataRowVersion.Original.
        /// </summary>
        public static CltCodigSnapshot FromRow(DataRow row, DataRowVersion v)
        {
            CltCodigSnapshot s = new CltCodigSnapshot();
            s.DESCCOD  = ToStr(row, "DESCCOD",  v);
            s.FATOR    = ToDec(row, "FATOR",    v);
            s.HORAS    = ToDec(row, "HORAS",    v);
            s.X_HORAS  = ToDec(row, "X_HORAS",  v);
            s.NOTURNO  = ToDec(row, "NOTURNO",  v);
            s.H_NORMAL = ToDec(row, "H_NORMAL", v);
            s.COMPATI  = ToStr(row, "COMPATI",  v);
            return s;
        }

        private static string ToStr(DataRow r, string col, DataRowVersion v)
        {
            try
            {
                object o = r[col, v];
                if (o == null || o == DBNull.Value) return null;
                return o.ToString();
            }
            catch (Exception) { return null; }
        }

        private static decimal? ToDec(DataRow r, string col, DataRowVersion v)
        {
            try
            {
                object o = r[col, v];
                if (o == null || o == DBNull.Value) return null;
                return Convert.ToDecimal(o);
            }
            catch (Exception) { return null; }
        }
    }

    /// <summary>
    /// Audit log para o cadastro de CLTCODIG. Grava na tabela LOG_CLTCODIG_AUDIT
    /// (criada via script Trabalho/LOG_CLTCODIG_AUDIT.sql).
    /// Falha ao gravar audit é logada mas NÃO bloqueia a operação principal.
    /// Adicionado 2026-05-08 (F9).
    /// </summary>
    public static class AuditService
    {
        private static DataTable _dtTemplate;

        private static DataTable CriaTemplate()
        {
            DataTable dt = new DataTable("LOG_CLTCODIG_AUDIT");
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("UsuarioWin", typeof(string));
            dt.Columns.Add("Operacao", typeof(string));
            dt.Columns.Add("Cltcodig_Id", typeof(int));
            dt.Columns.Add("INDCOD", typeof(string));
            // Antes
            dt.Columns.Add("Antes_DESCCOD", typeof(string));
            dt.Columns.Add("Antes_FATOR", typeof(decimal));
            dt.Columns.Add("Antes_HORAS", typeof(decimal));
            dt.Columns.Add("Antes_X_HORAS", typeof(decimal));
            dt.Columns.Add("Antes_NOTURNO", typeof(decimal));
            dt.Columns.Add("Antes_H_NORMAL", typeof(decimal));
            dt.Columns.Add("Antes_COMPATI", typeof(string));
            // Depois
            dt.Columns.Add("Depois_DESCCOD", typeof(string));
            dt.Columns.Add("Depois_FATOR", typeof(decimal));
            dt.Columns.Add("Depois_HORAS", typeof(decimal));
            dt.Columns.Add("Depois_X_HORAS", typeof(decimal));
            dt.Columns.Add("Depois_NOTURNO", typeof(decimal));
            dt.Columns.Add("Depois_H_NORMAL", typeof(decimal));
            dt.Columns.Add("Depois_COMPATI", typeof(string));
            return dt;
        }

        /// <summary>
        /// Registra uma entrada de auditoria de CLTCODIG.
        /// </summary>
        /// <param name="operacao">'I' Insert, 'U' Update, 'D' Delete</param>
        /// <param name="usuario">login do supervisor autenticado</param>
        /// <param name="cltcodigId">Id da row em CLTCODIG (pode ser 0 se Insert antes do servidor responder)</param>
        /// <param name="indcod">código (atual para U/I, antigo para D)</param>
        /// <param name="antes">snapshot Antes (null para Insert)</param>
        /// <param name="depois">snapshot Depois (null para Delete)</param>
        public static async Task<bool> RegistrarCltCodigAuditAsync(
            string operacao, string usuario, int cltcodigId, string indcod,
            CltCodigSnapshot antes, CltCodigSnapshot depois)
        {
            try
            {
                if (_dtTemplate == null) _dtTemplate = CriaTemplate();
                DataRow r = _dtTemplate.NewRow();
                r["Usuario"] = usuario ?? "";
                r["UsuarioWin"] = SafeWindowsUser();
                r["Operacao"] = operacao;
                if (cltcodigId > 0) r["Cltcodig_Id"] = cltcodigId;
                if (!string.IsNullOrEmpty(indcod)) r["INDCOD"] = indcod;

                if (antes != null) PreencheSnapshot(r, "Antes_", antes);
                if (depois != null) PreencheSnapshot(r, "Depois_", depois);

                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(
                    r, DataRowState.Added, "LOG_CLTCODIG_AUDIT");
                if (!ok)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "[AuditService] Falha ao gravar LOG_CLTCODIG_AUDIT (operacao=" + operacao + ", indcod=" + indcod + ")");
                }
                return ok;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[AuditService] Exception: " + ex);
                return false;
            }
        }

        private static void PreencheSnapshot(DataRow r, string prefix, CltCodigSnapshot s)
        {
            if (s.DESCCOD != null) r[prefix + "DESCCOD"] = s.DESCCOD;
            if (s.FATOR.HasValue) r[prefix + "FATOR"] = s.FATOR.Value;
            if (s.HORAS.HasValue) r[prefix + "HORAS"] = s.HORAS.Value;
            if (s.X_HORAS.HasValue) r[prefix + "X_HORAS"] = s.X_HORAS.Value;
            if (s.NOTURNO.HasValue) r[prefix + "NOTURNO"] = s.NOTURNO.Value;
            if (s.H_NORMAL.HasValue) r[prefix + "H_NORMAL"] = s.H_NORMAL.Value;
            if (s.COMPATI != null) r[prefix + "COMPATI"] = s.COMPATI;
        }

        private static string SafeWindowsUser()
        {
            try { return Environment.UserName; }
            catch (Exception) { return null; }
        }

        // =====================================================================
        // CLTPONTO AUDIT — adicionado 2026-05-11
        //
        // Espelha o pattern do CltCodig acima. Hooked em ServicoFiltroTrab.
        // Falha em gravar audit é logada via Debug.WriteLine mas NÃO bloqueia
        // a gravação principal — auditoria não pode virar SPOF.
        // =====================================================================

        private static DataTable _dtPontoTemplate;

        private static DataTable CriaTemplatePonto()
        {
            DataTable dt = new DataTable("LOG_CLTPONTO_AUDIT");
            dt.Columns.Add("Usuario", typeof(string));
            dt.Columns.Add("UsuarioWin", typeof(string));
            dt.Columns.Add("Operacao", typeof(string));
            dt.Columns.Add("CltPonto_Id", typeof(int));
            dt.Columns.Add("TRAB", typeof(string));
            dt.Columns.Add("DATA", typeof(DateTime));
            dt.Columns.Add("SETOR", typeof(string));
            // Antes
            dt.Columns.Add("Antes_FAZMOV", typeof(string));
            dt.Columns.Add("Antes_NUM_MOD", typeof(string));
            dt.Columns.Add("Antes_CODSER", typeof(string));
            dt.Columns.Add("Antes_QUANT", typeof(decimal));
            dt.Columns.Add("Antes_VALOR", typeof(decimal));
            dt.Columns.Add("Antes_DIA1", typeof(string));
            dt.Columns.Add("Antes_DIA2", typeof(string));
            dt.Columns.Add("Antes_DIA3", typeof(string));
            dt.Columns.Add("Antes_DIA4", typeof(string));
            dt.Columns.Add("Antes_DIA5", typeof(string));
            dt.Columns.Add("Antes_DIA6", typeof(string));
            dt.Columns.Add("Antes_DIA7", typeof(string));
            dt.Columns.Add("Antes_OBS", typeof(string));
            dt.Columns.Add("Antes_NOTURNO", typeof(string));
            // Depois
            dt.Columns.Add("Depois_FAZMOV", typeof(string));
            dt.Columns.Add("Depois_NUM_MOD", typeof(string));
            dt.Columns.Add("Depois_CODSER", typeof(string));
            dt.Columns.Add("Depois_QUANT", typeof(decimal));
            dt.Columns.Add("Depois_VALOR", typeof(decimal));
            dt.Columns.Add("Depois_DIA1", typeof(string));
            dt.Columns.Add("Depois_DIA2", typeof(string));
            dt.Columns.Add("Depois_DIA3", typeof(string));
            dt.Columns.Add("Depois_DIA4", typeof(string));
            dt.Columns.Add("Depois_DIA5", typeof(string));
            dt.Columns.Add("Depois_DIA6", typeof(string));
            dt.Columns.Add("Depois_DIA7", typeof(string));
            dt.Columns.Add("Depois_OBS", typeof(string));
            dt.Columns.Add("Depois_NOTURNO", typeof(string));
            return dt;
        }

        /// <summary>
        /// Registra uma entrada de auditoria de CLTPONTO. Chamado após gravação
        /// bem sucedida no servidor, ANTES do AcceptChanges (que limpa o
        /// snapshot Original).
        /// </summary>
        /// <param name="operacao">'I' Insert, 'U' Update, 'D' Delete</param>
        /// <param name="usuario">SessaoAtual.Usuario do app</param>
        /// <param name="cltPontoId">CLTPONTO.Id (0 se Insert antes do servidor responder)</param>
        /// <param name="trab">código do trabalhador</param>
        /// <param name="data">data do lançamento</param>
        /// <param name="setor">SETOR (nchar(2)) do lançamento</param>
        /// <param name="antes">snapshot Antes (null para Insert)</param>
        /// <param name="depois">snapshot Depois (null para Delete)</param>
        public static async Task<bool> RegistrarCltPontoAuditAsync(
            string operacao, string usuario, int cltPontoId,
            string trab, DateTime? data, string setor,
            CltPontoSnapshot antes, CltPontoSnapshot depois)
        {
            try
            {
                if (_dtPontoTemplate == null) _dtPontoTemplate = CriaTemplatePonto();
                DataRow r = _dtPontoTemplate.NewRow();
                r["Usuario"] = usuario ?? "";
                r["UsuarioWin"] = SafeWindowsUser();
                r["Operacao"] = operacao;
                if (cltPontoId > 0) r["CltPonto_Id"] = cltPontoId;
                if (!string.IsNullOrEmpty(trab)) r["TRAB"] = trab;
                if (data.HasValue) r["DATA"] = data.Value;
                if (!string.IsNullOrEmpty(setor)) r["SETOR"] = setor;

                if (antes != null) PreencheSnapshotPonto(r, "Antes_", antes);
                if (depois != null) PreencheSnapshotPonto(r, "Depois_", depois);

                bool ok = await Prepara_Sql.OpereRegistroServidorAsync(
                    r, DataRowState.Added, "LOG_CLTPONTO_AUDIT");
                if (!ok)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "[AuditService] Falha ao gravar LOG_CLTPONTO_AUDIT (operacao=" +
                        operacao + ", trab=" + trab + ", data=" + data + ")");
                }
                return ok;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("[AuditService] CltPonto Exception: " + ex);
                return false;
            }
        }

        private static void PreencheSnapshotPonto(DataRow r, string prefix, CltPontoSnapshot s)
        {
            if (s.FAZMOV != null) r[prefix + "FAZMOV"] = s.FAZMOV;
            if (s.NUM_MOD != null) r[prefix + "NUM_MOD"] = s.NUM_MOD;
            if (s.CODSER != null) r[prefix + "CODSER"] = s.CODSER;
            if (s.QUANT.HasValue) r[prefix + "QUANT"] = s.QUANT.Value;
            if (s.VALOR.HasValue) r[prefix + "VALOR"] = s.VALOR.Value;
            if (s.DIA1 != null) r[prefix + "DIA1"] = s.DIA1;
            if (s.DIA2 != null) r[prefix + "DIA2"] = s.DIA2;
            if (s.DIA3 != null) r[prefix + "DIA3"] = s.DIA3;
            if (s.DIA4 != null) r[prefix + "DIA4"] = s.DIA4;
            if (s.DIA5 != null) r[prefix + "DIA5"] = s.DIA5;
            if (s.DIA6 != null) r[prefix + "DIA6"] = s.DIA6;
            if (s.DIA7 != null) r[prefix + "DIA7"] = s.DIA7;
            if (s.OBS != null) r[prefix + "OBS"] = s.OBS;
            if (s.NOTURNO != null) r[prefix + "NOTURNO"] = s.NOTURNO;
        }
    }

    /// <summary>
    /// Snapshot de uma linha de CLTPONTO para audit Antes/Depois.
    /// Adicionado 2026-05-11.
    /// </summary>
    public class CltPontoSnapshot
    {
        public string FAZMOV;
        public string NUM_MOD;
        public string CODSER;
        public decimal? QUANT;
        public decimal? VALOR;
        public string DIA1, DIA2, DIA3, DIA4, DIA5, DIA6, DIA7;
        public string OBS;
        public string NOTURNO;

        public static CltPontoSnapshot FromRow(DataRow row, DataRowVersion v)
        {
            CltPontoSnapshot s = new CltPontoSnapshot();
            s.FAZMOV  = ToStr(row, "FAZMOV",  v);
            s.NUM_MOD = ToStr(row, "NUM_MOD", v);
            s.CODSER  = ToStr(row, "CODSER",  v);
            s.QUANT   = ToDec(row, "QUANT",   v);
            s.VALOR   = ToDec(row, "VALOR",   v);
            s.DIA1    = ToStr(row, "DIA1",    v);
            s.DIA2    = ToStr(row, "DIA2",    v);
            s.DIA3    = ToStr(row, "DIA3",    v);
            s.DIA4    = ToStr(row, "DIA4",    v);
            s.DIA5    = ToStr(row, "DIA5",    v);
            s.DIA6    = ToStr(row, "DIA6",    v);
            s.DIA7    = ToStr(row, "DIA7",    v);
            s.OBS     = ToStr(row, "OBS",     v);
            s.NOTURNO = ToStr(row, "NOTURNO", v);
            return s;
        }

        private static string ToStr(DataRow r, string col, DataRowVersion v)
        {
            try
            {
                if (!r.Table.Columns.Contains(col)) return null;
                object o = r[col, v];
                if (o == null || o == DBNull.Value) return null;
                return o.ToString();
            }
            catch (Exception) { return null; }
        }

        private static decimal? ToDec(DataRow r, string col, DataRowVersion v)
        {
            try
            {
                if (!r.Table.Columns.Contains(col)) return null;
                object o = r[col, v];
                if (o == null || o == DBNull.Value) return null;
                return Convert.ToDecimal(o);
            }
            catch (Exception) { return null; }
        }
    }
}
