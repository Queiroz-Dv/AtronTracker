using DatabaseMigrator.Domain;

namespace DatabaseMigrator.Presentation;

internal sealed class ConsoleReporter
{
    public void WriteHeader(MigratorOptions options)
    {
        Console.WriteLine($"Modo: {options.Mode}");
        Console.WriteLine($"Contexts: {string.Join(", ", options.Contexts)}");
        Console.WriteLine($"Reset target: {options.ResetTarget}");
        Console.WriteLine();
    }

    public void ContextStarted(string contextName, int tableCount)
    {
        Console.WriteLine($"== {contextName} ==");
        Console.WriteLine($"Tabelas mapeadas: {tableCount}");
    }

    public void CountHeader()
    {
        Console.WriteLine("Tabela | SQL Server | Supabase");
    }

    public void Count(TablePlan table, long sourceCount, long targetCount)
    {
        Console.WriteLine($"{table.DisplayName} | {sourceCount} | {targetCount}");
    }

    public void ResetStarted()
    {
        Console.WriteLine("Limpando tabelas do Supabase...");
    }

    public void TableDeleted(TablePlan table)
    {
        Console.WriteLine($"DELETE {table.DisplayName}");
    }

    public void TableCopied(TablePlan table, long rowCount)
    {
        Console.WriteLine($"COPIADO: {table.DisplayName} {rowCount} registro(s)");
    }

    public void SequenceResetStarted()
    {
        Console.WriteLine("Ajustando sequences do PostgreSQL...");
    }

    public void SequenceReset(TablePlan table, ColumnPlan column)
    {
        Console.WriteLine($"SEQUENCE: {table.DisplayName}.{column.Name}");
    }

    public void ValidationStarted()
    {
        Console.WriteLine("Validacao de contagens:");
    }

    public void ValidationResult(TablePlan table, long sourceCount, long targetCount)
    {
        var status = sourceCount == targetCount ? "OK" : "DIVERGENTE";
        Console.WriteLine($"{status}: {table.DisplayName} SQL Server={sourceCount}, Supabase={targetCount}");
    }

    public void Error(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Error.WriteLine(message);
        Console.ResetColor();
    }

    public void BlankLine()
    {
        Console.WriteLine();
    }
}
