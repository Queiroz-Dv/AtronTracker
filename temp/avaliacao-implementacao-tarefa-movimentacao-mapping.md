# Avaliação técnica: estrutura de TarefaMovimentacaoMapping

## Parecer executivo

**Parecer: requer correção.**

A direção arquitetural é boa: retirar a montagem das movimentações dos casos de uso, dar um contrato específico à capacidade e nomear os cenários de criação, atualização e obtenção melhora a leitura dos orquestradores.

A forma atual, porém, ainda não está adequada para ser consolidada. As três classes internas são pequenas funções estáticas com um único consumidor, sem dependências, contrato ou testes próprios. Essa divisão aumenta a quantidade de lugares que precisam preservar os mesmos campos obrigatórios, mas não cria isolamento real. A inconsistência já aparece nos objetos produzidos: cada cenário inicializa um subconjunto diferente dos dados exigidos pela persistência.

O principal problema não é a quantidade de classes. É a ausência de um ponto único que preserve as invariantes comuns de uma nova movimentação, especialmente `TarefaId`, responsável e data da ocorrência.

**Confiança geral: alta para a análise estrutural e média para o fluxo executável**, porque o build do projeto parou antes de compilar esse escopo devido a um erro de sintaxe em outro arquivo modificado.

## Escopo, referência e baseline

Não foi localizado um guia formal específico para esta implementação. A avaliação usa como referência:

- o objetivo declarado: reduzir o crescimento da classe principal e facilitar localização e manutenção dos mapeamentos;
- o ADR 0004, que aceita mapeadores especializados, mas rejeita extrair cada bloco automaticamente sem responsabilidade própria;
- `docs/padrao-responsabilidades-servicos.md`, que recomenda colaborador específico para mapeamento, mas alerta contra classes artificiais para operações curtas e estáveis;
- o contrato de persistência formado por `TarefaMovimentacao`, sua configuração EF e o repositório;
- os consumidores atuais em `CriarTarefaCase`, `AtualizarTarefaCase` e `AssumirTarefaCase`.

`TarefaMovimentacaoMapping.cs` e `ITarefaMovimentacaoMapping.cs` são arquivos novos e ainda não rastreados pelo Git. O checkout possui muitas alterações paralelas. Assim, esta avaliação se limita ao estado atual e não atribui ao autor todas as mudanças vizinhas.

## Evidências verificadas

- `TarefaMovimentacaoMapping.cs`, incluindo as três classes internas e as conversões entidade/DTO.
- `ITarefaMovimentacaoMapping.cs` e os contratos compartilhados `IMapper`, `IToDtoMapper`, `IToEntityMapper` e `Mapper`.
- Consumo de `ITarefaMovimentacaoMapping` nos três casos de uso.
- Registro de mapeadores em `TrackerMappingServiceCollectionExtensions`.
- `TarefaMovimentacaoDTO`, `TarefaMovimentacao`, `TarefaMovimentacaoConfiguration` e `TarefaMovimentacaoRepository`.
- Diff e estado do Git nos arquivos do escopo.
- Build executado:

```text
dotnet build AtronPlatform/Modules/Tracker/Application/AtronTracker.Application.csproj
  --no-restore
  -p:BuildProjectReferences=false
  -p:OutDir=C:\p\Projetos\AtronRC\artifacts\codex-review\tracker-mapping-20260818\
```

O build falhou antes do escopo avaliado, em `ITarefaObtencaoService.cs:10`, com `CS1003`. O arquivo está modificado no checkout e possui um tipo genérico sem o fechamento `>`.

Não existem testes específicos para `TarefaMovimentacaoMapping` no estado atual.

## Aderência ao objetivo e às decisões do projeto

| Expectativa | Estado | Evidência |
|---|---|---|
| Retirar montagem de movimentação dos casos de uso | Atende | Os casos de uso chamam `MapearParaCriacao`, `MapearParaAtualizacao` e `MapearParaObtencao`. |
| Dar nome explícito aos cenários | Atende | Métodos e classes indicam claramente criação, atualização e obtenção. |
| Evitar inflação da classe principal | Atende parcialmente | A classe pública ficou menor, mas o arquivo possui 119 linhas e três classes de passagem no mesmo arquivo. |
| Melhorar manutenção e reduzir risco | Não atende | Campos obrigatórios estão duplicados e inconsistentes entre os três cenários. |
| Preservar o contrato de persistência | Não atende | `TarefaId` não é transportado nem preenchido e dados obrigatórios ficam ausentes em cenários distintos. |
| Integrar os contratos aos consumidores | Não atende | Os casos consomem `ITarefaMovimentacaoMapping` e `IMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>`, mas nenhum deles está registrado na composição. |
| Isolar falhas por classe | Não atende | As classes são estáticas, sem contrato ou teste independente, e todas convergem no mesmo `MapToEntity`. |

## Acertos comprovados

### Acerto: contrato específico para a capacidade

**Evidência:** `CriarTarefaCase`, `AtualizarTarefaCase` e `AssumirTarefaCase` dependem de `ITarefaMovimentacaoMapping` para operações próprias da movimentação.

**Por que está correto:** métodos como `MapearParaCriacao` não pertencem ao contrato genérico `IMapper<TEntity, TDto>`. Um contrato nomeado pela capacidade comunica melhor a intenção e evita inflar uma abstração compartilhada.

**O que demonstra:** aplicação correta de segregação por capacidade, compatível com N2 na dimensão de implementação.

**Limite:** o contrato ainda não está registrado na DI e mistura operações específicas com conversões genéricas.

### Acerto: casos de uso ficaram orientados à sequência do fluxo

**Evidência:** os casos de uso obtêm tarefa e responsável, delegam a montagem da movimentação e seguem para registro.

**Por que está correto:** isso respeita o ADR 0004, segundo o qual casos de uso coordenam e mapeadores transformam dados.

**O que demonstra:** entendimento aplicado sobre propriedade de responsabilidades.

**Limite:** a delegação só é benéfica se o mapeador produzir um objeto válido e se a composição resolver seus contratos.

### Acerto: dependência de mapeamento do usuário permanece no ponto de entrada

**Evidência:** `TarefaMovimentacaoMapping` converte `Usuario` para `UsuarioDTO` antes de chamar os helpers internos.

**Por que está correto:** os helpers ficam puros e não precisam conhecer DI.

**O que demonstra:** controle básico da direção das dependências.

**Limite:** pureza não justifica, sozinha, criar uma classe por método.

## Findings

### [P1] A movimentação perde o vínculo com a tarefa e reutiliza o identificador errado

**Evidência:** os três helpers atribuem `tarefa.Id` ou `tarefaAnterior.Id` a `TarefaMovimentacaoDTO.Id`. `MapToEntity` copia esse valor para `TarefaMovimentacao.Id`. O DTO não possui `TarefaId` e `MapToEntity` não preenche `TarefaMovimentacao.TarefaId`. A entidade, a chave estrangeira e as consultas distinguem `Id` da movimentação de `TarefaId`.

**Impacto:** uma nova movimentação pode tentar usar o ID da tarefa como chave primária, colidir com outra movimentação e permanecer com `TarefaId = 0`. Mesmo que a gravação ocorra, a consulta do histórico por tarefa não encontrará o registro correto.

**Contrato esperado:** para uma movimentação nova, `Id` deve permanecer no valor de nova entidade e `TarefaId` deve receber `tarefa.Id`.

**Argumento:** essa distinção é uma invariante do modelo de persistência, não uma preferência de nomenclatura.

**Recomendação:** transportar `TarefaId` explicitamente no objeto usado para registro e mapear esse campo para a entidade. Não usar `Id` como atalho para dois conceitos diferentes.

**Como validar:** testes de cada cenário devem mapear até `TarefaMovimentacao` e confirmar `Id == 0` e `TarefaId == tarefa.Id`; um teste de integração deve registrar e consultar o histórico pelo ID da tarefa.

**Confiança:** alta.

### [P1] Cada cenário omite dados obrigatórios diferentes

**Evidência:** criação não preenche `Detalhes`; atualização não preenche `DataOcorrencia`; obtenção não preenche `ResponsavelCodigo`, `ResponsavelNome` nem `DataOcorrencia`. `MapToEntity` apenas copia esses campos. A configuração EF exige `Descricao`, `ResponsavelCodigo`, `ResponsavelNome` e `DataOcorrencia`.

**Impacto:** os objetos gerados podem falhar na validação ou na persistência. Datas não inicializadas também geram histórico temporalmente incorreto.

**Contrato esperado:** todo cenário de nova movimentação deve fornecer vínculo da tarefa, tipo, descrição, responsável e instante de ocorrência.

**Argumento:** a fragmentação duplicou a obrigação de lembrar os campos comuns. Isso é exatamente o tipo de risco que a estrutura pretendia reduzir.

**Recomendação:** centralizar a construção dos campos comuns em um único método ou objeto de criação e deixar cada cenário fornecer apenas tipo, descrição e dados realmente variáveis.

**Como validar:** criar testes parametrizados para criação, atualização e obtenção, verificando todos os campos obrigatórios antes de chamar o repositório.

**Confiança:** alta.

### [P1] Os contratos consumidos não estão conectados à composição do Tracker

**Evidência:** `TrackerMappingServiceCollectionExtensions` não registra `ITarefaMovimentacaoMapping` nem `IMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>`. Além disso, `TarefaMovimentacaoMapping` implementa o contrato específico, mas não implementa `IMapper<TarefaMovimentacao, TarefaMovimentacaoDTO>`; sua classe base `Mapper<TEntity, TDto>` também não declara essa interface.

**Impacto:** a ativação dos casos de uso que dependem desses contratos falhará na resolução de dependências, mesmo depois que o projeto voltar a compilar.

**Contrato esperado:** cada abstração injetada deve possuir uma implementação fechada registrada pelo módulo proprietário.

**Argumento:** separar interface e implementação só reduz acoplamento quando a composição fecha a relação entre ambas.

**Recomendação:** registrar uma única instância concreta por escopo e expor apenas os contratos realmente implementados. Para os casos de persistência, decidir explicitamente entre usar o contrato específico ou os contratos direcionais compartilhados, sem manter duas famílias desconectadas.

**Como validar:** construir o provider do módulo com validação habilitada e resolver os três casos de uso; depois executar ao menos um fluxo de criação.

**Confiança:** alta.

### [P2] As classes menores atuais não possuem uma fronteira própria

**Evidência:** cada classe interna possui somente um método `static Mapear`, não tem dependência, contrato, estado ou consumidor independente e permanece no mesmo arquivo. A classe pública apenas converte o usuário e encaminha a chamada.

**Impacto:** há mais tipos para navegar e mais locais repetindo invariantes, sem ganho proporcional de substituição, teste ou isolamento. A inconsistência dos campos obrigatórios já materializa esse custo.

**Contrato esperado:** conforme o ADR 0004, uma extração deve atribuir responsabilidade a um conceito claro e reduzir acoplamento; extrair todo bloco automaticamente foi uma alternativa rejeitada.

**Argumento:** classes menores são um meio. Coesão, invariantes centralizadas e razões independentes de mudança são o critério real.

**Recomendação:** no tamanho atual, manter um `TarefaMovimentacaoMapping` específico com métodos públicos nomeados e um método privado para os campos comuns. Separar um cenário em classe e arquivo próprios somente quando ele ganhar dependências, variações ou regras suficientes para ser testado como unidade independente.

**Como validar:** revisar se cada classe extraída responde a uma razão de mudança exclusiva e possui teste próprio que não depende da fachada principal.

**Confiança:** alta.

### [P3] A operação de atualização expõe dados correlacionados como três parâmetros soltos

**Evidência:** `MapearParaAtualizacao` e o helper correspondente recebem tarefa anterior, tarefa atual e responsável.

**Impacto:** a assinatura cresce a cada novo dado contextual e facilita inverter ou omitir informações correlacionadas em consumidores futuros.

**Contrato esperado:** operações com contexto composto devem nomear o conjunto de dados quando ele representa um conceito único da aplicação.

**Argumento:** um record semântico torna explícito que os três valores formam o contexto de uma atualização de movimentação.

**Recomendação:** quando consolidar o contrato, considerar um `AtualizacaoTarefaMovimentacaoParametros` imutável. Não criar esse record apenas para reduzir visualmente a quantidade de parâmetros; ele deve representar o contexto da operação.

**Como validar:** o nome e os campos do record devem permitir compreender a operação sem abrir a implementação.

**Confiança:** média.

## Avaliação por competência

### Correção

A intenção está delimitada, mas os objetos resultantes não preservam o contrato da entidade e da persistência. Não há teste protegendo os cenários.

### Implementação

Os nomes são claros, o escopo é interno e o contrato específico melhora os consumidores. A granularidade, porém, ficou abaixo de uma fronteira útil, e a composição ainda não foi concluída.

### Arquitetura

A direção de dependência está correta: casos de uso dependem de uma abstração da Application. O trade-off entre coesão e quantidade de classes ainda foi decidido principalmente pelo tamanho, não pelas razões de mudança.

### Validação

Não há teste específico do mapper. O build executado foi inconclusivo para este escopo porque falhou antes em outro arquivo modificado.

### Produto e operação

O risco é direto no histórico de tarefas: vínculo, descrição, responsável e data podem ser perdidos ou impedir o registro.

### Autonomia e raciocínio

O argumento apresentado demonstra preocupação válida com legibilidade, manutenção e evolução. Para avançar, falta confrontar a estrutura proposta com invariantes compartilhadas, composição e testes, não apenas com tamanho de classe.

## Nível demonstrado nesta entrega

| Dimensão | Nível demonstrado | Evidência | Confiança | Para o próximo nível |
|---|---|---|---|---|
| Correção | N1, Fundamental | A estrutura existe, mas não preserva campos obrigatórios nem a identidade correta. | Alta | Seguir DTO, entidade, EF e repositório de ponta a ponta antes de consolidar o mapper. |
| Implementação | N2, Aplicado | Contrato específico, nomes claros e casos de uso delegando mapeamento. | Alta | Remover abstrações sem fronteira e concluir a integração na DI. |
| Arquitetura | N2, Aplicado | Responsabilidade saiu dos orquestradores e permaneceu na Application. | Média | Justificar cada extração por razão de mudança, contrato e teste independente. |
| Validação | Não avaliado | Build bloqueado por erro externo ao escopo e ausência de testes do mapper. | Alta | Criar testes de contrato para cada cenário e validar resolução de DI. |
| Produto e operação | N1, Fundamental | O fluxo de persistência não foi preservado de ponta a ponta. | Média | Validar gravação e consulta do histórico com o provider e banco corretos. |
| Autonomia e raciocínio | N2, Aplicado | O objetivo e o trade-off de crescimento de classes foram explicitados pelo autor. | Média | Incluir invariantes, consumidores, composição e sinais de validação no próprio raciocínio. |

Não há evidência suficiente para N3 ou N4 nesta entrega. Isso não é um rótulo sobre a pessoa; é o limite do que este recorte comprova.

## Próximas ações recomendadas

### Correções obrigatórias

1. Separar `Id` de `TarefaId` e preservar o vínculo correto.
2. Garantir os campos obrigatórios comuns nos três cenários.
3. Fechar e registrar os contratos de mapeamento realmente consumidos.

### Melhoria arquitetural

4. Manter por enquanto uma única classe específica com métodos nomeados e construção comum centralizada.
5. Extrair uma classe por cenário apenas quando surgir uma fronteira real de dependência, variação ou teste.

### Validação

6. Adicionar testes unitários de criação, atualização, obtenção e entidade para DTO.
7. Validar a resolução dos casos de uso pelo container do Tracker.
8. Reexecutar o build depois de corrigir o erro sintático externo ao escopo.

## Limites da avaliação

- Não foi executado fluxo contra PostgreSQL.
- O build não alcançou o mapper por causa de um erro anterior em outro arquivo.
- Os dois arquivos principais são novos e não possuem baseline no Git.
- O checkout contém mudanças amplas e paralelas, portanto a autoria dos problemas de integração não foi inferida.
- Não foi feita nenhuma alteração na implementação avaliada.
