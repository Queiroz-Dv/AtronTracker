
# 🧱 Padrão de Modelagem de Objetos do Atron

Este documento define o padrão oficial de uso de **`class`**, **`record`** e **`struct`** no projeto **Atron**, com o objetivo de garantir **clareza**, **consistência** e **responsabilidade única** entre os tipos utilizados nas camadas do sistema.

---

## 🎯 Objetivo

Evitar o uso indiscriminado de `class` em todos os contextos (_primitive class obsession_), adotando **o tipo mais apropriado** para cada cenário: transporte de dados, manipulação de domínio ou otimização de performance.

---

## 🗂️ Resumo das Diretrizes

| Tipo          | Camada                         | Uso Recomendado                                                  | Comparação  | Mutabilidade |
|---------------|--------------------------------|------------------------------------------------------------------|-------------|--------------|
| `class`       | **Domínio**, **Serviços**      | Entidades com identidade, comportamento e regras de negócio      | Por referência | Mutável      |
| `record`      | **DTOs**, **Request/Response** | Dados imutáveis, comparáveis por valor, ideais para API          | Por valor      | Imutável (`init`) |
| `record struct` | **Value Objects leves**        | Estruturas pequenas, imutáveis, de alta frequência (até 3 campos) | Por valor      | Imutável      |
| `struct`      | **Infraestrutura específica**  | Tipos primitivos otimizados para performance extrema             | Por valor      | Imutável ou mutável (com cuidado) |

---

## 🧭 Fluxo entre as Camadas

### 🔹 Camada de Apresentação (API)

- **Entrada (Request DTOs)** → `record`
- **Saída (Response DTOs)** → `record`
- São imutáveis e fáceis de comparar por conteúdo.

```csharp
public record DepartamentoRequest(string Codigo, string Descricao);
public record DepartamentoResponse(Guid Id, string Codigo, string Descricao);
```

---

### 🔹 Camada de Aplicação

- **Mapeamento** de DTOs para entidades e vice-versa.
- Utiliza **classes** para entidades e **records** para DTOs.

```csharp
public class DepartamentoMapping : ApplicationMapService<DepartamentoRequest, Departamento>
{
    public override Departamento MapToEntity(DepartamentoRequest dto) =>
        new Departamento(dto.Codigo.ToUpper(), dto.Descricao.ToUpper());
}
```

---

### 🔹 Camada de Domínio

- **Entidades** → `class`
- **Value Objects ricos** → `record` ou `record struct`

```csharp
public class Departamento
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; }
    public string Descricao { get; private set; }
}
```

```csharp
public readonly record struct Currency(string Code, decimal Rate);
```

---

## 💡 Regras de Uso

### ✅ Quando usar `record`

- Dados imutáveis que representam *estado*, não identidade.
- DTOs trocados via JSON.
- Value Objects do domínio.

### ✅ Quando usar `class`

- Objetos com **identidade única** e ciclo de vida.
- Entidades persistidas no banco de dados.
- Serviços, Repositórios, Handlers.

### ✅ Quando usar `record struct` ou `struct`

- Objetos muito pequenos (até 3 campos primitivos).
- Usados intensivamente em loops/performance crítica.
- Sem mutabilidade e sem coleções internas.

---

## ⚠️ Cuidados Importantes

- Evite `struct` para DTOs com listas (`List<T>`), strings longas ou complexidade.
- Evite usar `record` como entidade de domínio com identidade (use `class`).
- Prefira `record` a `class` para DTOs de leitura — evita mutações inesperadas.

---

## 📚 Exemplos Completos

### ✅ DTO de Requisição (record)
```csharp
public record DepartamentoCreateRequest(string Codigo, string Descricao);
```

### ✅ Entidade de Domínio (class)
```csharp
public class Departamento
{
    public Guid Id { get; private set; }
    public string Codigo { get; private set; }
    public string Descricao { get; private set; }

    public Departamento(string codigo, string descricao)
    {
        Codigo = codigo;
        Descricao = descricao;
    }
}
```

### ✅ Value Object Imutável (record struct)
```csharp
public readonly record struct Coordinate(double Latitude, double Longitude);
```

---

## ✨ Benefícios deste Padrão

✅ Código mais expressivo  
✅ Segurança contra mutações indesejadas  
✅ Melhor suporte a testes e comparações  
✅ Eficiência de memória quando necessário  
✅ Clareza na arquitetura de camadas

---

## 👨‍💻 Observações Finais

Este padrão faz parte da estratégia de arquitetura limpa do projeto Atron.  
Todos os novos DTOs, entidades e objetos de valor devem ser criados seguindo estas diretrizes.  
Em caso de dúvida, prefira começar com `record` para DTOs e evolua conforme a complexidade justificar.

---

## 📌 Créditos e Referência

Este padrão foi consolidado com base em princípios da Clean Architecture, Domain-Driven Design (DDD) e recomendações oficiais do .NET (C# 9+).

Desenvolvido e aplicado por:  
**Eduardo Queiroz**  
Arquiteto de Software | Projeto Atron
