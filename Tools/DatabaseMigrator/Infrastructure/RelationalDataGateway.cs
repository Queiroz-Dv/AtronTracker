using System.Data;
using DatabaseMigrator.Domain;
using Microsoft.EntityFrameworkCore;

namespace DatabaseMigrator.Infrastructure;

internal sealed class RelationalDataGateway
{
    public async Task<long> CountAsync(DbContext context, TablePlan table, DatabaseKind kind)
    {
        await using var command = context.Database.GetDbConnection().CreateCommand();
        command.CommandText = kind == DatabaseKind.SqlServer
            ? $"SELECT COUNT(*) FROM {SqlIdentifier.QuoteSqlServerTable(table)};"
            : $"SELECT COUNT(*) FROM {SqlIdentifier.QuotePostgreSqlTable(table)};";

        var count = await command.ExecuteScalarAsync();
        return Convert.ToInt64(count);
    }

    public async Task DeleteAllAsync(DbContext target, TablePlan table)
    {
        await using var command = target.Database.GetDbConnection().CreateCommand();
        command.CommandText = $"DELETE FROM {SqlIdentifier.QuotePostgreSqlTable(table)};";
        await command.ExecuteNonQueryAsync();
    }

    public async Task<long> CopyTableAsync(DbContext source, DbContext target, TablePlan table)
    {
        var count = 0L;
        var sourceConnection = source.Database.GetDbConnection();
        var targetConnection = target.Database.GetDbConnection();

        await using var sourceCommand = sourceConnection.CreateCommand();
        sourceCommand.CommandText = CreateSelectSql(table);

        await using var reader = await sourceCommand.ExecuteReaderAsync(CommandBehavior.SequentialAccess);

        while (await reader.ReadAsync())
        {
            await using var insert = targetConnection.CreateCommand();
            insert.CommandText = CreateInsertSql(table);

            for (var i = 0; i < table.Columns.Count; i++)
            {
                var parameter = insert.CreateParameter();
                parameter.ParameterName = $"p{i}";
                parameter.Value = NormalizeValue(reader.GetValue(i));
                insert.Parameters.Add(parameter);
            }

            await insert.ExecuteNonQueryAsync();
            count++;
        }

        return count;
    }

    public async Task<string?> GetSequenceNameAsync(DbContext target, TablePlan table, ColumnPlan column)
    {
        await using var serialSequenceCommand = target.Database.GetDbConnection().CreateCommand();
        serialSequenceCommand.CommandText = "SELECT pg_get_serial_sequence(@tableName, @columnName);";

        var tableParameter = serialSequenceCommand.CreateParameter();
        tableParameter.ParameterName = "tableName";
        tableParameter.Value = $"{SqlIdentifier.QuotePostgreSqlIdentifier(table.Schema ?? "public")}.{SqlIdentifier.QuotePostgreSqlIdentifier(table.Name)}";
        serialSequenceCommand.Parameters.Add(tableParameter);

        var columnParameter = serialSequenceCommand.CreateParameter();
        columnParameter.ParameterName = "columnName";
        columnParameter.Value = column.Name;
        serialSequenceCommand.Parameters.Add(columnParameter);

        var sequence = await serialSequenceCommand.ExecuteScalarAsync();
        if (sequence is string serialSequence && !string.IsNullOrWhiteSpace(serialSequence))
            return serialSequence;

        await using var defaultCommand = target.Database.GetDbConnection().CreateCommand();
        defaultCommand.CommandText = """
            SELECT pg_get_expr(default_value.adbin, default_value.adrelid)
            FROM pg_attrdef default_value
            JOIN pg_attribute attribute
                ON attribute.attrelid = default_value.adrelid
                AND attribute.attnum = default_value.adnum
            JOIN pg_class table_info
                ON table_info.oid = default_value.adrelid
            JOIN pg_namespace schema_info
                ON schema_info.oid = table_info.relnamespace
            WHERE schema_info.nspname = @schemaName
                AND table_info.relname = @tableName
                AND attribute.attname = @columnName;
            """;

        var schemaNameParameter = defaultCommand.CreateParameter();
        schemaNameParameter.ParameterName = "schemaName";
        schemaNameParameter.Value = table.Schema ?? "public";
        defaultCommand.Parameters.Add(schemaNameParameter);

        var tableNameParameter = defaultCommand.CreateParameter();
        tableNameParameter.ParameterName = "tableName";
        tableNameParameter.Value = table.Name;
        defaultCommand.Parameters.Add(tableNameParameter);

        var defaultColumnParameter = defaultCommand.CreateParameter();
        defaultColumnParameter.ParameterName = "columnName";
        defaultColumnParameter.Value = column.Name;
        defaultCommand.Parameters.Add(defaultColumnParameter);

        var defaultExpression = await defaultCommand.ExecuteScalarAsync();
        return defaultExpression is string expression
            ? TryParseNextValSequenceName(expression)
            : null;
    }

    public async Task ResetSequenceAsync(DbContext target, TablePlan table, ColumnPlan column, string sequenceName)
    {
        await using var resetCommand = target.Database.GetDbConnection().CreateCommand();
        resetCommand.CommandText = $"""
            SELECT setval(
                @sequenceName,
                COALESCE((SELECT MAX({SqlIdentifier.QuotePostgreSqlIdentifier(column.Name)}) FROM {SqlIdentifier.QuotePostgreSqlTable(table)}), 1),
                (SELECT COUNT(*) > 0 FROM {SqlIdentifier.QuotePostgreSqlTable(table)})
            );
            """;

        var sequenceParameter = resetCommand.CreateParameter();
        sequenceParameter.ParameterName = "sequenceName";
        sequenceParameter.Value = sequenceName;
        resetCommand.Parameters.Add(sequenceParameter);

        await resetCommand.ExecuteNonQueryAsync();
    }

    private static object NormalizeValue(object value)
    {
        if (value == DBNull.Value)
            return DBNull.Value;

        if (value is DateTime dateTime)
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);

        if (value is DateTimeOffset dateTimeOffset)
            return DateTime.SpecifyKind(dateTimeOffset.DateTime, DateTimeKind.Unspecified);

        return value;
    }

    private static string CreateSelectSql(TablePlan table)
    {
        var columns = string.Join(", ", table.Columns.Select(c => SqlIdentifier.QuoteSqlServerIdentifier(c.Name)));
        return $"SELECT {columns} FROM {SqlIdentifier.QuoteSqlServerTable(table)};";
    }

    private static string CreateInsertSql(TablePlan table)
    {
        var columns = string.Join(", ", table.Columns.Select(c => SqlIdentifier.QuotePostgreSqlIdentifier(c.Name)));
        var parameters = string.Join(", ", table.Columns.Select((_, index) => $"@p{index}"));
        return $"INSERT INTO {SqlIdentifier.QuotePostgreSqlTable(table)} ({columns}) VALUES ({parameters});";
    }

    private static string? TryParseNextValSequenceName(string expression)
    {
        const string prefix = "nextval('";
        var start = expression.IndexOf(prefix, StringComparison.OrdinalIgnoreCase);

        if (start < 0)
            return null;

        start += prefix.Length;
        var end = expression.IndexOf('\'', start);

        if (end <= start)
            return null;

        return expression[start..end];
    }
}
