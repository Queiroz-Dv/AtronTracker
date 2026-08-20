using AtronStock.Application.DTO.Request;
using AtronStock.Domain.Entities;
using Shared.Application.Interfaces.Mapping;

namespace AtronStock.Application.Mapping
{
    public class ProdutoMapping : Mapper<Produto, ProdutoRequest>
    {       
        public override ProdutoRequest MapToDto(Produto entity)
        {
            return new ProdutoRequest
            {
                Codigo = entity.Codigo,
                Descricao = entity.Descricao,
                CategoriaCodigos = entity.Categorias.Select(c => c.CategoriaCodigo).ToList()
            };
        }

        public void MapToEntity(ProdutoRequest dto, Produto entityToUpdate)
        {
            entityToUpdate.Codigo = dto.Codigo;
            entityToUpdate.Descricao = dto.Descricao;
            entityToUpdate.Categorias = dto.CategoriaCodigos.Select(codigo => new ProdutoCategoria { CategoriaCodigo = codigo }).ToList();
        }

        public override Produto MapToEntity(ProdutoRequest request)
        {
            return new Produto
            {
                Codigo = request.Codigo,
                Descricao = request.Descricao,
                Categorias = request.CategoriaCodigos.Select(codigo => new ProdutoCategoria { CategoriaCodigo = codigo }).ToList()
            };

        }
    }
}