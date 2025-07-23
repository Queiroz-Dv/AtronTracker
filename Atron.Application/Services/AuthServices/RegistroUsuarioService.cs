﻿using Atron.Application.DTO.ApiDTO;
using Atron.Application.Interfaces.ApplicationInterfaces;
using Atron.Application.Services.AuthServices.Bases;
using Atron.Application.Specifications.UsuarioSpecifications;
using Atron.Domain.ApiEntities;
using Atron.Domain.Entities;
using Atron.Domain.Interfaces;
using Atron.Domain.Interfaces.Identity;
using Atron.Domain.Interfaces.UsuarioInterfaces;
using Shared.Extensions;
using Shared.Interfaces.Accessor;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace Atron.Application.Services.AuthServices
{
    public class RegistroUsuarioService : ServiceBase, IRegistroUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPerfilDeAcessoRepository perfilDeAcessoRepository;
        private readonly IPerfilDeAcessoUsuarioRepository _perfilDeAcessoUsuarioRepository;
        private readonly MessageModel _messageModel;

        public RegistroUsuarioService(
            IServiceAccessor accessor,
            IUsuarioRepository usuarioRepository,
            MessageModel messageModel,
            IPerfilDeAcessoUsuarioRepository perfilDeAcessoUsuarioRepository,
            IPerfilDeAcessoRepository perfilDeAcessoRepository) : base(accessor)
        {
            _usuarioRepository = usuarioRepository;
            _messageModel = messageModel;
            _perfilDeAcessoUsuarioRepository = perfilDeAcessoUsuarioRepository;
            this.perfilDeAcessoRepository = perfilDeAcessoRepository;
        }

        public async Task RegistrarUsuario(UsuarioRegistroDTO usuarioRegistroDTO)
        {
            var _usuarioIdentityRepository = ObterService<IUsuarioIdentityRepository>();

            var usuarioRegistro = new UsuarioRegistro()
            {
                CodigoDeAcesso = usuarioRegistroDTO.Codigo.ToUpper(),
                Email = usuarioRegistroDTO.Email,
                Senha = usuarioRegistroDTO.Senha,
                ConfirmarSenha = usuarioRegistroDTO.ConfirmaSenha
            };

            GetValidator<UsuarioRegistro>().Validate(usuarioRegistro);

            if (!Messages.Notificacoes.HasErrors())
            {
                var contaExiste = await _usuarioIdentityRepository.ContaExisteRepositoryAsync(usuarioRegistro.CodigoDeAcesso, usuarioRegistro.Email);

                if (contaExiste)
                {
                    _messageModel.AdicionarErro("Usuário já cadastrado.");
                    return;
                }

                var registrado = await _usuarioIdentityRepository.RegistrarContaDeUsuarioRepositoryAsync(usuarioRegistro.CodigoDeAcesso, usuarioRegistro.Email, usuarioRegistro.Senha);

                if (registrado)
                {
                    var usuario = new Usuario()
                    {
                        Codigo = usuarioRegistroDTO.Codigo.ToUpper(),
                        Nome = usuarioRegistroDTO.Nome,
                        Sobrenome = usuarioRegistroDTO.Sobrenome,                 
                        DataNascimento = usuarioRegistroDTO.DataNascimento?.ToDateTime(TimeOnly.MinValue),
                        Email = usuarioRegistroDTO.Email
                    };

                    GetValidator<Usuario>().Validate(usuario);

                    // Trocar validação por apenas código e email
                    var usuarioSpec = new UsuarioSpecification(usuario.Codigo, usuario.Email);
                    if (!usuarioSpec.IsSatisfiedBy(usuario))
                    {
                        usuarioSpec.Errors.ForEach(_messageModel.AdicionarErro);
                    }

                    if (!_messageModel.Notificacoes.HasErrors())
                    {
                        var usuarioGravado = await _usuarioRepository.CriarUsuarioAsync(usuario);
                        if (usuarioGravado)
                        {
                            var perfilDeAcesso = await perfilDeAcessoRepository.ObterPerfilPorCodigoRepositoryAsync(usuarioRegistroDTO.CodigoPerfilDeAcesso);
                            var usuarioBd = await _usuarioRepository.ObterUsuarioPorCodigoAsync(usuario.Codigo);

                            if (perfilDeAcesso != null)
                            {
                                var perfilDeAcessoUsuario = new PerfilDeAcessoUsuario()
                                {
                                    PerfilDeAcessoId = perfilDeAcesso.Id,
                                    PerfilDeAcessoCodigo = perfilDeAcesso.Codigo,
                                    UsuarioId = usuarioBd.Id,
                                    UsuarioCodigo = usuarioBd.Codigo
                                };

                                await _perfilDeAcessoUsuarioRepository.CriarPerfilRepositoryAsync(perfilDeAcessoUsuario);
                            }

                            _messageModel.AdicionarMensagem($"Usuário {usuario.Nome } {usuario.Sobrenome}: com o código de acesso {usuario.Codigo} registrado e cadastrado com sucesso.");
                        }
                    }
                }
            }

            return;
        }
    }
}