using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    public class PlanejamentoCustoRelatorioService : IPlanejamentoCustoRelatorioService
    {
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly PlanejamentoCustoRelatorioDepartamentoMontador _departamentoMontador;

        public PlanejamentoCustoRelatorioService(
            IPlanejamentoCustoRepository planejamentoCustoRepository,
            ICargoRepository cargoRepository)
        {
            _planejamentoCustoRepository = planejamentoCustoRepository;
            _cargoRepository = cargoRepository;
            _departamentoMontador = new PlanejamentoCustoRelatorioDepartamentoMontador();
        }

        public async Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano)
        {
            var cargos = (await _cargoRepository.ObterCargosAsync()).ToList();
            var planejamentos = (await _planejamentoCustoRepository.ObterPorAnoAsync(ano))
                .OrderBy(planejamento => planejamento.DepartamentoCodigo)
                .ToList();

            var relatorio = new PlanejamentoCustoRelatorioGeralDTO { Ano = ano };
            relatorio.Departamentos.AddRange(from planejamento in planejamentos
                                             select _departamentoMontador.Montar(planejamento, cargos));

            return Resultado<PlanejamentoCustoRelatorioGeralDTO>.Sucesso(relatorio);
        }

        public async Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCustoRelatorioGeralDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(codigo);
            if (planejamento == null)
                return Resultado<PlanejamentoCustoRelatorioGeralDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var cargos = (await _cargoRepository.ObterCargosAsync()).ToList();
            var relatorio = new PlanejamentoCustoRelatorioGeralDTO
            {
                Ano = planejamento.Ano,
                Departamentos = [_departamentoMontador.Montar(planejamento, cargos)]
            };

            return Resultado<PlanejamentoCustoRelatorioGeralDTO>.Sucesso(relatorio);
        }
    }
}
