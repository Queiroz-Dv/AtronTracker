using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class TarefaDTO : Factory
    {       
        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }
        public UsuarioDTO Usuario { get; set; }        

        public string Titulo { get; set; }

        public string Conteudo { get; set; }

        public DateTime DataInicial { get; set; }

        public DateTime DataFinal { get; set; }

        public int EstadoDaTarefa { get; set; }

        public string EstadoDaTarefaDescricao { get; set; }
    }
}