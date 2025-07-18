using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> ObterTodosRepositoryAsync();

        Task<TEntity> ObterPorIdRepositoryAsync(int id);

        Task<TEntity> ObterPorCodigoRepositoryAsync(string codigo);

        Task<bool> CriarRepositoryAsync(TEntity entity);

        Task<bool> AtualizarRepositoryAsync(TEntity entity);

        Task<bool> AtualizarRepositoryAsync(int id, TEntity entity);

        Task RemoverRepositoryAsync(TEntity entity);

        Task<bool> RemoverRepositoryAsync(int id);
    }
}