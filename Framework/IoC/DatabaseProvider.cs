using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;

namespace IoC
{
    internal enum DatabaseProvider
    {
        SqlServer,
        PostgreSql
    }

    internal sealed class DatabaseProviderConfiguration
    {
        public DatabaseProvider Provider { get; init; }
        public string ConnectionStringName { get; init; }
        public string ConnectionString { get; init; }
    }

    internal static class DatabaseProviderResolver
    {
        public static DatabaseProviderConfiguration Resolve(IConfiguration configuration)
        {
            var providerValue = configuration["Database:Provider"];
            var provider = ParseProvider(providerValue);
            var connectionStringName = configuration["Database:ConnectionStringName"];

            if (string.IsNullOrWhiteSpace(connectionStringName))
            {
                connectionStringName = provider == DatabaseProvider.PostgreSql
                    ? "DefaultConnection"
                    : "AtronConnection";
            }

            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Connection string '{connectionStringName}' nao encontrada para o provider '{provider}'.");

            return new DatabaseProviderConfiguration
            {
                Provider = provider,
                ConnectionStringName = connectionStringName,
                ConnectionString = connectionString
            };
        }

        public static void UseConfiguredDatabase(
            this DbContextOptionsBuilder options,
            DatabaseProviderConfiguration database,
            string migrationsAssembly)
        {
            switch (database.Provider)
            {
                case DatabaseProvider.SqlServer:
                    options.UseSqlServer(database.ConnectionString, builder => builder.MigrationsAssembly(migrationsAssembly));
                    break;
                case DatabaseProvider.PostgreSql:
                    options.UseNpgsql(database.ConnectionString, builder => builder.MigrationsAssembly(migrationsAssembly));
                    break;
                default:
                    throw new NotSupportedException($"Provider '{database.Provider}' nao suportado.");
            }
        }

        public static string ResolveMigrationsAssembly(
            this DatabaseProviderConfiguration database,
            string defaultMigrationsAssembly,
            string postgreSqlMigrationsAssembly)
        {
            return database.Provider == DatabaseProvider.PostgreSql
                ? postgreSqlMigrationsAssembly
                : defaultMigrationsAssembly;
        }

        public static void UseConfiguredDatabase(
            this DbContextOptionsBuilder options,
            DatabaseProviderConfiguration database,
            DbConnection connection)
        {
            switch (database.Provider)
            {
                case DatabaseProvider.SqlServer:
                    options.UseSqlServer(connection);
                    break;
                case DatabaseProvider.PostgreSql:
                    options.UseNpgsql(connection);
                    break;
                default:
                    throw new NotSupportedException($"Provider '{database.Provider}' nao suportado.");
            }
        }

        private static DatabaseProvider ParseProvider(string providerValue)
        {
            if (string.IsNullOrWhiteSpace(providerValue))
                return DatabaseProvider.SqlServer;

            return providerValue.Trim().ToLowerInvariant() switch
            {
                "sqlserver" => DatabaseProvider.SqlServer,
                "sql-server" => DatabaseProvider.SqlServer,
                "mssql" => DatabaseProvider.SqlServer,
                "postgresql" => DatabaseProvider.PostgreSql,
                "postgres" => DatabaseProvider.PostgreSql,
                "supabase" => DatabaseProvider.PostgreSql,
                _ => throw new InvalidOperationException($"Provider de banco '{providerValue}' invalido. Use SqlServer, PostgreSql ou Supabase.")
            };
        }
    }
}
