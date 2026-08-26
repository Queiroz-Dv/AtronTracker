using AtronStock.Application.DTO.Response;
using AtronStock.Application.Interfaces;
using AtronStock.Application.UseCases.ProdutoCases;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Services;

public sealed class ProcessamentoProdutoService(
    ObterMeusProcessamentosProdutoCase obterMeus,
    ObterProcessamentoProdutoCase obterPorId) : IProcessamentoProdutoService
{
    public Task<Resultado<ICollection<ProcessamentoProdutoResponse>>> ObterMeusAsync()
        => obterMeus.ExecutarAsync();

    public Task<Resultado<ProcessamentoProdutoResponse>> ObterPorIdAsync(int id)
        => obterPorId.ExecutarAsync(id);
}
