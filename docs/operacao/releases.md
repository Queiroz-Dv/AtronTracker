# Publicação, versões e release notes

## Uma versão do produto

O Atron usa uma versão de produto, com referências ao commit e aos artefatos de
backend/frontend. O `version` do pacote Angular e o `v1` do Swagger não são, por
si só, a versão publicada do Atron.

Adote `MAJOR.MINOR.PATCH` a partir do próximo corte documentado. Declare os
contratos de compatibilidade: correção compatível incrementa PATCH; capacidade
compatível incrementa MINOR; quebra dos contratos definidos incrementa MAJOR.
Não renumere o histórico nem converta nomes antigos de branches em releases.

## Procedimento

1. Selecione um commit aprovado e confira os checks do SHA escolhido.
2. Escreva as notas em `docs/releases/X.Y.Z.md` usando o modelo existente.
3. Registre migrations, configuração, compatibilidade e recuperação em um
   checklist técnico separado do texto destinado ao usuário.
4. Identifique a candidata e os artefatos. Uma tag anotada `vX.Y.Z-rc.N` pode
   identificar uma candidata; não anuncie que ela está em produção.
5. Valide em homologação quando disponível. Sem esse ambiente, registre a
   limitação e execute uma publicação manual controlada.
6. Publique o commit escolhido na API e no frontend. Não suponha que o deploy
   mais recente corresponde à ponta da branch.
7. Confirme commit, saúde e fluxos autenticados relevantes. Verifique migrations
   e compatibilidade dos componentes que estiverem em versões diferentes.
8. Registre a tag final no commit efetivamente entregue e publique a GitHub
   Release com as notas revisadas. Preserve a referência do último estado estável.
9. Somente depois anuncie a atualização aos usuários.

Uma tag, um merge, um check verde e um deploy iniciado são evidências diferentes.
Não mova uma tag final publicada para outro commit. Se houver erro, registre a
retirada/correção e prepare uma nova versão.

## Render durante a transição

O vínculo ainda é `RC2` até que a troca seja registrada como concluída.
Não envie alterações para essa branch somente para testar o CI.

No corte para `main`, registre primeiro os commits em execução nos dois
serviços, confira que a versão escolhida é compatível e altere o vínculo de
forma controlada. É possível manter deploy manual por commit. A opção
`After CI Checks Pass` deve ser usada somente após verificar os checks reais,
pois conclusões skipped/neutral de jobs também podem ser aceitas pelo Render;
o job agregado `Qualidade` deve falhar quando backend/frontend não passarem.

O guia de [publicação no Render](../publicacao-render-brevo-supabase.md) contém
configuração e verificações operacionais. Rollback de código não desfaz
migrations automaticamente; planeje compatibilidade e recuperação dos dados.

## Conteúdo único para as novidades

As notas revisadas em `docs/releases/` são a fonte do texto de cada versão.
O changelog é um índice; GitHub Release e futura tela do Atron devem usar esse
conteúdo, evitando três descrições independentes. Não inclua segredos, dados
pessoais ou detalhes exploráveis de vulnerabilidades nas notas públicas.

## Notificação no Atron: próxima etapa, não implementada

A capacidade de plataforma de novidades deverá servir o conteúdo aprovado da
release e publicar avisos pela central existente. Requisitos:

- Registrar a versão como anunciável somente após deploy confirmado.
- Um aviso por versão e usuário, incluindo o destinatário na chave idempotente.
- Preservar leitura/exclusão e impedir duplicação em acessos concorrentes.
- Linkar a notificação ao detalhe permanente da release.
- Tratar usuários desconectados e versões retiradas, sem avisar sobre candidatas.
- Não buscar o GitHub pelo navegador para decidir qual versão está disponível.

Essa implementação não faz parte da limpeza de branches e não exige SignalR ou
RabbitMQ na primeira entrega.

## Referências

- [SemVer](https://semver.org/lang/pt-BR/).
- [GitHub Releases](https://docs.github.com/en/repositories/releasing-projects-on-github/about-releases).
- [Render Deploys](https://render.com/docs/deploys).
