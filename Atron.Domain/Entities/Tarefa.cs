using System;

namespace Atron.Domain.Entities
{
    public class Tarefa
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string UsuarioCodigo { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataEntrega { get; set; }
        public int TarefaEstado { get; set; }
    }
}