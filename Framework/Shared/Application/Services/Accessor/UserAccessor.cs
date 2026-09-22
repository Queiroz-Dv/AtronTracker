using Microsoft.AspNetCore.Http;
using Shared.Application.DTOS.Auth;
using Shared.Application.Interfaces.Service;
using Shared.Extensions;
using System.Security.Claims;

namespace Shared.Application.Services.Accessor
{
    public class UserAccessor : IUserAccessor
    {
        private readonly IHttpContextAccessor _accessor;

        public UserAccessor(IHttpContextAccessor accessor) => _accessor = accessor;

        public string ObterLogadoUsuario()
        {
            if (_accessor.HttpContext.IsNullable()) return "Sistema";

            var user = _accessor.HttpContext.User;

            if (!user.IsNullable() && user.Identity.IsAuthenticated)
            {
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                if (!email.IsNullOrEmpty()) return email;

                var name = user.Identity.Name;
                if (!name.IsNullOrEmpty()) return name;

                return "Usuario_Sem_Nome";
            }

            return "Anonimo";
        }

        public string ObterCodigoUsuarioLogado()
        {
            var user = _accessor.HttpContext?.User;
            if (user?.Identity?.IsAuthenticated != true)
                return string.Empty;

            return user.FindFirst(ClaimCode.CODIGO_USUARIO)?.Value ?? string.Empty;
        }
    }
}
