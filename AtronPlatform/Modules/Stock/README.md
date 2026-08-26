# AtronStock

O módulo **AtronStock** é responsável pela cadeia de suprimentos, estoque, patrimônio e bens dentro da plataforma Atron. Ele deve oferecer cobertura ampla para rotinas físicas da empresa, protegendo rastreabilidade, consistência de saldos e histórico de movimentações.

---

## Funcionalidades principais

- **Gestão de produtos**: cadastro e categorização de itens.
- **Controle de estoque**: acompanhamento de saldos e disponibilidade.
- **Fornecedores e clientes**: cadastros relacionados aos fluxos de compra, entrada, venda e saída.
- **Entradas e saídas**: registro das movimentações que alteram o estoque.
- **Rastreabilidade**: histórico das movimentações e justificativas operacionais.
- **Patrimônio e bens**: direção de evolução para controlar itens patrimoniais e bens da empresa.

---

## Estrutura do projeto

O módulo reside em `AtronPlatform/Modules/Stock`. Seus projetos preservam os
nomes qualificados `AtronStock.Domain`, `AtronStock.Application` e
`AtronStock.Infrastructure`.

### Domain

O núcleo da lógica de negócios.

- **Entities**:
  - `Produto`: representa um patrimônio individual.
  - `LoteProduto`: identifica a origem opcional de Produtos gerados em lote.
  - `ProcessamentoProdutoLote`: trabalho durável aceito para geração assíncrona.
  - `Estoque`: mantém o estado atual do inventário de um produto.
  - `MovimentacaoEstoque`: registro imutável de cada alteração no estoque.
- **Enums**:
  - `TipoMovimentacao`: define se a operação é uma `Entrada` ou `Saida`.
- **Services**:
  - `EstoqueService`: encapsula as regras para adicionar ou remover itens, garantindo a consistência dos dados.

### Infrastructure

Implementação técnica e persistência.

- **Context**: `StockDbContext` configurado com Entity Framework Core.
- **Repositories**: implementações para acesso a dados, isolando o domínio de detalhes de banco de dados.
- **Configurations**: mapeamento fluente (Fluent API) para garantir um esquema de banco de dados otimizado.
- **DependencyInjection**: `AddStockModule` registra o contexto, repositórios,
  serviços, mapeamentos e validações pertencentes ao Stock.

### Application

- `ProdutoService`: fachada dos casos de uso de patrimônio individual e da
  aceitação de geração em lote.
- `ProcessamentoProdutoService`: fachada das consultas de acompanhamento,
  sempre restritas ao solicitante obtido de `IUserAccessor`.
- `GeracaoProdutosLoteWorker`: hosted service que cria um escopo por execução
  e delega a regra de geração ao caso de uso. A tomada do próximo trabalho usa
  lease, token de concorrência e `FOR UPDATE SKIP LOCKED`, permitindo recuperar
  reservas expiradas sem entregar o mesmo trabalho simultaneamente.

### Borda HTTP

Os controllers existentes ficam em `AtronPlatform/WebApi/Controllers/Stock` e
são publicados pelo único host `AtronPlatform.WebApi`. O Stock não possui host
executável próprio.

O acompanhamento de geração usa `GET /api/processamentos-produtos` e
`GET /api/processamentos-produtos/{id}`. O Angular solicita a geração no
próprio formulário de Produto por meio da opção `Gerar por lote` e acompanha
os trabalhos em `/atron/produtos/processamentos`. A tela usa polling somente
enquanto o detalhe está pendente ou em execução e permite abrir os Produtos
do lote concluído.

---

## Exemplo de uso

O `EstoqueService` é a porta de entrada para manipular o estoque de forma segura:

```csharp
// Registrar uma entrada de mercadoria
await _estoqueService.RegistrarEntradaAsync(
    produtoId: 1,
    quantidade: 50,
    observacao: "Recebimento NF-e 1234"
);

// Registrar uma saída (venda ou baixa)
await _estoqueService.RegistrarSaidaAsync(
    produtoId: 1,
    quantidade: 5,
    observacao: "Venda #9876"
);
```

---

## Modelo de dados

O modelo foi desenhado para separar a definição do produto de seu inventário, permitindo flexibilidade futura, como múltiplos estoques, filiais, centros de armazenamento ou controle patrimonial.

| Entidade         | Responsabilidade                                  |
| ---------------- | ------------------------------------------------- |
| **Produto**      | Patrimônio individual, aquisição, valor, status e classificação opcional. |
| **LoteProduto**  | Origem opcional de uma geração de Produtos.       |
| **ProcessamentoProdutoLote** | Solicitação durável e seu estado de execução. |
| **Estoque**      | Quantidade atual e data da última atualização.    |
| **Movimentacao** | Log de auditoria (quem, quando, quanto e porquê). |

## Migrations PostgreSQL/Supabase

O `StockDbContext` usa Supabase/PostgreSQL como provider de persistência do projeto.

As migrations ativas ficam no projeto principal de infraestrutura:

- `AtronPlatform/Modules/Stock/Infrastructure/Migrations`

Os scripts SQL correspondentes ficam em
`AtronPlatform/Modules/Stock/Infrastructure/Scripts`.

## Decisões arquiteturais

As decisões específicas do módulo ficam em `docs/adr/stock/`.

- [ADR 0006: Entregar a rotina de Categoria no Atron Stock](../../../docs/adr/stock/0006-entregar-rotina-de-categorias-no-atron-stock.md)
- [ADR 0011: Modelar Produto como patrimônio individual](../../../docs/adr/stock/0011-modelar-produto-patrimonial.md)
- [ADR 0007: Adotar monólito modular com host neutro](../../../docs/adr/transversais/0007-adotar-monolito-modular-com-host-neutro.md)
