namespace NTF.Interfaces
{
    /// <summary>
    /// Interface que define uma descrição utilizada em notificações. <br></br>
    /// Fornece a propriedade de mensagem e o método de representação em string.
    /// </summary>
    public interface IDescription
    {
        /// <summary>
        /// Obtém a mensagem da descrição.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Retorna a representação em string da descrição.
        /// </summary>
        /// <returns>A mensagem da descrição.</returns>
        string ToString();
    }
}
