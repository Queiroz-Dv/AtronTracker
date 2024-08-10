using Shared.Enums;
using Shared.Services;

namespace Shared.Models
{
    public class ResultResponseModel : ResultResponseService
    {
         public override void AddError(string message)
        {
            AddNotification(message, ResultResponseLevelEnum.Error);
        }

        public override void AddMessage(string message)
        {
            AddNotification(message, ResultResponseLevelEnum.Message);
        }

        public override void AddWarning(string message)
        {
            AddNotification(message, ResultResponseLevelEnum.Warning);
        }
    }
}