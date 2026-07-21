# Migração do histórico de notificações do Tracker

## Objetivo

Registrar a substituição da tabela de teste `NotificacoesInternas` pela tabela central `Notificacoes` no Supabase compartilhado.

## Segurança e idempotência

Em 20/07/2026, `NotificacoesInternas` foi inspecionada e continha um único registro de teste. Não houve migração de dados.

A tabela foi removida somente após a criação de `Notificacoes`. O registro descartado não é recuperável sem restauração do backup operacional do Supabase.

## Ordem de implantação

1. Aplicar as migrations do `AtronNotificacoes` no Supabase compartilhado. A central usa a tabela própria `Notificacoes` e o histórico `__AtronNotificacoesMigrationsHistory`, sem conflito com o legado `NotificacoesInternas`.
2. Configurar `NotificacoesInternas:BaseUrl` e as credenciais de serviço no Tracker e no host de notificações.
3. Configurar a URL da central no ambiente Angular e liberar somente essa origem no CORS do host de notificações.
4. Validar no navegador autenticado a consulta, a marcação individual, a marcação total e os deep links usando `api/notificacoes` diretamente.
5. Confirmar que `NotificacoesInternas` contém apenas dados de teste.
6. Remover a tabela de teste após a criação bem-sucedida de `Notificacoes`.
7. Remover o fluxo legado do Tracker na fase seguinte.
8. Monitorar publicação e consulta na central antes de encerrar a transição.

## Critérios para retirada do legado

Antes de remover o fluxo legado do Tracker, confirmar todos os itens abaixo:

- a tabela central `Notificacoes` está disponível e a migration inicial consta como aplicada;
- a central responde à consulta e às duas operações de leitura com JWT de usuário;
- o Angular em cada ambiente configurado usa a URL da central, sem chamadas a `api/NotificacaoInterna`;
- o plano de reversão informa como restaurar temporariamente a fachada sem recriar a tabela legada;
- os indicadores de publicação e falha permaneceram estáveis durante a janela acordada.

## Execução

Para futuras inspeções controladas do legado, o utilitário lê `ConnectionStrings:DefaultConnection` do Tracker. As variáveis de ambiente têm precedência:

```powershell
$env:TRACKER_CONNECTION_STRING = "..."
dotnet run --project Tools/NotificacoesLegacyMigrator/NotificacoesLegacyMigrator.csproj -- --inspecionar-legado
```

O modo de inspeção não grava dados. A remoção controlada exige a criação prévia de `Notificacoes` e rejeita uma tabela legada com mais de um registro.
