# Remodelar tarefas com gestao e notificacoes internas

O modulo de tarefas sera remodelado para deixar de ser uma listagem geral de tarefas atribuidas apenas a usuarios e passar a operar por visoes de trabalho, escopos estruturais, solicitacoes de obtencao e notificacoes internas. A decisao separa responsabilidade pessoal, gestao de equipe e aprovacao de solicitacoes, reduz carga de dados no carregamento padrao e prepara o produto para uma rotina de tarefas mais escalavel.

## Decisao

A tela de tarefas tera as visoes `Meu quadro`, `Equipe` e `Solicitacoes`. `Meu quadro` mostra tarefas ativas do usuario logado; `Equipe` mostra tarefas relacionadas aos subordinados diretos do gestor imediato; `Solicitacoes` e o local principal onde gestores aprovam ou recusam pedidos de obtencao de tarefa.

Uma tarefa podera nascer com destino inicial para `Usuario`, `Departamento/Cargo` ou `Equipe`. Tarefas estruturais podem ser vinculadas apenas a departamento ou a departamento e cargo, mas nunca a cargo sem departamento. O identificador da tarefa sera numerico, sequencial e global, separado do identificador tecnico.

Tarefas disponiveis poderao ser obtidas por usuarios. Quando a tarefa nao exigir aprovacao, ela entra diretamente em `Meu quadro`; quando exigir aprovacao, o sistema cria uma solicitacao para o aprovador. Obter ou aprovar uma tarefa nunca altera seu estado operacional. Tarefas finalizadas ou canceladas ficam fora dos carregamentos padrao e aparecem apenas por filtro explicito ou busca autorizada.

O produto tera uma central de notificacoes internas como canal principal de eventos do sistema, inicialmente para tarefas. Notificacoes devem suportar leitura e nao leitura e podem levar o usuario ao detalhe relacionado. E-mail deixa de ser canal padrao de operacao e passa a ser usado apenas em casos especificos de comunicacao externa ou aviso.

## Considered Options

Manter tarefa sempre atribuida a um unico usuario foi rejeitado porque impediria filas por estrutura, obtencao de tarefas e gestao por equipe.

Usar e-mail como principal meio de aprovacao foi rejeitado porque deixaria o fluxo operacional fora do produto. A decisao e manter `Solicitacoes` e a central de notificacoes como fonte principal, usando e-mail apenas quando houver necessidade especifica.

Tratar `Pendente de aprovacao` como estado operacional foi rejeitado porque mistura ciclo de execucao com regra de obtencao. A aprovacao para obter tarefa sera uma exigencia separada do estado.

## Consequencias

O cadastro de usuarios passa a precisar de `Gestor imediato` opcional, e departamentos passam a poder ter `Gestor do departamento` opcional, respeitando a regra de no maximo um gestor ativo por departamento quando aplicavel.

A aprovacao de obtencao deve seguir a ordem: gestor imediato do solicitante, gestor do departamento da tarefa, gestor do departamento do solicitante. Se nenhum aprovador existir, a solicitacao deve ser bloqueada por regra de negocio no backend.

O modulo de notificacoes internas passa a ser uma capacidade propria do produto. Mesmo com tarefas como primeiro caso de uso, ele deve nascer preparado para atender outros modulos no futuro.
