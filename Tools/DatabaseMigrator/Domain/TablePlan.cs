using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;

namespace DatabaseMigrator.Domain;

internal sealed class TablePlan
{
    public string? Schema { get; }
    public string Name { get; }
    public IReadOnlyList<ColumnPlan> Columns { get; }
    public IReadOnlyCollection<TableKey> DependsOn { get; }
    public string DisplayName => Schema is null ? Name : $"{Schema}.{Name}";
    public TableKey Key => new(Schema, Name);

    private TablePlan(
        string? schema,
        string name,
        IReadOnlyList<ColumnPlan> columns,
        IReadOnlyCollection<TableKey> dependsOn)
    {
        Schema = schema;
        Name = name;
        Columns = columns;
        DependsOn = dependsOn;
    }

    public static IEnumerable<TablePlan> Create(DbContext context)
    {
        var rawTables = context.Model.GetEntityTypes()
            .Where(entity => entity.GetTableName() is not null)
            .GroupBy(entity => new TableKey(entity.GetSchema(), entity.GetTableName()!))
            .Select(group => CreateFromGroup(group.Key, group))
            .ToDictionary(table => table.Key);

        return SortByDependencies(rawTables);
    }

    private static TablePlan CreateFromGroup(TableKey tableKey, IEnumerable<IEntityType> entityTypes)
    {
        var storeObject = StoreObjectIdentifier.Table(tableKey.Name, tableKey.Schema);
        var columns = entityTypes
            .SelectMany(entity => entity.GetProperties())
            .Select(property => CreateColumn(property, storeObject))
            .Where(column => column is not null)
            .Select(column => column!)
            .GroupBy(column => column.Name)
            .Select(group => group.First())
            .OrderBy(column => column.Name)
            .ToList();

        var dependencies = entityTypes
            .SelectMany(entity => entity.GetForeignKeys())
            .Select(foreignKey => new TableKey(
                foreignKey.PrincipalEntityType.GetSchema(),
                foreignKey.PrincipalEntityType.GetTableName()!))
            .Where(key => key != tableKey)
            .Distinct()
            .ToList();

        return new TablePlan(tableKey.Schema, tableKey.Name, columns, dependencies);
    }

    private static ColumnPlan? CreateColumn(IProperty property, StoreObjectIdentifier storeObject)
    {
        var columnName = property.GetColumnName(storeObject);

        if (columnName is null || property.GetComputedColumnSql(storeObject) is not null)
            return null;

        var propertyType = Nullable.GetUnderlyingType(property.ClrType) ?? property.ClrType;
        var isInteger = propertyType == typeof(short)
            || propertyType == typeof(int)
            || propertyType == typeof(long);

        return new ColumnPlan(
            columnName,
            isInteger && property.ValueGenerated == ValueGenerated.OnAdd);
    }

    private static IEnumerable<TablePlan> SortByDependencies(IReadOnlyDictionary<TableKey, TablePlan> tables)
    {
        var pending = tables.Values.ToDictionary(table => table.Key);
        var emitted = new HashSet<TableKey>();

        while (pending.Count > 0)
        {
            var ready = pending.Values
                .Where(table => table.DependsOn.All(dependency => !tables.ContainsKey(dependency) || emitted.Contains(dependency)))
                .OrderBy(table => table.DisplayName)
                .ToList();

            if (ready.Count == 0)
                throw new InvalidOperationException("Nao foi possivel ordenar as tabelas por dependencias. Pode existir ciclo de FK.");

            foreach (var table in ready)
            {
                pending.Remove(table.Key);
                emitted.Add(table.Key);
                yield return table;
            }
        }
    }
}
