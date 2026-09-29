# Guia de Caching no Atron

## Objetivo

Este guia descreve como executar, configurar, validar e operar o cache Redis
compatível do Atron nos ambientes local e publicado.

A adoção não transforma o Atron em microsserviço. O produto continua sendo um
monólito modular publicado pelo `AtronPlatform.WebApi`. Redis é uma dependência
externa e transversal usada por meio do contrato `ICacheService`.

A decisão arquitetural e seus trade-offs estão registrados no
[ADR 0009](../adr/transversais/0009-adotar-redis-como-cache-distribuido.md).

## Por que o Atron usa Redis?

O Atron já possuía três características que justificavam um provider
distribuído:

1. dados de acesso e autorização eram armazenados em cache;
2. recuperação de senha e primeiro acesso usavam dados temporários com TTL;
3. os consumidores já dependiam de `ICacheService`, sem conhecer a tecnologia
   concreta.

`IMemoryCache` atende uma única execução, mas cada instância da WebApi mantém
seu próprio conteúdo e perde tudo quando o processo reinicia. Redis mantém o
cache fora do processo e permite que mais de uma instância consulte as mesmas
chaves.

O objetivo de formação técnica faz parte do Atron, mas não é a única
justificativa. A mudança exercita uma necessidade real de aplicações
publicadas: configuração por ambiente, serialização, TTL, invalidação,
dependência de rede e compartilhamento de estado efêmero.

## Fluxo implementado

```mermaid
flowchart TD
    %% Clientes da Aplicação
    subgraph Client ["Cliente"]
        HTTP["Angular / Cliente HTTP"]
    end

    %% Camada WebApi e Injeção de Dependência
    subgraph WebApi ["AtronPlatform.WebApi"]
        API["WebApi (ASP.NET Core)"]
        DI["AddAtronCache\n(Injeção de Dependência)"]
        APP_SETTINGS["appsettings / Env Vars / UserSecrets\nCache:Provider"]
    end

    %% Contrato e Abstração de Cache
    subgraph Core ["Contrato Transversal"]
        CONTRACT["ICacheService"]
        KEY_BUILDER["ChaveCache\n(Ex: acesso:QRZ)"]
    end

    %% Implementações dos Providers
    subgraph Providers ["Providers de Cache"]
        MEM["CacheService\n(MemoryCache)"]
        JSON["JsonFileCacheService\n(Arquivo JSON)"]
        REDIS_SERVICE["RedisCacheService\n(IDistributedCache / StackExchange.Redis)"]
    end

    %% Ambiência e Destinos do Redis
    subgraph External ["Infraestrutura Externa de Cache"]
        subgraph DevEnv ["Ambiente Local (Desenvolvimento)"]
            DOCKER["Docker Compose\n(atron-redis:6379)\nPrefix: atron:dev:"]
        end
        
        subgraph ProdEnv ["Ambiente Publicado (Produção)"]
            RENDER["Render Key Value\n(red-exemplo:6379)\nPrefix: atron:prod:"]
        end
    end

    %% Conexões do Fluxo
    HTTP -->|"1. Requisição HTTP"| API
    API -->|"2. Resolve serviço via DI"| CONTRACT
    APP_SETTINGS -.->|"Lê configuração"| DI
    DI -.->|"Registra provider ativo"| CONTRACT

    CONTRACT --- KEY_BUILDER
    CONTRACT ==>|"Provider: Memory"| MEM
    CONTRACT ==>|"Provider: JsonFile"| JSON
    CONTRACT ==>|"Provider: Redis"| REDIS_SERVICE

    REDIS_SERVICE -->|"Conecta via localhost:6379"| DOCKER
    REDIS_SERVICE -->|"Conecta via URL Interna"| RENDER

    %% Estilização Visual
    classDef client fill:#3b82f6,stroke:#1d4ed8,color:#fff,font-weight:bold;
    classDef webapi fill:#f8fafc,stroke:#94a3b8,color:#0f172a;
    classDef core fill:#0284c7,stroke:#0369a1,color:#fff,font-weight:bold;
    classDef provider fill:#f1f5f9,stroke:#64748b,color:#0f172a;
    classDef redisService fill:#8b5cf6,stroke:#6d28d9,color:#fff,font-weight:bold;
    classDef env fill:#dc2626,stroke:#991b1b,color:#fff,font-weight:bold;

    class HTTP client;
    class API,DI,APP_SETTINGS webapi;
    class CONTRACT,KEY_BUILDER core;
    class MEM,JSON provider;
    class REDIS_SERVICE redisService;
    class DOCKER,RENDER env;
```

Responsabilidades:

| Componente | Responsabilidade |
|---|---|
| `ICacheService` | Contrato usado pelos casos de uso e pela autorização. |
| `ChaveCache` | Montagem centralizada de chaves como `acesso:QRZ`. |
| `CacheService` | Provider local em memória. |
| `JsonFileCacheService` | Provider local em arquivo JSON. |
| `RedisCacheService` | Serialização e acesso ao `IDistributedCache`. |
| `AddAtronCache` | Seleção de um único provider pela configuração. |

O prefixo configurado por ambiente é acrescentado pelo provider Redis. Uma
chave lógica `acesso:QRZ` aparece localmente como
`atron:dev:acesso:QRZ` e em produção como `atron:prod:acesso:QRZ`.

## Configuração base

O `appsettings.json` versionado usa `Memory` como padrão seguro. Dessa forma, a
aplicação não exige uma conexão externa apenas para iniciar:

```json
{
  "Cache": {
    "Provider": "Memory",
    "JsonFile": {
      "Diretorio": "App_Data/cache-json"
    },
    "Redis": {
      "InstanceName": "atron:"
    }
  }
}
```

Ambientes que adotam Redis sobrescrevem o provider e a conexão. Strings de
conexão e credenciais não são versionadas.

## Execução local com Docker Compose

### Pré-requisitos

- Docker Desktop iniciado;
- containers Linux habilitados;
- porta local `6379` disponível.

O arquivo `compose.redis.yaml` executa somente a dependência Redis. Ele não
executa a WebApi nem representa a topologia do Render.

Se o container `atron-redis` foi criado anteriormente com `docker run`, pare-o
antes de usar o Compose para liberar a porta:

```powershell
docker stop atron-redis
```

Suba o Redis a partir da raiz do repositório:

```powershell
docker compose -f compose.redis.yaml up -d
```

Valide o estado e o health check:

```powershell
docker compose -f compose.redis.yaml ps
docker compose -f compose.redis.yaml exec redis redis-cli PING
```

Resultado esperado:

```text
PONG
```

O Compose publica a porta somente em `127.0.0.1`, não cria volume e desabilita
persistência. Essa configuração é intencional: o conteúdo é cache e deve poder
ser reconstruído pela aplicação.

Para interromper:

```powershell
docker compose -f compose.redis.yaml stop
```

Para remover o container e a rede criados pelo Compose:

```powershell
docker compose -f compose.redis.yaml down
```

## Configuração local da WebApi

O projeto `AtronPlatform.WebApi` já possui `UserSecretsId`. 
O uso de user secrets é opcional mas o uso facilita no dia a dia e evita subir dados nos `appsettings.json`.

Coom cofigurar:

```powershell
dotnet user-secrets set "Cache:Provider" "Redis" --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
dotnet user-secrets set "Cache:Redis:ConnectionString" "localhost:6379,abortConnect=false" --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
dotnet user-secrets set "Cache:Redis:InstanceName" "atron:dev:" --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
```

Confira as chaves configuradas sem copiar valores para documentação ou logs:

```powershell
dotnet user-secrets list --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
```

Quando a WebApi é executada diretamente pelo Visual Studio ou por `dotnet run`,
`localhost:6379` aponta para a porta publicada pelo container Redis.

Se a WebApi também for executada dentro de um container Docker separado,
`localhost` passa a representar o próprio container da WebApi. No Docker
Desktop, use `host.docker.internal:6379` para alcançar a porta publicada no
host, ou coloque os dois serviços na mesma rede Compose e use `redis:6379`.

## Validação funcional local

1. inicie Redis;
2. inicie a WebApi;
3. autentique um usuário de teste;
4. chame `GET /api/Sessao/Info`;
5. liste as chaves:

```powershell
docker compose -f compose.redis.yaml exec redis redis-cli --scan --pattern "atron:dev:*"
```

Consulte o TTL de uma chave encontrada:

```powershell
docker compose -f compose.redis.yaml exec redis redis-cli TTL "atron:dev:acesso:QRZ"
```

Interpretação:

- valor positivo: chave existente com expiração em segundos;
- `-1`: chave existente sem expiração, resultado inesperado para esses fluxos;
- `-2`: chave inexistente.

Valide também:

- cache miss seguido de gravação;
- cache hit na segunda leitura;
- chave disponível após reiniciar somente a WebApi;
- invalidação após alterar o perfil de acesso;
- expiração e reconstrução da chave;
- remoção de dados temporários depois do uso.

## Configuração no Render

### Criar o serviço

No Dashboard do Render:

1. selecione `New > Key Value`;
2. escolha o mesmo workspace e a mesma região da WebApi;
3. selecione `allkeys-lru` para o uso como cache;
4. mantenha a persistência desligada quando o conteúdo for descartável;
5. crie o serviço e copie a `Internal Key Value URL`.

Uma URL interna sem autenticação tem formato semelhante a:

```text
redis://red-exemplo:6379
```

A configuração atual do StackExchange.Redis usa o formato tokenizado. Remova o
prefixo `redis://` e acrescente a opção de reconexão:

```text
red-exemplo:6379,abortConnect=false
```

Se a autenticação interna for habilitada futuramente, usuário e senha deverão
ser configurados de acordo com o formato do StackExchange.Redis. Não registre
esses valores em documentação ou no Git.

### Variáveis da WebApi

Em `AtronPlatform.WebApi > Environment`, configure:

```text
Cache__Provider=Redis
Cache__Redis__ConnectionString=red-exemplo:6379,abortConnect=false
Cache__Redis__InstanceName=atron:prod:
```

Escolha `Save and deploy`. User Secrets locais não são publicados no Render.

### Por que o Dockerfile não muda?

O Dockerfile da raiz continua publicando e executando apenas
`AtronPlatform.WebApi.dll`. Ele já restaura o pacote NuGet adicionado ao
`Shared.csproj` durante `dotnet restore`.

Redis não deve ser instalado nessa imagem porque:

- WebApi e cache possuem processos e ciclos de vida diferentes;
- o Render fornece o Key Value como serviço separado;
- atualizações e reinícios do host não devem administrar o processo Redis;
- colocar os dois no mesmo container impediria escala e operação independentes.

O Render conecta os serviços pela URL interna e pelas variáveis de ambiente.

## Validação no Render

Depois do deploy:

1. confirme que o deploy não apresenta erro de configuração ou DI;
2. confirme `GET /api/saude` com status `200`;
3. faça login e chame `GET /api/Sessao/Info`;
4. observe conexões, memória e comandos nas métricas do Key Value;
5. confirme o prefixo `atron:prod:`;
6. reinicie somente a WebApi e repita a consulta;
7. altere um perfil de teste e confirme a invalidação da chave;
8. verifique que logs não exibem a conexão.

`/api/saude` prova hoje a vida do processo, mas não é um health check específico
de Redis. Um readiness check do cache permanece uma evolução separada.

## Limites e política de falha atuais

- Redis não é fonte de verdade para usuários, perfis, tarefas ou estoque.
- Perder chaves de acesso deve causar reconstrução a partir do banco.
- Perder dados temporários pode invalidar links de recuperação ou primeiro
  acesso; o usuário precisará solicitar um novo link.
- A implementação atual propaga indisponibilidade e timeout do Redis. Uma
  política explícita de fallback e observabilidade ainda precisa ser definida
  antes de declarar alta disponibilidade.
- Invalidação de autorização é sensível: ignorar silenciosamente uma falha de
  remoção pode manter permissões antigas até o TTL expirar.
- Pub/Sub, Streams, filas, locks distribuídos e rate limiting não fazem parte
  desta decisão.

## Rollback do provider

Para voltar ao cache em memória sem alterar código, configure:

```text
Cache__Provider=Memory
```

Faça novo deploy e valide login, sessão e autorização. As chaves existentes no
Key Value podem expirar naturalmente. O rollback troca o provider, mas não
remove a dependência NuGet nem a implementação Redis.

## Referências

- Render Key Value: <https://render.com/docs/key-value>
- Variáveis de ambiente no Render: <https://render.com/docs/configure-environment-variables>
- StackExchange.Redis, configuração: <https://stackexchange.github.io/StackExchange.Redis/Configuration.html>
- ASP.NET Core, cache distribuído: <https://learn.microsoft.com/aspnet/core/performance/caching/distributed>
