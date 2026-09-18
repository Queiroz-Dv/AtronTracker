using Application.DTO;
using Application.EmailCompositor.Compositores;
using Shared.Application.Email.Rendering;
using Xunit;

namespace Tracker.Tests.Application.Acesso
{
    public class AcessoEmailCompositorTests
    {
        private readonly AcessoEmailCompositor _compositor = new(new EmailTemplateRenderer());
        private static ParametrosEmailDTO ObterDadosConfirmacaoCadastro()
        {
            var parametrosConfirmacao = new ParametrosEmailDTO("https://atron.test/confirmar", "123456", 24);
            parametrosConfirmacao.VincularDadosDeEnvio("destino@teste.com", "Ana Maria", "ANAM");
            return parametrosConfirmacao;
        }

        public static ParametrosEmailDTO ObterDadosRecuperacaoSenha()
        {
            var parametrosConfirmacao = new ParametrosEmailDTO("https://atron.test/senha", "123456", 24);
            parametrosConfirmacao.VincularDadosDeEnvio("destino@teste.com", "Ana Maria", "ANAM");
            return parametrosConfirmacao;
        }

        public static ParametrosEmailDTO ObterDadosConfirmacaoConcluida()
        {
            var parametrosConfirmacao = new ParametrosEmailDTO(null, null, 0);
            parametrosConfirmacao.VincularDadosDeEnvio("destino@teste.com", "Ana Maria", "ANAM");
            return parametrosConfirmacao;
        }

        public static ParametrosEmailDTO ObterDadosPrimeiroAcesso()
        {
            var parametrosConfirmacao = new ParametrosEmailDTO("https://atron.test/primeiro-acesso", null, 24);
            parametrosConfirmacao.VincularDadosDeEnvio("destino@teste.com", "Ana Maria", "ANAM");
            return parametrosConfirmacao;
        }

        public static ParametrosEmailDTO ObterDadosAlteracaoEmail()
        {
            var parametrosConfirmacao = new ParametrosEmailDTO("https://atron.test/alterar-email", null, 0);
            parametrosConfirmacao.VincularDadosDeEnvio("destino@teste.com", "Ana Maria", "ANAM");
            return parametrosConfirmacao;
        }
        public static ParametrosEmailDTO ObterDadosReativacaoConta()
        {
            var parametrosConfirmacao = new ParametrosEmailDTO(null, "ABC123", 0);
            parametrosConfirmacao.VincularDadosDeEnvio("destino@teste.com", "Ana Maria", "ANAM");
            return parametrosConfirmacao;
        }



        [Fact]
        public void DeveRenderizarTodosOsTemplatesDeAcesso()
        {           
            var resultados = new[]
            {
                _compositor.ComporConfirmacaoCadastro(ObterDadosConfirmacaoCadastro()),
                _compositor.ComporRecuperacaoSenha(ObterDadosRecuperacaoSenha()),
                _compositor.ComporConfirmacaoConcluida(ObterDadosConfirmacaoConcluida()),
                _compositor.ComporPrimeiroAcesso(ObterDadosPrimeiroAcesso()),
                _compositor.ComporAlteracaoEmail(ObterDadosAlteracaoEmail()),
                _compositor.ComporReativacaoConta(ObterDadosReativacaoConta())
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
            var parametrosConfirmacao = new ParametrosEmailDTO()
            {
                Link = "https://atron.test/confirmar?codigo=123%20456",
                Destinatario = "destino@teste.com",
                UsuarioNome = "Ana <script>",
                Codigo = "ANAM",
                Identificador = "123&456",
                Validade = 24
            };


            var resultado = _compositor.ComporConfirmacaoCadastro(parametrosConfirmacao);
            Assert.True(resultado.TeveSucesso);
            Assert.Contains("Ana &lt;script&gt;", resultado.Dados.Mensagem);
            Assert.Contains("123&amp;456", resultado.Dados.Mensagem);
            Assert.DoesNotContain("Ana <script>", resultado.Dados.Mensagem);
        }
    }
}
