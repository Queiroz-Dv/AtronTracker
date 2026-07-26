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

        Assert.True(request.TeveSucesso);
        Assert.Equal("Validação da infraestrutura", request.Dados.Assunto);
        Assert.Equal(["pessoa@exemplo.com"], request.Dados.EmailsDestino);
        Assert.Contains("<!DOCTYPE html>", request.Dados.Mensagem);
        Assert.Contains("Atron", request.Dados.Mensagem);
        Assert.Contains("Infraestrutura de templates", request.Dados.Mensagem);
        Assert.Contains("https://atron.exemplo.com/continuar", request.Dados.Mensagem);
    }

    [Fact]
    public void RenderizarDeveRejeitarCampoObrigatorioNaoInformado()
    {
        var model = CriarModelo() with { Nome = "" };

        var resultado = _renderer.Renderizar(CriarDefinicao(), model, ["pessoa@exemplo.com"]);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao.Contains("Nome"));
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

        Assert.True(request.TeveSucesso);
        Assert.Contains("&lt;Jo&#227;o &amp; Maria&gt;", request.Dados.Mensagem);
        Assert.Contains("&lt;script&gt;alert(&#39;x&#39;)&lt;/script&gt;", request.Dados.Mensagem);
        Assert.DoesNotContain("<script>", request.Dados.Mensagem);
    }

    [Fact]
    public void RenderizarDeveRejeitarUrlForaDeHttpOuHttps()
    {
        var model = CriarModelo() with { Link = "javascript:alert('x')" };

        var resultado = _renderer.Renderizar(CriarDefinicao(), model, ["pessoa@exemplo.com"]);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao.Contains("HTTP ou HTTPS"));
    }

    [Fact]
    public void RenderizarDeveInformarTemplateInexistente()
    {
        var definition = CriarDefinicao() with
        {
            TemplateResourceName = "Shared.Application.Email.Templates.pt-BR.inexistente.html"
        };

        var resultado = _renderer.Renderizar(definition, CriarModelo(), ["pessoa@exemplo.com"]);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao.Contains("não encontrado"));
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

        Assert.True(request.TeveSucesso);
        Assert.Contains("Diagnóstico de e-mail", request.Dados.Mensagem);
        Assert.Contains("&lt;configura&#231;&#227;o&gt;", request.Dados.Mensagem);
        Assert.DoesNotContain("<configuração>", request.Dados.Mensagem);
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
