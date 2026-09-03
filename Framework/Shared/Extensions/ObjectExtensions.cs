namespace Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNullable(this object entity)
        {
            return entity is null;
        }

        public static bool IsEquals(this object first, object second)
        {
            if (first == null && second == null) return true;
            if (first == null || second == null) return false;
                    
            if (first is string && second is string)
            {
                return string.Equals((string)first, (string)second, StringComparison.OrdinalIgnoreCase);
            }

            return first.Equals(second);
        }
    }
}