using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System;
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
        private readonly PlanejamentoCustoDetalhesCargoPreparador _detalhesCargoPreparador;
        private readonly PlanejamentoCustoIdentidadeAtualizacao _identidadeAtualizacao;

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
            _detalhesCargoPreparador = new PlanejamentoCustoDetalhesCargoPreparador(cargoRepository);
            _identidadeAtualizacao = new PlanejamentoCustoIdentidadeAtualizacao();
        }

        public async Task<Resultado<PlanejamentoCustoPreparado>> PrepararCriacaoAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoPreparado>.Falhas(erros);

            var planejamentoComMesmoCodigo = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(planejamentoCustoDTO.Codigo);
            if (planejamentoComMesmoCodigo != null)
                return Resultado<PlanejamentoCustoPreparado>.Falha(PlanejamentoCustoResource.Erro_CodigoExistente);

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(planejamentoCustoDTO.DepartamentoCodigo);
            if (departamento == null)
                return Resultado<PlanejamentoCustoPreparado>.Falha(PlanejamentoCustoResource.Erro_DepartamentoNaoEncontrado);

            var planejamentoExistente = await _planejamentoCustoRepository.ObterPorDepartamentoEAnoAsync(
                departamento.Id,
                departamento.Codigo,
                planejamentoCustoDTO.Ano);

            if (planejamentoExistente != null)
                return Resultado<PlanejamentoCustoPreparado>.Falha(PlanejamentoCustoResource.Erro_DepartamentoAnoExistente);

            var planejamento = await _asyncMap.MapToEntityAsync(planejamentoCustoDTO);
            planejamento.VincularDepartamento(departamento);

            var resultadoDetalhes = await _detalhesCargoPreparador.PrepararAsync(planejamentoCustoDTO, planejamento);
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
                return Resultado<PlanejamentoCustoPreparado>.Falha(PlanejamentoCustoResource.Erro_AnoPassadoNaoPodeSerEditado);

            var identidade = _identidadeAtualizacao.Aplicar(planejamento, planejamentoCustoDTO);
            if (identidade.TeveFalha)
                return Resultado<PlanejamentoCustoPreparado>.Falhas(identidade.Messages);

            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoPreparado>.Falhas(erros);

            await _asyncMap.MapToEntityAsync(planejamentoCustoDTO, planejamento);

            var resultadoDetalhes = await _detalhesCargoPreparador.PrepararAsync(planejamentoCustoDTO, planejamento);
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
                return Resultado<PlanejamentoCusto>.Falha(PlanejamentoCustoResource.Erro_AnoPassadoNaoPodeSerExcluido);

            return Resultado<PlanejamentoCusto>.Sucesso(planejamento);
        }

    }
}
