using Application.DTO;
using Application.Interfaces.Services;
using Application.UseCases.PlanejamentoCustoCases;
using Shared.Domain.ValueObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Services.EntitiesServices
{
    public class PlanejamentoCustoService(
        CriarPlanejamentoCustoCase criarPlanejamentoCusto,
        AtualizarPlanejamentoCustoCase atualizarPlanejamentoCusto,
        ExcluirPlanejamentoCustoCase excluirPlanejamentoCusto,
        ObterPlanejamentoCustoCase obterPlanejamentoCusto,
        IPlanejamentoCustoRelatorioService planejamentoCustoRelatorioService) : IPlanejamentoCustoService
    {
        private readonly CriarPlanejamentoCustoCase _criarPlanejamentoCusto = criarPlanejamentoCusto;
        private readonly AtualizarPlanejamentoCustoCase _atualizarPlanejamentoCusto = atualizarPlanejamentoCusto;
        private readonly ExcluirPlanejamentoCustoCase _excluirPlanejamentoCusto = excluirPlanejamentoCusto;
        private readonly ObterPlanejamentoCustoCase _obterPlanejamentoCusto = obterPlanejamentoCusto;
        private readonly IPlanejamentoCustoRelatorioService _planejamentoCustoRelatorioService = planejamentoCustoRelatorioService;

        public Task<Resultado> CriarAsync(PlanejamentoCustoDTO planejamentoCustoDTO)
            => _criarPlanejamentoCusto.ExecutarAsync(planejamentoCustoDTO);

        public Task<Resultado> AtualizarAsync(string codigo, PlanejamentoCustoDTO planejamentoCustoDTO)
            => _atualizarPlanejamentoCusto.ExecutarAsync(codigo, planejamentoCustoDTO);

        public Task<Resultado> RemoverAsync(string codigo)
            => _excluirPlanejamentoCusto.ExecutarAsync(codigo);

        public Task<Resultado<PlanejamentoCustoDTO>> ObterPorCodigoAsync(string codigo)
            => _obterPlanejamentoCusto.ObterPorCodigoAsync(codigo);

        public Task<Resultado<List<PlanejamentoCustoDTO>>> ObterPorAnoAsync(int ano)
            => _obterPlanejamentoCusto.ObterPorAnoAsync(ano);

        public Task<Resultado<List<PlanejamentoCustoDTO>>> ObterTodosAsync()
            => _obterPlanejamentoCusto.ObterTodosAsync();

        public Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano)
            => _planejamentoCustoRelatorioService.ObterRelatorioGeralAsync(ano);

        public Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioPorCodigoAsync(string codigo)
            => _planejamentoCustoRelatorioService.ObterRelatorioPorCodigoAsync(codigo);
    }
}
