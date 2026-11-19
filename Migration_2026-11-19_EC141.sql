/* ============================================================================
   EC 141/2026 - escala 5x2 - migracao das stored procedures de folha
   Gerado em 2026-11-19 10:15 a partir das definicoes de producao + regras da EC 141.

   O QUE FAZ
     1. Confere se as 5 SPs do banco sao as mesmas usadas como base (trava).
     2. Cria/atualiza dbo.fn_DivisorHorasExtras e dbo.fn_SegundoDSRAtivo.
     3. ALTER PROCEDURE em: FolhaClt_SoLeitura, FolhaClt_Atualiza, FolhaClt,
        Pontos_Mes, CLT_PontoValorNew.

   REGRAS (corte em 16/12/2026; segundo degrau em 16/12/2027)
     Hora extra: valorizada pelo divisor do DIA em que foi feita.
       dia <  2026-12-16 -> 220 | dia >= 2026-12-16 -> 210 | dia >= 2027-12-16 -> 200
       (em dez/2026 as horas de 1 a 15 saem por 220 e as de 16 a 31 por 210)
     Sabado: a partir de 2026-12-16 e o segundo descanso semanal (o primeiro e 19/12/2026).
       Deixam de valer a dobra de diaria e a metade das horas efetivas do sabado;
       hora trabalhada no sabado deve ser apontada com os codigos D1 a D8.
     Horas efetivas (HEFETIVA na folha e na CUSTOFOLHA): de 2026-12-16 a 2027-12-15 a jornada e de
       42 h em 5 dias, entao o dia normal vale 8,4 h. De segunda a sexta, os codigos com diaria
       (X, 1 a 7, .1 a .9, meia diaria, Y) tem as horas efetivas multiplicadas por 1,05.
       A gratificacao por atividade NAO muda: continua calculada sobre as horas do codigo.

   COMPATIBILIDADE: SQL Server 2012/2014. Idempotente.
   COMO RODAR: SSMS, ou  sqlcmd -S <servidor> -E -I -b -i <este arquivo>
   ANTES: backup do banco. DEPOIS: conferir o ultimo bloco (conferencia final).
   DESFAZER: Migration_2026-11-19_EC141_ROLLBACK.sql
   Mesmas datas no C# (TDataControlTrabalho) e no Delphi (Funcoes.pas).
   ============================================================================ */
USE [MLIBANIODADOS];
GO
-- ---------------------------------------------------------------------------
-- TRAVA: confere se as 5 SPs do banco-alvo sao as mesmas da copia usada como base.
-- Se alguma foi alterada depois, o script para sem mudar nada (SET NOEXEC ON).
-- ---------------------------------------------------------------------------
SET NOCOUNT ON;
DECLARE @base TABLE (nome sysname, tam bigint, modif varchar(19));
INSERT @base VALUES ('FolhaClt_SoLeitura', 19296, '2020-09-02 10:53:03');
INSERT @base VALUES ('FolhaClt_Atualiza', 65304, '2024-11-18 06:52:17');
INSERT @base VALUES ('FolhaClt', 50065, '2021-10-11 14:44:21');
INSERT @base VALUES ('Pontos_Mes', 69829, '2023-03-22 14:15:51');
INSERT @base VALUES ('CLT_PontoValorNew', 42268, '2024-11-18 06:52:01');
DECLARE @problemas int = 0, @ja_aplicado int = 0;
SELECT @ja_aplicado = COUNT(*) FROM sys.sql_modules m JOIN sys.objects o ON o.object_id = m.object_id
 WHERE o.name IN (SELECT nome FROM @base) AND m.definition LIKE '%fn_SegundoDSRAtivo%';
SELECT @problemas = COUNT(*)
  FROM @base b
  LEFT JOIN sys.objects o ON o.name = b.nome AND o.type = 'P' AND o.schema_id = SCHEMA_ID('dbo')
  LEFT JOIN sys.sql_modules m ON m.object_id = o.object_id
 WHERE o.object_id IS NULL
    OR (m.definition NOT LIKE '%fn_SegundoDSRAtivo%'
        AND (LEN(m.definition) <> b.tam OR CONVERT(varchar(19), o.modify_date, 120) <> b.modif));
SELECT b.nome AS sp, b.tam AS tam_base, LEN(m.definition) AS tam_banco, b.modif AS alterada_base,
       CONVERT(varchar(19), o.modify_date, 120) AS alterada_banco,
       CASE WHEN o.object_id IS NULL THEN 'NAO EXISTE'
            WHEN m.definition LIKE '%fn_SegundoDSRAtivo%' THEN 'ja tem o patch EC141'
            WHEN LEN(m.definition) <> b.tam OR CONVERT(varchar(19), o.modify_date, 120) <> b.modif THEN 'DIFERENTE DA BASE'
            ELSE 'ok' END AS situacao
  FROM @base b
  LEFT JOIN sys.objects o ON o.name = b.nome AND o.type = 'P' AND o.schema_id = SCHEMA_ID('dbo')
  LEFT JOIN sys.sql_modules m ON m.object_id = o.object_id;
IF @problemas > 0
BEGIN
    RAISERROR('EC141: %d SP(s) do banco-alvo nao conferem com a base do patch. NADA foi alterado. Regerar o patch a partir da versao atual.', 16, 1, @problemas);
    SET NOEXEC ON;
END
ELSE IF @ja_aplicado > 0
    PRINT 'EC141: patch ja presente em ' + CAST(@ja_aplicado AS varchar(5)) + ' SP(s); reaplicando (idempotente).';
ELSE
    PRINT 'EC141: base conferida. Aplicando.';
GO
-- ---------------------------------------------------------------------------
-- Funcoes auxiliares (idempotente; sintaxe SQL Server 2012+)
-- ---------------------------------------------------------------------------
IF OBJECT_ID('dbo.fn_DivisorHorasExtras', 'FN') IS NULL
    EXEC('CREATE FUNCTION dbo.fn_DivisorHorasExtras (@data date) RETURNS numeric(3,0) AS BEGIN RETURN 220 END');
GO
-- Divisor da hora extra na data informada (data do DIA trabalhado).
-- RETURNS numeric(3,0): mesmo tipo que o SQL Server atribui ao literal 220; com
-- smallint/int a divisao ganha casas decimais e o Round(...,2) pode mudar 1 centavo.
ALTER FUNCTION dbo.fn_DivisorHorasExtras (@data date)
RETURNS numeric(3,0)
WITH SCHEMABINDING
AS
BEGIN
    RETURN CASE
        WHEN @data < '2011-12-01' THEN 240
        WHEN @data < '2026-12-16' THEN 220
        WHEN @data < '2027-12-16' THEN 210
        ELSE                            200
    END;
END;
GO
IF OBJECT_ID('dbo.fn_SegundoDSRAtivo', 'FN') IS NULL
    EXEC('CREATE FUNCTION dbo.fn_SegundoDSRAtivo (@data date) RETURNS bit AS BEGIN RETURN 0 END');
GO
-- 1 quando, na data informada, o sabado ja e o segundo descanso semanal (escala 5x2).
ALTER FUNCTION dbo.fn_SegundoDSRAtivo (@data date)
RETURNS bit
WITH SCHEMABINDING
AS
BEGIN
    RETURN CASE WHEN @data < '2026-12-16' THEN 0 ELSE 1 END;
END;
GO
IF OBJECT_ID('dbo.fn_FatorHorasEfetivas', 'FN') IS NULL
    EXEC('CREATE FUNCTION dbo.fn_FatorHorasEfetivas (@data date) RETURNS numeric(5,4) AS BEGIN RETURN 1 END');
GO
-- Fator das horas efetivas de um dia normal (segunda a sexta, codigos com diaria: X, 1 a 7, .1 a .9, /, Y).
-- No primeiro ano da escala 5x2 a jornada e de 42 h em 5 dias = 8,4 h por dia (8,4 / 8 = 1,05):
-- X vale 8,40; meia diaria 4,20; 4 horas 4,20. Antes e depois desse ano o fator e 1.
-- Vale so para as horas efetivas gravadas (HEFETIVA); a gratificacao continua sobre as horas do codigo.
ALTER FUNCTION dbo.fn_FatorHorasEfetivas (@data date)
RETURNS numeric(5,4)
WITH SCHEMABINDING
AS
BEGIN
    RETURN CASE WHEN @data < '2026-12-16' THEN 1.0000
                WHEN @data < '2027-12-16' THEN 1.0500
                ELSE 1.0000 END;
END;
GO
-- ===========================================================================
-- FolhaClt_SoLeitura
-- ===========================================================================
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
--=============================================
 --Author:		<Author,,Name>
 --Create date: <Create Date,,>
 --Description:	<Description,,>
-- --=============================================
ALTER PROCEDURE [dbo].[FolhaClt_SoLeitura]  
	-- Add the parameters for the stored procedure here
	
	@inicio date,
	@fim date,
  @naograve int
AS
BEGIN
	
	SET LANGUAGE us_english;
/*	DECLARE @inicio date,
 	@fim date,@naograve int;
    SET @inicio ='2019-08-01'
	SET @fim ='2019-08-31';
	SET @naograve = 1;
	*/

	SET @fim = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@fim)+1,0));

	DECLARE @comparafolha bit = 0;
	DECLARE @CUSTO bit = 1;


	SET NOCOUNT ON;
	-- pega a semana do ponto anterior ao inicio do mes
	DECLARE @primeirasemana date;
	SET @primeirasemana = @inicio;
	WHILE (DATENAME(weekday,@primeirasemana) <> 'Thursday') 
	begin
      	 SET @primeirasemana= DATEADD(dd,-1, @primeirasemana);
	end;

--	TABELAS AUXILIARES
	-- Salario minimo e fgts
	DECLARE @SalMinFgts TABLE ([DATA] date,VALOR numeric(18,4),FGTS numeric(18,4));
	insert @SalMinFgts 
	EXECUTE [dbo].[SalMinFgts]
	        @fim; 
	DECLARE @SalMin numeric(18,4);		
	DECLARE @taxaFGTS numeric(18,4);
	SELECT @SalMin=VALOR,@taxaFGTS=FGTS from @SalMinFgts;

	-- tabeal do INNS  com tipo definido
	DECLARE @TabInss TipoTabelaINSS;
	insert into @TabInss
	SELECT [data], faixa, valor1, valor2, taxa FROM TABINSS  
         where  (Format(TABINSS.DATA,'yyyyMM') <= Format(@fim,'yyyyMM'))
       ORDER BY [DATA] DESC,faixa; 

    -- tabelas IRFONTE com tipo definido
	DECLARE @TabIrFonte TipoTabelaIRFonte;
	insert into @TabIrFonte
	SELECT TABIR.data, faixa, base1, base2, aliq, TABIR.deduzir,
    (SELECT MAX(irdedu.DEDUCAO) from irdedu where (TABIR.Data = IRDEDU.DATA) ) deduGeral
    FROM TABIR
	where  Format(TABIR.[data], 'yyyyMM') <= Format(@fim,'yyyyMM')
      ORDER BY TABIR.DATA DESC, faixa;

   -- tabela SALFAM
   DECLARE @TabSalFam TipoTabelaSalFam;
   insert into @TabSalFam
      SELECT data, faixa, valor1, valor2, salfam FROM SALFAM 
	  where  Format([data], 'yyyyMM') <= Format(@fim,'yyyyMM')
	  ORDER BY DATA DESC,faixa;
	

    --tabela gratificação
	DECLARE @tabelagratif TABLE (num_mod nvarchar(2),gcodser nvarchar(3),gsetor nvarchar(2),perc_diaria numeric(18,4),
	          vlr_real numeric(18,4),dta_ini date,dta_fim date  );
	insert into @tabelagratif                        
    SELECT NUM_MOD,CODSER,iif(SETOR='','  ',SETOR),[PERCENT],VLRDIARIA,DTA_INI,DTA_FIM FROM GRATIF WHERE 
                (DTA_INI <= @inicio) and ((DTA_FIM >= @fim) 
              or (DTA_FIM is null) ) and 
			  (CODSER is not null) and ([PERCENT] is not null) and (VLRDIARIA is not null);
	
	-- tabela cltcodig
	DECLARE @tabcltcodigo TABLE (INDCOD nchar(2),DESCOD nvarchar(21),FATOR numeric(19,4),
	 HORAS numeric(19,4),X_HORAS numeric(19,4),NOTURNO numeric(19,4),
	 H_NORMAL numeric(19,4),COMPATI nchar(1));
	 insert into @tabcltcodigo SELECT  INDCOD ,DESCCOD ,FATOR ,HORAS ,X_HORAS ,NOTURNO ,
	 H_NORMAL ,COMPATI from cltcodig;

    -- montagem da "tabela" vet_sabado
	DECLARE @vet_sabado TABLE(oindcod nvarchar(2));
    DECLARE @i int;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( ' '+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 10)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( '.'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( 'C'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
-- numero de dias no mes	
   DECLARE @ndiasmes int = DateDiff(day,@inicio,@fim)+1;

-- Este é o listaDados (numa store procedure) retorna as informações do CLTCAD e FERIAS 
  
   DECLARE @tabelaLista TABLE (CODCAD nvarchar(4),NOMECAD nvarchar(60),DEMI date,
     CATEGORIA nvarchar(30),CBO numeric(19,4),MENSALISTA nchar(1),
	 CONTA1 nchar(10),BANCO1 nchar(3),AGENCIA1 nchar(8),CPF nchar(11),END_RUA nvarchar(50) ,
	   END_CID nvarchar(25) ,END_UF nchar(2),END_CEP nchar(10),
       SETOR nchar(2), salbase numeric(18,4),ult_atual date,
       Glecad NVARCHAR(4),admi date,demitido int,admitido int,diastrab int,
         tsal numeric(18,4), soma_perc_conf numeric(18,4),      ndepend int,emgozo int,irdepend int, salcontrib numeric(18,4),prov_ferias numeric (18,4),
           prov_decimo numeric(18,4), inss numeric(18,4), salfam numeric(18,4),  FERIAS_BASE numeric(18,4),
		   FERIAS_MEDIA numeric(18,4), SALMEDIO_FERIAS numeric(18,4));
       insert into @tabelaLista
       EXECUTE  [dbo].[RetornaListaDados] 
          @inicio
          ,@fim
          ,@SalMin;
         
 --XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


 
  IF (@FIM < '2018-11-01') 
  BEGIN
    IF OBJECT_ID('tempdb..#DOM_FERIADO') IS NOT NULL
       DROP TABLE #DOM_FERIADO
    CREATE TABLE #DOM_FERIADO (COD NVARCHAR(4),VLR_HXS numeric(19,4), HORAS_DOM numeric(19,4),
		  HORAS_FER numeric(19,4),
		  VLR_DOM numeric(19,4),
		  VLR_FER numeric(19,4)
     );
    INSERT INTO #DOM_FERIADO 
		SELECT TRAB, VLR_HXS,  0.0 ,  0.0 ,  0.0 , 0.0  FROM NOVACLTfOLHA WHERE 
		DATA = @FIM AND VLR_HXS <> 0;    
    DECLARE @ndia int;
	DECLARE @dia NVARCHAR(4);
	DECLARE @tabelageral TABLE (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)) 
	set @ndia = 0;
	while (@ndia < 7) 
	begin
	    set @dia ='DIA'+CONVERT(nvarchar,(@ndia+1),1);
	    DECLARE @literal as varchar(5120);
	    SET @literal =
	    'select TRAB,SETOR,fazmov,BL,NUM_MOD,CODSER,cltponto.noturno as noturno,Data as Semana,quant,ok,'+
        'DATENAME(weekday, DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA))   as diasemana,'+
       'DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) as dtadia, '+@dia+  ', '+--cdg.indcod,'+
       'cdg.fator as Diarias,cdg.HORAS as Horas_efetivas,cdg.X_HORAS as Horas50,cdg.NOTURNO as Horas100,cdg.H_NORMAL as Horasfe,cdg.COMPATI'+
        ' from cltponto LEFT OUTER JOIN CLTCODIG as cdg '+
         'on ( cdg.INDCOD = '+@dia+')  where '+
        '(data between '''+CAST(@primeirasemana as NVARCHAR(12))+''' and '''+CAST(@fim as NVARCHAR(12))+''') and ('+@dia+' <> ''  '') and '+ 
       ' (DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) >='''+ CAST(@inicio as NVARCHAR(12))+''') and '+
        '(DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) <= '''+CAST(@fim as NVARCHAR(12))+''') '+
		' AND EXISTS (SELECT COD FROM #DOM_FERIADO WHERE COD = TRAB)'; 
      
	    INSERT INTO @TABELAGERAL EXEC(@LITERAL);
        set @ndia = @ndia+1;
    end;
	
   --processamneto corresponde ao Gera_Valores da versão Delphi (sem o processamento do ponto)
   DECLARE @tabelaprocess TABLE
   (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  Diarias numeric(18,4),
	  Diariasn numeric(18,4),
	  Horasfe numeric(18,4),
	  Horasfen numeric(18,4),
	  Horas50 numeric(18,4),
	  Horas50n numeric(18,4),
	  Horas100 numeric(18,4),
	  horas_efetivas numeric(18,4),
	  Diarias_adc numeric(18,4),
	 
	  compati nvarchar(1),
	  quant  numeric(18,4), 
	  ok nvarchar(1),
	  Horas_Dom numeric(18,4),  -- horas feriado que cairam num domingo
	  Horas_Domn numeric(18,4) --   
	  );
	  
   
	insert into  @tabelaprocess
	select trab ,setor ,gleba ,bl ,num_mod ,codser ,
	  noturno , 
	  semana , diasemana , dtadia,indcod ,
	  iif(diarias>0,iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiarias,
	 
	 iif(diarias>0 and (noturno='X'),iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiariasn,
	 
	  iif(horasfe>0,horasfe,0) as pHorasfe,
	  
	  iif((horasfe>0) and (noturno='X'),horasfe,0) as pHorasfen,
	  
	  iif(horas50>0,horas50,0) as pHoras50,
	  iif((horas50>0) and (noturno='X'),horas50,0) as pHoras50n,

	  iif(horas100>0,horas100,0) as pHoras100,
	  
	  iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0,iif( ( indcod = 'X'),horas_efetivas/2, horas_efetivas),horas_efetivas),0) * iif((diarias>0) and (diasemana not in ('Saturday','Sunday')), dbo.fn_FatorHorasEfetivas(dtadia), 1) as horas_efetivas,
	  -- diaria_adc
	  iif(not Exists(Select gcodser from @tabelagratif as grat where ((codser = grat.gcodser) and 
	  ((setor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))),0,
	     iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and (rtrim(indcod)='X'), 
	       ( (horas_efetivas/2) *  (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser))),
		(horas_efetivas * (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser)) )),0)) as diaria_adc

	  ,compati,quant,ok ,

	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0),horasfe,0) as pHoras_Dom,
	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0) and (noturno='X'),horasfe,0) as pHoras_Domn

	  from @tabelageral ;

	
	  
  -- Base do Calculo da Folha por TRABALHADOR (agregada) 
      DECLARE @tabelaagregada TABLE (trab nvarchar(4),
	    dias numeric(18,4),
	    diarias numeric(18,4), diarias_n numeric(18,4),
	     horasfe numeric(18,4),horasfen numeric(18,4),
	     horas50 numeric(18,4),horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	      horas_domn numeric(18,4) );  
	  
	  insert into @tabelaagregada
	  select trab,
	  Round(SUM(Diarias)/8,2),
	  SUM(Diarias) as diarias ,
	  SUM(diariasn) as diarias_n,
	  SUM(horasfe) as horasfe, SUM(horasfen) as horasfen,
	  SUM(horas50) as horas50 ,SUM(horas50n) as horas50n ,
	  SUM(horas100) as horas100 ,SUM(horas_efetivas) as horas_efetivas ,
	  SUM(diarias_adc) as diarias_adc ,
	  SUM(iif(indcod='FI',1,0)) as faltas,
	  SUM(iif(indcod='A',1,0)) as doenca,
	  SUM(iif(indcod='FE',1,0)) as diasferias,
	  SUM(horas_dom) as horas_dom, SUM(horas_domn) as horas_domn
	  FROM @tabelaprocess
	    group by trab order by trab

	 
	
	 -- operaçao para calculos agregados do trabalhador
	  -- a folha por trabalhador
	      DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @SALARIO numeric(19,4), --vlrdiarias
		  @VLR_HXS numeric(19,4), -- vlrhorasfe
		  @VLR_HXA numeric(19,4), -- vlrhoras50
	      @VLR_HXN numeric(19,4),-- vlrhoras100
		  @FGTS numeric(19,4),
		  @SALFAM numeric(19,4),
		  @INSS numeric(19,4),
		  @FERIAS numeric(19,4),
		  @DECIMO numeric(19,4),
		  @FGTS_FE numeric(19,4),
		  @FGTS_DEC numeric(19,4),
	      @EDUC numeric(19,4),
		  @TERC numeric(19,4),
		  @EDUC_FE numeric(19,4),
		  @EDUC_DEC numeric(19,4),
		  @MULTA_FGTS numeric(19,4),
		  @HX numeric(19,4),
		  @HXS numeric(19,4), -- vlrhorasfe
		  @HXA numeric(19,4), -- vlrhoras50
	      @HXN numeric(19,4),-- vlrhoras100
	      @HEFETIVA numeric(19,4),
	      @PONTO nvarchar(62),
	      @IRFONTE numeric(19,4),
	      @GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  @FALTAS numeric(13,0),
		  @HORAS_DOM numeric(19,4),
		  @HORAS_FER numeric(19,4),
		  @VLR_DOM numeric(19,4),
		  @VLR_FER numeric(19,4);

	DECLARE @agtrab nvarchar(4),
	    @agdias numeric(18,4),
	    @agdiarias numeric(18,4), @agdiarias_n numeric(18,4),
	     @aghorasfe numeric(18,4),@aghorasfen numeric(18,4),
	     @aghoras50 numeric(18,4),@aghoras50n numeric(18,4),
	     @aghoras100 numeric(18,4),
     	 @aghoras_efetivas numeric(18,4),
	     @agdiarias_adc numeric(18,4),
	     @agfaltas int,@agdoenca int,@agdiasferias int,
		 @aghoras_dom numeric(18,4),@aghoras_fer numeric(18,4),
		 @aghoras_domn numeric(18,4),@aghoras_fern numeric(18,4);

	-- INICIO DO PROCESSAMENTO PARA A FOLHA POR TRABALHADOR	  
	 DECLARE cursor_mst CURSOR FOR   
      SELECT * from @tabelaagregada order by trab;
	 OPEN cursor_mst;
	
	 -- dados do listadados
	 DECLARE @tsal numeric(18,4),
	     @irdepend int,
		 @ndepend int,
		 @emgozo int,
		 @diastrab int,
		 @salcontrib numeric(18,4),--ferias
		 @feriasinss numeric(18,4),
		 @feriassalfam numeric(18,4),
		 @setor nchar(2),
		  @centro nchar(4);
		 --operadores 
	 DECLARE @trab nvarchar(4),
	   @dias numeric(18,4),
	   @vlrdiarias numeric(18,4), 
	   @vlrdiarias_adc numeric(18,4),
	   @vlrhorasfe numeric(18,4),
       @vlrhoras50 numeric(18,4),
	   @vlrdiarias_n numeric(18,4),
	   @vlrhorasfen numeric(18,4),
	   @vlrhoras50n numeric(18,4),
	   @vlrhoras100 numeric(18,4),
	   @vlrhoras_dom numeric(18,4),
	   @vlrhoras_fer numeric(18,4);

	  -- @faltas int,@doenca int,@diasferias int;
	  DECLARE @tsalfam numeric(18,4);
	  DECLARE @insssalcontrib numeric(18,4);


	   FETCH NEXT  FROM cursor_mst INTO
	    @agtrab,
	    @agdias,
	    @agdiarias, @agdiarias_n,
	    @aghorasfe,@aghorasfen,
	    @aghoras50 ,@aghoras50n,
	    @aghoras100,
     	@aghoras_efetivas,
	    @agdiarias_adc,
	    @agfaltas,@agdoenca,@agdiasferias,
		@aghoras_dom,@aghoras_domn;
       DECLARE @totalfolha numeric(19,4) = 0;   
		  
       WHILE @@FETCH_STATUS = 0  
       BEGIN  
		  SET @trab = @agtrab;
		  SET @dias = @agdias
	
	      SELECT @tsal = lst.tsal,@irdepend = lst.irdepend, @ndepend = lst.ndepend,@emgozo = lst.emgozo,
		   @diastrab = lst.diastrab, @salcontrib = lst.salcontrib,@feriassalfam = lst.salfam,@feriasinss = lst.inss,
		   @setor = lst.Setor,@centro = lst.Glecad
		   FROM @tabelaLista as lst where (lst.CODCAD=@trab);
	
		  SET @vlrdiarias = Round(@agdias * Round(@tsal/@ndiasmes, 4),2);
		  -- SET @vlrdiarias_adc =  round((@agdiarias_adc/8) * Round(@tsal/@ndiasmes, 4),2);
		  SET @vlrdiarias_adc =  (@agdiarias_adc/8) * (@tsal/@ndiasmes);
		  -- EC 141/2026: a hora extra e valorizada pelo divisor do DIA em que foi feita.
		  -- So existem duas faixas no mes em que o divisor muda (dez/2026 e dez/2027);
		  -- nos demais meses @ec_..._2 fica zero e a conta e a mesma de sempre.
		  DECLARE @ec_div_ini numeric(3,0), @ec_div_fim numeric(3,0);
		  DECLARE @ec_hfe_1 numeric(18,4), @ec_hfe_2 numeric(18,4), @ec_hfen_1 numeric(18,4), @ec_hfen_2 numeric(18,4);
		  DECLARE @ec_h50_1 numeric(18,4), @ec_h50_2 numeric(18,4), @ec_h50n_1 numeric(18,4), @ec_h50n_2 numeric(18,4);
		  SET @ec_div_ini = dbo.fn_DivisorHorasExtras(@inicio);
		  SET @ec_div_fim = dbo.fn_DivisorHorasExtras(@fim);
		  SET @ec_hfe_2 = 0; SET @ec_hfen_2 = 0; SET @ec_h50_2 = 0; SET @ec_h50n_2 = 0;
		  IF (@ec_div_ini <> @ec_div_fim)
		      SELECT @ec_hfe_2 = ISNULL(SUM(horasfe),0), @ec_hfen_2 = ISNULL(SUM(horasfen),0),
		             @ec_h50_2 = ISNULL(SUM(horas50),0), @ec_h50n_2 = ISNULL(SUM(horas50n),0)
		        FROM @tabelaprocess
		       WHERE trab = @trab AND dbo.fn_DivisorHorasExtras(dtadia) = @ec_div_fim;
		  SET @ec_hfe_1 = @aghorasfe - @ec_hfe_2;  SET @ec_hfen_1 = @aghorasfen - @ec_hfen_2;
		  SET @ec_h50_1 = @aghoras50 - @ec_h50_2;  SET @ec_h50n_1 = @aghoras50n - @ec_h50n_2;
		  SET @vlrhorasfe= iif(@aghorasfe = 0,0,iif(@ec_hfe_2 = 0, Round((@aghorasfe * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfe_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfe_2 * ((@tsal / @ec_div_fim) * 2.0)), 2))); 
		  SET @vlrhoras50 =iif(@aghoras50 = 0,0,iif(@ec_h50_2 = 0, Round((@aghoras50 * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrdiarias_n =iif(@agdiarias_n=0,0,Round((@agdiarias_n/8) * Round(@tsal/@ndiasmes, 4),2));
		  SET @vlrhorasfen = iif(@aghorasfen = 0,0,iif(@ec_hfen_2 = 0, Round((@aghorasfen * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfen_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfen_2 * ((@tsal / @ec_div_fim) * 2.0)), 2)));
		  SET @vlrhoras50n = iif(@aghoras50n = 0,0,iif(@ec_h50n_2 = 0, Round((@aghoras50n * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50n_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50n_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrhoras100 = Round((@vlrdiarias_n+ @vlrhorasfen+ @vlrhoras50n)*0.25,2);
        
		  -- separa horas feriados e domingos
		  --set @aghorasfe = @aghorasfe - @aghorasfen;  
		 -- set @aghoras_dom = @aghoras_dom - @aghoras_domn;

		  SET @aghoras_fer = iif(@aghorasfe = 0,0, @aghorasfe-@aghoras_dom-@aghoras_domn); 
		 -- SET @aghoras_fern = iif(@aghorasfen = 0,0, @aghorasfen-@aghoras_domn); 
		
		  SET @vlrhoras_dom = 0;
		  SET @vlrhoras_fer = 0;
		  DECLARE @vlrcalc numeric (18,4);
		  SET @vlrcalc = @vlrhorasfe;--+@vlrhorasfen;


		  if (@aghoras_dom <> 0) and (@aghoras_fer = 0) 
          begin
		       SET @vlrhoras_dom = @vlrcalc;
		  end;
		  if (@aghoras_dom = 0) and (@aghoras_fer <> 0) 
          begin
		       SET @vlrhoras_fer = @vlrcalc;
		  end;
          if (@aghoras_dom <> 0) and (@aghoras_fer <> 0) 
          begin
             SET @vlrhoras_dom = Round((@aghoras_dom / (@aghorasfe))
                               * @vlrcalc, 2);		
             SET @vlrhoras_fer = @vlrcalc - @vlrhoras_dom;
		  end;
         
	  -- preparacao e calculo para a folha com 
          SET @VLR_DOM = @vlrhoras_dom;
		  SET @VLR_FER = @vlrhoras_fer;
		 
		  UPDATE #DOM_FERIADO  SET HORAS_DOM = @aghoras_dom, HORAS_FER = @aghoras_fer
		   ,VLR_DOM = @VLR_DOM ,VLR_FER = @VLR_FER WHERE  	
	      ( COD = @trab)
		  -- @dias
	      FETCH NEXT  FROM cursor_mst INTO
		  @agtrab,
	      @agdias,
	      @agdiarias, @agdiarias_n,
	      @aghorasfe,@aghorasfen,
	      @aghoras50 ,@aghoras50n,
	      @aghoras100,
     	  @aghoras_efetivas,
	      @agdiarias_adc,
	      @agfaltas,@agdoenca,@agdiasferias,@aghoras_dom,@aghoras_fer;
	   END;
	   CLOSE cursor_mst;  
       DEALLOCATE cursor_mst;  

      -- SELECT * FROM #DOM_FERIADO


END;
  -- //CALCULO PARA  HORAS_DOM,HORAS_FER,VLR_DOM,VLR_FER, 
  -- quando o CLTFOLHA é ANTERIOR A 30 de novembro de 2018
  
IF (@FIM < '2018-11-01') 
BEGIN
    SELECT TRAB as COD, lst.NOMECAD as NOME,   flt.setor, st.DESCRI ,centro, 
           CATEGORIA,CBO,
           MENSALISTA,conta1,banco1,agencia1,cpf,end_rua,end_cid,end_uf,end_cep,SALBASE,
	       -- tipo, 
	     -- num_mod,codser, quant,
		  ROW_NUMBER() OVER(ORDER BY TRAB ASC) as ID,   
		  SALARIO,@fim as DATA,ADMI, IIF(DEMI is NULL,NULL, IIF(DEMI > @FIM,NULL,DEMI)) as demi, 
		  flt.INSS, 0.0 TXINSS, 0.0 as BASEIRF,
		  IRFONTE,flt.SALFAM,EDUC,TERC,
		  FGTS,DECIMO,FERIAS,
		  VLR_HXA,HX,VLR_HXN,HXA, HXN,
		  flt.VLR_HXS, 0.0 as SALLIQ, GRATIF,  
		  0.0 as ISIND,0.0 as ADIANT,PONTO,      
          lst.tsal as SALARIOATUAL, lst.soma_perc_conf as PERCCONF,  		
		  dom.HORAS_DOM, dom.HORAS_FER,dom.VLR_DOM,dom.VLR_FER,
		  -- estes itens não constamm
		  FGTS_FE,FGTS_DEC,  EDUC_FE,EDUC_DEC,
		  MULTA_FGTS,HXS, HEFETIVA,
	     FALTAS, DIAS	
		 
		 from NOVACLTFOLHA as flt  
		 LEFT OUTER JOIN @tabelaLista as lst 
                       on ( lst.CODCAD = TRAB) 
		  LEFT OUTER JOIN SETORES as st 
               on ( st.COD = flt.setor)
          LEFT OUTER JOIN #DOM_FERIADO as dom 
               on ( dom.COD = TRAB)
		  WHERE (DATA = @FIM)  AND (IIF(DEMI is NULL,NULL, IIF(DEMI > @FIM,NULL,DEMI)) is NULL);

END
ELSE
BEGIN
	  
  SELECT   TRAB as COD, lst.NOMECAD as NOME,   flt.setor, st.DESCRI ,centro, 
           CATEGORIA,CBO,
           MENSALISTA,conta1,banco1,agencia1,cpf,end_rua,end_cid,end_uf,end_cep,SALBASE,
	       -- tipo, 
	     -- num_mod,codser, quant,
		  ROW_NUMBER() OVER(ORDER BY TRAB ASC) as ID,   
		  SALARIO,@fim as DATA,ADMI, IIF(DEMI is NULL,NULL, IIF(DEMI > @FIM,NULL,DEMI)) as demi, 
		  flt.INSS, 0.0 TXINSS, 0.0 as BASEIRF,
		  IRFONTE,flt.SALFAM,EDUC,TERC,
		  FGTS,DECIMO,FERIAS,
		  VLR_HXA,HX,VLR_HXN,HXA, HXN,
		  flt.VLR_HXS, 0.0 as SALLIQ, GRATIF,  
		  0.0 as ISIND,0.0 as ADIANT,PONTO,      
          lst.tsal as SALARIOATUAL, lst.soma_perc_conf as PERCCONF,  		
		  HORAS_DOM, HORAS_FER,VLR_DOM,VLR_FER,
		  -- estes itens não constamm
		  FGTS_FE,FGTS_DEC,  EDUC_FE,EDUC_DEC,
		  MULTA_FGTS,HXS, HEFETIVA,
	     FALTAS, DIAS	
		 
		 from NOVACLTFOLHA as flt  
		 LEFT OUTER JOIN @tabelaLista as lst 
                       on ( lst.CODCAD = TRAB) 
		  LEFT OUTER JOIN SETORES as st 
               on ( st.COD = flt.setor)
          
		  WHERE (DATA = @FIM)   AND (IIF(DEMI is NULL,NULL, IIF(DEMI > @FIM,NULL,DEMI)) is NULL);
END;

END;
GO
PRINT 'EC141: FolhaClt_SoLeitura ok';
GO

-- ===========================================================================
-- FolhaClt_Atualiza
-- ===========================================================================
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
--=============================================
 --Author:		<Author,,Name>
 --Create date: <Create Date,,>
 --Description:	<Description,,>
-- --=============================================
--- VERSÂO 20 de MAIO DE 2019 ONDE SE CRIA ARQ TEMPORARIO E CHAMA O FOLHACLT_ATUALIZA_TAREFA 
--- VERSÂO 06 DE ABRIL DE 2021 -> ACRESCENTA A GRATIFICAÇÂO DE PARTICIPAÇÂO NO RESULTADO
--- VERSÂO  23 de NOVEMBRO DE 2021 -> ATUALIZA O CLTPONTO PARA CORRIGIR QUADRAS LOCADAS EM GLEBAS ERRADAS
----- VERSAO em 18 de janeiro de 2024  retirei a rotina custofolha e acresc. [dbo].[CLT_PontoValorNew] @inicio , @fim
ALTER PROCEDURE [dbo].[FolhaClt_Atualiza]  
	-- Add the parameters for the stored procedure here
	
	@inicio date,
	@fim date,
  @naograve int
AS
BEGIN
	
	SET LANGUAGE us_english;
  /*  DECLARE @inicio date,
 	@fim date,@naograve int;
    SET @inicio ='2021-03-01'
	SET @fim ='2021-03-31';
	SET @naograve = 3; 
	*/
/*
RETORNO POSSIVEIS 
2 -> foLHA ANTERIOR À INICIO DO SISTEMA ==> Não Faz Nada
3 -> Folha e CUSTOFOLHA já EXISTEM  -> Só REATUALIZA SE AS DUAS TABELAS SE o  @naoGrave vier COM valor 3,
     caso DE INSUCESSO retorna 3 
	  
4 -> Folha e CUSTOFOLHA já EXISTE ATUALIZADA -> Só REATUALIZA A TABELA CUSTOFOLHA  E SOMENTE SE O @naoGrave vier COM valor 4,
     caso DE SUCESSO  retorna  4
1 -> SUCeSSO GRAVOU NOVATFOLHA E CUSTOFOLHA (independente do valor de @naoGrave


*/

    UPDATE C1
    SET FAZMOV = q.GLEBA
    FROM CLTPONTO C1 
   INNER JOIN QUADRAS q ON (q.QUA = BL)
    WHERE DATA BETWEEN DATEADD(day,-6,@inicio) and @fim 
   and q.FIM is null and (q.GLEBA <> fazmov) and (q.GLEBA <> '') and bl <> ''


     
     IF OBJECT_ID('tempdb..##TABELAGERAL') IS NOT NULL 
     BEGIN 
         DROP TABLE ##TABELAGERAL 
     END
	  CREATE TABLE ##TABELAGERAL (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)) 
	
    

    DECLARE @RETORNO int;

	DECLARE @SOATUALIZA_CUSTOFOLHA INT = 0;


	SET @fim = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@fim)+1,0));

	SET @RETORNO = 1;

	IF (@FIM < '2018-11-01')
	BEGIN
	    IF (@naograve <> 4)
		BEGIN
		   SELECT 2 as RETORNO; -- folha anterior ao inicio das atulizações CLTSERVER
		   RETURN;
        END;
	    SET @SOATUALIZA_CUSTOFOLHA = 1;
	    SET @RETORNO = 4;
	END
	ELSE
	BEGIN
    --SELECT @FOLHAJAEXISTE =  
		IF EXISTS(SELECT ID FROM NOVACLTFOLHA WHERE (DATA = @Fim)) 
		BEGIN
			IF ((@naograve <> 3) AND (@naograve <> 4))
			BEGIN
			   SELECT 3 as RETORNO; -- folha anterior ao inicio das atulizações CLTSERVER
			   RETURN;
			END; 
			IF (@naograve = 4) 
			BEGIN
			  SET @SOATUALIZA_CUSTOFOLHA = 1;
			  SET @RETORNO = 4;
			END; 
		END;
	END;

	DECLARE @comparafolha bit = 0;
	DECLARE @CUSTO bit = 1;


	SET NOCOUNT ON;
	-- pega a semana do ponto anterior ao inicio do mes
	DECLARE @primeirasemana date;
	SET @primeirasemana = @inicio;
	WHILE (DATENAME(weekday,@primeirasemana) <> 'Thursday') 
	begin
      	 SET @primeirasemana= DATEADD(dd,-1, @primeirasemana);
	end;

	-- RETORNOMES === 0 - evento fora do mes, 1 - retorno ao trabalho no mes, 2 - saida para o INSS no mes

	DECLARE @AFASTADOSINSS TABLE (COD nchar(4), TIPO INT, TPSEFIP NCHAR(2), DATAEVENTO Date, retornonoMes int);
	INSERT INTO @AFASTADOSINSS
	SELECT * FROM [dbo].[AFASTADOS_INSS](@fim);

--	TABELAS AUXILIARES
	-- Salario minimo e fgts
	DECLARE @SalMinFgts TABLE ([DATA] date,VALOR numeric(18,4),FGTS numeric(18,4));
	insert @SalMinFgts 
	EXECUTE [dbo].[SalMinFgts]
	        @fim; 
	DECLARE @SalMin numeric(18,4);		
	DECLARE @taxaFGTS numeric(18,4);
	SELECT @SalMin=VALOR,@taxaFGTS=FGTS from @SalMinFgts;

	-- tabeal do INNS  com tipo definido
	DECLARE @TabInss TipoTabelaINSS;
	insert into @TabInss
	SELECT [data], faixa, valor1, valor2, taxa FROM TABINSS  
         where  (Format(TABINSS.DATA,'yyyyMM') <= Format(@fim,'yyyyMM'))
       ORDER BY [DATA] DESC,faixa; 

	DECLARE @MAX_DATA DATE, @MAX_FAIXA numeric(6,0)
	SELECt @MAX_DATA=MAX(DATA)
	 FrOM @TabInss 
	SELECt 
	 @MAX_FAIXA=MAX(FAIXA) FrOM TABINSS WHERE DATA = @MAX_DATA;
	DECLARE @TETO_MAX NUMERIC(19,4), @TETO_INSS NUMERIC(19,4);
	SELECt @TETO_MAX =  VALOR2  FrOM TABINSS where data = @MAX_DATA and FAIXA = @MAX_FAIXA
	
	SET @TETO_INSS = dbo.ValorInss(@TabInss,@fim,@TETO_MAX);


	
    -- tabelas IRFONTE com tipo definido
	DECLARE @TabIrFonte TipoTabelaIRFonte;
	insert into @TabIrFonte
	SELECT TABIR.data, faixa, base1, base2, aliq, TABIR.deduzir,
    (SELECT MAX(irdedu.DEDUCAO) from irdedu where (TABIR.Data = IRDEDU.DATA) ) deduGeral
    FROM TABIR
	where  Format(TABIR.[data], 'yyyyMM') <= Format(@fim,'yyyyMM')
      ORDER BY TABIR.DATA DESC, faixa;

   -- tabela SALFAM
   DECLARE @TabSalFam TipoTabelaSalFam;
   insert into @TabSalFam
      SELECT data, faixa, valor1, valor2, salfam FROM SALFAM 
	  where  Format([data], 'yyyyMM') <= Format(@fim,'yyyyMM')
	  ORDER BY DATA DESC,faixa;
	

    --tabela gratificação
	DECLARE @tabelagratif TABLE (num_mod nvarchar(2),gcodser nvarchar(3),gsetor nvarchar(2),perc_diaria numeric(18,4),
	          vlr_real numeric(18,4),dta_ini date,dta_fim date  );
	insert into @tabelagratif                        
    SELECT NUM_MOD,CODSER,iif(SETOR='','  ',SETOR),[PERCENT],VLRDIARIA,DTA_INI,DTA_FIM FROM GRATIF WHERE 
                (DTA_INI <= @inicio) and ((DTA_FIM >= @fim) 
              or (DTA_FIM is null) ) and 
			  (CODSER is not null) and ([PERCENT] is not null) and (VLRDIARIA is not null);
	
	-- tabela cltcodig
	DECLARE @tabcltcodigo TABLE (INDCOD nchar(2),DESCOD nvarchar(21),FATOR numeric(19,4),
	 HORAS numeric(19,4),X_HORAS numeric(19,4),NOTURNO numeric(19,4),
	 H_NORMAL numeric(19,4),COMPATI nchar(1));
	 insert into @tabcltcodigo SELECT  INDCOD ,DESCCOD ,FATOR ,HORAS ,X_HORAS ,NOTURNO ,
	 H_NORMAL ,COMPATI from cltcodig;

    -- montagem da "tabela" vet_sabado
	DECLARE @vet_sabado TABLE(oindcod nvarchar(2));
    DECLARE @i int;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( ' '+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 10)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( '.'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( 'C'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
-- numero de dias no mes	
   DECLARE @ndiasmes int = DateDiff(day,@inicio,@fim)+1;

-- Este é o listaDados (numa store procedure) retorna as informações do CLTCAD e FERIAS 
  
   DECLARE @tabelaLista TABLE (CODCAD nvarchar(4),NOMECAD nvarchar(60),DEMI date,
     CATEGORIA nvarchar(30),CBO numeric(19,4),MENSALISTA nchar(1),
	 CONTA1 nchar(10),BANCO1 nchar(3),AGENCIA1 nchar(8),CPF nchar(11),END_RUA nvarchar(50) ,
	   END_CID nvarchar(25) ,END_UF nchar(2),END_CEP nchar(10),
       SETOR nchar(2), salbase numeric(18,4),ult_atual date,
       Glecad NVARCHAR(4),admi date,demitido int,admitido int,diastrab int,
         tsal numeric(18,4), soma_perc_conf numeric(18,4),      ndepend int,emgozo int,irdepend int, salcontrib numeric(18,4),prov_ferias numeric (18,4),
           prov_decimo numeric(18,4), inss numeric(18,4), salfam numeric(18,4), FERIAS_BASE numeric(18,4)
		   , FERIAS_MEDIA numeric(18,4)
		   , SALMEDIO_FERIAS numeric(18,4));
       insert into @tabelaLista
       EXECUTE  [dbo].[RetornaListaDados] 
          @inicio
          ,@fim
          ,@SalMin;
         
 --XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

-- TRANSFORMA o CLTPONTO ( 1-registro com dias da semana => nova tabela com 1 registro = 1dia )
 -- tabela geral que transforma o cltponto numa tabela individualizada de evento     
    DECLARE @ndia int;
	DECLARE @dia NVARCHAR(4);
	/*DECLARE @tabelageral TABLE (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)) */
	
	
	
	set @ndia = 0;
	while (@ndia < 7) 
	begin
	    set @dia ='DIA'+CONVERT(nvarchar,(@ndia+1),1);
	    DECLARE @literal as varchar(5120);
	    SET @literal =
	    'select TRAB,SETOR,fazmov,BL,NUM_MOD,CODSER,cltponto.noturno as noturno,Data as Semana,quant,ok,'+
        'DATENAME(weekday, DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA))   as diasemana,'+
       'DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) as dtadia, '+@dia+  ', '+--cdg.indcod,'+
       'cdg.fator as Diarias,cdg.HORAS as Horas_efetivas,cdg.X_HORAS as Horas50,cdg.NOTURNO as Horas100,cdg.H_NORMAL as Horasfe,cdg.COMPATI'+--,cdg.DESCCOD'+
        ' from cltponto LEFT OUTER JOIN CLTCODIG as cdg '+
         'on ( cdg.INDCOD = '+@dia+')  WHERE (CLTPONTO.NOTURNO <> ''P'') AND '+
        '(data between '''+CAST(@primeirasemana as NVARCHAR(12))+''' and '''+CAST(@fim as NVARCHAR(12))+''') and ('+@dia+' <> ''  '') and '+ 
       ' (DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) >='''+ CAST(@inicio as NVARCHAR(12))+''') and '+
        '(DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) <= '''+CAST(@fim as NVARCHAR(12))+''')'; 
        
		insert into ##TABELAGERAL EXEC(@literal);
        set @ndia = @ndia+1;
    end;
	
   --processamneto corresponde ao Gera_Valores da versão Delphi (sem o processamento do ponto)
   DECLARE @tabelaprocess TABLE
   ( numtarefa int,
   trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  Diarias numeric(18,4),
	  Diariasn numeric(18,4),
	  Horasfe numeric(18,4),
	  Horasfen numeric(18,4),
	  Horas50 numeric(18,4),
	  Horas50n numeric(18,4),
	  Horas100 numeric(18,4),
	  horas_efetivas numeric(18,4),
	  Diarias_adc numeric(18,4),
	 
	  compati nvarchar(1),
	  quant  numeric(18,4), 
	  ok nvarchar(1),
	  -- acrescentei
	  Horas_Dom numeric(18,4),  -- horas feriado que cairam num domingo
	  Horas_Domn numeric(18,4) --   
	  );
	  
   
	insert into  @tabelaprocess
	select 0, trab ,tbger.setor ,gleba ,bl ,num_mod ,codser ,
	  noturno , 
	  semana , diasemana , dtadia,indcod ,
	  iif(diarias>0,iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiarias,
	 
	 iif(diarias>0 and (noturno='X'),iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiariasn,
	 
	  iif(horasfe>0,horasfe,0) as pHorasfe,
	  
	  iif((horasfe>0) and (noturno='X'),horasfe,0) as pHorasfen,
	  
	  iif(horas50>0,horas50,0) as pHoras50,
	  iif((horas50>0) and (noturno='X'),horas50,0) as pHoras50n,

	  iif(horas100>0,horas100,0) as pHoras100,
	  
	  iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0,iif( ( indcod = 'X'),horas_efetivas/2, horas_efetivas),horas_efetivas),0) * iif((diarias>0) and (diasemana not in ('Saturday','Sunday')), dbo.fn_FatorHorasEfetivas(dtadia), 1) as horas_efetivas,
	  -- diaria_adc
	  iif(not Exists(Select gcodser from @tabelagratif as grat where ((codser = grat.gcodser) and 
	  ((tbger.setor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))),0,
	     iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and (rtrim(indcod)='X'), 
	       ( (horas_efetivas/2) *  (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser))),
		(horas_efetivas * (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser)) )),0)) as diaria_adc

	  ,compati,quant,ok ,

	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0),horasfe,0) as pHoras_Dom,
	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0) and (noturno='X'),horasfe,0) as pHoras_Domn

	  from ##TABELAGERAL 
	   as tbger left join @tabelaLista as lst 
	  ON (lst.CODCAD = tbger.trab) 
	     WHERE (admi <= dtadia) and ((demi is null) OR (dtadia <= DEMI)) and (noturno <> 'P')  ;
		 
/*
	SELECT * FROM  @tabelageral 
	   as tbger left join @tabelaLista as lst 
	  ON (lst.CODCAD = tbger.trab) 
	     WHERE (admi <= dtadia) and ((demi is null) OR (dtadia <= DEMI)) and tbger.setor = ' 6' and trab = '8942'
		 order by dtadia ;

return;
*/


  DECLARE @tabelaPREMIO TABLE
   ( 
      DATA Date,
	  setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3), trab nvarchar(4),  num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  
	  Valor numeric(18,4),
	  DTA_INI Date, DTA_FIM Date,
	  INCLUIDO int
	  );
	  
   INSERT INTO @tabelaPREMIO
   select  DATA, SETOR, FAZMOV, BL, TRAB, NUM_MOD, CODSER, NOTURNO, VALOR, DTA_INI, DTA_FIM, 0 as incluido  from cltponto  
       WHERE (CLTPONTO.NOTURNO = 'P') AND 
        (data between @inicio and @fim)
-----*** INcluido em nov/2024
 DECLARE @GratEspecial TABLE
   ( 
      DATA Date,
	  setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3), trab nvarchar(4),  num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  
	  GRATESPECIAL numeric(18,4),
	  
	  INCLUIDO int
	  );
	  
   INSERT INTO @GratEspecial
   select  DATA, SETOR, FAZMOV, BL, TRAB, NUM_MOD, CODSER, NOTURNO, DIARIA as GratEspecial, 0 as incluido  from cltponto  
       WHERE (Diaria <> 0) AND YEAR(@fim) > '2023' and 
        (data between @inicio and @fim)
		and (Month(data) = MONTH(@fim))
-----*** INcluido em nov/2024	
	


  -- Base do Calculo da Folha por TRABALHADOR (agregada) 
      /*DECLARE @tabelaagregada TABLE (trab nvarchar(4),
	    dias numeric(18,4),
	    diarias numeric(18,4), diarias_n numeric(18,4),
	     horasfe numeric(18,4),horasfen numeric(18,4),
	     horas50 numeric(18,4),horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	     horas_domn numeric(18,4));
		--  horas_BancoHoras_E int,
		 -- horas_BancoHoras_C int );*/  
      DECLARE @tabelaagregada TABLE (
	    trab nvarchar(4),
		--setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	    dias numeric(18,4),
	    diarias numeric(18,4)
		, diarias_n numeric(18,4),
	     horasfe numeric(18,4)
		 ,horasfen numeric(18,4),
	     horas50 numeric(18,4)
		 ,horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	     horas_domn numeric(18,4)
		 --,diasPrevidencia int
		 );


	  
	  insert into @tabelaagregada
	  select trab,
	--  setor ,gleba,bl ,num_mod ,codser ,
	  Round(SUM(Diarias)/8,2),
	  SUM(Diarias) as diarias ,
	  SUM(diariasn) as diarias_n,
	  SUM(horasfe) as horasfe, SUM(horasfen) as horasfen,
	  SUM(horas50) as horas50 ,SUM(horas50n) as horas50n ,
	  SUM(horas100) as horas100 ,SUM(horas_efetivas) as horas_efetivas ,
	  SUM(diarias_adc) as diarias_adc ,
	  SUM(iif(indcod='FI',1,0)) as faltas,
	  SUM(iif(indcod='A',1,0)) as doenca,
	  SUM(iif(indcod='FE',1,0)) as diasferias,
	  SUM(horas_dom) as horas_dom,
	  SUM(horas_domn) as horas_domn 
	 -- ,SUM(iif(indcod='AD',1,0)) as diasPrevidencia
	  /*SUM(iif(substring(indcod,1,1) <> 'E',0, 
	    CONVERT(int, substring(indcod,2,1))
	   )  ) as horas_BancosHoras_E,
	   SUM(iif(substring(indcod,1,1) <> 'C',0, 
	    convert( int, substring(indcod,2,1) )
	   )  ) as horas_BancosHoras_C*/

	  FROM @tabelaprocess
	    group by trab order by trab

--	 select trab, horas_BancoHoras_C, horas_BancoHoras_E from @tabelaagregada;
--	return;
	 -- operaçao para calculos agregados do trabalhador
	  -- a folha por trabalhador
	  DECLARE @FOLHA TABLE (   
	      DATA date,
		  SETOR nchar(2),
	      CENTRO nchar(4),
		  QUADRA nchar(3),
		   trab nchar(4),
	      TIPO nchar(1),
		  NUM_MOD nchar(2),
		  CODSER nchar(3),
		  QUANT numeric(19,4), 
		  SALARIO numeric(19,4), --vlrdiarias
		  VLR_HXS numeric(19,4), -- vlrhorasfe
		  VLR_HXA numeric(19,4), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
		  FGTS numeric(19,4),
		   SALFAM numeric(19,4),
		   INSS numeric(19,4),
		   FERIAS numeric(19,4),
		   DECIMO numeric(19,4),
		   FGTS_FE numeric(19,4),
		   FGTS_DEC numeric(19,4),
	      EDUC numeric(19,4),
		  TERC numeric(19,4),
		  EDUC_FE numeric(19,4),
		  EDUC_DEC numeric(19,4),
		  MULTA_FGTS numeric(19,4),
		  HX numeric(19,4),
		  HXS numeric(19,4), -- vlrhorasfe
		  HXA numeric(19,4), -- vlrhoras50
	      HXN numeric(19,4),-- vlrhoras100
	      HEFETIVA numeric(19,4),
	      PONTO nvarchar(62),
	      IRFONTE numeric(19,4),
	      GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  FALTAS numeric(13,0),
		  HORAS_DOM numeric(19,4),
		  HORAS_FER numeric(19,4),
		  VLR_DOM numeric(19,4),
		  VLR_FER numeric(19,4),
		  TXINSS numeric(19,4),
		  IRDEDU numeric(19,4),
		  DIAS numeric(19,4)
	  );
	
	      DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @SALARIO numeric(19,4), --vlrdiarias
		  @VLR_HXS numeric(19,4), -- vlrhorasfe
		  @VLR_HXA numeric(19,4), -- vlrhoras50
	      @VLR_HXN numeric(19,4),-- vlrhoras100
		  @FGTS numeric(19,4),
		  @SALFAM numeric(19,4),
		  @INSS numeric(19,4),
		  @FERIAS numeric(19,4),
		  @DECIMO numeric(19,4),
		  @FGTS_FE numeric(19,4),
		  @FGTS_DEC numeric(19,4),
	      @EDUC numeric(19,4),
		  @TERC numeric(19,4),
		  @EDUC_FE numeric(19,4),
		  @EDUC_DEC numeric(19,4),
		  @MULTA_FGTS numeric(19,4),
		  @HX numeric(19,4),
		  @HXS numeric(19,4), -- vlrhorasfe
		  @HXA numeric(19,4), -- vlrhoras50
	      @HXN numeric(19,4),-- vlrhoras100
	      @HEFETIVA numeric(19,4),
	      @PONTO nvarchar(62),
	      @IRFONTE numeric(19,4),
	      @GRATIF numeric(19,4),
	 	  @FALTAS numeric(13,0),
		  @HORAS_DOM numeric(19,4),
		  @HORAS_FER numeric(19,4),
		  @VLR_DOM numeric(19,4),
		  @VLR_FER numeric(19,4);

	DECLARE @agtrab nvarchar(4),
	    @agdias numeric(18,4),
	    @agdiarias numeric(18,4), @agdiarias_n numeric(18,4),
	     @aghorasfe numeric(18,4),@aghorasfen numeric(18,4),
	     @aghoras50 numeric(18,4),@aghoras50n numeric(18,4),
	     @aghoras100 numeric(18,4),
     	 @aghoras_efetivas numeric(18,4),
	     @agdiarias_adc numeric(18,4),
	     @agfaltas int,@agdoenca int,@agdiasferias int,
		 @aghoras_dom numeric(18,4),@aghoras_fer numeric(18,4),
		 @aghoras_domn numeric(18,4),@aghoras_fern numeric(18,4);

	-- INICIO DO PROCESSAMENTO PARA A FOLHA POR TRABALHADOR	  
	 DECLARE cursor_mst CURSOR FOR   
      SELECT * from @tabelaagregada order by trab;
	 OPEN cursor_mst;
	
	 -- dados do listadados
	 DECLARE @tsal numeric(18,4),
	     @irdepend int,
		 @ndepend int,
		 @emgozo int,
		 @diastrab int,
		 @salcontrib numeric(18,4),--ferias
		 @feriasinss numeric(18,4),
		 @feriassalfam numeric(18,4),
		 @setor nchar(2),
		  @centro nchar(4);
		 --operadores 
	 DECLARE @trab nvarchar(4),
	   @dias numeric(18,4),
	   @vlrdiarias numeric(18,4), 
	   @vlrdiarias_adc numeric(18,4),
	   @vlrhorasfe numeric(18,4),
       @vlrhoras50 numeric(18,4),
	   @vlrdiarias_n numeric(18,4),
	   @vlrhorasfen numeric(18,4),
	   @vlrhoras50n numeric(18,4),
	   @vlrhoras100 numeric(18,4),
	   @vlrhoras_dom numeric(18,4),
	   @vlrhoras_fer numeric(18,4),
	   @FERIAS_BASE numeric(18,4),
	   @FERIAS_MEDIA numeric(18,4),
	   @SALMEDIO_FERIAS numeric(18,4),

	   @vlrdiarias_adc_PREMIO numeric(18,4);
	   

	   DECLARE @DATAADM DATE;
		DECLARE @NDIASADIMITIDO NUMERIC(18,4), @DIASNOMES NUMERIC(18,4);

      DECLARE @TRAB_EM_FERIAS_PROV_FERIAS numeric(18,4);
	  DECLARE @TRAB_EM_FERIAS_PROV_DECIMO numeric(18,4);

	  -- @faltas int,@doenca int,@diasferias int;
	  DECLARE @tsalfam numeric(18,4);
	  DECLARE @insssalcontrib numeric(18,4);


	   FETCH NEXT  FROM cursor_mst INTO
	    @agtrab,
	    @agdias,
	    @agdiarias, @agdiarias_n,
	    @aghorasfe,@aghorasfen,
	    @aghoras50 ,@aghoras50n,
	    @aghoras100,
     	@aghoras_efetivas,
	    @agdiarias_adc,
	    @agfaltas,@agdoenca,@agdiasferias,
		@aghoras_dom,@aghoras_domn;
       DECLARE @totalfolha numeric(19,4) = 0;   
		 
	   DECLARE @demitido int, @admitido int;	 
		  
       WHILE @@FETCH_STATUS = 0  
       BEGIN  
		  SET @trab = @agtrab;
		  SET @dias = @agdias
	
	       /*IF (@trab = '8981')
		   BEGIN
		     SET @trab = '8981';
		   END;
		   */
	      SELECT @tsal = lst.tsal,@irdepend = lst.irdepend, @ndepend = lst.ndepend,@emgozo = lst.emgozo,
		   @diastrab = lst.diastrab, @salcontrib = lst.salcontrib,@feriassalfam = lst.salfam,@feriasinss = lst.inss,
		   @setor = lst.Setor,@centro = lst.Glecad,
		   @TRAB_EM_FERIAS_PROV_FERIAS = lst.prov_ferias, @TRAB_EM_FERIAS_PROV_DECIMO = lst.prov_decimo
		   , @demitido = lst.demitido , @admitido = lst.admitido
		   , @FERIAS_BASE = FERIAS_BASE, @FERIAS_MEDIA = FERIAS_MEDIA, @SALMEDIO_FERIAS = SALMEDIO_FERIAS
		   FROM @tabelaLista as lst where (lst.CODCAD=@trab);
	
	      /* IF   (@TRAB_EM_FERIAS_PROV_FERIAS < 0)    --(@trab = '9918') OR (@trab = '9919') OR (@trab = '8944')
		   BEGIN
		      SET @trab = @agtrab;
		   END;
		   */
		  SET @vlrdiarias = Round(@agdias * Round(@tsal/@ndiasmes, 4),2);
		  -- SET @vlrdiarias_adc =  round((@agdiarias_adc/8) * Round(@tsal/@ndiasmes, 4),2);
		  SET @vlrdiarias_adc =  (@agdiarias_adc/8) * (@tsal/@ndiasmes);  --ptRound(((Diarias_ADC / 8) * VlrDiaria), 2)
		  -- EC 141/2026: a hora extra e valorizada pelo divisor do DIA em que foi feita.
		  -- So existem duas faixas no mes em que o divisor muda (dez/2026 e dez/2027);
		  -- nos demais meses @ec_..._2 fica zero e a conta e a mesma de sempre.
		  DECLARE @ec_div_ini numeric(3,0), @ec_div_fim numeric(3,0);
		  DECLARE @ec_hfe_1 numeric(18,4), @ec_hfe_2 numeric(18,4), @ec_hfen_1 numeric(18,4), @ec_hfen_2 numeric(18,4);
		  DECLARE @ec_h50_1 numeric(18,4), @ec_h50_2 numeric(18,4), @ec_h50n_1 numeric(18,4), @ec_h50n_2 numeric(18,4);
		  SET @ec_div_ini = dbo.fn_DivisorHorasExtras(@inicio);
		  SET @ec_div_fim = dbo.fn_DivisorHorasExtras(@fim);
		  SET @ec_hfe_2 = 0; SET @ec_hfen_2 = 0; SET @ec_h50_2 = 0; SET @ec_h50n_2 = 0;
		  IF (@ec_div_ini <> @ec_div_fim)
		      SELECT @ec_hfe_2 = ISNULL(SUM(horasfe),0), @ec_hfen_2 = ISNULL(SUM(horasfen),0),
		             @ec_h50_2 = ISNULL(SUM(horas50),0), @ec_h50n_2 = ISNULL(SUM(horas50n),0)
		        FROM @tabelaprocess
		       WHERE trab = @trab AND dbo.fn_DivisorHorasExtras(dtadia) = @ec_div_fim;
		  SET @ec_hfe_1 = @aghorasfe - @ec_hfe_2;  SET @ec_hfen_1 = @aghorasfen - @ec_hfen_2;
		  SET @ec_h50_1 = @aghoras50 - @ec_h50_2;  SET @ec_h50n_1 = @aghoras50n - @ec_h50n_2;
		  SET @vlrhorasfe= iif(@aghorasfe = 0,0,iif(@ec_hfe_2 = 0, Round((@aghorasfe * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfe_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfe_2 * ((@tsal / @ec_div_fim) * 2.0)), 2))); 
		  SET @vlrhoras50 =iif(@aghoras50 = 0,0,iif(@ec_h50_2 = 0, Round((@aghoras50 * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrdiarias_n =iif(@agdiarias_n=0,0,Round((@agdiarias_n/8) * Round(@tsal/@ndiasmes, 4),2));
		  SET @vlrhorasfen = iif(@aghorasfen = 0,0,iif(@ec_hfen_2 = 0, Round((@aghorasfen * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfen_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfen_2 * ((@tsal / @ec_div_fim) * 2.0)), 2)));
		  SET @vlrhoras50n = iif(@aghoras50n = 0,0,iif(@ec_h50n_2 = 0, Round((@aghoras50n * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50n_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50n_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrhoras100 = Round((@vlrdiarias_n+ @vlrhorasfen+ @vlrhoras50n)*0.25,2);
          SET @vlrdiarias_adc_PREMIO = 0; 
		  SELECT @vlrdiarias_adc_PREMIO = ISNULL(SUM(VALOR),0) FROM  @tabelaPREMIO WHERE TRAB = @trab and incluido = 0;
		  UPDATE @tabelaPREMIO SET INCLUIDO = 1 WHERE TRAB = @trab;
		  -- INCLUIDO EM 15-11-2024
		  SELECT @vlrdiarias_adc = @vlrdiarias_adc + ISNULL(SUM(GRATESPECIAL),0) FROM  @GratEspecial WHERE TRAB = @trab and incluido = 0;
		  UPDATE @tabelaPREMIO SET INCLUIDO = 1 WHERE TRAB = @trab;
		 -- FIM INCLUIDO EM 15-11-2024

		  -- separa horas feriados e domingos
		  --set @aghorasfe = @aghorasfe - @aghorasfen;  
		 -- set @aghoras_dom = @aghoras_dom - @aghoras_domn;

		  SET @aghoras_fer = iif(@aghorasfe = 0,0, @aghorasfe-@aghoras_dom-@aghoras_domn); 
		 -- SET @aghoras_fern = iif(@aghorasfen = 0,0, @aghorasfen-@aghoras_domn); 
		
		  SET @vlrhoras_dom = 0;
		  SET @vlrhoras_fer = 0;
		  DECLARE @vlrcalc numeric (18,4);
		  SET @vlrcalc = @vlrhorasfe;--+@vlrhorasfen;


		  if (@aghoras_dom <> 0) and (@aghoras_fer = 0) 
          begin
		       SET @vlrhoras_dom = @vlrcalc;
		  end;
		  if (@aghoras_dom = 0) and (@aghoras_fer <> 0) 
          begin
		       SET @vlrhoras_fer = @vlrcalc;
		  end;
          if (@aghoras_dom <> 0) and (@aghoras_fer <> 0) 
          begin
             SET @vlrhoras_dom = Round((@aghoras_dom / (@aghorasfe))
                               * @vlrcalc, 2);		
             SET @vlrhoras_fer = @vlrcalc - @vlrhoras_dom;
		  end;
         
		
	  -- preparacao e calculo para a folha com 
          SET @SALARIO = @vlrdiarias;
		  SET @VLR_HXS = @vlrhorasfe;
		  SET @VLR_HXA = @vlrhoras50;
	      SET @VLR_HXN = @vlrhoras100;
		  -- REVISAO
		  SET @FGTS = dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ 
		           @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO )), 2);
          SET @VLR_DOM = @vlrhoras_dom;
		  SET @VLR_FER = @vlrhoras_fer;
		  /* if (@TRAB = '8981')
		   BEGIN
		     SET @TRAB = '8981';
		   END;
		   */
		  SET @SALFAM = 0;
		  SET @INSS = dbo.ValorInss(@TabInss,@fim,@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100
		       +@vlrdiarias_adc + @vlrdiarias_adc_PREMIO);
		  
		  -- ALTERADO POSTERIORMENTE NA REVISÃO DE 2019/MARÇO
		  SET @FERIAS =     Round (( (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO)/12) * 1.333,2) ;
		  --dbo.ptRound 
		 
		  SET @DECIMO = Round (( (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO)/12) ,2) ;
		 
		  IF (@emgozo=1) 
		  BEGIN
		    
			 SET @FERIAS = @FERIAS + IIF(@TRAB_EM_FERIAS_PROV_FERIAS is null,0,Round (( (@TRAB_EM_FERIAS_PROV_FERIAS)/12) * 1.3333,2));
			 SET @DECIMO =  @DECIMO + IIF(@TRAB_EM_FERIAS_PROV_DECIMO is null,0,Round (( (@TRAB_EM_FERIAS_PROV_DECIMO)/12) ,2));
		 
          END

		 
		  ----------- REVISAO 2019/MARÇO
		  SET @FGTS_FE =   dbo.ptRound(((@taxaFGTS / 100) * (@FERIAS)), 2);--dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@FERIAS)), 2);
		  SET @FGTS_DEC =  dbo.ptRound(((@taxaFGTS / 100) * (@DECIMO)), 2);--dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@DECIMO)), 2);
	      SET @EDUC =0;
		  SET @TERC =0;
		  SET @EDUC_FE =Round(@FERIAS * 0.027,2);
		  SET @EDUC_DEC = Round(@DECIMO * 0.027,2);
		 
		  SET @MULTA_FGTS = ROUND((@FGTS+@FGTS_FE+@FGTS_DEC) * 0.50,2)
		  
		  
	
		  SET @HX =@agdiarias;
		  SET @HXS = @aghorasfe;
		  SET @HXA = @aghoras50;
	      SET @HXN = @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; -- as horas noturnas incluem as feriados e extras
	      SET @HEFETIVA = @aghoras_efetivas;
	      SET @PONTO = '';
	      SET @IRFONTE =  dbo.CalcIrFonte(@TabIrFonte,@agtrab,@fim,(@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ 
		      @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO),
		    @INSS ,@irdepend);
          if (@fim >= '2019-05-01') 
		  BEGIN
		     IF @IRFONTE < 10 
			    SET @IRFONTE = 0;
		  END;
	      SET @GRATIF = @vlrdiarias_adc + @vlrdiarias_adc_PREMIO;
	 	  SET @FALTAS=@agfaltas;      
		  SET  @tsalfam = 0;

		  DECLARE @SALARIO_CONTRATUAL NUMERIC(19,4) = 0;
		  if (@fim >= '2019-09-01')
		  BEGIN
		     SET @SALARIO_CONTRATUAL = @tsal;
			 IF EXISTS(SELECT COD FROM @AFASTADOSINSS WHERE COD = @trab) 
			 BEGIN
			    SET @SALARIO_CONTRATUAL = 0;
			    IF EXISTS(SELECT COD FROM @AFASTADOSINSS WHERE COD = @trab AND retornonoMes = 2 AND (DATAevento > @INICIO) )
				BEGIN
				   SET @SALARIO_CONTRATUAL = @tsal;
				END; 
			 END; 
		  END; 
		  
		  if ((@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc +@vlrdiarias_adc_PREMIO ) <> 0) OR (@SALARIO_CONTRATUAL <> 0) 
		  begin
			  if (@emgozo = 0) 
			  begin
				if (@ndepend > 0) 
				begin
                  if (@fim > '2019-09-01') 
                  BEGIN					
			         SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
						   (@SALARIO_CONTRATUAL+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + +@vlrdiarias_adc_PREMIO ), @ndepend);
                  end
				  else
				  begin
				     SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
						   (@VLRDIARIAS + @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO), @ndepend);
				  end;

			      if (@fim >= '2019-05-01') 
				  BEGIN
				     if ((@admitido = 1) OR (@demitido = 1))
				     BEGIN
				        SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
					   	  @SALARIO_CONTRATUAL  + @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO, @ndepend);
				        if (@fim > '2019-09-01') 
                        BEGIN					
							IF (@admitido = 1)
							BEGIN
							  SELECT @DATAADM = ADMI FROM CLTCAD WHERE CODCAD = @trab;
							  SET @NDIASADIMITIDO = DATEDIFF(d,@DATAADM,@fim)+1;
							  SELECT @DIASNOMES = day(eomonth(@fim));
							  SET @tsalfam = ROUND((@NDIASADIMITIDO/@DIASNOMES) * @tsalfam,2); 
							END;
							IF (@demitido = 1)
							BEGIN
							  -- NO CASO E DEMITIDO (uso as mesmas variaveis para economizar)
							  SELECT @DATAADM = DEMI FROM CLTCAD WHERE CODCAD = @trab;
							  SET @NDIASADIMITIDO = DATEDIFF(d,@inicio,@DATAADM)+1;
							  SELECT @DIASNOMES = day(eomonth(@fim));
							  SET @tsalfam = ROUND(((@NDIASADIMITIDO/@DIASNOMES) * @tsalfam),2); 
							END;
						END
                     END; 
				  END;
				  
				  if (@diastrab <> 0) AND (@fim < '2019-09-01')
				  begin   
					 SET @tsalfam  = Round(  (CAST (@diastrab as decimal) / CAST( @ndiasmes as decimal)) * @tsalfam, 2); 
				  end;
				end;
			  end
			  else
			  if (@fim >= '2009-04-01') 
			  begin
            
				-- verifica se a faixa de contribuição p/ desc.INSS mudou!
				SET  @insssalcontrib = dbo.ValorInss(@tabinss, @fim,  @salcontrib + 
							  @vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO);
				if (@insssalcontrib >= (@feriasinss + @INSS)) 
				begin -- analise a diferença à maior
					SET @insssalcontrib = @insssalcontrib - (@feriasinss + @INSS);
					if ( @insssalcontrib > 0.02)  -- se a dif.for maior q 0,02
					 begin
					   SET @INSS = @INSS + @insssalcontrib;
					 end;
				end;
				if (@fim >= '2019-05-01') AND ((@feriasinss + @INSS) > @TETO_INSS) 
			    begin
			    	SET @INSS = @TETO_INSS - @feriasinss; 
					IF (@INSS < 0) 
					BEGIN   SET @INSS = 0; END;
				END;
		  -- calcula se o salario familia mudou de faixa em função do Salario Contrib. (ferias+salario)
				if (@ndepend > 0) 
				begin
				   DECLARE @VlrSUBSTITUA NUMERIC(19,4) = @TRAB_EM_FERIAS_PROV_DECIMO;
				    if (@fim < '2019-09-01') 
					BEGIN
					   SET @VlrSUBSTITUA = @salcontrib; 
					END;
				
				    if (@fim > '2019-10-01') 
					BEGIN
					  -- Incluido a partir da folha de agosto 2020, o @vlrdiarias para efeitos de calculo nestes casos
					  
					    if (@fim >= '2021-03-01') 
					    BEGIN
					      SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
						      (@SALARIO_CONTRATUAL +  @FERIAS_MEDIA + @vlrhorasfe+ @vlrhoras50+
							    @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO), @ndepend);
                        END
						ELSE
						BEGIN
					      SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
						      (@SALMEDIO_FERIAS + @vlrhorasfe+ @vlrhoras50+ @vlrhoras100
							  +@vlrdiarias_adc + @vlrdiarias_adc_PREMIO), @ndepend);
                        END
					END
					ELSE
					BEGIN
				       SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
						   (@VlrSUBSTITUA+@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100
						   +@vlrdiarias_adc+@vlrdiarias_adc_PREMIO), @ndepend);

					   if (@tsalfam > @feriassalfam) 
					   begin --diferença a mais pago no SALFAM . OQUE FAZER???
						  SET @tsalfam = (@tsalfam - @feriassalfam);
					   end
					   else
						 SET  @tsalfam = 0;
                   end; 
				end;
				--SET @FERIAS = @feriasProv_Ferias;
			  end;
		  end;

		  SET @SALFAM = @tsalfam;

		  SET @totalfolha = (@totalfolha +@SALARIO+@VLR_HXS+ @VLR_HXA+ @VLR_HXN+ROUND(@GRATIF,2));

		 /* IF (@FERIAS < 0) -- DEBUG
		  BEGIN
		  SET @SALFAM = @tsalfam;
		  END;
		  */

		  Insert into @FOLHA  (trab ,SETOR,CENTRO,TIPO,
		  SALARIO,VLR_HXS, VLR_HXA, VLR_HXN,FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
		   FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,MULTA_FGTS,HX,HXS,HXA, HXN,HEFETIVA,PONTO,IRFONTE,
	      GRATIF,FALTAS,HORAS_DOM,HORAS_FER,VLR_DOM,VLR_FER, DIAS) VALUES 	
	      (@trab , @setor ,@centro,'D',
		  @SALARIO,@VLR_HXS, @VLR_HXA, @VLR_HXN,@FGTS,@SALFAM,@INSS,@FERIAS,@DECIMO,@FGTS_FE,
		   @FGTS_DEC,@EDUC,@TERC,@EDUC_FE,@EDUC_DEC,@MULTA_FGTS,@HX,@HXS,@HXA, @HXN,@HEFETIVA,@PONTO,@IRFONTE,
	      ROUND(@GRATIF,2),@FALTAS,@aghoras_dom,@aghoras_fer,@VLR_DOM,@VLR_FER, @dias)
		  -- @dias
	      FETCH NEXT  FROM cursor_mst INTO
		  @agtrab,
	      @agdias,
	      @agdiarias, @agdiarias_n,
	      @aghorasfe,@aghorasfen,
	      @aghoras50 ,@aghoras50n,
	      @aghoras100,
     	  @aghoras_efetivas,
	      @agdiarias_adc,
	      @agfaltas,@agdoenca,@agdiasferias,@aghoras_dom,@aghoras_fer;
	   END;
	   CLOSE cursor_mst;  
       DEALLOCATE cursor_mst;  

	
	
	   -- rotina de salario_educação e terceiros
	  DECLARE @teduc numeric(19,4) ,@tterc numeric(19,4);
	  DECLARE @reduc numeric(19,4) ,@rterc numeric(19,4); 
	  SET @teduc = Round(@totalfolha * 0.025, 2);
      SET @tterc = Round(@totalfolha * 0.002, 2);
      UPDATE @FOLHA SET EDUC = Round(((SALARIO+VLR_HXS+ VLR_HXA+ VLR_HXN+GRATIF)/@totalfolha) * @teduc,2),
    	  TERC = Round(((SALARIO+VLR_HXS+ VLR_HXA+ VLR_HXN+GRATIF)/@totalfolha) * @tterc,2);
      SELECT @reduc = SUM(EDUC), @rterc = SUM(TERC) FROM @folha;
	  SET @reduc = @teduc - @reduc;
	  SET @rterc = @tterc - @rterc;
	  DECLARE @maior nvarchar(4);
	  if (@reduc <> 0) 
	  begin
	      SELECT TOP(1) @maior=TRAB FROM @FOLHA ORDER BY EDUC DESC; 
	      UPDATE @FOLHA  SET EDUC = (EDUC + @reduc) WHERE 
		   TRAB =  @maior;
	  end;	  
      if (@rterc <> 0) 
	  begin
	      SELECT TOP(1) @maior=TRAB FROM @FOLHA ORDER BY TERC DESC; 
	      UPDATE @FOLHA  SET TERC = (TERC + @terc) WHERE 
		   TRAB =  @maior;
	  end;	  
      

	     -- Rotina do ponto
	  --DECLARE @tabelaPonto TABLE(trab nvarchar(4), PONTO nvarchar(62));
	 --cursor      
	  DECLARE @pttrab nvarchar(4),@ptdtadia date,@ptindcod nvarchar(2),@ptdiarias numeric(14,2);
	 --trabalho pra logica
	  DECLARE @oponto nvarchar(62),@otrab nvarchar(4), @oacumdiarias numeric(14,2),@odtadia date,@oindcod nvarchar(2),
	  @oindcod2 nvarchar(2);
	  
	  DECLARE cursor_ponto CURSOR FOR  
	   SELECT trab,dtadia,indcod,
	    Diarias as diarias
	    FROM @tabelaprocess     -- where trab = '8942' 
	       order by trab,dtadia;
	 
	  OPEN cursor_ponto;
	  FETCH NEXT  FROM cursor_ponto INTO
	      @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;
      WHILE  @@FETCH_STATUS = 0  
       BEGIN  
		 SET @oponto = ''; 
		 DECLARE @ndias int = DateDiff(day,@inicio, @ptdtadia);
		 if (@ndias > 0) 
		 begin
		   SET @oponto = REPLICATE('  ',@ndias);
		 end;
		 SET @otrab = @pttrab; 
        DECLARE @auxtabela TABLE (indcod nvarchar(2),tabdiaria numeric(14,4));
		 
		 WHILE (@@FETCH_STATUS = 0)  and (@otrab = @pttrab)
         BEGIN
		    SET @odtadia = @ptdtadia;
		    SET @oacumdiarias = @ptdiarias; 
	        SET @oindcod = @ptindcod;
			
			FETCH NEXT  FROM cursor_ponto INTO
	            @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;      
		    
			if (@odtadia <> @ptdtadia)
			begin
			     SET @oponto =   @oponto +  @oindcod;
				
				 SET @i =  DATEDIFF(day, @odtadia, @ptdtadia) - 1;
				 IF (@i > 0) 
				 begin
				     SET @oponto = @oponto + REPLICATE(space(2),@i);
                 end;
				 
				continue; 
			 end;
			 insert into @auxtabela (indcod,tabdiaria) VALUES(@ptindcod, @ptdiarias);
			 WHILE (@@FETCH_STATUS = 0)  and (@otrab = @pttrab) and (@odtadia = @ptdtadia)
             BEGIN  
			     insert into @auxtabela (indcod,tabdiaria) VALUES(@ptindcod, @ptdiarias)
				 SET @oacumdiarias = @oacumdiarias + @ptdiarias;
				 FETCH NEXT  FROM cursor_ponto INTO
	              @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;      
		     END;
			 if (@oacumdiarias <> 0)   
             begin   
			   if (@oacumdiarias >= 7.9) and (@oacumdiarias <=8.1)   
               begin
			      if  Exists(select * from @auxtabela where (Substring(indcod,1,1)='Y') )
				      SET @oponto = @oponto + 'Y6';
                  else
			      begin
				    if  Exists(select * from @auxtabela where (Substring(indcod,1,1)='R') )
				        SET @oponto = @oponto + 'R ';
					else
						SET @oponto = @oponto + 'X ';
				  end;
               end
               else
               begin
			       SELECT @oindcod2 = INDCOD from @tabcltcodigo where (ROUND(fator,3) = ROUND(@oacumdiarias,3));
					if (@oindcod2 = null)  
					begin
					   if (@oacumdiarias>8)
			              SET @oponto =@oponto+'X+';
					   else
					      SET @oponto =@oponto+'? ';
					end
					else
					   SET @oponto =@oponto+'!!';
			   end
			end
			else
			begin
			  select top(1) @oindcod2 = INDCOD from @auxtabela where (Substring(indcod,1,1)<>'R')
			  if ( @oponto  <> null)
			     SET @oponto = @oponto +  @oindcod2
			  else
			     SET @oponto = @oponto +  '  ';
			end;
			delete  @auxtabela;
    	 END    
         --insert into @tabelaPonto (trab,ponto) VALUES(@otrab,@oponto);  		
		 UPDATE @FOLHA SET PONTO = @oponto where (TRAB = @otrab);
       END
	   CLOSE cursor_ponto;  
       DEALLOCATE cursor_ponto;  
  
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX CASO DESEJE PROCESSAR O CUSTO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
--if (@custo = 1)
--BEGIN
--------   CHAMADA  do  [dbo].[FolhaClt_Atualiza_TAREFAS] -----------
    -- XXXXXXXX DESATIVEI ESTA ROTINA EM 18.01.2024 e substitui pelo sp [dbo].[CLT_PontoValorNew] @inicio_aux date, @fim_aux date
	EXEC [dbo].[CLT_PontoValorNew] @inicio, @fim
	
	/*DECLARE @X_TRAB TABLE (setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  semana date,   dtadia date, TRAB nchar(4), NumTarefa int, dataLIMITEINFERIOR date, INICIOTAR date, FIMTAR date, DATAOK date); 
	insert into @X_TRAB
       EXECUTE  [dbo].[FolhaClt_Atualiza_TAREFAS]
          @inicio
          ,@fim;
      
       UPDATE tab SET numtarefa = du.NumTarefa 
	   FROM @tabelaprocess as tab 
	    LEFT JOIN   @X_TRAB du ON (
	     tab.bl = du.bl and tab.setor = du.setor and tab.gleba = du.gleba and
		  tab.num_mod = du.num_mod and tab.codser = du.codser and  tab.dtadia between 
		  du.INICIOTAR  and du.FIMTAR) 
	   where tab.bl <> ''   

	   
       DECLARE @CUSTOFOLHA TABLE (   
		  [Id] [int] IDENTITY(1,1) NOT NULL,
		  INICIO date,
		  FIM date,
		  SETOR nchar(2),
	      CENTRO nchar(4),
		  QUADRA nchar(3),
		  TRAB nchar(4),
	      OK nchar(1),
		  NUM_MOD nchar(2),
		  CODSER nchar(3),
		  QUANT numeric(19,4), 
		  SALARIO numeric(19,6), --vlrdiarias
		  VLR_HXS numeric(19,6), -- vlrhorasfe
		  VLR_HXA numeric(19,6), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
		  FGTS numeric(19,6),
		  SALFAM numeric(19,6),
		  INSS numeric(19,6),
		  FERIAS numeric(19,6),
		  DECIMO numeric(19,6),
		  FGTS_FE numeric(19,6),
		  FGTS_DEC numeric(19,6),
	      EDUC numeric(19,6),
		  TERC numeric(19,6),
		  EDUC_FE numeric(19,6),
		  EDUC_DEC numeric(19,6),
		  MULTA_FGTS numeric(19,6),
		  HX numeric(19,4),
		  HXS numeric(19,4), -- vlrhorasfe
		  HXA numeric(19,4), -- vlrhoras50
	      HXN numeric(19,4),-- vlrhoras100
	      HEFETIVA numeric(19,4),
	      IRFONTE numeric(19,4),
	      GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  FALTAS numeric(13,0),
		  DOENCA numeric(13,0),
		  BANCOHS_E	numeric(19,4),
		  BANCOHS_C	numeric(19,4),
		  TAREFA_IND_MES	numeric(18,0)
	  );
	   DECLARE 
		  @agsetor NVARCHAR(2),
		  @agcentro NVARCHAR(4),
		  @agquadra NVARCHAR(3),
		  @agnum_mod NVARCHAR(2),
	      @agcodser NVARCHAR(3),
		  @agdtadia date,
		  @agok NVARCHAR(1),
		  @agindcod NVARCHAR(2),
		  @agquant numeric(19,4),
	      
		  @ptsetor NVARCHAR(2),
		  @ptcentro NVARCHAR(4),
		  @ptquadra NVARCHAR(3),
		  @ptnum_mod NVARCHAR(2),
	      @ptcodser NVARCHAR(3),
		  @diainicio date,
		  @diafim date,
		  @ptok nchar(1),
		  @ptquant numeric(19,4),
		  @ptHX numeric(19,4),
		  @ptHXS numeric(19,4), -- vlrhorasfe
		  @ptHXA numeric(19,4), -- vlrhoras50
	      @ptHXN numeric(19,4),-- vlrhoras100
	      @ptHEFETIVA numeric(19,4),
		  @ptDiarias_adc numeric(19,4),
	      @ptfaltas int,
		  @ptdoenca int,

		  @ptBancoHs_E numeric(19,4),
		  @ptBANCOHs_C numeric(19,4),

		  @agINICIO date,
	      @agFIM date
		   ,@agID int;
	 	 
		-- DECLARE @TESTEDIAS int;
	


	 -- ESPAÇO DE DIAS ENTRE UMA OCORRENCIA E OUTRA = 10 DIAS
	-- DECLARE @DIAS_CORRIDOS int = 12;
	-- DECLARE @DATA_ULT_OK date = null;
	 
	-- DECLARE @NOVATAREFA int;
	-- Não irei trabalhar com agregados 
    
     DECLARE @agNUmTarefa int,  @ptNUmTarefa int; 
	
	 DECLARE cursor_custo CURSOR FOR   
      SELECT trab, 
	 setor ,gleba ,bl ,num_mod ,codser ,
	    dtadia ,indcod,ok, Diarias , Diariasn, Horasfe,
	  Horasfen, Horas50, Horas50n, Horas100, horas_efetivas,
	  Diarias_adc,quant, numtarefa from @tabelaprocess order by trab,setor,gleba,bl,codser,dtadia,num_mod;
	   OPEN cursor_custo;
	   FETCH NEXT  FROM cursor_custo INTO
		  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		  @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas,@agdiarias_adc,
		  @agquant, @agNumTarefa;
	 
	 WHILE @@FETCH_STATUS = 0  
     BEGIN  
	     SET @pttrab =  @agtrab;
	     SELECT @SALARIO = SALARIO,@VLR_HXS =VLR_HXS , @VLR_HXA = VLR_HXA, @VLR_HXN = VLR_HXN, @FGTS = FGTS,  @SALFAM =SALFAM, @INSS=INSS,
		  @FERIAS=FERIAS, @DECIMO=DECIMO, @FGTS_FE = FGTS_FE , @FGTS_DEC = FGTS_DEC, @EDUC = EDUC, @TERC = TERC ,
		  @EDUC_FE = EDUC_FE, @EDUC_DEC = EDUC_DEC, @HX = HX, @HXS = HXS, @HXA=HXA,  @HXN=HXN, @HEFETIVA=HEFETIVA,
	      @IRFONTE=IRFONTE, @GRATIF=GRATIF FROM @FOLHA WHERE TRAB=@agtrab; 	 
	      -- valor da hora na diaria normal
		  SET @vlrDiarias = iif(@HX<>0,(@SALARIO/@HX),0);
		  -- valor da hora na horaextra
		  SET @vlrhoras50 = iif(@HXS<>0,(@VLR_HXS/@HXS),0);
		  -- valor da hora na hora trabalhada em feriado
		  SET @vlrhorasfe = iif(@HXA<>0,(@VLR_HXA/@HXA),0);
		  -- vlr hora do adcional noturno
		  SET @vlrhoras100 = iif(@HXN<>0,(@VLR_HXN/@HXN),0);
		  
		  
		  if (@pttrab = '8955')
		  BEGIN
		     SET @pttrab = '8955';
		  END;

		  WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab)
          BEGIN  
		    SET @ptsetor =   @agsetor;
	    	SET @ptcentro = @agcentro;
		    SET @ptquadra = @agquadra;
		    SET @ptnum_mod = @agnum_mod;
	        SET @ptcodser = @agcodser;
			SET @ptNUmTarefa = @agNUmTarefa;
	        
			SET @ptHX =@agdiarias;
		    SET @ptHXS = @aghorasfe;
		    SET @ptHXA = @aghoras50;
	        SET @ptHXN = @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; 
	        SET @ptHEFETIVA = @aghoras_efetivas;
	        SET @ptdiarias_adc = @agdiarias_adc;-- diarias de gratificacao
			SET @diainicio = @agdtadia;
			SET @diafim = @agdtadia;
			SET @ptok =@agok;
			SET @ptquant =@agquant;
			SET @ptDiarias_adc = 0;
			SET @ptfaltas = iif(@agindcod='FI',1,0);
			SET @ptdoenca = iif(@agindcod='A',1,0);
			SET @ptBancoHs_E =   iif(substring(@agindcod,1,1) <> 'E',0, 
	           CONVERT(int, substring(@agindcod,2,1)  )  ); 
	      	SET @ptBancoHs_C =   iif(substring(@agindcod,1,1) <> 'C',0, 
	           CONVERT(int, substring(@agindcod,2,1)  )  ); 
	        


			FETCH NEXT  FROM cursor_custo INTO
	    	  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		      @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas, @agdiarias_adc,@agquant,@agNumTarefa;
			-- ALTERAÇÂO IMPORTANTE MAIO 2019
			WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab) and 
			   (@ptsetor= @agsetor) and  (@ptcentro = @agcentro) and (@ptquadra= @agquadra) and
			  (  @ptnum_mod= @agnum_mod) and  (@ptcodser = @agcodser) 
			  -- ALTERAÇÂO maio 2019
			  	and (@ptNUmTarefa = @agNUmTarefa)
			  -- and @TESTEDIAS < @DIAS_CORRIDOS
            BEGIN
			  SET @ptHX =@ptHX+ @agdiarias;
		      SET @ptHXS =@ptHXS+ @aghorasfe;
		      SET @ptHXA =@ptHXA+ @aghoras50;
	          SET @ptHXN =@ptHXN + @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; 
	          SET @ptHEFETIVA = @ptHEFETIVA+@aghoras_efetivas;
	          SET @ptdiarias_adc = @ptdiarias_adc + @agdiarias_adc;
			  SET @diafim = @agdtadia;
			  if (@ptok='')
			     SET @ptok =@agok;
			  SET @ptquant = @ptquant +@agquant;
			  SET @ptfaltas = @ptfaltas + iif(@agindcod='FI',1,0);
			  SET @ptdoenca = @ptdoenca + iif(@agindcod='A',1,0);
			  SET @ptBancoHs_E = @ptBancoHs_E +  iif(substring(@agindcod,1,1) <> 'E',0, 
	                                         CONVERT(int, substring(@agindcod,2,1)  )  ); 
	          SET @ptBancoHs_C =  @ptBancoHs_C + iif(substring(@agindcod,1,1) <> 'C',0, 
	           CONVERT(int, substring(@agindcod,2,1)  )  ); 


			  FETCH NEXT  FROM cursor_custo INTO
	    	  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		      @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas, @agdiarias_adc,@agquant,@agNumTarefa;
       		
		    END;
          -- verificar se houve gratificação 
		    if Exists(Select gcodser from @tabelagratif as grat where ((@ptcodser = grat.gcodser) and 
      	      ((@ptsetor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))) and (@ptHEFETIVA<>0)
            begin
		        SET @ptDiarias_adc =  @ptHEFETIVA *
			      (Select grat.perc_diaria/100 from @tabelagratif as grat where (@ptcodser = grat.gcodser)) * @vlrDiarias;
		    end;

	        INSERT INTO @CUSTOFOLHA (INICIO ,FIM ,SETOR,CENTRO,QUADRA,TRAB, OK,
		         NUM_MOD,CODSER,QUANT,SALARIO,VLR_HXS,VLR_HXA,VLR_HXN, HX,HXS,
		        HXA,HXN, HEFETIVA,  GRATIF, FALTAS,DOENCA, BANCOHS_E,BANCOHS_C, TAREFA_IND_MES)
		      VALUES (@diainicio,@diafim, @ptsetor, @ptcentro,@ptquadra,@pttrab,@ptok,
		         @ptnum_mod,@ptcodser,@ptquant,ROUND(@ptHX * @vlrDiarias,2), ROUND(@ptHXS * @vlrhoras50,2),
			     ROUND(@ptHXA * @vlrhorasfe,2), ROUND(@ptHXN * @vlrhoras100,2),@ptHX,@ptHXS,
			     @ptHXA,@ptHXN, @ptHEFETIVA,ROUND(@ptDiarias_adc,2),@ptfaltas,@ptdoenca, @ptBancoHs_E,@ptBANCOHs_C,@ptNUmTarefa);
     	 END;
	 END;	 
	 CLOSE cursor_custo;  
     DEALLOCATE cursor_custo;
	 
	 -- EM ABRIL DE 2021
	  INSERT INTO @CUSTOFOLHA (INICIO ,FIM ,SETOR,CENTRO,QUADRA,TRAB, OK,
		         NUM_MOD,CODSER,QUANT,SALARIO,VLR_HXS,VLR_HXA,VLR_HXN, HX,HXS,
		        HXA,HXN, HEFETIVA,  GRATIF, FALTAS,DOENCA, BANCOHS_E,BANCOHS_C, TAREFA_IND_MES)
		     SELECT DTA_INI ,DTA_FIM ,SETOR,GLEBA as CENTRO, BL as QUADRA,TRAB, '' OK,
		         NUM_MOD,CODSER, 0 QUANT, 0 SALARIO, 0 VLR_HXS, 0 VLR_HXA, 0 VLR_HXN, 0 HX, 0 HXS,
		        0 HXA, 0 HXN, 0 HEFETIVA, VALOR GRATIF, 0 FALTAS, 0 DOENCA, 0 BANCOHS_E, 0 BANCOHS_C, 0TAREFA_IND_MES
				FROM @tabelaPREMIO WHERE INCLUIDO = 1;
				*/
	/*
	 */
	 /*
	 */
	 
	/*   
	 -- alocar as diferenças de calculo
	  DECLARE @difsal numeric(19,4),@difhxs numeric(19,4),@difhxa numeric(19,4),@difhxn numeric(19,4),
	  @difgratif numeric(19,4);
	  DECLARE @tabDIF TABLE (trab nvarchar(4),difsal numeric(19,4),difhxs numeric(19,4),difhxa numeric(19,4),
	  difhxn numeric(19,4),	difgratif numeric(19,4));
	  
	  
	  INSERT INTO @tabDIF
	  SELECT TRAB, SALARIO- (SELECT SUM(cus.SALARIO) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsal ,
	  VLR_HXS- (SELECT SUM(cus.VLR_HXS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxs,
	  VLR_HXA- (SELECT SUM(cus.VLR_HXA) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxa,
	  VLR_HXN- (SELECT SUM(cus.VLR_HXN) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxn,
	  GRATIF- (SELECT SUM(cus.GRATIF) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difgratif
	   FROM @FOLHA as fol   order by fol.trab;
      
	  DECLARE cursor_rateio CURSOR FOR   
      SELECT trab, difsal,difhxs,difhxa,
	  difhxn,	difgratif from @tabdif order by trab;
	  OPEN cursor_rateio;
	    FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsal,@difhxs,@difhxa,
        	  @difhxn,	@difgratif;
     WHILE (@@FETCH_STATUS = 0)
	 begin
	    if (@difsal <> 0) 
           UPDATE @CUSTOFOLHA SET SALARIO = (SALARIO + @difsal)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.SALARIO DESC);
		if (@difhxs <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXS = (VLR_HXS + @difhxs)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXS DESC);
		if (@difhxa <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXA = (VLR_HXA + @difhxa)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXA DESC);
		if (@difhxn <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXN = (VLR_HXN + @difhxn)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXN DESC);
		if (@difgratif <> 0) 
           UPDATE @CUSTOFOLHA SET GRATIF = GRATIF + @difgratif  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.GRATIF DESC);

		FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsal,@difhxs,@difhxa,
        	  @difhxn,	@difgratif;
	 end;
	 CLOSE cursor_rateio;  
     DEALLOCATE cursor_rateio;  

	 --parcelas de OBRIGAÇÔES que serão rateadas pelo valor da folha
--	   SALARIO,VLR_HXN,GRATIF,VLR_HXS,VLR_HXA,  FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
	--	   FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,IRFONTE,
DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @agSALARIO numeric(19,4), --vlrdiarias
		  @agVLR_HXS numeric(19,4), -- vlrhorasfe
		  @agVLR_HXA numeric(19,4), -- vlrhoras50
	      @agVLR_HXN numeric(19,4),-- vlrhoras100
		  @agGRATIF numeric(19,4),-- vlrdiarias_adc
		  @agFGTS numeric(19,4),
		  @agSALFAM numeric(19,4),
		  @agINSS numeric(19,4),
		  @agFERIAS numeric(19,4),
		  @agDECIMO numeric(19,4),
		  @agFGTS_FE numeric(19,4),
		  @agFGTS_DEC numeric(19,4),
	      @agEDUC numeric(19,4),
		  @agTERC numeric(19,4),
		  @agEDUC_FE numeric(19,4),
		  @agEDUC_DEC numeric(19,4),
		    @agIRFONTE numeric(19,4),
			 @agMULTA_FGTS numeric(19,4);
		
		  DECLARE @ptsalario  numeric(19,4),
		  @fator numeric(19,4),
		  @ptFGTS numeric(19,4),
		  @ptSALFAM numeric(19,4),
		  @ptINSS numeric(19,4),
		  @ptFERIAS numeric(19,4),
		  @ptDECIMO numeric(19,4),
		  @ptFGTS_FE numeric(19,4),
		  @ptFGTS_DEC numeric(19,4),
	      @ptEDUC numeric(19,4),
		  @ptTERC numeric(19,4),
		  @ptEDUC_FE numeric(19,4),
		  @ptEDUC_DEC numeric(19,4),
          @ptIRFONTE numeric(19,4),
		  @ptMULTA_FGTS numeric(19,4),
		  @ptid integer;
		 -- @agid integer; 	  
   
	  	 DECLARE @fator_Pivot int; 

	DECLARE cursor_obrigacao CURSOR FOR   
      SELECT trab,id,SALARIO,VLR_HXN,GRATIF,VLR_HXS,VLR_HXA,  FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
	     FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,IRFONTE,MULTA_FGTS from @CUSTOFOLHA order by trab;
	  OPEN cursor_obrigacao;
	    FETCH NEXT  FROM cursor_obrigacao INTO
	       @agtrab, @agid,@agSALARIO,@agVLR_HXN,@agGRATIF,@agVLR_HXS,@agVLR_HXA,  @agFGTS,@agSALFAM,@agINSS,@agFERIAS,@agDECIMO,@agFGTS_FE,
	     @agFGTS_DEC,@agEDUC,@agTERC,@agEDUC_FE,@agEDUC_DEC,@agIRFONTE, @agMULTA_FGTS;
     WHILE (@@FETCH_STATUS = 0)
	 begin
	     SET @pttrab =  @agtrab;
		 
	     SELECT @SALARIO = (SALARIO+VLR_HXS +VLR_HXA+ VLR_HXN+GRATIF), @FGTS = FGTS,  @SALFAM =SALFAM, @INSS=INSS,
		  @FERIAS=FERIAS, @DECIMO=DECIMO, @FGTS_FE = FGTS_FE , @FGTS_DEC = FGTS_DEC, @EDUC = EDUC, @TERC = TERC ,
		  @EDUC_FE = EDUC_FE, @EDUC_DEC = EDUC_DEC,
	      @IRFONTE=IRFONTE , @MULTA_FGTS=MULTA_FGTS FROM @FOLHA WHERE TRAB=@pttrab; 	 
	     
		 --SELECT @ptdiarias = SUM(SALARIO+VLR_HXS+VLR_HXA+VLR_HXN+GRATIF) FROM @CUSTOFOLHA WHERE TRAB=@agtrab;
	    -- SET 
		   
		 SET @fator_Pivot = 0;
		 WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab)
         BEGIN  
			-- REVISAO DE 2019 MARÇO
		     SET @fator = 0;
		     SET @ptsalario = @agSALARIO+@agVLR_HXN+@agGRATIF+@agVLR_HXS+@agVLR_HXA;
			 if (@SALARIO <> 0) and (@ptsalario<>0)
			 begin
		         SET @fator =  @ptsalario/@SALARIO;
			 END
			 ELSE 
			 IF (@FERIAS <> 0) and (@fator_PIVOT =  0) 
			 BEGIN
			     SET @fator = 1;
				 SET @fator_PIVOT = 1;
				 IF (@ptsalario=0)
				 BEGIN
				   SET @fator = 0;
				 END;
			 END;
			 IF @FATOR <> 0
			 BEGIN
				SET @ptid = @agid; 
			
				SET @ptfgts = iif(@FGTS<>0,ROUND(@fator*@FGTS,2),0); 	    
				SET @ptsalfam = iif(@SALFAM<>0, ROUND(@fator*@SALFAM,2),0); 	    
				SET @ptirfonte = iif(@IRFONTE<>0, ROUND(@fator*@IRFONTE,2),0); 	    
				SET @ptinss = iif(@INSS<>0,ROUND(@fator*@INSS,2),0); 	    
				SET @ptEDUC = iif(@EDUC<>0,ROUND(@fator*@EDUC,2),0); 	    
				SET @ptTERC = iif(@TERC<>0,ROUND(@fator*@TERC,2),0); 	    
				
				SET @ptFERIAS = iif(@FERIAS<>0,ROUND(@fator*@FERIAS,2),0); 	    
				SET @ptDECIMO = iif(@DECIMO<>0,ROUND(@fator*@DECIMO,2),0); 	    
				SET @ptFGTS_FE = iif(@FGTS_FE<>0,ROUND(@fator*@FGTS_FE,2),0); 	    
				SET @ptFGTS_DEC = iif(@FGTS_DEC<>0,ROUND(@fator*@FGTS_DEC,2),0); 	    
				SET @ptEDUC_FE = iif(@EDUC_FE<>0,ROUND(@fator*@EDUC_FE,2),0); 	    	
				SET @ptEDUC_DEC = iif(@EDUC_DEC<>0,ROUND(@fator*@EDUC_DEC,2),0); 	    	
				
				SET @ptMULTA_FGTS = iif(@MULTA_FGTS<>0,ROUND(@fator*@MULTA_FGTS,2),0); 	    	
		  
                
	            		
			
				UPDATE @CUSTOFOLHA SET FGTS=iif(FGTS is null,0,FGTS) +@ptfgts,SALFAM=iif(SALFAM is null,0,SALFAM)+@ptsalfam,
				  IRFONTE = iif(IRFONTE is null,0,IRFONTE)+@ptIRFONTE,
				         INSS =iif(INSS is null,0,INSS)+ @ptINSS,EDUC=iif(EDUC is null,0,EDUC)+@ptEDUC, 
						 TERC =iif(TERC is null,0,TERC)+ @ptTERC,
						 FERIAS =iif(FERIAS is null,0,FERIAS)+ @ptFERIAS,
						 DECIMO =iif(DECIMO is null,0,DECIMO)+ @ptDECIMO,
						 FGTS_FE =iif(FGTS_FE is null,0,FGTS_FE)+ @ptFGTS_FE,    
						 FGTS_DEC =iif(FGTS_DEC is null,0,FGTS_DEC)+ @ptFGTS_DEC,
						 EDUC_FE =iif(EDUC_FE is null,0,EDUC_FE)+ @ptEDUC_FE,    
						 EDUC_DEC =iif(EDUC_DEC is null,0,EDUC_FE)+ @ptEDUC_DEC,
						 MULTA_FGTS =iif(MULTA_FGTS is null,0,MULTA_FGTS)+ @ptMULTA_FGTS
						 where (id = @ptid);  
			 END;
			  FETCH NEXT  FROM cursor_obrigacao INTO
	         @agtrab, @agid,@agSALARIO,@agVLR_HXN,@agGRATIF,@agVLR_HXS,@agVLR_HXA,  @agFGTS,@agSALFAM,@agINSS,@agFERIAS,@agDECIMO,@agFGTS_FE,
	        @agFGTS_DEC,@agEDUC,@agTERC,@agEDUC_FE,@agEDUC_DEC,@agIRFONTE, @agMULTA_FGTS;

		 END;


	 end;
	 CLOSE cursor_obrigacao;  
     DEALLOCATE cursor_obrigacao;  
      



      DECLARE @difsalfam numeric(19,4),@difinss numeric(19,4),@difirfonte numeric(19,4),@diffgts numeric(19,4),
	  @difeduc numeric(19,4),@difterc numeric(19,4),
	 
	  @differias numeric(19,4) , @difdecimo numeric(19,4) ,@diffgts_fe numeric(19,4), @diffgts_dec numeric(19,4) ,
	   @difeduc_fe numeric(19,4) , @difeduc_dec numeric(19,4) ,@difmulta_fgts numeric(19,4)   ;



	  DECLARE @tabObrigDif TABLE (trab nvarchar(4),difsalfam numeric(19,4),difinss numeric(19,4),difirfonte numeric(19,4),
	  diffgts numeric(19,4),	difeduc numeric(19,4),difterc numeric(19,4),
	   differias numeric(19,4) , difdecimo numeric(19,4) ,diffgts_fe numeric(19,4), diffgts_dec numeric(19,4) ,
	   difeduc_fe numeric(19,4) , difeduc_dec numeric(19,4) ,difmulta_fgts numeric(19,4)   
	  );
	  
	  
	  INSERT INTO @tabObrigDif

	 SELECT  TRAB, SALFAM- (SELECT SUM(cus.SALFAM) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsalfam ,
	  INSS- (SELECT SUM(cus.INSS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difINSS,
	  IRFONTE- (SELECT SUM(cus.IRFONTE) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_IRFONTE,
	  FGTS- (SELECT SUM(cus.FGTS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS,
	  EDUC- (SELECT SUM(cus.EDUC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC,
	  TERC- (SELECT SUM(cus.TERC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difTERC,

	  FERIAS- (SELECT SUM(cus.FERIAS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFERIAS,
	  DECIMO- (SELECT SUM(cus.DECIMO) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difDECIMO,
	  FGTS_FE- (SELECT SUM(cus.fgts_fe) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as diffgts_fe,
	  FGTS_DEC- (SELECT SUM(cus.FGTS_DEC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS_DEC,
	  EDUC_FE- (SELECT SUM(cus.EDUC_FE) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC_FE,
	  EDUC_DEC- (SELECT SUM(cus.EDUC_DEC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC_DEC,
	  MULTA_FGTS- (SELECT SUM(cus.MULTA_FGTS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difMULTA_FGTS



	   FROM @FOLHA as fol   order by fol.trab;
	 -- DECLARE @testea int;
	  DECLARE cursor_rateio CURSOR FOR   
      SELECT trab, difsalfam,difinss,difirfonte,
	  diffgts,	difeduc ,	difterc ,
	    differias  , difdecimo  ,diffgts_fe , diffgts_dec  ,
	   difeduc_fe  , difeduc_dec  ,difmulta_fgts
	   from @tabObrigDif order by trab;
	  OPEN cursor_rateio;
	    FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsalfam,@difinss,@difirfonte,
        	  @diffgts,	@difeduc,@difterc,  @differias  , @difdecimo  ,@diffgts_fe , @diffgts_dec  ,
	   @difeduc_fe  , @difeduc_dec  ,@difmulta_fgts;
 ;
     WHILE (@@FETCH_STATUS = 0)
	 begin

		if (@difsalfam <> 0) 
           UPDATE @CUSTOFOLHA SET SALFAM = (SALFAm + @difsalfam)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.SALFAM DESC);
		if (@difinss <> 0) 
           UPDATE @CUSTOFOLHA SET INSS = (INSS + @difinss)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.INSS DESC);
		if (@difirfonte <> 0) 
           UPDATE @CUSTOFOLHA SET IRFONTE = (IRFONTE + @difirfonte)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.IRFONTE DESC);
		if (@diffgts <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS = (FGTS + @diffgts)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.FGTS DESC);
		if (@difeduc <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC = EDUC + @difeduc  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.EDUC DESC);
		if (@difterc <> 0) 
           UPDATE @CUSTOFOLHA SET TERC = TERC + @difterc  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

		if (@differias <> 0) 
		BEGIN
           /*if (@differias < 0)
		   BEGIN
		      SET @testea = @differias;
			  IF @testea < -10
			  BEGIN
			    SET @testea = 0; -- 1689
			  END; 
		   END; */
		   UPDATE @CUSTOFOLHA SET FERIAS = FERIAS + @difFERIAS  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);
        END;
        if (@difDECIMO <> 0) 
           UPDATE @CUSTOFOLHA SET DECIMO = DECIMO + @difDECIMO  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

         if (@difFGTS_FE <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS_FE = FGTS_FE + @difFGTS_FE  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);
		  
		  if (@difFGTS_DEC <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS_DEC = FGTS_DEC + @difFGTS_DEC  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

          if (@difEDUC_FE <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC_FE = EDUC_FE + @difEDUC_FE  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

         if (@difEDUC_DEC <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC_DEC = EDUC_DEC + @difEDUC_DEC  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

          if (@difMULTA_FGTS <> 0) 
           UPDATE @CUSTOFOLHA SET MULTA_FGTS = MULTA_FGTS + @difMULTA_FGTS  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);


		FETCH NEXT  FROM cursor_rateio INTO
		  @trab, @difsalfam,@difinss,@difirfonte,
        	  @diffgts,	@difeduc,@difterc, @differias  , @difdecimo  ,@diffgts_fe , @diffgts_dec  ,
	   @difeduc_fe  , @difeduc_dec  ,@difmulta_fgts;
	       
	 end;
	 CLOSE cursor_rateio;  
     DEALLOCATE cursor_rateio;

    DECLARE @SALARIO_folha numeric(19,4), @HORAS_folha numeric(19,4) , @GRATIF_folha numeric(19,4);
	
	SELECT @SALARIO_folha = SUM(SALARIO),  @HORAS_folha = SUM(VLR_HXS+VLR_HXN+VLR_HXA), @GRATIF_folha = SUM(GRATIF) FROM 
	           NOVACLTFOLHA WHERE DATA = @fim;    

    DECLARE @SALARIO_CUSTO numeric(19,4), @HORAS_CUSTO numeric(19,4) , @GRATIF_CUSTO numeric(19,4);
    SELECT @SALARIO_CUSTO = SUM(SALARIO),  @HORAS_CUSTO = SUM(VLR_HXS+VLR_HXN+VLR_HXA), @GRATIF_CUSTO = SUM(GRATIF) FROM 
	           @CUSTOFOLHA;    

    SET @SALARIO_CUSTO = @SALARIO_CUSTO  - @SALARIO_folha;
    SET @HORAS_CUSTO = @HORAS_CUSTO  - @HORAS_folha;
	SET @GRATIF_CUSTO = @GRATIF_CUSTO -  @GRATIF_folha;
	
	IF (not (@SALARIO_CUSTO < 0.05 and  @SALARIO_CUSTO > -0.05)) and 
	    (not(@HORAS_CUSTO between 0.05 and -0.05)) and (not(@GRATIF_CUSTO between 0.05 and -0.05))
	BEGIN
        IF @SOATUALIZA_CUSTOFOLHA = 1 
		BEGIN
			   -- Só FOI AUTORIZADO A ATUALIZAR O CUSTOFOLHA
			 SET @RETORNO = 9;
	         	 
		END

	END;

	 DELETE CUSTOFOLHA WHERE DATA_FOLHA = @fim;
	
	  SET IDENTITY_INSERT CUSTOFOLHA OFF
	  INSERT INTO CUSTOFOLHA 
	 
	  SELECT 
	      @FIM AS DATA_FOLHA,
		  INICIO ,
		  FIM ,
		  SETOR ,
	      CENTRO ,
		  QUADRA ,
		  TRAB ,
	      OK ,
		  NUM_MOD ,
		  CODSER ,
		  QUANT , 
		  SALARIO , --vlrdiarias
		  VLR_HXS , -- vlrhorasfe
		  VLR_HXA , -- vlrhoras50
	      VLR_HXN ,-- vlrhoras100
		  FGTS ,
		  SALFAM ,
		  INSS ,
		  FERIAS,
		  DECIMO ,
		  FGTS_FE ,
		  FGTS_DEC ,
	      EDUC ,
		  TERC ,
		  EDUC_FE ,
		  EDUC_DEC,
		  MULTA_FGTS ,
		  HX ,
		  HXS , -- vlrhorasfe
		  HXA , -- vlrhoras50
	      HXN ,-- vlrhoras100
	      HEFETIVA ,
	      IRFONTE ,
	      GRATIF ,-- vlrdiarias_adc
	 	  FALTAS ,
		  DOENCA,
		  0 as LINKTAREFA,
		  0 as LINKGRUPO,
		  BANCOHS_E,
		  BANCOHS_C,
		   TAREFA_IND_MES
		  FROM @CUSTOFOLHA	
	   
	   */
	   --XXXXXXXXXXXXXXXXXXXXXXXXXXXX     FIM custofolha            XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


IF @SOATUALIZA_CUSTOFOLHA = 1 
BEGIN
   -- Só FOI AUTORIZADO A ATUALIZAR O CUSTOFOLHA
   SELECT @RETORNO as RETORNO;
   REtURN;
END
	
      Delete NOVACLTFOLHA where (data = @fim);
      
	  SET IDENTITY_INSERT NOVACLTFOLHA OFF
	  INSERT INTO NOVACLTFOLHA 
	  (
		[DATA] ,
		[SETOR] ,
		[CENTRO] ,
		[QUADRA] ,
		[TRAB] ,
		[TIPO] ,
		[NUM_MOD] ,
		[CODSER] ,
		[QUANT] ,
		[SALARIO] ,
		[VLR_HXS] ,
		[VLR_HXA] ,
		[VLR_HXN] ,
		[FGTS] ,
		[SALFAM] ,
		[INSS] ,
		[FERIAS],
		[DECIMO] ,
		[FGTS_FE] ,
		[FGTS_DEC] ,
		[EDUC] ,
		[TERC] ,
		[EDUC_FE] ,
		[EDUC_DEC] ,
		[MULTA_FGTS] ,
		[HX] ,
		[HXS] ,
		[HXA] ,
		[HXN] ,
		[HEFETIVA] ,
		[PONTO] ,
		[IRFONTE] ,
		[GRATIF] ,
		[FALTAS] ,
		HORAS_DOM ,
		HORAS_FER ,
		VLR_DOM ,
		VLR_FER ,
		TXINSS ,
		IRDEDU ,
		DIAS ,
		ULTPONTO,
		PODEALTERAR 
	)
	
	SELECT @fim ,
		[SETOR] ,
		[CENTRO] ,
		[QUADRA] ,
		[TRAB] ,
		[TIPO] ,
		[NUM_MOD] ,
		[CODSER] ,
		[QUANT] ,
		[SALARIO] ,
		[VLR_HXS] ,
		[VLR_HXA] ,
		[VLR_HXN] ,
		[FGTS] ,
		[SALFAM] ,
		[INSS] ,
		[FERIAS],
		[DECIMO] ,
		[FGTS_FE] ,
		[FGTS_DEC] ,
		[EDUC] ,
		[TERC] ,
		[EDUC_FE] ,
		[EDUC_DEC] ,
		[MULTA_FGTS] ,
		[HX] ,
		[HXS] ,
		[HXA] ,
		[HXN] ,
		[HEFETIVA] ,
		[PONTO] ,
		[IRFONTE] ,
		[GRATIF] ,
		[FALTAS] ,
		HORAS_DOM ,
		HORAS_FER ,
		VLR_DOM ,
		VLR_FER ,
		TXINSS ,
		IRDEDU ,
		DIAS ,
		
			@fim as ULTPONTO,
			1 as PODEALTERAR 
		  	from @FOLHA ;
	
	  SET IDENTITY_INSERT NOVACLTFOLHA ON

	  SELECT @RETORNO as RETORNO;  -- SUCESSO NA ATUALIZAÇAO DA FOLHA E DO CUSTO FOLHA
	 -- EXEC  [dbo].[CLT_PontoValor] @inicio, @fim

	  RETURN;
END;
GO
PRINT 'EC141: FolhaClt_Atualiza ok';
GO

-- ===========================================================================
-- FolhaClt
-- ===========================================================================
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
--=============================================
 --Author:		<Author,,Name>
 --Create date: <Create Date,,>
 --Description:	<Description,,>
-- --=============================================
ALTER PROCEDURE [dbo].[FolhaClt]  
	-- Add the parameters for the stored procedure here
	
	@inicio date,
	@fim date,
  @naograve int
AS
BEGIN
	
	SET LANGUAGE us_english;
/*	DECLARE @inicio date,
 	@fim date,@naograve int;
    SET @inicio ='2018-06-01'
	SET @fim ='2018-06-30';*/
	SET @naograve = 1;
	SET @fim = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@fim)+1,0));

	DECLARE @comparafolha bit = 0;
	DECLARE @CUSTO bit = 1;


	SET NOCOUNT ON;
	-- pega a semana do ponto anterior ao inicio do mes
	DECLARE @primeirasemana date;
	SET @primeirasemana = @inicio;
	WHILE (DATENAME(weekday,@primeirasemana) <> 'Thursday') 
	begin
      	 SET @primeirasemana= DATEADD(dd,-1, @primeirasemana);
	end;

	-- NECESSÁRIO INCLUIR A ATUALIZAÇÂO DAS GLEBAS NO CLTPONTO (ATUALIZADO EM 11 de OUTUBRO de 2021

	UPDATE ponto
	SET ponto.FAZMOV = q.GLEBA
	--SELECT g.COD, *
	FROM CLTPONTO ponto
	left JOIN QUADRAS q ON (q.QUA = ponto.BL)   
	LEFT JOIN GLEBAS g ON (g.COD = q.GLEBA)
	WHERE DATA BETWEEN @primeirasemana and @fim
	and bl <> '' and (q.FIM is null) and (g.FIM is null) and (ponto.FAZMOV <> q.GLEBA)


--	TABELAS AUXILIARES
	-- Salario minimo e fgts
	DECLARE @SalMinFgts TABLE ([DATA] date,VALOR numeric(18,4),FGTS numeric(18,4));
	insert @SalMinFgts 
	EXECUTE [dbo].[SalMinFgts]
	        @fim; 
	DECLARE @SalMin numeric(18,4);		
	DECLARE @taxaFGTS numeric(18,4);
	SELECT @SalMin=VALOR,@taxaFGTS=FGTS from @SalMinFgts;

	-- tabeal do INNS  com tipo definido
	DECLARE @TabInss TipoTabelaINSS;
	insert into @TabInss
	SELECT [data], faixa, valor1, valor2, taxa FROM TABINSS  
         where  (Format(TABINSS.DATA,'yyyyMM') <= Format(@fim,'yyyyMM'))
       ORDER BY [DATA] DESC,faixa; 

    -- tabelas IRFONTE com tipo definido
	DECLARE @TabIrFonte TipoTabelaIRFonte;
	insert into @TabIrFonte
	SELECT TABIR.data, faixa, base1, base2, aliq, TABIR.deduzir,
    (SELECT MAX(irdedu.DEDUCAO) from irdedu where (TABIR.Data = IRDEDU.DATA) ) deduGeral
    FROM TABIR
	where  Format(TABIR.[data], 'yyyyMM') <= Format(@fim,'yyyyMM')
      ORDER BY TABIR.DATA DESC, faixa;

   -- tabela SALFAM
   DECLARE @TabSalFam TipoTabelaSalFam;
   insert into @TabSalFam
      SELECT data, faixa, valor1, valor2, salfam FROM SALFAM 
	  where  Format([data], 'yyyyMM') <= Format(@fim,'yyyyMM')
	  ORDER BY DATA DESC,faixa;
	

    --tabela gratificação
	DECLARE @tabelagratif TABLE (num_mod nvarchar(2),gcodser nvarchar(3),gsetor nvarchar(2),perc_diaria numeric(18,4),
	          vlr_real numeric(18,4),dta_ini date,dta_fim date  );
	insert into @tabelagratif                        
    SELECT NUM_MOD,CODSER,iif(SETOR='','  ',SETOR),[PERCENT],VLRDIARIA,DTA_INI,DTA_FIM FROM GRATIF WHERE 
                (DTA_INI <= @inicio) and ((DTA_FIM >= @fim) 
              or (DTA_FIM is null) ) and 
			  (CODSER is not null) and ([PERCENT] is not null) and (VLRDIARIA is not null);
	
	-- tabela cltcodig
	DECLARE @tabcltcodigo TABLE (INDCOD nchar(2),DESCOD nvarchar(21),FATOR numeric(19,4),
	 HORAS numeric(19,4),X_HORAS numeric(19,4),NOTURNO numeric(19,4),
	 H_NORMAL numeric(19,4),COMPATI nchar(1));
	 insert into @tabcltcodigo SELECT  INDCOD ,DESCCOD ,FATOR ,HORAS ,X_HORAS ,NOTURNO ,
	 H_NORMAL ,COMPATI from cltcodig;

    -- montagem da "tabela" vet_sabado
	DECLARE @vet_sabado TABLE(oindcod nvarchar(2));
    DECLARE @i int;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( ' '+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 10)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( '.'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( 'C'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
-- numero de dias no mes	
   DECLARE @ndiasmes int = DateDiff(day,@inicio,@fim)+1;

-- Este é o listaDados (numa store procedure) retorna as informações do CLTCAD e FERIAS 
  
   DECLARE @tabelaLista TABLE (CODCAD nvarchar(4),NOMECAD nvarchar(60),DEMI date,
     CATEGORIA nvarchar(30),CBO numeric(19,4),MENSALISTA nchar(1),
	 CONTA1 nchar(10),BANCO1 nchar(3),AGENCIA1 nchar(8),CPF nchar(11),END_RUA nvarchar(50) ,
	   END_CID nvarchar(25) ,END_UF nchar(2),END_CEP nchar(10),
       SETOR nchar(2), salbase numeric(18,4),ult_atual date,
       Glecad NVARCHAR(4),admi date,demitido int,admitido int,diastrab int,
         tsal numeric(18,4), soma_perc_conf numeric(18,4),      ndepend int,emgozo int,irdepend int, salcontrib numeric(18,4),prov_ferias numeric (18,4),
           prov_decimo numeric(18,4), inss numeric(18,4), salfam numeric(18,4));
       insert into @tabelaLista
       EXECUTE  [dbo].[RetornaListaDados] 
          @inicio
          ,@fim
          ,@SalMin;
         
 --XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

-- TRANSFORMA o CLTPONTO ( 1-registro com dias da semana => nova tabela com 1 registro = 1dia )
 -- tabela geral que transforma o cltponto numa tabela individualizada de evento     
    DECLARE @ndia int;
	DECLARE @dia NVARCHAR(4);
	DECLARE @tabelageral TABLE (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)) 
	set @ndia = 0;
	while (@ndia < 7) 
	begin
	    set @dia ='DIA'+CONVERT(nvarchar,(@ndia+1),1);
	    DECLARE @literal as varchar(5120);
	    SET @literal =
	    'select TRAB,SETOR,fazmov,BL,NUM_MOD,CODSER,cltponto.noturno as noturno,Data as Semana,quant,ok,'+
        'DATENAME(weekday, DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA))   as diasemana,'+
       'DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) as dtadia, '+@dia+  ', '+--cdg.indcod,'+
       'cdg.fator as Diarias,cdg.HORAS as Horas_efetivas,cdg.X_HORAS as Horas50,cdg.NOTURNO as Horas100,cdg.H_NORMAL as Horasfe,cdg.COMPATI'+--,cdg.DESCCOD'+
        ' from cltponto LEFT OUTER JOIN CLTCODIG as cdg '+
         'on ( cdg.INDCOD = '+@dia+')  where '+
        '(data between '''+CAST(@primeirasemana as NVARCHAR(12))+''' and '''+CAST(@fim as NVARCHAR(12))+''') and ('+@dia+' <> ''  '') and '+ 
       ' (DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) >='''+ CAST(@inicio as NVARCHAR(12))+''') and '+
        '(DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) <= '''+CAST(@fim as NVARCHAR(12))+''')'; 
        insert into @tabelageral EXEC(@literal);
        set @ndia = @ndia+1;
    end;
	
   --processamneto corresponde ao Gera_Valores da versão Delphi (sem o processamento do ponto)
   DECLARE @tabelaprocess TABLE
   (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  Diarias numeric(18,4),
	  Diariasn numeric(18,4),
	  Horasfe numeric(18,4),
	  Horasfen numeric(18,4),
	  Horas50 numeric(18,4),
	  Horas50n numeric(18,4),
	  Horas100 numeric(18,4),
	  horas_efetivas numeric(18,4),
	  Diarias_adc numeric(18,4),
	 
	  compati nvarchar(1),
	  quant  numeric(18,4), 
	  ok nvarchar(1),
	  Horas_Dom numeric(18,4),  -- horas feriado que cairam num domingo
	  Horas_Domn numeric(18,4) --   
	  );
	  
   
	insert into  @tabelaprocess
	select trab ,setor ,gleba ,bl ,num_mod ,codser ,
	  noturno , 
	  semana , diasemana , dtadia,indcod ,
	  iif(diarias>0,iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiarias,
	 
	 iif(diarias>0 and (noturno='X'),iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiariasn,
	 
	  iif(horasfe>0,horasfe,0) as pHorasfe,
	  
	  iif((horasfe>0) and (noturno='X'),horasfe,0) as pHorasfen,
	  
	  iif(horas50>0,horas50,0) as pHoras50,
	  iif((horas50>0) and (noturno='X'),horas50,0) as pHoras50n,

	  iif(horas100>0,horas100,0) as pHoras100,
	  
	  iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0,iif( ( indcod = 'X'),horas_efetivas/2, horas_efetivas),horas_efetivas),0) * iif((diarias>0) and (diasemana not in ('Saturday','Sunday')), dbo.fn_FatorHorasEfetivas(dtadia), 1) as horas_efetivas,
	  -- diaria_adc
	  iif(not Exists(Select gcodser from @tabelagratif as grat where ((codser = grat.gcodser) and 
	  ((setor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))),0,
	     iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and (rtrim(indcod)='X'), 
	       ( (horas_efetivas/2) *  (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser))),
		(horas_efetivas * (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser)) )),0)) as diaria_adc

	  ,compati,quant,ok ,

	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0),horasfe,0) as pHoras_Dom,
	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0) and (noturno='X'),horasfe,0) as pHoras_Domn

	  from @tabelageral ;

	
	  
  -- Base do Calculo da Folha por TRABALHADOR (agregada) 
      DECLARE @tabelaagregada TABLE (trab nvarchar(4),
	    dias numeric(18,4),
	    diarias numeric(18,4), diarias_n numeric(18,4),
	     horasfe numeric(18,4),horasfen numeric(18,4),
	     horas50 numeric(18,4),horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	      horas_domn numeric(18,4) );  
	  
	  insert into @tabelaagregada
	  select trab,
	  Round(SUM(Diarias)/8,2),
	  SUM(Diarias) as diarias ,
	  SUM(diariasn) as diarias_n,
	  SUM(horasfe) as horasfe, SUM(horasfen) as horasfen,
	  SUM(horas50) as horas50 ,SUM(horas50n) as horas50n ,
	  SUM(horas100) as horas100 ,SUM(horas_efetivas) as horas_efetivas ,
	  SUM(diarias_adc) as diarias_adc ,
	  SUM(iif(indcod='FI',1,0)) as faltas,
	  SUM(iif(indcod='A',1,0)) as doenca,
	  SUM(iif(indcod='FE',1,0)) as diasferias,
	  SUM(horas_dom) as horas_dom, SUM(horas_domn) as horas_domn
	  FROM @tabelaprocess
	    group by trab order by trab

	 
	
	 -- operaçao para calculos agregados do trabalhador
	  -- a folha por trabalhador
	  DECLARE @FOLHA TABLE (   
	      DATA date,
		  SETOR nchar(2),
	      CENTRO nchar(4),
		  QUADRA nchar(3),
		   trab nchar(4),
	      TIPO nchar(1),
		  NUM_MOD nchar(2),
		  CODSER nchar(3),
		  QUANT numeric(19,4), 
		  SALARIO numeric(19,4), --vlrdiarias
		  VLR_HXS numeric(19,4), -- vlrhorasfe
		  VLR_HXA numeric(19,4), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
		  FGTS numeric(19,4),
		   SALFAM numeric(19,4),
		   INSS numeric(19,4),
		   FERIAS numeric(19,4),
		   DECIMO numeric(19,4),
		   FGTS_FE numeric(19,4),
		   FGTS_DEC numeric(19,4),
	      EDUC numeric(19,4),
		  TERC numeric(19,4),
		  EDUC_FE numeric(19,4),
		  EDUC_DEC numeric(19,4),
		  MULTA_FGTS numeric(19,4),
		  HX numeric(19,4),
		  HXS numeric(19,4), -- vlrhorasfe
		  HXA numeric(19,4), -- vlrhoras50
	      HXN numeric(19,4),-- vlrhoras100
	      HEFETIVA numeric(19,4),
	      PONTO nvarchar(62),
	      IRFONTE numeric(19,4),
	      GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  FALTAS numeric(13,0),
		  HORAS_DOM numeric(19,4),
		  HORAS_FER numeric(19,4),
		  VLR_DOM numeric(19,4),
		  VLR_FER numeric(19,4),
		  TXINSS numeric(19,4),
		  IRDEDU numeric(19,4),
		  DIAS numeric(19,4)
	  );
	
	      DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @SALARIO numeric(19,4), --vlrdiarias
		  @VLR_HXS numeric(19,4), -- vlrhorasfe
		  @VLR_HXA numeric(19,4), -- vlrhoras50
	      @VLR_HXN numeric(19,4),-- vlrhoras100
		  @FGTS numeric(19,4),
		  @SALFAM numeric(19,4),
		  @INSS numeric(19,4),
		  @FERIAS numeric(19,4),
		  @DECIMO numeric(19,4),
		  @FGTS_FE numeric(19,4),
		  @FGTS_DEC numeric(19,4),
	      @EDUC numeric(19,4),
		  @TERC numeric(19,4),
		  @EDUC_FE numeric(19,4),
		  @EDUC_DEC numeric(19,4),
		  @MULTA_FGTS numeric(19,4),
		  @HX numeric(19,4),
		  @HXS numeric(19,4), -- vlrhorasfe
		  @HXA numeric(19,4), -- vlrhoras50
	      @HXN numeric(19,4),-- vlrhoras100
	      @HEFETIVA numeric(19,4),
	      @PONTO nvarchar(62),
	      @IRFONTE numeric(19,4),
	      @GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  @FALTAS numeric(13,0),
		  @HORAS_DOM numeric(19,4),
		  @HORAS_FER numeric(19,4),
		  @VLR_DOM numeric(19,4),
		  @VLR_FER numeric(19,4);

	DECLARE @agtrab nvarchar(4),
	    @agdias numeric(18,4),
	    @agdiarias numeric(18,4), @agdiarias_n numeric(18,4),
	     @aghorasfe numeric(18,4),@aghorasfen numeric(18,4),
	     @aghoras50 numeric(18,4),@aghoras50n numeric(18,4),
	     @aghoras100 numeric(18,4),
     	 @aghoras_efetivas numeric(18,4),
	     @agdiarias_adc numeric(18,4),
	     @agfaltas int,@agdoenca int,@agdiasferias int,
		 @aghoras_dom numeric(18,4),@aghoras_fer numeric(18,4),
		 @aghoras_domn numeric(18,4),@aghoras_fern numeric(18,4);

	-- INICIO DO PROCESSAMENTO PARA A FOLHA POR TRABALHADOR	  
	 DECLARE cursor_mst CURSOR FOR   
      SELECT * from @tabelaagregada order by trab;
	 OPEN cursor_mst;
	
	 -- dados do listadados
	 DECLARE @tsal numeric(18,4),
	     @irdepend int,
		 @ndepend int,
		 @emgozo int,
		 @diastrab int,
		 @salcontrib numeric(18,4),--ferias
		 @feriasinss numeric(18,4),
		 @feriassalfam numeric(18,4),
		 @setor nchar(2),
		  @centro nchar(4);
		 --operadores 
	 DECLARE @trab nvarchar(4),
	   @dias numeric(18,4),
	   @vlrdiarias numeric(18,4), 
	   @vlrdiarias_adc numeric(18,4),
	   @vlrhorasfe numeric(18,4),
       @vlrhoras50 numeric(18,4),
	   @vlrdiarias_n numeric(18,4),
	   @vlrhorasfen numeric(18,4),
	   @vlrhoras50n numeric(18,4),
	   @vlrhoras100 numeric(18,4),
	   @vlrhoras_dom numeric(18,4),
	   @vlrhoras_fer numeric(18,4);

	  -- @faltas int,@doenca int,@diasferias int;
	  DECLARE @tsalfam numeric(18,4);
	  DECLARE @insssalcontrib numeric(18,4);


	   FETCH NEXT  FROM cursor_mst INTO
	    @agtrab,
	    @agdias,
	    @agdiarias, @agdiarias_n,
	    @aghorasfe,@aghorasfen,
	    @aghoras50 ,@aghoras50n,
	    @aghoras100,
     	@aghoras_efetivas,
	    @agdiarias_adc,
	    @agfaltas,@agdoenca,@agdiasferias,
		@aghoras_dom,@aghoras_domn;
       DECLARE @totalfolha numeric(19,4) = 0;   
		  
       WHILE @@FETCH_STATUS = 0  
       BEGIN  
		  SET @trab = @agtrab;
		  SET @dias = @agdias
	
	      SELECT @tsal = lst.tsal,@irdepend = lst.irdepend, @ndepend = lst.ndepend,@emgozo = lst.emgozo,
		   @diastrab = lst.diastrab, @salcontrib = lst.salcontrib,@feriassalfam = lst.salfam,@feriasinss = lst.inss,
		   @setor = lst.Setor,@centro = lst.Glecad
		   FROM @tabelaLista as lst where (lst.CODCAD=@trab);
	
		  SET @vlrdiarias = Round(@agdias * Round(@tsal/@ndiasmes, 4),2);
		  -- SET @vlrdiarias_adc =  round((@agdiarias_adc/8) * Round(@tsal/@ndiasmes, 4),2);
		  SET @vlrdiarias_adc =  (@agdiarias_adc/8) * (@tsal/@ndiasmes);
		  -- EC 141/2026: a hora extra e valorizada pelo divisor do DIA em que foi feita.
		  -- So existem duas faixas no mes em que o divisor muda (dez/2026 e dez/2027);
		  -- nos demais meses @ec_..._2 fica zero e a conta e a mesma de sempre.
		  DECLARE @ec_div_ini numeric(3,0), @ec_div_fim numeric(3,0);
		  DECLARE @ec_hfe_1 numeric(18,4), @ec_hfe_2 numeric(18,4), @ec_hfen_1 numeric(18,4), @ec_hfen_2 numeric(18,4);
		  DECLARE @ec_h50_1 numeric(18,4), @ec_h50_2 numeric(18,4), @ec_h50n_1 numeric(18,4), @ec_h50n_2 numeric(18,4);
		  SET @ec_div_ini = dbo.fn_DivisorHorasExtras(@inicio);
		  SET @ec_div_fim = dbo.fn_DivisorHorasExtras(@fim);
		  SET @ec_hfe_2 = 0; SET @ec_hfen_2 = 0; SET @ec_h50_2 = 0; SET @ec_h50n_2 = 0;
		  IF (@ec_div_ini <> @ec_div_fim)
		      SELECT @ec_hfe_2 = ISNULL(SUM(horasfe),0), @ec_hfen_2 = ISNULL(SUM(horasfen),0),
		             @ec_h50_2 = ISNULL(SUM(horas50),0), @ec_h50n_2 = ISNULL(SUM(horas50n),0)
		        FROM @tabelaprocess
		       WHERE trab = @trab AND dbo.fn_DivisorHorasExtras(dtadia) = @ec_div_fim;
		  SET @ec_hfe_1 = @aghorasfe - @ec_hfe_2;  SET @ec_hfen_1 = @aghorasfen - @ec_hfen_2;
		  SET @ec_h50_1 = @aghoras50 - @ec_h50_2;  SET @ec_h50n_1 = @aghoras50n - @ec_h50n_2;
		  SET @vlrhorasfe= iif(@aghorasfe = 0,0,iif(@ec_hfe_2 = 0, Round((@aghorasfe * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfe_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfe_2 * ((@tsal / @ec_div_fim) * 2.0)), 2))); 
		  SET @vlrhoras50 =iif(@aghoras50 = 0,0,iif(@ec_h50_2 = 0, Round((@aghoras50 * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrdiarias_n =iif(@agdiarias_n=0,0,Round((@agdiarias_n/8) * Round(@tsal/@ndiasmes, 4),2));
		  SET @vlrhorasfen = iif(@aghorasfen = 0,0,iif(@ec_hfen_2 = 0, Round((@aghorasfen * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfen_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfen_2 * ((@tsal / @ec_div_fim) * 2.0)), 2)));
		  SET @vlrhoras50n = iif(@aghoras50n = 0,0,iif(@ec_h50n_2 = 0, Round((@aghoras50n * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50n_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50n_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrhoras100 = Round((@vlrdiarias_n+ @vlrhorasfen+ @vlrhoras50n)*0.25,2);
        
		  -- separa horas feriados e domingos
		  --set @aghorasfe = @aghorasfe - @aghorasfen;  
		 -- set @aghoras_dom = @aghoras_dom - @aghoras_domn;

		  SET @aghoras_fer = iif(@aghorasfe = 0,0, @aghorasfe-@aghoras_dom-@aghoras_domn); 
		 -- SET @aghoras_fern = iif(@aghorasfen = 0,0, @aghorasfen-@aghoras_domn); 
		
		  SET @vlrhoras_dom = 0;
		  SET @vlrhoras_fer = 0;
		  DECLARE @vlrcalc numeric (18,4);
		  SET @vlrcalc = @vlrhorasfe;--+@vlrhorasfen;


		  if (@aghoras_dom <> 0) and (@aghoras_fer = 0) 
          begin
		       SET @vlrhoras_dom = @vlrcalc;
		  end;
		  if (@aghoras_dom = 0) and (@aghoras_fer <> 0) 
          begin
		       SET @vlrhoras_fer = @vlrcalc;
		  end;
          if (@aghoras_dom <> 0) and (@aghoras_fer <> 0) 
          begin
             SET @vlrhoras_dom = Round((@aghoras_dom / (@aghorasfe))
                               * @vlrcalc, 2);		
             SET @vlrhoras_fer = @vlrcalc - @vlrhoras_dom;
		  end;
         
		  -- separa horas feriados e domingos pra NOTURNO
		
		 -- if (@aghoras_domn <> 0) and (@aghoras_fern = 0) 
   --       begin
		 --      SET @vlrhoras_dom = @vlrhoras_dom + @vlrhorasfen;
		 -- end;
		 -- if (@aghoras_domn = 0) and (@aghoras_fern <> 0) 
   --       begin
		 --      SET @vlrhoras_fer = @vlrhoras_fer + @vlrhorasfen;
		 -- end;
   --       if (@aghoras_domn <> 0) and (@aghoras_fern <> 0) 
   --       begin
   --          SET @vlrhoras_dom = @vlrhoras_dom + Round((@aghoras_domn / (@aghoras_domn + @aghoras_fern))
   --                            * @vlrhorasfen, 2);		
   --          SET @vlrhoras_fer = @vlrhoras_fer + (@vlrhorasfen - (Round((@aghoras_domn / (@aghoras_domn + @aghoras_fern))
   --                            * @vlrhorasfen, 2)));
		 -- end;
   --      -- Soma Noturno e diario
		 --SET @aghoras_dom = @aghoras_dom + @aghoras_domn;
		 --SET @aghoras_fer = @aghoras_fer + @aghoras_fern;




	  -- preparacao e calculo para a folha com 
          SET @SALARIO = @vlrdiarias;
		  SET @VLR_HXS = @vlrhorasfe;
		  SET @VLR_HXA = @vlrhoras50;
	      SET @VLR_HXN = @vlrhoras100;
		  SET @FGTS = dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc)), 2);
          SET @VLR_DOM = @vlrhoras_dom;
		  SET @VLR_FER = @vlrhoras_fer;
		   
		  SET @SALFAM = 0;
		  SET @INSS = dbo.ValorInss(@TabInss,@fim,@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc);

		  SET @FERIAS =0;
		  SET @DECIMO =0;
		  SET @FGTS_FE =0;
		  SET @FGTS_DEC =0;
	      SET @EDUC =0;
		  SET @TERC =0;
		  SET @EDUC_FE =0;
		  SET @EDUC_DEC =0;
		  
		 
		  SET @MULTA_FGTS =0;
		  SET @HX =@agdiarias;
		  SET @HXS = @aghorasfe;
		  SET @HXA = @aghoras50;
	      SET @HXN = @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; -- as horas noturnas incluem as feriados e extras
	      SET @HEFETIVA = @aghoras_efetivas;
	      SET @PONTO = '';
	      SET @IRFONTE =  dbo.CalcIrFonte(@TabIrFonte,@agtrab,@fim,(@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc),
		    @INSS ,@irdepend);
	      SET @GRATIF = @vlrdiarias_adc;
	 	  SET @FALTAS=@agfaltas;      
		  SET  @tsalfam = 0;
		  
		  if ((@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc) <> 0)
		  begin
		  if (@emgozo = 0) 
          begin
            if (@ndepend > 0) 
            begin
              
			   SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
			           (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc), @ndepend);
              if (@diastrab <> 0) 
              begin   
			     SET @tsalfam  = Round(  (CAST (@diastrab as decimal) / CAST( @ndiasmes as decimal)) * @tsalfam, 2); 
			  end;
            end;
          end
          else
          if (@fim >= '2009-04-01') 
          begin
            
            -- verifica se a faixa de contribuição p/ desc.INSS mudou!
            SET  @insssalcontrib = dbo.ValorInss(@tabinss, @fim,  @salcontrib + 
			              @vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc);
            if (@insssalcontrib >= (@feriasinss + @INSS)) 
            begin -- analise a diferença à maior
                SET @insssalcontrib = @insssalcontrib - (@feriasinss + @INSS);
                if ( @insssalcontrib > 0.02)  -- se a dif.for maior q 0,02
                 begin
                   SET @INSS = @INSS + @insssalcontrib;
                 end;
            end;
      -- calcula se o salario familia mudou de faixa em função do Salario Contrib. (ferias+salario)
            if (@ndepend > 0) 
            begin
			   SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
			           (@salcontrib+@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc), @ndepend);
		       if (@tsalfam > @feriassalfam) 
               begin --diferença a mais pago no SALFAM . OQUE FAZER???
                  SET @tsalfam = (@tsalfam - @feriassalfam);
               end
               else
                 SET  @tsalfam = 0;
            end;
            --SET @FERIAS = @feriasProv_Ferias;
          end;
		  end;

		  SET @SALFAM = @tsalfam;

		  SET @totalfolha = (@totalfolha +@SALARIO+@VLR_HXS+ @VLR_HXA+ @VLR_HXN+@GRATIF);

		  Insert into @FOLHA  (trab ,SETOR,CENTRO,TIPO,
		  SALARIO,VLR_HXS, VLR_HXA, VLR_HXN,FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
		   FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,MULTA_FGTS,HX,HXS,HXA, HXN,HEFETIVA,PONTO,IRFONTE,
	      GRATIF,FALTAS,HORAS_DOM,HORAS_FER,VLR_DOM,VLR_FER, DIAS) VALUES 	
	      (@trab , @setor ,@centro,'D',
		  @SALARIO,@VLR_HXS, @VLR_HXA, @VLR_HXN,@FGTS,@SALFAM,@INSS,@FERIAS,@DECIMO,@FGTS_FE,
		   @FGTS_DEC,@EDUC,@TERC,@EDUC_FE,@EDUC_DEC,@MULTA_FGTS,@HX,@HXS,@HXA, @HXN,@HEFETIVA,@PONTO,@IRFONTE,
	      @GRATIF,@FALTAS,@aghoras_dom,@aghoras_fer,@VLR_DOM,@VLR_FER, @dias)
		  -- @dias
	      FETCH NEXT  FROM cursor_mst INTO
		  @agtrab,
	      @agdias,
	      @agdiarias, @agdiarias_n,
	      @aghorasfe,@aghorasfen,
	      @aghoras50 ,@aghoras50n,
	      @aghoras100,
     	  @aghoras_efetivas,
	      @agdiarias_adc,
	      @agfaltas,@agdoenca,@agdiasferias,@aghoras_dom,@aghoras_fer;
	   END;
	   CLOSE cursor_mst;  
       DEALLOCATE cursor_mst;  

	   -- rotina de salario_educação e terceiros
	  DECLARE @teduc numeric(19,4) ,@tterc numeric(19,4);
	  DECLARE @reduc numeric(19,4) ,@rterc numeric(19,4); 
	  SET @teduc = Round(@totalfolha * 0.025, 2);
      SET @tterc = Round(@totalfolha * 0.002, 2);
      UPDATE @FOLHA SET EDUC = Round(((SALARIO+VLR_HXS+ VLR_HXA+ VLR_HXN+GRATIF)/@totalfolha) * @teduc,2),
    	  TERC = Round(((SALARIO+VLR_HXS+ VLR_HXA+ VLR_HXN+GRATIF)/@totalfolha) * @tterc,2);
      SELECT @reduc = SUM(EDUC), @rterc = SUM(TERC) FROM @folha;
	  SET @reduc = @teduc - @reduc;
	  SET @rterc = @tterc - @rterc;
	  DECLARE @maior nvarchar(4);
	  if (@reduc <> 0) 
	  begin
	      SELECT TOP(1) @maior=TRAB FROM @FOLHA ORDER BY EDUC DESC; 
	      UPDATE @FOLHA  SET EDUC = (EDUC + @reduc) WHERE 
		   TRAB =  @maior;
	  end;	  
      if (@rterc <> 0) 
	  begin
	      SELECT TOP(1) @maior=TRAB FROM @FOLHA ORDER BY TERC DESC; 
	      UPDATE @FOLHA  SET TERC = (TERC + @terc) WHERE 
		   TRAB =  @maior;
	  end;	  
      

	     -- Rotina do ponto
	  --DECLARE @tabelaPonto TABLE(trab nvarchar(4), PONTO nvarchar(62));
	 --cursor      
	  DECLARE @pttrab nvarchar(4),@ptdtadia date,@ptindcod nvarchar(2),@ptdiarias numeric(14,2);
	 --trabalho pra logica
	  DECLARE @oponto nvarchar(62),@otrab nvarchar(4), @oacumdiarias numeric(14,2),@odtadia date,@oindcod nvarchar(2),
	  @oindcod2 nvarchar(2);
	  
	  DECLARE cursor_ponto CURSOR FOR  
	   SELECT trab,dtadia,indcod,
	    Diarias as diarias
	    FROM @tabelaprocess
	       order by trab,dtadia;
	 
	  OPEN cursor_ponto;
	  FETCH NEXT  FROM cursor_ponto INTO
	      @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;
      WHILE  @@FETCH_STATUS = 0  
       BEGIN  
		 SET @oponto = ''; 
		 DECLARE @ndias int = DateDiff(day,@inicio, @ptdtadia);
		 if (@ndias > 0) 
		 begin
		   SET @oponto = REPLICATE('  ',@ndias);
		 end;
		 SET @otrab = @pttrab; 
        /* if (@otrab = '8991')
		 begin
	    	 SET @otrab = @pttrab;
		 end;*/		
		 DECLARE @auxtabela TABLE (indcod nvarchar(2),tabdiaria numeric(14,4));
		 
		 WHILE (@@FETCH_STATUS = 0)  and (@otrab = @pttrab)
         BEGIN
		    SET @odtadia = @ptdtadia;
		    SET @oacumdiarias = @ptdiarias; 
	        SET @oindcod = @ptindcod;
			
			FETCH NEXT  FROM cursor_ponto INTO
	            @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;      
		    if (@odtadia <> @ptdtadia)
			begin
			    SET @oponto = @oponto +  @oindcod;
				continue; 
			 end;
			 insert into @auxtabela (indcod,tabdiaria) VALUES(@ptindcod, @ptdiarias);
			 WHILE (@@FETCH_STATUS = 0)  and (@otrab = @pttrab) and (@odtadia = @ptdtadia)
             BEGIN  
			     insert into @auxtabela (indcod,tabdiaria) VALUES(@ptindcod, @ptdiarias)
				 SET @oacumdiarias = @oacumdiarias + @ptdiarias;
				 FETCH NEXT  FROM cursor_ponto INTO
	              @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;      
		     END;
			 if (@oacumdiarias <> 0)   
             begin   
			   if (@oacumdiarias >= 7.9) and (@oacumdiarias <=8.1)   
               begin
			      if  Exists(select * from @auxtabela where (Substring(indcod,1,1)='Y') )
				      SET @oponto = @oponto + 'Y6';
                  else
			      begin
				    if  Exists(select * from @auxtabela where (Substring(indcod,1,1)='R') )
				        SET @oponto = @oponto + 'R ';
					else
						SET @oponto = @oponto + 'X ';
				  end;
               end
               else
               begin
			       SELECT @oindcod2 = INDCOD from @tabcltcodigo where (ROUND(fator,3) = ROUND(@oacumdiarias,3));
					if (@oindcod2 = null)  
					begin
					   if (@oacumdiarias>8)
			              SET @oponto =@oponto+'X+';
					   else
					      SET @oponto =@oponto+'? ';
					end
					else
					   SET @oponto =@oponto+'!!';
			   end
			end
			else
			begin
			  select top(1) @oindcod2 = INDCOD from @auxtabela where (Substring(indcod,1,1)<>'R')
			  if ( @oponto  <> null)
			     SET @oponto = @oponto +  @oindcod2
			  else
			     SET @oponto = @oponto +  '  ';
			end;
			delete  @auxtabela;
    	 END    
         --insert into @tabelaPonto (trab,ponto) VALUES(@otrab,@oponto);  		
		 UPDATE @FOLHA SET PONTO = @oponto where (TRAB = @otrab);
       END
	   CLOSE cursor_ponto;  
       DEALLOCATE cursor_ponto;  
  
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX CASO DESEJE PROCESSAR O CUSTO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
if (@custo = 1)
BEGIN
       DECLARE @CUSTOFOLHA TABLE (   
		  [Id] [int] IDENTITY(1,1) NOT NULL,
		  INICIO date,
		  FIM date,
		  SETOR nchar(2),
	      CENTRO nchar(4),
		  QUADRA nchar(3),
		  TRAB nchar(4),
	      OK nchar(1),
		  NUM_MOD nchar(2),
		  CODSER nchar(3),
		  QUANT numeric(19,4), 
		  SALARIO numeric(19,6), --vlrdiarias
		  VLR_HXS numeric(19,6), -- vlrhorasfe
		  VLR_HXA numeric(19,6), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
		  FGTS numeric(19,6),
		  SALFAM numeric(19,6),
		  INSS numeric(19,6),
		  FERIAS numeric(19,6),
		  DECIMO numeric(19,6),
		  FGTS_FE numeric(19,6),
		  FGTS_DEC numeric(19,6),
	      EDUC numeric(19,6),
		  TERC numeric(19,6),
		  EDUC_FE numeric(19,6),
		  EDUC_DEC numeric(19,6),
		  MULTA_FGTS numeric(19,6),
		  HX numeric(19,4),
		  HXS numeric(19,4), -- vlrhorasfe
		  HXA numeric(19,4), -- vlrhoras50
	      HXN numeric(19,4),-- vlrhoras100
	      HEFETIVA numeric(19,4),
	      IRFONTE numeric(19,4),
	      GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  FALTAS numeric(13,0),
		  DOENCA numeric(13,0)	
	  );
	   DECLARE 
		  @agsetor NVARCHAR(2),
		  @agcentro NVARCHAR(4),
		  @agquadra NVARCHAR(3),
		  @agnum_mod NVARCHAR(2),
	      @agcodser NVARCHAR(3),
		  @agdtadia date,
		  @agok NVARCHAR(1),
		  @agindcod NVARCHAR(2),
		  @agquant numeric(19,4),
	      
		  @ptsetor NVARCHAR(2),
		  @ptcentro NVARCHAR(4),
		  @ptquadra NVARCHAR(3),
		  @ptnum_mod NVARCHAR(2),
	      @ptcodser NVARCHAR(3),
		  @diainicio date,
		  @diafim date,
		  @ptok nchar(1),
		  @ptquant numeric(19,4),
		  @ptHX numeric(19,4),
		  @ptHXS numeric(19,4), -- vlrhorasfe
		  @ptHXA numeric(19,4), -- vlrhoras50
	      @ptHXN numeric(19,4),-- vlrhoras100
	      @ptHEFETIVA numeric(19,4),
		  @ptDiarias_adc numeric(19,4),
	      @ptfaltas int,
		  @ptdoenca int;

	     ;
	     
	 	 
	-- Não irei trabalhar com agregados 
     DECLARE cursor_custo CURSOR FOR   
      SELECT trab, 
	 setor ,gleba ,bl ,num_mod ,codser ,
	    dtadia ,indcod,ok, Diarias , Diariasn, Horasfe,
	  Horasfen, Horas50, Horas50n, Horas100, horas_efetivas,
	  Diarias_adc,quant from @tabelaprocess order by trab,setor,gleba,bl,num_mod,codser,dtadia;
	   OPEN cursor_custo;
	   FETCH NEXT  FROM cursor_custo INTO
		  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		  @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas,@agdiarias_adc,
		  @agquant;
	 
	 WHILE @@FETCH_STATUS = 0  
     BEGIN  
	     SET @pttrab =  @agtrab;
	     SELECT @SALARIO = SALARIO,@VLR_HXS =VLR_HXS , @VLR_HXA = VLR_HXA, @VLR_HXN = VLR_HXN, @FGTS = FGTS,  @SALFAM =SALFAM, @INSS=INSS,
		  @FERIAS=FERIAS, @DECIMO=DECIMO, @FGTS_FE = FGTS_FE , @FGTS_DEC = FGTS_DEC, @EDUC = EDUC, @TERC = TERC ,
		  @EDUC_FE = EDUC_FE, @EDUC_DEC = EDUC_DEC, @HX = HX, @HXS = HXS, @HXA=HXA,  @HXN=HXN, @HEFETIVA=HEFETIVA,
	      @IRFONTE=IRFONTE, @GRATIF=GRATIF FROM @FOLHA WHERE TRAB=@agtrab; 	 
	      -- valor da hora na diaria normal
		  SET @vlrDiarias = iif(@HX<>0,(@SALARIO/@HX),0);
		  -- valor da hora na horaextra
		  SET @vlrhoras50 = iif(@HXS<>0,(@VLR_HXS/@HXS),0);
		  -- valor da hora na hora trabalhada em feriado
		  SET @vlrhorasfe = iif(@HXA<>0,(@VLR_HXA/@HXA),0);
		  -- vlr hora do adcional noturno
		  SET @vlrhoras100 = iif(@HXN<>0,(@VLR_HXN/@HXN),0);
		  
		  WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab)
          BEGIN  
		    SET @ptsetor =   @agsetor;
	    	SET @ptcentro = @agcentro;
		    SET @ptquadra = @agquadra;
		    SET @ptnum_mod = @agnum_mod;
	        SET @ptcodser = @agcodser;
	        SET @ptHX =@agdiarias;
		    SET @ptHXS = @aghorasfe;
		    SET @ptHXA = @aghoras50;
	        SET @ptHXN = @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; 
	        SET @ptHEFETIVA = @aghoras_efetivas;
	        SET @ptdiarias_adc = @agdiarias_adc;-- diarias de gratificacao
			SET @diainicio = @agdtadia;
			SET @diafim = @agdtadia;
			SET @ptok =@agok;
			SET @ptquant =@agquant;
			SET @ptDiarias_adc = 0;
			SET @ptfaltas = iif(@agindcod='FI',1,0);
			SET @ptdoenca = iif(@agindcod='A',1,0);

			FETCH NEXT  FROM cursor_custo INTO
	    	  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		      @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas, @agdiarias_adc,@agquant;
			WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab) and 
			   (@ptsetor= @agsetor) and  (@ptcentro = @agcentro) and (@ptquadra= @agquadra) and
			  (  @ptnum_mod= @agnum_mod) and  (@ptcodser = @agcodser)
            BEGIN
			  SET @ptHX =@ptHX+ @agdiarias;
		      SET @ptHXS =@ptHXS+ @aghorasfe;
		      SET @ptHXA =@ptHXA+ @aghoras50;
	          SET @ptHXN =@ptHXN + @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; 
	          SET @ptHEFETIVA = @ptHEFETIVA+@aghoras_efetivas;
	          SET @ptdiarias_adc = @ptdiarias_adc + @agdiarias_adc;
			  SET @diafim = @agdtadia;
			  if (@ptok='')
			     SET @ptok =@agok;
			  SET @ptquant = @ptquant +@agquant;
			  SET @ptfaltas = @ptfaltas + iif(@agindcod='FI',1,0);
			  SET @ptdoenca = @ptdoenca + iif(@agindcod='A',1,0);
			  FETCH NEXT  FROM cursor_custo INTO
	    	  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		      @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas, @agdiarias_adc,@agquant;
		
		    END;
          -- verificar se houve gratificação 
		    if Exists(Select gcodser from @tabelagratif as grat where ((@ptcodser = grat.gcodser) and 
      	      ((@ptsetor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))) and (@ptHEFETIVA<>0)
            begin
		        SET @ptDiarias_adc =  @ptHEFETIVA *
			      (Select grat.perc_diaria/100 from @tabelagratif as grat where (@ptcodser = grat.gcodser)) * @vlrDiarias;
		    end;

	        INSERT INTO @CUSTOFOLHA (INICIO ,FIM ,SETOR,CENTRO,QUADRA,TRAB, OK,
		         NUM_MOD,CODSER,QUANT,SALARIO,VLR_HXS,VLR_HXA,VLR_HXN, HX,HXS,
		        HXA,HXN, HEFETIVA,  GRATIF, FALTAS,DOENCA)
		      VALUES (@diainicio,@diafim, @ptsetor, @ptcentro,@ptquadra,@pttrab,@ptok,
		         @ptnum_mod,@ptcodser,@ptquant,ROUND(@ptHX * @vlrDiarias,2), ROUND(@ptHXS * @vlrhoras50,2),
			     ROUND(@ptHXA * @vlrhorasfe,2), ROUND(@ptHXN * @vlrhoras100,2),@ptHX,@ptHXS,
			     @ptHXA,@ptHXN, @ptHEFETIVA,@ptDiarias_adc,@ptfaltas,@ptdoenca);
     	 END;
	 END;	 
	 CLOSE cursor_custo;  
     DEALLOCATE cursor_custo;  
	 -- alocar as diferenças de calculo
	  DECLARE @difsal numeric(19,4),@difhxs numeric(19,4),@difhxa numeric(19,4),@difhxn numeric(19,4),
	  @difgratif numeric(19,4);
	  DECLARE @tabDIF TABLE (trab nvarchar(4),difsal numeric(19,4),difhxs numeric(19,4),difhxa numeric(19,4),
	  difhxn numeric(19,4),	difgratif numeric(19,4));
	  
	  
	  INSERT INTO @tabDIF
	  SELECT TRAB, SALARIO- (SELECT SUM(cus.SALARIO) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsal ,
	  VLR_HXS- (SELECT SUM(cus.VLR_HXS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxs,
	  VLR_HXA- (SELECT SUM(cus.VLR_HXA) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxa,
	  VLR_HXN- (SELECT SUM(cus.VLR_HXN) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxn,
	  GRATIF- (SELECT SUM(cus.GRATIF) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difgratif
	   FROM @FOLHA as fol   order by fol.trab;
      
	  DECLARE cursor_rateio CURSOR FOR   
      SELECT trab, difsal,difhxs,difhxa,
	  difhxn,	difgratif from @tabdif order by trab;
	  OPEN cursor_rateio;
	    FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsal,@difhxs,@difhxa,
        	  @difhxn,	@difgratif;
     WHILE (@@FETCH_STATUS = 0)
	 begin
	    if (@difsal <> 0) 
           UPDATE @CUSTOFOLHA SET SALARIO = (SALARIO + @difsal)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.SALARIO DESC);
		if (@difhxs <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXS = (VLR_HXS + @difhxs)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXS DESC);
		if (@difhxa <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXA = (VLR_HXA + @difhxa)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXA DESC);
		if (@difhxn <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXN = (VLR_HXN + @difhxn)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXN DESC);
		if (@difgratif <> 0) 
           UPDATE @CUSTOFOLHA SET GRATIF = GRATIF + @difgratif  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.GRATIF DESC);

		FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsal,@difhxs,@difhxa,
        	  @difhxn,	@difgratif;
	 end;
	 CLOSE cursor_rateio;  
     DEALLOCATE cursor_rateio;  

	 --parcelas de OBRIGAÇÔES que serão rateadas pelo valor da folha
--	   SALARIO,VLR_HXN,GRATIF,VLR_HXS,VLR_HXA,  FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
	--	   FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,IRFONTE,

	DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @agSALARIO numeric(19,4), --vlrdiarias
		  @agVLR_HXS numeric(19,4), -- vlrhorasfe
		  @agVLR_HXA numeric(19,4), -- vlrhoras50
	      @agVLR_HXN numeric(19,4),-- vlrhoras100
		  @agGRATIF numeric(19,4),-- vlrdiarias_adc
		  @agFGTS numeric(19,4),
		  @agSALFAM numeric(19,4),
		  @agINSS numeric(19,4),
		  @agFERIAS numeric(19,4),
		  @agDECIMO numeric(19,4),
		  @agFGTS_FE numeric(19,4),
		  @agFGTS_DEC numeric(19,4),
	      @agEDUC numeric(19,4),
		  @agTERC numeric(19,4),
		  @agEDUC_FE numeric(19,4),
		  @agEDUC_DEC numeric(19,4),
		    @agIRFONTE numeric(19,4);
		
		  DECLARE @ptsalario  numeric(19,4),
		  @fator numeric(19,4),
		  @ptFGTS numeric(19,4),
		  @ptSALFAM numeric(19,4),
		  @ptINSS numeric(19,4),
		  @ptFERIAS numeric(19,4),
		  @ptDECIMO numeric(19,4),
		  @ptFGTS_FE numeric(19,4),
		  @ptFGTS_DEC numeric(19,4),
	      @ptEDUC numeric(19,4),
		  @ptTERC numeric(19,4),
		  @ptEDUC_FE numeric(19,4),
		  @ptEDUC_DEC numeric(19,4),
          @ptIRFONTE numeric(19,4),
		  @ptid integer,
		  @agid integer; 	  
   
	  	 

	DECLARE cursor_obrigacao CURSOR FOR   
      SELECT trab,id,SALARIO,VLR_HXN,GRATIF,VLR_HXS,VLR_HXA,  FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
	     FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,IRFONTE from @CUSTOFOLHA order by trab;
	  OPEN cursor_obrigacao;
	    FETCH NEXT  FROM cursor_obrigacao INTO
	       @agtrab, @agid,@agSALARIO,@agVLR_HXN,@agGRATIF,@agVLR_HXS,@agVLR_HXA,  @agFGTS,@agSALFAM,@agINSS,@agFERIAS,@agDECIMO,@agFGTS_FE,
	     @agFGTS_DEC,@agEDUC,@agTERC,@agEDUC_FE,@agEDUC_DEC,@agIRFONTE;
     WHILE (@@FETCH_STATUS = 0)
	 begin
	     SET @pttrab =  @agtrab;
		 
	     SELECT @SALARIO = (SALARIO+VLR_HXS +VLR_HXA+ VLR_HXN+GRATIF), @FGTS = FGTS,  @SALFAM =SALFAM, @INSS=INSS,
		  @FERIAS=FERIAS, @DECIMO=DECIMO, @FGTS_FE = FGTS_FE , @FGTS_DEC = FGTS_DEC, @EDUC = EDUC, @TERC = TERC ,
		  @EDUC_FE = EDUC_FE, @EDUC_DEC = EDUC_DEC,
	      @IRFONTE=IRFONTE FROM @FOLHA WHERE TRAB=@pttrab; 	 
	     
		 --SELECT @ptdiarias = SUM(SALARIO+VLR_HXS+VLR_HXA+VLR_HXN+GRATIF) FROM @CUSTOFOLHA WHERE TRAB=@agtrab;
	    -- SET 
		 WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab)
         BEGIN  
		 
		     SET @ptsalario = @agSALARIO+@agVLR_HXN+@agGRATIF+@agVLR_HXS+@agVLR_HXA;
			 if (@SALARIO <> 0) and (@ptsalario<>0)
			 begin
		        SET @ptid = @agid; 
			    SET @fator =  @ptsalario/@SALARIO;
			    SET @ptfgts = iif(@FGTS<>0,ROUND(@fator*@FGTS,2),0); 	    
				SET @ptsalfam = iif(@SALFAM<>0, ROUND(@fator*@SALFAM,2),0); 	    
				SET @ptirfonte = iif(@IRFONTE<>0, ROUND(@fator*@IRFONTE,2),0); 	    
				SET @ptinss = iif(@INSS<>0,ROUND(@fator*@INSS,2),0); 	    
				SET @ptEDUC = iif(@EDUC<>0,ROUND(@fator*@EDUC,2),0); 	    
				SET @ptTERC = iif(@TERC<>0,ROUND(@fator*@TERC,2),0); 	    
				UPDATE @CUSTOFOLHA SET FGTS=iif(FGTS is null,0,FGTS) +@ptfgts,SALFAM=iif(SALFAM is null,0,SALFAM)+@ptsalfam,
				  IRFONTE = iif(IRFONTE is null,0,IRFONTE)+@ptIRFONTE,
				         INSS =iif(INSS is null,0,INSS)+ @ptINSS,EDUC=iif(EDUC is null,0,EDUC)+@ptEDUC, 
						 TERC =iif(TERC is null,0,IRFONTE)+ @ptTERC 
						 where (id = @ptid);  
			 end;
			  FETCH NEXT  FROM cursor_obrigacao INTO
	         @agtrab, @agid,@agSALARIO,@agVLR_HXN,@agGRATIF,@agVLR_HXS,@agVLR_HXA,  @agFGTS,@agSALFAM,@agINSS,@agFERIAS,@agDECIMO,@agFGTS_FE,
	        @agFGTS_DEC,@agEDUC,@agTERC,@agEDUC_FE,@agEDUC_DEC,@agIRFONTE;

		 END;


	 end;
	 CLOSE cursor_obrigacao;  
     DEALLOCATE cursor_obrigacao;  

      
      DECLARE @difsalfam numeric(19,4),@difinss numeric(19,4),@difirfonte numeric(19,4),@diffgts numeric(19,4),
	  @difeduc numeric(19,4),@difterc numeric(19,4);
	  DECLARE @tabObrigDif TABLE (trab nvarchar(4),difsalfam numeric(19,4),difinss numeric(19,4),difirfonte numeric(19,4),
	  diffgts numeric(19,4),	difeduc numeric(19,4),difterc numeric(19,4));
	  
	  
	  INSERT INTO @tabObrigDif

	 SELECT  TRAB, SALFAM- (SELECT SUM(cus.SALFAM) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsalfam ,
	  INSS- (SELECT SUM(cus.INSS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difINSS,
	  IRFONTE- (SELECT SUM(cus.IRFONTE) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_IRFONTE,
	  FGTS- (SELECT SUM(cus.FGTS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS,
	  EDUC- (SELECT SUM(cus.EDUC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC,
	  TERC- (SELECT SUM(cus.TERC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difTERC
	   FROM @FOLHA as fol   order by fol.trab;

	  
	  DECLARE cursor_rateio CURSOR FOR   
      SELECT trab, difsalfam,difinss,difirfonte,
	  diffgts,	difeduc ,	difterc from @tabObrigDif order by trab;
	  OPEN cursor_rateio;
	    FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsalfam,@difinss,@difirfonte,
        	  @diffgts,	@difeduc,@difterc;
     WHILE (@@FETCH_STATUS = 0)
	 begin

		if (@difsalfam <> 0) 
           UPDATE @CUSTOFOLHA SET SALFAM = (SALFAm + @difsalfam)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.SALFAM DESC);
		if (@difinss <> 0) 
           UPDATE @CUSTOFOLHA SET INSS = (INSS + @difinss)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.INSS DESC);
		if (@difirfonte <> 0) 
           UPDATE @CUSTOFOLHA SET IRFONTE = (IRFONTE + @difirfonte)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.IRFONTE DESC);
		if (@diffgts <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS = (FGTS + @diffgts)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.FGTS DESC);
		if (@difeduc <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC = EDUC + @difeduc  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.EDUC DESC);
		if (@difterc <> 0) 
           UPDATE @CUSTOFOLHA SET TERC = TERC + @difterc  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

		FETCH NEXT  FROM cursor_rateio INTO
		  @trab, @difsalfam,@difinss,@difirfonte,
        	  @diffgts,	@difeduc,@difterc;
	       
	 end;
	 CLOSE cursor_rateio;  
     DEALLOCATE cursor_rateio;  
END;
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXX  FIM  PROCESSAMENTO CUSTO  XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX


  IF (@naograve = 0)
  BEGIN    
      Delete CLTFOLHA where (data = @fim);
      insert into cltfolha 
	  select @fim ,setor,centro, 
	      quadra, TRAB, tipo, 
	      num_mod,codser, quant,
		  SALARIO, VLR_HXS,  VLR_HXA,  VLR_HXN, FGTS, SALFAM,  INSS,
		   FERIAS, DECIMO, FGTS_FE,FGTS_DEC, EDUC,TERC, EDUC_FE,EDUC_DEC,
		  MULTA_FGTS,HX,HXS, HXA, HXN,HEFETIVA,PONTO,
	      IRFONTE,GRATIF,FALTAS	from @FOLHA ; 


		  --DELETE FROM CUSFOLHA 
    --      insert into CUSFOLHA
		  --SELECT INICIO, FIM,SETOR ,CENTRO, QUADRA, TRAB , OK , NUM_MOD , CODSER , QUANT , 
		  --SALARIO , VLR_HXS , VLR_HXA , VLR_HXN , FGTS ,SALFAM , INSS , FERIAS ,DECIMO ,
		  --FGTS_FE ,FGTS_DEC , EDUC ,TERC ,EDUC_FE , EDUC_DEC , MULTA_FGTS , HX , HXS , HXA , 
	   --   HXN ,HEFETIVA , IRFONTE , GRATIF ,FALTAS ,DOENCA 	FROM @CUSTOFOLHA ;
	  

  END;
DECLARE @TABDIFERENCA  TABLE (   
		  TRAB nchar(4),
		  NOME nvarchar(50),
		  SALARIO numeric(19,6), --vlrdiarias
		  VLR_HXS numeric(19,6), -- vlrhorasfe
		  VLR_HXA numeric(19,6), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
	   	  GRATIF numeric(19,4),
		  
		  SALFAM numeric(19,6),
		  INSS numeric(19,6),
		  IRFONTE numeric(19,6),
		  FGTS numeric(19,6),
		  EDUC numeric(19,6),
		  TERC numeric(19,6)
	  );
	  
DECLARE @numreg int;
SELECT @numreg =Count(ID) FROM CLTFOLHA WHERE (DATA = @Fim) 
IF (@numreg>0)
BEGIN
    DECLARE @CLTFOLHA  TABLE (   
		  TRAB nchar(4),
		  SALARIO numeric(19,6), --vlrdiarias
		  VLR_HXS numeric(19,6), -- vlrhorasfe
		  VLR_HXA numeric(19,6), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
	   	  GRATIF numeric(19,4),
		  
		  SALFAM numeric(19,6),
		  INSS numeric(19,6),
		  IRFONTE numeric(19,6),
		  FGTS numeric(19,6),
		  EDUC numeric(19,6),
		  TERC numeric(19,6)
	  );

	  INSERT INTO @CLTFOLHA 
	  SELECT TRAB,SUM(SALARIO),SUM(VLR_HXS),SUM(VLR_HXA),SUM(VLR_HXN),SUM(GRATIF),
	  SUM(SALFAM),SUM(INSS),SUM(IRFONTE),SUM(FGTS),SUM(EDUC),SUM(TERC)  FROM CLTFOLHA
	  WHERE (DATA = @fim) GROUP BY TRAB;  
	 --INSERT INTO  @TABDIFERENCA
	 --SELECT  TRAB, 
	 --  SALARIO - (cus.SALARIO from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB) ) as difSALARIO , 
	 --  VLR_HXS - (SELECT SUM(cus.VLR_HXS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difHORAS , 
	 --  VLR_HXA - (SELECT SUM(cus.VLR_HXA) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difHORASFERIADOS , 
	 --  VLR_HXN - (SELECT SUM(cus.VLR_HXN) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difHORASNOTURNO , 
	 --  GRATIF - (SELECT SUM(cus.GRATIF) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difGRATIF ,   
	 --  SALFAM- (SELECT SUM(cus.SALFAM) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsalfam ,
	 --  INSS- (SELECT SUM(cus.INSS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difINSS,
	 --  IRFONTE- (SELECT SUM(cus.IRFONTE) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_IRFONTE,
	 --  FGTS- (SELECT SUM(cus.FGTS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS,
	 --  EDUC- (SELECT SUM(cus.EDUC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC,
	 --  TERC- (SELECT SUM(cus.TERC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difTERC
	 --  FROM @FOLHA as fol  order by fol.trab ;
   
   INSERT INTO  @TABDIFERENCA
	 SELECT  TRAB as COD, lst.NOMECAD as NOME,
	   SALARIO - (SELECT SALARIO from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB) ) as difSALARIO , 
	   VLR_HXS - (SELECT cus.VLR_HXS from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difHORAS , 
	   VLR_HXA - (SELECT cus.VLR_HXA from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difHORASFERIADOS , 
	   VLR_HXN - (SELECT cus.VLR_HXN from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difHORASNOTURNO , 
	   GRATIF - (SELECT cus.GRATIF from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difGRATIF ,   
	   fol.SALFAM- (SELECT cus.SALFAM from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsalfam ,
	   fol.INSS- (SELECT cus.INSS from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difINSS,
	   IRFONTE- (SELECT cus.IRFONTE from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_IRFONTE,
	   FGTS- (SELECT cus.FGTS from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS,
	   EDUC- (SELECT cus.EDUC from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC,
	   TERC- (SELECT cus.TERC from @CLTFOLHA as cus where (cus.TRAB = fol.TRAB)) as difTERC
	   FROM @FOLHA as fol LEFT OUTER JOIN @tabelaLista as lst 
          on ( lst.CODCAD = TRAB) order by fol.trab ;
   

END;
SELECT * FROM @TABDIFERENCA WHERE ((SALARIO +VLR_HXS +VLR_HXA+VLR_HXN+GRATIF+SALFAM+INSS+IRFONTE+FGTS+EDUC+TERC) <> 0)

-- folha virtual
  --('SElect CODCAD as COD ,NOMECAD as Nome,SETOR,glecad AS centro,CATEGORIA,CBO,'
   -- + 'MENSALISTA,conta1,banco1,agencia1,cpf,end_rua,end_cid,end_uf,end_cep,SALBASE   from cltcad where id = 0',
  
  SELECT   TRAB as COD, lst.NOMECAD as NOME,   flt.setor, st.DESCRI ,centro, 
           CATEGORIA,CBO,
           MENSALISTA,conta1,banco1,agencia1,cpf,end_rua,end_cid,end_uf,end_cep,SALBASE,
	       -- tipo, 
	     -- num_mod,codser, quant,
		  ROW_NUMBER() OVER(ORDER BY TRAB ASC) as ID,   
		  SALARIO,@fim as DATA,ADMI, DEMI, 
		  flt.INSS, 0.0 TXINSS, 0.0 as BASEIRF,
		  IRFONTE,flt.SALFAM,EDUC,TERC,
		  FGTS,DECIMO,FERIAS,
		  VLR_HXA,HX,VLR_HXN,HXA, HXN,
		  VLR_HXS, 0.0 as SALLIQ, GRATIF,  
		  0.0 as ISIND,0.0 as ADIANT,PONTO,      
          lst.tsal as SALARIOATUAL, lst.soma_perc_conf as PERCCONF,  		
		  HORAS_DOM,HORAS_FER,VLR_DOM,VLR_FER,
		  -- estes itens não constamm
		  FGTS_FE,FGTS_DEC,  EDUC_FE,EDUC_DEC,
		  MULTA_FGTS,HXS, HEFETIVA,
	     FALTAS, DIAS	
		 
		 from @FOLHA as flt  LEFT OUTER JOIN @tabelaLista as lst 
          on ( lst.CODCAD = TRAB) LEFT OUTER JOIN SETORES as st 
          on ( st.COD = flt.setor);

  --SELECT INICIO, FIM,SETOR ,CENTRO, QUADRA, TRAB , OK , NUM_MOD , CODSER , QUANT , 
		--  SALARIO , VLR_HXS , VLR_HXA , VLR_HXN , FGTS ,SALFAM , INSS , FERIAS ,DECIMO ,
		--  FGTS_FE ,FGTS_DEC , EDUC ,TERC ,EDUC_FE , EDUC_DEC , MULTA_FGTS , HX , HXS , HXA , 
	 --     HXN ,HEFETIVA , IRFONTE , GRATIF ,FALTAS ,DOENCA 	FROM @CUSTOFOLHA ;

END
--// acrescentei ID para permitir a exclusao...
--  with folhavirtual.FieldDefs.AddFieldDef do
--    begin
--      Name := 'ID';
--      DataType := ftInteger;
--    end;

--  with folhavirtual.FieldDefs.AddFieldDef do
--    begin
--      Name := 'ISIND';
--      DataType := ftBCD;
--    end;

--  with folhavirtual.FieldDefs.AddFieldDef do
--    begin
--      Name := 'ADIANT';
--      DataType := ftBCD;
--    end;
--  with folhavirtual.FieldDefs.AddFieldDef do
--    begin
--      Name := 'PONTO';
--      DataType := ftString;
--      Size := 62;
--    end;
GO
PRINT 'EC141: FolhaClt ok';
GO

-- ===========================================================================
-- Pontos_Mes
-- ===========================================================================
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
--=============================================
 --Author:		<Author,,Name>
 --Create date: <Create Date,,>
 --Description:	<Description,,>
-- --=============================================
--- VERSÂO 20 de MAIO DE 2019 ONDE SE CRIA ARQ TEMPORARIO E CHAMA O FOLHACLT_ATUALIZA_TAREFA 
--- VERSÂO 06 DE ABRIL DE 2021 -> ACRESCENTA A GRATIFICAÇÂO DE PARTICIPAÇÂO NO RESULTADO
--- VERSÂO  23 de NOVEMBRO DE 2021 -> ATUALIZA O CLTPONTO PARA CORRIGIR QUADRAS LOCADAS EM GLEBAS ERRADAS
-- VERSAO 20 de março de 2023
ALTER PROCEDURE [dbo].[Pontos_Mes]  
	-- Add the parameters for the stored procedure here
	
	@inicio date,
	@fim date
AS
BEGIN
	
	

	SET LANGUAGE us_english;
  /*  DECLARE @inicio date,
 	@fim date,@naograve int;
    SET @inicio ='2020-04-01'
	SET @fim ='2020-04-30';
	SET @naograve = 3; 
	*/
/*
RETORNO POSSIVEIS 
2 -> foLHA ANTERIOR À INICIO DO SISTEMA ==> Não Faz Nada
3 -> Folha e CUSTOFOLHA já EXISTEM  -> Só REATUALIZA SE AS DUAS TABELAS SE o  @naoGrave vier COM valor 3,
     caso DE INSUCESSO retorna 3 
	  
4 -> Folha e CUSTOFOLHA já EXISTE ATUALIZADA -> Só REATUALIZA A TABELA CUSTOFOLHA  E SOMENTE SE O @naoGrave vier COM valor 4,
     caso DE SUCESSO  retorna  4
1 -> SUCeSSO GRAVOU NOVATFOLHA E CUSTOFOLHA (independente do valor de @naoGrave


*/

    UPDATE C1
    SET FAZMOV = q.GLEBA
    FROM CLTPONTO C1 
   INNER JOIN QUADRAS q ON (q.QUA = BL)
    WHERE DATA BETWEEN DATEADD(day,-6,@inicio) and @fim 
   and q.FIM is null and (q.GLEBA <> fazmov) and (q.GLEBA <> '') and bl <> ''


     
     IF OBJECT_ID('tempdb..##TABELAGERAL') IS NOT NULL 
     BEGIN 
         DROP TABLE ##TABELAGERAL 
     END
	  CREATE TABLE ##TABELAGERAL (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)) 
	
    

    DECLARE @RETORNO int;

	DECLARE @SOATUALIZA_CUSTOFOLHA INT = 0;


	SET @fim = DATEADD(s,-1,DATEADD(mm, DATEDIFF(m,0,@fim)+1,0));

	SET @RETORNO = 1;

	/*IF (@FIM < '2018-11-01')
	BEGIN
	    IF (@naograve <> 4)
		BEGIN
		   SELECT 2 as RETORNO; -- folha anterior ao inicio das atulizações CLTSERVER
		   RETURN;
        END;
	    SET @SOATUALIZA_CUSTOFOLHA = 1;
	    SET @RETORNO = 4;
	END
	ELSE
	BEGIN
    --SELECT @FOLHAJAEXISTE =  
		IF EXISTS(SELECT ID FROM NOVACLTFOLHA WHERE (DATA = @Fim)) 
		BEGIN
			IF ((@naograve <> 3) AND (@naograve <> 4))
			BEGIN
			   SELECT 3 as RETORNO; -- folha anterior ao inicio das atulizações CLTSERVER
			   RETURN;
			END; 
			IF (@naograve = 4) 
			BEGIN
			  SET @SOATUALIZA_CUSTOFOLHA = 1;
			  SET @RETORNO = 4;
			END; 
		END;
	END;
	*/
	DECLARE @comparafolha bit = 0;
	DECLARE @CUSTO bit = 1;


	SET NOCOUNT ON;
	-- pega a semana do ponto anterior ao inicio do mes
	DECLARE @primeirasemana date;
	SET @primeirasemana = @inicio;
	WHILE (DATENAME(weekday,@primeirasemana) <> 'Thursday') 
	begin
      	 SET @primeirasemana= DATEADD(dd,-1, @primeirasemana);
	end;

	-- RETORNOMES === 0 - evento fora do mes, 1 - retorno ao trabalho no mes, 2 - saida para o INSS no mes

	DECLARE @AFASTADOSINSS TABLE (COD nchar(4), TIPO INT, TPSEFIP NCHAR(2), DATAEVENTO Date, retornonoMes int);
	INSERT INTO @AFASTADOSINSS
	SELECT * FROM [dbo].[AFASTADOS_INSS](@fim);

--	TABELAS AUXILIARES
	-- Salario minimo e fgts
	DECLARE @SalMinFgts TABLE ([DATA] date,VALOR numeric(18,4),FGTS numeric(18,4));
	insert @SalMinFgts 
	EXECUTE [dbo].[SalMinFgts]
	        @fim; 
	DECLARE @SalMin numeric(18,4);		
	DECLARE @taxaFGTS numeric(18,4);
	SELECT @SalMin=VALOR,@taxaFGTS=FGTS from @SalMinFgts;

	-- tabeal do INNS  com tipo definido
	DECLARE @TabInss TipoTabelaINSS;
	insert into @TabInss
	SELECT [data], faixa, valor1, valor2, taxa FROM TABINSS  
         where  (Format(TABINSS.DATA,'yyyyMM') <= Format(@fim,'yyyyMM'))
       ORDER BY [DATA] DESC,faixa; 

	DECLARE @MAX_DATA DATE, @MAX_FAIXA numeric(6,0)
	SELECt @MAX_DATA=MAX(DATA)
	 FrOM @TabInss 
	SELECt 
	 @MAX_FAIXA=MAX(FAIXA) FrOM TABINSS WHERE DATA = @MAX_DATA;
	DECLARE @TETO_MAX NUMERIC(19,4), @TETO_INSS NUMERIC(19,4);
	SELECt @TETO_MAX =  VALOR2  FrOM TABINSS where data = @MAX_DATA and FAIXA = @MAX_FAIXA
	
	SET @TETO_INSS = dbo.ValorInss(@TabInss,@fim,@TETO_MAX);


	
    -- tabelas IRFONTE com tipo definido
	DECLARE @TabIrFonte TipoTabelaIRFonte;
	insert into @TabIrFonte
	SELECT TABIR.data, faixa, base1, base2, aliq, TABIR.deduzir,
    (SELECT MAX(irdedu.DEDUCAO) from irdedu where (TABIR.Data = IRDEDU.DATA) ) deduGeral
    FROM TABIR
	where  Format(TABIR.[data], 'yyyyMM') <= Format(@fim,'yyyyMM')
      ORDER BY TABIR.DATA DESC, faixa;

   -- tabela SALFAM
   DECLARE @TabSalFam TipoTabelaSalFam;
   insert into @TabSalFam
      SELECT data, faixa, valor1, valor2, salfam FROM SALFAM 
	  where  Format([data], 'yyyyMM') <= Format(@fim,'yyyyMM')
	  ORDER BY DATA DESC,faixa;
	

    --tabela gratificação
	DECLARE @tabelagratif TABLE (num_mod nvarchar(2),gcodser nvarchar(3),gsetor nvarchar(2),perc_diaria numeric(18,4),
	          vlr_real numeric(18,4),dta_ini date,dta_fim date  );
	insert into @tabelagratif                        
    SELECT NUM_MOD,CODSER,iif(SETOR='','  ',SETOR),[PERCENT],VLRDIARIA,DTA_INI,DTA_FIM FROM GRATIF WHERE 
                (DTA_INI <= @inicio) and ((DTA_FIM >= @fim) 
              or (DTA_FIM is null) ) and 
			  (CODSER is not null) and ([PERCENT] is not null) and (VLRDIARIA is not null);
	
	-- tabela cltcodig
	DECLARE @tabcltcodigo TABLE (INDCOD nchar(2),DESCOD nvarchar(21),FATOR numeric(19,4),
	 HORAS numeric(19,4),X_HORAS numeric(19,4),NOTURNO numeric(19,4),
	 H_NORMAL numeric(19,4),COMPATI nchar(1));
	 insert into @tabcltcodigo SELECT  INDCOD ,DESCCOD ,FATOR ,HORAS ,X_HORAS ,NOTURNO ,
	 H_NORMAL ,COMPATI from cltcodig;

    -- montagem da "tabela" vet_sabado
	DECLARE @vet_sabado TABLE(oindcod nvarchar(2));
    DECLARE @i int;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( ' '+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 10)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( '.'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( 'C'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
-- numero de dias no mes	
   DECLARE @ndiasmes int = DateDiff(day,@inicio,@fim)+1;

-- Este é o listaDados (numa store procedure) retorna as informações do CLTCAD e FERIAS 
  
   DECLARE @tabelaLista TABLE (CODCAD nvarchar(4),NOMECAD nvarchar(60),DEMI date,
     CATEGORIA nvarchar(30),CBO numeric(19,4),MENSALISTA nchar(1),
	 CONTA1 nchar(10),BANCO1 nchar(3),AGENCIA1 nchar(8),CPF nchar(11),END_RUA nvarchar(50) ,
	   END_CID nvarchar(25) ,END_UF nchar(2),END_CEP nchar(10),
       SETOR nchar(2), salbase numeric(18,4),ult_atual date,
       Glecad NVARCHAR(4),admi date,demitido int,admitido int,diastrab int,
         tsal numeric(18,4), soma_perc_conf numeric(18,4),      ndepend int,emgozo int,irdepend int, salcontrib numeric(18,4),prov_ferias numeric (18,4),
           prov_decimo numeric(18,4), inss numeric(18,4), salfam numeric(18,4), FERIAS_BASE numeric(18,4)
		   , FERIAS_MEDIA numeric(18,4)
		   , SALMEDIO_FERIAS numeric(18,4));
       insert into @tabelaLista
       EXECUTE  [dbo].[RetornaListaDados] 
          @inicio
          ,@fim
          ,@SalMin;
         
 --XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX

-- TRANSFORMA o CLTPONTO ( 1-registro com dias da semana => nova tabela com 1 registro = 1dia )
 -- tabela geral que transforma o cltponto numa tabela individualizada de evento     
    DECLARE @ndia int;
	DECLARE @dia NVARCHAR(4);
	/*DECLARE @tabelageral TABLE (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)) */
	
	
	
	set @ndia = 0;
	while (@ndia < 7) 
	begin
	    set @dia ='DIA'+CONVERT(nvarchar,(@ndia+1),1);
	    DECLARE @literal as varchar(5120);
	    SET @literal =
	    'select TRAB,SETOR,fazmov,BL,NUM_MOD,CODSER,cltponto.noturno as noturno,Data as Semana,quant,ok,'+
        'DATENAME(weekday, DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA))   as diasemana,'+
       'DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) as dtadia, '+@dia+  ', '+--cdg.indcod,'+
       'cdg.fator as Diarias,cdg.HORAS as Horas_efetivas,cdg.X_HORAS as Horas50,cdg.NOTURNO as Horas100,cdg.H_NORMAL as Horasfe,cdg.COMPATI'+--,cdg.DESCCOD'+
        ' from cltponto LEFT OUTER JOIN CLTCODIG as cdg '+
         'on ( cdg.INDCOD = '+@dia+')  WHERE (CLTPONTO.NOTURNO <> ''P'') AND '+
        '(data between '''+CAST(@primeirasemana as NVARCHAR(12))+''' and '''+CAST(@fim as NVARCHAR(12))+''') and ('+@dia+' <> ''  '') and '+ 
       ' (DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) >='''+ CAST(@inicio as NVARCHAR(12))+''') and '+
        '(DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) <= '''+CAST(@fim as NVARCHAR(12))+''')'; 
        
		insert into ##TABELAGERAL EXEC(@literal);
        set @ndia = @ndia+1;
    end;


	


      
	  /*
	  Cast(SUM(hxs) as float) as HEXT_NORMAIS,
Cast(SUM(vlr_hxs) as float)  VAL_HEXT_NORMAIS_100 , 
Cast(SUM(HXA) as float) HEXT_FERIADOS,
Cast(SUM(VLR_HXA)  as float)  as VAL_HEXT_FERIADOS_50,
Cast( SUM(HXN) as float) as HNOTURNAS,

	  */

/*	  SELECT *
    
FROM
(
    SELECT  '[DIA'+CAST(DAY(DTADIA) AS VARCHAR(1))+']' data1, INDCOD 
    FROM ##TABELAGERAL
    --WHERE data1 BETWEEN '[DIA1]' AND '[DIA3]'
) src
PIVOT
(
    MAX(INDCOD) FOR data1 IN ([DIA1] ,[DIA2], [DIA3])
) pvt

*/

--	return;
	
   --processamneto corresponde ao Gera_Valores da versão Delphi (sem o processamento do ponto)
   DECLARE @tabelaprocess TABLE
   ( numtarefa int,
   trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  Diarias numeric(18,4),
	  Diariasn numeric(18,4),
	  Horasfe numeric(18,4),
	  Horasfen numeric(18,4),
	  Horas50 numeric(18,4),
	  Horas50n numeric(18,4),
	  Horas100 numeric(18,4),
	  horas_efetivas numeric(18,4),
	  Diarias_adc numeric(18,4),
	 
	  compati nvarchar(1),
	  quant  numeric(18,4), 
	  ok nvarchar(1),
	  -- acrescentei
	  Horas_Dom numeric(18,4),  -- horas feriado que cairam num domingo
	  Horas_Domn numeric(18,4) --   
	  );
	  
   
	insert into  @tabelaprocess
	select 0, trab ,tbger.setor ,gleba ,bl ,num_mod ,codser ,
	  noturno , 
	  semana , diasemana , dtadia,indcod ,
	  iif(diarias>0,iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiarias,
	 
	 iif(diarias>0 and (noturno='X'),iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiariasn,
	 
	  iif(horasfe>0,horasfe,0) as pHorasfe,
	  
	  iif((horasfe>0) and (noturno='X'),horasfe,0) as pHorasfen,
	  
	  iif(horas50>0,horas50,0) as pHoras50,
	  iif((horas50>0) and (noturno='X'),horas50,0) as pHoras50n,

	  iif(horas100>0,horas100,0) as pHoras100,
	  
	  iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0,iif( ( indcod = 'X'),horas_efetivas/2, horas_efetivas),horas_efetivas),0) * iif((diarias>0) and (diasemana not in ('Saturday','Sunday')), dbo.fn_FatorHorasEfetivas(dtadia), 1) as horas_efetivas,
	  -- diaria_adc
	  iif(not Exists(Select gcodser from @tabelagratif as grat where ((codser = grat.gcodser) and 
	  ((tbger.setor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))),0,
	     iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and (rtrim(indcod)='X'), 
	       ( (horas_efetivas/2) *  (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser))),
		(horas_efetivas * (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser)) )),0)) as diaria_adc

	  ,compati,quant,ok ,

	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0),horasfe,0) as pHoras_Dom,
	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0) and (noturno='X'),horasfe,0) as pHoras_Domn

	  from ##TABELAGERAL 
	   as tbger left join @tabelaLista as lst 
	  ON (lst.CODCAD = tbger.trab) 
	     WHERE (admi <= dtadia) and ((demi is null) OR (dtadia <= DEMI)) and (noturno <> 'P')  ;
		 
/*
	SELECT * FROM  @tabelageral 
	   as tbger left join @tabelaLista as lst 
	  ON (lst.CODCAD = tbger.trab) 
	     WHERE (admi <= dtadia) and ((demi is null) OR (dtadia <= DEMI)) and tbger.setor = ' 6' and trab = '8942'
		 order by dtadia ;

return;
*/

DECLARE @ERRO1 TABLE
   ( 
   trab nvarchar(4), dia date,obs nvarchar(20))

DECLARE @ERRO2 TABLE
   ( 
   trab nvarchar(4), dia date,obs nvarchar(20))

insert into @ERRO1
SELECT DISTINCT TRAB,dtadia, 'FE ou Doença ou FI ' obs
from
(select trab, dtadia 

 ,sum(iif(diariasn<>0,0,diarias) ) diarias
				,sum(diariasn ) AS DIARIASn
				,sum(iif(horasFen<>0,0,HORASFE)) horasFe
				,sum( horasFeN) horasFeN
				,sum(iif(horas50n<>0,0,horas50) ) horas50
				,sum(horas50n) HORAS50N
				,sum(horas100 ) horas100
			    
			   ,sum(horas_efetivas ) horas_efetivas
				,sum(iif(indcod='FI',1,0)) as faltas
				,sum(iif(indcod='A',1,0)) as doenca
	            , sum(iif(indcod='FE',1,0)) as diasferias
from @tabelaprocess
GROUP BY trab, dtadia ) foo
WHERE 
( ( (FALTAS <> 0) OR (DOENca <> 0) or diasferias <> 0)
  and ((horasFe+horasFeN+horas50+horas50n+horas100) <> 0)

)
order by trab, dtadia
insert into @ERRO2
SELECT DIStiNCT TRAB,dtadia, 'DIAS ou HORAS ' obs
  
from
(select trab, dtadia 
			    ,sum(iif(diariasn<>0,0,diarias) ) diarias
				,sum(diariasn ) AS DIARIASn
				,sum(iif(horasFen<>0,0,HORASFE)) horasFe
				,sum( horasFeN) horasFeN
				,sum(iif(horas50n<>0,0,horas50) ) horas50
				,sum(horas50n) HORAS50N
				,sum(horas100 ) horas100
			   
			   ,sum(horas_efetivas ) horas_efetivas
				,sum(iif(indcod='FI',1,0)) as faltas
				,sum(iif(indcod='A',1,0)) as doenca
	            , sum(iif(indcod='FE',1,0)) as diasferias
from @tabelaprocess
GROUP BY trab, dtadia ) foo
WHERE 
(  ((diarias <> 0) and  (diarias < 8) and (horasFe+horas50+horas100) <> 0)
   OR   
 ( (diariasN <> 0) and ( diariasN < 8) and (horas50n <> 0) )
      OR
 ( ( HORASFEN <> 0) and ( HORASFEN < 8) and (horas50n <> 0) )
      OR
 
 (horasFeN > 8) 
    OR
   (DiariasN > 8) 
    OR
   (diarias  > 8) 
   )


order by trab, dtadia










DECLARE @PONTOMES TABLE (
	numtarefa int,
   trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  Diarias numeric(18,4),
	  Diariasn numeric(18,4),
	  Horasfe numeric(18,4),
	  Horasfen numeric(18,4),
	  Horas50 numeric(18,4),
	  Horas50n numeric(18,4),
	  Horas100 numeric(18,4),
	  horas_efetivas numeric(18,4),
	  Diarias_adc numeric(18,4),
	 
	  compati nvarchar(1),
	  quant  numeric(18,4), 
	  ok nvarchar(1),
	  -- acrescentei
	  Horas_Dom numeric(18,4),  -- horas feriado que cairam num domingo
	  Horas_Domn numeric(18,4) --   
	 
	  , DIA1 nchar(2),
       DIA2 nchar(2),
	     DIA3 nchar(2),
		   DIA4 nchar(2),
		     DIA5 nchar(2),
			   DIA6 nchar(2),
			   DIA7 nchar(2),
       DIA8 nchar(2),
	     DIA9 nchar(2),
		   DIA10 nchar(2),
		     DIA11 nchar(2),
			   DIA12 nchar(2),
			      DIA13 nchar(2),
		     DIA14 nchar(2),
			   DIA15 nchar(2),
			    DIA16 nchar(2),
       DIA17 nchar(2),
	     DIA18 nchar(2),
		   DIA19 nchar(2),
		     DIA20 nchar(2),
			   DIA21 nchar(2),
			   DIA22 nchar(2),
       DIA23 nchar(2),
	     DIA24 nchar(2),
		   DIA25 nchar(2),
		     DIA26 nchar(2),
			   DIA27 nchar(2),
			      DIA28 nchar(2),
		     DIA29 nchar(2),
			   DIA30 nchar(2),
			   DIA31 nchar(2));


INSERT INTO @PONTOMES

	  select tab.*,
	  iif(DAY(DTADIA)=1,INDCOD,'') DIA1
      ,iif(DAY(DTADIA)=2,INDCOD,'') DIA2
	  ,iif(DAY(DTADIA)=3,INDCOD,'') DIA3
	  ,iif(DAY(DTADIA)=4,INDCOD,'') DIA4
	  ,iif(DAY(DTADIA)=5,INDCOD,'') DIA5
	  ,iif(DAY(DTADIA)=6,INDCOD,'') DIA6
	  ,iif(DAY(DTADIA)=7,INDCOD,'') DIA7
	  ,iif(DAY(DTADIA)=8,INDCOD,'') DIA8
	  ,iif(DAY(DTADIA)=9,INDCOD,'') DIA9
	  ,iif(DAY(DTADIA)=10,INDCOD,'') DIA10
	  ,iif(DAY(DTADIA)=11,INDCOD,'') DIA11
	  ,iif(DAY(DTADIA)=12,INDCOD,'') DIA12
	  ,iif(DAY(DTADIA)=13,INDCOD,'') DIA13
	  ,iif(DAY(DTADIA)=14,INDCOD,'') DIA14
	  ,iif(DAY(DTADIA)=15,INDCOD,'') DIA15
	  ,iif(DAY(DTADIA)=16,INDCOD,'') DIA16
	  ,iif(DAY(DTADIA)=17,INDCOD,'') DIA17
	  ,iif(DAY(DTADIA)=18,INDCOD,'') DIA18
	  ,iif(DAY(DTADIA)=19,INDCOD,'') DIA19
	  ,iif(DAY(DTADIA)=20,INDCOD,'') DIA20
	  ,iif(DAY(DTADIA)=21,INDCOD,'') DIA21
	  ,iif(DAY(DTADIA)=22,INDCOD,'') DIA22
	  ,iif(DAY(DTADIA)=23,INDCOD,'') DIA23
	  ,iif(DAY(DTADIA)=24,INDCOD,'') DIA24
	  ,iif(DAY(DTADIA)=25,INDCOD,'') DIA25
	  ,iif(DAY(DTADIA)=26,INDCOD,'') DIA26
	  ,iif(DAY(DTADIA)=27,INDCOD,'') DIA27
	  ,iif(DAY(DTADIA)=28,INDCOD,'') DIA28
	  ,iif(DAY(DTADIA)=29,INDCOD,'') DIA29
	  ,iif(DAY(DTADIA)=30,INDCOD,'') DIA30
	  ,iif(DAY(DTADIA)=31,INDCOD,'') DIA31
	  
	  	  FROM  @tabelaprocess tab
	  
	  order by trab,DAY(DTADIA) --,num_mod,codser

	  
	  --SELECT * FROM
	  SELECT mes.trab Codigo,cltcad.NOMECAD Nome, cltcad.SETOR SetorCadastro,    mes.setor Setor, Gleba, bl as Quadra, moDELO.DESCRI Modelo, SERVIC.DESCRI Serviço,noturno
	  ,DIA1 ,
       DIA2 ,
	     DIA3 ,
		   DIA4 ,
		     DIA5 ,
			   DIA6 ,
			   DIA7 ,
       DIA8 ,
	     DIA9 ,
		   DIA10 ,
		     DIA11 ,
			   DIA12 ,
			      DIA13 ,
		     DIA14 ,
			   DIA15 ,
			    DIA16 ,
       DIA17 ,
	     DIA18 ,
		   DIA19 ,
		     DIA20 ,
			   DIA21 ,
			   DIA22 ,
       DIA23 ,
	     DIA24 ,
		   DIA25 ,
		     DIA26 ,
			   DIA27 ,
			      DIA28 ,
		     DIA29 ,
			   DIA30 ,
			   DIA31 
			    ,CAST(iif(diariasn<>0,0,diarias) as float) Horas_Normal
				,CAST(diariasn as float) Horas_Noturnas
				,CAST(iif(horas_dom <> 0,0, horasFe) as float) Horas_Feriado
				,CAST(iif(horas_domn<>0,0, horasFeN) as float) Horas_Feriado_Noturno
				,CAST(iif(horas50n<>0,0,horas50) as float) HorasExtra_Normais
				,CAST(horas50n as float) HorasExtra_Noturno
				,CAST(horas100 as float) HorasExtra_100
				,CAST(iif(horas_domn<>0,0,horas_dom) as float) HorasDomingo
				,CAST(horas_domn as float) HorasDomingo_Noturno
		       ,CAST(diarias_adc as float) horas_gratificadas
			   ,CAST(horas_efetivas as float) horas_efetivas
				,iif(indcod='FI',1,0) as faltas
				,iif(indcod='A',1,0) as doenca
	            , iif(indcod='FE',1,0) as diasferias
	            , iif(erro1.obs is null,'',erro1.obs) 
				   + iif(erro2.obs is null,'',' '+erro2.obs)
				as obs

	  FROM @PONTOMES mes
	  LEFT JOIN CLTCAD ON (CLTCAD.CODCAD = mes.trab)
	  LEFT JOIN MODELO ON (modelo.NUM = mes.num_mod)
	  LEFT JOIN SERVIC ON (servic.COD = mes.codser)
	  LEFT JOIN @ERRO1 erro1
	  ON (erro1.trab = mes.trab) and (erro1.dia = mes.dtadia) 
	  LEFT JOIN @ERRO2 erro2
	  ON (erro2.trab = mes.trab) and (erro2.dia = mes.dtadia) 


	   where modelo.DESCRI <> ''
	   
	 order by mes.trab,DAY(DTADIA),numtarefa
	
	 SELECT clt.* 
	 from 
	 (SELECT DISTINCT INDCOD FROM 
	   @PONTOMES) as foo
	   LEFT JOIN cltCODIG clt on (clt.INDCOD = foo.indcod) 


RETURN;
/*


  DECLARE @tabelaPREMIO TABLE
   ( 
      DATA Date,
	  setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3), trab nvarchar(4),  num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  
	  Valor numeric(18,4),
	  DTA_INI Date, DTA_FIM Date,
	  INCLUIDO int
	  );
	  
   INSERT INTO @tabelaPREMIO
   select  DATA, SETOR, FAZMOV, BL, TRAB, NUM_MOD, CODSER, NOTURNO, VALOR, DTA_INI, DTA_FIM, 0 as incluido  from cltponto  
       WHERE (CLTPONTO.NOTURNO = 'P') AND 
        (data between @inicio and @fim)

	  
  -- Base do Calculo da Folha por TRABALHADOR (agregada) 
      DECLARE @tabelaagregada TABLE (trab nvarchar(4),
	    dias numeric(18,4),
	    diarias numeric(18,4), diarias_n numeric(18,4),
	     horasfe numeric(18,4),horasfen numeric(18,4),
	     horas50 numeric(18,4),horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	     horas_domn numeric(18,4));
		--  horas_BancoHoras_E int,
		 -- horas_BancoHoras_C int );  
	  
	  insert into @tabelaagregada
	  select trab,
	  Round(SUM(Diarias)/8,2),
	  SUM(Diarias) as diarias ,
	  SUM(diariasn) as diarias_n,
	  SUM(horasfe) as horasfe, SUM(horasfen) as horasfen,
	  SUM(horas50) as horas50 ,SUM(horas50n) as horas50n ,
	  SUM(horas100) as horas100 ,SUM(horas_efetivas) as horas_efetivas ,
	  SUM(diarias_adc) as diarias_adc ,
	  SUM(iif(indcod='FI',1,0)) as faltas,
	  SUM(iif(indcod='A',1,0)) as doenca,
	  SUM(iif(indcod='FE',1,0)) as diasferias,
	  SUM(horas_dom) as horas_dom,
	  SUM(horas_domn) as horas_domn 
	 
	  FROM @tabelaprocess
	    group by trab order by trab

--	 select trab, horas_BancoHoras_C, horas_BancoHoras_E from @tabelaagregada;
--	return;
	 -- operaçao para calculos agregados do trabalhador
	  -- a folha por trabalhador
	  DECLARE @FOLHA TABLE (   
	      DATA date,
		  SETOR nchar(2),
	      CENTRO nchar(4),
		  QUADRA nchar(3),
		   trab nchar(4),
	      TIPO nchar(1),
		  NUM_MOD nchar(2),
		  CODSER nchar(3),
		  QUANT numeric(19,4), 
		  SALARIO numeric(19,4), --vlrdiarias
		  VLR_HXS numeric(19,4), -- vlrhorasfe
		  VLR_HXA numeric(19,4), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
		  FGTS numeric(19,4),
		   SALFAM numeric(19,4),
		   INSS numeric(19,4),
		   FERIAS numeric(19,4),
		   DECIMO numeric(19,4),
		   FGTS_FE numeric(19,4),
		   FGTS_DEC numeric(19,4),
	      EDUC numeric(19,4),
		  TERC numeric(19,4),
		  EDUC_FE numeric(19,4),
		  EDUC_DEC numeric(19,4),
		  MULTA_FGTS numeric(19,4),
		  HX numeric(19,4),
		  HXS numeric(19,4), -- vlrhorasfe
		  HXA numeric(19,4), -- vlrhoras50
	      HXN numeric(19,4),-- vlrhoras100
	      HEFETIVA numeric(19,4),
	      PONTO nvarchar(62),
	      IRFONTE numeric(19,4),
	      GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  FALTAS numeric(13,0),
		  HORAS_DOM numeric(19,4),
		  HORAS_FER numeric(19,4),
		  VLR_DOM numeric(19,4),
		  VLR_FER numeric(19,4),
		  TXINSS numeric(19,4),
		  IRDEDU numeric(19,4),
		  DIAS numeric(19,4)
	  );
	
	      DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @SALARIO numeric(19,4), --vlrdiarias
		  @VLR_HXS numeric(19,4), -- vlrhorasfe
		  @VLR_HXA numeric(19,4), -- vlrhoras50
	      @VLR_HXN numeric(19,4),-- vlrhoras100
		  @FGTS numeric(19,4),
		  @SALFAM numeric(19,4),
		  @INSS numeric(19,4),
		  @FERIAS numeric(19,4),
		  @DECIMO numeric(19,4),
		  @FGTS_FE numeric(19,4),
		  @FGTS_DEC numeric(19,4),
	      @EDUC numeric(19,4),
		  @TERC numeric(19,4),
		  @EDUC_FE numeric(19,4),
		  @EDUC_DEC numeric(19,4),
		  @MULTA_FGTS numeric(19,4),
		  @HX numeric(19,4),
		  @HXS numeric(19,4), -- vlrhorasfe
		  @HXA numeric(19,4), -- vlrhoras50
	      @HXN numeric(19,4),-- vlrhoras100
	      @HEFETIVA numeric(19,4),
	      @PONTO nvarchar(62),
	      @IRFONTE numeric(19,4),
	      @GRATIF numeric(19,4),
	 	  @FALTAS numeric(13,0),
		  @HORAS_DOM numeric(19,4),
		  @HORAS_FER numeric(19,4),
		  @VLR_DOM numeric(19,4),
		  @VLR_FER numeric(19,4);

	DECLARE @agtrab nvarchar(4),
	    @agdias numeric(18,4),
	    @agdiarias numeric(18,4), @agdiarias_n numeric(18,4),
	     @aghorasfe numeric(18,4),@aghorasfen numeric(18,4),
	     @aghoras50 numeric(18,4),@aghoras50n numeric(18,4),
	     @aghoras100 numeric(18,4),
     	 @aghoras_efetivas numeric(18,4),
	     @agdiarias_adc numeric(18,4),
	     @agfaltas int,@agdoenca int,@agdiasferias int,
		 @aghoras_dom numeric(18,4),@aghoras_fer numeric(18,4),
		 @aghoras_domn numeric(18,4),@aghoras_fern numeric(18,4);

	-- INICIO DO PROCESSAMENTO PARA A FOLHA POR TRABALHADOR	  
	 DECLARE cursor_mst CURSOR FOR   
      SELECT * from @tabelaagregada order by trab;
	 OPEN cursor_mst;
	
	 -- dados do listadados
	 DECLARE @tsal numeric(18,4),
	     @irdepend int,
		 @ndepend int,
		 @emgozo int,
		 @diastrab int,
		 @salcontrib numeric(18,4),--ferias
		 @feriasinss numeric(18,4),
		 @feriassalfam numeric(18,4),
		 @setor nchar(2),
		  @centro nchar(4);
		 --operadores 
	 DECLARE @trab nvarchar(4),
	   @dias numeric(18,4),
	   @vlrdiarias numeric(18,4), 
	   @vlrdiarias_adc numeric(18,4),
	   @vlrhorasfe numeric(18,4),
       @vlrhoras50 numeric(18,4),
	   @vlrdiarias_n numeric(18,4),
	   @vlrhorasfen numeric(18,4),
	   @vlrhoras50n numeric(18,4),
	   @vlrhoras100 numeric(18,4),
	   @vlrhoras_dom numeric(18,4),
	   @vlrhoras_fer numeric(18,4),
	   @FERIAS_BASE numeric(18,4),
	   @FERIAS_MEDIA numeric(18,4),
	   @SALMEDIO_FERIAS numeric(18,4),

	   @vlrdiarias_adc_PREMIO numeric(18,4);
	   

	   DECLARE @DATAADM DATE;
		DECLARE @NDIASADIMITIDO NUMERIC(18,4), @DIASNOMES NUMERIC(18,4);

      DECLARE @TRAB_EM_FERIAS_PROV_FERIAS numeric(18,4);
	  DECLARE @TRAB_EM_FERIAS_PROV_DECIMO numeric(18,4);

	  -- @faltas int,@doenca int,@diasferias int;
	  DECLARE @tsalfam numeric(18,4);
	  DECLARE @insssalcontrib numeric(18,4);


	   FETCH NEXT  FROM cursor_mst INTO
	    @agtrab,
	    @agdias,
	    @agdiarias, @agdiarias_n,
	    @aghorasfe,@aghorasfen,
	    @aghoras50 ,@aghoras50n,
	    @aghoras100,
     	@aghoras_efetivas,
	    @agdiarias_adc,
	    @agfaltas,@agdoenca,@agdiasferias,
		@aghoras_dom,@aghoras_domn;
       DECLARE @totalfolha numeric(19,4) = 0;   
		 
	   DECLARE @demitido int, @admitido int;	 
		  
       WHILE @@FETCH_STATUS = 0  
       BEGIN  
		  SET @trab = @agtrab;
		  SET @dias = @agdias
	
	      SELECT @tsal = lst.tsal,@irdepend = lst.irdepend, @ndepend = lst.ndepend,@emgozo = lst.emgozo,
		   @diastrab = lst.diastrab, @salcontrib = lst.salcontrib,@feriassalfam = lst.salfam,@feriasinss = lst.inss,
		   @setor = lst.Setor,@centro = lst.Glecad,
		   @TRAB_EM_FERIAS_PROV_FERIAS = lst.prov_ferias, @TRAB_EM_FERIAS_PROV_DECIMO = lst.prov_decimo
		   , @demitido = lst.demitido , @admitido = lst.admitido
		   , @FERIAS_BASE = FERIAS_BASE, @FERIAS_MEDIA = FERIAS_MEDIA, @SALMEDIO_FERIAS = SALMEDIO_FERIAS
		   FROM @tabelaLista as lst where (lst.CODCAD=@trab);
	
	    
		  SET @vlrdiarias = Round(@agdias * Round(@tsal/@ndiasmes, 4),2);
		  -- SET @vlrdiarias_adc =  round((@agdiarias_adc/8) * Round(@tsal/@ndiasmes, 4),2);
		  SET @vlrdiarias_adc =  (@agdiarias_adc/8) * (@tsal/@ndiasmes);  --ptRound(((Diarias_ADC / 8) * VlrDiaria), 2)
		  -- EC 141/2026: a hora extra e valorizada pelo divisor do DIA em que foi feita.
		  -- So existem duas faixas no mes em que o divisor muda (dez/2026 e dez/2027);
		  -- nos demais meses @ec_..._2 fica zero e a conta e a mesma de sempre.
		  DECLARE @ec_div_ini numeric(3,0), @ec_div_fim numeric(3,0);
		  DECLARE @ec_hfe_1 numeric(18,4), @ec_hfe_2 numeric(18,4), @ec_hfen_1 numeric(18,4), @ec_hfen_2 numeric(18,4);
		  DECLARE @ec_h50_1 numeric(18,4), @ec_h50_2 numeric(18,4), @ec_h50n_1 numeric(18,4), @ec_h50n_2 numeric(18,4);
		  SET @ec_div_ini = dbo.fn_DivisorHorasExtras(@inicio);
		  SET @ec_div_fim = dbo.fn_DivisorHorasExtras(@fim);
		  SET @ec_hfe_2 = 0; SET @ec_hfen_2 = 0; SET @ec_h50_2 = 0; SET @ec_h50n_2 = 0;
		  IF (@ec_div_ini <> @ec_div_fim)
		      SELECT @ec_hfe_2 = ISNULL(SUM(horasfe),0), @ec_hfen_2 = ISNULL(SUM(horasfen),0),
		             @ec_h50_2 = ISNULL(SUM(horas50),0), @ec_h50n_2 = ISNULL(SUM(horas50n),0)
		        FROM @tabelaprocess
		       WHERE trab = @trab AND dbo.fn_DivisorHorasExtras(dtadia) = @ec_div_fim;
		  SET @ec_hfe_1 = @aghorasfe - @ec_hfe_2;  SET @ec_hfen_1 = @aghorasfen - @ec_hfen_2;
		  SET @ec_h50_1 = @aghoras50 - @ec_h50_2;  SET @ec_h50n_1 = @aghoras50n - @ec_h50n_2;
		  SET @vlrhorasfe= iif(@aghorasfe = 0,0,iif(@ec_hfe_2 = 0, Round((@aghorasfe * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfe_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfe_2 * ((@tsal / @ec_div_fim) * 2.0)), 2))); 
		  SET @vlrhoras50 =iif(@aghoras50 = 0,0,iif(@ec_h50_2 = 0, Round((@aghoras50 * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrdiarias_n =iif(@agdiarias_n=0,0,Round((@agdiarias_n/8) * Round(@tsal/@ndiasmes, 4),2));
		  SET @vlrhorasfen = iif(@aghorasfen = 0,0,iif(@ec_hfen_2 = 0, Round((@aghorasfen * ((@tsal / @ec_div_ini) * 2.0)), 2), Round((@ec_hfen_1 * ((@tsal / @ec_div_ini) * 2.0)), 2) + Round((@ec_hfen_2 * ((@tsal / @ec_div_fim) * 2.0)), 2)));
		  SET @vlrhoras50n = iif(@aghoras50n = 0,0,iif(@ec_h50n_2 = 0, Round((@aghoras50n * ((@tsal / @ec_div_ini) * 1.5)), 2), Round((@ec_h50n_1 * ((@tsal / @ec_div_ini) * 1.5)), 2) + Round((@ec_h50n_2 * ((@tsal / @ec_div_fim) * 1.5)), 2)));
		  SET @vlrhoras100 = Round((@vlrdiarias_n+ @vlrhorasfen+ @vlrhoras50n)*0.25,2);
          SET @vlrdiarias_adc_PREMIO = 0; 
		  SELECT @vlrdiarias_adc_PREMIO = ISNULL(SUM(VALOR),0) FROM  @tabelaPREMIO WHERE TRAB = @trab and incluido = 0;
		  UPDATE @tabelaPREMIO SET INCLUIDO = 1 WHERE TRAB = @trab;

		  -- separa horas feriados e domingos
		  --set @aghorasfe = @aghorasfe - @aghorasfen;  
		 -- set @aghoras_dom = @aghoras_dom - @aghoras_domn;

		  SET @aghoras_fer = iif(@aghorasfe = 0,0, @aghorasfe-@aghoras_dom-@aghoras_domn); 
		 -- SET @aghoras_fern = iif(@aghorasfen = 0,0, @aghorasfen-@aghoras_domn); 
		
		  SET @vlrhoras_dom = 0;
		  SET @vlrhoras_fer = 0;
		  DECLARE @vlrcalc numeric (18,4);
		  SET @vlrcalc = @vlrhorasfe;--+@vlrhorasfen;


		  if (@aghoras_dom <> 0) and (@aghoras_fer = 0) 
          begin
		       SET @vlrhoras_dom = @vlrcalc;
		  end;
		  if (@aghoras_dom = 0) and (@aghoras_fer <> 0) 
          begin
		       SET @vlrhoras_fer = @vlrcalc;
		  end;
          if (@aghoras_dom <> 0) and (@aghoras_fer <> 0) 
          begin
             SET @vlrhoras_dom = Round((@aghoras_dom / (@aghorasfe))
                               * @vlrcalc, 2);		
             SET @vlrhoras_fer = @vlrcalc - @vlrhoras_dom;
		  end;
         
		
	  -- preparacao e calculo para a folha com 
          SET @SALARIO = @vlrdiarias;
		  SET @VLR_HXS = @vlrhorasfe;
		  SET @VLR_HXA = @vlrhoras50;
	      SET @VLR_HXN = @vlrhoras100;
		  -- REVISAO
		  SET @FGTS = dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ 
		           @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO )), 2);
          SET @VLR_DOM = @vlrhoras_dom;
		  SET @VLR_FER = @vlrhoras_fer;
		
		  SET @SALFAM = 0;
		  SET @INSS = dbo.ValorInss(@TabInss,@fim,@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100
		       +@vlrdiarias_adc + @vlrdiarias_adc_PREMIO);
		  
		  -- ALTERADO POSTERIORMENTE NA REVISÃO DE 2019/MARÇO
		  SET @FERIAS =     Round (( (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO)/12) * 1.333,2) ;
		  --dbo.ptRound 
		 
		  SET @DECIMO = Round (( (@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO)/12) ,2) ;
		 
		  IF (@emgozo=1) 
		  BEGIN
		    
			 SET @FERIAS = @FERIAS + IIF(@TRAB_EM_FERIAS_PROV_FERIAS is null,0,Round (( (@TRAB_EM_FERIAS_PROV_FERIAS)/12) * 1.3333,2));
			 SET @DECIMO =  @DECIMO + IIF(@TRAB_EM_FERIAS_PROV_DECIMO is null,0,Round (( (@TRAB_EM_FERIAS_PROV_DECIMO)/12) ,2));
		 
          END

		 
		  ----------- REVISAO 2019/MARÇO
		  SET @FGTS_FE =   dbo.ptRound(((@taxaFGTS / 100) * (@FERIAS)), 2);--dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@FERIAS)), 2);
		  SET @FGTS_DEC =  dbo.ptRound(((@taxaFGTS / 100) * (@DECIMO)), 2);--dbo.ptRoundTrunc(((@taxaFGTS / 100) * (@DECIMO)), 2);
	      SET @EDUC =0;
		  SET @TERC =0;
		  SET @EDUC_FE =Round(@FERIAS * 0.027,2);
		  SET @EDUC_DEC = Round(@DECIMO * 0.027,2);
		 
		  SET @MULTA_FGTS = ROUND((@FGTS+@FGTS_FE+@FGTS_DEC) * 0.50,2)
		  
		  
	
		  SET @HX =@agdiarias;
		  SET @HXS = @aghorasfe;
		  SET @HXA = @aghoras50;
	      SET @HXN = @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; -- as horas noturnas incluem as feriados e extras
	      SET @HEFETIVA = @aghoras_efetivas;
	      SET @PONTO = '';
	      SET @IRFONTE =  dbo.CalcIrFonte(@TabIrFonte,@agtrab,@fim,(@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ 
		      @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO),
		    @INSS ,@irdepend);
          if (@fim >= '2019-05-01') 
		  BEGIN
		     IF @IRFONTE < 10 
			    SET @IRFONTE = 0;
		  END;
	      SET @GRATIF = @vlrdiarias_adc + @vlrdiarias_adc_PREMIO;
	 	  SET @FALTAS=@agfaltas;      
		  SET  @tsalfam = 0;

		  DECLARE @SALARIO_CONTRATUAL NUMERIC(19,4) = 0;
		  if (@fim >= '2019-09-01')
		  BEGIN
		     SET @SALARIO_CONTRATUAL = @tsal;
			 IF EXISTS(SELECT COD FROM @AFASTADOSINSS WHERE COD = @trab) 
			 BEGIN
			    SET @SALARIO_CONTRATUAL = 0;
			    IF EXISTS(SELECT COD FROM @AFASTADOSINSS WHERE COD = @trab AND retornonoMes = 2 AND (DATAevento > @INICIO) )
				BEGIN
				   SET @SALARIO_CONTRATUAL = @tsal;
				END; 
			 END; 
		  END; 
		  
		  if ((@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc +@vlrdiarias_adc_PREMIO ) <> 0) OR (@SALARIO_CONTRATUAL <> 0) 
		  begin
			  if (@emgozo = 0) 
			  begin
				if (@ndepend > 0) 
				begin
                  if (@fim > '2019-09-01') 
                  BEGIN					
			         SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
						   (@SALARIO_CONTRATUAL+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + +@vlrdiarias_adc_PREMIO ), @ndepend);
                  end
				  else
				  begin
				     SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
						   (@VLRDIARIAS + @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO), @ndepend);
				  end;

			      if (@fim >= '2019-05-01') 
				  BEGIN
				     if ((@admitido = 1) OR (@demitido = 1))
				     BEGIN
				        SET @tsalfam = dbo.CalcSalFam( @tabSalfam, @fim, 
					   	  @SALARIO_CONTRATUAL  + @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO, @ndepend);
				        if (@fim > '2019-09-01') 
                        BEGIN					
							IF (@admitido = 1)
							BEGIN
							  SELECT @DATAADM = ADMI FROM CLTCAD WHERE CODCAD = @trab;
							  SET @NDIASADIMITIDO = DATEDIFF(d,@DATAADM,@fim)+1;
							  SELECT @DIASNOMES = day(eomonth(@fim));
							  SET @tsalfam = ROUND((@NDIASADIMITIDO/@DIASNOMES) * @tsalfam,2); 
							END;
							IF (@demitido = 1)
							BEGIN
							  -- NO CASO E DEMITIDO (uso as mesmas variaveis para economizar)
							  SELECT @DATAADM = DEMI FROM CLTCAD WHERE CODCAD = @trab;
							  SET @NDIASADIMITIDO = DATEDIFF(d,@inicio,@DATAADM)+1;
							  SELECT @DIASNOMES = day(eomonth(@fim));
							  SET @tsalfam = ROUND(((@NDIASADIMITIDO/@DIASNOMES) * @tsalfam),2); 
							END;
						END
                     END; 
				  END;
				  
				  if (@diastrab <> 0) AND (@fim < '2019-09-01')
				  begin   
					 SET @tsalfam  = Round(  (CAST (@diastrab as decimal) / CAST( @ndiasmes as decimal)) * @tsalfam, 2); 
				  end;
				end;
			  end
			  else
			  if (@fim >= '2009-04-01') 
			  begin
            
				-- verifica se a faixa de contribuição p/ desc.INSS mudou!
				SET  @insssalcontrib = dbo.ValorInss(@tabinss, @fim,  @salcontrib + 
							  @vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO);
				if (@insssalcontrib >= (@feriasinss + @INSS)) 
				begin -- analise a diferença à maior
					SET @insssalcontrib = @insssalcontrib - (@feriasinss + @INSS);
					if ( @insssalcontrib > 0.02)  -- se a dif.for maior q 0,02
					 begin
					   SET @INSS = @INSS + @insssalcontrib;
					 end;
				end;
				if (@fim >= '2019-05-01') AND ((@feriasinss + @INSS) > @TETO_INSS) 
			    begin
			    	SET @INSS = @TETO_INSS - @feriasinss; 
					IF (@INSS < 0) 
					BEGIN   SET @INSS = 0; END;
				END;
		  -- calcula se o salario familia mudou de faixa em função do Salario Contrib. (ferias+salario)
				if (@ndepend > 0) 
				begin
				   DECLARE @VlrSUBSTITUA NUMERIC(19,4) = @TRAB_EM_FERIAS_PROV_DECIMO;
				    if (@fim < '2019-09-01') 
					BEGIN
					   SET @VlrSUBSTITUA = @salcontrib; 
					END;
				
				    if (@fim > '2019-10-01') 
					BEGIN
					  -- Incluido a partir da folha de agosto 2020, o @vlrdiarias para efeitos de calculo nestes casos
					  
					    if (@fim >= '2021-03-01') 
					    BEGIN
					      SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
						      (@SALARIO_CONTRATUAL +  @FERIAS_MEDIA + @vlrhorasfe+ @vlrhoras50+
							    @vlrhoras100+@vlrdiarias_adc + @vlrdiarias_adc_PREMIO), @ndepend);
                        END
						ELSE
						BEGIN
					      SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
						      (@SALMEDIO_FERIAS + @vlrhorasfe+ @vlrhoras50+ @vlrhoras100
							  +@vlrdiarias_adc + @vlrdiarias_adc_PREMIO), @ndepend);
                        END
					END
					ELSE
					BEGIN
				       SET  @tsalfam =  dbo.CalcSalFam( @tabSalfam, @fim, 
						   (@VlrSUBSTITUA+@vlrdiarias+ @vlrhorasfe+ @vlrhoras50+ @vlrhoras100
						   +@vlrdiarias_adc+@vlrdiarias_adc_PREMIO), @ndepend);

					   if (@tsalfam > @feriassalfam) 
					   begin --diferença a mais pago no SALFAM . OQUE FAZER???
						  SET @tsalfam = (@tsalfam - @feriassalfam);
					   end
					   else
						 SET  @tsalfam = 0;
                   end; 
				end;
				--SET @FERIAS = @feriasProv_Ferias;
			  end;
		  end;

		  SET @SALFAM = @tsalfam;

		  SET @totalfolha = (@totalfolha +@SALARIO+@VLR_HXS+ @VLR_HXA+ @VLR_HXN+ROUND(@GRATIF,2));

	
		 

		  Insert into @FOLHA  (trab ,SETOR,CENTRO,TIPO,
		  SALARIO,VLR_HXS, VLR_HXA, VLR_HXN,FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
		   FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,MULTA_FGTS,HX,HXS,HXA, HXN,HEFETIVA,PONTO,IRFONTE,
	      GRATIF,FALTAS,HORAS_DOM,HORAS_FER,VLR_DOM,VLR_FER, DIAS) VALUES 	
	      (@trab , @setor ,@centro,'D',
		  @SALARIO,@VLR_HXS, @VLR_HXA, @VLR_HXN,@FGTS,@SALFAM,@INSS,@FERIAS,@DECIMO,@FGTS_FE,
		   @FGTS_DEC,@EDUC,@TERC,@EDUC_FE,@EDUC_DEC,@MULTA_FGTS,@HX,@HXS,@HXA, @HXN,@HEFETIVA,@PONTO,@IRFONTE,
	      ROUND(@GRATIF,2),@FALTAS,@aghoras_dom,@aghoras_fer,@VLR_DOM,@VLR_FER, @dias)
		  -- @dias
	      FETCH NEXT  FROM cursor_mst INTO
		  @agtrab,
	      @agdias,
	      @agdiarias, @agdiarias_n,
	      @aghorasfe,@aghorasfen,
	      @aghoras50 ,@aghoras50n,
	      @aghoras100,
     	  @aghoras_efetivas,
	      @agdiarias_adc,
	      @agfaltas,@agdoenca,@agdiasferias,@aghoras_dom,@aghoras_fer;
	   END;
	   CLOSE cursor_mst;  
       DEALLOCATE cursor_mst;  

	
	
	   -- rotina de salario_educação e terceiros
	  DECLARE @teduc numeric(19,4) ,@tterc numeric(19,4);
	  DECLARE @reduc numeric(19,4) ,@rterc numeric(19,4); 
	  SET @teduc = Round(@totalfolha * 0.025, 2);
      SET @tterc = Round(@totalfolha * 0.002, 2);
      UPDATE @FOLHA SET EDUC = Round(((SALARIO+VLR_HXS+ VLR_HXA+ VLR_HXN+GRATIF)/@totalfolha) * @teduc,2),
    	  TERC = Round(((SALARIO+VLR_HXS+ VLR_HXA+ VLR_HXN+GRATIF)/@totalfolha) * @tterc,2);
      SELECT @reduc = SUM(EDUC), @rterc = SUM(TERC) FROM @folha;
	  SET @reduc = @teduc - @reduc;
	  SET @rterc = @tterc - @rterc;
	  DECLARE @maior nvarchar(4);
	  if (@reduc <> 0) 
	  begin
	      SELECT TOP(1) @maior=TRAB FROM @FOLHA ORDER BY EDUC DESC; 
	      UPDATE @FOLHA  SET EDUC = (EDUC + @reduc) WHERE 
		   TRAB =  @maior;
	  end;	  
      if (@rterc <> 0) 
	  begin
	      SELECT TOP(1) @maior=TRAB FROM @FOLHA ORDER BY TERC DESC; 
	      UPDATE @FOLHA  SET TERC = (TERC + @terc) WHERE 
		   TRAB =  @maior;
	  end;	  
      

	     -- Rotina do ponto
	  --DECLARE @tabelaPonto TABLE(trab nvarchar(4), PONTO nvarchar(62));
	 --cursor      
	  DECLARE @pttrab nvarchar(4),@ptdtadia date,@ptindcod nvarchar(2),@ptdiarias numeric(14,2);
	 --trabalho pra logica
	  DECLARE @oponto nvarchar(62),@otrab nvarchar(4), @oacumdiarias numeric(14,2),@odtadia date,@oindcod nvarchar(2),
	  @oindcod2 nvarchar(2);
	  
	  DECLARE cursor_ponto CURSOR FOR  
	   SELECT trab,dtadia,indcod,
	    Diarias as diarias
	    FROM @tabelaprocess     -- where trab = '8942' 
	       order by trab,dtadia;
	 
	  OPEN cursor_ponto;
	  FETCH NEXT  FROM cursor_ponto INTO
	      @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;
      WHILE  @@FETCH_STATUS = 0  
       BEGIN  
		 SET @oponto = ''; 
		 DECLARE @ndias int = DateDiff(day,@inicio, @ptdtadia);
		 if (@ndias > 0) 
		 begin
		   SET @oponto = REPLICATE('  ',@ndias);
		 end;
		 SET @otrab = @pttrab; 
        DECLARE @auxtabela TABLE (indcod nvarchar(2),tabdiaria numeric(14,4));
		 
		 WHILE (@@FETCH_STATUS = 0)  and (@otrab = @pttrab)
         BEGIN
		    SET @odtadia = @ptdtadia;
		    SET @oacumdiarias = @ptdiarias; 
	        SET @oindcod = @ptindcod;
			
			FETCH NEXT  FROM cursor_ponto INTO
	            @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;      
		    
			if (@odtadia <> @ptdtadia)
			begin
			     SET @oponto =   @oponto +  @oindcod;
				
				 SET @i =  DATEDIFF(day, @odtadia, @ptdtadia) - 1;
				 IF (@i > 0) 
				 begin
				     SET @oponto = @oponto + REPLICATE(space(2),@i);
                 end;
				 
				continue; 
			 end;
			 insert into @auxtabela (indcod,tabdiaria) VALUES(@ptindcod, @ptdiarias);
			 WHILE (@@FETCH_STATUS = 0)  and (@otrab = @pttrab) and (@odtadia = @ptdtadia)
             BEGIN  
			     insert into @auxtabela (indcod,tabdiaria) VALUES(@ptindcod, @ptdiarias)
				 SET @oacumdiarias = @oacumdiarias + @ptdiarias;
				 FETCH NEXT  FROM cursor_ponto INTO
	              @pttrab ,@ptdtadia ,@ptindcod ,@ptdiarias;      
		     END;
			 if (@oacumdiarias <> 0)   
             begin   
			   if (@oacumdiarias >= 7.9) and (@oacumdiarias <=8.1)   
               begin
			      if  Exists(select * from @auxtabela where (Substring(indcod,1,1)='Y') )
				      SET @oponto = @oponto + 'Y6';
                  else
			      begin
				    if  Exists(select * from @auxtabela where (Substring(indcod,1,1)='R') )
				        SET @oponto = @oponto + 'R ';
					else
						SET @oponto = @oponto + 'X ';
				  end;
               end
               else
               begin
			       SELECT @oindcod2 = INDCOD from @tabcltcodigo where (ROUND(fator,3) = ROUND(@oacumdiarias,3));
					if (@oindcod2 = null)  
					begin
					   if (@oacumdiarias>8)
			              SET @oponto =@oponto+'X+';
					   else
					      SET @oponto =@oponto+'? ';
					end
					else
					   SET @oponto =@oponto+'!!';
			   end
			end
			else
			begin
			  select top(1) @oindcod2 = INDCOD from @auxtabela where (Substring(indcod,1,1)<>'R')
			  if ( @oponto  <> null)
			     SET @oponto = @oponto +  @oindcod2
			  else
			     SET @oponto = @oponto +  '  ';
			end;
			delete  @auxtabela;
    	 END    
         --insert into @tabelaPonto (trab,ponto) VALUES(@otrab,@oponto);  		
		 UPDATE @FOLHA SET PONTO = @oponto where (TRAB = @otrab);
       END
	   CLOSE cursor_ponto;  
       DEALLOCATE cursor_ponto;  
  
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX CASO DESEJE PROCESSAR O CUSTO XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
--XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
--if (@custo = 1)
--BEGIN
--------   CHAMADA  do  [dbo].[FolhaClt_Atualiza_TAREFAS] -----------
	 DECLARE @X_TRAB TABLE (setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  semana date,   dtadia date, TRAB nchar(4), NumTarefa int, dataLIMITEINFERIOR date, INICIOTAR date, FIMTAR date, DATAOK date); 
	insert into @X_TRAB
       EXECUTE  [dbo].[FolhaClt_Atualiza_TAREFAS]
          @inicio
          ,@fim;
      
       UPDATE tab SET numtarefa = du.NumTarefa 
	   FROM @tabelaprocess as tab 
	    LEFT JOIN   @X_TRAB du ON (
	     tab.bl = du.bl and tab.setor = du.setor and tab.gleba = du.gleba and
		  tab.num_mod = du.num_mod and tab.codser = du.codser and  tab.dtadia between 
		  du.INICIOTAR  and du.FIMTAR) 
	   where tab.bl <> ''   

	   
       DECLARE @CUSTOFOLHA TABLE (   
		  [Id] [int] IDENTITY(1,1) NOT NULL,
		  INICIO date,
		  FIM date,
		  SETOR nchar(2),
	      CENTRO nchar(4),
		  QUADRA nchar(3),
		  TRAB nchar(4),
	      OK nchar(1),
		  NUM_MOD nchar(2),
		  CODSER nchar(3),
		  QUANT numeric(19,4), 
		  SALARIO numeric(19,6), --vlrdiarias
		  VLR_HXS numeric(19,6), -- vlrhorasfe
		  VLR_HXA numeric(19,6), -- vlrhoras50
	      VLR_HXN numeric(19,4),-- vlrhoras100
		  FGTS numeric(19,6),
		  SALFAM numeric(19,6),
		  INSS numeric(19,6),
		  FERIAS numeric(19,6),
		  DECIMO numeric(19,6),
		  FGTS_FE numeric(19,6),
		  FGTS_DEC numeric(19,6),
	      EDUC numeric(19,6),
		  TERC numeric(19,6),
		  EDUC_FE numeric(19,6),
		  EDUC_DEC numeric(19,6),
		  MULTA_FGTS numeric(19,6),
		  HX numeric(19,4),
		  HXS numeric(19,4), -- vlrhorasfe
		  HXA numeric(19,4), -- vlrhoras50
	      HXN numeric(19,4),-- vlrhoras100
	      HEFETIVA numeric(19,4),
	      IRFONTE numeric(19,4),
	      GRATIF numeric(19,4),-- vlrdiarias_adc
	 	  FALTAS numeric(13,0),
		  DOENCA numeric(13,0),
		  BANCOHS_E	numeric(19,4),
		  BANCOHS_C	numeric(19,4),
		  TAREFA_IND_MES	numeric(18,0)
	  );
	   DECLARE 
		  @agsetor NVARCHAR(2),
		  @agcentro NVARCHAR(4),
		  @agquadra NVARCHAR(3),
		  @agnum_mod NVARCHAR(2),
	      @agcodser NVARCHAR(3),
		  @agdtadia date,
		  @agok NVARCHAR(1),
		  @agindcod NVARCHAR(2),
		  @agquant numeric(19,4),
	      
		  @ptsetor NVARCHAR(2),
		  @ptcentro NVARCHAR(4),
		  @ptquadra NVARCHAR(3),
		  @ptnum_mod NVARCHAR(2),
	      @ptcodser NVARCHAR(3),
		  @diainicio date,
		  @diafim date,
		  @ptok nchar(1),
		  @ptquant numeric(19,4),
		  @ptHX numeric(19,4),
		  @ptHXS numeric(19,4), -- vlrhorasfe
		  @ptHXA numeric(19,4), -- vlrhoras50
	      @ptHXN numeric(19,4),-- vlrhoras100
	      @ptHEFETIVA numeric(19,4),
		  @ptDiarias_adc numeric(19,4),
	      @ptfaltas int,
		  @ptdoenca int,

		  @ptBancoHs_E numeric(19,4),
		  @ptBANCOHs_C numeric(19,4),

		  @agINICIO date,
	      @agFIM date
		   ,@agID int;
	 	 
		-- DECLARE @TESTEDIAS int;
	


	 -- ESPAÇO DE DIAS ENTRE UMA OCORRENCIA E OUTRA = 10 DIAS
	-- DECLARE @DIAS_CORRIDOS int = 12;
	-- DECLARE @DATA_ULT_OK date = null;
	 
	-- DECLARE @NOVATAREFA int;
	-- Não irei trabalhar com agregados 
    
     DECLARE @agNUmTarefa int,  @ptNUmTarefa int; 
	
	 DECLARE cursor_custo CURSOR FOR   
      SELECT trab, 
	 setor ,gleba ,bl ,num_mod ,codser ,
	    dtadia ,indcod,ok, Diarias , Diariasn, Horasfe,
	  Horasfen, Horas50, Horas50n, Horas100, horas_efetivas,
	  Diarias_adc,quant, numtarefa from @tabelaprocess order by trab,setor,gleba,bl,codser,dtadia,num_mod;
	   OPEN cursor_custo;
	   FETCH NEXT  FROM cursor_custo INTO
		  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		  @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas,@agdiarias_adc,
		  @agquant, @agNumTarefa;
	 
	 WHILE @@FETCH_STATUS = 0  
     BEGIN  
	     SET @pttrab =  @agtrab;
	     SELECT @SALARIO = SALARIO,@VLR_HXS =VLR_HXS , @VLR_HXA = VLR_HXA, @VLR_HXN = VLR_HXN, @FGTS = FGTS,  @SALFAM =SALFAM, @INSS=INSS,
		  @FERIAS=FERIAS, @DECIMO=DECIMO, @FGTS_FE = FGTS_FE , @FGTS_DEC = FGTS_DEC, @EDUC = EDUC, @TERC = TERC ,
		  @EDUC_FE = EDUC_FE, @EDUC_DEC = EDUC_DEC, @HX = HX, @HXS = HXS, @HXA=HXA,  @HXN=HXN, @HEFETIVA=HEFETIVA,
	      @IRFONTE=IRFONTE, @GRATIF=GRATIF FROM @FOLHA WHERE TRAB=@agtrab; 	 
	      -- valor da hora na diaria normal
		  SET @vlrDiarias = iif(@HX<>0,(@SALARIO/@HX),0);
		  -- valor da hora na horaextra
		  SET @vlrhoras50 = iif(@HXS<>0,(@VLR_HXS/@HXS),0);
		  -- valor da hora na hora trabalhada em feriado
		  SET @vlrhorasfe = iif(@HXA<>0,(@VLR_HXA/@HXA),0);
		  -- vlr hora do adcional noturno
		  SET @vlrhoras100 = iif(@HXN<>0,(@VLR_HXN/@HXN),0);
		  
		  

		  WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab)
          BEGIN  
		    SET @ptsetor =   @agsetor;
	    	SET @ptcentro = @agcentro;
		    SET @ptquadra = @agquadra;
		    SET @ptnum_mod = @agnum_mod;
	        SET @ptcodser = @agcodser;
			SET @ptNUmTarefa = @agNUmTarefa;
	        
			SET @ptHX =@agdiarias;
		    SET @ptHXS = @aghorasfe;
		    SET @ptHXA = @aghoras50;
	        SET @ptHXN = @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; 
	        SET @ptHEFETIVA = @aghoras_efetivas;
	        SET @ptdiarias_adc = @agdiarias_adc;-- diarias de gratificacao
			SET @diainicio = @agdtadia;
			SET @diafim = @agdtadia;
			SET @ptok =@agok;
			SET @ptquant =@agquant;
			SET @ptDiarias_adc = 0;
			SET @ptfaltas = iif(@agindcod='FI',1,0);
			SET @ptdoenca = iif(@agindcod='A',1,0);
			SET @ptBancoHs_E =   iif(substring(@agindcod,1,1) <> 'E',0, 
	           CONVERT(int, substring(@agindcod,2,1)  )  ); 
	      	SET @ptBancoHs_C =   iif(substring(@agindcod,1,1) <> 'C',0, 
	           CONVERT(int, substring(@agindcod,2,1)  )  ); 
	        


			FETCH NEXT  FROM cursor_custo INTO
	    	  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		      @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas, @agdiarias_adc,@agquant,@agNumTarefa;
			-- ALTERAÇÂO IMPORTANTE MAIO 2019
			WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab) and 
			   (@ptsetor= @agsetor) and  (@ptcentro = @agcentro) and (@ptquadra= @agquadra) and
			  (  @ptnum_mod= @agnum_mod) and  (@ptcodser = @agcodser) 
			  -- ALTERAÇÂO maio 2019
			  	and (@ptNUmTarefa = @agNUmTarefa)
			  -- and @TESTEDIAS < @DIAS_CORRIDOS
            BEGIN
			  SET @ptHX =@ptHX+ @agdiarias;
		      SET @ptHXS =@ptHXS+ @aghorasfe;
		      SET @ptHXA =@ptHXA+ @aghoras50;
	          SET @ptHXN =@ptHXN + @aghoras100 + @agdiarias_n+ @aghorasfen+@aghoras50n; 
	          SET @ptHEFETIVA = @ptHEFETIVA+@aghoras_efetivas;
	          SET @ptdiarias_adc = @ptdiarias_adc + @agdiarias_adc;
			  SET @diafim = @agdtadia;
			  if (@ptok='')
			     SET @ptok =@agok;
			  SET @ptquant = @ptquant +@agquant;
			  SET @ptfaltas = @ptfaltas + iif(@agindcod='FI',1,0);
			  SET @ptdoenca = @ptdoenca + iif(@agindcod='A',1,0);
			  SET @ptBancoHs_E = @ptBancoHs_E +  iif(substring(@agindcod,1,1) <> 'E',0, 
	                                         CONVERT(int, substring(@agindcod,2,1)  )  ); 
	          SET @ptBancoHs_C =  @ptBancoHs_C + iif(substring(@agindcod,1,1) <> 'C',0, 
	           CONVERT(int, substring(@agindcod,2,1)  )  ); 


			  FETCH NEXT  FROM cursor_custo INTO
	    	  @agtrab, @agsetor, @agcentro, @agquadra, @agnum_mod,  @agcodser, @agdtadia, @agindcod, @agok, @agdiarias,
		      @agdiarias_n, @aghorasfe, @aghorasfen, @aghoras50 , @aghoras50n, @aghoras100,  @aghoras_efetivas, @agdiarias_adc,@agquant,@agNumTarefa;
       		
		    END;
          -- verificar se houve gratificação 
		    if Exists(Select gcodser from @tabelagratif as grat where ((@ptcodser = grat.gcodser) and 
      	      ((@ptsetor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))) and (@ptHEFETIVA<>0)
            begin
		        SET @ptDiarias_adc =  @ptHEFETIVA *
			      (Select grat.perc_diaria/100 from @tabelagratif as grat where (@ptcodser = grat.gcodser)) * @vlrDiarias;
		    end;

	        INSERT INTO @CUSTOFOLHA (INICIO ,FIM ,SETOR,CENTRO,QUADRA,TRAB, OK,
		         NUM_MOD,CODSER,QUANT,SALARIO,VLR_HXS,VLR_HXA,VLR_HXN, HX,HXS,
		        HXA,HXN, HEFETIVA,  GRATIF, FALTAS,DOENCA, BANCOHS_E,BANCOHS_C, TAREFA_IND_MES)
		      VALUES (@diainicio,@diafim, @ptsetor, @ptcentro,@ptquadra,@pttrab,@ptok,
		         @ptnum_mod,@ptcodser,@ptquant,ROUND(@ptHX * @vlrDiarias,2), ROUND(@ptHXS * @vlrhoras50,2),
			     ROUND(@ptHXA * @vlrhorasfe,2), ROUND(@ptHXN * @vlrhoras100,2),@ptHX,@ptHXS,
			     @ptHXA,@ptHXN, @ptHEFETIVA,ROUND(@ptDiarias_adc,2),@ptfaltas,@ptdoenca, @ptBancoHs_E,@ptBANCOHs_C,@ptNUmTarefa);
     	 END;
	 END;	 
	 CLOSE cursor_custo;  
     DEALLOCATE cursor_custo;
	 
	 -- EM ABRIL DE 2021
	  INSERT INTO @CUSTOFOLHA (INICIO ,FIM ,SETOR,CENTRO,QUADRA,TRAB, OK,
		         NUM_MOD,CODSER,QUANT,SALARIO,VLR_HXS,VLR_HXA,VLR_HXN, HX,HXS,
		        HXA,HXN, HEFETIVA,  GRATIF, FALTAS,DOENCA, BANCOHS_E,BANCOHS_C, TAREFA_IND_MES)
		     SELECT DTA_INI ,DTA_FIM ,SETOR,GLEBA as CENTRO, BL as QUADRA,TRAB, '' OK,
		         NUM_MOD,CODSER, 0 QUANT, 0 SALARIO, 0 VLR_HXS, 0 VLR_HXA, 0 VLR_HXN, 0 HX, 0 HXS,
		        0 HXA, 0 HXN, 0 HEFETIVA, VALOR GRATIF, 0 FALTAS, 0 DOENCA, 0 BANCOHS_E, 0 BANCOHS_C, 0TAREFA_IND_MES
				FROM @tabelaPREMIO WHERE INCLUIDO = 1;

	   
	 -- alocar as diferenças de calculo
	  DECLARE @difsal numeric(19,4),@difhxs numeric(19,4),@difhxa numeric(19,4),@difhxn numeric(19,4),
	  @difgratif numeric(19,4);
	  DECLARE @tabDIF TABLE (trab nvarchar(4),difsal numeric(19,4),difhxs numeric(19,4),difhxa numeric(19,4),
	  difhxn numeric(19,4),	difgratif numeric(19,4));
	  
	  
	  INSERT INTO @tabDIF
	  SELECT TRAB, SALARIO- (SELECT SUM(cus.SALARIO) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsal ,
	  VLR_HXS- (SELECT SUM(cus.VLR_HXS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxs,
	  VLR_HXA- (SELECT SUM(cus.VLR_HXA) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxa,
	  VLR_HXN- (SELECT SUM(cus.VLR_HXN) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_hxn,
	  GRATIF- (SELECT SUM(cus.GRATIF) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difgratif
	   FROM @FOLHA as fol   order by fol.trab;
      
	  DECLARE cursor_rateio CURSOR FOR   
      SELECT trab, difsal,difhxs,difhxa,
	  difhxn,	difgratif from @tabdif order by trab;
	  OPEN cursor_rateio;
	    FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsal,@difhxs,@difhxa,
        	  @difhxn,	@difgratif;
     WHILE (@@FETCH_STATUS = 0)
	 begin
	    if (@difsal <> 0) 
           UPDATE @CUSTOFOLHA SET SALARIO = (SALARIO + @difsal)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.SALARIO DESC);
		if (@difhxs <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXS = (VLR_HXS + @difhxs)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXS DESC);
		if (@difhxa <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXA = (VLR_HXA + @difhxa)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXA DESC);
		if (@difhxn <> 0) 
           UPDATE @CUSTOFOLHA SET VLR_HXN = (VLR_HXN + @difhxn)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.VLR_HXN DESC);
		if (@difgratif <> 0) 
           UPDATE @CUSTOFOLHA SET GRATIF = GRATIF + @difgratif  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.GRATIF DESC);

		FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsal,@difhxs,@difhxa,
        	  @difhxn,	@difgratif;
	 end;
	 CLOSE cursor_rateio;  
     DEALLOCATE cursor_rateio;  

	 --parcelas de OBRIGAÇÔES que serão rateadas pelo valor da folha
--	   SALARIO,VLR_HXN,GRATIF,VLR_HXS,VLR_HXA,  FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
	--	   FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,IRFONTE,
DECLARE                                   --trab nvarchar(4),dias numeric(3),
		  @agSALARIO numeric(19,4), --vlrdiarias
		  @agVLR_HXS numeric(19,4), -- vlrhorasfe
		  @agVLR_HXA numeric(19,4), -- vlrhoras50
	      @agVLR_HXN numeric(19,4),-- vlrhoras100
		  @agGRATIF numeric(19,4),-- vlrdiarias_adc
		  @agFGTS numeric(19,4),
		  @agSALFAM numeric(19,4),
		  @agINSS numeric(19,4),
		  @agFERIAS numeric(19,4),
		  @agDECIMO numeric(19,4),
		  @agFGTS_FE numeric(19,4),
		  @agFGTS_DEC numeric(19,4),
	      @agEDUC numeric(19,4),
		  @agTERC numeric(19,4),
		  @agEDUC_FE numeric(19,4),
		  @agEDUC_DEC numeric(19,4),
		    @agIRFONTE numeric(19,4),
			 @agMULTA_FGTS numeric(19,4);
		
		  DECLARE @ptsalario  numeric(19,4),
		  @fator numeric(19,4),
		  @ptFGTS numeric(19,4),
		  @ptSALFAM numeric(19,4),
		  @ptINSS numeric(19,4),
		  @ptFERIAS numeric(19,4),
		  @ptDECIMO numeric(19,4),
		  @ptFGTS_FE numeric(19,4),
		  @ptFGTS_DEC numeric(19,4),
	      @ptEDUC numeric(19,4),
		  @ptTERC numeric(19,4),
		  @ptEDUC_FE numeric(19,4),
		  @ptEDUC_DEC numeric(19,4),
          @ptIRFONTE numeric(19,4),
		  @ptMULTA_FGTS numeric(19,4),
		  @ptid integer;
		 -- @agid integer; 	  
   
	  	 DECLARE @fator_Pivot int; 

	DECLARE cursor_obrigacao CURSOR FOR   
      SELECT trab,id,SALARIO,VLR_HXN,GRATIF,VLR_HXS,VLR_HXA,  FGTS,SALFAM,INSS,FERIAS,DECIMO,FGTS_FE,
	     FGTS_DEC,EDUC,TERC,EDUC_FE,EDUC_DEC,IRFONTE,MULTA_FGTS from @CUSTOFOLHA order by trab;
	  OPEN cursor_obrigacao;
	    FETCH NEXT  FROM cursor_obrigacao INTO
	       @agtrab, @agid,@agSALARIO,@agVLR_HXN,@agGRATIF,@agVLR_HXS,@agVLR_HXA,  @agFGTS,@agSALFAM,@agINSS,@agFERIAS,@agDECIMO,@agFGTS_FE,
	     @agFGTS_DEC,@agEDUC,@agTERC,@agEDUC_FE,@agEDUC_DEC,@agIRFONTE, @agMULTA_FGTS;
     WHILE (@@FETCH_STATUS = 0)
	 begin
	     SET @pttrab =  @agtrab;
		 
	     SELECT @SALARIO = (SALARIO+VLR_HXS +VLR_HXA+ VLR_HXN+GRATIF), @FGTS = FGTS,  @SALFAM =SALFAM, @INSS=INSS,
		  @FERIAS=FERIAS, @DECIMO=DECIMO, @FGTS_FE = FGTS_FE , @FGTS_DEC = FGTS_DEC, @EDUC = EDUC, @TERC = TERC ,
		  @EDUC_FE = EDUC_FE, @EDUC_DEC = EDUC_DEC,
	      @IRFONTE=IRFONTE , @MULTA_FGTS=MULTA_FGTS FROM @FOLHA WHERE TRAB=@pttrab; 	 
	     
		 --SELECT @ptdiarias = SUM(SALARIO+VLR_HXS+VLR_HXA+VLR_HXN+GRATIF) FROM @CUSTOFOLHA WHERE TRAB=@agtrab;
	    -- SET 
		   
		 SET @fator_Pivot = 0;
		 WHILE (@@FETCH_STATUS = 0) and (@pttrab =  @agtrab)
         BEGIN  
			-- REVISAO DE 2019 MARÇO
		     SET @fator = 0;
		     SET @ptsalario = @agSALARIO+@agVLR_HXN+@agGRATIF+@agVLR_HXS+@agVLR_HXA;
			 if (@SALARIO <> 0) and (@ptsalario<>0)
			 begin
		         SET @fator =  @ptsalario/@SALARIO;
			 END
			 ELSE 
			 IF (@FERIAS <> 0) and (@fator_PIVOT =  0) 
			 BEGIN
			     SET @fator = 1;
				 SET @fator_PIVOT = 1;
				 IF (@ptsalario=0)
				 BEGIN
				   SET @fator = 0;
				 END;
			 END;
			 IF @FATOR <> 0
			 BEGIN
				SET @ptid = @agid; 
			
				SET @ptfgts = iif(@FGTS<>0,ROUND(@fator*@FGTS,2),0); 	    
				SET @ptsalfam = iif(@SALFAM<>0, ROUND(@fator*@SALFAM,2),0); 	    
				SET @ptirfonte = iif(@IRFONTE<>0, ROUND(@fator*@IRFONTE,2),0); 	    
				SET @ptinss = iif(@INSS<>0,ROUND(@fator*@INSS,2),0); 	    
				SET @ptEDUC = iif(@EDUC<>0,ROUND(@fator*@EDUC,2),0); 	    
				SET @ptTERC = iif(@TERC<>0,ROUND(@fator*@TERC,2),0); 	    
				
				SET @ptFERIAS = iif(@FERIAS<>0,ROUND(@fator*@FERIAS,2),0); 	    
				SET @ptDECIMO = iif(@DECIMO<>0,ROUND(@fator*@DECIMO,2),0); 	    
				SET @ptFGTS_FE = iif(@FGTS_FE<>0,ROUND(@fator*@FGTS_FE,2),0); 	    
				SET @ptFGTS_DEC = iif(@FGTS_DEC<>0,ROUND(@fator*@FGTS_DEC,2),0); 	    
				SET @ptEDUC_FE = iif(@EDUC_FE<>0,ROUND(@fator*@EDUC_FE,2),0); 	    	
				SET @ptEDUC_DEC = iif(@EDUC_DEC<>0,ROUND(@fator*@EDUC_DEC,2),0); 	    	
				
				SET @ptMULTA_FGTS = iif(@MULTA_FGTS<>0,ROUND(@fator*@MULTA_FGTS,2),0); 	    	
		  
                
	            		
			
				UPDATE @CUSTOFOLHA SET FGTS=iif(FGTS is null,0,FGTS) +@ptfgts,SALFAM=iif(SALFAM is null,0,SALFAM)+@ptsalfam,
				  IRFONTE = iif(IRFONTE is null,0,IRFONTE)+@ptIRFONTE,
				         INSS =iif(INSS is null,0,INSS)+ @ptINSS,EDUC=iif(EDUC is null,0,EDUC)+@ptEDUC, 
						 TERC =iif(TERC is null,0,TERC)+ @ptTERC,
						 FERIAS =iif(FERIAS is null,0,FERIAS)+ @ptFERIAS,
						 DECIMO =iif(DECIMO is null,0,DECIMO)+ @ptDECIMO,
						 FGTS_FE =iif(FGTS_FE is null,0,FGTS_FE)+ @ptFGTS_FE,    
						 FGTS_DEC =iif(FGTS_DEC is null,0,FGTS_DEC)+ @ptFGTS_DEC,
						 EDUC_FE =iif(EDUC_FE is null,0,EDUC_FE)+ @ptEDUC_FE,    
						 EDUC_DEC =iif(EDUC_DEC is null,0,EDUC_FE)+ @ptEDUC_DEC,
						 MULTA_FGTS =iif(MULTA_FGTS is null,0,MULTA_FGTS)+ @ptMULTA_FGTS
						 where (id = @ptid);  
			 END;
			  FETCH NEXT  FROM cursor_obrigacao INTO
	         @agtrab, @agid,@agSALARIO,@agVLR_HXN,@agGRATIF,@agVLR_HXS,@agVLR_HXA,  @agFGTS,@agSALFAM,@agINSS,@agFERIAS,@agDECIMO,@agFGTS_FE,
	        @agFGTS_DEC,@agEDUC,@agTERC,@agEDUC_FE,@agEDUC_DEC,@agIRFONTE, @agMULTA_FGTS;

		 END;


	 end;
	 CLOSE cursor_obrigacao;  
     DEALLOCATE cursor_obrigacao;  
      



      DECLARE @difsalfam numeric(19,4),@difinss numeric(19,4),@difirfonte numeric(19,4),@diffgts numeric(19,4),
	  @difeduc numeric(19,4),@difterc numeric(19,4),
	 
	  @differias numeric(19,4) , @difdecimo numeric(19,4) ,@diffgts_fe numeric(19,4), @diffgts_dec numeric(19,4) ,
	   @difeduc_fe numeric(19,4) , @difeduc_dec numeric(19,4) ,@difmulta_fgts numeric(19,4)   ;



	  DECLARE @tabObrigDif TABLE (trab nvarchar(4),difsalfam numeric(19,4),difinss numeric(19,4),difirfonte numeric(19,4),
	  diffgts numeric(19,4),	difeduc numeric(19,4),difterc numeric(19,4),
	   differias numeric(19,4) , difdecimo numeric(19,4) ,diffgts_fe numeric(19,4), diffgts_dec numeric(19,4) ,
	   difeduc_fe numeric(19,4) , difeduc_dec numeric(19,4) ,difmulta_fgts numeric(19,4)   
	  );
	  
	  
	  INSERT INTO @tabObrigDif

	 SELECT  TRAB, SALFAM- (SELECT SUM(cus.SALFAM) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difsalfam ,
	  INSS- (SELECT SUM(cus.INSS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difINSS,
	  IRFONTE- (SELECT SUM(cus.IRFONTE) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difvlr_IRFONTE,
	  FGTS- (SELECT SUM(cus.FGTS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS,
	  EDUC- (SELECT SUM(cus.EDUC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC,
	  TERC- (SELECT SUM(cus.TERC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difTERC,

	  FERIAS- (SELECT SUM(cus.FERIAS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFERIAS,
	  DECIMO- (SELECT SUM(cus.DECIMO) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difDECIMO,
	  FGTS_FE- (SELECT SUM(cus.fgts_fe) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as diffgts_fe,
	  FGTS_DEC- (SELECT SUM(cus.FGTS_DEC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difFGTS_DEC,
	  EDUC_FE- (SELECT SUM(cus.EDUC_FE) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC_FE,
	  EDUC_DEC- (SELECT SUM(cus.EDUC_DEC) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difEDUC_DEC,
	  MULTA_FGTS- (SELECT SUM(cus.MULTA_FGTS) from @CUSTOFOLHA as cus where (cus.TRAB = fol.TRAB)) as difMULTA_FGTS



	   FROM @FOLHA as fol   order by fol.trab;
	 -- DECLARE @testea int;
	  DECLARE cursor_rateio CURSOR FOR   
      SELECT trab, difsalfam,difinss,difirfonte,
	  diffgts,	difeduc ,	difterc ,
	    differias  , difdecimo  ,diffgts_fe , diffgts_dec  ,
	   difeduc_fe  , difeduc_dec  ,difmulta_fgts
	   from @tabObrigDif order by trab;
	  OPEN cursor_rateio;
	    FETCH NEXT  FROM cursor_rateio INTO
	       @trab, @difsalfam,@difinss,@difirfonte,
        	  @diffgts,	@difeduc,@difterc,  @differias  , @difdecimo  ,@diffgts_fe , @diffgts_dec  ,
	   @difeduc_fe  , @difeduc_dec  ,@difmulta_fgts;
 ;
     WHILE (@@FETCH_STATUS = 0)
	 begin

		if (@difsalfam <> 0) 
           UPDATE @CUSTOFOLHA SET SALFAM = (SALFAm + @difsalfam)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.SALFAM DESC);
		if (@difinss <> 0) 
           UPDATE @CUSTOFOLHA SET INSS = (INSS + @difinss)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.INSS DESC);
		if (@difirfonte <> 0) 
           UPDATE @CUSTOFOLHA SET IRFONTE = (IRFONTE + @difirfonte)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.IRFONTE DESC);
		if (@diffgts <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS = (FGTS + @diffgts)  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.FGTS DESC);
		if (@difeduc <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC = EDUC + @difeduc  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.EDUC DESC);
		if (@difterc <> 0) 
           UPDATE @CUSTOFOLHA SET TERC = TERC + @difterc  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

		if (@differias <> 0) 
		BEGIN
           /*if (@differias < 0)
		   BEGIN
		      SET @testea = @differias;
			  IF @testea < -10
			  BEGIN
			    SET @testea = 0; -- 1689
			  END; 
		   END; */
		   UPDATE @CUSTOFOLHA SET FERIAS = FERIAS + @difFERIAS  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);
        END;
        if (@difDECIMO <> 0) 
           UPDATE @CUSTOFOLHA SET DECIMO = DECIMO + @difDECIMO  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

         if (@difFGTS_FE <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS_FE = FGTS_FE + @difFGTS_FE  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);
		  
		  if (@difFGTS_DEC <> 0) 
           UPDATE @CUSTOFOLHA SET FGTS_DEC = FGTS_DEC + @difFGTS_DEC  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

          if (@difEDUC_FE <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC_FE = EDUC_FE + @difEDUC_FE  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

         if (@difEDUC_DEC <> 0) 
           UPDATE @CUSTOFOLHA SET EDUC_DEC = EDUC_DEC + @difEDUC_DEC  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);

          if (@difMULTA_FGTS <> 0) 
           UPDATE @CUSTOFOLHA SET MULTA_FGTS = MULTA_FGTS + @difMULTA_FGTS  where 
		     ID = (SELECT TOP(1) cus.ID FROM @CUSTOFOLHA  as cus where (cus.trab = @trab) ORDER BY cus.TERC DESC);


		FETCH NEXT  FROM cursor_rateio INTO
		  @trab, @difsalfam,@difinss,@difirfonte,
        	  @diffgts,	@difeduc,@difterc, @differias  , @difdecimo  ,@diffgts_fe , @diffgts_dec  ,
	   @difeduc_fe  , @difeduc_dec  ,@difmulta_fgts;
	       
	 end;
	 CLOSE cursor_rateio;  
     DEALLOCATE cursor_rateio;

    DECLARE @SALARIO_folha numeric(19,4), @HORAS_folha numeric(19,4) , @GRATIF_folha numeric(19,4);
	
	SELECT @SALARIO_folha = SUM(SALARIO),  @HORAS_folha = SUM(VLR_HXS+VLR_HXN+VLR_HXA), @GRATIF_folha = SUM(GRATIF) FROM 
	           NOVACLTFOLHA WHERE DATA = @fim;    

    DECLARE @SALARIO_CUSTO numeric(19,4), @HORAS_CUSTO numeric(19,4) , @GRATIF_CUSTO numeric(19,4);
    SELECT @SALARIO_CUSTO = SUM(SALARIO),  @HORAS_CUSTO = SUM(VLR_HXS+VLR_HXN+VLR_HXA), @GRATIF_CUSTO = SUM(GRATIF) FROM 
	           @CUSTOFOLHA;    

    SET @SALARIO_CUSTO = @SALARIO_CUSTO  - @SALARIO_folha;
    SET @HORAS_CUSTO = @HORAS_CUSTO  - @HORAS_folha;
	SET @GRATIF_CUSTO = @GRATIF_CUSTO -  @GRATIF_folha;
	
	IF (not (@SALARIO_CUSTO < 0.05 and  @SALARIO_CUSTO > -0.05)) and 
	    (not(@HORAS_CUSTO between 0.05 and -0.05)) and (not(@GRATIF_CUSTO between 0.05 and -0.05))
	BEGIN
        IF @SOATUALIZA_CUSTOFOLHA = 1 
		BEGIN
			   -- Só FOI AUTORIZADO A ATUALIZAR O CUSTOFOLHA
			 SET @RETORNO = 9;
	         	 
		END

	END;

	 DELETE CUSTOFOLHA WHERE DATA_FOLHA = @fim;
	
	  SET IDENTITY_INSERT CUSTOFOLHA OFF
	  INSERT INTO CUSTOFOLHA 
	 
	  SELECT 
	      @FIM AS DATA_FOLHA,
		  INICIO ,
		  FIM ,
		  SETOR ,
	      CENTRO ,
		  QUADRA ,
		  TRAB ,
	      OK ,
		  NUM_MOD ,
		  CODSER ,
		  QUANT , 
		  SALARIO , --vlrdiarias
		  VLR_HXS , -- vlrhorasfe
		  VLR_HXA , -- vlrhoras50
	      VLR_HXN ,-- vlrhoras100
		  FGTS ,
		  SALFAM ,
		  INSS ,
		  FERIAS,
		  DECIMO ,
		  FGTS_FE ,
		  FGTS_DEC ,
	      EDUC ,
		  TERC ,
		  EDUC_FE ,
		  EDUC_DEC,
		  MULTA_FGTS ,
		  HX ,
		  HXS , -- vlrhorasfe
		  HXA , -- vlrhoras50
	      HXN ,-- vlrhoras100
	      HEFETIVA ,
	      IRFONTE ,
	      GRATIF ,-- vlrdiarias_adc
	 	  FALTAS ,
		  DOENCA,
		  0 as LINKTAREFA,
		  0 as LINKGRUPO,
		  BANCOHS_E,
		  BANCOHS_C,
		   TAREFA_IND_MES
		  FROM @CUSTOFOLHA	
	   




IF @SOATUALIZA_CUSTOFOLHA = 1 
BEGIN
   -- Só FOI AUTORIZADO A ATUALIZAR O CUSTOFOLHA
   SELECT @RETORNO as RETORNO;
   REtURN;
END
	
      Delete NOVACLTFOLHA where (data = @fim);
      
	  SET IDENTITY_INSERT NOVACLTFOLHA OFF
	  INSERT INTO NOVACLTFOLHA 
	  (
		[DATA] ,
		[SETOR] ,
		[CENTRO] ,
		[QUADRA] ,
		[TRAB] ,
		[TIPO] ,
		[NUM_MOD] ,
		[CODSER] ,
		[QUANT] ,
		[SALARIO] ,
		[VLR_HXS] ,
		[VLR_HXA] ,
		[VLR_HXN] ,
		[FGTS] ,
		[SALFAM] ,
		[INSS] ,
		[FERIAS],
		[DECIMO] ,
		[FGTS_FE] ,
		[FGTS_DEC] ,
		[EDUC] ,
		[TERC] ,
		[EDUC_FE] ,
		[EDUC_DEC] ,
		[MULTA_FGTS] ,
		[HX] ,
		[HXS] ,
		[HXA] ,
		[HXN] ,
		[HEFETIVA] ,
		[PONTO] ,
		[IRFONTE] ,
		[GRATIF] ,
		[FALTAS] ,
		HORAS_DOM ,
		HORAS_FER ,
		VLR_DOM ,
		VLR_FER ,
		TXINSS ,
		IRDEDU ,
		DIAS ,
		ULTPONTO,
		PODEALTERAR 
	)
	
	SELECT @fim ,
		[SETOR] ,
		[CENTRO] ,
		[QUADRA] ,
		[TRAB] ,
		[TIPO] ,
		[NUM_MOD] ,
		[CODSER] ,
		[QUANT] ,
		[SALARIO] ,
		[VLR_HXS] ,
		[VLR_HXA] ,
		[VLR_HXN] ,
		[FGTS] ,
		[SALFAM] ,
		[INSS] ,
		[FERIAS],
		[DECIMO] ,
		[FGTS_FE] ,
		[FGTS_DEC] ,
		[EDUC] ,
		[TERC] ,
		[EDUC_FE] ,
		[EDUC_DEC] ,
		[MULTA_FGTS] ,
		[HX] ,
		[HXS] ,
		[HXA] ,
		[HXN] ,
		[HEFETIVA] ,
		[PONTO] ,
		[IRFONTE] ,
		[GRATIF] ,
		[FALTAS] ,
		HORAS_DOM ,
		HORAS_FER ,
		VLR_DOM ,
		VLR_FER ,
		TXINSS ,
		IRDEDU ,
		DIAS ,
		
			@fim as ULTPONTO,
			1 as PODEALTERAR 
		  	from @FOLHA ;
	
	  SET IDENTITY_INSERT NOVACLTFOLHA ON

	  SELECT @RETORNO as RETORNO;  -- SUCESSO NA ATUALIZAÇAO DA FOLHA E DO CUSTO FOLHA
	  RETURN;

	  */



END;
GO
PRINT 'EC141: Pontos_Mes ok';
GO

-- ===========================================================================
-- CLT_PontoValorNew
-- ===========================================================================
SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO
-- Batch submitted through debugger: [dbo].[CLT_PontoValorNew].sql|7|0|C:\Users\ptost\OneDrive\Documentos\SQL Server Management Studio\GRATIFICAÇÂO_ESPECIAL\[dbo].[CLT_PontoValorNew].sql
-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- em 2023-12-08
-- =============================================
ALTER PROCEDURE [dbo].[CLT_PontoValorNew] @inicio_aux date, @fim_aux date
AS 
 
BEGIN
	--SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	SET LANGUAGE us_english;

	/*DECLARE @inicio_aux date;
	DECLARE @fim_aux date;
	SET @inicio_aux = '2024-09-01'
	SET @fim_aux = '2024-09-30'
	*/


 DECLARE @RATEADA TABLE (
        ID int,
	    trab nvarchar(4),
		setor nvarchar(2)
		,gleba nvarchar(4)
		,bl nvarchar(3)
		,num_mod nvarchar(2)
		,codser nvarchar(3),
	     dias numeric(18,4),
	    diarias numeric(18,4)
		, diarias_n numeric(18,4),
	     horasfe numeric(18,4)
		 ,horasfen numeric(18,4),
	     horas50 numeric(18,4)
		 ,horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	     horas_domn numeric(18,4)

		 ,diasPrevidencia int
		 ,dtadia_min date
		 ,dtadia_max date
		 , ok varchar(1)
		 ,quant numeric(18,4)
		 ,numtarefa int
		, SALARIO numeric(18,4)
	     ,VLR_HXS numeric(18,4)
	     , VLR_HXA numeric(18,4) 
	    , VLR_HXN numeric(18,4)
	   ,GRATIF numeric(18,4)
		 ,FERIAS numeric(18,4)
		,FGTS_FE numeric(18,4)
		,EDUC_FE numeric(18,4)
	     ,DECIMO numeric(18,4)
		,FGTS_DEC numeric(18,4)
		,EDUC_DEC numeric(18,4)
		,FGTS numeric(18,4)
		 , EDUC numeric(18,4)
		 , TERC numeric(18,4)
		, SALFAM numeric(18,4)
		,IRFONTE NUMERIC(18,4)
		,INSS NUMERIC(18,4)
		,DEMITIDO int
		 );--,numtarefa int

IF  NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CUSTOFOLHA]') AND type in (N'U'))
BEGIN
	CREATE TABLE [dbo].[CUSTOFOLHA](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[DATA_FOLHA] [date] NULL,
	[INICIO] [date] NULL,
	[FIM] [date] NULL,
	[SETOR] [nchar](2) NULL,
	[CENTRO] [nchar](4) NULL,
	[QUADRA] [nchar](3) NULL,
	[TRAB] [nchar](4) NULL,
	[OK] [nchar](1) NULL,
	[NUM_MOD] [nchar](2) NULL,
	[CODSER] [nchar](3) NULL,
	[QUANT] [numeric](19, 4) NULL,
	[SALARIO] [numeric](19, 6) NULL,
	[VLR_HXS] [numeric](19, 6) NULL,
	[VLR_HXA] [numeric](19, 6) NULL,
	[VLR_HXN] [numeric](19, 4) NULL,
	[FGTS] [numeric](19, 6) NULL,
	[SALFAM] [numeric](19, 6) NULL,
	[INSS] [numeric](19, 6) NULL,
	[FERIAS] [numeric](19, 6) NULL,
	[DECIMO] [numeric](19, 6) NULL,
	[FGTS_FE] [numeric](19, 6) NULL,
	[FGTS_DEC] [numeric](19, 6) NULL,
	[EDUC] [numeric](19, 6) NULL,
	[TERC] [numeric](19, 6) NULL,
	[EDUC_FE] [numeric](19, 6) NULL,
	[EDUC_DEC] [numeric](19, 6) NULL,
	[MULTA_FGTS] [numeric](19, 6) NULL,
	[HX] [numeric](19, 4) NULL,
	[HXS] [numeric](19, 4) NULL,
	[HXA] [numeric](19, 4) NULL,
	[HXN] [numeric](19, 4) NULL,
	[HEFETIVA] [numeric](19, 4) NULL,
	[IRFONTE] [numeric](19, 4) NULL,
	[GRATIF] [numeric](19, 4) NULL,
	[DIARIAS_ADC] [numeric](19, 4) NULL,
	[FALTAS] INT  NULL,
	[DOENCA] INT NULL
	,[DIASPREVIDENCIA] int NULL
	,dias INT
	,diasferias INT 
	,horas_dom  INT  -- horas feriado que cairam num domingo
	,horas_domn INT 
		
	,[LINKTAREFA] INT NULL,
	[LINKGRUPO]  INT NULL,
	[BANCOHS_E] [numeric](19, 4) NULL,
	[BANCOHS_C] [numeric](19, 4) NULL,
	[TAREFA_IND_MES] [numeric](18, 0) NULL,
	[DEMITIDO] int null
) ON [PRIMARY]

END;

 DECLARE @DIFRATEADA TABLE (
	    trab nvarchar(4),
		SALARIOSDIF numeric(18,4)
		, VLR_HXSDIF numeric(18,4),
	     VLR_HXADIF numeric(18,4)
		 ,VLR_HXNDIF numeric(18,4),
	     GRATIFDIF numeric(18,4)
		 ,FERIASDIF numeric(18,4)
		,FGTS_FEDIF numeric(18,4)
		,EDUC_FEDIF numeric(18,4)
	     ,DECIMODIF numeric(18,4)
		,FGTS_DECDIF numeric(18,4)
		,EDUC_DECDIF numeric(18,4)
	    
		,FGTSDIF numeric(18,4)
		 , EDUCDIF numeric(18,4)
		 , TERCDIF numeric(18,4)
		, SALFAMDIF numeric(18,4)
		,IRFONTEDIF NUMERIC(18,4)
		,INSSDIF NUMERIC(18,4)
		 );




DECLARE @PERIODO TABLE (ANOMES NVARCHAR(6))

 
;WITH cte AS
(
    SELECT dt = DATEADD(DAY, -(DAY(@inicio_aux) - 1), @inicio_aux)
    UNION ALL
    SELECT DATEADD(MONTH, 1, dt)
    FROM cte
    WHERE dt < DATEADD(DAY, -(DAY(@fim_aux) - 1), @fim_aux)
)
INSERT INTO @PERIODO
SELECT substring(convert(varchar, dt, 112),1,6)
FROM cte  

	
	DECLARE @primeirasemana date;

	DECLARE @vet_sabado TABLE(oindcod nvarchar(2));
    DECLARE @i int;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( ' '+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 10)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( '.'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	SET @i = 1;
	WHILE (@i < 5)
	begin
	  insert into @vet_sabado (oindcod)
	  VALUES( 'C'+CAST(@i as NVARCHAR(1) ));
	  SET @i = @i +1;
	end;
	DECLARE @SalMinFgts TABLE ([DATA] date,VALOR numeric(18,4),FGTS numeric(18,4));
	insert @SalMinFgts 
	EXECUTE [dbo].[SalMinFgts]
	        @fim_aux; 
	DECLARE @SalMin numeric(18,4);		
	DECLARE @taxaFGTS numeric(18,4);
	SELECT @SalMin=VALOR,@taxaFGTS=FGTS from @SalMinFgts;


	DECLARE @tabelagratif TABLE (num_mod nvarchar(2),gcodser nvarchar(3),gsetor nvarchar(2),perc_diaria numeric(18,4),
	          vlr_real numeric(18,4),dta_ini date,dta_fim date  );
	insert into @tabelagratif                        
    SELECT NUM_MOD,CODSER,iif(SETOR='','  ',SETOR),[PERCENT],VLRDIARIA,DTA_INI,DTA_FIM FROM GRATIF WHERE 
                (DTA_INI <= @inicio_aux) and ((DTA_FIM >= @fim_aux) 
              or (DTA_FIM is null) ) and 
			  (CODSER is not null) and ([PERCENT] is not null) and (VLRDIARIA is not null);
	

	 DECLARE @tabelaLista TABLE (CODCAD nvarchar(4),NOMECAD nvarchar(60),DEMI date,
     CATEGORIA nvarchar(30),CBO numeric(19,4),MENSALISTA nchar(1),
	 CONTA1 nchar(10),BANCO1 nchar(3),AGENCIA1 nchar(8),CPF nchar(11),END_RUA nvarchar(50) ,
	   END_CID nvarchar(25) ,END_UF nchar(2),END_CEP nchar(10),
       SETOR nchar(2), salbase numeric(18,4),ult_atual date,
       Glecad NVARCHAR(4),admi date,demitido int,admitido int,diastrab int,
         tsal numeric(18,4), soma_perc_conf numeric(18,4),      ndepend int,emgozo int,irdepend int, salcontrib numeric(18,4),prov_ferias numeric (18,4),
           prov_decimo numeric(18,4), inss numeric(18,4), salfam numeric(18,4), FERIAS_BASE numeric(18,4)
		   , FERIAS_MEDIA numeric(18,4)
		   , SALMEDIO_FERIAS numeric(18,4));
     

	
	  DECLARE @TABELAGERAL TABLE   (trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, quant numeric(18,4),ok nvarchar(1), diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  diarias numeric(18,4),horas_efetivas numeric(18,4),horas50 numeric(18,4),horas100 numeric(18,4),horasfe numeric(18,4),compati nvarchar(5)
	  -- acrescentei
	   ,Diarias_adc numeric(18,4),
	  Horas_Dom numeric(18,4)  -- horas feriado que cairam num domingo
	  
	  
	  ) 
	
    
--	TRANSFORMA o CLTPONTO ( 1-registro com dias da semana => nova tabela com 1 registro = 1dia )
 -- tabela geral que transforma o cltponto numa tabela individualizada de evento     
    DECLARE @ndia int;
	DECLARE @dia NVARCHAR(4);
   DECLARE @tabelaprocess TABLE
   ( 
           ID int,
   numtarefa int,
   trab nvarchar(4),setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  semana date, diasemana nvarchar(12), dtadia date,indcod nvarchar(2),
	  Diarias numeric(18,4),
	  Diariasn numeric(18,4),
	  Horasfe numeric(18,4),
	  Horasfen numeric(18,4),
	  Horas50 numeric(18,4),
	  Horas50n numeric(18,4),
	  Horas100 numeric(18,4),
	  horas_efetivas numeric(18,4),
	  Diarias_adc numeric(18,4),
	 
	  compati nvarchar(1),
	  quant  numeric(18,4), 
	  ok nvarchar(1),
	  -- acrescentei
	  Horas_Dom numeric(18,4),  -- horas feriado que cairam num domingo
	  Horas_Domn numeric(18,4) --   
	 
	  );
	  
   DECLARE @tabelaPREMIO TABLE
   ( 
      DATA Date,
	  setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3), trab nvarchar(4),  num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  
	  Valor numeric(18,4),
	  DTA_INI Date, DTA_FIM Date,
	  INCLUIDO int
	  );
	  
  -- Base do Calculo da Folha por TRABALHADOR (agregada) 
      DECLARE @tabelaagregada TABLE (
	    trab nvarchar(4),
		setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3),num_mod nvarchar(2),codser nvarchar(3),
	    dias numeric(18,4),
	    diarias numeric(18,4)
		, diarias_n numeric(18,4),
	     horasfe numeric(18,4)
		 ,horasfen numeric(18,4),
	     horas50 numeric(18,4)
		 ,horas50n numeric(18,4),
	     horas100 numeric(18,4),
     	  horas_efetivas numeric(18,4),
	     diarias_adc numeric(18,4),
	     faltas int,doenca int,diasferias int ,
		  horas_dom numeric(18,4),  -- horas feriado que cairam num domingo
	     horas_domn numeric(18,4)
		 ,diasPrevidencia int
		 , dtadia_min date
		 , dtadia_max date
		 , ok nvarchar(1)
		 , QUANT numeric(18,4)
		,numtarefa int		 );


		DECLARE @tabelaDiferenca TABLE (
	    trab nvarchar(4),
	    diarias numeric(18,4)
		,diariasVirt numeric(18,4)
	     ,horasfe_HXS numeric(18,4)
		  ,horasfe_HXS_Virt numeric(18,4)
		 
	     ,horas50_HXA numeric(18,4)
		 ,horas50_HXA_Virt numeric(18,4)
		 
		 );
	  DECLARE @VALORporTRAB TABLE (
	    TRAB nvarchar(4),
	   SALARIO numeric(18,4)
	   ,GRATIF numeric(18,4)
	   ,VLR_HXS numeric(18,4)
	    , VLR_HXA numeric(18,4) 
	    , VLR_HXN numeric(18,4)
	
		 ,HX numeric(18,4)
		,HXS numeric(18,4)
	    , HXA numeric(18,4) 
	    , HXN numeric(18,4)
		, DIARIASADC numeric(18,4) 
		,FERIAS numeric(18,4)
		,FGTS_FE numeric(18,4)
		,EDUC_FE numeric(18,4)
	     ,DECIMO numeric(18,4)
		,FGTS_DEC numeric(18,4)
		,EDUC_DEC numeric(18,4)
	    
		,FGTS numeric(18,4)
		 , EDUC numeric(18,4)
		 , TERC numeric(18,4)
		, SALFAM numeric(18,4)
		,IRFONTE NUMERIC(18,4)
		,INSS NUMERIC(18,4)
		,DEMITIDO int
		);

  	DECLARE @MOVFIN TABLE 
	(  
	 DATA date,
	 DEBITO nvarchar(25)
	,CREDITO nvarchar(25)
	,VALOR NUMERIC(19,2)
	,DOC nvarchar(25)
	,HIST nvarchar(50)
	)

 
DECLARE cursor_mst CURSOR FOR   
SELECT ANOMES FROM @PERIODO
--SELECT DISTINCT substring(convert(varchar, dtadia, 112),1,6) from CLTPONTO order by substring(convert(varchar, dtadia, 112),1,6);

DECLARE @inicioMes date; 
DECLARE @fimMes date;
DECLARE @mesAno nvarchar(8); 
OPEN cursor_mst;
FETCH NEXT  FROM cursor_mst INTO
	@mesAno;
WHILE @@FETCH_STATUS = 0  
BEGIN   
     SET @mesAno = @mesAno + '01';  
	 SET @inicioMes = TRY_CONVERT(DATE,@mesAno,112); 
     SET @fimMes = DATEADD(dd,-1,DATEADD(mm,1,@inicioMes));
	 
	SET @primeirasemana = @inicioMes;
	WHILE (DATENAME(weekday,@primeirasemana) <> 'Thursday') 
	begin
      	 SET @primeirasemana= DATEADD(dd,-1, @primeirasemana);
	end;
	DELETE FROM @TABELAGERAL
	set @ndia = 0;
	while (@ndia < 7) 
	begin
	    set @dia ='DIA'+CONVERT(nvarchar,(@ndia+1),1);
	    DECLARE @literal as varchar(5120);
	    SET @literal =
	    'select TRAB,SETOR,fazmov,BL,NUM_MOD,CODSER,cltponto.noturno as noturno,Data as Semana,quant,ok,'+
        'DATENAME(weekday, DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA))   as diasemana,'+
       'DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) as dtadia, '+@dia+  ', '+--cdg.indcod,'+
       'cdg.fator as Diarias,cdg.HORAS as Horas_efetivas,cdg.X_HORAS as Horas50,cdg.NOTURNO as Horas100,cdg.H_NORMAL as Horasfe,cdg.COMPATI'+--,cdg.DESCCOD'+
       ', 0 Diarias_adc ,  0 Horas_Dom '+
	   ' from cltponto LEFT OUTER JOIN CLTCODIG as cdg '+
         'on ( cdg.INDCOD = '+@dia+')  WHERE (CLTPONTO.NOTURNO <> ''P'') AND '+
        '(data between '''+CAST(@primeirasemana as NVARCHAR(12))+''' and '''+CAST(@fimMes as NVARCHAR(12))+''') and ('+@dia+' <> ''  '') and '+ 
       ' (DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) >='''+ CAST(@inicioMes as NVARCHAR(12))+''') and '+
        '(DATEADD(dd, '+CAST(@ndia as nvarchar(1))+', DATA) <= '''+CAST(@fimMes as NVARCHAR(12))+''')'; 
        
		--SELECT @literal
		insert into @TABELAGERAL EXEC(@literal);
        set @ndia = @ndia+1;
    end;

-- Verificar se é necessário	 

-----*** INcluido em nov/2024
 DECLARE @GratEspecial TABLE
   ( 
      DATA Date,
	  setor nvarchar(2),gleba nvarchar(4),bl nvarchar(3), trab nvarchar(4),  num_mod nvarchar(2),codser nvarchar(3),
	  noturno nvarchar(1), 
	  
	  GRATESPECIAL numeric(18,4),
	  
	  INCLUIDO int,
	  numtarefa int
	  );
	  
   INSERT INTO @GratEspecial
   select  DATA, SETOR, FAZMOV, BL, TRAB, NUM_MOD, CODSER, NOTURNO, DIARIA as GratEspecial, 0 as incluido, 0 as numtarefa  from cltponto  
       WHERE (Diaria <> 0) AND YEAR(@fimMes) > '2023' and 
        (data between @inicioMes and @fimMes)
		and (Month(data) = MONTH(@fimMes))

-----*** INcluido em nov/2024	




	 
	 DELETE FROM @SalMinFgts;
	 insert @SalMinFgts 
	 EXECUTE [dbo].[SalMinFgts]
	        @fimMes; 
	 SELECT @SalMin=VALOR,@taxaFGTS=FGTS from @SalMinFgts;  
	 DELETE FROM @tabelaLista; 
     insert into @tabelaLista
       EXECUTE  [dbo].[RetornaListaDados] 
          @inicioMes
          ,@fimMes
          ,@SalMin;

	 DELETE FROM @tabelaprocess
	 INSERT into  @tabelaprocess
	  SELECT   ROW_NUMBER() OVER(ORDER BY trab ,tbger.setor ,gleba ,bl ,num_mod ,codser) as ID,   0, trab ,tbger.setor ,gleba ,bl ,num_mod ,codser ,
	  noturno , 
	  semana , diasemana , dtadia,indcod ,
	  iif(diarias>0,iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiarias,
	 
	 iif(diarias>0 and (noturno='X'),iif( ( (diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and exists(Select sab.oindcod from @vet_sabado as sab where (rtrim(indcod) = rtrim(sab.oindcod)))  ),
		    (diarias * 2),diarias),0) as pDiariasn,
	 
	  iif(horasfe>0,horasfe,0) as pHorasfe,
	  
	  iif((horasfe>0) and (noturno='X'),horasfe,0) as pHorasfen,
	  
	  iif(horas50>0,horas50,0) as pHoras50,
	  iif((horas50>0) and (noturno='X'),horas50,0) as pHoras50n,

	  iif(horas100>0,horas100,0) as pHoras100,
	  
	  iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0,iif( ( indcod = 'X'),horas_efetivas/2, horas_efetivas),horas_efetivas),0) * iif((diarias>0) and (diasemana not in ('Saturday','Sunday')), dbo.fn_FatorHorasEfetivas(dtadia), 1) as horas_efetivas,
	  -- diaria_adc
	  iif(not Exists(Select gcodser from @tabelagratif as grat where ((codser = grat.gcodser) and 
	  ((tbger.setor = grat.gSetor) or (grat.gsetor='')) and (grat.perc_diaria <> 0))),0,
	     iif(horas_efetivas>0,  iif((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=0 and (rtrim(indcod)='X'), 
	       ( (horas_efetivas/2) *  (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser))),
		(horas_efetivas * (Select grat.perc_diaria/100 from @tabelagratif as grat where (codser = grat.gcodser)) )),0)) as diaria_adc

	  ,compati,quant,ok ,

	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0),horasfe,0) as pHoras_Dom,
	  iif( ((diasemana='Sunday') OR ((diasemana='Saturday') AND dbo.fn_SegundoDSRAtivo(dtadia)=1)) and (horasfe>0) and (noturno='X'),horasfe,0) as pHoras_Domn
	 
	  from @TABELAGERAL 
	   as tbger left join @tabelaLista as lst 
	  ON (lst.CODCAD = tbger.trab) 
	     WHERE (dtadia between @inicioMes and @fimMes) and (admi <= dtadia) and ((demi is null) OR (dtadia <= DEMI)) and (noturno <> 'P')  
	
	
	---- IDENTIFICAR AS TAREFAS em 25 de janeiro   
    DECLARE @ID INT,@BL NVARCHAR(3),@NUM_MOD NVARCHAR(2),@CODSER NVARCHAR(3),
	    @DTADIA DATE;
	DECLARE @BL2 NVARCHAR(3),@NUM_MOD2 NVARCHAR(2),@CODSER2 NVARCHAR(3),
	    @DTADIA2 DATE;	
	DECLARE @numTAREFA int;
		--SELECT ID, bl,num_mod,codser,dtadia, diasemana FROM @tabelaprocess WHERE bl <> '' order by bl,num_mod,codser,dtadia
	DECLARE CURSOR_TAREFA CURSOR FOR   
	   SELECT ID, bl,num_mod,codser,dtadia, TRAB FROM @tabelaprocess WHERE bl <> '' order by bl,num_mod,codser,dtadia;
	   
    DECLARE @TRAB_ID nchar(4);
	OPEN CURSOR_TAREFA;
	   FETCH NEXT  FROM CURSOR_TAREFA INTO
			@ID ,@BL ,@NUM_MOD ,@CODSER ,
	        @DTADIA ,@TRAB_ID; 
			SET @BL2 = @BL;
			SET @NUM_MOD2 = @NUM_MOD;  
			SET @CODSER2 = @CODSER;
			
	  SET @numTAREFA = ((YEAR(@inicioMes) - 2000) * 10000000) + (MONTH(@inicioMes)*100000) + 1
	  --SELECT @inicioMes, @numtarefa
	  WHILE @@FETCH_STATUS = 0  
	   BEGIN
	      SET @DTADIA2 = @DTADIA;
		  SET @BL2 = @BL;
		  SET @NUM_MOD2 = @NUM_MOD;  
		  SET @CODSER2 = @CODSER;
		  SET @numTAREFA = @numTAREFA + 1;
	      WHILE (@@FETCH_STATUS = 0) AND (@BL2=@BL) AND (@NUM_MOD2 = @NUM_MOD) and (@CODSER2 = @CODSER)  
		  BEGIN     
		      IF (DATEDIFF(day,@DTADIA2,@DTADIA) > 7)
			  begin
			      SET @numTAREFA = @numTAREFA + 1;
			  end;
			  SET @DTADIA2 = @DTADIA;
			  UPDATE @TABELAPROCESS SET numtarefa = @numTAREFA FROM @TABELAPROCESS where id = @id   
			  if (Exists(Select * from @GratEspecial where (trab = @TRAB_ID) and (@BL2=BL) AND (@NUM_MOD2 = NUM_MOD) 
			  and (@CODSER2 = CODSER) and numtarefa = 0))
			  BEGIN
			      UPDATE @GratEspecial	SET numtarefa = @numTAREFA FROM @GRATESPECIAL
				     where trab = @TRAB_ID and bl = @BL2 and num_mod = @NUM_MOD2 and codser = @CODSER2 and numtarefa = 0
			  END;
			  
			  FETCH NEXT  FROM CURSOR_TAREFA INTO
				@ID ,@BL ,@NUM_MOD ,@CODSER ,
				@DTADIA ,@TRAB_ID;
			END;
	   END;
	   CLOSE CURSOR_TAREFA
	   DEALLOCATE CURSOR_TAREFA
     /* SELECT * FROM @GratEspecial	
	  return*/
	  --- PROCESSO MENSAL 
	  -- ANALISAR A TABELAPROCESSO PARA as QUADRAS <> '' 
	  -- TENTATIVA DE DETERMINAR AS TAREFAS LOGO NO INICIO DO PROCESSO





	  
	  DELETE FROM @tabelaPREMIO
	  INSERT INTO @tabelaPREMIO
       select  DATA, SETOR, FAZMOV, BL, TRAB, NUM_MOD, CODSER, NOTURNO, VALOR, DTA_INI, DTA_FIM, 0 as incluido  from cltponto  
       WHERE (CLTPONTO.NOTURNO = 'P') AND 
        (data between @inicioMes and @fimMes)

	
 	  DELETE FROM @tabelaagregada  
	  INSERT INTO @tabelaagregada
	  SELECT trab,
	  setor ,gleba ,bl ,num_mod ,codser,
	  
	  Round(SUM(Diarias)/8,2),
	  SUM(Diarias) as diarias ,
	  SUM(diariasn) as diarias_n,
	  SUM(horasfe) as horasfe
	  , SUM(horasfen) as horasfen,
	  SUM(horas50) as horas50 
	  ,SUM(horas50n) as horas50n ,
	  SUM(horas100) as horas100 
	  ,SUM(horas_efetivas) as horas_efetivas ,
	  SUM(diarias_adc) as diarias_adc ,
	  SUM(iif(indcod='FI',1,0)) as faltas,
	  SUM(iif(indcod='A',1,0)) as doenca,
	  SUM(iif(indcod='FE',1,0)) as diasferias,
	  SUM(horas_dom) as horas_dom,
	  SUM(horas_domn) as horas_domn 
	  ,SUM(iif(indcod='AD',1,0)) as diasPrevidencia  -- ACRESCENTEI
	  , MIN(dtadia) dtadia_min 
	  , MAX(dtadia) dtadia_max  
	  , MAX(ok) ok 
	  , MAX(QUANT) quant
	  ,numtarefa
	  FROM @tabelaprocess
	    group by trab,setor ,gleba ,bl ,num_mod ,codser,numtarefa order by trab


/*    --SELECT * FROM @tabelaagregada order by bl ,num_mod ,codser,dtadia_min,dtadia_max
	SELECT dtadia_min, DATEDIFF(day,dtadia_min,dtadia_max) dias, COUNT(TRAB) ntrab,MAX(ok) ok,SETOR, bl,
	model.DESCRI ModTIPO,serv.cod COD,serv.DESCRI FROM @tabelaagregada cus 
LEFT JOIN MODELO model ON (model.NUM = cus.NUM_MOD)
LEFT JOIN SERVIC serv ON (serv.COD = CODSER)
where 
model.DESCRI <> '' 
--year(data_folha) = 2023
and bl <> ''
and ((CODSER = ' 65') or (CODSER = ' 66'))
GROUP BY dtadia_min, DATEDIFF(day,dtadia_min,dtadia_max) ,SETOR, BL,model.DESCRI,serv.cod,serv.DESCRI
*/
   -- ** DIFERENÇA DO CLTPONTO processado e o NOVACLTFOLHA existente ---
   	
   DELETE FROM @tabelaDiferenca
   INSERT INTO @tabelaDiferenca
   SELECT TRAB , diarias, diariasVirt, HORASFE_HXS, HORASFE_HXS_virt, HORAS50_HXA,HORAS50_HXA_Virt 
	--, 	(diarias - diariasVirt) as difDiarias 
	--,  (HORASFE_HXS - HORASFE_HXS_virt) as difHorasFe 
  --,(HORAS50_HXA- HORAS50_HXA_Virt) as difHoras50
	FROM
	(SELECT DISTINCT trab,
	CAST((SELECT ROUND(SUM(HX),1) as DIARIAS FROM NOVACLTFOLHA folha where folha.trab = NOVACLTFOLHA.trab and data BETWEEN @inicio_aux and @fim_aux) as float)
	as diarias ,
	CAST((SELECT  ROUND(SUM(ag.diarias),1)  FROM @tabelaagregada ag where ag.trab = NOVACLTFOLHA.trab
	) as float) as diariasVirt
	,CAST((SELECT SUM(HXS) as DIARIAS FROM NOVACLTFOLHA folha where folha.trab = NOVACLTFOLHA.trab and data BETWEEN @inicio_aux and @fim_aux) as float)
	as HORASFE_HXS ,
	CAST((SELECT  ROUND(SUM(ag.horasfe),1)  FROM @tabelaagregada ag where ag.trab = NOVACLTFOLHA.trab
	) as float)  as HORASFE_HXS_virt

	,CAST((SELECT ROUND(SUM(HXA),1) as HORAS50_HXA FROM NOVACLTFOLHA folha where folha.trab = NOVACLTFOLHA.trab and data BETWEEN @inicio_aux and @fim_aux) as float)
	as HORAS50_HXA ,
	CAST((SELECT  ROUND(SUM(ag.horas50),1)  FROM @tabelaagregada ag where ag.trab = NOVACLTFOLHA.trab
	) as float)  as HORAS50_HXA_Virt
		FROM NOVACLTFOLHA
	WHERE DATA BETWEEN @inicio_aux and @fim_aux
	) as foo 
	LEFt JOIN CLTCAD ON (cltcad.CODCAD = trab)
	WHERE 
	(ABS(diarias - diariasVirt) > 0.01) or  ABS(HORASFE_HXS - HORASFE_HXS_virt) > 0.01 
	or
	ABS(HORAS50_HXA- HORAS50_HXA_Virt) > 0.01



  /***  RATEIO DOS VALORES DO NOVACLTFOLHA (oficial) para os lançamentos  ****/
  

  DELETE FROM @VALORporTRAB
/*  INSERT INTO @VALORporTRAB	
  SELECT TRAB
    ,CAST(SUM(ROUND(SALARIO,2)) as float) SALARIO 
	,CAST(SUM(ROUND(GRATIF,2)) as float) GRATIF
	,CAST(SUM(ROUND(VLR_HXS,2)) as float) VLR_HXS 
	,CAST(SUM(ROUND(VLR_HXA,2)) as float) VLR_HXA
	,CAST(SUM(ROUND(VLR_HXN,2)) as float) VLR_HXN
	,CAST(SUM(ROUND(SALARIO,2)+ROUND(GRATIF,2)+ROUND(VLR_HXS,2)+ROUND(VLR_HXA,2)+ROUND(VLR_HXN,2) ) as FLOAT) TOT
		 ,CAST(SUM(ROUND(val.FERIAS,2)+ROUND(val.FGTS_FE,2)+ROUND(val.EDUC_FE,2)) as float) PROV_FERIAS  
         ,CAST( SUM(ROUND(val.DECIMO,2)+ROUND(val.FGTS_DEC,2)+ROUND(val.EDUC_DEC,2))  as float) PROV_DECIMO
	     ,CAST(SUM(ROUND(val.FGTS,2)) as float) IMP_FGTS
		 ,CAST(SUM(ROUND(val.EDUC,2) + ROUND(val.TERC,2)) as float) IMP_INSS
		  , 0 HX
		 , 0 HXS
		 , 0 HXA
		 , 0 HXN
		 , 0 DIARIASADC
		 ,SUM(ROUND(val.FERIAS,2)) FERIAS 
		,SUM(ROUND(FGTS_FE,2)) FGTS_FE  
		,SUM(ROUND(EDUC_FE,2)) EDUC_FE 
	     ,SUM(ROUND(val.DECIMO,2)) DECIMO  
		,SUM(ROUND(FGTS_DEC,2)) FGTS_DEC  
		,SUM(ROUND(EDUC_DEC,2)) EDUC_DEC 
		,SUM(ROUND(val.FGTS,2)) FGTS  
		 , SUM(ROUND(EDUC,2)) EDUC 
		 ,SUM( ROUND(TERC,2)) TERC 
		, SUM(ROUND(SALFAM ,2)) SALFAM
		,SUM(ROUND(IRFONTE,2)) IRFONTE
		,SUM(ROUND(val.INSS,2)) INSS
	FROM NOVACLTFOLHA as val
	LEFT JOIN @FAZTRABALHO trab on (trab.codfaz = val.CENTRO)
	
	LEFT JOIN PLACON ON (placon.NUMCONTA = trab.mao_obra)
    WHERE  
		format(val.data,'yyyyMM') BETWEEN format(@inicio_aux,'yyyyMM') and format(@fim_aux,'yyyyMM')
		 and not exists(Select codcad,demi,admi from cltcad  
				where  (val.TRAB = cltcad.CODCAD) and (demi is not null)  and   (demi <= val.data)) 
     
	GROUP BY TRAB --, substring(CENTRO,1,2),MAO_OBRA
	ORDER BY TRAB
	*/
  
  INSERT INTO @VALORporTRAB	
  SELECT TRAB
    ,CAST(SUM(ROUND(SALARIO,4)) as float) SALARIO 
	,CAST(SUM(ROUND(GRATIF,4)) as float) GRATIF
	,CAST(SUM(ROUND(VLR_HXS,4)) as float) VLR_HXS 
	,CAST(SUM(ROUND(VLR_HXA,4)) as float) VLR_HXA
	,CAST(SUM(ROUND(VLR_HXN,4)) as float) VLR_HXN
		  , 0 HX
		 , 0 HXS
		 , 0 HXA
		 , 0 HXN
		 , 0 DIARIASADC
		 ,SUM(ROUND(val.FERIAS,4)) FERIAS 
		,SUM(ROUND(FGTS_FE,4)) FGTS_FE  
		,SUM(ROUND(EDUC_FE,4)) EDUC_FE 
	     ,SUM(ROUND(val.DECIMO,4)) DECIMO  
		,SUM(ROUND(FGTS_DEC,4)) FGTS_DEC  
		,SUM(ROUND(EDUC_DEC,4)) EDUC_DEC 
		,SUM(ROUND(val.FGTS,4)) FGTS  
		 , SUM(ROUND(EDUC,4)) EDUC 
		 ,SUM( ROUND(TERC,4)) TERC 
		, SUM(ROUND(SALFAM ,4)) SALFAM
		,SUM(ROUND(IRFONTE,4)) IRFONTE
		,SUM(ROUND(val.INSS,4)) INSS
		, 0 demitido
	FROM NOVACLTFOLHA as val
    WHERE  
		format(val.data,'yyyyMM') BETWEEN format(@inicioMes,'yyyyMM') and format(@fimMes,'yyyyMM')
		 and not exists(Select codcad,demi,admi from cltcad  
				where  (val.TRAB = cltcad.CODCAD) and (demi is not null)  and   (demi <= val.data)) 
     
	GROUP BY TRAB
	ORDER BY TRAB
	-- INCLUIR DEMITIDO NO MES
	INSERT INTO @VALORporTRAB	
    SELECT TRAB
    ,CAST(SUM(ROUND(SALARIO,4)) as float) SALARIO 
	,CAST(SUM(ROUND(GRATIF,4)) as float) GRATIF
	,CAST(SUM(ROUND(VLR_HXS,4)) as float) VLR_HXS 
	,CAST(SUM(ROUND(VLR_HXA,4)) as float) VLR_HXA
	,CAST(SUM(ROUND(VLR_HXN,4)) as float) VLR_HXN
		  , 0 HX
		 , 0 HXS
		 , 0 HXA
		 , 0 HXN
		 , 0 DIARIASADC
		 ,SUM(ROUND(val.FERIAS,4)) FERIAS 
		,SUM(ROUND(FGTS_FE,4)) FGTS_FE  
		,SUM(ROUND(EDUC_FE,4)) EDUC_FE 
	     ,SUM(ROUND(val.DECIMO,4)) DECIMO  
		,SUM(ROUND(FGTS_DEC,4)) FGTS_DEC  
		,SUM(ROUND(EDUC_DEC,4)) EDUC_DEC 
		,SUM(ROUND(val.FGTS,4)) FGTS  
		 , SUM(ROUND(EDUC,4)) EDUC 
		 ,SUM( ROUND(TERC,4)) TERC 
		, SUM(ROUND(SALFAM ,4)) SALFAM
		,SUM(ROUND(IRFONTE,4)) IRFONTE
		,SUM(ROUND(val.INSS,4)) INSS
		, 1 demitido
	FROM NOVACLTFOLHA as val
    WHERE  
		format(val.data,'yyyyMM') BETWEEN format(@inicioMes,'yyyyMM') and format(@fimMes,'yyyyMM')
		 and  exists(Select codcad,demi,admi from cltcad  
				where  (val.TRAB = cltcad.CODCAD) and (demi is not null)  and   (format(demi,'yyyyMM') =format(val.data,'yyyyMM'))   )
	GROUP BY TRAB
	ORDER BY TRAB

	UPDATE @VALORporTRAB  
	SET
		HX = foo.totxh
	   ,HXS = foo.totxhs
	   ,HXA = foo.totxha
	   ,HXN = foo.totxhn
	   ,DIARIASADC = totdiariasADC
	FROM
		@VALORporTRAB AS vlrTrab
		INNER JOIN (
		  SELECT TRAB
		  , SUM(diarias) totxh
		  , SUM(horasfe) totxhs 
		  , SUM(horas50) totxha
		  , SUM(horas100+diarias_n+horasfen+horas50n) totxhn
		  , SUM(diarias_adc) totdiariasADC
		  FROM @tabelaagregada
		   GROUP BY trab
		) foo ON (foo.trab = vlrTrab.trab)
	
		/**** Forma do Rateio ****/
	 
	DELETE FROM @RATEADA
	INSERT INTO @RATEADA
	SELECT ROW_NUMBER() OVER(ORDER BY agreg.TRAB) as ID , agreg.*
	, ROUND(iif(vlr.HX = 0,0,(vlr.SALARIO/vlr.HX) * agreg.diarias),2) as SALARIO
	, ROUND(iif(vlr.HXS=0,0,(vlr.VLR_HXS/vlr.HXS) * agreg.horasfe),2) as VLR_HXS
	, ROUND(iif(vlr.HXA=0,0,(vlr.VLR_HXA/vlr.HXA) * agreg.horas50),2) as VLR_HXA
	, ROUND(iif(vlr.HXN=0,0,(vlr.VLR_HXN/vlr.HXN) * (horas100+diarias_n+horasfen+horas50n)),2) as VLR_HXN
	, ROUND(iif(vlr.DIARIASADC=0,0,(vlr.GRATIF/vlr.DIARIASADC) * (agreg.diarias_adc)),2) as GRATIF
	
	, ROUND(iif(vlr.HX = 0,0,(vlr.FERIAS/vlr.HX) * agreg.diarias),2) FERIAS 
	,ROUND(iif(vlr.HX = 0,0,(vlr.FGTS_FE/vlr.HX) * agreg.diarias),2) FGTS_FE 
	,ROUND(iif(vlr.HX = 0,0,(vlr.EDUC_FE/vlr.HX) * agreg.diarias),2) EDUC_FE 
	, ROUND(iif(vlr.HX = 0,0,(vlr.DECIMO/vlr.HX) * agreg.diarias),2) DECIMO 
	,ROUND(iif(vlr.hx = 0,0,(vlr.FGTS_DEC/vlr.HX) * agreg.diarias),2) FGTS_DEC 
	,ROUND(iif(vlr.hx = 0,0,(vlr.EDUC_DEC/vlr.HX) * agreg.diarias),2) EDUC_DEC 
	,ROUND(iif(vlr.hx = 0,0,(vlr.FGTS/vlr.HX) * agreg.diarias),2) FGTS 
	, ROUND(iif(vlr.hx = 0,0,(vlr.EDUC/vlr.HX) * agreg.diarias),2) EDUC 
	 ,ROUND(iif(vlr.hx = 0,0,(vlr.TERC/vlr.HX) * agreg.diarias),2) TERC 
	, ROUND(iif(vlr.hx = 0,0,(vlr.SALFAM/vlr.HX) * agreg.diarias),2) SALFAM 
	,ROUND(iif(vlr.hx = 0,0,(vlr.IRFONTE/vlr.HX) * agreg.diarias),2) IRFONTE 
	,ROUND(iif(vlr.hx = 0,0,(vlr.INSS/vlr.HX) * agreg.diarias),2) INSS
	,vlr.DEMITIDO
	FROM @tabelaagregada agreg
	INNER JOIN 
	@VALORporTRAB vlr ON (vlr.TRAB = agreg.trab) 


	--IF (year(@fimMes) 

	uPDATE @RATEADA SET GRATIF = GRATIF + gratEsp.GratEspecial  
	from @RATEADA rateada
	INNER JOIN @GRATESPECIAL gratEsp  ON (gratEsp.TRAB = RATEADA.TRAB) and (gratEsp.numtarefa = RATEADA.numtarefa) and (gratEsp.numtarefa <> 0)
	


	--select * FROM @rateada rateada where trab= '8913'


	DELETE FROM @DIFRATEADA
	INSERT INTO @DIFRATEADA

	SELECT TRAB 
		, SALARIO -  SALARIORAt SALARIOSDIF
		 , VLR_HXS - VLR_HXSRat VLR_HXSDIF
		 , VLR_HXA - VLR_HXARat VLR_HXADIF
		, VLR_HXN - VLR_HXNRat VLR_HXNDIF 
		,GRATIF - GRATIFRat GRATIFDIF
		   , FERIAS- FERIASRAT FERIASDIF 
		,FGTS_FE - FGTS_FErat FGTS_FEDIF 
		,EDUC_FE - EDUC_FERat EDUC_FEDIF 
	     ,DECIMO - DECIMORAT DECIMODIF 
		,FGTS_DEC - FGTS_DECRat FGTS_DECDIF 
		,EDUC_DEC - EDUC_DECRat EDUC_DECDIF 
		, FGTS - FGTSRat FGTSDIF 
		 , EDUC - EDUCRAT EDUCDIF 
		 , TERC - TERCrat TERCDIF 
		, SALFAM - SALFAMRat SALFAMDIF 
		, IRFONTE - IRFONTERAT IRFONTEDIF 
		,INSS- INSSRat INSSDIF 
		FROM
		(
		SELECT TRAB ,
		CAST((SELECT SUM(SALARIO) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as SALARIORat 
		,SALARIO
		,CAST((SELECT SUM(VLR_HXS) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as VLR_HXSRat 
		, VLR_HXS
		,CAST((SELECT SUM(VLR_HXA) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as VLR_HXARat 
		, VLR_HXA
		,CAST((SELECT SUM(VLR_HXN) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as VLR_HXNRat 
	   , VLR_HXN
	   ,CAST((SELECT SUM(GRATIF) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as GRATIFRat 
		  , GRATIF

       ,CAST((SELECT SUM(FERIAS) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as FERIASRat 
		  , FERIAS

        ,CAST((SELECT SUM(FGTS_FE) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as FGTS_FERat 
		  , FGTS_FE

			  ,CAST((SELECT SUM(EDUC_FE) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
			  as EDUC_FERat 
			  , EDUC_FE
			
			,CAST((SELECT SUM(DECIMO) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as DECIMORat 
		  , DECIMO
		 
		 ,CAST((SELECT SUM(FGTS_DEC) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as FGTS_DECRat 
		  , FGTS_DEC
		  ,CAST((SELECT SUM(EDUC_DEC) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as EDUC_DECRat 
		  , EDUC_DEC

		,CAST((SELECT SUM(FGTS) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as FGTSRat 
		  , FGTS
		  ,CAST((SELECT SUM(EDUC) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as EDUCRat 
		  , EDUC
		  ,CAST((SELECT SUM(TERC) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as TERCRat 
		  , TERC
		  ,CAST((SELECT SUM(SALFAM) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as SALFAMRat 
		  , SALFAM

          ,CAST((SELECT SUM(IRFONTE) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as IRFONTERat 
		  , IRFONTE
		  ,CAST((SELECT SUM(INSS) FROM @RATEADA folha where folha.trab = vlrTrab.trab ) as float)
		  as INSSRat 
		  , INSS
		 FROM @VALORporTRAB vlrTrab
  
		) as foo 
	
	


	

	DECLARE @trab nvarchar(4),
			@SALARIOSDIF numeric(18,4)
			, @VLR_HXSDIF numeric(18,4),
			 @VLR_HXADIF numeric(18,4)
			 ,@VLR_HXNDIF numeric(18,4),
			 @GRATIFDIF numeric(18,4)
			  ,@FERIASDIF numeric(18,4)
		,@FGTS_FEDIF numeric(18,4)
		,@EDUC_FEDIF numeric(18,4)
	     ,@DECIMODIF numeric(18,4)
		,@FGTS_DECDIF numeric(18,4)
		,@EDUC_DECDIF numeric(18,4)
	    
		,@FGTSDIF numeric(18,4)
		 , @EDUCDIF numeric(18,4)
		 , @TERCDIF numeric(18,4)
		, @SALFAMDIF numeric(18,4)
		,@IRFONTEDIF NUMERIC(18,4)
		,@INSSDIF NUMERIC(18,4);
	 DECLARE cursor_trab CURSOR FOR   
	   SELECT TRAB 
		,  SALARIOSDIF
		 , VLR_HXSDIF
		 ,  VLR_HXADIF
		,  VLR_HXNDIF 
		, GRATIFDIF 
		 ,FERIASDIF 
		,FGTS_FEDIF 
		,EDUC_FEDIF 
	     ,DECIMODIF 
		,FGTS_DECDIF 
		,EDUC_DECDIF 
		,FGTSDIF 
		 , EDUCDIF 
		 , TERCDIF 
		, SALFAMDIF 
		,IRFONTEDIF 
		,INSSDIF
		FROM @DIFRATEADA;
	   DECLARE @TOTDIF numeric(18,4) = 0; 
	   OPEN cursor_trab;
	   FETCH NEXT  FROM cursor_trab INTO
			@TRAB ,  @SALARIOSDIF, @VLR_HXSDIF
		 ,  @VLR_HXADIF,  @VLR_HXNDIF, @GRATIFDIF
		  ,@FERIASDIF,@FGTS_FEDIF,@EDUC_FEDIF 
	     ,@DECIMODIF,@FGTS_DECDIF,@EDUC_DECDIF,@FGTSDIF 
		 , @EDUCDIF , @TERCDIF, @SALFAMDIF,@IRFONTEDIF,@INSSDIF
	   WHILE @@FETCH_STATUS = 0  
	   BEGIN    
		  SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by SALARIO desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB 
			) as temp where roworder = 1
			IF (@ID <> 0) and (@SALARIOSDIF <> 0)
			BEGIN
			   SET @TOTDIF = @TOTDIF - @SALARIOSDIF
			-- SALARIO
				UPDATE @RATEADA
				SET SALARIO = SALARIO + @SALARIOSDIF
				FROM @RATEADA rat
				WHERE rat.ID = @ID
			END
			
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by VLR_HXS desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and (@VLR_HXSDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET VLR_HXS = VLR_HXS +  @VLR_HXSDIF
				FROM @RATEADA rat
				where id = @id
			END
			-- VLR_HXA
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by VLR_HXA desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@VLR_HXADIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET VLR_HXA = VLR_HXA +  @VLR_HXADIF 
				FROM @RATEADA rat
				where id = @id
			 END
			-- VLR_HXN
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by VLR_HXN desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
         	
		    IF (@ID <> 0) and  (@VLR_HXNDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET VLR_HXN = VLR_HXN +  @VLR_HXNDIF 
				FROM @RATEADA rat
				where id = @id
			END
			-- GRATIF
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by GRATIF desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@GRATIFDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET GRATIF = GRATIF +   @GRATIFDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
         -- FErIAS
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by FERIAS desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@FERIASDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET FERIAS = FERIAS +   @FERIASDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
      -- FGTS_FE
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by FGTS_FE desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@FGTS_FEDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET FGTS_FE = FGTS_FE +   @FGTS_FEDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
   -- EDUC_FE
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by EDUC_FE desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@EDUC_FEDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET EDUC_FE = EDUC_FE +   @EDUC_FEDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
   
	-- DECIMO
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by DECIMO desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@DECIMODIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET DECIMO = DECIMO +   @DECIMODIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
   -- FGTS_DEC
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by FGTS_DEC desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@FGTS_DECDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET FGTS_DEC = FGTS_DEC +   @FGTS_DECDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
   
	-- EDUC_DEC
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by EDUC_DEC desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@EDUC_DECDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET EDUC_DEC = EDUC_DEC +   @EDUC_DECDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
   	-- FGTS
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by FGTS desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@FGTSDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET FGTS = FGTS +   @FGTSDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END	
			-- EDUC
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by EDUC desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@EDUCDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET EDUC = EDUC +   @EDUCDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END	
			-- TERC
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by TERC desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@TERCDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET TERC = TERC +   @TERCDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
				-- SALFAM
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by SALFAM desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@SALFAMDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET SALFAM = SALFAM +   @SALFAMDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
				-- IRFONTE
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by IRFONTE desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@IRFONTEDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET IRFONTE = IRFONTE +   @IRFONTEDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
				-- INSS
			SET @ID = 0
			SELECT  @ID = temp.ID from (
			select ID,row_number() over(order by INSS desc) as roworder
			from @RATEADA WHERE TRAB = @TRAB
			) as temp where roworder = 1
			IF (@ID <> 0) and  (@INSSDIF <> 0)
			BEGIN
				UPDATE @RATEADA
				SET INSS = INSS +   @INSSDIF
				FROM @RATEADA rat
				WHERE ID = @ID
			END
	
	    FETCH NEXT  FROM cursor_trab INTO
			@TRAB ,  @SALARIOSDIF, @VLR_HXSDIF
		 ,  @VLR_HXADIF,  @VLR_HXNDIF, @GRATIFDIF
		  ,@FERIASDIF,@FGTS_FEDIF,@EDUC_FEDIF 
	     ,@DECIMODIF,@FGTS_DECDIF,@EDUC_DECDIF,@FGTSDIF 
		 , @EDUCDIF , @TERCDIF, @SALFAMDIF,@IRFONTEDIF,@INSSDIF;
	   END;
	   CLOSE cursor_trab;
       DEALLOCATE cursor_trab;   
		
	/*DELETE FROM CLTPONTOVALORNEW where format(DATA,'yyyyMM') = format(@fimMes,'yyyyMM')
    
	INSERT INTO CLTPONTOVALORNEW
	SELECT @fimMes DATA,
	       trab ,
		    setor 
	    	,gleba 
	    	,bl 
		    ,num_mod 
		,codser ,
	     dias ,
	    diarias 
		, diarias_n ,
	     horasfe 
		 ,horasfen ,
	     horas50 
		 ,horas50n ,
	     horas100 ,
     	  horas_efetivas ,
	     diarias_adc ,
	     faltas 
		 ,doenca 
		 ,diasferias ,
		  horas_dom ,  -- horas feriado que cairam num domingo
	     horas_domn 
		 ,diasPrevidencia 
		 ,dtadia_min 
		 ,dtadia_max 
		 , ok 
		, SALARIO 
	     ,VLR_HXS 
	     , VLR_HXA 
	    , VLR_HXN 
	   ,GRATIF 
		 ,FERIAS 
		,FGTS_FE 
		,EDUC_FE 
	     ,DECIMO 
		,FGTS_DEC 
		,EDUC_DEC 
		,FGTS 
		 , EDUC 
		 , TERC 
		, SALFAM 
		,IRFONTE 
		,INSS
		,DEMITIDO
		,0 LINKTAREFA,
		 0 LINKGRUPO ,
  		 0 BANCOHS_E ,
	     0 BANCOHS_C ,
	     0 TAREFA_IND_MES ,
		 diarias HX ,
		 horasfe HXS ,
		 horas50 HXA ,
		(horas100+diarias_n+horasfen+horas50n) HXN ,
  	 horas_efetivas	HEFETIVA ,
		0 MULTA_FGTS 
		


	FROM @RATEADA as rat 
    */

/*
select * FROM @rateada rateada 
inner join @GratEspecial vlr on (vlr.numtarefa = rateada.numtarefa)
where rateada.trab = '8913' 
SELECT * FROM @VALORporTRAB where 
	trab = '8913'
select SUM(SALARIO) sal, SUM(GRATIF) gra , SUM(FERIAS) fe,SUM(DECIMO) de FROM @rateada where 
	trab = '8913'
    

RETURN
	*/
	


	DELETE FROM CUSTOFOLHA where format(DATA_FOLHA,'yyyyMM') = format(@fimMes,'yyyyMM')
    
	INSERT INTO CUSTOFOLHA
	SELECT @fimMes DATA_FOLHA,
	      dtadia_min INICIO
		 ,dtadia_max FIM 
	,SETOR ,
	gleba CENTRO ,
	bl QUADRA ,
	TRAB ,
	OK ,
	NUM_MOD ,
	CODSER ,
	QUANT ,
	SALARIO ,
	VLR_HXS ,
	VLR_HXA ,
	VLR_HXN ,
	FGTS ,
	SALFAM ,
	 
	INSS ,
	FERIAS ,
	DECIMO ,
	FGTS_FE ,
	FGTS_DEC ,
	EDUC ,
	TERC ,
	EDUC_FE ,
	EDUC_DEC ,
	0 MULTA_FGTS ,
	 diarias HX ,
		 horasfe HXS ,
		 horas50 HXA ,
		(horas100+diarias_n+horasfen+horas50n) HXN ,
  	 horas_efetivas	HEFETIVA ,     
		
	IRFONTE ,
	GRATIF ,
	DIARIAS_ADC,
	FALTAS ,
	DOENCA 
		,diasPrevidencia 
		, dias 
	    
	     ,diasferias ,
		  horas_dom ,  -- horas feriado que cairam num domingo
	     horas_domn 
		 
		,0 LINKTAREFA,
		 0 LINKGRUPO ,
  		 0 BANCOHS_E ,
	     0 BANCOHS_C ,
	     rat.numtarefa TAREFA_IND_MES 
		,DEMITIDO
		
	FROM @RATEADA as rat
	
	--SELECT * FROM CUSTOFOLHA where trab = '8913' and format(DATA_FOLHA,'yyyyMM') = format(@fimMes,'yyyyMM')

	   FETCH NEXT  FROM cursor_mst INTO
				@mesAno;
  END;
  CLOSE cursor_mst;  
  DEALLOCATE cursor_mst;  

   RETURN 
END;

/*

SELECT format(DATA,'yyyyMM'), TRAB, CAST(SUM(IRFONTE) as float) salarios  FROM CLTPONTOVALOR
where IRFONTE <> 0

GROUP BY format(DATA,'yyyyMM'), TRAB
ORDER BY format(DATA,'yyyyMM'), TRAB

SELECT format(DATA,'yyyyMM'), TRAB, CAST(SUM(IRFONTE) as float) salarios  FROM NOVACLTFOLHA
WHERE YEAR(DATA) = 2023
and not exists(Select codcad,demi,admi from cltcad  
				where  (NOVACLTFOLHA.TRAB = cltcad.CODCAD) and (demi is not null)  and   (demi <= NOVACLTFOLHA.data)) 
and IRFONTE <> 0     
GROUP BY format(DATA,'yyyyMM'), TRAB
ORDER BY format(DATA,'yyyyMM'), TRAB
*/

/*
SELECT *  FROM CLTPONTOVALOR
where format(DATA,'yyyyMM') = '202305' and trab = '8800'

SELECT *  FROM NOVACLTFOLHA
where format(DATA,'yyyyMM') = '202305' and trab = '8800'
*/
GO
PRINT 'EC141: CLT_PontoValorNew ok';
GO

SET NOEXEC OFF;
GO
-- ---------------------------------------------------------------------------
-- Conferencia final (so leitura)
-- ---------------------------------------------------------------------------
SELECT dbo.fn_DivisorHorasExtras('2011-11-30') AS pre_2012_240, dbo.fn_DivisorHorasExtras('2026-12-15') AS ultimo_dia_220,
       dbo.fn_DivisorHorasExtras('2026-12-16') AS primeiro_dia_210,  dbo.fn_DivisorHorasExtras('2027-12-15') AS ultimo_dia_210,
       dbo.fn_DivisorHorasExtras('2027-12-16') AS primeiro_dia_200,
       dbo.fn_SegundoDSRAtivo('2026-12-15')    AS dsr2_antes_0, dbo.fn_SegundoDSRAtivo('2026-12-16') AS dsr2_depois_1,
       dbo.fn_FatorHorasEfetivas('2026-12-15') AS fator_antes_1, dbo.fn_FatorHorasEfetivas('2026-12-16') AS fator_1ano_1_05,
       dbo.fn_FatorHorasEfetivas('2027-12-16') AS fator_depois_1;
SELECT o.name, CONVERT(varchar(19), o.modify_date, 120) AS alterada, m.uses_ansi_nulls, m.uses_quoted_identifier,
       CASE WHEN o.type = 'FN' THEN 'funcao EC141'
            WHEN m.definition LIKE '%fn_SegundoDSRAtivo%' THEN 'com patch' ELSE 'SEM patch' END AS situacao
  FROM sys.objects o JOIN sys.sql_modules m ON m.object_id = o.object_id
 WHERE o.name IN ('FolhaClt_SoLeitura','FolhaClt_Atualiza','FolhaClt','Pontos_Mes','CLT_PontoValorNew','fn_DivisorHorasExtras','fn_SegundoDSRAtivo','fn_FatorHorasEfetivas')
 ORDER BY o.type, o.name;
GO