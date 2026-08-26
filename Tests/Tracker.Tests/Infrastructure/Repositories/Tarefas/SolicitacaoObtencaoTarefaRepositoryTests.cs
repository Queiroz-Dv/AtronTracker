using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class SolicitacaoObtencaoTarefaRepositoryTests
{
    [Fact]
    public async Task ObterPendentesPorAprovadorOuDepartamentosAsync_DeveAplicarUniaoEFiltrarStatus()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AtronDbContext(options);
        var estado = new TarefaEstado { Id = 1, Descricao = "Aberta" };
        var adm = new Departamento { Id = 10, Codigo = "ADM", Descricao = "Administracao" };
        var fin = new Departamento { Id = 20, Codigo = "FIN", Descricao = "Financeiro" };
        var solicitante = CriarUsuario(1, "ANA");
        var gestor = CriarUsuario(2, "GESTOR");
        var outroAprovador = CriarUsuario(3, "OUTRO");
        context.AddRange(estado, adm, fin, solicitante, gestor, outroAprovador);

        var tarefaDireta = CriarTarefa(101, estado, fin);
        var tarefaEquipe = CriarTarefa(102, estado, adm);
        var tarefaAprovada = CriarTarefa(103, estado, adm);
        var tarefaAlheia = CriarTarefa(104, estado, fin);
        context.AddRange(tarefaDireta, tarefaEquipe, tarefaAprovada, tarefaAlheia);
        context.AddRange(
            CriarSolicitacao(1, tarefaDireta, solicitante, gestor, StatusSolicitacaoObtencaoTarefa.Pendente, 1),
            CriarSolicitacao(2, tarefaEquipe, solicitante, outroAprovador, StatusSolicitacaoObtencaoTarefa.Pendente, 2),
            CriarSolicitacao(3, tarefaAprovada, solicitante, outroAprovador, StatusSolicitacaoObtencaoTarefa.Aprovada, 3),
            CriarSolicitacao(4, tarefaAlheia, solicitante, outroAprovador, StatusSolicitacaoObtencaoTarefa.Pendente, 4));
        await context.SaveChangesAsync();
        var repository = new SolicitacaoObtencaoTarefaRepository(context);

        var resultado = await repository.ObterPendentesPorAprovadorOuDepartamentosAsync(
            gestor.Id,
            gestor.Codigo,
            [adm.Codigo]);

        Assert.Equal([2, 1], resultado.Select(solicitacao => solicitacao.Id));
    }

    private static Usuario CriarUsuario(int id, string codigo)
        => new()
        {
            Id = id,
            Codigo = codigo,
            Nome = codigo,
            Sobrenome = "Teste",
            Email = $"{codigo.ToLowerInvariant()}@teste.com",
            UsuarioCargoDepartamentos = []
        };

    private static Tarefa CriarTarefa(int id, TarefaEstado estado, Departamento departamento)
        => new()
        {
            Id = id,
            DestinoInicial = (int)DestinoInicialTarefa.Equipe,
            Titulo = $"Tarefa {id}",
            Conteudo = "Conteudo",
            DataInicial = new DateTime(2026, 8, 20),
            DataFinal = new DateTime(2026, 8, 21),
            TarefaEstadoId = estado.Id,
            EstadoDaTarefa = estado,
            DepartamentoId = departamento.Id,
            DepartamentoCodigo = departamento.Codigo,
            Departamento = departamento
        };

    private static SolicitacaoObtencaoTarefa CriarSolicitacao(
        int id,
        Tarefa tarefa,
        Usuario solicitante,
        Usuario aprovador,
        StatusSolicitacaoObtencaoTarefa status,
        int ordem)
        => new()
        {
            Id = id,
            TarefaId = tarefa.Id,
            Tarefa = tarefa,
            SolicitanteId = solicitante.Id,
            SolicitanteCodigo = solicitante.Codigo,
            Solicitante = solicitante,
            AprovadorId = aprovador.Id,
            AprovadorCodigo = aprovador.Codigo,
            Aprovador = aprovador,
            Status = (int)status,
            DataSolicitacao = new DateTime(2026, 8, 20, 10, ordem, 0)
        };
}
