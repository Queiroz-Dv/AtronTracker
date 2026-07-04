using Application.DTO;
using Domain.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    internal sealed class PlanejamentoCustoRelatorioDepartamentoMontador
    {
        public PlanejamentoCustoRelatorioDepartamentoDTO Montar(
            PlanejamentoCusto planejamento,
            IReadOnlyCollection<Cargo> cargos)
        {
            var departamentoRelatorio = CriarDepartamentoBase(planejamento);

            if (planejamento.ApenasDepartamento)
            {
                departamentoRelatorio.Informacoes.Add("Planejamento definido apenas por departamento.");
                return departamentoRelatorio;
            }

            var cargosDepartamento = ObterCargosDoDepartamento(planejamento, cargos);
            var detalhes = planejamento.DetalhesCargo ?? [];
            var detalhesPorCargo = detalhes.ToDictionary(detalhe => detalhe.CargoCodigo, detalhe => detalhe);

            departamentoRelatorio.CargosPendentes = ObterCargosPendentes(cargosDepartamento, detalhesPorCargo);
            departamentoRelatorio.QuantidadeCargosNaoDetalhados = detalhes.Count(detalhe => !detalhe.Detalhado);

            if (departamentoRelatorio.QuantidadeCargosNaoDetalhados > 0)
                departamentoRelatorio.Informacoes.Add($"{departamentoRelatorio.QuantidadeCargosNaoDetalhados} cargo(s) nao detalhado(s).");

            departamentoRelatorio.CargosDetalhados.AddRange(
                MontarCargosDetalhados(planejamento, detalhes));

            departamentoRelatorio.SomaMinimosCargos = departamentoRelatorio.CargosDetalhados.Sum(cargo => cargo.ValorMinimo);
            departamentoRelatorio.SomaTetosCargos = departamentoRelatorio.CargosDetalhados.Sum(cargo => cargo.ValorTeto);
            departamentoRelatorio.PercentualOcupacaoTeto = planejamento.ValorTeto > 0
                ? (departamentoRelatorio.SomaTetosCargos / planejamento.ValorTeto) * 100
                : 0;

            return departamentoRelatorio;
        }

        private static PlanejamentoCustoRelatorioDepartamentoDTO CriarDepartamentoBase(PlanejamentoCusto planejamento)
        {
            return new PlanejamentoCustoRelatorioDepartamentoDTO
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
        }

        private static List<Cargo> ObterCargosDoDepartamento(
            PlanejamentoCusto planejamento,
            IReadOnlyCollection<Cargo> cargos)
        {
            return cargos
                .Where(cargo => cargo.DepartamentoCodigo == planejamento.DepartamentoCodigo)
                .OrderBy(cargo => cargo.Codigo)
                .ToList();
        }

        private static List<string> ObterCargosPendentes(
            IEnumerable<Cargo> cargosDepartamento,
            IReadOnlyDictionary<string, PlanejamentoCustoCargo> detalhesPorCargo)
        {
            return cargosDepartamento
                .Where(cargo => !detalhesPorCargo.ContainsKey(cargo.Codigo))
                .Select(cargo => $"{cargo.Codigo} - {cargo.Descricao}")
                .ToList();
        }

        private static IEnumerable<PlanejamentoCustoRelatorioCargoDTO> MontarCargosDetalhados(
            PlanejamentoCusto planejamento,
            IEnumerable<PlanejamentoCustoCargo> detalhes)
        {
            foreach (var detalhe in detalhes.Where(detalhe => detalhe.Detalhado).OrderBy(detalhe => detalhe.CargoCodigo))
            {
                var percentualCargo = planejamento.ValorTeto > 0
                    ? (detalhe.ValorTeto.GetValueOrDefault() / planejamento.ValorTeto) * 100
                    : 0;

                yield return new PlanejamentoCustoRelatorioCargoDTO
                {
                    CargoCodigo = detalhe.CargoCodigo,
                    CargoDescricao = detalhe.Cargo?.Descricao,
                    ValorMinimo = detalhe.ValorMinimo.GetValueOrDefault(),
                    ValorTeto = detalhe.ValorTeto.GetValueOrDefault(),
                    PercentualOcupacaoTetoDepartamento = percentualCargo
                };
            }
        }
    }
}
