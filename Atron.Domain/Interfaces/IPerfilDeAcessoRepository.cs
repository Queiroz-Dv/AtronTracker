using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IPerfilDeAcessoRepository
    {
        Task<ICollection<PerfilDeAcesso>> ObterTodosPerfisRepositoryAsync();
        Task<PerfilDeAcesso> ObterPerfilPorIdRepositoryAsync(int id);
        Task<PerfilDeAcesso> ObterPerfilPorCodigoRepositoryAsync(string codigo);
        Task<bool> CriarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso);
        Task<bool> AtualizarPerfilRepositoryAsync(PerfilDeAcesso perfilDeAcesso);
        Task<bool> DeletarPerfilRepositoryAsync(string codigo);
    }
}