# Projeto Application

O projeto **Application** contém a lógica de aplicação, implementações de serviços, regras de negócios, e interfaces necessárias para orquestrar a interação entre diferentes camadas do sistema.

## Estrutura de Pastas

#### ApInterfaces
Esta pasta contém as interfaces relacionadas ao serviço de rotas da API.  
<hr>

#### ApiServices
Contém implementações dos serviços que utilizam as interfaces definidas na pasta
<hr>

### DTO (Data Transfer Objects)
Objetos de transferência de dados usados para transportar dados entre camadas da aplicação.
<hr>

### Records
Todos os tipos declarados como `record` no projeto `Application` devem ficar em
`Records`. A estrutura interna deve identificar a capacidade proprietária e o
namespace deve acompanhar a pasta, como `Application.Records.Tarefa` ou
`Application.Records.Usuario`.

O nome de toda classe declarada como `record` deve terminar com o sufixo
singular `Record`, como `ContextoNotificacaoTarefaRecord` e
`RotacaoRefreshTokenRecord`. O plural `Records` é reservado ao nome da pasta ou
a arquivos que agrupem vários records coesos, como `AcessoEmailRecords.cs`.

Um `record` pode expor propriedades derivadas, fábricas ou operações puras
relacionadas ao valor que representa. Antes do sufixo `Record`, o nome ainda
deve indicar sua função, como `Parametros`, `Context`, `Model` ou `Response`; a
pasta `Records` organiza o tipo técnico e não altera sua responsabilidade.

- `Records/Tarefa`: tipos pertencentes aos fluxos de tarefa;
- `Records/Usuario`: tipos pertencentes aos fluxos de usuário e acesso;
- `Records/Autenticacao`: tipos de token e autenticação;
- `Records/PlanejamentoCusto`: tipos do planejamento de custos.

Um tipo transversal só deve ficar diretamente em `Records` quando não existir
uma capacidade proprietária clara.
<hr>

### Statics
Classes cujo contrato é intencionalmente estático ficam em `Statics` e usam o
namespace `Application.Statics`. Essa pasta é destinada a catálogos de
constantes e operações puras e sem estado compartilhadas pela aplicação.

Não usar `Statics` como destino genérico para helpers. Comportamentos com
dependências, estado, variação de regra ou responsabilidade de caso de uso
devem permanecer em serviços, compositores, policies ou outros colaboradores
nomeados pelo conceito que representam.

Exemplo: `TarefaNotificacaoEventos` centraliza os identificadores dos eventos
de notificação de tarefa.
<hr>

### Interfaces
Interfaces que definem contratos para os serviços do domínio.
<hr>

### Mapping
Configuração de mapeamentos entre objetos de domínio e DTOs para facilitar a transferência de dados entre diferentes camadas.
<hr>

### Services
Implementações concretas dos serviços que manipulam as entidades e coordenam operações de negócios.
<hr>

### Policies
Decisões de negócio contextuais consumidas pelos serviços de aplicação. As
classes são agrupadas pela capacidade proprietária da regra, como `Tarefas` e
`PlanejamentoCustos`.
<hr>

### Resolvers
Seleção de um único resultado efetivo entre candidatos ou fontes contextuais.
Resolvers de aplicação são classes concretas `sealed` por padrão e ficam em
`Resolvers/<Capacidade>`.
<hr>

### Specifications
Critérios booleanos reutilizáveis sobre um candidato. Specifications não
substituem policies que precisam informar o motivo de uma decisão.
<hr>

### Validador e Validations
Validações de formação e coerência das entradas da aplicação.

O contrato detalhado entre essas responsabilidades está em
`docs/padrao-policies-specifications-validacoes.md` e
`docs/padrao-resolvers.md`.
