using Atron.Application.DTO.Account;
using ExternalServices.Interfaces;

namespace ExternalServices.Services
{
    public class AuthenticateExternalService : IAuthenticateExternalService
    {
        public Task<bool> Authenticate(LoginDTO loginDTO)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }

        public Task<bool> RegisterUser(RegisterDTO registerDTO)
        {
            throw new NotImplementedException();
        }
    }
}
