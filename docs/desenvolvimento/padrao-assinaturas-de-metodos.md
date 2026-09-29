# Padrao de assinaturas de metodos

## Regra

Metodos de codigo de aplicacao nao devem receber mais de tres parametros. Quando uma operacao precisar de quatro ou mais valores de entrada, esses valores devem ser agrupados em uma classe ou `record` com um nome que represente o conceito transportado.

## Aplicacao

- Use `record` para dados imutaveis e sem ciclo de vida proprio. O tipo pode
  conter propriedades derivadas, fabricas ou operacoes puras relacionadas ao
  valor que representa.
- Use classe quando o objeto precisar de comportamento, validacao de estado ou mutabilidade intencional.
- O objeto de parametros deve expressar um conceito do dominio ou da aplicacao. Nao crie um agrupamento generico somente para contornar a regra.
- A regra tambem vale para metodos privados. Nesse caso, prefira extrair um objeto ja existente ou reduzir a responsabilidade do metodo antes de criar um novo tipo.

## Organizacao dos tipos

- Todo tipo declarado como `record` no projeto `Application` deve ficar em
  `Records`, inclusive quando representar parametros, contexto, modelo de
  template ou contrato de transporte.
- A capacidade proprietaria deve ser representada por uma subpasta, como
  `Records/Tarefa`, `Records/Usuario`, `Records/Autenticacao` ou
  `Records/PlanejamentoCusto`. O namespace deve acompanhar a estrutura.
- Toda classe declarada como `record` deve terminar com o sufixo singular
  `Record`. O plural `Records` fica reservado a pasta ou a um arquivo que
  agrupe varios records coesos.
- Um tipo so deve ficar diretamente na raiz de `Records` quando for realmente
  transversal e nao possuir uma capacidade proprietaria clara.
- O nome do tipo deve representar o conjunto de dados transportado. Por
  exemplo, `ContextoNotificacaoTarefaRecord` representa o contexto necessario
  para compor notificacoes de tarefa.
- Classes intencionalmente estaticas devem ficar em `Statics`, usando o
  namespace `Application.Statics`. Essa pasta deve conter somente catalogos de
  constantes ou operacoes puras e sem estado, e nao helpers genericos.
- Se o comportamento tiver dependencias, estado, regras variaveis ou
  responsabilidade de um caso de uso, ele deve ser modelado como um
  colaborador proprio, e nao como uma classe estatica.

## Exemplo

```csharp
public sealed record RecuperacaoSenhaEmailParametrosRecord(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras);

EmailRequest ComporRecuperacaoSenha(RecuperacaoSenhaEmailParametrosRecord parametros);
```

O contrato reduz a dependencia da ordem dos argumentos e deixa os dados necessarios para a composicao explicitamente agrupados.
