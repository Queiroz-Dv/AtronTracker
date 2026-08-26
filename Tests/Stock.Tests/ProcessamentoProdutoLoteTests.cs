using AtronStock.Domain.Entities;
using AtronStock.Domain.Enums;
using AtronStock.Domain.ValueObjects;
using Xunit;

namespace Stock.Tests;

public sealed class ProcessamentoProdutoLoteTests
{
    [Fact]
    public void DevePercorrerFluxoDeSucesso()
    {
        var processamento = CriarProcessamento();
        var token = Guid.NewGuid();

        processamento.Reservar(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(5), token);
        processamento.Concluir(42, 3, token);

        Assert.Equal(EStatusProcessamentoProdutoLote.Concluido, processamento.Status);
        Assert.Equal(42, processamento.LoteProdutoId);
        Assert.Equal(3, processamento.Resultado.QuantidadeProcessada);
        Assert.Null(processamento.Resultado.Erro);
    }

    [Fact]
    public void NaoDeveConcluirAntesDaExecucao()
    {
        var processamento = CriarProcessamento();

        Assert.Throws<InvalidOperationException>(() =>
            processamento.Concluir(42, 3, Guid.NewGuid()));
        Assert.Equal(EStatusProcessamentoProdutoLote.Pendente, processamento.Status);
    }

    [Fact]
    public void DeveRecuperarReservaAbandonadaComNovoToken()
    {
        var processamento = CriarProcessamento();
        var inicio = new DateTimeOffset(2026, 8, 24, 12, 0, 0, TimeSpan.Zero);
        processamento.Reservar(inicio, TimeSpan.FromMinutes(5), Guid.NewGuid());
        var novoToken = Guid.NewGuid();

        processamento.Reservar(inicio.AddMinutes(6), TimeSpan.FromMinutes(5), novoToken);

        Assert.Equal(2, processamento.Tentativas);
        Assert.Equal(novoToken, processamento.TokenReserva);
        Assert.Equal(inicio.AddMinutes(11), processamento.ReservaExpiraEm);
    }

    [Fact]
    public void NaoDeveTomarReservaAindaVigente()
    {
        var processamento = CriarProcessamento();
        var agora = DateTimeOffset.UtcNow;
        processamento.Reservar(agora, TimeSpan.FromMinutes(5), Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => processamento.Reservar(
            agora.AddMinutes(1),
            TimeSpan.FromMinutes(5),
            Guid.NewGuid()));
    }

    [Fact]
    public void NaoDeveConcluirComTokenDeOutroWorker()
    {
        var processamento = CriarProcessamento();
        processamento.Reservar(
            DateTimeOffset.UtcNow,
            TimeSpan.FromMinutes(5),
            Guid.NewGuid());

        Assert.Throws<InvalidOperationException>(() => processamento.Concluir(
            42,
            3,
            Guid.NewGuid()));
        Assert.Equal(EStatusProcessamentoProdutoLote.EmExecucao, processamento.Status);
    }

    [Fact]
    public void DeveLimitarErroAoTamanhoPersistido()
    {
        var processamento = CriarProcessamento();
        var token = Guid.NewGuid();
        processamento.Reservar(DateTimeOffset.UtcNow, TimeSpan.FromMinutes(5), token);

        processamento.Falhar(new string('x', 2100), token);

        Assert.Equal(EStatusProcessamentoProdutoLote.Falha, processamento.Status);
        Assert.Equal(2000, processamento.Resultado.Erro!.Length);
    }

    private static ProcessamentoProdutoLote CriarProcessamento()
        => new(new SolicitacaoGeracaoProdutosLote(
            "MTG",
            3,
            "USR001",
            "Monitor",
            null,
            new DateTime(2026, 8, 24),
            1500m,
            []));
}
