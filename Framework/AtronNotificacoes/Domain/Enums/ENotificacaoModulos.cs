using System.ComponentModel;

namespace AtronNotificacoes.Domain.Enums
{
    public enum ENotificacaoModulos
    {
        [Description(nameof(Tracker))]
        Tracker,

        [Description(nameof(Sales))]
        Sales,

        [Description(nameof(Stock))]
        Stock
    }
}