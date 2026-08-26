using AtronStock.Application.DTO.Response;
using AtronStock.Domain.Entities;

namespace AtronStock.Application.Mapping;

public sealed class ProcessamentoProdutoMapping
{
    public ProcessamentoProdutoResponse MapToDto(ProcessamentoProdutoLote entity)
        => new()
        {
            Id = entity.Id,
            Status = entity.Status,
            CodigoBase = entity.Solicitacao.CodigoBase,
            QuantidadeSolicitada = entity.Solicitacao.QuantidadeSolicitada,
            QuantidadeProcessada = entity.Resultado.QuantidadeProcessada,
            Descricao = entity.Solicitacao.Descricao,
            DescricaoComplementar = entity.Solicitacao.DescricaoComplementar,
            DataAquisicao = entity.Solicitacao.DataAquisicao,
            PrecoUnitario = entity.Solicitacao.PrecoUnitario,
            CategoriaCodigos = entity.Solicitacao.CategoriaCodigos.ToList(),
            LoteProdutoId = entity.LoteProdutoId,
            LoteProdutoCodigo = entity.LoteProduto?.Codigo,
            Erro = entity.Resultado.Erro
        };
}
