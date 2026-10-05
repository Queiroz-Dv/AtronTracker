namespace Shared.Domain.Events.Auditoria;

public record AuditoriaRegistradaEvent(
    string CodigoRegistro,
    string Contexto,
    string DescricaoHistorico,
    Guid EventId,
    DateTime DataOcorrencia) : IEvent
{
    public AuditoriaRegistradaEvent(string codigoRegistro, string contexto, string descricaoHistorico)
        : this(codigoRegistro, contexto, descricaoHistorico, Guid.NewGuid(), DateTime.UtcNow) { }
}