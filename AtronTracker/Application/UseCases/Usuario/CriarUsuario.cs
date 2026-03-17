using Application.DTO.Request;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
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
        private readonly IAuditoriaService _auditoriaService;

        private const string UsuarioContexto = "Usuario";

        public CriarUsuario(
            IValidador<UsuarioRequest> validador,
            IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
            IUsuarioRepository usuarioRepository,
            IUsuarioIdentityRepository usuarioIdentityRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository,
            IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
            IEmailService emailService,
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
            _auditoriaService = auditoriaService;
        }

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            var usuarioExistente = await _usuarioRepository.ObterUsuarioPorCodigoAsync(request.Codigo.ToUpper());
            if (usuarioExistente != null)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);

            if (!request.Email.IsNullOrEmpty())
            {
                var emailExiste = await _usuarioRepository.VerificarEmailExistenteAsync(request.Email);
                if (emailExiste)
                    return Resultado<UsuarioRequest>.Falha(EmailResource.ErroEmailUtilizado);
            }

            if (!request.Senha.IsNullOrEmpty())
            {
                var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(request.Codigo.ToUpper(), request.Email);
                if (contaExiste)
                    return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroUsuarioExistente);
            }

            // ── NÚCLEO ATÔMICO ────────────────────────────────────────────────
            var usuario = await _mapService.MapToEntityAsync(request);
            var criado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!criado)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            if (!request.Senha.IsNullOrEmpty())
            {
                var identityCriado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(
                    request.Codigo.ToUpper(), request.Email, request.Senha);

                if (!identityCriado)
                {
                    await _usuarioRepository.RemoverUsuarioAsync(usuarioBd);
                    return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);
                }
            }
            // ── FIM DO NÚCLEO ATÔMICO ─────────────────────────────────────────

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
                    Descricao = $"Usuário {usuarioBd.Codigo} criado em {DateTime.Now:dd/MM/yyyy HH:mm}."
                }
            });

            await EnviarEmailBoasVindasAsync(request.Email, request.Nome);

            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem($"Usuário {request.Nome} {request.Sobrenome} salvo com sucesso.");
        }

        private async Task EnviarEmailBoasVindasAsync(string destinatario, string nomeUsuario)
        {
            if (string.IsNullOrEmpty(destinatario)) return;
            try
            {
                await _emailService.EnviarAsync(CriarEmailBoasVindas(destinatario, nomeUsuario));
            }
            catch { }
        }

        private static EmailRequest CriarEmailBoasVindas(string destinatario, string nomeUsuario)
        {
            var corpo = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h1 style='color: #007bff;'>Bem-vindo ao Sistema Atron!</h1>
                    <p>Olá, <strong>{nomeUsuario}</strong>!</p>
                    <p>Sua conta foi criada com sucesso. Acesse o sistema com suas credenciais.</p>
                    <p style='font-size: 12px; color: #aaa;'>Este é um e-mail automático. Por favor, não responda.</p>
                </div>";

            return new EmailRequest
            {
                EmailsDestino = [destinatario],
                Assunto = "Bem-vindo ao Sistema Atron!",
                Mensagem = corpo
            };
        }
    }
}