using Application.DTO.Request;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Application.Validador
{
    public class UsuarioRegistroValidador(IAccessorService accessorService) : IValidador<UsuarioRegistroRequest>
    {
        private readonly IAccessorService _accessorService = accessorService;

        public IList<NotificationMessage> Validar(UsuarioRegistroRequest entity)
        {
            var context = new NotificationBag();

            if (entity == null)
            {
                context.AdicionarErro("Registro nulo");
                return [.. context.Messages];
            }

            ValidarCodigo(entity, context);
            ValidarNome(entity, context);
            ValidarSobrenome(entity, context);
            ValidarDataNascimento(entity, context);
            ValidarEmail(entity, context);
            ValidarSenha(entity, context);

            return [.. context.Messages];
        }

        private static void ValidarCodigo(UsuarioRegistroRequest entity, NotificationBag context)
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

        private void ValidarNome(UsuarioRegistroRequest entity, NotificationBag context)
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

        private void ValidarSobrenome(UsuarioRegistroRequest entity, NotificationBag context)
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

        private void ValidarDataNascimento(UsuarioRegistroRequest entity, NotificationBag context)
        {
            if (entity.DataNascimento >= DateOnly.FromDateTime(DateTime.Now))
            {
                context.AdicionarErro(UsuarioResource.ErroDataDeNascimento);
            }
        }

        private void ValidarEmail(UsuarioRegistroRequest entity, NotificationBag context)
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

        private void ValidarSenha(UsuarioRegistroRequest entity, NotificationBag context)
        {
            if (entity.Senha.IsNullOrEmpty())
            {
                context.AdicionarErro("Senha vazia ou não informada.");
            }
            else
            {
                var senha = entity.Senha;

                if (entity.Senha.Length < 9)
                {
                    context.AdicionarErro("A senha deve conter mais de 8 caracteres");
                }

                if (!Regex.IsMatch(senha, @"[a-z]") || !Regex.IsMatch(senha, @"[A-Z]") || !Regex.IsMatch(senha, @"[0-9]"))
                {
                    context.AdicionarErro("A senha deve conter letras maiúsculas, minúsculas e pelo menos um número.");
                }

                if (!Regex.IsMatch(senha, @"[!@#$%^&*(),.?""{}|<>]"))
                {
                    context.AdicionarErro("A senha deve conter pelo menos um caractere especial.");
                }

                if (!entity.Senha.Equals(entity.ConfirmaSenha))
                {
                    context.AdicionarErro("As senhas não são iguais");
                }
            }
        }
    }
}