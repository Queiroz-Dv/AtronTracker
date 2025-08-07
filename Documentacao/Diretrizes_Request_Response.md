# Diretrizes para o Uso de Modelos de Contrato (Request/Response) no Projeto Atron

**Versão:** 1.0

**Data:** 06 de agosto de 2025

**Autor:** Arquitetura Atron
## 1. Objetivo
Este documento estabelece as diretrizes para o uso de classes `Request` e `Response` na camada de API do projeto Atron. O objetivo é equilibrar a segurança, clareza e robustez da aplicação com a produtividade do desenvolvimento, garantindo um design de API consistente e de longo prazo.

---
## 2. Diretriz Principal: Separação de Contratos  

O projeto Atron adota o princípio fundamental de que o **contrato público da API** (o que o cliente envia e recebe) deve ser desacoplado dos **modelos de dados internos** (DTOs da camada de aplicação e Entidades de Domínio).

As classes `Request` e `Response` são a materialização deste contrato e atuam como uma camada de proteção e adaptação entre o mundo externo e a lógica de negócio interna.

---
## 3. Regras de Uso

### 3.1. Use `Request` Models para TODAS as Operações de Escrita (`POST`, `PUT`, `PATCH`)


-   **Finalidade:** Segurança, Validação e Clareza de Intenção.

-   **Descrição:** Todo endpoint que cria ou modifica dados no sistema **deve** aceitar um objeto `Request` específico para aquela operação (ex: `CreateUsuarioRequest`, `UpdateTarefaRequest`, `ChangePasswordRequest`).

-   **Justificativa Técnica:**

    -   **Segurança (Prevenção de Mass Assignment):** Garante que um cliente não possa enviar dados para campos que ele não deveria ter permissão para alterar (ex: um usuário tentando definir a propriedade `IsAdmin` para `true`). O `Request` model expõe apenas os campos estritamente necessários para a operação.

    -   **Validação Específica do Caso de Uso:** Permite a aplicação de regras de validação (`Data Annotations` ou `FluentValidation`) que são relevantes apenas para aquele contexto. Por exemplo, a propriedade `Senha` é obrigatória no `CreateUsuarioRequest`, mas pode não existir no `UpdateUsuarioRequest`.

    -   **Clareza de Intenção:** O nome da classe documenta explicitamente a operação que está sendo realizada, melhorando a legibilidade do `Controller`.
---
### 3.2. Use `Response` Models para a Maioria das Operações de Leitura (`GET`)

-   **Finalidade:** Formatação de Dados e Proteção de Informações Internas.

-   **Descrição:** Todo endpoint que retorna dados para um cliente público (como o front-end Angular) **deve** mapear os DTOs internos para um objeto `Response` específico (ex: `UsuarioResponse`, `TarefaDetalhadaResponse`).

-   **Justificativa Técnica:**

    -   **Ocultação de Dados (Information Hiding):** Impede o vazamento de dados internos ou sensíveis que não são relevantes para o cliente (ex: chaves estrangeiras, hashes de senha, estruturas de dados internas, etc.).

    -   **Formato Otimizado para o Cliente:** Permite "achatar" ou remodelar a estrutura dos dados para que ela seja consumida da forma mais fácil e performática possível pelo cliente. Um `Response` pode combinar campos de várias entidades, calcular valores derivados ou simplificar hierarquias complexas, reduzindo a carga de trabalho no front-end.

  
---
### 3.3. (OPC) Permita o Uso Direto de `DTOs` em Endpoints de Leitura Simples

-   **Finalidade:** Pragmatismo e Redução de "Boilerplate".

-   **Descrição:** Em cenários de CRUD simples, onde um endpoint `GET` retorna uma lista ou um objeto e o `DTO` interno já está no formato exato e seguro para ser exposto, **é aceitável** retorná-lo diretamente, sem a criação de um `Response` model adicional.

-   **Exemplo Ideal:** Um endpoint administrativo que lista todos os `Departamentos`. Se o `DepartamentoDTO` contém apenas `Codigo` e `Descricao`, ele é seguro e ideal para o consumo direto.

    ```csharp

    [HttpGet]

    public async Task<ActionResult<IEnumerable<DepartamentoDTO>>> Get()

    {

        // Neste caso simples, onde o DTO é seguro e já está no formato

        // desejado, retorná-lo diretamente é uma exceção aceitável.

        return Ok(await _departamentoService.ObterTodosAsync());

    }

    ```

-   **Critério de Decisão:** Antes de aplicar esta exceção, responda: "Este DTO contém alguma informação que não deveria ser pública? O formato dele é exatamente o que o cliente precisa, sem exigir manipulação adicional no front-end?". Se a resposta para ambas for "sim", o uso direto é justificável. Caso contrário, a regra padrão (`Response` model) deve ser aplicada.

  
---
## 4. Implementação do Mapeamento

A abordagem de usar métodos de extensão, como no arquivo `BuilderExtensions.cs`, para realizar os mapeamentos `Request -> DTO` e `DTO -> Response` é uma estratégia leve e eficaz. Ela mantém a lógica de conversão centralizada e desacoplada, alinhando-se bem com os objetivos deste padrão.