using Shared.Application.Interfaces.Service;
using Shared.DTO.API;
using Shared.Extensions;
using Shared.Models;
using System;

namespace Application.Validations
{
    public class InfoTokenMessageValidation : MessageModel, IMessageBaseService, IValidateModelService<DadosDoTokenDTO>
    {
        public void Validate(DadosDoTokenDTO entity)
        {
            if (entity is null)
            {
                AdicionarErro("Informações do token inválidas para processamento.");
            }

            if (entity.Token.IsNullOrEmpty())
            {
                AdicionarErro($"Token não preenchido para processamento");
            }

            //if (entity.InfoRefreshToken.IsNullOrEmpty())
            //{
            //    AddError("Refresh token não preenchido para processamento.");
            //}

            if (entity.Expires <= DateTime.Now)
            {
                AdicionarErro("Refresh token inválido ou expirado.");
            }
        }
    }
}