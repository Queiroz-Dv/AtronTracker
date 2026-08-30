using AtronStock.Application.DTO.Response;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.UseCases.ProdutoCases;

public sealed class ObterProcessamentoProdutoCase(
    IProcessamentoProdutoLoteRepository repository,
    ProcessamentoProdutoMapping mapper,
    IUserAccessor userAccessor)
{
    public async Task<Resultado<ProcessamentoProdutoResponse>> ExecutarAsync(int id)
    {
        var solicitante = userAccessor.ObterCodigoUsuarioLogado();
        if (string.IsNullOrWhiteSpace(solicitante))
            return Resultado<ProcessamentoProdutoResponse>.Falha(
                ProdutoResource.ErroProcessamentoProdutoNaoEncontrado);

        var processamento = await repository.ObterPorIdDoSolicitanteAsync(
            id,
            solicitante);
        return processamento is null
            ? Resultado<ProcessamentoProdutoResponse>.Falha(
                ProdutoResource.ErroProcessamentoProdutoNaoEncontrado)
            : Resultado<ProcessamentoProdutoResponse>.Sucesso(mapper.MapToDto(processamento));
    }
}
