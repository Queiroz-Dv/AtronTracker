using Microsoft.AspNetCore.Authorization;

namespace Atron.WebApi.Helpers
{
    public class ModuloRequirement : IAuthorizationRequirement
    {
        public string Codigo { get; }

        public ModuloRequirement(string codigo) => Codigo = codigo;
    }
}
