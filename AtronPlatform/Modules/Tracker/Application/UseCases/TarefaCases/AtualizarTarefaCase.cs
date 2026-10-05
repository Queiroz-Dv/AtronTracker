using Application.DTO;
using Application.Records.Tarefa;
using Application.Resources;
using Application.Services.EntitiesServices.Tarefas;
using Application.UseCases.TarefaCases.Movimentacao;
using Application.UseCases.UsuarioCases;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.UseCases.TarefaCases
{
    public class AtualizarTarefaCase(
        ITarefaRepository tarefaRepository,
        TarefaPreparacaoService tarefaPreparacaoService,
        ObterUsuarioCase usuarioService,
        AtualizarTarefaMovimentacaoCase atualizarMovimentacao)
    {
        private readonly ITarefaRepository _tarefaRepository = tarefaRepository;
        private readonly TarefaPreparacaoService _tarefaPreparacaoService = tarefaPreparacaoService;
        private readonly ObterUsuarioCase _usuarioService = usuarioService;
        private readonly AtualizarTarefaMovimentacaoCase _atualizarMovimentacao = atualizarMovimentacao;

        public async Task<Resultado> ExecutarAsync(int id, TarefaDTO tarefaDTO)
        {
            var tarefaAnterior = await _tarefaRepository.ObterTarefaPorId(id);
            if (tarefaAnterior.IsNullable())
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var preparacaoResultado = await _tarefaPreparacaoService.PrepararParaPersistenciaAsync(tarefaDTO);
            if (preparacaoResultado.TeveFalha)
                return Resultado.Falha(preparacaoResultado.Messages);

            var responsavelResultado = await _usuarioService.ObterAsync();
            if (responsavelResultado.TeveFalha)
                return Resultado.Falha(responsavelResultado.Messages);

            var tarefaAtual = preparacaoResultado.Dados;
            var responsavel = responsavelResultado.Dados;

            var parametros = new AtualizacaoMovimentacaoRecord(tarefaAnterior, tarefaAtual, responsavel);

            var movimentacao = await _atualizarMovimentacao.ExecutarAsync(parametros);
            if (movimentacao.TeveFalha)
                return Resultado.Falha(movimentacao.Messages);

            if (!await _tarefaRepository.AtualizarTarefaAsync(id, tarefaAtual))
                return Resultado.Falha(TarefaResource.Erro_AtualizarTarefa);

            return Resultado.Sucesso().AdicionarMensagem(TarefaResource.Mensagem_TarefaAtualizada);
        }
    }
}