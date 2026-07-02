using Microsoft.AspNetCore.Authorization;

namespace WebApi.Helpers
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
