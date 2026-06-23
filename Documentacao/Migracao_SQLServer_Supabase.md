# Documentação da Migração para Supabase

Este documento registra a estratégia atual do projeto Atron para migrar do SQL Server para o Supabase/PostgreSQL.

A decisão atual é usar o Supabase como banco principal para viabilizar o deploy do sistema com menor custo inicial, evitando a necessidade de contratar ou manter uma instância SQL Server no início da operação.

## Objetivo da migração

Migrar o backend do Atron para Supabase/PostgreSQL mantendo a arquitetura atual da aplicação .NET, seus serviços, repositórios, contexts e fluxos de autenticação.

O objetivo principal não é trocar toda a arquitetura do sistema. O objetivo é substituir a infraestrutura de banco por uma opção mais adequada para deploy inicial, com menor atrito operacional e menor custo de entrada.

## Por que migrar para Supabase?

### Redução de custo inicial

O SQL Server tende a gerar custo maior em ambientes publicados, principalmente quando envolve instância gerenciada, licenciamento, planos de cloud ou infraestrutura dedicada.

O Supabase oferece PostgreSQL gerenciado e permite iniciar o deploy com custo menor, preservando um banco relacional robusto.

### Facilidade para deploy

Usar Supabase simplifica a publicação inicial porque o banco já fica disponível como serviço gerenciado. Isso reduz a necessidade de provisionar SQL Server separadamente para os primeiros ambientes.

### Continuidade da arquitetura

A aplicação continua usando Entity Framework Core, `DbContext`, repositórios, services e autenticação atual. O Supabase entra como provider PostgreSQL, não como substituto imediato da arquitetura do backend.

### Evolução futura

Com o sistema rodando sobre Supabase/PostgreSQL, poderemos avaliar recursos adicionais depois, como Realtime, storage, edge functions ou RLS. Esses recursos não fazem parte da primeira etapa.

## Decisão sobre SQL Server

O SQL Server deixa de ser o destino principal do projeto e passa a ter papel de origem legada e referência de transição.

Ele ainda é importante por três motivos:

- contém o histórico original de migrations e dados;
- serve como origem para cargas pontuais ou validações durante a transição;
- permite comparar comportamento caso alguma regra precise ser auditada.

Após a estabilização no Supabase, o SQL Server pode ser mantido apenas como referência histórica, backup ou ambiente legado. Não é necessário manter SQL Server como provider ativo em produção se o deploy será feito diretamente com Supabase.

## Provider de banco

Foi criado um resolvedor centralizado de provider em:

- `Framework/IoC/DatabaseProvider.cs`

Providers aceitos:

- `SqlServer`
- `PostgreSql`
- `Postgres`
- `Supabase`

O provider `Supabase` é tratado como PostgreSQL e usa `Npgsql.EntityFrameworkCore.PostgreSQL`.

Exemplo de configuração para desenvolvimento local com Supabase:

```json
{
  "Database": {
    "Provider": "Supabase",
    "ConnectionStringName": "DefaultConnection"
  }
}
```

Essa configuração permite testar localmente contra o Supabase, que passa a representar o ambiente alvo do deploy.

## Contexts migrados

Foram preparados três contexts para PostgreSQL/Supabase:

- `AtronDbContext`
- `SharedDbContext`
- `StockDbContext`

Cada context recebeu ajustes para compatibilidade com Npgsql, principalmente no mapeamento de `DateTime` como:

```sql
timestamp without time zone
```

No `SharedDbContext`, também foram tratados defaults específicos do PostgreSQL para auditoria e histórico.

## Projetos de migrations PostgreSQL

Foram criados projetos separados de migrations PostgreSQL:

- `AtronTracker/Infrastructure.PostgreSqlMigrations`
- `Framework/Shared.PostgreSqlMigrations`
- `AtronStock/Infrastructure.PostgreSqlMigrations`

## Necessidade de manter migrations separadas

Enquanto o projeto ainda possui migrations SQL Server existentes, faz sentido manter projetos separados para PostgreSQL.

Motivos:

- migrations EF Core geradas para SQL Server e PostgreSQL possuem tipos, defaults, sequences e comandos diferentes;
- misturar migrations dos dois providers no mesmo assembly aumenta o risco de gerar scripts inválidos;
- manter assemblies separados permite gerar, aplicar e revisar scripts PostgreSQL sem alterar o histórico original do SQL Server;
- a separação ajuda durante a fase de transição e auditoria.

Portanto, a estrutura atual com projetos distintos de migrations é adequada neste momento.

Depois que o Supabase estiver estabilizado e o SQL Server for considerado legado definitivo, existem duas opções:

- manter as migrations SQL Server como histórico, sem uso operacional;
- arquivar ou remover o fluxo SQL Server em uma etapa futura, caso o projeto decida não oferecer mais suporte a esse provider.

Essa remoção não deve ser feita agora porque ainda estamos na transição e o histórico SQL Server ajuda na comparação e rastreabilidade.

## Endpoint interno de teste de conexão

Foi criado um endpoint interno para validar conexões de banco sem expor isso ao usuário final.

Endpoints:

- `GET /api/ConexaoBanco/supabase`
- `GET /api/ConexaoBanco/sql-server`
- `POST /api/ConexaoBanco/testar`

Esse endpoint deve continuar restrito a ambiente de desenvolvimento ou habilitação explícita.

## Migrador de dados

O migrador de dados foi criado em:

- `Tools/DatabaseMigrator`

Ele transfere dados do SQL Server para o Supabase/PostgreSQL usando os metadados dos próprios `DbContext`.

O migrador não deve ser tratado como parte permanente do runtime da aplicação. Ele é uma ferramenta operacional para casos específicos.

Casos de uso previstos:

- carga inicial do Supabase a partir da base SQL Server;
- recarga controlada em ambiente de homologação;
- validação pontual entre origem SQL Server e destino Supabase;
- reprocessamento específico caso algum dado legado precise ser reenviado.

Após a migração principal, a aplicação deve operar diretamente no Supabase. O migrador fica disponível apenas para necessidades específicas.

## Estrutura atual do migrador

O migrador foi remodelado para separar responsabilidades:

```text
Tools/DatabaseMigrator
  Application
    MigrationRunner.cs
  Domain
    ColumnPlan.cs
    DatabaseKind.cs
    MigrationMode.cs
    TableKey.cs
    TablePlan.cs
  Infrastructure
    MigrationContextDefinition.cs
    MigrationContextFactory.cs
    RelationalDataGateway.cs
    SqlIdentifier.cs
  Presentation
    ConsoleReporter.cs
    MigratorOptions.cs
  Program.cs
```

Responsabilidades:

- `Program.cs`: entrada do console e composição simples das dependências;
- `Presentation`: parsing de argumentos e saída no console;
- `Application`: orquestração do fluxo de migração;
- `Domain`: representação do plano de tabelas, colunas e modo de execução;
- `Infrastructure`: criação dos contexts, leitura SQL Server, escrita PostgreSQL e comandos relacionais.

## Modos do migrador

- `dry-run`: exibe contagens da origem e do destino, sem gravar dados;
- `migrate`: copia os dados da origem para o destino;
- `validate`: compara contagens entre SQL Server e Supabase.

Exemplo:

```powershell
dotnet run --project Tools\DatabaseMigrator\DatabaseMigrator.csproj -- `
  --mode dry-run `
  --contexts all `
  --source "<connection-string-sql-server>" `
  --target "<connection-string-supabase>"
```

Exemplo com limpeza do destino:

```powershell
dotnet run --project Tools\DatabaseMigrator\DatabaseMigrator.csproj -- `
  --mode migrate `
  --contexts all `
  --reset-target `
  --source "<connection-string-sql-server>" `
  --target "<connection-string-supabase>"
```

## Estado atual da base Supabase

As migrations PostgreSQL foram aplicadas no Supabase para os três contexts:

- `AtronDbContext`
- `SharedDbContext`
- `StockDbContext`

Os dados do SQL Server foram copiados para o Supabase e validados por contagem.

## RLS e Supabase Auth

Neste momento, a decisão é manter a autenticação atual do projeto com ASP.NET Identity e JWT.

O Supabase será usado como PostgreSQL gerenciado. RLS e Supabase Auth não serão ativados nesta fase.

Motivos:

- o front-end não acessa o Supabase diretamente;
- a API .NET centraliza autenticação, autorização e regras de negócio;
- ativar RLS sem redesenhar o fluxo de autenticação pode gerar bloqueios ou falsa sensação de segurança;
- migrar para Supabase Auth exigiria refazer usuários, roles, claims, tokens e fluxos de login.