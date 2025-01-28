using System;

namespace Atron.Domain.Entities
{
    public class Tarefa : EntityBase
    {
        public Tarefa() { }

        public Tarefa(int usuarioId,
                      string usuarioCodigo,
                      string titulo,
                      string conteudo,
                      DateTime dataInicial,
                      DateTime dataFinal,
                      int tarefaEstadoId)
        {
            UsuarioId = usuarioId;
            UsuarioCodigo = usuarioCodigo;
            Titulo = titulo;
            Conteudo = conteudo;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            TarefaEstadoId = tarefaEstadoId;
        }

        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }

        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int TarefaEstadoId { get; set; }

        public Usuario Usuario { get; set; }
    }
}