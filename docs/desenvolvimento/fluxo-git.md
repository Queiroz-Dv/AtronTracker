# Fluxo Git

## Estado e direção

`main` foi iniciada no commit `413db0046001ae833e463bd1bc27211d7b9d8eeb`, o mesmo
da `RC2` remota em 27/08/2026. Não houve merge geral das branches históricas.
O histórico da RC2 foi preservado. Veja o [registro de transição](../operacao/transicao-branches-2026-08-27.md).

`main` recebe PRs curtos. O fluxo normal é branch de trabalho, revisão, CI e
Squash and merge. A [convenção de contribuição](../../CONTRIBUTING.md) contém os
nomes e comandos. Não existe obrigação de criar `develop` ou branch permanente
por versão. Uma branch temporária de estabilização só será criada quando houver
necessidade de manter uma candidata enquanto outras mudanças continuam.

## Atualizar uma branch de trabalho

Com o diretório limpo, obtenha as referências remotas e integre `origin/main` na
branch de trabalho. Resolva conflitos entendendo a regra de negócio e execute as
validações. Não use a cópia local desatualizada de outra branch como base.

Rebase pode ser usado em uma branch privada. Não reescreva uma branch
compartilhada nem force push na principal. Um PR integrado não autoriza remover
alterações não commitadas de outro worktree.

## Hotfix

Se `main` tiver mudanças ainda não publicadas, parta do commit/tag realmente em
produção. Valide e publique a correção de forma controlada; depois incorpore a
mesma correção à principal por PR para não perdê-la na próxima entrega.

## Arquivo e restauração

Tags `archive/20260827/remote-*` preservam pontas de branches remotas retiradas do
fluxo. Elas não são versões de produto, não comprovam publicação e não devem
disparar deploy. Tags `vX.Y.Z` têm outro significado e seguem o guia de releases.

Para consultar uma linha arquivada, sem reintroduzi-la na principal:

```powershell
git fetch origin --tags
git switch -c recuperar/analise-historica archive/20260827/remote-1.2
```

Branches locais antigas podem conter histórico distinto e configurações
sensíveis. Seu backup permanece privado; não use `git push --mirror` ou
`git push --tags` para publicá-lo. Publique somente referências explicitamente
selecionadas e verificadas.

## Proteções

A principal deve exigir PR, resolução de discussões e o check `Qualidade`, sem
force push ou exclusão. Não exigir aprovação de outra pessoa enquanto houver
somente um mantenedor. As proteções são configuração do GitHub, não consequência
automática da existência deste documento. O registro da transição informa a
situação efetivamente verificada.
