using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ITarefaMovimentacaoRepository
    {
        Task<bool> RegistrarAsync(TarefaMovimentacao movimentacao);

        Task<List<TarefaMovimentacao>> ObterMovimentacoesPorIdAsync(int tarefaId);
    }
}