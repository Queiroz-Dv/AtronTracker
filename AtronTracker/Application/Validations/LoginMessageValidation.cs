using Domain.ApiEntities;
using Shared.Extensions;
using Shared.Interfaces.Services;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Application.Validations
{
    public class LoginMessageValidation : MessageModel, IMessages, IValidateModel<ApiLogin>
    {
        public void Validate(ApiLogin entity)
        {
            if (entity.UserName.IsNullOrEmpty())
            {
                AdicionarErro("E-mail vazio");
            }


            if (entity.Password.IsNullOrEmpty())
            {
                AdicionarErro("Senha vazia.");
            }
        }
    }
}