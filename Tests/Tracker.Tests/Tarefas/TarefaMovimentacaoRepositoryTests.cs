using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaMovimentacaoRepositoryTests
{
    [Fact]
    public async Task ObterMovimentacoesPorIdAsync_DeveOrdenarPorDataEIdDecrescentes()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase($"historico-tarefa-{Guid.NewGuid()}")
            .Options;
        await using var context = new AtronDbContext(options);
        var dataMaisRecente = new DateTime(2026, 8, 19, 12, 0, 0);
        context.TarefaMovimentacoes.AddRange(
            CriarMovimentacao(10, 42, dataMaisRecente),
            CriarMovimentacao(30, 42, dataMaisRecente),
            CriarMovimentacao(20, 42, dataMaisRecente.AddMinutes(-1)),
            CriarMovimentacao(40, 99, dataMaisRecente.AddMinutes(1)));
        await context.SaveChangesAsync();
        var repository = new TarefaMovimentacaoRepository(context);

        var resultado = await repository.ObterMovimentacoesPorIdAsync(42);

        Assert.Equal([30, 10, 20], resultado.Select(item => item.Id));
        Assert.All(resultado, item => Assert.Equal(42, item.TarefaId));
    }

    private static TarefaMovimentacao CriarMovimentacao(
        int id,
        int tarefaId,
        DateTime dataOcorrencia)
    {
        return new TarefaMovimentacao
        {
            Id = id,
            TarefaId = tarefaId,
            Tipo = TipoMovimentacaoTarefa.Atualizacao,
            Descricao = "Detalhes",
            ResponsavelCodigo = "USR001",
            ResponsavelNome = "Usuario Responsavel",
            DataOcorrencia = dataOcorrencia
        };
    }
}
