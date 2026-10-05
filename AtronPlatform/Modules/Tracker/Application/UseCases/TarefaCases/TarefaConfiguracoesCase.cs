using Application.DTO;
using Application.DTO.Request;
using Application.Resources;
using Application.UseCases.UsuarioCases;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.TarefaCases
{
    public class TarefaConfiguracoesCase(ObterUsuarioCase usuarioService, IUsuarioRepository usuarioRepository)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly IUsuarioRepository _usuarioRepository = usuarioRepository;

        public async Task<Resultado<TarefaConfiguracoesDTO>> ObterAsync()
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            var usuario = usuarioResultado.Dados;
            if (usuarioResultado.TeveFalha)
                return Resultado<TarefaConfiguracoesDTO>.Falhas(usuarioResultado.Messages);

            return Resultado<TarefaConfiguracoesDTO>.Sucesso(CriarDto(usuario.ReceberNotificacaoInternaTarefa, usuario.ReceberNotificacaoTarefaPorEmail));
        }

        public async Task<Resultado<TarefaConfiguracoesDTO>> AtualizarAsync(TarefaConfiguracoesRequest request)
        {
            if (request.IsNullable())
                return Resultado<TarefaConfiguracoesDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNulo);

            var usuarioResultado = await _usuarioService.ObterAsync();
            var usuario = usuarioResultado.Dados;
            if (usuarioResultado.TeveFalha)
                return Resultado<TarefaConfiguracoesDTO>.Falhas(usuarioResultado.Messages);

            var atualizado = await _usuarioRepository.AtualizarPreferenciasNotificacaoTarefaAsync(
                usuario.Codigo,
                request.ReceberNotificacaoInternaTarefa,
                request.ReceberNotificacaoTarefaPorEmail);

            if (!atualizado)
                return Resultado<TarefaConfiguracoesDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            return Resultado<TarefaConfiguracoesDTO>
                .Sucesso(CriarDto(request.ReceberNotificacaoInternaTarefa, request.ReceberNotificacaoTarefaPorEmail))
                .AdicionarMensagem(TarefaResource.Mensagem_ConfiguracoesAtualizadas);
        }

        private static TarefaConfiguracoesDTO CriarDto(bool receberInterna, bool receberEmail)
        {
            return new TarefaConfiguracoesDTO
            {
                ReceberNotificacaoInternaTarefa = receberInterna,
                ReceberNotificacaoTarefaPorEmail = receberEmail
            };
        }
    }
}