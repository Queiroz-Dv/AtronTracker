using Application.DTO.ApiDTO;

namespace ExternalServices.Interfaces
{
    public interface ILoginExternalService
    {
        Task<LoginDTO> Autenticar(LoginDTO loginDTO);

        Task Logout();
    }
}
