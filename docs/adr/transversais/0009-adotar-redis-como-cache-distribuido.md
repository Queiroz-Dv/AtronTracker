# ADR 0009: Adotar Redis como cache distribuído da plataforma

- Status: Aceito
- Data: 2026-08-05
- Alcance: transversal

## Contexto

O Atron possuía `ICacheService`, chaves tipadas por `ChaveCache` e providers
locais em memória e arquivo JSON. O cache já era usado em autorização, dados da
sessão, primeiro acesso e recuperação de senha.

`IMemoryCache` pertence ao processo da WebApi. Reinícios descartam as entradas
e duas instâncias mantêm conteúdos independentes. `JsonFileCacheService`
sobrevive ao processo local, mas não é apropriado para compartilhamento entre
instâncias ou para o sistema de arquivos efêmero de containers publicados.

O Atron permanece um monólito modular. A necessidade não é extrair um
microsserviço, mas exercitar e suportar uma dependência distribuída real sem
acoplar os consumidores à tecnologia concreta.

## Decisão

Adotar Redis compatível como terceiro provider de `ICacheService`:

- `Memory` permanece o padrão versionado e a opção de menor custo local;
- `JsonFile` permanece disponível para experimentos locais;
- `Redis` é habilitado explicitamente por ambiente;
- desenvolvimento executa Redis em container Docker;
- produção usa Render Key Value pela rede interna;
- chaves continuam sendo montadas exclusivamente por `ChaveCache`;
- cada ambiente usa prefixo próprio, como `atron:dev:` e `atron:prod:`;
- conexão e credenciais ficam em User Secrets ou variáveis protegidas.

O Redis executa fora do processo e fora do container da WebApi. O Dockerfile da
aplicação não será alterado para instalar ou iniciar Redis.

## Motivadores

1. Compartilhar cache entre instâncias da WebApi.
2. Manter o cache independente do reinício do processo da aplicação.
3. Preservar os consumidores existentes atrás de `ICacheService`.
4. Praticar configuração por ambiente, TTL, serialização, invalidação e
   operação de uma dependência de rede.
5. Aproximar a estrutura operacional do Atron de aplicações publicadas
   modernas sem usar distribuição como objetivo vazio.

“Tecnologia comum no mercado” não é justificativa suficiente isoladamente. A
decisão se apoia em fluxos existentes e em uma fronteira de cache já criada.

## Consequências positivas

- reiniciar a WebApi não remove automaticamente as chaves;
- instâncias podem observar o mesmo cache;
- TTL passa a ser administrado pelo servidor de cache;
- invalidações deixam de depender da memória de um processo específico;
- o provider pode ser trocado por configuração, sem alterar casos de uso.

## Custos e riscos

- nova dependência externa, com custo, latência e modo de falha próprios;
- valores precisam ser serializados e versionados de forma compatível;
- indisponibilidade pode afetar autorização e autenticação se não houver
  política de fallback;
- remoção malsucedida de cache de acesso pode preservar permissão antiga até o
  TTL;
- planos sem persistência podem perder dados temporários;
- monitoramento de processo não comprova prontidão do Redis.

## Limites da decisão

Redis não será usado como:

- banco oficial de usuários, perfis, tarefas, estoque ou notificações;
- justificativa para transformar o Atron em microsserviços;
- barramento Pub/Sub ou Streams sem consumidor e requisito comprovados;
- mecanismo de lock, fila ou rate limiting nesta fase.

## Alternativas consideradas

### Permanecer somente com `IMemoryCache`

Menor custo operacional, mas não compartilha entradas nem sobrevive ao processo.

### Usar arquivos JSON na publicação

Mantém dados localmente, porém depende do sistema de arquivos do container, não
compartilha estado e exige sincronização inadequada entre instâncias.

### Colocar Redis dentro do container da WebApi

Rejeitado porque mistura dois processos e ciclos de vida, impede operação
independente e diverge do serviço gerenciado usado no Render.

### Migrar diretamente para microsserviços

Rejeitado por ausência de requisito de deploy, escala, dados e autonomia de
equipe que justifique a distribuição dos módulos.

## Implementação

- `RedisCacheService` adapta `IDistributedCache` ao contrato do Atron;
- `AddAtronCache` registra somente o provider selecionado;
- `CacheProviderInfoService` identifica Redis como distribuído;
- `Microsoft.Extensions.Caching.StackExchangeRedis` fornece o cliente;
- `compose.redis.yaml` reproduz a dependência local;
- `docs/cache-redis.md` descreve instalação, configuração e operação.

## Validação exigida

- testes unitários de serialização, TTL, leitura e remoção;
- testes da seleção de provider e da ausência de conexão;
- build e testes da solução;
- Docker Compose saudável e `PING` com `PONG`;
- cache miss, cache hit, expiração e invalidação local;
- deploy no Render com login, sessão, métricas e reinício da WebApi;
- ausência de credenciais e conexões em código, logs e arquivos versionados.

Validação local não comprova o comportamento publicado. O aceite operacional no
Render depende dos testes realizados depois do deploy.
