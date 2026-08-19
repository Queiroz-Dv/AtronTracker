using Application.DTO;
using Application.Mapping;
using Application.Records.Tarefa;
using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaMovimentacaoMappingTests
{
    private readonly TarefaMovimentacaoMapping _mapping = new();

    [Fact]
    public void MapearParaCriacao_DevePreencherCamposObrigatorios()
    {
        var tarefa = CriarTarefa();
        var responsavel = CriarResponsavel();
        var inicio = DateTime.UtcNow;

        var dto = _mapping.MapearParaCriacao(tarefa, responsavel);

        Assert.Equal(0, dto.Id);
        Assert.Equal(tarefa.Id, dto.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Criacao, dto.TipoMovimentacaoTarefa);
        Assert.Equal("Tarefa criada no estado Em atividade.", dto.Detalhes);
        Assert.Equal(responsavel.Codigo, dto.ResponsavelCodigo);
        Assert.Equal("Maria Silva", dto.ResponsavelNome);
        Assert.InRange(dto.DataOcorrencia, inicio, DateTime.UtcNow);

        var entity = _mapping.MapToEntity(dto);

        Assert.Equal(0, entity.Id);
        Assert.Equal(tarefa.Id, entity.TarefaId);
    }

    [Fact]
    public void MapearParaAtualizacao_DevePreservarContextoEDetalhes()
    {
        var tarefaAnterior = CriarTarefa();
        var tarefaAtual = CriarTarefa();
        tarefaAtual.Titulo = "Título atualizado";
        tarefaAtual.TarefaEstadoId = 5;
        tarefaAtual.EstadoDaTarefa = new TarefaEstado
        {
            Id = 5,
            Descricao = "Iniciada"
        };
        var parametros = new AtualizacaoMovimentacaoRecord(
            tarefaAnterior,
            tarefaAtual,
            CriarResponsavel());

        var dto = _mapping.MapearParaAtualizacao(parametros);

        Assert.Equal(tarefaAtual.Id, dto.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Atualizacao, dto.TipoMovimentacaoTarefa);
        Assert.Contains("Título atualizado.", dto.Detalhes);
        Assert.Contains("Estado alterado de Em atividade para Iniciada.", dto.Detalhes);
        Assert.NotEqual(default, dto.DataOcorrencia);
    }

    [Fact]
    public void MapearParaObtencao_DevePreencherCamposObrigatorios()
    {
        var tarefa = CriarTarefa();
        var responsavel = CriarResponsavel();

        var dto = _mapping.MapearParaObtencao(tarefa, responsavel);

        Assert.Equal(tarefa.Id, dto.TarefaId);
        Assert.Equal(TipoMovimentacaoTarefa.Obtencao, dto.TipoMovimentacaoTarefa);
        Assert.Equal("Tarefa obtida diretamente por Maria Silva.", dto.Detalhes);
        Assert.Equal(responsavel.Codigo, dto.ResponsavelCodigo);
        Assert.Equal("Maria Silva", dto.ResponsavelNome);
        Assert.NotEqual(default, dto.DataOcorrencia);
    }

    [Fact]
    public void MapToDtoEMapToEntity_DevemPreservarIdentificadores()
    {
        var ocorrencia = new DateTime(2026, 8, 18, 12, 0, 0);
        var movimentacao = new TarefaMovimentacao
        {
            Id = 10,
            TarefaId = 42,
            Tipo = TipoMovimentacaoTarefa.Criacao,
            Descricao = "Detalhes",
            ResponsavelCodigo = "USR001",
            ResponsavelNome = "Maria Silva",
            DataOcorrencia = ocorrencia
        };

        var dto = _mapping.MapToDto(movimentacao);
        var entity = _mapping.MapToEntity(dto);

        Assert.Equal(10, dto.Id);
        Assert.Equal(42, dto.TarefaId);
        Assert.Equal(10, entity.Id);
        Assert.Equal(42, entity.TarefaId);
    }

    private static Tarefa CriarTarefa()
    {
        return new Tarefa
        {
            Id = 42,
            Titulo = "Título original",
            Conteudo = "Conteúdo",
            DataInicial = new DateTime(2026, 8, 18),
            DataFinal = new DateTime(2026, 8, 20),
            TarefaEstadoId = 1,
            EstadoDaTarefa = new TarefaEstado
            {
                Id = 1,
                Descricao = "Em atividade"
            }
        };
    }

    private static Usuario CriarResponsavel()
    {
        return new Usuario
        {
            Id = 7,
            Codigo = "USR001",
            Nome = "Maria",
            Sobrenome = "Silva"
        };
    }
}
