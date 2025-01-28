using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class TarefaDTO
    {
        public TarefaDTO() { }

        public TarefaDTO(int id,
            string titulo,
            string conteudo,
            DateTime dataInicial,
            DateTime dataFinal,
            string usuarioCodigo)
        {
            Id = id;
            Titulo = titulo;
            Conteudo = conteudo;
            DataInicial = dataInicial;
            DataFinal = dataFinal;
            UsuarioCodigo = usuarioCodigo;
        }

        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }
        public UsuarioDTO Usuario { get; set; }

        [DisplayName("Título")]
        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        [DisplayName("Data inicial")]
        public DateTime DataInicial { get; set; }

        [DisplayName("Data final")]
        public DateTime DataFinal { get; set; }

        public int TarefaEstadoId { get; set; }

        [DisplayName("Estado")]
        public string EstadoDaTarefaDescricao { get; set; }
    }
}