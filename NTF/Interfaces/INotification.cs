using NTF.Entities;
using System.Collections.Generic;

namespace NTF.Interfaces
{
    /// <summary>
    /// Interface que define uma coleção de notificações em um contexto genérico.<br></br>
    /// Fornecendo propriedades e métodos para gerenciar e manipular uma lista de objetos de notificação.
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Obtém uma lista de objetos de notificação.
        /// </summary>
        IList<object> GetList();

        /// <summary>
        /// Obtém um valor que indica se a lista de notificações contém algum objeto.
        /// </summary>
        bool HasNotifications { get; }

        /// <summary>
        /// Verifica se a lista de notificações inclui um objeto de descrição específico.
        /// </summary>
        /// <param name="error">O objeto de descrição a ser verificado.</param>
        /// <returns>True se o objeto de descrição estiver presente na lista, caso contrário, False.</returns>
        bool Includes(Description error);

        /// <summary>
        /// Adiciona um objeto de descrição à lista de notificações.
        /// </summary>
        /// <param name="error">O objeto de descrição a ser adicionado.</param>
        void Add(Description error);
    }
}
