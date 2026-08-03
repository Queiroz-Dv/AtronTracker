using Domain.Entities;
using Domain.Queries;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ITarefaMovimentacaoRepository
    {
        Task<bool> RegistrarAsync(TarefaMovimentacao movimentacao);

        Task<TarefaMovimentacaoPagina> ObterPaginaAsync(TarefaMovimentacaoConsulta consulta);
    }
}
