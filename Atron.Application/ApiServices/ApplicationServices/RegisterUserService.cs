using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.Account;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IRegisterApplicationRepository _registerApp;
        private readonly IUsuarioRepository _usuarioRepository;

        public RegisterUserService(IRegisterApplicationRepository registerApp,
            IUsuarioRepository usuarioRepository)
        {
            _registerApp = registerApp;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<RegisterDTO> RegisterUser(RegisterDTO registerDTO)
        {
            var register = new ApiRegister()
            {
                UserName = registerDTO.UserName,
                Email = registerDTO.Email,
                Password = registerDTO.Passsword,
                ConfirmPassword = registerDTO.ConfirmPasssword
            };

            var result = await _registerApp.RegisterUserAccountAsync(register);

            registerDTO.RegisterConfirmed = result;

            if (result)
            {
                var usuario = new Usuario()
                {
                    Codigo = registerDTO.Codigo,
                    Nome = registerDTO.UserName,
                    Sobrenome = registerDTO.Sobrenome,
                    DataNascimento = registerDTO.DataNascimento,
                    Email = registerDTO.Email
                };

                await _usuarioRepository.CriarUsuarioAsync(usuario);               
            }

            return registerDTO;
        }
    }
}