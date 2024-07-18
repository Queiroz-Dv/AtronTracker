using Atron.Domain.Entities;
using Notification.Interfaces;
using Notification.Models;

namespace Atron.Domain.Validations
{
    public class SalarioValidation : NotificationModel<Salario>, INotificationService
    {
        public override void Validate(Salario entity)
        {
            if (entity.UsuarioId == 0)
            {
                AddError("Identificador de usuário inválido. Tente novamente.");
            }

            if (entity.UsuarioCodigo.Length < 3)
            {
                AddError("Código de usuário inválido. Tente novamente");
            }

            if (entity.SalarioMensal <= 0)
            {
                AddError("Quantidade de salário inválido ou abaixo de zero. Tente novamente");
            }

            if (entity.Ano == null)
            {
                AddError("Ano inválido ou não informado. Tente novamente.");
            }
        }
    }
}
