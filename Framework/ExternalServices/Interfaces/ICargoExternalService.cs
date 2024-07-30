using Atron.Application.DTO;

namespace ExternalServices.Interfaces
{
    public interface ICargoExternalService
    {
        Task<List<CargoDTO>> ObterTodos();
    }
}