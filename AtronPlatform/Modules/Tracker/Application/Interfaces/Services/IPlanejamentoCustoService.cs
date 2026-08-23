using Application.DTO;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPlanejamentoCustoService
    {
        Task<Resultado> CriarAsync(PlanejamentoCustoDTO planejamentoCustoDTO);

        Task<Resultado> AtualizarAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO);

        Task<Resultado> RemoverAsync(string codigo);

        Task<Resultado<PlanejamentoCustoDTO>> ObterPorCodigoAsync(string codigo);

        Task<Resultado<List<PlanejamentoCustoDTO>>> ObterTodosAsync();

        Task<Resultado<List<PlanejamentoCustoDTO>>> ObterPorAnoAsync(int ano);

        Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano);

        Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioPorCodigoAsync(string codigo);
    }
}
