using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class CargoDTO : FactoryDTO
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int DepartamentoId { get; set; }

        [Required(ErrorMessage = "Um departamento precisa ser selecionado")]
        public string DepartamentoCodigo { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public string DepartamentoDescricao { get; set; }

        [DisplayName("Departamentos")]
        public DepartamentoDTO Departamento { get; set; }        
    }
}