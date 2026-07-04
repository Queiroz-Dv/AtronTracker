using Application.DTO;
using Domain.Entities;
using Shared.Domain.ValueObjects;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    public sealed record PlanejamentoCustoPreparado(
        PlanejamentoCustoDTO Dto,
        PlanejamentoCusto Entidade,
        Resultado ResultadoDetalhes);
}
