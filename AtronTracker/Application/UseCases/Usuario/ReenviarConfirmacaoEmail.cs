using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    public class ReenviarConfirmacaoEmail
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ReenviarConfirmacaoEmail(
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService,
            IHttpContextAccessor httpContextAccessor)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Resultado> ExecutarAsync(string codigoUsuario, string clientUri)
        {
            var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            if (usuario == null)
                return Resultado.Falha("Usuário não encontrado.");

            var emailJaConfirmado = await _usuarioIdentityRepository.EmailConfirmadoAsync(codigoUsuario);
            if (emailJaConfirmado)
                return Resultado.Falha("O e-mail desta conta já foi confirmado.");

            var token = await _usuarioIdentityRepository.GerarTokenConfirmacaoEmailAsync(codigoUsuario);

            var baseUri = ObterUri(clientUri);
            var link = $"{baseUri}/confirmar-email?usuarioCodigo={codigoUsuario}&token={token}";

            try
            {
                var resultado = await _emailService.EnviarAsync(new EmailRequest
                {
                    Assunto = "Confirme seu cadastro - AtronTracker",
                    Mensagem = CorpoDoEmail(usuario.Nome, link),
                    EmailsDestino = [usuario.Email]
                });
                return resultado;
            }
            catch { }

            return Resultado.Sucesso("E-mail de confirmação reenviado. Verifique sua caixa de entrada.");
        }

        private string ObterUri(string uri)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriContext = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";
            return !string.IsNullOrEmpty(uri) ? uri : uriContext;
        }

        private static string CorpoDoEmail(string nomeUsuario, string link)
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
                            .cta {{ text-align: center; margin: 30px 0; }}
                            .link-text {{ font-size: 12px; color: #999; word-break: break-all; margin-top: 8px; }}
                            .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
                            /* Button style for modern clients */
                            .btn {{
                              display: inline-block;
                              background: linear-gradient(180deg, #2b8cff 0%, #0066d6 100%);
                              color: #ffffff !important;
                              padding: 14px 26px;
                              text-decoration: none;
                              border-radius: 8px;
                              font-weight: 700;
                              box-shadow: 0 6px 18px rgba(43,140,255,0.18), inset 0 -2px 0 rgba(0,0,0,0.06);
                              border: 1px solid rgba(0,0,0,0.06);
                              font-size: 15px;
                              letter-spacing: 0.2px;
                            }}
                            @media only screen and (max-width: 480px) {{
                              .container {{ padding: 20px; border-radius: 6px; }}
                              .btn {{ width: 100% !important; box-sizing: border-box; padding: 14px 18px; }}
                            }}
                          </style>
                        </head>
                        <body>
                          <div class='container'>
                            <div class='header'>
                              <h1>🔒 Confirme seu E-mail</h1>
                            </div>

                            <div class='content'>
                              <p>Olá, <strong>{nomeUsuario}</strong>!</p>
                              <p>Você solicitou o reenvio do e-mail de confirmação. Clique no botão abaixo para confirmar sua conta:</p>

                              <div class='cta'>
                                <!-- Outlook VML button for compatibility -->
                                <!--[if mso]>
                                <v:roundrect xmlns:v='urn:schemas-microsoft-com:vml' xmlns:w='urn:schemas-microsoft-com:office:word' href='{link}' style='height:48px;v-text-anchor:middle;width:260px;' arcsize='8%' strokecolor='#0066d6' fillcolor='#007bff'>
                                  <w:anchorlock/>
                                  <center style='color:#ffffff;font-family:Arial,sans-serif;font-size:15px;font-weight:700;'>Confirmar meu E-mail</center>
                                </v:roundrect>
                                <![endif]-->

                                <!-- Modern clients -->
                                <a href='{link}' class='btn' target='_blank' rel='noopener noreferrer' aria-label='Confirmar e-mail para {nomeUsuario}'>Confirmar meu E-mail</a>
                              </div>

                              <!-- Mostrar a URL completa em texto (para clientes que não suportam botão) -->
                              
                              <p style='font-size: 12px; color: #aaa;'>Se você não solicitou este reenvio, ignore este e-mail.</p>
                            </div>

                            <div class='footer'>
                              <p>Este é um e-mail automático. Por favor, não responda.</p>
                              <p>&copy; {DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
                            </div>
                          </div>
                        </body>
                        </html>";
        }
    }
}