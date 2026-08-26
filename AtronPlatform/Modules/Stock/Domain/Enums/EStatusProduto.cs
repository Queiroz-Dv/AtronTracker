using System.ComponentModel;

namespace AtronStock.Domain.Enums
{
    public enum EStatusProduto
    {
        [Description("Ativo")]
        Ativo = 1,

        [Description("Baixado")]
        Baixado = 2
    }
}
