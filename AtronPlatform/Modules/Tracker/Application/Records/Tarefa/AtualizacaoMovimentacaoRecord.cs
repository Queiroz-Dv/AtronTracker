namespace Application.Records.Tarefa
{
    public sealed record AtualizacaoMovimentacaoRecord(
        global::Domain.Entities.Tarefa TarefaAnterior,
        global::Domain.Entities.Tarefa TarefaAtual,
        global::Domain.Entities.Usuario Responsavel);
}
