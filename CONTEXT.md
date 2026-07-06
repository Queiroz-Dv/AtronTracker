# AtronRC

Este contexto descreve a linguagem de dominio usada no AtronRC para alinhar regras de negocio entre usuarios, cargos, departamentos e tarefas.

## Language

**Usuario**:
Pessoa cadastrada no sistema que pode receber responsabilidades, acessar modulos e estar vinculada a cargo e departamento.
_Avoid_: Conta, colaborador

**Pessoa remunerada**:
Pessoa vinculada a custos de trabalho ou prestacao de servico que podem ser planejados, acompanhados ou considerados em orcamento. Nem todo usuario do sistema precisa ser uma pessoa remunerada.
_Avoid_: Usuario, conta, colaborador generico

**Informacao remuneratoria sensivel**:
Dado financeiro associado a uma pessoa remunerada, como salario ou valor equivalente de remuneracao, que exige controle de acesso mais restrito do que os dados cadastrais de usuario.
_Avoid_: Campo comum do usuario, dado cadastral simples

**Departamento**:
Area organizacional usada para agrupar cargos, usuarios e custos planejados dentro da empresa.
_Avoid_: Centro de custo, setor generico

**Cargo**:
Funcao vinculada a um departamento e usada para organizar responsabilidades, usuarios e custos planejados por tipo de trabalho.
_Avoid_: Perfil de acesso, permissao, papel tecnico

**Estrutura planejada**:
Departamento ou cargo usado por um planejamento de custo atual ou futuro. No MVP, uma estrutura planejada nao pode ser removida enquanto estiver vinculada a planejamento, para evitar divergencia entre cadastro e planejamento.
_Avoid_: Remocao livre de departamento, remocao livre de cargo

**Edicao de estrutura planejada**:
Alteracao permitida em uma estrutura planejada apenas quando nao muda sua identidade ou seu vinculo estrutural. No MVP, descricao pode ser ajustada, mas codigo e vinculo entre cargo e departamento nao devem ser alterados quando houver planejamento atual ou futuro.
_Avoid_: Alteracao retroativa de codigo, mover cargo planejado para outro departamento

**Planejamento de custo**:
Definicao ou acompanhamento de valores previstos para uma parte da organizacao em uma competencia. No MVP, cada departamento possui no maximo um planejamento por ano; cargos detalham esse planejamento quando existem cargos vinculados ao departamento, mas nao sao obrigatorios para criar o planejamento.
_Avoid_: Salario do usuario, cadastro salarial

**Identificacao do planejamento de custo**:
Identidade do registro financeiro, contabil ou orcamentario criado pelo usuario para o planejamento. No MVP, o planejamento possui identificacao composta por id, codigo obrigatorio informado pelo usuario e descricao obrigatoria; a descricao nao precisa ser unica e o codigo nao muda depois da criacao.
_Avoid_: Registro anonimo, planejamento sem codigo, planejamento sem descricao, descricao unica obrigatoria

**Cadastro de planejamento de custo**:
Fluxo em que o usuario escolhe ano e departamento, informa os limites de custo do departamento e, quando existirem cargos vinculados, visualiza os cargos do departamento para detalhar limites no mesmo planejamento.
_Avoid_: Cadastro por cargo isolado, cadastro salarial

**Edicao de planejamento de custo**:
Alteracao do corpo de um planejamento existente, preservando sua identidade, departamento e ano. No MVP, a edicao permite ajustar descricao, valores e detalhamento por cargo, incluindo informar custos para cargos novos vinculados ao departamento depois da criacao do planejamento.
_Avoid_: Troca de departamento, troca de ano, recriacao disfarcada

**Planejamento apenas por departamento**:
Forma de planejamento em que o usuario decide explicitamente nao detalhar custos por cargo, mesmo quando o departamento possui cargos vinculados. No MVP, essa opcao faz parte do cadastro e deixa o detalhamento por cargo desabilitado ou oculto para aquele planejamento; enquanto o planejamento for editavel, o usuario pode alternar entre planejamento apenas por departamento e detalhamento por cargo. Ao mudar para apenas por departamento, os detalhes por cargo sao removidos definitivamente apos confirmacao. Cargos novos no departamento nao geram pendencia enquanto esta opcao estiver ativa.
_Avoid_: Esquecimento de preencher cargos, planejamento incompleto acidental

**Cargo nao detalhado**:
Cargo exibido no cadastro do planejamento do departamento, mas marcado explicitamente como fora do detalhamento de custos daquele planejamento. No MVP, cargo nao detalhado nao entra na soma dos cargos e nao aparece como linha no relatorio geral; o relatorio pode exibir um resumo neutro da quantidade de cargos nao detalhados.
_Avoid_: Campo vazio sem intencao, erro de cadastro, valor zero

**Cargo detalhado**:
Cargo que participa do detalhamento de custos de um planejamento e possui minimo e teto informados. No MVP, cargo detalhado sem minimo ou sem teto e considerado incompleto; ao sair de planejamento apenas por departamento para detalhamento por cargo, cada cargo comeca sem valores e exige decisao explicita.
_Avoid_: Cargo parcial, valor isolado de cargo

**Cargo pendente de decisao**:
Cargo vinculado ao departamento de um planejamento detalhado, mas ainda sem decisao explicita de detalhar ou nao detalhar. No MVP, cargos novos em departamentos ja planejados aparecem como pendentes ate receberem minimo e teto ou serem marcados como nao detalhados; se o usuario abrir e salvar o planejamento, pendencias de cargo bloqueiam o salvamento. No relatorio geral, cargos pendentes aparecem como alerta identificando o cargo.
_Avoid_: Cargo invisivel, cargo ignorado automaticamente

**Planejamento consolidado**:
Planejamento de custo marcado como efetivado para uma competencia, deixando de ser tratado como versao em edicao. No MVP, consolidacao de planejamento fica fora do escopo.
_Avoid_: Rascunho, edicao comum, historico tecnico

**Copia de planejamento**:
Acao opcional e explicita em que o usuario usa um planejamento existente como base para preencher um novo planejamento. Um novo planejamento nao copia valores anteriores automaticamente; no MVP, a copia de planejamento fica fora do escopo.
_Avoid_: Heranca automatica de valores, reaproveitamento implicito

**Modulo de planejamento de custos**:
Modulo responsavel por cadastrar e acompanhar planejamentos de custo por departamento e por cargos vinculados ao departamento. O codigo canonico deste modulo e `PLC`; no MVP, o acesso segue a permissao por modulo e as telas de cadastro e relatorio geral sao separadas. Ele substitui o modulo de salarios como capacidade de negocio, mas nao reaproveita o conceito de cadastro salarial.
_Avoid_: Modulo de salarios, cadastro de salario, salario de usuario

**Limite de custo planejado**:
Faixa esperada para o custo total planejado de um departamento ou de um cargo dentro de um departamento em uma competencia, composta por valor minimo e valor teto. No MVP, minimo e teto do departamento sao obrigatorios.
_Avoid_: Salario minimo, salario maximo, custo por pessoa, custo por vaga, valor livre sem competencia

**Teto de custo planejado**:
Valor maximo que o custo total planejado pode atingir em um departamento ou cargo dentro de um departamento. No MVP, cada cargo pode ter teto proprio, a soma dos tetos dos cargos nao pode ultrapassar o teto do departamento, e teto menor ou igual ao minimo bloqueia o planejamento.
_Avoid_: Valor sugerido, limite ignoravel

**Minimo de custo planejado**:
Valor de referencia inferior para o custo total planejado em um departamento ou cargo dentro de um departamento. No MVP, cada cargo pode ter minimo proprio; minimo maior ou igual ao teto bloqueia o planejamento, enquanto divergencias entre a soma dos minimos dos cargos e os limites do departamento geram alerta.
_Avoid_: Obrigacao de gasto, bloqueio de cadastro

**Competencia de planejamento**:
Periodo ao qual um planejamento de custo se aplica. No MVP, a competencia de planejamento e anual; novos planejamentos usam o ano atual ou anos futuros, enquanto anos passados ficam restritos a consulta e nao podem ser editados. O relatorio geral permite escolher o ano analisado; bimestre, trimestre e semestre sao expansoes futuras do mesmo conceito.
_Avoid_: Data solta, mes avulso sem planejamento

**Exclusao de planejamento**:
Remocao definitiva de um planejamento de custo ainda alteravel, incluindo seus detalhes por cargo. No MVP, planejamentos do ano atual ou futuro podem ser excluidos; anos passados ficam restritos a consulta.
_Avoid_: Exclusao retroativa, limpeza automatica

**Custo agregado**:
Valor consolidado usado para acompanhar planejamento ou orcamento sem expor diretamente a informacao remuneratoria sensivel de cada pessoa.
_Avoid_: Lista salarial individual, detalhe de remuneracao

**Relatorio geral de planejamento**:
Visao analitica do planejamento de custos por ano, exibindo apenas departamentos que possuem planejamento informado para a competencia analisada, detalhamento por cargos quando aplicavel, valores absolutos, percentual de ocupacao do teto e alertas de planejamento. No MVP, o percentual de ocupacao usa a soma dos tetos dos cargos detalhados sobre o teto do departamento; planejamento apenas por departamento aparece sem ocupacao detalhada.
Plano visual pendente: definir a paleta final do relatorio impresso com roxo, branco e preto, usando preto para fontes.
_Avoid_: Listagem simples, tela de cadastro

**Percentual de ocupacao do teto**:
Indicador calculado pela soma dos tetos dos cargos detalhados sobre o teto do departamento. No MVP, aparece no cadastro durante o preenchimento e no relatorio geral de planejamento; 100% e valido, acima de 100% bloqueia o planejamento.
_Avoid_: Percentual de salario, uso do minimo como base

**Alerta de planejamento**:
Indicacao de que um planejamento de custo informado merece revisao, como divergencia de minimo ou ausencia de decisao clara sobre detalhamento por cargo. Departamento sem planejamento nao entra no relatorio do MVP. Planejamento apenas por departamento e cargo nao detalhado sao decisoes explicitas e aparecem como informacao neutra, nao como alerta.
_Avoid_: Erro impeditivo, falha tecnica

**Detalhamento individual de custo**:
Visao restrita que mostra valores vinculados a pessoas remuneradas quando a analise precisa explicar ou confrontar o custo agregado.
_Avoid_: Dado publico do usuario, campo comum de cadastro

**Tarefa**:
Responsabilidade registrada no sistema para acompanhamento. Uma tarefa pode estar vinculada diretamente a um usuario responsavel ou a um escopo de responsabilidade por departamento e cargo.
_Avoid_: Atividade, demanda

**Escopo de responsabilidade da tarefa**:
Departamento, cargo ou usuario que delimita quem deve visualizar, assumir ou acompanhar uma tarefa. Uma tarefa pode nascer no escopo de uma estrutura funcional e depois ser atribuida a um usuario responsavel; enquanto nao tiver usuario responsavel, pertence a uma fila de triagem da estrutura.
_Avoid_: Tarefa sempre individual, fila tecnica, responsavel implicito

**Destino inicial da tarefa**:
Escolha feita na criacao da tarefa para definir se ela nasce atribuida a um usuario, vinculada a departamento e cargo, ou direcionada para equipe do gestor. As opcoes disponiveis devem respeitar as permissoes e a responsabilidade de gestao do usuario logado.
_Avoid_: Destino unico obrigatorio, tarefa sem escopo, escolha que ignora permissao

**Tarefa estrutural**:
Tarefa cujo destino inicial e uma estrutura funcional em vez de um usuario responsavel. Pode ser vinculada apenas a departamento ou a departamento e cargo; nao deve existir tarefa vinculada a cargo sem departamento.
_Avoid_: Cargo sem departamento, tarefa sem usuario e sem estrutura, tarefa de todos

**Identificador da tarefa**:
Codigo numerico sequencial usado pelos usuarios para localizar uma tarefa no produto. O identificador e global no sistema, independente de departamento ou cargo, e deve ser exibido de forma amigavel com zeros a esquerda quando necessario.
_Avoid_: Id tecnico, codigo por departamento, prefixo obrigatorio

**Busca por identificador da tarefa**:
Consulta direta de tarefa pelo identificador numerico, respeitando as permissoes do usuario. A busca pode localizar tarefas ativas, finalizadas, individuais ou estruturais, desde que estejam dentro do acesso permitido ao usuario.
_Avoid_: Filtro apenas do quadro atual, listagem geral, acesso irrestrito por codigo

**Tarefa finalizada**:
Tarefa encerrada para operacao e mantida apenas para consulta. Uma tarefa finalizada nao pode ser reaberta, atualizada ou removida.
_Avoid_: Reabertura de tarefa, edicao pos-finalizacao, exclusao de historico

**Tarefa cancelada**:
Tarefa encerrada antes da conclusao operacional e mantida apenas para consulta. O cancelamento substitui a exclusao de tarefa para preservar historico e deve impedir atualizacao posterior.
_Avoid_: Exclusao fisica, apagar tarefa, remover historico

**Cancelar tarefa**:
Acao em que o usuario responsavel encerra uma tarefa ainda nao finalizada sem conclui-la. O cancelamento nao exige motivo obrigatorio separado; quando houver justificativa, ela deve ser descrita no conteudo da propria tarefa.
_Avoid_: Deletar tarefa, finalizar sem conclusao, ocultar registro, motivo obrigatorio separado

**Cancelamento de tarefa em fila de triagem**:
Acao em que um usuario com responsabilidade de gestao sobre o departamento ou cargo da fila encerra uma tarefa estrutural antes de ela ser assumida ou atribuida a um usuario responsavel.
_Avoid_: Cancelamento por usuario comum, exclusao de fila, tarefa estrutural sem governanca

**Tarefa ativa**:
Tarefa ainda aberta para acompanhamento ou decisao operacional. Em Meu quadro e Equipe, tarefas iniciadas, em atividade, pendentes de aprovacao ou entregues sao consideradas ativas; tarefas finalizadas e canceladas aparecem somente quando filtradas explicitamente pelo usuario.
_Avoid_: Tarefa finalizada no carregamento padrao, tarefa cancelada no carregamento padrao, historico misturado com operacao

**Fila de triagem de tarefas**:
Conjunto de tarefas vinculadas a departamento ou cargo que ainda nao pertencem a Meu quadro de um usuario. A fila deve ser acompanhada por usuarios com responsabilidade de gestao sobre aquela estrutura, evitando distribuir tarefas estruturais para todos os usuarios do escopo.
_Avoid_: Quadro pessoal compartilhado, listagem geral de tarefas, carga automatica para todos

**Assumir tarefa**:
Acao em que um usuario com acesso a uma fila de triagem torna uma tarefa estrutural uma tarefa individual propria. Depois de assumida, a tarefa passa a aparecer em Meu quadro desse usuario.
_Avoid_: Atribuicao automatica, visualizacao sem responsabilidade

**Obter tarefa**:
Acao em que um usuario solicita ou assume uma tarefa disponivel em uma fila permitida. Quando a tarefa nao exige aprovacao, ela entra diretamente em Meu quadro; quando exige aprovacao, a obtencao gera uma solicitacao ao aprovador. Obter tarefa nunca altera o estado operacional da tarefa.
_Avoid_: Atribuicao pelo gestor, edicao do responsavel sem regra, tarefa invisivel, mudanca automatica de estado

**Solicitacao de obtencao de tarefa**:
Pedido feito por um usuario para receber uma tarefa marcada como pendente de aprovacao. A solicitacao deve aparecer para o aprovador na visao Solicitacoes e, quando aprovada, a tarefa passa para Meu quadro do usuario solicitante. Uma tarefa deve ter no maximo uma solicitacao de obtencao pendente por vez.
_Avoid_: Aprovacao implicita, e-mail como local de decisao, tarefa pendente assumida diretamente, varias solicitacoes pendentes para a mesma tarefa

**Aprovacao para obter tarefa**:
Exigencia separada do estado operacional da tarefa que define se um usuario pode obter a tarefa diretamente ou se precisa da aprovacao do gestor imediato. Essa exigencia nao deve ser confundida com o estado da tarefa.
_Avoid_: Estado pendente de aprovacao, bloqueio implicito, aprovacao como andamento da tarefa

**Aprovacao de obtencao**:
Decisao do aprovador de obtencao de tarefa para aprovar ou recusar uma solicitacao. A aprovacao valida transforma a tarefa solicitada em tarefa individual do usuario solicitante sem alterar seu estado operacional; a recusa encerra a solicitacao e mantem a tarefa disponivel conforme seu escopo.
_Avoid_: Aprovacao implicita, resposta manual ao e-mail, assumir tarefa sem decisao, mudanca automatica de estado

**Notificacao do sistema**:
Aviso interno apresentado ao usuario dentro do produto para informar eventos relevantes. No primeiro escopo, notificacoes de tarefas informam aprovacoes ou recusas de solicitacoes e podem levar o usuario ao detalhe da tarefa quando aplicavel; notificacoes devem poder ser controladas como lidas ou nao lidas pelo usuario.
_Avoid_: E-mail obrigatorio, alerta sem destino, historico invisivel, notificacao sem leitura

**Central de notificacoes**:
Area do produto onde o usuario acompanha notificacoes do sistema e acessa os detalhes relacionados ao evento notificado. No primeiro escopo, a central atende eventos do modulo de tarefas e deve permitir abrir a tarefa relacionada quando houver uma; no futuro, tende a ser o canal padrao de notificacoes internas do produto.
_Avoid_: Caixa de e-mail, alerta temporario sem historico, lista sem contexto

**Atualizacao em tempo real**:
Comportamento em que notificacoes e solicitacoes relevantes aparecem para o usuario sem depender de recarregamento manual da tela. No modulo de tarefas, esse comportamento apoia aprovacoes, recusas e acompanhamento de solicitacoes.
_Avoid_: Consulta manual constante, e-mail como fonte principal, tela desatualizada

**Notificacao por e-mail**:
Aviso enviado fora do produto apenas quando o evento exigir comunicacao externa ou acao especifica por e-mail. A direcao do produto e tratar notificacoes internas como padrao e usar e-mail em casos especificos.
_Avoid_: E-mail para todo evento, e-mail como fonte principal do sistema, duplicacao obrigatoria de notificacao

**Aprovador de obtencao de tarefa**:
Usuario responsavel por aprovar ou recusar uma solicitacao de obtencao de tarefa. A ordem preferencial e: gestor imediato do solicitante; gestor do departamento da tarefa; gestor do departamento do solicitante; se nenhum aprovador existir, a solicitacao deve ser bloqueada por regra de negocio.
_Avoid_: Solicitacao sem aprovador, aprovador aleatorio, regra fixa por cargo

**Atribuir tarefa**:
Acao em que um usuario com responsabilidade de gestao escolhe outro usuario dentro do escopo permitido para ser responsavel por uma tarefa. A atribuicao transforma uma tarefa estrutural em tarefa individual do usuario escolhido.
_Avoid_: Encaminhamento informal, alteracao de estado, permissao por cargo fixo

**Reatribuir tarefa**:
Acao explicita em que uma tarefa ja atribuida a um usuario troca de usuario responsavel. A reatribuicao deve respeitar a responsabilidade de gestao sobre o usuario ou estrutura envolvida e nao deve ser tratada como simples edicao silenciosa da tarefa.
_Avoid_: Troca silenciosa de responsavel, edicao comum, historico perdido

**Meu quadro**:
Conjunto de tarefas ativas atribuidas diretamente ao usuario logado. Tarefas apenas estruturais entram em Meu quadro somente quando forem assumidas ou atribuidas a esse usuario.
_Avoid_: Todas as tarefas da empresa, fila de departamento, tarefas de todos os usuarios

**Equipe**:
Visao separada de tarefas relacionadas aos subordinados diretos do gestor imediato. Pode existir na mesma tela de Meu quadro, mas deve manter filtros e listagem proprios para nao misturar responsabilidade pessoal com responsabilidade de gestao.
_Avoid_: Quadro unico misturado, tarefas de todos os usuarios, subordinados indiretos automaticos

**Solicitacoes**:
Visao operacional em que o gestor acompanha solicitacoes pendentes de obtencao de tarefa e decide aprovar ou recusar. O e-mail pode sinalizar a pendencia, mas Solicitacoes deve ser o local de trabalho para decisoes de aprovacao no modulo de tarefas.
_Avoid_: Aprovacao apenas por e-mail, pendencia invisivel, solicitacao sem acompanhamento

**Destino de equipe**:
Destino inicial de tarefa limitado aos subordinados diretos do gestor imediato. Pode criar uma tarefa diretamente para um subordinado direto ou para a fila da equipe, sem usuario responsavel inicial.
_Avoid_: Subordinado indireto automatico, equipe sem gestor, tarefa de toda a empresa

**Atribuicao de tarefa**:
Vinculo inicial entre uma tarefa e o usuario responsavel por ela. A atribuicao acontece quando a tarefa e criada para aquele usuario, nao quando o estado da tarefa muda.
_Avoid_: Alteracao de estado, movimentacao de tarefa

**Notificacao de tarefa por e-mail**:
Aviso enviado ao usuario responsavel quando uma tarefa e atribuida a ele, respeitando a preferencia individual de recebimento. Para usuarios novos, essa preferencia inicia desativada.
_Avoid_: Notificacao de estado, alerta de checklist

**Preferencia de notificacao do usuario**:
Escolha feita pelo usuario logado sobre receber ou nao e-mails quando tarefas forem atribuidas a ele.
_Avoid_: Configuracao administrativa, regra global

**Configuracoes do usuario**:
Area do modulo de usuario onde o usuario logado ajusta preferencias proprias, como o recebimento de notificacoes de tarefa por e-mail.
_Avoid_: Minhas Preferencias

**Estrutura funcional**:
Organizacao configuravel de usuarios em uma cadeia de responsabilidade, usada para decidir quem pode definir preferencias ou regras de acesso de outra pessoa.
_Avoid_: Cargo fixo, perfil hardcoded, if admin, if gerente

**Gestor imediato**:
Usuario opcionalmente vinculado a outro usuario como responsavel funcional direto. O gestor imediato pode servir como referencia para regras de tarefas e outros modulos, sem obrigar que todo usuario tenha gestor cadastrado.
_Avoid_: Cargo gerente, perfil de acesso, relacao obrigatoria

**Gestor do departamento**:
Usuario opcionalmente definido como responsavel de gestao de um departamento. Quando aplicavel, deve existir no maximo um gestor ativo por departamento e esse gestor pode atuar como aprovador estrutural quando o usuario solicitante nao tiver gestor imediato.
_Avoid_: Gestor imediato, cargo gerente, varios gestores ativos no mesmo departamento

**Responsabilidade de gestao**:
Capacidade de atuar sobre tarefas, usuarios ou informacoes de uma estrutura funcional com base no vinculo de gestor imediato, no gestor do departamento e nas permissoes do usuario. No MVP, a responsabilidade de gestao por pessoas considera apenas subordinados diretos; uma visao futura pode ampliar filtros por departamentos, cargos e estruturas de gestao.
_Avoid_: if gerente, if admin, permissao implicita por texto do cargo, subordinado indireto automatico, gestor unico para todos os contextos

**Perfil de acesso**:
Modulo responsavel por catalogar perfis de acesso e relacionar quais modulos cada perfil pode acessar. O codigo canonico deste modulo e `PERF`.
_Avoid_: PAC, PRF, modulo paralelo para perfis

**Relacionamento de perfil e usuarios**:
Modulo responsavel por vincular usuarios aos perfis de acesso ja cadastrados. O codigo canonico deste modulo e `RPERFUSR`.
_Avoid_: Regra fixa por cargo, if admin, if gerente

**Politica de acesso por modulo**:
Regra de autorizacao baseada no catalogo de modulos e nos perfis vinculados ao usuario. A policy tecnica usa o formato `Modulo:CODIGO` e ja aceita a forma futura `Modulo:CODIGO:ACAO`, mantendo a regra atual por modulo ate que permissoes granulares sejam modeladas.
_Avoid_: Regra hardcoded por cargo, usuario especial, if admin, if gerente

**Front Angular**:
Interface principal do produto. Para o MVP, o front mantido sera `AtronFront`.
_Avoid_: Duas estruturas de front ativas

**Validacao de regra de negocio**:
Mensagem de validacao que explica uma regra do produto ou bloqueia uma acao de negocio deve nascer no backend, especialmente no modulo de planejamento de custos. O front Angular pode manter apenas validacoes de formato, estado visual ou eventos entre componentes necessarios para montar a interacao, mas nao deve decidir a mensagem final de regra de negocio.
_Avoid_: Mensagem de regra duplicada no front, bloqueio local que impede a API de responder, validacao de negocio espalhada em componente Angular

**Atron.WebViews**:
Estrutura legada de front MVC/Razor deletada para evitar dois projetos de front diferentes evoluindo em paralelo.
_Avoid_: Front secundario, duplicacao de tela

## Example Dialogue

Dev: Quando crio uma tarefa para a Maria, isso conta como atribuicao de tarefa?
Especialista: Sim. A tarefa nasceu vinculada a ela, entao ela deve receber a notificacao se a preferencia dela permitir.

Dev: Se eu mudar a tarefa de "Aberta" para "Em andamento", envio outro e-mail?
Especialista: Nao. Mudanca de estado tera regras proprias no futuro, especialmente quando houver checklists.

Dev: Quem altera a preferencia de notificacao da Maria agora?
Especialista: A propria Maria, como usuario logado. No futuro, isso pode respeitar a estrutura funcional dela.

Dev: Onde a Maria altera essa preferencia no front?
Especialista: Em Configuracoes, dentro do modulo de usuario.
