using System;

namespace Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime SemTimezone(this DateTime? dateTime)
            => DateTime.SpecifyKind(dateTime ?? DateTime.UtcNow, DateTimeKind.Unspecified);

        public static DateTime SemTimezone(this DateTime dateTime)
            => DateTime.SpecifyKind(dateTime, DateTimeKind.Unspecified);
    }
}