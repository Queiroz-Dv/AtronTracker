using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Application.Records.PlanejamentoCusto
{
    public sealed record PlanejamentoCustoPreparadoRecord(
        PlanejamentoCustoDTO Dto,
        Domain.Entities.PlanejamentoCusto Entidade,
        Resultado ResultadoDetalhes);
}
