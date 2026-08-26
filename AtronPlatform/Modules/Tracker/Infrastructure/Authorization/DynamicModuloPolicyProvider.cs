using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Shared.Authorization;

namespace Infrastructure.Authorization
{
    public class DynamicModuloPolicyProvider(IOptions<AuthorizationOptions> options) :
        DefaultAuthorizationPolicyProvider(options)
    {
        public override Task<AuthorizationPolicy> GetPolicyAsync(string name)
        {
            if (name.StartsWith(ModuloPolicies.Prefixo, StringComparison.OrdinalIgnoreCase))
            {
                var dadosDaPolicy = name[ModuloPolicies.Prefixo.Length..].Split(':', 2);
                var codigoModulo = dadosDaPolicy[0];
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ModuloRequirement(codigoModulo))
                    .Build();

                return Task.FromResult(policy);
            }
            return base.GetPolicyAsync(name);
        }
    }
}
