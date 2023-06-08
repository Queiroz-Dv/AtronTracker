using NTF.Interfaces;

namespace NTF.Entities
{
    /// <summary>
    /// Classe abstrata que representa uma descrição utilizada em notificações. <br></br>
    /// Armazena a mensagem da descrição e oferece métodos para manipulação.
    /// </summary>
    public abstract class Description : IDescription
    {
        /// <summary>
        /// Obtém a mensagem da descrição.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Cria uma nova instância de Description com a mensagem e argumentos opcionais.
        /// </summary>
        /// <param name="message">A mensagem da descrição.</param>
        /// <param name="args">Argumentos opcionais para substituir na mensagem.</param>
        public Description(string message, params string[] args)
        {
            Message = message;

            // Substitui os argumentos na mensagem, se houver.
            for (var i = 0; i < args.Length; i++)
            {
                Message = Message.Replace("{" + i + "}", args[i]);
            }
        }

        /// <summary>
        /// Retorna a representação em string da descrição.
        /// </summary>
        /// <returns>A mensagem da descrição.</returns>
        public override string ToString()
        {
            return Message;
        }
    }
}
