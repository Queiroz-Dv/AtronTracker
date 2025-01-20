using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;

namespace Atron.Application.Validations
{
    //TODO: Fazer as validaçãoes para a entidade de salário
    public class SalarioMessageValidation : MessageModel, IMessages, IValidateModel<Salario>
    {
        public void Validate(Salario entity)
        {
            return;
        }
    }
}
