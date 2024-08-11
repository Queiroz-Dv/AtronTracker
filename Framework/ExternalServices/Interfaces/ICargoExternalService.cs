using Atron.Application.DTO;
using ExternalServices.Interfaces.ExternalMessage;

namespace ExternalServices.Interfaces
{
    public interface ICargoExternalService : IExternalMessageService
    {
        Task Criar(CargoDTO cargoDTO);
        Task<List<CargoDTO>> ObterTodos();
    }
}