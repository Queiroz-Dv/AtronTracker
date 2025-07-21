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