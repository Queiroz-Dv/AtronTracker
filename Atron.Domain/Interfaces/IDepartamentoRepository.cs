using Atron.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Domain.Interfaces
{
    /// <summary>
    /// Repository do módulo de departamento
    /// </summary>
    public interface IDepartamentoRepository : IRepository<Departamento>
    {
        /// <summary>
        /// Obtém todos os departamerntos de forma assíncrona
        /// </summary>
        /// <returns>Uma lista de departamentos</returns>
        Task<IEnumerable<Departamento>> ObterDepartmentosAsync();

        /// <summary>
        /// Obtém um departamento pelo id informado de forma assíncrona
        /// </summary>
        /// <param name="id">Identificador do departamento</param>
        /// <returns>Um departamento</returns>
        Task<Departamento> ObterDepartamentoPorIdRepositoryAsync(int? id);

        /// <summary>
        /// Obtém um departamento pelo código informado de forma assíncrona
        /// </summary>
        /// <param name="codigo">Código do departamento</param>
        /// <returns>Um departamento</returns>
        Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsync(string codigo);

        Task<Departamento> ObterDepartamentoPorCodigoRepositoryAsyncAsNoTracking(string codigo);

        /// <summary>
        /// Cria um departamento
        /// </summary>
        /// <param name="departamento">Entidade que será criada</param>
        Task<bool> CriarDepartamentoRepositoryAsync(Departamento departamento);

        /// <summary>
        /// Atualiza um departamento existente de forma assíncrona
        /// </summary>
        /// <param name="departamento">Entidade que será atualizada</param>
        /// <returns></returns>
        Task<bool> AtualizarDepartamentoRepositoryAsync(Departamento departamento);

        /// <summary>
        /// Exclui um departamento existente de forma assíncrona
        /// </summary>
        /// <param name="departamento">Entidade que será removida</param>
        Task<bool> RemoverDepartmentoRepositoryAsync(Departamento departamento);

        bool DepartamentoExiste(string codigo);
    }
}