using Application.DTO;
using Application.Interfaces.Mapping;
using Application.Interfaces.Services;
using Application.Resources;
using Application.Services.EntitiesServices.PerfisDeAcesso;
using Application.Validations;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Moq;
using Shared.Application.Interfaces.Service;
using Shared.Infrastructure.Repositories;
using Xunit;

namespace Tracker.Tests;

public class PerfilDeAcessoServiceTests
{
    [Fact]
    public async Task CriarAsync_DeveBloquearPerfilSemModulo()
    {
        var cenario = CriarCenario();

        var resultado = await cenario.Service.CriarAsync(new PerfilDeAcessoDTO());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(resultado.Messages, mensagem => mensagem.Descricao == PerfilDeAcessoResource.Erro_SemModulos);
        cenario.Perfis.Verify(repositorio => repositorio.CriarPerfilRepositoryAsync(It.IsAny<PerfilDeAcesso>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_DeveAtualizarPerfilEInvalidarCacheDosUsuariosAfetados()
    {
        var cenario = CriarCenario();
        var dto = CriarPerfilDto();
        var perfilAtual = CriarPerfilComUsuario("USR-ATUAL");
        cenario.Perfis
            .Setup(repositorio => repositorio.ObterPerfilPorCodigoRepositoryAsync("PRF"))
            .ReturnsAsync(perfilAtual);
        cenario.Perfis
            .Setup(repositorio => repositorio.AtualizarPerfilRepositoryAsync("PRF", It.IsAny<PerfilDeAcesso>()))
            .ReturnsAsync(true);

        var resultado = await cenario.Service.AtualizarAsync("PRF", dto);

        Assert.True(resultado.TeveSucesso);
        cenario.Cache.Verify(cache => cache.RemoverCacheDeAcessoTokenInfo("USR-ATUAL"), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DeveInvalidarCacheSomenteAposRemocaoConfirmada()
    {
        var cenario = CriarCenario();
        var perfil = CriarPerfilComUsuario("USR-REMOVER");
        cenario.Perfis
            .Setup(repositorio => repositorio.ObterPerfilPorCodigoRepositoryAsync("PRF"))
            .ReturnsAsync(perfil);
        cenario.Perfis
            .Setup(repositorio => repositorio.DeletarPerfilRepositoryAsync(perfil))
            .ReturnsAsync(true);

        var resultado = await cenario.Service.RemoverAsync("PRF");

        Assert.True(resultado.TeveSucesso);
        cenario.Cache.Verify(cache => cache.RemoverCacheDeAcessoTokenInfo("USR-REMOVER"), Times.Once);
    }

    [Fact]
    public async Task RelacionarPerfilDeAcessoUsuarioAsync_DevePersistirVinculoEInvalidarCachesAfetados()
    {
        var cenario = CriarCenario();
        var perfil = new PerfilDeAcesso
        {
            Id = 9,
            Codigo = "PRF",
            PerfisDeAcessoUsuario = []
        };
        cenario.Perfis
            .Setup(repositorio => repositorio.ObterPerfilPorCodigoRepositoryAsync("PRF"))
            .ReturnsAsync(perfil);
        cenario.Usuarios
            .Setup(repositorio => repositorio.ObterUsuarioPorCodigoAsync("USR-NOVO"))
            .ReturnsAsync(new Usuario { Id = 7, Codigo = "USR-NOVO" });
        cenario.PerfisUsuarios
            .Setup(repositorio => repositorio.CriarPerfilRepositoryAsync(It.IsAny<PerfilDeAcessoUsuario>()))
            .ReturnsAsync(true);

        var resultado = await cenario.Service.RelacionarPerfilDeAcessoUsuarioAsync(new PerfilDeAcessoUsuarioDTO
        {
            PerfilDeAcesso = CriarPerfilDto(),
            Usuarios = [new UsuarioDTO { Codigo = "USR-NOVO" }]
        });

        Assert.True(resultado.TeveSucesso);
        cenario.PerfisUsuarios.Verify(repositorio => repositorio.CriarPerfilRepositoryAsync(
            It.Is<PerfilDeAcessoUsuario>(relacao => relacao.PerfilDeAcessoCodigo == "PRF" && relacao.UsuarioCodigo == "USR-NOVO")), Times.Once);
        cenario.Cache.Verify(cache => cache.RemoverCacheDeAcessoTokenInfo("USR-NOVO"), Times.Once);
    }

    [Fact]
    public async Task RelacionarPerfilDeAcessoUsuarioAsync_DeveValidarTodosOsUsuariosAntesDeRemoverVinculosAtuais()
    {
        var cenario = CriarCenario();
        var perfil = CriarPerfilComUsuario("USR-ANTIGO");
        cenario.Perfis
            .Setup(repositorio => repositorio.ObterPerfilPorCodigoRepositoryAsync("PRF"))
            .ReturnsAsync(perfil);

        var resultado = await cenario.Service.RelacionarPerfilDeAcessoUsuarioAsync(new PerfilDeAcessoUsuarioDTO
        {
            PerfilDeAcesso = CriarPerfilDto(),
            Usuarios = [new UsuarioDTO { Codigo = "USR-INEXISTENTE" }]
        });

        Assert.True(resultado.TeveFalha);
        cenario.PerfisUsuarios.Verify(repositorio => repositorio.DeletarRelacionamento(It.IsAny<PerfilDeAcessoUsuario>()), Times.Never);
        cenario.Cache.Verify(cache => cache.RemoverCacheDeAcessoTokenInfo(It.IsAny<string>()), Times.Never);
    }

    private static Cenario CriarCenario()
    {
        var perfisUsuarios = new Mock<IPerfilDeAcessoUsuarioRepository>();
        var usuarios = new Mock<IUsuarioRepository>();
        var mapa = new Mock<IPerfilDeAcessoMapping>();
        mapa.Setup(servico => servico.MapToEntityAsync(It.IsAny<PerfilDeAcessoDTO>()))
            .ReturnsAsync((PerfilDeAcessoDTO dto) => new PerfilDeAcesso
            {
                Id = dto.Id,
                Codigo = dto.Codigo,
                Descricao = dto.Descricao,
                PerfisDeAcessoUsuario = []
            });
        var perfis = new Mock<IPerfilDeAcessoRepository>();
        var modulos = new Mock<IModuloRepository>();
        modulos.Setup(repositorio => repositorio.ObterPorCodigoRepository("TRF"))
            .ReturnsAsync(new Modulo { Id = 3, Codigo = "TRF" });
        var cache = new Mock<ICacheUsuarioService>();
        var mensagens = new PerfilDeAcessoMessageValidation();
        var preparacao = new PerfilDeAcessoPreparacaoService(
            mapa.Object,
            modulos.Object,
            mensagens,
            mensagens);
        var invalidacaoCache = new PerfilDeAcessoCacheInvalidator(cache.Object);
        var escopoTransacao = new Mock<ITransactionScope>();
        var transacoes = new Mock<ITransactionManager>();
        transacoes.Setup(gerenciador => gerenciador.CreateScope()).Returns(escopoTransacao.Object);
        var relacionamento = new PerfilDeAcessoUsuarioRelacionamentoService(
            perfisUsuarios.Object,
            usuarios.Object,
            mapa.Object,
            perfis.Object,
            invalidacaoCache,
            transacoes.Object);

        return new Cenario(
            new PerfilDeAcessoService(
                mapa.Object,
                perfis.Object,
                preparacao,
                relacionamento,
                invalidacaoCache),
            perfisUsuarios,
            usuarios,
            perfis,
            cache);
    }

    private static PerfilDeAcessoDTO CriarPerfilDto()
    {
        return new PerfilDeAcessoDTO
        {
            Codigo = "PRF",
            Descricao = "Perfil de tarefas",
            Modulos = [new ModuloDTO { Codigo = "TRF" }]
        };
    }

    private static PerfilDeAcesso CriarPerfilComUsuario(string codigoUsuario)
    {
        return new PerfilDeAcesso
        {
            Id = 9,
            Codigo = "PRF",
            PerfisDeAcessoUsuario = [new PerfilDeAcessoUsuario { UsuarioCodigo = codigoUsuario }]
        };
    }

    private sealed record Cenario(
        PerfilDeAcessoService Service,
        Mock<IPerfilDeAcessoUsuarioRepository> PerfisUsuarios,
        Mock<IUsuarioRepository> Usuarios,
        Mock<IPerfilDeAcessoRepository> Perfis,
        Mock<ICacheUsuarioService> Cache);
}
