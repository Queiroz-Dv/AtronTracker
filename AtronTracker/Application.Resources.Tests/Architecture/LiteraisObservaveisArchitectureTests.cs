using System.Text.RegularExpressions;
using Xunit;

namespace Application.Resources.Tests.Architecture;

public class LiteraisObservaveisArchitectureTests
{
    [Fact]
    public void CodigoDeProducaoNaoDeveCriarMensagensObservaveisOuHtmlForaDosPontosPermitidos()
    {
        var violacoes = LiteralObservavelVerifier.Verificar(ObterRaizDaSolucao());

        Assert.True(
            violacoes.Count == 0,
            $"Literais observaveis encontrados:{Environment.NewLine}{string.Join(Environment.NewLine, violacoes)}");
    }

    [Theory]
    [InlineData("return Resultado.Falha(\"Erro\");", "Resultado.Falha")]
    [InlineData("context.AdicionarErro(\"Erro\");", "AdicionarErro")]
    [InlineData("resultado.AdicionarAviso(\"Aviso\");", "AdicionarAviso")]
    [InlineData("var email = new EmailRequest { Assunto = \"Assunto\" };", "Assunto de EmailRequest")]
    [InlineData("return \"<html><body>conteudo</body></html>\";", "HTML")]
    public void VerificadorDeveDetectarPadroesProibidos(string codigo, string regraEsperada)
    {
        var violacoes = LiteralObservavelVerifier.VerificarConteudo("Application/Services/Exemplo.cs", codigo);

        Assert.Contains(violacoes, violacao => violacao.Contains(regraEsperada, StringComparison.Ordinal));
    }

    private static string ObterRaizDaSolucao()
    {
        for (var diretorio = new DirectoryInfo(AppContext.BaseDirectory);
             diretorio is not null;
             diretorio = diretorio.Parent)
        {
            if (File.Exists(Path.Combine(diretorio.FullName, "AtronPlatform.sln")))
                return diretorio.FullName;
        }

        throw new DirectoryNotFoundException("Nao foi possivel localizar AtronPlatform.sln a partir do diretorio de testes.");
    }
}

internal static class LiteralObservavelVerifier
{
    private static readonly Regex ResultadoFalhaLiteral = CriarRegex("\\bResultado(?:<[^>\\r\\n]+>)?\\.Falha\\s*\\(\\s*\\$?@?\\\"");
    private static readonly Regex AdicionarErroLiteral = CriarRegex("\\bAdicionarErro\\s*\\(\\s*\\$?@?\\\"");
    private static readonly Regex AdicionarAvisoLiteral = CriarRegex("\\bAdicionarAviso\\s*\\(\\s*\\$?@?\\\"");
    private static readonly Regex AssuntoLiteralEmailRequest = CriarRegex("new\\s+EmailRequest\\s*\\{(?:(?!\\}).)*\\bAssunto\\s*=\\s*\\$?@?\\\"", RegexOptions.Singleline);
    private static readonly Regex HtmlEmCodigo = CriarRegex(@"<!DOCTYPE\s+html|<html\b|<body\b", RegexOptions.IgnoreCase);

    private static readonly IReadOnlyDictionary<string, string> ExcecoesPorArquivo = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        ["Framework/Shared/Application/Services/Email/SharedEmailService.cs"] = "falhas tecnicas do adaptador de transporte, sem retorno de mensagem de produto"
    };

    public static IReadOnlyList<string> Verificar(string raizDaSolucao)
    {
        var violacoes = new List<string>();
        foreach (var arquivo in Directory.EnumerateFiles(raizDaSolucao, "*.cs", SearchOption.AllDirectories))
        {
            var caminhoRelativo = Path.GetRelativePath(raizDaSolucao, arquivo).Replace('\\', '/');
            if (!DeveAnalisar(caminhoRelativo))
                continue;

            var conteudo = File.ReadAllText(arquivo);
            violacoes.AddRange(VerificarConteudo(caminhoRelativo, conteudo));
        }

        return violacoes;
    }

    public static IReadOnlyList<string> VerificarConteudo(string caminhoRelativo, string conteudo)
    {
        if (ExcecoesPorArquivo.ContainsKey(caminhoRelativo))
            return [];

        var violacoes = new List<string>();
        AdicionarViolacaoSeEncontrar(violacoes, caminhoRelativo, conteudo, ResultadoFalhaLiteral, "Resultado.Falha");
        AdicionarViolacaoSeEncontrar(violacoes, caminhoRelativo, conteudo, AdicionarErroLiteral, "AdicionarErro");
        AdicionarViolacaoSeEncontrar(violacoes, caminhoRelativo, conteudo, AdicionarAvisoLiteral, "AdicionarAviso");
        AdicionarViolacaoSeEncontrar(violacoes, caminhoRelativo, conteudo, AssuntoLiteralEmailRequest, "Assunto de EmailRequest");

        if (EhServicoOuCasoDeUso(caminhoRelativo))
            AdicionarViolacaoSeEncontrar(violacoes, caminhoRelativo, conteudo, HtmlEmCodigo, "HTML em servico ou caso de uso");

        return violacoes;
    }

    private static bool DeveAnalisar(string caminhoRelativo)
    {
        if (!caminhoRelativo.StartsWith("AtronTracker/", StringComparison.OrdinalIgnoreCase) &&
            !caminhoRelativo.StartsWith("AtronStock/", StringComparison.OrdinalIgnoreCase) &&
            !caminhoRelativo.StartsWith("Framework/", StringComparison.OrdinalIgnoreCase) &&
            !caminhoRelativo.StartsWith("AtronEmail/", StringComparison.OrdinalIgnoreCase))
            return false;

        return !caminhoRelativo.Contains("/bin/", StringComparison.OrdinalIgnoreCase) &&
               !caminhoRelativo.Contains("/obj/", StringComparison.OrdinalIgnoreCase) &&
               !caminhoRelativo.Contains(".Tests/", StringComparison.OrdinalIgnoreCase) &&
               !caminhoRelativo.Contains("/Migrations/", StringComparison.OrdinalIgnoreCase) &&
               !caminhoRelativo.EndsWith(".Designer.cs", StringComparison.OrdinalIgnoreCase) &&
               !caminhoRelativo.EndsWith("PlanejamentoCustoRelatorioHtmlMontador.cs", StringComparison.OrdinalIgnoreCase);
    }

    private static bool EhServicoOuCasoDeUso(string caminhoRelativo)
        => caminhoRelativo.Contains("/Services/", StringComparison.OrdinalIgnoreCase) ||
           caminhoRelativo.Contains("/UseCases/", StringComparison.OrdinalIgnoreCase);

    private static void AdicionarViolacaoSeEncontrar(
        ICollection<string> violacoes,
        string caminhoRelativo,
        string conteudo,
        Regex padrao,
        string regra)
    {
        var match = padrao.Match(conteudo);
        if (match.Success)
            violacoes.Add($"{caminhoRelativo}: {regra} na linha {ObterLinha(conteudo, match.Index)}.");
    }

    private static int ObterLinha(string conteudo, int indice)
        => conteudo.AsSpan(0, indice).Count('\n') + 1;

    private static Regex CriarRegex(string padrao, RegexOptions opcoes = RegexOptions.None)
        => new(padrao, RegexOptions.Compiled | RegexOptions.CultureInvariant | opcoes);
}
