using Domain.ApiEntities;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace Application.Validations
{
    public class LoginMessageValidation : Notifiable, IMessageBaseService, IValidateModelService<ApiLogin>
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