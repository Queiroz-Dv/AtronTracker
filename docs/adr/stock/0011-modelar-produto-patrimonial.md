# ADR 0011: Modelar Produto como patrimônio individual

## Status

Aceito em 24/08/2026. O marco de aprendizado foi cumprido e a implementação
das fases 5 a 9 foi autorizada na mesma data.

## Contexto

O Produto atual representa um item comercializável, exige Categoria e
Fornecedor e concentra regras no serviço de aplicação. A evolução aprovada do
Atron Stock precisa representar cada Produto como um patrimônio individual,
com Categoria opcional e vínculo opcional a um lote de origem.

## Decisão

### Produto

- o código será informado pelo usuário, normalizado para maiúsculas e comparado
  sem diferença entre maiúsculas e minúsculas;
- o Produto iniciará com status `Ativo`;
- o cadastro comum não alterará o status nem a data efetiva de baixa;
- a futura rotina de baixa será a única responsável pela transição de `Ativo`
  para `Baixado` e pelo preenchimento da data efetiva de baixa;
- Produto poderá existir sem Categoria;
- somente Categorias ativas poderão ser vinculadas a um novo Produto;
- Fornecedor deixará de fazer parte do contrato de Produto;
- Produto criado individualmente não terá lote.

### Categoria em uso

Uma Categoria será considerada em uso quando estiver vinculada a qualquer
Produto, inclusive patrimônio já baixado. A inativação será recusada, manterá a
Categoria ativa e registrará a tentativa no histórico.

### LoteProduto

- `LoteProduto` terá somente `Id` e `Codigo`;
- o primeiro código de um código-base no ano seguirá `{BASE}_{ANO}`;
- colisões usarão sufixo sequencial, como `{BASE}_{ANO}_2` e
  `{BASE}_{ANO}_3`;
- a solicitação não terá limite funcional máximo de Produtos e exigirá somente
  uma quantidade positiva;
- os Produtos usarão o código-base normalizado seguido de sequência iniciada
  em 1;
- quantidade e valor total serão derivados dos Produtos relacionados.

## Implementação incremental

1. proteger Categoria contra inativação quando estiver em uso;
2. reestruturar domínio e persistência de Produto;
3. criar casos de uso para Produto individual;
4. entregar o fluxo Angular de Produto individual;
5. parar para estudar e autorizar Processamentos em Lote antes de implementar
   worker, fila persistente ou endpoint assíncrono;
6. implementar a regra sem worker, persistir o processamento, aceitar a
   solicitação com `202 Accepted` e executar o fluxo simples em hosted service.

## Consequências

- códigos equivalentes por caixa não poderão coexistir;
- Categorias que já participaram de qualquer Produto permanecerão ativas;
- a remoção de Fornecedor do contrato exige rastrear entradas, vendas, estoque,
  mapeamentos, API, persistência e testes;
- `Baixado` pertence ao ciclo patrimonial e não será antecipado pela manutenção
  comum;
- volumes em lote não serão executados em uma requisição HTTP longa.

## Fora do escopo desta decisão

- regras funcionais de baixa, adição e depreciação;
- SignalR, Redis Streams, RabbitMQ, Hangfire, dispatcher ou handlers genéricos;
- generalização de Processamentos sem um segundo consumidor real;
- reserva atômica, recuperação, idempotência, blocos e tentativas da Fase 10.

## Estado da implementação em 24/08/2026

- a proteção de Categoria em uso foi implementada com recusa auditada;
- Produto passou a representar patrimônio individual atrás de fachada e casos
  de uso específicos;
- `LoteProduto` e a FK opcional foram modelados e a geração em lote passou a
  ser responsabilidade de `ExecutarGeracaoProdutosLoteCase`;
- o contrato de Produto removeu Fornecedor, tornou Categoria opcional e manteve
  data efetiva de baixa somente para leitura;
- a rotina `PRD` foi incluída no catálogo, na policy do host e no Angular;
- listagem, filtros, cadastro, edição e histórico foram implementados no front;
- as migrations patrimoniais, do processamento e da primeira fatia de
  confiabilidade foram aplicadas ao PostgreSQL/Supabase configurado;
- o fluxo autenticado com banco PostgreSQL e perfil `PRD` não foi validado;
- a solicitação assíncrona persiste o ator e retorna `202 Accepted` com o id;
- `GeracaoProdutosLoteWorker` cria escopos para executar o caso de uso e marcar
  conclusão ou falha;
- os endpoints de acompanhamento listam e detalham somente processamentos do
  solicitante autenticado, sem aceitar sua identidade no contrato HTTP;
- o Angular apresenta lista, detalhe e progresso por polling controlado, que é
  encerrado nos estados terminais, e permite abrir os Produtos pelo lote gerado;
- a Fase 10 foi iniciada com reserva atômica por `FOR UPDATE SKIP LOCKED`, lease
  recuperável, token de reserva e concorrência otimista;
- idempotência, blocos, tentativas progressivas, cancelamento, auditoria do ator
  original e notificações do processamento permanecem nas próximas fatias da
  Fase 10.

## Atualização em 26/08/2026

- o Angular passou a solicitar a geração em lote pelo mesmo formulário usado
  no cadastro individual, por meio da opção `Gerar por lote` e do campo de
  quantidade;
- a tela exclusiva de criação de lote permanece retirada, pois a solicitação
  pertence ao formulário de Produto;
- o acompanhamento foi disponibilizado em
  `/atron/produtos/processamentos`, com lista, detalhe, filtros, progresso por
  polling encerrado nos estados terminais e acesso aos Produtos pelo lote
  concluído;
- após uma solicitação aceita, o Angular abre o detalhe do processamento
  retornado pela API.
