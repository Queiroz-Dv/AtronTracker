using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        [DisplayName("Título")]
        public string Titulo { get; set; }

        public string Conteudo { get; set; }
        
        [DisplayName("Data inicial")]
        public DateTime DataInicial { get; set; }

        [DisplayName("Data final")]
        public DateTime DataFinal { get; set; }

        public int EstadoDaTarefa { get; set; }

        [DisplayName("Estado")]
        public string EstadoDaTarefaDescricao { get; set; }
    }
}