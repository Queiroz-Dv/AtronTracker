using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;

namespace Application.Validations
{
    public class InfoTokenMessageValidation : Notifiable, IMessageBaseService, IValidateModelService<DadosDoTokenDTO>
    {
        public void Validate(DadosDoTokenDTO entity)
        {
            if (entity != null)
            {
                if (entity.Value.IsNullOrEmpty())
                {
                    AdicionarErro(AuthResource.Erro_TokenNaoPreenchido);
                }

                //if (entity.InfoRefreshToken.IsNullOrEmpty())
                //{
                //    AddError("Refresh token não preenchido para processamento.");
                //}

                if (entity.Expires <= DateTime.Now)
                {
                    AdicionarErro(AuthResource.Erro_RefreshTokenInvalido);
                }
            }
            else
            {
                AdicionarErro(AuthResource.Erro_InformacoesTokenInvalidas);

            }


        }
    }
}
