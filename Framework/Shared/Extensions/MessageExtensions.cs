﻿using Shared.Enums;
using Shared.Models;

namespace Shared.Extensions
{
    public static class MessageExtensions
    {
        public static bool HasErrors(this IList<Message> messages)
        {
            return messages.Any(m => m.Level == MessageLevel.Error);
        }

        public static IEnumerable<dynamic> ConvertMessageToJson(this IList<Message> messages)
        {
            return from ms in messages
                   select new
                   {
                       ms.Description,
                       Level = ms.Level.GetEnumDescription(),
                   };
        }
    }
}