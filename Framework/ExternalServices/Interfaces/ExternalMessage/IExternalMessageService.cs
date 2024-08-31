using Shared.DTO;

namespace ExternalServices.Interfaces.ExternalMessage
{
    /// <summary>
    /// Interface de acesso ao serviço de mensagens externas
    /// </summary>
    public interface IExternalMessageService
    {
        public List<ResultResponseDTO> ResultResponses { get; set; }
    }
}