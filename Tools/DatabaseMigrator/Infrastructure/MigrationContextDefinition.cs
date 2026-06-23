using Microsoft.EntityFrameworkCore;

namespace DatabaseMigrator.Infrastructure;

internal sealed class MigrationContextDefinition
{
    public string Name { get; }
    public Func<DbContext> CreateSource { get; }
    public Func<DbContext> CreateTarget { get; }

    public MigrationContextDefinition(
        string name,
        Func<DbContext> createSource,
        Func<DbContext> createTarget)
    {
        Name = name;
        CreateSource = createSource;
        CreateTarget = createTarget;
    }
}
