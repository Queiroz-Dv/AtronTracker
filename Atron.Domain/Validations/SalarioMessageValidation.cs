using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Models;
using System;

namespace Atron.Domain.Validations
{
    //TODO: Fazer as validaçãoes para a entidade de salário
    public class SalarioMessageValidation : MessageModel<Salario>, IMessages
    {
        public override void Validate(Salario entity)
        {
            return;
        }
    }
}
