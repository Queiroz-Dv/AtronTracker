using AtronStock.Application.DTO.Request;

namespace AtronStock.Application.Interfaces
{
    public interface IClienteService
    {
        Task CriarAsync(ClienteRequest request);
    }
}