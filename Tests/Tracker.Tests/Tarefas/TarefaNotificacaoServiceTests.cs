using Application.DTO;
using Application.Email.Compositores;
using Application.Services.EntitiesServices.Tarefas;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaNotificacaoServiceTests
{
    [Fact]
    public void CompositorDeveRenderizarTemplateTipadoEEncodarConteudoDinamico()
    {
        var compositor = new TarefaEmailCompositor(new EmailTemplateRenderer());

        var email = compositor.ComporAtribuicao(CriarTarefa("Revisar <contrato>"), CriarUsuario(true));

        Assert.True(email.TeveSucesso);
        Assert.Equal("Nova tarefa atribuída: Revisar <contrato>", email.Dados.Assunto);
        Assert.Equal(["usuario@teste.com"], email.Dados.EmailsDestino);
        Assert.Contains("<!DOCTYPE html>", email.Dados.Mensagem);
        Assert.Contains("Revisar &lt;contrato&gt;", email.Dados.Mensagem);
        Assert.Contains("Conteudo &amp; detalhes", email.Dados.Mensagem);
        Assert.Contains("10/07/2026", email.Dados.Mensagem);
        Assert.Contains("Aberta", email.Dados.Mensagem);
    }

    [Fact]
    public void CompositorDeveAceitarConteudoOpcionalVazio()
    {
        var compositor = new TarefaEmailCompositor(new EmailTemplateRenderer());
        var tarefa = CriarTarefa();
        tarefa.Conteudo = null;

        var email = compositor.ComporAtribuicao(tarefa, CriarUsuario(true));

        Assert.True(email.TeveSucesso);
        Assert.Contains("<strong>Conteudo:</strong> </td>", email.Dados.Mensagem);
    }

    [Fact]
    public async Task NotificarAtribuicaoAsync_DeveIgnorarEnvioQuandoPreferenciaEstaDesativada()
    {
        var emailService = new EmailServiceFake(Resultado.Sucesso());
        var compositor = new CompositorFake();
        var service = new TarefaNotificacaoService(emailService, compositor);

        var resultado = await service.NotificarAtribuicaoAsync(CriarTarefa(), CriarUsuario(false));

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(0, compositor.QuantidadeChamadas);
        Assert.Equal(0, emailService.QuantidadeEnvios);
    }

    [Fact]
    public async Task NotificarAtribuicaoAsync_DeveEnviarQuandoPreferenciaEstaAtivada()
    {
        var emailService = new EmailServiceFake(Resultado.Sucesso());
        var compositor = new CompositorFake();
        var service = new TarefaNotificacaoService(emailService, compositor);

        var resultado = await service.NotificarAtribuicaoAsync(CriarTarefa(), CriarUsuario(true));

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(1, compositor.QuantidadeChamadas);
        Assert.Equal(1, emailService.QuantidadeEnvios);
    }

    [Fact]
    public async Task NotificarAtribuicaoAsync_DevePropagarFalhaDoProvedorParaTratamentoAdvisoryDoFluxo()
    {
        var emailService = new EmailServiceFake(Resultado.Falha("Falha simulada"));
        var service = new TarefaNotificacaoService(emailService, new CompositorFake());

        var resultado = await service.NotificarAtribuicaoAsync(CriarTarefa(), CriarUsuario(true));

        Assert.True(resultado.TeveFalha);
        Assert.Equal(1, emailService.QuantidadeEnvios);
    }

    private static TarefaDTO CriarTarefa(string titulo = "Revisar contrato")
    {
        return new TarefaDTO
        {
            Titulo = titulo,
            Conteudo = "Conteudo & detalhes",
            DataInicial = new DateTime(2026, 7, 10),
            DataFinal = new DateTime(2026, 7, 12),
            EstadoDaTarefa = new TarefaEstadoDTO { Id = 1, Descricao = "Aberta" }
        };
    }

    private static UsuarioDTO CriarUsuario(bool receberEmail)
    {
        return new UsuarioDTO
        {
            Nome = "Usuario",
            Sobrenome = "Teste",
            Email = "usuario@teste.com",
            ReceberNotificacaoTarefaPorEmail = receberEmail
        };
    }

    private sealed class EmailServiceFake(Resultado resultado) : IEmailService
    {
        public int QuantidadeEnvios { get; private set; }

        public Task<Resultado> EnviarAsync(EmailRequest message)
        {
            QuantidadeEnvios++;
            return Task.FromResult(resultado);
        }
    }

    private sealed class CompositorFake : ITarefaEmailCompositor
    {
        public int QuantidadeChamadas { get; private set; }

        public Resultado<EmailRequest> ComporAtribuicao(TarefaDTO tarefa, UsuarioDTO usuario)
        {
            QuantidadeChamadas++;
            return Resultado<EmailRequest>.Sucesso(new EmailRequest
            {
                EmailsDestino = [usuario.Email],
                Assunto = "Assunto",
                Mensagem = "Mensagem"
            });
        }
    }
}
