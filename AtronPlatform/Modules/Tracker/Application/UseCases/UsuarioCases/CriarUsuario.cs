using Application.DTO.Request;
using Application.Email.Compositores;
using Application.Email.Models;
using Application.Interfaces.Services;
using Domain.Interfaces;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.DTOS.Common;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.Enums;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class CriarUsuario(
        IValidador<UsuarioRequest> validador,
        IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> mapService,
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository,
        IDepartamentoRepository departamentoRepository,
        ICargoRepository cargoRepository,
        IUsuarioCargoDepartamentoRepository usuarioCargoDepartamentoRepository,
        IEmailService emailService,
        IAcessoEmailCompositor emailCompositor,
        ICacheService cacheService,
        IAuditoriaService auditoriaService,
        IEnderecoFrontendService enderecoFrontendService,
        ITokenTemporarioService tokenTemporarioService)
    {
        private readonly IValidador<UsuarioRequest> _validador = validador;
        private readonly IAsyncMap<UsuarioRequest, Domain.Entities.Usuario> _mapService = mapService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;
        private readonly IDepartamentoRepository _departamentoRepository = departamentoRepository;
        private readonly ICargoRepository _cargoRepository = cargoRepository;
        private readonly IUsuarioCargoDepartamentoRepository _usuarioCargoDepartamentoRepository = usuarioCargoDepartamentoRepository;
        private readonly IEmailService _emailService = emailService;
        private readonly IAcessoEmailCompositor _emailCompositor = emailCompositor;
        private readonly ICacheService _cacheService = cacheService;
        private readonly IAuditoriaService _auditoriaService = auditoriaService;
        private readonly IEnderecoFrontendService _enderecoFrontendService = enderecoFrontendService;
        private readonly ITokenTemporarioService _tokenTemporarioService = tokenTemporarioService;

        private const string UsuarioContexto = nameof(Domain.Entities.Usuario);
        private const int ValidadeConvitePrimeiroAcessoEmHoras = 24;

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var mensagens = _validador.Validar(request);
            if (mensagens.Any())
                return Resultado<UsuarioRequest>.Falhas(mensagens);

            var codigoUsuario = request.Codigo.ToUpper();
            var usuarioExistente = await _usuarioRepository.ObterUsuarioGeralPorCodigoAsync(codigoUsuario);

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

            var conviteEnviado = await EnviarEmailPrimeiroAcessoAsync(usuarioBd);
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
                .AdicionarMensagem(string.Format(
                    UsuarioResource.MensagemUsuarioCriadoPrimeiroAcesso,
                    request.Nome,
                    request.Sobrenome));
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
                return Resultado.Falha(UsuarioResource.ErroGestorProprio);

            var gestor = await _usuarioRepository.ObterUsuarioPorCodigoAsync(codigoGestor);
            if (gestor is null)
                return Resultado.Falha(UsuarioResource.ErroGestorNaoEncontrado);

            usuario.GestorImediatoId = gestor.Id;
            usuario.GestorImediatoCodigo = gestor.Codigo;

            return Resultado.Sucesso();
        }

        private async Task<Resultado> EnviarEmailPrimeiroAcessoAsync(Domain.Entities.Usuario usuario)
        {
            var token = await _usuarioIdentityRepository.GerarTokenRecuperacaoSenhaAsync(usuario.Codigo);
            if (string.IsNullOrWhiteSpace(token))
                return Resultado.Falha(AuthResource.Erro_GerarLinkPrimeiroAcesso);

            var identificadorTemporario = _tokenTemporarioService.Criar();
            var dadosTemporarios = new DadosTemporarios
            {
                UsuarioCodigo = usuario.Codigo,
                Email = usuario.Email,
                Token = token,
                DataAlteracaoSenha = DateTime.UtcNow
            };

            var chaveCache = new ChaveCache(ECacheKeysInfo.DadosTemporarios, identificadorTemporario.Hash);
            var cacheInfo = new CacheInfo<DadosTemporarios>(chaveCache);
            cacheInfo.VincularDadosTemporarios(dadosTemporarios);

            _cacheService.GravarCache(cacheInfo, TimeSpan.FromHours(ValidadeConvitePrimeiroAcessoEmHoras));

            var uriBase = _enderecoFrontendService.ObterUriBase();
            var link = $"{uriBase}/trocar-senha#token={identificadorTemporario.Valor}";

            Resultado resultadoEmail;
            try
            {
                var email = _emailCompositor.ComporPrimeiroAcesso(new PrimeiroAcessoEmailParametros(
                    usuario.Email,
                    usuario.Nome,
                    link,
                    ValidadeConvitePrimeiroAcessoEmHoras));

                if (email.TeveFalha)
                    return Resultado.Falha(email.Messages);

                resultadoEmail = await _emailService.EnviarAsync(email.Dados);
            }
            catch
            {
                _cacheService.RemoverCache(chaveCache);
                return Resultado.Falha(AuthResource.Erro_EnvioEmailObrigatorio);
            }

            if (resultadoEmail.TeveFalha)
            {
                _cacheService.RemoverCache(chaveCache);
                return Resultado.Falha(resultadoEmail.Messages);
            }

            return Resultado.Sucesso();
        }

        private static string GerarSenhaTemporaria()
        {
            return $"Tmp!{Guid.NewGuid():N}9aA";
        }

    }
}
