using Domain.ApiEntities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
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
                AdicionarErro(AuthResource.Erro_EmailVazio);
            }


            if (entity.Password.IsNullOrEmpty())
            {
                AdicionarErro(AuthResource.Erro_SenhaVazia);
            }
        }
    }
}
