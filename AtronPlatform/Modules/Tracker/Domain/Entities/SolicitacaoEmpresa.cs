using System;
using Domain.Enums;

namespace Domain.Entities;

public sealed class SolicitacaoEmpresa : EntityBase
{
    public int EmpresaId { get; internal set; }
    public Empresa Empresa { get; internal set; } = null!;
    public int UsuarioId { get; internal set; }
    public string UsuarioCodigo { get; internal set; } = string.Empty;
    public Usuario Usuario { get; internal set; } = null!;
    public StatusSolicitacaoEmpresa Status { get; internal set; } = StatusSolicitacaoEmpresa.Pendente;
    public DateTime CriadaEm { get; internal set; }
}
