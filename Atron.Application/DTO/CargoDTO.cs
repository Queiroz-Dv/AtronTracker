using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class CargoDTO : Factory
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [Required(ErrorMessage = "O campo código é obrigatório", AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "O campo código deve conter 3 caracteres ou mais.")]
        [MaxLength(10, ErrorMessage = "O campo código deve conter até 10 caracteres ou menos.")]
        [DisplayName("Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "O campo descrição é obrigatório", AllowEmptyStrings = false)]
        [MinLength(3, ErrorMessage = "O campo descrição deve conter 3 caracteres ou mais.")]
        [MaxLength(50, ErrorMessage = "O campo descrição deve conter até 50 caracteres ou menos.")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

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