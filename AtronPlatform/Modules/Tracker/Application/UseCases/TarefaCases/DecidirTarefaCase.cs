using Application.DTO;
using Application.Extensions;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Application.Messaging;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.TarefaCases
{
    public class DecidirTarefaCase(
        RegistrarDecisaoTarefaMovimentacaoCase registrarDecisaoMovimentacao,
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository,
        IEventBus eventBus,
        ObterUsuarioCase usuarioService)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly RegistrarDecisaoTarefaMovimentacaoCase _registrarDecisaoMovimentacao = registrarDecisaoMovimentacao;
        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> _mapper = mapper;
        private readonly ISolicitacaoObtencaoTarefaRepository _solicitacaoRepository = solicitacaoRepository;
        private readonly IEventBus _eventBus = eventBus;

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> ExecutarAsync(int solicitacaoId, bool aprovar)
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            var usuario = usuarioResultado.Dados;
            if (usuarioResultado.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(usuarioResultado.Messages);

            var aprovado = aprovar
                ? await _solicitacaoRepository.AprovarAsync(solicitacaoId, usuario.Id, usuario.Codigo)
                : await _solicitacaoRepository.RecusarAsync(solicitacaoId, usuario.Id, usuario.Codigo);

            if (!aprovado)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falha(TarefaResource.Erro_DecidirSolicitacao);

            var solicitacao = await _solicitacaoRepository.ObterPorIdAsync(solicitacaoId);

            var dto = _mapper.MapToDto(solicitacao);

            var movimentacao = await _registrarDecisaoMovimentacao.ExecutarAsync(solicitacao, usuario, aprovar);

            if (movimentacao.TeveFalha)
                return Resultado<SolicitacaoObtencaoTarefaDTO>.Falhas(movimentacao.Messages);

            var notificacaoEvent = new Application.Events.TarefaNotificacaoEvent(solicitacao.CriarNotificacaoDeDecisao(aprovar));
            await _eventBus.PublicarAsync(notificacaoEvent);
            return Resultado<SolicitacaoObtencaoTarefaDTO>
                .Sucesso(dto)
                .AdicionarMensagem(aprovar ? TarefaResource.Mensagem_SolicitacaoAprovada : TarefaResource.Mensagem_SolicitacaoRecusada);
        }
    }
}