using Application.DTO.Response;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Resources;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class ObterConviteWorkspaceCase(
    IConviteWorkspaceRepository conviteWorkspaceRepository,
    ITokenTemporarioService tokenTemporarioService,
    ConviteWorkspaceMapping mapping)
{
    public async Task<Resultado<ConviteWorkspaceResponse>> ExecutarAsync(
        string identificador)
    {
        var hash = tokenTemporarioService.ObterHash(identificador);
        if (string.IsNullOrWhiteSpace(hash))
        {
            return Resultado<ConviteWorkspaceResponse>.Falha(
                WorkspaceResource.Erro_ConviteInvalido);
        }

        var convite = await conviteWorkspaceRepository.ObterAtivoPorHashAsync(hash);
        return convite is null
            ? Resultado<ConviteWorkspaceResponse>.Falha(
                WorkspaceResource.Erro_ConviteInvalido)
            : Resultado<ConviteWorkspaceResponse>.Sucesso(mapping.MapToDto(convite));
    }
}
