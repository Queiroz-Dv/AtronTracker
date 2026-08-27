# Atron Front

Interface Angular da **Atron Platform**, mantida neste mesmo repositório. A navegação organiza as rotinas em Tracker e Stock e apresenta Sales como uma área planejada, atualmente desabilitada.

As capturas e a apresentação do produto estão no [README principal](../README.md). Para preparar banco, API e credenciais, siga o [guia de execução local](../docs/desenvolvimento/execucao-local.md).

## Tecnologias

As dependências atuais incluem Angular 19, Angular Material, TypeScript e RxJS. O [package.json](package.json) define os scripts e as faixas de versão; o [package-lock.json](package-lock.json) registra as versões instaláveis. A CI utiliza Node.js 22.

## Executar em desenvolvimento

Com a API local configurada e em execução, abra um terminal na pasta `AtronFront`:

```powershell
npm ci
npm start
```

O servidor de desenvolvimento abre a interface normalmente em `http://localhost:4200`. O [ambiente de desenvolvimento](src/environments/environment.development.ts) aponta para `https://localhost:7280/`, correspondente ao perfil atual de `AtronPlatform.WebApi`.

A conta utilizada precisa existir na base de desenvolvimento e possuir os acessos necessários. A interface não cria uma base pronta para demonstração.

## Scripts

| Script | Uso |
| --- | --- |
| `npm start` | Servidor Angular de desenvolvimento, com abertura do navegador. |
| `npm run build` | Build da aplicação; a configuração padrão de build é produção. |
| `npm run watch` | Build contínuo em configuração de desenvolvimento. |
| `npm test` | Suíte configurada com Jasmine/Karma. Não faz parte da CI atual. |

As configurações e substituições de arquivos estão em [angular.json](angular.json). **O ambiente de produção aponta para a API publicada no Render.** Use desenvolvimento para testar cadastros contra a API local.

A CI executa `npm ci` e `npm run build -- --configuration production`. Isso verifica a compilação, não uma navegação autenticada nem o resultado das operações no banco.

## Organização

| Diretório | Conteúdo |
| --- | --- |
| `src/app/core` | Configuração e recursos centrais da interface. |
| `src/app/features` | Telas e funcionalidades organizadas por área. |
| `src/app/shared` | Componentes e utilitários compartilhados. |
| `src/environments` | Endereços e configuração por ambiente. |

A [configuração das áreas](src/app/core/config/areas-plataforma.config.ts) define os grupos da plataforma. O [mapeamento de módulos](src/app/shared/utils/modulo-functions.util.ts) associa as rotinas às rotas. A exibição dos cartões considera os acessos da conta; a proteção das operações também deve ser aplicada no backend.

## Leitura complementar

- [Roteiro de avaliação](../docs/guia-avaliacao.md).
- [Documentação do Stock](../AtronPlatform/Modules/Stock/README.md).
- [Fluxo de contribuição](../CONTRIBUTING.md).
- [Workflow de CI](../.github/workflows/ci.yml).
