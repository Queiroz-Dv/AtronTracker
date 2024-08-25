using Shared.DTO;
using Shared.Enums;
using Shared.Interfaces;

namespace Shared.Services
{
    public abstract class ResultResponseService : IResultResponseService
    {
        protected ResultResponseService()
        {
            ResultMessages = new List<ResultResponseDTO>();
        }

        public void AddNotification(string message, ResultResponseLevelEnum level)
        {
            ResultMessages.Add(new ResultResponseDTO() { Message = message, Level = level });
        }

        public List<ResultResponseDTO> ResultMessages { get; set; }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}