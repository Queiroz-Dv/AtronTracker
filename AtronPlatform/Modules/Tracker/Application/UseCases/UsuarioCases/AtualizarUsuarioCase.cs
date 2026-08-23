using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.Mapping;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.UsuarioCases
{
    public class AtualizarUsuarioCase(
        VerificarAtualizacaoUsuarioCase verificarAtualizacaoUsuarioCase,
        VincularGestorImediatoCase vincularGestorImediatoCase,
        AtualizarCredenciaisUsuarioCase atualizarCredenciaisUsuarioCase,
        AtualizarAssociacaoUsuarioCargoDepartamentoCase atualizarAssociacaoUsuarioCargoDepartamentoCase,
        AuditoriaUsuarioCase auditoriaUsuarioCase,
        UsuarioRequestMapping mapService,
        IUsuarioRepository usuarioRepository,
        ICacheUsuarioService cacheUsuarioService)
    {
        private readonly VerificarAtualizacaoUsuarioCase _verificarAtualizacaoUsuarioCase = verificarAtualizacaoUsuarioCase;
        private readonly VincularGestorImediatoCase _vincularGestorImediatoCase = vincularGestorImediatoCase;
        private readonly AtualizarCredenciaisUsuarioCase _atualizarCredenciaisUsuarioCase = atualizarCredenciaisUsuarioCase;
        private readonly AtualizarAssociacaoUsuarioCargoDepartamentoCase _atualizarAssociacaoUsuarioCargoDepartamentoCase = atualizarAssociacaoUsuarioCargoDepartamentoCase;
        private readonly AuditoriaUsuarioCase _auditoriaUsuarioCase = auditoriaUsuarioCase;
        private readonly UsuarioRequestMapping _mapService = mapService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;
        private readonly ICacheUsuarioService _cacheUsuarioService = cacheUsuarioService;

        public async Task<Resultado<UsuarioRequest>> ExecutarAsync(UsuarioRequest request)
        {
            var verificacao = await _verificarAtualizacaoUsuarioCase.ExecutarAsync(request);
            if (verificacao.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(verificacao.Messages);

            var usuario = verificacao.Dados!;
            _mapService.MapToEntity(request, usuario);

            var resultadoGestor = await _vincularGestorImediatoCase
                .ExecutarAsync(usuario, usuario.GestorImediatoCodigo);

            if (resultadoGestor.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(resultadoGestor.Messages);

            var atualizado = await _usuarioRepository.AtualizarUsuarioAsync(usuario);
            if (!atualizado)
                return Resultado<UsuarioRequest>.Falha(UsuarioResource.ErroInesperadoAtualizacao);

            var credenciais = await _atualizarCredenciaisUsuarioCase.ExecutarAsync(usuario, request.Senha);
            if (credenciais.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(credenciais.Messages);

            var associacao = await _atualizarAssociacaoUsuarioCargoDepartamentoCase
                .ExecutarAsync(request, usuario);
            if (associacao.TeveFalha)
                return Resultado<UsuarioRequest>.Falhas(associacao.Messages);

            await _auditoriaUsuarioCase.RegistrarAtualizacaoAsync(usuario);
            _cacheUsuarioService.RemoverCacheDeAcessoTokenInfo(usuario.Codigo);

            return Resultado<UsuarioRequest>
                .Sucesso(request)
                .AdicionarMensagem(UsuarioResource.MensagemUsuarioAtualizado);
        }
    }
}
