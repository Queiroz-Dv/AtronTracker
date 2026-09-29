# Operação das notificações internas

## Indicadores

O host `AtronPlatform.WebApi` expõe as métricas da capacidade no medidor `Atron.Notificacoes`, versão `1.0.0`.

| Instrumento | Tipo | Uso |
| --- | --- | --- |
| `atron_notificacoes_publicacoes_total` | contador | Publicações aceitas pela central. |
| `atron_notificacoes_falhas_publicacao_total` | contador | Falhas ocorridas ao persistir uma publicação. |

As métricas possuem somente as tags `modulo_origem` e `tipo_evento`. Não incluem destinatário, título, mensagem, URL, referência externa, chave de idempotência ou correlação, pois esses dados podem permitir identificação ou exposição de contexto de negócio.

O log estruturado da publicação inclui módulo, evento e correlação. Falhas devem gerar alerta quando houver crescimento sustentado do contador de falhas ou proporção anormal entre falhas e publicações por módulo.

## Saúde operacional

`GET /api/notificacoes/saude` verifica exclusivamente a conectividade do
`NotificacoesDbContext` e exige autenticação normal da plataforma.
`GET /api/saude` permanece como liveness anônimo do processo e não inclui essa
checagem de banco.

`GET /api/notificacoes/prontidao` permanece protegido e indica que a capacidade
foi composta pelo host.

## Consumo pelo Angular

O Angular consulta as rotas `GET /api/notificacoes`, `POST /api/notificacoes/{id}/marcar-como-lida`, `POST /api/notificacoes/marcar-todas-como-lidas` e `DELETE /api/notificacoes/{id}` pela mesma `apiRoute` do Platform.

O CORS é configurado uma única vez no Platform. A requisição envia o JWT de
usuário e a policy exige `CodigoUsuario`.

## Retenção

O histórico não é removido automaticamente nesta versão. Título e mensagem são registros históricos finais. Quando o destinatário exclui uma notificação, a central registra exclusão lógica e deixa o item fora da lista daquele usuário, sem eliminar a evidência operacional.

A definição futura de retenção deve informar prazo, base legal, responsáveis, rotina de expurgo auditável e tratamento de backups antes de qualquer expurgo físico ser adicionado.

## Atualização em tempo real

A política atual é consulta sob demanda pelo cliente, usando as rotas já existentes. Não há SignalR, WebSocket, polling automático nem push implementado. Uma evolução para tempo real deve definir primeiro volume esperado, autorização do canal, reconexão, ordenação, deduplicação e impacto de infraestrutura, mantendo a consulta HTTP como mecanismo de recuperação.
