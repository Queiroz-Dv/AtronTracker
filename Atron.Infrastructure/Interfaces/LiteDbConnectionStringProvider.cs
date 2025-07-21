using Atron.Infrastructure.Models;
using Microsoft.Extensions.Options;
using System.IO;

namespace Atron.Infrastructure.Interfaces
{
    public class LiteDbConnectionStringProvider
    {
        public string ConnectionString { get; }

        public LiteDbConnectionStringProvider(IOptions<LiteDbOptions> options)
        {
            var basePath = Path.GetFullPath(options.Value.DatabasePath);
            ConnectionString = string.IsNullOrEmpty(options.Value.Password)
                ? basePath
                : $"Filename={basePath};Password={options.Value.Password};";
        }
    }
}
