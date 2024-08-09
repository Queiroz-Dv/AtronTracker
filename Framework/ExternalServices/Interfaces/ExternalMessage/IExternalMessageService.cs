using Shared.DTO;

namespace ExternalServices.Interfaces.ExternalMessage
{
    public interface IExternalMessageService
    {
        public List<ResultResponse> ResultResponses { get; set; }
    }
}