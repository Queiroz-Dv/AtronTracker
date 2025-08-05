using System;

namespace Atron.Application.DTO.Response
{
    public class TarefaResponse
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Conteudo { get; set; }
        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        public TarefaEstadoDTO EstadoDaTarefa { get; set; }
        public UsuarioRecord Usuario { get; set; }
    }

    public record UsuarioRecord
    {
        public string UsuarioCodigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }

        public string CodigoDepartamento { get; set; }
        public string DescricaoDepartamento { get; set; }

        public string CodigoCargo { get; set; }
        public string DescricaoCargo { get; set; }
    }
}
