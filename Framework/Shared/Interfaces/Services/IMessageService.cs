namespace Shared.Interfaces
{
    public interface IMessageService : IMessages
    {
        /// <summary>
        /// Adiciona uma messagem
        /// </summary>
        /// <param name="message">Mensagem a ser adicionada</param>
        void AddMessage(string message);

        /// <summary>
        /// Adiciona um erro
        /// </summary>
        /// <param name="error">Error a ser adicionado</param>
        void AddError(string error);

        /// <summary>
        /// Adiciona um aviso
        /// </summary>
        /// <param name="warning">Aviso a ser adicionado</param>
        void AddWarning(string warning);
    }
}