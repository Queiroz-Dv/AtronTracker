using Atron.Domain.ApiEntities;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class LoginMessageValidation : MessageModel<ApiLogin>, IMessages
    {
        public override void Validate(ApiLogin entity)
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