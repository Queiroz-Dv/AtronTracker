using Application.DTO;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Domain.Entities;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPlanejamentoCustoPreparacaoService
    {
        Task<Resultado<PlanejamentoCustoPreparado>> PrepararCriacaoAsync(PlanejamentoCustoDTO planejamentoCustoDTO);

        Task<Resultado<PlanejamentoCustoPreparado>> PrepararAtualizacaoAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO);

        Task<Resultado<PlanejamentoCusto>> PrepararRemocaoAsync(string codigo);
    }
}
