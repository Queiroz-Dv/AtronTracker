using System.Text.Json;
using AtronNotificacoes.Contracts.DTO.Request;
using AtronNotificacoes.Contracts.DTO.Response;
using Xunit;

namespace Notificacoes.Tests.Contratos;

public class NotificacoesInternasContractsSerializationTests
{
    [Theory]
    [InlineData("Tracker", "TarefaAtribuida", "tarefa:123")]
    [InlineData("Stock", "SaidaEstoqueRegistrada", "produto:45")]
    [InlineData("Sales", "PropostaAprovada", "proposta:78")]
    public void PublicarNotificacaoInternaRequest_DeveManterContratoJsonParaCadaProdutor(
        string moduloOrigem,
        string tipoEvento,
        string referenciaExterna)
    {
        var dataCriacao = new DateTimeOffset(2026, 7, 19, 14, 30, 0, TimeSpan.Zero);
        var request = new PublicarNotificacaoInternaRequest
        {
            DestinatarioCodigo = "USR001",
            ModuloOrigem = moduloOrigem,
            TipoEvento = tipoEvento,
            Titulo = "Nova tarefa atribuída",
            Mensagem = "A tarefa 123 foi atribuída a você.",
            UrlDestino = "/atron/tarefas/123",
            ReferenciaExterna = referenciaExterna,
            DataCriacao = dataCriacao,
            ChaveIdempotencia = $"{moduloOrigem.ToLowerInvariant()}:{referenciaExterna}:teste",
            CorrelacaoId = "correlacao-123"
        };

        var json = JsonSerializer.Serialize(request);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal("USR001", root.GetProperty("DestinatarioCodigo").GetString());
        Assert.Equal(moduloOrigem, root.GetProperty("ModuloOrigem").GetString());
        Assert.Equal(tipoEvento, root.GetProperty("TipoEvento").GetString());
        Assert.Equal(referenciaExterna, root.GetProperty("ReferenciaExterna").GetString());
        Assert.Equal($"{moduloOrigem.ToLowerInvariant()}:{referenciaExterna}:teste", root.GetProperty("ChaveIdempotencia").GetString());
        Assert.Equal("correlacao-123", root.GetProperty("CorrelacaoId").GetString());

        var desserializado = JsonSerializer.Deserialize<PublicarNotificacaoInternaRequest>(json);
        Assert.Equal(request, desserializado);
    }

    [Fact]
    public void NotificacaoInternaResponse_DeveManterContratoJson()
    {
        var dataCriacao = new DateTimeOffset(2026, 7, 19, 14, 30, 0, TimeSpan.Zero);
        var dataLeitura = dataCriacao.AddMinutes(5);
        var response = new NotificacaoInternaResponse(
            845923,
            "Stock",
            "EstoqueMinimo",
            "Estoque em atenção",
            "O produto atingiu o estoque mínimo.",
            "/atron-stock/produtos/45",
            "produto:45",
            true,
            dataCriacao,
            dataLeitura);

        var json = JsonSerializer.Serialize(response);

        using var document = JsonDocument.Parse(json);
        var root = document.RootElement;
        Assert.Equal("Stock", root.GetProperty("ModuloOrigem").GetString());
        Assert.Equal("EstoqueMinimo", root.GetProperty("TipoEvento").GetString());
        Assert.True(root.GetProperty("Lida").GetBoolean());
        Assert.Equal("produto:45", root.GetProperty("ReferenciaExterna").GetString());

        var desserializado = JsonSerializer.Deserialize<NotificacaoInternaResponse>(json);
        Assert.Equal(response, desserializado);
    }
}
