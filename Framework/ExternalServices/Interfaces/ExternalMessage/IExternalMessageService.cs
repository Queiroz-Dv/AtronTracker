using Shared.DTO;

namespace ExternalServices.Interfaces.ExternalMessage
{
    public interface IExternalMessageService
    {
        public List<ResultResponseDTO> ResultResponses { get; set; }
    }
}