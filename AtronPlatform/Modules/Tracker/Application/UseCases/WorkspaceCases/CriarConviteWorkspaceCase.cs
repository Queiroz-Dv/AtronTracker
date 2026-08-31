using Application.DTO.Request;
using Application.DTO.Response;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Resources;
using Domain.Enums;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Threading.Tasks;

namespace Application.UseCases.WorkspaceCases;

public sealed class CriarConviteWorkspaceCase(
    IWorkspaceRepository workspaceRepository,
    IConviteWorkspaceRepository conviteWorkspaceRepository,
    ITokenTemporarioService tokenTemporarioService,
    IEnderecoFrontendService enderecoFrontendService,
    ConviteWorkspaceMapping mapping)
{
    private const int ValidadeConviteEmHoras = 24;

    public async Task<Resultado<ConviteWorkspaceCriadoResponse>> ExecutarAsync(
        int workspaceId,
        string remetenteCodigo)
    {
        var workspace = await workspaceRepository.ObterPorIdAsync(workspaceId);
        if (workspace.IsNullable())
            return Resultado<ConviteWorkspaceCriadoResponse>.Falha(WorkspaceResource.Erro_WorkspaceNaoEncontrado);

        if (workspace.Tipo == TipoWorkspace.Pessoal)
            return Resultado<ConviteWorkspaceCriadoResponse>.Falha(WorkspaceResource.Erro_ConviteWorkspacePessoal);

        if (remetenteCodigo.IsNullOrEmpty())
            return Resultado<ConviteWorkspaceCriadoResponse>.Falha(WorkspaceResource.Erro_ConviteRemetenteNaoMembro);

        var remetente = await workspaceRepository.ObterMembroAsync(workspaceId, remetenteCodigo);

        if (remetente.IsNullable())
            return Resultado<ConviteWorkspaceCriadoResponse>.Falha(WorkspaceResource.Erro_ConviteRemetenteNaoMembro);

        if (remetente.Tipo != TipoMembroWorkspace.Proprietario)
            return Resultado<ConviteWorkspaceCriadoResponse>.Falha(WorkspaceResource.Erro_ConviteSomenteProprietario);

        var token = tokenTemporarioService.Criar();
        var expiraEm = DateTime.UtcNow.AddHours(ValidadeConviteEmHoras).SemTimezone();

        var request = new CriarConviteWorkspaceRequest(workspaceId, remetenteCodigo, token.Hash, expiraEm);

        if (!await conviteWorkspaceRepository.CriarAsync(mapping.MapToEntity(request)))
        {
            return Resultado<ConviteWorkspaceCriadoResponse>.Falha(
                WorkspaceResource.Erro_ConvitePersistencia);
        }

        var uriBase = enderecoFrontendService.ObterUriBase();
        var response = new ConviteWorkspaceCriadoResponse($"{uriBase}/registrar?convite={Uri.EscapeDataString(token.Valor)}", expiraEm);

        return Resultado<ConviteWorkspaceCriadoResponse>
            .Sucesso(response)
            .AdicionarMensagem(WorkspaceResource.Mensagem_ConviteCriado);
    }
}