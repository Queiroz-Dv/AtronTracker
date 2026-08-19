using Application.DTO;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Policies.Tarefas;
using Application.Resolvers.Tarefas;
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
    public class SolicitarTarefaCase(
        IUsuarioService usuarioService,
        ITarefaRepository tarefaRepository,
        ITarefaObtencaoPolicy obtencaoPolicy,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository,
        RegistrarSolicitacaoTarefaMovimentacaoCase registrarMovimentacaoCase,
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        TarefaNotificacaoInternaCase tarefaNotificacao,
        AprovadorObtencaoTarefaResolver aprovadorResolver)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaObtencaoPolicy _obtencaoPolicy = obtencaoPolicy;
        private readonly ISolicitacaoObtencaoTarefaRepository _solicitacaoRepository = solicitacaoRepository;
        private readonly RegistrarSolicitacaoTarefaMovimentacaoCase _registrarMovimentacaoCase = registrarMovimentacaoCase;

        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> _mapper = mapper;
        private readonly TarefaNotificacaoInternaCase _notificacaoCase = tarefaNotificacao;
        private readonly AprovadorObtencaoTarefaResolver _aprovadorResolver = aprovadorResolver;

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> ExecutarAsync(int tarefaId)
        {
            var usuario = await _usuarioService.ObterUsuarioAtual();
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
            if (aprovador is null)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_AprovadorIndisponivel);

            var solicitacao = SolicitacaoObtencaoTarefa.CriarPendente(tarefa, usuario.Dados, aprovador);
            if (!await _solicitacaoRepository.CriarAsync(solicitacao))
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_CriarSolicitacao);

            var solicitacaoGravada = await _solicitacaoRepository.ObterPorIdAsync(solicitacao.Id);
            var movimentacao = await _registrarMovimentacaoCase.ExecutarAsync(solicitacaoGravada, usuario.Dados);
            if (movimentacao.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(movimentacao.Messages);

            await _notificacaoCase.ExecutarAsync(solicitacaoGravada.CriarNotificacaoDeRecebimento());

            var dto = _mapper.MapToDto(solicitacaoGravada);
            return Resultado<SolicitacaoObtencaoTarefaDTO>.Sucesso(dto).AdicionarMensagem(TarefaResource.Mensagem_SolicitacaoEnviada);
        }
    }
}
