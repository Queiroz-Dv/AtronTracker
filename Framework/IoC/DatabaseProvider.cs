using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.Common;

namespace IoC
{
    internal sealed class DatabaseProviderConfiguration
    {
        public string ConnectionStringName { get; init; }
        public string ConnectionString { get; init; }
    }

    internal static class DatabaseProviderResolver
    {
        public static DatabaseProviderConfiguration Resolve(IConfiguration configuration)
        {
            const string connectionStringName = "DefaultConnection";
            var connectionString = configuration.GetConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Connection string '{connectionStringName}' nao encontrada.");

            return new DatabaseProviderConfiguration
            {
                ConnectionStringName = connectionStringName,
                ConnectionString = connectionString
            };
        }

        public static void UseConfiguredDatabase(
            this DbContextOptionsBuilder options,
            DatabaseProviderConfiguration database,
            string migrationsAssembly)
        {
            options.UseNpgsql(database.ConnectionString, builder => builder.MigrationsAssembly(migrationsAssembly));
        }

        public static void UseConfiguredDatabase(
            this DbContextOptionsBuilder options,
            DatabaseProviderConfiguration database,
            DbConnection connection)
        {
            options.UseNpgsql(connection);
        }
    }
}
