using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly ILoginApplicationRepository _loginApplication;

        public LoginUserService(ILoginApplicationRepository loginApplication)
        {
            _loginApplication = loginApplication;
        }

        public async Task<LoginDTO> Authenticate(LoginDTO loginDTO)
        {
            var login = new ApiLogin()
            {
                Email = loginDTO.Email,
                Password = loginDTO.Passsword
            };

            var result = await _loginApplication.AuthenticateUserLoginAsync(login);

            loginDTO.Authenticated = result;

            if (loginDTO.ReturnUrl.IsNullOrEmpty())
            {
                loginDTO.ControllerName = "Home";
                loginDTO.ControllerAction = nameof(Index);
            }

            return loginDTO;
        }

        public async Task Logout()
        {
            await _loginApplication.Logout();
        }
    }
}