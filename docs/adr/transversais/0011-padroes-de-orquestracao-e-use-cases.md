# ADR 0011: Padrões de Orquestração, Use Cases e Services

## Contexto e Problema
Durante o desenvolvimento contínuo da Atron Platform (composta por módulos como Tracker, Stock, etc.), notamos a proliferação de camadas "pass-through". Controllers chamavam interfaces de Services apenas para delegar a chamada 1:1 para repositórios ou métodos internos, resultando em:
1. **Inchaço do Injetor de Dependências**: Centenas de interfaces sem comportamento real.
2. **Dificuldade de Navegação**: O desenvolvedor precisava pular por múltiplas camadas (`Controller -> IService -> Service -> Repository`) para entender um fluxo simples.
3. **Falta de Clareza na Fronteira de Domínio**: Não havia um critério claro de quando usar um `UseCase` versus um `Domain Service`.

## Decisão
Foi decidido adotar uma arquitetura pragmática baseada em **Use Cases** acionados diretamente pelos **Controllers**, reduzindo camadas anêmicas e centralizando o fluxo de negócio de forma declarativa.

### 1. Critério para Use Cases (Application Layer)
- **Quando usar:** Um Use Case **deve ser criado** para representar uma operação tangível (ação) iniciada pelo usuário ou sistema (Ex: `CriarTarefaCase`, `AssumirTarefaCase`, `AlterarEmailCase`).
- **Regras:**
  - Controllers injetam e consomem **diretamente** os Use Cases.
  - Um Use Case sempre retorna um `Resultado` ou `Resultado<T>`.
  - Interfaces (`ICriarTarefaCase`) são desnecessárias a menos que haja múltiplas implementações polimórficas reais. Injetar a classe concreta do Use Case é o padrão.
  - O Use Case concentra o fluxo: Validação -> Regras de Negócio -> Gravação -> Notificação/Eventos.

### 2. Critério para Domain Services (Domain Layer)
- **Quando usar:** Services (ex: `TarefaEstadoService`) só devem existir se houver necessidade de **orquestrar múltiplos repositórios ou regras de domínio complexas** que transcendam um único Use Case ou se o serviço for reutilizado extensivamente por múltiplos componentes.
- **Regras:**
  - **Não crie Services pass-through**. Se o Service apenas chama o repositório, o Use Case deve chamar o repositório diretamente.

### 3. Critério para Validação
- **`Regra<T>` (Domain):** Validações puras de estado e propriedades da entidade (ex: tamanho de string, formato de e-mail). Não possuem injeção de dependência.
- **`IValidador<T>` (Application):** Validações de fluxo que precisam consultar bancos de dados ou APIs externas (ex: "CPF já existe no banco?").

## Consequências
- **Positivas:** 
  - Redução drástica na quantidade de arquivos (interfaces e classes de serviços vazios).
  - Fluxos de negócio mais coesos e centralizados no Use Case correspondente.
  - Stack traces mais curtos e código mais legível.
- **Negativas:** 
  - Controllers que agrupavam múltiplas entidades (ex: `TarefaController`) podem ficar com o construtor inchado por injetar muitos Use Cases. A mitigação é **dividir os Controllers por contexto/responsabilidade** (ex: `TarefaObtencaoController`, `TarefaConfiguracoesController`).

## Status
Aceito e em fase de implementação através da refatoração contínua dos módulos existentes.
