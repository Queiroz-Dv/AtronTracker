using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.ApiServices.AuthServices.Bases;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Specifications.UsuarioSpecifications;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.ApplicationInterfaces;
using Atron.Domain.Interfaces.Identity;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.AuthServices
{
    public class RegisterUserService : ServiceBase, IRegisterUserService
    {
        private readonly IUserIdentityRepository _identityRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IValidateModel<ApiRegister> _validateModel;
        private readonly IValidateModel<Usuario> _validateUsuario;
        private readonly MessageModel _messageModel;

        public RegisterUserService(
            IServiceAccessor accessor,
            IUsuarioRepository usuarioRepository,
            IValidateModel<ApiRegister> validateModel,
            IValidateModel<Usuario> validateUsuario,
            MessageModel messageModel) : base(accessor)
        {
            _usuarioRepository = usuarioRepository;
            _messageModel = messageModel;
            _validateModel = validateModel;
            _validateUsuario = validateUsuario;
        }

        //public async Task<bool> EmailExiste(string email)
        //{
        //    return await _registerApp.UserExistsByEmail(email);            
        //}

        public async Task<UsuarioRegistroDTO> RegisterUser(UsuarioRegistroDTO registerDTO)
        {
            var register = new ApiRegister()
            {
                UserName = registerDTO.Codigo.ToUpper(),
                Email = registerDTO.Email,
                Password = registerDTO.Senha,
                ConfirmPassword = registerDTO.ConfirmaSenha
            };

            GetValidator<ApiRegister>().Validate(register);

            if (!Messages.Notificacoes.HasErrors())
            {
                var contaExiste = await _identityRepository.ContaExisteRepositoryAsync(register.UserName, register.Email); //await _registerApp.UserExists(register);
                if (contaExiste)
                {
                    _messageModel.AddError("Usuário já cadastrado.");
                    return null;
                }

                var result = await _identityRepository.RegistrarContaDeUsuarioRepositoryAsync(register.UserName, register.Email, register.Password);

                if (result)
                {
                    var usuario = new Usuario()
                    {
                        Codigo = registerDTO.Codigo.ToUpper(),
                        Nome = registerDTO.Nome,
                        Sobrenome = registerDTO.Sobrenome,
                        DataNascimento = registerDTO.DataNascimento,
                        Email = registerDTO.Email
                    };

                    _validateUsuario.Validate(usuario);

                    // Trocar validação por apenas código e email
                    var usuarioSpec = new UsuarioSpecification(usuario.Codigo,usuario.Email);
                    if (!usuarioSpec.IsSatisfiedBy(usuario))
                    {
                        usuarioSpec.Errors.ForEach(error => _messageModel.AddError(error));
                    }

                    if (!_messageModel.Notificacoes.HasErrors())
                    {
                        var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
                        if (usuarioGravado)
                        {
                            _messageModel.AddMessage($"Usuário: {usuario.Codigo} - {usuario.Nome} registrado e cadastrado com sucesso.");
                        }
                    }
                }
            }

            return registerDTO;
        }

        //public async Task<bool> UserExists(string code)
        //{
        //    return await _registerApp.UserExistsByUserCode(code);
        //}
    }
}