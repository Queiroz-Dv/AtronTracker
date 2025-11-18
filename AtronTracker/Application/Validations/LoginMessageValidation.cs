using Domain.ApiEntities;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using Shared.Models;

namespace Application.Validations
{
    public class LoginMessageValidation : MessageModel, IMessageBaseService, IValidateModelService<ApiLogin>
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