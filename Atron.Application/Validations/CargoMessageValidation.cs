using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class CargoMessageValidation : MessageModel, IMessages, IValidateModel<Cargo>
    {
        public void Validate(Cargo entity)
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

            if (string.IsNullOrEmpty(entity.DepartamentoCodigo))
            {
                AdicionarErro("Código do departamento obrigatório.");
            }
        }
    }
}