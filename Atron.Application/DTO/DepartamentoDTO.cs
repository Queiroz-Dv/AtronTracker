using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class DepartamentoDTO : Factory
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public Guid IdSequencial { get; set; }

        [Required(ErrorMessage = "O campo código é obrigatório")]
        [MinLength(3, ErrorMessage = "O campo código deve conter 3 caracteres ou mais.")]
        [MaxLength(10, ErrorMessage = "O campo código deve conter até 10 caracteres ou menos.")]
        [DisplayName("Código")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "O campo descrição é obrigatório")]
        [MinLength(3, ErrorMessage = "O campo descrição deve conter 3 caracteres ou mais.")]
        [MaxLength(50, ErrorMessage = "O campo descrição deve conter até 50 caracteres ou menos.")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int CargoId { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public string CargoCodigo { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public string CargoDescricao { get; set; }
    }
}