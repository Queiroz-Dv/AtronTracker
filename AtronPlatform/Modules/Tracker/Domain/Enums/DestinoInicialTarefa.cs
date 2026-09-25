using System.ComponentModel;

namespace Domain.Enums
{
    public enum DestinoInicialTarefa
    {
        [Description("Usuario")]
        Usuario = 1,

        [Description("Departamento Cargo")]
        DepartamentoCargo = 2,

        [Description("Equipe")]
        Equipe = 3
    }
}