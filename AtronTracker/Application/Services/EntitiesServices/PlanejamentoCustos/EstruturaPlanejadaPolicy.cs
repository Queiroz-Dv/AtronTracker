using Domain.Entities;
using Domain.Interfaces;
using Shared.Domain.ValueObjects;
using System;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    internal sealed class EstruturaPlanejadaPolicy
    {
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;

        public EstruturaPlanejadaPolicy(IPlanejamentoCustoRepository planejamentoCustoRepository)
        {
            _planejamentoCustoRepository = planejamentoCustoRepository;
        }

        public async Task<Resultado> ValidarRemocaoDepartamentoAsync(Departamento departamento)
        {
            var possuiPlanejamentoAtualOuFuturo = await _planejamentoCustoRepository
                .ExisteDepartamentoEmPlanejamentoAtualOuFuturoAsync(departamento.Id, departamento.Codigo, DateTime.Today.Year);

            return possuiPlanejamentoAtualOuFuturo
                ? Resultado.Falha($"O departamento {departamento.Codigo} possui planejamento de custo atual ou futuro e nao pode ser removido.")
                : Resultado.Sucesso();
        }

        public async Task<Resultado> ValidarRemocaoCargoAsync(Cargo cargo)
        {
            var possuiPlanejamentoAtualOuFuturo = await ExisteCargoPlanejadoAsync(cargo);

            return possuiPlanejamentoAtualOuFuturo
                ? Resultado.Falha($"O cargo {cargo.Codigo} possui planejamento de custo atual ou futuro e nao pode ser removido.")
                : Resultado.Sucesso();
        }

        public async Task<Resultado> ValidarMovimentacaoCargoAsync(Cargo cargo, Departamento novoDepartamento)
        {
            var alterouDepartamento = cargo.DepartamentoId != novoDepartamento.Id ||
                                      cargo.DepartamentoCodigo != novoDepartamento.Codigo;

            if (!alterouDepartamento)
                return Resultado.Sucesso();

            var possuiPlanejamentoAtualOuFuturo = await ExisteCargoPlanejadoAsync(cargo);

            return possuiPlanejamentoAtualOuFuturo
                ? Resultado.Falha($"O cargo {cargo.Codigo} possui planejamento de custo atual ou futuro e nao pode ser movido para outro departamento.")
                : Resultado.Sucesso();
        }

        private Task<bool> ExisteCargoPlanejadoAsync(Cargo cargo)
        {
            return _planejamentoCustoRepository
                .ExisteCargoEmPlanejamentoAtualOuFuturoAsync(cargo.Id, cargo.Codigo, cargo.DepartamentoId, cargo.DepartamentoCodigo, DateTime.Today.Year);
        }
    }
}
