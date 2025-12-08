# 🏛️ Atron.Domain

O projeto **Atron.Domain** é o coração da aplicação, projetado seguindo os princípios do **Domain-Driven Design (DDD)**. Ele encapsula a lógica de negócios, regras, entidades e contratos essenciais para o funcionamento do sistema.

---

## 📂 Estrutura de Pastas

A organização do projeto reflete a separação de responsabilidades e a clareza do domínio:

### 🔐 ApiEntities

Focada em entidades relacionadas aos processos de autenticação e rotas da API.

- **Login**: Entidades para gerenciamento de sessões.
- **Registro**: Entidades para criação de novos usuários.
- **Rotas**: Definições de endpoints e acessos.

---

### 🧩 Componentes

Entidades auxiliares e objetos de valor que dão suporte a outros fluxos do sistema.

- Reutilizáveis em diferentes contextos.
- Focados em lógica específica e isolada.

---

### 📦 Entities

Aqui residem as **Entidades de Domínio** principais.

- Representam os objetos concretos do negócio (ex: Cliente, Produto, Pedido).
- Contêm regras de negócio intrínsecas e validações de estado.

---

### 📝 Interfaces

Define os contratos (interfaces) para os repositórios e serviços.

- **Abstração**: Permite que a camada de domínio permaneça agnóstica à persistência de dados.
- **Implementação**: As implementações concretas residem na camada `Infrastructure`.

---

## 💡 Princípios Aplicados

- **Ignorância da Persistência**: As entidades não sabem como são salvas no banco de dados.
- **Linguagem Ubíqua**: Os nomes de classes e métodos refletem a linguagem usada pelos especialistas do negócio.
- **Rico em Comportamento**: Evita o anti-padrão "Anemic Domain Model", focando em entidades que possuem comportamento e não apenas dados.
