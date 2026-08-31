namespace Shared.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime SemTimezone(this DateTime data)
          => DateTime.SpecifyKind(data, DateTimeKind.Unspecified);
    }
}