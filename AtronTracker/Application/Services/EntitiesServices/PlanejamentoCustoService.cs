using Application.DTO;
using Application.Interfaces.Services;
using Application.Resources;
using Domain.Entities;
using Domain.Interfaces;
using Shared.Application.Interfaces.Service;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Shared.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class PlanejamentoCustoService : IPlanejamentoCustoService
    {
        private readonly IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto> _asyncMap;
        private readonly IPlanejamentoCustoRepository _planejamentoCustoRepository;
        private readonly IPlanejamentoCustoPreparacaoService _planejamentoCustoPreparacaoService;
        private readonly IPlanejamentoCustoRelatorioService _planejamentoCustoRelatorioService;

        public PlanejamentoCustoService(
            IAsyncMap<PlanejamentoCustoDTO, PlanejamentoCusto> asyncMap,
            IPlanejamentoCustoRepository planejamentoCustoRepository,
            IPlanejamentoCustoPreparacaoService planejamentoCustoPreparacaoService,
            IPlanejamentoCustoRelatorioService planejamentoCustoRelatorioService)
        {
            _asyncMap = asyncMap;
            _planejamentoCustoRepository = planejamentoCustoRepository;
            _planejamentoCustoPreparacaoService = planejamentoCustoPreparacaoService;
            _planejamentoCustoRelatorioService = planejamentoCustoRelatorioService;
        }

        public async Task<Resultado<PlanejamentoCustoDTO>> CriarAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var preparacao = await _planejamentoCustoPreparacaoService.PrepararCriacaoAsync(planejamentoCustoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PlanejamentoCustoDTO>.Falhas(preparacao.Messages);

            var criado = await _planejamentoCustoRepository.CriarAsync(preparacao.Dados.Entidade);
            if (!criado)
                return Resultado<PlanejamentoCustoDTO>.Falha(PlanejamentoCustoResource.Erro_CriarPlanejamento);

            var dto = await _asyncMap.MapToDTOAsync(preparacao.Dados.Entidade);
            var resultado = Resultado<PlanejamentoCustoDTO>.Sucesso(dto).ComMensagemRegistroSalvo(preparacao.Dados.Entidade.Codigo);
            AdicionarMensagensDetalhes(resultado, preparacao.Dados.ResultadoDetalhes);

            return resultado;
        }

        public async Task<Resultado<PlanejamentoCustoDTO>> AtualizarAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO)
        {
            var preparacao = await _planejamentoCustoPreparacaoService.PrepararAtualizacaoAsync(codigo, planejamentoCustoDTO);
            if (preparacao.TeveFalha)
                return Resultado<PlanejamentoCustoDTO>.Falhas(preparacao.Messages);

            var atualizado = await _planejamentoCustoRepository.AtualizarAsync(preparacao.Dados.Entidade);
            if (!atualizado)
                return Resultado<PlanejamentoCustoDTO>.Falha(string.Format(PlanejamentoCustoResource.Erro_AtualizarPlanejamento, codigo));

            var dto = await _asyncMap.MapToDTOAsync(preparacao.Dados.Entidade);
            var resultado = Resultado<PlanejamentoCustoDTO>
                .Sucesso(dto)
                .AdicionarMensagem(string.Format("Planejamento de custo {0} atualizado com sucesso.", codigo));
            AdicionarMensagensDetalhes(resultado, preparacao.Dados.ResultadoDetalhes);

            return resultado;
        }

        public async Task<Resultado> RemoverAsync(string codigo)
        {
            var preparacao = await _planejamentoCustoPreparacaoService.PrepararRemocaoAsync(codigo);
            if (preparacao.TeveFalha)
                return Resultado.Falha(preparacao.Messages);

            var removido = await _planejamentoCustoRepository.RemoverAsync(preparacao.Dados);
            if (!removido)
                return Resultado.Falha(string.Format(PlanejamentoCustoResource.Erro_RemoverPlanejamento, codigo));

            return Resultado.Sucesso().AdicionarMensagem(NotificacoesPadronizadas.MensagemRemocaoSucesso);
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
            return await _planejamentoCustoRelatorioService.ObterRelatorioGeralAsync(ano);
        }

        public async Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioPorCodigoAsync(string codigo)
        {
            return await _planejamentoCustoRelatorioService.ObterRelatorioPorCodigoAsync(codigo);
        }

        private static void AdicionarMensagensDetalhes(Resultado<PlanejamentoCustoDTO> resultado, Resultado resultadoDetalhes)
        {
            foreach (var mensagem in resultadoDetalhes.Messages)
                resultado.Adicionar(mensagem);
        }
    }
}
