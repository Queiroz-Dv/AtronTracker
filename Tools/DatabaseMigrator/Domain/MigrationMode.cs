namespace DatabaseMigrator.Domain;

internal enum MigrationMode
{
    DryRun,
    Migrate,
    Validate
}
