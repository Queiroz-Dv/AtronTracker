# Planejamento de Custos - Pos-MVP

Este documento lista ideias, expansoes e decisoes deixadas para etapas futuras do modulo de planejamento de custos. Nada aqui faz parte do MVP inicial, salvo se for reavaliado antes da implementacao.

## Consolidacao de planejamento

Criar uma rotina ou modulo para marcar um planejamento como consolidado, efetivado ou encerrado.

Ideia discutida:

- planejamento em edicao pode ser alterado;
- planejamento consolidado deixa de ser tratado como versao editavel;
- depois de consolidado, um novo planejamento poderia ser criado para o mesmo departamento e ano;
- consolidacao exigiria regras proprias de historico e auditoria.

Motivo para deixar depois: no MVP, ainda nao queremos trabalhar com multiplas versoes ou efetivacao formal.

## Copia de planejamento

Criar uma acao opcional para usar outro planejamento como base.

Possibilidades:

- botao para copiar do planejamento anterior;
- card ou seletor para escolher qual planejamento sera usado como base;
- copia explicita, nunca automatica.

Motivo para deixar depois: produtividade, nao regra essencial do cadastro.

## Auditoria e historico

Criar trilha de auditoria para documentar evolucao financeira, contabil ou orcamentaria dos planejamentos.

Possibilidades:

- registrar alteracoes de valores;
- registrar mudancas de modo de planejamento;
- registrar inclusao/remocao de detalhes por cargo;
- registrar usuario, data e motivo da alteracao;
- comparar versoes de planejamento.

Motivo para deixar depois: aumenta bastante o custo de implementacao e conversa melhor com consolidacao.

## Competencias menores que ano

Expandir competencia anual para outros periodos.

Periodos citados:

- bimestral;
- trimestral;
- semestral.

Motivo para deixar depois: o MVP usa ano como competencia simples e suficiente para orcamento basico.

## Detalhamento individual de custo

Criar visoes restritas para detalhar valores por pessoa remunerada quando houver necessidade real de explicar custos agregados.

Cuidados:

- informacao individual e sensivel;
- acesso deve ser mais restrito que o cadastro comum de usuario;
- nao deve reaproveitar o antigo modulo de salarios;
- deve evitar exposicao indevida em relatorios amplos.

Motivo para deixar depois: o MVP evita salario individual e trabalha apenas com custo agregado.

## Comparacao contra custo real

Criar funcionalidade para comparar planejamento contra custos realizados.

Possibilidades:

- planejado versus realizado por departamento;
- planejado versus realizado por cargo;
- variacao percentual;
- alertas de estouro de orcamento;
- analise por periodo.

Motivo para deixar depois: ainda nao existe uma fonte clara de custo real no MVP.

## Centro de custo contabil formal

Avaliar criacao de um modulo ou conceito formal de centro de custo.

Possibilidades:

- vincular departamentos a centros de custo;
- permitir rateio;
- integrar com contabilidade;
- separar centro de custo de departamento quando a empresa exigir.

Motivo para deixar depois: centro de custo e conceito contabil proprio, nao deve ser criado apenas para substituir salario.

## Modulo de contabilidade basico

Avaliar criacao de um modulo basico de contabilidade para apoiar planejamento, centros de custo e relatorios financeiros.

Motivo para deixar depois: escopo maior que o `PLC` e com regras de dominio ainda nao discutidas.

## Modulo de orcamento basico

Avaliar criacao de um modulo de orcamento mais amplo.

Possibilidades:

- tetos por area;
- acompanhamento de execucao;
- aprovacao de orcamentos;
- limites por cargo;
- relatorios de saturacao por departamento.

Motivo para deixar depois: o `PLC` sera a base inicial, nao o modulo completo de orcamento.

## Permissoes granulares

Separar acesso por acao no futuro.

Possibilidades:

- visualizar planejamento;
- criar planejamento;
- editar planejamento;
- excluir planejamento;
- acessar relatorio geral;
- consolidar planejamento;
- acessar dados sensiveis.

Preparo tecnico ja existente:

- a policy dinamica aceita o formato `Modulo:CODIGO:ACAO`;
- as acoes futuras do `PLC` devem usar nomes estaveis em `ModuloPolicies`;
- enquanto nao existir modelo de permissao por acao no perfil, `Modulo:PLC:ACAO` continua resolvendo pelo acesso ao modulo `PLC`;
- controllers do MVP devem continuar usando `Modulo:PLC`, sem trocar para acoes antes da modelagem granular.

Acoes inicialmente reservadas para o `PLC`:

- `Visualizar`;
- `Criar`;
- `Editar`;
- `Excluir`;
- `AcessarRelatorio`;
- `Consolidar`;
- `AcessarDadosSensiveis`.

Motivo para deixar depois: no MVP, o acesso segue a permissao por modulo `PLC`, e o usuario ainda vai testar relacionamento entre perfis de acesso e modulos.

## Planejamento fixo ou valor-alvo

Avaliar no futuro se existe necessidade de um valor fixo ou valor-alvo alem de minimo e teto.

Decisao atual:

- nao entra no MVP;
- minimo e teto formam a faixa do planejamento;
- valor fixo poderia virar outra regra e nao deve competir com teto.

## Retencao e limpeza automatica

Avaliar uma rotina de limpeza de planejamentos antigos.

Ideia citada:

- job ou rotina no banco para remover dados a cada 5 anos.

Pontos a decidir depois:

- se a remocao sera fisica ou logica;
- quais dados entram na politica de retencao;
- se planejamentos consolidados podem ser removidos;
- se existe obrigacao legal ou operacional de manter historico;
- como auditar a limpeza.

Motivo para deixar depois: no MVP, exclusao automatica nao entra; anos passados ficam apenas para consulta.

## Seletores e melhorias de experiencia

Possiveis melhorias futuras:

- selecionar planejamento base para copia;
- cards de planejamento;
- filtros avancados no relatorio;
- comparacao visual entre anos;
- indicadores de tendencia;
- graficos de ocupacao;
- alertas configuraveis.

Motivo para deixar depois: primeiro validar o fluxo principal de cadastro, edicao, validacao e relatorio.
