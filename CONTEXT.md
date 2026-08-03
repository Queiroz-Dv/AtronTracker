# AtronRC

Este contexto descreve a linguagem de domínio usada no AtronRC para alinhar regras de negócio, documentação e implementação entre os módulos da plataforma Atron.

## Product Direction

O Atron é uma plataforma comercial simples para negócios locais e pequenos empresários que desejam centralizar a gestão com custo inicial reduzido e tecnologia suficiente para lidar com fluxos reais da empresa.

O produto deve crescer como uma plataforma modular, não como um conjunto solto de cadastros. A documentação deve diferenciar o que já existe, o que está em planejamento e o que é direção futura. Quando uma regra de negócio virar contrato durável, ela deve aparecer neste contexto ou em documento específico em `docs/`. Quando uma decisão mudar a arquitetura ou o domínio, ela deve ser registrada em ADR.

Os módulos de produto são:

- `Atron Tracker`: gestão interna, estrutura organizacional, usuários, departamentos, cargos, tarefas, planejamento de custos e outras rotinas administrativas; eventos do Tracker podem originar notificações na capacidade transversal.
- `Atron Stock`: cadeia de suprimentos, estoque, patrimônio, bens, movimentações, fornecedores, produtos e rotinas físicas da empresa.
- `Atron Sales`: módulo planejado para comercial e financeiro, incluindo vendas, recebimentos, contas e relatórios comerciais ou financeiros.

O objetivo de evolução do projeto também é formar repertório real de arquitetura de software: escalar, manter, corrigir e documentar um sistema com qualidade e compromisso.

O produto adota um monólito modular. Tracker, Stock, AtronAuditoria, Notificações Internas e o futuro Sales são compostos por um único host neutro da Atron Platform e publicados no mesmo processo. Essa unidade de execução não autoriza mistura de entidades, regras, persistência ou migrations. Os hosts transitórios, o IoC global e o host independente de notificações foram removidos conforme os ADRs 0007 e 0008.

Identidade, autenticação, usuários, perfis de acesso, catálogo de módulos e estrutura funcional pertencem conceitualmente à plataforma. A implementação permanece fisicamente no Tracker durante a transição, mas módulos consumidores devem usar os contratos transversais de usuário atual e autorização por módulo, sem depender de entidades, serviços, repositórios ou DbContext internos do Tracker.

A arquitetura deve priorizar baixo custo operacional e evolução incremental. Microsserviços permanecem como direção condicionada a necessidade comprovada de escala, isolamento de falhas, disponibilidade, equipe ou ciclo de publicação independente. Estudos de sistemas distribuídos podem ocorrer em Docker Compose e protótipos sem impor essa complexidade ao produto principal.

## Language

**Atron**:
Plataforma comercial simples para centralizar a gestão de negócios locais e pequenos empresários, com custo inicial reduzido, arquitetura modular e foco em fluxos reais da empresa.
_Avoid_: Projeto apenas acadêmico, CRUD isolado, ERP complexo de alto custo

**Plataforma comercial simples**:
Produto de gestão com cobertura suficiente para operar rotinas administrativas, operacionais, comerciais e financeiras de uma empresa pequena, sem assumir complexidade ou custo inicial de uma implantação corporativa pesada.
_Avoid_: Sistema experimental sem uso real, ERP completo corporativo, coleção de telas sem unidade

**Negócio local**:
Empresa pequena ou operação regional que precisa controlar pessoas, tarefas, estoque, suprimentos, vendas ou financeiro com processo mais organizado do que planilhas soltas.
_Avoid_: Grande corporação, operação sem rotina de gestão

**Atron Tracker**:
Módulo de gestão interna do Atron. Concentra estrutura organizacional, usuários, departamentos, cargos, perfis de acesso, tarefas, planejamento de custos e rotinas administrativas. Pode produzir notificações, mas não é o proprietário definitivo da capacidade transversal de notificações internas.
_Avoid_: Apenas módulo de tarefas, apenas cadastro de usuário

**Atron Stock**:
Módulo de suprimentos, estoque, patrimônio e bens do Atron. Deve proteger a rastreabilidade de produtos, fornecedores, entradas, saídas, movimentações, saldos e controles físicos ou patrimoniais da empresa.
_Avoid_: Apenas cadastro de produto, controle manual sem rastreabilidade, estoque desconectado da empresa

**Atron Sales**:
Módulo planejado para centralizar o comercial e o financeiro do Atron. Deve ser tratado como direção futura até ter escopo formalizado em documentação própria.
_Avoid_: Funcionalidade já implementada sem evidência, financeiro misturado sem fronteira, venda improvisada dentro de outro módulo

**AtronNotificacoes**:
Capacidade transversal in-process responsável por receber publicações, persistir o conteúdo final, consultar notificações e controlar leitura ou exclusão lógica. É composta pelo `AtronPlatform.WebApi`, mas preserva contrato, aplicação, `NotificacoesDbContext` e migrations próprios. Tracker, Stock e Sales permanecem proprietários do evento e do texto publicado.
_Avoid_: Parte interna do Tracker, regra de negócio do produtor, DbContext compartilhado, chamada HTTP entre módulos do mesmo processo

**AtronAuditoria**:
Capacidade transversal in-process responsável por consultar ou registrar evidências de operações da plataforma. É composta exclusivamente pelo host neutro, possui contratos e persistência no `Shared` e não acessa entidades ou DbContexts dos módulos auditados.
_Avoid_: Microsserviço apenas por possuir WebApi, acesso irrestrito aos DbContexts dos módulos, log técnico genérico, justificativa automática para outro deploy

**Identidade e acesso da plataforma**:
Capacidade central formada por autenticação, usuários, perfis de acesso, catálogo de módulos e autorização. Sua propriedade é da plataforma, mesmo enquanto a implementação permanece fisicamente no Tracker por compatibilidade. Módulos consumidores usam `IUserAccessor` e `ModuloPolicies`, não os internos do Tracker.
_Avoid_: Identidade como domínio exclusivo do Tracker, serviço separado sem necessidade, módulo consultando repositório de perfil, configuração JWT duplicada

**Estrutura funcional da plataforma**:
Capacidade central formada por departamentos, cargos, vínculos, gestores e hierarquias usados por mais de um módulo. A implementação atual permanece no Tracker durante a transição, mas sua propriedade não se limita ao domínio de tarefas.
_Avoid_: Estrutura funcional como detalhe de tarefa, duplicação de departamento por módulo, acesso cruzado ao AtronDbContext

**Contrato de autorização por módulo**:
Vocabulário estável de policies publicado por `Shared.Authorization.ModuloPolicies`. Cada módulo declara o código necessário sem conhecer como perfis, usuários e permissões são persistidos ou resolvidos.
_Avoid_: String de policy montada manualmente, referência ao handler do Tracker, consulta direta a perfil de acesso

**Monólito modular do Atron**:
Arquitetura do produto principal em que Tracker, Stock e Sales são módulos de negócio separados no código, nos dados e nas regras, mas executados e publicados pelo mesmo host da plataforma. A unidade de processo reduz custo e operação sem transformar um módulo em parte interna de outro.
_Avoid_: Projeto único sem fronteiras, Stock dentro do Tracker, monólito distribuído, microsserviços apenas por separação de processo, compartilhamento livre de entidades

**Monólito modular com capacidades transversais**:
Topologia atual do Atron em que módulos de produto e capacidades transversais são compostos pelo host neutro, preservando fronteiras lógicas e propriedade de dados dentro de um único processo.
_Avoid_: Monólito sem fronteiras, serviço por módulo, processo separado sem autonomia, usar distribuição como sinônimo de modularidade

**AtronPlatform.WebApi**:
Host HTTP neutro responsável por compor os módulos principais, configurar autenticação, autorização, CORS, documentação e middlewares e produzir a unidade publicada. Não possui regras de negócio nem representa propriedade do Tracker, Stock ou Sales.
_Avoid_: Novo domínio, módulo de negócio, WebApi do Tracker renomeado sem corrigir dependências, concentrador de regras

**Módulo de produto**:
Fatia vertical da plataforma que possui linguagem, regras, casos de uso, persistência, migrations, resources e testes próprios. Tracker, Stock e Sales são módulos pares e nenhum deles funciona como contêiner conceitual dos demais.
_Avoid_: Pasta de telas, agrupamento sem domínio, módulo subordinado ao Tracker, divisão apenas por tecnologia

**Fronteira de módulo**:
Limite que impede um módulo de usar diretamente entidades, repositórios, DbContexts ou tabelas internas de outro. A colaboração ocorre por contratos de aplicação ou eventos explícitos, mesmo quando a chamada é executada no mesmo processo.
_Avoid_: Acesso cruzado ao banco, referência direta entre domínios, Shared como atalho, integração implícita

**Composição de módulo**:
Registro de dependências mantido pelo módulo proprietário e chamado pelo host neutro, como `AddTrackerModule`, `AddStockModule` ou `AddSalesModule`. A composição expõe o necessário para executar o módulo sem publicar seus detalhes internos para os demais.
_Avoid_: IoC global conhecendo todas as implementações, módulo registrando Infrastructure de outro, service locator

**Propriedade de dados do módulo**:
Responsabilidade exclusiva de um módulo por seu DbContext, entidades, configurações, migrations e tabelas. Os módulos podem usar a mesma instância PostgreSQL, mas não alteram ou consultam diretamente dados pertencentes a outro módulo.
_Avoid_: Banco compartilhado sem proprietário, DbContext de outro módulo, migration transversal acidental

**Serviço independente do Atron**:
Capacidade com contrato, estado, operação e ciclo de falha próprios que possui justificativa explícita para executar fora do monólito modular. Não há serviço independente aprovado na topologia atual; novas extrações exigem evidência e ADR.
_Avoid_: Um Web Service por pasta, microsserviço sem autonomia, processo separado com dependências internas dos módulos

**Microsserviço no Atron**:
Direção arquitetural futura condicionada a escala, isolamento, disponibilidade, propriedade de equipe, contrato estável ou ciclo de publicação independente. Vários processos interdependentes, por si só, não caracterizam a adoção aprovada de microsserviços.
_Avoid_: Objetivo obrigatório, sinônimo de vários containers, solução automática para modularidade, experimento imposto à produção

**Usuário**:
Pessoa cadastrada no sistema que pode receber responsabilidades, acessar módulos e estar vinculada a cargo e departamento.
_Avoid_: Conta, colaborador

**Pessoa remunerada**:
Pessoa vinculada a custos de trabalho ou prestação de serviço que podem ser planejados, acompanhados ou considerados em orçamento. Nem todo usuário do sistema precisa ser uma pessoa remunerada.
_Avoid_: Usuário, conta, colaborador genérico

**Informação remuneratória sensível**:
Dado financeiro associado a uma pessoa remunerada, como salário ou valor equivalente de remuneração, que exige controle de acesso mais restrito do que os dados cadastrais de usuário.
_Avoid_: Campo comum do usuário, dado cadastral simples

**Departamento**:
Área organizacional usada para agrupar cargos, usuários e custos planejados dentro da empresa.
_Avoid_: Centro de custo, setor genérico

**Cargo**:
Função vinculada a um departamento e usada para organizar responsabilidades, usuários e custos planejados por tipo de trabalho.
_Avoid_: Perfil de acesso, permissão, papel técnico

**Estrutura planejada**:
Departamento ou cargo usado por um planejamento de custo atual ou futuro. No MVP, uma estrutura planejada não pode ser removida enquanto estiver vinculada a planejamento, para evitar divergencia entre cadastro e planejamento.
_Avoid_: Remocao livre de departamento, remoção livre de cargo

**Edição de estrutura planejada**:
Alteração permitida em uma estrutura planejada apenas quando não muda sua identidade ou seu vínculo estrutural. No MVP, descrição pode ser ajustada, mas código e vínculo entre cargo e departamento não devem ser alterados quando houver planejamento atual ou futuro.
_Avoid_: Alteração retroativa de código, mover cargo planejado para outro departamento

**Planejamento de custo**:
Definição ou acompanhamento de valores previstos para uma parte da organização em uma competência. No MVP, cada departamento possui no máximo um planejamento por ano; cargos detalham esse planejamento quando existem cargos vinculados ao departamento, mas não são obrigatórios para criar o planejamento.
_Avoid_: Salário do usuário, cadastro salarial

**Identificação do planejamento de custo**:
Identidade do registro financeiro, contábil ou orçamentário criado pelo usuário para o planejamento. No MVP, o planejamento possui identificação composta por id, código obrigatório informado pelo usuário e descrição obrigatória; a descrição não precisa ser única e o código não muda depois da criação.
_Avoid_: Registro anônimo, planejamento sem código, planejamento sem descrição, descrição única obrigatória

**Cadastro de planejamento de custo**:
Fluxo em que o usuário escolhe ano e departamento, informa os limites de custo do departamento e, quando existirem cargos vinculados, visualiza os cargos do departamento para detalhar limites no mesmo planejamento.
_Avoid_: Cadastro por cargo isolado, cadastro salarial

**Edição de planejamento de custo**:
Alteração do corpo de um planejamento existente, preservando sua identidade, departamento e ano. No MVP, a edição permite ajustar descrição, valores e detalhamento por cargo, incluindo informar custos para cargos novos vinculados ao departamento depois da criação do planejamento.
_Avoid_: Troca de departamento, troca de ano, recriação disfarçada

**Planejamento apenas por departamento**:
Forma de planejamento em que o usuário decide explicitamente não detalhar custos por cargo, mesmo quando o departamento possui cargos vinculados. No MVP, essa opção faz parte do cadastro e deixa o detalhamento por cargo desabilitado ou oculto para aquele planejamento; enquanto o planejamento for editável, o usuário pode alternar entre planejamento apenas por departamento e detalhamento por cargo. Ao mudar para apenas por departamento, os detalhes por cargo são removidos definitivamente após confirmação. Cargos novos no departamento não geram pendência enquanto esta opção estiver ativa.
_Avoid_: Esquecimento de preencher cargos, planejamento incompleto acidental

**Cargo não detalhado**:
Cargo exibido no cadastro do planejamento do departamento, mas marcado explicitamente como fora do detalhamento de custos daquele planejamento. No MVP, cargo não detalhado não entra na soma dos cargos e não aparece como linha no relatório geral; o relatório pode exibir um resumo neutro da quantidade de cargos não detalhados.
_Avoid_: Campo vazio sem intenção, erro de cadastro, valor zero

**Cargo detalhado**:
Cargo que participa do detalhamento de custos de um planejamento e possui mínimo e teto informados. No MVP, cargo detalhado sem mínimo ou sem teto é considerado incompleto; ao sair de planejamento apenas por departamento para detalhamento por cargo, cada cargo começa sem valores e exige decisão explícita.
_Avoid_: Cargo parcial, valor isolado de cargo

**Cargo pendente de decisão**:
Cargo vinculado ao departamento de um planejamento detalhado, mas ainda sem decisão explícita de detalhar ou não detalhar. No MVP, cargos novos em departamentos já planejados aparecem como pendentes até receberem mínimo e teto ou serem marcados como não detalhados; se o usuário abrir e salvar o planejamento, pendências de cargo bloqueiam o salvamento. No relatório geral, cargos pendentes aparecem como alerta identificando o cargo.
_Avoid_: Cargo invisível, cargo ignorado automaticamente

**Planejamento consolidado**:
Planejamento de custo marcado como efetivado para uma competência, deixando de ser tratado como versão em edição. No MVP, consolidação de planejamento fica fora do escopo.
_Avoid_: Rascunho, edição comum, histórico técnico

**Cópia de planejamento**:
Ação opcional e explícita em que o usuário usa um planejamento existente como base para preencher um novo planejamento. Um novo planejamento não copia valores anteriores automaticamente; no MVP, a cópia de planejamento fica fora do escopo.
_Avoid_: Herança automática de valores, reaproveitamento implícito

**Módulo de planejamento de custos**:
Módulo responsável por cadastrar e acompanhar planejamentos de custo por departamento e por cargos vinculados ao departamento. O código canônico deste módulo e `PLC`; no MVP, o acesso segue a permissão por módulo e as telas de cadastro e relatório geral são separadas. Ele substitui o módulo de salários como capacidade de negócio, mas não reaproveita o conceito de cadastro salarial.
_Avoid_: Módulo de salários, cadastro de salário, salário de usuário

**Limite de custo planejado**:
Faixa esperada para o custo total planejado de um departamento ou de um cargo dentro de um departamento em uma competência, composta por valor mínimo e valor teto. No MVP, mínimo e teto do departamento são obrigatórios.
_Avoid_: Salário mínimo, salário máximo, custo por pessoa, custo por vaga, valor livre sem competência

**Teto de custo planejado**:
Valor máximo que o custo total planejado pode atingir em um departamento ou cargo dentro de um departamento. No MVP, cada cargo pode ter teto próprio, a soma dos tetos dos cargos não pode ultrapassar o teto do departamento, e teto menor ou igual ao mínimo bloqueia o planejamento.
_Avoid_: Valor sugerido, limite ignoravel

**Mínimo de custo planejado**:
Valor de referência inferior para o custo total planejado em um departamento ou cargo dentro de um departamento. No MVP, cada cargo pode ter mínimo próprio; mínimo maior ou igual ao teto bloqueia o planejamento, enquanto divergências entre a soma dos mínimos dos cargos e os limites do departamento geram alerta.
_Avoid_: Obrigacao de gasto, bloqueio de cadastro

**Competência de planejamento**:
Período ao qual um planejamento de custo se aplica. No MVP, a competência de planejamento é anual; novos planejamentos usam o ano atual ou anos futuros, enquanto anos passados ficam restritos a consulta e não podem ser editados. O relatório geral permite escolher o ano analisado; bimestre, trimestre e semestre são expansões futuras do mesmo conceito.
_Avoid_: Data solta, mes avulso sem planejamento

**Exclusão de planejamento**:
Remoção definitiva de um planejamento de custo ainda alterável, incluindo seus detalhes por cargo. No MVP, planejamentos do ano atual ou futuro podem ser excluídos; anos passados ficam restritos a consulta.
_Avoid_: Exclusão retroativa, limpeza automática

**Custo agregado**:
Valor consolidado usado para acompanhar planejamento ou orçamento sem expor diretamente a informação remuneratória sensível de cada pessoa.
_Avoid_: Lista salarial individual, detalhe de remuneração

**Relatório geral de planejamento**:
Visão analítica do planejamento de custos por ano, exibindo apenas departamentos que possuem planejamento informado para a competência analisada, detalhamento por cargos quando aplicável, valores absolutos, percentual de ocupação do teto e alertas de planejamento. No MVP, o percentual de ocupação usa a soma dos tetos dos cargos detalhados sobre o teto do departamento; planejamento apenas por departamento aparece sem ocupação detalhada.
Plano visual pendente: definir a paleta final do relatório impresso com roxo, branco e preto, usando preto para fontes.
_Avoid_: Listagem simples, tela de cadastro

**Percentual de ocupação do teto**:
Indicador calculado pela soma dos tetos dos cargos detalhados sobre o teto do departamento. No MVP, aparece no cadastro durante o preenchimento e no relatório geral de planejamento; 100% é válido, acima de 100% bloqueia o planejamento.
_Avoid_: Percentual de salário, uso do mínimo como base

**Alerta de planejamento**:
Indicação de que um planejamento de custo informado merece revisão, como divergência de mínimo ou ausência de decisão clara sobre detalhamento por cargo. Departamento sem planejamento não entra no relatório do MVP. Planejamento apenas por departamento e cargo não detalhado são decisões explícitas e aparecem como informação neutra, não como alerta.
_Avoid_: Erro impeditivo, falha técnica

**Detalhamento individual de custo**:
Visão restrita que mostra valores vinculados a pessoas remuneradas quando a análise precisa explicar ou confrontar o custo agregado.
_Avoid_: Dado público do usuário, campo comum de cadastro

**Tarefa**:
Responsabilidade registrada no sistema para acompanhamento. Uma tarefa pode estar vinculada diretamente a um usuário responsável ou a um escopo de responsabilidade por departamento e cargo.
_Avoid_: Atividade, demanda

**Escopo de responsabilidade da tarefa**:
Departamento, cargo ou usuário que delimita quem deve visualizar, assumir ou acompanhar uma tarefa. Uma tarefa pode nascer no escopo de uma estrutura funcional e depois ser atribuída a um usuário responsável; enquanto não tiver usuário responsável, pertence a uma fila de triagem da estrutura.
_Avoid_: Tarefa sempre individual, fila técnica, responsável implícito

**Destino inicial da tarefa**:
Escolha feita na criação da tarefa para definir se ela nasce atribuída a um usuário, vinculada a departamento e cargo, ou direcionada para equipe do gestor. As opções disponíveis devem respeitar as permissões e a responsabilidade de gestão do usuário logado.
No cadastro de tarefas, o destino `Departamento/Cargo` deve ficar disponível apenas para usuário que seja gestor de algum departamento. Usuário sem gestão de departamento deve poder escolher apenas `Usuario` e `Equipe` como destino inicial.
_Avoid_: Destino único obrigatório, tarefa sem escopo, escolha que ignora permissão

**Tarefa estrutural**:
Tarefa cujo destino inicial é uma estrutura funcional em vez de um usuário responsável. Pode ser vinculada apenas a departamento ou a departamento e cargo; não deve existir tarefa vinculada a cargo sem departamento.
_Avoid_: Cargo sem departamento, tarefa sem usuário e sem estrutura, tarefa de todos

**Identificador da tarefa**:
Código numérico sequencial usado pelos usuários para localizar uma tarefa no produto. O identificador é global no sistema, independente de departamento ou cargo, e deve ser exibido de forma amigável com zeros a esquerda quando necessário.
_Avoid_: Id técnico, código por departamento, prefixo obrigatório

**Busca por identificador da tarefa**:
Consulta direta de tarefa pelo identificador numérico, respeitando as permissões do usuário. A busca pode localizar tarefas ativas, finalizadas, individuais ou estruturais, desde que estejam dentro do acesso permitido ao usuário.
_Avoid_: Filtro apenas do quadro atual, listagem geral, acesso irrestrito por código

**Tarefa finalizada**:
Tarefa encerrada para operação e mantida apenas para consulta. Uma tarefa finalizada não pode ser reaberta, atualizada ou removida.
_Avoid_: Reabertura de tarefa, edição pós-finalização, exclusão de histórico

**Tarefa cancelada**:
Tarefa encerrada antes da conclusão operacional e mantida apenas para consulta. O cancelamento substitui a exclusão de tarefa para preservar histórico e deve impedir atualização posterior.
_Avoid_: Exclusão física, apagar tarefa, remover histórico

**Cancelar tarefa**:
Ação em que o usuário responsável encerra uma tarefa ainda não finalizada sem concluí-la. O cancelamento não exige motivo obrigatório separado; quando houver justificativa, ela deve ser descrita no conteúdo da própria tarefa.
_Avoid_: Deletar tarefa, finalizar sem conclusão, ocultar registro, motivo obrigatório separado

**Cancelamento de tarefa em fila de triagem**:
Ação em que um usuário com responsabilidade de gestão sobre o departamento ou cargo da fila encerra uma tarefa estrutural antes de ela ser assumida ou atribuída a um usuário responsável.
_Avoid_: Cancelamento por usuário comum, exclusão de fila, tarefa estrutural sem governança

**Tarefa ativa**:
Tarefa ainda aberta para acompanhamento ou decisão operacional. Em Meu quadro e Equipe, tarefas iniciadas, em atividade, pendentes de aprovação ou entregues são consideradas ativas; tarefas finalizadas e canceladas aparecem somente quando filtradas explicitamente pelo usuário.
_Avoid_: Tarefa finalizada no carregamento padrão, tarefa cancelada no carregamento padrão, histórico misturado com operação

**Fila de triagem de tarefas**:
Conjunto de tarefas ativas sem usuário responsável que ainda não pertencem a Meu quadro. A fila aparece em Disponíveis para qualquer usuário com acesso ao módulo de tarefas; a responsabilidade de gestão determina se a obtenção pode ser direta ou precisa de aprovação.
_Avoid_: Quadro pessoal compartilhado, tarefa disponível invisível, obtenção direta por usuário sem responsabilidade de gestão

**Assumir tarefa**:
Ação em que um usuário com responsabilidade de gestão torna uma tarefa disponível, que não exige aprovação, uma tarefa individual própria. Depois de assumida, a tarefa passa a aparecer em Meu quadro desse usuário.
_Avoid_: Atribuição automática, assunção direta por usuário sem responsabilidade de gestão

**Obter tarefa**:
Ação em que um usuário solicita ou assume uma tarefa exibida em Disponíveis. Usuário sem responsabilidade de gestão sempre gera uma solicitação ao aprovador. Usuário com responsabilidade de gestão pode assumir diretamente quando a tarefa não exige aprovação; quando exige, também gera solicitação. A solicitação, a obtenção direta e a recusa não alteram o estado operacional da tarefa. Na aprovação, uma tarefa em Pendente de aprovação passa para Iniciada e deixa de exigir aprovação para obtenção.
_Avoid_: Atribuição pelo gestor, edição do responsável sem regra, tarefa invisível, mudança de qualquer estado fora da transição aprovada

**Solicitação de obtenção de tarefa**:
Pedido feito por um usuário para receber uma tarefa marcada como pendente de aprovação. A solicitação deve aparecer para o aprovador na visão Solicitações e, quando aprovada, a tarefa passa para Meu quadro do usuário solicitante. Uma tarefa deve ter no máximo uma solicitação de obtenção pendente por vez.
_Avoid_: Aprovação implícita, e-mail como local de decisão, tarefa pendente assumida diretamente, várias solicitações pendentes para a mesma tarefa

**Aprovação para obter tarefa**:
Exigência separada do estado operacional da tarefa. Usuário sem responsabilidade de gestão sempre precisa de aprovação; para gestores, a configuração da tarefa define se a obtenção pode ser direta ou também precisa de aprovação. Essa exigência não deve ser confundida com o estado da tarefa.
_Avoid_: Bloqueio implícito, usar qualquer estado como substituto do flag, manter o flag marcado após aprovar uma tarefa pendente

**Aprovação de obtenção**:
Decisão do aprovador de obtenção de tarefa para aprovar ou recusar uma solicitação. A aprovação válida transforma a tarefa solicitada em tarefa individual do usuário solicitante. Quando a tarefa estiver em Pendente de aprovação, a aprovação também altera seu estado para Iniciada e desmarca a exigência de aprovação para obtenção. Para os demais estados, a aprovação preserva o estado e o flag. A recusa encerra a solicitação e mantém a tarefa disponível conforme seu escopo.
_Avoid_: Aprovação implícita, resposta manual ao e-mail, assumir tarefa sem decisão, alterar estados diferentes de Pendente de aprovação

**Histórico de movimentações da tarefa**:
Registro cronológico e imutável das movimentações relevantes da tarefa. Inclui criação, atualização, obtenção direta, solicitação de obtenção, aprovação e recusa; mudanças de estado como início, entrega e finalização aparecem nos detalhes da atualização. Cada registro preserva o evento, o código e o nome do responsável no momento da ação, a data e hora e as mudanças relevantes. A consulta é paginada, carregada sob demanda no detalhe da tarefa e limitada ao usuário responsável, às equipes e aos gestores autorizados.
_Avoid_: Sobrescrever eventos anteriores, inferir histórico apenas pelo estado atual, expor movimentações fora do escopo de acesso

**Notificação do sistema**:
Aviso interno apresentado ao usuário dentro do produto para informar eventos relevantes. No primeiro escopo, notificações de tarefas informam aprovações ou recusas de solicitações e podem levar o usuário ao detalhe da tarefa quando aplicável; notificações devem poder ser controladas como lidas ou não lidas pelo usuário.
_Avoid_: E-mail obrigatório, alerta sem destino, histórico invisível, notificação sem leitura

**Central de notificações**:
Área do produto onde o usuário acompanha notificações do sistema e acessa os detalhes relacionados ao evento notificado. No primeiro escopo, a central atende eventos do módulo de tarefas e deve permitir abrir a tarefa relacionada quando houver uma; no futuro, tende a ser o canal padrão de notificações internas do produto.
_Avoid_: Caixa de e-mail, alerta temporário sem histórico, lista sem contexto

**Módulo de notificações internas**:
Capacidade transversal planejada que centraliza a publicação, consulta e marcação de leitura das notificações internas do Atron. Deve atender Tracker, Stock e Sales sem depender das entidades de domínio de qualquer um desses módulos. Cada produtor informa destinatário, evento, conteúdo final em pt-BR e destino de navegação; o módulo mantém o histórico e o estado de leitura.
_Avoid_: Controller deslocada sem serviços e dados próprios, chave estrangeira para Tarefa ou outra entidade de módulo produtor, regra de negócio de Tracker dentro da central, e-mail como substituto obrigatório da notificação interna

**Destinatário de notificação**:
Identificador transversal e estável de uma pessoa que pode receber notificações do sistema. É informado pelo módulo produtor e validado pelo mecanismo compartilhado de identidade, sem exigir que a central de notificações consulte o repositório de usuários de Tracker, Stock ou Sales.
_Avoid_: Dependência direta de IUsuarioRepository do Tracker, cópia da entidade Usuario em cada módulo, destinatário implícito por tela

**Publicação de notificação interna**:
Contrato pelo qual um módulo produtor registra uma notificação para um destinatário. O contrato carrega origem, tipo de evento, título, mensagem, URL de destino e referência externa opcional. A publicação não recebe entidades de domínio do produtor e não altera o fluxo de negócio de origem quando a política daquele evento for somente consultiva.
_Avoid_: Passar Tarefa, Pedido ou Produto para a central, montar texto de produto dentro da infraestrutura compartilhada, acoplamento de transação distribuída

**Identificador de notificação interna**:
Chave numérica longa gerada por sequence da persistência própria do módulo de notificações. A sequence pode iniciar em valor aleatório definido uma única vez ao criar o ambiente, mas cada novo identificador é sequencial. O identificador não autoriza consulta nem alteração, que sempre dependem do destinatário autenticado.
_Avoid_: GUID como chave primária, número aleatório por registro sujeito a colisão, usar o identificador como prova de acesso

**Atualização em tempo real**:
Comportamento em que notificações e solicitações relevantes aparecem para o usuário sem depender de recarregamento manual da tela. No módulo de tarefas, esse comportamento apoia aprovações, recusas e acompanhamento de solicitações.
_Avoid_: Consulta manual constante, e-mail como fonte principal, tela desatualizada

**Notificação por e-mail**:
Aviso enviado fora do produto apenas quando o evento exigir comunicação externa ou ação específica por e-mail. A direção do produto é tratar notificações internas como padrão e usar e-mail em casos específicos.
_Avoid_: E-mail para todo evento, e-mail como fonte principal do sistema, duplicação obrigatória de notificação

**Aprovador de obtenção de tarefa**:
Usuário responsável por aprovar ou recusar uma solicitação de obtenção de tarefa. A ordem preferencial é: gestor imediato do solicitante; gestor do departamento da tarefa; gestores dos departamentos vinculados ao solicitante, sem prioridade de negócio entre estes últimos. Códigos repetidos são considerados uma única vez; se nenhum aprovador existir, a solicitação deve ser bloqueada por regra de negócio.
_Avoid_: Solicitação sem aprovador, aprovador aleatório, regra fixa por cargo

**Atribuir tarefa**:
Ação em que um usuário com responsabilidade de gestão escolhe outro usuário dentro do escopo permitido para ser responsável por uma tarefa. A atribuição transforma uma tarefa estrutural em tarefa individual do usuário escolhido.
_Avoid_: Encaminhamento informal, alteração de estado, permissão por cargo fixo

**Reatribuir tarefa**:
Ação explícita em que uma tarefa já atribuída a um usuário troca de usuário responsável. A reatribuição deve respeitar a responsabilidade de gestão sobre o usuário ou estrutura envolvida e não deve ser tratada como simples edição silenciosa da tarefa.
_Avoid_: Troca silenciosa de responsável, edição comum, histórico perdido

**Meu quadro**:
Conjunto de tarefas ativas atribuídas diretamente ao usuário logado. Tarefas apenas estruturais entram em Meu quadro somente quando forem assumidas ou atribuídas a esse usuário.
_Avoid_: Todas as tarefas da empresa, fila de departamento, tarefas de todos os usuários

**Disponíveis**:
Visão das tarefas ativas que ainda não possuem usuário responsável. Todo usuário com acesso ao módulo pode consultar essa visão; a ação de obter respeita a exigência de aprovação conforme a responsabilidade de gestão do usuário e a configuração da tarefa.
_Avoid_: Fila visível apenas para gestor, tarefa já atribuída, assunção sem autorização

**Equipe**:
Visão separada de tarefas relacionadas aos subordinados diretos do gestor imediato, visível apenas para usuário com responsabilidade de gestão. Pode existir na mesma tela de Meu quadro, mas deve manter filtros e listagem próprios para não misturar responsabilidade pessoal com responsabilidade de gestão.
_Avoid_: Quadro único misturado, tarefas de todos os usuários, subordinados indiretos automáticos

**Solicitações**:
Visão operacional, visível apenas para usuário com responsabilidade de gestão, em que o gestor acompanha solicitações pendentes de obtenção de tarefa e decide aprovar ou recusar. O e-mail pode sinalizar a pendência, mas Solicitações deve ser o local de trabalho para decisões de aprovação no módulo de tarefas.
_Avoid_: Aprovação apenas por e-mail, pendência invisível, solicitação sem acompanhamento

**Destino de equipe**:
Destino inicial de tarefa limitado aos subordinados diretos do gestor imediato. Pode criar uma tarefa diretamente para um subordinado direto ou para a fila da equipe, sem usuário responsável inicial.
_Avoid_: Subordinado indireto automático, equipe sem gestor, tarefa de toda a empresa

**Atribuição de tarefa**:
Vínculo inicial entre uma tarefa e o usuário responsável por ela. A atribuição acontece quando a tarefa é criada para aquele usuário, não quando o estado da tarefa muda.
_Avoid_: Alteração de estado, movimentação de tarefa

**Notificação de tarefa por e-mail**:
Aviso enviado ao usuário responsável quando uma tarefa é atribuída a ele, respeitando a preferência individual de recebimento. Para usuários novos, essa preferência inicia desativada.
_Avoid_: Notificação de estado, alerta de checklist

**Preferência de notificação do usuário**:
Escolha feita pelo usuário logado sobre receber ou não e-mails quando tarefas forem atribuídas a ele.
_Avoid_: Configuração administrativa, regra global

**Configurações do usuário**:
Área do módulo de usuário onde o usuário logado ajusta preferências próprias, como o recebimento de notificações de tarefa por e-mail.
_Avoid_: Minhas Preferências

**Estrutura funcional**:
Organização configurável de usuários em uma cadeia de responsabilidade, usada para decidir quem pode definir preferências ou regras de acesso de outra pessoa.
_Avoid_: Cargo fixo, perfil hardcoded, if admin, if gerente

**Gestor imediato**:
Usuário opcionalmente vinculado a outro usuário como responsável funcional direto. O gestor imediato pode servir como referência para regras de tarefas e outros módulos, sem obrigar que todo usuário tenha gestor cadastrado.
_Avoid_: Cargo gerente, perfil de acesso, relação obrigatória

**Gestor do departamento**:
Usuário opcionalmente definido como responsável de gestão de um departamento. Quando aplicável, deve existir no máximo um gestor ativo por departamento e esse gestor pode atuar como aprovador estrutural quando o usuário solicitante não tiver gestor imediato.
_Avoid_: Gestor imediato, cargo gerente, vários gestores ativos no mesmo departamento

**Responsabilidade de gestão**:
Capacidade de atuar sobre tarefas, usuários ou informações de uma estrutura funcional com base no vínculo de gestor imediato, no gestor do departamento e nas permissões do usuário. No MVP, a responsabilidade de gestão por pessoas considera apenas subordinados diretos; uma visão futura pode ampliar filtros por departamentos, cargos e estruturas de gestão.
_Avoid_: if gerente, if admin, permissão implícita por texto do cargo, subordinado indireto automático, gestor único para todos os contextos

**Perfil de acesso**:
Módulo responsável por catalogar perfis de acesso e relacionar quais módulos cada perfil pode acessar. O código canônico deste módulo e `PERF`.
_Avoid_: PAC, PRF, módulo paralelo para perfis

**Relacionamento de perfil e usuários**:
Módulo responsável por vincular usuários aos perfis de acesso já cadastrados. O código canônico deste módulo e `RPERFUSR`.
_Avoid_: Regra fixa por cargo, if admin, if gerente

**Política de acesso por módulo**:
Regra de autorização baseada no catálogo de módulos e nos perfis vinculados ao usuário. A policy técnica usa o formato `Modulo:CODIGO` e já aceita a forma futura `Modulo:CODIGO:ACAO`, mantendo a regra atual por módulo até que permissões granulares sejam modeladas.
_Avoid_: Regra hardcoded por cargo, usuário especial, if admin, if gerente

**Front Angular**:
Interface principal do produto. Para o MVP, o front mantido será `AtronFront`.
_Avoid_: Duas estruturas de front ativas

**Validação de regra de negócio**:
Mensagem de validação que explica uma regra do produto ou bloqueia uma ação de negócio deve nascer no backend, especialmente no módulo de planejamento de custos. O front Angular pode manter apenas validações de formato, estado visual ou eventos entre componentes necessários para montar a interação, mas não deve decidir a mensagem final de regra de negócio.
_Avoid_: Mensagem de regra duplicada no front, bloqueio local que impede a API de responder, validação de negocio espalhada em componente Angular

**Serviço de aplicação**:
Orquestrador de um caso de uso. Coordena interfaces de domínio, persistência e efeitos externos, mantendo visível a sequência do fluxo. Não é o dono de invariantes, mapeamentos, criação de objetos de negócio ou blocos extensos de validação; essas responsabilidades pertencem aos conceitos e colaboradores especializados que o serviço consome.
_Avoid_: Serviço concentrador, regra de domínio espalhada em orquestração, classe de passagem sem responsabilidade clara

**Validador de aplicação**:
Colaborador que valida a entrada e as condições de um fluxo de aplicação, especialmente quando essas verificações tornariam o serviço difícil de ler. Complementa, mas não substitui, as invariantes e validações do objeto de domínio.
_Avoid_: Validador que permite entidade inválida, bloco de validação centralizado no serviço, duplicação sem propósito de invariante

**Atron.WebViews**:
Estrutura legada de front MVC/Razor deletada para evitar dois projetos de front diferentes evoluindo em paralelo.
_Avoid_: Front secundário, duplicação de tela

## Example Dialogue

Dev: Quando crio uma tarefa para a Maria, isso conta como atribuição de tarefa?
Especialista: Sim. A tarefa nasceu vinculada a ela, então ela deve receber a notificação se a preferência dela permitir.

Dev: Se eu mudar a tarefa de "Aberta" para "Em andamento", envio outro e-mail?
Especialista: Não. Mudança de estado terá regras próprias no futuro, especialmente quando houver checklists.

Dev: Quem altera a preferência de notificação da Maria agora?
Especialista: A própria Maria, como usuário logado. No futuro, isso pode respeitar a estrutura funcional dela.

Dev: Onde a Maria altera essa preferência no front?
Especialista: Em Configurações, dentro do módulo de usuário.

Dev: Quando criarmos o Sales, ele deve ficar dentro do Tracker?
Especialista: Não. Tracker, Stock e Sales são módulos pares compostos pelo AtronPlatform.WebApi.

Dev: Se Tracker e Stock usam o mesmo processo, podem acessar os mesmos repositórios?
Especialista: Não. O processo é compartilhado, mas entidades, DbContexts, migrations e regras continuam pertencendo ao módulo proprietário.

Dev: Todo módulo novo deve receber um Web Service próprio?
Especialista: Não. O padrão é o monólito modular; uma extração exige contrato, estado e necessidade operacional comprovados em ADR.
