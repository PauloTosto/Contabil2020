# ApoioContabil2020

App de **apoio à contabilidade** que serve como ponte entre dois mundos:

1. **Origem (legado)**: tabelas DBF geradas por um sistema DOS antigo, ainda em uso
   pelo escritório. Lidas via `VFPOLEDB.1` (driver Visual FoxPro, 32-bit only).
2. **Destino**: o sistema **AlterData** (contabilidade de terceiros). A
   transferência é feita gerando uma **planilha XLS no formato de import do
   AlterData** — não é integração programática.

A **Contabilidade** é o módulo principal. Os outros (`Trabalho`, `Fiscal`,
`Almoxarifado`, `Produção`, `Financeiro`, `PagarReceber`) alimentam o ledger DBF
ou geram dados auxiliares.

## Stack

- **.NET Framework 4.8** + Windows Forms (não é .NET Core/8).
- C# `LangVersion=7.3` em algumas configs (legado).
- **EPPlus 4.5.3.3** (LGPL) — gera/lê todos os XLSX. App **não depende mais** de Excel instalado. `Microsoft.Office.Interop.Excel` foi totalmente removido em 2026-05.
- **Leitura de planilha** (RELACIONA Excel) usa `Microsoft.ACE.OLEDB.12.0` (precisa `AccessDatabaseEngine_X86.exe` instalado quando o app roda em x86).
- `VFPOLEDB.1` (COM) — exige build x86 ou ambiente com driver 32-bit.
- HTTP API local em `http://<servidor>:5000/api/SelectML/...` (out-of-tree).
- `packages.config` (NuGet legado, não `PackageReference`).

## Solution & projetos

`ApoioContabil2020.sln` contém:

| Projeto | Tipo | Papel |
|---|---|---|
| `ApoioContabilidade` | WinExe | Entry point (`FrmMenu`), forms, services |
| `ClassConexao` | DLL | Pool de conexões (OleDb / Sql / Odbc); `TDataControlReduzido` |
| `ClassFiltroEdite` | DLL | UI utilitária (filtros, edição de grids, `MonteGrid`) |
| `ClassLibTrabalho` | DLL | Domínio de folha/CLT/ponto |
| `ClassApropriaçoes` | DLL | Apropriações (referenciado por `ProjectReference`) |

`ApiPaceiro_C/WindowsFormsApp1` existe na árvore mas **não está na .sln** — projeto paralelo, não tocar sem confirmação.

## Modos de operação

`FrmMenu.VerificaServidores()` ([FrmMenu.cs:44](ApoioContabilidade/FrmMenu.cs:44)) lê `ConfigPath.txt` em `LocalApplicationData` e descobre dois caminhos:

- `CONTAB=<dir>` → onde estão as DBFs (`sucesso = true` se acessível)
- `SERVIDOR_IP=<host>` → onde a API HTTP responde (`sucessoServidor = true`)

Visibilidade do menu depende dessas duas flags:

| `sucesso` | `sucessoServidor` | Menus visíveis |
|---|---|---|
| ✓ | ✗ | Só "Modo Contab" (DBF local apenas) |
| ✗ | ✓ | Só "Modo Servidor SQL" |
| ✓ | ✓ | Tudo |

Se ambos falham, abre `FrmWinConfigura` para o usuário ajustar.

## Pipeline DBF ↔ API ↔ AlterData

```
SQL Server (out-of-tree)         App                          DBFs (CONTAB)         AlterData
       │  ┌──── HTTP API :5000 ────┘                                │
       │  │                                                         │
       │  │  Pipeline 1: importa para MOVFIN.DBF                    │
       │  │   FrmTransfereContabilidade  (SP 602) → MOVFIN.DBF      │
       │  │   FrmDadosContabilidade      (SP 601) → MOVFIN.DBF      │
       │  ▼                                                         │
       │  Pipeline 2: mapeamento plano de contas                    │
       │   FrmPreparaPlacon1   → PLACON                             │
       │   FrmRelaciona.btnImporta (Excel "PESQUISA") → RELACIONA.DBF│
       │  ▼                                                         │
       │  Pipeline 3: export AlterData                              │
       │   FrmRelaciona escolhe ano → lê PTMOVFIN<ano>.DBF          │
       │     PesquiseRazao() junta com RELACIONA, traduz NUMCONTA   │
       │     → NOVOCOD ou REDUZIDO                                  │
       │   btnExporta → ExportaLoteExcel → .xls ─────────────────────►
       │                                                            
       └─ Auxiliares: FrmBalancete, FrmBalancinho, FrmRazao*, FrmSdoPlano
```

### DBFs principais

| Tabela | Conteúdo |
|---|---|
| `MOVFIN.DBF` | Ledger "ao vivo". Colunas: `DATA, DEBITO, CREDITO, VALOR, TIPO, TP_FIN, DOC, HIST, DOC_FISC, FORN, EMISSOR, DATA_EMI, MOV_ID, OUTRO_ID, VENC, DOC_DUPL, OBS, NHIST` |
| `PTMOVFIN<ano>.DBF` | Snapshot/cópia do MOVFIN por ano fiscal. Lido pelo exportador AlterData. Nome do arquivo carrega o ano (`PTMOVFIN2024.DBF`). |
| `PTPLA<ano>.DBF` | Saldos do plano de contas no fim do ano `<ano>`. Necessário para o balancete do ano seguinte (cálculo de saldo anterior). |
| `RELACIONA.DBF` | De-para `NUMCONTA → NOVOCOD`. Colunas: `NUMCONTA, NOVOCOD, NOVADESC, DESCRICAO, REDUZIDO, NREG`. **NUMCONTA precisa ser único.** |
| `PLACON` | Plano de contas (preparado por `FrmPreparaPlacon1`). |

### API HTTP

Base: `http://<SERVIDOR_IP>:5000`

- `GET /api/SelectML/CSharp?query=<sqlGeral>` — `Api_QueryMulti` ([ServicosApi.cs](ApoioContabilidade/Services/ServicosApi.cs))
- `GET /api/SelectML/SP?query=<paramsSeparadosPorPV>` — `Api_QuerySP` (chama stored procedures)
- `POST /api/SelectML/editeCSharp?modo=inclue|altera` — `PostApi`

Stored procedures conhecidas:

| `sp_numero` | Nome | Uso |
|---|---|---|
| `601` | `Finan_Movs` | Movimentos financeiros num intervalo de datas (FrmDadosContabilidade) |
| `602` | `COMPATIBILIDADE_CUSTOS_CONTABIL` | Folha + saídas almoxarifado num intervalo de meses (FrmTransfereContabilidade). Retorna 2 datasets por mês: FORMATO 0 = INSUMOS, FORMATO 1 = CUSTOFOLHA |

Outros projetos consomem a mesma API — **não mudar contrato sem coordenar com servidor**.

## Convenções importantes

- **Tag de origem em MOVFIN.DOC** identifica de onde o lançamento veio:
  - `'SIST_RURAL NW '` → folha (SP 602, FORMATO 1)
  - `'SIST_ALMOX NW '` → insumos almoxarifado (SP 602, FORMATO 0)
  - `'SIST_BAL'` → fechamento de balanço
- **Excel para alimentar RELACIONA**: a planilha *dentro* do .xlsx **precisa se chamar `PESQUISA`**. Outras nomenclaturas são rejeitadas.
- **Código histórico no export AlterData é fixo `98`** ([ExportaLoteExcel.cs:203](ApoioContabilidade/Excel/ExportaLoteExcel.cs:203)).
- **Modo de export padrão é `REDUZIDO`** (radio `rbDesc2` em FrmRelaciona). Os outros (`rbNumconta`, `rbContabilidade`/`novocod`) são alternativas/debug.
- **Layout do .xls AlterData**: 10 colunas A-J (`lancto auto, debito, credito, data, valor, cód histórico, complemento historico, Ccusto debito, Ccusto credito, NrDocumento`).
- **`MAX(MOV_ID) + 1`** é como novos lançamentos pegam ID na DBF. Assume single-user.
- **`ConfigPath.txt`** vive em `Environment.SpecialFolder.LocalApplicationData` (não em `app.config`).

## Áreas sensíveis (cuidado ao mexer)

1. **Falha de comunicação com a API é silenciada** em [FrmTransfereContabilidade.cs:104-115](ApoioContabilidade/Contabilidade/FrmTransfereContabilidade.cs:104) e [FrmDadosContabilidade.cs:51-57](ApoioContabilidade/Contabilidade/FrmDadosContabilidade.cs:51). Usuário pode confundir "vazio" com "erro" e disparar transferência que apaga MOVFIN do mês sem repor nada.
2. **Conta sem mapeamento em RELACIONA vira `"-1"` no lote** ([FrmRelaciona.cs:480-487](ApoioContabilidade/FrmRelaciona.cs:480)). Hoje é detectado só pelo botão `btnUnicas` (manual). Idealmente bloquear o export.
3. **Race em `MAX(MOV_ID)`** — sem transação. Se duas pessoas gravarem ao mesmo tempo, IDs colidem.
4. **`MessageBox` no `ServicosApi`** — service layer acoplada à UI. Há tarefa separada planejada pra isolar.
5. **Senhas/credenciais** — não há nenhuma hardcoded hoje (a do Firebird foi removida). Manter assim. Para o futuro, usar `app.config` criptografado ou variáveis de ambiente.

## Forms desligados / em desuso

- O caminho de import de `RELACIONA.DBF` é exclusivamente via `FrmRelaciona.btnImporta_Click` ([FrmRelaciona.cs](ApoioContabilidade/FrmRelaciona.cs)) — abrir o Excel com a planilha `PESQUISA` dentro.
- O submenu **Cadastro de Contas** (`cadastroDeContasToolStripMenuItem`) está vazio e com `Visible=false` desde a remoção do `FrmPlaconRelaciona`. Pode ser removido em limpeza futura.

## Templates Excel embedded

- `ApoioContabilidade/Trabalho/Padrao_Excel/Modelo recibo de pagamento de salario.xlsx` — usado por `FrmRelTrabalhista.btnRecibos_Click` via `Properties.Resources.Modelo_recibo_de_pagamento_de_salario` (byte[]). Carregado em runtime via EPPlus, clonado N vezes (uma por funcionário) e preenchido. Layout do recibo: 2 vias verticalmente, via 2 começa 39 linhas abaixo da via 1 — ver helper `PonhaNoReciboExcel`.

## Build

- IDE: Visual Studio 2026 Community (`MSBuild` em `C:\Program Files\Microsoft Visual Studio\18\Community\MSBuild\Current\Bin\MSBuild.exe`).
- A pasta `packages/` é gitignored — primeira build precisa de NuGet restore.
- Plataforma: `Any CPU` mas alguns componentes só funcionam em **x86** (VFPOLEDB, alguns Interops). O warning `MSB3270` sobre `ClassFiltroEdite.dll` ser x86 é pré-existente.
- Para build da CLI no worktree (sem VS aberto), copiar/restaurar `packages/` antes.

## Convenções de código

- Português em nomes (campos, métodos, comentários). Manter.
- Prefixos de classes: `T` (Pascal/Delphi-like), `Frm`, `_MOVFIN` para forms versão DBF, etc.
- `MonteGrid` (de `ClassFiltroEdite`) é a abstração padrão para configurar `DataGridView` — usar quando montar grids.
- Nunca commitar pastas com acento renomeadas sem atualizar a `.sln` e `ProjectReference`s. As pastas `ClassApropriaçoes` e `Produção` estão acentuadas hoje.
