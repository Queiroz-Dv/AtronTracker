using Atron.Application.DTO.ApiDTO;

namespace ExternalServices.Interfaces
{
    public interface IRegisterExternalService
    {
        Task Registrar(RegisterDTO registerDTO);
    }
}