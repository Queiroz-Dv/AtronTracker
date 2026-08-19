using Application.DTO;
using Application.Mapping;
using Domain.Entities;
using Xunit;

namespace Tracker.Tests.Mapping;

public class PerfilDeAcessoMappingTests
{
    [Fact]
    public void MapToPerfilDeAcessoUsuarioDto_DeveMapearPerfilModulosEUsuariosRelacionados()
    {
        var mapeador = new PerfilDeAcessoMapping(
            new ModuloMapping(),
            new UsuarioDoPerfilDeAcessoMapping());
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

        var dto = mapeador.MapToPerfilDeAcessoUsuarioDto(perfil);

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
