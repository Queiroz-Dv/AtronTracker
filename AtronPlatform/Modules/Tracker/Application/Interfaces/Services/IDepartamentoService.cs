using Application.DTO;
using System.Collections.Generic;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IDepartamentoService
    {
        /// <summary>
        /// Obtém todos os departamentos
        /// </summary>
        /// <returns>Uma lista de departamentos</returns>
        Task<Resultado<List<DepartamentoDTO>>> ObterTodosAsync();

        /// <summary>
        /// Obtém um departamento por código
        /// </summary>
        /// <param name="codigo">Código do departamento</param>
        /// <returns>Um departamento</returns>
        Task<Resultado<DepartamentoDTO>> ObterPorCodigo(string codigo);

        /// <summary>
        /// Obtém um departamento por id
        /// </summary>
        /// <param name="departamentoId">Identificador do departamento</param>
        /// <returns>Um departamento</returns>
        Task<Resultado<DepartamentoDTO>> ObterPorIdAsync(int? departamentoId);

        /// <summary>
        /// Cria um departamento
        /// </summary>
        /// <param name="departamentoDTO">Modelo que será criado</param>

        Task<Resultado<DepartamentoDTO>> CriarAsync(DepartamentoDTO departamentoDTO);

        /// <summary>
        /// Atualiza um departamento existente
        /// </summary>
        /// <param name="departamentoDTO">Modelo que será atualizado</param>
        Task<Resultado<DepartamentoDTO>> AtualizarAsync(string codigo, DepartamentoDTO departamentoDTO);

        /// <summary>
        /// Exclui um departamento existente por código informado
        /// </summary>
        /// <param name="codigo">Código do departamento</param>
        Task<Resultado> RemoverAsync(string codigo);
    }
}