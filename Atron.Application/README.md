# Atron.Application

O projeto **Atron.Application** contém a lógica de aplicação, implementações de serviços e regras de negócios, e interfaces necessárias para orquestrar a interação entre diferentes camadas do sistema. Abaixo, uma descrição detalhada de cada pasta e arquivo dentro deste projeto.

## Estrutura do Projeto

### Estrutura para os processos de API

#### 1. ApInterfaces
Esta pasta contém as interfaces relacionadas ao serviço de rotas da API.  
- **ApiInterfaces/ApiRouteService.cs**: Interface que define os métodos para gerenciamento de rotas de APIs.

#### 2. ApiServices
Contém implementações dos serviços que utilizam as interfaces definidas na pasta `ApInterfaces`.

- **ApiServices/ApiRouteService.cs**: Implementação do serviço para gerenciamento de rotas de APIs, lidando com a lógica de criação, atualização, e remoção de rotas.
<hr>

### 3. DTO (Data Transfer Objects)
Objetos de transferência de dados usados para transportar dados entre camadas da aplicação.

- **DTO/CargoDTO.cs**: DTO para representar dados relacionados a cargos.
- **DTO/DepartamentoDTO.cs**: DTO para representar dados relacionados a departamentos.
- **DTO/Factory.cs**: Fábrica para criar instâncias de DTOs.
- **DTO/MesDTO.cs**: DTO para representar dados relacionados aos meses.
- **DTO/PermissaoDTO.cs**: DTO para representar dados de permissões.
- **DTO/SalarioDTO.cs**: DTO para representar dados de salários.
- **DTO/TarefaDTO.cs**: DTO para representar dados de tarefas.
- **DTO/UsuarioDTO.cs**: DTO para representar dados de usuários.

### 4. Enums
Define tipos enumerados que são usados em várias partes do aplicativo.

- **Enums/EPermissaoEstados.cs**: Enum que define os diferentes estados de permissões.

### 5. Interfaces
Interfaces que definem contratos para os serviços do domínio.

- **Interfaces/ICargoService.cs**: Interface para serviço de cargos.
- **Interfaces/IDepartamentoService.cs**: Interface para serviço de departamentos.
- **Interfaces/IMesService.cs**: Interface para serviço de meses.
- **Interfaces/IPermissaoService.cs**: Interface para serviço de permissões.
- **Interfaces/ISalarioService.cs**: Interface para serviço de salários.
- **Interfaces/ITarefaService.cs**: Interface para serviço de tarefas.
- **Interfaces/IUsuarioService.cs**: Interface para serviço de usuários.

### 6. Mapping
Configuração de mapeamentos entre objetos de domínio e DTOs para facilitar a transferência de dados entre diferentes camadas.

- **Mapping/DomainToDtoMappingProfile.cs**: Perfil de mapeamento que define como os objetos de domínio são convertidos em DTOs e vice-versa.

### 7. Services
Implementações concretas dos serviços que manipulam as entidades e coordenam operações de negócios.

- **Services/CargoService.cs**: Implementação do serviço para gerenciar operações relacionadas a cargos.
- **Services/DepartamentoService.cs**: Implementação do serviço para gerenciar operações relacionadas a departamentos.
- **Services/MesService.cs**: Implementação do serviço para gerenciar operações relacionadas aos meses.
- **Services/PermissaoService.cs**: Implementação do serviço para gerenciar operações relacionadas a permissões.
- **Services/SalarioService.cs**: Implementação do serviço para gerenciar operações relacionadas a salários.
- **Services/TarefaService.cs**: Implementação do serviço para gerenciar operações relacionadas a tarefas.
- **Services/UsuarioService.cs**: Implementação do serviço para gerenciar operações relacionadas a usuários.

## Resumo

A estrutura do projeto **Atron.Application** é organizada para separar claramente as responsabilidades de diferentes componentes,
facilitando a manutenção e a evolução da aplicação. Cada pasta e arquivo serve a um propósito específico dentro da lógica de aplicação,
garantindo que a arquitetura permaneça limpa e modular.