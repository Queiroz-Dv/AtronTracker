using Atron.Domain.Entities;
using Notification.Interfaces;
using Notification.Models;
using System;

namespace Atron.Domain.Validations
{
    public class UsuarioValidation : NotificationModel<Usuario>, INotificationService
    {
        public override void Validate(Usuario entity)
        {
            if (string.IsNullOrEmpty(entity.Codigo))
            {
                AddError("O campo código não está preenchido.");
            }

            if (entity.Id == 0)
            {
                AddError("Identificador inválido, tente novamente.");
            }

            if (entity.Codigo.Length > 10)
            {
                AddError("O código informado é muito longo.");
            }

            if (entity.Codigo.Length < 3)
            {
                AddError("O código informado é muito pequeno.");
            }

            if (string.IsNullOrEmpty(entity.Nome))
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

            if (string.IsNullOrEmpty(entity.Sobrenome))
            {
                AddError("Nome de usuário é obrigatório.");
            }

            if (entity.Sobrenome.Length < 3)
            {
                AddError("O nome é muito pequeno.");
            }

            if (entity.Sobrenome.Length > 50)
            {
                AddError("O nome é muito longo.");
            }

            if (entity.DepartamentoId == 0 || string.IsNullOrEmpty(entity.DepartamentoCodigo))
            {
                AddError("Identificador ou código de departamento inválido, tente novamente.");
            }

            if (entity.CargoId == 0 || string.IsNullOrEmpty(entity.CargoCodigo))
            {
                AddError("Identificador ou código de cargo inválido, tente novamente.");
            }

            if (entity.DataNascimento == DateTime.Now)
            {
                AddError("Data de nascimento inválida, tente novamente");
            }
        }
    }
}
