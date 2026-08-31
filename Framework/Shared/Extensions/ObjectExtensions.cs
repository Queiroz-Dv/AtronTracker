namespace Shared.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNullable(this object entity)
        {
            return entity == null;
        }
    }
}