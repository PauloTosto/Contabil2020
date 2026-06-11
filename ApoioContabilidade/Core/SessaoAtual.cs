using System.Collections.Generic;
using System.Linq;

namespace ApoioContabilidade.Core
{
    /// <summary>
    /// Estado global da sessão atual do usuário logado.
    /// Populado pelo fluxo de autenticação; consultado por componentes que
    /// precisam restringir dados ao escopo do usuário (ex: TabelasIniciais
    /// filtra GLEBAS pelos SETORES autorizados).
    /// </summary>
    /// <remarks>
    /// Adicionado 2026-05-11 (passo 1 da migração "gerentes acessam direto").
    ///
    /// CONVENÇÃO IMPORTANTE: usuário sem nenhum setor cadastrado em
    /// USUARIO_SETORES é tratado como "escritório" — vê tudo. Só quem é
    /// explicitamente cadastrado vira escopo restrito. Isso preserva o
    /// comportamento atual de todos os usuários enquanto o cadastro de
    /// USUARIO_SETORES está vazio.
    ///
    /// Static thread-safety: o app é WinForms single-user single-thread, então
    /// state estático é aceitável. Se um dia virar multi-thread (BackgroundWorker
    /// chamando autenticação concorrente), revisar.
    /// </remarks>
    public static class SessaoAtual
    {
        /// <summary>Usuário logado (USUARIOSMLSA.USUARIO). Vazio se ninguém logou ainda.</summary>
        public static string Usuario { get; set; } = "";

        /// <summary>Privilegios do usuário logado (USUARIOSMLSA.privilegios).</summary>
        public static int Privilegios { get; set; } = 0;

        /// <summary>
        /// Lista de SETORES que o usuário pode ver/editar.
        /// Vazia = sem restrição (vê tudo). É o caso de usuários do escritório
        /// e de qualquer usuário ainda não cadastrado em USUARIO_SETORES.
        /// </summary>
        public static List<string> Setores { get; set; } = new List<string>();

        /// <summary>
        /// True se o usuário tem escopo restrito (>=1 setor cadastrado).
        /// False se vê tudo (escritório ou ainda não cadastrado).
        /// </summary>
        public static bool TemEscopoRestrito => Setores != null && Setores.Count > 0;

        /// <summary>
        /// Limpa a sessão (logout).
        /// </summary>
        public static void Limpa()
        {
            Usuario = "";
            Privilegios = 0;
            Setores = new List<string>();
        }

        /// <summary>
        /// Monta um fragmento SQL <c>LTRIM(RTRIM(SETOR)) IN ('S1','S2',...)</c> escapado,
        /// ou string vazia se o usuário não tem escopo restrito.
        /// O caller usa para concatenar em WHERE — ex:
        /// <code>"SELECT ... FROM GLEBAS WHERE FIM IS NULL " + SessaoAtual.FiltroSqlSetor("SETOR")</code>
        /// </summary>
        /// <remarks>
        /// Por que LTRIM+RTRIM: as colunas SETOR no schema legado (CLTCAD, CLTPONTO,
        /// GLEBAS, SETORES) são <c>nchar(2)</c>. Pior: o padding NÃO É consistente —
        /// dados antigos populados pelo Delphi/VFP têm padding LEADING (' 2'),
        /// dados novos inseridos via SQL Server padrão têm padding TRAILING ('2 ').
        /// Verificado 2026-05-11: SETORES.COD tem leading space; USUARIO_SETORES.SETOR
        /// (criada agora) tem trailing space. <c>RTRIM</c> sozinho não cobre o caso
        /// leading e o JOIN falha silenciosamente. <c>LTRIM(RTRIM(...))</c> normaliza
        /// dos dois lados — custo desprezível na cardinalidade real (5 setores).
        /// </remarks>
        public static string FiltroSqlSetor(string colunaSetor)
        {
            if (!TemEscopoRestrito) return "";
            // Escape de apóstrofo (defesa mínima — a API ainda usa string concat).
            string lista = string.Join(",", Setores.Select(s => "'" + (s ?? "").Replace("'", "''").Trim() + "'"));
            return " AND LTRIM(RTRIM(" + colunaSetor + ")) IN (" + lista + ") ";
        }
    }
}
