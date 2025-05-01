using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace Atron.WebApi.Helpers
{
    public class ModuloHandler : AuthorizationHandler<ModuloRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModuloRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == "modulo" && c.Value == requirement.Codigo))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}