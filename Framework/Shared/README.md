# 📦 Shared - Núcleo de Utilitários Compartilhados

O projeto **Shared** faz parte do módulo **Framework** do Atron Tracker e concentra **componentes reutilizáveis**, **helpers**, **extensões** e **abstrações genéricas** que são compartilhadas entre múltiplas camadas da aplicação.

Seu objetivo principal é **evitar duplicação de código**, **padronizar comportamentos comuns** e **facilitar a manutenção e evolução do sistema**.

---

## 🎯 Objetivo do Projeto

O **Shared** atua como um **núcleo transversal**, oferecendo funcionalidades que não pertencem diretamente ao domínio de negócio, mas que são essenciais para o funcionamento consistente da aplicação como um todo.

Ele é utilizado por:
- Camadas de aplicação
- Infraestrutura
- Web API
- Outros módulos do Framework

---

## 🧩 Principais Responsabilidades

- Centralizar **extensões utilitárias**
- Fornecer **classes base e contratos genéricos**
- Padronizar **tratamento de erros e resultados**
- Disponibilizar **helpers comuns**
- Facilitar **validações e conversões**
- Reduzir acoplamento entre camadas

---

## 🛠️ Componentes Comuns

Embora o conteúdo possa evoluir, o projeto Shared normalmente inclui:

### 🔹 Extensions
- Extensões para tipos primitivos
- Extensões para coleções
- Helpers para strings, datas e enums

### 🔹 Helpers
- Classes utilitárias reutilizáveis
- Métodos auxiliares para lógica comum
- Conversores e formatadores

### 🔹 Result / Response Patterns
- Padronização de retorno de operações
- Encapsulamento de sucesso, falha e mensagens
- Facilita comunicação entre camadas

### 🔹 Constants
- Constantes globais
- Chaves de configuração
- Valores padronizados do sistema

### 🔹 Exceptions
- Exceções customizadas
- Padronização de erros de domínio e aplicação

---

## 🧱 Princípios Aplicados

O projeto Shared segue boas práticas de arquitetura e design:

- **Baixo Acoplamento**
- **Alta Reutilização**
- **Responsabilidade Única**
- **Independência de Frameworks**
- **Compatível com Clean Architecture**

> ⚠️ Importante:  
> O Shared **não deve conter regras de negócio** nem depender de camadas superiores.

---

## 🔗 Dependências

- **.NET 8**
- Não depende de infraestrutura externa
- Pode ser referenciado por qualquer camada do sistema

---

## 📁 Estrutura do Projeto (Exemplo)

Shared  
├── Extensions  
├── Helpers  
├── Constants  
├── Exceptions  
├── Results  
└── Utilities  

> A estrutura pode evoluir conforme novas necessidades surgirem.

---

## 🚀 Benefícios

- Código mais limpo e organizado
- Redução de duplicações
- Facilidade de manutenção
- Padronização de comportamentos
- Melhor legibilidade e escalabilidade

---

## 📌 Observações

- Qualquer funcionalidade genérica e reutilizável deve ser avaliada para inclusão no Shared.
- Evite dependências desnecessárias.
- Mantenha o projeto simples e focado.

---
