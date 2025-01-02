using Atron.Application.DTO.Account;

namespace ExternalServices.Interfaces
{
    public interface IAuthenticateExternalService
    {
        Task<bool> Authenticate(LoginDTO loginDTO);

        Task<bool> RegisterUser(RegisterDTO registerDTO);

        Task Logout();
    }
}
