using Application.Extensions;
using Application.Records.Tarefa;
using Application.Resources;
using Domain.Entities;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaMovimentacaoDetalhesExtensionsTests
{
    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDoTitulo()
    {
        var parametros = CriarParametros();
        parametros.TarefaAtual.Titulo = "Título atualizado";

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(TarefaResource.Historico_DetalheTituloAtualizado, detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDoConteudo()
    {
        var parametros = CriarParametros();
        parametros.TarefaAtual.Conteudo = "Conteúdo atualizado";

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(TarefaResource.Historico_DetalheConteudoAtualizado, detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDoPeriodo()
    {
        var parametros = CriarParametros();
        parametros.TarefaAtual.DataInicial = new DateTime(2026, 9, 1);
        parametros.TarefaAtual.DataFinal = new DateTime(2026, 9, 15);

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalhePeriodoAlterado,
                parametros.TarefaAnterior.DataInicial,
                parametros.TarefaAnterior.DataFinal,
                parametros.TarefaAtual.DataInicial,
                parametros.TarefaAtual.DataFinal),
            detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDoEstado()
    {
        var parametros = CriarParametros();
        parametros.TarefaAtual.TarefaEstadoId = 5;
        parametros.TarefaAtual.EstadoDaTarefa = new TarefaEstado
        {
            Id = 5,
            Descricao = "Iniciada"
        };

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheEstadoAlterado,
                "Em atividade",
                "Iniciada"),
            detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDoResponsavel()
    {
        var parametros = CriarParametros();
        parametros.TarefaAnterior.UsuarioCodigo = null;
        parametros.TarefaAtual.UsuarioCodigo = "USR002";

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(
            string.Format(
                TarefaResource.Historico_DetalheResponsavelAlterado,
                TarefaResource.Historico_ValorNaoInformado,
                "USR002"),
            detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDoEscopo()
    {
        var parametros = CriarParametros();
        parametros.TarefaAtual.DepartamentoCodigo = "DPT002";

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(TarefaResource.Historico_DetalheEscopoAtualizado, detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveDescreverAlteracaoDaExigenciaDeAprovacao()
    {
        var parametros = CriarParametros();
        parametros.TarefaAtual.ExigeAprovacaoParaObter = true;

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(TarefaResource.Historico_DetalheAprovacaoAtualizada, detalhes);
    }

    [Fact]
    public void CriarDetalhesDaAtualizacao_DeveUsarMensagemGenericaSemDiferencaEspecifica()
    {
        var parametros = CriarParametros();

        var detalhes = parametros.CriarDetalhesDaAtualizacao();

        Assert.Equal(TarefaResource.Historico_DetalheAtualizacao, detalhes);
    }

    private static AtualizacaoMovimentacaoRecord CriarParametros()
    {
        return new AtualizacaoMovimentacaoRecord(
            CriarTarefa(),
            CriarTarefa(),
            new Usuario
            {
                Id = 7,
                Codigo = "USR001",
                Nome = "Maria",
                Sobrenome = "Silva"
            });
    }

    private static Tarefa CriarTarefa()
    {
        return new Tarefa
        {
            Id = 42,
            Titulo = "Título original",
            Conteudo = "Conteúdo original",
            DataInicial = new DateTime(2026, 8, 18),
            DataFinal = new DateTime(2026, 8, 20),
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado { Id = 1, Descricao = "Em atividade" },
            UsuarioCodigo = "USR001",
            DestinoInicial = 1,
            DepartamentoCodigo = "DPT001",
            CargoCodigo = "CRG001",
            ExigeAprovacaoParaObter = false
        };
    }
}
