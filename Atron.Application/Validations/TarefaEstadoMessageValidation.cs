using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;

namespace Atron.Application.Validations
{
    public class TarefaEstadoMessageValidation : MessageModel, IMessages, IValidateModel<TarefaEstado>
    {
        public void Validate(TarefaEstado entity)
        {
            throw new NotImplementedException();
        }
    }
}
