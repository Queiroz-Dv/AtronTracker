using Domain.Entities;
using Shared.Interfaces.Services;
using Shared.Interfaces.Validations;
using Shared.Models;
using System.Linq;

namespace Application.Validations
{
    public class PerfilDeAcessoMessageValidation : MessageModel, IMessages, IValidateModel<PerfilDeAcesso>
    {
        public void Validate(PerfilDeAcesso entity)
        {
            if (string.IsNullOrEmpty(entity.Descricao) ||
                 string.IsNullOrEmpty(entity.Codigo))
            {
                AdicionarErro("O código ou a descrição não estão preenchidos.");
            }

            if (entity.Codigo.Length > 10)
            {
                AdicionarErro("O código informado é muito longo.");
            }

            if (entity.Codigo.Length < 3)
            {
                AdicionarErro("O código informado é muito pequeno.");
            }

            if (entity.Descricao.Length < 3)
            {
                AdicionarErro("A descricao é muito pequena.");
            }

            if (entity.Descricao.Length > 50)
            {
                AdicionarErro("A descricao é muito longa.");
            }

            if (!entity.PerfilDeAcessoModulos.Any())
            {
                AdicionarErro("Não contém nenhum módulo para relacionar ao perfil criado.");
            }
        }
    }
}