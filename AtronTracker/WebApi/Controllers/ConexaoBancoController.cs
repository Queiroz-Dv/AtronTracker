using IoC;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Npgsql;
using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace WebApi.Controllers
{
    /// <summary>
    /// Controller interna para validar conexões com bancos distintos.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ConexaoBancoController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _environment;
        private readonly IAtronConnectionStringProvider _connectionStringProvider;

        public ConexaoBancoController(
            IConfiguration configuration,
            IWebHostEnvironment environment,
            IAtronConnectionStringProvider connectionStringProvider)
        {
            _configuration = configuration;
            _environment = environment;
            _connectionStringProvider = connectionStringProvider;
        }

        /// <summary>
        /// Testa a conexao padrao PostgreSQL/Supabase configurada em ConnectionStrings:DefaultConnection.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("default")]
        public async Task<ActionResult<ConexaoBancoTesteResponse>> TestarDefault(CancellationToken cancellationToken)
        {
            if (!EndpointHabilitado())
                return NotFound();

            return await TestarConexaoConfigurada("DefaultConnection", BancoProvider.PostgreSql, cancellationToken);
        }

        /// <summary>
        /// Testa a conexão PostgreSQL/Supabase configurada em ConnectionStrings:DefaultConnection.
        /// </summary>
        [HttpGet("supabase")]
        public async Task<ActionResult<ConexaoBancoTesteResponse>> TestarSupabase(CancellationToken cancellationToken)
        {
            if (!EndpointHabilitado())
                return NotFound();

            return await TestarConexaoConfigurada("DefaultConnection", BancoProvider.PostgreSql, cancellationToken);
        }

        /// <summary>
        /// Testa uma connection string configurada pelo nome e provider informados.
        /// </summary>
        [HttpPost("testar")]
        public async Task<ActionResult<ConexaoBancoTesteResponse>> Testar(
            [FromBody] ConexaoBancoTesteRequest request,
            CancellationToken cancellationToken)
        {
            if (!EndpointHabilitado())
                return NotFound();

            if (request is null)
                return BadRequest("Informe os dados para teste de conexão.");

            if (!Enum.TryParse<BancoProvider>(request.Provider, true, out var provider))
                return BadRequest("Provider inválido. Use PostgreSql.");

            var connectionString = request.ConnectionString;

            if (string.IsNullOrWhiteSpace(connectionString) && !string.IsNullOrWhiteSpace(request.ConnectionStringName))
                connectionString = ResolverConnectionString(request.ConnectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
                return BadRequest("Connection string não informada ou não encontrada.");

            var nome = string.IsNullOrWhiteSpace(request.ConnectionStringName)
                ? "InformadaNoRequest"
                : request.ConnectionStringName;

            var resultado = await TestarConexao(nome, provider, connectionString, cancellationToken);
            return resultado.Sucesso ? Ok(resultado) : StatusCode(503, resultado);
        }

        private async Task<ActionResult<ConexaoBancoTesteResponse>> TestarConexaoConfigurada(
            string connectionStringName,
            BancoProvider provider,
            CancellationToken cancellationToken)
        {
            var connectionString = ResolverConnectionString(connectionStringName);

            if (string.IsNullOrWhiteSpace(connectionString))
                return BadRequest($"Connection string '{connectionStringName}' não encontrada.");

            var resultado = await TestarConexao(connectionStringName, provider, connectionString, cancellationToken);
            return resultado.Sucesso ? Ok(resultado) : StatusCode(503, resultado);
        }

        private string ResolverConnectionString(string connectionStringName)
        {
            if (string.Equals(connectionStringName, AtronConnectionStringProvider.DefaultConnectionName, StringComparison.OrdinalIgnoreCase))
                return _connectionStringProvider.ObterDefaultConnection();

            return _configuration.GetConnectionString(connectionStringName);
        }

        private bool EndpointHabilitado()
        {
            return _environment.IsDevelopment() || _configuration.GetValue<bool>("ConexaoBancoTeste:Habilitado");
        }

        private static async Task<ConexaoBancoTesteResponse> TestarConexao(
            string connectionStringName,
            BancoProvider provider,
            string connectionString,
            CancellationToken cancellationToken)
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                await using var connection = CriarConexao(provider, connectionString);
                await connection.OpenAsync(cancellationToken);

                stopwatch.Stop();

                return new ConexaoBancoTesteResponse
                {
                    Sucesso = true,
                    Provider = provider.ToString(),
                    ConnectionStringName = connectionStringName,
                    Database = connection.Database,
                    DataSource = connection.DataSource,
                    State = connection.State.ToString(),
                    TempoMs = stopwatch.ElapsedMilliseconds,
                    TestadoEmUtc = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                return new ConexaoBancoTesteResponse
                {
                    Sucesso = false,
                    Provider = provider.ToString(),
                    ConnectionStringName = connectionStringName,
                    State = ConnectionState.Closed.ToString(),
                    TempoMs = stopwatch.ElapsedMilliseconds,
                    TestadoEmUtc = DateTime.UtcNow,
                    Erro = ex.Message,
                    ErroTipo = ex.GetType().FullName,
                    ErroInterno = ex.InnerException?.Message
                };
            }
        }

        private static DbConnection CriarConexao(BancoProvider provider, string connectionString)
        {
            return provider switch
            {
                BancoProvider.PostgreSql => new NpgsqlConnection(connectionString),
                _ => throw new NotSupportedException("Provider não suportado.")
            };
        }
    }

    public class ConexaoBancoTesteRequest
    {
        public string Provider { get; set; }
        public string ConnectionStringName { get; set; }
        public string ConnectionString { get; set; }
    }

    public class ConexaoBancoTesteResponse
    {
        public bool Sucesso { get; set; }
        public string Provider { get; set; }
        public string ConnectionStringName { get; set; }
        public string Database { get; set; }
        public string DataSource { get; set; }
        public string State { get; set; }
        public long TempoMs { get; set; }
        public DateTime TestadoEmUtc { get; set; }
        public string Erro { get; set; }
        public string ErroTipo { get; set; }
        public string ErroInterno { get; set; }
    }

    public enum BancoProvider
    {
        PostgreSql
    }
}
