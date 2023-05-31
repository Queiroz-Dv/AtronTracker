using NTF.Entities;
using NTF.Interfaces;
using System.Collections;

namespace NTF.Services
{
    /// <summary>
    /// Classe que representa um serviço de notificação.
    /// <br />
    /// Gerencia notificações relacionadas a erros, avisos e informações.
    /// <br />
    /// Permite verificar a presença de notificações e recuperar as coleções correspondentes.
    /// </summary>
    public class NotificationService : INotificationService
    {
        /// <summary>
        /// Representa uma entidade de notificação.
        /// </summary>
        protected EntityModel NotificationEntity;

        public bool HasNotifications
        {
            get
            {
                var hasnotifications = NotificationEntity != null && NotificationEntity.Errors.HasNotifications;
                return hasnotifications;
            }
        }

        public bool HasErrors
        {
            get
            {
                var hasErrors = NotificationEntity != null && NotificationEntity.Errors.HasErrors;
                return hasErrors;
            }
        }

        public bool HasWarnings
        {
            get
            {
                var hasWarnings = NotificationEntity != null && NotificationEntity.Errors.HasWarnings;
                return hasWarnings;
            }
        }

        public bool HasInformations
        {
            get
            {
                var hasInformations = NotificationEntity != null && NotificationEntity.Errors.HasInformations;
                return hasInformations;
            }
        }

        public IEnumerable Errors()
        {
            var errors = NotificationEntity?.Errors.Errors;
            return errors;
        }

        public IEnumerable Warnings()
        {
            var warnings = NotificationEntity?.Errors.Warnings;
            return warnings;
        }

        public IEnumerable Informations()
        {
            var informations = NotificationEntity?.Errors.Informations;
            return informations;
        }
    }
}
