# Atron Stock

O Stock reúne produtos, patrimônio e estoque na plataforma Atron. A rotina atual de Produto trata cada registro como um **bem individual**, conforme o [ADR 0011](../../../docs/adr/stock/0011-modelar-produto-patrimonial.md).

## Estado do módulo

| Parte | Situação |
| --- | --- |
| Categorias | Rotina implementada e disponível na navegação conforme as permissões. |
| Produtos patrimoniais | Cadastro, atualização e consulta de bens individuais, com classificação opcional por categorias. |
| Geração por lote | Solicitação persistida para geração assíncrona, com consulta do estado de processamento e dos produtos resultantes. |
| Estoque, entradas e vendas do modelo anterior | Estruturas e endpoints ainda presentes no código; precisam ser avaliados durante a evolução patrimonial. |
| Recebimento pendente, novas movimentações patrimoniais e relatórios | Regras a detalhar e implementar. Não são funcionalidades concluídas apenas pela existência de Produto ou de auditoria transversal. |
| Integração com Sales | Planejada. Sales ainda não está implementado como módulo operacional da plataforma. |

A navegação atual do Stock apresenta Produtos e Categorias. Não há uma tela completa do novo ciclo de recebimento e movimentação patrimonial sendo demonstrada pelas capturas do [README principal](../../../README.md).

## Modelo patrimonial e geração em lote

| Entidade | Responsabilidade atual |
| --- | --- |
| `Produto` | Bem individual com código, descrição, aquisição, valor, status e categorias opcionais. |
| `LoteProduto` | Origem opcional comum aos produtos gerados em lote; não substitui a identidade de cada bem. |
| `ProcessamentoProdutoLote` | Solicitação persistida de geração e seu estado de execução. |
| `Estoque` | Saldo do modelo anterior de estoque. |
| `MovimentacaoEstoque` | Movimento do modelo anterior, com tipo, quantidade, data, observação, origem e referência de transação. |

Um produto cadastrado individualmente pode não possuir lote. Ao solicitar geração em lote, o sistema registra o trabalho e retorna sua identificação; o worker executa a geração posteriormente.

O modelo atual de `MovimentacaoEstoque` possui propriedades mutáveis e não contém, por si só, um campo de ator autenticado. Portanto, ele não deve ser apresentado como um histórico patrimonial imutável e completo. Auditoria técnica e histórico de negócio precisam ter seus contratos explicitados na evolução do módulo.

## Organização do código

Os projetos ficam em `AtronPlatform/Modules/Stock`, preservando os nomes `AtronStock.Domain`, `AtronStock.Application` e `AtronStock.Infrastructure`.

| Camada | Responsabilidades e exemplos |
| --- | --- |
| Domain | Entidades, enums e contratos de repositórios. |
| Application | DTOs, validações, mapeamentos, serviços de aplicação e casos de uso. `ProdutoService` é uma fachada; `EstoqueService` também pertence a esta camada. |
| Infrastructure | `StockDbContext`, mapeamentos de persistência, repositórios, migrations, composição `AddStockModule` e `GeracaoProdutosLoteWorker`. |
| Borda HTTP | Controllers em `AtronPlatform/WebApi/Controllers/Stock`, publicados pelo host único `AtronPlatform.WebApi`. |

Na rotina de Produto, a organização principal é:

```text
ProdutoController → ProdutoService → caso de uso → contrato de repositório
                                                      ↓
                                            implementação na Infrastructure
```

[ProdutoService](Application/Services/ProdutoService.cs) delega a criação, atualização, consulta e aceitação de lotes aos respectivos casos de uso. [ProcessamentoProdutoService](Application/Services/ProcessamentoProdutoService.cs) delega as consultas de acompanhamento, restritas ao solicitante obtido de `IUserAccessor`.

O [worker de geração](Infrastructure/Workers/GeracaoProdutosLoteWorker.cs) cria escopos de execução e delega as regras aos casos de uso. A reserva de trabalhos utiliza lease e controle de concorrência no repositório. Consulte o ADR 0011 para o estágio de implementação e os limites de recuperação, repetição e processamento em blocos; processamento em segundo plano não equivale, sozinho, a garantia de execução exatamente uma vez.

## Rotas e acompanhamento

| Operação | Rota |
| --- | --- |
| Consultar ou cadastrar produtos | `GET /api/Produto` e `POST /api/Produto` |
| Consultar ou atualizar um produto | `GET /api/Produto/{codigo}` e `PUT /api/Produto/{codigo}` |
| Solicitar geração por lote | `POST /api/Produto/lotes` |
| Consultar processamentos do solicitante | `GET /api/processamentos-produtos` |
| Consultar um processamento do solicitante | `GET /api/processamentos-produtos/{id}` |

A aceitação de um lote retorna `202 Accepted`, não a conclusão da geração. No Angular, a solicitação parte da opção `Gerar por lote` no formulário de Produto. O acompanhamento está em `/atron/produtos/processamentos`, com polling enquanto o trabalho estiver pendente ou em execução e acesso aos produtos do lote concluído.

### Rotas do modelo anterior

O [EstoqueController](../../WebApi/Controllers/Stock/EstoqueController.cs) ainda publica `POST /api/Estoque/entrada` e `POST /api/Estoque/venda`, delegando respectivamente para `ProcessarEntradaAsync(Entrada)` e `ProcessarVendaAsync(Venda)`.

Esses nomes descrevem o código existente, não a divisão final pretendida entre Stock e Sales. A direção discutida para o produto é concentrar compras e vendas em Sales e registrar seus efeitos patrimoniais no Stock. Não há integração entre esses módulos a ser considerada pronta nesta revisão.

## Persistência e testes

O `StockDbContext` utiliza PostgreSQL. As migrations pertencem a [Infrastructure/Migrations](Infrastructure/Migrations), e os scripts SQL ficam em [Infrastructure/Scripts](Infrastructure/Scripts). Os testes do módulo estão em [Tests/Stock.Tests](../../../Tests/Stock.Tests); testes do host também ficam em [Tests/Platform.Tests](../../../Tests/Platform.Tests).

Para preparar o ambiente, siga o [guia de execução local](../../../docs/desenvolvimento/execucao-local.md). Não aplique migrations em produção como parte de uma demonstração.

## Decisões e próximas definições

- [ADR 0006: rotina de Categoria](../../../docs/adr/stock/0006-entregar-rotina-de-categorias-no-atron-stock.md).
- [ADR 0011: Produto como patrimônio individual](../../../docs/adr/stock/0011-modelar-produto-patrimonial.md).
- [ADR 0007: monólito modular e host neutro](../../../docs/adr/transversais/0007-adotar-monolito-modular-com-host-neutro.md).

Antes de implementar o novo estoque, é necessário definir quando um bem passa a compor o saldo disponível, como representar a pendência de recebimento, quais operações geram movimentos e como preservar responsável, data e motivo. O código de lote deve permitir rastrear a origem sem apagar a identidade individual dos bens. Relatórios e integração com Sales dependem dessas definições.
