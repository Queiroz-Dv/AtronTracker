# Inventário de responsabilidades de Perfil de Acesso

## Finalidade

Este inventário registra o comportamento preservado de
`PerfilDeAcessoService` antes da extração das responsabilidades previstas nas
Fases 4 e 5. Não altera políticas de autorização, contratos HTTP ou o modelo de
perfis.

## Entradas públicas

`PerfilDeAcessoController` exige autenticação para todas as operações. Criar,
alterar, consultar e remover perfis usam a política
`ModuloPolicies.PerfilDeAcesso`; os vínculos de perfil e usuário usam
`ModuloPolicies.RelacionamentoPerfilUsuario`.

O contrato `IPerfilDeAcessoService` expõe os fluxos de consulta, criação,
atualização, remoção, sincronização de usuários e consulta do relacionamento.
O controller preserva o contrato `Resultado`: falhas de comandos retornam
`BadRequest`, consulta inexistente retorna `NotFound` e sucessos retornam
`Ok`.

## Comportamentos protegidos

Os testes em `Application.Tests/PerfilDeAcessoServiceTests.cs` descrevem os
seguintes efeitos observáveis:

- criação sem módulo é recusada e não persiste o perfil;
- atualização válida persiste o perfil e invalida o cache dos usuários já
  associados;
- remoção invalida o cache somente depois da confirmação da persistência;
- sincronização de usuários persiste os vínculos e invalida o cache dos
  usuários afetados.

## Dependências e responsabilidades atuais

| Dependência | Papel observado | Destino de responsabilidade nas próximas fases |
| --- | --- | --- |
| `IAsyncApplicationMapService<PerfilDeAcessoDTO, PerfilDeAcesso>` | Mapeia DTO e entidade. | Permanece como colaborador do preparador ou do caso de uso. |
| `IModuloRepository` | Resolve cada módulo informado para montar o vínculo. | Preparação de perfil e módulos, Fase 4. |
| `IValidateModelService<PerfilDeAcesso>` e `Notifiable` | Valida a entidade e acumula mensagens. | Validação de comando e resultado, Fase 4. |
| `IPerfilDeAcessoRepository` | Persiste e consulta perfis e associações carregadas. | Orquestração do caso de uso. |
| `IPerfilDeAcessoUsuarioRepository` | Remove e cria vínculos perfil-usuário. | Sincronização de usuários, Fase 5. |
| `IUsuarioRepository` | Resolve usuários para formar o vínculo. | Sincronização de usuários, Fase 5. |
| `ICacheUsuarioService` | Remove a informação de acesso em cache dos códigos afetados. | Invalidador de acesso, Fase 5. |

## Limites de extração aprovados pelo diagnóstico

A Fase 4 pode extrair a checagem do comando, a resolução de módulos e a
montagem de `PerfilDeAcessoModulo` para um preparador coeso. A Fase 5 pode
extrair a substituição dos vínculos perfil-usuário e a coleta e invalidação dos
códigos de usuário afetados.

O serviço de aplicação continuará responsável por coordenar o fluxo, chamar a
persistência e traduzir mensagens para `Resultado`. A extração não deve mudar
as políticas aplicadas pelo controller nem antecipar uma alteração de regra de
acesso.

## Implementação das Fases 4 e 5

A preparação de perfil foi isolada em `IPerfilDeAcessoPreparacaoService`. Ela
valida o comando, mapeia o DTO, resolve os módulos e monta os vínculos
`PerfilDeAcessoModulo` antes de o caso de uso solicitar a persistência.

A substituição de vínculos de usuários foi isolada em
`IPerfilDeAcessoUsuarioSincronizacaoService`. Todos os usuários informados são
resolvidos antes de remover vínculos existentes. A remoção e as novas gravações
ocorrem em uma transação; o cache só é invalidado depois da confirmação do
escopo transacional.

`IPerfilDeAcessoCacheInvalidator` concentra a coleta, a filtragem e a remoção
das entradas de cache de acesso, reutilizada nas alterações de perfil, remoção
e sincronização de usuários.
