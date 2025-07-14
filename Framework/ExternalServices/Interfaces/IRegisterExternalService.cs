using Atron.Application.DTO.ApiDTO;

namespace ExternalServices.Interfaces
{
    public interface IRegisterExternalService : IExternalService<UsuarioRegistroDTO>
    {
        Task<bool> EmailExiste(string email);
        Task Registrar(UsuarioRegistroDTO registerDTO);
        Task<bool> UsuarioExiste(string codigo);
    }
}