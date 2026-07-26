# Plano de refatoração dos serviços de aplicação

## Objetivo

Aplicar gradualmente o padrão de responsabilidades dos serviços de aplicação
sem alterar contratos HTTP nem misturar mudanças funcionais com refatoração.
Cada fase deve deixar um ponto de encaixe testável, preservar os fluxos
validados e terminar antes de iniciar o próximo conceito.

O padrão de decisão está em
[Padrão de responsabilidades dos serviços de aplicação](./padrao-responsabilidades-servicos.md).

## Regras de execução

- Executar somente a fase autorizada.
- Preservar comportamento, mensagens, políticas e contratos públicos, salvo
  requisito funcional explícito.
- Antes de extrair, identificar o conceito dono da regra e a interface mínima
  usada pelo caso de uso.
- Não criar classes apenas para redistribuir linhas; a extração precisa reduzir
  uma razão real de mudança.
- Criar ou ajustar testes no ponto de encaixe novo antes de remover o caminho
  anterior.
- Ao fim de cada fase, executar os testes diretamente relacionados e o build do
  `WebApi` com diretório de saída alternativo quando necessário.

## Fase 0 - Contrato arquitetural

**Status:** concluída.

- Registrar a responsabilidade de orquestração dos serviços.
- Diferenciar regras de domínio, validações de aplicação, mapeadores,
  auxiliares e integrações.
- Registrar a decisão no ADR 0004 e no glossário do projeto.

**Critério de conclusão:** documentação aprovada e critérios reutilizáveis em
revisões futuras.

## Fase 1 - Ciclo de obtenção de tarefa

**Status:** concluída.

- Tirar de `TarefaService` a elegibilidade para obter tarefa, a resolução de
  aprovador, a criação de solicitação, o mapeamento de solicitação e as
  notificações internas.
- Manter `TarefaService` como entrada dos contratos públicos do módulo.
- Criar a solicitação pendente na entidade `SolicitacaoObtencaoTarefa`.

**Pontos de encaixe criados:** `ITarefaObtencaoService`,
`ITarefaObtencaoValidador`, `IAprovadorObtencaoTarefaResolver`,
`ISolicitacaoObtencaoTarefaMapeador` e `ITarefaNotificacaoInternaService`.

**Critério de conclusão:** build do `WebApi` e testes de tarefa aprovados sem
mudança de comportamento observável.

## Fase 2 - Fechamento de responsabilidades residuais de tarefas

**Status:** concluída.

- Revisar se consulta de visões (`Meu quadro`, `Equipe` e disponíveis) e
  configurações de notificação continuam crescendo no mesmo serviço.
- Extrair o contexto do usuário logado para `ITarefaUsuarioAtualService`,
  reutilizado pelos fluxos de consulta e obtenção.
- Extrair as preferências de notificação para
  `ITarefaConfiguracoesService`, mantendo a atualização e o contrato de saída
  em um único caso de uso.
- Remover a seleção repetida de departamentos e cargos para extensões coesas
  de `Usuario`, reutilizadas também pela validação de obtenção.

**Não inclui:** alterar regras de obtenção, permissões, textos ou fluxos do
front-end.

**Critério de conclusão:** atingido. `TarefaService` não resolve mais usuário
logado nem atualiza configurações diretamente; cada regra extraída possui dono
explícito e testes focados.

## Fase 3 - Diagnóstico protegido de perfis de acesso

**Status:** concluída.

- Congelar o comportamento atual de `PerfilDeAcessoService` com testes de
  criação, alteração, remoção, associação de usuários e invalidação de cache.
- Mapear os contratos usados por controller, autenticação e cache.
- Separar no plano interno três conceitos: perfil e módulos, associação de
  usuários ao perfil e invalidação de acesso.

**Não inclui:** redefinir políticas de acesso nem alterar o modelo de perfis.

**Critério de conclusão:** testes descrevem o comportamento externo e cada
dependência tem responsabilidade identificada.

**Resultado:** quatro testes de contrato foram adicionados para os fluxos de
criação, atualização, remoção e associação. Os limites de extração e os
contratos envolvidos estão registrados no
[inventário de perfil de acesso](./inventario-perfil-de-acesso.md).

## Fase 4 - Preparação de perfil e módulos

**Status:** concluída.

- Extrair validação do comando de perfil e resolução dos módulos para um
  preparador de perfil.
- Mover a montagem de `PerfilDeAcessoModulo` para o conceito de perfil ou para
  uma fábrica/preparador que receba módulos já resolvidos.
- Deixar `PerfilDeAcessoService` apenas coordenar persistência e resultado.

**Critério de conclusão:** criação e atualização de perfil não contêm laços de
montagem, busca de módulo e validação espalhados no orquestrador.

**Resultado:** `IPerfilDeAcessoPreparacaoService` passou a validar o comando,
mapear o perfil e montar seus módulos. `PerfilDeAcessoService` apenas coordena
preparação, persistência, invalidação de acesso e resposta.

## Fase 5 - Associação de usuários e invalidação de acesso

**Status:** concluída.

- Extrair a sincronização de relacionamentos perfil-usuário para colaborador
  próprio.
- Centralizar a lista de usuários afetados e a invalidação de cache de acesso.
- Proteger o fluxo contra associação parcial ou cache desatualizado.

**Critério de conclusão:** mudanças no vínculo de usuários e mudanças no cache
podem evoluir sem alterar a preparação do perfil.

**Resultado:** `IPerfilDeAcessoUsuarioRelacionamentoService` valida os usuários
antes de substituir os vínculos e grava a substituição em escopo transacional.
`IPerfilDeAcessoCacheInvalidator` centraliza a invalidação posterior ao sucesso.

## Fase 6 - Diagnóstico protegido de registro de usuário

**Status:** concluída.

- Congelar em testes os fluxos de cadastro, confirmação de e-mail, solicitação
  de recuperação e troca de senha.
- Inventariar efeitos externos: Identity, usuário de negócio, perfil, cache,
  confirmação e e-mail.
- Confirmar que a normalização de código permanece no limite da aplicação,
  sem ser repetida nos repositórios.

**Critério de conclusão:** cada fluxo possui cobertura focada e os efeitos
adicionais estão documentados.

**Resultado:** os fluxos, seus efeitos externos e a normalização no limite da
aplicação estão inventariados em
[inventário de registro de usuário](./inventario-registro-usuario.md).

## Fase 7 - Cadastro e confirmação de e-mail

**Status:** concluída.

- Separar os casos de uso de registrar usuário e confirmar e-mail.
- Manter código de confirmação e composição de e-mail como colaboradores.
- Fazer do envio de e-mail de cadastro um aviso quando a regra atual permitir,
  sem ocultar falha de persistência.

**Critério de conclusão:** mudanças no convite/confirmação não alteram o fluxo
de cadastro por acoplamento acidental.

**Resultado:** `ICadastroUsuarioService` passou a concentrar cadastro e
confirmação, preservando a confirmação e os avisos de e-mail. A fachada mantém
o contrato original de `IRegistroUsuarioService`.

## Fase 8 - Recuperação e troca de senha

**Status:** concluída.

- Separar solicitação de recuperação e troca de senha em casos de uso próprios.
- Concentrar a criação e leitura de dados temporários, URL e expiração em
  colaborador explícito.
- Preservar o contrato de cache, criptografia e mensagens de segurança.

**Critério de conclusão:** recuperação de senha tem testes próprios e não
compartilha detalhes internos com cadastro ou confirmação.

**Resultado:** `IRecuperacaoSenhaService` passou a concentrar a criação e o
consumo de dados temporários, a recuperação e a troca de senha.

## Fase 9 - Estrutura organizacional: departamento e cargo

**Status:** concluída.

- Revisar juntos `DepartamentoService` e `CargoService` para não criar duas
  abstrações concorrentes para a mesma estrutura.
- Injetar a política de estrutura planejada em vez de instanciá-la dentro dos
  serviços.
- Extrair regras de vínculo de gestor, resolução de departamento e bloqueio de
  remoção para políticas ou preparadores nomeados.

**Critério de conclusão:** serviços coordenam os comandos; regras estruturais
ficam em políticas reutilizáveis e testadas.

**Resultado:** `EstruturaPlanejadaPolicy` deixou de ser criada dentro de
`DepartamentoService` e `CargoService`; ela é fornecida pela composição da
aplicação e permanece como ponto de encaixe único das regras de planejamento.

## Fase 10 - Varredura de consolidação

**Status:** concluída.

- Reexecutar a varredura sobre todos os serviços de aplicação.
- Classificar cada candidato em: corrigido, sem extração necessária, próximo
  ciclo ou dívida documentada.
- Remover abstrações que tenham se tornado apenas camadas de passagem.
- Atualizar o ADR e este plano com a decisão de encerrar ou continuar o ciclo.

**Critério de conclusão:** serviços revisados têm uma responsabilidade principal
verificável, testes nos pontos de encaixe e nenhuma extração especulativa.

**Resultado:** tarefas, perfis de acesso, registro de usuário e estrutura
organizacional foram tratados neste ciclo. Os pontos restantes devem ser
avaliados em um novo ciclo, com diagnóstico protegido antes de nova extração.

## Ordem recomendada

1. Fase 2, para fechar o módulo de tarefas já aberto.
2. Fases 3 a 5, para reduzir o maior acúmulo atual em `PerfilDeAcessoService`.
3. Fases 6 a 8, para separar os casos de uso de acesso de usuário.
4. Fase 9, para alinhar departamento e cargo em conjunto.
5. Fase 10, para medir o resultado e definir o próximo ciclo.
