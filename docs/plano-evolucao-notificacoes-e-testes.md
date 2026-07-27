# Plano de evolução das notificações internas e dos testes

> Documento histórico das fases executadas em julho de 2026. A topologia HTTP,
> o cliente interno e o host independente descritos abaixo foram substituídos
> pelo ADR 0008. Os contratos, dados, migrations e comportamentos funcionais
> continuam válidos.

## Objetivo

Concluir a extração das notificações internas do Tracker, permitir publicação por qualquer módulo sem acoplamento a entidades produtoras e centralizar projetos de teste com nomes curtos por módulo.

## Princípios

- A central conhece somente o contrato de notificação, nunca entidades de Tracker, Stock ou Sales.
- Produtores usam `INotificacoesInternasPublisher` por composição. Não criar uma classe base herdável para serviços de domínio.
- Cada produtor mantém a decisão de publicar, destinatário, texto final, URL e referência opaca.
- A publicação de eventos consultivos não reverte a transação do produtor.
- Os testes têm nomes por módulo e funções em pastas, não um projeto por funcionalidade pequena.
- Migrations são responsabilidade da Infrastructure interna do
  `AtronNotificacoes.Core` e ficam em
  `Framework/AtronNotificacoes/Infrastructure/Migrations`; não haverá projeto
  separado apenas para migrations.

## Fase 0: Consolidar linha de base e documentação

**Objetivo:** registrar a estrutura anterior à reorganização e eliminar ambiguidades documentais sem mover projetos, arquivos de teste ou código de produção.

**Escopo:** registrar os projetos atuais de teste, a execução vinculada de `TarefaPreparacaoServiceTests.cs`, os comandos de validação e a decisão de organização do módulo. Corrigir os documentos para preservar leitura, marcação individual, marcação total e exclusão lógica como comportamentos ativos da central.

**Linha de base em 20/07/2026:** `Application.Tests` e `Application.Resources.Tests` ainda estão em `AtronTracker`; o segundo inclui por link `TarefaPreparacaoServiceTests.cs` do primeiro. `AtronStock.Application.Tests` está em `AtronStock`; `Shared.Tests` está em `Framework`; e a central está dividida entre `AtronNotificacoes.Contracts.Tests`, `AtronNotificacoes.Client.Tests` e `AtronNotificacoes.Tests`. A lista de testes foi capturada com `dotnet test AtronPlatform.sln --list-tests --no-restore -p:OutDir=C:\p\Projetos\AtronRC\artifacts\phase0-test-baseline\`. Nenhum projeto de teste foi movido nesta fase.

**Critérios de aceite:** a decisão de leitura não fica pendente, o destino físico de migrations está registrado, a lista de testes executada é capturada antes de cada migração e não há alteração de código ou de caminho de projeto.

## Fase 1: Preparar a transição direta do Angular

**Objetivo:** deixar o front apto a consultar a central sem remover a compatibilidade ainda.

**Escopo:** configurar URL da central por ambiente, ajustar o serviço Angular para as rotas da central, definir CORS e criar testes de contrato da consulta, marcação individual, marcação total e exclusão de uma notificação pelo próprio destinatário. A exclusão é lógica: a notificação deixa a lista do usuário, mas o histórico operacional permanece na central.

**Critérios de aceite:** o navegador autenticado consulta a central diretamente; o JWT de usuário é aceito; deep links e estado de leitura são preservados; o usuário pode excluir uma notificação específica da lista e não pode excluir registros de outro destinatário; nenhuma chamada nova do front depende de `api/NotificacaoInterna`.

**Dependências e riscos:** URL publicada da central, configuração de CORS e disponibilidade simultânea dos dois hosts durante a janela de compatibilidade. A ação precisa ser exposta nas duas superfícies atuais de notificações do Angular para não criar comportamento divergente entre menu e central.

## Fase 2: Substituir a estrutura de teste pela central

**Objetivo:** tornar a central a fonte de consulta em uma tabela própria e limpa no Supabase compartilhado.

**Escopo:** criar `Notificacoes` e `__AtronNotificacoesMigrationsHistory`; descartar `NotificacoesInternas` após confirmar que continha somente o registro de teste; validar o esquema e os testes da central.

**Critérios de aceite:** a migration inicial da central está aplicada; `Notificacoes` existe; `NotificacoesInternas` não existe; publicação continua consultiva.

**Dependências e riscos:** descarte irreversível do registro de teste sem backup lógico; o fluxo legado do Tracker precisa ser removido na fase seguinte para que migrations futuras não recriem a estrutura antiga.

## Revisão de simplificação da central

**Decisão de responsabilidade:** a geração permanece nos serviços dos módulos produtores por meio de `INotificacoesInternasPublisher`. Eles definem evento, destinatário, texto final, destino e referência. A central não recebe entidades de domínio e não expõe criação ao usuário.

**Superfície HTTP mínima:** publicação técnica para módulos autenticados; consulta da caixa do usuário; exclusão lógica individual e, quando aprovada, exclusão lógica de toda a lista. A consulta não pode ser removida porque a tela Angular precisa carregar a lista.

**Decisão consolidada sobre leitura:** a central preserva estado de leitura, contador, filtro, marcação individual e marcação total. O Angular usa as rotas da central `marcar-como-lida` e `marcar-todas-como-lidas`; exclusão lógica é uma ação complementar e não substitui leitura. Qualquer alteração futura dessa experiência deve remover ou alterar em conjunto campo, filtros, contador, ações, contratos e testes.

**Não recomendado:** escrita direta do Angular ou dos módulos produtores no Supabase. Isso dispersaria autorização, idempotência, observabilidade e validação de destinatário fora da central.

**Gate da Fase 3:** configurar publicação autenticada no Tracker e validar consulta, leitura, exclusão e criação de uma notificação de tarefa na central antes de retirar o legado.

## Fase 3: Retirar o legado de notificações do Tracker

**Objetivo:** Tracker se torna apenas produtor de notificações.

**Escopo:** remover controller, DTO de compatibilidade, serviço, repositório, entidade, configuração EF, DbSet, registrations e testes exclusivos do fluxo legado; tratar tabela e migrations conforme a janela operacional aprovada. Preservar leitura e não leitura nesta fase e alinhar as rotas diretas do Angular à central.

**Critérios de aceite:** busca de runtime no Tracker não encontra fluxo legado; criação e obtenção de tarefas publicam pela central; testes de Tracker, central e front passam.

**Dependências e riscos:** Fases 1 e 2 aprovadas. O risco é remover dados ou rota ainda usados; mitigar com monitoramento e reversão da fachada durante a janela definida.

**Implementação em 20/07/2026:** o runtime legado foi removido e a migration `RemoverNotificacoesInternasLegadas`, com `DROP TABLE IF EXISTS`, foi aplicada no Supabase. Resta o teste autenticado de publicação, dependente da configuração de URL e credenciais da central no Tracker.

**Validação operacional em 21/07/2026:** os builds de `AtronTracker/WebApi`, `AtronNotificacoes` e `AtronFront` foram aprovados. A central aceita preflight CORS para a origem publicada do Angular, mas o host atualmente configurado no front responde `404` a `GET /api/notificacoes`. No Tracker, `NotificacoesInternas:BaseUrl` e o segredo de serviço não estão configurados, portanto o contêiner registra o publicador indisponível. O gate permanece pendente até publicar a central em uma URL definida, configurar URL e credenciais por variáveis seguras nos produtores, apontar o Angular para essa URL e executar os cenários autenticados de publicação, consulta, leitura, exclusão e deep link.

## Fase 4: Consolidar os testes do Tracker

**Status:** concluída em 20/07/2026.

**Objetivo:** estabelecer o padrão de localização com `Tests/Tracker.Tests`.

**Escopo:** mover os testes atuais do Tracker, unificar `Application.Tests` e `Application.Resources.Tests`, organizar por funcionalidade e eliminar o arquivo vinculado entre projetos.

**Critérios de aceite:** a contagem de testes do Tracker é preservada; cada arquivo pertence a um único projeto; `dotnet test Tests/Tracker.Tests/Tracker.Tests.csproj` e a solução passam.

**Dependências e riscos:** revisar filtros de CI e nomes de assembly. Não mistura remoção de legado com mudança mecânica de testes na mesma alteração. A lista de testes deve ser capturada antes e depois da mudança para comprovar a preservação da cobertura.

**Execução em 20/07/2026:** os testes de `Application.Tests` e `Application.Resources.Tests` foram reunidos em `Tests/Tracker.Tests/Tracker.Tests.csproj`. `TarefaPreparacaoServiceTests.cs` permanece somente em `Tarefas/`, sem `Compile Include` vinculado. `dotnet test Tests/Tracker.Tests/Tracker.Tests.csproj` aprovou 62 testes, e `dotnet test AtronPlatform.sln` aprovou a solução em saída isolada.

## Fase 5: Aplicar a estrutura aos demais módulos

**Status:** concluída em 20/07/2026.

**Objetivo:** usar o mesmo padrão em Stock, Shared e Notificacoes.

**Escopo:** criar `Stock.Tests`, `Shared.Tests` e `Notificacoes.Tests`; mover em fatias independentes e manter contratos, cliente e integração da central em pastas de `Notificacoes.Tests` enquanto não houver necessidade real de isolamento. Reorganizar fisicamente `AtronNotificacoes` para que `Migrations/` fique dentro de `Infrastructure/`, removendo o projeto separado de migrations e preservando o assembly `AtronNotificacoes.Infrastructure` como dono do contexto, repositório e migrations.

**Critérios de aceite:** nomes curtos na solução, testes preservados e CI apontando para os novos caminhos.

**Dependências e riscos:** executar um módulo por vez para que uma falha de referência seja localizada sem ruído.

**Execução em 20/07/2026:** `AtronStock.Application.Tests` foi movido para `Tests/Stock.Tests/Stock.Tests.csproj`, e `Framework/Shared.Tests` foi movido para `Tests/Shared.Tests/Shared.Tests.csproj`. Os namespaces do Stock foram simplificados para `Stock.Tests`; Shared preservou o nome já compatível. Os antigos projetos `AtronNotificacoes.Contracts.Tests`, `AtronNotificacoes.Client.Tests` e `AtronNotificacoes.Tests` foram reunidos em `Tests/Notificacoes.Tests/Notificacoes.Tests.csproj`, nas pastas `Contratos`, `Cliente`, `Integracao` e `Autorizacao`. As migrations e o factory de design foram movidos para `AtronNotificacoes.Infrastructure/Migrations`; o projeto separado foi removido da solução, e o host passou a usar o assembly `AtronNotificacoes.Infrastructure` para migrations. `dotnet test Tests/Stock.Tests/Stock.Tests.csproj` aprovou 5 testes, `dotnet test Tests/Shared.Tests/Shared.Tests.csproj` aprovou 12 testes, `dotnet test Tests/Notificacoes.Tests/Notificacoes.Tests.csproj` aprovou 20 testes e a solução foi aprovada em saída isolada.

## Fase 6: Adotar o próximo evento real de Stock

**Status:** já implementada. Stock publica `SaidaEstoqueRegistrada` pelo contrato transversal.

**Objetivo:** provar o reuso do contrato em um segundo produtor.

**Escopo:** manter a publicação de `SaidaEstoqueRegistrada` no módulo Stock, usando o mesmo publicador e seus próprios resources.

**Critérios de aceite:** Stock publica sem referenciar domínio de Tracker e sem mudar o contrato v1; a indisponibilidade da central não reverte a operação de estoque.

**Dependências e riscos:** configuração de destinatário e validação de integração autenticada.

## Fora do escopo

- criar Sales sem caso de uso formal;
- atualização em tempo real;
- retenção automática;
- classe base de notificação ou abstração compartilhada sem duplicidade comprovada.
