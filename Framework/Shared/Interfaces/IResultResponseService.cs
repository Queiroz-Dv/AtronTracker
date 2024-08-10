using Shared.DTO;

namespace Shared.Interfaces
{
    public interface IResultResponseService
    {
         List<ResultResponse> ResultMessages { get; set; }

        void AddMessage(string message);

        void AddError(string message);

        void AddWarning(string message);
    }
}