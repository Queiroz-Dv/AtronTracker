#nullable enable

using System.ComponentModel.DataAnnotations;
using AtronStock.Domain.Enums;
using AtronStock.Domain.ValueObjects;

namespace AtronStock.Domain.Entities;

public sealed partial class ProcessamentoProdutoLote
{
    private ProcessamentoProdutoLote()
    {
    }

    public ProcessamentoProdutoLote(SolicitacaoGeracaoProdutosLote solicitacao)
    {
        Solicitacao = solicitacao
            ?? throw new ArgumentNullException(nameof(solicitacao));
    }

    [Key]
    public int Id { get; set; }

    public EStatusProcessamentoProdutoLote Status { get; private set; }
        = EStatusProcessamentoProdutoLote.Pendente;

    public SolicitacaoGeracaoProdutosLote Solicitacao { get; private set; } = null!;

    public ResultadoProcessamentoProdutoLote Resultado { get; private set; } = new();

    public int? LoteProdutoId { get; set; }

    public LoteProduto? LoteProduto { get; set; }

    public int Tentativas { get; private set; }

    public DateTimeOffset? ReservadoEm { get; private set; }

    public DateTimeOffset? ReservaExpiraEm { get; private set; }

    public Guid? TokenReserva { get; private set; }
}
