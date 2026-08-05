# Decisões arquiteturais do Atron

Os ADRs são organizados pelo módulo proprietário da decisão. Contratos que
afetam mais de um módulo ficam em `transversais/` para não atribuir uma decisão
da plataforma artificialmente ao Tracker ou ao Stock.

A numeração continua sendo global e sequencial. A pasta identifica o
proprietário ou o alcance, mas não reinicia a sequência.

## Atron Tracker

| ADR | Status | Decisão |
|---|---|---|
| [0001](./tracker/0001-substituir-salarios-por-planejamento-de-custos.md) | Aceito | Substituir salários por planejamento de custos. |
| [0002](./tracker/0002-remodelar-tarefas-com-gestao-e-notificacoes-internas.md) | Aceito | Remodelar tarefas com gestão e notificações internas. |

## Atron Stock

| ADR | Status | Decisão |
|---|---|---|
| [0006](./stock/0006-entregar-rotina-de-categorias-no-atron-stock.md) | Suspenso | Retomar Categoria somente após a conclusão das Fases 1 a 9 da reestruturação modular. |

## Decisões transversais

| ADR | Status | Decisão |
|---|---|---|
| [0003](./transversais/0003-padronizar-resources-notificacoes-e-templates-email.md) | Aceito | Padronizar resources, notificações internas e templates de e-mail. |
| [0004](./transversais/0004-orquestrar-servicos-de-aplicacao-e-segregar-regras.md) | Aceito | Tratar serviços de aplicação como orquestradores e segregar regras. |
| [0005](./transversais/0005-extrair-notificacoes-internas-como-capacidade-transversal.md) | Substituído parcialmente | Extrair notificações internas como capacidade transversal; a topologia HTTP foi substituída pelo ADR 0008. |
| [0007](./transversais/0007-adotar-monolito-modular-com-host-neutro.md) | Aceito | Adotar monólito modular com host neutro e publicação única para os módulos principais. |
| [0008](./transversais/0008-incorporar-notificacoes-internas-ao-host-da-plataforma.md) | Aceito | Incorporar Notificações Internas ao host neutro sem perder sua fronteira e propriedade de dados. |
| [0009](./transversais/0009-adotar-redis-como-cache-distribuido.md) | Aceito | Adotar Redis compatível como provider distribuído do cache transversal da plataforma. |

## Convenções

- Um ADR novo deve ser criado na pasta do módulo proprietário ou em
  `transversais/`.
- O status deve distinguir decisão proposta, aceita, rejeitada ou substituída.
- Um ADR proposto não representa funcionalidade implementada.
- Mudanças de caminho devem atualizar todas as referências do repositório.
- Decisões de domínio duráveis também devem ser refletidas no `CONTEXT.md`
  quando forem aceitas.
