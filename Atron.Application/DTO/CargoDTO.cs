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

        [Required(ErrorMessage = "Código requerido")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Descrição requerida")]
        public string Descricao { get; set; }

        public int DepartamentoId { get; set; }
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