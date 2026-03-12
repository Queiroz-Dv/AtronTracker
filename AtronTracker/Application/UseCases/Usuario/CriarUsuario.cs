using Application.DTO.Request;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Requests;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.Usuario
{
    /// <summary>
    /// Caso de uso responsável pela criação de um novo usuário.
    /// Encapsula o núcleo atômico (Usuário de Negócio + Identity) e as operações
    /// best-effort (Cargo/Departamento e e-mail de boas-vindas).
    /// </summary>
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

        public CriarUsuario(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IEmailService emailService)
        {
            _validador = validador;
            _mapService = mapService;
            _usuarioRepository = usuarioRepository;
            _usuarioIdentityRepository = usuarioIdentityRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
            _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
            _emailService = emailService;
        }

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            // 1. Validação
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            // 2. Verificações de existência
            var usuarioExistente = await _usuarioRepository
                .ObterUsuarioPorCodigoAsync(request.Codigo.ToUpper());

            if (usuarioExistente != null)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);

            if (!request.Email.IsNullOrEmpty())
            {
                var emailExiste = await _usuarioRepository
                    .VerificarEmailExistenteAsync(request.Email);

                if (emailExiste)
                    return Resultado<UsuarioRequest>.Falha(EmailResource.ErroEmailUtilizado);
            }

            // 2.1 Verificação de conta Identity duplicada antes de persistir qualquer coisa.
            //     Evita criar o usuário de negócio e só então descobrir que o Identity já existe.
            if (!request.Senha.IsNullOrEmpty())
            {
                var contaExiste = await _usuarioIdentityRepository
                    .ContaExisteRepositoryAsync(request.Codigo.ToUpper(), request.Email);

                if (contaExiste)
                    return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);
            }

            // ── NÚCLEO ATÔMICO ────────────────────────────────────────────────────────
            // Os passos 3 e 4 formam a unidade indivisível: Usuário de Negócio + Identity.
            // Se o Identity falhar após o usuário de negócio ser gravado, o usuário de
            // negócio é removido manualmente (rollback explícito).
            // Motivação: UserManager do ASP.NET Identity não participa de TransactionScope
            // nem de IDbContextTransaction, portanto a atomicidade precisa ser garantida
            // de forma manual nesta camada.

            // 3. Criação do usuário de negócio
            var usuario = await _mapService.MapToEntityAsync(request);

            var criado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!criado)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioBd = await _usuarioRepository
                .ObterUsuarioPorCodigoAsync(usuario.Codigo);

            // 4. Criação da conta Identity
            if (!request.Senha.IsNullOrEmpty())
            {
                var identityCriado = await _usuarioIdentityRepository
                    .RegistrarContaDeUsuarioRepositoryAsync(
                        request.Codigo.ToUpper(),
                        request.Email,
                        request.Senha);

                if (!identityCriado)
                {
                    // Rollback manual: remove o usuário de negócio já persistido
                    // para não deixar um registro órfão sem acesso ao sistema.
                    await _usuarioRepository.RemoverUsuarioAsync(usuarioBd);
                    return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);
                }
            }

            // ── FIM DO NÚCLEO ATÔMICO ─────────────────────────────────────────────────

            // 5. Associação Cargo / Departamento (fora do núcleo atômico)
            //    Opcional: falha aqui não desfaz a criação do usuário.
            //    Decisão: Cargo/Departamento serão substituídos por RBAC em versão futura.
            if (!request.DepartamentoCodigo.IsNullOrEmpty() &&
                !request.CargoCodigo.IsNullOrEmpty())
            {
                var departamento = await _departamentoRepository
                    .ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(
                        request.DepartamentoCodigo);

                var cargo = await _cargoRepository
                    .ObterCargoPorCodigoAsync(request.CargoCodigo);

                if (departamento != null && cargo != null)
                {
                    await _usuarioCargoDepartamentoRepository
                        .GravarAssociacaoUsuarioCargoDepartamento(
                            usuarioBd,
                            cargo,
                            departamento);
                }
            }

            // 6. E-mail de boas-vindas (best-effort, fora do núcleo atômico)
            //    Falha no envio não interrompe nem desfaz a criação do usuário.
            await EnviarEmailBoasVindasAsync(request.Email, request.Nome);

            // 7. Retorno padronizado
            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem($"Usuário {request.Nome} {request.Sobrenome} salvo com sucesso.");
        }

        #region Métodos Privados

        private async Task EnviarEmailBoasVindasAsync(string destinatario, string nomeUsuario)
        {
            if (string.IsNullOrEmpty(destinatario)) return;

            try
            {
                var emailRequest = CriarEmailBoasVindas(destinatario, nomeUsuario);
                await _emailService.EnviarAsync(emailRequest);
            }
            catch
            {
                // Falha no envio não interrompe o fluxo de criação do usuário.
            }
        }

        private static EmailRequest CriarEmailBoasVindas(string destinatario, string nomeUsuario)
        {
            var assunto = "Bem-vindo ao Sistema Atron!";
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
                                        <h1>🎉 Bem-vindo ao Sistema Atron!</h1>
                                    </div>
                                    <div class='content'>
                                        <p>Olá, <strong>{nomeUsuario}</strong>!</p>
                                        <p>Sua conta foi criada com sucesso no Sistema Atron.</p>
                                        <p>Agora você pode acessar o sistema utilizando suas credenciais de login.</p>
                                        <p>Se você tiver alguma dúvida, entre em contato com o suporte.</p>
                                    </div>
                                    <div class='footer'>
                                        <p>Este é um e-mail automático. Por favor, não responda.</p>
                                        <p>&copy; {DateTime.Now.Year} Sistema Atron. Todos os direitos reservados.</p>
                                    </div>
                                </div>
                            </body>
                            </html>";

            return new EmailRequest
            {
                EmailsDestino = [destinatario],
                Assunto = assunto,
                Mensagem = corpo
            };
        }

        #endregion
    }
}