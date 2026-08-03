# Migração do histórico de notificações do Tracker

## Objetivo

Registrar a substituição da tabela de teste `NotificacoesInternas` pela tabela central `Notificacoes` no Supabase compartilhado.

## Segurança e idempotência

Em 20/07/2026, `NotificacoesInternas` foi inspecionada e continha um único registro de teste. Não houve migração de dados.

A tabela foi removida somente após a criação de `Notificacoes`. O registro descartado não é recuperável sem restauração do backup operacional do Supabase.

## Ordem de implantação

1. Aplicar as migrations do Core de AtronNotificacoes no Supabase compartilhado. A capacidade usa a tabela própria `Notificacoes` e o histórico `__AtronNotificacoesMigrationsHistory`.
2. Publicar `AtronPlatform.WebApi`, que compõe a capacidade in-process.
3. Apontar o Angular para a `apiRoute` do Platform.
4. Validar no navegador autenticado consulta, marcação individual, marcação total, exclusão e deep links.
5. Monitorar os contadores de publicação e falha no processo do Platform.

## Critérios para retirada do legado

Antes de remover o fluxo legado do Tracker, confirmar todos os itens abaixo:

- a tabela central `Notificacoes` está disponível e a migration inicial consta como aplicada;
- o Platform responde à consulta e às duas operações de leitura com JWT de usuário;
- o Angular em cada ambiente configurado usa a URL do Platform, sem uma URL exclusiva de notificações;
- o plano de reversão informa como restaurar temporariamente a fachada sem recriar a tabela legada;
- os indicadores de publicação e falha permaneceram estáveis durante a janela acordada.

## Execução

Para futuras inspeções controladas do legado, o utilitário lê `ConnectionStrings:DefaultConnection` do Tracker. As variáveis de ambiente têm precedência:

```powershell
$env:TRACKER_CONNECTION_STRING = "..."
dotnet run --project Tools/NotificacoesLegacyMigrator/NotificacoesLegacyMigrator.csproj -- --inspecionar-legado
```

O modo de inspeção não grava dados. A remoção controlada exige a criação prévia de `Notificacoes` e rejeita uma tabela legada com mais de um registro.
