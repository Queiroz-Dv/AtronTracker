# Padrão de resolvers

## Objetivo

Resolvers dão um nome explícito à seleção de um único resultado efetivo entre
múltiplos candidatos ou fontes possíveis. O padrão evita que prioridades,
fallbacks e critérios de seleção fiquem espalhados em serviços de aplicação,
repositórios ou blocos condicionais sem um responsável claro.

## Quando usar

Um resolver é apropriado quando o componente precisa:

- reunir candidatos ou fontes conhecidas;
- respeitar uma ordem de prioridade ou fallback;
- descartar opções ausentes ou inadequadas;
- devolver um único resultado efetivo, ou indicar que nenhum foi encontrado;
- manter essa seleção separada da execução completa do caso de uso.

A pergunta típica de um resolver é: "qual candidato ou valor deve ser usado
neste contexto?".

## Estrutura

Resolvers de aplicação são organizados primeiro pelo tipo arquitetural e depois
pela capacidade proprietária da seleção.

```text
Application/
└── Resolvers/
    └── Tarefas/
        └── AprovadorObtencaoTarefaResolver.cs
```

Não criar uma subpasta para cada classe. Uma subdivisão adicional, como
`Resolvers/Tarefas/Obtencao`, só é necessária quando a capacidade possuir
vários resolvers e o agrupamento reduzir ambiguidade.

Resolvers técnicos permanecem na camada proprietária da decisão. Um resolver
de provider de banco, por exemplo, pertence à configuração de Infrastructure e
não deve ser movido para Application apenas por compartilhar o mesmo sufixo.

## Forma da classe

Um resolver interno, estável e com uma única implementação deve ser uma classe
concreta `sealed` e pode ser injetado diretamente.

```csharp
public sealed class AprovadorObtencaoTarefaResolver(
    IUsuarioRepository usuarioRepository)
{
    public Task<Usuario> ResolverAsync(Usuario solicitante, Tarefa tarefa)
    {
        // Seleciona um único aprovador entre os candidatos do contexto.
    }
}
```

O registro de DI também usa o tipo concreto:

```csharp
services.AddScoped<AprovadorObtencaoTarefaResolver>();
```

Uma interface específica só deve ser criada quando houver uma fronteira real,
como implementações alternativas, seleção de implementação em runtime,
decoração, contrato entre módulos ou inversão entre Application e
Infrastructure. Facilidade para criar mocks, isoladamente, não justifica
duplicar o contrato de uma classe concreta estável.

Não criar `IResolver<T>`, `ResolverBase` ou outra abstração genérica. O nome do
resolver deve expressar o conceito selecionado e o contexto da seleção.

## Diferenças para outros componentes

| Componente | Pergunta principal | Resultado típico |
| --- | --- | --- |
| Use Case | Qual fluxo deve ser executado? | Resultado completo do caso de uso |
| Policy | A operação é permitida neste contexto? | Decisão acompanhada do motivo |
| Resolver | Qual candidato ou valor deve ser usado? | Um resultado efetivo ou ausência |
| Specification | Este candidato satisfaz o critério? | `bool` |
| Validation | Os dados recebidos estão coerentes? | Mensagens de validação |
| Mapping | Como traduzir uma representação? | Objeto em outro contrato |
| Provider | Como fornecer configuração ou dado externo? | Valor disponibilizado ao consumidor |
| Factory | Como criar um objeto válido? | Nova instância |

Consultar um objeto por um identificador conhecido não caracteriza, por si só,
um resolver. Relacionar entidades, validar uma operação, mapear dados ou
executar persistência também não devem ser renomeados para `Resolver` apenas
porque existe uma busca durante o fluxo.

## Aplicações atuais

### Aprovador de obtenção de tarefa

`AprovadorObtencaoTarefaResolver` seleciona o primeiro usuário existente na
ordem definida pelo domínio: gestor imediato, gestor do departamento da tarefa
e gestores dos departamentos vinculados ao solicitante. O caso de uso continua
responsável por criar a solicitação, persistir, registrar movimentação e
publicar notificações.

### Responsável por notificação do estoque não é um resolver

O componente do Stock apenas fornece o código configurado para receber uma
notificação consultiva. Como não seleciona entre candidatos ou fontes, ele é
nomeado `ResponsavelNotificacaoEstoqueProvider` e fica em
`Application/Providers/Notificacoes`.

Se futuramente o Stock precisar escolher entre responsável do produto, gestor
do departamento e responsável global, essa seleção poderá justificar um
resolver próprio.

## Testes

- testar diretamente a ordem, os fallbacks, as exclusões e a ausência de
  resultado do resolver;
- nos testes do consumidor, usar o resolver concreto com mocks de suas
  dependências externas quando a classe não possuir interface;
- não repetir nos testes do caso de uso toda combinação já coberta pelos testes
  focados do resolver;
- preservar testes de integração quando a resolução depender da composição de
  DI ou de configuração do host.

## Checklist

- Existe mais de um candidato ou fonte possível?
- A responsabilidade principal é selecionar um único resultado?
- A prioridade ou o fallback possui significado explícito?
- A seleção muda por motivo diferente do caso de uso consumidor?
- O nome identifica o resultado e o contexto resolvido?
- A classe pode ser concreta e `sealed`?
- Se existe interface, ela representa uma fronteira comprovada?
- O resolver está na camada e na capacidade proprietárias da seleção?
- Foram evitadas abstrações genéricas e classes de passagem?
