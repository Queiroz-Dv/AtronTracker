using AtronStock.Application.DTO.Request;
using Shared.Domain.ValueObjects;

namespace AtronStock.Application.Interfaces
{
    public interface ICategoriaService
    {
        Task<Resultado> CriarAsync(CategoriaRequest dto);
        Task<Resultado> AtualizarAsync(CategoriaRequest dto);
        Task<Resultado> AtivarInativarAsync(string codigo, bool ativar);

        Task<Resultado<ICollection<CategoriaRequest>>> ObterTodasAsync();
        Task<Resultado<ICollection<CategoriaRequest>>> ObterInativasAsync();
        Task<Resultado<CategoriaRequest>> ObterPorCodigoAsync(string codigo);
    }
}