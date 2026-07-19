using Application.DTO;
using Application.DTO.Request;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaConfiguracoesService : ITarefaConfiguracoesService
    {
        private readonly ITarefaUsuarioAtualService _usuarioAtualService;
        private readonly IUsuarioRepository _usuarioRepository;

        public TarefaConfiguracoesService(ITarefaUsuarioAtualService usuarioAtualService, IUsuarioRepository usuarioRepository)
        {
            _usuarioAtualService = usuarioAtualService;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Resultado<TarefaConfiguracoesDTO>> ObterAsync()
        {
            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<TarefaConfiguracoesDTO>.Falhas(usuario.Messages);

            return Resultado<TarefaConfiguracoesDTO>.Sucesso(CriarDto(usuario.Dados.ReceberNotificacaoInternaTarefa, usuario.Dados.ReceberNotificacaoTarefaPorEmail));
        }

        public async Task<Resultado<TarefaConfiguracoesDTO>> AtualizarAsync(TarefaConfiguracoesRequest request)
        {
            if (request is null)
                return Resultado<TarefaConfiguracoesDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNulo);

            var usuario = await _usuarioAtualService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<TarefaConfiguracoesDTO>.Falhas(usuario.Messages);

            var atualizado = await _usuarioRepository.AtualizarPreferenciasNotificacaoTarefaAsync(
                usuario.Dados.Codigo,
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
