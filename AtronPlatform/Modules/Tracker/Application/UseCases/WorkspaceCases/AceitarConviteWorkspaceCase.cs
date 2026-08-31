using Application.DTO.Response;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Resources;
using Domain.Interfaces;
using Domain.Interfaces.UsuarioInterfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class AceitarConviteWorkspaceCase(
    IConviteWorkspaceRepository conviteWorkspaceRepository,
    IWorkspaceRepository workspaceRepository,
    IUsuarioRepository usuarioRepository,
    ITokenTemporarioService tokenTemporarioService,
    WorkspaceMapping workspaceMapping)
{
    public async Task<Resultado<WorkspaceInicialResponse>> ExecutarAsync(
        string identificador,
        string usuarioCodigo)
    {
        var usuario = await usuarioRepository.ObterUsuarioGeralPorCodigoAsync(usuarioCodigo);
        if (usuario is null)
        {
            return Resultado<WorkspaceInicialResponse>.Falha(
                UsuarioResource.Erro_UsuarioNaoEncontrado);
        }

        var hash = tokenTemporarioService.ObterHash(identificador);
        var convite = await conviteWorkspaceRepository.ObterAtivoPorHashAsync(hash);
        if (convite is null)
        {
            return Resultado<WorkspaceInicialResponse>.Falha(
                WorkspaceResource.Erro_ConviteInvalido);
        }

        if (await workspaceRepository.UsuarioPertenceAoWorkspaceAsync(
                convite.WorkspaceId,
                usuarioCodigo))
        {
            return Resultado<WorkspaceInicialResponse>.Falha(
                WorkspaceResource.Erro_ConviteUsuarioJaMembro);
        }

        if (!await conviteWorkspaceRepository.ConsumirAsync(convite, usuarioCodigo))
        {
            return Resultado<WorkspaceInicialResponse>.Falha(
                WorkspaceResource.Erro_ConviteConsumo);
        }

        return Resultado<WorkspaceInicialResponse>
            .Sucesso(workspaceMapping.MapToDto(convite.Workspace))
            .AdicionarMensagem(WorkspaceResource.Mensagem_ConviteAceito);
    }
}
