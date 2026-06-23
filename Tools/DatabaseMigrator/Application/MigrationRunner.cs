using DatabaseMigrator.Domain;
using DatabaseMigrator.Infrastructure;
using DatabaseMigrator.Presentation;
using Microsoft.EntityFrameworkCore;

namespace DatabaseMigrator.Application;

internal sealed class MigrationRunner
{
    private readonly MigrationContextFactory _contextFactory;
    private readonly RelationalDataGateway _dataGateway;
    private readonly ConsoleReporter _reporter;

    public MigrationRunner(
        MigrationContextFactory contextFactory,
        RelationalDataGateway dataGateway,
        ConsoleReporter reporter)
    {
        _contextFactory = contextFactory;
        _dataGateway = dataGateway;
        _reporter = reporter;
    }

    public async Task ExecuteAsync(MigratorOptions options)
    {
        foreach (var contextDefinition in _contextFactory.Create(options.Contexts))
        {
            await ExecuteContextAsync(contextDefinition, options);
            _reporter.BlankLine();
        }
    }

    private async Task ExecuteContextAsync(MigrationContextDefinition contextDefinition, MigratorOptions options)
    {
        await using var source = contextDefinition.CreateSource();
        await using var target = contextDefinition.CreateTarget();

        var tables = TablePlan.Create(source).ToList();

        _reporter.ContextStarted(contextDefinition.Name, tables.Count);

        await source.Database.OpenConnectionAsync();
        await target.Database.OpenConnectionAsync();

        if (options.Mode == MigrationMode.DryRun)
        {
            await PrintCountsAsync(source, target, tables);
            return;
        }

        if (options.Mode == MigrationMode.Validate)
        {
            await ValidateAsync(source, target, tables);
            return;
        }

        if (options.ResetTarget)
            await ResetTargetAsync(target, tables);

        foreach (var table in tables)
        {
            var copiedRows = await _dataGateway.CopyTableAsync(source, target, table);
            _reporter.TableCopied(table, copiedRows);
        }

        await ResetSequencesAsync(target, tables);
        await ValidateAsync(source, target, tables);
    }

    private async Task PrintCountsAsync(DbContext source, DbContext target, IReadOnlyCollection<TablePlan> tables)
    {
        _reporter.CountHeader();

        foreach (var table in tables)
        {
            var sourceCount = await _dataGateway.CountAsync(source, table, DatabaseKind.SqlServer);
            var targetCount = await _dataGateway.CountAsync(target, table, DatabaseKind.PostgreSql);
            _reporter.Count(table, sourceCount, targetCount);
        }
    }

    private async Task ValidateAsync(DbContext source, DbContext target, IReadOnlyCollection<TablePlan> tables)
    {
        var hasMismatch = false;
        _reporter.ValidationStarted();

        foreach (var table in tables)
        {
            var sourceCount = await _dataGateway.CountAsync(source, table, DatabaseKind.SqlServer);
            var targetCount = await _dataGateway.CountAsync(target, table, DatabaseKind.PostgreSql);

            if (sourceCount != targetCount)
                hasMismatch = true;

            _reporter.ValidationResult(table, sourceCount, targetCount);
        }

        if (hasMismatch)
            throw new InvalidOperationException("A validacao encontrou divergencia de contagens.");
    }

    private async Task ResetTargetAsync(DbContext target, IReadOnlyCollection<TablePlan> tables)
    {
        _reporter.ResetStarted();

        foreach (var table in tables.Reverse())
        {
            await _dataGateway.DeleteAllAsync(target, table);
            _reporter.TableDeleted(table);
        }
    }

    private async Task ResetSequencesAsync(DbContext target, IReadOnlyCollection<TablePlan> tables)
    {
        _reporter.SequenceResetStarted();

        foreach (var table in tables)
        {
            foreach (var column in table.Columns.Where(c => c.IsIntegerGeneratedOnAdd))
            {
                var sequenceName = await _dataGateway.GetSequenceNameAsync(target, table, column);
                if (sequenceName is null)
                    continue;

                await _dataGateway.ResetSequenceAsync(target, table, column, sequenceName);
                _reporter.SequenceReset(table, column);
            }
        }
    }
}
