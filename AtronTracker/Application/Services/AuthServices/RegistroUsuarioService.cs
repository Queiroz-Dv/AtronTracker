using Application.DTO.Request;
using Application.Interfaces.ApplicationInterfaces;
using Domain.Entities;
using Domain.Interfaces;
using Domain.Interfaces.ApplicationInterfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;
using System.Web;
using Application.Extensions;

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
        private readonly ICacheService _cacheService;

        public RegistroUsuarioService(
            IAccessorService accessor,
            IUsuarioRepository usuarioRepository,
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IPerfilDeAcessoRepository perfilDeAcessoRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IEmailService emailService,
            IValidador<UsuarioRegistroRequest> validador,
            IHttpContextAccessor httpContextAccessor,
            ILoginRepository loginRepository,
            ICacheService cacheService)
        {
            _usuarioRepository = usuarioRepository;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            _perfilDeAcessoRepository = perfilDeAcessoRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _emailService = emailService;
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

        public async Task<Resultado> TrocarSenha(RedefinirSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.IdentificadorTemporario))
                return Resultado.Falha("Identificador temporário não informado.");

            // Buscar DadosTemporarios no cache usando o identificador
            var cacheKey = $"{ECacheKeysInfo.DadosTemporarios.GetDescription()}:{request.IdentificadorTemporario}";
            var dadosTemporarios = _cacheService.ObterCache<DadosTemporarios>(cacheKey);

            if (dadosTemporarios == null)
                return Resultado.Falha("Solicitação expirada ou inválida. Solicite uma nova recuperação de senha.");

            var novaSenha = CryptoHelper.DecryptCryptoJsAes(request.NovaSenha);
            var repetirSenha = CryptoHelper.DecryptCryptoJsAes(request.RepetirSenha);

            if (string.IsNullOrEmpty(novaSenha) || string.IsNullOrEmpty(repetirSenha))
                return Resultado.Falha("Senha inválida ou falha na descriptografia da requisição.");

            if (novaSenha != repetirSenha)
                return Resultado.Falha("As senhas informadas não coincidem.");

            // Extrair dados reais do cache (UsuarioCodigo e Token)
            var usuarioCodigo = dadosTemporarios.UsuarioCodigo;
            var token = dadosTemporarios.Token;

            var resultado = await _usuarioIdentityRepository.RedefinirSenhaAsync(usuarioCodigo, token, novaSenha);
            if (resultado)
            {
                // Tenta atualizar no repository de login se necessário
                var atualizouLogin = await _loginRepository.AtualizarSenhaUsuario(usuarioCodigo, novaSenha);

                // Remover dados temporários do cache após sucesso (evitar reutilização)
                _cacheService.RemoverCache(ECacheKeysInfo.DadosTemporarios, request.IdentificadorTemporario);

                return Resultado.Sucesso("Senha alterada com sucesso.");
            }

            return Resultado.Falha("Erro ao atualizar a senha. Token inválido ou expirado.");
        }

        public async Task<Resultado> SolicitarRecuperacaoSenha(SolicitarRecuperacaoSenhaRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Identificador)) return Resultado.Falha("Identificador não informado.");

            Usuario usuario = null;
            // Se tiver mais de 3 chars, regra dictamina como E-mail. Também vamos checar pelo '@'.
            if (request.Identificador.Length > 3 || request.Identificador.Contains('@'))
            {
                usuario = await _usuarioRepository.ObterUsuarioGeralPorEmailAsync(request.Identificador);
            }
            else
            {
                usuario = await _usuarioRepository.ObterUsuarioGeralPorCodigoAsync(request.Identificador);
            }

            if (usuario == null)
                return Resultado.Falha("Usuário não encontrado com o identificador fornecido.");

            if (usuario.Inativo)
                return Resultado.Falha("Usuário inativo. Solicite a um superior ou suporte a reativação desse usuário.");

            var token = await _usuarioIdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo);

            // Gerar identificador temporário de 9 dígitos com código do usuário embutido
            var identificadorTemporario = CryptoHelper.GerarIdentificadorTemporario(usuario.Codigo);

            // Criar snapshot dos dados temporários
            var dadosTemporarios = new DadosTemporarios
            {
                IdentificadorTemporario = identificadorTemporario,
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = token,
                DataAlteracaoSenha = DateTime.UtcNow
            };

            // Gravar no cache com TTL de 15 minutos
            var cacheInfo = new CacheInfo<DadosTemporarios>(ECacheKeysInfo.DadosTemporarios, identificadorTemporario)
            {
                EntityInfo = dadosTemporarios
            };
            _cacheService.GravarCache(cacheInfo, TimeSpan.FromMinutes(15));

            // Criptografar o identificador para o link (não expor dados na URL)
            var identificadorCriptografado = CryptoHelper.EncryptCryptoJsAes(identificadorTemporario);
            var identificadorUrlEncoded = HttpUtility.UrlEncode(identificadorCriptografado);

            var baseUri = ObterUri(request.ClientUri);
            var link = $"{baseUri}/trocar-senha?id={identificadorUrlEncoded}";

            try
            {
                 await _emailService.EnviarAsync(new EmailRequest
                 {
                     Assunto = "Recuperação de Senha - AtronTracker",
                     Mensagem = CorpoDoEmailRecuperacaoSenha(usuario.Nome, link),
                     EmailsDestino = [usuario.Email]
                 });
            }
            catch { }

            return Resultado.Sucesso("Se o identificador existir em nossa base, um e-mail com as instruções de recuperação foi enviado.");
        }

        private static string CorpoDoEmailRecuperacaoSenha(string nome, string link)
        {
             return $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #e0e0e0; border-radius: 10px;'>
                    <h1 style='color: #2c3e50;'>Recuperação de Senha</h1>
                    <p>Olá, <strong>{nome}</strong>!</p>
                    <p>Recebemos uma solicitação para redefinir a senha da sua conta no Atron.</p>
                    <p>Para criar uma nova senha, clique no botão abaixo. Este link expira em 15 minutos:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Redefinir Minha Senha</a>
                    </div>
                    <p style='font-size: 12px; color: #999; word-break: break-all;'>Se o botão não funcionar, copie e cole este link no navegador: <br>{link}</p>
                    <p style='font-size: 12px; color: #aaa;'>Se você não solicitou a alteração de senha, pode ignorar e excluir este e-mail com segurança.</p>
                </div>";
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