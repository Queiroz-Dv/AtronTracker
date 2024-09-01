using Notification.Enums;
using Shared.Extensions;
using System;
using System.Text.Json.Serialization;

namespace Notification.Models
{
    /// <summary>
    /// Classe padrão de notificações
    /// </summary>
    [Serializable]
    public class NotificationMessage
    {
        public NotificationMessage(string message, ENotificationType type)
        {
            Message = message;
            Type = type;
            Level = GetLevelMessage(type);
        }

        public NotificationMessage(string message)
        {
            Message = message;
            Type = ENotificationType.Message;
            Level = GetLevelMessage();
        }

        public NotificationMessage(string message, string notificationType)
        {
            Message = message;
            Type = GetNotificationType(notificationType);
            Level = GetLevelMessage();
        }

        public string Message { get; set; }

        [JsonIgnore]
        public ENotificationType Type { get; set; }

        public string Level { get; set; }

        /// <summary>
        /// Método que obtém o nível da mensgaem de notificação
        /// </summary>
        /// <returns>Retorna uma descrição do tipo do enumerado</returns>
        private string GetLevelMessage()
        {
            switch (Type)
            {
                case ENotificationType.Message: return ENotificationType.Message.GetEnumDescription();
                case ENotificationType.Error: return ENotificationType.Error.GetEnumDescription();
                case ENotificationType.Warning: return ENotificationType.Warning.GetEnumDescription();
                default: throw new ArgumentException("Invalid level");
            }
        }

        private string GetLevelMessage(ENotificationType type)
        {
            switch (type)
            {
                case ENotificationType.Message: return ENotificationType.Message.GetEnumDescription();
                case ENotificationType.Error: return ENotificationType.Error.GetEnumDescription();
                case ENotificationType.Warning: return ENotificationType.Warning.GetEnumDescription();
                default: throw new ArgumentException("Invalid level");
            }
        }

        const string MESSAGE = "Message";
        const string ERROR = "Error";
        const string WARNING = "Warning";

        public static ENotificationType GetNotificationType(string level)
        {
            return level switch
            {
                MESSAGE => ENotificationType.Message,
                ERROR => ENotificationType.Error,
                WARNING => ENotificationType.Warning,
                _ => throw new ArgumentException("Invalid level"),
            };
        }
    }
}