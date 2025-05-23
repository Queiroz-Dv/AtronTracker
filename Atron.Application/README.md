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

### Interfaces
Interfaces que definem contratos para os serviços do domínio.
<hr>

### Mapping
Configuração de mapeamentos entre objetos de domínio e DTOs para facilitar a transferência de dados entre diferentes camadas.
<hr>

### Services
Implementações concretas dos serviços que manipulam as entidades e coordenam operações de negócios.
<hr>

### Specifications
Classes de regras específicas para validação de determinados fluxos de negócios
<hr>

### Validations
Classes de implementações das validações de cada entidade