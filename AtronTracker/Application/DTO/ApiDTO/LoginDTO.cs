using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Shared.DTO.API;

namespace Application.DTO.ApiDTO
{
    public class LoginDTO
    {
        [ValidateNever]
        public DadosDoTokenDTO UserToken { get; set; }

        public DadosComplementaresDoUsuarioDTO DadosDoUsuario { get; set; }
    }
}