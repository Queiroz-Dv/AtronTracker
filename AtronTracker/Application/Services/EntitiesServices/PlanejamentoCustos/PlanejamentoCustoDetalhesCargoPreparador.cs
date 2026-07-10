using Application.DTO;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    internal sealed class PlanejamentoCustoDetalhesCargoPreparador
    {
        private readonly ICargoRepository _cargoRepository;

        public PlanejamentoCustoDetalhesCargoPreparador(ICargoRepository cargoRepository)
        {
            _cargoRepository = cargoRepository;
        }

        public async Task<Resultado> PrepararAsync(PlanejamentoCustoDTO dto, PlanejamentoCusto planejamento)
        {
            planejamento.DetalhesCargo ??= [];

            if (dto.ApenasDepartamento)
            {
                planejamento.DetalhesCargo.Clear();
                return Resultado.Sucesso();
            }

            var cargosDepartamento = (await _cargoRepository
                .ObterCargosPorDepartamento(planejamento.DepartamentoId, planejamento.DepartamentoCodigo))
                .ToList();

            if (!cargosDepartamento.Any())
                return Resultado.Falha("O departamento não possui cargos. Use planejamento apenas por departamento.");

            var detalhesRecebidos = dto.DetalhesCargo ?? [];

            if (detalhesRecebidos.Any(detalhe => detalhe.CargoCodigo.IsNullOrEmpty()))
                return Resultado.Falha("Todos os detalhes de cargo devem informar o código do cargo.");

            var cargosDuplicados = detalhesRecebidos
                .GroupBy(detalhe => detalhe.CargoCodigo)
                .Where(grupo => grupo.Count() > 1)
                .Select(grupo => grupo.Key)
                .ToList();

            if (cargosDuplicados.Any())
                return Resultado.Falha($"Existem cargos duplicados no planejamento: {string.Join(", ", cargosDuplicados)}.");

            var codigosRecebidos = detalhesRecebidos
                .Select(d => d.CargoCodigo)
                .Where(c => !c.IsNullOrEmpty())
                .ToHashSet();

            var cargosPorCodigo = cargosDepartamento.ToDictionary(c => c.Codigo, c => c);

            var cargosPendentes = cargosDepartamento
                .Where(cargo => !codigosRecebidos.Contains(cargo.Codigo))
                .Select(cargo => cargo.Codigo)
                .ToList();

            if (cargosPendentes.Any())
                return Resultado.Falha($"Existem cargos pendentes de decisão no planejamento: {string.Join(", ", cargosPendentes)}.");

            var cargosInvalidos = detalhesRecebidos
                .Where(detalhe => !detalhe.CargoCodigo.IsNullOrEmpty() && !cargosPorCodigo.ContainsKey(detalhe.CargoCodigo))
                .Select(detalhe => detalhe.CargoCodigo)
                .Distinct()
                .ToList();

            if (cargosInvalidos.Any())
                return Resultado.Falha($"Existem cargos que não pertencem ao departamento do planejamento: {string.Join(", ", cargosInvalidos)}.");

            if (!detalhesRecebidos.Any(detalhe => detalhe.Detalhado))
                return Resultado.Falha("Todos os cargos foram marcados como não detalhados. Use planejamento apenas por departamento.");

            decimal somaMinimos = 0;
            decimal somaTetos = 0;
            var detalhesPreparados = new List<PlanejamentoCustoCargo>();

            foreach (var detalhe in detalhesRecebidos)
            {
                var cargo = cargosPorCodigo[detalhe.CargoCodigo];

                if (detalhe.Detalhado)
                {
                    if (!detalhe.ValorMinimo.HasValue || !detalhe.ValorTeto.HasValue)
                        return Resultado.Falha($"Informe valor mínimo e teto para o cargo {cargo.Codigo}.");

                    if (detalhe.ValorMinimo.Value < 0)
                        return Resultado.Falha($"O valor mínimo do cargo {cargo.Codigo} não pode ser negativo.");

                    if (detalhe.ValorTeto.Value <= 0)
                        return Resultado.Falha($"O valor teto do cargo {cargo.Codigo} deve ser maior que zero.");

                    if (detalhe.ValorMinimo.Value >= detalhe.ValorTeto.Value)
                        return Resultado.Falha($"O valor mínimo do cargo {cargo.Codigo} deve ser menor que o teto.");

                    somaMinimos += detalhe.ValorMinimo.Value;
                    somaTetos += detalhe.ValorTeto.Value;
                }

                detalhesPreparados.Add(new PlanejamentoCustoCargo
                {
                    CargoId = cargo.Id,
                    CargoCodigo = cargo.Codigo,
                    Detalhado = detalhe.Detalhado,
                    ValorMinimo = detalhe.Detalhado ? detalhe.ValorMinimo : null,
                    ValorTeto = detalhe.Detalhado ? detalhe.ValorTeto : null
                });
            }

            if (somaTetos > planejamento.ValorTeto)
                return Resultado.Falha("A soma dos tetos dos cargos não pode ultrapassar o teto do departamento.");

            planejamento.DetalhesCargo.Clear();
            foreach (var detalhe in detalhesPreparados)
                planejamento.DetalhesCargo.Add(detalhe);

            var resultado = Resultado.Sucesso();
            if (somaMinimos != planejamento.ValorMinimo)
                resultado.AdicionarAviso("A soma dos mínimos dos cargos detalhados diverge do mínimo do departamento.");

            return resultado;
        }
    }
}
