# Padronizar resources, notificacoes internas e templates de e-mail

## Status

Aceito em 17/07/2026.

## Contexto

O backend do Atron possui mensagens observaveis definidas em resources e tambem literais espalhados por controllers, servicos, casos de uso e validadores. A composicao de e-mails segue situacao semelhante: o transporte compartilhado existe por meio de `IEmailService`, mas assuntos e corpos HTML ainda sao construidos dentro dos fluxos de aplicacao.

Essa distribuicao dificulta manter a linguagem do produto, comparar o comportamento antes e depois de uma alteracao e impedir que regras de composicao conhecam detalhes de SMTP ou Brevo. A central de notificacoes internas tambem precisa de um contrato explicito sobre qual texto deve ser persistido.

Esta decisao cobre a primeira implementacao em pt-BR. Internacionalizacao por usuario ou requisicao, fila de e-mail e retentativa automatica permanecem fora do escopo.

## Decisao

### Resources

- A implementacao inicial usara somente resources em pt-BR.
- Nao havera selecao de cultura por usuario, `Accept-Language` ou configuracao equivalente nesta rodada.
- Cada modulo sera proprietario das mensagens do proprio dominio.
- `Framework.Shared` manterá somente mensagens realmente transversais.
- Configuracoes, nomes de provedores, hosts, chaves, constantes de protocolo e mensagens exclusivamente tecnicas nao serao tratados como texto localizavel de produto.
- Arquivos `.Designer.cs` continuarao gerados pela ferramenta e nao serao editados manualmente.
- A substituicao de um literal por resource nao podera alterar nivel da mensagem, payload, status HTTP ou regra de negocio.

Convencao de chaves:

- usar o prefixo que representa o papel observavel: `Erro_`, `Aviso_`, `Mensagem_` ou `Titulo_`;
- completar a chave com uma descricao semantica em PascalCase, como `Erro_TarefaNaoEncontrada`;
- nao repetir o nome do modulo quando a propria classe resource ja delimitar o contexto;
- usar placeholders posicionais `{0}`, `{1}` e seguintes para valores dinamicos;
- formatar os placeholders explicitamente com a cultura pt-BR no chamador ou compositor;
- nao concatenar fragmentos de resource para montar uma frase observavel.

### Notificacoes internas

- A notificacao interna persistira titulo e mensagem finais em pt-BR.
- Nome de resource, chave, parametros de formatacao e cultura nao serao persistidos.
- Nao sera criada migration de localizacao nem alterado o esquema de `NotificacaoInterna`.
- Alteracoes futuras de resources afetarao somente notificacoes novas. O texto historico permanecera imutavel.

### Templates e transporte de e-mail

- Corpos de e-mail serao arquivos HTML incorporados ao assembly proprietario.
- Assuntos e textos observaveis serao obtidos de resources pt-BR.
- Dados dinamicos serao fornecidos por modelos tipados e codificados antes de entrar no HTML.
- URLs serao validadas antes de entrar em atributos de links.
- O compositor selecionara template, assunto e modelo.
- O renderizador carregara o template, validara campos obrigatorios e produzira o HTML final.
- `IEmailService` recebera um `EmailRequest` pronto e continuara responsavel somente pelo transporte.
- SMTP e Brevo permanecerao detalhes de `SharedEmailService`.
- Templates especificos nao serao movidos para `Framework.Shared` apenas para reutilizar a infraestrutura.

### Politica de falha de entrega

As politicas abaixo orientarao as fases que migrarem cada fluxo. Esta Fase 0 nao altera o comportamento atual.

| Fluxo | Politica | Consequencia contratual |
|---|---|---|
| Recuperacao de senha | Obrigatorio | O fluxo informa falha quando o link nao e entregue. |
| Reenvio de confirmacao | Obrigatorio | O fluxo informa falha e nao afirma que a confirmacao foi enviada. |
| Alteracao de e-mail | Obrigatorio | A solicitacao nao informa sucesso quando o link nao e entregue. |
| Codigo de reativacao | Obrigatorio | O fluxo nao informa que o codigo foi enviado quando o transporte falha. |
| Atribuicao de tarefa | Consultivo | A tarefa permanece criada ou atribuida e o resultado recebe um aviso. |
| Confirmacao concluida | Consultivo | A confirmacao permanece concluida mesmo se o e-mail posterior falhar. |
| Primeiro acesso de usuario interno | Obrigatorio | A criacao somente conclui com o convite entregue; preserva a reversao atual em caso de falha. |
| Cadastro publico com confirmacao | Consultivo | A conta permanece criada e o resultado informa o problema, permitindo usar o reenvio sem induzir nova tentativa de cadastro. |
| Diagnostico de e-mail | Obrigatorio | O endpoint de diagnostico falha se o envio que ele testa falhar. |
| Notificacao generica compartilhada | Sem politica operacional | Nao possui consumidor alem do registro de DI; deve ser reavaliada e removida ou redefinida na Fase 7. |

Politica obrigatoria nao implica rollback automatico. Cada fase deve preservar a consistencia do fluxo e definir explicitamente o estado persistido quando o transporte falhar.

### Excecoes para a futura protecao arquitetural

O verificador da Fase 8 podera aceitar somente as seguintes excecoes, identificadas de forma explicita:

- arquivos `.resx` e templates `.html` incorporados;
- testes, dados de teste e assercoes sobre mensagens;
- logs e excecoes exclusivamente tecnicas que nao sejam devolvidas ao cliente;
- descricoes historicas e de auditoria, enquanto preservarem o fato ocorrido e nao forem usadas como resposta de produto;
- constantes de configuracao ou protocolo, como nomes de provedores, hosts, chaves de configuracao e niveis de notificacao;
- `Framework/Shared/Application/Services/Email/SharedEmailService.cs`, somente para falhas tecnicas do adaptador SMTP ou Brevo, que nao sao respostas de produto;
- codigo gerado, migrations, snapshots, `bin` e `obj`;
- `PlanejamentoCustoRelatorioHtmlMontador`, por produzir deliberadamente um relatorio HTML que nao e template de e-mail. Seus textos observaveis continuam sujeitos a resources;

Uma excecao nao autoriza esconder texto observavel dentro de uma categoria tecnica. Se o texto chegar ao `Resultado`, resposta HTTP, notificacao interna ou e-mail de produto, ele devera ser classificado e migrado.

## Alternativas consideradas

### Manter todas as mensagens no projeto compartilhado

Rejeitada porque faria `Framework.Shared` conhecer conceitos como Tarefa e Planejamento de Custo e reduziria a localidade das regras.

### Persistir chave e parametros das notificacoes internas

Rejeitada porque exigiria cultura de exibicao, mudanca de esquema e tratamento retroativo sem requisito atual de internacionalizacao.

### Construir HTML dentro dos casos de uso

Rejeitada porque mistura decisao de negocio, composicao visual e transporte, alem de dificultar codificacao segura de valores dinamicos.

### Tratar toda falha de e-mail da mesma forma

Rejeitada porque alguns e-mails habilitam o proximo passo do usuario, enquanto outros sao apenas avisos complementares de uma operacao ja concluida.

## Consequencias

- A migracao ocorrera por modulo e por fatia vertical, com comparacao de payload antes e depois.
- O backend continuara entregando texto final em pt-BR ao Angular.
- Nao havera migration de localizacao.
- Os templates precisarao de testes de carregamento, campos obrigatorios, codificacao HTML e URLs.
- Os compositores precisarao de testes de assunto, template, destinatario e modelo.
- Fluxos que hoje ignoram o `Resultado` do transporte serao ajustados apenas em suas fases correspondentes.
- A Fase 8 devera transformar a lista de excecoes em allowlist explicita, evitando exclusoes amplas por diretorio.

## Validacao

- Conferir o inventario em `docs/inventario-resources-e-templates-email.md`.
- Confirmar que esta decisao nao modifica codigo de aplicacao nem regra de negocio.
- Confirmar que nenhuma migration foi criada na Fase 0.
- Aprovar as politicas de entrega antes de iniciar a Fase 1.
