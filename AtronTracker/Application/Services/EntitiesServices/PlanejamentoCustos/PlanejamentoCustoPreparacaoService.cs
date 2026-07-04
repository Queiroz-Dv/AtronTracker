using Application.DTO;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices.PlanejamentoCustos
{
    public class PlanejamentoCustoPreparacaoService : IPlanejamentoCustoPreparacaoService
    {
        private readonly IValidador<PlanejamentoCustoDTO> _validador;
        private readonly IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto> _asyncMap;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;

        public PlanejamentoCustoPreparacaoService(
            IValidador<PlanejamentoCustoDTO> validador,
            IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto> asyncMap,
            IPlanejamentoCustoRepository planejamentoCustoRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository)
        {
            _validador = validador;
            _asyncMap = asyncMap;
            _planejamentoCustoRepository = planejamentoCustoRepository;
            _departamentoRepository = departamentoRepository;
            _cargoRepository = cargoRepository;
        }

        public async Task<Resultado<PlanejamentoCustoPreparado>> PrepararCriacaoAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoPreparado>.Falhas(erros);

            var planejamentoComMesmoCodigo = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(planejamentoCustoDTO.Codigo);
            if (planejamentoComMesmoCodigo != null)
                return Resultado<PlanejamentoCustoPreparado>.Falha("Ja existe planejamento de custo com este codigo.");

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(planejamentoCustoDTO.DepartamentoCodigo);
            if (departamento == null)
                return Resultado<PlanejamentoCustoPreparado>.Falha("Departamento do planejamento de custo nao encontrado.");

            var planejamentoExistente = await _planejamentoCustoRepository.ObterPorDepartamentoEAnoAsync(
                departamento.Id,
                departamento.Codigo,
                planejamentoCustoDTO.Ano);

            if (planejamentoExistente != null)
                return Resultado<PlanejamentoCustoPreparado>.Falha("Ja existe planejamento de custo para este departamento e ano.");

            var planejamento = await _asyncMap.MapToEntityAsync(planejamentoCustoDTO);
            planejamento.VincularDepartamento(departamento);

            var resultadoDetalhes = await PrepararDetalhesCargoAsync(planejamentoCustoDTO, planejamento);
            if (resultadoDetalhes.TeveFalha)
                return Resultado<PlanejamentoCustoPreparado>.Falhas(resultadoDetalhes.Messages);

            return Resultado<PlanejamentoCustoPreparado>.Sucesso(
                new PlanejamentoCustoPreparado(planejamentoCustoDTO, planejamento, resultadoDetalhes));
        }

        public async Task<Resultado<PlanejamentoCustoPreparado>> PrepararAtualizacaoAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCustoPreparado>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsync(codigo);
            if (planejamento == null)
                return Resultado<PlanejamentoCustoPreparado>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (planejamento.Ano < DateTime.Today.Year)
                return Resultado<PlanejamentoCustoPreparado>.Falha("Planejamento de custo de ano passado nao pode ser editado.");

            if (!planejamentoCustoDTO.Codigo.IsNullOrEmpty() &&
                planejamentoCustoDTO.Codigo != planejamento.Codigo)
                return Resultado<PlanejamentoCustoPreparado>.Falha("O codigo do planejamento de custo nao pode ser alterado.");

            if (planejamentoCustoDTO.Ano != planejamento.Ano)
                return Resultado<PlanejamentoCustoPreparado>.Falha("O ano do planejamento de custo nao pode ser alterado.");

            if (!planejamentoCustoDTO.DepartamentoCodigo.IsNullOrEmpty() &&
                planejamentoCustoDTO.DepartamentoCodigo != planejamento.DepartamentoCodigo)
                return Resultado<PlanejamentoCustoPreparado>.Falha("O departamento do planejamento de custo nao pode ser alterado.");

            planejamentoCustoDTO.Codigo = planejamento.Codigo;
            planejamentoCustoDTO.Id = planejamento.Id;
            planejamentoCustoDTO.Ano = planejamento.Ano;
            planejamentoCustoDTO.DepartamentoId = planejamento.DepartamentoId;
            planejamentoCustoDTO.DepartamentoCodigo = planejamento.DepartamentoCodigo;

            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoPreparado>.Falhas(erros);

            await _asyncMap.MapToEntityAsync(planejamentoCustoDTO, planejamento);

            var resultadoDetalhes = await PrepararDetalhesCargoAsync(planejamentoCustoDTO, planejamento);
            if (resultadoDetalhes.TeveFalha)
                return Resultado<PlanejamentoCustoPreparado>.Falhas(resultadoDetalhes.Messages);

            return Resultado<PlanejamentoCustoPreparado>.Sucesso(
                new PlanejamentoCustoPreparado(planejamentoCustoDTO, planejamento, resultadoDetalhes));
        }

        public async Task<Resultado<PlanejamentoCusto>> PrepararRemocaoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCusto>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsync(codigo);
            if (planejamento == null)
                return Resultado<PlanejamentoCusto>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (planejamento.Ano < DateTime.Today.Year)
                return Resultado<PlanejamentoCusto>.Falha("Planejamento de custo de ano passado nao pode ser excluido.");

            return Resultado<PlanejamentoCusto>.Sucesso(planejamento);
        }

        private async Task<Resultado> PrepararDetalhesCargoAsync(PlanejamentoCustoDTO dto, PlanejamentoCusto planejamento)
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
                return Resultado.Falha("O departamento nao possui cargos. Use planejamento apenas por departamento.");

            var detalhesRecebidos = dto.DetalhesCargo ?? [];

            if (detalhesRecebidos.Any(detalhe => detalhe.CargoCodigo.IsNullOrEmpty()))
                return Resultado.Falha("Todos os detalhes de cargo devem informar o codigo do cargo.");

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
                return Resultado.Falha($"Existem cargos pendentes de decisao no planejamento: {string.Join(", ", cargosPendentes)}.");

            var cargosInvalidos = detalhesRecebidos
                .Where(detalhe => !detalhe.CargoCodigo.IsNullOrEmpty() && !cargosPorCodigo.ContainsKey(detalhe.CargoCodigo))
                .Select(detalhe => detalhe.CargoCodigo)
                .Distinct()
                .ToList();

            if (cargosInvalidos.Any())
                return Resultado.Falha($"Existem cargos que nao pertencem ao departamento do planejamento: {string.Join(", ", cargosInvalidos)}.");

            if (!detalhesRecebidos.Any(detalhe => detalhe.Detalhado))
                return Resultado.Falha("Todos os cargos foram marcados como nao detalhados. Use planejamento apenas por departamento.");

            decimal somaMinimos = 0;
            decimal somaTetos = 0;
            var detalhesPreparados = new List<PlanejamentoCustoCargo>();

            foreach (var detalhe in detalhesRecebidos)
            {
                var cargo = cargosPorCodigo[detalhe.CargoCodigo];

                if (detalhe.Detalhado)
                {
                    if (!detalhe.ValorMinimo.HasValue || !detalhe.ValorTeto.HasValue)
                        return Resultado.Falha($"Informe valor minimo e teto para o cargo {cargo.Codigo}.");

                    if (detalhe.ValorMinimo.Value < 0)
                        return Resultado.Falha($"O valor minimo do cargo {cargo.Codigo} nao pode ser negativo.");

                    if (detalhe.ValorTeto.Value <= 0)
                        return Resultado.Falha($"O valor teto do cargo {cargo.Codigo} deve ser maior que zero.");

                    if (detalhe.ValorMinimo.Value >= detalhe.ValorTeto.Value)
                        return Resultado.Falha($"O valor minimo do cargo {cargo.Codigo} deve ser menor que o teto.");

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
                return Resultado.Falha("A soma dos tetos dos cargos nao pode ultrapassar o teto do departamento.");

            planejamento.DetalhesCargo.Clear();
            foreach (var detalhe in detalhesPreparados)
                planejamento.DetalhesCargo.Add(detalhe);

            var resultado = Resultado.Sucesso();
            if (somaMinimos != planejamento.ValorMinimo)
                resultado.AdicionarAviso("A soma dos minimos dos cargos detalhados diverge do minimo do departamento.");

            return resultado;
        }
    }
}
