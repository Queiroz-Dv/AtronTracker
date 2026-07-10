using Application.Interfaces.Services;
using Shared.Domain.ValueObjects;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    public class PlanejamentoCustoRelatorioImpressaoService : IPlanejamentoCustoRelatorioImpressaoService
    {
        private readonly IPlanejamentoCustoRelatorioService _planejamentoCustoRelatorioService;
        private readonly PlanejamentoCustoRelatorioHtmlMontador _htmlMontador;

        public PlanejamentoCustoRelatorioImpressaoService(IPlanejamentoCustoRelatorioService planejamentoCustoRelatorioService)
        {
            _planejamentoCustoRelatorioService = planejamentoCustoRelatorioService;
            _htmlMontador = new PlanejamentoCustoRelatorioHtmlMontador();
        }

        public async Task<Resultado<string>> ObterHtmlRelatorioGeralAsync(int ano)
        {
            var relatorio = await _planejamentoCustoRelatorioService.ObterRelatorioGeralAsync(ano);
            return relatorio.TeveFalha
                ? Resultado<string>.Falhas(relatorio.Messages)
                : Resultado<string>.Sucesso(_htmlMontador.Montar(relatorio.Dados));
        }

        public async Task<Resultado<string>> ObterHtmlRelatorioPorCodigoAsync(string codigo)
        {
            var relatorio = await _planejamentoCustoRelatorioService.ObterRelatorioPorCodigoAsync(codigo);
            return relatorio.TeveFalha
                ? Resultado<string>.Falhas(relatorio.Messages)
                : Resultado<string>.Sucesso(_htmlMontador.Montar(relatorio.Dados));
        }
    }
}
