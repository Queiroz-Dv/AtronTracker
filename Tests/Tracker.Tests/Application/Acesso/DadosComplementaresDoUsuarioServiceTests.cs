using Application.DTO;
using Application.Interfaces.Services;
using Application.Services.AuthServices;
using Application.UseCases.WorkspaceCases;
using Moq;
using Xunit;

namespace Tracker.Tests.Application.Acesso;

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
        var usuarioMapper = new Mock<global::Shared.Application.Interfaces.Mapping.IToDtoMapper<global::Domain.Entities.Usuario, global::Application.DTO.UsuarioDTO>>();
        var mapping = new global::Application.Mapping.WorkspaceMapping(usuarioMapper.Object);
        var workspaceRepo = new Mock<global::Domain.Interfaces.UsuarioInterfaces.IWorkspaceRepository>();
        workspaceRepo.Setup(r => r.ObterWorkspacePorResponsavelEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((global::Domain.Entities.Workspace?)null);
        var obterWorkspaceCase = new global::Application.UseCases.WorkspaceCases.ObterWorkspaceCase(mapping, workspaceRepo.Object);
        var service = new DadosComplementaresDoUsuarioService(obterWorkspaceCase, perfis.Object);

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
        var usuarioMapper = new Mock<global::Shared.Application.Interfaces.Mapping.IToDtoMapper<global::Domain.Entities.Usuario, global::Application.DTO.UsuarioDTO>>();
        var mapping = new global::Application.Mapping.WorkspaceMapping(usuarioMapper.Object);
        var workspaceRepo = new Mock<global::Domain.Interfaces.UsuarioInterfaces.IWorkspaceRepository>();
        workspaceRepo.Setup(r => r.ObterWorkspacePorResponsavelEmailAsync(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync((global::Domain.Entities.Workspace?)null);
        var obterWorkspaceCase = new global::Application.UseCases.WorkspaceCases.ObterWorkspaceCase(mapping, workspaceRepo.Object);
        var service = new DadosComplementaresDoUsuarioService(obterWorkspaceCase, perfis.Object);

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
