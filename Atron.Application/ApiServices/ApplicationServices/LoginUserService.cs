using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces;
using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Shared.Interfaces;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class LoginUserService : ILoginUserService
    {
        private readonly IApplicationTokenService _tokenService;
        private readonly ILoginApplicationRepository _loginApplication;
        private readonly IUsuarioService _usuarioService;

        public LoginUserService(ILoginApplicationRepository loginApplication, IApplicationTokenService tokenService, IUsuarioService usuarioService)
        {
            _loginApplication = loginApplication;
            _tokenService = tokenService;
            _usuarioService = usuarioService;
        }

        public async Task<LoginDTO> Authenticate(LoginDTO loginDTO)
        {
            var login = new ApiLogin()
            {
                UserName = loginDTO.Codigo,
                Password = loginDTO.Passsword
            };

            var result = await _loginApplication.AuthenticateUserLoginAsync(login);

            loginDTO.Authenticated = result;

            if (loginDTO.Authenticated)
            {
                // Get user data
                var user = await _usuarioService.ObterPorCodigoAsync(loginDTO.Codigo);

                // Generate token
                var userToken = _tokenService.GenerateToken(user.Nome, loginDTO.Codigo, user.Email);

                // Set token
                loginDTO.UserToken = userToken;
            }

            return loginDTO;
        }

        public async Task Logout()
        {
            await _loginApplication.Logout();
        }
    }
}