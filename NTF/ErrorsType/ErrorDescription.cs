using NTF.Entities;
using NTF.Interfaces;

namespace NTF.ErrorsType
{
    /// <summary>
    /// Classe que representa uma descrição de erro.
    /// Contém uma mensagem de erro e um nível de gravidade associado.
    /// </summary>
    public class ErrorDescription : Description
    {
        private readonly ILevel level;

        /// <summary>
        /// Obtém o nível de gravidade do erro.
        /// </summary>
        public ILevel GetLevel()
        {
            return level;
        }

        /// <summary>
        /// Cria uma nova instância de ErrorDescription com a mensagem de erro, nível de gravidade e argumentos opcionais.
        /// </summary>
        /// <param name="message">A mensagem de erro.</param>
        /// <param name="level">O nível de gravidade do erro.</param>
        /// <param name="args">Argumentos opcionais para formatar a mensagem de erro.</param>
        public ErrorDescription(string message, ILevel level, params string[] args)
            : base(message, args)
        {
            this.level = level;
        }
    }
}
