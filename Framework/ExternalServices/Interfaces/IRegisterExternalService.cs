using Atron.Application.DTO.ApiDTO;

namespace ExternalServices.Interfaces
{
    public interface IRegisterExternalService : IExternalService<RegisterDTO>
    {
        Task<bool> EmailExiste(string email);
        Task Registrar(RegisterDTO registerDTO);
        Task<bool> UsuarioExiste(string codigo);
    }
}