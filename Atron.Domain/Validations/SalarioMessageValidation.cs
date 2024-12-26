using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Models;
using System;

namespace Atron.Domain.Validations
{
    public class SalarioMessageValidation : MessageModel<Salario>, IMessages
    {
        public override void Validate(Salario entity)
        {
            throw new NotImplementedException();
        }
    }
}
