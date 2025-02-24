using Atron.Application.DTO.ControleDeAcessoDTO;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO
{
    public class ModuloDTO
    {
        public int Id { get; set; }

        [MaxLength(10, ErrorMessage = "O tamanho máximo do código é de até 10 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do código é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo Código é obrigatório.")]
        [DisplayName("Código")]
        public string Codigo { get; set; }


        [MaxLength(50, ErrorMessage = "O tamanho máximo da descrição é de até 50 caracteres.")]
        [MinLength(3, ErrorMessage = "O tamanho mínimo do descrição é de 3 caracteres.")]
        [Required(ErrorMessage = "O campo Descrição é obrigatório.")]
        [DisplayName("Descrição")]
        public string Descricao { get; set; }

        public ICollection<PropriedadesDeAcessoDTO> Propriedades { get; set; }
    }
}
