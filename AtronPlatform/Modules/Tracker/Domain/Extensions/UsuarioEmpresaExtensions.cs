using System;
using Domain.Entities;
using Domain.Enums;

namespace Domain.Extensions
{
    public static class UsuarioEmpresaExtensions
    {
        internal static UsuarioEmpresa CriarResponsavelInicial(this Empresa empresa, Usuario usuario)
        {            
            return new UsuarioEmpresa
            {
                Empresa = empresa,
                EmpresaId = empresa.Id,
                Usuario = usuario,
                UsuarioId = usuario.Id,
                UsuarioCodigo = usuario.Codigo,
                Papel = PapelUsuarioEmpresa.Responsavel,
                Status = StatusUsuarioEmpresa.Ativo
            };
        }
    }
}
