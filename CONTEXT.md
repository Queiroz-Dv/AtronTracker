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
Identidade do registro financeiro, contabil ou orcamentario criado pelo usuario para o planejamento. No MVP, o planejamento possui identificacao composta por id, codigo gerado pelo sistema e descricao obrigatoria informada pelo usuario; a descricao nao precisa ser unica e o codigo nao muda depois da criacao.
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
Visao analitica do planejamento de custos por ano, exibindo departamentos, detalhamento por cargos quando aplicavel, valores absolutos, percentual de ocupacao do teto e alertas de planejamento. No MVP, o percentual de ocupacao usa a soma dos tetos dos cargos detalhados sobre o teto do departamento; planejamento apenas por departamento aparece sem ocupacao detalhada.
Plano visual pendente: definir a paleta final do relatorio impresso com roxo, branco e preto, usando preto para fontes.
_Avoid_: Listagem simples, tela de cadastro

**Percentual de ocupacao do teto**:
Indicador calculado pela soma dos tetos dos cargos detalhados sobre o teto do departamento. No MVP, aparece no cadastro durante o preenchimento e no relatorio geral de planejamento; 100% e valido, acima de 100% bloqueia o planejamento.
_Avoid_: Percentual de salario, uso do minimo como base

**Alerta de planejamento**:
Indicacao de que um planejamento de custo merece revisao, como departamento sem planejamento no ano, divergencia de minimo ou ausencia de decisao clara sobre detalhamento por cargo. Planejamento apenas por departamento e cargo nao detalhado sao decisoes explicitas e aparecem como informacao neutra, nao como alerta.
_Avoid_: Erro impeditivo, falha tecnica

**Detalhamento individual de custo**:
Visao restrita que mostra valores vinculados a pessoas remuneradas quando a analise precisa explicar ou confrontar o custo agregado.
_Avoid_: Dado publico do usuario, campo comum de cadastro

**Tarefa**:
Responsabilidade registrada no sistema e atribuida a um unico usuario para acompanhamento.
_Avoid_: Atividade, demanda

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
