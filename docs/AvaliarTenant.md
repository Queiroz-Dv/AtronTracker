# Solicitação de Análise e Planejamento Arquitetural: Isolação Multi-tenant via Interceptors e Global Query Filters

## 1. Contexto e Problema Atual

Atualmente, a aplicação utiliza uma abordagem de isolamento de *tenant* (*workspace*) baseada em tabelas de vínculo específicas para cada entidade (ex.: `DepartamentoWorkspace`, `CargoWorkspace`). 

Analisamos essa estrutura e identificamos os seguintes gargalos:
* **Crescimento Exponencial:** Para cada nova entidade/módulo, é necessário criar uma nova entidade de vínculo, novos mapeamentos e novas rotinas de gravação/consulta no repositório.
* **Acoplamento nos Use Cases:** As regras de orquestração de negócios (*Use Cases*) estão acumulando responsabilidades de infraestrutura (obtenção de contexto e persistência manual de vínculos de *tenant*)[cite: 1].

---

## 2. Visão Geral da Abordagem Proposta

Queremos evoluir a arquitetura para um modelo em que a gestão de *tenant* ocorra de forma **transparente, centralizada e desacoplada** da camada de aplicação e dos *Use Cases*.

A proposta fundamenta-se nos seguintes pilares:

### A. Tabela Única de Vínculo Polimórfica (`RecursoWorkspace`)
Em substituição às tabelas individuais (`XWorkspace`), utilizaremos uma única estrutura para registrar o pertencimento de recursos a um *workspace*:

* **Tabela/Entidade:** `RecursoWorkspace`
* **Campos principais:**
  * `WorkspaceId` / `WorkspaceCodigo`
  * `ModuloCodigo` (utilizando os prefixos/constantes padronizados de `ModuloPolicies`, ex.: `"Modulo:DPT"`, `"Modulo:CRG"`)
  * `RecursoId` / `RecursoCodigo`

### B. Interface de Domínio (`IModuloResource`)
As entidades de domínio que requerem isolamento por *tenant* implementarão um contrato simples expondo seu `Id`, `Codigo` e o `ModuloCodigo` correspondente. As entidades não conterão FKs diretas de *workspace*, mantendo o domínio livre de ruídos de infraestrutura.

### C. Persistência Transparente via Interceptor (`SaveChangesInterceptor`)
A gravação do vínculo na tabela `RecursoWorkspace` deixará de ser chamada explicitamente nos *Use Cases*. 
* Um `SaveChangesInterceptor` no EF Core detectará instâncias cadastradas que implementam `IModuloResource`.
* O interceptor resolverá o *workspace* ativo via `WorkspaceResolver` e persistirá o registro em `RecursoWorkspace` no mesmo ciclo de gravação, sem interferência direta da camada de aplicação[cite: 1].

### D. Leitura Transparente via Filtros Globais (`Global Query Filters`)
Para evitar vazamento de dados entre *tenants* nas consultas:
* Registraremos *Global Query Filters* no `DbContext` (aplicados dinamicamente via Reflection para todas as entidades que implementam `IModuloResource`)[cite: 1].
* Todas as buscas via LINQ/EF Core aplicarão automaticamente o filtro exigindo existência do vínculo na `RecursoWorkspace` para o *workspace* ativo resolvido pelo `WorkspaceResolver`[cite: 1].

---

## 3. Objetivo desta Análise (Instruções para o Agente)

**ATENÇÃO:** Não gere código de implementação neste momento. O objetivo desta etapa é estritamente **avaliar, criticar e planejar**.

Responda aos seguintes pontos:

1. **Análise de Viabilidade e Riscos:**
   * Quais são os potenciais pontos de atenção ao utilizar `SaveChangesInterceptor` para persistência secundária automática? Há risco de *deadlocks*, problemas de transação ou concorrência?
   * Quais são os impactos de performance ao utilizar subconsultas (`.Any()`) em *Global Query Filters* dinâmicos em tabelas de grande volume?

2. **Avaliando Alternativas Técnicas:**
   * Comparando a abordagem polimórfica (`RecursoWorkspace`) com a abordagem de **Shadow Properties** nativas do EF Core (`EF.Property<int>(e, "WorkspaceId")`), quais são os prós e contras de cada uma em relação a desempenho, simplicidade e pureza do domínio?

3. **Plano de Migração e Refatoração:**
   * Qual seria a estratégia ideal para migrar o código existente (`CriarDepartamentoCase`, repositórios e DTOs) sem quebrar as funcionalidades ativas?
   * Como estruturar o ciclo de vida do `WorkspaceResolver` dentro do `DbContext` para evitar vazamento de memória ou inconsistência de escopo (*Scoped Services*)[cite: 1]?

4. **Tratamento de Exceções e Casos Limite (*Edge Cases*):**
   * Como a arquitetura deve se comportar quando uma operação de leitura/escrita for executada por uma rotina de *background* (onde não existe contexto HTTP/header para o `WorkspaceResolver`)[cite: 1]?
   * Como contornar o filtro de *tenant* em consultas administrativas onde o usuário precisa de visão global (ex.: uso de `.IgnoreQueryFilters()`)?

---

*Aguardando análise detalhada e plano de ação estruturado antes de prosseguir para a fase de codificação.*