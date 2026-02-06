using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    /// <summary>
    /// Interface do serviço de Cargo
    /// </summary>
    public interface ICargoService
    {
        /// <summary>
        /// Obtém todos os cargos
        /// </summary>
        /// <returns>Uma lista de cargos</returns>
        Task<Resultado> ObterTodosAsync();

        /// <summary>
        /// Obtém um cargo por código
        /// </summary>
        /// <param name="codigo">Código do cargo</param>
        /// <returns>Um cargo</returns>
        Task<Resultado> ObterPorCodigoAsync(string codigo);

        /// <summary>
        /// Cria um cargo
        /// </summary>
        /// <param name="cargoDTO">Modelo que será criado</param>
        Task<Resultado> CriarAsync(CargoDTO cargoDTO);

        /// <summary>
        /// Atualiza um cargo existente
        /// </summary>
        /// <param name="codigo">Código do cargo</param>
        /// <param name="cargoDTO">Modelo que será atualizado</param>
        Task<Resultado> AtualizarAsync(string codigo, CargoDTO cargoDTO);

        /// <summary>
        /// Exclui um cargo existente por código informado
        /// </summary>
        /// <param name="codigo">Código do cargo</param>
        Task<Resultado> RemoverAsync(string codigo);
    }
}