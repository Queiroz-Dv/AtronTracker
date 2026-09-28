# Plano de implementação de resources e templates de e-mail

## 1. Objetivo

Este documento orienta a padronização das mensagens observáveis do backend e a separação da composição dos e-mails no AtronRC.

O trabalho possui duas frentes:

1. Centralizar em arquivos resource as mensagens de validação, erro, sucesso e aviso retornadas pelo backend.
2. Retirar os corpos HTML atualmente escritos dentro de arquivos `.cs`, mantendo a composição separada do transporte por SMTP ou Brevo.

O rollout será realizado em fases pequenas e verificáveis. Ao final de cada fase, a implementação deve parar para revisão antes de avançar.

## 2. Decisões consolidadas

### 2.1 Idioma

- A implementação inicial será somente em pt-BR.
- Não serão criados resources para outros idiomas nesta rodada.
- Não será implementada seleção de cultura por usuário ou requisição.
- A internacionalização será tratada como evolução futura, quando existir um requisito real.

### 2.2 Notificações internas

- A entidade de notificação interna continuará persistindo o título e a mensagem finais em pt-BR.
- Não serão persistidos nome de resource, chave de tradução, parâmetros de formatação ou cultura.
- Não haverá alteração de esquema de banco relacionada a localização.
- Uma mudança futura no resource afetará somente as novas notificações.
- As notificações antigas conservarão o texto entregue no momento em que foram criadas.

Fluxo aprovado:

```text
Evento do módulo
    -> Resource pt-BR
    -> Título e mensagem finalizados
    -> Notificação interna
    -> Persistência
```

### 2.3 E-mails

- O corpo dos e-mails será mantido em arquivos HTML.
- Os templates serão incorporados ao assembly para evitar dependência de caminho físico durante publicação, Docker ou Render.
- Assuntos e textos traduzíveis ficarão em resources pt-BR.
- Valores dinâmicos serão fornecidos por modelos tipados.
- Dados dinâmicos serão codificados antes de sua inclusão no HTML.
- URLs serão validadas antes de serem inseridas em atributos de links.
- `IEmailService` continuará responsável somente pelo transporte.
- A composição do e-mail não conhecerá detalhes de SMTP ou Brevo.

### 2.4 Resources

- Cada módulo deve ser proprietário das próprias mensagens.
- `Framework.Shared` deve manter somente mensagens realmente compartilhadas.
- Configurações técnicas, domínios de provedores e hosts SMTP não devem ser tratados como texto localizável.
- Arquivos `.Designer.cs` gerados a partir dos resources não devem ser editados manualmente.

## 3. Diagnóstico do estado atual

### 3.1 Resources

O backend possui 12 arquivos `.resx` base, sem variantes específicas por cultura.

A varredura direcionada encontrou 199 ocorrências candidatas a texto observável ou texto que precisa ser classificado, distribuídas em 45 arquivos.

| Área | Ocorrências candidatas | Arquivos |
|---|---:|---:|
| AtronTracker Application | 146 | 29 |
| AtronTracker WebApi | 6 | 4 |
| AtronStock Application | 27 | 2 |
| AtronStock WebApi | 2 | 2 |
| AtronEmail | 2 | 1 |
| AtronAuditoria | 0 | 0 |
| Framework.Shared Application | 9 | 4 |
| Framework.Shared Domain | 7 | 3 |
| Total | 199 | 45 |

Essas ocorrências devem ser classificadas antes da migração. Nem todo texto literal precisa virar resource.

Categorias de classificação:

- mensagem retornada ao usuário;
- validação;
- aviso;
- notificação interna;
- assunto ou conteúdo de e-mail;
- histórico ou auditoria;
- mensagem técnica;
- configuração ou constante de protocolo.

### 3.2 E-mails

Foram identificados 11 corpos HTML distribuídos em 8 classes C#.

Os principais fluxos são:

- cadastro público;
- confirmação de e-mail;
- reenvio de confirmação;
- recuperação de senha;
- primeiro acesso de usuário interno;
- alteração de e-mail;
- reativação de conta;
- confirmação concluída;
- atribuição de tarefa;
- notificação genérica;
- diagnóstico do serviço de e-mail.

O transporte compartilhado já existe por meio de `IEmailService` e `SharedEmailService`. A principal correção é separar a composição do conteúdo dos casos de uso e serviços de aplicação.

### 3.3 Política de falha de entrega

Os fluxos atuais não tratam falha de envio da mesma forma:

- alguns propagam a falha;
- a atribuição de tarefa devolve um aviso;
- alguns envios ignoram o `Resultado`;
- existem pontos que capturam exceções sem informar o chamador.

Antes de alterar o comportamento de qualquer fluxo, cada e-mail deve ser classificado como:

| Política | Comportamento |
|---|---|
| Obrigatório | A operação informa falha quando o envio não é concluído. |
| Consultivo | A operação principal conclui e devolve um aviso sobre o e-mail. |
| Assíncrono | A operação registra uma pendência para tentativa posterior. Fora do escopo inicial. |

Proposta inicial para validação durante a Fase 0:

| Fluxo | Política proposta |
|---|---|
| Recuperação de senha | Obrigatório |
| Reenvio de confirmação | Obrigatório |
| Alteração de e-mail | Obrigatório |
| Código de reativação | Obrigatório |
| Atribuição de tarefa | Consultivo |
| Confirmação concluída | Consultivo |
| Primeiro acesso de usuário interno | Decisão pendente |
| Cadastro público com confirmação | Decisão pendente |

## 4. Arquitetura pretendida

### 4.1 Mensagens do backend

```text
Controller ou serviço de aplicação
    -> Regra do módulo
    -> Resource do módulo
    -> Resultado
    -> WebApi
    -> AtronFront
```

O controller não deve repetir uma mensagem já definida pelo módulo. Mensagens transversais, como divergência entre identificador da rota e corpo, podem ficar em um resource compartilhado.

### 4.2 Notificação interna

```text
TarefaService
    -> TarefaResource
    -> Título e mensagem em pt-BR
    -> NotificacaoInternaService
    -> NotificacaoInternaRepository
    -> Banco
```

O frontend continua recebendo o título e a mensagem finais. Não haverá operador de tradução nesta etapa.

### 4.3 E-mail

```text
Caso de uso
    -> Compositor do módulo
    -> Modelo tipado
    -> Renderizador de template HTML
    -> EmailRequest
    -> IEmailService
    -> SMTP ou Brevo
```

Responsabilidades:

| Módulo | Responsabilidade |
|---|---|
| Caso de uso | Decidir quando o e-mail deve ser enviado e como a falha afeta a operação. |
| Compositor | Selecionar assunto, template e dados necessários. |
| Renderizador | Carregar o HTML, validar campos e produzir o corpo final. |
| `IEmailService` | Enviar assunto, destinatários e HTML. |
| `SharedEmailService` | Implementar os adaptadores SMTP e Brevo. |

Estrutura inicial sugerida:

```text
Email/
  Templates/
    pt-BR/
      base.html
      confirmacao-cadastro.html
      recuperacao-senha.html
      primeiro-acesso.html
      alteracao-email.html
      reativacao-conta.html
      tarefa-atribuida.html
  Rendering/
    IEmailTemplateRenderer.cs
    EmailTemplateRenderer.cs
  Compositores/
    CompositorEmailAcesso.cs
    CompositorEmailTarefa.cs
```

A estrutura definitiva deve respeitar os projetos proprietários. Templates específicos de Tarefas não devem obrigar `Framework.Shared` a conhecer regras de Tarefas.

## 5. Plano de implementação

## Fase 0: contrato e inventário executável

### Objetivo

Transformar as decisões deste plano em regras verificáveis antes de alterar comportamento.

### Implementação

- [x] Criar ADR para resources, notificações internas e templates de e-mail.
- [x] Registrar a decisão de manter somente pt-BR.
- [x] Registrar que notificações internas persistem texto final.
- [x] Registrar que não haverá migration de localização.
- [x] Classificar as ocorrências literais encontradas.
- [x] Separar mensagens de usuário de mensagens técnicas.
- [x] Revisar e aprovar a política de entrega de cada e-mail.
- [x] Identificar exceções permitidas para o futuro teste arquitetural.

### Validação

- [x] Revisar o ADR.
- [x] Confirmar que nenhuma regra de negócio foi alterada.
- [x] Confirmar que nenhuma migration foi criada.

### Critério de conclusão

Responsabilidades, classificação de mensagens e política de entrega aprovadas.

## Fase 1: fundação dos resources por módulo

### Objetivo

Preparar a estrutura de resources sem realizar uma migração transversal única.

### Implementação

- [x] Criar a estrutura de resources no AtronTracker Application.
- [x] Criar a estrutura de resources no AtronStock Application.
- [x] Criar `TarefaResource`.
- [x] Criar `NotificacaoInternaResource`.
- [x] Criar `PlanejamentoCustoResource`.
- [x] Criar `PerfilDeAcessoResource`.
- [x] Criar `ModuloResource`.
- [x] Definir a convenção de nomes das chaves.
- [x] Padronizar mensagens parametrizadas.
- [x] Manter os resources atuais enquanto ainda houver consumidores.
- [x] Não mover todos os resources compartilhados nesta fase.

Convenção inicial:

```text
Erro_TarefaNaoEncontrada
Erro_TarefaNaoPodeSerAssumida
Mensagem_TarefaCriada
Aviso_EmailNotificacaoNaoEnviado
```

### Validação

- [x] Testar carregamento dos resources.
- [x] Testar formatação de mensagens parametrizadas.
- [x] Conferir acentuação.
- [x] Compilar os projetos envolvidos.

### Critério de conclusão

Novos resources disponíveis sem mudança funcional.

## Fase 2: infraestrutura de templates de e-mail

### Objetivo

Criar o mecanismo comum de renderização antes de migrar fluxos reais.

### Implementação

- [x] Criar o diretório de templates pt-BR.
- [x] Criar um template base da identidade Atron.
- [x] Configurar os HTML como recursos incorporados ao assembly.
- [x] Criar o ponto de encaixe do renderizador.
- [x] Implementar carregamento de template.
- [x] Implementar validação dos campos obrigatórios.
- [x] Codificar dados dinâmicos para HTML.
- [x] Validar URLs usadas nos templates.
- [x] Criar modelos tipados de renderização.
- [x] Preservar `IEmailService` sem responsabilidade de composição.
- [x] Não migrar fluxos de usuário ou tarefa nesta fase.

### Validação

- [x] Testar carregamento de um template incorporado.
- [x] Testar campos obrigatórios.
- [x] Testar nome e conteúdo contendo caracteres HTML.
- [x] Testar template inexistente.
- [x] Testar geração de `EmailRequest` sem envio real.
- [x] Compilar o backend.

### Critério de conclusão

Um `EmailRequest` pode ser produzido a partir de template e modelo tipado sem acesso a SMTP ou Brevo.

## Fase 3: fatia vertical de Tarefas

### Objetivo

Validar resources, notificação interna e template de e-mail no mesmo fluxo.

### Implementação

- [x] Migrar mensagens de `TarefaService`.
- [x] Migrar mensagens de `TarefaPreparacaoService`.
- [x] Migrar mensagens de `TarefaValidador`.
- [x] Migrar mensagens de `NotificacaoInternaService`.
- [x] Revisar mensagens dos controllers relacionados.
- [x] Montar notificações internas com `TarefaResource`.
- [x] Persistir título e mensagem finais sem alterar a entidade.
- [x] Extrair o HTML de `TarefaNotificacaoService`.
- [x] Mover o assunto para resource.
- [x] Preservar a preferência de notificação do usuário.
- [x] Preservar o envio somente durante criação ou atribuição.
- [x] Preservar falha de e-mail como aviso, sem desfazer a tarefa.

### Validação

- [x] Testar preparação da tarefa.
- [x] Testar criação da notificação interna.
- [x] Testar conteúdo persistido.
- [x] Testar template do e-mail de tarefa.
- [x] Testar preferência ativada e desativada.
- [x] Testar falha consultiva do envio.
- [x] Compilar o WebApi.
- [x] Verificar o payload consumido pelo Angular.

### Critério de conclusão

O fluxo migrado de Tarefas não contém textos observáveis chumbados e mantém o comportamento atual.

## Fase 4: Acesso e Usuários

### Objetivo

Migrar o maior conjunto de mensagens e e-mails duplicados.

### Fluxos incluídos

- cadastro público;
- confirmação de e-mail;
- reenvio de confirmação;
- recuperação de senha;
- primeiro acesso;
- alteração de e-mail;
- reativação de conta;
- confirmação concluída.

### Implementação

- [x] Consolidar textos em `AuthResource`, `UsuarioResource` e resources de e-mail.
- [x] Criar os templates HTML de acesso.
- [x] Reutilizar o template base.
- [x] Criar ou aprofundar o compositor de e-mails de acesso.
- [x] Remover métodos privados que retornam HTML.
- [x] Eliminar duplicação entre cadastro e reenvio.
- [x] Migrar assuntos chumbados.
- [x] Aplicar a política de entrega aprovada.
- [x] Remover capturas silenciosas de exceção quando o fluxo precisar informar falha.
- [x] Preservar normalização de código no limite de entrada da aplicação.

### Ponto de decisão

- [x] Definir o comportamento do primeiro acesso quando o usuário é criado, mas o e-mail falha.
- [x] Definir o comportamento do cadastro público quando o e-mail inicial falha.

### Validação

- [x] Executar os testes de `RegistroUsuarioService`.
- [x] Testar cada template.
- [x] Testar sucesso e falha do transporte.
- [x] Verificar que tokens, códigos e segredos não aparecem em erros indevidos.
- [x] Compilar o WebApi.

### Critério de conclusão

Nenhum corpo HTML dos fluxos incluídos permanece dentro de serviço ou caso de uso.

## Fase 5: módulos restantes do AtronTracker

### Objetivo

Completar a cobertura funcional do Tracker.

### Ordem

1. Planejamento de Custos.
2. Cargo e Departamento.
3. Perfil de Acesso.
4. Módulos e permissões.
5. Validadores de login e token.
6. Mensagens compartilhadas de controllers.

### Implementação

- [x] Migrar erros.
- [x] Migrar avisos.
- [x] Migrar mensagens de sucesso.
- [x] Preservar descrições de histórico e auditoria.
- [x] Remover duplicações entre controller, serviço e validador.
- [x] Manter mensagens técnicas fora dos resources apresentados ao usuário.

### Validação

- [x] Executar testes focados por módulo.
- [x] Executar os testes de Planejamento de Custos.
- [x] Compilar depois de cada módulo migrado.
- [x] Não acumular todos os módulos em um único lote sem validação intermediária.

### Critério de conclusão

Nenhum `Resultado` observável do AtronTracker utiliza literal fora das exceções documentadas.

## Fase 6: AtronStock, AtronEmail, Auditoria e Framework.Shared

### Objetivo

Fechar a cobertura dos demais projetos do backend.

### Implementação

- [x] Completar `ClienteResource`.
- [x] Completar `FornecedorResource`.
- [x] Revisar Categoria e Produto.
- [x] Centralizar a mensagem de divergência entre rota e corpo.
- [x] Revisar respostas de diagnóstico do AtronEmail.
- [x] Confirmar a cobertura da Auditoria.
- [x] Retirar de `EmailResource` os domínios de provedores.
- [x] Retirar de `EmailResource` os hosts SMTP.
- [x] Separar nomes e configurações técnicas do transporte.
- [x] Manter em resource somente texto observável.

### Validação

- [ ] Executar testes dos validadores do AtronStock.
- [ ] Testar identificação de provedor.
- [ ] Testar diagnóstico do AtronEmail.
- [x] Compilar os projetos envolvidos.

### Critério de conclusão

Todos os módulos analisados passaram por classificação, migração ou exceção documentada.

## Fase 7: limpeza arquitetural

### Objetivo

Remover caminhos concorrentes, duplicações e abstrações sem uso.

### Implementação

- [x] Verificar consumidores de `IEmailNotificationService` novamente.
- [x] Remover `EmailNotificationService` se continuar sem consumidores.
- [x] Confirmar que não existe uso real que exija redefinir sua responsabilidade.
- [x] Remover métodos antigos de construção de HTML.
- [x] Remover resources sem referências.
- [x] Revisar chaves duplicadas e manter somente as reutilizações transversais intencionais.
- [x] Revisar nomes e acentuação.
- [x] Confirmar que arquivos `.Designer.cs` não foram editados manualmente.

### Validação

- [x] Buscar referências antes de cada remoção.
- [x] Executar os testes de composição.
- [x] Executar os testes de transporte.
- [x] Compilar a solução.
- [x] Confirmar que SMTP e Brevo continuam atrás de `IEmailService`.

### Critério de conclusão

Existe um caminho para composição de cada contexto e um único ponto de encaixe para transporte.

## Fase 8: proteção contra regressões

### Objetivo

Impedir que novos textos observáveis ou novos corpos HTML voltem a ser escritos dentro dos fluxos.

### Implementação

- [x] Criar teste arquitetural ou verificador de literais.
- [x] Detectar `Resultado.Falha("...")`.
- [x] Detectar `AdicionarErro("...")`.
- [x] Detectar `AdicionarAviso("...")`.
- [x] Detectar assuntos literais em `EmailRequest`.
- [x] Detectar HTML dentro de serviços e casos de uso.
- [x] Configurar exceções documentadas para testes, templates e mensagens técnicas.
- [x] Garantir teste mínimo para cada template.
- [x] Confirmar que não surgiu terminologia de domínio que exija atualização de `CONTEXT.md`.
- [x] Atualizar a documentação operacional dos e-mails.

### Validação final

- [x] Executar `dotnet test`.
- [x] Executar `dotnet build AtronPlatform.sln`.
- [x] Usar `OutDir` alternativo para isolar os artefatos da validação final.
- [x] Não executar `npm run build`, pois nenhum contrato ou tratamento no Angular foi alterado.
- [x] Repetir a varredura de literais.
- [x] Confirmar que nenhuma migration foi criada.
- [x] Testar o envio Brevo em ambiente controlado com `HttpMessageHandler` falso, validando o `htmlContent` sem envio externo.

### Critério de conclusão

Cobertura concluída, documentação atualizada e proteção contra regressão funcionando.

## 6. Estratégia de testes

### 6.1 Resources

- carregamento da chave;
- formatação com parâmetros;
- acentuação;
- chave utilizada no fluxo correto;
- ausência de literal observável no chamador migrado.

### 6.2 Templates

- carregamento a partir do assembly;
- presença dos campos obrigatórios;
- codificação de valores dinâmicos;
- formatação de datas;
- validade básica do HTML;
- geração de links;
- renderização determinística para o mesmo modelo.

### 6.3 Compositores

- assunto correto;
- template correto;
- destinatário correto;
- modelo correto;
- cultura pt-BR;
- retorno de `EmailRequest` pronto para transporte.

### 6.4 Transporte

- SMTP continua usando `IsBodyHtml`;
- Brevo continua enviando `HtmlContent`;
- falhas do provedor retornam `Resultado`;
- nenhum compositor depende diretamente de SMTP ou Brevo.

### 6.5 Notificações internas

- título final correto;
- mensagem final correta;
- tipo de evento preservado;
- URL de destino preservada;
- estado de leitura preservado;
- nenhum dado de localização adicional persistido.

## 7. Riscos e cuidados

### 7.1 Alteração de comportamento durante migração textual

Trocar um literal por resource não deve modificar regra, status HTTP ou nível da mensagem.

Mitigação:

- migrar uma fatia por vez;
- manter testes focados;
- comparar payload antes e depois.

### 7.2 Exposição de dados em HTML

Nome, título, conteúdo de tarefa e outros valores podem conter caracteres interpretados como HTML.

Mitigação:

- codificar valores dinâmicos;
- não permitir HTML arbitrário nos modelos;
- validar URLs.

### 7.3 Falha de entrega mascarada

Capturas silenciosas podem fazer a API informar sucesso sem que o e-mail tenha sido enviado.

Mitigação:

- política explícita por fluxo;
- retorno consistente de `Resultado`;
- teste de falha do adaptador.

### 7.4 Mudança transversal excessiva

Migrar todos os módulos e todos os templates em um único lote aumenta o risco de regressão e dificulta a revisão.

Mitigação:

- parar ao final de cada fase;
- compilar e testar por módulo;
- não avançar com falha pendente.

### 7.5 Resources compartilhados conhecendo regras dos módulos

Manter `TarefaResource` ou `PlanejamentoCustoResource` dentro do projeto compartilhado reduz a localidade e cria dependência conceitual indevida.

Mitigação:

- resources específicos ficam no projeto proprietário;
- compartilhado recebe somente texto transversal.

## 8. Fora do escopo inicial

- resources em outros idiomas;
- seleção de cultura por usuário;
- seleção de cultura por `Accept-Language`;
- operador de tradução de notificações persistidas;
- alteração do esquema de `NotificacaoInterna` para localização;
- fila de e-mail;
- retentativa automática;
- armazenamento de templates em banco;
- editor visual de e-mail;
- alteração dos provedores SMTP ou Brevo.

## 9. Protocolo de execução

Para cada fase:

1. Confirmar o escopo da fase.
2. Registrar o estado inicial do workspace.
3. Implementar somente os itens da fase.
4. Executar as validações previstas.
5. Apresentar arquivos alterados e resultados dos testes.
6. Registrar riscos ou pendências.
7. Parar e aguardar aprovação antes da fase seguinte.

Avisos preexistentes podem ser registrados como ruído conhecido, mas não devem esconder erros reais.

## 10. Ordem de execução aprovada

```text
Fase 0: contrato e inventário
    -> Fase 1: fundação de resources
    -> Fase 2: infraestrutura de templates
    -> Fase 3: Tarefas
    -> Fase 4: Acesso e Usuários
    -> Fase 5: módulos restantes do AtronTracker
    -> Fase 6: AtronStock, AtronEmail, Auditoria e Shared
    -> Fase 7: limpeza arquitetural
    -> Fase 8: proteção contra regressões
```

O primeiro passo de implementação é a Fase 0. Nenhuma fase posterior deve ser iniciada antes da aprovação do ADR e da política de entrega dos e-mails.
