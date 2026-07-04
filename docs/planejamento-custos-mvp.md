# Planejamento de Custos - MVP

Este documento consolida o escopo inicial do modulo de planejamento de custos do AtronRC. Ele foi definido para substituir o modulo de salarios, removendo salario individual do cadastro de usuario e tratando custos como planejamento agregado por departamento e por cargos vinculados ao departamento.

## Objetivo

Criar o modulo **Planejamento de Custos**, com codigo canonico `PLC`, para cadastrar, editar e analisar planejamentos anuais de custo da empresa sem expor informacao remuneratoria sensivel individual.

O modulo deve trabalhar com custo total planejado, nao com custo por pessoa, vaga ou salario individual.

## Decisao sobre salarios

O modulo atual de salarios sera removido por completo.

Devem ser removidos:

- telas, rotas e services do front relacionados a salarios;
- controllers, services, DTOs, entidades, repositorios e validacoes de salario no backend;
- tabela e dados de salarios;
- propriedade de salario atual do usuario;
- vinculos de permissao/perfil relacionados ao modulo de salarios;
- codigo de modulo antigo de salarios no catalogo de modulos.

Os valores antigos de salario nao serao migrados para o novo planejamento. A decisao esta registrada em [ADR 0001](./adr/0001-substituir-salarios-por-planejamento-de-custos.md).

## Modulo e acesso

O novo modulo sera cadastrado como `PLC`.

No MVP, o acesso segue a permissao por modulo ja existente no sistema. A separacao por acoes, como visualizar, criar, editar e acessar relatorio, fica fora do MVP.

O modulo deve ter telas separadas:

- cadastro/manutencao de planejamentos;
- relatorio geral de planejamento.

## Identificacao do planejamento

Cada planejamento e um registro financeiro, contabil ou orcamentario da empresa.

No MVP, o planejamento possui:

- `Id`, para controle interno;
- `Codigo`, gerado pelo sistema;
- `Descricao`, obrigatoria e informada pelo usuario;
- ano;
- departamento;
- minimo e teto do departamento;
- opcao de planejamento apenas por departamento;
- detalhes por cargo, quando aplicavel.

O codigo nao muda depois da criacao. A descricao pode ser editada e nao precisa ser unica.

Departamento, ano e codigo nao podem ser trocados depois da criacao. Para corrigir departamento ou ano, o planejamento deve ser excluido e recriado, desde que ainda seja de ano atual ou futuro.

## Regra anual

A competencia do MVP e anual.

Regras:

- cada departamento pode ter no maximo um planejamento por ano;
- novos planejamentos so podem usar ano atual ou anos futuros;
- planejamentos de anos passados ficam apenas para consulta;
- planejamentos de anos passados nao podem ser editados nem excluidos;
- o relatorio geral permite escolher o ano analisado.

## Cadastro de planejamento

O fluxo de cadastro deve partir do departamento.

Fluxo esperado:

1. Usuario informa ano.
2. Usuario seleciona departamento.
3. Usuario informa descricao.
4. Usuario informa minimo e teto do departamento.
5. Se o departamento tiver cargos vinculados, o usuario decide se o planejamento sera apenas por departamento ou se havera detalhamento por cargo.

O planejamento pode ser criado mesmo quando o departamento nao possui cargos.

## Planejamento apenas por departamento

O usuario pode marcar o planejamento como **apenas por departamento**.

Nesse caso:

- o detalhamento por cargo fica oculto ou desabilitado;
- cargos novos no departamento nao geram pendencia;
- o relatorio mostra essa escolha como informacao neutra, nao como alerta;
- nao ha percentual de ocupacao detalhada por cargo.

Enquanto o planejamento for editavel, o usuario pode mudar para detalhamento por cargo.

Se o planejamento ja tinha detalhes por cargo e o usuario mudar para apenas por departamento, os detalhes por cargo devem ser removidos definitivamente apos confirmacao.

## Detalhamento por cargo

Quando o planejamento nao for apenas por departamento, a tela deve listar os cargos vinculados ao departamento.

Cada cargo deve receber uma decisao explicita:

- **Cargo detalhado**: possui minimo e teto informados;
- **Cargo nao detalhado**: nao entra no planejamento detalhado;
- **Cargo pendente de decisao**: ainda nao foi detalhado nem marcado como nao detalhado.

Cargo detalhado exige minimo e teto. Cargo sem minimo ou sem teto e incompleto.

Cargo nao detalhado:

- nao entra na soma dos cargos;
- nao aparece como linha no relatorio geral;
- pode aparecer apenas como resumo neutro de quantidade.

Se todos os cargos forem marcados como nao detalhados, o sistema deve impedir salvar e orientar o usuario a usar **Planejamento apenas por departamento**.

Ao sair de planejamento apenas por departamento para detalhamento por cargo, todos os cargos comecam sem valores e exigem decisao explicita.

## Cargos novos em departamento planejado

Se um cargo novo for vinculado a um departamento que ja tem planejamento detalhado, esse cargo deve aparecer no cadastro do planejamento como pendente de decisao.

Enquanto o usuario nao abrir o planejamento, o relatorio pode alertar essa pendencia.

Se o usuario abrir e tentar salvar o planejamento, cargos pendentes bloqueiam o salvamento. Para salvar, cada cargo pendente deve receber minimo/teto ou ser marcado como nao detalhado.

## Limites e validacoes

Departamento:

- minimo obrigatorio;
- teto obrigatorio;
- minimo deve ser menor que teto;
- minimo maior ou igual ao teto bloqueia.

Cargo detalhado:

- minimo obrigatorio;
- teto obrigatorio;
- minimo deve ser menor que teto;
- minimo maior ou igual ao teto bloqueia.

Soma dos cargos:

- soma dos tetos dos cargos detalhados nao pode ultrapassar o teto do departamento;
- soma igual ao teto do departamento e valida;
- acima do teto do departamento bloqueia;
- divergencias envolvendo minimos geram alerta, nao bloqueio.

Percentual de ocupacao:

- calculado por `soma dos tetos dos cargos detalhados / teto do departamento`;
- aparece no cadastro durante o preenchimento;
- aparece no relatorio geral;
- 100% e valido;
- acima de 100% bloqueia;
- planejamento apenas por departamento aparece sem ocupacao detalhada.

## Edicao

Planejamentos de ano atual ou futuro podem ser editados.

Podem ser editados:

- descricao;
- minimo e teto do departamento;
- modo apenas por departamento;
- detalhamento por cargo;
- minimo e teto dos cargos;
- marcacao de cargo nao detalhado.

Nao podem ser editados:

- codigo;
- departamento;
- ano.

## Exclusao

Planejamentos de ano atual ou futuro podem ser excluidos definitivamente.

A exclusao remove tambem os detalhes por cargo vinculados ao planejamento.

Planejamentos de anos passados nao podem ser excluidos no MVP.

## Relatorio geral

O relatorio geral deve permitir escolher o ano.

Ele deve exibir:

- todos os departamentos;
- departamentos com planejamento;
- departamentos sem planejamento;
- dados do planejamento do departamento;
- cargos detalhados, quando aplicavel;
- valores absolutos;
- percentual de ocupacao do teto, quando houver detalhamento por cargo;
- resumo neutro de cargos nao detalhados;
- informacao neutra quando o planejamento for apenas por departamento;
- alertas de planejamento.

Alertas esperados:

- departamento sem planejamento no ano;
- cargo pendente de decisao;
- divergencias envolvendo minimos;
- ausencia de decisao clara sobre detalhamento por cargo.

Nao sao alertas:

- planejamento apenas por departamento;
- cargo marcado explicitamente como nao detalhado.

## Integridade com departamento e cargo

Departamento ou cargo vinculado a planejamento atual ou futuro nao pode ser removido.

Quando houver planejamento atual ou futuro:

- descricao de departamento/cargo pode ser ajustada;
- codigo de departamento/cargo nao deve ser alterado;
- cargo nao pode ser movido para outro departamento.

Essa regra evita divergencia entre cadastro estrutural e planejamento.

## Fora do MVP

Os itens fora do MVP estao consolidados em [Planejamento de Custos - Pos-MVP](./planejamento-custos-pos-mvp.md).
