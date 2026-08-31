# Fronteiras e propriedade da Atron Platform

## Objetivo

Este documento registra a propriedade lógica das capacidades, contratos, dados
e migrations do Atron após a Fase 9 do ADR 0007. Os hosts transitórios de
Tracker, Stock e Auditoria foram removidos; o host neutro compõe os módulos por
suas próprias extensões de registro.

O núcleo continua sendo um único processo. Centralizar uma capacidade na
plataforma não significa criar um novo serviço ou deploy.

## Matriz de capacidades

| Capacidade | Propriedade lógica | Local físico atual | Regra de consumo |
|---|---|---|---|
| Autenticação e sessão | Plataforma | Tracker e `Shared`, por compatibilidade | O host configura JWT uma vez; módulos não configuram autenticação própria. |
| Usuários e identidade | Plataforma | Domain e Infrastructure do Tracker | Outros módulos usam contratos transversais, nunca entidades ou repositórios do Tracker. |
| Perfis e catálogo de módulos | Plataforma | Tracker, transitoriamente | Módulos publicam códigos por `ModuloPolicies`; não consultam `IModuloService`. |
| Departamentos, cargos e hierarquias | Plataforma | Tracker, transitoriamente | São estrutura funcional compartilhável, não uma extensão do domínio de tarefas. |
| Tarefas e planejamento de custos | Tracker | `AtronPlatform/Modules/Tracker` | Somente o Tracker altera suas regras e tabelas. |
| Estoque e suprimentos | Stock | `AtronPlatform/Modules/Stock` | Nenhum consumidor usa entidades, repositórios ou `StockDbContext` diretamente. |
| Auditoria e histórico | Plataforma, capacidade transversal in-process | `Framework/Shared` | Produtores enviam contratos de auditoria; somente a capacidade acessa `SharedDbContext`. |
| Notificações internas | Plataforma, capacidade transversal in-process | `Framework/AtronNotificacoes` | Produtores publicam contratos; não compartilham DbContext ou entidades. |
| Cache, e-mail e segurança técnica | Plataforma, capacidades transversais in-process | `Framework/Shared` | São compostas pelo host e não representam módulos de produto. |

## Identidade, acesso e estrutura funcional

Usuários, autenticação, perfis, catálogo de módulos, departamentos, cargos e
hierarquias pertencem conceitualmente à plataforma. A implementação continua
fisicamente no Tracker durante a transição porque movê-la agora misturaria
mudança de propriedade, migrations, autenticação e comportamento.

A preparação para a centralização estabelece estas fronteiras:

- `Shared.Authorization.ModuloPolicies` é o contrato estável dos identificadores
  usados na autorização por módulo;
- `IUserAccessor` é o contrato transversal para identificar o usuário atual;
- o host neutro configura JWT, CORS e o pipeline de autenticação;
- `DynamicModuloPolicyProvider` e `ModuloHandler` continuam no Tracker como
  implementações transitórias do contrato de acesso;
- módulos de produto não podem referenciar `IModuloService`, entidades de
  perfil, repositórios de usuário ou `AtronDbContext`;
- uma futura centralização troca a implementação e a persistência sem alterar
  os atributos de autorização dos módulos consumidores.

Essa capacidade permanece no mesmo processo. Qualquer proposta de serviço
independente para identidade ou estrutura funcional exigirá ADR próprio.

## Direção proposta: workspaces e escopo de acesso

Esta seção registra uma proposta em avaliação, não uma decisão aceita nem uma
alteração já implementada.

O workspace deverá ser a fronteira lógica de dados e acesso para os fluxos de
uso pessoal, agência e empresa. `Usuario` continuará representando a identidade
global da pessoa. `Empresa` representará os dados formais do negócio quando o
workspace tiver esse perfil, sem ser obrigatória para todo workspace.

O pertencimento será representado por um vínculo de usuário com workspace. O
vínculo deverá ser a origem do contexto ativo, do estado do membro, da
hierarquia operacional e do escopo do perfil de acesso. O usuário poderá ter
perfis diferentes em workspaces diferentes.

O RBAC existente baseado em `PerfilDeAcesso`, `PerfilDeAcessoModulo` e
`ModuloPolicies` permanece como mecanismo de autorização. A evolução prevista é
associar a avaliação desse RBAC ao workspace ativo, sem criar um segundo modelo
de papéis. A policy de módulo não substitui a validação de pertencimento ao
workspace.

Os onboardings previstos são:

- cadastro pessoal ou de agência, que cria o workspace e o primeiro membro
  proprietário;
- cadastro empresarial, que cria o workspace, os dados formais de `Empresa` e o
  primeiro membro proprietário;
- convite de membro, que permite concluir o cadastro de um novo usuário ou
  associar um usuário existente ao workspace.

### Contrato do cadastro inicial do workspace

O workspace inicial é criado durante o cadastro do usuário, não no primeiro
acesso. A entrada do cadastro será composta pelos dados atuais do usuário e por
um objeto `workspace` com os seguintes campos:

- `nome`: obrigatório para qualquer tipo de workspace, com no máximo 150
  caracteres;
- `tipo`: enum numérico no contrato HTTP, usando `1` para `Pessoal`, `2` para
  `Agencia` e `3` para `Empresa`;
- `empresa`: ausente para `Pessoal` e `Agencia`, e obrigatório para `Empresa`.

O objeto `empresa` do cadastro empresarial contém `codigo`, `nomeFantasia`,
`endereco`, `numero` e `email`. O status inicial não é informado pelo cliente;
a aplicação cria a empresa como ativa. O código é utilizado exatamente como
informado, sem normalização.

As combinações válidas são:

| Tipo de workspace | Nome do workspace | Dados de empresa |
| --- | --- | --- |
| `Pessoal` | obrigatório | proibidos |
| `Agencia` | obrigatório | proibidos |
| `Empresa` | obrigatório | obrigatórios |

Ao concluir o cadastro, a resposta deverá identificar o usuário criado pelo
seu código e retornar o workspace inicial com `id`, `nome`, `tipo` e
`empresaCodigo` quando empresarial. Os enums continuam numéricos no JSON,
embora sejam persistidos no banco pela descrição textual. Senha, confirmação
de senha e identificadores técnicos do usuário nunca compõem a resposta.

A ativação desse contrato no endpoint `Acesso/Registrar` aceita os tipos
`Pessoal`, `Agencia` e `Empresa`. Os dois primeiros exigem apenas `nome` e
`tipo` no objeto `workspace`, sem dados formais de empresa. O tipo `Empresa`
exige o objeto `empresa`; seu status é definido como `Ativa` pela aplicação e
não faz parte do payload. O endpoint retorna o código do usuário, o workspace
inicial e as mensagens produzidas durante o cadastro.

A operação interna `CriarWorkspaceInicialCase` cria o workspace e seu primeiro
membro no mesmo `SaveChangesAsync`. Nos três tipos de cadastro, o endpoint usa
um escopo transacional que abrange Identity, usuário, empresa quando aplicável,
workspace, membro e código de confirmação; uma resposta de falha não confirma
essas gravações. Para workspace empresarial, a operação interna exige uma
`Empresa` persistida no mesmo onboarding e ainda sem workspace. A operação
também recusa o onboarding inicial quando o usuário já é membro de algum
workspace.

O convite é tratado como credencial temporária de entrada. O backend gera um
identificador aleatório, persiste somente seu hash, define validade de 24 horas
e permite um único consumo. A criação é aceita apenas para workspaces de
`Agencia` ou `Empresa` e exige que o remetente já pertença ao workspace. O
código visível do remetente tem finalidade informativa e não concede acesso.

A consulta pública do convite retorna o workspace de destino, o remetente e a
validade para que a tela de cadastro apresente o contexto conhecido. No
cadastro por convite, o payload informa o identificador em `convite` e não
informa um novo objeto `workspace`. A mesma credencial pode ser aceita por um
usuário já autenticado. Nos dois caminhos, o consumo e a criação do vínculo de
`MembroWorkspace` ocorrem em escopo transacional e o banco faz a marcação
condicional para impedir reutilização concorrente.

Nesta fatia, a autorização para gerar convites está limitada ao pertencimento
ao workspace. A distinção entre proprietário e outros membros depende do
futuro escopo do `PerfilDeAcesso` pelo workspace e não foi antecipada com um
segundo modelo de papéis.

O primeiro passo para o contexto ativo expõe `GET api/Workspace` para o usuário
autenticado. A consulta parte do código obtido no backend, percorre os vínculos
de `MembroWorkspace` e retorna somente `id`, `nome`, `tipo` e `empresaCodigo`.
Uma coleção vazia representa a ausência de vínculos. Essa consulta não escolhe,
persiste nem presume um workspace ativo e não altera sessão ou RBAC.

A seleção transitória usa `POST api/Workspace/selecionar` com `workspaceId`. O
backend obtém o código do usuário pela identidade autenticada e consulta o
workspace junto com o vínculo correspondente. Apenas depois dessa consulta a
seleção é registrada em cookie `HttpOnly`, `Secure` e `SameSite=None`. O cookie
é restrito à sessão do navegador e não cria preferência permanente no usuário.
`Sessao/Info` lê esse cookie e consulta novamente o vínculo entre o usuário
autenticado e o workspace antes de retornar `workspaceAtual`. Uma seleção sem
vínculo deixa de compor a sessão e o cookie correspondente é removido. O dado
não é incluído no cache de acesso indexado somente pelo usuário.

A resolução validada fica em `ObterWorkspaceAtualCase`. `Sessao/Info` obtém o
código do usuário pela identidade autenticada e o fornece ao caso de uso, sem
aceitar identificador de workspace do cliente. Nesta fase, policies e módulos
ainda não usam o workspace atual para autorização ou isolamento.

O Angular consulta essa coleção e apresenta o mesmo seletor no dashboard e no
layout autenticado das rotinas. A sessão retornada pelo backend inicializa a
indicação visual e uma nova seleção aceita atualiza o contexto em memória. O
frontend não persiste o identificador em `localStorage` ou `sessionStorage`.
No logout, o estado em memória e o cookie de seleção são removidos.

A escolha automática de um workspace no login, o escopo dos perfis de acesso
e a migração dos dados dos módulos ainda exigem decisões posteriores. Esses
pontos serão planejados em fatias pequenas antes de qualquer implementação.

## Contratos transversais

### Autorização por módulo

Controllers declaram policies por `ModuloPolicies`. O código da policy é
contrato da plataforma; a resolução de perfis e permissões é uma implementação
interna e substituível.

### Usuário atual

Casos de uso que precisam somente da identidade da requisição usam
`IUserAccessor`. Eles não devem consultar tabelas ou repositórios de identidade
de outro módulo.

### Auditoria

Tracker, Stock e o futuro Sales podem produzir auditoria pelos contratos
`IAuditoriaService`, `IAuditoriaDTO` e `IHistoricoDTO`. Esses contratos não
expõem entidades nem serviços dos módulos produtores.

Datas de criação, alteração e outros marcos de rastreabilidade não devem ser
adicionados às entidades de negócio por padrão. Esses dados pertencem à
Auditoria. Uma data em uma entidade somente é válida quando representar uma
regra operacional própria, com exceção explícita e documentada.

A capacidade possui composição própria por `AddAuditoriaCapability` e controla:

- `AuditoriaService` e `HistoricoService`;
- `AuditoriaRepository` e `HistoricoRepository`;
- `SharedDbContext`;
- tabelas `Auditorias` e `Historicos`;
- sequência `HistoricoSeq`;
- migrations do assembly `Framework.Shared.Migrations`.

O endpoint `GET /Auditoria/{codigoRegistro}/{contexto}` é composto
exclusivamente pelo host neutro e exige autenticação. O antigo executável
`AtronAuditoria` foi removido na Fase 8.

### Notificações

Notificações são compostas in-process pelo host neutro, conforme o ADR 0008.
Seu contrato não dá acesso aos DbContexts dos produtores. A configuração usa o
assembly que contém `NotificacoesDbContext` e suas migrations.
`AddNotificacoesInternasCapability` registra aplicação, persistência, health
check e `INotificacoesInternasPublisher`; Tracker, Stock e futuro Sales não
registram implementações dessa capacidade.

## Propriedade de dados e migrations

| Proprietário | DbContext | Dados principais | Migrations e snapshot |
|---|---|---|---|
| Plataforma, implementação transitória no Tracker | `AtronDbContext` | Identity, usuários, perfis, módulos, departamentos, cargos e relacionamentos | `AtronPlatform/Modules/Tracker/Infrastructure/Migrations` |
| Tracker | `AtronDbContext` | Tarefas, solicitações, movimentações e planejamento de custos | `AtronPlatform/Modules/Tracker/Infrastructure/Migrations` |
| Stock | `StockDbContext` | Clientes, categorias, produtos, fornecedores, estoque, entradas e vendas | `AtronPlatform/Modules/Stock/Infrastructure/Migrations` |
| Auditoria transversal | `SharedDbContext` | Auditorias, históricos e `HistoricoSeq` | `Framework/Shared/Migrations`, assembly `Framework.Shared.Migrations` |
| Notificações transversais | `NotificacoesDbContext` | Notificações internas e estado de leitura | `Framework/AtronNotificacoes/Infrastructure/Migrations` |
| Sales futuro | `SalesDbContext` futuro | Comercial e financeiro | Projeto do próprio módulo, ainda inexistente |

O compartilhamento da mesma instância PostgreSQL não autoriza leitura ou escrita
cruzada. Uma migration somente pode alterar objetos pertencentes ao DbContext
do mesmo proprietário.

## Direção para schemas físicos

Nenhum schema, tabela ou migration de movimentação é criado na Fase 6.

Os nomes `platform` ou `identity`, `tracker`, `stock`, `sales`, `audit` e
`notifications` são candidatos de planejamento, não contratos já implantados.
A adoção futura deverá ocorrer em uma decisão específica com:

1. inventário de tabelas, sequências, chaves estrangeiras e consultas;
2. definição do schema proprietário e da ordem de movimentação;
3. migration produzida pelo DbContext proprietário;
4. validação de compatibilidade com o ambiente publicado;
5. plano de rollback antes da aplicação;
6. observação do deploy antes de mover o próximo conjunto.

Mudança de host e mudança de schema não devem ocorrer na mesma iteração.

## Regra para o futuro Sales

O Sales deverá:

- possuir Domain, Application, Infrastructure, DbContext e migrations próprios;
- usar `ModuloPolicies` para autorização;
- usar `IUserAccessor` quando precisar do usuário atual;
- publicar auditoria pelos contratos transversais;
- publicar notificações pelo contrato de AtronNotificacoes;
- não referenciar Application, Infrastructure, entidades ou DbContexts de
  Tracker ou Stock;
- não acessar tabelas de outro proprietário, mesmo que estejam no mesmo banco;
- ser composto exclusivamente pelo host neutro.

Qualquer necessidade que não caiba nesses contratos deve produzir um contrato
novo e explícito ou uma decisão arquitetural, nunca uma referência direta por
conveniência.
