using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;
using Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Tracker.Tests.Infrastructure.Repositories.Empresas;

public sealed class EmpresaPersistenciaTests
{
    [Fact]
    public async Task SubstituirEndereco_DevePersistirNovoValorSemAlterarOutraEmpresaOuTelefone()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var context = new AtronDbContext(options);
        var primeira = new Empresa
        {
            Codigo = "EMP-A", NomeFantasia = "Empresa A",
            Endereco = new Endereco { Logradouro = "Rua inicial" },
            Numero = "(11) 99999-0000", Email = "a@example.test"
        };
        var segunda = new Empresa
        {
            Codigo = "EMP-B", NomeFantasia = "Empresa B",
            Endereco = new Endereco { Logradouro = "Rua inicial" },
            Numero = "(21) 99999-0000", Email = "b@example.test"
        };
        context.Empresas.AddRange(primeira, segunda);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var carregada = await context.Empresas.SingleAsync(item => item.Codigo == "EMP-A");
        carregada.Endereco = new Endereco { Logradouro = "Rua atualizada" };
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var empresas = await context.Empresas.OrderBy(item => item.Codigo).ToListAsync();
        Assert.Equal("Rua atualizada", empresas[0].Endereco.Logradouro);
        Assert.Equal("Rua inicial", empresas[1].Endereco.Logradouro);
        Assert.Equal("(11) 99999-0000", empresas[0].Numero);
        Assert.Equal("(21) 99999-0000", empresas[1].Numero);
    }

    [Fact]
    public async Task SalvarCadastro_DevePersistirVinculoComUsuarioExistenteSemDuplicarConta()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var context = new AtronDbContext(options);
        var usuario = new Usuario("ANA", "Ana", "Teste", "ana@example.test", null) { EmailConfirmado = true };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        var empresa = new Empresa { Codigo = "ESTUDO", NomeFantasia = "Empresa de estudos", Endereco = new Endereco { Logradouro = "Rua de Teste" }, Numero = "(11) 99999-0000", Email = "contato@example.test" };
        empresa.ConcluirCadastro(usuario);

        context.Empresas.Add(empresa);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();

        var gravada = await context.Empresas.Include(item => item.Usuarios).SingleAsync();
        var vinculo = Assert.Single(gravada.Usuarios);
        Assert.Equal(StatusEmpresa.Ativa, gravada.Status);
        Assert.Equal(empresa.Endereco, gravada.Endereco);
        Assert.Equal(empresa.Numero, gravada.Numero);
        Assert.Equal(gravada.Id, vinculo.EmpresaId);
        Assert.Equal(usuario.Id, vinculo.UsuarioId);
        Assert.Equal("ANA", vinculo.UsuarioCodigo);
        Assert.Equal(PapelUsuarioEmpresa.Responsavel, vinculo.Papel);
        Assert.Equal(1, await context.Usuarios.CountAsync());
    }

    [Fact]
    public async Task SalvarComEstadoDesatualizado_DeveDetectarConflitoDeConcorrencia()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using (var setup = new AtronDbContext(options))
        {
            setup.Empresas.Add(new Empresa { Codigo = "ESTUDO", NomeFantasia = "Empresa", Endereco = new Endereco { Logradouro = "Rua" }, Numero = "(11) 99999-0000", Email = "contato@example.test" });
            setup.Usuarios.AddRange(
                new Usuario("ANA", "Ana", "Teste", "ana@example.test", null) { EmailConfirmado = true },
                new Usuario("BRUNO", "Bruno", "Teste", "bruno@example.test", null) { EmailConfirmado = true });
            await setup.SaveChangesAsync();
        }
        await using var primeiro = new AtronDbContext(options);
        await using var segundo = new AtronDbContext(options);
        var empresaPrimeiro = await primeiro.Empresas.SingleAsync();
        var empresaSegundo = await segundo.Empresas.SingleAsync();
        empresaPrimeiro.ConcluirCadastro(await primeiro.Usuarios.SingleAsync(usuario => usuario.Codigo == "ANA"));
        empresaSegundo.ConcluirCadastro(await segundo.Usuarios.SingleAsync(usuario => usuario.Codigo == "BRUNO"));

        await primeiro.SaveChangesAsync();

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => segundo.SaveChangesAsync());
    }
}

