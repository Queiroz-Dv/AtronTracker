﻿using Atron.Domain.Entities;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Interfaces.Validations;
using Shared.Models;
using System;
using System.Linq;

namespace Atron.Application.Validations
{
    public class UsuarioMessageValidation : MessageModel, IMessages, IValidateModel<Usuario>
    {
        public void Validate(Usuario entity)
        {
            if (entity.Codigo.IsNullOrEmpty())
            {
                AddError("O campo código não está preenchido.");
            }

            if (entity.Codigo.Length > 10)
            {
                AddError("O código informado é muito longo.");
            }

            if (entity.Codigo.Length < 3)
            {
                AddError("O código informado é muito pequeno.");
            }

            if (entity.Nome.IsNullOrEmpty())
            {
                AddError("Nome de usuário é obrigatório.");
            }

            if (entity.Nome.Length < 3)
            {
                AddError("O nome é muito pequeno.");
            }

            if (entity.Nome.Length > 25)
            {
                AddError("O nome é muito longo.");
            }

            if (entity.Sobrenome.IsNullOrEmpty())
            {
                AddError("Nome de usuário é obrigatório.");
            }

            if (entity.Sobrenome.Length < 3)
            {
                AddError("O nome é muito pequeno.");
            }

            if (entity.Sobrenome.Length > 50)
            {
                AddError("O sobrenome é muito longo.");
            }

            if (entity.DataNascimento == DateTime.Now)
            {
                AddError("Data de nascimento inválida.");
            }           
        }
    }
}