using NTF.Interfaces;

namespace NTF.ErrorsType
{
    /// <summary>
    /// Classe que representa o nível de informação de uma notificação.<br></br>
    /// Implementa a interface ILevel e fornece uma descrição do nível de informação.
    /// </summary>
    public class Information : ILevel
    {
        /// <summary>
        /// Inicializa uma nova instância da classe Information com uma descrição personalizada.
        /// </summary>
        /// <param name="description">A descrição do nível de informação.</param>
        public Information(string description = "Information")
        {
            Description = description;
        }

        /// <summary>
        /// Obtém a descrição do nível de informação.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Retorna a descrição do nível de informação como uma string.
        /// </summary>
        /// <returns>A descrição do nível de informação.</returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
