using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Atron.Application.DTO
{
    public class SalarioDTO
    {
        public int Id { get; set; }

        [JsonIgnore]
        public int UsuarioId { get; set; }

        [MaxLength(10, ErrorMessage = "O tamanho máximo do código de usuário é de 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código de cargo é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo código do cargo é obrigatório.")]
        public string UsuarioCodigo { get; set; }

        [ValidateNever][JsonIgnore] public string NomeUsuario { get; set; }
        [ValidateNever][JsonIgnore] public string CargoDescricao { get; set; }
        [ValidateNever][JsonIgnore] public string DepartamentoDescricao { get; set; }

        public int MesId { get; set; }

        [ValidateNever][JsonIgnore] public string MesDescricao { get; set; }

        public string Ano { get; set; }

        [DisplayName("Salário Atual")]
        public int SalarioMensal { get; set; }
    }
}