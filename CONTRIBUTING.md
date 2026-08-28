# Contribuindo com o Atron

## Branches e publicação

`main` é a linha de integração do novo fluxo. Crie branches curtas a partir dela,
abra um pull request e integre somente alterações revisadas e verificadas.

Durante a transição de 27/08/2026, o Render continua acompanhando `RC2`.
Não faça merge nem push em `RC2` apenas para sincronizar branches: isso pode
iniciar um deploy. Consulte o [registro da transição](docs/operacao/transicao-branches-2026-08-27.md).

| Tipo | Exemplo |
| --- | --- |
| Funcionalidade | `feat/stock-recebimento` |
| Correção | `fix/stock-entrada-duplicada` |
| Refatoração | `refactor/tracker-atribuicao` |
| Documentação | `docs/fluxo-de-publicacao` |
| Automação | `ci/validacao-backend` |
| Correção urgente | `hotfix/correcao-login` |

Branches criadas por agentes usam `codex/`, por exemplo
`codex/feat-stock-recebimento`. Não reutilize branches numeradas para acumular
entregas sem relação. Versões publicadas são identificadas por tags `vX.Y.Z`.

```powershell
git fetch origin
git switch main
git pull --ff-only origin main
git switch -c feat/stock-recebimento
```

Execute esses comandos somente após preservar qualquer alteração local.
Nunca use `reset --hard`, `clean` ou force push como rotina de sincronização.

## Commits e pull requests

Use Conventional Commits, com descrição em português e escopo quando útil:

```text
feat(stock): adicionar recebimento parcial
fix(stock): impedir entrada duplicada
refactor(tracker): separar regras de atribuicao
docs: documentar o processo de publicacao
ci: validar backend e frontend nos pull requests
```

Use `!` e explique `BREAKING CHANGE` quando houver incompatibilidade de contrato.
Uma refatoração grande não é, por tamanho, uma quebra de compatibilidade.

O PR deve explicar problema, solução, validação, impacto em dados/permissões e
recuperação. Separe mudanças sem relação. Código, testes e documentação do mesmo
comportamento podem estar no mesmo PR. Revise também código produzido por IA.

Prefira **Squash and merge** para branches de trabalho: o título do PR será a
mensagem da entrega. Encerre a branch após o merge. Para uma promoção temporária
`main -> RC2`, use merge normal, preservando a ancestralidade entre as duas linhas.

## Verificações

A esteira inicial executa os testes .NET e o build de produção do Angular:

```powershell
dotnet test AtronPlatform.sln --configuration Release -m:1 -p:UseSharedCompilation=false
```

```powershell
cd AtronFront
npm ci
npm run build -- --configuration production
```

O build Angular não executa Jasmine. Testes de navegador e fluxos autenticados
devem ser planejados conforme a mudança; não estão comprovados pelo build.
Consulte a [arquitetura de testes](docs/arquitetura-testes.md).

Antes de integrar:

- [ ] Escopo revisado e alterações paralelas preservadas.
- [ ] Checks aplicáveis concluídos com sucesso; falhas preexistentes identificadas.
- [ ] Contratos, autorização, dados e consumidores avaliados.
- [ ] Migration e recuperação descritas, quando aplicáveis.
- [ ] Documentação atualizada e novidade para o usuário identificada, se houver.
- [ ] Nenhuma credencial, log sensível ou configuração pessoal incluída.

Não ignore checks falhando para publicar uma mudança funcional. Uma baseline
quebrada deve ser corrigida em escopo explícito, sem mascarar erros na esteira.

## Documentação e releases

- [Índice da documentação](docs/README.md).
- [Fluxo Git e recuperação de branches](docs/desenvolvimento/fluxo-git.md).
- [Publicação, versões e release notes](docs/operacao/releases.md).
- [Notas de versão](docs/releases/README.md).

Não publique uma release nem notifique usuários apenas porque um PR foi integrado.
Primeiro confirme o commit em execução e verifique o funcionamento publicado.
