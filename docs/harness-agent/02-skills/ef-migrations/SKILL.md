# EF Core Migrations

## Objetivo

Criar e revisar migrations do Entity Framework Core seguindo a estrutura do Atron.

## Procedimento

1. Identifique o módulo alterado.
2. Identifique o projeto Infrastructure que contém o DbContext.
3. Identifique o startup project.
4. Confirme o DbContext.
5. Gere a migration com nome descritivo.
6. Inspecione o arquivo gerado.
7. Verifique operações destrutivas.
8. Não aplique a migration em ambiente compartilhado sem autorização.

## Padrão

Use explicitamente:
- `--project`
- `--startup-project`
- `--context`

## Segurança

Se uma migration remover coluna, tabela, índice ou dados, destaque a operação antes de aplicá-la.
Avise o usuário sobre e envie os comandos de acordo com o projeto em questão. 

### Exemplos de comandos

``` powershell

--- Cria
dotnet ef migrations add AddDepartamentoTenant --project AtronPlatform/Modules/Tracker/Infrastructure/AtronTracker.Infrastructure.csproj --startup-project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --context AtronDbContext

-- Lista
dotnet ef migrations list --project AtronPlatform/Modules/Tracker/Infrastructure/AtronTracker.Infrastructure.csproj --startup-project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --context AtronDbContext


--- Aplica
dotnet ef database update --project AtronPlatform/Modules/Tracker/Infrastructure/AtronTracker.Infrastructure.csproj --startup-project AtronPlatform/WebApi/AtronPlatform.WebApi.csproj --context AtronDbContext
```

## Não fazer

Não apagar migrations existentes para resolver conflitos sem entender o histórico.