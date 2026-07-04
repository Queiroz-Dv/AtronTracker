using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    public class PlanejamentoCustoRelatorioService : IPlanejamentoCustoRelatorioService
    {
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;
        private readonly ICargoRepository _cargoRepository;

        public PlanejamentoCustoRelatorioService(
            IPlanejamentoCustoRepository planejamentoCustoRepository,
            ICargoRepository cargoRepository)
        {
            _planejamentoCustoRepository = planejamentoCustoRepository;
            _cargoRepository = cargoRepository;
        }

        public async Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano)
        {
            var cargos = (await _cargoRepository.ObterCargosAsync()).ToList();
            var planejamentos = (await _planejamentoCustoRepository.ObterPorAnoAsync(ano))
                .OrderBy(planejamento => planejamento.DepartamentoCodigo)
                .ToList();

            var relatorio = new PlanejamentoCustoRelatorioGeralDTO { Ano = ano };
            relatorio.Departamentos.AddRange(from planejamento in planejamentos
                                             select MontarDepartamentoRelatorio(planejamento, cargos));

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
                Departamentos = [MontarDepartamentoRelatorio(planejamento, cargos)]
            };

            return Resultado<PlanejamentoCustoRelatorioGeralDTO>.Sucesso(relatorio);
        }

        private static PlanejamentoCustoRelatorioDepartamentoDTO MontarDepartamentoRelatorio(
            PlanejamentoCusto planejamento,
            List<Cargo> cargos)
        {
            var departamentoRelatorio = new PlanejamentoCustoRelatorioDepartamentoDTO
            {
                DepartamentoCodigo = planejamento.DepartamentoCodigo,
                DepartamentoDescricao = planejamento.Departamento?.Descricao,
                PossuiPlanejamento = true,
                PlanejamentoCodigo = planejamento.Codigo,
                PlanejamentoDescricao = planejamento.Descricao,
                ApenasDepartamento = planejamento.ApenasDepartamento,
                ValorMinimoDepartamento = planejamento.ValorMinimo,
                ValorTetoDepartamento = planejamento.ValorTeto
            };

            if (planejamento.ApenasDepartamento)
            {
                departamentoRelatorio.Informacoes.Add("Planejamento definido apenas por departamento.");
                return departamentoRelatorio;
            }

            var cargosDepartamento = cargos
                .Where(cargo => cargo.DepartamentoCodigo == planejamento.DepartamentoCodigo)
                .OrderBy(cargo => cargo.Codigo)
                .ToList();

            var detalhesPorCargo = planejamento.DetalhesCargo?
                .ToDictionary(detalhe => detalhe.CargoCodigo, detalhe => detalhe)
                ?? [];

            departamentoRelatorio.CargosPendentes = cargosDepartamento
                .Where(cargo => !detalhesPorCargo.ContainsKey(cargo.Codigo))
                .Select(cargo => $"{cargo.Codigo} - {cargo.Descricao}")
                .ToList();

            var detalhes = planejamento.DetalhesCargo ?? [];
            departamentoRelatorio.QuantidadeCargosNaoDetalhados = detalhes.Count(detalhe => !detalhe.Detalhado);

            if (departamentoRelatorio.QuantidadeCargosNaoDetalhados > 0)
                departamentoRelatorio.Informacoes.Add($"{departamentoRelatorio.QuantidadeCargosNaoDetalhados} cargo(s) nao detalhado(s).");

            foreach (var detalhe in detalhes.Where(detalhe => detalhe.Detalhado).OrderBy(detalhe => detalhe.CargoCodigo))
            {
                var percentualCargo = planejamento.ValorTeto > 0
                    ? (detalhe.ValorTeto.GetValueOrDefault() / planejamento.ValorTeto) * 100
                    : 0;

                departamentoRelatorio.CargosDetalhados.Add(new PlanejamentoCustoRelatorioCargoDTO
                {
                    CargoCodigo = detalhe.CargoCodigo,
                    CargoDescricao = detalhe.Cargo?.Descricao,
                    ValorMinimo = detalhe.ValorMinimo.GetValueOrDefault(),
                    ValorTeto = detalhe.ValorTeto.GetValueOrDefault(),
                    PercentualOcupacaoTetoDepartamento = percentualCargo
                });
            }

            departamentoRelatorio.SomaMinimosCargos = departamentoRelatorio.CargosDetalhados.Sum(cargo => cargo.ValorMinimo);
            departamentoRelatorio.SomaTetosCargos = departamentoRelatorio.CargosDetalhados.Sum(cargo => cargo.ValorTeto);
            departamentoRelatorio.PercentualOcupacaoTeto = planejamento.ValorTeto > 0
                ? (departamentoRelatorio.SomaTetosCargos / planejamento.ValorTeto) * 100
                : 0;

            return departamentoRelatorio;
        }
    }
}
