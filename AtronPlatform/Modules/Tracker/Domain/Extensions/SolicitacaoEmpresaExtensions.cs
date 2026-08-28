using System;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Extensions;

public static class SolicitacaoEmpresaExtensions
{
    public static SolicitacaoEmpresa CriarSolicitacao(
        this Empresa empresa, Usuario usuario)
        => new()
        {
            EmpresaId = empresa.Id,
            Empresa = empresa,
            UsuarioId = usuario.Id,
            UsuarioCodigo = usuario.Codigo,
            Usuario = usuario,
            Status = StatusSolicitacaoEmpresa.Pendente,
            CriadaEm = DateTime.UtcNow
        };
}
