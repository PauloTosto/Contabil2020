-- =============================================================================
-- Migration 2026-05-11: USUARIO_SETORES (v2 — tipos alinhados ao schema legado)
--
-- Modela "qual usuário pode ver/editar dados de quais setores (fazendas)".
-- Primeiro passo da migração que abre o app C# direto pros gerentes de fazenda
-- (substituindo o pipeline mensal de DBF→escritório).
--
-- CONVENÇÃO: usuário SEM nenhuma linha aqui = sem restrição (vê tudo).
--   - Preserva comportamento de TODOS os usuários atuais (escritório).
--   - Só quem for explicitamente cadastrado vira "escopo restrito" (gerente).
--
-- Granularidade: SETOR. Um gerente pode cuidar de >1 fazenda → várias linhas
-- pro mesmo USUARIO.
--
-- =============================================================================
-- HISTÓRICO (importante se vc já rodou versão anterior):
--
-- v1 (2026-05-11) usava varchar(10) pro SETOR. Funcionava por implicit cast,
-- mas SETOR no schema legado (CLTCAD, CLTPONTO, GLEBAS, SETORES) é nchar(2)
-- — string fixed-length Unicode com trailing space padding. Manter o MESMO
-- tipo evita conversão implícita em todo JOIN/IN e elimina classe inteira de
-- bug silencioso ("filtro não casa por padding"). Esta v2 dropa e recria.
--
-- Como a tabela ainda está vazia (nenhum gerente cadastrado), o DROP é seguro.
-- Se já houver dados quando vc rodar isso, faça backup antes.
-- =============================================================================

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'USUARIO_SETORES')
BEGIN
    DROP TABLE USUARIO_SETORES;
    PRINT 'USUARIO_SETORES anterior removida (recriando com tipo correto).';
END
GO

CREATE TABLE USUARIO_SETORES (
    USUARIO    varchar(20) NOT NULL,   -- compatível com USUARIOSMLSA.USUARIO; comparação tolera nchar via ANSI_PADDING.
    SETOR      nchar(2)    NOT NULL,   -- MESMO tipo de CLTCAD.SETOR, CLTPONTO.SETOR, GLEBAS.SETOR, SETORES.COD.
    CRIADO_EM  datetime    NOT NULL DEFAULT GETDATE(),
    CRIADO_POR varchar(20) NULL,
    CONSTRAINT PK_USUARIO_SETORES PRIMARY KEY (USUARIO, SETOR)
);

CREATE INDEX IX_USUARIO_SETORES_SETOR ON USUARIO_SETORES (SETOR);

PRINT 'USUARIO_SETORES criada com SETOR nchar(2).';
GO

-- =============================================================================
-- CADASTRO DOS GERENTES PILOTO (rodar uma vez após CREATE acima)
-- =============================================================================
-- Mapeamento confirmado 2026-05-11:
--   AdmCm    → 2  CORACAO DE MARIA
--   AdmPim   → 3  PIMENTEIRAS
--   AdmSj    → 4  SAO JOSE
--   AdmPetro → 8  PETROLINA
--   AdmPain  → 9  PAINEIRAS
--
--   AdmGeral, AcessoLivre, e demais setores (1, 5, 6, 7) ficam SEM cadastro
--   — AdmGeral vê tudo (escritório); fazendas sem cadastro seguem fora do
--   piloto do app C# (continuam no Delphi ou administradas pelo escritório).

INSERT INTO USUARIO_SETORES (USUARIO, SETOR, CRIADO_POR) VALUES
    ('AdmCm',    '2', SUSER_SNAME()),
    ('AdmPim',   '3', SUSER_SNAME()),
    ('AdmSj',    '4', SUSER_SNAME()),
    ('AdmPetro', '8', SUSER_SNAME()),
    ('AdmPain',  '9', SUSER_SNAME());

-- SQL Server faz padding automático: '2' vira '2 ' (nchar(2)).
GO

-- =============================================================================
-- VERIFICAÇÃO — rode depois pra conferir o cadastro
--
-- O JOIN usa LTRIM+RTRIM nos DOIS lados porque o padding é inconsistente:
-- SETORES.COD tem leading space (' 2'), USUARIO_SETORES.SETOR tem trailing ('2 ').
-- RTRIM sozinho não cobre o caso leading e o JOIN volta NomeFazenda=NULL.
-- =============================================================================
SELECT us.USUARIO,
       '|' + us.SETOR + '|'    AS SetorUsuario,
       '|' + s.COD    + '|'    AS SetorTabela,
       LTRIM(RTRIM(s.DESCRI))  AS NomeFazenda
FROM USUARIO_SETORES us
LEFT JOIN SETORES s
       ON LTRIM(RTRIM(s.COD)) = LTRIM(RTRIM(us.SETOR))
ORDER BY us.USUARIO;
GO

-- =============================================================================
-- OPERAÇÕES COMUNS
-- =============================================================================
-- Adicionar setor a um gerente:
--   INSERT INTO USUARIO_SETORES (USUARIO, SETOR, CRIADO_POR)
--   VALUES ('AdmCm', '5', SUSER_SNAME());
--
-- Remover setor:
--   DELETE FROM USUARIO_SETORES WHERE USUARIO = 'AdmCm' AND RTRIM(SETOR) = '5';
--
-- "Promover" gerente a admin (vê tudo):
--   DELETE FROM USUARIO_SETORES WHERE USUARIO = 'AdmCm';
--
-- Ver escopo de um usuário:
--   SELECT us.SETOR, s.DESCRI
--   FROM USUARIO_SETORES us
--   LEFT JOIN SETORES s ON RTRIM(s.COD) = RTRIM(us.SETOR)
--   WHERE us.USUARIO = 'AdmCm';
-- =============================================================================
