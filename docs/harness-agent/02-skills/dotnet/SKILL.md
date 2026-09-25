# .NET

## Regras

- Preferir DI nativa do ASP.NET Core.
- Interfaces devem representar contratos úteis, não existir apenas por convenção.
- Evitar métodos gigantes.
- Evitar exceções para fluxo normal quando o projeto utiliza Notification Pattern.
- Preservar separação de responsabilidades.
- Validar entrada antes de executar regras que dependem dela.
- Não esconder chamadas de infraestrutura atrás de abstrações desnecessárias.
- Evitar Fat Methods ao invés disso crie records, dtos ou classes que façam sentido para o cenário ou correção necessária