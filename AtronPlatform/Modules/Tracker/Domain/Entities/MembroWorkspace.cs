using Domain.Enums;

namespace Domain.Entities
{
    public sealed class MembroWorkspace
    {
        public int WorkspaceId { get; set; }
        public string UsuarioCodigo { get; set; } = string.Empty;
        public TipoMembroWorkspace Tipo { get; set; }

        public Workspace Workspace { get; set; }
        public Usuario Usuario { get; set; }
    }
}
