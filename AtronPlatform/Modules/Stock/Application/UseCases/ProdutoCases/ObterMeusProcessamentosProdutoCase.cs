using AtronStock.Application.DTO.Response;
using AtronStock.Application.Mapping;
using AtronStock.Application.Resources;
using AtronStock.Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.UseCases.ProdutoCases;

public sealed class ObterMeusProcessamentosProdutoCase(
    IProcessamentoProdutoLoteRepository repository,
    ProcessamentoProdutoMapping mapper,
    IUserAccessor userAccessor)
{
    public async Task<Resultado<ICollection<ProcessamentoProdutoResponse>>> ExecutarAsync()
    {
        var solicitante = userAccessor.ObterCodigoUsuarioLogado();
        if (string.IsNullOrWhiteSpace(solicitante))
            return Resultado<ICollection<ProcessamentoProdutoResponse>>.Falha(
                ProdutoResource.ErroSolicitanteNaoIdentificado);

        var processamentos = await repository.ObterMeusAsync(
            solicitante.Trim().ToUpperInvariant());
        return Resultado<ICollection<ProcessamentoProdutoResponse>>.Sucesso(
            processamentos.Select(mapper.MapToDto).ToList());
    }
}
