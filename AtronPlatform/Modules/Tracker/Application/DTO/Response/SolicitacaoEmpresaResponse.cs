using System;
using Domain.Enums;

namespace Application.DTO.Response;

public sealed record SolicitacaoEmpresaResponse(
    int Id,
    int EmpresaId,
    string CodigoEmpresa,
    string NomeFantasia,
    StatusSolicitacaoEmpresa Status,
    DateTime CriadaEm);
