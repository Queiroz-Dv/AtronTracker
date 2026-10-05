using Application.DTO;
using Application.Events;
using Application.Extensions;
using Application.Policies.Tarefas;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Messaging;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.TarefaCases
{
    public class AssumirTarefaCase(
        ITarefaRepository tarefaRepository,
        ObterUsuarioCase usuarioService,
        ITarefaObtencaoPolicy tarefaObtencaoPolicy,
        IEventBus eventBus,
        IToDtoMapper<Tarefa, TarefaDTO> tarefaMapper,
        RegistrarObtencaoTarefaMovimentacaoCase registrarMovimentacaoCase)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly ITarefaObtencaoPolicy _tarefaObtencaoPolicy = tarefaObtencaoPolicy;
        private readonly IEventBus _eventBus = eventBus;
        private readonly IToDtoMapper<Tarefa, TarefaDTO> _tarefaMapper = tarefaMapper;
        private readonly RegistrarObtencaoTarefaMovimentacaoCase _registrarMovimentacaoCase = registrarMovimentacaoCase;

        public async Task<Resultado<TarefaDTO>> ExecutarAsync(int tarefaId)
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var entidade = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (entidade.IsNullable())
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
            var notificacaoEvent = new TarefaNotificacaoEvent(publicacaoDto);
            await _eventBus.PublicarAsync(notificacaoEvent);

            var tarefa = _tarefaMapper.MapToDto(tarefaAtualizada);

            return Resultado<TarefaDTO>.Sucesso(tarefa).AdicionarMensagem(TarefaResource.Mensagem_TarefaAssumida);
        }
    }
}
