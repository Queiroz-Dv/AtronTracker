using Application.DTO.Request;
using Application.DTO.Response;
using Application.Mapping;
using Application.Resources;
using Domain.Entities;
using Domain.Enums;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class CriarWorkspaceInicialCase(
    IWorkspaceRepository workspaceRepository,
    IUsuarioRepository usuarioRepository,
    IEmpresaRepository empresaRepository,
    IValidador<CriarWorkspaceInicialRequest> validador,
    WorkspaceMapping mapping)
{
    public async Task<Resultado<WorkspaceInicialResponse>> ExecutarAsync(
        CriarWorkspaceInicialRequest request)
    {
        var erros = validador.Validar(request);
        if (erros.TemErros())
            return Resultado<WorkspaceInicialResponse>.Falhas(erros);

        var usuario = await usuarioRepository.ObterUsuarioGeralPorCodigoAsync(request.UsuarioCodigo);
        if (usuario is null)
            return Resultado<WorkspaceInicialResponse>.Falha(WorkspaceResource.Erro_UsuarioNaoEncontrado);

        if (await workspaceRepository.UsuarioPossuiWorkspaceAsync(usuario.Codigo))
            return Resultado<WorkspaceInicialResponse>.Falha(WorkspaceResource.Erro_WorkspaceInicialUsuarioExistente);

        Empresa? empresa = null;
        if (request.Tipo == TipoWorkspace.Empresa)
        {
            empresa = await empresaRepository.ObterPorCodigoAsync(request.EmpresaCodigo!);
            if (empresa is null)
                return Resultado<WorkspaceInicialResponse>.Falha(
                    WorkspaceResource.Erro_EmpresaNaoEncontrada);

            if (await workspaceRepository.EmpresaPossuiWorkspaceAsync(empresa.Codigo))
                return Resultado<WorkspaceInicialResponse>.Falha(
                    WorkspaceResource.Erro_WorkspaceEmpresaExistente);
        }

        var workspace = mapping.MapToEntity(request);

        if (!await workspaceRepository.CriarInicialAsync(workspace))
            return Resultado<WorkspaceInicialResponse>.Falha(WorkspaceResource.Erro_Persistencia);

        if (!empresa.IsNullable())
            workspace.Empresa = empresa;

        return Resultado<WorkspaceInicialResponse>.Sucesso(mapping.MapToDto(workspace));
    }
}