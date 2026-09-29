# Padrão de policies, specifications e validações

## Objetivo

Este documento define quando usar Policy, Specification e Validation na camada
`Application` dos módulos do AtronRC. O objetivo é dar um dono explícito às
decisões de negócio e impedir que serviços de aplicação ou validadores
genéricos acumulem responsabilidades diferentes.

## Estrutura

Policies são organizadas primeiro pelo tipo arquitetural e depois pela
capacidade de negócio proprietária da decisão.

```text
Application/
├── Policies/
│   ├── PlanejamentoCustos/
│   │   └── EstruturaPlanejadaPolicy.cs
│   └── Tarefas/
│       ├── ITarefaObtencaoPolicy.cs
│       ├── TarefaGovernancaPolicy.cs
│       └── TarefaObtencaoPolicy.cs
├── Specifications/
├── Resolvers/
└── Validador/
```

Não deve ser criada uma interface genérica `IPolicy<T>`. Quando uma abstração
for necessária, seu contrato deve usar o vocabulário da decisão, como
`ITarefaObtencaoPolicy`.

## Policy

Uma Policy avalia se uma operação de negócio é permitida em determinado
contexto. Ela pode combinar o estado de entidades, o ator da operação,
consultas a repositórios e outras informações do fluxo. Seu resultado pode
explicar por que a operação foi recusada.

Exemplos:

- decidir se uma tarefa pode ser assumida ou solicitada;
- impedir a movimentação de um cargo comprometido com planejamento atual ou
  futuro;
- exigir que a estrutura de uma tarefa ofereça um aprovador.

O método público deve nomear a decisão avaliada. Preferir `Avaliar` quando a
operação toma uma decisão contextual. `Validar` permanece aceitável em uma
policy já consolidada quando expressa diretamente o vocabulário do fluxo.

Uma interface específica é indicada quando existe uma fronteira de DI,
implementações alternativas ou necessidade real de substituir o colaborador.
Não criar uma interface apenas para repetir todos os membros de uma classe
concreta estável e testável.

## Specification

Uma Specification representa um critério sobre um candidato. Sua pergunta
típica é: "este objeto satisfaz esta condição?". Ela é apropriada quando o
critério precisa ser reutilizado, combinado ou aplicado como filtro.

No contrato atual do Tracker, `ISpecification<T>` retorna somente `bool`.
Portanto, ele não substitui uma Policy que precisa devolver o motivo da decisão
nem deve ser usado apenas para trocar o nome de um bloco condicional.

## Validation

Validation verifica a formação e a coerência dos dados recebidos. O contrato
`IValidador<T>` é apropriado para regras de entrada que recebem um objeto e
podem acumular mensagens, como campos obrigatórios, limites de tamanho e
coerência de datas.

Validadores de aplicação não decidem, por si só, se um ator pode executar uma
operação contextual. Também não substituem invariantes que precisam ser
preservadas pela entidade em qualquer fluxo.

## Comparação

| Conceito | Pergunta | Resultado esperado | Exemplo |
| --- | --- | --- | --- |
| Validation | Os dados estão coerentes? | Mensagens de validação | Título e período de uma tarefa |
| Specification | O candidato satisfaz o critério? | Predicado booleano | Departamento possui o código informado |
| Policy | A operação é permitida neste contexto? | Decisão e motivo | Usuário pode assumir a tarefa |
| Resolver | Qual candidato ou valor deve ser usado? | Resultado efetivo ou ausência | Aprovador da obtenção da tarefa |

Os conceitos podem colaborar. Uma Policy pode usar Specifications para
critérios realmente reutilizados, mas condições locais e pequenas devem
permanecer na Policy para evitar fragmentação artificial.

Um Resolver seleciona um resultado entre candidatos ou fontes ordenadas. Ele
não substitui a Policy, pois não decide se a operação é permitida. O padrão
completo está em `docs/padrao-resolvers.md`.

## Relação com serviços e domínio

Serviços de aplicação consomem policies como colaboradores e preservam a ordem
do caso de uso. Eles não devem repetir as decisões encapsuladas nelas.

Uma regra deve permanecer ou ser movida para o domínio quando for uma
invariante que precisa valer em toda criação ou transição da entidade. Policies
da aplicação são adequadas quando a decisão depende do contexto do usuário,
de consultas ou da operação coordenada pelo caso de uso.

## Checklist

- A classe responde a uma decisão de negócio nomeável?
- A decisão depende da operação ou do ator, e não apenas da validade do objeto?
- O serviço apenas coordena e reage ao resultado da policy?
- A interface, quando existir, representa uma fronteira real?
- A classe está em `Policies/<capacidade>`?
- Mensagens observáveis continuam vindo de `Resources`?
- A extração preserva o comportamento e os contratos funcionais do fluxo?
