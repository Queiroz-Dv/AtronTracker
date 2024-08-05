using Atron.Application.DTO;
using Shared.DTO;

namespace ExternalServices.Interfaces
{
    public interface ICargoExternalService
    {
        Task<(bool isSucess, List<ResultResponse> responses)> Criar(CargoDTO cargoDTO);
        Task<List<CargoDTO>> ObterTodos();
    }
}