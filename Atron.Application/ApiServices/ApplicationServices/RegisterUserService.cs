using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Specifications.UsuarioSpecifications;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.ApplicationServices
{
    public class RegisterUserService : IRegisterUserService
    {
        private readonly IRegisterApplicationRepository _registerApp;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly MessageModel<ApiRegister> _messageModel;
        private readonly MessageModel<Usuario> _usuarioMessageModel;

        public RegisterUserService(
            IRegisterApplicationRepository registerApp,
            IUsuarioRepository usuarioRepository,
            MessageModel<ApiRegister> messageModel,
            MessageModel<Usuario> usuarioMessageModel)
        {
            _registerApp = registerApp;
            _usuarioRepository = usuarioRepository;
            _messageModel = messageModel;
            _usuarioMessageModel = usuarioMessageModel;
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

            _messageModel.Validate(register);
            registerDTO.RegisterConfirmed = false; 

            if (!_messageModel.Messages.HasErrors())
            {
                var result = await _registerApp.RegisterUserAccountAsync(register);

                if (result)
                {
                    var usuario = new Usuario()
                    {
                        IdSequencial = registerDTO.IdSequencial,
                        Codigo = registerDTO.Codigo,
                        Nome = registerDTO.UserName,
                        Sobrenome = registerDTO.Sobrenome,
                        DataNascimento = registerDTO.DataNascimento,
                        Email = registerDTO.Email
                    };

                    _usuarioMessageModel.Validate(usuario);                    
                    var usuarioSpec = new UsuarioSpecification(usuario.Codigo, usuario.Nome, usuario.Sobrenome, usuario.Email);
                    if (!usuarioSpec.IsSatisfiedBy(usuario))
                    {
                        foreach (var error in usuarioSpec.Errors)
                        {
                            _messageModel.AddError(error);
                        }
                    }

                    if (!_usuarioMessageModel.Messages.HasErrors())
                    {
                        var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
                        if (usuarioGravado)
                        {
                            registerDTO.RegisterConfirmed = result;
                        }
                    }
                }
            }

            return registerDTO;
        }
    }
}