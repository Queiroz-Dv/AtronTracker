#nullable enable

using Domain.Enums;

namespace Domain.Entities
{
    public sealed class UsuarioEmpresa : EntityBase
    {
        public int EmpresaId { get; internal set; }
        public Empresa Empresa { get; internal set; } = null!;
        public int UsuarioId { get; internal set; }
        public string UsuarioCodigo { get; internal set; } = string.Empty;
        public Usuario Usuario { get; internal set; } = null!;
        public PapelUsuarioEmpresa Papel { get; internal set; }
        public StatusUsuarioEmpresa Status { get; internal set; }
    }
}
