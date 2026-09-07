namespace Shared.Application.DTOS.Empresas;

public sealed record ContextoEmpresa(
    int? EmpresaId,
    string? Codigo,
    string? NomeFantasia,
    bool AcessoPermitido,
    string? MotivoBloqueio);
