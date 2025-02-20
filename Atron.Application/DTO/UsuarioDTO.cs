using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class UsuarioDTO
    {
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int Id { get; set; }

        [DisplayName("Código")]
        [MaxLength(10, ErrorMessage = "O tamanho máximo do código é de 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo Código é obrigatório.")]
        public string Codigo { get; set; }

        [MaxLength(25, ErrorMessage = "O tamanho máximo do nome é de 25 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do nome é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo nome é obrigatório.")]
        public string Nome { get; set; }

        [MaxLength(50, ErrorMessage = "O tamanho máximo do sobrenome é de 50 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do sobrenome é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo sobrenome é obrigatório.")]
        public string Sobrenome { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DataNascimento { get; set; }

        [DisplayName("Salário")]
        public int? Salario { get; set; }

        [MaxLength(10, ErrorMessage = "O tamanho máximo do código de cargo é de 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código de cargo é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo código do cargo é obrigatório.")]
        public string CargoCodigo { get; set; }

        [MaxLength(10, ErrorMessage = "O tamanho máximo do código de departamento é de 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código de departamento é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo código do departamento é obrigatório.")]
        public string DepartamentoCodigo { get; set; }

        [DataType(DataType.EmailAddress)]
        [DisplayName("E-mail")]
        [Required(ErrorMessage = "O campo e-mail é obrigatório")]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Senha { get; set; }

        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int DepartamentoId { get; set; }
        [JsonIgnore]
        [SwaggerSchema(ReadOnly = true, WriteOnly = true)]
        public int CargoId { get; set; }

        public DepartamentoDTO Departamento { get; set; }

        public CargoDTO Cargo { get; set; }

        public string NomeCompleto()
        {
            return $"{Nome} {Sobrenome}";
        }
    }
}