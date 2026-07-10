using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestDbController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<TestDbController> _logger;

        public TestDbController(IConfiguration configuration, ILogger<TestDbController> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("connection")]
        public async Task<IActionResult> GetConnection()
        {
            var connStr = _configuration.GetConnectionString("DefaultConnection")
                          ?? _configuration["ConnectionStrings:DefaultConnection"];

            if (string.IsNullOrWhiteSpace(connStr))
                return BadRequest(new { ok = false, message = "Connection string not found." });

            try
            {
                var csb = BuildConnectionString(connStr);
                // Ensure SSL for Supabase
                csb.SslMode = SslMode.Require;
                csb.TrustServerCertificate = true;

                await using var conn = new NpgsqlConnection(csb.ConnectionString);
                await conn.OpenAsync();
                var version = conn.PostgreSqlVersion?.ToString() ?? "unknown";
                await conn.CloseAsync();

                return Ok(new { ok = true, serverVersion = version });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test DB connection failed.");
                return StatusCode(500, new { ok = false, message = "DB connection failed", detail = ex.Message });
            }
        }

        private static NpgsqlConnectionStringBuilder BuildConnectionString(string connStr)
        {
            // Support URI form: postgresql://user:pass@host:port/db
            if (connStr.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
                connStr.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                var uri = new Uri(connStr);
                var userInfo = uri.UserInfo.Split(':', 2);
                var builder = new NpgsqlConnectionStringBuilder
                {
                    Host = uri.Host,
                    Port = uri.Port > 0 ? uri.Port : 5432,
                    Username = Uri.UnescapeDataString(userInfo.Length > 0 ? userInfo[0] : "postgres"),
                    Password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : null,
                    Database = uri.AbsolutePath?.TrimStart('/') ?? "postgres"
                };
                return builder;
            }

            // Key/value form
            return new NpgsqlConnectionStringBuilder(connStr);
        }
    }
}
