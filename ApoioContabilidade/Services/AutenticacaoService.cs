using ApoioContabilidade.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace ApoioContabilidade.Services
{
    /// <summary>
    /// Resultado de tentativa de autenticação contra a tabela USUARIOSMLSA.
    /// </summary>
    public class AuthResult
    {
        public bool Sucesso { get; set; }
        public string Usuario { get; set; }
        public int Privilegios { get; set; }
        public string Mensagem { get; set; }

        /// <summary>
        /// Setores que o usuário pode acessar (lidos da tabela USUARIO_SETORES).
        /// Lista vazia = sem restrição (vê tudo) — caso típico do escritório.
        /// Adicionado 2026-05-11.
        /// </summary>
        public List<string> Setores { get; set; } = new List<string>();
    }

    /// <summary>
    /// Valida usuário+senha contra a tabela USUARIOSMLSA.
    /// Adicionado 2026-05-08.
    /// </summary>
    /// <remarks>
    /// LIMITAÇÃO CONHECIDA (a corrigir em B9 futuro):
    /// senhas estão em PLAIN TEXT na tabela USUARIOSMLSA.SENHA.
    /// Migrar para hash exige mexer em todos os pontos do sistema (Delphi+C#) que
    /// validam essa tabela — fora do escopo de uma rodada cirúrgica.
    /// </remarks>
    public static class AutenticacaoService
    {
        /// <summary>
        /// Autenticação base contra USUARIOSMLSA. Devolve o usuário, privilegios E
        /// a lista de setores autorizados (de USUARIO_SETORES). Não aplica regras
        /// de negócio específicas — quem decide se o usuário pode fazer X é o caller.
        ///
        /// Convenção: usuário sem nenhuma linha em USUARIO_SETORES = sem restrição
        /// (vê tudo). É o caso do escritório e de qualquer usuário ainda não cadastrado
        /// (preserva comportamento atual enquanto a tabela está vazia/em rollout).
        ///
        /// Adicionado 2026-05-11.
        /// </summary>
        public static async Task<AuthResult> AutenticarAsync(string usuario, string senha)
        {
            AuthResult result = new AuthResult { Sucesso = false };
            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrEmpty(senha))
            {
                result.Mensagem = "Usuário e senha são obrigatórios.";
                return result;
            }

            // Escapa apóstrofo (defesa mínima contra SQL injection — a API usa string concat).
            string usuarioSql = usuario.Replace("'", "''");
            string senhaSql = senha.Replace("'", "''");

            string queryAuth =
                "SELECT TOP 1 USUARIO, privilegios FROM USUARIOSMLSA " +
                "WHERE USUARIO = '" + usuarioSql + "' AND SENHA = '" + senhaSql + "'";

            // Carrega os setores no MESMO round-trip pra economizar latência.
            // Se USUARIO_SETORES não existir ainda no banco (antes da migration),
            // o try/catch por DataSet abaixo vai cair no fallback "lista vazia".
            string querySetores =
                "SELECT SETOR FROM USUARIO_SETORES WHERE USUARIO = '" + usuarioSql + "'";

            try
            {
                DataSet ds = await ApiServices.Api_QueryMulti(new List<string> { queryAuth, querySetores });
                if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                {
                    result.Mensagem = "Usuário ou senha incorretos.";
                    return result;
                }

                DataRow row = ds.Tables[0].Rows[0];
                int privilegios = 0;
                try { privilegios = Convert.ToInt32(row["privilegios"]); } catch (Exception) { }

                result.Sucesso = true;
                result.Usuario = row["USUARIO"].ToString().Trim();
                result.Privilegios = privilegios;

                // Setores: se a tabela existir e tiver linhas, popula. Caso contrário
                // (tabela ainda não criada, ou usuário sem cadastro), Setores fica
                // vazio = sem restrição (vê tudo).
                if (ds.Tables.Count >= 2 && ds.Tables[1].Rows.Count > 0)
                {
                    foreach (DataRow rs in ds.Tables[1].Rows)
                    {
                        string s = (rs["SETOR"] ?? "").ToString().Trim();
                        if (s != "") result.Setores.Add(s);
                    }
                }

                result.Mensagem = "Autenticado.";
                return result;
            }
            catch (Exception ex)
            {
                result.Mensagem = "Erro ao consultar servidor: " + ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Tenta autenticar como supervisor para operações de edição em CLTCODIG.
        /// Aceita qualquer usuário com privilegios diferente de 1 (AcessoLivre não pode editar).
        /// </summary>
        public static async Task<AuthResult> AutenticarSupervisorAsync(string usuario, string senha)
        {
            AuthResult result = await AutenticarAsync(usuario, senha);
            if (!result.Sucesso) return result;

            // Regra acordada 2026-05-08: AcessoLivre (privilegios=1) não pode editar CLTCODIG.
            if (result.Privilegios == 1)
            {
                result.Sucesso = false;
                result.Mensagem = "Usuário 'AcessoLivre' não tem permissão para editar Códigos de Apontamento.";
                return result;
            }
            return result;
        }
    }
}
