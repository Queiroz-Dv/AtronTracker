using Application.DTO;
using Application.Extensions;
using Application.Interfaces.Services;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class DecidirTarefaCase(
        RegistrarDecisaoTarefaMovimentacaoCase registrarDecisaoMovimentacao,
        IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> mapper,
        ISolicitacaoObtencaoTarefaRepository solicitacaoRepository,
        TarefaNotificacaoInternaCase notificacaoInternaCase,
        IUsuarioService usuarioService)
    {
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly RegistrarDecisaoTarefaMovimentacaoCase _registrarDecisaoMovimentacao = registrarDecisaoMovimentacao;
        private readonly IToDtoMapper<SolicitacaoObtencaoTarefa, SolicitacaoObtencaoTarefaDTO> _mapper = mapper;
        private readonly ISolicitacaoObtencaoTarefaRepository _solicitacaoRepository = solicitacaoRepository;
        private readonly TarefaNotificacaoInternaCase _notificacaoInternaCase = notificacaoInternaCase;

        public async Task<Resultado<SolicitacaoObtencaoTarefaDTO>> ExecutarAsync(int solicitacaoId, bool aprovar)
        {
            var usuarioResultado = await _usuarioService.ObterUsuarioAtual();
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

            await _notificacaoInternaCase.ExecutarAsync(solicitacao.CriarNotificacaoDeDecisao(aprovar));
            return Resultado<SolicitacaoObtencaoTarefaDTO>
                .Sucesso(dto)
                .AdicionarMensagem(aprovar ? TarefaResource.Mensagem_SolicitacaoAprovada : TarefaResource.Mensagem_SolicitacaoRecusada);
        }
    }
}