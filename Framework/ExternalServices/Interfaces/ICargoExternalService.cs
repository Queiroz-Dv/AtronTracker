using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    /// <summary>
    /// Interface dos processos e fluxos do módulo de Cargos
    /// </summary>
    public interface ICargoExternalService
    {
        /// <summary>
        /// Método que atualiza um cargo existente
        /// </summary>
        /// <param name="codigo">Código do cargo</param>
        /// <param name="cargoDTO">DTO que será enviado para atualização</param>
        Task Atualizar(string codigo, CargoDTO cargoDTO);

        /// <summary>
        /// Método que cria um cargo
        /// </summary>
        /// <param name="cargoDTO">DTO que será enviado para criação</param>
        Task Criar(CargoDTO cargoDTO);

        /// <summary>
        /// Método que obtém todos os cargos
        /// </summary>
        /// <returns>Retorna uma lista de cargos</returns>
        Task<List<CargoDTO>> ObterTodos();

        Task<CargoDTO> ObterPorCodigo(string codigo);

        /// <summary>
        /// Método que remove um cargo por código
        /// </summary>
        /// <param name="codigo">Código do cargo informado</param>        
        Task Remover(string codigo);
    }
}