using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shared.DTO.API;
using System.ComponentModel.DataAnnotations;

namespace Atron.Application.DTO.ApiDTO
{
    public class LoginDTO
    {              
        [ValidateNever]
        public bool Authenticated { get; set; }

        [ValidateNever]
        public UserToken UserToken { get; set; }

        public DadosDoUsuario DadosDoUsuario { get; set; }
    }
}