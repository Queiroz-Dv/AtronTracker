using System;

namespace Atron.Domain.Entities
{
    public class Tarefa : EntityBase
    {
        public long UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }

        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int EstadoDaTarefaId { get; set; }
        public TarefaEstado TarefaEstado { get; set; }
    }
}