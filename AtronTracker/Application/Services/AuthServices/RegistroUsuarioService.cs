using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Email.Models;
using Application.Interfaces.ApplicationInterfaces;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Application.Extensions;
using Shared.Application.Resources;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService : IRegistroUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfirmacaoEmailRepository _confirmacaoEmailRepository;
        private readonly IConfirmacaoEmailCodigoService _confirmacaoEmailCodigoService;
        private readonly ILoginRepository _loginRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IEmailService _emailService;
        private readonly IAcessoEmailCompositor _emailCompositor;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidador<UsuarioRegistroRequest> _validador;
        private readonly ICacheService _cacheService;
        private const int ValidadeRecuperacaoSenhaEmHoras = 24;

        public RegistroUsuarioService(
            IUsuarioRepository usuarioRepository,
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService,
            IAcessoEmailCompositor emailCompositor,
            IValidador<UsuarioRegistroRequest> validador,
            IHttpContextAccessor httpContextAccessor,
            ILoginRepository loginRepository,
            ICacheService cacheService,
            IConfirmacaoEmailRepository confirmacaoEmailRepository,
            IConfirmacaoEmailCodigoService confirmacaoEmailCodigoService)
        {
            _usuarioRepository = usuarioRepository;
            _confirmacaoEmailRepository = confirmacaoEmailRepository;
            _confirmacaoEmailCodigoService = confirmacaoEmailCodigoService;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
            _emailCompositor = emailCompositor;
            _validador = validador;
            _httpContextAccessor = httpContextAccessor;
            _loginRepository = loginRepository;
            _cacheService = cacheService;
        }

        public async Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest request)
        {
            var notificacoes = _validador.Validar(request);
            if (notificacoes.TemErros()) return Resultado.Falha(notificacoes);

            var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(request.Codigo, request.Email);
            if (contaExiste) return Resultado.Falha(UsuarioResource.ErroUsuarioExistente);

            var registrado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha);
            if (!registrado) return Resultado.Falha(AuthResource.Erro_GravacaoConta);

            var usuario = new Usuario(request.Codigo,
                                      request.Nome,
                                      request.Sobrenome,
                                      request.Email,
                                      request.DataNascimento?.ToDateTime(TimeOnly.MinValue));

            var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!usuarioGravado) return Resultado.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            var perfilDeAcesso = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(request.CodigoPerfilDeAcesso);

            if (perfilDeAcesso != null)
            {
                await _perfilDeAcessoUsuarioRepository.CriarRelacionamentoRepositoryAsync(new PerfilDeAcessoUsuario
                {
                    PerfilDeAcessoId = perfilDeAcesso.Id,
                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                    UsuarioId = usuarioBd.Id,
                    UsuarioCodigo = usuarioBd.Codigo
                });
            }

            var confirmacao = await ObterDadosConfirmacaoEmail(request.ClientUri, usuarioBd.Codigo);
            if (!confirmacao.Gravado)
                return Resultado.Falha(AuthResource.Erro_GerarCodigoConfirmacao);

            var resultado = Resultado.Sucesso(
                string.Format(AuthResource.Mensagem_UsuarioRegistrado, usuario.Nome, usuario.Sobrenome));

            try
            {
                var email = _emailCompositor.ComporConfirmacaoCadastro(new ConfirmacaoCadastroEmailParametros(
                    request.Email,
                    usuario.Nome,
                    confirmacao.Identificador,
                    confirmacao.Link,
                    ValidadeRecuperacaoSenhaEmHoras));
                if (email.TeveFalha)
                    resultado.AdicionarAviso(string.Join(" | ", email.Messages.Select(mensagem => mensagem.Descricao)));
                else if ((await _emailService.EnviarAsync(email.Dados)).TeveFalha)
                    resultado.AdicionarAviso(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
            }
            catch
            {
                resultado.AdicionarAviso(AuthResource.Aviso_CadastroCriadoEmailNaoEnviado);
            }

            return resultado;
        }

        public async Task<Resultado> TrocarSenha(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdentificadorTemporario))
                return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

            var cacheKey = $"{ECacheKeysInfo.DadosTemporarios.GetDescription()}:{request.IdentificadorTemporario}";
            var dadosTemporarios = _cacheService.ObterCache<DadosTemporarios>(cacheKey);

            if (dadosTemporarios == null)
                return Resultado.Falha(AuthResource.Erro_CacheExpiradoNaTrocaDeSenha);

            var novaSenha = CryptoHelper.DecryptCryptoJsAes(request.NovaSenha);
            var repetirSenha = CryptoHelper.DecryptCryptoJsAes(request.RepetirSenha);

            if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(repetirSenha))
                return Resultado.Falha(AuthResource.Erro_SenhaInvalida);

            if (novaSenha != repetirSenha)
                return Resultado.Falha(AuthResource.Erro_SenhasDivergentes);

            var usuarioCodigo = dadosTemporarios.UsuarioCodigo;
            var token = dadosTemporarios.Token;

            var resultado = await _usuarioIdentityRepository.RedefinirSenhaAsync(usuarioCodigo, token, novaSenha);
            if (resultado)
            {
                var atualizouLogin = await _loginRepository.AtualizarSenhaUsuario(usuarioCodigo, novaSenha);              

                _cacheService.RemoverCache(ECacheKeysInfo.DadosTemporarios, request.IdentificadorTemporario);

                return Resultado.Sucesso(AuthResource.Mensagem_SenhaAlterada);
            }

            return Resultado.Falha(AuthResource.Erro_AtualizarSenha);
        }

        public async Task<Resultado> SolicitarRecuperacaoSenha(SolicitarRecuperacaoSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identificador)) return Resultado.Falha(AuthResource.Erro_IdentificadorTemporario);

            var identificador = request.Identificador.NormalizeIdentifier();
            var usuario = identificador.IdentifierIsEmail()
                ? await _usuarioRepository.ObterUsuarioGeralPorEmailAsync(identificador)
                : await _usuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificador.NormalizeUserCodeIdentifier());

            if (usuario == null)
                return Resultado.Falha(AuthResource.Erro_UsuarioNaoEncontrado);

            if (usuario.Inativo)
                return Resultado.Falha(AuthResource.Erro_UsuarioInativo);

            var token = await _usuarioIdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo);

            var identificadorTemporario = CryptoHelper.GerarIdentificadorTemporario(usuario.Codigo);

            var dadosTemporarios = new DadosTemporarios
            {
                IdentificadorTemporario = identificadorTemporario,
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = token,
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var cacheInfo = new CacheInfo<DadosTemporarios>(ECacheKeysInfo.DadosTemporarios, identificadorTemporario)
            { EntityInfo = dadosTemporarios };

            _cacheService.GravarCache(cacheInfo, TimeSpan.FromHours(ValidadeRecuperacaoSenhaEmHoras));

            var identificadorCriptografado = CryptoHelper.EncryptCryptoJsAes(identificadorTemporario);
            var identificadorUrlEncoded = HttpUtility.UrlEncode(identificadorCriptografado);

            var baseUri = ObterUri(request.ClientUri);
            var link = $"{baseUri}/trocar-senha?id={identificadorUrlEncoded}";

            Resultado resultadoEnvio;
            try
            {
                var email = _emailCompositor.ComporRecuperacaoSenha(new RecuperacaoSenhaEmailParametros(
                    usuario.Email,
                    usuario.Nome,
                    link,
                    ValidadeRecuperacaoSenhaEmHoras));
                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                resultadoEnvio = await _emailService.EnviarAsync(email.Dados);
            }
            catch
            {
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }

            if (resultadoEnvio.TeveFalha)
                return Resultado.Falha(resultadoEnvio.Messages);

            return Resultado.Sucesso();
        }

        private async Task<(string Link, string Identificador, bool Gravado)> ObterDadosConfirmacaoEmail(string uri, string codigoUsuario)
        {
            var confirmacao = _confirmacaoEmailCodigoService.CriarDadosConfirmacao(codigoUsuario, ValidadeRecuperacaoSenhaEmHoras);
            var gravado = await _confirmacaoEmailRepository.GravarOuSubstituirAsync(confirmacao.ConfirmacaoEmail);

            var baseUri = ObterUri(uri);
            return ($"{baseUri}/confirmar-email?usuarioCodigo={codigoUsuario}", confirmacao.Identificador, gravado);
        }

        private string ObterUri(string uri)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriContext = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

            return !string.IsNullOrEmpty(uri) ? uri : uriContext;
        }

        public async Task<Resultado> ConfirmarEmail(string codigoUsuario, string identificador)
        {
            var codigoNormalizado = codigoUsuario.NormalizeUserCodeIdentifier();
            var identificadorNormalizado = identificador.NormalizeIdentifier();

            if (string.IsNullOrWhiteSpace(codigoNormalizado) || string.IsNullOrWhiteSpace(identificadorNormalizado))
                return Resultado.Falha(AuthResource.Erro_DadosConfirmacaoObrigatorios);

            var confirmacaoEmail = await _confirmacaoEmailRepository.ObterAtivaPorUsuarioAsync(codigoNormalizado);
            if (confirmacaoEmail is null)
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            if (!_confirmacaoEmailCodigoService.ConfirmacaoValida(confirmacaoEmail, codigoNormalizado, identificadorNormalizado))
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            var confirmado = await _usuarioRepository.ConfirmarEmailAsync(codigoNormalizado);
            if (!confirmado)
                return Resultado.Falha(AuthResource.Erro_FalhaConfirmarEmail);

            await _confirmacaoEmailRepository.MarcarConfirmadaAsync(confirmacaoEmail.Id);

            var resultado = Resultado.Sucesso(AuthResource.Mensagem_EmailConfirmado);
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoNormalizado);
            if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
            {
                try
                {
                    var email = _emailCompositor.ComporConfirmacaoConcluida(usuario.Email, usuario.Nome);
                    if (email.TeveFalha)
                        resultado.AdicionarAviso(string.Join(" | ", email.Messages.Select(mensagem => mensagem.Descricao)));
                    else if ((await _emailService.EnviarAsync(email.Dados)).TeveFalha)
                        resultado.AdicionarAviso(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
                }
                catch
                {
                    resultado.AdicionarAviso(AuthResource.Aviso_ConfirmacaoConcluidaEmailNaoEnviado);
                }
            }

            return resultado;
        }
    }
}
