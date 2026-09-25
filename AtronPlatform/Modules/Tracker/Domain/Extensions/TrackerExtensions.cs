using Shared.Attributes;
using System;
using System.Reflection;

namespace Domain.Extensions
{
    public static class TrackerExtensions
    {
        public static string ObterDescricaoDoModulo(this Type tipo)
        {
            if (tipo == null) return string.Empty;

            var attr = tipo.GetCustomAttribute<TenantModuleAttribute>();
            return attr?.ModuloCodigo ?? string.Empty;
        }

        public static string ObterDescricaoDoModulo<T>(this T obj)
        {
            if (obj == null) return string.Empty;

            // Se 'obj' já for do tipo 'Type', usa ele diretamente. Caso contrário, pega o GetType()
            var tipo = obj as Type ?? obj.GetType();
            return tipo.ObterDescricaoDoModulo();
        }
    }
}