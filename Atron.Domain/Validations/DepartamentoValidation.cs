using Atron.Domain.Entities;
using Notification.Interfaces;
using Notification.Models;

namespace Atron.Domain.Validations
{
    public class DepartamentoValidation : NotificationModel<Departamento>, INotificationService
    {
        public override void Validate(Departamento entity)
        {
            if (string.IsNullOrEmpty(entity.DescricaoDepartamento) || 
                string.IsNullOrEmpty(entity.CodigoDepartamento))
            {
                AddError("O código ou a descrição não estão preenchidos.");
            }

            if (entity.CodigoDepartamento.Length > 10)
            {
                AddError("O código informado é muito longo.");
            }

            if (entity.CodigoDepartamento.Length < 3)
            {
                AddError("O código informado é muito pequeno.");
            }

            if (entity.DescricaoDepartamento.Length < 3)
            {
                AddError("A descricao é muito pequena.");
            }

            if (entity.DescricaoDepartamento.Length > 50)
            {
                AddError("A descricao é muito longa.");
            }
        }
    }
}