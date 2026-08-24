using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tracker.Tests.Infrastructure.Repositories.Tarefas;

public sealed class TarefaRepositoryTests
{
    [Fact]
    public async Task RemoverTarefaAsync_DeveRemoverEPersistir()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AtronDbContext(options);
        var estado = new TarefaEstado { Id = 1, Descricao = "Aberta" };
        var tarefa = new Tarefa
        {
            Id = 10,
            Titulo = "Tarefa para remocao",
            Conteudo = "Conteudo",
            DataInicial = new DateTime(2026, 8, 23),
            DataFinal = new DateTime(2026, 8, 24),
            TarefaEstadoId = estado.Id,
            EstadoDaTarefa = estado
        };
        context.AddRange(estado, tarefa);
        await context.SaveChangesAsync();
        var repository = new TarefaRepository(context);

        var removida = await repository.RemoverTarefaAsync(tarefa);

        Assert.True(removida);
        Assert.False(await context.Tarefas.AnyAsync(item => item.Id == tarefa.Id));
    }
}
