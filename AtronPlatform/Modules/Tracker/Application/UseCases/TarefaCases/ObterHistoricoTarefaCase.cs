using Application.DTO;
using Application.Resources;
using Application.UseCases.UsuarioCases;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Mapping;
using Shared.Domain.ValueObjects;

namespace Application.UseCases.TarefaCases
{
    public sealed class ObterHistoricoTarefaCase(
        ObterUsuarioCase usuarioService,
        ITarefaRepository tarefaRepository,
        ITarefaMovimentacaoRepository movimentacaoRepository,
        IToDtoMapper<TarefaMovimentacao, TarefaMovimentacaoDTO> mapper)
    {
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaMovimentacaoRepository _movimentacaoRepository = movimentacaoRepository;
        private readonly IToDtoMapper<TarefaMovimentacao, TarefaMovimentacaoDTO> _mapper = mapper;

        public async Task<Resultado<IReadOnlyCollection<TarefaMovimentacaoDTO>>> ExecutarAsync(
            int tarefaId)
        {
            var usuarioResultado = await _usuarioService.ObterAsync();
            if (usuarioResultado.TeveFalha)
                return Resultado<IReadOnlyCollection<TarefaMovimentacaoDTO>>
                    .Falhas(usuarioResultado.Messages);

            var usuario = usuarioResultado.Dados;
            var podeAcessar = await _tarefaRepository.PodeAcessarHistoricoAsync(
                tarefaId,
                usuario.Id,
                usuario.Codigo);

            if (!podeAcessar)
                return Resultado<IReadOnlyCollection<TarefaMovimentacaoDTO>>
                    .Falha(TarefaResource.Erro_AcessoHistoricoNaoPermitido);

            var movimentacoes = await _movimentacaoRepository
                .ObterMovimentacoesPorIdAsync(tarefaId);
            var dtos = _mapper.MapToDtos(movimentacoes).ToList();

            return Resultado<IReadOnlyCollection<TarefaMovimentacaoDTO>>.Sucesso(dtos);
        }
    }
}