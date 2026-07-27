# Publicação no Render, Supabase e Brevo

## Objetivo

Este manual descreve a publicação do núcleo do Atron com:

- Render para hospedar a API;
- Supabase/PostgreSQL como banco de dados;
- Brevo como provedor de e-mail transacional.

O produto é publicado pelo host neutro `AtronPlatform.WebApi`. Tracker, Stock,
Auditoria e Notificações Internas executam no mesmo processo, conforme os ADRs
0007 e 0008.

## Artefato publicado

O Web Service usa o `Dockerfile` da raiz. O build publica:

```text
AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
```

O container executa:

```text
AtronPlatform.WebApi.dll
```

na porta fornecida pelo Render. Os antigos hosts executáveis de Tracker,
Auditoria e Stock foram removidos nas Fases 8 e 9 do ADR 0007. O rollback
permanece disponível pelo último commit estável anterior à remoção.

## Configuração segura

Nenhuma credencial deve existir em `appsettings*.json`, arquivos `.env`, código
fonte ou documentação versionada. O host aceita configuração do .NET por
variáveis de ambiente.

Variáveis mínimas do núcleo:

```text
ASPNETCORE_ENVIRONMENT=Production
ATRON_CONNECTION_STRING=<connection string PostgreSQL/Supabase>
Jwt__SecretKey=<chave forte gerada para produção>
Jwt__Issuer=atron-web-api
Jwt__Audience=atron-audience
Cors__AllowedOrigins__0=<origem HTTPS do Angular publicado>
EmailSettings__Provider=Brevo
EmailSettings__FromName=Atron Platform
EmailSettings__FromEmail=<remetente validado no Brevo>
EmailSettings__Brevo__ApiKey=<api key do Brevo>
EmailSettings__Brevo__BaseUrl=https://api.brevo.com/v3
```

`ATRON_CONNECTION_STRING` é a chave canônica do host. O código ainda aceita
`ConnectionStrings__DefaultConnection` e `ConnectionString` como fallbacks de
compatibilidade, mas o Render não precisa configurar valores duplicados.

Notificações Internas não exigem URL, emissor, audiência ou segredo de serviço.
A capacidade usa a mesma `ATRON_CONNECTION_STRING` e o mesmo JWT do Platform,
mantendo `NotificacoesDbContext` e migrations próprios.

### Rotação obrigatória

Se uma senha, token ou chave já foi versionada, removê-la do arquivo atual não a
torna segura, pois ela pode permanecer no histórico Git e em clones. Antes do
próximo deploy:

1. rotacione a senha do banco no Supabase;
2. rotacione qualquer chave de API ou segredo que tenha sido versionado;
3. atualize somente as variáveis protegidas no Render;
4. reinicie ou faça novo deploy do serviço;
5. confirme que a credencial anterior não autentica mais.

Não reutilize valores encontrados no histórico do repositório.

## Preparação local

Para configuração local, use User Secrets, variáveis de usuário ou um arquivo
`.env.render.local`, que é ignorado pelo Git e pelo contexto do Docker.

Exemplo com User Secrets no host neutro:

```powershell
dotnet user-secrets set "ATRON_CONNECTION_STRING" "<connection string>" --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
dotnet user-secrets set "Jwt:SecretKey" "<chave local forte>" --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
dotnet user-secrets set "EmailSettings:Brevo:ApiKey" "<api key>" --project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj
```

Exemplo de nomes para `.env.render.local`, sem preencher valores no
repositório:

```text
ASPNETCORE_ENVIRONMENT=Production
ATRON_CONNECTION_STRING=
Jwt__SecretKey=
Jwt__Issuer=atron-web-api
Jwt__Audience=atron-audience
Cors__AllowedOrigins__0=http://localhost:4200
EmailSettings__Provider=Brevo
EmailSettings__FromName=Atron Platform
EmailSettings__FromEmail=
EmailSettings__Brevo__ApiKey=
EmailSettings__Brevo__BaseUrl=https://api.brevo.com/v3
```

## Validação do container antes do deploy

Build:

```powershell
docker build --tag atron-platform:phase7 .
```

Execução:

```powershell
docker run --rm --name atron-platform-phase7 --publish 8080:8080 --env-file .env.render.local atron-platform:phase7
```

Em outro terminal:

```powershell
Invoke-WebRequest http://localhost:8080/api/saude
Invoke-WebRequest http://localhost:8080/swagger/v1/swagger.json
curl.exe --include http://localhost:8080/api/Tarefa
curl.exe --include http://localhost:8080/Auditoria/registro/contexto
curl.exe --include http://localhost:8080/api/Categoria
```

Resultados esperados:

- `/api/saude` responde `200`;
- o Swagger responde `200` e contém as rotas do Tracker e do Stock;
- `/api/Tarefa` sem token responde `401`;
- `/Auditoria/registro/contexto` sem token responde `401`;
- `/api/Categoria` sem token responde `401`.

A presença técnica das rotas do Stock não habilita card, navegação, listagem ou
formulário de Categoria no Angular. Essa entrega funcional permanece governada
pelo ADR 0006.

O preflight CORS deve ser testado com uma origem realmente configurada:

```powershell
$headers = @{
  Origin = "http://localhost:4200"
  "Access-Control-Request-Method" = "GET"
}
Invoke-WebRequest http://localhost:8080/api/Tarefa -Method Options -Headers $headers
```

## Publicação manual no Render

Esta etapa é executada somente na branch vinculada ao Web Service:

1. confirme que o serviço usa o `Dockerfile` da raiz;
2. rotacione as credenciais que já tenham sido versionadas;
3. configure as variáveis da seção de configuração segura;
4. registre o commit anterior ao corte como referência de rollback;
5. publique o commit que contém o host neutro;
6. aguarde o health check ficar saudável;
7. execute a validação publicada;
8. preserve a referência do último commit estável para rollback.

Esta fase não cria nem aplica migrations. Antes do deploy, confirme que as
migrations já aprovadas para Tracker, Shared e Auditoria estão aplicadas ao
banco correto.

## Validação publicada

Após o deploy, valide sem registrar credenciais em comandos ou documentação:

1. `GET /api/saude` responde `200`;
2. uma origem não autorizada não recebe cabeçalhos CORS de permissão;
3. a origem do Angular recebe o preflight esperado;
4. uma rota protegida sem JWT responde `401`;
5. o login gera um JWT aceito pelas rotas protegidas;
6. sessão, tarefas, perfis de acesso e planejamento de custos mantêm os
   contratos anteriores;
7. `GET /api/Categoria` sem JWT responde `401`;
8. consulta de Auditoria exige autenticação;
9. o Swagger contém os 19 contratos HTTP já existentes do Stock;
10. logs não exibem strings de conexão, tokens ou chaves.

Valide publicação, consulta, leitura e exclusão lógica. A rota
`/api/notificacoes/saude` comprova a conectividade do contexto de notificações;
`/api/saude` comprova apenas a vida do processo.

## Rollback

O rollback da Fase 7 não exige reversão de banco porque ela não altera schema.

Ordem recomendada:

1. interrompa a validação funcional e preserve os logs do deploy com falha;
2. republique o último commit estável anterior à remoção dos hosts transitórios;
3. mantenha as mesmas variáveis de banco, JWT, CORS e e-mail;
4. valide login, sessão e tarefas no commit restaurado;
5. reverta o Angular somente se a URL pública da API tiver sido alterada;
6. corrija o host neutro em uma nova mudança, sem recriar parcialmente um host
   legado.

Como alternativa de código, um commit de reversão pode restaurar integralmente
o estado anterior às Fases 8 e 9. Não recrie manualmente apenas um executável
antigo, pois isso pode produzir uma combinação de referências que nunca foi
validada.

## Limitações operacionais atuais

- planos gratuitos podem apresentar cold start;
- o diretório local de chaves de Data Protection do container ainda não é
  persistente entre deploys;
- a conectividade do `NotificacoesDbContext` deve ser verificada pela rota
  autenticada específica;
- logs, variáveis protegidas e histórico de deploy pertencem à operação no
  Render.
