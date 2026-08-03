using Application.DTO;
using Domain.Entities;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaPreparada(TarefaDTO tarefaDTO, Tarefa tarefa, Usuario usuario)
    {
        public TarefaDTO TarefaDTO { get; } = tarefaDTO;

        public Tarefa Tarefa { get; } = tarefa;

        public Usuario Usuario { get; } = usuario;
    }
}
