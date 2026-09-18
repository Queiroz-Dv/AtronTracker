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

}
