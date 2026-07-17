using Shared.Application.Email.Models;
using Shared.Application.Email.Rendering;
using Xunit;

namespace Shared.Tests.Email;

public class EmailTemplateRendererTests
{
    private readonly EmailTemplateRenderer _renderer = new();

    [Fact]
    public void RenderizarDeveCarregarTemplatesIncorporadosEGerarEmailRequest()
    {
        var request = _renderer.Renderizar(
            CriarDefinicao(),
            CriarModelo(),
            ["pessoa@exemplo.com"]);

        Assert.Equal("Validação da infraestrutura", request.Assunto);
        Assert.Equal(["pessoa@exemplo.com"], request.EmailsDestino);
        Assert.Contains("<!DOCTYPE html>", request.Mensagem);
        Assert.Contains("Atron", request.Mensagem);
        Assert.Contains("Infraestrutura de templates", request.Mensagem);
        Assert.Contains("https://atron.exemplo.com/continuar", request.Mensagem);
    }

    [Fact]
    public void RenderizarDeveRejeitarCampoObrigatorioNaoInformado()
    {
        var model = CriarModelo() with { Nome = "" };

        var exception = Assert.Throws<EmailTemplateException>(() =>
            _renderer.Renderizar(CriarDefinicao(), model, ["pessoa@exemplo.com"]));

        Assert.Contains("Nome", exception.Message);
    }

    [Fact]
    public void RenderizarDeveCodificarNomeEConteudoDinamicos()
    {
        var model = CriarModelo() with
        {
            Nome = "<João & Maria>",
            Conteudo = "<script>alert('x')</script>"
        };

        var request = _renderer.Renderizar(CriarDefinicao(), model, ["pessoa@exemplo.com"]);

        Assert.Contains("&lt;Jo&#227;o &amp; Maria&gt;", request.Mensagem);
        Assert.Contains("&lt;script&gt;alert(&#39;x&#39;)&lt;/script&gt;", request.Mensagem);
        Assert.DoesNotContain("<script>", request.Mensagem);
    }

    [Fact]
    public void RenderizarDeveRejeitarUrlForaDeHttpOuHttps()
    {
        var model = CriarModelo() with { Link = "javascript:alert('x')" };

        var exception = Assert.Throws<EmailTemplateException>(() =>
            _renderer.Renderizar(CriarDefinicao(), model, ["pessoa@exemplo.com"]));

        Assert.Contains("HTTP ou HTTPS", exception.Message);
    }

    [Fact]
    public void RenderizarDeveInformarTemplateInexistente()
    {
        var definition = CriarDefinicao() with
        {
            TemplateResourceName = "Shared.Application.Email.Templates.pt-BR.inexistente.html"
        };

        var exception = Assert.Throws<EmailTemplateException>(() =>
            _renderer.Renderizar(definition, CriarModelo(), ["pessoa@exemplo.com"]));

        Assert.Contains("não encontrado", exception.Message);
    }

    [Fact]
    public void RenderizarDeveCarregarTemplateDeDiagnosticoECodificarMensagem()
    {
        var request = _renderer.Renderizar(
            new EmailTemplateDefinition(
                typeof(EmailTemplateRenderer).Assembly,
                EmailTemplateResourceNames.Diagnostico,
                "Diagnóstico",
                "Diagnóstico de e-mail"),
            new EmailDiagnosticoModel
            {
                Mensagem = "<configuração>",
                Provedor = "Brevo",
                Host = "https://api.brevo.com/v3",
                Remetente = "sistema@atron.com",
                DataHora = "17/07/2026 10:00:00"
            },
            ["pessoa@exemplo.com"]);

        Assert.Contains("Diagnóstico de e-mail", request.Mensagem);
        Assert.Contains("&lt;configura&#231;&#227;o&gt;", request.Mensagem);
        Assert.DoesNotContain("<configuração>", request.Mensagem);
    }

    private static EmailTemplateDefinition CriarDefinicao()
    {
        return new EmailTemplateDefinition(
            typeof(EmailTemplateRenderer).Assembly,
            EmailTemplateResourceNames.Fundacao,
            "Validação da infraestrutura",
            "Infraestrutura de templates");
    }

    private static EmailTemplateFoundationModel CriarModelo()
    {
        return new EmailTemplateFoundationModel
        {
            Nome = "Pessoa",
            Conteudo = "A infraestrutura foi carregada.",
            Link = "https://atron.exemplo.com/continuar",
            TextoLink = "Continuar"
        };
    }
}
