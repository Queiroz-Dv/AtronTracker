using Domain.Entities;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;

namespace Application.Validador
{
    public class UsuarioValidador(IAccessorService accessorService) : IValidador<Usuario>
    {
        private readonly IAccessorService _accessorService = accessorService;

        public IList<NotificationMessage> Validar(Usuario entity)
        {
            var context = new NotificationBag();

            if (entity == null)
            {
                context.AdicionarErro(UsuarioResource.ErroUsuarioNulo);
                return [.. context.Messages];
            }

            ValidarCodigo(entity, context);
            ValidarNome(entity, context);
            ValidarSobrenome(entity, context);
            ValidarDataNascimento(entity, context);
            ValidarEmail(entity, context);

            return [.. context.Messages];
        }

        private void ValidarCodigo(Usuario entity, NotificationBag context)
        {
            if (entity.Codigo.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoNulo);
            }
            else if (entity.Codigo.Length > 10)
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoLongo);
            }
            else if (entity.Codigo.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroCodigoPequeno);
            }
        }

        private void ValidarNome(Usuario entity, NotificationBag context)
        {
            if (entity.Nome.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroNomeUsuarioNulo);
            }
            else if (entity.Nome.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroNomePequeno);
            }
            else if (entity.Nome.Length > 25)
            {
                context.AdicionarErro(UsuarioResource.ErroNomeLongo);
            }
        }

        private void ValidarSobrenome(Usuario entity, NotificationBag context)
        {
            if (entity.Sobrenome.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomeObrigatorio);
            }
            else if (entity.Sobrenome.Length < 3)
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomePequeno);
            }
            else if (entity.Sobrenome.Length > 50)
            {
                context.AdicionarErro(UsuarioResource.ErroSobrenomeLongo);
            }
        }

        private void ValidarDataNascimento(Usuario entity, NotificationBag context)
        {
            if (entity.DataNascimento == DateTime.Now)
            {
                context.AdicionarErro(UsuarioResource.ErroDataDeNascimento);
            }
        }

        private void ValidarEmail(Usuario entity, NotificationBag context)
        {
            if (entity.Email.IsNullOrEmpty())
            {
                context.AdicionarErro(UsuarioResource.ErroEmailNulo);
                return;
            }

            var usuarioRepository = _accessorService.ObterService<IUsuarioRepository>();
            var emailExiste = usuarioRepository.VerificarEmailExistenteAsync(entity.Email).Result;
            if (emailExiste)
            {
                context.AdicionarErro(EmailResource.ErroEmailUtilizado);
            }
        }
    }
}