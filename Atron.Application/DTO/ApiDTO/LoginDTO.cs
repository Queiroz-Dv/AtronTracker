using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class LoginDTO : FactoryDTO
    {
        [Required(ErrorMessage = "Email necessário")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha necessária")]
        [MinLength(10, ErrorMessage = "Minímo de 10 caracteres")]
        [MaxLength(20, ErrorMessage = "Máximo de 20 caracteres")]
        [DataType(DataType.Password)]
        public string Passsword { get; set; }

        public string ReturnUrl { get; set; }

        public string ControllerName { get; set; }
        public string ControllerAction { get; set; }
        public bool Authenticated { get; set; }
    }
}
