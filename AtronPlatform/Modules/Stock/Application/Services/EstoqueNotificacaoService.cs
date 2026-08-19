using System.Globalization;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.Interfaces;
using AtronStock.Application.Interfaces;
using AtronStock.Application.Providers.Notificacoes;
using AtronStock.Application.Resources;
using AtronStock.Domain.Entities;

namespace AtronStock.Application.Services;

public sealed class EstoqueNotificacaoService(
    INotificacoesInternasPublisher publisher,
    ResponsavelNotificacaoEstoqueProvider responsavelProvider) : IEstoqueNotificacaoService
{
    public async Task NotificarSaidaRegistradaAsync(Venda venda, Produto produto, ItemVenda item, int saldoAtual)
    {
        var destinatarioCodigo = responsavelProvider.ObterCodigoResponsavel();
        if (string.IsNullOrWhiteSpace(destinatarioCodigo))
            return;

        try
        {
            await publisher.PublicarAsync(new PublicarNotificacaoInternaRequest
            {
                DestinatarioCodigo = destinatarioCodigo,
                ModuloOrigem = "Stock",
                TipoEvento = "SaidaEstoqueRegistrada",
                Titulo = EstoqueNotificacaoResource.Titulo_SaidaEstoqueRegistrada,
                Mensagem = string.Format(
                    CultureInfo.GetCultureInfo("pt-BR"),
                    EstoqueNotificacaoResource.Mensagem_SaidaEstoqueRegistrada,
                    produto.Codigo,
                    produto.Descricao,
                    item.Quantidade,
                    saldoAtual),
                UrlDestino = null,
                ReferenciaExterna = $"produto:{produto.Id}",
                DataCriacao = DateTimeOffset.UtcNow,
                ChaveIdempotencia = $"stock:venda:{venda.Id}:item:{item.Id}",
                CorrelacaoId = $"stock:venda:{venda.Id}:produto:{produto.Id}"
            });
        }
        catch
        {
            // A notificação é consultiva e não pode reverter a venda já registrada.
        }
    }
}
