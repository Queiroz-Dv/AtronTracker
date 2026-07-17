using Application.Email.Compositores;
using Application.Email.Models;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;
using UsuarioEntity = Domain.Entities.Usuario;

namespace Application.UseCases.Usuario
{
    public class ReenviarConfirmacaoEmail
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfirmacaoEmailRepository _confirmacaoEmailRepository;
        private readonly IConfirmacaoEmailCodigoService _confirmacaoEmailCodigoService;
        private readonly IEmailService _emailService;
        private readonly IAcessoEmailCompositor _emailCompositor;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int ValidadeConfirmacaoEmailEmHoras = 24;

        public ReenviarConfirmacaoEmail(
            IUsuarioRepository usuarioRepository,
            IConfirmacaoEmailRepository confirmacaoEmailRepository,
            IConfirmacaoEmailCodigoService confirmacaoEmailCodigoService,
            IEmailService emailService,
            IAcessoEmailCompositor emailCompositor,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository;
            _confirmacaoEmailRepository = confirmacaoEmailRepository;
            _confirmacaoEmailCodigoService = confirmacaoEmailCodigoService;
            _emailService = emailService;
            _emailCompositor = emailCompositor;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string clientUri)
        {
            var codigoNormalizado = codigoUsuario.NormalizeUserCodeIdentifier();
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoNormalizado);
            return await ReenviarAsync(usuario, clientUri);
        }

        public async Task<Resultado> ExecutarPorIdentificadorAsync(string identificador, string clientUri)
        {
            var identificadorNormalizado = identificador.NormalizeIdentifier();
            if (string.IsNullOrWhiteSpace(identificadorNormalizado))
                return Resultado.Falha(EmailResource.Erro_InformeEmailCodigo);

            var usuario = identificadorNormalizado.IdentifierIsEmail()
                ? await _usuarioRepository.ObterUsuarioGeralPorEmailAsync(identificadorNormalizado)
                : await _usuarioRepository.ObterUsuarioGeralPorCodigoAsync(identificadorNormalizado.NormalizeUserCodeIdentifier());

            return await ReenviarAsync(usuario, clientUri);
        }

        private async Task<Resultado> ReenviarAsync(UsuarioEntity usuario, string clientUri)
        {
            if (usuario == null)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioNaoEncontrado);

            if (usuario.Inativo)
                return Resultado.Falha(UsuarioResource.Erro_UsuarioInativo);

            if (usuario.EmailConfirmado)
                return Resultado.Falha(EmailResource.Erro_EmailConfirmado);

            var confirmacao = _confirmacaoEmailCodigoService.CriarDadosConfirmacao(usuario.Codigo, ValidadeConfirmacaoEmailEmHoras);
            var gravado = await _confirmacaoEmailRepository.GravarOuSubstituirAsync(confirmacao.ConfirmacaoEmail);
            if (!gravado)
                return Resultado.Falha(EmailResource.Erro_CriarCodigoDeConfirmacao);

            var baseUri = ObterUri(clientUri);
            var link = $"{baseUri}/confirmar-email?usuarioCodigo={usuario.Codigo}";

            try
            {
                var email = _emailCompositor.ComporConfirmacaoCadastro(new ConfirmacaoCadastroEmailParametros(
                    usuario.Email,
                    usuario.Nome,
                    confirmacao.Identificador,
                    link,
                    ValidadeConfirmacaoEmailEmHoras));
                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                var resultado = await _emailService.EnviarAsync(email.Dados);

                return resultado.TeveFalha
                    ? Resultado.Falha(resultado.Messages)
                    : Resultado.Sucesso(EmailResource.Mensagem_EnvioConfirmacaoEmail);
            }
            catch
            {
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }
        }

        private string ObterUri(string uri)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriContext = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            return !string.IsNullOrEmpty(uri) ? uri.TrimEnd('/') : uriContext;
        }

    }
}