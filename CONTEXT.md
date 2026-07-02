# AtronRC

Este contexto descreve a linguagem de dominio usada no AtronRC para alinhar regras de negocio entre usuarios, cargos, departamentos e tarefas.

## Language

**Usuario**:
Pessoa cadastrada no sistema que pode receber responsabilidades, acessar modulos e estar vinculada a cargo e departamento.
_Avoid_: Conta, colaborador

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

**Politicas e Acessos**:
Modulo responsavel por catalogar modulos, perfis e permissoes do sistema. O codigo canonico deste modulo e `PAC`.
_Avoid_: PRF, PERF, modulo paralelo para relacionamento perfil-usuario

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
