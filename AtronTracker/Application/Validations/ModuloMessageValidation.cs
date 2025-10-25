using Domain.Entities;
using Shared.Interfaces.Services;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Application.Validations
{
    public class ModuloMessageValidation : MessageModel, IMessages, IValidateModel<Modulo>
    {
        public void Validate(Modulo entity)
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
        }
    }
}