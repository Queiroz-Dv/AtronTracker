using Application.DTO.Request;
using Application.Mapping;
using Domain.Interfaces.Identity;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class CriarUsuarioCase(
        AuditoriaUsuarioCase auditoriaUsuarioCase,
        VincularGestorImediatoCase vincularGestorImediatoCase,
        EnviarEmailPrimeiroAcessoCase enviarEmailPrimeiroAcessoCase,
        VerificarUsuarioCase verificacaoUsuarioCase,
        AssociarUsuarioCargoDepartamentoCase associarUsuarioCargoDepartamentoCase,
        UsuarioRequestMapping mapService,
        IUsuarioRepository usuarioRepository,
        IUsuarioIdentityRepository usuarioIdentityRepository)
    {
        private readonly AuditoriaUsuarioCase _auditoriaUsuarioCase = auditoriaUsuarioCase;
        private readonly VincularGestorImediatoCase _vincularGestorImediatoCase = vincularGestorImediatoCase;
        private readonly EnviarEmailPrimeiroAcessoCase _enviarEmailPrimeiroAcessoCase = enviarEmailPrimeiroAcessoCase;
        private readonly VerificarUsuarioCase _verificacaoUsuarioCase = verificacaoUsuarioCase;
        private readonly AssociarUsuarioCargoDepartamentoCase _associarUsuarioCargoDepartamentoCase = associarUsuarioCargoDepartamentoCase;
        private readonly UsuarioRequestMapping _mapService = mapService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly IUsuarioIdentityRepository _usuarioIdentityRepository = usuarioIdentityRepository;

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var verificacaoResultado = await _verificacaoUsuarioCase.ExecutarAsync(request);
            if (verificacaoResultado.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(verificacaoResultado.Messages);

            var usuario = _mapService.MapToEntity(request);
            var resultadoGestor = await _vincularGestorImediatoCase.ExecutarAsync(usuario, request.GestorImediatoCodigo);
            if (resultadoGestor.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(resultadoGestor.Messages);

            var criado = await _usuarioRepository.CriarUsuarioAsync(usuario);
            if (!criado)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);

            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

            var identityCriado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(
                usuarioBd.Codigo,
                request.Email,
                GerarSenhaTemporaria());

            if (!identityCriado)
            {
                await _usuarioRepository.RemoverUsuarioAsync(usuarioBd);
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoGravacao);
            }

            var conviteEnviado = await _enviarEmailPrimeiroAcessoCase.ExecutarAsync(usuarioBd);
            if (conviteEnviado.TeveFalha)
            {
                await _usuarioIdentityRepository.DeletarContaUserRepositoryAsync(usuarioBd.Codigo);
                await _usuarioRepository.RemoverUsuarioAsync(usuarioBd);
                return Resultado<UsuarioRequest>.Falhas(conviteEnviado.Messages);
            }

            if (!request.DepartamentoCodigo.IsNullOrEmpty() && !request.CargoCodigo.IsNullOrEmpty())
            {
                await _associarUsuarioCargoDepartamentoCase.ExecutarAsync(request, usuarioBd);
            }

            await _auditoriaUsuarioCase.ExecutarAsync(usuario);

            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem(string.Format(
                    UsuarioResource.MensagemUsuarioCriadoPrimeiroAcesso,
                    request.Nome,
                    request.Sobrenome));
        }


        private static string GerarSenhaTemporaria()
        {
            return $"Tmp!{Guid.NewGuid():N}9aA";
        }
    }
}