namespace Shared.Application.Interfaces.Service
{
    public interface IMessageService : IMessageBaseService
    {
        /// <summary>
        /// Adiciona uma messagem
        /// </summary>
        /// <param name="message">Mensagem a ser adicionada</param>
        void AdicionarMensagem(string message);

        /// <summary>
        /// Adiciona um erro
        /// </summary>
        /// <param name="error">Error a ser adicionado</param>
        void AdicionarErro(string error);

        /// <summary>
        /// Adiciona um aviso
        /// </summary>
        /// <param name="warning">Aviso a ser adicionado</param>
        void AdicionarAviso(string warning);
    }
}