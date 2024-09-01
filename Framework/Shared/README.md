## Projeto Shared

A estrutura do projeto segue um padrão de camadas, com o objetivo de promover a organização do código e a separação de responsabilidades.

A camada Shared contém elementos comuns a diversas partes do sistema.

 **`DTO`:**
  * Descrição dos Data Transfer Objects (DTOs) utilizados para transportar dados entre as camadas.
    * **PageInfoDTO:** Contém informações sobre a paginação.
    * **ResultResponseDTO:** Representa uma resposta genérica do sistema.
        * **`API`:**
            * **AppSettingsConfigShared.cs:** Configurações da aplicação compartilhadas.
            * **RotaDeAcesso.cs:** Definição das rotas de acesso.
            * **RotasFixasConfig.cs:** Configurações de rotas fixas.
<br>
            
**`Enums`:**
  * **ResultResponseLevelEnum:** Define os níveis de resposta (sucesso, falha, etc.).
  
<br>

**`Extensions`:**
  * **EnumExtensions.cs:** Extensões para a classe Enum.
  * **HttpResponseMessageExtensions.cs:** Extensões para a classe HttpResponseMessage.

 <br>

**`Interfaces`:**
  * **IPaginationService:** Define o contrato para o serviço de paginação.
  * **IResultResponseService:** Define o contrato para o serviço de resposta.

<br>

**`Models`:**
  * **ResultResponseModel:** Representa o modelo de dados para a resposta do sistema.

<br>

**`Services`:**
  * **PaginationService:** Implementação do serviço de paginação.
  * **ResultResponseService:** Implementação do serviço de resposta.