using NTF.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace NTF.Entities
{
    /// <summary>
    /// Classe abstrata que representa uma coleção de notificações em um contexto genérico.<br></br>
    /// Implementa a interface INotification e fornece funcionalidades básicas para gerenciar uma lista de objetos de notificação.
    /// </summary>
    public abstract class Notification : INotification
    {
        private readonly IList<object> list = new List<object>();

        /// <summary>
        /// Obtém uma lista de objetos de notificação.
        /// </summary>
        public IList<object> GetList()
        {
            return list;
        }

        /// <summary>
        /// Obtém um valor que indica se a lista de notificações contém algum objeto.
        /// </summary>
        public bool HasNotifications => GetList().Any();

        /// <summary>
        /// Verifica se a lista de notificações inclui um objeto de descrição específico.
        /// </summary>
        /// <param name="error">O objeto de descrição a ser verificado.</param>
        /// <returns>True se o objeto de descrição estiver presente na lista, caso contrário, False.</returns>
        public bool Includes(Description error) => GetList().Contains(error);

        /// <summary>
        /// Adiciona um objeto de descrição à lista de notificações.
        /// </summary>
        /// <param name="description">O objeto de descrição a ser adicionado.</param>
        public void Add(Description description) => GetList().Add(description);
    }
}
