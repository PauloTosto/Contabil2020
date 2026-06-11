using System;

namespace ApoioContabilidade.Trabalho.ServicesTrab
{
    /// <summary>
    /// Regras de "mês fechado" — quando um lançamento de CLTPONTO está em mês
    /// cuja folha já foi paga e portanto certas alterações criariam divergência
    /// retroativa entre folha paga e ledger.
    ///
    /// Adicionado 2026-05-12. Lugar ÚNICO pra trocar a definição quando vier:
    ///   - Hoje: "fechado" derivado do calendário (passou dia 1 do mês seguinte).
    ///   - Futuro: "fechado" via flag explícito (ex: tabela PERIODOS_FECHADOS,
    ///     OU consultar NOVACLTFOLHA pra ver se aquele mês já tem folha gerada).
    /// Trocar a regra é editar este arquivo só.
    ///
    /// IMPORTANTE — granularidade por DIA, não por LINHA:
    /// CLTPONTO modela semana quinta→quarta. Uma semana pode atravessar virada
    /// de mês (ex: 28/mai a 03/jun). Quando junho chega, DIA1..DIA4 (maio)
    /// estão em mês fechado mas DIA5..DIA7 (junho) ainda nem aconteceram.
    /// Por isso DiaEstaFechado é por célula, não por registro.
    /// </summary>
    public static class RegrasMesFechado
    {
        /// <summary>
        /// True se a data caiu em mês cujo último dia já passou — ou seja,
        /// hoje &gt;= primeiro dia do mês seguinte ao da data.
        /// </summary>
        public static bool MesFechado(DateTime data)
        {
            DateTime primeiroDiaMesSeguinte = new DateTime(data.Year, data.Month, 1).AddMonths(1);
            return DateTime.Today >= primeiroDiaMesSeguinte;
        }

        /// <summary>
        /// True se o n-ésimo dia da semana de um registro CLTPONTO está em mês
        /// fechado. <paramref name="dataRegistro"/> é a quinta-feira (DIA1).
        /// <paramref name="indiceDia"/> é 1..7 (DIA1=quinta, DIA7=quarta seguinte).
        /// </summary>
        public static bool DiaEstaFechado(DateTime dataRegistro, int indiceDia)
        {
            if (indiceDia < 1 || indiceDia > 7)
                throw new ArgumentOutOfRangeException(nameof(indiceDia), "Índice deve ser 1..7");
            DateTime diaReal = dataRegistro.AddDays(indiceDia - 1);
            return MesFechado(diaReal);
        }

        /// <summary>
        /// True se TODOS os 7 dias da semana do registro estão em mês fechado.
        /// Usado pra bloquear inserção retroativa pura (semana inteira em mês fechado).
        /// Se a semana cruza virada de mês (ex: dia 28 de um mês a dia 03 do seguinte),
        /// isto retorna false — o lançamento ainda faz sentido pra preencher os dias
        /// do mês aberto.
        /// </summary>
        public static bool SemanaInteiraEmMesFechado(DateTime dataRegistro)
        {
            // Se DIA7 ainda está em mês aberto, semana cruzou virada — não é "inteiramente fechada".
            return DiaEstaFechado(dataRegistro, 7);
        }
    }
}
