using Application.DTO;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Records.PlanejamentoCusto;
using Application.Resources;
using Application.UseCases.PlanejamentoCustoCases;
using Domain.Entities;
using Domain.Interfaces;
using Moq;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.PlanejamentoCustos;

public class PlanejamentoCustoCasesTests
{
    [Fact]
    public async Task CriarAsync_DevePersistirERetornarSomenteResultadoComMensagens()
    {
        var cenario = CriarCenario();
        var preparacao = CriarPreparacao();
        preparacao.ResultadoDetalhes.AdicionarAviso("A soma dos mínimos diverge.");
        cenario.Preparacao
            .Setup(servico => servico.PrepararCriacaoAsync(It.IsAny<PlanejamentoCustoDTO>()))
            .ReturnsAsync(Resultado<PlanejamentoCustoPreparadoRecord>.Sucesso(preparacao));
        cenario.Repositorio
            .Setup(repositorio => repositorio.CriarAsync(preparacao.Entidade))
            .ReturnsAsync(true);

        Resultado resultado = await cenario.Criar.ExecutarAsync(preparacao.Dto);

        Assert.True(resultado.TeveSucesso);
        Assert.Null(resultado.Dados);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao.Contains(preparacao.Entidade.Codigo));
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao == "A soma dos mínimos diverge.");
        cenario.Repositorio.Verify(repositorio => repositorio.CriarAsync(preparacao.Entidade), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_DeveInterromperQuandoPreparacaoFalhar()
    {
        var cenario = CriarCenario();
        cenario.Preparacao
            .Setup(servico => servico.PrepararCriacaoAsync(It.IsAny<PlanejamentoCustoDTO>()))
            .ReturnsAsync(Resultado<PlanejamentoCustoPreparadoRecord>.Falha("Planejamento inválido."));

        var resultado = await cenario.Criar.ExecutarAsync(new PlanejamentoCustoDTO());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao == "Planejamento inválido.");
        cenario.Repositorio.Verify(repositorio => repositorio.CriarAsync(It.IsAny<PlanejamentoCusto>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveRetornarFalhaQuandoRepositorioNaoAtualizar()
    {
        var cenario = CriarCenario();
        var preparacao = CriarPreparacao();
        cenario.Preparacao
            .Setup(servico => servico.PrepararAtualizacaoAsync("PLC-01", preparacao.Dto))
            .ReturnsAsync(Resultado<PlanejamentoCustoPreparadoRecord>.Sucesso(preparacao));
        cenario.Repositorio
            .Setup(repositorio => repositorio.AtualizarAsync(preparacao.Entidade))
            .ReturnsAsync(false);

        var resultado = await cenario.Atualizar.ExecutarAsync("PLC-01", preparacao.Dto);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == string.Format(PlanejamentoCustoResource.Erro_AtualizarPlanejamento, "PLC-01"));
    }

    [Fact]
    public async Task ExcluirAsync_DeveRemoverEntidadePreparada()
    {
        var cenario = CriarCenario();
        var planejamento = CriarPreparacao().Entidade;
        cenario.Preparacao
            .Setup(servico => servico.PrepararRemocaoAsync("PLC-01"))
            .ReturnsAsync(Resultado<PlanejamentoCusto>.Sucesso(planejamento));
        cenario.Repositorio
            .Setup(repositorio => repositorio.RemoverAsync(planejamento))
            .ReturnsAsync(true);

        var resultado = await cenario.Excluir.ExecutarAsync("PLC-01");

        Assert.True(resultado.TeveSucesso);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao == NotificacoesPadronizadas.MensagemRemocaoSucesso);
        cenario.Repositorio.Verify(repositorio => repositorio.RemoverAsync(planejamento), Times.Once);
    }

    [Fact]
    public async Task ObterPorCodigoAsync_DeveMapearPlanejamentoEncontrado()
    {
        var cenario = CriarCenario();
        var planejamento = CriarPreparacao().Entidade;
        cenario.Repositorio
            .Setup(repositorio => repositorio.ObterPorCodigoAsNoTrackingAsync("PLC-01"))
            .ReturnsAsync(planejamento);

        var resultado = await cenario.Obter.ObterPorCodigoAsync("PLC-01");

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("PLC-01", resultado.Dados!.Codigo);
        Assert.Equal(2027, resultado.Dados.Ano);
    }

    [Fact]
    public async Task ObterPorAnoAsync_DeveRetornarSomenteResultadoDaConsultaMapeado()
    {
        var cenario = CriarCenario();
        var planejamento = CriarPreparacao().Entidade;
        cenario.Repositorio
            .Setup(repositorio => repositorio.ObterPorAnoAsync(2027))
            .ReturnsAsync([planejamento]);

        var resultado = await cenario.Obter.ObterPorAnoAsync(2027);

        Assert.True(resultado.TeveSucesso);
        Assert.Single(resultado.Dados!);
        Assert.Equal("PLC-01", resultado.Dados![0].Codigo);
    }

    private static Cenario CriarCenario()
    {
        var preparacao = new Mock<IPlanejamentoCustoPreparacaoService>();
        var repositorio = new Mock<IPlanejamentoCustoRepository>();

        return new Cenario(
            new CriarPlanejamentoCustoCase(preparacao.Object, repositorio.Object),
            new AtualizarPlanejamentoCustoCase(preparacao.Object, repositorio.Object),
            new ExcluirPlanejamentoCustoCase(preparacao.Object, repositorio.Object),
            new ObterPlanejamentoCustoCase(new PlanejamentoCustoMapping(), repositorio.Object),
            preparacao,
            repositorio);
    }

    private static PlanejamentoCustoPreparadoRecord CriarPreparacao()
    {
        var dto = new PlanejamentoCustoDTO
        {
            Codigo = "PLC-01",
            Descricao = "Planejamento administrativo",
            Ano = 2027,
            ValorMinimo = 1000,
            ValorTeto = 2000,
            DepartamentoCodigo = "ADM"
        };
        var entidade = new PlanejamentoCustoMapping().MapToEntity(dto);

        return new PlanejamentoCustoPreparadoRecord(dto, entidade, Resultado.Sucesso());
    }

    private sealed record Cenario(
        CriarPlanejamentoCustoCase Criar,
        AtualizarPlanejamentoCustoCase Atualizar,
        ExcluirPlanejamentoCustoCase Excluir,
        ObterPlanejamentoCustoCase Obter,
        Mock<IPlanejamentoCustoPreparacaoService> Preparacao,
        Mock<IPlanejamentoCustoRepository> Repositorio);
}
