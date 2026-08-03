using System.Globalization;
using AtronNotificacoes.Contracts;
using AtronStock.Application.Interfaces;
using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;

namespace AtronStock.Application.Services;

public sealed class EstoqueNotificacaoService(
    INotificacoesInternasPublisher publisher,
    IResponsavelNotificacaoEstoqueResolver responsavelResolver) : IEstoqueNotificacaoService
{
    public async Task NotificarSaidaRegistradaAsync(Venda venda, Produto produto, ItemVenda item, int saldoAtual)
    {
        var destinatarioCodigo = responsavelResolver.ObterCodigoResponsavel();
        if (string.IsNullOrWhiteSpace(destinatarioCodigo))
            return;

        try
        {
            await publisher.PublicarAsync(new PublicarNotificacaoInternaRequest(
                destinatarioCodigo,
                "Stock",
                "SaidaEstoqueRegistrada",
                EstoqueNotificacaoResource.Titulo_SaidaEstoqueRegistrada,
                string.Format(
                    CultureInfo.GetCultureInfo("pt-BR"),
                    EstoqueNotificacaoResource.Mensagem_SaidaEstoqueRegistrada,
                    produto.Codigo,
                    produto.Descricao,
                    item.Quantidade,
                    saldoAtual),
                null,
                $"produto:{produto.Id}",
                DateTimeOffset.UtcNow,
                $"stock:venda:{venda.Id}:item:{item.Id}",
                $"stock:venda:{venda.Id}:produto:{produto.Id}"));
        }
        catch
        {
            // A notificação é consultiva e não pode reverter a venda já registrada.
        }
    }
}
