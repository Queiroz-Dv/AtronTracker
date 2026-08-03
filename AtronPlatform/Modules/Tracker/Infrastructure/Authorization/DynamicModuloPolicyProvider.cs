using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Shared.Authorization;
using System;
using System.Threading.Tasks;

namespace AtronTracker.Infrastructure.Authorization
{
    public class DynamicModuloPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        public DynamicModuloPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string name)
        {
            if (name.StartsWith(ModuloPolicies.Prefixo, StringComparison.OrdinalIgnoreCase))
            {
                var dadosDaPolicy = name[ModuloPolicies.Prefixo.Length..].Split(':', 2);
                var codigoModulo = dadosDaPolicy[0];
                var acao = dadosDaPolicy.Length > 1 ? dadosDaPolicy[1] : ModuloPolicies.AcaoAcessar;
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ModuloRequirement(codigoModulo, acao))
                    .Build();
                return Task.FromResult(policy);
            }
            return base.GetPolicyAsync(name);
        }
    }
}
