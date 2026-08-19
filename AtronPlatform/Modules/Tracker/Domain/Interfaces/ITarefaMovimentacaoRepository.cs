using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITarefaMovimentacaoRepository
    {
        Task<bool> RegistrarAsync(TarefaMovimentacao movimentacao);

        Task<List<TarefaMovimentacao>> ObterMovimentacoesPorIdAsync(int tarefaId);
    }
}
