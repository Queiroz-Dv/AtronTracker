# Atron Platform

O Atron é uma plataforma comercial simples para negócios locais e pequenos empresários que desejam centralizar a gestão da empresa com custo inicial reduzido.

O projeto busca entregar tecnologia suficiente para fluxos reais de uma empresa pequena, mantendo arquitetura modular, documentação viva e capacidade de manutenção contínua. <br>
Ele também é uma trilha prática de formação em arquitetura de software, com foco em escalar, manter, corrigir e documentar um sistema com qualidade.

![Status do Projeto](https://img.shields.io/badge/Status-Em_Desenvolvimento-yellow)
![Licença](https://img.shields.io/badge/Licenca-MIT-blue)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)

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

## Tecnologias

| Tecnologia | Uso |
| ---------- | --- |
| .NET 8 | Backend e APIs |
| Entity Framework Core | Persistência |
| Supabase/PostgreSQL | Banco alvo para deploy inicial |
| Angular | Front principal em `AtronFront` |
| Swagger / Redoc | Documentação das APIs |

O front MVC/Razor legado foi removido do MVP pois não atendia às necessidades com
eventos específicos para o sistema. Adotamos então o Angular que está em `AtronFront`.

## Documentação principal

- [CONTEXT.md](CONTEXT.md): glossário canônico e contratos duráveis de domínio.
- [docs/visao-produto-atron.md](docs/visao-produto-atron.md): direção de produto da plataforma.
- [docs/publicacao-render-brevo-supabase.md](docs/publicacao-render-brevo-supabase.md): manual de publicação com Render, Supabase e Brevo.
- [docs/adr](docs/adr): decisões arquiteturais e de domínio.
- [docs/padrao-responsabilidades-servicos.md](docs/padrao-responsabilidades-servicos.md): critérios SOLID e distribuição de responsabilidades nos serviços de aplicação.
- [docs/plano-refatoracao-servicos-aplicacao.md](docs/plano-refatoracao-servicos-aplicacao.md): fases para aplicar o padrão aos serviços do Tracker.
- [docs/planejamento-custos-mvp.md](docs/planejamento-custos-mvp.md): contrato do módulo de planejamento de custos.

## Estrutura dos projetos

### Front

- `AtronFront`: interface Angular principal do produto.

### Tracker

- `AtronTracker/Domain`: entidades, contratos e regras centrais do Tracker.
- `AtronTracker/Application`: casos de uso, DTOs, validações e orquestração.
- `AtronTracker/Infrastructure`: persistência e repositórios.
- `AtronTracker/WebApi`: API do módulo Tracker.

### Stock

- `AtronStock/Domain`: domínio de estoque, suprimentos e bens.
- `AtronStock/Application`: serviços e validações do Stock.
- `AtronStock/Infrastructure`: persistência e repositórios.
- `AtronStock/WebApi`: API do módulo Stock.

### Framework

- [Framework/Shared](Framework/Shared/README.md): utilitários, recursos e componentes compartilhados.
- [Framework/IoC](Framework/IoC/README.md): injeção de dependência.

## Persistência

O projeto partiu do SQL Server, mas foi reestruturado para usar PostgreSQL como banco alvo do deploy. As migrations ativas ficam nos projetos `*.Migrations`.

## Configuração sensível

Credenciais não devem ser commitadas em `appsettings*.json`. Use variáveis de ambiente no Render e, localmente, variáveis de usuário, User Secrets ou `appsettings.Local.json`.

O passo a passo de publicação e o racional dos provedores estão em [docs/publicacao-render-brevo-supabase.md](docs/publicacao-render-brevo-supabase.md).

Variáveis principais:

- `ConnectionStrings__DefaultConnection`: string de conexão do PostgreSQL/Supabase.
- `ConnectionString`: fallback legado para a mesma conexão, quando necessário.
- `Jwt__SecretKey`: chave de assinatura dos tokens.
- `EmailSettings__Brevo__ApiKey`: chave da Brevo, se o provedor de e-mail estiver ativo.
- `EmailSettings__Password`: senha SMTP, se o provedor SMTP estiver ativo.

## Execução local

1. Instale o .NET SDK e as dependências do Angular.
2. Configure as strings de conexão e ambientes conforme o módulo desejado.
3. Rode a WebApi correspondente ao módulo.
4. Entre em `AtronFront` e execute:

```powershell
cd AtronFront
npm install
npm run start
```
