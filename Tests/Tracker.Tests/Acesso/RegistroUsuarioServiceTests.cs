using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Services.AuthServices;
using Application.UseCases.UsuarioCases;
using AtronTracker.Infrastructure.Configuration;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.Extensions.Configuration;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Requests;
using Shared.Application.Email.Rendering;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.Acesso;

public class RegistroUsuarioServiceTests
{
    [Fact]
    public void EnderecoFrontend_DevePriorizarEnderecoConfigurado()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:ClientBaseUri"] = "https://front.atron.test/caminho-ignorado",
                ["Cors:AllowedOrigins:0"] = "https://origem-alternativa.test"
            })
            .Build();

        var service = new EnderecoFrontendService(configuration);

        Assert.Equal("https://front.atron.test", service.ObterUriBase());
    }

    [Fact]
    public void EnderecoFrontend_DeveUsarOrigemCorsUnicaComoFallback()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "https://front.atron.test"
            })
            .Build();

        var service = new EnderecoFrontendService(configuration);

        Assert.Equal("https://front.atron.test", service.ObterUriBase());
    }

    [Fact]
    public void EnderecoFrontend_DeveExigirEnderecoExplicitoQuandoHaMultiplasOrigensCors()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Cors:AllowedOrigins:0"] = "https://front.atron.test",
                ["Cors:AllowedOrigins:1"] = "https://outro-front.atron.test"
            })
            .Build();

        var excecao = Assert.Throws<InvalidOperationException>(
            () => new EnderecoFrontendService(configuration));

        Assert.Equal(AuthResource.Erro_UriFrontendNaoConfigurada, excecao.Message);
    }

    [Fact]
    public void TokenTemporario_DeveSerOpacoAleatorioEArmazenavelPorHash()
    {
        var service = new TokenTemporarioService();

        var primeiro = service.Criar();
        var segundo = service.Criar();

        Assert.Equal(43, primeiro.Valor.Length);
        Assert.Equal(64, primeiro.Hash.Length);
        Assert.Equal(primeiro.Hash, service.ObterHash(primeiro.Valor));
        Assert.NotEqual(primeiro.Valor, segundo.Valor);
    }

    [Theory]
    [InlineData(typeof(LoginRequestDTO))]
    [InlineData(typeof(SolicitarRecuperacaoSenhaRequest))]
    [InlineData(typeof(ReenviarConfirmacaoEmailRequest))]
    [InlineData(typeof(UsuarioRegistroRequest))]
    [InlineData(typeof(AlterarEmailRequest))]
    public void ContratosDeAcesso_NaoDevemAceitarClientUri(Type contrato)
    {
        Assert.Null(contrato.GetProperty("ClientUri"));
    }

    [Fact]
    public void CadastroPublico_NaoDeveAceitarPerfilDeAcesso()
    {
        Assert.Null(typeof(UsuarioRegistroRequest).GetProperty("CodigoPerfilDeAcesso"));
    }

    [Fact]
    public async Task RecuperacaoSenha_DeveUsarOrigemConfiavelETokenDeUsoUnico()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var emailService = new EmailServiceFake();
        var cacheService = new CacheServiceFake();
        var service = CriarRecuperacaoSenha(usuarioRepository, emailService, cacheService);

        var solicitacao = await service.SolicitarAsync(new SolicitarRecuperacaoSenhaRequest
        {
            Identificador = "USR001"
        });

        Assert.True(solicitacao.TeveSucesso);
        Assert.NotNull(emailService.UltimoRequest);
        Assert.Contains("https://front.atron.test/trocar-senha#token=", emailService.UltimoRequest.Mensagem);
        Assert.DoesNotContain("?id=", emailService.UltimoRequest.Mensagem);

        var inicio = emailService.UltimoRequest.Mensagem.IndexOf("#token=", StringComparison.Ordinal) + 7;
        var fim = emailService.UltimoRequest.Mensagem.IndexOf('"', inicio);
        var token = emailService.UltimoRequest.Mensagem[inicio..fim];

        var primeiraTroca = await service.TrocarAsync(new RedefinirSenhaRequest
        {
            IdentificadorTemporario = token,
            NovaSenha = "NovaSenha@2026!",
            RepetirSenha = "NovaSenha@2026!"
        });
        var segundaTroca = await service.TrocarAsync(new RedefinirSenhaRequest
        {
            IdentificadorTemporario = token,
            NovaSenha = "OutraSenha@2026!",
            RepetirSenha = "OutraSenha@2026!"
        });

        Assert.True(primeiraTroca.TeveSucesso);
        Assert.True(segundaTroca.TeveFalha);
    }

    [Fact]
    public async Task RecuperacaoSenha_NaoDeveRevelarSeUsuarioExiste()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var service = CriarService(usuarioRepository);

        var existente = await service.SolicitarRecuperacaoSenha(new SolicitarRecuperacaoSenhaRequest
        {
            Identificador = "USR001"
        });
        var inexistente = await service.SolicitarRecuperacaoSenha(new SolicitarRecuperacaoSenhaRequest
        {
            Identificador = "DESCONHECIDO"
        });

        Assert.True(existente.TeveSucesso);
        Assert.True(inexistente.TeveSucesso);
        Assert.Equal(
            existente.Messages.Select(mensagem => mensagem.Descricao),
            inexistente.Messages.Select(mensagem => mensagem.Descricao));
    }

    [Fact]
    public async Task SolicitarRecuperacaoSenha_DeveBuscarPorCodigoQuandoIdentificadorNaoForEmail()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var service = CriarService(usuarioRepository);

        var resultado = await service.SolicitarRecuperacaoSenha(new SolicitarRecuperacaoSenhaRequest
        {
            Identificador = "USR001"
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
            Identificador = "usuario@teste.com"
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
    public async Task ConfirmacaoEmail_DeveBloquearAposCincoTentativasInvalidas()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var confirmacaoRepository = new ConfirmacaoEmailRepositoryFake();
        var codigoService = new ConfirmacaoEmailCodigoService();
        var confirmacao = codigoService.CriarDadosConfirmacao("USR001", 24);
        await confirmacaoRepository.GravarOuSubstituirAsync(confirmacao.ConfirmacaoEmail);
        var codigoInvalido = confirmacao.Identificador == "999999" ? "000000" : "999999";
        var cadastro = new CadastroUsuarioService(new CadastroUsuarioContext(
            usuarioRepository,
            new UsuarioIdentityRepositoryFake(),
            new EmailServiceFake(),
            new AcessoEmailCompositor(new EmailTemplateRenderer()),
            new ValidadorFake(),
            new EnderecoFrontendServiceFake(),
            confirmacaoRepository,
            codigoService));

        for (var tentativa = 0; tentativa < 6; tentativa++)
        {
            var resultado = await cadastro.ConfirmarEmailAsync("USR001", codigoInvalido);
            Assert.True(resultado.TeveFalha);
        }

        Assert.Equal(5, confirmacaoRepository.UltimaConfirmacaoGravada.TentativasFalhas);
    }

    [Fact]
    public async Task ReenviarConfirmacaoEmail_DeveBuscarCodigoUsuarioInformado()
    {
        var usuarioRepository = new UsuarioRepositoryFake(new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var confirmacaoRepository = new ConfirmacaoEmailRepositoryFake();
        var service = CriarReenviarConfirmacaoEmail(usuarioRepository, confirmacaoRepository);

        var resultado = await service.ExecutarPorIdentificadorAsync("USR001");

        Assert.True(resultado.TeveSucesso);
        Assert.Equal("USR001", usuarioRepository.UltimoCodigoGeralBuscado);
        Assert.NotNull(confirmacaoRepository.UltimaConfirmacaoGravada);
        Assert.Equal("USR001", confirmacaoRepository.UltimaConfirmacaoGravada.UsuarioCodigo);
    }

    [Fact]
    public async Task ReenviarConfirmacaoEmail_NaoDeveRevelarSeUsuarioExiste()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var confirmacaoRepository = new ConfirmacaoEmailRepositoryFake();
        var service = CriarReenviarConfirmacaoEmail(usuarioRepository, confirmacaoRepository);

        var existente = await service.ExecutarPorIdentificadorAsync("USR001");
        var inexistente = await service.ExecutarPorIdentificadorAsync("DESCONHECIDO");

        Assert.True(existente.TeveSucesso);
        Assert.True(inexistente.TeveSucesso);
        Assert.Equal(
            existente.Messages.Select(mensagem => mensagem.Descricao),
            inexistente.Messages.Select(mensagem => mensagem.Descricao));
    }

    [Fact]
    public async Task ReenviarConfirmacaoEmail_DeveRespeitarIntervaloMinimo()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var confirmacaoRepository = new ConfirmacaoEmailRepositoryFake();
        var emailService = new EmailServiceFake();
        var service = CriarReenviarConfirmacaoEmail(
            usuarioRepository,
            confirmacaoRepository,
            emailService);

        await service.ExecutarPorIdentificadorAsync("USR001");
        await service.ExecutarPorIdentificadorAsync("USR001");

        Assert.Equal(1, confirmacaoRepository.Gravacoes);
        Assert.Equal(1, emailService.Envios);
    }

    [Fact]
    public async Task RegistrarUsuario_DeveManterCadastroEAdicionarAvisoQuandoEmailFalha()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null),
            codigoJaExiste: false);
        var service = CriarService(usuarioRepository, Resultado.Falha("falha de transporte"));

        var resultado = await service.RegistrarUsuario(new UsuarioRegistroRequest
        {
            Codigo = "USR001",
            Nome = "Usuario",
            Sobrenome = "Teste",
            Email = "usuario@teste.com",
            Senha = "Senha@123",
            ConfirmaSenha = "Senha@123"
        });

        Assert.True(resultado.TeveSucesso);
        Assert.Contains(resultado.Messages, mensagem =>
            mensagem.Descricao == AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
    }

    [Fact]
    public async Task RegistrarUsuario_DeveReservarCodigoQueJaExisteNoCadastroDeNegocio()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null));
        var service = CriarService(usuarioRepository);

        var request = CriarRequestRegistro();
        request.Codigo = "USR001";

        var resultado = await service.RegistrarUsuario(request);

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == UsuarioResource.ErroUsuarioExistente);
    }

    [Fact]
    public async Task RegistrarUsuario_DeveBloquearEmailDeUsuarioOperacional()
    {
        var usuarioRepository = new UsuarioRepositoryFake(
            new Usuario("USR001", "Usuario", "Teste", "usuario@teste.com", null),
            codigoJaExiste: false,
            emailJaExiste: true);
        var service = CriarService(usuarioRepository);

        var resultado = await service.RegistrarUsuario(CriarRequestRegistro());

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == EmailResource.ErroEmailUtilizado);
    }

    private static UsuarioRegistroRequest CriarRequestRegistro()
        => new()
        {
            Codigo = "USR001",
            Nome = "Usuario",
            Sobrenome = "Teste",
            Email = "usuario@teste.com",
            Senha = "Senha@123",
            ConfirmaSenha = "Senha@123"
        };

    private static RegistroUsuarioService CriarService(
        UsuarioRepositoryFake usuarioRepository,
        Resultado? resultadoEmail = null)
    {
        var identidade = new UsuarioIdentityRepositoryFake();
        var email = new EmailServiceFake(resultadoEmail);
        var compositor = new AcessoEmailCompositor(new EmailTemplateRenderer());
        var enderecoFrontend = new EnderecoFrontendServiceFake();
        var tokenTemporario = new TokenTemporarioService();
        var cadastro = new CadastroUsuarioService(new CadastroUsuarioContext(
            usuarioRepository,
            identidade,
            email,
            compositor,
            new ValidadorFake(),
            enderecoFrontend,
            new ConfirmacaoEmailRepositoryFake(),
            new ConfirmacaoEmailCodigoService()));
        var recuperacao = new RecuperacaoSenhaService(new RecuperacaoSenhaContext(
            usuarioRepository,
            identidade,
            new LoginRepositoryFake(),
            new CacheServiceFake(),
            email,
            compositor,
            enderecoFrontend,
            tokenTemporario));

        return new RegistroUsuarioService(cadastro, recuperacao);
    }

    private static ReenviarConfirmacaoEmail CriarReenviarConfirmacaoEmail(
        UsuarioRepositoryFake usuarioRepository,
        ConfirmacaoEmailRepositoryFake confirmacaoRepository,
        EmailServiceFake? emailService = null)
    {
        return new ReenviarConfirmacaoEmail(
            usuarioRepository,
            confirmacaoRepository,
            new ConfirmacaoEmailCodigoService(),
            emailService ?? new EmailServiceFake(),
            new AcessoEmailCompositor(new EmailTemplateRenderer()),
            new EnderecoFrontendServiceFake());
    }

    private static RecuperacaoSenhaService CriarRecuperacaoSenha(
        UsuarioRepositoryFake usuarioRepository,
        EmailServiceFake emailService,
        CacheServiceFake cacheService)
    {
        return new RecuperacaoSenhaService(new RecuperacaoSenhaContext(
            usuarioRepository,
            new UsuarioIdentityRepositoryFake(),
            new LoginRepositoryFake(),
            cacheService,
            emailService,
            new AcessoEmailCompositor(new EmailTemplateRenderer()),
            new EnderecoFrontendServiceFake(),
            new TokenTemporarioService()));
    }

    private sealed class UsuarioRepositoryFake : IUsuarioRepository
    {
        private readonly Usuario _usuario;
        private readonly bool _codigoJaExiste;
        private readonly bool _emailJaExiste;

        public UsuarioRepositoryFake(
            Usuario usuario,
            bool codigoJaExiste = true,
            bool emailJaExiste = false)
        {
            _usuario = usuario;
            _codigoJaExiste = codigoJaExiste;
            _emailJaExiste = emailJaExiste;
        }

        public int BuscasPorCodigo { get; private set; }
        public int BuscasPorEmail { get; private set; }
        public string UltimoCodigoGeralBuscado { get; private set; }

        public Task<Usuario> ObterUsuarioGeralPorCodigoAsync(string codigo)
        {
            BuscasPorCodigo++;
            UltimoCodigoGeralBuscado = codigo;
            return Task.FromResult(
                _codigoJaExiste && codigo == _usuario.Codigo
                    ? _usuario
                    : null);
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
        public Task<bool> VerificarEmailExistenteAsync(string email)
            => Task.FromResult(_emailJaExiste);
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
        public Task<SessaoRefreshToken> ObterSessaoRefreshTokenRepositoryAsync(string refreshTokenHash) => Task.FromResult<SessaoRefreshToken>(null);
        public Task<bool> RotacionarRefreshTokenRepositoryAsync(RotacaoRefreshTokenHash rotacao) => Task.FromResult(true);
        public Task<bool> RedefinirRefreshTokenRepositoryAsync(string codigoUsuario) => Task.FromResult(true);
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
        public EmailRequest? UltimoRequest { get; private set; }
        public int Envios { get; private set; }

        public Task<Resultado> EnviarAsync(EmailRequest message)
        {
            Envios++;
            UltimoRequest = message;
            return Task.FromResult(resultado ?? Resultado.Sucesso());
        }
    }

    private sealed class CacheServiceFake : ICacheService
    {
        private readonly Dictionary<string, object> _itens = new(StringComparer.Ordinal);

        public void GravarCache<T>(CacheInfo<T> cacheInfo)
            => _itens[cacheInfo.KeyDescription] = cacheInfo.EntityInfo!;

        public void GravarCache<T>(CacheInfo<T> cacheInfo, TimeSpan expiracao)
            => GravarCache(cacheInfo);

        public T ObterCache<T>(ChaveCache chaveCache)
            => _itens.TryGetValue(chaveCache.Descricao, out var valor) ? (T)valor : default;

        public void RemoverCache(ChaveCache chaveCache)
            => _itens.Remove(chaveCache.Descricao);
    }

    private sealed class ConfirmacaoEmailRepositoryFake : IConfirmacaoEmailRepository
    {
        public ConfirmacaoEmail UltimaConfirmacaoGravada { get; private set; }
        public int Gravacoes { get; private set; }

        public Task<bool> GravarOuSubstituirAsync(ConfirmacaoEmail confirmacaoEmail)
        {
            Gravacoes++;
            UltimaConfirmacaoGravada = confirmacaoEmail;
            return Task.FromResult(true);
        }
        public Task<ConfirmacaoEmail> ObterAtivaPorUsuarioAsync(string usuarioCodigo)
            => Task.FromResult(UltimaConfirmacaoGravada);
        public Task<bool> MarcarConfirmadaAsync(int id) => Task.FromResult(true);
        public Task RegistrarTentativaFalhaAsync(int id)
        {
            UltimaConfirmacaoGravada.TentativasFalhas++;
            return Task.CompletedTask;
        }
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
        public Task<bool> ValidarCredenciaisAsync(string codigoUsuario, string senha) => Task.FromResult(true);
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

    private sealed class EnderecoFrontendServiceFake : IEnderecoFrontendService
    {
        public string ObterUriBase() => "https://front.atron.test";
    }
}
