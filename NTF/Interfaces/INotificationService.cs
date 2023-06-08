using System.Collections;

namespace NTF.Interfaces
{
    /// <summary>
    /// Interface que define um serviço de notificação. <br/>
    /// Permite verificar a presença de notificações, erros, avisos e informações, e obter coleções correspondentes.
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Obtém um valor que indica se existem notificações.
        /// </summary>
        bool HasNotifications { get; }

        /// <summary>
        /// Obtém um valor que indica se existem erros.
        /// </summary>
        bool HasErrors { get; }

        /// <summary>
        /// Obtém um valor que indica se existem avisos.
        /// </summary>
        bool HasWarnings { get; }

        /// <summary>
        /// Obtém um valor que indica se existem informações.
        /// </summary>
        bool HasInformations { get; }

        /// <summary>
        /// Obtém uma coleção de erros.
        /// </summary>
        /// <returns>A coleção de erros.</returns>
        IEnumerable Errors();

        /// <summary>
        /// Obtém uma coleção de avisos.
        /// </summary>
        /// <returns>A coleção de avisos.</returns>
        IEnumerable Warnings();


        /// <summary>
        /// Obtém uma coleção de informações.
        /// </summary>
        /// <returns>A coleção de informações.</returns>
        IEnumerable Informations();
    }
}
