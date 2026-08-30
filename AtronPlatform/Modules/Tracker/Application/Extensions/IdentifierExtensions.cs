using System;

namespace Application.Extensions
{
    public static class IdentifierExtensions
    {
        public static bool IdentifierIsEmail(this string identifier)
        {

            return !string.IsNullOrWhiteSpace(identifier)
                && identifier.Contains('@', StringComparison.Ordinal)
                && identifier.IndexOf('@') > 0
                && identifier.IndexOf('@') < identifier.Length - 1;
        }
    }
}
