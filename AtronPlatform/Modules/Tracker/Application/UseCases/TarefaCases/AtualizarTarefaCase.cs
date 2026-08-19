using Application.DTO;
using Application.Interfaces.Services;
using Application.Records.Tarefa;
using Application.Resources;
using Application.UseCases.TarefaCases.Movimentacao;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class AtualizarTarefaCase(
        ITarefaRepository tarefaRepository,
        ITarefaPreparacaoService tarefaPreparacaoService,
        IUsuarioService usuarioService,
        AtualizarTarefaMovimentacaoCase atualizarMovimentacao)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly ITarefaPreparacaoService _tarefaPreparacaoService = tarefaPreparacaoService;
        private readonly IUsuarioService _usuarioService = usuarioService;
        private readonly AtualizarTarefaMovimentacaoCase _atualizarMovimentacao = atualizarMovimentacao;

        public async Task<Resultado<TarefaDTO>> ExecutarAsync(int id, TarefaDTO tarefaDTO)
        {
            var tarefaAnterior = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaAnterior is null)
                return Resultado<TarefaDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var preparacaoResultado = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacaoResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(preparacaoResultado.Messages);

            var responsavelResultado = await _usuarioService.ObterUsuarioAtual();
            if (responsavelResultado.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(responsavelResultado.Messages);

            var tarefaAtual = preparacaoResultado.Dados;
            var responsavel = responsavelResultado.Dados;

            var parametros = new AtualizacaoMovimentacaoRecord(tarefaAnterior, tarefaAtual, responsavel);

            var movimentacao = await _atualizarMovimentacao.ExecutarAsync(parametros);
            if (movimentacao.TeveFalha)
                return Resultado<TarefaDTO>.Falhas(movimentacao.Messages);

            if (!await _tarefaRepository.AtualizarTarefaAsync(id, tarefaAtual))
                return Resultado<TarefaDTO>.Falha(TarefaResource.Erro_AtualizarTarefa);

            return Resultado<TarefaDTO>.Sucesso(tarefaDTO).AdicionarMensagem(TarefaResource.Mensagem_TarefaAtualizada);
        }
    }
}
