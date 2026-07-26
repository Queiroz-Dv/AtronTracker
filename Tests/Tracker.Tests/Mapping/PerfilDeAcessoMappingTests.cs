using Application.DTO;
using Application.Mapping;
using Domain.Entities;
using Moq;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;
using Xunit;

namespace Tracker.Tests.Mapping;

public class PerfilDeAcessoMappingTests
{
    [Fact]
    public async Task MapToPerfilDeAcessoUsuarioDTOAsync_DeveMapearPerfilModulosEUsuariosRelacionados()
    {
        var moduloMap = new Mock<IAsyncApplicationMapService<ModuloDTO, Modulo>>();
        moduloMap
            .Setup(mapeador => mapeador.MapToDTOAsync(It.IsAny<Modulo>()))
            .ReturnsAsync((Modulo modulo) => new ModuloDTO
            {
                Codigo = modulo.Codigo,
                Descricao = modulo.Descricao
            });
        var mapeador = new PerfilDeAcessoMapping(
            moduloMap.Object,
            new UsuarioDoPerfilDeAcessoMapping(),
            new MapperEngine());
        var perfil = new PerfilDeAcesso
        {
            Id = 9,
            Codigo = "PRF",
            Descricao = "Perfil de tarefas",
            PerfilDeAcessoModulos =
            [
                new PerfilDeAcessoModulo
                {
                    Modulo = new Modulo { Codigo = "TRF", Descricao = "Tarefas" }
                }
            ],
            PerfisDeAcessoUsuario =
            [
                new PerfilDeAcessoUsuario
                {
                    Usuario = new Usuario { Codigo = "USR-1", Nome = "Ana", Sobrenome = "Silva" }
                },
                new PerfilDeAcessoUsuario
                {
                    Usuario = new Usuario { Codigo = "USR-2", Nome = "Bruno", Sobrenome = "Souza" }
                }
            ]
        };

        var dto = await mapeador.MapToPerfilDeAcessoUsuarioDTOAsync(perfil);

        Assert.Equal("PRF", dto.PerfilDeAcesso.Codigo);
        Assert.Equal("Perfil de tarefas", dto.PerfilDeAcesso.Descricao);
        Assert.Equal(0, dto.PerfilDeAcesso.Id);
        Assert.Collection(
            dto.PerfilDeAcesso.Modulos,
            modulo => Assert.Equal("TRF", modulo.Codigo));
        Assert.Collection(
            dto.Usuarios,
            usuario => Assert.Equal(("USR-1", "Ana", "Silva"), (usuario.Codigo, usuario.Nome, usuario.Sobrenome)),
            usuario => Assert.Equal(("USR-2", "Bruno", "Souza"), (usuario.Codigo, usuario.Nome, usuario.Sobrenome)));
    }
}
