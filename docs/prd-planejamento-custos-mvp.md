# PRD: Planejamento de Custos - MVP

## Problem Statement

O AtronRC possui um modulo de salarios separado e tambem dados salariais associados ao usuario. Isso cria redundancia de modulo, mistura identidade/acesso de usuario com informacao remuneratoria sensivel e aumenta o risco de exposicao indevida em fluxos comuns do sistema.

O usuario precisa remover o conceito de salario individual do cadastro e substituir o modulo antigo por uma capacidade mais adequada: planejamento agregado de custos por departamento e por cargos vinculados ao departamento, com regras claras de minimo, teto, alertas e relatorio geral.

## Solution

Criar o modulo **Planejamento de Custos**, com codigo canonico `PLC`, para cadastrar, editar, excluir e analisar planejamentos anuais de custo da empresa.

O modulo deve substituir completamente o modulo de salarios. Os dados antigos de salario nao devem ser migrados para o novo planejamento. O novo modulo trabalha com custo total planejado, nao com salario individual, custo por pessoa ou custo por vaga.

O MVP deve permitir criar um planejamento anual por departamento, informar minimo e teto do departamento, detalhar custos por cargos vinculados ao departamento quando aplicavel, validar limites, exibir percentual de ocupacao do teto e apresentar um relatorio geral por ano com alertas e lacunas de planejamento.

## User Stories

1. Como usuario com acesso ao modulo `PLC`, quero visualizar o modulo Planejamento de Custos, para que eu possa acessar os planejamentos da empresa.
2. Como usuario com acesso ao modulo `PLC`, quero criar um planejamento anual por departamento, para organizar custos previstos por area.
3. Como usuario com acesso ao modulo `PLC`, quero informar uma descricao obrigatoria no planejamento, para identificar o registro com uma linguagem de negocio.
4. Como usuario com acesso ao modulo `PLC`, quero que o codigo do planejamento seja gerado pelo sistema, para evitar inconsistencias manuais.
5. Como usuario com acesso ao modulo `PLC`, quero que o codigo do planejamento nao mude depois da criacao, para manter a identidade do registro.
6. Como usuario com acesso ao modulo `PLC`, quero informar minimo e teto do departamento, para definir a faixa de custo planejado.
7. Como usuario com acesso ao modulo `PLC`, quero ser impedido de salvar minimo maior ou igual ao teto, para evitar faixas invalidas.
8. Como usuario com acesso ao modulo `PLC`, quero criar planejamento para o ano atual ou anos futuros, para evitar cadastro retroativo acidental.
9. Como usuario com acesso ao modulo `PLC`, quero consultar planejamentos de anos passados, para analisar historico disponivel.
10. Como usuario com acesso ao modulo `PLC`, quero ser impedido de editar planejamentos de anos passados, para preservar dados historicos sem auditoria.
11. Como usuario com acesso ao modulo `PLC`, quero que exista no maximo um planejamento por departamento e ano, para evitar planejamentos concorrentes.
12. Como usuario com acesso ao modulo `PLC`, quero editar descricao, valores e detalhes por cargo de planejamentos atuais ou futuros, para corrigir e evoluir o planejamento.
13. Como usuario com acesso ao modulo `PLC`, quero excluir definitivamente planejamentos atuais ou futuros, para remover cadastros incorretos.
14. Como usuario com acesso ao modulo `PLC`, quero que a exclusao do planejamento remova seus detalhes por cargo, para evitar dados sem planejamento pai.
15. Como usuario com acesso ao modulo `PLC`, quero escolher que um planejamento seja apenas por departamento, para planejar custos sem detalhar cargos.
16. Como usuario com acesso ao modulo `PLC`, quero que o detalhamento por cargo fique oculto ou desabilitado quando o planejamento for apenas por departamento, para evitar preenchimento contraditorio.
17. Como usuario com acesso ao modulo `PLC`, quero mudar de planejamento apenas por departamento para detalhamento por cargo, para detalhar custos posteriormente.
18. Como usuario com acesso ao modulo `PLC`, quero mudar de detalhamento por cargo para planejamento apenas por departamento, para simplificar o planejamento quando necessario.
19. Como usuario com acesso ao modulo `PLC`, quero receber confirmacao antes de descartar detalhes por cargo ao mudar para planejamento apenas por departamento, para evitar perda acidental de dados.
20. Como usuario com acesso ao modulo `PLC`, quero que cargos novos nao gerem pendencia quando o planejamento for apenas por departamento, para manter a decisao de nao detalhar cargos.
21. Como usuario com acesso ao modulo `PLC`, quero ver todos os cargos do departamento quando o planejamento for detalhado, para tomar decisao sobre cada cargo no mesmo planejamento.
22. Como usuario com acesso ao modulo `PLC`, quero marcar um cargo como detalhado informando minimo e teto, para incluir esse cargo no planejamento.
23. Como usuario com acesso ao modulo `PLC`, quero marcar um cargo como nao detalhado, para deixar claro que ele esta fora do detalhamento daquele planejamento.
24. Como usuario com acesso ao modulo `PLC`, quero que cargo nao detalhado nao entre na soma dos cargos, para nao tratar ausencia de detalhe como valor zero.
25. Como usuario com acesso ao modulo `PLC`, quero que cargo nao detalhado nao apareca como linha no relatorio geral, para manter o relatorio focado no que foi detalhado.
26. Como usuario com acesso ao modulo `PLC`, quero ver um resumo neutro da quantidade de cargos nao detalhados, para entender o escopo do planejamento.
27. Como usuario com acesso ao modulo `PLC`, quero ser impedido de salvar todos os cargos como nao detalhados, para usar a opcao mais intencional de planejamento apenas por departamento.
28. Como usuario com acesso ao modulo `PLC`, quero que cada cargo detalhado tenha minimo e teto obrigatorios, para garantir que a faixa do cargo esteja completa.
29. Como usuario com acesso ao modulo `PLC`, quero ser impedido de salvar cargo detalhado com minimo maior ou igual ao teto, para evitar faixa invalida.
30. Como usuario com acesso ao modulo `PLC`, quero que a soma dos tetos dos cargos detalhados nao ultrapasse o teto do departamento, para proteger o limite do planejamento.
31. Como usuario com acesso ao modulo `PLC`, quero salvar quando a soma dos tetos dos cargos detalhados for exatamente igual ao teto do departamento, para permitir ocupacao total do limite.
32. Como usuario com acesso ao modulo `PLC`, quero receber alerta quando houver divergencias envolvendo minimos, para revisar o planejamento sem ser bloqueado indevidamente.
33. Como usuario com acesso ao modulo `PLC`, quero ver o percentual de ocupacao do teto enquanto preencho cargos, para entender quanto do teto do departamento ja foi consumido.
34. Como usuario com acesso ao modulo `PLC`, quero ser bloqueado quando o percentual de ocupacao passar de 100%, para impedir estouro do teto.
35. Como usuario com acesso ao modulo `PLC`, quero que cargos novos em departamentos ja planejados aparecam como pendentes em planejamentos detalhados, para decidir se serao detalhados ou nao.
36. Como usuario com acesso ao modulo `PLC`, quero que cargos pendentes bloqueiem o salvamento quando eu abrir e salvar o planejamento, para nao perpetuar pendencias no cadastro.
37. Como usuario com acesso ao modulo `PLC`, quero que cargos pendentes aparecam como alerta no relatorio geral, para saber quais cargos precisam de decisao.
38. Como usuario com acesso ao modulo `PLC`, quero acessar uma tela separada de relatorio geral, para analisar planejamentos sem misturar com cadastro.
39. Como usuario com acesso ao modulo `PLC`, quero escolher o ano no relatorio geral, para analisar a competencia correta.
40. Como usuario com acesso ao modulo `PLC`, quero ver todos os departamentos no relatorio geral, para identificar tambem departamentos sem planejamento.
41. Como usuario com acesso ao modulo `PLC`, quero ver departamentos sem planejamento como alerta, para encontrar lacunas do planejamento anual.
42. Como usuario com acesso ao modulo `PLC`, quero ver valores absolutos no relatorio geral, para analisar os limites cadastrados.
43. Como usuario com acesso ao modulo `PLC`, quero ver percentual de ocupacao do teto no relatorio geral, para comparar rapidamente a saturacao dos departamentos.
44. Como usuario com acesso ao modulo `PLC`, quero que planejamento apenas por departamento apareca como informacao neutra, para nao tratar uma decisao explicita como pendencia.
45. Como usuario com acesso ao modulo `PLC`, quero que cargo nao detalhado apareca como informacao neutra, para nao tratar uma decisao explicita como erro.
46. Como usuario com acesso ao modulo `PLC`, quero ser impedido de remover departamento usado por planejamento atual ou futuro, para evitar divergencia estrutural.
47. Como usuario com acesso ao modulo `PLC`, quero ser impedido de remover cargo usado por planejamento atual ou futuro, para evitar detalhes orfaos.
48. Como usuario com acesso ao modulo `PLC`, quero permitir ajuste de descricao de departamento ou cargo mesmo quando houver planejamento, para corrigir nome sem quebrar identidade.
49. Como usuario com acesso ao modulo `PLC`, quero impedir alteracao de codigo de departamento ou cargo quando houver planejamento, para preservar identidade dos registros.
50. Como usuario com acesso ao modulo `PLC`, quero impedir mover cargo planejado para outro departamento, para preservar a leitura do planejamento.
51. Como administrador do sistema, quero remover a feature antiga de salarios do front e do backend, para eliminar codigo que nao representa mais o dominio.
52. Como administrador do sistema, quero remover dados antigos de salarios, para evitar manutencao de informacao remuneratoria sensivel desnecessaria.
53. Como administrador do sistema, quero remover vinculos de perfil com o modulo antigo de salarios, para que acesso antigo nao seja reaproveitado indevidamente.
54. Como administrador do sistema, quero cadastrar o modulo `PLC` no catalogo de modulos, para controlar acesso via perfis.
55. Como desenvolvedor, quero regras de planejamento concentradas em modulos bem definidos, para manter a implementacao organizada e facilitar validacoes futuras.

## Implementation Decisions

- O modulo antigo de salarios sera removido completamente, incluindo codigo, dados, permissao e referencias visuais.
- O novo modulo usara o codigo canonico `PLC`.
- O acesso do MVP seguira permissao por modulo, sem separacao inicial por acoes.
- O planejamento tera identidade composta por id e codigo, com descricao obrigatoria informada pelo usuario.
- O codigo do planejamento sera gerado pelo sistema e imutavel depois da criacao.
- A descricao sera editavel e nao sera unica.
- O par departamento/ano define a unicidade do planejamento no MVP.
- O ano e o departamento nao podem ser alterados depois da criacao.
- A competencia do MVP sera anual.
- O cadastro permitira apenas ano atual ou anos futuros.
- Anos passados serao somente consulta.
- Planejamentos atuais ou futuros poderao ser editados e excluidos definitivamente.
- A exclusao do planejamento removera tambem seus detalhes por cargo.
- O planejamento sempre parte do departamento.
- Cargo sempre sera detalhamento dentro do departamento, nunca planejamento isolado.
- O usuario podera marcar o planejamento como apenas por departamento.
- Planejamento apenas por departamento nao gera pendencia para cargos existentes ou novos.
- Alternar de detalhamento por cargo para apenas por departamento remove definitivamente os detalhes por cargo apos confirmacao.
- Alternar de apenas por departamento para detalhamento por cargo inicia os cargos sem valores e exige decisao explicita.
- Em planejamento detalhado, cada cargo pode ser detalhado, nao detalhado ou pendente de decisao.
- Cargo detalhado exige minimo e teto.
- Cargo nao detalhado nao entra na soma e nao aparece como linha no relatorio geral.
- Cargos pendentes bloqueiam salvamento quando o usuario abrir e salvar o planejamento.
- Cargos pendentes aparecem no relatorio geral como alerta identificando o cargo.
- Minimo e teto do departamento sao obrigatorios.
- Minimo e teto de cargo detalhado sao obrigatorios.
- Minimo maior ou igual ao teto bloqueia departamento e cargo.
- A soma dos tetos dos cargos detalhados nao pode ultrapassar o teto do departamento.
- Soma dos tetos igual ao teto do departamento e valida.
- Divergencias envolvendo minimos geram alerta, nao bloqueio.
- O percentual de ocupacao do teto sera calculado pela soma dos tetos dos cargos detalhados sobre o teto do departamento.
- Percentual de ocupacao sera exibido no cadastro e no relatorio geral.
- Percentual acima de 100% bloqueia o planejamento.
- O relatorio geral sera uma tela separada do cadastro.
- O relatorio geral listara todos os departamentos, incluindo os sem planejamento.
- Departamento sem planejamento no ano aparecera como alerta.
- Planejamento apenas por departamento e cargo nao detalhado aparecerao como informacao neutra.
- Departamento ou cargo usado por planejamento atual ou futuro nao podera ser removido.
- Quando houver planejamento atual ou futuro, codigo de departamento/cargo e vinculo cargo/departamento nao devem mudar.
- Descricao de departamento/cargo podera ser ajustada mesmo quando houver planejamento.
- O backend deve ter um modulo profundo de validacao do planejamento, com interface simples e isolada.
- O backend deve ter um modulo profundo de relatorio, responsavel por montar alertas, percentuais, departamentos sem planejamento e resumo de cargos.
- O backend deve ter um servico de geracao de codigo para planejamento, isolado da descricao.
- O backend deve proteger Departamento e Cargo contra alteracoes estruturais que quebrem planejamentos atuais ou futuros.
- O front Angular tera uma feature `PLC` com tela de cadastro/manutencao e tela de relatorio geral.
- O menu Angular deve trocar a entrada de salarios por Planejamento de Custos.
- O contrato de API deve expor operacoes de criar, listar, obter, editar, excluir e consultar relatorio geral.
- Nesta fase, a validacao sera manual.

## Validacao Manual

A validacao do MVP sera feita manualmente pelo usuario durante a implementacao e homologacao local.

## Out of Scope

- Consolidacao de planejamento.
- Copia de planejamento.
- Auditoria e historico detalhado.
- Competencias bimestrais, trimestrais ou semestrais.
- Detalhamento individual de custo por pessoa remunerada.
- Comparacao contra custo real.
- Centro de custo contabil formal.
- Modulo de contabilidade basico.
- Modulo de orcamento completo.
- Permissoes granulares por acao.
- Planejamento fixo ou valor-alvo.
- Retencao e limpeza automatica a cada 5 anos.
- Seletores avancados, cards, graficos, tendencias e alertas configuraveis.

## Further Notes

O `CONTEXT.md` e o ADR de substituicao de salarios por planejamento de custos sao fontes de verdade para linguagem e decisao arquitetural desta feature.

O primeiro corte de implementacao deve priorizar a remocao segura do conceito antigo de salarios, a criacao do modelo `PLC`, as regras de validacao bem definidas e a estrutura minima de cadastro e relatorio.

Como o projeto ainda nao possui uma base propria para esse tipo de cobertura, a validacao sera manual por enquanto.
