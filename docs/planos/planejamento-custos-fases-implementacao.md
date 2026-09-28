# Planejamento de Custos - Fases de Implementacao

Este documento quebra a implementacao do MVP de Planejamento de Custos em fases bloqueaveis. A intencao e reduzir risco, permitir validacao manual por etapa e evitar mudancas grandes demais no sistema em um unico corte.

## Regras de execucao

- Cada fase deve ser concluida e validada manualmente antes da proxima.
- Fases bloqueadas nao devem ser iniciadas enquanto a fase anterior nao estiver estavel.
- Usar validacao manual nesta leva.
- Nao reintroduzir salario individual em usuario, relatorio ou planejamento.
- Respeitar o `CONTEXT.md`, o PRD e o ADR 0001 como fontes de verdade.

## Status atual

- Fase 0 concluida: documentos revisados, escopo do MVP consolidado e validacao manual confirmada.
- Fase 1 concluida: modulo de Salarios removido do codigo ativo, menu, usuario, IoC e migrations de limpeza criadas.
- Fase 2 concluida: modulo `PLC` criado no catalogo, policy adicionada, rota protegida e feature Angular inicial criada.
- Fase 3 concluida: modelo persistente de Planejamento de Custo criado com API, regras basicas, relacionamento com departamento e migrations.
- Fase 4 concluida: tela de listagem, cadastro, edicao e exclusao de planejamentos sem cargos criada.
- Fase 5 concluida: detalhamento por cargo criado com entidade, migrations, validacoes de limite e grid no cadastro.
- Fase 6 concluida: alternancia de modos criada com confirmacao para descarte de detalhes e reabertura limpa do detalhamento.
- Fase 7 concluida: relatorio geral por ano criado com departamentos, cargos detalhados, alertas e informacoes neutras.
- Fase 8 concluida: departamento e cargo vinculados a planejamento atual ou futuro passam a ter bloqueios estruturais.
- Fase 9 em validacao manual: revisao final tecnica concluida; testes manuais ficam a cargo do usuario.

## Etapa atual de aprofundamento tecnico

Depois da validacao manual inicial, a evolucao tecnica passou a ocorrer em ciclos menores para proteger o fluxo ja validado:

- Fase 1 concluida: cadastro de planejamento protegido por testes e regras de preparacao aprofundadas.
- Fase 2 concluida: relatorio geral protegido por testes, com montagem analitica e impressao HTML separadas.
- Fase 3 concluida: regra de estrutura planejada concentrada em politica propria.
- Fase 4 concluida: estado do formulario Angular de planejamento concentrado em modulo proprio.
- Fase 5 concluida: caminho para permissoes por acao documentado e acoes futuras do `PLC` nomeadas sem alterar o comportamento do MVP.

## Fase 0 - Preparacao e congelamento do escopo

**Tipo**: HITL  
**Status**: concluida  
**Bloqueada por**: nenhuma  
**Bloqueia**: todas as fases seguintes

### Objetivo

Confirmar que os documentos atuais sao a base da implementacao e que nenhum requisito novo sera misturado no MVP sem reavaliacao.

### Escopo

- Revisar `docs/prd-planejamento-custos-mvp.md`.
- Revisar `docs/planejamento-custos-mvp.md`.
- Revisar `docs/planejamento-custos-pos-mvp.md`.
- Revisar `docs/adr/tracker/0001-substituir-salarios-por-planejamento-de-custos.md`.
- Confirmar que validacao sera manual.

### Criterios de conclusao

- Escopo do MVP aprovado.
- Itens pos-MVP continuam fora da implementacao.
- Validacao manual confirmada para esta leva.

## Fase 1 - Remocao controlada do modulo de Salarios

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 0  
**Bloqueia**: Fases 2, 3, 4, 5, 6, 7 e 8

### Objetivo

Remover o conceito antigo de salarios antes de criar o `PLC`, evitando que as duas capacidades coexistam e gerem ambiguidade.

### Escopo

- Remover feature Angular de salarios.
- Remover rota `/atron/salarios`.
- Remover entrada `SAL` do menu/configuracao de modulos no front.
- Remover endpoint de salario do front.
- Remover controller, service, interface, repository, mapping, DTOs, validacoes e entidade de salario no backend.
- Remover referencias de IoC relacionadas a salario.
- Remover propriedade salarial de usuario.
- Remover dependencias de salario em fluxos de usuario.
- Criar migration para remover tabela/dados de salario e campo salarial de usuario.
- Remover modulo `SAL` do catalogo de modulos e limpar vinculos de perfil/modulo associados.

### Criterios de conclusao

- Sistema compila sem classes ou rotas de salario.
- Menu nao exibe Salarios.
- Usuario nao possui campo salarial.
- Banco nao mantem tabela/dados de salario nem vinculos de perfil com `SAL`.
- Login, menu, usuarios, cargos e departamentos continuam funcionando.

### Validacao manual

- Abrir o sistema e confirmar que Salarios nao aparece no menu.
- Tentar acessar rota antiga de salarios e confirmar que nao existe fluxo ativo.
- Criar/editar usuario e confirmar que salario nao aparece.
- Conferir perfis/modulos e confirmar ausencia de `SAL`.

## Fase 2 - Base do modulo `PLC` no catalogo e autorizacao

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 1  
**Bloqueia**: Fases 3, 4, 5, 6, 7 e 8

### Objetivo

Criar a existencia institucional do modulo Planejamento de Custos antes das telas e regras de negocio.

### Escopo

- Cadastrar modulo `PLC` como Planejamento de Custos.
- Criar policy por modulo para `PLC`.
- Adicionar configuracao do modulo no front.
- Criar rota base do `PLC` no Angular.
- Criar estrutura inicial da feature Angular com telas separadas para cadastro/manutencao e relatorio geral, ainda sem comportamento completo.

### Criterios de conclusao

- `PLC` aparece no catalogo de modulos.
- Perfil com acesso a `PLC` consegue ver o modulo no menu.
- Perfil sem acesso a `PLC` nao deve acessar a rota.
- Rota base do `PLC` carrega sem erro.

### Validacao manual

- Relacionar `PLC` a um perfil.
- Entrar com usuario desse perfil e confirmar exibicao no menu.
- Entrar sem permissao e confirmar bloqueio de acesso.

## Fase 3 - Modelo persistente de Planejamento de Custo

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 2  
**Bloqueia**: Fases 4, 5, 6, 7 e 8

### Objetivo

Criar a estrutura de dados minima para planejamento anual por departamento, sem detalhamento por cargo ainda.

### Escopo

- Criar entidade de planejamento de custo.
- Usar identificacao por id, codigo obrigatorio informado pelo usuario e descricao obrigatoria.
- Relacionar planejamento com departamento.
- Persistir ano, minimo e teto do departamento.
- Persistir flag de planejamento apenas por departamento.
- Garantir unicidade de departamento + ano.
- Criar DTOs, requests/responses, mapping, repository, service e controller.
- Criar endpoints de criar, listar, obter, editar e excluir.
- Criar migration para tabela de planejamento.

### Criterios de conclusao

- Criar planejamento por departamento/ano.
- Impedir segundo planejamento para mesmo departamento/ano.
- Impedir ano passado na criacao.
- Impedir edicao/exclusao de ano passado.
- Exigir codigo informado pelo usuario e manter codigo imutavel.
- Permitir editar descricao e valores.
- Impedir trocar departamento e ano.

### Validacao manual

- Criar planejamento para ano atual.
- Criar planejamento para ano futuro.
- Tentar criar planejamento para ano passado.
- Tentar duplicar departamento/ano.
- Editar descricao e valores.
- Confirmar que codigo nao muda.
- Excluir planejamento atual/futuro.

## Fase 4 - Tela de cadastro/manutencao sem cargos

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 3  
**Bloqueia**: Fases 5, 6, 7 e 8

### Objetivo

Entregar um fluxo utilizavel para planejamento apenas por departamento, antes de introduzir a complexidade de cargos.

### Escopo

- Criar tela de listagem/manutencao de planejamentos.
- Criar formulario com ano, departamento, descricao, minimo, teto e flag apenas por departamento.
- Consumir endpoints da Fase 3.
- Aplicar validacoes visuais basicas.
- Mostrar mensagens da API.
- Garantir navegacao criar/editar/voltar.

### Criterios de conclusao

- Usuario consegue criar planejamento apenas por departamento pela UI.
- Usuario consegue editar descricao, minimo e teto pela UI.
- Usuario consegue excluir planejamento atual/futuro pela UI.
- Anos passados aparecem como consulta, sem edicao/exclusao.

### Validacao manual

- Criar planejamento apenas por departamento pela tela.
- Editar planejamento.
- Excluir planejamento.
- Verificar mensagens de erro para minimo maior ou igual ao teto.
- Verificar bloqueio de ano passado.

## Fase 5 - Detalhamento por cargo e regras de limite

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 4  
**Bloqueia**: Fases 6, 7 e 8

### Objetivo

Adicionar detalhamento por cargos vinculados ao departamento, mantendo um unico planejamento por departamento/ano.

### Escopo

- Criar entidade/tabela de detalhe por cargo.
- Relacionar detalhe ao planejamento e ao cargo.
- Permitir cargo detalhado com minimo e teto.
- Permitir cargo nao detalhado.
- Representar cargo pendente de decisao quando aplicavel.
- Bloquear cargo detalhado sem minimo ou teto.
- Bloquear minimo maior ou igual ao teto no cargo.
- Bloquear soma dos tetos dos cargos acima do teto do departamento.
- Permitir soma dos tetos igual ao teto do departamento.
- Gerar alerta, nao bloqueio, para divergencias envolvendo minimos.
- Calcular percentual de ocupacao do teto.
- Exibir percentual no cadastro.

### Criterios de conclusao

- Tela lista cargos do departamento no mesmo planejamento.
- Usuario detalha alguns cargos e marca outros como nao detalhados.
- Cargo nao detalhado nao entra na soma.
- Todos os cargos nao detalhados bloqueiam e orientam usar planejamento apenas por departamento.
- Cargo pendente bloqueia salvamento quando usuario abre e salva planejamento detalhado.
- Percentual de ocupacao aparece corretamente.
- Acima de 100% bloqueia.

### Validacao manual

- Criar planejamento detalhado por cargo.
- Informar cargos detalhados e nao detalhados.
- Simular soma de tetos igual ao teto do departamento.
- Simular soma de tetos acima do teto do departamento.
- Simular cargo sem minimo/teto.
- Simular todos os cargos como nao detalhados.

## Fase 6 - Alternancia entre planejamento por departamento e detalhamento por cargo

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 5  
**Bloqueia**: Fases 7 e 8

### Objetivo

Implementar com seguranca a mudanca de modo do planejamento, sem deixar dados escondidos ou inconsistentes.

### Escopo

- Permitir mudar de apenas por departamento para detalhamento por cargo.
- Ao sair de apenas por departamento, iniciar cargos sem valores e exigir decisao explicita.
- Permitir mudar de detalhamento por cargo para apenas por departamento.
- Exigir confirmacao antes de descartar detalhes por cargo.
- Remover definitivamente detalhes por cargo ao confirmar mudanca para apenas por departamento.
- Garantir que cargos novos nao gerem pendencia quando planejamento estiver apenas por departamento.

### Criterios de conclusao

- Alternancia de modos funciona pela UI.
- Mudanca para apenas por departamento remove detalhes por cargo apos confirmacao.
- Mudanca para detalhamento reabre lista de cargos sem reaproveitar valores descartados.
- Planejamento apenas por departamento nao gera pendencia para cargos novos.

### Validacao manual

- Criar planejamento detalhado e mudar para apenas por departamento.
- Confirmar descarte dos detalhes.
- Voltar para detalhamento e verificar cargos limpos.
- Adicionar cargo novo ao departamento e confirmar comportamento em ambos os modos.

## Fase 7 - Relatorio geral de planejamento

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 6  
**Bloqueia**: Fase 8

### Objetivo

Criar a visao analitica do MVP, separada da manutencao, exibindo valores, percentuais e alertas dos planejamentos informados.

### Escopo

- Criar endpoint de relatorio geral por ano.
- Listar departamentos com planejamento informado para o ano analisado.
- Mostrar departamentos com planejamento.
- Mostrar dados do planejamento do departamento.
- Mostrar cargos detalhados.
- Nao mostrar cargos nao detalhados como linhas.
- Mostrar resumo neutro da quantidade de cargos nao detalhados.
- Mostrar planejamento apenas por departamento como informacao neutra.
- Mostrar cargos pendentes como alerta com identificacao.
- Mostrar valores absolutos.
- Mostrar percentual de ocupacao quando houver detalhamento por cargo.
- Criar tela separada de relatorio geral no Angular.

### Criterios de conclusao

- Relatorio permite escolher ano.
- Relatorio nao exibe departamentos sem planejamento.
- Relatorio exibe alertas e informacoes neutras corretamente.
- Relatorio exibe percentual apenas quando houver base de cargos detalhados.

### Validacao manual

- Consultar ano com departamentos planejados.
- Consultar ano com departamento sem planejamento e confirmar que ele nao aparece no relatorio.
- Consultar planejamento apenas por departamento.
- Consultar planejamento com cargo nao detalhado.
- Consultar planejamento com cargo pendente.

## Fase 8 - Protecao de Departamento e Cargo vinculados a planejamento

**Tipo**: AFK com validacao manual  
**Status**: concluida  
**Bloqueada por**: Fase 7  
**Bloqueia**: encerramento do MVP

### Objetivo

Evitar que alteracoes estruturais em departamento e cargo quebrem planejamentos atuais ou futuros.

### Escopo

- Bloquear remocao de departamento usado por planejamento atual ou futuro.
- Bloquear remocao de cargo usado por planejamento atual ou futuro.
- Permitir ajuste de descricao de departamento/cargo.
- Impedir alteracao de codigo quando houver planejamento atual ou futuro.
- Impedir mover cargo planejado para outro departamento.
- Ajustar mensagens de erro para explicar o motivo do bloqueio.

### Criterios de conclusao

- Departamento planejado nao pode ser removido.
- Cargo planejado nao pode ser removido.
- Descricao continua editavel.
- Codigo e vinculo estrutural ficam protegidos quando houver planejamento atual ou futuro.

### Validacao manual

- Criar planejamento atual para departamento.
- Tentar remover departamento.
- Tentar remover cargo detalhado.
- Tentar mover cargo para outro departamento.
- Editar somente descricao e confirmar sucesso.

## Fase 9 - Revisao final e estabilizacao manual

**Tipo**: HITL  
**Status**: em validacao manual  
**Bloqueada por**: Fase 8  
**Bloqueia**: entrega do MVP

### Objetivo

Validar o fluxo completo do MVP em uso real antes de considerar a implementacao concluida.

### Escopo

- Revisar fluxo de menu e permissao `PLC`.
- Revisar criacao, edicao e exclusao de planejamento.
- Revisar detalhamento por cargo.
- Revisar alternancia de modos.
- Revisar relatorio geral.
- Revisar bloqueios em departamento e cargo.
- Revisar ausencia completa do modulo de salarios.
- Registrar ajustes encontrados na validacao manual.

### Criterios de conclusao

- Fluxo principal validado manualmente.
- Salarios nao aparecem no produto.
- `PLC` atende o MVP documentado.
- Itens fora do MVP permanecem fora do escopo.

## Ordem resumida

- Fase 0 - Preparacao e congelamento do escopo.
- Fase 1 - Remocao controlada do modulo de Salarios.
- Fase 2 - Base do modulo `PLC` no catalogo e autorizacao.
- Fase 3 - Modelo persistente de Planejamento de Custo.
- Fase 4 - Tela de cadastro/manutencao sem cargos.
- Fase 5 - Detalhamento por cargo e regras de limite.
- Fase 6 - Alternancia entre planejamento por departamento e detalhamento por cargo.
- Fase 7 - Relatorio geral de planejamento.
- Fase 8 - Protecao de Departamento e Cargo vinculados a planejamento.
- Fase 9 - Revisao final e estabilizacao manual.
