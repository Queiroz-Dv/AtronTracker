using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace Atron.WebApi.Helpers
{
    public class DynamicModuloPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        const string Prefixo = "Modulo:";
        public DynamicModuloPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
        }

        public override Task<AuthorizationPolicy> GetPolicyAsync(string name)
        {
            if (name.StartsWith(Prefixo, StringComparison.OrdinalIgnoreCase))
            {
                var code = name[Prefixo.Length..];
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .AddRequirements(new ModuloRequirement(code))
                    .Build();
                return Task.FromResult(policy);
            }
            return base.GetPolicyAsync(name);
        }
    }
}
