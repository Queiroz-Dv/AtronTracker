using Application.Interfaces.Services;
using Application.Services.EntitiesServices.Tarefas;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Queries;
using Moq;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaMovimentacaoServiceTests
{
    [Fact]
    public async Task RegistrarAtualizacaoAsync_DevePreservarAtorEAlteracoesRelevantes()
    {
        TarefaMovimentacao? registrada = null;
        var repository = new Mock<ITarefaMovimentacaoRepository>();
        repository
            .Setup(item => item.RegistrarAsync(It.IsAny<TarefaMovimentacao>()))
            .Callback<TarefaMovimentacao>(item => registrada = item)
            .ReturnsAsync(true);
        var service = CriarService(repository.Object);
        var responsavel = new Usuario
        {
            Codigo = "USR001",
            Nome = "Maria",
            Sobrenome = "Silva"
        };
        var anterior = new Tarefa
        {
            Id = 42,
            Titulo = "Título anterior",
            Conteudo = "Conteúdo anterior",
            DataInicial = new DateTime(2026, 7, 1),
            DataFinal = new DateTime(2026, 7, 10),
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" }
        };
        var atual = new Tarefa
        {
            Id = 42,
            Titulo = "Título novo",
            Conteudo = "Conteúdo anterior",
            DataInicial = anterior.DataInicial,
            DataFinal = anterior.DataFinal,
            TarefaEstadoId = 5,
            EstadoDaTarefa = new TarefaEstado { Id = 5, Descricao = "Iniciada" }
        };

        var resultado = await service.RegistrarAtualizacaoAsync(anterior, atual, responsavel);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(registrada);
        Assert.Equal(TipoMovimentacaoTarefa.Atualizacao, registrada.Tipo);
        Assert.Equal("USR001", registrada.ResponsavelCodigo);
        Assert.Equal("Maria Silva", registrada.ResponsavelNome);
        Assert.Contains("Título atualizado.", registrada.Descricao);
        Assert.Contains("Estado alterado de Em atividade para Iniciada.", registrada.Descricao);
    }

    [Fact]
    public async Task ObterAsync_DeveConsultarPaginaNoServidorEMapearMovimento()
    {
        TarefaMovimentacaoConsulta? consultaCapturada = null;
        var movimentacaoRepository = new Mock<ITarefaMovimentacaoRepository>();
        movimentacaoRepository
            .Setup(item => item.ObterPaginaAsync(It.IsAny<TarefaMovimentacaoConsulta>()))
            .Callback<TarefaMovimentacaoConsulta>(item => consultaCapturada = item)
            .ReturnsAsync(new TarefaMovimentacaoPagina(
                [
                    new TarefaMovimentacao
                    {
                        Id = 10,
                        Tipo = TipoMovimentacaoTarefa.Criacao,
                        Descricao = "Detalhes",
                        ResponsavelCodigo = "USR001",
                        ResponsavelNome = "Maria Silva",
                        DataOcorrencia = new DateTime(2026, 7, 26, 12, 0, 0)
                    }
                ],
                13));
        var tarefaRepository = new Mock<ITarefaRepository>();
        tarefaRepository
            .Setup(item => item.PodeAcessarHistoricoAsync(42, 7, "USR001"))
            .ReturnsAsync(true);
        var usuarioAtual = new Mock<ITarefaUsuarioAtualService>();
        usuarioAtual
            .Setup(item => item.ObterAsync())
            .ReturnsAsync(Resultado<Usuario>.Sucesso(new Usuario { Id = 7, Codigo = "USR001" }));
        var service = new TarefaMovimentacaoService(
            movimentacaoRepository.Object,
            tarefaRepository.Object,
            usuarioAtual.Object);

        var resultado = await service.ObterAsync(42, 2, 5);

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(new TarefaMovimentacaoConsulta(42, 2, 5), consultaCapturada);
        Assert.Equal(13, resultado.Dados.TotalItens);
        Assert.Equal("Criação", Assert.Single(resultado.Dados.Itens).Movimento);
    }

    private static TarefaMovimentacaoService CriarService(
        ITarefaMovimentacaoRepository movimentacaoRepository)
    {
        return new TarefaMovimentacaoService(
            movimentacaoRepository,
            Mock.Of<ITarefaRepository>(),
            Mock.Of<ITarefaUsuarioAtualService>());
    }
}
