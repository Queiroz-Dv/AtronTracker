using AtronStock.Domain.Entities;

namespace AtronStock.Domain.Interfaces
{
    public interface ICategoriaRepository
    {
        Task<bool> CriarCategoriaAsync(Categoria categoria);

        Task<ICollection<Categoria>> ObterTodasCategoriasAsync();

        Task<ICollection<Categoria>> ObterTodasCategoriasInativasAsync();

        Task<Categoria> ObterCategoriaPorCodigoAsync(string codigo);

        Task<ICollection<Categoria>> ObterPorCodigosAsync(IReadOnlyCollection<string> codigos);

        Task<bool> PossuiProdutosVinculadosAsync(int categoriaId);

        Task<bool> AtualizarCategoriaAsync(Categoria categoria);
    }
}
