namespace Shared.Interfaces.Data
{
    /// <summary>
    /// Interface compartilhada que gerencia o acesso aos dados
    /// </summary>
    /// <typeparam name="TEntity">Entidade de domínio</typeparam>
    public interface ISharedRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> ObterTodosRepositoryAsync();

        Task<TEntity> ObterPorIdRepositoryAsync(int id);

        Task<TEntity> ObterPorCodigoRepositoryAsync(string codigo);

        Task<bool> CriarRepositoryAsync(TEntity entity);

        Task<bool> AtualizarRepositoryAsync(TEntity entity);

        Task<bool> AtualizarRepositoryAsync(int id, TEntity entity);

        Task<bool> RemoverRepositoryAsync(TEntity entity);
    }
}