# Padrao de assinaturas de metodos

## Regra

Metodos de codigo de aplicacao nao devem receber mais de tres parametros. Quando uma operacao precisar de quatro ou mais valores de entrada, esses valores devem ser agrupados em uma classe ou `record` com um nome que represente o conceito transportado.

## Aplicacao

- Use `record` para dados imutaveis, sem comportamento proprio, usados apenas para transportar os parametros da operacao.
- Use classe quando o objeto precisar de comportamento, validacao de estado ou mutabilidade intencional.
- O objeto de parametros deve expressar um conceito do dominio ou da aplicacao. Nao crie um agrupamento generico somente para contornar a regra.
- A regra tambem vale para metodos privados. Nesse caso, prefira extrair um objeto ja existente ou reduzir a responsabilidade do metodo antes de criar um novo tipo.

## Exemplo

```csharp
public sealed record RecuperacaoSenhaEmailParametros(
    string Destinatario,
    string Nome,
    string Link,
    int ValidadeHoras);

EmailRequest ComporRecuperacaoSenha(RecuperacaoSenhaEmailParametros parametros);
```

O contrato reduz a dependencia da ordem dos argumentos e deixa os dados necessarios para a composicao explicitamente agrupados.
