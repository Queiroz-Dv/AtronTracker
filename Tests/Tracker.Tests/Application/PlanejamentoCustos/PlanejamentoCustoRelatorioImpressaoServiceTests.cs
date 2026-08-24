using Application.DTO;
using Application.Interfaces.Services;
using Application.Services.EntitiesServices.PlanejamentoCustos;
using Shared.Application.Resources;
using Shared.Domain.ValueObjects;
using Xunit;

namespace Tracker.Tests.PlanejamentoCustos;

public class PlanejamentoCustoRelatorioImpressaoServiceTests
{
    [Fact]
    public async Task ObterHtmlRelatorioGeralAsync_DeveGerarHtmlComDadosEscapadosEInformacoesDoRelatorio()
    {
        var relatorio = new PlanejamentoCustoRelatorioGeralDTO
        {
            Ano = DateTime.Today.Year,
            Departamentos =
            [
                new PlanejamentoCustoRelatorioDepartamentoDTO
                {
                    DepartamentoCodigo = "DPT",
                    DepartamentoDescricao = "Departamento <Operacional>",
                    PlanejamentoCodigo = "PLC001",
                    PlanejamentoDescricao = "Planejamento & Revisao",
                    ValorMinimoDepartamento = 100,
                    ValorTetoDepartamento = 500,
                    CargosPendentes = ["CRG2 - Cargo <Novo>"],
                    CargosDetalhados =
                    [
                        new PlanejamentoCustoRelatorioCargoDTO
                        {
                            CargoCodigo = "CRG1",
                            CargoDescricao = "Cargo & Analise",
                            ValorMinimo = 80,
                            ValorTeto = 300,
                            PercentualOcupacaoTetoDepartamento = 60
                        }
                    ]
                }
            ]
        };

        var service = new PlanejamentoCustoRelatorioImpressaoService(
            new PlanejamentoCustoRelatorioServiceFake(relatorio));

        var resultado = await service.ObterHtmlRelatorioGeralAsync(relatorio.Ano);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Contains("<html lang=\"pt-BR\">", resultado.Dados);
        Assert.Contains("Relatório de Planejamentos", resultado.Dados);
        Assert.Contains($"Ano analisado: {relatorio.Ano}", resultado.Dados);
        Assert.Contains("DPT - Departamento &lt;Operacional&gt;", resultado.Dados);
        Assert.Contains("PLC001 - Planejamento &amp; Revisao", resultado.Dados);
        Assert.Contains("CRG2 - Cargo &lt;Novo&gt;", resultado.Dados);
        Assert.Contains("CRG1 - Cargo &amp; Analise", resultado.Dados);
        Assert.Contains("60%", resultado.Dados);
    }

    [Fact]
    public async Task ObterHtmlRelatorioGeralAsync_DeveInformarQuandoNaoHouverPlanejamento()
    {
        var ano = DateTime.Today.Year;
        var service = new PlanejamentoCustoRelatorioImpressaoService(
            new PlanejamentoCustoRelatorioServiceFake(new PlanejamentoCustoRelatorioGeralDTO { Ano = ano }));

        var resultado = await service.ObterHtmlRelatorioGeralAsync(ano);

        Assert.True(resultado.TeveSucesso);
        Assert.NotNull(resultado.Dados);
        Assert.Contains("Nenhum planejamento encontrado para o ano informado.", resultado.Dados);
    }

    [Fact]
    public async Task ObterHtmlRelatorioPorCodigoAsync_DeveRepassarFalhaDoRelatorioAnalitico()
    {
        var service = new PlanejamentoCustoRelatorioImpressaoService(
            new PlanejamentoCustoRelatorioServiceFake(
                Resultado<PlanejamentoCustoRelatorioGeralDTO>.Falha(NotificacoesPadronizadas.ErroRegistroNaoEncontrado)));

        var resultado = await service.ObterHtmlRelatorioPorCodigoAsync("PLC999");

        Assert.True(resultado.TeveFalha);
        Assert.Contains(
            resultado.Messages,
            mensagem => mensagem.Descricao == NotificacoesPadronizadas.ErroRegistroNaoEncontrado);
    }

    private sealed class PlanejamentoCustoRelatorioServiceFake : IPlanejamentoCustoRelatorioService
    {
        private readonly Resultado<PlanejamentoCustoRelatorioGeralDTO> _resultado;

        public PlanejamentoCustoRelatorioServiceFake(PlanejamentoCustoRelatorioGeralDTO relatorio)
        {
            _resultado = Resultado<PlanejamentoCustoRelatorioGeralDTO>.Sucesso(relatorio);
        }

        public PlanejamentoCustoRelatorioServiceFake(Resultado<PlanejamentoCustoRelatorioGeralDTO> resultado)
        {
            _resultado = resultado;
        }

        public Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioGeralAsync(int ano)
            => Task.FromResult(_resultado);

        public Task<Resultado<PlanejamentoCustoRelatorioGeralDTO>> ObterRelatorioPorCodigoAsync(string codigo)
            => Task.FromResult(_resultado);
    }
}
