using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.Text.Json;

namespace AtronNotificacoes.Infrastructure.Migrations;

public sealed class NotificacoesDbContextFactory : IDesignTimeDbContextFactory<NotificacoesDbContext>
{
    public NotificacoesDbContext CreateDbContext(string[] args)
    {
        var connectionString = ObterConnectionString();
        var options = new DbContextOptionsBuilder<NotificacoesDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql
                    .MigrationsAssembly(typeof(NotificacoesDbContextFactory).Assembly.FullName)
                    .MigrationsHistoryTable("__AtronNotificacoesMigrationsHistory"))
            .Options;

        return new NotificacoesDbContext(options);
    }

    private static string ObterConnectionString()
    {
        var variavelAmbiente = Environment.GetEnvironmentVariable("NOTIFICACOES_CONNECTION_STRING");
        if (!string.IsNullOrWhiteSpace(variavelAmbiente))
            return variavelAmbiente;

        var caminhoAppSettings = LocalizarAppSettings();

        using var documento = JsonDocument.Parse(File.ReadAllText(caminhoAppSettings));
        return documento.RootElement
            .GetProperty("ConnectionStrings")
            .GetProperty("DefaultConnection")
            .GetString()
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection deve ser configurada para aplicar as migrations de notificações.");
    }

    private static string LocalizarAppSettings()
    {
        for (var diretorio = new DirectoryInfo(Directory.GetCurrentDirectory()); diretorio is not null; diretorio = diretorio.Parent)
        {
            var candidato = Path.Combine(diretorio.FullName, "AtronNotificacoes", "appsettings.json");
            if (File.Exists(candidato))
                return candidato;
        }

        throw new InvalidOperationException("Defina NOTIFICACOES_CONNECTION_STRING ou disponibilize AtronNotificacoes/appsettings.json em um diretório ancestral.");
    }
}
