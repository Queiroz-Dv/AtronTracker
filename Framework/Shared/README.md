## Projeto Shared

A estrutura do projeto segue um padrão de camadas, com o objetivo de promover a organização do código e a separação de responsabilidades.

A camada Shared contém elementos comuns a diversas partes do sistema.

## Estrutura de Pastas:

 * **`DTO`:** Descrição dos Data Transfer Objects (DTOs) utilizados para transportar dados entre as camadas.

<br>
            
* **`Enums`:** Enumeradores utilizados de forma global
  
<br>

* **`Extensions`:** Classes de extensão para reutilização de código e centralização de funcionalidades

 <br>

* **`Interfaces`:** Defini os contratos que serão utilizados de forma abstraída

<br>

* **`Models`:** Modelos de objeto que serão utilizados nos processos internos

<br>

* **`Services`:** Serviços que realizam as operações básicas do sistema como filtragem, mapeamento de objetos, notificações entre outros.