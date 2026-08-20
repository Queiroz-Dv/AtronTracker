# Avaliação técnica: refatoração de tarefas para Use Cases

## Parecer executivo

**Parecer: Requer correção. Confiança geral: alta para o estado atual do código e média para atribuição de autoria fora dos arquivos declarados.**

A direção arquitetural é válida: `TarefaService` ficou menor, comandos passaram a ter donos explícitos e o mapeamento de movimentações agora centraliza invariantes que estavam espalhadas. Esse é um avanço real em coesão.

A entrega, entretanto, não fecha uma fatia vertical executável. O contrato de `ITarefaService` foi reduzido sem migrar todos os endpoints do controller, a composição de DI não registra todos os novos casos, os testes ainda constroem as classes antigas e o histórico perdeu o contrato de paginação no servidor definido pelo ADR 0002. O fluxo de movimentação também está simultaneamente no serviço antigo e em novos casos, com código duplicado e partes que não compilam.

Minha avaliação como arquiteto sênior seria: a decisão de decompor está correta, mas a estratégia de migração abriu mais frentes do que a entrega conseguia fechar. A próxima evolução deveria trabalhar uma operação por vez, do endpoint ao teste, preservando o caminho anterior até o novo caminho estar validado.

## Escopo, referência e baseline

### Escopo atribuído ao usuário

- `TarefaObtencaoService.cs` no novo diretório `Tarefas/Obtencao`;
- `ObterTarefaCase.cs`;
- `TarefaService.cs`;
- casos de tarefa e movimentação atualmente em migração;
- contratos, consumidores, DI e testes diretamente necessários para avaliar esses fluxos.

O checkout possui muitas alterações paralelas em Tracker, Shared, Stock, notificações e front-end. Não atribuí ao usuário problemas fora do recorte declarado. Para arquivos vizinhos, avaliei apenas compatibilidade com o estado atual.

### Baseline

- branch: `1.5.1`;
- commit base: `b86c9a0a`;
- `TarefaObtencaoService` anterior aparece removido e a nova versão está em diretório ainda não rastreado;
- os novos casos de tarefa e movimentação também estão, em sua maioria, não rastreados;
- não existe commit isolado que permita atribuir todo o diff do checkout à mesma entrega.

### Referências usadas

Não foi informado um guia produzido especificamente para esta migração. Foram usados como contrato de referência:

- `docs/plano-refatoracao-servicos-aplicacao.md`, especialmente as regras de preservar contratos, migrar por pontos testáveis e validar cada fase;
- ADR 0004, que aceita extrações apenas quando atribuem responsabilidade clara e reduzem acoplamento;
- ADR 0002, que define consultas separadas para as visões e histórico cronológico, imutável e paginado no servidor;
- `docs/padrao-responsabilidades-servicos.md`;
- a avaliação anterior do mapeamento de movimentações, para verificar se os problemas então apontados foram corrigidos.

## Evidências verificadas

### Código e fluxo

- diff entre `HEAD` e os contratos/serviços atuais;
- `TarefaController`, `ITarefaService`, `ITarefaObtencaoService` e `ITarefaMovimentacaoService`;
- `TarefaService`, `TarefaObtencaoService` e `ObterTarefaCase`;
- casos de criar, atualizar, excluir, assumir, solicitar e decidir tarefa;
- casos e serviço de movimentação;
- mapper, DTO, entidade, repositório e registros de DI;
- testes de obtenção, criação, movimentação e mapeamento.

### Validações executadas

1. `git diff --check` no recorte avaliado: não encontrou erro de whitespace; emitiu somente avisos de conversão LF/CRLF.
2. Build isolado da Application com referências desabilitadas: falhou com 54 erros. Esse resultado é parcialmente ruidoso porque usa assemblies dependentes ainda não recompilados, mas também revelou inconsistências do recorte.
3. Build integrado do `AtronPlatform.WebApi` com `OutDir` alternativo: parou antes do Tracker por um erro em `Shared/Application/Validacoes/EmailValidador.cs`, fora do recorte declarado.

Os testes não foram executados porque o grafo não alcança compilação. Além disso, a inspeção mostra que os testes de obtenção e criação ainda instanciam construtores e tipos removidos, logo precisam ser migrados antes de fornecer evidência válida.

## Aderência ao guia

| Expectativa | Estado | Evidência |
|---|---|---|
| Reduzir responsabilidades de `TarefaService` | Atende | criação, atualização e exclusão delegam para casos específicos; o arquivo caiu de aproximadamente 134 para 56 linhas. |
| Manter regras de obtenção fora de `TarefaService` | Atende parcialmente | comandos foram movidos para casos, mas o controller ainda chama os métodos removidos de `ITarefaService`. |
| Preservar contratos HTTP durante a refatoração | Não atende | vários endpoints continuam consumindo métodos que não existem mais em `ITarefaService`. |
| Criar pontos de encaixe testáveis antes de remover o caminho anterior | Não atende | testes permanecem acoplados aos construtores e tipos anteriores; novos casos principais não possuem cobertura executável. |
| Extrair apenas responsabilidades com conceito claro | Atende parcialmente | `CriarTarefaCase`, `AssumirTarefaCase` e `DecidirTarefaCase` representam intenções claras; `ObterTarefaCase` agrega cinco consultas distintas e os casos de persistência de movimentação representam passos internos, não intenções externas. |
| Preservar histórico cronológico e paginação no servidor | Não atende | a consulta paginada e seu contrato foram removidos; o repositório atual retorna uma lista completa e o serviço chama APIs inexistentes. |
| Fechar composição de DI | Não atende | há casos requeridos não registrados e registros de tipos removidos. |
| Preservar invariantes do mapeamento da movimentação | Atende por inspeção | mapper atual diferencia `Id` e `TarefaId`, preenche ator/data/tipo e possui quatro testes focados; os testes não puderam ser executados. |

## Acertos comprovados

### Acerto: `TarefaService` passou a funcionar como fachada em comandos

**Evidência:** `CriarAsync`, `AtualizarAsync` e `ExcluirAsync` delegam, respectivamente, para `CriarTarefaCase`, `AtualizarTarefaCase` e `ExcluirTarefaCase`.

**Por que está correto:** criação, atualização e exclusão deixam de compartilhar uma unidade extensa apenas por pertencerem à entidade tarefa. Cada fluxo pode evoluir suas dependências e validações sem inflar a fachada.

**O que demonstra:** N2 Aplicado em implementação e arquitetura nesta entrega, porque um padrão real do repositório foi aplicado a operações concretas.

**Limite:** a fachada não está integrada de ponta a ponta, pois controller, DI e testes não foram concluídos na mesma fatia.

### Acerto: casos principais expressam intenções de negócio

**Evidência:** `AssumirTarefaCase`, `SolicitarTarefaCase` e `DecidirTarefaCase` coordenam usuário, policy, persistência, movimentação e notificação na ordem do fluxo.

**Por que está correto:** os nomes representam ações do usuário e permitem localizar rapidamente o fluxo de obtenção, em vez de concentrá-lo em um serviço genérico.

**O que demonstra:** N2 Aplicado em modelagem da camada de aplicação.

**Limite:** há inconsistência entre usar `ITarefaNotificacaoInternaService` e `TarefaNotificacaoInternaCase`, e a composição desses casos ainda não está fechada.

### Acerto: o mapper de movimentação corrigiu invariantes importantes

**Evidência:** `TarefaMovimentacaoMapping` usa `TarefaId` da tarefa, preserva `Id` da movimentação, define tipo, descrição, ator e `DataOcorrencia`; `TarefaMovimentacaoMappingTests` cobre criação, atualização, obtenção e conversão entidade/DTO.

**Por que está correto:** o histórico depende desses campos para manter vínculo, fotografia do ator e ordem cronológica. A construção comum em `CriarBase` reduz divergência entre cenários.

**O que demonstra:** N2 Aplicado em correção de mapeamento e proteção de invariantes.

**Limite:** a persistência e a consulta do histórico ainda não estão integradas, e os testes não puderam executar no baseline atual.

### Acerto: uso de saídas somente leitura nas consultas

**Evidência:** `ITarefaObtencaoService` e `ObterTarefaCase` retornam `IReadOnlyCollection<T>` para listas materializadas.

**Por que está correto:** o consumidor recebe o contrato mínimo necessário para enumerar os resultados sem depender de mutação por `List<T>`.

**O que demonstra:** N2 Aplicado em desenho de contrato local.

**Limite:** estreitar a coleção não compensa o fato de `ITarefaObtencaoService` ter crescido para nove operações de responsabilidades diferentes.

## Findings

### [P1] O controller continua chamando operações removidas de `ITarefaService`

**Evidência:** `ITarefaService` expõe somente configurações, criar, atualizar, excluir e obter por ID. Entretanto, `TarefaController` ainda chama nele `ObterMeuQuadroAsync`, `ObterEquipeAsync`, `ObterDisponiveisAsync`, `ObterAcessoAsync`, `ObterSolicitacoesAsync`, `ObterEstadosAsync`, `AssumirAsync`, `SolicitarObtencaoAsync`, `AprovarSolicitacaoAsync`, `RecusarSolicitacaoAsync` e `ObterHistoricoAsync`. Somente o endpoint geral foi migrado para `ITarefaObtencaoService`.

**Impacto:** o WebApi não compila quando o build alcançar o controller; todos esses endpoints ficam indisponíveis.

**Contrato esperado:** o plano exige preservar contratos HTTP e migrar consumidores como parte da mesma mudança de contrato.

**Argumento:** reduzir uma interface só fecha a refatoração quando todos os consumidores mudam atomicamente. Alterar apenas um endpoint deixa a fronteira pública em estado intermediário inválido.

**Recomendação:** migrar todos os endpoints para o contrato proprietário na mesma fatia. Obtenção deve usar `ITarefaObtencaoService`; estados, configurações e histórico precisam de destinos explícitos antes de remover os métodos antigos.

**Como validar:** build do WebApi, testes dos endpoints e busca por chamadas aos métodos removidos retornando zero.

**Confiança:** alta.

### [P1] A composição de DI não consegue construir as novas fachadas e casos

**Evidência:** `TarefaService` exige `AtualizarTarefaCase` e `ExcluirTarefaCase`; `TarefaObtencaoService` exige `AssumirTarefaCase`, `SolicitarTarefaCase`, `ObterSolicitacaoCase` e `ObterTarefaCase`; `CriarTarefaCase` exige `CriarTarefaMovimentacaoCase`. O módulo registra somente `CriarTarefaCase`, `DecidirTarefaCase` e `RegistrarDecisaoTarefaMovimentacaoCase`. Ao mesmo tempo, a composição ainda registra `ISolicitacaoObtencaoTarefaMapeador`, `TarefaNotificacaoInternaService`, `ITarefaUsuarioAtualService` e implementações que foram removidas ou movidas. Não existe registro de `IValidador<TarefaMovimentacaoDTO>`.

**Impacto:** mesmo após resolver erros de compilação, o startup falhará ao resolver `ITarefaService` ou `ITarefaObtencaoService`.

**Contrato esperado:** cada abstração e tipo concreto exigido por uma entrada pública precisa ser resolvido pelo módulo proprietário.

**Argumento:** o desacoplamento percebido nos Use Cases só existe em execução se a composição fechar o grafo. Caso contrário, a complexidade apenas foi transferida para o startup.

**Recomendação:** registrar a fatia completa de casos e colaboradores, remover registros obsoletos e adicionar um teste de composição que resolva as duas fachadas com validação do provider habilitada.

**Como validar:** `ValidateOnBuild` e `ValidateScopes`, resolução de `ITarefaService` e `ITarefaObtencaoService`, depois um smoke test do host.

**Confiança:** alta.

### [P1] A migração de movimentações quebrou o contrato funcional do histórico

**Evidência:** o ADR 0002 exige paginação no servidor. O contrato anterior recebia tarefa, página e tamanho. O `ITarefaMovimentacaoService` atual não expõe consulta; `TarefaMovimentacaoPaginaDTO` e `TarefaMovimentacaoConsulta` não possuem definição atual; o repositório oferece apenas `ObterMovimentacoesPorIdAsync`, que materializa todas as linhas. O serviço ainda chama `ObterPaginaAsync`, `Mapear` e `ObterResource`, que não existem, e `RegistrarObtencaoAsync` referencia `tarefa`, `responsavel`, `RegistrarAsync` e `RegistroMovimentacao`, também inexistentes no arquivo atual.

**Impacto:** o histórico não compila, o endpoint não possui consumidor válido e a tentativa de substituir a consulta paginada por lista completa aumenta custo conforme o histórico cresce.

**Contrato esperado:** histórico cronológico, imutável, autorizado e paginado no servidor.

**Argumento:** esta não é apenas uma reorganização estrutural; houve perda de comportamento explicitamente registrado no domínio do produto.

**Recomendação:** restaurar primeiro a consulta paginada e sua autorização em um caminho compilável. Se for migrá-la, `ObterHistoricoTarefaCase` é uma intenção real do usuário e pode possuir o fluxo; o repositório deve continuar recebendo um objeto de consulta ou parâmetros paginados. Remover o caminho antigo somente depois do teste do novo.

**Como validar:** teste que captura página e tamanho no repositório, teste de acesso negado, endpoint retornando página correta e consulta SQL limitada no servidor.

**Confiança:** alta.

### [P1] O novo contrato síncrono de mapeamento de tarefa não possui implementação compatível

**Evidência:** `TarefaService` e `ObterTarefaCase` dependem de `IMapper<Tarefa, TarefaDTO>`. `TarefaMapping` continua herdando `AsyncApplicationMapService<TarefaDTO, Tarefa>` porque precisa mapear usuário, departamento, cargo e estado de modo assíncrono. A DI registra apenas `IAsyncApplicationMapService<TarefaDTO, Tarefa>` para essa implementação.

**Impacto:** o código não compila ou o container não resolve o mapper esperado; uma adaptação apressada para mapeamento síncrono pode omitir as navegações já presentes no DTO.

**Contrato esperado:** o consumidor deve depender de uma capacidade realmente implementada pelo mapper, preservando o conteúdo atual de `TarefaDTO`.

**Argumento:** trocar a família de contratos durante a extração do Use Case ampliou o escopo e criou uma incompatibilidade independente da decomposição de serviços.

**Recomendação:** preservar temporariamente o mapper assíncrono validado, ou criar um contrato específico de tarefa que expresse explicitamente a projeção necessária. Não registrar `TarefaMapping` como `IMapper` enquanto ele não implementar esse contrato integralmente.

**Como validar:** testes de `ObterPorId`, `Meu quadro` e `Equipe` confirmando usuário, departamento, cargo e estado, além da resolução pelo container.

**Confiança:** alta.

### [P2] `ITarefaObtencaoService` ficou mais amplo e `ObterTarefaCase` é um serviço de consultas com sufixo de caso

**Evidência:** `ITarefaObtencaoService` possui nove operações, incluindo obtenção, listagem geral, `Meu quadro`, `Equipe`, disponíveis e acesso. `ObterTarefaCase` contém cinco consultas distintas. Seu construtor recebe `IUsuarioService`, mas não o armazena nem utiliza; a fachada resolve o usuário e entrega a entidade de domínio ao caso.

**Impacto:** o acoplamento saiu de `TarefaService`, mas parte dele foi concentrada em outro contrato amplo. O nome `ObterTarefaCase` não informa qual intenção está sendo executada, e a responsabilidade pelo usuário atual fica dividida entre fachada e caso.

**Contrato esperado:** ADR 0002 determina consultas separadas para as visões; ADR 0004 exige conceito claro e contrato mínimo útil.

**Argumento:** Use Case reduz acoplamento quando representa uma intenção. Uma classe com todos os métodos de leitura é um serviço de consulta válido, mas deve ser nomeada e tratada como tal.

**Recomendação:** escolher explicitamente uma das duas formas:

1. casos por intenção, como `ObterMeuQuadroCase`, `ObterEquipeCase`, `ObterDisponiveisCase` e `ObterAcessoTarefaCase`; ou
2. um colaborador coeso de consultas, como `TarefaConsultaService`, sem apresentá-lo como caso singular.

Manter em `ITarefaObtencaoService` somente assumir, solicitar, decidir e consultar solicitações. O caso ou colaborador contextual deve resolver o usuário por um contrato estreito, em vez de depender do `IUsuarioService` completo.

**Como validar:** cada consumidor recebe somente as operações necessárias; busca por `IUsuarioService` nos fluxos de tarefa deixa apenas dependências justificadas; nomes dos tipos correspondem às intenções reais.

**Confiança:** alta.

### [P2] Passos internos de auditoria foram modelados como Use Cases e duplicam persistência

**Evidência:** `CriarTarefaMovimentacaoCase`, `AtualizarTarefaMovimentacaoCase` e `RegistrarDecisaoTarefaMovimentacaoCase` repetem mapeamento e `RegistrarAsync`; os dois primeiros repetem validação e usam texto literal de falha, enquanto o terceiro usa resource e não valida. O serviço antigo mantém cópias das duas primeiras operações.

**Impacto:** a mesma política de persistência e erro pode divergir por evento. Também fica ambíguo se o limite transacional pertence ao caso principal ou ao caso de movimentação aninhado.

**Contrato esperado:** Use Case representa intenção do usuário; colaboradores especializados representam efeitos internos do fluxo. O ADR 0004 rejeita extrair cada bloco automaticamente.

**Argumento:** o usuário cria, atualiza, assume ou decide uma tarefa. Registrar a movimentação é um efeito obrigatório desses casos, não uma operação independente exposta ao produto. `Obter histórico`, ao contrário, é uma intenção consultiva real.

**Recomendação:** manter os casos principais e introduzir um único colaborador de escrita, por exemplo `ITarefaMovimentacaoRegistrar`, responsável por validar, converter e persistir um DTO já composto pelo mapper. Um caso específico de histórico pode ser mantido para a consulta autorizada e paginada.

**Como validar:** todos os eventos usam o mesmo caminho de persistência e resource de falha; não há duplicação de `RegistrarAsync`; testes dos casos principais confirmam que falha de movimentação impede a continuação conforme a política vigente.

**Confiança:** alta.

### [P2] Os testes existentes não foram migrados junto com os novos pontos de encaixe

**Evidência:** `TarefaObtencaoServiceTests` ainda chama o construtor anterior com repositórios, policy e mapeador; a nova fachada recebe casos concretos. `CriarTarefaTests` declara retorno `CriarTarefaCase`, mas ainda instancia `CriarTarefa`, usa `ITarefaUsuarioAtualService` removido e chama a assinatura antiga de movimentação. `TarefaMovimentacaoServiceTests` usa consulta, DTO e construtores antigos.

**Impacto:** regras sensíveis de assunção, solicitação, aprovação, recusa e histórico ficaram sem sinal repetível durante a migração.

**Contrato esperado:** o plano exige ajustar testes no novo ponto antes de remover o caminho anterior.

**Argumento:** em uma refatoração estrutural, testes são o mecanismo que diferencia mudança de forma de mudança acidental de comportamento.

**Recomendação:** migrar por fluxo. Começar por um caso, adaptar seus testes, fechar DI e controller, validar, e somente então seguir para o próximo.

**Como validar:** testes focados compilam e cobrem sucesso, falha de policy, falha de persistência, movimentação e notificação; depois o build do WebApi passa.

**Confiança:** alta.

### [P3] Há sinais locais de código transitório que reduzem clareza

**Evidência:** comentários “Removido para...” permanecem sobre métodos ainda ativos; `ExcluirTarefaCase` expõe o repositório como campo público; delegações simples usam `async/await`; `ObterSolicitacaoCase` usa campos com inicial maiúscula enquanto o restante usa privados com sublinhado.

**Impacto:** não quebra o produto por si só, mas dificulta saber qual caminho é definitivo durante uma migração já extensa.

**Contrato esperado:** código transitório deve indicar claramente o que ainda está ativo e ser removido quando a fatia fecha.

**Argumento:** consistência local reduz custo de revisão e evita que o caminho antigo permaneça por engano.

**Recomendação:** limpar apenas depois dos P1 e P2, sem transformar estilo em prioridade sobre integração.

**Como validar:** revisão do diff final sem comentários obsoletos, campos públicos acidentais ou delegações desnecessárias.

**Confiança:** alta.

## Avaliação por competência

### Correção

A lógica interna de vários casos preserva as regras centrais, e o mapper de movimentação corrigiu invariantes importantes. A entrega integrada, porém, quebra contratos do controller, DI e histórico. O resultado esperado ainda não foi atingido.

### Implementação

A decomposição de `TarefaService` é legível e os nomes dos casos principais são bons. O escopo ficou amplo demais: mapper genérico, contratos, identidade atual, notificações, DI, testes e paginação foram migrados ao mesmo tempo.

### Arquitetura

A direção de dependência dos casos para repositórios, policies, mappers e notificações é adequada. A fronteira entre Use Case, fachada, consulta e colaborador interno ainda precisa amadurecer. Houve transferência parcial de um contrato amplo para `ITarefaObtencaoService`.

### Validação

O recorte não possui build nem testes executáveis. O build integrado também está bloqueado antes pelo `EmailValidador` de Shared, que não atribuo a esta entrega. Ainda assim, consumidor, DI e testes incompatíveis são confirmados por inspeção direta.

### Produto e operação

As rotas públicas foram preservadas textualmente no controller, mas não possuem contratos compiláveis. O maior risco operacional é a indisponibilidade dos endpoints de tarefa e a perda da paginação do histórico.

### Autonomia e raciocínio

O recorte demonstra compreensão aplicada de coesão, fachada e intenção de Use Case. A correção do mapper após feedback anterior mostra boa capacidade de incorporar invariantes. Falta demonstrar fechamento vertical, revisão completa de consumidores e validação antes de abrir a próxima frente.

## Nível demonstrado nesta entrega

| Dimensão | Nível demonstrado | Evidências | Confiança | Para o próximo nível |
|---|---|---|---|---|
| Correção | N1 Fundamental | regras locais reconhecíveis, mas entrega integrada não compila e perde contrato de histórico | alta | preservar o fluxo antigo até validar a nova fatia de ponta a ponta |
| Implementação | N2 Aplicado | `TarefaService` reduzido, casos nomeados e mapper centralizado | alta | limitar cada migração a um fluxo completo com consumidor, DI e teste |
| Arquitetura | N2 Aplicado | separação de fachada, policy, mapper e casos principais | alta | distinguir intenção do usuário, consulta agrupada e colaborador interno sem usar sufixos como arquitetura |
| Validação | N1 Fundamental | `diff --check` executado, mas build e testes da entrega não fecham | alta | adotar build e teste focado como condição antes de remover contratos antigos |
| Produto e operação | N1 Fundamental | contratos HTTP e paginação foram considerados, mas estão quebrados no estado atual | alta | validar rotas e consulta paginada no provider real ou em integração representativa |
| Autonomia e raciocínio | N2 Aplicado | decisão própria de decompor e correções locais coerentes | média | revisar consumidores, DI, testes e rollback como parte da própria estratégia de migração |

Não há evidência suficiente para N3 ou N4 nesta entrega. Isso não é um rótulo profissional; é o limite demonstrado por este recorte ainda intermediário.

## Próximas ações recomendadas

### Correções obrigatórias

1. Escolher uma única fatia, por exemplo `Assumir tarefa`, e fechar controller, caso, movimentação, notificação, DI e teste.
2. Migrar os demais endpoints para seus contratos reais ou restaurar temporariamente compatibilidade em `ITarefaService`.
3. Restaurar a consulta autorizada e paginada do histórico antes de continuar a reorganização.
4. Fechar o grafo de DI e remover registros de tipos inexistentes.
5. Decidir e preservar o contrato correto de mapeamento de `Tarefa`, atualmente assíncrono.

### Melhorias arquiteturais

6. Manter `ITarefaObtencaoService` restrito ao ciclo de obtenção.
7. Tratar as visões como casos separados ou como um serviço de consulta explicitamente nomeado.
8. Tratar registro de movimentação como colaborador interno compartilhado, não como Use Case aninhado por evento.
9. Usar um contrato estreito para o usuário atual, evitando depender do `IUsuarioService` completo.

### Validação e aprendizado

10. Migrar testes junto com cada caso, não depois de toda a estrutura.
11. Adicionar teste de composição do módulo com validação do provider.
12. Só remover o caminho antigo depois de build do WebApi e testes focados passarem.

## Limites da avaliação

- O build integrado foi bloqueado primeiro por `EmailValidador` em Shared, fora do escopo atribuído.
- Não foi possível executar os testes do Tracker.
- Não foi executado fluxo manual nem integração com PostgreSQL.
- O checkout possui alterações amplas e sem commit isolado; autoria fora dos arquivos declarados não foi inferida.
- Os novos arquivos não rastreados não possuem baseline individual no Git.
- Nenhum código foi corrigido durante esta avaliação.
