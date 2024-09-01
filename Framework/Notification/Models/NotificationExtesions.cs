using Notification.Enums;
using System.Collections.Generic;
using System.Linq;

namespace Notification.Models
{
    /// <summary>
    /// Classe com processos utilitários para notificações
    /// </summary>
    public static class NotificationExtesions
    {
        /// <summary>
        /// Método que analisa se tem algum erro
        /// </summary>
        /// <param name="messages">Mensagens que serão analisadas</param>
        public static bool HasErrors(this IList<NotificationMessage> messages)
        {
            return messages.Count(m => m.Type == ENotificationType.Error) > 0;
        }
    }
}