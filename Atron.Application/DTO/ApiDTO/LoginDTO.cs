using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shared.DTO.API;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage ="Código de usuário é obrigatório")]
        public string Codigo { get; set; }

        [Required(ErrorMessage = "Senha necessária")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [ValidateNever]
        public bool Authenticated { get; set; }

        [ValidateNever]
        public UserToken UserToken { get; set; }
    }
}