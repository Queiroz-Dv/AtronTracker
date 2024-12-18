using System.Collections.Generic;

namespace Atron.Domain.Entities
{
    public class TarefaEstado
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public List<Tarefa> Tarefas { get; set; }
    }
}