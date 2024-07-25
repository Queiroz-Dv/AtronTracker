using Newtonsoft.Json;
using Notification.Enums;
using Notification.Interfaces;
using Shared.DTO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Notification.Services
{
    [Serializable]
    public abstract class NotificationWebViewService : INotificationWebViewService
    {
        public List<ApiWebViewMessageResponse> MessageResponses { get; set; }

        private ApiWebViewMessageResponse Response { get; set; }

        protected NotificationWebViewService()
        {
            MessageResponses = new List<ApiWebViewMessageResponse>();
            Response = new ApiWebViewMessageResponse();
        }

        public void AddApiNotification(string response)
        {
            var result = response.ResponseDeserialized<List<ApiWebViewMessageResponse>>();
            if (result.Count == 1)
            {
                Response = result.First();
                MessageResponses.Add(Response);
            }
            else
            {
                MessageResponses.AddRange(result);
            }
        }

        public void AddApiNotification(string response, string level)
        {
            Response.Message = response;
            Response.Level = level;
            MessageResponses.Add(Response);
        }

        public abstract void AddApiMessage(string message);

        public abstract void AddApiError(string message);

        public abstract void AddApiWarning(string message);

        public ApiWebViewMessageResponse GetResultResponse()
        {
            return Response;
        }
    }

    public static class NotificationWebViewExtensions
    {
        const string MESSAGE = "Message";
        const string ERROR = "Error";
        const string WARNING = "Warning";

        public static string ResponseSerialized(this ApiWebViewMessageResponse response)
        {
            return JsonConvert.SerializeObject(response);
        }

        public static ApiWebViewMessageResponse ResponseDeserialized(this string response)
        {
            return JsonConvert.DeserializeObject<ApiWebViewMessageResponse>(response);
        }

        public static T ResponseDeserialized<T>(this string response)
        {
            return JsonConvert.DeserializeObject<T>(response);
        }

        public static bool HasErrors(this IList<ApiWebViewMessageResponse> messages)
        {
            return messages.Count(m => m.Level == ERROR) > 0;
        }

        public static ENotificationType GetNotificationType(this ApiWebViewMessageResponse level)
        {
            if (level.Level == MESSAGE)
            {
                return ENotificationType.Message;
            }
            else if (level.Level == ERROR)
            {
                return ENotificationType.Error;
            }
            else if (level.Level == WARNING)
            {

                return ENotificationType.Warning;
            }
            else
            {
                throw new ArgumentException("Invalid level");
            }
        }
    }
}