## Projeto Infra.IoC

O projeto `Infra.IoC` é a camada de conteiner para tratar as dependências do sistema. <br>
Nesse projeto estarão definidos os contratos e implementações de acordo com o tempo de vida no fluxo do sistema.

## Provider de banco de dados

O projeto possui resolução centralizada de provider de banco em `DatabaseProvider.cs`.

Atualmente são suportados:

- `PostgreSql`
- `Postgres`
- `Supabase`

O provider `Supabase` é tratado como PostgreSQL e usa `Npgsql.EntityFrameworkCore.PostgreSQL`.

Exemplo de configuração local:

```json
{
  "Database": {
    "Provider": "Supabase",
    "ConnectionStringName": "DefaultConnection"
  }
}
```

Essa configuração usa Supabase/PostgreSQL como banco alvo do deploy sem alterar os repositórios, services ou casos de uso.
