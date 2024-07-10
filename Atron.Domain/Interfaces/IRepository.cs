using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> ObterTodosRepositoryAsync();

        Task<TEntity> ObterPorIdRepositoryAsync(int id);

        Task<TEntity> ObterPorCodigoRepositoryAsync(string codigo);

        Task CriarRepositoryAsync(TEntity entity);

        Task AtualizarRepositoryAsync(TEntity entity);

        Task RemoverRepositoryAsync(TEntity entity);
    }
}