using AtronTracker.Infrastructure.Context;
using Domain.Entities;
using Domain.Extensions;
using Domain.ValueObjects;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace Tracker.Tests.Empresas;

public sealed class EmpresaRepositoryTests
{
    [Fact]
    public async Task Catalogo_DeveIncluirModuloEmpresaSemCriarVinculos()
    {
        using var context = new AtronDbContext(new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        await context.Database.EnsureCreatedAsync();

        var modulo = await new ModuloRepository(context).ObterPorCodigoRepository("EMP");

        Assert.Equal(14, modulo.Id);
        Assert.Equal("Empresa", modulo.Descricao);
        Assert.Equal("Modulo:EMP", Shared.Authorization.ModuloPolicies.Empresa);
        Assert.Empty(await context.Empresas.ToListAsync());
        Assert.Empty(await context.UsuariosEmpresas.ToListAsync());
        Assert.Empty(await context.PerfilDeAcessoModulos.ToListAsync());
    }

    [Fact]
    public async Task Criar_DevePersistirEmpresaEVinculoSemDuplicarUsuarioDesanexado()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var context = new AtronDbContext(options);
        var usuario = new Usuario("ANA", "Ana", "Teste", "ana@example.test", null) { EmailConfirmado = true };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();
        context.ChangeTracker.Clear();
        var empresa = new Empresa
        {
            Codigo = "Estudo", NomeFantasia = "Empresa", Endereco = new Endereco { Logradouro = "Rua" },
            Numero = "(11) 99999-0000", Email = "empresa@example.test"
        };
        var vinculo = empresa.ConcluirCadastro(usuario);

        await new EmpresaRepository(context).CriarAsync(empresa);

        Assert.Equal(1, await context.Empresas.CountAsync());
        Assert.Equal(1, await context.UsuariosEmpresas.CountAsync());
        Assert.Equal(1, await context.Usuarios.CountAsync());
        Assert.Equal(usuario.Id, vinculo.UsuarioId);
        Assert.Equal(empresa.Id, vinculo.EmpresaId);
        Assert.Equal(EntityState.Unchanged, context.Entry(usuario).State);
    }

    [Fact]
    public async Task Criar_DevePropagarErroDoBanco()
    {
        var options = new DbContextOptionsBuilder<AtronDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var context = new AtronDbContext(options);
        var empresa = new Empresa { Codigo = null! };

        await Assert.ThrowsAsync<DbUpdateException>(() => new EmpresaRepository(context).CriarAsync(empresa));
    }

    [Fact]
    public void ModeloAtual_DeveContinuarCompativelComSnapshotAplicado()
    {
        using var context = new AtronDbContext(new DbContextOptionsBuilder<AtronDbContext>()
            .UseNpgsql("Host=localhost;Port=1;Database=schema_only;Username=unused;Password=unused").Options);
        var snapshot = context.GetService<IMigrationsAssembly>().ModelSnapshot!.Model;
        var inicializado = context.GetService<IModelRuntimeInitializer>().Initialize(snapshot, designTime: true);
        var atual = context.GetService<IDesignTimeModel>().Model;

        Assert.False(context.GetService<IMigrationsModelDiffer>().HasDifferences(
            inicializado.GetRelationalModel(), atual.GetRelationalModel()));
    }

}
