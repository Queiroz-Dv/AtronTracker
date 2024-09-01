## Projeto Infrastructure

O projeto `Atron.Infrastructure` é a camada de infraestrutura da aplicação, seguindo uma abordagem de arquitetura limpa ou design orientado a domínio (DDD). Ele contém o contexto do banco de dados, configurações de entidades, repositórios e componentes relacionados à API.

## Componentes Principais

### Contexto

- **AtronDbContext.cs**: Esta é a principal classe de contexto de banco de dados para o Entity Framework Core, representando a sessão com o banco de dados e permitindo operações de banco de dados.

### Configurações de Entidades

Localizadas na pasta `EntitiesConfiguration`, essas classes definem como as entidades são mapeadas para tabelas do banco de dados:

### Repositórios

A pasta `Repositories` contém implementações das interfaces de repositório, definidas na camada de domínio.

### Componentes Relacionados à API

- **ApiEntitiesConfiguration/ApiRouteConfiguration.cs**: Configuração para rotas da API.
- **ApiRepositories/ApiRouteRepository.cs**: Implementação do repositório para rotas da API.

### Migrações

A pasta `Migrations` define as migrações do Entity Framework Core para gerenciamento do esquema do banco de dados.

## Uso

Esta camada de infraestrutura deve ser usada para implementar as preocupações de persistência e acesso a dados da aplicação:

1. A classe `AtronDbContext` deve ser usada para interagir com o banco de dados.
2. As configurações de entidades na pasta `EntitiesConfiguration` definem como as entidades de domínio são mapeadas para tabelas do banco de dados.
3. As classes de repositório na pasta `Repositories` implementam operações de acesso a dados para cada entidade.
4. Componentes relacionados à API lidam com a configuração e acesso a dados para rotas da API.

### Para usar esta infraestrutura em sua aplicação:

1. Configure a conexão com o banco de dados na inicialização de sua aplicação.
2. Use a injeção de dependência para injetar o `AtronDbContext` e os repositórios onde necessário.
3. Use os repositórios para realizar operações CRUD em suas entidades.
4. Ao fazer mudanças no esquema do banco de dados, use migrações do EF Core para atualizar o banco de dados.