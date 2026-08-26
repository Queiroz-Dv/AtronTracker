using Domain.Entities;
using Domain.Enums;
using Xunit;

namespace Tracker.Tests.Tarefas;

public class TarefaTests
{
    [Fact]
    public void AprovarObtencao_DeveIniciarTarefaPendenteEDesmarcarExigenciaDeAprovacao()
    {
        var tarefa = new Tarefa
        {
            TarefaEstadoId = 2,
            ExigeAprovacaoParaObter = true,
            DestinoInicial = (int)DestinoInicialTarefa.DepartamentoCargo,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT-10",
            Departamento = new Departamento { Id = 10, Codigo = "DPT-10" },
            CargoId = 20,
            CargoCodigo = "CRG-20",
            Cargo = new Cargo { Id = 20, Codigo = "CRG-20" }
        };

        tarefa.AprovarObtencao(usuarioId: 42, usuarioCodigo: "USR-42");

        Assert.Equal(42, tarefa.UsuarioId);
        Assert.Equal("USR-42", tarefa.UsuarioCodigo);
        Assert.Equal((int)DestinoInicialTarefa.Usuario, tarefa.DestinoInicial);
        Assert.Null(tarefa.DepartamentoId);
        Assert.Null(tarefa.DepartamentoCodigo);
        Assert.Null(tarefa.Departamento);
        Assert.Null(tarefa.CargoId);
        Assert.Null(tarefa.CargoCodigo);
        Assert.Null(tarefa.Cargo);
        Assert.Equal(5, tarefa.TarefaEstadoId);
        Assert.False(tarefa.ExigeAprovacaoParaObter);
    }

    [Fact]
    public void AprovarObtencao_DevePreservarEstadoEFlagQuandoTarefaNaoEstiverPendente()
    {
        var tarefa = new Tarefa
        {
            TarefaEstadoId = 1,
            ExigeAprovacaoParaObter = true,
            DestinoInicial = (int)DestinoInicialTarefa.DepartamentoCargo,
            DepartamentoId = 10,
            DepartamentoCodigo = "DPT-10",
            CargoId = 20,
            CargoCodigo = "CRG-20"
        };

        tarefa.AprovarObtencao(usuarioId: 42, usuarioCodigo: "USR-42");

        Assert.Equal(42, tarefa.UsuarioId);
        Assert.Equal("USR-42", tarefa.UsuarioCodigo);
        Assert.Equal((int)DestinoInicialTarefa.Usuario, tarefa.DestinoInicial);
        Assert.Null(tarefa.DepartamentoId);
        Assert.Null(tarefa.DepartamentoCodigo);
        Assert.Null(tarefa.CargoId);
        Assert.Null(tarefa.CargoCodigo);
        Assert.Equal(1, tarefa.TarefaEstadoId);
        Assert.True(tarefa.ExigeAprovacaoParaObter);
    }
}
