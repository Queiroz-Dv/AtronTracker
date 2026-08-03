## Projeto Infrastructure

O projeto `AtronTracker.Infrastructure` é a camada de infraestrutura do módulo
Tracker. Ele contém o contexto do banco de dados, configurações de entidades,
repositórios, migrations e a composição interna do módulo.

## Estrutura de Pastas

### Context
Pasta que define os contextos que serão utilizados para a comunicação com o banco de dados.

### Configurações de Entidades

Localizadas na pasta `EntitiesConfiguration`, essas classes definem como as entidades são mapeadas para tabelas do banco de dados:

### Repositórios

A pasta `Repositories` contém implementações das interfaces de repositório, definidas na camada de domínio.

### Composição de dependências

A pasta `DependencyInjection` contém o ponto público de composição do Tracker:

```csharp
services.AddTrackerModule(configuration);
```

Esse método registra os serviços de aplicação, mapeadores, validadores,
repositórios, `AtronDbContext`, Identity, autorização por módulo e integrações
pertencentes ao Tracker. Configurações HTTP, CORS, Swagger, cache e e-mail
permanecem sob responsabilidade do host e das capacidades transversais.

O vocabulário público das policies fica em
`Shared.Authorization.ModuloPolicies`. O provider e o handler permanecem aqui
como implementações transitórias da capacidade de identidade e acesso da
plataforma.

O Tracker e seus hosts não dependem mais do `Framework/IoC`.

### Migrações

As migrations ativas de Supabase/PostgreSQL ficam neste projeto:

- `AtronPlatform/Modules/Tracker/Infrastructure/Migrations`
- `AtronPlatform/Modules/Tracker/Infrastructure/Scripts`

O mesmo assembly mantém o `AtronDbContext`, as configurações de entidades, os repositórios e o histórico usado no deploy.

## Uso

Esta camada de infraestrutura deve ser usada para implementar as preocupações de persistência e acesso a dados da aplicação:

1. A classe `AtronDbContext` deve ser usada para interagir com o banco de dados.
2. As configurações de entidades na pasta `EntitiesConfiguration` definem como as entidades de domínio são mapeadas para tabelas do banco de dados.
3. As classes de repositório na pasta `Repositories` implementam operações de acesso a dados para cada entidade.
4. A composição do módulo é exposta por `AddTrackerModule`.

### Para usar esta infraestrutura em sua aplicação:

1. Configure a conexão com o banco de dados na inicialização da aplicação.
2. Chame `AddTrackerModule` no host que compõe o Tracker.
3. Use os repositórios para realizar operações CRUD em suas entidades.
4. Ao fazer mudanças no esquema do banco de dados, use migrações do EF Core para atualizar o banco de dados.
