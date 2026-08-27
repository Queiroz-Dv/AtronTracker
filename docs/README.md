# Documentação do Atron

## Começar e contribuir

- [Apresentação e capturas da plataforma](../README.md).
- [Execução local com Visual Studio e Angular](desenvolvimento/execucao-local.md).
- [Branches, commits, PRs e critérios de conclusão](../CONTRIBUTING.md).
- [Fluxo Git](desenvolvimento/fluxo-git.md).
- [Arquitetura de testes](arquitetura-testes.md).

## Produto e arquitetura

- [Linguagem e contratos do domínio](../CONTEXT.md).
- [Visão de produto](visao-produto-atron.md).
- [Decisões arquiteturais](adr/README.md).
- [Fronteiras e propriedade da plataforma](arquitetura/fronteiras-e-propriedade-da-plataforma.md).
- [Responsabilidades dos serviços](padrao-responsabilidades-servicos.md).
- [Stock: implementação atual e próximos contratos](../AtronPlatform/Modules/Stock/README.md).
- [Frontend: estrutura, ambientes e scripts](../AtronFront/README.md).

## Operação e versões

- [Publicação e releases](operacao/releases.md).
- [Transição das branches em 27/08/2026](operacao/transicao-branches-2026-08-27.md).
- [Configuração de Render, Supabase e Brevo](publicacao-render-brevo-supabase.md).
- [Cache Redis](cache-redis.md).
- [Operação das notificações](operacao-notificacoes-internas.md).
- [Notas de versão](releases/README.md) e [changelog](../CHANGELOG.md).

## Como manter

`CONTEXT.md` descreve regras duráveis; ADRs registram decisões e alternativas;
guias de operação descrevem procedimentos; planos descrevem trabalho ainda não
entregue. Todo plano deve informar seu status e limites de validação.

Não mova documentos antigos em massa. Reorganize por assunto em mudanças
pequenas e atualize seus links. Conteúdo substituído deve apontar para a decisão
atual, sem apagar a justificativa histórica. Notas de release descrevem entregas,
não promessas de um plano.
