using NTF.Interfaces;

namespace NTF.ErrorsType
{
    /// <summary>
    /// Classe que representa o nível de criticidade de uma notificação. <br></br>
    /// Implementa a interface ILevel e fornece uma descrição do nível de criticidade.
    /// </summary>
    public class Critical : ILevel
    {
        /// <summary>
        /// Inicializa uma nova instância da classe Critical com uma descrição personalizada.
        /// </summary>
        /// <param name="description">A descrição do nível de criticidade.</param>
        public Critical(string description = "Critical")
        {
            Description = description;
        }

        /// <summary>
        /// Obtém a descrição do nível de criticidade.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Retorna a descrição do nível de criticidade como uma string.
        /// </summary>
        /// <returns>A descrição do nível de criticidade.</returns>
        public override string ToString()
        {
            return Description;
        }
    }
}
