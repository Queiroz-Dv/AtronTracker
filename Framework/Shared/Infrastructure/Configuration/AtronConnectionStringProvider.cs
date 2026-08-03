using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Shared.Infrastructure.Configuration
{
    public interface IAtronConnectionStringProvider
    {
        string ObterDefaultConnection();
    }

    public sealed class AtronConnectionStringProvider : IAtronConnectionStringProvider
    {
        public const string DefaultConnectionName = "DefaultConnection";

        private readonly IConfiguration _configuration;

        public AtronConnectionStringProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string ObterDefaultConnection()
        {
            return Resolve(_configuration).ConnectionString;
        }

        public static DatabaseProviderConfiguration Resolve(IConfiguration configuration)
        {
            var connectionString =
                configuration["ATRON_CONNECTION_STRING"]
                ?? configuration.GetConnectionString(DefaultConnectionName)
                ?? configuration["ConnectionString"]
                ?? TryReadAtronTrackerConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException($"Connection string '{DefaultConnectionName}' nao encontrada.");

            return new DatabaseProviderConfiguration
            {
                ConnectionStringName = DefaultConnectionName,
                ConnectionString = connectionString
            };
        }

        private static string TryReadAtronTrackerConnectionString()
        {
            foreach (var basePath in GetSearchBasePaths())
            {
                var current = new DirectoryInfo(basePath);

                while (current is not null)
                {
                    var candidate = Path.Combine(
                        current.FullName,
                        "AtronTracker",
                        "WebApi",
                        "appsettings.json");

                    var connectionString = TryReadConnectionString(candidate);
                    if (!string.IsNullOrWhiteSpace(connectionString))
                        return connectionString;

                    current = current.Parent;
                }
            }

            return null;
        }

        private static IEnumerable<string> GetSearchBasePaths()
        {
            yield return Directory.GetCurrentDirectory();
            yield return AppContext.BaseDirectory;
        }

        private static string TryReadConnectionString(string filePath)
        {
            if (!File.Exists(filePath))
                return null;

            using var document = JsonDocument.Parse(File.ReadAllText(filePath));
            var root = document.RootElement;

            if (root.TryGetProperty("ConnectionStrings", out var connectionStrings)
                && TryGetString(connectionStrings, DefaultConnectionName, out var defaultConnectionString))
            {
                return defaultConnectionString;
            }

            if (TryGetString(root, "ConnectionString", out var plainConnectionString))
                return plainConnectionString;

            return null;
        }

        private static bool TryGetString(JsonElement element, string propertyName, out string value)
        {
            value = null;

            if (!element.TryGetProperty(propertyName, out var property)
                || property.ValueKind != JsonValueKind.String)
            {
                return false;
            }

            value = property.GetString();
            return !string.IsNullOrWhiteSpace(value);
        }
    }
}
