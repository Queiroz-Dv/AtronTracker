using Application.DTO;
using Domain.Entities;

namespace Application.Services.EntitiesServices.Tarefas
{
    public class TarefaPreparada
    {
        public TarefaPreparada(TarefaDTO dto, Tarefa entidade, Usuario usuario)
        {
            Dto = dto;
            Entidade = entidade;
            Usuario = usuario;
        }

        public TarefaDTO Dto { get; }

        public Tarefa Entidade { get; }

        public Usuario Usuario { get; }
    }
}
