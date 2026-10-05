using Application.DTO;
using Application.Events;
using Application.Extensions;
using Application.Policies.Tarefas;
using Application.Resources;
using Application.Services.EntitiesServices;
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
    public class SolicitarTarefaCase(
        ObterUsuarioCase usuarioService,
        ITarefaRepository tarefaRepository,
        ITarefaObtencaoPolicy obtencaoPolicy,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository,
        RegistrarSolicitacaoTarefaMovimentacaoCase registrarMovimentacaoCase,
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        IEventBus eventBus,
        AprovadorObtencaoTarefaService aprovadorResolver)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaObtencaoPolicy _obtencaoPolicy = obtencaoPolicy;
        private readonly ISolicitacaoObtencaoTarefaRepository _solicitacaoRepository = solicitacaoRepository;
        private readonly RegistrarSolicitacaoTarefaMovimentacaoCase _registrarMovimentacaoCase = registrarMovimentacaoCase;

        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> _mapper = mapper;
        private readonly IEventBus _eventBus = eventBus;
        private readonly AprovadorObtencaoTarefaService _aprovadorResolver = aprovadorResolver;

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> ExecutarAsync(int tarefaId)
        {
            var usuario = await _usuarioService.ObterAsync();
            if (usuario.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(usuario.Messages);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var possuiResponsabilidadeGestao = await _tarefaRepository.PossuiResponsabilidadeGestaoAsync(usuario.Dados.Id, usuario.Dados.Codigo);

            var avaliacao = _obtencaoPolicy.AvaliarSolicitacao(tarefa, possuiResponsabilidadeGestao);
            if (avaliacao.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(avaliacao.Messages);

            if (await _solicitacaoRepository.ExisteSolicitacaoPendenteParaTarefaAsync(tarefaId))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_SolicitacaoPendenteExistente);

            var aprovador = await _aprovadorResolver.ResolverAsync(usuario.Dados, tarefa);
            if (aprovador.IsNullable() && !possuiResponsabilidadeGestao)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_AprovadorIndisponivel);

            var solicitacao = SolicitacaoObtencaoTarefa.CriarPendente(tarefa, usuario.Dados, aprovador);
            if (!await _solicitacaoRepository.CriarAsync(solicitacao))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_CriarSolicitacao);

            var solicitacaoGravada = await _solicitacaoRepository.ObterPorIdAsync(solicitacao.Id);
            var movimentacao = await _registrarMovimentacaoCase.ExecutarAsync(solicitacaoGravada, usuario.Dados);
            if (movimentacao.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(movimentacao.Messages);

            if (!possuiResponsabilidadeGestao)
            {
                var notificacaoEvent = new TarefaNotificacaoEvent(solicitacaoGravada.CriarNotificacaoDeRecebimento());
                await _eventBus.PublicarAsync(notificacaoEvent);
            }

            var dto = _mapper.MapToDto(solicitacaoGravada);
            return Resultado<SolicitacaoObtencaoTarefaDTO>.Sucesso(dto).AdicionarMensagem(TarefaResource.Mensagem_SolicitacaoEnviada);
        }
    }
}
