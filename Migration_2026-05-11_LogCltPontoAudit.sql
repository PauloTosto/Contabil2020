-- =============================================================================
-- Migration 2026-05-11: LOG_CLTPONTO_AUDIT
--
-- Audit log de alterações em CLTPONTO. Espelha o pattern do LOG_CLTCODIG_AUDIT
-- (criado em 2026-05-08). Toda inserção/alteração/remoção de ponto grava uma
-- linha aqui com:
--   - quem fez (USUARIO do app + USUARIO do Windows)
--   - quando (CRIADO_EM default GETDATE())
--   - o quê (Operacao: I/U/D)
--   - chave do recurso (CltPonto_Id, TRAB, DATA, SETOR — pra busca rápida)
--   - estado antes e depois dos campos que importam (FAZMOV, NUM_MOD, CODSER,
--     QUANT, VALOR, DIA1..DIA7, OBS, NOTURNO)
--
-- Sem essa tabela: investigar "quem mudou o ponto do Jose Silva no dia 15?"
-- exige restore de backup. Com ela: SELECT direto.
-- =============================================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'LOG_CLTPONTO_AUDIT')
BEGIN
    CREATE TABLE LOG_CLTPONTO_AUDIT (
        Id           int IDENTITY(1,1) NOT NULL,
        CRIADO_EM    datetime NOT NULL DEFAULT GETDATE(),
        Usuario      varchar(20) NULL,    -- SessaoAtual.Usuario do app
        UsuarioWin   varchar(50) NULL,    -- Environment.UserName
        Operacao     char(1) NOT NULL,    -- 'I' Insert, 'U' Update, 'D' Delete
        CltPonto_Id  int NULL,            -- CLTPONTO.Id (NULL em Insert antes de o servidor responder)
        TRAB         varchar(20) NULL,    -- código do trabalhador (busca rápida)
        DATA         date NULL,           -- data do lançamento (busca rápida)
        SETOR        nchar(2) NULL,       -- setor do lançamento (pra "quem mexeu na minha fazenda?")

        -- Estado ANTES (NULL em Insert)
        Antes_FAZMOV   nchar(4) NULL,
        Antes_NUM_MOD  varchar(10) NULL,
        Antes_CODSER   varchar(10) NULL,
        Antes_QUANT    decimal(18,4) NULL,
        Antes_VALOR    decimal(18,4) NULL,
        Antes_DIA1     varchar(4) NULL,
        Antes_DIA2     varchar(4) NULL,
        Antes_DIA3     varchar(4) NULL,
        Antes_DIA4     varchar(4) NULL,
        Antes_DIA5     varchar(4) NULL,
        Antes_DIA6     varchar(4) NULL,
        Antes_DIA7     varchar(4) NULL,
        Antes_OBS      varchar(200) NULL,
        Antes_NOTURNO  varchar(2) NULL,

        -- Estado DEPOIS (NULL em Delete)
        Depois_FAZMOV   nchar(4) NULL,
        Depois_NUM_MOD  varchar(10) NULL,
        Depois_CODSER   varchar(10) NULL,
        Depois_QUANT    decimal(18,4) NULL,
        Depois_VALOR    decimal(18,4) NULL,
        Depois_DIA1     varchar(4) NULL,
        Depois_DIA2     varchar(4) NULL,
        Depois_DIA3     varchar(4) NULL,
        Depois_DIA4     varchar(4) NULL,
        Depois_DIA5     varchar(4) NULL,
        Depois_DIA6     varchar(4) NULL,
        Depois_DIA7     varchar(4) NULL,
        Depois_OBS      varchar(200) NULL,
        Depois_NOTURNO  varchar(2) NULL,

        CONSTRAINT PK_LOG_CLTPONTO_AUDIT PRIMARY KEY (Id)
    );

    -- Índices pra investigações típicas:
    --   "tudo de um trabalhador específico"
    CREATE INDEX IX_LOG_CLTPONTO_AUDIT_TRAB ON LOG_CLTPONTO_AUDIT (TRAB, DATA DESC);
    --   "tudo de um setor num período"
    CREATE INDEX IX_LOG_CLTPONTO_AUDIT_SETOR ON LOG_CLTPONTO_AUDIT (SETOR, CRIADO_EM DESC);
    --   "tudo que um usuário fez num dia"
    CREATE INDEX IX_LOG_CLTPONTO_AUDIT_USUARIO ON LOG_CLTPONTO_AUDIT (Usuario, CRIADO_EM DESC);

    PRINT 'LOG_CLTPONTO_AUDIT criada.';
END
ELSE
BEGIN
    PRINT 'LOG_CLTPONTO_AUDIT já existe — nada a fazer.';
END
GO

-- =============================================================================
-- CONSULTAS DE INVESTIGAÇÃO (exemplos pra deixar guardado)
-- =============================================================================
-- "Quem alterou o ponto do trabalhador 12345 na semana de 13/05?"
--   SELECT CRIADO_EM, Usuario, Operacao, Antes_QUANT, Depois_QUANT,
--          Antes_FAZMOV, Depois_FAZMOV
--   FROM LOG_CLTPONTO_AUDIT
--   WHERE TRAB = '12345' AND DATA BETWEEN '2026-05-13' AND '2026-05-19'
--   ORDER BY CRIADO_EM DESC;
--
-- "Tudo que aconteceu no setor 2 hoje:"
--   SELECT CRIADO_EM, Usuario, Operacao, TRAB, DATA, Depois_FAZMOV
--   FROM LOG_CLTPONTO_AUDIT
--   WHERE LTRIM(RTRIM(SETOR)) = '2' AND CRIADO_EM >= CAST(GETDATE() AS DATE)
--   ORDER BY CRIADO_EM DESC;
--
-- "Volume de alterações por usuário no mês:"
--   SELECT Usuario, Operacao, COUNT(*) AS qtd
--   FROM LOG_CLTPONTO_AUDIT
--   WHERE CRIADO_EM >= DATEADD(DAY, -30, GETDATE())
--   GROUP BY Usuario, Operacao
--   ORDER BY Usuario, Operacao;
-- =============================================================================
