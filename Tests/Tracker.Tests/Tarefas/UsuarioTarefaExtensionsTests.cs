using Application.Extensions;
using Domain.Entities;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class UsuarioTarefaExtensionsTests
{
    [Fact]
    public void ObterEscoposParaTarefas_DeveRemoverDuplicidades()
    {
        var usuario = new Usuario
        {
            UsuarioCargoDepartamentos =
            [
                new UsuarioCargoDepartamento { DepartamentoId = 10, CargoId = 20 },
                new UsuarioCargoDepartamento { DepartamentoId = 10, CargoId = 30 }
            ]
        };

        var departamentos = usuario.ObterDepartamentoIdsParaTarefas();
        var cargos = usuario.ObterCargoIdsParaTarefas();

        Assert.Equal([10], departamentos);
        Assert.Equal([20, 30], cargos);
    }

    [Fact]
    public void ObterEscoposParaTarefas_DeveRetornarColecaoVaziaSemRelacionamentos()
    {
        var usuario = new Usuario();

        Assert.Empty(usuario.ObterDepartamentoIdsParaTarefas());
        Assert.Empty(usuario.ObterCargoIdsParaTarefas());
    }
}
