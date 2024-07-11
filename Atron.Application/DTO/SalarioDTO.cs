using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class SalarioDTO
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int UsuarioId { get; set; }

        public UsuarioDTO Usuario { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int MesId { get; set; }

        public MesDTO Mes { get; set; }

        public DateTime? Ano { get; set; }

        public int SalarioMensal { get; set; }
    }
}