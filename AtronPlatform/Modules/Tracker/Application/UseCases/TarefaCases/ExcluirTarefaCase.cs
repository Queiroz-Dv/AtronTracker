using Application.Resources;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.TarefaCases
{
    public class ExcluirTarefaCase(ITarefaRepository tarefaRepository)
    {
        public ITarefaRepository _tarefaRepository = tarefaRepository;

        public async Task<Resultado> ExecutarAsync(string id)
        {
            if (!int.TryParse(id, out var tarefaId))
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var tarefa = await _tarefaRepository.ObterTarefaPorId(tarefaId);
            if (tarefa is null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (!await _tarefaRepository.RemoverRepositoryAsync(tarefa))
                return Resultado.Falha(TarefaResource.Erro_RemoverTarefa);

            return Resultado.Sucesso().AdicionarMensagem(TarefaResource.Mensagem_TarefaRemovida);
        }
    }
}