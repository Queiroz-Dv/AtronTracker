using Atron.Domain.Entities;
using Notification.Interfaces;
using Notification.Models;
using System;

namespace Atron.Domain.Validations
{
    public class PermissaoValidation : NotificationModel<Permissao>, INotificationService
    {
        public override void Validate(Permissao entity)
        {
            if (entity.UsuarioId == 0)
            {
                AddError("Identificador de usuário inválido. Tente novamente");
            }

            if (entity.UsuarioCodigo.Length < 3)
            {
                AddError("Código de usuário inválido. Tente novamente com outro código");
            }

            if (entity.PermissaoEstadoId == 0)
            {
                AddError("Identificador inválido do estado da permissão. Tente novamente.");
            }

            if (entity.DataInicial > entity.DataFinal)
            {
                AddError("Data inicial maior que a data final. Tente novamente com datas diferentes.");
            }

            if (entity.QuantidadeDeDias <= 0)
            {
                AddError("Quantidade de dias inferior ou igual a zero. Tente novamente.");
            }
        }
    }
}