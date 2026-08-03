using Microsoft.AspNetCore.Authorization;

namespace AtronTracker.Infrastructure.Authorization
{
    public class ModuloRequirement : IAuthorizationRequirement
    {
        public string Codigo { get; }
        public string Acao { get; }

        public ModuloRequirement(string codigo, string acao)
        {
            Codigo = codigo;
            Acao = acao;
        }
    }
}
