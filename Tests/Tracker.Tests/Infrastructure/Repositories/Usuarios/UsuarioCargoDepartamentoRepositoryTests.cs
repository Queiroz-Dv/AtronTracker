using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tracker.Tests.Infrastructure.Repositories.Usuarios;

public sealed class UsuarioCargoDepartamentoRepositoryTests
{
    [Fact]
    public async Task RemoverAssociacaoUsuarioCargoDepartamento_DeveRemoverEPersistir()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        await using var context = new AtronDbContext(options);
        var usuario = new Usuario
        {
            Id = 1,
            Codigo = "USR",
            Nome = "Usuario",
            Sobrenome = "Teste",
            Email = "usuario@teste.com"
        };
        var departamento = new Departamento { Id = 2, Codigo = "DPT", Descricao = "Departamento" };
        var cargo = new Cargo
        {
            Id = 3,
            Codigo = "CRG",
            Descricao = "Cargo",
            DepartamentoId = departamento.Id,
            DepartamentoCodigo = departamento.Codigo,
            Departamento = departamento
        };
        var associacao = new UsuarioCargoDepartamento
        {
            UsuarioId = usuario.Id,
            UsuarioCodigo = usuario.Codigo,
            Usuario = usuario,
            DepartamentoId = departamento.Id,
            DepartamentoCodigo = departamento.Codigo,
            Departamento = departamento,
            CargoId = cargo.Id,
            CargoCodigo = cargo.Codigo,
            Cargo = cargo
        };
        context.AddRange(usuario, departamento, cargo, associacao);
        await context.SaveChangesAsync();
        var repository = new UsuarioCargoDepartamentoRepository(context);

        var removida = await repository.RemoverAssociacaoUsuarioCargoDepartamento(associacao);

        Assert.True(removida);
        Assert.False(await context.UsuarioCargoDepartamentos.AnyAsync());
    }
}
