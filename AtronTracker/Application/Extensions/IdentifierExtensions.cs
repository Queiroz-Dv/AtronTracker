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

        public static string NormalizeIdentifier(this string identifier)
            => identifier?.Trim() ?? string.Empty;

        public static string NormalizeUserCodeIdentifier(this string identifier)
            => identifier.NormalizeIdentifier().ToUpperInvariant();
    }
}
