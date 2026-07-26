# Contrato de notificações internas v1

## Publicação

PublicarNotificacaoInternaRequest é enviado por um módulo produtor para registrar uma notificação.

| Campo | Regra |
| --- | --- |
| DestinatarioCodigo | Código de usuário emitido no claim compartilhado CODIGO_USUARIO; obrigatório e normalizado em maiúsculas pelo produtor. |
| ModuloOrigem | Nome semântico do módulo produtor, como Tracker, Stock ou Sales. |
| TipoEvento | Tipo semântico do evento dentro do módulo de origem. |
| Titulo e Mensagem | Texto final em pt-BR definido pelo produtor. |
| UrlDestino | Deep link relativo opcional, sem depender de entidade do produtor. |
| ReferenciaExterna | Referência opaca opcional, como tarefa:123 ou produto:45. |
| DataCriacao | Data e hora fornecidas pelo produtor em UTC. |
| ChaveIdempotencia | Chave opcional para impedir publicação duplicada quando o adaptador puder reenviar. |
| CorrelacaoId | Identificador opcional propagado para rastrear uma tentativa de publicação entre o produtor e a central. |

## Consulta

NotificacaoInternaResponse é a representação de uma notificação para o destinatário autenticado. A identidade do destinatário é obtida exclusivamente do JWT da requisição de consulta e não integra o payload de resposta.

## Leitura e exclusão pelo destinatário

- `POST /api/notificacoes/{id}/marcar-como-lida` marca uma notificação do destinatário autenticado como lida.
- `POST /api/notificacoes/marcar-todas-como-lidas` marca as notificações pendentes do destinatário autenticado como lidas.
- `DELETE /api/notificacoes/{id}` exclui logicamente uma notificação da lista do destinatário autenticado. A operação não remove o conteúdo histórico do banco e retorna `404` quando o identificador não pertence ao usuário ou já está excluído.

A exclusão não faz parte do contrato de publicação e não adiciona entidades ou chaves estrangeiras de módulos produtores.

## Autenticação e falha

- Consulta, marcação de leitura e exclusão usam o JWT do usuário, com o claim CODIGO_USUARIO.
- Publicação usa token de serviço com a audiência `atron-notificacoes`, emissor e chave próprios, os claims `tipo_token=servico` e `escopo=notificacoes.publicar`.
- Consulta, marcação e exclusão aceitam somente JWT de usuário com o claim CODIGO_USUARIO; o token de serviço não possui acesso a essas operações.
- O adaptador compartilhado `AtronNotificacoes.Client` emite o token por cinco minutos, envia o `CorrelacaoId` também no cabeçalho `X-Correlation-Id` e retorna falha consultiva para indisponibilidade ou recusa HTTP.
- Até haver uma política específica aprovada, falha de publicação é consultiva: o caso de uso produtor preserva sua transação e registra aviso ou telemetria.
- Não há transação distribuída entre produtor e módulo de notificações.

## Identificadores

- A chave primária da notificação é um número inteiro longo gerado por sequence do banco.
- A sequence pode começar em um valor inicial aleatório, definido uma única vez na criação do banco, sem comprometer a geração monotônica posterior.
- Números aleatórios por registro não serão usados como chave primária: exigem controle adicional de colisão e não oferecem autorização.
- O identificador não concede acesso. Consulta e alteração sempre são limitadas ao destinatário autenticado.
