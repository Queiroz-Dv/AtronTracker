using Shared.Extensions;
using Shared.Models;

namespace AtronStock.WebApi.Controllers
{
    public class ControllerHelper
    {
        public static IEnumerable<dynamic> ObterNotificacoes(MessageModel messageModel)
        {
            return messageModel.Notificacoes.ConvertMessageToJson();
        }
    }
}
