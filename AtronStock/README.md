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

### Domain

O núcleo da lógica de negócios.

- **Entities**:
  - `Produto`: representa o item comercializável.
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
| **Produto**      | Dados cadastrais (nome, código, preço).           |
| **Estoque**      | Quantidade atual e data da última atualização.    |
| **Movimentacao** | Log de auditoria (quem, quando, quanto e porquê). |

## Migrations PostgreSQL/Supabase

O `StockDbContext` usa Supabase/PostgreSQL como provider de persistência do projeto.

As migrations ativas ficam no projeto principal de infraestrutura:

- `AtronStock/Infrastructure/Migrations`

Os scripts SQL correspondentes ficam em `AtronStock/Infrastructure/Scripts`.
