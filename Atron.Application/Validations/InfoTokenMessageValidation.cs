using Shared.DTO.API;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;

namespace Atron.Application.Validations
{
    public class InfoTokenMessageValidation : MessageModel, IMessages, IValidateModel<DadosDoTokenDTO>
    {
        public void Validate(DadosDoTokenDTO entity)
        {
            if (entity is null)
            {
                AddError("Informações do token inválidas para processamento.");
            }

            if (entity.Token.IsNullOrEmpty())
            {
                AddError($"Token não preenchido para processamento");
            }

            if (entity.InfoRefreshToken.IsNullOrEmpty())
            {
                AddError("Refresh token não preenchido para processamento.");
            }

            if (entity.RefreshTokenExpireTime <= DateTime.Now)
            {
                AddError("Refresh token inválido ou expirado.");
            }
        }
    }
}