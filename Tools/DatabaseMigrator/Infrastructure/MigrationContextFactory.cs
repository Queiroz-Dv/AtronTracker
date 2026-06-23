using AtronStock.Infrastructure.Context;
using AtronTracker.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Shared.Infrastructure.Context;

namespace DatabaseMigrator.Infrastructure;

internal sealed class MigrationContextFactory
{
    private readonly string _sourceConnectionString;
    private readonly string _targetConnectionString;

    public MigrationContextFactory(string sourceConnectionString, string targetConnectionString)
    {
        _sourceConnectionString = sourceConnectionString;
        _targetConnectionString = targetConnectionString;
    }

    public IEnumerable<MigrationContextDefinition> Create(IReadOnlyCollection<string> contexts)
    {
        if (contexts.Contains("atron"))
        {
            yield return new MigrationContextDefinition(
                "AtronDbContext",
                () => new AtronDbContext(new DbContextOptionsBuilder<AtronDbContext>().UseSqlServer(_sourceConnectionString).Options),
                () => new AtronDbContext(new DbContextOptionsBuilder<AtronDbContext>().UseNpgsql(_targetConnectionString).Options));
        }

        if (contexts.Contains("shared"))
        {
            yield return new MigrationContextDefinition(
                "SharedDbContext",
                () => new SharedDbContext(new DbContextOptionsBuilder<SharedDbContext>().UseSqlServer(_sourceConnectionString).Options),
                () => new SharedDbContext(new DbContextOptionsBuilder<SharedDbContext>().UseNpgsql(_targetConnectionString).Options));
        }

        if (contexts.Contains("stock"))
        {
            yield return new MigrationContextDefinition(
                "StockDbContext",
                () => new StockDbContext(new DbContextOptionsBuilder<StockDbContext>().UseSqlServer(_sourceConnectionString).Options),
                () => new StockDbContext(new DbContextOptionsBuilder<StockDbContext>().UseNpgsql(_targetConnectionString).Options));
        }
    }
}
