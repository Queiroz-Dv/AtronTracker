using Microsoft.AspNetCore.Authorization;

namespace Infrastructure.Authorization
{
    public class ModuloRequirement : IAuthorizationRequirement
    {
        public string Codigo { get; }        

        public ModuloRequirement(string codigo)
        {
            Codigo = codigo;
        }
    }
}