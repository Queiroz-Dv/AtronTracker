using System;

namespace Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class TenantModuleAttribute : Attribute
    {
        public string ModuloCodigo { get; }

        public TenantModuleAttribute(string moduloCodigo)
        {
            ModuloCodigo = moduloCodigo;
        }
    }
}