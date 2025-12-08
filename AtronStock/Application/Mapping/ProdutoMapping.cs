using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Service;
using Shared.Application.Services.Mapper;

namespace AtronStock.Application.Mapping
{
    public class ProdutoMapping : AsyncApplicationMapService<ProdutoRequest, Produto>, IAsyncMap<ProdutoRequest, Produto>
    {
        public override Task<ProdutoRequest> MapToDTOAsync(Produto entity)
        {
            var produtoRequest = new ProdutoRequest
            {
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                CategoriaCodigos = entity.Categorias.Select(c => c.CategoriaCodigo).ToList()
            };
            return Task.FromResult(produtoRequest);
        }

        public Task MapToEntityAsync(ProdutoRequest dto, Produto entityToUpdate)
        {
            entityToUpdate.Codigo = dto.Codigo;
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.Categorias = dto.CategoriaCodigos.Select(codigo => new ProdutoCategoria { CategoriaCodigo = codigo }).ToList();
            return Task.CompletedTask;
        }

        public override Task<Produto> MapToEntityAsync(ProdutoRequest request)
        {
            var produto = new Produto
            {
                Codigo = request.Codigo,
                Descricao = request.Descricao,
                Categorias = request.CategoriaCodigos.Select(codigo => new ProdutoCategoria { CategoriaCodigo = codigo }).ToList()
            };

            return Task.FromResult(produto);
        }
    }
}
