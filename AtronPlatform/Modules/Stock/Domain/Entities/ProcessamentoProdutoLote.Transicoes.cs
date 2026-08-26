#nullable enable

using AtronStock.Domain.Enums;

namespace AtronStock.Domain.Entities;

public sealed partial class ProcessamentoProdutoLote
{
    public void Reservar(
        DateTimeOffset agora,
        TimeSpan duracaoReserva,
        Guid tokenReserva)
    {
        var podeRecuperar = Status == EStatusProcessamentoProdutoLote.EmExecucao
            && ReservaExpiraEm <= agora;
        if (Status != EStatusProcessamentoProdutoLote.Pendente && !podeRecuperar)
            throw new InvalidOperationException(
                "Somente um processamento pendente ou abandonado pode ser reservado.");
        if (duracaoReserva <= TimeSpan.Zero || tokenReserva == Guid.Empty)
            throw new ArgumentException("A reserva precisa de duração e token válidos.");

        Status = EStatusProcessamentoProdutoLote.EmExecucao;
        Tentativas++;
        ReservadoEm = agora;
        ReservaExpiraEm = agora.Add(duracaoReserva);
        TokenReserva = tokenReserva;
        Resultado.LimparErro();
    }

    public void Concluir(
        int loteProdutoId,
        int quantidadeProcessada,
        Guid tokenReserva)
    {
        ExigirReserva(tokenReserva);
        Status = EStatusProcessamentoProdutoLote.Concluido;
        LoteProdutoId = loteProdutoId;
        Resultado.Concluir(quantidadeProcessada);
        LiberarReserva();
    }

    public void Falhar(string erro, Guid tokenReserva)
    {
        ExigirReserva(tokenReserva);
        Status = EStatusProcessamentoProdutoLote.Falha;
        Resultado.Falhar(erro);
        LiberarReserva();
    }

    private void ExigirReserva(Guid tokenReserva)
    {
        if (Status != EStatusProcessamentoProdutoLote.EmExecucao
            || TokenReserva != tokenReserva)
            throw new InvalidOperationException(
                "O processamento não pertence à reserva informada.");
    }

    private void LiberarReserva()
    {
        ReservaExpiraEm = null;
        TokenReserva = null;
    }
}
