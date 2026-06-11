-- ============================================================================
-- Migration consolidada para deploy em produção do banco MLIBANIODADOS.
-- Data: 2026-05-08
-- Resumo das mudanças desta rodada de evolução do FrmCltPonto/FrmCltCodig em C#:
--
--   1) Audit log do cadastro CLTCODIG (F9) — tabela LOG_CLTCODIG_AUDIT
--   2) Unicidade do código de apontamento — UNIQUE INDEX em CLTCODIG.INDCOD
--   3) Performance do filtro de ponto (F8) — 2 índices em CLTPONTO
--      → O filtro do FrmConsultaTrab caiu de ~22s para ~0.1s nos testes locais
--
-- COMO RODAR:
--   - Faça BACKUP antes (SSMS → DB → Tasks → Back Up)
--   - Conectar no banco de produção como dbo / sa
--   - Abrir este arquivo no SSMS, conferir USE <database>, e executar (F5)
--   - Tudo é idempotente (IF NOT EXISTS) — pode rodar múltiplas vezes sem efeito
--
-- ATENÇÃO antes de rodar:
--   - Conferir se há INDCODs duplicados em CLTCODIG (UNIQUE INDEX falha se houver):
--       SELECT INDCOD, COUNT(*) FROM CLTCODIG GROUP BY INDCOD HAVING COUNT(*) > 1;
--     Se retornar linhas, deletar duplicatas mantendo o registro "correto" antes
--     de prosseguir.
--   - Os índices em CLTPONTO podem demorar a criar se a tabela tem vários
--     milhões de linhas em produção. Rodar fora do horário de pico.
-- ============================================================================

USE MLIBANIODADOS;
GO

PRINT '============================================';
PRINT '  MIGRATION 2026-05-08 — INICIANDO';
PRINT '============================================';

-- ============================================================================
-- 1) ÍNDICES EM CLTPONTO (F8 — performance do filtro de ponto)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_CLTPONTO_DATA_TIPOMOV'
                 AND object_id = OBJECT_ID('CLTPONTO'))
BEGIN
    PRINT '[1/4] Criando IX_CLTPONTO_DATA_TIPOMOV (pode demorar)...';
    CREATE INDEX IX_CLTPONTO_DATA_TIPOMOV
        ON CLTPONTO(DATA, TIPOMOV)
        INCLUDE (TRAB);
    -- Cobre a query principal: WHERE TIPOMOV='D' AND DATA BETWEEN ... ORDER BY TRAB
    PRINT '[1/4] OK.';
END
ELSE
    PRINT '[1/4] IX_CLTPONTO_DATA_TIPOMOV já existe — pulando.';

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'IX_CLTPONTO_TRAB_DATA'
                 AND object_id = OBJECT_ID('CLTPONTO'))
BEGIN
    PRINT '[2/4] Criando IX_CLTPONTO_TRAB_DATA (pode demorar)...';
    CREATE INDEX IX_CLTPONTO_TRAB_DATA ON CLTPONTO(TRAB, DATA);
    -- Para queries por trabalhador específico (tsTrabAnterior_Click)
    PRINT '[2/4] OK.';
END
ELSE
    PRINT '[2/4] IX_CLTPONTO_TRAB_DATA já existe — pulando.';

-- ============================================================================
-- 2) UNIQUE INDEX EM CLTCODIG (proteção contra duplicidade de código)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = 'UX_CLTCODIG_INDCOD'
                 AND object_id = OBJECT_ID('CLTCODIG'))
BEGIN
    -- Pré-validação: aborta se há duplicatas
    DECLARE @duplicatas int = (SELECT COUNT(*) FROM (
        SELECT INDCOD FROM CLTCODIG GROUP BY INDCOD HAVING COUNT(*) > 1
    ) d);
    IF @duplicatas > 0
    BEGIN
        PRINT '[3/4] ERRO: existem ' + CAST(@duplicatas AS varchar) + ' INDCODs duplicados em CLTCODIG.';
        PRINT '       Resolver antes de criar o UNIQUE INDEX. Query:';
        PRINT '       SELECT INDCOD, COUNT(*) FROM CLTCODIG GROUP BY INDCOD HAVING COUNT(*) > 1;';
    END
    ELSE
    BEGIN
        PRINT '[3/4] Criando UX_CLTCODIG_INDCOD...';
        CREATE UNIQUE INDEX UX_CLTCODIG_INDCOD ON CLTCODIG(INDCOD);
        PRINT '[3/4] OK.';
    END
END
ELSE
    PRINT '[3/4] UX_CLTCODIG_INDCOD já existe — pulando.';

-- ============================================================================
-- 3) TABELA LOG_CLTCODIG_AUDIT (audit log do FrmCltCodig)
-- ============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LOG_CLTCODIG_AUDIT')
BEGIN
    PRINT '[4/4] Criando tabela LOG_CLTCODIG_AUDIT + 3 índices...';
    CREATE TABLE LOG_CLTCODIG_AUDIT (
        Id              int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        DataHora        datetime2 NOT NULL CONSTRAINT DF_LOG_CLTCODIG_AUDIT_DataHora DEFAULT SYSUTCDATETIME(),
        Usuario         nvarchar(30) NOT NULL,    -- supervisor autenticado em FrmCltCodig
        UsuarioWin      nvarchar(50) NULL,         -- usuário Windows (rastreio extra)
        Operacao        nchar(1) NOT NULL,         -- 'I' Insert, 'U' Update, 'D' Delete
        Cltcodig_Id     int NULL,                  -- Id da row em CLTCODIG (se conhecido)
        INDCOD          nchar(2) NULL,             -- código (atual ou antigo)

        Antes_DESCCOD   nvarchar(21) NULL,         -- snapshot ANTES (NULL para INSERT)
        Antes_FATOR     numeric(19,4) NULL,
        Antes_HORAS     numeric(19,4) NULL,
        Antes_X_HORAS   numeric(19,4) NULL,
        Antes_NOTURNO   numeric(19,4) NULL,
        Antes_H_NORMAL  numeric(19,4) NULL,
        Antes_COMPATI   nchar(1) NULL,

        Depois_DESCCOD  nvarchar(21) NULL,         -- snapshot DEPOIS (NULL para DELETE)
        Depois_FATOR    numeric(19,4) NULL,
        Depois_HORAS    numeric(19,4) NULL,
        Depois_X_HORAS  numeric(19,4) NULL,
        Depois_NOTURNO  numeric(19,4) NULL,
        Depois_H_NORMAL numeric(19,4) NULL,
        Depois_COMPATI  nchar(1) NULL
    );

    CREATE INDEX IX_LOG_CLTCODIG_AUDIT_DataHora ON LOG_CLTCODIG_AUDIT(DataHora DESC);
    CREATE INDEX IX_LOG_CLTCODIG_AUDIT_INDCOD   ON LOG_CLTCODIG_AUDIT(INDCOD);
    CREATE INDEX IX_LOG_CLTCODIG_AUDIT_Usuario  ON LOG_CLTCODIG_AUDIT(Usuario);

    PRINT '[4/4] OK.';
END
ELSE
    PRINT '[4/4] Tabela LOG_CLTCODIG_AUDIT já existe — pulando.';

PRINT '============================================';
PRINT '  MIGRATION 2026-05-08 — CONCLUÍDA';
PRINT '============================================';

-- ============================================================================
-- VALIDAÇÃO PÓS-MIGRATION (rodar manualmente para conferir)
-- ============================================================================
-- Confirma índices em CLTPONTO:
--   SELECT name, is_unique FROM sys.indexes WHERE object_id = OBJECT_ID('CLTPONTO');
--
-- Confirma índice unique em CLTCODIG:
--   SELECT name, is_unique FROM sys.indexes WHERE object_id = OBJECT_ID('CLTCODIG');
--
-- Confirma tabela de audit existe e está vazia:
--   SELECT COUNT(*) FROM LOG_CLTCODIG_AUDIT;
--
-- Testa performance da query de filtro de ponto (deve ser ~100ms ou menos):
--   SET STATISTICS TIME ON;
--   SELECT COUNT(*) FROM CLTPONTO
--   WHERE TIPOMOV = 'D' AND DATA BETWEEN '2026-04-16' AND '2026-04-30';
-- ============================================================================
