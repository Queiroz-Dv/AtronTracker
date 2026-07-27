using AtronStock.Domain.Entities;

namespace AtronStock.Application.Interfaces
{
    public interface IEstoqueService
    {
        Task ProcessarEntradaAsync(Entrada entrada);
        Task ProcessarVendaAsync(Venda venda);
    }
}
