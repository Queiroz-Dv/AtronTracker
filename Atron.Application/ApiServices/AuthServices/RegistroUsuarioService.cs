using Atron.Application.ApiInterfaces.ApplicationInterfaces;
using Atron.Application.ApiServices.AuthServices.Bases;
using Atron.Application.DTO.ApiDTO;
using Atron.Application.Specifications.UsuarioSpecifications;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces.Identity;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.Application.ApiServices.AuthServices
{
    public class RegistroUsuarioService : ServiceBase, IRegistroUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly MessageModel _messageModel;

        public RegistroUsuarioService(
            IServiceAccessor accessor,
            IUsuarioRepository usuarioRepository,
            MessageModel messageModel) : base(accessor)
        {
            _usuarioRepository = usuarioRepository;
            _messageModel = messageModel;
        }

        public async Task<UsuarioRegistroDTO> RegistrarUsuario(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            var _usuarioIdentityRepository = ObterService<IUsuarioIdentityRepository>();

            var usuarioRegistro = new UsuarioRegistro()
            {
                UserName = usuarioRegistroDTO.Codigo.ToUpper(),
                Email = usuarioRegistroDTO.Email,
                Password = usuarioRegistroDTO.Senha,
                ConfirmPassword = usuarioRegistroDTO.ConfirmaSenha
            };

            GetValidator<UsuarioRegistro>().Validate(usuarioRegistro);

            if (!Messages.Notificacoes.HasErrors())
            {
                var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(usuarioRegistro.UserName, usuarioRegistro.Email);

                if (contaExiste)
                {
                    _messageModel.AddError("Usuário já cadastrado.");
                    return null;
                }

                bool registrado; 

                try
                {
                     registrado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(usuarioRegistro.UserName, usuarioRegistro.Email, usuarioRegistro.Password);
                }
                catch (Exception ex)
                {

                    throw ex;
                }

                if (registrado)
                {
                    var usuario = new Usuario()
                    {
                        Codigo = usuarioRegistroDTO.Codigo.ToUpper(),
                        Nome = usuarioRegistroDTO.Nome,
                        Sobrenome = usuarioRegistroDTO.Sobrenome,
                        // Fixing the CS0029 error by converting 'DateOnly?' to 'DateTime?'
                        DataNascimento = usuarioRegistroDTO.DataNascimento?.ToDateTime(TimeOnly.MinValue),
                        Email = usuarioRegistroDTO.Email
                    };

                    GetValidator<Usuario>().Validate(usuario);

                    // Trocar validação por apenas código e email
                    var usuarioSpec = new UsuarioSpecification(usuario.Codigo, usuario.Email);
                    if (!usuarioSpec.IsSatisfiedBy(usuario))
                    {
                        usuarioSpec.Errors.ForEach(_messageModel.AddError);
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

            return usuarioRegistroDTO;
        }
    }
}