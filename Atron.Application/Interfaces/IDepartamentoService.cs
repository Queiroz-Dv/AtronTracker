using Atron.Application.DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atron.Application.Interfaces
{
    public interface IDepartamentoService
    {
        /// <summary>
        /// Obtém todos os departamentos
        /// </summary>
        /// <returns>Uma lista de departamentos</returns>
        Task<IEnumerable<DepartamentoDTO>> ObterTodosAsync();

        /// <summary>
        /// Obtém um departamento por código
        /// </summary>
        /// <param name="codigo">Código do departamento</param>
        /// <returns>Um departamento</returns>
        Task<DepartamentoDTO> ObterPorCodigo(string codigo);

        /// <summary>
        /// Obtém um departamento por id
        /// </summary>
        /// <param name="departamentoId">Identificador do departamento</param>
        /// <returns>Um departamento</returns>
        Task<DepartamentoDTO> ObterPorIdAsync(int? departamentoId);

        /// <summary>
        /// Cria um departamento
        /// </summary>
        /// <param name="departamentoDTO">Modelo que será criado</param>

        Task CriarAsync(DepartamentoDTO departamentoDTO);

        /// <summary>
        /// Atualiza um departamento existente
        /// </summary>
        /// <param name="departamentoDTO">Modelo que será atualizado</param>
        Task AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO);

        /// <summary>
        /// Exclui um departamento existente por código informado
        /// </summary>
        /// <param name="codigo">Código do departamento</param>
        Task RemoverAsync(string codigo);
    }
}