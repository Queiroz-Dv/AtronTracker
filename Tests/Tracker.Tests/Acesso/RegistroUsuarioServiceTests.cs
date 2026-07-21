using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Extensions;
using Application.Services.AuthServices;
using Application.Services.AuthServices.Bases;
using Application.UseCases.Usuario;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Acesso;

public class RegistroUsuarioServiceTests
{
    [Theory]
    [InlineData("", "http://localhost:5000")]
    [InlineData("http://localhost:4200", "http://localhost:4200")]
    public void ObterUri_DeveUsarClientUriOuUriDaRequisicao(string clientUri, string esperado)
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        httpContextAccessor.HttpContext.Request.Scheme = "http";
        httpContextAccessor.HttpContext.Request.Host = new HostString("localhost:5000");

        var service = new AuthUriBaseServiceFake(httpContextAccessor);

        Assert.Equal(esperado, service.Obter(clientUri));
    }

    [Fact]
    public async Task SolicitarRecuperacaoSenha_DeveBuscarPorCodigoQuandoIdentificadorNaoForEmail()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var service = CriarService(usuarioRepository);

        var resultado = await service.SolicitarRecuperacaoSenha(new SolicitarRecuperacaoSenhaRequest
        {
            Identificador = "USR001",
            ClientUri = "http://localhost:4200"
        });

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(1, usuarioRepository.BuscasPorCodigo);
        Assert.Equal(0, usuarioRepository.BuscasPorEmail);
    }

    [Fact]
    public async Task SolicitarRecuperacaoSenha_DeveBuscarPorEmailQuandoIdentificadorForEmail()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var service = CriarService(usuarioRepository);

        var resultado = await service.SolicitarRecuperacaoSenha(new SolicitarRecuperacaoSenhaRequest
        {
            Identificador = "usuario@teste.com",
            ClientUri = "http://localhost:4200"
        });

        Assert.True(resultado.TeveSucesso);
        Assert.Equal(0, usuarioRepository.BuscasPorCodigo);
        Assert.Equal(1, usuarioRepository.BuscasPorEmail);
    }

    [Theory]
    [InlineData("USR001", false)]
    [InlineData("usuario@teste.com", true)]
    [InlineData("@teste.com", false)]
    [InlineData("usuario@", false)]
    public void IdentifierIsEmail_DeveValidarFormatoMinimo(string identificador, bool esperado)
    {
        Assert.Equal(esperado, identificador.IdentifierIsEmail());
    }

    [Fact]
    public void GerarIdentificador_DeveGerarCodigoNumericoComSeisDigitos()
    {
        var service = new ConfirmacaoEmailCodigoService();

        var identificador = service.GerarIdentificador();

        Assert.Equal(6, identificador.Length);
        Assert.All(identificador, caractere => Assert.True(char.IsDigit(caractere)));
    }

    [Fact]
    public async Task ReenviarConfirmacaoEmail_DeveNormalizarCodigoUsuarioAntesDeBuscar()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var confirmacaoRepository = new ConfirmacaoEmailRepositoryFake();
        var service = CriarReenviarConfirmacaoEmail(usuarioRepository, confirmacaoRepository);

        var resultado = await service.ExecutarPorIdentificadorAsync(" usr001 ", "http://localhost:4200");

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("USR001", usuarioRepository.UltimoCodigoGeralBuscado);
        Assert.NotNull(confirmacaoRepository.UltimaConfirmacaoGravada);
        Assert.Equal("USR001", confirmacaoRepository.UltimaConfirmacaoGravada.UsuarioCodigo);
    }

    [Fact]
    public async Task RegistrarUsuario_DeveManterCadastroEAdicionarAvisoQuandoEmailFalha()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var service = CriarService(usuarioRepository, Resultado.Falha("falha de transporte"));

        var resultado = await service.RegistrarUsuario(new UsuarioRegistroRequest
        {
            Codigo = "USR001",
            Nome = "Usuario",
            Sobrenome = "Teste",
            Email = "usuario@teste.com",
            Senha = "Senha@123",
            ConfirmaSenha = "Senha@123",
            ClientUri = "http://localhost:4200"
        });

        Assert.True(resultado.TeveSucesso);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
    }

    private static RegistroUsuarioService CriarService(
        UsuarioRepositoryFake usuarioRepository,
        Resultado? resultadoEmail = null)
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        httpContextAccessor.HttpContext.Request.Scheme = "http";
        httpContextAccessor.HttpContext.Request.Host = new HostString("localhost:5000");

        var identidade = new UsuarioIdentityRepositoryFake();
        var email = new EmailServiceFake(resultadoEmail);
        var compositor = new AcessoEmailCompositor(new EmailTemplateRenderer());
        var cadastro = new CadastroUsuarioService(new CadastroUsuarioContext(
            usuarioRepository,
            new PerfilDeAcessoUsuarioRepositoryFake(),
            new PerfilDeAcessoRepositoryFake(),
            identidade,
            email,
            compositor,
            new ValidadorFake(),
            httpContextAccessor,
            new ConfirmacaoEmailRepositoryFake(),
            new ConfirmacaoEmailCodigoService()));
        var recuperacao = new RecuperacaoSenhaService(new RecuperacaoSenhaContext(
            usuarioRepository,
            identidade,
            new LoginRepositoryFake(),
            new CacheServiceFake(),
            email,
            compositor,
            httpContextAccessor));

        return new RegistroUsuarioService(cadastro, recuperacao);
    }

    private static ReenviarConfirmacaoEmail CriarReenviarConfirmacaoEmail(
        UsuarioRepositoryFake usuarioRepository,
        ConfirmacaoEmailRepositoryFake confirmacaoRepository)
    {
        var httpContextAccessor = new HttpContextAccessor
        {
            HttpContext = new DefaultHttpContext()
        };
        httpContextAccessor.HttpContext.Request.Scheme = "http";
        httpContextAccessor.HttpContext.Request.Host = new HostString("localhost:5000");

        return new ReenviarConfirmacaoEmail(
            usuarioRepository,
            confirmacaoRepository,
            new ConfirmacaoEmailCodigoService(),
            new EmailServiceFake(),
            new AcessoEmailCompositor(new EmailTemplateRenderer()),
            httpContextAccessor);
    }

    private sealed class UsuarioRepositoryFake : IUsuarioRepository
    {
        private readonly Usuario _usuario;

        public UsuarioRepositoryFake(Usuario usuario)
        {
            _usuario = usuario;
        }

        public int BuscasPorCodigo { get; private set; }
        public int BuscasPorEmail { get; private set; }
        public string UltimoCodigoGeralBuscado { get; private set; }

        public Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo)
        {
            BuscasPorCodigo++;
            UltimoCodigoGeralBuscado = codigo;
            return Task.FromResult(codigo == _usuario.Codigo ? _usuario : null);
        }

        public Task<Usuario> ObterUsuarioGeralPorEmailAsync(string email)
        {
            BuscasPorEmail++;
            return Task.FromResult(_usuario);
        }

        public Task<IEnumerable<Usuario>> ObterTodosRepositoryAsync() => Task.FromResult(Enumerable.Empty<Usuario>());
        public Task<Usuario> ObterPorIdRepositoryAsync(int id) => Task.FromResult<Usuario>(null);
        public Task<Usuario> ObterPorCodigoRepositoryAsync(string codigo) => Task.FromResult<Usuario>(null);
        public Task<bool> CriarRepositoryAsync(Usuario entity) => Task.FromResult(false);
        public Task<bool> AtualizarRepositoryAsync(Usuario entity) => Task.FromResult(false);
        public Task<bool> AtualizarRepositoryAsync(int id, Usuario entity) => Task.FromResult(false);
        public Task<bool> RemoverRepositoryAsync(Usuario entity) => Task.FromResult(false);
        public Task<IEnumerable<Usuario>> ObterUsuariosAsync() => Task.FromResult(Enumerable.Empty<Usuario>());
        public Task<Usuario> ObterUsuarioPorIdAsync(int? id) => Task.FromResult<Usuario>(null);
        public Task<Usuario> ObterUsuarioPorCodigoAsync(string codigo) => Task.FromResult(codigo == _usuario.Codigo ? _usuario : null);
        public Task<Usuario> ObterInativoPorEmailAsync(string email) => Task.FromResult<Usuario>(null);
        public Task<bool> CriarUsuarioAsync(Usuario usuario) => Task.FromResult(true);
        public Task<bool> AtualizarUsuarioAsync(Usuario usuario) => Task.FromResult(false);
        public Task<bool> AtualizarPreferenciaNotificacaoTarefaPorEmailAsync(string codigo, bool receberNotificacao) => Task.FromResult(false);
        public Task<bool> AtualizarPreferenciasNotificacaoTarefaAsync(string codigo, bool receberNotificacaoInterna, bool receberNotificacaoPorEmail) => Task.FromResult(false);
        public Task<bool> ConfirmarEmailAsync(string codigo) => Task.FromResult(true);
        public Task<bool> RemoverUsuarioAsync(Usuario usuario) => Task.FromResult(false);
        public Task<List<UsuarioIdentity>> ObterTodosUsuariosDoIdentity() => Task.FromResult(new List<UsuarioIdentity>());
        public Task<bool> VerificarEmailExistenteAsync(string email) => Task.FromResult(false);
    }

    private sealed class UsuarioIdentityRepositoryFake : IUsuarioIdentityRepository
    {
        public Task<bool> ContaExisteRepositoryAsync(string codigoUsuario, string email) => Task.FromResult(false);
        public Task<bool> RegistrarContaDeUsuarioRepositoryAsync(string codigoUsuario, string email, string senha) => Task.FromResult(true);
        public Task<bool> AtualizarUserIdentityRepositoryAsync(string codigoUsuario, string email, string senha) => Task.FromResult(true);
        public Task<bool> DeletarContaUserRepositoryAsync(string codigoUsuario) => Task.FromResult(true);
        public Task<bool> DesativarContaAsync(string codigoUsuario) => Task.FromResult(true);
        public Task<bool> ReativarContaAsync(string codigoUsuario) => Task.FromResult(true);
        public Task<bool> AtualizarRefreshTokenUsuarioRepositoryAsync(string codigoUsuario, string refreshToken, DateTime refreshTokenExpireTime) => Task.FromResult(true);
        public Task<bool> RefreshTokenExisteRepositoryAsync(string refreshToken) => Task.FromResult(true);
        public Task<string> ObterRefreshTokenPorCodigoUsuarioRepositoryAsync(string codigoUsuario) => Task.FromResult("refresh-token");
        public Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario) => Task.FromResult(true);
        public Task<bool> RefreshTokenExpiradoRepositoryAsync(string codigoUsuario) => Task.FromResult(false);
        public Task<string> GerarTokenConfirmacaoEmailAsync(string codigoUsuario) => Task.FromResult("token-confirmacao");
        public Task<bool> ConfirmarEmailAsync(string codigoUsuario, string token) => Task.FromResult(true);
        public Task<UsuarioIdentity> ObterUsuarioIdentityPorCodigo(string codigoUsuario) => Task.FromResult<UsuarioIdentity>(null);
        public Task<string> GerarTokenAlteracaoEmailAsync(string codigoUsuario, string emailNovo) => Task.FromResult("token-email");
        public Task<bool> ConfirmarAlteracaoEmailAsync(string codigoUsuario, string emailNovo, string token) => Task.FromResult(true);
        public Task<string> GerarTokenRecuperacaoSenhaAsync(string codigoUsuario) => Task.FromResult("token-recuperacao");
        public Task<bool> RedefinirSenhaAsync(string codigoUsuario, string token, string novaSenha) => Task.FromResult(true);
        public Task<bool> EmailConfirmadoAsync(string codigoUsuario) => Task.FromResult(false);
    }

    private sealed class EmailServiceFake(Resultado? resultado = null) : IEmailService
    {
        public Task<Resultado> EnviarAsync(EmailRequest message) => Task.FromResult(resultado ?? Resultado.Sucesso());
    }

    private sealed class CacheServiceFake : ICacheService
    {
        public void GravarCache<T>(CacheInfo<T> cacheInfo) { }
        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao) { }
        public T ObterCache<T>(ChaveCache chaveCache) => default;
        public void RemoverCache(ChaveCache chaveCache) { }
    }

    private sealed class ConfirmacaoEmailRepositoryFake : IConfirmacaoEmailRepository
    {
        public ConfirmacaoEmail UltimaConfirmacaoGravada { get; private set; }

        public Task<bool> GravarOuSubstituirAsync(ConfirmacaoEmail confirmacaoEmail)
        {
            UltimaConfirmacaoGravada = confirmacaoEmail;
            return Task.FromResult(true);
        }
        public Task<ConfirmacaoEmail> ObterAtivaPorUsuarioAsync(string usuarioCodigo) => Task.FromResult<ConfirmacaoEmail>(null);
        public Task<bool> MarcarConfirmadaAsync(int id) => Task.FromResult(true);
        public Task<IEnumerable<ConfirmacaoEmail>> ObterTodosRepositoryAsync() => Task.FromResult(Enumerable.Empty<ConfirmacaoEmail>());
        public Task<ConfirmacaoEmail> ObterPorIdRepositoryAsync(int id) => Task.FromResult<ConfirmacaoEmail>(null);
        public Task<ConfirmacaoEmail> ObterPorCodigoRepositoryAsync(string codigo) => Task.FromResult<ConfirmacaoEmail>(null);
        public Task<bool> CriarRepositoryAsync(ConfirmacaoEmail entity) => Task.FromResult(true);
        public Task<bool> AtualizarRepositoryAsync(ConfirmacaoEmail entity) => Task.FromResult(true);
        public Task<bool> AtualizarRepositoryAsync(int id, ConfirmacaoEmail entity) => Task.FromResult(true);
        public Task<bool> RemoverRepositoryAsync(ConfirmacaoEmail entity) => Task.FromResult(true);
    }

    private sealed class ValidadorFake : IValidador<UsuarioRegistroRequest>
    {
        public IList<NotificationMessage> Validar(UsuarioRegistroRequest entity) => new List<NotificationMessage>();
    }

    private sealed class AccessorServiceFake : IAccessorService
    {
        public T ObterService<T>() where T : class => null;
    }

    private sealed class LoginRepositoryFake : ILoginRepository
    {
        public Task<bool> AtualizarSenhaUsuario(string codigoDoUsuario, string senha) => Task.FromResult(true);
        public Task<bool> AutenticarUsuarioAsync(UsuarioIdentity usuarioIdentity) => Task.FromResult(true);
        public Task Logout() => Task.CompletedTask;
    }

    private sealed class PerfilDeAcessoRepositoryFake : IPerfilDeAcessoRepository
    {
        public Task<ICollection<PerfilDeAcesso>> ObterTodosPerfisRepositoryAsync() => Task.FromResult<ICollection<PerfilDeAcesso>>(new List<PerfilDeAcesso>());
        public Task<PerfilDeAcesso> ObterPerfilPorIdRepositoryAsync(int id) => Task.FromResult<PerfilDeAcesso>(null);
        public Task<PerfilDeAcesso> ObterPerfilPorCodigoRepositoryAsync(string codigo) => Task.FromResult<PerfilDeAcesso>(null);
        public Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso) => Task.FromResult(false);
        public Task<bool> AtualizarPerfilRepositoryAsync(string codigo, PerfilDeAcesso perfilDeAcesso) => Task.FromResult(false);
        public Task<bool> DeletarPerfilRepositoryAsync(PerfilDeAcesso perfil) => Task.FromResult(false);
        public Task<List<PerfilDeAcesso>> ObterPerfisPorCodigoDeUsuarioRepositoryAsync(string usuarioCodigo) => Task.FromResult(new List<PerfilDeAcesso>());
    }

    private sealed class PerfilDeAcessoUsuarioRepositoryFake : IPerfilDeAcessoUsuarioRepository
    {
        public Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso) => Task.FromResult(false);
        public Task<bool> CriarRelacionamentoRepositoryAsync(PerfilDeAcessoUsuario perfilDeAcesso) => Task.FromResult(false);
        public Task<PerfilDeAcessoUsuario> ObterPerfilDeAcessoPorCodigoRepositoryAsync(string codigo) => Task.FromResult<PerfilDeAcessoUsuario>(null);
        public Task DeletarRelacionamento(PerfilDeAcessoUsuario relacionamento) => Task.CompletedTask;
    }

    private sealed class AuthUriBaseServiceFake(IHttpContextAccessor httpContextAccessor)
        : AuthUriBaseService(httpContextAccessor)
    {
        public string Obter(string clientUri) => ObterUri(clientUri);
    }
}
