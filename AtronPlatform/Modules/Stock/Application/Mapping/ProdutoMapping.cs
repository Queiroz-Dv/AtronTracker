#nullable enable

using AtronStock.Application.DTO.Response;
using AtronStock.Application.Extensions;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Mapping;
using Shared.Extensions;

namespace AtronStock.Application.Mapping
{
    public sealed class ProdutoMapping
        : Mapper<Produto, ProdutoCriacaoMappingInput, ProdutoResponse>,
          IUpdateMapper<Produto, ProdutoAtualizacaoMappingInput>
    {
        public override ProdutoResponse MapToDto(Produto entity)
            => new()
            {
                Id = entity.Id,
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                DescricaoComplementar = entity.DescricaoComplementar,
                DataAquisicao = entity.DataAquisicao,
                PrecoUnitario = entity.PrecoUnitario,
                DataEfetivaBaixa = entity.DataEfetivaBaixa,
                Status = entity.Status,
                LoteProdutoId = entity.LoteProdutoId,
                LoteProdutoCodigo = entity.LoteProduto?.Codigo,
                Categorias = entity.Categorias.MapearCategorias()
            };

        public override Produto MapToEntity(ProdutoCriacaoMappingInput input)
            => new()
            {
                Codigo = input.Request.Codigo.NormalizarCodigo(),
                Descricao = input.Request.Descricao,
                DescricaoComplementar = input.Request.DescricaoComplementar,
                DataAquisicao = input.Request.DataAquisicao,
                PrecoUnitario = input.Request.PrecoUnitario,
                Categorias = CriarRelacionamentos(
                    input.Request.Codigo,
                    input.Categorias)
            };

        public void MapToUpdate(ProdutoAtualizacaoMappingInput input, Produto produto)
        {
            produto.Descricao = input.Request.Descricao;
            produto.DescricaoComplementar = input.Request.DescricaoComplementar;
            produto.DataAquisicao = input.Request.DataAquisicao;
            produto.PrecoUnitario = input.Request.PrecoUnitario;
            produto.Categorias.Clear();
            produto.Categorias.AddRange(CriarRelacionamentos(
                produto.Codigo,
                input.Categorias));
        }

        private static List<ProdutoCategoria> CriarRelacionamentos(
            string produtoCodigo,
            IEnumerable<Categoria> categorias)
            => categorias.Select(categoria => new ProdutoCategoria
            {
                CategoriaId = categoria.Id,
                CategoriaCodigo = categoria.Codigo,
                Categoria = categoria,
                ProdutoCodigo = produtoCodigo.NormalizarCodigo()
            }).ToList();
    }
}
