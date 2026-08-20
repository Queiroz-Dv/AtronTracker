using Domain.Entities;

namespace Domain.Extensions
{
    public static class TarefaObtencaoExtensions
    {
        public static string ObterEstado(this Tarefa tarefa)
        {
            return tarefa.EstadoDaTarefa?.Descricao ??
                   tarefa.TarefaEstadoId.ToString();
        }
    }
}