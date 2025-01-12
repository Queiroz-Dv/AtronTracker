# Projeto External Services

O projeto `External Services` é responsável pela implementação de serviços externos para manipulação dos módulos.
As interfaces definem os contratos e os serviços implementam a lógica de integração com APIs externas.

## Estrutura do Projeto

### Interfaces

#### 1. `IApiRouteExternalService`

- **Descrição**: Interface dos processos e fluxos do módulo de rotas da API.
- **Métodos**:
  - `Cadastrar(ApiRoute route, string rotaDoConnect)`: Método para cadastrar uma rota.
  - `ObterRotas(string rotaPrincipal)`: Método para obter todas as rotas.

#### 2. `IExternalMessageService`

- **Descrição**: Interface de acesso ao serviço de mensagens externas.
- **Propriedades**:
  - `List<ResultResponseDTO> ResultResponses`: Lista de respostas de resultados.

#### 3. `ICargoExternalService`

- **Descrição**: Interface dos processos e fluxos do módulo de Cargos.
- **Métodos**:
  - `Atualizar(string codigo, CargoDTO cargoDTO)`: Atualiza um cargo existente.
  - `Criar(CargoDTO cargoDTO)`: Cria um novo cargo.
  - `ObterTodos()`: Obtém todos os cargos.
  - `Remover(string codigo)`: Remove um cargo por código.

#### 4. `IDepartamentoExternalService`

- **Descrição**: Interface dos processos e fluxos do módulo de Departamentos.
- **Métodos**:
  - `ObterTodos()`: Obtém todos os departamentos.
  - `Criar(DepartamentoDTO departamento)`: Cria um novo departamento.
  - `Atualizar(string codigo, DepartamentoDTO departamentoDTO)`: Atualiza um departamento existente.
  - `Remover(string codigo)`: Remove um departamento por código.

#### 5. `IUsuarioExternalService`

- **Descrição**: Interface dos processos e fluxos do módulo de Usuários.
- **Métodos**:
  - `Criar(UsuarioDTO usuarioDTO)`: Cria um novo usuário.
  - `ObterTodos()`: Obtém todos os usuários.

### Implementações de Serviços

#### 1. `ApiRouteExternalService`

- **Descrição**: Implementação dos processos do módulo de rotas da API.
- **Métodos**:
  - `Cadastrar(ApiRoute route, string rotaDoConnect)`: Serializa a rota e faz uma requisição POST para cadastrar.
  - `ObterRotas(string rotaPrincipal)`: Faz uma requisição GET para obter todas as rotas.

#### 2. `CargoExternalService`

- **Descrição**: Implementação dos processos do módulo de cargos.
- **Métodos**:
  - `Criar(CargoDTO cargoDTO)`: Serializa o cargo e faz uma requisição POST para criar.
  - `ObterTodos()`: Faz uma requisição GET para obter todos os cargos.
  - `Atualizar(string codigo, CargoDTO cargoDTO)`: Atualiza o cargo com base no código.
  - `Remover(string codigo)`: Remove um cargo com base no código.

#### 3. `DepartamentoExternalService`

- **Descrição**: Implementação dos processos do módulo de departamentos.
- **Métodos**:
  - `Criar(DepartamentoDTO departamento)`: Serializa o departamento e faz uma requisição POST para criar.
  - `ObterTodos()`: Faz uma requisição GET para obter todos os departamentos.
  - `Atualizar(string codigo, DepartamentoDTO departamentoDTO)`: Atualiza o departamento com base no código.
  - `Remover(string codigo)`: Remove um departamento com base no código.

#### 4. `UsuarioExternalService`

- **Descrição**: Implementação dos processos do módulo de usuários.
- **Métodos**:
  - `Criar(UsuarioDTO usuarioDTO)`: Serializa o usuário e faz uma requisição POST para criar.
  - `ObterTodos()`: Faz uma requisição GET para obter todos os usuários.

## Resumo

O projeto `ExternalServices` é uma parte crítica da aplicação, pois lida com integrações externas e manipulação de dados de forma eficiente e organizada. Cada serviço é responsável por um módulo específico, garantindo assim uma separação clara de responsabilidades e facilitando a manutenção e expansão do código.
