using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO
{
    public class DepartamentoDTO
    {
        public int Id { get; set; }

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

        public int CargoId { get; set; }
        public string CargoCodigo { get; set; }
        public string CargoDescricao { get; set; }
    }
}