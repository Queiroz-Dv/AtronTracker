using Shared.DTO.API;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class LoginDTO
    {
        public string Codigo { get; set; }
        [Required(ErrorMessage = "Senha necessária")]
        [DataType(DataType.Password)]
        public string Passsword { get; set; }

        public bool Authenticated { get; set; }
        public UserToken UserToken { get; set; }
    }
}