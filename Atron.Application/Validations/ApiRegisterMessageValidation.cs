using Atron.Domain.ApiEntities;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Text.RegularExpressions;

namespace Atron.Application.Validations
{
    public class ApiRegisterMessageValidation : MessageModel, IMessages, IValidateModel<ApiRegister>
    {
        public void Validate(ApiRegister entity)
        {
            var senha = entity.Password;

            if (entity.UserName.IsNullOrEmpty())
            {
                AddError("Nome de usuário vazio ou não informado.");
            }

            if (entity.Password.IsNullOrEmpty())
            {
                AddError("Senha vazia ou não informada.");
            }

            if (entity.Password.Length < 9)
            {
                AddError("A senha deve conter mais de 8 caracteres");
            }

            if (!(Regex.IsMatch(senha, "a-z") && Regex.IsMatch(senha, "A-Z") && Regex.IsMatch(senha, "[0-9]")))
            {
                AddError("A senha deve conter letras maiúsculas, minúsculas e pelo menos um número.");
            }
            
            if(!Regex.IsMatch(senha, @"[!@#$%^&*(),.?""{}|<>]"))
            {
                AddError("A senha deve conter pelo menos um caractere especial.");
            }

            if (entity.Email.IsNullOrEmpty())
            {
                AddError("Endereço de e-mail vazio ou não informado.");
            }

            if (!entity.Password.Equals(entity.ConfirmPassword))
            {
                AddError("As senhas não são iguais");
            }

            if (entity.UserName.Length > 50)
            {
                AddError("Quantidade de caracteres para o nome de usuário excedido. Tamanho máximo de 50 caracteres.");
            }

            if (entity.Email.Length > 25)
            {
                AddError("Quantidade de caracteres para o email de usuário excedido. Tamanho máximo de 25 caracteres.");
            }
        }
    }
}