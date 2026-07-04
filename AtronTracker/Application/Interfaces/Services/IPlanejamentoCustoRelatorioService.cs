using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPlanejamentoCustoRelatorioService
    {
        Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano);

        Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioPorCodigoAsync(string codigo);
    }
}
