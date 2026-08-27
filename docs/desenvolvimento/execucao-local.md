# Execução local

Este guia descreve a preparação do Atron para desenvolvimento. O backend ativo é `AtronPlatform.WebApi`; não é necessário iniciar hosts separados de Tracker, Stock ou Notificações.

## Pré-requisitos

| Item | Referência do repositório |
| --- | --- |
| Visual Studio com suporte a ASP.NET Core e .NET 9 | Solução `AtronPlatform.sln`; host com alvo `net9.0`. |
| SDK .NET 9 | Versão usada no workflow de CI para restaurar, compilar e testar. |
| Node.js 22 e npm | Versão de Node usada no workflow do frontend. |
| PostgreSQL de desenvolvimento | Base isolada, credenciais próprias e esquema compatível com as migrations. |
| Conta de acesso e permissões | Necessárias para explorar as rotinas autenticadas. |

Estas versões descrevem a configuração do projeto, não uma recomendação de versões mais recentes. Não use a base de produção para experimentar cadastros, lotes ou migrations.

## 1. Abrir a solução no Visual Studio

Abra `AtronPlatform.sln` e defina `AtronPlatform.WebApi`, em `AtronPlatform/WebApi`, como projeto de inicialização. Aguarde a restauração dos pacotes.

O perfil de inicialização chamado `AtronPlatform.WebApi` define o ambiente `Development`, HTTPS na porta `7280` e HTTP na porta `5180`. Confira o [launchSettings.json](../../AtronPlatform/WebApi/Properties/launchSettings.json) caso essas portas sejam alteradas.

O uso do Git integrado ao Visual Studio está no [guia de branches](fluxo-git.md). Os comandos npm abaixo são apenas para executar a interface.

## 2. Configurar o host sem versionar segredos

Use User Secrets do projeto `AtronPlatform.WebApi`, variáveis de ambiente ou `AtronPlatform/WebApi/appsettings.Local.json`. O host possui `UserSecretsId`, e `appsettings.Local.json` está ignorado pelo Git. Não coloque credenciais em arquivos versionados ou em capturas de tela.

| Chave de configuração | Variável de ambiente equivalente | Finalidade |
| --- | --- | --- |
| `ATRON_CONNECTION_STRING` | `ATRON_CONNECTION_STRING` | Conexão canônica com o PostgreSQL de desenvolvimento. |
| `Jwt:SecretKey` | `Jwt__SecretKey` | Segredo próprio do ambiente para assinatura de tokens. |
| `Jwt:Issuer` | `Jwt__Issuer` | Emissor configurado para os tokens. |
| `Jwt:Audience` | `Jwt__Audience` | Audiência configurada para os tokens. |
| `Cache:Provider` | `Cache__Provider` | Use `Memory` para começar sem Redis. |
| `Cors:AllowedOrigins:0` | `Cors__AllowedOrigins__0` | Origem da interface, normalmente `http://localhost:4200`. |

Para preencher a conexão, use os dados do seu PostgreSQL. Para o JWT, gere um segredo privado para desenvolvimento e mantenha emissor e audiência consistentes com a configuração do host. Não reutilize segredos encontrados no histórico Git.

**Atenção à precedência:** o [Program.cs](../../AtronPlatform/WebApi/Program.cs) acrescenta `appsettings.Local.json` depois dos provedores padrão. Um valor nesse arquivo pode sobrescrever o mesmo nome vindo de User Secrets ou variáveis de ambiente. Evite manter a mesma chave em vários lugares.

A conexão aceita fallbacks de compatibilidade, mas configure explicitamente `ATRON_CONNECTION_STRING` para não depender de arquivos antigos presentes na máquina.

Para validar operações que enviam e-mail, configure também o provedor e as respectivas credenciais, conforme o [guia de configuração](../publicacao-render-brevo-supabase.md). Use destinatários de teste. Para Redis, siga o [guia de cache](../cache-redis.md); não é necessário habilitá-lo na primeira execução com cache em memória.

## 3. Preparar dados e migrations

Iniciar o host não provisiona automaticamente uma base completa para demonstração. Antes de testar as rotinas, a base precisa conter os esquemas compatíveis, os cadastros necessários e uma conta autorizada.

As migrations têm proprietários distintos:

| Capacidade | Local das migrations |
| --- | --- |
| Tracker | `AtronPlatform/Modules/Tracker/Infrastructure/Migrations` |
| Stock | `AtronPlatform/Modules/Stock/Infrastructure/Migrations` |
| Auditoria compartilhada | `Framework/Shared/Migrations` |
| Notificações internas | `Framework/AtronNotificacoes/Infrastructure/Migrations` |

Confira os contextos e seus históricos antes de aplicar migrations. Não execute uma atualização genérica de banco sem identificar o contexto, o projeto de migrations e a conexão de destino. Consulte as [fronteiras de persistência](../arquitetura/fronteiras-e-propriedade-da-plataforma.md).

Este guia não fornece um bootstrap universal de primeiro usuário nem um pacote de dados de demonstração. Se a base estiver vazia, combine com o mantenedor a preparação de uma base de teste e os acessos necessários antes de seguir para o login.

## 4. Iniciar e verificar a API

Inicie o perfil `AtronPlatform.WebApi` pelo Visual Studio. Se o certificado HTTPS de desenvolvimento não for confiável no navegador, regularize o certificado local antes de testar a interface; não desabilite a validação de certificados na aplicação.

No perfil de desenvolvimento:

- [Swagger](https://localhost:7280/swagger): contratos HTTP.
- [ReDoc](https://localhost:7280/docs): outra visualização da documentação HTTP.
- [Saúde do host](https://localhost:7280/api/saude): verificação básica do host.

Swagger e ReDoc são habilitados apenas em `Development`. O endpoint geral de saúde não comprova acesso a todos os bancos nem funcionamento das rotinas autenticadas. Notificações possuem uma verificação separada em `/api/notificacoes/saude`, que exige autenticação.

## 5. Iniciar o Angular

No terminal, a partir da raiz do repositório:

```powershell
cd AtronFront
npm ci
npm start
```

`npm ci` instala as versões registradas no lockfile; não é preciso reinstalar as dependências a cada inicialização. `npm start` executa o servidor de desenvolvimento e abre o navegador, normalmente em `http://localhost:4200`.

A configuração de desenvolvimento usa `https://localhost:7280/`, conforme [environment.development.ts](../../AtronFront/src/environments/environment.development.ts). Se mudar a porta do backend, mantenha as duas configurações alinhadas.

**O build de produção utiliza o ambiente com a API publicada no Render.** Para experimentar contra a API local, use a configuração de desenvolvimento. Não faça testes de cadastro a partir de um artefato apontando para produção.

## 6. Conferir o fluxo autenticado

Entre com a conta preparada para desenvolvimento. Verifique a navegação de Tracker e Stock e faça operações apenas nos dados de teste.

| Sintoma | O que conferir primeiro |
| --- | --- |
| API não inicia | Saída do host no Visual Studio, configuração da conexão, porta disponível e dependências registradas. |
| Front não alcança a API | API iniciada, endereço do ambiente Angular, confiança no certificado e origem permitida no CORS. |
| `401` | Sessão/token e configuração JWT do ambiente. |
| `403` ou cartão indisponível | Permissões da conta e módulos vinculados ao perfil; Sales está desabilitado na configuração atual. |
| Erro de tabela ou coluna ausente | Contexto, migrations e base de destino. |
| Lote aceito sem produtos imediatos | Estado do processamento, funcionamento do worker e persistência; `202` não indica conclusão. |

## Validação e limites

Os projetos .NET podem ser explorados no Test Explorer do Visual Studio. Para reproduzir as etapas automatizadas, consulte o [workflow de CI](../../.github/workflows/ci.yml) e a [organização dos testes](../arquitetura-testes.md).

Esta documentação foi conferida com a configuração e o código do repositório. Sua atualização não incluiu provisionar uma nova base, executar a aplicação ou validar um login real. A suíte Jasmine não foi executada nesta revisão e não integra o workflow atual.
