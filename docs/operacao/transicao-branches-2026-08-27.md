# Transição de branches em 27/08/2026

## Escopo e estado

Reorganização do Git e implantação da governança de contribuição. Não inclui
mudanças de domínio, migrations, notificações de versão ou reescrita do histórico.

`main` foi criada a partir de `origin/RC2` em
`413db0046001ae833e463bd1bc27211d7b9d8eeb`. Documentação e CI são preparados em
`codex/padronizar-fluxo-git`. O vínculo do Render permanece em `RC2` até conclusão
explícita da etapa operacional. Não há sincronização automática entre main e RC2.

## Inventário remoto anterior ao corte

| Branch | Commit | Destino |
| --- | --- | --- |
| RC2 | 413db0046001ae833e463bd1bc27211d7b9d8eeb | Preservar enquanto alimenta o Render |
| master | 030d162b1536d7aa1f9b429e81f13afc42e02c9d | Arquivo histórico após troca da branch padrão |
| 1.0 | 02af4e2d39871d2b21417ab23c560b8244fa4d7f | Arquivo histórico |
| 1.1 | 3a442a7358c371448c0f3b0f9eb234cba873c62e | Arquivo histórico |
| 1.2 | 1ffb9161a08db315a79b2b39ded5534e58f8888d | Arquivo histórico |
| 1.3 | 05a7c8cb2fa600daaf6660012405e55024bc8c56 | Arquivo histórico |
| 1.4.0 | 53e054b7c196ec70be61c43afe4ae9202807535a | Já incorporada à RC2; encerrar |
| 1.5.0 | 7094240bf773a775107c368ddc4a062c442bbf84 | Já incorporada à RC2; encerrar |
| 1.5.1 | e045d7b262a9460457dcbe38e3424c3d73662c1f | Já incorporada à RC2 pelo PR 73; encerrar |
| RC | 5647fd7bbb11f2ac3dfb82ce9dccf46d619119bb | Arquivo histórico |

As referências de arquivo remoto usam `archive/20260827/remote-<branch>`.
As dez tags foram publicadas e seus SHAs resolvidos foram conferidos no remoto.
Somente SHAs previamente públicos foram incluídos. Históricos locais
distintos permanecem em backup privado, pois há diferenças em configurações.

As nove branches históricas foram removidas do remoto após a conferência dos
arquivos. `main` é a branch padrão do GitHub. `RC2` permanece para publicação.
As branches locais obsoletas também foram retiradas após comparar suas pontas
com o bundle. Foi removido somente o registro de um worktree cujo diretório já
não existia; nenhum diretório de trabalho existente foi apagado.

`main` e `RC2` exigem PR, check `Qualidade` atualizado e resolução de discussões.
As regras incluem administradores e impedem force push/exclusão. Não exigem
aprovação de outra pessoa, pois o fluxo atual tem um mantenedor. Squash e merge
normal estão habilitados; branches de trabalho são removidas após o merge.

## Preservação local

Antes de qualquer limpeza, foi criado um bundle de todas as referências locais,
verificado com `git bundle verify`, e um inventário das pontas.

O worktree antigo em detached HEAD `5714154f5607073caa644fcf3b5cc0639b248150`
possui sete arquivos alterados/novos. Seus arquivos foram copiados byte a byte,
conferidos por SHA-256 e acompanhados de patches staged/unstaged. O worktree foi
preservado no lugar. Uma tag não protege conteúdo ainda não commitado.

O backup está fora do repositório público, com acesso local restrito. Não o
publique: ele inclui históricos e configurações que não foram auditados para
divulgação. A localização exata está registrada na entrega ao mantenedor.

## Baseline antes das alterações

`dotnet test AtronPlatform.sln --configuration Release -m:1
-p:UseSharedCompilation=false` encontrou contratos desatualizados em dois testes:

- `ModuloHandlerTests`: usa `ModuloPolicies.AcaoAcessar` e construtor de dois
  argumentos que não existem no contrato atual.
- `ExecutarGeracaoProdutosLoteCaseTests`: usa construtor anterior do caso de uso.

Shared: 32 testes aprovados; Notificações: 16; Platform: 63. Tracker e Stock não
concluíram a compilação dos testes. Isso é baseline, não falha criada pelo CI.
Qualquer correção adicional deve preservar o código de produção e ser registrada.

O build Angular de produção passou localmente. Jasmine e fluxos autenticados
não foram executados. A autorização para corrigir os dois arquivos de testes
foi solicitada; a falha não foi contornada na configuração de CI.

O [PR 74](https://github.com/Queiroz-Dv/AtronTracker/pull/74) está em rascunho.
Na [primeira execução do CI](https://github.com/Queiroz-Dv/AtronTracker/actions/runs/33098034570),
o frontend passou, o backend falhou e `Qualidade` bloqueou a integração. Os
erros de compilação correspondem aos dois arquivos identificados na baseline.
As actions foram atualizadas para versões atuais, fixadas por SHA, após o
runner alertar sobre a descontinuação do runtime das versões anteriores.
Consulte os checks da ponta atual do PR; uma execução anterior não valida um
commit posterior.

## Render verificado, sem alteração

O painel identifica o ambiente como **Homologação**. API (`Atron Platform`) e
frontend (`Atron Platform Front`) estão na `RC2`, com auto-deploy `On Commit`.
Nos dois serviços, o último commit implantado com sucesso é
`413db0046001ae833e463bd1bc27211d7b9d8eeb`, com status Live, publicado em
26/08/2026. Essa conferência é do painel, não uma validação funcional autenticada.

Não houve mudança nas configurações do Render nem acionamento de deploy nesta
etapa. A troca para main depende da resolução da baseline, CI verde e confirmação
operacional. RC2 não deve ser excluída antes disso.

## Pendências para encerrar a transição operacional

- [x] Verificar os checks do PR da governança e registrar seu resultado inicial.
- [ ] Corrigir a baseline em escopo autorizado e obter CI verde antes do merge.
- [x] Configurar e conferir branch padrão e proteções no GitHub.
- [x] Publicar/verificar arquivos históricos antes de remover branches remotas.
- [x] Registrar os SHAs em execução na API e no frontend.
- [ ] Concluir a troca do Render para main e verificar ambos os componentes.
- [ ] Somente então encerrar RC2.

O histórico de Git não comprova qual commit está publicado. Uma falha de CI
não deve ser escondida, e a remoção de branches não deve disparar deploy.
