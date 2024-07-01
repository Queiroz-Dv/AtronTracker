using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class PermissaoDTO
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int UsuarioId { get; set; }

        public string UsuarioCodigo { get; set; }

        public UsuarioDTO Usuario { get; set; }

        public DateTime DataInicial { get; set; }
        public DateTime DataFinal { get; set; }
        [JsonIgnore]

        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int PermissaoEstadoId { get; set; }
        public string PermissaoEstadoDescricao { get; set; }
        public string Descricao { get; set; }
        public int QuantidadeDeDias { get; set; }
    }
}