using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Specifications.UsuarioSpecifications;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IRegisterApplicationRepository _registerApp;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidateModel<ApiRegister> _validateModel;
        private readonly IValidateModel<Usuario> _validateUsuario;
        private readonly MessageModel _messageModel;

        public RegisterUserService(
            IValidateModel<ApiRegister> validateModel,
            IRegisterApplicationRepository registerApp,
            IUsuarioRepository usuarioRepository,
            MessageModel messageModel,
            IValidateModel<Usuario> validateUsuario)
        {
            _registerApp = registerApp;
            _usuarioRepository = usuarioRepository;
            _messageModel = messageModel;
            _validateModel = validateModel;
            _validateUsuario = validateUsuario;
        }

        public async Task<RegisterDTO> RegisterUser(RegisterDTO registerDTO)
        {
            var register = new ApiRegister()
            {                
                UserName = registerDTO.Codigo,
                Email = registerDTO.Email,
                Password = registerDTO.Senha,
                ConfirmPassword = registerDTO.ConfirmaSenha
            };

            _validateModel.Validate(register);
            registerDTO.Registrado = false; 

            if (!_messageModel.Messages.HasErrors())
            {
                var result = await _registerApp.RegisterUserAccountAsync(register);

                if (result)
                {
                    var usuario = new Usuario()
                    {                      
                        Codigo = registerDTO.Codigo,
                        Nome = registerDTO.Nome,
                        Sobrenome = registerDTO.Sobrenome,
                        DataNascimento = registerDTO.DataNascimento,
                        Email = registerDTO.Email
                    };

                    _validateUsuario.Validate(usuario);                    
                    var usuarioSpec = new UsuarioSpecification(usuario.Codigo, usuario.Nome, usuario.Sobrenome, usuario.Email);

                    if (!usuarioSpec.IsSatisfiedBy(usuario))
                    {
                        foreach (var error in usuarioSpec.Errors)
                        {
                            _messageModel.AddError(error);
                        }
                    }

                    if (!_messageModel.Messages.HasErrors())
                    {
                        var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
                        if (usuarioGravado)
                        {
                            _messageModel.AddMessage("Usuário registrado e cadastrado com sucesso.");
                            registerDTO.Registrado = result;
                        }
                    }
                }
            }

            return registerDTO;
        }
    }
}