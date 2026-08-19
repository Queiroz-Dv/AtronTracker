using AtronStock.Domain.Entities;

namespace AtronStock.Application.Interfaces;

public interface IEstoqueNotificacaoService
{
    Task NotificarSaidaRegistradaAsync(Venda venda, Produto produto, ItemVenda item, int saldoAtual);
}
