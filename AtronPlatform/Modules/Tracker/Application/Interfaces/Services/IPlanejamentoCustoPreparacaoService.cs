using Application.DTO;
using Application.Records.PlanejamentoCusto;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPlanejamentoCustoPreparacaoService
    {
        Task<Resultado<PlanejamentoCustoPreparadoRecord>> PrepararCriacaoAsync(PlanejamentoCustoDTO planejamentoCustoDTO);

        Task<Resultado<PlanejamentoCustoPreparadoRecord>> PrepararAtualizacaoAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO);

        Task<Resultado<PlanejamentoCusto>> PrepararRemocaoAsync(string codigo);
    }
}
