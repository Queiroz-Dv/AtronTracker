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

namespace Application.Services.EntitiesServices
{
    public class PlanejamentoCustoService : IPlanejamentoCustoService
    {
        private readonly IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto> _asyncMap;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly ICargoRepository _cargoRepository;
        private readonly IValidador<PlanejamentoCustoDTO> _validador;

        public PlanejamentoCustoService(
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

        public async Task<Resultado<PlanejamentoCustoDTO>> AtualizarAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCustoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsync(codigo);
            if (planejamento == null)
                return Resultado<PlanejamentoCustoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (planejamento.Ano < DateTime.Today.Year)
                return Resultado<PlanejamentoCustoDTO>.Falha("Planejamento de custo de ano passado não pode ser editado.");

            if (!planejamentoCustoDTO.Codigo.IsNullOrEmpty() &&
                planejamentoCustoDTO.Codigo != planejamento.Codigo)
                return Resultado<PlanejamentoCustoDTO>.Falha("O código do planejamento de custo não pode ser alterado.");

            if (planejamentoCustoDTO.Ano != planejamento.Ano)
                return Resultado<PlanejamentoCustoDTO>.Falha("O ano do planejamento de custo não pode ser alterado.");

            if (!planejamentoCustoDTO.DepartamentoCodigo.IsNullOrEmpty() &&
                planejamentoCustoDTO.DepartamentoCodigo != planejamento.DepartamentoCodigo)
                return Resultado<PlanejamentoCustoDTO>.Falha("O departamento do planejamento de custo não pode ser alterado.");

            planejamentoCustoDTO.Codigo = planejamento.Codigo;
            planejamentoCustoDTO.Id = planejamento.Id;
            planejamentoCustoDTO.Ano = planejamento.Ano;
            planejamentoCustoDTO.DepartamentoId = planejamento.DepartamentoId;
            planejamentoCustoDTO.DepartamentoCodigo = planejamento.DepartamentoCodigo;

            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoDTO>.Falhas(erros);

            await _asyncMap.MapToEntityAsync(planejamentoCustoDTO, planejamento);

            var resultadoDetalhes = await PrepararDetalhesCargoAsync(planejamentoCustoDTO, planejamento);
            if (resultadoDetalhes.TeveFalha)
                return Resultado<PlanejamentoCustoDTO>.Falhas(resultadoDetalhes.Messages);

            var atualizado = await _planejamentoCustoRepository.AtualizarAsync(planejamento);
            if (!atualizado)
                return Resultado<PlanejamentoCustoDTO>.Falha(string.Format("Não foi possível atualizar o planejamento de custo {0}.", codigo));

            var dto = await _asyncMap.MapToDTOAsync(planejamento);
            var resultado = Resultado<PlanejamentoCustoDTO>.Sucesso(dto).AdicionarMensagem(string.Format("Planejamento de custo {0} atualizado com sucesso.", codigo));
            foreach (var mensagem in resultadoDetalhes.Messages)
                resultado.Adicionar(mensagem);

            return resultado;
        }

        public async Task<Resultado<PlanejamentoCustoDTO>> CriarAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoDTO>.Falhas(erros);

            var planejamentoComMesmoCodigo = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(planejamentoCustoDTO.Codigo);
            if (planejamentoComMesmoCodigo != null)
                return Resultado<PlanejamentoCustoDTO>.Falha("Já existe planejamento de custo com este código.");

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(planejamentoCustoDTO.DepartamentoCodigo);
            if (departamento == null)
                return Resultado<PlanejamentoCustoDTO>.Falha("Departamento do planejamento de custo não encontrado.");

            var planejamentoExistente = await _planejamentoCustoRepository.ObterPorDepartamentoEAnoAsync(
                departamento.Id,
                departamento.Codigo,
                planejamentoCustoDTO.Ano);

            if (planejamentoExistente != null)
                return Resultado<PlanejamentoCustoDTO>.Falha("Já existe planejamento de custo para este departamento e ano.");

            var planejamento = await _asyncMap.MapToEntityAsync(planejamentoCustoDTO);
            planejamento.VincularDepartamento(departamento);

            var resultadoDetalhes = await PrepararDetalhesCargoAsync(planejamentoCustoDTO, planejamento);
            if (resultadoDetalhes.TeveFalha)
                return Resultado<PlanejamentoCustoDTO>.Falhas(resultadoDetalhes.Messages);

            var criado = await _planejamentoCustoRepository.CriarAsync(planejamento);
            if (!criado)
                return Resultado<PlanejamentoCustoDTO>.Falha("Não foi possível criar o planejamento de custo.");

            var dto = await _asyncMap.MapToDTOAsync(planejamento);
            var resultado = Resultado<PlanejamentoCustoDTO>.Sucesso(dto).ComMensagemRegistroSalvo(planejamento.Codigo);
            foreach (var mensagem in resultadoDetalhes.Messages)
                resultado.Adicionar(mensagem);

            return resultado;
        }

        public async Task<Resultado<PlanejamentoCustoDTO>> ObterPorCodigoAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCustoDTO>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(codigo);
            if (planejamento == null)
                return Resultado<PlanejamentoCustoDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            var dto = await _asyncMap.MapToDTOAsync(planejamento);
            return Resultado<PlanejamentoCustoDTO>.Sucesso(dto);
        }

        public async Task<Resultado<List<PlanejamentoCustoDTO>>> ObterPorAnoAsync(int ano)
        {
            var planejamentos = await _planejamentoCustoRepository.ObterPorAnoAsync(ano);
            var dtos = await _asyncMap.MapToListDTOAsync([.. planejamentos]);
            return Resultado<List<PlanejamentoCustoDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<List<PlanejamentoCustoDTO>>> ObterTodosAsync()
        {
            var planejamentos = await _planejamentoCustoRepository.ObterTodosAsync();
            var dtos = await _asyncMap.MapToListDTOAsync([.. planejamentos]);
            return Resultado<List<PlanejamentoCustoDTO>>.Sucesso(dtos);
        }

        public async Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano)
        {
            var cargos = (await _cargoRepository.ObterCargosAsync()).ToList();
            var planejamentos = (await _planejamentoCustoRepository.ObterPorAnoAsync(ano))
                .OrderBy(planejamento => planejamento.DepartamentoCodigo)
                .ToList();

            var relatorio = new PlanejamentoCustoRelatorioGeralDTO
            {
                Ano = ano
            };

            foreach (var planejamento in planejamentos)
            {
                relatorio.Departamentos.Add(MontarDepartamentoRelatorio(planejamento, cargos));
            }

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

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsync(codigo);
            if (planejamento == null)
                return Resultado.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (planejamento.Ano < DateTime.Today.Year)
                return Resultado.Falha("Planejamento de custo de ano passado não pode ser excluído.");

            var removido = await _planejamentoCustoRepository.RemoverAsync(planejamento);
            if (!removido)
                return Resultado.Falha(string.Format("Não foi possível remover o planejamento de custo {0}.", codigo));

            return Resultado.Sucesso().AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
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
