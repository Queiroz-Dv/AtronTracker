using Application.DTO;
using Application.Interfaces.Services;
using Application.Services.EntitiesServices;
using Moq;
using Xunit;

namespace Tracker.Tests;

public class DadosComplementaresDoUsuarioServiceTests
{
    [Fact]
    public async Task ObterInformacoesComplementaresDoUsuario_DevePropagarModuloCategoria()
    {
        var perfis = new Mock<IPerfilDeAcessoService>();
        perfis
            .Setup(servico => servico.ObterPerfisPorCodigoUsuarioAsync("USR-CAT"))
            .ReturnsAsync(
            [
                new PerfilDeAcessoDTO
                {
                    Codigo = "ESTOQUE",
                    Descricao = "Acesso ao estoque",
                    Modulos = [new ModuloDTO { Codigo = "CAT", Descricao = "Categorias" }]
                }
            ]);
        var service = new DadosComplementaresDoUsuarioService(perfis.Object);

        var dados = await service.ObterInformacoesComplementaresDoUsuario(new UsuarioDTO
        {
            Codigo = "USR-CAT",
            Nome = "Usuário",
            Email = "usuario@atron.local"
        });

        var perfil = Assert.Single(dados.DadosDoPerfil);
        var modulo = Assert.Single(perfil.Modulos);
        Assert.Equal("CAT", modulo.Codigo);
        Assert.Equal("Categorias", modulo.Descricao);
    }

    [Fact]
    public async Task ObterInformacoesComplementaresDoUsuario_NaoDeveAdicionarCategoriaSemRelacionamento()
    {
        var perfis = new Mock<IPerfilDeAcessoService>();
        perfis
            .Setup(servico => servico.ObterPerfisPorCodigoUsuarioAsync("USR-SEM-CAT"))
            .ReturnsAsync(
            [
                new PerfilDeAcessoDTO
                {
                    Codigo = "TAREFAS",
                    Descricao = "Acesso às tarefas",
                    Modulos = [new ModuloDTO { Codigo = "TAR", Descricao = "Tarefas" }]
                }
            ]);
        var service = new DadosComplementaresDoUsuarioService(perfis.Object);

        var dados = await service.ObterInformacoesComplementaresDoUsuario(new UsuarioDTO
        {
            Codigo = "USR-SEM-CAT",
            Nome = "Usuário",
            Email = "usuario@atron.local"
        });

        Assert.DoesNotContain(
            dados.DadosDoPerfil.SelectMany(perfil => perfil.Modulos),
            modulo => modulo.Codigo == "CAT");
    }
}
