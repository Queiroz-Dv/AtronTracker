using Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces
{
    public interface ISolicitacaoObtencaoTarefaRepository
    {
        Task<bool> ExisteSolicitacaoPendenteParaTarefaAsync(int tarefaId);

        Task<SolicitacaoObtencaoTarefa> ObterPorIdAsync(int id);

        Task<IEnumerable<SolicitacaoObtencaoTarefa>> ObterPendentesPorAprovadorAsync(int aprovadorId, string aprovadorCodigo);

        Task<bool> CriarAsync(SolicitacaoObtencaoTarefa solicitacao);

        Task<bool> AprovarAsync(int id, int usuarioId, string usuarioCodigo);

        Task<bool> RecusarAsync(int id, int usuarioId, string usuarioCodigo);

        Task<IEnumerable<SolicitacaoObtencaoTarefa>> ObterPendentesPorAprovadorOuDepartamentosAsync(
            int aprovadorId,
            string aprovadorCodigo,
            IEnumerable<string> departamentoCodigos);
    }
}
