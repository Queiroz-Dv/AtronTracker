using Atron.Domain.Entities;
using Shared.Extensions;
using Shared.Interfaces;
using Shared.Models;
using System;

namespace Atron.Application.Validations
{
    public class UsuarioMessageValidation : MessageModel<Usuario>, IMessages
    {
        public override void Validate(Usuario entity)
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
                AddError("Data de nascimento inválida, tente novamente");
            }

            //if (entity.DepartamentoId == 0 || string.IsNullOrEmpty(entity.DepartamentoCodigo))
            //{
            //    AddError("Identificador ou código de departamento inválido, tente novamente.");
            //}

            //if (entity.CargoId == 0 || string.IsNullOrEmpty(entity.CargoCodigo))
            //{
            //    AddError("Identificador ou código de cargo inválido, tente novamente.");
            //}
        }
    }
}