using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.Account;
using Atron.Domain.ApiEntities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IRegisterApplicationRepository _registerApp;

        public RegisterUserService(IRegisterApplicationRepository registerApp)
        {
            _registerApp = registerApp;
        }

        public async Task<RegisterDTO> RegisterUser(RegisterDTO registerDTO)
        {
            var register = new Register()
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                Password = registerDTO.Passsword,
            };

            var result = await _registerApp.RegisterUserAccountAsync(register);

            registerDTO.RegisterConfirmed = result;

            return registerDTO;
        }
    }
}