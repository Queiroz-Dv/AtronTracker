using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
            return AtronConnectionStringProvider.Resolve(configuration);
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
