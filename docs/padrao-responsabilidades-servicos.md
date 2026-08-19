# Padrão de responsabilidades dos serviços de aplicação

## Objetivo

Serviços de aplicação coordenam casos de uso. Eles não devem se transformar em
depósitos de regras, validações, mapeamentos e criação de objetos sem relação
direta com a orquestração do fluxo.

O padrão existe para preservar legibilidade, localidade das regras, testes
focados e evolução segura dos módulos do AtronRC.

## Responsabilidade do serviço de aplicação

O serviço de aplicação deve:

- receber o comando ou a consulta do caso de uso;
- obter os colaboradores necessários por interfaces;
- coordenar a sequência do fluxo;
- acionar persistência e efeitos externos no momento definido pelo caso de uso;
- traduzir o resultado do fluxo para o contrato de saída da aplicação.

Ele não deve decidir sozinho regras que pertencem a um conceito de domínio,
repetir mapeamentos, criar entidades manualmente quando houver uma fábrica ou
interface de domínio adequada, nem acumular validações extensas em cada método.

## Critérios SOLID

Em toda criação, alteração ou revisão de serviço, avaliar os pontos abaixo.

- **Responsabilidade única:** o serviço deve ter uma razão principal para
  mudar: a orquestração do caso de uso. Regras de tarefa, resolução de
  aprovador, montagem de notificação, validação e mapeamento são razões de
  mudança diferentes e devem ter donos explícitos.
- **Aberto para extensão e fechado para modificação:** quando uma regra puder
  variar, o serviço deve depender da interface do conceito, não de condicionais
  que precisem ser repetidamente alteradas nele.
- **Substituição e segregação de interfaces:** cada colaborador deve expor o
  menor contrato útil ao caso de uso. Não injetar interfaces amplas apenas para
  acessar uma operação isolada.
- **Inversão de dependência:** o serviço depende de interfaces da aplicação ou
  do domínio. Repositórios, transporte, mapeadores e integrações permanecem
  implementações conectadas na composição da aplicação.

## Destino de cada tipo de código

| Tipo de código | Dono esperado |
| --- | --- |
| Invariantes, transições de estado, decisões e criação de objetos de negócio | Entidade, objeto de valor, fábrica ou interface de domínio do conceito processado |
| Mapeamento entre contratos e entidades | Mapeador ou colaborador de preparação específico |
| Validação de campos e coerência do comando | Validador de aplicação específico |
| Decisão que combina operação, ator, estado e consultas | Policy de aplicação específica |
| Seleção de um resultado entre candidatos ou fontes ordenadas | Resolver da capacidade proprietária |
| Transformação simples, pura e reutilizável | Classe auxiliar ou método de extensão coeso |
| Tipo declarado como `record` no projeto de aplicação | `Application/Records/<Capacidade>` |
| Catálogo de constantes ou operação intencionalmente estática, pura e sem estado | Classe estática em `Application/Statics` |
| Persistência | Repositório |
| Comunicação interna, e-mail ou outra integração | Serviço ou compositor especializado |
| Ordem das etapas do caso de uso | Serviço de aplicação |

Validadores de aplicação não substituem as validações do domínio. O primeiro
reduz métodos inflados e protege a entrada de um fluxo; o segundo protege a
invariante mesmo quando a entidade for usada por outro caso de uso.

As diferenças entre Policy, Specification e Validation, incluindo a estrutura
de pastas adotada, estão em
`docs/padrao-policies-specifications-validacoes.md`.

A seleção contextual de um resultado e os critérios para classes concretas ou
interfaces estão em `docs/padrao-resolvers.md`.

## Sinais de extração obrigatória

Extrair uma responsabilidade quando um serviço:

- contém regras de negócio que poderiam ser executadas sem repositório,
  transporte ou contexto do usuário;
- instancia entidades ou pedidos de domínio em vários métodos;
- repete seleção, normalização, formatação ou mapeamento;
- resolve candidatos ou fontes em ordem de prioridade dentro do orquestrador;
- mistura leitura de dados, decisão de regra, persistência e notificação no
  mesmo método;
- acumula blocos de validação que ocultam a sequência principal do caso de uso;
- precisa mudar por motivos independentes, como alteração de regra de
  aprovação e mudança do formato de uma notificação.

Não criar classes artificiais para operações curtas e estáveis. Uma extensão ou
auxiliar é preferível quando a operação é pura, pequena e não representa um
conceito de domínio. Quando a regra possui dependências, varia por contexto ou
precisa de testes próprios, ela deve receber um colaborador com nome do conceito
que processa. A interface só é necessária quando existir uma fronteira ou
substituição real.

As pastas `Records` e `Statics` descrevem a natureza técnica dos tipos, sem
criar uma nova camada arquitetural. Todo `record` da aplicação fica em uma
subpasta da capacidade proprietária dentro de `Records`; o nome do tipo
continua indicando se ele representa parâmetros, contexto, modelo ou contrato
e termina com o sufixo singular `Record`.
Da mesma forma, `Statics` não deve se tornar um agrupamento genérico de helpers;
ela é reservada a catálogos de constantes e operações cujo contrato seja
deliberadamente estático, puro e sem dependências.

## Aplicação no módulo de tarefas

`TarefaService` deve permanecer como orquestrador dos casos de uso de tarefa.
Ao evoluir o módulo, regras de obtenção, aprovação, elegibilidade, criação de
solicitação, resolução de aprovador, mapeamento e notificações devem ser
avaliadas contra este padrão antes de serem adicionadas ao serviço. A existência
de `TarefaPreparacaoService` e `TarefaNotificacaoService` é um ponto de partida,
mas não dispensa revisar as responsabilidades ainda concentradas em
`TarefaService`.

## Checklist de revisão

- O método evidencia a sequência do caso de uso sem esconder regras extensas
  entre condicionais?
- Cada regra tem um dono nomeado pelo conceito de domínio ou de aplicação?
- A validação de fluxo está fora do serviço quando torna a orquestração difícil
  de ler?
- A entidade continua protegendo suas próprias invariantes?
- Um mapeamento, formatação ou normalização simples pode ser reutilizado por
  auxiliar ou extensão?
- A nova interface reduz dependência real ou apenas cria uma camada de passagem?
- Uma seleção contextual possui um resolver explícito sem abstração genérica?
