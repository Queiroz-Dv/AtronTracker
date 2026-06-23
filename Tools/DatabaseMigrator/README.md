# DatabaseMigrator

Ferramenta operacional para copiar dados do SQL Server legado para o Supabase/PostgreSQL.

O migrador foi usado para a carga inicial do Supabase e deve permanecer disponível apenas para necessidades específicas, como recarga controlada, validação pontual ou reenvio de dados legados.

Ele não faz parte do runtime da aplicação.

## Estrutura

```text
Tools/DatabaseMigrator
  Application
    MigrationRunner.cs
  Domain
    ColumnPlan.cs
    DatabaseKind.cs
    MigrationMode.cs
    TableKey.cs
    TablePlan.cs
  Infrastructure
    MigrationContextDefinition.cs
    MigrationContextFactory.cs
    RelationalDataGateway.cs
    SqlIdentifier.cs
  Presentation
    ConsoleReporter.cs
    MigratorOptions.cs
  Program.cs
```

## Responsabilidades

- descobrir tabelas e colunas pelos metadados dos `DbContext`;
- ordenar tabelas por dependências de FK;
- copiar dados preservando IDs;
- limpar o destino quando `--reset-target` for informado;
- ajustar sequences do PostgreSQL após a importação;
- validar contagens entre SQL Server e Supabase.

## Modos

- `dry-run`: lê as contagens na origem e no destino, sem gravar dados;
- `migrate`: executa a cópia dos dados;
- `validate`: compara contagens entre SQL Server e Supabase.

## Exemplo

```powershell
dotnet run --project Tools\DatabaseMigrator\DatabaseMigrator.csproj -- `
  --mode dry-run `
  --contexts all `
  --source "<connection-string-sql-server>" `
  --target "<connection-string-supabase>"
```

Mais detalhes estão documentados em `Documentacao/Migracao_SQLServer_Supabase.md`.
