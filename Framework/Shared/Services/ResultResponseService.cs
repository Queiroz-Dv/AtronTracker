using Shared.DTO;
using Shared.Enums;
using Shared.Interfaces;

namespace Shared.Services
{
    public abstract class ResultResponseService : IResultResponseService
    {
        protected ResultResponseService()
        {
            ResultMessages = new List<ResultResponse>();
        }

        public void AddNotification(string message, ResultResponseLevelEnum level)
        {
            ResultMessages.Add(new ResultResponse() { Message = message, Level = level });
        }

        public List<ResultResponse> ResultMessages { get; set; }

        public abstract void AddMessage(string message);

        public abstract void AddError(string message);

        public abstract void AddWarning(string message);
    }
}