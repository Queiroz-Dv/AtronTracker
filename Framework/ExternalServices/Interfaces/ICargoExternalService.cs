using Atron.Application.DTO;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces
{
    public interface ICargoExternalService : IExternalMessageService
    {
        Task Atualizar(string codigo, CargoDTO cargoDTO);
        Task Criar(CargoDTO cargoDTO);
        Task<List<CargoDTO>> ObterTodos();
        Task Remover(string codigo);
    }
}