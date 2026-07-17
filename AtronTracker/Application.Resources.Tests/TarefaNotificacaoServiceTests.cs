using Application.DTO;
using Application.Email.Compositores;
using Application.Services.EntitiesServices.Tarefas;
using Domain.Entities;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Application.Resources.Tests;

public class TarefaNotificacaoServiceTests
{
    [Fact]
    public void CompositorDeveRenderizarTemplateTipadoEEncodarConteudoDinamico()
    {
        var compositor = new TarefaEmailCompositor(new EmailTemplateRenderer());

        var email = compositor.ComporAtribuicao(CriarTarefa("Revisar <contrato>"), CriarUsuario(true));

        Assert.Equal("Nova tarefa atribuída: Revisar <contrato>", email.Assunto);
        Assert.Equal(["usuario@teste.com"], email.EmailsDestino);
        Assert.Contains("<!DOCTYPE html>", email.Mensagem);
        Assert.Contains("Revisar &lt;contrato&gt;", email.Mensagem);
        Assert.Contains("Conteudo &amp; detalhes", email.Mensagem);
        Assert.Contains("10/07/2026", email.Mensagem);
        Assert.Contains("Aberta", email.Mensagem);
    }

    [Fact]
    public void CompositorDeveAceitarConteudoOpcionalVazio()
    {
        var compositor = new TarefaEmailCompositor(new EmailTemplateRenderer());
        var tarefa = CriarTarefa();
        tarefa.Conteudo = null;

        var email = compositor.ComporAtribuicao(tarefa, CriarUsuario(true));

        Assert.Contains("<strong>Conteudo:</strong> </td>", email.Mensagem);
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

    private static Usuario CriarUsuario(bool receberEmail)
    {
        return new Usuario
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

        public EmailRequest ComporAtribuicao(TarefaDTO tarefa, Usuario usuario)
        {
            QuantidadeChamadas++;
            return new EmailRequest
            {
                EmailsDestino = [usuario.Email],
                Assunto = "Assunto",
                Mensagem = "Mensagem"
            };
        }
    }
}
