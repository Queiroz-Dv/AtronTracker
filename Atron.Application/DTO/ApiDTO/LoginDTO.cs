using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shared.DTO.API;

namespace Atron.Application.DTO.ApiDTO
{
    public class LoginDTO
    {
        [ValidateNever]
        public InfoToken UserToken { get; set; }

        public DadosDoUsuario DadosDoUsuario { get; set; }
    }
}