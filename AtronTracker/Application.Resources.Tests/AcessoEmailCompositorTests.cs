using Application.Email.Compositores;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Xunit;

namespace Application.Resources.Tests;

public class AcessoEmailCompositorTests
{
    private readonly AcessoEmailCompositor _compositor = new(new EmailTemplateRenderer());

    [Fact]
    public void DeveRenderizarTodosOsTemplatesDeAcesso()
    {
        var emails = new EmailRequest[]
        {
            _compositor.ComporConfirmacaoCadastro("destino@teste.com", "Ana", "123456", "https://atron.test/confirmar", 24),
            _compositor.ComporRecuperacaoSenha("destino@teste.com", "Ana", "https://atron.test/senha", 24),
            _compositor.ComporConfirmacaoConcluida("destino@teste.com", "Ana"),
            _compositor.ComporPrimeiroAcesso("destino@teste.com", "Ana", "https://atron.test/primeiro-acesso", 24),
            _compositor.ComporAlteracaoEmail("destino@teste.com", "Ana", "https://atron.test/alterar-email"),
            _compositor.ComporReativacaoConta("destino@teste.com", "Ana", "ABC123")
        };

        Assert.Equal(6, emails.Length);
        Assert.All(emails, email =>
        {
            Assert.Equal(["destino@teste.com"], email.EmailsDestino);
            Assert.False(string.IsNullOrWhiteSpace(email.Assunto));
            Assert.Contains("<!DOCTYPE html>", email.Mensagem);
            Assert.DoesNotContain("{{", email.Mensagem);
        });
    }

    [Fact]
    public void DeveCodificarDadosDinamicosDoTemplate()
    {
        var email = _compositor.ComporConfirmacaoCadastro(
            "destino@teste.com",
            "Ana <script>",
            "123&456",
            "https://atron.test/confirmar?codigo=123%20456",
            24);

        Assert.Contains("Ana &lt;script&gt;", email.Mensagem);
        Assert.Contains("123&amp;456", email.Mensagem);
        Assert.DoesNotContain("Ana <script>", email.Mensagem);
    }
}
