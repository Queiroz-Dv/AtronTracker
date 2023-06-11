using NTF.Interfaces;

namespace NTF.ErrorsType
{
    /// <summary>
    /// Classe que representa o nível de advertência de uma notificação.<br></br>
    /// Implementa a interface ILevel e fornece uma descrição do nível de advertência.
    /// </summary>
    public class Warning : ILevel
    {
        /// <summary>
        /// Inicializa uma nova instância da classe Warning com uma descrição personalizada.
        /// </summary>
        /// <param name="description">A descrição do nível de advertência.</param>
        public Warning(string description = "Warning")
        {
            Description = description;
        }

        /// <summary>
        /// Obtém a descrição do nível de advertência.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Retorna a descrição do nível de advertência como uma string.
        /// </summary>
        /// <returns>A descrição do nível de advertência.</returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
