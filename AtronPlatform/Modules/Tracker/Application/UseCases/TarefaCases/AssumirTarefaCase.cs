using Application.DTO;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Policies.Tarefas;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class AssumirTarefaCase(
        ITarefaRepository tarefaRepository,
        IUsuarioService usuarioService,
        ITarefaObtencaoPolicy tarefaObtencaoPolicy,
        TarefaNotificacaoInternaCase notificacaoInternaCase,
        IToDtoMapper<Tarefa, TarefaDTO> tarefaMapper,
        RegistrarObtencaoTarefaMovimentacaoCase registrarMovimentacaoCase)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ITarefaObtencaoPolicy _tarefaObtencaoPolicy = tarefaObtencaoPolicy;
        private readonly TarefaNotificacaoInternaCase _notificacaoInternaCase = notificacaoInternaCase;
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _tarefaMapper = tarefaMapper;
        private readonly RegistrarObtencaoTarefaMovimentacaoCase _registrarMovimentacaoCase = registrarMovimentacaoCase;

        public async Task<Resultado<TarefaDTO>> ExecutarAsync(int tarefaId)
        {
            var usuarioResultado = await _usuarioService.ObterUsuarioAtual();
            if (usuarioResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var entidade = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (entidade is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var possuiResponsabilidadeGestao = await _tarefaRepository.PossuiResponsabilidadeGestaoAsync(usuario.Id, usuario.Codigo);
            var policyResultado = _tarefaObtencaoPolicy.AvaliarAssuncao(entidade, possuiResponsabilidadeGestao);
            if (policyResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(policyResultado.Messages);

            if (!await _tarefaRepository.AssumirTarefaAsync(tarefaId, usuario.Id, usuario.Codigo))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_AssumirTarefa);

            var tarefaAtualizada = await _tarefaRepository.ObterTarefaPorId(tarefaId);

            var movimentacao = await _registrarMovimentacaoCase.ExecutarAsync(entidade, usuario);
            if (movimentacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(movimentacao.Messages);

            var publicacaoDto = tarefaAtualizada.CriarNotificacaoDeObtencao(usuario);
            await _notificacaoInternaCase.ExecutarAsync(publicacaoDto);

            var tarefa = _tarefaMapper.MapToDto(tarefaAtualizada);

            return Resultado<TarefaDTO>.Sucesso(tarefa).AdicionarMensagem(TarefaResource.Mensagem_TarefaAssumida);
        }
    }
}
