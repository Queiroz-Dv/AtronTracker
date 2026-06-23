using DatabaseMigrator.Application;
using DatabaseMigrator.Infrastructure;
using DatabaseMigrator.Presentation;

var options = MigratorOptions.Parse(args);
var reporter = new ConsoleReporter();

if (string.IsNullOrWhiteSpace(options.SourceConnectionString))
{
    reporter.Error("Informe a conexao SQL Server via --source, Migration__SqlServerConnection, ConnectionStrings__AtronConnection, MIGRATION_SQLSERVER_CONNECTION ou SQLSERVER_CONNECTION.");
    return 1;
}

if (string.IsNullOrWhiteSpace(options.TargetConnectionString))
{
    reporter.Error("Informe a conexao Supabase via --target, Migration__SupabaseConnection, ConnectionStrings__DefaultConnection, MIGRATION_SUPABASE_CONNECTION ou SUPABASE_CONNECTION.");
    return 1;
}

reporter.WriteHeader(options);

var contextFactory = new MigrationContextFactory(
    options.SourceConnectionString,
    options.TargetConnectionString);

var runner = new MigrationRunner(
    contextFactory,
    new RelationalDataGateway(),
    reporter);

await runner.ExecuteAsync(options);

return 0;
