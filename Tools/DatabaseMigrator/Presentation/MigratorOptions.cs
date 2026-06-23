using DatabaseMigrator.Domain;

namespace DatabaseMigrator.Presentation;

internal sealed class MigratorOptions
{
    public MigrationMode Mode { get; private init; } = MigrationMode.DryRun;
    public bool ResetTarget { get; private init; }
    public string? SourceConnectionString { get; private init; }
    public string? TargetConnectionString { get; private init; }
    public IReadOnlyCollection<string> Contexts { get; private init; } = Array.Empty<string>();

    public static MigratorOptions Parse(string[] args)
    {
        var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < args.Length; i++)
        {
            var arg = args[i];

            if (!arg.StartsWith("--", StringComparison.Ordinal))
                continue;

            var key = arg[2..];
            if (key.Equals("reset-target", StringComparison.OrdinalIgnoreCase))
            {
                values[key] = "true";
                continue;
            }

            if (i + 1 >= args.Length)
                throw new ArgumentException($"Argumento '{arg}' sem valor.");

            values[key] = args[++i];
        }

        var contexts = Get(values, "contexts", "all")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(value => value.ToLowerInvariant())
            .ToList();

        if (contexts.Contains("all"))
            contexts = new List<string> { "atron", "shared", "stock" };

        return new MigratorOptions
        {
            Mode = ParseMode(Get(values, "mode", "dry-run")),
            ResetTarget = bool.TryParse(Get(values, "reset-target", "false"), out var reset) && reset,
            SourceConnectionString = Get(values, "source", null)
                ?? Environment.GetEnvironmentVariable("Migration__SqlServerConnection")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__AtronConnection")
                ?? Environment.GetEnvironmentVariable("MIGRATION_SQLSERVER_CONNECTION")
                ?? Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION"),
            TargetConnectionString = Get(values, "target", null)
                ?? Environment.GetEnvironmentVariable("Migration__SupabaseConnection")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
                ?? Environment.GetEnvironmentVariable("MIGRATION_SUPABASE_CONNECTION")
                ?? Environment.GetEnvironmentVariable("SUPABASE_CONNECTION"),
            Contexts = contexts
        };
    }

    private static MigrationMode ParseMode(string value)
    {
        return value.Trim().ToLowerInvariant() switch
        {
            "dry-run" => MigrationMode.DryRun,
            "migrate" => MigrationMode.Migrate,
            "validate" => MigrationMode.Validate,
            _ => throw new InvalidOperationException($"Modo '{value}' invalido. Use dry-run, migrate ou validate.")
        };
    }

    private static string Get(IReadOnlyDictionary<string, string> values, string key, string? fallback)
    {
        return values.TryGetValue(key, out var value) ? value : fallback ?? string.Empty;
    }
}
