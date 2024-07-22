using Notification.Interfaces;
using Shared.DTO;
using System;
using System.Collections.Generic;

namespace Notification.Services
{
    [Serializable]
    public abstract class NotificationWebViewService : INotificationWebViewService
    {
        public List<ApiWebViewMessageResponse> MessageResponses { get; set; }

        public ApiWebViewMessageResponse Response { get; set; }

        protected NotificationWebViewService()
        {
            MessageResponses = new List<ApiWebViewMessageResponse>();
        }

        public void AddApiNotification(string message, string level)
        {
            MessageResponses.Add(new ApiWebViewMessageResponse() { Message = message, Level = level });
        }

        public void AddApiNotification(string responseMessage)
        {
            Response = new ApiWebViewMessageResponse();
            Response.ResultApiJson = responseMessage;            
        }

        public abstract void AddApiMessage(string message);

        public abstract void AddApiError(string message);

        public abstract void AddApiWarning(string message);

        public abstract void AddApiResponse(string apiResponse);

        public ApiWebViewMessageResponse GetJsonResponseContent()
        {
            return Response;
        }
    }
}