using System.ComponentModel;

namespace AtronStock.Domain.Enums
{
    public enum EStatusPessoa
    {
        [Description("Ativo")]
        Ativo = 1,
        [Description("Inativo")]
        Inativo = 2,
        [Description("Removido")]
        Removido = 3
    }
}