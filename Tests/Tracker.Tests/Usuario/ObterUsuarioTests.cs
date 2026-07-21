using Application.DTO;
using Application.UseCases.Usuario;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Xunit;

namespace Tracker.Tests.Usuarios;

public class ObterUsuarioTests
{
    [Fact]
    public async Task ExecutarAsync_DeveFalharQuandoCodigoForVazio()
    {
        var repositorio = new Mock<IUsuarioRepository>();
        var mapa = new Mock<IAsyncMap<UsuarioDTO, Domain.Entities.Usuario>>();
        var casoDeUso = new ObterUsuario(mapa.Object, repositorio.Object);

        var resultado = await casoDeUso.ExecutarAsync(string.Empty);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages,
            mensagem => mensagem.Descricao == NotificacoesPadronizadas.ErroCampoInvalido);
        repositorio.Verify(
            item => item.ObterUsuarioPorCodigoAsync(It.IsAny<string>()),
            Times.Never);
        mapa.Verify(
            item => item.MapToDTOAsync(It.IsAny<Domain.Entities.Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DeveFalharQuandoUsuarioNaoForEncontrado()
    {
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("USR"))
            .ReturnsAsync((Domain.Entities.Usuario)null!);
        var mapa = new Mock<IAsyncMap<UsuarioDTO, Domain.Entities.Usuario>>();
        var casoDeUso = new ObterUsuario(mapa.Object, repositorio.Object);

        var resultado = await casoDeUso.ExecutarAsync("USR");

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages,
            mensagem => mensagem.Descricao == NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
        mapa.Verify(
            item => item.MapToDTOAsync(It.IsAny<Domain.Entities.Usuario>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecutarAsync_DeveRetornarUsuarioMapeado()
    {
        var usuario = new Domain.Entities.Usuario { Codigo = "USR" };
        var usuarioDto = new UsuarioDTO { Codigo = "USR" };
        var repositorio = new Mock<IUsuarioRepository>();
        repositorio
            .Setup(item => item.ObterUsuarioPorCodigoAsync("USR"))
            .ReturnsAsync(usuario);
        var mapa = new Mock<IAsyncMap<UsuarioDTO, Domain.Entities.Usuario>>();
        mapa
            .Setup(item => item.MapToDTOAsync(usuario))
            .ReturnsAsync(usuarioDto);
        var casoDeUso = new ObterUsuario(mapa.Object, repositorio.Object);

        var resultado = await casoDeUso.ExecutarAsync("USR");

        Assert.True(resultado.TeveSucesso);
        Assert.Same(usuarioDto, resultado.Dados);
    }
}
