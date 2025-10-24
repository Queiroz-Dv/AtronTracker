using Microsoft.AspNetCore.Authorization;

namespace Atron.Tracker.WebApi.Helpers
{
    public class ModuloRequirement : IAuthorizationRequirement
    {
        public string Codigo { get; }

        public ModuloRequirement(string codigo) => Codigo = codigo;
    }
}
