using Xunit;

namespace Platform.Tests;

public class ModularArchitectureTests
{
    [Fact]
    public void HostNeutroCompoeStockSemDependerDoIoCGlobal()
    {
        var conteudo = LerArquivo(
            "AtronPlatform/WebApi/AtronPlatform.WebApi.csproj");

        Assert.DoesNotContain("Framework/IoC", conteudo);
        Assert.Contains("Modules/Stock", conteudo);
        Assert.DoesNotContain("AtronStock/WebApi", conteudo);
    }

    [Theory]
    [InlineData("AtronPlatform/Modules/Tracker/Domain/AtronTracker.Domain.csproj")]
    [InlineData("AtronPlatform/Modules/Tracker/Application/AtronTracker.Application.csproj")]
    [InlineData("AtronPlatform/Modules/Tracker/Infrastructure/AtronTracker.Infrastructure.csproj")]
    public void ProjetosDoTrackerNaoDependemDoStockOuDoIoCGlobal(
        string caminhoProjeto)
    {
        var conteudo = LerArquivo(caminhoProjeto);

        Assert.DoesNotContain("AtronStock/", conteudo);
        Assert.DoesNotContain("Framework/IoC", conteudo);
    }

    [Fact]
    public void DomainDoTrackerNaoPossuiReferenciasDeProjeto()
    {
        var referencias = LerReferenciasDeProjeto(
            "AtronPlatform/Modules/Tracker/Domain/AtronTracker.Domain.csproj");

        Assert.Empty(referencias);
    }

    [Fact]
    public void ApplicationDoTrackerNaoReferenciaInfrastructureOuWebApi()
    {
        var referencias = LerReferenciasDeProjeto(
            "AtronPlatform/Modules/Tracker/Application/AtronTracker.Application.csproj");

        Assert.DoesNotContain("/Infrastructure/", referencias);
        Assert.DoesNotContain("/WebApi/", referencias);
    }

    [Fact]
    public void InfrastructureDoTrackerNaoReferenciaWebApis()
    {
        var referencias = LerReferenciasDeProjeto(
            "AtronPlatform/Modules/Tracker/Infrastructure/AtronTracker.Infrastructure.csproj");

        Assert.DoesNotContain("/WebApi/", referencias);
    }

    [Fact]
    public void SharedNaoReferenciaProjetosDeModulosDeProduto()
    {
        var conteudo = LerReferenciasDeProjeto("Framework/Shared/Shared.csproj");

        Assert.DoesNotContain("AtronPlatform/Modules/", conteudo);
        Assert.DoesNotContain("AtronStock/", conteudo);
    }

    [Theory]
    [InlineData("AtronPlatform/Modules/Stock/Domain/AtronStock.Domain.csproj")]
    [InlineData("AtronPlatform/Modules/Stock/Application/AtronStock.Application.csproj")]
    [InlineData("AtronPlatform/Modules/Stock/Infrastructure/AtronStock.Infrastructure.csproj")]
    public void ProjetosDoStockNaoDependemDoTrackerOuDoIoCGlobal(
        string caminhoProjeto)
    {
        var conteudo = LerArquivo(caminhoProjeto);

        Assert.DoesNotContain("Modules/Tracker/", conteudo);
        Assert.DoesNotContain("AtronTracker.", conteudo);
        Assert.DoesNotContain("Framework/IoC", conteudo);
    }

    [Fact]
    public void CamadasDoStockRespeitamDirecaoDeDependencias()
    {
        var referenciasDomain = LerReferenciasDeProjeto(
            "AtronPlatform/Modules/Stock/Domain/AtronStock.Domain.csproj");
        var referenciasApplication = LerReferenciasDeProjeto(
            "AtronPlatform/Modules/Stock/Application/AtronStock.Application.csproj");
        var referenciasInfrastructure = LerReferenciasDeProjeto(
            "AtronPlatform/Modules/Stock/Infrastructure/AtronStock.Infrastructure.csproj");

        Assert.DoesNotContain("/Application/", referenciasDomain);
        Assert.DoesNotContain("/Infrastructure/", referenciasDomain);
        Assert.DoesNotContain("/Infrastructure/", referenciasApplication);
        Assert.DoesNotContain("/WebApi/", referenciasApplication);
        Assert.Contains("/Application/", referenciasInfrastructure);
        Assert.DoesNotContain("/WebApi/", referenciasInfrastructure);
    }

    [Fact]
    public void AutorizacaoEComposicaoDoTrackerPertencemAoModulo()
    {
        Assert.True(ExisteArquivo(
            "Framework/Shared/Authorization/ModuloPolicies.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/Modules/Tracker/Infrastructure/DependencyInjection/TrackerModuleServiceCollectionExtensions.cs"));
        Assert.False(ExisteArquivo(
            "AtronPlatform/Modules/Tracker/Infrastructure/Authorization/ModuloPolicies.cs"));
        Assert.False(ExisteArquivo(
            "Framework/IoC/DependencyInjectionContainerAPI.cs"));
    }

    [Fact]
    public void AuditoriaPossuiComposicaoEControllerTransversais()
    {
        Assert.True(ExisteArquivo(
            "Framework/Shared/Infrastructure/DependencyInjection/AuditoriaServiceCollectionExtensions.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Transversais/AuditoriaController.cs"));
        Assert.False(ExisteArquivo(
            "AtronAuditoria/Controllers/AuditoriaController.cs"));
        Assert.False(ExisteArquivo("AtronAuditoria/AtronAuditoria.csproj"));
        Assert.False(ExisteArquivo("AtronAuditoria/Program.cs"));
        Assert.False(ExisteArquivo("AtronAuditoria/Startup.cs"));
    }

    [Fact]
    public void HostsLegadosEIoCGlobalForamRemovidosDaSolucao()
    {
        var solution = LerArquivo("AtronPlatform.sln");

        Assert.False(ExisteArquivo(
            "AtronTracker/WebApi/AtronTracker.WebApi.csproj"));
        Assert.False(ExisteArquivo("AtronTracker/WebApi/Program.cs"));
        Assert.False(ExisteArquivo("AtronTracker/WebApi/Startup.cs"));
        Assert.DoesNotContain("AtronTracker/WebApi", solution);
        Assert.DoesNotContain("AtronTracker.WebApi", solution);
        Assert.DoesNotContain("AtronAuditoria/", solution);
        Assert.DoesNotContain("\"AtronAuditoria\"", solution);
        Assert.False(ExisteArquivo("AtronStock/WebApi/AtronStock.WebApi.csproj"));
        Assert.False(ExisteArquivo("AtronStock/WebApi/Program.cs"));
        Assert.False(ExisteArquivo("AtronStock/WebApi/Startup.cs"));
        Assert.DoesNotContain("AtronStock/WebApi", solution);
        Assert.DoesNotContain("AtronStock.WebApi", solution);
        Assert.False(ExisteArquivo("Framework/IoC/IoC.csproj"));
        Assert.DoesNotContain("Framework/IoC", solution);
        Assert.DoesNotContain("\"IoC\"", solution);
    }

    [Fact]
    public void StockPossuiComposicaoEControllersNoHostNeutro()
    {
        Assert.True(ExisteArquivo(
            "AtronPlatform/Modules/Stock/Infrastructure/DependencyInjection/StockModuleServiceCollectionExtensions.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Stock/CategoriaController.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Stock/ClienteController.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Stock/EstoqueController.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Stock/FornecedorController.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Stock/ProdutoController.cs"));
    }

    [Fact]
    public void NotificacoesSaoCompostasInProcessComoCapacidadeTransversal()
    {
        var startup = LerArquivo("AtronPlatform/WebApi/Startup.cs");
        var composicaoTracker = LerArquivo(
            "AtronPlatform/Modules/Tracker/Infrastructure/DependencyInjection/TrackerModuleServiceCollectionExtensions.cs");

        Assert.True(ExisteArquivo(
            "Framework/AtronNotificacoes/Infrastructure/DependencyInjection/NotificacoesInternasServiceCollectionExtensions.cs"));
        Assert.True(ExisteArquivo(
            "Framework/AtronNotificacoes/Infrastructure/NotificacoesInternasInProcessPublisher.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/WebApi/Controllers/Transversais/NotificacoesInternasController.cs"));
        Assert.Contains("AddNotificacoesInternasCapability", startup);
        Assert.DoesNotContain("AddAtronNotificacoesClient", startup);
        Assert.False(ExisteArquivo(
            "Framework/AtronNotificacoes/Client/NotificacoesInternasHttpPublisher.cs"));
        Assert.False(ExisteArquivo(
            "Framework/AtronNotificacoes/Client/NotificacoesInternasHttpConsultaClient.cs"));
        Assert.DoesNotContain("NotificacoesInternasHttpPublisher", composicaoTracker);
        Assert.DoesNotContain("ConfigureNotificacoesTransversais", composicaoTracker);
    }

    [Fact]
    public void DbContextsEMigrationsPossuemProprietariosExplicitos()
    {
        Assert.True(ExisteArquivo(
            "AtronPlatform/Modules/Tracker/Infrastructure/Context/AtronDbContext.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/Modules/Tracker/Infrastructure/Migrations/AtronDbContextModelSnapshot.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/Modules/Stock/Infrastructure/Context/StockDbContext.cs"));
        Assert.True(ExisteArquivo(
            "AtronPlatform/Modules/Stock/Infrastructure/Migrations/StockDbContextModelSnapshot.cs"));
        Assert.True(ExisteArquivo(
            "Framework/Shared/Infrastructure/Context/SharedDbContext.cs"));
        Assert.True(ExisteArquivo(
            "Framework/Shared/Migrations/SharedDbContextModelSnapshot.cs"));
        Assert.True(ExisteArquivo(
            "Framework/Shared/Migrations/Framework.Shared.Migrations.csproj"));
        Assert.True(ExisteArquivo(
            "Framework/AtronNotificacoes/Infrastructure/NotificacoesDbContext.cs"));
        Assert.True(ExisteArquivo(
            "Framework/AtronNotificacoes/Infrastructure/Migrations/NotificacoesDbContextModelSnapshot.cs"));
    }

    [Fact]
    public void PlatformUsaOAssemblyProprietarioDasMigrationsDeNotificacoes()
    {
        var composicao = LerArquivo(
            "Framework/AtronNotificacoes/Infrastructure/DependencyInjection/NotificacoesInternasServiceCollectionExtensions.cs");

        Assert.Contains(
            "typeof(NotificacoesDbContext).Assembly.GetName().Name",
            composicao);
        Assert.DoesNotContain(
            "MigrationsAssembly(\"AtronNotificacoes.Infrastructure\")",
            composicao);
    }

    [Fact]
    public void HostIndependenteDeNotificacoesFoiRemovido()
    {
        var solution = LerArquivo("AtronPlatform.sln");

        Assert.False(ExisteArquivo("AtronNotificacoes/AtronNotificacoes.csproj"));
        Assert.False(ExisteArquivo("AtronNotificacoes/Program.cs"));
        Assert.False(ExisteArquivo("AtronNotificacoes/Startup.cs"));
        Assert.DoesNotContain("AtronNotificacoes/AtronNotificacoes.csproj", solution);
        Assert.DoesNotContain(
            "\"AtronNotificacoes\", \"AtronNotificacoes/",
            solution);
    }

    [Fact]
    public void DockerfilePublicaEExecutaOHostNeutro()
    {
        var dockerfile = LerArquivo("Dockerfile");

        Assert.Contains(
            "AtronPlatform/WebApi/AtronPlatform.WebApi.csproj",
            dockerfile);
        Assert.Contains("dotnet AtronPlatform.WebApi.dll", dockerfile);
        Assert.DoesNotContain(
            "AtronTracker/WebApi/AtronTracker.WebApi.csproj",
            dockerfile);
        Assert.DoesNotContain("dotnet AtronTracker.WebApi.dll", dockerfile);
    }

    [Fact]
    public void ContratosTransversaisNaoExpoemTiposDeModulosDeProduto()
    {
        var raizContratos = Path.Combine(
            ObterRaizDoRepositorio(),
            "Framework",
            "Shared",
            "Application",
            "Interfaces");
        var conteudo = string.Join(
            Environment.NewLine,
            Directory.GetFiles(raizContratos, "*.cs", SearchOption.AllDirectories)
                .Select(File.ReadAllText));

        Assert.DoesNotContain("AtronTracker", conteudo);
        Assert.DoesNotContain("AtronStock", conteudo);
        Assert.DoesNotContain("using Application.", conteudo);
        Assert.DoesNotContain("using Domain.", conteudo);
    }

    private static string LerArquivo(string caminhoRelativo)
    {
        var conteudo = File.ReadAllText(
            Path.Combine(ObterRaizDoRepositorio(), caminhoRelativo));

        return conteudo.Replace('\\', '/');
    }

    private static string LerReferenciasDeProjeto(string caminhoRelativo)
    {
        var caminho = Path.Combine(
            ObterRaizDoRepositorio(),
            caminhoRelativo);
        var referencias = File.ReadLines(caminho)
            .Where(linha => linha.Contains(
                "<ProjectReference",
                StringComparison.Ordinal))
            .Select(linha => linha.Replace('\\', '/'));

        return string.Join(Environment.NewLine, referencias);
    }

    private static bool ExisteArquivo(string caminhoRelativo)
    {
        return File.Exists(Path.Combine(
            ObterRaizDoRepositorio(),
            caminhoRelativo));
    }

    private static string ObterRaizDoRepositorio()
    {
        var diretorio = new DirectoryInfo(AppContext.BaseDirectory);

        while (diretorio is not null &&
               !File.Exists(Path.Combine(diretorio.FullName, "AtronPlatform.sln")))
        {
            diretorio = diretorio.Parent;
        }

        return diretorio?.FullName
            ?? throw new DirectoryNotFoundException(
                "A raiz do repositório Atron não foi encontrada.");
    }
}
