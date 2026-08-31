using System;

namespace Domain.Entities;

public sealed class ConviteWorkspace : EntityBase
{
    public int WorkspaceId { get; set; }
    public string IdentificadorHash { get; set; } = string.Empty;
    public string RemetenteCodigo { get; set; } = string.Empty;
    public DateTime ExpiraEm { get; set; }
    public string? UtilizadoPorUsuarioCodigo { get; set; }
    public DateTime? UtilizadoEm { get; set; }

    public Workspace Workspace { get; set; }
    public Usuario Remetente { get; set; }
    public Usuario UtilizadoPorUsuario { get; set; }
}
