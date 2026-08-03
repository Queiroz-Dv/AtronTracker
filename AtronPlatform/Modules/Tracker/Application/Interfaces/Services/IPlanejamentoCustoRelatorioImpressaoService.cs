using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Interfaces.Services
{
    public interface IPlanejamentoCustoRelatorioImpressaoService
    {
        Task<Resultado<string>> ObterHtmlRelatorioGeralAsync(int ano);

        Task<Resultado<string>> ObterHtmlRelatorioPorCodigoAsync(string codigo);
    }
}
