using System;

namespace Atron.Tracker.Domain.Customs
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class ModuloInfoAttribute : Attribute
    {
        public string Codigo { get; }
        public string Descricao { get; }

        public ModuloInfoAttribute(string codigo, string descricao)
        {
            Codigo = codigo;
            Descricao = descricao;
        }
    }

}
