using Atron.Domain.Entities;
using Notification.Interfaces;
using Notification.Models;

namespace Atron.Domain.Validations
{
    public class CargoValidation : NotificationModel<Cargo>, INotificationService
    {
        public override void Validate(Cargo entity)
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

            if (string.IsNullOrEmpty(entity.DepartmentoCodigo))
            {
                AddError("Código do departamento obrigatório.");
            }
        }
    }
}