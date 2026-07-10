using Domain.Entities;
using Application.Extensions;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;
using UsuarioEntity = Domain.Entities.Usuario;
using Shared.Application.Resources;

namespace Application.UseCases.Usuario
{
    public class ReenviarConfirmacaoEmail
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IConfirmacaoEmailRepository _confirmacaoEmailRepository;
        private readonly IConfirmacaoEmailCodigoService _confirmacaoEmailCodigoService;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int ValidadeConfirmacaoEmailEmHoras = 24;

        public ReenviarConfirmacaoEmail(
            IUsuarioRepository usuarioRepository,
            IConfirmacaoEmailRepository confirmacaoEmailRepository,
            IConfirmacaoEmailCodigoService confirmacaoEmailCodigoService,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository;
            _confirmacaoEmailRepository = confirmacaoEmailRepository;
            _confirmacaoEmailCodigoService = confirmacaoEmailCodigoService;
            _emailService = emailService;
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
                var resultado = await _emailService.EnviarAsync(new EmailRequest
                {
                    Assunto = EmailResource.Assunto_ConfirmeCadastro,
                    Mensagem = CorpoDoEmail(usuario.Nome, link, confirmacao.Identificador),
                    EmailsDestino = [usuario.Email]
                });

                return resultado.TeveFalha
                    ? Resultado.Falha(resultado.Messages)
                    : Resultado.Sucesso(EmailResource.Mensagem_EnvioConfirmacaoEmail);
            }
            catch (Exception ex)
            {
                return Resultado.Falha(string.Format(EmailResource.Erro_ExcesaoEnvioEmail, ex.Message));
            }
        }

        private string ObterUri(string uri)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriContext = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            return !string.IsNullOrEmpty(uri) ? uri.TrimEnd('/') : uriContext;
        }

        private static string CorpoDoEmail(string nomeUsuario, string link, string identificador)
        {
            return $@"
                        <!DOCTYPE html>
                        <html>
                        <head>
                          <meta charset='utf-8'>
                          <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f4f4f4; }}
                            .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; padding: 30px; border-radius: 8px; box-shadow: 0 2px 4px rgba(0,0,0,0.1); }}
                            .header {{ text-align: center; padding-bottom: 20px; border-bottom: 2px solid #007bff; }}
                            .header h1 {{ color: #2c3e50; margin: 0; font-size: 22px; }}
                            .content {{ padding: 20px 0; }}
                            .content p {{ color: #333; line-height: 1.6; margin: 0 0 12px 0; }}
                            .code {{
                              color: #1f2937;
                              font-size: 28px;
                              font-weight: 700;
                              letter-spacing: 6px;
                              margin: 24px 0;
                              text-align: center;
                            }}
                            .cta {{ text-align: center; margin: 30px 0; }}
                            .btn {{
                              display: inline-block;
                              background: linear-gradient(180deg, #2b8cff 0%, #0066d6 100%);
                              color: #ffffff !important;
                              padding: 14px 26px;
                              text-decoration: none;
                              border-radius: 8px;
                              font-weight: 700;
                              font-size: 15px;
                            }}
                            .link-text {{ font-size: 12px; color: #999; word-break: break-all; margin-top: 8px; }}
                            .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
                          </style>
                        </head>
                        <body>
                          <div class='container'>
                            <div class='header'>
                              <h1>Confirme seu e-mail</h1>
                            </div>

                            <div class='content'>
                              <p>Ola, <strong>{nomeUsuario}</strong>!</p>
                              <p>Use o codigo abaixo para confirmar seu acesso ao Atron:</p>
                              <p class='code'>{identificador}</p>

                              <div class='cta'>
                                <a href='{link}' class='btn' target='_blank' rel='noopener noreferrer'>Informar codigo</a>
                              </div>

                              <p class='link-text'>Se o botao nao funcionar, copie e cole este link no navegador:<br>{link}</p>
                              <p class='link-text'>Este codigo expira em {ValidadeConfirmacaoEmailEmHoras} horas.</p>
                              <p style='font-size: 12px; color: #aaa;'>Se voce nao solicitou este reenvio, ignore este e-mail.</p>
                            </div>

                            <div class='footer'>
                              <p>Este e um e-mail automatico. Por favor, nao responda.</p>
                              <p>&copy; {DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
                            </div>
                          </div>
                        </body>
                        </html>";
        }
    }
}
