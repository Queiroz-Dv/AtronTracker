using System.ComponentModel;
using System.Reflection;

namespace Shared.Extensions
{
    public static class TypeExtensions
    {
        public static string ObterDescricao(this Type tipo)
        {
            var attr = tipo.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description;
        }
    }

    public static class ObjectExtensions
    {
        public static string ObterDescricaoDoTipo<T>(this T obj)
        {
            var tipo = obj?.GetType();
            var attr = tipo?.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description;
        }
    }
}
