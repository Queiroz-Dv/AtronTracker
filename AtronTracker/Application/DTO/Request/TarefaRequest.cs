using System;

namespace Application.DTO.Request
{
    public class TarefaRequest
    {
        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public string UsuarioCodigo { get; set; }

        public int TarefaEstadoId { get; set; }
    }
}