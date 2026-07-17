# Orquestrar serviços de aplicação e segregar regras

## Contexto

O módulo de tarefas cresceu com regras de obtenção, solicitação, aprovação,
notificação, resolução de usuários, mapeamento e validação distribuídas em
`TarefaService`. Embora parte desse comportamento já tenha sido extraída para
colaboradores como `TarefaPreparacaoService` e `TarefaNotificacaoService`, o
serviço ainda concentra responsabilidades com razões de mudança independentes.

Manter esse padrão em novos serviços dificulta localizar regras, amplia a
superfície de teste e torna alterações de domínio mais arriscadas. A mesma
preocupação se aplica aos demais módulos do AtronRC.

## Decisão

Serviços de aplicação serão tratados como orquestradores de casos de uso. Eles
coordenam colaboradores, persistência e efeitos externos, mas não serão o dono
de regras de domínio, mapeamentos, criação de objetos ou blocos extensos de
validação.

As responsabilidades serão distribuídas da seguinte forma:

- regras, invariantes, transições e criação de objetos de negócio ficam em
  entidades, objetos de valor, fábricas ou interfaces de domínio do conceito
  processado;
- mapeamento fica em mapeadores ou colaboradores de preparação;
- validações de entrada e de fluxo que tornem o método extenso ficam em
  validadores específicos de aplicação;
- transformações simples, puras e reutilizáveis ficam em auxiliares ou métodos
  de extensão coesos;
- notificações e integrações permanecem em colaboradores especializados.

Validadores específicos de aplicação complementam os validadores e invariantes
do domínio. Eles não podem permitir que uma entidade inválida exista quando ela
for criada ou alterada por outro caminho.

Toda mudança em serviço deve avaliar SOLID, em especial responsabilidade única,
segregação de interfaces e inversão de dependência. Uma extração só é aceita
quando atribui a regra a um conceito claro e reduz acoplamento, não quando cria
uma classe de passagem sem comportamento próprio.

## Consequências

O fluxo principal dos serviços fica mais curto e torna explícita a ordem do
caso de uso. Regras passam a ter localidade e testes próprios. Em contrapartida,
o número de colaboradores pode aumentar, exigindo nomes claros e interfaces
pequenas.

O padrão detalhado e o checklist de revisão estão em
`docs/padrao-responsabilidades-servicos.md`. Refatorações futuras de
`TarefaService` devem usar esse documento como critério e preservar o contrato
funcional definido no ADR 0002.
