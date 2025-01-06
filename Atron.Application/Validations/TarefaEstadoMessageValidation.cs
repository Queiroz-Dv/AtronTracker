using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Models;
using System;

namespace Atron.Application.Validations
{
    public class TarefaEstadoMessageValidation : MessageModel<TarefaEstado>, IMessages
    {
        public override void Validate(TarefaEstado entity)
        {
            throw new NotImplementedException();
        }
    }
}
