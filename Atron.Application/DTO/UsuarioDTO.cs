using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class UsuarioDTO : Factory
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        public string Codigo { get; set; }
        public string Nome { get; set; }
        public string Sobrenome { get; set; }
        public DateTime? DataNascimento { get; set; }
        public int Salario { get; set; }
        public string CargoCodigo { get; set; }
        public string DepartamentoCodigo { get; set; }


        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int DepartamentoId { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int CargoId { get; set; }
        
        public DepartamentoDTO Departamento { get; set; }
        
        public CargoDTO Cargo { get; set; }
    }
}