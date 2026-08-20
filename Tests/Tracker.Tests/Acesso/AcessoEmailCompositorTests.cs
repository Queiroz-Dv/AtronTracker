using Application.Email.Compositores;
using Application.Records.Usuario;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Xunit;

namespace Tracker.Tests.Acesso;

public class AcessoEmailCompositorTests
{
    private readonly AcessoEmailCompositor _compositor = new(new EmailTemplateRenderer());

    [Fact]
    public void DeveRenderizarTodosOsTemplatesDeAcesso()
    {
        var resultados = new[]
        {
            _compositor.ComporConfirmacaoCadastro(new("destino@teste.com", "Ana", "123456", "https://atron.test/confirmar", 24)),
            _compositor.ComporRecuperacaoSenha(new("destino@teste.com", "Ana", "https://atron.test/senha", 24)),
            _compositor.ComporConfirmacaoConcluida("destino@teste.com", "Ana"),
            _compositor.ComporPrimeiroAcesso(new("destino@teste.com", "Ana", "https://atron.test/primeiro-acesso", 24)),
            _compositor.ComporAlteracaoEmail("destino@teste.com", "Ana", "https://atron.test/alterar-email"),
            _compositor.ComporReativacaoConta("destino@teste.com", "Ana", "ABC123")
        };

        Assert.Equal(6, resultados.Length);
        Assert.All(resultados, resultado =>
        {
            Assert.True(resultado.TeveSucesso);
            var email = resultado.Dados;
            Assert.Equal(["destino@teste.com"], email.EmailsDestino);
            Assert.False(string.IsNullOrWhiteSpace(email.Assunto));
            Assert.Contains("<!DOCTYPE html>", email.Mensagem);
            Assert.DoesNotContain("{{", email.Mensagem);
        });
    }

    [Fact]
    public void DeveCodificarDadosDinamicosDoTemplate()
    {
        var resultado = _compositor.ComporConfirmacaoCadastro(new(
            "destino@teste.com",
            "Ana <script>",
            "123&456",
            "https://atron.test/confirmar?codigo=123%20456",
            24));

        Assert.True(resultado.TeveSucesso);
        Assert.Contains("Ana &lt;script&gt;", resultado.Dados.Mensagem);
        Assert.Contains("123&amp;456", resultado.Dados.Mensagem);
        Assert.DoesNotContain("Ana <script>", resultado.Dados.Mensagem);
    }
}
