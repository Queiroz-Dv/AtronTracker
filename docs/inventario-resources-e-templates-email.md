# Inventario de mensagens e templates de e-mail

## Finalidade

Este inventario e a linha de base classificavel da Fase 0 do plano de resources e templates de e-mail. Ele separa texto observavel de texto tecnico e registra o destino esperado de cada grupo antes de qualquer migracao.

A contagem e diagnostica, nao um criterio isolado de qualidade. Um mesmo literal pode gerar mais de uma ocorrencia em uma busca textual, e um corpo HTML multilinha e contado como um template, nao como cada linha de texto.

## Escopo e criterio reproduzivel

Foram analisados arquivos `.cs` de:

- `AtronPlatform/Modules/Tracker/Application` e controllers de Tracker em `AtronPlatform/WebApi`;
- `AtronPlatform/Modules/Stock/Application` e controllers de Stock em
  `AtronPlatform/WebApi`;
- capacidades de e-mail e Auditoria em `Framework/Shared`;
- `Framework/Shared/Application` e `Framework/Shared/Domain`.

Foram excluidos `bin`, `obj`, migrations, snapshots e codigo gerado. A busca direta considera strings literais usadas em:

- `Resultado.Falha` e `Resultado.Sucesso`;
- `AdicionarErro`, `AdicionarAviso` e `AdicionarMensagem`;
- `WithMessage`;
- atribuicoes literais a `Assunto` e `Mensagem`.

Expressao de referencia para a futura verificacao arquitetural:

```regex
(Resultado(?:<[^>]+>)?\.(?:Falha|Sucesso)|Adicionar(?:Erro|Aviso|Mensagem)|WithMessage)\s*\(\s*\$?@?"|Assunto\s*=\s*\$?@?"|Mensagem\s*=\s*\$?@?"
```

No estado registrado em 15/07/2026, a busca encontrou 195 ocorrencias diretas em 45 arquivos. O diagnostico inicial do plano registrava 199 candidatas nos mesmos 45 arquivos. A diferenca decorre da evolucao do workspace e da formalizacao do detector; a linha de base executavel passa a ser a busca descrita acima.

| Area | Ocorrencias diretas | Arquivos | Classificacao predominante |
|---|---:|---:|---|
| AtronTracker Application | 137 | 28 | Produto, validacao, notificacao interna e e-mail |
| AtronPlatform WebApi, controllers do Tracker | 6 | 4 | Resposta transversal de controller |
| AtronStock Application | 27 | 2 | Validacao de produto |
| AtronPlatform WebApi, controllers de Stock | 2 | 2 | Resposta transversal de controller |
| AtronEmail | 2 | 1 | Diagnostico tecnico observavel |
| Auditoria transversal | 0 | 0 | Sem candidata direta |
| Framework Shared Application | 9 | 4 | Diagnostico e transporte de e-mail |
| Framework Shared Domain | 12 | 4 | Mensagens genericas e constantes de protocolo |
| Total | 195 | 45 | A classificar e migrar por fase |

## Classificacao por destino

### Mensagens de produto e validacao

Devem migrar para resource do modulo proprietario sem alterar nivel, payload ou status HTTP.

| Grupo | Arquivos ou componentes | Fase de destino |
|---|---|---:|
| Tarefas | `TarefaService`, `TarefaPreparacaoService`, `TarefaValidador`, `TarefaMessageValidation` | 3 |
| Notificacoes internas | `NotificacaoInternaService` | 3 |
| Acesso e registro | `RegistroUsuarioService`, `LoginService`, `UsuarioRegistroValidador`, validacoes de login e token | 4 |
| Usuarios | Casos de uso em `Application/UseCases/Usuario`, validadores e servicos de usuario | 4 |
| Planejamento de Custos | Preparacao, detalhes por cargo, identidade, policy, service e validador | 5 |
| Cargo e Departamento | Servicos, validadores e controllers correspondentes | 5 |
| Perfil de Acesso e Modulos | Servicos, validacoes e controllers correspondentes | 5 |
| AtronStock | `ClienteValidador`, `FornecedorValidador` e controllers com divergencia rota/corpo | 6 |
| Framework generico | `Notifiable`, `NotificationBag` e `Resultado` | 6 |

Os controllers nao devem duplicar uma mensagem ja produzida pelo caso de uso. A divergencia entre identificador da rota e do corpo e transversal e pode usar resource compartilhado.

### Notificacoes internas

Os literais de `NotificacaoInternaService` e os textos montados a partir de eventos de Tarefa sao texto observavel de produto. O destino e `TarefaResource` ou `NotificacaoInternaResource`, conforme a propriedade da mensagem. Titulo e mensagem continuarao finalizados em pt-BR antes da persistencia.

Nao sao candidatos a migration de banco, chave de traducao ou cultura persistida.

### Assuntos e corpos de e-mail

A varredura manual confirmou 11 corpos HTML em 8 classes C#:

| Classe | Corpos | Fluxo | Destino previsto |
|---|---:|---|---|
| `RegistroUsuarioService` | 3 | Cadastro publico, recuperacao de senha e confirmacao concluida | Fase 4 |
| `CriarUsuario` | 1 | Primeiro acesso interno | Fase 4 |
| `AlterarEmail` | 1 | Alteracao de e-mail | Fase 4 |
| `ReenviarConfirmacaoEmail` | 1 | Reenvio de confirmacao | Fase 4 |
| `SolicitarReativacao` | 1 | Codigo de reativacao | Fase 4 |
| `TarefaNotificacaoService` | 1 | Atribuicao de tarefa | Fase 3 |
| `EmailDiagnosticService` | 1 | Diagnostico de transporte | Migrado na Fase 7 |

`PlanejamentoCustoRelatorioHtmlMontador` tambem produz HTML, mas sua responsabilidade e montar relatorio de impressao, nao e-mail. Ele e excecao ao detector de corpo de e-mail, sem deixar de estar sujeito a resources para seus textos observaveis.

### Historico e auditoria

Descricoes que registram um fato ocorrido, como a descricao de criacao em `CriarUsuario`, devem permanecer semanticamente estaveis. Elas nao devem ser confundidas com mensagem de sucesso ao usuario.

Tratamento:

- preservar o texto historico durante as migracoes funcionais;
- nao reutilizar resource de resposta de produto para auditoria apenas porque a frase e parecida;
- revisar separadamente na Fase 5 ou 6 se houver necessidade de padronizacao;
- permitir literal no futuro verificador somente em ponto identificado de historico ou auditoria.

### Mensagens tecnicas

Mensagens de configuracao, diagnostico interno, logs e falhas de adaptador nao pertencem automaticamente a resources de produto.

Principais pontos:

- `EmailDiagnosticService` e `EmailStatusResponse` possuem texto tecnico observavel pelo endpoint interno;
- `SharedEmailService` possui falhas de validacao e transporte;
- excecoes de SMTP, Brevo, HTTP e configuracao devem preservar detalhe tecnico adequado sem expor segredo;
- um texto tecnico que for devolvido diretamente ao usuario final deve ser reclassificado como observavel.

### Configuracao e protocolo

Nao devem virar resources:

- nomes `Brevo`, `Gmail`, `Outlook`, `Yahoo` e equivalentes;
- hosts, URLs base, portas e chaves de configuracao;
- nomes de secoes como `EmailSettings:FromEmail`;
- niveis de notificacao de `ENotificationType`;
- nomes de headers, claims, rotas e constantes de protocolo.

## Politica de entrega classificada

| Fluxo | Comportamento atual relevante | Politica aprovada pelo ADR proposto |
|---|---|---|
| Recuperacao de senha | Propaga `Resultado` de falha | Obrigatorio |
| Reenvio de confirmacao | Propaga retorno e excecao | Obrigatorio |
| Alteracao de e-mail | Ignora retorno e captura excecao silenciosamente | Obrigatorio |
| Codigo de reativacao | Ignora retorno e captura excecao silenciosamente | Obrigatorio |
| Atribuicao de tarefa | Converte falha em aviso | Consultivo |
| Confirmacao concluida | Ignora falha posterior a confirmacao | Consultivo |
| Primeiro acesso interno | Reverte usuario e conta quando o convite falha | Obrigatorio |
| Cadastro publico | Persiste conta e ignora falha de envio | Consultivo com aviso explicito |
| Diagnostico | Retorna erro quando transporte falha | Obrigatorio |
| Notificacao generica | Sem consumidor identificado | Sem politica operacional ate a Fase 7 |

## Allowlist inicial para a Fase 8

Excecoes permanentes ou estruturais:

- resources e templates HTML;
- testes e fixtures;
- logs e excecoes exclusivamente tecnicas;
- historico e auditoria identificados;
- configuracao e protocolo;
- codigo gerado, migrations, snapshots, `bin` e `obj`;
- `PlanejamentoCustoRelatorioHtmlMontador` apenas para a regra que proibe HTML de e-mail dentro de servicos.

Cada item da allowlist futura devera apontar para arquivo e motivo. Nao sera aceita exclusao ampla de um projeto inteiro.

## Estado da Fase 0

- Resources e literais classificados por responsabilidade e fase de destino.
- Mensagens de usuario separadas de mensagens tecnicas, historico e protocolo.
- Onze corpos HTML localizados e associados aos respectivos fluxos.
- Politica de entrega proposta para todos os fluxos encontrados.
- Excecoes iniciais identificadas para o teste arquitetural.
- Nenhuma mudanca funcional ou migration faz parte deste inventario.

A conclusao da Fase 0 depende da revisao e aprovacao do ADR e das politicas de entrega.
