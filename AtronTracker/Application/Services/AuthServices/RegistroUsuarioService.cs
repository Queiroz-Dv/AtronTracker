using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;
using System.Web;

namespace Application.Services.AuthServices
{
    public class RegistroUsuarioService : IRegistroUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILoginRepository _loginRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IEmailService _emailService;
        private readonly IPerfilDeAcessoRepository _perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IValidador<UsuarioRegistroRequest> _validador;

        public RegistroUsuarioService(
            IAccessorService accessor,
            IUsuarioRepository usuarioRepository,
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService,
            IValidador<UsuarioRegistroRequest> validador,
            IHttpContextAccessor httpContextAccessor,
            ILoginRepository loginRepository)
        {
            _usuarioRepository = usuarioRepository;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
            _validador = validador;
            _httpContextAccessor = httpContextAccessor;
            _loginRepository = loginRepository;
        }

        public async Task<Resultado> RegistrarUsuario(UsuarioRegistroRequest request)
        {
            var notificacoes = _validador.Validar(request);
            if (notificacoes.TemErros()) return Resultado.Falha(notificacoes);

            var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(request.Codigo, request.Email);
            if (contaExiste) return Resultado.Falha("Usuário já cadastrado.");

            var registrado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(request.Codigo, request.Email, request.Senha);
            if (!registrado) return Resultado.Falha("Erro na gravação da conta.");

            var usuario = new Usuario(request.Codigo,
                                      request.Nome,
                                      request.Sobrenome,
                                      request.Email,
                                      request.DataNascimento?.ToDateTime(TimeOnly.MinValue));

            var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!usuarioGravado) return Resultado.Falha("Erro ao salvar usuário.");

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            var perfilDeAcesso = await _perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(request.CodigoPerfilDeAcesso);

            if (perfilDeAcesso != null)
            {
                await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(new PerfilDeAcessoUsuario
                {
                    PerfilDeAcessoId = perfilDeAcesso.Id,
                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                    UsuarioId = usuarioBd.Id,
                    UsuarioCodigo = usuarioBd.Codigo
                });
            }

            string link = await ObterUrlDeConfirmacao(request.ClientUri, request.Codigo);

            try
            {
                await _emailService.EnviarAsync(new EmailRequest
                {
                    Assunto = "Confirme seu cadastro - AtronTracker",
                    Mensagem = CorpoDoEmailDeCadastro(usuario, link),
                    EmailsDestino = [request.Email]
                });
            }
            catch { }

            return Resultado.Sucesso($"Usuário {usuario.Nome} {usuario.Sobrenome}: cadastro realizado com sucesso! Verifique seu e-mail para confirmar.");
        }

        public async Task<Resultado> TrocarSenha(LoginRequestDTO dto)
        {
            var resultado = await _loginRepository.AtualizarSenhaUsuario(dto.CodigoDoUsuario, dto.Senha);
            return resultado ? Resultado.Sucesso() : Resultado.Falha("Erro ao atualizar a senha");
        }

        private async Task<string> ObterUrlDeConfirmacao(string uri, string codigoUsuario)
        {
            var token = await _usuarioIdentityRepository.GerarTokenConfirmacaoEmailAsync(codigoUsuario);
            var tokenEncoded = HttpUtility.UrlEncode(token); // <-- importante
            var baseUri = ObterUri(uri);
            return $"{baseUri}/confirmar-email?usuarioCodigo={codigoUsuario}&token={token}";
        }

        private string ObterUri(string uri)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var uriContext = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}";

            return !string.IsNullOrEmpty(uri) ? uri : uriContext;
        }

        public async Task<Resultado> ConfirmarEmail(string codigoUsuario, string token)
        {
            var resultado = await _usuarioIdentityRepository.ConfirmarEmailAsync(codigoUsuario, token);

            if (!resultado)
                return Resultado.Falha("Falha ao confirmar e-mail. Token inválido ou expirado.");

            // Se a confirmação foi bem sucedida, tentar enviar e-mail de notificação ao usuário
            try
            {
                // Obter e-mail do usuário pelo código (não depende do request)
                var usuario = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
                if (usuario != null && !string.IsNullOrEmpty(usuario.Email))
                {
                    var assunto = "E-mail confirmado - AtronTracker";
                    var mensagem = CorpoEmailConfirmacaoSucesso(usuario.Nome);

                    // Envio best-effort: não interrompe o fluxo se falhar
                    await _emailService.EnviarAsync(new EmailRequest
                    {
                        Assunto = assunto,
                        Mensagem = mensagem,
                        EmailsDestino = [usuario.Email]
                    });
                }
            }
            catch
            {
                // Log opcional aqui (não interrompe o fluxo)
            }

            return Resultado.Sucesso("E-mail confirmado com sucesso!");
        }

        private static string CorpoEmailConfirmacaoSucesso(string nomeUsuario)
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
                            .header h1 {{ color: #007bff; margin: 0; }}
                            .content {{ padding: 20px 0; }}
                            .content p {{ color: #333; line-height: 1.6; }}
                            .footer {{ text-align: center; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 12px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>✔️ E-mail confirmado</h1>
                            </div>
                            <div class='content'>
                                <p>Olá, <strong>{nomeUsuario}</strong>!</p>
                                <p>Seu e-mail foi confirmado com sucesso. Agora você pode acessar sua conta normalmente.</p>
                                <p>Se você não realizou essa ação, entre em contato com o suporte imediatamente.</p>
                            </div>
                            <div class='footer'>
                                <p>Este é um e-mail automático. Por favor, não responda.</p>
                                <p>&copy; {DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
                            </div>
                        </div>
                    </body>
                    </html>";
        }

        private static string CorpoDoEmailDeCadastro(Usuario usuario, string link)
        {
            return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h1 style='color: #2c3e50;'>Bem-vindo(a) ao Atron!</h1>
                    <p>Olá, <strong>{usuario.Nome}</strong>!</p>
                    <p>Seu cadastro foi recebido. Para confirmar seu e-mail, clique no botão abaixo:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Confirmar meu E-mail</a>
                    </div>
                    <p style='font-size: 12px; color: #999; word-break: break-all;'>{link}</p>
                    <p style='font-size: 12px; color: #aaa;'>Se você não criou esta conta, ignore este e-mail.</p>
                </div>";
        }
    }
}