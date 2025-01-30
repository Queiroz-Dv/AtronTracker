using Shared.DTO.API;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class LoginDTO : FactoryDTO
    {
        [DisplayName("Usuário")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "Senha necessária")]     
        [DataType(DataType.Password)]
        public string Passsword { get; set; }

        public string ReturnUrl { get; set; }

        public string ControllerName { get; set; }
        public string ControllerAction { get; set; }
        public bool Authenticated { get; set; }
        public UserToken UserToken { get; set; }
    }
}