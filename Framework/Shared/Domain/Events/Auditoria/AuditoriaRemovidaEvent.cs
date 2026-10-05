using System;

namespace Shared.Domain.Events.Auditoria;

public record AuditoriaRemovidaEvent(
    string CodigoRegistro, 
    string Contexto, 
    string DescricaoHistorico, 
    Guid EventId, 
    DateTime DataOcorrencia) : IEvent
{
    public AuditoriaRemovidaEvent(string codigoRegistro, string contexto, string descricaoHistorico)
        : this(codigoRegistro, contexto, descricaoHistorico, Guid.NewGuid(), DateTime.UtcNow) { }
}
