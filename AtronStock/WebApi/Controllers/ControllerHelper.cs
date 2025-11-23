using Shared.Domain.ValueObjects;
using Shared.Extensions;

namespace AtronStock.WebApi.Controllers
{
    public class ControllerHelper
    {
        public static IEnumerable<dynamic> ObterNotificacoes(Notifiable messageModel)
        {
            return messageModel.Notificacoes.ConvertMessageToJson();
        }
    }
}
