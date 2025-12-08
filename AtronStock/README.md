# 📦 AtronStock

O módulo **AtronStock** é responsável pelo gerenciamento de estoque e controle de produtos dentro do ecossistema Atron Tracker. Ele foi projetado para ser robusto, rastreável e fácil de integrar.

---

## 🚀 Funcionalidades Principais

- **Gestão de Produtos**: Cadastro e categorização de itens.
- **Controle de Estoque**: Monitoramento em tempo real da quantidade disponível.
- **Rastreabilidade**: Histórico completo de todas as movimentações (Entradas e Saídas).

---

## 📂 Estrutura do Projeto

### 🏗️ Domain

O núcleo da lógica de negócios.

- **Entities**:
  - `Produto`: Representa o item comercializável.
  - `Estoque`: Mantém o estado atual do inventário de um produto.
  - `MovimentacaoEstoque`: Registro imutável de cada alteração no estoque.
- **Enums**:
  - `TipoMovimentacao`: Define se a operação é uma `Entrada` ou `Saida`.
- **Services**:
  - `EstoqueService`: Encapsula as regras para adicionar ou remover itens, garantindo a consistência dos dados.

### 🔧 Infrastructure

Implementação técnica e persistência.

- **Context**: `StockDbContext` configurado com Entity Framework Core.
- **Repositories**: Implementações para acesso a dados, isolando o domínio de detalhes de banco de dados.
- **Configurations**: Mapeamento fluente (Fluent API) para garantir um esquema de banco de dados otimizado.

---

## 💻 Exemplo de Uso (Service)

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

## 📊 Modelo de Dados

O modelo foi desenhado para separar a definição do produto de seu inventário, permitindo flexibilidade futura (ex: múltiplos estoques/filiais).

| Entidade         | Responsabilidade                                  |
| ---------------- | ------------------------------------------------- |
| **Produto**      | Dados cadastrais (Nome, Código, Preço).           |
| **Estoque**      | Quantidade atual e data da última atualização.    |
| **Movimentacao** | Log de auditoria (Quem, Quando, Quanto e Porquê). |

---

Feito com ❤️ por [Queiroz-Dv](https://github.com/Queiroz-Dv)
