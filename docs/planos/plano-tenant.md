# Solicitação de Análise e Planejamento Arquitetural: Isolação Multi-tenant via Interceptors e Global Query Filters

## 1. Contexto e Problema Atual

Atualmente, a aplicação utilizava uma abordagem de isolamento de *tenant* (*workspace*) baseada em tabelas de vínculo específicas para cada entidade (ex.: `DepartamentoWorkspace`, `CargoWorkspace`). 

Analisamos essa estrutura e identificamos os seguintes gargalos:
* **Crescimento Exponencial:** Para cada nova entidade/módulo, é necessário criar uma nova entidade de vínculo, novos mapeamentos e novas rotinas de gravação/consulta no repositório.
* **Acoplamento nos Use Cases:** As regras de orquestração de negócios (*Use Cases*) estão acumulando responsabilidades de infraestrutura (obtenção de contexto e persistência manual de vínculos de *tenant*)

---

## 2. Visão Geral e Decisões (Para desenvolvedores)

Queremos evoluir a arquitetura para um modelo em que a gestão de *tenant* ocorra de forma **transparente, centralizada e desacoplada** da camada de aplicação e dos *Use Cases*.

A proposta fundamenta-se nos seguintes pilares:

### A. Tabela Única de Vínculo Polimórfica (`RecursoWorkspace`)
Em substituição às tabelas individuais (`XWorkspace`), inicialmente decidi manter uma única estrutura para registrar o pertencimento de recursos a um *workspace*:

* **Tabela/Entidade:** `RecursoWorkspace`
* **Campos principais:**
  * `WorkspaceId` / `WorkspaceCodigo`
  * `ModuloCodigo` (utilizando os prefixos/constantes padronizados de constantes em cada pasta de domínio, ex.: `"DPT"`, `"CRG"`)
  * `RecursoId` / `RecursoCodigo`

Veja o exemplo de [Cargo](../../AtronPlatform/Modules/Tracker/Domain/Entities/Cargo.cs)

### B. Interface de Domínio (`ITenantScoped`)
As entidades de domínio que requerem isolamento por *tenant* implementarão um contrato simples expondo seu `Id`, `Codigo` e o `ModuloCodigo` correspondente. As entidades não conterão FKs diretas de *workspace*, mantendo o domínio livre de ruídos de infraestrutura.

A interface dedicada é a [ITenantScoped](../../Framework/Shared/Domain/Entities/Identity/ITenantScoped.cs)

Ela foi repassada para o projeto `Shared` para evitar chamadas e replicar código em outros domínios. Isso quer dizer que essa interface é um cross-cutting para todas as outras entidades que dependerão dela.

### C. Persistência Transparente via Interceptor (`SaveChangesInterceptor`)
A gravação do vínculo na tabela `RecursoWorkspace` deixará de ser chamada explicitamente nos *Use Cases*. 
* Um `SaveChangesInterceptor` no EF Core detectará instâncias cadastradas que implementam `ITenantScoped`.
* O interceptor resolverá o *workspace* ativo via [WorkspaceService](../../AtronPlatform/Modules/Tracker/Application/Services/EntitiesServices/WorkspaceService.cs) e persistirá o registro em `DbContext` no mesmo ciclo de gravação, sem interferência direta da camada de aplicação.

Cada módulo hoje registra o fluxo de para workspace em seus `DbContexts`, em um próximo update será remodelado para que o sistema centralize essa operação em um único lugar.

### D. Leitura Transparente via Filtros Globais (`Global Query Filters`)
Para evitar vazamento de dados entre *tenants* nas consultas:
* Registraremos *Global Query Filters* no `DbContext` (aplicados dinamicamente via Reflection para todas as entidades que implementam `ITenantScoped`)
* Todas as buscas via LINQ/EF Core aplicarão automaticamente o filtro exigindo existência do vínculo na `RecursoWorkspace` para o *workspace* ativo resolvido pelo `WorkspaceService`.

---

# Representação do processo (Departamento)

``` mermaid
flowchart TD
    %% Definição dos Clientes e Camadas da Aplicação
    subgraph Client ["Cliente HTTP"]
        REQ["Requisição HTTP\n(Bearer Token + Claim Workspace)"]
    end

    subgraph Presentation ["1. Camada de Apresentação"]
        CTRL["DepartamentoController\n[Authorize]"]
    end

    subgraph Application ["2. Camada de Aplicação"]
        SRV["DepartamentoService"]
        UC["Use Cases\n(Criar / Obter / Atualizar)"]
    end

    subgraph Infrastructure ["3. Camada de Infraestrutura"]
        REPO["DepartamentoRepository"]
        
        subgraph Persistence ["EF Core (AtronDbContext)"]
            DBCXT["AtronDbContext"]
            GQF["Global Query Filter\n(Filtra RecursoWorkspace)"]
            INTERCEPTOR["SaveChangesAsync()\n(Intercepção Multi-tenant)"]
            REC_TAB["DbSet<RecursoWorkspace>"]
        end
    end

    subgraph Database ["4. Banco de Dados (PostgreSQL)"]
        DB_DEPT[("Tabela Departamentos")]
        DB_REC[("Tabela RecursoWorkspace")]
    end

    %% Fluxo de Requisição
    REQ -->|"HTTP POST / GET"| CTRL
    CTRL -->|"Chama Serviço"| SRV
    SRV -->|"Orquestra Regra de Negócio"| UC
    UC -->|"Chama Persistência/Consulta"| REPO

    %% Fluxo de Leitura (GET)
    REPO -.->|"1. Consulta LINQ"| DBCXT
    DBCXT -.->|"2. Aplica Filtro de Tenant"| GQF
    GQF -.->|"3. INNER JOIN com RecursoWorkspace"| DB_DEPT

    %% Fluxo de Escrita (POST/PUT)
    REPO ==>|"1. AddAsync / Update"| DBCXT
    DBCXT ==>|"2. Dispara"| INTERCEPTOR
    INTERCEPTOR ==>|"3. Salva Entidade Principal"| DB_DEPT
    INTERCEPTOR ==>|"4. Captura ID e Insere RecursoWorkspace"| REC_TAB
    REC_TAB ==>|"5. Salva Vínculo de Tenant"| DB_REC

    %% Estilização Visual
    classDef client fill:#3b82f6,stroke:#1d4ed8,color:#fff;
    classDef layer fill:#f8fafc,stroke:#cbd5e1,color:#0f172a,stroke-width:1px;
    classDef component fill:#e2e8f0,stroke:#64748b,color:#0f172a;
    classDef efcore fill:#8b5cf6,stroke:#6d28d9,color:#fff;
    classDef database fill:#059669,stroke:#047857,color:#fff;

    class REQ client;
    class CTRL,SRV,UC,REPO component;
    class DBCXT,GQF,INTERCEPTOR,REC_TAB efcore;
    class DB_DEPT,DB_REC database;
    
    linkStyle 4,5,6 stroke:#3b82f6,stroke-width:2px,stroke-dasharray: 5 5;
    linkStyle 7,8,9,10,11 stroke:#10b981,stroke-width:2px;
```
