# Atron Platform

O Atron é uma plataforma comercial simples para negócios locais e pequenos empresários que desejam centralizar a gestão da empresa com custo inicial reduzido.

O projeto busca entregar tecnologia suficiente para fluxos reais de uma empresa pequena, mantendo arquitetura modular, documentação viva e capacidade de manutenção contínua. <br>
Ele também é uma trilha prática de formação em arquitetura de software, com foco em escalar, manter, corrigir e documentar um sistema com qualidade.

![Status do Projeto](https://img.shields.io/badge/Status-Em_Desenvolvimento-yellow)
![Licença](https://img.shields.io/badge/Licenca-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-9.0-purple)

## Visão de produto

A direção de produto está documentada em [docs/visao-produto-atron.md](docs/visao-produto-atron.md).

O Atron deve crescer como uma plataforma de gestão comercial de porte enxuto: simples para rodar em um negócio pequeno, mas estruturada o suficiente para evoluir sem virar uma coleção de telas desconectadas.

## Módulos

### Atron Tracker

Módulo de gestão interna e estrutura organizacional. Cobre usuários, departamentos, cargos, perfis de acesso, tarefas, notificações internas, planejamento de custos e rotinas administrativas.

### Atron Stock

Módulo destinado a suprimentos, estoque, patrimônio e bens da empresa. Cobre produtos, fornecedores, clientes ligados ao fluxo de estoque, entradas, saídas, movimentações, saldos e rastreabilidade.

### Atron Sales

Módulo planejado para comercial e financeiro. Deve concentrar vendas, recebimentos, contas, formas de pagamento e relatórios comerciais ou financeiros quando seu escopo for formalizado.

## Direção arquitetural

O produto principal adota um monólito modular: Tracker, Stock e o futuro Sales
permanecem módulos pares, compostos e publicados por um único host neutro
`AtronPlatform.WebApi`. Os hosts transitórios de Tracker, Stock, Auditoria e
Notificações Internas foram removidos, conforme os
[ADR 0007](docs/adr/transversais/0007-adotar-monolito-modular-com-host-neutro.md)
e [ADR 0008](docs/adr/transversais/0008-incorporar-notificacoes-internas-ao-host-da-plataforma.md).

## Tecnologias

| Tecnologia | Uso |
| ---------- | --- |
| .NET 9 | Backend e APIs |
| Entity Framework Core | Persistência |
| Supabase/PostgreSQL | Banco alvo para deploy inicial |
| Redis / Render Key Value | Cache distribuído de acesso e dados temporários |
| Angular | Front principal em `AtronFront` |
| Swagger / Redoc | Documentação das APIs |

O front MVC/Razor legado foi removido do MVP pois não atendia às necessidades com
eventos específicos para o sistema. Adotamos então o Angular que está em `AtronFront`.

## Documentação principal

- [CONTRIBUTING.md](CONTRIBUTING.md): branches, commits, pull requests e validações.
- [docs/README.md](docs/README.md): índice da documentação por finalidade.
- [docs/operacao/releases.md](docs/operacao/releases.md): versões, publicação e release notes.
- [CHANGELOG.md](CHANGELOG.md): índice das mudanças e versões formalizadas.
- [CONTEXT.md](CONTEXT.md): glossário canônico e contratos duráveis de domínio.
- [docs/visao-produto-atron.md](docs/visao-produto-atron.md): direção de produto da plataforma.
- [docs/publicacao-render-brevo-supabase.md](docs/publicacao-render-brevo-supabase.md): manual de publicação com Render, Supabase e Brevo.
- [docs/cache-redis.md](docs/cache-redis.md): decisão aplicada, configuração local e operação do cache Redis compatível.
- [docs/adr](docs/adr/README.md): índice das decisões arquiteturais e de domínio, organizadas por módulo e por alcance transversal.
- [docs/padrao-responsabilidades-servicos.md](docs/padrao-responsabilidades-servicos.md): critérios SOLID e distribuição de responsabilidades nos serviços de aplicação.
- [docs/plano-refatoracao-servicos-aplicacao.md](docs/plano-refatoracao-servicos-aplicacao.md): fases para aplicar o padrão aos serviços do Tracker.
- [docs/planejamento-custos-mvp.md](docs/planejamento-custos-mvp.md): contrato do módulo de planejamento de custos.

## Estrutura atual dos projetos

### Front

- `AtronFront`: interface Angular principal do produto.

### Tracker

- `AtronPlatform/Modules/Tracker/Domain`: entidades, contratos e regras centrais do Tracker.
- `AtronPlatform/Modules/Tracker/Application`: casos de uso, DTOs, validações e orquestração.
- `AtronPlatform/Modules/Tracker/Infrastructure`: persistência e repositórios.
- `AtronPlatform/WebApi`: único host que compõe e publica Tracker, Stock, Auditoria e Notificações Internas.

### Stock

- `AtronPlatform/Modules/Stock/Domain`: domínio de estoque, suprimentos e bens.
- `AtronPlatform/Modules/Stock/Application`: serviços e validações do Stock.
- `AtronPlatform/Modules/Stock/Infrastructure`: persistência, migrations,
  repositórios e composição `AddStockModule`.
- `AtronPlatform/WebApi/Controllers/Stock`: controllers HTTP do módulo no host
  neutro.

### Framework

- [Framework/Shared](Framework/Shared/README.md): utilitários, recursos e componentes compartilhados.
- `Framework/AtronNotificacoes`: capacidade transversal in-process, com contratos,
  aplicação, persistência e migrations próprios.

## Persistência

O projeto partiu do SQL Server, mas foi reestruturado para usar PostgreSQL como
banco alvo do deploy. Cada DbContext mantém migrations sob seu proprietário:
Tracker e Stock em suas respectivas Infrastructures, Auditoria em
`Framework.Shared.Migrations` e AtronNotificacoes em seu Core.

## Configuração sensível

Credenciais não devem ser commitadas em `appsettings*.json`. Use variáveis de ambiente no Render e, localmente, variáveis de usuário, User Secrets ou `appsettings.Local.json`.

O passo a passo de publicação e o racional dos provedores estão em [docs/publicacao-render-brevo-supabase.md](docs/publicacao-render-brevo-supabase.md).

Variáveis principais:

- `ATRON_CONNECTION_STRING`: string de conexão canônica do PostgreSQL/Supabase.
- `Jwt__SecretKey`: chave de assinatura dos tokens.
- `EmailSettings__Brevo__ApiKey`: chave da Brevo, se o provedor de e-mail estiver ativo.
- `EmailSettings__Password`: senha SMTP, se o provedor SMTP estiver ativo.

## Execução local

1. Instale o .NET SDK e as dependências do Angular.
2. Configure as strings de conexão e ambientes conforme o módulo desejado.
3. Rode `AtronPlatform/WebApi/AtronPlatform.WebApi.csproj` para o núcleo da plataforma.
4. Entre em `AtronFront` e execute:

```powershell
cd AtronFront
npm install
npm run start
```
