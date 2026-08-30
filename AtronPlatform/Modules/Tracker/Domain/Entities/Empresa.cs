using Domain.Enums;

namespace Domain.Entities
{
    public sealed class Empresa : EntityBase
    {
        public string Codigo { get; set; } = string.Empty;
        public string NomeFantasia { get; set; } = string.Empty;
        public string Endereco { get; set; } = string.Empty;
        public string Numero { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public StatusEmpresa Status { get; set; } = StatusEmpresa.Ativa;
    }
}
