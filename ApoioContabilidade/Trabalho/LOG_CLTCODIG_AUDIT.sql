-- ATENÇÃO: este script faz 2 coisas:
--  1) Cria a tabela LOG_CLTCODIG_AUDIT (audit log)
--  2) Cria UNIQUE INDEX em CLTCODIG.INDCOD (proteção contra duplicatas)
--
-- Tabela de auditoria para o cadastro CLTCODIG (códigos de apontamento).
-- Registra cada INSERT/UPDATE/DELETE feito via FrmCltCodig com snapshot
-- do estado Antes (UPDATE/DELETE) e Depois (INSERT/UPDATE).
--
-- Criado 2026-05-08 (F9).
--
-- USO: rodar este script UMA VEZ no banco de produção e no banco de teste local.
--      Depois disso o FrmCltCodig grava automaticamente nesta tabela.
--
-- CONSULTAS úteis:
--   SELECT TOP 100 * FROM LOG_CLTCODIG_AUDIT ORDER BY DataHora DESC;
--   SELECT * FROM LOG_CLTCODIG_AUDIT WHERE INDCOD = 'Y6' ORDER BY DataHora DESC;
--   SELECT Usuario, COUNT(*) AS qt FROM LOG_CLTCODIG_AUDIT GROUP BY Usuario ORDER BY qt DESC;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LOG_CLTCODIG_AUDIT')
BEGIN
    CREATE TABLE LOG_CLTCODIG_AUDIT (
        Id              int IDENTITY(1,1) NOT NULL PRIMARY KEY,
        DataHora        datetime2 NOT NULL CONSTRAINT DF_LOG_CLTCODIG_AUDIT_DataHora DEFAULT SYSUTCDATETIME(),
        Usuario         nvarchar(30) NOT NULL,    -- supervisor autenticado em FrmCltCodig
        UsuarioWin      nvarchar(50) NULL,         -- usuário Windows (rastreio extra)
        Operacao        nchar(1) NOT NULL,         -- 'I' Insert, 'U' Update, 'D' Delete
        Cltcodig_Id     int NULL,                  -- Id da row em CLTCODIG (NULL apenas se INSERT falhou em retornar Id)
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

    PRINT 'Tabela LOG_CLTCODIG_AUDIT criada com 3 índices.';
END
ELSE
BEGIN
    PRINT 'Tabela LOG_CLTCODIG_AUDIT já existe — nada a fazer.';
END

-- (2) UNIQUE INDEX em CLTCODIG.INDCOD para impedir duplicatas no banco.
-- Pré-requisito: não pode haver INDCODs duplicados ANTES de rodar isso.
-- Se houver, identificar com:
--   SELECT INDCOD, COUNT(*) FROM CLTCODIG GROUP BY INDCOD HAVING COUNT(*) > 1;
-- e resolver antes (deletar duplicatas, manter a "correta").

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UX_CLTCODIG_INDCOD' AND object_id = OBJECT_ID('CLTCODIG'))
BEGIN
    CREATE UNIQUE INDEX UX_CLTCODIG_INDCOD ON CLTCODIG(INDCOD);
    PRINT 'Unique index UX_CLTCODIG_INDCOD criado em CLTCODIG(INDCOD).';
END
ELSE
BEGIN
    PRINT 'Unique index UX_CLTCODIG_INDCOD já existe — nada a fazer.';
END
