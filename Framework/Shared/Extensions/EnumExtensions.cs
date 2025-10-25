using System.ComponentModel;
using System.Reflection;

namespace Shared.Extensions
{
    /// <summary>
    /// Classe de extensão para obter informações de enumeradores
    /// </summary>
    public static class EnumExtensions
    {
        /// <summary>
        /// Obtém a descrição do enum atual
        /// </summary>
        /// <param name="value">Enum que será processado</param>
        /// <returns>O valor da descrição do enumerado</returns>
        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = fi.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

            if (attributes != null && attributes.Any())
            {
                return attributes.First().Description;
            }

            return value.ToString();
        }
    }
}