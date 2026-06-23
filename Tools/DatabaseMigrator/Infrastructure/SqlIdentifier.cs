using DatabaseMigrator.Domain;

namespace DatabaseMigrator.Infrastructure;

internal static class SqlIdentifier
{
    public static string QuoteSqlServerTable(TablePlan table)
    {
        return table.Schema is null
            ? QuoteSqlServerIdentifier(table.Name)
            : $"{QuoteSqlServerIdentifier(table.Schema)}.{QuoteSqlServerIdentifier(table.Name)}";
    }

    public static string QuotePostgreSqlTable(TablePlan table)
    {
        return table.Schema is null
            ? QuotePostgreSqlIdentifier(table.Name)
            : $"{QuotePostgreSqlIdentifier(table.Schema)}.{QuotePostgreSqlIdentifier(table.Name)}";
    }

    public static string QuoteSqlServerIdentifier(string value)
    {
        return $"[{value.Replace("]", "]]")}]";
    }

    public static string QuotePostgreSqlIdentifier(string value)
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
}
