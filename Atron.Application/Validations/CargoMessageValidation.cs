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
            if (string.IsNullOrWhiteSpace(entity.Codigo) || string.IsNullOrWhiteSpace(entity.Descricao))
                AdicionarErro("O código ou a descrição não estão preenchidos.");

            if (entity.Codigo?.Length is < 3 or > 10)
                AdicionarErro("O código deve ter entre 3 e 10 caracteres.");

            if (entity.Descricao?.Length is < 3)
                AdicionarErro("A descrição é muito pequena.");

            if (entity.Descricao?.Length is > 50)
                AdicionarErro("A descrição é muito longa.");

            if (string.IsNullOrWhiteSpace(entity.DepartamentoCodigo))
                AdicionarErro("Código do departamento obrigatório.");
        }
    }
}