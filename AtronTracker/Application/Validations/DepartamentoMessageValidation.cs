using Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using Shared.Models;

namespace Application.Validations
{
    public class DepartamentoMessageValidation : MessageModel, IMessageBaseService, IValidateModelService<Departamento>
    {
        public void Validate(Departamento entity)
        {
            if (entity.Descricao.IsNullOrEmpty() ||
                entity.Codigo.IsNullOrEmpty())
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