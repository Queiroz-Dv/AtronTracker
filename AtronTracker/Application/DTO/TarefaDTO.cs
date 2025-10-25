using System;

namespace Application.DTO
{
    public class TarefaDTO
    {
        public int Id { get; set; }

        public string UsuarioCodigo { get; set; }

        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public UsuarioDTO Usuario { get; set; }

        public TarefaEstadoDTO EstadoDaTarefa { get; set; }
    }
}