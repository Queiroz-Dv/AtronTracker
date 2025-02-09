using Atron.Domain.ApiEntities;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class LoginMessageValidation : MessageModel, IMessages, IValidateModel<ApiLogin>
    {
        public void Validate(ApiLogin entity)
        {
            if (entity.UserName.IsNullOrEmpty())
            {
                AddError("E-mail vazio");
            }


            if (entity.Password.IsNullOrEmpty())
            {
                AddError("Senha vazia.");
            }
        }
    }
}