using Application.DTO.Request;
using Application.Extensions;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Application.UseCases.Usuario
{
    public class CriarUsuario
    {
        private readonly IValidador<UsuarioRequest> _validador;
        private readonly IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> _mapService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository;
        private readonly IEmailService _emailService;
        private readonly ICacheService _cacheService;
        private readonly IAuditoriaService _auditoriaService;

        private const string UsuarioContexto = "Usuario";
        private const int ValidadeConvitePrimeiroAcessoEmHoras = 24;

        public CriarUsuario(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IEmailService emailService,
            ICacheService cacheService,
            IAuditoriaService auditoriaService)
        {
            _validador = validador;
            _mapService = mapService;
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _emailService = emailService;
            _cacheService = cacheService;
            _auditoriaService = auditoriaService;
        }

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            var codigoUsuario = request.Codigo.ToUpper();
            var usuarioExistente = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoUsuario);
            if (usuarioExistente != null)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);

            if (!request.Email.IsNullOrEmpty())
            {
                var emailExiste = await _usuarioRepository.VerificarEmailExistenteAsync(request.Email);
                if (emailExiste)
                    return Resultado<UsuarioRequest>.Falha(EmailResource.ErroEmailUtilizado);
            }

            var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(codigoUsuario, request.Email);
            if (contaExiste)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);

            var usuario = await _mapService.MapToEntityAsync(request);
            var resultadoGestor = await VincularGestorImediatoAsync(usuario, request.GestorImediatoCodigo);
            if (resultadoGestor.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(resultadoGestor.Messages);

            var criado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!criado)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            var identityCriado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(
                codigoUsuario,
                request.Email,
                GerarSenhaTemporaria());

            if (!identityCriado)
            {
                await _usuarioRepository.RemoverUsuarioAsync(usuarioBd);
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);
            }

            var conviteEnviado = await EnviarEmailPrimeiroAcessoAsync(usuarioBd, request.ClientUri);
            if (conviteEnviado.TeveFalha)
            {
                await _usuarioIdentityRepository.DeletarContaUserRepositoryAsync(usuarioBd.Codigo);
                await _usuarioRepository.RemoverUsuarioAsync(usuarioBd);
                return Resultado<UsuarioRequest>.Falhas(conviteEnviado.Messages);
            }

            if (!request.DepartamentoCodigo.IsNullOrEmpty() && !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository
                    .ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(request.DepartamentoCodigo);
                var cargo = await _cargoRepository.ObterCargoPorCodigoAsync(request.CargoCodigo);

                if (departamento != null && cargo != null)
                {
                    await _usuarioCargoDepartamentoRepository
                        .GravarAssociacaoUsuarioCargoDepartamento(usuarioBd, cargo, departamento);
                }
            }

            await _auditoriaService.RegistrarServiceAsync(new AuditoriaDTO
            {
                CodigoRegistro = usuarioBd.Codigo,
                Contexto = UsuarioContexto,
                Historico = new HistoricoDTO
                {
                    CodigoRegistro = usuarioBd.Codigo,
                    Contexto = UsuarioContexto,
                    Descricao = $"Usuario {usuarioBd.Codigo} criado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });

            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem($"Usuario {request.Nome} {request.Sobrenome} salvo com sucesso. O link de primeiro acesso foi enviado por e-mail.");
        }

        private async Task<Resultado> VincularGestorImediatoAsync(Domain.Entities.Usuario usuario, string gestorCodigo)
        {
            if (gestorCodigo.IsNullOrEmpty())
            {
                usuario.GestorImediatoId = null;
                usuario.GestorImediatoCodigo = null;
                return Resultado.Sucesso();
            }

            var codigoGestor = gestorCodigo.ToUpper();
            if (codigoGestor == usuario.Codigo)
                return Resultado.Falha("O usuario nao pode ser gestor imediato dele mesmo.");

            var gestor = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoGestor);
            if (gestor is null)
                return Resultado.Falha("Gestor imediato nao encontrado.");

            usuario.GestorImediatoId = gestor.Id;
            usuario.GestorImediatoCodigo = gestor.Codigo;

            return Resultado.Sucesso();
        }

        private async Task<Resultado> EnviarEmailPrimeiroAcessoAsync(Domain.Entities.Usuario usuario, string clientUri)
        {
            if (string.IsNullOrWhiteSpace(clientUri))
                return Resultado.Falha("URI da aplicacao nao informada para envio do link de primeiro acesso.");

            var token = await _usuarioIdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo);
            if (string.IsNullOrWhiteSpace(token))
                return Resultado.Falha("Nao foi possivel gerar o link de primeiro acesso.");

            var identificadorTemporario = Guid.NewGuid().ToString("N");
            var dadosTemporarios = new DadosTemporarios
            {
                IdentificadorTemporario = identificadorTemporario,
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = token,
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var cacheInfo = new CacheInfo<DadosTemporarios>(ECacheKeysInfo.DadosTemporarios, identificadorTemporario)
            {
                EntityInfo = dadosTemporarios
            };
            _cacheService.GravarCache(cacheInfo, TimeSpan.FromHours(ValidadeConvitePrimeiroAcessoEmHoras));

            var identificadorCriptografado = CryptoHelper.EncryptCryptoJsAes(identificadorTemporario);
            var identificadorUrlEncoded = HttpUtility.UrlEncode(identificadorCriptografado);
            var link = $"{clientUri.TrimEnd('/')}/trocar-senha?id={identificadorUrlEncoded}";

            var resultadoEmail = await _emailService.EnviarAsync(CriarEmailPrimeiroAcesso(
                usuario.Email,
                usuario.Nome,
                link));

            if (resultadoEmail.TeveFalha)
            {
                _cacheService.RemoverCache(ECacheKeysInfo.DadosTemporarios, identificadorTemporario);
                return Resultado.Falha(resultadoEmail.Messages);
            }

            return Resultado.Sucesso();
        }

        private static string GerarSenhaTemporaria()
        {
            return $"Tmp!{Guid.NewGuid():N}9aA";
        }

        private static EmailRequest CriarEmailPrimeiroAcesso(string destinatario, string nomeUsuario, string link)
        {
            var corpo = $@"
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
                                        <h1>Bem-vindo ao Sistema Atron</h1>
                                    </div>
                                    <div class='content'>
                                        <p>Ola, <strong>{nomeUsuario}</strong>!</p>
                                        <p>Sua conta foi criada no Sistema Atron.</p>
                                        <p>Para definir sua senha de acesso, clique no botao abaixo. Este link expira em {ValidadeConvitePrimeiroAcessoEmHoras} horas.</p>
                                        <p style='text-align: center; margin: 30px 0;'>
                                            <a href='{link}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>Definir minha senha</a>
                                        </p>
                                        <p style='font-size: 12px; color: #999; word-break: break-all;'>Se o botao nao funcionar, copie e cole este link no navegador:<br>{link}</p>
                                    </div>
                                    <div class='footer'>
                                        <p>Este e um e-mail automatico. Por favor, nao responda.</p>
                                        <p>&copy; {DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

            return new EmailRequest
            {
                EmailsDestino = [destinatario],
                Assunto = "Defina sua senha de acesso - Sistema Atron",
                Mensagem = corpo
            };
        }
    }
}
