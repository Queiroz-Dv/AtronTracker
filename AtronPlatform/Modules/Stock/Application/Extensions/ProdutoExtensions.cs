using AtronStock.Application.DTO.Response;
using AtronStock.Domain.Entities;

namespace AtronStock.Application.Extensions
{
    public static class ProdutoExtensions
    {
        public static List<CategoriaProdutoResponse> MapearCategorias(this List<ProdutoCategoria> relacionamentos)
        {
            var responses = new List<CategoriaProdutoResponse>();

            foreach (var categoria in relacionamentos)
            {
                var response = new CategoriaProdutoResponse()
                {
                    Codigo = categoria.CategoriaCodigo,
                    Descricao = categoria.Categoria.Descricao ?? string.Empty
                };

                responses.Add(response);
            }

            return responses;
        }
    }
}