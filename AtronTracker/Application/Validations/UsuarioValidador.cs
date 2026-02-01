using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Validations
{
    public class UsuarioValidador : IValidador<Usuario>
    {
        private readonly IAccessorService _accessorService;

        public UsuarioValidador(IAccessorService accessorService)
        {
            _accessorService = accessorService;
        }

        public IList<NotificationMessage> Validar(Usuario entity)
        {
            var context = new NotificationBag();

            if (entity == null)
            {
                context.AdicionarErro(UsuarioResource.ErroUsuarioNulo);
                return [.. context.Messages];
            }

            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoNulo);
            }

            if (entity.Codigo.Length > 10)
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoLongo);
            }

            if (entity.Codigo.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoPequeno);
            }

            if (entity.Nome.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroNomeUsuarioNulo);
            }

            if (entity.Nome.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroNomePequeno);
            }

            if (entity.Nome.Length > 25)
            {
                context.AdicionarErro(UsuarioResource.ErroNomeLongo);
            }

            if (entity.Sobrenome.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomeObrigatorio);
            }

            if (entity.Sobrenome.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomePequeno);
            }

            if (entity.Sobrenome.Length > 50)
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomeLongo);
            }

            if (entity.DataNascimento == DateTime.Now)
            {
                context.AdicionarErro(UsuarioResource.ErroDataDeNascimento);
            }

            if (entity.Email.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroEmailNulo);
            }

            if (!entity.Email.IsNullOrEmpty())
            {
                var _usuarioRepository = _accessorService.ObterService<IUsuarioRepository>();
                var emailExiste = _usuarioRepository.VerificarEmailExistenteAsync(entity.Email).Result;
                if (emailExiste)
                {
                    context.AdicionarErro(EmailResource.ErroEmailUtilizado);
                }
            }

            return [.. context.Messages];
        }
    }
}