using Application.DTO;
using Application.DTO.Request;
using Domain.Entities;
using Domain.Enums;
using System;

namespace Application.Extensions;

public static class UsuarioCadastroExtensions
{
    public static Usuario MontarUsuario(this UsuarioRegistroRequest request)
        => new(
            request.Codigo,
            request.Nome,
            request.Sobrenome,
            request.Email,
            request.DataNascimento?.ToDateTime(TimeOnly.MinValue));

    public static UsuarioRequest MontarUsuarioRequest(this UsuarioRegistroRequest request)
        => new()
        {
            Codigo = request.Codigo,
            Nome = request.Nome,
            Sobrenome = request.Sobrenome,
            DataNascimento = request.DataNascimento?.ToDateTime(TimeOnly.MinValue),
            Email = request.Email,
            Senha = request.Senha
        };

    public static CriarWorkspaceInicialRequest MontarWorkspaceInicial(
        this WorkspaceRegistroRequest request,
        string usuarioCodigo)
        => new(
            request.Nome,
            request.Tipo,
            usuarioCodigo,
            request.Empresa?.Codigo);

    public static EmpresaDTO MontarEmpresa(this EmpresaRegistroRequest request)
        => new()
        {
            Codigo = request.Codigo,
            NomeFantasia = request.NomeFantasia,
            Endereco = request.Endereco,
            Numero = request.Numero,
            Email = request.Email,
            Status = StatusEmpresa.Ativa
        };
}
