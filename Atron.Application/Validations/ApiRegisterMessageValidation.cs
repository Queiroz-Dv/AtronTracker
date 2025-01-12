using Atron.Domain.ApiEntities;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class ApiRegisterMessageValidation : MessageModel<ApiRegister>, IMessages
    {
        public override void Validate(ApiRegister entity)
        {
            if (entity.UserName.IsNullOrEmpty())
            {
                AddError("Nome de usuário vazio ou não informado.");
            }

            if (entity.Password.IsNullOrEmpty())
            {
                AddError("Senha vazia ou não informada.");
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