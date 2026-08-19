# ADR 0010: Adotar resolvers para seleção contextual

## Status

Aceito.

## Contexto

Os módulos possuem situações em que um serviço precisa selecionar um único
resultado entre candidatos ou fontes ordenadas. A resolução do aprovador de uma
solicitação de tarefa, por exemplo, combina hierarquia organizacional, fallback
e consulta de usuários. Outros componentes apenas fornecem configuração e não
devem receber a mesma nomenclatura.

Sem uma convenção explícita, essas seleções podem permanecer dentro dos
orquestradores, ser confundidas com policies ou gerar interfaces sem uma
fronteira real.

## Decisão

O AtronRC adotará o sufixo `Resolver` para componentes cuja responsabilidade
principal seja selecionar um único resultado efetivo entre múltiplos candidatos
ou fontes possíveis.

Resolvers de aplicação serão organizados em
`Application/Resolvers/<Capacidade>`. Classes internas, estáveis e com uma única
implementação serão concretas, `sealed` e injetadas diretamente. Uma interface
específica será mantida somente quando houver implementações alternativas,
seleção em runtime, decoração, contrato entre módulos ou uma fronteira entre
Application e Infrastructure.

Não serão criados `IResolver<T>`, classes base ou registros genéricos. O nome de
cada resolver deve representar o conceito e o contexto resolvidos.

O padrão não substitui:

- policies, que decidem se uma operação é permitida;
- specifications, que avaliam um candidato;
- validadores, que verificam coerência de entrada;
- mapeadores, que traduzem representações;
- providers, que expõem configuração ou dados externos;
- factories, que constroem objetos;
- use cases, que executam o fluxo completo.

Como primeira adequação, `AprovadorObtencaoTarefaResolver` passa a ser uma
classe concreta `sealed` em `Application/Resolvers/Tarefas`. A interface que
apenas repetia esse tipo concreto é removida.

O antigo `ResponsavelNotificacaoEstoqueResolver` não selecionava candidatos:
apenas fornecia o código recebido da configuração. Ele passa a se chamar
`ResponsavelNotificacaoEstoqueProvider`, fica em
`Application/Providers/Notificacoes` e também é consumido como classe concreta
`sealed`.

Resolvers técnicos permanecem na camada proprietária. O
`DatabaseProviderResolver`, por exemplo, continua na configuração de
Infrastructure.

## Alternativas consideradas

### Manter resolvers dentro de Services

Rejeitada porque esconde uma seleção contextual entre serviços genéricos e não
estabelece localidade para a responsabilidade.

### Criar uma interface genérica para todos os resolvers

Rejeitada porque entradas, resultados e critérios são específicos de cada
capacidade. Uma abstração genérica não representa um contrato de domínio comum.

### Exigir interface para todo resolver

Rejeitada porque uma interface sem substituição ou fronteira real apenas duplica
uma classe concreta e amplia a composição e a manutenção.

### Renomear toda consulta ou fallback como resolver

Rejeitada porque busca, configuração, validação, relacionamento e seleção são
responsabilidades diferentes. O sufixo será reservado à seleção de um resultado
efetivo.

## Consequências

- seleções contextuais passam a possuir nome e localização previsíveis;
- serviços e use cases permanecem focados na ordem completa do fluxo;
- testes podem cobrir prioridade e fallback diretamente;
- consumidores de resolvers concretos passam a testar a colaboração real com
  dependências externas substituídas;
- novas interfaces exigem justificativa arquitetural explícita;
- o uso indevido do sufixo precisa ser evitado em revisões de código.

## Validação

O padrão detalhado, os exemplos e o checklist estão em
`docs/padrao-resolvers.md`. Mudanças devem preservar contratos funcionais,
consumidores, registros de DI e testes focados.
