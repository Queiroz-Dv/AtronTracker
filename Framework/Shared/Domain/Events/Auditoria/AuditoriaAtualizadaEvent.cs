using System;

namespace Shared.Domain.Events.Auditoria;

public record AuditoriaAtualizadaEvent(
    string CodigoRegistro, 
    string Contexto, 
    string DescricaoHistorico, 
    Guid EventId, 
    DateTime DataOcorrencia) : IEvent
{
    public AuditoriaAtualizadaEvent(string codigoRegistro, string contexto, string descricaoHistorico)
        : this(codigoRegistro, contexto, descricaoHistorico, Guid.NewGuid(), DateTime.UtcNow) { }
}
