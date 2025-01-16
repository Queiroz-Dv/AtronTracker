﻿using Atron.Domain.Entities;
using Shared.Interfaces;
using Shared.Models;

namespace Atron.Application.Validations
{
    public class DepartamentoMessageValidation : MessageModel<Departamento>, IMessages
    {
        public override void Validate(Departamento entity)
        {
            if (string.IsNullOrEmpty(entity.Descricao) ||
               string.IsNullOrEmpty(entity.Codigo))
            {
                AddError("O código ou a descrição não estão preenchidos.");
            }

            if (entity.Codigo.Length > 10)
            {
                AddError("O código informado é muito longo.");
            }

            if (entity.Codigo.Length < 3)
            {
                AddError("O código informado é muito pequeno.");
            }

            if (entity.Descricao.Length < 3)
            {
                AddError("A descricao é muito pequena.");
            }

            if (entity.Descricao.Length > 50)
            {
                AddError("A descricao é muito longa.");
            }
        }
    }
}