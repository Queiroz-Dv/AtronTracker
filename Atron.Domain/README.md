## Projeto Domain

O projeto `Atron.Domain` é a camada de domínio da aplicação, seguindo os princípios do Domain-Driven Design (DDD). <br>
Ele contém entidades, interfaces para repositórios, classes de validação e objetos de valor.

## Componentes Principais

### Entidades

Localizadas na pasta `Entities`, essas classes representam os objetos principais de negócio.

### Interfaces

A pasta `Interfaces` contém interfaces de repositório para cada entidade, seguindo o padrão de Repositório.

### Validações

A pasta `Validations` contém classes de validação para várias entidades.

### Objetos de Valor

A pasta `ValueObjects` contém objetos de valor complexo usado dentro do domínio.

### Componentes Relacionados à API

- **ApiEntities/ApiRoute.cs**: Representação de rotas da API.
- **ApiInterfaces/ApiRouteRepository.cs**: Uma interface para gerenciar rotas da API.

## Uso

Esta camada de domínio deve ser usada como a lógica de negócio central e as regras para a aplicação. <br>
Ela deve ser independente de quaisquer preocupações de infraestrutura ou apresentação.

### Para usar este domínio na aplicação:

1. Implemente as interfaces de repositório na sua camada de infraestrutura.
2. Utilize as entidades para representar seus objetos de negócio.
3. Aplique as regras de validação das classes de validação ao criar ou atualizar entidades.
4. Utilize os objetos de valor onde for apropriado na sua lógica de domínio.