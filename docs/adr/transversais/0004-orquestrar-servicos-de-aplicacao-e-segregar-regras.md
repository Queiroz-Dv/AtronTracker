# ADR 0004: Orquestrar serviços de aplicação e segregar regras

## Status

Aceito.

## Contexto

O módulo de tarefas cresceu com regras de obtenção, solicitação, aprovação, notificação, resolução de usuários, mapeamento e validação distribuídas em `TarefaService`. Parte desse comportamento já foi extraída para colaboradores como `TarefaPreparacaoService` e `TarefaNotificacaoService`, mas serviços extensos ainda podem concentrar responsabilidades com razões de mudança independentes.

Repetir esse padrão em novos módulos dificulta localizar regras, amplia a superfície de teste e torna alterações de domínio mais arriscadas.

## Decisão

Serviços de aplicação serão orquestradores de casos de uso. Eles coordenam colaboradores, persistência e efeitos externos, mas não serão proprietários de regras de domínio, mapeamentos, criação de objetos ou blocos extensos de validação.

```mermaid
flowchart LR
    A["Entrada do caso de uso"] --> B["Orquestrador de aplicação"]
    B --> C["Validador de fluxo"]
    B --> D["Domínio e invariantes"]
    B --> E["Mapeador ou preparador"]
    B --> F["Persistência"]
    B --> G["Notificações e integrações"]
    C --> H["Resultado"]
    D --> H
    E --> H
    F --> H
    G --> H
```

As responsabilidades serão distribuídas assim:

| Responsabilidade | Local preferencial |
|---|---|
| Regras, invariantes e transições | Entidades, objetos de valor, fábricas ou políticas de domínio. |
| Mapeamento e preparação | Mapeadores ou colaboradores especializados. |
| Validação extensa de entrada e fluxo | Validadores específicos de aplicação. |
| Transformações simples, puras e reutilizáveis | Auxiliares ou métodos de extensão coesos. |
| Notificações e integrações | Colaboradores especializados e contratos explícitos. |
| Ordem do caso de uso | Serviço orquestrador. |

### Critérios de Design: Use Cases vs. Services

Para evitar overengineering e granularidade excessiva, aplicam-se as seguintes regras de limite estrutural:

1. **Critério para criação de Use Cases (Granularidade):**
   Um Use Case só deve ser extraído como uma classe própria se corresponder a uma ação acionável do usuário (um endpoint/ação específica) OU se for explicitamente reutilizado por outros Use Cases. Blocos lógicos ou auxiliares não reutilizáveis devem ser mantidos como métodos privados ou colaboradores internos do caso de uso principal.

2. **Critério para existência de Services (Orquestração vs. Pass-Through):**
   Serviços de aplicação de domínio (ex: `TarefaService`) só devem existir se realizarem orquestração real: composição de múltiplos Use Cases, gerenciamento transacional de operações compostas, ou lógica de *cross-cutting* integrada. 
   **É vetada a criação de "Services de passagem"** que operam em 1:1 repassando chamadas sem adicionar lógica. Para operações auto-contidas, os Controllers da Web API devem consumir as classes de Use Case diretamente.

Validadores de aplicação complementam as invariantes do domínio. Eles não podem permitir que uma entidade inválida exista quando for criada ou alterada por outro caminho.

Toda mudança em serviço deve avaliar, principalmente, responsabilidade única, segregação de interfaces e inversão de dependência. Uma extração só é aceita quando atribui a regra a um conceito claro e reduz acoplamento. Classes de passagem sem responsabilidade própria não atendem essa decisão e devem ser removidas.

## Alternativas consideradas

### Manter toda a lógica no serviço principal

Rejeitada porque aumenta o acoplamento e obriga testes de comportamentos independentes a compartilhar a mesma unidade extensa.

### Extrair cada bloco para uma nova classe

Rejeitada como regra automática. A quantidade de classes não mede qualidade; cada colaborador precisa possuir responsabilidade e contrato claros.

### Mover validações de fluxo para as entidades

Rejeitada quando a validação depende de repositórios, identidade autenticada ou integração externa. Nesses casos, a aplicação coordena e o domínio preserva apenas suas invariantes próprias.

## Consequências

- o fluxo principal dos serviços fica mais curto e explicita a ordem do caso de uso;
- regras passam a ter localidade e testes próprios;
- o número de colaboradores pode aumentar, exigindo nomes claros e interfaces pequenas;
- mudanças devem preservar contratos funcionais e comparar consumidores antes de remover compatibilidade;
- composição é preferida a herança quando não existir uma abstração de domínio comprovada.

## Validação

O padrão detalhado e o checklist de revisão estão em `docs/padrao-responsabilidades-servicos.md`. Refatorações de `TarefaService` devem usar esse documento como critério e preservar o contrato funcional do ADR 0002.
