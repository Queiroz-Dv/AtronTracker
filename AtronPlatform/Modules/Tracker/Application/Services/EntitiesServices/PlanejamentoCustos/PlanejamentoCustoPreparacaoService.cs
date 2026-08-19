using Application.DTO;
using Application.Interfaces.Services;
using Application.Mapping;
using Application.Records.PlanejamentoCusto;
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
        private readonly PlanejamentoCustoMapping _planejamentoCustoMapping;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;
        private readonly IDepartamentoRepository _departamentoRepository;
        private readonly PlanejamentoCustoDetalhesCargoPreparador _detalhesCargoPreparador;
        private readonly PlanejamentoCustoIdentidadeAtualizacao _identidadeAtualizacao;

        public PlanejamentoCustoPreparacaoService(
            IValidador<PlanejamentoCustoDTO> validador,
            PlanejamentoCustoMapping planejamentoCustoMapping,
            IPlanejamentoCustoRepository planejamentoCustoRepository,
            IDepartamentoRepository departamentoRepository,
            ICargoRepository cargoRepository)
        {
            _validador = validador;
            _planejamentoCustoMapping = planejamentoCustoMapping;
            _planejamentoCustoRepository = planejamentoCustoRepository;
            _departamentoRepository = departamentoRepository;
            _detalhesCargoPreparador = new PlanejamentoCustoDetalhesCargoPreparador(cargoRepository);
            _identidadeAtualizacao = new PlanejamentoCustoIdentidadeAtualizacao();
        }

        public async Task<Resultado<PlanejamentoCustoPreparadoRecord>> PrepararCriacaoAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falhas(erros);

            var planejamentoComMesmoCodigo = await _planejamentoCustoRepository.ObterPorCodigoAsNoTrackingAsync(planejamentoCustoDTO.Codigo);
            if (planejamentoComMesmoCodigo != null)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falha(PlanejamentoCustoResource.Erro_CodigoExistente);

            var departamento = await _departamentoRepository.ObterDepartamentoPorCodigoRepositoryAsync(planejamentoCustoDTO.DepartamentoCodigo);
            if (departamento == null)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falha(PlanejamentoCustoResource.Erro_DepartamentoNaoEncontrado);

            var planejamentoExistente = await _planejamentoCustoRepository.ObterPorDepartamentoEAnoAsync(
                departamento.Id,
                departamento.Codigo,
                planejamentoCustoDTO.Ano);

            if (planejamentoExistente != null)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falha(PlanejamentoCustoResource.Erro_DepartamentoAnoExistente);

            var planejamento = _planejamentoCustoMapping.MapToEntity(planejamentoCustoDTO);
            planejamento.VincularDepartamento(departamento);

            var resultadoDetalhes = await _detalhesCargoPreparador.PrepararAsync(planejamentoCustoDTO, planejamento);
            if (resultadoDetalhes.TeveFalha)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falhas(resultadoDetalhes.Messages);

            return Resultado<PlanejamentoCustoPreparadoRecord>.Sucesso(
                new PlanejamentoCustoPreparadoRecord(planejamentoCustoDTO, planejamento, resultadoDetalhes));
        }

        public async Task<Resultado<PlanejamentoCustoPreparadoRecord>> PrepararAtualizacaoAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            if (codigo.IsNullOrEmpty())
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falha(NotificacoesPadronizadas.ErroCampoInvalido);

            var planejamento = await _planejamentoCustoRepository.ObterPorCodigoAsync(codigo);
            if (planejamento == null)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado);

            if (planejamento.Ano < DateTime.Today.Year)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falha(PlanejamentoCustoResource.Erro_AnoPassadoNaoPodeSerEditado);

            var identidade = _identidadeAtualizacao.Aplicar(planejamento, planejamentoCustoDTO);
            if (identidade.TeveFalha)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falhas(identidade.Messages);

            var erros = _validador.Validar(planejamentoCustoDTO);
            if (erros.Any())
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falhas(erros);

            _planejamentoCustoMapping.MapToEntity(planejamentoCustoDTO, planejamento);

            var resultadoDetalhes = await _detalhesCargoPreparador.PrepararAsync(planejamentoCustoDTO, planejamento);
            if (resultadoDetalhes.TeveFalha)
                return Resultado<PlanejamentoCustoPreparadoRecord>.Falhas(resultadoDetalhes.Messages);

            return Resultado<PlanejamentoCustoPreparadoRecord>.Sucesso(
                new PlanejamentoCustoPreparadoRecord(planejamentoCustoDTO, planejamento, resultadoDetalhes));
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
