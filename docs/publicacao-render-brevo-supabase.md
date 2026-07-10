# Publicação no Render, Supabase e Brevo

## Objetivo

Este manual descreve o que precisa ser configurado para o Atron funcionar fora da máquina local usando:

- Render para publicar a API;
- Supabase/PostgreSQL como banco de dados;
- Brevo como provedor de envio de e-mail transacional.

Esses provedores foram escolhidos para manter o custo inicial reduzido, permitir publicação real do sistema e evitar que o projeto dependa de infraestrutura local ou de um banco SQL Server pago para rodar em produção.

## Decisão de infraestrutura

### Render

O Render hospeda a API do Atron a partir do `Dockerfile` da raiz do repositório. O Dockerfile publica o projeto `AtronTracker/WebApi/WebApi.csproj` e executa a aplicação na porta informada pelo próprio Render.

Por que usamos:

- permite colocar a API no ar com pouco atrito;
- lê variáveis de ambiente sem versionar credenciais;
- integra com GitHub para deploy por branch;
- reduz custo inicial de publicação.

Trade-offs:

- planos gratuitos ou econômicos podem ter cold start;
- a disponibilidade depende do provedor;
- logs, deploy e variáveis precisam ser administrados no painel do Render;
- mudanças de histórico Git exigem force push cuidadoso.

### Supabase/PostgreSQL

O Supabase é usado como destino principal de deploy do banco, usando PostgreSQL. No código, o provider `Supabase` é tratado como PostgreSQL via `Npgsql.EntityFrameworkCore.PostgreSQL`.

Por que usamos:

- oferece PostgreSQL gerenciado com custo inicial menor;
- evita depender de SQL Server para publicar a primeira versão;
- combina bem com Entity Framework Core e migrations separadas por provider;
- simplifica a criação de banco remoto para testes reais.

Trade-offs:

- o projeto passa a depender das limitações e políticas do Supabase;
- pooler, SSL e string de conexão precisam estar corretos;
- recursos específicos de SQL Server não devem ser assumidos no fluxo de produção;
- a rotação de senha exige atualização imediata das variáveis no Render.

### Brevo

O Brevo é usado para envio de e-mails transacionais, como primeiro acesso, troca de senha e notificações que exigem comunicação externa.

Por que usamos:

- evita depender de senha pessoal de Gmail ou Outlook em produção;
- separa envio transacional da conta pessoal do desenvolvedor;
- oferece API própria, mais adequada para aplicação publicada;
- permite trocar provedor sem mudar os casos de uso do sistema.

Trade-offs:

- depende da reputação e dos limites do provedor;
- exige chave de API protegida;
- domínio/remetente precisam estar configurados corretamente;
- falhas de e-mail devem bloquear fluxos críticos quando a regra exigir entrega.

## Configuração no Supabase

1. Crie ou acesse o projeto do Supabase.
2. Copie a connection string PostgreSQL recomendada para aplicação.
3. Use senha atual do banco. Se alguma senha já foi versionada, gere uma nova antes de publicar.
4. Confirme se a string contém SSL habilitado.
5. Aplique as migrations PostgreSQL do projeto antes de usar o sistema em produção.

Formato esperado no Render:

```text
ConnectionStrings__DefaultConnection=<connection string PostgreSQL/Supabase>
ConnectionString=<mesma connection string, enquanto o fallback legado existir>
```

`ConnectionStrings__DefaultConnection` é a configuração principal. `ConnectionString` existe por compatibilidade com código legado e deve receber o mesmo valor enquanto esse fallback ainda estiver ativo.

## Configuração no Brevo

1. Crie ou acesse a conta no Brevo.
2. Configure o remetente que será usado pelo Atron.
3. Gere uma API key.
4. Não coloque a API key em `appsettings*.json`.
5. Configure a chave no Render como variável de ambiente.

Variáveis:

```text
EmailSettings__Provider=Brevo
EmailSettings__FromName=Atron Platform
EmailSettings__FromEmail=<remetente configurado no Brevo>
EmailSettings__Brevo__ApiKey=<api key do Brevo>
EmailSettings__Brevo__BaseUrl=https://api.brevo.com/v3
```

Se o projeto for configurado para SMTP em algum ambiente, use:

```text
EmailSettings__Password=<senha SMTP ou senha de app>
```

Para produção, a direção preferida é Brevo via API.

## Configuração no Render

1. Crie um Web Service no Render.
2. Conecte o repositório do GitHub.
3. Escolha a branch que será publicada.
4. Use o Dockerfile da raiz do repositório.
5. Configure as variáveis de ambiente.
6. Faça deploy.

Variáveis mínimas para a API:

```text
ASPNETCORE_ENVIRONMENT=Production
ConnectionStrings__DefaultConnection=<connection string PostgreSQL/Supabase>
ConnectionString=<mesma connection string, se o fallback legado ainda estiver em uso>
Jwt__SecretKey=<chave forte gerada para produção>
Jwt__Issuer=atron-web-api
Jwt__Audience=atron-audience
EmailSettings__Provider=Brevo
EmailSettings__FromName=Atron Platform
EmailSettings__FromEmail=<remetente configurado no Brevo>
EmailSettings__Brevo__ApiKey=<api key do Brevo>
EmailSettings__Brevo__BaseUrl=https://api.brevo.com/v3
Cors__AllowedOrigins__0=<URL do front Angular publicado>
```

Observações:

- `Jwt__SecretKey` precisa ser forte e diferente da chave de desenvolvimento.
- `Cors__AllowedOrigins__0` deve apontar para a origem real do front.
- Se a senha do Supabase for trocada, atualize `ConnectionStrings__DefaultConnection` e `ConnectionString`.
- Depois de alterar variáveis sensíveis, faça restart ou redeploy do serviço.

## Configuração local

Para rodar localmente sem versionar credenciais, use uma destas opções:

- variáveis de ambiente do usuário;
- User Secrets do .NET;
- `appsettings.Local.json`, que deve continuar ignorado pelo Git.

Exemplo com User Secrets no projeto da API:

```powershell
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<connection string>" --project AtronTracker/WebApi/WebApi.csproj
dotnet user-secrets set "ConnectionString" "<connection string>" --project AtronTracker/WebApi/WebApi.csproj
dotnet user-secrets set "Jwt:SecretKey" "<chave local>" --project AtronTracker/WebApi/WebApi.csproj
dotnet user-secrets set "EmailSettings:Brevo:ApiKey" "<api key>" --project AtronTracker/WebApi/WebApi.csproj
```