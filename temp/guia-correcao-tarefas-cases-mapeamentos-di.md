# Guia de correção: casos de tarefa, movimentações, mapeamentos e DI

## Como usar este documento

Este guia organiza a correção em fases executáveis, começando pela Fase 1. Ele foi escrito para você aplicar as mudanças manualmente, preservando o padrão de classes `*Case` com `ExecutarAsync` que já começou a usar.

Execute somente uma fase por vez. Cada fase possui um critério de parada. Não remova o caminho antigo antes que os consumidores do novo caminho estejam compilando.

Nesta rodada:

- não serão criados, migrados nem executados testes;
- cada fase será validada por build e buscas estruturais;
- os testes de cada fluxo serão tratados depois, em uma rodada própria;
- não haverá paginação no backend do histórico;
- a paginação visual do histórico permanecerá no grid Angular e passará a operar em memória.

## Resultado esperado

Ao final das fases:

1. `TarefaService` será responsável somente pelo CRUD de tarefa, incluindo as consultas CRUD `ObterTodos` e `ObterPorId`;
2. obtenção, solicitação, decisão, visões contextuais e movimentações serão coordenadas por casos de uso;
3. todos os métodos ainda existentes em `TarefaMovimentacaoService` terão um caso correspondente e consumidores migrados;
4. `ITarefaMovimentacaoService` e `TarefaMovimentacaoService` poderão ser removidos;
5. o endpoint de histórico retornará a coleção completa, ordenada e autorizada, sem parâmetros ou contratos de paginação;
6. o Angular carregará o histórico uma vez e o `MatPaginator` do grid fará a paginação local;
7. os mapeamentos de produção usarão `IMapper<TEntity, TDto>`, `IToDtoMapper<TEntity, TDto>` ou `IToEntityMapper<TEntity, TDto>` da nova família;
8. a família `IAsyncApplicationMapService` será marcada como obsoleta e não terá consumidores de produção ao final;
9. todos os casos de tarefa utilizados estarão registrados na DI do Tracker;
10. Shared, Tracker, Stock, WebApi e Angular compilarão sem executar testes.

## Origem e evidências

### Decisões aceitas nesta conversa

- O backend não será responsável por paginação do histórico.
- O front pode manter paginação quando ela pertence ao grid.
- `TarefaService` deve concentrar somente operações CRUD próprias da tarefa.
- Obtenção, solicitação e movimentação são fluxos de negócio e devem ficar em casos de uso.
- Os casos serão registrados explicitamente na DI.
- O mapeador novo será a família baseada em `IMapper.cs`.
- O mapeador antigo permanecerá temporariamente com `[Obsolete]`, sem novos consumidores.
- Testes serão tratados depois em um fluxo apartado.

### Fatos confirmados no repositório

#### Cobertura atual dos métodos de movimentação

| Método ou responsabilidade atual | Case existente | Consumidor já migrado | Situação |
|---|---|---|---|
| registrar criação | `CriarTarefaMovimentacaoCase` | `CriarTarefaCase` | parcialmente concluído; assinatura e responsabilidades precisam ser padronizadas |
| registrar atualização | `AtualizarTarefaMovimentacaoCase` | não; `AtualizarTarefaCase` ainda usa `ITarefaMovimentacaoService` | pendente |
| registrar obtenção direta | não existe case específico | `AssumirTarefaCase` usa o service | pendente |
| registrar solicitação | não existe case específico | `SolicitarTarefaCase` usa o service | pendente |
| registrar aprovação ou recusa | `RegistrarDecisaoTarefaMovimentacaoCase` | `DecidirTarefaCase` | parcialmente concluído |
| consultar histórico | não existe case específico | controller ainda passa pelo contrato antigo | pendente |
| montar detalhes de atualização | `TarefaMovimentacaoCase.ObterDetalhes` | mapper e service | não é um caso de uso; é transformação auxiliar |

#### Paginação

- `TarefaController` ainda recebe `pagina` e `tamanhoPagina`.
- O front ainda envia esses parâmetros e usa `TarefaMovimentacaoPagina`.
- O histórico Angular possui um `mat-table` e um `mat-paginator` no mesmo componente.
- Dentro do módulo de tarefas, a paginação encontrada pertence a grids.
- O repositório atual já possui `ObterMovimentacoesPorIdAsync(int tarefaId)`, que retorna a coleção ordenada.
- O ADR 0002 ainda declara paginação no servidor e precisa ser atualizado para refletir a nova decisão.

#### Mapeadores

- `IMapper<TEntity, TDto>`, `IToDtoMapper` e `IToEntityMapper` já existem.
- `Mapper<TEntity, TDto>` ainda não implementa formalmente `IMapper<TEntity, TDto>` nem fornece os métodos de coleção exigidos pelas interfaces.
- `TarefaMovimentacaoMapping` já utiliza a nova família e registra a mesma instância para o contrato específico e para `IMapper`.
- `TarefaMapping` ainda utiliza `AsyncApplicationMapService` e mappers filhos antigos.
- A família antiga ainda possui consumidores de produção no Tracker e no Stock.
- Remover ou bloquear a família antiga imediatamente quebraria diversos módulos.

#### DI

Atualmente estão registrados somente:

- `CriarTarefaCase`;
- `DecidirTarefaCase`;
- `RegistrarDecisaoTarefaMovimentacaoCase`.

Existem outros casos já consumidos, mas ainda não registrados.

#### Build

O build integrado mais recente parou primeiro em `EmailValidador`, porque a implementação retorna `IList<NotificationMessage>` enquanto `IValidador<T>` exige `IEnumerable<NotificationMessage>`. Esse bloqueio precisa ser resolvido para que os builds seguintes alcancem o Tracker.

### Inferências usadas no plano

- Mapeamento não realiza I/O no estado atual. Os `Task.FromResult` e `Task.WhenAll` da família antiga não representam assincronicidade necessária.
- O mapper novo deve possuir métodos síncronos e usar composição direta dos mappers filhos.
- Casos de movimentação serão internos à Application. Eles serão consumidos pelos casos principais de tarefa, não expostos como endpoints independentes.
- `ObterHistoricoTarefaCase` é um caso consultivo real porque representa uma ação solicitada pelo usuário e contém autorização, consulta e mapeamento.

### A confirmar durante a execução

- Se todos os repositórios que alimentam `TarefaMapping` carregam as navegações necessárias para usuário, departamento, cargo e estado. O mapper novo não deve consultar banco para compensar um carregamento incompleto.
- Se `ObterTodos` continuará público para qualquer usuário com acesso ao módulo. O guia preserva a rota atual e não altera autorização.
- Se o atributo `[Obsolete]` deverá permanecer apenas como warning ou virar erro em uma rodada futura. Nesta rodada ele será warning.

## Estado atual e estado desejado

### Estado atual

```text
TarefaController
  -> ITarefaService parcialmente reduzido
  -> ITarefaObtencaoService parcialmente conectado

Casos principais
  -> alguns chamam cases de movimentação
  -> outros ainda chamam ITarefaMovimentacaoService

TarefaMovimentacaoService
  -> contém código já migrado
  -> contém código incompleto
  -> mistura escrita e consulta

Mapeamentos
  -> nova e antiga família coexistem
  -> DI possui contratos sem implementação correspondente

Histórico
  -> controller e Angular ainda usam contrato paginado
  -> repositório atual retorna coleção completa
```

### Estado desejado

```text
TarefaController
  -> ITarefaService: CRUD
  -> ITarefaObtencaoService: obtenção e visões contextuais
  -> ITarefaConfiguracoesService: configurações
  -> TarefaEstadoService ou caso equivalente: estados
  -> ObterHistoricoTarefaCase: histórico

CriarTarefaCase
  -> CriarTarefaMovimentacaoCase

AtualizarTarefaCase
  -> AtualizarTarefaMovimentacaoCase

AssumirTarefaCase
  -> RegistrarObtencaoTarefaMovimentacaoCase

SolicitarTarefaCase
  -> RegistrarSolicitacaoTarefaMovimentacaoCase

DecidirTarefaCase
  -> RegistrarDecisaoTarefaMovimentacaoCase

ObterHistoricoTarefaCase
  -> autorização
  -> ITarefaMovimentacaoRepository.ObterMovimentacoesPorIdAsync
  -> IToDtoMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>

Angular
  -> GET /api/Tarefa/{id}/Movimentacoes
  -> TarefaMovimentacao[]
  -> MatTableDataSource + MatPaginator local
```

## Mapa de conhecimento

| Assunto | Nível mínimo | Nível para adaptar | Motivo |
|---|---:|---:|---|
| Use Cases e orquestração | N2 Aplicado | N3 Autônomo | distinguir intenção principal de efeito interno e migrar consumidores sem duplicar fluxo |
| DI do ASP.NET Core | N2 Aplicado | N3 Autônomo | registrar todos os concretos e fechar construtores transitivamente |
| Contratos genéricos de mapping | N2 Aplicado | N3 Autônomo | preservar a ordem `TEntity, TDto` e a direção DTO/entidade |
| Mapeamento de grafos | N2 Aplicado | N3 Autônomo | converter filhos sem reintroduzir I/O dentro do mapper |
| Angular Material table e paginator | N2 Aplicado | N3 Autônomo | mover paginação de servidor para `MatTableDataSource` sem remover o paginator do grid |
| Migração incremental | N2 Aplicado | N4 Arquitetural | manter cada fase compilável e evitar remover contratos antes dos consumidores |

**Ponto de partida observado:** há evidência de N2 Aplicado em Use Cases e mapeamentos, porque já existem casos específicos, contrato de movimentação e testes de mapper preparados. A adaptação do grafo completo ainda não foi validada por build.

## Preparação antes de editar

1. Não limpe nem reverta o worktree. Ele contém mudanças amplas e paralelas.
2. Registre o baseline:

```powershell
git status --short
git branch --show-current
git diff --stat
```

3. Capture as buscas que devem chegar a zero ou a uma lista conhecida:

```powershell
rg -n "ITarefaMovimentacaoService|TarefaMovimentacaoService" AtronPlatform Tests
rg -n "IAsyncApplicationMapService|IAsyncMap<|AsyncApplicationMapService|IMapperEngineService|MapperEngine" AtronPlatform Framework Tests
rg -n "pagina|tamanhoPagina|TarefaMovimentacaoPagina|TarefaMovimentacaoConsulta|ObterPaginaAsync" AtronPlatform/Modules/Tracker AtronPlatform/WebApi AtronFront/src/app/plataforma/tracker/tarefas
rg -n "AddScoped<.*Case" AtronPlatform/Modules/Tracker/Infrastructure/DependencyInjection/TrackerModuleServiceCollectionExtensions.cs
```

4. Use saída alternativa para builds .NET durante toda a migração:

```powershell
dotnet build Framework/Shared/Shared.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\guia-shared\ -p:UseSharedCompilation=false
```

5. Pare a fase quando o primeiro erro não pertencer ao seu escopo. Registre o erro antes de ampliar a alteração.

## Plano de execução

### Fase 1: estabilizar Shared e declarar os contratos da migração

#### Por que esta fase existe

Os projetos consumidores não podem fornecer um build confiável enquanto Shared não compilar e o mapeador novo não tiver um contrato completo. Esta fase cria uma base compilável sem migrar módulos de negócio ainda.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- implementação exata de métodos de interface em C#;
- diferença entre mapper bidirecional e mapper de uma única direção;
- `[Obsolete]` como warning de migração;
- métodos de coleção e extensões LINQ.

#### Arquivos e módulos envolvidos

- `Framework/Shared/Application/Interfaces/Mapping/IMapper.cs`;
- `IToDtoMapper.cs`;
- `IToEntityMapper.cs`;
- `Mapper.cs`;
- `MapperExtensions.cs`;
- família antiga em `Framework/Shared/Application/Interfaces/Service` e `Application/Services/Mapper`;
- `EmailValidador.cs`;
- ADR 0002.

#### Passos

1. Corrigir `EmailValidador.Validar` para retornar exatamente o tipo declarado por `IValidador<EmailRequest>`.
2. Fazer `Mapper<TEntity, TDto>` implementar `IMapper<TEntity, TDto>`.
3. Tornar `MapToDto` e `MapToEntity` abstratos no mapper bidirecional, evitando falha tardia por `NotSupportedException`.
4. Implementar na base `MapToDtos` e `MapToEntities`, ou ajustar as interfaces para que as extensões sejam o único caminho. Escolha somente uma fonte de comportamento de coleção.
5. Preservar `IToDtoMapper` e `IToEntityMapper` para capacidades unidirecionais.
6. Marcar `IAsyncApplicationMapService`, `IAsyncMap`, `AsyncApplicationMapService`, `IMapperEngineService` e `MapperEngine` com `[Obsolete]` sem `error: true`.
7. Atualizar o trecho de histórico do ADR 0002: a API retorna a coleção cronológica autorizada; paginação visual pertence ao grid Angular.
8. Registrar no ADR que a decisão remove paginação do backend, não ordenação nem autorização.

#### Invariantes a preservar

- `IMapper<TEntity, TDto>` mantém a ordem genérica entidade primeiro, DTO depois.
- Mappers antigos continuam compilando temporariamente.
- Nenhum mapper novo deve realizar I/O.
- Histórico continua ordenado e protegido por autorização.

#### Como validar

```powershell
dotnet build Framework/Shared/Shared.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-1-shared\ -p:UseSharedCompilation=false
git diff --check
```

#### Resultado esperado

Shared compila. O legado gera aviso de obsolescência, mas ainda pode ser consumido durante a transição.

#### Erros comuns e recuperação

- Se todos os consumidores virarem erro após `[Obsolete]`, confirme que `error` permaneceu `false`.
- Se `Mapper` não implementar métodos de coleção, não adicione implementações duplicadas em cada mapper. Corrija a base ou o contrato uma vez.
- Se o build chegar a um erro fora de Shared, a fase está concluída somente quando `Shared.csproj` estiver verde.

#### Perguntas de checagem

- Um mapper bidirecional é obrigado pelo compilador a implementar as duas direções?
- Um mapper unidirecional consegue depender somente de `IToDtoMapper` ou `IToEntityMapper`?
- O ADR ainda menciona paginação no servidor?

#### Critério de parada

Parar quando Shared compilar e o novo contrato estiver utilizável sem remover o legado.

### Fase 2: migrar o grafo de mapeamento necessário para tarefas

#### Por que esta fase existe

`TarefaMapping` depende de usuário, departamento, cargo e estado. Migrá-lo isoladamente criaria adaptações artificiais. Esta fase migra o grafo mínimo necessário para os casos de tarefa e solicitação.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- composição de mappers filhos;
- mapeamento de navegações já carregadas;
- contratos direcionais;
- registro de uma instância concreta sob vários contratos de DI.

#### Arquivos e módulos envolvidos

- `DepartamentoMapping.cs`;
- `CargoMapping.cs`;
- `ModuloMapping.cs`;
- `UsuarioDoPerfilDeAcessoMapping.cs`;
- `PerfilDeAcessoMapping.cs`;
- `UsuarioMapping.cs`;
- `TarefaEstadoMapping.cs`;
- `TarefaMapping.cs`;
- `SolicitacaoObtencaoTarefaMapping.cs`;
- `TarefaMovimentacaoMapping.cs`;
- `TrackerMappingServiceCollectionExtensions.cs`;
- consumidores desses mappers nos casos de tarefa.

#### Passos

1. Migrar primeiro mappers folha: departamento, cargo, módulo, estado e usuário resumido de perfil.
2. Migrar `PerfilDeAcessoMapping` para composição síncrona dos mappers filhos.
3. Migrar `UsuarioMapping`, preservando cargos, departamentos, perfis, gestor e preferências de notificação.
4. Migrar `TarefaMapping` para `IMapper<Tarefa, TarefaDTO>`, preservando usuário, departamento, cargo e estado.
5. Transformar `SolicitacaoObtencaoTarefaMapping.MapearAsync` em mapeamento síncrono real. Use `IToDtoMapper` se a direção inversa não existir.
6. Manter `ITarefaMovimentacaoMapping` como capacidade específica e fazê-lo herdar do novo contrato realmente implementado.
7. Remover de `TarefaMovimentacaoMapping` os métodos de coleção duplicados se a base já os fornecer.
8. Registrar cada mapper concreto uma vez e expor a mesma instância pelos contratos necessários.
9. Atualizar `TarefaService`, `ObterTarefaCase`, `AssumirTarefaCase`, `SolicitarTarefaCase`, `DecidirTarefaCase` e `ObterSolicitacaoCase` para a família nova.

#### Invariantes a preservar

- `TarefaDTO` continua recebendo navegações carregadas.
- Normalização de códigos para maiúsculas permanece igual.
- Preferências de notificação do usuário não desaparecem.
- `TarefaMovimentacao.Id` não é confundido com `TarefaId`.
- `SolicitacaoObtencaoTarefaDTO` preserva tarefa, solicitante, aprovador, status e datas.

#### Como validar

```powershell
dotnet build AtronPlatform/Modules/Tracker/Application/AtronTracker.Application.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-2-tracker-application\ -p:UseSharedCompilation=false
rg -n "IAsyncApplicationMapService|IAsyncMap<|AsyncApplicationMapService" AtronPlatform/Modules/Tracker/Application/UseCases/TarefaCases AtronPlatform/Modules/Tracker/Application/Services/EntitiesServices/TarefaService.cs AtronPlatform/Modules/Tracker/Application/Mapping/TarefaMapping.cs
git diff --check
```

#### Resultado esperado

O grafo de tarefa compila usando somente a família nova. Outros módulos ainda podem emitir warnings do legado.

#### Erros comuns e recuperação

- Se uma navegação vier nula, não consulte repositório dentro do mapper. Verifique o carregamento no repositório proprietário.
- Se aparecer dependência circular em DI, procure um mapper composto apontando de volta para o pai.
- Se a direção inversa não existir, não implemente retorno vazio ou `throw`; use o contrato direcional.

#### Perguntas de checagem

- `TarefaMapping` ainda contém `Task`, `await` ou `Task.FromResult`?
- O mesmo mapper concreto é instanciado uma vez por escopo?
- Cada caso depende da menor direção necessária?

#### Critério de parada

Parar quando a Application do Tracker compilar e o recorte de tarefas não possuir referências à família antiga.

### Fase 3: padronizar e completar os casos de escrita de movimentação

#### Por que esta fase existe

Hoje criação e decisão possuem casos, atualização possui um caso não consumido e obtenção/solicitação permanecem no service. Esta fase cria um padrão único antes de remover o serviço.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- caso principal versus caso interno;
- entrada semântica de um fluxo;
- persistência de histórico imutável;
- retorno uniforme por `Resultado` e Resources.

#### Arquivos e módulos envolvidos

- pasta `Application/UseCases/TarefaCases/Movimentacao`;
- `TarefaMovimentacaoMapping.cs`;
- `AtualizacaoTarefaMovimentacaoParametrosRecord.cs`;
- `TarefaResource.resx`;
- `ITarefaMovimentacaoRepository`;
- validações de tarefa/movimentação.

#### Passos

1. Adotar a mesma forma em todos os casos de movimentação: `ExecutarAsync`, entrada semântica, mapper específico, conversão para entidade, persistência e `Resultado`.
2. Ajustar `CriarTarefaMovimentacaoCase` para receber `Tarefa` e `Usuario`, deixando a criação do DTO dentro do caso.
3. Ajustar `AtualizarTarefaMovimentacaoCase` para receber `AtualizacaoTarefaMovimentacaoParametrosRecord`.
4. Criar `RegistrarObtencaoTarefaMovimentacaoCase` recebendo tarefa e responsável.
5. Criar `RegistrarSolicitacaoTarefaMovimentacaoCase` recebendo solicitação e responsável.
6. Alinhar `RegistrarDecisaoTarefaMovimentacaoCase` ao mesmo tratamento de erro e validação.
7. Usar `TarefaResource.Erro_RegistrarMovimentacao`; remover textos literais de falha.
8. Decidir se `TarefaMovimentacaoDTO` precisa de validação própria. Se sim, criar `TarefaMovimentacaoValidador` e registrá-lo. Se os mappers já garantirem integralmente os campos obrigatórios, não criar um validador artificial apenas para satisfazer o construtor atual.
9. Mover `ObterDetalhes` de `TarefaMovimentacaoCase` para um método privado do mapper ou helper puro com nome de transformação.
10. Remover `TarefaMovimentacaoCase.cs` quando não houver consumidor.

#### Invariantes a preservar

- toda movimentação possui tarefa, tipo, detalhes, código/nome do ator e data UTC;
- criação, atualização, obtenção, solicitação, aprovação e recusa continuam registradas;
- histórico permanece append-only;
- falha de movimentação continua propagada pelo caso principal conforme o comportamento atual.

#### Como validar

```powershell
dotnet build AtronPlatform/Modules/Tracker/Application/AtronTracker.Application.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-3-movimentacoes\ -p:UseSharedCompilation=false
rg -n "class .*TarefaMovimentacaoCase|Erro ao registrar movimentação|TarefaMovimentacaoCase.ObterDetalhes" AtronPlatform/Modules/Tracker/Application
git diff --check
```

#### Resultado esperado

Existem cinco casos de escrita coerentes. Nenhum deles depende de `TarefaMovimentacaoService`.

#### Erros comuns e recuperação

- Se um caso começar a decidir policy de obtenção, devolva essa responsabilidade ao caso principal.
- Se o mesmo detalhe for montado em dois lugares, mantenha o mapper como fonte única.
- Se o DTO for validado depois de convertido em entidade, ajuste a ordem para evitar persistir estado inválido.

#### Perguntas de checagem

- Cada evento possui exatamente um caso de movimentação?
- O caso principal entrega dados de domínio, não um DTO parcialmente montado?
- Todos usam o mesmo Resource de falha?

#### Critério de parada

Parar quando os cinco casos compilarem e a transformação auxiliar não estiver nomeada como Use Case.

### Fase 4: migrar consumidores e remover `TarefaMovimentacaoService`

#### Por que esta fase existe

Criar casos sem migrar consumidores mantém dois caminhos concorrentes. Esta fase troca cada consumidor de forma explícita e só então remove o serviço antigo.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- migração de dependências por construtor;
- propagação de `Resultado`;
- remoção segura de contrato sem consumidores;
- busca de referências no repositório.

#### Arquivos e módulos envolvidos

- `CriarTarefaCase.cs`;
- `AtualizarTarefaCase.cs`;
- `AssumirTarefaCase.cs`;
- `SolicitarTarefaCase.cs`;
- `DecidirTarefaCase.cs`;
- novos casos de movimentação;
- `ITarefaMovimentacaoService.cs`;
- `TarefaMovimentacaoService.cs`.

#### Passos

1. Fazer `CriarTarefaCase` consumir somente `CriarTarefaMovimentacaoCase` para o histórico.
2. Trocar `AtualizarTarefaCase` para `AtualizarTarefaMovimentacaoCase`.
3. Trocar `AssumirTarefaCase` para `RegistrarObtencaoTarefaMovimentacaoCase`.
4. Trocar `SolicitarTarefaCase` para `RegistrarSolicitacaoTarefaMovimentacaoCase`.
5. Confirmar que `DecidirTarefaCase` usa `RegistrarDecisaoTarefaMovimentacaoCase`.
6. Remover `ITarefaMovimentacaoMapping` dos casos principais quando o mapper passar a pertencer ao caso de movimentação.
7. Executar a busca de consumidores de `ITarefaMovimentacaoService`.
8. Quando a busca em produção retornar zero, remover `ITarefaMovimentacaoService`, `TarefaMovimentacaoService` e sua DI.
9. Não alterar os testes antigos nesta fase. Registre que eles serão migrados na rodada futura.

#### Invariantes a preservar

- ordem de persistência e efeitos externos de cada fluxo;
- mensagens de falha e sucesso;
- notificação interna e e-mail continuam nos mesmos casos principais;
- nenhuma movimentação deixa de ser registrada.

#### Como validar

```powershell
rg -n "ITarefaMovimentacaoService|TarefaMovimentacaoService" AtronPlatform/Modules/Tracker AtronPlatform/WebApi
dotnet build AtronPlatform/Modules/Tracker/Application/AtronTracker.Application.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-4-consumidores\ -p:UseSharedCompilation=false
git diff --check
```

#### Resultado esperado

A busca em código de produção retorna zero referências ao serviço removido. Os fluxos principais apontam diretamente para seus casos de movimentação.

#### Erros comuns e recuperação

- Se a remoção quebrar um consumidor não identificado, restaure somente o contrato e a implementação removidos e migre esse consumidor antes de repetir a remoção.
- Se um caso ganhar mapper, repositório e regra de outro fluxo, verifique se foi injetado o caso de movimentação correto.

#### Perguntas de checagem

- Há apenas um caminho de escrita para cada tipo de movimentação?
- Algum serviço ainda recebe `ITarefaMovimentacaoService`?
- O caso principal continua legível como sequência de negócio?

#### Critério de parada

Parar quando a Application compilar sem o serviço/interface de movimentação.

### Fase 5: fechar a fronteira entre CRUD e fluxos de negócio

#### Por que esta fase existe

`TarefaService` já foi reduzido, mas controller e serviços ainda estão em um estado híbrido. Esta fase define qual entrada é dona de cada operação.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- CRUD versus consulta contextual;
- fachada de compatibilidade;
- segregação de interfaces;
- contrato entre controller e Application.

#### Arquivos e módulos envolvidos

- `ITarefaService.cs` e `TarefaService.cs`;
- `ITarefaObtencaoService.cs` e `TarefaObtencaoService.cs`;
- `ObterTarefaCase.cs`;
- novos casos de visões, se adotados;
- `TarefaController.cs`;
- serviços de configurações e estados.

#### Passos

1. Manter em `ITarefaService` somente criar, atualizar, excluir, obter por ID e obter todos.
2. Fazer esses métodos delegarem para `CriarTarefaCase`, `AtualizarTarefaCase`, `ExcluirTarefaCase` e `ObterTarefaCase`.
3. Retirar configurações de `ITarefaService`; usar `ITarefaConfiguracoesService` diretamente na entrada correspondente.
4. Manter em `ITarefaObtencaoService` assumir, solicitar, decidir e obter solicitações.
5. Para `Meu quadro`, `Equipe`, `Disponíveis` e `Acesso`, escolher casos com intenção explícita: `ObterMeuQuadroCase`, `ObterEquipeCase`, `ObterTarefasDisponiveisCase` e `ObterAcessoTarefaCase`.
6. A fachada de obtenção pode delegar a esses casos se você quiser preservar somente dois serviços no controller.
7. Remover de `ObterTarefaCase` os métodos contextuais, mantendo apenas as leituras CRUD.
8. Migrar todos os endpoints do controller para o contrato proprietário. Não deixar somente o primeiro endpoint migrado.
9. Manter as rotas HTTP atuais.

#### Invariantes a preservar

- URLs e verbos HTTP não mudam;
- policies de autorização permanecem;
- usuários sem gestão continuam sem poder assumir diretamente;
- solicitações, aprovações e recusas mantêm regras do ADR 0002;
- consultas `Meu quadro`, `Equipe`, `Disponíveis` e `Solicitações` continuam separadas.

#### Como validar

```powershell
rg -n "tarefaService\.(ObterMeuQuadro|ObterEquipe|ObterDisponiveis|ObterAcesso|ObterSolicitacoes|Assumir|SolicitarObtencao|AprovarSolicitacao|RecusarSolicitacao|ObterHistorico)" AtronPlatform/WebApi
dotnet build AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-5-webapi\ -p:UseSharedCompilation=false
git diff --check
```

#### Resultado esperado

O controller compila apontando cada endpoint para sua entrada correta. `TarefaService` é CRUD e não conhece obtenção, movimentação, configurações ou histórico.

#### Erros comuns e recuperação

- Se o controller ganhar muitos detalhes de negócio, mantenha fachadas finas que apenas delegam.
- Se uma interface ampla reaparecer com todos os métodos, revise a propriedade de cada rota em vez de mover o acoplamento para outro nome.

#### Perguntas de checagem

- É possível explicar cada método de `ITarefaService` como CRUD de tarefa?
- `ITarefaObtencaoService` contém somente operações do ciclo de obtenção?
- Alguma rota mudou sem requisito funcional?

#### Critério de parada

Parar quando o WebApi compilar e não houver chamada do controller a método inexistente.

### Fase 6: remover paginação do backend e manter paginação local no grid

#### Por que esta fase existe

O contrato atual mistura paginação de servidor com um grid que já pode paginar a coleção localmente. A nova decisão exige uma API simples e paginação exclusivamente visual no Angular.

#### Nível mínimo para executar

N2 Aplicado em C# e Angular Material.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- contrato HTTP de coleção;
- ordenação no repositório;
- `MatTableDataSource` e `MatPaginator`;
- ciclo de renderização de `@if` e `@ViewChild`.

#### Arquivos e módulos envolvidos

- `ITarefaMovimentacaoRepository.cs`;
- `TarefaMovimentacaoRepository.cs`;
- novo `ObterHistoricoTarefaCase.cs`;
- `TarefaController.cs`;
- DTOs de página/consulta ainda existentes;
- `tarefa.service.ts`;
- `tarefa-movimentacao.model.ts`;
- `tarefa-historico.component.ts/.html/.css`.

#### Passos no backend

1. Criar `ObterHistoricoTarefaCase` com usuário atual, autorização de acesso, repositório e mapper direcional para DTO.
2. Retornar `Resultado<IReadOnlyCollection<TarefaMovimentacaoDTO>>`.
3. Usar `ObterMovimentacoesPorIdAsync(tarefaId)` e preservar `OrderByDescending(DataOcorrencia).ThenByDescending(Id)` no repositório.
4. Alterar o endpoint para receber somente `id`.
5. Remover `pagina` e `tamanhoPagina` de comentários, assinatura e chamadas.
6. Remover `TarefaMovimentacaoPaginaDTO`, `TarefaMovimentacaoConsulta`, `TarefaMovimentacaoPagina` e `ObterPaginaAsync` quando a busca retornar zero.

#### Passos no Angular

1. Remover `TarefaMovimentacaoPagina`; o contrato passa a ser `TarefaMovimentacao[]`.
2. Alterar `obterHistorico(id)` para não enviar parâmetros.
3. Remover `PageEvent`, `paginaAtual`, `tamanhoPagina`, `totalItens` e `alterarPagina` do componente.
4. Carregar a coleção uma única vez quando o painel for aberto.
5. Conectar o `MatPaginator` ao `MatTableDataSource` por um `@ViewChild` setter, porque o paginator nasce dentro de `@if`.
6. Manter o `mat-paginator` dentro de `tarefa-historico-grid` ou imediatamente associado à tabela.
7. Usar opções locais `[5, 10, 20]` e reiniciar a primeira página quando uma nova coleção for atribuída.
8. Não remover os paginadores de `tarefa-view` ou outros grids.
9. Não alterar paginação de planejamento de custos, perfis, usuários, cargos ou departamentos nesta refatoração.

#### Invariantes a preservar

- carregamento sob demanda ao expandir;
- estados de carregamento, vazio, erro e retry;
- ordenação cronológica descendente;
- autorização no backend;
- paginator continua visível e funcional no grid;
- nenhuma segunda chamada HTTP ocorre ao trocar de página.

#### Como validar

```powershell
rg -n "pagina|tamanhoPagina|TarefaMovimentacaoPagina|TarefaMovimentacaoConsulta|ObterPaginaAsync" AtronPlatform/Modules/Tracker AtronPlatform/WebApi AtronFront/src/app/plataforma/tracker/tarefas
dotnet build AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-6-webapi\ -p:UseSharedCompilation=false
npm run build --prefix AtronFront
git diff --check
```

Na busca final, ocorrências de paginação podem permanecer no `mat-paginator` do grid, mas não no contrato HTTP ou nos modelos de resposta.

#### Resultado esperado

O endpoint retorna uma lista. O grid pagina localmente sem novas requisições por página.

#### Erros comuns e recuperação

- Se o paginator não funcionar após abrir o painel, confirme que ele foi atribuído ao `dataSource` depois de aparecer no `@if`.
- Se a tabela mostrar todos os itens, o paginator existe visualmente mas não está ligado ao `MatTableDataSource`.
- Se a API retornar itens fora de ordem, corrija o repositório, não o componente.

#### Perguntas de checagem

- Mudar a página gera uma nova requisição HTTP? A resposta deve ser não.
- A API ainda conhece `pageIndex`, `pagina` ou `tamanhoPagina`? A resposta deve ser não.
- O paginator está associado a uma tabela? A resposta deve ser sim.

#### Critério de parada

Parar quando WebApi e Angular compilarem e a busca não encontrar paginação no contrato de histórico.

### Fase 7: migrar os mapeadores restantes de produção

#### Por que esta fase existe

Marcar o legado como obsoleto não o torna inutilizado. A busca atual mostra consumidores no restante do Tracker e no Stock. Esta fase conclui a decisão de usar somente a família nova em produção.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- migração por dependência de mappers folha para compostos;
- atualização in-place de entidades;
- contratos direcionais;
- DI por módulo proprietário.

#### Arquivos e módulos envolvidos

No Tracker:

- planejamento de custo;
- perfil de acesso;
- módulo;
- usuário identity/request;
- serviços e casos consumidores;
- `TrackerMappingServiceCollectionExtensions.cs`.

No Stock:

- `CategoriaMapping`;
- `ClienteMapping`;
- `FornecedorMapping`;
- `ProdutoMapping`;
- serviços consumidores;
- `StockModuleServiceCollectionExtensions.cs`.

#### Passos

1. Migrar mappers folha antes dos compostos.
2. Para atualizações in-place, decidir um contrato específico, por exemplo `IAtualizaEntidadeMapper<TEntity, TDto>`, somente se houver mais de um consumidor real. Não sobrecarregar `IMapper` silenciosamente.
3. Substituir `MapToDTOAsync`, `MapToEntityAsync` e `MapToListDTOAsync` por operações da nova família.
4. Remover `Task.FromResult` usado apenas para simular assincronicidade.
5. Atualizar DI do Tracker e Stock para registrar `IMapper`, `IToDtoMapper` ou `IToEntityMapper` conforme a capacidade real.
6. Remover o registro de `IMapperEngineService` quando não houver consumidor de produção.
7. Manter os arquivos legados por compatibilidade com os testes desta rodada, mas sem consumidores em `AtronPlatform` e `Framework` fora das próprias definições.

#### Invariantes a preservar

- normalização e atualização in-place permanecem;
- nenhum mapper realiza persistência ou consulta;
- cada módulo registra seus próprios mappers;
- Stock não passa a depender do Tracker.

#### Como validar

```powershell
rg -n "IAsyncApplicationMapService|IAsyncMap<|AsyncApplicationMapService|IMapperEngineService|MapperEngine" AtronPlatform Framework --glob '!Framework/Shared/Application/Interfaces/Service/IAsyncApplicationMapService.cs' --glob '!Framework/Shared/Application/Interfaces/Service/IMapperEngineService.cs' --glob '!Framework/Shared/Application/Services/Mapper/AsyncApplicationMapService.cs' --glob '!Framework/Shared/Application/Services/Mapper/MapperEngine.cs'
dotnet build AtronPlatform/Modules/Stock/Application/AtronStock.Application.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-7-stock\ -p:UseSharedCompilation=false
dotnet build AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\fase-7-webapi\ -p:UseSharedCompilation=false
git diff --check
```

#### Resultado esperado

A busca de produção retorna zero consumidores da família antiga. Os arquivos obsoletos permanecem apenas como ponte para a rodada futura de testes ou remoção final.

#### Erros comuns e recuperação

- Se a migração de Stock alterar regra de normalização, restaure o comportamento antes de continuar.
- Se um mapper precisar de I/O, interrompa e verifique se a consulta pertence ao repositório ou preparador.
- Se a fase ficar grande, subdivida por módulo, mas mantenha cada subfase compilável.

#### Perguntas de checagem

- Existe algum `Task.FromResult` em mapper de produção sem I/O?
- MapperEngine ainda possui consumidor real?
- Todos os mappers compostos dependem de contratos da nova família?

#### Critério de parada

Parar quando Tracker e Stock compilarem sem consumidores de produção do mapper antigo.

### Fase 8: fechar DI, limpar transição e executar builds finais

#### Por que esta fase existe

Build confirma referências de compilação, mas não garante que todos os casos utilizados tenham registro explícito. Esta fase faz o inventário final da composição e encerra a migração estrutural.

#### Nível mínimo para executar

N2 Aplicado.

#### Nível para adaptar

N3 Autônomo.

#### Conceitos que você precisa dominar

- grafo de dependências;
- composição por módulo;
- limpeza de código transitório;
- limites do que build comprova.

#### Casos que devem estar registrados se permanecerem consumidos

Casos principais:

- `CriarTarefaCase`;
- `AtualizarTarefaCase`;
- `ExcluirTarefaCase`;
- `ObterTarefaCase`;
- `AssumirTarefaCase`;
- `SolicitarTarefaCase`;
- `DecidirTarefaCase`;
- `ObterSolicitacaoCase`;
- casos das quatro visões contextuais, se forem separados;
- `TarefaNotificacaoInternaCase`, somente se continuar como caminho oficial.

Casos de movimentação:

- `CriarTarefaMovimentacaoCase`;
- `AtualizarTarefaMovimentacaoCase`;
- `RegistrarObtencaoTarefaMovimentacaoCase`;
- `RegistrarSolicitacaoTarefaMovimentacaoCase`;
- `RegistrarDecisaoTarefaMovimentacaoCase`;
- `ObterHistoricoTarefaCase`.

#### Passos

1. Listar todas as classes `*Case` na pasta de tarefas.
2. Para cada classe consumida por construtor, confirmar `AddScoped<ClasseCase>()`.
3. Remover registros de tipos excluídos ou substituídos.
4. Confirmar registros de mappers e validadores usados pelos casos.
5. Remover comentários transitórios como “Removido para...”.
6. Remover imports, arquivos e contratos sem consumidores de produção.
7. Atualizar `docs/plano-refatoracao-servicos-aplicacao.md` com o estado realmente concluído, sem declarar testes executados.
8. Revisar o diff apenas do escopo e executar os builds finais.

#### Invariantes a preservar

- lifetime scoped para casos, repositórios e mappers ligados ao DbContext;
- nenhuma implementação do Tracker é registrada pelo Stock ou vice-versa;
- não declarar a rodada concluída se algum build de produção falhar;
- não editar testes nesta fase.

#### Como validar

```powershell
rg -n "public (sealed )?class .*Case\(" AtronPlatform/Modules/Tracker/Application/UseCases/TarefaCases
rg -n "AddScoped<.*Case" AtronPlatform/Modules/Tracker/Infrastructure/DependencyInjection/TrackerModuleServiceCollectionExtensions.cs
rg -n "ITarefaMovimentacaoService|TarefaMovimentacaoService|TarefaMovimentacaoPagina|TarefaMovimentacaoConsulta|ObterPaginaAsync" AtronPlatform AtronFront/src/app/plataforma/tracker/tarefas
dotnet build Framework/Shared/Shared.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\final-shared\ -p:UseSharedCompilation=false
dotnet build AtronPlatform/Modules/Stock/Application/AtronStock.Application.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\final-stock\ -p:UseSharedCompilation=false
dotnet build AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --no-restore -m:1 -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-build\final-webapi\ -p:UseSharedCompilation=false
npm run build --prefix AtronFront
git diff --check
```

#### Resultado esperado

Todos os projetos de produção do escopo compilam. Não há serviço de movimentação, paginação de histórico no backend nem consumidor de produção do mapper antigo.

#### Erros comuns e recuperação

- Se o build falhar em testes, confirme que você executou os projetos de produção indicados, não a solução inteira. A migração dos testes pertence à rodada posterior.
- Se DI ainda possuir um tipo removido, remova somente o registro obsoleto e reconstrua.
- Se um case não tiver consumidor, decida se ele está incompleto ou desnecessário antes de registrá-lo automaticamente.

#### Perguntas de checagem

- Todo case registrado possui consumidor?
- Todo case consumido possui registro?
- O histórico faz uma única chamada HTTP e pagina somente no grid?
- `TarefaService` contém apenas CRUD?
- A busca do mapper antigo retorna apenas definições legadas e testes adiados?

#### Critério de parada

Parar quando todos os builds de produção passarem e as buscas estruturais confirmarem o estado desejado.

## Validação final desta rodada

Esta rodada não executará testes. Os sinais obrigatórios são:

1. build de `Shared.csproj`;
2. build de `AtronStock.Application.csproj`;
3. build de `AtronPlatform.WebApi.csproj` com referências;
4. `npm run build --prefix AtronFront`;
5. `git diff --check`;
6. buscas de ausência para serviço de movimentação, paginação de backend e mapper antigo em produção;
7. comparação manual entre casos existentes e registros de DI.

### O que esses sinais não comprovam

- regra de autorização do histórico em execução;
- transação entre tarefa e movimentação;
- publicação de notificação e e-mail;
- comportamento de aprovação e recusa;
- paginação do grid em navegador;
- resolução real do provider de DI no startup;
- compatibilidade dos testes existentes.

Esses itens serão cobertos na rodada posterior de testes por fluxo.

## Estratégia de recuperação

1. Antes de cada fase, registre os arquivos do escopo com `git status --short`.
2. Não use `git reset --hard`, `git clean` ou checkout destrutivo.
3. Se uma fase falhar, reverta manualmente apenas os arquivos alterados naquela fase ou use o diff para restaurar trechos específicos.
4. Enquanto um consumidor ainda usar o caminho antigo, preserve a interface e a implementação antigas.
5. Remova o serviço de movimentação somente depois da busca de consumidores em produção retornar zero.
6. Mantenha o mapper antigo como warning até a Fase 7 terminar.
7. Se o Angular perder o paginator, restaure somente o vínculo `MatTableDataSource`/`MatPaginator`; não restaure parâmetros de paginação HTTP.
8. Se o build parar em erro preexistente fora da fase, registre o erro e não amplie o escopo sem confirmar a relação.

## Definição de concluído

- [ ] Shared compila com a nova família de mapper completa.
- [ ] Família antiga está marcada como obsoleta sem bloquear a migração.
- [ ] `TarefaMapping` e seus filhos usam a nova família.
- [ ] Todos os cinco eventos de escrita de movimentação possuem case.
- [ ] Histórico possui case consultivo sem paginação de backend.
- [ ] Casos principais consomem casos de movimentação, não o service antigo.
- [ ] `ITarefaMovimentacaoService` e `TarefaMovimentacaoService` foram removidos.
- [ ] `TarefaService` contém somente CRUD.
- [ ] Controller aponta cada rota para o contrato proprietário.
- [ ] API do histórico retorna `TarefaMovimentacaoDTO[]` ou coleção equivalente.
- [ ] Angular não envia página ou tamanho para o histórico.
- [ ] `MatPaginator` permanece associado ao grid e opera localmente.
- [ ] Todos os casos consumidos estão registrados na DI.
- [ ] Não há registro de tipo removido.
- [ ] Não há consumidor de produção do mapper antigo.
- [ ] Shared, Stock, WebApi e Angular compilam.
- [ ] Nenhum teste foi alterado ou executado nesta rodada.
- [ ] ADR 0002 e plano de refatoração refletem o comportamento realmente adotado.

## Próximos conhecimentos

Depois desta rodada, o próximo trabalho real deve ser o plano de testes por fluxo:

1. criação de tarefa e movimentação de criação;
2. atualização e descrição das diferenças;
3. obtenção direta e policy;
4. solicitação e resolução de aprovador;
5. aprovação e recusa;
6. consulta autorizada do histórico;
7. composição de DI;
8. integração do endpoint com o grid Angular.

Essa rodada posterior deve usar casos diretamente nos testes de Application e reservar testes de controller/integração para contrato HTTP e DI.

## Fora do escopo

- implementar as fases agora;
- criar, corrigir ou executar testes nesta rodada;
- alterar migrations ou estrutura da tabela de movimentações;
- mudar regras de obtenção, aprovação, recusa ou autorização;
- alterar textos de produto sem necessidade de compilação;
- remover paginação de grids de outras funcionalidades;
- refatorar planejamento de custos, perfis, usuários, cargos ou departamentos além do necessário para migrar seus mappers;
- alterar a topologia de módulos ou extrair serviços externos;
- declarar deploy ou produção validados apenas porque os builds passaram.
