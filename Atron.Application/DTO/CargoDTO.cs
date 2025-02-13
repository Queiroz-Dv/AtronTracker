using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO
{
    public class CargoDTO
    {
        public CargoDTO() { }

        public CargoDTO(string codigo, string descricao)
        {
            Codigo = codigo.ToUpper();
            Descricao = descricao.ToUpper();
        }

        public int Id { get; set; }

        [MaxLength(10, ErrorMessage = "O tamanho máximo do código é de até 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo Código é obrigatório.")]
        [DisplayName("Código")]
        public string Codigo { get; set; }

        [MaxLength(50, ErrorMessage = "O tamanho máximo do código é de até 50 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        public int DepartamentoId { get; set; }
        [MaxLength(10, ErrorMessage = "Código do departamento é maior que 10 caracteres.")]
        [Required(ErrorMessage = "Um departamento precisa ser selecionado")]
        public string DepartamentoCodigo { get; set; }
        public string DepartamentoDescricao { get; set; }
        [DisplayName("Departamentos")]
        public DepartamentoDTO Departamento { get; set; }

        public string ObterCodigoComDescricao()
        {
            return $"{Codigo} - {Descricao}";
        }
    }
}