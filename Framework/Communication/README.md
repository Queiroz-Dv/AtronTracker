# Projeto Communication


O projeto **Communication** é responsável por gerenciar a comunicação com APIs externas
<br> e por lidar com o tratamento de respostas de APIs. <br>
Ele é estruturado em diferentes pastas, cada uma com responsabilidades específicas para manter a modularidade e clareza do código.

## Estrutura do Projeto

### 1. Extensions
Contém classes de métodos de extensão que auxiliam na manipulação de dados de resposta da API.

- **CommunicationExtensions.cs**: Classe que contém métodos de extensão para manipular listas de respostas da API. <br>
<br> Exemplo de método:
  - `HasErrors()`: Verifica se a lista de respostas contém algum erro.

### 2. Interfaces
Define contratos para a comunicação e operações da API. A pasta está dividida em duas subpastas:

#### a. Interfaces/Services
Contém a interface para serviços de comunicação que lidam com as respostas da API.

- **ICommunicationService.cs**: Define métodos para adicionar e obter conteúdos de resposta da API. <br>
<br> Métodos principais incluem:
  - `AddResponseContent(ResultResponseDTO resultResponse)`: Adiciona uma resposta.
  - `AddResponseContent(List<ResultResponseDTO> resultResponses)`: Adiciona múltiplas respostas.
  - `GetResultResponses()`: Retorna todas as respostas armazenadas.

#### b. Interfaces
Contém interfaces que lidam com operações do cliente da API.

- **IApiClient.cs**: Define operações básicas para interagir com APIs usando diferentes verbos HTTP:
  - `GetAsync(string uri)`: Executa uma operação de GET.
  - `PostAsync(string uri, string content)`: Executa uma operação de POST.
  - `PutAsync(string uri, string parameter, string content)`: Executa uma operação de PUT.
  - `DeleteAsync(string uri, string codigo)`: Executa uma operação de DELETE.

### 3. Models
Contém classes que representam modelos de comunicação com a API.

- **ApiClient.cs**: Implementa a interface `IApiClient` e gerencia a lógica de comunicação HTTP com a API, incluindo serialização de respostas e tratamento de exceções. <br>
<br> Principais métodos implementados:
  - `GetAsync()`, `PostAsync()`, `PutAsync()`, `DeleteAsync()`: Para operações CRUD.
  - `FillResultResponse(HttpResponseMessage response)`: Método auxiliar para preencher respostas a partir de respostas HTTP.

### 4. Services
Implementações concretas dos serviços que gerenciam a comunicação e o tratamento de respostas da API.

- **CommunicationService.cs**: Implementa a interface `ICommunicationService`. Esta classe é responsável por gerenciar a lista de respostas de uma API, incluindo métodos para adicionar novas respostas e obter a lista completa de respostas. <br>
<br> Métodos principais:
  - `AddResponseContent(ResultResponseDTO resultResponse)`: Adiciona uma única resposta à lista.
  - `AddResponseContent(List<ResultResponseDTO> resultResponses)`: Adiciona uma lista de respostas.
  - `GetResultResponses()`: Retorna todas as respostas armazenadas.

## Resumo

A arquitetura do projeto **Communication** está organizada em termos de separação de responsabilidades. As interfaces definem os contratos para os serviços de comunicação e operações da API, enquanto as implementações nas pastas `Services` e `Models` lidam com a lógica de negócios concreta. A utilização de métodos de extensão em `Extensions` oferece funcionalidade adicional para o gerenciamento de respostas de API.

Essa estrutura modular facilita a manutenção e a expansão do código, garantindo que novas funcionalidades possam ser adicionadas sem impactar significativamente o sistema existente.
