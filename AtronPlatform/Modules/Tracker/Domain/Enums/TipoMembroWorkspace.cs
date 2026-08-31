using System.ComponentModel;

namespace Domain.Enums
{
    public enum TipoMembroWorkspace
    {
        [Description("Proprietário")]
        Proprietario = 1,
        [Description("Membro")]
        Membro = 2
    }
}
