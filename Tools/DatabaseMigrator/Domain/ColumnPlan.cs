namespace DatabaseMigrator.Domain;

internal sealed record ColumnPlan(string Name, bool IsIntegerGeneratedOnAdd);
